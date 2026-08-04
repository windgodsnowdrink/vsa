#:sdk Microsoft.NET.Sdk.Web
#:package jieba.NET@0.42.2
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using System.Threading.Channels;
using JiebaNet.Segmenter;
using JiebaNet.Analyser;
using JiebaNet.Segmenter.PosSeg;

namespace JiebaNetIntegration;

/// <summary>
/// 分词选项配置
/// </summary>
public class JiebaOptions
{
    /// <summary>
    /// 是否启用并行分词
    /// </summary>
    public bool UseParallel { get; set; } = true;
    
    /// <summary>
    /// 用户词典路径
    /// </summary>
    public string UserDictPath { get; set; } = "./Data/userdict.txt";
    
    /// <summary>
    /// 停用词路径
    /// </summary>
    public string StopWordsPath { get; set; } = "./Data/stopwords.txt";
    
    /// <summary>
    /// 缓存过期时间
    /// </summary>
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(30);
    
    /// <summary>
    /// 允许的词性列表(用于关键词提取)
    /// </summary>
    public List<string> AllowPosTags { get; set; } = new List<string> { "n", "ns", "nr", "nt", "nz", "v", "a" };
    
    /// <summary>
    /// 是否启用词性标注
    /// </summary>
    public bool EnablePosTagging { get; set; } = true;
}

/// <summary>
/// 分词服务接口
/// </summary>
public interface IJiebaService
{
    /// <summary>
    /// 精确模式分词
    /// </summary>
    Task<List<string>> CutAsync(string text, bool hmm = true);
    
    /// <summary>
    /// 搜索引擎模式分词
    /// </summary>
    Task<List<string>> CutForSearchAsync(string text, bool hmm = true);
    
    /// <summary>
    /// 全模式分词
    /// </summary>
    Task<List<string>> CutAllAsync(string text);
    
    /// <summary>
    /// 关键词提取(TF-IDF算法)
    /// </summary>
    Task<List<string>> ExtractTagsAsync(string text, int count = 20, IEnumerable<string> allowPos = null);
    
    /// <summary>
    /// 关键词提取(TextRank算法)
    /// </summary>
    Task<List<string>> ExtractTagsWithTextRankAsync(string text, int count = 20, IEnumerable<string> allowPos = null);
    
    /// <summary>
    /// 词性标注
    /// </summary>
    Task<List<Pair>> PosTagAsync(string text);
    
    /// <summary>
    /// 并行分词(使用多线程加速)
    /// </summary>
    Task<List<string>> ParallelCutAsync(string text, bool hmm = true);
}

/// <summary>
/// 分词服务实现
/// </summary>
public sealed class JiebaService : IJiebaService
{
    private readonly JiebaOptions _options;
    private readonly ILogger<JiebaService> _logger;
    private readonly IMemoryCache _cache;
    private readonly ObjectPool<JiebaSegmenter> _segmenterPool;
    private readonly StopWords _stopWords;
    
    public JiebaService(
        IOptions<JiebaOptions> options,
        ILogger<JiebaService> logger,
        IMemoryCache cache,
        ObjectPool<JiebaSegmenter> segmenterPool)
    {
        _options = options.Value;
        _logger = logger;
        _cache = cache;
        _segmenterPool = segmenterPool;
        
        // 初始化停用词
        _stopWords = new StopWords();
        if (File.Exists(_options.StopWordsPath))
            _stopWords.Load(_options.StopWordsPath);
    }
    
    public async Task<IEnumerable<string>> CutAsync(string text, bool hmm = true, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"cut_{text.GetHashCode()}_{hmm}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<string> cachedResult))
            return cachedResult;
            
        var segmenter = _segmenterPool.Get();
        try
        {
            var result = await Task.Run(() => 
                segmenter.Cut(text, hmm).Where(w => !_stopWords.IsStopWord(w)), 
                cancellationToken);
                
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheExpiration));
            return result;
        }
        finally
        {
            _segmenterPool.Return(segmenter);
        }
    }
    
    public async Task<IEnumerable<string>> ExtractTagsAsync(string text, int topN = 20, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"extract_{text.GetHashCode()}_{topN}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<string> cachedResult))
            return cachedResult;
            
        var segmenter = _segmenterPool.Get();
        try
        {
            var result = await Task.Run(() => 
                segmenter.ExtractTags(text, topN), 
                cancellationToken);
                
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_options.CacheExpiration));
            return result;
        }
        finally
        {
            _segmenterPool.Return(segmenter);
        }
    }
    
    // 其他接口方法实现...
}

/// <summary>
/// DI扩展方法
/// </summary>
public static class JiebaServiceCollectionExtensions
{
    public static IServiceCollection AddJiebaServices(this IServiceCollection services, Action<JiebaOptions> configure = null)
    {
        // 配置选项
        services.AddOptions<JiebaOptions>()
            .Configure(configure ?? (opt => { }))
            .ValidateDataAnnotations();
            
        // 注册对象池
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton(sp => 
        {
            var provider = sp.GetRequiredService<ObjectPoolProvider>();
            var policy = new JiebaSegmenterPooledObjectPolicy(
                sp.GetRequiredService<IOptions<JiebaOptions>>().Value);
            return provider.Create(policy);
        });
        
        // 注册分词服务
        services.AddSingleton<IJiebaService, JiebaService>();
        
        return services;
    }
}

/// <summary>
/// JiebaSegmenter对象池策略
/// </summary>
public class JiebaSegmenterPooledObjectPolicy : IPooledObjectPolicy<JiebaSegmenter>
{
    private readonly JiebaOptions _options;
    
    public JiebaSegmenterPooledObjectPolicy(JiebaOptions options)
    {
        _options = options;
    }
    
    public JiebaSegmenter Create()
    {
        var segmenter = new JiebaSegmenter();
        
        // 加载用户词典
        if (File.Exists(_options.UserDictPath))
            segmenter.LoadUserDict(_options.UserDictPath);
            
        return segmenter;
    }
    
    public bool Return(JiebaSegmenter obj) => true;
}