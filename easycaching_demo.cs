#:sdk Microsoft.NET.Sdk.Web
#:package EasyCaching.Redis@8.0.0
#:package EasyCaching.InMemory@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using System.Buffers;
using System.Threading.Channels;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
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
// 在缓存服务中添加详细监控
public class TodoCacheService
{
    private readonly Counter<int> _cacheHits;
    private readonly Counter<int> _cacheMisses;
    private readonly Histogram<double> _cacheLatency;
    
    private readonly IEasyCachingProvider _cacheProvider;
    private readonly Channel<TodoItem> _cacheChannel;
    private readonly MemoryOptimizer _memoryOptimizer;
    private readonly CircuitBreaker _circuitBreaker;

    public TodoCacheService(
        IEasyCachingProviderFactory cacheFactory,
        MemoryOptimizer memoryOptimizer)
    {
        _cacheProvider = cacheFactory.GetCachingProvider("default");
        _memoryOptimizer = memoryOptimizer;
        _cacheChannel = Channel.CreateUnbounded<TodoItem>();
        _circuitBreaker = new CircuitBreaker(
            maxFailures: 5,
            resetTimeout: TimeSpan.FromSeconds(30));
        
        // 启动后台处理任务
        _ = ProcessCacheUpdatesAsync();
        var meter = new Meter("TodoCache");
        _cacheHits = meter.CreateCounter<int>("cache_hits");
        _cacheMisses = meter.CreateCounter<int>("cache_misses");
        _cacheLatency = meter.CreateHistogram<double>("cache_latency", "ms");
    }

    private async Task ProcessCacheUpdatesAsync()
    {
        await foreach (var todo in _cacheChannel.Reader.ReadAllAsync())
        {
            await _cacheProvider.SetAsync($"todo:{todo.Id}", todo, TimeSpan.FromMinutes(30));
        }
    }

    public async Task<TodoItem> GetTodoAsync(int id)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await _circuitBreaker.ExecuteAsync(async () => 
            {
                var cacheValue = await _cacheProvider.GetAsync<TodoItem>($"todo:{id}");
                if (cacheValue.HasValue) 
                {
                    _cacheHits.Add(1);
                    return cacheValue.Value;
                }
                _cacheMisses.Add(1);
                return null;
            });
            
            return result;
        }
        finally
        {
            _cacheLatency.Record(stopwatch.ElapsedMilliseconds);
        }
    }
}

// 添加健康检查
builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration["RedisCache:Configuration"])
    .AddDbContextCheck<TodoDbContext>();

// 添加指标端点
app.UseMetricServer("/metrics");
app.UseHttpMetrics();

var builder = WebApplication.CreateBuilder();

// 配置EasyCaching
builder.Services.AddEasyCaching(options =>
{
    // 内存缓存
    options.UseInMemory("inmemory");
    
    // Redis缓存
    options.UseRedis(config =>
    {
        config.DBConfig = new EasyCaching.Redis.RedisDBOptions
        {
            Configuration = builder.Configuration["RedisCache:Configuration"]
        };
    }, "default");
    
    // 混合缓存策略
    options.UseHybrid(config =>
    {
        config.TopicName = "todo-cache-topic";
        config.EnableLogging = true;
        config.LocalCacheProviderName = "inmemory";
        config.DistributedCacheProviderName = "default";
    });
});

// 添加内存优化服务
builder.Services.AddSingleton<MemoryOptimizer>();
builder.Services.AddSingleton<TodoCacheService>();

// 添加缓存预热服务
public class CacheWarmupService : IHostedService
{
    private readonly TodoCacheService _cacheService;
    private readonly TodoDbContext _dbContext;

    public CacheWarmupService(TodoCacheService cacheService, TodoDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var todos = await _dbContext.Todos.ToListAsync(cancellationToken);
        foreach (var todo in todos)
        {
            await _cacheService.UpdateTodoAsync(todo);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

// 在Startup中注册
builder.Services.AddHostedService<CacheWarmupService>();

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