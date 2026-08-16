// =========================================================
// PlcAiot.WebApi — 内网控制台后端
// 职责：
//   1. 托管 wwwroot 静态前端（HTMX + Axios 单页外壳）
//   2. 暴露 /api/{resource} REST 端点（演示用种子数据，
//      生产替换为领域服务 / PostgreSQL / MQTT 桥接）
// 技术基线：.NET 10 LTS + ASP.NET Core Minimal API
// =========================================================
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

// 端口 / 允许的前端源 / 静态根路径均可在 appsettings 与环境变量中覆盖
var app = builder.Build();

// ---------- 1. 静态前端（wwwroot 控制台 SPA） ----------
// 默认取项目自带 wwwroot；容器或本地可经 StaticWebRoot 指向仓库 wwwroot
var webRoot = builder.Configuration["StaticWebRoot"]
              ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
if (Directory.Exists(webRoot))
{
    var provider = new PhysicalFileProvider(webRoot);
    app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = provider, RequestPath = "" });
    app.UseStaticFiles(new StaticFileOptions { FileProvider = provider, RequestPath = "" });
}

// ---------- 2. 种子数据（演示） ----------
var seedFile = Path.Combine(builder.Environment.ContentRootPath, "SeedData.json");
JsonObject? seed = File.Exists(seedFile)
    ? JsonNode.Parse(await File.ReadAllTextAsync(seedFile))?.AsObject()
    : null;

var api = app.MapGroup("/api");

// 健康检查（容器探针 / 负载均衡用）
api.MapGet("/health", () => Results.Ok(new { status = "healthy", time = DateTime.UtcNow }));

// 统一资源端点：/api/{resource} 返回 SeedData 对应子树
// 资源键：dashboard / devices / alarms / statistics / realtime
//        / map / system / admin / social / notion
api.MapGet("/{resource}", (string resource) =>
{
    if (seed is null || !seed.TryGetPropertyValue(resource, out var node) || node is null)
        return Results.NotFound(new { error = $"未知资源: {resource}" });

    return Results.Content(
        node.ToJsonString(new JsonSerializerOptions { WriteIndented = false }),
        "application/json; charset=utf-8");
});

app.Run();

// 便于集成测试的可选访问点（非必需）
public partial class Program { }
