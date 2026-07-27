#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore@10.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// 1. 工作单元接口定义
public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync();
    IRepository<T> GetRepository<T>() where T : class, IAggregateRoot;
}

// 2. 泛型仓储接口
public interface IRepository<T> where T : class, IAggregateRoot
{
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetByIdAsync(object id);
}

// 3. 聚合根标记接口
public interface IAggregateRoot { }

// 4. 高性能工作单元实现
public sealed class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    private readonly Channel<(Func<Task>, TaskCompletionSource)> _operations;
    private readonly ObjectPool<IRepository<IAggregateRoot>> _repositoryPool;

    public UnitOfWork(
        TContext context,
        ObjectPool<IRepository<IAggregateRoot>> repositoryPool)
    {
        _context = context;
        _repositoryPool = repositoryPool;
        _operations = Channel.CreateBounded<(Func<Task>, TaskCompletionSource)>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.Wait
            });
        
        _ = ProcessOperationsAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessOperationsAsync()
    {
        await foreach (var (operation, tcs) in _operations.Reader.ReadAllAsync())
        {
            try
            {
                await operation();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IRepository<T> GetRepository<T>() where T : class, IAggregateRoot
    {
        var repo = _repositoryPool.Get();
        return (IRepository<T>)repo;
    }

    [SkipLocalsInit]
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    break;
                case EntityState.Modified:
                case EntityState.Deleted:
                    entry.Reload();
                    break;
            }
        }
    }

    public void Dispose() => _context.Dispose();
}

// 5. 泛型仓储实现
public class Repository<T> : IRepository<T> where T : class, IAggregateRoot
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    [SkipLocalsInit]
    public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);
}

// 6. DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        
        services.AddSingleton<ObjectPool<IRepository<IAggregateRoot>>>(sp => 
            new DefaultObjectPool<IRepository<IAggregateRoot>>(
                new RepositoryPoolPolicy(sp.GetRequiredService<TContext>()),
                Environment.ProcessorCount * 2));

        // 注册增强版Repository池
        builder.Services.AddSingleton<ObjectPool<EnhancedRepository<IAggregateRoot>>>(sp => 
            new DefaultObjectPool<EnhancedRepository<IAggregateRoot>>(
                new EnhancedRepositoryPoolPolicy(sp.GetRequiredService<TContext>()), 
                Environment.ProcessorCount * 2));
        builder.Services.AddScoped<IUnitOfWork, EnhancedUnitOfWork<TContext>>();
            
        return services;
    }

    private class RepositoryPoolPolicy : IPooledObjectPolicy<IRepository<IAggregateRoot>>
    {
        private readonly DbContext _context;

        public RepositoryPoolPolicy(DbContext context) => _context = context;

        public IRepository<IAggregateRoot> Create() => new Repository<IAggregateRoot>(_context);
        public bool Return(IRepository<IAggregateRoot> obj) => true;
    }
}

// 7. 使用示例
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddUnitOfWork<AppDbContext>();

var app = builder.Build();
app.MapGet("/", () => "UnitOfWork Service Ready");
app.Run();

// 5. 批量操作扩展
public interface IBulkRepository<T> where T : class, IAggregateRoot
{
    Task BulkInsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task BulkUpdateAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task BulkDeleteAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task BulkMergeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}

// 6. 统计查询扩展
public interface IAnalyticalRepository<T> where T : class, IAggregateRoot
{
    Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<decimal> SumAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<decimal> AverageAsync(Expression<Func<T, decimal>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}

// 7. 增强的Repository实现
public sealed class EnhancedRepository<T> : IRepository<T>, IBulkRepository<T>, IAnalyticalRepository<T> 
    where T : class, IAggregateRoot
{
    private readonly DbContext _context;
    private readonly Channel<Func<Task>> _bulkOperations;
    private readonly ThreadLocal<DbSet<T>> _threadLocalSet;

    public EnhancedRepository(DbContext context)
    {
        _context = context;
        _bulkOperations = Channel.CreateBounded<Func<Task>>(10000);
        _threadLocalSet = new(() => _context.Set<T>());
        
        // 启动批量处理后台任务
        _ = Task.Run(ProcessBulkOperationsAsync);
    }

    private async Task ProcessBulkOperationsAsync()
    {
        await foreach (var operation in _bulkOperations.Reader.ReadAllAsync())
        {
            await operation();
        }
    }

    public async Task BulkInsertAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _bulkOperations.Writer.WriteAsync(async () => 
        {
            await _context.BulkInsertAsync(entities, options => 
            {
                options.BatchSize = 1000;
                options.UseTempDB = true;
            }, cancellationToken);
        });
    }

    // ... 其他批量操作方法实现 ...

    public async Task<long> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _threadLocalSet.Value
            .AsNoTracking()
            .Where(predicate)
            .LongCountAsync(cancellationToken);
    }

    // ... 其他统计查询方法实现 ...
}

// 8. 增强的UnitOfWork实现
public sealed class EnhancedUnitOfWork<TContext> : UnitOfWork<TContext> where TContext : DbContext
{
    private readonly ObjectPool<EnhancedRepository<IAggregateRoot>> _enhancedRepositoryPool;

    public EnhancedUnitOfWork(
        TContext context,
        ObjectPool<IRepository<IAggregateRoot>> repositoryPool,
        ObjectPool<EnhancedRepository<IAggregateRoot>> enhancedRepositoryPool)
        : base(context, repositoryPool)
    {
        _enhancedRepositoryPool = enhancedRepositoryPool;
    }

    public IRepository<T> GetEnhancedRepository<T>() where T : class, IAggregateRoot
    {
        var repo = _enhancedRepositoryPool.Get();
        return (IRepository<T>)repo;
    }
}