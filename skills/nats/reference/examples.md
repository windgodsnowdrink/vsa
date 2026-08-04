# NATS - 使用示例

## 快速入门

### 1. 基本用法示例

`csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NATS 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var natsService = serviceProvider.GetRequiredService<INatsService>();
        var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
        var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();
        
        // 检查连接状态
        var isConnected = await natsService.IsConnectedAsync();
        Console.WriteLine($"NATS 连接状态: {isConnected}");
        
        // 订阅消息
        await subscriber.SubscribeAsync("demo.subject", (subject, message) => {
            Console.WriteLine($"收到消息 from {subject}: {Encoding.UTF8.GetString(message)}");
        });
        
        // 发布消息
        for (int i = 0; i < 5; i++)
        {
            var message = Encoding.UTF8.GetBytes($"Hello NATS! Message {i}");
            await publisher.PublishAsync("demo.subject", message);
            Console.WriteLine($"发布消息: Message {i}");
            await Task.Delay(500);
        }
        
        // 等待一段时间，确保消息被处理
        await Task.Delay(2000);
        
        Console.WriteLine("示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NATS 选项
        builder.Configure<NatsOptions>(options => {
            options.Url = "nats://localhost:4222";
            options.ConnectionTimeout = 5000;
            options.MaxPayloadSize = 1048576; // 1MB
            options.ReconnectWait = 2000;
        });
        
        // 注册服务
        builder.AddSingleton<INatsService, NatsService>();
        builder.AddSingleton<INatsPublisher, NatsPublisher>();
        builder.AddSingleton<INatsSubscriber, NatsSubscriber>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 2. 高级配置示例

`csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NATS 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 NATS 设置
        builder.Configure<NatsOptions>(options => {
            options.Url = "nats://localhost:4222";
            options.ConnectionTimeout = 10000;
            options.MaxPayloadSize = 2097152; // 2MB
            options.ReconnectWait = 3000;
        });
        
        // 注册服务
        builder.AddSingleton<INatsService, NatsService>();
        builder.AddSingleton<INatsPublisher, NatsPublisher>();
        builder.AddSingleton<INatsSubscriber, NatsSubscriber>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<NatsOptions>>().Value;
        Console.WriteLine($"配置: Url={settings.Url}, Timeout={settings.ConnectionTimeout}ms");
        Console.WriteLine($"最大消息大小: {settings.MaxPayloadSize} bytes, 重连等待: {settings.ReconnectWait}ms");
        
        // 使用服务
        var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
        var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();
        
        // 订阅消息
        await subscriber.SubscribeAsync("advanced.subject", (subject, message) => {
            Console.WriteLine($"收到高级消息: {Encoding.UTF8.GetString(message)}");
        });
        
        // 发布消息
        var message = Encoding.UTF8.GetBytes("这是一条使用高级配置的消息");
        await publisher.PublishAsync("advanced.subject", message);
        
        // 等待消息处理
        await Task.Delay(1000);
        
        Console.WriteLine("高级配置示例完成");
    }
}
`

### 3. 性能优化示例

`csharp
using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NATS 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
        var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();
        
        // 订阅消息（用于验证消息是否被接收）
        int receivedCount = 0;
        await subscriber.SubscribeAsync("performance.subject", (subject, message) => {
            receivedCount++;
        });
        
        // 性能测试
        const int iterations = 10000;
        var stopwatch = Stopwatch.StartNew();
        
        // 使用内存池减少内存分配
        var pool = ArrayPool<byte>.Shared;
        
        for (int i = 0; i < iterations; i++)
        {
            // 从内存池获取缓冲区
            var buffer = pool.Rent(100);
            try
            {
                // 写入消息内容
                var message = Encoding.UTF8.GetBytes($"Performance test message {i}", 0, $"Performance test message {i}".Length, buffer, 0);
                
                // 使用 Span 优化
                var messageSpan = new Span<byte>(buffer, 0, message);
                var messageArray = messageSpan.ToArray();
                
                // 发布消息
                await publisher.PublishAsync("performance.subject", messageArray);
            }
            finally
            {
                // 归还缓冲区到内存池
                pool.Return(buffer);
            }
        }
        
        stopwatch.Stop();
        
        // 等待所有消息被处理
        await Task.Delay(2000);
        
        Console.WriteLine($"执行 {iterations} 次发布操作的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次操作: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        Console.WriteLine($"收到的消息数量: {receivedCount}");
        
        Console.WriteLine("性能优化示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NATS 选项
        builder.Configure<NatsOptions>(options => {
            options.Url = "nats://localhost:4222";
            options.ConnectionTimeout = 5000;
            options.MaxPayloadSize = 1048576; // 1MB
            options.ReconnectWait = 2000;
        });
        
        // 注册服务
        builder.AddSingleton<INatsService, NatsService>();
        builder.AddSingleton<INatsPublisher, NatsPublisher>();
        builder.AddSingleton<INatsSubscriber, NatsSubscriber>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 4. 错误处理示例

`csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NATS 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var natsService = serviceProvider.GetRequiredService<INatsService>();
        var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
        
        try
        {
            // 检查连接状态
            var isConnected = await natsService.IsConnectedAsync();
            Console.WriteLine($"NATS 连接状态: {isConnected}");
            
            // 尝试发布消息
            var message = Encoding.UTF8.GetBytes("测试消息");
            await publisher.PublishAsync("test.subject", message);
            Console.WriteLine("消息发布成功");
        }
        catch (NATSException ex)
        {
            Console.WriteLine($"NATS 错误: {ex.Message}");
            // 处理 NATS 特定错误
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
            // 处理超时错误
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
            // 处理操作错误
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
            // 处理通用错误
        }
        
        Console.WriteLine("错误处理示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NATS 选项（故意使用错误的 URL 来测试错误处理）
        builder.Configure<NatsOptions>(options => {
            options.Url = "nats://localhost:9999"; // 错误的端口
            options.ConnectionTimeout = 2000;
        });
        
        // 注册服务
        builder.AddSingleton<INatsService, NatsService>();
        builder.AddSingleton<INatsPublisher, NatsPublisher>();
        builder.AddSingleton<INatsSubscriber, NatsSubscriber>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 5. 请求-响应模式示例

`csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NATS 请求-响应模式示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var natsService = serviceProvider.GetRequiredService<INatsService>();
        
        // 启动响应器
        StartResponder(natsService.GetConnection());
        
        // 发送请求
        var response = await SendRequest(natsService.GetConnection(), "request.subject", "Hello, what's the time?");
        Console.WriteLine($"收到响应: {response}");
        
        // 等待一段时间
        await Task.Delay(2000);
        
        Console.WriteLine("请求-响应模式示例完成");
    }
    
    private static void StartResponder(IConnection connection)
    {
        // 创建响应器
        var responder = connection.CreateResponder("request.subject");
        
        // 启动响应线程
        Task.Run(async () => {
            while (true)
            {
                try
                {
                    // 等待请求
                    var request = responder.NextMessage(1000);
                    if (request != null)
                    {
                        Console.WriteLine($"收到请求: {Encoding.UTF8.GetString(request.Data)}");
                        
                        // 生成响应
                        var response = Encoding.UTF8.GetBytes($"当前时间: {DateTime.Now}");
                        
                        // 发送响应
                        connection.Publish(request.ReplyTo, response);
                        Console.WriteLine("发送响应");
                    }
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"响应器错误: {ex.Message}");
                }
            }
        });
    }
    
    private static async Task<string> SendRequest(IConnection connection, string subject, string message)
    {
        // 发送请求并等待响应
        var requestData = Encoding.UTF8.GetBytes(message);
        var response = connection.Request(subject, requestData, 5000);
        
        return Encoding.UTF8.GetString(response.Data);
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NATS 选项
        builder.Configure<NatsOptions>(options => {
            options.Url = "nats://localhost:4222";
            options.ConnectionTimeout = 5000;
        });
        
        // 注册服务
        builder.AddSingleton<INatsService, NatsService>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 6. 批量消息处理示例

`csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NATS 批量消息处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
        var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();
        
        // 用于存储接收到的消息
        var receivedMessages = new List<string>();
        
        // 订阅消息
        await subscriber.SubscribeAsync("batch.subject", (subject, message) => {
            lock (receivedMessages)
            {
                receivedMessages.Add(Encoding.UTF8.GetString(message));
                
                // 当收到 10 条消息时批量处理
                if (receivedMessages.Count >= 10)
                {
                    ProcessBatchMessages(new List<string>(receivedMessages));
                    receivedMessages.Clear();
                }
            }
        });
        
        // 批量发布消息
        Console.WriteLine("开始批量发布消息...");
        for (int i = 0; i < 25; i++)
        {
            var message = Encoding.UTF8.GetBytes($"Batch message {i}");
            await publisher.PublishAsync("batch.subject", message);
            
            // 每发布 5 条消息暂停一下
            if ((i + 1) % 5 == 0)
            {
                Console.WriteLine($"已发布 {i + 1} 条消息");
                await Task.Delay(500);
            }
        }
        
        // 等待所有消息被处理
        await Task.Delay(3000);
        
        // 处理剩余的消息
        if (receivedMessages.Count > 0)
        {
            ProcessBatchMessages(receivedMessages);
        }
        
        Console.WriteLine("批量消息处理示例完成");
    }
    
    private static void ProcessBatchMessages(List<string> messages)
    {
        Console.WriteLine($"\n批量处理 {messages.Count} 条消息:");
        foreach (var message in messages)
        {
            Console.WriteLine($"  - {message}");
        }
        Console.WriteLine("批量处理完成\n");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NATS 选项
        builder.Configure<NatsOptions>(options => {
            options.Url = "nats://localhost:4222";
            options.ConnectionTimeout = 5000;
        });
        
        // 注册服务
        builder.AddSingleton<INatsService, NatsService>();
        builder.AddSingleton<INatsPublisher, NatsPublisher>();
        builder.AddSingleton<INatsSubscriber, NatsSubscriber>();
        
        return builder.BuildServiceProvider();
    }
}
`

## 总结

以上示例展示了 NATS 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用请求-响应模式
6. 实现批量消息处理

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 性能优化技巧

1. **使用内存池**：通过 ArrayPool<byte> 减少内存分配和 GC 压力
2. **异步编程**：使用异步 API 避免阻塞，提高系统吞吐量
3. **批处理**：对于大量消息，使用批处理提高效率
4. **通道使用**：使用 Channel 进行消息缓冲，提高并发处理能力
5. **Span 优化**：使用 Span<byte> 减少内存拷贝，提高性能

### 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期
2. **错误处理**：正确处理异常情况，确保系统稳定性
3. **连接管理**：合理管理 NATS 连接，避免连接泄漏
4. **日志记录**：添加适当的日志记录，便于故障排查
5. **监控**：监控系统性能和消息处理情况

通过这些示例和最佳实践，您可以充分利用 NATS 技能的强大功能，构建高性能、可靠的消息系统。
