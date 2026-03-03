#:sdk Microsoft.NET.Sdk
#:package Scrutor@4.2.0
#:package System.CommandLine@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.Logging@8.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package System.IO.Abstractions@17.0.21
#:package Newtonsoft.Json@13.0.3
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property SelfContained=true
#:property PublishSingleFile=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Abstractions;
using Newtonsoft.Json;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace TailwindCSS.Generator
{
    // 代码生成相关接口
    public interface ICodeGeneratorService
    {
        Task<string> GenerateComponentAsync(string componentName, string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        Task<string> GenerateConfigurationAsync(string configType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        Task<string> GenerateStyleAsync(string styleType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        Task<string> GeneratePluginAsync(string pluginName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
    }

    public interface ITemplateService
    {
        Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default);
        void RegisterTemplate(string templateName, string templateContent);
        IEnumerable<string> GetAvailableTemplates();
    }

    public interface IScrutorDemoService
    {
        Task DemonstrateScrutorUsageAsync(CancellationToken cancellationToken = default);
    }

    // 实现类
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ITemplateService _templateService;
        private readonly IFileSystem _fileSystem;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CodeGeneratorService> _logger;

        public CodeGeneratorService(
            ITemplateService templateService,
            IFileSystem fileSystem,
            IMemoryCache cache,
            ILogger<CodeGeneratorService> logger)
        {
            _templateService = templateService;
            _fileSystem = fileSystem;
            _cache = cache;
            _logger = logger;
        }

        public async Task<string> GenerateComponentAsync(string componentName, string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating component {Component} using template {Template}", componentName, templateName);

            // 添加组件名称到参数
            parameters["ComponentName"] = componentName;
            parameters["ComponentNameLower"] = componentName.ToLower();
            parameters["ComponentNameCamel"] = char.ToLower(componentName[0]) + componentName.Substring(1);

            // 渲染模板
            var result = await _templateService.RenderTemplateAsync(templateName, parameters, cancellationToken);

            _logger.LogInformation("Component {Component} generated successfully", componentName);
            return result;
        }

        public async Task<string> GenerateConfigurationAsync(string configType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating configuration of type {ConfigType}", configType);

            // 渲染模板
            var templateName = $"config_{configType}";
            var result = await _templateService.RenderTemplateAsync(templateName, parameters, cancellationToken);

            _logger.LogInformation("Configuration {ConfigType} generated successfully", configType);
            return result;
        }

        public async Task<string> GenerateStyleAsync(string styleType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating style of type {StyleType}", styleType);

            // 渲染模板
            var templateName = $"style_{styleType}";
            var result = await _templateService.RenderTemplateAsync(templateName, parameters, cancellationToken);

            _logger.LogInformation("Style {StyleType} generated successfully", styleType);
            return result;
        }

        public async Task<string> GeneratePluginAsync(string pluginName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Generating plugin {Plugin}", pluginName);

            // 添加插件名称到参数
            parameters["PluginName"] = pluginName;
            parameters["PluginNameLower"] = pluginName.ToLower();

            // 渲染模板
            var result = await _templateService.RenderTemplateAsync("plugin_default", parameters, cancellationToken);

            _logger.LogInformation("Plugin {Plugin} generated successfully", pluginName);
            return result;
        }
    }

    public class TemplateService : ITemplateService
    {
        private readonly Dictionary<string, string> _templates = new Dictionary<string, string>();
        private readonly IFileSystem _fileSystem;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TemplateService> _logger;

        public TemplateService(IFileSystem fileSystem, IMemoryCache cache, ILogger<TemplateService> logger)
        {
            _fileSystem = fileSystem;
            _cache = cache;
            _logger = logger;
            RegisterDefaultTemplates();
        }

        public Task<string> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default)
        {
            if (_templates.TryGetValue(templateName, out var template))
            {
                return Task.FromResult(template);
            }

            throw new KeyNotFoundException($"Template not found: {templateName}");
        }

        public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            var template = await GetTemplateAsync(templateName, cancellationToken);
            var result = template;

            // 简单的模板渲染
            foreach (var (key, value) in parameters)
            {
                result = result.Replace($"{{{{{key}}}}}", value?.ToString() ?? string.Empty);
            }

            return result;
        }

        public void RegisterTemplate(string templateName, string templateContent)
        {
            _templates[templateName] = templateContent;
            _logger.LogInformation("Template {Template} registered successfully", templateName);
        }

        public IEnumerable<string> GetAvailableTemplates()
        {
            return _templates.Keys;
        }

        private void RegisterDefaultTemplates()
        {
            // 注册默认模板
            RegisterTemplate("component_button", @"/* Button Component */
@layer components {
  .btn-{{ComponentNameLower}} {
    @apply px-4 py-2 rounded font-medium transition-colors duration-200;
  }
  
  .btn-{{ComponentNameLower}}-primary {
    @apply bg-blue-500 text-white hover:bg-blue-600;
  }
  
  .btn-{{ComponentNameLower}}-secondary {
    @apply bg-gray-500 text-white hover:bg-gray-600;
  }
}");

            RegisterTemplate("component_card", @"/* Card Component */
@layer components {
  .card-{{ComponentNameLower}} {
    @apply bg-white rounded-lg shadow-md p-6 border border-gray-200;
  }
  
  .card-{{ComponentNameLower}}-header {
    @apply text-xl font-bold mb-4;
  }
  
  .card-{{ComponentNameLower}}-body {
    @apply text-gray-600;
  }
  
  .card-{{ComponentNameLower}}-footer {
    @apply mt-4 pt-4 border-t border-gray-200;
  }
}");

            RegisterTemplate("config_tailwind", @"/* Tailwind Configuration */
module.exports = {
  content: [
    './**/*.html',
    './**/*.cshtml',
    './**/*.razor',
    './**/*.js',
    './**/*.ts'
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#eff6ff',
          100: '#dbeafe',
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6',
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a'
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['Fira Code', 'monospace']
      }
    }
  },
  plugins: []
};");

            RegisterTemplate("style_utilities", @"/* Custom Utilities */
@layer utilities {
  .content-auto {
    content-visibility: auto;
  }
  
  .text-shadow {
    text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
  }
  
  .transition-all-300 {
    transition: all 300ms ease-in-out;
  }
}");

            RegisterTemplate("plugin_default", @"/* {{PluginName}} Plugin */
public class {{PluginName}}Plugin : IPlugin
{
    public string Name => "{{PluginName}}Plugin";
    
    public Task ExecuteAsync(string input, CancellationToken cancellationToken = default)
    {
        // Plugin logic here
        return Task.CompletedTask;
    }
    
    public void Configure(PluginConfiguration configuration)
    {
        // Configuration logic here
    }
}");
        }
    }

    // Scrutor 演示服务
    public class ScrutorDemoService : IScrutorDemoService
    {
        private readonly ILogger<ScrutorDemoService> _logger;

        public ScrutorDemoService(ILogger<ScrutorDemoService> logger)
        {
            _logger = logger;
        }

        public async Task DemonstrateScrutorUsageAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Demonstrating Scrutor usage patterns");

            // 创建服务容器
            var services = new ServiceCollection();

            // 1. 基本扫描注册
            _logger.LogInformation("1. Basic scan registration");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses()
                .AsSelf()
                .WithTransientLifetime()
            );

            // 2. 按约定注册
            _logger.LogInformation("2. Convention-based registration");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Provider")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            // 3. 按命名空间注册
            _logger.LogInformation("3. Namespace-based registration");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses(classes => classes.InNamespaces("TailwindCSS.Generator.Services"))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            // 4. 按属性注册
            _logger.LogInformation("4. Attribute-based registration");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses(classes => classes.WithAttribute<RegisterServiceAttribute>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // 5. 装饰器模式
            _logger.LogInformation("5. Decorator pattern");
            services.AddTransient<ICodeGeneratorService, CodeGeneratorService>();
            services.Decorate<ICodeGeneratorService, CodeGeneratorLoggingDecorator>();
            services.Decorate<ICodeGeneratorService, CodeGeneratorCachingDecorator>();

            // 6. 多重装饰器
            _logger.LogInformation("6. Multiple decorators");
            services.AddTransient<ITemplateService, TemplateService>();
            services.Decorate<ITemplateService, TemplateServiceLoggingDecorator>();
            services.Decorate<ITemplateService, TemplateServiceCachingDecorator>();
            services.Decorate<ITemplateService, TemplateServiceValidationDecorator>();

            // 7. 开放泛型
            _logger.LogInformation("7. Open generics");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses(classes => classes.Where(type => type.IsGenericTypeDefinition))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            // 8. 自定义注册规则
            _logger.LogInformation("8. Custom registration rules");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                .AddClasses(classes => classes.Where(type => 
                    type.GetInterfaces().Any(i => i.Name.StartsWith("I")) &&
                    !type.IsAbstract &&
                    !type.IsInterface
                ))
                .As(t => t.GetInterfaces().Where(i => i.Name == "I" + t.Name))
                .WithScopedLifetime()
            );

            // 9. 组合多种注册方式
            _logger.LogInformation("9. Combined registration methods");
            services.Scan(scan => scan
                .FromAssemblyOf<ScrutorDemoService>()
                // 注册服务
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                // 注册仓库
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                // 注册单例
                .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Singleton")))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            // 10. 装饰器链
            _logger.LogInformation("10. Decorator chain");
            services.AddTransient<IScrutorDemoService, ScrutorDemoService>();
            services.Decorate<IScrutorDemoService, ScrutorDemoServiceLoggingDecorator>();

            _logger.LogInformation("Scrutor usage demonstration completed");
            await Task.CompletedTask;
        }
    }

    // 装饰器类
    public class CodeGeneratorLoggingDecorator : ICodeGeneratorService
    {
        private readonly ICodeGeneratorService _inner;
        private readonly ILogger<CodeGeneratorLoggingDecorator> _logger;

        public CodeGeneratorLoggingDecorator(ICodeGeneratorService inner, ILogger<CodeGeneratorLoggingDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<string> GenerateComponentAsync(string componentName, string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Generating component {Component}", componentName);
            var result = await _inner.GenerateComponentAsync(componentName, templateName, parameters, cancellationToken);
            _logger.LogInformation("[Decorator] Component {Component} generated", componentName);
            return result;
        }

        public async Task<string> GenerateConfigurationAsync(string configType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Generating configuration {Config}", configType);
            var result = await _inner.GenerateConfigurationAsync(configType, parameters, cancellationToken);
            _logger.LogInformation("[Decorator] Configuration {Config} generated", configType);
            return result;
        }

        public async Task<string> GenerateStyleAsync(string styleType, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Generating style {Style}", styleType);
            var result = await _inner.GenerateStyleAsync(styleType, parameters, cancellationToken);
            _logger.LogInformation("[Decorator] Style {Style} generated", styleType);
            return result;
        }

        public async Task<string> GeneratePluginAsync(string pluginName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Decorator] Generating plugin {Plugin}", pluginName);
            var result = await _inner.GeneratePluginAsync(pluginName, parameters, cancellationToken);
            _logger.LogInformation("[Decorator] Plugin {Plugin} generated", pluginName);
            return result;
        }
    }

    public class CodeGeneratorCachingDecorator : ICodeGeneratorService
    {
        private readonly ICodeGeneratorService _inner;
        private readonly IMemoryCache _cache;

        public CodeGeneratorCachingDecorator(ICodeGeneratorService inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<string> GenerateComponentAsync(string componentName, string templateName, Dictionary<string, object> parameters, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"component:{componentName}:{templateName}:{JsonConvert.SerializeObject(parameters)}