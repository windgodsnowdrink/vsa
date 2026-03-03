#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package RabbitMQ.Client@6.7.0
#:package TieredMemoryService:2.0.0
#:package MemoryPoolingExtensions:1.5.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using MagicOnion;
using System.Threading.Channels;
using RabbitMQ.Client;
using System.Threading.Tasks.Dataflow;

var builder = WebApplication.CreateBuilder();

// 1. 集群连接工厂
builder.Services.AddSingleton<IConnectionFactory>(_ => 
    new ConnectionFactory {
        HostName = "cluster-node1",
        Port = 5672,
        UserName = "admin",
        Password = "P@ssw0rd",
        VirtualHost = "/",
        AutomaticRecoveryEnabled = true,
        TopologyRecoveryEnabled = true
    });

// 2. 消息确认通道
var ackChannel = Channel.CreateBounded<DeliveryTag>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 3. 死信队列处理器
builder.Services.AddSingleton<IDeadLetterProcessor>(sp => 
    new ChannelDeadLetterProcessor(
        Channel.CreateBounded<DeadLetterMessage>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[512])));

// 4. 数据流网络
builder.Services.AddSingleton<DataflowNetwork>(sp => 
    new DataflowNetwork(
        new TransformBlock<QueueMessage, QueueMessage>(msg => 
            // 消息转换逻辑
            msg),
        new ActionBlock<QueueMessage>(msg => 
            // 消息处理逻辑
            {})));

var app = builder.Build();
app.MapGet("/", () => "Enhanced Message Queue Ready");
app.Run();

// 消息确认处理器
[SkipLocalsInit]
public class ChannelAckProcessor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void ProcessAck(ulong deliveryTag)
    {
        Span<byte> buffer = stackalloc byte[64];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // SIMD优化处理确认消息
            }
        }
    }
}

// 死信队列处理器
// 在builder.Services.AddSingleton<DataflowNetwork>之前添加
builder.Services.AddTieredMemoryService(options =>
{
    options.Tier1Size = 1024 * 1024 * 50; // 50MB 高速内存
    options.Tier2Size = 1024 * 1024 * 200; // 200MB 中速内存
    options.Tier3Size = 1024 * 1024 * 500; // 500MB 低速内存
    options.PromotionThreshold = 0.85;
    options.DemotionThreshold = 0.15;
});

// 修改ChannelDeadLetterProcessor类
[SkipLocalsInit]
public class ChannelDeadLetterProcessor : IDeadLetterProcessor
{
    private readonly Channel<DeadLetterMessage> _channel;
    private readonly ThreadLocal<Span<byte>> _buffer;
    private readonly ITieredMemoryService _memoryService;

    public ChannelDeadLetterProcessor(
        Channel<DeadLetterMessage> channel,
        ThreadLocal<Span<byte>> buffer,
        ITieredMemoryService memoryService)
    {
        _channel = channel;
        _buffer = buffer;
        _memoryService = memoryService;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void ProcessDeadLetter(DeadLetterMessage message)
    {
        Span<byte> buffer = stackalloc byte[512];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // 使用分层内存
                using var memoryHandle = _memoryService.AllocateTiered(message.Body.Length);
                // SIMD优化处理死信
            }
        }
    }
}

// 数据流网络
public class DataflowNetwork
{
    private readonly TransformBlock<QueueMessage, QueueMessage> _transform;
    private readonly ActionBlock<QueueMessage> _action;
    
    public DataflowNetwork(
        TransformBlock<QueueMessage, QueueMessage> transform,
        ActionBlock<QueueMessage> action)
    {
        _transform = transform;
        _action = action;
        _transform.LinkTo(_action);
    }
}

[MessagePackObject]
public class DeadLetterMessage
{
    [Key(0)]
    public string OriginalQueue { get; set; }
    
    [Key(1)]
    public string Reason { get; set; }
    
    [Key(2)]
    public byte[] Body { get; set; }
}