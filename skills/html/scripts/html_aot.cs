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
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<HtmlSettings>(options => {
            options.EnableCache = true;
            options.CacheSize = 100;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = false;
            options.MaxDocumentSize = 10485760; // 10MB
            options.EnableJavaScript = false;
            options.EnableCss = false;
            options.EnableImages = false;
        });
        
        services.AddSingleton<HtmlService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task ParseHtml(HtmlService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("请提供HTML文件路径或URL");
            return;
        }
        
        var input = arguments[0];
        Console.WriteLine($"解析HTML: {input}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ParseHtmlAsync(input);
        stopwatch.Stop();
        
        Console.WriteLine($"解析完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"标题: {result.Title}");
        Console.WriteLine($"链接数: {result.Links.Count}");
        Console.WriteLine($"图片数: {result.Images.Count}");
        Console.WriteLine($"脚本数: {result.Scripts.Count}");
        
        if (result.Links.Count > 0)
        {
            Console.WriteLine("\n前5个链接:");
            foreach (var link in result.Links.Take(5))
            {
                Console.WriteLine($"  - {link}");
            }
        }
    }
    
    private static async Task SanitizeHtml(HtmlService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("请提供HTML文件路径或内容");
            return;
        }
        
        var input = arguments[0];
        Console.WriteLine($"清理HTML: {input}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SanitizeHtmlAsync(input);
        stopwatch.Stop();
        
        Console.WriteLine($"清理完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine("清理后的HTML:");
        Console.WriteLine(result);
    }
    
    private static async Task ExtractContent(HtmlService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("请提供HTML文件路径或URL");
            return;
        }
        
        var input = arguments[0];
        var selector = arguments.Length > 1 ? arguments[1] : "body";
        Console.WriteLine($"提取内容: {input}");
        Console.WriteLine($"选择器: {selector}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ExtractContentAsync(input, selector);
        stopwatch.Stop();
        
        Console.WriteLine($"提取完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine("提取的内容:");
        Console.WriteLine(result);
    }
    
    private static async Task ScrapeWebsite(HtmlService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("请提供网站URL");
            return;
        }
        
        var url = arguments[0];
        Console.WriteLine($"抓取网站: {url}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ScrapeWebsiteAsync(url);
        stopwatch.Stop();
        
        Console.WriteLine($"抓取完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"标题: {result.Title}");
        Console.WriteLine($"链接数: {result.Links.Count}");
        Console.WriteLine($"图片数: {result.Images.Count}");
        Console.WriteLine($"元描述: {result.MetaDescription}");
        
        if (!string.IsNullOrEmpty(result.MetaKeywords))
        {
            Console.WriteLine($"元关键词: {result.MetaKeywords}");
        }
    }
    
    private static async Task ValidateHtml(HtmlService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("请提供HTML文件路径或URL");
            return;
        }
        
        var input = arguments[0];
        Console.WriteLine($"验证HTML: {input}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ValidateHtmlAsync(input);
        stopwatch.Stop();
        
        Console.WriteLine($"验证完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"是否有效: {result.IsValid}");
        
        if (!result.IsValid && result.Errors.Count > 0)
        {
            Console.WriteLine("错误:");
            foreach (var error in result.Errors.Take(5))
            {
                Console.WriteLine($"  - {error}");
            }
        }
    }
    
    private static async Task MinifyHtml(HtmlService service, string[] arguments)
    {
        if (arguments.Length == 0)
        {
            Console.WriteLine("请提供HTML文件路径");
            return;
        }
        
        var input = arguments[0];
        var output = arguments.Length > 1 ? arguments[1] : null;
        Console.WriteLine($"压缩HTML: {input}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.MinifyHtmlAsync(input);
        stopwatch.Stop();
        
        Console.WriteLine($"压缩完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"原始大小: {new FileInfo(input).Length} 字节");
        Console.WriteLine($"压缩大小: {result.Length} 字节");
        Console.WriteLine($"压缩率: {((double)(new FileInfo(input).Length - result.Length) / new FileInfo(input).Length * 100):F2}%");
        
        if (!string.IsNullOrEmpty(output))
        {
            await File.WriteAllTextAsync(output, result);
            Console.WriteLine($"压缩结果已保存到: {output}");
        }
        else
        {
            Console.WriteLine("压缩后的HTML:");
            Console.WriteLine(result);
        }
    }
    
    private static void ShowConfig(HtmlSettings settings)
    {
        Console.WriteLine("HTML 配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"启用缓存: {settings.EnableCache}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"超时: {settings.Timeout}");
        Console.WriteLine($"启用详细日志: {settings.EnableDetailedLogging}");
        Console.WriteLine($"最大文档大小: {settings.MaxDocumentSize} 字节");
        Console.WriteLine($"启用JavaScript: {settings.EnableJavaScript}");
        Console.WriteLine($"启用CSS: {settings.EnableCss}");
        Console.WriteLine($"启用图片: {settings.EnableImages}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("HTML AOT 引擎 命令帮助:");
        Console.WriteLine("=" * 60);
        Console.WriteLine("parse (p)     - 解析HTML文件或URL");
        Console.WriteLine("sanitize (s)  - 清理HTML内容");
        Console.WriteLine("extract (e)   - 提取HTML内容");
        Console.WriteLine("scrape (sc)   - 抓取网站内容");
        Console.WriteLine("validate (v)  - 验证HTML有效性");
        Console.WriteLine("minify (m)    - 压缩HTML文件");
        Console.WriteLine("config (c)    - 显示配置信息");
        Console.WriteLine("help (h, ?)   - 显示帮助信息");
    }
}

public class HtmlSettings
{
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 100;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDetailedLogging { get; set; } = false;
    public long MaxDocumentSize { get; set; } = 10485760; // 10MB
    public bool EnableJavaScript { get; set; } = false;
    public bool EnableCss { get; set; } = false;
    public bool EnableImages { get; set; } = false;
}

public class HtmlParseResult
{
    public string Title { get; set; }
    public List<string> Links { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public List<string> Scripts { get; set; } = new();
    public string Html { get; set; }
}

public class HtmlValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class WebsiteScrapeResult
{
    public string Title { get; set; }
    public string MetaDescription { get; set; }
    public string MetaKeywords { get; set; }
    public List<string> Links { get; set; } = new();
    public List<string> Images { get; set; } = new();
    public string Content { get; set; }
}

public class HtmlService : IAsyncDisposable
{
    private readonly ILogger<HtmlService> _logger;
    private readonly HttpClient _httpClient;
    private readonly HtmlSanitizer _sanitizer;
    private readonly HtmlParser _parser;
    private readonly Dictionary<string, string> _cache;
    private readonly object _cacheLock = new();
    
    public HtmlService(ILogger<HtmlService> logger)
    {
        _logger = logger;
        _httpClient = new HttpClient();
        _sanitizer = new HtmlSanitizer();
        _parser = new HtmlParser();
        _cache = new Dictionary<string, string>();
        
        // 配置清理器
        _sanitizer.AllowedTags.Add("p");
        _sanitizer.AllowedTags.Add("div");
        _sanitizer.AllowedTags.Add("span");
        _sanitizer.AllowedTags.Add("h1");
        _sanitizer.AllowedTags.Add("h2");
        _sanitizer.AllowedTags.Add("h3");
        _sanitizer.AllowedTags.Add("a");
        _sanitizer.AllowedTags.Add("img");
        _sanitizer.AllowedTags.Add("br");
        _sanitizer.AllowedTags.Add("hr");
        _sanitizer.AllowedAttributes.Add("href");
        _sanitizer.AllowedAttributes.Add("src");
        _sanitizer.AllowedAttributes.Add("alt");
        _sanitizer.AllowedAttributes.Add("title");
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedAttributes.Add("id");
    }
    
    public async Task<HtmlParseResult> ParseHtmlAsync(string input)
    {
        var html = await GetHtmlContentAsync(input);
        var result = new HtmlParseResult { Html = html };
        
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        // 提取标题
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        if (titleNode != null)
        {
            result.Title = titleNode.InnerText.Trim();
        }
        
        // 提取链接
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
        if (linkNodes != null)
        {
            foreach (var node in linkNodes)
            {
                var href = node.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(href))
                {
                    result.Links.Add(href);
                }
            }
        }
        
        // 提取图片
        var imgNodes = doc.DocumentNode.SelectNodes("//img[@src]");
        if (imgNodes != null)
        {
            foreach (var node in imgNodes)
            {
                var src = node.GetAttributeValue("src", string.Empty);
                if (!string.IsNullOrEmpty(src))
                {
                    result.Images.Add(src);
                }
            }
        }
        
        // 提取脚本
        var scriptNodes = doc.DocumentNode.SelectNodes("//script");
        if (scriptNodes != null)
        {
            foreach (var node in scriptNodes)
            {
                var src = node.GetAttributeValue("src", string.Empty);
                if (!string.IsNullOrEmpty(src))
                {
                    result.Scripts.Add(src);
                }
                else if (!string.IsNullOrEmpty(node.InnerText))
                {
                    result.Scripts.Add("inline script");
                }
            }
        }
        
        return result;
    }
    
    public async Task<string> SanitizeHtmlAsync(string input)
    {
        var html = await GetHtmlContentAsync(input);
        return _sanitizer.Sanitize(html);
    }
    
    public async Task<string> ExtractContentAsync(string input, string selector = "body")
    {
        var html = await GetHtmlContentAsync(input);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        var nodes = doc.DocumentNode.SelectNodes(selector);
        if (nodes == null || nodes.Count == 0)
        {
            return string.Empty;
        }
        
        var sb = new StringBuilder();
        foreach (var node in nodes)
        {
            sb.AppendLine(node.InnerText.Trim());
        }
        
        return sb.ToString();
    }
    
    public async Task<WebsiteScrapeResult> ScrapeWebsiteAsync(string url)
    {
        var html = await GetHtmlContentAsync(url);
        var result = new WebsiteScrapeResult { Content = html };
        
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        
        // 提取标题
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        if (titleNode != null)
        {
            result.Title = titleNode.InnerText.Trim();
        }
        
        // 提取元描述
        var metaDescNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
        if (metaDescNode != null)
        {
            result.MetaDescription = metaDescNode.GetAttributeValue("content", string.Empty);
        }
        
        // 提取元关键词
        var metaKeywordsNode = doc.DocumentNode.SelectSingleNode("//meta[@name='keywords']");
        if (metaKeywordsNode != null)
        {
            result.MetaKeywords = metaKeywordsNode.GetAttributeValue("content", string.Empty);
        }
        
        // 提取链接
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
        if (linkNodes != null)
        {
            foreach (var node in linkNodes)
            {
                var href = node.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(href))
                {
                    result.Links.Add(href);
                }
            }
        }
        
        // 提取图片
        var imgNodes = doc.DocumentNode.SelectNodes("//img[@src]");
        if (imgNodes != null)
        {
            foreach (var node in imgNodes)
            {
                var src = node.GetAttributeValue("src", string.Empty);
                if (!string.IsNullOrEmpty(src))
                {
                    result.Images.Add(src);
                }
            }
        }
        
        return result;
    }
    
    public async Task<HtmlValidationResult> ValidateHtmlAsync(string input)
    {
        var html = await GetHtmlContentAsync(input);
        var result = new HtmlValidationResult { IsValid = true };
        
        // 简单的HTML验证
        try
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            
            // 检查基本结构
            if (doc.DocumentNode.SelectSingleNode("//html") == null)
            {
                result.IsValid = false;
                result.Errors.Add("缺少 <html> 标签");
            }
            
            if (doc.DocumentNode.SelectSingleNode("//body") == null)
            {
                result.IsValid = false;
                result.Errors.Add("缺少 <body> 标签");
            }
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"解析错误: {ex.Message}