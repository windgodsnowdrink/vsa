#!/usr/bin/env dotnet
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<DataSyncService>();
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();

/// <summary>
/// 同步数据记录
/// </summary>
public record SyncData(string Id, string TableName, string Data, DateTimeOffset Timestamp);

/// <summary>
/// 同步状态枚举
/// </summary>
enum SyncStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}

/// <summary>
/// 数据同步服务
/// </summary>
public class DataSyncService : IHostedService
{
    private readonly IMetricFamily<IGauge> _syncMetrics;
    private readonly IDistributedLockFactory _lockFactory;
    private readonly Channel<SyncData> _channel;
    private readonly ILogger<DataSyncService> _logger;
    private readonly string _mysqlConnStr;
    private readonly string _sqlitePath;
    private readonly string _litedbPath;
    private CancellationTokenSource _cts;
    private Task _processingTask;

    /// <summary>
    /// 初始化数据同步服务
    /// </summary>
    /// <param name="config">配置</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="metricFactory">指标工厂</param>
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

    /// <summary>
    /// 启动服务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _processingTask = ProcessSyncDataAsync(_cts.Token);

        // 启动数据读取任务
        _ = Task.Run(() => ReadFromMySQLAsync(_cts.Token), _cts.Token);

        _logger.LogInformation("Data sync service started");
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        await _processingTask;
        _logger.LogInformation("Data sync service stopped");
    }

    /// <summary>
    /// 从MySQL读取数据
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ReadFromMySQLAsync(CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_mysqlConnStr);
        await connection.OpenAsync(cancellationToken);

        // 读取用户表数据
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT id, name, email, created_at FROM users WHERE updated_at > @LastSyncTime";
        command.Parameters.AddWithValue("@LastSyncTime", DateTime.UtcNow.AddHours(-1));

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var syncData = new SyncData(
                reader.GetInt32(0).ToString(),
                "users",
                $"{{\"name\":\"{reader.GetString(1)}\",\"email\":\"{reader.GetString(2)}\",\"created_at\":\"{reader.GetDateTime(3):o}\"}}",
                DateTimeOffset.UtcNow
            );

            await _channel.Writer.WriteAsync(syncData, cancellationToken);
        }
    }

    /// <summary>
    /// 处理同步数据
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessSyncDataAsync(CancellationToken cancellationToken)
    {
        await foreach (var syncData in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            await ProcessSingleSyncDataAsync(syncData, cancellationToken);
        }
    }

    /// <summary>
    /// 处理单个同步数据
    /// </summary>
    /// <param name="syncData">同步数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessSingleSyncDataAsync(SyncData syncData, CancellationToken cancellationToken)
    {
        using var redLock = await _lockFactory.CreateLockAsync($"sync:{syncData.TableName}:{syncData.Id}", TimeSpan.FromMinutes(5));
        if (!redLock.IsAcquired)
        {
            _logger.LogWarning("Failed to acquire lock for sync data {Id}", syncData.Id);
            return;
        }

        try
        {
            // 同步到SQLite
            await SyncToSQLiteAsync(syncData, cancellationToken);
            
            // 同步到LiteDB
            await SyncToLiteDBAsync(syncData, cancellationToken);

            _syncMetrics.WithLabels("success").Inc();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync data {Id}", syncData.Id);
            _syncMetrics.WithLabels("error").Inc();
        }
    }

    /// <summary>
    /// 同步到SQLite
    /// </summary>
    /// <param name="syncData">同步数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task SyncToSQLiteAsync(SyncData syncData, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection($"Data Source={_sqlitePath}");
        await connection.OpenAsync(cancellationToken);

        // 确保表存在
        await EnsureSQLiteTableExistsAsync(connection, syncData.TableName, cancellationToken);

        // 插入或更新数据
        using var command = connection.CreateCommand();
        command.CommandText = $@"
            INSERT INTO {syncData.TableName} (id, data, synced_at)
            VALUES (@Id, @Data, @SyncedAt)
            ON CONFLICT(id) DO UPDATE SET
                data = excluded.data,
                synced_at = excluded.synced_at
        ";
        command.Parameters.AddWithValue("@Id", syncData.Id);
        command.Parameters.AddWithValue("@Data", syncData.Data);
        command.Parameters.AddWithValue("@SyncedAt", DateTimeOffset.UtcNow.ToString("o"));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// 确保SQLite表存在
    /// </summary>
    /// <param name="connection">SQLite连接</param>
    /// <param name="tableName">表名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task EnsureSQLiteTableExistsAsync(SqliteConnection connection, string tableName, CancellationToken cancellationToken)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $@"
            CREATE TABLE IF NOT EXISTS {tableName} (
                id TEXT PRIMARY KEY,
                data TEXT NOT NULL,
                synced_at TEXT NOT NULL
            );
            CREATE INDEX IF NOT EXISTS IX_{tableName}_SyncedAt ON {tableName}(synced_at);
        ";
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// 同步到LiteDB
    /// </summary>
    /// <param name="syncData">同步数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task SyncToLiteDBAsync(SyncData syncData, CancellationToken cancellationToken)
    {
        using var db = new LiteDatabase(_litedbPath);
        var collection = db.GetCollection<LiteDbSyncItem>(syncData.TableName);

        // 创建索引
        collection.EnsureIndex(x => x.Id);
        collection.EnsureIndex(x => x.SyncedAt);

        var item = new LiteDbSyncItem
        {
            Id = syncData.Id,
            Data = syncData.Data,
            SyncedAt = DateTimeOffset.UtcNow
        };

        collection.Upsert(item);
    }

    /// <summary>
    /// LiteDB同步项
    /// </summary>
    public class LiteDbSyncItem
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTimeOffset SyncedAt { get; set; }
    }
}

/// <summary>
/// 数据同步控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DataSyncController : ControllerBase
{
    private readonly IHostedService _syncService;

    /// <summary>
    /// 初始化数据同步控制器
    /// </summary>
    /// <param name="syncService">数据同步服务</param>
    public DataSyncController(IHostedService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// 触发手动同步
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost("trigger")]
    public IActionResult TriggerSync(CancellationToken cancellationToken)
    {
        // 这里可以实现手动触发同步的逻辑
        return Ok(new { message = "Sync triggered" });
    }
}

/// <summary>
/// 指标工厂扩展方法
/// </summary>
public static class MetricFactoryExtensions
{
    /// <summary>
    /// 创建 gauge 指标
    /// </summary>
    /// <param name="factory">指标工厂</param>
    /// <param name="name">指标名称</param>
    /// <param name="help">指标帮助文本</param>
    /// <param name="labelNames">标签名称</param>
    /// <returns>指标族</returns>
    public static IMetricFamily<IGauge> CreateGauge(this IMetricFactory factory, string name, string help, params string[] labelNames)
    {
        // 模拟实现
        return new MockGaugeMetricFamily(name, help, labelNames);
    }
}

/// <summary>
/// 模拟 gauge 指标族
/// </summary>
public class MockGaugeMetricFamily : IMetricFamily<IGauge>
{
    private readonly string _name;
    private readonly string _help;
    private readonly string[] _labelNames;
    private readonly Dictionary<string, IGauge> _gauges = new();

    /// <summary>
    /// 初始化模拟 gauge 指标族
    /// </summary>
    /// <param name="name">指标名称</param>
    /// <param name="help">指标帮助文本</param>
    /// <param name="labelNames">标签名称</param>
    public MockGaugeMetricFamily(string name, string help, string[] labelNames)
    {
        _name = name;
        _help = help;
        _labelNames = labelNames;
    }

    /// <summary>
    /// 指标名称
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// 指标帮助文本
    /// </summary>
    public string Help => _help;

    /// <summary>
    /// 指标类型
    /// </summary>
    public MetricType Type => MetricType.Gauge;

    /// <summary>
    /// 获取指标
    /// </summary>
    /// <param name="labels">标签值</param>
    /// <returns>指标</returns>
    public IGauge WithLabels(params string[] labels)
    {
        var key = string.Join(":", labels);
        if (!_gauges.TryGetValue(key, out var gauge))
        {
            gauge = new MockGauge();
            _gauges[key] = gauge;
        }
        return gauge;
    }

    /// <summary>
    /// 收集指标
    /// </summary>
    /// <returns>指标枚举</returns>
    public IEnumerable<Collector> Collect()
    {
        return Enumerable.Empty<Collector>();
    }
}

/// <summary>
/// 模拟 gauge 指标
/// </summary>
public class MockGauge : IGauge
{
    private double _value;

    /// <summary>
    /// 增加指标值
    /// </summary>
    /// <param name="value">增加的值</param>
    /// <returns>当前指标</returns>
    public IGauge Inc(double value = 1)
    {
        _value += value;
        return this;
    }

    /// <summary>
    /// 减少指标值
    /// </summary>
    /// <param name="value">减少的值</param>
    /// <returns>当前指标</returns>
    public IGauge Dec(double value = 1)
    {
        _value -= value;
        return this;
    }

    /// <summary>
    /// 设置指标值
    /// </summary>
    /// <param name="value">指标值</param>
    /// <returns>当前指标</returns>
    public IGauge Set(double value)
    {
        _value = value;
        return this;
    }

    /// <summary>
    /// 获取指标值
    /// </summary>
    /// <returns>指标值</returns>
    public double Value => _value;
}

/// <summary>
/// 指标类型
/// </summary>
enum MetricType
{
    Counter,
    Gauge,
    Histogram,
    Summary
}

/// <summary>
/// 收集器接口
/// </summary>
public interface Collector
{
}

/// <summary>
/// 指标族接口
/// </summary>
/// <typeparam name="T">指标类型</typeparam>
public interface IMetricFamily<out T>
{
    /// <summary>
    /// 指标名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 指标帮助文本
    /// </summary>
    string Help { get; }

    /// <summary>
    /// 指标类型
    /// </summary>
    MetricType Type { get; }

    /// <summary>
    /// 获取指标
    /// </summary>
    /// <param name="labels">标签值</param>
    /// <returns>指标</returns>
    T WithLabels(params string[] labels);

    /// <summary>
    /// 收集指标
    /// </summary>
    /// <returns>指标枚举</returns>
    IEnumerable<Collector> Collect();
}

/// <summary>
/// Gauge 指标接口
/// </summary>
public interface IGauge
{
    /// <summary>
    /// 增加指标值
    /// </summary>
    /// <param name="value">增加的值</param>
    /// <returns>当前指标</returns>
    IGauge Inc(double value = 1);

    /// <summary>
    /// 减少指标值
    /// </summary>
    /// <param name="value">减少的值</param>
    /// <returns>当前指标</returns>
    IGauge Dec(double value = 1);

    /// <summary>
    /// 设置指标值
    /// </summary>
    /// <param name="value">指标值</param>
    /// <returns>当前指标</returns>
    IGauge Set(double value);

    /// <summary>
    /// 获取指标值
    /// </summary>
    /// <returns>指标值</returns>
    double Value { get; }
}

/// <summary>
/// 指标工厂接口
/// </summary>
public interface IMetricFactory
{
}
