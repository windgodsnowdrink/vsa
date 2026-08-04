#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry@1.7.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:package OpenTelemetry.Exporter.Jaeger@1.7.0
#:package OpenTelemetry.Exporter.Prometheus.AspNetCore@1.7.0
#:package OpenTelemetry.Instrumentation.AspNetCore@1.7.0
#:package OpenTelemetry.Instrumentation.Http@1.7.0
#:package OpenTelemetry.Instrumentation.SqlClient@1.7.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:package OpenTelemetry.Instrumentation.AspNetCore@1.7.0
#:package OpenTelemetry.Instrumentation.Http@1.7.0
#:package OpenTelemetry.Instrumentation.SqlClient@1.7.0
#:property LangVersion=preview
#:property TargetFramework net8.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property TargetFramework=net11.0

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置资源
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder =>
    {
        resourceBuilder
            .AddService(serviceName: "MyService",
                       serviceVersion: "1.0.0")
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = builder.Environment.EnvironmentName,
                ["host.name"] = Environment.MachineName
            });
    })
    // 2. 配置追踪
    .WithTracing(tracing =>
    {
        tracing
            // 采样策略
            .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.5)))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("http.request_content_length", request.ContentLength);
                };
            })
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()
            // Jaeger导出
            .AddJaegerExporter(jaegerOptions =>
            {
                jaegerOptions.AgentHost = "localhost";
                jaegerOptions.AgentPort = 6831;
                jaegerOptions.ExportProcessorType = ExportProcessorType.Batch;
            });
    })
    // 3. 配置指标
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            // 自定义指标
            .AddMeter("MyApp.Metrics")
            // Prometheus导出
            .AddPrometheusExporter();
    });

// 4. 配置日志
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.ParseStateValues = true;
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService("MyService"));
});

// 5. 自定义指标
var meter = new Meter("MyApp.Metrics");
var requestCounter = meter.CreateCounter<long>("app.request.count");
var responseTimeHistogram = meter.CreateHistogram<double>("app.response.time", "ms");

// 6. 告警规则配置
builder.Services.AddHostedService<AlertingService>();

var app = builder.Build();

// 7. 性能优化配置
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.Use(async (context, next) =>
{
    // 分布式追踪上下文传播
    var activity = Activity.Current;
    if (activity != null)
    {
        context.Response.Headers["traceparent"] = activity.Id;
        context.Response.Headers["tracestate"] = activity.TraceStateString;
    }

    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    // 记录指标
    requestCounter.Add(1);
    responseTimeHistogram.Record(stopwatch.ElapsedMilliseconds);
});

// 8. 高级采样策略 - 动态采样率
builder.Services.AddSingleton<DynamicSampler>();

// 9. 自定义导出器 - 实现自定义的Span导出器
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddProcessor(new BatchActivityExportProcessor(new CustomSpanExporter()))
    );

// 10. 性能优化 - 使用Span池减少分配
builder.Services.AddSingleton<ActivityPool>();

// 11. 高级指标 - 直方图桶配置
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddView("app.response.time",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new[] { 10, 25, 50, 75, 100, 250, 500, 1000 }
            })
    );

// 12. 分布式追踪上下文增强 - 添加自定义标签
app.Use(async (context, next) =>
{
    var activity = Activity.Current;
    if (activity != null)
    {
        activity.SetTag("tenant.id", context.Request.Headers["X-Tenant-Id"]);
        activity.SetTag("user.id", context.User?.Identity?.Name);
    }
    await next();
});

// 13. 日志增强 - 结构化日志
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeScopes = true;
    logging.ParseStateValues = true;
    logging.AddProcessor(new LogRecordProcessor());
});

// 14. 资源自动发现
builder.Services.AddHostedService<ResourceDiscoveryService>();

app.MapGet("/", () => "OpenTelemetry Production Ready!");

// 自定义Span导出器实现
public class CustomSpanExporter : BaseExporter<Activity>
{
    public override ExportResult Export(in Batch<Activity> batch)
    {
        using var scope = SuppressInstrumentationScope.Begin();
        foreach (var activity in batch)
        {
            // 自定义导出逻辑
        }
        return ExportResult.Success;
    }
}

// 动态采样器
public class DynamicSampler : Sampler
{
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        // 动态采样逻辑
        return new SamplingResult(SamplingDecision.RecordAndSample);
    }
}

// Activity池
public class ActivityPool
{
    private readonly ObjectPool<Activity> _pool;

    public ActivityPool()
    {
        _pool = new DefaultObjectPool<Activity>(
            new ActivityPooledObjectPolicy(),
            maximumRetained: 100);
    }

    public Activity Get() => _pool.Get();
    public void Return(Activity activity) => _pool.Return(activity);
}

// 资源自动发现服务
public class ResourceDiscoveryService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 自动发现资源
        return Task.CompletedTask;
    }
}
app.Run();

// 告警服务实现
public class AlertingService : BackgroundService
{
    private readonly MeterListener _meterListener = new();

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == "MyApp.Metrics")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _meterListener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
        {
            if (instrument.Name == "app.request.count" && measurement > 1000)
            {
                // 触发告警
                Console.WriteLine($"High request count alert: {measurement}");
            }
        });

        _meterListener.Start();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _meterListener?.Dispose();
        base.Dispose();
    }
}

// 1. 定义指标和跟踪
public static class Telemetry
{
    public static readonly string ServiceName = "MyService";
    
    // 定义自定义指标
    public static readonly Meter MyMeter = new(ServiceName);
    public static readonly Counter<long> RequestCounter = 
        MyMeter.CreateCounter<long>(name: "requests", unit: "count", description: "Total requests");
    
    // 定义自定义活动源
    public static readonly ActivitySource MyActivitySource = new(ServiceName);
}

// 2. 配置OpenTelemetry
var builder = WebApplication.CreateBuilder(args);

// 配置资源
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(Telemetry.ServiceName)
    .AddTelemetrySdk()
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
        ["host.name"] = Environment.MachineName
    });

// 配置跟踪
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .SetResourceBuilder(resourceBuilder)
        .AddSource(Telemetry.ServiceName)
        .AddAspNetCoreInstrumentation(options => 
        {
            options.RecordException = true;
            options.Enrich = (activity, eventName, rawObject) =>
            {
                if (eventName == "OnStartActivity")
                {
                    if (rawObject is HttpRequest httpRequest)
                    {
                        activity.SetTag("http.user_agent", httpRequest.Headers.UserAgent);
                    }
                }
            };
        })
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(); // 通常用于Jaeger或Zipkin
});

// 配置指标
builder.Services.AddOpenTelemetryMetrics(metricsProviderBuilder =>
{
    metricsProviderBuilder
        .SetResourceBuilder(resourceBuilder)
        .AddMeter(Telemetry.ServiceName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter();
});

// 配置日志
builder.Logging.AddOpenTelemetry(logging =>
{
    logging.SetResourceBuilder(resourceBuilder)
        .AddConsoleExporter()
        .IncludeFormattedMessage = true;
});

// 3. 使用示例
var app = builder.Build();

// 暴露Prometheus指标端点
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", async (ILogger<Program> logger) =>
{
    // 记录指标
    Telemetry.RequestCounter.Add(1, new("route", "/"), new("method", "GET"));
    
    // 记录日志
    logger.LogInformation("Processing request for /");
    
    // 创建跟踪活动
    using var activity = Telemetry.MyActivitySource.StartActivity("ProcessRequest");
    activity?.SetTag("http.route", "/");
    
    try
    {
        // 模拟工作
        await Task.Delay(100);
        return "Hello World!";
    }
    catch (Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error);
        activity?.RecordException(ex);
        logger.LogError(ex, "Error processing request");
        throw;
    }
});

app.Run();