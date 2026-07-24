#:sdk Microsoft.NET.Sdk.Web
#:package TransactionalFileMgr@2.1.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System.IO;

public sealed class FileVersioningService
{
    private readonly ITransactionalFileManager _fileManager;
    
    public async Task CreateSnapshotAsync(string filePath, string versionId)
    {
        var snapshotPath = $"{filePath}.snapshot.{versionId}";
        await _fileManager.CopyAsync(filePath, snapshotPath);
    }
    
    public async Task RollbackToVersionAsync(string filePath, string versionId)
    {
        var snapshotPath = $"{filePath}.snapshot.{versionId}";
        await _fileManager.CopyAsync(snapshotPath, filePath);
    }
}