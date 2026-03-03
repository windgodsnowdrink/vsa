# SMS 技能文档

## 技能概述

SMS 技能是一个基于 .NET 10 开发的完整 SMS 消息处理解决方案，支持 AOT 编译，集成了 Twilio 服务提供商，提供了丰富的 SMS 发送、接收、模板管理、批量发送、定时发送和分析功能。

### 主要特性

- **AOT 编译优化**：支持 Ahead-of-Time 编译，提供更快的启动速度和更低的内存占用
- **完整的 SMS 功能**：发送、接收、模板管理、批量发送、定时发送和分析
- **Twilio 集成**：与 Twilio SMS 服务提供商无缝集成
- **Scrutor 支持**：提供高级服务注册和装饰器模式的使用示例
- **命令行接口**：通过 System.CommandLine 提供友好的命令行操作界面
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection 实现服务管理
- **异步编程**：全面使用 async/await 模式提高性能

## 快速开始

### 环境要求

- .NET 10.0 或更高版本
- Twilio 账户和 API 凭证
- Visual Studio 2026 或 VS Code

### 安装步骤

1. **克隆或下载技能**：将 SMS 技能目录复制到您的项目中
2. **配置 Twilio 凭证**：在 `sms_core.cs` 文件中设置您的 Twilio Account SID、Auth Token 和发件人号码
3. **编译技能**：使用 .NET CLI 编译技能
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
   ```
4. **运行技能**：使用命令行工具执行 SMS 操作
   ```bash
   sms send --to "+1234567890" --message "Hello from SMS Skill!"
   ```

## 核心功能

### 1. SMS 发送

支持单条 SMS 消息的发送，包括文本内容、媒体附件等。

**功能特点**：
- 支持国际电话号码格式
- 自动处理消息长度限制
- 提供发送状态跟踪
- 支持异步发送操作

### 2. SMS 接收

支持接收和处理传入的 SMS 消息，包括自动回复、消息转发等功能。

**功能特点**：
- 支持 webhook 接收模式
- 消息内容解析和处理
- 自动回复配置
- 消息存储和检索

### 3. SMS 模板

支持 SMS 模板的创建、管理和使用，实现消息内容的标准化和个性化。

**功能特点**：
- 模板参数替换
- 模板版本管理
- 多语言模板支持
- 模板使用统计

### 4. 批量发送

支持向多个收件人批量发送 SMS 消息，提高发送效率。

**功能特点**：
- 支持从文件导入收件人列表
- 批量发送速率控制
- 发送状态批量跟踪
- 失败重试机制

### 5. 定时发送

支持设置 SMS 消息的发送时间，实现定时发送功能。

**功能特点**：
- 支持绝对时间和相对时间设置
- 定时任务管理
- 发送前取消功能
- 时区自动调整

### 6. 发送分析

提供 SMS 发送的统计和分析功能，帮助了解发送效果和趋势。

**功能特点**：
- 发送成功率统计
- 发送时间分布分析
- 消息长度统计
- 发送趋势图表

## API 参考

### ISmsService 接口

```csharp
public interface ISmsService
{
    Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default);
    Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
    Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default);
    Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default);
    Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
```

### ITemplateService 接口

```csharp
public interface ITemplateService
{
    Task<string> CreateTemplateAsync(string name, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<SmsTemplate>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<SmsTemplate> GetTemplateAsync(string templateId, CancellationToken cancellationToken = default);
    Task UpdateTemplateAsync(string templateId, string name, string content, CancellationToken cancellationToken = default);
    Task DeleteTemplateAsync(string templateId, CancellationToken cancellationToken = default);
    Task<string> RenderTemplateAsync(string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}
```

### IAnalyticsService 接口

```csharp
public interface IAnalyticsService
{
    Task RecordSmsEventAsync(SmsEvent @event, CancellationToken cancellationToken = default);
    Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<SmsEvent>> GetEventsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
```

### ISmsProvider 接口

```csharp
public interface ISmsProvider
{
    Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default);
    Task<SmsResult> SendWithMediaAsync(string to, string message, IEnumerable<Uri> mediaUrls, CancellationToken cancellationToken = default);
    Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default);
}
```

## AOT 编译

### 编译配置

SMS 技能支持 AOT 编译，通过以下配置实现：

```yaml
compilation:
  targetFramework: net10.0
  publishAot: true
  trimMode: partial
  selfContained: true
  publishSingleFile: true
  langVersion: latest
  nullable: enable
  implicitUsings: enable
```

### 编译命令

```bash
# Windows
 dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true

# Linux
 dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true

# macOS
 dotnet publish -c Release -r osx-x64 --self-contained true -p:PublishSingleFile=true -p:PublishAot=true
```

### AOT 优化效果

- **启动速度**：比 JIT 编译快 3-5 倍
- **内存占用**：减少约 20-30%
- **运行性能**：关键路径性能提升 5-15%
- **部署大小**：单文件部署，便于分发

## 示例

### 基本发送示例

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSmsServices();

var serviceProvider = services.BuildServiceProvider();
var smsService = serviceProvider.GetRequiredService<ISmsService>();

var result = await smsService.SendAsync("+1234567890", "Hello from SMS Skill!");
Console.WriteLine($"SMS sent: {result.Success}");
Console.WriteLine($"Message SID: {result.MessageSid}");
```

### 模板使用示例

```csharp
var templateService = serviceProvider.GetRequiredService<ITemplateService>();

// 创建模板
var templateId = await templateService.CreateTemplateAsync(
    "welcome",
    "欢迎 {{name}} 使用我们的服务！您的验证码是：{{code}}"
);

// 使用模板发送
var parameters = new Dictionary<string, string>
{
    { "name", "张三" },
    { "code", "123456" }
};

var result = await smsService.SendTemplateAsync("+1234567890", templateId, parameters);
```

### 批量发送示例

```csharp
var recipients = new List<string>
{
    "+1234567890",
    "+0987654321",
    "+1122334455"
};

var results = await smsService.BatchSendAsync(recipients, "重要通知：系统将于明天进行维护升级");

foreach (var result in results)
{
    Console.WriteLine($"To: {result.To}, Success: {result.Success}, SID: {result.MessageSid}");
}
```

### 定时发送示例

```csharp
var sendTime = DateTime.Now.AddHours(2);
var result = await smsService.ScheduleSendAsync(
    "+1234567890",
    "提醒：您的会议将在 30 分钟后开始",
    sendTime
);

Console.WriteLine($"Scheduled SMS: {result.Success}");
Console.WriteLine($"Schedule ID: {result.ScheduleId}");
Console.WriteLine($"Send Time: {result.SendTime}");
```

### 分析查询示例

```csharp
var analyticsService = serviceProvider.GetRequiredService<IAnalyticsService>();
var analytics = await analyticsService.GetAnalyticsAsync(
    DateTime.Now.AddDays(-30),
    DateTime.Now
);

Console.WriteLine($"Total SMS: {analytics.TotalSms}");
Console.WriteLine($"Success Rate: {analytics.SuccessRate:P2}");
Console.WriteLine($"Average Length: {analytics.AverageMessageLength}");
```

## Scrutor 使用示例

### 服务注册示例

```csharp
using Scrutor;

var services = new ServiceCollection();

// 基本注册
services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
services.AddSingleton<ITemplateService, TemplateService>();
services.AddSingleton<IAnalyticsService, AnalyticsService>();

// 使用 Scrutor 装饰器模式
services.Decorate<ISmsService, SmsServiceLoggingDecorator>();
services.Decorate<ISmsService, SmsServiceRetryDecorator>();

// 使用 Scrutor 程序集扫描
services.Scan(scan => scan
    .FromAssemblyOf<SmsService>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

var serviceProvider = services.BuildServiceProvider();
```

### 装饰器模式示例

```csharp
public class SmsServiceLoggingDecorator : ISmsService
{
    private readonly ISmsService _decorated;
    private readonly ILogger<SmsServiceLoggingDecorator> _logger;

    public SmsServiceLoggingDecorator(ISmsService decorated, ILogger<SmsServiceLoggingDecorator> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }

    public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending SMS to {To}", to);
        var result = await _decorated.SendAsync(to, message, cancellationToken);
        _logger.LogInformation("SMS sent to {To}, Success: {Success}", to, result.Success);
        return result;
    }

    // 其他方法实现...
}
```

## 故障排除

### 常见问题

1. **发送失败**
   - 检查 Twilio 凭证是否正确
   - 确认收件人号码格式正确
   - 检查 Twilio 账户余额
   - 查看发送日志获取详细错误信息

2. **接收不到消息**
   - 确认 Twilio webhook 配置正确
   - 检查网络连接和防火墙设置
   - 验证接收服务是否正常运行
   - 查看接收日志获取详细错误信息

3. **批量发送速率限制**
   - Twilio 对发送速率有一定限制
   - 调整批量发送的速率控制参数
   - 考虑使用 Twilio 的消息队列功能

4. **AOT 编译错误**
   - 确保所有依赖项都支持 AOT 编译
   - 检查代码中是否使用了不支持 AOT 的功能
   - 调整 trimMode 配置为 partial 或 copyused
   - 查看编译日志获取详细错误信息

### 日志和监控

SMS 技能集成了 Microsoft.Extensions.Logging 框架，提供了详细的日志记录：

- **日志级别**：支持 Trace、Debug、Information、Warning、Error、Critical 等级别
- **日志输出**：可配置为控制台、文件、数据库等输出目标
- **性能监控**：记录关键操作的执行时间
- **错误追踪**：捕获和记录异常信息

### 性能优化

1. **连接池管理**：使用连接池减少网络连接开销
2. **批量操作**：合并多个小操作为批量操作
3. **缓存策略**：缓存常用模板和配置信息
4. **异步处理**：充分使用异步编程模式
5. **重试机制**：实现智能重试策略处理临时故障

## 总结

SMS 技能是一个功能完整、性能优化的 SMS 消息处理解决方案，基于 .NET 10 和 AOT 编译技术，提供了丰富的功能和友好的使用接口。通过集成 Twilio 服务提供商和 Scrutor 高级服务注册库，它不仅满足了基本的 SMS 发送和接收需求，还提供了模板管理、批量发送、定时发送和分析等高级功能。

无论是用于企业通知、客户服务、验证码发送还是营销活动，SMS 技能都能提供可靠、高效的 SMS 消息处理能力。