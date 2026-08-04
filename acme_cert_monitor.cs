#:sdk Microsoft.NET.Sdk.Web
#:package Prometheus.Client@6.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0

using Prometheus.Client;

[SkipLocalsInit]
public sealed class CertificateMonitor
{
    private readonly IMetricFamily<IGauge> _certExpiryGauge;
    private readonly Channel<X509Certificate2> _monitorChannel;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public CertificateMonitor()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _monitorChannel = Channel.CreateBounded<X509Certificate2>(1000);
        
        _certExpiryGauge = Metrics.DefaultFactory.CreateGauge(
            "certificate_expiry_days", 
            "Days until certificate expiry",
            "domain");
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task MonitorAsync(CancellationToken ct)
    {
        await foreach (var cert in _monitorChannel.Reader.ReadAllAsync(ct))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var expiryDays = (cert.NotAfter - DateTime.UtcNow).TotalDays;
            _certExpiryGauge.WithLabels(cert.GetNameInfo(X509NameType.DnsName, false))
                .Set(expiryDays);
            
            if (expiryDays < 7) TriggerAlert(cert);
        }
    }
}