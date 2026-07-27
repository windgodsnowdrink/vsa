#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.ApplicationInsights.AspNetCore@2.21.0
#:package Microsoft.ApplicationInsights.SnapshotCollector@1.4.7
#:package Microsoft.ApplicationInsights.DependencyCollector@2.21.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置Application Insights
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = "InstrumentationKey=your-instrumentation-key";
    options.EnableAdaptiveSampling = false; // 关闭采样保证数据完整
    options.EnableDependencyTrackingTelemetryModule = true;
    options.EnablePerformanceCounterCollectionModule = true;
    options.EnableQuickPulseMetricStream = true;
});

// 高性能通道处理器
builder.Services.AddSingleton<Channel<TelemetryItem>>(Channel.CreateUnbounded<TelemetryItem>(
    new UnboundedChannelOptions { SingleReader = true }));
builder.Services.AddSingleton<ITelemetryProcessor, ChannelTelemetryProcessor>();

var app = builder.Build();

// 自定义监控点示例
app.MapGet("/api/orders", ([FromServices] TelemetryClient telemetry) =>
{
    using (var operation = telemetry.StartOperation<RequestTelemetry>("OrderProcessing"))
    {
        // 记录自定义指标
        telemetry.TrackMetric("Orders/Processed", 1);
        return Results.Ok();
    }
});

app.MapGet("/", () => "Application Insights Ready");
app.Run();

// 零拷贝通道处理器
public class ChannelTelemetryProcessor : ITelemetryProcessor
{
    private readonly ChannelWriter<TelemetryItem> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;

    public ChannelTelemetryProcessor(Channel<TelemetryItem> channel)
    {
        _writer = channel.Writer;
        _buffer = new ThreadLocal<Span<byte>>(() => stackalloc byte[256]);
    }

    public void Process(TelemetryItem item)
    {
        var span = _buffer.Value;
        // 零拷贝处理遥测数据
        _writer.TryWrite(item);
    }
}