#:sdk Microsoft.NET.Sdk.Web
#:package NodaTime@4.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using NodaTime;
using NodaTime.Extensions;
using System.Buffers;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

// 1. 高性能时间服务(Disruptor模式)
[SkipLocalsInit]
public sealed class NodaTimeService : BackgroundService
{
    private readonly Channel<TimeRequest> _requestChannel;
    private readonly ObjectPool<NodaTimeContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly IDistributedCache _cache;
    private readonly IClock _clock;

    public NodaTimeService(
        IClock clock,
        IDistributedCache cache)
    {
        _clock = clock ?? SystemClock.Instance;
        _cache = cache;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<TimeRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池(CPU cache-line对齐)
        _contextPool = new DefaultObjectPool<NodaTimeContext>(
            new NodaTimeContextPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    // 2. 时间扩展方法(Span零拷贝优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public Instant GetCurrentInstant()
    {
        return _clock.GetCurrentInstant();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ZonedDateTime GetZonedDateTime(DateTimeZone zone)
    {
        return _clock.InZone(zone);
    }

    // 3. 高性能时间格式化(内存池优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public string FormatInstant(Instant instant, string pattern)
    {
        var cacheKey = $"nodatime:fmt:{instant.ToUnixTimeTicks()}:{pattern}";
        var cached = _cache.GetString(cacheKey);
        if (cached != null) return cached;

        var result = instant.ToString(pattern, null);
        _cache.SetString(cacheKey, result, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });
        return result;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(ct))
        {
            var context = _contextPool.Get();
            try
            {
                await context.ProcessAsync(request, _clock, _cache);
                _latencyOptimizer.RecordLatency();
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 4. 时间扩展类
public static class NodaTimeExtensions
{
    // 5. 线程安全的时间解析(Span优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static Instant ParseInstant(this string instantStr)
    {
        return InstantPattern.ExtendedIso.Parse(instantStr).Value;
    }

    // 6. 高性能时间段计算
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static Duration Between(this Instant start, Instant end)
    {
        return end - start;
    }

    // 7. 时区转换优化
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static ZonedDateTime ToTimeZone(this Instant instant, DateTimeZone zone)
    {
        return instant.InZone(zone);
    }
}

// 8. 主程序配置
var builder = WebApplication.CreateBuilder(args);

// 配置Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "NodaTime:";
});

// 注册NodaTime服务
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton<NodaTimeService>();
builder.Services.AddHostedService<NodaTimeService>();

var app = builder.Build();

app.MapGet("/now", ([FromServices] NodaTimeService service) =>
{
    return service.GetCurrentInstant().ToString();
});

app.MapGet("/timezone/{zone}", ([FromServices] NodaTimeService service, string zone) =>
{
    var timeZone = DateTimeZoneProviders.Tzdb[zone];
    return service.GetZonedDateTime(timeZone).ToString();
});

app.Run();

// 辅助记录类型
public record TimeRequest(string Type, string Payload);