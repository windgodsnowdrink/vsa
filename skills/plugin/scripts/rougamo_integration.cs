#:sdk Microsoft.NET.Sdk
#:package Rougamo@2.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rougamo;
using Rougamo.Context;

namespace Plugin.AOP
{
    /// <summary>
    /// 插件AOP拦截器，使用Rougamo实现方法拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PluginInterceptorAttribute : MoAttribute
    {
        /// <summary>
        /// 执行前拦截
        /// </summary>
        public override void OnEntry(MethodContext context)
        {
            var logger = context.GetService<ILogger<PluginInterceptorAttribute>>();
            logger?.LogDebug($"插件方法执行开始: {context.TargetType.Name}.{context.Method.Name}");
            context["StartTime"] = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// 执行后拦截
        /// </summary>
        public override void OnSuccess(MethodContext context)
        {
            var logger = context.GetService<ILogger<PluginInterceptorAttribute>>();
            var startTime = (long)context["StartTime"];
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            logger?.LogDebug($"插件方法执行成功: {context.TargetType.Name}.{context.Method.Name}, 耗时: {elapsed.TotalMilliseconds:F2}ms");
        }

        /// <summary>
        /// 异常拦截
        /// </summary>
        public override void OnException(MethodContext context)
        {
            var logger = context.GetService<ILogger<PluginInterceptorAttribute>>();
            logger?.LogError(context.Exception, $"插件方法执行异常: {context.TargetType.Name}.{context.Method.Name}");
        }

        /// <summary>
        /// 执行完成拦截
        /// </summary>
        public override void OnExit(MethodContext context)
        {
            var logger = context.GetService<ILogger<PluginInterceptorAttribute>>();
            logger?.LogDebug($"插件方法执行完成: {context.TargetType.Name}.{context.Method.Name}");
        }
    }

    /// <summary>
    /// 性能监控拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class PerformanceMonitorAttribute : MoAttribute
    {
        private static readonly ConcurrentDictionary<string, (long Count, double TotalTime)> _metrics = new();

        /// <summary>
        /// 执行前拦截
        /// </summary>
        public override void OnEntry(MethodContext context)
        {
            context["StartTime"] = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// 执行后拦截
        /// </summary>
        public override void OnSuccess(MethodContext context)
        {
            RecordMetrics(context);
        }

        /// <summary>
        /// 异常拦截
        /// </summary>
        public override void OnException(MethodContext context)
        {
            RecordMetrics(context);
        }

        /// <summary>
        /// 记录性能指标
        /// </summary>
        private void RecordMetrics(MethodContext context)
        {
            var startTime = (long)context["StartTime"];
            var elapsed = Stopwatch.GetElapsedTime(startTime);
            var key = $"{context.TargetType.Name}.{context.Method.Name}";

            _metrics.AddOrUpdate(
                key,
                _ => (1, elapsed.TotalMilliseconds),
                (_, existing) => (existing.Count + 1, existing.TotalTime + elapsed.TotalMilliseconds)
            );

            var metrics = _metrics[key];
            var averageTime = metrics.TotalTime / metrics.Count;

            var logger = context.GetService<ILogger<PerformanceMonitorAttribute>>();
            logger?.LogInformation($"性能指标: {key}, 执行次数: {metrics.Count}, 平均耗时: {averageTime:F2}ms");
        }

        /// <summary>
        /// 获取性能指标
        /// </summary>
        public static ConcurrentDictionary<string, (long Count, double TotalTime)> GetMetrics()
        {
            return _metrics;
        }
    }

    /// <summary>
    /// 事务拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TransactionAttribute : MoAttribute
    {
        /// <summary>
        /// 执行前拦截
        /// </summary>
        public override void OnEntry(MethodContext context)
        {
            var logger = context.GetService<ILogger<TransactionAttribute>>();
            logger?.LogDebug($"开始事务: {context.TargetType.Name}.{context.Method.Name}");
            // 这里可以实现事务开始逻辑
        }

        /// <summary>
        /// 执行后拦截
        /// </summary>
        public override void OnSuccess(MethodContext context)
        {
            var logger = context.GetService<ILogger<TransactionAttribute>>();
            logger?.LogDebug($"提交事务: {context.TargetType.Name}.{context.Method.Name}");
            // 这里可以实现事务提交逻辑
        }

        /// <summary>
        /// 异常拦截
        /// </summary>
        public override void OnException(MethodContext context)
        {
            var logger = context.GetService<ILogger<TransactionAttribute>>();
            logger?.LogDebug($"回滚事务: {context.TargetType.Name}.{context.Method.Name}");
            // 这里可以实现事务回滚逻辑
        }
    }

    /// <summary>
    /// 缓存拦截器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheAttribute : MoAttribute
    {
        private readonly TimeSpan _expiration;
        private static readonly ConcurrentDictionary<string, (object Value, DateTimeOffset ExpiresAt)> _cache = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="expirationSeconds">过期时间（秒）</param>
        public CacheAttribute(int expirationSeconds = 300)
        {
            _expiration = TimeSpan.FromSeconds(expirationSeconds);
        }

        /// <summary>
        /// 执行前拦截
        /// </summary>
        public override void OnEntry(MethodContext context)
        {
            var cacheKey = GenerateCacheKey(context);
            if (_cache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
            {
                context.ReturnValue = cached.Value;
                context.IsSkip = true;

                var logger = context.GetService<ILogger<CacheAttribute>>();
                logger?.LogDebug($"缓存命中: {cacheKey}");
            }
        }

        /// <summary>
        /// 执行后拦截
        /// </summary>
        public override void OnSuccess(MethodContext context)
        {
            if (!context.IsSkip)
            {
                var cacheKey = GenerateCacheKey(context);
                _cache[cacheKey] = (context.ReturnValue, DateTimeOffset.UtcNow.Add(_expiration));

                var logger = context.GetService<ILogger<CacheAttribute>>();
                logger?.LogDebug($"缓存设置: {cacheKey}");
            }
        }

        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey(MethodContext context)
        {
            var args = string.Join(",", context.Arguments);
            return $"{context.TargetType.Name}.{context.Method.Name}:{args}";
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public static void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// 移除指定缓存
        /// </summary>
        public static void RemoveCache(string pattern)
        {
            foreach (var key in _cache.Keys)
            {
                if (key.Contains(pattern))
                {
                    _cache.TryRemove(key, out _);
                }
            }
        }
    }

    /// <summary>
    /// Rougamo集成服务
    /// </summary>
    public class RougamoIntegrationService
    {
        private readonly ILogger<RougamoIntegrationService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public RougamoIntegrationService(ILogger<RougamoIntegrationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 初始化Rougamo
        /// </summary>
        public void Initialize()
        {
            _logger?.LogInformation("初始化Rougamo AOP集成");
            // 这里可以添加Rougamo的初始化逻辑
        }

        /// <summary>
        /// 扫描并注册带有AOP特性的类型
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assemblies">要扫描的程序集</param>
        public void RegisterAopTypes(IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.GetCustomAttributes(typeof(MoAttribute), true).Length > 0)
                        {
                            services.AddTransient(type);
                            _logger?.LogDebug($"注册带有AOP特性的类型: {type.FullName}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"扫描程序集AOP类型失败: {assembly.FullName}");
                }
            }
        }

        /// <summary>
        /// 获取性能指标
        /// </summary>
        public ConcurrentDictionary<string, (long Count, double TotalTime)> GetPerformanceMetrics()
        {
            return PerformanceMonitorAttribute.GetMetrics();
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void ClearAllCache()
        {
            CacheAttribute.ClearCache();
            _logger?.LogInformation("清除所有AOP缓存");
        }

        /// <summary>
        /// 移除指定模式的缓存
        /// </summary>
        /// <param name="pattern">缓存键模式</param>
        public void RemoveCache(string pattern)
        {
            CacheAttribute.RemoveCache(pattern);
            _logger?.LogInformation($"移除缓存模式: {pattern}");
        }
    }

    /// <summary>
    /// 依赖注入扩展
    /// </summary>
    public static class RougamoDependencyInjection
    {
        /// <summary>
        /// 添加Rougamo集成
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddRougamoIntegration(this IServiceCollection services)
        {
            services.AddSingleton<RougamoIntegrationService>();
            return services;
        }

        /// <summary>
        /// 添加Rougamo集成并扫描程序集
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assemblies">要扫描的程序集</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddRougamoIntegration(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<RougamoIntegrationService>(sp =>
            {
                var service = new RougamoIntegrationService(sp.GetRequiredService<ILogger<RougamoIntegrationService>>());
                service.Initialize();
                service.RegisterAopTypes(services, assemblies);
                return service;
            });
            return services;
        }
    }

    /// <summary>
    /// 示例插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 初始化插件
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        Task<object> ExecuteAsync(object context);

        /// <summary>
        /// 关闭插件
        /// </summary>
        Task ShutdownAsync();
    }

    /// <summary>
    /// 示例插件实现
    /// </summary>
    public class SamplePlugin : IPlugin
    {
        private readonly ILogger<SamplePlugin> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public SamplePlugin(ILogger<SamplePlugin> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        [PluginInterceptor]
        [PerformanceMonitor]
        public async Task InitializeAsync()
        {
            _logger.LogInformation("初始化示例插件");
            await Task.Delay(100);
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        /// <param name="context">执行上下文</param>
        [PluginInterceptor]
        [PerformanceMonitor]
        [Cache(60)]
        public async Task<object> ExecuteAsync(object context)
        {
            _logger.LogInformation($"执行示例插件，上下文: {context}");
            await Task.Delay(200);
            return $"插件执行结果: {DateTime.Now}";
        }

        /// <summary>
        /// 关闭插件
        /// </summary>
        [PluginInterceptor]
        [PerformanceMonitor]
        public async Task ShutdownAsync()
        {
            _logger.LogInformation("关闭示例插件");
            await Task.Delay(50);
        }
    }

    /// <summary>
    /// 程序入口
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args">命令行参数</param>
        public static async Task Main(string[] args)
        {
            // 创建服务集合
            var services = new ServiceCollection();

            // 添加日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            // 添加Rougamo集成
            services.AddRougamoIntegration(Assembly.GetExecutingAssembly());

            // 添加插件
            services.AddTransient<IPlugin, SamplePlugin>();

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 测试插件执行
            var plugin = serviceProvider.GetRequiredService<IPlugin>();
            await plugin.InitializeAsync();
            
            for (int i = 0; i < 3; i++)
            {
                var result = await plugin.ExecuteAsync($"测试参数 {i}");
                Console.WriteLine($"插件执行结果: {result}");
                await Task.Delay(1000);
            }

            await plugin.ShutdownAsync();

            // 获取性能指标
            var rougamoService = serviceProvider.GetRequiredService<RougamoIntegrationService>();
            var metrics = rougamoService.GetPerformanceMetrics();
            Console.WriteLine("\n性能指标:");
            foreach (var (method, (count, totalTime)) in metrics)
            {
                var averageTime = totalTime / count;
                Console.WriteLine($"{method}: 执行 {count} 次, 平均耗时 {averageTime:F2}ms");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
