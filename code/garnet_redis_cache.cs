#:sdk Microsoft.NET.Sdk.Web
#:package Garnet.StandardCache.Redis@1.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package Microsoft.EntityFrameworkCore.SqlServer@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Garnet.StandardCache; 
using Garnet.StandardCache.Redis; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore; 
using StackExchange.Redis; 

// 定义数据实体
class Product 
{ 
    public int Id { get; set; } 
    public string Name { get; set; } 
    public decimal Price { get; set; } 
} 

// 定义数据库上下文
class ProductDbContext : DbContext 
{ 
    public DbSet<Product> Products { get; set; } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    { 
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ProductDB;Trusted_Connection=True;"); 
    } 
} 

// 定义缓存服务
class ProductCacheService 
{ 
    private readonly ICache _productCache; 
    private readonly ProductDbContext _dbContext; 

    public ProductCacheService(ICache productCache, ProductDbContext dbContext, 
                             IDistributedLock distributedLock) 
    { 
        _productCache = productCache; 
        _dbContext = dbContext; 
        _distributedLock = distributedLock;
    } 

    // 获取产品信息，优先从缓存获取
    // 更新GetProductAsync方法
    public async Task<Product> GetProductAsync(int productId)
    {
        var cacheKey = $"product:{productId}";
        
        await using var @lock = await _distributedLock.CreateLockAsync(
            $"lock:{cacheKey}", 
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10));
        
        if (!@lock.IsAcquired)
        {
            // 降级策略：直接查询数据库
            return await _dbContext.Products.FindAsync(productId);
        }

        // 布隆过滤器防穿透
        if (_bloomFilter != null && !_bloomFilter.Test(cacheKey))
            return null;
            
        // 使用内存池优化
        var memory = _memoryPool.Get();
        try
        {
            // 随机过期时间防雪崩
            var randomExpiry = TimeSpan.FromMinutes(30) 
                             + TimeSpan.FromSeconds(new Random().Next(0, 300));
                             
            var cachedProduct = await _productCache.GetAsync<Product>(cacheKey);
    
            if (cachedProduct != null)
            {
                return cachedProduct;
            }
    
            var product = await _dbContext.Products.FindAsync(productId);
            if (product != null)
            {
                await _productCache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30));
            }
    
            return product;
        }
        finally
        {
            _memoryPool.Return(memory);
        }
        _cacheHits.Add(1, new("hit", cachedProduct != null));
    }

    // 添加布隆过滤器
    builder.Services.AddSingleton<IBloomFilter>(new RedisBloomFilter(
        "ProductBloomFilter", 
        0.01,  // 误差率
        10000, // 预期元素数量
        builder.Configuration.GetConnectionString("Redis")));
    // 更新产品信息，同时更新缓存
    public async Task UpdateProductAsync(Product product) 
    { 
        _dbContext.Products.Update(product); 
        await _dbContext.SaveChangesAsync(); 

        var cacheKey = $"product:{product.Id}"; 
        await _productCache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30)); 
    } 

    // 删除产品信息，同时删除缓存
    public async Task DeleteProductAsync(int productId) 
    { 
        var product = await _dbContext.Products.FindAsync(productId); 
        if (product != null) 
        { 
            _dbContext.Products.Remove(product); 
            await _dbContext.SaveChangesAsync(); 

            var cacheKey = $"product:{productId}"; 
            await _productCache.RemoveAsync(cacheKey); 
        } 
    } 
} 

// 定义API控制器
[ApiController] 
[Route("[controller]")] 
class ProductController : ControllerBase 
{ 
    private readonly ProductCacheService _productCacheService; 

    public ProductController(ProductCacheService productCacheService) 
    { 
        _productCacheService = productCacheService; 
    } 

    [HttpGet("{productId}")] 
    public async Task<IActionResult> GetProduct(int productId) 
    { 
        var product = await _productCacheService.GetProductAsync(productId); 
        if (product == null) 
        { 
            return NotFound(); 
        } 
        return Ok(product); 
    } 

    [HttpPut] 
    public async Task<IActionResult> UpdateProduct([FromBody] Product product) 
    { 
        await _productCacheService.UpdateProductAsync(product); 
        return Ok(); 
    } 

    [HttpDelete("{productId}")] 
    public async Task<IActionResult> DeleteProduct(int productId) 
    { 
        await _productCacheService.DeleteProductAsync(productId); 
        return Ok(); 
    } 
} 

var builder = WebApplication.CreateBuilder(); 

// 配置 Redis 连接
var redisConfiguration = ConfigurationOptions.Parse("localhost:6379"); 
redisConfiguration.AbortOnConnectFail = false; 
redisConfiguration.ConnectRetry = 3; 
redisConfiguration.ConnectTimeout = 5000; 

// 配置 Garnet Redis 缓存
builder.Services.AddGarnetRedisCache(option => 
{ 
    option.RedisConfiguration = redisConfiguration; 
    option.CacheName = "ProductCache"; 
    option.DefaultAbsoluteExpiration = TimeSpan.FromMinutes(30); 
}); 

builder.Services.AddDbContext<ProductDbContext>(); 
builder.Services.AddScoped<ProductCacheService>(); 

// 添加内存池和零拷贝优化
builder.Services.AddSingleton<ObjectPool<Memory<byte>>>(new DefaultObjectPool<Memory<byte>>(
    new DefaultPooledObjectPolicy<Memory<byte>>(), 1000));
builder.Services.AddSingleton<TailLatencyOptimizer>();

// 添加高性能通道处理
builder.Services.AddSingleton<Channel<Product>>(Channel.CreateUnbounded<Product>(
    new UnboundedChannelOptions { SingleReader = true }));

// 在Startup中注册分布式锁服务
builder.Services.AddSingleton<IDistributedLock>(sp => 
    new RedisDistributedLock(sp.GetRequiredService<IConnectionMultiplexer>()));

app.MapControllers(); 

// 初始化数据库
using (var scope = app.Services.CreateScope()) 
{ 
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>(); 
    dbContext.Database.EnsureCreated(); 
    
    if (!dbContext.Products.Any()) 
    { 
        dbContext.Products.AddRange( 
            new Product { Id = 1, Name = "示例产品1", Price = 99.99m }, 
            new Product { Id = 2, Name = "示例产品2", Price = 199.99m } 
        ); 
        dbContext.SaveChanges(); 
    } 
} 

app.Run("http://localhost:5000");

/*
- 获取产品： GET http://localhost:5000/product/1
- 更新产品： PUT http://localhost:5000/product ，请求体为产品 JSON 数据
- 删除产品： DELETE http://localhost:5000/product/1
*/

// 添加健康检查服务
builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis"))
    .AddDbContextCheck<ProductDbContext>();

// 添加健康检查端点
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
        
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(result);
    }
});

// 添加指标收集
builder.Services.AddMetrics()
    .AddRedisMetrics()
    .AddEntityFrameworkCoreMetrics();

// 添加指标端点
app.UseMetricServer("/metrics");
app.UseHttpMetrics();

// 在ProductCacheService中添加详细指标
public class ProductCacheService
{
    private readonly Counter<int> _cacheHits;
    private readonly Counter<int> _cacheMisses;
    private readonly Histogram<double> _cacheLatency;
    
    public ProductCacheService(ICache productCache, ProductDbContext dbContext, 
                             IDistributedLock distributedLock)
    {
        _productCache = productCache;
        _dbContext = dbContext;
        _distributedLock = distributedLock;
        _meter = new Meter("ProductCache");
        _cacheHits = _meter.CreateCounter<int>("cache_hits", "count", "Cache hits");
        _cacheMisses = _meter.CreateCounter<int>("cache_misses", "count", "Cache misses");
        _cacheLatency = _meter.CreateHistogram<double>("cache_latency", "ms", "Cache operation latency");
    }

    public async Task<Product> GetProductAsync(int productId)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // 1. 先检查本地内存缓存
            if (_localCache.TryGetValue(cacheKey, out Product localCached))
                return localCached;
    
            // 2. 布隆过滤器防穿透
            if (_bloomFilter != null && !_bloomFilter.Test(cacheKey))
                return null;
                
            // 3. 检查分布式缓存
            var distributedCached = await _distributedCache.GetAsync<Product>(cacheKey);
            if (distributedCached != null)
            {
                // 写入本地缓存
                _localCache.Set(cacheKey, distributedCached, 
                    TimeSpan.FromSeconds(30)); // 本地缓存较短过期时间
                return distributedCached;
            }
    
            // 4. 回源数据库
            var product = await _dbContext.Products.FindAsync(productId);
            if (product != null)
            {
                // 双写缓存
                await _distributedCache.SetAsync(cacheKey, product, 
                    TimeSpan.FromMinutes(30));
                _localCache.Set(cacheKey, product, 
                    TimeSpan.FromSeconds(30));
            }
    
            return product;
        }
        finally
        {
            _cacheLatency.Record(stopwatch.ElapsedMilliseconds);
        }
    }
}

// 添加布隆过滤器
builder.Services.AddSingleton<IBloomFilter>(new RedisBloomFilter(
        "ProductBloomFilter", 
        0.01,  // 误差率
        10000, // 预期元素数量
        builder.Configuration.GetConnectionString("Redis")));
    // 更新产品信息，同时更新缓存
    public async Task UpdateProductAsync(Product product) 
    { 
        _dbContext.Products.Update(product); 
        await _dbContext.SaveChangesAsync(); 

        var cacheKey = $"product:{product.Id}"; 
        await _productCache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(30)); 
    } 

    // 删除产品信息，同时删除缓存
    public async Task DeleteProductAsync(int productId) 
    { 
        var product = await _dbContext.Products.FindAsync(productId); 
        if (product != null) 
        { 
            _dbContext.Products.Remove(product); 
            await _dbContext.SaveChangesAsync(); 

            var cacheKey = $"product:{productId}"; 
            await _productCache.RemoveAsync(cacheKey); 
        } 
    } 
}

// 添加限流策略
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Request.Path,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
    });
    
    options.OnRejected = (context, _) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        return new ValueTask();
    };
});

// 应用限流中间件
app.UseRateLimiter();

// 在Startup中添加本地缓存服务
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024 * 1024 * 100; // 100MB内存限制
    options.CompactionPercentage = 0.5;
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
});

// 添加缓存预热服务
public class CacheWarmupService : IHostedService
{
    private readonly ProductCacheService _cacheService;
    private readonly ProductDbContext _dbContext;

    public CacheWarmupService(ProductCacheService cacheService, ProductDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var products = await _dbContext.Products.ToListAsync(cancellationToken);
        foreach (var product in products)
        {
            await _cacheService.UpdateProductAsync(product);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

// 在Startup中注册
builder.Services.AddHostedService<CacheWarmupService>();


public class ProductCacheService
{
    private readonly CircuitBreaker _circuitBreaker;
    
    public ProductCacheService(/*...*/)
    {
        _circuitBreaker = new CircuitBreaker(
            maxFailures: 5,
            resetTimeout: TimeSpan.FromSeconds(30));
    }

    public async Task<Product> GetProductAsync(int productId)
    {
        try
        {
            return await _circuitBreaker.ExecuteAsync(async () => 
            {
                // ... normal cache logic ...
            });
        }
        catch (CircuitBreakerOpenException)
        {
            // 降级：直接查询数据库
            return await _dbContext.Products.FindAsync(productId);
        }
    }
}