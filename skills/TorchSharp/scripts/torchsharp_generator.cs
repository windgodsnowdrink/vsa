#:sdk Microsoft.NET.Sdk
#:package Scrutor@4.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Logging.Console@10.0.0
#:package System.Reflection.Metadata@7.0.0
#:package System.Linq.Expressions@4.7.0
#:property LangVersion=10.0
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TorchSharp.Skill.Generator
{
    /// <summary>
    /// Scrutor 用法示例生成器
    /// </summary>
    public class ScrutorGenerator
    {
        private readonly IServiceProvider _serviceProvider;

        public ScrutorGenerator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static void Main(string[] args)
        {
            // 初始化服务容器
            var services = new ServiceCollection();
            
            // 添加日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // 添加生成器服务
            services.AddSingleton<ScrutorGenerator>();
            
            // 构建服务提供者
            var serviceProvider = services.BuildServiceProvider();
            
            // 获取生成器实例
            var generator = serviceProvider.GetRequiredService<ScrutorGenerator>();
            
            // 运行所有演示
            generator.RunAllDemos();
        }

        public void RunAllDemos()
        {
            Console.WriteLine("=========================================");
            Console.WriteLine("Scrutor 用法示例生成器");
            Console.WriteLine("=========================================");
            
            RunBasicRegistrationDemo();
            RunDecoratorPatternDemo();
            RunServiceFilteringDemo();
            RunLifetimeManagementDemo();
            RunAdvancedRegistrationDemo();
            RunAssemblyScanningDemo();
            RunMultipleDecoratorsDemo();
            RunConditionalRegistrationDemo();
            
            Console.WriteLine("=========================================");
            Console.WriteLine("所有演示完成！");
            Console.WriteLine("=========================================");
        }

        /// <summary>
        /// 基本服务注册演示
        /// </summary>
        public void RunBasicRegistrationDemo()
        {
            Console.WriteLine("\n=== 1. 基本服务注册演示 ===");
            
            var services = new ServiceCollection();
            
            // 定义服务接口和实现
            interface IHelloService { string SayHello(string name); }
            class HelloService : IHelloService { public string SayHello(string name) => $"Hello, {name}!"; }
            
            // 传统注册方式
            services.AddSingleton<IHelloService, HelloService>();
            
            var provider = services.BuildServiceProvider();
            var helloService = provider.GetRequiredService<IHelloService>();
            
            Console.WriteLine(helloService.SayHello("World"));
            Console.WriteLine("基本服务注册演示完成！");
        }

        /// <summary>
        /// 装饰器模式演示
        /// </summary>
        public void RunDecoratorPatternDemo()
        {
            Console.WriteLine("\n=== 2. 装饰器模式演示 ===");
            
            // 定义服务接口和实现
            interface ICalculator { int Add(int a, int b); }
            class Calculator : ICalculator { public int Add(int a, int b) => a + b; }
            
            // 定义装饰器
            class LoggingCalculator : ICalculator
            {
                private readonly ICalculator _calculator;
                private readonly ILogger<LoggingCalculator> _logger;
                
                public LoggingCalculator(ICalculator calculator, ILogger<LoggingCalculator> logger)
                {
                    _calculator = calculator;
                    _logger = logger;
                }
                
                public int Add(int a, int b)
                {
                    _logger.LogInformation($"Adding {a} + {b}");
                    var result = _calculator.Add(a, b);
                    _logger.LogInformation($"Result: {result}");
                    return result;
                }
            }
            
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            
            // 注册服务和装饰器
            services.AddSingleton<ICalculator, Calculator>();
            services.Decorate<ICalculator, LoggingCalculator>();
            
            var provider = services.BuildServiceProvider();
            var calculator = provider.GetRequiredService<ICalculator>();
            
            var result = calculator.Add(10, 20);
            Console.WriteLine($"最终结果: {result}");
            Console.WriteLine("装饰器模式演示完成！");
        }

        /// <summary>
        /// 服务筛选演示
        /// </summary>
        public void RunServiceFilteringDemo()
        {
            Console.WriteLine("\n=== 3. 服务筛选演示 ===");
            
            // 定义多个服务接口和实现
            interface IService { string Name { get; } }
            class ServiceA : IService { public string Name => "Service A"; }
            class ServiceB : IService { public string Name => "Service B"; }
            class ServiceC : IService { public string Name => "Service C"; }
            
            var services = new ServiceCollection();
            
            // 注册所有服务
            services.AddSingleton<IService, ServiceA>();
            services.AddSingleton<IService, ServiceB>();
            services.AddSingleton<IService, ServiceC>();
            
            var provider = services.BuildServiceProvider();
            
            // 获取所有服务实例
            var allServices = provider.GetServices<IService>();
            Console.WriteLine("所有服务:");
            foreach (var service in allServices)
            {
                Console.WriteLine($"- {service.Name}");
            }
            
            Console.WriteLine("服务筛选演示完成！");
        }

        /// <summary>
        /// 生命周期管理演示
        /// </summary>
        public void RunLifetimeManagementDemo()
        {
            Console.WriteLine("\n=== 4. 生命周期管理演示 ===");
            
            // 定义服务接口和实现
            interface ITransientService { Guid Id { get; } }
            interface IScopedService { Guid Id { get; } }
            interface ISingletonService { Guid Id { get; } }
            
            class TransientService : ITransientService { public Guid Id { get; } = Guid.NewGuid(); }
            class ScopedService : IScopedService { public Guid Id { get; } = Guid.NewGuid(); }
            class SingletonService : ISingletonService { public Guid Id { get; } = Guid.NewGuid(); }
            
            var services = new ServiceCollection();
            
            // 注册不同生命周期的服务
            services.AddTransient<ITransientService, TransientService>();
            services.AddScoped<IScopedService, ScopedService>();
            services.AddSingleton<ISingletonService, SingletonService>();
            
            var provider = services.BuildServiceProvider();
            
            Console.WriteLine("第一次解析:");
            var transient1 = provider.GetRequiredService<ITransientService>();
            var singleton1 = provider.GetRequiredService<ISingletonService>();
            Console.WriteLine($"Transient: {transient1.Id}");
            Console.WriteLine($"Singleton: {singleton1.Id}");
            
            Console.WriteLine("第二次解析:");
            var transient2 = provider.GetRequiredService<ITransientService>();
            var singleton2 = provider.GetRequiredService<ISingletonService>();
            Console.WriteLine($"Transient: {transient2.Id} (不同实例: {transient1.Id != transient2.Id})");
            Console.WriteLine($"Singleton: {singleton2.Id} (相同实例: {singleton1.Id == singleton2.Id})");
            
            // 测试 Scoped 生命周期
            Console.WriteLine("\n测试 Scoped 生命周期:");
            using (var scope1 = provider.CreateScope())
            {
                var scoped1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
                Console.WriteLine($"Scope 1 - Scoped: {scoped1.Id}");
            }
            
            using (var scope2 = provider.CreateScope())
            {
                var scoped2 = scope2.ServiceProvider.GetRequiredService<IScopedService>();
                Console.WriteLine($"Scope 2 - Scoped: {scoped2.Id} (不同实例: {true})");
            }
            
            Console.WriteLine("生命周期管理演示完成！");
        }

        /// <summary>
        /// 高级服务注册演示
        /// </summary>
        public void RunAdvancedRegistrationDemo()
        {
            Console.WriteLine("\n=== 5. 高级服务注册演示 ===");
            
            var services = new ServiceCollection();
            
            // 定义服务接口和实现
            interface IRepository<T> { void Save(T entity); }
            class Repository<T> : IRepository<T> { public void Save(T entity) => Console.WriteLine($"Saved entity: {entity}"); }
            
            // 注册泛型服务
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));
            
            var provider = services.BuildServiceProvider();
            
            // 解析不同类型的仓库
            var userRepo = provider.GetRequiredService<IRepository<User>>();
            var productRepo = provider.GetRequiredService<IRepository<Product>>();
            
            // 测试保存
            userRepo.Save(new User { Id = 1, Name = "John" });
            productRepo.Save(new Product { Id = 1, Name = "Laptop" });
            
            Console.WriteLine("高级服务注册演示完成！");
        }

        /// <summary>
        /// 程序集扫描演示
        /// </summary>
        public void RunAssemblyScanningDemo()
        {
            Console.WriteLine("\n=== 6. 程序集扫描演示 ===");
            
            var services = new ServiceCollection();
            
            // 使用 Scrutor 扫描当前程序集并注册服务
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly())
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Scanner")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
            
            var provider = services.BuildServiceProvider();
            
            // 测试扫描结果
            var demoScanners = provider.GetServices<IDemoScanner>();
            Console.WriteLine($"找到 {demoScanners.Count()} 个 IDemoScanner 实现:");
            foreach (var scanner in demoScanners)
            {
                scanner.Scan();
            }
            
            Console.WriteLine("程序集扫描演示完成！");
        }

        /// <summary>
        /// 多个装饰器演示
        /// </summary>
        public void RunMultipleDecoratorsDemo()
        {
            Console.WriteLine("\n=== 7. 多个装饰器演示 ===");
            
            // 定义服务接口和实现
            interface IOperation { int Execute(int a, int b); }
            class Operation : IOperation { public int Execute(int a, int b) => a + b; }
            
            // 定义多个装饰器
            class LoggingDecorator : IOperation
            {
                private readonly IOperation _operation;
                public LoggingDecorator(IOperation operation) { _operation = operation; }
                public int Execute(int a, int b) { Console.WriteLine($"Logging: {a} + {b}"); return _operation.Execute(a, b); }
            }
            
            class ValidationDecorator : IOperation
            {
                private readonly IOperation _operation;
                public ValidationDecorator(IOperation operation) { _operation = operation; }
                public int Execute(int a, int b) { if (a < 0 || b < 0) throw new ArgumentException("参数不能为负数"); return _operation.Execute(a, b); }
            }
            
            class CachingDecorator : IOperation
            {
                private readonly IOperation _operation;
                private readonly Dictionary<(int, int), int> _cache = new();
                public CachingDecorator(IOperation operation) { _operation = operation; }
                public int Execute(int a, int b) 
                {
                    var key = (a, b);
                    if (_cache.TryGetValue(key, out var result))
                    {
                        Console.WriteLine($"Cache hit for {a} + {b}");
                        return result;
                    }
                    result = _operation.Execute(a, b);
                    _cache[key] = result;
                    Console.WriteLine($"Cache miss for {a} + {b}");
                    return result;
                }
            }
            
            var services = new ServiceCollection();
            
            // 注册服务和多个装饰器
            services.AddSingleton<IOperation, Operation>();
            services.Decorate<IOperation, ValidationDecorator>();
            services.Decorate<IOperation, LoggingDecorator>();
            services.Decorate<IOperation, CachingDecorator>();
            
            var provider = services.BuildServiceProvider();
            var operation = provider.GetRequiredService<IOperation>();
            
            // 测试执行
            Console.WriteLine("第一次执行: 5 + 3");
            var result1 = operation.Execute(5, 3);
            Console.WriteLine($"结果: {result1}");
            
            Console.WriteLine("\n第二次执行: 5 + 3 (应该从缓存获取)");
            var result2 = operation.Execute(5, 3);
            Console.WriteLine($"结果: {result2}");
            
            Console.WriteLine("\n第三次执行: 10 + 20");
            var result3 = operation.Execute(10, 20);
            Console.WriteLine($"结果: {result3}");
            
            try
            {
                Console.WriteLine("\n测试负数参数:");
                operation.Execute(-1, 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"预期的验证错误: {ex.Message}");
            }
            
            Console.WriteLine("多个装饰器演示完成！");
        }

        /// <summary>
        /// 条件注册演示
        /// </summary>
        public void RunConditionalRegistrationDemo()
        {
            Console.WriteLine("\n=== 8. 条件注册演示 ===");
            
            var services = new ServiceCollection();
            
            // 定义服务接口和实现
            interface IFeatureService { string GetFeatureName(); }
            class PremiumFeatureService : IFeatureService { public string GetFeatureName() => "Premium Feature"; }
            class StandardFeatureService : IFeatureService { public string GetFeatureName() => "Standard Feature"; }
            
            // 模拟环境变量
            bool isPremium = Environment.GetEnvironmentVariable("IS_PREMIUM") == "true";
            Console.WriteLine($"当前环境: {(isPremium ? "Premium" : "Standard")}");
            
            // 条件注册服务
            if (isPremium)
            {
                services.AddSingleton<IFeatureService, PremiumFeatureService>();
            }
            else
            {
                services.AddSingleton<IFeatureService, StandardFeatureService>();
            }
            
            var provider = services.BuildServiceProvider();
            var featureService = provider.GetRequiredService<IFeatureService>();
            
            Console.WriteLine($"激活的功能: {featureService.GetFeatureName()}");
            
            Console.WriteLine("条件注册演示完成！");
        }
    }

    // 辅助类
    public class User { public int Id { get; set; } public string Name { get; set; } public override string ToString() => $"User({Id}, {Name})"; }
    public class Product { public int Id { get; set; } public string Name { get; set; } public override string ToString() => $"Product({Id}, {Name})"; }

    // 用于程序集扫描演示的接口和实现
    public interface IDemoScanner { void Scan(); }
    public class DemoScanner1 : IDemoScanner { public void Scan() => Console.WriteLine("DemoScanner1 扫描完成"); }
    public class DemoScanner2 : IDemoScanner { public void Scan() => Console.WriteLine("DemoScanner2 扫描完成"); }
}
