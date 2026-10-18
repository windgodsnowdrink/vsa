using Plc.Plugins.Contracts;

namespace Plc.Host.Pipeline;

/// <summary>
/// 计量处理器：统计已处理帧数、成功率与端到端延迟（入队 → 处理）。
/// 默认注册在管道首阶段，为 <c>/sys/pipeline</c> 与 M6 门禁提供观测数据。
/// </summary>
public sealed class MetricsFrameHandler : IFrameHandler
{
    private long _processed;
    private long _failedFrames;
    private long _latencyTicks;
    private long _maxLatencyTicks;

    /// <summary>已处理帧数。</summary>
    public long Processed => Interlocked.Read(ref _processed);

    /// <summary>处理过的<strong>失败帧</strong>数（<see cref="DeviceFrame.IsSuccess"/> 为 false）。</summary>
    public long FailedFrames => Interlocked.Read(ref _failedFrames);

    /// <summary>平均端到端延迟；无样本时为 null。</summary>
    public TimeSpan? AverageLatency
    {
        get
        {
            var n = Processed;
            if (n == 0)
            {
                return null;
            }

            return TimeSpan.FromTicks(Interlocked.Read(ref _latencyTicks) / n);
        }
    }

    /// <summary>峰值端到端延迟；无样本时为 null。</summary>
    public TimeSpan? MaxLatency =>
        _maxLatencyTicks == 0 ? null : TimeSpan.FromTicks(Interlocked.Read(ref _maxLatencyTicks));

    public void OnFrame(DeviceFrame frame, long sequence, bool endOfBatch)
    {
        Interlocked.Increment(ref _processed);
        if (!frame.IsSuccess)
        {
            Interlocked.Increment(ref _failedFrames);
        }

        var latencyTicks = (DateTimeOffset.UtcNow - frame.Timestamp).Ticks;
        if (latencyTicks < 0)
        {
            latencyTicks = 0; // 时钟回拨保护
        }

        Interlocked.Add(ref _latencyTicks, latencyTicks);

        long seen;
        while (latencyTicks > (seen = Interlocked.Read(ref _maxLatencyTicks)))
        {
            if (Interlocked.CompareExchange(ref _maxLatencyTicks, latencyTicks, seen) == seen)
            {
                break;
            }
        }
    }
}
