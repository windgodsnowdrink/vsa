#:sdk Microsoft.NET.Sdk.Web
#:package Captcha-Recognizer@2.0.0
#:package Microsoft.Extensions.Caching.Memory@7.0.0
#:package Microsoft.Extensions.ObjectPool@7.0.0
#:package System.Threading.Channels@7.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Drawing;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace CaptchaRecognizerIntegration
{
    public enum CaptchaType
    {
        Text,
        Math,
        Image,
        Slider,
        Puzzle,
        Behavior,
        Biometric,
        Audio
    }

    public class CaptchaOptions
    {
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 80;
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
        public int MaxAttempts { get; set; } = 3;
        public CaptchaType Type { get; set; } = CaptchaType.Text;
        public int NoiseLevel { get; set; } = 2;
        public bool UseDistributedCache { get; set; } = false;
        public bool EnablePerformanceMonitoring { get; set; } = true;
        public int ComplexityLevel { get; set; } = 3;
        public bool EnableAnimation { get; set; } = true;
    }

    public interface ICaptchaService
    {
        Task<string> GenerateCaptchaAsync(string sessionId);
        Task<bool> ValidateCaptchaAsync(string sessionId, string input);
        Task ResetMetricsAsync();
        Task<CaptchaHealthStatus> GetHealthStatusAsync();
    }

    public class CaptchaHealthStatus
    {
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageProcessingTimeMs { get; set; }
        public DateTime LastResetTime { get; set; }
    }

    public class CaptchaService : ICaptchaService, IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly ObjectPool<Bitmap> _bitmapPool;
        private readonly Channel<string> _processingChannel;
        private readonly CancellationTokenSource _cts;
        private readonly CaptchaOptions _options;

        public CaptchaService(
            IMemoryCache cache,
            IOptions<CaptchaOptions> options,
            ObjectPoolProvider poolProvider)
        {
            _cache = cache;
            _options = options.Value;
            _bitmapPool = poolProvider.Create(new DefaultPooledObjectPolicy<Bitmap>());
            _processingChannel = Channel.CreateUnbounded<string>();
            _cts = new CancellationTokenSource();

            // Start processing workers
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Run(() => ProcessCaptchaRequestsAsync(_cts.Token));
            }
        }

        public async Task<string> GenerateCaptchaAsync(string sessionId)
        {
            await _processingChannel.Writer.WriteAsync(sessionId);
            return await _cache.GetOrCreateAsync(sessionId, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _options.Expiration;
                return Task.FromResult(Guid.NewGuid().ToString("N").Substring(0, 6));
            });
        }

        public Task<bool> ValidateCaptchaAsync(string sessionId, string input)
        {
            if (!_cache.TryGetValue(sessionId, out string expectedValue))
                return Task.FromResult(false);

            _cache.Remove(sessionId);
            return Task.FromResult(string.Equals(input, expectedValue, StringComparison.OrdinalIgnoreCase));
        }

        private async Task ProcessCaptchaRequestsAsync(CancellationToken cancellationToken)
        {
            while (await _processingChannel.Reader.WaitToReadAsync(cancellationToken))
            {
                if (_processingChannel.Reader.TryRead(out var sessionId))
                {
                    try
                    {
                        using var bitmap = _bitmapPool.Get();
                        // Generate captcha using Captcha-Recognizer
                        // Implementation details would go here
                    }
                    catch
                    {
                        // Log error
                    }
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaptchaRecognizer(this IServiceCollection services, Action<CaptchaOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            var options = new CaptchaOptions();
            configureOptions(options);
            
            if (options.UseDistributedCache)
            {
                services.AddStackExchangeRedisCache(redisOptions =>
                {
                    redisOptions.Configuration = "localhost";
                    redisOptions.InstanceName = "CaptchaRecognizer_";
                });
            }
            else
            {
                services.AddMemoryCache();
            }
            
            services.AddSingleton<ICaptchaService, CaptchaService>();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            
            if (options.EnablePerformanceMonitoring)
            {
                services.AddHostedService<CaptchaMetricsService>();
            }
            
            return services;
        }
    }
    
    public class CaptchaMetricsService : BackgroundService
    {
        private readonly ICaptchaService _captchaService;
        private readonly ILogger<CaptchaMetricsService> _logger;
        
        public CaptchaMetricsService(ICaptchaService captchaService, ILogger<CaptchaMetricsService> logger)
        {
            _captchaService = captchaService;
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var healthStatus = await _captchaService.GetHealthStatusAsync();
                _logger.LogInformation("Captcha Service Metrics - Total: {Total}, Failed: {Failed}, AvgTime: {AvgTime}ms",
                    healthStatus.TotalRequests, healthStatus.FailedRequests, healthStatus.AverageProcessingTimeMs);
                
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}