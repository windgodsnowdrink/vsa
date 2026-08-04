# chaos Agent Skill - 混沌测试技能

## 技能概述

基于 .NET 10 的高性能混沌测试技能，为 .NET 开发者提供强大的混沌工程和故障注入功能，支持 AOT（提前编译）编译，适用于构建弹性、可靠的分布式系统。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
```

### 注册服务

在您的主应用程序中注册 chaos 服务：

```csharp
// 配置应用程序
var builder = WebApplication.CreateBuilder(args);

// 注册 chaos 服务
builder.Services.AddChaos();

// AOT 优化配置
builder.Services.Configure<ChaosSettings>(options => {
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
    options.FaultInjectionMode = FaultInjectionMode.Async;
});

var app = builder.Build();
```

### 使用示例

```csharp
// 获取 chaos 服务
var chaosService = serviceProvider.GetRequiredService<IChaosService>();

// 创建故障注入配置
var faultConfig = new FaultConfig
{
    FaultType = FaultType.Delay,
    DelayTime = TimeSpan.FromSeconds(5),
    Probability = 0.5,
    EnableAotOptimization = true
};

// 执行故障注入
var result = await chaosService.InjectFaultAsync(faultConfig);
Console.WriteLine($"故障注入结果: {result}");

// 创建混沌测试场景
var scenario = new ChaosScenario
{
    Name = "服务中断测试",
    Description = "测试系统在服务中断情况下的表现",
    Faults = new List<FaultConfig>
    {
        new FaultConfig { FaultType = FaultType.Abort, Probability = 0.3 },
        new FaultConfig { FaultType = FaultType.Delay, DelayTime = TimeSpan.FromSeconds(3), Probability = 0.5 }
    },
    Duration = TimeSpan.FromMinutes(5)
};

// 运行混沌测试场景
var scenarioResult = await chaosService.RunScenarioAsync(scenario);
Console.WriteLine($"混沌测试场景结果: {scenarioResult.Status}");
```

## 导航地图

```
chaos/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── chaos_integration.cs           # chaos 集成示例
    ├── chaos_integration.run.json      # 运行配置
    └── chaos_integration.setting.json  # 设置文件
```

## 主要功能

1. **现代化混沌测试框架**: 基于 .NET 10 构建的轻量级、高性能混沌测试框架
2. **AOT 编译支持**: 支持将应用编译为本机代码，提高运行时性能和启动速度
3. **多种故障类型**: 支持延迟、中断、异常、资源耗尽等多种故障类型
4. **精细的故障控制**: 支持按概率、时间段、请求类型等条件进行故障注入
5. **混沌测试场景管理**: 支持创建、运行和监控复杂的混沌测试场景
6. **实时监控和报告**: 提供实时监控和详细的测试报告
7. **异步编程模型**: 基于异步/等待模式，提高并发处理能力
8. **依赖注入**: 原生支持 .NET 依赖注入容器
9. **模块化设计**: 便于扩展和定制功能
10. **与分布式系统集成**: 支持与各种分布式系统架构集成

## 故障类型

### 1. 延迟故障 (Delay)

- **功能**: 模拟网络延迟或服务响应延迟
- **配置选项**: 延迟时间、延迟分布、概率
- **使用场景**: 测试系统在高延迟情况下的表现

### 2. 中断故障 (Abort)

- **功能**: 模拟服务中断或崩溃
- **配置选项**: 概率、异常类型
- **使用场景**: 测试系统的容错能力和恢复机制

### 3. 异常故障 (Exception)

- **功能**: 抛出指定类型的异常
- **配置选项**: 异常类型、异常消息、概率
- **使用场景**: 测试系统的异常处理机制

### 4. 资源耗尽故障 (ResourceExhaustion)

- **功能**: 模拟 CPU、内存、磁盘等资源耗尽
- **配置选项**: 资源类型、消耗程度、持续时间
- **使用场景**: 测试系统在资源受限情况下的表现

### 5. 网络故障 (Network)

- **功能**: 模拟网络丢包、延迟、断连等
- **配置选项**: 故障类型、概率、参数
- **使用场景**: 测试分布式系统的网络弹性

### 6. 状态故障 (StateCorruption)

- **功能**: 模拟数据损坏或状态不一致
- **配置选项**: 影响范围、概率
- **使用场景**: 测试系统的数据一致性和恢复能力

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

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
[ApiController]
[Route("api/[controller]")]
public class ChaosController : ControllerBase
{
    private readonly IChaosService _chaosService;

    public ChaosController(IChaosService chaosService)
    {
        _chaosService = chaosService;
    }

    [HttpPost("inject-fault")]
    public async Task<IActionResult> InjectFault([FromBody] FaultConfig request)
    {
        var result = await _chaosService.InjectFaultAsync(request);
        return Ok(new { Result = result });
    }

    [HttpPost("run-scenario")]
    public async Task<IActionResult> RunScenario([FromBody] ChaosScenario request)
    {
        var result = await _chaosService.RunScenarioAsync(request);
        return Ok(new { Result = result });
    }
}
```

### 与 gRPC 集成

```csharp
// gRPC 服务实现
public class ChaosServiceImpl : ChaosService.ChaosServiceBase
{
    private readonly IChaosService _chaosService;

    public ChaosServiceImpl(IChaosService chaosService)
    {
        _chaosService = chaosService;
    }

    public override async Task<InjectFaultResponse> InjectFault(InjectFaultRequest request, ServerCallContext context)
    {
        var faultConfig = MapToFaultConfig(request);
        var result = await _chaosService.InjectFaultAsync(faultConfig);
        return new InjectFaultResponse { Result = result.ToString() };
    }
}

// 配置 gRPC 服务
builder.Services.AddGrpc();
builder.Services.AddSingleton<ChaosServiceImpl>();

// 映射 gRPC 端点
app.MapGrpcService<ChaosServiceImpl>();
```

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化故障注入逻辑**: 简化故障注入逻辑，减少性能开销
4. **使用缓存**: 对频繁使用的配置和场景进行缓存
5. **限制故障注入范围**: 只对必要的组件进行故障注入
6. **使用高效的随机数生成**: 对于概率性故障，使用高效的随机数生成器
7. **监控性能**: 使用 OpenTelemetry 等工具监控混沌测试性能

## 故障排除

### 常见问题

1. **故障注入失败**
   - 检查故障配置是否正确
   - 验证服务注册是否完整
   - 查看详细日志

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode

3. **性能问题**
   - 启用 AOT 编译
   - 优化故障注入逻辑
   - 减少故障注入频率
   - 增加资源限制

4. **测试结果不准确**
   - 检查故障配置概率设置
   - 确保测试环境稳定
   - 增加测试样本数量

## 最佳实践

1. **从简单开始**: 从简单的故障类型和场景开始，逐步增加复杂性
2. **定义明确的目标**: 为每个混沌测试场景定义明确的目标和衡量标准
3. **在非生产环境测试**: 先在测试环境进行混沌测试，再逐步扩展到生产环境
4. **监控关键指标**: 测试期间监控系统的关键指标，如响应时间、错误率、吞吐量等
5. **制定恢复计划**: 确保有明确的故障恢复计划
6. **使用自动化工具**: 结合 CI/CD 流水线，实现混沌测试的自动化
7. **持续改进**: 根据测试结果持续改进系统的弹性设计
8. **与团队协作**: 混沌测试应该是团队协作的过程，包括开发、测试、运维等角色

## 混沌测试场景示例

### 1. 服务中断测试

```csharp
// 服务中断测试场景
var scenario = new ChaosScenario
{
    Name = "服务中断测试",
    Description = "测试系统在服务中断情况下的表现",
    Faults = new List<FaultConfig>
    {
        new FaultConfig {
            FaultType = FaultType.Abort,
            Probability = 0.3,
            TargetService = "PaymentService",
            EnableAotOptimization = true
        },
        new FaultConfig {
            FaultType = FaultType.Delay,
            DelayTime = TimeSpan.FromSeconds(3),
            Probability = 0.5,
            TargetService = "OrderService"
        }
    },
    Duration = TimeSpan.FromMinutes(5),
    MetricsToMonitor = new List<string> { "response_time", "error_rate", "throughput" }
};
```

### 2. 网络故障测试

```csharp
// 网络故障测试场景
var scenario = new ChaosScenario
{
    Name = "网络故障测试",
    Description = "测试系统在网络故障情况下的表现",
    Faults = new List<FaultConfig>
    {
        new FaultConfig {
            FaultType = FaultType.Network,
            NetworkFaultType = NetworkFaultType.PacketLoss,
            PacketLossRate = 0.2,
            Probability = 0.4,
            EnableAotOptimization = true
        },
        new FaultConfig {
            FaultType = FaultType.Network,
            NetworkFaultType = NetworkFaultType.Latency,
            DelayTime = TimeSpan.FromSeconds(2),
            Probability = 0.6
        }
    },
    Duration = TimeSpan.FromMinutes(10)
};
```

## 总结

chaos 技能提供了一套完整的现代化混沌测试解决方案，基于 .NET 10 构建，支持 AOT 编译，具有高性能、模块化、可扩展等特点。通过 chaos，开发者可以快速构建弹性、可靠的分布式系统，提高系统的容错能力和恢复能力。

该技能遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持与多种系统集成，如 Web API、gRPC 等。同时，提供了详细的性能优化建议和故障排除指南，帮助开发者构建高质量的混沌测试系统。

混沌工程是提高系统可靠性的重要手段，通过主动注入故障，开发者可以发现系统中的潜在问题，提前进行优化和改进，从而构建更加健壮的分布式系统。