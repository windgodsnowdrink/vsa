# appinsights - 参考文档

## 概述

appinsights是一个基于.NET 10的高性能Application Insights监控系统，专为.NET开发者设计。

## 核心组件

### 1. App Insights 集成服务 (appinsights_integration.cs)
- **位置**: scripts/appinsights_integration.cs
- **功能**: 核心App Insights集成逻辑
- **特性**: 
  - 应用性能监控
  - 遥测数据收集
  - 实时性能分析
  - 依赖关系跟踪

### 2. App Insights 告警服务 (appinsights_alerting.cs)
- **位置**: scripts/appinsights_alerting.cs
- **功能**: 告警规则处理和通知
- **特性**: 
  - 可配置的告警规则
  - 多渠道通知（Email、Slack、Webhook）
  - 错误率和性能阈值监控
  - 告警静默和恢复机制

### 3. App Insights 日志服务 (appinsights_logging.cs)
- **位置**: scripts/appinsights_logging.cs
- **功能**: 日志集成和管理
- **特性**: 
  - 结构化日志记录
  - 日志级别控制
  - 日志关联和追踪
  - 日志导出和归档

### 4. App Insights Prometheus 集成 (appinsights_prometheus.cs)
- **位置**: scripts/appinsights_prometheus.cs
- **功能**: 与Prometheus监控系统集成
- **特性**: 
  - Prometheus指标导出
  - 指标采集和转换
  - 指标聚合和统计
  - 多维度指标支持

### 5. 应用指标集成 (appmetrics_integration.cs)
- **位置**: scripts/appmetrics_integration.cs
- **功能**: 应用自定义指标收集
- **特性**: 
  - 自定义指标定义
  - 指标采集和聚合
  - 指标可视化支持
  - 指标告警配置

## 使用示例

### 基本用法

```csharp
var appInsightsService = serviceProvider.GetRequiredService<AppInsightsService>();
var result = await appInsightsService.TrackCustomEventAsync("UserLogin", new Dictionary<string, string>
{
    { "UserName", "user123" },
    { "Location", "China" }
});
```

### 高级配置

```csharp
var settings = new AppInsightsSettings {
    InstrumentationKey = "your-instrumentation-key",
    SamplingPercentage = 100,
    EnableLiveMetrics = true
};

builder.Services.Configure<AppInsightsSettings>(options => {
    options.InstrumentationKey = settings.InstrumentationKey;
    options.SamplingPercentage = settings.SamplingPercentage;
    options.EnableLiveMetrics = settings.EnableLiveMetrics;
});
```

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

### AlertingSettings 配置

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

## 性能优化

1. **遥测采样**: 配置适当的采样率以减少数据量
2. **异步编程**: 使用异步API避免阻塞主线程
3. **批处理**: 对高频遥测数据使用批处理
4. **内存管理**: 优化遥测数据处理的内存使用
5. **网络优化**: 优化遥测数据传输的网络使用

## 故障排除

### 常见问题

1. **无遥测数据**
   - 检查仪表键和连接字符串
   - 验证网络连接
   - 检查日志信息

2. **高内存使用**
   - 调整遥测采样率
   - 优化批处理设置
   - 检查自定义遥测处理器

3. **高CPU使用**
   - 优化自定义遥测处理器
   - 减少遥测数据量
   - 调整采样率

4. **告警疲劳**
   - 调整告警阈值
   - 实现智能告警
   - 配置告警静默规则

5. **应用程序缓慢**
   - 检查遥测数据处理是否阻塞主应用程序
   - 优化自定义遥测处理器
   - 调整批处理设置

## 扩展开发

### 添加自定义遥测处理器

```csharp
public class CustomTelemetryProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public CustomTelemetryProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        // 自定义遥测处理逻辑
        if (item is RequestTelemetry requestTelemetry)
        {
            requestTelemetry.Properties["CustomProperty"] = "CustomValue";
        }

        // 继续处理链
        _next.Process(item);
    }
}
```

### 添加自定义遥测导出器

```csharp
public class CustomTelemetryExporter : ITelemetryExporter
{
    public Task<ExportResult> ExportAsync(IEnumerable<ITelemetry> telemetryItems, CancellationToken cancellationToken)
    {
        // 自定义遥测导出逻辑
        foreach (var item in telemetryItems)
        {
            // 处理遥测项
        }

        return Task.FromResult(ExportResult.Success);
    }
}
```
