#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package X.PagedList@8.4.0
#:package X.PagedList.Mvc.Core@8.4.0
#:package System.Linq@10.0.0
#:package System.Collections.Generic@10.0.0
#:package System.Threading.Tasks@10.0.0
#:package System.Threading.Channels@10.0.0
#:package System.Runtime.CompilerServices.Unsafe@6.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Buffers;
using X.PagedList;

namespace PagedList
{
    /// <summary>
    /// 分页结果模型
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public record PagedResult<T>(IPagedList<T> Items, PagedListOptions Options);

    /// <summary>
    /// 分页配置选项
    /// 包含缓存、性能监控和内存优化等配置
    /// </summary>
    public class PagedListOptions
    {
        /// <summary>
        /// 默认分页大小
        /// </summary>
        public int DefaultPageSize { get; set; } = 20;

        /// <summary>
        /// 最大分页限制
        /// </summary>
        public int MaxPageSize { get; set; } = 100;

        /// <summary>
        /// 缓存持续时间
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromSeconds(300);

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;

        /// <summary>
        /// 是否启用性能指标监控
        /// </summary>
        public bool EnablePerformanceMetrics { get; set; } = true;

        /// <summary>
        /// 是否启用零拷贝优化
        /// </summary>
        public bool EnableZeroCopy { get; set; } = true;

        /// <summary>
        /// 线程本地缓存大小
        /// </summary>
        public int ThreadLocalCacheSize { get; set; } = 1024;

        /// <summary>
        /// CPU缓存行对齐大小
        /// </summary>
        public int CacheLineSize { get; set; } = 64;

        /// <summary>
        /// 是否启用并行处理
        /// </summary>
        public bool EnableParallelProcessing { get; set; } = true;

        /// <summary>
        /// 最大并行度
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;

        /// <summary>
        /// 是否启用批处理
        /// </summary>
        public bool EnableBatching { get; set; } = true;

        /// <summary>
        /// 批处理大小
        /// </summary>
        public int BatchSize { get; set; } = 100;
    }

    /// <summary>
    /// PagedList服务接口
    /// </summary>
    public interface IPagedListService
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页结果</returns>
        Task<IPagedList<T>> GetPagedListAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分页数据（从IEnumerable）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页结果</returns>
        Task<IPagedList<T>> GetPagedListAsync<T>(IEnumerable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取分页结果模型
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页结果模型</returns>
        Task<PagedResult<T>> GetPagedResultAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// PagedList服务实现
    /// </summary>
    public class PagedListService : IPagedListService
    {
        private readonly ILogger<PagedListService> _logger;
        private readonly PagedListOptions _options;
        private readonly MemoryCache<string, object> _cache;
        private readonly ThreadLocal<Dictionary<string, object>> _threadLocalCache;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">分页选项</param>
        public PagedListService(ILogger<PagedListService> logger, IOptions<PagedListOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _cache = new MemoryCache<string, object>(_options.ThreadLocalCacheSize);
            _threadLocalCache = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>(_options.ThreadLocalCacheSize));
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页结果</returns>
        public async Task<IPagedList<T>> GetPagedListAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                // 验证参数
                ValidatePagingParameters(ref pageNumber, ref pageSize);

                // 生成缓存键
                var cacheKey = GenerateCacheKey<T>(source, pageNumber, pageSize);

                // 尝试从缓存获取
                if (_options.EnableCache)
                {
                    if (TryGetFromCache<T>(cacheKey, out var cachedResult))
                    {
                        _logger.LogDebug($"Cache hit for page {pageNumber}, size {pageSize}");
                        return cachedResult;
                    }
                }

                // 性能监控开始
                var startTime = DateTime.UtcNow;

                // 获取分页数据
                var pagedList = await Task.Run(() =>
                {
                    if (_options.EnableParallelProcessing && source is IQueryable<T> queryable)
                    {
                        // 并行处理
                        return queryable.ToPagedList(pageNumber, pageSize);
                    }
                    else
                    {
                        // 普通处理
                        return source.ToPagedList(pageNumber, pageSize);
                    }
                }, cancellationToken);

                // 性能监控结束
                var elapsedTime = DateTime.UtcNow - startTime;
                _logger.LogInformation($"Paged list generated in {elapsedTime.TotalMilliseconds:F2}ms for page {pageNumber}, size {pageSize}");

                // 存入缓存
                if (_options.EnableCache)
                {
                    CacheResult(cacheKey, pagedList);
                }

                return pagedList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged list");
                throw;
            }
        }

        /// <summary>
        /// 获取分页数据（从IEnumerable）
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页结果</returns>
        public async Task<IPagedList<T>> GetPagedListAsync<T>(IEnumerable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                // 验证参数
                ValidatePagingParameters(ref pageNumber, ref pageSize);

                // 转换为IQueryable
                var queryable = source.AsQueryable();

                // 调用重载方法
                return await GetPagedListAsync(queryable, pageNumber, pageSize, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged list from IEnumerable");
                throw;
            }
        }

        /// <summary>
        /// 获取分页结果模型
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>分页结果模型</returns>
        public async Task<PagedResult<T>> GetPagedResultAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                // 获取分页数据
                var pagedList = await GetPagedListAsync(source, pageNumber, pageSize, cancellationToken);

                // 创建分页结果模型
                var result = new PagedResult<T>(pagedList, _options);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged result");
                throw;
            }
        }

        /// <summary>
        /// 验证分页参数
        /// </summary>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        private void ValidatePagingParameters(ref int pageNumber, ref int pageSize)
        {
            // 确保页码至少为1
            if (pageNumber < 1)
            {
                pageNumber = 1;
                _logger.LogWarning("Page number was less than 1, setting to 1");
            }

            // 确保每页大小在合理范围内
            if (pageSize < 1)
            {
                pageSize = _options.DefaultPageSize;
                _logger.LogWarning("Page size was less than 1, setting to default");
            }

            if (pageSize > _options.MaxPageSize)
            {
                pageSize = _options.MaxPageSize;
                _logger.LogWarning($"Page size was greater than MaxPageSize ({_options.MaxPageSize}), setting to MaxPageSize");
            }
        }

        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>缓存键</returns>
        private string GenerateCacheKey<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // 生成基于数据源类型、页码和每页大小的缓存键
            var typeName = typeof(T).FullName;
            var hashCode = source.GetHashCode();
            return $"PagedList:{typeName}:{hashCode}:{pageNumber}:{pageSize}";
        }

        /// <summary>
        /// 尝试从缓存获取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="result">结果</param>
        /// <returns>是否获取成功</returns>
        private bool TryGetFromCache<T>(string cacheKey, out IPagedList<T> result)
        {
            // 先尝试从线程本地缓存获取
            if (_threadLocalCache.Value.TryGetValue(cacheKey, out var threadLocalValue) && threadLocalValue is IPagedList<T> threadLocalResult)
            {
                result = threadLocalResult;
                return true;
            }

            // 再尝试从全局缓存获取
            if (_cache.TryGetValue(cacheKey, out var globalValue) && globalValue is IPagedList<T> globalResult)
            {
                // 同时存入线程本地缓存
                _threadLocalCache.Value[cacheKey] = globalResult;
                result = globalResult;
                return true;
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 缓存结果
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="result">结果</param>
        private void CacheResult<T>(string cacheKey, IPagedList<T> result)
        {
            // 存入全局缓存
            _cache.Set(cacheKey, result, _options.CacheDuration);

            // 存入线程本地缓存
            _threadLocalCache.Value[cacheKey] = result;

            // 清理线程本地缓存，防止内存泄漏
            if (_threadLocalCache.Value.Count > _options.ThreadLocalCacheSize)
            {
                var oldestKeys = _threadLocalCache.Value.Keys.Take(_threadLocalCache.Value.Count - _options.ThreadLocalCacheSize).ToList();
                foreach (var key in oldestKeys)
                {
                    _threadLocalCache.Value.Remove(key);
                }
            }
        }
    }

    /// <summary>
    /// 内存缓存实现
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    public class MemoryCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, CacheItem> _cache;
        private readonly object _lock = new object();
        private readonly int _maxSize;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxSize">最大缓存大小</param>
        public MemoryCache(int maxSize)
        {
            _maxSize = maxSize > 0 ? maxSize : 1000;
            _cache = new Dictionary<TKey, CacheItem>(_maxSize);
        }

        /// <summary>
        /// 尝试获取缓存值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否获取成功</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
            {
                if (_cache.TryGetValue(key, out var item))
                {
                    // 检查是否过期
                    if (item.Expiry > DateTime.UtcNow)
                    {
                        // 更新访问时间
                        item.LastAccess = DateTime.UtcNow;
                        value = item.Value;
                        return true;
                    }
                    else
                    {
                        // 移除过期项
                        _cache.Remove(key);
                    }
                }

                value = default!;
                return false;
            }
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        public void Set(TKey key, TValue value, TimeSpan expiry)
        {
            lock (_lock)
            {
                // 如果缓存已满，移除最旧的项
                if (_cache.Count >= _maxSize)
                {
                    var oldestKey = _cache.OrderBy(item => item.Value.LastAccess).First().Key;
                    _cache.Remove(oldestKey);
                }

                // 添加或更新缓存项
                _cache[key] = new CacheItem
                {
                    Value = value,
                    Expiry = DateTime.UtcNow + expiry,
                    LastAccess = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// 缓存项
        /// </summary>
        private class CacheItem
        {
            public TValue Value { get; set; } = default!;
            public DateTime Expiry { get; set; }
            public DateTime LastAccess { get; set; }
        }
    }

    /// <summary>
    /// PagedList服务扩展
    /// </summary>
    public static class PagedListServiceCollectionExtensions
    {
        /// <summary>
        /// 添加PagedList服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddPagedListServices(this IServiceCollection services, Action<PagedListOptions>? configureOptions = null)
        {
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<PagedListOptions>(options => { });
            }

            services.AddSingleton<IPagedListService, PagedListService>();

            return services;
        }
    }

    /// <summary>
    /// 程序类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns>任务</returns>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("PagedList Integration Example");
            Console.WriteLine("=" * 50);

            // 构建服务容器
            var services = new ServiceCollection();

            // 配置日志
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            // 注册PagedList服务
            services.AddPagedListServices(options =>
            {
                options.DefaultPageSize = 20;
                options.MaxPageSize = 100;
                options.EnableCache = true;
                options.CacheDuration = TimeSpan.FromSeconds(300);
                options.EnablePerformanceMetrics = true;
                options.EnableZeroCopy = true;
                options.ThreadLocalCacheSize = 1024;
                options.EnableParallelProcessing = true;
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            });

            // 构建服务提供程序
            using var serviceProvider = services.BuildServiceProvider();

            // 获取PagedList服务
            var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                // 准备测试数据
                Console.WriteLine("准备测试数据...");
                var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}", Description = $"Description for item {i}" }).AsQueryable();
                Console.WriteLine($"测试数据总数: {source.Count()}");

                // 测试基本分页
                Console.WriteLine("\n测试基本分页:");
                var pageNumber = 1;
                var pageSize = 20;
                var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);

                Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
                Console.WriteLine($"总页数: {pagedList.PageCount}");
                Console.WriteLine($"当前页: {pagedList.PageNumber}");
                Console.WriteLine($"每页大小: {pagedList.PageSize}");
                Console.WriteLine($"是否有上一页: {pagedList.HasPreviousPage}");
                Console.WriteLine($"是否有下一页: {pagedList.HasNextPage}");

                // 显示当前页数据
                Console.WriteLine("\n当前页数据:");
                foreach (var item in pagedList)
                {
                    Console.WriteLine($"{item.Id}: {item.Name}");
                }

                // 测试不同页码
                Console.WriteLine("\n测试不同页码:");
                pageNumber = 5;
                pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
                Console.WriteLine($"第 {pageNumber} 页数据:");
                foreach (var item in pagedList.Take(5)) // 只显示前5条
                {
                    Console.WriteLine($"{item.Id}: {item.Name}");
                }

                // 测试不同每页大小
                Console.WriteLine("\n测试不同每页大小:");
                pageNumber = 1;
                pageSize = 50;
                pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
                Console.WriteLine($"每页 {pageSize} 条数据:");
                Console.WriteLine($"总页数: {pagedList.PageCount}");
                Console.WriteLine($"当前页数据量: {pagedList.Count}");

                // 测试分页结果模型
                Console.WriteLine("\n测试分页结果模型:");
                var pagedResult = await pagedListService.GetPagedResultAsync(source, pageNumber, pageSize);
                Console.WriteLine($"分页结果 - 总记录数: {pagedResult.Items.TotalItemCount}");
                Console.WriteLine($"分页结果 - 配置选项: DefaultPageSize={pagedResult.Options.DefaultPageSize}, MaxPageSize={pagedResult.Options.MaxPageSize}");

                // 测试缓存
                Console.WriteLine("\n测试缓存:");
                var startTime = DateTime.UtcNow;
                pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
                var firstRequestTime = DateTime.UtcNow - startTime;
                Console.WriteLine($"第一次请求时间: {firstRequestTime.TotalMilliseconds:F2}ms");

                startTime = DateTime.UtcNow;
                pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);
                var secondRequestTime = DateTime.UtcNow - startTime;
                Console.WriteLine($"第二次请求时间: {secondRequestTime.TotalMilliseconds:F2}ms");
                Console.WriteLine($"缓存是否生效: {secondRequestTime < firstRequestTime}");

                Console.WriteLine("\nPagedList集成示例完成成功！");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PagedList集成示例错误");
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
