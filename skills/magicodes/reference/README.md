# magicodes - 参考文档

## 概述

magicodes 是基于 .NET 10 开发的高性能文档处理系统，专为 .NET 开发者设计，提供全面的文档导入导出功能。

## 核心组件

### 1. DocumentService（文档服务）
- **位置**: scripts/magicodes_ie_integration.cs
- **功能**: 核心文档处理业务逻辑
- **特性**: 
  - 支持 Excel、Word、PDF、HTML 等多种文档格式
  - 高性能异步处理
  - 错误处理和日志记录
  - 使用 Threading.Channels 进行任务队列管理
  - 对象池优化内存使用

## AOT 架构说明

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### 性能优化技术

1. **Threading.Channels**: 高效的异步事件队列处理，支持背压控制
2. **ObjectPool**: 减少对象创建开销，优化内存使用
3. **Span 零拷贝**: 减少内存分配和复制
4. **TailLatencyOptimizer**: 尾延迟优化
5. **AggressiveOptimization**: 编译器级优化

## 使用示例

### 基本用法

```csharp
var documentService = serviceProvider.GetRequiredService<DocumentService>();

// Excel导出
var data = new List<ExportModel> { /* 数据 */ };
await documentService.ExportExcelAsync(data, "output.xlsx");

// Excel导入
var importedData = await documentService.ImportExcelAsync<ImportModel>("input.xlsx");
```

### 高级配置

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置文档处理服务
builder.Services.Configure<DocumentProcessingOptions>(options => {
    options.MaxConcurrentRequests = 100;
    options.ChannelCapacity = 10000;
    options.StreamPoolSize = 16;
    options.TimeoutSeconds = 300;
});

// 注册服务
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddSingleton<IImporter, MagicodesImporter>();
builder.Services.AddSingleton<IExporter, MagicodesExporter>();
```

## 配置选项

### 文档处理配置

```json
{
  "DocumentProcessing": {
    "MaxConcurrentRequests": 100,      // 最大并发请求数
    "ChannelCapacity": 10000,          // 通道容量
    "StreamPoolSize": 16,              // 流池大小
    "TimeoutSeconds": 300              // 超时时间（秒）
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理提高效率
4. **对象池**: 使用对象池管理资源
5. **通道队列**: 使用 Channels 进行任务调度

## 故障排除

### 常见问题

1. **文档处理失败**
   - 检查文件格式是否支持
   - 验证文件路径是否正确
   - 查看日志信息

2. **性能问题**
   - 调整通道容量和并发设置
   - 启用对象池
   - 优化数据量大小

## 扩展开发

### 添加自定义功能

```csharp
public class CustomDocumentService : DocumentService
{
    public CustomDocumentService(IImporter importer, IExporter exporter)
        : base(importer, exporter)
    {
    }

    // 自定义方法
    public async Task<byte[]> ProcessCustomDocumentAsync(byte[] documentData)
    {
        // 实现自定义文档处理逻辑
        return processedData;
    }
}
```
