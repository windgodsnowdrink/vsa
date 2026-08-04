#:sdk Microsoft.NET.Sdk
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;

// 1. 定义实体模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// 2. 实现DbContext
public class AppDbContext : DbContext
{
    private readonly SqliteConnection _connection;
    
    public AppDbContext(SqliteConnection connection)
    {
        _connection = connection;
    }
    
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_connection);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("DECIMAL(18,2)");
        });
    }
}

// 3. 批量操作服务
public class BulkOperationService : IAsyncDisposable
{
    private readonly Channel<BulkOperation> _operationChannel;
    private readonly Task _processingTask;
    private readonly ObjectPool<AppDbContext> _dbContextPool;
    private readonly CancellationTokenSource _cts = new();

    public BulkOperationService(SqliteConnection connection, int maxDegreeOfParallelism = 4)
    {
        _dbContextPool = new DefaultObjectPool<AppDbContext>(
            new DbContextPoolPolicy(connection), 
            maxDegreeOfParallelism);

        _operationChannel = Channel.CreateBounded<BulkOperation>(10000);
        _processingTask = Task.Run(ProcessOperationsAsync);
    }

    public async ValueTask AddBulkAsync(IEnumerable<Product> products)
    {
        await _operationChannel.Writer.WriteAsync(
            new BulkOperation { Type = OperationType.Add, Products = products.ToArray() });
    }

    public async ValueTask UpdateBulkAsync(ReadOnlySpan<Product> products)
    {
        var buffer = ArrayPool<Product>.Shared.Rent(products.Length);
        try
        {
            products.CopyTo(buffer);
            await _operationChannel.Writer.WriteAsync(
                new BulkOperation { 
                    Type = OperationType.Update, 
                    Products = buffer.Take(products.Length).ToArray() 
                });
        }
        finally
        {
            ArrayPool<Product>.Shared.Return(buffer);
        }
    }
    private async Task ProcessOperationsAsync()
    {
        await foreach (var operation in _operationChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var db = _dbContextPool.Get();
            try
            {
                switch (operation.Type)
                {
                    case OperationType.Add:
                        await db.Products.AddRangeAsync(operation.Products);
                        break;
                    // 其他操作类型...
                }
                await db.SaveChangesAsync();
            }
            finally
            {
                _dbContextPool.Return(db);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _operationChannel.Writer.Complete();
        _cts.Cancel();
        await _processingTask;
    }

    private class DbContextPoolPolicy : IPooledObjectPolicy<AppDbContext>
    {
        private readonly SqliteConnection _connection;

        public DbContextPoolPolicy(SqliteConnection connection)
        {
            _connection = connection;
        }

        public AppDbContext Create() => new AppDbContext(_connection);

        public bool Return(AppDbContext obj)
        {
            obj.ChangeTracker.Clear();
            return true;
        }
    }

    private class BulkDeleteHandler : IEventHandler<BulkOperation>
    {
        private readonly ObjectPool<AppDbContext> _dbContextPool;
    
        public BulkDeleteHandler(ObjectPool<AppDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool;
        }
    
        public void OnEvent(BulkOperation operation, long sequence, bool endOfBatch)
        {
            using var db = _dbContextPool.Get();
            try
            {
                db.Products.RemoveRange(operation.Products);
                db.SaveChanges();
            }
            finally
            {
                _dbContextPool.Return(db);
            }
        }
    }
}

// 4. 使用示例
public static class SqliteEfCoreDemo
{
    public static async Task RunAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        
        // 初始化数据库
        using (var db = new AppDbContext(connection))
        {
            await db.Database.EnsureCreatedAsync();
        }

        // 使用批量操作
        await using var bulkService = new BulkOperationService(connection);
        
        // 批量添加1000个产品
        var products = Enumerable.Range(1, 1000)
            .Select(i => new Product { Name = $"Product {i}", Price = i * 10 });
            
        await bulkService.AddBulkAsync(products);
    }
}

// 支持的操作类型
public enum OperationType { Add, Update, Delete }

// 批量操作数据结构
public class BulkOperation
{
    public OperationType Type { get; set; }
    public Product[] Products { get; set; }
}

public class QueryService : IAsyncDisposable
{
    private readonly Channel<QueryRequest> _queryChannel;
    private readonly Task _processingTask;
    private readonly ObjectPool<AppDbContext> _dbContextPool;

    public QueryService(ObjectPool<AppDbContext> dbContextPool)
    {
        _dbContextPool = dbContextPool;
        _queryChannel = Channel.CreateUnbounded<QueryRequest>();
        _processingTask = Task.Run(ProcessQueriesAsync);
    }

    public ValueTask<QueryResult> QueryAsync(Func<AppDbContext, Task<QueryResult>> query)
    {
        var tcs = new TaskCompletionSource<QueryResult>();
        _queryChannel.Writer.TryWrite(new QueryRequest { Query = query, Tcs = tcs });
        return new ValueTask<QueryResult>(tcs.Task);
    }
    private async Task ProcessOperationsAsync()
    {
        await foreach (var operation in _operationChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var db = _dbContextPool.Get();
            try
            {
                switch (operation.Type)
                {
                    case OperationType.Add:
                        await db.Products.AddRangeAsync(operation.Products);
                        break;
                    // 其他操作类型...
                }
                await db.SaveChangesAsync();
            }
            finally
            {
                _dbContextPool.Return(db);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _operationChannel.Writer.Complete();
        _cts.Cancel();
        await _processingTask;
    }

    private class DbContextPoolPolicy : IPooledObjectPolicy<AppDbContext>
    {
        private readonly SqliteConnection _connection;

        public DbContextPoolPolicy(SqliteConnection connection)
        {
            _connection = connection;
        }

        public AppDbContext Create() => new AppDbContext(_connection);

        public bool Return(AppDbContext obj)
        {
            obj.ChangeTracker.Clear();
            return true;
        }
    }

    private class BulkDeleteHandler : IEventHandler<BulkOperation>
    {
        private readonly ObjectPool<AppDbContext> _dbContextPool;
    
        public BulkDeleteHandler(ObjectPool<AppDbContext> dbContextPool)
        {
            _dbContextPool = dbContextPool;
        }
    
        public void OnEvent(BulkOperation operation, long sequence, bool endOfBatch)
        {
            using var db = _dbContextPool.Get();
            try
            {
                db.Products.RemoveRange(operation.Products);
                db.SaveChanges();
            }
            finally
            {
                _dbContextPool.Return(db);
            }
        }
    }
}