#:sdk Microsoft.NET.Sdk.Web
#:package DeltaCompressionDotNet@1.0.0
#:package Microsoft.IO.RecyclableMemoryStream@2.3.2
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable

using System.Buffers;
using DeltaCompression;
using Microsoft.IO;

public sealed class IncrementalBackupService
{
    private readonly ITransactionalFileManager _fileManager;
    
    public async Task CreateDeltaBackupAsync(string sourcePath, string targetPath)
    {
        using var deltaCreator = new DeltaCreator();
        await deltaCreator.CreateDeltaAsync(sourcePath, targetPath);
    }
    
    public async Task ApplyDeltaAsync(string deltaPath, string targetPath)
    {
        using var deltaApplier = new DeltaApplier();
        await deltaApplier.ApplyDeltaAsync(deltaPath, targetPath);
    }
}

// 高性能增量备份引擎（基于RSync算法优化）
public sealed class IncrementalBackupEngine
{
    private readonly RecyclableMemoryStreamManager _memoryManager;
    private readonly ThreadLocal<byte[]> _deltaBuffer;

    public IncrementalBackupEngine(RecyclableMemoryStreamManager memoryManager)
    {
        _memoryManager = memoryManager;
        _deltaBuffer = new(() => GC.AllocateUninitializedArray<byte>(4096));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask CreateDeltaAsync(string sourcePath, string targetPath)
    {
        using var deltaCreator = new DeltaCreator();
        using var sourceStream = _memoryManager.GetStream(sourcePath);
        using var targetStream = _memoryManager.GetStream();
        
        await deltaCreator.CreateDeltaAsync(sourceStream, targetStream);
        await File.WriteAllBytesAsync(targetPath, targetStream.ToArray());
    }

    [SkipLocalsInit]
    public unsafe Memory<byte> GetDeltaBuffer() => _deltaBuffer.Value;
}