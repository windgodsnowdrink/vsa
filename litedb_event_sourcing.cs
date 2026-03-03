#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:package MemoryPack@1.9.0
#:package Disruptor-net@3.4.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LiteDB;
using MemoryPack;
using Disruptor;
using Disruptor.Dsl;

// 1. 高性能聚合根基类
[MemoryPackable]
public abstract class AggregateRoot
{
    [BsonId] public ObjectId Id { get; protected set; }
    public long Version { get; protected set; }
    
    private readonly List<EventData> _pendingEvents = new();
    
    public IReadOnlyCollection<EventData> GetPendingEvents() => _pendingEvents.AsReadOnly();
    public void ClearPendingEvents() => _pendingEvents.Clear();
    
    protected void ApplyEvent(EventData @event)
    {
        Version++;
        _pendingEvents.Add(@event);
        // 动态分发到具体的Apply方法
        ((dynamic)this).Apply((dynamic)@event);
    }
}

// 2. 基于Disruptor的事件存储引擎
public class EventStoreEngine : IAsyncDisposable
{
    private readonly Disruptor<EventData> _disruptor;
    private readonly RingBuffer<EventData> _ringBuffer;
    private readonly ILiteDatabase _db;
    private readonly ObjectPool<EventData> _eventPool;
    
    public EventStoreEngine(ILiteDatabase db)
    {
        _db = db;
        _db.Pragma("WAL", true);
        
        _eventPool = new DefaultObjectPool<EventData>(
            new EventDataPoolPolicy(), 
            Environment.ProcessorCount * 2);
            
        _disruptor = new Disruptor<EventData>(() => _eventPool.Get(), 1024 * 1024, TaskScheduler.Default);
        _disruptor.HandleEventsWith(new EventPersistHandler(_db));
        _ringBuffer = _disruptor.Start();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(EventData @event)
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
        await Task.Delay(100); // 等待处理完成
        _db.Dispose();
    }
}

// 3. 事件持久化处理器
public class EventPersistHandler : IEventHandler<EventData>
{
    private readonly ILiteDatabase _db;
    
    public EventPersistHandler(ILiteDatabase db) => _db = db;
    
    public void OnEvent(EventData @event, long sequence, bool endOfBatch)
    {
        var collection = _db.GetCollection<EventData>("events");
        collection.Insert(@event);
        
        // 更新聚合根版本
        var aggregateCollection = _db.GetCollection<AggregateRoot>("aggregates");
        aggregateCollection.Update(@event.AggregateId, 
            new BsonDocument { ["$set"] = new BsonDocument("Version", @event.Version) });
    }
}

// 4. 使用示例
public class Order : AggregateRoot
{
    public string OrderNumber { get; private set; }
    public decimal TotalAmount { get; private set; }
    
    public void Create(string orderNumber, decimal amount)
    {
        ApplyEvent(new OrderCreatedEvent
        {
            AggregateId = Id,
            OrderNumber = orderNumber,
            Amount = amount
        });
    }
    
    // 事件处理方法
    private void Apply(OrderCreatedEvent @event)
    {
        OrderNumber = @event.OrderNumber;
        TotalAmount = @event.Amount;
    }
}

[MemoryPackable]
public partial class OrderCreatedEvent : EventData
{
    public string OrderNumber { get; set; }
    public decimal Amount { get; set; }
}