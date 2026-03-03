# Linkers 技能使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 分析程序集
            Console.WriteLine("1. 分析程序集...");
            await linkersService.AnalyzeAssemblyAsync("SampleAssembly.dll");
            
            // 解析符号
            Console.WriteLine("\n2. 解析符号...");
            await linkersService.ResolveSymbolAsync("SampleAssembly.dll", "SampleClass");
            
            // 列出依赖项
            Console.WriteLine("\n3. 列出依赖项...");
            await linkersService.ListDependenciesAsync("SampleAssembly.dll");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 2. 程序集优化示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 程序集优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 优化程序集
            Console.WriteLine("优化程序集...");
            Console.WriteLine("输入: SampleAssembly.dll");
            Console.WriteLine("输出: OptimizedAssembly.dll");
            Console.WriteLine("优化级别: medium");
            
            await linkersService.OptimizeAssemblyAsync(
                "SampleAssembly.dll", 
                "OptimizedAssembly.dll", 
                "medium"
            );
            
            Console.WriteLine("\n优化完成！");
            
            // 验证优化后的程序集
            Console.WriteLine("\n验证优化后的程序集...");
            await linkersService.VerifyAssemblyAsync("OptimizedAssembly.dll");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 3. 程序集内容提取示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 程序集内容提取示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 提取程序集内容
            Console.WriteLine("提取程序集内容...");
            Console.WriteLine("程序集: SampleAssembly.dll");
            Console.WriteLine("输出目录: ExtractedContent");
            
            await linkersService.ExtractAssemblyAsync(
                "SampleAssembly.dll", 
                "ExtractedContent"
            );
            
            Console.WriteLine("\n提取完成！");
            Console.WriteLine("内容已提取到: ExtractedContent 目录");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 4. 原生库绑定生成示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 原生库绑定生成示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 生成原生库绑定
            Console.WriteLine("生成原生库绑定...");
            Console.WriteLine("原生库: native-lib.dll");
            Console.WriteLine("输出目录: Bindings");
            
            await linkersService.GenerateBindingsAsync(
                "native-lib.dll", 
                "Bindings"
            );
            
            Console.WriteLine("\n绑定生成完成！");
            Console.WriteLine("绑定代码已生成到: Bindings 目录");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 5. 性能基准测试示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 性能基准测试示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 运行性能基准测试
            Console.WriteLine("运行性能基准测试...");
            Console.WriteLine("测试程序集: SampleAssembly.dll");
            Console.WriteLine("运行次数: 10");
            
            await linkersService.RunBenchmarkAsync(
                "SampleAssembly.dll", 
                10
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 命令行使用示例

### 1. 分析程序集

```bash
# 分析程序集
linkers_aot.exe analyze SampleAssembly.dll

# 或使用别名
linkers_aot.exe a SampleAssembly.dll
```

### 2. 解析符号

```bash
# 解析符号
linkers_aot.exe resolve SampleAssembly.dll SampleClass

# 或使用别名
linkers_aot.exe r SampleAssembly.dll SampleClass
```

### 3. 列出依赖项

```bash
# 列出依赖项
linkers_aot.exe list-dependencies SampleAssembly.dll

# 或使用别名
linkers_aot.exe ld SampleAssembly.dll
```

### 4. 优化程序集

```bash
# 优化程序集（中级优化）
linkers_aot.exe optimize SampleAssembly.dll OptimizedAssembly.dll medium

# 或使用别名
linkers_aot.exe o SampleAssembly.dll OptimizedAssembly.dll medium
```

### 5. 验证程序集

```bash
# 验证程序集
linkers_aot.exe verify SampleAssembly.dll

# 或使用别名
linkers_aot.exe v SampleAssembly.dll
```

### 6. 提取程序集内容

```bash
# 提取程序集内容
linkers_aot.exe extract SampleAssembly.dll ExtractedContent

# 或使用别名
linkers_aot.exe e SampleAssembly.dll ExtractedContent
```

### 7. 生成原生库绑定

```bash
# 生成原生库绑定
linkers_aot.exe generate-bindings native-lib.dll Bindings

# 或使用别名
linkers_aot.exe gb native-lib.dll Bindings
```

### 8. 运行性能基准测试

```bash
# 运行性能基准测试（10次迭代）
linkers_aot.exe benchmark SampleAssembly.dll 10

# 或使用别名
linkers_aot.exe bm SampleAssembly.dll 10
```

### 9. 显示帮助信息

```bash
# 显示帮助信息
linkers_aot.exe help

# 或使用别名
linkers_aot.exe h
```

## 编程集成示例

### 与 ASP.NET Core 集成

```csharp
// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

var builder = WebApplication.CreateBuilder(args);

// 注册 Linkers 服务
builder.Services.AddSingleton<LinkersService>();

var app = builder.Build();

app.MapGet("/analyze", async (LinkersService linkersService, string assembly) => {
    await linkersService.AnalyzeAssemblyAsync(assembly);
    return Results.Ok("程序集分析完成");
});

app.MapGet("/optimize", async (LinkersService linkersService, string input, string output, string level) => {
    await linkersService.OptimizeAssemblyAsync(input, output, level);
    return Results.Ok("程序集优化完成");
});

app.Run();
```

### 与 Blazor 集成

```csharp
// Program.cs (Blazor Server)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 注册 Linkers 服务
builder.Services.AddSingleton<LinkersService>();

var app = builder.Build();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();

// LinkersComponent.razor
@page "/linkers"
@inject LinkersService LinkersService

<h3>Linkers 工具</h3>

<div class="form-group">
    <label>程序集路径</label>
    <input @bind="assemblyPath" class="form-control" />
</div>

<button @onclick="AnalyzeAssembly" class="btn btn-primary">分析程序集</button>
<button @onclick="ListDependencies" class="btn btn-secondary">列出依赖项</button>

@code {
    private string assemblyPath = "SampleAssembly.dll";
    
    private async Task AnalyzeAssembly() {
        await LinkersService.AnalyzeAssemblyAsync(assemblyPath);
    }
    
    private async Task ListDependencies() {
        await LinkersService.ListDependenciesAsync(assemblyPath);
    }
}
```

### 与控制台应用集成

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 控制台集成示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n请选择操作:");
            Console.WriteLine("1. 分析程序集");
            Console.WriteLine("2. 解析符号");
            Console.WriteLine("3. 列出依赖项");
            Console.WriteLine("4. 优化程序集");
            Console.WriteLine("5. 验证程序集");
            Console.WriteLine("6. 提取程序集内容");
            Console.WriteLine("7. 生成原生库绑定");
            Console.WriteLine("8. 运行性能测试");
            Console.WriteLine("9. 退出");
            Console.Write("请输入选项: ");
            var choice = Console.ReadLine();
            
            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("请输入程序集路径: ");
                        var assembly = Console.ReadLine();
                        await linkersService.AnalyzeAssemblyAsync(assembly);
                        break;
                    
                    case "2":
                        Console.Write("请输入程序集路径: ");
                        var resolveAssembly = Console.ReadLine();
                        Console.Write("请输入符号名称: ");
                        var symbol = Console.ReadLine();
                        await linkersService.ResolveSymbolAsync(resolveAssembly, symbol);
                        break;
                    
                    case "3":
                        Console.Write("请输入程序集路径: ");
                        var depAssembly = Console.ReadLine();
                        await linkersService.ListDependenciesAsync(depAssembly);
                        break;
                    
                    case "4":
                        Console.Write("请输入输入程序集路径: ");
                        var inputAssembly = Console.ReadLine();
                        Console.Write("请输入输出程序集路径: ");
                        var outputAssembly = Console.ReadLine();
                        Console.Write("请输入优化级别 (low/medium/high): ");
                        var level = Console.ReadLine();
                        await linkersService.OptimizeAssemblyAsync(inputAssembly, outputAssembly, level);
                        break;
                    
                    case "5":
                        Console.Write("请输入程序集路径: ");
                        var verifyAssembly = Console.ReadLine();
                        await linkersService.VerifyAssemblyAsync(verifyAssembly);
                        break;
                    
                    case "6":
                        Console.Write("请输入程序集路径: ");
                        var extractAssembly = Console.ReadLine();
                        Console.Write("请输入输出目录: ");
                        var extractDir = Console.ReadLine();
                        await linkersService.ExtractAssemblyAsync(extractAssembly, extractDir);
                        break;
                    
                    case "7":
                        Console.Write("请输入原生库路径: ");
                        var nativeLib = Console.ReadLine();
                        Console.Write("请输入输出目录: ");
                        var bindingsDir = Console.ReadLine();
                        await linkersService.GenerateBindingsAsync(nativeLib, bindingsDir);
                        break;
                    
                    case "8":
                        Console.Write("请输入程序集路径: ");
                        var benchAssembly = Console.ReadLine();
                        Console.Write("请输入运行次数: ");
                        int.TryParse(Console.ReadLine(), out var iterations);
                        await linkersService.RunBenchmarkAsync(benchAssembly, iterations);
                        break;
                    
                    case "9":
                        exit = true;
                        break;
                    
                    default:
                        Console.WriteLine("无效选项，请重试。");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }
    }
}
```

## 使用场景示例

### 1. 程序集分析工具

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class AssemblyAnalyzer
{
    private readonly LinkersService _linkersService;
    
    public AssemblyAnalyzer(LinkersService linkersService)
    {
        _linkersService = linkersService;
    }
    
    public async Task AnalyzeAssembly(string assemblyPath)
    {
        Console.WriteLine($"分析程序集: {assemblyPath}");
        await _linkersService.AnalyzeAssemblyAsync(assemblyPath);
    }
    
    public async Task AnalyzeDirectory(string directoryPath)
    {
        Console.WriteLine($"分析目录中的所有程序集: {directoryPath}");
        
        var assemblyFiles = Directory.GetFiles(directoryPath, "*.dll", SearchOption.AllDirectories);
        foreach (var assemblyFile in assemblyFiles)
        {
            try
            {
                await AnalyzeAssembly(assemblyFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"分析 {assemblyFile} 时出错: {ex.Message}");
            }
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        var analyzer = new AssemblyAnalyzer(linkersService);
        
        // 分析单个程序集
        await analyzer.AnalyzeAssembly("SampleAssembly.dll");
        
        // 分析目录中的所有程序集
        await analyzer.AnalyzeDirectory("path/to/assemblies");
    }
}
```

### 2. 程序集优化工具

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class AssemblyOptimizer
{
    private readonly LinkersService _linkersService;
    
    public AssemblyOptimizer(LinkersService linkersService)
    {
        _linkersService = linkersService;
    }
    
    public async Task OptimizeAssemblies(string inputDirectory, string outputDirectory, string optimizationLevel)
    {
        Console.WriteLine($"优化目录中的程序集: {inputDirectory}");
        Console.WriteLine($"输出目录: {outputDirectory}");
        Console.WriteLine($"优化级别: {optimizationLevel}");
        
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        
        var assemblyFiles = Directory.GetFiles(inputDirectory, "*.dll", SearchOption.AllDirectories);
        
        foreach (var assemblyFile in assemblyFiles)
        {
            try
            {
                var outputFile = Path.Combine(outputDirectory, Path.GetFileName(assemblyFile));
                Console.WriteLine($"\n优化: {Path.GetFileName(assemblyFile)}");
                await _linkersService.OptimizeAssemblyAsync(assemblyFile, outputFile, optimizationLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"优化 {assemblyFile} 时出错: {ex.Message}");
            }
        }
        
        Console.WriteLine($"\n优化完成！处理了 {assemblyFiles.Length} 个程序集");
    }
}

public class Program
{
    public static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        var optimizer = new AssemblyOptimizer(linkersService);
        
        // 优化目录中的所有程序集
        await optimizer.OptimizeAssemblies(
            "path/to/input/assemblies",
            "path/to/output/optimized",
            "medium"
        );
    }
}
```

### 3. 依赖项分析工具

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class DependencyAnalyzer
{
    private readonly LinkersService _linkersService;
    private readonly Dictionary<string, List<string>> _dependencyGraph = new();
    
    public DependencyAnalyzer(LinkersService linkersService)
    {
        _linkersService = linkersService;
    }
    
    public async Task BuildDependencyGraph(string rootAssemblyPath)
    {
        Console.WriteLine($"构建依赖项图: {rootAssemblyPath}");
        await BuildGraphRecursive(rootAssemblyPath, new HashSet<string>());
        DisplayDependencyGraph();
    }
    
    private async Task BuildGraphRecursive(string assemblyPath, HashSet<string> visited)
    {
        if (visited.Contains(assemblyPath))
            return;
        
        visited.Add(assemblyPath);
        
        try
        {
            // 获取依赖项
            // 注意：这里需要实现获取依赖项的逻辑
            var dependencies = await GetDependencies(assemblyPath);
            _dependencyGraph[assemblyPath] = dependencies;
            
            foreach (var dependency in dependencies)
            {
                await BuildGraphRecursive(dependency, visited);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"分析 {assemblyPath} 时出错: {ex.Message}");
            _dependencyGraph[assemblyPath] = new List<string>();
        }
    }
    
    private async Task<List<string>> GetDependencies(string assemblyPath)
    {
        // 这里需要实现获取程序集依赖项的逻辑
        // 可以使用 LinkersService 的功能
        var dependencies = new List<string>();
        // 实现获取依赖项的代码
        return dependencies;
    }
    
    private void DisplayDependencyGraph()
    {
        Console.WriteLine("\n依赖项图:");
        foreach (var entry in _dependencyGraph)
        {
            Console.WriteLine($"{Path.GetFileName(entry.Key)} 依赖:");
            foreach (var dependency in entry.Value)
            {
                Console.WriteLine($"  - {Path.GetFileName(dependency)}");
            }
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        var analyzer = new DependencyAnalyzer(linkersService);
        
        // 构建依赖项图
        await analyzer.BuildDependencyGraph("SampleAssembly.dll");
    }
}
```

## 性能优化示例

### 1. 内存优化

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 内存优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器，配置内存优化
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache(options => {
                options.SizeLimit = 512; // 设置缓存大小限制
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
            })
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 处理多个程序集，使用内存优化
            var assemblyFiles = Directory.GetFiles("assemblies", "*.dll");
            
            foreach (var assemblyFile in assemblyFiles)
            {
                Console.WriteLine($"处理: {Path.GetFileName(assemblyFile)}");
                await linkersService.AnalyzeAssemblyAsync(assemblyFile);
                
                // 强制垃圾回收，释放内存
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 2. 并行处理优化

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 并行处理优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 获取要处理的程序集
            var assemblyFiles = Directory.GetFiles("assemblies", "*.dll");
            Console.WriteLine($"找到 {assemblyFiles.Length} 个程序集，开始并行处理...");
            
            // 并行处理多个程序集
            var tasks = new List<Task>();
            foreach (var assemblyFile in assemblyFiles)
            {
                tasks.Add(Task.Run(async () => {
                    try
                    {
                        Console.WriteLine($"开始分析: {Path.GetFileName(assemblyFile)}");
                        await linkersService.AnalyzeAssemblyAsync(assemblyFile);
                        Console.WriteLine($"完成分析: {Path.GetFileName(assemblyFile)}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"分析 {assemblyFile} 时出错: {ex.Message}");
                    }
                }));
            }
            
            // 等待所有任务完成
            await Task.WhenAll(tasks);
            Console.WriteLine($"\n所有程序集分析完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 3. 批处理优化

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 批处理优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 批处理程序集
            var assemblyFiles = Directory.GetFiles("assemblies", "*.dll");
            const int batchSize = 5;
            
            for (int i = 0; i < assemblyFiles.Length; i += batchSize)
            {
                var batch = assemblyFiles.Skip(i).Take(batchSize).ToList();
                Console.WriteLine($"\n处理批次 {i/batchSize + 1}: {batch.Count} 个程序集");
                
                var tasks = new List<Task>();
                foreach (var assemblyFile in batch)
                {
                    tasks.Add(Task.Run(async () => {
                        try
                        {
                            await linkersService.AnalyzeAssemblyAsync(assemblyFile);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"处理 {assemblyFile} 时出错: {ex.Message}");
                        }
                    }));
                }
                
                await Task.WhenAll(tasks);
            }
            
            Console.WriteLine($"\n批处理完成！共处理 {assemblyFiles.Length} 个程序集");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 错误处理示例

### 1. 异常处理

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        try
        {
            // 尝试分析不存在的程序集
            Console.WriteLine("尝试分析不存在的程序集...");
            await linkersService.AnalyzeAssemblyAsync("non_existent.dll");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine("建议: 检查文件路径是否正确");
        }
        
        try
        {
            // 尝试处理无效的程序集
            Console.WriteLine("\n尝试处理无效的程序集...");
            await linkersService.AnalyzeAssemblyAsync("invalid.dll");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine("建议: 确保提供有效的.NET程序集");
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
}
```

### 2. 重试机制

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LinkersAot;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Linkers 重试机制示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LinkersService>()
            .BuildServiceProvider();
        
        var linkersService = serviceProvider.GetRequiredService<LinkersService>();
        
        // 带重试机制的操作
        await RetryOperationAsync(async () => {
            Console.WriteLine("尝试分析程序集...");
            await linkersService.AnalyzeAssemblyAsync("SampleAssembly.dll");
        }, maxRetries: 3, delayMs: 1000);
    }
    
    private static async Task RetryOperationAsync(Func<Task> operation, int maxRetries = 3, int delayMs = 1000)
    {
        int attempts = 0;
        while (true)
        {
            try
            {
                attempts++;
                await operation();
                Console.WriteLine($"操作成功 (尝试 {attempts}/{maxRetries})");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"尝试 {attempts}/{maxRetries} 失败: {ex.Message}");
                if (attempts >= maxRetries)
                {
                    Console.WriteLine("达到最大重试次数，操作失败");
                    throw;
                }
                Console.WriteLine($"等待 {delayMs}ms 后重试...");
                await Task.Delay(delayMs);
                delayMs *= 2; // 指数退避
            }
        }
    }
}
```

## 总结

本文档提供了 Linkers 技能的详细使用示例，包括：

1. **基本使用**：快速上手 Linkers 的核心功能
2. **高级配置**：详细的配置选项和示例
3. **命令行使用**：通过命令行使用 Linkers 功能
4. **编程集成**：与 ASP.NET Core、Blazor 和控制台应用集成
5. **使用场景**：程序集分析、优化、依赖项管理等实际应用场景
6. **性能优化**：内存优化、并行处理、批处理等性能提升技巧
7. **错误处理**：异常处理和重试机制

通过这些示例，您可以快速掌握 Linkers 技能的使用方法，并根据自己的需求进行扩展和定制。系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。