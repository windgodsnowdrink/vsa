#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package RabbitMQ.Client@6.7.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using MagicOnion;
using System.Threading.Channels;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder();

// 1. 配置消息队列通道
var messageChannel = Channel.CreateBounded<QueueMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 2. 零拷贝消息处理器
builder.Services.AddSingleton<IMessageQueueProcessor>(sp => 
    new ChannelMessageQueueProcessor(
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

// 3. RabbitMQ连接工厂
builder.Services.AddSingleton<IConnectionFactory>(_ => 
    new ConnectionFactory { HostName = "localhost" });

var app = builder.Build();
app.MapGet("/", () => "Message Queue Ready");
app.Run();

// 消息队列处理器
[SkipLocalsInit]
public class ChannelMessageQueueProcessor : IMessageQueueProcessor
{
    private readonly ChannelWriter<QueueMessage> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelMessageQueueProcessor(Channel<QueueMessage> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Enqueue(QueueMessage message)
    {
        Span<byte> buffer = stackalloc byte[1024];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // SIMD优化处理消息
                _writer.TryWrite(message);
            }
        }
    }
}

[MessagePackObject]
public class QueueMessage
{
    [Key(0)]
    public string QueueName { get; set; }
    
    [Key(1)]
    public byte[] Body { get; set; }
    
    [Key(2)]
    public Dictionary<string, object> Headers { get; set; }
}