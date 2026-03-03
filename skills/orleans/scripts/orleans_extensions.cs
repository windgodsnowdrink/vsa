#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Orleans.Core@8.0.0
#:package Microsoft.Orleans.Client@8.0.0
#:package Microsoft.Orleans.Server@8.0.0
#:package System.Text.Json@10.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 配置选项
public class OrleansOptions
{
    public bool Enabled { get; set; } = true;
    public string ClusterId { get; set; } = "orleans-cluster";
    public string ServiceId { get; set; } = "orleans-service";
    public int SiloPort { get; set; } = 11111;
    public int GatewayPort { get; set; } = 30000;
    public bool EnableEventSourcing { get; set; } = true;
    public bool EnableStateMachine { get; set; } = true;
    public bool EnableClusterManagement { get; set; } = true;
    public bool EnableConfiguration { get; set; } = true;
    public bool EnableDeployment { get; set; } = true;
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
}

// Orleans 配置
public class OrleansConfiguration
{
    public string ClusterId { get; set; } = "orleans-cluster";
    public string ServiceId { get; set; } = "orleans-service";
    public int SiloPort { get; set; } = 11111;
    public int GatewayPort { get; set; } = 30000;
    public bool EnableEventSourcing { get; set; } = true;
    public bool EnableStateMachine { get; set; } = true;
    public bool EnableClusterManagement { get; set; } = true;
    public bool EnableConfiguration { get; set; } = true;
    public bool EnableDeployment { get; set; } = true;
}

// Orleans 集群节点信息
public class OrleansClusterNode
{
    public string NodeId { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? LastHeartbeat { get; set; }
}

// Orleans 集群状态
public class OrleansClusterStatus
{
    public string Status { get; set; } = "Unknown";
    public List<OrleansClusterNode> Nodes { get; set; } = new List<OrleansClusterNode>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Orleans 服务接口
public interface IOrleansService
{
    Task InitializeClusterAsync(CancellationToken cancellationToken = default);
    Task<OrleansClusterStatus> GetClusterStatusAsync(CancellationToken cancellationToken = default);
    Task<bool> AddNodeAsync(string nodeId, string endpoint, CancellationToken cancellationToken = default);
    Task<bool> RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default);
    Task<bool> RestartNodeAsync(string nodeId, CancellationToken cancellationToken = default);
}

// Orleans 事件溯源服务接口
public interface IOrleansEventSourcingService
{
    Task SaveEventAsync(string grainId, object @event, CancellationToken cancellationToken = default);
    Task SaveEventsAsync(string grainId, IEnumerable<object> events, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetEventsAsync(string grainId, CancellationToken cancellationToken = default);
    Task<T> ReplayEventsAsync<T>(string grainId, CancellationToken cancellationToken = default) where T : new();
}

// Orleans 状态机服务接口
public interface IOrleansStateMachineService
{
    Task CreateStateMachineAsync(string stateMachineId, string initialState, CancellationToken cancellationToken = default);
    Task<bool> TriggerEventAsync(string stateMachineId, string @event, CancellationToken cancellationToken = default);
    Task<string> GetCurrentStateAsync(string stateMachineId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetStateHistoryAsync(string stateMachineId, CancellationToken cancellationToken = default);
}

// Orleans 集群管理服务接口
public interface IOrleansClusterService
{
    Task<OrleansClusterStatus> GetClusterStatusAsync(CancellationToken cancellationToken = default);
    Task<bool> AddNodeAsync(string nodeId, string endpoint, CancellationToken cancellationToken = default);
    Task<bool> RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default);
    Task<bool> RestartNodeAsync(string nodeId, CancellationToken cancellationToken = default);
    Task<bool> UpdateNodeStatusAsync(string nodeId, string status, CancellationToken cancellationToken = default);
}

// Orleans 配置管理服务接口
public interface IOrleansConfigurationService
{
    Task<OrleansConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateConfigurationAsync(OrleansConfiguration configuration, CancellationToken cancellationToken = default);
    Task<bool> ResetConfigurationAsync(CancellationToken cancellationToken = default);
}

// Orleans 部署服务接口
public interface IOrleansDeploymentService
{
    Task<bool> PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default);
    Task<bool> DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default);
    Task<bool> DeployToKubernetesAsync(string kubeconfigPath, string namespaceName, string deploymentName, CancellationToken cancellationToken = default);
}

// Orleans 服务实现
public class OrleansService : IOrleansService
{
    private readonly ILogger<OrleansService> _logger;
    private readonly OrleansOptions _options;
    private bool _isInitialized = false;

    public OrleansService(ILogger<OrleansService> logger, IOptions<OrleansOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task InitializeClusterAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing Orleans cluster: {ClusterId}", _options.ClusterId);
        await Task.Delay(1000, cancellationToken); // 模拟初始化过程

        _isInitialized = true;
        _logger.LogInformation("Orleans cluster initialized successfully");
    }

    public async Task<OrleansClusterStatus> GetClusterStatusAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting cluster status");
        await Task.Delay(500, cancellationToken); // 模拟获取过程

        var status = new OrleansClusterStatus
        {
            Status = _isInitialized ? "Healthy" : "NotInitialized",
            Nodes = new List<OrleansClusterNode>
            {
                new OrleansClusterNode
                {
                    NodeId = "node-1",
                    Endpoint = "localhost:11111",
                    Status = "Active",
                    LastHeartbeat = DateTime.UtcNow
                }
            }
        };

        return status;
    }

    public async Task<bool> AddNodeAsync(string nodeId, string endpoint, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding node: {NodeId} at {Endpoint}", nodeId, endpoint);
        await Task.Delay(800, cancellationToken); // 模拟添加过程

        _logger.LogInformation("Node added successfully: {NodeId}", nodeId);
        return true;
    }

    public async Task<bool> RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing node: {NodeId}", nodeId);
        await Task.Delay(600, cancellationToken); // 模拟移除过程

        _logger.LogInformation("Node removed successfully: {NodeId}", nodeId);
        return true;
    }

    public async Task<bool> RestartNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Restarting node: {NodeId}", nodeId);
        await Task.Delay(1200, cancellationToken); // 模拟重启过程

        _logger.LogInformation("Node restarted successfully: {NodeId}", nodeId);
        return true;
    }
}

// Orleans 事件溯源服务实现
public class OrleansEventSourcingService : IOrleansEventSourcingService
{
    private readonly ILogger<OrleansEventSourcingService> _logger;
    private readonly Dictionary<string, List<object>> _eventStore = new Dictionary<string, List<object>>();

    public OrleansEventSourcingService(ILogger<OrleansEventSourcingService> logger)
    {
        _logger = logger;
    }

    public async Task SaveEventAsync(string grainId, object @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving event for grain: {GrainId}", grainId);
        await Task.Delay(200, cancellationToken); // 模拟保存过程

        if (!_eventStore.ContainsKey(grainId))
        {
            _eventStore[grainId] = new List<object>();
        }

        _eventStore[grainId].Add(@event);
        _logger.LogInformation("Event saved successfully for grain: {GrainId}", grainId);
    }

    public async Task SaveEventsAsync(string grainId, IEnumerable<object> events, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving events for grain: {GrainId}", grainId);
        await Task.Delay(300, cancellationToken); // 模拟保存过程

        if (!_eventStore.ContainsKey(grainId))
        {
            _eventStore[grainId] = new List<object>();
        }

        _eventStore[grainId].AddRange(events);
        _logger.LogInformation("Events saved successfully for grain: {GrainId}", grainId);
    }

    public async Task<IEnumerable<object>> GetEventsAsync(string grainId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting events for grain: {GrainId}", grainId);
        await Task.Delay(100, cancellationToken); // 模拟获取过程

        if (_eventStore.ContainsKey(grainId))
        {
            return _eventStore[grainId];
        }

        return Enumerable.Empty<object>();
    }

    public async Task<T> ReplayEventsAsync<T>(string grainId, CancellationToken cancellationToken = default) where T : new()
    {
        _logger.LogInformation("Replaying events for grain: {GrainId}", grainId);
        await Task.Delay(400, cancellationToken); // 模拟回放过程

        var state = new T();
        // 注意：在实际应用中，这里应该根据事件类型更新状态
        return state;
    }
}

// Orleans 状态机服务实现
public class OrleansStateMachineService : IOrleansStateMachineService
{
    private readonly ILogger<OrleansStateMachineService> _logger;
    private readonly Dictionary<string, string> _currentStates = new Dictionary<string, string>();
    private readonly Dictionary<string, List<string>> _stateHistories = new Dictionary<string, List<string>>();

    public OrleansStateMachineService(ILogger<OrleansStateMachineService> logger)
    {
        _logger = logger;
    }

    public async Task CreateStateMachineAsync(string stateMachineId, string initialState, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating state machine: {StateMachineId} with initial state: {InitialState}", stateMachineId, initialState);
        await Task.Delay(300, cancellationToken); // 模拟创建过程

        _currentStates[stateMachineId] = initialState;
        _stateHistories[stateMachineId] = new List<string> { initialState };
        _logger.LogInformation("State machine created successfully: {StateMachineId}", stateMachineId);
    }

    public async Task<bool> TriggerEventAsync(string stateMachineId, string @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Triggering event: {Event} for state machine: {StateMachineId}", @event, stateMachineId);
        await Task.Delay(200, cancellationToken); // 模拟触发过程

        if (_currentStates.TryGetValue(stateMachineId, out var currentState))
        {
            // 注意：在实际应用中，这里应该根据状态机定义计算下一个状态
            var nextState = CalculateNextState(currentState, @event);
            if (!string.IsNullOrEmpty(nextState))
            {
                _currentStates[stateMachineId] = nextState;
                _stateHistories[stateMachineId].Add(nextState);
                _logger.LogInformation("State transition successful: {CurrentState} -> {NextState}", currentState, nextState);
                return true;
            }
        }

        _logger.LogError("State transition failed for state machine: {StateMachineId}", stateMachineId);
        return false;
    }

    public async Task<string> GetCurrentStateAsync(string stateMachineId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting current state for state machine: {StateMachineId}", stateMachineId);
        await Task.Delay(100, cancellationToken); // 模拟获取过程

        if (_currentStates.TryGetValue(stateMachineId, out var currentState))
        {
            return currentState;
        }

        return "Unknown";
    }

    public async Task<IEnumerable<string>> GetStateHistoryAsync(string stateMachineId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting state history for state machine: {StateMachineId}", stateMachineId);
        await Task.Delay(150, cancellationToken); // 模拟获取过程

        if (_stateHistories.TryGetValue(stateMachineId, out var history))
        {
            return history;
        }

        return Enumerable.Empty<string>();
    }

    private string CalculateNextState(string currentState, string @event)
    {
        // 简单的状态转换逻辑，实际应用中应该使用更复杂的状态机定义
        switch (currentState)
        {
            case "Created":
                if (@event == "Start") return "Running";
                break;
            case "Running":
                if (@event == "Pause") return "Paused";
                if (@event == "Complete") return "Completed";
                if (@event == "Fail") return "Failed";
                break;
            case "Paused":
                if (@event == "Resume") return "Running";
                if (@event == "Fail") return "Failed";
                break;
        }
        return string.Empty;
    }
}

// Orleans 集群管理服务实现
public class OrleansClusterService : IOrleansClusterService
{
    private readonly ILogger<OrleansClusterService> _logger;
    private readonly List<OrleansClusterNode> _nodes = new List<OrleansClusterNode>();

    public OrleansClusterService(ILogger<OrleansClusterService> logger)
    {
        _logger = logger;
        // 初始化默认节点
        _nodes.Add(new OrleansClusterNode
        {
            NodeId = "node-1",
            Endpoint = "localhost:11111",
            Status = "Active",
            LastHeartbeat = DateTime.UtcNow
        });
    }

    public async Task<OrleansClusterStatus> GetClusterStatusAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting cluster status");
        await Task.Delay(200, cancellationToken); // 模拟获取过程

        var status = new OrleansClusterStatus
        {
            Status = _nodes.Any(n => n.Status == "Active") ? "Healthy" : "Unhealthy",
            Nodes = _nodes
        };

        return status;
    }

    public async Task<bool> AddNodeAsync(string nodeId, string endpoint, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding node: {NodeId} at {Endpoint}", nodeId, endpoint);
        await Task.Delay(500, cancellationToken); // 模拟添加过程

        if (!_nodes.Any(n => n.NodeId == nodeId))
        {
            _nodes.Add(new OrleansClusterNode
            {
                NodeId = nodeId,
                Endpoint = endpoint,
                Status = "Active",
                LastHeartbeat = DateTime.UtcNow
            });
            _logger.LogInformation("Node added successfully: {NodeId}", nodeId);
            return true;
        }

        _logger.LogWarning("Node already exists: {NodeId}", nodeId);
        return false;
    }

    public async Task<bool> RemoveNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing node: {NodeId}", nodeId);
        await Task.Delay(400, cancellationToken); // 模拟移除过程

        var node = _nodes.FirstOrDefault(n => n.NodeId == nodeId);
        if (node != null)
        {
            _nodes.Remove(node);
            _logger.LogInformation("Node removed successfully: {NodeId}", nodeId);
            return true;
        }

        _logger.LogWarning("Node not found: {NodeId}", nodeId);
        return false;
    }

    public async Task<bool> RestartNodeAsync(string nodeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Restarting node: {NodeId}", nodeId);
        await Task.Delay(800, cancellationToken); // 模拟重启过程

        var node = _nodes.FirstOrDefault(n => n.NodeId == nodeId);
        if (node != null)
        {
            node.Status = "Active";
            node.LastHeartbeat = DateTime.UtcNow;
            _logger.LogInformation("Node restarted successfully: {NodeId}", nodeId);
            return true;
        }

        _logger.LogWarning("Node not found: {NodeId}", nodeId);
        return false;
    }

    public async Task<bool> UpdateNodeStatusAsync(string nodeId, string status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating node status: {NodeId} to {Status}", nodeId, status);
        await Task.Delay(300, cancellationToken); // 模拟更新过程

        var node = _nodes.FirstOrDefault(n => n.NodeId == nodeId);
        if (node != null)
        {
            node.Status = status;
            node.LastHeartbeat = DateTime.UtcNow;
            _logger.LogInformation("Node status updated successfully: {NodeId}", nodeId);
            return true;
        }

        _logger.LogWarning("Node not found: {NodeId}", nodeId);
        return false;
    }
}

// Orleans 配置管理服务实现
public class OrleansConfigurationService : IOrleansConfigurationService
{
    private readonly ILogger<OrleansConfigurationService> _logger;
    private OrleansConfiguration _configuration;

    public OrleansConfigurationService(ILogger<OrleansConfigurationService> logger)
    {
        _logger = logger;
        // 初始化默认配置
        _configuration = new OrleansConfiguration
        {
            ClusterId = "orleans-cluster",
            ServiceId = "orleans-service",
            SiloPort = 11111,
            GatewayPort = 30000,
            EnableEventSourcing = true,
            EnableStateMachine = true,
            EnableClusterManagement = true,
            EnableConfiguration = true,
            EnableDeployment = true
        };
    }

    public async Task<OrleansConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting Orleans configuration");
        await Task.Delay(100, cancellationToken); // 模拟获取过程

        return _configuration;
    }

    public async Task<bool> UpdateConfigurationAsync(OrleansConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating Orleans configuration");
        await Task.Delay(400, cancellationToken); // 模拟更新过程

        _configuration = configuration;
        _logger.LogInformation("Orleans configuration updated successfully");
        return true;
    }

    public async Task<bool> ResetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resetting Orleans configuration to default");
        await Task.Delay(300, cancellationToken); // 模拟重置过程

        _configuration = new OrleansConfiguration
        {
            ClusterId = "orleans-cluster",
            ServiceId = "orleans-service",
            SiloPort = 11111,
            GatewayPort = 30000,
            EnableEventSourcing = true,
            EnableStateMachine = true,
            EnableClusterManagement = true,
            EnableConfiguration = true,
            EnableDeployment = true
        };

        _logger.LogInformation("Orleans configuration reset successfully");
        return true;
    }
}

// Orleans 部署服务实现
public class OrleansDeploymentService : IOrleansDeploymentService
{
    private readonly ILogger<OrleansDeploymentService> _logger;

    public OrleansDeploymentService(ILogger<OrleansDeploymentService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> PackageApplicationAsync(string outputPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Packaging Orleans application to: {OutputPath}", outputPath);
        await Task.Delay(2000, cancellationToken); // 模拟打包过程

        // 确保输出目录存在
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 模拟创建包文件
        await File.WriteAllTextAsync(Path.Combine(outputPath, "orleans-package.zip"), "Package content", cancellationToken);
        _logger.LogInformation("Orleans application packaged successfully");
        return true;
    }

    public async Task<bool> DeployApplicationAsync(string serverUrl, string username, string password, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deploying Orleans application to: {ServerUrl}", serverUrl);
        await Task.Delay(3000, cancellationToken); // 模拟部署过程

        _logger.LogInformation("Orleans application deployed successfully to {ServerUrl}", serverUrl);
        return true;
    }

    public async Task<bool> DeployToKubernetesAsync(string kubeconfigPath, string namespaceName, string deploymentName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deploying Orleans application to Kubernetes: {DeploymentName} in namespace {NamespaceName}", deploymentName, namespaceName);
        await Task.Delay(2500, cancellationToken); // 模拟部署过程

        _logger.LogInformation("Orleans application deployed successfully to Kubernetes");
        return true;
    }
}

// 依赖注入扩展
public static class OrleansServiceCollectionExtensions
{
    public static IServiceCollection AddOrleansServices(this IServiceCollection services, Action<OrleansOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OrleansOptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IOrleansService, OrleansService>();
        services.AddSingleton<IOrleansEventSourcingService, OrleansEventSourcingService>();
        services.AddSingleton<IOrleansStateMachineService, OrleansStateMachineService>();
        services.AddSingleton<IOrleansClusterService, OrleansClusterService>();
        services.AddSingleton<IOrleansConfigurationService, OrleansConfigurationService>();
        services.AddSingleton<IOrleansDeploymentService, OrleansDeploymentService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Orleans 技能示例");
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
            options.EnableClusterManagement = true;
            options.EnableConfiguration = true;
            options.EnableDeployment = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var orleansService = serviceProvider.GetRequiredService<IOrleansService>();
        var eventSourcingService = serviceProvider.GetRequiredService<IOrleansEventSourcingService>();
        var stateMachineService = serviceProvider.GetRequiredService<IOrleansStateMachineService>();
        var clusterService = serviceProvider.GetRequiredService<IOrleansClusterService>();
        var configService = serviceProvider.GetRequiredService<IOrleansConfigurationService>();
        var deploymentService = serviceProvider.GetRequiredService<IOrleansDeploymentService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 初始化集群
            Console.WriteLine("示例 1: 初始化 Orleans 集群");
            await orleansService.InitializeClusterAsync();
            Console.WriteLine("集群初始化成功");

            // 示例 2: 使用事件溯源
            Console.WriteLine("\n示例 2: 使用事件溯源");
            var @event = new { Type = "UserCreated", UserId = 1, Username = "user1" };
            await eventSourcingService.SaveEventAsync("UserGrain-1", @event);
            Console.WriteLine("事件保存成功");

            var events = await eventSourcingService.GetEventsAsync("UserGrain-1");
            Console.WriteLine($"事件数量: {events.Count()}");

            // 示例 3: 使用状态机
            Console.WriteLine("\n示例 3: 使用状态机");
            await stateMachineService.CreateStateMachineAsync("Order-123", "Created");
            await stateMachineService.TriggerEventAsync("Order-123", "Start");
            var currentState = await stateMachineService.GetCurrentStateAsync("Order-123");
            Console.WriteLine($"当前状态: {currentState}");

            // 示例 4: 获取集群状态
            Console.WriteLine("\n示例 4: 获取集群状态");
            var clusterStatus = await clusterService.GetClusterStatusAsync();
            Console.WriteLine($"集群状态: {clusterStatus.Status}");
            Console.WriteLine($"节点数量: {clusterStatus.Nodes.Count}");

            // 示例 5: 配置管理
            Console.WriteLine("\n示例 5: 配置管理");
            var config = await configService.GetConfigurationAsync();
            Console.WriteLine($"当前配置: 集群 ID={config.ClusterId}, 服务 ID={config.ServiceId}");

            // 示例 6: 部署工具
            Console.WriteLine("\n示例 6: 部署工具");
            var tempPath = Path.Combine(Path.GetTempPath(), "orleans-package");
            Directory.CreateDirectory(tempPath);
            await deploymentService.PackageApplicationAsync(tempPath);
            Console.WriteLine("应用打包成功");

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}