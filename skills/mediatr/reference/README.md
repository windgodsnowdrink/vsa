# MediatR - 参考文档

## 概述

MediatR 是基于 .NET 10 的高性能中介者模式系统，专为 .NET 开发者设计。它提供了一种松耦合的方式来处理应用程序中的消息传递，包括命令、查询和事件。

## 核心组件

### 1. IMediator 接口
- **位置**: scripts/*.cs
- **功能**: 核心消息处理接口
- **特性**: 
  - 支持命令处理
  - 支持查询处理
  - 支持事件发布/订阅
  - 支持管道行为
  - 异步编程模型

### 2. 管道行为
- **位置**: scripts/*.cs
- **功能**: 消息处理管道
- **特性**: 
  - 支持中间件
  - 支持异常处理
  - 支持日志记录
  - 支持性能监控

### 3. 分布式系统集成
- **位置**: scripts/*.cs
- **功能**: 与分布式系统集成
- **特性**: 
  - MassTransit 集成
  - Dapr 集成
  - gRPC 集成
  - Kafka 集成
  - MQTT 集成
  - Orleans 集成
  - SignalR 集成

## 使用示例

### 基本用法

```csharp
// 获取 IMediator 实例
var mediator = serviceProvider.GetRequiredService<IMediator>();

// 发送命令
var command = new CreateOrderCommand { OrderId = 1, CustomerId = 2, Amount = 100.0m };
var result = await mediator.Send(command);

// 发布事件
var @event = new OrderCreatedEvent { OrderId = 1, CustomerId = 2, Amount = 100.0m };
await mediator.Publish(@event);

// 发送查询
var query = new GetOrderQuery { OrderId = 1 };
var order = await mediator.Send(query);
```

### 高级配置

```csharp
// 注册 MediatR 服务
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

// 配置选项
builder.Services.Configure<MediatROptions>(options => {
    options.EnableDetailedLogging = true;
    options.EnablePerformanceMonitoring = true;
});
```

## 配置选项

### MediatR 配置

```json
{
  "MediatROptions": {
    "EnableDetailedLogging": true,          // 启用详细日志
    "EnablePerformanceMonitoring": true,     // 启用性能监控
    "MaxConcurrentRequests": 100,            // 最大并发请求数
    "RequestTimeout": "00:01:00"            // 请求超时时间
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理提高效率
4. **连接池**: 使用连接池管理资源
5. **内存优化**: 使用 Span<T> 和 Memory<T> 优化内存使用
6. **对象池**: 使用对象池减少 GC 压力
7. **并发优化**: 使用线程安全的集合和锁策略
8. **批处理优化**: 使用批量操作减少网络往返

## 故障排除

### 常见问题

1. **消息处理失败**
   - 检查命令/查询/事件的实现
   - 验证依赖注入配置
   - 检查日志信息

2. **性能问题**
   - 启用性能监控
   - 优化消息处理逻辑
   - 增加资源限制

3. **分布式集成问题**
   - 检查网络连接
   - 验证分布式系统配置
   - 检查相关服务状态

## 扩展开发

### 添加自定义管道行为

```csharp
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
        // 处理前日志
        _logger.LogInformation("处理请求: {RequestType}", typeof(TRequest).Name);

        var response = await next();

        // 处理后日志
        _logger.LogInformation("请求处理完成: {RequestType}", typeof(TRequest).Name);

        return response;
    }
}
```

### 添加自定义消息处理器

```csharp
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
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // 处理命令逻辑
        var order = new Order {
            Id = request.OrderId,
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            CreatedAt = DateTime.UtcNow
        };

        await _orderRepository.CreateAsync(order, cancellationToken);
        return true;
    }
}
```

## AOT 编译优化

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

### AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码
2. **避免动态类型**：使用强类型
3. **避免运行时代码生成**：使用预编译代码
4. **优化内存使用**：使用 Span<T> 和 Memory<T>
5. **减少依赖**：最小化依赖项

## 部署说明

### 部署步骤

1. **编译**：使用 .NET 10 SDK 编译代码
2. **打包**：打包为单文件可执行文件
3. **部署**：部署到目标环境
4. **配置**：配置环境变量和配置文件
5. **启动**：启动服务

### 环境要求

- .NET 10 运行时或更高版本
- 足够的内存和磁盘空间
- 网络连接（如果使用分布式系统集成）
