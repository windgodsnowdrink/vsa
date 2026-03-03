# mailkit - 参考文档

## 概述

mailkit 是基于 .NET 10 开发的高性能邮件处理系统，专为 .NET 开发者设计，提供强大的邮件发送、接收和处理功能。

## 核心组件

### 1. EmailService（邮件服务）
- **位置**: scripts/mailkit_integration.cs
- **功能**: 核心邮件发送和处理业务逻辑
- **特性**: 
  - 支持 SMTP 邮件发送
  - 支持邮件附件处理
  - 高性能异步处理
  - 错误处理和重试机制
  - 详细的日志记录

### 2. EmailProcessingService（邮件处理服务）
- **位置**: scripts/papercut_integration.cs
- **功能**: 高性能邮件处理和 Papercut 集成
- **特性**: 
  - 支持 Papercut 本地邮件服务器集成
  - 高性能邮件队列处理
  - 邮件存储和管理
  - 邮件处理监控

### 3. ChannelProcessor（通道处理器）
- **位置**: 各 C# 文件中
- **功能**: 高效的异步邮件队列处理
- **特性**: 
  - 使用 Threading.Channels 进行背压控制
  - 支持高并发邮件处理
  - 内存优化设计

## AOT 架构说明

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### 性能优化技术

1. **Threading.Channels**: 高效的异步事件队列处理，支持背压控制
2. **ObjectPool**: 减少对象创建开销，优化内存使用
3. **Span 零拷贝**: 减少内存分配和复制
4. **TailLatencyOptimizer**: 尾延迟优化
5. **AggressiveOptimization**: 编译器级优化
6. **Cache-line 对齐**: 内存分配优化
7. **SIMD 指令**: 使用 SIMD 指令优化邮件处理

## 使用示例

### 基本用法

```csharp
var emailService = serviceProvider.GetRequiredService<EmailService>();

// 发送简单邮件
await emailService.SendEmailAsync(new EmailMessage
{
    FromName = "发送者",
    FromEmail = "sender@example.com",
    ToName = "接收者",
    ToEmail = "recipient@example.com",
    Subject = "测试邮件",
    HtmlBody = "<h1>Hello World</h1><p>这是一封测试邮件</p>",
    TextBody = "Hello World\n这是一封测试邮件"
});

// 发送带附件的邮件
await emailService.SendEmailAsync(new EmailMessage
{
    FromName = "发送者",
    FromEmail = "sender@example.com",
    ToName = "接收者",
    ToEmail = "recipient@example.com",
    Subject = "带附件的邮件",
    HtmlBody = "<h1>Hello World</h1><p>请查看附件</p>",
    TextBody = "Hello World\n请查看附件",
    Attachments = new List<EmailAttachment>
    {
        new EmailAttachment
        {
            FileName = "document.pdf",
            Data = File.ReadAllBytes("document.pdf")
        }
    }
});
```

### 高级配置

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置邮件服务
builder.Services.AddEmailService(options => {
    options.Host = "smtp.example.com";
    options.Port = 587;
    options.Username = "user@example.com";
    options.Password = "password";
    options.UseSsl = true;
    options.MaxRetryCount = 3;
    options.ConnectionTimeoutSeconds = 30;
});

// 配置 Papercut 邮件服务器
builder.Services.AddEmailServer();

// 注册后台服务
builder.Services.AddHostedService<EmailProcessingService>();

var app = builder.Build();

// 启动 Papercut SMTP 服务器
app.UsePapercutSmtp();

app.MapGet("/send-email", async (EmailService emailService) => {
    await emailService.SendEmailAsync(new EmailMessage
    {
        FromName = "System",
        FromEmail = "no-reply@example.com",
        ToName = "User",
        ToEmail = "user@example.com",
        Subject = "系统通知",
        HtmlBody = "<h1>系统通知</h1><p>这是一条系统通知</p>"
    });
    return "邮件发送成功";
});

app.Run();
```

## 配置选项

### SMTP 配置

```json
{
  "Smtp": {
    "Host": "smtp.example.com",        // SMTP 服务器地址
    "Port": 587,                      // SMTP 服务器端口
    "Username": "user@example.com",   // SMTP 用户名
    "Password": "password",           // SMTP 密码
    "UseSsl": true,                   // 是否使用 SSL
    "MaxRetryCount": 3,               // 最大重试次数
    "ConnectionTimeoutSeconds": 30     // 连接超时时间
  }
}
```

### 邮件配置

```json
{
  "Email": {
    "DefaultFrom": "no-reply@example.com",  // 默认发件人
    "DefaultFromName": "System Notification", // 默认发件人名称
    "MaxAttachmentsSize": 10485760,          // 最大附件大小（10MB）
    "MaxRecipients": 50,                     // 最大收件人数
    "EnableTracking": true                   // 是否启用邮件跟踪
  }
}
```

### Papercut 配置

```json
{
  "Papercut": {
    "Smtp": {
      "Ip": "0.0.0.0",                 // SMTP 服务器 IP
      "Port": 25,                       // SMTP 服务器端口
      "MaxConnections": 100,             // 最大连接数
      "MaxMessageSize": 20971520,        // 最大邮件大小（20MB）
      "EnableTls": false,                // 是否启用 TLS
      "IdleTimeoutSeconds": 300          // 空闲超时时间
    },
    "MessageStore": {
      "Path": "./mailstore",            // 邮件存储路径
      "MaxMessages": 10000,              // 最大存储邮件数
      "CleanupIntervalHours": 24,         // 清理间隔（小时）
      "EnableCompression": true          // 是否启用压缩
    }
  }
}
```

## 性能优化

1. **连接池使用**: 使用 SMTP 客户端对象池减少连接开销
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理邮件提高效率
4. **内存优化**: 使用 Span 零拷贝和对象池减少内存分配
5. **通道配置**: 根据实际负载调整通道容量
6. **缓存使用**: 缓存常用邮件模板和配置

## 故障排除

### 常见问题

1. **SMTP 连接失败**
   - 检查 SMTP 服务器配置
   - 验证网络连接和防火墙设置
   - 查看日志信息
   - 确认 SMTP 用户名和密码正确

2. **邮件发送超时**
   - 检查网络连接质量
   - 增加连接超时时间
   - 检查 SMTP 服务器响应速度
   - 减少邮件附件大小

3. **性能问题**
   - 调整通道容量和并发设置
   - 增加对象池大小
   - 优化邮件处理逻辑
   - 启用邮件压缩

4. **AOT 编译问题**
   - 确保所有依赖支持 AOT 编译
   - 检查运行时标识符设置
   - 验证单文件可执行配置

## 扩展开发

### 添加自定义邮件处理器

```csharp
public class CustomEmailProcessor : EmailService
{
    public CustomEmailProcessor(SmtpOptions options) : base(options)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ProcessEmailAsync(SmtpClient client, EmailMessage message)
    {
        // 自定义邮件处理逻辑
        
        // 1. 邮件内容处理
        var processedMessage = await ProcessEmailContentAsync(message);
        
        // 2. 调用基类方法发送邮件
        await base.ProcessEmailAsync(client, processedMessage);
        
        // 3. 发送后处理
        await PostProcessEmailAsync(message);
    }

    private async Task<EmailMessage> ProcessEmailContentAsync(EmailMessage message)
    {
        // 处理邮件内容，如添加签名、替换变量等
        message.HtmlBody += "<p>--<br>这是自动添加的签名</p>";
        return message;
    }

    private async Task PostProcessEmailAsync(EmailMessage message)
    {
        // 发送后处理，如记录发送日志、触发后续操作等
        Console.WriteLine($"邮件已发送: {message.Subject} 到 {message.ToEmail}");
    }
}

// 注册自定义处理器
builder.Services.AddSingleton<EmailService>(sp => {
    var options = sp.GetRequiredService<SmtpOptions>();
    return new CustomEmailProcessor(options);
});
```

### 扩展 Papercut 功能

```csharp
public class CustomPapercutService : EmailProcessingService
{
    public CustomPapercutService(
        Channel<EmailMessage> channel,
        IObjectPool<SmtpClient> smtpPool)
        : base(channel, smtpPool)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 自定义执行逻辑
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            // 添加自定义处理
            await PreProcessMessageAsync(message);
            
            // 调用基类方法处理
            await base.ProcessEmailAsync(message);
            
            // 自定义后处理
            await PostProcessMessageAsync(message);
        }
    }

    private async Task PreProcessMessageAsync(EmailMessage message)
    {
        // 预处理邮件，如内容过滤、格式转换等
    }

    private async Task PostProcessMessageAsync(EmailMessage message)
    {
        // 后处理邮件，如存储到数据库、触发通知等
    }
}

// 注册自定义服务
builder.Services.AddHostedService<CustomPapercutService>();
```
