# Resonance 使用示例

## 1. 动态路由示例

### 1.1 基本路由注册

```csharp
// 注册基本路由
dynamicRouter.RegisterRoute("GET", "/users", async (context) =>
{
    return new { Users = new[] { "User1", "User2", "User3" } };
});

// 注册带路径参数的路由
dynamicRouter.RegisterRoute("GET", "/users/{id}", async (context) =>
{
    var id = context.RouteParameters["id"];
    return new { UserId = id, Name = "Test User" };
});

// 注册 POST 路由
dynamicRouter.RegisterRoute("POST", "/users", async (context) =>
{
    return new { Id = Guid.NewGuid(), Name = "New User", Created = DateTime.UtcNow };
});
```

### 1.2 带中间件的路由

```csharp
// 注册带中间件的路由
dynamicRouter.RegisterRoute("GET", "/protected", async (context) =>
{
    return new { Protected = "Resource", User = "Authenticated" };
},
// 认证中间件
async (ctx, next) =>
{
    Console.WriteLine("[Middleware] 认证检查");
    // 模拟认证通过
    return await next(ctx);
},
// 日志中间件
async (ctx, next) =>
{
    var startTime = DateTime.UtcNow;
    Console.WriteLine($"[Middleware] 开始处理请求: {ctx.Path}");
    var result = await next(ctx);
    var executionTime = DateTime.UtcNow - startTime;
    Console.WriteLine($"[Middleware] 请求处理完成: {ctx.Path} - {executionTime.TotalMilliseconds:F2}ms");
    return result;
});
```

### 1.3 路由执行

```csharp
// 执行 GET 请求
var getContext = new RouteContext
{
    HttpMethod = HttpMethod.Get,
    Path = "/api/users/123"
};
var getResult = await dynamicRouter.MatchAndExecuteAsync(getContext);
Console.WriteLine($"GET /api/users/123: {getResult}");

// 执行 POST 请求
var postContext = new RouteContext
{
    HttpMethod = HttpMethod.Post,
    Path = "/api/users",
    Body = new { Name = "John Doe", Age = 30 }
};
var postResult = await dynamicRouter.MatchAndExecuteAsync(postContext);
Console.WriteLine($"POST /api/users: {postResult}");
```

### 1.4 路由管理

```csharp
// 获取所有注册的路由
var routes = dynamicRouter.GetRoutes();
Console.WriteLine("注册的路由:");
foreach (var route in routes)
{
    Console.WriteLine($"- {route.HttpMethod} {route.Path} (中间件: {route.MiddlewareCount})");
}

// 移除路由
dynamicRouter.RemoveRoute("GET", "/users/{id}");

// 清空所有路由
dynamicRouter.ClearRoutes();

// 获取路由执行统计
var stats = dynamicRouter.GetStats();
Console.WriteLine("路由执行统计:");
Console.WriteLine($"总请求数: {stats.TotalRequests}");
Console.WriteLine($"成功请求数: {stats.SuccessfulRequests}");
Console.WriteLine($"失败请求数: {stats.FailedRequests}");
Console.WriteLine($"平均执行时间: {stats.AverageExecutionTimeMs:F2}ms");
Console.WriteLine($"活跃路由数: {stats.ActiveRoutes}");
```

## 2. 消息处理示例

### 2.1 基本消息发送

```csharp
// 定义消息类型
record UserMessage(string Name, int Age);
record OrderMessage(Guid OrderId, decimal Amount, string Status);

// 注册消息处理器
resonanceAdapter.RegisterHandler<UserMessage>(async (context, message) =>
{
    Console.WriteLine($"[Handler] 处理用户消息: {message.Name}, {message.Age}");
    return new MessageResult
    {
        Success = true,
        Result = new { Processed = true, User = message.Name },
        ExecutionTime = TimeSpan.FromMilliseconds(50)
    };
});

// 发送消息
var userResult = await resonanceAdapter.SendAsync("user-service", new UserMessage("张三", 30), "demo-client");
Console.WriteLine($"用户消息发送结果: {userResult.Success} - {userResult.Result}");
```

### 2.2 带中间件的消息处理

```csharp
// 注册带中间件的消息处理器
resonanceAdapter.RegisterHandler("System.Heartbeat", async (context) =>
{
    Console.WriteLine($"[Handler] 处理心跳消息: {context.MessageId}");
    return new MessageResult
    {
        Success = true,
        Result = new { Status = "Alive", Timestamp = DateTime.UtcNow },
        ExecutionTime = TimeSpan.FromMilliseconds(10)
    };
},
// 日志中间件
MessageMiddleware.Logging,
// 超时中间件
MessageMiddleware.Timeout(1000)
);

// 发送心跳消息
var heartbeatContext = new MessageContext
{
    Source = "demo-client",
    Destination = "system-service",
    Type = "System.Heartbeat",
    Body = new { Timestamp = DateTime.UtcNow }
};
var heartbeatResult = await resonanceAdapter.SendAsync(heartbeatContext);
Console.WriteLine($"心跳消息发送结果: {heartbeatResult.Success} - {heartbeatResult.Result}");
```

### 2.3 消息处理器管理

```csharp
// 获取所有注册的处理器
var handlers = resonanceAdapter.GetRegisteredHandlers();
Console.WriteLine("注册的消息处理器:");
foreach (var handler in handlers)
{
    Console.WriteLine($"- {handler}");
}

// 取消注册消息处理器
resonanceAdapter.UnregisterHandler("System.Heartbeat");
```

### 2.4 消息处理统计

```csharp
// 获取消息处理统计
var stats = resonanceAdapter.GetStats();
Console.WriteLine("消息处理统计:");
Console.WriteLine($"总消息数: {stats.TotalMessages}");
Console.WriteLine($"成功消息数: {stats.SuccessfulMessages}");
Console.WriteLine($"失败消息数: {stats.FailedMessages}");
Console.WriteLine($"重试消息数: {stats.RetriedMessages}");
Console.WriteLine($"平均处理时间: {stats.AverageProcessingTimeMs:F2}ms");
Console.WriteLine($"活跃处理器数: {stats.ActiveHandlers}");
Console.WriteLine($"队列深度: {stats.QueueDepth}");
```

## 3. 完整集成示例

### 3.1 ASP.NET Core 集成

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 注册动态路由服务
builder.Services.AddDynamicRouter(options =>
{
    options.RoutePrefix = "/api";
    options.MaxConcurrentRequests = 100;
    options.EnableTelemetry = true;
});

// 注册消息处理服务
builder.Services.AddResonanceAdapter(options =>
{
    options.MaxMessageSize = 1024 * 1024;
    options.QueueCapacity = 1000;
    options.WorkerCount = Environment.ProcessorCount;
    options.RetryCount = 3;
    options.RetryDelayMs = 100;
    options.EnableDeadLetterQueue = true;
});

var app = builder.Build();

// 获取服务
var dynamicRouter = app.Services.GetRequiredService<IDynamicRouter>();
var resonanceAdapter = app.Services.GetRequiredService<IResonanceAdapter>();

// 启动消息处理适配器
await resonanceAdapter.StartAsync();

// 注册路由
dynamicRouter.RegisterRoute("GET", "/users", async (context) =>
{
    return new { Users = new[] { "User1", "User2", "User3" } };
});

// 处理 HTTP 请求
app.Map("/{**path}", async (HttpContext httpContext) =>
{
    var path = httpContext.Request.Path.Value;
    var method = httpContext.Request.Method;
    
    // 构建路由上下文
    var routeContext = new RouteContext
    {
        HttpMethod = new HttpMethod(method),
        Path = path,
        QueryParameters = httpContext.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()),
        Headers = httpContext.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
    };
    
    try
    {
        // 执行路由
        var result = await dynamicRouter.MatchAndExecuteAsync(routeContext);
        await httpContext.Response.WriteAsJsonAsync(result);
    }
    catch (Exception ex)
    {
        httpContext.Response.StatusCode = 404;
        await httpContext.Response.WriteAsJsonAsync(new { Error = ex.Message });
    }
});

// 运行应用
app.Run();
```

### 3.2 控制台应用集成

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 构建服务容器
var services = new ServiceCollection();

// 配置日志
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册动态路由服务
services.AddDynamicRouter(options =>
{
    options.RoutePrefix = "/api";
    options.MaxConcurrentRequests = 100;
    options.EnableTelemetry = true;
});

// 注册消息处理服务
services.AddResonanceAdapter(options =>
{
    options.MaxMessageSize = 1024 * 1024;
    options.QueueCapacity = 1000;
    options.WorkerCount = Environment.ProcessorCount;
    options.RetryCount = 3;
    options.RetryDelayMs = 100;
    options.EnableDeadLetterQueue = true;
});

var serviceProvider = services.BuildServiceProvider();

// 获取服务
var dynamicRouter = serviceProvider.GetRequiredService<IDynamicRouter>();
var resonanceAdapter = serviceProvider.GetRequiredService<IResonanceAdapter>();

// 启动消息处理适配器
await resonanceAdapter.StartAsync();

Console.WriteLine("Resonance Demo Application");
Console.WriteLine("=" + new string('=', 50));

// 注册路由
dynamicRouter.RegisterRoute("GET", "/users/{id}", async (context) =>
{
    var id = context.RouteParameters["id"];
    return new { UserId = id, Name = "Test User" };
});

// 注册消息处理器
record TestMessage(string Content);
resonanceAdapter.RegisterHandler<TestMessage>(async (context, message) =>
{
    Console.WriteLine($"[Handler] 处理测试消息: {message.Content}");
    return new MessageResult
    {
        Success = true,
        Result = new { Processed = true, Content = message.Content },
        ExecutionTime = TimeSpan.FromMilliseconds(20)
    };
});

// 测试路由
Console.WriteLine("\n测试路由:");
var routeContext = new RouteContext
{
    HttpMethod = HttpMethod.Get,
    Path = "/api/users/123"
};
var routeResult = await dynamicRouter.MatchAndExecuteAsync(routeContext);
Console.WriteLine($"GET /api/users/123: {routeResult}");

// 测试消息处理
Console.WriteLine("\n测试消息处理:");
var messageResult = await resonanceAdapter.SendAsync("test-service", new TestMessage("Hello Resonance"), "demo-client");
Console.WriteLine($"消息发送结果: {messageResult.Success} - {messageResult.Result}");

// 停止消息处理适配器
await resonanceAdapter.StopAsync();

// 释放资源
if (serviceProvider is IDisposable disposable)
{
    disposable.Dispose();
}
```

## 4. 高级用法

### 4.1 自定义中间件

```csharp
// 自定义错误处理中间件
var errorHandlingMiddleware = async (context, next) =>
{
    try
    {
        return await next(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Middleware] 错误处理: {ex.Message}");
        return new MessageResult
        {
            Success = false,
            ErrorMessage = ex.Message,
            ExecutionTime = TimeSpan.Zero
        };
    }
};

// 注册带自定义中间件的路由
dynamicRouter.RegisterRoute("GET", "/error-prone", async (context) =>
{
    throw new InvalidOperationException("故意抛出的错误");
}, errorHandlingMiddleware);

// 注册带自定义中间件的消息处理器
resonanceAdapter.RegisterHandler("ErrorTest", async (context) =>
{
    throw new InvalidOperationException("故意抛出的错误");
}, errorHandlingMiddleware);
```

### 4.2 批量消息处理

```csharp
// 批量发送消息
var messages = new List<TestMessage>
{
    new TestMessage("Message 1"),
    new TestMessage("Message 2"),
    new TestMessage("Message 3")
};

var tasks = messages.Select(msg => resonanceAdapter.SendAsync("test-service", msg, "demo-client")).ToList();
var results = await Task.WhenAll(tasks);

for (int i = 0; i < results.Length; i++)
{
    Console.WriteLine($"消息 {i + 1} 结果: {results[i].Success} - {results[i].Result}");
}
```

### 4.3 路由和消息处理组合

```csharp
// 注册路由，内部使用消息处理
dynamicRouter.RegisterRoute("POST", "/messages", async (context) =>
{
    // 从请求体获取消息
    var messageContent = context.Body.ToString();
    
    // 发送消息
    var messageResult = await resonanceAdapter.SendAsync("message-service", 
        new { Content = messageContent }, "api-gateway");
    
    return new {
        MessageId = Guid.NewGuid(),
        Status = messageResult.Success ? "Sent" : "Failed",
        Details = messageResult.Result
    };
});
```

## 5. 性能测试示例

### 5.1 路由性能测试

```csharp
// 注册多个路由
for (int i = 0; i < 100; i++)
{
    dynamicRouter.RegisterRoute("GET", $"/test/{i}", async (context) =>
    {
        return new { TestId = i, Timestamp = DateTime.UtcNow };
    });
}

// 性能测试
var stopwatch = new System.Diagnostics.Stopwatch();
int iterations = 1000;

stopwatch.Start();
for (int i = 0; i < iterations; i++)
{
    var routeContext = new RouteContext
    {
        HttpMethod = HttpMethod.Get,
        Path = $"/api/test/{i % 100}"
    };
    await dynamicRouter.MatchAndExecuteAsync(routeContext);
}
stopwatch.Stop();

Console.WriteLine($"路由性能测试: {iterations} 次请求");
Console.WriteLine($"总时间: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
Console.WriteLine($"平均时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2}ms");
Console.WriteLine($"QPS: {iterations / stopwatch.Elapsed.TotalSeconds:F2}");
```

### 5.2 消息处理性能测试

```csharp
// 注册消息处理器
resonanceAdapter.RegisterHandler("PerformanceTest", async (context) =>
{
    return new MessageResult
    {
        Success = true,
        Result = new { Processed = true, Timestamp = DateTime.UtcNow },
        ExecutionTime = TimeSpan.FromMilliseconds(1)
    };
});

// 性能测试
var stopwatch = new System.Diagnostics.Stopwatch();
int iterations = 1000;

stopwatch.Start();
var tasks = new List<Task>();
for (int i = 0; i < iterations; i++)
{
    var messageContext = new MessageContext
    {
        Source = "test-client",
        Destination = "test-service",
        Type = "PerformanceTest",
        Body = new { TestId = i, Timestamp = DateTime.UtcNow }
    };
    tasks.Add(resonanceAdapter.SendAsync(messageContext));
}
await Task.WhenAll(tasks);
stopwatch.Stop();

Console.WriteLine($"消息处理性能测试: {iterations} 次消息");
Console.WriteLine($"总时间: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
Console.WriteLine($"平均时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2}ms");
Console.WriteLine($"QPS: {iterations / stopwatch.Elapsed.TotalSeconds:F2}");
```

## 5. 最佳实践

### 5.1 路由设计

- **使用 RESTful 风格**：遵循 REST 原则设计路由
- **保持路由简洁**：避免过长或复杂的路由路径
- **使用路径参数**：对于资源标识使用路径参数，如 `/users/{id}`
- **合理使用中间件**：将横切关注点放在中间件中
- **避免路由冲突**：确保路由模式不会产生歧义

### 5.2 消息处理

- **消息类型设计**：使用明确、具体的消息类型
- **消息大小控制**：避免发送过大的消息
- **错误处理**：在消息处理器中妥善处理异常
- **重试策略**：合理设置重试次数和延迟
- **监控和日志**：记录消息处理状态和性能

### 5.3 性能优化

- **异步处理**：使用 async/await 避免阻塞
- **批量操作**：对于多个相似操作使用批量处理
- **缓存**：对于频繁访问的数据使用缓存
- **资源管理**：及时释放资源，避免内存泄漏
- **负载均衡**：合理分配工作负载

### 5.4 安全性

- **输入验证**：对所有输入进行验证
- **授权检查**：在中间件中进行授权验证
- **加密传输**：使用 HTTPS 传输敏感数据
- **速率限制**：防止恶意请求过载
- **错误处理**：避免在响应中暴露敏感信息

## 6. 故障排除

### 6.1 路由问题

| 问题 | 解决方案 |
|------|----------|
| 路由匹配失败 | 检查 HTTP 方法和路径格式是否正确 |
| 路由冲突 | 确保路由模式唯一，避免歧义 |
| 中间件执行顺序 | 注意中间件注册顺序，从外到内执行 |
| 路由参数解析 | 确保路径参数格式正确，如 `/users/{id}` |

### 6.2 消息处理问题

| 问题 | 解决方案 |
|------|----------|
| 消息发送失败 | 检查消息类型是否注册了处理器 |
| 消息处理超时 | 优化处理器逻辑或增加超时时间 |
| 队列过载 | 调整队列容量和批处理大小 |
| 重试失败 | 检查网络连接和依赖服务状态 |

### 6.3 性能问题

| 问题 | 解决方案 |
|------|----------|
| 路由匹配慢 | 使用更具体的路由模式，避免过度使用参数 |
| 消息处理延迟高 | 优化消息处理器逻辑，减少处理时间 |
| 内存占用高 | 调整队列容量，使用对象池减少 GC |
| CPU 使用率高 | 优化算法，减少不必要的计算 |

---

**© 2026 Resonance 团队** - 高性能动态路由与消息处理框架