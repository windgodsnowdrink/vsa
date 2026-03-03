# audio Agent Skill - audio 技能

## 技能概述

基于 .NET 10 的高性能音频处理技能，为 .NET 开发者提供强大的音频功能，包括音频录制、播放、编辑、转换、语音识别（ASR）、文本到语音（TTS）、音频增强等核心功能，支持 AOT 编译优化。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package NAudio@2.2.1
#:package Accord.Audio@3.8.0
#:package Microsoft.CognitiveServices.Speech@1.37.0
```

### 注册服务

在您的主应用程序中注册音频服务：

```csharp
// 注册音频服务
var builder = WebApplication.CreateBuilder();

// 配置 NAudio 服务
builder.Services.AddSingleton<IAudioRecorder, NaudioAudioRecorder>();
builder.Services.AddSingleton<IAudioPlayer, NaudioAudioPlayer>();
builder.Services.AddSingleton<IAudioConverter, NaudioAudioConverter>();

// 配置语音服务
builder.Services.AddSpeechServices(options => {
    options.SubscriptionKey = "your-subscription-key";
    options.Region = "your-region";
});

// 配置音频增强服务
builder.Services.AddAudioEnhancement(options => {
    options.EnableNoiseReduction = true;
    options.EnableEchoCancellation = true;
    options.EnableBassBoost = false;
});

var app = builder.Build();
app.Run();
```

### 使用示例

```csharp
// 获取音频服务
var audioRecorder = serviceProvider.GetRequiredService<IAudioRecorder>();
var audioPlayer = serviceProvider.GetRequiredService<IAudioPlayer>();
var speechService = serviceProvider.GetRequiredService<ISpeechService>();

// 录制音频
Console.WriteLine("开始录制音频...");
var audioData = await audioRecorder.RecordAsync(TimeSpan.FromSeconds(5));
Console.WriteLine($"录制完成，音频长度: {audioData.Duration}");

// 播放音频
Console.WriteLine("播放录制的音频...");
await audioPlayer.PlayAsync(audioData);

// 语音识别
Console.WriteLine("进行语音识别...");
var recognizedText = await speechService.RecognizeSpeechAsync(audioData);
Console.WriteLine($"识别结果: {recognizedText}");

// 文本到语音
Console.WriteLine("进行文本到语音转换...");
var speechAudio = await speechService.SynthesizeSpeechAsync("这是一个文本到语音的示例");
Console.WriteLine("播放合成语音...");
await audioPlayer.PlayAsync(speechAudio);
```

## 导航地图

```
audio/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
│   ├── accord_audio_integration.cs      # Accord.Audio 集成实现
│   ├── accord_audio_integration.run.json  # Accord.Audio 运行配置
│   ├── accord_audio_integration.setting.json  # Accord.Audio 设置文件
│   ├── asr_integration.cs      # 自动语音识别集成
│   ├── asr_integration.run.json  # ASR 运行配置
│   ├── asr_integration.setting.json  # ASR 设置文件
│   ├── audio_enhancement.cs      # 音频增强实现
│   ├── audio_enhancement.run.json  # 音频增强运行配置
│   ├── audio_enhancement.setting.json  # 音频增强设置文件
│   ├── csvideo_audio.cs      # 音视频处理集成
│   ├── csvideo_audio.run.json  # 音视频处理运行配置
│   ├── csvideo_audio.setting.json  # 音视频处理设置文件
│   ├── naudio_integration.cs      # NAudio 集成实现
│   ├── naudio_integration.run.json  # NAudio 运行配置
│   ├── naudio_integration.setting.json  # NAudio 设置文件
│   ├── rtc_audio_processor.cs      # RTC 音频处理器
│   ├── rtc_audio_processor.run.json  # RTC 音频处理器运行配置
│   ├── rtc_audio_processor.setting.json  # RTC 音频处理器设置文件
│   ├── speech_integration.cs      # 语音服务集成
│   ├── speech_integration.run.json  # 语音服务运行配置
│   ├── speech_integration.setting.json  # 语音服务设置文件
│   ├── tts_integration.cs      # 文本到语音集成
│   ├── tts_integration.run.json  # TTS 运行配置
│   └── tts_integration.setting.json  # TTS 设置文件
└── rtc_audio_processor_signalr.js  # RTC 音频处理器 SignalR 客户端
```

## 主要功能

1. **音频录制与播放**: 支持多种音频格式的录制和播放
2. **音频编辑与转换**: 支持音频裁剪、合并、格式转换等编辑功能
3. **语音识别 (ASR)**: 集成多种语音识别引擎，支持实时和离线识别
4. **文本到语音 (TTS)**: 支持多种语音合成引擎，提供自然流畅的语音输出
5. **音频增强**: 支持噪声 reduction、回声消除、音频增强等功能
6. **实时通信 (RTC) 音频处理**: 为实时通信应用提供音频处理支持
7. **音视频同步处理**: 支持音视频同步录制和处理
8. **高性能设计**: 优化的性能实现，支持高并发音频处理
9. **AOT 编译优化**: 支持将音频应用编译为本机代码，提高启动速度和运行性能
10. **易于使用的 API**: 简洁直观的 API 设计，降低开发复杂度
11. **可扩展架构**: 支持自定义扩展和插件开发

## 扩展说明

此技能提供完整的音频解决方案，您可以根据需要进行扩展：

1. **自定义音频处理器**: 实现自定义的音频处理逻辑
2. **集成新的语音引擎**: 集成其他语音识别和合成引擎
3. **扩展音频格式支持**: 添加对新音频格式的支持
4. **性能优化**: 针对特定场景优化音频处理性能
5. **与其他系统集成**: 与其他系统如流媒体服务器、AI 平台等集成

## 最佳实践

1. **使用依赖注入**: 使用依赖注入管理音频服务，提高代码的可测试性和可维护性
2. **异步编程**: 优先使用异步 API 进行音频操作，避免阻塞主线程
3. **资源管理**: 确保正确释放音频资源，避免内存泄漏
4. **错误处理**: 适当处理音频操作中可能出现的异常
5. **日志记录**: 配置适当的日志记录，便于调试和问题排查
6. **性能监控**: 监控音频处理性能，及时发现和解决性能瓶颈
7. **AOT 编译**: 对于性能敏感的音频应用，考虑使用 AOT 编译优化
8. **适当的缓冲**: 对于实时音频应用，使用适当的缓冲策略，平衡延迟和稳定性
9. **格式选择**: 根据应用场景选择合适的音频格式和编码参数
10. **测试验证**: 充分测试音频处理功能，确保在各种场景下正常工作

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的音频库**: 确保使用的音频库支持 AOT 编译
2. **避免反射**: 避免在音频处理中使用反射，或使用 Source Generator 替代
3. **资源加载**: 确保所有音频资源都能在 AOT 编译时被正确处理
4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
5. **测试验证**: 在 AOT 编译后进行充分的测试，确保音频功能正常工作
6. **性能优化**: AOT 编译可以显著提高音频应用的启动速度和运行性能
7. **内存优化**: AOT 编译可以减少音频应用的内存占用

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加音频服务
builder.Services.AddSingleton<IAudioRecorder, NaudioAudioRecorder>();
builder.Services.AddSingleton<IAudioPlayer, NaudioAudioPlayer>();
builder.Services.AddSpeechServices(options => {
    options.SubscriptionKey = builder.Configuration["SpeechService:SubscriptionKey"];
    options.Region = builder.Configuration["SpeechService:Region"];
});

var app = builder.Build();

// 定义音频 API 端点
app.MapPost("/api/audio/record", async ([FromServices] IAudioRecorder recorder) => {
    var audioData = await recorder.RecordAsync(TimeSpan.FromSeconds(10));
    return Results.Ok(new {
        Duration = audioData.Duration,
        Format = audioData.Format,
        Size = audioData.Size
    });
});

app.MapPost("/api/audio/recognize", async ([FromServices] ISpeechService speechService, [FromBody] AudioData audioData) => {
    var text = await speechService.RecognizeSpeechAsync(audioData);
    return Results.Ok(new { Text = text });
});

app.MapPost("/api/audio/synthesize", async ([FromServices] ISpeechService speechService, [FromBody] SpeechRequest request) => {
    var audioData = await speechService.SynthesizeSpeechAsync(request.Text, request.VoiceName);
    return Results.Ok(new {
        Audio = Convert.ToBase64String(audioData.Data),
        Format = audioData.Format
    });
});

app.Run();
```

### 与 SignalR 集成

```csharp
// 音频流处理 hub
public class AudioStreamHub : Hub
{
    private readonly IAudioProcessor _audioProcessor;
    
    public AudioStreamHub(IAudioProcessor audioProcessor)
    {
        _audioProcessor = audioProcessor;
    }
    
    // 处理客户端发送的音频流
    public async Task SendAudioStream(byte[] audioData)
    {
        try
        {
            // 处理音频数据
            var processedAudio = await _audioProcessor.ProcessAsync(audioData);
            
            // 广播处理后的音频数据
            await Clients.Others.SendAsync("ReceiveAudioStream", processedAudio);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("AudioError", ex.Message);
        }
    }
}
```
