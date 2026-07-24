#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
#:package Microsoft.Extensions.Hosting@8.0.0
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
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scheduler.Generator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Scheduler 代码生成工具");
            
            // 创建生成命令
            var generateCommand = new Command("generate", "生成任务调度代码");
            var typeOption = new Option<string>("--type", "生成类型 (host/worker/service)");
            var outputOption = new Option<string>("--output", "输出文件路径");
            var formatOption = new Option<string>("--format", () => "csharp", "输出格式 (csharp)");
            var namespaceOption = new Option<string>("--namespace", () => "Scheduler.Generated", "命名空间");
            var classNameOption = new Option<string>("--class", "类名");
            generateCommand.AddOption(typeOption);
            generateCommand.AddOption(outputOption);
            generateCommand.AddOption(formatOption);
            generateCommand.AddOption(namespaceOption);
            generateCommand.AddOption(classNameOption);
            generateCommand.SetHandler(async (context) =>
            {
                var type = context.ParseResult.GetValueForOption(typeOption);
                var output = context.ParseResult.GetValueForOption(outputOption);
                var format = context.ParseResult.GetValueForOption(formatOption);
                var @namespace = context.ParseResult.GetValueForOption(namespaceOption);
                var className = context.ParseResult.GetValueForOption(classNameOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleGenerateCommand(serviceProvider, type, output, format, @namespace, className, cancellationToken);
            });
            
            // 创建模板命令
            var templateCommand = new Command("template", "管理代码模板");
            var templateSubCommand = new Command("list", "列出可用模板");
            templateCommand.AddCommand(templateSubCommand);
            templateSubCommand.SetHandler(async (context) =>
            {
                await HandleListTemplatesCommand(serviceProvider, context.GetCancellationToken());
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(generateCommand);
            rootCommand.AddCommand(templateCommand);
            
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
            services.AddSingleton<ICodeGenerator, CodeGenerator>();
            services.AddSingleton<ITemplateManager, TemplateManager>();
            
            // 注册代码生成器
            services.AddSingleton<ICodeGeneratorStrategy, HostCodeGenerator>();
            services.AddSingleton<ICodeGeneratorStrategy, WorkerCodeGenerator>();
            services.AddSingleton<ICodeGeneratorStrategy, ServiceCodeGenerator>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleGenerateCommand(IServiceProvider serviceProvider, string type, string output, string format, string @namespace, string className, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(type))
            {
                Console.WriteLine("错误: 必须指定生成类型");
                return;
            }
            
            if (string.IsNullOrEmpty(output))
            {
                Console.WriteLine("错误: 必须指定输出文件路径");
                return;
            }
            
            try
            {
                var codeGenerator = serviceProvider.GetRequiredService<ICodeGenerator>();
                var code = await codeGenerator.GenerateAsync(type, output, format, @namespace, className, cancellationToken);
                
                Console.WriteLine($"成功生成代码到: {output}");
                Console.WriteLine($"生成的代码行数: {code.Split('\n').Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成代码时发生错误: {ex.Message}");
            }
        }
        
        private static async Task HandleListTemplatesCommand(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            try
            {
                var templateManager = serviceProvider.GetRequiredService<ITemplateManager>();
                var templates = await templateManager.GetTemplatesAsync(cancellationToken);
                
                Console.WriteLine("可用的代码模板:");
                Console.WriteLine("-" + new string('-', 80) + "-");
                
                foreach (var template in templates)
                {
                    Console.WriteLine($"模板: {template.Name}");
                    Console.WriteLine($"描述: {template.Description}");
                    Console.WriteLine($"类型: {template.Type}");
                    Console.WriteLine("-" + new string('-', 80) + "-");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"列出模板时发生错误: {ex.Message}");
            }
        }
    }
    
    // 服务接口
    public interface ICodeGenerator
    {
        Task<string> GenerateAsync(string type, string outputPath, string format, string @namespace, string className, CancellationToken cancellationToken = default);
    }
    
    public interface ITemplateManager
    {
        Task<IEnumerable<TemplateInfo>> GetTemplatesAsync(CancellationToken cancellationToken = default);
        Task<string> GetTemplateAsync(string name, CancellationToken cancellationToken = default);
    }
    
    public interface ICodeGeneratorStrategy
    {
        string Type { get; }
        string Generate(string @namespace, string className);
    }
    
    // 数据结构
    public class TemplateInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
    
    // 实现类
    public class CodeGenerator : ICodeGenerator
    {
        private readonly IEnumerable<ICodeGeneratorStrategy> _strategies;
        private readonly ITemplateManager _templateManager;
        
        public CodeGenerator(IEnumerable<ICodeGeneratorStrategy> strategies, ITemplateManager templateManager)
        {
            _strategies = strategies;
            _templateManager = templateManager;
        }
        
        public async Task<string> GenerateAsync(string type, string outputPath, string format, string @namespace, string className, CancellationToken cancellationToken = default)
        {
            // 选择代码生成策略
            var strategy = _strategies.FirstOrDefault(s => s.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            if (strategy == null)
            {
                throw new InvalidOperationException($"不支持的生成类型: {type}");
            }
            
            // 生成代码
            var code = strategy.Generate(@namespace, className ?? GetDefaultClassName(type));
            
            // 写入输出文件
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            await File.WriteAllTextAsync(outputPath, code, cancellationToken);
            
            return code;
        }
        
        private string GetDefaultClassName(string type)
        {
            switch (type.ToLower())
            {
                case "host":
                    return "SchedulerHost";
                case "worker":
                    return "SchedulerWorker";
                case "service":
                    return "SchedulerService";
                default:
                    return "SchedulerGenerated";
            }
        }
    }
    
    public class TemplateManager : ITemplateManager
    {
        public Task<IEnumerable<TemplateInfo>> GetTemplatesAsync(CancellationToken cancellationToken = default)
        {
            var templates = new List<TemplateInfo>
            {
                new TemplateInfo
                {
                    Name = "host",
                    Description = "主机应用模板",
                    Type = "host"
                },
                new TemplateInfo
                {
                    Name = "worker",
                    Description = "工作器服务模板",
                    Type = "worker"
                },
                new TemplateInfo
                {
                    Name = "service",
                    Description = "Windows 服务模板",
                    Type = "service"
                }
            };
            
            return Task.FromResult<IEnumerable<TemplateInfo>>(templates);
        }
        
        public Task<string> GetTemplateAsync(string name, CancellationToken cancellationToken = default)
        {
            // 根据模板名称返回对应的模板内容
            // 实际项目中应该从文件或数据库加载模板
            var templateContent = "";
            
            switch (name)
            {
                case "host":
                    templateContent = GetHostTemplate();
                    break;
                case "worker":
                    templateContent = GetWorkerTemplate();
                    break;
                case "service":
                    templateContent = GetServiceTemplate();
                    break;
            }
            
            return Task.FromResult(templateContent);
        }
        
        private string GetHostTemplate()
        {
            return @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加托管服务
                    services.AddHostedService<SchedulerHostedService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }
}";
        }
        
        private string GetWorkerTemplate()
        {
            return @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加工作器服务
                    services.AddHostedService<SchedulerWorker>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }

    public class SchedulerWorker : BackgroundService
    {
        private readonly ISchedulerService _schedulerService;
        private readonly ILogger<SchedulerWorker> _logger;

        public SchedulerWorker(ISchedulerService schedulerService, ILogger<SchedulerWorker> logger)
        {
            _schedulerService = schedulerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("启动调度工作器");
            
            // 启动调度器
            await _schedulerService.StartAsync(stoppingToken);
            
            // 等待停止信号
            await stoppingToken.WhenCanceled();
            
            _logger.LogInformation("停止调度工作器");
            await _schedulerService.StopAsync(stoppingToken);
        }
    }
}";
        }
        
        private string GetServiceTemplate()
        {
            return @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加托管服务
                    services.AddHostedService<SchedulerHostedService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseWindowsService();
    }
}";
        }
    }
    
    public class HostCodeGenerator : ICodeGeneratorStrategy
    {
        public string Type => "host";
        
        public string Generate(string @namespace, string className)
        {
            var template = @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加托管服务
                    services.AddHostedService<SchedulerHostedService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }
}";
            
            return template
                .Replace("{{Namespace}}", @namespace)
                .Replace("{{ClassName}}", className);
        }
    }
    
    public class WorkerCodeGenerator : ICodeGeneratorStrategy
    {
        public string Type => "worker";
        
        public string Generate(string @namespace, string className)
        {
            var template = @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加工作器服务
                    services.AddHostedService<SchedulerWorker>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
    }

    public class SchedulerWorker : BackgroundService
    {
        private readonly ISchedulerService _schedulerService;
        private readonly ILogger<SchedulerWorker> _logger;

        public SchedulerWorker(ISchedulerService schedulerService, ILogger<SchedulerWorker> logger)
        {
            _schedulerService = schedulerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("启动调度工作器");
            
            // 启动调度器
            await _schedulerService.StartAsync(stoppingToken);
            
            // 等待停止信号
            await stoppingToken.WhenCanceled();
            
            _logger.LogInformation("停止调度工作器");
            await _schedulerService.StopAsync(stoppingToken);
        }
    }
}";
            
            return template
                .Replace("{{Namespace}}", @namespace)
                .Replace("{{ClassName}}", className);
        }
    }
    
    public class ServiceCodeGenerator : ICodeGeneratorStrategy
    {
        public string Type => "service";
        
        public string Generate(string @namespace, string className)
        {
            var template = @"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Core;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // 配置服务
                    services.AddSingleton<ISchedulerService, SchedulerService>();
                    services.AddSingleton<ITaskExecutor, TaskExecutor>();
                    services.AddSingleton<IJobStore, FileJobStore>();
                    services.AddSingleton<ICronParser, CronParser>();
                    services.AddSingleton<IDelayScheduler, DelayScheduler>();
                    services.AddSingleton<IIntervalScheduler, IntervalScheduler>();
                    
                    // 添加托管服务
                    services.AddHostedService<SchedulerHostedService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseWindowsService();
    }
}";
            
            return template
                .Replace("{{Namespace}}", @namespace)
                .Replace("{{ClassName}}", className);
        }
    }
    
    // 扩展方法
    public static class CancellationTokenExtensions
    {
        public static Task WhenCanceled(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}
