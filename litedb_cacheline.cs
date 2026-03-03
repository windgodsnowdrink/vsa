#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Runtime.InteropServices;
using LiteDB;

[StructLayout(LayoutKind.Explicit, Size = 64)] // 64字节对齐
public struct CacheLineAlignedEvent
{
    [FieldOffset(0)] public ObjectId Id;
    [FieldOffset(12)] public DateTime Timestamp;
    // ...其他字段...
}

public class CacheOptimizedRepository
{
    private readonly ILiteDatabase _db;

    public CacheOptimizedRepository(ILiteDatabase db)
    {
        _db = db;
    }

    public void InsertAligned(CacheLineAlignedEvent @event)
    {
        var collection = _db.GetCollection<CacheLineAlignedEvent>("aligned_events");
        collection.Insert(@event);
    }
}