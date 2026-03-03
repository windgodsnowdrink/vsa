# AgileConfig 配置管理技术 - 示例说明

## AgileConfig集成示例

### 基本集成

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package AgileConfig.Client@2.0.0
#:package Microsoft.Extensions.Hosting@10.0.1

using AgileConfig.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 注册AgileConfig客户端服务
builder.Services.AddAgileConfig(options =>
{
    options.AppId = "your-app-id";
    options.Secret = "your-app-secret";
    options.Nodes = "http://localhost:5000";
    options.Tenant = "default";
});

var app = builder.Build();

// 使用配置
var config = app.Services.GetRequiredService<IConfiguration>();
var appName = config["AppName"];

app.MapGet("/", () => $"Hello from {appName} using AgileConfig!");
app.Run();
```

### 配置变更事件监听

```csharp
// 监听配置变更事件
var configClient = app.Services.GetRequiredService<IConfigClient>();
configClient.ConfigChanged += (sender, e) =>
{
    Console.WriteLine($"配置变更: {e.Key} = {e.Value}");
};
```

### 配置值绑定

```csharp
// 定义配置类
public class AppSettings
{
    public string AppName { get; set; }
    public int Port { get; set; }
    public string Environment { get; set; }
}

// 绑定配置
var appSettings = new AppSettings();
config.Bind("AppSettings", appSettings);
Console.WriteLine($"AppName: {appSettings.AppName}");
```

## 时间轮服务示例

### 基本使用

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Hosting@10.0.1

using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 注册时间轮服务
builder.Services.AddSingleton<TimeWheelService>();

var app = builder.Build();

// 获取时间轮服务
var timeWheelService = app.Services.GetRequiredService<TimeWheelService>();

// 添加定时任务
var taskId = timeWheelService.AddTask(
    delay: TimeSpan.FromSeconds(5),
    callback: () => Console.WriteLine("定时任务执行!")
);

// 添加周期性任务
var periodicTaskId = timeWheelService.AddPeriodicTask(
    interval: TimeSpan.FromSeconds(10),
    callback: () => Console.WriteLine("周期性任务执行!")
);

// 启动时间轮
await timeWheelService.StartAsync();

app.MapGet("/", () => "时间轮服务已启动!");
app.Run();
```

### 时间轮服务实现

```csharp
public class TimeWheelService
{
    private readonly Channel<TimeWheelTask> _taskChannel;
    private readonly Dictionary<Guid, TimeWheelTask> _tasks;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly SemaphoreSlim _semaphore;
    private bool _isRunning;
    private int _currentTick;
    private readonly int _wheelSize;
    private readonly TimeSpan _tickInterval;

    public TimeWheelService(int wheelSize = 60, TimeSpan? tickInterval = null)
    {
        _wheelSize = wheelSize;
        _tickInterval = tickInterval ?? TimeSpan.FromSeconds(1);
        _taskChannel = Channel.CreateUnbounded<TimeWheelTask>();
        _tasks = new Dictionary<Guid, TimeWheelTask>();
        _cancellationTokenSource = new CancellationTokenSource();
        _semaphore = new SemaphoreSlim(1);
        _isRunning = false;
        _currentTick = 0;
    }

    public Guid AddTask(TimeSpan delay, Action callback)
    {
        var task = new TimeWheelTask
        {
            Id = Guid.NewGuid(),
            Delay = delay,
            Callback = callback,
            IsPeriodic = false
        };
        _taskChannel.Writer.TryWrite(task);
        return task.Id;
    }

    public Guid AddPeriodicTask(TimeSpan interval, Action callback)
    {
        var task = new TimeWheelTask
        {
            Id = Guid.NewGuid(),
            Delay = interval,
            Callback = callback,
            IsPeriodic = true
        };
        _taskChannel.Writer.TryWrite(task);
        return task.Id;
    }

    public async Task StartAsync()
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _ = Task.Run(async () => await ProcessTasksAsync());
        _ = Task.Run(async () => await RunTimeWheelAsync());
    }

    private async Task ProcessTasksAsync()
    {
        await foreach (var task in _taskChannel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
        {
            await _semaphore.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                _tasks[task.Id] = task;
                Console.WriteLine($"任务添加成功: {task.Id}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }

    private async Task RunTimeWheelAsync()
    {
        while (_isRunning)
        {
            await _semaphore.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                // 处理当前刻度的任务
                var tasksToExecute = _tasks.Values
                    .Where(t => (int)(t.Delay.TotalSeconds / _tickInterval.TotalSeconds) <= _currentTick)
                    .ToList();

                foreach (var task in tasksToExecute)
                {
                    // 执行任务
                    try
                    {
                        task.Callback?.Invoke();
                        Console.WriteLine($"任务执行成功: {task.Id}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"任务执行失败: {task.Id}, 错误: {ex.Message}");
                    }

                    // 如果是周期性任务，重新添加
                    if (task.IsPeriodic)
                    {
                        task.Delay = task.Delay.Add(TimeSpan.FromSeconds(_wheelSize * _tickInterval.TotalSeconds));
                    }
                    else
                    {
                        _tasks.Remove(task.Id);
                    }
                }

                // 更新刻度
                _currentTick = (_currentTick + 1) % _wheelSize;
            }
            finally
            {
                _semaphore.Release();
            }

            await Task.Delay(_tickInterval, _cancellationTokenSource.Token);
        }
    }

    public async Task StopAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;
        _cancellationTokenSource.Cancel();
        await _semaphore.WaitAsync();
        try
        {
            _tasks.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

public class TimeWheelTask
{
    public Guid Id { get; set; }
    public TimeSpan Delay { get; set; }
    public Action Callback { get; set; }
    public bool IsPeriodic { get; set; }
}
```

## 性能优化建议

1. **使用适当的时间轮大小**：根据任务数量和延迟要求选择合适的时间轮大小
2. **批量处理任务**：在每个刻度批量处理所有到期任务
3. **使用异步回调**：对于耗时任务，建议使用异步回调避免阻塞时间轮
4. **合理设置tick间隔**：根据任务精度要求设置合适的tick间隔
5. **定期清理过期任务**：及时清理非周期性任务，避免内存泄漏

## 分布式部署建议

1. **配置中心集群**：部署多个AgileConfig服务节点，实现高可用
2. **客户端负载均衡**：客户端配置多个服务节点，实现负载均衡
3. **配置数据持久化**：将配置数据持久化到数据库，避免数据丢失
4. **配置版本管理**：启用配置版本管理，支持配置回滚
5. **灰度发布策略**：根据业务需求制定合理的灰度发布策略

## 监控和告警

1. **配置变更监控**：监控配置变更记录，及时发现异常变更
2. **配置中心健康检查**：定期检查配置中心服务健康状态
3. **配置一致性检查**：确保各节点配置一致
4. **告警机制**：配置变更、服务异常等情况及时告警
5. **性能监控**：监控配置中心的响应时间和吞吐量
