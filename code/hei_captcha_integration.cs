#:sdk Microsoft.NET.Sdk.Web
#:package Hei.Captcha@1.0.0
#:package Microsoft.Extensions.Caching.Memory@7.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Drawing;
using System.Threading.Channels;
using Hei.Captcha;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CaptchaDemo
{
    public enum CaptchaType
    {
        Text,
        Math,
        Image,
        Slider,
        Puzzle,
        Behavior,
        Audio,
        Biometric,
        Custom
    }

    public class CaptchaOptions
    {
        public int Width { get; set; } = 120;
        public int Height { get; set; } = 40;
        public TimeSpan Expiry { get; set; } = TimeSpan.FromMinutes(5);
        public int MaxAttempts { get; set; } = 3;
        public CaptchaType Type { get; set; } = CaptchaType.Text;
        public int NoiseLevel { get; set; } = 2;
        public bool UseDistributedCache { get; set; } = false;
        public string? RedisConnectionString { get; set; }
        public bool EnableAotCompilation { get; set; } = false;
        public bool EnableDistributedTracing { get; set; } = false;
        public string? TracingEndpoint { get; set; }
        public CaptchaComplexity Complexity { get; set; } = CaptchaComplexity.Medium;
    }

    public enum CaptchaComplexity
    {
        Low,
        Medium,
        High,
        Extreme
    }

    public interface ICaptchaService
    {
        Task<string> GenerateCaptchaAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> ValidateCaptchaAsync(string key, string value, CancellationToken cancellationToken = default);
        Task<CaptchaHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);
        Task ResetMetricsAsync(CancellationToken cancellationToken = default);
        Task<CaptchaMetrics> GetDetailedMetricsAsync(CancellationToken cancellationToken = default);
        Task<string> GenerateCustomCaptchaAsync(string pattern, CancellationToken cancellationToken = default);
    }

    public class CaptchaMetrics
    {
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageGenerationTimeMs { get; set; }
        public double AverageValidationTimeMs { get; set; }
        public Dictionary<CaptchaType, int> TypeDistribution { get; set; } = new();
    }

    public class CaptchaHealthStatus
    {
        public int TotalRequests { get; set; }
        public int FailedRequests { get; set; }
        public double SuccessRate => TotalRequests == 0 ? 0 : (TotalRequests - FailedRequests) / (double)TotalRequests;
    }

    public class CaptchaService : ICaptchaService, IDisposable
    {
        private readonly IMemoryCache _cache;
        private readonly Channel<string> _requestChannel;
        private readonly ObjectPool<Bitmap> _bitmapPool;
        private readonly CaptchaOptions _options;
        private readonly FreeCaptcha _freeCaptcha;
        private readonly SlideCaptcha _slideCaptcha;
        private readonly MemoryCaptcha _memoryCaptcha;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CaptchaService> _logger;
        private readonly CancellationTokenSource _cts = new();
        private long _totalRequests;
        private long _failedRequests;

        public CaptchaService(
            IMemoryCache cache,
            IOptions<CaptchaOptions> options,
            FreeCaptcha freeCaptcha,
            SlideCaptcha slideCaptcha,
            MemoryCaptcha memoryCaptcha,
            IServiceScopeFactory scopeFactory,
            ILogger<CaptchaService> logger)
        {
            _cache = cache;
            _options = options.Value;
            _freeCaptcha = freeCaptcha;
            _slideCaptcha = slideCaptcha;
            _memoryCaptcha = memoryCaptcha;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _requestChannel = Channel.CreateBounded<string>(1000);
            _bitmapPool = new ObjectPool<Bitmap>(() => new Bitmap(_options.Width, _options.Height));

            _ = Task.Run(() => ProcessRequestsAsync(_cts.Token));
        }

        public async Task<string> GenerateCaptchaAsync(string key, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _totalRequests);
            try
            {
                var captchaValue = await GenerateCaptchaValueAsync();
                _cache.Set(key, captchaValue, _options.Expiry);
                return captchaValue;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                _logger.LogError(ex, "Failed to generate captcha");
                throw;
            }
        }

        public async Task<bool> ValidateCaptchaAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _totalRequests);
            try
            {
                if (_cache.TryGetValue(key, out string? storedValue) && storedValue == value)
                {
                    _cache.Remove(key);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failedRequests);
                _logger.LogError(ex, "Failed to validate captcha");
                throw;
            }
        }

        public Task<CaptchaHealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new CaptchaHealthStatus
            {
                TotalRequests = (int)Interlocked.Read(ref _totalRequests),
                FailedRequests = (int)Interlocked.Read(ref _failedRequests)
            });
        }

        public Task ResetMetricsAsync(CancellationToken cancellationToken = default)
        {
            Interlocked.Exchange(ref _totalRequests, 0);
            Interlocked.Exchange(ref _failedRequests, 0);
            return Task.CompletedTask;
        }

        private async Task ProcessRequestsAsync(CancellationToken cancellationToken)
        {
            await foreach (var request in _requestChannel.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    await GenerateCaptchaAsync(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process captcha request");
                }
            }
        }

        private async Task<string> GenerateCaptchaValueAsync()
        {
            return _options.Type switch
            {
                CaptchaType.Text => _freeCaptcha.GenerateNumberCaptcha(4),
                CaptchaType.Math => _freeCaptcha.GenerateMathCaptcha(),
                CaptchaType.Image => await _memoryCaptcha.GenerateAsync(),
                CaptchaType.Slider => _slideCaptcha.Generate(),
                _ => throw new NotSupportedException($"Captcha type {_options.Type} is not supported")
            };
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHeiCaptcha(this IServiceCollection services, Action<CaptchaOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<FreeCaptcha>();
            services.AddSingleton<SlideCaptcha>();
            services.AddSingleton<MemoryCaptcha>();
            services.AddSingleton<ICaptchaService, CaptchaService>();
            services.AddMemoryCache();
            
            services.AddOptions<CaptchaOptions>()
                .PostConfigure<IServiceProvider>((options, sp) =>
                {
                    if (options.EnableAotCompilation)
                    {
                        NativeLibrary.SetDllImportResolver(typeof(CaptchaService).Assembly, (name, assembly, path) =>
                        {
                            return IntPtr.Zero;
                        });
                    }
                    
                    if (options.EnableDistributedTracing && !string.IsNullOrEmpty(options.TracingEndpoint))
                    {
                        services.AddOpenTelemetry()
                            .WithTracing(builder => 
                            {
                                builder.AddSource("Hei.Captcha")
                                    .AddOtlpExporter(o => 
                                    {
                                        o.Endpoint = new Uri(options.TracingEndpoint);
                                    });
                            });
                    }
                });
                
            return services;
        }
    }
}