#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package TransactionalFileMgr@2.1.0
#:package RedLock.net@2.0.0
#:package DeltaCompressionDotNet@1.0.0
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.IO;
using System.Reflection;
using System.Threading.Channels;
using RedLockNet;
using DeltaCompression;
using TransactionalFileMgr;
using Microsoft.IO;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// 1. 核心服务配置
builder.Services.AddSingleton<ITransactionalFileManager>(_ => 
    new TransactionalFileManager(new TransactionalFileManagerOptions {
        TempDirectory = "transactions/temp",
        BackupDirectory = "transactions/backup",
        MaxRetryAttempts = 3,
        RetryDelay = TimeSpan.FromMilliseconds(100)
    }));

builder.Services.AddSingleton<RecyclableMemoryStreamManager>();
builder.Services.AddSingleton<IDistributedLockFactory>(_ => 
    RedLockFactory.Create(new List<RedLockMultiplexer> {
        ConnectionMultiplexer.Connect("redis1:6379"),
        ConnectionMultiplexer.Connect("redis2:6379"),
        ConnectionMultiplexer.Connect("redis3:6379")
    }));

builder.Services.AddSingleton<DatabaseBackupService>();

var app = builder.Build();
app.MapGet("/", () => "Database Backup Service Ready");
app.Run();

/// <summary>
/// 数据库备份服务
/// 提供分布式锁、事务性文件管理、增量压缩和备份调度功能
/// </summary>
public sealed class DatabaseBackupService
{
    private readonly ITransactionalFileManager _fileManager;
    private readonly IDistributedLockFactory _lockFactory;
    private readonly RecyclableMemoryStreamManager _memoryManager;
    private readonly Channel<BackupRequest> _backupChannel;
    private readonly CancellationTokenSource _cts;
    private readonly Task _processingTask;

    /// <summary>
    /// 初始化数据库备份服务
    /// </summary>
    /// <param name="fileManager">事务性文件管理器</param>
    /// <param name="lockFactory">分布式锁工厂</param>
    /// <param name="memoryManager">可回收内存流管理器</param>
    public DatabaseBackupService(
        ITransactionalFileManager fileManager,
        IDistributedLockFactory lockFactory,
        RecyclableMemoryStreamManager memoryManager)
    {
        _fileManager = fileManager;
        _lockFactory = lockFactory;
        _memoryManager = memoryManager;
        _backupChannel = Channel.CreateUnbounded<BackupRequest>();
        _cts = new CancellationTokenSource();
        _processingTask = ProcessBackupRequestsAsync(_cts.Token);
    }

    /// <summary>
    /// 尝试获取数据库分布式锁
    /// </summary>
    /// <param name="dbPath">数据库路径</param>
    /// <param name="expiry">锁过期时间</param>
    /// <returns>是否获取到锁</returns>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<bool> TryLockDatabaseAsync(string dbPath, TimeSpan expiry)
    {
        using var redLock = await _lockFactory.CreateLockAsync($"backup:{dbPath}", expiry);
        return redLock.IsAcquired;
    }

    /// <summary>
    /// 执行数据库备份
    /// </summary>
    /// <param name="dbPath">数据库路径</param>
    /// <param name="backupPath">备份路径</param>
    /// <param name="strategy">备份策略</param>
    /// <returns>备份是否成功</returns>
    public async Task<bool> BackupDatabaseAsync(string dbPath, string backupPath, BackupStrategy strategy = BackupStrategy.Full)
    {
        var request = new BackupRequest
        {
            DatabasePath = dbPath,
            BackupPath = backupPath,
            Strategy = strategy
        };

        await _backupChannel.Writer.WriteAsync(request);
        return true;
    }

    /// <summary>
    /// 处理备份请求
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessBackupRequestsAsync(CancellationToken cancellationToken)
    {
        await foreach (var request in _backupChannel.Reader.ReadAllAsync(cancellationToken))
        {
            await ProcessSingleBackupRequestAsync(request, cancellationToken);
        }
    }

    /// <summary>
    /// 处理单个备份请求
    /// </summary>
    /// <param name="request">备份请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task ProcessSingleBackupRequestAsync(BackupRequest request, CancellationToken cancellationToken)
    {
        using var redLock = await _lockFactory.CreateLockAsync($"backup:{request.DatabasePath}", TimeSpan.FromMinutes(5));
        if (!redLock.IsAcquired)
        {
            Console.WriteLine($"Failed to acquire lock for database: {request.DatabasePath}");
            return;
        }

        try
        {
            await _fileManager.RunAsync(async () =>
            {
                if (request.Strategy == BackupStrategy.Incremental)
                {
                    await CreateIncrementalBackupAsync(request.DatabasePath, request.BackupPath, cancellationToken);
                }
                else
                {
                    await CreateFullBackupAsync(request.DatabasePath, request.BackupPath, cancellationToken);
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Backup failed: {ex.Message}");
        }
    }

    /// <summary>
    /// 创建完整备份
    /// </summary>
    /// <param name="dbPath">数据库路径</param>
    /// <param name="backupPath">备份路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task CreateFullBackupAsync(string dbPath, string backupPath, CancellationToken cancellationToken)
    {
        using var sourceStream = File.OpenRead(dbPath);
        using var destinationStream = _fileManager.Create(backupPath);
        await sourceStream.CopyToAsync(destinationStream, cancellationToken);
    }

    /// <summary>
    /// 创建增量备份
    /// </summary>
    /// <param name="dbPath">数据库路径</param>
    /// <param name="backupPath">备份路径</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    private async Task CreateIncrementalBackupAsync(string dbPath, string backupPath, CancellationToken cancellationToken)
    {
        var baseBackupPath = Path.Combine(Path.GetDirectoryName(backupPath)!, "base.backup");
        if (!File.Exists(baseBackupPath))
        {
            await CreateFullBackupAsync(dbPath, baseBackupPath, cancellationToken);
        }

        using var sourceStream = File.OpenRead(dbPath);
        using var baseStream = File.OpenRead(baseBackupPath);
        using var patchStream = _fileManager.Create(backupPath);

        await DeltaCompressionUtility.CreateDeltaAsync(baseStream, sourceStream, patchStream, cancellationToken);
    }

    /// <summary>
    /// 恢复数据库从备份
    /// </summary>
    /// <param name="backupPath">备份路径</param>
    /// <param name="targetPath">目标路径</param>
    /// <returns>恢复是否成功</returns>
    public async Task<bool> RestoreDatabaseAsync(string backupPath, string targetPath)
    {
        try
        {
            await _fileManager.RunAsync(async () =>
            {
                if (Path.GetExtension(backupPath).Equals(".patch", StringComparison.OrdinalIgnoreCase))
                {
                    var baseBackupPath = Path.Combine(Path.GetDirectoryName(backupPath)!, "base.backup");
                    await ApplyIncrementalBackupAsync(baseBackupPath, backupPath, targetPath);
                }
                else
                {
                    await ApplyFullBackupAsync(backupPath, targetPath);
                }
            });
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Restore failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 应用完整备份
    /// </summary>
    /// <param name="backupPath">备份路径</param>
    /// <param name="targetPath">目标路径</param>
    /// <returns>任务</returns>
    private async Task ApplyFullBackupAsync(string backupPath, string targetPath)
    {
        using var sourceStream = File.OpenRead(backupPath);
        using var destinationStream = _fileManager.Create(targetPath);
        await sourceStream.CopyToAsync(destinationStream);
    }

    /// <summary>
    /// 应用增量备份
    /// </summary>
    /// <param name="baseBackupPath">基础备份路径</param>
    /// <param name="patchPath">补丁路径</param>
    /// <param name="targetPath">目标路径</param>
    /// <returns>任务</returns>
    private async Task ApplyIncrementalBackupAsync(string baseBackupPath, string patchPath, string targetPath)
    {
        using var baseStream = File.OpenRead(baseBackupPath);
        using var patchStream = File.OpenRead(patchPath);
        using var destinationStream = _fileManager.Create(targetPath);

        await DeltaCompressionUtility.ApplyDeltaAsync(baseStream, patchStream, destinationStream);
    }

    /// <summary>
    /// 备份请求类
    /// </summary>
    private class BackupRequest
    {
        public string DatabasePath { get; set; }
        public string BackupPath { get; set; }
        public BackupStrategy Strategy { get; set; }
    }

    /// <summary>
    /// 备份策略枚举
    /// </summary>
    public enum BackupStrategy
    {
        Full,
        Incremental
    }
}

/// <summary>
/// 分布式锁扩展方法
/// </summary>
public static class DistributedLockExtensions
{
    /// <summary>
    /// 尝试获取锁并执行操作
    /// </summary>
    /// <param name="factory">锁工厂</param>
    /// <param name="resource">资源名称</param>
    /// <param name="expiry">过期时间</param>
    /// <param name="action">要执行的操作</param>
    /// <returns>是否成功执行</returns>
    public static async Task<bool> TryExecuteWithLockAsync(
        this IDistributedLockFactory factory,
        string resource,
        TimeSpan expiry,
        Func<Task> action)
    {
        using var redLock = await factory.CreateLockAsync(resource, expiry);
        if (!redLock.IsAcquired)
        {
            return false;
        }

        await action();
        return true;
    }
}
