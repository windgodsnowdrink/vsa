# RabbitMQ 技能参考文档

## 概述

RabbitMQ 技能是一个基于 .NET 10 开发的高性能消息队列系统，专为 .NET 开发者设计。它支持 AOT 编译，提供了完整的 RabbitMQ 集成功能，包括消息发布/订阅、RPC 模式、工作队列、死信队列和延迟队列等。

## 核心组件

### 1. RabbitMqService
- **位置**: scripts/rabbitmq_production_integration.cs
- **功能**: RabbitMQ 核心业务逻辑处理
- **特性**: 
  - 消息发布与订阅
  - RPC 模式支持
  - 工作队列实现
  - 死信队列支持
  - 延迟队列支持
  - 消息序列化与压缩
  - 错误处理与重试机制
  - 连接池管理
  - 日志记录

### 2. RabbitMqConnectionPool
- **位置**: scripts/rabbitmq_production_integration.cs
- **功能**: 管理 RabbitMQ 连接池
- **特性**: 
  - 连接复用
  - 自动连接恢复
  - 连接健康检查
  - 连接生命周期管理

### 3. RabbitMqOptions
- **位置**: scripts/rabbitmq_production_integration.cs
- **功能**: RabbitMQ 配置选项
- **特性**: 
  - 连接配置
  - 重试策略配置
  - 消息处理配置
  - 性能优化配置

## 技术架构

### 分层架构
1. **接口层**: 定义核心服务接口
2. **实现层**: 提供具体实现
3. **配置层**: 管理运行时配置
4. **工具层**: 提供辅助功能

### 依赖注入
使用 Microsoft.Extensions.DependencyInjection 进行服务注册和管理，支持构造函数注入和选项模式。

### 性能优化
- **连接池**: 复用 RabbitMQ 连接，减少连接开销
- **异步编程**: 使用 async/await 模式，避免阻塞
- **批量处理**: 支持消息批量处理，提高效率
- **内存池**: 使用对象池减少内存分配
- **消息压缩**: 支持消息压缩，减少网络传输

### 容错机制
- **重试策略**: 使用 Polly 实现重试机制
- **断路器模式**: 使用 Polly 实现断路器模式
- **连接恢复**: 自动检测并恢复断开的连接
- **死信队列**: 处理无法消费的消息

## 配置选项

### RabbitMq 配置

```json
{
  "RabbitMq": {
    "HostName": "localhost",        // RabbitMQ 主机名
    "Port": 5672,                   // RabbitMQ 端口
    "UserName": "guest",            // 用户名
    "Password": "guest",            // 密码
    "VirtualHost": "/",             // 虚拟主机
    "EnableConnectionPooling": true, // 启用连接池
    "MaxConnections": 10,            // 最大连接数
    "ConnectionTimeout": 30,         // 连接超时时间（秒）
    "EnableAutomaticRecovery": true, // 启用自动恢复
    "RequestedHeartbeat": 60,        // 心跳间隔（秒）
    "EnableMessageCompression": false, // 启用消息压缩
    "EnableBatchProcessing": false,  // 启用批量处理
    "BatchSize": 100,                // 批量大小
    "BatchTimeout": 100,             // 批量超时时间（毫秒）
    "MaxRetries": 3,                 // 最大重试次数
    "RetryInterval": 500,            // 重试间隔（毫秒）
    "EnableCircuitBreaker": true,    // 启用断路器
    "CircuitBreakerFailureThreshold": 50, // 断路器失败阈值（百分比）
    "CircuitBreakerResetTimeout": 30 // 断路器重置超时时间（秒）
  }
}
```

### 性能配置

```json
{
  "Performance": {
    "EnableMemoryPooling": true,     // 启用内存池
    "MaxPoolSize": 100,              // 最大池大小
    "EnableThreadLocalStorage": true, // 启用线程本地存储
    "MaxDegreeOfParallelism": 4      // 最大并行度
  }
}
```

### 安全配置

```json
{
  "Security": {
    "EnableInputValidation": true,   // 启用输入验证
    "MaxMessageSize": 10485760,      // 最大消息大小（10MB）
    "AllowedContentTypes": [         // 允许的内容类型
      "application/json",
      "application/xml",
      "text/plain"
    ]
  }
}
```

## 性能优化

1. **连接池优化**
   - 调整 `MaxConnections` 以匹配应用需求
   - 启用 `EnableConnectionPooling` 减少连接开销

2. **消息处理优化**
   - 启用 `EnableBatchProcessing` 批量处理消息
   - 调整 `BatchSize` 和 `BatchTimeout` 以获得最佳性能

3. **内存管理优化**
   - 启用 `EnableMemoryPooling` 减少内存分配
   - 启用 `EnableThreadLocalStorage` 提高线程局部变量访问速度

4. **网络优化**
   - 启用 `EnableMessageCompression` 减少网络传输
   - 调整 `RequestedHeartbeat` 以平衡网络开销和连接可靠性

5. **并发优化**
   - 调整 `MaxDegreeOfParallelism` 以充分利用系统资源
   - 使用异步 API 避免阻塞

## 故障排除

### 常见问题

1. **连接失败**
   - 检查 RabbitMQ 服务是否运行
   - 验证连接配置（主机名、端口、用户名、密码）
   - 检查网络连接是否正常
   - 查看日志获取详细错误信息

2. **消息丢失**
   - 确保使用了持久化消息
   - 确保队列和交换机已正确声明
   - 检查消费者确认机制是否正确实现

3. **性能问题**
   - 启用连接池和内存池
   - 优化批量处理设置
   - 增加系统资源（CPU、内存）
   - 检查网络带宽是否足够

4. **消息重复**
   - 实现幂等性处理
   - 使用消息 ID 去重
   - 确保消费者确认机制正确实现

### 日志分析

RabbitMQ 技能使用标准的 .NET 日志系统，可通过配置 `Logging` 部分调整日志级别：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "RabbitMqSkill": "Debug"
    }
  }
}
```

## 扩展开发

### 添加自定义消息处理器

```csharp
public class CustomMessageHandler : IMessageHandler
{
    private readonly ILogger<CustomMessageHandler> _logger;

    public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleMessageAsync(string message, IDictionary<string, object> headers = null)
    {
        _logger.LogInformation("Processing custom message: {Message}", message);
        // 实现自定义消息处理逻辑
        await Task.CompletedTask;
    }
}
```

### 注册自定义处理器

```csharp
builder.Services.AddSingleton<IMessageHandler, CustomMessageHandler>();
```

### 扩展 RabbitMqService

```csharp
public class ExtendedRabbitMqService : RabbitMqService
{
    public ExtendedRabbitMqService(
        IRabbitMqConnectionPool connectionPool,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqService> logger)
        : base(connectionPool, options, logger)
    {
    }

    public async Task<string> CustomPublishAsync(string exchange, string routingKey, object message)
    {
        // 实现自定义发布逻辑
        return await PublishAsync(exchange, routingKey, message);
    }
}
```

## AOT 编译支持

### 项目配置

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
  </PropertyGroup>
</Project>
```

### 编译命令

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### AOT 优化注意事项

1. **避免反射**：使用静态代码生成或预编译序列化
2. **资源管理**：确保所有资源正确释放
3. **配置文件**：确保配置文件正确包含在发布中
4. **依赖项**：确保所有依赖项支持 AOT 编译

## 依赖项管理

### 核心依赖

- **Microsoft.Extensions.DependencyInjection**：依赖注入
- **Microsoft.Extensions.Options**：配置管理
- **Microsoft.Extensions.Logging**：日志记录
- **RabbitMQ.Client**：RabbitMQ 客户端
- **Polly**：重试和断路器模式
- **System.Threading.Channels**：通道实现
- **System.Text.Json**：JSON 序列化

### 版本要求

- **.NET**：10.0 或更高
- **RabbitMQ.Client**：6.8.1 或更高
- **Polly**：8.3.1 或更高

## 最佳实践

1. **使用依赖注入**：通过 DI 容器管理服务生命周期
2. **配置外部化**：使用配置文件管理运行时设置
3. **错误处理**：实现完善的错误处理和重试机制
4. **日志记录**：记录关键操作和错误信息
5. **性能监控**：监控系统性能和资源使用
6. **安全配置**：使用安全的连接配置
7. **幂等性设计**：确保消息处理的幂等性
8. **资源管理**：正确管理连接和通道资源
9. **扩展性考虑**：设计可扩展的架构
10. **测试覆盖**：编写充分的单元测试和集成测试

## 部署建议

1. **容器化部署**：使用 Docker 容器化应用
2. **环境隔离**：使用不同的配置文件隔离开发、测试和生产环境
3. **监控集成**：集成 Prometheus 或其他监控系统
4. **日志聚合**：使用 ELK 或其他日志聚合系统
5. **自动扩缩容**：根据负载自动调整实例数量

## 总结

RabbitMQ 技能是一个功能完整、性能优化的 RabbitMQ 集成方案，基于 .NET 10 和 AOT 编译技术，提供了丰富的消息处理功能和灵活的配置选项。通过合理配置和优化，可以满足各种复杂场景的需求，为 .NET 开发者提供高效、可靠的消息队列解决方案。
