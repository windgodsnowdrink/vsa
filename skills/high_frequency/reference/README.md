# 高频处理 - 参考文档

## 概述

高频处理是基于 .NET 10 的高性能高频处理系统，专为 .NET 开发者设计，支持 AOT 编译，提供单文件执行脚本，具有极高的性能和低内存占用。

## 核心组件

### 1. 高频处理 AOT 引擎
- **位置**: scripts/high_frequency_aot.cs
- **功能**: 核心高频处理业务逻辑，包括消息处理、内存管理和并发控制
- **特性**: 
  - 高频消息处理：支持每秒处理数百万条消息
  - 零拷贝优化：使用 Span<T> 和 Memory<T> 实现零拷贝
  - 管道处理：基于 System.IO.Pipelines 的高性能管道处理
  - 通道处理：基于 System.Threading.Channels 的并发处理
  - 批处理优化：支持批量处理提高吞吐量
  - 内存优化：智能内存管理和对象池
  - AOT 编译优化：使用 .NET 10 AOT 编译，提高性能和降低内存占用
  - 命令行工具：完整的命令行界面，支持多种操作和别名
  - 配置管理：支持环境变量和配置文件配置
  - 性能监控：内置执行时间监控和指标收集

## 使用示例

### 基本用法

```csharp
// 初始化服务
var serviceProvider = BuildServiceProvider();
var highFrequencyService = serviceProvider.GetRequiredService<HighFrequencyService>();

// 使用高频处理功能
var result = await highFrequencyService.ProcessMessagesAsync(1_000_000);
Console.WriteLine($"处理消息: {result.ProcessedMessages:N0}");
Console.WriteLine($"吞吐量: {result.Throughput:F2} msg/s");
Console.WriteLine($"平均延迟: {result.AverageLatency:F3} μs");

// 使用管道处理
var pipelineResult = await highFrequencyService.ProcessMessagesWithPipelineAsync(1_000_000);
Console.WriteLine($"管道处理吞吐量: {pipelineResult.Throughput:F2} msg/s");

// 使用通道处理
var channelResult = await highFrequencyService.ProcessMessagesWithChannelAsync(1_000_000);
Console.WriteLine($"通道处理吞吐量: {channelResult.Throughput:F2} msg/s");

// 释放资源
await highFrequencyService.DisposeAsync();
```

### 高级配置

```csharp
var highFrequencySettings = new HighFrequencySettings {
    MessageCount = 1_000_000,
    ConcurrencyLevel = Environment.ProcessorCount,
    BufferSize = 65536,
    EnableZeroCopy = true,
    EnableBatchProcessing = true,
    BatchSize = 1000,
    EnableCompression = false,
    EnableMetrics = true
};

builder.Services.Configure<HighFrequencySettings>(options => {
    options.MessageCount = highFrequencySettings.MessageCount;
    options.ConcurrencyLevel = highFrequencySettings.ConcurrencyLevel;
    options.BufferSize = highFrequencySettings.BufferSize;
    options.EnableZeroCopy = highFrequencySettings.EnableZeroCopy;
    options.EnableBatchProcessing = highFrequencySettings.EnableBatchProcessing;
    options.BatchSize = highFrequencySettings.BatchSize;
    options.EnableCompression = highFrequencySettings.EnableCompression;
    options.EnableMetrics = highFrequencySettings.EnableMetrics;
});
```

## 配置选项

### 高频处理配置

```json
{
  "HighFrequencySettings": {
    "MessageCount": 1000000,           // 消息数量
    "ConcurrencyLevel": 8,             // 并发级别
    "BufferSize": 65536,               // 缓冲区大小
    "EnableZeroCopy": true,            // 启用零拷贝
    "EnableBatchProcessing": true,     // 启用批处理
    "BatchSize": 1000,                 // 批处理大小
    "EnableCompression": false,        // 启用压缩
    "EnableMetrics": true              // 启用指标
  }
}
```

## 性能优化

1. **AOT 编译**: 使用 .NET 10 AOT 编译，提高性能和降低内存占用
2. **零拷贝优化**: 使用 Span<T> 和 Memory<T> 实现零拷贝
3. **批处理**: 批量处理提高效率
4. **内存管理**: 使用对象池和缓存减少内存分配
5. **并发控制**: 合理配置线程池大小，支持高并发
6. **异步编程**: 使用异步 API 避免阻塞
7. **管道处理**: 基于 System.IO.Pipelines 的高性能管道处理
8. **通道处理**: 基于 System.Threading.Channels 的并发处理

## 故障排除

### 常见问题

1. **内存不足**
   - 减少消息数量
   - 增加批处理大小
   - 调整缓冲区大小

2. **性能下降**
   - 确保启用 AOT 编译
   - 检查并发级别设置
   - 验证零拷贝是否启用

3. **编译错误**
   - 确保使用 .NET 10 SDK
   - 检查依赖项版本
   - 验证 AOT 编译配置

4. **运行时错误**
   - 检查配置文件
   - 验证环境变量
   - 检查日志信息

## 扩展开发

### 添加自定义消息处理器

```csharp
public interface IMessageProcessor
{
    Task<ProcessingResult> ProcessMessagesAsync(long messageCount);
}

public class CustomMessageProcessor : IMessageProcessor
{
    public async Task<ProcessingResult> ProcessMessagesAsync(long messageCount)
    {
        var processed = 0L;
        var stopwatch = Stopwatch.StartNew();
        
        for (long i = 0; i < messageCount; i++)
        {
            // 自定义消息处理逻辑
            ProcessMessage(i);
            processed++;
        }
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        
        return new ProcessingResult
        {
            ProcessedMessages = processed,
            Throughput = processed / elapsedSeconds,
            AverageLatency = 0,
            MaxLatency = 0,
            MinLatency = 0
        };
    }
    
    private void ProcessMessage(long id)
    {
        // 自定义消息处理实现
    }
}

// 注册服务
builder.Services.AddSingleton<IMessageProcessor, CustomMessageProcessor>();
```

### 添加新的序列化器

```csharp
public interface ISerializer
{
    byte[] Serialize<T>(T value);
    T Deserialize<T>(byte[] data);
}

public class CustomSerializer : ISerializer
{
    public byte[] Serialize<T>(T value)
    {
        // 自定义序列化实现
        return Array.Empty<byte>();
    }
    
    public T Deserialize<T>(byte[] data)
    {
        // 自定义反序列化实现
        return default;
    }
}

// 注册服务
builder.Services.AddSingleton<ISerializer, CustomSerializer>();
```

## AOT 编译指南

### 编译命令

```bash
# 编译 AOT 版本
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial

# 编译 Linux 版本
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial

# 编译 macOS 版本
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial
```

### 编译选项

- **Target Framework**: net11.0
- **Runtime Identifier**: win-x64, linux-x64, osx-x64
- **Trim Mode**: partial
- **Publish Aot**: true
- **Optimize**: true
- **Debug Type**: none

### 部署指南

1. **准备环境**: 无需安装 .NET 运行时
2. **复制文件**: 将编译后的单文件可执行文件复制到目标机器
3. **配置环境**: 设置必要的环境变量
4. **启动服务**: 运行可执行文件

### 监控和维护

1. **性能监控**: 定期检查吞吐量和延迟指标
2. **内存监控**: 监控内存使用情况，避免内存泄漏
3. **日志分析**: 分析日志文件，识别潜在问题
4. **定期更新**: 及时更新到最新版本，获取性能改进
