#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Scrutor@4.2.2
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ===============================
// Scrutor 用法示例
// ===============================

namespace Util.ScrutorDemo
{
    // ===============================
    // 服务接口定义
    // ===============================

    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task SaveAsync(T entity);
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
    }

    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> GetBySkuAsync(string sku);
    }

    public interface IService
    {
        Task<string> GetServiceName();
    }

    public interface IUserService : IService
    {
        Task<User> GetUserAsync(int id);
        Task CreateUserAsync(User user);
    }

    public interface IProductService : IService
    {
        Task<Product> GetProductAsync(int id);
        Task CreateProductAsync(Product product);
    }

    public interface ILoggerService
    {
        void Log(string message);
    }

    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value);
    }

    // ===============================
    // 实体类
    // ===============================

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
    }

    // ===============================
    // 服务实现
    // ===============================

    public class UserRepository : IUserRepository
    {
        public async Task<User> GetByIdAsync(int id)
        {
            Console.WriteLine($"UserRepository.GetByIdAsync: {id}");
            return await Task.FromResult(new User { Id = id, Name = "用户" + id, Email = "user" + id + "@example.com" });
        }

        public async Task SaveAsync(User entity)
        {
            Console.WriteLine($"UserRepository.SaveAsync: {entity.Id}, {entity.Name}");
            await Task.CompletedTask;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            Console.WriteLine($"UserRepository.GetByEmailAsync: {email}");
            return await Task.FromResult(new User { Id = 1, Name = "用户", Email = email });
        }
    }

    public class ProductRepository : IProductRepository
    {
        public async Task<Product> GetByIdAsync(int id)
        {
            Console.WriteLine($"ProductRepository.GetByIdAsync: {id}");
            return await Task.FromResult(new Product { Id = id, Name = "产品" + id, Sku = "SKU" + id, Price = id * 100 });
        }

        public async Task SaveAsync(Product entity)
        {
            Console.WriteLine($"ProductRepository.SaveAsync: {entity.Id}, {entity.Name}");
            await Task.CompletedTask;
        }

        public async Task<Product> GetBySkuAsync(string sku)
        {
            Console.WriteLine($"ProductRepository.GetBySkuAsync: {sku}");
            return await Task.FromResult(new Product { Id = 1, Name = "产品", Sku = sku, Price = 100 });
        }
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GetServiceName()
        {
            return await Task.FromResult("UserService");
        }

        public async Task<User> GetUserAsync(int id)
        {
            Console.WriteLine($"UserService.GetUserAsync: {id}");
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task CreateUserAsync(User user)
        {
            Console.WriteLine($"UserService.CreateUserAsync: {user.Name}");
            await _userRepository.SaveAsync(user);
        }
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<string> GetServiceName()
        {
            return await Task.FromResult("ProductService");
        }

        public async Task<Product> GetProductAsync(int id)
        {
            Console.WriteLine($"ProductService.GetProductAsync: {id}");
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            Console.WriteLine($"ProductService.CreateProductAsync: {product.Name}");
            await _productRepository.SaveAsync(product);
        }
    }

    public class LoggerService : ILoggerService
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }
    }

    public class CacheService : ICacheService
    {
        public async Task<T> GetAsync<T>(string key)
        {
            Console.WriteLine($"CacheService.GetAsync: {key}");
            return await Task.FromResult(default(T));
        }

        public async Task SetAsync<T>(string key, T value)
        {
            Console.WriteLine($"CacheService.SetAsync: {key}, {value}");
            await Task.CompletedTask;
        }
    }

    // ===============================
    // 装饰器类
    // ===============================

    public class LoggingRepositoryDecorator<T> : IRepository<T>
    {
        private readonly IRepository<T> _inner;
        private readonly ILoggerService _logger;

        public LoggingRepositoryDecorator(IRepository<T> inner, ILoggerService logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            _logger.Log($"开始获取 {typeof(T).Name}，ID: {id}");
            var result = await _inner.GetByIdAsync(id);
            _logger.Log($"完成获取 {typeof(T).Name}，ID: {id}");
            return result;
        }

        public async Task SaveAsync(T entity)
        {
            _logger.Log($"开始保存 {typeof(T).Name}");
            await _inner.SaveAsync(entity);
            _logger.Log($"完成保存 {typeof(T).Name}");
        }
    }

    public class CachingUserServiceDecorator : IUserService
    {
        private readonly IUserService _inner;
        private readonly ICacheService _cache;

        public CachingUserServiceDecorator(IUserService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<string> GetServiceName()
        {
            return await _inner.GetServiceName();
        }

        public async Task<User> GetUserAsync(int id)
        {
            var cacheKey = $"user:{id}";
            var cachedUser = await _cache.GetAsync<User>(cacheKey);
            
            if (cachedUser != null)
            {
                Console.WriteLine($"从缓存获取用户: {id}");
                return cachedUser;
            }

            var user = await _inner.GetUserAsync(id);
            await _cache.SetAsync(cacheKey, user);
            Console.WriteLine($"将用户存入缓存: {id}");
            return user;
        }

        public async Task CreateUserAsync(User user)
        {
            await _inner.CreateUserAsync(user);
            var cacheKey = $"user:{user.Id}";
            await _cache.SetAsync(cacheKey, user);
            Console.WriteLine($"将新用户存入缓存: {user.Id}");
        }
    }

    public class LoggingServiceDecorator<T> : T where T : class, IService
    {
        private readonly T _inner;
        private readonly ILoggerService _logger;

        public LoggingServiceDecorator(T inner, ILoggerService logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> GetServiceName()
        {
            _logger.Log($"开始获取服务名称");
            var name = await _inner.GetServiceName();
            _logger.Log($"完成获取服务名称: {name}");
            return name;
        }
    }

    // ===============================
    // Scrutor 注册扩展
    // ===============================

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // 方式1: 直接注册
            // services.AddSingleton<IUserRepository, UserRepository>();
            // services.AddSingleton<IProductRepository, ProductRepository>();

            // 方式2: 使用Scrutor自动注册
            services.Scan(scan => scan
                .FromAssemblyOf<IRepository<User>>()
                .AddClasses(classes => classes.AssignableTo<IRepository<>>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // 使用Scrutor自动注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<IService>()
                .AddClasses(classes => classes.AssignableTo<IService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<ICacheService, CacheService>();
            return services;
        }

        public static IServiceCollection AddDecorators(this IServiceCollection services)
        {
            // 注册装饰器
            services.Decorate(typeof(IRepository<>), typeof(LoggingRepositoryDecorator<>));
            services.Decorate<IUserService, CachingUserServiceDecorator>();
            services.Decorate(typeof(IService), typeof(LoggingServiceDecorator<>));

            return services;
        }
    }

    // ===============================
    // 主程序
    // ===============================

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Scrutor 用法示例程序");
            Console.WriteLine("=" * 50);

            // 测试基本注册
            await TestBasicRegistration();

            // 测试装饰器模式
            await TestDecoratorPattern();

            // 测试高级注册
            await TestAdvancedRegistration();

            Console.WriteLine("\nScrutor 用法示例程序完成！");
        }

        private static async Task TestBasicRegistration()
        {
            Console.WriteLine("\n1. 测试基本注册:");

            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddConsole());
            
            // 方式1: 传统注册
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IProductService, ProductService>();

            var serviceProvider = services.BuildServiceProvider();

            // 测试服务
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var productService = serviceProvider.GetRequiredService<IProductService>();

            var user = await userService.GetUserAsync(1);
            Console.WriteLine($"获取用户: {user.Id}, {user.Name}, {user.Email}");

            var product = await productService.GetProductAsync(1);
            Console.WriteLine($"获取产品: {product.Id}, {product.Name}, {product.Sku}, {product.Price}");
        }

        private static async Task TestDecoratorPattern()
        {
            Console.WriteLine("\n2. 测试装饰器模式:");

            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddConsole());

            // 注册基础服务
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<ICacheService, CacheService>();

            // 注册装饰器
            services.Decorate<IUserService, CachingUserServiceDecorator>();
            services.Decorate(typeof(IRepository<>), typeof(LoggingRepositoryDecorator<>));

            var serviceProvider = services.BuildServiceProvider();

            // 测试装饰器
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

            Console.WriteLine("\n测试用户服务装饰器:");
            var user1 = await userService.GetUserAsync(1);
            Console.WriteLine($"获取用户: {user1.Id}, {user1.Name}");

            // 再次获取，应该从缓存获取
            var user2 = await userService.GetUserAsync(1);
            Console.WriteLine($"再次获取用户: {user2.Id}, {user2.Name}");

            Console.WriteLine("\n测试仓库装饰器:");
            var user3 = await userRepository.GetByIdAsync(2);
            Console.WriteLine($"获取用户: {user3.Id}, {user3.Name}");
        }

        private static async Task TestAdvancedRegistration()
        {
            Console.WriteLine("\n3. 测试高级注册:");

            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddConsole());

            // 使用扩展方法注册所有服务
            services.AddInfrastructure();
            services.AddRepositories();
            services.AddServices();
            services.AddDecorators();

            var serviceProvider = services.BuildServiceProvider();

            // 测试所有服务
            Console.WriteLine("\n测试用户服务:");
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var user = await userService.GetUserAsync(3);
            Console.WriteLine($"获取用户: {user.Id}, {user.Name}");
            
            var serviceName = await userService.GetServiceName();
            Console.WriteLine($"服务名称: {serviceName}");

            Console.WriteLine("\n测试产品服务:");
            var productService = serviceProvider.GetRequiredService<IProductService>();
            var product = await productService.GetProductAsync(3);
            Console.WriteLine($"获取产品: {product.Id}, {product.Name}, {product.Price}");
            
            var productServiceName = await productService.GetServiceName();
            Console.WriteLine($"服务名称: {productServiceName}");

            // 测试泛型仓库
            Console.WriteLine("\n测试泛型仓库:");
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var userByEmail = await userRepository.GetByEmailAsync("test@example.com");
            Console.WriteLine($"通过邮箱获取用户: {userByEmail.Id}, {userByEmail.Email}");

            var productRepository = serviceProvider.GetRequiredService<IProductRepository>();
            var productBySku = await productRepository.GetBySkuAsync("SKU123");
            Console.WriteLine($"通过SKU获取产品: {productBySku.Id}, {productBySku.Sku}, {productBySku.Price}");
        }
    }
}
