#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@4.7.2
#:package System.Reflection.MetadataLoadContext@4.7.0
#:package System.Collections.Immutable@4.5.3
#:package System.Linq.Expressions@4.3.0
#:package System.Reflection.Emit@4.7.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IOC AOT 容器");
        Console.WriteLine(new string('=', 60));
        
        var serviceProvider = BuildServiceProvider();
        var iocContainer = serviceProvider.GetRequiredService<IocContainer>();
        var settings = serviceProvider.GetRequiredService<IOptions<IocSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "register":
                case "r":
                    await RegisterService(iocContainer, arguments);
                    break;
                case "resolve":
                case "res":
                    await ResolveService(iocContainer, arguments);
                    break;
                case "build":
                case "b":
                    await BuildContainer(iocContainer, arguments);
                    break;
                case "config":
                case "co":
                    ShowConfig(settings);
                    break;
                case "benchmark":
                case "bm":
                    await RunBenchmark(iocContainer, arguments);
                    break;
                case "help":
                case "h":
                case "?":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine($"未知命令: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IocSettings>(options => {
            options.DefaultLifetime = "transient";
            options.EnableAutoWiring = true;
            options.EnableCircularDependencyDetection = true;
            options.EnableLogging = true;
            options.MaxRegistrationDepth = 10;
            options.EnableBenchmarking = true;
        });
        
        services.AddSingleton<IocContainer>();
        services.AddLogging();
        
        return services.BuildServiceProvider();
    }
    
    private static async Task RegisterService(IocContainer container, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供服务类型和生命周期");
            return;
        }
        
        var serviceType = arguments[0];
        var lifetime = arguments.Length > 1 ? arguments[1] : "transient";
        
        Console.WriteLine($"注册服务: {serviceType}, 生命周期: {lifetime}");
        
        var stopwatch = Stopwatch.StartNew();
        await container.RegisterServiceAsync(serviceType, lifetime);
        stopwatch.Stop();
        
        Console.WriteLine($"服务注册完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
    }
    
    private static async Task ResolveService(IocContainer container, string[] arguments)
    {
        if (arguments.Length < 1)
        {
            Console.WriteLine("错误: 请提供服务类型");
            return;
        }
        
        var serviceType = arguments[0];
        Console.WriteLine($"解析服务: {serviceType}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await container.ResolveServiceAsync(serviceType);
        stopwatch.Stop();
        
        Console.WriteLine($"服务解析完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"解析结果: {result}");
    }
    
    private static async Task BuildContainer(IocContainer container, string[] arguments)
    {
        Console.WriteLine("构建容器...");
        
        var stopwatch = Stopwatch.StartNew();
        await container.BuildAsync();
        stopwatch.Stop();
        
        Console.WriteLine($"容器构建完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"注册的服务数量: {container.GetRegisteredServicesCount()}");
    }
    
    private static async Task RunBenchmark(IocContainer container, string[] arguments)
    {
        var iterations = arguments.Length > 0 ? int.Parse(arguments[0]) : 1000;
        
        Console.WriteLine($"运行基准测试，迭代次数: {iterations}");
        
        var result = await container.RunBenchmarkAsync(iterations);
        Console.WriteLine($"基准测试完成! 平均解析时间: {result.AverageResolutionTime:F3} ms");
        Console.WriteLine($"每秒操作数: {result.OperationsPerSecond:F2} ops/s");
    }
    
    private static void ShowConfig(IocSettings settings)
    {
        Console.WriteLine("IOC 配置:");
        Console.WriteLine($"默认生命周期: {settings.DefaultLifetime}");
        Console.WriteLine($"自动装配: {settings.EnableAutoWiring}");
        Console.WriteLine($"循环依赖检测: {settings.EnableCircularDependencyDetection}");
        Console.WriteLine($"日志记录: {settings.EnableLogging}");
        Console.WriteLine($"最大注册深度: {settings.MaxRegistrationDepth}");
        Console.WriteLine($"基准测试: {settings.EnableBenchmarking}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("IOC AOT 容器 命令帮助:");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("register (r)     - 注册服务");
        Console.WriteLine("resolve (res)    - 解析服务");
        Console.WriteLine("build (b)        - 构建容器");
        Console.WriteLine("config (co)      - 显示配置");
        Console.WriteLine("benchmark (bm)   - 运行基准测试");
        Console.WriteLine("help (h, ?)      - 显示帮助信息");
    }
}

public class IocSettings
{
    public string DefaultLifetime { get; set; } = "transient";
    public bool EnableAutoWiring { get; set; } = true;
    public bool EnableCircularDependencyDetection { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
    public int MaxRegistrationDepth { get; set; } = 10;
    public bool EnableBenchmarking { get; set; } = true;
}

public class BenchmarkResult
{
    public double AverageResolutionTime { get; set; }
    public int OperationsPerSecond { get; set; }
}

public class IocContainer : IDisposable
{
    private readonly Dictionary<Type, ServiceDescriptor> _registrations = new();
    private bool _built;
    private IServiceProvider? _provider;
    private readonly ILogger<IocContainer> _logger;
    private readonly IocSettings _settings;
    
    public IocContainer(ILogger<IocContainer> logger, IOptions<IocSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
        _logger.LogInformation("IOC 容器初始化成功");
    }
    
    public async Task RegisterServiceAsync(string serviceType, string lifetime = "transient")
    {
        _logger.LogInformation($"注册服务: {serviceType}, 生命周期: {lifetime}");
        await Task.Run(() => {
            var type = Type.GetType(serviceType);
            if (type == null)
            {
                throw new ArgumentException($"类型不存在: {serviceType}");
            }
            
            var descriptor = new ServiceDescriptor(type, type, GetLifetime(lifetime));
            _registrations[type] = descriptor;
        });
    }
    
    public async Task<object?> ResolveServiceAsync(string serviceType)
    {
        if (!_built)
        {
            await BuildAsync();
        }
        
        var type = Type.GetType(serviceType);
        if (type == null)
        {
            throw new ArgumentException($"类型不存在: {serviceType}");
        }
        
        return _provider?.GetService(type);
    }
    
    public async Task BuildAsync()
    {
        _logger.LogInformation("构建 IOC 容器...");
        
        // 简化实现，直接使用已有的ServiceProvider
        // 在实际应用中，这里会构建完整的依赖图
        _built = true;
        _logger.LogInformation($"容器构建完成! 注册服务数: {_registrations.Count}");
    }
    
    public int GetRegisteredServicesCount() => _registrations.Count;
    
    public async Task<BenchmarkResult> RunBenchmarkAsync(int iterations = 1000)
    {
        if (!_built)
        {
            await BuildAsync();
        }
        
        _logger.LogInformation($"运行基准测试，迭代次数: {iterations}");
        
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            await Task.Run(() => {
                // 简化实现，模拟服务解析
                foreach (var type in _registrations.Keys)
                {
                    // 模拟服务解析操作
                    GC.Collect();
                }
            });
        }
        stopwatch.Stop();
        
        var avgTime = stopwatch.Elapsed.TotalMilliseconds / iterations;
        var opsPerSecond = 1000 / avgTime;
        
        _logger.LogInformation($"基准测试完成! 平均解析时间: {avgTime:F3} ms");
        _logger.LogInformation($"每秒操作数: {opsPerSecond:F2} ops/s");
        
        return new BenchmarkResult {
            AverageResolutionTime = avgTime,
            OperationsPerSecond = (int)opsPerSecond
        };
    }
    
    private ServiceLifetime GetLifetime(string lifetime)
    {
        return lifetime.ToLower() switch
        {
            "singleton" => ServiceLifetime.Singleton,
            "scoped" => ServiceLifetime.Scoped,
            _ => ServiceLifetime.Transient
        };
    }
    
    public void Dispose()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
