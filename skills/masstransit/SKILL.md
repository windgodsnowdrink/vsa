# MassTransit 智能体技能 - 消息总线集成技能

## 技能概述

基于 .NET 10 的高性能 MassTransit 消息总线集成技能，为 .NET 开发者提供强大的消息传递和分布式系统集成功能，支持多种消息 broker 和 Saga 状态机编排。

## 快速开始指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package MassTransit@8.0.0
#:package MassTransit.RabbitMQ@8.0.0
#:package MassTransit.LiteDB@8.0.0
#:package MQTTnet@4.0.0
```

### 注册服务

在主应用中注册 MassTransit 服务：

```csharp
// 注册 MassTransit 服务
builder.Services.AddMassTransit(x => {
    x.UsingRabbitMq((context, cfg) => {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

// 注册 MassTransit 宿主服务
builder.Services.AddMassTransitHostedService();
```

### 使用示例

```csharp
// 获取 MassTransit 发送器
var sendEndpointProvider = serviceProvider.GetRequiredService<ISendEndpointProvider>();
var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("exchange:order-commands"));

// 发送消息
await endpoint.Send(new CreateOrder {
    OrderId = Guid.NewGuid(),
    CustomerId = 1,
    TotalAmount = 100.00
});

Console.WriteLine("订单创建消息已发送");
```

## AOT 架构执行

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

### 高性能消息处理

MassTransit 技能使用多种高性能消息传递技术，提供不同场景下的最佳解决方案：

```csharp
// 配置高性能消息处理
cfg.EnableScopedSendFilters();
cfg.EnableScopedPublishFilters();
cfg.UseMessageRetry(r => r.Immediate(3));
cfg.UseCircuitBreaker(cb => {
    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
    cb.TripThreshold = 15;
    cb.ActiveThreshold = 10;
    cb.ResetInterval = TimeSpan.FromMinutes(5);
});

// 配置消息批处理
cfg.UseBatching(b => {
    b.MessageLimit = 100;
    b.TimeLimit = TimeSpan.FromMilliseconds(50);
    b.SizeLimit = 1024 * 1024; // 1MB
});
```

## 导航地图

```
masstransit/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── masstransit_integration.cs     # MassTransit 核心集成
    ├── masstransit_integration.run.json  # 运行配置
    ├── masstransit_integration.setting.json  # 设置文件
    ├── masstransit_production_integration.cs     # 生产环境集成
    ├── masstransit_production_integration.run.json  # 运行配置
    ├── masstransit_production_integration.setting.json  # 设置文件
    ├── masstransit_inmemory_integration.cs     # 内存模式集成
    ├── masstransit_inmemory_integration.run.json  # 运行配置
    ├── masstransit_inmemory_integration.setting.json  # 设置文件
    ├── masstransit_litedb_integration.cs     # LiteDB 集成
    ├── masstransit_litedb_integration.run.json  # 运行配置
    ├── masstransit_litedb_integration.setting.json  # 设置文件
    ├── masstransit_mqttnet_integration.cs     # MQTTnet 集成
    ├── masstransit_mqttnet_integration.run.json  # 运行配置
    ├── masstransit_mqttnet_integration.setting.json  # 设置文件
    ├── saga_orchestrator.cs     # Saga 编排器
    ├── saga_orchestrator.run.json  # 运行配置
    ├── saga_orchestrator.setting.json  # 设置文件
    ├── masuit_tools_integration.cs     # Masuit 工具集成
    ├── masuit_tools_integration.run.json  # 运行配置
    └── masuit_tools_integration.setting.json  # 设置文件
```

## 主要功能

1. **消息发布和订阅**：支持发布/订阅模式的消息传递
2. **命令发送**：支持基于命令模式的消息传递
3. **Saga 状态机编排**：支持复杂业务流程的状态机编排
4. **分布式事务处理**：支持基于 Saga 模式的分布式事务
5. **消息重试和错误处理**：内置消息重试机制和错误处理策略
6. **多种消息 broker 集成**：支持 RabbitMQ、Azure Service Bus、Kafka 等
7. **MQTT 集成**：支持 IoT 场景的 MQTT 消息传递
8. **LiteDB 持久化**：支持 Saga 状态的 LiteDB 持久化

## 扩展说明

本技能提供了完整的 MassTransit 解决方案，您可以根据需要进行扩展：

1. **自定义消息处理器**：实现 IConsumer 接口创建自定义消息处理器
2. **扩展 Saga 状态机**：创建复杂的业务流程状态机
3. **系统集成**：与其他系统集成，如数据库、缓存、外部 API 等
4. **性能优化**：针对特定场景优化消息处理性能
5. **监控和日志**：集成监控和日志系统

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期
2. **异步编程**：优先使用异步 API 避免线程阻塞
3. **错误处理**：正确处理异常情况，实现幂等性设计
4. **日志记录**：添加适当的日志记录，便于问题排查
5. **性能监控**：监控关键性能指标，如消息处理延迟、吞吐量等
6. **AOT 编译**：使用 AOT 编译提升启动速度和运行性能
7. **内存优化**：使用对象池和 Span 零拷贝技术优化内存使用
8. **消息设计**：合理设计消息结构，避免消息过大
9. **重试策略**：根据业务场景选择合适的重试策略
10. **死信处理**：实现死信队列处理机制

## 应用场景

1. **微服务通信**：在微服务架构中实现服务间通信
2. **事件驱动架构**：构建基于事件的分布式系统
3. **工作流编排**：使用 Saga 状态机编排复杂业务流程
4. **IoT 消息传递**：通过 MQTT 集成处理 IoT 设备消息
5. **后台任务处理**：实现可靠的后台任务处理
6. **分布式系统集成**：集成不同系统间的消息传递

## 技术优势

1. **高性能设计**：优化的消息处理管道，支持高并发场景
2. **可靠性保证**：内置消息重试、死信处理等机制
3. **灵活性**：支持多种消息 broker 和传输方式
4. **可扩展性**：模块化设计，易于扩展和定制
5. **开发效率**：简化消息传递代码，提高开发效率
6. **AOT 优化**：支持 .NET 10 AOT 编译，提升性能

