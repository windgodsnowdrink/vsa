#:sdk Microsoft.NET.Sdk.Web
#:package SkyAPM.Agent.AspNetCore@1.6.0
#:package Serilog@3.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Serilog.Context;
using SkyApm.Tracing;

public class TraceContextEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = Tracer.Context;
        if (context != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "TraceId", context.TraceId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "SegmentId", context.SegmentId));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "SpanId", context.SpanId));
        }
    }
}