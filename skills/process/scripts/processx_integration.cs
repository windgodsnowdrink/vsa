#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Diagnostics.Process@10.0.0
#:package System.Threading.Tasks.Dataflow@10.0.0
#:package System.IO.Pipelines@10.0.0
#:package System.Threading.Channels@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property Optimize=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.IO.Pipelines;
using System.Threading.Channels;

namespace ProcessXIntegration
{
    /// <summary>
    /// 进程管理选项
    /// </summary>
    public class ProcessManagementOptions
    {
        /// <summary>
        /// 是否启用进程池化
        /// </summary>
        public bool EnableProcessPooling { get; set; } = true;
        
        /// <summary>
        /// 最大进程数
        /// </summary>
        public int MaxProcesses { get; set; } = 10;
        
        /// <summary>
        /// 进程启动超时时间
        /// </summary>
        public TimeSpan ProcessStartTimeout { get; set; } = TimeSpan.FromSeconds(30);
        
        /// <summary>
        /// 进程空闲超时时间
        /// </summary>
        public TimeSpan ProcessIdleTimeout { get; set; } = TimeSpan.FromMinutes(5);
        
        /// <summary>
        /// 是否启用进程监控
        /// </summary>
        public bool EnableProcessMonitoring { get; set; } = true;
        
        /// <summary>
        /// CPU使用率检查间隔
        /// </summary>
        public TimeSpan CpuUsageCheckInterval { get; set; } = TimeSpan.FromSeconds(5);
        
        /// <summary>
        /// 最大CPU使用率百分比
        /// </summary>
        public float MaxCpuUsagePercentage { get; set; } = 80;
        
        /// <summary>
        /// 最大内存使用量（字节）
        /// </summary>
        public long MaxMemoryBytes { get; set; } = 1024 * 1024 * 1024; // 1GB
        
        /// <summary>
        /// 是否启用跨平台支持
        /// </summary>
        public bool EnableCrossPlatformSupport { get; set; } = true;
    }

    /// <summary>
    /// 进程信息
    /// </summary>
    public class ProcessInfo
    {
        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessId { get; set; }
        
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; }
        
        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// 内存使用量（字节）
        /// </summary>
        public long MemoryUsage { get; set; }
        
        /// <summary>
        /// CPU使用率（百分比）
        /// </summary>
        public float CpuUsage { get; set; }
        
        /// <summary>
        /// 进程状态
        /// </summary>
        public ProcessState State { get; set; }
    }

    /// <summary>
    /// 进程状态
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// 正在启动
        /// </summary>
        Starting,
        
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        
        /// <summary>
        /// 正在停止
        /// </summary>
        Stopping,
        
        /// <summary>
        /// 已停止
        /// </summary>
        Stopped,
        
        /// <summary>
        /// 出错
        /// </summary>
        Error
    }

    /// <summary>
    /// 进程管理器接口
    /// </summary>
    public interface IProcessManager
    {
        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="startInfo">进程启动信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>启动的进程</returns>
        Task<Process> StartProcessAsync(ProcessStartInfo startInfo, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 停止进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否成功停止</returns>
        Task<bool> StopProcessAsync(int processId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 获取进程信息
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns>进程信息</returns>
        ProcessInfo GetProcessInfo(int processId);
        
        /// <summary>
        /// 列出所有进程
        /// </summary>
        /// <returns>进程信息列表</returns>
        IEnumerable<ProcessInfo> ListProcesses();
        
        /// <summary>
        /// 监控进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>监控任务</returns>
        Task MonitorProcessAsync(int processId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 进程池接口
    /// </summary>
    public interface IProcessPool
    {
        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>池化进程</returns>
        Task<IPooledProcess> GetProcessAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 释放进程
        /// </summary>
        /// <param name="process">要释放的进程</param>
        void ReleaseProcess(IPooledProcess process);
        
        /// <summary>
        /// 获取池状态
        /// </summary>
        /// <returns>池状态</returns>
        ProcessPoolStatus GetPoolStatus();
    }

    /// <summary>
    /// 池化进程接口
    /// </summary>
    public interface IPooledProcess : IDisposable
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="arguments">参数</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>执行结果</returns>
        Task<string> ExecuteAsync(string command, string arguments, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="startInfo">进程启动信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>执行结果</returns>
        Task<string> ExecuteAsync(ProcessStartInfo startInfo, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 进程ID
        /// </summary>
        int ProcessId { get; }
        
        /// <summary>
        /// 进程状态
        /// </summary>
        ProcessState State { get; }
    }

    /// <summary>
    /// 进程池状态
    /// </summary>
    public class ProcessPoolStatus
    {
        /// <summary>
        /// 总进程数
        /// </summary>
        public int TotalProcesses { get; set; }
        
        /// <summary>
        /// 空闲进程数
        /// </summary>
        public int IdleProcesses { get; set; }
        
        /// <summary>
        /// 忙碌进程数
        /// </summary>
        public int BusyProcesses { get; set; }
        
        /// <summary>
        /// 最大进程数
        /// </summary>
        public int MaxProcesses { get; set; }
    }

    /// <summary>
    /// 进程管理器实现
    /// </summary>
    public class ProcessManager : IProcessManager, IDisposable
    {
        private readonly IOptions<ProcessManagementOptions> _options;
        private readonly ILogger<ProcessManager> _logger;
        private readonly ConcurrentDictionary<int, ProcessInfo> _processes;
        private readonly ConcurrentDictionary<int, Task> _monitoringTasks;
        private readonly CancellationTokenSource _cts;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">选项</param>
        /// <param name="logger">日志</param>
        public ProcessManager(IOptions<ProcessManagementOptions> options, ILogger<ProcessManager> logger)
        {
            _options = options;
            _logger = logger;
            _processes = new ConcurrentDictionary<int, ProcessInfo>();
            _monitoringTasks = new ConcurrentDictionary<int, Task>();
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 启动进程
        /// </summary>
        /// <param name="startInfo">进程启动信息</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>启动的进程</returns>
        public async Task<Process> StartProcessAsync(ProcessStartInfo startInfo, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting process: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);

            try
            {
                // 创建进程
                var process = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                // 启动进程
                if (!process.Start())
                {
                    throw new InvalidOperationException("Failed to start process");
                }

                // 创建进程信息
                var processInfo = new ProcessInfo
                {
                    ProcessId = process.Id,
                    ProcessName = process.ProcessName,
                    StartTime = DateTime.Now,
                    State = ProcessState.Running
                };

                // 添加到进程列表
                _processes[process.Id] = processInfo;

                // 注册进程退出事件
                process.Exited += (sender, e) =>
                {
                    if (_processes.TryGetValue(process.Id, out var info))
                    {
                        info.State = ProcessState.Stopped;
                        _logger.LogInformation("Process exited: {ProcessName} (ID: {ProcessId}) with code: {ExitCode}", 
                            process.ProcessName, process.Id, process.ExitCode);
                    }
                };

                // 启动监控（如果启用）
                if (_options.Value.EnableProcessMonitoring)
                {
                    var monitorCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
                    var monitoringTask = MonitorProcessAsync(process.Id, monitorCts.Token);
                    _monitoringTasks[process.Id] = monitoringTask;
                }

                _logger.LogInformation("Process started successfully: {ProcessName} (ID: {ProcessId})", 
                    process.ProcessName, process.Id);

                return process;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start process: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);
                throw;
            }
        }

        /// <summary>
        /// 停止进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否成功停止</returns>
        public async Task<bool> StopProcessAsync(int processId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Stopping process: {ProcessId}", processId);

            try
            {
                // 获取进程
                var process = Process.GetProcessById(processId);
                if (process == null)
                {
                    _logger.LogWarning("Process not found: {ProcessId}", processId);
                    return false;
                }

                // 更新进程状态
                if (_processes.TryGetValue(processId, out var info))
                {
                    info.State = ProcessState.Stopping;
                }

                // 尝试优雅停止
                process.CloseMainWindow();
                
                // 等待进程退出
                if (!await process.WaitForExitAsync(TimeSpan.FromSeconds(30), cancellationToken))
                {
                    // 强制终止
                    _logger.LogWarning("Forcing process termination: {ProcessId}", processId);
                    process.Kill(true);
                    await process.WaitForExitAsync(cancellationToken);
                }

                // 更新进程状态
                if (_processes.TryGetValue(processId, out info))
                {
                    info.State = ProcessState.Stopped;
                }

                // 移除监控任务
                _monitoringTasks.TryRemove(processId, out _);

                _logger.LogInformation("Process stopped successfully: {ProcessId}", processId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop process: {ProcessId}", processId);
                return false;
            }
        }

        /// <summary>
        /// 获取进程信息
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <returns>进程信息</returns>
        public ProcessInfo GetProcessInfo(int processId)
        {
            if (_processes.TryGetValue(processId, out var info))
            {
                try
                {
                    var process = Process.GetProcessById(processId);
                    if (process != null)
                    {
                        // 更新内存使用量
                        info.MemoryUsage = process.WorkingSet64;
                        
                        // 简单的CPU使用率计算（实际项目中可能需要更复杂的实现）
                        info.CpuUsage = CalculateCpuUsage(process);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to get process info: {ProcessId}", processId);
                }

                return info;
            }

            return null;
        }

        /// <summary>
        /// 列出所有进程
        /// </summary>
        /// <returns>进程信息列表</returns>
        public IEnumerable<ProcessInfo> ListProcesses()
        {
            return _processes.Values;
        }

        /// <summary>
        /// 监控进程
        /// </summary>
        /// <param name="processId">进程ID</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>监控任务</returns>
        public async Task MonitorProcessAsync(int processId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting process monitoring: {ProcessId}", processId);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_processes.TryGetValue(processId, out var info))
                    {
                        break;
                    }

                    try
                    {
                        var process = Process.GetProcessById(processId);
                        if (process == null)
                        {
                            break;
                        }

                        // 更新进程信息
                        info.MemoryUsage = process.WorkingSet64;
                        info.CpuUsage = CalculateCpuUsage(process);

                        // 检查资源使用
                        if (info.CpuUsage > _options.Value.MaxCpuUsagePercentage)
                        {
                            _logger.LogWarning("Process CPU usage exceeded threshold: {ProcessId}, Usage: {CpuUsage}%", 
                                processId, info.CpuUsage);
                        }

                        if (info.MemoryUsage > _options.Value.MaxMemoryBytes)
                        {
                            _logger.LogWarning("Process memory usage exceeded threshold: {ProcessId}, Usage: {MemoryUsage} bytes", 
                                processId, info.MemoryUsage);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error monitoring process: {ProcessId}", processId);
                    }

                    // 等待下一次检查
                    await Task.Delay(_options.Value.CpuUsageCheckInterval, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // 监控被取消
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in process monitoring: {ProcessId}", processId);
            }
            finally
            {
                _logger.LogInformation("Process monitoring stopped: {ProcessId}", processId);
            }
        }

        /// <summary>
        /// 计算CPU使用率
        /// </summary>
        /// <param name="process">进程</param>
        /// <returns>CPU使用率（百分比）</returns>
        private float CalculateCpuUsage(Process process)
        {
            // 简单实现，实际项目中可能需要更复杂的计算
            // 这里返回一个模拟值，实际实现需要使用性能计数器
            return 0;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    /// <summary>
    /// 进程池实现
    /// </summary>
    public class ProcessPool : IProcessPool, IDisposable
    {
        private readonly IOptions<ProcessManagementOptions> _options;
        private readonly ILogger<ProcessPool> _logger;
        private readonly ConcurrentBag<IPooledProcess> _idleProcesses;
        private readonly ConcurrentDictionary<int, IPooledProcess> _busyProcesses;
        private readonly SemaphoreSlim _semaphore;
        private readonly CancellationTokenSource _cts;
        private readonly Task _cleanupTask;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">选项</param>
        /// <param name="logger">日志</param>
        public ProcessPool(IOptions<ProcessManagementOptions> options, ILogger<ProcessPool> logger)
        {
            _options = options;
            _logger = logger;
            _idleProcesses = new ConcurrentBag<IPooledProcess>();
            _busyProcesses = new ConcurrentDictionary<int, IPooledProcess>();
            _semaphore = new SemaphoreSlim(options.Value.MaxProcesses);
            _cts = new CancellationTokenSource();
            
            // 启动清理任务
            _cleanupTask = RunCleanupAsync(_cts.Token);
        }

        /// <summary>
        /// 获取进程
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>池化进程</returns>
        public async Task<IPooledProcess> GetProcessAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Getting process from pool");

            // 等待信号量
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                // 尝试从空闲池获取
                if (_idleProcesses.TryTake(out var pooledProcess))
                {
                    // 检查进程是否有效
                    if (pooledProcess.State == ProcessState.Running)
                    {
                        // 标记为忙碌
                        _busyProcesses[pooledProcess.ProcessId] = pooledProcess;
                        _logger.LogDebug("Got idle process from pool: {ProcessId}", pooledProcess.ProcessId);
                        return pooledProcess;
                    }
                    else
                    {
                        // 进程无效，释放并创建新的
                        pooledProcess.Dispose();
                        _logger.LogDebug("Discarded invalid process: {ProcessId}", pooledProcess.ProcessId);
                    }
                }

                // 创建新进程
                var newProcess = CreatePooledProcess();
                _busyProcesses[newProcess.ProcessId] = newProcess;
                _logger.LogDebug("Created new pooled process: {ProcessId}", newProcess.ProcessId);
                return newProcess;
            }
            catch
            {
                // 释放信号量
                _semaphore.Release();
                throw;
            }
        }

        /// <summary>
        /// 释放进程
        /// </summary>
        /// <param name="process">要释放的进程</param>
        public void ReleaseProcess(IPooledProcess process)
        {
            if (process == null)
            {
                _semaphore.Release();
                return;
            }

            _logger.LogDebug("Releasing process to pool: {ProcessId}", process.ProcessId);

            try
            {
                // 从忙碌列表移除
                _busyProcesses.TryRemove(process.ProcessId, out _);

                // 检查进程是否有效
                if (process.State == ProcessState.Running)
                {
                    // 添加到空闲池
                    _idleProcesses.Add(process);
                    _logger.LogDebug("Process added to idle pool: {ProcessId}", process.ProcessId);
                }
                else
                {
                    // 进程无效，释放
                    process.Dispose();
                    _logger.LogDebug("Discarded invalid process: {ProcessId}", process.ProcessId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing process: {ProcessId}", process.ProcessId);
            }
            finally
            {
                // 释放信号量
                _semaphore.Release();
            }
        }

        /// <summary>
        /// 获取池状态
        /// </summary>
        /// <returns>池状态</returns>
        public ProcessPoolStatus GetPoolStatus()
        {
            return new ProcessPoolStatus
            {
                TotalProcesses = _idleProcesses.Count + _busyProcesses.Count,
                IdleProcesses = _idleProcesses.Count,
                BusyProcesses = _busyProcesses.Count,
                MaxProcesses = _options.Value.MaxProcesses
            };
        }

        /// <summary>
        /// 创建池化进程
        /// </summary>
        /// <returns>池化进程</returns>
        private IPooledProcess CreatePooledProcess()
        {
            // 这里返回一个模拟实现，实际项目中需要根据具体需求实现
            // 例如，可以启动一个通用的命令行进程，用于执行各种命令
            return new MockPooledProcess();
        }

        /// <summary>
        /// 运行清理任务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>清理任务</returns>
        private async Task RunCleanupAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // 清理无效进程
                    var processesToRemove = new List<IPooledProcess>();
                    
                    foreach (var process in _idleProcesses)
                    {
                        if (process.State != ProcessState.Running)
                        {
                            processesToRemove.Add(process);
                        }
                    }

                    // 移除无效进程
                    foreach (var process in processesToRemove)
                    {
                        _idleProcesses.TryTake(out var _);
                        process.Dispose();
                        _logger.LogDebug("Cleaned up invalid process from pool");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in pool cleanup task");
                }

                // 等待下一次清理
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            
            // 释放所有进程
            foreach (var process in _idleProcesses)
            {
                process.Dispose();
            }
            
            foreach (var process in _busyProcesses.Values)
            {
                process.Dispose();
            }
            
            _semaphore.Dispose();
        }

        /// <summary>
        /// 模拟池化进程实现
        /// </summary>
        private class MockPooledProcess : IPooledProcess
        {
            public int ProcessId => 0;
            public ProcessState State => ProcessState.Running;

            public Task<string> ExecuteAsync(string command, string arguments, CancellationToken cancellationToken = default)
            {
                // 模拟执行命令
                return Task.FromResult($"Executed: {command} {arguments}");
            }

            public Task<string> ExecuteAsync(ProcessStartInfo startInfo, CancellationToken cancellationToken = default)
            {
                // 模拟执行命令
                return Task.FromResult($"Executed: {startInfo.FileName} {startInfo.Arguments}");
            }

            public void Dispose()
            {
                // 释放资源
            }
        }
    }

    /// <summary>
    /// 依赖注入扩展
    /// </summary>
    public static class ProcessManagementExtensions
    {
        /// <summary>
        /// 添加进程管理服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddProcessManagement(this IServiceCollection services)
        {
            return AddProcessManagement(services, options => { });
        }

        /// <summary>
        /// 添加进程管理服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddProcessManagement(this IServiceCollection services, Action<ProcessManagementOptions> configureOptions)
        {
            // 配置选项
            services.Configure(configureOptions);

            // 添加服务
            services.AddSingleton<IProcessManager, ProcessManager>();
            services.AddSingleton<IProcessPool, ProcessPool>();

            return services;
        }
    }
}

// 主程序入口
public class Program
{
    public static async Task Main(string[] args)
    {
        // 构建服务提供器
        var serviceProvider = BuildServiceProvider();
        
        // 获取进程管理服务
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        // 示例：启动一个进程
        var processInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "--version",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        using (var process = await processManager.StartProcessAsync(processInfo))
        {
            // 读取输出
            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            
            Console.WriteLine("Process output:");
            Console.WriteLine(output);
            
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Process error:");
                Console.WriteLine(error);
            }
            
            // 等待进程完成
            await process.WaitForExitAsync();
            Console.WriteLine($"Process exited with code: {process.ExitCode}");
        }
        
        // 示例：使用进程池
        var processPool = serviceProvider.GetRequiredService<IProcessPool>();
        
        using (var pooledProcess = await processPool.GetProcessAsync())
        {
            var result = await pooledProcess.ExecuteAsync("echo", "Hello from pooled process");
            Console.WriteLine($"Pooled process result: {result}");
        }
        
        // 获取池状态
        var poolStatus = processPool.GetPoolStatus();
        Console.WriteLine($"Pool status: Total={poolStatus.TotalProcesses}, Idle={poolStatus.IdleProcesses}, Busy={poolStatus.BusyProcesses}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // 添加日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });
        
        // 添加进程管理服务
        services.AddProcessManagement(options =>
        {
            options.EnableProcessPooling = true;
            options.MaxProcesses = 5;
            options.EnableProcessMonitoring = true;
        });
        
        return services.BuildServiceProvider();
    }
}