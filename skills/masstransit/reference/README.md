# MassTransit - 参考文档

## 概述

MassTransit 是基于 .NET 10 开发的高性能消息传递系统，专为 .NET 开发者设计，提供强大的消息发送、接收和处理功能，支持多种消息 broker 和 Saga 状态机编排。

## 核心组件

### 1. MassTransitService（消息服务）
- **位置**: scripts/masstransit_integration.cs
- **功能**: 核心消息发送和处理业务逻辑
- **特性**: 
  - 支持多种消息传输（RabbitMQ、Azure Service Bus、Kafka等）
  - 高性能异步消息处理
  - 消息重试和错误处理
  - 详细的日志记录
  - 零拷贝消息序列化

### 2. ProductionService（生产环境服务）
- **位置**: scripts/masstransit_production_integration.cs
- **功能**: 生产环境消息处理和监控
- **特性**: 
  - 支持断路器模式
  - 支持速率限制
  - 生产环境监控
  - 高可用配置
  - 健康检查集成

### 3. SagaOrchestrator（Saga编排器）
- **位置**: scripts/saga_orchestrator.cs
- **功能**: 分布式事务和Saga状态机编排
- **特性**: 
  - 支持补偿事务
  - 状态机管理
  - 故障恢复
  - 持久化存储
  - 超时处理

### 4. InMemoryService（内存模式服务）
- **位置**: scripts/masstransit_inmemory_integration.cs
- **功能**: 内存模式消息处理
- **特性**: 
  - 轻量级消息处理
  - 适合测试和开发环境
  - 高性能内存队列
  - 无外部依赖

### 5. LiteDBService（LiteDB集成服务）
- **位置**: scripts/masstransit_litedb_integration.cs
- **功能**: 使用LiteDB存储Saga状态
- **特性**: 
  - 轻量级持久化
  - 嵌入式数据库
  - 适合小型应用
  - 无需外部数据库

### 6. MQTTService（MQTT集成服务）
- **位置**: scripts/masstransit_mqttnet_integration.cs
- **功能**: MQTT消息集成
- **特性**: 
  - 支持IoT设备消息
  - 轻量级协议
  - 桥接RabbitMQ和MQTT
  - 适合边缘设备

### 7. MasuitToolsService（工具集成服务）
- **位置**: scripts/masuit_tools_integration.cs
- **功能**: 集成Masuit.Tools工具库
- **特性**: 
  - 提供常用工具类
  - 加密和安全功能
  - 文件和日期处理
  - 扩展方法支持

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

### AOT 架构优势

1. **启动速度提升**：AOT 编译消除了 JIT 编译开销，显著提高应用启动速度
2. **内存使用减少**：减少了运行时元数据和 JIT 编译缓存的内存占用
3. **部署简化**：单文件发布减少了部署复杂性
4. **安全性增强**：减少了运行时反射攻击面
5. **性能稳定**：编译时优化确保运行时性能稳定

### 性能优化技术

1. **Threading.Channels**: 高效的异步事件队列处理，支持背压控制
2. **ObjectPool**: 减少对象创建开销，优化内存使用
3. **Span 零拷贝**: 减少内存分配和复制
4. **TailLatencyOptimizer**: 尾延迟优化
5. **AggressiveOptimization**: 编译器级优化
6. **Cache-line 对齐**: 内存分配优化
7. **RingBuffer + Disruptor 模式**: 高性能消息处理
8. **Batch 处理**: 减少网络往返开销
9. **内存池化**: 减少 GC 压力
10. **并行处理**: 提高多核利用率

## 使用示例

### 基本用法

```csharp
var publishEndpoint = serviceProvider.GetRequiredService<IPublishEndpoint>();
await publishEndpoint.Publish(new OrderCreated {
    OrderId = Guid.NewGuid(),
    CustomerId = 1,
    TotalAmount = 100.00
});
Console.WriteLine("订单创建消息已发布");
```

### 高级配置

```csharp
var builder = WebApplication.CreateBuilder();

// 配置 MassTransit
builder.Services.AddMassTransit(x => {
    x.AddConsumer<OrderConsumer>();
    x.AddConsumer<PaymentConsumer>();
    
    x.UsingRabbitMq((context, cfg) => {
        cfg.Host("localhost", h => {
            h.Username("guest");
            h.Password("guest");
        });
        
        // 高性能配置
        cfg.PrefetchCount = (ushort)(Environment.ProcessorCount * 2);
        cfg.UseMessageRetry(r => r.Interval(3, 1000));
        cfg.UseCircuitBreaker(cb => {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });
        
        // 消息批处理
        cfg.UseBatching(b => {
            b.MessageLimit = 100;
            b.TimeLimit = TimeSpan.FromMilliseconds(50);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

// 注册 MassTransit 宿主服务
builder.Services.AddMassTransitHostedService();

var app = builder.Build();
app.Run();
```

### Saga 状态机示例

```csharp
// 配置 Saga 状态机
builder.Services.AddMassTransit(x => {
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .RedisRepository(r => r.DatabaseConfiguration("localhost"));
    
    x.UsingRabbitMq((context, cfg) => {
        cfg.Host("localhost", h => {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

## 配置选项

### MassTransit 核心配置

```json
{
  "MassTransit": {
    "Enabled": true,
    "RabbitMQ": {
      "Host": "localhost",
      "Username": "guest",
      "Password": "guest",
      "VirtualHost": "/",
      "Port": 5672,
      "UseSsl": false
    },
    "Options": {
      "PrefetchCount": 16,
      "ConcurrentMessageLimit": 10,
      "RetryCount": 3,
      "EnableCompression": true,
      "EnableBatchProcessing": true,
      "BatchSize": 100,
      "BatchTimeout": 50
    }
  }
}
```

### 生产环境配置

```json
{
  "MassTransit": {
    "Production": {
      "EnableCircuitBreaker": true,
      "CircuitBreaker": {
        "TrackingPeriod": "00:01:00",
        "TripThreshold": 15,
        "ActiveThreshold": 10,
        "ResetInterval": "00:05:00"
      },
      "EnableRateLimiting": true,
      "RateLimiting": {
        "Limit": 100,
        "Window": "00:00:01"
      },
      "EnableMonitoring": true,
      "Monitoring": {
        "MetricsEndpoint": "/metrics",
        "HealthCheckEndpoint": "/health"
      }
    }
  }
}
```

### Saga 配置

```json
{
  "Saga": {
    "Enabled": true,
    "Options": {
      "MaxConcurrency": 1,
      "EnableCompensation": true,
      "CompensationTimeout": "00:30:00",
      "EnablePersistence": true,
      "PersistenceProvider": "LiteDB",
      "LiteDB": {
        "ConnectionString": "saga.db"
      }
    }
  }
}
```

### MQTT 配置

```json
{
  "MQTT": {
    "Enabled": true,
    "Options": {
      "Server": "localhost",
      "Port": 1883,
      "ClientId": "masstransit-mqtt-bridge",
      "Username": "",
      "Password": "",
      "CleanSession": true,
      "KeepAlivePeriod": 60
    }
  }
}
```

## 性能优化

1. **消息批处理**: 使用批量消息处理减少网络开销
2. **连接池**: 使用连接池减少连接创建开销
3. **异步编程**: 使用异步API避免线程阻塞
4. **消息压缩**: 启用消息压缩减少网络传输
5. **内存优化**: 使用对象池和Span减少内存分配
6. **并行处理**: 合理配置并发度提高处理能力
7. **缓存使用**: 缓存常用消息和配置
8. **消息路由优化**: 合理设计消息路由减少网络跳数
9. **消费者粒度**: 合理设计消费者粒度提高并行处理能力
10. **监控和调优**: 持续监控和调优系统性能

## 故障排除

### 常见问题

1. **RabbitMQ连接失败**
   - 检查RabbitMQ服务器状态
   - 验证连接字符串配置
   - 检查网络连接和防火墙设置
   - 查看RabbitMQ日志
   - 检查认证信息是否正确

2. **消息处理失败**
   - 检查消费者代码异常
   - 验证消息格式正确
   - 检查重试策略配置
   - 查看MassTransit日志
   - 检查死信队列

3. **性能问题**
   - 调整PrefetchCount配置
   - 检查消息处理逻辑效率
   - 启用消息压缩
   - 增加消费者并发度
   - 检查网络带宽
   - 优化数据库操作

4. **Saga状态丢失**
   - 检查持久化配置
   - 验证存储连接
   - 检查状态机配置
   - 查看存储日志
   - 检查补偿事务是否正确执行

5. **AOT编译问题**
   - 确保所有依赖支持AOT编译
   - 检查运行时标识符设置
   - 验证单文件可执行配置
   - 检查反射使用情况
   - 查看AOT编译警告和错误

6. **MQTT连接问题**
   - 检查MQTT broker状态
   - 验证MQTT配置
   - 检查网络连接
   - 查看MQTT客户端日志
   - 检查认证信息

## 扩展开发

### 添加自定义消费者

```csharp
public class CustomConsumer : IConsumer<CustomMessage>
{
    private readonly ILogger<CustomConsumer> _logger;
    private readonly IService _service;
    
    public CustomConsumer(ILogger<CustomConsumer> logger, IService service)
    {
        _logger = logger;
        _service = service;
    }
    
    public async Task Consume(ConsumeContext<CustomMessage> context)
    {
        _logger.LogInformation("Processing message: {MessageId}", context.MessageId);
        
        try
        {
            // 处理消息逻辑
            await _service.ProcessAsync(context.Message);
            
            // 发布响应消息
            await context.Publish(new CustomMessageProcessed {
                CorrelationId = context.Message.CorrelationId,
                ProcessedAt = DateTimeOffset.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {MessageId}", context.MessageId);
            throw; // 重新抛出异常以触发重试
        }
    }
}
```

### 注册自定义消费者

```csharp
builder.Services.AddMassTransit(x => {
    x.AddConsumer<CustomConsumer>();
    
    x.UsingRabbitMq((context, cfg) => {
        cfg.ReceiveEndpoint("custom-message-queue", e => {
            e.ConfigureConsumer<CustomConsumer>(context);
            
            // 配置消费者选项
            e.PrefetchCount = 10;
            e.UseMessageRetry(r => r.Interval(3, 1000));
            e.DeadLetterQueueName = "custom-message-dead-letter";
        });
    });
});
```

### 创建自定义Saga状态机

```csharp
public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; }
    public string InventoryStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentCompleted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReserved, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCanceled, x => x.CorrelateById(context => context.Message.OrderId));
        
        Initially(
            When(OrderCreated)
                .Then(context => {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CustomerId = context.Message.CustomerId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(OrderReceived)
                .Send(new Uri("exchange:payment-commands"), context => new ProcessPayment {
                    OrderId = context.Message.OrderId,
                    Amount = context.Message.Amount
                }));
        
        During(OrderReceived,
            When(PaymentCompleted)
                .Then(context => {
                    context.Saga.PaymentStatus = "Completed";
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(PaymentConfirmed)
                .Send(new Uri("exchange:inventory-commands"), context => new ReserveInventory {
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Saga.CustomerId
                }),
            When(OrderCanceled)
                .Then(context => {
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Canceled)
                .Finalize());
        
        During(PaymentConfirmed,
            When(InventoryReserved)
                .Then(context => {
                    context.Saga.InventoryStatus = "Reserved";
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(ReadyForShipping)
                .Send(new Uri("exchange:shipping-commands"), context => new ShipOrder {
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Saga.CustomerId
                })
                .Finalize(),
            When(OrderCanceled)
                .Then(context => {
                    context.Saga.UpdatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Canceling)
                .Send(new Uri("exchange:payment-commands"), context => new RefundPayment {
                    OrderId = context.Message.OrderId,
                    Amount = context.Saga.Amount
                })
                .TransitionTo(Canceled)
                .Finalize());
        
        SetCompletedWhenFinalized();
    }
    
    public State OrderReceived { get; private set; }
    public State PaymentConfirmed { get; private set; }
    public State ReadyForShipping { get; private set; }
    public State Canceling { get; private set; }
    public State Canceled { get; private set; }
    
    public Event<OrderCreated> OrderCreated { get; private set; }
    public Event<PaymentCompleted> PaymentCompleted { get; private set; }
    public Event<InventoryReserved> InventoryReserved { get; private set; }
    public Event<OrderCanceled> OrderCanceled { get; private set; }
}
```

### 自定义消息序列化

```csharp
// 配置自定义消息序列化
builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) => {
        cfg.Host("localhost", h => {
            h.Username("guest");
            h.Password("guest");
        });
        
        // 配置消息序列化
        cfg.UseRawJsonSerializer();
        
        cfg.ConfigureEndpoints(context);
    });
});
```

## 监控和运维

### 健康检查

```csharp
// 添加健康检查
builder.Services.AddHealthChecks()
    .AddRabbitMQ("localhost", name: "rabbitmq")
    .AddRedis("localhost", name: "redis");

// 配置健康检查端点
app.MapHealthChecks("/health");
```

### 指标监控

```csharp
// 添加指标监控
builder.Services.AddMetrics();
builder.Services.AddMetricsEndpoint();

// 配置指标端点
app.MapMetrics("/metrics");
```

### 日志管理

```csharp
// 配置日志
builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug()
    .AddSeq("http://localhost:5341");
```

## 部署和扩展

### 容器化部署

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish -r linux-x64 --self-contained true -p:PublishAot=true -p:IncludeNativeLibrariesForSelfExtract=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./masstransit-service"]
```

### 扩展架构

1. **消息处理扩展**: 通过实现自定义消费者扩展消息处理能力
2. **传输扩展**: 添加新的消息传输实现
3. **序列化扩展**: 实现自定义消息序列化器
4. **监控扩展**: 集成第三方监控系统
5. **存储扩展**: 添加新的持久化存储实现

### 最佳实践

1. **消息设计**: 合理设计消息结构，避免消息过大
2. **错误处理**: 实现幂等性设计，正确处理错误情况
3. **监控告警**: 建立完善的监控和告警机制
4. **容量规划**: 根据业务需求合理规划系统容量
5. **灾备方案**: 制定完善的灾备方案
6. **文档管理**: 保持文档更新，便于维护和排查问题
7. **版本管理**: 合理管理消息版本，确保兼容性
8. **安全管理**: 加强消息安全，防止消息泄露和篡改

## 总结

MassTransit 技能为 .NET 开发者提供了强大的消息传递和分布式系统集成能力，支持多种消息 broker 和传输方式，提供了完整的 Saga 状态机编排功能，适合构建复杂的分布式系统和微服务架构。通过 AOT 编译优化，系统启动速度和运行性能得到显著提升，内存使用减少，部署更加简化。

本技能提供了完整的开发、部署和运维方案，开发者可以根据具体业务需求进行定制和扩展，构建高性能、可靠的分布式系统。

