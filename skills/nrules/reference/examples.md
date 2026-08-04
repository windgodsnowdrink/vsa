# NRules 技能使用示例

## 概述

本文档提供了 NRules 技能的各种使用示例，包括基本用法、高级配置、性能优化、错误处理等。这些示例旨在帮助您快速上手 NRules 规则引擎，并了解其在不同场景下的应用。

## 示例 1: 基本使用

### 功能说明

演示 NRules 的基本使用方法，包括服务注册、规则定义、事实插入和规则执行。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

// 订单类
public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Discount { get; set; }
}

// 订单状态枚举
public enum OrderStatus
{
    New,
    Processed,
    Completed
}

// 规则基类
public abstract class Rule
{
    public abstract void Define();
}

// 高价值订单规则
public class HighValueOrderRule : Rule
{
    public override void Define()
    {
        // 规则定义
    }
}

// 规则引擎服务接口
public interface IRuleEngineService
{
    IRuleSession CreateSession();
}

// 规则会话接口
public interface IRuleSession
{
    void Insert(object fact);
    void Fire();
}

// 规则仓储接口
public interface IRuleRepository
{
    void RegisterRulesFromAssembly(System.Reflection.Assembly assembly);
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services)
    {
        // 简化实现，实际项目中应该注册具体实现
        services.AddSingleton<IRuleEngineService, MockRuleEngineService>();
        services.AddSingleton<IRuleRepository, MockRuleRepository>();
        return services;
    }
}

// 模拟实现
public class MockRuleEngineService : IRuleEngineService
{
    public IRuleSession CreateSession() => new MockRuleSession();
}

public class MockRuleSession : IRuleSession
{
    public void Insert(object fact)
    {
        Console.WriteLine($"插入事实: {fact.GetType().Name}");
    }

    public void Fire()
    {
        Console.WriteLine("执行规则");
    }
}

public class MockRuleRepository : IRuleRepository
{
    public void RegisterRulesFromAssembly(System.Reflection.Assembly assembly)
    {
        Console.WriteLine($"注册规则从程序集: {assembly.GetName().Name}");
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("NRules 基本使用示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 NRules 服务
        services.AddNRulesServices();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();
        var ruleRepository = serviceProvider.GetRequiredService<IRuleRepository>();

        // 注册规则
        ruleRepository.RegisterRulesFromAssembly(typeof(HighValueOrderRule).Assembly);

        // 创建订单
        var order = new Order { Id = 1, Amount = 6000, Status = OrderStatus.New, Discount = 0 };
        Console.WriteLine($"原始订单: ID={order.Id}, 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}");

        // 创建规则会话
        var session = ruleEngineService.CreateSession();

        // 插入事实
        session.Insert(order);

        // 执行规则
        session.Fire();

        // 查看结果
        Console.WriteLine($"执行后: ID={order.Id}, 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}");

        Console.WriteLine("示例执行完成！");
    }
}
```

## 示例 2: 高级配置

### 功能说明

演示 NRules 的高级配置选项，包括缓存设置、超时设置、详细日志等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

// 配置选项
public class NRulesOptions
{
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDetailedLogging { get; set; } = false;
    public string DefaultTimeZone { get; set; } = "UTC";
}

// 规则引擎服务
public class RuleEngineService : IRuleEngineService
{
    private readonly NRulesOptions _options;
    private readonly ILogger<RuleEngineService> _logger;

    public RuleEngineService(IOptions<NRulesOptions> options, ILogger<RuleEngineService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _logger.LogInformation("RuleEngineService 初始化完成");
        _logger.LogInformation($"缓存启用: {_options.EnableCache}");
        _logger.LogInformation($"缓存大小: {_options.CacheSize}");
        _logger.LogInformation($"超时设置: {_options.Timeout}");
        _logger.LogInformation($"详细日志: {_options.EnableDetailedLogging}");
        _logger.LogInformation($"默认时区: {_options.DefaultTimeZone}");
    }

    public IRuleSession CreateSession()
    {
        return new RuleSession(_options, _logger);
    }
}

// 规则会话
public class RuleSession : IRuleSession
{
    private readonly NRulesOptions _options;
    private readonly ILogger<RuleEngineService> _logger;

    public RuleSession(NRulesOptions options, ILogger<RuleEngineService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public void Insert(object fact)
    {
        if (_options.EnableDetailedLogging)
        {
            _logger.LogDebug($"插入事实: {fact.GetType().Name}");
        }
    }

    public void Fire()
    {
        _logger.LogInformation("执行规则");
    }
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services, Action<NRulesOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<NRulesOptions>(options => { });
        }

        services.AddSingleton<IRuleEngineService, RuleEngineService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("NRules 高级配置示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        // 注册 NRules 服务并配置选项
        services.AddNRulesServices(options =>
        {
            options.EnableCache = true;
            options.CacheSize = 5000;
            options.Timeout = TimeSpan.FromMinutes(1);
            options.EnableDetailedLogging = true;
            options.DefaultTimeZone = "Asia/Shanghai";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();

        // 创建会话
        var session = ruleEngineService.CreateSession();

        // 插入事实并执行规则
        session.Insert(new { Id = 1, Name = "Test" });
        session.Fire();

        Console.WriteLine("示例执行完成！");
    }
}
```

## 示例 3: 性能优化

### 功能说明

演示 NRules 的性能优化技术，包括规则缓存、批处理和内存优化。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Threading.Channels@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

// 性能优化的规则引擎服务
public class PerformanceOptimizedRuleEngineService : IRuleEngineService
{
    private readonly ILogger<PerformanceOptimizedRuleEngineService> _logger;
    private readonly Dictionary<Type, object> _ruleCache = new Dictionary<Type, object>();

    public PerformanceOptimizedRuleEngineService(ILogger<PerformanceOptimizedRuleEngineService> logger)
    {
        _logger = logger;
    }

    public IRuleSession CreateSession()
    {
        return new PerformanceOptimizedRuleSession(_ruleCache, _logger);
    }
}

// 性能优化的规则会话
public class PerformanceOptimizedRuleSession : IRuleSession
{
    private readonly Dictionary<Type, object> _ruleCache;
    private readonly List<object> _facts = new List<object>();
    private readonly ILogger<PerformanceOptimizedRuleEngineService> _logger;

    public PerformanceOptimizedRuleSession(Dictionary<Type, object> ruleCache, ILogger<PerformanceOptimizedRuleEngineService> logger)
    {
        _ruleCache = ruleCache;
        _logger = logger;
    }

    public void Insert(object fact)
    {
        _facts.Add(fact);
    }

    public void Fire()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation($"开始执行规则，事实数量: {_facts.Count}");

        // 批处理事实
        ProcessFactsInBatches();

        stopwatch.Stop();
        _logger.LogInformation($"规则执行完成，耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
    }

    private void ProcessFactsInBatches()
    {
        const int batchSize = 100;
        for (int i = 0; i < _facts.Count; i += batchSize)
        {
            var batch = _facts.GetRange(i, Math.Min(batchSize, _facts.Count - i));
            ProcessBatch(batch);
        }
    }

    private void ProcessBatch(List<object> batch)
    {
        // 模拟批处理
        _logger.LogDebug($"处理批处理: {batch.Count} 个事实");
    }
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services)
    {
        services.AddSingleton<IRuleEngineService, PerformanceOptimizedRuleEngineService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("NRules 性能优化示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 NRules 服务
        services.AddNRulesServices();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();

        // 性能测试
        await RunPerformanceTest(ruleEngineService);

        Console.WriteLine("示例执行完成！");
    }

    private static async Task RunPerformanceTest(IRuleEngineService ruleEngineService)
    {
        const int iterations = 1000;
        const int factsPerIteration = 10;

        Console.WriteLine($"性能测试: {iterations} 次迭代，每次 {factsPerIteration} 个事实");

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            var session = ruleEngineService.CreateSession();

            // 插入多个事实
            for (int j = 0; j < factsPerIteration; j++)
            {
                session.Insert(new { Id = i * 100 + j, Value = j });
            }

            // 执行规则
            session.Fire();

            // 模拟异步操作
            await Task.Delay(1);
        }

        stopwatch.Stop();
        Console.WriteLine($"性能测试完成，总耗时: {stopwatch.Elapsed.TotalSeconds:F3} s");
        Console.WriteLine($"平均每次迭代: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
}
```

## 示例 4: 错误处理

### 功能说明

演示 NRules 的错误处理机制，包括规则执行异常处理、规则定义验证等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

// 错误处理的规则引擎服务
public class ErrorHandlingRuleEngineService : IRuleEngineService
{
    private readonly ILogger<ErrorHandlingRuleEngineService> _logger;

    public ErrorHandlingRuleEngineService(ILogger<ErrorHandlingRuleEngineService> logger)
    {
        _logger = logger;
    }

    public IRuleSession CreateSession()
    {
        return new ErrorHandlingRuleSession(_logger);
    }
}

// 错误处理的规则会话
public class ErrorHandlingRuleSession : IRuleSession
{
    private readonly ILogger<ErrorHandlingRuleEngineService> _logger;

    public ErrorHandlingRuleSession(ILogger<ErrorHandlingRuleEngineService> logger)
    {
        _logger = logger;
    }

    public void Insert(object fact)
    {
        if (fact == null)
        {
            throw new ArgumentNullException(nameof(fact), "事实不能为 null");
        }
        _logger.LogInformation($"插入事实: {fact.GetType().Name}");
    }

    public void Fire()
    {
        try
        {
            _logger.LogInformation("开始执行规则");
            // 模拟规则执行错误
            throw new Exception("规则执行时发生错误");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "规则执行失败");
            // 可以选择重新抛出异常或处理异常
            // throw;
        }
    }
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services)
    {
        services.AddSingleton<IRuleEngineService, ErrorHandlingRuleEngineService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("NRules 错误处理示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 NRules 服务
        services.AddNRulesServices();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();

        // 测试 1: 插入 null 事实
        try
        {
            Console.WriteLine("测试 1: 插入 null 事实");
            var session1 = ruleEngineService.CreateSession();
            session1.Insert(null);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"捕获异常: {ex.Message}");
        }

        // 测试 2: 规则执行错误
        Console.WriteLine("\n测试 2: 规则执行错误");
        var session2 = ruleEngineService.CreateSession();
        session2.Insert(new { Id = 1 });
        session2.Fire();

        Console.WriteLine("示例执行完成！");
    }
}
```

## 示例 5: 自定义规则引擎服务

### 功能说明

演示如何创建自定义的规则引擎服务，扩展 NRules 的功能。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

// 自定义规则引擎服务
public class CustomRuleEngineService : IRuleEngineService
{
    private readonly ILogger<CustomRuleEngineService> _logger;
    private readonly Dictionary<string, object> _customData = new Dictionary<string, object>();

    public CustomRuleEngineService(ILogger<CustomRuleEngineService> logger)
    {
        _logger = logger;
        _logger.LogInformation("自定义规则引擎服务初始化");
    }

    public IRuleSession CreateSession()
    {
        return new CustomRuleSession(_customData, _logger);
    }

    // 自定义方法
    public void SetCustomData(string key, object value)
    {
        _customData[key] = value;
        _logger.LogInformation($"设置自定义数据: {key} = {value}");
    }

    public object GetCustomData(string key)
    {
        return _customData.TryGetValue(key, out var value) ? value : null;
    }
}

// 自定义规则会话
public class CustomRuleSession : IRuleSession
{
    private readonly Dictionary<string, object> _customData;
    private readonly List<object> _facts = new List<object>();
    private readonly ILogger<CustomRuleEngineService> _logger;

    public CustomRuleSession(Dictionary<string, object> customData, ILogger<CustomRuleEngineService> logger)
    {
        _customData = customData;
        _logger = logger;
    }

    public void Insert(object fact)
    {
        _facts.Add(fact);
        _logger.LogInformation($"插入事实: {fact.GetType().Name}");
    }

    public void Fire()
    {
        _logger.LogInformation("执行自定义规则");
        _logger.LogInformation($"自定义数据项数量: {_customData.Count}");
        
        // 使用自定义数据
        foreach (var kvp in _customData)
        {
            _logger.LogInformation($"自定义数据: {kvp.Key} = {kvp.Value}");
        }
    }
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services)
    {
        services.AddSingleton<IRuleEngineService, CustomRuleEngineService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("NRules 自定义规则引擎服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 NRules 服务
        services.AddNRulesServices();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();

        // 使用自定义方法
        if (ruleEngineService is CustomRuleEngineService customService)
        {
            // 设置自定义数据
            customService.SetCustomData("Environment", "Production");
            customService.SetCustomData("Version", "1.0.0");
            customService.SetCustomData("MaxRules", 100);

            // 获取自定义数据
            var environment = customService.GetCustomData("Environment");
            Console.WriteLine($"获取自定义数据 - Environment: {environment}");
        }

        // 创建会话并执行规则
        var session = ruleEngineService.CreateSession();
        session.Insert(new { Id = 1, Name = "Test Order" });
        session.Fire();

        Console.WriteLine("示例执行完成！");
    }
}
```

## 示例 6: 规则测试

### 功能说明

演示如何测试 NRules 规则，确保规则正确执行。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

// 订单类
public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Discount { get; set; }
    public OrderPriority Priority { get; set; }
}

// 订单状态枚举
public enum OrderStatus
{
    New,
    Processed,
    Priority,
    Completed
}

// 订单优先级枚举
public enum OrderPriority
{
    Normal,
    High,
    Critical
}

// 规则引擎服务
public class RuleEngineService : IRuleEngineService
{
    public IRuleSession CreateSession() => new RuleSession();
}

// 规则会话
public class RuleSession : IRuleSession
{
    private Order _order;

    public void Insert(object fact)
    {
        if (fact is Order order)
        {
            _order = order;
        }
    }

    public void Fire()
    {
        if (_order != null)
        {
            // 应用高价值订单规则
            if (_order.Amount > 5000)
            {
                _order.Discount = 0.1m;
                _order.Status = OrderStatus.Processed;
                Console.WriteLine($"应用高价值订单规则: 折扣 = 10%");
            }

            // 应用优先级订单规则
            if (_order.Priority == OrderPriority.High || _order.Priority == OrderPriority.Critical)
            {
                _order.Status = OrderStatus.Priority;
                Console.WriteLine($"应用优先级订单规则: 状态 = {OrderStatus.Priority}");
            }
        }
    }
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services)
    {
        services.AddSingleton<IRuleEngineService, RuleEngineService>();
        return services;
    }
}

// 测试类
public class RuleTests
{
    private readonly IRuleEngineService _ruleEngineService;

    public RuleTests(IRuleEngineService ruleEngineService)
    {
        _ruleEngineService = ruleEngineService;
    }

    public void TestHighValueOrderRule()
    {
        Console.WriteLine("测试 1: 高价值订单规则");
        var order = new Order { Id = 1, Amount = 6000, Status = OrderStatus.New, Discount = 0, Priority = OrderPriority.Normal };
        Console.WriteLine($"测试前: 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}");

        var session = _ruleEngineService.CreateSession();
        session.Insert(order);
        session.Fire();

        Console.WriteLine($"测试后: 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}");
        Console.WriteLine($"测试结果: {(order.Discount == 0.1m && order.Status == OrderStatus.Processed ? "通过" : "失败")}");
    }

    public void TestPriorityOrderRule()
    {
        Console.WriteLine("\n测试 2: 优先级订单规则");
        var order = new Order { Id = 2, Amount = 1000, Status = OrderStatus.New, Discount = 0, Priority = OrderPriority.High };
        Console.WriteLine($"测试前: 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}, 优先级={order.Priority}");

        var session = _ruleEngineService.CreateSession();
        session.Insert(order);
        session.Fire();

        Console.WriteLine($"测试后: 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}, 优先级={order.Priority}");
        Console.WriteLine($"测试结果: {(order.Status == OrderStatus.Priority ? "通过" : "失败")}");
    }

    public void TestNormalOrder()
    {
        Console.WriteLine("\n测试 3: 普通订单");
        var order = new Order { Id = 3, Amount = 500, Status = OrderStatus.New, Discount = 0, Priority = OrderPriority.Normal };
        Console.WriteLine($"测试前: 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}");

        var session = _ruleEngineService.CreateSession();
        session.Insert(order);
        session.Fire();

        Console.WriteLine($"测试后: 金额={order.Amount}, 状态={order.Status}, 折扣={order.Discount:P}");
        Console.WriteLine($"测试结果: {(order.Discount == 0 && order.Status == OrderStatus.New ? "通过" : "失败")}");
    }
}

// 主程序
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("NRules 规则测试示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 NRules 服务
        services.AddNRulesServices();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 创建测试类
        var tests = new RuleTests(serviceProvider.GetRequiredService<IRuleEngineService>());

        // 运行测试
        tests.TestHighValueOrderRule();
        tests.TestPriorityOrderRule();
        tests.TestNormalOrder();

        Console.WriteLine("\n所有测试完成！");
    }
}
```

## 实际应用场景

### 场景 1: 电商订单处理

**功能说明**：使用 NRules 处理电商订单，根据订单金额、用户等级等因素应用不同的规则。

**应用示例**：
- 高价值订单自动升级为优先处理
- 会员用户享受额外折扣
- 促销活动期间应用特殊价格规则
- 订单风险评估和处理

### 场景 2: 金融风控系统

**功能说明**：使用 NRules 构建金融风控系统，根据交易金额、用户行为等因素评估风险。

**应用示例**：
- 大额交易自动触发人工审核
- 异常交易行为检测
- 欺诈风险评估
- 合规性检查

### 场景 3: 人力资源管理

**功能说明**：使用 NRules 处理人力资源管理中的各种规则。

**应用示例**：
- 员工绩效评估规则
- 薪酬计算规则
- 请假和考勤规则
- 晋升和调岗规则

### 场景 4: 物流配送系统

**功能说明**：使用 NRules 优化物流配送系统，根据订单类型、配送地址等因素制定配送策略。

**应用示例**：
- 配送路线优化规则
- 配送时间估计规则
- 运费计算规则
- 异常订单处理规则

### 场景 5: 医疗诊断系统

**功能说明**：使用 NRules 构建医疗诊断辅助系统，根据患者症状、检查结果等因素提供诊断建议。

**应用示例**：
- 症状匹配规则
- 诊断优先级规则
- 治疗方案推荐规则
- 药物相互作用检查规则

## 总结

NRules 技能提供了强大的规则引擎功能，可以帮助您在各种场景下实现复杂的业务规则。通过本文档的示例，您应该已经了解了 NRules 的基本使用方法、高级配置、性能优化、错误处理、自定义扩展和规则测试等方面的知识。

在实际应用中，您可以根据具体需求选择合适的使用方式，并结合 AOT 编译等技术优化性能。NRules 技能的灵活性和扩展性使其成为构建复杂业务规则系统的理想选择。
