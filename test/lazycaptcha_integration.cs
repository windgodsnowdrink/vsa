#:sdk Microsoft.NET.Sdk.Web
#:package LazyCaptcha@1.2.0
#:package Microsoft.Extensions.Caching.Memory@7.0.0
#:package Microsoft.Extensions.ObjectPool@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using LazyCaptcha;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;
using System.Threading.Channels;

public enum CaptchaMode
{
    Slide,
    Rotate,
    Puzzle,
    Click,
    Drag
}

public class CaptchaOptions
{
    public int Width { get; set; } = 300;
    public int Height { get; set; } = 150;
    public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxAttempts { get; set; } = 3;
    public bool UseDistributedCache { get; set; }
    public CaptchaMode Mode { get; set; } = CaptchaMode.Slide;
    public int NoiseLevel { get; set; } = 2;
    public int Complexity { get; set; } = 3;
    public bool EnableAnimation { get; set; }
    public bool EnablePerformanceMonitoring { get; set; } = true;
}

public interface ICaptchaService
{
    Task<CaptchaResult> GenerateCaptchaAsync(string sessionId);
    Task<bool> ValidateCaptchaAsync(string sessionId, string userInput);
    CaptchaMetrics GetMetrics();
    Task ResetMetricsAsync();
    Task<CaptchaHealthStatus> GetHealthStatusAsync();
}

public class CaptchaService : ICaptchaService, IDisposable
{
    private readonly CaptchaOptions _options;
    private readonly IMemoryCache _cache;
    private readonly ObjectPool<Bitmap> _bitmapPool;
    private readonly Channel<CaptchaRequest> _requestChannel;
    private readonly CancellationTokenSource _cts = new();
    private readonly CaptchaMetrics _metrics = new();

    public CaptchaService(CaptchaOptions options, IMemoryCache cache)
    {
        _options = options;
        _cache = cache;
        _bitmapPool = new DefaultObjectPool<Bitmap>(new BitmapPooledPolicy(), 10);
        _requestChannel = Channel.CreateBounded<CaptchaRequest>(100);
        StartProcessing();
    }

    private void StartProcessing()
    {
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            Task.Run(async () => await ProcessRequestsAsync(_cts.Token));
        }
    }

    private async Task ProcessRequestsAsync(CancellationToken ct)
    {
        while (await _requestChannel.Reader.WaitToReadAsync(ct))
        {
            if (_requestChannel.Reader.TryRead(out var request))
            {
                try
                {
                    using var bitmap = _bitmapPool.Get();
                    var result = GenerateCaptchaInternal(bitmap);
                    request.Tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    request.Tcs.SetException(ex);
                }
            }
        }
    }

    public async Task<CaptchaResult> GenerateCaptchaAsync(string sessionId)
    {
        var tcs = new TaskCompletionSource<CaptchaResult>();
        await _requestChannel.Writer.WriteAsync(new CaptchaRequest(tcs), _cts.Token);
        return await tcs.Task;
    }

    public async Task<bool> ValidateCaptchaAsync(string sessionId, string userInput)
    {
        // 验证逻辑实现
        return true;
    }

    public CaptchaMetrics GetMetrics() => _metrics;

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }

    private class CaptchaRequest
    {
        public TaskCompletionSource<CaptchaResult> Tcs { get; }
        public CaptchaRequest(TaskCompletionSource<CaptchaResult> tcs) => Tcs = tcs;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLazyCaptcha(this IServiceCollection services, Action<CaptchaOptions> configure)
    {
        var options = new CaptchaOptions();
        configure(options);
        
        services.Configure(configure);
        services.AddSingleton<ICaptchaService, CaptchaService>();
        
        if (options.UseDistributedCache)
        {
            services.AddStackExchangeRedisCache(redis => 
            {
                redis.Configuration = "localhost:6379";
                redis.InstanceName = "Captcha_";
            });
        }
        else
        {
            services.AddMemoryCache();
        }
        
        if (options.EnablePerformanceMonitoring)
        {
            services.AddHostedService<CaptchaMetricsService>();
        }
        
        return services;
    }
}