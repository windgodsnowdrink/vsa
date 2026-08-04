#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package OpenTelemetry.Exporter.Console@1.7.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:package Polly@8.3.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using MagicOnion;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;

var builder = WebApplication.CreateBuilder();

// 1. 服务网格集成
builder.Services.AddServiceMeshIntegration(options => 
{
    options.ServiceName = "MagicOnionService";
    options.ClusterName = "ProductionCluster";
});

// 2. 可观测性增强
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

// 3. 混沌工程支持
builder.Services.AddChaosEngineering(options => 
{
    options.LatencyInjection = TimeSpan.FromMilliseconds(100);
    options.ErrorRate = 0.01;
});

var app = builder.Build();
app.MapGet("/", () => "Observable MagicOnion Ready");
app.Run();

// 服务网格集成器
[SkipLocalsInit]
public static class ServiceMeshExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static IServiceCollection AddServiceMeshIntegration(this IServiceCollection services, Action<ServiceMeshOptions> configure)
    {
        // ... existing code ...
        return services;
    }
}

// 混沌工程策略
[SkipLocalsInit]
public static class ChaosPolicyBuilder
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static IAsyncPolicy<HttpResponseMessage> BuildChaosPolicy()
    {
        // ... existing code ...
        return Policy.NoOpAsync<HttpResponseMessage>();
    }
}