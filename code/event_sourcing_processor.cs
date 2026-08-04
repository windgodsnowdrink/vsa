#:sdk Microsoft.NET.Sdk.Web
#:package MongoDB.Driver@2.20.0
#:package MongoDB.Driver.Core@2.20.0
#:package MongoDB.Bson@2.20.0
#:package System.Threading.Channels@8.0.0
#:package TieredMemory@1.2.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using MongoDB.Driver;
using System.Threading.Channels;
using TieredMemory;

var builder = WebApplication.CreateBuilder();

// 1. 配置MongoDB事件存储
builder.Services.AddSingleton<IMongoClient>(_ => 
    new MongoClient("mongodb://localhost:27017"));
builder.Services.AddSingleton(sp => 
    sp.GetRequiredService<IMongoClient>()
     .GetDatabase("event_store")
     .GetCollection<EventDocument>("events"));

// 2. 高性能事件通道 (Disruptor模式)
var eventChannel = Channel.CreateBounded<Event>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 3. 分层内存优化的事件处理器
builder.Services.AddSingleton<IEventProcessor>(sp => 
    new TieredEventProcessor(
        eventChannel,
        new TieredMemoryPool<byte>(1024 * 1024, 64),
        sp.GetRequiredService<IMongoCollection<EventDocument>>()));

var app = builder.Build();
app.MapGet("/", () => "Event Sourcing Processor Ready");
app.Run();

[SkipLocalsInit]
public class TieredEventProcessor : IEventProcessor
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessAsync(Event @event)
    {
        // 使用分层内存和零拷贝技术处理事件
    }
}
public record EventWrapper(ReadOnlyMemory<byte> Data);