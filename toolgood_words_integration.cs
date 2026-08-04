#:sdk Microsoft.NET.Sdk.Web
#:package ToolGood.Words@2.0.8
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Text.Json;
using ToolGood.Words;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace YourNamespace;

/// <summary>
/// 词云选项配置
/// </summary>
public record WordCloudOptions
{
    /// <summary>
    /// 最大词数限制
    /// </summary>
    public int MaxWords { get; init; } = 100;
    
    /// <summary>
    /// 是否启用敏感词过滤
    /// </summary>
    public bool EnableSensitiveWordFilter { get; init; } = true;
    
    /// <summary>
    /// 词云图片尺寸
    /// </summary>
    public Size ImageSize { get; init; } = new Size(800, 600);
    
    /// <summary>
    /// 敏感词文件路径
    /// </summary>
    public string? SensitiveWordsFilePath { get; init; } = "./Data/IllegalWords.txt";
    
    /// <summary>
    /// 是否启用拼音转换
    /// </summary>
    public bool EnablePinyinConversion { get; init; } = false;
    
    /// <summary>
    /// 文本相似度计算阈值
    /// </summary>
    public double SimilarityThreshold { get; init; } = 0.8;
    
    /// <summary>
    /// 是否启用繁体转简体
    /// </summary>
    public bool EnableTraditionalToSimplified { get; init; } = true;
}

/// <summary>
/// 词云服务接口
/// </summary>
public interface IWordCloudService
{
    /// <summary>
    /// 生成单个词云
    /// </summary>
    Task<Stream> GenerateWordCloudAsync(string text, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 批量生成词云
    /// </summary>
    Task<IReadOnlyList<Stream>> BatchGenerateWordCloudAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取文本中的敏感词
    /// </summary>
    Task<IReadOnlyList<string>> GetSensitiveWordsAsync(string text, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 文本相似度计算
    /// </summary>
    Task<double> CalculateTextSimilarityAsync(string text1, string text2, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 文本转拼音
    /// </summary>
    Task<string> ConvertToPinyinAsync(string text, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 繁体转简体
    /// </summary>
    Task<string> TraditionalToSimplifiedAsync(string text, CancellationToken cancellationToken = default);
}

/// <summary>
/// 词云服务实现
/// </summary>
public sealed class WordCloudService : IWordCloudService
{
    private readonly IMemoryCache _cache;
    private readonly WordCloudOptions _options;
    private readonly ILogger<WordCloudService> _logger;
    private readonly WordsHelper _wordsHelper;
    private readonly ObjectPool<WordCloudGenerator> _generatorPool;
    private readonly Channel<WordCloudJob> _processingChannel;

    public WordCloudService(
        IMemoryCache cache,
        IOptions<WordCloudOptions> options,
        ILogger<WordCloudService> logger,
        WordsHelper wordsHelper,
        ObjectPool<WordCloudGenerator> generatorPool)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;
        _wordsHelper = wordsHelper;
        _generatorPool = generatorPool;
        
        // 创建高性能处理通道
        _processingChannel = Channel.CreateBounded<WordCloudJob>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.Wait
        });
        
        // 启动后台处理服务
        _ = Task.Run(ProcessJobsAsync);
    }

    public async Task<Stream> GenerateWordCloudAsync(string text, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"wordcloud_{text.GetHashCode()}";
        if (_cache.TryGetValue(cacheKey, out MemoryStream cachedStream))
            return cachedStream;

        var generator = _generatorPool.Get();
        try
        {
            // 预处理文本
            var processedText = await PreprocessTextAsync(text, cancellationToken);
            
            // 生成词云
            var image = generator.Generate(processedText, _options.ImageSize.Width, _options.ImageSize.Height);
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            
            // 缓存结果
            _cache.Set(cacheKey, memoryStream, TimeSpan.FromMinutes(30));
            return memoryStream;
        }
        finally
        {
            _generatorPool.Return(generator);
        }
    }

    public async Task<IReadOnlyList<Stream>> BatchGenerateWordCloudAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        var results = new List<Stream>();
        var writer = _processingChannel.Writer;
        
        // 提交所有任务到处理通道
        foreach (var text in texts)
            await writer.WriteAsync(new WordCloudJob(Guid.NewGuid().ToString(), text), cancellationToken);
            
        // 读取处理结果
        var reader = _processingChannel.Reader;
        while (await reader.WaitToReadAsync(cancellationToken))
        {
            while (reader.TryRead(out var job))
            {
                results.Add(await GenerateWordCloudAsync(job.Text, cancellationToken));
                if (results.Count == texts.Count()) break;
            }
        }
        
        return results;
    }

    public async Task<IReadOnlyList<string>> GetSensitiveWordsAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!_options.EnableSensitiveWordFilter) 
            return Array.Empty<string>();
            
        return await Task.Run(() => 
            _wordsHelper.FindAllSensitiveWords(text).Distinct().ToList(), 
            cancellationToken);
    }

    public async Task<double> CalculateTextSimilarityAsync(string text1, string text2, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => 
            _wordsHelper.Similarity(text1, text2), 
            cancellationToken);
    }

    public async Task<string> ConvertToPinyinAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!_options.EnablePinyinConversion) 
            return text;
            
        return await Task.Run(() => 
            _wordsHelper.ToPinyin(text), 
            cancellationToken);
    }

    public async Task<string> TraditionalToSimplifiedAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!_options.EnableTraditionalToSimplified) 
            return text;
            
        return await Task.Run(() => 
            _wordsHelper.ToSimplifiedChinese(text), 
            cancellationToken);
    }

    private async Task ProcessJobsAsync()
    {
        var reader = _processingChannel.Reader;
        while (await reader.WaitToReadAsync())
        {
            while (reader.TryRead(out var job))
            {
                try
                {
                    await GenerateWordCloudAsync(job.Text);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing word cloud job {JobId}", job.Id);
                }
            }
        }
    }

    private async Task<string> PreprocessTextAsync(string text, CancellationToken cancellationToken)
    {
        // 1. 繁体转简体
        if (_options.EnableTraditionalToSimplified)
            text = await TraditionalToSimplifiedAsync(text, cancellationToken);
            
        // 2. 过滤敏感词
        if (_options.EnableSensitiveWordFilter)
        {
            var sensitiveWords = await GetSensitiveWordsAsync(text, cancellationToken);
            text = sensitiveWords.Aggregate(text, (current, word) => 
                current.Replace(word, new string('*', word.Length)));
        }
        
        // 3. 拼音转换
        if (_options.EnablePinyinConversion)
            text = await ConvertToPinyinAsync(text, cancellationToken);
            
        return text;
    }
}

/// <summary>
/// DI扩展方法
/// </summary>
public static class WordCloudServiceCollectionExtensions
{
    public static IServiceCollection AddWordCloudServices(this IServiceCollection services, Action<WordCloudOptions> configure = null)
    {
        // 配置选项
        services.AddOptions<WordCloudOptions>()
            .Configure(configure ?? (opt => { }))
            .ValidateDataAnnotations();
            
        // 注册敏感词帮助类
        services.AddSingleton(sp => 
        {
            var options = sp.GetRequiredService<IOptions<WordCloudOptions>>().Value;
            var helper = new WordsHelper();
            if (options.EnableIllegalWordsFilter && File.Exists(options.IllegalWordsPath))
                helper.SetIllegalWords(File.ReadAllText(options.IllegalWordsPath));
            return helper;
        });
        
        // 配置对象池
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(sp => 
            sp.GetRequiredService<ObjectPoolProvider>().Create(new WordCloudGeneratorPooledPolicy()));
            
        // 注册词云服务
        services.AddSingleton<IWordCloudService, WordCloudService>();
        
        // 后台处理服务
        services.AddHostedService<WordCloudProcessingService>();
        
        return services;
    }
}

// 对象池策略
public class WordCloudGeneratorPooledPolicy : IPooledObjectPolicy<WordCloudGenerator>
{
    public WordCloudGenerator Create() => new WordCloudGenerator();
    
    public bool Return(WordCloudGenerator obj)
    {
        obj.Reset();
        return true;
    }
}

// 后台处理服务
public class WordCloudProcessingService : BackgroundService
{
    private readonly IWordCloudService _service;
    private readonly Channel<WordCloudJob> _channel;
    
    public WordCloudProcessingService(IWordCloudService service, Channel<WordCloudJob> channel)
    {
        _service = service;
        _channel = channel;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = _channel.Reader;
        while (await reader.WaitToReadAsync(stoppingToken))
        {
            while (reader.TryRead(out var job))
            {
                await _service.GenerateAsync(job.Text, stoppingToken);
            }
        }
    }
}

// 处理任务
public record WordCloudJob(string Id, string Text);