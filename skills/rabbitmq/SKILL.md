# rabbitmq Agent Skill - rabbitmq 技能

## 技能概述

基于 .NET 10 的高性能 RabbitMQ 技能，为 .NET 开发者提供强大的消息队列功能，支持消息发布/订阅、RPC 模式、工作队列等多种场景。

## 快速开始

### 安装依赖

在主应用程序的 runfile 中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package RabbitMQ.Client@6.8.1
#:package Polly@8.3.1
#:package System.Threading.Channels@8.0.0
```

### 注册服务

在主应用程序中注册 RabbitMQ 服务：

```csharp
// 注册 RabbitMQ 服务
builder.Services.AddRabbitMqServices(options => {
    options.HostName = "localhost";
    options.Port = 5672;
    options.UserName = "guest";
    options.Password = "guest";
    options.VirtualHost = "/";
    options.EnableConnectionPooling = true;
    options.MaxConnections = 10;
    options.EnableAutomaticRecovery = true;
    options.RequestedHeartbeat = TimeSpan.FromSeconds(60);
});
```

### 使用示例

```csharp
// 获取 RabbitMQ 服务
var rabbitMqService = serviceProvider.GetRequiredService<IRabbitMqService>();

// 发布消息
await rabbitMqService.PublishAsync("exchange-name", "routing-key", new {
    Message = "Hello, RabbitMQ!",
    Timestamp = DateTime.UtcNow
});

// 订阅消息
await rabbitMqService.SubscribeAsync("exchange-name", "queue-name", "routing-key", async (message) => {
    Console.WriteLine($"收到消息: {message}");
    // 处理消息
    return true; // 确认消息
});

// 使用 RPC 模式
var response = await rabbitMqService.RpcCallAsync<string, string>("rpc-queue", "请求数据");
Console.WriteLine($"RPC 响应: {response}");
```

## 导航地图

```
rabbitmq/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── rabbitmq_production_integration.cs     # RabbitMQ 核心实现
    ├── rabbitmq_production_integration.run.json  # 运行配置
    └── rabbitmq_production_integration.setting.json  # 设置文件
```

## 主要功能

1. **消息发布与订阅**：支持多种交换器类型（direct、fanout、topic、headers）
2. **RPC 模式支持**：实现请求-响应模式的消息传递
3. **工作队列实现**：支持任务分发和负载均衡
4. **死信队列支持**：处理失败或过期的消息
5. **延迟消息队列**：实现消息的延迟处理
6. **消息确认机制**：支持手动和自动消息确认
7. **集群支持**：适配 RabbitMQ 集群环境
8. **镜像队列配置**：提高消息可靠性
9. **高性能连接池**：优化连接管理，提高性能
10. **重试机制**：处理网络波动等临时故障
11. **熔断保护**：防止系统故障扩散
12. **批量消息处理**：提高处理效率
13. **消息压缩**：减少网络传输开销

## 扩展说明

本技能提供了完整的 RabbitMQ 解决方案，您可以根据需要进行扩展：

1. **自定义连接工厂**：实现 `IRabbitMqConnectionFactory` 接口
2. **扩展消息处理器**：实现自定义消息处理逻辑
3. **集成其他系统**：与其他消息系统或业务系统集成
4. **性能优化**：针对特定场景优化性能配置
5. **监控与告警**：添加自定义监控指标和告警机制

## AOT 编译配置

### 项目文件配置

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
    <PackageReference Include="RabbitMQ.Client" Version="6.8.1" />
    <PackageReference Include="Polly" Version="8.3.1" />
    <PackageReference Include="System.Threading.Channels" Version="8.0.0" />
  </ItemGroup>
</Project>
```

### 发布命令

```bash
# 发布为 AOT 编译的可执行文件
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true

# 发布为 Linux 版本
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true

# 发布为 macOS 版本
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishAot=true
```

## 故障排除

### 常见问题

1. **连接失败**
   - 检查 RabbitMQ 服务器是否运行
   - 验证连接参数是否正确
   - 检查网络连接和防火墙设置
   - 查看日志信息了解具体错误

2. **消息丢失**
   - 确保启用了消息确认机制
   - 检查队列是否持久化
   - 验证消息是否设置了持久化标志

3. **性能问题**
   - 调整连接池大小
   - 启用批量消息处理
   - 优化消息大小和频率
   - 考虑使用消息压缩

4. **内存使用过高**
   - 调整连接池大小
   - 优化消息处理速度
   - 考虑使用流控机制

5. **消费者过载**
   - 调整消费者数量
   - 实现背压机制
   - 考虑使用工作队列分散负载

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况，实现重试机制
4. **日志记录**：添加适当的日志记录，便于故障排查
5. **性能监控**：监控关键性能指标，如消息处理速度、队列长度等
6. **连接管理**：使用连接池优化连接管理
7. **消息设计**：合理设计消息结构，避免过大的消息
8. **序列化选择**：选择高效的序列化方式
9. **安全配置**：使用安全的连接参数，避免硬编码密码
10. **环境隔离**：为不同环境配置不同的 RabbitMQ 实例

## 系统要求

- **.NET 10.0** 或更高版本
- **RabbitMQ 3.8** 或更高版本
- **Erlang 23.0** 或更高版本
- **操作系统**：Windows、Linux、macOS

## 支持的平台

- **Windows**：完全支持
- **Linux**：完全支持
- **macOS**：完全支持

## 版本兼容性

| RabbitMQ.Client 版本 | .NET 版本 | 兼容性 |
|-------------------|----------|--------|
| 6.8.1 | net11.0 | 完全兼容 |
| 6.8.0 | net11.0 | 完全兼容 |
| 6.7.0 | net11.0 | 部分兼容 |
| 6.6.0 | net8.0 | 完全兼容 |
| 6.5.0 | net6.0 | 完全兼容 |

## 总结

rabbitmq 技能是一个功能强大、性能优异的消息队列解决方案，基于 .NET 10 和 AOT 编译技术。它提供了完整的 RabbitMQ 功能，包括消息发布/订阅、RPC 模式、工作队列等，并针对性能和可靠性进行了优化。通过本文档的指导，您可以快速上手并充分利用其功能，为您的应用程序添加可靠的消息传递能力。
