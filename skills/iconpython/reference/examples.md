# iconpython - 使用示例

## 快速入门

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var pythonService = serviceProvider.GetRequiredService<PythonService>();
        
        Console.WriteLine("IconPython 基本使用示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 执行 Python 脚本
            Console.WriteLine("执行 Python 脚本...");
            var scriptResult = await pythonService.RunScriptAsync("test.py");
            Console.WriteLine($"脚本执行状态: {(scriptResult.Success ? "成功" : "失败"}");
            
            if (!string.IsNullOrEmpty(scriptResult.Output))
            {
                Console.WriteLine($"脚本输出: {scriptResult.Output}");
            }
            
            // 计算 Python 表达式
            Console.WriteLine("\n计算 Python 表达式...");
            var evalResult = await pythonService.EvaluateExpressionAsync("1 + 1");
            Console.WriteLine($"表达式计算结果: {evalResult}");
            
            // 导入 Python 模块
            Console.WriteLine("\n导入 Python 模块...");
            var module = await pythonService.ImportModuleAsync("math");
            Console.WriteLine($"模块导入状态: {(module != null ? "成功" : "失败"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<PythonSettings>(options => {
            options.BasePath = Environment.CurrentDirectory;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDebug = false;
            options.EnableTracing = false;
            options.ModulePaths = new List<string> {
                Path.Combine(Environment.CurrentDirectory, "modules"),
                Path.Combine(Environment.CurrentDirectory, "scripts")
            };
        });
        
        services.AddSingleton<PythonService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("IconPython 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置 IconPython 设置
        services.Configure<PythonSettings>(options => {
            options.BasePath = Environment.CurrentDirectory;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDebug = true;
            options.EnableTracing = true;
            options.ModulePaths = new List<string> {
                Path.Combine(Environment.CurrentDirectory, "modules"),
                Path.Combine(Environment.CurrentDirectory, "scripts"),
                Path.Combine(Environment.CurrentDirectory, "custom_modules")
            };
            options.GlobalVariables = new Dictionary<string, object> {
                { "__name__", "__main__" },
                { "__file__", "Program.cs" },
                { "custom_function", new Func<int, int, int>((a, b) => a + b) }
            };
        });
        
        // 注册服务
        services.AddSingleton<PythonService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<PythonSettings>>().Value;
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"  基础路径: {settings.BasePath}");
        Console.WriteLine($"  超时时间: {settings.Timeout}");
        Console.WriteLine($"  启用调试: {settings.EnableDebug}");
        Console.WriteLine($"  启用跟踪: {settings.EnableTracing}");
        Console.WriteLine($"  模块路径数量: {settings.ModulePaths.Count}");
        Console.WriteLine($"  全局变量数量: {settings.GlobalVariables.Count}");
        
        // 使用服务
        var pythonService = serviceProvider.GetRequiredService<PythonService>();
        
        try
        {
            // 测试自定义全局变量
            Console.WriteLine("\n测试自定义全局变量...");
            var customResult = await pythonService.EvaluateExpressionAsync("custom_function(10, 20)");
            Console.WriteLine($"自定义函数结果: {customResult}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("IconPython 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var pythonService = serviceProvider.GetRequiredService<PythonService>();
        
        try
        {
            // 性能测试
            const int iterations = 1000;
            var stopwatch = Stopwatch.StartNew();
            
            Console.WriteLine($"运行性能测试，迭代次数: {iterations}");
            
            var successes = 0;
            var failures = 0;
            
            for (int i = 0; i < iterations; i++)
            {
                try
                {
                    await pythonService.EvaluateExpressionAsync("1 + 1");
                    successes++;
                }
                catch
                {
                    failures++;
                }
            }
            
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            var averagePerIteration = elapsedMilliseconds / iterations;
            var operationsPerSecond = iterations / (stopwatch.Elapsed.TotalSeconds);
            
            Console.WriteLine($"性能测试完成!");
            Console.WriteLine($"总执行时间: {elapsedMilliseconds:F3} ms");
            Console.WriteLine($"平均每次执行时间: {averagePerIteration:F3} ms");
            Console.WriteLine($"每秒操作数: {operationsPerSecond:F2} ops/s");
            Console.WriteLine($"成功次数: {successes}");
            Console.WriteLine($"失败次数: {failures}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<PythonSettings>(options => {
            options.BasePath = Environment.CurrentDirectory;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDebug = false;
            options.EnableTracing = false;
        });
        
        services.AddSingleton<PythonService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("IconPython 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var pythonService = serviceProvider.GetRequiredService<PythonService>();
        
        try
        {
            // 测试不存在的脚本
            Console.WriteLine("测试不存在的脚本...");
            var scriptResult = await pythonService.RunScriptAsync("nonexistent.py");
            Console.WriteLine($"脚本执行状态: {(scriptResult.Success ? "成功" : "失败"}");
            
            if (!string.IsNullOrEmpty(scriptResult.Error))
            {
                Console.WriteLine($"脚本错误: {scriptResult.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        try
        {
            // 测试语法错误的表达式
            Console.WriteLine("\n测试语法错误的表达式...");
            var evalResult = await pythonService.EvaluateExpressionAsync("1 + ");
            Console.WriteLine($"表达式计算结果: {evalResult}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        try
        {
            // 测试不存在的模块
            Console.WriteLine("\n测试不存在的模块...");
            var module = await pythonService.ImportModuleAsync("nonexistent_module");
            Console.WriteLine($"模块导入状态: {(module != null ? "成功" : "失败"}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<PythonSettings>(options => {
            options.BasePath = Environment.CurrentDirectory;
            options.Timeout = TimeSpan.FromSeconds(30);
        });
        
        services.AddSingleton<PythonService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 5. IconPython AOT 核心引擎示例

#### IconPython AOT 命令行使用示例

```bash
# 运行 Python 脚本
iconpython_aot run test.py

# 计算 Python 表达式
iconpython_aot eval 1 + 1

# 启动 REPL 环境
iconpython_aot repl

# 运行基准测试
iconpython_aot benchmark 1000 1 + 1

# 运行 Python 模块
iconpython_aot module math

# 导入 Python 模块
iconpython_aot import math

# 显示配置
iconpython_aot config

# 显示帮助
iconpython_aot help
```

#### IconPython AOT 核心引擎代码示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package IronPython@3.4.1
#:package Microsoft.Scripting@1.3.4
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IconPython AOT 核心引擎示例");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var pythonService = serviceProvider.GetRequiredService<PythonService>();
        
        try
        {
            // 测试 Python 表达式计算
            Console.WriteLine("测试 Python 表达式计算...");
            var evalResult = await pythonService.EvaluateExpressionAsync("1 + 1");
            Console.WriteLine($"表达式计算结果: {evalResult}");
            
            // 测试 Python 模块导入
            Console.WriteLine("\n测试 Python 模块导入...");
            var module = await pythonService.ImportModuleAsync("math");
            Console.WriteLine($"模块导入状态: {(module != null ? "成功" : "失败"}");
            
            // 测试 Python 脚本执行
            Console.WriteLine("\n测试 Python 脚本执行...");
            // 创建测试脚本
            await File.WriteAllTextAsync("test.py", "print('Hello from Python!')\nprint('2 + 2 =', 2 + 2)");
            
            var scriptResult = await pythonService.RunScriptAsync("test.py");
            Console.WriteLine($"脚本执行状态: {(scriptResult.Success ? "成功" : "失败"}");
            
            if (!string.IsNullOrEmpty(scriptResult.Output))
            {
                Console.WriteLine($"脚本输出: {scriptResult.Output}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            await pythonService.DisposeAsync();
            
            // 清理测试文件
            if (File.Exists("test.py"))
            {
                File.Delete("test.py");
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<PythonSettings>(options => {
            options.BasePath = Environment.CurrentDirectory;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDebug = false;
            options.EnableTracing = false;
            options.ModulePaths = new List<string> {
                Path.Combine(Environment.CurrentDirectory, "modules"),
                Path.Combine(Environment.CurrentDirectory, "scripts")
            };
        });
        
        services.AddSingleton<PythonService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

## 总结

上述示例展示了 IconPython 技能的主要功能和使用方法，包括：

1. **基本使用**：执行 Python 脚本、计算表达式、导入模块
2. **高级配置**：配置 IconPython 服务的各种选项
3. **性能优化**：运行性能测试，优化 Python 执行效率
4. **错误处理**：处理各种错误情况
5. **IconPython AOT 核心引擎**：使用 AOT 编译的高性能 Python 执行引擎

所有示例均遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

## IconPython AOT 架构总结

IconPython AOT 架构是一个完整的高性能 Python 执行解决方案，包含以下核心组件：

- **iconpython_aot.cs**：核心 IconPython AOT 引擎，实现了完整的 Python 执行功能
- **iconpython_aot.setting.json**：AOT 编译配置，包含依赖项和运行时选项
- **iconpython_aot.run.json**：运行环境配置，包含不同命令的启动设置

IconPython AOT 架构的主要优势：

1. **高性能**：基于 AOT 编译，启动速度快，运行性能高
2. **功能丰富**：支持 Python 脚本执行、表达式计算、REPL 环境等
3. **易于使用**：提供友好的命令行界面，支持命令别名
4. **可靠性高**：内置错误处理和日志记录
5. **可扩展性强**：模块化设计，便于扩展和维护

通过 IconPython AOT 架构，开发者可以快速构建高性能、可靠的 Python 执行应用，满足各种 Python 执行相关的开发需求。
