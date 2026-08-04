# 媒体处理 - 参考文档

## 概述

媒体处理是基于 .NET 10 开发的高性能媒体系统，专为 .NET 开发者设计，提供强大的音视频处理、流媒体服务、SIP 信令等功能。

## 核心组件

### 1. FFmpeg 集成服务
- **位置**: scripts/ffmpeg_integration.cs
- **功能**: 音视频编码解码、格式转换、流媒体处理
- **特性**: 
  - 支持多种音视频格式
  - 硬件加速
  - 异步处理
  - 高性能编码解码
  - 详细的日志记录

### 2. SIPSorcery 集成服务
- **位置**: scripts/sipsorcery_integration.cs
- **功能**: SIP 信令处理、VoIP 集成
- **特性**: 
  - 高性能 SIP 信令处理
  - 零拷贝优化
  - 支持多种 SIP 方法
  - 错误处理和重试

### 3. GB28181 SIP 信令服务
- **位置**: scripts/gb28181_sip_signaling.cs
- **功能**: GB28181 协议的 SIP 信令处理
- **特性**: 
  - 设备注册和心跳
  - 设备信息查询
  - 实时点播
  - 报警处理

### 4. NAT 穿透服务
- **位置**: scripts/sip_stun_integration.cs, scripts/sip_turn_integration.cs
- **功能**: STUN、TURN、COTURN、DOTURN 等 NAT 穿透方案
- **特性**: 
  - 多种 NAT 穿透方案
  - 自适应选择最优方案
  - 网络状况检测
  - 连接可靠性保证

### 5. 流媒体服务集成
- **位置**: scripts/srs_ssr_integration.cs, scripts/zlmediakit_ssr_integration.cs
- **功能**: SRS、ZLMediaKit 等流媒体服务集成
- **特性**: 
  - 支持多种流媒体协议
  - 高性能流媒体处理
  - 实时流转发
  - 录制和回放

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

### AOT 架构优势

1. **启动速度提升**：AOT 编译消除了 JIT 编译开销，显著提高应用启动速度
2. **内存使用减少**：减少了运行时元数据和 JIT 编译缓存的内存占用
3. **部署简化**：单文件发布减少了部署复杂性
4. **安全性增强**：减少了运行时反射攻击面
5. **性能稳定**：编译时优化确保运行时性能稳定

### 性能优化技术

1. **Threading.Channels**: 高效的异步事件队列处理，支持背压控制
2. **ObjectPool**: 减少对象创建开销，优化内存使用
3. **Span 零拷贝**: 减少内存分配和复制
4. **TailLatencyOptimizer**: 尾延迟优化
5. **AggressiveOptimization**: 编译器级优化
6. **Cache-line 对齐**: 内存分配优化
7. **SIMD 指令**: 利用 SIMD 指令加速媒体处理
8. **硬件加速**: 利用 GPU 等硬件加速媒体处理

## 使用示例

### 基本用法

```csharp
// 获取 FFmpeg 服务
var ffmpegService = serviceProvider.GetRequiredService<IFFMpegService>();

// 转换视频格式
var result = await ffmpegService.ConvertVideoAsync(
    inputPath: "input.mp4",
    outputPath: "output.avi",
    options: new ConversionOptions
    {
        VideoCodec = "mpeg4",
        AudioCodec = "mp3",
        Quality = VideoQuality.Medium
    });

Console.WriteLine($"视频转换完成，输出文件: {result.OutputPath}");
```

### 高级配置

```csharp
// 配置高性能媒体处理
var mediaOptions = new MediaProcessingOptions
{
    EnableHardwareAcceleration = true,
    UseAsyncProcessing = true,
    BufferSize = 8192,
    MaxConcurrentProcesses = Environment.ProcessorCount,
    EnableDetailedLogging = true
};

builder.Services.Configure<MediaProcessingOptions>(options => {
    options.EnableHardwareAcceleration = mediaOptions.EnableHardwareAcceleration;
    options.UseAsyncProcessing = mediaOptions.UseAsyncProcessing;
    options.BufferSize = mediaOptions.BufferSize;
    options.MaxConcurrentProcesses = mediaOptions.MaxConcurrentProcesses;
    options.EnableDetailedLogging = mediaOptions.EnableDetailedLogging;
});

// 注册媒体服务
builder.Services.AddSingleton<IFFMpegService, FFMpegService>();
builder.Services.AddSingleton<ISIPService, SIPService>();
builder.Services.AddSingleton<IMediaService, MediaService>();
```

### SIP 信令示例

```csharp
// 初始化 SIP 服务
var sipService = serviceProvider.GetRequiredService<ISIPService>();
await sipService.InitializeAsync();

// 注册 SIP 账号
await sipService.RegisterAsync(
    username: "user",
    password: "password",
    domain: "sip.domain.com",
    port: 5060);

// 发起 SIP 呼叫
var callId = await sipService.MakeCallAsync(
    destination: "sip:destination@sip.domain.com",
    options: new CallOptions
    {
        VideoEnabled = true,
        AudioEnabled = true,
        Codec = "H264"
    });

Console.WriteLine($"SIP 呼叫已发起，呼叫ID: {callId}");

// 挂断呼叫
await sipService.HangupCallAsync(callId);
```

## 配置选项

### 媒体处理配置

```json
{
  "MediaProcessing": {
    "EnableHardwareAcceleration": true,  // 启用硬件加速
    "UseAsyncProcessing": true,          // 使用异步处理
    "BufferSize": 8192,                  // 缓冲区大小
    "MaxConcurrentProcesses": 4,         // 最大并发进程数
    "EnableDetailedLogging": false,       // 启用详细日志
    "TempDirectory": "temp",            // 临时目录
    "EnableCompression": true            // 启用压缩
  }
}
```

### SIP 服务配置

```json
{
  "SIP": {
    "Server": {
      "Port": 5060,                       // SIP 服务端口
      "Transport": "UDP",                // 传输协议
      "EnableRegistration": true,         // 启用注册
      "MaxConnections": 1000              // 最大连接数
    },
    "Client": {
      "Timeout": "00:00:30",             // 超时时间
      "RetryCount": 3,                    // 重试次数
      "EnableKeepAlive": true,            // 启用心跳
      "KeepAliveInterval": "00:01:00"    // 心跳间隔
    }
  }
}
```

### 流媒体配置

```json
{
  "Streaming": {
    "EnableRTSP": true,                   // 启用 RTSP
    "EnableRTMP": true,                   // 启用 RTMP
    "EnableHTTP": true,                   // 启用 HTTP
    "EnableWebSocket": true,              // 启用 WebSocket
    "MaxConnections": 1000,               // 最大连接数
    "BufferSize": 4096,                   // 缓冲区大小
    "EnableRecording": false              // 启用录制
  }
}
```

## 性能优化

1. **硬件加速**：启用硬件加速提高媒体处理性能
2. **异步编程**：使用异步 API 避免线程阻塞
3. **批处理**：使用批处理减少网络往返开销
4. **连接池**：使用连接池管理资源
5. **缓存**：合理使用缓存提高性能
6. **并行处理**：合理配置并发度提高处理能力
7. **内存优化**：使用对象池和 Span 减少内存分配
8. **网络优化**：优化网络设置，减少延迟

## 故障排除

### 常见问题

1. **FFmpeg 执行失败**
   - 检查 FFmpeg 安装是否正确
   - 验证输入文件格式是否支持
   - 检查输出目录权限
   - 查看详细日志信息

2. **SIP 注册失败**
   - 检查网络连接
   - 验证 SIP 服务器地址和端口
   - 检查用户名和密码
   - 查看 SIP 信令日志

3. **NAT 穿透失败**
   - 检查网络类型（对称 NAT、锥形 NAT 等）
   - 尝试不同的 NAT 穿透方案
   - 检查防火墙设置
   - 验证 STUN/TURN 服务器配置

4. **流媒体播放失败**
   - 检查流媒体服务状态
   - 验证流地址是否正确
   - 检查网络带宽
   - 查看流媒体服务日志

5. **性能问题**
   - 启用硬件加速
   - 调整缓冲区大小
   - 增加并发处理能力
   - 优化媒体处理参数

6. **AOT 编译问题**
   - 确保所有依赖支持 AOT 编译
   - 检查运行时标识符设置
   - 验证单文件可执行配置
   - 查看 AOT 编译警告和错误

## 扩展开发

### 添加自定义媒体处理器

```csharp
public class CustomMediaProcessor : IMediaProcessor
{
    private readonly ILogger<CustomMediaProcessor> _logger;
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public CustomMediaProcessor(
        ILogger<CustomMediaProcessor> logger,
        ObjectPool<Memory<byte>> memoryPool)
    {
        _logger = logger;
        _memoryPool = memoryPool;
        _buffer = new ThreadLocal<Span<byte>>(() => stackalloc byte[4096]);
    }
    
    public async Task<ProcessResult> ProcessAsync(ProcessInput input)
    {
        _logger.LogInformation("Processing media with custom processor");
        
        var memory = _memoryPool.Get();
        try
        {
            // 自定义处理逻辑
            var result = await ProcessMediaAsync(input, memory.Span);
            return result;
        }
        finally
        {
            _memoryPool.Return(memory);
        }
    }
    
    private async Task<ProcessResult> ProcessMediaAsync(ProcessInput input, Span<byte> buffer)
    {
        // 具体处理逻辑
        return new ProcessResult { Success = true };
    }
}

// 注册自定义媒体处理器
builder.Services.AddSingleton<IMediaProcessor, CustomMediaProcessor>();
```

### 扩展 SIP 服务

```csharp
public class CustomSIPService : SIPService
{
    public CustomSIPService(
        ILogger<SIPService> logger,
        IOptions<SIPOptions> options)
        : base(logger, options)
    {
    }
    
    public async Task<string> CustomSIPMethodAsync(string parameter)
    {
        // 自定义 SIP 方法实现
        return "Custom SIP method result";
    }
}

// 注册自定义 SIP 服务
builder.Services.AddSingleton<ISIPService, CustomSIPService>();
```

### 集成新的流媒体服务

```csharp
public class CustomStreamingService : IStreamingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomStreamingService> _logger;
    
    public CustomStreamingService(
        HttpClient httpClient,
        ILogger<CustomStreamingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<string> StartStreamAsync(StreamOptions options)
    {
        // 启动自定义流媒体服务
        return "stream-url";
    }
    
    public async Task StopStreamAsync(string streamId)
    {
        // 停止自定义流媒体服务
    }
}

// 注册自定义流媒体服务
builder.Services.AddSingleton<IStreamingService, CustomStreamingService>();
builder.Services.AddHttpClient();
```

## 监控和运维

### 健康检查

```csharp
// 添加健康检查
builder.Services.AddHealthChecks()
    .AddCheck<MediaHealthCheck>("media")
    .AddCheck<SIPHealthCheck>("sip")
    .AddCheck<StreamingHealthCheck>("streaming");

// 配置健康检查端点
app.MapHealthChecks("/health");
```

### 指标监控

```csharp
// 添加指标监控
builder.Services.AddMetrics();
builder.Services.AddMetricsEndpoint();

// 配置指标端点
app.MapMetrics("/metrics");

// 记录媒体处理指标
var mediaCounter = Metrics.CreateCounter(
    "media_processing_total",
    "Total number of media processing operations");

// 使用指标
mediaCounter.Inc();
```

### 日志管理

```csharp
// 配置日志
builder.Logging.ClearProviders()
    .AddConsole()
    .AddDebug()
    .AddFile("logs/media-.txt", rollingInterval: RollingInterval.Day);

// 记录详细日志
logger.LogInformation("Media processing started: {InputFile}", inputFile);
logger.LogError("Media processing failed: {ErrorMessage}", errorMessage);
```

## 部署和扩展

### 容器化部署

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish -r linux-x64 --self-contained true -p:PublishAot=true -p:IncludeNativeLibrariesForSelfExtract=true

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN apt-get update && apt-get install -y ffmpeg
ENTRYPOINT ["./media-service"]
```

### 扩展架构

1. **媒体处理器扩展**: 通过实现 IMediaProcessor 接口扩展媒体处理能力
2. **SIP 服务扩展**: 扩展 SIP 服务添加新的 SIP 方法
3. **流媒体服务扩展**: 添加新的流媒体服务集成
4. **NAT 穿透扩展**: 添加新的 NAT 穿透方案
5. **监控扩展**: 集成第三方监控系统

### 最佳实践

1. **依赖注入**: 使用依赖注入管理服务生命周期
2. **异步编程**: 优先使用异步 API 避免线程阻塞
3. **错误处理**: 正确处理异常情况，实现重试机制
4. **日志记录**: 添加适当的日志记录，便于问题排查
5. **性能监控**: 监控关键性能指标，如处理时间、内存使用等
6. **资源管理**: 正确管理媒体资源，避免资源泄漏
7. **配置管理**: 使用配置系统管理服务配置
8. **安全性**: 确保媒体传输和存储的安全性
9. **AOT 编译**: 使用 AOT 编译提升性能
10. **容器化部署**: 使用容器化部署提高部署一致性

## 总结

媒体处理技能为 .NET 开发者提供了强大的音视频处理、流媒体服务、SIP 信令等功能，支持多种协议和服务集成。通过 AOT 编译优化，系统启动速度和运行性能得到显著提升，内存使用减少，部署更加简化。

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。开发者可以根据需要进行扩展，添加新的功能和集成新的服务。

主要技术优势包括：
- 高性能设计和优化
- 多种媒体处理能力
- 丰富的协议支持
- 强大的扩展性
- AOT 编译优化
- 跨平台支持

媒体处理技能为构建现代化、高性能的媒体应用提供了完整的解决方案。
