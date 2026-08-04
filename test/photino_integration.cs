#:sdk Microsoft.NET.Sdk.Web
#:package Photino.NET@2.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package CommunityToolkit.Mvvm@8.2.0
#:package Microsoft.Data.Sqlite@8.0.0
#:package Microsoft.EntityFrameworkCore.Sqlite@8.0.0
#:package System.IO.Pipelines@8.0.0
#:package System.IO.MemoryMappedFiles@8.0.0
#:package System.Composition@8.0.0
#:package Microsoft.Extensions.Localization@8.0.0
#:package Squirrel@2.0.0
#:package OpenTelemetry@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Photino.NET;
using CommunityToolkit.Mvvm.ComponentModel;

public class PhotinoOptions
{
    // IPC配置
    public string PipeName { get; set; } = "photino_pipe";
    public int SharedMemorySize { get; set; } = 1024 * 1024;
    
    // 主题配置
    public string DefaultTheme { get; set; } = "light";
    
    // 快捷键配置
    public Dictionary<string, string> Shortcuts { get; set; } = new();
    
    // 自动更新配置
    public string UpdateUrl { get; set; } = "https://api.example.com/updates";
    
    // 崩溃报告配置
    public string CrashReportUrl { get; set; } = "https://api.example.com/crash-reports";
    
    // 性能监控配置
    public bool EnablePerformanceMonitoring { get; set; } = true;
    
    // 多语言配置
    public string DefaultCulture { get; set; } = "en-US";
    public string Title { get; set; } = "Photino App";
    public int Width { get; set; } = 1200;
    public int Height { get; set; } = 900;
    public string StartUrl { get; set; } = "wwwroot/index.html";
    public bool UseDeveloperTools { get; set; } = true;
    public string LocalStoragePath { get; set; } = "localstorage.db";
    public int MaxWindows { get; set; } = 5;
}

public interface IPhotinoService
{
    // IPC通信
    Task SendMessageAsync(string message);
    Task<string> ReceiveMessageAsync();
    
    // 插件管理
    Task LoadPluginAsync(string pluginPath);
    Task UnloadPluginAsync(string pluginId);
    
    // 主题管理
    Task SetThemeAsync(string themeName);
    
    // 快捷键绑定
    Task RegisterShortcutAsync(string shortcut, string command);
    
    // 自动更新
    Task CheckForUpdatesAsync();
    Task ApplyUpdateAsync();
    
    // 崩溃报告
    Task SendCrashReportAsync(Exception ex);
    
    // 性能监控
    Task<PerformanceMetrics> GetPerformanceMetricsAsync();
    
    // 多语言支持
    Task SetCultureAsync(string culture);
{
    Task RunAsync();
    Task<PhotinoWindow> CreateWindowAsync(string title, string url, int width, int height);
    Task CloseWindowAsync(string windowId);
    Task SaveDataAsync<T>(string key, T value);
    Task<T?> LoadDataAsync<T>(string key);
}

public class PhotinoService : IPhotinoService, IDisposable
{
    // IPC通信实现
    private readonly NamedPipeServerStream _pipeServer;
    private readonly MemoryMappedFile _sharedMemory;
    
    // 插件系统
    private readonly CompositionContainer _pluginContainer;
    private readonly Dictionary<string, object> _loadedPlugins = new();
    
    // 主题管理
    private string _currentTheme;
    
    // 性能监控
    private readonly PerformanceCounter _fpsCounter = new();
    private readonly Timer _memoryMonitor;
    
    // 多语言支持
    private IStringLocalizer _localizer;
    private readonly IOptions<PhotinoOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly LocalStorageContext _dbContext;
    private readonly ConcurrentDictionary<string, PhotinoWindow> _windows = new();

    public PhotinoService(IOptions<PhotinoOptions> options, 
                         IServiceProvider serviceProvider,
                         LocalStorageContext dbContext)
    {
        _options = options;
        _serviceProvider = serviceProvider;
        _dbContext = dbContext;
    }

    public Task RunAsync()
    {
        return CreateWindowAsync(_options.Value.Title, 
                               _options.Value.StartUrl,
                               _options.Value.Width,
                               _options.Value.Height);
    }

    public async Task<PhotinoWindow> CreateWindowAsync(string title, string url, int width, int height)
    {
        if (_windows.Count >= _options.Value.MaxWindows)
            throw new InvalidOperationException("Maximum window count reached");

        var window = new PhotinoWindow()
            .SetTitle(title)
            .SetSize(width, height)
            .Center()
            .Load(url);

        var windowId = Guid.NewGuid().ToString();
        _windows.TryAdd(windowId, window);

        if (_options.Value.UseDeveloperTools)
            window.WaitForClose();

        return window;
    }

    public Task CloseWindowAsync(string windowId)
    {
        if (_windows.TryRemove(windowId, out var window))
        {
            window.Close();
        }
        return Task.CompletedTask;
    }

    public async Task SaveDataAsync<T>(string key, T value)
    {
        await _dbContext.SaveDataAsync(key, value);
    }

    public async Task<T?> LoadDataAsync<T>(string key)
    {
        return await _dbContext.LoadDataAsync<T>(key);
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPhotino(this IServiceCollection services, Action<PhotinoOptions> configure)
    {
        // 添加本地化服务
        services.AddLocalization();
        
        // 添加插件目录
        var pluginCatalog = new DirectoryCatalog("./Plugins");
        services.AddSingleton(pluginCatalog);
        
        // 添加IPC服务
        services.AddSingleton<NamedPipeServerStream>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<PhotinoOptions>>();
            return new NamedPipeServerStream(options.Value.PipeName);
        });
        
        // 添加共享内存服务
        services.AddSingleton<MemoryMappedFile>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<PhotinoOptions>>();
            return MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), options.Value.SharedMemorySize);
        });
        
        // 添加自动更新服务
        services.AddSingleton<UpdateManager>();
        
        // 添加性能监控服务
        services.AddSingleton<PerformanceCounter>();
        
        // 配置选项
        services.Configure(configure);
        
        // 添加数据库上下文
        services.AddDbContext<LocalStorageContext>((provider, options) => 
        {
            var config = provider.GetRequiredService<IOptions<PhotinoOptions>>();
            options.UseSqlite($"Data Source={config.Value.LocalStoragePath}");
        });
        
        // 添加视图模型
        services.AddSingleton<MainViewModel>();
        
        // 添加窗口服务
        services.AddSingleton<PhotinoWindow>();
        
        // 添加插件系统
        services.AddSingleton<CompositionContainer>();
        
        // 添加多语言支持
        services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        
        // 添加性能监控服务
        services.AddSingleton<PerformanceCounter>();
        
        // 添加自动更新服务
        services.AddSingleton<UpdateManager>();
        
        // 添加崩溃报告服务
        services.AddSingleton<CrashReporter>();
        
        // 添加快捷键绑定服务
        services.AddSingleton<ShortcutManager>();
        
        // 添加主题管理服务
        services.AddSingleton<ThemeManager>();
        
        // 添加插件管理服务
        services.AddSingleton<PluginManager>();
        
        // 添加IPC通信服务
        services.AddSingleton<PhotinoService>();
}
    public static IServiceCollection AddPhotino(this IServiceCollection services, Action<PhotinoOptions> configure)
    {
        services.Configure(configure);
        services.AddDbContext<LocalStorageContext>((provider, options) => 
        {
            var config = provider.GetRequiredService<IOptions<PhotinoOptions>>();
            options.UseSqlite($"");
        }
    }
 }

public class MainViewModel : ObservableObject
{
    private string _title = "Photino MVVM";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}

public class LocalStorageContext : DbContext
{
    public DbSet<LocalStorageItem> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocalStorageItem>().HasKey(x => x.Key);
    }

    public async Task SaveDataAsync<T>(string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        var item = new LocalStorageItem { Key = key, Value = json };
        
        var existing = await Items.FirstOrDefaultAsync(x => x.Key == key);
        if (existing != null)
        {
            existing.Value = json;
            Items.Update(existing);
        }
        else
        {
            await Items.AddAsync(item);
        }
        
        await SaveChangesAsync();
    }

    public async Task<T?> LoadDataAsync<T>(string key)
    {
        var item = await Items.FirstOrDefaultAsync(x => x.Key == key);
        if (item == null) return default;
        
        return JsonSerializer.Deserialize<T>(item.Value);
    }
}

public class LocalStorageItem
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddPhotino(options =>
        {
            options.Title = "My Photino App";
            options.Width = 1024;
            options.Height = 768;
            options.LocalStoragePath = "appdata.db";
            options.MaxWindows = 3;
        });
        
        services.AddSingleton<MainViewModel>();
        
        using var serviceProvider = services.BuildServiceProvider();
        
        // 初始化数据库
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<LocalStorageContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
        
        var photinoService = serviceProvider.GetRequiredService<IPhotinoService>();
        await photinoService.RunAsync();
    }
}