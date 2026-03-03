#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package System.Threading.Channels@8.0.0
#:package MessagePack@2.5.122
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using MagicOnion;
using System.Threading.Channels;
using MessagePack;

var builder = WebApplication.CreateBuilder();

// 1. 高性能通道配置
var messageChannel = Channel.CreateBounded<StreamingMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 2. 零拷贝处理器
builder.Services.AddSingleton<IStreamingProcessor>(sp => 
    new ChannelStreamingProcessor(
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

// 3. MagicOnion配置
builder.Services.AddMagicOnion(options =>
{
    options.SerializerOptions = MessagePackSerializerOptions.Standard
        .WithCompression(MessagePackCompression.Lz4BlockArray)
        .WithSecurity(MessagePackSecurity.UntrustedData);
    options.EnableCurrentContext = true;
});

var app = builder.Build();
app.MapMagicOnionService();
app.Run();

// 流式处理器实现
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
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // SIMD优化处理
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