#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Extensions.Logging@1.6.0
#:package Serilog@3.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Serilog.Context;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder();

// 配置日志上下文集成
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.ParseStateValues = true;
});

// 日志增强器
builder.Services.AddSingleton<ILogEventEnricher, TraceContextEnricher>();

var app = builder.Build();
app.MapGet("/", () => "Logging Integration Ready");
app.Run();

public class TraceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Tracer.CurrentSpan.Context.TraceId;
        var spanId = Tracer.CurrentSpan.Context.SpanId;
        
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", traceId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", spanId));
    }
}