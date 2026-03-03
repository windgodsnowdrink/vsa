#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System.Threading.Tasks.Dataflow;

// 内存优化相关类
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

// Todo数据模型
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

// 缓存服务
public class TodoCacheService
{
    private readonly IMemoryCache _localCache;
    private readonly IDistributedCache _distributedCache;
    private readonly Channel<TodoItem> _cacheChannel;
    private readonly MemoryOptimizer _memoryOptimizer;

    public TodoCacheService(
        IMemoryCache localCache,
        IDistributedCache distributedCache,
        MemoryOptimizer memoryOptimizer)
    {
        _localCache = localCache;
        _distributedCache = distributedCache;
        _memoryOptimizer = memoryOptimizer;
        _cacheChannel = Channel.CreateUnbounded<TodoItem>();
        
        // 启动后台处理任务
        _ = ProcessCacheUpdatesAsync();
    }

    private async Task ProcessCacheUpdatesAsync()
    {
        await foreach (var todo in _cacheChannel.Reader.ReadAllAsync())
        {
            var cacheKey = $"todo:{todo.Id}";
            var serialized = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(todo);
            
            // 更新本地缓存
            _localCache.Set(cacheKey, todo, TimeSpan.FromMinutes(5));
            
            // 更新分布式缓存
            await _distributedCache.SetAsync(cacheKey, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
        }
    }

    public async Task<TodoItem> GetTodoAsync(int id)
    {
        var cacheKey = $"todo:{id}";
        
        // 1. 检查本地缓存
        if (_localCache.TryGetValue<TodoItem>(cacheKey, out var localCached))
            return localCached;

        // 2. 检查分布式缓存
        var distributedCached = await _distributedCache.GetAsync(cacheKey);
        if (distributedCached != null)
        {
            var todo = System.Text.Json.JsonSerializer.Deserialize<TodoItem>(distributedCached);
            
            // 更新本地缓存
            _localCache.Set(cacheKey, todo, TimeSpan.FromMinutes(5));
            
            return todo;
        }

        return null;
    }

    public async Task UpdateTodoAsync(TodoItem todo)
    {
        // 使用通道异步更新缓存
        await _cacheChannel.Writer.WriteAsync(todo);
    }
}

var builder = WebApplication.CreateBuilder();

// 配置内存缓存
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100MB限制
    options.CompactionPercentage = 0.5;
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
});

// 配置Redis分布式缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCache:Configuration"];
    options.InstanceName = "todo_cache_";
});

// 添加内存优化服务
builder.Services.AddSingleton<MemoryOptimizer>();
builder.Services.AddSingleton<TodoCacheService>();

var app = builder.Build();

// API端点
app.MapGet("/todo/{id}", async (int id, TodoCacheService cacheService) =>
{
    var todo = await cacheService.GetTodoAsync(id);
    return todo != null ? Results.Ok(todo) : Results.NotFound();
});

app.MapPost("/todo", async (TodoItem todo, TodoCacheService cacheService) =>
{
    await cacheService.UpdateTodoAsync(todo);
    return Results.Created($"/todo/{todo.Id}", todo);
});

app.Run();