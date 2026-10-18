using Plc.Plugins.Contracts;

namespace Plc.Host.Pipeline;

/// <summary>
/// Disruptor 环形缓冲区的<strong>槽位</strong>对象：预分配、可变、逐帧复用。
/// <para>
/// <strong>为什么不让 <see cref="DeviceFrame"/> 直接进 RingBuffer</strong>：
/// ADR-119 决策 ③ 要求 <c>DeviceFrame</c> <strong>不可变</strong>（多个 <c>IEventHandler</c>
/// 并发共享同一帧实例时无需加锁），而 Disruptor 低分配率的前提是槽位对象可<strong>原地改写</strong>。
/// 二者用一个可变持有者解耦：
/// </para>
/// <list type="bullet">
/// <item>槽位数组在启动时一次性预分配 → 发布路径<strong>不分配槽位</strong>；</item>
/// <item>帧本身仍是不可变快照 → 处理器之间共享安全；</item>
/// <item>载荷 <c>ReadOnlyMemory&lt;byte&gt;</c> 只传引用，<strong>全程不拷贝字节</strong>。</item>
/// </list>
/// <para>
/// 代价：每帧仍会分配一个 <c>DeviceFrame</c> 记录（不可变性的必然成本）。
/// 相比它承载的 <c>byte[]</c> 载荷（协议层已分配），这个开销可以忽略。
/// </para>
/// </summary>
public sealed class FrameSlot
{
    /// <summary>当前帧；<c>null</c> 表示槽位空闲（尚未载入或已被末端处理器清空）。</summary>
    public DeviceFrame? Frame { get; private set; }

    /// <summary>载入时的环形缓冲区序号。</summary>
    public long Sequence { get; private set; }

    /// <summary>入队时刻（UTC），用于计算端到端处理延迟。</summary>
    public DateTimeOffset EnqueuedAt { get; private set; }

    /// <summary>把一帧载入槽位（原地改写，不分配新槽位）。</summary>
    internal void Load(DeviceFrame frame, long sequence, DateTimeOffset enqueuedAt)
    {
        Frame = frame;
        Sequence = sequence;
        EnqueuedAt = enqueuedAt;
    }

    /// <summary>
    /// 清空对帧的引用。由管道<strong>末端</strong>处理器调用——
    /// 不清空的话，槽位会一直持有旧帧，直到下次发布覆盖，环形缓冲区越大滞留内存越多。
    /// </summary>
    internal void Reset()
    {
        Frame = null;
        Sequence = -1;
        EnqueuedAt = default;
    }

    /// <summary>从入队到此刻的延迟；空闲槽位返回 null。</summary>
    public TimeSpan? Latency(DateTimeOffset now) =>
        EnqueuedAt == default ? null : now - EnqueuedAt;
}
