# TPL 技能参考文档

## 概述

TPL（Task Parallel Library）技能是一个基于 .NET 任务并行库的示例实现，展示了如何使用 TPL 进行并行计算、数据流处理和异步编程。本参考文档提供了详细的使用指南、API 参考和配置说明。

## 目录结构

```
tpl/
├── index.yaml          # 技能配置文件
├── SKILL.md            # 技能文档
├── scripts/            # 脚本文件
│   ├── tpl_core.cs                 # 核心TPL实现
│   ├── tpl_core.setting.json       # 编译配置
│   ├── tpl_core.run.json           # 运行配置
│   ├── tpl_generator.cs            # Scrutor演示
│   ├── tpl_generator.setting.json  # Scrutor编译配置
│   └── tpl_generator.run.json      # Scrutor运行配置
└── reference/          # 参考文档
    ├── README.md       # 本参考文档
    └── examples.md     # 使用示例
```

## 核心功能

### 1. 并行任务处理

- **Task.Run** - 在后台线程上运行任务
- **Task.WhenAll** - 等待所有任务完成
- **Task.WhenAny** - 等待任一任务完成
- **Parallel.ForEach** - 并行遍历集合
- **Parallel.Invoke** - 并行执行多个操作

### 2. 数据流处理

- **TransformBlock** - 转换数据
- **ActionBlock** - 处理数据
- **BufferBlock** - 缓存数据
- **JoinBlock** - 合并数据
- **BatchBlock** - 批处理数据

### 3. 并行 LINQ (PLINQ)

- **AsParallel** - 将序列转换为并行序列
- **WithDegreeOfParallelism** - 设置并行度
- **WithCancellation** - 支持取消操作
- **WithMergeOptions** - 控制合并行为

### 4. 异步编程

- **async/await** - 异步方法和等待
- **Task.Yield** - 让出执行权
- **Task.Delay** - 异步延迟
- **CancellationToken** - 取消令牌

## API 参考

### ITaskService

```csharp
public interface ITaskService
{
    Task RunParallelTasksAsync(int taskCount, CancellationToken cancellationToken = default);
    Task<T[]> WhenAllAsync<T>(params Task<T>[] tasks);
    Task<T> WhenAnyAsync<T>(params Task<T>[] tasks);
}
```

### IDataflowService

```csharp
public interface IDataflowService
{
    Task ProcessDataflowPipelineAsync(IEnumerable<int> data, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> ProcessWithTransformBlockAsync(IEnumerable<string> data);
}
```

### IParallelService

```csharp
public interface IParallelService
{
    IEnumerable<T> ParallelForEach<T>(IEnumerable<T> source, Action<T> action);
    T[] ParallelInvoke<T>(params Func<T>[] functions);
    double[] PLINQTransform(IEnumerable<double> source);
}
```

### IAsyncService

```csharp
public interface IAsyncService
{
    Task<long> CalculateFactorialAsync(int n);
    Task<string> SimulateAsyncOperationAsync(string input, int delayMs);
    Task<int> RetryAsync(Func<Task<int>> operation, int maxRetries);
}
```

## 命令行接口

### 基本命令

```bash
# 运行并行任务
tpl_core task --count 10

# 运行数据流操作
tpl_core dataflow --count 20

# 运行并行处理
tpl_core parallel --count 1000

# 运行异步操作
tpl_core async --simulate "test" --delay 1000
# 计算阶乘
tpl_core async --factorial 10
```

### Scrutor 演示命令

```bash
# 运行所有Scrutor演示
tpl_generator demo --type all

# 运行基本注册演示
tpl_generator demo --type basic

# 运行装饰器模式演示
tpl_generator demo --type decorator

# 运行生命周期管理演示
tpl_generator demo --type lifetime

# 显示Scrutor信息
tpl_generator info
```

## 配置说明

### 编译配置 (setting.json)

主要配置项：

- **publishAot**: 启用 AOT 编译
- **trimMode**: 裁剪模式（partial）
- **selfContained**: 自包含发布
- **publishSingleFile**: 单文件发布
- **targetFramework**: 目标框架（net11.0）

### 运行配置 (run.json)

主要配置项：

- **environmentVariables**: 环境变量设置
- **profiles**: 不同的运行配置文件
- **logging**: 日志配置
- **performance**: 性能监控配置

## 环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|-------|
| DOTNET_ENVIRONMENT | 运行环境 | Development |
| DOTNET_LOG_LEVEL | 日志级别 | Information |
| DOTNET_TieredCompilation | 分层编译 | 1 |
| DOTNET_gcServer | 服务器GC | 1 |
| TPL_MAX_DEGREE_OF_PARALLELISM | 最大并行度 | -1 (自动) |
| SCRUTOR_DEMO_MODE | Scrutor演示模式 | all |

## 性能优化

### 1. 并行度设置

- 根据 CPU 核心数设置合适的并行度
- 使用 `Environment.ProcessorCount` 获取核心数
- 避免过度并行导致的上下文切换开销

### 2. 内存管理

- 使用对象池减少 GC 压力
- 避免在并行循环中创建大量临时对象
- 考虑使用 `Span<T>` 和 `Memory<T>` 进行内存优化

### 3. 任务调度

- 使用 `TaskScheduler` 自定义任务调度
- 考虑使用 `ConcurrentQueue` 进行任务队列管理
- 避免任务嵌套过深

### 4. 数据流优化

- 设置合适的 `MaxDegreeOfParallelism`
- 合理配置 `BoundedCapacity` 避免内存溢出
- 使用 `DataflowLinkOptions.PropagateCompletion` 正确传播完成状态

## 最佳实践

1. **使用 async/await 而非 ContinueWith**
2. **始终处理 CancellationToken**
3. **使用 Task.WhenAll 而非多次 await**
4. **避免在并行操作中使用共享状态**
5. **使用 PLINQ 处理大数据集合**
6. **使用数据流处理复杂的管道操作**
7. **合理设置并行度，避免过度并行**
8. **使用对象池减少内存分配**
9. **考虑使用 Channel<T> 进行异步通信**
10. **始终测试不同并行度的性能表现**

## 常见问题

### 1. 并行性能不如预期

**原因**：
- 并行度设置不当
- 共享状态导致的竞争条件
- 任务创建开销大于计算开销

**解决方案**：
- 调整并行度
- 使用线程安全的集合
- 考虑批处理减少任务创建开销

### 2. 内存使用过高

**原因**：
- 数据流块的 `BoundedCapacity` 未设置
- 并行操作创建过多临时对象
- 任务队列过长

**解决方案**：
- 设置合适的 `BoundedCapacity`
- 使用对象池
- 考虑使用背压机制

### 3. 死锁

**原因**：
- 任务等待层次过深
- 阻塞调用与异步调用混用
- 不正确的任务延续

**解决方案**：
- 避免嵌套的阻塞等待
- 一致使用 async/await
- 正确使用 `ConfigureAwait(false)`

### 4. 取消操作不生效

**原因**：
- 未正确传递 `CancellationToken`
- 操作中未检查取消状态
- 长时间运行的同步操作

**解决方案**：
- 确保传递 `CancellationToken`
- 定期检查 `cancellationToken.IsCancellationRequested`
- 将长时间操作拆分为可取消的小块

## 依赖项

| 包名 | 版本 | 用途 |
|-----|------|------|
| System.CommandLine | 2.0.0-beta4.22272.1 | 命令行接口 |
| Microsoft.Extensions.DependencyInjection | 8.0.0 | 依赖注入 |
| Microsoft.Extensions.Logging | 8.0.0 | 日志系统 |
| Microsoft.Extensions.Logging.Console | 8.0.0 | 控制台日志 |
| System.Threading.Tasks.Dataflow | 8.0.0 | 数据流处理 |
| Scrutor | 4.2.0 | 依赖注入装饰器模式 |

## AOT 编译

本技能支持 AOT 编译，主要配置：

```json
{
  "publishAot": true,
  "trimMode": "partial",
  "selfContained": true,
  "publishSingleFile": true
}
```

### AOT 编译注意事项

1. **反射使用**：避免在运行时使用反射创建类型
2. **动态代码**：避免使用 `dynamic` 类型
3. **序列化**：确保序列化库支持 AOT
4. **配置验证**：编译前验证配置

## 部署指南

### 本地部署

1. **编译**：使用 `dotnet publish` 命令编译
2. **运行**：直接执行生成的可执行文件

### Docker 部署

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["tpl/scripts/tpl_core.cs", "tpl/scripts/"]
RUN dotnet publish "tpl/scripts/tpl_core.cs" -c Release -o /app/publish /p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./tpl_core"]
```

### 云部署

- **Azure Functions**：使用 .NET 隔离模式
- **AWS Lambda**：使用 .NET 运行时
- **Google Cloud Functions**：使用 .NET 运行时

## 监控与日志

### 日志级别

- **Debug**：详细调试信息
- **Information**：一般信息
- **Warning**：警告信息
- **Error**：错误信息
- **Critical**：严重错误信息

### 性能监控

- **metricsPort**：性能指标端口（默认为 5000/5001）
- **enableMetrics**：启用性能监控

## 安全考虑

1. **输入验证**：始终验证命令行参数
2. **异常处理**：妥善处理异常，避免信息泄露
3. **资源管理**：使用 `using` 语句管理资源
4. **并行安全**：确保并行操作的线程安全性

## 扩展与自定义

### 扩展核心服务

1. **实现自定义服务接口**
2. **注册到依赖注入容器**
3. **使用装饰器模式扩展现有服务**

### 自定义数据流管道

1. **创建自定义数据流块**
2. **配置块选项**
3. **链接块形成管道**

### 自定义命令

1. **继承 `Command` 类**
2. **添加选项和参数**
3. **设置命令处理器**

## 示例代码

详细的示例代码请参考 [examples.md](examples.md) 文件。

## 版本历史

| 版本 | 日期 | 变更 |
|-----|------|------|
| 1.0.0 | 2026-01-25 | 初始版本 |

## 贡献指南

欢迎贡献代码和文档！请遵循以下流程：

1. Fork 仓库
2. 创建功能分支
3. 提交变更
4. 创建 Pull Request

## 许可证

本项目采用 MIT 许可证。详见 [LICENSE](LICENSE) 文件。

## 联系方式

- **作者**：.NET 专家
- **邮箱**：contact@example.com
- **GitHub**：https://github.com/example/tpl-skill
