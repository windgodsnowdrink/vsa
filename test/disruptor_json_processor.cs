#:sdk Microsoft.NET.Sdk.Web
#:package System.Text.Json@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Threading.Channels;
using System.Text.Json;

public class DisruptorJsonProcessor : IAsyncDisposable
{
    private readonly Channel<JsonDocument> _ringBuffer;
    private readonly TransformBlock<JsonDocument, byte[]>[] _workers;
    private readonly ActionBlock<byte[]> _resultHandler;
    private readonly ObjectPool<Memory<byte>> _memoryPool;

    public DisruptorJsonProcessor()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);

        _ringBuffer = Channel.CreateBounded<JsonDocument>(new BoundedChannelOptions(1024)
        {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        _workers = new TransformBlock<JsonDocument, byte[]>[Environment.ProcessorCount];
        for (int i = 0; i < _workers.Length; i++)
        {
            _workers[i] = new TransformBlock<JsonDocument, byte[]>(doc =>
            {
                using var memory = _memoryPool.Get();
                var span = memory.Span;
                var writer = new Utf8JsonWriter(span);
                doc.WriteTo(writer);
                return span[..writer.BytesWritten].ToArray();
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 256,
                MaxDegreeOfParallelism = 1,
                EnsureOrdered = false
            });
        }

        _resultHandler = new ActionBlock<byte[]>(bytes =>
        {
            // 结果处理逻辑
        });

        foreach (var worker in _workers)
        {
            _ringBuffer.Reader.AsObservable().Subscribe(doc => worker.Post(doc));
            worker.LinkTo(_resultHandler);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _ringBuffer.Writer.Complete();
        await Task.WhenAll(_workers.Select(w => w.Completion));
    }
}