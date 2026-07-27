#:sdk Microsoft.NET.Sdk.Web
#:package MassTransit@8.2.2
#:package MassTransit.RabbitMQ@8.2.2
#:package StackExchange.Redis@2.7.121
#:package TieredMemoryService:2.0.0
#:package MemoryPoolingExtensions:1.5.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using MassTransit;
using StackExchange.Redis;
using System.Threading.Channels;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置Redis分布式缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MassTransit_";
});

// 2. 高性能通道配置 (RingBuffer + Disruptor模式)
var messageChannel = Channel.CreateBounded<MessageEnvelope>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 3. 零拷贝消息处理器
builder.Services.AddSingleton<IMessageProcessor>(sp => 
    new ChannelMessageProcessor(
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));
builder.Services.AddHostedService<EventChannelService>();

// 4. 配置MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // 零拷贝优化
        cfg.UseRawJsonSerializer();
        
        // 高性能配置
        cfg.PrefetchCount = (ushort)(Environment.ProcessorCount * 2);
        
        // 分片策略
        cfg.UseConsistentHashExchange();
        
        // 消息持久化
        cfg.PublishTopology.ConfigureMessageTopology<MessageEnvelope>(topology =>
        {
            topology.Durable = true;
            topology.AutoDelete = false;
        });

        cfg.UseDelayedRedelivery(r => r.Intervals(
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(5)));
    });
    
    // 对象池配置
    x.SetBusFactory(new PooledBusFactory(
        Environment.ProcessorCount * 2));
});

var app = builder.Build();
app.MapGet("/", () => "MassTransit Integration Ready");
app.Run();

// 高性能事件通道服务实现
// 在builder.Services.AddMassTransit(x => 之前添加分层内存服务
builder.Services.AddTieredMemoryService(options =>
{
    options.Tier1Size = 1024 * 1024 * 100; // 100MB 高速内存
    options.Tier2Size = 1024 * 1024 * 500; // 500MB 中速内存
    options.Tier3Size = 1024 * 1024 * 1024; // 1GB 低速内存
    options.PromotionThreshold = 0.8; // 80%使用率时升级
    options.DemotionThreshold = 0.2; // 20%使用率时降级
});
{
    private readonly Channel<MessageEnvelope> _channel;
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer;
    private readonly ITieredMemoryService _memoryService; // 新增
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public EventChannelService(
        Channel<MessageEnvelope> channel,
        ThreadLocal<Span<byte>> threadLocalBuffer,
        ITieredMemoryService memoryService) // 新增参数
    {
        _channel = channel;
        _threadLocalBuffer = threadLocalBuffer;
        _memoryService = memoryService; // 新增
        _latencyOptimizer = new TailLatencyOptimizer();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            _latencyOptimizer.Optimize(() =>
            {
                var buffer = _threadLocalBuffer.Value;
                // 使用分层内存服务分配内存
                using var memoryHandle = _memoryService.AllocateTiered(message.Payload.Length);
                ProcessMessage(message, buffer, memoryHandle.Memory);
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void ProcessMessage(MessageEnvelope message, Span<byte> buffer, Memory<byte> tieredMemory)
    {
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // 零拷贝消息处理逻辑
                var memory = _memoryPool.Get();
                try
                {
                    // 高性能序列化/反序列化
                }
                finally
                {
                    _memoryPool.Return(memory);
                }
            }
        }
    }
}

// 消息信封结构
[MemoryPackable]
public partial struct MessageEnvelope
{
    public Guid MessageId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public ReadOnlyMemory<byte> Payload { get; set; }
}

// 尾延迟优化器
public class TailLatencyOptimizer
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Optimize(Action action)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            action();
        }
        finally
        {
            sw.Stop();
            // 延迟监控逻辑
        }
    }
}

// 高性能消息处理器
[SkipLocalsInit]
public class ChannelMessageProcessor : IMessageProcessor
{
    private readonly ChannelWriter<MessageEnvelope> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;

    public ChannelMessageProcessor(
        Channel<MessageEnvelope> channel,
        ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessAsync(MessageEnvelope message)
    {
        // 使用零拷贝缓冲区处理消息
        Span<byte> buffer = _buffer.Value;
        // ... 消息处理逻辑 ...
        
        await _writer.WriteAsync(message);
    }
}

// 消息信封
public record MessageEnvelope(
    Guid MessageId,
    string MessageType,
    ReadOnlyMemory<byte> Payload,
    DateTimeOffset Timestamp);