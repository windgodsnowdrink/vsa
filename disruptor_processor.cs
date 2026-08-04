#:sdk Microsoft.NET.Sdk.Web
#:package Disruptor-net@3.4.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Runtime.CompilerServices;
using Disruptor;
using System.Threading.Channels;

public class DisruptorProcessor
{
    private readonly Channel<TaskItem> _ringBuffer;
    private readonly TransformBlock<TaskItem, TaskResult>[] _workers;
    private readonly ActionBlock<TaskResult> _resultHandler;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public DisruptorProcessor()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);

        // RingBuffer实现
        _ringBuffer = Channel.CreateBounded<TaskItem>(new BoundedChannelOptions(1024)
        {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        _workers = new TransformBlock<TaskItem, TaskResult>[Environment.ProcessorCount];
        for (int i = 0; i < _workers.Length; i++)
        {
            _workers[i] = new TransformBlock<TaskItem, TaskResult>(async item =>
            {
                using var memory = _memoryPool.Get();
                var span = memory.Span;
                var bytesWritten = MemoryPackSerializer.Serialize(span, item);
                return new TaskResult(item, memory[..bytesWritten]);
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 256,
                MaxDegreeOfParallelism = 1,
                EnsureOrdered = false
            });
        }

        _resultHandler = new ActionBlock<TaskResult>(result =>
        {
            // 结果处理逻辑
        });

        // 建立处理管道
        foreach (var worker in _workers)
        {
            _ringBuffer.Reader.AsObservable().Subscribe(item => worker.Post(item));
            worker.LinkTo(_resultHandler);
        }
    }
}

public record TaskItem(string Id);
public record TaskResult(TaskItem Item, ReadOnlyMemory<byte> Payload);

// 高性能事件处理器
public sealed class LogEventProcessor : IEventHandler<LogEvent>
{
    private readonly ChannelWriter<LogEvent> _writer;
    
    public LogEventProcessor(ChannelWriter<LogEvent> writer) => _writer = writer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnEvent(LogEvent data, long sequence, bool endOfBatch)
    {
        _writer.TryWrite(data);  // 零拷贝写入Channel
    }
}

// Disruptor配置
var disruptor = new Disruptor.Dsl.Disruptor<LogEvent>(
    () => new LogEvent(), 
    ringBufferSize: 1024 * 1024,
    taskScheduler: TaskScheduler.Default,
    producerType: ProducerType.Multi,
    waitStrategy: new BlockingWaitStrategy());

// 构建处理管道
var channel = Channel.CreateUnbounded<LogEvent>();
disruptor.HandleEventsWith(new LogEventProcessor(channel.Writer));

// 启动Disruptor
_ = Task.Run(() => {
    var ringBuffer = disruptor.Start();
    while (true)
    {
        var seq = ringBuffer.Next();
        var evt = ringBuffer[seq];
        // ... 填充事件数据 ...
        ringBuffer.Publish(seq);
    }
});