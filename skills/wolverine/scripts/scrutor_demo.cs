#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Scrutor@4.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ScrutorDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Scrutor 用法演示");
            Console.WriteLine("==================");

            // 构建服务容器
            var services = new ServiceCollection();

            // 演示 1: 自动服务注册
            await DemoAutomaticRegistration(services);

            // 演示 2: 装饰器模式
            await DemoDecoratorPattern(services);

            // 演示 3: 泛型服务注册
            await DemoGenericServices(services);

            // 演示 4: 基于约定的注册
            await DemoConventionBasedRegistration(services);

            // 演示 5: 服务筛选和条件注册
            await DemoServiceFiltering(services);

            // 演示 6: 性能测试
            await DemoPerformanceTesting(services);

            Console.WriteLine("\nScrutor 用法演示完成");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        private static async Task DemoAutomaticRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 演示 1: 自动服务注册 ===");

            // 重置服务容器
            services.Clear();

            // 使用 Scrutor 自动注册服务
            // 注册所有实现了接口的类型，默认生命周期为 Transient
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 测试自动注册的服务
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var productService = serviceProvider.GetRequiredService<IProductService>();

            var user = await userService.GetUserAsync(1);
            var product = await productService.GetProductAsync(1);

            Console.WriteLine($"获取用户: {user.Name}");
            Console.WriteLine($"获取产品: {product.Name}, 价格: {product.Price}");
        }

        private static async Task DemoDecoratorPattern(IServiceCollection services)
        {
            Console.WriteLine("\n=== 演示 2: 装饰器模式 ===");

            // 重置服务容器
            services.Clear();

            // 注册基础服务
            services.AddTransient<IUserService, UserService>();

            // 使用 Scrutor 添加装饰器
            services.Decorate<IUserService, LoggingUserServiceDecorator>();
            services.Decorate<IUserService, CachingUserServiceDecorator>();

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 测试装饰后的服务
            var userService = serviceProvider.GetRequiredService<IUserService>();

            Console.WriteLine("第一次获取用户 (应该缓存):");
            var user1 = await userService.GetUserAsync(1);
            Console.WriteLine($"用户: {user1.Name}");

            Console.WriteLine("\n第二次获取用户 (应该从缓存获取):");
            var user2 = await userService.GetUserAsync(1);
            Console.WriteLine($"用户: {user2.Name}");

            Console.WriteLine("\n更新用户 (应该清除缓存):");
            await userService.UpdateUserAsync(1, "更新后的用户名");

            Console.WriteLine("\n第三次获取用户 (应该重新获取):");
            var user3 = await userService.GetUserAsync(1);
            Console.WriteLine($"用户: {user3.Name}");
        }

        private static async Task DemoGenericServices(IServiceCollection services)
        {
            Console.WriteLine("\n=== 演示 3: 泛型服务注册 ===");

            // 重置服务容器
            services.Clear();

            // 注册泛型服务
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));

            // 使用 Scrutor 添加泛型装饰器
            services.Decorate(typeof(IRepository<>), typeof(LoggingRepositoryDecorator<>));
            services.Decorate(typeof(IRepository<>), typeof(CachingRepositoryDecorator<>));

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 测试泛型服务
            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            var productRepository = serviceProvider.GetRequiredService<IRepository<Product>>();

            Console.WriteLine("测试用户仓库:");
            var user = await userRepository.GetAsync(1);
            Console.WriteLine($"用户: {user.Name}");

            Console.WriteLine("\n测试产品仓库:");
            var product = await productRepository.GetAsync(1);
            Console.WriteLine($"产品: {product.Name}");

            Console.WriteLine("\n测试泛型方法:");
            await userRepository.AddAsync(new User { Id = 2, Name = "新用户" });
            await productRepository.AddAsync(new Product { Id = 2, Name = "新产品", Price = 99.99m });
        }

        private static async Task DemoConventionBasedRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 演示 4: 基于约定的注册 ===");

            // 重置服务容器
            services.Clear();

            // 使用 Scrutor 基于约定注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                // 注册所有以 "Service" 结尾的类
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                // 注册所有以 "Repository" 结尾的类
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                // 注册所有以 "Provider" 结尾的类
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Provider")))
                .AsSelf()
                .WithSingletonLifetime()
            );

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 测试基于约定注册的服务
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var productService = serviceProvider.GetRequiredService<IProductService>();
            var configProvider = serviceProvider.GetRequiredService<ConfigProvider>();

            var user = await userService.GetUserAsync(1);
            var product = await productService.GetProductAsync(1);
            var config = configProvider.GetConfig();

            Console.WriteLine($"获取用户: {user.Name}");
            Console.WriteLine($"获取产品: {product.Name}");
            Console.WriteLine($"获取配置: {config}");
        }

        private static async Task DemoServiceFiltering(IServiceCollection services)
        {
            Console.WriteLine("\n=== 演示 5: 服务筛选和条件注册 ===");

            // 重置服务容器
            services.Clear();

            // 使用 Scrutor 进行服务筛选和条件注册
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                // 只注册实现了特定接口的类
                .AddClasses(classes => classes.Where(c => c.GetInterfaces().Any(i => i.Name == "IUserService" || i.Name == "IProductService")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                // 注册带有特定属性的类
                .AddClasses(classes => classes.Where(c => c.GetCustomAttributes<RegisterAsSingletonAttribute>().Any()))
                .AsSelf()
                .WithSingletonLifetime()
            );

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 测试筛选后的服务
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var productService = serviceProvider.GetRequiredService<IProductService>();
            var singletonService = serviceProvider.GetRequiredService<SingletonService>();

            var user = await userService.GetUserAsync(1);
            var product = await productService.GetProductAsync(1);
            var singletonValue = singletonService.GetValue();

            Console.WriteLine($"获取用户: {user.Name}");
            Console.WriteLine($"获取产品: {product.Name}");
            Console.WriteLine($"获取单例服务值: {singletonValue}");
        }

        private static async Task DemoPerformanceTesting(IServiceCollection services)
        {
            Console.WriteLine("\n=== 演示 6: 性能测试 ===");

            // 重置服务容器
            services.Clear();

            // 注册大量服务进行性能测试
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            var stopwatch = new Stopwatch();

            // 测试服务注册性能
            stopwatch.Start();
            using var serviceProvider = services.BuildServiceProvider();
            stopwatch.Stop();

            Console.WriteLine($"服务容器构建时间: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 测试服务解析性能
            stopwatch.Restart();
            for (int i = 0; i < 1000; i++)
            {
                var userService = serviceProvider.GetRequiredService<IUserService>();
                var productService = serviceProvider.GetRequiredService<IProductService>();
            }
            stopwatch.Stop();

            Console.WriteLine($"1000 次服务解析时间: {stopwatch.ElapsedMilliseconds} 毫秒");

            // 测试装饰器性能
            stopwatch.Restart();
            var decoratedUserService = serviceProvider.GetRequiredService<IUserService>();
            for (int i = 0; i < 100; i++)
            {
                await decoratedUserService.GetUserAsync(1);
            }
            stopwatch.Stop();

            Console.WriteLine($"100 次装饰器方法调用时间: {stopwatch.ElapsedMilliseconds} 毫秒");
        }
    }

    // 服务接口
    public interface IUserService
    {
        Task<User> GetUserAsync(int id);
        Task UpdateUserAsync(int id, string name);
    }

    public interface IProductService
    {
        Task<Product> GetProductAsync(int id);
    }

    public interface IRepository<T>
    {
        Task<T> GetAsync(int id);
        Task AddAsync(T entity);
    }

    // 服务实现
    public class UserService : IUserService
    {
        public async Task<User> GetUserAsync(int id)
        {
            await Task.Delay(10); // 模拟数据库操作
            return new User { Id = id, Name = "用户" + id };
        }

        public async Task UpdateUserAsync(int id, string name)
        {
            await Task.Delay(10); // 模拟数据库操作
            Console.WriteLine($"更新用户: {id}, 新名称: {name}");
        }
    }

    public class ProductService : IProductService
    {
        public async Task<Product> GetProductAsync(int id)
        {
            await Task.Delay(10); // 模拟数据库操作
            return new Product { Id = id, Name = "产品" + id, Price = id * 10.99m };
        }
    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        public async Task<T> GetAsync(int id)
        {
            await Task.Delay(10); // 模拟数据库操作
            return new T();
        }

        public async Task AddAsync(T entity)
        {
            await Task.Delay(10); // 模拟数据库操作
            Console.WriteLine($"添加实体: {typeof(T).Name}");
        }
    }

    // 装饰器实现
    public class LoggingUserServiceDecorator : IUserService
    {
        private readonly IUserService _decorated;

        public LoggingUserServiceDecorator(IUserService decorated)
        {
            _decorated = decorated;
        }

        public async Task<User> GetUserAsync(int id)
        {
            Console.WriteLine($"[日志] 获取用户: {id}");
            var result = await _decorated.GetUserAsync(id);
            Console.WriteLine($"[日志] 获取用户完成: {result.Name}");
            return result;
        }

        public async Task UpdateUserAsync(int id, string name)
        {
            Console.WriteLine($"[日志] 更新用户: {id}, 新名称: {name}");
            await _decorated.UpdateUserAsync(id, name);
            Console.WriteLine($"[日志] 更新用户完成: {id}");
        }
    }

    public class CachingUserServiceDecorator : IUserService
    {
        private readonly IUserService _decorated;
        private readonly Dictionary<int, User> _cache = new();

        public CachingUserServiceDecorator(IUserService decorated)
        {
            _decorated = decorated;
        }

        public async Task<User> GetUserAsync(int id)
        {
            if (_cache.TryGetValue(id, out var user))
            {
                Console.WriteLine($"[缓存] 从缓存获取用户: {id}");
                return user;
            }

            var result = await _decorated.GetUserAsync(id);
            _cache[id] = result;
            Console.WriteLine($"[缓存] 将用户添加到缓存: {id}");
            return result;
        }

        public async Task UpdateUserAsync(int id, string name)
        {
            await _decorated.UpdateUserAsync(id, name);
            _cache.Remove(id);
            Console.WriteLine($"[缓存] 从缓存移除用户: {id}");
        }
    }

    public class LoggingRepositoryDecorator<T> : IRepository<T> where T : class
    {
        private readonly IRepository<T> _decorated;

        public LoggingRepositoryDecorator(IRepository<T> decorated)
        {
            _decorated = decorated;
        }

        public async Task<T> GetAsync(int id)
        {
            Console.WriteLine($"[日志] 获取 {typeof(T).Name}: {id}");
            var result = await _decorated.GetAsync(id);
            Console.WriteLine($"[日志] 获取 {typeof(T).Name} 完成");
            return result;
        }

        public async Task AddAsync(T entity)
        {
            Console.WriteLine($"[日志] 添加 {typeof(T).Name}");
            await _decorated.AddAsync(entity);
            Console.WriteLine($"[日志] 添加 {typeof(T).Name} 完成");
        }
    }

    public class CachingRepositoryDecorator<T> : IRepository<T> where T : class
    {
        private readonly IRepository<T> _decorated;
        private readonly Dictionary<int, T> _cache = new();

        public CachingRepositoryDecorator(IRepository<T> decorated)
        {
            _decorated = decorated;
        }

        public async Task<T> GetAsync(int id)
        {
            if (_cache.TryGetValue(id, out var entity))
            {
                Console.WriteLine($"[缓存] 从缓存获取 {typeof(T).Name}: {id}");
                return entity;
            }

            var result = await _decorated.GetAsync(id);
            _cache[id] = result;
            Console.WriteLine($"[缓存] 将 {typeof(T).Name} 添加到缓存: {id}");
            return result;
        }

        public async Task AddAsync(T entity)
        {
            await _decorated.AddAsync(entity);
            Console.WriteLine($"[缓存] 清除 {typeof(T).Name} 缓存");
        }
    }

    // 其他服务
    public class ConfigProvider
    {
        public string GetConfig()
        {
            return "应用配置";
        }
    }

    [RegisterAsSingleton]
    public class SingletonService
    {
        public string GetValue()
        {
            return "单例服务值";
        }
    }

    // 特性
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsSingletonAttribute : Attribute
    {
    }

    // 实体类
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
