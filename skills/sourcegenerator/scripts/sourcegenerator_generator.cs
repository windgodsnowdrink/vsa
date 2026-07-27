#:sdk Microsoft.NET.Sdk
#:package Microsoft.CodeAnalysis.Analyzers@4.10.0
#:package Microsoft.CodeAnalysis.CSharp@4.10.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Scrutor@4.2.2
#:package System.CommandLine@2.0.0
#:package System.Text.Json@10.0.0
#:property TargetFramework=net11.0
#:property LangVersion=preview
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace SourceGenerator.Generator
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters);
        Task GenerateFileAsync(string templateName, string outputPath, Dictionary<string, string> parameters);
        Task<IEnumerable<string>> GetAvailableTemplatesAsync();
    }

    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ITemplateService _templateService;

        public CodeGeneratorService(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters)
        {
            var template = await _templateService.GetTemplateAsync(templateName);
            if (template == null)
            {
                throw new ArgumentException($"Template '{templateName}' not found.");
            }

            return _templateService.ProcessTemplate(template, parameters);
        }

        public async Task GenerateFileAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            var code = await GenerateCodeAsync(templateName, parameters);
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await File.WriteAllTextAsync(outputPath, code, Encoding.UTF8);
        }

        public async Task<IEnumerable<string>> GetAvailableTemplatesAsync()
        {
            return await _templateService.GetTemplateNamesAsync();
        }
    }

    public interface IGeneratorRegistry
    {
        void RegisterGenerators(IServiceCollection services);
        void RegisterDecorators(IServiceCollection services);
        void RegisterExtensions(IServiceCollection services);
    }

    public class GeneratorRegistry : IGeneratorRegistry
    {
        public void RegisterGenerators(IServiceCollection services)
        {
            // Scrutor用法示例1：按约定注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Generator")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            // Scrutor用法示例2：按命名空间注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.InNamespaces("SourceGenerator.Generator.Generators"))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // Scrutor用法示例3：按属性注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.WithAttribute<GeneratorAttribute>())
                .AsSelfWithInterfaces()
                .WithSingletonLifetime()
            );
        }

        public void RegisterDecorators(IServiceCollection services)
        {
            // Scrutor用法示例4：注册装饰器
            services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();
            services.Decorate<ICodeGeneratorService, CodeGeneratorValidationDecorator>();

            // Scrutor用法示例5：带条件的装饰器
            services.Decorate<ITemplateService>((inner, provider) =>
                new TemplateServiceCachingDecorator(inner, provider.GetRequiredService<ICacheService>())
            );
        }

        public void RegisterExtensions(IServiceCollection services)
        {
            // Scrutor用法示例6：注册开放泛型
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

            // Scrutor用法示例7：注册特定实现
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IFileService, FileSystemService>();
            services.AddScoped<ILoggerService, ConsoleLoggerService>();
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class GeneratorAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public GeneratorAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public interface IRepository<T>
    {
        Task<T> GetAsync(int id);
        Task SaveAsync(T entity);
    }

    public class Repository<T> : IRepository<T>
    {
        public Task<T> GetAsync(int id)
        {
            // 实现省略
            return Task.FromResult(default(T)!);
        }

        public Task SaveAsync(T entity)
        {
            // 实现省略
            return Task.CompletedTask;
        }
    }

    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
    }

    public class MemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, (object Value, DateTime Expiration)> _cache = new();

        public Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var item) && item.Expiration > DateTime.Now)
            {
                return Task.FromResult((T?)item.Value);
            }
            _cache.Remove(key);
            return Task.FromResult(default(T));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            _cache[key] = (value!, DateTime.Now.Add(expiration));
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }

    public interface IFileService
    {
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string content);
        Task<bool> ExistsAsync(string path);
        Task<IEnumerable<string>> GetFilesAsync(string directory, string pattern);
    }

    public class FileSystemService : IFileService
    {
        public Task<string> ReadAllTextAsync(string path)
        {
            return File.ReadAllTextAsync(path, Encoding.UTF8);
        }

        public Task WriteAllTextAsync(string path, string content)
        {
            return File.WriteAllTextAsync(path, content, Encoding.UTF8);
        }

        public Task<bool> ExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        public Task<IEnumerable<string>> GetFilesAsync(string directory, string pattern)
        {
            if (!Directory.Exists(directory))
            {
                return Task.FromResult(Enumerable.Empty<string>());
            }
            return Task.FromResult(Directory.GetFiles(directory, pattern).AsEnumerable());
        }
    }

    public interface ILoggerService
    {
        void LogInformation(string message, params object[] args);
        void LogError(string message, params object[] args);
        void LogDebug(string message, params object[] args);
    }

    public class ConsoleLoggerService : ILoggerService
    {
        public void LogInformation(string message, params object[] args)
        {
            Console.WriteLine($"[INFO] {string.Format(message, args)}");
        }

        public void LogError(string message, params object[] args)
        {
            Console.WriteLine($"[ERROR] {string.Format(message, args)}");
        }

        public void LogDebug(string message, params object[] args)
        {
            Console.WriteLine($"[DEBUG] {string.Format(message, args)}");
        }
    }

    public class CodeGeneratorLoggingDecorator : ICodeGeneratorService
    {
        private readonly ICodeGeneratorService _inner;
        private readonly ILoggerService _logger;

        public CodeGeneratorLoggingDecorator(ICodeGeneratorService inner, ILoggerService logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters)
        {
            _logger.LogInformation("Generating code for template: {TemplateName}", templateName);
            try
            {
                var result = await _inner.GenerateCodeAsync(templateName, parameters);
                _logger.LogInformation("Code generated successfully for template: {TemplateName}", templateName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating code for template {TemplateName}: {Error}", templateName, ex.Message);
                throw;
            }
        }

        public async Task GenerateFileAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            _logger.LogInformation("Generating file for template {TemplateName} to {OutputPath}", templateName, outputPath);
            try
            {
                await _inner.GenerateFileAsync(templateName, outputPath, parameters);
                _logger.LogInformation("File generated successfully: {OutputPath}", outputPath);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating file: {Error}", ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAvailableTemplatesAsync()
        {
            _logger.LogInformation("Getting available templates");
            try
            {
                var templates = await _inner.GetAvailableTemplatesAsync();
                _logger.LogInformation("Found {TemplateCount} templates", templates.Count());
                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting available templates: {Error}", ex.Message);
                throw;
            }
        }
    }

    public class CodeGeneratorValidationDecorator : ICodeGeneratorService
    {
        private readonly ICodeGeneratorService _inner;

        public CodeGeneratorValidationDecorator(ICodeGeneratorService inner)
        {
            _inner = inner;
        }

        public async Task<string> GenerateCodeAsync(string templateName, Dictionary<string, string> parameters)
        {
            ValidateTemplateName(templateName);
            ValidateParameters(parameters);
            return await _inner.GenerateCodeAsync(templateName, parameters);
        }

        public async Task GenerateFileAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            ValidateTemplateName(templateName);
            ValidateOutputPath(outputPath);
            ValidateParameters(parameters);
            await _inner.GenerateFileAsync(templateName, outputPath, parameters);
        }

        public Task<IEnumerable<string>> GetAvailableTemplatesAsync()
        {
            return _inner.GetAvailableTemplatesAsync();
        }

        private void ValidateTemplateName(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentException("Template name cannot be empty.");
            }
        }

        private void ValidateOutputPath(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path cannot be empty.");
            }

            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                throw new ArgumentException($"Output directory does not exist: {directory}");
            }
        }

        private void ValidateParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }
        }
    }

    public class TemplateServiceCachingDecorator : ITemplateService
    {
        private readonly ITemplateService _inner;
        private readonly ICacheService _cache;

        public TemplateServiceCachingDecorator(ITemplateService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<string?> GetTemplateAsync(string name)
        {
            var cacheKey = $"template:{name}";
            var cachedTemplate = await _cache.GetAsync<string>(cacheKey);
            if (cachedTemplate != null)
            {
                return cachedTemplate;
            }

            var template = await _inner.GetTemplateAsync(name);
            if (template != null)
            {
                await _cache.SetAsync(cacheKey, template, TimeSpan.FromMinutes(30));
            }
            return template;
        }

        public Task<IEnumerable<string>> GetTemplateNamesAsync()
        {
            return _inner.GetTemplateNamesAsync();
        }

        public string ProcessTemplate(string template, Dictionary<string, string> parameters)
        {
            return _inner.ProcessTemplate(template, parameters);
        }

        public Task SaveTemplateAsync(string name, string content)
        {
            return _inner.SaveTemplateAsync(name, content);
        }

        public Task DeleteTemplateAsync(string name)
        {
            return _inner.DeleteTemplateAsync(name);
        }
    }

    [GeneratorAttribute("ClassGenerator", "Generates C# class files")]
    public class ClassGenerator
    {
        public string GenerateClass(string className, string @namespace, IEnumerable<string> properties)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            
            foreach (var property in properties)
            {
                var parts = property.Split(':');
                var propertyName = parts[0].Trim();
                var propertyType = parts[1].Trim();
                sb.AppendLine($"        public {propertyType} {propertyName} {{ get; set; }}");
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
    }

    [GeneratorAttribute("InterfaceGenerator", "Generates C# interface files")]
    public class InterfaceGenerator
    {
        public string GenerateInterface(string interfaceName, string @namespace, IEnumerable<string> methods)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public interface {interfaceName}");
            sb.AppendLine("    {");
            
            foreach (var method in methods)
            {
                sb.AppendLine($"        {method};");
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
    }

    [GeneratorAttribute("ServiceGenerator", "Generates C# service files")]
    public class ServiceGenerator
    {
        public string GenerateService(string serviceName, string @namespace, string interfaceName, IEnumerable<string> methods)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {serviceName} : {interfaceName}");
            sb.AppendLine("    {");
            
            foreach (var method in methods)
            {
                var methodSignature = method.Replace(";", "");
                sb.AppendLine($"        public {methodSignature}");
                sb.AppendLine("        {");
                sb.AppendLine("            // Implementation goes here");
                sb.AppendLine("        }");
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Source Generator CLI Tool");

            // 注册命令
            RegisterCommands(rootCommand);

            // 配置依赖注入
            var services = ConfigureServices();

            // 执行命令
            return await rootCommand.InvokeAsync(args, new ServiceProviderBinder(services.BuildServiceProvider()));
        }

        private static void RegisterCommands(RootCommand rootCommand)
        {
            // 生成代码命令
            var generateCommand = new Command("generate", "Generate code from template");
            var templateOption = new Option<string>("--template", "Template name") { IsRequired = true };
            var outputOption = new Option<string>("--output", "Output file path") { IsRequired = true };
            var parametersOption = new Option<string[]>("--parameters", "Template parameters in format key=value");

            generateCommand.AddOption(templateOption);
            generateCommand.AddOption(outputOption);
            generateCommand.AddOption(parametersOption);

            generateCommand.SetHandler(async (context) =>
            {
                var template = context.ParseResult.GetValueForOption(templateOption);
                var output = context.ParseResult.GetValueForOption(outputOption);
                var parameters = context.ParseResult.GetValueForOption(parametersOption) ?? Array.Empty<string>();

                var generatorService = context.BindingContext.GetService<ICodeGeneratorService>();
                var paramDict = parameters.ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);

                await generatorService.GenerateFileAsync(template, output, paramDict);
            });

            // 列出模板命令
            var listCommand = new Command("list", "List available templates");
            listCommand.SetHandler(async (context) =>
            {
                var generatorService = context.BindingContext.GetService<ICodeGeneratorService>();
                var templates = await generatorService.GetAvailableTemplatesAsync();

                foreach (var template in templates)
                {
                    Console.WriteLine(template);
                }
            });

            // Scrutor演示命令
            var scrutorDemoCommand = new Command("scrutor-demo", "Demonstrate Scrutor usage");
            scrutorDemoCommand.SetHandler((context) =>
            {
                var registry = context.BindingContext.GetService<IGeneratorRegistry>();
                var services = new ServiceCollection();
                
                Console.WriteLine("=== Scrutor Usage Demo ===");
                Console.WriteLine("1. Registering generators...");
                registry.RegisterGenerators(services);
                
                Console.WriteLine("2. Registering decorators...");
                registry.RegisterDecorators(services);
                
                Console.WriteLine("3. Registering extensions...");
                registry.RegisterExtensions(services);
                
                Console.WriteLine("=== Scrutor Demo Complete ===");
                Console.WriteLine($"Registered {services.Count} services");
            });

            rootCommand.AddCommand(generateCommand);
            rootCommand.AddCommand(listCommand);
            rootCommand.AddCommand(scrutorDemoCommand);
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // 注册核心服务
            services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IGeneratorRegistry, GeneratorRegistry>();
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IFileService, FileSystemService>();
            services.AddScoped<ILoggerService, ConsoleLoggerService>();

            // 注册生成器
            var registry = new GeneratorRegistry();
            registry.RegisterGenerators(services);
            registry.RegisterDecorators(services);
            registry.RegisterExtensions(services);

            return services;
        }
    }

    public class ServiceProviderBinder : BinderBase<InvocationContext>
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderBinder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override InvocationContext GetBoundValue(BindingContext bindingContext)
        {
            bindingContext.AddService(typeof(IServiceProvider), _ => _serviceProvider);
            return bindingContext.GetService<InvocationContext>()!;
        }
    }

    // 从sourcegenerator_core.cs中复制的必要接口和类
    public interface ITemplateService
    {
        Task<string?> GetTemplateAsync(string name);
        Task<IEnumerable<string>> GetTemplateNamesAsync();
        string ProcessTemplate(string template, Dictionary<string, string> parameters);
        Task SaveTemplateAsync(string name, string content);
        Task DeleteTemplateAsync(string name);
    }

    public class TemplateService : ITemplateService
    {
        private readonly string _templatesDirectory = Path.Combine(AppContext.BaseDirectory, "templates");

        public TemplateService()
        {
            if (!Directory.Exists(_templatesDirectory))
            {
                Directory.CreateDirectory(_templatesDirectory);
            }
        }

        public async Task<string?> GetTemplateAsync(string name)
        {
            var templatePath = Path.Combine(_templatesDirectory, $"{name}.tmpl");
            if (!File.Exists(templatePath))
            {
                return null;
            }
            return await File.ReadAllTextAsync(templatePath, Encoding.UTF8);
        }

        public async Task<IEnumerable<string>> GetTemplateNamesAsync()
        {
            if (!Directory.Exists(_templatesDirectory))
            {
                return Enumerable.Empty<string>();
            }

            var files = Directory.GetFiles(_templatesDirectory, "*.tmpl");
            return files.Select(Path.GetFileNameWithoutExtension).ToList();
        }

        public string ProcessTemplate(string template, Dictionary<string, string> parameters)
        {
            var result = template;
            foreach (var (key, value) in parameters)
            {
                result = result.Replace($"{{{{{key}}}}}", value);
            }
            return result;
        }

        public async Task SaveTemplateAsync(string name, string content)
        {
            var templatePath = Path.Combine(_templatesDirectory, $"{name}.tmpl");
            await File.WriteAllTextAsync(templatePath, content, Encoding.UTF8);
        }

        public Task DeleteTemplateAsync(string name)
        {
            var templatePath = Path.Combine(_templatesDirectory, $"{name}.tmpl");
            if (File.Exists(templatePath))
            {
                File.Delete(templatePath);
            }
            return Task.CompletedTask;
        }
    }
}
