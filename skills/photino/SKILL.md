# Photino Agent Skill - Photino 技能

## 技能概述

基于 .NET 10 的高性能 Photino 技能，为 .NET 开发者提供强大的跨平台桌面应用开发能力。该技能支持 AOT 编译，具有高性能、可扩展性和易用性等特点，适用于各种跨平台桌面应用场景。

## 快速开始指南

### 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- 基本的 Web 开发知识（HTML、CSS、JavaScript）

### 安装依赖

在你的主应用程序的运行文件中添加以下依赖：

```yaml
#:sdk Microsoft.NET.Sdk.Web
#:package Photino.NET@2.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package CommunityToolkit.Mvvm@8.2.0
#:package Microsoft.Data.Sqlite@10.0.0
#:package Microsoft.EntityFrameworkCore.Sqlite@10.0.0
#:package System.IO.Pipelines@10.0.0
#:package System.IO.MemoryMappedFiles@10.0.0
#:package System.Composition@10.0.0
#:package Microsoft.Extensions.Localization@10.0.0
#:package Squirrel@2.0.0
#:package OpenTelemetry@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true
```

### 注册服务

在你的主应用程序中注册 Photino 服务：

```csharp
// 注册 Photino 服务
builder.Services.AddPhotinoServices(options =>
{
    options.Title = "Photino 应用";
    options.Width = 1200;
    options.Height = 900;
    options.StartUrl = "wwwroot/index.html";
    options.UseDeveloperTools = true;
    options.LocalStoragePath = "localstorage.db";
    options.MaxWindows = 5;
    options.EnablePerformanceMetrics = true;
    options.EnableZeroCopy = true;
    options.ThreadLocalCacheSize = 1024;
});
```

### 使用示例

```csharp
// 获取 Photino 服务
var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();

// 创建主窗口
var mainWindow = await photinoService.CreateWindowAsync();

// 显示窗口
await mainWindow.ShowAsync();

// 发送消息到 Web 端
await photinoService.SendMessageAsync("Hello from .NET!");

// 接收来自 Web 端的消息
var message = await photinoService.ReceiveMessageAsync();
Console.WriteLine($"Received message: {message}");

// 注册插件
await photinoService.RegisterPluginAsync<CustomPlugin>();

// 检查更新
var updateResult = await photinoService.CheckForUpdatesAsync();
if (updateResult.HasUpdate)
{
    Console.WriteLine("有新版本可用！");
}
```

## 导航地图

```
photino/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── photino_integration.cs     # Photino 集成功能实现
    ├── photino_integration.run.json  # 运行配置
    └── photino_integration.setting.json  # 设置文件
```

## 核心功能

1. **跨平台桌面应用开发**：支持 Windows、Linux、macOS 等多个平台
2. **Web 技术与原生能力融合**：使用 HTML、CSS、JavaScript 开发界面，同时访问原生 API
3. **轻量级应用部署**：体积小，启动快，资源占用低
4. **高性能渲染**：优化的渲染引擎，提供流畅的用户体验
5. **丰富的原生 API 访问**：访问文件系统、系统托盘、通知等原生功能
6. **插件系统支持**：可扩展的插件架构，支持功能模块化
7. **自动更新机制**：内置应用自动更新功能
8. **多语言支持**：内置本地化系统，支持多语言界面
9. **主题定制**：支持浅色/深色主题，可自定义样式
10. **快捷键系统**：可配置的全局快捷键

## 技术特性

1. **模块化设计**：采用模块化架构，便于扩展和维护
2. **依赖注入**：支持 .NET 依赖注入，便于服务管理
3. **异步编程**：使用 async/await 模式，提高并发性能
4. **高性能算法**：实现高效的渲染和消息处理算法
5. **内存优化**：采用内存池、零拷贝等技术，减少内存使用
6. **错误处理与重试机制**：提供完善的错误处理和重试逻辑
7. **状态管理**：使用状态机管理应用状态
8. **管道处理模式**：使用管道模式处理消息和事件

## 性能特性

1. **高性能设计**：优化的性能实现，支持高并发
2. **内存优化**：减少内存使用，提高内存效率
3. **并发支持**：支持并行处理，提高处理速度
4. **异步编程**：使用异步 API，避免阻塞
5. **批处理**：支持批处理，提高效率
6. **缓存机制**：使用缓存，减少重复计算
7. **零拷贝技术**：使用零拷贝技术，提高数据传输速度
8. **线程本地缓存**：使用线程本地缓存，减少线程竞争

## AOT 编译支持

Photino 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 编译选项

```yaml
# AOT 编译选项
PublishAot: true      # 启用 AOT 编译
ReadyToRun: true      # 启用 ReadyToRun 编译
TieredCompilation: true  # 启用分层编译
TrimMode: partial     # 剪裁模式
Optimize: true        # 启用优化
EnableCompilationRelaxations: true  # 启用编译松弛
EnableEnhancedNgen: true  # 启用增强的 Ngen
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

### 编译命令

```bash
# 使用 AOT 编译 Photino 技能
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true
```

## 扩展说明

Photino 技能提供了完整的跨平台桌面应用解决方案，你可以根据需要进行扩展：

1. **自定义插件**：实现 IPhotinoPlugin 接口，添加自定义功能
2. **扩展原生 API**：添加新的原生功能访问能力
3. **与其他系统集成**：将 Photino 与其他系统集成，如数据库、云服务等
4. **性能优化**：针对特定场景优化性能，如大量数据处理、复杂 UI 渲染等

### 示例：自定义插件

```csharp
public class CustomPlugin : IPhotinoPlugin
{
    public string Name => "CustomPlugin";
    public string Version => "1.0.0";
    
    public Task InitializeAsync(IPhotinoService photinoService)
    {
        Console.WriteLine("CustomPlugin initialized");
        return Task.CompletedTask;
    }
    
    public Task ExecuteAsync(string command, params object[] parameters)
    {
        Console.WriteLine($"Executing command: {command}");
        return Task.CompletedTask;
    }
}
```

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，便于测试和扩展
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，提供适当的错误消息
4. **日志记录**：添加适当的日志记录，便于故障排除
5. **性能监控**：监控关键性能指标，如启动时间、内存使用等
6. **资源管理**：正确管理资源，避免资源泄漏
7. **配置管理**：使用配置文件或环境变量管理配置，便于部署和维护
8. **安全实践**：遵循安全最佳实践，如验证输入参数
9. **用户体验**：注重用户体验，提供流畅的界面和响应迅速的操作
10. **可访问性**：确保应用对所有用户都可访问

## 配置选项

Photino 技能提供了丰富的配置选项，可以根据需要进行调整：

### 基本配置

```json
{
  "PhotinoOptions": {
    "Title": "Photino 应用",            // 应用标题
    "Width": 1200,                     // 窗口宽度
    "Height": 900,                     // 窗口高度
    "StartUrl": "wwwroot/index.html",  // 启动 URL
    "UseDeveloperTools": true,         // 是否使用开发者工具
    "LocalStoragePath": "localstorage.db",  // 本地存储路径
    "MaxWindows": 5,                   // 最大窗口数
    "EnablePerformanceMetrics": true,  // 是否启用性能指标
    "EnableZeroCopy": true,            // 是否启用零拷贝优化
    "ThreadLocalCacheSize": 1024       // 线程本地缓存大小
  }
}
```

### 性能配置

```json
{
  "PhotinoPerformance": {
    "EnableParallelProcessing": true,  // 是否启用并行处理
    "MaxDegreeOfParallelism": 4,       // 最大并行度
    "EnableBatching": true,            // 是否启用批处理
    "BatchSize": 100,                  // 批处理大小
    "EnableCaching": true,             // 是否启用缓存
    "CacheSize": 1000,                 // 缓存大小
    "CacheDuration": "00:05:00"        // 缓存持续时间
  }
}
```

## 部署指南

### 自包含部署

```bash
# 构建自包含部署包
dotnet publish -c Release -r win-x64 --self-contained true

# 运行应用
./bin/Release/net11.0/win-x64/publish/YourApp.exe
```

### AOT 编译部署

```bash
# 使用 AOT 编译构建
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true

# 运行应用
./bin/Release/net11.0/win-x64/publish/YourApp.exe
```

### 容器化部署

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0-windowsservercore-ltsc2022 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["YourApp.csproj", "."]
RUN dotnet restore "YourApp.csproj"
COPY . .
WORKDIR "/src/YourApp"
RUN dotnet build "YourApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YourApp.csproj" -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["YourApp.exe"]
```

## 故障排除

### 常见问题

1. **窗口创建失败**
   - 检查系统权限
   - 验证依赖项是否正确安装
   - 检查日志文件获取详细错误信息

2. **Web 端与 .NET 端通信失败**
   - 确保消息格式正确
   - 检查网络连接
   - 验证插件是否正确注册

3. **性能问题**
   - 启用缓存
   - 优化 Web 资源
   - 减少 DOM 操作
   - 检查内存使用情况

4. **跨平台兼容性问题**
   - 避免使用平台特定的 API
   - 测试所有目标平台
   - 使用条件编译处理平台差异

### 日志记录

Photino 技能提供了详细的日志记录，可以帮助诊断问题：

```json
{
  "logging": {
    "logLevel": {
      "Default": "Information",
      "Photino": "Debug"  // 设置 Photino 相关日志为 Debug 级别
    }
  }
}
```

## 结论

Photino 技能是一个功能强大的跨平台桌面应用开发解决方案，可以帮助开发者使用 Web 技术构建高性能的桌面应用。该技能支持 .NET 10 AOT 编译，具有高性能、可扩展性和易用性等特点，适用于各种规模的项目。

通过本文档，你应该已经了解了 Photino 技能的核心功能、技术特性、使用方法和最佳实践。如果你有任何问题或建议，请参考参考文档或联系 VSA Architecture Team。
