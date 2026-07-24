#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Exporter.Prometheus.AspNetCore@1.6.0
#:package Prometheus.Client@4.3.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using OpenTelemetry.Metrics;
using Prometheus.Client;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 高性能指标通道 (Disruptor模式)
var metricsChannel = Channel.CreateBounded<MetricSample>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝指标处理器
builder.Services.AddSingleton<IMetricProcessor>(sp => 
    new ChannelMetricProcessor(
        metricsChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

// 配置OpenTelemetry指标
builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder
            .AddMeter("SampleApp.Metrics")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("SampleApp"))
            .AddPrometheusExporter(options =>
            {
                options.ScrapeEndpointPath = "/metrics";
                options.ScrapeResponseCacheDurationMilliseconds = 0;
            });
    });

// Prometheus客户端配置
builder.Services.AddSingleton<IMetricExporter>(sp => 
    new PrometheusExporter(
        metricsChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapGet("/", () => "Enhanced Metrics Ready");
app.Run();

[SkipLocalsInit]
public class ChannelMetricProcessor : IMetricProcessor
{
    private readonly ChannelWriter<MetricSample> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelMetricProcessor(Channel<MetricSample> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Process(in MetricSample sample)
    {
        var span = _buffer.Value;
        // 零拷贝处理指标数据
        _writer.TryWrite(sample);
    }
}