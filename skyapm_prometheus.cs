#:sdk Microsoft.NET.Sdk.Web
#:package SkyAPM.Agent.AspNetCore@1.6.0
#:package Prometheus.Client@4.3.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Prometheus.Client;
using System.Threading.Channels;

public class PrometheusExporter : BackgroundService
{
    private readonly ChannelReader<MetricSample> _reader;
    private readonly IMetricFactory _factory;
    private readonly ThreadLocal<Span<byte>> _buffer;

    public PrometheusExporter(ChannelReader<MetricSample> reader, IMetricFactory factory)
    {
        _reader = reader;
        _factory = factory;
        _buffer = new ThreadLocal<Span<byte>>(() => stackalloc byte[256]);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var counter = _factory.CreateCounter("skyapm_metrics_total", "Total exported metrics");
        
        while (await _reader.WaitToReadAsync(stoppingToken))
        {
            var span = _buffer.Value;
            while (_reader.TryRead(out var sample))
            {
                // 零拷贝处理指标数据
                counter.Inc();
            }
        }
    }
}