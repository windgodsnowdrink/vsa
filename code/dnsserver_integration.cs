#:sdk Microsoft.NET.Sdk
#:package DnsServer@1.7.0
#:property TargetFramework net10.0
#:property Nullable enable

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DnsServer;
using DnsServer.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class DnsServerOptions
{
    public string Domain { get; set; } = "local.";
    public TimeSpan ScanInterval { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
    public int RetryCount { get; set; } = 3;
    public string ServiceType { get; set; } = "_device._tcp";
    public int Port { get; set; } = 5353;
}

public interface IDnsServerService : IDisposable
{
    Task RegisterServiceAsync(string serviceName, int port, IDictionary<string, string> properties);
    Task UnregisterServiceAsync();
    IObservable<DnsServiceEntry> DiscoveredDevices { get; }
    Task StartDiscoveryAsync();
    Task StopDiscoveryAsync();
}

public class DnsServiceEntry
{
    public string Name { get; set; }
    public int Port { get; set; }
    public IPAddress[] Addresses { get; set; }
    public IDictionary<string, string> Properties { get; set; }
}

public class DnsServerService : IDnsServerService
{
    private readonly DnsServerOptions _options;
    private readonly IDnsServer _dnsServer;
    private readonly ConcurrentDictionary<string, DnsServiceEntry> _discoveredDevices = new();
    private readonly Subject<DnsServiceEntry> _discoveredSubject = new();
    private IDisposable _scanSubscription;
    private string _registeredServiceName;
    private int _registeredPort;
    private IDictionary<string, string> _registeredProperties;
    private bool _disposed;

    public DnsServerService(IOptions<DnsServerOptions> options, IDnsServer dnsServer)
    {
        _options = options.Value;
        _dnsServer = dnsServer;
    }

    public IObservable<DnsServiceEntry> DiscoveredDevices => _discoveredSubject;

    public async Task RegisterServiceAsync(string serviceName, int port, IDictionary<string, string> properties)
    {
        _registeredServiceName = serviceName;
        _registeredPort = port;
        _registeredProperties = properties;

        // Register PTR record
        var ptrRecord = new PtrRecord
        {
            DomainName = $"{_options.ServiceType}.{_options.Domain}",
            PtrDomainName = $"{serviceName}.{_options.ServiceType}.{_options.Domain}"
        };
        await _dnsServer.AddRecordAsync(ptrRecord);

        // Register SRV record
        var srvRecord = new SrvRecord
        {
            DomainName = $"{serviceName}.{_options.ServiceType}.{_options.Domain}",
            Port = port,
            Target = Dns.GetHostName(),
            Priority = 10,
            Weight = 1
        };
        await _dnsServer.AddRecordAsync(srvRecord);

        // Register TXT record
        var txtProperties = string.Join(";", properties.Select(p => $"{p.Key}={p.Value}"));
        var txtRecord = new TxtRecord
        {
            DomainName = $"{serviceName}.{_options.ServiceType}.{_options.Domain}",
            Text = new[] { txtProperties }
        };
        await _dnsServer.AddRecordAsync(txtRecord);
    }

    public async Task UnregisterServiceAsync()
    {
        if (_registeredServiceName != null)
        {
            await _dnsServer.RemoveRecordAsync($"{_registeredServiceName}.{_options.ServiceType}.{_options.Domain}", ResourceRecordType.SRV);
            await _dnsServer.RemoveRecordAsync($"{_registeredServiceName}.{_options.ServiceType}.{_options.Domain}", ResourceRecordType.TXT);
            await _dnsServer.RemoveRecordAsync($"{_options.ServiceType}.{_options.Domain}", ResourceRecordType.PTR);
            _registeredServiceName = null;
        }
    }

    public async Task StartDiscoveryAsync()
    {
        _scanSubscription = Observable.Interval(_options.ScanInterval)
            .SelectMany(_ => DiscoverServicesAsync())
            .Subscribe(entry => 
            {
                if (_discoveredDevices.TryAdd(entry.Name, entry))
                {
                    _discoveredSubject.OnNext(entry);
                }
            });

        // Initial scan
        var entries = await DiscoverServicesAsync();
        foreach (var entry in entries)
        {
            if (_discoveredDevices.TryAdd(entry.Name, entry))
            {
                _discoveredSubject.OnNext(entry);
            }
        }
    }

    private async Task<IEnumerable<DnsServiceEntry>> DiscoverServicesAsync()
    {
        var query = $"{_options.ServiceType}.{_options.Domain}";
        var result = await _dnsServer.ResolveAsync(query, ResourceRecordType.PTR);
        
        var entries = new List<DnsServiceEntry>();
        foreach (var ptrRecord in result.Answers.OfType<PtrRecord>())
        {
            var srvResult = await _dnsServer.ResolveAsync(ptrRecord.PtrDomainName, ResourceRecordType.SRV);
            var srvRecord = srvResult.Answers.OfType<SrvRecord>().FirstOrDefault();
            
            if (srvRecord != null)
            {
                var txtResult = await _dnsServer.ResolveAsync(ptrRecord.PtrDomainName, ResourceRecordType.TXT);
                var txtRecord = txtResult.Answers.OfType<TxtRecord>().FirstOrDefault();
                
                var aResult = await _dnsServer.ResolveAsync(srvRecord.Target, ResourceRecordType.A);
                var aRecords = aResult.Answers.OfType<ARecord>().Select(a => a.Address).ToArray();
                
                var properties = txtRecord?.Text.FirstOrDefault()?
                    .Split(';')
                    .Select(p => p.Split('='))
                    .Where(p => p.Length == 2)
                    .ToDictionary(p => p[0], p => p[1]) ?? new Dictionary<string, string>();
                
                entries.Add(new DnsServiceEntry
                {
                    Name = ptrRecord.PtrDomainName,
                    Port = srvRecord.Port,
                    Addresses = aRecords,
                    Properties = properties
                });
            }
        }
        
        return entries;
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

public static class DnsServerExtensions
{
    public static IServiceCollection AddDnsServerService(this IServiceCollection services, Action<DnsServerOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IDnsServer>(provider => 
        {
            var options = provider.GetRequiredService<IOptions<DnsServerOptions>>().Value;
            return new DnsServer(options.Port);
        });
        services.AddSingleton<IDnsServerService, DnsServerService>();
        return services;
    }
}

// Usage example:
// var services = new ServiceCollection();
// services.AddDnsServerService(options => 
// {
//     options.ServiceType = "_myapp._tcp";
//     options.ScanInterval = TimeSpan.FromMinutes(1);
// });
// var provider = services.BuildServiceProvider();
// var dnsServer = provider.GetRequiredService<IDnsServerService>();
// await dnsServer.RegisterServiceAsync("MyDevice", 8080, new Dictionary<string, string> { ["key"] = "value" });
// await dnsServer.StartDiscoveryAsync();