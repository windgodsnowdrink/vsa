#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.6.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using OpenTelemetry.Trace;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 分布式追踪通道
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

var app = builder.Build();
app.MapGet("/", () => "Distributed Tracing Ready");
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
                // SIMD优化处理追踪数据
            }
        }
    }
}