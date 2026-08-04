#:sdk Microsoft.NET.Sdk.Web
#:package Zeroconf@3.0.0
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Zeroconf;

public class MdnsOptions
{
    public string ServiceType { get; set; } = "_http._tcp.local.";
    public string Domain { get; set; } = "local.";
    public TimeSpan ScanInterval { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    public int RetryCount { get; set; } = 3;
}

public interface IMdnsService : IDisposable
{
    Task RegisterServiceAsync(string serviceName, int port, IDictionary<string, string> properties);
    Task UnregisterServiceAsync();
    IObservable<IZeroconfHost> DiscoveredDevices { get; }
    Task StartDiscoveryAsync();
    Task StopDiscoveryAsync();
}

public class MdnsService : IMdnsService
{
    private readonly MdnsOptions _options;
    private readonly ConcurrentDictionary<string, IZeroconfHost> _discoveredDevices = new();
    private readonly Subject<IZeroconfHost> _discoveredSubject = new();
    private IDisposable _scanSubscription;
    private string _registeredServiceName;
    private int _registeredPort;
    private IDictionary<string, string> _registeredProperties;
    private bool _disposed;

    public MdnsService(IOptions<MdnsOptions> options)
    {
        _options = options.Value;
    }

    public IObservable<IZeroconfHost> DiscoveredDevices => _discoveredSubject;

    public async Task RegisterServiceAsync(string serviceName, int port, IDictionary<string, string> properties)
    {
        _registeredServiceName = serviceName;
        _registeredPort = port;
        _registeredProperties = properties;

        for (int i = 0; i < _options.RetryCount; i++)
        {
            try
            {
                await ZeroconfResolver.RegisterService(serviceName, _options.ServiceType, port, 
                    _options.Domain, properties);
                return;
            }
            catch (Exception) when (i < _options.RetryCount - 1)
            {
                await Task.Delay(1000);
            }
        }
    }

    public async Task UnregisterServiceAsync()
    {
        if (_registeredServiceName != null)
        {
            await ZeroconfResolver.UnregisterService(_registeredServiceName, _options.ServiceType, 
                _registeredPort, _options.Domain);
            _registeredServiceName = null;
        }
    }

    public async Task StartDiscoveryAsync()
    {
        _scanSubscription = Observable.Interval(_options.ScanInterval)
            .SelectMany(_ => ZeroconfResolver.ResolveAsync(_options.ServiceType, _options.Timeout))
            .Subscribe(host => 
            {
                if (_discoveredDevices.TryAdd(host.Id, host))
                {
                    _discoveredSubject.OnNext(host);
                }
            });

        // Initial scan
        var hosts = await ZeroconfResolver.ResolveAsync(_options.ServiceType, _options.Timeout);
        foreach (var host in hosts)
        {
            if (_discoveredDevices.TryAdd(host.Id, host))
            {
                _discoveredSubject.OnNext(host);
            }
        }
    }

    public Task StopDiscoveryAsync()
    {
        _scanSubscription?.Dispose();
        _scanSubscription = null;
        _discoveredDevices.Clear();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed) return;

        _scanSubscription?.Dispose();
        _discoveredSubject.Dispose();
        _disposed = true;
    }
}

public static class MdnsExtensions
{
    public static IServiceCollection AddMdnsService(this IServiceCollection services, Action<MdnsOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IMdnsService, MdnsService>();
        return services;
    }
}

// Usage example:
// var services = new ServiceCollection();
// services.AddMdnsService(options => 
// {
//     options.ServiceType = "_myapp._tcp.local.";
//     options.ScanInterval = TimeSpan.FromMinutes(1);
// });
// var provider = services.BuildServiceProvider();
// var mdns = provider.GetRequiredService<IMdnsService>();
// await mdns.RegisterServiceAsync("MyDevice", 8080, new Dictionary<string, string> { ["key"] = "value" });
// await mdns.StartDiscoveryAsync();