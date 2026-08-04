# Scheduler 技能使用示例文档

## 1. 基础使用示例

### 1.1 添加 Cron 任务

**功能说明**：添加一个基于 cron 表达式的定时任务，每天凌晨 1 点执行备份操作。

**命令示例**：
```bash
scheduler_core add --name backup --type cron --expression "0 1 * * *" --handler "BackupService.Execute" --group maintenance --priority 5
```

**执行结果**：
```
任务添加成功：
名称：backup
组：maintenance
类型：cron
表达式：0 1 * * *
处理器：BackupService.Execute
优先级：5
状态：已启用
```

### 1.2 添加延迟任务

**功能说明**：添加一个延迟任务，5 分钟后发送通知。

**命令示例**：
```bash
scheduler_core add --name notify --type delay --expression "00:05:00" --handler "NotificationService.Send" --group notification --priority 3
```

**执行结果**：
```
任务添加成功：
名称：notify
组：notification
类型：delay
表达式：00:05:00
处理器：NotificationService.Send
优先级：3
状态：已启用
```

### 1.3 添加间隔任务

**功能说明**：添加一个间隔任务，每 10 分钟执行一次清理操作。

**命令示例**：
```bash
scheduler_core add --name cleanup --type interval --expression "00:10:00" --handler "CleanupService.Execute" --group maintenance --priority 4
```

**执行结果**：
```
任务添加成功：
名称：cleanup
组：maintenance
类型：interval
表达式：00:10:00
处理器：CleanupService.Execute
优先级：4
状态：已启用
```

### 1.4 列出所有任务

**功能说明**：列出所有已添加的任务。

**命令示例**：
```bash
scheduler_core list --format text
```

**执行结果**：
```
┌──────────┬──────────────┬──────────┬───────────────┬──────────────────────┬──────────┬────────┐
│ 名称     │ 组           │ 类型     │ 表达式        │ 处理器               │ 优先级   │ 状态   │
├──────────┼──────────────┼──────────┼───────────────┼──────────────────────┼──────────┼────────┤
│ backup   │ maintenance  │ cron     │ 0 1 * * *     │ BackupService.Execute │ 5        │ 启用   │
│ notify   │ notification │ delay    │ 00:05:00      │ NotificationService.Send │ 3        │ 启用   │
│ cleanup  │ maintenance  │ interval │ 00:10:00      │ CleanupService.Execute │ 4        │ 启用   │
└──────────┴──────────────┴──────────┴───────────────┴──────────────────────┴──────────┴────────┘
```

### 1.5 移除任务

**功能说明**：移除指定的任务。

**命令示例**：
```bash
scheduler_core remove --name notify --group notification
```

**执行结果**：
```
任务移除成功：
名称：notify
组：notification
```

### 1.6 启动调度器

**功能说明**：启动任务调度器。

**命令示例**：
```bash
scheduler_core start
```

**执行结果**：
```
调度器启动成功
状态：运行中
```

### 1.7 停止调度器

**功能说明**：停止任务调度器。

**命令示例**：
```bash
scheduler_core stop
```

**执行结果**：
```
调度器停止成功
状态：已停止
```

### 1.8 查看调度器状态

**功能说明**：查看调度器的当前状态。

**命令示例**：
```bash
scheduler_core status
```

**执行结果**：
```
调度器状态：运行中
任务数量：2
正在执行的任务：0
```

### 1.9 查看执行历史

**功能说明**：查看任务执行历史记录。

**命令示例**：
```bash
scheduler_core history --start "2026-01-01" --end "2026-01-31" --format text
```

**执行结果**：
```
┌────────────────────┬──────────┬──────────────┬────────┬───────────┬────────────┐
│ 执行时间           │ 任务名称 │ 任务组       │ 成功   │ 消息      │ 持续时间   │
├────────────────────┼──────────┼──────────────┼────────┼───────────┼────────────┤
│ 2026-01-24 01:00:00 │ backup   │ maintenance  │ 是     │ 备份完成  │ 00:05:23   │
│ 2026-01-24 01:10:00 │ cleanup  │ maintenance  │ 是     │ 清理完成  │ 00:01:45   │
│ 2026-01-24 01:20:00 │ cleanup  │ maintenance  │ 是     │ 清理完成  │ 00:01:38   │
└────────────────────┴──────────┴──────────────┴────────┴───────────┴────────────┘
```

## 2. 高级使用示例

### 2.1 添加带参数的任务

**功能说明**：添加一个带参数的任务。

**命令示例**：
```bash
scheduler_core add --name report --type cron --expression "0 8 * * *" --handler "ReportService.Generate" --group business --priority 2 --parameters '{"reportType":"daily","email":"admin@example.com"}'
```

**执行结果**：
```
任务添加成功：
名称：report
组：business
类型：cron
表达式：0 8 * * *
处理器：ReportService.Generate
优先级：2
参数：{"reportType":"daily","email":"admin@example.com"}
状态：已启用
```

### 2.2 使用 JSON 格式输出

**功能说明**：使用 JSON 格式列出任务。

**命令示例**：
```bash
scheduler_core list --format json
```

**执行结果**：
```json
[
  {
    "name": "backup",
    "group": "maintenance",
    "type": "cron",
    "expression": "0 1 * * *",
    "handler": "BackupService.Execute",
    "priority": 5,
    "enabled": true,
    "createdAt": "2026-01-24T00:00:00Z",
    "lastModifiedAt": "2026-01-24T00:00:00Z"
  },
  {
    "name": "cleanup",
    "group": "maintenance",
    "type": "interval",
    "expression": "00:10:00",
    "handler": "CleanupService.Execute",
    "priority": 4,
    "enabled": true,
    "createdAt": "2026-01-24T00:00:00Z",
    "lastModifiedAt": "2026-01-24T00:00:00Z"
  },
  {
    "name": "report",
    "group": "business",
    "type": "cron",
    "expression": "0 8 * * *",
    "handler": "ReportService.Generate",
    "priority": 2,
    "parameters": {
      "reportType": "daily",
      "email": "admin@example.com"
    },
    "enabled": true,
    "createdAt": "2026-01-24T00:00:00Z",
    "lastModifiedAt": "2026-01-24T00:00:00Z"
  }
]
```

### 2.3 复杂的 Cron 表达式

**功能说明**：使用复杂的 cron 表达式设置任务。

**命令示例**：
```bash
scheduler_core add --name sync --type cron --expression "0 0 1 * *" --handler "SyncService.SyncData" --group integration --priority 3
```

**执行结果**：
```
任务添加成功：
名称：sync
组：integration
类型：cron
表达式：0 0 1 * *
处理器：SyncService.SyncData
优先级：3
状态：已启用
```

### 2.4 生成主机应用代码

**功能说明**：生成一个主机应用代码。

**命令示例**：
```bash
scheduler_generator generate --type host --output "..\generated\SchedulerHost.cs" --namespace "Scheduler.Generated" --class "SchedulerHost"
```

**执行结果**：
```
成功生成代码到: ..\generated\SchedulerHost.cs
生成的代码行数: 45
```

**生成的代码**：
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace Scheduler.Generated
{
    public class SchedulerHost
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加托管服务
                    services.AddHostedService<SchedulerHostedService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }
}
```

### 2.5 生成工作器服务代码

**功能说明**：生成一个工作器服务代码。

**命令示例**：
```bash
scheduler_generator generate --type worker --output "..\generated\SchedulerWorker.cs" --namespace "Scheduler.Generated" --class "SchedulerWorker"
```

**执行结果**：
```
成功生成代码到: ..\generated\SchedulerWorker.cs
生成的代码行数: 75
```

**生成的代码**：
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace Scheduler.Generated
{
    public class SchedulerWorker
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加工作器服务
                    services.AddHostedService<SchedulerWorkerService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }

    public class SchedulerWorkerService : BackgroundService
    {
        private readonly ISchedulerService _schedulerService;
        private readonly ILogger<SchedulerWorkerService> _logger;

        public SchedulerWorkerService(ISchedulerService schedulerService, ILogger<SchedulerWorkerService> logger)
        {
            _schedulerService = schedulerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("启动调度工作器");
            
            // 启动调度器
            await _schedulerService.StartAsync(stoppingToken);
            
            // 等待停止信号
            await stoppingToken.WhenCanceled();
            
            _logger.LogInformation("停止调度工作器");
            await _schedulerService.StopAsync(stoppingToken);
        }
    }
}
```

### 2.6 生成 Windows 服务代码

**功能说明**：生成一个 Windows 服务代码。

**命令示例**：
```bash
scheduler_generator generate --type service --output "..\generated\SchedulerService.cs" --namespace "Scheduler.Generated" --class "SchedulerService"
```

**执行结果**：
```
成功生成代码到: ..\generated\SchedulerService.cs
生成的代码行数: 47
```

**生成的代码**：
```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace Scheduler.Generated
{
    public class SchedulerService
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加托管服务
                    services.AddHostedService<SchedulerHostedService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseWindowsService();
    }
}
```

### 2.7 列出可用的代码模板

**功能说明**：列出可用的代码模板。

**命令示例**：
```bash
scheduler_generator template list
```

**执行结果**：
```
可用的代码模板:
--------------------------------------------------------------------------------
模板: host
描述: 主机应用模板
类型: host
--------------------------------------------------------------------------------
模板: worker
描述: 工作器服务模板
类型: worker
--------------------------------------------------------------------------------
模板: service
描述: Windows 服务模板
类型: service
--------------------------------------------------------------------------------
```

## 3. 实际应用场景示例

### 3.1 系统维护任务

**场景描述**：设置系统维护相关的任务，包括备份、清理和检查。

**示例命令**：

```bash
# 添加备份任务（每天凌晨 1 点）
scheduler_core add --name backup --type cron --expression "0 1 * * *" --handler "MaintenanceService.Backup" --group maintenance --priority 5

# 添加清理任务（每小时执行一次）
scheduler_core add --name cleanup --type interval --expression "01:00:00" --handler "MaintenanceService.Cleanup" --group maintenance --priority 4

# 添加系统检查任务（每天早上 6 点）
scheduler_core add --name check --type cron --expression "0 6 * * *" --handler "MaintenanceService.CheckSystem" --group maintenance --priority 3
```

### 3.2 业务流程任务

**场景描述**：设置业务流程相关的任务，包括报表生成、数据同步和通知。

**示例命令**：

```bash
# 添加日报生成任务（每天早上 8 点）
scheduler_core add --name daily-report --type cron --expression "0 8 * * *" --handler "ReportService.GenerateDaily" --group business --priority 2

# 添加周报生成任务（每周一早上 9 点）
scheduler_core add --name weekly-report --type cron --expression "0 9 * * 1" --handler "ReportService.GenerateWeekly" --group business --priority 2

# 添加数据同步任务（每天中午 12 点）
scheduler_core add --name sync-data --type cron --expression "0 12 * * *" --handler "SyncService.Sync" --group integration --priority 3
```

### 3.3 通知和提醒任务

**场景描述**：设置通知和提醒相关的任务。

**示例命令**：

```bash
# 添加生日提醒任务（每天早上 7 点）
scheduler_core add --name birthday-reminder --type cron --expression "0 7 * * *" --handler "NotificationService.BirthdayReminder" --group notification --priority 1

# 添加系统状态通知任务（每 30 分钟）
scheduler_core add --name status-notification --type interval --expression "00:30:00" --handler "NotificationService.SystemStatus" --group notification --priority 2
```

### 3.4 集成测试任务

**场景描述**：设置集成测试相关的任务。

**示例命令**：

```bash
# 添加 API 测试任务（每小时执行一次）
scheduler_core add --name api-test --type interval --expression "01:00:00" --handler "TestService.ApiTest" --group testing --priority 4

# 添加数据库测试任务（每天晚上 10 点）
scheduler_core add --name db-test --type cron --expression "0 22 * * *" --handler "TestService.DatabaseTest" --group testing --priority 4
```

## 4. 常见问题解决方案

### 4.1 任务不执行

**问题描述**：添加的任务没有执行。

**解决方案**：

1. 检查调度器是否已启动：
   ```bash
   scheduler_core status
   ```

2. 检查任务是否正确添加：
   ```bash
   scheduler_core list --format text
   ```

3. 检查任务表达式是否正确：
   - Cron 表达式：确保格式正确，例如 "0 1 * * *" 表示每天凌晨 1 点
   - 延迟/间隔表达式：确保格式正确，例如 "00:05:00" 表示 5 分钟

4. 检查任务处理器是否存在：
   - 确保处理器类和方法存在且可访问
   - 确保处理器方法是公共的，并且参数正确

### 4.2 任务执行失败

**问题描述**：任务执行失败，查看错误信息。

**解决方案**：

1. 查看执行历史：
   ```bash
   scheduler_core history --format text
   ```

2. 检查任务处理器代码：
   - 确保处理器代码没有异常
   - 添加适当的错误处理

3. 检查系统资源：
   - 确保系统有足够的内存和 CPU
   - 确保磁盘空间充足

### 4.3 调度器启动失败

**问题描述**：调度器无法启动。

**解决方案**：

1. 检查配置文件：
   - 确保 appsettings.json 文件存在且格式正确
   - 确保配置值正确

2. 检查依赖项：
   - 确保所有依赖项已正确安装
   - 确保 .NET 10 运行时已安装

3. 检查端口占用：
   - 确保调度器使用的端口未被占用

### 4.4 代码生成失败

**问题描述**：代码生成失败。

**解决方案**：

1. 检查输出路径：
   - 确保输出目录存在
   - 确保有写入权限

2. 检查参数：
   - 确保所有必需参数都已提供
   - 确保参数值正确

3. 检查模板：
   - 确保使用的模板存在
   - 确保模板格式正确

## 5. 性能优化示例

### 5.1 批量添加任务

**功能说明**：批量添加多个任务，减少命令执行次数。

**示例脚本**：

```powershell
# 批量添加维护任务
$scheduledTasks = @(
    @{name="backup"; type="cron"; expression="0 1 * * *"; handler="MaintenanceService.Backup"; group="maintenance"; priority=5},
    @{name="cleanup"; type="interval"; expression="00:30:00"; handler="MaintenanceService.Cleanup"; group="maintenance"; priority=4},
    @{name="check"; type="cron"; expression="0 6 * * *"; handler="MaintenanceService.CheckSystem"; group="maintenance"; priority=3}
)

foreach ($task in $scheduledTasks) {
    $command = "scheduler_core add --name $($task.name) --type $($task.type) --expression `"$($task.expression)`" --handler `"$($task.handler)`" --group $($task.group) --priority $($task.priority)"
    Write-Host "执行命令: $command"
    Invoke-Expression $command
}
```

### 5.2 优化任务执行顺序

**功能说明**：通过设置优先级，优化任务执行顺序。

**示例命令**：

```bash
# 高优先级任务（系统关键）
scheduler_core add --name critical-task --type cron --expression "0 0 * * *" --handler "CriticalService.Execute" --group system --priority 1

# 中优先级任务（业务重要）
scheduler_core add --name business-task --type cron --expression "0 12 * * *" --handler "BusinessService.Execute" --group business --priority 3

# 低优先级任务（系统维护）
scheduler_core add --name maintenance-task --type cron --expression "0 1 * * *" --handler "MaintenanceService.Execute" --group maintenance --priority 5
```

### 5.3 减少任务执行频率

**功能说明**：根据实际需求，合理设置任务执行频率，减少系统负载。

**示例命令**：

```bash
# 每天执行一次的任务
scheduler_core add --name daily-task --type cron --expression "0 0 * * *" --handler "TaskService.Execute" --group general --priority 3

# 每周执行一次的任务
scheduler_core add --name weekly-task --type cron --expression "0 0 * * 0" --handler "TaskService.ExecuteWeekly" --group general --priority 3

# 每月执行一次的任务
scheduler_core add --name monthly-task --type cron --expression "0 0 1 * *" --handler "TaskService.ExecuteMonthly" --group general --priority 3
```

## 6. 总结

本示例文档提供了 Scheduler 技能的各种使用场景和示例，包括基础使用、高级使用、实际应用场景、常见问题解决方案和性能优化示例。通过这些示例，您可以更好地理解和使用 Scheduler 技能，为您的应用程序添加可靠的任务调度功能。

Scheduler 技能支持三种类型的任务调度：Cron 表达式任务、延迟任务和间隔任务，满足不同场景的需求。同时，它还提供了代码生成功能，可以快速生成主机应用、工作器服务和 Windows 服务代码，简化开发过程。

通过合理配置和使用 Scheduler 技能，您可以实现自动化的任务调度，提高系统的可靠性和效率。