# OCR 技能使用示例

## 概述

本文档提供了 OCR 技能的各种使用示例，包括基本用法、高级配置、性能优化、错误处理等。这些示例旨在帮助您快速上手 OCR 文字识别功能，并了解其在不同场景下的应用。

## 示例 1: 基本使用

### 功能说明

演示 OCR 技能的基本使用方法，包括服务注册、图像识别和结果处理。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

// 配置选项
public class OCROptions
{
    public bool Enabled { get; set; } = true;
    public int MaxImageSize { get; set; } = 4096;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public System.Collections.Generic.List<string> Languages { get; set; } = new System.Collections.Generic.List<string> { "zh-CN", "en-US" };
}

// OCR 服务接口
public interface IOCRService
{
    Task<IOCRResult> RecognizeTextAsync(Bitmap image);
}

// OCR 结果接口
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    System.Collections.Generic.IEnumerable<string> TextLines { get; }
    double Confidence { get; }
    TimeSpan ProcessingTime { get; }
}

// OCR 结果实现
public class OCRResult : IOCRResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public System.Collections.Generic.IEnumerable<string> TextLines { get; set; } = System.Array.Empty<string>();
    public double Confidence { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}

// OCR 服务实现
public class OCRService : IOCRService
{
    private readonly OCROptions _options;
    private readonly ILogger<OCRService> _logger;

    public OCRService(Microsoft.Extensions.Options.IOptions<OCROptions> options, ILogger<OCRService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IOCRResult> RecognizeTextAsync(Bitmap image)
    {
        var result = new OCRResult();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            if (!_options.Enabled)
            {
                result.Success = false;
                result.ErrorMessage = "OCR 服务已禁用";
                return result;
            }

            _logger.LogInformation("开始 OCR 识别");
            // 模拟 OCR 识别过程
            await Task.Delay(1000);

            // 模拟识别结果
            var textLines = new System.Collections.Generic.List<string>
            {
                "这是一段示例文本",
                "This is a sample text",
                "1234567890"
            };

            result.Success = true;
            result.TextLines = textLines;
            result.Confidence = 0.95;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 识别失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            _logger.LogInformation("OCR 识别完成，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }

        return result;
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddOCRServices(this IServiceCollection services, System.Action<OCROptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OCROptions>(options => { });
        }

        services.AddSingleton<IOCRService, OCRService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 基本使用示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OCR 服务
        services.AddOCRServices(options =>
        {
            options.Enabled = true;
            options.MaxImageSize = 4096;
            options.Languages = new System.Collections.Generic.List<string> { "zh-CN", "en-US" };
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<IOCRService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 创建模拟图像
            Console.WriteLine("创建模拟图像...");
            using var bitmap = new Bitmap(800, 600);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                using (var font = new Font("Arial", 24))
                using (var brush = new SolidBrush(Color.Black))
                {
                    graphics.DrawString("这是一段示例文本", font, brush, 50, 50);
                    graphics.DrawString("This is a sample text", font, brush, 50, 100);
                    graphics.DrawString("1234567890", font, brush, 50, 150);
                }
            }

            // 识别图像
            Console.WriteLine("开始识别图像...");
            var result = await ocrService.RecognizeTextAsync(bitmap);

            // 处理结果
            Console.WriteLine($"识别结果: {(result.Success ? "成功" : "失败")}");
            if (result.Success)
            {
                Console.WriteLine("识别到的文字:");
                foreach (var text in result.TextLines)
                {
                    Console.WriteLine($"- {text}");
                }
                Console.WriteLine($"置信度: {result.Confidence:P}");
                Console.WriteLine($"处理时间: {result.ProcessingTime.TotalMilliseconds:F2}ms");
            }
            else
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }

            Console.WriteLine("示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 2: 高级配置

### 功能说明

演示 OCR 技能的高级配置选项，包括图像预处理、并行处理和批量处理等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

// 配置选项
public class OCROptions
{
    public bool Enabled { get; set; } = true;
    public int MaxImageSize { get; set; } = 4096;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public System.Collections.Generic.List<string> Languages { get; set; } = new System.Collections.Generic.List<string> { "zh-CN", "en-US" };
    public bool EnableImagePreprocessing { get; set; } = true;
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public int BatchSize { get; set; } = 10;
}

// OCR 服务接口
public interface IOCRService
{
    Task<IOCRResult> RecognizeTextAsync(Bitmap image);
    Task<System.Collections.Generic.IEnumerable<IOCRResult>> RecognizeTextBatchAsync(System.Collections.Generic.IEnumerable<Bitmap> images);
}

// OCR 结果接口
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    System.Collections.Generic.IEnumerable<string> TextLines { get; }
    double Confidence { get; }
    TimeSpan ProcessingTime { get; }
}

// OCR 结果实现
public class OCRResult : IOCRResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public System.Collections.Generic.IEnumerable<string> TextLines { get; set; } = System.Array.Empty<string>();
    public double Confidence { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}

// 图像处理器
public class ImageProcessor
{
    public async Task<Bitmap> PreprocessAsync(Bitmap image)
    {
        // 模拟图像预处理
        await Task.Delay(500);
        return new Bitmap(image);
    }
}

// OCR 服务实现
public class OCRService : IOCRService
{
    private readonly OCROptions _options;
    private readonly ILogger<OCRService> _logger;
    private readonly ImageProcessor _imageProcessor;

    public OCRService(Microsoft.Extensions.Options.IOptions<OCROptions> options, ILogger<OCRService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _imageProcessor = new ImageProcessor();
    }

    public async Task<IOCRResult> RecognizeTextAsync(Bitmap image)
    {
        var result = new OCRResult();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            if (!_options.Enabled)
            {
                result.Success = false;
                result.ErrorMessage = "OCR 服务已禁用";
                return result;
            }

            // 图像预处理
            if (_options.EnableImagePreprocessing)
            {
                image = await _imageProcessor.PreprocessAsync(image);
            }

            _logger.LogInformation("开始 OCR 识别");
            // 模拟 OCR 识别过程
            await Task.Delay(1000);

            // 模拟识别结果
            var textLines = new System.Collections.Generic.List<string>
            {
                "这是一段示例文本",
                "This is a sample text"
            };

            result.Success = true;
            result.TextLines = textLines;
            result.Confidence = 0.95;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 识别失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
        }

        return result;
    }

    public async Task<System.Collections.Generic.IEnumerable<IOCRResult>> RecognizeTextBatchAsync(System.Collections.Generic.IEnumerable<Bitmap> images)
    {
        var results = new System.Collections.Generic.List<IOCRResult>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        _logger.LogInformation("开始批量 OCR 识别，图像数量: {Count}", images.Count());

        if (_options.EnableParallelProcessing)
        {
            // 并行处理
            var processingTasks = images.Select(image => RecognizeTextAsync(image));
            results.AddRange(await Task.WhenAll(processingTasks));
        }
        else
        {
            // 串行处理
            foreach (var image in images)
            {
                var result = await RecognizeTextAsync(image);
                results.Add(result);
            }
        }

        stopwatch.Stop();
        _logger.LogInformation("批量 OCR 识别完成，总耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);

        return results;
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddOCRServices(this IServiceCollection services, System.Action<OCROptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OCROptions>(options => { });
        }

        services.AddSingleton<IOCRService, OCRService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 高级配置示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OCR 服务并配置高级选项
        services.AddOCRServices(options =>
        {
            options.Enabled = true;
            options.MaxImageSize = 4096;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.Languages = new System.Collections.Generic.List<string> { "zh-CN", "en-US" };
            options.EnableImagePreprocessing = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            options.BatchSize = 10;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<IOCRService>();

        try
        {
            // 创建多个模拟图像
            Console.WriteLine("创建模拟图像...");
            var images = new System.Collections.Generic.List<Bitmap>();
            for (int i = 0; i < 3; i++)
            {
                var bitmap = new Bitmap(800, 600);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);
                    using (var font = new Font("Arial", 24))
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        graphics.DrawString($"图像 {i + 1}: 示例文本", font, brush, 50, 50);
                    }
                }
                images.Add(bitmap);
            }

            // 批量识别
            Console.WriteLine("开始批量识别...");
            var results = await ocrService.RecognizeTextBatchAsync(images);

            // 处理结果
            Console.WriteLine("识别结果:");
            foreach (var (result, index) in results.Select((r, i) => (r, i)))
            {
                Console.WriteLine($"图像 {index + 1}: {(result.Success ? "成功" : "失败")}");
                if (result.Success)
                {
                    foreach (var text in result.TextLines)
                    {
                        Console.WriteLine($"  - {text}");
                    }
                }
                else
                {
                    Console.WriteLine($"  错误: {result.ErrorMessage}");
                }
            }

            // 释放资源
            foreach (var image in images)
            {
                image.Dispose();
            }

            Console.WriteLine("示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 3: 性能优化

### 功能说明

演示 OCR 技能的性能优化技术，包括内存优化、并行处理和批处理等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:package System.Threading.Tasks.Dataflow@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
using System.Diagnostics;

// 配置选项
public class OCROptions
{
    public bool Enabled { get; set; } = true;
    public int MaxImageSize { get; set; } = 4096;
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public int BatchSize { get; set; } = 10;
}

// OCR 服务接口
public interface IOCRService
{
    Task<IOCRResult> RecognizeTextAsync(Bitmap image);
    Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths);
}

// OCR 结果接口
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    IEnumerable<string> TextLines { get; }
    double Confidence { get; }
    TimeSpan ProcessingTime { get; }
}

// OCR 结果实现
public class OCRResult : IOCRResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public IEnumerable<string> TextLines { get; set; } = Array.Empty<string>();
    public double Confidence { get; set; }
    public TimeSpan ProcessingTime { get; }
}

// 内存优化的 OCR 服务
public class OCRService : IOCRService
{
    private readonly OCROptions _options;
    private readonly ILogger<OCRService> _logger;

    public OCRService(Microsoft.Extensions.Options.IOptions<OCROptions> options, ILogger<OCRService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IOCRResult> RecognizeTextAsync(Bitmap image)
    {
        var result = new OCRResult();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 内存优化：使用 using 语句确保及时释放资源
            using (var processedImage = new Bitmap(image))
            {
                _logger.LogInformation("开始 OCR 识别");
                // 模拟 OCR 识别过程
                await Task.Delay(500);

                // 模拟识别结果
                var textLines = new List<string>
                {
                    "性能优化示例",
                    "Performance optimization example"
                };

                result.Success = true;
                result.TextLines = textLines;
                result.Confidence = 0.95;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 识别失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
        }

        return result;
    }

    public async Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths)
    {
        var results = new List<IOCRResult>();
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("开始批量 OCR 识别，图像数量: {Count}", imagePaths.Count());

        // 使用数据流块进行批处理
        var transformBlock = new TransformBlock<string, IOCRResult>(
            async path =>
            {
                // 模拟从文件加载图像
                using var bitmap = new Bitmap(800, 600);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);
                    using (var font = new Font("Arial", 24))
                    using (var brush = new SolidBrush(Color.Black))
                    {
                        graphics.DrawString($"文件: {System.IO.Path.GetFileName(path)}", font, brush, 50, 50);
                    }
                }

                return await RecognizeTextAsync(bitmap);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism
            }
        );

        var actionBlock = new ActionBlock<IOCRResult>(
            result => results.Add(result)
        );

        transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

        // 发布所有图像路径
        foreach (var path in imagePaths)
        {
            await transformBlock.SendAsync(path);
        }

        transformBlock.Complete();
        await actionBlock.Completion;

        stopwatch.Stop();
        _logger.LogInformation("批量 OCR 识别完成，总耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);

        return results;
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddOCRServices(this IServiceCollection services, Action<OCROptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OCROptions>(options => { });
        }

        services.AddSingleton<IOCRService, OCRService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 性能优化示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OCR 服务并配置性能优化选项
        services.AddOCRServices(options =>
        {
            options.Enabled = true;
            options.MaxImageSize = 4096;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            options.BatchSize = 10;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<IOCRService>();

        try
        {
            // 性能测试：批量处理
            Console.WriteLine("性能测试：批量处理");
            var imagePaths = Enumerable.Range(1, 5).Select(i => $"image{i}.jpg");

            var stopwatch = Stopwatch.StartNew();
            var results = await ocrService.RecognizeTextBatchAsync(imagePaths);
            stopwatch.Stop();

            Console.WriteLine($"处理 5 个图像耗时: {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
            Console.WriteLine($"平均每个图像耗时: {stopwatch.Elapsed.TotalMilliseconds / 5:F2}ms");

            // 处理结果
            Console.WriteLine("识别结果:");
            foreach (var (result, index) in results.Select((r, i) => (r, i)))
            {
                Console.WriteLine($"图像 {index + 1}: {(result.Success ? "成功" : "失败")}");
            }

            Console.WriteLine("示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 4: 错误处理

### 功能说明

演示 OCR 技能的错误处理机制，包括超时处理、异常捕获和错误恢复等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Threading;

// 配置选项
public class OCROptions
{
    public bool Enabled { get; set; } = true;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}

// OCR 服务接口
public interface IOCRService
{
    Task<IOCRResult> RecognizeTextAsync(string imagePath, CancellationToken cancellationToken = default);
}

// OCR 结果接口
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    IEnumerable<string> TextLines { get; }
    double Confidence { get; }
}

// OCR 结果实现
public class OCRResult : IOCRResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public IEnumerable<string> TextLines { get; set; } = Array.Empty<string>();
    public double Confidence { get; set; }
}

// OCR 服务实现
public class OCRService : IOCRService
{
    private readonly OCROptions _options;
    private readonly ILogger<OCRService> _logger;

    public OCRService(Microsoft.Extensions.Options.IOptions<OCROptions> options, ILogger<OCRService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IOCRResult> RecognizeTextAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        var result = new OCRResult();

        try
        {
            if (!_options.Enabled)
            {
                result.Success = false;
                result.ErrorMessage = "OCR 服务已禁用";
                return result;
            }

            // 检查文件是否存在
            if (!System.IO.File.Exists(imagePath))
            {
                result.Success = false;
                result.ErrorMessage = "图像文件不存在";
                return result;
            }

            _logger.LogInformation("开始 OCR 识别: {ImagePath}", imagePath);

            // 模拟长时间运行的操作
            await Task.Delay(2000, cancellationToken);

            // 模拟 OCR 识别过程
            using var bitmap = new Bitmap(imagePath);
            // 模拟识别结果
            var textLines = new List<string>
            {
                "错误处理示例",
                "Error handling example"
            };

            result.Success = true;
            result.TextLines = textLines;
            result.Confidence = 0.95;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("OCR 处理已取消或超时");
            result.Success = false;
            result.ErrorMessage = "处理已取消或超时";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 识别失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddOCRServices(this IServiceCollection services, Action<OCROptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OCROptions>(options => { });
        }

        services.AddSingleton<IOCRService, OCRService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 错误处理示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OCR 服务
        services.AddOCRServices(options =>
        {
            options.Enabled = true;
            options.Timeout = TimeSpan.FromSeconds(30);
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<IOCRService>();

        try
        {
            // 测试 1: 不存在的文件
            Console.WriteLine("测试 1: 不存在的文件");
            var result1 = await ocrService.RecognizeTextAsync("non_existent.jpg");
            Console.WriteLine($"结果: {(result1.Success ? "成功" : "失败")}");
            if (!result1.Success)
            {
                Console.WriteLine($"错误: {result1.ErrorMessage}");
            }

            // 测试 2: 超时处理
            Console.WriteLine("\n测试 2: 超时处理");
            using var cts = new CancellationTokenSource(500); // 500ms 超时
            try
            {
                var result2 = await ocrService.RecognizeTextAsync("image.jpg", cts.Token);
                Console.WriteLine($"结果: {(result2.Success ? "成功" : "失败")}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("操作已超时");
            }

            // 测试 3: 创建测试图像并识别
            Console.WriteLine("\n测试 3: 创建测试图像并识别");
            var testImagePath = "test_image.jpg";
            CreateTestImage(testImagePath);

            var result3 = await ocrService.RecognizeTextAsync(testImagePath);
            Console.WriteLine($"结果: {(result3.Success ? "成功" : "失败")}");
            if (result3.Success)
            {
                Console.WriteLine("识别到的文字:");
                foreach (var text in result3.TextLines)
                {
                    Console.WriteLine($"- {text}");
                }
            }
            else
            {
                Console.WriteLine($"错误: {result3.ErrorMessage}");
            }

            // 清理测试文件
            if (System.IO.File.Exists(testImagePath))
            {
                System.IO.File.Delete(testImagePath);
            }

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }

    private static void CreateTestImage(string path)
    {
        using var bitmap = new Bitmap(800, 600);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        using var font = new Font("Arial", 24);
        using var brush = new SolidBrush(Color.Black);
        graphics.DrawString("测试图像", font, brush, 50, 50);
        graphics.DrawString("Test Image", font, brush, 50, 100);
        bitmap.Save(path, ImageFormat.Jpeg);
    }
}
```

## 示例 5: 自定义 OCR 服务

### 功能说明

演示如何创建自定义的 OCR 服务，扩展基本功能以满足特定需求。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

// 自定义配置选项
public class CustomOCROptions
{
    public bool Enabled { get; set; } = true;
    public List<string> SupportedLanguages { get; set; } = new List<string> { "zh-CN", "en-US" };
    public bool EnableCustomPreprocessing { get; set; } = true;
    public string CustomConfigPath { get; set; } = "ocr_config.json";
}

// 自定义 OCR 服务接口
public interface ICustomOCRService
{
    Task<IOCRResult> RecognizeTextAsync(string imagePath);
    Task<IOCRResult> RecognizeTextWithLanguageAsync(string imagePath, string language);
    Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths);
}

// OCR 结果接口
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    IEnumerable<string> TextLines { get; }
    double Confidence { get; }
    string Language { get; }
}

// OCR 结果实现
public class OCRResult : IOCRResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public IEnumerable<string> TextLines { get; set; } = Array.Empty<string>();
    public double Confidence { get; set; }
    public string Language { get; set; } = "zh-CN";
}

// 自定义图像处理器
public class CustomImageProcessor
{
    public async Task<Bitmap> PreprocessAsync(Bitmap image, string language)
    {
        // 模拟针对特定语言的预处理
        await Task.Delay(300);
        return new Bitmap(image);
    }
}

// 自定义 OCR 服务实现
public class CustomOCRService : ICustomOCRService
{
    private readonly CustomOCROptions _options;
    private readonly ILogger<CustomOCRService> _logger;
    private readonly CustomImageProcessor _imageProcessor;

    public CustomOCRService(Microsoft.Extensions.Options.IOptions<CustomOCROptions> options, ILogger<CustomOCRService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _imageProcessor = new CustomImageProcessor();
        
        // 加载自定义配置
        LoadCustomConfig();
    }

    private void LoadCustomConfig()
    {
        _logger.LogInformation("加载自定义 OCR 配置: {ConfigPath}", _options.CustomConfigPath);
        // 模拟加载配置
    }

    public async Task<IOCRResult> RecognizeTextAsync(string imagePath)
    {
        return await RecognizeTextWithLanguageAsync(imagePath, _options.SupportedLanguages[0]);
    }

    public async Task<IOCRResult> RecognizeTextWithLanguageAsync(string imagePath, string language)
    {
        var result = new OCRResult { Language = language };

        try
        {
            if (!_options.Enabled)
            {
                result.Success = false;
                result.ErrorMessage = "OCR 服务已禁用";
                return result;
            }

            if (!_options.SupportedLanguages.Contains(language))
            {
                result.Success = false;
                result.ErrorMessage = $"不支持的语言: {language}";
                return result;
            }

            _logger.LogInformation("开始 OCR 识别: {ImagePath}, 语言: {Language}", imagePath, language);

            // 创建测试图像（如果不存在）
            if (!System.IO.File.Exists(imagePath))
            {
                CreateTestImage(imagePath, language);
            }

            using var bitmap = new Bitmap(imagePath);

            // 自定义预处理
            if (_options.EnableCustomPreprocessing)
            {
                bitmap.Dispose();
                bitmap = await _imageProcessor.PreprocessAsync(bitmap, language);
            }

            // 模拟 OCR 识别过程
            await Task.Delay(1000);

            // 模拟针对特定语言的识别结果
            var textLines = new List<string>();
            if (language == "zh-CN")
            {
                textLines.AddRange(new[] { "自定义 OCR 服务", "语言: 中文" });
            }
            else if (language == "en-US")
            {
                textLines.AddRange(new[] { "Custom OCR Service", "Language: English" });
            }

            result.Success = true;
            result.TextLines = textLines;
            result.Confidence = 0.95;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 识别失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths)
    {
        var tasks = imagePaths.Select(path => RecognizeTextAsync(path));
        return await Task.WhenAll(tasks);
    }

    private void CreateTestImage(string path, string language)
    {
        using var bitmap = new Bitmap(800, 600);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        using var font = new Font("Arial", 24);
        using var brush = new SolidBrush(Color.Black);
        
        if (language == "zh-CN")
        {
            graphics.DrawString("测试图像 - 中文", font, brush, 50, 50);
        }
        else
        {
            graphics.DrawString("Test Image - English", font, brush, 50, 50);
        }
        
        bitmap.Save(path, ImageFormat.Jpeg);
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddCustomOCRServices(this IServiceCollection services, Action<CustomOCROptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<CustomOCROptions>(options => { });
        }

        services.AddSingleton<ICustomOCRService, CustomOCRService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 自定义服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册自定义 OCR 服务
        services.AddCustomOCRServices(options =>
        {
            options.Enabled = true;
            options.SupportedLanguages = new List<string> { "zh-CN", "en-US" };
            options.EnableCustomPreprocessing = true;
            options.CustomConfigPath = "ocr_config.json";
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<ICustomOCRService>();

        try
        {
            // 测试 1: 中文识别
            Console.WriteLine("测试 1: 中文识别");
            var result1 = await ocrService.RecognizeTextWithLanguageAsync("chinese_image.jpg", "zh-CN");
            Console.WriteLine($"结果: {(result1.Success ? "成功" : "失败")}");
            if (result1.Success)
            {
                Console.WriteLine("识别到的文字:");
                foreach (var text in result1.TextLines)
                {
                    Console.WriteLine($"- {text}");
                }
            }

            // 测试 2: 英文识别
            Console.WriteLine("\n测试 2: 英文识别");
            var result2 = await ocrService.RecognizeTextWithLanguageAsync("english_image.jpg", "en-US");
            Console.WriteLine($"结果: {(result2.Success ? "成功" : "失败")}");
            if (result2.Success)
            {
                Console.WriteLine("识别到的文字:");
                foreach (var text in result2.TextLines)
                {
                    Console.WriteLine($"- {text}");
                }
            }

            // 测试 3: 批量识别
            Console.WriteLine("\n测试 3: 批量识别");
            var imagePaths = new[] { "batch1.jpg", "batch2.jpg" };
            var results = await ocrService.RecognizeTextBatchAsync(imagePaths);

            Console.WriteLine("批量识别结果:");
            foreach (var (result, index) in results.Select((r, i) => (r, i)))
            {
                Console.WriteLine($"图像 {index + 1}: {(result.Success ? "成功" : "失败")}");
            }

            // 清理测试文件
            var testFiles = new[] { "chinese_image.jpg", "english_image.jpg", "batch1.jpg", "batch2.jpg" };
            foreach (var file in testFiles)
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }

            Console.WriteLine("\n示例执行完成！");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 示例 6: 实际应用场景

### 功能说明

演示 OCR 技能在实际应用场景中的使用，如文档数字化、证件识别等。

### 代码示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Threading.Tasks;

// 应用场景配置选项
public class OCRApplicationOptions
{
    public bool Enabled { get; set; } = true;
    public string DocumentOutputPath { get; set; } = "output";
    public bool EnableDocumentSaving { get; set; } = true;
    public bool EnableBatchProcessing { get; set; } = true;
}

// 文档类型
public enum DocumentType
{
    Invoice,
    IdCard,
    Passport,
    Receipt,
    General
}

// 文档信息
public class DocumentInfo
{
    public string FilePath { get; set; }
    public DocumentType Type { get; set; }
    public string DocumentNumber { get; set; }
    public string Issuer { get; set; }
    public DateTime IssueDate { get; set; }
    public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
}

// OCR 服务接口
public interface IOCRService
{
    Task<DocumentInfo> ProcessDocumentAsync(string imagePath, DocumentType type);
    Task<IEnumerable<DocumentInfo>> ProcessDocumentsAsync(IEnumerable<string> imagePaths, DocumentType type);
}

// OCR 服务实现
public class OCRService : IOCRService
{
    private readonly OCRApplicationOptions _options;
    private readonly ILogger<OCRService> _logger;

    public OCRService(Microsoft.Extensions.Options.IOptions<OCRApplicationOptions> options, ILogger<OCRService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        // 确保输出目录存在
        if (_options.EnableDocumentSaving && !System.IO.Directory.Exists(_options.DocumentOutputPath))
        {
            System.IO.Directory.CreateDirectory(_options.DocumentOutputPath);
        }
    }

    public async Task<DocumentInfo> ProcessDocumentAsync(string imagePath, DocumentType type)
    {
        var documentInfo = new DocumentInfo
        {
            FilePath = imagePath,
            Type = type,
            IssueDate = DateTime.Now
        };

        try
        {
            if (!_options.Enabled)
            {
                throw new InvalidOperationException("OCR 服务已禁用");
            }

            _logger.LogInformation("处理文档: {ImagePath}, 类型: {Type}", imagePath, type);

            // 创建测试图像（如果不存在）
            if (!System.IO.File.Exists(imagePath))
            {
                CreateTestDocument(imagePath, type);
            }

            // 模拟文档处理
            await Task.Delay(1500);

            // 根据文档类型提取信息
            switch (type)
            {
                case DocumentType.Invoice:
                    ProcessInvoice(documentInfo);
                    break;
                case DocumentType.IdCard:
                    ProcessIdCard(documentInfo);
                    break;
                case DocumentType.Passport:
                    ProcessPassport(documentInfo);
                    break;
                case DocumentType.Receipt:
                    ProcessReceipt(documentInfo);
                    break;
                default:
                    ProcessGeneralDocument(documentInfo);
                    break;
            }

            // 保存处理结果
            if (_options.EnableDocumentSaving)
            {
                SaveDocumentInfo(documentInfo);
            }

            _logger.LogInformation("文档处理完成: {DocumentNumber}", documentInfo.DocumentNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文档处理失败");
            throw;
        }

        return documentInfo;
    }

    public async Task<IEnumerable<DocumentInfo>> ProcessDocumentsAsync(IEnumerable<string> imagePaths, DocumentType type)
    {
        _logger.LogInformation("开始批量处理文档，数量: {Count}", imagePaths.Count());

        var tasks = imagePaths.Select(path => ProcessDocumentAsync(path, type));
        var results = await Task.WhenAll(tasks);

        _logger.LogInformation("批量处理完成，成功: {SuccessCount}", results.Length);
        return results;
    }

    private void ProcessInvoice(DocumentInfo info)
    {
        info.DocumentNumber = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        info.Issuer = "示例公司";
        info.Fields["InvoiceNumber"] = info.DocumentNumber;
        info.Fields["Amount"] = "¥1,234.56";
        info.Fields["Tax"] = "¥123.46";
        info.Fields["Total"] = "¥1,358.02";
        info.Fields["Customer"] = "测试客户";
    }

    private void ProcessIdCard(DocumentInfo info)
    {
        info.DocumentNumber = "110101199001011234";
        info.Issuer = "公安局";
        info.Fields["Name"] = "张三";
        info.Fields["Gender"] = "男";
        info.Fields["BirthDate"] = "1990-01-01";
        info.Fields["Address"] = "北京市朝阳区";
    }

    private void ProcessPassport(DocumentInfo info)
    {
        info.DocumentNumber = "E12345678";
        info.Issuer = "外交部";
        info.Fields["Name"] = "ZHANG SAN";
        info.Fields["Nationality"] = "CHN";
        info.Fields["BirthDate"] = "1990-01-01";
        info.Fields["ExpiryDate"] = "2030-01-01";
    }

    private void ProcessReceipt(DocumentInfo info)
    {
        info.DocumentNumber = "RC-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        info.Issuer = "超市";
        info.Fields["ReceiptNumber"] = info.DocumentNumber;
        info.Fields["Amount"] = "¥99.99";
        info.Fields["Items"] = "3";
        info.Fields["Cashier"] = "李四";
    }

    private void ProcessGeneralDocument(DocumentInfo info)
    {
        info.DocumentNumber = "DOC-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        info.Issuer = "未知";
        info.Fields["Content"] = "通用文档内容";
    }

    private void SaveDocumentInfo(DocumentInfo info)
    {
        var outputPath = System.IO.Path.Combine(_options.DocumentOutputPath, $"{info.DocumentNumber}.txt");
        var content = new List<string>
        {
            $"文档类型: {info.Type}",
            $"文档编号: {info.DocumentNumber}",
            $"签发机构: {info.Issuer}",
            $"签发日期: {info.IssueDate}",
            "字段:",
        };

        foreach (var field in info.Fields)
        {
            content.Add($"  {field.Key}: {field.Value}");
        }

        System.IO.File.WriteAllLines(outputPath, content);
        _logger.LogInformation("文档信息已保存: {OutputPath}", outputPath);
    }

    private void CreateTestDocument(string path, DocumentType type)
    {
        using var bitmap = new Bitmap(800, 600);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        using var font = new Font("Arial", 16);
        using var brush = new SolidBrush(Color.Black);

        string title = type switch
        {
            DocumentType.Invoice => "发票",
            DocumentType.IdCard => "身份证",
            DocumentType.Passport => "护照",
            DocumentType.Receipt => "收据",
            _ => "通用文档"
        };

        graphics.DrawString($"{title} 示例", new Font("Arial", 24), brush, 50, 50);
        graphics.DrawString($"编号: TEST-{type.ToString().Substring(0, 3).ToUpper()}-{DateTime.Now.ToString("yyyyMMdd")}", font, brush, 50, 100);
        graphics.DrawString($"日期: {DateTime.Now.ToShortDateString()}", font, brush, 50, 140);
        graphics.DrawString("这是一个测试文档", font, brush, 50, 180);

        bitmap.Save(path, ImageFormat.Jpeg);
        _logger.LogInformation("创建测试文档: {Path}", path);
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddOCRServices(this IServiceCollection services, Action<OCRApplicationOptions> configureOptions = null)
    {
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OCRApplicationOptions>(options => { });
        }

        services.AddSingleton<IOCRService, OCRService>();
        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 实际应用场景示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 注册 OCR 服务
        services.AddOCRServices(options =>
        {
            options.Enabled = true;
            options.DocumentOutputPath = "output";
            options.EnableDocumentSaving = true;
            options.EnableBatchProcessing = true;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<IOCRService>();

        try
        {
            // 测试 1: 处理发票
            Console.WriteLine("测试 1: 处理发票");
            var invoiceInfo = await ocrService.ProcessDocumentAsync("invoice.jpg", DocumentType.Invoice);
            Console.WriteLine($"发票处理完成: {invoiceInfo.DocumentNumber}");
            Console.WriteLine($"金额: {invoiceInfo.Fields["Amount"]}");
            Console.WriteLine($"总计: {invoiceInfo.Fields["Total"]}");

            // 测试 2: 处理身份证
            Console.WriteLine("\n测试 2: 处理身份证");
            var idCardInfo = await ocrService.ProcessDocumentAsync("idcard.jpg", DocumentType.IdCard);
            Console.WriteLine($"身份证处理完成: {idCardInfo.DocumentNumber}");
            Console.WriteLine($"姓名: {idCardInfo.Fields["Name"]}");
            Console.WriteLine($"性别: {idCardInfo.Fields["Gender"]}");

            // 测试 3: 批量处理收据
            Console.WriteLine("\n测试 3: 批量处理收据");
            var receiptPaths = Enumerable.Range(1, 3).Select(i => $"receipt{i}.jpg");
            var receipts = await ocrService.ProcessDocumentsAsync(receiptPaths, DocumentType.Receipt);
            Console.WriteLine($"批量处理完成，共 {receipts.Count()} 张收据");
            foreach (var receipt in receipts)
            {
                Console.WriteLine($"- {receipt.DocumentNumber}: {receipt.Fields["Amount"]}");
            }

            Console.WriteLine("\n示例执行完成！");
            Console.WriteLine($"处理结果已保存到: output 目录");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

## 实际应用场景

### 场景 1: 文档数字化

**功能说明**：将纸质文档扫描后转换为可编辑的数字文档。

**应用示例**：
- 扫描纸质合同、发票、收据等文档
- 自动识别文档中的文字内容
- 将识别结果保存为可编辑的电子文档
- 建立文档索引，方便搜索和管理

### 场景 2: 证件识别

**功能说明**：自动识别身份证、护照等证件信息。

**应用示例**：
- 身份证信息自动提取（姓名、身份证号、地址等）
- 护照信息自动识别
- 驾驶证、行驶证信息提取
- 企业营业执照信息识别

### 场景 3: 发票识别

**功能说明**：自动识别发票内容并提取关键信息。

**应用示例**：
- 增值税发票识别
- 普通发票识别
- 电子发票识别
- 发票真伪验证
- 自动生成记账凭证

### 场景 4: 车牌识别

**功能说明**：识别车辆牌照信息。

**应用示例**：
- 停车场出入口车牌识别
- 高速公路收费车牌识别
- 违章车辆车牌识别
- 车辆管理系统

### 场景 5: 票据识别

**功能说明**：识别各类票据的文字信息。

**应用示例**：
- 银行支票识别
- 汇票、本票识别
- 保险单识别
- 医疗票据识别
- 交通罚单识别

### 场景 6: 图像搜索

**功能说明**：基于图像中的文字进行搜索。

**应用示例**：
- 基于文字内容搜索图片
- 产品包装上的文字搜索
- 广告牌、海报上的文字搜索
- 文档扫描件的文字搜索

### 场景 7: 辅助阅读

**功能说明**：帮助视障人士阅读图像中的文字。

**应用示例**：
- 实时识别并朗读图像中的文字
- 识别书籍、杂志等印刷品
- 识别屏幕上的文字
- 识别公共场所的标识、指示牌

## 总结

OCR 技能提供了强大的光学字符识别功能，可以帮助您在各种场景下实现文字识别和信息提取。通过本文档的示例，您应该已经了解了 OCR 技能的基本使用方法、高级配置、性能优化、错误处理、自定义扩展和实际应用场景等方面的知识。

在实际应用中，您可以根据具体需求选择合适的使用方式，并结合 AOT 编译等技术优化性能。OCR 技能的灵活性和扩展性使其成为构建文字识别系统的理想选择。
