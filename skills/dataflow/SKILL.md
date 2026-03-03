# Dataflow AOT Agent Skill - Dataflow AOT高性能数据流处理

## 技能概述

基于.NET 10 AOT架构的高性能Dataflow数据流处理技能，为.NET开发者提供强大、高效的数据处理能力，支持简单数据流、批处理数据流、复杂多步骤数据流和高性能管道处理等核心功能，适合在各种环境下运行，包括容器化部署和无依赖运行。

## 快速入门指南

### 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Tasks.Dataflow@9.0.0
```

### 配置AOT编译

在项目文件中添加以下属性：

```yaml
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true
```

### 注册服务

在主应用程序中注册Dataflow服务：

```csharp
// 配置Dataflow选项
builder.Configuration.AddJsonFile("dataflow_aot.setting.json");
builder.Services.Configure<Dataflow.AOT.DataflowOptions>(builder.Configuration.GetSection("Dataflow"));

// 注册Dataflow服务
builder.Services.AddDataflow();
```

### 使用示例

```csharp
// 获取Dataflow AOT引擎
var engine = serviceProvider.GetRequiredService<Dataflow.AOT.DataflowAotEngine>();

// 准备测试数据
var testData = Enumerable.Range(1, 100).Select(i => new { Id = i, Value = i * 2 }).ToList();

// 简单数据流处理
var simpleResult = await engine.ProcessSimpleFlowAsync(
    testData,
    async item => {
        // 模拟处理延迟
        await Task.Delay(10);
        return new { item.Id, item.Value, Processed = true, Timestamp = DateTime.Now };
    },
    4);

Console.WriteLine($"简单数据流处理完成，结果: {simpleResult.Success ? "成功" : "失败"}");
if (simpleResult.Success)
{
    Console.WriteLine($"处理项目数: {simpleResult.ProcessedItems}");
    Console.WriteLine($"执行时间: {simpleResult.ExecutionTimeMs} ms");
}
```

## 目录结构

```
dataflow/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── dataflow_aot.cs         # Dataflow AOT核心实现
    ├── dataflow_aot.run.json    # 运行配置
    ├── dataflow_aot.setting.json  # 设置文件
    └── ...                     # 其他Dataflow相关脚本
```

## 主要功能

1. **简单数据流处理**：支持基本的单步骤数据流转换，可配置并行度
2. **批处理数据流**：支持按批次处理数据，提高处理效率
3. **复杂多步骤数据流**：支持多阶段数据流处理，每阶段可配置不同的并行度
4. **高性能管道处理**：支持构建可扩展的管道处理流程
5. **背压机制**：内置背压支持，防止系统过载
6. **性能监控**：内置性能指标收集和监控
7. **详细日志记录**：支持不同级别的日志记录
8. **灵活配置**：支持通过配置文件和环境变量进行配置
9. **依赖注入**：完全支持.NET依赖注入
10. **异步编程**：基于async/await的异步API设计

## 技术架构

### 核心组件

1. **IDataflowService** - 定义Dataflow的核心功能接口
2. **DataflowService** - 实现IDataflowService接口，提供Dataflow功能的核心实现
3. **DataflowAotEngine** - 管理Dataflow功能调用的执行引擎
4. **DataflowOptions** - 配置选项类，用于控制Dataflow的行为
5. **DataflowResult** - Dataflow操作结果类，用于返回操作结果
6. **DataflowStatus** - 状态信息类，用于返回Dataflow的状态
7. **DataflowExtensions** - Dataflow扩展，简化服务注册

### 技术特性

- **.NET 10 AOT编译** - 提供原生性能，减少启动时间和内存占用
- **System.Threading.Tasks.Dataflow** - 基于官方Dataflow库，提供完整的数据流功能
- **依赖注入** - 支持IoC容器，便于扩展和测试
- **选项模式** - 支持灵活的配置管理
- **异步编程** - 支持非阻塞操作，提高并发性能
- **日志记录** - 提供详细的日志信息，便于调试和监控
- **性能监控** - 内置性能指标收集
- **命令行接口** - 支持脚本化使用
- **灵活配置** - 支持通过配置文件和环境变量进行配置

### 执行流程

1. 创建DataflowAotEngine实例
2. 调用相应的Dataflow功能方法，如ProcessSimpleFlowAsync、ProcessBatchFlowAsync等
3. DataflowAotEngine将请求转发给DataflowService
4. DataflowService使用System.Threading.Tasks.Dataflow库执行实际的数据流操作
5. 数据流处理完成后，将执行结果返回给调用者

## 配置选项

### 配置文件格式

```json
{
  "Dataflow": {
    "EnableDataflowEngine": true,
    "DefaultBatchSize": 100,
    "DefaultMaxDegreeOfParallelism": 8,
    "DefaultBufferSize": 1000,
    "EnableDetailedLogging": false,
    "EnablePerformanceMonitoring": true,
    "Timeout": "00:00:30",
    "MaxRetryCount": 3,
    "RetryInterval": "00:00:00.5",
    "EnableBackpressure": true,
    "BackpressureThreshold": 800
  }
}
```

### 配置选项说明

| 选项名称 | 类型 | 默认值 | 说明 |
|---------|------|-------|------|
| EnableDataflowEngine | bool | true | 是否启用Dataflow引擎 |
| DefaultBatchSize | int | 100 | 默认批处理大小 |
| DefaultMaxDegreeOfParallelism | int | 8 | 默认并行度 |
| DefaultBufferSize | int | 1000 | 默认缓冲区大小 |
| EnableDetailedLogging | bool | false | 是否启用详细日志 |
| EnablePerformanceMonitoring | bool | true | 是否启用性能监控 |
| Timeout | TimeSpan | 30秒 | 操作超时时间 |
| MaxRetryCount | int | 3 | 最大重试次数 |
| RetryInterval | TimeSpan | 500毫秒 | 重试间隔 |
| EnableBackpressure | bool | true | 是否启用背压机制 |
| BackpressureThreshold | int | 800 | 背压阈值 |

## 命令行使用

### 命令格式

```
dataflow_aot.exe <command> [arguments]
```

### 命令参数

| 命令 | 说明 | 参数 |
|-----|------|------|
| status | 获取服务状态 | 无 |
| reset | 重置服务状态 | 无 |
| demo | 运行演示数据流 | 无 |

### 示例

```
# 获取服务状态
dataflow_aot.exe status

# 重置服务状态
dataflow_aot.exe reset

# 运行演示数据流
dataflow_aot.exe demo
```

## 扩展开发

### 自定义Dataflow服务

```csharp
// 自定义Dataflow服务实现
public class CustomDataflowService : Dataflow.AOT.DataflowService
{
    public CustomDataflowService(ILogger<DataflowService> logger, IOptions<Dataflow.AOT.DataflowOptions> options)
        : base(logger, options)
    {
    }
    
    // 重写ProcessSimpleFlowAsync方法，添加自定义逻辑
    public override async Task<Dataflow.AOT.DataflowResult<List<TOutput>>> ProcessSimpleFlowAsync<TInput, TOutput>(
        List<TInput> inputData,
        Func<TInput, Task<TOutput>> transformFunc,
        int? parallelism = null)
    {
        // 自定义预处理逻辑
        _logger.LogInformation("自定义简单数据流处理开始");
        
        // 调用基类方法执行处理
        var result = await base.ProcessSimpleFlowAsync(inputData, transformFunc, parallelism);
        
        // 自定义后处理逻辑
        _logger.LogInformation("自定义简单数据流处理完成");
        
        return result;
    }
}

// 注册自定义服务
builder.Services.AddSingleton<Dataflow.AOT.IDataflowService, CustomDataflowService>();
builder.Services.AddSingleton<Dataflow.AOT.DataflowAotEngine>();
```

### 自定义配置

```csharp
// 自定义Dataflow配置
builder.Services.AddDataflow(options => {
    options.DefaultMaxDegreeOfParallelism = 16;
    options.DefaultBatchSize = 200;
    options.EnableDetailedLogging = true;
    options.EnableBackpressure = true;
    options.BackpressureThreshold = 1000;
});
```

## 最佳实践

1. **使用AOT编译** - 启用AOT编译以获得最佳性能
2. **合理配置并行度** - 根据系统资源和工作负载调整并行度
3. **使用异步API** - 优先使用异步API，提高并发性能
4. **合理配置批处理大小** - 根据数据特性和处理逻辑调整批处理大小
5. **启用性能监控** - 在生产环境中启用性能监控，便于调优和故障排除
6. **使用背压机制** - 启用背压机制防止系统过载
7. **合理配置缓冲区大小** - 根据内存资源和处理速度调整缓冲区大小
8. **处理异常情况** - 合理处理Dataflow操作可能出现的异常情况
9. **使用依赖注入** - 使用依赖注入管理Dataflow服务和客户端
10. **监控服务状态** - 定期监控Dataflow服务的状态，及时发现问题

## 性能优化

1. **启用AOT编译** - AOT编译可以减少启动时间和内存占用
2. **调整并行度** - 根据CPU核心数调整并行度，充分利用系统资源
3. **优化批处理大小** - 过大或过小的批处理大小都会影响性能，需要根据实际情况调整
4. **使用适当的缓冲区大小** - 合理的缓冲区大小可以提高吞吐量，减少内存占用
5. **启用背压机制** - 背压机制可以防止系统过载，提高系统稳定性
6. **优化转换函数** - 转换函数的性能直接影响整体性能，需要优化转换逻辑
7. **减少日志开销** - 在生产环境中适当降低日志级别，减少日志开销
8. **使用性能监控** - 使用性能监控数据识别瓶颈，进行针对性优化

## 故障排除

### 常见问题

1. **Dataflow引擎未启用**
   - 检查配置文件中的EnableDataflowEngine选项是否设置为true
   - 检查环境变量DATAFLOW_ENABLE_ENGINE是否设置为true

2. **处理速度慢**
   - 检查并行度设置是否合理
   - 检查批处理大小是否合适
   - 检查转换函数是否存在性能瓶颈
   - 检查系统资源使用情况

3. **内存占用高**
   - 减小缓冲区大小
   - 减小批处理大小
   - 检查是否存在内存泄漏
   - 考虑使用更高效的数据结构

4. **系统过载**
   - 启用背压机制
   - 减小并行度
   - 减小缓冲区大小
   - 考虑增加系统资源

5. **处理失败**
   - 检查输入数据是否合法
   - 检查转换函数是否存在异常
   - 查看日志信息，了解具体错误原因
   - 检查配置选项是否正确

## 应用场景

1. **数据处理管道** - 构建复杂的数据处理管道，支持多阶段处理
2. **批处理作业** - 处理大量数据的批处理作业
3. **实时数据处理** - 处理实时数据流，如日志分析、事件处理等
4. **ETL过程** - 数据抽取、转换和加载过程
5. **图像处理** - 批量图像处理，如缩放、滤镜应用等
6. **文本处理** - 批量文本处理，如分析、转换、分类等
7. **机器学习** - 机器学习模型训练和推理过程中的数据处理
8. **科学计算** - 科学计算中的并行数据处理
9. **Web服务后端** - Web服务中的并行请求处理
10. **微服务架构** - 微服务架构中的数据流处理

## 相关资源

- [.NET 10 AOT编译文档](https://learn.microsoft.com/zh-cn/dotnet/core/deploying/native-aot/)
- [System.Threading.Tasks.Dataflow文档](https://learn.microsoft.com/zh-cn/dotnet/standard/parallel-programming/dataflow-task-parallel-library)
- [依赖注入文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/dependency-injection)
- [选项模式文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/options)
- [异步编程文档](https://learn.microsoft.com/zh-cn/dotnet/csharp/asynchronous-programming/)
- [日志记录文档](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/logging)

## 版本历史

### v1.0.0

- 初始版本
- 支持简单数据流处理
- 支持批处理数据流处理
- 支持复杂多步骤数据流处理
- 支持高性能管道处理
- 基于.NET 10 AOT架构
- 支持依赖注入和选项模式
- 支持异步编程和日志记录
- 支持性能监控和命令行接口
- 提供灵活的配置选项
