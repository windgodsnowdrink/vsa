# Fody - 使用示例

## 快速开始

### 1. 基本使用示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fody.AOT;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var host = CreateHostBuilder().Build();
        var fodyService = host.Services.GetRequiredService<IFodyService>();
        
        Console.WriteLine("Fody 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 Fody 功能
        Console.WriteLine("1. 列出可用的织入器");
        var weaversResult = await fodyService.ListWeaversAsync();
        Console.WriteLine($"获取织入器结果: {(weaversResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"执行时间: {weaversResult.ExecutionTimeMs} ms");
        foreach (var item in weaversResult.Results)
        {
            Console.WriteLine($"- {item}");
        }
        
        Console.WriteLine("\n2. 织入程序集");
        // 注意：这里需要替换为实际的程序集路径
        var weaveResult = await fodyService.WeaveAssemblyAsync("MyAssembly.dll", "Output.dll");
        Console.WriteLine($"织入结果: {(weaveResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"执行时间: {weaveResult.ExecutionTimeMs} ms");
        foreach (var item in weaveResult.Results)
        {
            Console.WriteLine($"- {item}");
        }
        
        Console.WriteLine("\n3. 获取版本信息");
        var versionResult = await fodyService.GetVersionInfoAsync();
        Console.WriteLine($"获取版本信息结果: {(versionResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"执行时间: {versionResult.ExecutionTimeMs} ms");
        foreach (var item in versionResult.Results)
        {
            Console.WriteLine($"- {item}");
        }
    }
    
    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 Fody 选项
                services.Configure<FodyOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnablePerformanceMonitoring = true;
                    options.EnableCache = true;
                });
                
                // 注册服务
                services.AddSingleton<IFodyService, FodyService>();
                services.AddSingleton<FodyAotEngine>();
            });
    }
}
`

### 2. 高级配置示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Fody.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Fody 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 Fody 设置
                services.Configure<FodyOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnableDetailedLogging = true;
                    options.EnablePerformanceMonitoring = true;
                    options.RequestTimeoutMs = 60000; // 60秒超时
                    options.EnableCache = true;
                    options.CacheSize = 2000;
                });
                
                // 注册服务
                services.AddSingleton<IFodyService, FodyService>();
                services.AddSingleton<FodyAotEngine>();
            })
            .Build();
        
        // 获取配置
        var fodyOptions = host.Services.GetRequiredService<IOptions<FodyOptions>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"- 工作目录: {fodyOptions.WorkingDirectory}");
        Console.WriteLine($"- 启用详细日志: {fodyOptions.EnableDetailedLogging}");
        Console.WriteLine($"- 启用性能监控: {fodyOptions.EnablePerformanceMonitoring}");
        Console.WriteLine($"- 请求超时时间: {fodyOptions.RequestTimeoutMs} ms");
        Console.WriteLine($"- 启用缓存: {fodyOptions.EnableCache}");
        Console.WriteLine($"- 缓存大小: {fodyOptions.CacheSize}");
        
        // 使用服务
        var fodyService = host.Services.GetRequiredService<IFodyService>();
        
        // 列出可用的织入器
        var weaversResult = await fodyService.ListWeaversAsync();
        Console.WriteLine("\n可用的织入器:");
        foreach (var item in weaversResult.Results)
        {
            Console.WriteLine($"- {item}");
        }
    }
}
`

### 3. 性能优化示例

`csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fody.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Fody 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 Fody 选项
                services.Configure<FodyOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnablePerformanceMonitoring = true;
                    options.EnableCache = true; // 启用缓存以提高性能
                    options.CacheSize = 2000;
                });
                
                // 注册服务
                services.AddSingleton<IFodyService, FodyService>();
                services.AddSingleton<FodyAotEngine>();
            })
            .Build();
        
        var fodyService = host.Services.GetRequiredService<IFodyService>();
        
        // 性能测试
        const int iterations = 100;
        var totalTime = 0L;
        
        Console.WriteLine($"测试 {iterations} 次织入器列表获取操作的性能...");
        
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var result = await fodyService.ListWeaversAsync();
            if (result.Success)
            {
                totalTime += result.ExecutionTimeMs;
            }
            
            // 每10次迭代显示进度
            if ((i + 1) % 10 == 0)
            {
                Console.WriteLine($"已完成 {i + 1}/{iterations} 次迭代");
            }
        }
        
        stopwatch.Stop();
        
        Console.WriteLine("\n性能测试结果:");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"操作总时间: {totalTime:F3} ms");
        Console.WriteLine($"平均每次操作时间: {totalTime / (double)iterations:F3} ms");
        Console.WriteLine($"每秒可处理操作次数: {iterations / stopwatch.Elapsed.TotalSeconds:F2}");
    }
}
`

### 4. 错误处理示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fody.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Fody 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) => {
                // 配置 Fody 选项
                services.Configure<FodyOptions>(options => {
                    options.WorkingDirectory = Environment.CurrentDirectory;
                    options.EnablePerformanceMonitoring = true;
                });
                
                // 注册服务
                services.AddSingleton<IFodyService, FodyService>();
                services.AddSingleton<FodyAotEngine>();
            })
            .Build();
        
        var fodyService = host.Services.GetRequiredService<IFodyService>();
        
        Console.WriteLine("1. 测试各种错误情况");
        
        // 测试1: 织入不存在的程序集
        Console.WriteLine("\n测试1: 织入不存在的程序集");
        var nonExistentResult = await fodyService.WeaveAssemblyAsync("NonExistentAssembly.dll", "Output.dll");
        Console.WriteLine($"织入结果: {(nonExistentResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"执行时间: {nonExistentResult.ExecutionTimeMs} ms");
        if (!nonExistentResult.Success)
        {
            Console.WriteLine($"错误信息: {nonExistentResult.ErrorMessage}");
        }
        
        // 测试2: 正确操作示例
        Console.WriteLine("\n测试2: 列出可用的织入器（正确操作）");
        var weaversResult = await fodyService.ListWeaversAsync();
        Console.WriteLine($"获取织入器结果: {(weaversResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"执行时间: {weaversResult.ExecutionTimeMs} ms");
        if (weaversResult.Success && weaversResult.Results.Count > 0)
        {
            Console.WriteLine("第一个织入器: {0}", weaversResult.Results[1]);
        }
    }
}
`

### 5. 命令行工具使用示例

`bash
# 显示帮助信息
fody_aot help

# 列出可用的织入器
fody_aot weavers

# 列出可用的织入器（别名）
fody_aot list

# 织入程序集
fody_aot weave MyAssembly.dll Output.dll

# 显示版本信息
fody_aot version

# 显示版本信息（别名）
fody_aot info
`

## 总结

以上示例展示了 Fody 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本织入操作
2. 配置高级选项，如缓存和超时
3. 优化性能，适用于高并发场景
4. 正确处理错误情况
5. 使用命令行工具进行织入操作

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 关键特性回顾

- **AOT 编译**: 提前编译为本地代码，减少启动时间和内存占用
- **完整的织入功能**: 支持程序集织入、织入器管理等核心功能
- **详细的执行结果**: 返回详细的织入结果，包括执行时间、错误信息等
- **高性能设计**: 支持缓存、异步操作等性能优化特性
- **可扩展性**: 易于添加自定义织入器和扩展功能
- **命令行支持**: 提供方便的命令行工具

Fody AOT 引擎是一个功能强大、性能优异的代码织入解决方案，可满足各种织入场景的需求。
