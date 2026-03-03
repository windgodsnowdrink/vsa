#:sdk Microsoft.NET.Sdk.Web
#:package App.Metrics@4.3.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.6.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:package OpenTelemetry.Exporter.Zipkin@1.7.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using OpenTelemetry;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder();

// 1. 分布式追踪配置
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => {
        tracing.AddSource("ZeroFormatter.MessageBus")
              .AddZipkinExporter(o => 
                  o.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"))
              .AddConsoleExporter();
    });

builder.Services.AddMetrics(metrics =>
{
    metrics.Report.ToOpenTelemetry(options =>
    {
        options.Endpoint = "http://localhost:4317";
        options.Protocol = OpenTelemetryProtocol.Http;
    });
});

var app = builder.Build();
app.MapGet("/", () => "Distributed Tracing Ready");
app.Run();

// 2. 追踪增强型消息处理器
public sealed class TracedMessageHandler : IEventHandler<MessageEvent>
{
    private static readonly ActivitySource ActivitySource = new("ZeroFormatter.MessageBus");
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void OnEvent(MessageEvent data, long sequence, bool endOfBatch)
    {
        using var activity = ActivitySource.StartActivity("ProcessMessage");
        activity?.SetTag("message.type", data.MessageType);
        // ... 处理逻辑 ...
    }
}