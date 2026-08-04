---
kind: error_handling
name: C# 错误处理体系：异常传播、日志记录与弹性模式
category: error_handling
scope:
    - '**'
source_files:
    - vsa/ChangeDataCaptureService.cs
    - vsa/PluginLoadContext.cs
    - vsa/SourceGenerator_ddd_advanced_generator.cs
    - vsa/skills/audio/scripts/asr_integration.cs
    - vsa/skills/dataflow/scripts/dataflow_demo.cs
    - vsa/skills/message/scripts/channels_demo.cs
---

## 错误处理架构概述

该 C# 代码库采用多层次、系统化的错误处理策略，结合了传统异常处理与现代弹性编程模式，确保应用程序的健壮性和可恢复性。

## 核心错误处理模式

### 1. 异常类型定义与传播
- **标准异常使用**：广泛使用 `ArgumentException`、`InvalidOperationException`、`SecurityException` 等 .NET 标准异常类型
- **参数验证**：在构造函数和方法入口处进行参数验证，抛出适当的异常（如 `ArgumentNullException`、`ArgumentOutOfRangeException`）
- **业务异常**：通过 `InvalidOperationException` 表达业务逻辑错误状态

### 2. 异步错误处理模式
- **try-catch-finally 模式**：在异步方法中使用标准的异常捕获和清理模式
- **Task 异常传播**：利用 Task 的异常聚合机制，正确处理并发操作中的错误
- **CancellationToken 集成**：在取消操作中正确处理 `OperationCanceledException`

### 3. 中间件和管道错误处理
- **ASP.NET Core 中间件**：使用自定义中间件进行全局异常处理
- **SignalR Hub 异常处理**：在实时通信中捕获并处理连接级错误
- **gRPC 拦截器**：统一处理 gRPC 服务层的异常转换

## 弹性编程实践

### 1. Polly 重试和熔断模式
```csharp
// 指数退避重试
var retryPolicy = Policy
    .Handle<Exception>()
    .WaitAndRetryAsync(
        3,
        attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
        (ex, time, retryCount, context) => 
            _logger.LogWarning(ex, $"请求失败。重试尝试 {retryCount} 后 {time.TotalSeconds} 秒重试。"));
```

### 2. 断路器模式实现
- **自动故障检测**：基于失败次数和时间的断路器配置
- **降级策略**：在主服务不可用时提供备用方案
- **健康检查集成**：将断路器状态暴露为健康检查端点

### 3. 超时和限流控制
- **操作超时**：使用 `CancellationTokenSource` 实现操作级超时
- **并发限制**：通过 `SemaphoreSlim` 控制并发执行数量
- **背压处理**：使用有界通道 (`Channel.CreateBounded`) 防止内存溢出

## 日志记录和监控

### 结构化日志
- **Microsoft.Extensions.Logging**：统一的日志抽象层
- **结构化日志格式**：使用 `{PropertyName}` 语法记录上下文信息
- **日志级别管理**：根据错误严重性选择合适的日志级别

### 分布式追踪
- **ActivitySource 集成**：使用 OpenTelemetry 进行分布式追踪
- **Span 标签**：为关键操作添加语义化标签
- **错误状态标记**：在追踪活动中标记错误状态

## 资源管理和清理

### 1. IAsyncDisposable 模式
- **异步资源清理**：实现 `IAsyncDisposable` 接口确保异步资源的正确释放
- **优雅关闭**：在服务停止时等待正在进行的任务完成
- **资源泄漏防护**：使用 `using` 语句和 finally 块确保资源释放

### 2. 对象池和内存管理
- **ObjectPool<T>**：重用昂贵对象减少 GC 压力
- **MemoryPool<byte>**：高效内存分配和回收
- **ArrayPool<T>**：数组对象的池化管理

## 特定场景的错误处理

### 1. 数据库操作
- **事务回滚**：在异常发生时自动回滚数据库事务
- **连接重试**：使用 Polly 处理数据库连接问题
- **批量操作错误隔离**：单个记录失败不影响整个批处理

### 2. 外部服务调用
- **HTTP 客户端错误处理**：处理网络超时、连接失败等异常情况
- **第三方 API 错误映射**：将外部服务的错误转换为内部异常类型
- **缓存失效策略**：在外部服务不可用时使用缓存数据

### 3. 消息队列处理
- **死信队列**：将无法处理的消息发送到死信队列
- **幂等性保证**：确保消息处理的幂等性
- **顺序保证**：在需要时保证消息处理顺序

## 最佳实践总结

1. **防御性编程**：在所有边界点进行输入验证和错误检查
2. **有意义的异常信息**：提供足够的上下文信息便于问题诊断
3. **分层错误处理**：不同层次采用不同的错误处理策略
4. **可观测性优先**：确保所有错误都被适当记录和追踪
5. **弹性设计**：假设外部依赖会失败，设计相应的恢复策略
6. **资源安全**：确保所有资源都能正确释放，避免内存泄漏

这种综合性的错误处理体系确保了应用程序在面对各种异常情况时能够保持稳定运行，同时提供了足够的可观测性和可维护性。