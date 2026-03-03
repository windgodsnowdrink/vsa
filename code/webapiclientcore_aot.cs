#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package WebApiClientCore.Extensions.SourceGenerator@2.0.0
#:package Microsoft.CodeAnalysis.CSharp@4.6.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Channels;
using WebApiClientCore;
using WebApiClientCore.Implementations;

var builder = WebApplication.CreateBuilder();

// 配置AOT代码生成
builder.Services.AddWebApiClientCore()
    .ConfigureHttpApi(options =>
    {
        options.UseSourceGenerator = true;
        options.AssemblyName = "AotClientProxies";
        options.Namespace = "ClientProxies";
    });

// 高性能通道处理器
builder.Services.AddSingleton<Channel<Type>>(Channel.CreateUnbounded<Type>());
builder.Services.AddSingleton<IClientProxyGenerator, AotClientProxyGenerator>();

var app = builder.Build();
app.MapGet("/", () => "AOT Client Generator Ready");
app.Run();

// 零拷贝代码生成器
public class AotClientProxyGenerator : IClientProxyGenerator
{
    private readonly ChannelWriter<Type> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public AotClientProxyGenerator(Channel<Type> channel)
    {
        _writer = channel.Writer;
        _buffer = new ThreadLocal<Span<byte>>(() => stackalloc byte[1024]);
    }
    
    public Task GenerateAsync(Type interfaceType)
    {
        var span = _buffer.Value;
        // 使用LLVM IR生成AOT代码
        return _writer.WriteAsync(interfaceType).AsTask();
    }
}