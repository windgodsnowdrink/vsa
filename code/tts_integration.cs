using LogCorner.EduSync.Speech.Broker;
using LogCorner.EduSync.Speech.Command;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Threading.Channels;
using OpenTelemetry.Metrics;
using System.IO;
using System.Buffers;

#:sdk Microsoft.NET.Sdk.Web
#:package LogCorner.EduSync.Speech.Broker@latest
#:package LogCorner.EduSync.Speech.Command@latest
#:package Microsoft.Extensions.ObjectPool@latest
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

namespace TtsIntegration;

/// <summary>
/// TTS服务配置选项
/// </summary>
/// <remarks>
/// 包含语音合成和识别的各种参数配置
/// 支持本地语音引擎和Azure认知服务
/// </remarks>
/// <summary>
/// TTS配置选项（生产级优化）
/// 包含语音合成参数、缓存和性能优化配置
/// </summary>
public class TtsOptions
{
    /// <summary>
    /// 语音合成区域
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 音频处理管道容量
    /// </summary>
    public int AudioPipelineCapacity { get; set; } = 1000;

    /// <summary>
    /// 断路器触发阈值
    /// </summary>
    public int CircuitBreakerThreshold { get; set; } = 10;

    /// <summary>
    /// 断路器持续时间(秒)
    /// </summary>
    public int CircuitBreakerDuration { get; set; } = 30;

    /// <summary>
    /// 音频采样率
    /// </summary>
    public int SampleRate { get; set; } = 44100;

    /// <summary>
    /// 音频位深度
    /// </summary>
    public int BitDepth { get; set; } = 16;

    /// <summary>
    /// 音频通道数
    /// </summary>
    public int Channels { get; set; } = 2;

    /// <summary>
    /// 语音合成密钥
    /// </summary>
    public string SubscriptionKey { get; set; } = string.Empty;

    /// <summary>
    /// 默认语音名称
    /// </summary>
    public string VoiceName { get; set; } = "zh-CN-YunxiNeural";

    /// <summary>
    /// 缓存时长（秒）
    /// </summary>
    public int CacheDuration { get; set; } = 3600;

    /// <summary>
    /// 是否启用零拷贝优化
    /// </summary>
    public bool EnableZeroCopy { get; set; } = true;

    /// <summary>
    /// 线程本地缓存大小
    /// </summary>
    public int ThreadLocalCacheSize { get; set; } = 1024;

    /// <summary>
    /// 音频处理通道容量
    /// </summary>
    public int AudioChannelCapacity { get; set; } = 1000;

    /// <summary>
    /// CPU缓存行对齐大小
    /// </summary>
    public int CacheLineSize { get; set; } = 64;
{
    public int BufferSize { get; set; } = 1024;
    public int MaxConcurrentProcesses { get; set; } = 4;
    public string VoiceName { get; set; } = "Microsoft Huihui Desktop";
    public int AudioSampleRate { get; set; } = 16000;
    public string SpeechRecognitionCulture { get; set; } = "en-US";
}

/// <summary>
/// TTS处理器核心实现类
/// </summary>
/// <remarks>
/// 实现文本转语音(TTS)和语音转文本(STT)功能
/// 使用对象池管理资源，支持高性能并发处理
/// 集成实时音频处理管道
/// </remarks>
public class TtsProcessor : IAsyncDisposable
{
    private readonly ObjectPool<SpeechSynthesizer> _synthPool;
    private readonly ObjectPool<SpeechRecognizer> _recognizerPool;
    private readonly ObjectPool<MemoryStream> _memoryStreamPool;
    private readonly TtsOptions _options;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TtsProcessor> _logger;
    private readonly Channel<SpeechTask> _channel;
    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _cts;
    private readonly SpeechConfig _azureSpeechConfig;

    /// <summary>
/// 初始化TTS处理器
/// </summary>
/// <param name="synthPool">语音合成器对象池</param>
/// <param name="recognizerPool">语音识别器对象池</param>
/// <param name="memoryStreamPool">内存流对象池</param>
/// <param name="options">TTS配置选项</param>
/// <param name="cache">内存缓存用于音频结果缓存</param>
/// <param name="logger">日志记录器</param>
public TtsProcessor(
        ObjectPool<SpeechSynthesizer> synthPool,
        ObjectPool<SpeechRecognizer> recognizerPool,
        ObjectPool<MemoryStream> memoryStreamPool,
        IOptions<TtsOptions> options,
        IMemoryCache cache,
        ILogger<TtsProcessor> logger,
        IMeterFactory meterFactory,
        IAsyncPolicy<byte[]> circuitBreakerPolicy)
    {
        _synthPool = synthPool;
        _recognizerPool = recognizerPool;
        _memoryStreamPool = memoryStreamPool;
        _options = options.Value;
        _cache = cache;
        _logger = logger;
        _channel = Channel.CreateBounded<SpeechTask>(_options.BufferSize);
        _semaphore = new SemaphoreSlim(_options.MaxConcurrentProcesses);
        _cts = new CancellationTokenSource();

        if (!string.IsNullOrEmpty(_options.SpeechKey) && !string.IsNullOrEmpty(_options.SpeechRegion))
        {
            _azureSpeechConfig = SpeechConfig.FromSubscription(_options.SpeechKey, _options.SpeechRegion);
            _azureSpeechConfig.SpeechRecognitionLanguage = "zh-CN";
            _azureSpeechConfig.SetProfanity(ProfanityOption.Raw);
        }

        StartProcessingLoop();
    }
{
    private readonly SpeechSynthesizer _synthesizer;
    private readonly SpeechRecognitionEngine _recognizer;
    private readonly TtsOptions _options;

    public TtsProcessor(TtsOptions options)
    {
        _options = options;
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SetOutputToDefaultAudioDevice();
        _synthesizer.SelectVoice(options.VoiceName);

        _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(options.SpeechRecognitionCulture));
        _recognizer.SetInputToDefaultAudioDevice();
    }

    /// <summary>
/// 文本转语音(TTS)
/// </summary>
/// <param name="text">要转换的文本内容</param>
/// <param name="cancellationToken">取消令牌</param>
/// <returns>WAV格式的音频字节数组</returns>
/// <remarks>
/// 实现流程:
/// 1. 检查缓存
/// 2. 从对象池获取语音合成器
/// 3. 执行文本到语音转换
/// 4. 可选实时音频处理
/// 5. 缓存结果
/// </remarks>
public async Task<byte[]> TextToSpeechAsync(string text, CancellationToken cancellationToken = default)
{
    using var activity = TtsDiagnostics.Source.StartActivity(nameof(TextToSpeechAsync));
    TtsDiagnostics.RequestCounter.Add(1);
    
    try
    {
        return await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
    if (string.IsNullOrWhiteSpace(text))
        return Array.Empty<byte>();

    var cacheKey = $"tts_{text.GetHashCode()}";
    if (_cache.TryGetValue(cacheKey, out byte[] cachedAudio))
        return cachedAudio;

    var synth = _synthPool.Get();
    try
    {
        using var memoryStream = _memoryStreamPool.Get();
        synth.SetOutputToWaveStream(memoryStream);
        synth.Speak(text);

        if (_options.EnableRealTimeProcessing)
        {
            memoryStream.Position = 0;
            var processedAudio = await ProcessAudioRealTime(memoryStream, cancellationToken);
            _cache.Set(cacheKey, processedAudio, TimeSpan.FromHours(1));
            return processedAudio;
        }

        var audioData = memoryStream.ToArray();
        _cache.Set(cacheKey, audioData, TimeSpan.FromHours(1));
        return audioData;
        });
    }
    catch (Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        TtsDiagnostics.ErrorCounter.Add(1);
        throw;
    }
    finally
    {
        _synthPool.Return(synth);
    }
}
    {
        using var stream = new MemoryStream();
        _synthesizer.SetOutputToWaveStream(stream);
        _synthesizer.Speak(text);
        return stream.ToArray();
    }

    /// <summary>
/// 语音转文本(STT)
/// </summary>
/// <param name="audioData">WAV格式的音频数据</param>
/// <param name="cancellationToken">取消令牌</param>
/// <returns>识别出的文本内容</returns>
/// <remarks>
/// 实现流程:
/// 1. 验证输入音频
/// 2. 从对象池获取语音识别器
/// 3. 执行语音识别
/// 4. 可选实时音频预处理
/// </remarks>
public async Task<string> SpeechToTextAsync(byte[] audioData, CancellationToken cancellationToken = default)
{
    if (audioData == null || audioData.Length == 0)
        return string.Empty;

    var recognizer = _recognizerPool.Get();
    try
    {
        using var memoryStream = _memoryStreamPool.Get();
        await memoryStream.WriteAsync(audioData, 0, audioData.Length, cancellationToken);
        memoryStream.Position = 0;

        using var waveStream = new WaveFileReader(memoryStream);
        var format = waveStream.WaveFormat;

        if (_options.EnableRealTimeProcessing)
        {
            var processedAudio = await ProcessAudioRealTime(memoryStream, cancellationToken);
            memoryStream.SetLength(0);
            await memoryStream.WriteAsync(processedAudio, 0, processedAudio.Length, cancellationToken);
            memoryStream.Position = 0;
        }

        recognizer.SetInputToWaveStream(memoryStream);
        var result = await recognizer.RecognizeAsync();
        return result?.Text ?? string.Empty;
    }
    finally
    {
        _recognizerPool.Return(recognizer);
    }
}
    {
        using var stream = new MemoryStream(audioData);
        using var recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(_options.SpeechRecognitionCulture));
        recognizer.SetInputToWaveStream(stream);
        
        var result = recognizer.Recognize();
        return result?.Text ?? string.Empty;
    }
}

/// <summary>
/// TTS处理上下文
/// </summary>
/// <remarks>
/// 管理TTS命令的排队和处理
/// 使用Channel实现生产者-消费者模式
/// 通过对象池重用处理器实例
/// </remarks>
public class TtsContext : IAsyncDisposable
{
    private readonly Channel<TtsCommand> _commandChannel;
    private readonly ObjectPool<TtsProcessor> _processorPool;
    private readonly TtsOptions _options;
    private readonly ActionBlock<TtsCommand> _processingBlock;

    public TtsContext(TtsOptions options)
    {
        _options = options;
        _commandChannel = Channel.CreateBounded<TtsCommand>(options.BufferSize);
        _processorPool = new DefaultObjectPool<TtsProcessor>(new TtsProcessorPooledPolicy());

        _processingBlock = new ActionBlock<TtsCommand>(async command =>
        {
            var processor = _processorPool.Get();
            try
            {
                await processor.ProcessAsync(command);
            }
            finally
            {
                _processorPool.Return(processor);
            }
        }, new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = _options.MaxConcurrentProcesses
        });
    }

    public async ValueTask EnqueueCommandAsync(TtsCommand command)
    {
        await _commandChannel.Writer.WriteAsync(command);
    }

    public async ValueTask DisposeAsync()
    {
        _commandChannel.Writer.Complete();
        await _processingBlock.Completion;
    }
}

/// <summary>
/// 依赖注入扩展方法
/// </summary>
/// <remarks>
/// 提供TTS服务的DI容器注册
/// 配置选项模式和核心服务
/// </remarks>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTtsIntegration(this IServiceCollection services, Action<TtsOptions> configure)
    {
        var options = new TtsOptions();
        configure(options);

        services.AddSingleton(options);
        services.AddSingleton<TtsContext>();
        services.AddSingleton<ISpeechBroker, SpeechBroker>();
        services.AddSingleton<ISpeechCommandService, SpeechCommandService>();

        return services;
    }
}