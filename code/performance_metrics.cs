#:sdk Microsoft.NET.Sdk.Web
#:package Prometheus.Client@4.3.0
#:package System.Diagnostics.Metrics@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Prometheus.Client;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 性能指标通道
var metricsChannel = Channel.CreateBounded<PerformanceMetric>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝指标处理器
builder.Services.AddSingleton<IPerformanceMonitor>(sp => 
    new ChannelPerformanceMonitor(
        metricsChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

var app = builder.Build();
app.MapGet("/metrics", () => "Metrics Endpoint");
app.MapGet("/", () => "Performance Metrics Ready");
app.Run();

[SkipLocalsInit]
public class ChannelPerformanceMonitor : IPerformanceMonitor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Record(in PerformanceMetric metric)
    {
        // 零拷贝处理性能指标
    }
}