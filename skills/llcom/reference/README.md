# llcom - 参考文档

## 概述

llcom 是一个基于 .NET 10 的高性能 llcom 系统，专为 .NET 开发者设计。

## 核心组件

### 1. LLStreamProcessor（流处理器）
- **位置**: scripts/llcom_integration.cs
- **功能**: 处理串口数据流
- **特性**: 
  - 高性能数据处理
  - Threading.Channels 事件处理
  - Span 零拷贝优化
  - 数据格式解析

### 2. LLComService（llcom 服务）
- **位置**: scripts/llcom_integration.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 设备管理
  - 串口通信
  - 错误处理
  - 日志记录

## 使用示例

### 基本使用

`csharp
var llComService = serviceProvider.GetRequiredService<ILLComService>();
var device = await llComService.ConnectDeviceAsync();
await llComService.SendDataAsync(device.DeviceId, new byte[] { 0x01, 0x02, 0x03 });
`

### 高级配置

`csharp
var settings = new LLComSettings {
    EnableDataProcessing = true,
    MaxBufferSize = 1024 * 1024,
    ReadTimeout = TimeSpan.FromSeconds(30),
    WriteTimeout = TimeSpan.FromSeconds(10)
};

builder.Services.Configure<LLComSettings>(options => {
    options.EnableDataProcessing = settings.EnableDataProcessing;
    options.MaxBufferSize = settings.MaxBufferSize;
    options.ReadTimeout = settings.ReadTimeout;
    options.WriteTimeout = settings.WriteTimeout;
});
`

## 配置选项

### LLCom 配置

`json
{
  "LLComSettings": {
    "EnableDataProcessing": true,          // 启用数据处理
    "MaxBufferSize": 1048576,           // 最大缓冲区大小
    "ReadTimeout": "00:00:30",        // 读取超时
    "WriteTimeout": "00:00:10",        // 写入超时
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
#:package LLCOM.SDK@11.0.0
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
dotnet publish scripts/llcom_integration.cs -c Release -r win-x64 --aot

# Linux 发布命令
dotnet publish scripts/llcom_integration.cs -c Release -r linux-x64 --aot

# macOS 发布命令
dotnet publish scripts/llcom_integration.cs -c Release -r osx-x64 --aot
```

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件
   - 验证设备连接
   - 检查日志信息

2. **性能问题**
   - 启用缓存
   - 优化数据处理
   - 增加资源限制
   - 检查 Channel 配置

3. **内存问题**
   - 调整缓冲区大小
   - 优化内存使用
   - 检查 Span 使用

## 扩展开发

### 添加自定义功能

`csharp
public class CustomLLComService : ILLComService
{
    public async Task<LLDevice> ConnectDeviceAsync()
    {
        // 实现自定义逻辑
        return new LLDevice { DeviceId = "custom-device-1" };
    }
    
    public async Task SendDataAsync(string deviceId, byte[] data)
    {
        // 实现自定义逻辑
    }
}
`

### 扩展流处理器

`csharp
public class CustomLLStreamProcessor : ILLStreamProcessor
{
    public async Task ProcessDataAsync(ReadOnlyMemory<byte> data)
    {
        // 实现自定义数据处理逻辑
    }
    
    public async Task<byte[]> ParseDataAsync(ReadOnlyMemory<byte> rawData)
    {
        // 实现自定义数据解析逻辑
        return rawData.ToArray();
    }
}
`
