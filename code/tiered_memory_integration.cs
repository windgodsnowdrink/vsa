#:sdk Microsoft.NET.Sdk.Web
#:package TieredMemory@2.0.0
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Runtime.CompilerServices;
using TieredMemory;

var builder = WebApplication.CreateBuilder(args);

// 1. 分层内存服务配置（适配16GB内存）
builder.Services.AddTieredMemory(t => {
    // 热层：4GB内存（占总内存25%），线程本地分配，缓存行对齐
    t.AddHotLayer(4.GB, new HotLayerOptions {
        AllocationStrategy = AllocationStrategy.ThreadLocal,
        CacheLineSize = 64,
        PreWarm = true,
        MaxItemSize = 1.MB  // 限制单个对象最大1MB
    });
    
    // 温层：8GB SSD存储（占总内存50%），LZ4压缩
    t.AddWarmLayer(8.GB, new WarmLayerOptions {
        Compression = MemoryCompression.LZ4,
        EvictionPolicy = EvictionPolicy.LRU,
        BatchSize = 1000
    });
    
    // 冷层：4GB HDD存储（占总内存25%），批量持久化
    t.AddColdLayer(4.GB, new ColdLayerOptions {
        PersistenceInterval = 1.Seconds(),  // 降低刷盘频率
        BatchSize = 10_000,
        Compression = MemoryCompression.Zstd
    });
});

// 2. 内存监控服务,m高性能数据处理服务
builder.Services.AddSingleton<DataProcessingService>();
builder.Services.AddHostedService<MemoryMonitorService>();

var app = builder.Build();
app.MapGet("/", () => "Tiered Memory Service (16GB Optimized)");
app.Run();

// 高性能数据处理服务（内存优化版）
public sealed class DataProcessingService
{
    private readonly ITieredMemory _memory;
    private readonly ThreadLocal<Span<byte>> _buffer;

    public DataProcessingService(ITieredMemory memory)
    {
        _memory = memory;
        _buffer = new ThreadLocal<Span<byte>>(() => stackalloc byte[64]); // 减小线程本地缓冲区
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Process(in ReadOnlySpan<byte> data)
    {
        // 使用更小的处理窗口
        const int windowSize = 1024;
        for (int i = 0; i < data.Length; i += windowSize)
        {
            var window = data.Slice(i, Math.Min(windowSize, data.Length - i));
            // 零拷贝处理
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessDataAsync(ReadOnlyMemory<byte> input)
    {
        // 热层处理
        var hotBuffer = _buffer.Value;
        input.Span.CopyTo(hotBuffer);
        
        // 温层处理
        await _memory.WarmLayer.WriteAsync("data_key", input);
        
        // 冷层处理
        await _memory.ColdLayer.ArchiveAsync("data_key", input);
    }

    [SkipLocalsInit]
    private unsafe void ProcessHotData(Span<byte> data)
    {
        fixed (byte* ptr = data)
        {
            // 零拷贝处理
        }
    }
}

// 内存监控服务
public class MemoryMonitorService : BackgroundService
{
    private readonly ITieredMemory _memory;
    private readonly ILogger<MemoryMonitorService> _logger;

    public MemoryMonitorService(ITieredMemory memory, ILogger<MemoryMonitorService> logger)
    {
        _memory = memory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var stats = _memory.GetStatistics();
            _logger.LogInformation(
                "Memory Stats - Hot: {HotUsage}/{HotCapacity} | Warm: {WarmUsage}/{WarmCapacity} | Cold: {ColdUsage}/{ColdCapacity}",
                stats.HotLayer.Usage, stats.HotLayer.Capacity,
                stats.WarmLayer.Usage, stats.WarmLayer.Capacity,
                stats.ColdLayer.Usage, stats.ColdLayer.Capacity);
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}