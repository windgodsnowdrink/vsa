# 媒体处理智能体技能 - 媒体技能

## 技能概述

基于 .NET 10 的高性能媒体处理技能，为 .NET 开发者提供强大的音视频处理、流媒体、SIP 信令等功能。

## 快速开始指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SIPSorcery@6.0.0
#:package FFMpegCore@5.0.0
```

### 注册服务

在主应用中注册媒体服务：

```csharp
// 注册媒体服务
builder.Services.AddSingleton<ISIPService, SIPService>();
builder.Services.AddSingleton<IMediaService, MediaService>();
builder.Services.AddSingleton<IFFMpegService, FFMpegService>();
```

### 使用示例

```csharp
// 获取 SIP 服务
var sipService = serviceProvider.GetRequiredService<ISIPService>();

// 初始化 SIP 服务
await sipService.InitializeAsync();

// 注册 SIP 账号
await sipService.RegisterAsync("sip:user@domain.com", "password");

// 发起 SIP 呼叫
var callId = await sipService.MakeCallAsync("sip:destination@domain.com");

Console.WriteLine($"SIP 呼叫已发起，呼叫ID: {callId}");
```

## AOT 架构执行

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

### AOT 架构优势

1. **启动速度提升**：AOT 编译消除了 JIT 编译开销，显著提高应用启动速度
2. **内存使用减少**：减少了运行时元数据和 JIT 编译缓存的内存占用
3. **部署简化**：单文件发布减少了部署复杂性
4. **安全性增强**：减少了运行时反射攻击面
5. **性能稳定**：编译时优化确保运行时性能稳定

### 高性能媒体处理

媒体处理技能使用多种高性能技术，提供不同场景下的最佳解决方案：

```csharp
// 配置高性能媒体处理
var mediaOptions = new MediaProcessingOptions
{
    EnableHardwareAcceleration = true,
    UseAsyncProcessing = true,
    BufferSize = 8192,
    MaxConcurrentProcesses = Environment.ProcessorCount
};

// 初始化媒体服务
var mediaService = new MediaService(mediaOptions);

// 处理视频文件
var result = await mediaService.ProcessVideoAsync(
    inputPath: "input.mp4",
    outputPath: "output.mp4",
    options: new VideoProcessingOptions
    {
        Width = 1920,
        Height = 1080,
        Bitrate = 5000,
        FrameRate = 30
    });

Console.WriteLine($"视频处理完成，输出文件: {result.OutputPath}");
```

## 导航地图

```
media/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── ffmpeg_integration.cs     # FFmpeg 集成
    ├── gb28181_sip_signaling.cs  # GB28181 SIP 信令
    ├── sipsorcery_integration.cs  # SIPSorcery 集成
    ├── sip_stun_integration.cs    # SIP STUN 集成
    ├── sip_turn_integration.cs    # SIP TURN 集成
    ├── srs_ssr_integration.cs     # SRS SSR 集成
    ├── voip_integration.cs        # VoIP 集成
    └── 其他集成脚本...
```

## 主要功能

1. **音视频编码解码**：支持多种音视频格式的编码和解码
2. **流媒体服务集成**：支持 SRS、ZLMediaKit 等流媒体服务
3. **SIP 信令处理**：支持 SIP 注册、呼叫、挂断等信令
4. **NAT 穿透**：支持 STUN、TURN、COTURN、DOTURN 等 NAT 穿透方案
5. **媒体加密传输**：支持 SRTP、DTLS 等加密传输
6. **GB28181 集成**：支持 GB28181 协议的 SIP 信令
7. **FFmpeg 集成**：支持 FFmpeg 的音视频处理功能
8. **VoIP 集成**：支持 VoIP 相关功能

## 扩展说明

本技能提供了完整的媒体处理解决方案，您可以根据需要进行扩展：

1. **自定义媒体处理器**：实现 IMediaProcessor 接口
2. **扩展流媒体支持**：添加新的流媒体服务集成
3. **系统集成**：与其他系统集成，如数据库、缓存、外部 API 等
4. **性能优化**：针对特定场景优化媒体处理性能
5. **新功能添加**：添加新的媒体处理功能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期
2. **异步编程**：优先使用异步 API 避免线程阻塞
3. **错误处理**：正确处理异常情况，实现重试机制
4. **日志记录**：添加适当的日志记录，便于问题排查
5. **性能监控**：监控关键性能指标，如处理时间、内存使用等
6. **资源管理**：正确管理媒体资源，避免资源泄漏
7. **配置管理**：使用配置系统管理服务配置
8. **安全性**：确保媒体传输和存储的安全性

## 应用场景

1. **视频会议系统**：支持多人视频会议的音视频处理
2. **直播平台**：支持直播流的处理和分发
3. **监控系统**：支持监控视频的处理和存储
4. **VoIP 系统**：支持语音通话和视频通话
5. **音视频编辑**：支持音视频的编辑和处理
6. **流媒体服务器**：支持流媒体的接收和分发
7. **GB28181 系统**：支持 GB28181 协议的设备接入和管理

## 技术优势

1. **高性能设计**：优化的媒体处理管道，支持高并发场景
2. **可靠性保证**：内置错误处理和重试机制
3. **灵活性**：支持多种媒体格式和协议
4. **可扩展性**：模块化设计，易于扩展和定制
5. **开发效率**：简化媒体处理代码，提高开发效率
6. **AOT 优化**：支持 .NET 10 AOT 编译，提升性能
7. **跨平台支持**：支持 Windows、Linux、macOS 等多种平台
