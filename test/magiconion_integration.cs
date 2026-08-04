#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using MagicOnion;
using System.Threading.Channels;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder();

// 1. 高性能通道
var messageChannel = Channel.CreateBounded<StreamingMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 2. 零拷贝处理器
builder.Services.AddSingleton<IStreamingProcessor>(sp => 
    new ChannelStreamingProcessor(
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

// 3. 分布式缓存
builder.Services.AddStackExchangeRedisCache(options => 
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "MagicOnion_";
});

var app = builder.Build();
app.MapGet("/", () => "MagicOnion Integration Ready");
app.Run();

// 流式处理器
[SkipLocalsInit]
public class ChannelStreamingProcessor : IStreamingProcessor
{
    private readonly ChannelWriter<StreamingMessage> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelStreamingProcessor(Channel<StreamingMessage> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(StreamingMessage message)
    {
        Span<byte> buffer = stackalloc byte[1024];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                _writer.TryWrite(message);
            }
        }
    }
}

[MessagePackObject]
public class StreamingMessage
{
    [Key(0)]
    public string Route { get; set; }
    
    [Key(1)]
    public byte[] Payload { get; set; }
    
    [Key(2)]
    public DateTimeOffset Timestamp { get; set; }
}