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

// 澶氱鎴锋敮鎸?
public interface ITenantProvider
{
    string GetCurrentTenantId();
}

// 瀹¤鏃ュ織鎺ュ彛
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
}

// 杞垹闄ゆ帴鍙?
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}

// 鍔犲瘑鏁版嵁鎺ュ彛
public interface IEncryptedEntity
{
    string EncryptedData { get; set; }
}

// 鏁版嵁搴撲笂涓嬫枃
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
        // 鍏ㄥ眬鏌ヨ杩囨护鍣?- 澶氱鎴?
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(e => EF.Property<string>(e, "TenantId") == _tenantProvider.GetCurrentTenantId());
        }

        // 鍏ㄥ眬鏌ヨ杩囨护鍣?- 杞垹闄?
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
        // 瀹¤鏃ュ織澶勭悊
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditableEntity && 
                (e.State == EntityState.Added || e.State == EntityState.Modified));

        var currentUserId = "system"; // 浠嶤laimsPrincipal鑾峰彇瀹為檯鐢ㄦ埛ID
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

        // 杞垹闄ゅ鐞?
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

        // 鍔犲瘑鏁版嵁澶勭悊
        var encryptedEntries = ChangeTracker.Entries()
            .Where(e => e.Entity is IEncryptedEntity);

        foreach (var entry in encryptedEntries)
        {
            var entity = (IEncryptedEntity)entry.Entity;
            // 杩欓噷瀹炵幇鍔犲瘑閫昏緫
            // entity.EncryptedData = Encrypt(entity.EncryptedData);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    // 缂撳瓨浠撳偍瀹炵幇
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

    // 鍒嗗簱鍒嗚〃绛栫暐
    public class ShardingStrategy
    {
        public string GetDatabaseName(string tenantId) => $"Database_{tenantId}";
        public string GetTableName(Type entityType, DateTime date) => $"{entityType.Name}_{date:yyyyMM}";
    }

    // 鏁版嵁蹇収
    public class EntitySnapshot<T> where T : class
    {
        public string SerializedData { get; set; }
        public DateTime SnapshotTime { get; set; }
    }

    // 鏁版嵁褰掓.鏈嶅姟
    /// <summary>
    /// 鏁版嵁褰掓.鏈嶅姟锛屽疄鐜扮敓浜х骇鏁版嵁褰掓.鍔熻兘
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

    // 鏁版嵁澶囦唤鏈嶅姟
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
                
                // 澶囦唤鎵€鏈夎〃鏁版嵁
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
                
                // 鍘嬬缉鍜屽姞瀵嗗浠芥枃浠?
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
                
                // 澶囦唤鍙樻洿鏁版嵁
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
                
                // 鍘嬬缉鍜屽姞瀵嗗浠芥枃浠?
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
                // 楠岃瘉澶囦唤鏂囦欢瀹屾暣鎬?
                if (!await VerifyBackupIntegrityAsync(backupFile, ct))
                    throw new InvalidOperationException("Backup file integrity check failed");
                
                // 瑙ｅ瘑鍜岃В鍘嬪浠芥枃浠?
                await using var decryptedStream = await DecryptAndDecompressBackupAsync(backupFile, ct);
                
                // 鎭㈠鏁版嵁
                using var reader = new BinaryReader(decryptedStream);
                
                while (decryptedStream.Position < decryptedStream.Length)
                {
                    var tableName = reader.ReadString();
                    var count = reader.ReadInt32();
                    
                    for (int i = 0; i < count; i++)
                    {
                        var json = reader.ReadString();
                        // 鍙嶅簭鍒楀寲骞朵繚瀛樺疄浣?
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
            // 瀹炵幇鍘嬬缉鍜屽姞瀵嗛€昏緫
        }
    
        private async Task<bool> VerifyBackupIntegrityAsync(string backupFile, CancellationToken ct)
        {
            // 瀹炵幇澶囦唤瀹屾暣鎬ф鏌?
        }
    
        private async Task ScheduleBackupJobsAsync()
        {
            // 瀹炵幇澶囦唤璁″垝璋冨害
        }
    }
}

// DI鎵╁睍鏂规硶
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
            
            // 绂佺敤绾跨▼瀹夊叏妫€鏌ヤ互鎻愰珮鎬ц兘
            options.EnableThreadSafetyChecks(false);
        });

        // 娉ㄥ唽缂撳瓨浠撳偍
        services.AddScoped(typeof(CachedRepository<>));
        
        // 娉ㄥ唽鍏朵粬鏈嶅姟
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
            // 蹇収瀛樺偍閫昏緫
            var snapshotFile = Path.Combine(_options.Value.StoragePath, $"{snapshotName}_{DateTime.UtcNow:yyyyMMddHHmmss}.snap");
            
            // 蹇収鏁版嵁搴忓垪鍖栧拰淇濇姢
            await using var memoryStream = new MemoryStream();
            await using var writer = new BinaryWriter(memoryStream);
            
            // 蹇収鎭㈠鐐瑰疄鐜?
            foreach (var entityType in _dbContext.Model.GetEntityTypes())
            {
                var entities = await _dbContext.Set(entityType.ClrType).ToListAsync(ct);
                var json = JsonSerializer.Serialize(entities);
                var protectedData = _dataProtector.Protect(json);
                writer.Write(protectedData);
            }
            
            // 瀛樺偍蹇収鍒版湰鍦?浜戝瓨鍌?
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
            // 浠庡揩鐓ф仮澶嶉€昏緫
            await using var fileStream = File.OpenRead(snapshotFile);
            using var reader = new BinaryReader(fileStream);
            
            // 鎭㈠鏁版嵁搴撶姸鎬?
            while (fileStream.Position < fileStream.Length)
            {
                var protectedData = reader.ReadString();
                var json = _dataProtector.Unprotect(protectedData);
                // 鍙嶅簭鍒楀寲骞舵仮澶嶅疄浣?
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
        // 瀹炵幇鍩轰簬瀹炰綋灞炴€х殑鍒嗙墖閿绠楅€昏緫
        return entity switch
        {
            // 鍒嗙墖瑙勫垯瀹炵幇
            _ => "default"
        };
    }
    
    public string GetConnectionString(string shardKey)
    {
        // 瀹炵幇鍩轰簬鍒嗙墖閿殑杩炴帴瀛楃涓查€夋嫨閫昏緫
        return _options.Value.ConnectionStrings.TryGetValue(shardKey, out var connStr) 
            ? connStr 
            : _options.Value.DefaultConnectionString;
    }
}