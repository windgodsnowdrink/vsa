# dotnetscript - 使用示例

## 快速开始

### 1. 基本脚本执行示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetScriptAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建主机和服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("dotnetscript_aot.setting.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // 配置 DotNetScript 选项
                    services.Configure<DotNetScript.AOT.DotNetScriptOptions>(context.Configuration.GetSection("DotNetScript"));
                    
                    // 注册 DotNetScript 服务
                    services.AddDotNetScript();
                })
                .Build();

            Console.WriteLine("DotNetScript 基本脚本执行示例");
            Console.WriteLine("=" * 50);
            
            // 获取 DotNetScript 服务实例
            var scriptService = host.Services.GetRequiredService<DotNetScript.AOT.IDotNetScriptService>();
            
            // 1. 执行内联脚本
            Console.WriteLine("\n1. 执行内联脚本");
            string inlineScript = @"Console.WriteLine(\"Hello, DotNetScript AOT!\");
return 42;";
            
            var inlineResult = await scriptService.ExecuteScriptAsync(inlineScript, null, 5000);
            if (inlineResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {inlineResult.ExecutionTimeMs}ms)");
                if (!string.IsNullOrEmpty(inlineResult.Output))
                {
                    Console.WriteLine($"输出: {inlineResult.Output}");
                }
                if (inlineResult.Results.Any())
                {
                    Console.WriteLine($"返回值: {inlineResult.Results[0]}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {inlineResult.ErrorMessage}");
            }
            
            // 2. 执行复杂脚本
            Console.WriteLine("\n2. 执行复杂脚本");
            string complexScript = @"// 计算斐波那契数列
int Fibonacci(int n)
{
    if (n <= 1)
        return n;
    return Fibonacci(n - 1) + Fibonacci(n - 2);
}

Console.WriteLine(\"计算斐波那契数列前 10 项:\");
for (int i = 0; i < 10; i++)
{
    Console.Write($\"{Fibonacci(i)}, \";
}
Console.WriteLine();
return Fibonacci(10);";
            
            var complexResult = await scriptService.ExecuteScriptAsync(complexScript, null, 10000);
            if (complexResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {complexResult.ExecutionTimeMs}ms)");
                if (!string.IsNullOrEmpty(complexResult.Output))
                {
                    Console.WriteLine($"输出:\n{complexResult.Output}");
                }
                if (complexResult.Results.Any())
                {
                    Console.WriteLine($"第 10 项斐波那契数: {complexResult.Results[0]}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {complexResult.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 2. 脚本编译示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetScriptAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DotNetScript 脚本编译示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 配置 DotNetScript 选项
                    services.Configure<DotNetScript.AOT.DotNetScriptOptions>(options => {
                        options.DefaultTimeoutMs = 5000;
                        options.EnableDetailedLogging = true;
                    });
                    
                    // 注册 DotNetScript 服务
                    services.AddDotNetScript();
                })
                .Build();
            
            // 获取 DotNetScript 服务实例
            var scriptService = host.Services.GetRequiredService<DotNetScript.AOT.IDotNetScriptService>();
            
            // 1. 编译有效的脚本
            Console.WriteLine("\n1. 编译有效的脚本");
            string validScript = @"Console.WriteLine(\"This is a valid script\");
return 1;";
            
            var validResult = await scriptService.CompileScriptAsync(validScript);
            if (validResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {validResult.ExecutionTimeMs}ms)");
                foreach (var result in validResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {validResult.ErrorMessage}");
            }
            
            // 2. 编译无效的脚本（语法错误）
            Console.WriteLine("\n2. 编译无效的脚本（语法错误）");
            string invalidScript = @"Console.WriteLine(\"This is an invalid script\");
return ; // 缺少返回值";
            
            var invalidResult = await scriptService.CompileScriptAsync(invalidScript);
            if (invalidResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {invalidResult.ExecutionTimeMs}ms)");
                foreach (var result in invalidResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败（预期行为）(耗时: {invalidResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"错误信息: {invalidResult.ErrorMessage}");
            }
            
            // 3. 编译带有警告的脚本
            Console.WriteLine("\n3. 编译带有警告的脚本");
            string warningScript = @"var x = 10; // 变量 x 未使用
Console.WriteLine(\"Script with warning\");";
            
            var warningResult = await scriptService.CompileScriptAsync(warningScript);
            if (warningResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {warningResult.ExecutionTimeMs}ms)");
                foreach (var result in warningResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {warningResult.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 3. 脚本文件管理示例

```csharp
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetScriptAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DotNetScript 脚本文件管理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 注册 DotNetScript 服务
                    services.AddDotNetScript();
                })
                .Build();
            
            // 获取 DotNetScript 服务实例
            var scriptService = host.Services.GetRequiredService<DotNetScript.AOT.IDotNetScriptService>();
            
            // 1. 创建临时脚本文件用于测试
            string tempDir = Path.Combine(Path.GetTempPath(), "dotnetscript-test");
            Directory.CreateDirectory(tempDir);
            
            // 创建测试脚本文件
            string script1Path = Path.Combine(tempDir, "script1.csx");
            await File.WriteAllTextAsync(script1Path, @"Console.WriteLine(\"Script 1\"); return 1;");
            
            string script2Path = Path.Combine(tempDir, "script2.csx");
            await File.WriteAllTextAsync(script2Path, @"Console.WriteLine(\"Script 2\"); return 2;");
            
            // 创建子目录和脚本
            string subDir = Path.Combine(tempDir, "sub");
            Directory.CreateDirectory(subDir);
            
            string script3Path = Path.Combine(subDir, "script3.csx");
            await File.WriteAllTextAsync(script3Path, @"Console.WriteLine(\"Script 3\"); return 3;");
            
            // 2. 列出脚本文件（非递归）
            Console.WriteLine($"\n2. 列出脚本文件（非递归）: {tempDir}");
            var listResult = await scriptService.ListScriptsAsync(tempDir, false);
            if (listResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {listResult.ExecutionTimeMs}ms)");
                foreach (var result in listResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {listResult.ErrorMessage}");
            }
            
            // 3. 列出脚本文件（递归）
            Console.WriteLine($"\n3. 列出脚本文件（递归）: {tempDir}");
            var recursiveResult = await scriptService.ListScriptsAsync(tempDir, true);
            if (recursiveResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {recursiveResult.ExecutionTimeMs}ms)");
                foreach (var result in recursiveResult.Results)
                {
                    Console.WriteLine($"  {result}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {recursiveResult.ErrorMessage}");
            }
            
            // 4. 执行脚本文件
            Console.WriteLine($"\n4. 执行脚本文件: {script1Path}");
            var fileResult = await scriptService.ExecuteScriptFromFileAsync(script1Path, 5000);
            if (fileResult.Success)
            {
                Console.WriteLine($"成功 (耗时: {fileResult.ExecutionTimeMs}ms)");
                if (!string.IsNullOrEmpty(fileResult.Output))
                {
                    Console.WriteLine($"输出: {fileResult.Output}");
                }
                if (fileResult.Results.Any())
                {
                    Console.WriteLine($"返回值: {fileResult.Results[0]}");
                }
            }
            else
            {
                Console.WriteLine($"失败: {fileResult.ErrorMessage}");
            }
            
            // 清理临时文件
            Directory.Delete(tempDir, true);
            
            await host.RunAsync();
        }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetScriptAotExample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("DotNetScript 错误处理示例");
            Console.WriteLine("=" * 50);
            
            // 构建服务容器
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // 配置 DotNetScript 选项
                    services.Configure<DotNetScript.AOT.DotNetScriptOptions>(options => {
                        options.DefaultTimeoutMs = 2000; // 短超时，便于测试
                    });
                    
                    // 注册 DotNetScript 服务
                    services.AddDotNetScript();
                })
                .Build();
            
            // 获取 DotNetScript 服务实例
            var scriptService = host.Services.GetRequiredService<DotNetScript.AOT.IDotNetScriptService>();
            
            // 1. 测试超时
            Console.WriteLine("\n1. 测试脚本执行超时");
            string timeoutScript = @"System.Threading.Thread.Sleep(3000); // 超过 2000ms 超时设置
Console.WriteLine(\"This script will timeout");";
            
            var timeoutResult = await scriptService.ExecuteScriptAsync(timeoutScript);
            if (!timeoutResult.Success)
            {
                Console.WriteLine($"预期超时失败 (耗时: {timeoutResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"错误信息: {timeoutResult.ErrorMessage}");
            }
            
            // 2. 测试语法错误
            Console.WriteLine("\n2. 测试语法错误");
            string syntaxErrorScript = @"Console.WriteLine(\"Missing closing quote; // 缺少引号");";
            
            var syntaxResult = await scriptService.ExecuteScriptAsync(syntaxErrorScript);
            if (!syntaxResult.Success)
            {
                Console.WriteLine($"预期语法错误失败 (耗时: {syntaxResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"错误信息: {syntaxResult.ErrorMessage}");
            }
            
            // 3. 测试运行时错误
            Console.WriteLine("\n3. 测试运行时错误");
            string runtimeErrorScript = @"var x = 0;
var y = 10 / x; // 除以零错误
Console.WriteLine($\"Result: {y}\");";
            
            var runtimeResult = await scriptService.ExecuteScriptAsync(runtimeErrorScript);
            if (!runtimeResult.Success)
            {
                Console.WriteLine($"预期运行时错误失败 (耗时: {runtimeResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"错误信息: {runtimeResult.ErrorMessage}");
            }
            
            // 4. 测试文件不存在
            Console.WriteLine("\n4. 测试执行不存在的文件");
            var fileNotFoundResult = await scriptService.ExecuteScriptFromFileAsync("non_existent_script.csx");
            if (!fileNotFoundResult.Success)
            {
                Console.WriteLine($"预期文件不存在失败 (耗时: {fileNotFoundResult.ExecutionTimeMs}ms)");
                Console.WriteLine($"错误信息: {fileNotFoundResult.ErrorMessage}");
            }
            
            await host.RunAsync();
        }
    }
}
```

### 5. 命令行工具示例

```bash
# 查看帮助信息
dotnetscript_aot.exe help
dotnetscript_aot.exe --help
dotnetscript_aot.exe -h

# 执行内联脚本
dotnetscript_aot.exe run "Console.WriteLine(\"Hello from CLI\"); return 42"

# 执行脚本文件
dotnetscript_aot.exe run script.csx

# 执行脚本文件并设置超时时间
dotnetscript_aot.exe run script.csx --timeout 10000

# 编译脚本
dotnetscript_aot.exe compile script.csx

# 列出当前目录下的脚本文件
dotnetscript_aot.exe list

# 递归列出目录下的所有脚本文件
dotnetscript_aot.exe list scripts/ -r
dotnetscript_aot.exe list scripts/ --recursive

# 清理编译缓存
dotnetscript_aot.exe clean

# 查看版本信息
dotnetscript_aot.exe version

# 执行复杂的内联脚本
dotnetscript_aot.exe run @"var x = 10; var y = 20; Console.WriteLine($\"Sum: {x + y}\"); return x + y;"
```

## 总结

以上示例展示了 DotNetScript 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本脚本执行操作
2. 学习脚本编译和语法检查
3. 管理脚本文件和目录
4. 处理各种错误情况
5. 使用命令行工具进行便捷操作

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。所有示例都支持 .NET 10 AOT 编译，提供极致的性能表现。
