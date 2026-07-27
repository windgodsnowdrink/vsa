#:sdk Microsoft.NET.Sdk.Web
#:package Paramore.Brighter@10.0.0
#:package Paramore.Brighter.Extensions.Hosting@10.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. 配置Brighter
builder.Services.AddBrighter(options =>
{
    // 使用内存存储
    options.UseInMemoryOutbox();
    
    // 零拷贝优化
    options.UseChannel(ChannelType.SpanBased);
    
    // 对象池配置
    options.ObjectPoolSize = Environment.ProcessorCount * 2;
    
    // 分片策略
    options.ShardingStrategy = new ConsistentHashShardingStrategy(3);
}).UseExternalBus(new RedisTransport(
    new RedisConfiguration("localhost:6379"),
    new RedisPublication()));

// 2. 高性能处理器
builder.Services.AddScoped<RequestHandlerFactory>(_ => 
    new PooledRequestHandlerFactory(
        new DefaultHandlerFactory(), 
        Environment.ProcessorCount * 2));

var app = builder.Build();
app.MapGet("/", () => "Brighter Integration");
app.Run();

// 3. 一致性哈希分片策略
public class ConsistentHashShardingStrategy : IShardingStrategy
{
    private readonly int _shards;
    private readonly ThreadLocal<Span<byte>> _hashBuffer;
    
    public ConsistentHashShardingStrategy(int shards)
    {
        _shards = shards;
        _hashBuffer = new(() => stackalloc byte[256]);
    }
    
    public int GetShard(string key)
    {
        var buffer = _hashBuffer.Value;
        var hash = System.Security.Cryptography.SHA256.HashData(buffer);
        return hash[0] % _shards;
    }
}