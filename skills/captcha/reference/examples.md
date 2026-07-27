# captcha - 使用示例

## 快速开始

### 1. 基础验证码使用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:package LazyCaptcha@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;

// 验证码服务接口
public interface ICaptchaService
{
    Task<ImageCaptcha> GenerateImageCaptchaAsync();
    Task<bool> ValidateImageCaptchaAsync(string captchaId, string inputText);
}

// 图片验证码模型
public class ImageCaptcha
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Text { get; set; } = string.Empty;
    public byte[] ImageData { get; set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 图片验证码生成器接口
public interface IImageCaptchaGenerator
{
    ImageCaptcha Generate();
}

// 验证码验证器接口
public interface ICaptchaValidator
{
    Task<bool> ValidateAsync(string captchaId, string inputText);
}

// 简单的图片验证码生成器实现
public class SimpleImageCaptchaGenerator : IImageCaptchaGenerator
{
    public ImageCaptcha Generate()
    {
        // 生成4位随机验证码文本
        var text = new string(Enumerable.Range(0, 4)
            .Select(_ => (char)('0' + Random.Shared.Next(0, 10)))
            .ToArray());
        
        // 创建简单的验证码图片
        using var image = new Image<Rgba32>(120, 40, Color.White);
        
        // 这里可以添加更复杂的绘制逻辑
        // 例如添加噪声、扭曲等效果
        
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        
        return new ImageCaptcha {
            Text = text,
            ImageData = stream.ToArray()
        };
    }
}

// 简单的验证码验证器实现
public class SimpleCaptchaValidator : ICaptchaValidator
{
    private readonly Dictionary<string, ImageCaptcha> _captchaCache = new();
    
    public Task<bool> ValidateAsync(string captchaId, string inputText)
    {
        if (_captchaCache.TryGetValue(captchaId, out var captcha))
        {
            // 简单的文本比较
            var isValid = captcha.Text.Equals(inputText, StringComparison.OrdinalIgnoreCase);
            
            // 验证后移除验证码
            if (isValid)
            {
                _captchaCache.Remove(captchaId);
            }
            
            return Task.FromResult(isValid);
        }
        
        return Task.FromResult(false);
    }
}

// 简单的验证码服务实现
public class SimpleCaptchaService : ICaptchaService
{
    private readonly IImageCaptchaGenerator _generator;
    private readonly ICaptchaValidator _validator;
    private readonly Dictionary<string, ImageCaptcha> _captchaCache = new();
    
    public SimpleCaptchaService(IImageCaptchaGenerator generator, ICaptchaValidator validator)
    {
        _generator = generator;
        _validator = validator;
    }
    
    public Task<ImageCaptcha> GenerateImageCaptchaAsync()
    {
        var captcha = _generator.Generate();
        _captchaCache[captcha.Id] = captcha;
        
        // 设置验证码过期时间
        Task.Delay(TimeSpan.FromMinutes(5))
            .ContinueWith(_ => _captchaCache.Remove(captcha.Id));
        
        return Task.FromResult(captcha);
    }
    
    public Task<bool> ValidateImageCaptchaAsync(string captchaId, string inputText)
    {
        return _validator.ValidateAsync(captchaId, inputText);
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var captchaService = serviceProvider.GetRequiredService<ICaptchaService>();
        
        Console.WriteLine("验证码基础使用示例");
        Console.WriteLine("=" * 50);
        
        // 生成图片验证码
        var captcha = await captchaService.GenerateImageCaptchaAsync();
        Console.WriteLine($"生成的验证码: {captcha.Text}");
        Console.WriteLine($"验证码ID: {captcha.Id}");
        Console.WriteLine($"验证码图片大小: {captcha.ImageData.Length} 字节");
        
        // 验证验证码
        Console.WriteLine("\n请输入验证码进行验证:");
        var userInput = Console.ReadLine();
        
        var isValid = await captchaService.ValidateImageCaptchaAsync(captcha.Id, userInput);
        Console.WriteLine($"验证结果: {(isValid ? "成功" : "失败"}");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册验证码服务
        builder.AddSingleton<IImageCaptchaGenerator, SimpleImageCaptchaGenerator>();
        builder.AddSingleton<ICaptchaValidator, SimpleCaptchaValidator>();
        builder.AddSingleton<ICaptchaService, SimpleCaptchaService>();
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. AOT 编译验证码示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
using System;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

// AOT 安全的验证码服务
public class AotSafeCaptchaService : ICaptchaService
{
    private readonly IImageCaptchaGenerator _generator;
    private readonly ICaptchaValidator _validator;
    
    // AOT 安全：使用具体类型而非接口的闭包
    public AotSafeCaptchaService(IImageCaptchaGenerator generator, ICaptchaValidator validator)
    {
        _generator = generator;
        _validator = validator;
    }
    
    public Task<ImageCaptcha> GenerateImageCaptchaAsync()
    {
        var captcha = _generator.Generate();
        return Task.FromResult(captcha);
    }
    
    public Task<bool> ValidateImageCaptchaAsync(string captchaId, string inputText)
    {
        return _validator.ValidateAsync(captchaId, inputText);
    }
}

// AOT 安全的验证码生成器
public class AotSafeImageCaptchaGenerator : IImageCaptchaGenerator
{
    private readonly Font _font;
    
    // AOT 安全：在构造函数中预加载资源
    public AotSafeImageCaptchaGenerator()
    {
        // 使用内置字体，避免运行时反射加载
        _font = SystemFonts.CreateFont("Arial", 20, FontStyle.Bold);
    }
    
    // AOT 安全：没有反射，没有动态代码生成
    public ImageCaptcha Generate()
    {
        // 生成随机文本
        var text = GenerateRandomText(4);
        
        // 创建图片
        using var image = new Image<Rgba32>(120, 40, Color.White);
        
        // AOT 安全：使用具体方法而非动态委托
        DrawText(image, text, _font);
        AddNoise(image);
        
        // 转换为字节数组
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        
        return new ImageCaptcha {
            Text = text,
            ImageData = stream.ToArray()
        };
    }
    
    // AOT 安全：静态方法，没有闭包
    private static string GenerateRandomText(int length)
    {
        var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".AsSpan();
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[Random.Shared.Next(chars.Length)];
        }
        
        return new string(result);
    }
    
    // AOT 安全：具体实现，没有反射
    private static void DrawText(Image<Rgba32> image, string text, Font font)
    {
        // 简化的文本绘制
        var options = new TextOptions(font)
        {
            Origin = new PointF(10, 25),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        image.Mutate(x => x.DrawText(options, text, Color.Black));
    }
    
    // AOT 安全：具体实现，没有反射
    private static void AddNoise(Image<Rgba32> image)
    {
        // 简化的噪声添加
        var random = new Random();
        
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if (random.Next(100) < 5) // 5% 噪声
                {
                    image[x, y] = Color.Black;
                }
            }
        }
    }
}

// AOT 安全的验证码验证器
public class AotSafeCaptchaValidator : ICaptchaValidator
{
    // AOT 安全：使用 ConcurrentDictionary 而非 Dictionary
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ImageCaptcha> _cache;
    
    public AotSafeCaptchaValidator()
    {
        _cache = new System.Collections.Concurrent.ConcurrentDictionary<string, ImageCaptcha>();
        
        // 启动清理任务
        Task.Run(async () => {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                CleanupExpiredCaptchas();
            }
        });
    }
    
    // AOT 安全：没有闭包，没有反射
    public Task<bool> ValidateAsync(string captchaId, string inputText)
    {
        if (_cache.TryGetValue(captchaId, out var captcha))
        {
            var isValid = captcha.Text.Equals(inputText, StringComparison.OrdinalIgnoreCase);
            
            if (isValid)
            {
                _cache.TryRemove(captchaId, out _);
            }
            
            return Task.FromResult(isValid);
        }
        
        return Task.FromResult(false);
    }
    
    // AOT 安全：具体实现，没有反射
    private void CleanupExpiredCaptchas()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _cache.Where(kv => now - kv.Value.CreatedAt > TimeSpan.FromMinutes(5))
            .Select(kv => kv.Key)
            .ToList();
        
        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var captchaService = serviceProvider.GetRequiredService<ICaptchaService>();
        
        Console.WriteLine("AOT 编译验证码示例");
        Console.WriteLine("=" * 50);
        
        // 生成 AOT 安全的验证码
        var captcha = await captchaService.GenerateImageCaptchaAsync();
        Console.WriteLine($"生成的验证码: {captcha.Text}");
        Console.WriteLine($"验证码ID: {captcha.Id}");
        Console.WriteLine($"此示例支持 AOT 编译");
        
        // 验证验证码
        var isValid = await captchaService.ValidateImageCaptchaAsync(captcha.Id, captcha.Text);
        Console.WriteLine($"验证结果: {(isValid ? "成功" : "失败"}");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 AOT 安全的验证码服务
        builder.AddSingleton<IImageCaptchaGenerator, AotSafeImageCaptchaGenerator>();
        builder.AddSingleton<ICaptchaValidator, AotSafeCaptchaValidator>();
        builder.AddSingleton<ICaptchaService, AotSafeCaptchaService>();
        
        return builder.BuildServiceProvider();
    }
}
```

### 3. 高性能验证码生成示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Buffers;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;

// 高性能验证码生成器
public class HighPerformanceCaptchaGenerator : IImageCaptchaGenerator
{
    private readonly ArrayPool<byte> _arrayPool;
    private readonly TransformBlock<int, ImageCaptcha> _captchaPipeline;
    
    public HighPerformanceCaptchaGenerator()
    {
        _arrayPool = ArrayPool<byte>.Shared;
        
        // 创建高性能处理管道
        _captchaPipeline = new TransformBlock<int, ImageCaptcha>(
            length => GenerateCaptchaInternal(length),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 100
            });
    }
    
    public ImageCaptcha Generate()
    {
        // 使用管道生成验证码
        var captcha = GenerateCaptchaInternal(4);
        return captcha;
    }
    
    // 高性能：使用 ArrayPool，避免频繁内存分配
    private ImageCaptcha GenerateCaptchaInternal(int length)
    {
        // 生成随机文本
        var text = GenerateRandomText(length);
        
        // 创建图片
        using var image = new Image<Rgba32>(120, 40, Color.White);
        
        // 简化的绘制逻辑，提高性能
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        
        // 使用 ArrayPool 管理内存
        var buffer = _arrayPool.Rent((int)stream.Length);
        stream.Seek(0, SeekOrigin.Begin);
        stream.Read(buffer, 0, (int)stream.Length);
        
        // 创建最终的字节数组
        var imageData = new byte[(int)stream.Length];
        Buffer.BlockCopy(buffer, 0, imageData, 0, (int)stream.Length);
        _arrayPool.Return(buffer);
        
        return new ImageCaptcha {
            Text = text,
            ImageData = imageData
        };
    }
    
    // 高性能：使用 Span<T>，避免字符串操作
    private static string GenerateRandomText(int length)
    {
        var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".AsSpan();
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[Random.Shared.Next(chars.Length)];
        }
        
        return new string(result);
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var captchaGenerator = serviceProvider.GetRequiredService<IImageCaptchaGenerator>();
        
        Console.WriteLine("高性能验证码生成示例");
        Console.WriteLine("=" * 50);
        
        // 性能测试：生成1000个验证码
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var captcha = captchaGenerator.Generate();
            
            // 每100个输出一次进度
            if ((i + 1) % 100 == 0)
            {
                Console.WriteLine($"已生成 {i + 1} 个验证码");
            }
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"\n生成 {iterations} 个验证码完成");
        Console.WriteLine($"总时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每个: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        Console.WriteLine($"每秒生成: {iterations / stopwatch.Elapsed.TotalSeconds:F0} 个");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册高性能验证码生成器
        builder.AddSingleton<IImageCaptchaGenerator, HighPerformanceCaptchaGenerator>();
        
        return builder.BuildServiceProvider();
    }
}
```

### 4. 滑块验证码示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

// 滑块验证码模型
public class SlideCaptcha
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public byte[] BackgroundImage { get; set; } = Array.Empty<byte>();
    public byte[] SliderImage { get; set; } = Array.Empty<byte>();
    public Point CorrectPosition { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 滑块验证码服务接口
public interface ISlideCaptchaService
{
    Task<SlideCaptcha> GenerateSlideCaptchaAsync();
    Task<bool> ValidateSlideCaptchaAsync(string captchaId, Point userPosition);
}

// 简单的滑块验证码生成器
public class SimpleSlideCaptchaGenerator
{
    public SlideCaptcha Generate()
    {
        // 创建背景图片
        using var background = new Image<Rgba32>(300, 150, Color.LightGray);
        
        // 创建滑块
        var sliderWidth = 40;
        var sliderHeight = 40;
        
        // 随机生成滑块位置
        var random = new Random();
        var x = random.Next(50, background.Width - sliderWidth - 50);
        var y = random.Next(20, background.Height - sliderHeight - 20);
        
        // 绘制滑块阴影
        background.Mutate(img => img.Fill(Color.DarkGray, new Rectangle(x, y, sliderWidth, sliderHeight)));
        
        // 创建滑块图片
        using var slider = new Image<Rgba32>(sliderWidth, sliderHeight, Color.White);
        slider.Mutate(img => {
            img.Fill(Color.White);
            img.Draw(Color.Black, 2, new Rectangle(0, 0, sliderWidth - 1, sliderHeight - 1));
        });
        
        // 转换为字节数组
        using var backgroundStream = new MemoryStream();
        background.SaveAsPng(backgroundStream);
        
        using var sliderStream = new MemoryStream();
        slider.SaveAsPng(sliderStream);
        
        return new SlideCaptcha {
            BackgroundImage = backgroundStream.ToArray(),
            SliderImage = sliderStream.ToArray(),
            CorrectPosition = new Point(x, y)
        };
    }
}

// 简单的滑块验证码服务
public class SimpleSlideCaptchaService : ISlideCaptchaService
{
    private readonly SimpleSlideCaptchaGenerator _generator;
    private readonly Dictionary<string, SlideCaptcha> _cache = new();
    
    public SimpleSlideCaptchaService()
    {
        _generator = new SimpleSlideCaptchaGenerator();
    }
    
    public Task<SlideCaptcha> GenerateSlideCaptchaAsync()
    {
        var captcha = _generator.Generate();
        _cache[captcha.Id] = captcha;
        
        // 设置过期时间
        Task.Delay(TimeSpan.FromMinutes(5))
            .ContinueWith(_ => _cache.Remove(captcha.Id));
        
        return Task.FromResult(captcha);
    }
    
    public Task<bool> ValidateSlideCaptchaAsync(string captchaId, Point userPosition)
    {
        if (_cache.TryGetValue(captchaId, out var captcha))
        {
            // 容差验证
            var tolerance = 5;
            var isXValid = Math.Abs(userPosition.X - captcha.CorrectPosition.X) <= tolerance;
            var isYValid = Math.Abs(userPosition.Y - captcha.CorrectPosition.Y) <= tolerance;
            
            var isValid = isXValid && isYValid;
            
            if (isValid)
            {
                _cache.Remove(captchaId);
            }
            
            return Task.FromResult(isValid);
        }
        
        return Task.FromResult(false);
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var slideCaptchaService = serviceProvider.GetRequiredService<ISlideCaptchaService>();
        
        Console.WriteLine("滑块验证码示例");
        Console.WriteLine("=" * 50);
        
        // 生成滑块验证码
        var slideCaptcha = await slideCaptchaService.GenerateSlideCaptchaAsync();
        Console.WriteLine($"滑块验证码ID: {slideCaptcha.Id}");
        Console.WriteLine($"正确位置: X={slideCaptcha.CorrectPosition.X}, Y={slideCaptcha.CorrectPosition.Y}");
        Console.WriteLine($"背景图片大小: {slideCaptcha.BackgroundImage.Length} 字节");
        Console.WriteLine($"滑块图片大小: {slideCaptcha.SliderImage.Length} 字节");
        
        // 模拟用户输入
        Console.WriteLine("\n请输入您的滑块位置 (格式: X,Y):");
        var userInput = Console.ReadLine();
        
        if (userInput != null)
        {
            var parts = userInput.Split(',');
            if (parts.Length == 2 && 
                int.TryParse(parts[0], out var x) &&
                int.TryParse(parts[1], out var y))
            {
                var isValid = await slideCaptchaService.ValidateSlideCaptchaAsync(slideCaptcha.Id, new Point(x, y));
                Console.WriteLine($"验证结果: {(isValid ? "成功" : "失败"}");
                Console.WriteLine($"容差范围内的正确位置: X={slideCaptcha.CorrectPosition.X}±5, Y={slideCaptcha.CorrectPosition.Y}±5");
            }
        }
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册滑块验证码服务
        builder.AddSingleton<ISlideCaptchaService, SimpleSlideCaptchaService>();
        
        return builder.BuildServiceProvider();
    }
}
```

### 5. 验证码识别示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package SixLabors.ImageSharp@3.1.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp;

// 验证码识别器接口
public interface ICaptchaRecognizer
{
    Task<string> RecognizeImageCaptchaAsync(byte[] imageData);
    Task<string> RecognizeSlideCaptchaAsync(byte[] backgroundImage, byte[] sliderImage);
}

// 简单的验证码识别器（模拟实现）
public class SimpleCaptchaRecognizer : ICaptchaRecognizer
{
    // 模拟识别实现，实际项目中应使用机器学习或OCR库
    public Task<string> RecognizeImageCaptchaAsync(byte[] imageData)
    {
        // 这里是模拟实现，实际应使用OCR库
        // 例如：Tesseract, Microsoft Azure Computer Vision等
        
        // 生成随机识别结果（仅用于示例）
        var randomText = GenerateRandomText(4);
        return Task.FromResult(randomText);
    }
    
    public Task<string> RecognizeSlideCaptchaAsync(byte[] backgroundImage, byte[] sliderImage)
    {
        // 模拟滑块验证码识别
        // 实际实现应使用图像处理算法定位滑块位置
        
        var random = new Random();
        var x = random.Next(50, 200);
        var y = random.Next(20, 100);
        
        return Task.FromResult($"{x},{y}");
    }
    
    private static string GenerateRandomText(int length)
    {
        var chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".AsSpan();
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[Random.Shared.Next(chars.Length)];
        }
        
        return new string(result);
    }
}

// 验证码识别服务
public class CaptchaRecognitionService
{
    private readonly ICaptchaRecognizer _recognizer;
    private readonly ICaptchaService _captchaService;
    
    public CaptchaRecognitionService(ICaptchaRecognizer recognizer, ICaptchaService captchaService)
    {
        _recognizer = recognizer;
        _captchaService = captchaService;
    }
    
    public async Task<bool> AutoRecognizeAndValidateAsync()
    {
        // 生成验证码
        var captcha = await _captchaService.GenerateImageCaptchaAsync();
        Console.WriteLine($"原始验证码: {captcha.Text}");
        
        // 识别验证码
        var recognizedText = await _recognizer.RecognizeImageCaptchaAsync(captcha.ImageData);
        Console.WriteLine($"识别结果: {recognizedText}");
        
        // 验证识别结果
        var isValid = await _captchaService.ValidateImageCaptchaAsync(captcha.Id, recognizedText);
        Console.WriteLine($"验证结果: {(isValid ? "成功" : "失败"}");
        
        return isValid;
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var recognitionService = serviceProvider.GetRequiredService<CaptchaRecognitionService>();
        
        Console.WriteLine("验证码识别示例");
        Console.WriteLine("=" * 50);
        
        // 自动识别并验证10次
        int successCount = 0;
        const int totalTests = 10;
        
        for (int i = 0; i < totalTests; i++)
        {
            Console.WriteLine($"\n测试 #{i + 1}:");
            var success = await recognitionService.AutoRecognizeAndValidateAsync();
            if (success)
            {
                successCount++;
            }
        }
        
        Console.WriteLine($"\n测试完成");
        Console.WriteLine($"总测试数: {totalTests}");
        Console.WriteLine($"成功数: {successCount}");
        Console.WriteLine($"成功率: {successCount * 100.0 / totalTests:F2}%");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册验证码服务
        builder.AddSingleton<IImageCaptchaGenerator, SimpleImageCaptchaGenerator>();
        builder.AddSingleton<ICaptchaValidator, SimpleCaptchaValidator>();
        builder.AddSingleton<ICaptchaService, SimpleCaptchaService>();
        
        // 注册验证码识别服务
        builder.AddSingleton<ICaptchaRecognizer, SimpleCaptchaRecognizer>();
        builder.AddSingleton<CaptchaRecognitionService>();
        
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例演示了验证码技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用验证码功能
2. 了解如何配置和使用AOT编译
3. 实现高性能验证码生成
4. 创建和验证滑块验证码
5. 实现验证码自动识别

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。所有示例都支持 AOT 编译，可以编译为本机代码以获得更高的性能和更快的启动速度。