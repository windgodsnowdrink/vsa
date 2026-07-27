#:sdk Microsoft.NET.Sdk.Web
#:package Prometheus.Client@4.3.0
#:package System.Diagnostics.Metrics@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Prometheus.Client;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 指标收集通道
var metricsChannel = Channel.CreateBounded<CodeGenMetric>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝指标处理器
builder.Services.AddSingleton<ICodeGenMonitor>(sp => 
    new ChannelCodeGenMonitor(
        metricsChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[128])));

var app = builder.Build();
app.MapGet("/metrics", () => "Metrics Endpoint");
app.MapGet("/", () => "CodeGen Metrics Ready");
app.Run();

[SkipLocalsInit]
public class ChannelCodeGenMonitor : ICodeGenMonitor
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Record(in CodeGenMetric metric)
    {
        // 零拷贝处理指标数据
    }
}