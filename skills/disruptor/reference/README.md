# disruptor - 参考文档

## 概述

disruptor 是基于 .NET 10 AOT 架构的高性能事件处理技能，专为 .NET 开发者设计，提供强大的事件处理功能支持。该技能采用高效的环形缓冲区设计，支持多消费者并行处理，适用于高并发、低延迟的事件处理场景。

## 核心组件

### 1. DisruptorService（Disruptor 服务）
- **位置**: scripts/disruptor_aot.cs
- **功能**: 事件处理核心服务，负责事件的发布、消费和管理
- **特性**: 
  - 基于 .NET 10 AOT 编译，高性能
  - 高效的环形缓冲区设计
  - 支持多消费者并行处理
  - 批量事件处理优化
  - 优先级事件支持
  - 自动重试机制
  - 详细的状态监控

### 2. DisruptorAotEngine（Disruptor AOT 引擎）
- **位置**: scripts/disruptor_aot.cs
- **功能**: 管理 Disruptor 功能调用的引擎
- **特性**: 
  - 简化的 API 调用
  - 统一的错误处理
  - 性能监控和状态管理
  - 支持多种事件发布方式

## 核心接口

### IDisruptorService
Disruptor 服务的核心接口，定义了所有事件处理方法：

| 方法名 | 描述 | 参数 | 返回值 |
|--------|------|------|--------|
| PublishEventAsync | 发布事件 | eventData: DisruptorEvent | DisruptorResult |
| PublishEventsAsync | 批量发布事件 | events: List<DisruptorEvent> | List<DisruptorResult> |
| PublishPriorityEventAsync | 发布优先级事件 | eventData: DisruptorEvent, priority: int = 100 | DisruptorResult |
| GetStatusAsync | 获取 Disruptor 状态 | 无 | Task<DisruptorStatus> |
| ResetStatusAsync | 重置 Disruptor 状态 | 无 | Task<bool> |
| StartAsync | 启动 Disruptor 服务 | 无 | Task<bool> |
| StopAsync | 停止 Disruptor 服务 | 无 | Task<bool> |

## 数据结构

### DisruptorEvent（事件数据）
```csharp
public class DisruptorEvent
{
    public Guid Id { get; set; }                 // 事件ID
    public DisruptorEventType Type { get; set; }  // 事件类型
    public string? Data { get; set; }             // 事件数据
    public DateTime CreatedAt { get; set; }       // 创建时间
    public int Priority { get; set; }             // 优先级（0-100）
    public string? Source { get; set; }           // 事件来源
    public List<string>? Tags { get; set; }       // 事件标签
}
```

### DisruptorEventType（事件类型）
```csharp
public enum DisruptorEventType
{
    Normal,      // 普通事件
    Priority,    // 优先级事件
    Emergency,   // 紧急事件
    System       // 系统事件
}
```

### DisruptorOptions（配置选项）
```csharp
public class DisruptorOptions
{
    public int RingBufferSize { get; set; } = 4096;       // 环形缓冲区大小（必须是2的幂）
    public int ConsumerCount { get; set; } = 1;           // 消费者数量
    public bool EnableBatching { get; set; } = true;      // 是否启用批量处理
    public int BatchSize { get; set; } = 64;              // 批量处理大小
    public bool EnablePriorityQueue { get; set; } = false; // 是否启用优先级队列
    public bool EnableEventTracking { get; set; } = true;  // 是否启用事件跟踪
    public int EventTimeoutMs { get; set; } = 5000;        // 事件超时时间（毫秒）
    public int MaxRetryCount { get; set; } = 3;            // 最大重试次数
    public bool EnableDetailedLogging { get; set; } = false; // 是否启用详细日志
    public bool EnablePerformanceMonitoring { get; set; } = true; // 是否启用性能监控
}
```

### DisruptorResult（事件处理结果）
```csharp
public class DisruptorResult
{
    public bool Success { get; set; }               // 处理是否成功
    public Guid EventId { get; set; }               // 事件ID
    public object? ResultData { get; set; }         // 处理结果数据
    public string? ErrorMessage { get; set; }       // 错误信息
    public long ExecutionTimeMs { get; set; }       // 执行时间（毫秒）
    public int RetryCount { get; set; }             // 重试次数
}
```

### DisruptorStatus（Disruptor 状态）
```csharp
public class DisruptorStatus
{
    public bool IsRunning { get; set; }              // 服务是否正常运行
    public long ProcessedEvents { get; set; }        // 已处理的事件数
    public long SuccessfulEvents { get; set; }       // 成功处理的事件数
    public long FailedEvents { get; set; }           // 失败处理的事件数
    public double AverageProcessingTimeMs { get; set; } // 平均处理时间（毫秒）
    public double RingBufferUsage { get; set; }      // 当前环形缓冲区使用率（%）
    public DateTime StartTime { get; set; }          // 服务启动时间
    public int ActiveConsumers { get; set; }         // 当前活跃消费者数量
    public int RingBufferSize { get; set; }          // 环形缓冲区大小
}
```

## 配置选项

### Disruptor 配置（disruptor_aot.setting.json）

```json
{
  "Disruptor": {
    "RingBufferSize": 4096,             // 环形缓冲区大小（必须是2的幂）
    "ConsumerCount": 2,                 // 消费者数量
    "EnableBatching": true,             // 是否启用批量处理
    "BatchSize": 64,                    // 批量处理大小
    "EnablePriorityQueue": false,       // 是否启用优先级队列
    "EnableEventTracking": true,        // 是否启用事件跟踪
    "EventTimeoutMs": 5000,             // 事件超时时间（毫秒）
    "MaxRetryCount": 3,                 // 最大重试次数
    "EnableDetailedLogging": false,     // 是否启用详细日志
    "EnablePerformanceMonitoring": true // 是否启用性能监控
  }
}
```

## 运行配置（disruptor_aot.run.json）

```json
{
  "$schema": "https://dot.net/v1/dotnet.run.schema.json",
  "framework": "net10.0",
  "options": {
    "PublishAot": true,                   // 启用 AOT 编译
    "InvariantGlobalization": true,       // 启用不变全球化
    "EnableCompilationRelaxations": true, // 启用编译松弛
    "PublishReadyToRun": true,            // 启用 ReadyToRun
    "LangVersion": "preview",            // 语言版本
    "Nullable": true,                     // 启用可空引用类型
    "ImplicitUsings": true                // 启用隐式 using
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Hosting": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "Microsoft.Extensions.Options": "10.0.0",
    "System.Threading.Channels": "8.0.0"
  }
}
```

## 性能优化

1. **AOT 编译**: 启用 PublishAot=true，获得极致的启动速度和运行性能
2. **环形缓冲区大小**: 根据实际负载调整缓冲区大小，建议使用 2 的幂
3. **消费者数量**: 根据 CPU 核心数量调整消费者数量，通常为核心数的 1-2 倍
4. **批量处理**: 启用批量处理，根据实际情况调整批量大小
5. **优先级队列**: 对于有优先级需求的场景，启用优先级队列
6. **日志级别**: 在生产环境中，将日志级别设置为 Information 或更高，减少日志开销
7. **重试机制**: 根据业务需求调整最大重试次数，避免无限重试

## 故障排除

### 常见问题

1. **事件发布失败**
   - 检查 Disruptor 服务是否已启动
   - 查看环形缓冲区是否已满
   - 检查日志获取详细错误信息

2. **性能问题**
   - 调整环形缓冲区大小
   - 增加消费者数量
   - 启用批量处理
   - 检查事件处理逻辑是否有性能瓶颈
   - 启用 AOT 编译

3. **消费者异常**
   - 检查事件处理逻辑是否有未处理的异常
   - 调整最大重试次数
   - 查看日志获取详细错误信息

## 扩展开发

### 自定义事件处理

1. 继承 DisruptorService 类，重写 ProcessEventAsync 方法
2. 实现自定义的事件处理逻辑
3. 注册自定义服务

```csharp
builder.Services.AddSingleton<IDisruptorService, CustomDisruptorService>();
```

### 扩展事件类型

1. 在 DisruptorEventType 枚举中添加新的事件类型
2. 在事件处理逻辑中添加对应的处理分支
3. 根据新事件类型实现特定的处理逻辑

## AOT 编译注意事项

1. **反射使用**: 避免在运行时使用反射，或使用 AOT 友好的反射方式
2. **动态类型**: 谨慎使用 dynamic 类型，可能会影响 AOT 编译效果
3. **依赖项**: 确保所有依赖项都支持 AOT 编译
4. **配置文件**: AOT 编译后，配置文件路径可能需要调整
5. **测试**: 在 AOT 模式下进行充分测试，确保所有功能正常工作

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `start`: 启动 Disruptor 服务
- `stop`: 停止 Disruptor 服务
- `status`: 查看 Disruptor 服务状态
- `reset`: 重置 Disruptor 服务状态
- `demo`: 运行 Disruptor 演示程序

使用示例：
```
disruptor_aot.exe start
disruptor_aot.exe status
disruptor_aot.exe demo
```

## 版本历史

| 版本 | 日期 | 描述 |
|------|------|------|
| 1.0.0 | 2026-01-03 | 初始版本，基于 .NET 10 AOT 架构 |

## 许可证

MIT License