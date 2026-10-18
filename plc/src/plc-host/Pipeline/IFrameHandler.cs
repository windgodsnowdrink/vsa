using Disruptor;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace Plc.Host.Pipeline;

/// <summary>
/// 管道处理器：消费一帧。与 Disruptor 的 <see cref="IEventHandler{T}"/> 解耦，
/// 让业务代码只看到 <see cref="DeviceFrame"/>，不必关心 <see cref="FrameSlot"/> 槽位机制。
/// </summary>
/// <remarks>
/// 处理器<strong>不应抛异常</strong>：抛出会被 <see cref="SlotHandler"/> 捕获并记录，
/// 以免单个 sink 故障停掉整条采集管道。处理器应尽量短小——耗时操作请再次投递到后台队列。
/// </remarks>
public interface IFrameHandler
{
    /// <param name="frame">当前帧（不可变，可安全跨处理器共享）。</param>
    /// <param name="sequence">环形缓冲区序号。</param>
    /// <param name="endOfBatch">是否为本批最后一个事件（可用于批量刷写）。</param>
    void OnFrame(DeviceFrame frame, long sequence, bool endOfBatch);
}

/// <summary>
/// 把 <see cref="IFrameHandler"/> 适配到 Disruptor 的 <see cref="IEventHandler{T}"/>。
/// </summary>
internal sealed class SlotHandler : IEventHandler<FrameSlot>
{
    private readonly IFrameHandler _inner;
    private readonly ILogger? _logger;
    private readonly string _name;

    public SlotHandler(IFrameHandler inner, ILogger? logger, string? name = null)
    {
        _inner = inner;
        _logger = logger;
        _name = name ?? inner.GetType().Name;
    }

    public void OnEvent(FrameSlot data, long sequence, bool endOfBatch)
    {
        var frame = data.Frame;
        if (frame is null)
        {
            return; // 槽位空闲（已被末端处理器清空）——不该发生，防御性跳过
        }

        try
        {
            _inner.OnFrame(frame, sequence, endOfBatch);
        }
        catch (Exception ex)
        {
            // 单个 sink 故障不得停掉整条采集管道：记录并继续。
            // Disruptor 的 EventProcessor 一旦因未处理异常退出，整个 RingBuffer 将停止消费。
            _logger?.LogError(ex, "[pipeline] 处理器 {Handler} 处理帧 seq={Seq} 失败，已跳过", _name, sequence);
        }
    }
}

/// <summary>
/// 管道末端处理器：清空槽位对帧的引用，避免大环形缓冲区滞留历史帧。
/// </summary>
internal sealed class SlotResetHandler : IEventHandler<FrameSlot>
{
    public static readonly SlotResetHandler Instance = new();

    private SlotResetHandler()
    {
    }

    public void OnEvent(FrameSlot data, long sequence, bool endOfBatch) => data.Reset();
}
