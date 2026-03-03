# MimeKit - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        Console.WriteLine("MimeKit 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建邮件
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
        message.To.Add(new MailboxAddress("收件人", "recipient@example.com"));
        message.Subject = "测试邮件";
        message.Body = new TextPart(TextFormat.Plain) {
            Text = "这是一封测试邮件"
        };
        
        // 发送邮件
        await emailService.SendEmailAsync(message);
        Console.WriteLine("邮件发送成功");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
        });
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MimeKit 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置邮件处理服务
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.MaxRetries = 3;
            options.RetryDelay = TimeSpan.FromSeconds(5);
            options.EnableDetailedLogging = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<EmailProcessingOptions>>().Value;
        Console.WriteLine($"配置信息: Server={settings.SmtpServer}, Port={settings.SmtpPort}, SSL={settings.EnableSsl}");
        
        // 使用服务
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        // 构建邮件
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
        message.To.Add(new MailboxAddress("收件人", "recipient@example.com"));
        message.Subject = "高级配置测试邮件";
        
        // 创建混合内容（文本和HTML）
        var builderBody = new BodyBuilder();
        builderBody.TextBody = "这是纯文本内容";
        builderBody.HtmlBody = "<p>这是 <b>HTML</b> 内容</p>";
        message.Body = builderBody.ToMessageBody();
        
        // 发送邮件
        await emailService.SendEmailAsync(message);
        Console.WriteLine("邮件发送成功");
    }
}
```

### 3. 附件处理示例

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MimeKit 附件处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        // 构建邮件
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
        message.To.Add(new MailboxAddress("收件人", "recipient@example.com"));
        message.Subject = "带附件的测试邮件";
        
        // 创建带附件的内容
        var builderBody = new BodyBuilder();
        builderBody.TextBody = "这是带附件的测试邮件";
        
        // 添加文本文件附件
        builderBody.Attachments.Add("document.txt", File.ReadAllBytes("document.txt"), ContentType.Parse("text/plain"));
        
        // 添加图片附件
        builderBody.Attachments.Add("image.jpg", File.ReadAllBytes("image.jpg"), ContentType.Parse("image/jpeg"));
        
        message.Body = builderBody.ToMessageBody();
        
        // 发送邮件
        await emailService.SendEmailAsync(message);
        Console.WriteLine("带附件的邮件发送成功");
    }
}
```

### 4. 批量邮件处理示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MimeKit 批量邮件处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        // 批量邮件列表
        var recipients = new List<string> {
            "recipient1@example.com",
            "recipient2@example.com",
            "recipient3@example.com"
        };
        
        var tasks = new List<Task>();
        
        foreach (var recipient in recipients)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
            message.To.Add(new MailboxAddress("收件人", recipient));
            message.Subject = "批量邮件测试";
            message.Body = new TextPart(TextFormat.Plain) {
                Text = $"这是发给 {recipient} 的批量测试邮件"
            };
            
            // 异步发送邮件
            tasks.Add(emailService.SendEmailAsync(message));
        }
        
        // 等待所有邮件发送完成
        await Task.WhenAll(tasks);
        Console.WriteLine($"批量邮件发送完成，共发送 {tasks.Count} 封邮件");
    }
}
```

### 5. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MimeKit 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        try
        {
            // 构建邮件
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
            message.To.Add(new MailboxAddress("收件人", "recipient@example.com"));
            message.Subject = "错误处理测试邮件";
            message.Body = new TextPart(TextFormat.Plain) {
                Text = "这是错误处理测试邮件"
            };
            
            // 发送邮件
            await emailService.SendEmailAsync(message);
            Console.WriteLine("邮件发送成功");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
    }
}
```

## 性能优化示例

### 1. 内存优化示例

```csharp
using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MimeKit 内存优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        // 使用内存池优化大附件处理
        using var memory = MemoryPool<byte>.Shared.Rent(1024 * 1024); // 1MB 缓冲区
        var buffer = memory.Memory;
        
        // 构建邮件
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
        message.To.Add(new MailboxAddress("收件人", "recipient@example.com"));
        message.Subject = "内存优化测试邮件";
        
        // 创建带附件的内容
        var builderBody = new BodyBuilder();
        builderBody.TextBody = "这是内存优化测试邮件";
        
        // 使用 Span<byte> 处理附件数据
        Span<byte> attachmentData = buffer.Span.Slice(0, 1024); // 模拟1KB附件
        // 填充数据...
        
        builderBody.Attachments.Add("optimized.txt", attachmentData.ToArray(), ContentType.Parse("text/plain"));
        message.Body = builderBody.ToMessageBody();
        
        // 发送邮件
        await emailService.SendEmailAsync(message);
        Console.WriteLine("内存优化邮件发送成功");
    }
}
```

### 2. 并发优化示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MimeKit 并发优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.AddEmailProcessingServices(options => {
            options.SmtpServer = "smtp.example.com";
            options.SmtpPort = 587;
            options.SmtpUsername = "username";
            options.SmtpPassword = "password";
            options.EnableSsl = true;
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        var emailService = serviceProvider.GetRequiredService<IEmailService>();
        
        // 批量邮件列表
        var recipients = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            recipients.Add($"recipient{i}@example.com");
        }
        
        // 使用并行处理
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        
        Parallel.ForEach(recipients, options, async recipient => {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("发件人", "sender@example.com"));
            message.To.Add(new MailboxAddress("收件人", recipient));
            message.Subject = "并发测试邮件";
            message.Body = new TextPart(TextFormat.Plain) {
                Text = $"这是发给 {recipient} 的并发测试邮件"
            };
            
            await emailService.SendEmailAsync(message);
            Console.WriteLine($"邮件发送成功: {recipient}");
        });
        
        Console.WriteLine("并发邮件发送完成");
    }
}
```

## 总结

以上示例展示了 MimeKit 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始基本操作
2. 配置高级选项
3. 处理邮件附件
4. 批量处理邮件
5. 优化内存使用
6. 实现并发处理
7. 正确处理错误情况

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
