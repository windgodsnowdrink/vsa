#:sdk Microsoft.NET.Sdk.Web
#:package Linkerd.Client@0.3.0
#:package System.Net.Security@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Linkerd;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 服务网格通道
var meshChannel = Channel.CreateBounded<MeshPacket>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝网格处理器
builder.Services.AddSingleton<IMeshProcessor>(sp => 
    new ChannelMeshProcessor(
        meshChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[512])));

var app = builder.Build();
app.MapGet("/", () => "Service Mesh Ready");
app.Run();

[SkipLocalsInit]
public class ChannelMeshProcessor : IMeshProcessor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(MeshPacket packet)
    {
        Span<byte> buffer = stackalloc byte[512];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // 零拷贝处理网格数据
            }
        }
    }
}