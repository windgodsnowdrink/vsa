#:sdk Microsoft.NET.Sdk
#:package Microsoft.Data.Sqlite@8.0.0
#:package Prometheus.Net.AspNetCore@8.0.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Prometheus;

public record SyncItem(string Id, string Data, string ETag, DateTimeOffset LastModified);

public class OfflineSyncService : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SyncConflictResolver _conflictResolver;
    private readonly ILogger<OfflineSyncService> _logger;

    public OfflineSyncService(string dbPath, SyncConflictResolver conflictResolver, ILogger<OfflineSyncService> logger)
    {
        _connection = new SqliteConnection($"Data Source={dbPath}");
        _connection.Open();
        _conflictResolver = conflictResolver;
        _logger = logger;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS SyncItems (
                Id TEXT PRIMARY KEY,
                Data TEXT NOT NULL,
                ETag TEXT NOT NULL,
                LastModified TEXT NOT NULL,
                SyncStatus INTEGER DEFAULT 0
            );
            CREATE INDEX IF NOT EXISTS IX_SyncItems_LastModified ON SyncItems(LastModified);";
        command.ExecuteNonQuery();
    }

    public async Task AddOrUpdateItemAsync(SyncItem item, CancellationToken cancellationToken = default)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO SyncItems (Id, Data, ETag, LastModified)
            VALUES (@Id, @Data, @ETag, @LastModified)
            ON CONFLICT(Id) DO UPDATE SET
                Data = excluded.Data,
                ETag = excluded.ETag,
                LastModified = excluded.LastModified,
                SyncStatus = 1;";
        
        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Data", item.Data);
        command.Parameters.AddWithValue("@ETag", item.ETag);
        command.Parameters.AddWithValue("@LastModified", item.LastModified.ToString("o"));
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncItem>> GetPendingSyncItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = new List<SyncItem>();
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT Id, Data, ETag, LastModified FROM SyncItems WHERE SyncStatus = 1";
        
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            items.Add(new SyncItem(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                DateTimeOffset.Parse(reader.GetString(3))
            ));
        }
        
        return items;
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}

public class SyncConflictResolver
{
    public SyncItem Resolve(SyncItem local, SyncItem remote)
    {
        // 实现冲突解决策略
        return local.LastModified > remote.LastModified ? local : remote;
    }
}

public class DeltaSyncProvider
{
    private readonly Dictionary<string, DateTimeOffset> _changeTracker = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public void TrackChange(string id, DateTimeOffset timestamp)
    {
        _lock.EnterWriteLock();
        try
        {
            _changeTracker[id] = timestamp;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public IEnumerable<string> GetChangedItems(DateTimeOffset since)
    {
        _lock.EnterReadLock();
        try
        {
            return _changeTracker
                .Where(x => x.Value > since)
                .Select(x => x.Key)
                .ToList();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void ClearTrackedChanges(IEnumerable<string> ids)
    {
        _lock.EnterWriteLock();
        try
        {
            foreach (var id in ids)
            {
                _changeTracker.Remove(id);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}

public class SyncPerformanceMonitor
{
    private static readonly Counter _syncCounter = Metrics
        .CreateCounter("datasync_sync_operations_total", "Total sync operations");
    
    private static readonly Histogram _syncDuration = Metrics
        .CreateHistogram("datasync_sync_duration_seconds", "Sync operation duration");

    public IDisposable TrackSyncOperation()
    {
        _syncCounter.Inc();
        return _syncDuration.NewTimer();
    }

    public void RecordSyncError()
    {
        Metrics.CreateCounter("datasync_sync_errors_total", "Total sync errors").Inc();
    }

    public void RecordBatchSize(int batchSize)
    {
        Metrics.CreateGauge("datasync_batch_size", "Current sync batch size").Set(batchSize);
    }

    public void RecordQueueLength(int length)
    {
        Metrics.CreateGauge("datasync_queue_length", "Current sync queue length").Set(length);
    }
}

public class DataSyncService
{
    private readonly Channel<SyncItem> _syncQueue;
    private readonly IOptions<DataSyncOptions> _options;
    private readonly OfflineSyncService _offlineSync;
    private readonly DeltaSyncProvider _deltaSync;
    private readonly SyncPerformanceMonitor _perfMonitor;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(
        IOptions<DataSyncOptions> options,
        OfflineSyncService offlineSync,
        DeltaSyncProvider deltaSync,
        SyncPerformanceMonitor perfMonitor,
        ILogger<DataSyncService> logger)
    {
        _options = options;
        _offlineSync = offlineSync;
        _deltaSync = deltaSync;
        _perfMonitor = perfMonitor;
        _logger = logger;
        _syncQueue = Channel.CreateBounded<SyncItem>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
    }

    public async Task StartSyncAsync(CancellationToken cancellationToken = default)
    {
        using var _ = _perfMonitor.TrackSyncOperation();
        
        // 获取本地待同步项
        var pendingItems = await _offlineSync.GetPendingSyncItemsAsync(cancellationToken);
        
        // 批量处理
        var batch = new List<SyncItem>();
        foreach (var item in pendingItems)
        {
            batch.Add(item);
            if (batch.Count >= _options.Value.BatchSize)
            {
                await ProcessBatchAsync(batch, cancellationToken);
                batch.Clear();
            }
        }
        
        // 处理剩余项
        if (batch.Any())
        {
            await ProcessBatchAsync(batch, cancellationToken);
        }
    }

    private async Task ProcessBatchAsync(List<SyncItem> batch, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var item in batch)
            {
                await _syncQueue.Writer.WriteAsync(item, cancellationToken);
            }
            
            _perfMonitor.RecordBatchSize(batch.Count);
            _perfMonitor.RecordQueueLength(_syncQueue.Reader.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process sync batch");
            _perfMonitor.RecordSyncError();
            throw;
        }
    }

    public async Task EnqueueChangesAsync(IEnumerable<SyncItem> changes, CancellationToken cancellationToken = default)
    {
        foreach (var item in changes)
        {
            await _syncQueue.Writer.WriteAsync(item, cancellationToken);
            _deltaSync.TrackChange(item.Id, item.LastModified);
        }
    }
}

public static class DataSyncExtensions
{
    public static IServiceCollection AddDataSyncServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataSyncOptions>(configuration.GetSection("DataSync"));
        
        services.AddSingleton<OfflineSyncService>();
        services.AddSingleton<SyncConflictResolver>();
        services.AddSingleton<DeltaSyncProvider>();
        services.AddSingleton<SyncPerformanceMonitor>();
        services.AddSingleton<DataSyncService>();
        services.AddHostedService<DataSyncBackgroundService>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
            
        return services;
    }
}
#:package Microsoft.Azure.Mobile.Client@4.2.1
#:package Microsoft.Azure.Mobile.Server@2.0.3
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.Azure.Mobile.Client;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace DataSyncDemo
{
    public class DataSyncOptions
    {
        public string ServiceUrl { get; set; } = "https://your-service.azurewebsites.net";
        public string DatabaseConnectionString { get; set; } = "Server=(localdb)\\mssqllocaldb;Database=SyncDB;Trusted_Connection=True;";
        public int MaxRetryAttempts { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);
        public int BatchSize { get; set; } = 1000;
    }

    public static class DataSyncExtensions
    {
        public static IServiceCollection AddDataSync(this IServiceCollection services, Action<DataSyncOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<DataSyncService>();
            services.AddHostedService<DataSyncBackgroundService>();
            return services;
        }
    }

    public class DataSyncService : IAsyncDisposable
    {
        private readonly MobileServiceClient _client;
        private readonly Channel<SyncItem> _syncChannel;
        private readonly IOptions<DataSyncOptions> _options;
        private readonly ILogger<DataSyncService> _logger;

        public DataSyncService(IOptions<DataSyncOptions> options, ILogger<DataSyncService> logger)
        {
            _options = options;
            _logger = logger;
            _client = new MobileServiceClient(_options.Value.ServiceUrl);
            _syncChannel = Channel.CreateBounded<SyncItem>(new BoundedChannelOptions(10000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });
        }

        public async ValueTask DisposeAsync()
        {
            _syncChannel.Writer.Complete();
            await Task.CompletedTask;
        }

        public async Task QueueForSyncAsync(SyncItem item, CancellationToken cancellationToken = default)
        {
            await _syncChannel.Writer.WriteAsync(item, cancellationToken);
        }

        public async Task ProcessSyncQueueAsync(CancellationToken cancellationToken = default)
        {
            await foreach (var item in _syncChannel.Reader.ReadAllAsync(cancellationToken))
            {
                await SyncItemWithRetryAsync(item, cancellationToken);
            }
        }

        private async Task SyncItemWithRetryAsync(SyncItem item, CancellationToken cancellationToken)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    await _client.GetSyncTable<SyncItem>().InsertAsync(item);
                    _logger.LogInformation("Successfully synced item {ItemId}", item.Id);
                    return;
                }
                catch (Exception ex) when (attempt < _options.Value.MaxRetryAttempts)
                {
                    attempt++;
                    _logger.LogWarning(ex, "Failed to sync item {ItemId} (attempt {Attempt})", item.Id, attempt);
                    await Task.Delay(_options.Value.RetryDelay, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync item {ItemId} after {Attempt} attempts", item.Id, attempt);
                    throw;
                }
            }
        }
    }

    public class DataSyncBackgroundService : BackgroundService
    {
        private readonly DataSyncService _syncService;

        public DataSyncBackgroundService(DataSyncService syncService)
        {
            _syncService = syncService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _syncService.ProcessSyncQueueAsync(stoppingToken);
        }
    }

    public class SyncItem : EntityData
    {
        public string Data { get; set; }
        public string Type { get; set; }
    }
}