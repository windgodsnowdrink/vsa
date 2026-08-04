# Wolverine 技能使用示例

## 基本使用示例

### 1. 创建消息总线并发送命令

```csharp
using WolverineSkill;

// 创建消息总线
var messageBus = new MessageBus();

// 定义命令
public class CreateOrderCommand : Command
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}

// 定义事件
public class OrderCreatedEvent : Event
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 实现命令处理器
public class CreateOrderHandler : IHandler<CreateOrderCommand>
{
    private readonly IMessageBus _messageBus;

    public CreateOrderHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        // 处理命令逻辑
        Console.WriteLine($"创建订单: {command.OrderId}, 客户: {command.CustomerId}, 金额: {command.Amount}");

        // 发布事件
        var @event = new OrderCreatedEvent
        {
            OrderId = command.OrderId,
            CustomerId = command.CustomerId,
            Amount = command.Amount,
            CreatedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);
    }
}

// 实现事件处理器
public class OrderCreatedHandler : IHandler<OrderCreatedEvent>
{
    public Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        // 处理事件逻辑
        Console.WriteLine($"订单已创建: {@event.OrderId}, 客户: {@event.CustomerId}, 金额: {@event.Amount}, 时间: {@event.CreatedAt}");
        return Task.CompletedTask;
    }
}

// 注册处理器
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();
messageBus.RegisterHandler<OrderCreatedEvent, OrderCreatedHandler>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 等待所有消息处理完成
await Task.Delay(1000);
```

### 2. 使用依赖注入

```csharp
using Microsoft.Extensions.DependencyInjection;
using WolverineSkill;

// 注册服务
var services = new ServiceCollection();
services.AddWolverineSkill();

// 注册处理器
services.AddTransient<CreateOrderHandler>();
services.AddTransient<OrderCreatedHandler>();

var serviceProvider = services.BuildServiceProvider();

// 获取消息总线
var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 等待所有消息处理完成
await Task.Delay(1000);
```

### 3. 消息路由示例

```csharp
using WolverineSkill;

// 创建消息总线
var messageBus = new MessageBus();

// 定义订单事件基类
public class OrderEvent : Event
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 定义具体订单事件
public class OrderCreatedEvent : OrderEvent {}
public class OrderUpdatedEvent : OrderEvent {}
public class OrderCanceledEvent : OrderEvent {}

// 实现通用订单处理器
public class OrderHandler : IHandler<OrderEvent>
{
    public Task HandleAsync(OrderEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理订单事件: {@event.GetType().Name}, 订单: {@event.OrderId}, 客户: {@event.CustomerId}, 金额: {@event.Amount}");
        return Task.CompletedTask;
    }
}

// 实现高级客户订单处理器
public class PremiumCustomerOrderHandler : IHandler<OrderEvent>
{
    public Task HandleAsync(OrderEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理高级客户订单事件: {@event.GetType().Name}, 订单: {@event.OrderId}, 客户: {@event.CustomerId}, 金额: {@event.Amount}");
        return Task.CompletedTask;
    }
}

// 注册基于类型的路由
messageBus.RegisterHandler<OrderCreatedEvent, OrderHandler>();
messageBus.RegisterHandler<OrderUpdatedEvent, OrderHandler>();
messageBus.RegisterHandler<OrderCanceledEvent, OrderHandler>();

// 注册基于属性的路由（处理金额大于1000的订单）
messageBus.RegisterHandler<OrderEvent, PremiumCustomerOrderHandler>(message => message is OrderEvent e && e.Amount > 1000.0m);

// 发布事件
var @event1 = new OrderCreatedEvent { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 500.0m, CreatedAt = DateTime.UtcNow };
var @event2 = new OrderCreatedEvent { OrderId = Guid.NewGuid(), CustomerId = 456, Amount = 1500.0m, CreatedAt = DateTime.UtcNow };

await messageBus.PublishAsync(@event1);
await messageBus.PublishAsync(@event2);

// 等待所有消息处理完成
await Task.Delay(1000);
```

## 高级使用示例

### 1. 自定义消息序列化器

```csharp
using WolverineSkill;

// 实现自定义消息序列化器
public class CustomMessageSerializer : IMessageSerializer
{
    public byte[] Serialize(object message)
    {
        // 实现序列化逻辑
        Console.WriteLine($"序列化消息: {message.GetType().Name}");
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);
    }

    public object Deserialize(byte[] data, Type messageType)
    {
        // 实现反序列化逻辑
        Console.WriteLine($"反序列化消息: {messageType.Name}");
        return System.Text.Json.JsonSerializer.Deserialize(data, messageType);
    }

    public T Deserialize<T>(byte[] data)
    {
        // 实现泛型反序列化逻辑
        Console.WriteLine($"反序列化消息: {typeof(T).Name}");
        return System.Text.Json.JsonSerializer.Deserialize<T>(data);
    }
}

// 创建消息总线并使用自定义序列化器
var messageBus = new MessageBus(new CustomMessageSerializer());

// 注册处理器
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 等待所有消息处理完成
await Task.Delay(1000);
```

### 2. 自定义消息持久化

```csharp
using WolverineSkill;

// 实现自定义消息持久化
public class FileMessagePersistence : IMessagePersistence
{
    private readonly string _filePath;

    public FileMessagePersistence(string filePath)
    {
        _filePath = filePath;
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
    }

    public Task PersistAsync(object message, CancellationToken cancellationToken = default)
    {
        // 实现消息持久化逻辑
        var messageJson = System.Text.Json.JsonSerializer.Serialize(message);
        File.AppendAllText(_filePath, messageJson + Environment.NewLine);
        Console.WriteLine($"持久化消息: {message.GetType().Name}");
        return Task.CompletedTask;
    }

    public Task<IEnumerable<object>> LoadAsync(CancellationToken cancellationToken = default)
    {
        // 实现消息加载逻辑
        if (!File.Exists(_filePath))
        {
            return Task.FromResult(Enumerable.Empty<object>());
        }

        var messages = new List<object>();
        var lines = File.ReadAllLines(_filePath);
        foreach (var line in lines)
        {
            // 注意：实际实现中需要存储消息类型信息
            try
            {
                var message = System.Text.Json.JsonSerializer.Deserialize<CreateOrderCommand>(line);
                if (message != null)
                {
                    messages.Add(message);
                }
            }
            catch { }
        }

        Console.WriteLine($"加载消息: {messages.Count} 条");
        return Task.FromResult<IEnumerable<object>>(messages);
    }

    public Task DeleteAsync(object message, CancellationToken cancellationToken = default)
    {
        // 实现消息删除逻辑
        Console.WriteLine($"删除消息: {message.GetType().Name}");
        return Task.CompletedTask;
    }
}

// 创建消息总线并使用自定义持久化
var persistence = new FileMessagePersistence(Path.Combine(Environment.CurrentDirectory, "messages.txt"));
var messageBus = new MessageBus(persistence: persistence);

// 注册处理器
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 等待所有消息处理完成
await Task.Delay(1000);
```

### 3. 自定义重试策略

```csharp
using WolverineSkill;

// 实现自定义重试策略
public class ExponentialBackoffRetryPolicy : IRetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialRetryInterval;

    public ExponentialBackoffRetryPolicy(int maxRetries, TimeSpan initialRetryInterval)
    {
        _maxRetries = maxRetries;
        _initialRetryInterval = initialRetryInterval;
    }

    public bool ShouldRetry(int retryCount, Exception exception)
    {
        return retryCount < _maxRetries;
    }

    public TimeSpan GetRetryInterval(int retryCount)
    {
        // 指数退避策略：每次重试间隔翻倍
        return TimeSpan.FromMilliseconds(_initialRetryInterval.TotalMilliseconds * Math.Pow(2, retryCount));
    }
}

// 实现会失败的处理器
public class FailingHandler : IHandler<CreateOrderCommand>
{
    private int _attempts = 0;

    public Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        _attempts++;
        Console.WriteLine($"尝试处理命令: {_attempts} 次");

        if (_attempts < 3)
        {
            throw new Exception("模拟处理失败");
        }

        Console.WriteLine($"成功处理命令: {command.OrderId}");
        return Task.CompletedTask;
    }
}

// 创建消息总线并使用自定义重试策略
var retryPolicy = new ExponentialBackoffRetryPolicy(5, TimeSpan.FromSeconds(1));
var messageBus = new MessageBus(retryPolicy: retryPolicy);

// 注册处理器
messageBus.RegisterHandler<CreateOrderCommand, FailingHandler>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 等待所有消息处理完成
await Task.Delay(10000);
```

### 4. 异常处理

```csharp
using WolverineSkill;

// 定义自定义异常
public class OrderProcessingException : Exception
{
    public OrderProcessingException(string message) : base(message) {}
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) {}
}

// 实现异常处理器
public class OrderExceptionHandler : IExceptionHandler<OrderProcessingException>
{
    public Task HandleAsync(OrderProcessingException exception, object message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理订单异常: {exception.Message}, 消息: {message.GetType().Name}");
        return Task.CompletedTask;
    }
}

public class ValidationExceptionHandler : IExceptionHandler<ValidationException>
{
    public Task HandleAsync(ValidationException exception, object message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理验证异常: {exception.Message}, 消息: {message.GetType().Name}");
        return Task.CompletedTask;
    }
}

public class GenericExceptionHandler : IExceptionHandler<Exception>
{
    public Task HandleAsync(Exception exception, object message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理通用异常: {exception.Message}, 消息: {message.GetType().Name}");
        return Task.CompletedTask;
    }
}

// 实现会抛出异常的处理器
public class ExceptionThrowingHandler : IHandler<CreateOrderCommand>
{
    public Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Amount <= 0)
        {
            throw new ValidationException($"无效的订单金额: {command.Amount}");
        }

        if (command.CustomerId <= 0)
        {
            throw new OrderProcessingException($"无效的客户ID: {command.CustomerId}");
        }

        throw new Exception($"未知错误处理订单: {command.OrderId}");
    }
}

// 创建消息总线
var messageBus = new MessageBus();

// 注册处理器
messageBus.RegisterHandler<CreateOrderCommand, ExceptionThrowingHandler>();

// 注册异常处理器
messageBus.RegisterExceptionHandler<OrderProcessingException, OrderExceptionHandler>();
messageBus.RegisterExceptionHandler<ValidationException, ValidationExceptionHandler>();
messageBus.RegisterExceptionHandler<Exception, GenericExceptionHandler>();

// 发送可能失败的命令
var command1 = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = -100.0m };
var command2 = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = -1, Amount = 100.0m };
var command3 = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };

await messageBus.SendAsync(command1);
await messageBus.SendAsync(command2);
await messageBus.SendAsync(command3);

// 等待所有消息处理完成
await Task.Delay(1000);
```

## Scrutor 集成示例

### 1. 自动服务注册

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using WolverineSkill;

// 定义命令和处理器
public class CreateProductCommand : Command
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class UpdateProductCommand : Command
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class DeleteProductCommand : Command
{
    public Guid ProductId { get; set; }
}

public class CreateProductHandler : IHandler<CreateProductCommand>
{
    public Task HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"创建产品: {command.ProductId}, 名称: {command.Name}, 价格: {command.Price}");
        return Task.CompletedTask;
    }
}

public class UpdateProductHandler : IHandler<UpdateProductCommand>
{
    public Task HandleAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"更新产品: {command.ProductId}, 名称: {command.Name}, 价格: {command.Price}");
        return Task.CompletedTask;
    }
}

public class DeleteProductHandler : IHandler<DeleteProductCommand>
{
    public Task HandleAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"删除产品: {command.ProductId}");
        return Task.CompletedTask;
    }
}

// 注册服务
var services = new ServiceCollection();
services.AddWolverineSkill();

// 使用 Scrutor 自动注册处理器
services.Scan(scan => scan
    .FromAssemblyOf<CreateProductHandler>()
    .AddClasses(classes => classes.AssignableTo(typeof(IHandler<>)))
    .AsImplementedInterfaces()
    .WithTransientLifetime());

var serviceProvider = services.BuildServiceProvider();

// 获取消息总线
var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

// 发送命令
var createCommand = new CreateProductCommand { ProductId = Guid.NewGuid(), Name = "产品1", Price = 99.99m };
var updateCommand = new UpdateProductCommand { ProductId = createCommand.ProductId, Name = "产品1 (更新)", Price = 149.99m };
var deleteCommand = new DeleteProductCommand { ProductId = createCommand.ProductId };

await messageBus.SendAsync(createCommand);
await messageBus.SendAsync(updateCommand);
await messageBus.SendAsync(deleteCommand);

// 等待所有消息处理完成
await Task.Delay(1000);
```

### 2. 装饰器模式

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using WolverineSkill;

// 定义命令和处理器
public class ProcessPaymentCommand : Command
{
    public Guid PaymentId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
}

// 实现基本支付处理器
public class PaymentHandler : IHandler<ProcessPaymentCommand>
{
    public Task HandleAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理支付: {command.PaymentId}, 客户: {command.CustomerId}, 金额: {command.Amount} {command.Currency}");
        return Task.CompletedTask;
    }
}

// 实现日志装饰器
public class LoggingHandlerDecorator<T> : IHandler<T>
{
    private readonly IHandler<T> _innerHandler;

    public LoggingHandlerDecorator(IHandler<T> innerHandler)
    {
        _innerHandler = innerHandler;
    }

    public async Task HandleAsync(T message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[日志] 开始处理消息: {typeof(T).Name}");
        await _innerHandler.HandleAsync(message, cancellationToken);
        Console.WriteLine($"[日志] 完成处理消息: {typeof(T).Name}");
    }
}

// 实现验证装饰器
public class ValidationHandlerDecorator<T> : IHandler<T>
{
    private readonly IHandler<T> _innerHandler;

    public ValidationHandlerDecorator(IHandler<T> innerHandler)
    {
        _innerHandler = innerHandler;
    }

    public async Task HandleAsync(T message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[验证] 开始验证消息: {typeof(T).Name}");
        
        // 验证逻辑
        if (message is ProcessPaymentCommand paymentCommand)
        {
            if (paymentCommand.Amount <= 0)
            {
                throw new Exception($"无效的支付金额: {paymentCommand.Amount}");
            }
            if (string.IsNullOrEmpty(paymentCommand.Currency))
            {
                throw new Exception("货币类型不能为空");
            }
        }

        Console.WriteLine($"[验证] 验证通过，继续处理消息: {typeof(T).Name}");
        await _innerHandler.HandleAsync(message, cancellationToken);
    }
}

// 注册服务
var services = new ServiceCollection();
services.AddWolverineSkill();

// 注册基本处理器
services.AddTransient<IHandler<ProcessPaymentCommand>, PaymentHandler>();

// 使用 Scrutor 添加装饰器
services.Decorate(typeof(IHandler<>), typeof(LoggingHandlerDecorator<>));
services.Decorate(typeof(IHandler<>), typeof(ValidationHandlerDecorator<>));

var serviceProvider = services.BuildServiceProvider();

// 获取消息总线
var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

// 发送命令
var command = new ProcessPaymentCommand { PaymentId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m, Currency = "USD" };
await messageBus.SendAsync(command);

// 等待所有消息处理完成
await Task.Delay(1000);
```

### 3. 泛型服务注册

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using WolverineSkill;

// 定义泛型命令
public class GenericCommand<T> : Command
{
    public Guid CommandId { get; set; }
    public T Data { get; set; }
    public string Description { get; set; }
}

// 实现泛型处理器
public class GenericCommandHandler<T> : IHandler<GenericCommand<T>>
{
    public Task HandleAsync(GenericCommand<T> command, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理泛型命令: {command.CommandId}, 类型: {typeof(T).Name}, 描述: {command.Description}, 数据: {command.Data}");
        return Task.CompletedTask;
    }
}

// 注册服务
var services = new ServiceCollection();
services.AddWolverineSkill();

// 注册具体的泛型处理器
services.AddTransient<IHandler<GenericCommand<string>>, GenericCommandHandler<string>>();
services.AddTransient<IHandler<GenericCommand<int>>, GenericCommandHandler<int>>();
services.AddTransient<IHandler<GenericCommand<decimal>>, GenericCommandHandler<decimal>>();
services.AddTransient<IHandler<GenericCommand<Guid>>, GenericCommandHandler<Guid>>();

var serviceProvider = services.BuildServiceProvider();

// 获取消息总线
var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

// 发送不同类型的泛型命令
var stringCommand = new GenericCommand<string> { CommandId = Guid.NewGuid(), Data = "Hello, World!", Description = "字符串命令" };
var intCommand = new GenericCommand<int> { CommandId = Guid.NewGuid(), Data = 42, Description = "整数命令" };
var decimalCommand = new GenericCommand<decimal> { CommandId = Guid.NewGuid(), Data = 100.50m, Description = "小数命令" };
var guidCommand = new GenericCommand<Guid> { CommandId = Guid.NewGuid(), Data = Guid.NewGuid(), Description = "GUID命令" };

await messageBus.SendAsync(stringCommand);
await messageBus.SendAsync(intCommand);
await messageBus.SendAsync(decimalCommand);
await messageBus.SendAsync(guidCommand);

// 等待所有消息处理完成
await Task.Delay(1000);
```

## 性能测试示例

### 1. 消息吞吐量测试

```csharp
using WolverineSkill;
using System.Diagnostics;

// 创建消息总线
var messageBus = new MessageBus();

// 定义测试命令
public class TestCommand : Command
{
    public int Id { get; set; }
    public string Data { get; set; }
}

// 实现简单处理器
public class TestCommandHandler : IHandler<TestCommand>
{
    public Task HandleAsync(TestCommand command, CancellationToken cancellationToken = default)
    {
        // 简单处理逻辑
        return Task.CompletedTask;
    }
}

// 注册处理器
messageBus.RegisterHandler<TestCommand, TestCommandHandler>();

// 测试参数
int messageCount = 10000;

// 预热
for (int i = 0; i < 1000; i++)
{
    var command = new TestCommand { Id = i, Data = "Warmup" };
    await messageBus.SendAsync(command);
}
await Task.Delay(1000);

// 测试吞吐量
var stopwatch = Stopwatch.StartNew();

for (int i = 0; i < messageCount; i++)
{
    var command = new TestCommand { Id = i, Data = $"Test {i}" };
    await messageBus.SendAsync(command);
}

// 等待所有消息处理完成
await Task.Delay(2000);
stopwatch.Stop();

// 计算结果
var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
var throughput = messageCount / elapsedSeconds;

Console.WriteLine($"测试结果:");
Console.WriteLine($"消息数量: {messageCount}");
Console.WriteLine($"耗时: {elapsedSeconds:F2} 秒");
Console.WriteLine($"吞吐量: {throughput:F2} 消息/秒");
```

### 2. 并发性能测试

```csharp
using WolverineSkill;
using System.Diagnostics;
using System.Threading.Tasks;

// 创建消息总线
var messageBus = new MessageBus();

// 定义测试命令
public class ConcurrentTestCommand : Command
{
    public int Id { get; set; }
    public int ThreadId { get; set; }
}

// 实现处理器（模拟处理时间）
public class ConcurrentTestCommandHandler : IHandler<ConcurrentTestCommand>
{
    public async Task HandleAsync(ConcurrentTestCommand command, CancellationToken cancellationToken = default)
    {
        // 模拟处理时间
        await Task.Delay(1);
    }
}

// 注册处理器
messageBus.RegisterHandler<ConcurrentTestCommand, ConcurrentTestCommandHandler>();

// 测试参数
int concurrentThreads = 10;
int messagesPerThread = 1000;

// 预热
for (int i = 0; i < 100; i++)
{
    var command = new ConcurrentTestCommand { Id = i, ThreadId = 0 };
    await messageBus.SendAsync(command);
}
await Task.Delay(1000);

// 测试并发性能
var stopwatch = Stopwatch.StartNew();
var tasks = new List<Task>();

for (int threadId = 0; threadId < concurrentThreads; threadId++)
{
    int currentThreadId = threadId;
    tasks.Add(Task.Run(async () =>
    {
        for (int i = 0; i < messagesPerThread; i++)
        {
            var command = new ConcurrentTestCommand { Id = i, ThreadId = currentThreadId };
            await messageBus.SendAsync(command);
        }
    }));
}

// 等待所有任务完成
await Task.WhenAll(tasks);

// 等待所有消息处理完成
await Task.Delay(2000);
stopwatch.Stop();

// 计算结果
var totalMessages = concurrentThreads * messagesPerThread;
var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
var throughput = totalMessages / elapsedSeconds;

Console.WriteLine($"并发测试结果:");
Console.WriteLine($"并发线程数: {concurrentThreads}");
Console.WriteLine($"每线程消息数: {messagesPerThread}");
Console.WriteLine($"总消息数: {totalMessages}");
Console.WriteLine($"耗时: {elapsedSeconds:F2} 秒");
Console.WriteLine($"吞吐量: {throughput:F2} 消息/秒");
```

### 3. 内存使用测试

```csharp
using WolverineSkill;
using System.Diagnostics;

// 创建消息总线
var messageBus = new MessageBus();

// 定义测试命令
public class LargeMessageCommand : Command
{
    public int Id { get; set; }
    public string LargeData { get; set; }
}

// 实现处理器
public class LargeMessageCommandHandler : IHandler<LargeMessageCommand>
{
    public Task HandleAsync(LargeMessageCommand command, CancellationToken cancellationToken = default)
    {
        // 简单处理
        return Task.CompletedTask;
    }
}

// 注册处理器
messageBus.RegisterHandler<LargeMessageCommand, LargeMessageCommandHandler>();

// 获取初始内存使用
var process = Process.GetCurrentProcess();
var initialMemory = process.WorkingSet64;

// 测试参数
int messageCount = 10000;
int dataSize = 1024; // 1KB 数据
string largeData = new string('x', dataSize);

// 发送消息
for (int i = 0; i < messageCount; i++)
{
    var command = new LargeMessageCommand { Id = i, LargeData = largeData };
    await messageBus.SendAsync(command);
}

// 等待所有消息处理完成
await Task.Delay(2000);

// 获取最终内存使用
var finalMemory = process.WorkingSet64;
var memoryUsed = finalMemory - initialMemory;
var memoryPerMessage = memoryUsed / (double)messageCount;

Console.WriteLine($"内存使用测试结果:");
Console.WriteLine($"消息数量: {messageCount}");
Console.WriteLine($"每条消息数据大小: {dataSize} 字节");
Console.WriteLine($"初始内存: {initialMemory / 1024 / 1024:F2} MB");
Console.WriteLine($"最终内存: {finalMemory / 1024 / 1024:F2} MB");
Console.WriteLine($"使用内存: {memoryUsed / 1024 / 1024:F2} MB");
Console.WriteLine($"每条消息平均内存使用: {memoryPerMessage / 1024:F2} KB");
```

## 实际应用示例

### 1. 订单处理系统

```csharp
using WolverineSkill;

// 定义命令
public class CreateOrderCommand : Command
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; }
    public string PaymentMethod { get; set; }
}

public class UpdateOrderCommand : Command
{
    public Guid OrderId { get; set; }
    public string ShippingAddress { get; set; }
    public string Status { get; set; }
}

public class CancelOrderCommand : Command
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}

// 定义事件
public class OrderCreatedEvent : Event
{
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderUpdatedEvent : Event
{
    public Guid OrderId { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderCanceledEvent : Event
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
    public DateTime CanceledAt { get; set; }
}

public class OrderShippedEvent : Event
{
    public Guid OrderId { get; set; }
    public string TrackingNumber { get; set; }
    public DateTime ShippedAt { get; set; }
}

// 定义订单项
public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

// 实现命令处理器
public class CreateOrderHandler : IHandler<CreateOrderCommand>
{
    private readonly IMessageBus _messageBus;

    public CreateOrderHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        // 验证命令
        if (command.Items == null || !command.Items.Any())
        {
            throw new Exception("订单必须包含至少一个商品");
        }

        if (command.TotalAmount <= 0)
        {
            throw new Exception("订单金额必须大于零");
        }

        // 处理创建订单逻辑
        Console.WriteLine($"创建订单: {command.OrderId}, 客户: {command.CustomerId}, 金额: {command.TotalAmount}");
        Console.WriteLine("商品:");
        foreach (var item in command.Items)
        {
            Console.WriteLine($"  - {item.ProductName}: {item.Quantity} x {item.Price}");
        }

        // 发布订单创建事件
        var @event = new OrderCreatedEvent
        {
            OrderId = command.OrderId,
            CustomerId = command.CustomerId,
            TotalAmount = command.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);
    }
}

public class UpdateOrderHandler : IHandler<UpdateOrderCommand>
{
    private readonly IMessageBus _messageBus;

    public UpdateOrderHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(UpdateOrderCommand command, CancellationToken cancellationToken = default)
    {
        // 处理更新订单逻辑
        Console.WriteLine($"更新订单: {command.OrderId}, 状态: {command.Status}, 地址: {command.ShippingAddress}");

        // 发布订单更新事件
        var @event = new OrderUpdatedEvent
        {
            OrderId = command.OrderId,
            Status = command.Status,
            UpdatedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);

        // 如果状态为已发货，发布发货事件
        if (command.Status == "Shipped")
        {
            var shippedEvent = new OrderShippedEvent
            {
                OrderId = command.OrderId,
                TrackingNumber = Guid.NewGuid().ToString().Substring(0, 10).ToUpper(),
                ShippedAt = DateTime.UtcNow
            };

            await _messageBus.PublishAsync(shippedEvent, cancellationToken);
        }
    }
}

public class CancelOrderHandler : IHandler<CancelOrderCommand>
{
    private readonly IMessageBus _messageBus;

    public CancelOrderHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken = default)
    {
        // 处理取消订单逻辑
        Console.WriteLine($"取消订单: {command.OrderId}, 原因: {command.Reason}");

        // 发布订单取消事件
        var @event = new OrderCanceledEvent
        {
            OrderId = command.OrderId,
            Reason = command.Reason,
            CanceledAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);
    }
}

// 实现事件处理器
public class OrderCreatedHandler : IHandler<OrderCreatedEvent>
{
    public Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 订单已创建: {@event.OrderId}, 客户: {@event.CustomerId}, 金额: {@event.TotalAmount}, 时间: {@event.CreatedAt}");
        // 可以在这里实现发送邮件通知、更新库存等逻辑
        return Task.CompletedTask;
    }
}

public class OrderUpdatedHandler : IHandler<OrderUpdatedEvent>
{
    public Task HandleAsync(OrderUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 订单已更新: {@event.OrderId}, 状态: {@event.Status}, 时间: {@event.UpdatedAt}");
        // 可以在这里实现状态更新通知等逻辑
        return Task.CompletedTask;
    }
}

public class OrderCanceledHandler : IHandler<OrderCanceledEvent>
{
    public Task HandleAsync(OrderCanceledEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 订单已取消: {@event.OrderId}, 原因: {@event.Reason}, 时间: {@event.CanceledAt}");
        // 可以在这里实现退款处理、恢复库存等逻辑
        return Task.CompletedTask;
    }
}

public class OrderShippedHandler : IHandler<OrderShippedEvent>
{
    public Task HandleAsync(OrderShippedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 订单已发货: {@event.OrderId}, 物流单号: {@event.TrackingNumber}, 时间: {@event.ShippedAt}");
        // 可以在这里实现物流通知等逻辑
        return Task.CompletedTask;
    }
}

// 创建消息总线
var messageBus = new MessageBus();

// 注册处理器
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();
messageBus.RegisterHandler<UpdateOrderCommand, UpdateOrderHandler>();
messageBus.RegisterHandler<CancelOrderCommand, CancelOrderHandler>();
messageBus.RegisterHandler<OrderCreatedEvent, OrderCreatedHandler>();
messageBus.RegisterHandler<OrderUpdatedEvent, OrderUpdatedHandler>();
messageBus.RegisterHandler<OrderCanceledEvent, OrderCanceledHandler>();
messageBus.RegisterHandler<OrderShippedEvent, OrderShippedHandler>();

// 测试订单处理流程
var orderId = Guid.NewGuid();

// 1. 创建订单
var createCommand = new CreateOrderCommand
{
    OrderId = orderId,
    CustomerId = 123,
    Items = new List<OrderItem>
    {
        new OrderItem { ProductId = 1, ProductName = "商品1", Quantity = 2, Price = 50.0m },
        new OrderItem { ProductId = 2, ProductName = "商品2", Quantity = 1, Price = 100.0m }
    },
    TotalAmount = 200.0m,
    ShippingAddress = "北京市朝阳区",
    PaymentMethod = "支付宝"
};

await messageBus.SendAsync(createCommand);
await Task.Delay(500);

// 2. 更新订单状态为已支付
var updateCommand1 = new UpdateOrderCommand
{
    OrderId = orderId,
    Status = "Paid",
    ShippingAddress = "北京市朝阳区"
};

await messageBus.SendAsync(updateCommand1);
await Task.Delay(500);

// 3. 更新订单状态为已发货
var updateCommand2 = new UpdateOrderCommand
{
    OrderId = orderId,
    Status = "Shipped",
    ShippingAddress = "北京市朝阳区"
};

await messageBus.SendAsync(updateCommand2);
await Task.Delay(500);

// 4. 取消订单（演示）
var cancelCommand = new CancelOrderCommand
{
    OrderId = orderId,
    Reason = "客户要求取消"
};

await messageBus.SendAsync(cancelCommand);
await Task.Delay(500);

Console.WriteLine("订单处理流程测试完成");
```

### 2. 库存管理系统

```csharp
using WolverineSkill;

// 定义命令
public class AddInventoryCommand : Command
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}

public class RemoveInventoryCommand : Command
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; }
}

public class TransferInventoryCommand : Command
{
    public int ProductId { get; set; }
    public int FromLocationId { get; set; }
    public int ToLocationId { get; set; }
    public int Quantity { get; set; }
}

// 定义事件
public class InventoryAddedEvent : Event
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime AddedAt { get; set; }
}

public class InventoryRemovedEvent : Event
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; }
    public DateTime RemovedAt { get; set; }
}

public class InventoryTransferredEvent : Event
{
    public int ProductId { get; set; }
    public int FromLocationId { get; set; }
    public int ToLocationId { get; set; }
    public int Quantity { get; set; }
    public DateTime TransferredAt { get; set; }
}

public class InventoryLowEvent : Event
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int CurrentQuantity { get; set; }
    public int Threshold { get; set; }
    public DateTime NotifiedAt { get; set; }
}

// 实现命令处理器
public class AddInventoryHandler : IHandler<AddInventoryCommand>
{
    private readonly IMessageBus _messageBus;
    private readonly Dictionary<int, int> _inventory = new Dictionary<int, int>(); // 模拟库存

    public AddInventoryHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(AddInventoryCommand command, CancellationToken cancellationToken = default)
    {
        // 验证命令
        if (command.Quantity <= 0)
        {
            throw new Exception("添加数量必须大于零");
        }

        if (command.UnitCost <= 0)
        {
            throw new Exception("单位成本必须大于零");
        }

        // 更新库存
        if (!_inventory.ContainsKey(command.ProductId))
        {
            _inventory[command.ProductId] = 0;
        }
        _inventory[command.ProductId] += command.Quantity;

        // 处理添加库存逻辑
        Console.WriteLine($"添加库存: 商品 {command.ProductId} ({command.ProductName}), 数量: {command.Quantity}, 单位成本: {command.UnitCost}");
        Console.WriteLine($"当前库存: {_inventory[command.ProductId]}");

        // 发布库存添加事件
        var @event = new InventoryAddedEvent
        {
            ProductId = command.ProductId,
            ProductName = command.ProductName,
            Quantity = command.Quantity,
            UnitCost = command.UnitCost,
            AddedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);
    }
}

public class RemoveInventoryHandler : IHandler<RemoveInventoryCommand>
{
    private readonly IMessageBus _messageBus;
    private readonly Dictionary<int, int> _inventory = new Dictionary<int, int>(); // 模拟库存
    private readonly Dictionary<int, string> _productNames = new Dictionary<int, string>(); // 模拟商品名称

    public RemoveInventoryHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(RemoveInventoryCommand command, CancellationToken cancellationToken = default)
    {
        // 验证命令
        if (command.Quantity <= 0)
        {
            throw new Exception("移除数量必须大于零");
        }

        // 检查库存
        if (!_inventory.ContainsKey(command.ProductId) || _inventory[command.ProductId] < command.Quantity)
        {
            throw new Exception($"库存不足: 商品 {command.ProductId}, 可用: {_inventory.GetValueOrDefault(command.ProductId)}, 请求: {command.Quantity}");
        }

        // 更新库存
        _inventory[command.ProductId] -= command.Quantity;

        // 处理移除库存逻辑
        Console.WriteLine($"移除库存: 商品 {command.ProductId}, 数量: {command.Quantity}, 原因: {command.Reason}");
        Console.WriteLine($"当前库存: {_inventory[command.ProductId]}");

        // 发布库存移除事件
        var @event = new InventoryRemovedEvent
        {
            ProductId = command.ProductId,
            Quantity = command.Quantity,
            Reason = command.Reason,
            RemovedAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);

        // 检查库存是否过低
        const int lowInventoryThreshold = 10;
        if (_inventory[command.ProductId] < lowInventoryThreshold)
        {
            var lowEvent = new InventoryLowEvent
            {
                ProductId = command.ProductId,
                ProductName = _productNames.GetValueOrDefault(command.ProductId, "未知商品"),
                CurrentQuantity = _inventory[command.ProductId],
                Threshold = lowInventoryThreshold,
                NotifiedAt = DateTime.UtcNow
            };

            await _messageBus.PublishAsync(lowEvent, cancellationToken);
        }
    }
}

public class TransferInventoryHandler : IHandler<TransferInventoryCommand>
{
    private readonly IMessageBus _messageBus;
    private readonly Dictionary<(int ProductId, int LocationId), int> _inventoryByLocation = new Dictionary<(int, int), int>(); // 模拟按位置存储的库存

    public TransferInventoryHandler(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task HandleAsync(TransferInventoryCommand command, CancellationToken cancellationToken = default)
    {
        // 验证命令
        if (command.Quantity <= 0)
        {
            throw new Exception("转移数量必须大于零");
        }

        if (command.FromLocationId == command.ToLocationId)
        {
            throw new Exception("源位置和目标位置不能相同");
        }

        // 检查源位置库存
        var fromKey = (command.ProductId, command.FromLocationId);
        var toKey = (command.ProductId, command.ToLocationId);

        if (!_inventoryByLocation.ContainsKey(fromKey) || _inventoryByLocation[fromKey] < command.Quantity)
        {
            throw new Exception($"源位置库存不足: 商品 {command.ProductId}, 位置 {command.FromLocationId}, 可用: {_inventoryByLocation.GetValueOrDefault(fromKey)}, 请求: {command.Quantity}");
        }

        // 更新库存
        _inventoryByLocation[fromKey] -= command.Quantity;
        if (!_inventoryByLocation.ContainsKey(toKey))
        {
            _inventoryByLocation[toKey] = 0;
        }
        _inventoryByLocation[toKey] += command.Quantity;

        // 处理转移库存逻辑
        Console.WriteLine($"转移库存: 商品 {command.ProductId}, 从位置 {command.FromLocationId} 到 {command.ToLocationId}, 数量: {command.Quantity}");
        Console.WriteLine($"源位置库存: {_inventoryByLocation[fromKey]}");
        Console.WriteLine($"目标位置库存: {_inventoryByLocation[toKey]}");

        // 发布库存转移事件
        var @event = new InventoryTransferredEvent
        {
            ProductId = command.ProductId,
            FromLocationId = command.FromLocationId,
            ToLocationId = command.ToLocationId,
            Quantity = command.Quantity,
            TransferredAt = DateTime.UtcNow
        };

        await _messageBus.PublishAsync(@event, cancellationToken);
    }
}

// 实现事件处理器
public class InventoryAddedHandler : IHandler<InventoryAddedEvent>
{
    public Task HandleAsync(InventoryAddedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 库存已添加: 商品 {@event.ProductId} ({@event.ProductName}), 数量: {@event.Quantity}, 单位成本: {@event.UnitCost}, 时间: {@event.AddedAt}");
        // 可以在这里实现库存记录、财务处理等逻辑
        return Task.CompletedTask;
    }
}

public class InventoryRemovedHandler : IHandler<InventoryRemovedEvent>
{
    public Task HandleAsync(InventoryRemovedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 库存已移除: 商品 {@event.ProductId}, 数量: {@event.Quantity}, 原因: {@event.Reason}, 时间: {@event.RemovedAt}");
        // 可以在这里实现库存记录、销售处理等逻辑
        return Task.CompletedTask;
    }
}

public class InventoryTransferredHandler : IHandler<InventoryTransferredEvent>
{
    public Task HandleAsync(InventoryTransferredEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 库存已转移: 商品 {@event.ProductId}, 从位置 {@event.FromLocationId} 到 {@event.ToLocationId}, 数量: {@event.Quantity}, 时间: {@event.TransferredAt}");
        // 可以在这里实现库存记录、位置调整等逻辑
        return Task.CompletedTask;
    }
}

public class InventoryLowHandler : IHandler<InventoryLowEvent>
{
    public Task HandleAsync(InventoryLowEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[事件] 库存过低警告: 商品 {@event.ProductId} ({@event.ProductName}), 当前库存: {@event.CurrentQuantity}, 阈值: {@event.Threshold}, 时间: {@event.NotifiedAt}");
        // 可以在这里实现通知、自动补货等逻辑
        return Task.CompletedTask;
    }
}

// 创建消息总线
var messageBus = new MessageBus();

// 注册处理器
messageBus.RegisterHandler<AddInventoryCommand, AddInventoryHandler>();
messageBus.RegisterHandler<RemoveInventoryCommand, RemoveInventoryHandler>();
messageBus.RegisterHandler<TransferInventoryCommand, TransferInventoryHandler>();
messageBus.RegisterHandler<InventoryAddedEvent, InventoryAddedHandler>();
messageBus.RegisterHandler<InventoryRemovedEvent, InventoryRemovedHandler>();
messageBus.RegisterHandler<InventoryTransferredEvent, InventoryTransferredHandler>();
messageBus.RegisterHandler<InventoryLowEvent, InventoryLowHandler>();

// 测试库存管理流程

// 1. 添加库存
var addCommand = new AddInventoryCommand
{
    ProductId = 1,
    ProductName = "商品1",
    Quantity = 100,
    UnitCost = 50.0m
};

await messageBus.SendAsync(addCommand);
await Task.Delay(500);

// 2. 移除库存
var removeCommand = new RemoveInventoryCommand
{
    ProductId = 1,
    Quantity = 95,
    Reason = "销售"
};

await messageBus.SendAsync(removeCommand);
await Task.Delay(500);

// 3. 转移库存
var transferCommand = new TransferInventoryCommand
{
    ProductId = 1,
    FromLocationId = 1,
    ToLocationId = 2,
    Quantity = 3
};

await messageBus.SendAsync(transferCommand);
await Task.Delay(500);

Console.WriteLine("库存管理流程测试完成");
```