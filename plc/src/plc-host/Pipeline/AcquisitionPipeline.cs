using System.Diagnostics;
using Disruptor;
using Disruptor.Dsl;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace Plc.Host.Pipeline;

/// <summary>采集管道配置。</summary>
/// <param name="RingSize">
/// 环形缓冲区槽位数，<strong>必须是 2 的幂</strong>（Disruptor 硬性要求；非法值抛
/// <see cref="ArgumentOutOfRangeException"/> 而不是在运行时静默降级）。
/// </param>
/// <param name="DropOnFull">
/// 缓冲区满时的背压策略：<c>true</c> → <see cref="AcquisitionPipeline.TryPublish"/> 返回 false
/// 并计入丢弃（采集侧继续跑，不阻塞）；<c>false</c> → 调用阻塞式
/// <see cref="AcquisitionPipeline.Publish"/> 等待空位。
/// </param>
/// <param name="ShutdownTimeoutMs">关闭时等待在飞事件排空的上限。</param>
public sealed record PipelineOptions(
    int RingSize = 1024,
    bool DropOnFull = true,
    int ShutdownTimeoutMs = 5000);

/// <summary>
/// 采集管道（ADR-119 M3）：基于 Disruptor 4.0.0 环形缓冲区的无锁帧流水线。
/// <para>
/// 职责边界：本类只负责「帧的搬运与背压」，<strong>不做协议编解码、不做 IO</strong>——
/// 那些是 <see cref="IFrameHandler"/> 的事（落库 / MQTT / OTel / 告警）。
/// </para>
/// </summary>
public sealed class AcquisitionPipeline : IAsyncDisposable
{
    private readonly Disruptor<FrameSlot> _disruptor;
    private readonly PipelineOptions _options;
    private readonly ILogger? _logger;
    private readonly List<IFrameHandler[]> _stages = new();
    private RingBuffer<FrameSlot>? _ring;

    private long _published;
    private long _dropped;
    private long _failed;
    private bool _started;
    private bool _disposed;

    public AcquisitionPipeline(PipelineOptions options, ILogger? logger = null)
    {
        if (options.RingSize < 2 || (options.RingSize & (options.RingSize - 1)) != 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options), options.RingSize, "RingSize 必须是 2 的幂且 ≥ 2（Disruptor 硬性要求）");
        }

        _options = options;
        _logger = logger;
        _disruptor = new Disruptor<FrameSlot>(static () => new FrameSlot(), options.RingSize);
    }

    /// <summary>配置值（只读）。</summary>
    public PipelineOptions Options => _options;

    /// <summary>管道是否已启动。</summary>
    public bool IsStarted => _started;

    /// <summary>已成功发布帧数。</summary>
    public long Published => Interlocked.Read(ref _published);

    /// <summary>因缓冲区满而被丢弃的帧数（背压证据）。</summary>
    public long Dropped => Interlocked.Read(ref _dropped);

    /// <summary>发布失败数（取序号失败等异常路径）。</summary>
    public long Failed => Interlocked.Read(ref _failed);

    /// <summary>环形缓冲区剩余容量；未启动时为 0。</summary>
    public long RemainingCapacity => _ring?.GetRemainingCapacity() ?? 0;

    /// <summary>
    /// 追加一个处理阶段。同一阶段内的多个处理器<strong>并行</strong>执行（每个都收到全部帧）；
    /// 阶段之间按顺序串联（前一阶段全部完成才进入下一阶段）。
    /// </summary>
    /// <remarks>需在 <see cref="Start"/> 之前调用。末端会自动追加槽位清理处理器。</remarks>
    public AcquisitionPipeline AddStage(params IFrameHandler[] handlers)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        if (handlers.Length == 0)
        {
            throw new ArgumentException("阶段至少需要一个处理器", nameof(handlers));
        }

        if (_started)
        {
            throw new InvalidOperationException("管道已启动，不能再追加阶段");
        }

        _stages.Add(handlers);
        return this;
    }

    /// <summary>启动管道（启动消费者线程）。幂等。</summary>
    public RingBuffer<FrameSlot> Start()
    {
        if (_started)
        {
            return _ring!;
        }

        EventHandlerGroup<FrameSlot>? group = null;
        foreach (var stage in _stages)
        {
            var wrapped = Array.ConvertAll(stage, h => (IEventHandler<FrameSlot>)new SlotHandler(h, _logger));
            group = group is null ? _disruptor.HandleEventsWith(wrapped) : group.Then(wrapped);
        }

        // 末端追加槽位清理：释放对帧的引用，避免大环形缓冲区滞留历史帧
        if (group is not null)
        {
            group.Then(SlotResetHandler.Instance);
        }

        _ring = _disruptor.Start();
        _started = true;
        _logger?.LogInformation(
            "[pipeline] 已启动：ringSize={RingSize}, stages={Stages}, dropOnFull={DropOnFull}",
            _options.RingSize, _stages.Count, _options.DropOnFull);
        return _ring;
    }

    /// <summary>
    /// 非阻塞发布：缓冲区满时按 <see cref="PipelineOptions.DropOnFull"/> 丢弃并计数。
    /// <para>采集侧应优先用这个——采集循环被阻塞会导致所有设备一起超时。</para>
    /// </summary>
    /// <returns>是否成功入队。</returns>
    public bool TryPublish(DeviceFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        var ring = EnsureStarted();

        if (!ring.TryNext(out long sequence))
        {
            Interlocked.Increment(ref _dropped);
            return false;
        }

        WriteAndPublish(ring, sequence, frame);
        return true;
    }

    /// <summary>
    /// 阻塞式发布：缓冲区满时等待空位。仅在可以承受调用方被阻塞的场景使用
    /// （例如批量补采），采集热路径请用 <see cref="TryPublish"/>。
    /// </summary>
    public void Publish(DeviceFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        var ring = EnsureStarted();
        var sequence = ring.Next();
        WriteAndPublish(ring, sequence, frame);
    }

    private void WriteAndPublish(RingBuffer<FrameSlot> ring, long sequence, DeviceFrame frame)
    {
        try
        {
            var slot = ring[sequence];
            slot.Load(frame, sequence, DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            // 极少路径：槽位写入失败也必须 Publish，否则序号空洞会让整个 RingBuffer 停摆
            Interlocked.Increment(ref _failed);
            _logger?.LogError(ex, "[pipeline] 槽位写入失败 seq={Seq}", sequence);
            ring.Publish(sequence);
            throw;
        }

        ring.Publish(sequence);
        Interlocked.Increment(ref _published);
    }

    private RingBuffer<FrameSlot> EnsureStarted()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _ring ?? Start();
    }

    /// <summary>
    /// 优雅关闭：停止接收新帧并等待在飞事件处理完毕（上限 <see cref="PipelineOptions.ShutdownTimeoutMs"/>）。
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (!_started)
        {
            return;
        }

        var sw = Stopwatch.StartNew();
        try
        {
            // Shutdown 会先排空已发布事件再停止消费者线程；超时则强制停止（避免优雅关闭卡死进程）。
            _disruptor.Shutdown(TimeSpan.FromMilliseconds(_options.ShutdownTimeoutMs));
        }
        catch (TimeoutException)
        {
            _logger?.LogWarning("[pipeline] 关闭超时：{Ms}ms 内未排空在飞事件", _options.ShutdownTimeoutMs);
        }
        finally
        {
            _logger?.LogInformation(
                "[pipeline] 已关闭：published={Published}, dropped={Dropped}, failed={Failed}, 耗时 {Ms}ms",
                Published, Dropped, Failed, sw.ElapsedMilliseconds);
        }

        await ValueTask.CompletedTask.ConfigureAwait(false);
    }
}
