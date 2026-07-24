#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
#:package Microsoft.Extensions.Configuration.Json@8.0.0
#:package Scrutor@4.2.2
#:package System.Text.Json@8.0.0
#:property LangVersion=preview
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
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scrutor.Core
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Scrutor 依赖注入扩展工具");
            
            // 创建扫描命令
            var scanCommand = new Command("scan", "扫描程序集并注册服务");
            var assemblyOption = new Option<string>("--assembly", "要扫描的程序集路径");
            var patternOption = new Option<string>("--pattern", () => "*", "类型匹配模式");
            var lifetimeOption = new Option<string>("--lifetime", () => "scoped", "服务生命周期 (singleton/transient/scoped)");
            scanCommand.AddOption(assemblyOption);
            scanCommand.AddOption(patternOption);
            scanCommand.AddOption(lifetimeOption);
            scanCommand.SetHandler(async (context) =>
            {
                var assembly = context.ParseResult.GetValueForOption(assemblyOption);
                var pattern = context.ParseResult.GetValueForOption(patternOption);
                var lifetime = context.ParseResult.GetValueForOption(lifetimeOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleScanCommand(serviceProvider, assembly, pattern, lifetime, cancellationToken);
            });
            
            // 创建装饰命令
            var decorateCommand = new Command("decorate", "为服务添加装饰器");
            var serviceOption = new Option<string>("--service", "服务类型");
            var decoratorOption = new Option<string>("--decorator", "装饰器类型");
            var decorateLifetimeOption = new Option<string>("--lifetime", () => "scoped", "服务生命周期 (singleton/transient/scoped)");
            decorateCommand.AddOption(serviceOption);
            decorateCommand.AddOption(decoratorOption);
            decorateCommand.AddOption(decorateLifetimeOption);
            decorateCommand.SetHandler(async (context) =>
            {
                var service = context.ParseResult.GetValueForOption(serviceOption);
                var decorator = context.ParseResult.GetValueForOption(decoratorOption);
                var lifetime = context.ParseResult.GetValueForOption(decorateLifetimeOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleDecorateCommand(serviceProvider, service, decorator, lifetime, cancellationToken);
            });
            
            // 创建列表命令
            var listCommand = new Command("list", "列出已注册的服务");
            var formatOption = new Option<string>("--format", () => "text", "输出格式 (text/json)");
            listCommand.AddOption(formatOption);
            listCommand.SetHandler(async (context) =>
            {
                var format = context.ParseResult.GetValueForOption(formatOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleListCommand(serviceProvider, format, cancellationToken);
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(scanCommand);
            rootCommand.AddCommand(decorateCommand);
            rootCommand.AddCommand(listCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 配置配置管理
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            services.AddSingleton<IConfiguration>(configuration);
            
            // 注册服务
            services.AddSingleton<IAssemblyScanner, AssemblyScanner>();
            services.AddSingleton<IServiceDecorator, ServiceDecorator>();
            services.AddSingleton<IServiceRegistry, ServiceRegistry>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleScanCommand(IServiceProvider serviceProvider, string assemblyPath, string pattern, string lifetime, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                Console.WriteLine("错误: 必须指定要扫描的程序集路径");
                return;
            }
            
            try
            {
                var scanner = serviceProvider.GetRequiredService<IAssemblyScanner>();
                var registry = serviceProvider.GetRequiredService<IServiceRegistry>();
                
                var assemblies = await scanner.LoadAssembliesAsync(assemblyPath, cancellationToken);
                var services = await scanner.ScanAssembliesAsync(assemblies, pattern, lifetime, cancellationToken);
                
                foreach (var service in services)
                {
                    registry.RegisterService(service);
                }
                
                Console.WriteLine($"成功扫描 {assemblies.Length} 个程序集");
                Console.WriteLine($"成功注册 {services.Count} 个服务");
                
                foreach (var service in services)
                {
                    Console.WriteLine($"- {service.ServiceType.FullName} -> {service.ImplementationType?.FullName} ({service.Lifetime})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"扫描程序集时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleDecorateCommand(IServiceProvider serviceProvider, string serviceType, string decoratorType, string lifetime, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(serviceType))
            {
                Console.WriteLine("错误: 必须指定服务类型");
                return;
            }
            
            if (string.IsNullOrEmpty(decoratorType))
            {
                Console.WriteLine("错误: 必须指定装饰器类型");
                return;
            }
            
            try
            {
                var decorator = serviceProvider.GetRequiredService<IServiceDecorator>();
                var registry = serviceProvider.GetRequiredService<IServiceRegistry>();
                
                var service = Type.GetType(serviceType);
                var decorator = Type.GetType(decoratorType);
                
                if (service == null)
                {
                    Console.WriteLine($"错误: 找不到服务类型 {serviceType}");
                    return;
                }
                
                if (decorator == null)
                {
                    Console.WriteLine($"错误: 找不到装饰器类型 {decoratorType}");
                    return;
                }
                
                var decoratedService = await decorator.DecorateServiceAsync(service, decorator, lifetime, cancellationToken);
                registry.RegisterService(decoratedService);
                
                Console.WriteLine($"成功为服务 {serviceType} 添加装饰器 {decoratorType}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加装饰器时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleListCommand(IServiceProvider serviceProvider, string format, CancellationToken cancellationToken)
        {
            try
            {
                var registry = serviceProvider.GetRequiredService<IServiceRegistry>();
                var services = registry.GetRegisteredServices();
                
                if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    var json = JsonSerializer.Serialize(services, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    });
                    Console.WriteLine(json);
                }
                else
                {
                    Console.WriteLine($"已注册的服务 ({services.Count}):");
                    Console.WriteLine("-" + new string('-', 120) + "-");
                    Console.WriteLine($"| {"服务类型",-60} | {"实现类型",-40} | {"生命周期",-15} |");
                    Console.WriteLine("-" + new string('-', 120) + "-");
                    
                    foreach (var service in services)
                    {
                        var serviceType = service.ServiceType.FullName;
                        var implementationType = service.ImplementationType?.FullName ?? "N/A";
                        var lifetime = service.Lifetime.ToString();
                        
                        Console.WriteLine($"| {serviceType,-60} | {implementationType,-40} | {lifetime,-15} |");
                    }
                    
                    Console.WriteLine("-" + new string('-', 120) + "-");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"列出服务时发生错误: {ex.Message}");
            }
        }
    }
    
    // 服务接口
    public interface IAssemblyScanner
    {
        Task<Assembly[]> LoadAssembliesAsync(string path, CancellationToken cancellationToken = default);
        Task<List<ServiceDescriptor>> ScanAssembliesAsync(Assembly[] assemblies, string pattern, string lifetime, CancellationToken cancellationToken = default);
    }
    
    public interface IServiceDecorator
    {
        Task<ServiceDescriptor> DecorateServiceAsync(Type serviceType, Type decoratorType, string lifetime, CancellationToken cancellationToken = default);
    }
    
    public interface IServiceRegistry
    {
        void RegisterService(ServiceDescriptor service);
        List<ServiceDescriptor> GetRegisteredServices();
    }
    
    // 实现类
    public class AssemblyScanner : IAssemblyScanner
    {
        public async Task<Assembly[]> LoadAssembliesAsync(string path, CancellationToken cancellationToken = default)
        {
            var assemblies = new List<Assembly>();
            
            if (Directory.Exists(path))
            {
                // 加载目录中的所有 DLL 文件
                var dllFiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
                foreach (var dllFile in dllFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(dllFile);
                        assemblies.Add(assembly);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"警告: 无法加载程序集 {dllFile}: {ex.Message}");
                    }
                }
            }
            else if (File.Exists(path))
            {
                // 加载单个 DLL 文件
                try
                {
                    var assembly = Assembly.LoadFrom(path);
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"错误: 无法加载程序集 {path}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"错误: 路径不存在: {path}");
            }
            
            return assemblies.ToArray();
        }
        
        public async Task<List<ServiceDescriptor>> ScanAssembliesAsync(Assembly[] assemblies, string pattern, string lifetime, CancellationToken cancellationToken = default)
        {
            var services = new List<ServiceDescriptor>();
            var serviceLifetime = ParseLifetime(lifetime);
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes().Where(t => 
                        t.IsClass && !t.IsAbstract && !t.IsGenericType &&
                        t.Name.Contains(pattern.Replace("*", ""), StringComparison.OrdinalIgnoreCase)
                    );
                    
                    foreach (var type in types)
                    {
                        // 注册为实现的接口
                        var interfaces = type.GetInterfaces().Where(i => !i.IsGenericType);
                        foreach (var @interface in interfaces)
                        {
                            var service = new ServiceDescriptor(@interface, type, serviceLifetime);
                            services.Add(service);
                        }
                        
                        // 注册为自身
                        var selfService = new ServiceDescriptor(type, type, serviceLifetime);
                        services.Add(selfService);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"警告: 扫描程序集 {assembly.FullName} 时发生错误: {ex.Message}");
                }
            }
            
            return services;
        }
        
        private ServiceLifetime ParseLifetime(string lifetime)
        {
            return lifetime.ToLower() switch
            {
                "singleton" => ServiceLifetime.Singleton,
                "transient" => ServiceLifetime.Transient,
                "scoped" => ServiceLifetime.Scoped,
                _ => ServiceLifetime.Scoped
            };
        }
    }
    
    public class ServiceDecorator : IServiceDecorator
    {
        public async Task<ServiceDescriptor> DecorateServiceAsync(Type serviceType, Type decoratorType, string lifetime, CancellationToken cancellationToken = default)
        {
            var serviceLifetime = ParseLifetime(lifetime);
            
            // 检查装饰器是否实现了服务接口
            if (!serviceType.IsAssignableFrom(decoratorType))
            {
                throw new InvalidOperationException($"装饰器类型 {decoratorType.FullName} 必须实现服务类型 {serviceType.FullName}");
            }
            
            // 创建装饰器服务描述符
            var service = new ServiceDescriptor(serviceType, decoratorType, serviceLifetime);
            return service;
        }
        
        private ServiceLifetime ParseLifetime(string lifetime)
        {
            return lifetime.ToLower() switch
            {
                "singleton" => ServiceLifetime.Singleton,
                "transient" => ServiceLifetime.Transient,
                "scoped" => ServiceLifetime.Scoped,
                _ => ServiceLifetime.Scoped
            };
        }
    }
    
    public class ServiceRegistry : IServiceRegistry
    {
        private readonly List<ServiceDescriptor> _services = new List<ServiceDescriptor>();
        
        public void RegisterService(ServiceDescriptor service)
        {
            // 检查是否已存在相同的服务类型
            var existingService = _services.FirstOrDefault(s => s.ServiceType == service.ServiceType);
            if (existingService != null)
            {
                // 替换现有服务
                _services.Remove(existingService);
            }
            
            _services.Add(service);
        }
        
        public List<ServiceDescriptor> GetRegisteredServices()
        {
            return _services;
        }
    }
}
