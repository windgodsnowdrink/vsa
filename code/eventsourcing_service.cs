#:sdk Microsoft.NET.Sdk.Web
#:package EventSourcing.NetCore@4.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package MemoryPack@1.9.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Buffers;
using System.Runtime.CompilerServices;
using EventSourcing.NetCore;
using MemoryPack;
using Microsoft.Extensions.ObjectPool;

// 1. 高性能事件存储引擎
[SkipLocalsInit]
public sealed class EventStoreEngine : BackgroundService
{
    private readonly Channel<EventData> _eventChannel;
    private readonly ObjectPool<EventContext> _contextPool;
    private readonly IEventStore _eventStore;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public EventStoreEngine(IEventStore eventStore)
    {
        _eventStore = eventStore;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _eventChannel = Channel.CreateBounded<EventData>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<EventContext>(
            new EventContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AppendEventAsync(EventData eventData)
    {
        await _eventChannel.Writer.WriteAsync(eventData);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var eventData in _eventChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                _latencyOptimizer.Optimize(() => 
                {
                    context.Process(eventData, _eventStore);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. 聚合根实现
[MemoryPackable]
public partial class OrderAggregate : AggregateRoot<OrderId>
{
    public string Status { get; private set; }

    public void CreateOrder(string status)
    {
        Apply(new OrderCreatedEvent(status));
    }

    private void Apply(OrderCreatedEvent @event)
    {
        Status = @event.Status;
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置EventSourcing
builder.Services.AddEventSourcing(opt =>
{
    opt.UseMemoryPackSerializer();
    opt.UseInMemoryStorage();
});

// 注册事件存储引擎
builder.Services.AddHostedService<EventStoreEngine>();

var app = builder.Build();

// 命令端点
app.MapPost("/orders", async (CreateOrderCommand cmd, EventStoreEngine engine) =>
{
    var order = new OrderAggregate();
    order.CreateOrder(cmd.Status);
    
    var eventData = new EventData(
        order.Id.ToString(),
        "OrderCreated",
        MemoryPackSerializer.Serialize(order));
    
    await engine.AppendEventAsync(eventData);
    return Results.Ok(order.Id);
});

app.Run();

// 4. 辅助类
[MemoryPackable]
public partial record OrderCreatedEvent(string Status);

public record CreateOrderCommand(string Status);
public record OrderId(Guid Value);

public class EventData
{
    public string StreamId { get; }
    public string EventType { get; }
    public byte[] Payload { get; }

    public EventData(string streamId, string eventType, byte[] payload)
    {
        StreamId = streamId;
        EventType = eventType;
        Payload = payload;
    }
}

public class EventContext
{
    public void Process(EventData eventData, IEventStore eventStore)
    {
        eventStore.AppendToStream(
            eventData.StreamId, 
            eventData.EventType, 
            eventData.Payload);
    }
}

public class EventContextPooledPolicy : IPooledObjectPolicy<EventContext>
{
    public EventContext Create() => new EventContext();
    public bool Return(EventContext obj) => true;
}