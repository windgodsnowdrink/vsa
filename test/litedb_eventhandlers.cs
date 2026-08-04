#:sdk Microsoft.NET.Sdk.Web
#:package LiteDB@5.0.17
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.ObjectPool;

public class EventHandlerPool
{
    private readonly ObjectPool<IEventHandler> _pool;

    public EventHandlerPool()
    {
        _pool = new DefaultObjectPool<IEventHandler>(
            new EventHandlerPoolPolicy(), 
            Environment.ProcessorCount * 2);
    }

    public IEventHandler Rent() => _pool.Get();
    public void Return(IEventHandler handler) => _pool.Return(handler);

    private class EventHandlerPoolPolicy : IPooledObjectPolicy<IEventHandler>
    {
        public IEventHandler Create() => new DefaultEventHandler();
        public bool Return(IEventHandler obj) => true;
    }
}

public interface IEventHandler
{
    Task HandleAsync(OutOfBandEvent @event);
}

public class DefaultEventHandler : IEventHandler
{
    public Task HandleAsync(OutOfBandEvent @event)
    {
        // 默认处理逻辑...
        return Task.CompletedTask;
    }
}