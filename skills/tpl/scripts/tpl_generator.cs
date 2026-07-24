#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0-beta4.22272.1
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:package Microsoft.Extensions.Logging.Console@8.0.0
#:package Scrutor@4.2.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TPLSkill.ScrutorDemo
{
    public interface ILoggerService
    {
        void Log(string message);
    }

    public class ConsoleLoggerService : ILoggerService
    {
        private readonly ILogger<ConsoleLoggerService> _logger;

        public ConsoleLoggerService(ILogger<ConsoleLoggerService> logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            _logger.LogInformation($"[ConsoleLogger] {message}");
            Console.WriteLine($"Console: {message}");
        }
    }

    public interface IDataService
    {
        string GetData();
    }

    public class DefaultDataService : IDataService
    {
        public string GetData()
        {
            return "Default data from DefaultDataService";
        }
    }

    public class CachedDataService : IDataService
    {
        private readonly IDataService _inner;
        private string _cachedData;

        public CachedDataService(IDataService inner)
        {
            _inner = inner;
        }

        public string GetData()
        {
            if (_cachedData == null)
            {
                Console.WriteLine("Cache miss, fetching data...");
                _cachedData = _inner.GetData();
            }
            else
            {
                Console.WriteLine("Cache hit, returning cached data...");
            }
            return _cachedData;
        }
    }

    public class LoggingDataService : IDataService
    {
        private readonly IDataService _inner;
        private readonly ILoggerService _logger;

        public LoggingDataService(IDataService inner, ILoggerService logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public string GetData()
        {
            _logger.Log("Getting data from LoggingDataService");
            var data = _inner.GetData();
            _logger.Log($"Got data: {data}");
            return data;
        }
    }

    public interface IRepository<T>
    {
        void Save(T item);
        T Get(int id);
    }

    public class User { public int Id { get; set; } public string Name { get; set; } }

    public class UserRepository : IRepository<User>
    {
        private readonly Dictionary<int, User> _users = new();

        public void Save(User item)
        {
            _users[item.Id] = item;
            Console.WriteLine($"Saved user: {item.Name}");
        }

        public User Get(int id)
        {
            _users.TryGetValue(id, out var user);
            return user;
        }
    }

    public interface IScopedService { string GetId(); }
    public interface ISingletonService { string GetId(); }
    public interface ITransientService { string GetId(); }

    public class ScopedService : IScopedService
    {
        private readonly string _id = Guid.NewGuid().ToString();
        public string GetId() => _id;
    }

    public class SingletonService : ISingletonService
    {
        private readonly string _id = Guid.NewGuid().ToString();
        public string GetId() => _id;
    }

    public class TransientService : ITransientService
    {
        private readonly string _id = Guid.NewGuid().ToString();
        public string GetId() => _id;
    }

    public class ScrutorDemoService
    {
        public void DemonstrateBasicRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 1. 基本服务注册 ===");
            services.AddTransient<ILoggerService, ConsoleLoggerService>();
            services.AddTransient<IDataService, DefaultDataService>();
        }

        public void DemonstrateDecoratorPattern(IServiceCollection services)
        {
            Console.WriteLine("\n=== 2. 装饰器模式 ===");
            services.Decorate<IDataService, CachedDataService>();
            services.Decorate<IDataService, LoggingDataService>();
        }

        public void DemonstrateServiceFiltering(IServiceCollection services)
        {
            Console.WriteLine("\n=== 3. 服务过滤 ===");
            services.AddTransient<IRepository<User>, UserRepository>();
        }

        public void DemonstrateLifetimeManagement(IServiceCollection services)
        {
            Console.WriteLine("\n=== 4. 生命周期管理 ===");
            services.AddScoped<IScopedService, ScopedService>();
            services.AddSingleton<ISingletonService, SingletonService>();
            services.AddTransient<ITransientService, TransientService>();
        }

        public void DemonstrateAdvancedRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 5. 高级注册 ===");
            services.AddTransient<IDataService>(provider =>
            {
                var logger = provider.GetRequiredService<ILoggerService>();
                logger.Log("Creating custom data service");
                return new DefaultDataService();
            });
        }

        public void DemonstrateAssemblyScanning(IServiceCollection services)
        {
            Console.WriteLine("\n=== 6. 程序集扫描 ===");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
        }

        public void DemonstrateMultipleDecorators(IServiceCollection services)
        {
            Console.WriteLine("\n=== 7. 多个装饰器 ===");
            services.AddTransient<IDataService, DefaultDataService>();
            services.Decorate<IDataService, CachedDataService>();
            services.Decorate<IDataService, LoggingDataService>();
        }

        public void DemonstrateConditionalRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 8. 条件注册 ===");
            bool useAdvancedService = true;
            if (useAdvancedService)
            {
                services.AddTransient<IDataService, DefaultDataService>();
            }
            else
            {
                services.AddTransient<IDataService>(provider =>
                {
                    var logger = provider.GetRequiredService<ILoggerService>();
                    logger.Log("Using simple data service");
                    return new DefaultDataService();
                });
            }
        }

        public async Task RunAllDemosAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("\n=== 运行所有演示 ===");

            // 测试基本服务
            var dataService = serviceProvider.GetRequiredService<IDataService>();
            Console.WriteLine("\n测试数据服务:");
            Console.WriteLine(dataService.GetData());
            Console.WriteLine(dataService.GetData()); // 测试缓存

            // 测试生命周期
            using (var scope1 = serviceProvider.CreateScope())
            {
                var scoped1 = scope1.ServiceProvider.GetRequiredService<IScopedService>();
                var singleton1 = scope1.ServiceProvider.GetRequiredService<ISingletonService>();
                var transient1 = scope1.ServiceProvider.GetRequiredService<ITransientService>();
                var transient2 = scope1.ServiceProvider.GetRequiredService<ITransientService>();

                Console.WriteLine("\n测试生命周期 - Scope 1:");
                Console.WriteLine($"Scoped: {scoped1.GetId()}");
                Console.WriteLine($"Singleton: {singleton1.GetId()}");
                Console.WriteLine($"Transient 1: {transient1.GetId()}");
                Console.WriteLine($"Transient 2: {transient2.GetId()}");
            }

            using (var scope2 = serviceProvider.CreateScope())
            {
                var scoped2 = scope2.ServiceProvider.GetRequiredService<IScopedService>();
                var singleton2 = scope2.ServiceProvider.GetRequiredService<ISingletonService>();
                var transient3 = scope2.ServiceProvider.GetRequiredService<ITransientService>();

                Console.WriteLine("\n测试生命周期 - Scope 2:");
                Console.WriteLine($"Scoped: {scoped2.GetId()}");
                Console.WriteLine($"Singleton: {singleton2.GetId()}");
                Console.WriteLine($"Transient 3: {transient3.GetId()}");
            }

            // 测试仓储
            using (var scope = serviceProvider.CreateScope())
            {
                var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
                var user = new User { Id = 1, Name = "John Doe" };
                userRepo.Save(user);
                var savedUser = userRepo.Get(1);
                Console.WriteLine($"\n测试仓储 - 保存的用户: {savedUser?.Name}");
            }

            Console.WriteLine("\n=== 所有演示完成 ===");
        }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Scrutor演示 - 依赖注入装饰器模式示例");

            var demoCommand = new Command("demo", "运行Scrutor演示");
            var demoTypeOption = new Option<string>("--type", getDefaultValue: () => "all", description: "演示类型: all, basic, decorator, filtering, lifetime, advanced, scanning, multiple, conditional");
            demoCommand.AddOption(demoTypeOption);

            demoCommand.SetHandler(async (type) =>
            {
                var services = new ServiceCollection();
                services.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                });

                var scrutorDemo = new ScrutorDemoService();

                switch (type.ToLower())
                {
                    case "basic":
                        scrutorDemo.DemonstrateBasicRegistration(services);
                        break;
                    case "decorator":
                        scrutorDemo.DemonstrateBasicRegistration(services);
                        scrutorDemo.DemonstrateDecoratorPattern(services);
                        break;
                    case "filtering":
                        scrutorDemo.DemonstrateServiceFiltering(services);
                        break;
                    case "lifetime":
                        scrutorDemo.DemonstrateLifetimeManagement(services);
                        break;
                    case "advanced":
                        scrutorDemo.DemonstrateAdvancedRegistration(services);
                        break;
                    case "scanning":
                        scrutorDemo.DemonstrateAssemblyScanning(services);
                        break;
                    case "multiple":
                        scrutorDemo.DemonstrateMultipleDecorators(services);
                        break;
                    case "conditional":
                        scrutorDemo.DemonstrateConditionalRegistration(services);
                        break;
                    case "all":
                    default:
                        scrutorDemo.DemonstrateBasicRegistration(services);
                        scrutorDemo.DemonstrateDecoratorPattern(services);
                        scrutorDemo.DemonstrateServiceFiltering(services);
                        scrutorDemo.DemonstrateLifetimeManagement(services);
                        scrutorDemo.DemonstrateAdvancedRegistration(services);
                        scrutorDemo.DemonstrateAssemblyScanning(services);
                        scrutorDemo.DemonstrateMultipleDecorators(services);
                        scrutorDemo.DemonstrateConditionalRegistration(services);
                        break;
                }

                var serviceProvider = services.BuildServiceProvider();
                await scrutorDemo.RunAllDemosAsync(serviceProvider);
            }, demoTypeOption);

            var infoCommand = new Command("info", "显示Scrutor信息");
            infoCommand.SetHandler(() =>
            {
                Console.WriteLine("Scrutor 演示工具");
                Console.WriteLine("版本: 1.0.0");
                Console.WriteLine("描述: 展示Scrutor库的各种用法");
                Console.WriteLine("\n可用的演示类型:");
                Console.WriteLine("  basic      - 基本服务注册");
                Console.WriteLine("  decorator  - 装饰器模式");
                Console.WriteLine("  filtering  - 服务过滤");
                Console.WriteLine("  lifetime   - 生命周期管理");
                Console.WriteLine("  advanced   - 高级注册");
                Console.WriteLine("  scanning   - 程序集扫描");
                Console.WriteLine("  multiple   - 多个装饰器");
                Console.WriteLine("  conditional - 条件注册");
                Console.WriteLine("  all        - 运行所有演示");
            });

            rootCommand.AddCommand(demoCommand);
            rootCommand.AddCommand(infoCommand);

            var parser = new CommandLineBuilder(rootCommand)
                .UseDefaults()
                .Build();

            return await parser.InvokeAsync(args);
        }
    }
}
