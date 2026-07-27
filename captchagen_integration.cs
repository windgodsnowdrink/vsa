#:sdk Microsoft.NET.Sdk.Web
#:package CaptchaGen.NetCore@2.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Caching;
using CaptchaGen;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.ObjectPool;

public enum CaptchaType
{
    TextOnly,
    ImageText,
    MathExpression,
    SlidingPuzzle
}

public record CaptchaOptions
{
    public int Width { get; set; } = 200;
    public int Height { get; set; } = 80;
    public int CodeLength { get; set; } = 6;
    public int NoiseLevel { get; set; } = 20;
    public int ExpirySeconds { get; set; } = 300;
    public CaptchaType Type { get; set; } = CaptchaType.ImageText;
    public bool UseDistributedCache { get; set; } = false;
    public bool EnablePerformanceMonitoring { get; set; } = true;
}

public interface ICaptchaService
{
    (byte[] ImageBytes, string Code) GenerateCaptcha();
    bool ValidateCaptcha(string code, string userInput);
    
    // 新增方法
    Task<(byte[] ImageBytes, string Code)> GenerateCaptchaAsync(CancellationToken ct = default);
    Task<bool> ValidateCaptchaAsync(string code, string userInput, CancellationToken ct = default);
    
    // 性能监控
    CaptchaPerformanceMetrics GetPerformanceMetrics();
}

public class CaptchaService : ICaptchaService
{
    private readonly IMemoryCache _cache;
    private readonly ObjectPool<Bitmap> _bitmapPool;
    private readonly CaptchaOptions _options;
    private readonly Random _random = new();

    public CaptchaService(
        IMemoryCache cache,
        ObjectPool<Bitmap> bitmapPool,
        IOptions<CaptchaOptions> options)
    {
        _cache = cache;
        _bitmapPool = bitmapPool;
        _options = options.Value;
    }

    public (byte[] ImageBytes, string Code) GenerateCaptcha()
    {
        var code = GenerateRandomCode();
        var bitmap = _bitmapPool.Get();
        
        try
        {
            using var ms = new RecyclableMemoryStream(ArrayPool<byte>.Shared);
            var imageBytes = ImageFactory.BuildImage(
                code, 
                _options.Width, 
                _options.Height, 
                _options.NoiseLevel).ToArray();
                
            _cache.Set(code, code, TimeSpan.FromSeconds(_options.ExpirySeconds));
            return (imageBytes, code);
        }
        finally
        {
            _bitmapPool.Return(bitmap);
        }
    }

    public bool ValidateCaptcha(string code, string userInput)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(userInput))
            return false;
            
        var cachedCode = _cache.Get<string>(code);
        _cache.Remove(code);
        return string.Equals(cachedCode, userInput, StringComparison.OrdinalIgnoreCase);
    }

    private string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Repeat(chars, _options.CodeLength)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}

public record CaptchaPerformanceMetrics
{
    public long TotalRequests { get; init; }
    public long FailedRequests { get; init; }
    public TimeSpan AverageGenerationTime { get; init; }
    public TimeSpan AverageValidationTime { get; init; }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaptchaService(this IServiceCollection services, Action<CaptchaOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        services.AddSingleton<ObjectPool<Bitmap>>(sp => 
        {
            var policy = new BitmapPooledPolicy(
                sp.GetRequiredService<IOptions<CaptchaOptions>>().Value.Width,
                sp.GetRequiredService<IOptions<CaptchaOptions>>().Value.Height,
                PixelFormat.Format32bppArgb);
                
            return new DefaultObjectPool<Bitmap>(policy, Environment.ProcessorCount * 2);
        });
        
        // 根据配置选择缓存类型
        services.AddOptions<CaptchaOptions>()
            .PostConfigure<IServiceProvider>((options, sp) => 
            {
                if (options.UseDistributedCache)
                {
                    services.AddStackExchangeRedisCache(opts => 
                    {
                        opts.Configuration = "localhost";
                        opts.InstanceName = "Captcha_";
                    });
                }
                else
                {
                    services.AddMemoryCache();
                }
            });
            
        services.AddSingleton<ICaptchaService, CaptchaService>();
        
        // 添加性能监控服务
        services.AddSingleton<CaptchaPerformanceMetrics>();
        
        return services;
    }
}

internal class BitmapPooledPolicy : PooledObjectPolicy<Bitmap>
{
    private readonly int _width;
    private readonly int _height;
    private readonly PixelFormat _pixelFormat;

    public BitmapPooledPolicy(int width, int height, PixelFormat pixelFormat)
    {
        _width = width;
        _height = height;
        _pixelFormat = pixelFormat;
    }

    public override Bitmap Create() => new(_width, _height, _pixelFormat);
    
    public override bool Return(Bitmap obj)
    {
        if (obj.Width != _width || obj.Height != _height || obj.PixelFormat != _pixelFormat)
            return false;
            
        using var g = Graphics.FromImage(obj);
        g.Clear(Color.White);
        return true;
    }
}