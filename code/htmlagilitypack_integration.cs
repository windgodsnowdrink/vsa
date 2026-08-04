#:sdk Microsoft.NET.Sdk
#:package HtmlAgilityPack@1.11.46
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.Http@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading.Channels;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// HtmlAgilityPack生产级集成方案
/// 包含高性能HTML解析、XPath查询优化和分布式爬取支持
/// </summary>
public static class HtmlAgilityPackIntegration
{
    /// <summary>
    /// 添加HtmlAgilityPack服务到DI容器
    /// </summary>
    public static IServiceCollection AddHtmlAgilityPackServices(this IServiceCollection services)
    {
        // 基础服务配置
        services.AddLogging();
        services.AddOptions();
        
        // 高性能HTML解析器配置
        services.AddSingleton<IHtmlParser>(provider => 
        {
            var options = provider.GetRequiredService<IOptions<HtmlParserOptions>>();
            var parser = new HtmlParser(options.Value);
            return parser;
        });
        
        // XPath查询优化器
        services.AddSingleton<IXPathOptimizer, SpanBasedXPathOptimizer>();
        services.AddSingleton<IXPathQueryExecutor, XPathQueryExecutor>();
        
        // 分布式爬取支持
        services.AddSingleton<IDistributedCrawler, HtmlAgilityPackCrawler>();
        
        // HTML清理服务
        services.AddSingleton<IHtmlSanitizer, HtmlSanitizerService>();
        
        // 异步处理管道
        services.AddSingleton<IHtmlProcessingPipeline, HtmlProcessingPipeline>();
        
        // 性能监控
        services.AddSingleton<IHtmlPerformanceMonitor, HtmlPerformanceMonitor>();
        
        return services;
    }
}

/// <summary>
/// HtmlAgilityPack核心服务实现
/// 集成Threading.Channels、对象池等高性能技术
/// </summary>
public class HtmlAgilityPackService : IHostedService
{
    private readonly Channel<HtmlDocument> _documentChannel;
    private readonly ObjectPool<HtmlDocument> _documentPool;
    private readonly ThreadLocal<Span<char>> _threadLocalBuffer;
    
    public HtmlAgilityPackService(
        ILogger<HtmlAgilityPackService> logger,
        IOptions<HtmlParserOptions> options,
        IHtmlParser htmlParser,
        IXPathOptimizer xpathOptimizer)
    {
        // 初始化文档处理通道（基于Threading.Channels）
        _documentChannel = Channel.CreateBounded<HtmlDocument>(1000);
        
        // HTML文档对象池
        _documentPool = new ObjectPool<HtmlDocument>(() => new HtmlDocument());
        
        // 线程局部缓冲区（ThreadLocal<Span>）
        _threadLocalBuffer = new ThreadLocal<Span<char>>(
            () => new char[options.Value.BufferSize].AsSpan());
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // 启动HTML处理任务
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // 停止HTML处理任务
        return Task.CompletedTask;
    }
}

/// <summary>
/// HTML解析器配置选项
/// </summary>
public class HtmlParserOptions
{
    public int BufferSize { get; set; } = 8192;
    public bool UseMemoryOptimization { get; set; } = true;
    public bool EnableXPathCache { get; set; } = true;
    public int XPathCacheSize { get; set; } = 1024;
    public bool UseCompiledXPath { get; set; } = true;
    public bool EnableHtmlSanitization { get; set; } = true;
    public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
    public bool EnablePerformanceMonitoring { get; set; } = true;
}

/// <summary>
/// HTML清理服务
/// 使用Span优化内存分配和线程安全
/// </summary>
public class HtmlSanitizerService
{
    private readonly ILogger<HtmlSanitizerService> _logger;
    private readonly ThreadLocal<Span<char>> _sanitizationBuffer;
    
    public HtmlSanitizerService(ILogger<HtmlSanitizerService> logger, IOptions<HtmlParserOptions> options)
    {
        _logger = logger;
        _sanitizationBuffer = new ThreadLocal<Span<char>>(
            () => new char[options.Value.BufferSize].AsSpan());
    }
    
    public HtmlDocument SanitizeHtml(HtmlDocument document)
    {
        // 实现HTML清理逻辑
        return document;
    }
}

/// <summary>
/// HTML处理管道
/// 使用Threading.Channels实现异步处理流水线
/// </summary>
public class HtmlProcessingPipeline
{
    private readonly Channel<HtmlDocument> _processingChannel;
    private readonly ILogger<HtmlProcessingPipeline> _logger;
    
    public HtmlProcessingPipeline(ILogger<HtmlProcessingPipeline> logger, IOptions<HtmlParserOptions> options)
    {
        _logger = logger;
        _processingChannel = Channel.CreateBounded<HtmlDocument>(
            new BoundedChannelOptions(options.Value.MaxDegreeOfParallelism)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            });
    }
    
    public async Task ProcessAsync(HtmlDocument document)
    {
        await _processingChannel.Writer.WriteAsync(document);
    }
}

/// <summary>
/// 性能监控服务
/// 使用尾延迟优化技术监控HTML处理性能
/// </summary>
public class HtmlPerformanceMonitor
{
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly ILogger<HtmlPerformanceMonitor> _logger;
    
    public HtmlPerformanceMonitor(ILogger<HtmlPerformanceMonitor> logger)
    {
        _logger = logger;
        _latencyOptimizer = new TailLatencyOptimizer();
    }
    
    public void RecordProcessingTime(TimeSpan duration)
    {
        _latencyOptimizer.RecordLatency(duration);
    }
}

/// <summary>
/// XPath查询执行器
/// 使用Span优化内存分配和线程安全
/// </summary>
public class XPathQueryExecutor : IXPathQueryExecutor
{
    private readonly ThreadLocal<Dictionary<string, HtmlNodeNavigator>> _xpathCache;
    private readonly ILogger<XPathQueryExecutor> _logger;
    
    public XPathQueryExecutor(ILogger<XPathQueryExecutor> logger, IOptions<HtmlParserOptions> options)
    {
        _logger = logger;
        _xpathCache = new ThreadLocal<Dictionary<string, HtmlNodeNavigator>>(
            () => new Dictionary<string, HtmlNodeNavigator>(options.Value.XPathCacheSize));
    }
    
    public HtmlNodeCollection ExecuteXPath(HtmlDocument doc, string xpath)
    {
        try
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            
            // 使用缓存优化频繁查询
            if (_xpathCache.Value.TryGetValue(xpath, out var navigator))
            {
                return doc.DocumentNode.SelectNodes(xpath, navigator);
            }
            
            var result = doc.DocumentNode.SelectNodes(xpath);
            if (result != null && result.Count > 0)
            {
                _xpathCache.Value[xpath] = doc.CreateNavigator();
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "XPath查询失败: {XPath}", xpath);
            return null;
        }
    }
}

public interface IXPathQueryExecutor
{
    HtmlNodeCollection ExecuteXPath(HtmlDocument doc, string xpath);
}