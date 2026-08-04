#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Runtime.CompilerServices;
using LiteDB;

public class AotOptimizedRepository
{
    private readonly ILiteDatabase _db;

    public AotOptimizedRepository(ILiteDatabase db)
    {
        _db = db;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void InsertOptimized(OutOfBandEvent @event)
    {
        var collection = _db.GetCollection<OutOfBandEvent>("events");
        collection.Insert(@event);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public OutOfBandEvent QueryOptimized(ObjectId id)
    {
        var collection = _db.GetCollection<OutOfBandEvent>("events");
        return collection.FindById(id);
    }
}