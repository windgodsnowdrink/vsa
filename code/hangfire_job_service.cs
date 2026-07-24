#:sdk Microsoft.NET.Sdk.Web
#:package Hangfire.Core@1.8.0
#:package Hangfire.Redis@2.8.0
#:package NCrontab@3.3.1
#:package StackExchange.Redis@2.6.116
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Hangfire;
using Hangfire.Redis;
using NCrontab;
using StackExchange.Redis;

// 1. 高性能Job处理器(Disruptor模式)
[SkipLocalsInit]
public sealed class JobProcessor : IAsyncDisposable
{
    private readonly Channel<JobItem> _jobChannel;
    private readonly IBackgroundJobClient _jobClient;
    private readonly CancellationTokenSource _cts = new();
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public JobProcessor(IBackgroundJobClient jobClient)
    {
        _jobClient = jobClient;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _jobChannel = Channel.CreateBounded<JobItem>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task EnqueueJobAsync(JobItem job)
    {
        await _jobChannel.Writer.WriteAsync(job);
    }

    private async Task ProcessJobsAsync()
    {
        await foreach (var job in _jobChannel.Reader.ReadAllAsync(_cts.Token))
        {
            _latencyOptimizer.Optimize(() => 
            {
                _jobClient.Enqueue(() => ExecuteJob(job));
            });
        }
    }

    public static void ExecuteJob(JobItem job)
    {
        // 实际执行任务的逻辑
        Console.WriteLine($"Executing job: {job.Id}");
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _jobChannel.Writer.Complete();
    }
}

// 2. Hangfire服务扩展
public static class HangfireServiceExtensions
{
    public static IServiceCollection AddHangfireWithRedis(
        this IServiceCollection services,
        Action<HangfireOptions> configure)
    {
        var options = new HangfireOptions();
        configure(options);
        
        // Redis连接配置
        var redis = ConnectionMultiplexer.Connect(options.RedisConnectionString);
        
        // Hangfire配置
        services.AddHangfire(config =>
        {
            config.UseRedisStorage(redis, new RedisStorageOptions
            {
                Prefix = "jobs:",
                Db = options.RedisDatabase,
                InvisibilityTimeout = TimeSpan.FromMinutes(30)
            });
            
            config.UseFilter(new AutomaticRetryAttribute { Attempts = 3 });
        });
        
        // 注册Job处理器
        services.AddSingleton<JobProcessor>();
        
        return services;
    }
}

// 3. 定时任务调度器
public class CronScheduler : BackgroundService
{
    private readonly CrontabSchedule _schedule;
    private readonly IBackgroundJobClient _jobClient;
    
    public CronScheduler(IBackgroundJobClient jobClient)
    {
        _jobClient = jobClient;
        _schedule = CrontabSchedule.Parse("*/5 * * * *"); // 每5分钟执行
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nextRun = _schedule.GetNextOccurrence(DateTime.Now);
            var delay = nextRun - DateTime.Now;
            
            await Task.Delay(delay, stoppingToken);
            
            _jobClient.Enqueue(() => JobProcessor.ExecuteJob(new JobItem
            {
                Id = Guid.NewGuid(),
                Type = "cron"
            }));
        }
    }
}

// 4. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置Hangfire+Redis
builder.Services.AddHangfireWithRedis(options =>
{
    options.RedisConnectionString = "localhost:6379";
    options.RedisDatabase = 0;
});

// 注册定时任务
builder.Services.AddHostedService<CronScheduler>();

var app = builder.Build();

// Hangfire仪表板
app.UseHangfireDashboard("/jobs");

// Job端点
app.MapPost("/jobs", async (JobItem job, JobProcessor processor) =>
{
    await processor.EnqueueJobAsync(job);
    return Results.Ok();
});

app.Run();

// 5. 辅助类
public record JobItem(Guid Id, string Type);
public class HangfireOptions
{
    public string RedisConnectionString { get; set; }
    public int RedisDatabase { get; set; }
}