#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:package MemoryPack@1.9.0
#:package Microsoft.Extensions.Options@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LiteDB;
using MemoryPack;
using Microsoft.Extensions.Options;

// 1. 可配置的事件存储选项
public sealed class EventStoreOptions
{
    public string ConnectionString { get; set; } = "Filename=events.db;Connection=shared";
    public int ChannelCapacity { get; set; } = 10000;
    public int BufferPoolSize { get; set; } = Environment.ProcessorCount * 2;
    public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(30);
}

// 2. 模块化事件存储服务
public sealed class EventStoreService : IAsyncDisposable
{
    private readonly ILiteDatabase _db;
    private readonly Channel<EventData> _eventChannel;
    private readonly ObjectPool<EventData> _eventPool;
    private readonly Task _processingTask;
    private readonly CancellationTokenSource _cts = new();
    private readonly EventStoreOptions _options;

    public EventStoreService(IOptions<EventStoreOptions> options)
    {
        _options = options.Value;
        _db = new LiteDatabase(_options.ConnectionString);
        
        // 高性能通道配置
        _eventChannel = Channel.CreateBounded<EventData>(new BoundedChannelOptions(_options.ChannelCapacity)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        // 对象池配置
        _eventPool = new DefaultObjectPool<EventData>(
            new EventDataPooledPolicy(), 
            _options.BufferPoolSize);

        _processingTask = Task.Run(ProcessEventsAsync);
    }

    // 3. 支持自定义事件类型的写入接口
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ValueTask AppendAsync<T>(T payload, EventType type) where T : IMemoryPackable<T>
    {
        var eventData = _eventPool.Get();
        try
        {
            eventData.Id = ObjectId.NewObjectId();
            eventData.Timestamp = DateTime.UtcNow.Ticks;
            eventData.Type = type;
            
            // 零拷贝序列化
            var serializedSize = MemoryPackSerializer.Serialize(payload, eventData.Payload);
            eventData.PayloadSize = serializedSize;
            
            return _eventChannel.Writer.WriteAsync(eventData, _cts.Token);
        }
        catch
        {
            _eventPool.Return(eventData);
            throw;
        }
    }

    // 4. 事件处理核心逻辑
    private async Task ProcessEventsAsync()
    {
        var collection = _db.GetCollection<EventData>("events");
        collection.EnsureIndex(x => x.Timestamp);
        collection.EnsureIndex(x => x.Type);

        await foreach (var eventData in _eventChannel.Reader.ReadAllAsync(_cts.Token))
        {
            try
            {
                collection.Insert(eventData);
            }
            finally
            {
                _eventPool.Return(eventData);
            }
        }
    }

    // 5. 对象池策略
    private sealed class EventDataPooledPolicy : PooledObjectPolicy<EventData>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override EventData Create() => new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Return(EventData obj)
        {
            obj.Id = default;
            obj.Timestamp = default;
            obj.Type = default;
            obj.PayloadSize = default;
            return true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _eventChannel.Writer.Complete();
        await _processingTask;
        _db.Dispose();
    }
}

// 6. 使用示例
public static class EventStoreDemo
{
    public static async Task RunAsync()
    {
        // 配置选项
        var options = new EventStoreOptions
        {
            ConnectionString = "Filename=custom_events.db",
            ChannelCapacity = 20000
        };

        // 创建服务
        var service = new EventStoreService(Options.Create(options));
        
        // 自定义事件类型
        [MemoryPackable]
        public partial record UserCreatedEvent(string UserId, string UserName);
        
        // 写入事件
        await service.AppendAsync(new UserCreatedEvent("123", "test"), EventType.UserCreated);
    }
}
#:package EventStore.Client@22.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using EventStore.Client;
using Microsoft.Extensions.ObjectPool;

// 1. 高性能事件处理器(Disruptor模式)
[SkipLocalsInit]
public sealed class EventStoreProcessor : BackgroundService
{
    private readonly Channel<EventData> _eventChannel;
    private readonly ObjectPool<EventContext> _contextPool;
    private readonly EventStoreClient _eventStore;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public EventStoreProcessor(EventStoreClient eventStore)
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
    public async Task PublishEventAsync(EventData eventData)
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

// 2. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置EventStore
builder.Services.AddSingleton(sp => 
    new EventStoreClient(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false")));

// 注册事件处理器
builder.Services.AddHostedService<EventStoreProcessor>();

var app = builder.Build();

// 事件发布端点
app.MapPost("/events", async (EventData eventData, EventStoreProcessor processor) =>
{
    await processor.PublishEventAsync(eventData);
    return Results.Ok();
});

app.Run();

// 3. 辅助类
public record EventData(string StreamName, string EventType, byte[] Data);
public class EventContext
{
    public void Process(EventData eventData, EventStoreClient eventStore)
    {
        var eventRecord = new EventData(
            Uuid.NewUuid(),
            eventData.EventType,
            eventData.Data);
        
        eventStore.AppendToStreamAsync(
            eventData.StreamName,
            StreamState.Any,
            new[] { eventRecord });
    }
}
public class EventContextPooledPolicy : IPooledObjectPolicy<EventContext>
{
    public EventContext Create() => new EventContext();
    public bool Return(EventContext obj) => true;
}