#:sdk Microsoft.NET.Sdk.Web
#:package ScheduleMaster.Core@3.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using ScheduleMaster.Core;
using Microsoft.Extensions.ObjectPool;

// 1. 任务接口抽象
public interface ITaskHandler
{
    Task ExecuteAsync(string payload, CancellationToken ct);
}

// 2. 可配置存储接口
public interface IStorageProvider
{
    Task SaveTaskStateAsync(string taskId, string state);
    Task<string> GetTaskStateAsync(string taskId);
}

// 3. 模块化任务注册器
public class TaskRegistry
{
    private readonly Dictionary<string, Type> _taskTypes = new();
    
    public TaskRegistry Register<T>(string taskType) where T : ITaskHandler
    {
        _taskTypes[taskType] = typeof(T);
        return this;
    }
    
    public Type GetTaskType(string taskType) => _taskTypes[taskType];
}

// 4. 高性能任务处理器(增强版)
[SkipLocalsInit]
public sealed class ScheduleMasterProcessor : BackgroundService
{
    private readonly Channel<ScheduleTask> _taskChannel;
    private readonly ObjectPool<TaskContext> _contextPool;
    private readonly IScheduleMaster _scheduler;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly TaskRegistry _taskRegistry;
    private readonly IStorageProvider _storage;

    public ScheduleMasterProcessor(
        IScheduleMaster scheduler, 
        TaskRegistry taskRegistry,
        IStorageProvider storage)
    {
        _taskRegistry = taskRegistry;
        _storage = storage;
        _scheduler = scheduler;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _taskChannel = Channel.CreateBounded<ScheduleTask>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<TaskContext>(
            new TaskContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ScheduleTaskAsync(ScheduleTask task)
    {
        await _taskChannel.Writer.WriteAsync(task);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in _taskChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                _latencyOptimizer.Optimize(() => 
                {
                    context.Execute(task, _scheduler, _taskRegistry, _storage);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 5. 增强版任务上下文
public class TaskContext
{
    public void Execute(
        ScheduleTask task, 
        IScheduleMaster scheduler,
        TaskRegistry registry,
        IStorageProvider storage)
    {
        var taskType = registry.GetTaskType(task.TaskType);
        scheduler.Schedule(task.Id, task.CronExpression, async () => 
        {
            var handler = (ITaskHandler)Activator.CreateInstance(taskType);
            await handler.ExecuteAsync(task.Payload, CancellationToken.None);
            
            // 保存任务状态
            await storage.SaveTaskStateAsync(task.Id, "Completed");
        });
    }
}

// 6. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置存储模式(可切换)
builder.Services.AddSingleton<IStorageProvider>(sp => 
    builder.Configuration["Storage:Type"] == "Redis" 
        ? new RedisStorageProvider(builder.Configuration["Redis"]) 
        : new InMemoryStorageProvider());

// 注册任务类型
var taskRegistry = new TaskRegistry()
    .Register<EmailTaskHandler>("email")
    .Register<ReportTaskHandler>("report");

builder.Services.AddSingleton(taskRegistry);

// 配置ScheduleMaster
builder.Services.AddScheduleMaster(config =>
{
    config.UseInMemoryStorage();
    config.UseDistributedLock();
});

// 注册任务处理器
builder.Services.AddHostedService<ScheduleMasterProcessor>();

var app = builder.Build();

// 任务调度端点
app.MapPost("/schedule", async (ScheduleTask task, ScheduleMasterProcessor processor) =>
{
    await processor.ScheduleTaskAsync(task);
    return Results.Ok();
});

app.Run();

// 3. 辅助类
public record ScheduleTask(string Id, string CronExpression, string Payload);
public class TaskContextPooledPolicy : IPooledObjectPolicy<TaskContext>
{
    public TaskContext Create() => new TaskContext();
    public bool Return(TaskContext obj) => true;
}

// 7. 示例任务处理器
public class EmailTaskHandler : ITaskHandler
{
    public Task ExecuteAsync(string payload, CancellationToken ct)
    {
        Console.WriteLine($"Sending email: {payload}");
        return Task.CompletedTask;
    }
}