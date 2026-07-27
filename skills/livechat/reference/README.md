# livechat - 参考文档

## 概述

livechat 是一个基于 .NET 10 的高性能实时聊天系统，专为 .NET 开发者设计。

## 核心组件

### 1. LiveStreamProcessor（实时流处理器）
- **位置**: scripts/livechat_integration.cs
- **功能**: 处理实时视频和音频流
- **特性**: 
  - 高性能视频流处理
  - 低延迟音频流处理
  - Threading.Channels 事件处理
  - Span 零拷贝优化

### 2. LiveChatService（实时聊天服务）
- **位置**: scripts/livechat_integration.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 实时消息传递
  - 会话管理
  - 错误处理
  - 日志记录

## 使用示例

### 基本使用

`csharp
var liveChatService = serviceProvider.GetRequiredService<ILiveChatService>();
var session = await liveChatService.CreateSessionAsync();
await liveChatService.SendMessageAsync(session.SessionId, "Hello, world!");
`

### 高级配置

`csharp
var settings = new LiveChatSettings {
    EnableVideoProcessing = true,
    EnableAudioProcessing = true,
    MaxConcurrentSessions = 100,
    MessageTimeout = TimeSpan.FromSeconds(30)
};

builder.Services.Configure<LiveChatSettings>(options => {
    options.EnableVideoProcessing = settings.EnableVideoProcessing;
    options.EnableAudioProcessing = settings.EnableAudioProcessing;
    options.MaxConcurrentSessions = settings.MaxConcurrentSessions;
    options.MessageTimeout = settings.MessageTimeout;
});
`

## 配置选项

### LiveChat 配置

`json
{
  "LiveChatSettings": {
    "EnableVideoProcessing": true,          // 启用视频处理
    "EnableAudioProcessing": true,          // 启用音频处理
    "MaxConcurrentSessions": 100,           // 最大并发会话数
    "MessageTimeout": "00:00:30",        // 消息超时
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
#:package LiveChatSDK@11.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.AspNetCore.SignalR@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
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
dotnet publish scripts/livechat_integration.cs -c Release -r win-x64 --aot

# Linux 发布命令
dotnet publish scripts/livechat_integration.cs -c Release -r linux-x64 --aot

# macOS 发布命令
dotnet publish scripts/livechat_integration.cs -c Release -r osx-x64 --aot
```

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件
   - 验证网络连接
   - 检查日志信息

2. **性能问题**
   - 启用缓存
   - 优化查询条件
   - 增加资源限制
   - 检查 Channel 配置

3. **内存问题**
   - 调整对象池大小
   - 优化内存使用
   - 检查 Span 使用

## 扩展开发

### 添加自定义功能

`csharp
public class CustomLiveChatService : ILiveChatService
{
    public async Task<LiveChatSession> CreateSessionAsync()
    {
        // 实现自定义逻辑
        return new LiveChatSession { SessionId = Guid.NewGuid().ToString() };
    }
    
    public async Task SendMessageAsync(string sessionId, string message)
    {
        // 实现自定义逻辑
    }
}
`

### 扩展视频处理器

`csharp
public class CustomLiveStreamProcessor : ILiveStreamProcessor
{
    public async Task ProcessVideoAsync(ReadOnlyMemory<byte> frameData, int width, int height)
    {
        // 实现自定义视频处理逻辑
    }
    
    public async Task ProcessAudioAsync(ReadOnlyMemory<byte> audioData, int sampleRate)
    {
        // 实现自定义音频处理逻辑
    }
}
`

