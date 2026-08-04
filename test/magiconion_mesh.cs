#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package Linkerd.Client@0.3.0
#:property LangVersion=preview
#:property TargetFramework=net11.0

// 服务网格配置
builder.Services.AddLinkerdClient(options =>
{
    options.ControlPlaneAddr = "linkerd-proxy-api:8086";
    options.EnableTLS = true;
    options.UseZeroCopy = true;
});

// 网格感知的MagicOnion配置
services.AddMagicOnion()
    .AddServiceMeshIntegration();