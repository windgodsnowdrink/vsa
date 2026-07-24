#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Reflection.MetadataLoadContext@10.0.0
#:package System.Composition@10.0.0
#:package System.IO.Pipelines@10.0.0
#:package System.Threading.Tasks.Dataflow@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.MetadataLoadContext;
using System.Threading;
using System.Threading.Tasks;
using System.Composition;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.Threading.Tasks.Dataflow;

namespace Plugin
{
    /// <summary>
    /// 插件加载选项
    /// </summary>
    public class PluginLoadOptions
    {
        /// <summary>
        /// 插件目录
        /// </summary>
        public string PluginDirectory { get; set; } = "plugins";
        
        /// <summary>
        /// 是否启用热重载
        /// </summary>
        public bool EnableHotReload { get; set; } = true;
        
        /// <summary>
        /// 是否启用沙箱
        /// </summary>
        public bool EnableSandbox { get; set; } = true;
        
        /// <summary>
        /// 最大并发插件数
        /// </summary>
        public int MaxConcurrentPlugins { get; set; } = 10;
        
        /// <summary>
        /// 是否启用性能指标
        /// </summary>
        public bool EnablePerformanceMetrics { get; set; } = true;
        
        /// <summary>
        /// 是否启用零拷贝
        /// </summary>
        public bool EnableZeroCopy { get; set; } = true;
        
        /// <summary>
        /// 线程本地缓存大小
        /// </summary>
        public int ThreadLocalCacheSize { get; set; } = 1024;
    }

    /// <summary>
    /// 插件加载上下文
    /// </summary>
    public class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;
        private readonly ILogger<PluginLoadContext> _logger;
        private readonly bool _enablePerformanceMetrics;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PluginLoadContext(string pluginPath, ILogger<PluginLoadContext> logger = null, bool enablePerformanceMetrics = true)
            : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
            _logger = logger;
            _enablePerformanceMetrics = enablePerformanceMetrics;
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            try
            {
                // 先尝试从当前上下文加载
                var currentAssembly = Default.LoadFromAssemblyName(assemblyName);
                if (currentAssembly != null)
                {
                    return currentAssembly;
                }

                // 通过解析器加载
                string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
                if (assemblyPath != null)
                {
                    _logger?.LogDebug("加载程序集: {AssemblyName} 从路径: {AssemblyPath}", assemblyName.FullName, assemblyPath);
                    return LoadFromAssemblyPath(assemblyPath);
                }

                _logger?.LogWarning("无法解析程序集: {AssemblyName}", assemblyName.FullName);
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "加载程序集时出错: {AssemblyName}", assemblyName.FullName);
                return null;
            }
        }

        /// <summary>
        /// 加载非托管DLL
        /// </summary>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            try
            {
                string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                if (libraryPath != null)
                {
                    _logger?.LogDebug("加载非托管DLL: {UnmanagedDllName} 从路径: {LibraryPath}", unmanagedDllName, libraryPath);
                    return LoadUnmanagedDllFromPath(libraryPath);
                }

                _logger?.LogWarning("无法解析非托管DLL: {UnmanagedDllName}", unmanagedDllName);
                return IntPtr.Zero;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "加载非托管DLL时出错: {UnmanagedDllName}", unmanagedDllName);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// 卸载上下文
        /// </summary>
        public new void Unload()
        {
            try
            {
                _logger?.LogDebug("卸载插件加载上下文");
                base.Unload();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "卸载插件加载上下文时出错");
            }
        }
    }

    /// <summary>
    /// 插件加载器接口
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// 加载插件
        /// </summary>
        Task<Assembly> LoadPluginAsync(string pluginPath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 卸载插件
        /// </summary>
        Task UnloadPluginAsync(string pluginPath, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 获取已加载的插件
        /// </summary>
        IEnumerable<(string Path, Assembly Assembly)> GetLoadedPlugins();
    }

    /// <summary>
    /// 隔离插件加载器
    /// </summary>
    public class IsolatedPluginLoader : IPluginLoader
    {
        private readonly Dictionary<string, (PluginLoadContext Context, Assembly Assembly)> _loadedAssemblies = new();
        private readonly ILogger<IsolatedPluginLoader> _logger;
        private readonly SemaphoreSlim _loadSemaphore;
        private readonly bool _enablePerformanceMetrics;
        private readonly ThreadLocal<Dictionary<string, object>> _threadLocalCache;

        /// <summary>
        /// 构造函数
        /// </summary>
        public IsolatedPluginLoader(ILogger<IsolatedPluginLoader> logger = null, int maxConcurrentLoads = 5, bool enablePerformanceMetrics = true, int threadLocalCacheSize = 1024)
        {
            _logger = logger;
            _loadSemaphore = new SemaphoreSlim(maxConcurrentLoads, maxConcurrentLoads);
            _enablePerformanceMetrics = enablePerformanceMetrics;
            _threadLocalCache = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>(threadLocalCacheSize));
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        public async Task<Assembly> LoadPluginAsync(string pluginPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            if (!File.Exists(pluginPath))
            {
                throw new FileNotFoundException("插件文件不存在", pluginPath);
            }

            // 检查是否已加载
            if (_loadedAssemblies.TryGetValue(pluginPath, out var existingEntry))
            {
                _logger?.LogDebug("插件已加载: {PluginPath}", pluginPath);
                return existingEntry.Assembly;
            }

            await _loadSemaphore.WaitAsync(cancellationToken);
            try
            {
                // 再次检查，防止并发加载
                if (_loadedAssemblies.TryGetValue(pluginPath, out existingEntry))
                {
                    _logger?.LogDebug("插件已加载: {PluginPath}", pluginPath);
                    return existingEntry.Assembly;
                }

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // 创建加载上下文
                var context = new PluginLoadContext(pluginPath, _logger, _enablePerformanceMetrics);
                
                // 加载程序集
                var assembly = context.LoadFromAssemblyPath(pluginPath);
                
                // 缓存加载结果
                _loadedAssemblies[pluginPath] = (context, assembly);
                
                stopwatch.Stop();
                _logger?.LogInformation("加载插件成功: {PluginPath}, 耗时: {ElapsedMilliseconds}ms", pluginPath, stopwatch.ElapsedMilliseconds);
                
                return assembly;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "加载插件时出错: {PluginPath}", pluginPath);
                throw;
            }
            finally
            {
                _loadSemaphore.Release();
            }
        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        public async Task UnloadPluginAsync(string pluginPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            await Task.Run(() =>
            {
                if (_loadedAssemblies.TryGetValue(pluginPath, out var entry))
                {
                    try
                    {
                        // 卸载上下文
                        entry.Context.Unload();
                        
                        // 从缓存中移除
                        _loadedAssemblies.Remove(pluginPath);
                        
                        // 清理线程本地缓存
                        _threadLocalCache.Value?.Clear();
                        
                        _logger?.LogInformation("卸载插件成功: {PluginPath}", pluginPath);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "卸载插件时出错: {PluginPath}", pluginPath);
                        throw;
                    }
                }
                else
                {
                    _logger?.LogWarning("插件未加载: {PluginPath}", pluginPath);
                }
            }, cancellationToken);
        }

        /// <summary>
        /// 获取已加载的插件
        /// </summary>
        public IEnumerable<(string Path, Assembly Assembly)>
        GetLoadedPlugins()
        {
            return _loadedAssemblies.Select(kv => (kv.Key, kv.Value.Assembly));
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            foreach (var entry in _loadedAssemblies.Values)
            {
                try
                {
                    entry.Context.Unload();
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "卸载插件时出错");
                }
            }
            
            _loadedAssemblies.Clear();
            _threadLocalCache.Dispose();
            _loadSemaphore.Dispose();
        }
    }

    /// <summary>
    /// 插件加载器工厂
    /// </summary>
    public static class PluginLoaderFactory
    {
        /// <summary>
        /// 创建插件加载器
        /// </summary>
        public static IPluginLoader CreateLoader(ILoggerFactory loggerFactory = null, int maxConcurrentLoads = 5, bool enablePerformanceMetrics = true, int threadLocalCacheSize = 1024)
        {
            var logger = loggerFactory?.CreateLogger<IsolatedPluginLoader>();
            return new IsolatedPluginLoader(logger, maxConcurrentLoads, enablePerformanceMetrics, threadLocalCacheSize);
        }
    }

    /// <summary>
    /// 插件服务扩展
    /// </summary>
    public static class PluginServiceCollectionExtensions
    {
        /// <summary>
        /// 添加插件服务
        /// </summary>
        public static IServiceCollection AddPluginServices(this IServiceCollection services, Action<PluginLoadOptions> configureOptions = null)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<PluginLoadOptions>(options => { });
            }

            services.AddSingleton<IPluginLoader, IsolatedPluginLoader>();

            return services;
        }
    }

    /// <summary>
    /// 程序类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Plugin Load Context Example");
            Console.WriteLine("=" * 50);

            // 构建服务容器
            var services = new ServiceCollection();

            // 配置日志
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            // 注册插件服务
            services.AddPluginServices(options =>
            {
                options.PluginDirectory = "plugins";
                options.EnableHotReload = true;
                options.EnableSandbox = true;
                options.MaxConcurrentPlugins = 10;
                options.EnablePerformanceMetrics = true;
                options.EnableZeroCopy = true;
                options.ThreadLocalCacheSize = 1024;
            });

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 获取插件加载器
            var pluginLoader = serviceProvider.GetRequiredService<IPluginLoader>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // 模拟插件路径
                string pluginPath = "plugins/MyPlugin.dll";

                // 加载插件
                Console.WriteLine($"加载插件: {pluginPath}");
                var assembly = await pluginLoader.LoadPluginAsync(pluginPath);
                logger.LogInformation("插件加载成功: {PluginName}", assembly.GetName().Name);

                // 获取已加载的插件
                Console.WriteLine("已加载的插件:");
                foreach (var (path, loadedAssembly) in pluginLoader.GetLoadedPlugins())
                {
                    Console.WriteLine($"- {Path.GetFileName(path)}: {loadedAssembly.GetName().Version}");
                }

                // 卸载插件
                Console.WriteLine($"卸载插件: {pluginPath}");
                await pluginLoader.UnloadPluginAsync(pluginPath);
                logger.LogInformation("插件卸载成功");

                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "插件加载示例错误");
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
