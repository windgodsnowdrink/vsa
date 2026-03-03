# magiconion - 参考文档

## 概述

magiconion 是基于 .NET 10 开发的高性能分布式系统，专为 .NET 开发者设计，提供强大的实时通信和分布式计算功能。

## 核心组件

### 1. MagiconionService（核心服务）
- **位置**: scripts/magiconion_chat.cs, scripts/magiconion_integration.cs 等
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 实时通信功能实现
  - 高性能分布式计算
  - 错误处理和恢复
  - 详细的日志记录

### 2. ChannelProcessor（通道处理器）
- **位置**: 各 C# 文件中
- **功能**: 高效的异步事件队列处理
- **特性**: 
  - 使用 Threading.Channels 进行背压控制
  - 支持高并发事件处理
  - 内存优化设计

### 3. StreamingProcessor（流式处理器）
- **位置**: scripts/magiconion_realtime.cs
- **功能**: 实时数据流处理
- **特性**: 
  - 低延迟数据传输
  - 高吞吐量处理
  - 零拷贝内存优化

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
6. **Cache-line 对齐**: 内存分配优化

## 使用示例

### 基本用法

```csharp
var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();

// 基本处理
var result = await magiconionService.ProcessAsync(data);
Console.WriteLine($"处理结果: {result}");

// 实时通信
var chatService = serviceProvider.GetRequiredService<IChatService>();
var joinResult = await chatService.JoinAsync("room1", "user1");
Console.WriteLine($"加入房间成功: {joinResult.Success}");
```

### 高级配置

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 Magiconion 服务
builder.Services.Configure<MagicOnionOptions>(options => {
    options.Server.Port = 5001;
    options.Server.UseTls = false;
    options.Server.MaxConcurrentConnections = 1000;
    options.Serialization.EnableCompression = true;
});

// 配置通道
builder.Services.Configure<ChannelOptions>(options => {
    options.Capacity = 10000;
    options.FullMode = "Wait";
    options.SingleReader = false;
});

// 注册服务
builder.Services.AddSingleton<IMagiconionService, MagiconionService>();
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IStreamingProcessor, ChannelStreamingProcessor>();
```

## 配置选项

### Magiconion 配置

```json
{
  "MagicOnion": {
    "Server": {
      "Port": 5001,                 // 服务端口
      "UseTls": false,              // 是否使用 TLS
      "MaxConcurrentConnections": 1000, // 最大并发连接数
      "ReceiveTimeoutMilliseconds": 30000 // 接收超时时间
    },
    "Serialization": {
      "EnableCompression": true,    // 启用压缩
      "CompressionType": "Lz4BlockArray", // 压缩类型
      "BufferSize": 65536           // 缓冲区大小
    }
  },
  "Channel": {
    "Capacity": 10000,              // 通道容量
    "FullMode": "Wait",            // 满时模式
    "SingleReader": false,          // 单读取器
    "SingleWriter": false           // 单写入器
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理提高效率
4. **连接池**: 使用连接池管理资源
5. **内存优化**: 使用 Span 零拷贝和对象池
6. **通道配置**: 根据实际负载调整通道容量

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件中的端口设置
   - 验证网络连接和防火墙设置
   - 查看日志信息

2. **性能问题**
   - 调整通道容量和并发设置
   - 启用压缩和缓存
   - 优化数据传输大小

3. **AOT 编译问题**
   - 确保所有依赖支持 AOT 编译
   - 检查运行时标识符设置
   - 验证单文件可执行配置

## 扩展开发

### 添加自定义功能

```csharp
public class CustomMagiconionService : MagiconionService, ICustomMagiconionService
{
    public CustomMagiconionService(IStreamingProcessor processor)
        : base(processor)
    {
    }

    public async Task<CustomResult> CustomOperationAsync(CustomRequest request)
    {
        // 实现自定义逻辑
        var result = await ProcessCustomLogicAsync(request);
        return new CustomResult { Success = true, Data = result };
    }
}

// 注册自定义服务
builder.Services.AddSingleton<ICustomMagiconionService, CustomMagiconionService>();
```

### 扩展通道处理器

```csharp
public class CustomChannelProcessor : ChannelStreamingProcessor
{
    public CustomChannelProcessor(Channel<StreamingMessage> channel, ThreadLocal<Span<byte>> buffer)
        : base(channel, buffer)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override async Task ProcessAsync(StreamingMessage message)
    {
        // 自定义处理逻辑
        await base.ProcessAsync(message);
    }
}
```
