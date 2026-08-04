#:sdk Microsoft.NET.Sdk.Web
#:package CaptchaSharp@2.1.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using CaptchaSharp;
using CaptchaSharp.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CaptchaSharpIntegration
{
    public enum CaptchaType
    {
        Text,
        Image,
        Audio,
        Puzzle,
        Behavior,
        Biometric,
        Math,
        Slider
    }

    public enum CaptchaComplexity
    {
        Low,
        Medium,
        High,
        Extreme
    }

    public class CaptchaOptions
    {
        public int Width { get; set; } = 200;
        public int Height { get; set; } = 100;
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(5);
        public int MaxAttempts { get; set; } = 3;
        public CaptchaType Type { get; set; } = CaptchaType.Text;
        public int NoiseLevel { get; set; } = 2;
        public bool UseDistributedCache { get; set; } = false;
        public bool EnablePerformanceMonitoring { get; set; } = true;
        public CaptchaComplexity Complexity { get; set; } = CaptchaComplexity.Medium;
        public bool EnableAnimation { get; set; } = false;
        public string RedisConnectionString { get; set; }
    }

    public interface ICaptchaService
    {
        Task<string> GenerateCaptchaAsync(CancellationToken cancellationToken = default);
        Task<bool> ValidateCaptchaAsync(string captchaId, string userInput, CancellationToken cancellationToken = default);
        Task ResetMetricsAsync(CancellationToken cancellationToken = default);
        Task<CaptchaHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);
        Task<CaptchaMetrics> GetMetricsAsync(CancellationToken cancellationToken = default);
        Task<bool> IsCaptchaValidAsync(string captchaId, CancellationToken cancellationToken = default);
    }

    public class CaptchaMetrics
    {
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public Dictionary<CaptchaType, int> TypeDistribution { get; set; }
        public Dictionary<CaptchaComplexity, int> ComplexityDistribution { get; set; }
    }

    public class CaptchaHealthStatus
    {
        public bool IsHealthy { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public int SuccessRate { get; set; }
    }

    public class CaptchaService : ICaptchaService
    {
        private readonly IMemoryCache _cache;
        private readonly IOptions<CaptchaOptions> _options;
        private readonly CaptchaServiceProvider _captchaProvider;
        private readonly Channel<string> _captchaChannel;
        private readonly ObjectPool<Bitmap> _bitmapPool;

        public CaptchaService(
            IMemoryCache cache,
            IOptions<CaptchaOptions> options,
            CaptchaServiceProvider captchaProvider)
        {
            _cache = cache;
            _options = options;
            _captchaProvider = captchaProvider;
            _captchaChannel = Channel.CreateUnbounded<string>();
            _bitmapPool = new ObjectPool<Bitmap>(() => new Bitmap(_options.Value.Width, _options.Value.Height));

            // Start background workers
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                Task.Run(GenerateCaptchaWorkerAsync);
            }
        }

        public async Task<string> GenerateCaptchaAsync(CancellationToken cancellationToken = default)
        {
            return await _captchaChannel.Reader.ReadAsync(cancellationToken);
        }

        public async Task<bool> ValidateCaptchaAsync(string captchaId, string userInput, CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(captchaId, out string correctValue))
            {
                _cache.Remove(captchaId);
                return string.Equals(correctValue, userInput, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public Task ResetMetricsAsync(CancellationToken cancellationToken = default)
        {
            // Implementation omitted for brevity
            return Task.CompletedTask;
        }

        public Task<CaptchaHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default)
        {
            // Implementation omitted for brevity
            return Task.FromResult(new CaptchaHealthStatus { IsHealthy = true });
        }

        private async Task GenerateCaptchaWorkerAsync()
        {
            while (true)
            {
                try
                {
                    using var bitmap = _bitmapPool.Get();
                    // Generate captcha using CaptchaSharp
                    var result = await _captchaProvider.GenerateAsync(new CaptchaGenerationOptions
                    {
                        Type = _options.Value.Type switch
                        {
                            CaptchaType.Text => CaptchaType.Text,
                            CaptchaType.Image => CaptchaType.Image,
                            _ => CaptchaType.Text
                        },
                        Difficulty = _options.Value.NoiseLevel
                    });

                    var captchaId = Guid.NewGuid().ToString();
                    _cache.Set(captchaId, result.CorrectAnswer, _options.Value.Expiration);
                    await _captchaChannel.Writer.WriteAsync(captchaId);
                }
                catch (Exception ex)
                {
                    // Log error
                }
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCaptchaSharpIntegration(this IServiceCollection services, Action<CaptchaOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<CaptchaServiceProvider>();
            services.AddSingleton<ICaptchaService, CaptchaService>();
            
            var options = new CaptchaOptions();
            configureOptions(options);
            
            if (options.UseDistributedCache && !string.IsNullOrEmpty(options.RedisConnectionString))
            {
                services.AddStackExchangeRedisCache(opt => 
                {
                    opt.Configuration = options.RedisConnectionString;
                    opt.InstanceName = "CaptchaSharp_";
                });
            }
            else
            {
                services.AddMemoryCache();
            }

            if (options.EnablePerformanceMonitoring)
            {
                services.AddHostedService<CaptchaMetricsService>();
                services.AddSingleton<CaptchaMetricsCollector>();
            }

            return services;
        }

        public static IServiceCollection AddCaptchaSharpIntegrationWithRedis(this IServiceCollection services, Action<CaptchaOptions> configureOptions, string redisConnectionString)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            return services.AddCaptchaSharpIntegration(configureOptions);
        }
    }

    public class CaptchaMetricsService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Implementation omitted for brevity
            return Task.CompletedTask;
        }
    }
}