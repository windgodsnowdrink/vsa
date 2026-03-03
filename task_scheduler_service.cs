#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Hosting@8.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;

// 1. 高性能任务处理器(Disruptor模式)
[SkipLocalsInit]
public sealed class TaskSchedulerService : BackgroundService
{
    private readonly Channel<ScheduledTask> _taskChannel;
    private readonly ObjectPool<TaskContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public TaskSchedulerService()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _taskChannel = Channel.CreateBounded<ScheduledTask>(new BoundedChannelOptions(10000)
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
    public async Task ScheduleTaskAsync(ScheduledTask task)
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
                    context.Execute(task);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 注册任务调度服务
builder.Services.AddHostedService<TaskSchedulerService>();

var app = builder.Build();

// 任务调度端点
app.MapPost("/schedule", async (ScheduledTask task, TaskSchedulerService scheduler) =>
{
    await scheduler.ScheduleTaskAsync(task);
    return Results.Ok();
});

app.Run();

// 3. 辅助类
public record ScheduledTask(string Id, DateTimeOffset ExecuteAt, string Payload);
public class TaskContext
{
    public void Execute(ScheduledTask task) => Console.WriteLine($"Executing task: {task.Id}");
}
public class TaskContextPooledPolicy : IPooledObjectPolicy<TaskContext>
{
    public TaskContext Create() => new TaskContext();
    public bool Return(TaskContext obj) => true;
}