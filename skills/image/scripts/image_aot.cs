#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@8.0.0
#:package System.Text.RegularExpressions@4.3.1
#:package System.Collections.Immutable@8.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:package SixLabors.ImageSharp@3.0.0
#:package SixLabors.ImageSharp.Drawing@3.0.0
#:package SixLabors.Fonts@3.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
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
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Image AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var imageService = serviceProvider.GetRequiredService<ImageService>();
        var settings = serviceProvider.GetRequiredService<IOptions<ImageSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "resize":
                case "r":
                    await ResizeImage(imageService, arguments);
                    break;
                case "convert":
                case "c":
                    await ConvertImage(imageService, arguments);
                    break;
                case "crop":
                case "cr":
                    await CropImage(imageService, arguments);
                    break;
                case "rotate":
                case "ro":
                    await RotateImage(imageService, arguments);
                    break;
                case "flip":
                case "f":
                    await FlipImage(imageService, arguments);
                    break;
                case "watermark":
                case "w":
                    await AddWatermark(imageService, arguments);
                    break;
                case "filter":
                case "fi":
                    await ApplyFilter(imageService, arguments);
                    break;
                case "metadata":
                case "m":
                    await GetMetadata(imageService, arguments);
                    break;
                case "optimize":
                case "o":
                    await OptimizeImage(imageService, arguments);
                    break;
                case "batch":
                case "b":
                    await BatchProcess(imageService, arguments);
                    break;
                case "benchmark":
                case "bm":
                    await RunBenchmark(imageService, arguments);
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
            await imageService.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<ImageSettings>(options => {
            options.DefaultFormat = "png";
            options.DefaultQuality = 85;
            options.EnableCache = true;
            options.CacheSize = 100;
            options.CacheExpiry = TimeSpan.FromMinutes(30);
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            options.TempDirectory = Path.GetTempPath();
            options.SupportedFormats = new List<string> { "jpeg", "jpg", "png", "gif", "webp", "bmp" };
        });
        
        services.AddSingleton<ImageService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task ResizeImage(ImageService service, string[] arguments)
    {
        if (arguments.Length < 3)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件和尺寸");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var size = arguments[2];
        
        Console.WriteLine($"调整图像大小: {inputFile} -> {outputFile}, 尺寸: {size}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ResizeImageAsync(inputFile, outputFile, size);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"输入尺寸: {result.InputWidth}x{result.InputHeight}");
            Console.WriteLine($"输出尺寸: {result.OutputWidth}x{result.OutputHeight}");
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task ConvertImage(ImageService service, string[] arguments)
    {
        if (arguments.Length < 3)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件和格式");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var format = arguments[2];
        
        Console.WriteLine($"转换图像格式: {inputFile} -> {outputFile}, 格式: {format}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ConvertImageAsync(inputFile, outputFile, format);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"输入格式: {result.InputFormat}");
            Console.WriteLine($"输出格式: {result.OutputFormat}");
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task CropImage(ImageService service, string[] arguments)
    {
        if (arguments.Length < 5)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件、X、Y、宽度和高度");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var x = int.Parse(arguments[2]);
        var y = int.Parse(arguments[3]);
        var width = int.Parse(arguments[4]);
        var height = int.Parse(arguments[5]);
        
        Console.WriteLine($"裁剪图像: {inputFile} -> {outputFile}, 区域: {x},{y},{width}x{height}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.CropImageAsync(inputFile, outputFile, x, y, width, height);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"输出尺寸: {result.OutputWidth}x{result.OutputHeight}");
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task RotateImage(ImageService service, string[] arguments)
    {
        if (arguments.Length < 3)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件和角度");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var angle = float.Parse(arguments[2]);
        
        Console.WriteLine($"旋转图像: {inputFile} -> {outputFile}, 角度: {angle}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.RotateImageAsync(inputFile, outputFile, angle);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task FlipImage(ImageService service, string[] arguments)
    {
        if (arguments.Length < 3)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件和方向");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var direction = arguments[2];
        
        Console.WriteLine($"翻转图像: {inputFile} -> {outputFile}, 方向: {direction}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.FlipImageAsync(inputFile, outputFile, direction);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task AddWatermark(ImageService service, string[] arguments)
    {
        if (arguments.Length < 4)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件、水印文本和位置");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var watermark = arguments[2];
        var position = arguments[3];
        
        Console.WriteLine($"添加水印: {inputFile} -> {outputFile}, 文本: {watermark}, 位置: {position}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.AddWatermarkAsync(inputFile, outputFile, watermark, position);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task ApplyFilter(ImageService service, string[] arguments)
    {
        if (arguments.Length < 3)
        {
            Console.WriteLine("错误: 请提供输入文件、输出文件和滤镜类型");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var filter = arguments[2];
        
        Console.WriteLine($"应用滤镜: {inputFile} -> {outputFile}, 滤镜: {filter}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ApplyFilterAsync(inputFile, outputFile, filter);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"文件大小: {result.OutputSize} bytes");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task GetMetadata(ImageService service, string[] arguments)
    {
        if (arguments.Length < 1)
        {
            Console.WriteLine("错误: 请提供图像文件");
            return;
        }
        
        var inputFile = arguments[0];
        Console.WriteLine($"获取图像元数据: {inputFile}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.GetMetadataAsync(inputFile);
        stopwatch.Stop();
        
        Console.WriteLine($"元数据获取完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"格式: {result.Format}");
            Console.WriteLine($"尺寸: {result.Width}x{result.Height}");
            Console.WriteLine($"文件大小: {result.FileSize} bytes");
            Console.WriteLine($"创建时间: {result.CreationTime}");
            Console.WriteLine($"修改时间: {result.LastWriteTime}");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task OptimizeImage(ImageService service, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供输入文件和输出文件");
            return;
        }
        
        var inputFile = arguments[0];
        var outputFile = arguments[1];
        var quality = arguments.Length > 2 ? int.Parse(arguments[2]) : 85;
        
        Console.WriteLine($"优化图像: {inputFile} -> {outputFile}, 质量: {quality}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.OptimizeImageAsync(inputFile, outputFile, quality);
        stopwatch.Stop();
        
        Console.WriteLine($"图像处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"原始大小: {result.InputSize} bytes");
            Console.WriteLine($"优化大小: {result.OutputSize} bytes");
            Console.WriteLine($"压缩率: {result.CompressionRatio:P2}");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task BatchProcess(ImageService service, string[] arguments)
    {
        if (arguments.Length < 3)
        {
            Console.WriteLine("错误: 请提供输入目录、输出目录和操作");
            return;
        }
        
        var inputDir = arguments[0];
        var outputDir = arguments[1];
        var operation = arguments[2];
        var parameters = arguments.Skip(3).ToArray();
        
        Console.WriteLine($"批量处理: {inputDir} -> {outputDir}, 操作: {operation}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.BatchProcessAsync(inputDir, outputDir, operation, parameters);
        stopwatch.Stop();
        
        Console.WriteLine($"批量处理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理状态: {(result.Success ? "成功" : "失败")}");
        
        if (result.Success)
        {
            Console.WriteLine($"处理文件数: {result.ProcessedFiles}");
            Console.WriteLine($"成功: {result.SuccessCount}");
            Console.WriteLine($"失败: {result.FailureCount}");
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task RunBenchmark(ImageService service, string[] arguments)
    {
        var operation = arguments.Length > 0 ? arguments[0] : "resize";
        var iterations = arguments.Length > 1 ? int.Parse(arguments[1]) : 10;
        
        Console.WriteLine($"运行基准测试: 操作={operation}, 迭代次数={iterations}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.RunBenchmarkAsync(operation, iterations);
        stopwatch.Stop();
        
        Console.WriteLine($"基准测试完成!");
        Console.WriteLine($"总用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"成功: {result.SuccessCount}");
        Console.WriteLine($"失败: {result.FailureCount}");
        Console.WriteLine($"平均每操作: {result.AverageTime:F3} ms");
        Console.WriteLine($"每秒操作数: {result.OperationsPerSecond:F2} ops/s");
    }
    
    private static void ShowConfig(ImageSettings settings)
    {
        Console.WriteLine("Image 配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"默认格式: {settings.DefaultFormat}");
        Console.WriteLine($"默认质量: {settings.DefaultQuality}");
        Console.WriteLine($"启用缓存: {settings.EnableCache}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"缓存过期: {settings.CacheExpiry}");
        Console.WriteLine($"启用并行处理: {settings.EnableParallelProcessing}");
        Console.WriteLine($"最大并行度: {settings.MaxDegreeOfParallelism}");
        Console.WriteLine($"临时目录: {settings.TempDirectory}");
        Console.WriteLine($"支持的格式: {string.Join(", ", settings.SupportedFormats)}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("Image AOT 引擎 命令帮助:");
        Console.WriteLine("=" * 60);
        Console.WriteLine("resize (r)      - 调整图像大小");
        Console.WriteLine("convert (c)     - 转换图像格式");
        Console.WriteLine("crop (cr)       - 裁剪图像");
        Console.WriteLine("rotate (ro)     - 旋转图像");
        Console.WriteLine("flip (f)        - 翻转图像");
        Console.WriteLine("watermark (w)   - 添加水印");
        Console.WriteLine("filter (fi)     - 应用滤镜");
        Console.WriteLine("metadata (m)    - 获取图像元数据");
        Console.WriteLine("optimize (o)    - 优化图像");
        Console.WriteLine("batch (b)       - 批量处理图像");
        Console.WriteLine("benchmark (bm)  - 运行基准测试");
        Console.WriteLine("config (co)     - 显示配置信息");
        Console.WriteLine("help (h, ?)     - 显示帮助信息");
    }
}

public class ImageSettings
{
    public string DefaultFormat { get; set; } = "png";
    public int DefaultQuality { get; set; } = 85;
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 100;
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(30);
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public string TempDirectory { get; set; } = Path.GetTempPath();
    public List<string> SupportedFormats { get; set; } = new List<string> { "jpeg", "jpg", "png", "gif", "webp", "bmp" };
}

public class ImageProcessingResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public int InputWidth { get; set; }
    public int InputHeight { get; set; }
    public int OutputWidth { get; set; }
    public int OutputHeight { get; set; }
    public long OutputSize { get; set; }
    public string InputFormat { get; set; }
    public string OutputFormat { get; set; }
}

public class ImageOptimizationResult : ImageProcessingResult
{
    public long InputSize { get; set; }
    public double CompressionRatio { get; set; }
}

public class ImageMetadataResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string Format { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long FileSize { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime LastWriteTime { get; set; }
}

public class BatchProcessingResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public int ProcessedFiles { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
}

public class BenchmarkResult
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public double AverageTime { get; set; }
    public double OperationsPerSecond { get; set; }
}

public class ImageService : IAsyncDisposable
{
    private readonly ILogger<ImageService> _logger;
    private readonly ImageSettings _settings;
    private readonly MemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly FontCollection _fontCollection;
    private readonly object _cacheLock = new();
    
    public ImageService(ILogger<ImageService> logger, IOptions<ImageSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
        
        // 初始化缓存
        var cacheOptions = new MemoryCacheOptions {
            SizeLimit = _settings.CacheSize
        };
        _cache = new MemoryCache(cacheOptions);
        
        // 初始化 HTTP 客户端
        _httpClient = new HttpClient {
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // 初始化字体集合
        _fontCollection = new FontCollection();
        
        _logger.LogInformation("Image 服务初始化成功");
    }
    
    public async Task<ImageProcessingResult> ResizeImageAsync(string inputFile, string outputFile, string size)
    {
        try
        {
            _logger.LogInformation($"调整图像大小: {inputFile} -> {outputFile}, 尺寸: {size}");
            
            // 解析尺寸
            var sizeParts = size.Split('x');
            if (sizeParts.Length != 2 || !int.TryParse(sizeParts[0], out int width) || !int.TryParse(sizeParts[1], out int height))
            {
                return new ImageProcessingResult {
                    Success = false,
                    Error = "无效的尺寸格式，请使用 WxH 格式"
                };
            }
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            var inputWidth = image.Width;
            var inputHeight = image.Height;
            
            // 调整大小
            image.Mutate(x => x.Resize(width, height));
            
            // 保存图像
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                InputWidth = inputWidth,
                InputHeight = inputHeight,
                OutputWidth = width,
                OutputHeight = height,
                OutputSize = outputSize,
                InputFormat = Path.GetExtension(inputFile).Trim('.'),
                OutputFormat = Path.GetExtension(outputFile).Trim('.')
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调整图像大小失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageProcessingResult> ConvertImageAsync(string inputFile, string outputFile, string format)
    {
        try
        {
            _logger.LogInformation($"转换图像格式: {inputFile} -> {outputFile}, 格式: {format}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            var inputWidth = image.Width;
            var inputHeight = image.Height;
            
            // 保存为指定格式
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                InputWidth = inputWidth,
                InputHeight = inputHeight,
                OutputWidth = inputWidth,
                OutputHeight = inputHeight,
                OutputSize = outputSize,
                InputFormat = Path.GetExtension(inputFile).Trim('.'),
                OutputFormat = format
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "转换图像格式失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageProcessingResult> CropImageAsync(string inputFile, string outputFile, int x, int y, int width, int height)
    {
        try
        {
            _logger.LogInformation($"裁剪图像: {inputFile} -> {outputFile}, 区域: {x},{y},{width}x{height}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            
            // 裁剪图像
            image.Mutate(x => x.Crop(new Rectangle(x, y, width, height)));
            
            // 保存图像
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                InputWidth = image.Width,
                InputHeight = image.Height,
                OutputWidth = width,
                OutputHeight = height,
                OutputSize = outputSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "裁剪图像失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageProcessingResult> RotateImageAsync(string inputFile, string outputFile, float angle)
    {
        try
        {
            _logger.LogInformation($"旋转图像: {inputFile} -> {outputFile}, 角度: {angle}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            var inputWidth = image.Width;
            var inputHeight = image.Height;
            
            // 旋转图像
            image.Mutate(x => x.Rotate(angle));
            
            // 保存图像
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                InputWidth = inputWidth,
                InputHeight = inputHeight,
                OutputWidth = image.Width,
                OutputHeight = image.Height,
                OutputSize = outputSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "旋转图像失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageProcessingResult> FlipImageAsync(string inputFile, string outputFile, string direction)
    {
        try
        {
            _logger.LogInformation($"翻转图像: {inputFile} -> {outputFile}, 方向: {direction}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            var inputWidth = image.Width;
            var inputHeight = image.Height;
            
            // 翻转图像
            switch (direction.ToLower())
            {
                case "horizontal":
                case "h":
                    image.Mutate(x => x.Flip(FlipMode.Horizontal));
                    break;
                case "vertical":
                case "v":
                    image.Mutate(x => x.Flip(FlipMode.Vertical));
                    break;
                default:
                    return new ImageProcessingResult {
                        Success = false,
                        Error = "无效的方向，请使用 horizontal 或 vertical"
                    };
            }
            
            // 保存图像
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                InputWidth = inputWidth,
                InputHeight = inputHeight,
                OutputWidth = inputWidth,
                OutputHeight = inputHeight,
                OutputSize = outputSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "翻转图像失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageProcessingResult> AddWatermarkAsync(string inputFile, string outputFile, string watermark, string position)
    {
        try
        {
            _logger.LogInformation($"添加水印: {inputFile} -> {outputFile}, 文本: {watermark}, 位置: {position}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            
            // 获取字体
            var font = GetDefaultFont();
            
            // 计算水印位置
            var textOptions = new TextOptions {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            // 根据位置设置对齐方式
            switch (position.ToLower())
            {
                case "top-left":
                    textOptions.HorizontalAlignment = HorizontalAlignment.Left;
                    textOptions.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case "top-right":
                    textOptions.HorizontalAlignment = HorizontalAlignment.Right;
                    textOptions.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case "bottom-left":
                    textOptions.HorizontalAlignment = HorizontalAlignment.Left;
                    textOptions.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case "bottom-right":
                    textOptions.HorizontalAlignment = HorizontalAlignment.Right;
                    textOptions.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case "center":
                default:
                    textOptions.HorizontalAlignment = HorizontalAlignment.Center;
                    textOptions.VerticalAlignment = VerticalAlignment.Center;
                    break;
            }
            
            // 添加水印
            image.Mutate(x => x.DrawText(textOptions, watermark, Color.White, new PointF(0, 0)));
            
            // 保存图像
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                OutputSize = outputSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加水印失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageProcessingResult> ApplyFilterAsync(string inputFile, string outputFile, string filter)
    {
        try
        {
            _logger.LogInformation($"应用滤镜: {inputFile} -> {outputFile}, 滤镜: {filter}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            
            // 应用滤镜
            switch (filter.ToLower())
            {
                case "grayscale":
                case "gray":
                    image.Mutate(x => x.Grayscale());
                    break;
                case "sepia":
                    image.Mutate(x => x.Sepia());
                    break;
                case "blur":
                    image.Mutate(x => x.GaussianBlur(5));
                    break;
                case "sharpen":
                    image.Mutate(x => x.Sharpen(5));
                    break;
                case "contrast":
                    image.Mutate(x => x.AdjustContrast(1.5f));
                    break;
                case "brightness":
                    image.Mutate(x => x.AdjustBrightness(1.2f));
                    break;
                default:
                    return new ImageProcessingResult {
                        Success = false,
                        Error = "不支持的滤镜类型"
                    };
            }
            
            // 保存图像
            await SaveImageAsync(image, outputFile);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            return new ImageProcessingResult {
                Success = true,
                OutputSize = outputSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "应用滤镜失败");
            return new ImageProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageMetadataResult> GetMetadataAsync(string inputFile)
    {
        try
        {
            _logger.LogInformation($"获取图像元数据: {inputFile}");
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            
            // 获取文件信息
            var fileInfo = new FileInfo(inputFile);
            
            return new ImageMetadataResult {
                Success = true,
                Format = Path.GetExtension(inputFile).Trim('.'),
                Width = image.Width,
                Height = image.Height,
                FileSize = fileInfo.Length,
                CreationTime = fileInfo.CreationTime,
                LastWriteTime = fileInfo.LastWriteTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取图像元数据失败");
            return new ImageMetadataResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<ImageOptimizationResult> OptimizeImageAsync(string inputFile, string outputFile, int quality)
    {
        try
        {
            _logger.LogInformation($"优化图像: {inputFile} -> {outputFile}, 质量: {quality}");
            
            // 获取输入文件大小
            var inputSize = new FileInfo(inputFile).Length;
            
            // 读取图像
            using var image = await ReadImageAsync(inputFile);
            
            // 保存图像（优化）
            await SaveImageAsync(image, outputFile, quality);
            
            // 获取输出文件大小
            var outputSize = new FileInfo(outputFile).Length;
            
            // 计算压缩率
            var compressionRatio = 1.0 - (double)outputSize / inputSize;
            
            return new ImageOptimizationResult {
                Success = true,
                InputSize = inputSize,
                OutputSize = outputSize,
                CompressionRatio = compressionRatio
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "优化图像失败");
            return new ImageOptimizationResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<BatchProcessingResult> BatchProcessAsync(string inputDir, string outputDir, string operation, string[] parameters)
    {
        try
        {
            _logger.LogInformation($"批量处理: {inputDir} -> {outputDir}, 操作: {operation}");
            
            // 确保输出目录存在
            Directory.CreateDirectory(outputDir);
            
            // 获取输入文件
            var inputFiles = Directory.GetFiles(inputDir)
                .Where(f => _settings.SupportedFormats.Contains(Path.GetExtension(f).Trim('.').ToLower()))
                .ToList();
            
            int processedFiles = 0;
            int successCount = 0;
            int failureCount = 0;
            
            // 并行处理文件
            if (_settings.EnableParallelProcessing)
            {
                await Parallel.ForEachAsync(inputFiles, new ParallelOptions {
                    MaxDegreeOfParallelism = _settings.MaxDegreeOfParallelism
                }, async (inputFile, cancellationToken) => {
                    var outputFile = Path.Combine(outputDir, Path.GetFileName(inputFile));
                    bool success = false;
                    
                    try
                    {
                        switch (operation.ToLower())
                        {
                            case "resize":
                                var size = parameters.Length > 0 ? parameters[0] : "800x600";
                                var resizeResult = await ResizeImageAsync(inputFile, outputFile, size);
                                success = resizeResult.Success;
                                break;
                            case "convert":
                                var format = parameters.Length > 0 ? parameters[0] : "jpg";
                                var outputFileWithFormat = Path.ChangeExtension(outputFile, format);
                                var convertResult = await ConvertImageAsync(inputFile, outputFileWithFormat, format);
                                success = convertResult.Success;
                                break;
                            case "optimize":
                                var quality = parameters.Length > 0 ? int.Parse(parameters[0]) : 85;
                                var optimizeResult = await OptimizeImageAsync(inputFile, outputFile, quality);
                                success = optimizeResult.Success;
                                break;
                            default:
                                _logger.LogError($"不支持的批量操作: {operation}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "批量处理文件失败: {inputFile}");
                    }
                    
                    Interlocked.Increment(ref processedFiles);
                    if (success)
                    {
                        Interlocked.Increment(ref successCount);
                    }
                    else
                    {
                        Interlocked.Increment(ref failureCount);
                    }
                });
            }
            else
            {
                // 串行处理
                foreach (var inputFile in inputFiles)
                {
                    var outputFile = Path.Combine(outputDir, Path.GetFileName(inputFile));
                    bool success = false;
                    
                    try
                    {
                        switch (operation.ToLower())
                        {
                            case "resize":
                                var size = parameters.Length > 0 ? parameters[0] : "800x600";
                                var resizeResult = await ResizeImageAsync(inputFile, outputFile, size);
                                success = resizeResult.Success;
                                break;
                            case "convert":
                                var format = parameters.Length > 0 ? parameters[0] : "jpg";
                                var outputFileWithFormat = Path.ChangeExtension(outputFile, format);
                                var convertResult = await ConvertImageAsync(inputFile, outputFileWithFormat, format);
                                success = convertResult.Success;
                                break;
                            case "optimize":
                                var quality = parameters.Length > 0 ? int.Parse(parameters[0]) : 85;
                                var optimizeResult = await OptimizeImageAsync(inputFile, outputFile, quality);
                                success = optimizeResult.Success;
                                break;
                            default:
                                _logger.LogError($"不支持的批量操作: {operation}");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "批量处理文件失败: {inputFile}");
                    }
                    
                    processedFiles++;
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failureCount++;
                    }
                }
            }
            
            return new BatchProcessingResult {
                Success = true,
                ProcessedFiles = processedFiles,
                SuccessCount = successCount,
                FailureCount = failureCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量处理失败");
            return new BatchProcessingResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<BenchmarkResult> RunBenchmarkAsync(string operation, int iterations)
    {
        try
        {
            _logger.LogInformation($"运行基准测试: 操作={operation}, 迭代次数={iterations}");
            
            // 创建测试图像
            var testImagePath = Path.Combine(_settings.TempDirectory, "test_benchmark.jpg");
            await CreateTestImageAsync(testImagePath);
            
            int successCount = 0;
            int failureCount = 0;
            var totalTime = 0.0;
            
            for (int i = 0; i < iterations; i++)
            {
                var outputPath = Path.Combine(_settings.TempDirectory, $"test_output_{i}.jpg");
                
                var stopwatch = Stopwatch.StartNew();
                bool success = false;
                
                try
                {
                    switch (operation.ToLower())
                    {
                        case "resize":
                            var resizeResult = await ResizeImageAsync(testImagePath, outputPath, "800x600");
                            success = resizeResult.Success;
                            break;
                        case "convert":
                            var convertResult = await ConvertImageAsync(testImagePath, outputPath, "png");
                            success = convertResult.Success;
                            break;
                        case "optimize":
                            var optimizeResult = await OptimizeImageAsync(testImagePath, outputPath, 85);
                            success = optimizeResult.Success;
                            break;
                        default:
                            _logger.LogError($"不支持的基准测试操作: {operation}");
                            break;
                    }
                }
                catch
                {
                    success = false;
                }
                
                stopwatch.Stop();
                totalTime += stopwatch.Elapsed.TotalMilliseconds;
                
                if (success)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                }
                
                // 清理输出文件
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
            }
            
            // 清理测试图像
            if (File.Exists(testImagePath))
            {
                File.Delete(testImagePath);
            }
            
            var averageTime = totalTime / iterations;
            var operationsPerSecond = 1000.0 / averageTime;
            
            return new BenchmarkResult {
                SuccessCount = successCount,
                FailureCount = failureCount,
                AverageTime = averageTime,
                OperationsPerSecond = operationsPerSecond
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "运行基准测试失败");
            return new BenchmarkResult {
                SuccessCount = 0,
                FailureCount = iterations,
                AverageTime = 0,
                OperationsPerSecond = 0
            };
        }
    }
    
    private async Task<Image> ReadImageAsync(string filePath)
    {
        // 检查缓存
        var cacheKey = $"image:{filePath}:{File.GetLastWriteTime(filePath)}";
        
        if (_settings.EnableCache)
        {
            if (_cache.TryGetValue(cacheKey, out Image cachedImage))
            {
                _logger.LogInformation($"从缓存读取图像: {filePath}");
                return cachedImage.Clone();
            }
        }
        
        _logger.LogInformation($"读取图像: {filePath}");
        
        // 读取图像文件
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
        var image = await Image.LoadAsync(stream);
        
        // 缓存图像
        if (_settings.EnableCache)
        {
            lock (_cacheLock)
            {
                _cache.Set(cacheKey, image.Clone(), new MemoryCacheEntryOptions {
                    Size = 1,
                    AbsoluteExpirationRelativeToNow = _settings.CacheExpiry
                });
            }
        }
        
        return image;
    }
    
    private async Task SaveImageAsync(Image image, string outputFile, int quality = -1)
    {
        _logger.LogInformation($"保存图像: {outputFile}");
        
        var extension = Path.GetExtension(outputFile).Trim('.').ToLower();
        IImageEncoder encoder = null;
        
        // 根据文件扩展名选择编码器
        switch (extension)
        {
            case "jpg":
            case "jpeg":
                encoder = new JpegEncoder {
                    Quality = quality > 0 ? quality : _settings.DefaultQuality
                };
                break;
            case "png":
                encoder = new PngEncoder {
                    CompressionLevel = PngCompressionLevel.Best
                };
                break;
            case "gif":
                encoder = new GifEncoder();
                break;
            case "webp":
                encoder = new WebpEncoder {
                    Quality = quality > 0 ? quality : _settings.DefaultQuality
                };
                break;
            default:
                // 默认使用 JPEG 编码器
                encoder = new JpegEncoder {
                    Quality = quality > 0 ? quality : _settings.DefaultQuality
                };
                break;
        }
        
        // 保存图像
        using var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
        await image.SaveAsync(stream, encoder);
    }
    
    private async Task CreateTestImageAsync(string filePath)
    {
        _logger.LogInformation($"创建测试图像: {filePath}");
        
        // 创建一个 1000x1000 的测试图像
        using var image = new Image<Rgba32>(1000, 1000);
        
        // 填充背景色
        image.Mutate(x => x.Fill(Color.Blue));
        
        // 添加一些文本
        var font = GetDefaultFont();
        var textOptions = new TextOptions {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        image.Mutate(x => x.DrawText(textOptions, "Test Image", Color.White, new PointF(500, 500)));
        
        // 保存测试图像
        await SaveImageAsync(image, filePath);
    }
    
    private Font GetDefaultFont()
    {
        // 使用默认字体
        return SystemFonts.CreateFont("Arial", 24);
    }
    
    public async ValueTask DisposeAsync()
    {
        _cache.Dispose();
        await _httpClient.DisposeAsync();
        _logger.LogInformation("Image 服务已释放");
    }
}
