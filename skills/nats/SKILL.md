# NATS Agent Skill - NATS 技能

## 技能概览

基于 .NET 10 的高性能 NATS 技能，为 .NET 开发者提供强大的 NATS 消息系统功能。

## 快速入门指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package csharp-nats@2.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
`

### 注册服务

在主应用中注册 NATS 服务：

`csharp
// 注册 NATS 服务
builder.Services.Configure<NatsOptions>(options => {
    options.Url = "nats://localhost:4222";
    options.ConnectionTimeout = 5000;
    options.MaxPayloadSize = 1048576; // 1MB
    options.ReconnectWait = 2000;
});
builder.Services.AddSingleton<INatsService, NatsService>();
builder.Services.AddSingleton<INatsPublisher, NatsPublisher>();
builder.Services.AddSingleton<INatsSubscriber, NatsSubscriber>();
`

### 使用示例

`csharp
// 获取 NATS 服务
var natsService = serviceProvider.GetRequiredService<INatsService>();
var publisher = serviceProvider.GetRequiredService<INatsPublisher>();
var subscriber = serviceProvider.GetRequiredService<INatsSubscriber>();

// 发布消息
await publisher.PublishAsync("demo.subject", Encoding.UTF8.GetBytes("Hello NATS!"));

// 订阅消息
await subscriber.SubscribeAsync("demo.subject", (subject, message) => {
    Console.WriteLine($"Received message from {subject}: {Encoding.UTF8.GetString(message)}");
});

// 使用 NATS 服务的其他功能
var isConnected = await natsService.IsConnectedAsync();
Console.WriteLine($"NATS connection status: {isConnected}");
`

## 导航地图

`
nats/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── csharp_nats_integration.cs     # NATS 核心实现
    ├── csharp_nats_integration.run.json  # 运行配置
    └── csharp_nats_integration.setting.json  # 设置文件
`

## 主要功能

1. **消息发布与订阅**：支持高性能的消息发布和订阅功能，使用通道和内存池优化性能
2. **连接管理**：自动重连、连接状态监控和管理
3. **消息队列**：支持消息队列功能，确保消息可靠传递
4. **高性能设计**：使用 Span 优化、内存池、通道和零拷贝技术
5. **易于使用的 API**：简单直观的 API 设计，支持异步操作
6. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 NATS 解决方案，您可以根据需要进行扩展：

1. **自定义消息处理器**：实现自定义的消息处理逻辑
2. **扩展功能**：添加新的 NATS 功能，如请求-响应模式、流处理等
3. **与其他系统集成**：将 NATS 与其他系统集成，如数据库、缓存等
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **连接管理**：合理管理 NATS 连接，避免连接泄漏

## 配置选项

### NatsOptions 配置

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| Url | string | "nats://localhost:4222" | NATS 服务器 URL |
| ConnectionTimeout | int | 5000 | 连接超时时间（毫秒） |
| MaxPayloadSize | int | 1048576 | 最大消息大小（字节） |
| ReconnectWait | int | 2000 | 重连等待时间（毫秒） |

## 性能特性

- **高吞吐量**：使用通道和批处理技术，支持高并发消息处理
- **低延迟**：使用 Span 和零拷贝技术，减少内存分配和拷贝
- **内存优化**：使用内存池和对象池，减少 GC 压力
- **可扩展性**：支持水平扩展，适用于大规模应用

## 版本兼容性

| .NET 版本 | 兼容性 |
|-----------|--------|
| .NET 10.0 | ✅ 完全支持 |
| .NET 9.0  | ✅ 支持 |
| .NET 8.0  | ✅ 支持 |
