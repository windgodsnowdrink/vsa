# UI 技能参考文档

## 1. 概述

本参考文档提供了 UI 技能的详细技术信息，包括 API 参考、配置选项、使用示例和最佳实践。UI 技能是一个基于 .NET 10 的全功能用户界面开发框架，支持 Web、桌面和移动应用的开发与部署。

## 2. 核心 API

### 2.1 IUIApplication

```csharp
public interface IUIApplication
{
    Task RunAsync();
    void ConfigureServices(IServiceCollection services);
    void Configure(IConfiguration configuration);
}
```

**功能**：UI 应用程序的核心接口，负责协调 UI 组件的初始化、渲染和关闭。

**方法**：
- `RunAsync()`：运行 UI 应用程序
- `ConfigureServices(IServiceCollection services)`：配置依赖注入服务
- `Configure(IConfiguration configuration)`：配置应用程序设置

### 2.2 IUIProvider

```csharp
public interface IUIProvider
{
    UIType Type { get; }
    Task InitializeAsync();
    Task RenderAsync();
    Task ShutdownAsync();
}
```

**功能**：UI 提供程序接口，定义了不同类型 UI 的通用操作。

**属性**：
- `Type`：UI 类型（Web、Desktop、Mobile）

**方法**：
- `InitializeAsync()`：初始化 UI 提供程序
- `RenderAsync()`：渲染 UI
- `ShutdownAsync()`：关闭 UI 提供程序

### 2.3 IThemeManager

```csharp
public interface IThemeManager
{
    Theme CurrentTheme { get; }
    Task SetThemeAsync(Theme theme);
    Task ToggleThemeAsync();
    event EventHandler<ThemeChangedEventArgs> ThemeChanged;
}
```

**功能**：主题管理接口，负责处理应用程序的主题设置。

**属性**：
- `CurrentTheme`：当前主题

**方法**：
- `SetThemeAsync(Theme theme)`：设置主题
- `ToggleThemeAsync()`：切换主题（浅色/深色）

**事件**：
- `ThemeChanged`：主题变更时触发

### 2.4 ILocalizationManager

```csharp
public interface ILocalizationManager
{
    string CurrentCulture { get; }
    Task SetCultureAsync(string culture);
    string GetString(string key);
    event EventHandler<CultureChangedEventArgs> CultureChanged;
}
```

**功能**：本地化管理接口，负责处理应用程序的语言设置。

**属性**：
- `CurrentCulture`：当前文化（语言）

**方法**：
- `SetCultureAsync(string culture)`：设置文化
- `GetString(string key)`：获取本地化字符串

**事件**：
- `CultureChanged`：文化变更时触发

### 2.5 IPerformanceMonitor

```csharp
public interface IPerformanceMonitor
{
    void StartMeasurement(string operationName);
    void StopMeasurement(string operationName);
    IReadOnlyDictionary<string, PerformanceMetrics> GetMetrics();
}
```

**功能**：性能监控接口，负责跟踪应用程序的性能指标。

**方法**：
- `StartMeasurement(string operationName)`：开始性能测量
- `StopMeasurement(string operationName)`：停止性能测量
- `GetMetrics()`：获取性能指标

### 2.6 IAccessibilityChecker

```csharp
public interface IAccessibilityChecker
{
    Task<AccessibilityReport> CheckAsync();
    bool IsCompliant(AccessibilityStandard standard);
}
```

**功能**：无障碍检查接口，负责评估应用程序的无障碍合规性。

**方法**：
- `CheckAsync()`：执行无障碍检查
- `IsCompliant(AccessibilityStandard standard)`：检查是否符合特定的无障碍标准

## 3. 配置选项

### 3.1 核心配置

| 配置项 | 类型 | 默认值 | 描述 |
|-------|------|-------|------|
| UI:Type | string | "Web" | UI 类型（Web、Desktop、Mobile） |
| UI:Theme | string | "Light" | 默认主题（Light、Dark、System） |
| UI:Culture | string | "en-US" | 默认文化（如 zh-CN、en-US） |
| UI:PerformanceMonitoring | bool | true | 是否启用性能监控 |
| UI:AccessibilityChecks | bool | true | 是否启用无障碍检查 |

### 3.2 Web 配置

| 配置项 | 类型 | 默认值 | 描述 |
|-------|------|-------|------|
| Web:Port | int | 8080 | Web 服务器端口 |
| Web:Host | string | "localhost" | Web 服务器主机 |
| Web:UseHttps | bool | true | 是否使用 HTTPS |

### 3.3 日志配置

| 配置项 | 类型 | 默认值 | 描述 |
|-------|------|-------|------|
| Logging:LogLevel:Default | string | "Information" | 默认日志级别 |
| Logging:LogLevel:Microsoft | string | "Warning" | Microsoft 组件日志级别 |
| Logging:LogLevel:System | string | "Warning" | 系统组件日志级别 |

### 3.4 运行时配置

| 配置项 | 类型 | 默认值 | 描述 |
|-------|------|-------|------|
| DOTNET_ENVIRONMENT | string | "Development" | .NET 环境 |
| DOTNET_CLI_TELEMETRY_OPTOUT | string | "1" | 是否选择退出遥测 |
| DOTNET_SYSTEM_GLOBALIZATION_INVARIANT | string | "false" | 是否启用全球化不变模式 |

## 4. 命令行参数

### 4.1 核心参数

| 参数 | 类型 | 默认值 | 描述 |
|-----|------|-------|------|
| --ui-type | string | "web" | UI 类型（web、desktop、mobile） |
| --theme | string | "light" | 主题（light、dark、system） |
| --culture | string | "en-US" | 文化（如 en-US、zh-CN） |
| --log-level | string | "information" | 日志级别（debug、information、warning、error） |

### 4.2 使用示例

```bash
# 运行 Web UI，使用深色主题，中文语言
dotnet run --project scripts/ui_core.cs --ui-type web --theme dark --culture zh-CN

# 运行桌面 UI，使用系统主题，英文语言
dotnet run --project scripts/ui_core.cs --ui-type desktop --theme system --culture en-US

# 运行移动 UI，使用浅色主题，启用调试日志
dotnet run --project scripts/ui_core.cs --ui-type mobile --theme light --log-level debug
```

## 5. 环境变量

### 5.1 核心环境变量

| 环境变量 | 类型 | 默认值 | 描述 |
|---------|------|-------|------|
| DOTNET_ENVIRONMENT | string | "Development" | .NET 环境 |
| UI_TYPE | string | "Web" | UI 类型 |
| UI_THEME | string | "Light" | 默认主题 |
| UI_CULTURE | string | "en-US" | 默认文化 |
| UI_PERFORMANCE_MONITORING | string | "true" | 是否启用性能监控 |
| UI_ACCESSIBILITY_CHECKS | string | "true" | 是否启用无障碍检查 |
| LOG_LEVEL | string | "Information" | 日志级别 |

### 5.2 Web 环境变量

| 环境变量 | 类型 | 默认值 | 描述 |
|---------|------|-------|------|
| WEB_PORT | string | "8080" | Web 服务器端口 |
| WEB_HOST | string | "localhost" | Web 服务器主机 |
| WEB_USE_HTTPS | string | "true" | 是否使用 HTTPS |

## 6. 部署选项

### 6.1 本地开发

```bash
# 构建项目
dotnet build -c Release

# 运行项目
dotnet run --project scripts/ui_core.cs

# 发布项目（AOT 编译）
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
```

### 6.2 Docker 部署

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

### 6.3 Kubernetes 部署

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
        - name: UI_TYPE
          value: "Web"
        - name: WEB_HOST
          value: "0.0.0.0"
        - name: WEB_PORT
          value: "8080"
        - name: DOTNET_ENVIRONMENT
          value: "Production"
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

## 7. Scrutor 使用指南

### 7.1 基本用法

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

### 7.2 高级用法

#### 7.2.1 多个装饰器

```csharp
services.AddTransient<IMyService, MyService>();
services.Decorate<IMyService, LoggingDecorator>();
services.Decorate<IMyService, CachingDecorator>();
services.Decorate<IMyService, ValidationDecorator>();
```

#### 7.2.2 条件注册

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithLifetimeFromAttributes());
```

#### 7.2.3 生命周期管理

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

### 7.3 在 UI 技能中的应用

UI 技能广泛使用 Scrutor 来实现：

- **主题管理装饰器**：在主题变更时添加日志和性能监控
- **本地化装饰器**：缓存本地化字符串，提高性能
- **性能监控装饰器**：跟踪 UI 操作的执行时间
- **无障碍检查装饰器**：在 UI 渲染时执行无障碍检查

## 8. 性能优化

### 8.1 AOT 编译

UI 技能使用 AOT 编译技术来提高应用程序的启动性能和运行时性能。AOT 编译将 IL 代码编译为本地机器代码，减少了运行时的 JIT 编译开销。

**配置**：

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial /p:PublishSingleFile=true
```

### 8.2 内存优化

- **服务器 GC**：启用服务器 GC 以提高性能
- **对象池**：使用对象池减少内存分配和垃圾回收
- **内存压缩**：启用内存压缩以减少内存碎片
- **延迟加载**：仅在需要时加载 UI 组件

### 8.3 渲染优化

- **虚拟滚动**：对大型列表和网格使用虚拟滚动
- **批处理更新**：批量处理 UI 更新，减少渲染次数
- **缓存**：缓存频繁使用的数据和计算结果
- **异步渲染**：使用异步渲染避免 UI 阻塞

## 9. 无障碍支持

### 9.1 WCAG 标准

UI 技能遵循 WCAG 2.1 标准，确保应用程序对所有人都可访问，包括使用辅助技术的用户。

### 9.2 无障碍检查

UI 技能内置了无障碍检查工具，可以在开发过程中检测和修复无障碍问题。

```csharp
var accessibilityChecker = serviceProvider.GetRequiredService<IAccessibilityChecker>();
var report = await accessibilityChecker.CheckAsync();
if (!report.IsCompliant)
{
    // 处理无障碍问题
}
```

### 9.3 最佳实践

- **语义化 HTML**：使用语义化 HTML 元素
- **键盘导航**：确保所有功能都可以通过键盘访问
- **屏幕阅读器支持**：为所有 UI 元素提供适当的 ARIA 属性
- **颜色对比度**：确保文本和背景之间有足够的对比度
- **焦点管理**：合理管理键盘焦点

## 10. 国际化和本地化

### 10.1 支持的语言

UI 技能支持多语言界面，默认包含以下语言：

- 英语（en-US）
- 中文（zh-CN）

### 10.2 使用本地化

```csharp
var localizationManager = serviceProvider.GetRequiredService<ILocalizationManager>();

// 设置语言
await localizationManager.SetCultureAsync("zh-CN");

// 获取本地化字符串
var welcomeText = localizationManager.GetString("Welcome");
```

### 10.3 添加新语言

要添加新语言，需要：

1. 在 `LocalizationManager` 中添加新的语言资源
2. 实现相应的本地化字符串
3. 确保所有用户可见的文本都使用本地化

## 11. 主题管理

### 11.1 支持的主题

UI 技能支持以下主题：

- 浅色主题（Light）
- 深色主题（Dark）
- 系统主题（System，跟随操作系统设置）

### 11.2 使用主题管理

```csharp
var themeManager = serviceProvider.GetRequiredService<IThemeManager>();

// 设置主题
await themeManager.SetThemeAsync(Theme.Dark);

// 切换主题
await themeManager.ToggleThemeAsync();

// 监听主题变更
themeManager.ThemeChanged += (sender, args) =>
{
    Console.WriteLine($"Theme changed from {args.OldTheme} to {args.NewTheme}");
};
```

### 11.3 自定义主题

要创建自定义主题，需要：

1. 扩展 `Theme` 枚举
2. 在 `ThemeManager` 中添加新主题的处理逻辑
3. 为新主题提供相应的样式和资源

## 12. 故障排除

### 12.1 常见问题

| 问题 | 可能原因 | 解决方案 |
|-----|---------|--------|
| Web UI 无法启动 | 端口被占用 | 更改 Web:Port 配置 |
| 主题不生效 | 缓存问题 | 清除浏览器缓存或应用缓存 |
| 本地化失败 | 缺少资源文件 | 确保添加了相应语言的资源文件 |
| 性能下降 | 内存泄漏 | 检查是否正确释放资源，使用性能监控工具 |
| 无障碍检查失败 | 不符合 WCAG 标准 | 检查 UI 组件是否符合无障碍标准 |

### 12.2 日志和诊断

UI 技能使用结构化日志，可配置不同的日志级别：

```bash
# 启用详细日志
dotnet run --project scripts/ui_core.cs --log-level debug
```

### 12.3 调试技巧

- **使用浏览器开发工具**：调试 Web UI 时使用浏览器的开发工具
- **MAUI 调试器**：调试桌面和移动应用时使用 MAUI 调试器
- **性能分析器**：使用 .NET 性能分析器识别瓶颈
- **日志分析**：分析应用日志以识别问题

## 13. 示例项目

UI 技能包含以下示例项目：

- **BasicWebApp**：基本的 Web 应用示例
- **DesktopApp**：桌面应用示例
- **MobileApp**：移动应用示例
- **ThemeDemo**：主题管理示例
- **LocalizationDemo**：本地化示例
- **PerformanceDemo**：性能监控示例
- **AccessibilityDemo**：无障碍支持示例

这些示例的详细代码和说明可以在 `examples.md` 文件中找到。

## 14. 总结

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