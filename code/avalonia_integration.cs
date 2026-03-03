#:sdk Microsoft.NET.Sdk
#:package Avalonia@11.0.5
#:package Avalonia.Desktop@11.0.5
#:package Avalonia.ReactiveUI@11.0.5
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Composition@7.0.0
#:package OpenTelemetry@1.6.0
#:package OpenTelemetry.Extensions.Hosting@1.0.0
#:package Microsoft.Extensions.Localization@8.0.0
#:property LangVersion preview
#:property TargetFramework net8.0
#:property Nullable enable

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;

namespace AvaloniaIntegration
{
    public class AvaloniaOptions
    {
        public string Title { get; set; } = "Avalonia App";
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;
        public bool UseReactiveUI { get; set; } = true;
        public string DefaultTheme { get; set; } = "Light";
        public string DefaultCulture { get; set; } = "en-US";
        public string PluginsDirectory { get; set; } = "Plugins";
        public bool EnableAOT { get; set; } = false;
        public bool EnableHotReload { get; set; } = true;
        public bool EnableAccessibility { get; set; } = true;
    }

    public interface IAvaloniaService
    {
        void Initialize();
        void Run();
        void SetTheme(string themeName);
        string GetString(string key);
        Window CreateWindow();
        IEnumerable<object> GetPlugins();
    }

    public class AvaloniaService : IAvaloniaService
    {
        private readonly AvaloniaOptions _options;
        private readonly CompositionHost _compositionHost;
        private readonly IStringLocalizer _localizer;
        private AppBuilder _appBuilder;
        private List<Window> _windows = new();
        private ThemeVariant _currentTheme;

        public AvaloniaService(
            AvaloniaOptions options, 
            CompositionHost compositionHost,
            IStringLocalizerFactory localizerFactory)
        {
            _options = options;
            _compositionHost = compositionHost;
            _localizer = localizerFactory.Create("AvaloniaUI", typeof(AvaloniaService).Assembly.FullName);
            
            // 初始化主题
            _currentTheme = _options.DefaultTheme == "Dark" ? 
                ThemeVariant.Dark : ThemeVariant.Light;
        }

        public void Initialize()
        {
            _appBuilder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

            if (_options.UseReactiveUI)
            {
                _appBuilder.UseReactiveUI();
            }
            
            // 配置AOT
            if (_options.EnableAOT)
            {
                _appBuilder.WithAotCompilation();
            }
            
            // 配置热重载
            if (_options.EnableHotReload)
            {
                _appBuilder.WithHotReload();
            }
            
            // 配置无障碍支持
            if (_options.EnableAccessibility)
            {
                _appBuilder.WithAccessibilitySupport();
            }
        }

        public void Run()
        {
            _appBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>());
        }
        
        public void SetTheme(string themeName)
        {
            _currentTheme = themeName == "Dark" ? 
                ThemeVariant.Dark : ThemeVariant.Light;
            Application.Current.RequestedThemeVariant = _currentTheme;
        }
        
        public string GetString(string key)
        {
            return _localizer[key];
        }
        
        public Window CreateWindow()
        {
            var window = new Window
            {
                Title = _options.Title,
                Width = _options.Width,
                Height = _options.Height
            };
            
            _windows.Add(window);
            return window;
        }
        
        public IEnumerable<object> GetPlugins()
        {
            return _compositionHost.GetExports<object>();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAvalonia(this IServiceCollection services, Action<AvaloniaOptions> configure)
        {
            var options = new AvaloniaOptions();
            configure(options);
            services.AddSingleton(options);
            services.AddSingleton<IAvaloniaService, AvaloniaService>();
            
            // 添加本地化服务
            services.AddLocalization();
            
            // 添加OpenTelemetry性能监控
            services.AddOpenTelemetry()
                .WithTracing(builder => builder
                    .AddAvaloniaInstrumentation()
                    .AddConsoleExporter());
                    
            // 添加插件系统
            var compositionHost = new ContainerConfiguration()
                .WithAssembliesInPath(options.PluginsDirectory)
                .CreateContainer();
            services.AddSingleton(compositionHost);
            
            return services;
        }
    }

    public class App : Application
    {
        private readonly AvaloniaOptions _options;

        public App(AvaloniaOptions options)
        {
            _options = options;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    Width = _options.Width,
                    Height = _options.Height,
                    Title = _options.Title
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class MainViewModel : ReactiveObject
    {
        private string _greeting = "Welcome to Avalonia!";
        public string Greeting
        {
            get => _greeting;
            set => this.RaiseAndSetIfChanged(ref _greeting, value);
        }
    }

    public class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            ViewModel = new MainViewModel();
            this.WhenActivated(disposables => { });
            AvaloniaXamlLoader.Load(this);
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddAvalonia(options =>
            {
                options.Title = "Avalonia Production App";
                options.Width = 1024;
                options.Height = 768;
            });

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<IAvaloniaService>();
            service.Run();
        }
    }
}