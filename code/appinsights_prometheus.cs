#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.ApplicationInsights.AspNetCore@2.21.0
#:package Prometheus.Client@4.3.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.ApplicationInsights;
using Prometheus.Client;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置Prometheus导出
builder.Services.AddSingleton<IMetricExporter>(sp => 
    new PrometheusExporter(
        Channel.CreateUnbounded<MetricSample>(),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();
app.MapGet("/metrics", () => PrometheusMiddleware.GetMetrics());
app.MapGet("/", () => "Prometheus Export Ready");
app.Run();

public class PrometheusExporter : IMetricExporter
{
    private readonly ChannelReader<MetricSample> _reader;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public PrometheusExporter(Channel<MetricSample> channel, ThreadLocal<Span<byte>> buffer)
    {
        _reader = channel.Reader;
        _buffer = buffer;
    }
    
    public ExportResult Export(in Batch<Metric> batch)
    {
        var span = _buffer.Value;
        // 零拷贝处理指标数据
        return ExportResult.Success;
    }
}