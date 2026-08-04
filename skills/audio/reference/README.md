# audio - 参考文档

## 概述

audio 是一个基于 .NET 10 的高性能音频处理系统，专为 .NET 开发者设计，提供了全面的音频功能，包括音频录制、播放、编辑、转换、语音识别（ASR）、文本到语音（TTS）、音频增强等核心功能，支持 AOT 编译优化，适用于各种音频应用场景。

## 核心组件

### 1. IAudioRecorder (音频录制器)
- **位置**: scripts/naudio_integration.cs, scripts/accord_audio_integration.cs
- **功能**: 提供音频录制功能，支持多种音频格式和设备
- **特性**: 
  - 支持多种音频输入设备
  - 支持多种音频格式（WAV, MP3, FLAC, OGG 等）
  - 支持设置采样率、位深度和声道数
  - 支持实时音频预览
  - 支持自动增益控制
  - 支持降噪处理

### 2. IAudioPlayer (音频播放器)
- **位置**: scripts/naudio_integration.cs, scripts/accord_audio_integration.cs
- **功能**: 提供音频播放功能，支持多种音频格式
- **特性**: 
  - 支持多种音频格式
  - 支持播放控制（播放、暂停、停止、音量调节等）
  - 支持音频可视化
  - 支持音频均衡器
  - 支持音频效果处理

### 3. IAudioConverter (音频转换器)
- **位置**: scripts/naudio_integration.cs, scripts/accord_audio_integration.cs
- **功能**: 提供音频格式转换和编辑功能
- **特性**: 
  - 支持多种音频格式之间的转换
  - 支持音频裁剪、合并、分割等编辑操作
  - 支持音频采样率和位深度转换
  - 支持音频压缩和质量调整
  - 支持批量转换

### 4. IAudioEnhancer (音频增强器)
- **位置**: scripts/audio_enhancement.cs
- **功能**: 提供音频增强和处理功能
- **特性**: 
  - 支持噪声 reduction
  - 支持回声消除
  - 支持音频增强
  - 支持低音增强和高音调节
  - 支持音频去混响
  - 支持实时音频处理

### 5. ISpeechService (语音服务)
- **位置**: scripts/speech_integration.cs, scripts/asr_integration.cs, scripts/tts_integration.cs
- **功能**: 提供语音识别和合成功能
- **特性**: 
  - 支持实时语音识别
  - 支持离线语音识别
  - 支持多种语言和方言
  - 支持文本到语音合成
  - 支持多种语音和风格
  - 支持语音情感调节

### 6. IRtcAudioProcessor (RTC 音频处理器)
- **位置**: scripts/rtc_audio_processor.cs
- **功能**: 为实时通信应用提供音频处理支持
- **特性**: 
  - 支持实时音频编码和解码
  - 支持网络抖动缓冲
  - 支持音频质量自适应
  - 支持多路音频混音
  - 支持音频降噪和回声消除

### 7. IVideoAudioProcessor (音视频处理器)
- **位置**: scripts/csvideo_audio.cs
- **功能**: 提供音视频同步处理功能
- **特性**: 
  - 支持音视频同步录制
  - 支持音视频同步播放
  - 支持音频和视频的同步编辑
  - 支持音视频格式转换
  - 支持音视频分离和合并

### 8. AOT Compilation (AOT 编译)
- **位置**: 内置支持
- **功能**: 将音频应用编译为本机代码，提高启动速度和运行性能
- **特性**: 
  - 支持静态代码分析
  - 减少内存占用
  - 提高启动速度
  - 优化运行时性能
  - 减少应用大小

## 使用示例

### 基本音频录制和播放

```csharp
// 创建服务容器
var services = new ServiceCollection();
services.AddSingleton<IAudioRecorder, NaudioAudioRecorder>();
services.AddSingleton<IAudioPlayer, NaudioAudioPlayer>();
var serviceProvider = services.BuildServiceProvider();

// 获取音频服务
var recorder = serviceProvider.GetRequiredService<IAudioRecorder>();
var player = serviceProvider.GetRequiredService<IAudioPlayer>();

// 配置录制参数
var recordOptions = new AudioRecordOptions {
    Format = AudioFormat.Wav,
    SampleRate = 44100,
    Channels = 2,
    BitDepth = 16,
    DeviceName = "默认麦克风"
};

// 录制音频
Console.WriteLine("开始录制音频...");
var audioData = await recorder.RecordAsync(TimeSpan.FromSeconds(5), recordOptions);
Console.WriteLine($"录制完成，音频长度: {audioData.Duration}");

// 播放音频
Console.WriteLine("播放录制的音频...");
await player.PlayAsync(audioData);
```

### 音频格式转换

```csharp
// 创建服务容器
var services = new ServiceCollection();
services.AddSingleton<IAudioConverter, NaudioAudioConverter>();
var serviceProvider = services.BuildServiceProvider();

// 获取音频转换器
var converter = serviceProvider.GetRequiredService<IAudioConverter>();

// 加载音频文件
var audioData = await AudioData.LoadFromFileAsync("input.wav");

// 转换为 MP3 格式
Console.WriteLine("转换音频格式...");
var mp3Data = await converter.ConvertAsync(audioData, AudioFormat.Mp3, new AudioConvertOptions {
    Quality = 0.8,
    BitRate = 128000
});

// 保存转换后的音频
await mp3Data.SaveToFileAsync("output.mp3");
Console.WriteLine("音频格式转换完成");
```

### 语音识别和合成

```csharp
// 创建服务容器
var services = new ServiceCollection();
services.AddSpeechServices(options => {
    options.SubscriptionKey = "your-subscription-key";
    options.Region = "your-region";
});
var serviceProvider = services.BuildServiceProvider();

// 获取语音服务
var speechService = serviceProvider.GetRequiredService<ISpeechService>();

// 语音识别
var audioData = await AudioData.LoadFromFileAsync("speech.wav");
Console.WriteLine("进行语音识别...");
var recognizedText = await speechService.RecognizeSpeechAsync(audioData, "zh-CN");
Console.WriteLine($"识别结果: {recognizedText}");

// 文本到语音合成
Console.WriteLine("进行文本到语音转换...");
var speechAudio = await speechService.SynthesizeSpeechAsync(
    "这是一个文本到语音的示例", 
    "zh-CN-YunxiNeural", 
    new SpeechSynthesisOptions {
        Style = SpeechStyle.Conversational,
        Rate = 1.0,
        Pitch = 0
    });

// 保存合成语音
await speechAudio.SaveToFileAsync("synthesized.mp3");
Console.WriteLine("文本到语音转换完成");
```

### 音频增强处理

```csharp
// 创建服务容器
var services = new ServiceCollection();
services.AddAudioEnhancement(options => {
    options.EnableNoiseReduction = true;
    options.EnableEchoCancellation = true;
    options.NoiseReductionLevel = 0.8;
});
var serviceProvider = services.BuildServiceProvider();

// 获取音频增强器
var enhancer = serviceProvider.GetRequiredService<IAudioEnhancer>();

// 加载音频文件
var audioData = await AudioData.LoadFromFileAsync("noisy_audio.wav");

// 增强音频
Console.WriteLine("进行音频增强处理...");
var enhancedAudio = await enhancer.EnhanceAsync(audioData, new AudioEnhancementOptions {
    EnableBassBoost = true,
    BassBoostLevel = 0.5,
    EnableTrebleBoost = true,
    TrebleBoostLevel = 0.3
});

// 保存增强后的音频
await enhancedAudio.SaveToFileAsync("enhanced_audio.wav");
Console.WriteLine("音频增强处理完成");
```

## 配置选项

### 音频录制配置

```json
{
  "AudioRecorderSettings": {
    "DefaultFormat": "Wav",          // 默认音频格式
    "DefaultSampleRate": 44100,       // 默认采样率
    "DefaultChannels": 2,             // 默认声道数
    "DefaultBitDepth": 16,            // 默认位深度
    "EnableAutoGainControl": true,    // 启用自动增益控制
    "EnableNoiseReduction": false,    // 启用降噪
    "BufferSize": 4096,               // 缓冲区大小
    "MaxRecordingDuration": "00:30:00" // 最大录制时长
  }
}
```

### 音频播放配置

```json
{
  "AudioPlayerSettings": {
    "DefaultVolume": 0.8,             // 默认音量
    "EnableEqualizer": true,           // 启用均衡器
    "EnableVisualization": false,      // 启用可视化
    "BufferSize": 4096,               // 缓冲区大小
    "EnableCrossfading": true,         // 启用淡入淡出
    "CrossfadeDuration": "00:00:02"   // 淡入淡出时长
  }
}
```

### 语音服务配置

```json
{
  "SpeechServiceSettings": {
    "SubscriptionKey": "your-subscription-key",  // 订阅密钥
    "Region": "your-region",                    // 区域
    "DefaultLanguage": "zh-CN",                // 默认语言
    "DefaultVoice": "zh-CN-YunxiNeural",       // 默认语音
    "EnableOfflineRecognition": false,          // 启用离线识别
    "RecognitionTimeout": "00:00:30",          // 识别超时
    "SynthesisQuality": "High"                  // 合成质量
  }
}
```

### 音频增强配置

```json
{
  "AudioEnhancementSettings": {
    "EnableNoiseReduction": true,     // 启用降噪
    "NoiseReductionLevel": 0.7,       // 降噪级别 (0-1)
    "EnableEchoCancellation": true,   // 启用回声消除
    "EnableBassBoost": false,          // 启用低音增强
    "BassBoostLevel": 0.5,            // 低音增强级别
    "EnableTrebleBoost": false,        // 启用高音增强
    "TrebleBoostLevel": 0.5,          // 高音增强级别
    "EnableReverbReduction": false,    // 启用去混响
    "ReverbReductionLevel": 0.5       // 去混响级别
  }
}
```

## 性能优化

### 应用开发优化

1. **使用 AOT 编译**: 对于性能敏感的音频应用，使用 AOT 编译优化
   ```csharp
   // 在项目文件中启用 AOT 编译
   <PublishAot>true</PublishAot>
   <TrimMode>Full</TrimMode>
   ```

2. **异步编程**: 优先使用异步 API 进行音频操作，避免阻塞主线程

3. **资源管理**: 确保正确释放音频资源，避免内存泄漏

4. **适当的缓冲区大小**: 根据应用场景选择合适的缓冲区大小，平衡延迟和稳定性

5. **批量处理**: 对于大量音频文件，使用批量处理提高效率

6. **缓存机制**: 对于频繁使用的音频资源，使用缓存机制减少重复加载

### 运行时优化

1. **硬件加速**: 启用硬件加速，利用 GPU 进行音频处理

2. **多线程处理**: 对于复杂的音频处理任务，使用多线程并行处理

3. **内存池**: 使用内存池管理音频缓冲区，减少内存分配和回收开销

4. **采样率优化**: 根据应用场景选择合适的采样率，避免不必要的高采样率

5. **音频格式优化**: 选择高效的音频格式，减少存储空间和处理开销

6. **实时处理优化**: 对于实时音频应用，优化算法减少处理延迟

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

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的音频库**: 确保使用的音频库支持 AOT 编译

2. **避免反射**: 避免在音频处理中使用反射，或使用 Source Generator 替代

3. **资源加载**: 确保所有音频资源都能在 AOT 编译时被正确处理

4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit

5. **测试验证**: 在 AOT 编译后进行充分的测试，确保音频功能正常工作

6. **性能测试**: 比较 AOT 编译前后的性能差异，优化编译配置

7. **调试**: 使用 AOT 编译后的调试工具，如 dotnet-gcdump 和 dotnet-trace

## 故障排除

### 常见问题

1. **音频录制失败**
   - 检查音频输入设备是否可用
   - 检查设备权限设置
   - 检查音频驱动程序是否最新
   - 检查音频格式是否支持

2. **音频播放失败**
   - 检查音频输出设备是否可用
   - 检查音频格式是否支持
   - 检查音频文件是否损坏
   - 检查音量设置

3. **语音识别准确率低**
   - 检查音频质量是否良好
   - 检查背景噪声是否过大
   - 检查语音识别语言设置是否正确
   - 尝试调整麦克风位置和距离

4. **音频延迟问题**
   - 调整缓冲区大小
   - 优化音频处理算法
   - 关闭不必要的音频效果
   - 使用高性能音频设备

5. **内存占用过高**
   - 优化音频缓冲区管理
   - 使用内存池减少内存分配
   - 及时释放不再使用的音频资源
   - 降低音频采样率和位深度

6. **AOT 编译错误**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射或动态代码生成
   - 检查是否正确配置了 AOT 编译选项
   - 查看详细的编译日志，定位错误原因

### 调试建议

1. **启用详细日志**: 配置音频服务日志为详细级别，便于调试
   ```csharp
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Debug);
   ```

2. **使用音频分析工具**: 使用音频分析工具，如 Audacity，分析音频质量

3. **性能分析**: 使用性能分析工具，如 dotnet-trace，分析性能瓶颈

4. **内存分析**: 使用内存分析工具，如 dotnet-gcdump，分析内存使用情况

5. **网络调试**: 对于网络相关的音频应用，使用网络调试工具，如 Wireshark

6. **设备测试**: 测试不同的音频设备，排除设备问题

## 扩展开发

### 实现自定义音频处理器

```csharp
// 定义自定义音频处理器接口
public interface ICustomAudioProcessor
{
    Task<AudioData> ProcessAsync(AudioData audioData, CustomProcessingOptions options);
}

// 实现自定义音频处理器
public class CustomAudioProcessor : ICustomAudioProcessor
{
    private readonly ILogger<CustomAudioProcessor> _logger;
    
    public CustomAudioProcessor(ILogger<CustomAudioProcessor> logger)
    {
        _logger = logger;
    }
    
    public async Task<AudioData> ProcessAsync(AudioData audioData, CustomProcessingOptions options)
    {
        _logger.LogInformation("开始自定义音频处理");
        
        // 实现自定义音频处理逻辑
        // 例如：音频特效处理、音频分析等
        
        // 示例：简单的音频增益处理
        var processedData = new AudioData(audioData.Format, audioData.SampleRate, audioData.Channels, audioData.BitDepth);
        processedData.Data = await ApplyGainAsync(audioData.Data, options.GainLevel);
        processedData.Duration = audioData.Duration;
        
        _logger.LogInformation("自定义音频处理完成");
        return processedData;
    }
    
    private async Task<byte[]> ApplyGainAsync(byte[] audioData, float gainLevel)
    {
        // 实现音频增益处理
        // 这里使用简单的示例实现
        var result = new byte[audioData.Length];
        Array.Copy(audioData, result, audioData.Length);
        
        // 实际应用中，这里应该根据音频格式和位深度进行适当的增益处理
        
        return await Task.FromResult(result);
    }
}

// 定义处理选项
public class CustomProcessingOptions
{
    public float GainLevel { get; set; } = 1.0f;
    public bool EnableEcho { get; set; } = false;
    public float EchoDelay { get; set; } = 0.5f;
}

// 扩展服务注册
public static class CustomAudioProcessorExtensions
{
    public static IServiceCollection AddCustomAudioProcessor(this IServiceCollection services, Action<CustomProcessingOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        
        services.AddSingleton<ICustomAudioProcessor, CustomAudioProcessor>();
        return services;
    }
}

// 使用自定义音频处理器
var services = new ServiceCollection();
services.AddLogging(configure => configure.AddConsole());
services.AddCustomAudioProcessor(options => {
    options.GainLevel = 1.5f;
    options.EnableEcho = true;
    options.EchoDelay = 0.3f;
});

var serviceProvider = services.BuildServiceProvider();
var processor = serviceProvider.GetRequiredService<ICustomAudioProcessor>();
var audioData = await AudioData.LoadFromFileAsync("input.wav");
var processedAudio = await processor.ProcessAsync(audioData, new CustomProcessingOptions {
    GainLevel = 2.0f
});
await processedAudio.SaveToFileAsync("output.wav");
```

### 扩展音频格式支持

```csharp
// 实现自定义音频格式转换器
public class CustomAudioFormatConverter : IAudioFormatConverter
{
    public string FormatName => "CustomFormat";
    public string FormatExtension => ".cust";
    
    public async Task<AudioData> DecodeAsync(byte[] data)
    {
        // 实现自定义格式解码
        // 返回解码后的 AudioData
        return await Task.FromResult(new AudioData(AudioFormat.Wav, 44100, 2, 16));
    }
    
    public async Task<byte[]> EncodeAsync(AudioData audioData)
    {
        // 实现自定义格式编码
        // 返回编码后的字节数组
        return await Task.FromResult(Array.Empty<byte>());
    }
    
    public bool IsFormatSupported(byte[] data)
    {
        // 检查是否支持该格式
        return data.Length > 4 && data[0] == 'C' && data[1] == 'U' && data[2] == 'S' && data[3] == 'T';
    }
}

// 注册自定义格式转换器
services.AddSingleton<IAudioFormatConverter, CustomAudioFormatConverter>();
```

### 实现自定义语音识别引擎

```csharp
// 实现自定义语音识别引擎
public class CustomSpeechRecognizer : ISpeechRecognizer
{
    public string Name => "CustomSpeechRecognizer";
    public bool SupportsRealTime => true;
    public bool SupportsOffline => false;
    
    public async Task<string> RecognizeAsync(AudioData audioData, string language = "zh-CN")
    {
        // 实现自定义语音识别逻辑
        // 例如：调用自定义的语音识别 API 或模型
        return await Task.FromResult("识别结果");
    }
    
    public async IAsyncEnumerable<string> RecognizeContinuousAsync(AudioData audioData, string language = "zh-CN")
    {
        // 实现连续语音识别
        yield return "连续识别结果 1";
        await Task.Delay(1000);
        yield return "连续识别结果 2";
    }
}

// 注册自定义语音识别引擎
services.AddSingleton<ISpeechRecognizer, CustomSpeechRecognizer>();
```

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

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// 定义音频 API 端点
app.MapPost("/api/audio/record", async ([FromServices] IAudioRecorder recorder) => {
    var audioData = await recorder.RecordAsync(TimeSpan.FromSeconds(10));
    return Results.Ok(new {
        Duration = audioData.Duration,
        Format = audioData.Format,
        Size = audioData.Size
    });
});

app.MapPost("/api/audio/recognize", async ([FromServices] ISpeechService speechService, [FromBody] AudioRequest request) => {
    var audioData = new AudioData(request.Format, request.SampleRate, request.Channels, request.BitDepth, Convert.FromBase64String(request.Data));
    var text = await speechService.RecognizeSpeechAsync(audioData, request.Language);
    return Results.Ok(new { Text = text });
});

app.MapPost("/api/audio/synthesize", async ([FromServices] ISpeechService speechService, [FromBody] SpeechRequest request) => {
    var audioData = await speechService.SynthesizeSpeechAsync(request.Text, request.VoiceName, new SpeechSynthesisOptions {
        Style = request.Style,
        Rate = request.Rate,
        Pitch = request.Pitch
    });
    return Results.Ok(new {
        Audio = Convert.ToBase64String(audioData.Data),
        Format = audioData.Format,
        Duration = audioData.Duration
    });
});

app.Run();
```

### 与 Blazor 集成

```razor
@page "/audio-recorder"
@inject IAudioRecorder Recorder
@inject IAudioPlayer Player

<h3>音频录制器</h3>

<div class="audio-controls">
    <button @onclick="StartRecording" disabled="@IsRecording">开始录制</button>
    <button @onclick="StopRecording" disabled="@!IsRecording">停止录制</button>
    <button @onclick="PlayRecording" disabled="@!HasRecording">播放录制</button>
    <button @onclick="SaveRecording" disabled="@!HasRecording">保存录制</button>
</div>

<div class="audio-info">
    @if (IsRecording)
    {
        <p>正在录制... @RecordingDuration</p>
    }
    else if (HasRecording)
    {
        <p>录制时长: @RecordedAudio.Duration</p>
        <p>音频格式: @RecordedAudio.Format</p>
        <p>文件大小: @(RecordedAudio.Size / 1024) KB</p>
    }
</div>

@code {
    private bool IsRecording { get; set; } = false;
    private bool HasRecording { get; set; } = false;
    private TimeSpan RecordingDuration { get; set; } = TimeSpan.Zero;
    private AudioData RecordedAudio { get; set; }
    private CancellationTokenSource _cts;
    
    private async Task StartRecording()
    {
        IsRecording = true;
        _cts = new CancellationTokenSource();
        
        // 启动录制计时器
        var timer = new Timer(_ => {
            RecordingDuration += TimeSpan.FromSeconds(1);
            StateHasChanged();
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        
        try
        {
            RecordedAudio = await Recorder.RecordAsync(TimeSpan.FromSeconds(30), cancellationToken: _cts.Token);
            HasRecording = true;
        }
        catch (OperationCanceledException)
        {
            // 录制被取消
        }
        finally
        {
            IsRecording = false;
            timer.Dispose();
            _cts.Dispose();
        }
    }
    
    private void StopRecording()
    {
        _cts.Cancel();
    }
    
    private async Task PlayRecording()
    {
        if (RecordedAudio != null)
        {
            await Player.PlayAsync(RecordedAudio);
        }
    }
    
    private async Task SaveRecording()
    {
        if (RecordedAudio != null)
        {
            // 实现保存逻辑，例如上传到服务器或保存到本地
            var fileName = $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            await RecordedAudio.SaveToFileAsync(fileName);
            await JSRuntime.InvokeVoidAsync("alert", $"音频已保存: {fileName}");
        }
    }
}
```

## 性能测试和基准测试

### 音频处理性能测试

```csharp
// 音频处理性能测试
public class AudioPerformanceTest
{
    private readonly IAudioEnhancer _enhancer;
    private readonly IAudioConverter _converter;
    private readonly ISpeechService _speechService;
    
    public AudioPerformanceTest(IAudioEnhancer enhancer, IAudioConverter converter, ISpeechService speechService)
    {
        _enhancer = enhancer;
        _converter = converter;
        _speechService = speechService;
    }
    
    public async Task RunTests()
    {
        // 加载测试音频文件
        var audioData = await AudioData.LoadFromFileAsync("test_audio.wav");
        
        Console.WriteLine("音频性能测试开始");
        Console.WriteLine($"测试音频: {audioData.Duration}, {audioData.Format}, {audioData.SampleRate}Hz, {audioData.Channels}ch");
        
        // 测试音频增强性能
        await TestAudioEnhancement(audioData);
        
        // 测试音频转换性能
        await TestAudioConversion(audioData);
        
        // 测试语音识别性能
        await TestSpeechRecognition(audioData);
        
        // 测试语音合成性能
        await TestSpeechSynthesis();
        
        Console.WriteLine("音频性能测试完成");
    }
    
    private async Task TestAudioEnhancement(AudioData audioData)
    {
        var stopwatch = Stopwatch.StartNew();
        var enhancedAudio = await _enhancer.EnhanceAsync(audioData);
        stopwatch.Stop();
        
        Console.WriteLine($"音频增强测试: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  输入大小: {audioData.Data.Length / 1024} KB");
        Console.WriteLine($"  输出大小: {enhancedAudio.Data.Length / 1024} KB");
        Console.WriteLine($"  处理速度: {audioData.Data.Length / 1024 / stopwatch.Elapsed.TotalSeconds:F2} KB/s");
    }
    
    private async Task TestAudioConversion(AudioData audioData)
    {
        var stopwatch = Stopwatch.StartNew();
        var mp3Audio = await _converter.ConvertAsync(audioData, AudioFormat.Mp3);
        stopwatch.Stop();
        
        Console.WriteLine($"音频转换测试: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  输入格式: {audioData.Format}");
        Console.WriteLine($"  输出格式: {mp3Audio.Format}");
        Console.WriteLine($"  压缩比: {(float)mp3Audio.Data.Length / audioData.Data.Length:F2}:1");
        Console.WriteLine($"  转换速度: {audioData.Data.Length / 1024 / stopwatch.Elapsed.TotalSeconds:F2} KB/s");
    }
    
    private async Task TestSpeechRecognition(AudioData audioData)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await _speechService.RecognizeSpeechAsync(audioData);
        stopwatch.Stop();
        
        Console.WriteLine($"语音识别测试: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  识别结果长度: {result.Length} 字符");
        Console.WriteLine($"  识别速度: {audioData.Duration.TotalSeconds / stopwatch.Elapsed.TotalSeconds:F2}x 实时");
    }
    
    private async Task TestSpeechSynthesis()
    {
        var testText = "这是一个用于测试语音合成性能的示例文本。";
        var stopwatch = Stopwatch.StartNew();
        var audioData = await _speechService.SynthesizeSpeechAsync(testText);
        stopwatch.Stop();
        
        Console.WriteLine($"语音合成测试: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  文本长度: {testText.Length} 字符");
        Console.WriteLine($"  合成音频时长: {audioData.Duration}");
        Console.WriteLine($"  合成速度: {audioData.Duration.TotalSeconds / stopwatch.Elapsed.TotalSeconds:F2}x 实时");
    }
}

// 运行性能测试
var services = new ServiceCollection();
services.AddAudioEnhancement();
services.AddSingleton<IAudioConverter, NaudioAudioConverter>();
services.AddSpeechServices();
services.AddSingleton<AudioPerformanceTest>();

var serviceProvider = services.BuildServiceProvider();
var test = serviceProvider.GetRequiredService<AudioPerformanceTest>();
await test.RunTests();
```

### AOT 编译性能对比

```csharp
// AOT 编译性能对比测试
public class AotPerformanceComparison
{
    public static async Task RunComparison()
    {
        Console.WriteLine("AOT 编译性能对比测试");
        Console.WriteLine("=" * 50);
        
        // 测试 JIT 编译版本
        Console.WriteLine("1. 测试 JIT 编译版本:");
        await TestJitVersion();
        
        // 测试 AOT 编译版本
        Console.WriteLine("\n2. 测试 AOT 编译版本:");
        await TestAotVersion();
        
        Console.WriteLine("\nAOT 编译性能对比测试完成");
    }
    
    private static async Task TestJitVersion()
    {
        var stopwatch = Stopwatch.StartNew();
        
        // 启动 JIT 编译版本的应用
        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "dotnet",
                Arguments = "AudioApp.dll",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        await process.WaitForExitAsync();
        
        stopwatch.Stop();
        Console.WriteLine($"  启动时间: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  退出代码: {process.ExitCode}");
    }
    
    private static async Task TestAotVersion()
    {
        var stopwatch = Stopwatch.StartNew();
        
        // 启动 AOT 编译版本的应用
        var process = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "./AudioApp",
                Arguments = "",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        await process.WaitForExitAsync();
        
        stopwatch.Stop();
        Console.WriteLine($"  启动时间: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  退出代码: {process.ExitCode}");
    }
}

// 运行 AOT 编译性能对比测试
await AotPerformanceComparison.RunComparison();
```
