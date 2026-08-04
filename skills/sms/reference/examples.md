# SMS 技能使用示例

## 基本用法示例

### 1. 发送简单 SMS 消息

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 发送 SMS 消息
var result = await smsService.SendAsync("+1234567890", "你好！这是一条来自 SMS 技能的测试消息。");

// 输出结果
Console.WriteLine($"发送结果: {(result.Success ? "成功" : "失败")}");
if (result.Success)
{
    Console.WriteLine($"消息 ID: {result.MessageSid}");
    Console.WriteLine($"发送时间: {result.SentAt}");
}
else
{
    Console.WriteLine($"错误信息: {result.ErrorMessage}");
}
```

### 2. 使用模板发送 SMS 消息

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System.Collections.Generic;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var templateService = serviceProvider.GetRequiredService<ITemplateService>();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 创建模板
var templateId = await templateService.CreateTemplateAsync(
    "验证码",
    "您的验证码是：{{code}}，有效期为 {{minutes}} 分钟。请勿向他人透露此验证码。"
);

// 准备模板参数
var parameters = new Dictionary<string, string>
{
    { "code", "123456" },
    { "minutes", "5" }
};

// 使用模板发送 SMS
var result = await smsService.SendTemplateAsync("+1234567890", templateId, parameters);

// 输出结果
Console.WriteLine($"发送结果: {(result.Success ? "成功" : "失败")}");
```

### 3. 批量发送 SMS 消息

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System.Collections.Generic;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 准备收件人列表
var recipients = new List<string>
{
    "+1234567890",
    "+0987654321",
    "+1122334455"
};

// 批量发送 SMS
var results = await smsService.BatchSendAsync(recipients, "重要通知：系统将于明天上午 10 点进行维护升级，预计持续 2 小时。");

// 统计结果
var successCount = results.Count(r => r.Success);
var totalCount = results.Count();

Console.WriteLine($"批量发送完成: 成功 {successCount}/{totalCount}");

// 输出详细结果
foreach (var result in results)
{
    Console.WriteLine($"收件人: {result.To}, 结果: {(result.Success ? "成功" : "失败")}");
    if (!result.Success)
    {
        Console.WriteLine($"  错误: {result.ErrorMessage}");
    }
}
```

### 4. 定时发送 SMS 消息

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 设置发送时间（5分钟后）
var sendTime = DateTime.Now.AddMinutes(5);

// 定时发送 SMS
var result = await smsService.ScheduleSendAsync(
    "+1234567890",
    "提醒：您的会议将在 30 分钟后开始，请准时参加。",
    sendTime
);

// 输出结果
Console.WriteLine($"定时设置: {(result.Success ? "成功" : "失败")}");
if (result.Success)
{
    Console.WriteLine($"调度 ID: {result.ScheduleId}");
    Console.WriteLine($"发送时间: {result.SendTime}");
    Console.WriteLine($"目标号码: {result.To}");
}
else
{
    Console.WriteLine($"错误信息: {result.ErrorMessage}");
}
```

### 5. 查看发送分析

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var analyticsService = serviceProvider.GetRequiredService<IAnalyticsService>();

// 设置分析时间范围（最近7天）
var endDate = DateTime.Now;
var startDate = endDate.AddDays(-7);

// 获取分析数据
var analytics = await analyticsService.GetAnalyticsAsync(startDate, endDate);

// 输出分析结果
Console.WriteLine("SMS 发送分析报告");
Console.WriteLine("==================");
Console.WriteLine($"时间范围: {startDate:yyyy-MM-dd} 至 {endDate:yyyy-MM-dd}");
Console.WriteLine($"总发送量: {analytics.TotalSms}");
Console.WriteLine($"成功量: {analytics.SuccessfulSms}");
Console.WriteLine($"成功率: {analytics.SuccessRate:P2}");
Console.WriteLine($"平均消息长度: {analytics.AverageMessageLength:F2} 字符");

// 输出状态分布
Console.WriteLine("\n状态分布:");
foreach (var (status, count) in analytics.StatusDistribution)
{
    Console.WriteLine($"{status}: {count} ({(double)count / analytics.TotalSms:P2})");
}

// 输出每日发送量
Console.WriteLine("\n每日发送量:");
foreach (var (date, count) in analytics.DailyCounts.OrderBy(kv => kv.Key))
{
    Console.WriteLine($"{date:yyyy-MM-dd}: {count}");
}
```

## Scrutor 用法示例

### 1. 基本装饰器模式

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;

var services = new ServiceCollection();

// 注册基本服务
services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
services.AddSingleton<ITemplateService, TemplateService>();
services.AddSingleton<IAnalyticsService, AnalyticsService>();
services.AddSingleton<ISmsService, SmsService>();

// 使用 Scrutor 添加装饰器
services.Decorate<ISmsService, SmsServiceLoggingDecorator>();

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 现在 smsService 已经被 SmsServiceLoggingDecorator 装饰
// 调用方法时会自动记录日志
await smsService.SendAsync("+1234567890", "测试装饰器模式");
```

### 2. 多个装饰器

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;

var services = new ServiceCollection();

// 注册基本服务
services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
services.AddSingleton<ITemplateService, TemplateService>();
services.AddSingleton<IAnalyticsService, AnalyticsService>();
services.AddSingleton<ISmsService, SmsService>();

// 添加多个装饰器（顺序很重要）
services.Decorate<ISmsService, SmsServiceValidationDecorator>();  // 1. 验证
services.Decorate<ISmsService, SmsServiceRetryDecorator>();       // 2. 重试
services.Decorate<ISmsService, SmsServiceLoggingDecorator>();     // 3. 日志

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 调用顺序：
// 1. SmsServiceLoggingDecorator.SendAsync
// 2. SmsServiceRetryDecorator.SendAsync
// 3. SmsServiceValidationDecorator.SendAsync
// 4. SmsService.SendAsync
// 5. 反向返回结果
await smsService.SendAsync("+1234567890", "测试多个装饰器");
```

### 3. 条件装饰器

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// 注册基本服务
services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
services.AddSingleton<ITemplateService, TemplateService>();
services.AddSingleton<IAnalyticsService, AnalyticsService>();
services.AddSingleton<ISmsService, SmsService>();

// 添加条件装饰器
services.Decorate<ISmsService>((provider, decorated) =>
{
    var logger = provider.GetRequiredService<ILogger<SmsServiceLoggingDecorator>>();
    
    // 根据配置决定是否添加装饰器
    bool enableLogging = true; // 可以从配置中读取
    
    if (enableLogging)
    {
        return new SmsServiceLoggingDecorator(decorated, logger);
    }
    
    return decorated;
});

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

await smsService.SendAsync("+1234567890", "测试条件装饰器");
```

### 4. 程序集扫描

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;

var services = new ServiceCollection();

// 使用 Scrutor 程序集扫描注册服务
services.Scan(scan => scan
    // 从包含 SmsService 的程序集中扫描
    .FromAssemblyOf<SmsService>()
    // 注册所有实现了接口的类
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

// 添加装饰器
services.Decorate<ISmsService, SmsServiceLoggingDecorator>();

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

await smsService.SendAsync("+1234567890", "测试程序集扫描");
```

### 5. 高级程序集扫描配置

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;

var services = new ServiceCollection();

// 高级程序集扫描配置
services.Scan(scan => scan
    .FromAssemblyOf<SmsService>()
    // 注册 SMS 提供商
    .AddClasses(classes => classes.AssignableTo<ISmsProvider>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    // 注册模板服务
    .AddClasses(classes => classes.AssignableTo<ITemplateService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    // 注册分析服务
    .AddClasses(classes => classes.AssignableTo<IAnalyticsService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
    // 注册核心 SMS 服务
    .AddClasses(classes => classes.AssignableTo<ISmsService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

// 添加装饰器
services.Decorate<ISmsService, SmsServiceLoggingDecorator>();
services.Decorate<ISmsService, SmsServiceRetryDecorator>();

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

await smsService.SendAsync("+1234567890", "测试高级程序集扫描");
```

### 6. 装饰器链示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;

// 定义一个自定义装饰器
public class SmsServicePerformanceDecorator : ISmsService
{
    private readonly ISmsService _decorated;
    private readonly ILogger<SmsServicePerformanceDecorator> _logger;

    public SmsServicePerformanceDecorator(ISmsService decorated, ILogger<SmsServicePerformanceDecorator> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            return await _decorated.SendAsync(to, message, cancellationToken);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("SMS 发送耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
    }

    // 实现其他方法...
    public Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        return _decorated.SendTemplateAsync(to, templateId, parameters, cancellationToken);
    }

    public Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default)
    {
        return _decorated.BatchSendAsync(recipients, message, cancellationToken);
    }

    public Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default)
    {
        return _decorated.ScheduleSendAsync(to, message, sendTime, cancellationToken);
    }

    public Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return _decorated.GetAnalyticsAsync(startDate, endDate, cancellationToken);
    }

    public Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        return _decorated.ReceiveAsync(cancellationToken);
    }
}

// 使用装饰器链
var services = new ServiceCollection();
services.AddSmsServices();

// 添加多个装饰器形成装饰器链
services.Decorate<ISmsService, SmsServiceValidationDecorator>();      // 1. 输入验证
services.Decorate<ISmsService, SmsServiceRetryDecorator>();           // 2. 失败重试
services.Decorate<ISmsService, SmsServicePerformanceDecorator>();     // 3. 性能监控
services.Decorate<ISmsService, SmsServiceLoggingDecorator>();         // 4. 日志记录

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 调用会经过整个装饰器链
await smsService.SendAsync("+1234567890", "测试装饰器链");
```

### 7. 自定义服务注册扩展

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SmsSkill;

public static class SmsServiceCollectionExtensions
{
    public static IServiceCollection AddSmsServicesWithDecorators(this IServiceCollection services)
    {
        // 注册基本服务
        services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
        services.AddSingleton<ITemplateService, TemplateService>();
        services.AddSingleton<IAnalyticsService, AnalyticsService>();
        services.AddSingleton<ISmsService, SmsService>();

        // 添加装饰器
        services.Decorate<ISmsService, SmsServiceValidationDecorator>();
        services.Decorate<ISmsService, SmsServiceRetryDecorator>();
        services.Decorate<ISmsService, SmsServiceLoggingDecorator>();

        return services;
    }

    public static IServiceCollection AddSmsServicesWithAssemblyScanning(this IServiceCollection services)
    {
        // 使用程序集扫描
        services.Scan(scan => scan
            .FromAssemblyOf<SmsService>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );

        // 添加装饰器
        services.Decorate<ISmsService, SmsServiceLoggingDecorator>();

        return services;
    }
}

// 使用扩展方法
var services = new ServiceCollection();

// 方法 1: 使用带装饰器的服务注册
services.AddSmsServicesWithDecorators();

// 方法 2: 使用程序集扫描
services.AddSmsServicesWithAssemblyScanning();

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

await smsService.SendAsync("+1234567890", "测试自定义扩展方法");
```

## 高级用法示例

### 1. 自定义 SMS 提供商

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmsSkill;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// 自定义 SMS 提供商
public class CustomSmsProvider : ISmsProvider
{
    private readonly ILogger<CustomSmsProvider> _logger;

    public CustomSmsProvider(ILogger<CustomSmsProvider> logger)
    {
        _logger = logger;
    }

    public Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("使用自定义提供商发送 SMS 到 {To}", to);
        
        // 实现自定义发送逻辑
        // 例如调用其他 SMS API 服务
        
        return Task.FromResult(new SmsResult
        {
            Success = true,
            MessageSid = Guid.NewGuid().ToString(),
            To = to,
            From = "自定义提供商",
            SentAt = DateTime.Now
        });
    }

    public Task<SmsResult> SendWithMediaAsync(string to, string message, IEnumerable<System.Uri> mediaUrls, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("使用自定义提供商发送带媒体的 SMS 到 {To}", to);
        
        // 实现带媒体的发送逻辑
        
        return Task.FromResult(new SmsResult
        {
            Success = true,
            MessageSid = Guid.NewGuid().ToString(),
            To = to,
            From = "自定义提供商",
            SentAt = DateTime.Now
        });
    }

    public Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        // 实现接收逻辑
        return Task.FromResult<IEnumerable<SmsMessage>>(new List<SmsMessage>());
    }
}

// 注册和使用自定义提供商
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole());
services.AddSingleton<ISmsProvider, CustomSmsProvider>();
services.AddSingleton<ITemplateService, TemplateService>();
services.AddSingleton<IAnalyticsService, AnalyticsService>();
services.AddSingleton<ISmsService, SmsService>();

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 使用自定义提供商发送 SMS
var result = await smsService.SendAsync("+1234567890", "使用自定义 SMS 提供商发送的消息");
Console.WriteLine($"发送结果: {(result.Success ? "成功" : "失败")}");
```

### 2. 批量发送优化

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System.Collections.Generic;
using System.Threading.Tasks;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 准备大量收件人
var recipients = new List<string>();
for (int i = 0; i < 1000; i++)
{
    // 模拟生成电话号码
    recipients.Add($"+1234567{i.ToString().PadLeft(4, '0')}");
}

// 批量发送
Console.WriteLine($"开始批量发送，共 {recipients.Count} 条消息");
var startTime = DateTime.Now;

var results = await smsService.BatchSendAsync(recipients, "这是一条批量发送的测试消息");

var endTime = DateTime.Now;
var successCount = results.Count(r => r.Success);
var totalCount = results.Count();

Console.WriteLine($"批量发送完成");
Console.WriteLine($"耗时: {(endTime - startTime).TotalSeconds:F2} 秒");
Console.WriteLine($"成功: {successCount}/{totalCount} ({successCount / (double)totalCount:P2})");
Console.WriteLine($"速度: {totalCount / (endTime - startTime).TotalSeconds:F2} 条/秒");
```

### 3. 定时发送任务管理

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System.Collections.Generic;

var services = new ServiceCollection();
services.AddSmsServices();

using var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

// 准备定时发送任务
var scheduleTasks = new List<(string phone, string message, DateTime time)>()
{
    ("+1234567890", "任务 1: 上午 9 点提醒", DateTime.Now.Date.AddHours(9)),
    ("+1234567890", "任务 2: 中午 12 点提醒", DateTime.Now.Date.AddHours(12)),
    ("+1234567890", "任务 3: 下午 6 点提醒", DateTime.Now.Date.AddHours(18))
};

// 调度所有任务
foreach (var (phone, message, time) in scheduleTasks)
{
    var result = await smsService.ScheduleSendAsync(phone, message, time);
    Console.WriteLine($"调度任务: {(result.Success ? "成功" : "失败")}");
    if (result.Success)
    {
        Console.WriteLine($"  电话: {phone}");
        Console.WriteLine($"  时间: {time}");
        Console.WriteLine($"  消息: {message}");
        Console.WriteLine($"  调度 ID: {result.ScheduleId}");
    }
    else
    {
        Console.WriteLine($"  错误: {result.ErrorMessage}");
    }
    Console.WriteLine();
}

Console.WriteLine("所有定时任务已调度完成");
```

## 集成示例

### 1. 与 ASP.NET Core 集成

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;

var builder = WebApplication.CreateBuilder(args);

// 注册 SMS 服务
builder.Services.AddSmsServices();

// 其他服务注册...
builder.Services.AddControllers();

var app = builder.Build();

// 配置中间件...
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// 在控制器中使用
[ApiController]
[Route("[controller]")]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;

    public SmsController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
    {
        var result = await _smsService.SendAsync(request.PhoneNumber, request.Message);
        return Ok(result);
    }

    [HttpPost("template")]
    public async Task<IActionResult> SendTemplateSms([FromBody] TemplateSmsRequest request)
    {
        var result = await _smsService.SendTemplateAsync(
            request.PhoneNumber,
            request.TemplateId,
            request.Parameters
        );
        return Ok(result);
    }
}

public class SmsRequest
{
    public string PhoneNumber { get; set; }
    public string Message { get; set; }
}

public class TemplateSmsRequest
{
    public string PhoneNumber { get; set; }
    public string TemplateId { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}
```

### 2. 与控制台应用集成

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsSkill;

var builder = Host.CreateDefaultBuilder(args);

// 配置服务
builder.ConfigureServices(services =>
{
    services.AddSmsServices();
    services.AddHostedService<SmsWorker>();
});

var host = builder.Build();
await host.RunAsync();

public class SmsWorker : IHostedService
{
    private readonly ISmsService _smsService;
    private readonly ILogger<SmsWorker> _logger;

    public SmsWorker(ISmsService smsService, ILogger<SmsWorker> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SMS Worker 启动");

        // 发送测试消息
        var result = await _smsService.SendAsync("+1234567890", "控制台应用测试消息");
        _logger.LogInformation("发送结果: {Success}", result.Success);

        // 等待取消
        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SMS Worker 停止");
        return Task.CompletedTask;
    }
}
```

## 测试示例

### 1. 单元测试

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SmsSkill;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class SmsServiceTests
{
    [Fact]
    public async Task SendAsync_ShouldReturnSuccess_WhenProviderReturnsSuccess()
    {
        // 准备模拟对象
        var mockProvider = new Mock<ISmsProvider>();
        mockProvider.Setup(p => p.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmsResult
            {
                Success = true,
                MessageSid = "SM123",
                To = "+1234567890",
                From = "+0987654321",
                SentAt = DateTime.Now
            });

        var mockTemplateService = new Mock<ITemplateService>();
        var mockAnalyticsService = new Mock<IAnalyticsService>();
        var mockOptions = new Mock<IOptions<SmsOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new SmsOptions());
        var mockLogger = new Mock<ILogger<SmsService>>();

        // 创建服务实例
        var smsService = new SmsService(
            mockProvider.Object,
            mockTemplateService.Object,
            mockAnalyticsService.Object,
            mockOptions.Object,
            mockLogger.Object
        );

        // 执行测试
        var result = await smsService.SendAsync("+1234567890", "测试消息");

        // 验证结果
        Assert.True(result.Success);
        Assert.Equal("SM123", result.MessageSid);
        mockProvider.Verify(p => p.SendAsync("+1234567890", "测试消息", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendAsync_ShouldReturnFailure_WhenProviderReturnsFailure()
    {
        // 准备模拟对象
        var mockProvider = new Mock<ISmsProvider>();
        mockProvider.Setup(p => p.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SmsResult
            {
                Success = false,
                To = "+1234567890",
                From = "+0987654321",
                ErrorMessage = "发送失败",
                SentAt = DateTime.Now
            });

        var mockTemplateService = new Mock<ITemplateService>();
        var mockAnalyticsService = new Mock<IAnalyticsService>();
        var mockOptions = new Mock<IOptions<SmsOptions>>();
        mockOptions.Setup(o => o.Value).Returns(new SmsOptions());
        var mockLogger = new Mock<ILogger<SmsService>>();

        // 创建服务实例
        var smsService = new SmsService(
            mockProvider.Object,
            mockTemplateService.Object,
            mockAnalyticsService.Object,
            mockOptions.Object,
            mockLogger.Object
        );

        // 执行测试
        var result = await smsService.SendAsync("+1234567890", "测试消息");

        // 验证结果
        Assert.False(result.Success);
        Assert.Equal("发送失败", result.ErrorMessage);
    }
}
```

### 2. 集成测试

```csharp
using Microsoft.Extensions.DependencyInjection;
using SmsSkill;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class SmsIntegrationTests
{
    [Fact(Skip = "需要真实的 Twilio 凭证")]
    public async Task SendSms_ShouldWorkWithRealTwilio()
    {
        // 配置服务
        var services = new ServiceCollection();
        services.AddSmsServices();

        // 替换为真实的 Twilio 配置
        services.Configure<TwilioOptions>(options =>
        {
            options.AccountSid = "your-real-account-sid";
            options.AuthToken = "your-real-auth-token";
            options.FromNumber = "your-real-from-number";
        });

        using var serviceProvider = services.BuildServiceProvider();
        var smsService = serviceProvider.GetRequiredService<ISmsService>();

        // 发送测试消息
        var result = await smsService.SendAsync("+1234567890", "集成测试消息");

        // 验证结果
        Assert.True(result.Success);
        Assert.NotNull(result.MessageSid);
    }
}
```

## 总结

本文档提供了 SMS 技能的详细使用示例，包括基本用法、Scrutor 高级用法、高级功能和集成示例。通过这些示例，您可以快速上手 SMS 技能，并根据自己的需求进行定制和扩展。

### 关键要点

1. **基本功能**：发送、接收、模板、批量、定时、分析
2. **Scrutor 用法**：装饰器模式、程序集扫描、高级配置
3. **扩展能力**：自定义提供商、装饰器、服务注册
4. **集成选项**：ASP.NET Core、控制台应用、其他 .NET 应用
5. **测试策略**：单元测试、集成测试

SMS 技能设计灵活，易于扩展，可以满足各种 SMS 消息处理需求。通过结合 .NET 10 的新特性和 AOT 编译优化，它提供了高性能、可靠的 SMS 消息处理能力。