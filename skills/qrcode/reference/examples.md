# qrcode - 使用示例

## 快速开始

### 1. 基本使用示例

#### 生成二维码

```csharp
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("二维码生成示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
        
        // 生成二维码
        var options = new QrCodeGenerationOptions
        {
            Content = "https://example.com",
            Size = 256,
            ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
        };
        
        var qrCodeBytes = await qrCodeGenerator.GenerateQrCodeAsync(options);
        
        // 保存到文件
        File.WriteAllBytes("qrcode.png", qrCodeBytes);
        Console.WriteLine("二维码已生成并保存为 qrcode.png");
        
        // 显示文件大小
        Console.WriteLine($"二维码文件大小: {qrCodeBytes.Length / 1024:F2} KB");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

#### 生成条码

```csharp
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("条码生成示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var barcodeGenerator = serviceProvider.GetRequiredService<IBarcodeGenerator>();
        
        // 生成CODE_128条码
        var options = new BarcodeGenerationOptions
        {
            Content = "123456789012",
            Format = BarcodeFormat.CODE_128,
            Width = 300,
            Height = 100
        };
        
        var barcodeBytes = await barcodeGenerator.GenerateBarcodeAsync(options);
        
        // 保存到文件
        File.WriteAllBytes("barcode.png", barcodeBytes);
        Console.WriteLine("条码已生成并保存为 barcode.png");
        
        // 显示文件大小
        Console.WriteLine($"条码文件大小: {barcodeBytes.Length / 1024:F2} KB");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

#### 自定义服务配置

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("二维码服务高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置二维码服务设置
        builder.Configure<QrCodeServiceOptions>(options => {
            options.EnableCaching = true;
            options.CacheSize = 200;
            options.DefaultErrorCorrectionLevel = ErrorCorrectionLevel.High;
            options.DefaultQRCodeSize = 512;
            options.EnableAsyncProcessing = true;
            options.MaxConcurrentOperations = 15;
            options.EnableDetailedLogging = true;
            options.CacheExpirationMinutes = 60;
        });
        
        // 注册服务
        builder.AddQrCodeServices();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<QrCodeServiceOptions>>().Value;
        Console.WriteLine($"配置信息: 缓存={settings.EnableCaching}, 缓存大小={settings.CacheSize}");
        Console.WriteLine($"默认错误纠正级别={settings.DefaultErrorCorrectionLevel}, 默认二维码大小={settings.DefaultQRCodeSize}");
        Console.WriteLine($"最大并发操作数={settings.MaxConcurrentOperations}, 缓存过期时间={settings.CacheExpirationMinutes}分钟");
        
        // 使用服务
        var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
        var options = new QrCodeGenerationOptions
        {
            Content = "https://example.com/advanced",
            Size = settings.DefaultQRCodeSize,
            ErrorCorrectionLevel = settings.DefaultErrorCorrectionLevel
        };
        
        var qrCodeBytes = await qrCodeGenerator.GenerateQrCodeAsync(options);
        Console.WriteLine($"生成的二维码大小: {qrCodeBytes.Length / 1024:F2} KB");
        Console.WriteLine("高级配置示例执行完成");
    }
}
```

### 3. 性能优化示例

#### 批量生成二维码

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("批量生成二维码性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
        
        // 准备批量生成的选项
        var optionsList = new List<QrCodeGenerationOptions>();
        for (int i = 0; i < 100; i++)
        {
            optionsList.Add(new QrCodeGenerationOptions
            {
                Content = $"https://example.com/item/{i}",
                Size = 200,
                ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
            });
        }
        
        // 性能测试
        var stopwatch = Stopwatch.StartNew();
        
        // 批量生成
        var results = await qrCodeGenerator.GenerateQrCodesAsync(optionsList);
        
        stopwatch.Stop();
        Console.WriteLine($"批量生成 {results.Length} 个二维码耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每个二维码生成时间: {stopwatch.Elapsed.TotalMilliseconds / results.Length:F3} ms");
        
        // 保存第一个和最后一个二维码作为示例
        if (results.Length > 0)
        {
            File.WriteAllBytes("batch_qrcode_0.png", results[0]);
            File.WriteAllBytes($"batch_qrcode_{results.Length - 1}.png", results[results.Length - 1]);
            Console.WriteLine("已保存示例二维码文件");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.Configure<QrCodeServiceOptions>(options => {
            options.EnableCaching = true;
            options.CacheSize = 1000;
            options.EnableAsyncProcessing = true;
            options.MaxConcurrentOperations = 10;
        });
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

#### 缓存性能测试

```csharp
using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("缓存性能测试示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
        
        // 测试内容
        var testContent = "https://example.com/cache-test";
        var options = new QrCodeGenerationOptions
        {
            Content = testContent,
            Size = 256,
            ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
        };
        
        // 第一次生成（无缓存）
        var stopwatch = Stopwatch.StartNew();
        var firstResult = await qrCodeGenerator.GenerateQrCodeAsync(options);
        stopwatch.Stop();
        Console.WriteLine($"第一次生成（无缓存）耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 第二次生成（有缓存）
        stopwatch.Restart();
        var cachedResult = await qrCodeGenerator.GenerateQrCodeAsync(options);
        stopwatch.Stop();
        Console.WriteLine($"第二次生成（有缓存）耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        // 验证结果是否相同
        var resultsMatch = firstResult.Length == cachedResult.Length;
        Console.WriteLine($"缓存结果验证: {(resultsMatch ? "成功" : "失败")}");
        
        // 性能提升百分比
        if (stopwatch.Elapsed.TotalMilliseconds > 0)
        {
            var improvementPercent = ((stopwatch.Elapsed.TotalMilliseconds - stopwatch.Elapsed.TotalMilliseconds) / stopwatch.Elapsed.TotalMilliseconds) * 100;
            Console.WriteLine($"性能提升: {improvementPercent:F2}%");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.Configure<QrCodeServiceOptions>(options => {
            options.EnableCaching = true;
            options.CacheSize = 100;
        });
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

#### 输入验证错误处理

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
        
        try
        {
            // 测试空内容
            var emptyOptions = new QrCodeGenerationOptions
            {
                Content = string.Empty, // 空内容
                Size = 256,
                ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
            };
            
            Console.WriteLine("测试空内容...");
            var result = await qrCodeGenerator.GenerateQrCodeAsync(emptyOptions);
            Console.WriteLine("成功: 这行不应该执行");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"参数错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 测试无效尺寸
            var invalidSizeOptions = new QrCodeGenerationOptions
            {
                Content = "https://example.com",
                Size = 10, // 尺寸太小
                ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
            };
            
            Console.WriteLine("\n测试无效尺寸...");
            var result = await qrCodeGenerator.GenerateQrCodeAsync(invalidSizeOptions);
            Console.WriteLine("成功: 这行不应该执行");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"参数错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        Console.WriteLine("\n错误处理示例执行完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

### 5. 条码识别示例

#### 识别图像中的条码

```csharp
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("条码识别示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var barcodeReader = serviceProvider.GetRequiredService<IBarcodeReader>();
        
        try
        {
            // 读取图像文件
            if (!File.Exists("barcode.png"))
            {
                Console.WriteLine("错误: barcode.png 文件不存在，请先运行条码生成示例");
                return;
            }
            
            byte[] imageBytes = File.ReadAllBytes("barcode.png");
            
            // 识别条码
            var result = await barcodeReader.ReadBarcodeAsync(imageBytes);
            
            if (result.Success)
            {
                Console.WriteLine($"识别成功!");
                Console.WriteLine($"条码内容: {result.Content}");
                Console.WriteLine($"条码格式: {result.Format}");
                Console.WriteLine($"识别时间: {result.ProcessTimeMs:F2} ms");
            }
            else
            {
                Console.WriteLine("识别失败: 未找到有效的条码");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

### 6. 跨平台使用示例

#### Linux/macOS 环境使用

```csharp
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("跨平台使用示例");
        Console.WriteLine("=" * 50);
        
        // 检测操作系统
        var os = Environment.OSVersion.Platform;
        Console.WriteLine($"当前操作系统: {os}");
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
        
        // 生成二维码
        var options = new QrCodeGenerationOptions
        {
            Content = "https://example.com/cross-platform",
            Size = 256,
            ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
        };
        
        try
        {
            var qrCodeBytes = await qrCodeGenerator.GenerateQrCodeAsync(options);
            
            // 确定输出路径
            string outputPath = os == PlatformID.Unix || os == PlatformID.MacOSX
                ? "/tmp/qrcode_crossplatform.png"
                : "qrcode_crossplatform.png";
            
            // 保存到文件
            File.WriteAllBytes(outputPath, qrCodeBytes);
            Console.WriteLine($"二维码已生成并保存到: {outputPath}");
            Console.WriteLine($"文件大小: {qrCodeBytes.Length / 1024:F2} KB");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine("跨平台使用可能需要安装额外的依赖项");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddQrCodeServices();
        return builder.BuildServiceProvider();
    }
}
```

### 7. ASP.NET Core 集成示例

#### Web API 集成

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 配置二维码服务
builder.Services.Configure<QrCodeServiceOptions>(options => {
    options.EnableCaching = true;
    options.CacheSize = 100;
    options.DefaultErrorCorrectionLevel = ErrorCorrectionLevel.Medium;
    options.DefaultQRCodeSize = 256;
    options.EnableAsyncProcessing = true;
    options.MaxConcurrentOperations = 10;
});

// 注册二维码服务
builder.Services.AddQrCodeServices();

var app = builder.Build();

// 健康检查端点
app.MapGet("/health", () => "QR Code Service is healthy");

// 二维码生成端点
app.MapGet("/qrcode", async (HttpContext context, IQrCodeGenerator qrCodeGenerator) => {
    // 从查询参数获取内容
    var content = context.Request.Query["content"].ToString();
    if (string.IsNullOrEmpty(content))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Content parameter is required");
        return;
    }
    
    // 从查询参数获取尺寸
    var sizeStr = context.Request.Query["size"].ToString();
    int size = string.IsNullOrEmpty(sizeStr) ? 256 : int.Parse(sizeStr);
    
    // 从查询参数获取错误纠正级别
    var eccStr = context.Request.Query["ecc"].ToString();
    var ecc = string.IsNullOrEmpty(eccStr) ? ErrorCorrectionLevel.Medium : 
        Enum.Parse<ErrorCorrectionLevel>(eccStr, true);
    
    // 生成二维码
    var options = new QrCodeGenerationOptions
    {
        Content = content,
        Size = size,
        ErrorCorrectionLevel = ecc
    };
    
    var qrCodeBytes = await qrCodeGenerator.GenerateQrCodeAsync(options);
    
    // 返回图像
    context.Response.ContentType = "image/png";
    await context.Response.Body.WriteAsync(qrCodeBytes);
});

// 条码生成端点
app.MapGet("/barcode", async (HttpContext context, IBarcodeGenerator barcodeGenerator) => {
    // 从查询参数获取内容
    var content = context.Request.Query["content"].ToString();
    if (string.IsNullOrEmpty(content))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Content parameter is required");
        return;
    }
    
    // 从查询参数获取格式
    var formatStr = context.Request.Query["format"].ToString();
    var format = string.IsNullOrEmpty(formatStr) ? BarcodeFormat.CODE_128 : 
        Enum.Parse<BarcodeFormat>(formatStr, true);
    
    // 从查询参数获取尺寸
    var widthStr = context.Request.Query["width"].ToString();
    var heightStr = context.Request.Query["height"].ToString();
    int width = string.IsNullOrEmpty(widthStr) ? 300 : int.Parse(widthStr);
    int height = string.IsNullOrEmpty(heightStr) ? 100 : int.Parse(heightStr);
    
    // 生成条码
    var options = new BarcodeGenerationOptions
    {
        Content = content,
        Format = format,
        Width = width,
        Height = height
    };
    
    var barcodeBytes = await barcodeGenerator.GenerateBarcodeAsync(options);
    
    // 返回图像
    context.Response.ContentType = "image/png";
    await context.Response.Body.WriteAsync(barcodeBytes);
});

app.Run();
```

## 总结

上述示例展示了qrcode技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作（生成二维码和条码）
2. 配置高级选项（缓存、并发、错误纠正级别等）
3. 优化性能（批量生成、缓存使用）
4. 处理错误情况（输入验证、异常处理）
5. 在不同平台上使用（Windows、Linux、macOS）
6. 集成到ASP.NET Core应用程序中

系统设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 示例使用提示

1. **安装依赖项**：在运行示例前，请确保已安装所有必要的依赖项
   ```bash
   dotnet add package ZXing.Net --version 0.16.12
   dotnet add package SkiaSharp --version 2.88.6
   dotnet add package Microsoft.Extensions.DependencyInjection --version 10.0.0
   dotnet add package Microsoft.Extensions.Options --version 10.0.0
   dotnet add package Microsoft.Extensions.Logging --version 10.0.0
   ```

2. **AOT编译**：如需使用AOT编译，请在项目文件中添加以下配置
   ```xml
   <PublishAot>true</PublishAot>
   <TrimMode>partial</TrimMode>
   <ReadyToRun>true</ReadyToRun>
   ```

3. **性能调优**：根据实际使用场景调整缓存大小和并发度

4. **跨平台注意事项**：在Linux和macOS上可能需要安装额外的依赖项

5. **错误处理**：在生产环境中，建议添加适当的错误处理和日志记录
