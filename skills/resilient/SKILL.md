# Resilient Agent Skill - 弹性技能

## 技能概述

基于 .NET 10 的高性能弹性技能，为 .NET 开发者提供强大的弹性功能，包括重试策略、断路器模式、限速器、超时策略等。

## 快速开始指南

### 安装依赖

在您的主应用程序的 runfile 中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Polly@7.2.4
#:package System.Threading.RateLimiting@8.0.0
#:package Microsoft.Extensions.Http.Resilience@10.0.0
```

### 注册服务

在您的主应用程序中注册弹性服务：

```csharp
// 注册弹性服务
var builder = WebApplication.CreateBuilder(args);

// 注册基础弹性服务
builder.Services.AddResilientServices();

// 注册带有配置的弹性服务
builder.Services.AddResilientServices(options => {
    options.RetryCount = 3;
    options.RetryDelay = TimeSpan.FromSeconds(1);
    options.CircuitBreakerFailureThreshold = 0.5;
    options.CircuitBreakerDurationOfBreak = TimeSpan.FromSeconds(30);
    options.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();
```

### 使用示例

```csharp
// 获取弹性服务
var resilientService = serviceProvider.GetRequiredService<IResilientService>();

// 使用重试策略
var result = await resilientService.ExecuteWithRetryAsync(async () => {
    // 可能会失败的操作，例如网络请求
    return await httpClient.GetFromJsonAsync<WeatherForecast>("https://api.example.com/weather");
});

// 使用断路器模式
var circuitResult = await resilientService.ExecuteWithCircuitBreakerAsync(async () => {
    // 调用可能不稳定的服务
    return await externalService.DoSomethingAsync();
});

// 使用限速器
var rateLimitedResult = await resilientService.ExecuteWithRateLimiterAsync(async () => {
    // 需要限制速率的操作
    return await apiClient.SendRequestAsync();
});

Console.WriteLine($"结果: {result}");
```

## 导航地图

```
resilient/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── resilient_polly_integration.cs     # 基于Polly的弹性实现
    ├── resilient_polly_integration.run.json  # 运行配置
    ├── resilient_polly_integration.setting.json  # 设置文件
    ├── rate_limiter.cs     # 限速器实现
    ├── rate_limiter.run.json  # 运行配置
    ├── rate_limiter.setting.json  # 设置文件
    ├── publicapi_integration.cs     # 公共API集成
    ├── publicapi_integration.run.json  # 运行配置
    └── publicapi_integration.setting.json  # 设置文件
```

## 主要功能

1. **重试策略**: 自动重试失败的操作，支持指数退避和自定义重试条件
2. **断路器模式**: 在服务不可用时快速失败，防止级联故障
3. **限速器**: 控制请求速率，防止系统过载
4. **超时策略**: 为操作设置超时时间，避免长时间阻塞
5. **回退策略**: 当所有尝试都失败时提供备用方案
6. **缓存策略**: 缓存频繁访问的数据，减少重复操作
7. **组合策略**: 将多种弹性策略组合使用，提供全面的弹性解决方案
8. **高性能设计**: 优化的实现，最小化开销
9. **内存优化**: 使用对象池和内存池，减少垃圾回收
10. **可扩展架构**: 支持自定义扩展和集成

## 核心 API

### IResilientService 接口

```csharp
public interface IResilientService
{
    // 使用重试策略执行操作
    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用断路器模式执行操作
    Task<T> ExecuteWithCircuitBreakerAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用限速器执行操作
    Task<T> ExecuteWithRateLimiterAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用超时策略执行操作
    Task<T> ExecuteWithTimeoutAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用回退策略执行操作
    Task<T> ExecuteWithFallbackAsync<T>(Func<Task<T>> operation, Func<Task<T>> fallback, CancellationToken cancellationToken = default);
    
    // 使用组合策略执行操作
    Task<T> ExecuteWithResilienceAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 获取健康状态
    Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
}
```

### 弹性策略配置

```csharp
public class ResilientOptions
{
    // 重试策略配置
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);
    public bool UseExponentialBackoff { get; set; } = true;
    
    // 断路器配置
    public double CircuitBreakerFailureThreshold { get; set; } = 0.5;
    public int CircuitBreakerSamplingDuration { get; set; } = 10;
    public int CircuitBreakerMinimumThroughput { get; set; } = 10;
    public TimeSpan CircuitBreakerDurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
    
    // 限速器配置
    public int RateLimiterPermitLimit { get; set; } = 100;
    public TimeSpan RateLimiterWindow { get; set; } = TimeSpan.FromSeconds(1);
    public int RateLimiterQueueLimit { get; set; } = 50;
    
    // 超时配置
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    
    // 缓存配置
    public bool EnableCaching { get; set; } = false;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public int CacheSizeLimit { get; set; } = 1000;
}
```

## 扩展说明

本技能提供了完整的弹性解决方案，您可以根据需要进行扩展：

1. **自定义策略实现**: 实现 `IResilientStrategy` 接口，创建自定义弹性策略
2. **扩展现有策略**: 继承现有策略类，修改或增强其行为
3. **集成其他系统**: 与监控、日志或追踪系统集成
4. **性能优化**: 针对特定场景优化性能
5. **自定义配置**: 创建特定于应用程序的配置选项

### 自定义策略示例

```csharp
public class CustomResilientStrategy : IResilientStrategy
{
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        // 实现自定义弹性逻辑
        for (int i = 0; i < 3; i++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                // 自定义错误处理
                if (i == 2)
                    throw;
                
                // 自定义延迟逻辑
                await Task.Delay(TimeSpan.FromMilliseconds(100 * (i + 1)), cancellationToken);
            }
        }
        
        throw new InvalidOperationException("操作失败");
    }
}

// 注册自定义策略
builder.Services.AddSingleton<IResilientStrategy, CustomResilientStrategy>();
builder.Services.AddSingleton<IResilientService, ResilientService>();
```

## 最佳实践

1. **依赖注入**: 使用依赖注入管理服务，便于测试和维护
2. **异步编程**: 优先使用异步 API，避免阻塞线程
3. **合理配置**: 根据实际场景调整弹性策略配置
4. **错误处理**: 正确处理异常情况，区分可重试和不可重试的错误
5. **日志记录**: 添加适当的日志，便于调试和监控
6. **性能监控**: 监控关键性能指标，如重试次数、断路器状态等
7. **测试策略**: 测试不同故障场景下的弹性策略行为
8. **合理使用组合策略**: 根据具体场景选择合适的策略组合
9. **内存管理**: 注意内存使用，尤其是在使用缓存策略时
10. **AOT 兼容性**: 确保自定义扩展与 AOT 编译兼容

## 性能优化建议

1. **策略重用**: 重用弹性策略实例，避免频繁创建
2. **配置优化**: 根据实际网络条件和服务特性调整重试次数和延迟
3. **断路器阈值**: 根据服务的实际可靠性设置合理的断路器阈值
4. **限速器配置**: 根据系统容量和外部服务限制设置适当的速率限制
5. **超时设置**: 根据操作的实际执行时间设置合理的超时时间
6. **内存使用**: 监控缓存大小，避免内存泄漏
7. **并发控制**: 合理控制并发请求数量，避免系统过载
8. **批量操作**: 对于多个相似操作，考虑使用批量处理减少网络往返
9. **预热策略**: 在系统启动时预热弹性策略，避免首次请求延迟
10. **监控和调优**: 持续监控系统性能，根据实际情况调整策略配置

## AOT 编译支持

本技能完全支持 .NET 10 的 AOT 编译，提供以下优势：

1. **启动速度快**: AOT 编译减少了启动时间
2. **内存占用低**: 减少了运行时需要的内存
3. **部署简单**: 支持单文件部署，易于分发
4. **安全性高**: 减少了运行时漏洞

### AOT 编译配置

在您的项目文件中添加以下配置：

```xml
<PropertyGroup>
    <PublishAot>true</PublishAot>
    <TrimMode>partial</TrimMode>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <Optimize>true</Optimize>
    <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
    <SelfContained>true</SelfContained>
</PropertyGroup>
```

## 故障排除

### 常见问题

1. **重试策略不生效**
   - 检查操作是否正确抛出可重试的异常
   - 验证重试配置是否正确
   - 检查日志，确认重试逻辑是否执行

2. **断路器一直打开**
   - 检查服务是否持续失败
   - 验证断路器配置是否合理
   - 确认故障阈值和半开状态设置

3. **限速器导致请求延迟**
   - 检查限速器配置是否过严格
   - 考虑增加速率限制或队列大小
   - 验证是否有异常多的请求导致限速

4. **内存使用过高**
   - 检查缓存大小是否合理
   - 验证对象池使用是否正确
   - 监控长时间运行的操作是否泄漏资源

5. **AOT 编译错误**
   - 确保所有依赖项支持 AOT 编译
   - 调整 `TrimMode` 为 `partial` 或 `full`
   - 检查是否使用了反射或动态类型
   - 查看编译日志，了解具体错误原因

### 诊断工具

使用以下工具帮助诊断弹性策略问题：

1. **日志分析**: 检查详细的策略执行日志
2. **性能计数器**: 监控重试次数、断路器状态等指标
3. **健康检查**: 使用健康检查端点监控系统状态
4. **分布式追踪**: 跟踪请求通过弹性策略的路径
5. **压力测试**: 模拟高负载和故障场景，测试弹性策略效果

## 总结

Resilient 技能提供了强大而灵活的弹性解决方案，基于 .NET 10 和 AOT 编译技术，具有高性能、可靠、易于使用的特点。通过本技能，您可以轻松实现各种弹性模式，提高系统的可靠性和可用性。

本技能支持多种扩展方式，您可以根据具体需求自定义实现，满足特定场景的要求。同时，我们提供了详细的文档和示例，帮助您快速上手和使用。

通过合理配置和使用 Resilient 技能，您可以构建出更加健壮、可靠的分布式系统，提高应用程序的弹性和容错能力。
