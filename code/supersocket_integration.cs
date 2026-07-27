#:sdk Microsoft.NET.Sdk
#:package SuperSocket@2.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

public class SuperSocketOptions
{
    public int Port { get; set; } = 4040;
    public int MaxPackageLength { get; set; } = 1024 * 1024;
    public int ReceiveBufferSize { get; set; } = 8192;
    public int SendBufferSize { get; set; } = 8192;
    public bool UseMemoryPool { get; set; } = true;
    public int MemoryPoolSize { get; set; } = 1024 * 1024 * 100; // 100MB
    public int MemoryChunkSize { get; set; } = 4096; // 细粒度分块大小
    public bool UseTokenRingBuffer { get; set; } = true; // TokenRing缓冲区
    public bool EnableTailLatencyOptimizer { get; set; } = true; // 尾延迟优化
    public TieredMemoryConfig TieredMemory { get; set; } = new(); // 分层内存配置
}

public class TieredMemoryConfig
{
    public int HotMemorySize { get; set; } = 1024 * 1024 * 10; // 10MB热内存
    public int WarmMemorySize { get; set; } = 1024 * 1024 * 30; // 30MB温内存
    public int ColdMemorySize { get; set; } = 1024 * 1024 * 60; // 60MB冷内存
}

public interface ISuperSocketService
{
    Task StartAsync();
    Task StopAsync();
    ValueTask SendAsync(string sessionId, string message);
}

public class SuperSocketService : ISuperSocketService
{
    private readonly SuperSocketOptions _options;
    private readonly ILogger _logger;
    private IServer _server;
    private readonly ObjectPool<byte[]> _memoryPool;
    private readonly Stopwatch _performanceMonitor = new();
    private readonly ITokenRingBuffer _tokenRingBuffer;
    private readonly ITailLatencyOptimizer _tailLatencyOptimizer;
    private readonly ITieredMemoryService _tieredMemoryService;

    public SuperSocketService(IOptions<SuperSocketOptions> options, 
    ILogger<SuperSocketService> logger,
    ITokenRingBuffer tokenRingBuffer,
    ITailLatencyOptimizer tailLatencyOptimizer,
    ITieredMemoryService tieredMemoryService)
{
    _options = options.Value;
    _logger = logger;
    _tokenRingBuffer = tokenRingBuffer;
    _tailLatencyOptimizer = tailLatencyOptimizer;
    _tieredMemoryService = tieredMemoryService;
    
    // 细粒度内存池初始化
    var policy = new TieredMemoryPooledObjectPolicy(
        _options.MemoryChunkSize,
        _options.TieredMemory);
        
    _memoryPool = new DefaultObjectPool<byte[]>(policy, Environment.ProcessorCount * 2);
}

    public async Task StartAsync()
    {
        _performanceMonitor.Start();
        
        var builder = SuperSocketHostBuilder.Create<StringPackageInfo, LinePipelineFilter>()
            .UsePackageHandler(async (s, p) =>
            {
                _logger.LogInformation($"Received: {p.Body}");
                await s.SendAsync(Utf8StringPackageInfo.Create(p.Body));
            })
            .ConfigureServerOptions((ctx, config) =>
            {
                config.Listeners = new[] {
                    new ListenOptions
                    {
                        Ip = "Any",
                        Port = _options.Port,
                        BackLog = 100
                    }
                };

                if (_options.UseMemoryPool)
                {
                    config.ReceiveBufferSize = _options.ReceiveBufferSize;
                    config.SendBufferSize = _options.SendBufferSize;
                }
            });

        if (_options.UseMemoryPool)
        {
            builder.UseSessionFactory<MemoryPoolSessionFactory>();
        }

        _server = builder.BuildAsServer();
        await _server.StartAsync();
    }

    public async Task StopAsync()
    {
        _performanceMonitor.Stop();
        _logger.LogInformation($"Server ran for {_performanceMonitor.Elapsed.TotalSeconds} seconds");
        
        await _server.StopAsync();
    }

    public async ValueTask SendAsync(string sessionId, string message)
    {
        var session = _server.GetSessionByID(sessionId);
        if (session != null)
        {
            await session.SendAsync(Utf8StringPackageInfo.Create(message));
        }
    }
}

public static class SuperSocketExtensions
{
    public static IServiceCollection AddSuperSocket(this IServiceCollection services, Action<SuperSocketOptions> configure)
    {
        services.Configure(configure);
        
        // 注册高级内存管理组件
        services.AddSingleton<ITokenRingBuffer>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<SuperSocketOptions>>();
            return new TokenRingBuffer(Environment.ProcessorCount * 2, options.Value.MemoryChunkSize);
        });
            
        services.AddSingleton<ITailLatencyOptimizer, TailLatencyOptimizer>();
        
        services.AddSingleton<ITieredMemoryService>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<SuperSocketOptions>>();
            return new TieredMemoryService(options.Value.TieredMemory);
        });
        
        services.AddSingleton<ISuperSocketService, SuperSocketService>();
        services.AddHostedService(sp => sp.GetRequiredService<ISuperSocketService>());
        return services;
    }
}

public class TailLatencyOptimizer : ITailLatencyOptimizer
{
    private readonly ConcurrentDictionary<Socket, DateTime> _lastOperationTimes = new();
    
    public void Optimize(SocketAsyncEventArgs e)
    {
        if (_lastOperationTimes.TryGetValue(e.AcceptSocket, out var lastTime))
        {
            var latency = DateTime.UtcNow - lastTime;
            if (latency > TimeSpan.FromMilliseconds(100))
            {
                // 调整Socket缓冲区大小以优化尾延迟
                e.AcceptSocket.ReceiveBufferSize = Math.Min(
                    e.AcceptSocket.ReceiveBufferSize * 2, 
                    1024 * 1024); // 最大1MB
            }
        }
        _lastOperationTimes[e.AcceptSocket] = DateTime.UtcNow;
    }
}

public class TieredMemoryService : ITieredMemoryService, IDisposable
{
    private readonly MemoryPool<byte> _hotMemoryPool;
    private readonly MemoryPool<byte> _warmMemoryPool;
    private readonly MemoryPool<byte> _coldMemoryPool;
    
    public TieredMemoryService(TieredMemoryConfig config)
    {
        _hotMemoryPool = MemoryPool<byte>.Shared;
        _warmMemoryPool = new SizedMemoryPool(config.WarmMemorySize);
        _coldMemoryPool = new SizedMemoryPool(config.ColdMemorySize);
    }
    
    public IMemoryOwner<byte> Rent(int size)
    {
        if (size <= 4096) return _hotMemoryPool.Rent(size);
        if (size <= 32768) return _warmMemoryPool.Rent(size);
        return _coldMemoryPool.Rent(size);
    }
    
    public void Dispose()
    {
        _warmMemoryPool.Dispose();
        _coldMemoryPool.Dispose();
    }
}

internal class SizedMemoryPool : MemoryPool<byte>
{
    private readonly int _maxBufferSize;
    private readonly ArrayPool<byte> _arrayPool;
    
    public SizedMemoryPool(int maxBufferSize)
    {
        _maxBufferSize = maxBufferSize;
        _arrayPool = ArrayPool<byte>.Create(maxBufferSize, 50);
    }
    
    public override IMemoryOwner<byte> Rent(int size)
    {
        if (size > _maxBufferSize)
            throw new ArgumentOutOfRangeException(nameof(size));
            
        return new ArrayMemoryOwner(_arrayPool, size);
    }
    
    protected override void Dispose(bool disposing)
    {
        // 清理资源
    }
    
    public override int MaxBufferSize => _maxBufferSize;
}

internal class ArrayMemoryOwner : IMemoryOwner<byte>
{
    private readonly ArrayPool<byte> _pool;
    private byte[] _array;
    
    public ArrayMemoryOwner(ArrayPool<byte> pool, int size)
    {
        _pool = pool;
        _array = _pool.Rent(size);
    }
    
    public Memory<byte> Memory => _array;
    
    public void Dispose()
    {
        if (_array != null)
        {
            _pool.Return(_array);
            _array = null;
        }
    }
}

// Memory pool implementation
public class TieredMemoryPooledObjectPolicy : IPooledObjectPolicy<byte[]>
{
    private readonly int _chunkSize;
    private readonly TieredMemoryConfig _config;
    
    public TieredMemoryPooledObjectPolicy(int chunkSize, TieredMemoryConfig config)
    {
        _chunkSize = chunkSize;
        _config = config;
    }

    public byte[] Create() => new byte[_chunkSize];

    public bool Return(byte[] obj)
    {
        Array.Clear(obj, 0, obj.Length);
        return true;
    }
}

public class TokenRingBuffer : ITokenRingBuffer
{
    private readonly byte[][] _buffers;
    private readonly int[] _writePositions;
    private readonly int[] _readPositions;
    private int _currentIndex;
    private readonly object _lock = new();

    public TokenRingBuffer(int bufferCount, int bufferSize)
    {
        _buffers = new byte[bufferCount][];
        _writePositions = new int[bufferCount];
        _readPositions = new int[bufferCount];
        
        for (int i = 0; i < bufferCount; i++)
        {
            _buffers[i] = new byte[bufferSize];
            _writePositions[i] = 0;
            _readPositions[i] = 0;
        }
    }

    public void Write(ReadOnlySpan<byte> data)
    {
        lock (_lock)
        {
            var buffer = _buffers[_currentIndex];
            var remaining = buffer.Length - _writePositions[_currentIndex];
            
            if (data.Length <= remaining)
            {
                data.CopyTo(buffer.AsSpan(_writePositions[_currentIndex]));
                _writePositions[_currentIndex] += data.Length;
            }
            else
            {
                // 处理环形写入
                var firstPart = data.Slice(0, remaining);
                var secondPart = data.Slice(remaining);
                
                firstPart.CopyTo(buffer.AsSpan(_writePositions[_currentIndex]));
                secondPart.CopyTo(buffer.AsSpan(0));
                _writePositions[_currentIndex] = secondPart.Length;
            }
            
            _currentIndex = (_currentIndex + 1) % _buffers.Length;
        }
    }

    public ReadOnlySpan<byte> Read(int size)
    {
        lock (_lock)
        {
            var buffer = _buffers[_currentIndex];
            var readPos = _readPositions[_currentIndex];
            
            if (readPos + size <= buffer.Length)
            {
                _readPositions[_currentIndex] += size;
                return buffer.AsSpan(readPos, size);
            }
            else
            {
                // 处理环形读取
                var firstPartSize = buffer.Length - readPos;
                var secondPartSize = size - firstPartSize;
                
                var firstPart = buffer.AsSpan(readPos, firstPartSize);
                var secondPart = buffer.AsSpan(0, secondPartSize);
                
                _readPositions[_currentIndex] = secondPartSize;
                return firstPart.ToArray().Concat(secondPart.ToArray()).ToArray();
            }
        }
    }
}

public class TailLatencyOptimizer : ITailLatencyOptimizer
{
    private readonly Stopwatch _sw = new();
    private long _totalRequests;
    private long _tailLatencyThreshold;

    public TailLatencyOptimizer(long thresholdMs = 100)
    {
        _tailLatencyThreshold = thresholdMs;
    }

    public void BeginRequest() => _sw.Restart();

    public void EndRequest()
    {
        _totalRequests++;
        var elapsed = _sw.ElapsedMilliseconds;
        if (elapsed > _tailLatencyThreshold)
        {
            // 触发尾延迟优化逻辑
            OptimizeTailLatency();
        }
    }

    private void OptimizeTailLatency() { /* 优化实现 */ }
}

public class TieredMemoryService : ITieredMemoryService
{
    private readonly byte[] _hotMemory;
    private readonly byte[] _warmMemory;
    private readonly byte[] _coldMemory;

    public TieredMemoryService(TieredMemoryConfig config)
    {
        _hotMemory = new byte[config.HotMemorySize];
        _warmMemory = new byte[config.WarmMemorySize];
        _coldMemory = new byte[config.ColdMemorySize];
    }

    public byte[] GetHotMemory() => _hotMemory;
    public byte[] GetWarmMemory() => _warmMemory;
    public byte[] GetColdMemory() => _coldMemory;
}

// Span-based optimized package info
public sealed class Utf8StringPackageInfo : StringPackageInfo
{
    private Utf8StringPackageInfo(string body) : base(body) {}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringPackageInfo Create(string body)
    {
        return new Utf8StringPackageInfo(body);
    }

    public override int EncodeBody(IBufferWriter<byte> writer)
    {
        var span = writer.GetSpan(Encoding.UTF8.GetMaxByteCount(Body.Length));
        var bytesWritten = Encoding.UTF8.GetBytes(Body, span);
        writer.Advance(bytesWritten);
        return bytesWritten;
    }
}

public class SuperSocketExample
{
    public static async Task Demo()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSuperSocket(options =>
        {
            options.Port = 4040;
            options.MaxPackageLength = 2 * 1024 * 1024;
            options.ReceiveBufferSize = 16384;
            options.SendBufferSize = 16384;
            options.UseMemoryPool = true;
        });

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<ISuperSocketService>();

        try
        {
            await service.StartAsync();
            Console.WriteLine("SuperSocket server started");
            Console.ReadLine();
        }
        finally
        {
            await service.StopAsync();
        }
    }
}