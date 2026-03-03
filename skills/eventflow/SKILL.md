# EventFlow Agent Skill - EventFlow 技能

## 技能概述

基于 .NET 10 构建的高性能 EventFlow 技能，为 .NET 开发者提供强大的事件流功能。该技能采用 AOT（提前编译）架构，具有启动速度快、内存占用低、执行效率高等特点，适用于各种事件驱动架构场景。

## 快速开始指南

### 安装依赖

在您的主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在您的主应用程序中注册 EventFlow 服务：

```csharp
// 注册 EventFlow 服务
var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<EventFlowOptions>(builder.Configuration.GetSection("EventFlow"));
builder.Services.AddSingleton<IEventFlowService, EventFlowService>();
```

### 使用示例

```csharp
// 获取 EventFlow 服务
var serviceProvider = builder.Build().Services;
var eventFlowService = serviceProvider.GetRequiredService<IEventFlowService>();

// 使用 EventFlow 功能
var result = await eventFlowService.PublishEventAsync("UserCreated", "{\"userId\":\"123\",\"name\":\"测试用户\"}");
Console.WriteLine($"结果: {result.Success}");
```

### AOT 单文件执行

EventFlow 技能提供了 .NET 10 AOT 编译的单文件执行脚本，可以直接运行：

```bash
# 运行版本命令
./eventflow_aot version

# 创建事件
./eventflow_aot create UserCreated '{"userId":"123","name":"测试用户"}'

# 列出事件
./eventflow_aot list UserCreated 5

# 发布事件
./eventflow_aot publish OrderPlaced '{"orderId":"456","amount":100}'

# 订阅事件
./eventflow_aot subscribe UserCreated Subscriber1
```

## 导航地图

```
eventflow/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── eventflow_aot.cs        # EventFlow AOT 核心实现
    ├── eventflow_aot.run.json  # 运行配置
    ├── eventflow_aot.setting.json  # 应用程序设置
    ├── eventflow_service.cs     # EventFlow 服务实现
    ├── eventflow_service.run.json  # 服务运行配置
    ├── eventflow_service.setting.json  # 服务设置文件
    ├── eventsourcing_service.cs  # 事件溯源服务实现
    ├── eventsourcing_service.run.json  # 事件溯源运行配置
    ├── eventsourcing_service.setting.json  # 事件溯源设置文件
    ├── eventstore_service.cs     # 事件存储服务实现
    ├── eventstore_service.run.json  # 事件存储运行配置
    └── eventstore_service.setting.json  # 事件存储设置文件
```

## 主要特性

1. **事件创建与管理**：支持创建、获取、列出事件等核心操作
2. **事件发布与订阅**：实现了事件的发布订阅机制
3. **高性能 AOT 架构**：采用 .NET 10 AOT 编译，启动速度快，内存占用低
4. **多环境配置支持**：支持开发、测试、生产等多环境配置
5. **可扩展的事件存储**：支持多种事件存储类型，包括内存存储
6. **可扩展的事件总线**：支持多种事件总线类型，包括内存总线
7. **详细的日志记录**：支持详细的日志记录和性能监控
8. **简单易用的 API**：提供简洁直观的 API 设计
9. **事件压缩与加密**：支持事件数据的压缩和加密
10. **命令行工具**：提供功能完整的命令行工具

## 命令行使用

EventFlow AOT 引擎提供了丰富的命令行功能：

```bash
# 显示帮助信息
eventflow_aot help

# 显示版本信息
eventflow_aot version

# 创建事件
eventflow_aot create <eventType> <eventData> [source]

# 获取事件
eventflow_aot get <eventId>

# 列出事件
eventflow_aot list [eventType] [limit]

# 发布事件
eventflow_aot publish <eventType> <eventData> [source]

# 订阅事件
eventflow_aot subscribe <eventType> <subscriptionId>
```

## 扩展说明

EventFlow 技能提供了完整的事件流解决方案，您可以根据需要进行扩展：

1. **自定义事件存储**：实现自定义的事件存储逻辑
2. **自定义事件总线**：实现自定义的事件总线逻辑
3. **扩展事件类型**：添加新的事件类型和处理逻辑
4. **集成其他系统**：与其他系统进行集成
5. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：妥善处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **事件设计**：合理设计事件结构和命名
7. **事件版本管理**：妥善处理事件版本变更
8. **使用 AOT 编译**：对于生产环境，推荐使用 AOT 编译以获得最佳性能
9. **配置管理**：使用配置文件管理不同环境的设置
10. **安全考虑**：对于敏感事件数据，启用事件加密功能

## 性能特性

- **AOT 编译**：提前编译为本地代码，减少启动时间和内存占用
- **内存优化**：采用高效的内存管理策略
- **异步设计**：全异步 API 设计，提高并发处理能力
- **性能监控**：内置性能监控功能，便于分析和优化
- **事件压缩**：支持事件数据压缩，减少网络传输和存储开销

## 应用场景

1. **微服务架构**：用于微服务之间的事件通信
2. **事件驱动架构**：构建事件驱动的应用系统
3. **事件溯源**：实现领域驱动设计中的事件溯源模式
4. **实时数据处理**：处理实时数据流
5. **消息队列集成**：与各种消息队列系统集成
6. **分布式系统**：用于分布式系统中的状态同步
7. **业务流程管理**：实现复杂的业务流程
8. **监控与告警**：构建监控和告警系统
9. **数据分析**：用于数据采集和分析
10. **IoT 应用**：处理 IoT 设备产生的事件数据