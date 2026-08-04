# LibVLCSharp 技能使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            // 播放媒体文件（10秒）
            Console.WriteLine("1. 播放媒体文件...");
            await vlcService.PlayMediaAsync("sample.mp4", 10);
            
            // 获取媒体信息
            Console.WriteLine("\n2. 获取媒体信息...");
            await vlcService.GetMediaInfoAsync("sample.mp4");
            
            // 列出支持的编解码器
            Console.WriteLine("\n3. 列出支持的编解码器...");
            await vlcService.ListCodecsAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "操作失败");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

### 2. 媒体录制示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 媒体录制示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 录制媒体文件（30秒）
            Console.WriteLine("开始录制媒体文件...");
            Console.WriteLine("输入文件: sample.mp4");
            Console.WriteLine("输出文件: recorded_output.mp4");
            Console.WriteLine("录制时长: 30秒");
            
            await vlcService.RecordMediaAsync("sample.mp4", "recorded_output.mp4", 30);
            
            Console.WriteLine("\n录制完成！");
            Console.WriteLine("文件已保存为: recorded_output.mp4");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"录制失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

### 3. 媒体格式转换示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 媒体格式转换示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 转换媒体格式
            Console.WriteLine("开始转换媒体格式...");
            Console.WriteLine("输入文件: sample.mp4");
            Console.WriteLine("输出文件: converted_output.mp4");
            Console.WriteLine("输出格式: mp4");
            
            await vlcService.ConvertMediaAsync("sample.mp4", "converted_output.mp4", "mp4");
            
            Console.WriteLine("\n转换完成！");
            Console.WriteLine("文件已保存为: converted_output.mp4");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"转换失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

### 4. 流媒体示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 流媒体示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 启动流媒体服务
            Console.WriteLine("开始流媒体服务...");
            Console.WriteLine("媒体文件: sample.mp4");
            Console.WriteLine("流媒体地址: :8080");
            Console.WriteLine("流媒体时长: 60秒");
            
            await vlcService.StreamMediaAsync("sample.mp4", ":8080", 60);
            
            Console.WriteLine("\n流媒体服务已停止！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"流媒体服务失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

### 5. 性能基准测试示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 性能基准测试示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 运行性能基准测试
            Console.WriteLine("运行性能基准测试...");
            Console.WriteLine("测试媒体: sample.mp4");
            Console.WriteLine("运行次数: 10");
            
            await vlcService.RunBenchmarkAsync("sample.mp4", 10);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"测试失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

### 6. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 设置环境变量
        Environment.SetEnvironmentVariable("LIBVLCSharp_VIDEO_CACHE_SIZE", "5000");
        Environment.SetEnvironmentVariable("LIBVLCSharp_AUDIO_CACHE_SIZE", "3000");
        Environment.SetEnvironmentVariable("LIBVLCSharp_NETWORK_CACHE_SIZE", "2000");
        Environment.SetEnvironmentVariable("LIBVLCSharp_LOG_LEVEL", "DEBUG");
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => 
                builder.AddConsole()
                       .SetMinimumLevel(LogLevel.Debug)
            )
            .AddMemoryCache()
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 播放媒体文件
            Console.WriteLine("使用高级配置播放媒体文件...");
            await vlcService.PlayMediaAsync("sample.mp4", 20);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

## 命令行使用示例

### 1. 播放媒体文件

```bash
# 播放媒体文件（60秒）
libvlcsharp_aot.exe play sample.mp4 60

# 或使用别名
libvlcsharp_aot.exe p sample.mp4 60
```

### 2. 录制媒体文件

```bash
# 录制媒体文件（30秒）
libvlcsharp_aot.exe record sample.mp4 output.mp4 30

# 或使用别名
libvlcsharp_aot.exe r sample.mp4 output.mp4 30
```

### 3. 流媒体播放

```bash
# 启动流媒体服务（120秒）
libvlcsharp_aot.exe stream sample.mp4 :8080 120

# 或使用别名
libvlcsharp_aot.exe s sample.mp4 :8080 120
```

### 4. 转换媒体格式

```bash
# 转换媒体格式为 mp4
libvlcsharp_aot.exe convert input.avi output.mp4 mp4

# 或使用别名
libvlcsharp_aot.exe c input.avi output.mp4 mp4
```

### 5. 获取媒体信息

```bash
# 获取媒体信息
libvlcsharp_aot.exe info sample.mp4

# 或使用别名
libvlcsharp_aot.exe i sample.mp4
```

### 6. 列出支持的编解码器

```bash
# 列出支持的编解码器
libvlcsharp_aot.exe list-codecs

# 或使用别名
libvlcsharp_aot.exe lc
```

### 7. 运行性能基准测试

```bash
# 运行性能基准测试（5次）
libvlcsharp_aot.exe benchmark sample.mp4 5

# 或使用别名
libvlcsharp_aot.exe bm sample.mp4 5
```

### 8. 显示帮助信息

```bash
# 显示帮助信息
libvlcsharp_aot.exe help

# 或使用别名
libvlcsharp_aot.exe h
```

## 编程集成示例

### 与 ASP.NET Core 集成

```csharp
// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

var builder = WebApplication.CreateBuilder(args);

// 初始化 LibVLC
Core.Initialize();

// 注册 LibVLCSharp 服务
builder.Services.AddSingleton<LibVLCSharpService>();

var app = builder.Build();

app.MapGet("/play", async (LibVLCSharpService vlcService) => {
    await vlcService.PlayMediaAsync("sample.mp4", 10);
    return Results.Ok("媒体播放完成");
});

app.MapGet("/info", async (LibVLCSharpService vlcService) => {
    await vlcService.GetMediaInfoAsync("sample.mp4");
    return Results.Ok("媒体信息获取完成");
});

app.Run();
```

### 与 Blazor 集成

```csharp
// Program.cs (Blazor Server)
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

var builder = WebApplication.CreateBuilder(args);

// 初始化 LibVLC
Core.Initialize();

// 注册 LibVLCSharp 服务
builder.Services.AddScoped<LibVLCSharpService>();

var app = builder.Build();

app.Run();

// MediaPlayer.razor
@page "/media-player"
@inject LibVLCSharpService VlcService

<h3>媒体播放器</h3>

<button @onclick="PlayMedia">播放媒体</button>
<button @onclick="GetMediaInfo">获取信息</button>

@code {
    private async Task PlayMedia() {
        await VlcService.PlayMediaAsync("sample.mp4", 10);
    }
    
    private async Task GetMediaInfo() {
        await VlcService.GetMediaInfoAsync("sample.mp4");
    }
}
```

### 与 MAUI 集成

```csharp
// MauiProgram.cs
using Microsoft.Extensions.Logging;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // 初始化 LibVLC
        Core.Initialize();

        // 注册 LibVLCSharp 服务
        builder.Services.AddSingleton<LibVLCSharpService>();

        return builder.Build();
    }
}

// MainPage.xaml.cs
public partial class MainPage : ContentPage
{
    private readonly LibVLCSharpService _vlcService;

    public MainPage(LibVLCSharpService vlcService)
    {
        InitializeComponent();
        _vlcService = vlcService;
    }

    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        await _vlcService.PlayMediaAsync("sample.mp4", 10);
    }

    private async void OnInfoButtonClicked(object sender, EventArgs e)
    {
        await _vlcService.GetMediaInfoAsync("sample.mp4");
    }
}
```

## 使用场景示例

### 1. 媒体播放器应用

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class MediaPlayerApp
{
    private readonly LibVLCSharpService _vlcService;
    
    public MediaPlayerApp(LibVLCSharpService vlcService)
    {
        _vlcService = vlcService;
    }
    
    public async Task Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== 媒体播放器 ===");
            Console.WriteLine("1. 播放媒体文件");
            Console.WriteLine("2. 录制媒体文件");
            Console.WriteLine("3. 获取媒体信息");
            Console.WriteLine("4. 退出");
            Console.Write("请选择操作: ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    Console.Write("请输入媒体文件路径: ");
                    var mediaPath = Console.ReadLine();
                    Console.Write("请输入播放时长（秒）: ");
                    int.TryParse(Console.ReadLine(), out var duration);
                    await _vlcService.PlayMediaAsync(mediaPath, duration);
                    break;
                
                case "2":
                    Console.Write("请输入媒体文件路径: ");
                    var inputPath = Console.ReadLine();
                    Console.Write("请输入输出文件路径: ");
                    var outputPath = Console.ReadLine();
                    Console.Write("请输入录制时长（秒）: ");
                    int.TryParse(Console.ReadLine(), out var recordDuration);
                    await _vlcService.RecordMediaAsync(inputPath, outputPath, recordDuration);
                    break;
                
                case "3":
                    Console.Write("请输入媒体文件路径: ");
                    var infoPath = Console.ReadLine();
                    await _vlcService.GetMediaInfoAsync(infoPath);
                    break;
                
                case "4":
                    return;
                
                default:
                    Console.WriteLine("无效选择，请重试。");
                    break;
            }
            
            Console.WriteLine("\n按任意键继续...");
            Console.ReadKey();
        }
    }
}
```

### 2. 批量媒体转换工具

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class MediaConverter
{
    private readonly LibVLCSharpService _vlcService;
    
    public MediaConverter(LibVLCSharpService vlcService)
    {
        _vlcService = vlcService;
    }
    
    public async Task ConvertFolder(string inputFolder, string outputFolder, string format)
    {
        if (!Directory.Exists(inputFolder))
        {
            Console.WriteLine($"输入文件夹不存在: {inputFolder}");
            return;
        }
        
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        var mediaFiles = Directory.GetFiles(inputFolder, "*.*", SearchOption.AllDirectories)
            .Where(f => 
                f.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".avi", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".wmv", StringComparison.OrdinalIgnoreCase)
            );
        
        Console.WriteLine($"找到 {mediaFiles.Count()} 个媒体文件");
        Console.WriteLine("开始批量转换...");
        
        int successCount = 0;
        int failedCount = 0;
        
        foreach (var inputFile in mediaFiles)
        {
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(inputFile);
                var outputFile = Path.Combine(outputFolder, $"{fileName}.{format}");
                
                Console.WriteLine($"转换: {Path.GetFileName(inputFile)} -> {Path.GetFileName(outputFile)}");
                await _vlcService.ConvertMediaAsync(inputFile, outputFile, format);
                successCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"转换失败 {Path.GetFileName(inputFile)}: {ex.Message}");
                failedCount++;
            }
        }
        
        Console.WriteLine($"\n转换完成！");
        Console.WriteLine($"成功: {successCount}");
        Console.WriteLine($"失败: {failedCount}");
    }
}
```

## 性能优化示例

### 1. 内存优化

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 内存优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache(options => {
                options.SizeLimit = 1024; // 设置缓存大小限制
            })
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 批量处理媒体文件
            Console.WriteLine("批量处理媒体文件...");
            
            var mediaFiles = new[] { "file1.mp4", "file2.mp4", "file3.mp4" };
            
            foreach (var file in mediaFiles)
            {
                Console.WriteLine($"处理: {file}");
                await vlcService.GetMediaInfoAsync(file);
                
                // 强制垃圾回收（仅用于示例）
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

### 2. 并行处理优化

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 并行处理优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        
        try
        {
            // 并行获取多个媒体文件的信息
            Console.WriteLine("并行获取媒体信息...");
            
            var mediaFiles = new[] { "file1.mp4", "file2.mp4", "file3.mp4", "file4.mp4" };
            var tasks = new List<Task>();
            
            foreach (var file in mediaFiles)
            {
                tasks.Add(Task.Run(async () => {
                    Console.WriteLine($"开始处理: {file}");
                    await vlcService.GetMediaInfoAsync(file);
                    Console.WriteLine($"完成处理: {file}");
                }));
            }
            
            await Task.WhenAll(tasks);
            Console.WriteLine("\n所有任务完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
    }
}
```

## 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LibVLCSharp.Shared;
using LibVLCSharpAot;

public class Program
{
    public static async Task Main()
    {
        // 初始化 LibVLC
        Core.Initialize();
        
        Console.WriteLine("LibVLCSharp 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .AddMemoryCache()
            .AddSingleton<LibVLCSharpService>()
            .BuildServiceProvider();
        
        var vlcService = serviceProvider.GetRequiredService<LibVLCSharpService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            // 尝试播放不存在的文件
            Console.WriteLine("尝试播放不存在的文件...");
            await vlcService.PlayMediaAsync("non_existent_file.mp4", 10);
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "文件未找到");
            Console.WriteLine("错误: 文件未找到，请检查文件路径是否正确。");
        }
        catch (DirectoryNotFoundException ex)
        {
            logger.LogError(ex, "目录未找到");
            Console.WriteLine("错误: 目录未找到，请检查路径是否正确。");
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "访问权限不足");
            Console.WriteLine("错误: 访问权限不足，请检查文件权限。");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "未知错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 清理 LibVLC
            Core.Cleanup();
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
}
```

## 总结

本文档提供了 LibVLCSharp 技能的详细使用示例，包括：

1. **基本使用**：快速上手 LibVLCSharp 的核心功能
2. **媒体录制**：如何录制媒体文件
3. **格式转换**：如何转换媒体格式
4. **流媒体**：如何创建流媒体服务
5. **性能测试**：如何测试媒体处理性能
6. **高级配置**：如何配置 LibVLCSharp 以获得最佳性能
7. **命令行使用**：如何通过命令行使用 LibVLCSharp 工具
8. **编程集成**：如何与 ASP.NET Core、Blazor 和 MAUI 集成
9. **使用场景**：实际应用场景的完整示例
10. **性能优化**：如何优化内存使用和并行处理
11. **错误处理**：如何正确处理各种异常情况

通过这些示例，您可以快速掌握 LibVLCSharp 的使用方法，并根据自己的需求进行扩展和定制。系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。
