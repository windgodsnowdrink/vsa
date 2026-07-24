#:sdk Microsoft.NET.Sdk.Web
#:package Datadog.Trace@3.0.0
#:package Serilog@3.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Serilog.Context;

public class LogEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var scope = Tracer.Instance.ActiveScope;
        if (scope != null)
        {
            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                "dd.trace_id", 
                new ScalarValue(scope.Span.TraceId.ToString())));
            
            logEvent.AddPropertyIfAbsent(new LogEventProperty(
                "dd.span_id", 
                new ScalarValue(scope.Span.SpanId.ToString())));
        }
    }
}