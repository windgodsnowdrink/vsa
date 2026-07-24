#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Natasha@3.0.0
#:package System.Reflection.Emit@10.0.0
#:package System.Linq.Expressions@10.0.0
#:package System.Runtime.CompilerServices.Unsafe@10.0.0
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
using Natasha;
using Natasha.CSharp;
using Natasha.CSharp.Compiler;
using Natasha.CSharp.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Plugin.Natasha
{
    /// <summary>
    /// Natasha集成选项
    /// </summary>
    public class NatashaIntegrationOptions
    {
        /// <summary>
        /// 是否启用动态代码生成
        /// </summary>
        public bool EnableDynamicCodeGeneration { get; set; } = true;
        
        /// <summary>
        /// 是否启用代码缓存
        /// </summary>
        public bool EnableCodeCache { get; set; } = true;
        
        /// <summary>
        /// 代码缓存大小
        /// </summary>
        public int CodeCacheSize { get; set; } = 100;
        
        /// <summary>
        /// 是否启用AOT兼容模式
        /// </summary>
        public bool EnableAotCompatibleMode { get; set; } = true;
        
        /// <summary>
        /// 最大并发编译数
        /// </summary>
        public int MaxConcurrentCompilations { get; set; } = 5;
        
        /// <summary>
        /// 编译超时时间（毫秒）
        /// </summary>
        public int CompilationTimeoutMs { get; set; } = 10000;
        
        /// <summary>
        /// 是否启用性能指标
        /// </summary>
        public bool EnablePerformanceMetrics { get; set; } = true;
    }

    /// <summary>
    /// Natasha代码生成器
    /// </summary>
    public class NatashaCodeGenerator
    {
        private readonly ILogger<NatashaCodeGenerator> _logger;
        private readonly NatashaIntegrationOptions _options;
        private readonly Dictionary<string, (Type Type, long Timestamp)> _typeCache;
        private readonly SemaphoreSlim _compilationSemaphore;
        private readonly object _cacheLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        public NatashaCodeGenerator(ILogger<NatashaCodeGenerator> logger, IOptions<NatashaIntegrationOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _typeCache = _options.EnableCodeCache ? new Dictionary<string, (Type Type, long Timestamp)>(_options.CodeCacheSize) : null;
            _compilationSemaphore = new SemaphoreSlim(_options.MaxConcurrentCompilations, _options.MaxConcurrentCompilations);

            // 初始化Natasha
            InitializeNatasha();
        }

        /// <summary>
        /// 初始化Natasha
        /// </summary>
        private void InitializeNatasha()
        {
            try
            {
                // 配置Natasha编译器
                NatashaInitializer.Initialize();
                
                // 设置编译选项
                CompilerOptions.DefaultInstance
                    .SetLanguageVersion(LanguageVersions.CSharp13)
                    .SetOptimizationLevel(OptimizationLevels.Release)
                    .SetWarningLevel(4)
                    .SetAllowUnsafe(true)
                    .SetNullableContextOptions(NullableContextOptions.Enable);

                _logger?.LogInformation("Natasha初始化成功");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Natasha初始化失败");
                throw;
            }
        }

        /// <summary>
        /// 生成类型
        /// </summary>
        public async Task<Type> GenerateTypeAsync(string typeName, string code, CancellationToken cancellationToken = default)
        {
            if (!_options.EnableDynamicCodeGeneration)
            {
                throw new InvalidOperationException("动态代码生成已禁用");
            }

            // 检查缓存
            if (_options.EnableCodeCache)
            {
                lock (_cacheLock)
                {
                    if (_typeCache.TryGetValue(typeName, out var cachedType))
                    {
                        _logger?.LogDebug("从缓存中获取类型: {TypeName}", typeName);
                        return cachedType.Type;
                    }
                }
            }

            await _compilationSemaphore.WaitAsync(cancellationToken);
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                _logger?.LogDebug("开始生成类型: {TypeName}", typeName);

                // 创建编译单元
                var compilationUnit = new CompilationUnit()
                    .SetNameSpace("DynamicTypes")
                    .AddUsing("System")
                    .AddUsing("System.Collections.Generic")
                    .AddUsing("System.Linq")
                    .AddUsing("System.Threading.Tasks");

                // 添加代码
                compilationUnit.Add(code);

                // 编译
                var assembly = compilationUnit.GetAssembly();
                if (assembly == null)
                {
                    throw new InvalidOperationException($"类型编译失败: {typeName}");
                }

                // 获取类型
                var type = assembly.GetType($"DynamicTypes.{typeName}");
                if (type == null)
                {
                    throw new InvalidOperationException($"无法找到生成的类型: {typeName}");
                }

                stopwatch.Stop();
                _logger?.LogInformation("类型生成成功: {TypeName}, 耗时: {ElapsedMilliseconds}ms", typeName, stopwatch.ElapsedMilliseconds);

                // 缓存类型
                if (_options.EnableCodeCache)
                {
                    lock (_cacheLock)
                    {
                        // 检查缓存大小
                        if (_typeCache.Count >= _options.CodeCacheSize)
                        {
                            // 删除最旧的缓存项
                            var oldestKey = _typeCache.OrderBy(kv => kv.Value.Timestamp).First().Key;
                            _typeCache.Remove(oldestKey);
                            _logger?.LogDebug("缓存已满，删除最旧的类型: {OldestKey}", oldestKey);
                        }

                        _typeCache[typeName] = (type, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                        _logger?.LogDebug("类型已缓存: {TypeName}", typeName);
                    }
                }

                return type;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "类型生成失败: {TypeName}", typeName);
                throw;
            }
            finally
            {
                _compilationSemaphore.Release();
            }
        }

        /// <summary>
        /// 生成实例
        /// </summary>
        public async Task<object> GenerateInstanceAsync(string typeName, string code, params object[] parameters)
        {
            var type = await GenerateTypeAsync(typeName, code);
            return Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// 生成实例（泛型）
        /// </summary>
        public async Task<T> GenerateInstanceAsync<T>(string typeName, string code, params object[] parameters)
        {
            var instance = await GenerateInstanceAsync(typeName, code, parameters);
            return (T)instance;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            if (_options.EnableCodeCache)
            {
                lock (_cacheLock)
                {
                    _typeCache.Clear();
                    _logger?.LogInformation("类型缓存已清除");
                }
            }
        }

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        public int GetCacheSize()
        {
            if (_options.EnableCodeCache)
            {
                lock (_cacheLock)
                {
                    return _typeCache.Count;
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// 动态插件生成器
    /// </summary>
    public class DynamicPluginGenerator
    {
        private readonly NatashaCodeGenerator _codeGenerator;
        private readonly ILogger<DynamicPluginGenerator> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DynamicPluginGenerator(NatashaCodeGenerator codeGenerator, ILogger<DynamicPluginGenerator> logger)
        {
            _codeGenerator = codeGenerator;
            _logger = logger;
        }

        /// <summary>
        /// 生成插件
        /// </summary>
        public async Task<IPlugin> GeneratePluginAsync(string pluginName, string pluginCode, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("开始生成插件: {PluginName}", pluginName);

            // 生成插件类型
            var pluginType = await _codeGenerator.GenerateTypeAsync(pluginName, pluginCode, cancellationToken);

            // 检查是否实现了IPlugin接口
            if (!typeof(IPlugin).IsAssignableFrom(pluginType))
            {
                throw new InvalidOperationException($"插件类型必须实现IPlugin接口: {pluginName}");
            }

            // 创建插件实例
            var plugin = (IPlugin)Activator.CreateInstance(pluginType);
            _logger?.LogInformation("插件生成成功: {PluginName}", pluginName);

            return plugin;
        }

        /// <summary>
        /// 生成插件代码模板
        /// </summary>
        public string GeneratePluginCodeTemplate(string pluginName, string description)
        {
            return $@"
public class {pluginName} : IPlugin
{{
    public string Name => "{pluginName}";
    public string Description => "{description}";
    public Version Version => new Version("1.0.0");
    public string Author => "Dynamic Generator";
    
    public Task InitializeAsync()
    {{
        Console.WriteLine($"插件 {Name} 初始化");
        return Task.CompletedTask;
    }}
    
    public Task ExecuteAsync(object parameters = null)
    {{
        Console.WriteLine($"插件 {Name} 执行，参数: {{parameters}}");
        return Task.CompletedTask;
    }}
    
    public Task ShutdownAsync()
    {{
        Console.WriteLine($"插件 {Name} 关闭");
        return Task.CompletedTask;
    }}
    
    public T GetService<T>() where T : class
    {{
        return null;
    }}
    
    public void SetServiceProvider(IServiceProvider serviceProvider)
    {{
        // 保存服务提供程序
    }}
}}";
        }
    }

    /// <summary>
    /// 插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// 插件版本
        /// </summary>
        Version Version { get; }
        
        /// <summary>
        /// 插件作者
        /// </summary>
        string Author { get; }
        
        /// <summary>
        /// 初始化插件
        /// </summary>
        Task InitializeAsync();
        
        /// <summary>
        /// 执行插件
        /// </summary>
        Task ExecuteAsync(object parameters = null);
        
        /// <summary>
        /// 关闭插件
        /// </summary>
        Task ShutdownAsync();
        
        /// <summary>
        /// 获取服务
        /// </summary>
        T GetService<T>() where T : class;
        
        /// <summary>
        /// 设置服务提供程序
        /// </summary>
        void SetServiceProvider(IServiceProvider serviceProvider);
    }

    /// <summary>
    /// Natasha服务扩展
    /// </summary>
    public static class NatashaServiceCollectionExtensions
    {
        /// <summary>
        /// 添加Natasha服务
        /// </summary>
        public static IServiceCollection AddNatashaServices(this IServiceCollection services, Action<NatashaIntegrationOptions> configureOptions = null)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<NatashaIntegrationOptions>(options => { });
            }

            services.AddSingleton<NatashaCodeGenerator>();
            services.AddSingleton<DynamicPluginGenerator>();

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
            Console.WriteLine("Natasha Integration Example");
            Console.WriteLine("=" * 50);

            // 构建服务容器
            var services = new ServiceCollection();

            // 配置日志
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            // 注册Natasha服务
            services.AddNatashaServices(options =>
            {
                options.EnableDynamicCodeGeneration = true;
                options.EnableCodeCache = true;
                options.CodeCacheSize = 100;
                options.EnableAotCompatibleMode = true;
                options.MaxConcurrentCompilations = 5;
                options.CompilationTimeoutMs = 10000;
                options.EnablePerformanceMetrics = true;
            });

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 获取动态插件生成器
            var pluginGenerator = serviceProvider.GetRequiredService<DynamicPluginGenerator>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // 生成插件代码
                string pluginName = "HelloPlugin";
                string pluginDescription = "Hello World Plugin";
                string pluginCode = pluginGenerator.GeneratePluginCodeTemplate(pluginName, pluginDescription);

                // 生成并执行插件
                Console.WriteLine($"生成插件: {pluginName}");
                var plugin = await pluginGenerator.GeneratePluginAsync(pluginName, pluginCode);

                // 初始化插件
                Console.WriteLine("初始化插件...");
                await plugin.InitializeAsync();

                // 执行插件
                Console.WriteLine("执行插件...");
                await plugin.ExecuteAsync("Hello from Natasha!");

                // 关闭插件
                Console.WriteLine("关闭插件...");
                await plugin.ShutdownAsync();

                Console.WriteLine("插件执行成功！");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Natasha集成示例错误");
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
