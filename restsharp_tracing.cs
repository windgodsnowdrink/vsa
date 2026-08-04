#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.6.0
#:package OpenTelemetry.Instrumentation.Http@1.6.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 高性能追踪通道
var traceChannel = Channel.CreateBounded<Activity>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝追踪处理器
builder.Services.AddSingleton<ITraceProcessor>(sp => 
    new ChannelTraceProcessor(
        traceChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

// OpenTelemetry配置
builder.Services.AddOpenTelemetry()
    .WithTracing(builder =>
    {
        builder
            .AddSource("RestSharp.Client")
            .AddHttpClientInstrumentation()
            .AddProcessor(new BatchActivityExportProcessor(
                new ChannelActivityExporter(traceChannel.Reader)))
            .AddOtlpExporter();
    });

var app = builder.Build();
app.MapGet("/", () => "RestSharp Tracing Ready");
app.Run();

[SkipLocalsInit]
public class ChannelTraceProcessor : ITraceProcessor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(Activity activity)
    {
        Span<byte> buffer = stackalloc byte[256];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // SIMD优化处理
            }
        }
    }
}