# Photino 技能参考文档

## 1. 技能概述

Photino 技能是一个基于 Photino.NET 的跨平台桌面应用开发框架，允许开发者使用 Web 技术（HTML、CSS、JavaScript）构建原生桌面应用程序。该技能提供了完整的 Photino 集成方案，支持 .NET 10 和 AOT 编译，为开发者提供了高性能、跨平台的桌面应用开发能力。

### 核心特性

- **跨平台支持**：支持 Windows、Linux 和 macOS
- **Web 技术集成**：使用 HTML、CSS、JavaScript 构建用户界面
- **高性能**：基于 WebView2（Windows）、WebKit（macOS）和 WebKitGTK（Linux）
- **轻量级**：最小化应用体积，支持单文件部署
- **.NET 10 支持**：完全兼容 .NET 10 SDK 和运行时
- **AOT 编译**：支持 Ahead-of-Time 编译，提高应用启动速度和运行性能
- **插件系统**：可扩展的插件架构，支持功能模块化
- **本地存储**：内置本地存储功能，支持数据持久化
- **自动更新**：支持应用自动更新机制
- **性能监控**：内置性能指标收集和监控

### 技术栈

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 10.0 | 运行时框架 |
| Photino.NET | 2.0.0 | 核心框架 |
| CommunityToolkit.Mvvm | 8.2.0 | MVVM 模式支持 |
| Microsoft.Extensions.DependencyInjection | 10.0.0 | 依赖注入 |
| Microsoft.Extensions.Logging | 10.0.0 | 日志记录 |
| System.IO.Pipelines | 10.0.0 | 高性能 I/O |
| System.IO.MemoryMappedFiles | 10.0.0 | 共享内存 |
| Microsoft.Data.Sqlite | 10.0.0 | 本地存储 |
| OpenTelemetry | 1.6.0 | 性能监控 |

## 2. 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- 操作系统：
  - Windows 10 或更高版本（需要 WebView2）
  - macOS 10.15 或更高版本
  - Linux（需要 WebKitGTK）

### 安装

1. **安装 .NET 10 SDK**
   从 [Microsoft 官网](https://dotnet.microsoft.com/download/dotnet/10.0) 下载并安装 .NET 10 SDK。

2. **创建项目**
   ```bash
   dotnet new console -n PhotinoApp
   cd PhotinoApp
   ```

3. **添加依赖**
   ```bash
   dotnet add package Photino.NET
   dotnet add package Microsoft.Extensions.DependencyInjection
   dotnet add package Microsoft.Extensions.Logging
   dotnet add package Microsoft.Extensions.Logging.Console
   ```

4. **配置 AOT 编译**
   在项目文件中添加以下配置：
   ```xml
   <PropertyGroup>
     <TargetFramework>net11.0</TargetFramework>
     <PublishAot>true</PublishAot>
     <ReadyToRun>true</ReadyToRun>
     <TieredCompilation>true</TieredCompilation>
     <TrimMode>partial</TrimMode>
     <Optimize>true</Optimize>
   </PropertyGroup>
   ```

### 基本示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Photino;

class Program
{
    static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole());
        
        // 注册 Photino 服务
        services.AddPhotinoServices(options =>
        {
            options.Title = "Photino 示例应用";
            options.Width = 1200;
            options.Height = 900;
            options.StartUrl = "wwwroot/index.html";
            options.UseDeveloperTools = true;
        });
        
        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();
        
        // 获取 Photino 服务
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        
        // 创建并显示窗口
        var window = await photinoService.CreateWindowAsync();
        await window.ShowAsync();
        
        // 等待用户输入
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        
        // 关闭窗口
        await window.CloseAsync();
    }
}
```

### 项目结构

```
PhotinoApp/
├── wwwroot/
│   ├── index.html        # 主页面
│   ├── css/
│   │   └── style.css     # 样式文件
│   └── js/
│       └── app.js        # JavaScript 文件
├── Program.cs            # 主程序
├── PhotinoApp.csproj     # 项目文件
└── photino_integration.setting.json  # 配置文件
```

## 3. 核心功能

### 窗口管理

Photino 技能提供了完整的窗口管理功能，包括创建、显示、隐藏、关闭窗口，以及设置窗口标题、大小、位置等。

#### 创建窗口

```csharp
var window = await photinoService.CreateWindowAsync();
await window.ShowAsync();
```

#### 设置窗口属性

```csharp
await window.SetTitleAsync("新窗口标题");
await window.SetSizeAsync(1024, 768);
```

#### 导航和脚本执行

```csharp
// 导航到 URL
await window.NavigateToAsync("https://example.com");

// 执行 JavaScript
var result = await window.ExecuteJavaScriptAsync("document.title");
Console.WriteLine($"页面标题: {result}");
```

### 消息通信

Photino 技能提供了 .NET 和 Web 端之间的双向通信机制，支持消息发送和接收。

#### 发送消息到 Web 端

```csharp
await photinoService.SendMessageAsync("Hello from .NET!");
```

#### 接收来自 Web 端的消息

```csharp
var message = await photinoService.ReceiveMessageAsync();
Console.WriteLine($"收到消息: {message}");
```

### 插件系统

Photino 技能支持可扩展的插件系统，允许开发者通过插件扩展应用功能。

#### 注册插件

```csharp
await photinoService.RegisterPluginAsync<CustomPlugin>();
```

#### 插件示例

```csharp
public class CustomPlugin : IPhotinoPlugin
{
    public string Name => "CustomPlugin";
    public string Version => "1.0.0";

    public Task InitializeAsync(IPhotinoService photinoService)
    {
        Console.WriteLine("CustomPlugin 初始化成功");
        return Task.CompletedTask;
    }

    public Task<object> ExecuteAsync(string command, params object[] parameters)
    {
        Console.WriteLine($"执行命令: {command}");
        return Task.FromResult<object>("命令执行成功");
    }
}
```

### 本地存储

Photino 技能提供了本地存储功能，支持数据持久化。

#### 获取本地存储

```csharp
var localStorage = await photinoService.GetLocalStorageAsync();
```

#### 使用本地存储

```csharp
// 存储数据
await localStorage.SetAsync("username", "张三");

// 获取数据
var username = await localStorage.GetAsync<string>("username");
Console.WriteLine($"用户名: {username}");

// 删除数据
await localStorage.DeleteAsync("username");

// 清空存储
await localStorage.ClearAsync();
```

### 主题管理

Photino 技能支持应用主题管理，允许开发者和用户切换应用主题。

#### 设置主题

```csharp
await photinoService.SetThemeAsync("dark");
```

### 快捷键注册

Photino 技能支持快捷键注册，允许开发者为应用注册自定义快捷键。

#### 注册快捷键

```csharp
await photinoService.RegisterShortcutAsync("Ctrl+S", () => Console.WriteLine("保存操作"));
```

### 更新检查

Photino 技能支持应用自动更新检查，确保应用始终使用最新版本。

#### 检查更新

```csharp
var updateResult = await photinoService.CheckForUpdatesAsync();
if (updateResult.HasUpdate)
{
    Console.WriteLine($"发现新版本: {updateResult.Version}");
    Console.WriteLine($"更新描述: {updateResult.Description}");
    
    // 应用更新
    await photinoService.ApplyUpdateAsync();
}
else
{
    Console.WriteLine("当前版本已是最新");
}
```

## 4. 配置选项

Photino 技能提供了丰富的配置选项，开发者可以根据需要自定义应用行为。

### 基本配置

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| Title | string | "Photino 应用" | 应用标题 |
| Width | int | 1200 | 窗口宽度 |
| Height | int | 900 | 窗口高度 |
| StartUrl | string | "wwwroot/index.html" | 启动 URL |
| UseDeveloperTools | bool | true | 是否使用开发者工具 |
| LocalStoragePath | string | "localstorage.db" | 本地存储路径 |
| MaxWindows | int | 5 | 最大窗口数 |

### 高级配置

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| PipeName | string | "photino_pipe" | IPC 管道名称 |
| SharedMemorySize | int | 1048576 | 共享内存大小 |
| DefaultTheme | string | "light" | 默认主题 |
| Shortcuts | Dictionary<string, string> | {} | 快捷键配置 |
| UpdateUrl | string | "https://api.example.com/updates" | 更新 URL |
| CrashReportUrl | string | "https://api.example.com/crash-reports" | 崩溃报告 URL |
| EnablePerformanceMetrics | bool | true | 是否启用性能指标 |
| DefaultCulture | string | "zh-CN" | 默认语言 |
| EnableZeroCopy | bool | true | 是否启用零拷贝优化 |
| ThreadLocalCacheSize | int | 1024 | 线程本地缓存大小 |
| EnableParallelProcessing | bool | true | 是否启用并行处理 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |

## 5. 部署指南

### 自包含部署

自包含部署将应用和 .NET 运行时一起打包，无需用户安装 .NET 运行时。

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### AOT 编译部署

AOT 编译将 .NET 代码编译为本地机器代码，提高应用启动速度和运行性能。

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
```

### 跨平台部署

#### Windows

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
```

#### Linux

```bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
```

#### macOS

```bash
dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
```

### Docker 部署

Photino 应用也可以部署到 Docker 容器中，适用于服务器端场景或容器化部署。

#### Dockerfile 示例

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0

WORKDIR /app

COPY bin/Release/net11.0/linux-x64/publish/ .

ENTRYPOINT ["./PhotinoApp"]
```

#### 构建和运行

```bash
docker build -t photino-app .
docker run -it --rm photino-app
```

## 6. 性能优化

### AOT 编译优化

- **启用 AOT 编译**：`-p:PublishAot=true`
- **启用 ReadyToRun**：`-p:ReadyToRun=true`
- **启用分层编译**：`-p:TieredCompilation=true`
- **设置裁剪模式**：`-p:TrimMode=partial`
- **启用优化**：`-p:Optimize=true`

### 内存优化

- **启用零拷贝**：`options.EnableZeroCopy = true`
- **配置线程本地缓存**：`options.ThreadLocalCacheSize = 1024`
- **启用内存池**：在性能设置中启用内存池

### 线程优化

- **启用并行处理**：`options.EnableParallelProcessing = true`
- **配置最大并行度**：`options.MaxDegreeOfParallelism = Environment.ProcessorCount`
- **优化线程池设置**：在运行环境配置中调整线程池参数

### GC 优化

- **使用服务器 GC**：`System.GC.Server = true`
- **启用并发 GC**：`System.GC.Concurrent = true`
- **保留 VM**：`System.GC.RetainVM = true`
- **设置 GC 延迟模式**：`options.GCLatencyMode = GCLatencyMode.Interactive`

## 7. 故障排除

### 常见问题

#### WebView2 未安装（Windows）

**问题**：Windows 系统上运行应用时提示 WebView2 未安装。

**解决方案**：
1. 下载并安装 WebView2 运行时：[Microsoft WebView2 运行时](https://developer.microsoft.com/en-us/microsoft-edge/webview2/#download-section)
2. 或者在应用启动时自动下载和安装 WebView2 运行时。

#### WebKit 未安装（Linux）

**问题**：Linux 系统上运行应用时提示 WebKit 未安装。

**解决方案**：
- Ubuntu/Debian：`sudo apt install libwebkit2gtk-4.0-dev`
- Fedora：`sudo dnf install webkit2gtk3-devel`
- Arch Linux：`sudo pacman -S webkit2gtk`

#### 应用启动缓慢

**问题**：应用启动时间较长。

**解决方案**：
1. 启用 AOT 编译：`-p:PublishAot=true`
2. 启用 ReadyToRun：`-p:ReadyToRun=true`
3. 优化应用资源加载，减少启动时的资源消耗。

#### 内存占用过高

**问题**：应用运行时内存占用过高。

**解决方案**：
1. 启用内存池和零拷贝优化
2. 优化数据结构和算法，减少内存分配
3. 定期清理不再使用的资源
4. 配置合理的 GC 策略

### 日志和调试

#### 启用调试日志

```bash
export PHOTINO_DEBUG=true
export PHOTINO_LOG_LEVEL=Debug
./photino_app
```

#### 查看应用日志

应用日志默认存储在 `logs/photino_integration.log` 文件中，可以通过查看日志文件了解应用运行状态和错误信息。

#### 使用开发者工具

在开发模式下，可以启用开发者工具查看 Web 端的调试信息：

```csharp
options.UseDeveloperTools = true;
```

## 8. 最佳实践

### 应用架构

- **采用 MVVM 模式**：使用 CommunityToolkit.Mvvm 实现 MVVM 模式
- **依赖注入**：使用 Microsoft.Extensions.DependencyInjection 进行依赖注入
- **模块化设计**：将应用功能划分为多个模块，通过插件系统集成
- **异步编程**：使用 async/await 模式处理异步操作

### 性能优化

- **启用 AOT 编译**：提高应用启动速度和运行性能
- **使用零拷贝**：减少内存拷贝，提高 I/O 性能
- **线程本地缓存**：减少线程间竞争，提高并发性能
- **并行处理**：合理使用并行处理，提高 CPU 利用率

### 安全性

- **沙箱模式**：启用沙箱模式，限制应用访问系统资源
- **CORS 配置**：合理配置 CORS 策略，防止跨站请求伪造
- **输入验证**：对用户输入进行严格验证，防止注入攻击
- **安全更新**：定期检查和应用安全更新

### 跨平台兼容性

- **使用条件编译**：针对不同平台使用条件编译
- **测试覆盖**：在所有目标平台上进行测试
- **平台特定代码**：将平台特定代码隔离，使用抽象接口统一调用

## 9. 示例应用

### 简单计算器应用

一个使用 Photino 构建的简单计算器应用，演示了基本的 Photino 集成和 Web 与 .NET 的通信。

### 待办事项应用

一个功能完整的待办事项应用，演示了 Photino 的高级功能，包括本地存储、插件系统和自动更新。

### 图像查看器

一个使用 Photino 构建的图像查看器应用，演示了如何处理本地文件和高性能图像处理。

## 10. 总结

Photino 技能为 .NET 开发者提供了一种使用 Web 技术构建跨平台桌面应用的强大方案。通过支持 .NET 10 和 AOT 编译，该技能不仅提供了高性能的应用运行体验，还简化了应用部署和分发过程。

### 核心优势

- **跨平台**：一套代码，多平台运行
- **Web 技术**：使用熟悉的 HTML、CSS、JavaScript 构建 UI
- **高性能**：支持 AOT 编译，启动速度快，运行性能高
- **轻量级**：最小化应用体积，支持单文件部署
- **可扩展**：插件系统支持功能模块化和扩展
- **易于集成**：与 .NET 生态系统无缝集成

### 适用场景

- **企业内部工具**：快速构建企业内部使用的桌面工具
- **跨平台应用**：需要在多个平台上运行的应用
- **Web 技术迁移**：将现有 Web 应用迁移到桌面
- **原型开发**：快速构建应用原型和演示
- **嵌入式应用**：需要轻量级桌面界面的嵌入式系统

Photino 技能为开发者提供了一种现代化、高性能、跨平台的桌面应用开发方案，是构建下一代桌面应用的理想选择。