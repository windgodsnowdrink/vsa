#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
#:package System.Text.Json@8.0.0
#:package Quartz@3.8.0
#:package Cronos@0.12.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Scheduler.Core
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Scheduler 核心工具");
            
            // 创建添加任务命令
            var addCommand = new Command("add", "添加新任务");
            var nameOption = new Option<string>("--name", "任务名称");
            var typeOption = new Option<string>("--type", "任务类型 (cron/delay/interval)");
            var expressionOption = new Option<string>("--expression", "Cron 表达式或间隔时间");
            var handlerOption = new Option<string>("--handler", "任务处理程序");
            var groupOption = new Option<string>("--group", () => "default", "任务分组");
            var priorityOption = new Option<int>("--priority", () => 5, "任务优先级");
            addCommand.AddOption(nameOption);
            addCommand.AddOption(typeOption);
            addCommand.AddOption(expressionOption);
            addCommand.AddOption(handlerOption);
            addCommand.AddOption(groupOption);
            addCommand.AddOption(priorityOption);
            addCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(nameOption);
                var type = context.ParseResult.GetValueForOption(typeOption);
                var expression = context.ParseResult.GetValueForOption(expressionOption);
                var handler = context.ParseResult.GetValueForOption(handlerOption);
                var group = context.ParseResult.GetValueForOption(groupOption);
                var priority = context.ParseResult.GetValueForOption(priorityOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleAddCommand(serviceProvider, name, type, expression, handler, group, priority, cancellationToken);
            });
            
            // 创建列出任务命令
            var listCommand = new Command("list", "列出所有任务");
            var formatOption = new Option<string>("--format", () => "json", "输出格式 (json/text/yaml)");
            var listGroupOption = new Option<string>("--group", "按分组筛选");
            listCommand.AddOption(formatOption);
            listCommand.AddOption(listGroupOption);
            listCommand.SetHandler(async (context) =>
            {
                var format = context.ParseResult.GetValueForOption(formatOption);
                var group = context.ParseResult.GetValueForOption(listGroupOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleListCommand(serviceProvider, format, group, cancellationToken);
            });
            
            // 创建删除任务命令
            var removeCommand = new Command("remove", "删除任务");
            var removeNameOption = new Option<string>("--name", "任务名称");
            var removeGroupOption = new Option<string>("--group", () => "default", "任务分组");
            removeCommand.AddOption(removeNameOption);
            removeCommand.AddOption(removeGroupOption);
            removeCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(removeNameOption);
                var group = context.ParseResult.GetValueForOption(removeGroupOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleRemoveCommand(serviceProvider, name, group, cancellationToken);
            });
            
            // 创建启动命令
            var startCommand = new Command("start", "启动任务调度器");
            startCommand.SetHandler(async (context) =>
            {
                var cancellationToken = context.GetCancellationToken();
                await HandleStartCommand(serviceProvider, cancellationToken);
            });
            
            // 创建停止命令
            var stopCommand = new Command("stop", "停止任务调度器");
            stopCommand.SetHandler(async (context) =>
            {
                var cancellationToken = context.GetCancellationToken();
                await HandleStopCommand(serviceProvider, cancellationToken);
            });
            
            // 创建状态命令
            var statusCommand = new Command("status", "查看任务调度器状态");
            statusCommand.SetHandler(async (context) =>
            {
                var cancellationToken = context.GetCancellationToken();
                await HandleStatusCommand(serviceProvider, cancellationToken);
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(addCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(removeCommand);
            rootCommand.AddCommand(startCommand);
            rootCommand.AddCommand(stopCommand);
            rootCommand.AddCommand(statusCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 配置配置管理
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
            
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs", "scheduler.log"));
            });
            
            // 注册服务
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<ITaskExecutor, TaskExecutor>();
            services.AddSingleton<IJobStore, FileJobStore>();
            services.AddSingleton<ICronParser, CronParser>();
            services.AddSingleton<IDelayScheduler, DelayScheduler>();
            services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
            services.AddSingleton<ISchedulerHostedService, SchedulerHostedService>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleAddCommand(IServiceProvider serviceProvider, string name, string type, string expression, string handler, string group, int priority, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("错误: 必须指定任务名称");
                return;
            }
            
            if (string.IsNullOrEmpty(type))
            {
                Console.WriteLine("错误: 必须指定任务类型");
                return;
            }
            
            if (string.IsNullOrEmpty(expression))
            {
                Console.WriteLine("错误: 必须指定调度表达式");
                return;
            }
            
            if (string.IsNullOrEmpty(handler))
            {
                Console.WriteLine("错误: 必须指定任务处理程序");
                return;
            }
            
            try
            {
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                var task = new ScheduleTask
                {
                    Name = name,
                    Group = group,
                    Type = type,
                    Expression = expression,
                    Handler = handler,
                    Priority = priority,
                    Status = TaskStatus.Pending,
                    MaxRetries = 3,
                    RetryInterval = TimeSpan.FromMinutes(1),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                var addedTask = await schedulerService.AddTaskAsync(task, cancellationToken);
                Console.WriteLine($"成功添加任务: {addedTask.Name} (类型: {addedTask.Type}, 表达式: {addedTask.Expression})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加任务时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleListCommand(IServiceProvider serviceProvider, string format, string group, CancellationToken cancellationToken)
        {
            try
            {
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                var tasks = await schedulerService.GetTasksAsync(group, cancellationToken);
                
                switch (format.ToLower())
                {
                    case "json":
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                        var jsonContent = JsonSerializer.Serialize(tasks, jsonOptions);
                        Console.WriteLine(jsonContent);
                        break;
                    case "text":
                        Console.WriteLine("任务列表:");
                        Console.WriteLine("-" + new string('-', 100) + "-");
                        Console.WriteLine($"{'名称',-20} {'分组',-15} {'类型',-10} {'表达式',-20} {'状态',-10} {'优先级',-8}");
                        Console.WriteLine("-" + new string('-', 100) + "-");
                        foreach (var task in tasks)
                        {
                            Console.WriteLine($"{task.Name,-20} {task.Group,-15} {task.Type,-10} {task.Expression,-20} {task.Status,-10} {task.Priority,-8}");
                        }
                        Console.WriteLine("-" + new string('-', 100) + "-");
                        break;
                    case "yaml":
                        foreach (var task in tasks)
                        {
                            Console.WriteLine($"- name: {task.Name}");
                            Console.WriteLine($"  group: {task.Group}");
                            Console.WriteLine($"  type: {task.Type}");
                            Console.WriteLine($"  expression: {task.Expression}");
                            Console.WriteLine($"  handler: {task.Handler}");
                            Console.WriteLine($"  status: {task.Status}");
                            Console.WriteLine($"  priority: {task.Priority}");
                            Console.WriteLine();
                        }
                        break;
                    default:
                        Console.WriteLine("错误: 不支持的输出格式");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"列出任务时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleRemoveCommand(IServiceProvider serviceProvider, string name, string group, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("错误: 必须指定任务名称");
                return;
            }
            
            try
            {
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                var success = await schedulerService.RemoveTaskAsync(name, group, cancellationToken);
                
                if (success)
                {
                    Console.WriteLine($"成功删除任务: {name} (分组: {group})");
                }
                else
                {
                    Console.WriteLine($"未找到任务: {name} (分组: {group})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除任务时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleStartCommand(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            try
            {
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                await schedulerService.StartAsync(cancellationToken);
                Console.WriteLine("任务调度器已启动");
                
                // 等待用户输入以保持运行
                Console.WriteLine("按 Ctrl+C 停止调度器...");
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("调度器正在停止...");
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                await schedulerService.StopAsync(cancellationToken);
                Console.WriteLine("任务调度器已停止");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"启动调度器时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleStopCommand(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            try
            {
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                await schedulerService.StopAsync(cancellationToken);
                Console.WriteLine("任务调度器已停止");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"停止调度器时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleStatusCommand(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            try
            {
                var schedulerService = serviceProvider.GetRequiredService<ISchedulerService>();
                var status = schedulerService.GetStatus();
                
                Console.WriteLine("任务调度器状态:");
                Console.WriteLine("-" + new string('-', 60) + "-");
                Console.WriteLine($"运行状态: {(status.IsRunning ? "运行中" : "已停止")}");
                if (status.StartedAt.HasValue)
                {
                    Console.WriteLine($"启动时间: {status.StartedAt.Value}");
                    Console.WriteLine($"运行时间: {status.Uptime}");
                }
                Console.WriteLine($"任务总数: {status.TotalTasks}");
                Console.WriteLine($"运行中任务: {status.RunningTasks}");
                Console.WriteLine($"等待中任务: {status.PendingTasks}");
                Console.WriteLine($"失败任务: {status.FailedTasks}");
                Console.WriteLine("-" + new string('-', 60) + "-");
                
                if (status.RecentExecutions.Any())
                {
                    Console.WriteLine("最近执行的任务:");
                    Console.WriteLine($"{'时间',-25} {'任务',-20} {'状态',-10} {'耗时',-10}");
                    foreach (var execution in status.RecentExecutions)
                    {
                        Console.WriteLine($"{execution.ExecutionTime,-25} {execution.TaskName,-20} {execution.Status,-10} {execution.Duration,-10}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取调度器状态时发生错误: {ex.Message}");
            }
        }
    }
    
    // 服务接口
    public interface ISchedulerService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        SchedulerStatus GetStatus();
        Task<ScheduleTask> AddTaskAsync(ScheduleTask task, CancellationToken cancellationToken = default);
        Task<bool> RemoveTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
        Task<ScheduleTask> GetTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<ScheduleTask>> GetTasksAsync(string group = null, CancellationToken cancellationToken = default);
        Task<bool> PauseTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
        Task<bool> ResumeTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
        Task<bool> ExecuteTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
    }
    
    public interface ITaskExecutor
    {
        Task<TaskExecutionResult> ExecuteAsync(ScheduleTask task, CancellationToken cancellationToken = default);
        Task<bool> CancelAsync(string taskId, CancellationToken cancellationToken = default);
        TaskExecutionStatus GetExecutionStatus(string taskId);
    }
    
    public interface IJobStore
    {
        Task SaveTaskAsync(ScheduleTask task, CancellationToken cancellationToken = default);
        Task<ScheduleTask> GetTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<ScheduleTask>> GetTasksAsync(string group = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteTaskAsync(string name, string group = null, CancellationToken cancellationToken = default);
        Task SaveExecutionRecordAsync(TaskExecutionRecord record, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskExecutionRecord>> GetExecutionRecordsAsync(string taskName, string group = null, int limit = 100, CancellationToken cancellationToken = default);
    }
    
    public interface ICronParser
    {
        bool IsValidExpression(string expression);
        DateTime GetNextExecutionTime(string expression, DateTime baseTime = default);
    }
    
    public interface IDelayScheduler
    {
        Task<DateTime> CalculateNextExecutionTime(string expression, DateTime baseTime = default);
    }
    
    public interface IIntervalScheduler
    {
        Task<DateTime> CalculateNextExecutionTime(string expression, DateTime baseTime = default);
    }
    
    public interface ISchedulerHostedService : IHostedService
    {
    }
    
    // 数据结构
    public class ScheduleTask
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string Expression { get; set; }
        public string Handler { get; set; }
        public int Priority { get; set; }
        public TaskStatus Status { get; set; }
        public int MaxRetries { get; set; }
        public TimeSpan RetryInterval { get; set; }
        public List<string> Dependencies { get; set; } = new List<string>();
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public enum TaskStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Paused,
        Cancelled
    }
    
    public class TaskExecutionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int RetryCount { get; set; }
    }
    
    public enum TaskExecutionStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }
    
    public class SchedulerStatus
    {
        public bool IsRunning { get; set; }
        public DateTime? StartedAt { get; set; }
        public TimeSpan? Uptime { get; set; }
        public int TotalTasks { get; set; }
        public int RunningTasks { get; set; }
        public int PendingTasks { get; set; }
        public int FailedTasks { get; set; }
        public List<TaskExecutionRecord> RecentExecutions { get; set; } = new List<TaskExecutionRecord>();
    }
    
    public class TaskExecutionRecord
    {
        public string TaskName { get; set; }
        public string TaskGroup { get; set; }
        public DateTime ExecutionTime { get; set; }
        public TaskExecutionStatus Status { get; set; }
        public TimeSpan Duration { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
    
    // 实现类
    public class SchedulerService : ISchedulerService
    {
        private readonly ITaskExecutor _taskExecutor;
        private readonly IJobStore _jobStore;
        private readonly ICronParser _cronParser;
        private readonly IDelayScheduler _delayScheduler;
        private readonly IIntervalScheduler _intervalScheduler;
        private readonly ILogger<SchedulerService> _logger;
        
        private bool _isRunning;
        private DateTime? _startedAt;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _schedulingTask;
        private Dictionary<string, Task> _runningTasks = new Dictionary<string, Task>();
        
        public SchedulerService(
            ITaskExecutor taskExecutor,
            IJobStore jobStore,
            ICronParser cronParser,
            IDelayScheduler delayScheduler,
            IIntervalScheduler intervalScheduler,
            ILogger<SchedulerService> logger)
        {
            _taskExecutor = taskExecutor;
            _jobStore = jobStore;
            _cronParser = cronParser;
            _delayScheduler = delayScheduler;
            _intervalScheduler = intervalScheduler;
            _logger = logger;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_isRunning)
            {
                _logger.LogInformation("调度器已经在运行中");
                return;
            }
            
            _logger.LogInformation("正在启动任务调度器");
            _isRunning = true;
            _startedAt = DateTime.UtcNow;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            // 启动调度循环
            _schedulingTask = Task.Run(async () => await SchedulingLoopAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            
            _logger.LogInformation("任务调度器已启动");
        }
        
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (!_isRunning)
            {
                _logger.LogInformation("调度器已经停止");
                return;
            }
            
            _logger.LogInformation("正在停止任务调度器");
            
            // 取消调度循环
            _cancellationTokenSource.Cancel();
            
            // 等待调度循环结束
            if (_schedulingTask != null)
            {
                await Task.WhenAny(_schedulingTask, Task.Delay(30000, cancellationToken));
            }
            
            // 取消所有运行中的任务
            foreach (var task in _runningTasks.Values)
            {
                if (!task.IsCompleted)
                {
                    try
                    {
                        // 尝试取消任务
                    }
                    catch { }
                }
            }
            
            _isRunning = false;
            _logger.LogInformation("任务调度器已停止");
        }
        
        public SchedulerStatus GetStatus()
        {
            var tasks = _jobStore.GetTasksAsync().GetAwaiter().GetResult();
            var recentExecutions = _jobStore.GetExecutionRecordsAsync(null, null, 5).GetAwaiter().GetResult().ToList();
            
            return new SchedulerStatus
            {
                IsRunning = _isRunning,
                StartedAt = _startedAt,
                Uptime = _startedAt.HasValue ? DateTime.UtcNow - _startedAt.Value : null,
                TotalTasks = tasks.Count(),
                RunningTasks = tasks.Count(t => t.Status == TaskStatus.Running),
                PendingTasks = tasks.Count(t => t.Status == TaskStatus.Pending),
                FailedTasks = tasks.Count(t => t.Status == TaskStatus.Failed),
                RecentExecutions = recentExecutions
            };
        }
        
        public async Task<ScheduleTask> AddTaskAsync(ScheduleTask task, CancellationToken cancellationToken = default)
        {
            // 验证任务类型
            if (!IsValidTaskType(task.Type))
            {
                throw new InvalidOperationException($"无效的任务类型: {task.Type}");
            }
            
            // 验证表达式
            if (!await ValidateExpressionAsync(task.Type, task.Expression, cancellationToken))
            {
                throw new InvalidOperationException($"无效的调度表达式: {task.Expression}");
            }
            
            // 检查任务是否已存在
            var existingTask = await _jobStore.GetTaskAsync(task.Name, task.Group, cancellationToken);
            if (existingTask != null)
            {
                throw new InvalidOperationException($"任务已存在: {task.Name} (分组: {task.Group})");
            }
            
            // 保存任务
            await _jobStore.SaveTaskAsync(task, cancellationToken);
            _logger.LogInformation($"添加任务成功: {task.Name} (分组: {task.Group})");
            
            return task;
        }
        
        public async Task<bool> RemoveTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            group ??= "default";
            var success = await _jobStore.DeleteTaskAsync(name, group, cancellationToken);
            
            if (success)
            {
                _logger.LogInformation($"删除任务成功: {name} (分组: {group})");
            }
            else
            {
                _logger.LogWarning($"删除任务失败: 任务不存在 {name} (分组: {group})");
            }
            
            return success;
        }
        
        public async Task<ScheduleTask> GetTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            return await _jobStore.GetTaskAsync(name, group, cancellationToken);
        }
        
        public async Task<IEnumerable<ScheduleTask>> GetTasksAsync(string group = null, CancellationToken cancellationToken = default)
        {
            return await _jobStore.GetTasksAsync(group, cancellationToken);
        }
        
        public async Task<bool> PauseTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            group ??= "default";
            var task = await _jobStore.GetTaskAsync(name, group, cancellationToken);
            if (task == null)
            {
                return false;
            }
            
            task.Status = TaskStatus.Paused;
            task.UpdatedAt = DateTime.UtcNow;
            await _jobStore.SaveTaskAsync(task, cancellationToken);
            _logger.LogInformation($"暂停任务成功: {name} (分组: {group})");
            
            return true;
        }
        
        public async Task<bool> ResumeTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            group ??= "default";
            var task = await _jobStore.GetTaskAsync(name, group, cancellationToken);
            if (task == null)
            {
                return false;
            }
            
            task.Status = TaskStatus.Pending;
            task.UpdatedAt = DateTime.UtcNow;
            await _jobStore.SaveTaskAsync(task, cancellationToken);
            _logger.LogInformation($"恢复任务成功: {name} (分组: {group})");
            
            return true;
        }
        
        public async Task<bool> ExecuteTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            group ??= "default";
            var task = await _jobStore.GetTaskAsync(name, group, cancellationToken);
            if (task == null)
            {
                return false;
            }
            
            // 立即执行任务
            await ExecuteTaskInternalAsync(task, cancellationToken);
            return true;
        }
        
        private async Task SchedulingLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // 获取所有任务
                    var tasks = await _jobStore.GetTasksAsync(null, cancellationToken);
                    
                    // 检查每个任务是否需要执行
                    foreach (var task in tasks.Where(t => t.Status == TaskStatus.Pending || t.Status == TaskStatus.Running))
                    {
                        if (await ShouldExecuteTaskAsync(task, cancellationToken))
                        {
                            // 执行任务
                            var taskId = $"{task.Group}.{task.Name}";
                            if (!_runningTasks.ContainsKey(taskId) || _runningTasks[taskId].IsCompleted)
                            {
                                _runningTasks[taskId] = Task.Run(async () => await ExecuteTaskInternalAsync(task, cancellationToken), cancellationToken);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "调度循环发生错误");
                }
                
                // 等待一段时间后再次检查
                await Task.Delay(1000, cancellationToken);
            }
        }
        
        private async Task ExecuteTaskInternalAsync(ScheduleTask task, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"开始执行任务: {task.Name} (分组: {task.Group})");
            
            // 更新任务状态
            task.Status = TaskStatus.Running;
            await _jobStore.SaveTaskAsync(task, cancellationToken);
            
            var startTime = DateTime.UtcNow;
            TaskExecutionResult result;
            
            try
            {
                // 执行任务
                result = await _taskExecutor.ExecuteAsync(task, cancellationToken);
                
                // 更新任务状态
                task.Status = result.Success ? TaskStatus.Completed : TaskStatus.Failed;
                task.UpdatedAt = DateTime.UtcNow;
                await _jobStore.SaveTaskAsync(task, cancellationToken);
                
                // 保存执行记录
                var executionRecord = new TaskExecutionRecord
                {
                    TaskName = task.Name,
                    TaskGroup = task.Group,
                    ExecutionTime = startTime,
                    Status = result.Success ? TaskExecutionStatus.Completed : TaskExecutionStatus.Failed,
                    Duration = result.Duration,
                    Message = result.Message,
                    Error = result.Exception?.ToString()
                };
                await _jobStore.SaveExecutionRecordAsync(executionRecord, cancellationToken);
                
                if (result.Success)
                {
                    _logger.LogInformation($"任务执行成功: {task.Name} (分组: {task.Group})，耗时: {result.Duration}");
                }
                else
                {
                    _logger.LogError(result.Exception, $"任务执行失败: {task.Name} (分组: {task.Group})");
                    
                    // 检查是否需要重试
                    if (task.MaxRetries > 0)
                    {
                        _logger.LogInformation($"将在 {task.RetryInterval} 后重试任务: {task.Name} (分组: {task.Group})");
                        // 这里可以实现重试逻辑
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"任务执行发生异常: {task.Name} (分组: {task.Group})");
                
                // 更新任务状态
                task.Status = TaskStatus.Failed;
                task.UpdatedAt = DateTime.UtcNow;
                await _jobStore.SaveTaskAsync(task, cancellationToken);
                
                // 保存执行记录
                var executionRecord = new TaskExecutionRecord
                {
                    TaskName = task.Name,
                    TaskGroup = task.Group,
                    ExecutionTime = startTime,
                    Status = TaskExecutionStatus.Failed,
                    Duration = DateTime.UtcNow - startTime,
                    Error = ex.ToString()
                };
                await _jobStore.SaveExecutionRecordAsync(executionRecord, cancellationToken);
            }
        }
        
        private async Task<bool> ShouldExecuteTaskAsync(ScheduleTask task, CancellationToken cancellationToken)
        {
            try
            {
                DateTime nextExecutionTime;
                
                switch (task.Type.ToLower())
                {
                    case "cron":
                        nextExecutionTime = _cronParser.GetNextExecutionTime(task.Expression);
                        break;
                    case "delay":
                        nextExecutionTime = await _delayScheduler.CalculateNextExecutionTime(task.Expression, task.CreatedAt);
                        break;
                    case "interval":
                        nextExecutionTime = await _intervalScheduler.CalculateNextExecutionTime(task.Expression, task.CreatedAt);
                        break;
                    default:
                        return false;
                }
                
                // 检查是否到达执行时间
                return DateTime.UtcNow >= nextExecutionTime;
            }
            catch
            {
                return false;
            }
        }
        
        private bool IsValidTaskType(string type)
        {
            return new[] { "cron", "delay", "interval" }.Contains(type.ToLower());
        }
        
        private async Task<bool> ValidateExpressionAsync(string type, string expression, CancellationToken cancellationToken)
        {
            try
            {
                switch (type.ToLower())
                {
                    case "cron":
                        return _cronParser.IsValidExpression(expression);
                    case "delay":
                    case "interval":
                        // 尝试解析时间间隔
                        var parts = expression.Split(':');
                        if (parts.Length != 3)
                        {
                            return false;
                        }
                        return int.TryParse(parts[0], out _) && int.TryParse(parts[1], out _) && int.TryParse(parts[2], out _);
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
    
    public class TaskExecutor : ITaskExecutor
    {
        private readonly ILogger<TaskExecutor> _logger;
        private readonly Dictionary<string, TaskExecutionStatus> _executionStatuses = new Dictionary<string, TaskExecutionStatus>();
        
        public TaskExecutor(ILogger<TaskExecutor> logger)
        {
            _logger = logger;
        }
        
        public async Task<TaskExecutionResult> ExecuteAsync(ScheduleTask task, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var result = new TaskExecutionResult
            {
                Success = false,
                StartTime = startTime
            };
            
            try
            {
                _logger.LogDebug($"正在执行任务处理程序: {task.Handler}");
                
                // 解析处理程序
                var handlerParts = task.Handler.Split('.');
                if (handlerParts.Length < 2)
                {
                    throw new InvalidOperationException($"无效的处理程序格式: {task.Handler}");
                }
                
                var className = handlerParts[0];
                var methodName = handlerParts[1];
                
                // 这里应该通过反射或依赖注入来执行处理程序
                // 为了简化，这里只做模拟执行
                
                // 模拟任务执行
                await Task.Delay(1000, cancellationToken);
                
                result.Success = true;
                result.Message = "任务执行成功";
                _logger.LogInformation($"任务处理程序执行成功: {task.Handler}");
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
                result.Message = "任务执行失败";
                _logger.LogError(ex, $"任务处理程序执行失败: {task.Handler}");
            }
            finally
            {
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime - result.StartTime;
            }
            
            return result;
        }
        
        public Task<bool> CancelAsync(string taskId, CancellationToken cancellationToken = default)
        {
            if (_executionStatuses.ContainsKey(taskId))
            {
                _executionStatuses[taskId] = TaskExecutionStatus.Cancelled;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        
        public TaskExecutionStatus GetExecutionStatus(string taskId)
        {
            return _executionStatuses.TryGetValue(taskId, out var status) ? status : TaskExecutionStatus.Pending;
        }
    }
    
    public class FileJobStore : IJobStore
    {
        private readonly string _tasksPath;
        private readonly string _executionsPath;
        private readonly ILogger<FileJobStore> _logger;
        
        public FileJobStore(ILogger<FileJobStore> logger)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "schedules");
            _tasksPath = Path.Combine(basePath, "tasks");
            _executionsPath = Path.Combine(basePath, "executions");
            _logger = logger;
            
            // 创建目录
            Directory.CreateDirectory(_tasksPath);
            Directory.CreateDirectory(_executionsPath);
        }
        
        public async Task SaveTaskAsync(ScheduleTask task, CancellationToken cancellationToken = default)
        {
            var filePath = GetTaskFilePath(task.Group, task.Name);
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(task, jsonOptions);
            
            await File.WriteAllTextAsync(filePath, jsonContent, cancellationToken);
            _logger.LogDebug($"保存任务成功: {task.Name} (分组: {task.Group})");
        }
        
        public async Task<ScheduleTask> GetTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            group ??= "default";
            var filePath = GetTaskFilePath(group, name);
            
            if (!File.Exists(filePath))
            {
                return null;
            }
            
            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                return JsonSerializer.Deserialize<ScheduleTask>(jsonContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"读取任务失败: {name} (分组: {group})");
                return null;
            }
        }
        
        public async Task<IEnumerable<ScheduleTask>> GetTasksAsync(string group = null, CancellationToken cancellationToken = default)
        {
            var tasks = new List<ScheduleTask>();
            
            try
            {
                if (group == null)
                {
                    // 获取所有分组
                    var groups = Directory.GetDirectories(_tasksPath);
                    foreach (var groupDir in groups)
                    {
                        var groupName = Path.GetFileName(groupDir);
                        tasks.AddRange(await GetTasksByGroupAsync(groupName, cancellationToken));
                    }
                }
                else
                {
                    tasks.AddRange(await GetTasksByGroupAsync(group, cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取任务列表失败");
            }
            
            return tasks;
        }
        
        public async Task<bool> DeleteTaskAsync(string name, string group = null, CancellationToken cancellationToken = default)
        {
            group ??= "default";
            var filePath = GetTaskFilePath(group, name);
            
            if (!File.Exists(filePath))
            {
                return false;
            }
            
            try
            {
                File.Delete(filePath);
                _logger.LogDebug($"删除任务成功: {name} (分组: {group})");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除任务失败: {name} (分组: {group})");
                return false;
            }
        }
        
        public async Task SaveExecutionRecordAsync(TaskExecutionRecord record, CancellationToken cancellationToken = default)
        {
            var filePath = GetExecutionFilePath(record.TaskGroup, record.TaskName, record.ExecutionTime);
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(record, jsonOptions);
            
            await File.WriteAllTextAsync(filePath, jsonContent, cancellationToken);
        }
        
        public async Task<IEnumerable<TaskExecutionRecord>> GetExecutionRecordsAsync(string taskName, string group = null, int limit = 100, CancellationToken cancellationToken = default)
        {
            var records = new List<TaskExecutionRecord>();
            
            try
            {
                if (group == null)
                {
                    // 获取所有分组
                    var groups = Directory.GetDirectories(_executionsPath);
                    foreach (var groupDir in groups)
                    {
                        var groupName = Path.GetFileName(groupDir);
                        records.AddRange(await GetExecutionRecordsByTaskAsync(groupName, taskName, limit, cancellationToken));
                    }
                }
                else
                {
                    records.AddRange(await GetExecutionRecordsByTaskAsync(group, taskName, limit, cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取执行记录失败");
            }
            
            return records.OrderByDescending(r => r.ExecutionTime).Take(limit);
        }
        
        private async Task<IEnumerable<ScheduleTask>> GetTasksByGroupAsync(string group, CancellationToken cancellationToken)
        {
            var tasks = new List<ScheduleTask>();
            var groupPath = Path.Combine(_tasksPath, group);
            
            if (!Directory.Exists(groupPath))
            {
                return tasks;
            }
            
            var files = Directory.GetFiles(groupPath, "*.json");
            foreach (var file in files)
            {
                try
                {
                    var jsonContent = await File.ReadAllTextAsync(file, cancellationToken);
                    var task = JsonSerializer.Deserialize<ScheduleTask>(jsonContent);
                    if (task != null)
                    {
                        tasks.Add(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"读取任务文件失败: {file}");
                }
            }
            
            return tasks;
        }
        
        private async Task<IEnumerable<TaskExecutionRecord>> GetExecutionRecordsByTaskAsync(string group, string taskName, int limit, CancellationToken cancellationToken)
        {
            var records = new List<TaskExecutionRecord>();
            var taskPath = Path.Combine(_executionsPath, group, taskName);
            
            if (!Directory.Exists(taskPath))
            {
                return records;
            }
            
            var files = Directory.GetFiles(taskPath, "*.json").OrderByDescending(f => f).Take(limit);
            foreach (var file in files)
            {
                try
                {
                    var jsonContent = await File.ReadAllTextAsync(file, cancellationToken);
                    var record = JsonSerializer.Deserialize<TaskExecutionRecord>(jsonContent);
                    if (record != null)
                    {
                        records.Add(record);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"读取执行记录文件失败: {file}");
                }
            }
            
            return records;
        }
        
        private string GetTaskFilePath(string group, string name)
        {
            var groupPath = Path.Combine(_tasksPath, group);
            Directory.CreateDirectory(groupPath);
            return Path.Combine(groupPath, $"{name}.json");
        }
        
        private string GetExecutionFilePath(string group, string taskName, DateTime executionTime)
        {
            var taskPath = Path.Combine(_executionsPath, group, taskName);
            Directory.CreateDirectory(taskPath);
            return Path.Combine(taskPath, $"{executionTime:yyyyMMddHHmmss}.json");
        }
    }
    
    public class CronParser : ICronParser
    {
        public bool IsValidExpression(string expression)
        {
            try
            {
                var parsed = Cronos.CronExpression.Parse(expression);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public DateTime GetNextExecutionTime(string expression, DateTime baseTime = default)
        {
            if (baseTime == default)
            {
                baseTime = DateTime.UtcNow;
            }
            
            var parsed = Cronos.CronExpression.Parse(expression);
            return parsed.GetNextOccurrence(baseTime).GetValueOrDefault(baseTime.AddDays(1));
        }
    }
    
    public class DelayScheduler : IDelayScheduler
    {
        public async Task<DateTime> CalculateNextExecutionTime(string expression, DateTime baseTime = default)
        {
            if (baseTime == default)
            {
                baseTime = DateTime.UtcNow;
            }
            
            // 解析时间间隔
            var parts = expression.Split(':');
            if (parts.Length != 3)
            {
                throw new InvalidOperationException($"无效的延迟表达式: {expression}");
            }
            
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            
            var delay = new TimeSpan(hours, minutes, seconds);
            return baseTime.Add(delay);
        }
    }
    
    public class IntervalScheduler : IIntervalScheduler
    {
        public async Task<DateTime> CalculateNextExecutionTime(string expression, DateTime baseTime = default)
        {
            if (baseTime == default)
            {
                baseTime = DateTime.UtcNow;
            }
            
            // 解析时间间隔
            var parts = expression.Split(':');
            if (parts.Length != 3)
            {
                throw new InvalidOperationException($"无效的间隔表达式: {expression}");
            }
            
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            
            var interval = new TimeSpan(hours, minutes, seconds);
            
            // 计算下一次执行时间
            var now = DateTime.UtcNow;
            var elapsed = now - baseTime;
            var intervals = elapsed.Ticks / interval.Ticks;
            return baseTime.AddTicks((intervals + 1) * interval.Ticks);
        }
    }
    
    public class SchedulerHostedService : BackgroundService, ISchedulerHostedService
    {
        private readonly ISchedulerService _schedulerService;
        private readonly ILogger<SchedulerHostedService> _logger;
        
        public SchedulerHostedService(ISchedulerService schedulerService, ILogger<SchedulerHostedService> logger)
        {
            _schedulerService = schedulerService;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("启动托管调度服务");
            await _schedulerService.StartAsync(stoppingToken);
            
            // 等待停止信号
            await stoppingToken.WhenCanceled();
            
            _logger.LogInformation("停止托管调度服务");
            await _schedulerService.StopAsync(stoppingToken);
        }
    }
    
    // 扩展方法
    public static class CancellationTokenExtensions
    {
        public static Task WhenCanceled(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
    
    // 日志扩展
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
        {
            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // 这里应该添加文件日志提供程序
            // 为了简化，这里只做模拟
            
            return builder;
        }
    }
}
