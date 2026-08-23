using Carter;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// CommunityToolkit.Diagnostics 能力：源码生成的高性能 Guard/ThrowHelper，零分配防御性编程。
/// <para>
/// 当前仅引入 Diagnostics（宿主代码直接受益）。Mvvm 子库记为“未来 HMI 客户端复用”，不在服务器宿主引包。
/// </para>
/// </summary>
public sealed class CommunityToolkitCapability : ICapabilityModule, ICarterModule
{
    public string Id => "diag.toolkit";

    public string Name => "CommunityToolkit.Diagnostics 防御性编程";

    public int Order => 40;

    public void RegisterServices(IServiceCollection services)
    {
        // 纯源码生成库，无需 DI 注册。
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // 演示 Guard：非法输入直接抛标准异常（源码生成，零分配）
        app.MapGet("/sys/guard-demo", (string? name) =>
        {
            Guard.IsNotNullOrWhiteSpace(name, nameof(name));
            Guard.HasSizeLessThanOrEqualTo(name!, 50, nameof(name));
            return Results.Ok(new { greeting = $"Hello, {name}", validatedBy = "CommunityToolkit.Diagnostics.Guard" });
        });
    }
}
