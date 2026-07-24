#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Data.Sqlite.Core@8.0.0
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:package System.Buffers@4.5.1
#:package System.Memory@4.5.5
#:package System.Threading.Channels@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Data;
using System.Threading.Channels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqliteIntegration();
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

namespace SqliteIntegration
{
    /// <summary>
    /// 客户实体
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// EFCore集成DbContext
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// 初始化AppDbContext
        /// </summary>
        /// <param name="options">数据库选项</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// 客户集合
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// 配置数据库选项
        /// </summary>
        /// <param name="optionsBuilder">选项构建器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        /// <summary>
        /// 配置模型
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }

    /// <summary>
    /// SQLite数据库配置选项
    /// </summary>
    public class SqliteOptions
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "Data Source=:memory:";

        /// <summary>
        /// 池大小
        /// </summary>
        public int PoolSize { get; set; } = 16;

        /// <summary>
        /// 命令超时时间（秒）
        /// </summary>
        public int CommandTimeout { get; set; } = 30;
    }

    /// <summary>
    /// SQLite连接池
    /// </summary>
    public class SqliteConnectionPool : IDisposable
    {
        private readonly Channel<SqliteConnection> _pool;
        private readonly SqliteOptions _options;
        private readonly ILogger<SqliteConnectionPool> _logger;
        private int _createdConnections;

        /// <summary>
        /// 初始化SQLite连接池
        /// </summary>
        /// <param name="options">SQLite选项</param>
        /// <param name="logger">日志记录器</param>
        public SqliteConnectionPool(IOptions<SqliteOptions> options, ILogger<SqliteConnectionPool> logger)
        {
            _options = options.Value;
            _logger = logger;
            _pool = Channel.CreateBounded<SqliteConnection>(new BoundedChannelOptions(_options.PoolSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            });

            // 预创建连接
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < _options.PoolSize / 2; i++)
                {
                    var connection = CreateConnection();
                    await _pool.Writer.WriteAsync(connection);
                }
            });
        }

        /// <summary>
        /// 创建SQLite连接
        /// </summary>
        /// <returns>SQLite连接</returns>
        private SqliteConnection CreateConnection()
        {
            var connection = new SqliteConnection(_options.ConnectionString);
            connection.Open();
            Interlocked.Increment(ref _createdConnections);
            _logger.LogInformation("Created new SQLite connection. Total: {Count}", _createdConnections);
            return connection;
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>SQLite连接</returns>
        public async Task<SqliteConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (_pool.Reader.TryRead(out var connection))
            {
                if (connection.State == ConnectionState.Open)
                {
                    return connection;
                }
                connection.Dispose();
            }

            // 池中没有可用连接，创建新连接
            return CreateConnection();
        }

        /// <summary>
        /// 归还连接
        /// </summary>
        /// <param name="connection">SQLite连接</param>
        public async Task ReturnConnectionAsync(SqliteConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                if (!_pool.Writer.TryWrite(connection))
                {
                    connection.Dispose();
                    Interlocked.Decrement(ref _createdConnections);
                }
            }
            else
            {
                connection.Dispose();
                Interlocked.Decrement(ref _createdConnections);
            }
        }

        /// <summary>
        /// 使用连接执行操作
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="func">操作函数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        public async Task<T> UseConnectionAsync<T>(Func<SqliteConnection, Task<T>> func, CancellationToken cancellationToken = default)
        {
            var connection = await GetConnectionAsync(cancellationToken);
            try
            {
                return await func(connection);
            }
            finally
            {
                await ReturnConnectionAsync(connection);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 释放所有连接
            while (_pool.Reader.TryRead(out var connection))
            {
                connection.Dispose();
            }
        }
    }

    /// <summary>
    /// SQLite数据库服务
    /// </summary>
    public class SqliteDatabaseService : IAsyncDisposable
    {
        private readonly SqliteConnectionPool _connectionPool;
        private readonly ILogger<SqliteDatabaseService> _logger;

        /// <summary>
        /// 初始化SQLite数据库服务
        /// </summary>
        /// <param name="connectionPool">SQLite连接池</param>
        /// <param name="logger">日志记录器</param>
        public SqliteDatabaseService(SqliteConnectionPool connectionPool, ILogger<SqliteDatabaseService> logger)
        {
            _connectionPool = connectionPool;
            _logger = logger;
        }

        /// <summary>
        /// 执行SQL命令
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteAsync(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _connectionPool.UseConnectionAsync(async (connection) =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 30;

                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                return await command.ExecuteNonQueryAsync(cancellationToken);
            }, cancellationToken);
        }

        /// <summary>
        /// 执行查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>结果集合</returns>
        public async Task<List<T>> QueryAsync<T>(string sql, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _connectionPool.UseConnectionAsync(async (connection) =>
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 30;

                if (parameters != null)
                {
                    AddParameters(command, parameters);
                }

                var results = new List<T>();
                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var mapper = GetMapper<T>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    results.Add(mapper(reader));
                }
                return results;
            }, cancellationToken);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="command">SQLite命令</param>
        /// <param name="parameters">参数对象</param>
        private void AddParameters(SqliteCommand command, object parameters)
        {
            var properties = parameters.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(parameters);
                command.Parameters.AddWithValue($"@{property.Name}", value ?? DBNull.Value);
            }
        }

        /// <summary>
        /// 获取映射器
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <returns>映射函数</returns>
        private Func<SqliteDataReader, T> GetMapper<T>()
        {
            // 简单映射实现
            return reader =>
            {
                var obj = Activator.CreateInstance<T>();
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    try
                    {
                        var value = reader[property.Name];
                        if (value != DBNull.Value)
                        {
                            property.SetValue(obj, value);
                        }
                    }
                    catch
                    {
                        // 忽略映射错误
                    }
                }
                return obj;
            };
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns>任务</returns>
        public async ValueTask DisposeAsync()
        {
            // 释放资源
        }
    }

    /// <summary>
    /// SQLite集成扩展方法
    /// </summary>
    public static class SqliteIntegrationExtensions
    {
        /// <summary>
        /// 添加SQLite集成服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddSqliteIntegration(
            this IServiceCollection services,
            Action<SqliteOptions> configureOptions = null)
        {
            services.Configure<SqliteOptions>(options =>
            {
                configureOptions?.Invoke(options);
            });

            services.AddSingleton<SqliteConnectionPool>();
            services.AddSingleton<SqliteDatabaseService>();

            // 配置EFCore
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var sqliteOptions = sp.GetRequiredService<IOptions<SqliteOptions>>().Value;
                options.UseSqlite(sqliteOptions.ConnectionString);
            });

            return services;
        }
    }

    /// <summary>
    /// SQLite控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SqliteController : ControllerBase
    {
        private readonly SqliteDatabaseService _databaseService;
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// 初始化SQLite控制器
        /// </summary>
        /// <param name="databaseService">SQLite数据库服务</param>
        /// <param name="dbContext">AppDbContext</param>
        public SqliteController(SqliteDatabaseService databaseService, AppDbContext dbContext)
        {
            _databaseService = databaseService;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpGet("test")]
        public async Task<IActionResult> TestConnection(CancellationToken cancellationToken = default)
        {
            var result = await _databaseService.ExecuteAsync(
                "CREATE TABLE IF NOT EXISTS Test (Id INTEGER PRIMARY KEY, Name TEXT)",
                cancellationToken: cancellationToken);
            return Ok(new { message = "Connection test successful" });
        }

        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>客户列表</returns>
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken = default)
        {
            var customers = await _databaseService.QueryAsync<Customer>(
                "SELECT * FROM Customers",
                cancellationToken: cancellationToken);
            return Ok(customers);
        }

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="customer">客户信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>操作结果</returns>
        [HttpPost("customers")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer, CancellationToken cancellationToken = default)
        {
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(customer);
        }
    }
}
