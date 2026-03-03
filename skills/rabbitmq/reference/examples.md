# RabbitMQ 技能使用示例

## 快速开始

### 1. 基本设置与服务注册

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 技能基本设置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 RabbitMQ 设置
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.VirtualHost = "/";
            options.EnableConnectionPooling = true;
            options.MaxConnections = 10;
        });
        
        // 注册服务
        builder.AddRabbitMqServices();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取 RabbitMQ 服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        Console.WriteLine("RabbitMQ 服务注册成功！");
        Console.WriteLine("准备开始消息操作...");
        
        // 后续操作...
    }
}
```

## 消息发布示例

### 2. 基本消息发布

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 基本消息发布示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        // 定义消息内容
        var message = new {
            Id = Guid.NewGuid(),
            Content = "Hello, RabbitMQ!",
            Timestamp = DateTime.UtcNow
        };
        
        try
        {
            // 发布消息
            var messageId = await rabbitMqService.PublishAsync(
                exchange: "amq.topic",
                routingKey: "test.message",
                message: message
            );
            
            Console.WriteLine($"消息发布成功！消息ID: {messageId}");
            Console.WriteLine($"消息内容: {message.Content}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"消息发布失败: {ex.Message}");
        }
    }
}
```

### 3. 持久化消息发布

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 持久化消息发布示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        // 定义消息内容
        var orderMessage = new {
            OrderId = Guid.NewGuid(),
            CustomerId = "C12345",
            Amount = 99.99,
            Status = "Created",
            Timestamp = DateTime.UtcNow
        };
        
        try
        {
            // 发布持久化消息
            var messageId = await rabbitMqService.PublishAsync(
                exchange: "order.exchange",
                routingKey: "order.created",
                message: orderMessage,
                persistent: true // 启用持久化
            );
            
            Console.WriteLine($"持久化消息发布成功！消息ID: {messageId}");
            Console.WriteLine($"订单ID: {orderMessage.OrderId}");
            Console.WriteLine($"金额: {orderMessage.Amount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"消息发布失败: {ex.Message}");
        }
    }
}
```

## 消息订阅示例

### 4. 基本消息订阅

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 基本消息订阅示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 订阅消息
            var subscriptionId = await rabbitMqService.SubscribeAsync<dynamic>(
                queueName: "test.queue",
                exchange: "amq.topic",
                routingKey: "test.#",
                onMessageReceived: async (message, headers, deliveryTag) => {
                    Console.WriteLine($"收到消息: {message.Content}");
                    Console.WriteLine($"消息ID: {headers["message-id"]}");
                    Console.WriteLine($"时间戳: {message.Timestamp}");
                    
                    // 处理完消息后确认
                    await Task.CompletedTask;
                    return true; // 返回 true 表示确认消息
                }
            );
            
            Console.WriteLine($"订阅成功！订阅ID: {subscriptionId}");
            Console.WriteLine("等待接收消息...");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            
            // 取消订阅
            await rabbitMqService.UnsubscribeAsync(subscriptionId);
            Console.WriteLine("订阅已取消");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"订阅失败: {ex.Message}");
        }
    }
}
```

### 5. 工作队列订阅

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 工作队列订阅示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 订阅工作队列消息（多个消费者会轮询接收消息）
            var subscriptionId = await rabbitMqService.SubscribeAsync<dynamic>(
                queueName: "work.queue",
                exchange: "", // 空交换机表示默认交换机
                routingKey: "work.queue",
                onMessageReceived: async (message, headers, deliveryTag) => {
                    Console.WriteLine($"[工作线程] 收到任务: {message.TaskId}");
                    Console.WriteLine($"[工作线程] 任务内容: {message.Content}");
                    
                    // 模拟处理时间
                    await Task.Delay(1000);
                    
                    Console.WriteLine($"[工作线程] 任务处理完成: {message.TaskId}");
                    return true;
                },
                prefetchCount: 1 // 每次只处理一条消息，确保公平分配
            );
            
            Console.WriteLine($"工作队列订阅成功！订阅ID: {subscriptionId}");
            Console.WriteLine("等待接收任务...");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            
            // 取消订阅
            await rabbitMqService.UnsubscribeAsync(subscriptionId);
            Console.WriteLine("订阅已取消");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"订阅失败: {ex.Message}");
        }
    }
}
```

## RPC 模式示例

### 6. RPC 服务端

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ RPC 服务端示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 注册 RPC 处理程序
            var handlerId = await rabbitMqService.RegisterRpcHandler<CalculatorRequest, CalculatorResponse>(
                queueName: "rpc.calculator",
                handler: async (request) => {
                    Console.WriteLine($"收到计算请求: {request.Operation} {request.Number1} 和 {request.Number2}");
                    
                    var response = new CalculatorResponse {
                        RequestId = request.RequestId,
                        Success = true
                    };
                    
                    // 执行计算
                    switch (request.Operation.ToLower())
                    {
                        case "add":
                            response.Result = request.Number1 + request.Number2;
                            break;
                        case "subtract":
                            response.Result = request.Number1 - request.Number2;
                            break;
                        case "multiply":
                            response.Result = request.Number1 * request.Number2;
                            break;
                        case "divide":
                            if (request.Number2 == 0)
                            {
                                response.Success = false;
                                response.ErrorMessage = "除数不能为零";
                            }
                            else
                            {
                                response.Result = request.Number1 / request.Number2;
                            }
                            break;
                        default:
                            response.Success = false;
                            response.ErrorMessage = "不支持的操作";
                            break;
                    }
                    
                    Console.WriteLine($"计算结果: {response.Result}");
                    return response;
                }
            );
            
            Console.WriteLine($"RPC 处理程序注册成功！处理程序ID: {handlerId}");
            Console.WriteLine("等待接收 RPC 请求...");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            
            // 取消注册
            await rabbitMqService.UnregisterRpcHandler(handlerId);
            Console.WriteLine("RPC 处理程序已取消注册");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RPC 处理程序注册失败: {ex.Message}");
        }
    }
}

// 计算请求模型
public class CalculatorRequest
{
    public string RequestId { get; set; }
    public string Operation { get; set; }
    public double Number1 { get; set; }
    public double Number2 { get; set; }
}

// 计算响应模型
public class CalculatorResponse
{
    public string RequestId { get; set; }
    public bool Success { get; set; }
    public double Result { get; set; }
    public string ErrorMessage { get; set; }
}
```

### 7. RPC 客户端

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ RPC 客户端示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 构建请求
            var request = new CalculatorRequest {
                RequestId = Guid.NewGuid().ToString(),
                Operation = "add",
                Number1 = 10,
                Number2 = 5
            };
            
            Console.WriteLine($"发送计算请求: {request.Operation} {request.Number1} 和 {request.Number2}");
            
            // 发送 RPC 请求并等待响应
            var response = await rabbitMqService.RpcCallAsync<CalculatorRequest, CalculatorResponse>(
                queueName: "rpc.calculator",
                request: request,
                timeout: TimeSpan.FromSeconds(30)
            );
            
            if (response.Success)
            {
                Console.WriteLine($"RPC 调用成功！结果: {response.Result}");
            }
            else
            {
                Console.WriteLine($"RPC 调用失败: {response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RPC 调用失败: {ex.Message}");
        }
    }
}

// 计算请求模型
public class CalculatorRequest
{
    public string RequestId { get; set; }
    public string Operation { get; set; }
    public double Number1 { get; set; }
    public double Number2 { get; set; }
}

// 计算响应模型
public class CalculatorResponse
{
    public string RequestId { get; set; }
    public bool Success { get; set; }
    public double Result { get; set; }
    public string ErrorMessage { get; set; }
}
```

## 高级功能示例

### 8. 死信队列示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 死信队列示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 配置死信队列参数
            var queueArgs = new Dictionary<string, object> {
                { "x-dead-letter-exchange", "dead.letter.exchange" },
                { "x-dead-letter-routing-key", "dead.letter" },
                { "x-message-ttl", 5000 } // 消息过期时间 5 秒
            };
            
            // 创建主队列（带死信配置）
            await rabbitMqService.DeclareQueueAsync(
                queueName: "main.queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs
            );
            
            // 创建死信交换机
            await rabbitMqService.DeclareExchangeAsync(
                exchangeName: "dead.letter.exchange",
                exchangeType: "direct",
                durable: true,
                autoDelete: false
            );
            
            // 创建死信队列
            await rabbitMqService.DeclareQueueAsync(
                queueName: "dead.letter.queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            
            // 绑定死信队列
            await rabbitMqService.BindQueueAsync(
                queueName: "dead.letter.queue",
                exchangeName: "dead.letter.exchange",
                routingKey: "dead.letter"
            );
            
            // 订阅死信队列
            await rabbitMqService.SubscribeAsync<dynamic>(
                queueName: "dead.letter.queue",
                exchange: "dead.letter.exchange",
                routingKey: "dead.letter",
                onMessageReceived: async (message, headers, deliveryTag) => {
                    Console.WriteLine($"收到死信消息: {message.Content}");
                    Console.WriteLine($"原队列: {headers["x-death"][0]["queue"]}");
                    Console.WriteLine($"原路由键: {headers["x-death"][0]["routing-keys"][0]}");
                    Console.WriteLine($"过期时间: {headers["x-death"][0]["original-expiration"]}");
                    
                    await Task.CompletedTask;
                    return true;
                }
            );
            
            // 发布消息到主队列
            await rabbitMqService.PublishAsync(
                exchange: "",
                routingKey: "main.queue",
                message: new { Content = "测试死信队列消息" },
                persistent: true
            );
            
            Console.WriteLine("消息已发布到主队列，5秒后将进入死信队列...");
            Console.WriteLine("等待接收死信消息...");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
        }
    }
}
```

### 9. 延迟队列示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 延迟队列示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 配置延迟队列参数
            var delayQueueArgs = new Dictionary<string, object> {
                { "x-dead-letter-exchange", "delayed.exchange" },
                { "x-dead-letter-routing-key", "delayed.message" },
                { "x-message-ttl", 0 } // 延迟由消息本身的 expiration 决定
            };
            
            // 创建延迟交换机
            await rabbitMqService.DeclareExchangeAsync(
                exchangeName: "delay.exchange",
                exchangeType: "direct",
                durable: true,
                autoDelete: false
            );
            
            // 创建延迟队列
            await rabbitMqService.DeclareQueueAsync(
                queueName: "delay.queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: delayQueueArgs
            );
            
            // 绑定延迟队列
            await rabbitMqService.BindQueueAsync(
                queueName: "delay.queue",
                exchangeName: "delay.exchange",
                routingKey: "delay.message"
            );
            
            // 创建目标交换机
            await rabbitMqService.DeclareExchangeAsync(
                exchangeName: "delayed.exchange",
                exchangeType: "direct",
                durable: true,
                autoDelete: false
            );
            
            // 创建目标队列
            await rabbitMqService.DeclareQueueAsync(
                queueName: "delayed.queue",
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            
            // 绑定目标队列
            await rabbitMqService.BindQueueAsync(
                queueName: "delayed.queue",
                exchangeName: "delayed.exchange",
                routingKey: "delayed.message"
            );
            
            // 订阅目标队列
            await rabbitMqService.SubscribeAsync<dynamic>(
                queueName: "delayed.queue",
                exchange: "delayed.exchange",
                routingKey: "delayed.message",
                onMessageReceived: async (message, headers, deliveryTag) => {
                    Console.WriteLine($"收到延迟消息: {message.Content}");
                    Console.WriteLine($"发送时间: {message.SendTime}");
                    Console.WriteLine($"接收时间: {DateTime.UtcNow}");
                    
                    await Task.CompletedTask;
                    return true;
                }
            );
            
            // 发送延迟消息（延迟 10 秒）
            var headers = new Dictionary<string, object> {
                { "expiration", "10000" } // 延迟 10 秒
            };
            
            await rabbitMqService.PublishAsync(
                exchange: "delay.exchange",
                routingKey: "delay.message",
                message: new {
                    Content = "测试延迟队列消息",
                    SendTime = DateTime.UtcNow
                },
                headers: headers,
                persistent: true
            );
            
            Console.WriteLine("延迟消息已发送，10秒后将被处理...");
            Console.WriteLine("等待接收延迟消息...");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
        }
    }
}
```

## 批量操作示例

### 10. 批量消息发布

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 批量消息发布示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.EnableBatchProcessing = true;
            options.BatchSize = 100;
            options.BatchTimeout = 100;
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 准备批量消息
            var messages = new List<object>();
            for (int i = 1; i <= 1000; i++)
            {
                messages.Add(new {
                    Id = i,
                    Content = $"批量消息 #{i}",
                    Timestamp = DateTime.UtcNow
                });
            }
            
            Console.WriteLine($"准备发布 {messages.Count} 条消息...");
            
            // 批量发布消息
            var tasks = new List<Task<string>>();
            foreach (var message in messages)
            {
                tasks.Add(rabbitMqService.PublishAsync(
                    exchange: "amq.topic",
                    routingKey: "batch.message",
                    message: message
                ));
            }
            
            // 等待所有发布完成
            await Task.WhenAll(tasks);
            
            Console.WriteLine($"批量发布完成！成功发布 {tasks.Count} 条消息");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"批量发布失败: {ex.Message}");
        }
    }
}
```

## 性能优化示例

### 11. 连接池优化

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 连接池性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.EnableConnectionPooling = true;
            options.MaxConnections = 10;
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            const int messageCount = 1000;
            var stopwatch = Stopwatch.StartNew();
            
            Console.WriteLine($"测试发布 {messageCount} 条消息...");
            
            // 并行发布消息
            var tasks = Enumerable.Range(1, messageCount).Select(async i => {
                await rabbitMqService.PublishAsync(
                    exchange: "amq.topic",
                    routingKey: "performance.test",
                    message: new {
                        Id = i,
                        Content = $"性能测试消息 #{i}",
                        Timestamp = DateTime.UtcNow
                    }
                );
            });
            
            // 等待所有发布完成
            await Task.WhenAll(tasks);
            
            stopwatch.Stop();
            Console.WriteLine($"发布完成！");
            Console.WriteLine($"总耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每条消息: {stopwatch.Elapsed.TotalMilliseconds / messageCount:F3} ms");
            Console.WriteLine($"吞吐量: {messageCount / stopwatch.Elapsed.TotalSeconds:F2} 条/秒");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"性能测试失败: {ex.Message}");
        }
    }
}
```

## 错误处理示例

### 12. 重试机制示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("RabbitMQ 重试机制示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        builder.Configure<RabbitMqOptions>(options => {
            options.HostName = "localhost";
            options.Port = 5672;
            options.UserName = "guest";
            options.Password = "guest";
            options.MaxRetries = 3;
            options.RetryInterval = 500;
            options.EnableCircuitBreaker = true;
            options.CircuitBreakerFailureThreshold = 50;
            options.CircuitBreakerResetTimeout = 30;
        });
        builder.AddRabbitMqServices();
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取服务
        var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();
        
        try
        {
            // 发布消息（模拟网络故障重试）
            var messageId = await rabbitMqService.PublishAsync(
                exchange: "amq.topic",
                routingKey: "retry.test",
                message: new {
                    Content = "重试机制测试消息",
                    Timestamp = DateTime.UtcNow
                }
            );
            
            Console.WriteLine($"消息发布成功！消息ID: {messageId}");
            Console.WriteLine("重试机制工作正常");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"消息发布失败: {ex.Message}");
            Console.WriteLine("已达到最大重试次数");
        }
    }
}
```

## 总结

以上示例展示了 RabbitMQ 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速上手**：了解基本的服务注册和消息操作
2. **消息发布**：掌握基本消息发布和持久化消息发布
3. **消息订阅**：实现基本消息订阅和工作队列模式
4. **RPC 模式**：使用远程过程调用模式进行服务间通信
5. **高级功能**：实现死信队列和延迟队列
6. **批量操作**：优化批量消息处理
7. **性能优化**：配置连接池和其他性能参数
8. **错误处理**：使用重试机制和断路器模式

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

通过合理配置和使用这些功能，可以构建高性能、可靠的消息系统，满足各种业务场景的需求。
