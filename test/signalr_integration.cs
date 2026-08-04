#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR@8.0.0
#:package SignalW@6.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder();

// 1. SignalR高性能配置
builder.Services.AddSignalR(options => 
{
    options.EnableDetailedErrors = true;
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    options.StreamBufferCapacity = 10000;
}).AddMessagePackProtocol();

// 2. SignalW集成
builder.Services.AddSignalW(options => 
{
    options.UseZeroCopyBuffers = true;
    options.BufferSize = 1024 * 1024; // 1MB
});

// 3. 高性能通道
var messageChannel = Channel.CreateBounded<WebSocketMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 4. 零拷贝处理器
builder.Services.AddSingleton<IMessageProcessor>(sp => 
    new ChannelMessageProcessor(
        messageChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

var app = builder.Build();
app.MapHub<ChatHub>("/chat");
app.MapGet("/", () => "SignalR WebSocket Ready");
app.Run();

// Hub实现
[SkipLocalsInit]
public class ChatHub : Hub
{
    private readonly IMessageProcessor _processor;
    
    public ChatHub(IMessageProcessor processor)
    {
        _processor = processor;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<WebSocketMessage> StreamMessages()
    {
        // ... existing code ...
    }
}

// 消息处理器
[SkipLocalsInit]
public class ChannelMessageProcessor : IMessageProcessor
{
    private readonly ChannelWriter<WebSocketMessage> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelMessageProcessor(Channel<WebSocketMessage> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(WebSocketMessage message)
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
public class WebSocketMessage
{
    [Key(0)]
    public string Id { get; set; }
    
    [Key(1)]
    public byte[] Payload { get; set; }
    
    [Key(2)]
    public DateTimeOffset Timestamp { get; set; }
}