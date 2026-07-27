#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using LiteDB;
using System.Buffers;

// 版本元数据服务（基于内存数据库）
public sealed class VersionMetadataService : IDisposable
{
    private readonly LiteDatabase _db;
    private readonly MemoryPool<byte> _memoryPool;

    public VersionMetadataService()
    {
        _db = new LiteDatabase(":memory:");
        _memoryPool = MemoryPool<byte>.Shared;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void AddVersionMetadata(string versionId, DateTime timestamp, ReadOnlySpan<byte> checksum)
    {
        using var owner = _memoryPool.Rent(checksum.Length);
        checksum.CopyTo(owner.Memory.Span);
        
        var col = _db.GetCollection<VersionMetadata>();
        col.Insert(new VersionMetadata {
            VersionId = versionId,
            Timestamp = timestamp,
            Checksum = owner.Memory.ToArray()
        });
    }

    public void Dispose() => _db.Dispose();
}

public class VersionMetadata
{
    public string VersionId { get; set; }
    public DateTime Timestamp { get; set; }
    public byte[] Checksum { get; set; }
}