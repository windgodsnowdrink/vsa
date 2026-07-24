#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@8.0.0
#:package System.Text.RegularExpressions@4.3.1
#:package System.Collections.Immutable@8.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:package System.Security.Cryptography@4.3.0
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
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IDGen AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var idgenService = serviceProvider.GetRequiredService<IdgenService>();
        var settings = serviceProvider.GetRequiredService<IOptions<IdgenSettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var arguments = args.Skip(1).ToArray();
        
        try
        {
            switch (command)
            {
                case "generate":
                case "g":
                    await GenerateId(idgenService, arguments);
                    break;
                case "batch":
                case "b":
                    await GenerateBatchIds(idgenService, arguments);
                    break;
                case "validate":
                case "v":
                    await ValidateId(idgenService, arguments);
                    break;
                case "decode":
                case "d":
                    await DecodeId(idgenService, arguments);
                    break;
                case "benchmark":
                case "bm":
                    await RunBenchmark(idgenService, arguments);
                    break;
                case "config":
                case "co":
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
            await idgenService.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IdgenSettings>(options => {
            options.DefaultAlgorithm = "snowflake";
            options.SnowflakeWorkerId = 1;
            options.SnowflakeDatacenterId = 1;
            options.SnowflakeEpoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            options.SnowflakeDriftThreshold = 1000;
            options.UlidEncoding = "base32";
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.CacheExpiry = TimeSpan.FromMinutes(5);
            options.EnableRateLimiting = false;
            options.MaxRequestsPerSecond = 1000;
        });
        
        services.AddSingleton<IdgenService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task GenerateId(IdgenService service, string[] arguments)
    {
        var algorithm = arguments.Length > 0 ? arguments[0] : "snowflake";
        var count = arguments.Length > 1 ? int.Parse(arguments[1]) : 1;
        
        Console.WriteLine($"生成 ID: 算法={algorithm}, 数量={count}");
        
        var stopwatch = Stopwatch.StartNew();
        var ids = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            var id = await service.GenerateIdAsync(algorithm);
            ids.Add(id);
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"ID 生成完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每 ID: {stopwatch.Elapsed.TotalMilliseconds / count:F3} ms");
        
        foreach (var id in ids)
        {
            Console.WriteLine($"ID: {id}");
        }
    }
    
    private static async Task GenerateBatchIds(IdgenService service, string[] arguments)
    {
        var algorithm = arguments.Length > 0 ? arguments[0] : "snowflake";
        var count = arguments.Length > 1 ? int.Parse(arguments[1]) : 10;
        
        Console.WriteLine($"批量生成 ID: 算法={algorithm}, 数量={count}");
        
        var stopwatch = Stopwatch.StartNew();
        var ids = await service.GenerateBatchIdsAsync(algorithm, count);
        stopwatch.Stop();
        
        Console.WriteLine($"批量生成完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"每秒生成: {(count / stopwatch.Elapsed.TotalSeconds):F2} IDs/s");
        Console.WriteLine($"平均每 ID: {stopwatch.Elapsed.TotalMilliseconds / count:F3} ms");
        
        for (int i = 0; i < Math.Min(10, ids.Count); i++)
        {
            Console.WriteLine($"ID {i + 1}: {ids[i]}");
        }
        
        if (ids.Count > 10)
        {
            Console.WriteLine($"... 还有 {ids.Count - 10} 个 ID");
        }
    }
    
    private static async Task ValidateId(IdgenService service, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供算法和 ID");
            return;
        }
        
        var algorithm = arguments[0];
        var id = arguments[1];
        Console.WriteLine($"验证 ID: 算法={algorithm}, ID={id}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ValidateIdAsync(algorithm, id);
        stopwatch.Stop();
        
        Console.WriteLine($"ID 验证完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"验证状态: {(result.Valid ? "有效" : "无效"}");
        
        if (!result.Valid)
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task DecodeId(IdgenService service, string[] arguments)
    {
        if (arguments.Length < 2)
        {
            Console.WriteLine("错误: 请提供算法和 ID");
            return;
        }
        
        var algorithm = arguments[0];
        var id = arguments[1];
        Console.WriteLine($"解码 ID: 算法={algorithm}, ID={id}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.DecodeIdAsync(algorithm, id);
        stopwatch.Stop();
        
        Console.WriteLine($"ID 解码完成! 用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"解码状态: {(result.Success ? "成功" : "失败"}");
        
        if (result.Success)
        {
            Console.WriteLine($"解码结果:");
            foreach (var kvp in result.DecodedData)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }
        }
        else
        {
            Console.WriteLine($"错误: {result.Error}");
        }
    }
    
    private static async Task RunBenchmark(IdgenService service, string[] arguments)
    {
        var algorithm = arguments.Length > 0 ? arguments[0] : "snowflake";
        var iterations = arguments.Length > 1 ? int.Parse(arguments[1]) : 10000;
        
        Console.WriteLine($"运行基准测试: 算法={algorithm}, 迭代次数={iterations}");
        
        var stopwatch = Stopwatch.StartNew();
        int successes = 0;
        int failures = 0;
        
        for (int i = 0; i < iterations; i++)
        {
            try
            {
                var id = await service.GenerateIdAsync(algorithm);
                var validateResult = await service.ValidateIdAsync(algorithm, id);
                
                if (validateResult.Valid)
                {
                    successes++;
                }
                else
                {
                    failures++;
                }
            }
            catch
            {
                failures++;
            }
        }
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var operationsPerSecond = iterations / elapsedSeconds;
        
        Console.WriteLine($"基准测试完成!");
        Console.WriteLine($"总用时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"成功: {successes}");
        Console.WriteLine($"失败: {failures}");
        Console.WriteLine($"每秒操作数: {operationsPerSecond:F2} ops/s");
        Console.WriteLine($"平均每操作: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
    
    private static void ShowConfig(IdgenSettings settings)
    {
        Console.WriteLine("IDGen 配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"默认算法: {settings.DefaultAlgorithm}");
        Console.WriteLine($"Snowflake Worker ID: {settings.SnowflakeWorkerId}");
        Console.WriteLine($"Snowflake Datacenter ID: {settings.SnowflakeDatacenterId}");
        Console.WriteLine($"Snowflake Epoch: {settings.SnowflakeEpoch}");
        Console.WriteLine($"Snowflake Drift Threshold: {settings.SnowflakeDriftThreshold} ms");
        Console.WriteLine($"ULID 编码: {settings.UlidEncoding}");
        Console.WriteLine($"启用缓存: {settings.EnableCache}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"缓存过期: {settings.CacheExpiry}");
        Console.WriteLine($"启用速率限制: {settings.EnableRateLimiting}");
        Console.WriteLine($"最大每秒请求数: {settings.MaxRequestsPerSecond}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("IDGen AOT 引擎 命令帮助:");
        Console.WriteLine("=" * 60);
        Console.WriteLine("generate (g)    - 生成 ID");
        Console.WriteLine("batch (b)       - 批量生成 ID");
        Console.WriteLine("validate (v)    - 验证 ID");
        Console.WriteLine("decode (d)      - 解码 ID");
        Console.WriteLine("benchmark (bm)  - 运行基准测试");
        Console.WriteLine("config (co)     - 显示配置信息");
        Console.WriteLine("help (h, ?)     - 显示帮助信息");
    }
}

public class IdgenSettings
{
    public string DefaultAlgorithm { get; set; } = "snowflake";
    public int SnowflakeWorkerId { get; set; } = 1;
    public int SnowflakeDatacenterId { get; set; } = 1;
    public DateTime SnowflakeEpoch { get; set; } = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public int SnowflakeDriftThreshold { get; set; } = 1000;
    public string UlidEncoding { get; set; } = "base32";
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableRateLimiting { get; set; } = false;
    public int MaxRequestsPerSecond { get; set; } = 1000;
}

public class IdGenerationResult
{
    public string Id { get; set; }
    public string Algorithm { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class IdValidationResult
{
    public bool Valid { get; set; }
    public string Error { get; set; }
}

public class IdDecodeResult
{
    public bool Success { get; set; }
    public Dictionary<string, object> DecodedData { get; set; } = new();
    public string Error { get; set; }
}

public class IdgenService : IAsyncDisposable
{
    private readonly ILogger<IdgenService> _logger;
    private readonly IdgenSettings _settings;
    private readonly MemoryCache _cache;
    private readonly object _snowflakeLock = new();
    private readonly object _ulidLock = new();
    private long _snowflakeSequence = 0;
    private long _lastTimestamp = -1;
    
    public IdgenService(ILogger<IdgenService> logger, IOptions<IdgenSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
        
        // 初始化缓存
        var cacheOptions = new MemoryCacheOptions {
            SizeLimit = _settings.CacheSize
        };
        _cache = new MemoryCache(cacheOptions);
        
        _logger.LogInformation("IDGen 服务初始化成功");
    }
    
    public async Task<string> GenerateIdAsync(string algorithm)
    {
        try
        {
            _logger.LogInformation($"生成 ID: 算法={algorithm}");
            
            string id;
            
            switch (algorithm.ToLower())
            {
                case "snowflake":
                    id = GenerateSnowflakeId();
                    break;
                case "snowflake_drift":
                    id = GenerateSnowflakeDriftId();
                    break;
                case "ulid":
                    id = GenerateUlid();
                    break;
                case "uuid":
                    id = GenerateUuid();
                    break;
                default:
                    throw new ArgumentException($"不支持的算法: {algorithm}");
            }
            
            // 缓存生成的 ID
            if (_settings.EnableCache)
            {
                var cacheKey = $"id:{algorithm}:{id}";
                _cache.Set(cacheKey, new { Algorithm = algorithm, GeneratedAt = DateTime.UtcNow }, new MemoryCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = _settings.CacheExpiry,
                    Size = 1
                });
            }
            
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成 ID 失败");
            throw;
        }
    }
    
    public async Task<List<string>> GenerateBatchIdsAsync(string algorithm, int count)
    {
        try
        {
            _logger.LogInformation($"批量生成 ID: 算法={algorithm}, 数量={count}");
            
            var ids = new List<string>();
            
            for (int i = 0; i < count; i++)
            {
                var id = await GenerateIdAsync(algorithm);
                ids.Add(id);
            }
            
            return ids;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量生成 ID 失败");
            throw;
        }
    }
    
    public async Task<IdValidationResult> ValidateIdAsync(string algorithm, string id)
    {
        try
        {
            _logger.LogInformation($"验证 ID: 算法={algorithm}, ID={id}");
            
            switch (algorithm.ToLower())
            {
                case "snowflake":
                    return ValidateSnowflakeId(id);
                case "snowflake_drift":
                    return ValidateSnowflakeDriftId(id);
                case "ulid":
                    return ValidateUlid(id);
                case "uuid":
                    return ValidateUuid(id);
                default:
                    return new IdValidationResult {
                        Valid = false,
                        Error = $"不支持的算法: {algorithm}"
                    };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证 ID 失败");
            return new IdValidationResult {
                Valid = false,
                Error = ex.Message
            };
        }
    }
    
    public async Task<IdDecodeResult> DecodeIdAsync(string algorithm, string id)
    {
        try
        {
            _logger.LogInformation($"解码 ID: 算法={algorithm}, ID={id}");
            
            switch (algorithm.ToLower())
            {
                case "snowflake":
                    return DecodeSnowflakeId(id);
                case "snowflake_drift":
                    return DecodeSnowflakeDriftId(id);
                case "ulid":
                    return DecodeUlid(id);
                case "uuid":
                    return DecodeUuid(id);
                default:
                    return new IdDecodeResult {
                        Success = false,
                        Error = $"不支持的算法: {algorithm}"
                    };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解码 ID 失败");
            return new IdDecodeResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    private string GenerateSnowflakeId()
    {
        lock (_snowflakeLock)
        {
            var timestamp = GetCurrentTimestamp();
            
            // 处理时钟回拨
            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException($"时钟回拨检测到: {timestamp} < {_lastTimestamp}");
            }
            
            // 处理同一毫秒内的序列
            if (timestamp == _lastTimestamp)
            {
                _snowflakeSequence = (_snowflakeSequence + 1) & 4095;
                if (_snowflakeSequence == 0)
                {
                    // 等待下一毫秒
                    timestamp = WaitForNextMillisecond(_lastTimestamp);
                }
            }
            else
            {
                _snowflakeSequence = 0;
            }
            
            _lastTimestamp = timestamp;
            
            // 构建 Snowflake ID
            long id = ((timestamp - GetEpochTimestamp(_settings.SnowflakeEpoch)) << 22)
                     | ((long)_settings.SnowflakeDatacenterId << 17)
                     | ((long)_settings.SnowflakeWorkerId << 12)
                     | _snowflakeSequence;
            
            return id.ToString();
        }
    }
    
    private string GenerateSnowflakeDriftId()
    {
        lock (_snowflakeLock)
        {
            var timestamp = GetCurrentTimestamp();
            
            // 处理时钟回拨（允许一定的漂移）
            if (timestamp < _lastTimestamp)
            {
                var drift = _lastTimestamp - timestamp;
                if (drift > _settings.SnowflakeDriftThreshold)
                {
                    throw new InvalidOperationException($"时钟回拨超过阈值: {drift}ms > {_settings.SnowflakeDriftThreshold}ms");
                }
                // 使用上次的时间戳，增加序列
                timestamp = _lastTimestamp;
            }
            
            // 处理同一毫秒内的序列
            if (timestamp == _lastTimestamp)
            {
                _snowflakeSequence = (_snowflakeSequence + 1) & 4095;
                if (_snowflakeSequence == 0)
                {
                    // 等待下一毫秒
                    timestamp = WaitForNextMillisecond(_lastTimestamp);
                }
            }
            else
            {
                _snowflakeSequence = 0;
            }
            
            _lastTimestamp = timestamp;
            
            // 构建 Snowflake Drift ID
            long id = ((timestamp - GetEpochTimestamp(_settings.SnowflakeEpoch)) << 22)
                     | ((long)_settings.SnowflakeDatacenterId << 17)
                     | ((long)_settings.SnowflakeWorkerId << 12)
                     | _snowflakeSequence;
            
            return id.ToString();
        }
    }
    
    private string GenerateUlid()
    {
        lock (_ulidLock)
        {
            // 生成 ULID
            var ulid = ULID.NewULID();
            
            switch (_settings.UlidEncoding.ToLower())
            {
                case "base32":
                    return ulid.ToString();
                case "hex":
                    return ulid.ToHexString();
                default:
                    return ulid.ToString();
            }
        }
    }
    
    private string GenerateUuid()
    {
        return Guid.NewGuid().ToString();
    }
    
    private IdValidationResult ValidateSnowflakeId(string id)
    {
        if (!long.TryParse(id, out var snowflakeId))
        {
            return new IdValidationResult {
                Valid = false,
                Error = "ID 必须是有效的数字"
            };
        }
        
        // 检查 ID 长度
        var idString = snowflakeId.ToString();
        if (idString.Length < 10 || idString.Length > 20)
        {
            return new IdValidationResult {
                Valid = false,
                Error = "ID 长度无效"
            };
        }
        
        return new IdValidationResult { Valid = true };
    }
    
    private IdValidationResult ValidateSnowflakeDriftId(string id)
    {
        // 与 Snowflake 验证逻辑相同
        return ValidateSnowflakeId(id);
    }
    
    private IdValidationResult ValidateUlid(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return new IdValidationResult {
                Valid = false,
                Error = "ID 不能为空"
            };
        }
        
        try
        {
            ULID.Parse(id);
            return new IdValidationResult { Valid = true };
        }
        catch
        {
            return new IdValidationResult {
                Valid = false,
                Error = "无效的 ULID 格式"
            };
        }
    }
    
    private IdValidationResult ValidateUuid(string id)
    {
        if (!Guid.TryParse(id, out _))
        {
            return new IdValidationResult {
                Valid = false,
                Error = "无效的 UUID 格式"
            };
        }
        
        return new IdValidationResult { Valid = true };
    }
    
    private IdDecodeResult DecodeSnowflakeId(string id)
    {
        if (!long.TryParse(id, out var snowflakeId))
        {
            return new IdDecodeResult {
                Success = false,
                Error = "ID 必须是有效的数字"
            };
        }
        
        // 解析 Snowflake ID
        var timestamp = (snowflakeId >> 22) + GetEpochTimestamp(_settings.SnowflakeEpoch);
        var datacenterId = (snowflakeId >> 17) & 31;
        var workerId = (snowflakeId >> 12) & 31;
        var sequence = snowflakeId & 4095;
        
        var decodedData = new Dictionary<string, object> {
            { "timestamp", timestamp },
            { "datacenter_id", datacenterId },
            { "worker_id", workerId },
            { "sequence", sequence },
            { "generated_at", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp) }
        };
        
        return new IdDecodeResult {
            Success = true,
            DecodedData = decodedData
        };
    }
    
    private IdDecodeResult DecodeSnowflakeDriftId(string id)
    {
        // 与 Snowflake 解码逻辑相同
        return DecodeSnowflakeId(id);
    }
    
    private IdDecodeResult DecodeUlid(string id)
    {
        try
        {
            var ulid = ULID.Parse(id);
            
            var decodedData = new Dictionary<string, object> {
                { "timestamp", ulid.Timestamp },
                { "random", ulid.Random },
                { "generated_at", ulid.DateTime }
            };
            
            return new IdDecodeResult {
                Success = true,
                DecodedData = decodedData
            };
        }
        catch (Exception ex)
        {
            return new IdDecodeResult {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    private IdDecodeResult DecodeUuid(string id)
    {
        if (!Guid.TryParse(id, out var uuid))
        {
            return new IdDecodeResult {
                Success = false,
                Error = "无效的 UUID 格式"
            };
        }
        
        var decodedData = new Dictionary<string, object> {
            { "version", uuid.Version },
            { "variant", uuid.Variant }
        };
        
        return new IdDecodeResult {
            Success = true,
            DecodedData = decodedData
        };
    }
    
    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    
    private long GetEpochTimestamp(DateTime epoch)
    {
        return new DateTimeOffset(epoch).ToUnixTimeMilliseconds();
    }
    
    private long WaitForNextMillisecond(long lastTimestamp)
    {
        var timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
    
    public async ValueTask DisposeAsync()
    {
        try
        {
            _cache.Dispose();
            _logger.LogInformation("IDGen 服务已释放");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放 IDGen 服务失败");
        }
        
        await Task.CompletedTask;
    }
}

// ULID 实现
public struct ULID : IEquatable<ULID>, IComparable<ULID>, IComparable
{
    private readonly byte[] _bytes;
    
    private ULID(byte[] bytes)
    {
        _bytes = bytes;
    }
    
    public static ULID NewULID()
    {
        var bytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        
        // 设置时间戳部分
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        bytes[0] = (byte)((timestamp >> 40) & 0xFF);
        bytes[1] = (byte)((timestamp >> 32) & 0xFF);
        bytes[2] = (byte)((timestamp >> 24) & 0xFF);
        bytes[3] = (byte)((timestamp >> 16) & 0xFF);
        bytes[4] = (byte)((timestamp >> 8) & 0xFF);
        bytes[5] = (byte)(timestamp & 0xFF);
        
        return new ULID(bytes);
    }
    
    public static ULID Parse(string s)
    {
        if (s.Length == 26)
        {
            return ParseBase32(s);
        }
        else if (s.Length == 32)
        {
            return ParseHex(s);
        }
        else
        {
            throw new FormatException("无效的 ULID 格式");
        }
    }
    
    private static ULID ParseBase32(string s)
    {
        var bytes = new byte[16];
        var base32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        
        for (int i = 0; i < 26; i++)
        {
            var c = char.ToUpper(s[i]);
            var index = base32Chars.IndexOf(c);
            if (index == -1)
            {
                throw new FormatException("无效的 Base32 字符");
            }
            
            int bitPosition = i * 5;
            int byteIndex = bitPosition / 8;
            int bitOffset = bitPosition % 8;
            
            bytes[byteIndex] |= (byte)(index << (8 - bitOffset - 5));
            if (bitOffset > 3)
            {
                bytes[byteIndex + 1] |= (byte)(index >> (bitOffset - 3));
            }
        }
        
        return new ULID(bytes);
    }
    
    private static ULID ParseHex(string s)
    {
        var bytes = new byte[16];
        for (int i = 0; i < 16; i++)
        {
            bytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
        }
        return new ULID(bytes);
    }
    
    public DateTime DateTime
    {
        get
        {
            var timestamp = Timestamp;
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
        }
    }
    
    public long Timestamp
    {
        get
        {
            return ((long)_bytes[0] << 40) |
                   ((long)_bytes[1] << 32) |
                   ((long)_bytes[2] << 24) |
                   ((long)_bytes[3] << 16) |
                   ((long)_bytes[4] << 8) |
                   _bytes[5];
        }
    }
    
    public ulong Random
    {
        get
        {
            return ((ulong)_bytes[6] << 56) |
                   ((ulong)_bytes[7] << 48) |
                   ((ulong)_bytes[8] << 40) |
                   ((ulong)_bytes[9] << 32) |
                   ((ulong)_bytes[10] << 24) |
                   ((ulong)_bytes[11] << 16) |
                   ((ulong)_bytes[12] << 8) |
                   _bytes[13];
        }
    }
    
    public override string ToString()
    {
        return ToBase32String();
    }
    
    public string ToBase32String()
    {
        var base32Chars = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";
        var result = new char[26];
        
        for (int i = 0; i < 26; i++)
        {
            int bitPosition = i * 5;
            int byteIndex = bitPosition / 8;
            int bitOffset = bitPosition % 8;
            
            int value = (_bytes[byteIndex] >> (8 - bitOffset - 5)) & 0x1F;
            if (bitOffset > 3)
            {
                value |= (_bytes[byteIndex + 1] << (bitOffset - 3)) & 0x1F;
            }
            
            result[i] = base32Chars[value];
        }
        
        return new string(result);
    }
    
    public string ToHexString()
    {
        return BitConverter.ToString(_bytes).Replace("-", "").ToLower();
    }
    
    public bool Equals(ULID other)
    {
        for (int i = 0; i < 16; i++)
        {
            if (_bytes[i] != other._bytes[i])
            {
                return false;
            }
        }
        return true;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is ULID ulid)
        {
            return Equals(ulid);
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        int hash = 17;
        for (int i = 0; i < 16; i++)
        {
            hash = hash * 31 + _bytes[i];
        }
        return hash;
    }
    
    public int CompareTo(ULID other)
    {
        for (int i = 0; i < 16; i++)
        {
            if (_bytes[i] < other._bytes[i])
            {
                return -1;
            }
            if (_bytes[i] > other._bytes[i])
            {
                return 1;
            }
        }
        return 0;
    }
    
    public int CompareTo(object obj)
    {
        if (obj is ULID ulid)
        {
            return CompareTo(ulid);
        }
        throw new ArgumentException("对象必须是 ULID 类型");
    }
    
    public static bool operator ==(ULID left, ULID right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(ULID left, ULID right)
    {
        return !left.Equals(right);
    }
    
    public static bool operator <(ULID left, ULID right)
    {
        return left.CompareTo(right) < 0;
    }
    
    public static bool operator <=(ULID left, ULID right)
    {
        return left.CompareTo(right) <= 0;
    }
    
    public static bool operator >(ULID left, ULID right)
    {
        return left.CompareTo(right) > 0;
    }
    
    public static bool operator >=(ULID left, ULID right)
    {
        return left.CompareTo(right) >= 0;
    }
}