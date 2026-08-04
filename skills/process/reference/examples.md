# process - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 启动记事本进程
            var startInfo = new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = "",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            Console.WriteLine("启动记事本进程...");
            var process = await processManager.StartProcessAsync(startInfo);
            Console.WriteLine($"进程已启动，ID: {process.Id}");
            
            // 等待3秒
            await Task.Delay(3000);
            
            // 终止进程
            Console.WriteLine("终止进程...");
            await processManager.StopProcessAsync(process.Id);
            Console.WriteLine("进程已终止");
            
            // 获取进程信息
            var processInfo = processManager.GetProcessInfo(process.Id);
            Console.WriteLine($"进程退出代码: {processInfo.ExitCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices(options => {
            options.EnableProcessPooling = false;
            options.EnableProcessMonitoring = true;
        });
        return builder.BuildServiceProvider();
    }
}
```

### 2. 进程池使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 进程池使用示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var processPool = serviceProvider.GetRequiredService<IProcessPool>();
        
        try
        {
            // 配置进程池
            var poolConfig = new ProcessPoolConfig
            {
                ProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c echo Hello from ProcessX",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                MinProcesses = 2,
                MaxProcesses = 5,
                IdleTimeout = TimeSpan.FromMinutes(2)
            };
            
            Console.WriteLine("初始化进程池...");
            await processPool.InitializeAsync(poolConfig);
            Console.WriteLine("进程池初始化完成");
            
            // 并行执行多个任务
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                int taskId = i;
                tasks.Add(Task.Run(async () => {
                    Console.WriteLine($"任务 {taskId}: 从进程池获取进程");
                    using var pooledProcess = await processPool.GetProcessAsync();
                    Console.WriteLine($"任务 {taskId}: 使用进程 ID: {pooledProcess.Process.Id}");
                    
                    // 读取进程输出
                    string output = await pooledProcess.Process.StandardOutput.ReadToEndAsync();
                    Console.WriteLine($"任务 {taskId}: 进程输出: {output.Trim()}");
                    
                    // 模拟工作
                    await Task.Delay(1000);
                    
                    Console.WriteLine($"任务 {taskId}: 完成，进程返回池");
                }));
            }
            
            // 等待所有任务完成
            await Task.WhenAll(tasks);
            
            // 获取进程池状态
            var poolStatus = await processPool.GetPoolStatusAsync();
            Console.WriteLine($"\n进程池状态:");
            Console.WriteLine($"总进程数: {poolStatus.TotalProcesses}");
            Console.WriteLine($"空闲进程数: {poolStatus.IdleProcesses}");
            Console.WriteLine($"忙进程数: {poolStatus.BusyProcesses}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 关闭进程池
            await processPool.ShutdownAsync();
            Console.WriteLine("进程池已关闭");
            
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices(options => {
            options.EnableProcessPooling = true;
            options.MaxProcesses = 10;
            options.EnableProcessMonitoring = true;
        });
        return builder.BuildServiceProvider();
    }
}
```

### 3. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 ProcessX 设置
        builder.Configure<ProcessManagementOptions>(options => {
            options.EnableProcessPooling = true;
            options.MaxProcesses = 20;
            options.ProcessStartTimeout = TimeSpan.FromSeconds(60);
            options.ProcessIdleTimeout = TimeSpan.FromMinutes(10);
            options.EnableProcessMonitoring = true;
            options.CpuUsageCheckInterval = TimeSpan.FromSeconds(2);
            options.MaxCpuUsagePercentage = 75;
            options.MaxMemoryBytes = 2147483648; // 2GB
            options.EnableCrossPlatformSupport = true;
        });
        
        // 注册服务
        builder.AddProcessXServices();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<ProcessManagementOptions>>().Value;
        Console.WriteLine("当前配置:");
        Console.WriteLine($"启用进程池: {settings.EnableProcessPooling}");
        Console.WriteLine($"最大进程数: {settings.MaxProcesses}");
        Console.WriteLine($"进程启动超时: {settings.ProcessStartTimeout}");
        Console.WriteLine($"进程空闲超时: {settings.ProcessIdleTimeout}");
        Console.WriteLine($"启用进程监控: {settings.EnableProcessMonitoring}");
        Console.WriteLine($"CPU 检查间隔: {settings.CpuUsageCheckInterval}");
        Console.WriteLine($"最大 CPU 使用率: {settings.MaxCpuUsagePercentage}%");
        Console.WriteLine($"最大内存使用量: {settings.MaxMemoryBytes / 1024 / 1024}MB");
        Console.WriteLine($"启用跨平台支持: {settings.EnableCrossPlatformSupport}");
        
        // 使用服务
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 启动一个命令行进程
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c dir",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            
            Console.WriteLine("\n启动命令行进程...");
            var process = await processManager.StartProcessAsync(startInfo);
            
            // 读取输出
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            
            Console.WriteLine("进程执行完成，输出:");
            Console.WriteLine(output.Substring(0, Math.Min(output.Length, 500))); // 只显示前500个字符
            if (output.Length > 500)
            {
                Console.WriteLine("... (输出被截断)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
}
```

### 4. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var processPool = serviceProvider.GetRequiredService<IProcessPool>();
        
        try
        {
            // 配置进程池
            var poolConfig = new ProcessPoolConfig
            {
                ProcessStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c echo Test",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                },
                MinProcesses = 5,
                MaxProcesses = 10,
                IdleTimeout = TimeSpan.FromMinutes(5)
            };
            
            Console.WriteLine("初始化进程池...");
            await processPool.InitializeAsync(poolConfig);
            
            // 性能测试
            const int iterations = 100;
            var stopwatch = Stopwatch.StartNew();
            
            Console.WriteLine($"执行 {iterations} 次进程操作...");
            
            var tasks = new List<Task>();
            for (int i = 0; i < iterations; i++)
            {
                tasks.Add(Task.Run(async () => {
                    using var pooledProcess = await processPool.GetProcessAsync();
                    // 读取输出以确保进程执行完成
                    await pooledProcess.Process.StandardOutput.ReadToEndAsync();
                }));
            }
            
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            
            Console.WriteLine($"执行完成!");
            Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
            Console.WriteLine($"平均执行时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2} ms");
            
            // 获取性能指标
            var poolStatus = await processPool.GetPoolStatusAsync();
            Console.WriteLine($"\n进程池状态:");
            Console.WriteLine($"总进程数: {poolStatus.TotalProcesses}");
            Console.WriteLine($"空闲进程数: {poolStatus.IdleProcesses}");
            Console.WriteLine($"忙进程数: {poolStatus.BusyProcesses}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 关闭进程池
            await processPool.ShutdownAsync();
            
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices(options => {
            options.EnableProcessPooling = true;
            options.MaxProcesses = 20;
            options.EnableProcessMonitoring = false; // 禁用监控以提高性能
        });
        return builder.BuildServiceProvider();
    }
}
```

### 5. 错误处理示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 测试1: 启动不存在的进程
            Console.WriteLine("测试1: 启动不存在的进程");
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "nonexistent.exe",
                    Arguments = "",
                    UseShellExecute = false
                };
                
                var process = await processManager.StartProcessAsync(startInfo);
                Console.WriteLine("意外: 进程启动成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"预期错误: {ex.GetType().Name}: {ex.Message}");
            }
            
            // 测试2: 进程启动超时
            Console.WriteLine("\n测试2: 进程启动超时");
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c ping localhost -n 10",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                // 设置1秒超时
                var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(1));
                var process = await processManager.StartProcessAsync(startInfo, cts.Token);
                await process.WaitForExitAsync();
                Console.WriteLine("进程执行完成");
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"预期取消: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.GetType().Name}: {ex.Message}");
            }
            
            // 测试3: 无效的进程ID
            Console.WriteLine("\n测试3: 获取无效的进程信息");
            try
            {
                var processInfo = processManager.GetProcessInfo(999999); // 不存在的进程ID
                Console.WriteLine("意外: 获取进程信息成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"预期错误: {ex.GetType().Name}: {ex.Message}");
            }
            
            // 测试4: 正常执行
            Console.WriteLine("\n测试4: 正常执行命令");
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c echo Hello World",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
                
                var process = await processManager.StartProcessAsync(startInfo);
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                
                Console.WriteLine($"成功: {output.Trim()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.GetType().Name}: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"全局错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices(options => {
            options.EnableProcessMonitoring = true;
        });
        return builder.BuildServiceProvider();
    }
}
```

### 6. 跨平台使用示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 跨平台使用示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 检测当前平台
            var platform = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows) ? "Windows" :
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Linux) ? "Linux" :
                System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.OSX) ? "macOS" : "Unknown";
            
            Console.WriteLine($"当前平台: {platform}");
            
            // 根据平台执行不同的命令
            ProcessStartInfo startInfo;
            
            if (platform == "Windows")
            {
                // Windows: 执行 dir 命令
                startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c dir",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
            }
            else if (platform == "Linux" || platform == "macOS")
            {
                // Linux/macOS: 执行 ls 命令
                startInfo = new ProcessStartInfo
                {
                    FileName = "ls",
                    Arguments = "-la",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
            }
            else
            {
                Console.WriteLine("不支持的平台");
                return;
            }
            
            Console.WriteLine("执行平台特定命令...");
            var process = await processManager.StartProcessAsync(startInfo);
            
            // 读取输出
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            
            Console.WriteLine("命令执行结果:");
            Console.WriteLine(output.Substring(0, Math.Min(output.Length, 800))); // 只显示前800个字符
            if (output.Length > 800)
            {
                Console.WriteLine("... (输出被截断)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices(options => {
            options.EnableCrossPlatformSupport = true;
            options.EnableProcessMonitoring = true;
        });
        return builder.BuildServiceProvider();
    }
}
```

### 7. AOT 编译示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

// AOT 编译支持
// 注意: 此代码已优化为支持 AOT 编译
// 避免使用反射、动态代码等不兼容 AOT 的特性

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 启动一个简单的进程
            var startInfo = new ProcessStartInfo
            {
                FileName = GetPlatformCommand(),
                Arguments = GetPlatformArgument(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            
            Console.WriteLine("启动进程...");
            var process = await processManager.StartProcessAsync(startInfo);
            
            // 读取输出
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            
            Console.WriteLine("进程执行完成，输出:");
            Console.WriteLine(output.Trim());
            
            Console.WriteLine("\nAOT 编译示例执行成功!");
            Console.WriteLine("此代码已优化为支持 AOT 编译，避免了反射和动态代码。");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置并注册服务
        builder.Configure<ProcessManagementOptions>(options => {
            options.EnableProcessPooling = true;
            options.MaxProcesses = 5;
            options.EnableProcessMonitoring = false; // 禁用监控以减少 AOT 大小
        });
        
        // 注册服务
        builder.AddProcessXServices();
        
        return builder.BuildServiceProvider();
    }
    
    // 平台检测辅助方法
    private static string GetPlatformCommand()
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return "cmd.exe";
        }
        else
        {
            return "echo";
        }
    }
    
    private static string GetPlatformArgument()
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return "/c echo Hello from AOT compiled ProcessX";
        }
        else
        {
            return "Hello from AOT compiled ProcessX";
        }
    }
}
```

## 高级示例

### 1. 进程监控示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 进程监控示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 启动一个计算密集型进程
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c for /l %i in (1,1,1000000) do echo %i > nul",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            Console.WriteLine("启动计算密集型进程...");
            var process = await processManager.StartProcessAsync(startInfo);
            int processId = process.Id;
            Console.WriteLine($"进程已启动，ID: {processId}");
            
            // 监控进程
            Console.WriteLine("\n监控进程状态...");
            Console.WriteLine("按任意键停止监控");
            
            var monitoringTask = Task.Run(async () => {
                while (!Console.KeyAvailable)
                {
                    var processInfo = processManager.GetProcessInfo(processId);
                    if (processInfo != null)
                    {
                        Console.WriteLine($"进程 ID: {processInfo.ProcessId}, 状态: {processInfo.State}, " +
                                      $"CPU: {processInfo.CpuUsagePercentage:F1}%, " +
                                      $"内存: {processInfo.MemoryUsageBytes / 1024 / 1024:F1}MB");
                    }
                    await Task.Delay(1000);
                }
            });
            
            // 等待用户输入
            Console.ReadKey(true);
            
            // 终止进程
            Console.WriteLine("\n终止进程...");
            await processManager.StopProcessAsync(processId);
            
            // 等待监控任务完成
            await monitoringTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices(options => {
            options.EnableProcessMonitoring = true;
            options.CpuUsageCheckInterval = TimeSpan.FromSeconds(1);
        });
        return builder.BuildServiceProvider();
    }
}
```

### 2. 进程间通信示例

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProcessXIntegration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ProcessX 进程间通信示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var processManager = serviceProvider.GetRequiredService<IProcessManager>();
        
        try
        {
            // 创建一个临时文件用于通信
            string tempFile = Path.GetTempFileName();
            Console.WriteLine($"创建临时文件: {tempFile}");
            
            // 写入一些数据到临时文件
            await File.WriteAllTextAsync(tempFile, "Hello from parent process!");
            
            // 启动一个进程来读取和修改临时文件
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c type "{tempFile}" && echo "Hello from child process!" >> "{tempFile}" && type "{tempFile}"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            
            Console.WriteLine("启动子进程...");
            var process = await processManager.StartProcessAsync(startInfo);
            
            // 读取子进程输出
            string output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            
            Console.WriteLine("子进程输出:");
            Console.WriteLine(output);
            
            // 清理临时文件
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
                Console.WriteLine($"临时文件已删除: {tempFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 释放服务
            if (serviceProvider is IDisposable disposable) {
                disposable.Dispose();
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddProcessXServices();
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例展示了 ProcessX 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 实现跨平台支持
6. 利用 AOT 编译提高性能
7. 监控进程状态
8. 实现进程间通信

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

所有示例代码都已优化为支持 AOT 编译，避免了反射、动态代码等不兼容 AOT 的特性，确保在 AOT 编译模式下能够正常运行。
