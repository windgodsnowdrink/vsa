#:sdk Microsoft.NET.Sdk
#:package Ardalis.Specification@6.1.0
#:package Ardalis.Specification.EntityFrameworkCore@6.1.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package CAP@7.1.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Channels;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.ObjectPool;
using DotNetCore.CAP;

/*
services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite("Data Source=database.db"));

services.AddSpecificationRepository(options =>
{
    options.UseAotOptimizedEvaluator = true;
});
*/

// 1. 高性能仓储基类
public abstract class HighPerformanceRepository<T> : IRepositoryBase<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly ObjectPool<ISpecificationEvaluator> _evaluatorPool;
    private readonly ThreadLocal<ISpecificationEvaluator> _threadLocalEvaluator;

    protected HighPerformanceRepository(
        DbContext dbContext,
        ObjectPool<ISpecificationEvaluator> evaluatorPool)
    {
        _dbContext = dbContext;
        _evaluatorPool = evaluatorPool;
        _threadLocalEvaluator = new ThreadLocal<ISpecificationEvaluator>(
            () => _evaluatorPool.Get());
    }

    public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
        return entity;
    }

    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> GetBySpecAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    // 2. 零拷贝查询实现
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    // 3. 高性能批量操作
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.CountAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.AnyAsync(cancellationToken);
    }

    protected virtual IQueryable<T> ApplySpecification(
        ISpecification<T> specification, 
        ISpecificationEvaluator evaluator,
        bool evaluateCriteriaOnly = false)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<T, TResult> specification,
        ISpecificationEvaluator evaluator)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
    }

    public void Dispose()
    {
        if (_threadLocalEvaluator.IsValueCreated)
        {
            _evaluatorPool.Return(_threadLocalEvaluator.Value!);
        }
        _threadLocalEvaluator.Dispose();
    }
}

// 4. 自定义规约求值器（支持AOT编译）
[SkipLocalsInit]
public class AotOptimizedSpecificationEvaluator : SpecificationEvaluator
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override IQueryable<TResult> GetQuery<T, TResult>(
        IQueryable<T> query, 
        ISpecification<T, TResult> specification)
    {
        // ... existing code ...
        
        // 零拷贝优化处理
        if (specification.Selector is not null)
        {
            query = query.Select(specification.Selector);
        }

        return query;
    }
}

// 5. 仓储工厂实现
public class RepositoryFactory : IRepositoryFactory
{
    private readonly ObjectPool<ISpecificationEvaluator> _evaluatorPool;
    private readonly DbContext _dbContext;

    public RepositoryFactory(
        DbContext dbContext,
        ObjectPool<ISpecificationEvaluator> evaluatorPool)
    {
        _dbContext = dbContext;
        _evaluatorPool = evaluatorPool;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        return new HighPerformanceRepository<T>(_dbContext, _evaluatorPool);
    }

    public IReadRepository<T> GetReadRepository<T>() where T : class
    {
        return new HighPerformanceRepository<T>(_dbContext, _evaluatorPool);
    }
}

// 6. DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpecificationRepository(
        this IServiceCollection services,
        Action<SpecificationRepositoryOptions>? configure = null)
    {
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        
        services.AddSingleton(sp =>
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            var policy = new DefaultPooledObjectPolicy<ISpecificationEvaluator>();
            return provider.Create(policy);
        });

        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        
        return services;
    }
}

public class SpecificationRepositoryOptions
{
    public bool UseAotOptimizedEvaluator { get; set; } = true;
}

// 1. 分布式事务仓储基类
public abstract class DistributedRepository<T> : IRepositoryBase<T> where T : class
{
    private readonly DbContext _writeDbContext;
    private readonly DbContext _readDbContext;
    private readonly ICapPublisher _capPublisher;
    private readonly ObjectPool<ISpecificationEvaluator> _evaluatorPool;
    private readonly ThreadLocal<ISpecificationEvaluator> _threadLocalEvaluator;

    protected DistributedRepository(
        DbContext writeDbContext,
        DbContext readDbContext,
        ICapPublisher capPublisher,
        ObjectPool<ISpecificationEvaluator> evaluatorPool)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
        _capPublisher = capPublisher;
        _evaluatorPool = evaluatorPool;
        _threadLocalEvaluator = new ThreadLocal<ISpecificationEvaluator>(() => _evaluatorPool.Get());
    }

    // 2. 读写分离实现
    protected virtual IQueryable<T> ApplySpecification(
        ISpecification<T> specification, 
        bool evaluateCriteriaOnly = false)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        return evaluator.GetQuery(
            specification.IsReadOnly ? _readDbContext.Set<T>() : _writeDbContext.Set<T>(),
            specification,
            evaluateCriteriaOnly);
    }

    // 3. 分布式事务方法
    [CapTransaction]
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _writeDbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _capPublisher.PublishAsync($"entity.added.{typeof(T).Name}", entity);
    }

    public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
        return entity;
    }

    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> GetBySpecAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    // 2. 零拷贝查询实现
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    // 3. 高性能批量操作
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.CountAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.AnyAsync(cancellationToken);
    }

    protected virtual IQueryable<T> ApplySpecification(
        ISpecification<T> specification, 
        ISpecificationEvaluator evaluator,
        bool evaluateCriteriaOnly = false)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<T, TResult> specification,
        ISpecificationEvaluator evaluator)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
    }

    public void Dispose()
    {
        if (_threadLocalEvaluator.IsValueCreated)
        {
            _evaluatorPool.Return(_threadLocalEvaluator.Value!);
        }
        _threadLocalEvaluator.Dispose();
    }
}

// 4. 缓存只读仓储实现
public class CachedReadRepository<T> : IReadRepositoryBase<T> where T : class
{
    private readonly DbContext _readDbContext;
    private readonly IDistributedCache _cache;
    private readonly ObjectPool<ISpecificationEvaluator> _evaluatorPool;
    private readonly ThreadLocal<ISpecificationEvaluator> _threadLocalEvaluator;
    private readonly Channel<CacheUpdateEvent> _cacheChannel;

    public CachedReadRepository(
        DbContext readDbContext,
        IDistributedCache cache,
        ObjectPool<ISpecificationEvaluator> evaluatorPool)
    {
        _readDbContext = readDbContext;
        _cache = cache;
        _evaluatorPool = evaluatorPool;
        _threadLocalEvaluator = new ThreadLocal<ISpecificationEvaluator>(() => _evaluatorPool.Get());
        _cacheChannel = Channel.CreateBounded<CacheUpdateEvent>(10000);
        _ = Task.Run(ProcessCacheUpdatesAsync);
    }

    private async Task ProcessCacheUpdatesAsync()
    {
        await foreach (var update in _cacheChannel.Reader.ReadAllAsync())
        {
            var cacheKey = $"{typeof(T).Name}:{update.EntityId}";
            await _cache.RemoveAsync(cacheKey);
        }
    }

    public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{typeof(T).Name}:{id}";
        var cachedValue = await _cache.GetAsync(cacheKey, cancellationToken);
        
        if (cachedValue != null)
        {
            return MemoryPackSerializer.Deserialize<T>(cachedValue);
        }

        var entity = await _readDbContext.Set<T>().FindAsync(new object[] { id! }, cancellationToken);
        if (entity != null)
        {
            await _cache.SetAsync(cacheKey, MemoryPackSerializer.Serialize(entity), 
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(30) },
                cancellationToken);
        }
        
        return entity;
    }

    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> GetBySpecAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    // 2. 零拷贝查询实现
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    // 3. 高性能批量操作
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.CountAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.AnyAsync(cancellationToken);
    }

    protected virtual IQueryable<T> ApplySpecification(
        ISpecification<T> specification, 
        ISpecificationEvaluator evaluator,
        bool evaluateCriteriaOnly = false)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<T, TResult> specification,
        ISpecificationEvaluator evaluator)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
    }

    public void Dispose()
    {
        if (_threadLocalEvaluator.IsValueCreated)
        {
            _evaluatorPool.Return(_threadLocalEvaluator.Value!);
        }
        _threadLocalEvaluator.Dispose();
    }
}

// 5. 分库分表策略
public class ShardingRepository<T> : IRepositoryBase<T> where T : class
{
    private readonly Func<string, DbContext> _dbContextFactory;
    private readonly IShardingStrategy<T> _shardingStrategy;
    private readonly ObjectPool<ISpecificationEvaluator> _evaluatorPool;
    private readonly ThreadLocal<ISpecificationEvaluator> _threadLocalEvaluator;

    public ShardingRepository(
        Func<string, DbContext> dbContextFactory,
        IShardingStrategy<T> shardingStrategy,
        ObjectPool<ISpecificationEvaluator> evaluatorPool)
    {
        _dbContextFactory = dbContextFactory;
        _shardingStrategy = shardingStrategy;
        _evaluatorPool = evaluatorPool;
        _threadLocalEvaluator = new ThreadLocal<ISpecificationEvaluator>(() => _evaluatorPool.Get());
    }

    private DbContext GetShardedDbContext(T entity)
    {
        var shardKey = _shardingStrategy.GetShardKey(entity);
        return _dbContextFactory(shardKey);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var dbContext = GetShardedDbContext(entity);
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);
    }

    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<TResult?> GetBySpecAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    // 2. 零拷贝查询实现
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<List<TResult>> ListAsync<TResult>(
        ISpecification<T, TResult> specification, 
        CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator);
        return await query.ToListAsync(cancellationToken);
    }

    // 3. 高性能批量操作
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.CountAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var evaluator = _threadLocalEvaluator.Value!;
        var query = ApplySpecification(specification, evaluator, true);
        return await query.AnyAsync(cancellationToken);
    }

    protected virtual IQueryable<T> ApplySpecification(
        ISpecification<T> specification, 
        ISpecificationEvaluator evaluator,
        bool evaluateCriteriaOnly = false)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification, evaluateCriteriaOnly);
    }

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(
        ISpecification<T, TResult> specification,
        ISpecificationEvaluator evaluator)
    {
        return evaluator.GetQuery(_dbContext.Set<T>().AsQueryable(), specification);
    }

    public void Dispose()
    {
        if (_threadLocalEvaluator.IsValueCreated)
        {
            _evaluatorPool.Return(_threadLocalEvaluator.Value!);
        }
        _threadLocalEvaluator.Dispose();
    }
}

// 6. 集成配置
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedSpecificationRepository(
        this IServiceCollection services,
        Action<SpecificationRepositoryOptions> configureOptions)
    {
        services.AddCap(x =>
        {
            x.UseEntityFramework<DbContext>();
            x.UseRabbitMQ(options =>
            {
                options.HostName = "localhost";
                options.Port = 5672;
            });
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
            options.InstanceName = "SpecificationRepo:";
        });

        services.AddSingleton<ObjectPool<ISpecificationEvaluator>>(sp => 
            new DefaultObjectPool<ISpecificationEvaluator>(
                new AotOptimizedSpecificationEvaluatorPolicy(), 
                Environment.ProcessorCount * 2));

        return services;
    }
}