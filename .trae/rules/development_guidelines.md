---
description: net10+MAUI+blazor-hybrid+Minimal API开发规范
globs: []
alwaysApply: false
---

# 一、技术架构概览
✨ 核心优势：

Blazor Hybrid（WebAssembly + .NET MAUI 混合开发）
Minimal API + Hot Reload
模块化部署（通过 Blazor模块和 MAUI Plugin）
静态编译优化（原生 AOT 支持）
统一跨平台开发体验（iOS / Android / Windows / macOS）
| 技术模块 | 版本 | 说明 |
|---------|------|------|
| .NET SDK | 10.0 | 基于 .NET 10 LTS，支持原生 AOT |
| ASP.NET Core | 10.0 | 高性能 JSON 处理 + gRPC 增强支持 |
| Blazor WebAssembly | 8.0 | 支持 WASM 2.0 + 静态链接优化 |
| .NET MAUI | 8.0 | 跨平台原生应用开发框架 |
| 模块化库 | net10 | 共享逻辑、服务、样式 |
# 二、项目结构规范

```
/Client/                # Blazor WebAssembly 应用 (net10)
│ ├── Pages/           # 路由页面
│ ├── Components/      # 基础组件 (模块化)
│ ├── Services/        # HTTP/本地服务
│ ├── App.razor        # 主应用容器
│ └── Program.cs       # 启动配置
```

```
/Server/                # ASP.NET Core API (net10)
│ ├── Controllers/     # API 端点
│ ├── Models/          # 领域模型
│ ├── DbContexts/      # EF Core 领域上下文
│ └── Program.cs       # 接收请求端口配置 (Kestrel + HTTPS 优化)
```

```
/MauiApp/               # .NET MAUI 应用混合容器(net10)
│ ├── Pages/           # 集成 BlazorWebView 的 MAUI 页面
│ ├── Services/        # 平台相关逻辑 (文件/权限)
│ └── MauiProgram.cs   # MAUI 程序入口 (使用 BlazorWebView2)
```

```
/Common/                # 跨平台共享组件 (net10)
│ ├── Models/          # 领域对象
│ ├── Services/        # 跨平台服务接口
│ └── SharedStyles/    # 主题、样式文件 (MauiCSS + Blazor Styles)
```
# 三、代码质量规范
1. Blazor 组件编写规范

```razor
<!-- 采用 C# 12 record 缩略特性 -->
<Index.razor>
<EditForm Model="model" OnValidSubmit="HandleSubmit">
    <InputText @bind-Value="model.UserName" />
    <ValidationMessage For="@(() => model.UserName)" />
    <button type="submit">提交</button>
</EditForm>

@code {
    [CascadingParameter] 
    public required NavigationManager Navigation { get; set; }

    private readonly UserRegistrationModel model = new();

    private async Task HandleSubmit(EditContext context) 
        => await AuthService.Register(model) ?? Navigation.NavigateTo("/login");

    record UserRegistrationModel : IValidatable 
    {
        [EditRequired(ErrorMessage = "用户名必填")]
        public string? UserName { get; init; }
    }
}
```
2. ASP.NET Core 端点设计
推荐方式（Minimal API）

```csharp
// Program.cs
app.MapGet("/api/weather", (IWeatherService service)
    => Results.Json(
        service.GetReports(), 
        statusCode: 200, 
        cancellationToken: default, 
        options: JsonOptions))
    .WithOpenApi()
    .RequireRateLimiting("ip-10/sec");

app.MapPost("/api/login", (CredentialModel cred, IAuthService service)
    => Results.OkOrUnauthorized(service.Authenticate(cred)))
    .WithProduces(Status401Unauthorized);
```
遗留方式（Controller）

```csharp
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public UserController(IUserRepository repository) => this.Repository = repository;

    [HttpGet]
    public async Task<IActionResult> Get(int page, [FromQuery] int limit)
        => Results.Paginated(
            page, limit, 
            await this.Repository.GetUsers(limit: limit, offset: (page - 1) * limit),
            totalItems: await this.Repository.CountUsers()
        );
}
```
# 四、跨平台适配指南
平台相关服务接口设计

```csharp
// IScreenshotService.cs
public interface IScreenshotService
{
    /// <summary>
    /// 捕获当前屏幕内容
    /// </summary>
    Task<byte[]> CaptureAsync(CancellationToken token = default);
}

// MauiImplementation.cs
#if ANDROID
public class AndroidScreenshotService : IScreenshotService
{
    public async Task<byte[]> CaptureAsync(CancellationToken token)
        => await File.ReadAllTextAsync(DeviceDisplay.MainDisplayInfo.Path);
}
#elif iOS
public class IosScreenshotService : IScreenshotService 
{
    public Task<byte[]> CaptureAsync(CancellationToken token) 
        => Task.FromResult(Encoding.UTF8.GetBytes("iOS dummy capture"));
}
#endif
```
MAUI 平台配置示例

```xml
<!-- Platforms/Android/MauiApp1.Android.csproj -->
<ItemGroup>
    <PackageReference Update="Xamarin.AndroidX.Browser" Version="1.5.1" />
</ItemGroup>

<PropertyGroup>
    <!-- 配置 BlazorWebView 适应刘海屏 -->
    <BlazorWebViewFullScreen>true</BlazorWebViewFullScreen>
    <BlazorWebViewUseShellView>true</BlazorWebViewUseShellView>
</PropertyGroup>
```
# 五、Blazor 混合开发优化
1. WebAssembly 打包优化（最小部署）

```xml
<!-- 项目属性 -->
<ProjectPropertyGroup>
    <TargetFramework>net10</TargetFramework>
    <BlazorWebAssemblyBuildTrimMode>full</BlazorWebAssemblyBuildTrimMode>
    <LinkerPropertyFile>Linker.config</LinkerPropertyFile>
    <BlazorWebAssemblyMaxWasmMemory>256</BlazorWebAssemblyMaxWasmMemory>
</ProjectPropertyGroup>
```

```xml
<!-- Linker.config -->
<linker>
    <assembly fullname="System.Private.CoreLib">
        <remove type="corlib.NativeResources" />
        <type fullname="corlib.Logging">
            <method name="Log" parameters="string" />
        </type>
    </assembly>
</linker>
```
2. 与 MAUI 的通信方式

```csharp
// Blazor 调用原生功能
public partial class PlatformInterop
{
    [JSImport("showCustomAlert", "#MAUIApp")]
    public static partial ValueTask ShowAlert(string title, string content);

    [JSImport("playNativeSound", "#MAUIApp")]
    public static partial ValueTask<bool> PlaySound(string assetName);
}
```
# 六、性能优化策略
| 优化方向 | 实现方式 | 目标 |
|---------|---------|------|
| 首屏加载 | 使用 dotnet workload install maui-net10 预加载框架 | 启动加快 40% |
| AOT 编译 | dotnet publish -p:PublishAot=true -r win10-x64 | 去除 JIT 编译延迟 |
| WebAssembly 压缩 | 采用 wasm-opt -O3 工具链优化 | 压缩体积 35% |
| 模块加载 | 使用 dotnet add <proj> reference <modularproj> | 按需加载组件模块 |
| 资源预加载 | 在 MauiProgram.cs 配置 App.MainPage = new PreloadedContentView(); | 减少冷启动时间 |
# 七、版本迁移检查清单
🔧 从 ASP.NET 6/Blazor 6 升级到 10 必须检查项

依赖升级

```bash
dotnet add package Microsoft.AspNetCore.Components.WebAssembly --version 8.0.0
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.DevServer --version 8.0.0
dotnet add package Microsoft.Maui.Controls --version 8.0.0
```
C# 12 语法升级
使用 record 初始化语法
推荐使用 required 属性标记
throw UnreachableException 代替 null check
依赖配置更新
Program.cs 内部的 ApplicationBuilder 构造方式变更
ASP.NET Core 配置方式改为最小 API API 为主
# 八、兼容性处理方案
| 问题场景 | 解决方案 | 备注 |
|---------|---------|------|
| iOS 隐私权限 | 在 MauiApp/App.cs 的 MauiInitialize 中检查 DeviceInfo.Platform == DevicePlatform.iOS 后注册权限 | Apple 审核强制要求 |
| 低版本设备 | 通过 PlatformDeviceCompatibility 配置最小支持系统版本 | Android 10+，iOS 14+ |
| 字体嵌入 | 在 Platforms/Android/res/values/styles.xml 添加资源映射 | 支持动态字体大小 |
| UI 框架统一 | 使用 MauiCSS + BlazorComponentBase 公共样式基类 | 确保 UI 统一性 |
# 九、典型部署场景

```bash
# Blazor WebAssembly 单位部署
dotnet publish Client -c Release -r wasmlinux-x64 --self-contained false

# MAUI 原生应用打包
dotnet publish MauiApp -c Release -f net10 -r win10-x64 -o out/win
dotnet publish MauiApp -c Release -f net10 -r osx-x64 -o out/mac

# 自托管容器支持
dotnet publish Server -c Release --self-contained true \
  -r linux-x64 \
  --output container \
  --use-target-framework \
  -p:PublishSingleFile=true
```
# 十、维护性优化建议
模块热更新
使用 .blazor.hmr.json 实现 Blazor 模块热更新机制
性能监控
集成 applicationinsights-aspnetcore 收集端点性能指标
平台独立测试
利用 .MauiApp.cs 创建测试专用项目，适配不同设备
通过以上方法，开发者可以高效复用业务逻辑层（Common），实现 Blazor WebAssembly 与 .NET MAUI 的无缝集成，并且利用 .NET 10 的新特性优化跨平台性能和部署复杂度。对于大型企业应用，建议将 Common 层设计为独立 NuGet 包，便于多项目共享和版本管理。