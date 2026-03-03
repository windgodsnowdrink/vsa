# disruptor Agent Skill - 高性能事件处理技能

## 技能概述

基于 .NET 10 构建的高性能 Disruptor 事件处理技能，为 .NET 开发者提供强大的事件处理功能支持。该技能采用 AOT（预编译）技术，提供极致的性能表现和启动速度，适用于高并发、低延迟的事件处理场景。

## 快速入门指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Channels@8.0.0
```

### 注册服务

在主应用程序中注册 Disruptor 服务：

```csharp
// 配置 Disruptor 选项
builder.Configuration.AddJsonFile("disruptor_aot.setting.json", optional: true);
builder.Services.Configure<DisruptorOptions>(builder.Configuration.GetSection("Disruptor"));

// 注册 Disruptor 服务
builder.Services.AddDisruptor();
```

### 使用示例

```csharp
// 获取 Disruptor 引擎实例
var engine = serviceProvider.GetRequiredService<DisruptorAotEngine>();

// 启动 Disruptor 服务
await engine.StartAsync();

// 创建并发布事件
var evnt = new DisruptorEvent
{
    Id = Guid.NewGuid(),
    Type = DisruptorEventType.Normal,
    Data = "事件数据",
    CreatedAt = DateTime.UtcNow,
    Source = "MyApp",
    Tags = new List<string> { "demo", "test" }
};

// 发布事件
var result = await engine.PublishEventAsync(evnt);
Console.WriteLine($"事件发布结果: {result.Success ? "成功" : "失败"}");

// 停止 Disruptor 服务
await engine.StopAsync();
```

## 导航地图

```
disruptor/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── disruptor_aot.cs       # Disruptor 核心实现（AOT）
    ├── disruptor_aot.run.json # 运行配置
    └── disruptor_aot.setting.json # 应用设置
```

## 主要功能

1. **高性能 AOT 编译**：基于 .NET 10 AOT 技术，提供极致性能和启动速度
2. **环形缓冲区设计**：采用高效的环形缓冲区实现，减少内存分配和 GC 压力
3. **多消费者支持**：支持多个消费者并行处理事件
4. **批量处理优化**：支持批量事件处理，提高吞吐量
5. **优先级事件支持**：支持不同优先级的事件处理
6. **自动重试机制**：内置事件处理失败自动重试功能
7. **详细的状态监控**：实时监控事件处理状态和性能指标
8. **灵活的配置选项**：支持通过配置文件自定义各种参数
9. **完善的错误处理**：详细的错误信息和日志记录

## 扩展说明

该技能提供了完整的 Disruptor 事件处理解决方案，您可以根据需要进行扩展：

1. **自定义事件处理器**：实现您自己的事件处理逻辑
2. **扩展事件类型**：添加新的事件类型和处理方式
3. **集成其他系统**：与其他系统和框架集成，实现事件驱动架构
4. **性能优化**：针对特定场景优化事件处理性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **合理配置缓冲区大小**：根据实际负载调整环形缓冲区大小
4. **批量处理优化**：对于大量事件，启用批量处理以提高吞吐量
5. **适当的消费者数量**：根据 CPU 核心数量调整消费者数量
6. **日志级别控制**：在生产环境中，将日志级别设置为 Information 或更高，减少日志开销
7. **错误处理**：妥善处理事件处理过程中的异常情况
8. **性能监控**：定期监控 Disruptor 服务状态，及时发现性能瓶颈

## AOT 编译说明

该技能支持 .NET 10 AOT 编译，通过预编译将应用程序编译为本地机器代码，提供以下优势：

- **极致的启动速度**：无需 JIT 编译，直接运行本地代码
- **减少内存占用**：更小的运行时占用
- **提高安全性**：减少可攻击面，提高应用程序安全性
- **跨平台支持**：支持多种操作系统和架构

## 命令行工具

该技能提供了命令行工具，支持以下命令：

- `start`：启动 Disruptor 服务
- `stop`：停止 Disruptor 服务
- `status`：查看 Disruptor 服务状态
- `reset`：重置 Disruptor 服务状态
- `demo`：运行 Disruptor 演示程序

使用示例：
```
disruptor_aot.exe start
disruptor_aot.exe status
disruptor_aot.exe demo
```