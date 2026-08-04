#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using LiteDB;

public class TieredMemoryStorage
{
    private readonly ILiteDatabase _hotDb;
    private readonly ILiteDatabase _warmDb;
    private readonly ILiteDatabase _coldDb;
    
    public TieredMemoryStorage()
    {
        _hotDb = new LiteDatabase("Filename=hot.db;Mode=Shared");
        _warmDb = new LiteDatabase("Filename=warm.db;Mode=Shared");
        _coldDb = new LiteDatabase("Filename=cold.db;Mode=Shared");
    }

    public void StoreHotData(OutOfBandEvent @event)
    {
        var collection = _hotDb.GetCollection<OutOfBandEvent>("hot_events");
        collection.Insert(@event);
    }

    public void MigrateToWarm(ObjectId eventId)
    {
        var hotCollection = _hotDb.GetCollection<OutOfBandEvent>("hot_events");
        var warmCollection = _warmDb.GetCollection<OutOfBandEvent>("warm_events");
        
        var @event = hotCollection.FindById(eventId);
        if (@event != null)
        {
            warmCollection.Insert(@event);
            hotCollection.Delete(eventId);
        }
    }
    // ... 其他迁移方法 ...
}