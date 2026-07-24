#:sdk Microsoft.NET.Sdk.Web
#:package Carter@7.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Carter;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// 1. 模块接口定义
public interface ICarterModule
{
    string ModuleName { get; }
    Task InitializeAsync();
    Task ExecuteAsync(RouteRequest request);
}

// 2. 抽象模块基类(CPU cache-line对齐)
[SkipLocalsInit]
public abstract class CarterModuleBase : CarterModule, ICarterModule
{
    private readonly Channel<RouteRequest> _requestChannel;
    private readonly ObjectPool<RouteContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _memoryCacheOptions;

    public string ModuleName => GetType().Name;

    protected CarterModuleBase(
        IDistributedCache cache,
        IMemoryCache memoryCache,
        string basePath = "") : base(basePath)
    {
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _memoryCacheOptions = new MemoryCacheEntryOptions()
            .SetSize(1)  // 每个缓存项大小
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetPostEvictionCallback((key, value, reason, state) =>
            {
                // 缓存淘汰时的回调
                var logger = ctx.RequestServices.GetService<ILogger<CarterModuleBase>>();
                logger?.LogInformation($"Cache evicted: Key={key}, Reason={reason}");
                
                // 如果是主动删除，则不处理
                if (reason == EvictionReason.Removed) return;
                
                // 其他情况可以添加自定义处理逻辑
                // 例如：记录到监控系统或触发缓存预热
            });
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<RouteRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<RouteContext>(
            new RouteContextPooledPolicy(), 1000);
    }

    // 2. 高性能路由注册(Span优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected void MapGet<T>(string pattern, Func<HttpContext, Task<T>> handler)
    {
        base.MapGet(pattern, async ctx =>
        {
            var cacheKey = $"route:{ctx.Request.Path}";
            var cached = await _cache.GetAsync(cacheKey);
            if (cached != null)
            {
                await ctx.Response.WriteAsync(Encoding.UTF8.GetString(cached.Span));
                return;
            }

            var request = new RouteRequest(ctx);
            await _requestChannel.Writer.WriteAsync(request);
            var result = await request.CompletionSource.Task;
            
            await _cache.SetAsync(cacheKey, 
                Encoding.UTF8.GetBytes(result.ToString()),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            
            await ctx.Response.WriteAsJsonAsync(result);
        });
    }

    // 2. 高性能路由注册扩展(Span优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected void MapPost<TRequest, TResponse>(string pattern, Func<HttpContext, TRequest, Task<TResponse>> handler)
    {
        base.MapPost(pattern, async ctx =>
        {
            var cacheKey = $"route:{ctx.Request.Path}";
            
            // 1. 检查本地缓存
            if (_memoryCache.TryGetValue(cacheKey, out TResponse cachedValue))
            {
                await ctx.Response.WriteAsJsonAsync(cachedValue);
                return;
            }

            // 2. 处理请求
            var request = await ctx.Request.ReadFromJsonAsync<TRequest>();
            var response = await handler(ctx, request!);
            
            // 3. 更新两级缓存
            var bytes = JsonSerializer.SerializeToUtf8Bytes(response);
            await _distributedCache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            _memoryCache.Set(cacheKey, response, _memoryCacheOptions);
            
            await ctx.Response.WriteAsJsonAsync(response);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected void MapPut<TRequest, TResponse>(string pattern, Func<HttpContext, TRequest, Task<TResponse>> handler)
    {
        base.MapPut(pattern, async ctx =>
        {
            var cacheKey = $"route:{ctx.Request.Path}";
            var request = await ctx.Request.ReadFromJsonAsync<TRequest>();
            var response = await handler(ctx, request!);
            
            // 更新两级缓存
            var bytes = JsonSerializer.SerializeToUtf8Bytes(response);
            await _distributedCache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            _memoryCache.Set(cacheKey, response, _memoryCacheOptions);
            
            await ctx.Response.WriteAsJsonAsync(response);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected void MapDelete<T>(string pattern, Func<HttpContext, Task<T>> handler)
    {
        base.MapPatch(pattern, async ctx =>
        {
            var cacheKey = $"route:{ctx.Request.Path}";
            var request = await ctx.Request.ReadFromJsonAsync<TRequest>();
            var response = await handler(ctx, request!);
            
            // 更新两级缓存
            var bytes = JsonSerializer.SerializeToUtf8Bytes(response);
            await _distributedCache.SetAsync(cacheKey, bytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            _memoryCache.Set(cacheKey, response, _memoryCacheOptions);
            
            await ctx.Response.WriteAsJsonAsync(response);
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected void MapPatch<TRequest, TResponse>(string pattern, Func<HttpContext, TRequest, Task<TResponse>> handler)
    {
        base.MapDelete(pattern, async ctx =>
        {
            var cacheKey = $"route:{ctx.Request.Path}";
            var response = await handler(ctx);
            
            // 删除两级缓存
            await _distributedCache.RemoveAsync(cacheKey);
            _memoryCache.Remove(cacheKey);
            
            await ctx.Response.WriteAsJsonAsync(response);
        });
    }

    // 3. 后台路由处理
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                await context.ProcessAsync(request);
                _latencyOptimizer.RecordLatency();
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }

    // 3. 程序集批量注册扩展
    public static class CarterModuleExtensions
    {
        public static IServiceCollection AddCarterModules(this IServiceCollection services, 
            params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            foreach (var assembly in assemblies)
            {
                var moduleTypes = assembly.GetTypes()
                    .Where(t => typeof(ICarterModule).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var type in moduleTypes)
                {
                    services.AddTransient(typeof(ICarterModule), type);
                }
            }

            return services;
        }
    }

    // 4. 模块注册器(高性能实现)
    [SkipLocalsInit]
    public sealed class CarterModuleRegistry
    {
        private readonly Dictionary<string, ICarterModule> _modules = new();
        private readonly ReaderWriterLockSlim _lock = new();

        public void RegisterModule(ICarterModule module)
        {
            _lock.EnterWriteLock();
            try
            {
                _modules[module.ModuleName] = module;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public ICarterModule GetModule(string moduleName)
        {
            _lock.EnterReadLock();
            try
            {
                return _modules.TryGetValue(moduleName, out var module) 
                    ? module 
                    : null;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }
}

// 4. 示例模块实现
public class UserModule : CarterModuleBase
{
    public UserModule(IDistributedCache cache) : base(cache, "/api/users")
    {
        MapGet("/{id}", GetUserById);
        MapPost<UserCreateRequest, User>("", CreateUser);
        MapPut<UserUpdateRequest, User>("/{id}", UpdateUser);
        MapDelete<User>("/{id}", DeleteUser);
    }

    private async Task<User> GetUserById(HttpContext ctx)
    {
        var userId = ctx.Request.RouteValues["id"]?.ToString();
        return new User { Id = userId, Name = "Test User" };
    }

    private async Task<User> CreateUser(HttpContext ctx, UserCreateRequest request)
    {
        return new User { Id = Guid.NewGuid().ToString(), Name = request.Name };
    }

    private async Task<User> UpdateUser(HttpContext ctx, UserUpdateRequest request)
    {
        var userId = ctx.Request.RouteValues["id"]?.ToString();
        return new User { Id = userId, Name = request.Name };
    }

    private async Task<User> DeleteUser(HttpContext ctx)
    {
        var userId = ctx.Request.RouteValues["id"]?.ToString();
        return new User { Id = userId, Name = "Deleted User" };
    }
}

// 新增请求记录类型
public record UserCreateRequest(string Name);
public record UserUpdateRequest(string Name);
// 辅助记录类型
public record User(string Id, string Name);
public record RouteRequest(HttpContext Context, TaskCompletionSource<object> CompletionSource = null!)
{
    public RouteRequest(HttpContext context) : this(context, new TaskCompletionSource<object>())
    {
    }
}

// 1. 添加缓存健康检查端点
public class CacheHealthEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/health/cache", async (IDistributedCache cache, IMemoryCache memoryCache) =>
        {
            var cacheKey = $"healthcheck:{Guid.NewGuid()}";
            try
            {
                // 测试分布式缓存
                await cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes("test"), 
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1) });
                
                // 测试内存缓存
                memoryCache.Set(cacheKey, "test", TimeSpan.FromSeconds(1));
                
                return Results.Ok(new { Status = "Healthy" });
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message);
            }
        }).WithTags("Health");
    }
}

// 2. 添加性能监控中间件
[SkipLocalsInit]
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public PerformanceMonitoringMiddleware(RequestDelegate next, TailLatencyOptimizer latencyOptimizer)
    {
        _next = next;
        _latencyOptimizer = latencyOptimizer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _latencyOptimizer.RecordLatency(stopwatch.ElapsedTicks);
        }
    }
}

// 5. 主程序配置
var builder = WebApplication.CreateBuilder(args);

// 3. 主程序配置更新
var builder = WebApplication.CreateBuilder(args);

// 添加性能监控服务
builder.Services.AddSingleton<TailLatencyOptimizer>();
builder.Services.AddSingleton<PerformanceMonitoringMiddleware>();

// 配置Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Carter:";
});

// 自动注册所有Carter模块
builder.Services.AddCarterModules(typeof(Program).Assembly);
builder.Services.AddCarter(configurator: options =>
{
    options.WithModule<UserModule>();
    // 其他模块...
});

// 添加新模块
builder.Services
    .AddDistributedTracing()
    .AddCarterApiDocs()
    .AddSingleton<CarterMetrics>();

var app = builder.Build();

// 使用性能监控中间件
app.UseMiddleware<PerformanceMonitoringMiddleware>();
// 使用Swagger
app.UseSwagger();
app.UseSwaggerUI();
// 注册健康检查端点
app.MapCarter();
new CacheHealthEndpoint().AddRoutes(app);

app.Run();