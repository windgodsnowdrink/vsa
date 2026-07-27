# mailkit - 使用示例

## 快速开始

### 1. 基本邮件发送示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MailKit.Net.Smtp;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<EmailService>();
        
        Console.WriteLine("mailkit 基本邮件发送示例");
        Console.WriteLine("=" * 50);
        
        // 发送简单邮件
        var message = new EmailMessage
        {
            FromName = "发送者",
            FromEmail = "sender@example.com",
            ToName = "接收者",
            ToEmail = "recipient@example.com",
            Subject = "测试邮件",
            HtmlBody = "<h1>Hello World</h1><p>这是一封测试邮件</p>",
            TextBody = "Hello World\n这是一封测试邮件"
        };
        
        Console.WriteLine("1. 发送简单邮件");
        await emailService.SendEmailAsync(message);
        Console.WriteLine("邮件发送成功！");
        
        // 发送带附件的邮件
        Console.WriteLine("\n2. 发送带附件的邮件");
        var attachmentMessage = new EmailMessage
        {
            FromName = "发送者",
            FromEmail = "sender@example.com",
            ToName = "接收者",
            ToEmail = "recipient@example.com",
            Subject = "带附件的邮件",
            HtmlBody = "<h1>带附件的邮件</h1><p>请查看附件</p>",
            TextBody = "带附件的邮件\n请查看附件",
            Attachments = new List<EmailAttachment>
            {
                new EmailAttachment
                {
                    FileName = "test.txt",
                    Data = System.Text.Encoding.UTF8.GetBytes("这是附件内容")
                }
            }
        };
        
        await emailService.SendEmailAsync(attachmentMessage);
        Console.WriteLine("带附件的邮件发送成功！");
        
        Console.WriteLine("\n基本邮件发送示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置SMTP选项
        var smtpOptions = new SmtpOptions
        {
            Host = "smtp.example.com",
            Port = 587,
            Username = "user@example.com",
            Password = "password",
            UseSsl = true,
            MaxRetryCount = 3
        };
        
        builder.AddSingleton(smtpOptions);
        builder.AddSingleton<EmailService>();
        
        return builder.BuildServiceProvider();
    }
}

// 邮件消息结构
public class EmailMessage
{
    public string FromName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string ToName { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string TextBody { get; set; } = string.Empty;
    public List<EmailAttachment>? Attachments { get; set; }
}

// 邮件附件结构
public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
}

// SMTP选项
public class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public int ConnectionTimeoutSeconds { get; set; } = 30;
}

// 邮件服务
public class EmailService
{
    private readonly SmtpOptions _options;
    
    public EmailService(SmtpOptions options)
    {
        _options = options;
    }
    
    public async Task SendEmailAsync(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(message.FromName, message.FromEmail));
        mimeMessage.To.Add(new MailboxAddress(message.ToName, message.ToEmail));
        mimeMessage.Subject = message.Subject;
        
        var builder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody
        };
        
        // 添加附件
        if (message.Attachments != null)
        {
            foreach (var attachment in message.Attachments)
            {
                using var stream = new MemoryStream(attachment.Data);
                builder.Attachments.Add(attachment.FileName, stream);
            }
        }
        
        mimeMessage.Body = builder.ToMessageBody();
        
        // 发送邮件
        using var client = new SmtpClient();
        await client.ConnectAsync(
            _options.Host, 
            _options.Port, 
            _options.UseSsl ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None,
            cancellationToken: new CancellationTokenSource(_options.ConnectionTimeoutSeconds * 1000).Token);
        
        if (!string.IsNullOrEmpty(_options.Username))
        {
            await client.AuthenticateAsync(_options.Username, _options.Password);
        }
        
        await client.SendAsync(mimeMessage);
        await client.DisconnectAsync(true);
    }
}
```

### 2. Papercut 集成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("mailkit Papercut 集成示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = BuildHost();
        
        Console.WriteLine("1. 启动 Papercut SMTP 服务器");
        await host.StartAsync();
        
        Console.WriteLine("Papercut SMTP 服务器启动成功！");
        Console.WriteLine("服务器地址: 0.0.0.0:25");
        Console.WriteLine("邮件存储路径: ./mailstore");
        
        // 获取邮件服务
        var emailService = host.Services.GetRequiredService<EmailService>();
        
        Console.WriteLine("\n2. 测试邮件发送到 Papercut");
        var testMessage = new EmailMessage
        {
            FromName = "Test Sender",
            FromEmail = "test@example.com",
            ToName = "Test Recipient",
            ToEmail = "recipient@example.com",
            Subject = "Papercut 测试邮件",
            HtmlBody = "<h1>Papercut 测试</h1><p>这封邮件应该被 Papercut 捕获</p>"
        };
        
        await emailService.SendEmailAsync(testMessage);
        Console.WriteLine("测试邮件发送成功！");
        Console.WriteLine("请在 Papercut 界面中查看邮件");
        
        Console.WriteLine("\n3. 停止服务器");
        await host.StopAsync();
        Console.WriteLine("Papercut SMTP 服务器已停止");
        
        Console.WriteLine("\nPapercut 集成示例完成！");
    }
    
    private static IHost BuildHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 SMTP 选项（指向 Papercut 服务器）
                services.AddSingleton(new SmtpOptions
                {
                    Host = "localhost",
                    Port = 25,
                    Username = "",
                    Password = "",
                    UseSsl = false
                });
                
                // 添加邮件服务
                services.AddSingleton<EmailService>();
                
                // 添加 Papercut 服务
                services.AddEmailServer();
                
                // 添加邮件处理服务
                services.AddHostedService<EmailProcessingService>();
            })
            .Build();
    }
}

// Papercut 服务扩展
public static class EmailServerExtensions
{
    public static IServiceCollection AddEmailServer(this IServiceCollection services)
    {
        // 这里添加 Papercut 相关服务
        // 实际项目中需要引用 Papercut.Smtp 包
        return services;
    }
}

// 邮件处理服务
public class EmailProcessingService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("邮件处理服务启动");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            // 模拟邮件处理
            await Task.Delay(1000, stoppingToken);
        }
        
        Console.WriteLine("邮件处理服务停止");
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("mailkit 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<OptimizedEmailService>();
        
        // 准备测试数据
        var testMessages = new List<EmailMessage>();
        for (int i = 0; i < 100; i++)
        {
            testMessages.Add(new EmailMessage
            {
                FromName = "Sender",
                FromEmail = "sender@example.com",
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = $"Test Email {i}",
                HtmlBody = $"<h1>Test Email {i}</h1><p>This is test email {i}</p>"
            });
        }
        
        // 性能测试
        const int iterations = 10;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine("测试高性能邮件发送...");
        for (int i = 0; i < iterations; i++)
        {
            Console.WriteLine($"测试迭代 {i + 1}/{iterations}");
            
            // 并行发送邮件
            var tasks = testMessages.Select(message => emailService.SendEmailAsync(message)).ToList();
            await Task.WhenAll(tasks);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"\n性能测试结果:");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalSeconds:F2} 秒");
        Console.WriteLine($"平均每次执行: {stopwatch.Elapsed.TotalSeconds / iterations:F3} 秒");
        Console.WriteLine($"处理速度: {testMessages.Count * iterations / stopwatch.Elapsed.TotalSeconds:F2} 封/秒");
        Console.WriteLine($"总邮件数: {testMessages.Count * iterations}");
        
        Console.WriteLine("\n性能优化示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 创建高性能通道
        var emailChannel = Channel.CreateBounded<EmailMessage>(
            new BoundedChannelOptions(1000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        builder.AddSingleton(emailChannel);
        builder.AddSingleton(new SmtpOptions
        {
            Host = "smtp.example.com",
            Port = 587,
            Username = "user@example.com",
            Password = "password",
            UseSsl = true
        });
        
        builder.AddSingleton<OptimizedEmailService>();
        builder.AddHostedService<EmailWorker>();
        
        return builder.BuildServiceProvider();
    }
}

// 优化的邮件服务
public class OptimizedEmailService
{
    private readonly Channel<EmailMessage> _channel;
    
    public OptimizedEmailService(Channel<EmailMessage> channel)
    {
        _channel = channel;
    }
    
    public async Task SendEmailAsync(EmailMessage message)
    {
        await _channel.Writer.WriteAsync(message);
    }
}

// 邮件处理工作器
public class EmailWorker : BackgroundService
{
    private readonly Channel<EmailMessage> _channel;
    private readonly SmtpOptions _options;
    
    public EmailWorker(Channel<EmailMessage> channel, SmtpOptions options)
    {
        _channel = channel;
        _options = options;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            // 模拟邮件处理
            await Task.Delay(10);
        }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("mailkit 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<FaultTolerantEmailService>();
        
        try
        {
            Console.WriteLine("1. 测试正常邮件发送");
            var normalMessage = new EmailMessage
            {
                FromName = "Sender",
                FromEmail = "sender@example.com",
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = "正常邮件",
                HtmlBody = "<h1>正常邮件</h1><p>这是一封正常邮件</p>"
            };
            
            await emailService.SendEmailAsync(normalMessage);
            Console.WriteLine("✓ 正常邮件发送成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 错误: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n2. 测试无效的 SMTP 服务器");
            var invalidServerMessage = new EmailMessage
            {
                FromName = "Sender",
                FromEmail = "sender@example.com",
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = "测试邮件",
                HtmlBody = "<h1>测试邮件</h1><p>测试无效服务器</p>"
            };
            
            await emailService.SendEmailWithInvalidServerAsync(invalidServerMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✓ 捕获到预期错误: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n3. 测试无效的邮箱地址");
            var invalidEmailMessage = new EmailMessage
            {
                FromName = "Sender",
                FromEmail = "invalid-email", // 无效的邮箱格式
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = "测试邮件",
                HtmlBody = "<h1>测试邮件</h1><p>测试无效邮箱</p>"
            };
            
            await emailService.SendEmailAsync(invalidEmailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✓ 捕获到预期错误: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n4. 测试超时情况");
            var timeoutMessage = new EmailMessage
            {
                FromName = "Sender",
                FromEmail = "sender@example.com",
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = "测试邮件",
                HtmlBody = "<h1>测试邮件</h1><p>测试超时</p>"
            };
            
            await emailService.SendEmailWithTimeoutAsync(timeoutMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✓ 捕获到预期错误: {ex.Message}");
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 SMTP 选项
        builder.AddSingleton(new SmtpOptions
        {
            Host = "smtp.example.com",
            Port = 587,
            Username = "user@example.com",
            Password = "password",
            UseSsl = true
        });
        
        builder.AddSingleton<FaultTolerantEmailService>();
        
        return builder.BuildServiceProvider();
    }
}

// 容错的邮件服务
public class FaultTolerantEmailService : EmailService
{
    public FaultTolerantEmailService(SmtpOptions options) : base(options)
    {
    }
    
    public async Task SendEmailWithInvalidServerAsync(EmailMessage message)
    {
        // 使用无效的 SMTP 服务器
        var invalidOptions = new SmtpOptions
        {
            Host = "invalid-server.example.com",
            Port = 587,
            UseSsl = true,
            ConnectionTimeoutSeconds = 5
        };
        
        var tempService = new EmailService(invalidOptions);
        await tempService.SendEmailAsync(message);
    }
    
    public async Task SendEmailWithTimeoutAsync(EmailMessage message)
    {
        // 使用会超时的配置
        var timeoutOptions = new SmtpOptions
        {
            Host = "192.0.2.1", // 保留地址，不会响应
            Port = 587,
            UseSsl = true,
            ConnectionTimeoutSeconds = 2
        };
        
        var tempService = new EmailService(timeoutOptions);
        await tempService.SendEmailAsync(message);
    }
}
```

### 5. AOT 编译示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package MailKit@4.3.0 
#:package MimeKit@4.3.0 
#:package System.Threading.Channels@8.0.0 
#:property LangVersion=preview 
#:property TargetFramework=net11.0 
#:property Nullable=enable 
#:property ImplicitUsings=enable 
#:property PublishAot=true 
#:property IncludeNativeLibrariesForSelfExtract=true 
#:property EnableCppCodeGen=true 
#:property PublishSingleFile=true 
#:property SelfContained=true 
#:property RuntimeIdentifier=win-x64 

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

public class AotExample
{
    public static async Task Main()
    {
        Console.WriteLine("mailkit AOT 编译示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("使用 AOT 编译的单文件可执行程序");
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<OptimizedEmailService>();
        
        // 测试 AOT 编译后的性能
        var testMessages = new List<EmailMessage> {
            new EmailMessage {
                FromName = "AOT Test",
                FromEmail = "test@example.com",
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = "AOT 测试邮件 1",
                HtmlBody = "<h1>AOT 测试</h1><p>这是 AOT 编译测试邮件 1</p>"
            },
            new EmailMessage {
                FromName = "AOT Test",
                FromEmail = "test@example.com",
                ToName = "Recipient",
                ToEmail = "recipient@example.com",
                Subject = "AOT 测试邮件 2",
                HtmlBody = "<h1>AOT 测试</h1><p>这是 AOT 编译测试邮件 2</p>"
            }
        };
        
        Console.WriteLine("\n测试 AOT 编译后的邮件发送");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        foreach (var message in testMessages)
        {
            await emailService.SendEmailAsync(message);
            Console.WriteLine($"✓ 邮件发送成功: {message.Subject}");
        }
        
        stopwatch.Stop();
        Console.WriteLine($"\nAOT 编译性能测试:");
        Console.WriteLine($"处理时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均每封: {stopwatch.Elapsed.TotalMilliseconds / testMessages.Count:F3} ms");
        
        Console.WriteLine("\nAOT 编译示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 创建高性能通道
        var emailChannel = Channel.CreateBounded<EmailMessage>(
            new BoundedChannelOptions(1000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        builder.AddSingleton(emailChannel);
        builder.AddSingleton(new SmtpOptions
        {
            Host = "smtp.example.com",
            Port = 587,
            Username = "user@example.com",
            Password = "password",
            UseSsl = true
        });
        
        builder.AddSingleton<OptimizedEmailService>();
        builder.AddHostedService<EmailWorker>();
        
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例展示了 mailkit 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本邮件发送操作
2. 发送带附件的邮件
3. 集成 Papercut 本地邮件服务器
4. 优化邮件发送性能
5. 处理各种错误情况
6. 使用 AOT 编译提升性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 支持的功能

- **基本邮件发送**：支持发送文本和 HTML 格式的邮件
- **附件处理**：支持添加多个附件
- **批量发送**：支持批量处理和发送邮件
- **错误处理**：完善的错误处理和重试机制
- **性能优化**：使用通道、对象池和零拷贝技术优化性能
- **Papercut 集成**：支持与 Papercut 本地邮件服务器集成
- **AOT 编译**：支持 AOT 编译，提升启动速度和运行性能

### 性能优化特点

- **Threading.Channels**：高效的异步邮件队列处理
- **ObjectPool**：减少 SMTP 客户端创建开销
- **Span 零拷贝**：减少内存分配和复制
- **TailLatencyOptimizer**：尾延迟优化
- **SIMD 指令**：使用 SIMD 指令优化邮件处理
- **Cache-line 对齐**：内存分配优化
