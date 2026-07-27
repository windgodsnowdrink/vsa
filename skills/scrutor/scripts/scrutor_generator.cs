#:sdk Microsoft.NET.Sdk
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
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

namespace Scrutor.Generator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // 配置依赖注入
            var serviceProvider = ConfigureServices();
            
            // 创建命令行根命令
            var rootCommand = new RootCommand("Scrutor 代码生成工具");
            
            // 创建生成命令
            var generateCommand = new Command("generate", "生成依赖注入配置代码");
            var outputOption = new Option<string>("--output", "输出文件路径");
            var namespaceOption = new Option<string>("--namespace", () => "Scrutor.Generated", "命名空间");
            var classOption = new Option<string>("--class", () => "DependencyInjection", "类名");
            generateCommand.AddOption(outputOption);
            generateCommand.AddOption(namespaceOption);
            generateCommand.AddOption(classOption);
            generateCommand.SetHandler(async (context) =>
            {
                var output = context.ParseResult.GetValueForOption(outputOption);
                var @namespace = context.ParseResult.GetValueForOption(namespaceOption);
                var className = context.ParseResult.GetValueForOption(classOption);
                var cancellationToken = context.GetCancellationToken();
                
                await HandleGenerateCommand(serviceProvider, output, @namespace, className, cancellationToken);
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
            services.AddSingleton<ICodeGeneratorStrategy, DependencyInjectionCodeGenerator>();
            services.AddSingleton<ICodeGeneratorStrategy, ServiceCollectionCodeGenerator>();
            services.AddSingleton<ICodeGeneratorStrategy, WebApplicationCodeGenerator>();
            
            return services.BuildServiceProvider();
        }
        
        private static async Task HandleGenerateCommand(IServiceProvider serviceProvider, string outputPath, string @namespace, string className, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                Console.WriteLine("错误: 必须指定输出文件路径");
                return;
            }
            
            try
            {
                var codeGenerator = serviceProvider.GetRequiredService<ICodeGenerator>();
                var code = await codeGenerator.GenerateAsync(outputPath, @namespace, className, cancellationToken);
                
                Console.WriteLine($"成功生成代码到: {outputPath}");
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
        Task<string> GenerateAsync(string outputPath, string @namespace, string className, CancellationToken cancellationToken = default);
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
        
        public async Task<string> GenerateAsync(string outputPath, string @namespace, string className, CancellationToken cancellationToken = default)
        {
            // 选择默认的代码生成策略
            var strategy = _strategies.FirstOrDefault(s => s.Type == "dependency-injection") ?? 
                          _strategies.FirstOrDefault();
            
            if (strategy == null)
            {
                throw new InvalidOperationException("找不到可用的代码生成策略");
            }
            
            // 生成代码
            var code = strategy.Generate(@namespace, className);
            
            // 写入输出文件
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            await File.WriteAllTextAsync(outputPath, code, cancellationToken);
            
            return code;
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
                    Name = "dependency-injection",
                    Description = "依赖注入配置代码模板",
                    Type = "dependency-injection"
                },
                new TemplateInfo
                {
                    Name = "service-collection",
                    Description = "服务集合配置代码模板",
                    Type = "service-collection"
                },
                new TemplateInfo
                {
                    Name = "web-application",
                    Description = "Web 应用配置代码模板",
                    Type = "web-application"
                }
            };
            
            return Task.FromResult<IEnumerable<TemplateInfo>>(templates);
        }
        
        public Task<string> GetTemplateAsync(string name, CancellationToken cancellationToken = default)
        {
            // 根据模板名称返回对应的模板内容
            var templateContent = "";
            
            switch (name)
            {
                case "dependency-injection":
                    templateContent = GetDependencyInjectionTemplate();
                    break;
                case "service-collection":
                    templateContent = GetServiceCollectionTemplate();
                    break;
                case "web-application":
                    templateContent = GetWebApplicationTemplate();
                    break;
            }
            
            return Task.FromResult(templateContent);
        }
        
        private string GetDependencyInjectionTemplate()
        {
            return @"using Microsoft.Extensions.DependencyInjection;

namespace {{Namespace}}
{
    public static class {{ClassName}}
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            // 注册服务
            // 示例：services.AddScoped<IService, Service>();
            
            return services;
        }
    }
}";
        }
        
        private string GetServiceCollectionTemplate()
        {
            return @"using Microsoft.Extensions.DependencyInjection;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 注册服务
            // 示例：services.AddScoped<IService, Service>();
            
            return services;
        }
    }
}";
        }
        
        private string GetWebApplicationTemplate()
        {
            return @"using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace {{Namespace}}
{
    public class {{ClassName}}
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // 注册服务
            // 示例：builder.Services.AddScoped<IService, Service>();
            
            // 配置其他服务
            // builder.Services.AddControllers();
            // builder.Services.AddSwaggerGen();
        }
        
        public static void ConfigurePipeline(WebApplication app)
        {
            // 配置中间件
            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }
            
            // app.UseHttpsRedirection();
            // app.UseAuthorization();
            // app.MapControllers();
        }
    }
}";
        }
    }
    
    public class DependencyInjectionCodeGenerator : ICodeGeneratorStrategy
    {
        public string Type => "dependency-injection";
        
        public string Generate(string @namespace, string className)
        {
            var template = @"using Microsoft.Extensions.DependencyInjection;

namespace {{Namespace}}
{
    /// <summary>
    /// 依赖注入配置
    /// </summary>
    public static class {{ClassName}}
    {
        /// <summary>
        /// 添加依赖项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            // 注册服务
            // 示例：
            // services.AddScoped<IService, Service>();
            // services.AddSingleton<IRepository, Repository>();
            // services.AddTransient<IFactory, Factory>();
            
            // 使用 Scrutor 扫描程序集
            // services.Scan(scan => scan
            //     .FromAssemblyOf<Program>()
            //     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            // );
            
            // 添加装饰器
            // services.Decorate<IService, ServiceDecorator>();
            
            return services;
        }
        
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="assemblyPattern">程序集匹配模式</param>
        /// <param name="typePattern">类型匹配模式</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, string assemblyPattern = "*", string typePattern = "*Service", ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            // 注册服务
            // 示例：
            // services.Scan(scan => scan
            //     .FromAssembliesOfType<IService>()
            //     .AddClasses(classes => classes.Where(type => type.Name.Contains(typePattern)))
            //     .AsImplementedInterfaces()
            //     .WithLifetime(lifetime)
            // );
            
            return services;
        }
    }
}";
            
            return template
                .Replace("{{Namespace}}", @namespace)
                .Replace("{{ClassName}}", className);
        }
    }
    
    public class ServiceCollectionCodeGenerator : ICodeGeneratorStrategy
    {
        public string Type => "service-collection";
        
        public string Generate(string @namespace, string className)
        {
            var template = @"using Microsoft.Extensions.DependencyInjection;

namespace {{Namespace}}
{
    /// <summary>
    /// 服务集合配置
    /// </summary>
    public class {{ClassName}}
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // 注册服务
            // 示例：
            // services.AddScoped<IService, Service>();
            // services.AddSingleton<IRepository, Repository>();
            // services.AddTransient<IFactory, Factory>();
            
            // 使用 Scrutor 扫描程序集
            // services.Scan(scan => scan
            //     .FromApplicationDependencies()
            //     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            // );
            
            // 添加装饰器
            // services.Decorate<IService, ServiceDecorator>();
            
            return services;
        }
        
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="assemblyPattern">程序集匹配模式</param>
        /// <param name="typePattern">类型匹配模式</param>
        /// <param name="lifetime">服务生命周期</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServices(string assemblyPattern = "*", string typePattern = "*Service", ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var services = new ServiceCollection();
            
            // 注册服务
            // 示例：
            // services.Scan(scan => scan
            //     .FromAssembliesInPath(Directory.GetCurrentDirectory())
            //     .AddClasses(classes => classes.Where(type => type.Name.Contains(typePattern)))
            //     .AsImplementedInterfaces()
            //     .WithLifetime(lifetime)
            // );
            
            return services;
        }
    }
}";
            
            return template
                .Replace("{{Namespace}}", @namespace)
                .Replace("{{ClassName}}", className);
        }
    }
    
    public class WebApplicationCodeGenerator : ICodeGeneratorStrategy
    {
        public string Type => "web-application";
        
        public string Generate(string @namespace, string className)
        {
            var template = @"using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace {{Namespace}}
{
    /// <summary>
    /// Web 应用配置
    /// </summary>
    public class {{ClassName}}
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="builder">Web 应用构建器</param>
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            // 注册服务
            // 示例：
            // builder.Services.AddScoped<IService, Service>();
            // builder.Services.AddSingleton<IRepository, Repository>();
            // builder.Services.AddTransient<IFactory, Factory>();
            
            // 使用 Scrutor 扫描程序集
            // builder.Services.Scan(scan => scan
            //     .FromAssemblyOf<Program>()
            //     .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            //     .AsImplementedInterfaces()
            //     .WithScopedLifetime()
            // );
            
            // 添加装饰器
            // builder.Services.Decorate<IService, ServiceDecorator>();
            
            // 配置其他服务
            // builder.Services.AddControllers();
            // builder.Services.AddSwaggerGen();
            // builder.Services.AddAuthentication();
            // builder.Services.AddAuthorization();
        }
        
        /// <summary>
        /// 配置管道
        /// </summary>
        /// <param name="app">Web 应用</param>
        public static void ConfigurePipeline(WebApplication app)
        {
            // 配置中间件
            // if (app.Environment.IsDevelopment())
            // {
            //     app.UseSwagger();
            //     app.UseSwaggerUI();
            // }
            
            // app.UseHttpsRedirection();
            // app.UseAuthentication();
            // app.UseAuthorization();
            // app.MapControllers();
        }
        
        /// <summary>
        /// 构建 Web 应用
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>Web 应用</returns>
        public static WebApplication BuildWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 配置服务
            ConfigureServices(builder);
            
            var app = builder.Build();
            
            // 配置管道
            ConfigurePipeline(app);
            
            return app;
        }
    }
}";
            
            return template
                .Replace("{{Namespace}}", @namespace)
                .Replace("{{ClassName}}", className);
        }
    }
}
