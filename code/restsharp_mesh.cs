#:sdk Microsoft.NET.Sdk.Web
#:package Linkerd.Client@0.3.0
#:package System.Net.Security@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Linkerd;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// mTLS安全通道
var secureChannel = Channel.CreateBounded<SslStream>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 零拷贝网格处理器
builder.Services.AddSingleton<IMeshSecurityHandler>(sp => 
    new ChannelMeshSecurityHandler(
        secureChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[512])));

// Linkerd配置
builder.Services.AddLinkerdClient(options =>
{
    options.ControlPlaneAddr = "linkerd-proxy-api:8086";
    options.EnableTLS = true;
});

var app = builder.Build();
app.MapGet("/", () => "RestSharp Mesh Ready");
app.Run();

[SkipLocalsInit]
public class ChannelMeshSecurityHandler : IMeshSecurityHandler
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(SslStream stream)
    {
        Span<byte> buffer = stackalloc byte[512];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // 零拷贝加密处理
            }
        }
    }
}