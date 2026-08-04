#:sdk Microsoft.NET.Sdk.Web
#:package Prometheus.Client@4.3.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Prometheus.Client;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置指标收集
builder.Services.AddSingleton<IMetricCollector>(sp => 
    new PrometheusMetricCollector(
        Channel.CreateBounded<MetricEvent>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

var app = builder.Build();
app.MapGet("/metrics", () => "Metrics Endpoint");
app.MapGet("/", () => "Metrics Ready");
app.Run();

public class PrometheusMetricCollector : IMetricCollector
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Record(MetricEvent @event)
    {
        // 实现细节...
    }
}