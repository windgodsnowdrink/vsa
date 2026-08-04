# HTML - 使用示例

## 快速开始

### 1. 基本用法示例

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
        var htmlService = serviceProvider.GetRequiredService<HtmlService>();
        
        Console.WriteLine("HTML 基本用法示例");
        Console.WriteLine("=" * 60);
        
        // 解析 HTML 文件
        var parseResult = await htmlService.ParseHtmlAsync("index.html");
        Console.WriteLine($"标题: {parseResult.Title}");
        Console.WriteLine($"链接数: {parseResult.Links.Count}");
        Console.WriteLine($"图片数: {parseResult.Images.Count}");
        Console.WriteLine($"脚本数: {parseResult.Scripts.Count}");
        
        // 清理 HTML
        var sanitizedHtml = await htmlService.SanitizeHtmlAsync("<p>安全的 <script>alert('危险')</script> HTML</p>");
        Console.WriteLine($"\n清理后的 HTML: {sanitizedHtml}");
        
        // 提取内容
        var content = await htmlService.ExtractContentAsync("index.html", "//div[@class='content']");
        Console.WriteLine($"\n提取的内容: {content}");
        
        // 释放资源
        await htmlService.DisposeAsync();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<HtmlService>();
        builder.Configure<HtmlSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 100;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = false;
            options.MaxDocumentSize = 10485760;
            options.EnableJavaScript = false;
            options.EnableCss = false;
            options.EnableImages = false;
        });
        return builder.BuildServiceProvider();
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
        Console.WriteLine("HTML 高级配置示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 HTML 设置
        builder.Configure<HtmlSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 200;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
            options.MaxDocumentSize = 20971520; // 20MB
            options.EnableJavaScript = true;
            options.EnableCss = true;
            options.EnableImages = true;
        });
        
        // 注册服务
        builder.AddSingleton<HtmlService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<HtmlSettings>>().Value;
        Console.WriteLine($"配置: 启用缓存={settings.EnableCache}, 缓存大小={settings.CacheSize}");
        Console.WriteLine($"超时={settings.Timeout}, 最大文档大小={settings.MaxDocumentSize} 字节");
        Console.WriteLine($"启用 JavaScript={settings.EnableJavaScript}, 启用 CSS={settings.EnableCss}, 启用图片={settings.EnableImages}");
        
        // 使用服务
        var service = serviceProvider.GetRequiredService<HtmlService>();
        
        // 抓取网站
        var scrapeResult = await service.ScrapeWebsiteAsync("https://example.com");
        Console.WriteLine($"\n网站标题: {scrapeResult.Title}");
        Console.WriteLine($"元描述: {scrapeResult.MetaDescription}");
        Console.WriteLine($"链接数: {scrapeResult.Links.Count}");
        Console.WriteLine($"图片数: {scrapeResult.Images.Count}");
        
        // 释放资源
        await service.DisposeAsync();
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
        Console.WriteLine("HTML 性能优化示例");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<HtmlService>();
        
        // 性能测试
        var testFiles = new[] { "small.html", "medium.html", "large.html" };
        
        foreach (var file in testFiles)
        {
            Console.WriteLine($"\n测试文件: {file}");
            var stopwatch = Stopwatch.StartNew();
            
            // 解析 HTML
            var parseResult = await service.ParseHtmlAsync(file);
            stopwatch.Stop();
            
            Console.WriteLine($"  解析时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"  标题: {parseResult.Title}");
            Console.WriteLine($"  链接数: {parseResult.Links.Count}");
            Console.WriteLine($"  图片数: {parseResult.Images.Count}");
        }
        
        // 测试缓存性能
        Console.WriteLine($"\n测试缓存性能:");
        var cacheStopwatch = Stopwatch.StartNew();
        
        // 重复解析同一个文件
        for (int i = 0; i < 10; i++)
        {
            await service.ParseHtmlAsync("medium.html");
        }
        
        cacheStopwatch.Stop();
        Console.WriteLine($"  10次解析时间: {cacheStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  平均每次: {cacheStopwatch.Elapsed.TotalMilliseconds / 10:F3} ms");
        
        // 释放资源
        await service.DisposeAsync();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<HtmlService>();
        builder.Configure<HtmlSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 100;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = false;
        });
        return builder.BuildServiceProvider();
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
        Console.WriteLine("HTML 错误处理示例");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<HtmlService>();
        
        try
        {
            // 测试不存在的文件
            Console.WriteLine("测试不存在的文件:");
            var result = await service.ParseHtmlAsync("nonexistent.html");
            Console.WriteLine($"成功: {result.Title}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件不存在错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 测试无效的 URL
            Console.WriteLine("\n测试无效的 URL:");
            var result = await service.ParseHtmlAsync("https://invalid-url-12345.com");
            Console.WriteLine($"成功: {result.Title}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP 请求错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 测试无效的 HTML
            Console.WriteLine("\n测试无效的 HTML:");
            var sanitizedHtml = await service.SanitizeHtmlAsync("<p>不完整的 HTML");
            Console.WriteLine($"成功: {sanitizedHtml}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTML 错误: {ex.Message}");
        }
        finally
        {
            // 确保释放资源
            await service.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<HtmlService>();
        return builder.BuildServiceProvider();
    }
}
```

### 5. AOT 编译示例

```csharp
// scripts/html_aot.cs 文件示例
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package HtmlAgilityPack@1.11.58
#:package AngleSharp@1.10.0
#:package HtmlSanitizer@8.0.482
#:package CsQuery@1.3.4
#:package System.Text.RegularExpressions@4.3.1
#:package System.Net.Http@4.3.4
#:package System.IO@4.3.0
#:package System.Collections.Immutable@8.0.0
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
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using AngleSharp;
using AngleSharp.Html.Parser;
using Ganss.Xss;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("HTML AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var htmlService = serviceProvider.GetRequiredService<HtmlService>();
        var settings = serviceProvider.GetRequiredService<IOptions<HtmlSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "parse":
                case "p":
                    await ParseHtml(htmlService, arguments);
                    break;
                case "sanitize":
                case "s":
                    await SanitizeHtml(htmlService, arguments);
                    break;
                case "extract":
                case "e":
                    await ExtractContent(htmlService, arguments);
                    break;
                case "scrape":
                case "sc":
                    await ScrapeWebsite(htmlService, arguments);
                    break;
                case "validate":
                case "v":
                    await ValidateHtml(htmlService, arguments);
                    break;
                case "minify":
                case "m":
                    await MinifyHtml(htmlService, arguments);
                    break;
                case "config":
                case "c":
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
            await htmlService.DisposeAsync();
        }
    }
    
    // 其他方法实现...
}
```

## 总结

以上示例展示了 HTML 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用 AOT 编译

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### AOT 编译优势

- **快速启动**: 比 JIT 编译快 3-5 倍
- **低内存占用**: 内存占用减少 20-30%
- **简单部署**: 单文件执行，无运行时依赖
- **稳定性能**: 编译时优化
- **高安全性**: 减少运行时攻击面

### 命令行使用

```bash
# 解析 HTML 文件或 URL
html_aot.exe parse index.html

# 清理 HTML 内容
html_aot.exe sanitize "<p>安全的 <script>alert('危险')</script> HTML</p>"

# 提取 HTML 内容
html_aot.exe extract https://example.com "//div[@class='content']"

# 抓取网站
html_aot.exe scrape https://example.com

# 验证 HTML
html_aot.exe validate index.html

# 压缩 HTML 文件
html_aot.exe minify index.html minified.html

# 显示配置信息
html_aot.exe config

# 显示帮助信息
html_aot.exe help
```

### 支持的功能

- **HTML 解析**: 支持从文件、URL 或字符串解析 HTML
- **HTML 清理**: 防止 XSS 攻击，清理不安全的 HTML
- **内容提取**: 使用 XPath 和 CSS 选择器提取特定内容
- **网站抓取**: 抓取网站的标题、链接、图片、元数据等
- **HTML 验证**: 验证 HTML 文档的基本结构
- **HTML 压缩**: 压缩 HTML 文件，减少文件大小
- **缓存机制**: 智能缓存，提高性能

### 系统要求

- .NET 10.0 或更高版本
- Windows 10/11 (x64)
- 至少 2GB 内存
- 支持的浏览器: Chrome, Firefox, Edge, Safari
