# NetPro - 使用示例

## 快速入门

### 1. 基本用法示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var netProService = serviceProvider.GetRequiredService<INetProService>();
        var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
        var extensionManager = serviceProvider.GetRequiredService<INetProExtensionManager>();
        
        Console.WriteLine("NetPro 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 加载插件
        await pluginManager.LoadPluginsAsync();
        Console.WriteLine($"已加载 {pluginManager.GetLoadedPlugins().Count} 个插件");
        
        // 使用插件功能
        foreach (var plugin in pluginManager.GetLoadedPlugins())
        {
            Console.WriteLine($"插件: {plugin.Name}, 版本: {plugin.Version}");
            var result = await plugin.ExecuteAsync("Hello from NetPro!");
            Console.WriteLine($"插件执行结果: {result}");
        }
        
        // 使用扩展功能
        var extensionResult = await extensionManager.ExecuteExtensionAsync("SampleExtension", new ExtensionContext { Data = "Test data" });
        Console.WriteLine($"扩展执行结果: {extensionResult}");
        
        // 使用核心功能
        var coreResult = await netProService.DoSomethingAsync();
        Console.WriteLine($"核心功能结果: {coreResult}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = true;
            options.EnableCaching = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INetProService, NetProService>();
        builder.AddSingleton<INetProPluginManager, NetProPluginManager>();
        builder.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 2. 高级配置示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 NetPro 设置
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = true;
            options.EnableCaching = true;
            options.CacheSize = 2000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        builder.AddSingleton<INetProService, NetProService>();
        builder.AddSingleton<INetProPluginManager, NetProPluginManager>();
        builder.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<NetProOptions>>().Value;
        Console.WriteLine($"配置: 插件={settings.EnablePlugins}, 路径={settings.PluginsPath}");
        Console.WriteLine($"热重载={settings.EnableHotReload}, 缓存={settings.EnableCaching}");
        Console.WriteLine($"缓存大小={settings.CacheSize}, 超时={settings.Timeout}");
        
        // 使用服务
        var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
        await pluginManager.LoadPluginsAsync();
        
        var netProService = serviceProvider.GetRequiredService<INetProService>();
        var result = await netProService.DoSomethingAsync();
        Console.WriteLine($"核心功能结果: {result}");
    }
}
`

### 3. 性能优化示例

`csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var netProService = serviceProvider.GetRequiredService<INetProService>();
        var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
        
        // 性能测试
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        // 预热
        await netProService.DoSomethingAsync();
        await pluginManager.LoadPluginsAsync();
        
        // 测试核心功能
        stopwatch.Restart();
        for (int i = 0; i < iterations; i++)
        {
            await netProService.DoSomethingAsync();
        }
        stopwatch.Stop();
        Console.WriteLine($"核心功能执行时间 ({iterations} 次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 测试插件功能
        if (pluginManager.GetLoadedPlugins().Count > 0)
        {
            var plugin = pluginManager.GetLoadedPlugins()[0];
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                await plugin.ExecuteAsync($"Test {i}");
            }
            stopwatch.Stop();
            Console.WriteLine($"插件功能执行时间 ({iterations} 次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        }
        
        // 内存使用情况
        var memoryInfo = GC.GetTotalMemory(true);
        Console.WriteLine($"内存使用: {memoryInfo / 1024 / 1024:F2} MB");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项（启用缓存）
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = false; // 禁用热重载以提高性能
            options.EnableCaching = true;
            options.CacheSize = 5000;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Error));
        builder.AddSingleton<INetProService, NetProService>();
        builder.AddSingleton<INetProPluginManager, NetProPluginManager>();
        builder.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 4. 错误处理示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var netProService = serviceProvider.GetRequiredService<INetProService>();
        var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
        var extensionManager = serviceProvider.GetRequiredService<INetProExtensionManager>();
        
        try
        {
            // 尝试加载插件
            await pluginManager.LoadPluginsAsync();
            Console.WriteLine($"成功加载 {pluginManager.GetLoadedPlugins().Count} 个插件");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载插件时出错: {ex.Message}");
        }
        
        try
        {
            // 尝试使用核心功能
            var result = await netProService.DoSomethingAsync();
            Console.WriteLine($"核心功能执行成功: {result}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 尝试使用不存在的扩展
            var result = await extensionManager.ExecuteExtensionAsync("NonExistentExtension", new ExtensionContext { Data = "Test" });
            Console.WriteLine($"扩展执行结果: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"执行扩展时出错: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = true;
            options.EnableCaching = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INetProService, NetProService>();
        builder.AddSingleton<INetProPluginManager, NetProPluginManager>();
        builder.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 5. 自定义插件示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 自定义插件实现
public class SamplePlugin : INetProPlugin
{
    private readonly ILogger<SamplePlugin> _logger;
    
    public SamplePlugin(ILogger<SamplePlugin> logger)
    {
        _logger = logger;
    }
    
    public string Name => "SamplePlugin";
    public string Version => "1.0.0";
    public string Description => "示例插件";
    
    public Task InitializeAsync(IServiceProvider serviceProvider)
    {
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

// 自定义扩展实现
public class SampleExtension : INetProExtension
{
    private readonly ILogger<SampleExtension> _logger;
    
    public SampleExtension(ILogger<SampleExtension> logger)
    {
        _logger = logger;
    }
    
    public string Name => "SampleExtension";
    public string Description => "示例扩展";
    
    public Task InitializeAsync(IServiceProvider serviceProvider)
    {
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

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 自定义插件示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
        var extensionManager = serviceProvider.GetRequiredService<INetProExtensionManager>();
        
        // 加载插件
        await pluginManager.LoadPluginsAsync();
        Console.WriteLine($"已加载 {pluginManager.GetLoadedPlugins().Count} 个插件");
        
        // 使用插件
        foreach (var plugin in pluginManager.GetLoadedPlugins())
        {
            var result = await plugin.ExecuteAsync("Hello from custom plugin!");
            Console.WriteLine($"插件 {plugin.Name} 执行结果: {result}");
        }
        
        // 使用扩展
        var extensionResult = await extensionManager.ExecuteExtensionAsync("SampleExtension", new ExtensionContext { Data = "Hello from custom extension!" });
        Console.WriteLine($"扩展执行结果: {extensionResult}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = true;
            options.EnableCaching = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INetProService, NetProService>();
        builder.AddSingleton<INetProPluginManager, NetProPluginManager>();
        builder.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
        
        // 注册自定义插件和扩展
        builder.AddSingleton<INetProPlugin, SamplePlugin>();
        builder.AddSingleton<INetProExtension, SampleExtension>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 6. 热重载示例

`csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 热重载示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var pluginManager = serviceProvider.GetRequiredService<INetProPluginManager>();
        
        // 初始加载插件
        await pluginManager.LoadPluginsAsync();
        Console.WriteLine($"初始加载 {pluginManager.GetLoadedPlugins().Count} 个插件");
        
        // 监控插件变化
        Console.WriteLine("开始监控插件变化...");
        Console.WriteLine("提示: 修改插件文件后，插件将自动重新加载");
        Console.WriteLine("按 Ctrl+C 退出");
        
        try
        {
            while (true)
            {
                // 检查插件是否有变化
                var pluginCount = pluginManager.GetLoadedPlugins().Count;
                Console.WriteLine($"当前加载 {pluginCount} 个插件");
                
                // 使用插件
                foreach (var plugin in pluginManager.GetLoadedPlugins())
                {
                    var result = await plugin.ExecuteAsync($"测试时间: {DateTime.Now}");
                    Console.WriteLine($"插件 {plugin.Name} 执行结果: {result}");
                }
                
                // 等待一段时间
                await Task.Delay(5000);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("退出监控");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项（启用热重载）
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = true;
            options.PluginsPath = "plugins";
            options.EnableHotReload = true;
            options.EnableCaching = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INetProService, NetProService>();
        builder.AddSingleton<INetProPluginManager, NetProPluginManager>();
        builder.AddSingleton<INetProExtensionManager, NetProExtensionManager>();
        
        return builder.BuildServiceProvider();
    }
}
`

## 性能优化最佳实践

### 1. 内存优化示例

`csharp
using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 内存优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var netProService = serviceProvider.GetRequiredService<INetProService>();
        
        // 使用内存池
        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(1024 * 1024); // 1MB 缓冲区
        
        try
        {
            // 使用 Span 优化
            var span = new Span<byte>(buffer, 0, 1024 * 1024);
            // 填充数据
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = (byte)(i % 256);
            }
            
            // 处理数据
            var result = await netProService.ProcessDataAsync(span);
            Console.WriteLine($"处理结果: {result}");
        }
        finally
        {
            // 归还缓冲区
            pool.Return(buffer);
        }
        
        // 检查内存使用
        var memoryInfo = GC.GetTotalMemory(true);
        Console.WriteLine($"内存使用: {memoryInfo / 1024 / 1024:F2} MB");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = false; // 禁用插件以减少内存使用
            options.EnableCaching = true;
        });
        
        // 注册服务
        builder.AddSingleton<INetProService, NetProService>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 2. 并发优化示例

`csharp
using System;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetPro 并发优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var netProService = serviceProvider.GetRequiredService<INetProService>();
        
        // 创建通道
        var channel = Channel.CreateUnbounded<string>();
        
        // 生产者
        var producerTask = Task.Run(async () => {
            for (int i = 0; i < 100; i++)
            {
                await channel.Writer.WriteAsync($"Task {i}");
                await Task.Delay(10);
            }
            channel.Writer.Complete();
        });
        
        // 消费者
        var consumerTask = Task.Run(async () => {
            var tasks = new System.Collections.Generic.List<Task>();
            
            await foreach (var item in channel.Reader.ReadAllAsync())
            {
                // 并行处理
                tasks.Add(Task.Run(async () => {
                    var result = await netProService.ProcessItemAsync(item);
                    Console.WriteLine($"处理结果: {result}");
                }));
                
                // 限制并发数
                if (tasks.Count >= 10)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            
            // 处理剩余任务
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
        });
        
        // 等待完成
        await Task.WhenAll(producerTask, consumerTask);
        Console.WriteLine("所有任务处理完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetPro 选项
        builder.Configure<NetProOptions>(options => {
            options.EnablePlugins = false;
            options.EnableCaching = true;
        });
        
        // 注册服务
        builder.AddSingleton<INetProService, NetProService>();
        
        return builder.BuildServiceProvider();
    }
}
`

## 总结

以上示例展示了 NetPro 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 创建自定义插件和扩展
6. 使用热重载功能
7. 实现内存和并发优化

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

## 实际应用场景

### 企业级应用

NetPro 技能可以用于构建企业级应用，通过插件系统实现功能模块化，便于团队协作和维护。

### 微服务架构

在微服务架构中，NetPro 技能可以用于实现服务的扩展点，便于在不修改核心代码的情况下添加新功能。

### 插件化应用

对于需要插件化的应用，如 CMS、ERP 等系统，NetPro 技能提供了完整的插件管理和扩展功能。

### 开发工具

开发工具可以使用 NetPro 技能实现插件系统，支持第三方插件和扩展，提高工具的灵活性和可扩展性。

### 云原生应用

在云原生应用中，NetPro 技能可以用于实现服务的动态扩展和配置管理，适应云环境的变化。
