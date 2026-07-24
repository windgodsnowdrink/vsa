# audio - 使用示例

## 快速入门

### 1. 基本音频录制和播放示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package NAudio@2.2.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("基本音频录制和播放示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<IAudioRecorder, BasicAudioRecorder>();
        services.AddSingleton<IAudioPlayer, BasicAudioPlayer>();
        
        var serviceProvider = services.BuildServiceProvider();
        var recorder = serviceProvider.GetRequiredService<IAudioRecorder>();
        var player = serviceProvider.GetRequiredService<IAudioPlayer>();
        
        try
        {
            // 录制音频
            Console.WriteLine("按任意键开始录制音频 (5秒)...");
            Console.ReadKey(true);
            Console.WriteLine("正在录制...");
            
            var audioData = await recorder.RecordAsync(TimeSpan.FromSeconds(5));
            Console.WriteLine($"录制完成！音频时长: {audioData.Duration}");
            Console.WriteLine($"音频格式: {audioData.Format}, 采样率: {audioData.SampleRate}Hz");
            Console.WriteLine($"声道数: {audioData.Channels}, 位深度: {audioData.BitDepth}");
            
            // 播放音频
            Console.WriteLine("\n按任意键播放录制的音频...");
            Console.ReadKey(true);
            Console.WriteLine("正在播放...");
            
            await player.PlayAsync(audioData);
            Console.WriteLine("播放完成！");
            
            // 保存音频
            var fileName = $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            await audioData.SaveToFileAsync(fileName);
            Console.WriteLine($"\n音频已保存到文件: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"出现错误: {ex.Message}");
        }
    }
}

// 音频数据模型
public class AudioData
{
    public AudioFormat Format { get; set; }
    public int SampleRate { get; set; }
    public int Channels { get; set; }
    public int BitDepth { get; set; }
    public byte[] Data { get; set; }
    public TimeSpan Duration { get; set; }
    public long Size => Data?.Length ?? 0;
    
    public async Task SaveToFileAsync(string fileName)
    {
        using var writer = new WaveFileWriter(fileName, new WaveFormat(SampleRate, BitDepth, Channels));
        await writer.WriteAsync(Data.AsMemory(0, Data.Length));
    }
}

// 音频格式枚举
public enum AudioFormat
{
    Wav,
    Mp3,
    Flac,
    Ogg
}

// 音频录制器接口
public interface IAudioRecorder
{
    Task<AudioData> RecordAsync(TimeSpan duration);
}

// 基本音频录制器实现
public class BasicAudioRecorder : IAudioRecorder
{
    private readonly ILogger<BasicAudioRecorder> _logger;
    
    public BasicAudioRecorder(ILogger<BasicAudioRecorder> logger)
    {
        _logger = logger;
    }
    
    public async Task<AudioData> RecordAsync(TimeSpan duration)
    {
        _logger.LogInformation("开始录制音频，时长: {Duration}", duration);
        
        using var waveIn = new WaveInEvent();
        waveIn.WaveFormat = new WaveFormat(44100, 16, 2);
        
        var buffer = new List<byte>();
        
        waveIn.DataAvailable += (sender, e) => {
            var data = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, data, 0, e.BytesRecorded);
            buffer.AddRange(data);
        };
        
        waveIn.StartRecording();
        
        await Task.Delay(duration);
        
        waveIn.StopRecording();
        
        _logger.LogInformation("录制完成，获取到 {Bytes} 字节数据", buffer.Count);
        
        return new AudioData
        {
            Format = AudioFormat.Wav,
            SampleRate = 44100,
            Channels = 2,
            BitDepth = 16,
            Data = buffer.ToArray(),
            Duration = duration
        };
    }
}

// 音频播放器接口
public interface IAudioPlayer
{
    Task PlayAsync(AudioData audioData);
}

// 基本音频播放器实现
public class BasicAudioPlayer : IAudioPlayer
{
    private readonly ILogger<BasicAudioPlayer> _logger;
    
    public BasicAudioPlayer(ILogger<BasicAudioPlayer> logger)
    {
        _logger = logger;
    }
    
    public async Task PlayAsync(AudioData audioData)
    {
        _logger.LogInformation("开始播放音频，时长: {Duration}", audioData.Duration);
        
        using var waveOut = new WaveOutEvent();
        using var waveStream = new RawSourceWaveStream(
            audioData.Data.AsMemory(0, audioData.Data.Length),
            new WaveFormat(audioData.SampleRate, audioData.BitDepth, audioData.Channels));
        
        waveOut.Init(waveStream);
        
        var tcs = new TaskCompletionSource<bool>();
        waveOut.PlaybackStopped += (sender, e) => tcs.TrySetResult(true);
        
        waveOut.Play();
        await tcs.Task;
        
        _logger.LogInformation("音频播放完成");
    }
}
```

### 2. 音频格式转换示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package NAudio@2.2.1
#:package NAudio.Lame@1.1.2
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using NAudio.Lame;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("音频格式转换示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<IAudioConverter, BasicAudioConverter>();
        
        var serviceProvider = services.BuildServiceProvider();
        var converter = serviceProvider.GetRequiredService<IAudioConverter>();
        
        try
        {
            // 创建示例 WAV 音频（正弦波）
            Console.WriteLine("生成示例 WAV 音频...");
            var wavData = GenerateTestWavAudio();
            Console.WriteLine($"生成的 WAV 音频: {wavData.Duration}, {wavData.Size / 1024} KB");
            
            // 转换为 MP3
            Console.WriteLine("\n转换为 MP3 格式...");
            var mp3Data = await converter.ConvertToMp3Async(wavData, 128);
            Console.WriteLine($"MP3 转换完成: {mp3Data.Duration}, {mp3Data.Size / 1024} KB");
            Console.WriteLine($"压缩比: {(float)mp3Data.Size / wavData.Size:F2}:1");
            
            // 保存文件
            var wavFileName = $"test_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            var mp3FileName = $"test_{DateTime.Now:yyyyMMdd_HHmmss}.mp3";
            
            await wavData.SaveToFileAsync(wavFileName);
            await mp3Data.SaveToFileAsync(mp3FileName);
            
            Console.WriteLine($"\n文件已保存:");
            Console.WriteLine($"- WAV: {wavFileName}");
            Console.WriteLine($"- MP3: {mp3FileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"出现错误: {ex.Message}");
        }
    }
    
    // 生成测试 WAV 音频（正弦波）
    private static AudioData GenerateTestWavAudio()
    {
        int sampleRate = 44100;
        int channels = 2;
        int bitDepth = 16;
        double durationSeconds = 3;
        
        int sampleCount = (int)(sampleRate * durationSeconds * channels);
        short[] samples = new short[sampleCount];
        
        // 生成 440Hz 正弦波
        double frequency = 440;
        for (int i = 0; i < sampleCount; i += channels)
        {
            double t = i / (double)sampleRate;
            short sample = (short)(Math.Sin(2 * Math.PI * frequency * t) * short.MaxValue * 0.3);
            
            // 双声道
            samples[i] = sample;
            if (channels == 2)
            {
                samples[i + 1] = sample;
            }
        }
        
        // 转换为字节数组
        byte[] data = new byte[samples.Length * 2];
        Buffer.BlockCopy(samples, 0, data, 0, data.Length);
        
        return new AudioData
        {
            Format = AudioFormat.Wav,
            SampleRate = sampleRate,
            Channels = channels,
            BitDepth = bitDepth,
            Data = data,
            Duration = TimeSpan.FromSeconds(durationSeconds)
        };
    }
}

// 音频转换器接口
public interface IAudioConverter
{
    Task<AudioData> ConvertToMp3Async(AudioData wavData, int bitRate);
}

// 基本音频转换器实现
public class BasicAudioConverter : IAudioConverter
{
    private readonly ILogger<BasicAudioConverter> _logger;
    
    public BasicAudioConverter(ILogger<BasicAudioConverter> logger)
    {
        _logger = logger;
    }
    
    public async Task<AudioData> ConvertToMp3Async(AudioData wavData, int bitRate)
    {
        _logger.LogInformation("开始转换 WAV 到 MP3，比特率: {BitRate}kbps", bitRate);
        
        using var ms = new MemoryStream();
        using var waveStream = new RawSourceWaveStream(
            wavData.Data.AsMemory(0, wavData.Data.Length),
            new WaveFormat(wavData.SampleRate, wavData.BitDepth, wavData.Channels));
        
        // 使用 LAME 编码器转换为 MP3
        using var mp3Writer = new LameMP3FileWriter(ms, waveStream.WaveFormat, bitRate);
        await waveStream.CopyToAsync(mp3Writer);
        
        var mp3Data = ms.ToArray();
        _logger.LogInformation("MP3 转换完成，输出大小: {Bytes} 字节", mp3Data.Length);
        
        return new AudioData
        {
            Format = AudioFormat.Mp3,
            SampleRate = wavData.SampleRate,
            Channels = wavData.Channels,
            BitDepth = wavData.BitDepth,
            Data = mp3Data,
            Duration = wavData.Duration
        };
    }
}
```

### 3. 语音识别 (ASR) 示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Configuration.Json@10.0.0
#:package Microsoft.CognitiveServices.Speech@1.37.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("语音识别 (ASR) 示例");
        Console.WriteLine("=" * 50);
        
        // 构建配置
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ISpeechRecognizer, AzureSpeechRecognizer>();
        
        var serviceProvider = services.BuildServiceProvider();
        var speechRecognizer = serviceProvider.GetRequiredService<ISpeechRecognizer>();
        
        try
        {
            Console.WriteLine("开始语音识别...");
            Console.WriteLine("请说话，说完后按 Enter 键结束...");
            
            // 实时语音识别
            var recognizedText = await speechRecognizer.RecognizeFromMicrophoneAsync();
            
            Console.WriteLine($"\n识别结果: {recognizedText}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"出现错误: {ex.Message}");
            Console.WriteLine("请确保已在 appsettings.json 中配置了正确的 Azure Speech 服务密钥和区域");
        }
    }
}

// 语音识别器接口
public interface ISpeechRecognizer
{
    Task<string> RecognizeFromMicrophoneAsync();
    Task<string> RecognizeFromFileAsync(string audioFilePath);
}

// Azure Speech 识别器实现
public class AzureSpeechRecognizer : ISpeechRecognizer
{
    private readonly ILogger<AzureSpeechRecognizer> _logger;
    private readonly IConfiguration _configuration;
    
    public AzureSpeechRecognizer(ILogger<AzureSpeechRecognizer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task<string> RecognizeFromMicrophoneAsync()
    {
        // 从配置获取 Azure Speech 服务密钥和区域
        var subscriptionKey = _configuration["SpeechService:SubscriptionKey"] ?? throw new InvalidOperationException("SpeechService:SubscriptionKey is not configured");
        var region = _configuration["SpeechService:Region"] ?? throw new InvalidOperationException("SpeechService:Region is not configured");
        
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechRecognitionLanguage = "zh-CN";
        
        using var recognizer = new SpeechRecognizer(config);
        _logger.LogInformation("Azure Speech 识别器已初始化，开始录音...");
        
        Console.WriteLine("正在监听...");
        
        // 连续识别直到用户按 Enter
        var stopRecognition = new TaskCompletionSource<int>();
        var recognizedText = new System.Text.StringBuilder();
        
        // 连接事件
        recognizer.Recognizing += (s, e) => {
            Console.Write($"\r正在识别: {e.Result.Text}");
        };
        
        recognizer.Recognized += (s, e) => {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                recognizedText.Append(e.Result.Text);
            }
        };
        
        recognizer.Canceled += (s, e) => {
            Console.WriteLine($"\n识别取消: {e.Reason}");
            if (e.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"错误详情: {e.ErrorDetails}");
            }
            stopRecognition.TrySetResult(0);
        };
        
        recognizer.SessionStopped += (s, e) => {
            stopRecognition.TrySetResult(0);
        };
        
        // 开始连续识别
        await recognizer.StartContinuousRecognitionAsync();
        
        // 等待用户按 Enter 键停止
        Console.WriteLine("\n\n请说话... (按 Enter 键结束)");
        Console.ReadLine();
        
        // 停止识别
        await recognizer.StopContinuousRecognitionAsync();
        await stopRecognition.Task;
        
        var result = recognizedText.ToString().Trim();
        _logger.LogInformation("语音识别完成，结果: {Text}", result);
        
        return result;
    }
    
    public async Task<string> RecognizeFromFileAsync(string audioFilePath)
    {
        throw new NotImplementedException();
    }
}
```

### 4. 文本到语音 (TTS) 示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Configuration.Json@10.0.0
#:package Microsoft.CognitiveServices.Speech@1.37.0
#:package NAudio.Wave@2.2.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.CognitiveServices.Speech;
using NAudio.Wave;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("文本到语音 (TTS) 示例");
        Console.WriteLine("=" * 50);
        
        // 构建配置
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ITextToSpeechService, AzureTextToSpeechService>();
        services.AddSingleton<IAudioPlayer, BasicAudioPlayer>();
        
        var serviceProvider = services.BuildServiceProvider();
        var ttsService = serviceProvider.GetRequiredService<ITextToSpeechService>();
        var player = serviceProvider.GetRequiredService<IAudioPlayer>();
        
        try
        {
            // 要转换的文本
            var text = "欢迎使用文本到语音服务。这是一个基于 Azure Cognitive Services 的示例，展示了如何将文本转换为自然流畅的语音。";
            Console.WriteLine($"要转换的文本: {text}");
            
            // 选择语音
            Console.WriteLine("\n可用语音:");
            Console.WriteLine("1. 中文女声 (zh-CN-YunxiNeural)");
            Console.WriteLine("2. 中文男声 (zh-CN-YunjianNeural)");
            Console.Write("请选择语音 (1-2): ");
            
            var voiceChoice = Console.ReadLine();
            var voiceName = voiceChoice == "2" ? "zh-CN-YunjianNeural" : "zh-CN-YunxiNeural";
            
            // 转换文本到语音
            Console.WriteLine($"\n正在转换文本到语音...");
            var speechAudio = await ttsService.SynthesizeSpeechAsync(text, voiceName);
            
            Console.WriteLine($"转换完成！音频时长: {speechAudio.Duration}");
            
            // 播放生成的语音
            Console.WriteLine("\n按任意键播放生成的语音...");
            Console.ReadKey(true);
            Console.WriteLine("正在播放...");
            
            await player.PlayAsync(speechAudio);
            Console.WriteLine("播放完成！");
            
            // 保存语音
            var fileName = $"tts_output_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            await speechAudio.SaveToFileAsync(fileName);
            Console.WriteLine($"\n语音已保存到文件: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"出现错误: {ex.Message}");
            Console.WriteLine("请确保已在 appsettings.json 中配置了正确的 Azure Speech 服务密钥和区域");
        }
    }
}

// 文本到语音服务接口
public interface ITextToSpeechService
{
    Task<AudioData> SynthesizeSpeechAsync(string text, string voiceName);
}

// Azure 文本到语音服务实现
public class AzureTextToSpeechService : ITextToSpeechService
{
    private readonly ILogger<AzureTextToSpeechService> _logger;
    private readonly IConfiguration _configuration;
    
    public AzureTextToSpeechService(ILogger<AzureTextToSpeechService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task<AudioData> SynthesizeSpeechAsync(string text, string voiceName)
    {
        // 从配置获取 Azure Speech 服务密钥和区域
        var subscriptionKey = _configuration["SpeechService:SubscriptionKey"] ?? throw new InvalidOperationException("SpeechService:SubscriptionKey is not configured");
        var region = _configuration["SpeechService:Region"] ?? throw new InvalidOperationException("SpeechService:Region is not configured");
        
        var config = SpeechConfig.FromSubscription(subscriptionKey, region);
        config.SpeechSynthesisLanguage = "zh-CN";
        config.SpeechSynthesisVoiceName = voiceName;
        
        using var synthesizer = new SpeechSynthesizer(config);
        
        _logger.LogInformation("开始文本到语音转换");
        _logger.LogInformation("文本长度: {Length} 字符", text.Length);
        _logger.LogInformation("使用语音: {VoiceName}", voiceName);
        
        // 执行语音合成
        var result = await synthesizer.SpeakTextAsync(text);
        
        if (result.Reason != ResultReason.SynthesizingAudioCompleted)
        {
            throw new Exception($"语音合成失败: {result.Reason}");
        }
        
        _logger.LogInformation("语音合成完成，音频大小: {Bytes} 字节", result.AudioData.Length);
        
        // 将音频数据转换为 AudioData 对象
        return new AudioData
        {
            Format = AudioFormat.Wav,
            SampleRate = 24000, // Azure TTS 默认采样率
            Channels = 1,
            BitDepth = 16,
            Data = result.AudioData,
            Duration = TimeSpan.FromSeconds((double)result.AudioData.Length / (24000 * 2)) // 估算时长
        };
    }
}

// 基本音频播放器（简化版）
public class BasicAudioPlayer : IAudioPlayer
{
    public async Task PlayAsync(AudioData audioData)
    {
        using var waveOut = new WaveOutEvent();
        using var waveStream = new RawSourceWaveStream(
            audioData.Data.AsMemory(0, audioData.Data.Length),
            new WaveFormat(audioData.SampleRate, audioData.BitDepth, audioData.Channels));
        
        waveOut.Init(waveStream);
        
        var tcs = new TaskCompletionSource<bool>();
        waveOut.PlaybackStopped += (sender, e) => tcs.TrySetResult(true);
        
        waveOut.Play();
        await tcs.Task;
    }
}
```

## AOT 编译示例

### AOT 优化的音频应用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package NAudio@2.2.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAudio.Wave;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("AOT 优化的音频应用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器（AOT 兼容方式）
        var services = new ServiceCollection();
        
        // 配置日志（AOT 兼容）
        services.AddLogging(configure => {
            configure.AddSimpleConsole(options => {
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        });
        
        // 注册音频服务（AOT 兼容）
        services.AddSingleton<IAudioProcessor, AotOptimizedAudioProcessor>();
        services.AddSingleton<IAudioPlayer, AotOptimizedAudioPlayer>();
        
        var serviceProvider = services.BuildServiceProvider();
        var processor = serviceProvider.GetRequiredService<IAudioProcessor>();
        var player = serviceProvider.GetRequiredService<IAudioPlayer>();
        
        try
        {
            Console.WriteLine("AOT 应用启动成功！");
            Console.WriteLine("当前时间: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            // 生成测试音频
            Console.WriteLine("\n生成测试音频...");
            var testAudio = GenerateTestAudio();
            Console.WriteLine($"测试音频: {testAudio.Duration}, {testAudio.SampleRate}Hz");
            
            // 处理音频（AOT 优化）
            Console.WriteLine("\n处理音频（AOT 优化）...");
            var processedAudio = await processor.ProcessAsync(testAudio);
            Console.WriteLine("音频处理完成！");
            
            // 播放处理后的音频
            Console.WriteLine("\n播放处理后的音频...");
            await player.PlayAsync(processedAudio);
            Console.WriteLine("播放完成！");
            
            Console.WriteLine("\nAOT 音频应用运行成功！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
    }
    
    // 生成测试音频（AOT 兼容）
    private static AudioData GenerateTestAudio()
    {
        const int sampleRate = 44100;
        const int channels = 1;
        const int bitDepth = 16;
        const double durationSeconds = 2;
        
        int sampleCount = (int)(sampleRate * durationSeconds * channels);
        short[] samples = new short[sampleCount];
        
        // 生成简单的正弦波
        double frequency = 440; // A4 音符
        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)sampleRate;
            short sample = (short)(Math.Sin(2 * Math.PI * frequency * t) * short.MaxValue * 0.3);
            samples[i] = sample;
        }
        
        // 转换为字节数组（AOT 兼容方式）
        byte[] data = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            byte[] sampleBytes = BitConverter.GetBytes(samples[i]);
            data[i * 2] = sampleBytes[0];
            data[i * 2 + 1] = sampleBytes[1];
        }
        
        return new AudioData
        {
            Format = AudioFormat.Wav,
            SampleRate = sampleRate,
            Channels = channels,
            BitDepth = bitDepth,
            Data = data,
            Duration = TimeSpan.FromSeconds(durationSeconds)
        };
    }
}

// 音频数据模型（AOT 兼容）
public sealed class AudioData
{
    public AudioFormat Format { get; init; }
    public int SampleRate { get; init; }
    public int Channels { get; init; }
    public int BitDepth { get; init; }
    public byte[] Data { get; init; }
    public TimeSpan Duration { get; init; }
    public long Size => Data?.Length ?? 0;
    
    // AOT 兼容的保存方法
    public async Task SaveToFileAsync(string filePath)
    {
        using var stream = File.Create(filePath);
        await stream.WriteAsync(Data.AsMemory(0, Data.Length));
    }
}

// 音频格式枚举（AOT 兼容）
public enum AudioFormat
{
    Wav,
    Mp3,
    Flac,
    Ogg
}

// 音频处理器接口（AOT 兼容）
public interface IAudioProcessor
{
    Task<AudioData> ProcessAsync(AudioData audioData);
}

// AOT 优化的音频处理器
public sealed class AotOptimizedAudioProcessor : IAudioProcessor
{
    private readonly ILogger<AotOptimizedAudioProcessor> _logger;
    
    public AotOptimizedAudioProcessor(ILogger<AotOptimizedAudioProcessor> logger)
    {
        _logger = logger;
    }
    
    // AOT 优化的音频处理方法
    public async Task<AudioData> ProcessAsync(AudioData audioData)
    {
        _logger.LogInformation("开始音频处理");
        
        // 模拟音频处理（AOT 兼容方式）
        byte[] processedData = new byte[audioData.Data.Length];
        
        // 简单的音频增益处理（AOT 兼容）
        const float gain = 1.5f;
        
        // 处理音频数据（AOT 兼容方式）
        for (int i = 0; i < audioData.Data.Length; i += 2)
        {
            // 读取 16 位样本
            short sample = BitConverter.ToInt16(audioData.Data, i);
            
            // 应用增益
            float processedSample = sample * gain;
            
            // 限制范围
            if (processedSample > short.MaxValue)
                processedSample = short.MaxValue;
            if (processedSample < short.MinValue)
                processedSample = short.MinValue;
            
            // 写回数据
            short finalSample = (short)processedSample;
            byte[] sampleBytes = BitConverter.GetBytes(finalSample);
            processedData[i] = sampleBytes[0];
            processedData[i + 1] = sampleBytes[1];
        }
        
        await Task.CompletedTask; // 模拟异步
        
        _logger.LogInformation("音频处理完成");
        
        return new AudioData
        {
            Format = audioData.Format,
            SampleRate = audioData.SampleRate,
            Channels = audioData.Channels,
            BitDepth = audioData.BitDepth,
            Data = processedData,
            Duration = audioData.Duration
        };
    }
}

// 音频播放器接口（AOT 兼容）
public interface IAudioPlayer
{
    Task PlayAsync(AudioData audioData);
}

// AOT 优化的音频播放器
public sealed class AotOptimizedAudioPlayer : IAudioPlayer
{
    private readonly ILogger<AotOptimizedAudioPlayer> _logger;
    
    public AotOptimizedAudioPlayer(ILogger<AotOptimizedAudioPlayer> logger)
    {
        _logger = logger;
    }
    
    // AOT 优化的音频播放方法
    public async Task PlayAsync(AudioData audioData)
    {
        _logger.LogInformation("开始播放音频");
        
        // 简单的音频播放实现（AOT 兼容）
        // 注意：在实际 AOT 应用中，可能需要使用更简单的音频播放方式
        // 这里仅作示例，实际使用时可能需要调整
        
        try
        {
            // 简单的音频播放实现
            using var waveOut = new WaveOutEvent();
            using var waveStream = new RawSourceWaveStream(
                audioData.Data.AsMemory(0, audioData.Data.Length),
                new WaveFormat(audioData.SampleRate, audioData.BitDepth, audioData.Channels));
            
            waveOut.Init(waveStream);
            
            var tcs = new TaskCompletionSource<bool>();
            waveOut.PlaybackStopped += (sender, e) => tcs.TrySetResult(true);
            
            waveOut.Play();
            await tcs.Task;
            
            _logger.LogInformation("音频播放完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "音频播放失败");
            Console.WriteLine($"播放失败（AOT 环境可能需要特殊配置）: {ex.Message}");
        }
    }
}
```

### AOT 编译配置和命令

```xml
<!-- AOT 编译配置文件 (csproj) -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- AOT 编译配置 -->
    <PublishAot>true</PublishAot>
    <TrimMode>Full</TrimMode>
    <PublishReadyToRun>true</PublishReadyToRun>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    
    <!-- 优化配置 -->
    <Optimize>true</Optimize>
    <DebugType>None</DebugType>
    <DebugSymbols>false</DebugSymbols>
    
    <!-- AOT 兼容配置 -->
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <EnableAotAnalyzer>true</EnableAotAnalyzer>
    <IsAotCompatible>true</IsAotCompatible>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
    <PackageReference Include="NAudio" Version="2.2.1" />
  </ItemGroup>
</Project>
```

```bash
# AOT 编译命令

# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained

# 运行 AOT 编译后的应用（Windows）
./bin/Release/net11.0/win-x64/publish/AudioApp.exe

# 运行 AOT 编译后的应用（Linux）
chmod +x ./bin/Release/net11.0/linux-x64/publish/AudioApp
./bin/Release/net11.0/linux-x64/publish/AudioApp

# 运行 AOT 编译后的应用（macOS）
chmod +x ./bin/Release/net11.0/osx-x64/publish/AudioApp
./bin/Release/net11.0/osx-x64/publish/AudioApp
```

## 总结

上述示例展示了 audio 技能的主要功能和使用方法，包括：

1. **基本音频录制和播放**: 演示了如何使用 NAudio 进行音频录制和播放
2. **音频格式转换**: 展示了如何将 WAV 音频转换为 MP3 格式
3. **语音识别 (ASR)**: 集成 Azure Cognitive Services 实现实时语音识别
4. **文本到语音 (TTS)**: 集成 Azure Cognitive Services 实现文本到语音转换
5. **AOT 编译支持**: 提供了 AOT 优化的音频应用示例，包括：
   - AOT 兼容的音频处理
   - AOT 编译配置文件
   - AOT 编译命令

所有示例均遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的音频应用。audio 技能提供了全面的音频解决方案，支持 AOT 编译优化，可用于各种音频相关的应用场景。
