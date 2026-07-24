#:sdk Microsoft.NET.Sdk.Web
#:package SkyAPM.Agent.AspNetCore@1.6.0
#:package SkyAPM.Diagnostics.EntityFrameworkCore@1.6.0
#:package SkyAPM.Transport.Grpc@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using SkyApm;
using SkyApm.Tracing;
using SkyApm.Transport;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置SkyAPM监控服务
builder.Services.AddSkyAPM(settings =>
{
    // 基础配置
    settings.ServiceName = "SampleApp";
    settings.Namespace = "Production";
    settings.DirectServers = "localhost:11800";
    
    // 性能优化配置
    settings.SamplingInterval = 1000; // 采样间隔(ms)
    settings.QueueSize = 10000;      // 队列大小
    settings.BatchSize = 100;       // 批量大小
    settings.PendingSegmentLimit = 10000; // 待处理段限制
    
    // 组件集成
    settings.EnableEntityFrameworkCore = true;
    settings.EnableHttpClient = true;
    settings.EnableGrpcClient = true;
});

// 高性能追踪处理器
builder.Services.AddSingleton<ISegmentDispatcher>(sp => 
    new ChannelSegmentDispatcher(
        Channel.CreateBounded<SegmentRequest>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();

// 自定义监控点示例
app.MapGet("/api/products", ([FromServices] ITracingContext tracingContext) =>
{
    using (var context = tracingContext.CreateEntrySegment("product.query"))
    {
        // 记录自定义标签
        context.Span.AddTag("query.type", "list");
        return Results.Ok();
    }
});

app.MapGet("/", () => "SkyAPM Monitoring Ready");
app.Run();

// 零拷贝通道分发器
public class ChannelSegmentDispatcher : ISegmentDispatcher
{
    private readonly ChannelWriter<SegmentRequest> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelSegmentDispatcher(Channel<SegmentRequest> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }
    
    public async Task DispatchAsync(SegmentRequest segmentRequest)
    {
        var span = _buffer.Value;
        // 零拷贝处理追踪数据
        await _writer.WriteAsync(segmentRequest);
    }
}