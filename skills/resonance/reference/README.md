# Resonance 技术参考文档

## 1. 项目概述

Resonance 是一个基于 .NET 10 和 AOT 编译的动态路由与消息处理框架，旨在提供高性能、可扩展的路由和消息处理能力。

### 核心功能

- **动态路由**：支持 HTTP 方法路由、路径参数、查询参数和中间件
- **消息处理**：支持异步消息发送、处理、重试和中间件
- **AOT 编译**：支持 Ahead-of-Time 编译，提高运行性能
- **依赖注入**：集成 Microsoft.Extensions.DependencyInjection
- **可扩展性**：支持插件系统和自定义中间件

## 2. 架构设计

### 2.1 目录结构

```
resonance/
├── index.yaml          # 技能元数据
├── SKILL.md            # 技能文档
├── scripts/            # .NET 10 单文件执行脚本
│   ├── dynamic_router.cs             # 动态路由实现
│   ├── dynamic_router.setting.json   # 动态路由配置
│   ├── dynamic_router.run.json       # 动态路由运行配置
│   ├── resonance_adapter.cs          # 消息处理适配器
│   ├── resonance_adapter.setting.json # 消息处理适配器配置
│   └── resonance_adapter.run.json    # 消息处理适配器运行配置
└── reference/          # 技术参考文档
    ├── README.md       # 技术参考
    └── examples.md     # 使用示例
```

### 2.2 核心组件

| 组件 | 说明 | 职责 |
|------|------|------|
| DynamicRouter | 动态路由实现 | 路由注册、匹配、执行 |
| ResonanceAdapter | 消息处理适配器 | 消息发送、处理、重试 |
| MessagePipeline | 消息管道 | 中间件管理和执行 |
| RouterOptions | 路由选项 | 路由配置管理 |
| MessageOptions | 消息选项 | 消息处理配置管理 |

## 3. API 参考

### 3.1 动态路由 API

#### IDynamicRouter 接口

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
    
    // 获取路由执行统计
    RouteExecutionStats GetStats();
}
```

#### RouteContext 类

```csharp
public class RouteContext
{
    public HttpMethod HttpMethod { get; set; }
    public string Path { get; set; }
    public Dictionary<string, string> RouteParameters { get; set; } = new();
    public Dictionary<string, string> QueryParameters { get; set; } = new();
    public Dictionary<string, string> Headers { get; set; } = new();
    public object Body { get; set; }
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}
```

### 3.2 消息处理 API

#### IResonanceAdapter 接口

```csharp
public interface IResonanceAdapter
{
    // 发送消息
    Task<MessageResult> SendAsync(MessageContext context);
    
    // 发送消息（泛型）
    Task<MessageResult> SendAsync<T>(string destination, T message, string source = "");
    
    // 注册消息处理器
    void RegisterHandler(string messageType, MessageHandler handler);
    
    // 注册消息处理器（泛型）
    void RegisterHandler<T>(Func<MessageContext, T, Task<MessageResult>> handler);
    
    // 注册消息处理器（带中间件）
    void RegisterHandler(string messageType, MessageHandler handler, params Func<MessageContext, Func<MessageContext, Task<MessageResult>>, Task<MessageResult>>[] middleware);
    
    // 取消注册消息处理器
    void UnregisterHandler(string messageType);
    
    // 获取所有注册的处理器
    IEnumerable<string> GetRegisteredHandlers();
    
    // 启动适配器
    Task StartAsync(CancellationToken cancellationToken = default);
    
    // 停止适配器
    Task StopAsync(CancellationToken cancellationToken = default);
    
    // 获取消息处理统计
    MessageStats GetStats();
}
```

#### MessageContext 类

```csharp
public class MessageContext
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public string Source { get; set; }
    public string Destination { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public object Body { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}
```

## 4. 配置管理

### 4.1 路由配置

通过 `RouterOptions` 类配置路由行为：

```csharp
public class RouterOptions
{
    public string RoutePrefix { get; set; } = "/api";
    public int MaxConcurrentRequests { get; set; } = 100;
    public bool EnableTelemetry { get; set; } = true;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}
```

### 4.2 消息处理配置

通过 `MessageOptions` 类配置消息处理行为：

```csharp
public class MessageOptions
{
    public int MaxMessageSize { get; set; } = 1024 * 1024; // 1MB
    public int QueueCapacity { get; set; } = 1000;
    public int WorkerCount { get; set; } = Environment.ProcessorCount;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 100;
    public bool EnableDeadLetterQueue { get; set; } = true;
}
```

### 4.3 环境变量配置

| 环境变量 | 说明 | 默认值 |
|---------|------|--------|
| ROUTE_PREFIX | 路由前缀 | /api |
| MAX_CONCURRENT_REQUESTS | 最大并发请求数 | 100 |
| ENABLE_TELEMETRY | 启用遥测 | true |
| MAX_MESSAGE_SIZE | 最大消息大小 | 1048576 |
| QUEUE_CAPACITY | 队列容量 | 1000 |
| WORKER_COUNT | 工作线程数 | 4 |
| RETRY_COUNT | 重试次数 | 3 |
| RETRY_DELAY_MS | 重试延迟 | 100 |
| ENABLE_DEAD_LETTER_QUEUE | 启用死信队列 | true |

## 5. 依赖注入

### 5.1 注册服务

```csharp
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
```

### 5.2 解析服务

```csharp
// 获取动态路由服务
var dynamicRouter = serviceProvider.GetRequiredService<IDynamicRouter>();

// 获取消息处理服务
var resonanceAdapter = serviceProvider.GetRequiredService<IResonanceAdapter>();
```

## 6. AOT 编译配置

### 6.1 编译选项

| 选项 | 说明 | 值 |
|------|------|------|
| PublishAot | 启用 AOT 编译 | true |
| TrimMode | 裁剪模式 | partial |
| SelfContained | 自包含部署 | true |
| EnableCompressionInSingleFile | 启用单文件压缩 | true |
| ReadyToRun | 启用 ReadyToRun 编译 | true |
| TieredCompilation | 启用分层编译 | true |
| Optimize | 启用优化 | true |

### 6.2 依赖管理

确保所有依赖项支持 AOT 编译，特别是：

- 避免使用反射
- 避免使用动态类型
- 使用 trim-safe APIs
- 配置必要的 AOT 兼容性属性

## 7. 性能优化

### 7.1 路由性能

- 使用路由树结构进行快速匹配
- 缓存路由匹配结果
- 避免在路由处理器中执行 heavy 操作
- 使用异步处理避免阻塞

### 7.2 消息处理性能

- 使用队列限流避免过载
- 批量处理消息
- 优化消息序列化/反序列化
- 使用异步处理提高吞吐量

### 7.3 内存优化

- 使用对象池减少 GC 压力
- 避免大型对象分配
- 使用 Span<T> 和 Memory<T> 进行零拷贝
- 合理设置队列容量

## 8. 故障排除

### 8.1 常见问题

| 问题 | 可能原因 | 解决方案 |
|------|---------|---------|
| 路由匹配失败 | 路径格式不正确 | 检查 HTTP 方法和路径格式 |
| 消息处理超时 | 处理器执行时间过长 | 优化处理器逻辑或增加超时时间 |
| 内存占用过高 | 队列容量过大 | 调整队列容量和批处理大小 |
| AOT 编译失败 | 依赖项不支持 AOT | 检查依赖项的 AOT 兼容性 |

### 8.2 日志和监控

- 启用详细日志记录
- 监控队列深度和处理延迟
- 跟踪路由执行时间
- 监控消息处理成功率

## 9. 版本兼容性

| .NET 版本 | 支持状态 |
|-----------|----------|
| .NET 10.0 | ✅ 完全支持 |
| .NET 8.0 | ✅ 支持 |
| .NET 7.0 | ⚠️ 部分支持 |
| .NET 6.0 | ❌ 不支持 |

## 10. 开发环境设置

### 10.1 前提条件

- .NET 10 SDK 或更高版本
- Visual Studio 2022 或更高版本
- Windows 10/11、macOS 13+ 或 Linux

### 10.2 构建和运行

```bash
# 构建项目
dotnet build

# 运行动态路由示例
dotnet run --project dynamic_router.cs

# 运行消息处理示例
dotnet run --project resonance_adapter.cs

# 发布 AOT 版本
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishAot=true
```

## 11. 许可证

本项目采用 MIT 许可证，详见 LICENSE 文件。

## 12. 贡献指南

欢迎贡献代码、报告问题或提出建议。请遵循以下步骤：

1. Fork 项目仓库
2. 创建特性分支
3. 提交更改
4. 推送到分支
5. 打开 Pull Request

## 13. 联系我们

- 项目地址：https://github.com/resonance
- 问题反馈：https://github.com/resonance/issues
- 邮件：contact@resonance.dev

---

**© 2026 Resonance 团队** - 高性能动态路由与消息处理框架