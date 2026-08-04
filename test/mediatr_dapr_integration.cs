#:sdk Microsoft.NET.Sdk.Web
#:package Dapr.AspNetCore@1.12.0
#:package MediatR@12.1.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class DaprEventBusOptions
{
    [Required(ErrorMessage = "PubSubName is required")]
    [MinLength(3, ErrorMessage = "PubSubName must be at least 3 characters")]
    public string PubSubName { get; set; } = "pubsub";

    [Required(ErrorMessage = "StateStoreName is required")]
    [MinLength(3, ErrorMessage = "StateStoreName must be at least 3 characters")]
    public string StateStoreName { get; set; } = "statestore";

    [Range(1, 10, ErrorMessage = "RetryCount must be between 1 and 10")]
    public int RetryCount { get; set; } = 3;

    [Range(100, 5000, ErrorMessage = "Timeout must be between 100 and 5000 ms")]
    public int TimeoutMs { get; set; } = 1000;
}

public class DaprEventBus<T> : IDistributedEventBus<T> where T : INotification
{
    private readonly DaprClient _daprClient;
    private readonly IMediator _mediator;
    private readonly DaprEventBusOptions _options;
    private readonly ILogger<DaprEventBus<T>> _logger;

    public DaprEventBus(DaprClient daprClient, IMediator mediator, 
        IOptions<DaprEventBusOptions> options, ILogger<DaprEventBus<T>> logger)
    {
        _daprClient = daprClient;
        _mediator = mediator;
        _options = options.Value;
        _logger = logger;
    }

    public async Task Publish(T notification, CancellationToken cancellationToken = default)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(_options.RetryCount, 
                retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100));

        await policy.ExecuteAsync(async () => 
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(_options.TimeoutMs);

            var activity = Activity.Current?.AddTag("event.type", typeof(T).Name)
                .AddTag("dapr.pubsub", _options.PubSubName);

            await _daprClient.PublishEventAsync(_options.PubSubName, 
                typeof(T).Name.ToLowerInvariant(), notification, cts.Token);
        });
    }

    public async Task SaveStateAsync(string key, object value, CancellationToken ct = default)
    {
        await _daprClient.SaveStateAsync(_options.StateStoreName, key, value, cancellationToken: ct);
    }

    public async Task<TValue> GetStateAsync<TValue>(string key, CancellationToken ct = default)
    {
        return await _daprClient.GetStateAsync<TValue>(_options.StateStoreName, key, cancellationToken: ct);
    }

    // 在DaprEventBus中添加Brotli压缩
    public async Task Publish(T notification, CancellationToken ct = default)
    {
        using var memoryStream = new MemoryStream();
        await using (var compressionStream = new BrotliStream(memoryStream, CompressionLevel.Optimal))
        {
            await JsonSerializer.SerializeAsync(compressionStream, notification, cancellationToken: ct);
        }
        var compressedData = memoryStream.ToArray();
        await _daprClient.PublishEventAsync(_options.PubSubName, typeof(T).Name, compressedData, ct);
    }

    // 添加批量处理器
    public class BatchEventProcessor<T> : BackgroundService where T : INotification
    {
        private readonly Channel<T> _channel;
        private readonly IDistributedEventBus<T> _eventBus;
        
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            await foreach (var batch in _channel.Reader.ReadAllAsync(ct).Buffer(100, 1000))
            {
                await _eventBus.PublishBatch(batch, ct);
            }
        }
    }

    // 死信队列处理器
    public class DeadLetterProcessor : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var deadLetters = await _daprClient.GetDeadLetterEventsAsync(ct);
                foreach (var dl in deadLetters)
                {
                    // 重试或记录错误
                }
                await Task.Delay(5000, ct);
            }
        }
    }
}

public static class DaprEventBusExtensions
{
    public static IServiceCollection AddDaprEventBus<T>(this IServiceCollection services, 
    Action<DaprEventBusOptions> configure) where T : INotification
{
    services.AddDaprClient(builder => builder
        .UseJsonSerializationOptions(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        }));
    
    // 在AddDaprEventBus中注册新服务
    services.AddHostedService<BatchEventProcessor<T>>();
    services.AddHostedService<DeadLetterProcessor>();
    services.AddSingleton<Channel<T>>(_ => 
        Channel.CreateBounded<T>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait
        }));
        
    services.AddOptions<DaprEventBusOptions>()
        .Configure(configure)
        .ValidateDataAnnotations()
        .ValidateOnStart();

    services.AddSingleton<IDistributedEventBus<T>, DaprEventBus<T>>();
    services.AddHealthChecks()
        .AddCheck<DaprHealthCheck>("dapr")
        .AddPublisherCheck("dapr_pubsub", () => 
            new HealthCheckResult(HealthStatus.Healthy, "Dapr PubSub is healthy"));

    services.AddOpenTelemetry()
        .WithTracing(builder => builder
            .AddSource("DaprEventBus")
            .AddDaprInstrumentation());

    return services;
}
}

public class DaprHealthCheck : IHealthCheck
{
    private readonly DaprClient _daprClient;

    public DaprHealthCheck(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await _daprClient.GetMetadataAsync(ct);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}

// 订阅处理示例
[Topic("pubsub", "orderevent")]
public class OrderEventHandler : IRequestHandler<OrderEvent>
{
    private readonly DaprClient _daprClient;

    public OrderEventHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task Handle(OrderEvent request, CancellationToken ct)
    {
        // 使用Dapr状态管理
        await _daprClient.SaveStateAsync("statestore", request.OrderId, request, ct);
    }
}