#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.NET.Sdk.Boxed.Templates@6.0.0
#:package Microsoft.Orleans.Core@7.0.0
#:package Microsoft.Orleans.Server@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Orleans;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 1. Orleans配置
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("OrleansStorage");
});

// 2. 高性能通道
var orleansChannel = Channel.CreateBounded<OrleansMessage>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true,
        FullMode = BoundedChannelFullMode.Wait
    });

// 3. Boxed模板集成
builder.Services.AddBoxedOrleans(options =>
{
    options.EnableDashboard = true;
    options.EnableTelemetry = true;
});

var app = builder.Build();
app.MapGet("/", () => "Boxed Orleans Ready");
app.Run();

// Grain实现
public class TodoGrain : Grain, ITodoGrain
{
    private readonly ChannelWriter<OrleansMessage> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public TodoGrain(
        Channel<OrleansMessage> channel,
        ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe Task ProcessAsync(TodoCommand command)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                var msg = new OrleansMessage
                {
                    Payload = MessagePackSerializer.Serialize(command),
                    Timestamp = DateTimeOffset.UtcNow
                };
                return _writer.WriteAsync(msg).AsTask();
            }
        }
        return Task.CompletedTask;
    }
}

[MessagePackObject]
public class OrleansMessage
{
    [Key(0)]
    public byte[] Payload { get; set; }
    
    [Key(1)]
    public DateTimeOffset Timestamp { get; set; }
}

public interface ITodoGrain : IGrainWithGuidKey
{
    Task ProcessAsync(TodoCommand command);
}

public record TodoCommand(string Title);