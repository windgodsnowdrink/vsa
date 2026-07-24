#:sdk Microsoft.NET.Sdk.Web
#:package System.Memory@8.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Threading.Tasks.Dataflow;

public class TieredMemoryProcessor
{
    private readonly MemoryPool<byte> _hotMemory;
    private readonly MemoryPool<byte> _coldMemory;
    private readonly TransformBlock<TaskItem, TaskResult> _hotProcessor;
    private readonly TransformBlock<TaskItem, TaskResult> _coldProcessor;
    private readonly ActionBlock<TaskResult> _resultHandler;

    public TieredMemoryProcessor()
    {
        // 热数据内存池(高性能)
        _hotMemory = MemoryPool<byte>.Shared;
        
        // 冷数据内存池(大容量)
        _coldMemory = new NativeMemoryPool(1024 * 1024 * 256); // 256MB

        var hotOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        var coldOptions = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = 1,
            EnsureOrdered = true
        };

        _hotProcessor = new TransformBlock<TaskItem, TaskResult>(item =>
        {
            using var memory = _hotMemory.Rent(1024);
            var span = memory.Memory.Span;
            var bytesWritten = MemoryPackSerializer.Serialize(span, item);
            return new TaskResult(item, memory.Memory[..bytesWritten]);
        }, hotOptions);

        _coldProcessor = new TransformBlock<TaskItem, TaskResult>(item =>
        {
            using var memory = _coldMemory.Rent(1024 * 1024); // 1MB
            var span = memory.Memory.Span;
            var bytesWritten = MemoryPackSerializer.Serialize(span, item);
            return new TaskResult(item, memory.Memory[..bytesWritten]);
        }, coldOptions);

        _resultHandler = new ActionBlock<TaskResult>(result =>
        {
            // 结果处理逻辑
        });

        // 条件路由
        _hotProcessor.LinkTo(_resultHandler);
        _coldProcessor.LinkTo(_resultHandler);
    }
}

public class NativeMemoryPool : MemoryPool<byte>
{
    // 原生内存池实现...
}