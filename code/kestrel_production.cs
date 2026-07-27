#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Server.Kestrel@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Net;
using System.Threading.Channels;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class KestrelProductionServer : IAsyncDisposable
{
    private readonly IConnectionListener _listener;
    private readonly Channel<ConnectionContext> _connectionChannel;
    private readonly ObjectPool<Memory<byte>> _bufferPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public KestrelProductionServer(IPEndPoint endpoint)
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        
        _bufferPool = new DefaultObjectPool<Memory<byte>>(
            new MemoryPooledPolicy(), 
            Environment.ProcessorCount * 2);
        
        _connectionChannel = Channel.CreateBounded<ConnectionContext>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _listener = new SocketConnectionListener(endpoint, new ConnectionListenerOptions
        {
            MemoryPool = new MemoryPool(),
            Backlog = 1024
        });
        
        _ = Task.Run(AcceptConnectionsAsync);
        _ = Task.Run(ProcessConnectionsAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task AcceptConnectionsAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                var connection = await _listener.AcceptAsync(_cts.Token);
                await _connectionChannel.Writer.WriteAsync(connection, _cts.Token);
            }
            catch (OperationCanceledException) when (_cts.IsCancellationRequested)
            {
                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessConnectionsAsync()
    {
        await foreach (var connection in _connectionChannel.Reader.ReadAllAsync(_cts.Token))
        {
            _ = Task.Run(async () =>
            {
                using (connection)
                {
                    var buffer = _bufferPool.Get();
                    try
                    {
                        while (!connection.ConnectionClosed.IsCancellationRequested)
                        {
                            var result = await connection.Transport.Input.ReadAsync(_cts.Token);
                            if (result.IsCompleted) break;
                            
                            // 处理数据
                            await ProcessDataAsync(result.Buffer, connection);
                            
                            connection.Transport.Input.AdvanceTo(result.Buffer.End);
                        }
                    }
                    finally
                    {
                        _bufferPool.Return(buffer);
                    }
                }
            });
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _connectionChannel.Writer.Complete();
        await _connectionChannel.Reader.Completion;
        await _listener.DisposeAsync();
    }
}

[SkipLocalsInit]
internal sealed class MemoryPooledPolicy : PooledObjectPolicy<Memory<byte>>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override Memory<byte> Create() => new byte[4096];

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(Memory<byte> obj)
    {
        obj.Span.Clear();
        return true;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5000, listenOptions =>
    {
        listenOptions.UseConnectionHandler<ProductionConnectionHandler>();
    });
});

var app = builder.Build();
app.MapGet("/", () => "Kestrel Production Server");
app.Run();