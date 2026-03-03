#:sdk Microsoft.NET.Sdk.Web
#:package MySql.Data@8.2.0
#:package Microsoft.Data.Sqlite@8.0.0
#:package LiteDB@5.0.17
#:package System.Threading.Channels@7.0.0
#:package Prometheus.Client@4.3.0
#:package RedLock.net@2.0.0
#:package ChoETL.NETStandard@1.2.1.70
#:package ChoETL.JSON.NETStandard@1.2.1.71
#:package ChoETL.SQLite.Core@1.0.0.4
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Threading.Channels;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;
using LiteDB;
using ChoETL;
using Prometheus.Client;
using RedLockNet;
using RedLockNet.SERedis;
using StackExchange.Redis;

// 数据同步服务
public class DataSyncService : IHostedService
{
    private readonly IMetricFamily<IGauge> _syncMetrics;
    private readonly IDistributedLockFactory _lockFactory;
    private readonly Channel<SyncData> _channel;
    private readonly ILogger<DataSyncService> _logger;
    private readonly string _mysqlConnStr;
    private readonly string _sqlitePath;
    private readonly string _litedbPath;

    public DataSyncService(IConfiguration config, ILogger<DataSyncService> logger, IMetricFactory metricFactory)
    {
        _syncMetrics = metricFactory.CreateGauge("data_sync_metrics", "Data synchronization metrics", "type");
        
        // 初始化分布式锁
        var redis = ConnectionMultiplexer.Connect(config["Redis:ConnectionString"]);
        _lockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { redis });
        _mysqlConnStr = config.GetConnectionString("MySQL");
        _sqlitePath = config["SQLite:Path"];
        _litedbPath = config["LiteDB:Path"];
        _logger = logger;
        
        // 创建有界通道，优化内存使用
        _channel = Channel.CreateBounded<SyncData>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 启动读取任务
        _ = Task.Run(() => ReadFromMySQL(cancellationToken));
        
        // 启动写入任务
        _ = Task.Run(() => WriteToSQLite(cancellationToken));
        _ = Task.Run(() => WriteToLiteDB(cancellationToken));
    }

    private async Task ReadFromMySQL(CancellationToken cancellationToken)
    {
        _syncMetrics.WithLabels("mysql_read_start").Set(1);
        using var conn = new MySqlConnection(_mysqlConnStr);
        await conn.OpenAsync(cancellationToken);

        // 使用增量同步策略
        var lastSyncTime = await GetLastSyncTime();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // 使用ChoETL进行高效数据读取
                using var cmd = new MySqlCommand(
                    "SELECT * FROM YourTable WHERE UpdateTime > @lastSync", conn);
                cmd.Parameters.AddWithValue("@lastSync", lastSyncTime);
                
                using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                
                // 使用ChoReader包装DataReader提高性能
                using var choReader = new ChoReader(reader);
                
                while (await choReader.ReadAsync(cancellationToken))
                {
                    var data = new SyncData
                    {
                        Id = choReader.GetInt32(0),
                        // 其他字段映射...
                        UpdateTime = choReader.GetDateTime(choReader.GetOrdinal("UpdateTime"))
                    };
                    
                    await _channel.Writer.WriteAsync(data, cancellationToken);
_syncMetrics.WithLabels("records_processed").Inc();
                    lastSyncTime = data.UpdateTime;
                }
                
                await Task.Delay(5000, cancellationToken); // 5秒轮询间隔
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MySQL读取错误");
                await Task.Delay(10000, cancellationToken); // 错误后等待10秒
            }
        }
    }

    private async Task WriteToSQLite(CancellationToken cancellationToken)
    {
        using var conn = new SqliteConnection($"Data Source={_sqlitePath}");
        await conn.OpenAsync(cancellationToken);
        
        // 使用ChoETL创建表结构
        using var createCmd = conn.CreateCommand();
        createCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS YourTable (
                Id INTEGER PRIMARY KEY,
                -- 其他字段定义...
                UpdateTime DATETIME
            )";
        await createCmd.ExecuteNonQueryAsync(cancellationToken);

        // 使用ChoETL批量写入优化
        var batchWriter = new ChoSQLiteWriter(conn);
        
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            var batch = new List<SyncData>();
            
            while (_channel.Reader.TryRead(out var data))
            {
                batch.Add(data);
                if(batch.Count >= 1000) // 批量处理1000条记录
                {
                    try
                    {
                        await batchWriter.WriteAsync(batch, cancellationToken);
                        batch.Clear();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "SQLite批量写入错误");
                    }
                }
            }
            
            // 处理剩余记录
            if(batch.Count > 0)
            {
                try
                {
                    await batchWriter.WriteAsync(batch, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SQLite批量写入错误");
                }
            }
        }
    }

    private async Task WriteToLiteDB(CancellationToken cancellationToken)
    {
        using var db = new LiteDatabase(_litedbPath);
        var collection = db.GetCollection<SyncData>("YourTable");
        
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            while (_channel.Reader.TryRead(out var data))
            {
                try
                {
                    // Upsert操作
                    collection.Upsert(data);
                    db.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "LiteDB写入错误");
                }
            }
        }
    }

    private async Task<DateTime> GetLastSyncTime()
    {
        // 使用分布式锁确保只有一个实例执行同步
        await using var redLock = await _lockFactory.CreateLockAsync("sync_lock", TimeSpan.FromSeconds(30));
        if (!redLock.IsAcquired)
        {
            _logger.LogWarning("Failed to acquire distributed lock");
            return DateTime.MinValue;
        }
        
        _syncMetrics.WithLabels("lock_acquired").Set(1);
        // 实现从SQLite或LiteDB获取最后同步时间
        return DateTime.MinValue;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public class SyncData
{
    public int Id { get; set; }
    public DateTime UpdateTime { get; set; }
    // 其他数据字段...
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataSyncService(this IServiceCollection services)
    {
        services.AddHostedService<DataSyncService>();
        
        // 添加Prometheus监控
        services.AddSingleton<IMetricFactory>(MetricFactory.Default);
        
        // 添加断点续传状态存储
        services.AddSingleton<ISyncStateStore, LiteDBSyncStateStore>();
        
        return services;
    }
}

// 断点续传状态存储接口
public interface ISyncStateStore
{
    Task<DateTime> GetLastSyncTimeAsync();
    Task SaveLastSyncTimeAsync(DateTime time);
}

// LiteDB实现
public class LiteDBSyncStateStore : ISyncStateStore
{
    private readonly string _dbPath;
    
    public LiteDBSyncStateStore(IConfiguration config)
    {
        _dbPath = config["LiteDB:Path"];
    }
    
    public async Task<DateTime> GetLastSyncTimeAsync()
    {
        using var db = new LiteDatabase(_dbPath);
        var collection = db.GetCollection<SyncState>("sync_state");
        var state = collection.FindOne(Query.All());
        return state?.LastSyncTime ?? DateTime.MinValue;
    }
    
    public async Task SaveLastSyncTimeAsync(DateTime time)
    {
        using var db = new LiteDatabase(_dbPath);
        var collection = db.GetCollection<SyncState>("sync_state");
        collection.Upsert(new SyncState { LastSyncTime = time });
    }
}

public class SyncState
{
    public DateTime LastSyncTime { get; set; }
}