#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.EntityFrameworkCore@10.0.0
#:package Microsoft.EntityFrameworkCore.InMemory@10.0.0
#:package Microsoft.EntityFrameworkCore.Relational@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EFCore.AOT
{
    /// <summary>
    /// EFCore 命令类型枚举
    /// </summary>
    public enum EFCoreCommandType
    {
        /// <summary>
        /// 数据库迁移
        /// </summary>
        Migrate,
        /// <summary>
        /// 创建数据库
        /// </summary>
        CreateDatabase,
        /// <summary>
        /// 删除数据库
        /// </summary>
        DropDatabase,
        /// <summary>
        /// 查询数据
        /// </summary>
        Query,
        /// <summary>
        /// 插入数据
        /// </summary>
        Insert,
        /// <summary>
        /// 更新数据
        /// </summary>
        Update,
        /// <summary>
        /// 删除数据
        /// </summary>
        Delete,
        /// <summary>
        /// 显示版本信息
        /// </summary>
        VersionInfo
    }

    /// <summary>
    /// EFCore 选项配置
    /// </summary>
    public class EFCoreOptions
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; } = "InMemoryDatabase=efcore-aot-db";        
        
        /// <summary>
        /// 数据库提供程序类型
        /// </summary>
        public string ProviderType { get; set; } = "InMemory";
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 查询超时时间（秒）
        /// </summary>
        public int QueryTimeoutSeconds { get; set; } = 30;
        
        /// <summary>
        /// 批量操作大小
        /// </summary>
        public int BatchSize { get; set; } = 100;
    }

    /// <summary>
    /// EFCore 命令结果
    /// </summary>
    public class EFCoreCommandResult
    {
        /// <summary>
        /// 命令是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public EFCoreCommandType CommandType { get; set; }
        
        /// <summary>
        /// 结果数据
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// 受影响的行数
        /// </summary>
        public int RowsAffected { get; set; }
    }

    /// <summary>
    /// 示例实体：用户
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        /// <summary>
        /// 电子邮件
        /// </summary>
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// 示例实体：订单
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; }
        
        /// <summary>
        /// 订单金额
        /// </summary>
        [Required]
        public decimal Amount { get; set; }
        
        /// <summary>
        /// 订单状态
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 导航属性：用户
        /// </summary>
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    /// <summary>
    /// EFCore 数据库上下文
    /// </summary>
    public class EFCoreAotContext : DbContext
    {
        private readonly EFCoreOptions _options;
        
        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<User> Users { get; set; }
        
        /// <summary>
        /// 订单表
        /// </summary>
        public DbSet<Order> Orders { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">EFCore 选项</param>
        public EFCoreAotContext(IOptions<EFCoreOptions> options) : base()
        {
            _options = options.Value;
        }
        
        /// <summary>
        /// 配置数据库连接
        /// </summary>
        /// <param name="optionsBuilder">选项构建器</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_options.ProviderType.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseInMemoryDatabase(_options.ConnectionString.Replace("InMemoryDatabase=", ""));
            }
            else if (_options.ProviderType.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlServer(_options.ConnectionString);
            }
            else if (_options.ProviderType.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlite(_options.ConnectionString);
            }
            
            if (_options.EnableDetailedLogging)
            {
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
            
            if (_options.EnablePerformanceMonitoring)
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.EnableDetailedErrors();
            }
        }
        
        /// <summary>
        /// 配置模型关系
        /// </summary>
        /// <param name="modelBuilder">模型构建器</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 User 实体
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            // 配置 Order 实体
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    /// <summary>
    /// EFCore 服务接口
    /// </summary>
    public interface IEFCoreService
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>命令结果</returns>
        Task<EFCoreCommandResult> ExecuteCommandAsync(EFCoreCommandType commandType, Dictionary<string, string>? parameters = null);
        
        /// <summary>
        /// 执行数据库迁移
        /// </summary>
        /// <returns>迁移结果</returns>
        Task<EFCoreCommandResult> MigrateDatabaseAsync();
        
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <returns>创建结果</returns>
        Task<EFCoreCommandResult> CreateDatabaseAsync();
        
        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <returns>删除结果</returns>
        Task<EFCoreCommandResult> DropDatabaseAsync();
        
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="queryType">查询类型</param>
        /// <param name="filter">过滤条件</param>
        /// <returns>查询结果</returns>
        Task<EFCoreCommandResult> QueryDataAsync(string queryType, string? filter = null);
        
        /// <summary>
        /// 插入示例数据
        /// </summary>
        /// <param name="count">插入数量</param>
        /// <returns>插入结果</returns>
        Task<EFCoreCommandResult> InsertSampleDataAsync(int count = 1);
        
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns>版本信息</returns>
        Task<EFCoreCommandResult> GetVersionInfoAsync();
    }

    /// <summary>
    /// EFCore 服务实现
    /// </summary>
    public class EFCoreService : IEFCoreService
    {
        private readonly EFCoreOptions _options;
        private readonly ILogger<EFCoreService> _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">EFCore 选项</param>
        /// <param name="logger">日志记录器</param>
        /// <param name="serviceProvider">服务提供器</param>
        public EFCoreService(IOptions<EFCoreOptions> options, ILogger<EFCoreService> logger, IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public async Task<EFCoreCommandResult> ExecuteCommandAsync(EFCoreCommandType commandType, Dictionary<string, string>? parameters = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EFCoreCommandResult
            {
                CommandType = commandType
            };

            try
            {
                switch (commandType)
                {
                    case EFCoreCommandType.Migrate:
                        result = await MigrateDatabaseAsync();
                        break;
                    
                    case EFCoreCommandType.CreateDatabase:
                        result = await CreateDatabaseAsync();
                        break;
                    
                    case EFCoreCommandType.DropDatabase:
                        result = await DropDatabaseAsync();
                        break;
                    
                    case EFCoreCommandType.Query:
                        string queryType = parameters?.ContainsKey("type") == true ? parameters["type"] : "users";
                        string? filter = parameters?.ContainsKey("filter") == true ? parameters["filter"] : null;
                        result = await QueryDataAsync(queryType, filter);
                        break;
                    
                    case EFCoreCommandType.Insert:
                        int count = parameters?.ContainsKey("count") == true ? int.Parse(parameters["count"]) : 1;
                        result = await InsertSampleDataAsync(count);
                        break;
                    
                    case EFCoreCommandType.Update:
                        // 模拟更新操作
                        result.Success = true;
                        result.Results.Add("Update operation is not implemented in this example");
                        break;
                    
                    case EFCoreCommandType.Delete:
                        // 模拟删除操作
                        result.Success = true;
                        result.Results.Add("Delete operation is not implemented in this example");
                        break;
                    
                    case EFCoreCommandType.VersionInfo:
                        result = await GetVersionInfoAsync();
                        break;
                    
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"Unknown command type: {commandType}";
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing command: {CommandType}", commandType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EFCoreCommandResult> MigrateDatabaseAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EFCoreCommandResult
            {
                CommandType = EFCoreCommandType.Migrate
            };

            try
            {
                _logger.LogInformation("Migrating database...");
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EFCoreAotContext>();
                    await context.Database.EnsureCreatedAsync();
                }
                
                result.Success = true;
                result.Results.Add("Database migrated successfully");
                result.Results.Add($"Provider: {_options.ProviderType}");
                result.Results.Add($"Connection: {_options.ConnectionString}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error migrating database");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EFCoreCommandResult> CreateDatabaseAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EFCoreCommandResult
            {
                CommandType = EFCoreCommandType.CreateDatabase
            };

            try
            {
                _logger.LogInformation("Creating database...");
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EFCoreAotContext>();
                    bool created = await context.Database.EnsureCreatedAsync();
                    
                    if (created)
                    {
                        result.Results.Add("Database created successfully");
                    }
                    else
                    {
                        result.Results.Add("Database already exists");
                    }
                }
                
                result.Success = true;
                result.Results.Add($"Provider: {_options.ProviderType}");
                result.Results.Add($"Connection: {_options.ConnectionString}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating database");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EFCoreCommandResult> DropDatabaseAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EFCoreCommandResult
            {
                CommandType = EFCoreCommandType.DropDatabase
            };

            try
            {
                _logger.LogInformation("Dropping database...");
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EFCoreAotContext>();
                    bool dropped = await context.Database.EnsureDeletedAsync();
                    
                    if (dropped)
                    {
                        result.Results.Add("Database dropped successfully");
                    }
                    else
                    {
                        result.Results.Add("Database does not exist");
                    }
                }
                
                result.Success = true;
                result.Results.Add($"Provider: {_options.ProviderType}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dropping database");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EFCoreCommandResult> QueryDataAsync(string queryType, string? filter = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EFCoreCommandResult
            {
                CommandType = EFCoreCommandType.Query
            };

            try
            {
                _logger.LogInformation("Querying data...");
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EFCoreAotContext>();
                    
                    if (queryType.Equals("users", StringComparison.OrdinalIgnoreCase))
                    {
                        var users = await context.Users.OrderBy(u => u.Id).ToListAsync();
                        result.Results.Add($"Found {users.Count} users:");
                        foreach (var user in users)
                        {
                            result.Results.Add($"  {user.Id}: {user.Username} ({user.Email})");
                        }
                        result.RowsAffected = users.Count;
                    }
                    else if (queryType.Equals("orders", StringComparison.OrdinalIgnoreCase))
                    {
                        var orders = await context.Orders.Include(o => o.User).OrderBy(o => o.Id).ToListAsync();
                        result.Results.Add($"Found {orders.Count} orders:");
                        foreach (var order in orders)
                        {
                            result.Results.Add($"  {order.Id}: User={order.User.Username}, Amount={order.Amount:C}, Status={order.Status}");
                        }
                        result.RowsAffected = orders.Count;
                    }
                    else
                    {
                        result.Results.Add($"Unknown query type: {queryType}");
                    }
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying data");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<EFCoreCommandResult> InsertSampleDataAsync(int count = 1)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EFCoreCommandResult
            {
                CommandType = EFCoreCommandType.Insert
            };

            try
            {
                _logger.LogInformation("Inserting sample data...");
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EFCoreAotContext>();
                    
                    for (int i = 0; i < count; i++)
                    {
                        // 创建用户
                        var user = new User
                        {
                            Username = $"user{i+1}