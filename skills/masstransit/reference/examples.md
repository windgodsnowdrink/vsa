# MassTransit - 使用示例

## 快速开始

### 1. 基本消息发送示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
        
        Console.WriteLine("MassTransit 基本消息发送示例");
        Console.WriteLine("=" * 50);
        
        // 定义消息
        var orderCreated = new OrderCreated {
            OrderId = Guid.NewGuid(),
            CustomerId = 1,
            CustomerName = "张三",
            TotalAmount = 100.00,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("发送订单创建消息:");
        Console.WriteLine($"订单ID: {orderCreated.OrderId}");
        Console.WriteLine($"客户ID: {orderCreated.CustomerId}");
        Console.WriteLine($"客户姓名: {orderCreated.CustomerName}");
        Console.WriteLine($"总金额: {orderCreated.TotalAmount}");
        
        // 发布消息
        await publishEndpoint.Publish(orderCreated);
        Console.WriteLine("\n消息发布成功！");
        
        // 发送命令
        var sendEndpointProvider = serviceProvider.GetRequiredService<ISendEndpointProvider>();
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:process-order"));
        
        var processOrder = new ProcessOrder {
            OrderId = orderCreated.OrderId,
            ProcessingId = Guid.NewGuid(),
            RequestedAt = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("\n发送订单处理命令:");
        Console.WriteLine($"处理ID: {processOrder.ProcessingId}");
        await endpoint.Send(processOrder);
        Console.WriteLine("命令发送成功！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 MassTransit
        builder.AddMassTransit(x => {
            x.UsingRabbitMq((context, cfg) => {
                cfg.Host("localhost", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return builder.BuildServiceProvider();
    }
}

// 消息定义
public record OrderCreated {
    public Guid OrderId { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public record ProcessOrder {
    public Guid OrderId { get; init; }
    public Guid ProcessingId { get; init; }
    public DateTimeOffset RequestedAt { get; init; }
}
```

### 2. 消费者实现示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;

public class Program
{
    public static async Task Main()
    {
        var serviceProvider = BuildServiceProvider();
        var busControl = serviceProvider.GetRequiredService<IBusControl>();
        
        Console.WriteLine("MassTransit 消费者实现示例");
        Console.WriteLine("=" * 50);
        
        // 启动总线
        await busControl.StartAsync();
        Console.WriteLine("MassTransit 总线启动成功！");
        
        // 发布测试消息
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
        await publishEndpoint.Publish(new OrderCreated {
            OrderId = Guid.NewGuid(),
            CustomerId = 1,
            CustomerName = "测试客户",
            TotalAmount = 200.00,
            CreatedAt = DateTimeOffset.UtcNow
        });
        
        Console.WriteLine("测试消息已发布，消费者正在处理...");
        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        
        await busControl.StopAsync();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddLogging();
        
        // 配置 MassTransit
        builder.AddMassTransit(x => {
            // 注册消费者
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<ProcessOrderConsumer>();
            
            x.UsingRabbitMq((context, cfg) => {
                cfg.Host("localhost", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                // 配置消费者
                cfg.ReceiveEndpoint("order-created-queue", e => {
                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                });
                
                cfg.ReceiveEndpoint("process-order", e => {
                    e.ConfigureConsumer<ProcessOrderConsumer>(context);
                });
            });
        });
        
        return builder.BuildServiceProvider();
    }
}

// 订单创建消费者
public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    
    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("接收到订单创建消息: {OrderId}", context.Message.OrderId);
        
        Console.WriteLine("\n=== 订单创建消费者 ===");
        Console.WriteLine($"订单ID: {context.Message.OrderId}");
        Console.WriteLine($"客户: {context.Message.CustomerName}");
        Console.WriteLine($"金额: {context.Message.TotalAmount}");
        Console.WriteLine($"创建时间: {context.Message.CreatedAt}");
        
        // 模拟处理
        await Task.Delay(1000);
        Console.WriteLine("订单创建消息处理完成！");
        
        // 发布订单已接收事件
        await context.Publish(new OrderReceived {
            OrderId = context.Message.OrderId,
            ReceivedAt = DateTimeOffset.UtcNow
        });
    }
}

// 订单处理消费者
public class ProcessOrderConsumer : IConsumer<ProcessOrder>
{
    private readonly ILogger<ProcessOrderConsumer> _logger;
    
    public ProcessOrderConsumer(ILogger<ProcessOrderConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ProcessOrder> context)
    {
        _logger.LogInformation("接收到订单处理命令: {OrderId}", context.Message.OrderId);
        
        Console.WriteLine("\n=== 订单处理消费者 ===");
        Console.WriteLine($"订单ID: {context.Message.OrderId}");
        Console.WriteLine($"处理ID: {context.Message.ProcessingId}");
        
        // 模拟处理
        await Task.Delay(2000);
        Console.WriteLine("订单处理命令执行完成！");
        
        // 发送处理结果
        await context.RespondAsync(new OrderProcessed {
            OrderId = context.Message.OrderId,
            ProcessingId = context.Message.ProcessingId,
            ProcessedAt = DateTimeOffset.UtcNow,
            Status = "Completed"
        });
    }
}

// 消息定义
public record OrderReceived {
    public Guid OrderId { get; init; }
    public DateTimeOffset ReceivedAt { get; init; }
}

public record OrderProcessed {
    public Guid OrderId { get; init; }
    public Guid ProcessingId { get; init; }
    public DateTimeOffset ProcessedAt { get; init; }
    public string Status { get; init; }
}
```

### 3. Saga 状态机示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;

public class Program
{
    public static async Task Main()
    {
        var serviceProvider = BuildServiceProvider();
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
        
        Console.WriteLine("MassTransit Saga 状态机示例");
        Console.WriteLine("=" * 50);
        
        // 启动 Saga
        var orderId = Guid.NewGuid();
        var createOrder = new CreateOrder {
            OrderId = orderId,
            CustomerId = 1,
            CustomerName = "李四",
            TotalAmount = 150.00,
            RequestedAt = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("启动订单处理 Saga:");
        Console.WriteLine($"订单ID: {createOrder.OrderId}");
        await publishEndpoint.Publish(createOrder);
        Console.WriteLine("Saga 启动成功！");
        
        // 模拟支付完成
        await Task.Delay(2000);
        var paymentCompleted = new PaymentCompleted {
            OrderId = orderId,
            PaymentId = Guid.NewGuid(),
            Amount = createOrder.TotalAmount,
            CompletedAt = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("\n模拟支付完成:");
        Console.WriteLine($"支付ID: {paymentCompleted.PaymentId}");
        await publishEndpoint.Publish(paymentCompleted);
        
        // 模拟库存预留
        await Task.Delay(1000);
        var inventoryReserved = new InventoryReserved {
            OrderId = orderId,
            ReservationId = Guid.NewGuid(),
            ReservedAt = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("\n模拟库存预留:");
        Console.WriteLine($"预留ID: {inventoryReserved.ReservationId}");
        await publishEndpoint.Publish(inventoryReserved);
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddLogging();
        
        // 配置 MassTransit
        builder.AddMassTransit(x => {
            // 注册 Saga
            x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                .InMemoryRepository();
            
            x.UsingRabbitMq((context, cfg) => {
                cfg.Host("localhost", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                // 配置 Saga
                cfg.ReceiveEndpoint("order-saga", e => {
                    e.StateMachineSaga<OrderStateMachine, OrderState>(context);
                });
            });
        });
        
        return builder.BuildServiceProvider();
    }
}

// Saga 状态
public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? PaymentId { get; set; }
    public Guid? ReservationId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? PaymentCompletedAt { get; set; }
    public DateTimeOffset? InventoryReservedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}

// Saga 状态机
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        // 事件定义
        Event(() => CreateOrder, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentCompleted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCancelled, x => x.CorrelateById(context => context.Message.OrderId));
        
        // 状态定义
        State(() => OrderCreated);
        State(() => PaymentProcessing);
        State(() => PaymentConfirmed);
        State(() => InventoryProcessing);
        State(() => InventoryConfirmed);
        State(() => OrderCompleted);
        State(() => OrderCancelled);
        
        // 初始状态
        Initially(
            When(CreateOrder)
                .Then(context => {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.CustomerName = context.Message.CustomerName;
                    context.Saga.TotalAmount = context.Message.TotalAmount;
                    context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                    Console.WriteLine($"Saga: 订单创建 - {context.Message.OrderId}");
                })
                .TransitionTo(OrderCreated)
                .Publish(context => new OrderCreatedEvent {
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Message.CustomerId
                })
                .ThenAsync(async context => {
                    // 发送支付请求
                    var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:process-payment"));
                    await sendEndpoint.Send(new ProcessPayment {
                        OrderId = context.Message.OrderId,
                        Amount = context.Message.TotalAmount,
                        CustomerId = context.Message.CustomerId
                    });
                })
                .TransitionTo(PaymentProcessing));
        
        // 支付处理
        During(PaymentProcessing,
            When(PaymentCompleted)
                .Then(context => {
                    context.Saga.PaymentId = context.Message.PaymentId;
                    context.Saga.PaymentCompletedAt = context.Message.CompletedAt;
                    Console.WriteLine($"Saga: 支付完成 - {context.Message.OrderId}");
                })
                .TransitionTo(PaymentConfirmed)
                .Publish(context => new PaymentConfirmedEvent {
                    OrderId = context.Message.OrderId,
                    PaymentId = context.Message.PaymentId
                })
                .ThenAsync(async context => {
                    // 发送库存预留请求
                    var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:reserve-inventory"));
                    await sendEndpoint.Send(new ReserveInventory {
                        OrderId = context.Saga.OrderId
                    });
                })
                .TransitionTo(InventoryProcessing),
            When(OrderCancelled)
                .Then(context => {
                    Console.WriteLine($"Saga: 订单取消 - {context.Message.OrderId}");
                })
                .TransitionTo(OrderCancelled)
                .Finalize());
        
        // 库存处理
        During(InventoryProcessing,
            When(InventoryReserved)
                .Then(context => {
                    context.Saga.ReservationId = context.Message.ReservationId;
                    context.Saga.InventoryReservedAt = context.Message.ReservedAt;
                    Console.WriteLine($"Saga: 库存预留完成 - {context.Message.OrderId}");
                })
                .TransitionTo(InventoryConfirmed)
                .Publish(context => new InventoryConfirmedEvent {
                    OrderId = context.Message.OrderId,
                    ReservationId = context.Message.ReservationId
                })
                .Then(context => {
                    context.Saga.CompletedAt = DateTimeOffset.UtcNow;
                    Console.WriteLine($"Saga: 订单完成 - {context.Saga.OrderId}");
                })
                .TransitionTo(OrderCompleted)
                .Publish(context => new OrderCompletedEvent {
                    OrderId = context.Saga.OrderId
                })
                .Finalize(),
            When(OrderCancelled)
                .Then(context => {
                    Console.WriteLine($"Saga: 订单取消 - {context.Message.OrderId}");
                })
                .TransitionTo(OrderCancelled)
                .Finalize());
        
        SetCompletedWhenFinalized();
    }
    
    // 事件
    public Event<CreateOrder> CreateOrder { get; private set; }
    public Event<PaymentCompleted> PaymentCompleted { get; private set; }
    public Event<InventoryReserved> InventoryReserved { get; private set; }
    public Event<OrderCancelled> OrderCancelled { get; private set; }
    
    // 状态
    public State OrderCreated { get; private set; }
    public State PaymentProcessing { get; private set; }
    public State PaymentConfirmed { get; private set; }
    public State InventoryProcessing { get; private set; }
    public State InventoryConfirmed { get; private set; }
    public State OrderCompleted { get; private set; }
    public State OrderCancelled { get; private set; }
}

// 消息定义
public record CreateOrder {
    public Guid OrderId { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTimeOffset RequestedAt { get; init; }
}

public record PaymentCompleted {
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset CompletedAt { get; init; }
}

public record InventoryReserved {
    public Guid OrderId { get; init; }
    public Guid ReservationId { get; init; }
    public DateTimeOffset ReservedAt { get; init; }
}

public record OrderCancelled {
    public Guid OrderId { get; init; }
    public string Reason { get; init; }
    public DateTimeOffset CancelledAt { get; init; }
}

// 命令
public record ProcessPayment {
    public Guid OrderId { get; init; }
    public decimal Amount { get; init; }
    public int CustomerId { get; init; }
}

public record ReserveInventory {
    public Guid OrderId { get; init; }
}

// 事件
public record OrderCreatedEvent {
    public Guid OrderId { get; init; }
    public int CustomerId { get; init; }
}

public record PaymentConfirmedEvent {
    public Guid OrderId { get; init; }
    public Guid PaymentId { get; init; }
}

public record InventoryConfirmedEvent {
    public Guid OrderId { get; init; }
    public Guid ReservationId { get; init; }
}

public record OrderCompletedEvent {
    public Guid OrderId { get; init; }
}
```

### 4. 生产环境配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) => {
                // 配置 MassTransit
                services.AddMassTransit(x => {
                    // 注册消费者
                    x.AddConsumer<OrderConsumer>();
                    x.AddConsumer<PaymentConsumer>();
                    x.AddConsumer<InventoryConsumer>();
                    
                    // 注册 Saga
                    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                        .InMemoryRepository();
                    
                    // 使用 RabbitMQ
                    x.UsingRabbitMq((context, cfg) => {
                        // 配置主机
                        cfg.Host(hostContext.Configuration["RabbitMQ:Host"], h => {
                            h.Username(hostContext.Configuration["RabbitMQ:Username"]);
                            h.Password(hostContext.Configuration["RabbitMQ:Password"]);
                        });
                        
                        // 生产环境配置
                        cfg.UseMessageRetry(r => {
                            r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                        });
                        
                        cfg.UseCircuitBreaker(cb => {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                            cb.TripThreshold = 15;
                            cb.ActiveThreshold = 10;
                            cb.ResetInterval = TimeSpan.FromMinutes(5);
                        });
                        
                        cfg.UseRateLimit(100, TimeSpan.FromSeconds(1));
                        
                        // 配置端点
                        cfg.ReceiveEndpoint("order-service", e => {
                            e.PrefetchCount = 16;
                            e.ConcurrentMessageLimit = 10;
                            e.ConfigureConsumer<OrderConsumer>(context);
                        });
                        
                        cfg.ReceiveEndpoint("payment-service", e => {
                            e.PrefetchCount = 8;
                            e.ConcurrentMessageLimit = 5;
                            e.ConfigureConsumer<PaymentConsumer>(context);
                        });
                        
                        cfg.ReceiveEndpoint("inventory-service", e => {
                            e.PrefetchCount = 12;
                            e.ConcurrentMessageLimit = 8;
                            e.ConfigureConsumer<InventoryConsumer>(context);
                        });
                        
                        cfg.ReceiveEndpoint("order-saga", e => {
                            e.StateMachineSaga<OrderStateMachine, OrderState>(context);
                        });
                    });
                });
                
                // 添加主机服务
                services.AddMassTransitHostedService();
                services.AddHostedService<Worker>();
            });
}

// 工作服务
public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("生产环境服务启动成功！");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

// 消费者
public class OrderConsumer : IConsumer<OrderCreated> {
    public Task Consume(ConsumeContext<OrderCreated> context) => Task.CompletedTask;
}

public class PaymentConsumer : IConsumer<PaymentCompleted> {
    public Task Consume(ConsumeContext<PaymentCompleted> context) => Task.CompletedTask;
}

public class InventoryConsumer : IConsumer<InventoryReserved> {
    public Task Consume(ConsumeContext<InventoryReserved> context) => Task.CompletedTask;
}
```

### 5. AOT 编译示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package MassTransit@8.0.0 
#:package MassTransit.RabbitMQ@8.0.0 
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
using Microsoft.Extensions.DependencyInjection;
using MassTransit;

public class AotExample
{
    public static async Task Main()
    {
        Console.WriteLine("MassTransit AOT 编译示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("使用 AOT 编译的单文件可执行程序");
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
        
        // 测试消息发布
        var message = new TestMessage {
            MessageId = Guid.NewGuid(),
            Content = "AOT 编译测试消息",
            Timestamp = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("发布测试消息:");
        Console.WriteLine($"消息ID: {message.MessageId}");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await publishEndpoint.Publish(message);
        stopwatch.Stop();
        
        Console.WriteLine($"消息发布成功！");
        Console.WriteLine($"发布时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 测试命令发送
        var sendEndpointProvider = serviceProvider.GetRequiredService<ISendEndpointProvider>();
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:test-command"));
        
        var command = new TestCommand {
            CommandId = Guid.NewGuid(),
            Action = "TestAction",
            RequestedAt = DateTimeOffset.UtcNow
        };
        
        Console.WriteLine("\n发送测试命令:");
        Console.WriteLine($"命令ID: {command.CommandId}");
        
        stopwatch.Restart();
        await endpoint.Send(command);
        stopwatch.Stop();
        
        Console.WriteLine($"命令发送成功！");
        Console.WriteLine($"发送时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        Console.WriteLine("\nAOT 编译示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 MassTransit
        builder.AddMassTransit(x => {
            x.UsingRabbitMq((context, cfg) => {
                cfg.Host("localhost", h => {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return builder.BuildServiceProvider();
    }
}

// 消息定义
public record TestMessage {
    public Guid MessageId { get; init; }
    public string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public record TestCommand {
    public Guid CommandId { get; init; }
    public string Action { get; init; }
    public DateTimeOffset RequestedAt { get; init; }
}
```

## 总结

以上示例展示了 MassTransit 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本的消息发送和接收操作
2. 实现消费者处理消息
3. 使用 Saga 状态机处理分布式事务
4. 配置生产环境的高可用设置
5. 使用 AOT 编译提升性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 支持的功能

- **消息发布/订阅**: 支持事件驱动架构
- **命令发送**: 支持请求/响应模式
- **Saga 状态机**: 支持分布式事务和补偿
- **消费者**: 支持多种消息处理模式
- **重试策略**: 支持多种重试机制
- **断路器**: 支持服务保护
- **速率限制**: 支持流量控制
- **多传输支持**: 支持 RabbitMQ、Azure Service Bus、Kafka 等
- **AOT 编译**: 支持 AOT 编译，提升启动速度和运行性能

### 性能优化特点

- **异步编程**: 全异步 API，避免线程阻塞
- **连接池**: 优化消息代理连接
- **内存优化**: 使用对象池和 Span 减少内存分配
- **批量处理**: 支持消息批处理
- **压缩**: 支持消息压缩
- **并行处理**: 合理配置并发度提高处理能力
