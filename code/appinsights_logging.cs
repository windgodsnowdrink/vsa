#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.ApplicationInsights.AspNetCore@2.21.0
#:package Serilog@3.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Serilog.Context;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder();

// 配置日志上下文
builder.Services.AddSingleton<ITelemetryInitializer, TraceContextInitializer>();
builder.Services.AddSingleton<ILogEventEnricher, TraceContextEnricher>();

var app = builder.Build();
app.MapGet("/", () => "Logging Integration Ready");
app.Run();

public class TraceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = TelemetryConfiguration.Active.TelemetryInitializers
            .OfType<TraceContextInitializer>()
            .FirstOrDefault()?.Context;
        
        if (context != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "OperationId", context.OperationId));
        }
    }
}