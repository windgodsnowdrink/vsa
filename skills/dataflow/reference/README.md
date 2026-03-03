# Dataflow AOT - 参考文档

## 概述

Dataflow AOT是基于.NET 10 AOT架构的高性能数据流处理框架，专为.NET开发者设计，提供强大、高效的数据处理能力。它基于System.Threading.Tasks.Dataflow库，支持多种数据流处理模式，包括简单数据流、批处理数据流、复杂多步骤数据流和高性能管道处理。

## 核心组件

### 1. IDataflowService (Dataflow服务接口)
- **位置**: scripts/dataflow_aot.cs
- **功能**: 定义Dataflow的核心功能接口
- **特性**: 
  - 支持多种数据流处理模式
  - 可配置的并行度
  - 批处理支持
  - 背压机制
  - 性能监控

### 2. DataflowService (Dataflow服务实现)
- **位置**: scripts/dataflow_aot.cs
- **功能**: 实现IDataflowService接口，提供核心Dataflow功能
- **特性**: 
  - 高性能实现
  - 详细日志记录
  - 性能指标收集
  - 错误处理
  - 异步编程模型

### 3. DataflowAotEngine (Dataflow AOT执行引擎)
- **位置**: scripts/dataflow_aot.cs
- **功能**: 管理Dataflow功能调用的执行引擎
- **特性**: 
  - 统一的API入口
  - 简化的调用方式
  - 内置的性能监控
  - 灵活的配置选项

### 4. DataflowOptions (Dataflow配置选项)
- **位置**: scripts/dataflow_aot.cs
- **功能**: 配置Dataflow的行为
- **特性**: 
  - 支持多种配置来源
  - 灵活的选项设置
  - 默认值合理
  - 类型安全

## 使用示例

### 基本用法

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

### 高级配置

```csharp
// 自定义Dataflow配置
builder.Services.AddDataflow(options => {
    options.DefaultMaxDegreeOfParallelism = 16;
    options.DefaultBatchSize = 200;
    options.EnableDetailedLogging = true;
    options.EnableBackpressure = true;
    options.BackpressureThreshold = 1000;
});

// 或者通过配置文件配置
builder.Configuration.AddJsonFile("dataflow_aot.setting.json");
builder.Services.Configure<Dataflow.AOT.DataflowOptions>(builder.Configuration.GetSection("Dataflow"));
builder.Services.AddDataflow();
```

## 配置选项

### Dataflow配置

```json
{
  "Dataflow": {
    "EnableDataflowEngine": true,          // 是否启用Dataflow引擎
    "DefaultBatchSize": 100,               // 默认批处理大小
    "DefaultMaxDegreeOfParallelism": 8,    // 默认并行度
    "DefaultBufferSize": 1000,             // 默认缓冲区大小
    "EnableDetailedLogging": false,        // 是否启用详细日志
    "EnablePerformanceMonitoring": true,   // 是否启用性能监控
    "Timeout": "00:00:30",                // 超时时间
    "MaxRetryCount": 3,                    // 最大重试次数
    "RetryInterval": "00:00:00.5",        // 重试间隔
    "EnableBackpressure": true,            // 是否启用背压机制
    "BackpressureThreshold": 800           // 背压阈值
  }
}
```

## 性能优化

1. **启用AOT编译** - 启用AOT编译以获得最佳性能
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

### 自定义数据流处理步骤

```csharp
// 自定义数据流处理步骤
public class CustomTransformStep
{
    public async Task<T> ProcessAsync<T>(T input)
    {
        // 实现自定义转换逻辑
        await Task.Delay(5); // 模拟处理延迟
        return input;
    }
}

// 使用自定义步骤
var customStep = new CustomTransformStep();
var result = await engine.ProcessSimpleFlowAsync(
    inputData,
    async item => await customStep.ProcessAsync(item),
    4);
```

