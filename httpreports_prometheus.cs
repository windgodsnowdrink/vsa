#:sdk Microsoft.NET.Sdk.Web
#:package HttpReports@3.0.0
#:package Prometheus.Client.Http@4.3.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using HttpReports;
using Prometheus.Client;

var builder = WebApplication.CreateBuilder();

// 配置Prometheus导出
builder.Services.AddHttpReportsPrometheus(options =>
{
    options.MetricPrefix = "httpreports_";
    options.ExportInterval = TimeSpan.FromSeconds(15);
    options.Counters = new List<string>
    {
        "requests_total",
        "errors_total",
        "response_time_seconds"
    };
});

var app = builder.Build();
app.UseHttpReports();
app.MapGet("/", () => "HttpReports with Prometheus Ready");
app.Run();