#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Persistence.AdoNet@8.0.0
#:package Microsoft.Orleans.Clustering.AdoNet@8.0.0
#:package Microsoft.Orleans.Reminders.AdoNet@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Diagnostics.DiagnosticSource@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Streams;

// 状态定义
public enum WorkflowState { Created, Running, Paused, Completed, Failed }

// 触发器定义
public enum WorkflowTrigger { Start, Pause, Resume, Complete, Fail }

// 状态机Grain接口
public interface IWorkflowStateMachineGrain : IGrainWithGuidKey
{
    Task<WorkflowState> GetCurrentState();
    Task FireAsync(WorkflowTrigger trigger);
    Task SubscribeAsync(IAsyncObserver<WorkflowState> observer);
}

// 状态机Grain实现
[StorageProvider(ProviderName = "OrleansStorage")]
public class WorkflowStateMachineGrain : Grain<WorkflowStateMachineState>, IWorkflowStateMachineGrain
{
    private readonly Meter _meter;
    private readonly Counter<int> _transitionCounter;
    private IAsyncStream<WorkflowState> _stream;
    private IAsyncObserver<WorkflowState> _observer;

    public WorkflowStateMachineGrain(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("WorkflowStateMachine");
        _transitionCounter = _meter.CreateCounter<int>("state_transitions");
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider("SMS");
        _stream = streamProvider.GetStream<WorkflowState>(this.GetPrimaryKey(), "WorkflowState");
        await base.OnActivateAsync(cancellationToken);
    }

    public Task<WorkflowState> GetCurrentState() => Task.FromResult(State.CurrentState);

    public async Task FireAsync(WorkflowTrigger trigger)
    {
        var previousState = State.CurrentState;
        State.CurrentState = (previousState, trigger) switch
        {
            (WorkflowState.Created, WorkflowTrigger.Start) => WorkflowState.Running,
            (WorkflowState.Running, WorkflowTrigger.Pause) => WorkflowState.Paused,
            (WorkflowState.Paused, WorkflowTrigger.Resume) => WorkflowState.Running,
            (WorkflowState.Running, WorkflowTrigger.Complete) => WorkflowState.Completed,
            (_, WorkflowTrigger.Fail) => WorkflowState.Failed,
            _ => throw new InvalidOperationException($"Invalid transition: {previousState} -> {trigger}")
        };

        _transitionCounter.Add(1);
        await WriteStateAsync();
        await _stream.OnNextAsync(State.CurrentState);
    }

    public Task SubscribeAsync(IAsyncObserver<WorkflowState> observer)
    {
        _observer = observer;
        return Task.CompletedTask;
    }
}

// Grain状态
[GenerateSerializer]
public class WorkflowStateMachineState
{
    [Id(0)] public WorkflowState CurrentState { get; set; } = WorkflowState.Created;
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrleansStateMachine(this IServiceCollection services, 
        Action<OrleansStateMachineOptions> configure = null)
    {
        services.Configure(configure ?? (opts => {}));
        services.AddSingleton<IWorkflowStateMachineService, WorkflowStateMachineService>();
        return services;
    }
}

// 服务层接口
public interface IWorkflowStateMachineService
{
    Task<WorkflowState> GetCurrentStateAsync(Guid workflowId);
    Task FireAsync(Guid workflowId, WorkflowTrigger trigger);
}

// 服务层实现
public class WorkflowStateMachineService : IWorkflowStateMachineService
{
    private readonly IClusterClient _clusterClient;

    public WorkflowStateMachineService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public Task<WorkflowState> GetCurrentStateAsync(Guid workflowId) => 
        _clusterClient.GetGrain<IWorkflowStateMachineGrain>(workflowId).GetCurrentState();

    public Task FireAsync(Guid workflowId, WorkflowTrigger trigger) => 
        _clusterClient.GetGrain<IWorkflowStateMachineGrain>(workflowId).FireAsync(trigger);
}

// Orleans配置
public class OrleansStateMachineOptions
{
    public string ClusterId { get; set; } = "dev";
    public string ServiceId { get; set; } = "WorkflowService";
    public string AdoNetConnectionString { get; set; } = "Server=.;Database=Orleans;Integrated Security=true;";
}

// 示例用法
public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .UseOrleans((context, builder) =>
            {
                var options = context.Configuration.Get<OrleansStateMachineOptions>();
                
                builder.UseAdoNetClustering(options =>
                {
                    options.ConnectionString = options.AdoNetConnectionString;
                    options.Invariant = "System.Data.SqlClient";
                })
                .AddAdoNetGrainStorage("OrleansStorage", opts =>
                {
                    opts.ConnectionString = options.AdoNetConnectionString;
                    opts.Invariant = "System.Data.SqlClient";
                })
                .AddSimpleMessageStreamProvider("SMS")
                .AddMemoryGrainStorage("PubSubStore")
                .Configure<ClusterOptions>(opts =>
                {
                    opts.ClusterId = options.ClusterId;
                    opts.ServiceId = options.ServiceId;
                });
            })
            .ConfigureServices(services =>
            {
                services.AddOrleansStateMachine(options =>
                {
                    options.ClusterId = "prod";
                    options.ServiceId = "WorkflowService";
                });
            })
            .Build();

        await host.StartAsync();
        
        var service = host.Services.GetRequiredService<IWorkflowStateMachineService>();
        var workflowId = Guid.NewGuid();
        
        await service.FireAsync(workflowId, WorkflowTrigger.Start);
        await service.FireAsync(workflowId, WorkflowTrigger.Pause);
        await service.FireAsync(workflowId, WorkflowTrigger.Resume);
        await service.FireAsync(workflowId, WorkflowTrigger.Complete);

        await host.WaitForShutdownAsync();
    }
}