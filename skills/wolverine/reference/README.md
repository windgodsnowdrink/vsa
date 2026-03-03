# Wolverine 技能参考文档

## 概述

Wolverine 技能是一个专注于高性能消息传递和命令处理的工具库，提供了轻量级的消息总线、命令处理器、事件处理器等功能，帮助开发者构建可靠、可扩展的分布式系统。

## 核心功能

### 1. 轻量级消息总线

提供简单易用的消息发送和接收机制，支持：
- 同步和异步消息发送
- 消息路由和分发
- 消息重试和错误处理
- 消息持久化和恢复

### 2. 命令处理

支持命令模式，实现命令的发送和处理：
- 命令的定义和发送
- 命令处理器的注册和执行
- 命令的验证和授权
- 命令的结果返回

### 3. 事件处理

支持事件模式，实现事件的发布和订阅：
- 事件的定义和发布
- 事件处理器的注册和执行
- 事件的过滤和路由
- 事件的持久化和重播

### 4. 消息路由

基于类型和配置的消息路由机制：
- 基于消息类型的路由
- 基于属性的路由
- 基于规则的路由
- 自定义路由策略

### 5. 消息重试

内置消息重试机制，提高系统可靠性：
- 可配置的重试次数和间隔
- 指数退避重试策略
- 重试失败后的错误处理
- 重试状态的持久化

### 6. 消息持久化

支持消息的持久化存储，确保消息不丢失：
- 内存存储（适用于开发和测试）
- 文件存储（适用于单机部署）
- 数据库存储（适用于生产环境）
- 自定义存储提供者

### 7. 分布式支持

支持分布式环境下的消息传递：
- 消息的序列化和反序列化
- 网络传输和协议支持
- 节点发现和负载均衡
- 分布式事务支持

### 8. 性能优化

采用高性能的消息处理机制，减少延迟：
- 消息批处理
- 消息压缩
- 异步处理
- 内存池
- 批处理确认

### 9. 依赖注入

支持通过 DI 容器注册和使用：
- 服务注册和解析
- 生命周期管理
- 构造函数注入
- 选项模式支持

### 10. 可扩展性

提供插件机制，支持自定义消息处理器和中间件：
- 自定义消息处理器
- 自定义中间件
- 自定义存储提供者
- 自定义序列化器

## 架构设计

### 核心组件

1. **MessageBus**：消息总线核心类，负责消息的发送和接收
2. **IHandler**：消息处理器接口，定义了消息处理的方法
3. **Command**：命令基类，表示需要执行的操作
4. **Event**：事件基类，表示已经发生的事情
5. **IMessageSerializer**：消息序列化接口，负责消息的序列化和反序列化
6. **IMessagePersistence**：消息持久化接口，负责消息的存储和恢复
7. **IRetryPolicy**：重试策略接口，定义了消息重试的逻辑
8. **IMessageRouter**：消息路由接口，负责消息的路由和分发
9. **IExceptionHandler**：异常处理器接口，负责处理消息处理过程中的异常
10. **WolverineSkillExtensions**：依赖注入扩展方法

### 依赖关系

```
┌─────────────────┐     ┌─────────────────┐
│   MessageBus    │ ◄── │   IHandler      │
└─────────────────┘     └─────────────────┘
        ▲                      ▲
        │                      │
┌─────────────────┐     ┌─────────────────┐
│ IMessageRouter  │     │    Command      │
└─────────────────┘     └─────────────────┘
        ▲                      ▲
        │                      │
┌─────────────────┐     ┌─────────────────┐
│ IMessagePersistence │  │     Event       │
└─────────────────┘     └─────────────────┘
        ▲                      ▲
        │                      │
┌─────────────────┐     ┌─────────────────┐
│ IMessageSerializer │  │ IRetryPolicy   │
└─────────────────┘     └─────────────────┘
        ▲                      ▲
        │                      │
┌─────────────────┐     ┌─────────────────┐
│ IExceptionHandler │  │ WolverineSkill  │
└─────────────────┘     │ Extensions     │
                        └─────────────────┘
```

## 性能特性

### 消息处理性能

- **吞吐量**：支持每秒处理 thousands 级别的消息
- **延迟**：平均消息处理延迟在毫秒级别
- **并发**：支持高并发的消息处理
- **可扩展性**：支持水平扩展以提高处理能力

### 内存使用

- **对象池**：使用对象池减少内存分配和垃圾回收
- **内存限制**：可配置的内存使用限制
- **内存监控**：内置内存使用监控

### 可靠性

- **消息不丢失**：通过持久化确保消息不丢失
- **消息不重复**：通过消息 ID 确保消息不重复处理
- **故障恢复**：支持从故障中恢复消息处理

## 配置选项

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| MessageBus.EnableRetries | bool | true | 是否启用消息重试 |
| MessageBus.MaxRetries | int | 5 | 最大重试次数 |
| MessageBus.RetryInterval | TimeSpan | 00:00:01 | 重试间隔 |
| MessageBus.EnablePersistence | bool | true | 是否启用消息持久化 |
| MessageBus.PersistenceProvider | string | InMemory | 持久化提供者 |
| Serialization.Type | string | Json | 序列化类型 |
| Serialization.EnableCompression | bool | true | 是否启用消息压缩 |
| MessageBus.BatchSize | int | 100 | 消息批处理大小 |
| MessageBus.MaxConcurrentHandlers | int | 10 | 最大并发处理器数量 |

## 使用指南

### 基本用法

```csharp
using WolverineSkill;

// 创建消息总线
var messageBus = new MessageBus();

// 注册命令处理器
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 注册事件处理器
messageBus.RegisterHandler<OrderCreatedEvent, OrderCreatedHandler>();

// 发布事件
var @event = new OrderCreatedEvent { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m, CreatedAt = DateTime.UtcNow };
await messageBus.PublishAsync(@event);
```

### 通过依赖注入使用

```csharp
using Microsoft.Extensions.DependencyInjection;
using WolverineSkill;

// 注册服务
var services = new ServiceCollection();
services.AddWolverineSkill(options =>
{
    options.MessageBus.EnableRetries = true;
    options.MessageBus.MaxRetries = 5;
    options.MessageBus.EnablePersistence = true;
    options.Serialization.EnableCompression = true;
});

// 注册命令处理器
services.AddTransient<CreateOrderHandler>();

// 注册事件处理器
services.AddTransient<OrderCreatedHandler>();

var serviceProvider = services.BuildServiceProvider();

// 获取消息总线
var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 发布事件
var @event = new OrderCreatedEvent { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m, CreatedAt = DateTime.UtcNow };
await messageBus.PublishAsync(@event);
```

### 自定义消息处理器

```csharp
using WolverineSkill;

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
```

### 消息路由

```csharp
using WolverineSkill;

// 创建消息总线
var messageBus = new MessageBus();

// 注册基于类型的路由
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();
messageBus.RegisterHandler<UpdateOrderCommand, UpdateOrderHandler>();
messageBus.RegisterHandler<DeleteOrderCommand, DeleteOrderHandler>();

// 注册基于属性的路由
messageBus.RegisterHandler<OrderEvent, PremiumCustomerOrderHandler>(message => message is OrderEvent e && e.Amount > 1000.0m);
messageBus.RegisterHandler<OrderEvent, RegularCustomerOrderHandler>(message => message is OrderEvent e && e.Amount <= 1000.0m);

// 发送命令
await messageBus.SendAsync(new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m });
await messageBus.SendAsync(new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 456, Amount = 2000.0m });
```

### 异常处理

```csharp
using WolverineSkill;

// 创建消息总线
var messageBus = new MessageBus();

// 注册命令处理器
messageBus.RegisterHandler<CreateOrderCommand, CreateOrderHandler>();

// 注册异常处理器
messageBus.RegisterExceptionHandler<OrderProcessingException, OrderExceptionHandler>();
messageBus.RegisterExceptionHandler<ValidationException, ValidationExceptionHandler>();

// 发送可能失败的命令
try
{
    var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = -100.0m }; // 无效金额
    await messageBus.SendAsync(command);
}
catch (Exception ex)
{
    Console.WriteLine($"命令发送失败: {ex.Message}");
}

// 实现异常处理器
public class OrderExceptionHandler : IExceptionHandler<OrderProcessingException>
{
    public Task HandleAsync(OrderProcessingException exception, object message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理订单异常: {exception.Message}, 消息: {message}");
        return Task.CompletedTask;
    }
}

public class ValidationExceptionHandler : IExceptionHandler<ValidationException>
{
    public Task HandleAsync(ValidationException exception, object message, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"处理验证异常: {exception.Message}, 消息: {message}");
        return Task.CompletedTask;
    }
}
```

## 最佳实践

### 1. 消息设计

- **命令设计**：命令应该是具体的、可执行的操作，通常使用动词开头的命名方式（如 CreateOrderCommand）
- **事件设计**：事件应该是已经发生的事实，通常使用名词+过去分词的命名方式（如 OrderCreatedEvent）
- **消息大小**：尽量保持消息小巧，避免在消息中包含大量数据
- **消息版本**：考虑消息的版本控制，确保向后兼容性

### 2. 处理器设计

- **单一职责**：每个处理器应该只处理一种类型的消息
- **异步处理**：使用异步方法处理消息，避免阻塞
- **错误处理**：在处理器中捕获和处理预期的异常
- **依赖注入**：通过构造函数注入依赖，避免硬编码

### 3. 消息总线配置

- **环境配置**：根据不同环境（开发、测试、生产）配置不同的消息总线参数
- **性能调优**：根据系统负载和硬件配置调整批处理大小、并发度等参数
- **监控配置**：启用适当的监控和日志记录
- **安全配置**：确保消息传递的安全性，特别是在分布式环境中

### 4. 分布式系统考虑

- **消息序列化**：选择高效、可靠的序列化方式
- **网络传输**：考虑网络延迟和带宽限制
- **节点发现**：实现可靠的节点发现机制
- **负载均衡**：在多个节点之间合理分配消息处理负载
- **分布式事务**：考虑使用分布式事务或最终一致性模式

## 常见问题

### Q: 消息总线的默认配置是什么？

A: 默认配置如下：
- 启用重试：true
- 最大重试次数：5
- 重试间隔：1秒
- 启用持久化：true
- 持久化提供者：InMemory
- 序列化类型：Json
- 启用压缩：true

### Q: 如何自定义消息序列化器？

A: 可以实现 `IMessageSerializer` 接口并注册到依赖注入容器中：

```csharp
services.AddSingleton<IMessageSerializer, CustomMessageSerializer>();
```

### Q: 如何实现消息的持久化存储？

A: 可以实现 `IMessagePersistence` 接口并注册到依赖注入容器中：

```csharp
services.AddSingleton<IMessagePersistence, DatabaseMessagePersistence>();
```

### Q: 如何处理消息处理失败的情况？

A: 可以注册异常处理器来处理消息处理失败的情况：

```csharp
messageBus.RegisterExceptionHandler<Exception, GenericExceptionHandler>();
```

### Q: 如何在分布式环境中使用消息总线？

A: 需要配置消息的序列化、网络传输和节点发现机制，具体配置取决于使用的传输协议和集群管理方案。

## 示例项目

请参考 `examples.md` 文件中的详细使用示例，包括：
- 基本使用示例
- 高级使用示例
- Scrutor 集成示例
- 性能测试示例

## 贡献指南

欢迎提交 Issue 和 Pull Request 来帮助改进 Wolverine 技能！

### 开发环境

- .NET 8.0 或更高版本
- Visual Studio 2022 或更高版本
- .NET CLI

### 测试

```bash
dotnet test
```

### 构建

```bash
dotnet build
```

## 许可证

Wolverine 技能采用 MIT 许可证。