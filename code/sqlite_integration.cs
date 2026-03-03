#:sdk Microsoft.NET.Sdk
#:package Microsoft.Data.Sqlite.Core@8.0.0
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:package System.Buffers@4.5.1
#:package System.Memory@4.5.5
#:package System.Threading.Channels@7.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Data;
using System.Threading.Channels;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SqliteIntegration
{
    /// <summary>
    /// EFCore集成DbContext
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

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
        public string ConnectionString { get; set; } = "Data Source=:memory:";
        public int PoolSize { get; set; } = 16;
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

        public SqliteConnectionPool(IOptions<SqliteOptions> options, ILogger<SqliteConnectionPool> logger)
        {
            _options = options.Value;
            _logger = logger;
            _pool = Channel.CreateBounded<SqliteConnection>(_options.PoolSize);

            // 初始化连接池
            for (int i = 0; i < _options.PoolSize; i++)
            {
                var conn = new SqliteConnection(_options.ConnectionString);
                conn.Open();
                _pool.Writer.TryWrite(conn);
            }
        }

        public async ValueTask<SqliteConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = await _pool.Reader.ReadAsync(cancellationToken);
            return connection;
        }

        public void ReturnConnection(SqliteConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            _pool.Writer.TryWrite(connection);
        }

        public void Dispose()
        {
            while (_pool.Reader.TryRead(out var connection))
            {
                connection.Dispose();
            }
        }
    }

    /// <summary>
    /// SQLite数据库服务扩展
    /// </summary>
    public static class SqliteServiceExtensions
    {
        public static IServiceCollection AddSqliteServices(this IServiceCollection services, Action<SqliteOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddDbContextPool<AppDbContext>(options => 
        {
            options.UseSqlite(services.BuildServiceProvider().GetRequiredService<IOptions<SqliteOptions>>().Value.ConnectionString);
            options.EnableThreadSafetyChecks(false);
            options.EnableDetailedErrors();
        }, poolSize: 128);
        services.AddSingleton<SqliteConnectionPool>();
        services.AddScoped<SqliteTransactionScope>();
        return services;
    }
    }

    /// <summary>
    /// SQLite事务范围
    /// </summary>
    public class SqliteTransactionScope : IDisposable
    {
        private readonly SqliteConnectionPool _pool;
        private SqliteConnection _connection;
        private SqliteTransaction _transaction;

        public SqliteTransactionScope(SqliteConnectionPool pool)
        {
            _pool = pool;
            _connection = _pool.GetConnectionAsync().GetAwaiter().GetResult();
            _transaction = _connection.BeginTransaction();
        }

        public SqliteCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return command;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _pool.ReturnConnection(_connection);
        }
    }

    /// <summary>
    /// 高性能批量插入
    /// </summary>
    public class SqliteBulkInserter
    {
        private readonly SqliteConnectionPool _pool;
        private readonly MemoryPool<byte> _memoryPool;

        public SqliteBulkInserter(SqliteConnectionPool pool)
        {
            _pool = pool;
            _memoryPool = MemoryPool<byte>.Shared;
        }

        public async Task BulkInsertAsync(string tableName, IEnumerable<IDictionary<string, object>> data)
        {
            using var connection = await _pool.GetConnectionAsync();
            using var transaction = connection.BeginTransaction();
            
            // 使用Span和MemoryPool优化内存分配
            var buffer = _memoryPool.Rent(4096);
            try
            {
                // 批量插入逻辑
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                // 实现细节省略...
                
                await transaction.CommitAsync();
            }
            finally
            {
                buffer.Dispose();
            }
        }
    }

    /// <summary>
    /// SQLite数据库示例服务
    /// </summary>
    /// <summary>
    /// 高性能EF Core批量操作
    /// </summary>
    public class EfCoreBulkOperations
    {
        private readonly AppDbContext _dbContext;

        public EfCoreBulkOperations(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BulkInsertAsync(IEnumerable<Customer> customers)
        {
            await _dbContext.BulkInsertAsync(customers, options => 
            {
                options.BatchSize = 1000;
                options.InsertIfNotExists = true;
            });
        }

        public async Task BulkUpdateAsync(IEnumerable<Customer> customers)
        {
            await _dbContext.BulkUpdateAsync(customers, options => 
            {
                options.BatchSize = 1000;
                options.PropertiesToInclude = new List<string> { nameof(Customer.Name), nameof(Customer.Email) };
            });
        }
    }

    public class SqliteDbExampleService
    {
        private readonly SqliteConnectionPool _connectionPool;

        public SqliteDbExampleService(SqliteConnectionPool connectionPool)
        {
            _connectionPool = connectionPool;
            InitializeDatabaseAsync().Wait();
        }

        private async Task InitializeDatabaseAsync()
        {
            using var scope = new SqliteTransactionScope(_connectionPool);
            var command = scope.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Email TEXT UNIQUE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );
            """;
            await command.ExecuteNonQueryAsync();
            scope.Commit();
        }

        public async Task<int> CreateCustomerAsync(string name, string email)
        {
            using var scope = new SqliteTransactionScope(_connectionPool);
            var command = scope.CreateCommand();
            command.CommandText = "INSERT INTO Customers (Name, Email) VALUES (@Name, @Email); SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Email", email);
            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            scope.Commit();
            return id;
        }

        public async Task<Customer?> GetCustomerAsync(int id)
        {
            using var connection = await _connectionPool.GetConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Customers WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Customer
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                };
            }
            return null;
        }

        public async Task UpdateCustomerAsync(int id, string name, string email)
        {
            using var scope = new SqliteTransactionScope(_connectionPool);
            var command = scope.CreateCommand();
            command.CommandText = "UPDATE Customers SET Name = @Name, Email = @Email WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Email", email);
            await command.ExecuteNonQueryAsync();
            scope.Commit();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            using var scope = new SqliteTransactionScope(_connectionPool);
            var command = scope.CreateCommand();
            command.CommandText = "DELETE FROM Customers WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            await command.ExecuteNonQueryAsync();
            scope.Commit();
        }

        public async Task<IEnumerable<Customer>> ListCustomersAsync()
        {
            var customers = new List<Customer>();
            using var connection = await _connectionPool.GetConnectionAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Customers ORDER BY CreatedAt DESC";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                customers.Add(new Customer
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                });
            }
            return customers;
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}