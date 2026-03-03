#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Scrutor@4.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UI.Generator
{
    public interface IUIComponent
    {
        string Name { get; }
        void Render();
    }

    public interface IThemeProvider
    {
        string GetTheme();
        void SetTheme(string theme);
    }

    public interface ILocalizationProvider
    {
        string GetString(string key);
        void SetCulture(string culture);
    }

    public interface IPerformanceTracker
    {
        void Track(string operation);
        void StopTracking(string operation);
    }

    public interface IAccessibilityValidator
    {
        bool Validate(IUIComponent component);
        IEnumerable<string> GetIssues();
    }

    public class ButtonComponent : IUIComponent
    {
        public string Name => "Button";

        public void Render()
        {
            Console.WriteLine($"Rendering {Name} component");
        }
    }

    public class LabelComponent : IUIComponent
    {
        public string Name => "Label";

        public void Render()
        {
            Console.WriteLine($"Rendering {Name} component");
        }
    }

    public class TextBoxComponent : IUIComponent
    {
        public string Name => "TextBox";

        public void Render()
        {
            Console.WriteLine($"Rendering {Name} component");
        }
    }

    public class DefaultThemeProvider : IThemeProvider
    {
        private string _theme = "Light";

        public string GetTheme()
        {
            return _theme;
        }

        public void SetTheme(string theme)
        {
            _theme = theme;
            Console.WriteLine($"Theme set to: {_theme}");
        }
    }

    public class DefaultLocalizationProvider : ILocalizationProvider
    {
        private string _culture = "en-US";

        public string GetString(string key)
        {
            return $"[{_culture}] {key}";
        }

        public void SetCulture(string culture)
        {
            _culture = culture;
            Console.WriteLine($"Culture set to: {_culture}");
        }
    }

    public class DefaultPerformanceTracker : IPerformanceTracker
    {
        private readonly Dictionary<string, DateTime> _operations = new Dictionary<string, DateTime>();

        public void Track(string operation)
        {
            _operations[operation] = DateTime.UtcNow;
            Console.WriteLine($"Started tracking: {operation}");
        }

        public void StopTracking(string operation)
        {
            if (_operations.TryGetValue(operation, out var startTime))
            {
                var duration = DateTime.UtcNow - startTime;
                _operations.Remove(operation);
                Console.WriteLine($"Stopped tracking: {operation}, Duration: {duration.TotalMilliseconds:F2}ms");
            }
        }
    }

    public class DefaultAccessibilityValidator : IAccessibilityValidator
    {
        private readonly List<string> _issues = new List<string>();

        public bool Validate(IUIComponent component)
        {
            _issues.Clear();
            
            // 模拟无障碍验证
            if (component.Name == "Button")
            {
                _issues.Add("Button missing aria-label");
            }
            
            if (component.Name == "TextBox")
            {
                _issues.Add("TextBox missing aria-required");
            }
            
            return _issues.Count == 0;
        }

        public IEnumerable<string> GetIssues()
        {
            return _issues;
        }
    }

    public class LoggingDecorator<T> : IUIComponent where T : IUIComponent
    {
        private readonly T _component;
        private readonly ILogger<LoggingDecorator<T>> _logger;

        public LoggingDecorator(T component, ILogger<LoggingDecorator<T>> logger)
        {
            _component = component;
            _logger = logger;
        }

        public string Name => _component.Name;

        public void Render()
        {
            _logger.LogInformation("Before rendering {Component}", Name);
            _component.Render();
            _logger.LogInformation("After rendering {Component}", Name);
        }
    }

    public class PerformanceDecorator<T> : IUIComponent where T : IUIComponent
    {
        private readonly T _component;
        private readonly IPerformanceTracker _tracker;

        public PerformanceDecorator(T component, IPerformanceTracker tracker)
        {
            _component = component;
            _tracker = tracker;
        }

        public string Name => _component.Name;

        public void Render()
        {
            var operationName = $"Render_{Name}";
            _tracker.Track(operationName);
            _component.Render();
            _tracker.StopTracking(operationName);
        }
    }

    public class AccessibilityDecorator<T> : IUIComponent where T : IUIComponent
    {
        private readonly T _component;
        private readonly IAccessibilityValidator _validator;

        public AccessibilityDecorator(T component, IAccessibilityValidator validator)
        {
            _component = component;
            _validator = validator;
        }

        public string Name => _component.Name;

        public void Render()
        {
            _component.Render();
            
            if (!_validator.Validate(this))
            {
                Console.WriteLine($"Accessibility issues found for {Name}:");
                foreach (var issue in _validator.GetIssues())
                {
                    Console.WriteLine($"  - {issue}");
                }
            }
        }
    }

    public class UIComponentFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UIComponentFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>() where T : IUIComponent
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public IUIComponent Create(Type componentType)
        {
            return (IUIComponent)_serviceProvider.GetRequiredService(componentType);
        }
    }

    public class ScrutorDemo
    {
        private readonly IServiceProvider _serviceProvider;

        public ScrutorDemo(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void RunBasicRegistrationDemo()
        {
            Console.WriteLine("\n=== Basic Registration Demo ===");
            
            var button = _serviceProvider.GetRequiredService<ButtonComponent>();
            button.Render();
            
            var label = _serviceProvider.GetRequiredService<LabelComponent>();
            label.Render();
        }

        public void RunDecoratorPatternDemo()
        {
            Console.WriteLine("\n=== Decorator Pattern Demo ===");
            
            var button = _serviceProvider.GetRequiredService<IUIComponent>();
            button.Render();
        }

        public void RunServiceFilteringDemo()
        {
            Console.WriteLine("\n=== Service Filtering Demo ===");
            
            var themeProvider = _serviceProvider.GetRequiredService<IThemeProvider>();
            Console.WriteLine($"Current theme: {themeProvider.GetTheme()}");
            themeProvider.SetTheme("Dark");
            Console.WriteLine($"New theme: {themeProvider.GetTheme()}");
        }

        public void RunLifetimeManagementDemo()
        {
            Console.WriteLine("\n=== Lifetime Management Demo ===");
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedTracker1 = scope.ServiceProvider.GetRequiredService<IPerformanceTracker>();
                var scopedTracker2 = scope.ServiceProvider.GetRequiredService<IPerformanceTracker>();
                
                Console.WriteLine($"Same instance: {ReferenceEquals(scopedTracker1, scopedTracker2)}");
                
                scopedTracker1.Track("ScopeOperation");
                scopedTracker2.StopTracking("ScopeOperation");
            }
        }

        public void RunAdvancedRegistrationDemo()
        {
            Console.WriteLine("\n=== Advanced Registration Demo ===");
            
            var factory = _serviceProvider.GetRequiredService<UIComponentFactory>();
            var textBox = factory.Create<TextBoxComponent>();
            textBox.Render();
        }

        public void RunAssemblyScanningDemo()
        {
            Console.WriteLine("\n=== Assembly Scanning Demo ===");
            
            var components = _serviceProvider.GetServices<IUIComponent>();
            foreach (var component in components)
            {
                Console.WriteLine($"Found component: {component.Name}");
            }
        }

        public void RunMultipleDecoratorsDemo()
        {
            Console.WriteLine("\n=== Multiple Decorators Demo ===");
            
            var component = _serviceProvider.GetRequiredService<IUIComponent>();
            component.Render();
        }

        public void RunConditionalRegistrationDemo()
        {
            Console.WriteLine("\n=== Conditional Registration Demo ===");
            
            var localizationProvider = _serviceProvider.GetRequiredService<ILocalizationProvider>();
            Console.WriteLine($"Current culture: {localizationProvider.GetString("Test")}");
            localizationProvider.SetCulture("zh-CN");
            Console.WriteLine($"New culture: {localizationProvider.GetString("Test")}");
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("UI Generator with Scrutor Examples");
            Console.WriteLine("====================================");

            // 构建服务容器
            var services = new ServiceCollection();

            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            // 基本注册
            services.AddTransient<ButtonComponent>();
            services.AddTransient<LabelComponent>();
            services.AddTransient<TextBoxComponent>();

            // 注册基础服务
            services.AddTransient<IThemeProvider, DefaultThemeProvider>();
            services.AddTransient<ILocalizationProvider, DefaultLocalizationProvider>();
            services.AddScoped<IPerformanceTracker, DefaultPerformanceTracker>();
            services.AddSingleton<IAccessibilityValidator, DefaultAccessibilityValidator>();

            // 装饰器模式 - 单个装饰器
            services.AddTransient<IUIComponent, ButtonComponent>();
            services.Decorate<IUIComponent, LoggingDecorator<ButtonComponent>>();

            // 装饰器模式 - 多个装饰器
            services.AddTransient<IUIComponent, TextBoxComponent>(serviceProvider =>
            {
                var textBox = new TextBoxComponent();
                var logger = serviceProvider.GetRequiredService<ILogger<LoggingDecorator<TextBoxComponent>>>();
                var tracker = serviceProvider.GetRequiredService<IPerformanceTracker>();
                var validator = serviceProvider.GetRequiredService<IAccessibilityValidator>();

                // 应用多个装饰器
                var loggingDecorator = new LoggingDecorator<TextBoxComponent>(textBox, logger);
                var performanceDecorator = new PerformanceDecorator<LoggingDecorator<TextBoxComponent>>(loggingDecorator, tracker);
                return new AccessibilityDecorator<PerformanceDecorator<LoggingDecorator<TextBoxComponent>>>(performanceDecorator, validator);
            });

            // 程序集扫描 - 自动注册实现了IUIComponent的类型
            services.Scan(scan => scan
                .FromAssemblyOf<Program>()
                .AddClasses(classes => classes.Where(type => typeof(IUIComponent).IsAssignableFrom(type) && type != typeof(IUIComponent)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            // 条件注册
            services.AddTransient<ILocalizationProvider>(serviceProvider =>
            {
                // 根据环境变量或配置决定使用哪个实现
                var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
                
                if (environment == "Production")
                {
                    Console.WriteLine("Using production localization provider");
                }
                
                return new DefaultLocalizationProvider();
            });

            // 注册工厂
            services.AddTransient<UIComponentFactory>();

            // 构建服务提供程序
            var serviceProvider = services.BuildServiceProvider();

            // 创建并运行演示
            var demo = new ScrutorDemo(serviceProvider);
            
            demo.RunBasicRegistrationDemo();
            demo.RunDecoratorPatternDemo();
            demo.RunServiceFilteringDemo();
            demo.RunLifetimeManagementDemo();
            demo.RunAdvancedRegistrationDemo();
            demo.RunAssemblyScanningDemo();
            demo.RunMultipleDecoratorsDemo();
            demo.RunConditionalRegistrationDemo();

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
