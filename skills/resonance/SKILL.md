# resonance 技能

## 技能概述

基于 .NET 10 和 AOT 编译的高性能 resonance 技能，为 .NET 开发者提供强大的动态路由、插件系统和消息处理功能。该技能采用模块化设计，支持高并发、异步编程和零拷贝内存操作，适用于构建可扩展的服务框架和高性能消息处理系统。

## 快速入门指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Collections.Concurrent@8.0.0
#:package System.Threading.Channels@8.0.0
```

### 注册服务

在主应用中注册 resonance 服务：

```csharp
// 构建服务容器
var services = new ServiceCollection();

// 配置日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册 resonance 服务
services.AddResonanceServices(options =>
{
    options.RoutePrefix = "/api";
    options.MaxConcurrentRequests = 100;
    options.BufferSize = 1024;
    options.EnableTelemetry = true;
});

var serviceProvider = services.BuildServiceProvider();

// 获取 dynamic router 服务
var dynamicRouter = serviceProvider.GetRequiredService<IDynamicRouter>();

// 获取 resonance adapter 服务
var resonanceAdapter = serviceProvider.GetRequiredService<IResonanceAdapter>();
```

### 使用示例

#### 动态路由示例

```csharp
// 注册路由
dynamicRouter.RegisterRoute("GET", "/users/{id}", async (context) =>
{
    var id = context.RouteParameters["id"];
    return new { UserId = id, Name = "Test User" };
});

// 匹配和执行路由
var routeContext = new RouteContext(HttpMethod.Get, "/api/users/123");
var result = await dynamicRouter.MatchAndExecuteAsync(routeContext);

Console.WriteLine($"路由执行结果: {result}");
```

#### 消息处理示例

```csharp
// 发送消息
var message = new ResonanceMessage { Id = Guid.NewGuid(), Content = "Test message" };
var response = await resonanceAdapter.SendMessageAsync(message);

Console.WriteLine($"消息发送结果: {response.Success}");

// 注册消息处理器
resonanceAdapter.RegisterMessageHandler<TestMessage>(async (message) =>
{
    Console.WriteLine($"收到测试消息: {message.Content}");
    return new ResonanceMessageResponse { Success = true, Result = "Message processed" };
});
```

## 导航地图

```
resonance/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── dynamic_router.cs       # 动态路由实现
    ├── dynamic_router.run.json  # 运行配置
    ├── dynamic_router.setting.json  # 设置文件
    ├── resonance_adapter.cs    # resonance 适配器实现
    ├── resonance_adapter.run.json  # 运行配置
    └── resonance_adapter.setting.json  # 设置文件
```

## 核心功能

### 1. 动态路由系统
- **路由注册**：支持 HTTP 方法和路径模式的路由注册
- **路由匹配**：高性能路由匹配算法，支持路径参数
- **路由执行**：异步执行路由处理函数
- **中间件支持**：可扩展的中间件系统，用于请求预处理和后处理

### 2. 插件系统
- **插件发现**：自动发现和加载插件
- **插件注册**：支持插件的注册和管理
- **插件生命周期**：完整的插件生命周期管理
- **依赖注入**：插件支持依赖注入

### 3. 消息处理管道
- **消息发送**：异步消息发送和响应
- **消息处理**：基于类型的消息处理器注册
- **消息路由**：根据消息类型路由到相应的处理器
- **消息转换**：支持消息的序列化和反序列化

### 4. 服务注册与解析
- **服务注册**：支持单例、作用域和瞬时服务的注册
- **服务解析**：支持构造函数注入和属性注入
- **服务生命周期**：完整的服务生命周期管理
- **服务装饰器**：支持服务装饰器模式

### 5. 事件驱动架构
- **事件发布**：支持事件的发布和订阅
- **事件处理**：异步事件处理
- **事件过滤**：支持事件过滤和路由
- **事件溯源**：支持事件溯源模式

### 6. 高性能消息分发
- **消息队列**：基于 System.Threading.Channels 的高性能消息队列
- **并发处理**：支持并行消息处理
- **背压处理**：内置背压机制，防止系统过载
- **批处理**：支持消息批处理

### 7. 可扩展中间件系统
- **中间件注册**：支持全局和路由级中间件
- **中间件执行**：异步中间件执行管道
- **中间件顺序**：可配置的中间件执行顺序
- **中间件组合**：支持中间件的组合和嵌套

### 8. 配置管理
- **配置加载**：支持从文件、环境变量和命令行加载配置
- **配置监控**：支持配置变更的实时监控
- **配置验证**：支持配置的验证和默认值
- **配置加密**：支持敏感配置的加密

## API 参考

### 动态路由接口

```csharp
public interface IDynamicRouter
{
    // 注册路由
    void RegisterRoute(string httpMethod, string path, Func<RouteContext, Task<object>> handler);
    
    // 注册路由（带中间件）
    void RegisterRoute(string httpMethod, string path, Func<RouteContext, Task<object>> handler, params Func<RouteContext, Func<RouteContext, Task<object>>, Task<object>>[] middleware);
    
    // 匹配和执行路由
    Task<object> MatchAndExecuteAsync(RouteContext context);
    
    // 获取所有注册的路由
    IEnumerable<RouteInfo> GetRoutes();
    
    // 移除路由
    void RemoveRoute(string httpMethod, string path);
    
    // 清空所有路由
    void ClearRoutes();
}

public class RouteContext
{
    public HttpMethod HttpMethod { get; set; }
    public string Path { get; set; }
    public Dictionary<string, string> RouteParameters { get; set; }
    public Dictionary<string, string> QueryParameters { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public object Body { get; set; }
}

public class RouteInfo
{
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public int MiddlewareCount { get; set; }
}
```

### Resonance 适配器接口

```csharp
public interface IResonanceAdapter
{
    // 发送消息
    Task<ResonanceMessageResponse> SendMessageAsync(ResonanceMessage message, CancellationToken cancellationToken = default);
    
    // 注册消息处理器
    void RegisterMessageHandler<TMessage>(Func<TMessage, Task<ResonanceMessageResponse>> handler) where TMessage : ResonanceMessage;
    
    // 取消注册消息处理器
    void UnregisterMessageHandler<TMessage>() where TMessage : ResonanceMessage;
    
    // 启动消息处理
    Task StartAsync(CancellationToken cancellationToken = default);
    
    // 停止消息处理
    Task StopAsync(CancellationToken cancellationToken = default);
    
    // 获取消息处理统计
    MessageProcessingStats GetStats();
}

public class ResonanceMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public object Content { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class ResonanceMessageResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public object? Result { get; set; }
    public DateTime Timestamp { get; set; }
}

public class MessageProcessingStats
{
    public long TotalMessages { get; set; }
    public long SuccessfulMessages { get; set; }
    public long FailedMessages { get; set; }
    public double AverageProcessingTimeMs { get; set; }
    public int ActiveHandlers { get; set; }
}
```

## 扩展说明

### 自定义路由处理器

```csharp
// 自定义路由处理器
public class CustomRouteHandler : IRouterHandler
{
    public Task<object> HandleAsync(RouteContext context)
    {
        // 自定义处理逻辑
        return Task.FromResult<object>(new { Custom = "Handler", Path = context.Path });
    }
}

// 注册自定义路由处理器
var handler = new CustomRouteHandler();
dynamicRouter.RegisterRoute("GET", "/custom", handler.HandleAsync);
```

### 自定义中间件

```csharp
// 自定义中间件
public class AuthenticationMiddleware
{
    public async Task<object> InvokeAsync(RouteContext context, Func<RouteContext, Task<object>> next)
    {
        // 验证身份
        if (!context.Headers.TryGetValue("Authorization", out var token))
        {
            return new { Error = "Unauthorized" };
        }
        
        // 调用下一个中间件
        return await next(context);
    }
}

// 使用自定义中间件
var authMiddleware = new AuthenticationMiddleware();
dynamicRouter.RegisterRoute("GET", "/protected", async (context) =>
{
    return new { Protected = "Resource" };
}, authMiddleware.InvokeAsync);
```

### 自定义消息处理器

```csharp
// 自定义消息
public class OrderMessage : ResonanceMessage
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
}

// 注册自定义消息处理器
resonanceAdapter.RegisterMessageHandler<OrderMessage>(async (message) =>
{
    // 处理订单消息
    Console.WriteLine($"处理订单: {message.OrderId}, 金额: {message.Amount}");
    
    // 模拟处理
    await Task.Delay(100);
    
    return new ResonanceMessageResponse
    {
        Success = true,
        Result = new { OrderId = message.OrderId, Processed = true }
    };
});

// 发送自定义消息
var orderMessage = new OrderMessage
{
    Id = Guid.NewGuid(),
    Type = "Order",
    OrderId = "ORD-12345",
    Amount = 99.99m,
    Status = "Pending"
};

var response = await resonanceAdapter.SendMessageAsync(orderMessage);
```

## 最佳实践

### 1. 路由设计最佳实践
- **使用 RESTful 路径**：遵循 RESTful 设计原则，使用名词表示资源
- **避免深度嵌套**：路径深度不宜过深，一般不超过 3 层
- **使用小写字母**：路径和参数名使用小写字母，单词之间用连字符分隔
- **合理使用参数**：路径参数用于标识资源，查询参数用于过滤和排序

### 2. 消息处理最佳实践
- **消息序列化**：使用高效的序列化格式，如 JSON 或 Protocol Buffers
- **消息大小限制**：避免发送过大的消息，建议不超过 1MB
- **消息幂等性**：确保消息处理是幂等的，避免重复处理导致副作用
- **消息超时**：设置合理的消息处理超时时间

### 3. 性能优化最佳实践
- **使用异步操作**：优先使用异步方法，避免阻塞线程
- **合理设置并发限制**：根据系统资源设置合理的并发请求限制
- **使用对象池**：对于频繁创建和销毁的对象，使用对象池减少垃圾回收
- **避免锁竞争**：减少锁的范围和竞争，使用无锁数据结构

### 4. 监控与日志最佳实践
- **结构化日志**：使用结构化日志记录操作和错误
- **关键指标监控**：监控请求响应时间、消息处理延迟、错误率等关键指标
- **健康检查**：定期执行健康检查，确保系统正常运行
- **告警设置**：设置合理的告警阈值，及时发现和处理异常

### 5. 安全性最佳实践
- **输入验证**：对所有输入进行验证，防止注入攻击
- **输出编码**：对输出进行编码，防止 XSS 攻击
- **敏感信息保护**：避免在日志中记录敏感信息
- **权限控制**：实现细粒度的权限控制，确保资源安全

## AOT 编译支持

### 编译配置

resonance 技能完全支持 AOT (Ahead-of-Time) 编译，通过以下配置实现：

```json
{
  "compilation": {
    "targetFramework": "net10.0",
    "publishAot": true,
    "trimMode": "partial",
    "readyToRun": true,
    "tieredCompilation": true,
    "optimize": true,
    "enableCompressionInSingleFile": true,
    "selfContained": true
  }
}
```

### AOT 兼容性

- **使用 trim-safe 的 API**：避免使用反射、动态类型等不兼容 AOT 的特性
- **显式类型标注**：使用 `[DynamicallyAccessedMembers]` 等属性标注需要保留的类型
- **资源管理**：确保资源正确释放，避免内存泄漏
- **序列化兼容**：使用 AOT 兼容的序列化库

### 性能优势

- **启动速度快**：AOT 编译消除了 JIT 编译的开销，启动速度显著提升
- **内存占用低**：Trim 模式移除了未使用的代码和类型，减少内存占用
- **运行时性能**：静态编译的代码执行效率更高，特别是对于热点路径
- **部署简单**：自包含部署减少了依赖项，部署更加简单可靠

## 故障排除

### 1. 路由匹配失败

**症状**：路由请求返回 404 错误

**可能原因**：
- 路由路径不匹配
- HTTP 方法不正确
- 路由前缀配置错误

**解决方案**：
- 检查路由注册的路径和 HTTP 方法
- 验证请求的完整路径，包括路由前缀
- 使用 `GetRoutes()` 方法查看所有注册的路由

### 2. 消息处理失败

**症状**：消息发送后返回失败响应

**可能原因**：
- 消息处理器未注册
- 消息处理抛出异常
- 消息序列化失败

**解决方案**：
- 确保已注册对应类型的消息处理器
- 检查消息处理器中的异常处理
- 验证消息内容是否可序列化

### 3. 性能下降

**症状**：系统响应时间变长，吞吐量下降

**可能原因**：
- 并发请求过多
- 消息队列积压
- 资源泄漏

**解决方案**：
- 调整 `MaxConcurrentRequests` 配置
- 增加 `BufferSize` 配置
- 使用性能分析工具查找瓶颈
- 检查是否有资源未正确释放

### 4. AOT 编译错误

**症状**：AOT 编译失败，出现 trim 警告或错误

**可能原因**：
- 使用了不兼容 AOT 的 API
- 反射使用不当
- 动态类型或表达式树使用

**解决方案**：
- 替换为 trim-safe 的 API
- 使用 `[DynamicallyAccessedMembers]` 属性标注需要保留的类型
- 避免使用动态类型和表达式树
- 参考 AOT 编译文档，了解兼容的 API

### 5. 内存泄漏

**症状**：系统内存占用持续增长，不释放

**可能原因**：
- 对象未正确释放
- 事件订阅未取消
- 长时间运行的任务未完成

**解决方案**：
- 使用内存分析工具查找泄漏点
- 确保所有 IDisposable 对象都被正确释放
- 及时取消事件订阅
- 为长时间运行的任务设置超时

## 总结

resonance 技能是一个功能强大、性能优异的动态路由和消息处理框架，基于 .NET 10 和 AOT 编译技术，为开发者提供了构建可扩展服务框架和高性能消息处理系统的完整解决方案。通过合理使用其核心功能和最佳实践，开发者可以构建更加可靠、高效的分布式系统和微服务应用。

该技能的主要优势包括：

1. **高性能**：基于 .NET 10 和 AOT 编译，提供卓越的性能和启动速度
2. **可扩展**：模块化设计，支持插件和中间件扩展
3. **灵活**：动态路由和消息处理系统，适应各种复杂场景
4. **可靠**：内置错误处理、重试机制和健康检查
5. **易于使用**：简洁的 API 设计，丰富的文档和示例

resonance 技能为 .NET 开发者提供了一个强大的工具，帮助他们构建下一代高性能、可扩展的应用系统。