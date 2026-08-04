#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true
#:property RuntimeIdentifier=win-x64
#:property Configuration=Release
#:property EnableCompilation=true
#:property EnableOptimalCodeGeneration=true
#:property LangVersion=preview
#:property Nullable=enable
#:property ImplicitUsings=enable

#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Scrutor@4.2.0
#:package Microsoft.Extensions.Logging@9.0.0
#:package Microsoft.Extensions.Logging.Console@9.0.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tye.Generator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Scrutor用法示例");
            Console.WriteLine("================");
            
            // 示例1: 基本注册
            Console.WriteLine("\n1. 基本注册示例:");
            BasicRegistrationExample();
            
            // 示例2: 装饰器模式
            Console.WriteLine("\n2. 装饰器模式示例:");
            DecoratorPatternExample();
            
            // 示例3: 服务过滤
            Console.WriteLine("\n3. 服务过滤示例:");
            ServiceFilteringExample();
            
            // 示例4: 生命周期管理
            Console.WriteLine("\n4. 生命周期管理示例:");
            LifetimeManagementExample();
            
            // 示例5: 高级注册
            Console.WriteLine("\n5. 高级注册示例:");
            AdvancedRegistrationExample();
            
            // 示例6: 程序集扫描
            Console.WriteLine("\n6. 程序集扫描示例:");
            AssemblyScanningExample();
            
            // 示例7: 多个装饰器
            Console.WriteLine("\n7. 多个装饰器示例:");
            MultipleDecoratorsExample();
            
            // 示例8: 条件注册
            Console.WriteLine("\n8. 条件注册示例:");
            ConditionalRegistrationExample();
            
            Console.WriteLine("\n所有示例执行完成！");
        }
        
        // 示例1: 基本注册
        private static void BasicRegistrationExample()
        {
            var services = new ServiceCollection();
            
            // 基本注册方式
            services.AddTransient<IService, Service>();
            
            // 使用Scrutor的注册方式
            services.Scan(scan => scan
                .AddTypes(typeof(IService), typeof(Service))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IService>();
            service.Execute();
            
            Console.WriteLine("基本注册示例完成");
        }
        
        // 示例2: 装饰器模式
        private static void DecoratorPatternExample()
        {
            var services = new ServiceCollection();
            
            // 注册基础服务
            services.AddTransient<IService, Service>();
            
            // 使用Scrutor添加装饰器
            services.Decorate<IService, LoggingServiceDecorator>();
            
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IService>();
            service.Execute();
            
            Console.WriteLine("装饰器模式示例完成");
        }
        
        // 示例3: 服务过滤
        private static void ServiceFilteringExample()
        {
            var services = new ServiceCollection();
            
            // 注册多个服务
            services.AddTransient<IService, Service>();
            services.AddTransient<IService, AnotherService>();
            services.AddTransient<IAnotherService, AnotherService>();
            
            // 使用Scrutor过滤服务
            services.Scan(scan => scan
                .FromAssemblies(typeof(Program).Assembly)
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            var serviceProvider = services.BuildServiceProvider();
            var servicesList = serviceProvider.GetServices<IService>();
            
            Console.WriteLine($"找到 {servicesList.Count()} 个 IService 实现");
            foreach (var service in servicesList)
            {
                service.Execute();
            }
            
            Console.WriteLine("服务过滤示例完成");
        }
        
        // 示例4: 生命周期管理
        private static void LifetimeManagementExample()
        {
            var services = new ServiceCollection();
            
            // 使用Scrutor管理不同生命周期
            services.Scan(scan => scan
                .FromAssemblies(typeof(Program).Assembly)
                .AddClasses(classes => classes.Where(c => c.Name == "Service"))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                
                .AddClasses(classes => classes.Where(c => c.Name == "SingletonService"))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                
                .AddClasses(classes => classes.Where(c => c.Name == "ScopedService"))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            
            var serviceProvider = services.BuildServiceProvider();
            
            // 测试瞬态服务
            var transient1 = serviceProvider.GetRequiredService<IService>();
            var transient2 = serviceProvider.GetRequiredService<IService>();
            Console.WriteLine($"瞬态服务是否相同实例: {ReferenceEquals(transient1, transient2)}");
            
            // 测试单例服务
            var singleton1 = serviceProvider.GetRequiredService<ISingletonService>();
            var singleton2 = serviceProvider.GetRequiredService<ISingletonService>();
            Console.WriteLine($"单例服务是否相同实例: {ReferenceEquals(singleton1, singleton2)}");
            
            // 测试作用域服务
            using (var scope1 = serviceProvider.CreateScope())
            using (var scope2 = serviceProvider.CreateScope())
            {
                var scoped1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
                var scoped2 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
                var scoped3 = scope2.ServiceProvider.GetRequiredService<IScopedService>();
                
                Console.WriteLine($"同一作用域内服务是否相同实例: {ReferenceEquals(scoped1, scoped2)}");
                Console.WriteLine($"不同作用域服务是否相同实例: {ReferenceEquals(scoped1, scoped3)}");
            }
            
            Console.WriteLine("生命周期管理示例完成");
        }
        
        // 示例5: 高级注册
        private static void AdvancedRegistrationExample()
        {
            var services = new ServiceCollection();
            
            // 高级注册配置
            services.Scan(scan => scan
                .FromAssemblies(typeof(Program).Assembly)
                
                // 注册所有实现了IService接口的类
                .AddClasses(classes => classes.AssignableTo<IService>())
                .As<IService>()
                .WithTransientLifetime()
                
                // 注册所有类作为自身类型
                .AddClasses()
                .AsSelf()
                .WithTransientLifetime()
                
                // 注册所有类同时作为自身和实现的接口
                .AddClasses()
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
            
            var serviceProvider = services.BuildServiceProvider();
            
            // 测试注册结果
            var service = serviceProvider.GetRequiredService<IService>();
            var serviceInstance = serviceProvider.GetRequiredService<Service>();
            var anotherService = serviceProvider.GetRequiredService<AnotherService>();
            
            service.Execute();
            serviceInstance.Execute();
            anotherService.Execute();
            
            Console.WriteLine("高级注册示例完成");
        }
        
        // 示例6: 程序集扫描
        private static void AssemblyScanningExample()
        {
            var services = new ServiceCollection();
            
            // 扫描当前程序集
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            // 也可以扫描多个程序集
            // services.Scan(scan => scan
            //     .FromAssemblies(
            //         Assembly.GetExecutingAssembly(),
            //         Assembly.Load("AnotherAssembly"))
            //     .AddClasses()
            //     .AsImplementedInterfaces()
            //     .WithTransientLifetime());
            
            var serviceProvider = services.BuildServiceProvider();
            
            // 测试所有注册的服务
            var service = serviceProvider.GetRequiredService<IService>();
            var anotherService = serviceProvider.GetRequiredService<IAnotherService>();
            var singletonService = serviceProvider.GetRequiredService<ISingletonService>();
            var scopedService = serviceProvider.GetRequiredService<IScopedService>();
            
            service.Execute();
            anotherService.Execute();
            singletonService.Execute();
            scopedService.Execute();
            
            Console.WriteLine("程序集扫描示例完成");
        }
        
        // 示例7: 多个装饰器
        private static void MultipleDecoratorsExample()
        {
            var services = new ServiceCollection();
            
            // 注册基础服务
            services.AddTransient<IService, Service>();
            
            // 添加多个装饰器（顺序很重要）
            services.Decorate<IService, LoggingServiceDecorator>();
            services.Decorate<IService, CachingServiceDecorator>();
            services.Decorate<IService, TransactionalServiceDecorator>();
            
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IService>();
            service.Execute();
            
            Console.WriteLine("多个装饰器示例完成");
        }
        
        // 示例8: 条件注册
        private static void ConditionalRegistrationExample()
        {
            var services = new ServiceCollection();
            
            // 条件注册
            services.Scan(scan => scan
                .FromAssemblies(typeof(Program).Assembly)
                .AddClasses(classes => classes.Where(c => {
                    // 根据条件注册
                    return c.Name.StartsWith("S") && c.Name.EndsWith("Service");
                }))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
            
            // 条件装饰器
            services.AddTransient<IService, Service>();
            
            // 只在特定条件下添加装饰器
            bool enableLogging = true;
            if (enableLogging)
            {
                services.Decorate<IService, LoggingServiceDecorator>();
            }
            
            var serviceProvider = services.BuildServiceProvider();
            var service = serviceProvider.GetRequiredService<IService>();
            service.Execute();
            
            Console.WriteLine("条件注册示例完成");
        }
    }
    
    // 服务接口和实现
    public interface IService
    {
        void Execute();
    }
    
    public class Service : IService
    {
        public void Execute()
        {
            Console.WriteLine("Service.Execute() 执行");
        }
    }
    
    public class AnotherService : IService, IAnotherService
    {
        public void Execute()
        {
            Console.WriteLine("AnotherService.Execute() 执行");
        }
    }
    
    public interface IAnotherService
    {
        void Execute();
    }
    
    public interface ISingletonService
    {
        void Execute();
    }
    
    public class SingletonService : ISingletonService
    {
        public void Execute()
        {
            Console.WriteLine("SingletonService.Execute() 执行");
        }
    }
    
    public interface IScopedService
    {
        void Execute();
    }
    
    public class ScopedService : IScopedService
    {
        public void Execute()
        {
            Console.WriteLine("ScopedService.Execute() 执行");
        }
    }
    
    // 装饰器实现
    public class LoggingServiceDecorator : IService
    {
        private readonly IService _decorated;
        private readonly ILogger<LoggingServiceDecorator> _logger;
        
        public LoggingServiceDecorator(IService decorated, ILogger<LoggingServiceDecorator> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }
        
        public void Execute()
        {
            Console.WriteLine("[LoggingDecorator] 开始执行");
            _decorated.Execute();
            Console.WriteLine("[LoggingDecorator] 执行完成");
        }
    }
    
    public class CachingServiceDecorator : IService
    {
        private readonly IService _decorated;
        
        public CachingServiceDecorator(IService decorated)
        {
            _decorated = decorated;
        }
        
        public void Execute()
        {
            Console.WriteLine("[CachingDecorator] 检查缓存");
            // 模拟缓存检查
            bool cached = false;
            
            if (!cached)
            {
                _decorated.Execute();
                Console.WriteLine("[CachingDecorator] 缓存结果");
            }
            else
            {
                Console.WriteLine("[CachingDecorator] 使用缓存结果");
            }
        }
    }
    
    public class TransactionalServiceDecorator : IService
    {
        private readonly IService _decorated;
        
        public TransactionalServiceDecorator(IService decorated)
        {
            _decorated = decorated;
        }
        
        public void Execute()
        {
            Console.WriteLine("[TransactionalDecorator] 开始事务");
            try
            {
                _decorated.Execute();
                Console.WriteLine("[TransactionalDecorator] 提交事务");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TransactionalDecorator] 回滚事务: {ex.Message}");
                throw;
            }
        }
    }
}
