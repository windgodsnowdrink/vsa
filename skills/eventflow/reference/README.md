# EventFlow - 参考文档

## 概述

EventFlow 是基于 .NET 10 构建的高性能事件流系统，专为 .NET 开发者设计。它采用 AOT（提前编译）架构，具有启动速度快、内存占用低、执行效率高等特点，适用于各种事件驱动架构场景。

## 核心组件

### 1. EventFlow 服务 (IEventFlowService)
- **位置**: scripts/eventflow_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 事件创建与管理
  - 事件发布与订阅
  - 事件查询
  - 事务支持
  - 性能优化
  - 错误处理
  - 日志记录
  - AOT 编译支持

### 2. EventFlow AOT 引擎 (EventFlowAotEngine)
- **位置**: scripts/eventflow_aot.cs
- **功能**: 命令行执行引擎
- **特性**: 
  - 命令行界面
  - 多种命令支持
  - 结果格式化输出
  - AOT 编译
  - 单文件执行

## 架构设计

### 分层架构

```
┌───────────────────────────────────────────────────┐
│                   命令行界面层                    │
│    (EventFlowAotEngine, 命令解析与执行)          │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   服务接口层                      │
│              (IEventFlowService)                 │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   服务实现层                      │
│              (EventFlowService)                  │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   数据访问层                      │
│    (事件存储、事件总线、事务管理)                │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   基础设施层                      │
│ (依赖注入、日志记录、配置管理、性能监控)         │
└───────────────────────────────────────────────────┘
```

## 使用示例

### 基本用法

```csharp
// 获取 EventFlow 服务
var eventFlowService = serviceProvider.GetRequiredService<IEventFlowService>();

// 发布事件
var result = await eventFlowService.PublishEventAsync("UserCreated", "{\"userId\":\"123\",\"name\":\"测试用户\"}");

// 列出事件
var eventsResult = await eventFlowService.ListEventsAsync("UserCreated", 5);
```

### AOT 单文件使用

```bash
# 运行版本命令
./eventflow_aot version

# 创建事件
./eventflow_aot create UserCreated '{"userId":"123","name":"测试用户"}'

# 列出事件
./eventflow_aot list UserCreated 5
```

### 高级配置

```csharp
// 配置 EventFlow 选项
var eventFlowOptions = new EventFlowOptions
{
    EventStoreType = "InMemory",
    EventBusType = "InMemory",
    RequestTimeoutMs = 5000,
    EnableDetailedLogging = true,
    EnablePerformanceMonitoring = true,
    MaxEventStoreCount = 10000,
    EnableEventCompression = true,
    EnableEventEncryption = true
};

// 注册配置
builder.Services.Configure<EventFlowOptions>(options =>
{
    options.EventStoreType = eventFlowOptions.EventStoreType;
    options.EventBusType = eventFlowOptions.EventBusType;
    options.RequestTimeoutMs = eventFlowOptions.RequestTimeoutMs;
    options.EnableDetailedLogging = eventFlowOptions.EnableDetailedLogging;
    options.EnablePerformanceMonitoring = eventFlowOptions.EnablePerformanceMonitoring;
    options.MaxEventStoreCount = eventFlowOptions.MaxEventStoreCount;
    options.EnableEventCompression = eventFlowOptions.EnableEventCompression;
    options.EnableEventEncryption = eventFlowOptions.EnableEventEncryption;
});
```

## 配置选项

### EventFlow 配置

```json
{
  "EventFlow": {
    "EventStoreType": "InMemory",         // 事件存储类型
    "EventStoreConnectionString": "",      // 事件存储连接字符串
    "EventBusType": "InMemory",           // 事件总线类型
    "EventBusConnectionString": "",        // 事件总线连接字符串
    "RequestTimeoutMs": 5000,              // 请求超时时间（毫秒）
    "EnableDetailedLogging": false,        // 启用详细日志
    "EnablePerformanceMonitoring": true,   // 启用性能监控
    "MaxEventStoreCount": 10000,           // 最大事件存储数量
    "EnableEventCompression": false,       // 启用事件压缩
    "EnableEventEncryption": false         // 启用事件加密
  }
}
```

## 性能优化

1. **使用 AOT 编译**：对于生产环境，推荐使用 AOT 编译以获得最佳性能
2. **启用事件压缩**：减少网络传输和存储开销
3. **异步编程**：使用异步 API 避免阻塞
4. **批量处理**：对于大量事件，使用批量处理提高效率
5. **合理设置缓存大小**：根据实际需求调整缓存大小
6. **优化事件结构**：设计紧凑的事件结构，减少序列化开销
7. **使用高性能事件存储**：根据场景选择合适的事件存储类型
8. **启用性能监控**：监控关键性能指标，便于分析和优化

## 故障排除

### 常见问题

1. **命令执行失败**
   - 检查命令格式是否正确
   - 查看日志输出获取详细错误信息
   - 检查配置文件是否正确

2. **性能问题**
   - 启用性能监控查看瓶颈
   - 考虑使用 AOT 编译
   - 优化事件结构和大小
   - 调整缓存大小和超时设置

3. **事件发布失败**
   - 检查事件总线配置
   - 查看网络连接是否正常
   - 检查事件存储是否可用

## 扩展开发

### 添加自定义事件存储

```csharp
// 自定义事件存储接口
public interface ICustomEventStore
{
    Task<EventData> GetEventAsync(string eventId);
    Task<List<EventData>> GetEventsAsync(string eventType, int limit);
    Task<string> SaveEventAsync(EventData eventData);
}

// 自定义事件存储实现
public class CustomEventStore : ICustomEventStore
{
    // 实现自定义事件存储逻辑
    public async Task<EventData> GetEventAsync(string eventId)
    {
        // 实现获取事件逻辑
        return new EventData();
    }
    
    // 其他方法实现...
}

// 注册自定义事件存储
builder.Services.AddSingleton<ICustomEventStore, CustomEventStore>();
```

### 扩展命令行功能

可以通过修改 `EventFlowAotEngine` 类来扩展命令行功能，添加新的命令处理逻辑。

## AOT 编译说明

### 编译命令

```bash
# 编译为 AOT 单文件
dotnet publish -c Release -r win-x64 -p:PublishAot=true -p:PublishSingleFile=true

# 编译为 Linux AOT 单文件
dotnet publish -c Release -r linux-x64 -p:PublishAot=true -p:PublishSingleFile=true

# 编译为 macOS AOT 单文件
dotnet publish -c Release -r osx-x64 -p:PublishAot=true -p:PublishSingleFile=true
```

### AOT 优势

- **快速启动**：AOT 编译的应用程序启动时间比 JIT 编译的应用程序快得多
- **低内存占用**：AOT 编译的应用程序内存占用更低
- **无需 JIT 编译**：运行时不需要 JIT 编译，减少了运行时开销
- **自包含**：可以编译为单个可执行文件，便于部署和分发
- **跨平台**：支持多个平台的 AOT 编译

## 最佳实践

1. **事件设计原则**
   - 事件名称使用过去式（如 UserCreated）
   - 事件数据包含足够的上下文信息
   - 避免事件数据过大
   - 考虑事件版本管理

2. **生产环境配置**
   - 使用 AOT 编译
   - 启用事件压缩
   - 启用性能监控
   - 配置适当的日志级别
   - 定期备份事件数据

3. **安全考虑**
   - 对于敏感事件数据，启用事件加密
   - 限制事件总线的访问权限
   - 实施适当的身份验证和授权

4. **监控与告警**
   - 监控事件发布和订阅的成功率
   - 监控事件处理延迟
   - 监控事件存储的使用情况
   - 设置适当的告警阈值

5. **部署建议**
   - 使用容器化部署
   - 考虑高可用性设计
   - 实施适当的负载均衡
   - 定期更新和维护