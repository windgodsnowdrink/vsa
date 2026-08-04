#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package System.Threading.Channels@10.0.0
#:package System.Collections.Concurrent@10.0.0
#:package System.IO.Abstractions@10.0.0
#:package System.Diagnostics.DiagnosticSource@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;

namespace Plugin.Service
{
    /// <summary>
    /// 插件服务选项
    /// </summary>
    public class PluginServiceOptions
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
        /// 热重载间隔（毫秒）
        /// </summary>
        public int HotReloadIntervalMs { get; set; } = 2000;
        
        /// <summary>
        /// 是否启用插件依赖注入
        /// </summary>
        public bool EnableDependencyInjection { get; set; } = true;
        
        /// <summary>
        /// 是否启用插件生命周期管理
        /// </summary>
        public bool EnableLifecycleManagement { get; set; } = true;
        
        /// <summary>
        /// 插件初始化超时时间（毫秒）
        /// </summary>
        public int PluginInitializationTimeoutMs { get; set; } = 30000;
        
        /// <summary>
        /// 插件执行超时时间（毫秒）
        /// </summary>
        public int PluginExecutionTimeoutMs { get; set; } = 60000;
    }

    /// <summary>
    /// 插件信息
    /// </summary>
    public class PluginInfo
    {
        /// <summary>
        /// 插件路径
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 插件版本
        /// </summary>
        public Version Version { get; set; }
        
        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// 插件作者
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// 插件状态
        /// </summary>
        public PluginStatus Status { get; set; }
        
        /// <summary>
        /// 上次修改时间
        /// </summary>
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// 加载时间
        /// </summary>
        public DateTime LoadTime { get; set; }
        
        /// <summary>
        /// 执行次数
        /// </summary>
        public long ExecutionCount { get; set; }
        
        /// <summary>
        /// 平均执行时间（毫秒）
        /// </summary>
        public double AverageExecutionTimeMs { get; set; }
    }

    /// <summary>
    /// 插件状态
    /// </summary>
    public enum PluginStatus
    {
        /// <summary>
        /// 未加载
        /// </summary>
        Unloaded,
        /// <summary>
        /// 加载中
        /// </summary>
        Loading,
        /// <summary>
        /// 已加载
        /// </summary>
        Loaded,
        /// <summary>
        /// 初始化中
        /// </summary>
        Initializing,
        /// <summary>
        /// 已初始化
        /// </summary>
        Initialized,
        /// <summary>
        /// 执行中
        /// </summary>
        Executing,
        /// <summary>
        /// 出错
        /// </summary>
        Error,
        /// <summary>
        /// 已卸载
        /// </summary>
        Unloading
    }

    /// <summary>
    /// 插件事件参数
    /// </summary>
    public class PluginEventArgs : EventArgs
    {
        /// <summary>
        /// 插件信息
        /// </summary>
        public PluginInfo PluginInfo { get; }
        
        /// <summary>
        /// 事件时间
        /// </summary>
        public DateTime Timestamp { get; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public PluginEventArgs(PluginInfo pluginInfo)
        {
            PluginInfo = pluginInfo;
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// 插件服务
    /// </summary>
    public class PluginService : IHostedService, IDisposable
    {
        private readonly ILogger<PluginService> _logger;
        private readonly PluginServiceOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<string, (PluginLoadContext Context, IPlugin Plugin, PluginInfo Info)> _loadedPlugins;
        private readonly Channel<(string Path, object Parameters)> _executionQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _executionTask;
        private readonly Task _hotReloadTask;
        private readonly SemaphoreSlim _executionSemaphore;
        private readonly object _fileWatchLock = new object();
        private FileSystemWatcher _fileWatcher;
        private bool _disposed;

        // 性能指标
        private readonly Counter<long> _pluginLoadCounter;
        private readonly Counter<long> _pluginExecuteCounter;
        private readonly Histogram<double> _pluginExecutionTimeHistogram;
        private readonly Gauge<long> _loadedPluginGauge;

        /// <summary>
        /// 插件加载事件
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginLoaded;
        
        /// <summary>
        /// 插件卸载事件
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginUnloaded;
        
        /// <summary>
        /// 插件执行事件
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginExecuted;
        
        /// <summary>
        /// 插件错误事件
        /// </summary>
        public event EventHandler<PluginEventArgs> PluginError;

        /// <summary>
        /// 构造函数
        /// </summary>
        public PluginService(
            ILogger<PluginService> logger,
            IOptions<PluginServiceOptions> options,
            IFileSystem fileSystem,
            IServiceProvider serviceProvider,
            MeterFactory meterFactory = null)
        {
            _logger = logger;
            _options = options.Value;
            _fileSystem = fileSystem;
            _serviceProvider = serviceProvider;
            _loadedPlugins = new ConcurrentDictionary<string, (PluginLoadContext, IPlugin, PluginInfo)>();
            _executionQueue = Channel.CreateUnbounded<(string Path, object Parameters)>();
            _cancellationTokenSource = new CancellationTokenSource();
            _executionSemaphore = new SemaphoreSlim(_options.MaxConcurrentPlugins, _options.MaxConcurrentPlugins);

            // 初始化性能指标
            if (_options.EnablePerformanceMetrics && meterFactory != null)
            {
                var meter = meterFactory.Create("Plugin.Service");
                _pluginLoadCounter = meter.CreateCounter<long>("plugin.load.count", description: "插件加载次数");
                _pluginExecuteCounter = meter.CreateCounter<long>("plugin.execute.count", description: "插件执行次数");
                _pluginExecutionTimeHistogram = meter.CreateHistogram<double>("plugin.execute.time.ms", description: "插件执行时间（毫秒）");
                _loadedPluginGauge = meter.CreateGauge<long>("plugin.loaded.count", description: "已加载插件数量");
            }

            // 启动执行队列处理任务
            _executionTask = ProcessExecutionQueueAsync(_cancellationTokenSource.Token);

            // 启动热重载任务
            if (_options.EnableHotReload)
            {
                _hotReloadTask = MonitorPluginsAsync(_cancellationTokenSource.Token);
            }

            // 初始化插件目录
            InitializePluginDirectory();
        }

        /// <summary>
        /// 初始化插件目录
        /// </summary>
        private void InitializePluginDirectory()
        {
            try
            {
                if (!_fileSystem.Directory.Exists(_options.PluginDirectory))
                {
                    _fileSystem.Directory.CreateDirectory(_options.PluginDirectory);
                    _logger?.LogInformation("创建插件目录: {PluginDirectory}", _options.PluginDirectory);
                }

                // 配置文件监视器
                if (_options.EnableHotReload)
                {
                    ConfigureFileSystemWatcher();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "初始化插件目录时出错: {PluginDirectory}", _options.PluginDirectory);
                throw;
            }
        }

        /// <summary>
        /// 配置文件监视器
        /// </summary>
        private void ConfigureFileSystemWatcher()
        {
            try
            {
                _fileWatcher = new FileSystemWatcher(_options.PluginDirectory)
                {
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                    Filter = "*.dll"
                };

                _fileWatcher.Created += OnPluginFileChanged;
                _fileWatcher.Changed += OnPluginFileChanged;
                _fileWatcher.Deleted += OnPluginFileChanged;
                _fileWatcher.Renamed += OnPluginFileRenamed;
                _fileWatcher.Error += OnFileWatcherError;

                _fileWatcher.EnableRaisingEvents = true;
                _logger?.LogInformation("启动插件文件监视器");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "配置文件监视器时出错");
            }
        }

        /// <summary>
        /// 插件文件变更事件
        /// </summary>
        private void OnPluginFileChanged(object sender, FileSystemEventArgs e)
        {
            lock (_fileWatchLock)
            {
                _logger?.LogDebug("插件文件变更: {ChangeType} - {Path}", e.ChangeType, e.FullPath);
                // 热重载逻辑在MonitorPluginsAsync中处理
            }
        }

        /// <summary>
        /// 插件文件重命名事件
        /// </summary>
        private void OnPluginFileRenamed(object sender, RenamedEventArgs e)
        {
            lock (_fileWatchLock)
            {
                _logger?.LogDebug("插件文件重命名: {OldPath} -> {NewPath}", e.OldFullPath, e.FullPath);
                // 热重载逻辑在MonitorPluginsAsync中处理
            }
        }

        /// <summary>
        /// 文件监视器错误事件
        /// </summary>
        private void OnFileWatcherError(object sender, ErrorEventArgs e)
        {
            _logger?.LogError(e.GetException(), "文件监视器错误");
        }

        /// <summary>
        /// 监视插件变化
        /// </summary>
        private async Task MonitorPluginsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_options.HotReloadIntervalMs, cancellationToken);
                    await ReloadPluginsAsync(cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // 任务已取消，退出循环
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "监视插件变化时出错");
                }
            }
        }

        /// <summary>
        /// 重载插件
        /// </summary>
        private async Task ReloadPluginsAsync(CancellationToken cancellationToken)
        {
            try
            {
                // 获取插件目录中的所有DLL文件
                var pluginFiles = _fileSystem.Directory.GetFiles(_options.PluginDirectory, "*.dll", SearchOption.AllDirectories);

                // 检查需要加载的插件
                foreach (var pluginFile in pluginFiles)
                {
                    var fileInfo = _fileSystem.FileInfo.FromFileName(pluginFile);
                    var lastModified = fileInfo.LastWriteTimeUtc;

                    // 检查是否已加载
                    if (_loadedPlugins.TryGetValue(pluginFile, out var existingEntry))
                    {
                        // 检查是否需要重载
                        if (lastModified > existingEntry.Info.LastModified)
                        {
                            _logger?.LogInformation("插件文件已修改，准备重载: {PluginFile}", pluginFile);
                            await UnloadPluginAsync(pluginFile, cancellationToken);
                            await LoadPluginAsync(pluginFile, cancellationToken);
                        }
                    }
                    else
                    {
                        // 加载新插件
                        await LoadPluginAsync(pluginFile, cancellationToken);
                    }
                }

                // 检查需要卸载的插件
                var loadedPaths = _loadedPlugins.Keys.ToList();
                foreach (var loadedPath in loadedPaths)
                {
                    if (!pluginFiles.Contains(loadedPath))
                    {
                        _logger?.LogInformation("插件文件已删除，准备卸载: {PluginPath}", loadedPath);
                        await UnloadPluginAsync(loadedPath, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "重载插件时出错");
            }
        }

        /// <summary>
        /// 处理执行队列
        /// </summary>
        private async Task ProcessExecutionQueueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var item = await _executionQueue.Reader.ReadAsync(cancellationToken);
                    await ExecutePluginAsync(item.Path, item.Parameters, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // 任务已取消，退出循环
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "处理执行队列时出错");
                }
            }
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        public async Task<PluginInfo> LoadPluginAsync(string pluginPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            if (!_fileSystem.File.Exists(pluginPath))
            {
                throw new FileNotFoundException("插件文件不存在", pluginPath);
            }

            // 检查是否已加载
            if (_loadedPlugins.TryGetValue(pluginPath, out var existingEntry))
            {
                _logger?.LogDebug("插件已加载: {PluginPath}", pluginPath);
                return existingEntry.Info;
            }

            await _executionSemaphore.WaitAsync(cancellationToken);
            try
            {
                // 再次检查，防止并发加载
                if (_loadedPlugins.TryGetValue(pluginPath, out existingEntry))
                {
                    _logger?.LogDebug("插件已加载: {PluginPath}", pluginPath);
                    return existingEntry.Info;
                }

                var stopwatch = Stopwatch.StartNew();
                _logger?.LogInformation("开始加载插件: {PluginPath}", pluginPath);

                // 创建插件信息
                var pluginInfo = new PluginInfo
                {
                    Path = pluginPath,
                    Name = Path.GetFileNameWithoutExtension(pluginPath),
                    Status = PluginStatus.Loading,
                    LastModified = _fileSystem.FileInfo.FromFileName(pluginPath).LastWriteTimeUtc,
                    LoadTime = DateTime.UtcNow
                };

                // 创建加载上下文
                var loadContext = new PluginLoadContext(pluginPath, _logger);

                try
                {
                    // 加载程序集
                    var assembly = loadContext.LoadFromAssemblyPath(pluginPath);

                    // 查找插件类型
                    var pluginType = assembly.GetTypes()
                        .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);

                    if (pluginType == null)
                    {
                        throw new InvalidOperationException($"插件中未找到实现IPlugin接口的类型: {pluginPath}");
                    }

                    // 创建插件实例
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType);

                    // 设置服务提供程序
                    if (_options.EnableDependencyInjection)
                    {
                        plugin.SetServiceProvider(_serviceProvider);
                    }

                    // 更新插件信息
                    pluginInfo.Name = plugin.Name;
                    pluginInfo.Version = plugin.Version;
                    pluginInfo.Description = plugin.Description;
                    pluginInfo.Author = plugin.Author;
                    pluginInfo.Status = PluginStatus.Initializing;

                    // 初始化插件
                    if (_options.EnableLifecycleManagement)
                    {
                        using (var cts = new CancellationTokenSource(_options.PluginInitializationTimeoutMs))
                        using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token))
                        {
                            await plugin.InitializeAsync();
                        }
                    }

                    pluginInfo.Status = PluginStatus.Initialized;

                    // 缓存插件
                    _loadedPlugins[pluginPath] = (loadContext, plugin, pluginInfo);

                    stopwatch.Stop();
                    _logger?.LogInformation("插件加载成功: {PluginName}, 耗时: {ElapsedMilliseconds}ms", pluginInfo.Name, stopwatch.ElapsedMilliseconds);

                    // 更新性能指标
                    _pluginLoadCounter?.Add(1);
                    _loadedPluginGauge?.Record(_loadedPlugins.Count);

                    // 触发插件加载事件
                    PluginLoaded?.Invoke(this, new PluginEventArgs(pluginInfo));

                    return pluginInfo;
                }
                catch (Exception ex)
                {
                    pluginInfo.Status = PluginStatus.Error;
                    _logger?.LogError(ex, "加载插件时出错: {PluginPath}", pluginPath);
                    
                    // 触发插件错误事件
                    PluginError?.Invoke(this, new PluginEventArgs(pluginInfo));
                    
                    throw;
                }
            }
            finally
            {
                _executionSemaphore.Release();
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

            if (_loadedPlugins.TryRemove(pluginPath, out var entry))
            {
                try
                {
                    var pluginInfo = entry.Info;
                    pluginInfo.Status = PluginStatus.Unloading;

                    // 关闭插件
                    if (_options.EnableLifecycleManagement)
                    {
                        using (var cts = new CancellationTokenSource(_options.PluginInitializationTimeoutMs))
                        using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token))
                        {
                            await entry.Plugin.ShutdownAsync();
                        }
                    }

                    // 卸载上下文
                    entry.Context.Unload();

                    pluginInfo.Status = PluginStatus.Unloaded;
                    _logger?.LogInformation("插件卸载成功: {PluginName}", pluginInfo.Name);

                    // 更新性能指标
                    _loadedPluginGauge?.Record(_loadedPlugins.Count);

                    // 触发插件卸载事件
                    PluginUnloaded?.Invoke(this, new PluginEventArgs(pluginInfo));
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
        }

        /// <summary>
        /// 执行插件
        /// </summary>
        public async Task<object> ExecutePluginAsync(string pluginPath, object parameters = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            if (!_loadedPlugins.TryGetValue(pluginPath, out var entry))
            {
                throw new InvalidOperationException($"插件未加载: {pluginPath}");
            }

            var pluginInfo = entry.Info;
            if (pluginInfo.Status != PluginStatus.Initialized)
            {
                throw new InvalidOperationException($"插件状态不正确，无法执行: {pluginInfo.Status}");
            }

            await _executionSemaphore.WaitAsync(cancellationToken);
            try
            {
                var stopwatch = Stopwatch.StartNew();
                pluginInfo.Status = PluginStatus.Executing;

                try
                {
                    // 执行插件
                    using (var cts = new CancellationTokenSource(_options.PluginExecutionTimeoutMs))
                    using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token))
                    {
                        await entry.Plugin.ExecuteAsync(parameters);
                    }

                    stopwatch.Stop();
                    var executionTimeMs = stopwatch.Elapsed.TotalMilliseconds;

                    // 更新插件信息
                    pluginInfo.ExecutionCount++;
                    pluginInfo.AverageExecutionTimeMs = ((pluginInfo.AverageExecutionTimeMs * (pluginInfo.ExecutionCount - 1)) + executionTimeMs) / pluginInfo.ExecutionCount;
                    pluginInfo.Status = PluginStatus.Initialized;

                    _logger?.LogInformation("插件执行成功: {PluginName}, 耗时: {ExecutionTimeMs}ms", pluginInfo.Name, executionTimeMs);

                    // 更新性能指标
                    _pluginExecuteCounter?.Add(1);
                    _pluginExecutionTimeHistogram?.Record(executionTimeMs);

                    // 触发插件执行事件
                    PluginExecuted?.Invoke(this, new PluginEventArgs(pluginInfo));

                    return null;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    pluginInfo.Status = PluginStatus.Error;
                    _logger?.LogError(ex, "执行插件时出错: {PluginName}", pluginInfo.Name);
                    
                    // 触发插件错误事件
                    PluginError?.Invoke(this, new PluginEventArgs(pluginInfo));
                    
                    throw;
                }
            }
            finally
            {
                _executionSemaphore.Release();
            }
        }

        /// <summary>
        /// 队列执行插件
        /// </summary>
        public void QueueExecutePlugin(string pluginPath, object parameters = null)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            if (!_loadedPlugins.ContainsKey(pluginPath))
            {
                throw new InvalidOperationException($"插件未加载: {pluginPath}");
            }

            _executionQueue.Writer.TryWrite((pluginPath, parameters));
            _logger?.LogDebug("插件执行已加入队列: {PluginPath}", pluginPath);
        }

        /// <summary>
        /// 获取已加载的插件
        /// </summary>
        public IEnumerable<PluginInfo> GetLoadedPlugins()
        {
            return _loadedPlugins.Values.Select(entry => entry.Info);
        }

        /// <summary>
        /// 获取插件信息
        /// </summary>
        public PluginInfo GetPluginInfo(string pluginPath)
        {
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            if (_loadedPlugins.TryGetValue(pluginPath, out var entry))
            {
                return entry.Info;
            }

            return null;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("插件服务启动中...");

            try
            {
                // 加载所有插件
                await LoadAllPluginsAsync(cancellationToken);

                _logger?.LogInformation("插件服务启动成功");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "插件服务启动失败");
                throw;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation("插件服务停止中...");

            try
            {
                // 取消所有任务
                _cancellationTokenSource.Cancel();

                // 等待执行队列处理完成
                if (!_executionTask.IsCompleted)
                {
                    await Task.WhenAny(_executionTask, Task.Delay(5000, cancellationToken));
                }

                // 等待热重载任务完成
                if (_options.EnableHotReload && !_hotReloadTask.IsCompleted)
                {
                    await Task.WhenAny(_hotReloadTask, Task.Delay(2000, cancellationToken));
                }

                // 卸载所有插件
                await UnloadAllPluginsAsync(cancellationToken);

                _logger?.LogInformation("插件服务停止成功");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "插件服务停止失败");
                throw;
            }
        }

        /// <summary>
        /// 加载所有插件
        /// </summary>
        private async Task LoadAllPluginsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pluginFiles = _fileSystem.Directory.GetFiles(_options.PluginDirectory, "*.dll", SearchOption.AllDirectories);
                _logger?.LogInformation("发现 {PluginCount} 个插件文件", pluginFiles.Length);

                foreach (var pluginFile in pluginFiles)
                {
                    try
                    {
                        await LoadPluginAsync(pluginFile, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "加载插件时出错: {PluginFile}", pluginFile);
                        // 继续加载其他插件
                    }
                }

                _logger?.LogInformation("插件加载完成，已加载 {LoadedCount} 个插件", _loadedPlugins.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "加载所有插件时出错");
                throw;
            }
        }

        /// <summary>
        /// 卸载所有插件
        /// </summary>
        private async Task UnloadAllPluginsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var pluginPaths = _loadedPlugins.Keys.ToList();
                _logger?.LogInformation("开始卸载 {PluginCount} 个插件", pluginPaths.Count);

                foreach (var pluginPath in pluginPaths)
                {
                    try
                    {
                        await UnloadPluginAsync(pluginPath, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "卸载插件时出错: {PluginPath}", pluginPath);
                        // 继续卸载其他插件
                    }
                }

                _logger?.LogInformation("插件卸载完成");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "卸载所有插件时出错");
                throw;
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                // 取消任务
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();

                // 清理文件监视器
                if (_fileWatcher != null)
                {
                    _fileWatcher.Dispose();
                }

                // 清理信号量
                _executionSemaphore.Dispose();

                // 清理通道
                _executionQueue.Writer.Complete();

                _disposed = true;
            }
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
        public static IServiceCollection AddPluginServices(this IServiceCollection services, Action<PluginServiceOptions> configureOptions = null)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<PluginServiceOptions>(options => { });
            }

            // 添加文件系统
            services.AddSingleton<IFileSystem, FileSystem>();

            // 添加插件服务
            services.AddSingleton<PluginService>();
            services.AddHostedService(provider => provider.GetRequiredService<PluginService>());

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
            Console.WriteLine("Plugin Service Example");
            Console.WriteLine("=" * 50);

            // 构建主机
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
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
                        options.HotReloadIntervalMs = 2000;
                        options.EnableDependencyInjection = true;
                        options.EnableLifecycleManagement = true;
                        options.PluginInitializationTimeoutMs = 30000;
                        options.PluginExecutionTimeoutMs = 60000;
                    });
                })
                .Build();

            try
            {
                // 启动主机
                await host.StartAsync();

                // 获取插件服务
                var pluginService = host.Services.GetRequiredService<PluginService>();
                var logger = host.Services.GetRequiredService<ILogger<Program>>();

                // 注册事件处理程序
                pluginService.PluginLoaded += (sender, e) => logger.LogInformation("插件加载: {PluginName}", e.PluginInfo.Name);
                pluginService.PluginUnloaded += (sender, e) => logger.LogInformation("插件卸载: {PluginName}", e.PluginInfo.Name);
                pluginService.PluginExecuted += (sender, e) => logger.LogInformation("插件执行: {PluginName}", e.PluginInfo.Name);
                pluginService.PluginError += (sender, e) => logger.LogError("插件错误: {PluginName}", e.PluginInfo.Name);

                // 等待用户输入
                Console.WriteLine("插件服务已启动，按任意键退出...");
                Console.ReadKey();

                // 停止主机
                await host.StopAsync();
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "插件服务示例错误");
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
