#:sdk Microsoft.NET.Sdk.Web
#:package Resonance@6.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Resonance;
using Microsoft.Extensions.DependencyInjection;

public class DynamicMessageRouter
{
    private readonly Dictionary<Type, IResonanceAdapter> _adapters;
    private readonly IServiceProvider _serviceProvider;

    public DynamicMessageRouter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _adapters = new Dictionary<Type, IResonanceAdapter>();
    }

    public void RegisterAdapter<TMessage>(IResonanceAdapter adapter)
    {
        _adapters[typeof(TMessage)] = adapter;
    }

    public async ValueTask<object> RouteAsync(ReadOnlyMemory<byte> data)
    {
        // 协议检测逻辑
        var protocol = DetectProtocol(data.Span);
        
        if (_adapters.TryGetValue(protocol, out var adapter))
        {
            return await adapter.AdaptInboundAsync(data);
        }

        throw new ResonanceException($"未注册的协议类型: {protocol}");
    }
}
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

public class DynamicRouter<T>
{
    private readonly BroadcastBlock<T> _broadcaster;
    private readonly Dictionary<string, ITargetBlock<T>> _routes = new();

    public DynamicRouter()
    {
        _broadcaster = new BroadcastBlock<T>(msg => msg);
    }

    public void AddRoute(string routeName, ITargetBlock<T> processor)
    {
        _routes[routeName] = processor;
        _broadcaster.LinkTo(processor, new DataflowLinkOptions 
        { 
            PropagateCompletion = true 
        });
    }

    public async Task RouteAsync(T message)
    {
        await _broadcaster.SendAsync(message);
    }
}