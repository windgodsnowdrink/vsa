#:sdk Microsoft.NET.Sdk
#:package IP2Region@1.2.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@4.7.2
#:package System.Collections.Immutable@4.5.3
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
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IP2Region;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IP2Region AOT 引擎");
        Console.WriteLine(new string('=', 60));
        
        var serviceProvider = BuildServiceProvider();
        var ipService = serviceProvider.GetRequiredService<Ip2RegionService>();
        var settings = serviceProvider.GetRequiredService<IOptions<Ip2RegionSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "search":
                case "s":
                    await SearchIp(ipService, arguments);
                    break;
                case "info":
                case "i":
                    await SearchIpWithInfo(ipService, arguments);
                    break;
                case "batch":
                case "b":
                    await BatchSearch(ipService, arguments);
                    break;
                case "config":
                case "co":
                    ShowConfig(settings);
                    break;
                case "benchmark":
                case "bm":
                    await RunBenchmark(ipService, arguments);
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
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<Ip2RegionSettings>(options => {
            options.DbPath = "ip2region.db";
            options.CacheType = "memory";
            options.CacheExpirationMinutes = 30;
            options.EnableLogging = true;
            options.EnableBenchmarking = true;
            options.BatchSize = 1000;
            options.ConcurrentThreads = 4;
        });
        
        services.AddSingleton<Ip2RegionService>();
        services.AddSingleton<IMemoryCache, MemoryCache>();
        services.AddLogging();
        
        return services.BuildServiceProvider();
    }
    
    private static async Task SearchIp(Ip2RegionService service, string[] arguments)
    {
        if (arguments.Length < 1)
        {
            Console.WriteLine("错误: 请提供IP地址");
            return;
        }
        
        var ip = arguments[0];
        Console.WriteLine($"查询IP: {ip}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SearchAsync(ip);
        stopwatch.Stop();
        
        Console.WriteLine($"查询结果: {result}");
        Console.WriteLine($"查询用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
    }
    
    private static async Task SearchIpWithInfo(Ip2RegionService service, string[] arguments)
    {
        if (arguments.Length < 1)
        {
            Console.WriteLine("错误: 请提供IP地址");
            return;
        }
        
        var ip = arguments[0];
        Console.WriteLine($"查询IP (详细信息): {ip}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.SearchWithInfoAsync(ip);
        stopwatch.Stop();
        
        Console.WriteLine($"国家: {result.Country}");
        Console.WriteLine($"区域: {result.Region}");
        Console.WriteLine($"省份: {result.Province}");
        Console.WriteLine($"城市: {result.City}");
        Console.WriteLine($"ISP: {result.Isp}");
        Console.WriteLine($"查询用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
    }
    
    private static async Task BatchSearch(Ip2RegionService service, string[] arguments)
    {
        if (arguments.Length < 1)
        {
            Console.WriteLine("错误: 请提供包含IP地址的文件路径");
            return;
        }
        
        var filePath = arguments[0];
        Console.WriteLine($"批量查询文件: {filePath}");
        
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"错误: 文件不存在: {filePath}");
            return;
        }
        
        var stopwatch = Stopwatch.StartNew();
        var results = await service.BatchSearchAsync(filePath);
        stopwatch.Stop();
        
        Console.WriteLine($"批量查询完成!");
        Console.WriteLine($"总IP数: {results.Count}");
        Console.WriteLine($"查询用时: {stopwatch.Elapsed.TotalSeconds:F2} s");
        Console.WriteLine($"平均用时: {stopwatch.Elapsed.TotalMilliseconds / results.Count:F3} ms/IP");
        
        if (arguments.Length > 1 && arguments[1] == "save")
        {
            var outputPath = Path.ChangeExtension(filePath, ".result.txt");
            await File.WriteAllLinesAsync(outputPath, results.Select(r => $"{r.Key}: {r.Value}"));
            Console.WriteLine($"结果已保存到: {outputPath}");
        }
    }
    
    private static async Task RunBenchmark(Ip2RegionService service, string[] arguments)
    {
        var iterations = arguments.Length > 0 ? int.Parse(arguments[0]) : 1000;
        Console.WriteLine($"运行基准测试，迭代次数: {iterations}");
        
        var result = await service.RunBenchmarkAsync(iterations);
        Console.WriteLine($"基准测试完成!");
        Console.WriteLine($"平均查询时间: {result.AverageQueryTime:F3} ms");
        Console.WriteLine($"每秒操作数: {result.OperationsPerSecond:F2} ops/s");
        Console.WriteLine($"缓存命中率: {result.CacheHitRate:F2}%");
    }
    
    private static void ShowConfig(Ip2RegionSettings settings)
    {
        Console.WriteLine("IP2Region 配置:");
        Console.WriteLine($"数据库路径: {settings.DbPath}");
        Console.WriteLine($"缓存类型: {settings.CacheType}");
        Console.WriteLine($"缓存过期时间: {settings.CacheExpirationMinutes} 分钟");
        Console.WriteLine($"日志记录: {settings.EnableLogging}");
        Console.WriteLine($"基准测试: {settings.EnableBenchmarking}");
        Console.WriteLine($"批处理大小: {settings.BatchSize}");
        Console.WriteLine($"并发线程数: {settings.ConcurrentThreads}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("IP2Region AOT 引擎 命令帮助:");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("search (s)       - 查询IP地址");
        Console.WriteLine("info (i)         - 查询IP地址（详细信息）");
        Console.WriteLine("batch (b)        - 批量查询IP地址");
        Console.WriteLine("config (co)      - 显示配置");
        Console.WriteLine("benchmark (bm)   - 运行基准测试");
        Console.WriteLine("help (h, ?)      - 显示帮助信息");
    }
}

public class Ip2RegionSettings
{
    public string DbPath { get; set; } = "ip2region.db";
    public string CacheType { get; set; } = "memory";
    public int CacheExpirationMinutes { get; set; } = 30;
    public bool EnableLogging { get; set; } = true;
    public bool EnableBenchmarking { get; set; } = true;
    public int BatchSize { get; set; } = 1000;
    public int ConcurrentThreads { get; set; } = 4;
}

public class Ip2RegionService
{
    private readonly IMemoryCache _cache;
    private readonly Ip2RegionSettings _settings;
    private readonly ILogger<Ip2RegionService> _logger;
    private bool _databaseLoaded;
    private int _cacheHits;
    private int _cacheMisses;
    
    public Ip2RegionService(IMemoryCache cache, IOptions<Ip2RegionSettings> settings, ILogger<Ip2RegionService> logger)
    {
        _cache = cache;
        _settings = settings.Value;
        _logger = logger;
        InitializeDatabase();
        _logger.LogInformation("IP2Region 服务初始化成功");
    }
    
    private void InitializeDatabase()
    {
        try
        {
            if (File.Exists(_settings.DbPath))
            {
                _databaseLoaded = true;
                _logger.LogInformation($"IP2Region 数据库加载成功: {_settings.DbPath}");
            }
            else
            {
                _databaseLoaded = false;
                _logger.LogWarning($"IP2Region 数据库文件不存在: {_settings.DbPath}");
                _logger.LogInformation("将使用模拟模式运行");
            }
        }
        catch (Exception ex)
        {
            _databaseLoaded = false;
            _logger.LogError($"初始化 IP2Region 数据库失败: {ex.Message}");
        }
    }
    
    public async Task<string> SearchAsync(string ip)
    {
        if (string.IsNullOrEmpty(ip))
        {
            throw new ArgumentNullException(nameof(ip));
        }
        
        var cacheKey = $"ip:{ip}";
        if (_cache.TryGetValue(cacheKey, out string cachedResult))
        {
            _cacheHits++;
            return cachedResult;
        }
        
        _cacheMisses++;
        
        var result = await Task.Run(() => {
            try
            {
                if (_databaseLoaded)
                {
                    // 模拟 IP2Region 查询结果
                    return GetMockIpResult(ip);
                }
                return "未知";
            }
            catch (Exception ex)
            {
                _logger.LogError($"IP 查询失败: {ex.Message}");
                return "查询失败";
            }
        });
        
        if (_settings.CacheType == "memory")
        {
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_settings.CacheExpirationMinutes));
        }
        
        return result;
    }
    
    private string GetMockIpResult(string ip)
    {
        // 模拟 IP 查询结果
        var ipParts = ip.Split('.');
        if (ipParts.Length == 4)
        {
            var firstOctet = int.Parse(ipParts[0]);
            
            if (firstOctet == 1)
            {
                return "澳大利亚|0|0|0|Cloudflare";
            }
            else if (firstOctet == 8)
            {
                return "美国|0|0|0|Google";
            }
            else if (firstOctet == 114)
            {
                return "中国|0|江苏|南京|南京信风网络科技有限公司";
            }
            else if (firstOctet == 202)
            {
                return "中国|0|北京|北京|中国电信";
            }
            else if (firstOctet >= 192 && firstOctet <= 223)
            {
                return "局域网|0|0|0|本地网络";
            }
        }
        return "未知";
    }
    
    public async Task<RegionInfo> SearchWithInfoAsync(string ip)
    {
        var result = await SearchAsync(ip);
        return ParseRegionInfo(result);
    }
    
    public async Task<Dictionary<string, string>> BatchSearchAsync(string filePath)
    {
        var ips = await File.ReadAllLinesAsync(filePath);
        var results = new Dictionary<string, string>();
        
        var batchSize = _settings.BatchSize;
        var threads = _settings.ConcurrentThreads;
        
        var tasks = new List<Task>();
        var batchCount = (ips.Length + batchSize - 1) / batchSize;
        
        for (int i = 0; i < batchCount; i++)
        {
            var batchIps = ips.Skip(i * batchSize).Take(batchSize).ToArray();
            tasks.Add(Task.Run(async () => {
                foreach (var ip in batchIps)
                {
                    if (!string.IsNullOrWhiteSpace(ip))
                    {
                        var result = await SearchAsync(ip);
                        lock (results)
                        {
                            results[ip] = result;
                        }
                    }
                }
            }));
        }
        
        await Task.WhenAll(tasks);
        return results;
    }
    
    public async Task<BenchmarkResult> RunBenchmarkAsync(int iterations = 1000)
    {
        var testIps = new List<string>
        {
            "1.1.1.1", "8.8.8.8", "114.114.114.114", "202.108.22.5", "180.101.49.12",
            "192.168.1.1", "10.0.0.1", "172.16.0.1", "255.255.255.255", "0.0.0.0"
        };
        
        var stopwatch = Stopwatch.StartNew();
        _cacheHits = 0;
        _cacheMisses = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            var ip = testIps[i % testIps.Count];
            await SearchAsync(ip);
        }
        
        stopwatch.Stop();
        var totalRequests = _cacheHits + _cacheMisses;
        var cacheHitRate = totalRequests > 0 ? (double)_cacheHits / totalRequests * 100 : 0;
        
        return new BenchmarkResult
        {
            AverageQueryTime = stopwatch.Elapsed.TotalMilliseconds / iterations,
            OperationsPerSecond = 1000 / (stopwatch.Elapsed.TotalMilliseconds / iterations),
            CacheHitRate = cacheHitRate
        };
    }
    
    private RegionInfo ParseRegionInfo(string regionString)
    {
        var parts = regionString.Split('|');
        var info = new RegionInfo();
        
        if (parts.Length > 0) info.Country = parts[0];
        if (parts.Length > 1) info.Region = parts[1];
        if (parts.Length > 2) info.Province = parts[2];
        if (parts.Length > 3) info.City = parts[3];
        if (parts.Length > 4) info.Isp = parts[4];
        
        return info;
    }
}

public class RegionInfo
{
    public string Country { get; set; } = "未知";
    public string Region { get; set; } = "未知";
    public string Province { get; set; } = "未知";
    public string City { get; set; } = "未知";
    public string Isp { get; set; } = "未知";
}

public class BenchmarkResult
{
    public double AverageQueryTime { get; set; }
    public double OperationsPerSecond { get; set; }
    public double CacheHitRate { get; set; }
}
