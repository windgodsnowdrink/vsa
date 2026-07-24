#:sdk Microsoft.NET.Sdk.Web
#:package Resonance@6.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Tasks.Dataflow;
using Resonance;

public class DataflowMessagePipeline : IAsyncDisposable
{
    private readonly TransformBlock<ResonanceMessage, ResonanceMessage> _processor;
    private readonly ActionBlock<ResonanceMessage> _consumer;
    private readonly BufferBlock<ResonanceMessage> _buffer;
    private readonly CancellationTokenSource _cts = new();

    public DataflowMessagePipeline()
    {
        _buffer = new BufferBlock<ResonanceMessage>(new DataflowBlockOptions
        {
            BoundedCapacity = 10000,
            CancellationToken = _cts.Token
        });

        _processor = new TransformBlock<ResonanceMessage, ResonanceMessage>(msg =>
        {
            // 消息处理逻辑
            return ProcessMessage(msg);
        }, new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 1000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = _cts.Token
        });

        _consumer = new ActionBlock<ResonanceMessage>(msg =>
        {
            // 消费处理逻辑
            ConsumeMessage(msg);
        }, new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 1000,
            MaxDegreeOfParallelism = 1,
            CancellationToken = _cts.Token
        });

        _buffer.LinkTo(_processor);
        _processor.LinkTo(_consumer);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        await Task.WhenAll(
            _buffer.Completion,
            _processor.Completion,
            _consumer.Completion);
    }
}