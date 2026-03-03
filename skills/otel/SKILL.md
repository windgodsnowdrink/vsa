# OpenTelemetry 技能

## 技能概述

OpenTelemetry 技能是一个基于 .NET 10 的高性能可观测性解决方案，提供完整的追踪、指标、日志和告警功能，帮助开发者构建更加可靠、可维护的分布式系统。

本技能旨在简化 OpenTelemetry 的使用，提供一系列工具和功能，帮助开发者更高效地实现系统的可观测性。

## 主要功能

- **OpenTelemetry 集成**：与 OpenTelemetry 框架的无缝集成，支持最新版本的 OpenTelemetry
- **分布式追踪**：支持分布式系统的追踪功能，实现请求的全链路追踪
- **指标监控**：提供系统和应用的指标监控，支持多种指标类型
- **日志集成**：集成日志系统，提供完整的可观测性
- **告警系统**：基于指标和日志的告警功能，及时发现和处理问题
- **数据可视化**：与 Grafana 等可视化工具集成，提供直观的数据展示
- **性能分析**：提供系统性能分析功能，帮助优化系统性能
- **AOT 编译**：支持 .NET 10 AOT 编译，提升运行性能

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- OpenTelemetry 1.8.0 或更高版本

### 安装方法

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

## 使用指南

### 分布式追踪

OpenTelemetry 技能提供了完整的分布式追踪功能，可以帮助你实现请求的全链路追踪。

```csharp
// 获取追踪服务
var tracingService = serviceProvider.GetRequiredService<IOpenTelemetryTracingService>();

// 创建追踪
using var activity = tracingService.StartActivity("UserOperation");
try
{
    // 执行业务操作
    await DoBusinessOperationAsync();
    activity.SetStatus(ActivityStatusCode.Ok);
}
catch (Exception ex)
{
    activity.SetStatus(ActivityStatusCode.Error);
    activity.RecordException(ex);
    throw;
}
```

### 指标监控

管理系统和应用的指标，实现实时监控。

```csharp
// 获取指标服务
var metricsService = serviceProvider.GetRequiredService<IOpenTelemetryMetricsService>();

// 记录计数器指标
metricsService.IncrementCounter("user.operations.count", tags: new Dictionary<string, object> { { "operation", "login" } });

// 记录仪表盘指标
metricsService.RecordGauge("system.memory.usage", value: 1024, tags: new Dictionary<string, object> { { "type", "used" } });

// 记录直方图指标
metricsService.RecordHistogram("request.duration", value: 150, tags: new Dictionary<string, object> { { "endpoint", "/api/users" } });
```

### 日志集成

集成日志系统，提供完整的可观测性。

```csharp
// 获取日志服务
var loggingService = serviceProvider.GetRequiredService<IOpenTelemetryLoggingService>();

// 记录日志
loggingService.LogInformation("User logged in successfully", new Dictionary<string, object> { { "userId", 1 }, { "username", "user1" } });
loggingService.LogError("Failed to process request", ex, new Dictionary<string, object> { { "requestId", "req-123" } });
```

### 告警系统

基于指标和日志的告警功能，及时发现和处理问题。

```csharp
// 获取告警服务
var alertingService = serviceProvider.GetRequiredService<IOpenTelemetryAlertingService>();

// 创建告警规则
alertingService.CreateAlertRule(
    name: "HighCPUUsage",
    description: "CPU usage exceeds threshold",
    condition: "system.cpu.usage > 80",
    severity: "High",
    threshold: 80,
    evaluationPeriod: TimeSpan.FromMinutes(1),
    notificationChannels: new List<string> { "email", "slack" }
);
```

## AOT 编译支持

OpenTelemetry 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 编译选项

- `PublishAot=true`：启用 AOT 编译
- `ReadyToRun=true`：启用 ReadyToRun 编译
- `TieredCompilation=true`：启用分层编译

### 支持的运行时

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

## 性能特性

- **快速启动**：AOT 编译减少了应用启动时间
- **内存占用低**：优化的内存使用
- **响应速度快**：提升了请求处理速度
- **部署简便**：单文件执行模式，部署更简单
- **低延迟**：优化的追踪和指标收集，减少系统开销

## 参考文档

- **README.md**：详细参考文档，包含核心组件和配置说明
- **examples.md**：使用示例文档，包含各种使用场景和代码示例

## 许可证

本技能采用 MIT 许可证，详情请参阅 LICENSE 文件。

## 联系方式

- **官方网站**：https://vsa-arch.com
- **GitHub**：https://github.com/vsa-arch/otel-skill
- **文档**：https://docs.vsa-arch.com/otel-skill

---

**版本**：1.0.0
**发布日期**：2026-01-24
**作者**：VSA Architecture Team
