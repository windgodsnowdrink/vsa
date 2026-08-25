using Disruptor;
using Disruptor.Dsl;
using Microsoft.Extensions.Logging;

namespace PlcGateway;

/// <summary>
/// Disruptor 环形缓冲区流水线（需求 4）：采集线程 produce 原始帧，
/// 消费者线程做解析 / 落库 / 转发。日志与消息走同一 RingBuffer 分流。
/// </summary>
public sealed class PipelineHandler : IEventHandler<RawFrame>
{
    private readonly ILogger _log;
    public PipelineHandler(ILogger log) => _log = log;

    public void OnEvent(RawFrame data, long sequence, bool endOfBatch)
    {
        _log.LogInformation("[seq {Seq}] {Device} -> {Len} bytes {Note}",
            sequence, data.DeviceId, data.Payload.Length, data.Note ?? "");
    }
}

public sealed class FramePipeline : IAsyncDisposable
{
    private readonly Disruptor.Dsl.Disruptor<RawFrame> _disruptor;
    private readonly RingBuffer<RawFrame> _ring;

    public FramePipeline(int ringSize, int consumers, ILogger log)
    {
        _disruptor = new Disruptor.Dsl.Disruptor<RawFrame>(() => new RawFrame(), ringSize);
        int n = Math.Max(1, consumers);
        for (int i = 0; i < n; i++)
            _disruptor.HandleEventsWith(new PipelineHandler(log));
        _ring = _disruptor.Start();
    }

    /// <summary>采集侧 produce：把一 帧投入 RingBuffer。</summary>
    public void Publish(RawFrame frame)
    {
        long seq = _ring.Next();
        RawFrame slot = _ring[seq];   // RingBuffer 索引器按引用类型返回槽位对象，原地改写
        slot.CopyFrom(frame);
        _ring.Publish(seq);
    }

    public ValueTask DisposeAsync()
    {
        _disruptor.Shutdown();
        return ValueTask.CompletedTask;
    }
}
