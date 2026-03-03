#:sdk Microsoft.NET.Sdk.Web
#:package SIPSorcery.Net@6.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using SIPSorcery.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

public class NatTypeTesterOptions
{
    public int TestTimeoutMs { get; set; } = 5000;
    public string[] StunServers { get; set; } = new[] { "stun.l.google.com:19302", "stun1.l.google.com:19302" };
}

public interface INatTypeTester
{
    Task<NatTypes> DetectNatTypeAsync(CancellationToken cancellationToken = default);
    IAsyncEnumerable<NatTypes> ContinuousDetectionAsync(CancellationToken cancellationToken = default);
}

public class NatTypeTester : INatTypeTester, IDisposable
{
    private readonly NatTypeTesterOptions _options;
    private readonly Channel<NatTypes> _resultsChannel = Channel.CreateUnbounded<NatTypes>();
    private readonly ObjectPool<UdpClient> _udpClientPool;
    private bool _disposed;

    public NatTypeTester(IOptions<NatTypeTesterOptions> options)
    {
        _options = options.Value;
        _udpClientPool = new DefaultObjectPool<UdpClient>(new UdpClientPooledObjectPolicy());
    }

    public async Task<NatTypes> DetectNatTypeAsync(CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(_options.TestTimeoutMs);

        var udpClient = _udpClientPool.Get();
        try
        {
            var stunClient = new STUNClient(udpClient);
            var result = await stunClient.DiscoverNatTypeAsync(
                _options.StunServers.First(),
                cts.Token);
            return result.NatType;
        }
        finally
        {
            _udpClientPool.Return(udpClient);
        }
    }

    public async IAsyncEnumerable<NatTypes> ContinuousDetectionAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var natType = await DetectNatTypeAsync(cancellationToken);
                await _resultsChannel.Writer.WriteAsync(natType, cancellationToken);
                yield return natType;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _resultsChannel.Writer.Complete();
        _disposed = true;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNatTypeTester(this IServiceCollection services, Action<NatTypeTesterOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<INatTypeTester, NatTypeTester>();
        services.AddSingleton<ObjectPool<UdpClient>>(sp =>
            new DefaultObjectPool<UdpClient>(new UdpClientPooledObjectPolicy()));
        return services;
    }
}

internal class UdpClientPooledObjectPolicy : PooledObjectPolicy<UdpClient>
{
    public override UdpClient Create() => new UdpClient(0);

    public override bool Return(UdpClient obj)
    {
        if (obj.Client.Connected)
        {
            obj.Client.Disconnect(false);
        }
        return true;
    }
}

// 示例用法
public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddNatTypeTester(options =>
                {
                    options.TestTimeoutMs = 3000;
                    options.StunServers = new[] { "stun.voipbuster.com:3478" };
                });
            })
            .Build();

        var tester = host.Services.GetRequiredService<INatTypeTester>();
        var natType = await tester.DetectNatTypeAsync();
        Console.WriteLine($"Detected NAT type: {natType}");

        await host.RunAsync();
    }
}