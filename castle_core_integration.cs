#:sdk Microsoft.NET.Sdk.Web
#:package Castle.Core@5.2.1
#:package Castle.Core.AsyncInterceptor@2.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Threading.Channels;
using Castle.DynamicProxy;
using System.Threading.Tasks.Dataflow;

var builder = WebApplication.CreateBuilder();

// 添加内存优化服务
builder.Services.AddSingleton<MemoryOptimizer>();

// 配置Castle动态代理
builder.Services.AddSingleton<IInterceptor, HttpProxyInterceptor>();
builder.Services.AddScoped<IHttpProxyService>(provider =>
{
    var generator = new ProxyGenerator();
    var interceptor = provider.GetRequiredService<IInterceptor>();
    return generator.CreateInterfaceProxyWithTarget<IHttpProxyService>(
        new HttpProxyService(), interceptor);
});

var app = builder.Build();

app.MapGet("/proxy/get", async (IHttpProxyService proxy, string url) =>
{
    return await proxy.GetAsync(url);
});

app.MapPost("/proxy/post", async (IHttpProxyService proxy, string url, string content) =>
{
    return await proxy.PostAsync(url, content);
});

app.Run();

// 尾延优化器
public class MemoryOptimizer
{
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly ThreadLocal<Span<byte>> _threadLocalSpan;

    public MemoryOptimizer()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);
        _threadLocalSpan = new ThreadLocal<Span<byte>>(() => stackalloc byte[1024]);
    }
}

// HTTP请求拦截器
public class HttpProxyInterceptor : IInterceptor
{
    private readonly Channel<object> _requestChannel;
    private readonly MemoryOptimizer _memoryOptimizer;

    public HttpProxyInterceptor(MemoryOptimizer memoryOptimizer)
    {
        _memoryOptimizer = memoryOptimizer;
        _requestChannel = Channel.CreateUnbounded<object>();
        _ = ProcessRequestsAsync();
    }

    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync())
        {
            // 处理HTTP请求
        }
    }

    public void Intercept(IInvocation invocation)
    {
        using var memory = _memoryOptimizer.GetMemory();
        _requestChannel.Writer.TryWrite(invocation.Arguments);
        invocation.Proceed();
    }
}

// HTTP代理服务
public interface IHttpProxyService
{
    Task<string> GetAsync(string url);
    Task<string> PostAsync(string url, string content);
}

[Proxy]
public class HttpProxyService : IHttpProxyService
{
    public async Task<string> GetAsync(string url)
    {
        // 实现GET请求
        return "get";
    }

    public async Task<string> PostAsync(string url, string content)
    {
        // 实现POST请求
        return "post";
    }
}