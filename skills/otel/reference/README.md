# OpenTelemetry 技能参考文档

## 1. 技能概述

OpenTelemetry 技能是一个基于 .NET 10 的高性能可观测性解决方案，提供完整的追踪、指标、日志和告警功能，帮助开发者构建更加可靠、可维护的分布式系统。

本技能旨在简化 OpenTelemetry 的使用，提供一系列工具和功能，帮助开发者更高效地实现系统的可观测性。

## 2. 核心组件

### 2.1 IOpenTelemetryService

`IOpenTelemetryService` 是 OpenTelemetry 技能的核心服务接口，提供了 OpenTelemetry 的初始化、关闭和配置管理功能。

**主要方法**：

- `InitializeAsync(CancellationToken cancellationToken = default)` - 初始化 OpenTelemetry 服务
- `ShutdownAsync(CancellationToken cancellationToken = default)` - 关闭 OpenTelemetry 服务
- `GetConfigurationAsync(CancellationToken cancellationToken = default)` - 获取 OpenTelemetry 配置
- `UpdateConfigurationAsync(OpenTelemetryConfiguration configuration, CancellationToken cancellationToken = default)` - 更新 OpenTelemetry 配置

### 2.2 IOpenTelemetryTracingService

`IOpenTelemetryTracingService` 负责 OpenTelemetry 的追踪功能，包括活动的创建、标签的添加、事件的记录和异常的记录。

### 2.3 IOpenTelemetryMetricsService

`IOpenTelemetryMetricsService` 负责 OpenTelemetry 的指标功能，包括计数器、仪表盘、直方图等指标的记录。

### 2.4 IOpenTelemetryLoggingService

`IOpenTelemetryLoggingService` 负责 OpenTelemetry 的日志功能，包括不同级别的日志记录。

### 2.5 IOpenTelemetryAlertingService

`IOpenTelemetryAlertingService` 负责 OpenTelemetry 的告警功能，包括告警规则的创建、更新、删除和告警的管理。

## 3. 配置选项

### 3.1 OpenTelemetryOptions

`OpenTelemetryOptions` 是 OpenTelemetry 技能的主要配置选项类，包含以下属性：

| 属性名 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| Enabled | bool | true | 是否启用 OpenTelemetry 技能 |
| ServiceName | string | "OpenTelemetry-Service" | 服务名称 |
| ServiceVersion | string | "1.0.0" | 服务版本 |
| Environment | string | "Development" | 环境名称 |
| EnableTracing | bool | true | 是否启用追踪 |
| EnableMetrics | bool | true | 是否启用指标 |
| EnableLogging | bool | true | 是否启用日志 |
| EnableAlerting | bool | true | 是否启用告警 |
| EnablePrometheusExporter | bool | true | 是否启用 Prometheus 导出器 |
| EnableJaegerExporter | bool | true | 是否启用 Jaeger 导出器 |
| EnableZipkinExporter | bool | false | 是否启用 Zipkin 导出器 |
| PrometheusEndpoint | string | "/metrics" | Prometheus 端点 |
| JaegerEndpoint | string | "http://localhost:14268/api/traces" | Jaeger 端点 |
| ZipkinEndpoint | string | "http://localhost:9411/api/v2/spans" | Zipkin 端点 |
| EnableParallelProcessing | bool | true | 是否启用并行处理 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |

### 3.2 配置文件结构

OpenTelemetry 技能的配置文件包含以下主要部分：

- `opentelemetry.options` - 基本配置选项
- `opentelemetry.tracing` - 追踪配置
- `opentelemetry.metrics` - 指标配置
- `opentelemetry.logging` - 日志配置
- `opentelemetry.alerting` - 告警配置
- `opentelemetry.aot` - AOT 编译配置
- `opentelemetry.performance` - 性能配置
- `opentelemetry.security` - 安全配置

## 4. 安装和设置

### 4.1 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- OpenTelemetry 1.8.0 或更高版本

### 4.2 安装方法

1. **克隆技能仓库**：
   ```bash
   git clone <repository-url>
   cd skills/otel
   ```

2. **安装依赖**：
   ```bash
   dotnet restore
   ```

3. **配置技能**：
   编辑 `scripts/otel_extensions.setting.json` 文件，根据需要修改配置。

4. **运行技能**：
   ```bash
   dotnet run --project scripts/otel_extensions.cs
   ```

### 4.3 依赖注入

在 .NET 应用中，你可以使用依赖注入来注册和使用 OpenTelemetry 服务：

```csharp
// 注册 OpenTelemetry 服务
services.AddOpenTelemetryServices(options =>
{
    options.Enabled = true;
    options.ServiceName = "MyService";
    options.ServiceVersion = "1.0.0";
    options.Environment = "Production";
    options.EnableTracing = true;
    options.EnableMetrics = true;
    options.EnableLogging = true;
    options.EnableAlerting = true;
    options.EnablePrometheusExporter = true;
    options.EnableJaegerExporter = true;
    options.EnableZipkinExporter = false;
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = Environment.ProcessorCount;
});

// 使用 OpenTelemetry 服务
var otelService = serviceProvider.GetRequiredService<IOpenTelemetryService>();
```

## 5. 使用指南

### 5.1 初始化 OpenTelemetry

```csharp
var otelService = serviceProvider.GetRequiredService<IOpenTelemetryService>();
await otelService.InitializeAsync();
Console.WriteLine("OpenTelemetry 初始化成功");
```

### 5.2 分布式追踪

**创建追踪**：
```csharp
var tracingService = serviceProvider.GetRequiredService<IOpenTelemetryTracingService>();
using var activity = tracingService.StartActivity("UserOperation");
try
{
    // 执行业务操作
    await DoBusinessOperationAsync();
    tracingService.AddActivityTag(activity!, "result", "success");
    tracingService.AddActivityEvent(activity!, "OperationCompleted");
}
catch (Exception ex)
{
    tracingService.RecordException(activity!, ex);
    tracingService.AddActivityTag(activity!, "result", "error");
    throw;
}
```

**使用异步追踪**：
```csharp
var tracingService = serviceProvider.GetRequiredService<IOpenTelemetryTracingService>();
var result = await tracingService.TraceAsync("CalculateSum", async () =>
{
    await Task.Delay(100); // 模拟计算过程
    return 1 + 2;
}, new Dictionary<string, object> { { "operation", "addition" } });
Console.WriteLine($"计算结果: {result}");
```

### 5.3 指标监控

**记录计数器指标**：
```csharp
var metricsService = serviceProvider.GetRequiredService<IOpenTelemetryMetricsService>();
metricsService.IncrementCounter("user.operations.count", tags: new Dictionary<string, object> { { "operation", "login" } });
Console.WriteLine("计数器指标记录成功");
```

**记录仪表盘指标**：
```csharp
metricsService.RecordGauge("system.memory.usage", value: 1024 * 1024 * 1024, tags: new Dictionary<string, object> { { "type", "used" } });
Console.WriteLine("仪表盘指标记录成功");
```

**记录直方图指标**：
```csharp
metricsService.RecordHistogram("request.duration", value: 150, tags: new Dictionary<string, object> { { "endpoint", "/api/users" } });
Console.WriteLine("直方图指标记录成功");
```

### 5.4 日志集成

**记录不同级别的日志**：
```csharp
var loggingService = serviceProvider.GetRequiredService<IOpenTelemetryLoggingService>();
loggingService.LogInformation("User logged in successfully", new Dictionary<string, object> { { "userId", 1 }, { "username", "user1" } });
loggingService.LogWarning("API rate limit exceeded", new Dictionary<string, object> { { "clientIp", "192.168.1.1" } });
try
{
    // 执行业务操作
}
catch (Exception ex)
{
    loggingService.LogError("Failed to process request", ex, new Dictionary<string, object> { { "requestId", "req-123" } });
}
```

### 5.5 告警系统

**创建告警规则**：
```csharp
var alertingService = serviceProvider.GetRequiredService<IOpenTelemetryAlertingService>();
var alertRule = new AlertRule
{
    Name = "HighCPUUsage",
    Description = "CPU usage exceeds threshold",
    Condition = "system.cpu.usage > 80",
    Severity = "High",
    Threshold = 80,
    EvaluationPeriod = TimeSpan.FromMinutes(1),
    NotificationChannels = new List<string> { "email", "slack" }
};
await alertingService.CreateAlertRuleAsync(alertRule);
Console.WriteLine("告警规则创建成功");
```

**获取告警规则**：
```csharp
var alertRules = await alertingService.GetAlertRulesAsync();
Console.WriteLine($"告警规则数量: {alertRules.Count()}");
foreach (var rule in alertRules)
{
    Console.WriteLine($"规则名称: {rule.Name}, 严重性: {rule.Severity}, 阈值: {rule.Threshold}
