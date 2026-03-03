#:sdk Microsoft.NET.Sdk
#:package HandyControl@3.4.0
#:package Microsoft.Extensions.Localization@8.0.0
#:package OpenTelemetry@1.7.0
#:package System.Composition@7.0.0
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Composition;
using System.Composition.Hosting;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Animation;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using Microsoft.Extensions.Localization;
using OpenTelemetry.Metrics;
#:package HandyControl@3.4.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0

public class HandyControlOptions
{
    public string DefaultTheme { get; set; } = "Dark";
    public string DefaultCulture { get; set; } = "zh-CN";
    public bool EnableAnimations { get; set; } = true;
    public bool EnableAccessibility { get; set; } = true;
    public bool EnablePerformanceOptimization { get; set; } = true;
    public string PluginsDirectory { get; set; } = "Plugins";
}

public interface IHandyControlService
{
    void SetTheme(string themeName);
    void SetCulture(string cultureName);
    void RegisterCustomControl(Type controlType);
    void AddAnimation(Storyboard animation);
    void EnableAccessibilityFeatures();
    void OptimizePerformance();
}

public class HandyControlService : IHandyControlService
{
    private readonly CompositionHost _compositionHost;
    private readonly IStringLocalizer _localizer;
    private readonly HandyControlOptions _options;
    private readonly List<Storyboard> _animations = new();

    public HandyControlService(CompositionHost compositionHost, 
                             IStringLocalizer localizer,
                             IOptions<HandyControlOptions> options)
    {
        _compositionHost = compositionHost;
        _localizer = localizer;
        _options = options.Value;
    }

    public void SetTheme(string themeName)
    {
        ThemeManager.Current.ApplicationTheme = new Theme(themeName);
    }

    public void SetCulture(string cultureName)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
    }

    public void RegisterCustomControl(Type controlType)
    {
        ControlHelper.Register(controlType);
    }

    public void AddAnimation(Storyboard animation)
    {
        if(_options.EnableAnimations)
            _animations.Add(animation);
    }

    public void EnableAccessibilityFeatures()
    {
        if(_options.EnableAccessibility)
            AutomationProperties.SetLiveSetting(Application.Current.MainWindow, AutomationLiveSetting.Assertive);
    }

    public void OptimizePerformance()
    {
        if(_options.EnablePerformanceOptimization)
        {
            RenderOptions.ProcessRenderMode = RenderMode.Default;
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(Timeline), 
                new FrameworkPropertyMetadata(60));
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandyControl(this IServiceCollection services, Action<HandyControlOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IHandyControlService, HandyControlService>();
        services.AddLocalization();
        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddMeter("HandyControl.Performance"));
        return services;
    }
}

// 示例用法
// var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddHandyControl(options =>
// {
//     options.DefaultTheme = "Light";
//     options.EnableAnimations = true;
//     options.EnableAccessibility = true;
// });
#:package Microsoft.Extensions.Options@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Collections.Generic;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Themes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class HandyControlOptions
{
    public string Title { get; set; } = "WPF App";
    public int Width { get; set; } = 1024;
    public int Height { get; set; } = 768;
    public bool UseMVVM { get; set; } = true;
    public ThemeType DefaultTheme { get; set; } = ThemeType.Light;
    public Dictionary<string, string> KeyBindings { get; set; } = new();
}

public interface IHandyControlService
{
    Window CreateMainWindow();
    void SetTheme(ThemeType theme);
    ThemeType GetCurrentTheme();
    void RegisterGlobalHotKey(string key, Action callback);
    void UnregisterGlobalHotKey(string key);
}

public class HandyControlService : IHandyControlService
{
    private readonly HandyControlOptions _options;
    private readonly IServiceProvider _serviceProvider;

    public HandyControlService(IOptions<HandyControlOptions> options, IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
    }

    public Window CreateMainWindow()
    {
        var window = new Window
        {
            Title = _options.Title,
            Width = _options.Width,
            Height = _options.Height,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        ThemeManager.Current.ApplicationTheme = _options.DefaultTheme;
        return window;
    }

    public void SetTheme(ThemeType theme)
    {
        ThemeManager.Current.ApplicationTheme = theme;
    }

    public ThemeType GetCurrentTheme()
    {
        return ThemeManager.Current.ApplicationTheme;
    }

    public void RegisterGlobalHotKey(string key, Action callback)
    {
        // 实现全局快捷键注册
    }

    public void UnregisterGlobalHotKey(string key)
    {
        // 实现全局快捷键注销
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandyControl(this IServiceCollection services, Action<HandyControlOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IHandyControlService, HandyControlService>();
        return services;
    }
}

// 示例用法
public class Program
{
    [STAThread]
    public static void Main()
    {
        var services = new ServiceCollection();
        services.AddHandyControl(options =>
        {
            options.Title = "My WPF App";
            options.DefaultTheme = ThemeType.Dark;
        });

        var provider = services.BuildServiceProvider();
        var app = new Application();
        var mainWindow = provider.GetRequiredService<IHandyControlService>().CreateMainWindow();
        app.Run(mainWindow);
    }
}