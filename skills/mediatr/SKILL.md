# MediatR 智能体技能 - 中介者模式技能

## 技能概述

基于 .NET 10 的高性能 MediatR 中介者模式技能实现，为 .NET 开发者提供强大的消息处理功能。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package MediatR@12.0.0
```

### 注册服务

在主应用程序中注册 MediatR 服务：

```csharp
// 注册 MediatR 服务
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));
```

### 使用示例

```csharp
// 获取 MediatR 服务
var mediator = serviceProvider.GetRequiredService<IMediator>();

// 发送命令
var command = new CreateOrderCommand { OrderId = 1, CustomerId = 2, Amount = 100.0m };
var result = await mediator.Send(command);
Console.WriteLine($"命令执行结果: {result}");

// 发布事件
var @event = new OrderCreatedEvent { OrderId = 1, CustomerId = 2, Amount = 100.0m };
await mediator.Publish(@event);
Console.WriteLine("事件发布成功");
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

### 执行流程

1. **编译阶段**：使用 .NET 10 的 AOT 编译功能将代码编译为本地机器码
2. **打包阶段**：将编译后的代码打包为单文件可执行文件
3. **部署阶段**：将打包后的可执行文件部署到目标环境
4. **运行阶段**：执行单文件可执行文件，处理消息

## 导航地图

```
mediatr/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? *.cs                    # MediatR 核心实现
    ????? *.run.json              # 运行配置
    ????? *.setting.json          # 设置文件
```

## 主要功能

1. **命令处理**：处理各种命令消息
2. **查询处理**：处理各种查询消息
3. **事件发布/订阅**：发布和订阅各种事件
4. **管道行为**：支持中间件管道处理
5. **分布式系统集成**：支持与 MassTransit、Dapr、gRPC、Kafka、MQTT 等集成
6. **高性能设计**：优化的性能实现
7. **易于使用的 API**：简单直观的 API 设计
8. **可扩展架构**：支持自定义扩展

## 扩展说明

本技能提供了完整的 MediatR 解决方案，您可以根据需要进行扩展：

1. **自定义管道行为**：实现 IPipelineBehavior 接口
2. **扩展功能**：添加新的 MediatR 功能
3. **与其他系统集成**：与其他系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **模块化设计**：将消息处理逻辑模块化
7. **代码组织**：按功能组织代码结构
8. **测试覆盖**：为消息处理逻辑添加单元测试
