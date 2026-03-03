#:sdk Microsoft.NET.Sdk.Web
#:package System.Diagnostics.PerformanceCounter@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0

using System.Diagnostics.Metrics;

public sealed class CarterMetrics
{
    private readonly Meter _meter;
    public readonly Counter<long> RequestCounter;
    public readonly Histogram<double> ResponseTimeHistogram;

    public CarterMetrics()
    {
        _meter = new Meter("Carter.Metrics");
        RequestCounter = _meter.CreateCounter<long>("carter.requests");
        ResponseTimeHistogram = _meter.CreateHistogram<double>(
            "carter.response.time",
            unit: "ms",
            description: "Response time in milliseconds");
    }
}