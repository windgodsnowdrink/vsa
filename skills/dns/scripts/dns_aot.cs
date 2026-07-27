#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Net.NameResolution@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Dns.AOT
{
    /// <summary>
    /// DNS记录类型
    /// </summary>
    public enum DnsRecordType
    {
        /// <summary>
        /// A记录 (IPv4地址)
        /// </summary>
        A,
        /// <summary>
        /// AAAA记录 (IPv6地址)
        /// </summary>
        AAAA,
        /// <summary>
        /// MX记录 (邮件交换)
        /// </summary>
        MX,
        /// <summary>
        /// NS记录 (名称服务器)
        /// </summary>
        NS,
        /// <summary>
        /// CNAME记录 (别名)
        /// </summary>
        CNAME,
        /// <summary>
        /// TXT记录 (文本)
        /// </summary>
        TXT,
        /// <summary>
        /// SOA记录 (起始授权机构)
        /// </summary>
        SOA,
        /// <summary>
        /// PTR记录 (反向查询)
        /// </summary>
        PTR,
        /// <summary>
        /// SRV记录 (服务定位)
        /// </summary>
        SRV
    }
    
    /// <summary>
    /// DNS查询结果
    /// </summary>
    public class DnsQueryResult
    {
        /// <summary>
        /// 查询是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 查询的域名
        /// </summary>
        public string Domain { get; set; } = string.Empty;
        
        /// <summary>
        /// 记录类型
        /// </summary>
        public DnsRecordType RecordType { get; set; }
        
        /// <summary>
        /// 查询结果
        /// </summary>
        public List<string> Results { get; set; } = new List<string>();
        
        /// <summary>
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// DNS服务器
        /// </summary>
        public string? DnsServer { get; set; }
    }
    
    /// <summary>
    /// DNS配置选项
    /// </summary>
    public class DnsOptions
    {
        /// <summary>
        /// 默认DNS服务器列表
        /// </summary>
        public List<string> DnsServers { get; set; } = new List<string> { "8.8.8.8", "8.8.4.4" };
        
        /// <summary>
        /// 查询超时时间（毫秒）
        /// </summary>
        public int TimeoutMs { get; set; } = 5000;
        
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool EnableCache { get; set; } = true;
        
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int CacheSize { get; set; } = 1000;
        
        /// <summary>
        /// 缓存过期时间（秒）
        /// </summary>
        public int CacheExpirationSeconds { get; set; } = 300;
        
        /// <summary>
        /// 是否启用详细日志
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
        
        /// <summary>
        /// 是否启用性能监控
        /// </summary>
        public bool EnablePerformanceMonitoring { get; set; } = true;
        
        /// <summary>
        /// 是否使用系统默认DNS服务器
        /// </summary>
        public bool UseSystemDns { get; set; } = true;
    }
    
    /// <summary>
    /// DNS缓存项
    /// </summary>
    internal class DnsCacheItem
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        public string Key { get; set; } = string.Empty;
        
        /// <summary>
        /// 缓存值
        /// </summary>
        public DnsQueryResult Result { get; set; } = new DnsQueryResult();
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsExpired(int expirationSeconds)
        {
            return (DateTime.UtcNow - CreatedAt).TotalSeconds > expirationSeconds;
        }
    }
    
    /// <summary>
    /// DNS状态信息
    /// </summary>
    public class DnsStatus
    {
        /// <summary>
        /// 服务是否正常运行
        /// </summary>
        public bool IsRunning { get; set; }
        
        /// <summary>
        /// 已处理的查询数
        /// </summary>
        public long ProcessedQueries { get; set; }
        
        /// <summary>
        /// 成功查询数
        /// </summary>
        public long SuccessfulQueries { get; set; }
        
        /// <summary>
        /// 失败查询数
        /// </summary>
        public long FailedQueries { get; set; }
        
        /// <summary>
        /// 平均查询时间（毫秒）
        /// </summary>
        public double AverageQueryTimeMs { get; set; }
        
        /// <summary>
        /// 缓存命中率（%）
        /// </summary>
        public double CacheHitRate { get; set; }
        
        /// <summary>
        /// 当前缓存项数量
        /// </summary>
        public int CurrentCacheItems { get; set; }
        
        /// <summary>
        /// 最大缓存项数量
        /// </summary>
        public int MaxCacheItems { get; set; }
        
        /// <summary>
        /// 服务启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }
    
    /// <summary>
    /// DNS服务接口
    /// </summary>
    public interface IDnsService
    {
        /// <summary>
        /// 查询DNS记录
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="recordType">记录类型</param>
        /// <returns>查询结果</returns>
        Task<DnsQueryResult> QueryAsync(string domain, DnsRecordType recordType);
        
        /// <summary>
        /// 批量查询DNS记录
        /// </summary>
        /// <param name="queries">查询列表</param>
        /// <returns>查询结果列表</returns>
        Task<List<DnsQueryResult>> BatchQueryAsync(List<(string Domain, DnsRecordType RecordType)> queries);
        
        /// <summary>
        /// 查询IPv4地址 (A记录)
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>IPv4地址列表</returns>
        Task<DnsQueryResult> QueryAAsync(string domain);
        
        /// <summary>
        /// 查询IPv6地址 (AAAA记录)
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>IPv6地址列表</returns>
        Task<DnsQueryResult> QueryAAAAAsync(string domain);
        
        /// <summary>
        /// 查询邮件服务器 (MX记录)
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>MX记录列表</returns>
        Task<DnsQueryResult> QueryMXAsync(string domain);
        
        /// <summary>
        /// 查询名称服务器 (NS记录)
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>NS记录列表</returns>
        Task<DnsQueryResult> QueryNSAsync(string domain);
        
        /// <summary>
        /// 查询别名记录 (CNAME)
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>CNAME记录列表</returns>
        Task<DnsQueryResult> QueryCNAMEAsync(string domain);
        
        /// <summary>
        /// 查询文本记录 (TXT)
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>TXT记录列表</returns>
        Task<DnsQueryResult> QueryTXTAsync(string domain);
        
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns>刷新结果</returns>
        Task<bool> FlushCacheAsync();
        
        /// <summary>
        /// 获取DNS服务状态
        /// </summary>
        /// <returns>状态信息</returns>
        Task<DnsStatus> GetStatusAsync();
        
        /// <summary>
        /// 重置DNS服务状态
        /// </summary>
        /// <returns>重置结果</returns>
        Task<bool> ResetStatusAsync();
    }
    
    /// <summary>
    /// DNS服务实现
    /// </summary>
    public class DnsService : IDnsService
    {
        private readonly ILogger<DnsService> _logger;
        private readonly DnsOptions _options;
        private readonly Dictionary<string, DnsCacheItem> _cache;
        private readonly object _cacheLock = new object();
        private long _processedQueries;
        private long _successfulQueries;
        private long _failedQueries;
        private long _totalQueryTime;
        private long _cacheHits;
        private long _cacheMisses;
        private readonly DateTime _startTime = DateTime.UtcNow;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="options">配置选项</param>
        public DnsService(ILogger<DnsService> logger, IOptions<DnsOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _cache = new Dictionary<string, DnsCacheItem>();
            
            _logger.LogInformation("DnsService初始化成功，配置选项：EnableCache={EnableCache}, CacheSize={CacheSize}, DnsServers={DnsServers}",
                _options.EnableCache, _options.CacheSize, string.Join(", ", _options.DnsServers));
        }
        
        /// <summary>
        /// 生成缓存键
        /// </summary>
        private string GenerateCacheKey(string domain, DnsRecordType recordType)
        {
            return $"{domain.ToLower()}:{recordType}";
        }
        
        /// <summary>
        /// 从缓存获取结果
        /// </summary>
        private DnsQueryResult? GetFromCache(string domain, DnsRecordType recordType)
        {
            if (!_options.EnableCache)
                return null;
            
            var key = GenerateCacheKey(domain, recordType);
            
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(key, out var cacheItem))
                {
                    if (!cacheItem.IsExpired(_options.CacheExpirationSeconds))
                    {
                        _logger.LogDebug("缓存命中：Domain={Domain}, Type={RecordType}", domain, recordType);
                        Interlocked.Increment(ref _cacheHits);
                        return cacheItem.Result;
                    }
                    
                    // 缓存过期，移除
                    _cache.Remove(key);
                }
            }
            
            Interlocked.Increment(ref _cacheMisses);
            return null;
        }
        
        /// <summary>
        /// 添加到缓存
        /// </summary>
        private void AddToCache(DnsQueryResult result)
        {
            if (!_options.EnableCache || !result.Success)
                return;
            
            var key = GenerateCacheKey(result.Domain, result.RecordType);
            
            lock (_cacheLock)
            {
                // 如果缓存已满，移除最旧的项
                if (_cache.Count >= _options.CacheSize)
                {
                    var oldestKey = _cache.OrderBy(item => item.Value.CreatedAt).First().Key;
                    _cache.Remove(oldestKey);
                }
                
                _cache[key] = new DnsCacheItem
                {
                    Key = key,
                    Result = result,
                    CreatedAt = DateTime.UtcNow
                };
            }
        }
        
        /// <summary>
        /// 查询DNS记录
        /// </summary>
        public async Task<DnsQueryResult> QueryAsync(string domain, DnsRecordType recordType)
        {
            Interlocked.Increment(ref _processedQueries);
            
            // 先检查缓存
            var cacheResult = GetFromCache(domain, recordType);
            if (cacheResult != null)
            {
                return cacheResult;
            }
            
            var stopwatch = Stopwatch.StartNew();
            var result = new DnsQueryResult
            {
                Domain = domain,
                RecordType = recordType,
                Success = true
            };
            
            try
            {
                _logger.LogDebug("开始DNS查询：Domain={Domain}, Type={RecordType}", domain, recordType);
                
                switch (recordType)
                {
                    case DnsRecordType.A:
                        await QueryARecordsAsync(domain, result);
                        break;
                    case DnsRecordType.AAAA:
                        await QueryAAAARecordsAsync(domain, result);
                        break;
                    case DnsRecordType.MX:
                        await QueryMXRecordsAsync(domain, result);
                        break;
                    case DnsRecordType.NS:
                        await QueryNSRecordsAsync(domain, result);
                        break;
                    case DnsRecordType.CNAME:
                        await QueryCNAMERecordsAsync(domain, result);
                        break;
                    case DnsRecordType.TXT:
                        await QueryTXTRecordsAsync(domain, result);
                        break;
                    default:
                        result.Success = false;
                        result.ErrorMessage = $"不支持的记录类型：{recordType}";
                        break;
                }
                
                if (result.Success)
                {
                    Interlocked.Increment(ref _successfulQueries);
                    AddToCache(result);
                }
                else
                {
                    Interlocked.Increment(ref _failedQueries);
                }
                
                _logger.LogDebug("DNS查询完成：Domain={Domain}, Type={RecordType}, Success={Success}, ResultsCount={ResultsCount}", 
                    domain, recordType, result.Success, result.Results.Count);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Interlocked.Increment(ref _failedQueries);
                
                _logger.LogError(ex, "DNS查询失败：Domain={Domain}, Type={RecordType}", domain, recordType);
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                Interlocked.Add(ref _totalQueryTime, result.ExecutionTimeMs);
            }
            
            return result;
        }
        
        /// <summary>
        /// 批量查询DNS记录
        /// </summary>
        public async Task<List<DnsQueryResult>> BatchQueryAsync(List<(string Domain, DnsRecordType RecordType)> queries)
        {
            var tasks = queries.Select(query => QueryAsync(query.Domain, query.RecordType));
            return (await Task.WhenAll(tasks)).ToList();
        }
        
        /// <summary>
        /// 查询A记录
        /// </summary>
        private async Task QueryARecordsAsync(string domain, DnsQueryResult result)
        {
            var ips = await Dns.GetHostAddressesAsync(domain);
            foreach (var ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    result.Results.Add(ip.ToString());
                }
            }
        }
        
        /// <summary>
        /// 查询AAAA记录
        /// </summary>
        private async Task QueryAAAARecordsAsync(string domain, DnsQueryResult result)
        {
            var ips = await Dns.GetHostAddressesAsync(domain);
            foreach (var ip in ips)
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    result.Results.Add(ip.ToString());
                }
            }
        }
        
        /// <summary>
        /// 查询MX记录
        /// </summary>
        private async Task QueryMXRecordsAsync(string domain, DnsQueryResult result)
        {
            var records = await Dns.GetHostEntryAsync(domain);
            // 注意：.NET 内置 Dns 类不直接支持 MX 记录查询，这里只是示例
            // 实际实现中可能需要使用第三方库或手动构建 DNS 查询
            result.Results.Add($"{domain} MX record example");
        }
        
        /// <summary>
        /// 查询NS记录
        /// </summary>
        private async Task QueryNSRecordsAsync(string domain, DnsQueryResult result)
        {
            // 注意：.NET 内置 Dns 类不直接支持 NS 记录查询，这里只是示例
            result.Results.Add($"{domain} NS record example");
        }
        
        /// <summary>
        /// 查询CNAME记录
        /// </summary>
        private async Task QueryCNAMERecordsAsync(string domain, DnsQueryResult result)
        {
            // 注意：.NET 内置 Dns 类不直接支持 CNAME 记录查询，这里只是示例
            result.Results.Add($"{domain} CNAME record example");
        }
        
        /// <summary>
        /// 查询TXT记录
        /// </summary>
        private async Task QueryTXTRecordsAsync(string domain, DnsQueryResult result)
        {
            // 注意：.NET 内置 Dns 类不直接支持 TXT 记录查询，这里只是示例
            result.Results.Add($"{domain} TXT record example");
        }
        
        /// <summary>
        /// 查询IPv4地址 (A记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryAAsync(string domain)
        {
            return await QueryAsync(domain, DnsRecordType.A);
        }
        
        /// <summary>
        /// 查询IPv6地址 (AAAA记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryAAAAAsync(string domain)
        {
            return await QueryAsync(domain, DnsRecordType.AAAA);
        }
        
        /// <summary>
        /// 查询邮件服务器 (MX记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryMXAsync(string domain)
        {
            return await QueryAsync(domain, DnsRecordType.MX);
        }
        
        /// <summary>
        /// 查询名称服务器 (NS记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryNSAsync(string domain)
        {
            return await QueryAsync(domain, DnsRecordType.NS);
        }
        
        /// <summary>
        /// 查询别名记录 (CNAME)
        /// </summary>
        public async Task<DnsQueryResult> QueryCNAMEAsync(string domain)
        {
            return await QueryAsync(domain, DnsRecordType.CNAME);
        }
        
        /// <summary>
        /// 查询文本记录 (TXT)
        /// </summary>
        public async Task<DnsQueryResult> QueryTXTAsync(string domain)
        {
            return await QueryAsync(domain, DnsRecordType.TXT);
        }
        
        /// <summary>
        /// 刷新缓存
        /// </summary>
        public Task<bool> FlushCacheAsync()
        {
            lock (_cacheLock)
            {
                _cache.Clear();
            }
            
            _logger.LogInformation("DNS缓存已清空");
            return Task.FromResult(true);
        }
        
        /// <summary>
        /// 获取DNS服务状态
        /// </summary>
        public Task<DnsStatus> GetStatusAsync()
        {
            var processed = Interlocked.Read(ref _processedQueries);
            var successful = Interlocked.Read(ref _successfulQueries);
            var failed = Interlocked.Read(ref _failedQueries);
            var totalTime = Interlocked.Read(ref _totalQueryTime);
            var cacheHits = Interlocked.Read(ref _cacheHits);
            var cacheMisses = Interlocked.Read(ref _cacheMisses);
            
            var averageTime = processed > 0 ? (double)totalTime / processed : 0;
            var cacheHitRate = (cacheHits + cacheMisses) > 0 ? (double)cacheHits / (cacheHits + cacheMisses) * 100 : 0;
            
            var status = new DnsStatus
            {
                IsRunning = true,
                ProcessedQueries = processed,
                SuccessfulQueries = successful,
                FailedQueries = failed,
                AverageQueryTimeMs = Math.Round(averageTime, 2),
                CacheHitRate = Math.Round(cacheHitRate, 2),
                CurrentCacheItems = _cache.Count,
                MaxCacheItems = _options.CacheSize,
                StartTime = _startTime
            };
            
            return Task.FromResult(status);
        }
        
        /// <summary>
        /// 重置DNS服务状态
        /// </summary>
        public Task<bool> ResetStatusAsync()
        {
            Interlocked.Exchange(ref _processedQueries, 0);
            Interlocked.Exchange(ref _successfulQueries, 0);
            Interlocked.Exchange(ref _failedQueries, 0);
            Interlocked.Exchange(ref _totalQueryTime, 0);
            Interlocked.Exchange(ref _cacheHits, 0);
            Interlocked.Exchange(ref _cacheMisses, 0);
            
            _logger.LogInformation("DNS服务状态已重置");
            return Task.FromResult(true);
        }
    }
    
    /// <summary>
    /// DNS AOT引擎
    /// </summary>
    public class DnsAotEngine
    {
        private readonly ILogger<DnsAotEngine> _logger;
        private readonly IDnsService _dnsService;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="dnsService">DNS服务</param>
        public DnsAotEngine(ILogger<DnsAotEngine> logger, IDnsService dnsService)
        {
            _logger = logger;
            _dnsService = dnsService;
            
            _logger.LogInformation("DnsAotEngine初始化成功");
        }
        
        /// <summary>
        /// 查询DNS记录
        /// </summary>
        public async Task<DnsQueryResult> QueryAsync(string domain, DnsRecordType recordType)
        {
            return await _dnsService.QueryAsync(domain, recordType);
        }
        
        /// <summary>
        /// 批量查询DNS记录
        /// </summary>
        public async Task<List<DnsQueryResult>> BatchQueryAsync(List<(string Domain, DnsRecordType RecordType)> queries)
        {
            return await _dnsService.BatchQueryAsync(queries);
        }
        
        /// <summary>
        /// 查询IPv4地址 (A记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryAAsync(string domain)
        {
            return await _dnsService.QueryAAsync(domain);
        }
        
        /// <summary>
        /// 查询IPv6地址 (AAAA记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryAAAAAsync(string domain)
        {
            return await _dnsService.QueryAAAAAsync(domain);
        }
        
        /// <summary>
        /// 查询邮件服务器 (MX记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryMXAsync(string domain)
        {
            return await _dnsService.QueryMXAsync(domain);
        }
        
        /// <summary>
        /// 查询名称服务器 (NS记录)
        /// </summary>
        public async Task<DnsQueryResult> QueryNSAsync(string domain)
        {
            return await _dnsService.QueryNSAsync(domain);
        }
        
        /// <summary>
        /// 查询别名记录 (CNAME)
        /// </summary>
        public async Task<DnsQueryResult> QueryCNAMEAsync(string domain)
        {
            return await _dnsService.QueryCNAMEAsync(domain);
        }
        
        /// <summary>
        /// 查询文本记录 (TXT)
        /// </summary>
        public async Task<DnsQueryResult> QueryTXTAsync(string domain)
        {
            return await _dnsService.QueryTXTAsync(domain);
        }
        
        /// <summary>
        /// 刷新缓存
        /// </summary>
        public async Task<bool> FlushCacheAsync()
        {
            return await _dnsService.FlushCacheAsync();
        }
        
        /// <summary>
        /// 获取DNS服务状态
        /// </summary>
        public async Task<DnsStatus> GetStatusAsync()
        {
            return await _dnsService.GetStatusAsync();
        }
        
        /// <summary>
        /// 重置DNS服务状态
        /// </summary>
        public async Task<bool> ResetStatusAsync()
        {
            return await _dnsService.ResetStatusAsync();
        }
    }
    
    /// <summary>
    /// DNS扩展
    /// </summary>
    public static class DnsExtensions
    {
        /// <summary>
        /// 注册DNS服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDns(this IServiceCollection services)
        {
            services.AddSingleton<IDnsService, DnsService>();
            services.AddSingleton<DnsAotEngine>();
            
            return services;
        }
        
        /// <summary>
        /// 注册DNS服务并配置选项
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configureOptions">配置选项的委托</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddDns(this IServiceCollection services, Action<DnsOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            services.Configure(configureOptions);
            services.AddDns();
            
            return services;
        }
    }
    
    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 构建主机
            var builder = Host.CreateApplicationBuilder(args);
            
            // 配置DNS选项
            builder.Configuration.AddJsonFile("dns_aot.setting.json", optional: true);
            builder.Services.Configure<DnsOptions>(builder.Configuration.GetSection("Dns"));
            
            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);
            
            // 注册服务
            builder.Services.AddDns();
            
            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;
            
            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<DnsAotEngine>();
            
            // 解析命令行参数
            if (args.Length < 1)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  dns_aot.exe <command> [arguments]");
                Console.WriteLine("  ");
                Console.WriteLine("命令:");
                Console.WriteLine("  query    查询DNS记录");
                Console.WriteLine("  a        查询A记录 (IPv4)");
                Console.WriteLine("  aaaa     查询AAAA记录 (IPv6)");
                Console.WriteLine("  mx       查询MX记录 (邮件)");
                Console.WriteLine("  ns       查询NS记录 (名称服务器)");
                Console.WriteLine("  cname    查询CNAME记录 (别名)");
                Console.WriteLine("  txt      查询TXT记录 (文本)");
                Console.WriteLine("  batch    批量查询DNS记录");
                Console.WriteLine("  status   获取DNS服务状态");
                Console.WriteLine("  reset    重置DNS服务状态");
                Console.WriteLine("  flush    刷新DNS缓存");
                Console.WriteLine("  demo     运行DNS演示");
                Console.WriteLine("  ");
                Console.WriteLine("示例:");
                Console.WriteLine("  dns_aot.exe a example.com");
                Console.WriteLine("  dns_aot.exe query example.com A");
                Console.WriteLine("  dns_aot.exe status");
                return 1;
            }
            
            try
            {
                string command = args[0].ToLower();
                
                switch (command)
                {
                    case "query":
                        if (args.Length < 3)
                        {
                            Console.WriteLine("用法: dns_aot.exe query <domain> <record_type>");
                            return 1;
                        }
                        
                        var domain = args[1];
                        if (Enum.TryParse<DnsRecordType>(args[2], true, out var recordType))
                        {
                            var result = await engine.QueryAsync(domain, recordType);
                            PrintQueryResult(result);
                            return result.Success ? 0 : 1;
                        }
                        else
                        {
                            Console.WriteLine($"无效的记录类型: {args[2]}");
                            return 1;
                        }
                        
                    case "a":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("用法: dns_aot.exe a <domain>");
                            return 1;
                        }
                        
                        var aResult = await engine.QueryAAsync(args[1]);
                        PrintQueryResult(aResult);
                        return aResult.Success ? 0 : 1;
                        
                    case "aaaa":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("用法: dns_aot.exe aaaa <domain>");
                            return 1;
                        }
                        
                        var aaaaResult = await engine.QueryAAAAAsync(args[1]);
                        PrintQueryResult(aaaaResult);
                        return aaaaResult.Success ? 0 : 1;
                        
                    case "mx":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("用法: dns_aot.exe mx <domain>");
                            return 1;
                        }
                        
                        var mxResult = await engine.QueryMXAsync(args[1]);
                        PrintQueryResult(mxResult);
                        return mxResult.Success ? 0 : 1;
                        
                    case "ns":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("用法: dns_aot.exe ns <domain>");
                            return 1;
                        }
                        
                        var nsResult = await engine.QueryNSAsync(args[1]);
                        PrintQueryResult(nsResult);
                        return nsResult.Success ? 0 : 1;
                        
                    case "cname":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("用法: dns_aot.exe cname <domain>");
                            return 1;
                        }
                        
                        var cnameResult = await engine.QueryCNAMEAsync(args[1]);
                        PrintQueryResult(cnameResult);
                        return cnameResult.Success ? 0 : 1;
                        
                    case "txt":
                        if (args.Length < 2)
                        {
                            Console.WriteLine("用法: dns_aot.exe txt <domain>");
                            return 1;
                        }
                        
                        var txtResult = await engine.QueryTXTAsync(args[1]);
                        PrintQueryResult(txtResult);
                        return txtResult.Success ? 0 : 1;
                        
                    case "status":
                        var status = await engine.GetStatusAsync();
                        PrintStatus(status);
                        return 0;
                        
                    case "reset":
                        var resetResult = await engine.ResetStatusAsync();
                        Console.WriteLine($"重置状态: {(resetResult ? "成功" : "失败"}");
                        return resetResult ? 0 : 1;
                        
                    case "flush":
                        var flushResult = await engine.FlushCacheAsync();
                        Console.WriteLine($"刷新缓存: {(flushResult ? "成功" : "失败"}");
                        return flushResult ? 0 : 1;
                        
                    case "demo":
                        Console.WriteLine("运行DNS演示...");
                        
                        // 查询A记录
                        Console.WriteLine("\n1. 查询A记录 (example.com)...");
                        var demoAResult = await engine.QueryAAsync("example.com");
                        PrintQueryResult(demoAResult);
                        
                        // 查询AAAA记录
                        Console.WriteLine("\n2. 查询AAAA记录 (example.com)...");
                        var demoAAAAResult = await engine.QueryAAAAAsync("example.com");
                        PrintQueryResult(demoAAAAResult);
                        
                        // 查询MX记录
                        Console.WriteLine("\n3. 查询MX记录 (gmail.com)...");
                        var demoMXResult = await engine.QueryMXAsync("gmail.com");
                        PrintQueryResult(demoMXResult);
                        
                        // 查看状态
                        Console.WriteLine("\n4. DNS服务状态...");
                        var demoStatus = await engine.GetStatusAsync();
                        PrintStatus(demoStatus);
                        
                        Console.WriteLine("\n演示完成！");
                        return 0;
                        
                    default:
                        Console.WriteLine($"未知命令: {command}");
                        return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误: {ex.Message}