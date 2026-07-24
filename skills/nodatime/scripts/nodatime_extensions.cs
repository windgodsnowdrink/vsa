#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package NodaTime@3.1.9
#:package NodaTime.Serialization.SystemTextJson@1.0.0
#:package NodaTime.Serialization.JsonNet@3.1.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Channels@10.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true

using System;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Buffers;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Serialization.SystemTextJson;

// 配置选项
public class NodaTimeOptions
{
    public string DefaultTimeZone { get; set; } = "UTC";
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDetailedLogging { get; set; } = false;
}

// 时间服务接口
public interface INodaTimeService
{
    Instant GetCurrentInstant();
    ZonedDateTime GetCurrentDateTime();
    Task<Instant> GetCurrentInstantAsync();
    Task<ZonedDateTime> GetCurrentDateTimeAsync();
    ZonedDateTime ConvertToLocalTime(Instant instant, string timeZoneId);
    Task<ZonedDateTime> ConvertToLocalTimeAsync(Instant instant, string timeZoneId);
    string FormatDateTime(ZonedDateTime dateTime, string format);
    LocalDateTime ParseDateTime(string dateTimeString, string format);
}

// 时间计算器接口
public interface INodaTimeCalculator
{
    Duration CalculateDuration(LocalDateTime start, LocalDateTime end);
    LocalDateTime AddDuration(LocalDateTime dateTime, Duration duration);
    LocalDateTime SubtractDuration(LocalDateTime dateTime, Duration duration);
    bool IsInRange(LocalDateTime dateTime, LocalDateTime start, LocalDateTime end);
}

// 时间请求类
public class TimeRequest
{
    public int Id { get; set; }
    public Instant Timestamp { get; set; }
    public string TimeZoneId { get; set; }
    public Func<ZonedDateTime, Task> Callback { get; set; }
}

// 时间上下文类
public class NodaTimeContext
{
    public Instant Instant { get; set; }
    public ZonedDateTime ZonedDateTime { get; set; }
    public string TimeZoneId { get; set; }
    public StringBuilder StringBuilder { get; set; } = new StringBuilder();

    public void Reset()
    {
        Instant = Instant.MinValue;
        ZonedDateTime = ZonedDateTime.MinValue;
        TimeZoneId = string.Empty;
        StringBuilder.Clear();
    }
}

// 时间上下文池策略
public class NodaTimeContextPooledPolicy : IPooledObjectPolicy<NodaTimeContext>
{
    public NodaTimeContext Create()
    {
        return new NodaTimeContext();
    }

    public bool Return(NodaTimeContext obj)
    {
        obj.Reset();
        return true;
    }
}

// 尾延迟优化器
public class TailLatencyOptimizer
{
    private readonly int _windowSize = 100;
    private readonly double[] _latencies;
    private int _index = 0;
    private readonly object _lock = new object();

    public TailLatencyOptimizer()
    {
        _latencies = new double[_windowSize];
    }

    public void RecordLatency(double latencyMs)
    {
        lock (_lock)
        {
            _latencies[_index] = latencyMs;
            _index = (_index + 1) % _windowSize;
        }
    }

    public double GetP95Latency()
    {
        lock (_lock)
        {
            var sorted = new double[_windowSize];
            Array.Copy(_latencies, sorted, _windowSize);
            Array.Sort(sorted);
            return sorted[(int)(_windowSize * 0.95)];
        }
    }
}

// 时间服务实现
public class NodaTimeService : INodaTimeService
{
    private readonly IClock _clock;
    private readonly ILogger<NodaTimeService> _logger;
    private readonly NodaTimeOptions _options;
    private readonly Channel<TimeRequest> _requestChannel;
    private readonly ObjectPool<NodaTimeContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly DateTimeZoneCache _timeZoneCache;

    public NodaTimeService(
        IClock clock,
        ILogger<NodaTimeService> logger,
        IOptions<NodaTimeOptions> options)
    {
        _clock = clock ?? SystemClock.Instance;
        _logger = logger;
        _options = options.Value;
        _latencyOptimizer = new TailLatencyOptimizer();
        _timeZoneCache = new DateTimeZoneCache(_options.CacheSize);

        // 配置通道
        _requestChannel = Channel.CreateBounded<TimeRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<NodaTimeContext>(
            new NodaTimeContextPooledPolicy(),
            Environment.ProcessorCount * 2);

        // 启动处理循环
        _ = ProcessRequestsAsync();
    }

    // 处理请求的异步循环
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync())
        {
            try
            {
                var context = _contextPool.Get();
                try
                {
                    var startTime = DateTime.UtcNow;

                    // 转换时间
                    var timeZone = _timeZoneCache.GetTimeZone(request.TimeZoneId);
                    var zonedDateTime = request.Timestamp.InZone(timeZone);

                    // 执行回调
                    await request.Callback(zonedDateTime);

                    // 记录延迟
                    var latency = (DateTime.UtcNow - startTime).TotalMilliseconds;
                    _latencyOptimizer.RecordLatency(latency);

                    if (_options.EnableDetailedLogging)
                    {
                        _logger.LogDebug("Time request processed in {Latency}ms", latency);
                    }
                }
                finally
                {
                    _contextPool.Return(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing time request");
            }
        }
    }

    // 获取当前时间戳
    public Instant GetCurrentInstant()
    {
        return _clock.GetCurrentInstant();
    }

    // 获取当前日期时间
    public ZonedDateTime GetCurrentDateTime()
    {
        var instant = _clock.GetCurrentInstant();
        var timeZone = _timeZoneCache.GetTimeZone(_options.DefaultTimeZone);
        return instant.InZone(timeZone);
    }

    // 异步获取当前时间戳
    public async Task<Instant> GetCurrentInstantAsync()
    {
        // 模拟异步操作
        await Task.Yield();
        return _clock.GetCurrentInstant();
    }

    // 异步获取当前日期时间
    public async Task<ZonedDateTime> GetCurrentDateTimeAsync()
    {
        // 模拟异步操作
        await Task.Yield();
        var instant = _clock.GetCurrentInstant();
        var timeZone = _timeZoneCache.GetTimeZone(_options.DefaultTimeZone);
        return instant.InZone(timeZone);
    }

    // 转换为本地时间
    public ZonedDateTime ConvertToLocalTime(Instant instant, string timeZoneId)
    {
        var timeZone = _timeZoneCache.GetTimeZone(timeZoneId);
        return instant.InZone(timeZone);
    }

    // 异步转换为本地时间
    public async Task<ZonedDateTime> ConvertToLocalTimeAsync(Instant instant, string timeZoneId)
    {
        // 使用通道处理
        var completionSource = new TaskCompletionSource<ZonedDateTime>();

        var request = new TimeRequest
        {
            Id = Interlocked.Increment(ref _requestId),
            Timestamp = instant,
            TimeZoneId = timeZoneId,
            Callback = async (zonedDateTime) =>
            {
                completionSource.SetResult(zonedDateTime);
                await Task.CompletedTask;
            }
        };

        await _requestChannel.Writer.WriteAsync(request);
        return await completionSource.Task;
    }

    // 格式化日期时间
    public string FormatDateTime(ZonedDateTime dateTime, string format)
    {
        // 使用内存池优化字符串构建
        using var bufferWriter = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(bufferWriter);

        writer.WriteStartObject();
        writer.WriteString("formatted", dateTime.ToString(format));
        writer.WriteEndObject();
        writer.Flush();

        return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
    }

    // 解析日期时间
    public LocalDateTime ParseDateTime(string dateTimeString, string format)
    {
        return LocalDateTime.Parse(dateTimeString);
    }

    private static int _requestId = 0;
}

// 时间计算器实现
public class NodaTimeCalculator : INodaTimeCalculator
{
    private readonly ILogger<NodaTimeCalculator> _logger;
    private readonly NodaTimeOptions _options;

    public NodaTimeCalculator(
        ILogger<NodaTimeCalculator> logger,
        IOptions<NodaTimeOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    // 计算时间差异
    public Duration CalculateDuration(LocalDateTime start, LocalDateTime end)
    {
        if (_options.EnableDetailedLogging)
        {
            _logger.LogDebug("Calculating duration from {Start} to {End}", start, end);
        }

        return end - start;
    }

    // 添加时间
    public LocalDateTime AddDuration(LocalDateTime dateTime, Duration duration)
    {
        if (_options.EnableDetailedLogging)
        {
            _logger.LogDebug("Adding duration {Duration} to {DateTime}", duration, dateTime);
        }

        return dateTime + duration;
    }

    // 减去时间
    public LocalDateTime SubtractDuration(LocalDateTime dateTime, Duration duration)
    {
        if (_options.EnableDetailedLogging)
        {
            _logger.LogDebug("Subtracting duration {Duration} from {DateTime}", duration, dateTime);
        }

        return dateTime - duration;
    }

    // 检查时间是否在范围内
    public bool IsInRange(LocalDateTime dateTime, LocalDateTime start, LocalDateTime end)
    {
        return dateTime >= start && dateTime <= end;
    }
}

// 时区缓存
public class DateTimeZoneCache
{
    private readonly int _cacheSize;
    private readonly object _lock = new object();
    private readonly (string Id, DateTimeZone Zone)[] _cache;
    private int _count = 0;

    public DateTimeZoneCache(int cacheSize)
    {
        _cacheSize = Math.Max(1, cacheSize);
        _cache = new (string, DateTimeZone)[_cacheSize];
    }

    public DateTimeZone GetTimeZone(string timeZoneId)
    {
        // 尝试从缓存获取
        for (int i = 0; i < _count; i++)
        {
            if (_cache[i].Id == timeZoneId)
            {
                return _cache[i].Zone;
            }
        }

        // 缓存未命中，从提供者获取
        var timeZone = DateTimeZoneProviders.Tzdb[timeZoneId];

        // 添加到缓存
        lock (_lock)
        {
            if (_count < _cacheSize)
            {
                _cache[_count++] = (timeZoneId, timeZone);
            }
            else
            {
                // 缓存已满，替换第一个
                Array.Copy(_cache, 1, _cache, 0, _count - 1);
                _cache[_count - 1] = (timeZoneId, timeZone);
            }
        }

        return timeZone;
    }
}

// 依赖注入扩展
public static class NodaTimeServiceCollectionExtensions
{
    public static IServiceCollection AddNodaTimeServices(this IServiceCollection services, Action<NodaTimeOptions> configureOptions = null)
    {
        // 配置选项
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<NodaTimeOptions>(options => { });
        }

        // 注册服务
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddSingleton<INodaTimeService, NodaTimeService>();
        services.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();

        return services;
    }

    // 配置 JSON 序列化
    public static IServiceCollection ConfigureNodaTimeJsonSerialization(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
        {
            options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        });

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("NodaTime 技能示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();

        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // 配置 NodaTime
        services.AddNodaTimeServices(options =>
        {
            options.DefaultTimeZone = "Asia/Shanghai";
            options.EnableCache = true;
            options.CacheSize = 500;
            options.EnableDetailedLogging = false;
        });

        // 配置 JSON 序列化
        services.ConfigureNodaTimeJsonSerialization();

        // 构建服务提供者
        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        var nodaTimeCalculator = serviceProvider.GetRequiredService<INodaTimeCalculator>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 示例1: 获取当前时间
            Console.WriteLine("示例1: 获取当前时间");
            var now = nodaTimeService.GetCurrentDateTime();
            Console.WriteLine($"当前时间: {now}");
            Console.WriteLine($"当前时间戳: {nodaTimeService.GetCurrentInstant()}");

            // 示例2: 异步获取当前时间
            Console.WriteLine("\n示例2: 异步获取当前时间");
            var nowAsync = await nodaTimeService.GetCurrentDateTimeAsync();
            Console.WriteLine($"异步获取当前时间: {nowAsync}");

            // 示例3: 时间计算
            Console.WriteLine("\n示例3: 时间计算");
            var start = new LocalDateTime(2024, 1, 1, 0, 0, 0);
            var end = new LocalDateTime(2024, 1, 2, 12, 0, 0);
            var duration = nodaTimeCalculator.CalculateDuration(start, end);
            Console.WriteLine($"开始时间: {start}");
            Console.WriteLine($"结束时间: {end}");
            Console.WriteLine($"时间差异: {duration}");

            // 示例4: 时区转换
            Console.WriteLine("\n示例4: 时区转换");
            var utcTime = Instant.FromUtc(2024, 1, 1, 0, 0);
            var shanghaiTime = nodaTimeService.ConvertToLocalTime(utcTime, "Asia/Shanghai");
            var newYorkTime = nodaTimeService.ConvertToLocalTime(utcTime, "America/New_York");
            Console.WriteLine($"UTC 时间: {utcTime}");
            Console.WriteLine($"上海时间: {shanghaiTime}");
            Console.WriteLine($"纽约时间: {newYorkTime}");

            // 示例5: 时间格式化
            Console.WriteLine("\n示例5: 时间格式化");
            var formatted = nodaTimeService.FormatDateTime(now, "yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"格式化时间: {formatted}");

            // 示例6: 时间解析
            Console.WriteLine("\n示例6: 时间解析");
            var parsed = nodaTimeService.ParseDateTime("2024-01-01 12:00:00", "yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"解析时间: {parsed}");

            // 示例7: 时间范围检查
            Console.WriteLine("\n示例7: 时间范围检查");
            var testTime = new LocalDateTime(2024, 1, 1, 12, 0, 0);
            var isInRange = nodaTimeCalculator.IsInRange(testTime, start, end);
            Console.WriteLine($"时间 {testTime} 是否在范围内: {isInRange}");

            // 示例8: 性能测试
            Console.WriteLine("\n示例8: 性能测试");
            const int iterations = 10000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                nodaTimeService.GetCurrentInstant();
            }

            stopwatch.Stop();
            Console.WriteLine($"执行 {iterations} 次获取时间戳: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");

            Console.WriteLine("\n所有示例执行完成！");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "执行示例时发生错误");
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}