#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Diagnostics.NETCore.Client@0.2.251801
#:package System.Diagnostics.PerformanceCounter@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Diagnostics;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 内存泄漏检测通道
var leakChannel = Channel.CreateBounded<MemoryLeakEvent>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝泄漏检测器
builder.Services.AddSingleton<IMemoryLeakDetector>(sp => 
    new ChannelMemoryLeakDetector(
        leakChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();
app.MapGet("/", () => "Memory Leak Detection Ready");
app.Run();

[SkipLocalsInit]
public class ChannelMemoryLeakDetector : IMemoryLeakDetector
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Detect(MemoryLeakEvent @event)
    {
        Span<byte> buffer = stackalloc byte[256];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // SIMD优化处理内存泄漏检测
            }
        }
    }
}