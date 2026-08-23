using System.Linq;
using Carter;
using Dapr.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// Dapr 能力（opt-in，按 ADR-116 决策）：仅在 <c>Capabilities:Dapr:Enabled=true</c> 时注册 <c>AddDaprClient</c>。
/// <para>
/// 对应需求：Pub/Sub→AIOT 事件总线、State→设备影子、Virtual Actors→设备会话、Workflow→编排。
/// 未启用时端点返回降级说明，不影响宿主启动；真正调用需在 sidecar 在线时执行。
/// </para>
/// </summary>
public sealed class DaprCapability : ICapabilityModule, ICarterModule
{
    public string Id => "dapr";

    public string Name => "Dapr（opt-in：AIOT 事件总线/设备会话）";

    public int Order => 50;

    public void RegisterServices(IServiceCollection services)
    {
        // 不构建临时 ServiceProvider（反模式）；直接从已注册的单例 IConfiguration 描述符取实例。
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
        var config = descriptor?.ImplementationInstance as IConfiguration;
        var enabled = config?.GetValue<bool>("Capabilities:Dapr:Enabled") ?? false;
        if (enabled)
        {
            services.AddDaprClient();
        }
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sys/dapr/status", (IConfiguration config) =>
        {
            var enabled = config.GetValue<bool>("Capabilities:Dapr:Enabled");
            return Results.Ok(new
            {
                enabled,
                note = enabled
                    ? "Dapr sidecar 应已就绪；可调用 Pub/Sub、State、Actors、Workflow"
                    : "opt-in 未启用；设置 Capabilities:Dapr:Enabled=true 并部署 sidecar 后生效",
            });
        });
    }
}
