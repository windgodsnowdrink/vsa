#:sdk Microsoft.NET.Sdk.Web
#:package TransactionalFileMgr@2.1.0
#:package RedLock.net@2.0.0
#:package DeltaCompressionDotNet@1.0.0
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.IO;
using RedLockNet;
using DeltaCompression;
using TransactionalFileMgr;
using Microsoft.IO;

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

var app = builder.Build();
app.MapGet("/", () => "Database Backup Service Ready");
app.Run();

// 数据库备份服务
public sealed class DatabaseBackupService
{
    private readonly ITransactionalFileManager _fileManager;
    private readonly IDistributedLockFactory _lockFactory;
    private readonly RecyclableMemoryStreamManager _memoryManager;

    public DatabaseBackupService(
        ITransactionalFileManager fileManager,
        IDistributedLockFactory lockFactory,
        RecyclableMemoryStreamManager memoryManager)
    {
        _fileManager = fileManager;
        _lockFactory = lockFactory;
        _memoryManager = memoryManager;
    }

    // 1. 分布式文件锁集成
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<bool> TryLockDatabaseAsync(string dbPath, TimeSpan expiry)
    {
        using var redLock = await _lockFactory.CreateLockAsync(
            $"dblock:{dbPath}", expiry, 
            TimeSpan.FromSeconds(1),
            TimeSpan.FromMilliseconds(100));
        
        return redLock.IsAcquired;
    }

    // 2. 文件版本控制
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task CreateVersionedBackupAsync(string dbPath, string versionId)
    {
        var backupPath = $"{dbPath}.v{versionId}";
        using var scope = _fileManager.BeginScope();
        try
        {
            await _fileManager.CopyAsync(dbPath, backupPath);
            scope.Complete();
        }
        catch
        {
            scope.Rollback();
            throw;
        }
    }

    // 3. 增量备份机制
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task CreateIncrementalBackupAsync(string dbPath, string deltaPath)
    {
        using var deltaCreator = new DeltaCreator();
        using var scope = _fileManager.BeginScope();
        try
        {
            await deltaCreator.CreateDeltaAsync(dbPath, deltaPath);
            scope.Complete();
        }
        catch
        {
            scope.Rollback();
            throw;
        }
    }

    // 高性能备份方法
    [SkipLocalsInit]
    public async Task<Memory<byte>> BackupDatabaseAsync(string dbPath)
    {
        using var stream = _memoryManager.GetStream();
        await _fileManager.ReadAllBytesAsync(dbPath, stream);
        return stream.GetBuffer().AsMemory(0, (int)stream.Length);
    }
}