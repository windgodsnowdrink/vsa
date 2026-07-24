#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

/// <summary>
/// 产品实体模型
/// </summary>
public class Product
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 产品名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 产品价格
    /// </summary>
    public decimal Price { get; set; }
}

/// <summary>
/// 应用数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    private readonly SqliteConnection _connection;

    /// <summary>
    /// 产品集合
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// 初始化应用数据库上下文
    /// </summary>
    /// <param name="connection">SQLite 连接</param>
    public AppDbContext(SqliteConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// 配置数据库选项
    /// </summary>
    /// <param name="options">数据库选项构建器</param>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(_connection);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    /// <summary>
    /// 配置模型
    /// </summary>
    /// <param name="modelBuilder">模型构建器</param>
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

/// <summary>
/// 批量操作类型
/// </summary>
enum BulkOperationType
{
    Insert,
    Update,
    Delete
}

/// <summary>
/// 批量操作
/// </summary>
public class BulkOperation
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public BulkOperationType Type { get; set; }

    /// <summary>
    /// 产品集合
    /// </summary>
    public List<Product> Products { get; set; }
}

/// <summary>
/// 批量操作服务
/// </summary>
public class BulkOperationService : IAsyncDisposable
{
    private readonly Channel<BulkOperation> _operationChannel;
    private readonly Task _processingTask;
    private readonly ObjectPool<AppDbContext> _dbContextPool;
    private readonly CancellationTokenSource _cts = new();

    /// <summary>
    /// 初始化批量操作服务
    /// </summary>
    /// <param name="connection">SQLite 连接</param>
    /// <param name="maxDegreeOfParallelism">最大并行度</param>
    public BulkOperationService(SqliteConnection connection, int maxDegreeOfParallelism = 4)
    {
        _dbContextPool = new DefaultObjectPool<AppDbContext>(
            new DbContextPoolPolicy(connection), 
            maxDegreeOfParallelism);

        _operationChannel = Channel.CreateBounded<BulkOperation>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });

        _processingTask = Task.Run(() => ProcessOperationsAsync(_cts.Token));
    }

    /// <summary>
    /// 批量插入产品
    /// </summary>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task BulkInsertAsync(List<Product> products, CancellationToken cancellationToken = default)
    {
        await _operationChannel.Writer.WriteAsync(new BulkOperation
        {
            Type = BulkOperationType.Insert,
            Products = products
        }, cancellationToken);
    }

    /// <summary>
    /// 批量更新产品
    /// </summary>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task BulkUpdateAsync(List<Product> products, CancellationToken cancellationToken = default)
    {
        await _operationChannel.Writer.WriteAsync(new BulkOperation
        {
            Type = BulkOperationType.Update,
            Products = products
        }, cancellationToken);
    }

    /// <summary>
    /// 批量删除产品
    /// </summary>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task BulkDeleteAsync(List<Product> products, CancellationToken cancellationToken = default)
    {
        await _operationChannel.Writer.WriteAsync(new BulkOperation
        {
            Type = BulkOperationType.Delete,
            Products = products
        }, cancellationToken);
    }

    /// <summary>
    /// 处理批量操作
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessOperationsAsync(CancellationToken cancellationToken)
    {
        await foreach (var operation in _operationChannel.Reader.ReadAllAsync(cancellationToken))
        {
            await ProcessSingleOperationAsync(operation, cancellationToken);
        }
    }

    /// <summary>
    /// 处理单个批量操作
    /// </summary>
    /// <param name="operation">批量操作</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessSingleOperationAsync(BulkOperation operation, CancellationToken cancellationToken)
    {
        var dbContext = _dbContextPool.Get();
        try
        {
            switch (operation.Type)
            {
                case BulkOperationType.Insert:
                    await dbContext.Products.AddRangeAsync(operation.Products, cancellationToken);
                    break;
                case BulkOperationType.Update:
                    dbContext.Products.UpdateRange(operation.Products);
                    break;
                case BulkOperationType.Delete:
                    dbContext.Products.RemoveRange(operation.Products);
                    break;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _dbContextPool.Return(dbContext);
        }
    }

    /// <summary>
    /// 执行批量插入（直接SQL方式）
    /// </summary>
    /// <param name="connection">SQLite 连接</param>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public static async Task BulkInsertWithSqlAsync(SqliteConnection connection, List<Product> products, CancellationToken cancellationToken = default)
    {
        if (products.Count == 0)
            return;

        using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO Products (Name, Price) VALUES (@Name, @Price)";

            foreach (var product in products)
            {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Price", product.Price);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        await _processingTask;
    }

    /// <summary>
    /// 数据库上下文池策略
    /// </summary>
    private class DbContextPoolPolicy : IPooledObjectPolicy<AppDbContext>
    {
        private readonly SqliteConnection _connection;

        /// <summary>
        /// 初始化数据库上下文池策略
        /// </summary>
        /// <param name="connection">SQLite 连接</param>
        public DbContextPoolPolicy(SqliteConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 创建数据库上下文
        /// </summary>
        /// <returns>数据库上下文</returns>
        public AppDbContext Create()
        {
            return new AppDbContext(_connection);
        }

        /// <summary>
        /// 验证并归还数据库上下文
        /// </summary>
        /// <param name="obj">数据库上下文</param>
        /// <returns>是否可以归还到池</returns>
        public bool Return(AppDbContext obj)
        {
            return true;
        }
    }
}

/// <summary>
/// 批量操作服务扩展方法
/// </summary>
public static class BulkOperationServiceExtensions
{
    /// <summary>
    /// 注册批量操作服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddBulkOperationService(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<SqliteConnection>(sp =>
        {
            var connection = new SqliteConnection(connectionString);
            connection.Open();
            return connection;
        });
        services.AddSingleton<BulkOperationService>();
        return services;
    }
}

/// <summary>
/// 批量操作控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BulkController : ControllerBase
{
    private readonly BulkOperationService _bulkService;

    /// <summary>
    /// 初始化批量操作控制器
    /// </summary>
    /// <param name="bulkService">批量操作服务</param>
    public BulkController(BulkOperationService bulkService)
    {
        _bulkService = bulkService;
    }

    /// <summary>
    /// 批量插入产品
    /// </summary>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> BulkInsert([FromBody] List<Product> products, CancellationToken cancellationToken)
    {
        await _bulkService.BulkInsertAsync(products, cancellationToken);
        return Ok(new { message = $"Inserted {products.Count} products" });
    }

    /// <summary>
    /// 批量更新产品
    /// </summary>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("update")]
    public async Task<IActionResult> BulkUpdate([FromBody] List<Product> products, CancellationToken cancellationToken)
    {
        await _bulkService.BulkUpdateAsync(products, cancellationToken);
        return Ok(new { message = $"Updated {products.Count} products" });
    }

    /// <summary>
    /// 批量删除产品
    /// </summary>
    /// <param name="products">产品集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("delete")]
    public async Task<IActionResult> BulkDelete([FromBody] List<Product> products, CancellationToken cancellationToken)
    {
        await _bulkService.BulkDeleteAsync(products, cancellationToken);
        return Ok(new { message = $"Deleted {products.Count} products" });
    }
}
