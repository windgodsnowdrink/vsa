#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.NET.Sdk.Boxed.Templates@6.0.0
#:package FastEndpoints@6.1.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using FastEndpoints;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 1. 高性能通道(RingBuffer + Disruptor模式)
var apiChannel = Channel.CreateBounded<ApiMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 2. 零拷贝处理器
builder.Services.AddSingleton<IApiProcessor>(sp => 
    new ApiChannelProcessor(
        apiChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

// 3. Boxed模板集成
builder.Services.AddBoxedApi(options =>
{
    options.EnableSwagger = true;
    options.EnableHttps = true;
});

var app = builder.Build();
app.UseFastEndpoints();
app.MapGet("/", () => "Boxed WebAPI Ready");
app.Run();

// API端点
public class TodoEndpoint : Endpoint<TodoRequest, TodoResponse>
{
    private readonly IApiProcessor _processor;
    
    public TodoEndpoint(IApiProcessor processor) => _processor = processor;

    public override void Configure()
    {
        Get("/api/todos");
        AllowAnonymous();
    }

    public override async Task HandleAsync(TodoRequest req, CancellationToken ct)
    {
        await _processor.ProcessAsync(req);
        await SendAsync(new TodoResponse());
    }
}

// 高性能处理器
[SkipLocalsInit]
public class ApiChannelProcessor : IApiProcessor
{
    private readonly ChannelWriter<ApiMessage> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ApiChannelProcessor(
        Channel<ApiMessage> channel,
        ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe Task ProcessAsync(TodoRequest request)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                var msg = new ApiMessage
                {
                    Payload = MessagePackSerializer.Serialize(request),
                    Timestamp = DateTimeOffset.UtcNow
                };
                return _writer.WriteAsync(msg).AsTask();
            }
        }
        return Task.CompletedTask;
    }
}

[MessagePackObject]
public class ApiMessage
{
    [Key(0)]
    public byte[] Payload { get; set; }
    
    [Key(1)]
    public DateTimeOffset Timestamp { get; set; }
}

public record TodoRequest(string Title);
public record TodoResponse();