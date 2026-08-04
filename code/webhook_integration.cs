#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Http.Polly@8.0.0
#:package System.Threading.Channels@7.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using System.Diagnostics.Metrics;

public sealed class WebhookOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
    public int CircuitBreakerThreshold { get; set; } = 5;
    public string? SigningKey { get; set; }
    public TimeSpan SignatureExpiry { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableMetrics { get; set; } = true;
}

public interface IWebhookDispatcher
{
    ValueTask DispatchAsync(WebhookNotification notification, CancellationToken ct = default);
}

public sealed class WebhookNotification
{
    public required Uri Endpoint { get; init; }
    public required object Payload { get; init; }
    public int Priority { get; init; }
    public string? Signature { get; init; }
    public DateTimeOffset? Timestamp { get; init; }
    public string? Nonce { get; init; }
}

public sealed class WebhookMetrics
{
    private readonly Meter _meter;
    private readonly Counter<int> _deliveryCounter;
    private readonly Histogram<double> _deliveryLatency;
    
    public WebhookMetrics(IMeterFactory meterFactory)
    {
        _meter = meterFactory.Create("Webhook");
        _deliveryCounter = _meter.CreateCounter<int>("webhook.delivery.count", "count", "Total webhook delivery attempts");
        _deliveryLatency = _meter.CreateHistogram<double>("webhook.delivery.latency", "milliseconds", "Delivery latency distribution");
    }
    
    public void RecordDelivery(bool success, double latencyMs)
    {
        _deliveryCounter.Add(1, new("success", success));
        _deliveryLatency.Record(latencyMs);
    }
}

public sealed class WebhookDispatcher : IWebhookDispatcher, IAsyncDisposable
{
    private readonly Channel<WebhookNotification> _channel;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly IOptions<WebhookOptions> _options;
    private readonly ILogger<WebhookDispatcher> _logger;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker;
    private readonly WebhookMetrics _metrics;
    private readonly IOptions<WebhookOptions> _options;
    
    public WebhookDispatcher(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IOptions<WebhookOptions> options,
        ILogger<WebhookDispatcher> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _options = options;
        _logger = logger;
        
        _channel = Channel.CreateUnbounded<WebhookNotification>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
        
        _circuitBreaker = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                _options.Value.CircuitBreakerThreshold,
                _options.Value.CircuitBreakerDuration,
                (ex, state) => _logger.LogWarning("Circuit breaker opened: {State}", state),
                () => _logger.LogInformation("Circuit breaker reset"));
    }
    
    public ValueTask DispatchAsync(WebhookNotification notification, CancellationToken ct = default)
    {
        return _channel.Writer.WriteAsync(notification, ct);
    }
    
    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await Task.WhenAll(Enumerable.Range(0, Environment.ProcessorCount)
            .Select(_ => ProcessNotificationsAsync(default)));
    }
    
    private async Task ProcessNotificationsAsync(CancellationToken ct)
    {
        await foreach (var notification in _channel.Reader.ReadAllAsync(ct))
        {
            try
            {
                using var buffer = MemoryPool<byte>.Shared.Rent(4096);
                var client = _httpClientFactory.CreateClient();
                
                var response = await _circuitBreaker.ExecuteAsync(async () =>
                {
                    using var content = new ByteArrayContent(buffer.Memory.ToArray());
                    return await client.PostAsync(notification.Endpoint, content, ct);
                });
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Webhook delivery failed: {StatusCode}", response.StatusCode);
                }
                
                if (_options.Value.EnableMetrics)
                {
                    _metrics.RecordDelivery(response.IsSuccessStatusCode, stopwatch.Elapsed.TotalMilliseconds);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Webhook processing failed");
            }
        }
    }
}

public static class WebhookExtensions
{
    public static IServiceCollection AddWebhookDispatcher(this IServiceCollection services, Action<WebhookOptions>? configure = null)
    {
        services.AddOptions<WebhookOptions>()
            .Configure(configure ?? (opt => { }))
            .ValidateDataAnnotations();
            
        services.AddHttpClient();
        services.AddMemoryCache();
        
        services.AddSingleton<IWebhookDispatcher, WebhookDispatcher>();
        services.AddHostedService<WebhookBackgroundService>();
        
        services.AddMetrics();
        services.AddSingleton<WebhookMetrics>();
        
        return services;
    }
}

public sealed class WebhookBackgroundService : BackgroundService
{
    private readonly IWebhookDispatcher _dispatcher;
    
    public WebhookBackgroundService(IWebhookDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_dispatcher is WebhookDispatcher dispatcher)
        {
            await dispatcher.ProcessNotificationsAsync(stoppingToken);
        }
    }
}