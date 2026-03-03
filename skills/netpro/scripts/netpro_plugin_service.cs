#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.FileProviders@10.0.0
#:package System.Threading.Channels@10.0.0
#:package Microsoft.Extensions.ObjectPool@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.FileProviders;

// 1. 核心接口定义
public interface INetProService
{
    Task<string> DoSomethingAsync();
    Task<int> ProcessDataAsync(ReadOnlySpan<byte> data);
    Task<string> ProcessItemAsync(string item);
}

public interface INetProPluginManager
{
    Task LoadPluginsAsync();
    List<INetProPlugin> GetLoadedPlugins();
}

public interface INetProExtensionManager
{
    Task<object> ExecuteExtensionAsync(string extensionName, ExtensionContext context);
}

public interface INetProPlugin
{
    string Name { get; }
    string Version { get; }
    string Description { get; }
    Task InitializeAsync(IServiceProvider serviceProvider);
    Task<string> ExecuteAsync(string input);
    Task DestroyAsync();
}

public interface INetProExtension
{
    string Name { get; }
    string Description { get; }
    Task InitializeAsync(IServiceProvider serviceProvider);
    Task<object> ExecuteAsync(ExtensionContext context);
    Task DestroyAsync();
}

// 2. 数据传输对象
public class NetProOptions
{
    public bool EnablePlugins { get; set; } = true;
    public string PluginsPath { get; set; } = "plugins";
    public bool EnableHotReload { get; set; } = true;
    public bool EnableCaching { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDetailedLogging { get; set; } = false;
}

public class ExtensionContext
{
    public object Data { get; set; }
}

// 3. 实现类
public class NetProService : INetProService
{
    private readonly ILogger<NetProService> _logger;
    private readonly IOptions<NetProOptions> _options;

    public NetProService(ILogger<NetProService> logger, IOptions<NetProOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<string> DoSomethingAsync()
    {
        _logger.LogInformation("执行核心功能");
        // 模拟核心功能执行
        await Task.Delay(100);
        return "核心功能执行成功";
    }

    public async Task<int> ProcessDataAsync(ReadOnlySpan<byte> data)
    {
        _logger.LogInformation($"处理数据，长度: {data.Length}");
        // 模拟数据处理
        await Task.Delay(50);
        return data.Length;
    }

    public async Task<string> ProcessItemAsync(string item)
    {
        _logger.LogInformation($"处理项目: {item}");
        // 模拟项目处理
        await Task.Delay(20);
        return $"处理结果: {item}";
    }
}

public class NetProPluginManager : INetProPluginManager
{
    private readonly ILogger<NetProPluginManager> _logger;
    private readonly IOptions<NetProOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<INetProPlugin> _plugins = new();
    private readonly Channel<FileChangeEvent> _fileChangeChannel;
    private readonly FileSystemWatcher _watcher;

    public NetProPluginManager(ILogger<NetProPluginManager> logger, IOptions<NetProOptions> options, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _options = options;
        _serviceProvider = serviceProvider;
        _fileChangeChannel = Channel.CreateUnbounded<FileChangeEvent>();

        // 初始化文件系统监视器
        if (_options.Value.EnableHotReload)
        {
            _watcher = new FileSystemWatcher(_options.Value.PluginsPath)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileCreated;
            _watcher.Deleted += OnFileDeleted;
            _watcher.Renamed += OnFileRenamed;

            // 启动文件变化处理任务
            _ = Task.Run(ProcessFileChangesAsync);
        }
    }

    public async Task LoadPluginsAsync()
    {
        _logger.LogInformation("开始加载插件");

        try
        {
            var pluginsPath = _options.Value.PluginsPath;
            
            // 检查插件目录是否存在
            if (!Directory.Exists(pluginsPath))
            {
                _logger.LogWarning($"插件目录不存在: {pluginsPath}");
                return;
            }

            // 加载目录中的插件
            var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories);
            _logger.LogInformation($"找到 {pluginFiles.Length} 个插件文件");

            foreach (var pluginFile in pluginFiles)
            {
                try
                {
                    await LoadPluginFromFileAsync(pluginFile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"加载插件文件失败: {pluginFile}");
                }
            }

            // 加载通过依赖注入注册的插件
            var registeredPlugins = _serviceProvider.GetServices<INetProPlugin>();
            foreach (var plugin in registeredPlugins)
            {
                try
                {
                    await plugin.InitializeAsync(_serviceProvider);
                    _plugins.Add(plugin);
                    _logger.LogInformation($"加载注册的插件: {plugin.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"初始化注册的插件失败: {plugin.Name}");
                }
            }

            _logger.LogInformation($"插件加载完成，共加载 {_plugins.Count} 个插件");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载插件时发生错误");
        }
    }

    public List<INetProPlugin> GetLoadedPlugins()
    {
        return _plugins;
    }

    private async Task LoadPluginFromFileAsync(string pluginFile)
    {
        try
        {
            var assembly = Assembly.LoadFrom(pluginFile);
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(INetProPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    var plugin = (INetProPlugin)Activator.CreateInstance(pluginType);
                    await plugin.InitializeAsync(_serviceProvider);
                    _plugins.Add(plugin);
                    _logger.LogInformation($"加载插件: {plugin.Name} 版本: {plugin.Version}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"创建插件实例失败: {pluginType.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"加载插件程序集失败: {pluginFile}");
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _fileChangeChannel.Writer.TryWrite(new FileChangeEvent { Type = FileChangeType.Changed, Path = e.FullPath });
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _fileChangeChannel.Writer.TryWrite(new FileChangeEvent { Type = FileChangeType.Created, Path = e.FullPath });
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        _fileChangeChannel.Writer.TryWrite(new FileChangeEvent { Type = FileChangeType.Deleted, Path = e.FullPath });
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        _fileChangeChannel.Writer.TryWrite(new FileChangeEvent { Type = FileChangeType.Renamed, Path = e.FullPath, OldPath = e.OldFullPath });
    }

    private async Task ProcessFileChangesAsync()
    {
        await foreach (var changeEvent in _fileChangeChannel.Reader.ReadAllAsync())
        {
            try
            {
                _logger.LogInformation($"检测到文件变化: {changeEvent.Type} - {changeEvent.Path}");
                
                // 重新加载插件
                await ReloadPluginsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理文件变化时发生错误");
            }
        }
    }

    private async Task ReloadPluginsAsync()
    {
        _logger.LogInformation("开始重新加载插件");

        // 销毁现有插件
        foreach (var plugin in _plugins)
        {
            try
            {
                await plugin.DestroyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"销毁插件失败: {plugin.Name}");
            }
        }

        // 清空插件列表
        _plugins.Clear();

        // 重新加载插件
        await LoadPluginsAsync();
    }

    private enum FileChangeType
    {
        Created,
        Changed,
        Deleted,
        Renamed
    }

    private class FileChangeEvent
    {
        public FileChangeType Type { get; set; }
        public string Path { get; set; }
        public string OldPath { get; set; }
    }
}

public class NetProExtensionManager : INetProExtensionManager
{
    private readonly ILogger<NetProExtensionManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<INetProExtension> _extensions = new();

    public NetProExtensionManager(ILogger<NetProExtensionManager> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        // 加载通过依赖注入注册的扩展
        LoadRegisteredExtensions();
    }

    public async Task<object> ExecuteExtensionAsync(string extensionName, ExtensionContext context)
    {
        _logger.LogInformation($"执行扩展: {extensionName}");

        var extension = _extensions.FirstOrDefault(e => e.Name == extensionName);
        if (extension == null)
        {
            throw new InvalidOperationException($"扩展不存在: {extensionName}");
        }

        try
        {
            return await extension.ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"执行扩展失败: {extensionName}");
            throw;
        }
    }

    private void LoadRegisteredExtensions()
    {
        try
        {
            var registeredExtensions = _serviceProvider.GetServices<INetProExtension>();
            foreach (var extension in registeredExtensions)
            {
                try
                {
                    extension.InitializeAsync(_serviceProvider).Wait();
                    _extensions.Add(extension);
                    _logger.LogInformation($"加载注册的扩展: {extension.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"初始化注册的扩展失败: {extension.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载注册的扩展时发生错误");
        }
    }
}

// 4. 依赖注入扩展
public static class NetProServiceCollectionExtensions
{
    public static IServiceCollection AddNetPro(this IServiceCollection services, Action<NetProOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        // 注册服务
        services.AddSingleton<INetProService, NetProService>();
        services.AddSingleton<INetProPluginManager, NetProPluginManager>();
        services.AddSingleton<INetProExtensionManager, NetProExtensionManager>();

        return services;
    }
}

// 5. 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 配置日志
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // 配置 NetPro
        builder.Services.AddNetPro(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = true;
            options.EnableCaching = true;
        });

        var app = builder.Build();

        // 健康检查端点
        app.MapGet("/health", () => "NetPro Plugin Service is healthy");

        // 插件信息端点
        app.MapGet("/plugins", async (INetProPluginManager pluginManager) => {
            var plugins = pluginManager.GetLoadedPlugins();
            return Results.Ok(plugins.Select(p => new {
                Name = p.Name,
                Version = p.Version,
                Description = p.Description
            }));
        });

        // 执行插件端点
        app.MapPost("/plugins/{pluginName}/execute", async (string pluginName, string input, INetProPluginManager pluginManager) => {
            var plugin = pluginManager.GetLoadedPlugins().FirstOrDefault(p => p.Name == pluginName);
            if (plugin == null)
            {
                return Results.NotFound($"插件不存在: {pluginName}");
            }

            try
            {
                var result = await plugin.ExecuteAsync(input);
                return Results.Ok(new { Result = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // 执行扩展端点
        app.MapPost("/extensions/{extensionName}/execute", async (string extensionName, ExtensionContext context, INetProExtensionManager extensionManager) => {
            try
            {
                var result = await extensionManager.ExecuteExtensionAsync(extensionName, context);
                return Results.Ok(new { Result = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Error = ex.Message });
            }
        });

        // 启动服务
        await app.RunAsync();
    }
}

// 6. 示例插件和扩展
public class SamplePlugin : INetProPlugin
{
    private ILogger<SamplePlugin> _logger;

    public string Name => "SamplePlugin";
    public string Version => "1.0.0";
    public string Description => "示例插件";

    public Task InitializeAsync(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<SamplePlugin>>();
        _logger.LogInformation($"插件 {Name} 初始化");
        return Task.CompletedTask;
    }

    public Task<string> ExecuteAsync(string input)
    {
        _logger.LogInformation($"插件 {Name} 执行，输入: {input}");
        return Task.FromResult($"{Name} 处理结果: {input}");
    }

    public Task DestroyAsync()
    {
        _logger.LogInformation($"插件 {Name} 销毁");
        return Task.CompletedTask;
    }
}

public class SampleExtension : INetProExtension
{
    private ILogger<SampleExtension> _logger;

    public string Name => "SampleExtension";
    public string Description => "示例扩展";

    public Task InitializeAsync(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<SampleExtension>>();
        _logger.LogInformation($"扩展 {Name} 初始化");
        return Task.CompletedTask;
    }

    public Task<object> ExecuteAsync(ExtensionContext context)
    {
        _logger.LogInformation($"扩展 {Name} 执行，输入: {context.Data}");
        return Task.FromResult<object>($"{Name} 处理结果: {context.Data}");
    }

    public Task DestroyAsync()
    {
        _logger.LogInformation($"扩展 {Name} 销毁");
        return Task.CompletedTask;
    }
}
