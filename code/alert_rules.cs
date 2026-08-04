#:sdk Microsoft.NET.Sdk.Web
#:package App.Metrics@4.3.0
#:package App.Metrics.HealthChecks@4.3.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using App.Metrics.Health;

var builder = WebApplication.CreateBuilder();

builder.Services.AddMetricsHealthChecks(health =>
{
    health.Configure(config =>
    {
        config.Check("memory", () =>
        {
            var memory = GC.GetTotalMemory(false);
            return memory > 500 * 1024 * 1024 
                ? HealthCheckResult.Unhealthy("内存使用过高") 
                : HealthCheckResult.Healthy();
        });
    });
});

var app = builder.Build();
app.MapGet("/", () => "Alert Rules Ready");
app.Run();