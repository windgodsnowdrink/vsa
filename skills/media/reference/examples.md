# 媒体处理 - 使用示例

## 快速开始

### 1. FFmpeg 视频处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xabe.FFmpeg;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var ffmpegService = serviceProvider.GetRequiredService<IFFMpegService>();
        
        Console.WriteLine("FFmpeg 视频处理示例");
        Console.WriteLine("=" * 50);
        
        // 转换视频格式
        Console.WriteLine("正在转换视频格式...");
        var conversionResult = await ffmpegService.ConvertVideoAsync(
            inputPath: "input.mp4",
            outputPath: "output.avi",
            options: new ConversionOptions
            {
                VideoCodec = "mpeg4",
                AudioCodec = "mp3",
                Quality = VideoQuality.Medium
            });
        
        Console.WriteLine($"视频转换完成，输出文件: {conversionResult.OutputPath}");
        Console.WriteLine($"转换时间: {conversionResult.Duration.TotalSeconds:F2} 秒");
        
        // 提取音频
        Console.WriteLine("\n正在提取音频...");
        var audioResult = await ffmpegService.ExtractAudioAsync(
            inputPath: "input.mp4",
            outputPath: "audio.mp3");
        
        Console.WriteLine($"音频提取完成，输出文件: {audioResult.OutputPath}");
        
        // 截图
        Console.WriteLine("\n正在截图...");
        var thumbnailResult = await ffmpegService.CreateThumbnailAsync(
            inputPath: "input.mp4",
            outputPath: "thumbnail.jpg",
            position: TimeSpan.FromSeconds(10));
        
        Console.WriteLine($"截图完成，输出文件: {thumbnailResult.OutputPath}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IFFMpegService, FFMpegService>();
        return builder.BuildServiceProvider();
    }
}

// FFmpeg 服务接口
public interface IFFMpegService
{
    Task<ConversionResult> ConvertVideoAsync(string inputPath, string outputPath, ConversionOptions options);
    Task<AudioExtractionResult> ExtractAudioAsync(string inputPath, string outputPath);
    Task<ThumbnailResult> CreateThumbnailAsync(string inputPath, string outputPath, TimeSpan position);
}

// 实现类
public class FFMpegService : IFFMpegService
{
    public async Task<ConversionResult> ConvertVideoAsync(string inputPath, string outputPath, ConversionOptions options)
    {
        // 实现视频转换逻辑
        return new ConversionResult { OutputPath = outputPath, Duration = TimeSpan.FromSeconds(5) };
    }
    
    public async Task<AudioExtractionResult> ExtractAudioAsync(string inputPath, string outputPath)
    {
        // 实现音频提取逻辑
        return new AudioExtractionResult { OutputPath = outputPath };
    }
    
    public async Task<ThumbnailResult> CreateThumbnailAsync(string inputPath, string outputPath, TimeSpan position)
    {
        // 实现截图逻辑
        return new ThumbnailResult { OutputPath = outputPath };
    }
}

// 结果类
public class ConversionResult { public string OutputPath { get; set; } public TimeSpan Duration { get; set; } }
public class AudioExtractionResult { public string OutputPath { get; set; } }
public class ThumbnailResult { public string OutputPath { get; set; } }
public class ConversionOptions { public string VideoCodec { get; set; } public string AudioCodec { get; set; } public VideoQuality Quality { get; set; } }
public enum VideoQuality { Low, Medium, High }
```

### 2. SIP 信令处理示例

```csharp
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SIPSorcery.SIP;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("SIP 信令处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var sipService = serviceProvider.GetRequiredService<ISIPService>();
        
        // 初始化 SIP 服务
        Console.WriteLine("正在初始化 SIP 服务...");
        await sipService.InitializeAsync();
        Console.WriteLine("SIP 服务初始化成功！");
        
        // 注册 SIP 账号
        Console.WriteLine("\n正在注册 SIP 账号...");
        var registerResult = await sipService.RegisterAsync(
            username: "testuser",
            password: "password",
            domain: "sip.example.com",
            port: 5060);
        
        if (registerResult.Success)
        {
            Console.WriteLine("SIP 账号注册成功！");
        }
        else
        {
            Console.WriteLine($"SIP 账号注册失败: {registerResult.ErrorMessage}");
            return;
        }
        
        // 发起 SIP 呼叫
        Console.WriteLine("\n正在发起 SIP 呼叫...");
        var callResult = await sipService.MakeCallAsync(
            destination: "sip:destination@sip.example.com",
            options: new CallOptions
            {
                VideoEnabled = true,
                AudioEnabled = true,
                Codec = "H264"
            });
        
        if (callResult.Success)
        {
            Console.WriteLine($"SIP 呼叫发起成功，呼叫ID: {callResult.CallId}");
            
            // 等待用户输入
            Console.WriteLine("\n按任意键挂断呼叫...");
            Console.ReadKey();
            
            // 挂断呼叫
            Console.WriteLine("\n正在挂断呼叫...");
            var hangupResult = await sipService.HangupCallAsync(callResult.CallId);
            
            if (hangupResult.Success)
            {
                Console.WriteLine("呼叫挂断成功！");
            }
            else
            {
                Console.WriteLine($"呼叫挂断失败: {hangupResult.ErrorMessage}");
            }
        }
        else
        {
            Console.WriteLine($"SIP 呼叫发起失败: {callResult.ErrorMessage}");
        }
        
        // 注销 SIP 账号
        Console.WriteLine("\n正在注销 SIP 账号...");
        var unregisterResult = await sipService.UnregisterAsync();
        
        if (unregisterResult.Success)
        {
            Console.WriteLine("SIP 账号注销成功！");
        }
        else
        {
            Console.WriteLine($"SIP 账号注销失败: {unregisterResult.ErrorMessage}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ISIPService, SIPService>();
        return builder.BuildServiceProvider();
    }
}

// SIP 服务接口
public interface ISIPService
{
    Task InitializeAsync();
    Task<RegisterResult> RegisterAsync(string username, string password, string domain, int port);
    Task<UnregisterResult> UnregisterAsync();
    Task<CallResult> MakeCallAsync(string destination, CallOptions options);
    Task<HangupResult> HangupCallAsync(string callId);
}

// 实现类
public class SIPService : ISIPService
{
    private SIPTransport _transport;
    private string _registrationId;
    
    public async Task InitializeAsync()
    {
        // 初始化 SIP 传输
        _transport = new SIPTransport();
        await _transport.Start();
    }
    
    public async Task<RegisterResult> RegisterAsync(string username, string password, string domain, int port)
    {
        // 实现注册逻辑
        return new RegisterResult { Success = true };
    }
    
    public async Task<UnregisterResult> UnregisterAsync()
    {
        // 实现注销逻辑
        return new UnregisterResult { Success = true };
    }
    
    public async Task<CallResult> MakeCallAsync(string destination, CallOptions options)
    {
        // 实现呼叫逻辑
        return new CallResult { Success = true, CallId = Guid.NewGuid().ToString() };
    }
    
    public async Task<HangupResult> HangupCallAsync(string callId)
    {
        // 实现挂断逻辑
        return new HangupResult { Success = true };
    }
}

// 结果类
public class RegisterResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
public class UnregisterResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
public class CallResult { public bool Success { get; set; } public string CallId { get; set; } public string ErrorMessage { get; set; } }
public class HangupResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
public class CallOptions { public bool VideoEnabled { get; set; } public bool AudioEnabled { get; set; } public string Codec { get; set; } }
```

### 3. 流媒体服务集成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("流媒体服务集成示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var streamingService = serviceProvider.GetRequiredService<IStreamingService>();
        
        // 启动流媒体服务
        Console.WriteLine("正在启动流媒体服务...");
        var startResult = await streamingService.StartServiceAsync(
            options: new StreamingServiceOptions
            {
                RtspPort = 554,
                RtmpPort = 1935,
                HttpPort = 8080,
                EnableWebSocket = true
            });
        
        if (startResult.Success)
        {
            Console.WriteLine("流媒体服务启动成功！");
            Console.WriteLine($"RTSP 地址: rtsp://localhost:554/live");
            Console.WriteLine($"RTMP 地址: rtmp://localhost:1935/live");
            Console.WriteLine($"HTTP 地址: http://localhost:8080/live");
        }
        else
        {
            Console.WriteLine($"流媒体服务启动失败: {startResult.ErrorMessage}");
            return;
        }
        
        // 推流
        Console.WriteLine("\n正在推流...");
        var pushResult = await streamingService.PushStreamAsync(
            streamId: "live",
            sourceUrl: "rtsp://camera.example.com:554/stream",
            options: new PushOptions
            {
                EnableRecording = true,
                RecordingPath = "recordings",
                EnableTranscoding = true
            });
        
        if (pushResult.Success)
        {
            Console.WriteLine("推流成功！");
        }
        else
        {
            Console.WriteLine($"推流失败: {pushResult.ErrorMessage}");
        }
        
        // 等待用户输入
        Console.WriteLine("\n按任意键停止服务...");
        Console.ReadKey();
        
        // 停止流媒体服务
        Console.WriteLine("正在停止流媒体服务...");
        var stopResult = await streamingService.StopServiceAsync();
        
        if (stopResult.Success)
        {
            Console.WriteLine("流媒体服务停止成功！");
        }
        else
        {
            Console.WriteLine($"流媒体服务停止失败: {stopResult.ErrorMessage}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IStreamingService, StreamingService>();
        return builder.BuildServiceProvider();
    }
}

// 流媒体服务接口
public interface IStreamingService
{
    Task<ServiceResult> StartServiceAsync(StreamingServiceOptions options);
    Task<ServiceResult> StopServiceAsync();
    Task<StreamResult> PushStreamAsync(string streamId, string sourceUrl, PushOptions options);
    Task<StreamResult> StopStreamAsync(string streamId);
    Task<StreamInfo> GetStreamInfoAsync(string streamId);
}

// 实现类
public class StreamingService : IStreamingService
{
    public async Task<ServiceResult> StartServiceAsync(StreamingServiceOptions options)
    {
        // 实现启动逻辑
        return new ServiceResult { Success = true };
    }
    
    public async Task<ServiceResult> StopServiceAsync()
    {
        // 实现停止逻辑
        return new ServiceResult { Success = true };
    }
    
    public async Task<StreamResult> PushStreamAsync(string streamId, string sourceUrl, PushOptions options)
    {
        // 实现推流逻辑
        return new StreamResult { Success = true };
    }
    
    public async Task<StreamResult> StopStreamAsync(string streamId)
    {
        // 实现停止推流逻辑
        return new StreamResult { Success = true };
    }
    
    public async Task<StreamInfo> GetStreamInfoAsync(string streamId)
    {
        // 实现获取流信息逻辑
        return new StreamInfo { StreamId = streamId, Viewers = 0 };
    }
}

// 结果类
public class ServiceResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
public class StreamResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
public class StreamInfo { public string StreamId { get; set; } public int Viewers { get; set; } public string Status { get; set; } }
public class StreamingServiceOptions { public int RtspPort { get; set; } public int RtmpPort { get; set; } public int HttpPort { get; set; } public bool EnableWebSocket { get; set; } }
public class PushOptions { public bool EnableRecording { get; set; } public string RecordingPath { get; set; } public bool EnableTranscoding { get; set; } }
```

### 4. NAT 穿透示例

```csharp
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NAT 穿透示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var natService = serviceProvider.GetRequiredService<INATService>();
        
        // 检测网络类型
        Console.WriteLine("正在检测网络类型...");
        var networkInfo = await natService.DetectNetworkTypeAsync();
        Console.WriteLine($"网络类型: {networkInfo.NetworkType}");
        Console.WriteLine($"公网 IP: {networkInfo.PublicIP}");
        Console.WriteLine($"内网 IP: {networkInfo.LocalIP}");
        
        // 测试 STUN
        Console.WriteLine("\n正在测试 STUN...");
        var stunResult = await natService.TestSTUNAsync(
            server: "stun.l.google.com",
            port: 19302);
        
        if (stunResult.Success)
        {
            Console.WriteLine("STUN 测试成功！");
            Console.WriteLine($"映射地址: {stunResult.MappedEndPoint}");
        }
        else
        {
            Console.WriteLine($"STUN 测试失败: {stunResult.ErrorMessage}");
        }
        
        // 测试 TURN
        Console.WriteLine("\n正在测试 TURN...");
        var turnResult = await natService.TestTURNAsync(
            server: "turn.example.com",
            port: 3478,
            username: "testuser",
            password: "password");
        
        if (turnResult.Success)
        {
            Console.WriteLine("TURN 测试成功！");
            Console.WriteLine($"分配地址: {turnResult.AllocatedEndPoint}");
        }
        else
        {
            Console.WriteLine($"TURN 测试失败: {turnResult.ErrorMessage}");
        }
        
        // 选择最优 NAT 穿透方案
        Console.WriteLine("\n正在选择最优 NAT 穿透方案...");
        var bestSolution = await natService.SelectBestSolutionAsync();
        Console.WriteLine($"最优方案: {bestSolution.SolutionType}");
        Console.WriteLine($"预计成功率: {bestSolution.SuccessRate:P2}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<INATService, NATService>();
        return builder.BuildServiceProvider();
    }
}

// NAT 服务接口
public interface INATService
{
    Task<NetworkInfo> DetectNetworkTypeAsync();
    Task<STUNResult> TestSTUNAsync(string server, int port);
    Task<TURNResult> TestTURNAsync(string server, int port, string username, string password);
    Task<NATSolution> SelectBestSolutionAsync();
    Task<NATResult> EstablishConnectionAsync(string remoteAddress, int remotePort);
}

// 实现类
public class NATService : INATService
{
    public async Task<NetworkInfo> DetectNetworkTypeAsync()
    {
        // 实现网络类型检测逻辑
        return new NetworkInfo
        {
            NetworkType = "锥形 NAT",
            PublicIP = IPAddress.Parse("203.0.113.1"),
            LocalIP = IPAddress.Parse("192.168.1.100")
        };
    }
    
    public async Task<STUNResult> TestSTUNAsync(string server, int port)
    {
        // 实现 STUN 测试逻辑
        return new STUNResult
        {
            Success = true,
            MappedEndPoint = new IPEndPoint(IPAddress.Parse("203.0.113.1"), 50000)
        };
    }
    
    public async Task<TURNResult> TestTURNAsync(string server, int port, string username, string password)
    {
        // 实现 TURN 测试逻辑
        return new TURNResult
        {
            Success = true,
            AllocatedEndPoint = new IPEndPoint(IPAddress.Parse("198.51.100.1"), 50001)
        };
    }
    
    public async Task<NATSolution> SelectBestSolutionAsync()
    {
        // 实现选择最优方案逻辑
        return new NATSolution
        {
            SolutionType = "STUN",
            SuccessRate = 0.95
        };
    }
    
    public async Task<NATResult> EstablishConnectionAsync(string remoteAddress, int remotePort)
    {
        // 实现建立连接逻辑
        return new NATResult { Success = true };
    }
}

// 结果类
public class NetworkInfo { public string NetworkType { get; set; } public IPAddress PublicIP { get; set; } public IPAddress LocalIP { get; set; } }
public class STUNResult { public bool Success { get; set; } public IPEndPoint MappedEndPoint { get; set; } public string ErrorMessage { get; set; } }
public class TURNResult { public bool Success { get; set; } public IPEndPoint AllocatedEndPoint { get; set; } public string ErrorMessage { get; set; } }
public class NATSolution { public string SolutionType { get; set; } public double SuccessRate { get; set; } }
public class NATResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
```

### 5. AOT 编译示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package Xabe.FFmpeg@6.0.2 
#:package SIPSorcery@6.0.0 
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

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

public class AotExample
{
    public static async Task Main()
    {
        Console.WriteLine("媒体处理 AOT 编译示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("使用 AOT 编译的单文件可执行程序");
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var mediaService = serviceProvider.GetRequiredService<IMediaService>();
        
        // 测试启动时间
        var stopwatch = Stopwatch.StartNew();
        
        // 初始化媒体服务
        await mediaService.InitializeAsync();
        
        stopwatch.Stop();
        Console.WriteLine($"\n服务初始化时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 测试视频处理性能
        stopwatch.Restart();
        
        var processResult = await mediaService.ProcessVideoAsync(
            inputPath: "input.mp4",
            outputPath: "output.mp4",
            options: new VideoProcessingOptions
            {
                Width = 1280,
                Height = 720,
                Bitrate = 3000,
                FrameRate = 30
            });
        
        stopwatch.Stop();
        
        if (processResult.Success)
        {
            Console.WriteLine($"\n视频处理成功！");
            Console.WriteLine($"处理时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"输出文件: {processResult.OutputPath}");
        }
        else
        {
            Console.WriteLine($"\n视频处理失败: {processResult.ErrorMessage}");
        }
        
        // 测试 SIP 服务
        var sipService = serviceProvider.GetRequiredService<ISIPService>();
        var sipInitResult = await sipService.InitializeAsync();
        
        if (sipInitResult.Success)
        {
            Console.WriteLine("\nSIP 服务初始化成功！");
        }
        else
        {
            Console.WriteLine($"\nSIP 服务初始化失败: {sipInitResult.ErrorMessage}");
        }
        
        Console.WriteLine("\nAOT 编译示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IMediaService, MediaService>();
        builder.AddSingleton<ISIPService, SIPService>();
        return builder.BuildServiceProvider();
    }
}

// 媒体服务接口
public interface IMediaService
{
    Task InitializeAsync();
    Task<VideoProcessResult> ProcessVideoAsync(string inputPath, string outputPath, VideoProcessingOptions options);
}

// SIP 服务接口
public interface ISIPService
{
    Task<InitializeResult> InitializeAsync();
}

// 实现类
public class MediaService : IMediaService
{
    public async Task InitializeAsync()
    {
        // 实现初始化逻辑
    }
    
    public async Task<VideoProcessResult> ProcessVideoAsync(string inputPath, string outputPath, VideoProcessingOptions options)
    {
        // 实现视频处理逻辑
        return new VideoProcessResult { Success = true, OutputPath = outputPath };
    }
}

public class SIPService : ISIPService
{
    public async Task<InitializeResult> InitializeAsync()
    {
        // 实现初始化逻辑
        return new InitializeResult { Success = true };
    }
}

// 结果类
public class VideoProcessResult { public bool Success { get; set; } public string OutputPath { get; set; } public string ErrorMessage { get; set; } }
public class InitializeResult { public bool Success { get; set; } public string ErrorMessage { get; set; } }
public class VideoProcessingOptions { public int Width { get; set; } public int Height { get; set; } public int Bitrate { get; set; } public int FrameRate { get; set; } }
```

## 总结

以上示例展示了媒体处理技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手 FFmpeg 视频处理功能
2. 实现 SIP 信令处理和 VoIP 功能
3. 集成流媒体服务
4. 实现 NAT 穿透
5. 使用 AOT 编译提升性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 支持的功能

- **音视频编码解码**: 支持多种音视频格式的编码和解码
- **流媒体服务**: 支持 RTSP、RTMP、HTTP 等多种流媒体协议
- **SIP 信令**: 支持 SIP 注册、呼叫、挂断等信令
- **NAT 穿透**: 支持 STUN、TURN、COTURN、DOTURN 等 NAT 穿透方案
- **GB28181 集成**: 支持 GB28181 协议的设备接入和管理
- **硬件加速**: 支持 GPU 等硬件加速
- **AOT 编译**: 支持 AOT 编译，提升启动速度和运行性能

### 性能优化特点

- **异步编程**: 全异步 API，避免线程阻塞
- **内存优化**: 使用对象池和 Span 减少内存分配
- **并行处理**: 合理配置并发度提高处理能力
- **批处理**: 支持批量处理提高效率
- **缓存使用**: 合理使用缓存提高性能
- **网络优化**: 优化网络设置，减少延迟
- **SIMD 指令**: 利用 SIMD 指令加速媒体处理
- **硬件加速**: 利用 GPU 等硬件加速媒体处理

### 应用场景

- **视频会议系统**: 支持多人视频会议的音视频处理
- **直播平台**: 支持直播流的处理和分发
- **监控系统**: 支持监控视频的处理和存储
- **VoIP 系统**: 支持语音通话和视频通话
- **音视频编辑**: 支持音视频的编辑和处理
- **流媒体服务器**: 支持流媒体的接收和分发
- **GB28181 系统**: 支持 GB28181 协议的设备接入和管理

媒体处理技能为构建现代化、高性能的媒体应用提供了完整的解决方案。
