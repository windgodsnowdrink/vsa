#:sdk Microsoft.NET.Sdk
#:package SSH.NET@2023.0.0
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
using Microsoft.Extensions.DependencyInjection;

namespace SSH.Generator
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null);
        Task GenerateSshScriptAsync(string templateName, string outputPath, Dictionary<string, string> parameters);
        Task<IEnumerable<string>> GetAvailableTemplatesAsync();
    }

    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ITemplateService _templateService;

        public CodeGeneratorService(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
        {
            var parameters = new Dictionary<string, string>
            {
                { "Host", host },
                { "Port", port.ToString() },
                { "Username", username },
                { "PrivateKeyPath", privateKeyPath ?? string.Empty }
            };

            var template = await _templateService.GetTemplateAsync("SshConfig");
            if (template == null)
            {
                throw new ArgumentException("Template 'SshConfig' not found.");
            }

            return _templateService.ProcessTemplate(template, parameters);
        }

        public async Task GenerateSshScriptAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            var template = await _templateService.GetTemplateAsync(templateName);
            if (template == null)
            {
                throw new ArgumentException($"Template '{templateName}' not found.");
            }

            var generatedCode = _templateService.ProcessTemplate(template, parameters);
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await File.WriteAllTextAsync(outputPath, generatedCode, Encoding.UTF8);
        }

        public async Task<IEnumerable<string>> GetAvailableTemplatesAsync()
        {
            return await _templateService.GetTemplateNamesAsync();
        }
    }

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
                // 内置模板
                return GetBuiltInTemplate(name);
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
            var templateNames = files.Select(Path.GetFileNameWithoutExtension).ToList();
            
            // 添加内置模板
            templateNames.AddRange(new[] { "SshConfig", "SshScript", "SshBatchScript" });
            
            return templateNames.Distinct();
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

        private string GetBuiltInTemplate(string name)
        {
            switch (name)
            {
                case "SshConfig":
                    return @"Host {{{Host}}}
  HostName {{{Host}}}
  Port {{{Port}}}
  User {{{Username}}}
{{#if PrivateKeyPath}}
  IdentityFile {{{PrivateKeyPath}}}
{{/if}}";
                case "SshScript":
                    return @"#!/bin/bash

# SSH连接脚本
HOST={{Host}}
PORT={{Port}}
USER={{Username}}

ssh -p $PORT $USER@$HOST";
                case "SshBatchScript":
                    return @"#!/bin/bash

# SSH批量操作脚本
HOSTS=({{Hosts}})
COMMAND="{{Command}}"

for host in "${HOSTS[@]}"; do
  echo "Executing on $host..."
  ssh $host "$COMMAND"
done";
                default:
                    return null;
            }
        }
    }

    public interface ISshServiceRegistry
    {
        void RegisterServices(IServiceCollection services);
        void RegisterDecorators(IServiceCollection services);
        void RegisterExtensions(IServiceCollection services);
    }

    public class SshServiceRegistry : ISshServiceRegistry
    {
        public void RegisterServices(IServiceCollection services)
        {
            // Scrutor用法示例1：按约定注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            // Scrutor用法示例2：按命名空间注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.InNamespaces("SSH.Generator.Services"))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // Scrutor用法示例3：按属性注册服务
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.WithAttribute<ServiceAttribute>())
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

            // Scrutor用法示例6：多层装饰器
            services.Decorate<ICodeGeneratorService>((inner, provider) =>
                new CodeGeneratorCachingDecorator(inner, provider.GetRequiredService<ICacheService>())
            );
        }

        public void RegisterExtensions(IServiceCollection services)
        {
            // Scrutor用法示例7：注册开放泛型
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));

            // Scrutor用法示例8：注册特定实现
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IFileService, FileSystemService>();
            services.AddScoped<ILoggerService, ConsoleLoggerService>();
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServiceAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public ServiceAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public interface IRepository<T>
    {
        Task<T> GetAsync(int id);
        Task SaveAsync(T entity);
        Task DeleteAsync(int id);
    }

    public class Repository<T> : IRepository<T>
    {
        public Task<T> GetAsync(int id)
        {
            return Task.FromResult(default(T)!);
        }

        public Task SaveAsync(T entity)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
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

        public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
        {
            _logger.LogInformation("Generating SSH config for {Host}:{Port}", host, port);
            try
            {
                var result = await _inner.GenerateSshConfigAsync(host, port, username, privateKeyPath);
                _logger.LogInformation("SSH config generated successfully for {Host}:{Port}", host, port);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating SSH config for {Host}:{Port}: {Error}", host, port, ex.Message);
                throw;
            }
        }

        public async Task GenerateSshScriptAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            _logger.LogInformation("Generating SSH script from template {TemplateName} to {OutputPath}", templateName, outputPath);
            try
            {
                await _inner.GenerateSshScriptAsync(templateName, outputPath, parameters);
                _logger.LogInformation("SSH script generated successfully: {OutputPath}", outputPath);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error generating SSH script: {Error}", ex.Message);
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

        public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
        {
            ValidateHost(host);
            ValidatePort(port);
            ValidateUsername(username);
            ValidatePrivateKeyPath(privateKeyPath);
            return await _inner.GenerateSshConfigAsync(host, port, username, privateKeyPath);
        }

        public async Task GenerateSshScriptAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            ValidateTemplateName(templateName);
            ValidateOutputPath(outputPath);
            ValidateParameters(parameters);
            await _inner.GenerateSshScriptAsync(templateName, outputPath, parameters);
        }

        public Task<IEnumerable<string>> GetAvailableTemplatesAsync()
        {
            return _inner.GetAvailableTemplatesAsync();
        }

        private void ValidateHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Host cannot be empty.");
            }
        }

        private void ValidatePort(int port)
        {
            if (port < 1 || port > 65535)
            {
                throw new ArgumentException("Port must be between 1 and 65535.");
            }
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be empty.");
            }
        }

        private void ValidatePrivateKeyPath(string privateKeyPath)
        {
            if (!string.IsNullOrEmpty(privateKeyPath) && !File.Exists(privateKeyPath))
            {
                throw new ArgumentException($"Private key file not found: {privateKeyPath}");
            }
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

    public class CodeGeneratorCachingDecorator : ICodeGeneratorService
    {
        private readonly ICodeGeneratorService _inner;
        private readonly ICacheService _cache;

        public CodeGeneratorCachingDecorator(ICodeGeneratorService inner, ICacheService cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<string> GenerateSshConfigAsync(string host, int port, string username, string privateKeyPath = null)
        {
            var cacheKey = $"ssh-config:{host}:{port}:{username}:{privateKeyPath}";
            var cachedResult = await _cache.GetAsync<string>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var result = await _inner.GenerateSshConfigAsync(host, port, username, privateKeyPath);
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
            return result;
        }

        public Task GenerateSshScriptAsync(string templateName, string outputPath, Dictionary<string, string> parameters)
        {
            return _inner.GenerateSshScriptAsync(templateName, outputPath, parameters);
        }

        public async Task<IEnumerable<string>> GetAvailableTemplatesAsync()
        {
            var cacheKey = "ssh-templates";
            var cachedResult = await _cache.GetAsync<IEnumerable<string>>(cacheKey);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var result = await _inner.GetAvailableTemplatesAsync();
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(1));
            return result;
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

        public async Task<IEnumerable<string>> GetTemplateNamesAsync()
        {
            var cacheKey = "template-names";
            var cachedNames = await _cache.GetAsync<IEnumerable<string>>(cacheKey);
            if (cachedNames != null)
            {
                return cachedNames;
            }

            var names = await _inner.GetTemplateNamesAsync();
            await _cache.SetAsync(cacheKey, names, TimeSpan.FromHours(1));
            return names;
        }

        public string ProcessTemplate(string template, Dictionary<string, string> parameters)
        {
            return _inner.ProcessTemplate(template, parameters);
        }

        public async Task SaveTemplateAsync(string name, string content)
        {
            await _inner.SaveTemplateAsync(name, content);
            var cacheKey = $"template:{name}";
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync("template-names");
        }

        public async Task DeleteTemplateAsync(string name)
        {
            await _inner.DeleteTemplateAsync(name);
            var cacheKey = $"template:{name}";
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync("template-names");
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServiceAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public ServiceAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    [ServiceAttribute("SshConfigGenerator", "Generates SSH config files")]
    public class SshConfigGenerator
    {
        public string GenerateConfig(string host, int port, string username, string privateKeyPath = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Host {host}");
            sb.AppendLine($"  HostName {host}");
            sb.AppendLine($"  Port {port}");
            sb.AppendLine($"  User {username}");
            
            if (!string.IsNullOrEmpty(privateKeyPath))
            {
                sb.AppendLine($"  IdentityFile {privateKeyPath}");
            }
            
            return sb.ToString();
        }
    }

    [ServiceAttribute("SshScriptGenerator", "Generates SSH script files")]
    public class SshScriptGenerator
    {
        public string GenerateScript(string host, int port, string username, string command)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#!/bin/bash");
            sb.AppendLine();
            sb.AppendLine("# SSH执行脚本");
            sb.AppendLine($"HOST={host}");
            sb.AppendLine($"PORT={port}");
            sb.AppendLine($"USER={username}");
            sb.AppendLine($"COMMAND=\"{command}\"");
            sb.AppendLine();
            sb.AppendLine("echo \"Executing command on $HOST:$PORT...\"");
            sb.AppendLine("ssh -p $PORT $USER@$HOST \"$COMMAND\"");
            
            return sb.ToString();
        }
    }

    [ServiceAttribute("SshBatchGenerator", "Generates SSH batch script files")]
    public class SshBatchGenerator
    {
        public string GenerateBatchScript(IEnumerable<string> hosts, string command)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#!/bin/bash");
            sb.AppendLine();
            sb.AppendLine("# SSH批量执行脚本");
            sb.AppendLine($"HOSTS=({string.Join(' ', hosts)})");
            sb.AppendLine($"COMMAND=\"{command}\"");
            sb.AppendLine();
            sb.AppendLine("for host in \"${{HOSTS[@]}}\"; do");
            sb.AppendLine("  echo \"Executing on $host...\"");
            sb.AppendLine("  ssh $host \"$COMMAND\"");
            sb.AppendLine("done");
            
            return sb.ToString();
        }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("SSH Generator CLI Tool");

            // 注册命令
            RegisterCommands(rootCommand);

            // 配置依赖注入
            var services = ConfigureServices();

            // 执行命令
            return await rootCommand.InvokeAsync(args, new ServiceProviderBinder(services.BuildServiceProvider()));
        }

        private static void RegisterCommands(RootCommand rootCommand)
        {
            // 生成SSH配置命令
            var generateConfigCommand = new Command("generate-config", "Generate SSH config file");
            var hostOption = new Option<string>("--host", "Hostname or IP address") { IsRequired = true };
            var portOption = new Option<int>("--port", "SSH port") { DefaultValue = 22 };
            var usernameOption = new Option<string>("--username", "Username") { IsRequired = true };
            var keyfileOption = new Option<string>("--keyfile", "Private key file path");
            var outputOption = new Option<string>("--output", "Output file path") { IsRequired = true };

            generateConfigCommand.AddOption(hostOption);
            generateConfigCommand.AddOption(portOption);
            generateConfigCommand.AddOption(usernameOption);
            generateConfigCommand.AddOption(keyfileOption);
            generateConfigCommand.AddOption(outputOption);

            generateConfigCommand.SetHandler(async (context) =>
            {
                var host = context.ParseResult.GetValueForOption(hostOption);
                var port = context.ParseResult.GetValueForOption(portOption);
                var username = context.ParseResult.GetValueForOption(usernameOption);
                var keyfile = context.ParseResult.GetValueForOption(keyfileOption);
                var output = context.ParseResult.GetValueForOption(outputOption);

                var generatorService = context.BindingContext.GetService<ICodeGeneratorService>();
                var config = await generatorService.GenerateSshConfigAsync(host, port, username, keyfile);

                await File.WriteAllTextAsync(output, config, Encoding.UTF8);
                Console.WriteLine($"SSH config generated successfully to {output}");
            });

            // 生成SSH脚本命令
            var generateScriptCommand = new Command("generate-script", "Generate SSH script file");
            var templateOption = new Option<string>("--template", "Template name") { IsRequired = true };
            var scriptOutputOption = new Option<string>("--output", "Output file path") { IsRequired = true };
            var scriptParametersOption = new Option<string[]>("--parameters", "Script parameters in format key=value");

            generateScriptCommand.AddOption(templateOption);
            generateScriptCommand.AddOption(scriptOutputOption);
            generateScriptCommand.AddOption(scriptParametersOption);

            generateScriptCommand.SetHandler(async (context) =>
            {
                var template = context.ParseResult.GetValueForOption(templateOption);
                var output = context.ParseResult.GetValueForOption(scriptOutputOption);
                var parameters = context.ParseResult.GetValueForOption(scriptParametersOption) ?? Array.Empty<string>();

                var generatorService = context.BindingContext.GetService<ICodeGeneratorService>();
                var paramDict = parameters.ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);

                await generatorService.GenerateSshScriptAsync(template, output, paramDict);
                Console.WriteLine($"SSH script generated successfully to {output}");
            });

            // 列出模板命令
            var listTemplatesCommand = new Command("list-templates", "List available templates");
            listTemplatesCommand.SetHandler(async (context) =>
            {
                var generatorService = context.BindingContext.GetService<ICodeGeneratorService>();
                var templates = await generatorService.GetAvailableTemplatesAsync();

                Console.WriteLine("Available templates:");
                foreach (var template in templates)
                {
                    Console.WriteLine($"- {template}");
                }
            });

            // Scrutor演示命令
            var scrutorDemoCommand = new Command("scrutor-demo", "Demonstrate Scrutor usage");
            scrutorDemoCommand.SetHandler((context) =>
            {
                var registry = context.BindingContext.GetService<ISshServiceRegistry>();
                var services = new ServiceCollection();
                
                Console.WriteLine("=== Scrutor Usage Demo ===");
                Console.WriteLine("1. Registering services...");
                registry.RegisterServices(services);
                
                Console.WriteLine("2. Registering decorators...");
                registry.RegisterDecorators(services);
                
                Console.WriteLine("3. Registering extensions...");
                registry.RegisterExtensions(services);
                
                Console.WriteLine("=== Scrutor Demo Complete ===");
                Console.WriteLine($"Registered {services.Count} services");
            });

            rootCommand.AddCommand(generateConfigCommand);
            rootCommand.AddCommand(generateScriptCommand);
            rootCommand.AddCommand(listTemplatesCommand);
            rootCommand.AddCommand(scrutorDemoCommand);
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // 注册核心服务
            services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<ISshServiceRegistry, SshServiceRegistry>();
            services.AddScoped<ICacheService, MemoryCacheService>();
            services.AddScoped<IFileService, FileSystemService>();
            services.AddScoped<ILoggerService, ConsoleLoggerService>();

            // 使用Scrutor注册服务
            var registry = new SshServiceRegistry();
            registry.RegisterServices(services);
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
}
