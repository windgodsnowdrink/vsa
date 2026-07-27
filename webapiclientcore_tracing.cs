#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using OpenTelemetry.Trace;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置分布式追踪
builder.Services.AddOpenTelemetry()
    .WithTracing(tracer => {
        tracer.AddSource("WebApiClientCore")
            .AddOtlpExporter(opt => {
                opt.Endpoint = new Uri("http://localhost:4317");
                opt.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
            });
    });

// 追踪上下文传播器
builder.Services.AddSingleton<ITraceContextPropagator>(sp => 
    new ChannelTracePropagator(
        Channel.CreateBounded<Activity>(1000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();
app.MapGet("/", () => "Distributed Tracing Ready");
app.Run();

public class ChannelTracePropagator : ITraceContextPropagator
{
    private readonly ChannelWriter<Activity> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelTracePropagator(Channel<Activity> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }
}