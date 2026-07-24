# Orleans 技能使用示例

## 概述

本文档提供了 Orleans 技能的各种使用示例，包括基本用法、高级配置、性能优化、错误处理等。这些示例旨在帮助您快速上手 Orleans 功能，并了解其在不同场景下的应用。

## 示例 1: 基本使用

### 功能说明

演示 Orleans 技能的基本使用方法，包括服务注册、集群初始化、事件溯源和状态机功能。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Client@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 事件类
public abstract class Event
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class UserCreatedEvent : Event
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}

public class UserUpdatedEvent : Event
{
    public int UserId { get; set; }
    public string Email { get; set; }
}

// 状态类
public class UserState
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

// 主程序
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
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 初始化集群
            Console.WriteLine("示例 1: 初始化 Orleans 集群");
            await orleansService.InitializeClusterAsync();
            Console.WriteLine("集群初始化成功");

            // 示例 2: 使用事件溯源
            Console.WriteLine("\n示例 2: 使用事件溯源");
            var userCreatedEvent = new UserCreatedEvent { UserId = 1, Username = "user1", Email = "user1@example.com" };
            await eventSourcingService.SaveEventAsync("UserGrain-1", userCreatedEvent);
            Console.WriteLine("事件保存成功");

            var userUpdatedEvent = new UserUpdatedEvent { UserId = 1, Email = "user1_updated@example.com" };
            await eventSourcingService.SaveEventAsync("UserGrain-1", userUpdatedEvent);
            Console.WriteLine("事件更新成功");

            var events = await eventSourcingService.GetEventsAsync("UserGrain-1");
            Console.WriteLine($"事件数量: {events.Count()}");
            foreach (var e in events)
            {
                Console.WriteLine($"事件类型: {e.GetType().Name}, 时间: {e.Timestamp}");
            }

            // 示例 3: 使用状态机
            Console.WriteLine("\n示例 3: 使用状态机");
            await stateMachineService.CreateStateMachineAsync("Order-123", "Pending");
            Console.WriteLine("状态机创建成功，初始状态: Pending");

            await stateMachineService.TriggerEventAsync("Order-123", "PaymentReceived");
            var currentState = await stateMachineService.GetCurrentStateAsync("Order-123");
            Console.WriteLine($"触发 PaymentReceived 事件后，当前状态: {currentState}");

            await stateMachineService.TriggerEventAsync("Order-123", "Shipped");
            currentState = await stateMachineService.GetCurrentStateAsync("Order-123");
            Console.WriteLine($"触发 Shipped 事件后，当前状态: {currentState}");

            var stateHistory = await stateMachineService.GetStateHistoryAsync("Order-123");
            Console.WriteLine($"状态转换历史: {string.Join(" -> ", stateHistory)}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 2: 高级配置

### 功能说明

演示 Orleans 技能的高级配置选项，包括集群配置、事件溯源配置、状态机配置和性能优化选项。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Client@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

// 主程序
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
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"  集群 ID: {options.ClusterId}");
        Console.WriteLine($"  服务 ID: {options.ServiceId}");
        Console.WriteLine($"  Silo 端口: {options.SiloPort}");
        Console.WriteLine($"  网关端口: {options.GatewayPort}");
        Console.WriteLine($"  启用事件溯源: {options.EnableEventSourcing}");
        Console.WriteLine($"  启用状态机: {options.EnableStateMachine}");
        Console.WriteLine($"  启用集群管理: {options.EnableClusterManagement}");
        Console.WriteLine($"  启用并行处理: {options.EnableParallelProcessing}");
        Console.WriteLine($"  最大并行度: {options.MaxDegreeOfParallelism}");

        // 获取服务
        var clusterService = serviceProvider.GetRequiredService<IOrleansClusterService>();
        var configService = serviceProvider.GetRequiredService<IOrleansConfigurationService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 获取集群状态
            Console.WriteLine("\n示例 1: 获取集群状态");
            var clusterStatus = await clusterService.GetClusterStatusAsync();
            Console.WriteLine($"集群状态: {clusterStatus.Status}");
            Console.WriteLine($"节点数量: {clusterStatus.Nodes.Count}");
            foreach (var node in clusterStatus.Nodes)
            {
                Console.WriteLine($"  节点 ID: {node.NodeId}, 状态: {node.Status}, 地址: {node.Endpoint}");
            }

            // 示例 2: 配置管理
            Console.WriteLine("\n示例 2: 配置管理");
            var config = await configService.GetConfigurationAsync();
            Console.WriteLine($"当前配置: 集群 ID={config.ClusterId}, 服务 ID={config.ServiceId}");

            // 更新配置
            config.ClusterId = "updated-cluster";
            config.ServiceId = "updated-service";
            var updateResult = await configService.UpdateConfigurationAsync(config);
            Console.WriteLine($"配置更新: {(updateResult ? "成功" : "失败")}");

            // 验证更新
            var updatedConfig = await configService.GetConfigurationAsync();
            Console.WriteLine($"更新后配置: 集群 ID={updatedConfig.ClusterId}, 服务 ID={updatedConfig.ServiceId}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 3: 性能优化

### 功能说明

演示 Orleans 技能的性能优化选项，包括并行处理、缓存和批处理功能。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Client@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// 事件类
public abstract class Event
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class UserCreatedEvent : Event
{
    public int UserId { get; set; }
    public string Username { get; set; }
}

public class UserUpdatedEvent : Event
{
    public int UserId { get; set; }
    public string Email { get; set; }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Orleans 性能优化示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Orleans 服务并配置性能选项
        services.AddOrleansServices(options =>
        {
            options.Enabled = true;
            options.ClusterId = "performance-cluster";
            options.ServiceId = "performance-service";
            options.EnableEventSourcing = true;
            options.EnableStateMachine = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var eventSourcingService = serviceProvider.GetRequiredService<IOrleansEventSourcingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 批处理事件
            Console.WriteLine("示例 1: 批处理事件");
            var events = new List<object>();
            for (int i = 1; i <= 100; i++)
            {
                events.Add(new UserCreatedEvent { UserId = i, Username = $"user{i}" });
            }
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await eventSourcingService.SaveEventsAsync("BatchUserGrain", events);
            stopwatch.Stop();
            Console.WriteLine($"批处理 100 个事件耗时: {stopwatch.ElapsedMilliseconds}ms");

            // 示例 2: 并行处理多个任务
            Console.WriteLine("\n示例 2: 并行处理多个任务");
            var tasks = new List<Task>();
            for (int i = 1; i <= 10; i++)
            {
                var grainId = $"UserGrain-{i}";
                var userId = i;
                tasks.Add(Task.Run(async () =>
                {
                    await eventSourcingService.SaveEventAsync(grainId, new UserCreatedEvent { UserId = userId, Username = $"user{userId}" });
                    await eventSourcingService.SaveEventAsync(grainId, new UserUpdatedEvent { UserId = userId, Email = $"user{userId}@example.com" });
                }));
            }
            
            stopwatch.Restart();
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine($"并行处理 10 个用户任务耗时: {stopwatch.ElapsedMilliseconds}ms");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 4: 错误处理

### 功能说明

演示 Orleans 技能的错误处理机制，包括异常捕获、错误日志和优雅错误处理。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Client@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Orleans 错误处理示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 Orleans 服务
        services.AddOrleansServices(options =>
        {
            options.Enabled = true;
            options.ClusterId = "error-handling-cluster";
            options.ServiceId = "error-handling-service";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var orleansService = serviceProvider.GetRequiredService<IOrleansService>();
        var eventSourcingService = serviceProvider.GetRequiredService<IOrleansEventSourcingService>();
        var stateMachineService = serviceProvider.GetRequiredService<IOrleansStateMachineService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 正常操作
            Console.WriteLine("示例 1: 正常操作");
            await orleansService.InitializeClusterAsync();
            Console.WriteLine("集群初始化成功");

            // 示例 2: 事件保存
            Console.WriteLine("\n示例 2: 事件保存");
            await eventSourcingService.SaveEventAsync("UserGrain-1", new { Type = "TestEvent" });
            Console.WriteLine("事件保存成功");

            // 示例 3: 状态机操作
            Console.WriteLine("\n示例 3: 状态机操作");
            await stateMachineService.CreateStateMachineAsync("Order-123", "Created");
            await stateMachineService.TriggerEventAsync("Order-123", "Start");
            var currentState = await stateMachineService.GetCurrentStateAsync("Order-123");
            Console.WriteLine($"当前状态: {currentState}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine($"错误类型: {ex.GetType().Name}");
            
            // 优雅错误处理
            Console.WriteLine("正在执行错误恢复操作...");
            // 这里可以添加错误恢复逻辑，例如重试、降级等
            Console.WriteLine("错误恢复操作完成");
        }
    }
}
```

## 示例 5: 自定义 Orleans 服务

### 功能说明

演示如何创建自定义的 Orleans 服务，扩展 Orleans 技能的功能。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Client@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// 自定义 Orleans 服务接口
public interface ICustomOrleansService
{
    Task<string> ProcessCustomOperationAsync(string input, CancellationToken cancellationToken = default);
    Task<int> CalculateAsync(int a, int b, CancellationToken cancellationToken = default);
}

// 自定义 Orleans 服务实现
public class CustomOrleansService : ICustomOrleansService
{
    private readonly ILogger<CustomOrleansService> _logger;

    public CustomOrleansService(ILogger<CustomOrleansService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ProcessCustomOperationAsync(string input, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing custom operation with input: {Input}", input);
        await Task.Delay(500, cancellationToken); // 模拟处理过程
        return $"Processed: {input}";
    }

    public async Task<int> CalculateAsync(int a, int b, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating {A} + {B}", a, b);
        await Task.Delay(100, cancellationToken); // 模拟计算过程
        return a + b;
    }
}

// 依赖注入扩展
public static class CustomOrleansServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOrleansServices(this IServiceCollection services)
    {
        // 注册基础 Orleans 服务
        services.AddOrleansServices();
        
        // 注册自定义服务
        services.AddSingleton<ICustomOrleansService, CustomOrleansService>();
        
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Orleans 自定义服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册自定义 Orleans 服务
        services.AddCustomOrleansServices();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var customService = serviceProvider.GetRequiredService<ICustomOrleansService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 调用自定义操作
            Console.WriteLine("示例 1: 调用自定义操作");
            var result = await customService.ProcessCustomOperationAsync("Hello, Orleans!");
            Console.WriteLine($"操作结果: {result}");

            // 示例 2: 调用计算方法
            Console.WriteLine("\n示例 2: 调用计算方法");
            var sum = await customService.CalculateAsync(10, 20);
            Console.WriteLine($"计算结果: 10 + 20 = {sum}");

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 实际应用场景

### 场景 1: 电子商务系统

在电子商务系统中，Orleans 技能可以用于：

- **订单处理**：使用状态机管理订单生命周期（创建、支付、发货、完成）
- **库存管理**：使用事件溯源记录库存变动
- **用户行为追踪**：记录用户浏览、购买行为，用于推荐系统
- **分布式事务**：确保订单、支付、库存等操作的一致性

### 场景 2: 金融交易系统

在金融交易系统中，Orleans 技能可以用于：

- **交易处理**：使用事件溯源记录每笔交易
- **风险监控**：实时监控交易风险
- **账户管理**：管理用户账户状态和余额变动
- **审计日志**：提供完整的操作审计记录

### 场景 3: IoT 系统

在 IoT 系统中，Orleans 技能可以用于：

- **设备管理**：管理大量设备的状态和通信
- **数据处理**：处理设备产生的大量数据
- **规则引擎**：基于设备数据执行规则和触发动作
- **设备仿真**：模拟设备行为进行测试

### 场景 4: 游戏服务器

在游戏服务器中，Orleans 技能可以用于：

- **游戏状态管理**：管理游戏世界和玩家状态
- **玩家会话**：处理玩家登录、登出和会话状态
- **游戏逻辑**：执行游戏核心逻辑
- **排行榜**：维护游戏排行榜数据

### 场景 5: 内容管理系统

在内容管理系统中，Orleans 技能可以用于：

- **内容处理**：处理和转换内容（图片、视频、文档）
- **工作流管理**：管理内容审批和发布流程
- **版本控制**：使用事件溯源记录内容版本变更
- **搜索索引**：维护内容搜索索引

### 场景 6: 实时分析系统

在实时分析系统中，Orleans 技能可以用于：

- **数据收集**：收集和处理实时数据
- **指标计算**：计算各种业务指标
- **告警系统**：基于指标触发告警
- **报表生成**：生成实时和历史报表
