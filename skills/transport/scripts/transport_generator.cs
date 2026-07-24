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

namespace TransportSkill.ScrutorDemo
{
    public interface ITransportService
    {
        Task<string> SendAsync(string message);
        Task<string> ReceiveAsync();
    }

    public class DefaultTransportService : ITransportService
    {
        private readonly ILogger<DefaultTransportService> _logger;

        public DefaultTransportService(ILogger<DefaultTransportService> logger)
        {
            _logger = logger;
        }

        public async Task<string> SendAsync(string message)
        {
            _logger.LogInformation($"DefaultTransportService: 发送消息: {message}");
            await Task.Delay(100);
            return $"已发送: {message}";
        }

        public async Task<string> ReceiveAsync()
        {
            _logger.LogInformation("DefaultTransportService: 接收消息");
            await Task.Delay(100);
            return "默认接收消息";
        }
    }

    public class LoggingTransportService : ITransportService
    {
        private readonly ITransportService _inner;
        private readonly ILogger<LoggingTransportService> _logger;

        public LoggingTransportService(ITransportService inner, ILogger<LoggingTransportService> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> SendAsync(string message)
        {
            _logger.LogInformation("LoggingTransportService: 开始发送");
            var result = await _inner.SendAsync(message);
            _logger.LogInformation($"LoggingTransportService: 发送完成: {result}");
            return result;
        }

        public async Task<string> ReceiveAsync()
        {
            _logger.LogInformation("LoggingTransportService: 开始接收");
            var result = await _inner.ReceiveAsync();
            _logger.LogInformation($"LoggingTransportService: 接收完成: {result}");
            return result;
        }
    }

    public class CachingTransportService : ITransportService
    {
        private readonly ITransportService _inner;
        private readonly Dictionary<string, string> _cache = new();
        private readonly ILogger<CachingTransportService> _logger;

        public CachingTransportService(ITransportService inner, ILogger<CachingTransportService> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> SendAsync(string message)
        {
            if (_cache.TryGetValue(message, out var cachedResult))
            {
                _logger.LogInformation($"CachingTransportService: 缓存命中: {message}");
                return cachedResult;
            }

            _logger.LogInformation($"CachingTransportService: 缓存未命中: {message}");
            var result = await _inner.SendAsync(message);
            _cache[message] = result;
            return result;
        }

        public async Task<string> ReceiveAsync()
        {
            return await _inner.ReceiveAsync();
        }
    }

    public interface IValidatorService
    {
        bool Validate(string data);
    }

    public class DefaultValidatorService : IValidatorService
    {
        public bool Validate(string data)
        {
            Console.WriteLine($"DefaultValidatorService: 验证数据: {data}");
            return !string.IsNullOrEmpty(data);
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

    public class TransportGeneratorService
    {
        public void DemonstrateBasicRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 1. 基本服务注册 ===");
            services.AddTransient<ITransportService, DefaultTransportService>();
            services.AddTransient<IValidatorService, DefaultValidatorService>();
        }

        public void DemonstrateDecoratorPattern(IServiceCollection services)
        {
            Console.WriteLine("\n=== 2. 装饰器模式 ===");
            services.Decorate<ITransportService, LoggingTransportService>();
            services.Decorate<ITransportService, CachingTransportService>();
        }

        public void DemonstrateServiceFiltering(IServiceCollection services)
        {
            Console.WriteLine("\n=== 3. 服务过滤 ===");
            services.AddTransient<IValidatorService, DefaultValidatorService>();
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
            services.AddTransient<ITransportService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DefaultTransportService>>();
                Console.WriteLine("使用工厂方法创建 DefaultTransportService");
                return new DefaultTransportService(logger);
            });
        }

        public void DemonstrateAssemblyScanning(IServiceCollection services)
        {
            Console.WriteLine("\n=== 6. 程序集扫描 ===");
            services.Scan(scan => scan
                .FromAssemblyOf<ITransportService>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );
        }

        public void DemonstrateMultipleDecorators(IServiceCollection services)
        {
            Console.WriteLine("\n=== 7. 多个装饰器 ===");
            services.AddTransient<ITransportService, DefaultTransportService>();
            services.Decorate<ITransportService, LoggingTransportService>();
            services.Decorate<ITransportService, CachingTransportService>();
        }

        public void DemonstrateConditionalRegistration(IServiceCollection services)
        {
            Console.WriteLine("\n=== 8. 条件注册 ===");
            bool useAdvanced = true;
            if (useAdvanced)
            {
                services.AddTransient<ITransportService, DefaultTransportService>();
                Console.WriteLine("注册了 DefaultTransportService");
            }
            else
            {
                services.AddTransient<ITransportService>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<DefaultTransportService>>();
                    Console.WriteLine("使用工厂方法创建 DefaultTransportService");
                    return new DefaultTransportService(logger);
                });
            }
        }

        public async Task RunAllDemosAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine("\n=== 运行所有演示 ===");

            // 测试传输服务
            var transportService = serviceProvider.GetRequiredService<ITransportService>();
            Console.WriteLine("\n测试传输服务:");
            var sendResult1 = await transportService.SendAsync("Hello World");
            Console.WriteLine($"发送结果1: {sendResult1}");
            var sendResult2 = await transportService.SendAsync("Hello World");
            Console.WriteLine($"发送结果2: {sendResult2}");
            var receiveResult = await transportService.ReceiveAsync();
            Console.WriteLine($"接收结果: {receiveResult}");

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

            // 测试验证服务
            var validatorService = serviceProvider.GetRequiredService<IValidatorService>();
            Console.WriteLine("\n测试验证服务:");
            var isValid1 = validatorService.Validate("test data");
            Console.WriteLine($"验证结果1: {isValid1}");
            var isValid2 = validatorService.Validate("");
            Console.WriteLine($"验证结果2: {isValid2}");

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

                var generatorService = new TransportGeneratorService();

                switch (type.ToLower())
                {
                    case "basic":
                        generatorService.DemonstrateBasicRegistration(services);
                        break;
                    case "decorator":
                        generatorService.DemonstrateBasicRegistration(services);
                        generatorService.DemonstrateDecoratorPattern(services);
                        break;
                    case "filtering":
                        generatorService.DemonstrateServiceFiltering(services);
                        break;
                    case "lifetime":
                        generatorService.DemonstrateLifetimeManagement(services);
                        break;
                    case "advanced":
                        generatorService.DemonstrateAdvancedRegistration(services);
                        break;
                    case "scanning":
                        generatorService.DemonstrateAssemblyScanning(services);
                        break;
                    case "multiple":
                        generatorService.DemonstrateMultipleDecorators(services);
                        break;
                    case "conditional":
                        generatorService.DemonstrateConditionalRegistration(services);
                        break;
                    case "all":
                    default:
                        generatorService.DemonstrateBasicRegistration(services);
                        generatorService.DemonstrateDecoratorPattern(services);
                        generatorService.DemonstrateServiceFiltering(services);
                        generatorService.DemonstrateLifetimeManagement(services);
                        generatorService.DemonstrateAdvancedRegistration(services);
                        generatorService.DemonstrateAssemblyScanning(services);
                        generatorService.DemonstrateMultipleDecorators(services);
                        generatorService.DemonstrateConditionalRegistration(services);
                        break;
                }

                var serviceProvider = services.BuildServiceProvider();
                await generatorService.RunAllDemosAsync(serviceProvider);
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
