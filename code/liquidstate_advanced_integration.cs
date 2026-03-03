#:sdk Microsoft.NET.Sdk
#:package LiquidState@8.0.0
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Diagnostics.Metrics;
using System.Text.Json;
using LiquidState;
using LiquidState.Awaitable.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

// 分布式状态存储接口
public interface IStateRepository
{
    Task SaveAsync<TState, TTrigger>(string machineId, 
        IStateMachine<TState, TTrigger> machine);
    Task<IStateMachine<TState, TTrigger>?> LoadAsync<TState, TTrigger>(
        string machineId, TState initialState);
}

// Redis状态存储实现
public class RedisStateRepository : IStateRepository, IAsyncDisposable
{
    private readonly IDistributedCache _cache;
    private readonly IOptions<RedisOptions> _options;
    
    public RedisStateRepository(IDistributedCache cache, 
        IOptions<RedisOptions> options)
    {
        _cache = cache;
        _options = options;
    }
    
    public async Task SaveAsync<TState, TTrigger>(string machineId, 
        IStateMachine<TState, TTrigger> machine)
    {
        var bytes = ArrayPool<byte>.Shared.Rent(1024);
        try
        {
            var writer = new ArrayBufferWriter<byte>(bytes);
            await JsonSerializer.SerializeAsync(writer.AsStream(), 
                new StateMachineData<TState>(machine.CurrentState));
            
            await _cache.SetAsync(GetKey(machineId), bytes.AsMemory(0, writer.WrittenCount),
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = _options.Value.SlidingExpiration
                });
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }
    
    public async Task<IStateMachine<TState, TTrigger>?> LoadAsync<TState, TTrigger>(
        string machineId, TState initialState)
    {
        var bytes = await _cache.GetAsync(GetKey(machineId));
        if (bytes == null) return null;
        
        var data = await JsonSerializer.DeserializeAsync<StateMachineData<TState>>(
            new MemoryStream(bytes));
            
        var config = StateMachineFactory.Create<TState, TTrigger>();
        // 配置状态转换规则...
        
        return config.Build(data?.CurrentState ?? initialState);
    }
    
    public ValueTask DisposeAsync() => default;
}

// AOT优化状态机工厂
public static class AotStateMachineFactory
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void Initialize()
    {
        // 预编译常见状态机配置
        var config = StateMachineFactory.Create<WorkflowState, WorkflowTrigger>();
        config.In(WorkflowState.Started)
            .On(WorkflowTrigger.Begin).TransitionTo(WorkflowState.Processing);
        // 其他状态转换规则...
    }
}

// 性能监控装饰器
public class MonitoredStateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
{
    private readonly IStateMachine<TState, TTrigger> _inner;
    private readonly Counter<int> _transitionCounter;
    
    public TState CurrentState => _inner.CurrentState;
    
    public MonitoredStateMachine(IStateMachine<TState, TTrigger> inner, 
        IMeterFactory meterFactory)
    {
        _inner = inner;
        var meter = meterFactory.Create("StateMachine");
        _transitionCounter = meter.CreateCounter<int>("state_transitions");
    }
    
    public Task FireAsync<TArg>(ParameterizedTrigger<TTrigger, TArg> parameterizedTrigger, 
        TArg arg, CancellationToken cancellationToken = default)
    {
        _transitionCounter.Add(1);
        return _inner.FireAsync(parameterizedTrigger, arg, cancellationToken);
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedStateMachine(this IServiceCollection services,
        Action<RedisOptions> configure = null)
    {
        services.Configure(configure ?? (opts => {}));
        services.AddSingleton<IStateRepository, RedisStateRepository>();
        services.AddSingleton(typeof(IStateMachine<,>), typeof(MonitoredStateMachine<,>));
        return services;
    }
}