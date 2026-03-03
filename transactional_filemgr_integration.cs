#:sdk Microsoft.NET.Sdk.Web
#:package TransactionalFileMgr@2.1.0
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.IO;
using TransactionalFileMgr;
using Microsoft.IO;

var builder = WebApplication.CreateBuilder(args);

// 1. 事务文件管理器配置
builder.Services.AddSingleton<ITransactionalFileManager>(_ => 
    new TransactionalFileManager(
        new TransactionalFileManagerOptions {
            TempDirectory = "transactions/temp",
            BackupDirectory = "transactions/backup",
            MaxRetryAttempts = 3,
            RetryDelay = TimeSpan.FromMilliseconds(100)
        }));

// 2. 内存流管理器（高性能）
builder.Services.AddSingleton<RecyclableMemoryStreamManager>();

var app = builder.Build();
app.MapGet("/", () => "Transactional File Manager Ready");
app.Run();

// 事务文件操作服务
public sealed class FileTransactionService
{
    private readonly ITransactionalFileManager _fileManager;
    private readonly RecyclableMemoryStreamManager _memoryManager;

    public FileTransactionService(
        ITransactionalFileManager fileManager,
        RecyclableMemoryStreamManager memoryManager)
    {
        _fileManager = fileManager;
        _memoryManager = memoryManager;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task CommitFileTransactionAsync(string filePath, byte[] content)
    {
        using var scope = _fileManager.BeginScope();
        try
        {
            using var stream = _memoryManager.GetStream(content);
            await _fileManager.WriteAllBytesAsync(filePath, stream);
            scope.Complete();
        }
        catch
        {
            scope.Rollback();
            throw;
        }
    }
}