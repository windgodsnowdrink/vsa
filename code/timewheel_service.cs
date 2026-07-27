#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;

// 1. 时间轮算法核心实现
[SkipLocalsInit]
public sealed class TimeWheel : BackgroundService
{
    private const int WHEEL_SIZE = 60; // 60个槽位，精度1秒
    private readonly Channel<TimeTask>[] _wheel = new Channel<TimeTask>[WHEEL_SIZE];
    private readonly ObjectPool<TimeTask> _taskPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private int _currentSlot = 0;

    public TimeWheel()
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // 初始化时间轮槽位
        for (int i = 0; i < WHEEL_SIZE; i++)
        {
            _wheel[i] = Channel.CreateBounded<TimeTask>(new BoundedChannelOptions(1000)
            {
                SingleReader = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        }

        // 任务对象池
        _taskPool = new DefaultObjectPool<TimeTask>(new TimeTaskPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ScheduleAsync(TimeSpan delay, Func<Task> action)
    {
        var slots = (int)(delay.TotalSeconds % WHEEL_SIZE);
        var targetSlot = (_currentSlot + slots) % WHEEL_SIZE;
        
        var task = _taskPool.Get();
        try
        {
            task.Action = action;
            task.ExecutionTime = DateTime.UtcNow + delay;
            await _wheel[targetSlot].Writer.WriteAsync(task);
        }
        finally
        {
            _taskPool.Return(task);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            _latencyOptimizer.Optimize(() => 
            {
                ProcessCurrentSlot();
                _currentSlot = (_currentSlot + 1) % WHEEL_SIZE;
            });
        }
    }

    private void ProcessCurrentSlot()
    {
        var channel = _wheel[_currentSlot];
        while (channel.Reader.TryRead(out var task))
        {
            _ = Task.Run(async () => 
            {
                if (DateTime.UtcNow >= task.ExecutionTime)
                {
                    await task.Action();
                }
                else
                {
                    // 重新调度未到期的任务
                    await ScheduleAsync(task.ExecutionTime - DateTime.UtcNow, task.Action);
                }
            });
        }
    }
}

// 2. 主程序集成
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TimeWheel>();
builder.Services.AddHostedService<TimeWheel>();

var app = builder.Build();
app.MapGet("/schedule", async (TimeWheel timeWheel) =>
{
    await timeWheel.ScheduleAsync(TimeSpan.FromSeconds(5), () => 
    {
        Console.WriteLine($"Task executed at {DateTime.UtcNow}");
        return Task.CompletedTask;
    });
    return Results.Ok("Task scheduled");
});

app.Run();

// 3. 辅助类
public class TimeTask
{
    public Func<Task> Action { get; set; }
    public DateTime ExecutionTime { get; set; }
}

public class TimeTaskPooledPolicy : IPooledObjectPolicy<TimeTask>
{
    public TimeTask Create() => new TimeTask();
    public bool Return(TimeTask obj) => true;
}