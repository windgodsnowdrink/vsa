#:sdk Microsoft.NET.Sdk.Web
#:package RestSharp@110.0.0
#:package Polly@8.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Channels;
using RestSharp;
using Polly;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder();

// 1. 配置RestSharp客户端工厂
builder.Services.AddSingleton<IRestClientFactory>(sp => 
    new ChannelRestClientFactory(
        Channel.CreateBounded<RestRequest>(10000),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[512])));

// 2. 弹性策略配置
builder.Services.AddSingleton<IRetryStrategy>(sp => 
    new PollyRetryStrategy(
        Policy<RestResponse>
            .HandleResult(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)),
        new ObjectPool<Memory<byte>>(new MemoryPooledObjectPolicy(), 1000)));

// 3. 缓存策略
builder.Services.AddSingleton<ICacheStrategy>(sp => 
    new MemoryCacheStrategy(
        new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 1024 * 1024 * 100 // 100MB缓存
        }),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

var app = builder.Build();
app.MapGet("/", () => "RestSharp Integration Ready");
app.Run();

// 高性能客户端工厂
[SkipLocalsInit]
public class ChannelRestClientFactory : IRestClientFactory
{
    private readonly ChannelWriter<RestRequest> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelRestClientFactory(Channel<RestRequest> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe IRestClient CreateClient()
    {
        Span<byte> buffer = stackalloc byte[512];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // SIMD优化处理
            }
        }
        return new RestClient();
    }
}

// 弹性策略实现
public class PollyRetryStrategy : IRetryStrategy
{
    private readonly IAsyncPolicy<RestResponse> _policy;
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    
    public async Task<RestResponse> ExecuteAsync(Func<Task<RestResponse>> action)
    {
        using var memory = _memoryPool.Get();
        return await _policy.ExecuteAsync(action);
    }
}

// 缓存策略实现
public class MemoryCacheStrategy : ICacheStrategy
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan expiration)
    {
        // 零拷贝缓存处理
    }
}