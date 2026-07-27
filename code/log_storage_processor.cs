#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Tasks.Dataflow@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Tasks.Dataflow;

public class LogStorageProcessor
{
    private readonly TransformBlock<LogEntry, LogEntry> _filterBlock;
    private readonly BatchBlock<LogEntry> _batchBlock;
    private readonly ActionBlock<LogEntry[]> _storageBlock;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public LogStorageProcessor()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);

        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = 10000,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            EnsureOrdered = false
        };

        _filterBlock = new TransformBlock<LogEntry, LogEntry>(log =>
        {
            // 日志过滤逻辑
            return log;
        }, options);

        _batchBlock = new BatchBlock<LogEntry>(100);
        _storageBlock = new ActionBlock<LogEntry[]>(async logs =>
        {
            using var memory = _memoryPool.Get();
            var span = memory.Span;
            var bytesWritten = MemoryPackSerializer.Serialize(span, logs);
            // 存储逻辑
        }, options);

        _filterBlock.LinkTo(_batchBlock);
        _batchBlock.LinkTo(_storageBlock);
    }
}

public record LogEntry(string Message, DateTime Timestamp);