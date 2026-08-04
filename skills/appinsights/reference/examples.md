# appinsights - 使用示例

## 快速入门

### 1. 基本使用示例

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var appInsightsService = serviceProvider.GetRequiredService<AppInsightsService>();
        
        Console.WriteLine("App Insights 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 App Insights 功能
        await appInsightsService.TrackCustomEventAsync("UserLogin", new Dictionary<string, string>
        {
            { "UserName", "user123" },
            { "Location", "China" }
        });
        
        Console.WriteLine("自定义事件已发送");
        
        // 跟踪依赖关系
        await appInsightsService.TrackDependencyAsync(
            "SQL", 
            "GetUserData", 
            "SELECT * FROM Users WHERE Id = @Id", 
            true, 
            DateTimeOffset.UtcNow, 
            TimeSpan.FromMilliseconds(123)
        );
        
        Console.WriteLine("依赖关系已跟踪");
        
        // 记录自定义指标
        await appInsightsService.TrackMetricAsync(
            "RequestCount", 
            100, 
            new Dictionary<string, string>
            {
                { "Endpoint", "/api/users" }
            }
        );
        
        Console.WriteLine("自定义指标已记录");
        
        // 获取使用统计信息
        var stats = await appInsightsService.GetUsageStatisticsAsync();
        Console.WriteLine($"活跃用户: {stats.ActiveUsers}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 App Insights
        builder.AddApplicationInsightsTelemetry(options =>
        {
            options.InstrumentationKey = "your-instrumentation-key";
            options.EnableLiveMetrics = true;
        });
        
        // 注册服务
        builder.AddSingleton<AppInsightsService>();
        builder.AddSingleton<IAppInsightsProvider, AppInsightsProvider>();
        builder.AddSingleton<AppInsightsAlertingService>();
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("App Insights 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 App Insights 设置
        builder.Configure<AppInsightsSettings>(options => {
            options.InstrumentationKey = "your-instrumentation-key";
            options.ConnectionString = "your-connection-string";
            options.EnableTelemetry = true;
            options.SamplingPercentage = 100;
            options.EnableAdaptiveSampling = true;
            options.EnableLiveMetrics = true;
            options.EnableRequestTracking = true;
            options.EnableDependencyTracking = true;
            options.EnableExceptionTracking = true;
            options.EnablePerformanceCounters = true;
        });
        
        // 配置告警设置
        builder.Configure<AlertingSettings>(options => {
            options.EnableAlerts = true;
            options.ErrorRateThreshold = 0.05;
            options.ResponseTimeThresholdMs = 1000;
            options.RequestVolumeThreshold = 1000;
            options.AlertNotificationChannels = new[] { "Email", "Slack", "Webhook" };
            options.EmailRecipients = new[] { "admin@example.com" };
            options.SlackWebhookUrl = "https://hooks.slack.com/services/your-slack-webhook";
        });
        
        // 注册服务
        builder.AddSingleton<IAppInsightsProvider, AppInsightsProvider>();
        builder.AddSingleton<AppInsightsService>();
        builder.AddSingleton<AppInsightsAlertingService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var appInsightsSettings = serviceProvider.GetRequiredService<IOptions<AppInsightsSettings>>().Value;
        var alertingSettings = serviceProvider.GetRequiredService<IOptions<AlertingSettings>>().Value;
        
        Console.WriteLine($"App Insights 配置: InstrumentationKey={appInsightsSettings.InstrumentationKey}");
        Console.WriteLine($"告警配置: EnableAlerts={alertingSettings.EnableAlerts}, ErrorRateThreshold={alertingSettings.ErrorRateThreshold}");
        
        // 使用服务
        var appInsightsService = serviceProvider.GetRequiredService<AppInsightsService>();
        await appInsightsService.TrackCustomEventAsync("AdvancedConfigurationExample", new Dictionary<string, string>
        {
            { "Environment", "Production" },
            { "Version", "1.0.0" }
        });
        
        Console.WriteLine("高级配置示例执行完成");
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("App Insights 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var appInsightsService = serviceProvider.GetRequiredService<AppInsightsService>();
        
        // 性能测试
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            await appInsightsService.TrackMetricAsync(
                "PerformanceTest", 
                i % 100, 
                new Dictionary<string, string>
                {
                    { "Iteration", i.ToString() }
                }
            );
        }
        
        stopwatch.Stop();
        Console.WriteLine($"{iterations} 次迭代执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次迭代: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 验证遥测数据已发送
        var status = await appInsightsService.GetStatusAsync();
        Console.WriteLine($"App Insights 状态: {status.Status}");
        Console.WriteLine($"最后发送遥测数据: {status.LastTelemetrySent}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 App Insights 以优化性能
        builder.AddApplicationInsightsTelemetry(options =>
        {
            options.InstrumentationKey = "your-instrumentation-key";
            options.EnableAdaptiveSampling = true;
            options.SamplingPercentage = 50; // 降低采样率以减少数据量
            options.EnablePerformanceCounterCollectionModule = false; // 禁用性能计数器以减少开销
        });
        
        builder.AddSingleton<AppInsightsService>();
        builder.AddSingleton<IAppInsightsProvider, AppInsightsProvider>();
        
        return builder.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("App Insights 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var appInsightsService = serviceProvider.GetRequiredService<AppInsightsService>();
        
        try
        {
            // 尝试发送遥测数据
            await appInsightsService.TrackCustomEventAsync("TestEvent", new Dictionary<string, string>
            {
                { "TestKey", "TestValue" }
            });
            
            Console.WriteLine("遥测数据发送成功");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
            // 记录异常到 App Insights
            await appInsightsService.TrackExceptionAsync(ex);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
            await appInsightsService.TrackExceptionAsync(ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"一般错误: {ex.Message}");
            await appInsightsService.TrackExceptionAsync(ex);
        }
        
        // 启用调试日志
        var telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();
        telemetryClient.TrackTrace("错误处理示例执行完成", SeverityLevel.Information);
        
        Console.WriteLine("错误处理示例执行完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 App Insights
        builder.AddApplicationInsightsTelemetry(options =>
        {
            options.InstrumentationKey = "your-instrumentation-key";
        });
        
        builder.AddSingleton<AppInsightsService>();
        builder.AddSingleton<IAppInsightsProvider, AppInsightsProvider>();
        
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例演示了 App Insights 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
