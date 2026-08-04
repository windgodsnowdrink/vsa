#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.CognitiveServices.Speech@1.36.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Polly@8.3.1
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using Polly;
using System.Buffers;
using System.Threading.Channels;
using System.Threading.Tasks.Dataflow;

namespace AsrIntegration;

/// <summary>
/// ASR配置选项（生产级优化）
/// </summary>
/// <summary>
/// ASR配置选项（不可变记录类型）
/// 包含语音识别服务的所有可配置参数
/// 使用record类型确保配置对象不可变且具有值语义
/// </summary>
public sealed record AsrOptions
{
    /// <summary>
    /// 语音识别区域
    /// </summary>
    /// <summary>
/// 语音服务区域标识符（如"eastus"）
/// 必须与订阅密钥对应的区域匹配
/// </summary>
public string Region { get; init; } = string.Empty;
    
    /// <summary>
    /// 处理超时时间(毫秒)
    /// </summary>
    /// <summary>
/// 单次识别操作超时时间（毫秒）
/// 超过此时间未完成识别将抛出TimeoutException
/// </summary>
public int ProcessingTimeoutMs { get; init; } = 5000;

    /// <summary>
    /// 实时音频处理管道容量
    /// </summary>
    public int AudioPipelineCapacity { get; init; } = 1000;

    /// <summary>
    /// 断路器触发阈值
    /// </summary>
    public int CircuitBreakerThreshold { get; init; } = 10;

    /// <summary>
    /// 断路器持续时间(秒)
    /// </summary>
    public int CircuitBreakerDuration { get; init; } = 30;

    /// <summary>
    /// 音频采样率
    /// </summary>
    public int SampleRate { get; init; } = 44100;

    /// <summary>
    /// 音频位深度
    /// </summary>
    public int BitDepth { get; init; } = 16;

    /// <summary>
    /// 音频通道数
    /// </summary>
    public int Channels { get; init; } = 2;

    /// <summary>
    /// 语音识别密钥
    /// </summary>
    public string SubscriptionKey { get; init; } = string.Empty;

    /// <summary>
    /// 默认语言
    /// </summary>
    public string Language { get; init; } = "zh-CN";

    /// <summary>
    /// 缓存时长（秒）
    /// </summary>
    public int CacheDuration { get; init; } = 3600;

    /// <summary>
    /// 是否启用零拷贝优化
    /// </summary>
    public bool EnableZeroCopy { get; init; } = true;

    /// <summary>
    /// 线程本地缓存大小
    /// </summary>
    public int ThreadLocalCacheSize { get; init; } = 1024;

    /// <summary>
    /// 音频处理通道容量
    /// </summary>
    public int AudioChannelCapacity { get; init; } = 1000;

    /// <summary>
    /// CPU缓存行对齐大小
    /// </summary>
    public int CacheLineSize { get; init; } = 64;

    public AsrOptions()
    {
        if (string.IsNullOrWhiteSpace(Region))
            throw new ArgumentException("Region不能为空", nameof(Region));
            
        if (string.IsNullOrWhiteSpace(SubscriptionKey))
            throw new ArgumentException("SubscriptionKey不能为空", nameof(SubscriptionKey));
            
        if (ProcessingTimeoutMs <= 0)
            throw new ArgumentException("ProcessingTimeoutMs必须大于0", nameof(ProcessingTimeoutMs));
            
        if (AudioPipelineCapacity <= 0)
            throw new ArgumentException("AudioPipelineCapacity必须大于0", nameof(AudioPipelineCapacity));
            
        if (ThreadLocalCacheSize <= 0)
            throw new ArgumentException("ThreadLocalCacheSize必须大于0", nameof(ThreadLocalCacheSize));
            
        if (AudioChannelCapacity <= 0)
            throw new ArgumentException("AudioChannelCapacity必须大于0", nameof(AudioChannelCapacity));
            
        if (CacheLineSize <= 0)
            throw new ArgumentException("CacheLineSize必须大于0", nameof(CacheLineSize));
    }
}

/// <summary>
/// ASR处理器核心实现类
/// </summary>
/// <summary>
/// 音频处理管道核心接口
/// 定义语音识别服务的基本操作和状态查询
/// 实现此接口的类应保证线程安全
/// </summary>
public interface IAudioProcessingPipeline
{
    Task<string> RecognizeAsync(ReadOnlyMemory<byte> audioData, CancellationToken cancellationToken = default);
    Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
    int CurrentCircuitState { get; }
    int CurrentQueueSize { get; }
}

/// <summary>
/// ASR处理器核心实现类
/// 实现语音识别、资源管理、健康检查和断路器模式
/// 使用对象池管理SpeechRecognizer和MemoryStream实例
/// 采用Channel实现生产者-消费者模式处理音频队列
/// </summary>
public class AsrProcessor : IAsyncDisposable, IHealthCheck, IDisposable, IAudioProcessingPipeline
{
    /// <summary>
/// SpeechRecognizer对象池
/// 重用识别器实例以减少创建开销
/// </summary>
private readonly ObjectPool<SpeechRecognizer> _recognizerPool;
    private readonly ObjectPool<MemoryStream> _memoryStreamPool;
    private readonly AsrOptions _options;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AsrProcessor> _logger;
    /// <summary>
/// 音频任务处理通道
/// 采用BoundedChannel限制最大队列大小
/// 生产者-消费者模式解耦接收和处理
/// </summary>
private readonly Channel<AsrTask> _channel;
    /// <summary>
/// 信号量控制并发识别数量
/// 配合断路器阈值实现流量控制
/// </summary>
private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _cts;
    private readonly IReadOnlyDictionary<string, SpeechConfig> _speechConfigs;
    private readonly string _currentEngine;
    private readonly IAsyncPolicy<string> _circuitBreakerPolicy;
    private readonly ArrayPool<byte> _audioBufferPool;
    /// <summary>
/// 线程本地音频缓冲区
/// 每个工作线程独享内存区域，避免竞争
/// 使用ArrayPool分配，自动回收内存
/// </summary>
private readonly ThreadLocal<byte[]> _threadLocalBuffer;
    private bool _disposed;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_circuitBreakerPolicy is CircuitBreakerPolicy<string> cb && cb.CircuitState == CircuitState.Open)
                return HealthCheckResult.Degraded("ASR服务处于断路器开启状态");
                
            if (_channel.Count >= _options.AudioChannelCapacity * 0.9)
                return HealthCheckResult.Degraded("ASR处理队列接近满载");
                
            return HealthCheckResult.Healthy("ASR服务运行正常");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("ASR健康检查失败", ex);
        }
    }
    
    public static int CurrentCircuitState => _circuitBreakerPolicy is CircuitBreakerPolicy<string> cb 
        ? cb.CircuitState switch
        {
            CircuitState.Closed => 0,
            CircuitState.Open => 1,
            CircuitState.HalfOpen => 2,
            _ => 0
        }
        : 0;
        
    public static int CurrentQueueSize => _channel.Count;

    public AsrProcessor(
        ObjectPool<SpeechRecognizer> recognizerPool,
        ObjectPool<MemoryStream> memoryStreamPool,
        IOptions<AsrOptions> options,
        IMemoryCache cache,
        ILogger<AsrProcessor> logger,
        IMeterFactory meterFactory,
        IAsyncPolicy<string> circuitBreakerPolicy,
        IAsyncPolicy<string> timeoutPolicy)
    {
        _recognizerPool = recognizerPool;
        _memoryStreamPool = memoryStreamPool;
        _options = options.Value;
        _cache = cache;
        _logger = logger;
        _circuitBreakerPolicy = Policy.WrapAsync(circuitBreakerPolicy, timeoutPolicy);
        _channel = Channel.CreateBounded<AsrTask>(new BoundedChannelOptions(_options.AudioChannelCapacity)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });
        _semaphore = new SemaphoreSlim(_options.CircuitBreakerThreshold);
        _cts = new CancellationTokenSource();
        _audioBufferPool = ArrayPool<byte>.Create(_options.ThreadLocalCacheSize, _options.CacheLineSize);

        if (!string.IsNullOrEmpty(_options.SubscriptionKey) && !string.IsNullOrEmpty(_options.Region))
        {
            _speechConfigs = new Dictionary<string, SpeechConfig>
            {
                ["azure"] = SpeechConfig.FromSubscription(_options.SubscriptionKey, _options.Region),
                // 可以添加其他引擎配置
            };
            _currentEngine = "azure";
            _speechConfigs[_currentEngine].SpeechRecognitionLanguage = _options.Language;
        }

        StartProcessingLoop();
    }

    public async Task<IReadOnlyDictionary<string, string>> RecognizeBatchAsync(IReadOnlyDictionary<string, ReadOnlyMemory<byte>> audioBatch, CancellationToken cancellationToken = default)
    {
        using var activity = AsrDiagnostics.Source.StartActivity(nameof(RecognizeBatchAsync));
        AsrDiagnostics.BatchRequestCounter.Add(1);
        
        var tasks = audioBatch.Select(async kv => 
        {
            try
            {
                var result = await RecognizeAsync(kv.Value, cancellationToken);
                return (kv.Key, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批处理识别失败: {AudioId}", kv.Key);
                return (kv.Key, string.Empty);
            }
        });
        
        var results = await Task.WhenAll(tasks);
        return results.ToDictionary(x => x.Key, x => x.Item2);
    }
    
    public async Task<string> RecognizeAsync(ReadOnlyMemory<byte> audioData, CancellationToken cancellationToken = default)
{
    using var activity = AsrDiagnostics.Source.StartActivity(nameof(RecognizeAsync));
    AsrDiagnostics.RequestCounter.Add(1);
    AsrDiagnostics.QueueGauge.Record(_channel.Count);
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    
    // 分级降级策略：当队列负载超过80%时启用缓存优先模式
    var fallbackToCache = _channel.Count >= _options.AudioChannelCapacity * 0.8;
    
    try
    {
        return await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            if (audioData.IsEmpty)
                return string.Empty;

            var cacheKey = $"asr_{audioData.Span.GetHashCode()}";
            if (fallbackToCache && _cache.TryGetValue(cacheKey, out string cachedText))
            {
                AsrDiagnostics.LatencyHistogram.Record(stopwatch.ElapsedMilliseconds);
                AsrDiagnostics.FallbackCounter.Add(1);
                return cachedText;
            }

                var recognizer = _recognizerPool.Get();
                try
                {
                    var memoryStream = _memoryStreamPool.Get();
                    try
                    {
                        await memoryStream.WriteAsync(audioData, cancellationToken);
                        memoryStream.Position = 0;

                    recognizer.SetInputToWaveStream(memoryStream);
                    var result = await recognizer.RecognizeOnceAsync();
                    
                    var recognizedText = result.Text;
                    _cache.Set(cacheKey, recognizedText, TimeSpan.FromSeconds(_options.CacheDuration));
                    AsrDiagnostics.LatencyHistogram.Record(stopwatch.ElapsedMilliseconds);
                    return recognizedText;
                }
                finally
                {
                    _recognizerPool.Return(recognizer);
                }
            });
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            AsrDiagnostics.ErrorCounter.Add(1);
            throw;
        }
        
        try
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                if (audioData == null || audioData.Length == 0)
                    return string.Empty;

                var cacheKey = $"asr_{audioData.GetHashCode()}";
                if (_cache.TryGetValue(cacheKey, out string cachedText))
                    return cachedText;

                var recognizer = _recognizerPool.Get();
                try
                {
                    using var memoryStream = _memoryStreamPool.Get();
                    await memoryStream.WriteAsync(audioData, 0, audioData.Length, cancellationToken);
                    memoryStream.Position = 0;

                    recognizer.SetInputToWaveStream(memoryStream);
                    var result = await recognizer.RecognizeOnceAsync();
                    
                    var recognizedText = result.Text;
                    _cache.Set(cacheKey, recognizedText, TimeSpan.FromSeconds(_options.CacheDuration));
                    return recognizedText;
                }
                finally
                {
                    _recognizerPool.Return(recognizer);
                }
            });
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            AsrDiagnostics.ErrorCounter.Add(1);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }
    
    protected virtual async ValueTask DisposeAsyncCore()
    {
        try
        {
            await _channel.Writer.CompleteAsync().ConfigureAwait(false);
            _cts.Cancel();
            _semaphore.Dispose();
            _cts.Dispose();
            
            // 清理线程本地缓存
            if (_threadLocalBuffer.IsValueCreated)
            {
                var buffer = _threadLocalBuffer.Value;
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            _threadLocalBuffer.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ASR处理器异步释放资源时发生错误");
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            _semaphore.Dispose();
            _cts.Dispose();
            
            // 清理线程本地缓存
            if (_threadLocalBuffer.IsValueCreated)
            {
                var buffer = _threadLocalBuffer.Value;
                if (buffer != null)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            _threadLocalBuffer.Dispose();
        }
        
        _disposed = true;
    }

    private void StartProcessingLoop()
    {
        Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var task = await _channel.Reader.ReadAsync(_cts.Token);
                    await _semaphore.WaitAsync(_cts.Token);
                    
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await ProcessTaskAsync(task);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    }, _cts.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }, _cts.Token);
    }

    private async Task ProcessTaskAsync(AsrTask task)
    {
        // 实现任务处理逻辑
    }
}

/// <summary>
/// ASR诊断工具类
/// </summary>
public static class AsrDiagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("AsrIntegration");
    public static readonly Counter<int> RequestCounter = MeterProvider.Default.GetMeter("AsrIntegration").CreateCounter<int>("asr_requests");
    public static readonly Counter<int> ErrorCounter = MeterProvider.Default.GetMeter("AsrIntegration").CreateCounter<int>("asr_errors");
    public static readonly Counter<int> FallbackCounter = MeterProvider.Default.GetMeter("AsrIntegration").CreateCounter<int>("asr_fallback");
    public static readonly Histogram<double> LatencyHistogram = MeterProvider.Default.GetMeter("AsrIntegration")
        .CreateHistogram<double>("asr_latency", "milliseconds", "ASR处理延迟直方图");
    public static readonly ObservableGauge<int> CircuitBreakerState = MeterProvider.Default.GetMeter("AsrIntegration")
        .CreateObservableGauge("asr_circuit_breaker_state", () => AsrProcessor.CurrentCircuitState, "断路器状态(0=Closed,1=Open,2=HalfOpen)");
    public static readonly ObservableGauge<int> QueueGauge = MeterProvider.Default.GetMeter("AsrIntegration")
        .CreateObservableGauge("asr_queue_size", () => AsrProcessor.CurrentQueueSize, "当前请求队列大小");

    public static IHealthChecksBuilder AddAsrHealthChecks(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<AsrProcessor>("asr_health_check", 
            tags: new[] { "asr" });
    }
}

/// <summary>
/// 依赖注入扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAsrIntegration(this IServiceCollection services, Action<AsrOptions> configure)
    {
        services.Configure(configure);
        
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            return provider.Create<SpeechRecognizer>(new SpeechRecognizerPooledPolicy());
        });
        
        services.AddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            return provider.Create<MemoryStream>(new MemoryStreamPooledPolicy());
        });
        
        services.AddSingleton<IAsyncPolicy<string>>(sp =>
            Policy<string>
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    sp.GetRequiredService<IOptions<AsrOptions>>().Value.CircuitBreakerThreshold,
                    TimeSpan.FromSeconds(sp.GetRequiredService<IOptions<AsrOptions>>().Value.CircuitBreakerDuration)
                ));
                
        services.AddSingleton<IAsyncPolicy<string>>(sp =>
            Policy<string>
                .TimeoutAsync<string>(TimeSpan.FromMilliseconds(sp.GetRequiredService<IOptions<AsrOptions>>().Value.ProcessingTimeoutMs)));
        
        services.AddSingleton<AsrProcessor>();
        
        return services;
    }
}

internal class SpeechRecognizerPooledPolicy : PooledObjectPolicy<SpeechRecognizer>
{
    public override SpeechRecognizer Create()
    {
        return new SpeechRecognizer(SpeechConfig.FromSubscription("temp", "temp"));
    }

    public override bool Return(SpeechRecognizer obj)
    {
        return true;
    }
}

internal class MemoryStreamPooledPolicy : PooledObjectPolicy<MemoryStream>
{
    public override MemoryStream Create()
    {
        return new MemoryStream();
    }

    public override bool Return(MemoryStream obj)
    {
        obj.SetLength(0);
        return true;
    }
}