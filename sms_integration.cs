#:sdk Microsoft.NET.Sdk.Web
#:package Aliyun.Acs.Core@1.0.0
#:package Microsoft.Extensions.Options@7.0.0
#:package Microsoft.Extensions.Caching.Memory@7.0.0
#:package Polly@7.2.3
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

public class SmsOptions
{
    public string AccessKeyId { get; set; }
    public string AccessKeySecret { get; set; }
    public string SignName { get; set; }
    public string TemplateCode { get; set; }
    public int RetryCount { get; set; } = 3;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    public int CircuitBreakerThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableMetrics { get; set; } = true;
    public string EncryptionKey { get; set; }
    public int BatchSize { get; set; } = 100;
    public TimeSpan BatchInterval { get; set; } = TimeSpan.FromSeconds(5);
}

public interface ISmsSender
{
    Task<bool> SendAsync(string phoneNumber, string templateParamJson, CancellationToken cancellationToken = default);
}

public class SmsSender : ISmsSender
{
    private readonly Channel<(string, string)> _channel;
    private readonly IOptions<SmsOptions> _options;
    private readonly IMemoryCache _cache;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly SmsMetrics? _metrics;
    private readonly Timer _batchTimer;
    private readonly List<(string, string)> _batchBuffer;
    
    public SmsSender(IOptions<SmsOptions> options, IMemoryCache cache, SmsMetrics? metrics = null)
    {
        _metrics = metrics;
        _batchBuffer = new List<(string, string)>(options.Value.BatchSize);
        _batchTimer = new Timer(ProcessBatch, null, Timeout.Infinite, Timeout.Infinite);

    public SmsSender(IOptions<SmsOptions> options, IMemoryCache cache)
    {
        _options = options;
        _cache = cache;
        _channel = Channel.CreateUnbounded<(string, string)>();

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(options.Value.RetryCount, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: options.Value.CircuitBreakerThreshold,
                durationOfBreak: options.Value.CircuitBreakerDuration);

        StartProcessing();
    }

    private void StartProcessing()
    {
        Task.Run(async () =>
        {
            await foreach (var (phoneNumber, templateParamJson) in _channel.Reader.ReadAllAsync())
            {
                try
                {
                    var cacheKey = $"sms_{phoneNumber}_{templateParamJson}";
                    if (_cache.TryGetValue(cacheKey, out _))
                        continue;

                    await _circuitBreakerPolicy.ExecuteAsync(() => 
                        _retryPolicy.ExecuteAsync(async () => 
                        {
                            var profile = DefaultProfile.GetProfile("cn-hangzhou", 
                                _options.Value.AccessKeyId, 
                                _options.Value.AccessKeySecret);
                            var client = new DefaultAcsClient(profile);

                            var request = new SendSmsRequest
                            {
                                PhoneNumbers = phoneNumber,
                                SignName = _options.Value.SignName,
                                TemplateCode = _options.Value.TemplateCode,
                                TemplateParam = templateParamJson
                            };

                            var response = await client.GetAcsResponseAsync(request);
                            _cache.Set(cacheKey, true, TimeSpan.FromMinutes(5));
                        }));
                }
                catch (Exception ex)
                {
                    // Log error
                }
            }
        });
    }

    public async Task<bool> SendAsync(string phoneNumber, string templateParamJson, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync((phoneNumber, templateParamJson), cancellationToken);
        return true;
    }
}

public static class SmsExtensions
{
    public static IServiceCollection AddSmsService(this IServiceCollection services, Action<SmsOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddMemoryCache();
        services.AddSingleton<ISmsSender, SmsSender>();
        
        var options = new SmsOptions();
        configureOptions(options);
        
        if (options.EnableMetrics)
        {
            services.AddSingleton<SmsMetrics>();
        }
        
        return services;
    }
}