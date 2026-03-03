#:sdk Microsoft.NET.Sdk
#:package LiteDB@5.0.17
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Threading.Channels;
using LiteDB;

public class EventBusService : IAsyncDisposable
{
    private readonly Channel<OutOfBandEvent> _eventChannel;
    private readonly Task _processingTask;
    private readonly ILiteDatabase _db;
    private readonly CancellationTokenSource _cts = new();

    public EventBusService(ILiteDatabase db)
    {
        _db = db;
        _eventChannel = Channel.CreateBounded<OutOfBandEvent>(10000);
        _processingTask = Task.Run(ProcessEventsAsync);
    }

    public ValueTask PublishAsync(OutOfBandEvent @event)
    {
        return _eventChannel.Writer.WriteAsync(@event, _cts.Token).AsTask();
    }

    private async Task ProcessEventsAsync()
    {
        await foreach (var @event in _eventChannel.Reader.ReadAllAsync(_cts.Token))
        {
            // 持久化到LiteDB
            var collection = _db.GetCollection<OutOfBandEvent>("events");
            collection.Insert(@event);

            // 分发到订阅者...
        }
    }

    public async ValueTask DisposeAsync()
    {
        _eventChannel.Writer.Complete();
        _cts.Cancel();
        await _processingTask;
    }
}