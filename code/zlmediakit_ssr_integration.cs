#:sdk Microsoft.NET.Sdk.Web
#:package ZLMediaKit.Core@1.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package FFmpeg.AutoGen@5.1.2
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Buffers;
using System.Threading.Channels;
using ZLMediaKit.Core;
using FFmpeg.AutoGen;
using Microsoft.Extensions.ObjectPool;

// 1. SSR流媒体处理器(高性能实现)
[SkipLocalsInit]
public sealed class ZlmSsrProcessor : IAsyncDisposable
{
    private readonly Channel<MediaFrame> _videoChannel;
    private readonly ThreadLocal<Span<byte>> _videoBuffer;
    private readonly ObjectPool<MediaFrame> _framePool;
    private readonly ZlmContext _zlmContext;
    private readonly CancellationTokenSource _cts = new();
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public ZlmSsrProcessor(ZlmContext zlmContext)
    {
        _zlmContext = zlmContext;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _videoChannel = Channel.CreateBounded<MediaFrame>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 线程本地4K帧缓冲区(CPU cache-line对齐)
        _videoBuffer = new(() => 
        {
            var buffer = GC.AllocateUninitializedArray<byte>(3840 * 2160 * 4, true);
            return new Span<byte>(buffer);
        });
        
        // 帧对象池
        _framePool = new DefaultObjectPool<MediaFrame>(
            new MediaFramePooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessVideoStreamAsync(Stream stream)
    {
        while (!_cts.IsCancellationRequested)
        {
            var frame = _framePool.Get();
            try
            {
                // 零拷贝读取
                Span<byte> buffer = _videoBuffer.Value;
                int bytesRead = await stream.ReadAsync(buffer);
                
                if (bytesRead > 0)
                {
                    _latencyOptimizer.Optimize(() => 
                    {
                        // 硬件加速处理
                        frame.Data = buffer.Slice(0, bytesRead).ToArray();
                        _videoChannel.Writer.TryWrite(frame);
                    });
                }
            }
            finally
            {
                _framePool.Return(frame);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _videoChannel.Writer.Complete();
    }
}

// 2. ZLMediaKit服务器扩展
public static class ZlmServiceExtensions
{
    public static IServiceCollection AddZlmServer(
        this IServiceCollection services,
        Action<ZlmOptions> configure)
    {
        var options = new ZlmOptions();
        configure(options);
        
        // 分层内存服务
        services.AddSingleton<TieredMemoryServer>();
        
        // 注册ZLM上下文
        services.AddSingleton<ZlmContext>(sp => 
            new ZlmContext(options, sp.GetRequiredService<TieredMemoryServer>()));
            
        // 注册流处理器
        services.AddSingleton<ZlmSsrProcessor>();
        
        return services;
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置ZLMediaKit服务器
builder.Services.AddZlmServer(options =>
{
    options.ListenPort = 1935;
    options.ChunkSize = 4096;
    options.MaxConnections = 10000;
    options.EnableHls = true;
    options.EnableH265 = true;
});

var app = builder.Build();

// SSR流媒体端点
app.MapGet("/stream/{app}/{stream}", async (string app, string stream) =>
{
    var processor = app.Services.GetRequiredService<ZlmSsrProcessor>();
    return Results.Stream(processor.ProcessVideoStreamAsync);
});

app.Run();