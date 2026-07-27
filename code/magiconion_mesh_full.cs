#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package Linkerd.Client@0.3.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using MagicOnion;
using Linkerd;

var builder = WebApplication.CreateBuilder();

// 1. Linkerd服务网格配置
builder.Services.AddLinkerdClient(options =>
{
    options.ControlPlaneAddr = "linkerd-proxy-api:8086";
    options.EnableTLS = true;
    options.UseZeroCopy = true;
    options.ConnectionPoolSize = 100;
});

// 2. 网格感知服务注册
builder.Services.AddMagicOnion()
    .AddServiceMeshIntegration(options =>
    {
        options.ServiceName = "MagicOnionService";
        options.ClusterName = "ProductionCluster";
        options.EnableAutoRegistration = true;
    });

var app = builder.Build();
app.MapMagicOnionService();
app.Run();

// 网格集成扩展方法
public static class ServiceMeshExtensions
{
    public static IMagicOnionServerBuilder AddServiceMeshIntegration(
        this IMagicOnionServerBuilder builder,
        Action<ServiceMeshOptions> configure)
    {
        // ... existing code ...
        return builder;
    }
}