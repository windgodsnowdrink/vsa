#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.6.0
#:package OpenTelemetry.Extensions.Hosting@1.6.0
#:package OpenTelemetry.Instrumentation.AspNetCore@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder();

// 配置OpenTelemetry资源
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService("SampleApp", 
        serviceVersion: "1.0.0",
        serviceInstanceId: Environment.MachineName)
    .AddTelemetrySdk()
    .AddEnvironmentVariableDetector();

// 配置Tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnableGrpcAspNetCoreSupport = true;
            })
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri("http://localhost:4317");
                opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
    })
    .WithMetrics(metricsProviderBuilder =>
    {
        metricsProviderBuilder
            .SetResourceBuilder(resourceBuilder)
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddOtlpExporter();
    });

// 高性能指标处理器
builder.Services.AddSingleton<IMetricsProcessor, OtelMetricsProcessor>();

var app = builder.Build();

// 自定义监控点示例
app.MapGet("/api/orders", ([FromServices] Meter meter) =>
{
    var counter = meter.CreateCounter<int>("orders.request_count");
    counter.Add(1);
    return Results.Ok();
});

app.MapGet("/", () => "OpenTelemetry APM Ready");
app.Run();

// 零拷贝指标处理器
public class OtelMetricsProcessor : IMetricsProcessor
{
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer = 
        new(() => stackalloc byte[256]);
    
    public void Process(MetricData data)
    {
        var buffer = _threadLocalBuffer.Value;
        // 使用SIMD指令处理指标数据
    }
}