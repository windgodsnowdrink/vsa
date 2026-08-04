#:sdk Microsoft.NET.Sdk
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Persistence.AdoNet@8.0.0
#:package Microsoft.Orleans.Streaming.EventStore@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Diagnostics.DiagnosticSource@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams;

// 事件定义
[GenerateSerializer]
public abstract record EventBase([property: Id(0)] Guid Id, [property: Id(1)] DateTimeOffset Timestamp);

[GenerateSerializer]
public record OrderCreatedEvent(
    Guid OrderId, 
    string CustomerId, 
    decimal Amount) : EventBase(OrderId, DateTimeOffset.UtcNow);

[GenerateSerializer]
public record OrderPaidEvent(
    Guid OrderId, 
    string TransactionId) : EventBase(OrderId, DateTimeOffset.UtcNow);

[GenerateSerializer]
public record OrderShippedEvent(
    Guid OrderId, 
    string TrackingNumber) : EventBase(OrderId, DateTimeOffset.UtcNow);

// 聚合根状态
[GenerateSerializer]
public class OrderState
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public string CustomerId { get; set; }
    [Id(2)] public decimal Amount { get; set; }
    [Id(3)] public string Status { get; set; } = "Created";
    [Id(4)] public string? TransactionId { get; set; }
    [Id(5)] public string? TrackingNumber { get; set; }
    [Id(6)] public int Version { get; set; }
}

// 聚合根Grain接口
public interface IOrderGrain : IGrainWithGuidKey
{
    Task<OrderState> GetState();
    Task ProcessEvent(EventBase @event);
    Task SubscribeAsync(IAsyncObserver<EventBase> observer);
}

// 聚合根Grain实现
[StorageProvider(ProviderName = "EventStore")]
public class OrderGrain : Grain<OrderState>, IOrderGrain
{
    private readonly Meter _meter;
    private readonly Counter<int> _eventCounter;
    private IAsyncStream<EventBase> _stream;

    public OrderGrain(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("OrderGrain");
        _eventCounter = _meter.CreateCounter<int>("processed_events");
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("EventStoreStream");
        _stream = streamProvider.GetStream<EventBase>(this.GetPrimaryKey(), "OrderEvents");
        await base.OnActivateAsync(cancellationToken);
    }

    public Task<OrderState> GetState() => Task.FromResult(State);

    public async Task ProcessEvent(EventBase @event)
    {
        switch (@event)
        {
            case OrderCreatedEvent created:
                State = new OrderState
                {
                    Id = created.OrderId,
                    CustomerId = created.CustomerId,
                    Amount = created.Amount,
                    Status = "Created",
                    Version = State.Version + 1
                };
                break;
            case OrderPaidEvent paid:
                State.TransactionId = paid.TransactionId;
                State.Status = "Paid";
                State.Version++;
                break;
            case OrderShippedEvent shipped:
                State.TrackingNumber = shipped.TrackingNumber;
                State.Status = "Shipped";
                State.Version++;
                break;
        }

        _eventCounter.Add(1);
        await WriteStateAsync();
        await _stream.OnNextAsync(@event);
    }

    public Task SubscribeAsync(IAsyncObserver<EventBase> observer) => 
        _stream.SubscribeAsync(observer);
}

// 事件处理器Grain接口
public interface IOrderEventHandlerGrain : IGrainWithGuidKey
{
    Task HandleAsync(EventBase @event);
}

// 事件处理器Grain实现
public class OrderEventHandlerGrain : Grain, IOrderEventHandlerGrain
{
    private readonly ILogger<OrderEventHandlerGrain> _logger;

    public OrderEventHandlerGrain(ILogger<OrderEventHandlerGrain> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(EventBase @event)
    {
        _logger.LogInformation("Processing event {EventType} for order {OrderId}", 
            @event.GetType().Name, @event.Id);
        
        // 这里可以添加具体的业务逻辑处理
        return Task.CompletedTask;
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrleansEventSourcing(this IServiceCollection services, 
        Action<OrleansEventSourcingOptions> configure = null)
    {
        services.Configure(configure ?? (opts => {}));
        return services;
    }
}

// Orleans配置
public class OrleansEventSourcingOptions
{
    public string ClusterId { get; set; } = "event-sourcing-cluster";
    public string ServiceId { get; set; } = "OrderService";
    public string AdoNetConnectionString { get; set; } = "Server=.;Database=OrleansEventStore;Integrated Security=true;";
}

// 示例用法
public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseOrleans((context, builder) =>
            {
                var options = context.Configuration.Get<OrleansEventSourcingOptions>();
                
                builder.UseAdoNetClustering(options =>
                {
                    options.ConnectionString = options.AdoNetConnectionString;
                    options.Invariant = "System.Data.SqlClient";
                })
                .AddAdoNetGrainStorage("EventStore", opts =>
                {
                    opts.ConnectionString = options.AdoNetConnectionString;
                    opts.Invariant = "System.Data.SqlClient";
                })
                .AddEventStoreStreams("EventStoreStream", configurator =>
                {
                    configurator.ConfigureEventStore(ob => ob.Configure(options => 
                    {
                        options.ConnectionString = options.AdoNetConnectionString;
                    }));
                })
                .Configure<ClusterOptions>(opts =>
                {
                    opts.ClusterId = options.ClusterId;
                    opts.ServiceId = options.ServiceId;
                });
            })
            .ConfigureServices(services =>
            {
                services.AddOrleansEventSourcing(options =>
                {
                    options.ClusterId = "prod";
                    options.ServiceId = "OrderService";
                });
            })
            .Build();

        await host.StartAsync();
        
        var client = host.Services.GetRequiredService<IClusterClient>();
        var orderId = Guid.NewGuid();
        var orderGrain = client.GetGrain<IOrderGrain>(orderId);
        
        // 处理订单创建事件
        await orderGrain.ProcessEvent(new OrderCreatedEvent(orderId, "customer-123", 100.50m));
        
        // 处理订单支付事件
        await orderGrain.ProcessEvent(new OrderPaidEvent(orderId, "txn-456"));
        
        // 处理订单发货事件
        await orderGrain.ProcessEvent(new OrderShippedEvent(orderId, "track-789"));

        // 获取最终状态
        var state = await orderGrain.GetState();
        Console.WriteLine($"Order {state.Id} status: {state.Status}");

        await host.WaitForShutdownAsync();
    }
}