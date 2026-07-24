#:sdk Microsoft.NET.Sdk.Web
#:package WebApiClientCore@2.0.0
#:package Castle.Core@5.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Castle.DynamicProxy;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder();

// 配置动态代理生成器
builder.Services.AddSingleton<IProxyGenerator>(sp => 
    new ProxyGenerator(new ThreadLocalSpanInterceptor(
        new ThreadLocal<Span<byte>>(() => stackalloc byte[1024]))));

var app = builder.Build();
app.MapGet("/", () => "Dynamic Proxy Ready");
app.Run();

public class ThreadLocalSpanInterceptor : IInterceptor
{
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ThreadLocalSpanInterceptor(ThreadLocal<Span<byte>> buffer) => _buffer = buffer;
    
    public void Intercept(IInvocation invocation)
    {
        var span = _buffer.Value;
        // 零拷贝处理代理调用
    }
}