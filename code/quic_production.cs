#:sdk Microsoft.NET.Sdk.Web
#:package System.Net.Quic@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Net.Quic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class QuicServer : BackgroundService
{
    private readonly QuicListener _listener;
    private readonly Channel<QuicConnection> _connectionChannel;
    private readonly ObjectPool<QuicStream> _streamPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly X509Certificate2 _certificate;

    public QuicServer(X509Certificate2 certificate, IPEndPoint endpoint)
    {
        _certificate = certificate;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        var options = new QuicListenerOptions
        {
            ListenEndPoint = endpoint,
            ApplicationProtocols = new List<SslApplicationProtocol> 
            { 
                SslApplicationProtocol.Http3 
            },
            ConnectionOptionsCallback = (_, _, _) => 
                ValueTask.FromResult(new QuicServerConnectionOptions
                {
                    DefaultStreamErrorCode = 0x0c,
                    DefaultCloseErrorCode = 0x10,
                    ServerAuthenticationOptions = new SslServerAuthenticationOptions
                    {
                        ServerCertificate = _certificate,
                        ApplicationProtocols = new List<SslApplicationProtocol>
                        {
                            SslApplicationProtocol.Http3
                        }
                    }
                })
        };
        
        _listener = new QuicListener(options);
        _connectionChannel = Channel.CreateBounded<QuicConnection>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _streamPool = new DefaultObjectPool<QuicStream>(
            new QuicStreamPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var acceptTask = AcceptConnectionsAsync(stoppingToken);
        var processTask = ProcessConnectionsAsync(stoppingToken);
        
        await Task.WhenAll(acceptTask, processTask);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task AcceptConnectionsAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var connection = await _listener.AcceptConnectionAsync(ct);
                await _connectionChannel.Writer.WriteAsync(connection, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessConnectionsAsync(CancellationToken ct)
    {
        await foreach (var connection in _connectionChannel.Reader.ReadAllAsync(ct))
        {
            _ = Task.Run(async () =>
            {
                using (connection)
                {
                    while (!connection.IsClosed && !ct.IsCancellationRequested)
                    {
                        try
                        {
                            var stream = await connection.AcceptInboundStreamAsync(ct);
                            _ = ProcessStreamAsync(stream, ct);
                        }
                        catch (QuicException) { break; }
                    }
                }
            }, ct);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessStreamAsync(QuicStream stream, CancellationToken ct)
    {
        using var latencyToken = _latencyOptimizer.BeginOperation();
        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        try
        {
            var bytesRead = await stream.ReadAsync(buffer, ct);
            // 处理QUIC数据流
            await HandleQuicData(buffer.AsMemory(0, bytesRead), stream, ct);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _listener.DisposeAsync();
        _connectionChannel.Writer.Complete();
        await base.StopAsync(cancellationToken);
    }
}

[SkipLocalsInit]
internal sealed class QuicStreamPooledPolicy : PooledObjectPolicy<QuicStream>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override QuicStream Create() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(QuicStream obj)
    {
        if (!obj.CanWrite) return false;
        obj.Reset(QuicAbortDirection.Both, 0);
        return true;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<QuicServer>(provider => 
    new QuicServer(
        LoadCertificate(), 
        new IPEndPoint(IPAddress.Any, 443)));

var app = builder.Build();
app.MapGet("/", () => "QUIC Server Running");
app.Run();

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
static X509Certificate2 LoadCertificate()
{
    // 加载证书逻辑
    return new X509Certificate2("certificate.pfx", "password");
}