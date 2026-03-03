# appinsights Agent Skill - App Insights 监控技能

## 技能概述

基于 .NET 10 的高性能 Application Insights 监控技能，为 .NET 开发者提供全面的应用性能监控、遥测数据收集和分析功能。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.ApplicationInsights.AspNetCore@2.22.0
#:package Microsoft.ApplicationInsights.WorkerService@2.22.0
```

### 注册服务

在您的主应用程序中注册 App Insights 服务：

```csharp
// 注册 App Insights 服务
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Services.AddSingleton<AppInsightsService>();
builder.Services.AddSingleton<IAppInsightsProvider, AppInsightsProvider>();
builder.Services.AddSingleton<AppInsightsAlertingService>();
```

### 使用示例

```csharp
// 获取 App Insights 服务
var appInsightsService = serviceProvider.GetRequiredService<AppInsightsService>();

// 跟踪自定义事件
await appInsightsService.TrackCustomEventAsync("UserLogin", new Dictionary<string, string>
{
    { "UserName", "user123" },
    { "Location", "China" }
});

// 跟踪依赖关系
await appInsightsService.TrackDependencyAsync("SQL", "GetUserData", "SELECT * FROM Users", true, DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(123));

// 记录自定义指标
await appInsightsService.TrackMetricAsync("RequestCount", 100, new Dictionary<string, string>
{
    { "Endpoint", "/api/users" }
});

// 获取使用统计信息
var stats = await appInsightsService.GetUsageStatisticsAsync();
Console.WriteLine($"活跃用户: {stats.ActiveUsers}");
```

## 导航地图

```
appinsights/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── appinsights_alerting.cs      # App Insights 告警实现
    ├── appinsights_alerting.run.json  # 告警服务运行配置
    ├── appinsights_alerting.setting.json  # 告警服务设置
    ├── appinsights_integration.cs      # 核心 App Insights 集成
    ├── appinsights_integration.run.json  # 集成服务运行配置
    ├── appinsights_integration.setting.json  # 集成服务设置
    ├── appinsights_logging.cs      # App Insights 日志实现
    ├── appinsights_logging.run.json  # 日志服务运行配置
    ├── appinsights_logging.setting.json  # 日志服务设置
    ├── appinsights_prometheus.cs      # Prometheus 集成
    ├── appinsights_prometheus.run.json  # Prometheus 集成运行配置
    ├── appinsights_prometheus.setting.json  # Prometheus 集成设置
    ├── appmetrics_integration.cs      # 应用指标集成
    ├── appmetrics_integration.run.json  # 指标集成运行配置
    └── appmetrics_integration.setting.json  # 指标集成设置
```

## 主要功能

1. **遥测数据收集**：全面的应用遥测数据收集，包括请求、依赖关系、异常、跟踪和自定义事件
2. **性能监控**：实时性能监控和分析
3. **告警系统**：可配置的性能阈值和错误率告警
4. **Prometheus 集成**：与 Prometheus 监控系统无缝集成
5. **应用指标**：自定义应用指标收集和报告
6. **使用分析**：用户行为和应用使用情况分析
7. **高性能设计**：针对低开销遥测数据收集进行优化
8. **可扩展架构**：支持自定义遥测处理器和导出器

## 扩展说明

此技能提供完整的 Application Insights 解决方案，您可以根据需要进行扩展：

1. **自定义遥测处理器**：实现自定义遥测处理器以过滤或增强遥测数据
2. **自定义导出器**：添加对其他遥测导出器的支持
3. **高级告警**：实现复杂的告警规则和通知渠道
4. **性能优化**：为特定场景自定义遥测采样和过滤
5. **与其他系统集成**：与外部监控和分析平台集成

## 最佳实践

1. **依赖注入**：使用依赖注入来管理 App Insights 服务
2. **异步编程**：优先使用异步 API 进行遥测操作，避免阻塞
3. **遥测采样**：配置适当的遥测采样以管理数据量
4. **错误处理**：在遥测处理中正确处理异常情况
5. **结构化日志**：使用结构化日志以获得更好的遥测关联
6. **性能监控**：监控遥测数据收集对性能的影响
7. **自定义属性**：向遥测数据添加有意义的自定义属性以获得更好的分析
8. **告警调整**：定期审查和调整告警阈值以减少误报

## 配置选项

### AppInsightsSettings 配置

```json
{
  "AppInsightsSettings": {
    "InstrumentationKey": "your-instrumentation-key",
    "ConnectionString": "your-connection-string",
    "EnableTelemetry": true,
    "SamplingPercentage": 100,
    "EnableAdaptiveSampling": true,
    "EnableLiveMetrics": true,
    "EnableRequestTracking": true,
    "EnableDependencyTracking": true,
    "EnableExceptionTracking": true,
    "EnablePerformanceCounters": true
  }
}
```

### 告警配置

```json
{
  "AlertingSettings": {
    "EnableAlerts": true,
    "ErrorRateThreshold": 0.05,
    "ResponseTimeThresholdMs": 1000,
    "RequestVolumeThreshold": 1000,
    "AlertNotificationChannels": ["Email", "Slack", "Webhook"],
    "EmailRecipients": ["admin@example.com"],
    "SlackWebhookUrl": "https://hooks.slack.com/services/your-slack-webhook"
  }
}
```

## 性能考虑

1. **遥测采样**：实现适当的采样以减少遥测数据量
2. **批处理**：对高频遥测数据使用批处理
3. **异步处理**：异步处理遥测数据以避免阻塞主应用程序流
4. **内存管理**：确保遥测数据处理的高效内存使用
5. **网络使用**：优化遥测数据传输的网络使用

## 故障排除

### 常见问题

1. **无遥测数据**：检查您的仪表键和连接字符串
2. **高内存使用**：调整遥测采样和批处理设置
3. **高 CPU 使用**：优化自定义遥测处理器
4. **告警疲劳**：调整告警阈值并实现智能告警
5. **应用程序缓慢**：检查遥测数据处理是否阻塞主应用程序

### 日志记录和调试

```csharp
// 启用调试日志
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Debug);
builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);

// 检查 App Insights 状态
var status = await appInsightsService.GetStatusAsync();
Console.WriteLine($"App Insights 状态: {status.Status}");
Console.WriteLine($"最后发送遥测数据: {status.LastTelemetrySent}");
Console.WriteLine($"错误计数: {status.ErrorCount}");
```