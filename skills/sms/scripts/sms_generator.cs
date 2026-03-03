#:sdk Microsoft.NET.Sdk
#:package Scrutor@4.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Logging.Console@10.0.0
#:package System.CommandLine@2.0.0
#:property LangVersion=latest
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;

namespace SmsSkill.Generator
{
    // 代码生成接口
    public interface ICodeGeneratorService
    {
        Task<string> GenerateServiceInterfaceAsync(string serviceName, IEnumerable<string> methods, CancellationToken cancellationToken = default);
        Task<string> GenerateServiceImplementationAsync(string serviceName, string interfaceName, IEnumerable<string> methods, CancellationToken cancellationToken = default);
        Task<string> GenerateDecoratorAsync(string serviceName, string interfaceName, CancellationToken cancellationToken = default);
        Task<string> GenerateScrutorRegistrationAsync(string assemblyName, CancellationToken cancellationToken = default);
    }

    // 代码生成实现
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ILogger<CodeGeneratorService> _logger;

        public CodeGeneratorService(ILogger<CodeGeneratorService> logger)
        {
            _logger = logger;
        }

        public Task<string> GenerateServiceInterfaceAsync(string serviceName, IEnumerable<string> methods, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"using System.Threading;");
            sb.AppendLine($"using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace SmsSkill");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public interface I{serviceName}");
            sb.AppendLine($"    {{");
            
            foreach (var method in methods)
            {
                sb.AppendLine($"        Task {method};");
            }
            
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> GenerateServiceImplementationAsync(string serviceName, string interfaceName, IEnumerable<string> methods, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"using System.Threading;");
            sb.AppendLine($"using System.Threading.Tasks;");
            sb.AppendLine($"using Microsoft.Extensions.Logging;");
            sb.AppendLine();
            sb.AppendLine($"namespace SmsSkill");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {serviceName} : {interfaceName}");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        private readonly ILogger<{serviceName}> _logger;");
            sb.AppendLine();
            sb.AppendLine($"        public {serviceName}(ILogger<{serviceName}> logger)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger = logger;");
            sb.AppendLine($"        }}");
            sb.AppendLine();
            
            foreach (var method in methods)
            {
                var methodName = method.Split('(')[0];
                sb.AppendLine($"        public async Task {method}");
                sb.AppendLine($"        {{");
                sb.AppendLine($"            _logger.LogInformation(\"Executing {methodName}\");");
                sb.AppendLine($"            // 实现逻辑");
                sb.AppendLine($"            await Task.CompletedTask;");
                sb.AppendLine($"        }}");
                sb.AppendLine();
            }
            
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> GenerateDecoratorAsync(string serviceName, string interfaceName, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"using System.Threading;");
            sb.AppendLine($"using System.Threading.Tasks;");
            sb.AppendLine($"using Microsoft.Extensions.Logging;");
            sb.AppendLine();
            sb.AppendLine($"namespace SmsSkill");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public class {serviceName}Decorator : {interfaceName}");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        private readonly {interfaceName} _decorated;");
            sb.AppendLine($"        private readonly ILogger<{serviceName}Decorator> _logger;");
            sb.AppendLine();
            sb.AppendLine($"        public {serviceName}Decorator({interfaceName} decorated, ILogger<{serviceName}Decorator> logger)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _decorated = decorated;");
            sb.AppendLine($"            _logger = logger;");
            sb.AppendLine($"        }}");
            sb.AppendLine();
            sb.AppendLine($"        // 实现接口方法，添加装饰逻辑");
            sb.AppendLine($"        public async Task ExampleMethodAsync(CancellationToken cancellationToken = default)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            _logger.LogInformation(\"Before executing ExampleMethod\");");
            sb.AppendLine($"            await _decorated.ExampleMethodAsync(cancellationToken);");
            sb.AppendLine($"            _logger.LogInformation(\"After executing ExampleMethod\");");
            sb.AppendLine($"        }}");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            
            return Task.FromResult(sb.ToString());
        }

        public Task<string> GenerateScrutorRegistrationAsync(string assemblyName, CancellationToken cancellationToken = default)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine($"using Scrutor;");
            sb.AppendLine();
            sb.AppendLine($"namespace SmsSkill");
            sb.AppendLine($"{{");
            sb.AppendLine($"    public static class ServiceCollectionExtensions");
            sb.AppendLine($"    {{");
            sb.AppendLine($"        public static IServiceCollection AddSmsServices(this IServiceCollection services)");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            // 1. 基本注册");
            sb.AppendLine($"            services.AddSingleton<ISmsProvider, TwilioSmsProvider>();");
            sb.AppendLine($"            services.AddSingleton<ITemplateService, TemplateService>();");
            sb.AppendLine($"            services.AddSingleton<IAnalyticsService, AnalyticsService>();");
            sb.AppendLine($"            services.AddSingleton<ISmsService, SmsService>();");
            sb.AppendLine();
            sb.AppendLine($"            // 2. 使用 Scrutor 装饰器模式");
            sb.AppendLine($"            services.Decorate<ISmsService, SmsServiceLoggingDecorator>();");
            sb.AppendLine($"            services.Decorate<ISmsService, SmsServiceRetryDecorator>();");
            sb.AppendLine();
            sb.AppendLine($"            // 3. 使用 Scrutor 程序集扫描");
            sb.AppendLine($"            services.Scan(scan => scan");
            sb.AppendLine($"                .FromAssemblyOf<SmsService>());");
            sb.AppendLine();
            sb.AppendLine($"            // 4. 高级程序集扫描配置");
            sb.AppendLine($"            services.Scan(scan => scan");
            sb.AppendLine($"                .FromAssemblyOf<SmsService>());");
            sb.AppendLine($"            ");
            sb.AppendLine($"            return services;");
            sb.AppendLine($"        }}");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            
            return Task.FromResult(sb.ToString());
        }
    }

    // Scrutor 用法示例
    public class ScrutorUsageExamples
    {
        public static void BasicRegistration(IServiceCollection services)
        {
            // 基本注册方式
            services.AddSingleton<ISmsProvider, TwilioSmsProvider>();
            services.AddSingleton<ITemplateService, TemplateService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<ISmsService, SmsService>();
        }

        public static void DecoratorPattern(IServiceCollection services)
        {
            // 使用 Scrutor 装饰器模式
            services.AddSingleton<ISmsService, SmsService>();
            services.Decorate<ISmsService, SmsServiceLoggingDecorator>();
            services.Decorate<ISmsService, SmsServiceRetryDecorator>();
        }

        public static void AssemblyScanning(IServiceCollection services)
        {
            // 使用 Scrutor 程序集扫描
            services.Scan(scan => scan
                .FromAssemblyOf<SmsService>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        }

        public static void AdvancedAssemblyScanning(IServiceCollection services)
        {
            // 高级程序集扫描配置
            services.Scan(scan => scan
                .FromAssemblyOf<SmsService>()
                // 注册所有实现了 ISmsProvider 的类
                .AddClasses(classes => classes.AssignableTo<ISmsProvider>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                // 注册所有实现了 ITemplateService 的类
                .AddClasses(classes => classes.AssignableTo<ITemplateService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                // 注册所有实现了 IAnalyticsService 的类
                .AddClasses(classes => classes.AssignableTo<IAnalyticsService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
                // 注册所有实现了 ISmsService 的类
                .AddClasses(classes => classes.AssignableTo<ISmsService>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        }

        public static void DecoratorWithCondition(IServiceCollection services)
        {
            // 条件装饰器
            services.AddSingleton<ISmsService, SmsService>();
            services.Decorate<ISmsService>((provider, decorated) =>
            {
                var logger = provider.GetRequiredService<ILogger<SmsServiceLoggingDecorator>>();
                return new SmsServiceLoggingDecorator(decorated, logger);
            });
        }

        public static void MultipleDecorators(IServiceCollection services)
        {
            // 多个装饰器
            services.AddSingleton<ISmsService, SmsService>();
            services.Decorate<ISmsService, SmsServiceLoggingDecorator>();
            services.Decorate<ISmsService, SmsServiceRetryDecorator>();
            services.Decorate<ISmsService, SmsServiceValidationDecorator>();
        }
    }

    // 装饰器示例类
    public class SmsServiceLoggingDecorator : ISmsService
    {
        private readonly ISmsService _decorated;
        private readonly ILogger<SmsServiceLoggingDecorator> _logger;

        public SmsServiceLoggingDecorator(ISmsService decorated, ILogger<SmsServiceLoggingDecorator> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }

        public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending SMS to {To}", to);
            var result = await _decorated.SendAsync(to, message, cancellationToken);
            _logger.LogInformation("SMS sent to {To}, Success: {Success}", to, result.Success);
            return result;
        }

        public async Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending SMS template to {To}, TemplateId: {TemplateId}", to, templateId);
            var result = await _decorated.SendTemplateAsync(to, templateId, parameters, cancellationToken);
            _logger.LogInformation("SMS template sent to {To}, Success: {Success}", to, result.Success);
            return result;
        }

        public async Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting batch SMS send to {Count} recipients", recipients.Count());
            var results = await _decorated.BatchSendAsync(recipients, message, cancellationToken);
            var successCount = results.Count(r => r.Success);
            _logger.LogInformation("Batch SMS send completed, Success: {SuccessCount}/{TotalCount}", successCount, results.Count());
            return results;
        }

        public async Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Scheduling SMS to {To} at {SendTime}", to, sendTime);
            var result = await _decorated.ScheduleSendAsync(to, message, sendTime, cancellationToken);
            _logger.LogInformation("SMS scheduled to {To}, Success: {Success}", to, result.Success);
            return result;
        }

        public async Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting SMS analytics from {StartDate} to {EndDate}", startDate, endDate);
            var result = await _decorated.GetAnalyticsAsync(startDate, endDate, cancellationToken);
            _logger.LogInformation("SMS analytics retrieved, Total SMS: {TotalSms}", result.TotalSms);
            return result;
        }

        public async Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Receiving SMS messages");
            var result = await _decorated.ReceiveAsync(cancellationToken);
            _logger.LogInformation("Received {Count} SMS messages", result.Count());
            return result;
        }
    }

    public class SmsServiceRetryDecorator : ISmsService
    {
        private readonly ISmsService _decorated;
        private readonly ILogger<SmsServiceRetryDecorator> _logger;

        public SmsServiceRetryDecorator(ISmsService decorated, ILogger<SmsServiceRetryDecorator> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }

        public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
        {
            int retryCount = 3;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    return await _decorated.SendAsync(to, message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "SMS send failed, retry {RetryCount}/{MaxRetries}", i + 1, retryCount);
                    if (i == retryCount - 1)
                    {
                        throw;
                    }
                    await Task.Delay(1000 * (i + 1), cancellationToken);
                }
            }
            throw new Exception("All retries failed");
        }

        public Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            return _decorated.SendTemplateAsync(to, templateId, parameters, cancellationToken);
        }

        public Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default)
        {
            return _decorated.BatchSendAsync(recipients, message, cancellationToken);
        }

        public Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default)
        {
            return _decorated.ScheduleSendAsync(to, message, sendTime, cancellationToken);
        }

        public Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return _decorated.GetAnalyticsAsync(startDate, endDate, cancellationToken);
        }

        public Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            return _decorated.ReceiveAsync(cancellationToken);
        }
    }

    public class SmsServiceValidationDecorator : ISmsService
    {
        private readonly ISmsService _decorated;
        private readonly ILogger<SmsServiceValidationDecorator> _logger;

        public SmsServiceValidationDecorator(ISmsService decorated, ILogger<SmsServiceValidationDecorator> logger)
        {
            _decorated = decorated;
            _logger = logger;
        }

        public async Task<SmsResult> SendAsync(string to, string message, CancellationToken cancellationToken = default)
        {
            // 验证电话号码
            if (string.IsNullOrWhiteSpace(to) || !to.StartsWith("+"))
            {
                throw new ArgumentException("Invalid phone number format, must start with '+'");
            }

            // 验证消息长度
            if (string.IsNullOrWhiteSpace(message) || message.Length > 1600)
            {
                throw new ArgumentException("Message cannot be empty or exceed 1600 characters");
            }

            return await _decorated.SendAsync(to, message, cancellationToken);
        }

        public Task<SmsResult> SendTemplateAsync(string to, string templateId, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            return _decorated.SendTemplateAsync(to, templateId, parameters, cancellationToken);
        }

        public Task<IEnumerable<SmsResult>> BatchSendAsync(IEnumerable<string> recipients, string message, CancellationToken cancellationToken = default)
        {
            return _decorated.BatchSendAsync(recipients, message, cancellationToken);
        }

        public Task<SmsScheduleResult> ScheduleSendAsync(string to, string message, DateTime sendTime, CancellationToken cancellationToken = default)
        {
            return _decorated.ScheduleSendAsync(to, message, sendTime, cancellationToken);
        }

        public Task<SmsAnalytics> GetAnalyticsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return _decorated.GetAnalyticsAsync(startDate, endDate, cancellationToken);
        }

        public Task<IEnumerable<SmsMessage>> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            return _decorated.ReceiveAsync(cancellationToken);
        }
    }

    // 命令行接口
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // 创建根命令
            var rootCommand = new RootCommand("SMS 技能代码生成工具");
            
            // 生成接口命令
            var generateInterfaceCommand = new Command("generate-interface", "生成服务接口");
            var interfaceNameOption = new Option<string>("--name", "接口名称") { IsRequired = true };
            var methodsOption = new Option<string[]>("--methods", "方法列表，用逗号分隔") { IsRequired = true };
            generateInterfaceCommand.AddOption(interfaceNameOption);
            generateInterfaceCommand.AddOption(methodsOption);
            generateInterfaceCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(interfaceNameOption);
                var methods = context.ParseResult.GetValueForOption(methodsOption);
                await HandleGenerateInterfaceCommand(name, methods);
            });
            
            // 生成实现命令
            var generateImplementationCommand = new Command("generate-implementation", "生成服务实现");
            var implementationNameOption = new Option<string>("--name", "实现类名称") { IsRequired = true };
            var interfaceOption = new Option<string>("--interface", "接口名称") { IsRequired = true };
            var implementationMethodsOption = new Option<string[]>("--methods", "方法列表，用逗号分隔") { IsRequired = true };
            generateImplementationCommand.AddOption(implementationNameOption);
            generateImplementationCommand.AddOption(interfaceOption);
            generateImplementationCommand.AddOption(implementationMethodsOption);
            generateImplementationCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(implementationNameOption);
                var interfaceName = context.ParseResult.GetValueForOption(interfaceOption);
                var methods = context.ParseResult.GetValueForOption(implementationMethodsOption);
                await HandleGenerateImplementationCommand(name, interfaceName, methods);
            });
            
            // 生成装饰器命令
            var generateDecoratorCommand = new Command("generate-decorator", "生成装饰器");
            var decoratorNameOption = new Option<string>("--name", "装饰器名称") { IsRequired = true };
            var decoratorInterfaceOption = new Option<string>("--interface", "接口名称") { IsRequired = true };
            generateDecoratorCommand.AddOption(decoratorNameOption);
            generateDecoratorCommand.AddOption(decoratorInterfaceOption);
            generateDecoratorCommand.SetHandler(async (context) =>
            {
                var name = context.ParseResult.GetValueForOption(decoratorNameOption);
                var interfaceName = context.ParseResult.GetValueForOption(decoratorInterfaceOption);
                await HandleGenerateDecoratorCommand(name, interfaceName);
            });
            
            // 生成 Scrutor 注册命令
            var generateScrutorCommand = new Command("generate-scrutor", "生成 Scrutor 注册代码");
            var assemblyNameOption = new Option<string>("--assembly", "程序集名称") { IsRequired = true };
            generateScrutorCommand.AddOption(assemblyNameOption);
            generateScrutorCommand.SetHandler(async (context) =>
            {
                var assemblyName = context.ParseResult.GetValueForOption(assemblyNameOption);
                await HandleGenerateScrutorCommand(assemblyName);
            });
            
            // 添加命令到根命令
            rootCommand.AddCommand(generateInterfaceCommand);
            rootCommand.AddCommand(generateImplementationCommand);
            rootCommand.AddCommand(generateDecoratorCommand);
            rootCommand.AddCommand(generateScrutorCommand);
            
            // 执行命令
            return await rootCommand.InvokeAsync(args);
        }
        
        private static async Task HandleGenerateInterfaceCommand(string name, string[] methods)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>();
            
            using var serviceProvider = services.BuildServiceProvider();
            var generator = serviceProvider.GetRequiredService<ICodeGeneratorService>();
            
            var code = await generator.GenerateServiceInterfaceAsync(name, methods);
            Console.WriteLine("生成的接口代码:");
            Console.WriteLine(code);
            
            // 保存到文件
            var filePath = $"I{name}.cs";
            File.WriteAllText(filePath, code);
            Console.WriteLine($"代码已保存到: {filePath}");
        }
        
        private static async Task HandleGenerateImplementationCommand(string name, string interfaceName, string[] methods)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>();
            
            using var serviceProvider = services.BuildServiceProvider();
            var generator = serviceProvider.GetRequiredService<ICodeGeneratorService>();
            
            var code = await generator.GenerateServiceImplementationAsync(name, interfaceName, methods);
            Console.WriteLine("生成的实现代码:");
            Console.WriteLine(code);
            
            // 保存到文件
            var filePath = $"{name}.cs";
            File.WriteAllText(filePath, code);
            Console.WriteLine($"代码已保存到: {filePath}");
        }
        
        private static async Task HandleGenerateDecoratorCommand(string name, string interfaceName)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>();
            
            using var serviceProvider = services.BuildServiceProvider();
            var generator = serviceProvider.GetRequiredService<ICodeGeneratorService>();
            
            var code = await generator.GenerateDecoratorAsync(name, interfaceName);
            Console.WriteLine("生成的装饰器代码:");
            Console.WriteLine(code);
            
            // 保存到文件
            var filePath = $"{name}Decorator.cs";
            File.WriteAllText(filePath, code);
            Console.WriteLine($"代码已保存到: {filePath}");
        }
        
        private static async Task HandleGenerateScrutorCommand(string assemblyName)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>();
            
            using var serviceProvider = services.BuildServiceProvider();
            var generator = serviceProvider.GetRequiredService<ICodeGeneratorService>();
            
            var code = await generator.GenerateScrutorRegistrationAsync(assemblyName);
            Console.WriteLine("生成的 Scrutor 注册代码:");
            Console.WriteLine(code);
            
            // 保存到文件
            var filePath = "ServiceCollectionExtensions.cs";
            File.WriteAllText(filePath, code);
            Console.WriteLine($"代码已保存到: {filePath}");
        }
    }
}
