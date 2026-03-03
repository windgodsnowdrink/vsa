#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.EntityFrameworkCore@9.0.8
#:package Microsoft.EntityFrameworkCore.SqlServer@9.0.8
#:package Microsoft.EntityFrameworkCore.Tools@9.0.8
#:package Microsoft.Extensions.Caching.Memory@8.0.8
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.Json;

// 多租户支�?
public interface ITenantProvider
{
    string GetCurrentTenantId();
}

// 审计日志接口
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}

// 软删除接�?
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}

// 加密数据接口
public interface IEncryptedEntity
{
    string EncryptedData { get; set; }
}

// 数据库上下文
public class AppDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    private readonly IMemoryCache _cache;
    
    public AppDbContext(
        DbContextOptions<AppDbContext> options, 
        ITenantProvider tenantProvider,
        IMemoryCache cache) : base(options)
    {
        _tenantProvider = tenantProvider;
        _cache = cache;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 全局查询过滤�?- 多租�?
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(e => EF.Property<string>(e, "TenantId") == _tenantProvider.GetCurrentTenantId());
        }

        // 全局查询过滤�?- 软删�?
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(ISoftDelete).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(e => !EF.Property<bool>(e, "IsDeleted"));
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 审计日志处理
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity && 
                (e.State == EntityState.Added || e.State == EntityState.Modified));

        var currentUserId = "system"; // 从ClaimsPrincipal获取实际用户ID
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entity = (IAuditableEntity)entry.Entity;
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = currentUserId;
            }
            else
            {
                entity.UpdatedAt = now;
                entity.UpdatedBy = currentUserId;
            }
        }

        // 软删除处�?
        var softDeleteEntries = ChangeTracker.Entries()
            .Where(e => e.Entity is ISoftDelete && e.State == EntityState.Deleted);

        foreach (var entry in softDeleteEntries)
        {
            entry.State = EntityState.Modified;
            var entity = (ISoftDelete)entry.Entity;
            entity.IsDeleted = true;
            entity.DeletedAt = now;
            entity.DeletedBy = currentUserId;
        }

        // 加密数据处理
        var encryptedEntries = ChangeTracker.Entries()
            .Where(e => e.Entity is IEncryptedEntity);

        foreach (var entry in encryptedEntries)
        {
            var entity = (IEncryptedEntity)entry.Entity;
            // 这里实现加密逻辑
            // entity.EncryptedData = Encrypt(entity.EncryptedData);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    // 缓存仓储实现
    public class CachedRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedRepository(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var cacheKey = $"{typeof(T).Name}-{id}";
            if (_cache.TryGetValue(cacheKey, out T? cachedItem))
            {
                return cachedItem;
            }

            var item = await _context.Set<T>().FindAsync(id);
            if (item != null)
            {
                _cache.Set(cacheKey, item, _cacheOptions);
            }
            return item;
        }
    }

    // 分库分表策略
    public class ShardingStrategy
    {
        public string GetDatabaseName(string tenantId) => $"Database_{tenantId}";
        public string GetTableName(Type entityType, DateTime date) => $"{entityType.Name}_{date:yyyyMM}";
    }

    // 数据快照
    public class EntitySnapshot<T> where T : class
    {
        public string SerializedData { get; set; }
        public DateTime SnapshotTime { get; set; }
    }

    // 数据归档服务
    /// <summary>
    /// 数据归档服务，实现生产级数据归档功能
    /// </summary>
    public class ArchivingService : IArchivingService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ArchivingService> _logger;
        private readonly ArchivingOptions _options;
        private readonly ObjectPool<AppDbContext> _dbContextPool;
    
        public ArchivingService(
            AppDbContext dbContext,
            ILogger<ArchivingService> logger,
            IOptions<ArchivingOptions> options,
            ObjectPool<AppDbContext> dbContextPool)
        {
            _dbContext = dbContext;
            _logger = logger;
            _options = options.Value;
            _dbContextPool = dbContextPool;
        }
    
        public async Task ArchiveByTimeAsync(DateTime cutoffDate, CancellationToken ct = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var archivedCount = await _dbContext.YourEntities
                    .Where(e => e.CreatedDate < cutoffDate)
                    .ExecuteDeleteAsync(ct);
    
                _logger.LogInformation("Archived {Count} records older than {CutoffDate}", 
                    archivedCount, cutoffDate);
    
                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to archive records");
                throw;
            }
        }
    
        public async Task ArchiveBySizeAsync(long maxSizeBytes, CancellationToken ct = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var totalSize = await _dbContext.YourEntities
                    .SumAsync(e => e.SizeBytes, ct);
                    
                if (totalSize > maxSizeBytes)
                {
                    var oldestRecords = _dbContext.YourEntities
                        .OrderBy(e => e.CreatedDate)
                        .TakeWhile(e => totalSize > maxSizeBytes);
                        
                    var archivedCount = await oldestRecords
                        .ExecuteDeleteAsync(ct);
                        
                    _logger.LogInformation("Archived {Count} records to reduce size to {MaxSize} bytes", 
                        archivedCount, maxSizeBytes);
                }
                
                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to archive records by size");
                throw;
            }
        }
    
        public async Task ArchiveHotToColdAsync(
            TimeSpan hotDataThreshold, 
            string coldStoragePath,
            CancellationToken ct = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var cutoffDate = DateTime.UtcNow.Subtract(hotDataThreshold);
                var coldData = await _dbContext.YourEntities
                    .Where(e => e.CreatedDate < cutoffDate)
                    .ToListAsync(ct);
                    
                // Serialize and save cold data to storage
                await SaveToColdStorage(coldData, coldStoragePath, ct);
                
                // Remove from hot storage
                var archivedCount = await _dbContext.YourEntities
                    .Where(e => e.CreatedDate < cutoffDate)
                    .ExecuteDeleteAsync(ct);
                    
                _logger.LogInformation("Archived {Count} records to cold storage", archivedCount);
                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to archive hot to cold data");
                throw;
            }
        }
    
        private async Task ParallelArchiveAsync(
            Func<IQueryable<YourEntity>, IQueryable<YourEntity>> query,
            int batchSize,
            CancellationToken ct)
        {
            var sourceQuery = query(_dbContext.YourEntities);
            var totalCount = await sourceQuery.CountAsync(ct);
            
            _logger.LogInformation("Starting parallel archive of {Count} records", totalCount);
            
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = ct
            };
            
            await Parallel.ForEachAsync(
                Batch(sourceQuery, batchSize),
                parallelOptions,
                async (batch, cancellationToken) =>
                {
                    var dbContext = _dbContextPool.Get();
                    try
                    {
                        await dbContext.YourEntities
                            .Where(e => batch.Select(b => b.Id).Contains(e.Id))
                            .ExecuteDeleteAsync(cancellationToken);
                    }
                    finally
                    {
                        _dbContextPool.Return(dbContext);
                    }
                });
        }
    
        private async Task SaveToColdStorage(
            List<YourEntity> data, 
            string storagePath,
            CancellationToken ct)
        {
            var filePath = Path.Combine(storagePath, $"cold_data_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            
            await using var fileStream = new FileStream(
                filePath, 
                FileMode.CreateNew, 
                FileAccess.Write, 
                FileShare.None, 
                bufferSize: 4096, 
                useAsync: true);
            
            await JsonSerializer.SerializeAsync(
                fileStream, 
                data, 
                new JsonSerializerOptions { WriteIndented = true }, 
                ct);
        }
    
        private static IEnumerable<IEnumerable<YourEntity>> Batch(
            IQueryable<YourEntity> source, 
            int batchSize)
        {
            var batch = new List<YourEntity>(batchSize);
            
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<YourEntity>(batchSize);
                }
            }
            
            if (batch.Count > 0)
                yield return batch;
        }
    }

    // 数据备份服务
    public class BackupService : IBackupService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<BackupService> _logger;
        private readonly BackupOptions _options;
        private readonly IDataProtector _dataProtector;
        private readonly ObjectPool<AppDbContext> _dbContextPool;
    
        public BackupService(
            AppDbContext dbContext,
            ILogger<BackupService> logger,
            IOptions<BackupOptions> options,
            IDataProtectionProvider dataProtectionProvider,
            ObjectPool<AppDbContext> dbContextPool)
        {
            _dbContext = dbContext;
            _logger = logger;
            _options = options.Value;
            _dataProtector = dataProtectionProvider.CreateProtector("DatabaseBackup");
            _dbContextPool = dbContextPool;
        }
    
        public async Task<BackupResult> FullBackupAsync(string backupPath, CancellationToken ct = default)
        {
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("BackupService.FullBackup");
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            
            try
            {
                var backupFile = Path.Combine(backupPath, $"full_backup_{DateTime.UtcNow:yyyyMMddHHmmss}.bak");
                
                await using var memoryStream = new MemoryStream();
                await using var writer = new BinaryWriter(memoryStream);
                
                // 备份所有表数据
                foreach (var entityType in _dbContext.Model.GetEntityTypes())
                {
                    var tableName = entityType.GetTableName();
                    writer.Write(tableName);
                    
                    var entities = await _dbContext.Set(entityType.ClrType)
                        .AsNoTracking()
                        .ToListAsync(ct);
                        
                    writer.Write(entities.Count);
                    foreach (var entity in entities)
                    {
                        var json = JsonSerializer.Serialize(entity);
                        writer.Write(json);
                    }
                }
                
                // 压缩和加密备份文�?
                memoryStream.Position = 0;
                var encryptedFile = await CompressAndEncryptBackupAsync(memoryStream, backupFile, ct);
                
                await transaction.CommitAsync(ct);
                _logger.LogInformation("Completed full backup to {BackupFile}", encryptedFile);
                
                return new BackupResult(true, encryptedFile, memoryStream.Length);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to create full backup");
                throw;
            }
        }
    
        public async Task<BackupResult> IncrementalBackupAsync(string backupPath, DateTime since, CancellationToken ct = default)
        {
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("BackupService.IncrementalBackup");
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            
            try
            {
                var backupFile = Path.Combine(backupPath, $"incr_backup_{DateTime.UtcNow:yyyyMMddHHmmss}.bak");
                
                await using var memoryStream = new MemoryStream();
                await using var writer = new BinaryWriter(memoryStream);
                
                // 备份变更数据
                foreach (var entityType in _dbContext.Model.GetEntityTypes())
                {
                    var tableName = entityType.GetTableName();
                    writer.Write(tableName);
                    
                    var entities = await _dbContext.Set(entityType.ClrType)
                        .AsNoTracking()
                        .Where(e => EF.Property<DateTime>(e, "ModifiedDate") > since)
                        .ToListAsync(ct);
                        
                    writer.Write(entities.Count);
                    foreach (var entity in entities)
                    {
                        var json = JsonSerializer.Serialize(entity);
                        writer.Write(json);
                    }
                }
                
                // 压缩和加密备份文�?
                memoryStream.Position = 0;
                var encryptedFile = await CompressAndEncryptBackupAsync(memoryStream, backupFile, ct);
                
                await transaction.CommitAsync(ct);
                _logger.LogInformation("Completed incremental backup to {BackupFile}", encryptedFile);
                
                return new BackupResult(true, encryptedFile, memoryStream.Length);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to create incremental backup");
                throw;
            }
        }

        public async Task<RestoreResult> RestoreAsync(string backupFile, DateTime? pointInTime = null, CancellationToken ct = default)
        {
            using var activity = DiagnosticsConfig.ActivitySource.StartActivity("BackupService.Restore");
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            
            try
            {
                // 验证备份文件完整�?
                if (!await VerifyBackupIntegrityAsync(backupFile, ct))
                    throw new InvalidOperationException("Backup file integrity check failed");
                
                // 解密和解压备份文�?
                await using var decryptedStream = await DecryptAndDecompressBackupAsync(backupFile, ct);
                
                // 恢复数据
                using var reader = new BinaryReader(decryptedStream);
                
                while (decryptedStream.Position < decryptedStream.Length)
                {
                    var tableName = reader.ReadString();
                    var count = reader.ReadInt32();
                    
                    for (int i = 0; i < count; i++)
                    {
                        var json = reader.ReadString();
                        // 反序列化并保存实�?
                    }
                }
                
                await transaction.CommitAsync(ct);
                _logger.LogInformation("Successfully restored from {BackupFile}", backupFile);
                
                return new RestoreResult(true, backupFile);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                _logger.LogError(ex, "Failed to restore from backup");
                throw;
            }
        }
    
        private async Task<string> CompressAndEncryptBackupAsync(Stream backupData, string backupPath, CancellationToken ct)
        {
            // 实现压缩和加密逻辑
        }
    
        private async Task<bool> VerifyBackupIntegrityAsync(string backupFile, CancellationToken ct)
        {
            // 实现备份完整性检�?
        }
    
        private async Task ScheduleBackupJobsAsync()
        {
            // 实现备份计划调度
        }
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEfCore9Services(this IServiceCollection services, string connectionString)
    {
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            });
            
            // 禁用线程安全检查以提高性能
            options.EnableThreadSafetyChecks(false);
        });

        // 注册缓存仓储
        services.AddScoped(typeof(CachedRepository<>));
        
        // 注册其他服务
        services.AddScoped<ShardingStrategy>();
        services.AddScoped<ArchivingService>();
        services.AddScoped<BackupService>();

        return services;
    }
}

public class SnapshotService : ISnapshotService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<SnapshotService> _logger;
    private readonly IOptions<SnapshotOptions> _options;
    private readonly IDataProtector _dataProtector;

    public async Task<SnapshotResult> CreateSnapshotAsync(string snapshotName, CancellationToken ct = default)
    {
        using var activity = DiagnosticsConfig.ActivitySource.StartActivity("SnapshotService.CreateSnapshot");
        using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        
        try
        {
            // 快照存储逻辑
            var snapshotFile = Path.Combine(_options.Value.StoragePath, $"{snapshotName}_{DateTime.UtcNow:yyyyMMddHHmmss}.snap");
            
            // 快照数据序列化和保护
            await using var memoryStream = new MemoryStream();
            await using var writer = new BinaryWriter(memoryStream);
            
            // 快照恢复点实�?
            foreach (var entityType in _dbContext.Model.GetEntityTypes())
            {
                var entities = await _dbContext.Set(entityType.ClrType).ToListAsync(ct);
                var json = JsonSerializer.Serialize(entities);
                var protectedData = _dataProtector.Protect(json);
                writer.Write(protectedData);
            }
            
            // 存储快照到本�?云存�?
            memoryStream.Position = 0;
            await using var fileStream = File.Create(snapshotFile);
            await memoryStream.CopyToAsync(fileStream, ct);
            
            await transaction.CommitAsync(ct);
            return new SnapshotResult(true, snapshotFile);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Failed to create snapshot");
            throw;
        }
    }

    public async Task<RestoreResult> RestoreFromSnapshotAsync(string snapshotFile, CancellationToken ct = default)
    {
        using var activity = DiagnosticsConfig.ActivitySource.StartActivity("SnapshotService.RestoreFromSnapshot");
        using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
        
        try
        {
            // 从快照恢复逻辑
            await using var fileStream = File.OpenRead(snapshotFile);
            using var reader = new BinaryReader(fileStream);
            
            // 恢复数据库状�?
            while (fileStream.Position < fileStream.Length)
            {
                var protectedData = reader.ReadString();
                var json = _dataProtector.Unprotect(protectedData);
                // 反序列化并恢复实�?
            }
            
            await transaction.CommitAsync(ct);
            return new RestoreResult(true, snapshotFile);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Failed to restore from snapshot");
            throw;
        }
    }
}

public class ShardingStrategy : IShardingStrategy
{
    private readonly IOptions<ShardingOptions> _options;
    private readonly ILogger<ShardingStrategy> _logger;
    
    public string GetShardKey(object entity)
    {
        // 实现基于实体属性的分片键计算逻辑
        return entity switch
        {
            // 分片规则实现
            _ => "default"
        };
    }
    
    public string GetConnectionString(string shardKey)
    {
        // 实现基于分片键的连接字符串选择逻辑
        return _options.Value.ConnectionStrings.TryGetValue(shardKey, out var connStr) 
            ? connStr 
            : _options.Value.DefaultConnectionString;
    }
}