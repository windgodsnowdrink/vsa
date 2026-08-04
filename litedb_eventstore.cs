#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:package Disruptor-net@3.4.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using Disruptor;
using Disruptor.Dsl;
using LiteDB;

// 1. 定义箱外事件模型
[MemoryPackable]
public partial class OutOfBandEvent
{
    [BsonId]
    public ObjectId Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; }
    public byte[] Payload { get; set; }
    public string Source { get; set; }
    public string CorrelationId { get; set; }
}

// 2. 基于Disruptor的高性能事件处理器
public class EventProcessor : IEventHandler<OutOfBandEvent>
{
    private readonly ILiteDatabase _db;
    private readonly ObjectPool<byte[]> _bufferPool;

    public EventProcessor(ILiteDatabase db)
    {
        _db = db;
        _bufferPool = new DefaultObjectPool<byte[]>(
            new BufferPoolPolicy(), 
            Environment.ProcessorCount * 2);
    }

    public void OnEvent(OutOfBandEvent data, long sequence, bool endOfBatch)
    {
        var buffer = _bufferPool.Get();
        try
        {
            // 零拷贝处理
            var span = buffer.AsSpan();
            // 事件处理逻辑...
            
            // 持久化到LiteDB
            var collection = _db.GetCollection<OutOfBandEvent>("events");
            collection.Insert(data);
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    private class BufferPoolPolicy : IPooledObjectPolicy<byte[]>
    {
        public byte[] Create() => new byte[4096];
        public bool Return(byte[] obj) => true;
    }
}

// 3. 事件存储服务
public class EventStoreService : IAsyncDisposable
{
    private readonly Disruptor<OutOfBandEvent> _disruptor;
    private readonly RingBuffer<OutOfBandEvent> _ringBuffer;
    private readonly ILiteDatabase _db;

    public EventStoreService(string connectionString, int bufferSize = 1024 * 1024)
    {
        _db = new LiteDatabase(connectionString);
        _db.Pragma("WAL", true);
        
        _disruptor = new Disruptor<OutOfBandEvent>(() => new OutOfBandEvent(), bufferSize);
        _disruptor.HandleEventsWith(new EventProcessor(_db));
        _ringBuffer = _disruptor.Start();
    }

    public void PublishEvent(OutOfBandEvent @event)
    {
        var sequence = _ringBuffer.Next();
        try
        {
            _ringBuffer[sequence] = @event;
        }
        finally
        {
            _ringBuffer.Publish(sequence);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disruptor.Halt();
        _db.Dispose();
    }
}

// 4. 使用示例
public static class EventStoreDemo
{
    public static async Task RunAsync()
    {
        var eventStore = new EventStoreService("Filename=events.db;Connection=shared");
        
        // 发布箱外事件
        eventStore.PublishEvent(new OutOfBandEvent
        {
            Timestamp = DateTime.UtcNow,
            EventType = "PaymentProcessed",
            Payload = MemoryPackSerializer.Serialize(new { Amount = 100.50m }),
            CorrelationId = Guid.NewGuid().ToString()
        });
        
        await eventStore.DisposeAsync();
    }
}