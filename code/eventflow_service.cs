#:sdk Microsoft.NET.Sdk.Web
#:package EventFlow@1.0.0
#:package LiteDB@5.0.17
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using EventFlow;
using EventFlow.Aggregates;
using EventFlow.Configuration;
using EventFlow.Extensions;
using Microsoft.Extensions.ObjectPool;
using LiteDB;
using System.Buffers;
using MemoryPack;

// 1. 定义聚合根和事件
[MemoryPackable]
public partial class OrderAggregate : AggregateRoot<OrderAggregate, OrderId>
{
    public string Status { get; private set; }
    public decimal Amount { get; private set; }

    public OrderAggregate(OrderId id) : base(id) { }

    public void Create(string status)
    {
        Emit(new OrderCreatedEvent(status));
    }

    public void Apply(OrderCreatedEvent e)
    {
        Status = e.Status;
    }
}

[MemoryPackable]
public sealed class OrderCreatedEvent : AggregateEvent<OrderAggregate, OrderId>
{
    public string Status { get; }
    public decimal Amount { get; }
    public OrderCreatedEvent(string status, decimal amount)
    {
        Status = status;
        Amount = amount;
    }
}

// 2. 高性能事件处理器(Disruptor模式)
[SkipLocalsInit]
public sealed class EventFlowProcessor : BackgroundService
{
    private readonly Channel<IDomainEvent> _eventChannel;
    private readonly ObjectPool<EventContext> _contextPool;
    private readonly IRootResolver _resolver;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly ILiteDatabase _db;

    public EventFlowProcessor(IRootResolver resolver, ILiteDatabase db)
    {
        _resolver = resolver;
        _db = db;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _eventChannel = Channel.CreateBounded<IDomainEvent>(new BoundedChannelOptions(10000)
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
    public async Task PublishEventAsync(IDomainEvent domainEvent)
    {
        await _eventChannel.Writer.WriteAsync(domainEvent);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var collection = _db.GetCollection<BsonDocument>("events");
        
        await foreach (var domainEvent in _eventChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                _latencyOptimizer.Optimize(() => 
                {
                    // 持久化到LiteDB
                    var doc = new BsonDocument
                    {
                        ["_id"] = ObjectId.NewObjectId(),
                        ["AggregateId"] = domainEvent.AggregateIdentity.Value,
                        ["EventType"] = domainEvent.EventType.Name,
                        ["Timestamp"] = DateTime.UtcNow,
                        ["Data"] = BsonMapper.Global.ToDocument(domainEvent)
                    };
                    collection.Insert(doc);
                    
                    // 处理事件
                    context.Process(domainEvent, _resolver);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置EventFlow
var resolver = EventFlowOptions.New
    // .ConfigureMsSql(MsSqlConfiguration.New
    //     .SetConnectionString("Server=localhost;Database=EventFlowDemo;User Id=sa;Password=your_password;"))
    .Configure(c => c.RegisterServices(sr => 
        sr.Register<ILiteDatabase>(_ => new LiteDatabase("eventflow.db"), Lifetime.Singleton)))
    .AddAggregates(typeof(OrderAggregate))
    .CreateResolver();

builder.Services.AddSingleton(resolver);
builder.Services.AddSingleton<ILiteDatabase>(resolver.Resolve<ILiteDatabase>());
builder.Services.AddHostedService<EventFlowProcessor>();

var app = builder.Build();

// 命令端点
app.MapPost("/orders", async (OrderCommand command, EventFlowProcessor processor) =>
{
    var orderId = OrderId.New;
    var aggregate = await resolver.Resolve<IAggregateStore>()
        .LoadAsync<OrderAggregate, OrderId>(orderId, CancellationToken.None);
    
    aggregate.Create(command.Status);
    await processor.PublishEventAsync(aggregate.UncommittedEvents.Single());
    
    return Results.Ok(orderId);
});

app.Run();

// 4. 辅助类
[MemoryPackable]
public partial record OrderCommand(string Status);
public record OrderId(string Value) : Identity<OrderId>(Value);
public class EventContext
{
    public void Process(IDomainEvent domainEvent, IRootResolver resolver)
    {
        var eventPublisher = resolver.Resolve<IEventPublisher>();
        eventPublisher.PublishAsync(domainEvent, CancellationToken.None).Wait();
    }
}

public class EventContextPooledPolicy : IPooledObjectPolicy<EventContext>
{
    public EventContext Create() => new EventContext();
    public bool Return(EventContext obj) => true;
}