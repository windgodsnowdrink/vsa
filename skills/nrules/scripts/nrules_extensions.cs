#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package NRules@1.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Channels@10.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Buffers;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 配置选项
public class NRulesOptions
{
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDetailedLogging { get; set; } = false;
    public string DefaultTimeZone { get; set; } = "UTC";
}

// 规则引擎服务接口
public interface IRuleEngineService
{
    IRuleSession CreateSession();
}

// 规则仓储接口
public interface IRuleRepository
{
    void RegisterRulesFromAssembly(Assembly assembly);
    void RegisterRule(Type ruleType);
    RuleFactory CreateRuleFactory();
}

// 规则会话接口
public interface IRuleSession
{
    void Insert(object fact);
    void Update(object fact);
    void Delete(object fact);
    void Fire();
}

// 规则基类
public abstract class Rule
{
    public abstract void Define();
}

// 规则上下文接口
public interface IContext
{
    void Insert(object fact);
    void Update(object fact);
    void Delete(object fact);
}

// 规则工厂接口
public interface RuleFactory
{
    ISession CreateSession();
}

// 会话接口
public interface ISession
{
    void Insert(object fact);
    void Update(object fact);
    void Delete(object fact);
    void Fire();
}

// 规则定义
public class RuleDefinition
{
    public Type RuleType { get; set; }
    public Action<IContext, object> Action { get; set; }
    public Func<object, bool> Condition { get; set; }
}

// 规则仓储实现
public class RuleRepository : IRuleRepository
{
    private readonly List<RuleDefinition> _rules = new List<RuleDefinition>();
    private readonly object _lock = new object();
    private readonly NRulesOptions _options;
    private readonly ILogger<RuleRepository> _logger;

    public RuleRepository(IOptions<NRulesOptions> options, ILogger<RuleRepository> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public void RegisterRulesFromAssembly(Assembly assembly)
    {
        var ruleTypes = assembly.GetTypes()
            .Where(t => typeof(Rule).IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        foreach (var ruleType in ruleTypes)
        {
            RegisterRule(ruleType);
        }

        _logger.LogInformation("从程序集 {AssemblyName} 注册了 {RuleCount} 个规则", assembly.GetName().Name, ruleTypes.Count);
    }

    public void RegisterRule(Type ruleType)
    {
        lock (_lock)
        {
            var ruleDefinition = new RuleDefinition
            {
                RuleType = ruleType
            };

            _rules.Add(ruleDefinition);
            _logger.LogDebug("注册规则: {RuleName}", ruleType.Name);
        }
    }

    public RuleFactory CreateRuleFactory()
    {
        return new RuleFactoryImpl(_rules, _options, _logger);
    }
}

// 规则工厂实现
public class RuleFactoryImpl : RuleFactory
{
    private readonly List<RuleDefinition> _rules;
    private readonly NRulesOptions _options;
    private readonly ILogger<RuleFactoryImpl> _logger;

    public RuleFactoryImpl(List<RuleDefinition> rules, NRulesOptions options, ILogger<RuleFactoryImpl> logger)
    {
        _rules = rules;
        _options = options;
        _logger = logger;
    }

    public ISession CreateSession()
    {
        return new SessionImpl(_rules, _options, _logger);
    }
}

// 会话实现
public class SessionImpl : ISession
{
    private readonly List<RuleDefinition> _rules;
    private readonly List<object> _facts = new List<object>();
    private readonly NRulesOptions _options;
    private readonly ILogger<SessionImpl> _logger;

    public SessionImpl(List<RuleDefinition> rules, NRulesOptions options, ILogger<SessionImpl> logger)
    {
        _rules = rules;
        _options = options;
        _logger = logger;
    }

    public void Insert(object fact)
    {
        if (fact == null)
        {
            throw new ArgumentNullException(nameof(fact));
        }

        _facts.Add(fact);
        _logger.LogDebug("插入事实: {FactType}", fact.GetType().Name);
    }

    public void Update(object fact)
    {
        if (fact == null)
        {
            throw new ArgumentNullException(nameof(fact));
        }

        if (!_facts.Contains(fact))
        {
            _facts.Add(fact);
        }
        _logger.LogDebug("更新事实: {FactType}", fact.GetType().Name);
    }

    public void Delete(object fact)
    {
        if (fact == null)
        {
            throw new ArgumentNullException(nameof(fact));
        }

        _facts.Remove(fact);
        _logger.LogDebug("删除事实: {FactType}", fact.GetType().Name);
    }

    public void Fire()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("开始执行规则，事实数量: {FactCount}, 规则数量: {RuleCount}", _facts.Count, _rules.Count);

        try
        {
            foreach (var rule in _rules)
            {
                FireRule(rule);
            }

            stopwatch.Stop();
            _logger.LogInformation("规则执行完成，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "规则执行失败，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private void FireRule(RuleDefinition rule)
    {
        try
        {
            var ruleInstance = Activator.CreateInstance(rule.RuleType) as Rule;
            if (ruleInstance == null)
            {
                _logger.LogError("无法创建规则实例: {RuleName}", rule.RuleType.Name);
                return;
            }

            // 这里简化实现，实际NRules会有更复杂的规则定义和执行逻辑
            _logger.LogDebug("执行规则: {RuleName}", rule.RuleType.Name);

            // 模拟规则执行
            foreach (var fact in _facts)
            {
                // 这里应该根据规则定义的条件进行匹配
                // 简化实现，直接处理Order类型
                if (fact is Order order)
                {
                    ProcessOrderRule(ruleInstance, order);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行规则时发生错误: {RuleName}", rule.RuleType.Name);
        }
    }

    private void ProcessOrderRule(Rule rule, Order order)
    {
        // 简化实现，根据规则类型处理订单
        if (rule.GetType().Name.Contains("HighValueOrderRule"))
        {
            if (order.Amount > 5000 && order.Status == OrderStatus.New)
            {
                order.Discount = 0.1m;
                order.Status = OrderStatus.Processed;
                _logger.LogInformation("应用高价值订单规则: 订单 ID={OrderId}, 金额={Amount}, 折扣={Discount:P}", order.Id, order.Amount, order.Discount);
            }
        }
        else if (rule.GetType().Name.Contains("PriorityOrderRule"))
        {
            if (order.Priority == OrderPriority.High && order.Status == OrderStatus.New)
            {
                order.Status = OrderStatus.Priority;
                _logger.LogInformation("应用优先级订单规则: 订单 ID={OrderId}, 优先级={Priority}", order.Id, order.Priority);
            }
        }
    }
}

// 规则引擎服务实现
public class RuleEngineService : IRuleEngineService
{
    private readonly IRuleRepository _ruleRepository;
    private readonly NRulesOptions _options;
    private readonly ILogger<RuleEngineService> _logger;

    public RuleEngineService(IRuleRepository ruleRepository, IOptions<NRulesOptions> options, ILogger<RuleEngineService> logger)
    {
        _ruleRepository = ruleRepository;
        _options = options.Value;
        _logger = logger;
    }

    public IRuleSession CreateSession()
    {
        _logger.LogDebug("创建规则会话");
        var factory = _ruleRepository.CreateRuleFactory();
        var session = factory.CreateSession();
        return new RuleSessionImpl(session, _logger);
    }
}

// 规则会话实现
public class RuleSessionImpl : IRuleSession
{
    private readonly ISession _session;
    private readonly ILogger<RuleSessionImpl> _logger;

    public RuleSessionImpl(ISession session, ILogger<RuleSessionImpl> logger)
    {
        _session = session;
        _logger = logger;
    }

    public void Insert(object fact)
    {
        _session.Insert(fact);
    }

    public void Update(object fact)
    {
        _session.Update(fact);
    }

    public void Delete(object fact)
    {
        _session.Delete(fact);
    }

    public void Fire()
    {
        _session.Fire();
    }
}

// 订单类
public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }
    public OrderPriority Priority { get; set; }
    public decimal Discount { get; set; }
}

// 订单状态枚举
public enum OrderStatus
{
    New,
    Processed,
    Priority,
    Completed,
    Cancelled
}

// 订单优先级枚举
public enum OrderPriority
{
    Normal,
    High,
    Critical
}

// 依赖注入扩展
public static class NRulesServiceCollectionExtensions
{
    public static IServiceCollection AddNRulesServices(this IServiceCollection services, Action<NRulesOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<NRulesOptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IRuleRepository, RuleRepository>();
        services.AddSingleton<IRuleEngineService, RuleEngineService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("NRules 规则引擎示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 配置 NRules
        services.AddNRulesServices(options =>
        {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.EnableDetailedLogging = false;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ruleEngineService = serviceProvider.GetRequiredService<IRuleEngineService>();
        var ruleRepository = serviceProvider.GetRequiredService<IRuleRepository>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 注册规则
            ruleRepository.RegisterRulesFromAssembly(typeof(OrderRules).Assembly);

            // 示例1: 高价值订单
            Console.WriteLine("示例1: 高价值订单");
            var session1 = ruleEngineService.CreateSession();
            var highValueOrder = new Order { Id = 1, Amount = 6000, Status = OrderStatus.New, Priority = OrderPriority.Normal };
            session1.Insert(highValueOrder);
            session1.Fire();
            Console.WriteLine($"执行结果: 订单状态={highValueOrder.Status}, 折扣={highValueOrder.Discount:P}");

            // 示例2: 优先级订单
            Console.WriteLine("\n示例2: 优先级订单");
            var session2 = ruleEngineService.CreateSession();
            var priorityOrder = new Order { Id = 2, Amount = 1000, Status = OrderStatus.New, Priority = OrderPriority.High };
            session2.Insert(priorityOrder);
            session2.Fire();
            Console.WriteLine($"执行结果: 订单状态={priorityOrder.Status}, 折扣={priorityOrder.Discount:P}");

            // 示例3: 普通订单
            Console.WriteLine("\n示例3: 普通订单");
            var session3 = ruleEngineService.CreateSession();
            var normalOrder = new Order { Id = 3, Amount = 500, Status = OrderStatus.New, Priority = OrderPriority.Normal };
            session3.Insert(normalOrder);
            session3.Fire();
            Console.WriteLine($"执行结果: 订单状态={normalOrder.Status}, 折扣={normalOrder.Discount:P}");

            // 示例4: 性能测试
            Console.WriteLine("\n示例4: 性能测试");
            const int iterations = 1000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var session = ruleEngineService.CreateSession();
                var order = new Order { Id = i + 100, Amount = 6000, Status = OrderStatus.New };
                session.Insert(order);
                session.Fire();
            }

            stopwatch.Stop();
            Console.WriteLine($"执行 {iterations} 次规则引擎会话: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}

// 订单规则
public class OrderRules
{
    public class HighValueOrderRule : Rule
    {
        public override void Define()
        {
            // 规则定义
        }
    }

    public class PriorityOrderRule : Rule
    {
        public override void Define()
        {
            // 规则定义
        }
    }
}