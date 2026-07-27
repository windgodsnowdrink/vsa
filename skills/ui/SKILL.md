# UI 技能文档

## 1. 技能概述

UI 技能是一个基于 .NET 10 的全功能用户界面开发框架，支持 Web、桌面和移动应用的开发与部署。该技能提供了统一的 UI 开发体验，同时具备高性能、跨平台和易于扩展的特性。

### 主要特性

- **多平台支持**：同时支持 Web、桌面和移动应用开发
- **高性能**：采用 AOT 编译技术，提供原生执行性能
- **统一开发体验**：使用相同的代码基础构建不同平台的应用
- **主题管理**：内置支持浅色/深色主题切换
- **本地化**：多语言支持，轻松实现国际化
- **性能监控**：实时跟踪 UI 性能指标
- **无障碍支持**：符合 WCAG 标准的无障碍设计
- **模块化架构**：易于扩展和定制

## 2. 技术栈

### 核心技术

- **.NET 10**：最新的 .NET 框架，提供 AOT 编译支持
- **C#**：主要开发语言
- **Blazor**：用于 Web UI 开发
- **MAUI**：用于桌面和移动 UI 开发
- **ASP.NET Core**：Web 服务器和 API 框架
- **SignalR**：实时通信

### 依赖库

- **Microsoft.Extensions.DependencyInjection**：依赖注入
- **Scrutor**：装饰器模式实现
- **System.CommandLine**：命令行界面
- **Microsoft.Extensions.Logging**：日志记录
- **Microsoft.Extensions.Configuration**：配置管理
- **Microsoft.AspNetCore.Components.Web**：Blazor Web 组件
- **Microsoft.Maui**：跨平台 UI 框架
- **Microsoft.AspNetCore.SignalR.Client**：实时通信客户端

## 3. 快速开始

### 环境要求

- **.NET 10 SDK**：确保安装了最新版本的 .NET 10 SDK
- **IDE**：推荐使用 Visual Studio 2022 或 Rider
- **操作系统**：Windows 11、macOS 13+ 或 Linux（支持 .NET 10 的发行版）

### 安装与配置

1. **克隆项目**

```bash
git clone <repository-url>
cd <project-directory>/ui
```

2. **安装依赖**

```bash
dotnet restore
```

3. **构建项目**

```bash
dotnet build -c Release
```

4. **发布项目（AOT 编译）**

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
```

### 运行示例

```bash
# 运行核心 UI 示例
dotnet run --project scripts/ui_core.cs

# 运行 Scrutor 示例
dotnet run --project scripts/ui_generator.cs
```

## 4. 核心功能

### 4.1 多平台 UI 支持

UI 技能提供了统一的接口，支持在不同平台上构建应用：

- **Web UI**：基于 Blazor 的单页应用
- **桌面 UI**：基于 MAUI 的桌面应用（Windows、macOS、Linux）
- **移动 UI**：基于 MAUI 的移动应用（iOS、Android）

### 4.2 主题管理

内置主题管理系统，支持：

- 浅色/深色主题切换
- 自定义主题创建
- 主题持久化
- 系统主题自动同步

### 4.3 本地化

完整的本地化支持：

- 多语言资源文件
- 运行时语言切换
- 区域设置自动检测
- 日期、时间、数字格式本地化

### 4.4 性能监控

实时性能监控功能：

- UI 渲染时间跟踪
- 内存使用监控
- 网络请求分析
- 性能指标可视化

### 4.5 无障碍支持

符合 WCAG 2.1 标准的无障碍功能：

- 屏幕阅读器支持
- 键盘导航优化
- 颜色对比度检查
- 语义化 HTML 结构

### 4.6 实时通信

基于 SignalR 的实时通信：

- 服务器推送通知
- 实时数据更新
- 客户端状态同步
- 消息广播

## 5. API 参考

### 5.1 核心接口

#### IUIApplication

```csharp
public interface IUIApplication
{
    Task RunAsync();
    void ConfigureServices(IServiceCollection services);
    void Configure(IApplicationBuilder app);
}
```

#### IThemeManager

```csharp
public interface IThemeManager
{
    Theme CurrentTheme { get; }
    Task SetThemeAsync(Theme theme);
    Task ToggleThemeAsync();
    event EventHandler<ThemeChangedEventArgs> ThemeChanged;
}
```

#### ILocalizationManager

```csharp
public interface ILocalizationManager
{
    string CurrentCulture { get; }
    Task SetCultureAsync(string culture);
    string GetString(string key);
    event EventHandler<CultureChangedEventArgs> CultureChanged;
}
```

#### IPerformanceMonitor

```csharp
public interface IPerformanceMonitor
{
    void StartMeasurement(string operationName);
    void StopMeasurement(string operationName);
    IReadOnlyDictionary<string, PerformanceMetrics> GetMetrics();
}
```

#### IAccessibilityChecker

```csharp
public interface IAccessibilityChecker
{
    Task<AccessibilityReport> CheckAsync();
    bool IsCompliant(AccessibilityStandard standard);
}
```

### 5.2 UI 类型

#### WebUI

基于 Blazor 的 Web 界面实现，支持：

- 单页应用
- 服务器端渲染
- 渐进式 Web 应用 (PWA)
- 响应式设计

#### DesktopUI

基于 MAUI 的桌面界面实现，支持：

- Windows 桌面应用
- macOS 桌面应用
- Linux 桌面应用
- 系统原生控件

#### MobileUI

基于 MAUI 的移动界面实现，支持：

- iOS 应用
- Android 应用
- 触摸优化
- 设备传感器集成

## 6. 配置管理

### 6.1 配置文件

UI 技能使用标准的 .NET 配置系统，支持以下配置源：

- **appsettings.json**：主配置文件
- **appsettings.{Environment}.json**：环境特定配置
- **环境变量**：运行时配置覆盖
- **命令行参数**：启动时配置

### 6.2 核心配置选项

| 配置项 | 类型 | 默认值 | 描述 |
|-------|------|-------|------|
| UI:Type | string | "Web" | UI 类型（Web、Desktop、Mobile） |
| UI:Theme | string | "Light" | 默认主题（Light、Dark、System） |
| UI:Culture | string | "en-US" | 默认文化（如 zh-CN、en-US） |
| UI:PerformanceMonitoring | bool | true | 是否启用性能监控 |
| UI:AccessibilityChecks | bool | true | 是否启用无障碍检查 |
| Web:Port | int | 8080 | Web 服务器端口 |
| Web:Host | string | "localhost" | Web 服务器主机 |
| Web:UseHttps | bool | true | 是否使用 HTTPS |

## 7. 部署选项

### 7.1 本地开发

```bash
# 运行 Web UI
dotnet run --project scripts/ui_core.cs --ui-type web

# 运行桌面 UI
dotnet run --project scripts/ui_core.cs --ui-type desktop

# 运行移动 UI（需要相应平台 SDK）
dotnet run --project scripts/ui_core.cs --ui-type mobile
```

### 7.2 Docker 部署

**Dockerfile**

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .
RUN dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true -o out

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["./ui_core"]
```

**构建和运行**

```bash
docker build -t ui-app .
docker run -p 8080:8080 ui-app
```

### 7.3 Kubernetes 部署

**deployment.yaml**

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ui-app
spec:
  replicas: 3
  selector:
    matchLabels:
      app: ui-app
  template:
    metadata:
      labels:
        app: ui-app
    spec:
      containers:
      - name: ui-app
        image: ui-app:latest
        ports:
        - containerPort: 8080
        env:
        - name: UI__Type
          value: "Web"
        - name: Web__Host
          value: "0.0.0.0"
---
apiVersion: v1
kind: Service
metadata:
  name: ui-app
spec:
  selector:
    app: ui-app
  ports:
  - port: 80
    targetPort: 8080
  type: LoadBalancer
```

### 7.4 CI/CD 集成

#### GitHub Actions

```yaml
name: UI Skill CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore ui/
    - name: Build
      run: dotnet build ui/ --configuration Release
    - name: Test
      run: dotnet test ui/ --configuration Release
    - name: Publish
      run: dotnet publish ui/ --configuration Release --runtime linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: ui-app
        path: ui/bin/Release/net11.0/linux-x64/publish/
```

#### Azure DevOps

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '10.0.x'
    includePreviewVersions: true

- script: dotnet restore ui/
  displayName: 'Restore dependencies'

- script: dotnet build ui/ --configuration Release
  displayName: 'Build'

- script: dotnet test ui/ --configuration Release
  displayName: 'Test'

- script: dotnet publish ui/ --configuration Release --runtime linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
  displayName: 'Publish'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: 'ui/bin/Release/net11.0/linux-x64/publish/'
    ArtifactName: 'ui-app'
```

## 8. Scrutor 使用指南

### 8.1 什么是 Scrutor？

Scrutor 是一个 .NET 库，用于增强依赖注入容器的功能，特别是提供了装饰器模式的简洁实现方式。它允许你：

- 自动扫描程序集并注册服务
- 轻松实现装饰器模式
- 基于约定的服务注册
- 灵活的服务筛选和注册规则

### 8.2 基本用法

```csharp
// 扫描程序集并注册服务
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses()
    .AsImplementedInterfaces()
    .WithTransientLifetime());

// 实现装饰器模式
services.AddTransient<IMyService, MyService>();
services.Decorate<IMyService, MyServiceDecorator>();
```

### 8.3 高级用法

#### 多个装饰器

```csharp
services.AddTransient<IMyService, MyService>();
services.Decorate<IMyService, LoggingDecorator>();
services.Decorate<IMyService, CachingDecorator>();
services.Decorate<IMyService, ValidationDecorator>();
```

#### 条件注册

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithLifetimeFromAttributes());
```

#### 生命周期管理

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses()
    .AsImplementedInterfaces()
    .WithTransientLifetime()
    .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

### 8.4 在 UI 技能中的应用

UI 技能广泛使用 Scrutor 来实现：

- **主题管理装饰器**：在主题变更时添加日志和性能监控
- **本地化装饰器**：缓存本地化字符串，提高性能
- **性能监控装饰器**：跟踪 UI 操作的执行时间
- **无障碍检查装饰器**：在 UI 渲染时执行无障碍检查

## 9. 最佳实践

### 9.1 架构设计

- **模块化**：将 UI 功能拆分为独立的模块
- **依赖注入**：充分利用依赖注入，避免硬编码依赖
- **装饰器模式**：使用 Scrutor 实现横切关注点
- **接口分离**：为不同的功能定义清晰的接口

### 9.2 性能优化

- **延迟加载**：仅在需要时加载 UI 组件
- **缓存**：缓存频繁使用的数据和计算结果
- **虚拟化**：对大型列表和网格使用虚拟化
- **批处理**：批量处理 UI 更新，减少渲染次数
- **AOT 编译**：使用 AOT 编译提高启动性能和运行时性能

### 9.3 可维护性

- **一致的命名约定**：使用清晰、一致的命名约定
- **文档**：为公共 API 和复杂逻辑提供文档
- **测试**：编写单元测试和集成测试
- **代码组织**：按功能和责任组织代码

### 9.4 安全性

- **输入验证**：验证所有用户输入
- **HTTPS**：在生产环境中使用 HTTPS
- **安全头**：配置适当的安全 HTTP 头
- **权限检查**：实现适当的权限检查

## 10. 故障排除

### 10.1 常见问题

| 问题 | 可能原因 | 解决方案 |
|-----|---------|--------|
| Web UI 无法启动 | 端口被占用 | 更改 Web:Port 配置 |
| 主题不生效 | 缓存问题 | 清除浏览器缓存或应用缓存 |
| 本地化失败 | 缺少资源文件 | 确保添加了相应语言的资源文件 |
| 性能下降 | 内存泄漏 | 检查是否正确释放资源，使用性能监控工具 |
| 无障碍检查失败 | 不符合 WCAG 标准 | 检查 UI 组件是否符合无障碍标准 |

### 10.2 日志和诊断

UI 技能使用结构化日志，可配置不同的日志级别：

```bash
# 启用详细日志
dotnet run --project scripts/ui_core.cs --log-level debug
```

### 10.3 调试技巧

- **使用浏览器开发工具**：调试 Web UI 时使用浏览器的开发工具
- **MAUI 调试器**：调试桌面和移动应用时使用 MAUI 调试器
- **性能分析器**：使用 .NET 性能分析器识别瓶颈
- **日志分析**：分析应用日志以识别问题

## 11. 示例项目

UI 技能包含以下示例项目：

- **BasicWebApp**：基本的 Web 应用示例
- **DesktopApp**：桌面应用示例
- **MobileApp**：移动应用示例
- **ThemeDemo**：主题管理示例
- **LocalizationDemo**：本地化示例
- **PerformanceDemo**：性能监控示例
- **AccessibilityDemo**：无障碍支持示例

这些示例可以在 `reference/examples.md` 文件中找到详细说明和代码示例。

## 12. 总结

UI 技能是一个功能强大、性能优异的用户界面开发框架，为 .NET 开发者提供了统一的跨平台 UI 开发体验。通过结合 AOT 编译、装饰器模式和现代化的 UI 框架，它能够满足从简单工具到复杂企业应用的各种 UI 开发需求。

### 优势

- **高性能**：AOT 编译提供原生执行性能
- **跨平台**：单一代码库支持多平台部署
- **易于扩展**：模块化架构和装饰器模式
- **现代化**：基于最新的 .NET 10 和 UI 框架
- **企业级**：内置安全、性能监控和无障碍支持

### 适用场景

- **企业 Web 应用**：基于 Blazor 的现代化 Web 应用
- **跨平台桌面应用**：使用 MAUI 构建的桌面解决方案
- **移动应用**：针对 iOS 和 Android 的移动应用
- **内部工具**：快速构建的内部业务工具
- **原型开发**：快速原型设计和验证

UI 技能为开发者提供了一套完整的工具和框架，使他们能够专注于业务逻辑的实现，而不是底层的 UI 技术细节，从而提高开发效率和应用质量。