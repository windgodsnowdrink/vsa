# chaos - 参考文档

## 概述

chaos 是基于 .NET 10 构建的高性能混沌测试系统，为 .NET 开发者提供强大的混沌工程和故障注入功能，支持 AOT（提前编译）编译，适用于各种规模的分布式系统测试。

## 核心组件

### 1. 混沌测试服务 (IChaosService)

- **位置**: scripts/chaos_integration.cs
- **功能**: 混沌测试的核心业务逻辑处理
- **特性**: 
  - 支持多种故障类型的注入
  - 混沌测试场景的管理和执行
  - 实时监控和报告生成
  - 高性能设计，支持高并发场景
  - 完善的错误处理和日志记录
  - AOT 编译优化支持
  - 异步编程模型
- **使用示例**: 
  ```csharp
  public class ChaosService : IChaosService
  {
      private readonly ILogger<ChaosService> _logger;
      private readonly ChaosSettings _settings;
      private readonly List<FaultConfig> _activeFaults = new();
      
      public ChaosService(ILogger<ChaosService> logger, IOptions<ChaosSettings> settings)
      {
          _logger = logger;
          _settings = settings.Value;
      }
      
      public async Task<FaultInjectionResult> InjectFaultAsync(FaultConfig faultConfig)
      {
          _logger.LogInformation("执行故障注入: {FaultType}, 概率: {Probability}", 
              faultConfig.FaultType, faultConfig.Probability);
          
          // 故障注入逻辑
          var result = await ExecuteFaultInjectionAsync(faultConfig);
          
          _logger.LogInformation("故障注入完成: {Result}", result);
          return result;
      }
      
      public async Task<ScenarioResult> RunScenarioAsync(ChaosScenario scenario)
      {
          _logger.LogInformation("开始混沌测试场景: {Name}", scenario.Name);
          
          // 场景执行逻辑
          var result = await ExecuteScenarioAsync(scenario);
          
          _logger.LogInformation("混沌测试场景完成: {Name}, 状态: {Status}", 
              scenario.Name, result.Status);
          return result;
      }
      
      private async Task<FaultInjectionResult> ExecuteFaultInjectionAsync(FaultConfig faultConfig)
      {
          // 实现故障注入逻辑
          await Task.Delay(100);
          return new FaultInjectionResult { Success = true, Message = "故障注入成功" };
      }
      
      private async Task<ScenarioResult> ExecuteScenarioAsync(ChaosScenario scenario)
      {
          // 实现场景执行逻辑
          await Task.Delay(500);
          return new ScenarioResult { Status = ScenarioStatus.Completed, Metrics = new Dictionary<string, object>() };
      }
  }
  ```

### 2. 故障配置 (FaultConfig)

- **功能**: 定义故障注入的配置参数
- **特性**: 
  - 支持多种故障类型
  - 精细的概率控制
  - AOT 优化开关
  - 目标服务指定
  - 延迟时间配置
- **使用示例**: 
  ```csharp
  public class FaultConfig
  {
      public FaultType FaultType { get; set; }
      public double Probability { get; set; } = 0.5;
      public string? TargetService { get; set; }
      public TimeSpan DelayTime { get; set; } = TimeSpan.Zero;
      public ExceptionType ExceptionType { get; set; } = ExceptionType.General;
      public ResourceType ResourceType { get; set; } = ResourceType.CPU;
      public double ResourceConsumption { get; set; } = 0.8;
      public NetworkFaultType NetworkFaultType { get; set; } = NetworkFaultType.Latency;
      public double PacketLossRate { get; set; } = 0.1;
      public bool EnableAotOptimization { get; set; } = false;
  }
  ```

### 3. 混沌测试场景 (ChaosScenario)

- **功能**: 定义复杂的混沌测试场景
- **特性**: 
  - 支持多个故障的组合
  - 持续时间控制
  - 指标监控配置
  - 场景描述和命名
- **使用示例**: 
  ```csharp
  public class ChaosScenario
  {
      public string Name { get; set; } = string.Empty;
      public string Description { get; set; } = string.Empty;
      public List<FaultConfig> Faults { get; set; } = new();
      public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(5);
      public List<string> MetricsToMonitor { get; set; } = new();
      public Dictionary<string, object> Parameters { get; set; } = new();
  }
  ```

### 4. 混沌测试设置 (ChaosSettings)

- **功能**: 全局混沌测试配置
- **特性**: 
  - AOT 优化配置
  - 故障注入模式
  - 日志级别配置
  - 缓存设置
- **使用示例**: 
  ```csharp
  public class ChaosSettings
  {
      public bool EnableAotOptimization { get; set; } = false;
      public bool EnableTrimOptimization { get; set; } = false;
      public FaultInjectionMode FaultInjectionMode { get; set; } = FaultInjectionMode.Async;
      public LogLevel LogLevel { get; set; } = LogLevel.Information;
      public bool EnableDetailedLogging { get; set; } = false;
      public int CacheSize { get; set; } = 1000;
      public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
  }
  ```

## 使用示例

### 基础使用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Chaos;

// 配置服务
var services = new ServiceCollection();
services.AddChaos();

// 构建服务提供器
var serviceProvider = services.BuildServiceProvider();

// 获取混沌测试服务
var chaosService = serviceProvider.GetRequiredService<IChaosService>();

// 创建故障配置
var faultConfig = new FaultConfig
{
    FaultType = FaultType.Delay,
    DelayTime = TimeSpan.FromSeconds(2),
    Probability = 0.5
};

// 执行故障注入
Console.WriteLine("开始执行延迟故障注入...");
var result = await chaosService.InjectFaultAsync(faultConfig);
Console.WriteLine($"故障注入结果: {result.Success}");
Console.WriteLine($"结果消息: {result.Message}");

// 其他操作...
```

### AOT 编译优化示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
using System;
using Microsoft.Extensions.DependencyInjection;
using Chaos;

// AOT 安全的混沌服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeChaosService : IChaosService
{
    public async Task<FaultInjectionResult> InjectFaultAsync(FaultConfig faultConfig)
    {
        // AOT 安全的故障注入逻辑
        await Task.Delay(100);
        return new FaultInjectionResult { Success = true, Message = "AOT 优化的故障注入成功" };
    }
    
    public async Task<ScenarioResult> RunScenarioAsync(ChaosScenario scenario)
    {
        // AOT 安全的场景执行逻辑
        await Task.Delay(500);
        return new ScenarioResult { Status = ScenarioStatus.Completed };
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
        
        // 注册 AOT 安全的混沌服务
        services.AddSingleton<IChaosService, AotSafeChaosService>();
        
        // 配置 AOT 优化
        services.Configure<ChaosSettings>(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var chaosService = serviceProvider.GetRequiredService<IChaosService>();
        
        // 创建 AOT 优化的故障配置
        var faultConfig = new FaultConfig
        {
            FaultType = FaultType.Delay,
            DelayTime = TimeSpan.FromSeconds(1),
            Probability = 0.3,
            EnableAotOptimization = true
        };
        
        // 执行 AOT 优化的故障注入
        var result = await chaosService.InjectFaultAsync(faultConfig);
        Console.WriteLine($"AOT 优化的故障注入结果: {result.Success}");
        Console.WriteLine($"结果消息: {result.Message}");
    }
}
```

### 高级配置

```csharp
// 配置混沌测试服务
builder.Services.AddChaos(options =>
{
    // 配置 AOT 优化
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
    
    // 配置故障注入模式
    options.FaultInjectionMode = FaultInjectionMode.Async;
    
    // 配置日志
    options.LogLevel = LogLevel.Information;
    options.EnableDetailedLogging = true;
    
    // 配置缓存
    options.CacheSize = 2000;
    options.CacheExpiration = TimeSpan.FromHours(2);
    
    // 配置超时
    options.Timeout = TimeSpan.FromSeconds(60);
});

// 配置应用
var app = builder.Build();

// 使用混沌测试中间件
app.UseChaosMiddleware();

await app.RunAsync();
```

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
</ItemGroup>
```

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的 chaos 版本和依赖库支持 AOT 编译
2. **避免反射**: 避免在故障注入逻辑中使用反射
3. **资源处理**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用值类型**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试
7. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库
8. **使用 DynamicallyAccessedMembers 特性**: 为 AOT 编译提供必要的类型信息

## 配置选项

### chaos 配置

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| EnableAotOptimization | bool | false | 启用 AOT 优化 |
| EnableTrimOptimization | bool | false | 启用修剪优化 |
| FaultInjectionMode | enum | Async | 故障注入模式（Sync/Async） |
| LogLevel | LogLevel | Information | 日志级别 |
| EnableDetailedLogging | bool | false | 启用详细日志记录 |
| CacheSize | int | 1000 | 缓存大小 |
| CacheExpiration | TimeSpan | 01:00:00 | 缓存过期时间 |
| Timeout | TimeSpan | 00:00:30 | 操作超时时间 |

### 应用配置示例

```json
{
  "Chaos": {
    "EnableAotOptimization": true,
    "EnableTrimOptimization": true,
    "FaultInjectionMode": "Async",
    "LogLevel": "Information",
    "EnableDetailedLogging": false,
    "CacheSize": 2000,
    "CacheExpiration": "02:00:00",
    "Timeout": "00:01:00"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Chaos": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化故障注入逻辑**: 简化故障注入逻辑，减少性能开销
4. **使用缓存**: 对频繁使用的配置和场景进行缓存
5. **限制故障注入范围**: 只对必要的组件进行故障注入
6. **使用高效的随机数生成**: 对于概率性故障，使用高效的随机数生成器
7. **批量处理**: 对多个故障注入请求进行批量处理
8. **连接池管理**: 对于分布式系统，优化连接池配置
9. **监控性能**: 使用 OpenTelemetry 等工具监控混沌测试性能
10. **调整概率设置**: 根据系统负载调整故障注入概率

## 故障排除

### 常见问题

1. **故障注入失败**
   - 检查故障配置是否正确
   - 验证服务注册是否完整
   - 查看详细日志
   - 检查权限设置

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode
   - 使用 AOT 分析工具检查问题
   - 添加必要的 DynamicallyAccessedMembers 特性

3. **性能问题**
   - 启用 AOT 编译
   - 优化故障注入逻辑
   - 减少故障注入频率
   - 增加资源限制
   - 调整缓存设置
   - 使用异步 API

4. **测试结果不准确**
   - 检查故障配置概率设置
   - 确保测试环境稳定
   - 增加测试样本数量
   - 检查监控指标配置

5. **服务连接问题**
   - 检查网络连接
   - 验证服务地址和端口
   - 检查防火墙设置
   - 查看详细日志

## 扩展开发

### 创建自定义故障类型

```csharp
// 自定义故障类型枚举扩展
enum CustomFaultType
{
    DatabaseTimeout,
    MessageLoss,
    SecurityBreach
}

// 自定义故障实现
public class CustomFaultHandler : IFaultHandler
{
    private readonly ILogger<CustomFaultHandler> _logger;
    
    public CustomFaultHandler(ILogger<CustomFaultHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task<FaultInjectionResult> HandleAsync(FaultConfig faultConfig)
    {
        _logger.LogInformation("处理自定义故障: {FaultType}", faultConfig.FaultType);
        
        // 实现自定义故障逻辑
        await Task.Delay(200);
        
        return new FaultInjectionResult { Success = true, Message = "自定义故障处理成功" };
    }
    
    public bool CanHandle(FaultType faultType)
    {
        // 检查是否可以处理该故障类型
        return faultType == (FaultType)CustomFaultType.DatabaseTimeout;
    }
}

// 注册自定义故障处理器
builder.Services.AddSingleton<IFaultHandler, CustomFaultHandler>();
builder.Services.AddChaos();
```

### 创建自定义混沌测试场景

```csharp
// 自定义混沌测试场景
public class CustomChaosScenario : ChaosScenario
{
    public CustomChaosScenario()
    {
        Name = "自定义混沌测试场景";
        Description = "包含多种故障类型的复杂测试场景";
        
        // 添加故障配置
        Faults = new List<FaultConfig>
        {
            new FaultConfig {
                FaultType = FaultType.Delay,
                DelayTime = TimeSpan.FromSeconds(1),
                Probability = 0.3
            },
            new FaultConfig {
                FaultType = FaultType.Abort,
                Probability = 0.1
            },
            new FaultConfig {
                FaultType = FaultType.Exception,
                ExceptionType = ExceptionType.General,
                Probability = 0.2
            }
        };
        
        Duration = TimeSpan.FromMinutes(10);
        MetricsToMonitor = new List<string> { "response_time", "error_rate", "throughput" };
    }
    
    // 自定义场景执行逻辑
    public async Task<ScenarioResult> ExecuteAsync(IChaosService chaosService)
    {
        // 实现自定义场景执行逻辑
        var result = await chaosService.RunScenarioAsync(this);
        
        // 添加自定义处理
        result.Metrics["custom_metric"] = 123;
        
        return result;
    }
}

// 使用自定义场景
var customScenario = new CustomChaosScenario();
var result = await customScenario.ExecuteAsync(chaosService);
```

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
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
    [ProducesResponseType(typeof(FaultInjectionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InjectFault([FromBody] FaultConfig request)
    {
        _logger.LogInformation("API 故障注入请求: {FaultType}", request.FaultType);
        
        try
        {
            var result = await _chaosService.InjectFaultAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 故障注入失败: {Message}", ex.Message);
            return BadRequest(new ErrorResponse {
                Success = false,
                Message = "故障注入失败",
                ErrorCode = "FAULT_INJECTION_FAILED",
                Details = ex.Message
            });
        }
    }

    [HttpPost("run-scenario")]
    [ProducesResponseType(typeof(ScenarioResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RunScenario([FromBody] ChaosScenario request)
    {
        _logger.LogInformation("API 混沌场景请求: {Name}", request.Name);
        
        try
        {
            var result = await _chaosService.RunScenarioAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 混沌场景失败: {Message}", ex.Message);
            return BadRequest(new ErrorResponse {
                Success = false,
                Message = "混沌场景执行失败",
                ErrorCode = "SCENARIO_EXECUTION_FAILED",
                Details = ex.Message
            });
        }
    }
}
```

### 与消息队列集成

```csharp
// 消息队列消费者
public class ChaosMessageConsumer : BackgroundService
{
    private readonly IChaosService _chaosService;
    private readonly ILogger<ChaosMessageConsumer> _logger;
    private readonly IMessageQueueClient _messageQueueClient;

    public ChaosMessageConsumer(IChaosService chaosService, ILogger<ChaosMessageConsumer> logger, IMessageQueueClient messageQueueClient)
    {
        _chaosService = chaosService;
        _logger = logger;
        _messageQueueClient = messageQueueClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("混沌测试消息消费者已启动");
        
        await _messageQueueClient.SubscribeAsync("chaos-commands", async (message) => {
            try
            {
                _logger.LogInformation("收到混沌测试命令: {MessageType}", message.MessageType);
                
                switch (message.MessageType)
                {
                    case "InjectFault":
                        var faultConfig = JsonSerializer.Deserialize<FaultConfig>(message.Body);
                        await _chaosService.InjectFaultAsync(faultConfig);
                        break;
                    
                    case "RunScenario":
                        var scenario = JsonSerializer.Deserialize<ChaosScenario>(message.Body);
                        await _chaosService.RunScenarioAsync(scenario);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理混沌测试命令失败: {Message}", ex.Message);
            }
        }, stoppingToken);
    }
}

// 注册消息消费者
builder.Services.AddHostedService<ChaosMessageConsumer>();
```

## 最佳实践

1. **从简单开始**: 从简单的故障类型和场景开始，逐步增加复杂性
2. **定义明确的目标**: 为每个混沌测试场景定义明确的目标和衡量标准
3. **在非生产环境测试**: 先在测试环境进行混沌测试，再逐步扩展到生产环境
4. **监控关键指标**: 测试期间监控系统的关键指标，如响应时间、错误率、吞吐量等
5. **制定恢复计划**: 确保有明确的故障恢复计划
6. **使用自动化工具**: 结合 CI/CD 流水线，实现混沌测试的自动化
7. **持续改进**: 根据测试结果持续改进系统的弹性设计
8. **与团队协作**: 混沌测试应该是团队协作的过程，包括开发、测试、运维等角色
9. **使用 AOT 编译**: 对于性能敏感场景，考虑使用 AOT 编译提高性能
10. **安全第一**: 确保混沌测试不会对生产系统造成不可恢复的损坏

## 混沌测试场景设计原则

1. **真实性**: 模拟真实世界中可能发生的故障
2. **可控性**: 能够精确控制故障的类型、强度和持续时间
3. **可观测性**: 能够监控和记录测试过程中的系统行为
4. **可重复性**: 能够重复执行相同的测试场景，验证修复效果
5. **渐进性**: 从轻微故障开始，逐步增加故障强度
6. **全面性**: 覆盖系统的各个层面，包括网络、服务、数据库、应用等
7. **安全性**: 确保测试不会导致数据丢失或系统崩溃
8. **可扩展性**: 能够轻松添加新的故障类型和场景

## 总结

chaos 是一个基于 .NET 10 的现代化混沌测试框架，具有高性能、模块化、可扩展等特点。通过 chaos，开发者可以快速构建弹性、可靠的分布式系统，提高系统的容错能力和恢复能力。

本参考文档提供了 chaos 的核心组件、使用示例、配置选项、性能优化建议、故障排除指南和扩展开发方法，帮助开发者充分利用 chaos 框架的优势，构建高质量的混沌测试系统。

chaos 支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。通过遵循本文档中的最佳实践和 AOT 兼容性建议，开发者可以构建出高性能、可靠的混沌测试系统，提高分布式系统的弹性和可靠性。

混沌工程是提高系统可靠性的重要手段，通过主动注入故障，开发者可以发现系统中的潜在问题，提前进行优化和改进，从而构建更加健壮的分布式系统。