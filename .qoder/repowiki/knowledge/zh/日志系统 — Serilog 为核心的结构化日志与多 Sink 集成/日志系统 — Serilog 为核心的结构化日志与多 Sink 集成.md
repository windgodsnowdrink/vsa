---
kind: logging_system
name: 日志系统 — Serilog 为核心的结构化日志与多 Sink 集成
category: logging_system
scope:
    - '**'
source_files:
    - code/serilog_integration.cs
    - code/seq_integration.cs
    - code/opentelemetry_logging.cs
    - code/appinsights_logging.cs
    - code/datadog_logging.cs
    - code/skyapm_logging.cs
    - code/litedb_logging.cs
    - ai/DigitalHumanDemo.cs
---

## 1. 使用的系统与框架
- 核心日志库：Serilog（版本 3.x/4.x），作为所有示例的默认日志实现。
- 标准抽象：Microsoft.Extensions.Logging，通过 Serilog.Extensions.Logging 桥接，使业务代码统一使用 ILogger<T>。
- 常用 Sink：Console、Seq、RollingFileAlternate、EventLog、Http、SpectreConsole、Trace 等，覆盖控制台、文件、集中式日志平台与 Windows Event Log。
- 结构化字段增强：Enrichers.Environment、Enrichers.Process、Enrichers.Thread、Enrichers.Span、Exceptions 等提供进程、线程、异常、Span 上下文等结构化字段。
- 异步写入：Serilog.Sinks.Async 用于高吞吐场景下的异步落盘/上报。
- 配置来源：Serilog.Settings.Configuration 支持从配置文件注入日志级别、Sink 参数等。
- 追踪集成：OpenTelemetry、SkyAPM、Datadog、Application Insights 等通过自定义 ILogEventEnricher 将 TraceId/SpanId/SegmentId 等注入日志事件。

## 2. 关键文件与位置
- code/serilog_integration.cs：Serilog 扩展能力演示（插件化加载 ISerilogPlugin、高性能 Channel 队列、数据脱敏 DataMaskingEnricher、Prometheus/OpenTelemetry 集成注册）。
- code/seq_integration.cs：基于 Seq 的结构化日志示例，包含 Logger 构建、Async 写入、BackgroundService 消费 Channel、UseSerilogRequestLogging 请求级 enrich。
- code/opentelemetry_logging.cs：OpenTelemetry Logging 集成，AddOpenTelemetry + ILogEventEnricher 注入 TraceId/SpanId。
- code/appinsights_logging.cs：Application Insights TelemetryInitializer + Serilog ILogEventEnricher 关联 OperationId。
- code/datadog_logging.cs：SkyWalking/Datadog Tracer 上下文注入 dd.trace_id/dd.span_id。
- code/skyapm_logging.cs：SkyAPM Agent 上下文注入 TraceId/SpanId/SegmentId。
- code/litedb_logging.cs：自定义轻量日志存储（LiteDB + Channel + ObjectPool），展示结构化日志实体模型与索引策略。
- ai/DigitalHumanDemo.cs：完整 Serilog 生态包清单（Console/Seq/RollingFile/EventLog/Http/SpectreConsole/Trace 等），体现生产级日志依赖组合。

## 3. 架构与约定
- 日志抽象层：业务代码通过 Microsoft.Extensions.Logging 的 ILogger<T> 记录日志，不直接依赖具体实现；Serilog 通过 UseSerilog(Log.Logger) 或 AddSerilog() 接入 DI。
- 结构化字段：统一通过 Serilog.Enrich.FromLogContext() 与自定义 ILogEventEnricher 注入跨领域上下文（如 TraceId、SpanId、dd.trace_id、OperationId 等），保证日志可关联分布式追踪。
- 高吞吐模式：大量示例采用 System.Threading.Channels 做生产者-消费者缓冲，配合 BackgroundService 或独立 Task 消费并写入 Sink，避免阻塞主流程。
- 安全与合规：DataMaskingEnricher 对敏感属性进行掩码处理，防止密码、Token 等泄露到日志。
- 插件化扩展：ISerilogPlugin 接口 + AssemblyLoadContext 动态加载外部 DLL，按约定发现并调用 Configure(LoggerConfiguration)，实现 Sink/Enricher 的可插拔扩展。
- 配置驱动：MinimumLevel 与 Override 控制模块日志级别（如 Microsoft/System 降级为 Warning），Seq/Http 等 Sink 参数通过 IConfiguration 注入。

## 4. 约定与约束
- 日志级别策略：示例中普遍设置 MinimumLevel.Information，并将第三方库（Microsoft、System）降级为 Warning，减少噪音。
- 结构化字段命名：追踪相关字段遵循各平台约定（如 dd.trace_id/dd.span_id、TraceId/SpanId、OperationId、SegmentId），便于在对应平台聚合查询。
- 异步写入：在高并发场景下优先使用 Serilog.Sinks.Async 或自建 Channel + BackgroundService，确保写入不阻塞业务线程。
- 资源释放：Channel 与 Logger 需在应用退出时正确 Complete/Dispose，示例中通过 IAsyncDisposable 与 HostedService 管理生命周期。
- 性能优化：使用 [SkipLocalsInit]、[MethodImpl(MethodImplOptions.AggressiveOptimization)] 标记高频路径，结合 ArrayPool 与 ObjectPool 降低 GC 压力。
- 安全约束：禁止直接输出敏感信息，需通过 DataMaskingEnricher 或自定义 Enricher 进行脱敏后再写入 Sink。

## 5. 典型用法模式
- Web 应用初始化：WebApplicationBuilder.AddSeqLogging()/UseSerilogRequestLogging()，在请求上下文中自动 enrich RequestHost、RemoteIpAddress 等字段。
- 追踪关联：ILogEventEnricher 读取当前 Tracer 上下文，将 TraceId/SpanId 附加到 LogEvent，实现“日志-链路”一体化。
- 自定义 Sink/Enricher：通过 WithCustomEnricher<T>() 或 ISerilogPlugin 动态注册，无需修改核心日志配置即可扩展功能。
- 本地持久化：LiteDbLogService 展示如何将结构化日志写入本地数据库，并提供时间范围与 Level 过滤查询。