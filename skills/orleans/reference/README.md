# Orleans 技能参考文档

## 1. 技能概述

Orleans 技能是一个基于 .NET 10 的高性能技能，提供与 Orleans 框架的无缝集成和扩展功能。Orleans 是一个现代化的 .NET 分布式计算框架，基于 Actor 模型，专为构建可扩展、高可用的分布式系统而设计。

本技能旨在简化 Orleans 应用的开发、部署和管理流程，提供一系列工具和功能，帮助开发者更高效地使用 Orleans 框架。

## 2. 核心组件

### 2.1 IOrleansService

`IOrleansService` 是 Orleans 技能的核心服务接口，提供了与 Orleans 框架交互的主要方法。

**主要方法**：

- `InitializeClusterAsync(CancellationToken cancellationToken = default)` - 初始化 Orleans 集群
- `GetClusterStatusAsync(CancellationToken cancellationToken = default)` - 获取集群状态
- `AddNodeAsync(string nodeId, string endpoint, CancellationToken cancellationToken = default)` - 添加集群节点
- `RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default)` - 移除集群节点
- `RestartNodeAsync(string nodeId, CancellationToken cancellationToken = default)` - 重启集群节点

### 2.2 IOrleansEventSourcingService

`IOrleansEventSourcingService` 负责 Orleans 事件溯源的管理，包括事件的保存、获取和回放。

### 2.3 IOrleansStateMachineService

`IOrleansStateMachineService` 负责 Orleans 状态机的管理，包括状态机的创建、状态转换和状态查询。

### 2.4 IOrleansClusterService

`IOrleansClusterService` 负责 Orleans 集群的管理，包括节点的添加、移除、监控和集群状态的获取。

### 2.5 IOrleansConfigurationService

`IOrleansConfigurationService` 负责 Orleans 应用配置的管理，包括获取和更新配置。

### 2.6 IOrleansDeploymentService

`IOrleansDeploymentService` 负责 Orleans 应用的部署，包括打包和部署应用。

## 3. 配置选项

### 3.1 OrleansOptions

`OrleansOptions` 是 Orleans 技能的主要配置选项类，包含以下属性：

| 属性名 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| Enabled | bool | true | 是否启用 Orleans 技能 |
| ClusterId | string | "orleans-cluster" | 集群 ID |
| ServiceId | string | "orleans-service" | 服务 ID |
| SiloPort | int | 11111 | Silo 端口 |
| GatewayPort | int | 30000 | 网关端口 |
| EnableEventSourcing | bool | true | 是否启用事件溯源 |
| EnableStateMachine | bool | true | 是否启用状态机 |
| EnableClusterManagement | bool | true | 是否启用集群管理 |
| EnableConfiguration | bool | true | 是否启用配置管理 |
| EnableDeployment | bool | true | 是否启用部署工具 |
| EnableParallelProcessing | bool | true | 是否启用并行处理 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |

### 3.2 配置文件结构

Orleans 技能的配置文件包含以下主要部分：

- `orleans.options` - 基本配置选项
- `orleans.eventsourcing` - 事件溯源配置
- `orleans.statemachine` - 状态机配置
- `orleans.cluster` - 集群管理配置
- `orleans.configuration` - 配置管理配置
- `orleans.deployment` - 部署工具配置
- `orleans.aot` - AOT 编译配置
- `orleans.logging` - 日志配置
- `orleans.performance` - 性能配置
- `orleans.security` - 安全配置

## 4. 安装和设置

### 4.1 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- Orleans 8.0 或更高版本

### 4.2 安装方法

1. **克隆技能仓库**：
   ```bash
   git clone <repository-url>
   cd skills/orleans
   ```

2. **安装依赖**：
   ```bash
   dotnet restore
   ```

3. **配置技能**：
   编辑 `scripts/orleans_eventsourcing_integration.setting.json` 或 `scripts/orleans_statemachine_integration.setting.json` 文件，根据需要修改配置。

4. **运行技能**：
   ```bash
   dotnet run --project scripts/orleans_eventsourcing_integration.cs
   ```

### 4.3 依赖注入

在 .NET 应用中，你可以使用依赖注入来注册和使用 Orleans 服务：

```csharp
// 注册 Orleans 服务
services.AddOrleansServices(options =>
{
    options.Enabled = true;
    options.ClusterId = "my-cluster";
    options.ServiceId = "my-service";
    options.SiloPort = 11111;
    options.GatewayPort = 30000;
    options.EnableEventSourcing = true;
    options.EnableStateMachine = true;
    options.EnableClusterManagement = true;
    options.EnableConfiguration = true;
    options.EnableDeployment = true;
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = Environment.ProcessorCount;
});

// 使用 Orleans 服务
var orleansService = serviceProvider.GetRequiredService<IOrleansService>();
```

## 5. 使用指南

### 5.1 事件溯源

**保存事件**：
```csharp
var eventSourcingService = serviceProvider.GetRequiredService<IOrleansEventSourcingService>();
var @event = new UserCreatedEvent { UserId = 1, Username = "user1", Email = "user1@example.com" };
await eventSourcingService.SaveEventAsync("UserGrain-1", @event);
Console.WriteLine("事件保存成功");
```

**获取事件历史**：
```csharp
var events = await eventSourcingService.GetEventsAsync("UserGrain-1");
Console.WriteLine($"事件数量: {events.Count()}");
foreach (var e in events)
{
    Console.WriteLine($"事件类型: {e.GetType().Name}, 时间: {e.Timestamp}");
}
```

**回放事件**：
```csharp
var state = await eventSourcingService.ReplayEventsAsync<UserState>("UserGrain-1");
Console.WriteLine($"回放后状态: Username={state.Username}, Email={state.Email}");
```

### 5.2 状态机

**创建状态机**：
```csharp
var stateMachineService = serviceProvider.GetRequiredService<IOrleansStateMachineService>();
await stateMachineService.CreateStateMachineAsync("Order-123", "Pending");
Console.WriteLine("状态机创建成功");
```

**触发状态转换**：
```csharp
await stateMachineService.TriggerEventAsync("Order-123", "PaymentReceived");
var currentState = await stateMachineService.GetCurrentStateAsync("Order-123");
Console.WriteLine($"当前状态: {currentState}");
```

**获取状态历史**：
```csharp
var stateHistory = await stateMachineService.GetStateHistoryAsync("Order-123");
Console.WriteLine($"状态转换历史: {string.Join(" -> ", stateHistory)}");
```

### 5.3 集群管理

**获取集群状态**：
```csharp
var clusterService = serviceProvider.GetRequiredService<IOrleansClusterService>();
var clusterStatus = await clusterService.GetClusterStatusAsync();
Console.WriteLine($"集群节点数量: {clusterStatus.Nodes.Count}");
foreach (var node in clusterStatus.Nodes)
{
    Console.WriteLine($"节点 ID: {node.NodeId}, 状态: {node.Status}, 地址: {node.Endpoint}");
}
```

**添加节点**：
```csharp
await clusterService.AddNodeAsync("new-node-1", "192.168.1.100:30000");
Console.WriteLine("节点添加成功");
```

**移除节点**：
```csharp
await clusterService.RemoveNodeAsync("node-to-remove");
Console.WriteLine("节点移除成功");
```

### 5.4 配置管理

**获取配置**：
```csharp
var configService = serviceProvider.GetRequiredService<IOrleansConfigurationService>();
var config = await configService.GetConfigurationAsync();
Console.WriteLine($"集群 ID: {config.ClusterId}");
Console.WriteLine($"服务 ID: {config.ServiceId}");
Console.WriteLine($"Silo 端口: {config.SiloPort}");
Console.WriteLine($"网关端口: {config.GatewayPort}");
```

**更新配置**：
```csharp
var newConfig = new OrleansConfiguration
{
    ClusterId = "updated-cluster",
    ServiceId = "updated-service",
    SiloPort = 11111,
    GatewayPort = 30000,
    EnableEventSourcing = true,
    EnableStateMachine = true
};
var result = await configService.UpdateConfigurationAsync(newConfig);
Console.WriteLine($"配置更新: {(result ? "成功" : "失败")}");
```

### 5.5 部署工具

**打包应用**：
```csharp
var deploymentService = serviceProvider.GetRequiredService<IOrleansDeploymentService>();
var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "output");
var result = await deploymentService.PackageApplicationAsync(outputPath);
Console.WriteLine($"应用打包: {(result ? "成功" : "失败")}");
```

**部署应用**：
```csharp
var result = await deploymentService.DeployApplicationAsync("https://myserver.com", "username", "password");
Console.WriteLine($"应用部署: {(result ? "成功" : "失败")}");
```

## 6. AOT 编译

Orleans 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 6.1 编译选项

在配置文件中，你可以配置以下 AOT 编译选项：

- `publishAot` - 是否启用 AOT 编译（默认: true）
- `readyToRun` - 是否启用 ReadyToRun 编译（默认: true）
- `tieredCompilation` - 是否启用分层编译（默认: true）
- `optimizeForSize` - 是否优化大小（默认: false）
- `trimMode` - 剪裁模式（默认: "partial"）

### 6.2 支持的运行时

Orleans 技能支持以下运行时：

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

### 6.3 编译命令

要使用 AOT 编译 Orleans 技能，你可以使用以下命令：

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true
```

## 7. 性能优化

### 7.1 并行处理

Orleans 技能支持并行处理，可以提高处理多个任务时的性能：

```csharp
// 启用并行处理
options.EnableParallelProcessing = true;
// 设置最大并行度
options.MaxDegreeOfParallelism = Environment.ProcessorCount;
```

### 7.2 缓存

Orleans 技能使用缓存来提高性能，你可以在配置文件中设置缓存持续时间：

```json
"performance": {
  "enableCaching": true,
  "cacheDuration": "00:05:00",
  "cacheSize": 1000
}
```

### 7.3 批处理

对于大量事件的处理，你可以使用批处理来提高性能：

```csharp
var events = new List<object>
{
    new UserCreatedEvent { UserId = 1, Username = "user1" },
    new UserUpdatedEvent { UserId = 1, Email = "user1@example.com" },
    new UserActivatedEvent { UserId = 1 }
};
await eventSourcingService.SaveEventsAsync("UserGrain-1", events);
```

## 8. 安全配置

### 8.1 集群安全

你可以在配置文件中启用集群安全：

```json
"security": {
  "enableClusterSecurity": true,
  "clusterSecret": "your-cluster-secret",
  "enableTls": true,
  "certificatePath": "cert.pfx",
  "certificatePassword": "password"
}
```

### 8.2 访问控制

配置访问控制策略：

```json
"security": {
  "enableAccessControl": true,
  "allowedRoles": ["Admin", "Developer"],
  "enableAuditLogging": true
}
```

## 9. 故障排除

### 9.1 常见问题

**问题**：集群节点无法加入
**解决方案**：检查网络连接，确保节点间可以通信；验证集群 ID 和服务 ID 是否一致；查看日志文件获取详细错误信息。

**问题**：事件保存失败
**解决方案**：检查存储配置，确保存储服务可用；验证事件序列化是否正确；查看日志文件获取详细错误信息。

**问题**：状态机转换失败
**解决方案**：检查状态机配置，确保状态和事件定义正确；验证当前状态是否允许指定的事件；查看日志文件获取详细错误信息。

**问题**：性能下降
**解决方案**：启用缓存，调整最大并行度，使用批处理，检查资源使用情况（CPU、内存、网络）。

### 9.2 日志文件

Orleans 技能的日志文件默认位于应用根目录的 `orleans.log` 文件中，你可以在配置文件中修改日志文件路径：

```json
"logging": {
  "logFile": "orleans.log",
  "logLevel": "Information",
  "maxFileSize": 10485760,
  "maxRetainedFiles": 5
}
```

## 10. 示例代码

### 10.1 基本使用

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Orleans 基本使用示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Orleans 服务
        services.AddOrleansServices(options =>
        {
            options.Enabled = true;
            options.ClusterId = "example-cluster";
            options.ServiceId = "example-service";
            options.EnableEventSourcing = true;
            options.EnableStateMachine = true;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var orleansService = serviceProvider.GetRequiredService<IOrleansService>();
        var eventSourcingService = serviceProvider.GetRequiredService<IOrleansEventSourcingService>();
        var stateMachineService = serviceProvider.GetRequiredService<IOrleansStateMachineService>();

        try
        {
            // 初始化集群
            Console.WriteLine("初始化 Orleans 集群...");
            await orleansService.InitializeClusterAsync();
            Console.WriteLine("集群初始化成功");

            // 使用事件溯源
            Console.WriteLine("\n使用事件溯源...");
            await eventSourcingService.SaveEventAsync("User-1", new UserCreatedEvent { UserId = 1, Username = "user1" });
            var events = await eventSourcingService.GetEventsAsync("User-1");
            Console.WriteLine($"事件数量: {events.Count()}");

            // 使用状态机
            Console.WriteLine("\n使用状态机...");
            await stateMachineService.CreateStateMachineAsync("Order-1", "Pending");
            await stateMachineService.TriggerEventAsync("Order-1", "PaymentReceived");
            var currentState = await stateMachineService.GetCurrentStateAsync("Order-1");
            Console.WriteLine($"当前状态: {currentState}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 10.2 高级配置

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Orleans 高级配置示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Orleans 服务并配置所有选项
        services.AddOrleansServices(options =>
        {
            options.Enabled = true;
            options.ClusterId = "advanced-cluster";
            options.ServiceId = "advanced-service";
            options.SiloPort = 11111;
            options.GatewayPort = 30000;
            options.EnableEventSourcing = true;
            options.EnableStateMachine = true;
            options.EnableClusterManagement = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取配置
        var options = serviceProvider.GetRequiredService<IOptions<OrleansOptions>>().Value;
        Console.WriteLine($"集群 ID: {options.ClusterId}");
        Console.WriteLine($"服务 ID: {options.ServiceId}");
        Console.WriteLine($"Silo 端口: {options.SiloPort}");
        Console.WriteLine($"网关端口: {options.GatewayPort}");

        // 获取服务
        var clusterService = serviceProvider.GetRequiredService<IOrleansClusterService>();
        var configService = serviceProvider.GetRequiredService<IOrleansConfigurationService>();

        try
        {
            // 获取集群状态
            var clusterStatus = await clusterService.GetClusterStatusAsync();
            Console.WriteLine($"\n集群状态: {clusterStatus.Status}");
            Console.WriteLine($"节点数量: {clusterStatus.Nodes.Count}");

            // 获取配置
            var config = await configService.GetConfigurationAsync();
            Console.WriteLine($"\n当前配置: 集群 ID={config.ClusterId}, 服务 ID={config.ServiceId}");

            // 更新配置
            config.ClusterId = "updated-cluster";
            var updateResult = await configService.UpdateConfigurationAsync(config);
            Console.WriteLine($"配置更新: {(updateResult ? "成功" : "失败")}");

            // 验证更新
            var updatedConfig = await configService.GetConfigurationAsync();
            Console.WriteLine($"更新后配置: 集群 ID={updatedConfig.ClusterId}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 11. 结论

Orleans 技能是一个功能强大的工具，可以帮助开发者更高效地使用 Orleans 框架。通过本参考文档，你应该已经了解了 Orleans 技能的核心组件、配置选项和使用方法。

如果你有任何问题或建议，请参考 Orleans 官方文档或联系 VSA Architecture Team。

## 12. 参考资料

- [Orleans 官方文档](https://docs.microsoft.com/en-us/dotnet/orleans/)
- [Orleans GitHub 仓库](https://github.com/dotnet/orleans)
- [.NET 10 文档](https://learn.microsoft.com/dotnet/)
- [AOT 编译文档](https://learn.microsoft.com/dotnet/core/deploying/aot)
