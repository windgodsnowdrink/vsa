#:sdk Microsoft.NET.Sdk.Web
#:package prometheus-net@8.0.0
#:package prometheus-net.AspNetCore@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

using Prometheus;

public static class MetricsRegistry
{
    public static readonly Counter MessagesReceived = Metrics
        .CreateCounter("mqtt_messages_received", "Total received messages");
        
    public static readonly Histogram MessageProcessingTime = Metrics
        .CreateHistogram("mqtt_message_processing_time", "Message processing duration in ms",
            new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(start: 0, width: 50, count: 20)
            });
}