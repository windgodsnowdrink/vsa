# MediatR - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

// 命令
public class CreateOrderCommand : IRequest<bool>
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}

// 命令处理器
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"创建订单: OrderId={request.OrderId}, CustomerId={request.CustomerId}, Amount={request.Amount}");
        // 模拟处理
        await Task.Delay(100);
        return true;
    }
}

// 事件
public class OrderCreatedEvent : INotification
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}

// 事件处理器
public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"处理订单创建事件: OrderId={notification.OrderId}");
        // 模拟处理
        await Task.Delay(50);
    }
}

// 查询
public class GetOrderQuery : IRequest<Order>
{
    public int OrderId { get; set; }
}

// 订单模型
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 查询处理器
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Order>
{
    public async Task<Order> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"查询订单: OrderId={request.OrderId}");
        // 模拟查询
        await Task.Delay(50);
        return new Order {
            Id = request.OrderId,
            CustomerId = 1,
            Amount = 100.0m,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MediatR 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // 发送命令
        var command = new CreateOrderCommand { OrderId = 1, CustomerId = 2, Amount = 100.0m };
        var commandResult = await mediator.Send(command);
        Console.WriteLine($"命令执行结果: {commandResult}");
        
        // 发布事件
        var @event = new OrderCreatedEvent { OrderId = 1, CustomerId = 2, Amount = 100.0m };
        await mediator.Publish(@event);
        Console.WriteLine("事件发布完成");
        
        // 发送查询
        var query = new GetOrderQuery { OrderId = 1 };
        var order = await mediator.Send(query);
        Console.WriteLine($"查询结果: OrderId={order.Id}, Amount={order.Amount}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;

// 自定义管道行为 - 日志行为
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("开始处理请求: {RequestType}", typeof(TRequest).Name);
        
        var response = await next();
        
        _logger.LogInformation("请求处理完成: {RequestType}", typeof(TRequest).Name);
        return response;
    }
}

// 自定义管道行为 - 验证行为
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"验证请求: {typeof(TRequest).Name}");
        // 这里可以添加验证逻辑
        return await next();
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MediatR 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 添加日志
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 MediatR 服务并添加自定义管道行为
        builder.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // 发送命令
        var command = new CreateOrderCommand { OrderId = 2, CustomerId = 3, Amount = 200.0m };
        var result = await mediator.Send(command);
        Console.WriteLine($"命令执行结果: {result}");
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MediatR 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // 性能测试
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        // 并行处理
        var tasks = new List<Task>();
        for (int i = 0; i < iterations; i++)
        {
            var orderId = i + 1;
            tasks.Add(mediator.Send(new CreateOrderCommand {
                OrderId = orderId,
                CustomerId = orderId % 100,
                Amount = 100.0m
            }));
        }
        
        await Task.WhenAll(tasks);
        
        stopwatch.Stop();
        Console.WriteLine($"执行 {iterations} 次命令的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次执行: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        return builder.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

// 异常命令
public class ExceptionCommand : IRequest<bool>
{
    public bool ThrowException { get; set; }
}

// 异常命令处理器
public class ExceptionCommandHandler : IRequestHandler<ExceptionCommand, bool>
{
    public async Task<bool> Handle(ExceptionCommand request, CancellationToken cancellationToken)
    {
        if (request.ThrowException)
        {
            throw new InvalidOperationException("模拟异常");
        }
        return true;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MediatR 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        try
        {
            // 正常执行
            var result1 = await mediator.Send(new ExceptionCommand { ThrowException = false });
            Console.WriteLine($"正常执行结果: {result1}");
            
            // 异常执行
            var result2 = await mediator.Send(new ExceptionCommand { ThrowException = true });
            Console.WriteLine($"异常执行结果: {result2}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作异常: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用异常: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        return builder.BuildServiceProvider();
    }
}
```

### 5. 分布式系统集成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MediatR 分布式系统集成示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // 发送命令到分布式系统
        var command = new CreateOrderCommand { OrderId = 3, CustomerId = 4, Amount = 300.0m };
        var result = await mediator.Send(command);
        Console.WriteLine($"分布式命令执行结果: {result}");
        
        // 发布事件到分布式系统
        var @event = new OrderCreatedEvent { OrderId = 3, CustomerId = 4, Amount = 300.0m };
        await mediator.Publish(@event);
        Console.WriteLine("分布式事件发布完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        // 这里可以添加分布式系统集成，如 MassTransit、Dapr 等
        return builder.BuildServiceProvider();
    }
}
```

### 6. AOT 编译示例

```csharp
//:sdk Microsoft.NET.Sdk
//:package Microsoft.Extensions.DependencyInjection@10.0.0
//:package Microsoft.Extensions.Logging@10.0.0
//:package MediatR@12.0.0
//:property LangVersion=preview
//:property TargetFramework=net11.0
//:property Nullable=enable
//:property ImplicitUsings=enable
//:property PublishAot=true
//:property IncludeNativeLibrariesForSelfExtract=true
//:property EnableCppCodeGen=true
//:property PublishSingleFile=true
//:property SelfContained=true
//:property RuntimeIdentifier=win-x64

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MediatR AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        
        // 发送命令
        var command = new CreateOrderCommand { OrderId = 4, CustomerId = 5, Amount = 400.0m };
        var result = await mediator.Send(command);
        Console.WriteLine($"命令执行结果: {result}");
        
        // 发布事件
        var @event = new OrderCreatedEvent { OrderId = 4, CustomerId = 5, Amount = 400.0m };
        await mediator.Publish(@event);
        Console.WriteLine("事件发布完成");
        
        Console.WriteLine("AOT 编译示例执行完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例展示了 MediatR 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速开始**：快速上手基本操作
2. **高级配置**：配置高级选项和自定义管道行为
3. **性能优化**：优化性能，支持并行处理
4. **错误处理**：正确处理异常情况
5. **分布式集成**：与分布式系统集成
6. **AOT 编译**：使用 AOT 编译提高性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
