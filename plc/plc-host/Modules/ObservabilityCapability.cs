using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// OpenTelemetry 可观测性能力：支撑“可靠性”品质。
/// <para>
/// 通过 OTLP 导出 trace/metrics，使宿主可被 Aspire 仪表盘（或任意 OTLP collector）观察，
/// 无需引入 Aspire.Hosting（那是编排器侧）。本模块即“Aspire 就绪”的服务侧接入。
/// 无 collector 时导出静默失败，不影响宿主启动。
/// </para>
/// </summary>
public sealed class ObservabilityCapability : ICapabilityModule, ICarterModule
{
    public string Id => "obs.otel";

    public string Name => "OpenTelemetry 可观测性（Aspire 就绪）";

    public int Order => 30;

    public void RegisterServices(IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddSource("PLC.Host")
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter())
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter());
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sys/observability", (IConfiguration config) => new
        {
            serviceName = "PLC Plugin Host",
            tracing = "OpenTelemetry (ASP.NET Core instrumentation + OTLP)",
            metrics = "OpenTelemetry (ASP.NET Core instrumentation + OTLP)",
            otlpEndpoint = config["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317",
            aspireDashboard = "经 OTLP 暴露给 Aspire 仪表盘观察；无需 Aspire.Hosting 依赖",
        });
    }
}
