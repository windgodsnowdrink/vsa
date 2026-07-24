#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Drawing.Common@8.0.6
#:package System.Threading.Tasks.Dataflow@8.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// 配置选项
public class OCROptions
{
    public bool Enabled { get; set; } = true;
    public int MaxImageSize { get; set; } = 4096;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public List<string> Languages { get; set; } = new List<string> { "zh-CN", "en-US" };
    public bool EnableImagePreprocessing { get; set; } = true;
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public int BatchSize { get; set; } = 10;
}

// OCR 服务接口
public interface IOCRService
{
    Task<IOCRResult> RecognizeTextAsync(string imagePath, CancellationToken cancellationToken = default);
    Task<IOCRResult> RecognizeTextAsync(Stream imageStream, CancellationToken cancellationToken = default);
    Task<IOCRResult> RecognizeTextAsync(Bitmap image, CancellationToken cancellationToken = default);
    Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths, CancellationToken cancellationToken = default);
}

// 图像处理器接口
public interface IImageProcessor
{
    Task<Bitmap> PreprocessAsync(Bitmap image, CancellationToken cancellationToken = default);
    Task<Bitmap> ResizeAsync(Bitmap image, int maxSize, CancellationToken cancellationToken = default);
    Task<Bitmap> ConvertToGrayscaleAsync(Bitmap image, CancellationToken cancellationToken = default);
    Task<Bitmap> EnhanceContrastAsync(Bitmap image, CancellationToken cancellationToken = default);
}

// 文本识别器接口
public interface ITextRecognizer
{
    Task<IOCRResult> RecognizeAsync(Bitmap image, IEnumerable<string> languages, CancellationToken cancellationToken = default);
}

// OCR 结果接口
public interface IOCRResult
{
    bool Success { get; }
    string ErrorMessage { get; }
    IEnumerable<string> TextLines { get; }
    IEnumerable<OCRTextLine> TextLineDetails { get; }
    double Confidence { get; }
    TimeSpan ProcessingTime { get; }
}

// OCR 文本行
public class OCRTextLine
{
    public string Text { get; set; }
    public double Confidence { get; set; }
    public Rectangle BoundingBox { get; set; }
    public int LineNumber { get; set; }
}

// OCR 结果实现
public class OCRResult : IOCRResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public IEnumerable<string> TextLines { get; set; } = Enumerable.Empty<string>();
    public IEnumerable<OCRTextLine> TextLineDetails { get; set; } = Enumerable.Empty<OCRTextLine>();
    public double Confidence { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}

// 图像处理器实现
public class ImageProcessor : IImageProcessor
{
    private readonly ILogger<ImageProcessor> _logger;

    public ImageProcessor(ILogger<ImageProcessor> logger)
    {
        _logger = logger;
    }

    public async Task<Bitmap> PreprocessAsync(Bitmap image, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始图像预处理");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 调整图像大小
        var resizedImage = await ResizeAsync(image, 2048, cancellationToken);

        // 转换为灰度图
        var grayscaleImage = await ConvertToGrayscaleAsync(resizedImage, cancellationToken);

        // 增强对比度
        var enhancedImage = await EnhanceContrastAsync(grayscaleImage, cancellationToken);

        stopwatch.Stop();
        _logger.LogInformation("图像预处理完成，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);

        return enhancedImage;
    }

    public async Task<Bitmap> ResizeAsync(Bitmap image, int maxSize, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            int width = image.Width;
            int height = image.Height;

            if (width <= maxSize && height <= maxSize)
            {
                return new Bitmap(image);
            }

            double aspectRatio = (double)width / height;
            int newWidth, newHeight;

            if (width > height)
            {
                newWidth = maxSize;
                newHeight = (int)(maxSize / aspectRatio);
            }
            else
            {
                newHeight = maxSize;
                newWidth = (int)(maxSize * aspectRatio);
            }

            var resizedBitmap = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(resizedBitmap))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return resizedBitmap;
        }, cancellationToken);
    }

    public async Task<Bitmap> ConvertToGrayscaleAsync(Bitmap image, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var grayscaleBitmap = new Bitmap(image.Width, image.Height);
            using (var graphics = Graphics.FromImage(grayscaleBitmap))
            {
                var colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });

                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return grayscaleBitmap;
        }, cancellationToken);
    }

    public async Task<Bitmap> EnhanceContrastAsync(Bitmap image, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var enhancedBitmap = new Bitmap(image.Width, image.Height);
            using (var graphics = Graphics.FromImage(enhancedBitmap))
            {
                graphics.DrawImage(image, 0, 0);
            }

            // 简化的对比度增强
            for (int y = 0; y < enhancedBitmap.Height; y++)
            {
                for (int x = 0; x < enhancedBitmap.Width; x++)
                {
                    var pixel = enhancedBitmap.GetPixel(x, y);
                    int brightness = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                    
                    // 对比度增强
                    int newBrightness = Math.Min(255, Math.Max(0, (brightness - 128) * 1.5 + 128));
                    var newPixel = Color.FromArgb(newBrightness, newBrightness, newBrightness);
                    enhancedBitmap.SetPixel(x, y, newPixel);
                }
            }

            return enhancedBitmap;
        }, cancellationToken);
    }
}

// 文本识别器实现
public class TextRecognizer : ITextRecognizer
{
    private readonly ILogger<TextRecognizer> _logger;

    public TextRecognizer(ILogger<TextRecognizer> logger)
    {
        _logger = logger;
    }

    public async Task<IOCRResult> RecognizeAsync(Bitmap image, IEnumerable<string> languages, CancellationToken cancellationToken = default)
    {
        var result = new OCRResult();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("开始文本识别");
            cancellationToken.ThrowIfCancellationRequested();

            // 模拟 OCR 识别过程
            // 实际项目中，这里应该集成真实的 OCR 引擎，如 Tesseract、Azure Cognitive Services 等
            await Task.Delay(1000, cancellationToken); // 模拟识别耗时

            // 模拟识别结果
            var textLines = new List<string>
            {
                "这是一段示例文本",
                "This is a sample text",
                "1234567890",
                "示例地址：北京市海淀区"
            };

            var textLineDetails = textLines.Select((text, index) => new OCRTextLine
            {
                Text = text,
                Confidence = 0.95,
                BoundingBox = new Rectangle(10, 10 + index * 30, image.Width - 20, 25),
                LineNumber = index + 1
            });

            result.Success = true;
            result.TextLines = textLines;
            result.TextLineDetails = textLineDetails;
            result.Confidence = 0.95;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文本识别失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            _logger.LogInformation("文本识别完成，耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }

        return result;
    }
}

// OCR 服务实现
public class OCRService : IOCRService
{
    private readonly IImageProcessor _imageProcessor;
    private readonly ITextRecognizer _textRecognizer;
    private readonly OCROptions _options;
    private readonly ILogger<OCRService> _logger;

    public OCRService(IImageProcessor imageProcessor, ITextRecognizer textRecognizer, IOptions<OCROptions> options, ILogger<OCRService> logger)
    {
        _imageProcessor = imageProcessor;
        _textRecognizer = textRecognizer;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IOCRResult> RecognizeTextAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始识别图像: {ImagePath}", imagePath);

        using (var bitmap = new Bitmap(imagePath))
        {
            return await RecognizeTextAsync(bitmap, cancellationToken);
        }
    }

    public async Task<IOCRResult> RecognizeTextAsync(Stream imageStream, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始识别流中的图像");

        using (var bitmap = new Bitmap(imageStream))
        {
            return await RecognizeTextAsync(bitmap, cancellationToken);
        }
    }

    public async Task<IOCRResult> RecognizeTextAsync(Bitmap image, CancellationToken cancellationToken = default)
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

            // 创建超时令牌
            using (var timeoutCts = new CancellationTokenSource(_options.Timeout))
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token))
            {
                var linkedToken = linkedCts.Token;

                // 图像预处理
                Bitmap processedImage = image;
                if (_options.EnableImagePreprocessing)
                {
                    processedImage = await _imageProcessor.PreprocessAsync(image, linkedToken);
                }
                else
                {
                    processedImage = new Bitmap(image);
                }

                try
                {
                    // 文本识别
                    result = (OCRResult)await _textRecognizer.RecognizeAsync(processedImage, _options.Languages, linkedToken);
                }
                finally
                {
                    if (processedImage != image)
                    {
                        processedImage.Dispose();
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("OCR 处理已取消或超时");
            result.Success = false;
            result.ErrorMessage = "处理已取消或超时";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OCR 处理失败");
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        finally
        {
            stopwatch.Stop();
            result.ProcessingTime = stopwatch.Elapsed;
            _logger.LogInformation("OCR 处理完成，总耗时: {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }

        return result;
    }

    public async Task<IEnumerable<IOCRResult>> RecognizeTextBatchAsync(IEnumerable<string> imagePaths, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("开始批量 OCR 处理，图像数量: {Count}", imagePaths.Count());

        if (_options.EnableParallelProcessing)
        {
            // 并行处理
            var results = await Task.WhenAll(
                imagePaths.Select(path => RecognizeTextAsync(path, cancellationToken))
            );
            return results;
        }
        else
        {
            // 串行处理
            var results = new List<IOCRResult>();
            foreach (var path in imagePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await RecognizeTextAsync(path, cancellationToken);
                results.Add(result);
            }
            return results;
        }
    }
}

// 依赖注入扩展
public static class OCRServiceCollectionExtensions
{
    public static IServiceCollection AddOCRServices(this IServiceCollection services, Action<OCROptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OCROptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IImageProcessor, ImageProcessor>();
        services.AddSingleton<ITextRecognizer, TextRecognizer>();
        services.AddSingleton<IOCRService, OCRService>();

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OCR 服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 配置 OCR 服务
        services.AddOCRServices(options =>
        {
            options.Enabled = true;
            options.MaxImageSize = 4096;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.Languages = new List<string> { "zh-CN", "en-US" };
            options.EnableImagePreprocessing = true;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount;
        });

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var ocrService = serviceProvider.GetRequiredService<IOCRService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例 1: 模拟图像识别
            Console.WriteLine("示例 1: 模拟图像识别");
            Console.WriteLine("创建模拟图像...");

            // 创建模拟图像
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
                    graphics.DrawString("示例地址：北京市海淀区", font, brush, 50, 200);
                }
            }

            // 识别图像
            Console.WriteLine("开始识别图像...");
            var result = await ocrService.RecognizeTextAsync(bitmap);

            // 显示结果
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

            // 示例 2: 批量处理
            Console.WriteLine("\n示例 2: 批量处理");
            Console.WriteLine("创建模拟图像路径列表...");

            var imagePaths = Enumerable.Range(1, 3).Select(i => $"image{i}.jpg");
            var batchResults = await ocrService.RecognizeTextBatchAsync(imagePaths);

            Console.WriteLine($"批量处理完成，处理了 {batchResults.Count()} 个图像");
            foreach (var batchResult in batchResults)
            {
                Console.WriteLine($"- 结果: {(batchResult.Success ? "成功" : "失败")}, 识别到 {batchResult.TextLines.Count()} 行文字");
            }

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
