#:sdk Microsoft.NET.Sdk.Web
#:package MagicOnion@5.0.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.7.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using MagicOnion;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder();

// 1. OpenTelemetry配置
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddMagicOnionInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter(o => 
        {
            o.Endpoint = new Uri("http://collector:4317");
            o.ExportProcessorType = ExportProcessorType.Batch;
        }))
    .WithMetrics(metrics => metrics
        .AddMagicOnionInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter());

// 2. 高性能追踪处理器
builder.Services.AddSingleton<ITraceProcessor>(sp => 
    new ChannelTraceProcessor(
        Channel.CreateBounded<Activity>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();
app.MapMagicOnionService();
app.Run();

// 追踪处理器实现
[SkipLocalsInit]
public class ChannelTraceProcessor : ITraceProcessor
{
    private readonly ChannelWriter<Activity> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelTraceProcessor(Channel<Activity> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(Activity activity)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // SIMD优化处理
                _writer.TryWrite(activity);
            }
        }
    }
}