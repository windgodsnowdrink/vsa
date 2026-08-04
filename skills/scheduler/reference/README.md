# Scheduler 技能技术参考文档

## 1. 架构概述

Scheduler 技能是一个基于 .NET 10 的任务调度系统，采用 Ahead-of-Time (AOT) 编译技术，提供高性能、可靠的任务调度能力。本技能支持三种类型的任务调度：

- **Cron 表达式任务**：基于标准 cron 表达式的定时任务
- **延迟任务**：在指定延迟时间后执行的任务
- **间隔任务**：按照指定时间间隔重复执行的任务

## 2. 目录结构

```
scheduler/
├── index.yaml           # 技能配置文件
├── SKILL.md             # 技能文档
├── scripts/             # 脚本目录
│   ├── scheduler_core.cs                  # 调度器核心功能
│   ├── scheduler_core.setting.json        # 核心功能配置
│   ├── scheduler_core.run.json            # 核心功能运行配置
│   ├── scheduler_generator.cs             # 代码生成功能
│   ├── scheduler_generator.setting.json   # 代码生成配置
│   └── scheduler_generator.run.json       # 代码生成运行配置
└── reference/           # 参考文档目录
    ├── README.md        # 技术参考文档
    └── examples.md      # 使用示例文档
```

## 3. 核心组件

### 3.1 调度器服务 (ISchedulerService)

调度器服务是整个系统的核心，负责管理任务的生命周期和执行。

**接口定义**：

```csharp
public interface ISchedulerService {
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    SchedulerStatus GetStatus();
    Task<ScheduleTask> AddTaskAsync(ScheduleTask task, CancellationToken cancellationToken = default);
    Task<bool> RemoveTaskAsync(string name, string group, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduleTask>> GetTasksAsync(CancellationToken cancellationToken = default);
    Task<ScheduleTask?> GetTaskAsync(string name, string group, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionRecord>> GetExecutionRecordsAsync(DateTime? startTime = null, DateTime? endTime = null, CancellationToken cancellationToken = default);
}
```

**主要方法**：
- `StartAsync`：启动调度器
- `StopAsync`：停止调度器
- `GetStatus`：获取调度器状态
- `AddTaskAsync`：添加任务
- `RemoveTaskAsync`：移除任务
- `GetTasksAsync`：获取所有任务
- `GetTaskAsync`：获取单个任务
- `GetExecutionRecordsAsync`：获取执行记录

### 3.2 任务执行器 (ITaskExecutor)

任务执行器负责实际执行任务，处理任务的调用和异常。

**接口定义**：

```csharp
public interface ITaskExecutor {
    Task<ExecutionResult> ExecuteAsync(ScheduleTask task, CancellationToken cancellationToken = default);
}
```

### 3.3 任务存储 (IJobStore)

任务存储负责持久化任务和执行记录。

**接口定义**：

```csharp
public interface IJobStore {
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task SaveTaskAsync(ScheduleTask task, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(string name, string group, CancellationToken cancellationToken = default);
    Task<IEnumerable<ScheduleTask>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<ScheduleTask?> GetTaskAsync(string name, string group, CancellationToken cancellationToken = default);
    Task SaveExecutionRecordAsync(ExecutionRecord record, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExecutionRecord>> GetExecutionRecordsAsync(DateTime? startTime = null, DateTime? endTime = null, CancellationToken cancellationToken = default);
}
```

### 3.4 调度策略

#### 3.4.1 Cron 解析器 (ICronParser)

负责解析和计算 cron 表达式的下一次执行时间。

**接口定义**：

```csharp
public interface ICronParser {
    bool IsValid(string cronExpression);
    DateTime? GetNextExecutionTime(string cronExpression, DateTime? baseTime = null);
}
```

#### 3.4.2 延迟调度器 (IDelayScheduler)

负责处理延迟任务的调度。

**接口定义**：

```csharp
public interface IDelayScheduler {
    Task ScheduleAsync(ScheduleTask task, CancellationToken cancellationToken = default);
    Task CancelAsync(string name, string group, CancellationToken cancellationToken = default);
}
```

#### 3.4.3 间隔调度器 (IIntervalScheduler)

负责处理间隔任务的调度。

**接口定义**：

```csharp
public interface IIntervalScheduler {
    Task ScheduleAsync(ScheduleTask task, CancellationToken cancellationToken = default);
    Task CancelAsync(string name, string group, CancellationToken cancellationToken = default);
}
```

## 4. 数据结构

### 4.1 任务信息 (ScheduleTask)

```csharp
public class ScheduleTask {
    public string Name { get; set; }
    public string Group { get; set; }
    public TaskType Type { get; set; }
    public string Expression { get; set; }
    public string Handler { get; set; }
    public int Priority { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public bool Enabled { get; set; }
}
```

### 4.2 任务类型 (TaskType)

```csharp
public enum TaskType {
    Cron = 0,
    Delay = 1,
    Interval = 2
}
```

### 4.3 调度器状态 (SchedulerStatus)

```csharp
public enum SchedulerStatus {
    Stopped = 0,
    Starting = 1,
    Running = 2,
    Stopping = 3,
    Paused = 4
}
```

### 4.4 执行结果 (ExecutionResult)

```csharp
public class ExecutionResult {
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Exception? Exception { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
}
```

### 4.5 执行记录 (ExecutionRecord)

```csharp
public class ExecutionRecord {
    public string Id { get; set; }
    public string TaskName { get; set; }
    public string TaskGroup { get; set; }
    public DateTime ExecutionTime { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
    public TimeSpan Duration { get; set; }
}
```

## 5. 命令行接口

### 5.1 核心命令

#### 5.1.1 add 命令

添加新任务。

**语法**：
```
scheduler_core add --name <name> --group <group> --type <type> --expression <expression> --handler <handler> --priority <priority> --parameters <parameters>
```

**参数**：
- `--name`：任务名称
- `--group`：任务组
- `--type`：任务类型 (cron/delay/interval)
- `--expression`：表达式 (cron 表达式或时间间隔)
- `--handler`：任务处理器
- `--priority`：优先级 (可选)
- `--parameters`：参数 (可选，JSON 格式)

#### 5.1.2 remove 命令

移除任务。

**语法**：
```
scheduler_core remove --name <name> --group <group>
```

**参数**：
- `--name`：任务名称
- `--group`：任务组

#### 5.1.3 list 命令

列出所有任务。

**语法**：
```
scheduler_core list --format <format>
```

**参数**：
- `--format`：输出格式 (text/json)

#### 5.1.4 start 命令

启动调度器。

**语法**：
```
scheduler_core start
```

#### 5.1.5 stop 命令

停止调度器。

**语法**：
```
scheduler_core stop
```

#### 5.1.6 status 命令

获取调度器状态。

**语法**：
```
scheduler_core status
```

#### 5.1.7 history 命令

查看执行历史。

**语法**：
```
scheduler_core history --start <start> --end <end> --format <format>
```

**参数**：
- `--start`：开始时间
- `--end`：结束时间
- `--format`：输出格式 (text/json)

### 5.2 代码生成命令

#### 5.2.1 generate 命令

生成任务调度代码。

**语法**：
```
scheduler_generator generate --type <type> --output <output> --format <format> --namespace <namespace> --class <class>
```

**参数**：
- `--type`：生成类型 (host/worker/service)
- `--output`：输出文件路径
- `--format`：输出格式 (csharp)
- `--namespace`：命名空间
- `--class`：类名

#### 5.2.2 template list 命令

列出可用的代码模板。

**语法**：
```
scheduler_generator template list
```

## 6. 代码生成器

代码生成器支持生成三种类型的代码：

### 6.1 主机应用 (Host)

生成一个完整的主机应用，包含调度器服务的配置和启动逻辑。

**生成命令**：
```
scheduler_generator generate --type host --output "SchedulerHost.cs" --namespace "Scheduler.Generated" --class "SchedulerHost"
```

### 6.2 工作器服务 (Worker)

生成一个工作器服务，适合在后台运行任务调度。

**生成命令**：
```
scheduler_generator generate --type worker --output "SchedulerWorker.cs" --namespace "Scheduler.Generated" --class "SchedulerWorker"
```

### 6.3 Windows 服务 (Service)

生成一个 Windows 服务，适合在 Windows 系统上作为服务运行。

**生成命令**：
```
scheduler_generator generate --type service --output "SchedulerService.cs" --namespace "Scheduler.Generated" --class "SchedulerService"
```

## 7. 配置选项

### 7.1 编译配置 (.setting.json)

**核心功能配置** (scheduler_core.setting.json)：

```json
{
  "version": "1.0",
  "compileOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": "enable",
    "implicitUsings": "enable",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "8.0.0",
    "Microsoft.Extensions.Configuration": "8.0.0",
    "Microsoft.Extensions.Configuration.Json": "8.0.0",
    "Microsoft.Extensions.Hosting": "8.0.0",
    "Microsoft.Extensions.Logging": "8.0.0",
    "Microsoft.Extensions.Logging.Console": "8.0.0",
    "System.CommandLine": "2.0.0",
    "System.Text.Json": "8.0.0",
    "Quartz": "3.8.0",
    "Cronos": "0.12.0"
  }
}
```

**代码生成配置** (scheduler_generator.setting.json)：

```json
{
  "version": "1.0",
  "compileOptions": {
    "targetFramework": "net10.0",
    "langVersion": "preview",
    "nullable": "enable",
    "implicitUsings": "enable",
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "8.0.0",
    "Microsoft.Extensions.Configuration": "8.0.0",
    "Microsoft.Extensions.Configuration.Json": "8.0.0",
    "Microsoft.Extensions.Hosting": "8.0.0",
    "System.CommandLine": "2.0.0",
    "System.Text.Json": "8.0.0"
  }
}
```

### 7.2 运行配置 (.run.json)

**核心功能运行配置** (scheduler_core.run.json)：

```json
{
  "version": "1.0",
  "profiles": {
    "Add Cron Task": {
      "commandName": "Project",
      "commandLineArgs": "add --name backup --type cron --expression \"0 1 * * *\" --handler \"BackupService.Execute\" --group maintenance --priority 5",
      "workingDirectory": "scripts",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
    // 其他配置...
  }
}
```

**代码生成运行配置** (scheduler_generator.run.json)：

```json
{
  "version": "1.0",
  "profiles": {
    "Generate Host": {
      "commandName": "Project",
      "commandLineArgs": "generate --type host --output \"..\\generated\\SchedulerHost.cs\" --namespace \"Scheduler.Generated\" --class \"SchedulerHost\"",
      "workingDirectory": "scripts",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    }
    // 其他配置...
  }
}
```

## 8. AOT 编译配置

为了支持 AOT 编译，系统使用了以下配置：

```json
{
  "publishAot": true,
  "trimMode": "partial",
  "selfContained": true,
  "publishSingleFile": true
}
```

**关键配置说明**：
- `publishAot`：启用 AOT 编译
- `trimMode`：设置为 partial，保留必要的反射信息
- `selfContained`：生成自包含的可执行文件
- `publishSingleFile`：生成单个可执行文件

## 9. 依赖项

### 核心依赖项

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Microsoft.Extensions.DependencyInjection | 8.0.0 | 依赖注入 |
| Microsoft.Extensions.Configuration | 8.0.0 | 配置管理 |
| Microsoft.Extensions.Configuration.Json | 8.0.0 | JSON 配置支持 |
| Microsoft.Extensions.Hosting | 8.0.0 | 主机管理 |
| Microsoft.Extensions.Logging | 8.0.0 | 日志记录 |
| Microsoft.Extensions.Logging.Console | 8.0.0 | 控制台日志 |
| System.CommandLine | 2.0.0 | 命令行接口 |
| System.Text.Json | 8.0.0 | JSON 序列化 |
| Quartz | 3.8.0 | 任务调度 |
| Cronos | 0.12.0 | Cron 表达式解析 |

## 10. 性能优化

### 10.1 AOT 编译

通过启用 AOT 编译，系统获得以下性能优势：
- 启动时间显著减少
- 内存使用降低
- 运行时性能提升

### 10.2 任务执行优化

- **并行执行**：使用多线程并行执行任务
- **任务队列**：使用队列管理任务执行顺序
- **错误处理**：完善的错误处理机制，确保单个任务失败不影响其他任务

### 10.3 存储优化

- **文件存储**：使用高效的文件存储格式
- **缓存机制**：缓存任务信息，减少磁盘 I/O
- **批量操作**：支持批量读写操作，提高性能

## 11. 安全性

### 11.1 任务执行安全

- **沙箱执行**：任务在受控环境中执行
- **权限控制**：限制任务的执行权限
- **异常隔离**：任务执行异常不会影响调度器本身

### 11.2 配置安全

- **环境变量**：敏感配置通过环境变量传递
- **配置加密**：支持配置文件加密

## 12. 故障恢复

### 12.1 任务重试机制

- **自动重试**：任务失败时支持自动重试
- **重试策略**：可配置的重试策略

### 12.2 调度器恢复

- **状态持久化**：调度器状态持久化到磁盘
- **重启恢复**：重启后自动恢复之前的状态

## 13. 监控与日志

### 13.1 日志记录

- **详细日志**：记录任务执行的详细信息
- **错误日志**：记录错误和异常信息
- **性能日志**：记录任务执行时间和资源使用情况

### 13.2 监控指标

- **任务执行统计**：任务执行次数、成功率、平均执行时间
- **调度器状态**：调度器运行状态、任务队列长度
- **系统资源**：CPU、内存使用情况

## 14. 扩展性

### 14.1 自定义任务类型

通过实现 `ITaskExecutor` 接口，可以支持自定义任务类型。

### 14.2 自定义存储

通过实现 `IJobStore` 接口，可以支持自定义存储方式，如数据库存储。

### 14.3 自定义调度策略

通过实现相应的调度器接口，可以支持自定义调度策略。

## 15. 最佳实践

### 15.1 任务设计

- **任务粒度**：任务应该设计得小而专注
- **执行时间**：避免长时间运行的任务
- **错误处理**：任务应该包含完善的错误处理

### 15.2 调度策略

- **Cron 表达式**：使用标准 cron 表达式，避免过于复杂的表达式
- **延迟任务**：合理设置延迟时间，避免任务堆积
- **间隔任务**：根据任务性质设置合适的间隔时间

### 15.3 性能优化

- **批量处理**：对于多个类似任务，考虑批量处理
- **资源管理**：合理管理任务的资源使用
- **并行度**：根据系统资源设置合适的并行度

## 16. 常见问题

### 16.1 任务不执行

**可能原因**：
- 调度器未启动
- 任务表达式错误
- 任务处理器不存在
- 权限不足

**解决方案**：
- 检查调度器状态
- 验证任务表达式
- 确保任务处理器存在且可访问
- 检查权限设置

### 16.2 任务执行失败

**可能原因**：
- 任务处理器抛出异常
- 资源不足
- 网络问题

**解决方案**：
- 检查任务处理器代码
- 增加系统资源
- 检查网络连接

### 16.3 调度器启动失败

**可能原因**：
- 配置错误
- 端口被占用
- 权限不足

**解决方案**：
- 检查配置文件
- 确保端口可用
- 以管理员权限运行

## 17. 版本控制

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本 |

## 18. 总结

Scheduler 技能是一个功能强大、性能优异的任务调度系统，支持多种任务类型和代码生成能力。通过 AOT 编译技术，系统获得了更好的启动性能和运行时性能，适合在各种场景下使用。

系统的模块化设计和丰富的扩展点，使得它可以轻松适应不同的业务需求，为开发者提供了一个可靠的任务调度解决方案。