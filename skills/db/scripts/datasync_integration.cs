#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Data.Sqlite@8.0.0
#:package Prometheus.Net.AspNetCore@8.0.0
#:package Microsoft.AspNetCore.Authentication.JwtBearer@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Channels;
using Microsoft.Data.Sqlite;
using Prometheus;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging();
builder.Services.AddSingleton<OfflineSyncService>(_ => 
    new OfflineSyncService(
        "sync.db", 
        new LastWriteWinsConflictResolver(),
        _.GetRequiredService<ILogger<OfflineSyncService>>()));
builder.Services.AddControllers();
builder.Services.AddPrometheusHttpMetrics();

var app = builder.Build();
app.UseMetricServer();
app.UseHttpMetrics();
app.MapControllers();
app.Run();

/// <summary>
/// 同步项目记录
/// </summary>
/// <param name="Id">项目ID</param>
/// <param name="Data">项目数据</param>
/// <param name="ETag">ETag标识符</param>
/// <param name="LastModified">最后修改时间</param>
public record SyncItem(string Id, string Data, string ETag, DateTimeOffset LastModified);

/// <summary>
/// 同步状态枚举
/// </summary>
enum SyncStatus
{
    Pending = 0,
    Synced = 1,
    Conflict = 2
}

/// <summary>
/// 同步冲突解决器接口
/// </summary>
public interface SyncConflictResolver
{
    /// <summary>
    /// 解决同步冲突
    /// </summary>
    /// <param name="localItem">本地项目</param>
    /// <param name="remoteItem">远程项目</param>
    /// <returns>解决后的项目</returns>
    SyncItem ResolveConflict(SyncItem localItem, SyncItem remoteItem);
}

/// <summary>
/// 最后写入 wins 冲突解决器
/// </summary>
public class LastWriteWinsConflictResolver : SyncConflictResolver
{
    /// <summary>
    /// 解决同步冲突
    /// </summary>
    /// <param name="localItem">本地项目</param>
    /// <param name="remoteItem">远程项目</param>
    /// <returns>解决后的项目</returns>
    public SyncItem ResolveConflict(SyncItem localItem, SyncItem remoteItem)
    {
        return localItem.LastModified > remoteItem.LastModified ? localItem : remoteItem;
    }
}

/// <summary>
/// 本地优先冲突解决器
/// </summary>
public class LocalWinsConflictResolver : SyncConflictResolver
{
    /// <summary>
    /// 解决同步冲突
    /// </summary>
    /// <param name="localItem">本地项目</param>
    /// <param name="remoteItem">远程项目</param>
    /// <returns>解决后的项目</returns>
    public SyncItem ResolveConflict(SyncItem localItem, SyncItem remoteItem)
    {
        return localItem;
    }
}

/// <summary>
/// 离线同步服务
/// </summary>
public class OfflineSyncService : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly SyncConflictResolver _conflictResolver;
    private readonly ILogger<OfflineSyncService> _logger;
    private readonly Channel<SyncItem> _syncChannel;
    private readonly CancellationTokenSource _cts;
    private readonly Task _syncProcessingTask;

    /// <summary>
    /// 同步指标
    /// </summary>
    private static readonly Counter SyncOperationsCounter = Metrics.CreateCounter(
        "sync_operations_total", 
        "Total number of sync operations",
        new CounterConfiguration
        {
            LabelNames = new[] { "operation", "status" }
        });

    /// <summary>
    /// 同步延迟指标
    /// </summary>
    private static readonly Histogram SyncLatencyHistogram = Metrics.CreateHistogram(
        "sync_latency_seconds", 
        "Sync operation latency in seconds");

    /// <summary>
    /// 初始化离线同步服务
    /// </summary>
    /// <param name="dbPath">数据库路径</param>
    /// <param name="conflictResolver">冲突解决器</param>
    /// <param name="logger">日志记录器</param>
    public OfflineSyncService(string dbPath, SyncConflictResolver conflictResolver, ILogger<OfflineSyncService> logger)
    {
        _connection = new SqliteConnection($"Data Source={dbPath}");
        _connection.Open();
        _conflictResolver = conflictResolver;
        _logger = logger;
        _syncChannel = Channel.CreateBounded<SyncItem>(1000);
        _cts = new CancellationTokenSource();
        _syncProcessingTask = ProcessSyncItemsAsync(_cts.Token);
        InitializeDatabase();
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
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
            CREATE INDEX IF NOT EXISTS IX_SyncItems_LastModified ON SyncItems(LastModified);
            CREATE INDEX IF NOT EXISTS IX_SyncItems_SyncStatus ON SyncItems(SyncStatus);
        ";
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// 添加或更新同步项目
    /// </summary>
    /// <param name="item">同步项目</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task AddOrUpdateItemAsync(SyncItem item, CancellationToken cancellationToken = default)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO SyncItems (Id, Data, ETag, LastModified, SyncStatus)
            VALUES (@Id, @Data, @ETag, @LastModified, 0)
            ON CONFLICT(Id) DO UPDATE SET
                Data = excluded.Data,
                ETag = excluded.ETag,
                LastModified = excluded.LastModified,
                SyncStatus = 0
        ";
        command.Parameters.AddWithValue("@Id", item.Id);
        command.Parameters.AddWithValue("@Data", item.Data);
        command.Parameters.AddWithValue("@ETag", item.ETag);
        command.Parameters.AddWithValue("@LastModified", item.LastModified.ToString("o"));

        await command.ExecuteNonQueryAsync(cancellationToken);
        await _syncChannel.Writer.WriteAsync(item, cancellationToken);
    }

    /// <summary>
    /// 获取项目
    /// </summary>
    /// <param name="id">项目ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>同步项目</returns>
    public async Task<SyncItem?> GetItemAsync(string id, CancellationToken cancellationToken = default)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT Id, Data, ETag, LastModified FROM SyncItems WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            return new SyncItem(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                DateTimeOffset.Parse(reader.GetString(3))
            );
        }
        return null;
    }

    /// <summary>
    /// 获取所有待同步项目
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>待同步项目列表</returns>
    public async Task<List<SyncItem>> GetPendingSyncItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = new List<SyncItem>();
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT Id, Data, ETag, LastModified FROM SyncItems WHERE SyncStatus = 0";

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

    /// <summary>
    /// 标记项目为已同步
    /// </summary>
    /// <param name="id">项目ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task MarkAsSyncedAsync(string id, CancellationToken cancellationToken = default)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "UPDATE SyncItems SET SyncStatus = 1 WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// 标记项目为冲突
    /// </summary>
    /// <param name="id">项目ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public async Task MarkAsConflictAsync(string id, CancellationToken cancellationToken = default)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "UPDATE SyncItems SET SyncStatus = 2 WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// 处理同步项目
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessSyncItemsAsync(CancellationToken cancellationToken)
    {
        await foreach (var item in _syncChannel.Reader.ReadAllAsync(cancellationToken))
        {
            await SyncItemAsync(item, cancellationToken);
        }
    }

    /// <summary>
    /// 同步单个项目
    /// </summary>
    /// <param name="item">同步项目</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task SyncItemAsync(SyncItem item, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // 模拟同步到远程服务器
            await Task.Delay(100, cancellationToken);
            await MarkAsSyncedAsync(item.Id, cancellationToken);
            SyncOperationsCounter.WithLabels("sync", "success").Inc();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync item {Id}", item.Id);
            SyncOperationsCounter.WithLabels("sync", "error").Inc();
        }
        finally
        {
            stopwatch.Stop();
            SyncLatencyHistogram.Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }

    /// <summary>
    /// 执行完整同步
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>同步是否成功</returns>
    public async Task<bool> PerformFullSyncAsync(CancellationToken cancellationToken = default)
    {
        var pendingItems = await GetPendingSyncItemsAsync(cancellationToken);
        foreach (var item in pendingItems)
        {
            await SyncItemAsync(item, cancellationToken);
        }
        return true;
    }

    /// <summary>
    /// 生成ETag
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns>ETag字符串</returns>
    public static string GenerateETag(string data)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        await _syncProcessingTask;
        await _connection.DisposeAsync();
        _cts.Dispose();
    }
}

/// <summary>
/// 同步控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly OfflineSyncService _syncService;

    /// <summary>
    /// 初始化同步控制器
    /// </summary>
    /// <param name="syncService">同步服务</param>
    public SyncController(OfflineSyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// 获取项目
    /// </summary>
    /// <param name="id">项目ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>同步项目</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(string id, CancellationToken cancellationToken)
    {
        var item = await _syncService.GetItemAsync(id, cancellationToken);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    /// <summary>
    /// 添加或更新项目
    /// </summary>
    /// <param name="item">同步项目</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<IActionResult> AddOrUpdateItem([FromBody] SyncItem item, CancellationToken cancellationToken)
    {
        await _syncService.AddOrUpdateItemAsync(item, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// 执行同步
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>同步结果</returns>
    [HttpPost("sync")]
    public async Task<IActionResult> PerformSync(CancellationToken cancellationToken)
    {
        await _syncService.PerformFullSyncAsync(cancellationToken);
        return Ok();
    }
}
