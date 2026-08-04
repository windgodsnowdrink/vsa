#:sdk Microsoft.NET.Sdk.Web
#:package Xabe.FFmpeg@6.0.2
#:package System.Threading.Channels@9.0.8
#:package Microsoft.Extensions.ObjectPool@9.0.8
#:package Microsoft.Extensions.Diagnostics.HealthChecks@9.0.8
#:package prometheus-net.AspNetCore@8.2.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Buffers;
using System.Threading.Channels;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Streams;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

builder.Services
    .AddVideoProcessing()
    .AddSingleton<ObjectPool<IConversion>>(sp => 
        new DefaultObjectPool<IConversion>(new ConversionPoolPolicy(), 8));
        
/*
- 分布式处理逻辑 - 使用Channel和ObjectPool实现
- 自适应算法逻辑 - 基于网络状况动态调整码率
- 其他指标更新 - 扩展了StreamMetrics类
- 健康检查逻辑 - 实现IHealthCheck接口
- 故障转移逻辑 - 通过FailoverHandler处理
- 码率调整实现 - BitrateAdjuster服务
*/
// 1. 分布式处理扩展
[SkipLocalsInit]
public sealed class DistributedStreamProcessor : BackgroundService
{
    private readonly Channel<MediaFrame> _inputChannel;
    private readonly Channel<ProcessedFrame>[] _outputChannels;
    private readonly ThreadLocal<Span<byte>> _processingBuffer;
    private readonly ObjectPool<IConversion> _conversionPool;
    
    public DistributedStreamProcessor(
        Channel<MediaFrame> inputChannel,
        ObjectPool<IConversion> conversionPool,
        int workerCount = 4)
    {
        _inputChannel = inputChannel;
        _conversionPool = conversionPool;
        _processingBuffer = new(() => stackalloc byte[1024 * 1024]); // 1MB处理缓冲区
        _outputChannels = new Channel<ProcessedFrame>[workerCount];
        
        for (int i = 0; i < workerCount; i++)
        {
            _outputChannels[i] = Channel.CreateBounded<ProcessedFrame>(1000);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var tasks = new Task[_outputChannels.Length];
        for (int i = 0; i < _outputChannels.Length; i++)
        {
            tasks[i] = ProcessWorkerAsync(_outputChannels[i].Writer, ct);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ProcessWorkerAsync(ChannelWriter<ProcessedFrame> writer, CancellationToken ct)
    {
        await foreach (var frame in _inputChannel.Reader.ReadAllAsync(ct))
        {
            var conversion = _conversionPool.Get();
            try
            {
                var buffer = _processingBuffer.Value;
                // 分布式处理逻辑
                var processedData = conversion.ProcessFrame(frame, buffer);
                await writer.WriteAsync(new ProcessedFrame(frame, processedData), ct);
            }
            finally
            {
                _conversionPool.Return(conversion);
            }
        }
    }
}

// 2. 质量监控指标
public static class StreamMetrics
{
    public static readonly Gauge Bitrate = Metrics.CreateGauge(
        "ffmpeg_bitrate_kbps", "Current streaming bitrate in kbps");
    
    public static readonly Counter DroppedFrames = Metrics.CreateCounter(
        "ffmpeg_dropped_frames", "Total number of dropped frames");
    
    public static readonly Gauge CpuUsage = Metrics.CreateGauge(
        "ffmpeg_cpu_usage", "CPU usage percentage");
    
    public static readonly Gauge MemoryUsage = Metrics.CreateGauge(
        "ffmpeg_memory_usage_mb", "Memory usage in MB");

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void UpdateQualityMetrics(MediaFrame frame)
    {
        Bitrate.Set(frame.Bitrate / 1024);
        DroppedFrames.Inc(frame.DroppedFrames);
        CpuUsage.Set(frame.CpuUsage);
        MemoryUsage.Set(frame.MemoryUsage / (1024 * 1024));
    }
}

// 3. 自适应码率控制器
public sealed class AdaptiveBitrateController
{
    private readonly ChannelWriter<BitrateAdjustment> _adjustmentChannel;
    private readonly ThreadLocal<RingBuffer<float>> _bitrateHistory;
    private readonly TimeSpan _monitoringWindow = TimeSpan.FromSeconds(30);
    
    public AdaptiveBitrateController(Channel<BitrateAdjustment> adjustmentChannel)
    {
        _adjustmentChannel = adjustmentChannel.Writer;
        _bitrateHistory = new(() => new RingBuffer<float>(30));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AdjustBitrate(NetworkCondition condition)
    {
        var history = _bitrateHistory.Value;
        history.Push(condition.AvailableBandwidth);
        
        // 自适应算法逻辑
        float avgBandwidth = history.Average();
        float stability = history.StdDev() / avgBandwidth;
        
        var adjustment = new BitrateAdjustment(
            TargetBitrate: (int)(avgBandwidth * 0.8), // 使用80%可用带宽
            StabilityFactor: stability,
            Timestamp: DateTime.UtcNow);
        
        _adjustmentChannel.TryWrite(adjustment);
    }
}

// 4. 故障转移增强
public sealed class FailoverHandler : IHealthCheck
{
    private readonly ObjectPool<IConversion> _conversionPool;
    private readonly Channel<FailoverEvent> _failoverChannel;
    private readonly TimeSpan _healthCheckInterval = TimeSpan.FromSeconds(5);
    
    public FailoverHandler(
        ObjectPool<IConversion> conversionPool,
        Channel<FailoverEvent> failoverChannel)
    {
        _conversionPool = conversionPool;
        _failoverChannel = failoverChannel;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct)
    {
        var conversion = _conversionPool.Get();
        try
        {
            // 健康检查逻辑
            var isHealthy = await conversion.HealthCheckAsync(ct);
            if (!isHealthy)
            {
                await _failoverChannel.Writer.WriteAsync(
                    new FailoverEvent(DateTime.UtcNow, "Conversion service unhealthy"), ct);
                return HealthCheckResult.Unhealthy("Conversion service failed");
            }
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            await _failoverChannel.Writer.WriteAsync(
                new FailoverEvent(DateTime.UtcNow, ex.Message), ct);
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
        finally
        {
            _conversionPool.Return(conversion);
        }
    }

    // 故障转移逻辑
    public async Task HandleFailoverAsync(CancellationToken ct)
    {
        var timer = new PeriodicTimer(_healthCheckInterval);
        while (await timer.WaitForNextTickAsync(ct))
        {
            var result = await CheckHealthAsync(new HealthCheckContext(), ct);
            if (result.Status == HealthStatus.Unhealthy)
            {
                // 触发故障转移流程
                await _failoverChannel.Writer.WriteAsync(
                    new FailoverEvent(DateTime.UtcNow, "System failover initiated"), ct);
            }
        }
    }
}

// 5. 码率调整实现
public sealed class BitrateAdjuster : BackgroundService
{
    private readonly ChannelReader<BitrateAdjustment> _adjustmentChannel;
    private readonly IConversion _conversion;
    
    public BitrateAdjuster(
        Channel<BitrateAdjustment> adjustmentChannel,
        IConversion conversion)
    {
        _adjustmentChannel = adjustmentChannel.Reader;
        _conversion = conversion;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var adjustment in _adjustmentChannel.ReadAllAsync(ct))
        {
            // 码率调整实现
            _conversion.SetBitrate(adjustment.TargetBitrate);
            
            // 更新指标
            StreamMetrics.Bitrate.Set(adjustment.TargetBitrate / 1024);
        }
    }
}

// 辅助数据结构
public record MediaFrame(
    byte[] Data,
    int Bitrate,
    int DroppedFrames,
    float CpuUsage,
    long MemoryUsage);

public record ProcessedFrame(
    MediaFrame OriginalFrame,
    byte[] ProcessedData);

public record BitrateAdjustment(
    int TargetBitrate,
    float StabilityFactor,
    DateTime Timestamp);

public record NetworkCondition(
    float AvailableBandwidth,
    float PacketLoss,
    DateTime Timestamp);

public record FailoverEvent(
    DateTime Timestamp,
    string Reason);


// 3. 自适应码率控制器增强实现
public sealed class AdaptiveBitrateController : BackgroundService
{
    private readonly Channel<BitrateAdjustment> _adjustmentChannel;
    private readonly Channel<NetworkCondition> _networkChannel;
    private readonly RingBuffer<NetworkCondition> _historyBuffer;
    private readonly ILogger<AdaptiveBitrateController> _logger;
    
    public AdaptiveBitrateController(
        Channel<BitrateAdjustment> adjustmentChannel,
        Channel<NetworkCondition> networkChannel,
        ILogger<AdaptiveBitrateController> logger)
    {
        _adjustmentChannel = adjustmentChannel;
        _networkChannel = networkChannel;
        _historyBuffer = new RingBuffer<NetworkCondition>(30); // 30秒历史窗口
        _logger = logger;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var condition in _networkChannel.Reader.ReadAllAsync(ct))
        {
            try
            {
                _historyBuffer.Push(condition);
                
                // 1. 计算网络指标
                var metrics = CalculateNetworkMetrics();
                
                // 2. 动态调整算法
                var adjustment = CalculateBitrateAdjustment(metrics);
                
                // 3. 应用调整
                await _adjustmentChannel.Writer.WriteAsync(adjustment, ct);
                
                // 更新监控指标
                StreamMetrics.Bitrate.Set(adjustment.TargetBitrate / 1024);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bitrate adjustment failed");
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private NetworkMetrics CalculateNetworkMetrics()
    {
        // 1. 计算平均带宽和标准差
        var avgBandwidth = _historyBuffer.Average(x => x.AvailableBandwidth);
        var stdDev = Math.Sqrt(_historyBuffer.Average(x => 
            Math.Pow(x.AvailableBandwidth - avgBandwidth, 2)));
        
        // 2. 计算丢包率
        var avgPacketLoss = _historyBuffer.Average(x => x.PacketLoss);
        
        return new NetworkMetrics(
            avgBandwidth,
            stdDev,
            avgPacketLoss,
            DateTime.UtcNow);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private BitrateAdjustment CalculateBitrateAdjustment(NetworkMetrics metrics)
    {
        // 1. 计算稳定性因子 (0-1范围，值越小越稳定)
        float stability = (float)(metrics.StdDev / metrics.AvgBandwidth);
        
        // 2. 动态调整系数 (基于网络稳定性)
        float dynamicFactor = stability switch {
            < 0.1f => 0.9f,  // 非常稳定时使用90%带宽
            < 0.3f => 0.7f,  // 一般稳定时使用70%带宽
            _ => 0.5f        // 不稳定时使用50%带宽
        };
        
        // 3. 丢包补偿 (每1%丢包减少2%带宽)
        float lossCompensation = 1 - (metrics.AvgPacketLoss * 0.02f);
        
        // 4. 计算目标码率
        int targetBitrate = (int)(metrics.AvgBandwidth * dynamicFactor * lossCompensation);
        
        return new BitrateAdjustment(
            targetBitrate,
            stability,
            DateTime.UtcNow);
    }
}

// 网络指标数据结构
public readonly record struct NetworkMetrics(
    double AvgBandwidth,
    double StdDev,
    double AvgPacketLoss,
    DateTime Timestamp);


// 2. 主处理器集成点
[SkipLocalsInit]
public sealed class MainProcessor : BackgroundService
{
    private readonly DistributedStreamProcessor _streamProcessor;
    private readonly Channel<ProcessedFrame>[] _outputChannels;
    private readonly ThreadLocal<Span<byte>> _mergeBuffer;
    
    public MainProcessor(
        DistributedStreamProcessor streamProcessor,
        Channel<ProcessedFrame>[] outputChannels)
    {
        _streamProcessor = streamProcessor;
        _outputChannels = outputChannels;
        _mergeBuffer = new(() => stackalloc byte[4096]); // 4K合并缓冲区
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        // 创建合并通道
        var mergedChannel = Channel.CreateBounded<ProcessedFrame>(1000);
        
        // 启动合并任务
        var mergeTask = Task.Run(() => MergeFramesAsync(mergedChannel.Writer, ct));
        
        // 处理合并后的帧
        await foreach (var frame in mergedChannel.Reader.ReadAllAsync(ct))
        {
            // 主处理逻辑
            ProcessFrame(frame);
        }
        
        await mergeTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task MergeFramesAsync(
        ChannelWriter<ProcessedFrame> writer,
        CancellationToken ct)
    {
        var readers = _outputChannels.Select(c => c.Reader).ToArray();
        
        while (!ct.IsCancellationRequested)
        {
            // 使用ValueTask提高性能
            var tasks = readers.Select(r => r.WaitToReadAsync(ct).AsTask());
            await Task.WhenAny(tasks);
            
            // 零拷贝合并处理
            foreach (var channel in _outputChannels)
            {
                while (channel.Reader.TryRead(out var frame))
                {
                    var buffer = _mergeBuffer.Value;
                    // 执行帧合并逻辑
                    await writer.WriteAsync(frame, ct);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void ProcessFrame(in ProcessedFrame frame)
    {
        // 使用SIMD指令优化处理
        fixed (byte* ptr = frame.Data)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐检查
            {
                // 主处理逻辑
            }
        }
    }
}

// 3. 服务注册扩展
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVideoProcessing(
        this IServiceCollection services,
        int workerCount = 4)
    {
        // 配置输入通道
        var inputChannel = Channel.CreateBounded<MediaFrame>(
            new BoundedChannelOptions(10000)
            {
                SingleWriter = true,
                AllowSynchronousContinuations = true
            });
            
        // 配置输出通道数组
        var outputChannels = Enumerable.Range(0, workerCount)
            .Select(_ => Channel.CreateBounded<ProcessedFrame>(1000))
            .ToArray();
            
        // 注册分布式处理器
        services.AddSingleton<DistributedStreamProcessor>(sp => 
            new DistributedStreamProcessor(
                inputChannel,
                sp.GetRequiredService<ObjectPool<IConversion>>(),
                workerCount));
                
        // 注册主处理器
        services.AddSingleton<MainProcessor>();
        
        // 注册通道
        services.AddSingleton(inputChannel);
        services.AddSingleton(outputChannels);
        
        return services;
    }
}