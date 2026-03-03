#:sdk Microsoft.NET.Sdk.Web
#:package StackExchange.Redis@2.6.128
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using System.Buffers;
using System.Threading.Channels;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System.Collections.Concurrent;
using System.Threading.Tasks.Dataflow;

// 内存优化相关类
public class MemoryObjectPool<T>
{
    private readonly ObjectPool<T> _pool;
    
    public MemoryObjectPool(Func<T> createFunc, int size)
    {
        _pool = new DefaultObjectPool<T>(new DefaultPooledObjectPolicy<T> { Create = createFunc }, size);
    }
    
    public T Get() => _pool.Get();
    public void Return(T obj) => _pool.Return(obj);
}

// 尾延迟优化器
public class TailLatencyOptimizer
{
    private readonly ConcurrentQueue<Action> _workItems = new();
    private readonly ThreadLocal<Span<byte>> _threadLocalSpan = new(() => stackalloc byte[1024]);
    
    public void Enqueue(Action workItem)
    {
        _workItems.Enqueue(workItem);
    }
    
    public void Process()
    {
        while (_workItems.TryDequeue(out var item))
        {
            item();
        }
    }
}

// Todo数据模型
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// Redis缓存配置选项
public class RedisCacheOptions
{
    public string Configuration { get; set; }
    public string InstanceName { get; set; }
}

// 缓存服务
// Redis缓存服务增强
public class RedisCacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _cache;
    private readonly MemoryObjectPool<Memory<byte>> _memoryPool;
    private readonly TailLatencyOptimizer _optimizer;
    private readonly Channel<(string key, string value)> _cacheWriteChannel = Channel.CreateUnbounded<(string, string)>();
    
    public RedisCacheService(RedisCacheOptions options)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(options.Configuration);
        _cache = _connectionMultiplexer.GetDatabase();
        _memoryPool = new MemoryObjectPool<Memory<byte>>(() => new Memory<byte>(new byte[1024]), 100);
        _optimizer = new TailLatencyOptimizer();
        
        // 启动通道处理任务
        _ = ProcessWriteChannelAsync();
    }
    
    private async Task ProcessWriteChannelAsync()
    {
        await foreach (var (key, value) in _cacheWriteChannel.Reader.ReadAllAsync())
        {
            await _cache.StringSetAsync(key, value);
        }
    }
    
    // 添加本地内存缓存
    private readonly ConcurrentDictionary<string, (T value, DateTimeOffset expiry)> _localCache = new();
    private readonly TimeSpan _localCacheExpiry = TimeSpan.FromMinutes(5);

    public async Task<T> GetAsync<T>(string key)
    {
        // 先检查本地缓存
        if (_localCache.TryGetValue(key, out var cached) && cached.expiry > DateTimeOffset.UtcNow)
        {
            return (T)cached.value;
        }

        // 本地缓存未命中则查询Redis
        var value = await _cache.StringGetAsync(key);
        if (value.HasValue)
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<T>(value);
            
            // 更新本地缓存
            _localCache[key] = (result, DateTimeOffset.UtcNow.Add(_localCacheExpiry));
            
            return result;
        }
        return default;
    }

    // 添加缓存降级和限流策略
    private readonly SemaphoreSlim _cacheLock = new(10, 10);
    
    public async Task<T> GetWithFallbackAsync<T>(string key, Func<Task<T>> fallback, TimeSpan? expiry = null)
    {
        try
        {
            await _cacheLock.WaitAsync();
            
            var cached = await GetAsync<T>(key);
            if (cached != null) return cached;
            
            // 缓存未命中则调用fallback
            var result = await fallback();
            if (result != null)
            {
                await SetAsync(key, result, expiry);
            }
            return result;
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}

public async Task SetAsync<T>(string key, T value)
{
    var serializedValue = System.Text.Json.JsonSerializer.Serialize(value);
    _cacheWriteChannel.Writer.TryWrite((key, serializedValue));
    _optimizer.Enqueue(() =>
    {
        using var memory = _memoryPool.Get();
        // 内存优化操作
    });
}

public async Task DeleteAsync(string key)
{
    await _cache.KeyDeleteAsync(key);
}
}

// 数据处理管道
public class TodoDataPipeline
{
    private readonly TransformBlock<TodoItem, string> _serializeBlock;
    private readonly ActionBlock<string> _cacheBlock;
    
    public TodoDataPipeline(RedisCacheService cacheService)
    {
        _serializeBlock = new TransformBlock<TodoItem, string>(item =>
            System.Text.Json.JsonSerializer.Serialize(item));
        
        _cacheBlock = new ActionBlock<string>(async json =>
        {
            var item = System.Text.Json.JsonSerializer.Deserialize<TodoItem>(json);
            await cacheService.SetAsync($"todo:{item.Id}", item);
        });
        
        _serializeBlock.LinkTo(_cacheBlock);
    }
    
    public async Task ProcessTodoItem(TodoItem item)
    {
        await _serializeBlock.SendAsync(item);
    }
}

var builder = WebApplication.CreateBuilder();

// 配置Redis缓存
builder.Services.Configure<RedisCacheOptions>(builder.Configuration.GetSection("RedisCache"));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
    return ConnectionMultiplexer.Connect(options.Configuration);
});
builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddSingleton<TodoDataPipeline>();

var app = builder.Build();

// API端点
app.MapGet("/todo/{id}", async (int id, RedisCacheService cacheService) =>
{
    var todo = await cacheService.GetAsync<TodoItem>($"todo:{id}");
    return todo != null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todo", async (TodoItem todo, TodoDataPipeline pipeline) =>
{
    await pipeline.ProcessTodoItem(todo);
    return Results.Created($"/todo/{todo.Id}", todo);
});

app.MapDelete("/todo/{id}", async (int id, RedisCacheService cacheService) =>
{
    await cacheService.DeleteAsync($"todo:{id}");
    return Results.NoContent();
});

app.Run();