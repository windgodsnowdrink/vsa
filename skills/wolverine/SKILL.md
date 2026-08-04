# Wolverine 技能

Wolverine 技能是一个专注于高性能消息传递和命令处理的工具库，提供了轻量级的消息总线、命令处理器、事件处理器等功能，帮助开发者构建可靠、可扩展的分布式系统。

## 功能特性

- **轻量级消息总线**：提供简单易用的消息发送和接收机制
- **命令处理**：支持命令模式，实现命令的发送和处理
- **事件处理**：支持事件模式，实现事件的发布和订阅
- **消息路由**：基于类型和配置的消息路由机制
- **消息重试**：内置消息重试机制，提高系统可靠性
- **消息持久化**：支持消息的持久化存储，确保消息不丢失
- **分布式支持**：支持分布式环境下的消息传递
- **性能优化**：采用高性能的消息处理机制，减少延迟
- **依赖注入**：支持通过 DI 容器注册和使用
- **可扩展性**：提供插件机制，支持自定义消息处理器和中间件

## 安装

### 方法一：通过 NuGet 安装

```bash
dotnet add package WolverineSkill
```

### 方法二：手动引用

将 `wolverine_core.cs` 文件添加到您的项目中。

## 注册服务

在 `Startup.cs` 或 `Program.cs` 中注册 Wolverine 技能服务：

```csharp
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 注册 Wolverine 技能服务
builder.Services.AddWolverineSkill();

var app = builder.Build();
app.Run();
```

## 使用示例

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
services.AddWolverineSkill();
var serviceProvider = services.BuildServiceProvider();

// 获取消息总线
var messageBus = serviceProvider.GetRequiredService<IMessageBus>();

// 注册命令处理器
services.AddTransient<CreateOrderHandler>();

// 发送命令
var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
await messageBus.SendAsync(command);

// 注册事件处理器
services.AddTransient<OrderCreatedHandler>();

// 发布事件
var @event = new OrderCreatedEvent { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m, CreatedAt = DateTime.UtcNow };
await messageBus.PublishAsync(@event);
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

// 发送不同类型的命令
await messageBus.SendAsync(new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m });
await messageBus.SendAsync(new UpdateOrderCommand { OrderId = existingOrderId, Amount = 150.0m });
await messageBus.SendAsync(new DeleteOrderCommand { OrderId = existingOrderId });
```

## 性能优化

Wolverine 技能内置了多种性能优化机制，包括：

1. **消息批处理**：将多个小消息合并为一个批次处理，减少网络开销
2. **消息压缩**：对大型消息进行压缩，减少传输大小
3. **异步处理**：采用异步非阻塞的消息处理机制，提高并发性能
4. **内存池**：使用对象池减少内存分配和垃圾回收
5. **批处理确认**：批量确认消息处理完成，减少网络往返

## Scrutor 集成

Wolverine 技能支持使用 Scrutor 进行自动服务注册，详见 `scrutor_demo.cs` 文件中的示例。

## 配置选项

可以通过配置文件自定义 Wolverine 技能的行为：

```json
{
  "WolverineSkill": {
    "MessageBus": {
      "EnableRetries": true,
      "MaxRetries": 5,
      "RetryInterval": "00:00:01",
      "EnablePersistence": true,
      "PersistenceProvider": "InMemory"
    },
    "Serialization": {
      "Type": "Json",
      "EnableCompression": true
    }
  }
}
```

## 异常处理

Wolverine 技能在遇到消息处理异常时会自动重试，也可以通过自定义异常处理器来处理特殊情况：

```csharp
using WolverineSkill;

// 创建消息总线
var messageBus = new MessageBus();

// 注册异常处理器
messageBus.RegisterExceptionHandler<OrderProcessingException, OrderExceptionHandler>();

// 发送可能失败的命令
try
{
    var command = new CreateOrderCommand { OrderId = Guid.NewGuid(), CustomerId = 123, Amount = 100.0m };
    await messageBus.SendAsync(command);
}
catch (Exception ex)
{
    Console.WriteLine($"命令发送失败: {ex.Message}");
}
```

## 相关资源

- [参考文档](./reference/README.md)
- [使用示例](./reference/examples.md)

## 贡献

欢迎提交 Issue 和 Pull Request 来帮助改进 Wolverine 技能！

## 许可证

Wolverine 技能采用 MIT 许可证。