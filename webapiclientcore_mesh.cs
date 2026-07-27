#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package Linkerd.Client@0.3.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Linkerd;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置服务网格
builder.Services.AddLinkerdClient(options =>
{
    options.ControlPlaneAddr = "linkerd-proxy-api:8086";
    options.EnableTLS = true;
});

// 网格代理通道
builder.Services.AddSingleton<IMeshProxy>(sp => 
    new ChannelMeshProxy(
        Channel.CreateBounded<MeshPacket>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));

var app = builder.Build();
app.MapGet("/", () => "Service Mesh Ready");
app.Run();

public class ChannelMeshProxy : IMeshProxy
{
    private readonly ChannelWriter<MeshPacket> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelMeshProxy(Channel<MeshPacket> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }
}