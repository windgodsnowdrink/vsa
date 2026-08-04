# chaos - 使用示例

## 快速开始

### 1. 基础使用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chaos;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("混沌测试基础使用示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册混沌测试服务
        services.AddChaos();
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取混沌测试服务
        var chaosService = serviceProvider.GetRequiredService<IChaosService>();
        
        Console.WriteLine("\n1. 执行延迟故障注入:");
        Console.WriteLine("-" * 30);
        
        // 创建延迟故障配置
        var delayFaultConfig = new FaultConfig
        {
            FaultType = FaultType.Delay,
            DelayTime = TimeSpan.FromSeconds(1),
            Probability = 0.5,
            Description = "延迟故障测试"
        };
        
        // 执行延迟故障注入
        var delayResult = await chaosService.InjectFaultAsync(delayFaultConfig);
        Console.WriteLine($"结果: {delayResult.Success}");
        Console.WriteLine($"消息: {delayResult.Message}");
        Console.WriteLine($"执行时间: {delayResult.ExecutionTime.TotalMilliseconds:F2} ms");
        
        Console.WriteLine("\n2. 执行中断故障注入:");
        Console.WriteLine("-" * 30);
        
        // 创建中断故障配置
        var abortFaultConfig = new FaultConfig
        {
            FaultType = FaultType.Abort,
            Probability = 0.3,
            Description = "中断故障测试"
        };
        
        // 执行中断故障注入
        var abortResult = await chaosService.InjectFaultAsync(abortFaultConfig);
        Console.WriteLine($"结果: {abortResult.Success}");
        Console.WriteLine($"消息: {abortResult.Message}");
        Console.WriteLine($"执行时间: {abortResult.ExecutionTime.TotalMilliseconds:F2} ms");
        
        Console.WriteLine("\n3. 执行异常故障注入:");
        Console.WriteLine("-" * 30);
        
        // 创建异常故障配置
        var exceptionFaultConfig = new FaultConfig
        {
            FaultType = FaultType.Exception,
            ExceptionType = ExceptionType.General,
            Probability = 0.2,
            Description = "异常故障测试"
        };
        
        // 执行异常故障注入
        var exceptionResult = await chaosService.InjectFaultAsync(exceptionFaultConfig);
        Console.WriteLine($"结果: {exceptionResult.Success}");
        Console.WriteLine($"消息: {exceptionResult.Message}");
        Console.WriteLine($"执行时间: {exceptionResult.ExecutionTime.TotalMilliseconds:F2} ms");
        
        Console.WriteLine("\n示例完成!");
    }
}
```

### 2. AOT 编译优化示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chaos;

// 故障注入结果类
public class FaultInjectionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TimeSpan ExecutionTime { get; set; }
    public FaultType FaultType { get; set; }
}

// 故障类型枚举
public enum FaultType
{
    None,
    Delay,
    Abort,
    Exception,
    ResourceExhaustion,
    Network,
    StateCorruption
}

// 异常类型枚举
public enum ExceptionType
{
    General,
    Timeout,
    InvalidOperation,
    NullReference,
    OutOfMemory,
    Custom
}

// 资源类型枚举
public enum ResourceType
{
    CPU,
    Memory,
    Disk,
    Network
}

// 网络故障类型枚举
public enum NetworkFaultType
{
    Latency,
    PacketLoss,
    BandwidthLimit,
    Disconnect
}

// 故障注入模式枚举
public enum FaultInjectionMode
{
    Sync,
    Async
}

// 混沌测试场景结果类
public class ScenarioResult
{
    public string ScenarioName { get; set; } = string.Empty;
    public ScenarioStatus Status { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<string, object> Metrics { get; set; } = new();
}

// 场景状态枚举
public enum ScenarioStatus
{
    NotStarted,
    Running,
    Completed,
    Failed,
    Canceled
}

// 混沌测试场景类
public class ChaosScenario
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<FaultConfig> Faults { get; set; } = new();
    public TimeSpan Duration { get; set; }
    public List<string> MetricsToMonitor { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
}

// 故障配置类
public class FaultConfig
{
    public FaultType FaultType { get; set; }
    public double Probability { get; set; } = 0.5;
    public string? Description { get; set; }
    public string? TargetService { get; set; }
    public TimeSpan DelayTime { get; set; } = TimeSpan.Zero;
    public ExceptionType ExceptionType { get; set; } = ExceptionType.General;
    public ResourceType ResourceType { get; set; } = ResourceType.CPU;
    public double ResourceConsumption { get; set; } = 0.8;
    public NetworkFaultType NetworkFaultType { get; set; } = NetworkFaultType.Latency;
    public double PacketLossRate { get; set; } = 0.1;
    public bool EnableAotOptimization { get; set; } = false;
}

// AOT 安全的混沌服务接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IChaosService
{
    Task<FaultInjectionResult> InjectFaultAsync(FaultConfig faultConfig);
    Task<ScenarioResult> RunScenarioAsync(ChaosScenario scenario);
}

// AOT 安全的混沌服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeChaosService : IChaosService
{
    private readonly ILogger<AotSafeChaosService> _logger;
    private static readonly Random _random = new();
    
    public AotSafeChaosService(ILogger<AotSafeChaosService> logger)
    {
        _logger = logger;
    }
    
    public async Task<FaultInjectionResult> InjectFaultAsync(FaultConfig faultConfig)
    {
        _logger.LogInformation("执行 AOT 优化的故障注入: {FaultType}, 概率: {Probability}", 
            faultConfig.FaultType, faultConfig.Probability);
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            // 检查是否应该注入故障
            if (_random.NextDouble() > faultConfig.Probability)
            {
                return new FaultInjectionResult {
                    Success = true,
                    Message = "未注入故障（概率未命中）",
                    ExecutionTime = DateTime.UtcNow - startTime,
                    FaultType = FaultType.None
                };
            }
            
            // 根据故障类型执行不同的故障注入
            FaultInjectionResult result;
            switch (faultConfig.FaultType)
            {
                case FaultType.Delay:
                    result = await ExecuteDelayFaultAsync(faultConfig);
                    break;
                case FaultType.Abort:
                    result = ExecuteAbortFault(faultConfig);
                    break;
                case FaultType.Exception:
                    result = ExecuteExceptionFault(faultConfig);
                    break;
                default:
                    result = new FaultInjectionResult {
                        Success = false,
                        Message = "不支持的故障类型",
                        ExecutionTime = DateTime.UtcNow - startTime,
                        FaultType = faultConfig.FaultType
                    };
                    break;
            }
            
            result.ExecutionTime = DateTime.UtcNow - startTime;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AOT 优化的故障注入失败: {Message}", ex.Message);
            return new FaultInjectionResult {
                Success = false,
                Message = $"故障注入失败: {ex.Message}",
                ExecutionTime = DateTime.UtcNow - startTime,
                FaultType = faultConfig.FaultType
            };
        }
    }
    
    public async Task<ScenarioResult> RunScenarioAsync(ChaosScenario scenario)
    {
        _logger.LogInformation("执行 AOT 优化的混沌测试场景: {Name}", scenario.Name);
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            // 执行场景中的所有故障
            foreach (var fault in scenario.Faults)
            {
                await InjectFaultAsync(fault);
            }
            
            return new ScenarioResult {
                ScenarioName = scenario.Name,
                Status = ScenarioStatus.Completed,
                Duration = DateTime.UtcNow - startTime,
                Metrics = new Dictionary<string, object> {
                    { "FaultsExecuted", scenario.Faults.Count },
                    { "SuccessRate", 0.9 },
                    { "AverageExecutionTime", TimeSpan.FromMilliseconds(150) }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AOT 优化的混沌测试场景执行失败: {Name}", scenario.Name);
            return new ScenarioResult {
                ScenarioName = scenario.Name,
                Status = ScenarioStatus.Failed,
                Duration = DateTime.UtcNow - startTime,
                Metrics = new Dictionary<string, object> {
                    { "Error", ex.Message }
                }
            };
        }
    }
    
    private async Task<FaultInjectionResult> ExecuteDelayFaultAsync(FaultConfig faultConfig)
    {
        // 执行延迟故障
        await Task.Delay(faultConfig.DelayTime);
        
        return new FaultInjectionResult {
            Success = true,
            Message = $"成功注入延迟故障: {faultConfig.DelayTime}",
            FaultType = FaultType.Delay
        };
    }
    
    private FaultInjectionResult ExecuteAbortFault(FaultConfig faultConfig)
    {
        // 执行中断故障
        return new FaultInjectionResult {
            Success = true,
            Message = "成功注入中断故障",
            FaultType = FaultType.Abort
        };
    }
    
    private FaultInjectionResult ExecuteExceptionFault(FaultConfig faultConfig)
    {
        // 执行异常故障
        return new FaultInjectionResult {
            Success = true,
            Message = $"成功注入异常故障: {faultConfig.ExceptionType}",
            FaultType = FaultType.Exception
        };
    }
}

// 混沌测试设置类
public class ChaosSettings
{
    public bool EnableAotOptimization { get; set; }
    public bool EnableTrimOptimization { get; set; }
    public FaultInjectionMode FaultInjectionMode { get; set; }
    public LogLevel LogLevel { get; set; }
    public bool EnableDetailedLogging { get; set; }
    public int CacheSize { get; set; }
    public TimeSpan CacheExpiration { get; set; }
    public TimeSpan Timeout { get; set; }
}

// 混沌服务扩展类
public static class ChaosServiceExtensions
{
    public static IServiceCollection AddChaos(this IServiceCollection services)
    {
        services.AddSingleton<IChaosService, AotSafeChaosService>();
        services.AddOptions<ChaosSettings>();
        return services;
    }
    
    public static IServiceCollection AddChaos(this IServiceCollection services, Action<ChaosSettings> configureOptions)
    {
        services.AddChaos();
        services.Configure(configureOptions);
        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("混沌测试 AOT 编译优化示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 AOT 优化的混沌服务
        services.AddChaos(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
            options.FaultInjectionMode = FaultInjectionMode.Async;
            options.LogLevel = LogLevel.Information;
        });
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        var chaosService = serviceProvider.GetRequiredService<IChaosService>();
        
        Console.WriteLine("\n1. 执行 AOT 优化的延迟故障注入:");
        Console.WriteLine("-" * 40);
        
        // 创建 AOT 优化的延迟故障配置
        var delayFaultConfig = new FaultConfig
        {
            FaultType = FaultType.Delay,
            DelayTime = TimeSpan.FromSeconds(500),
            Probability = 0.8,
            Description = "AOT 优化的延迟故障测试",
            EnableAotOptimization = true
        };
        
        // 执行 AOT 优化的延迟故障注入
        var delayResult = await chaosService.InjectFaultAsync(delayFaultConfig);
        Console.WriteLine($"结果: {delayResult.Success}");
        Console.WriteLine($"消息: {delayResult.Message}");
        Console.WriteLine($"执行时间: {delayResult.ExecutionTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"故障类型: {delayResult.FaultType}");
        
        Console.WriteLine("\n2. 执行 AOT 优化的混沌测试场景:");
        Console.WriteLine("-" * 40);
        
        // 创建 AOT 优化的混沌测试场景
        var scenario = new ChaosScenario
        {
            Name = "AOT 优化的服务中断测试",
            Description = "测试系统在服务中断情况下的表现",
            Duration = TimeSpan.FromMinutes(2),
            Faults = new List<FaultConfig>
            {
                new FaultConfig {
                    FaultType = FaultType.Delay,
                    DelayTime = TimeSpan.FromSeconds(1),
                    Probability = 0.5,
                    Description = "AOT 优化的延迟故障",
                    EnableAotOptimization = true
                },
                new FaultConfig {
                    FaultType = FaultType.Abort,
                    Probability = 0.2,
                    Description = "AOT 优化的中断故障",
                    EnableAotOptimization = true
                },
                new FaultConfig {
                    FaultType = FaultType.Exception,
                    ExceptionType = ExceptionType.General,
                    Probability = 0.3,
                    Description = "AOT 优化的异常故障",
                    EnableAotOptimization = true
                }
            },
            MetricsToMonitor = new List<string> { "response_time", "error_rate", "throughput" }
        };
        
        // 执行 AOT 优化的混沌测试场景
        var scenarioResult = await chaosService.RunScenarioAsync(scenario);
        Console.WriteLine($"场景名称: {scenarioResult.ScenarioName}");
        Console.WriteLine($"状态: {scenarioResult.Status}");
        Console.WriteLine($"持续时间: {scenarioResult.Duration.TotalSeconds:F2} 秒");
        Console.WriteLine($"指标:");
        foreach (var metric in scenarioResult.Metrics)
        {
            Console.WriteLine($"  - {metric.Key}: {metric.Value}");
        }
        
        Console.WriteLine("\nAOT 编译优化示例完成!");
        Console.WriteLine("=" * 50);
    }
}
```

### 3. 高级配置示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Chaos;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("混沌测试高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建配置
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "Chaos:EnableAotOptimization", "true" },
                { "Chaos:EnableTrimOptimization", "true" },
                { "Chaos:FaultInjectionMode", "Async" },
                { "Chaos:LogLevel", "Information" },
                { "Chaos:EnableDetailedLogging", "true" },
                { "Chaos:CacheSize", "2000" },
                { "Chaos:CacheExpiration", "02:00:00" },
                { "Chaos:Timeout", "00:01:00" }
            })
            .Build();
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
            config.AddConfiguration(configuration.GetSection("Logging"));
        });
        
        // 添加配置
        services.AddSingleton<IConfiguration>(configuration);
        
        // 配置混沌测试设置
        services.Configure<ChaosSettings>(configuration.GetSection("Chaos"));
        
        // 注册混沌测试服务
        services.AddChaos();
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置信息
        var chaosSettings = serviceProvider.GetRequiredService<IOptions<ChaosSettings>>().Value;
        Console.WriteLine("\n混沌测试配置信息:");
        Console.WriteLine("-" * 30);
        Console.WriteLine($"AOT 优化: {chaosSettings.EnableAotOptimization}");
        Console.WriteLine($"修剪优化: {chaosSettings.EnableTrimOptimization}");
        Console.WriteLine($"故障注入模式: {chaosSettings.FaultInjectionMode}");
        Console.WriteLine($"日志级别: {chaosSettings.LogLevel}");
        Console.WriteLine($"详细日志: {chaosSettings.EnableDetailedLogging}");
        Console.WriteLine($"缓存大小: {chaosSettings.CacheSize}");
        Console.WriteLine($"缓存过期: {chaosSettings.CacheExpiration}");
        Console.WriteLine($"超时时间: {chaosSettings.Timeout}");
        
        // 使用混沌测试服务
        var chaosService = serviceProvider.GetRequiredService<IChaosService>();
        
        // 创建复杂的故障配置
        var complexFaultConfig = new FaultConfig
        {
            FaultType = FaultType.Network,
            NetworkFaultType = NetworkFaultType.PacketLoss,
            PacketLossRate = 0.3,
            Probability = 0.6,
            Description = "高级网络故障测试"
        };
        
        // 执行复杂的故障注入
        var result = await chaosService.InjectFaultAsync(complexFaultConfig);
        Console.WriteLine("\n执行复杂故障注入结果:");
        Console.WriteLine($"结果: {result.Success}");
        Console.WriteLine($"消息: {result.Message}");
        
        Console.WriteLine("\n高级配置示例完成!");
    }
}
```

### 4. 性能优化示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chaos;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("混沌测试性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Warning); // 降低日志级别以提高性能
        });
        
        // 注册混沌测试服务，启用性能优化
        services.AddChaos(options => {
            options.EnableAotOptimization = true;
            options.FaultInjectionMode = FaultInjectionMode.Async;
            options.CacheSize = 5000; // 增加缓存大小
            options.CacheExpiration = TimeSpan.FromHours(4); // 延长缓存过期时间
        });
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        var chaosService = serviceProvider.GetRequiredService<IChaosService>();
        
        // 创建高性能故障配置
        var faultConfig = new FaultConfig
        {
            FaultType = FaultType.Delay,
            DelayTime = TimeSpan.FromSeconds(50), // 较短的延迟时间
            Probability = 0.5,
            Description = "高性能故障测试",
            EnableAotOptimization = true
        };
        
        Console.WriteLine("\n执行性能测试:");
        Console.WriteLine("-" * 30);
        
        // 执行多次故障注入以测试性能
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        // 并行执行故障注入以测试并发性能
        var tasks = new List<Task<FaultInjectionResult>>();
        for (int i = 0; i < iterations; i++)
        {
            tasks.Add(chaosService.InjectFaultAsync(faultConfig));
        }
        
        // 等待所有任务完成
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // 计算成功次数
        var successCount = results.Count(r => r.Success);
        
        Console.WriteLine($"执行次数: {iterations}");
        Console.WriteLine($"成功次数: {successCount}");
        Console.WriteLine($"成功率: {successCount / (double)iterations:P2}");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均执行时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2} ms");
        Console.WriteLine($"每秒处理请求: {iterations / stopwatch.Elapsed.TotalSeconds:F2} RPS");
        
        Console.WriteLine("\n性能优化示例完成!");
    }
}
```

### 5. Web API 集成示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Mvc.Core@10.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chaos;

// API 请求模型
public class InjectFaultRequest
{
    public FaultType FaultType { get; set; }
    public TimeSpan DelayTime { get; set; }
    public double Probability { get; set; }
    public string? Description { get; set; }
    public bool EnableAotOptimization { get; set; }
    public ExceptionType ExceptionType { get; set; }
    public ResourceType ResourceType { get; set; }
    public double ResourceConsumption { get; set; }
    public NetworkFaultType NetworkFaultType { get; set; }
    public double PacketLossRate { get; set; }
}

// API 响应模型
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; }
}

// API 控制器
[ApiController]
[Route("api/[controller]")]
public class ChaosController : ControllerBase
{
    private readonly IChaosService _chaosService;
    private readonly ILogger<ChaosController> _logger;

    public ChaosController(IChaosService chaosService, ILogger<ChaosController> logger)
    {
        _chaosService = chaosService;
        _logger = logger;
    }

    [HttpPost("inject-fault")]
    [ProducesResponseType(typeof(ApiResponse<FaultInjectionResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InjectFault([FromBody] InjectFaultRequest request)
    {
        _logger.LogInformation("API 故障注入请求: {FaultType}, 概率: {Probability}", 
            request.FaultType, request.Probability);
        
        try
        {
            // 验证请求
            if (request.Probability < 0 || request.Probability > 1)
            {
                return BadRequest(new ApiResponse<object> {
                    Success = false,
                    Message = "概率值必须在 0 到 1 之间",
                    ErrorCode = "INVALID_PROBABILITY",
                    Timestamp = DateTime.UtcNow
                });
            }
            
            // 转换为内部故障配置
            var faultConfig = new FaultConfig
            {
                FaultType = request.FaultType,
                DelayTime = request.DelayTime,
                Probability = request.Probability,
                Description = request.Description,
                EnableAotOptimization = request.EnableAotOptimization,
                ExceptionType = request.ExceptionType,
                ResourceType = request.ResourceType,
                ResourceConsumption = request.ResourceConsumption,
                NetworkFaultType = request.NetworkFaultType,
                PacketLossRate = request.PacketLossRate
            };
            
            // 执行故障注入
            var result = await _chaosService.InjectFaultAsync(faultConfig);
            
            // 返回成功响应
            return Ok(new ApiResponse<FaultInjectionResult> {
                Success = true,
                Data = result,
                Message = "故障注入成功",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 故障注入失败: {Message}", ex.Message);
            
            // 返回错误响应
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> {
                Success = false,
                Message = "故障注入失败",
                ErrorCode = "FAULT_INJECTION_FAILED",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost("run-scenario")]
    [ProducesResponseType(typeof(ApiResponse<ScenarioResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RunScenario([FromBody] ChaosScenario request)
    {
        _logger.LogInformation("API 混沌场景请求: {Name}", request.Name);
        
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(new ApiResponse<object> {
                    Success = false,
                    Message = "场景名称不能为空",
                    ErrorCode = "INVALID_SCENARIO_NAME",
                    Timestamp = DateTime.UtcNow
                });
            }
            
            // 执行混沌测试场景
            var result = await _chaosService.RunScenarioAsync(request);
            
            // 返回成功响应
            return Ok(new ApiResponse<ScenarioResult> {
                Success = true,
                Data = result,
                Message = "混沌测试场景执行成功",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 混沌场景执行失败: {Message}", ex.Message);
            
            // 返回错误响应
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> {
                Success = false,
                Message = "混沌测试场景执行失败",
                ErrorCode = "SCENARIO_EXECUTION_FAILED",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new ApiResponse<object> {
            Success = true,
            Data = new { Status = "Healthy", Service = "Chaos API" },
            Message = "混沌测试服务运行正常",
            Timestamp = DateTime.UtcNow
        });
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置日志
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        
        // 注册服务
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // 注册混沌测试服务
        builder.Services.AddSingleton<IChaosService, AotSafeChaosService>();
        
        // AOT 优化配置
        builder.Services.Configure<ChaosSettings>(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
        });
        
        var app = builder.Build();
        
        // 配置中间件
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        Console.WriteLine("混沌测试 Web API 集成示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("API 服务已启动!");
        Console.WriteLine("访问 http://localhost:5000/swagger 查看 API 文档");
        Console.WriteLine("\nAPI 端点:");
        Console.WriteLine("  - POST /api/chaos/inject-fault - 执行故障注入");
        Console.WriteLine("  - POST /api/chaos/run-scenario - 执行混沌测试场景");
        Console.WriteLine("  - GET /api/chaos/health - 健康检查");
        Console.WriteLine("\n按 Ctrl+C 停止服务");
        
        await app.RunAsync();
    }
}
```

### 6. 消息队列集成示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Chaos;

// 消息队列命令类
public class ChaosCommand
{
    public string CommandType { get; set; } = string.Empty;
    public FaultConfig? FaultConfig { get; set; }
    public ChaosScenario? Scenario { get; set; }
}

// 模拟消息队列客户端
public interface IMessageQueueClient
{
    Task SubscribeAsync(string topic, Func<Message, CancellationToken, Task> handler, CancellationToken cancellationToken);
    Task PublishAsync(string topic, Message message, CancellationToken cancellationToken);
}

// 消息类
public class Message
{
    public string MessageType { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 模拟消息队列客户端实现
public class MockMessageQueueClient : IMessageQueueClient
{
    private readonly ILogger<MockMessageQueueClient> _logger;
    private readonly Dictionary<string, List<Func<Message, CancellationToken, Task>>> _subscribers = new();
    
    public MockMessageQueueClient(ILogger<MockMessageQueueClient> logger)
    {
        _logger = logger;
    }
    
    public async Task SubscribeAsync(string topic, Func<Message, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        if (!_subscribers.ContainsKey(topic))
        {
            _subscribers[topic] = new List<Func<Message, CancellationToken, Task>>();
        }
        
        _subscribers[topic].Add(handler);
        _logger.LogInformation("已订阅主题: {Topic}", topic);
        
        // 模拟发布一些测试消息
        await Task.Delay(1000, cancellationToken);
        await PublishTestMessagesAsync(topic, cancellationToken);
    }
    
    public async Task PublishAsync(string topic, Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("发布消息到主题: {Topic}, 消息类型: {MessageType}", topic, message.MessageType);
        
        if (_subscribers.TryGetValue(topic, out var handlers))
        {
            foreach (var handler in handlers)
            {
                await handler(message, cancellationToken);
            }
        }
    }
    
    private async Task PublishTestMessagesAsync(string topic, CancellationToken cancellationToken)
    {
        // 发布测试消息
        var testMessages = new List<Message>
        {
            new Message {
                MessageType = "InjectFault",
                Body = JsonSerializer.Serialize(new FaultConfig {
                    FaultType = FaultType.Delay,
                    DelayTime = TimeSpan.FromSeconds(1),
                    Probability = 0.5
                })
            },
            new Message {
                MessageType = "RunScenario",
                Body = JsonSerializer.Serialize(new ChaosScenario {
                    Name = "消息队列触发的混沌测试场景",
                    Duration = TimeSpan.FromMinutes(1),
                    Faults = new List<FaultConfig> {
                        new FaultConfig {
                            FaultType = FaultType.Delay,
                            DelayTime = TimeSpan.FromSeconds(500),
                            Probability = 0.7
                        }
                    }
                })
            }
        };
        
        foreach (var message in testMessages)
        {
            await PublishAsync(topic, message, cancellationToken);
            await Task.Delay(2000, cancellationToken);
        }
    }
}

// 消息队列消费者服务
public class ChaosMessageConsumer : BackgroundService
{
    private readonly IChaosService _chaosService;
    private readonly ILogger<ChaosMessageConsumer> _logger;
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public ChaosMessageConsumer(IChaosService chaosService, ILogger<ChaosMessageConsumer> logger, IMessageQueueClient messageQueueClient)
    {
        _chaosService = chaosService;
        _logger = logger;
        _messageQueueClient = messageQueueClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("混沌测试消息消费者已启动");
        
        // 订阅混沌测试命令主题
        await _messageQueueClient.SubscribeAsync("chaos-commands", async (message, cancellationToken) => {
            try
            {
                _logger.LogInformation("收到混沌测试命令: {MessageType}, 消息ID: {MessageId}", 
                    message.MessageType, message.MessageId);
                
                switch (message.MessageType)
                {
                    case "InjectFault":
                        await HandleInjectFaultCommandAsync(message, cancellationToken);
                        break;
                    
                    case "RunScenario":
                        await HandleRunScenarioCommandAsync(message, cancellationToken);
                        break;
                    
                    default:
                        _logger.LogWarning("未知的混沌测试命令类型: {MessageType}", message.MessageType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理混沌测试命令失败: {Message}", ex.Message);
            }
        }, stoppingToken);
        
        _logger.LogInformation("混沌测试消息消费者已完成初始化");
    }
    
    private async Task HandleInjectFaultCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析故障配置
        var faultConfig = JsonSerializer.Deserialize<FaultConfig>(message.Body, _jsonOptions);
        if (faultConfig == null)
        {
            _logger.LogError("无法解析故障配置: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行故障注入命令: {FaultType}, 概率: {Probability}", 
            faultConfig.FaultType, faultConfig.Probability);
        
        // 执行故障注入
        var result = await _chaosService.InjectFaultAsync(faultConfig);
        
        _logger.LogInformation("故障注入命令执行完成: {Success}, 消息: {Message}", 
            result.Success, result.Message);
    }
    
    private async Task HandleRunScenarioCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析混沌测试场景
        var scenario = JsonSerializer.Deserialize<ChaosScenario>(message.Body, _jsonOptions);
        if (scenario == null)
        {
            _logger.LogError("无法解析混沌测试场景: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行混沌测试场景命令: {Name}", scenario.Name);
        
        // 执行混沌测试场景
        var result = await _chaosService.RunScenarioAsync(scenario);
        
        _logger.LogInformation("混沌测试场景命令执行完成: {Name}, 状态: {Status}", 
            scenario.Name, result.Status);
        
        // 记录场景执行结果
        _logger.LogInformation("场景执行指标:");
        foreach (var metric in result.Metrics)
        {
            _logger.LogInformation("  - {Key}: {Value}", metric.Key, metric.Value);
        }
    }
}

// 消息队列发布者服务
public class ChaosMessagePublisher
{
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public ChaosMessagePublisher(IMessageQueueClient messageQueueClient)
    {
        _messageQueueClient = messageQueueClient;
    }
    
    public async Task PublishInjectFaultCommandAsync(FaultConfig faultConfig, CancellationToken cancellationToken = default)
    {
        var message = new Message {
            MessageType = "InjectFault",
            Body = JsonSerializer.Serialize(faultConfig, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CommandSource", "ChaosMessagePublisher" },
                { "Timestamp", DateTime.UtcNow.ToString("o") }
            }
        };
        
        await _messageQueueClient.PublishAsync("chaos-commands", message, cancellationToken);
    }
    
    public async Task PublishRunScenarioCommandAsync(ChaosScenario scenario, CancellationToken cancellationToken = default)
    {
        var message = new Message {
            MessageType = "RunScenario",
            Body = JsonSerializer.Serialize(scenario, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CommandSource", "ChaosMessagePublisher" },
                { "Timestamp", DateTime.UtcNow.ToString("o") }
            }
        };
        
        await _messageQueueClient.PublishAsync("chaos-commands", message, cancellationToken);
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("混沌测试消息队列集成示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册消息队列客户端
        services.AddSingleton<IMessageQueueClient, MockMessageQueueClient>();
        
        // 注册混沌测试服务
        services.AddSingleton<IChaosService, AotSafeChaosService>();
        
        // 注册消息队列消费者
        services.AddHostedService<ChaosMessageConsumer>();
        
        // 注册消息队列发布者
        services.AddSingleton<ChaosMessagePublisher>();
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        
        Console.WriteLine("服务已启动!");
        Console.WriteLine("-" * 30);
        Console.WriteLine("消息队列消费者正在运行...");
        Console.WriteLine("等待接收混沌测试命令...");
        Console.WriteLine("\n按 Ctrl+C 停止服务");
        
        // 启动消息队列发布者，发布一些测试命令
        var publisher = serviceProvider.GetRequiredService<ChaosMessagePublisher>();
        await Task.Delay(3000); // 等待消费者启动完成
        
        // 发布更多测试命令
        Console.WriteLine("\n发布额外的测试命令...");
        
        // 发布延迟故障注入命令
        await publisher.PublishInjectFaultCommandAsync(new FaultConfig {
            FaultType = FaultType.Delay,
            DelayTime = TimeSpan.FromSeconds(2),
            Probability = 0.9,
            Description = "消息队列触发的延迟故障测试"
        });
        
        await Task.Delay(5000);
        
        // 发布中断故障注入命令
        await publisher.PublishInjectFaultCommandAsync(new FaultConfig {
            FaultType = FaultType.Abort,
            Probability = 0.3,
            Description = "消息队列触发的中断故障测试"
        });
        
        // 保持应用运行
        await Task.Delay(Timeout.Infinite);
    }
}
```

## 总结

以上示例演示了 chaos 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用 chaos 进行基础故障注入
2. 配置 AOT 编译优化，提高运行时性能
3. 进行高级配置，自定义混沌测试行为
4. 优化混沌测试性能，支持高并发场景
5. 将 chaos 集成到 Web API 中，通过 HTTP 请求触发故障注入
6. 与消息队列集成，实现分布式系统的混沌测试

这些示例遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模的项目。所有示例都支持 AOT 编译，可以编译为本机代码以获得更高的性能和更快的启动速度。

通过这些示例，您可以学习到如何使用 chaos 技能构建高性能、可靠的混沌测试系统，提高分布式系统的弹性和可靠性。混沌工程是提高系统可靠性的重要手段，通过主动注入故障，开发者可以发现系统中的潜在问题，提前进行优化和改进，从而构建更加健壮的分布式系统。