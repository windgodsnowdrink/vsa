#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Runtime.InteropServices@4.3.0
#:package System.Text.Json@8.0.0
#:package System.Text.RegularExpressions@4.3.1
#:package System.Collections.Immutable@8.0.0
#:package IronPython@3.4.1
#:package Microsoft.Scripting@1.3.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IconPython AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var pythonService = serviceProvider.GetRequiredService<PythonService>();
        var settings = serviceProvider.GetRequiredService<IOptions<PythonSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "run":
                case "r":
                    await RunScript(pythonService, arguments);
                    break;
                case "eval":
                case "e":
                    await EvaluateExpression(pythonService, arguments);
                    break;
                case "repl":
                case "re":
                    await StartREPL(pythonService, arguments);
                    break;
                case "benchmark":
                case "b":
                    await RunBenchmark(pythonService, arguments);
                    break;
                case "module":
                case "m":
                    await RunModule(pythonService, arguments);
                    break;
                case "import":
                case "i":
                    await ImportModule(pythonService, arguments);
                    break;
                case "config":
                case "co":
                    ShowConfig(settings);
                    break;
                case "help":
                case "h":
                case "?":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine($"未知命令: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            await pythonService.DisposeAsync();
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
            options.GlobalVariables = new Dictionary<string, object> {
                { "__name__", "__main__" },
                { "__file__", "iconpython_aot.cs" }
            };
        });
        
        services.AddSingleton<PythonService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task RunScript(PythonService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供脚本文件路径");
            return;
        }
        
        var scriptPath = arguments[0];
        Console.WriteLine($"运行 Python 脚本: {scriptPath}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.RunScriptAsync(scriptPath);
        stopwatch.Stop();
        
        Console.WriteLine($"脚本执行完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"执行状态: {(result.Success ? "成功" : "失败"}");
        
        if (!string.IsNullOrEmpty(result.Output))
        {
            Console.WriteLine($"\n输出:");
            Console.WriteLine(result.Output);
        }
        
        if (!string.IsNullOrEmpty(result.Error))
        {
            Console.WriteLine($"\n错误:");
            Console.WriteLine(result.Error);
        }
    }
    
    private static async Task EvaluateExpression(PythonService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供 Python 表达式");
            return;
        }
        
        var expression = string.Join(" ", arguments);
        Console.WriteLine($"计算 Python 表达式: {expression}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.EvaluateExpressionAsync(expression);
        stopwatch.Stop();
        
        Console.WriteLine($"表达式计算完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"计算结果: {result}");
    }
    
    private static async Task StartREPL(PythonService service, string[] arguments)
    {
        Console.WriteLine("启动 Python REPL (按 Ctrl+C 退出)");
        Console.WriteLine("=" * 60);
        
        while (true)
        {
            Console.Write(">>> ");
            var input = Console.ReadLine();
            
            if (input == null || input.Trim() == "exit()" || input.Trim() == "quit()")
            {
                break;
            }
            
            try
            {
                var result = await service.EvaluateExpressionAsync(input);
                if (result != null)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }
        
        Console.WriteLine("\nREPL 已退出");
    }
    
    private static async Task RunBenchmark(PythonService service, string[] arguments)
    {
        var iterations = arguments.Length > 0 ? int.Parse(arguments[0]) : 1000;
        var expression = arguments.Length > 1 ? string.Join(" ", arguments.Skip(1)) : "1 + 1";
        
        Console.WriteLine($"运行基准测试: {expression}");
        Console.WriteLine($"迭代次数: {iterations}");
        
        var stopwatch = Stopwatch.StartNew();
        var successes = 0;
        var failures = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                await service.EvaluateExpressionAsync(expression);
                successes++;
            }
            catch
            {
                failures++;
            }
        }
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var operationsPerSecond = iterations / elapsedSeconds;
        
        Console.WriteLine($"基准测试完成!");
        Console.WriteLine($"总用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"成功: {successes}");
        Console.WriteLine($"失败: {failures}");
        Console.WriteLine($"每秒操作数: {operationsPerSecond:F2} ops/s");
    }
    
    private static async Task RunModule(PythonService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供模块名");
            return;
        }
        
        var moduleName = arguments[0];
        Console.WriteLine($"运行 Python 模块: {moduleName}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.RunModuleAsync(moduleName);
        stopwatch.Stop();
        
        Console.WriteLine($"模块执行完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"执行状态: {(result.Success ? "成功" : "失败"}");
        
        if (!string.IsNullOrEmpty(result.Output))
        {
            Console.WriteLine($"\n输出:");
            Console.WriteLine(result.Output);
        }
        
        if (!string.IsNullOrEmpty(result.Error))
        {
            Console.WriteLine($"\n错误:");
            Console.WriteLine(result.Error);
        }
    }
    
    private static async Task ImportModule(PythonService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("错误: 请提供模块名");
            return;
        }
        
        var moduleName = arguments[0];
        Console.WriteLine($"导入 Python 模块: {moduleName}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ImportModuleAsync(moduleName);
        stopwatch.Stop();
        
        Console.WriteLine($"模块导入完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"导入状态: {(result != null ? "成功" : "失败"}");
        
        if (result != null)
        {
            Console.WriteLine($"\n模块信息:");
            Console.WriteLine($"模块名: {moduleName}");
            Console.WriteLine($"模块类型: {result.GetType().Name}");
        }
    }
    
    private static void ShowConfig(PythonSettings settings)
    {
        Console.WriteLine("Python 配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"基础路径: {settings.BasePath}");
        Console.WriteLine($"超时: {settings.Timeout}");
        Console.WriteLine($"启用调试: {settings.EnableDebug}");
        Console.WriteLine($"启用跟踪: {settings.EnableTracing}");
        Console.WriteLine($"模块路径:");
        foreach (var path in settings.ModulePaths)
        {
            Console.WriteLine($"  - {path}