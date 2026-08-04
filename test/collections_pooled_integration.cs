#:sdk Microsoft.NET.Sdk.Web
#:package Collections.Pooled@1.0.82
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

// 文件头部包含完整版权和版本信息
#region 配置选项
/// <summary>
/// Collections.Pooled配置选项
/// </summary>
public class PooledCollectionsOptions
{
    /// <summary>
    /// 默认初始容量
    /// </summary>
    public int DefaultInitialCapacity { get; set; } = 16;

    /// <summary>
    /// 最大池大小
    /// </summary>
    public int MaximumPoolSize { get; set; } = 1024;

    /// <summary>
    /// 是否启用严格模式
    /// </summary>
    public bool EnableStrictMode { get; set; } = false;

    /// <summary>
    /// 是否启用自动清理
    /// </summary>
    public bool EnableAutoClear { get; set; } = true;

    /// <summary>
    /// 清理间隔(毫秒)
    /// </summary>
    public int ClearInterval { get; set; } = 5000;
}

#region 服务接口
/// <summary>
/// 池化集合服务接口
/// </summary>
public interface IPooledCollectionsService
{
    /// <summary>
    /// 获取池化列表
    /// </summary>
    PooledList<T> GetList<T>();

    /// <summary>
    /// 获取带初始容量的池化列表
    /// </summary>
    PooledList<T> GetList<T>(int initialCapacity);

    /// <summary>
    /// 获取池化字典
    /// </summary>
    PooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>();

    /// <summary>
    /// 获取带初始容量的池化字典
    /// </summary>
    PooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>(int initialCapacity);

    /// <summary>
    /// 获取池化哈希集
    /// </summary>
    PooledSet<T> GetSet<T>();

    /// <summary>
    /// 获取带初始容量的池化哈希集
    /// </summary>
    PooledSet<T> GetSet<T>(int initialCapacity);

    /// <summary>
    /// 返回集合到池中
    /// </summary>
    void Return<T>(T pooledCollection) where T : class, IPooledObject;
}

#region 实现类
/// <summary>
/// 池化集合服务实现
/// </summary>
public class PooledCollectionsService : IPooledCollectionsService, IDisposable
{
    private readonly PooledCollectionsOptions _options;
    private readonly ILogger<PooledCollectionsService> _logger;
    private readonly Timer _clearTimer;

    public PooledCollectionsService(
        IOptions<PooledCollectionsOptions> options,
        ILogger<PooledCollectionsService> logger)
    {
        _options = options.Value;
        _logger = logger;

        // 初始化池化集合配置
        PooledCollectionSettings.DefaultInitialCapacity = _options.DefaultInitialCapacity;
        PooledCollectionSettings.MaximumPoolSize = _options.MaximumPoolSize;
        PooledCollectionSettings.EnableStrictMode = _options.EnableStrictMode;

        // 设置自动清理定时器
        if (_options.EnableAutoClear)
        {
            _clearTimer = new Timer(ClearUnusedCollections, null, 
                _options.ClearInterval, _options.ClearInterval);
        }
    }

    public PooledList<T> GetList<T>() => PooledList<T>.Get();

    public PooledList<T> GetList<T>(int initialCapacity) => PooledList<T>.Get(initialCapacity);

    public PooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>() => 
        PooledDictionary<TKey, TValue>.Get();

    public PooledDictionary<TKey, TValue> GetDictionary<TKey, TValue>(int initialCapacity) => 
        PooledDictionary<TKey, TValue>.Get(initialCapacity);

    public PooledSet<T> GetSet<T>() => PooledSet<T>.Get();

    public PooledSet<T> GetSet<T>(int initialCapacity) => PooledSet<T>.Get(initialCapacity);

    public void Return<T>(T pooledCollection) where T : class, IPooledObject
    {
        pooledCollection?.Free();
    }

    private void ClearUnusedCollections(object state)
    {
        try
        {
            PooledList.ClearAllPools();
            PooledDictionary.ClearAllPools();
            PooledSet.ClearAllPools();
            _logger.LogInformation("Cleared unused pooled collections");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear pooled collections");
        }
    }

    public void Dispose()
    {
        _clearTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}

#region DI扩展
/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class PooledCollectionsExtensions
{
    /// <summary>
    /// 添加池化集合服务
    /// </summary>
    public static IServiceCollection AddPooledCollectionsServices(
        this IServiceCollection services,
        Action<PooledCollectionsOptions> configureOptions = null)
    {
        services.AddOptions<PooledCollectionsOptions>()
            .Configure(configureOptions ?? (opts => { }))
            .ValidateDataAnnotations();

        services.AddSingleton<IPooledCollectionsService, PooledCollectionsService>();
        return services;
    }
}

#region 示例用法
/// <summary>
/// 演示如何使用池化集合服务
/// </summary>
public static class PooledCollectionsExample
{
    public static void Demo()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPooledCollectionsServices(opts =>
        {
            opts.DefaultInitialCapacity = 32;
            opts.MaximumPoolSize = 2048;
            opts.EnableAutoClear = true;
            opts.ClearInterval = 3000;
        });

        using var provider = services.BuildServiceProvider();
        var pooledService = provider.GetRequiredService<IPooledCollectionsService>();

        // 使用池化列表
        var list = pooledService.GetList<int>();
        try
        {
            list.AddRange(Enumerable.Range(1, 100));
            Console.WriteLine($"PooledList count: {list.Count}");
        }
        finally
        {
            pooledService.Return(list);
        }

        // 使用池化字典
        var dict = pooledService.GetDictionary<string, int>();
        try
        {
            dict["one"] = 1;
            dict["two"] = 2;
            Console.WriteLine($"PooledDictionary count: {dict.Count}");
        }
        finally
        {
            pooledService.Return(dict);
        }

        Console.WriteLine("PooledCollections demo completed.");
    }
}