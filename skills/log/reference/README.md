# log - 参考文档

## 概述

log 是一个基于 .NET 10 的高性能日志系统，专为 .NET 开发者设计。

## 核心组件

### 1. LogProcessor（日志处理器）
- **位置**: scripts/serilog_integration.cs
- **功能**: 处理日志记录和格式化
- **特性**: 
  - 高性能日志处理
  - Threading.Channels 事件处理
  - Span 零拷贝优化
  - 多提供商支持

### 2. LogService（日志服务）
- **位置**: scripts/serilog_integration.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 结构化日志
  - 异步处理
  - 错误处理
  - 日志记录

## 使用示例

### 基本使用

`csharp
var logService = serviceProvider.GetRequiredService<ILogService>();
await logService.LogInformationAsync("Application started");
await logService.LogErrorAsync("An error occurred", new Exception("Test error"));
`

### 高级配置

`csharp
var settings = new LogSetting {
    EnableAsyncProcessing = true,
    BatchSize = 100,
    FlushInterval = TimeSpan.FromSeconds(5),
    MinimumLogLevel = LogLevel.Information
};

builder.Services.Configure<LogSetting>(options => {
    options.EnableAsyncProcessing = settings.EnableAsyncProcessing;
    options.BatchSize = settings.BatchSize;
    options.FlushInterval = settings.FlushInterval;
    options.MinimumLogLevel = settings.MinimumLogLevel;
});
`

## 配置选项

### Log 配置

`json
{
  "LogSetting": {
    "EnableAsyncProcessing": true,          // 启用异步处理
    "BatchSize": 100,            // 批处理大小
    "FlushInterval": "00:00:05",        // 刷新间隔
    "MinimumLogLevel": "Information",        // 最小日志级别
    "EnableDetailedLogging": false          // 启用详细日志
  }
}
`

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **连接池**: 使用连接池管理资源
5. **Channel 事件处理**: 使用 Threading.Channels 实现高效的事件队列
6. **Span 零拷贝**: 使用 Span 减少内存分配和复制
7. **对象池**: 使用 ObjectPool 减少对象创建开销

## AOT 编译配置

### 构建配置

```yaml
#:sdk Microsoft.NET.Sdk.Web
#:package Serilog@4.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
```

### 发布命令

```bash
# AOT 发布命令
dotnet publish scripts/serilog_integration.cs -c Release -r win-x64 --aot

# Linux 发布命令
dotnet publish scripts/serilog_integration.cs -c Release -r linux-x64 --aot

# macOS 发布命令
dotnet publish scripts/serilog_integration.cs -c Release -r osx-x64 --aot
```

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件
   - 验证网络连接
   - 检查日志信息

2. **性能问题**
   - 启用异步处理
   - 优化批处理大小
   - 增加资源限制
   - 检查 Channel 配置

3. **内存问题**
   - 调整批处理大小
   - 优化内存使用
   - 检查 Span 使用

## 扩展开发

### 添加自定义功能

`csharp
public class CustomLogService : ILogService
{
    public async Task LogInformationAsync(string message, params object[] args)
    {
        // 实现自定义逻辑
        Console.WriteLine($"Custom info: {string.Format(message, args)}");
    }
    
    public async Task LogErrorAsync(string message, Exception exception, params object[] args)
    {
        // 实现自定义逻辑
        Console.WriteLine($"Custom error: {string.Format(message, args)}");
        Console.WriteLine($"Exception: {exception.Message}");
    }
}
`

### 扩展处理器

`csharp
public class CustomLogProcessor : ILogProcessor
{
    public async Task ProcessAsync(LogEntry logEntry, LogSetting settings)
    {
        // 实现自定义处理逻辑
        Console.WriteLine($"Processing log: {logEntry.Message}");
    }
}
`
