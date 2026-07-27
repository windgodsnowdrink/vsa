#:sdk Microsoft.NET.Sdk.Web
#:package Dapr.AspNetCore@1.12.0
#:package Dapr.Client@1.12.0
#:property TargetFramework net11.0
#:property Nullable enable

using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

// Dapr配置选项
public class DaprOptions
{
    public string StateStoreName { get; set; } = "statestore";
    public string PubSubName { get; set; } = "pubsub";
    public string[] SupportedStateStores { get; set; } = new[] { "redis", "cosmosdb" };
    public string[] SupportedMessageBrokers { get; set; } = new[] { "rabbitmq", "kafka" };
    public int RetryCount { get; set; } = 3;
    public int CircuitBreakerThreshold { get; set; } = 5;
}

// Dapr服务接口
public interface IDaprService
{
    // 状态管理
    Task SaveStateAsync<T>(string storeName, string key, T value, string etag = null, StateOptions options = null);
    Task<T> GetStateAsync<T>(string storeName, string key, ConsistencyMode? consistencyMode = null);
    Task DeleteStateAsync(string storeName, string key);
    Task<bool> TrySaveStateAsync<T>(string storeName, string key, T value, string etag = null);
    
    // 发布订阅
    Task PublishAsync<T>(string pubsubName, string topic, T data);
    Task SubscribeAsync<T>(string pubsubName, string topic, Func<T, Task> handler);
    
    // 服务调用
    Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest request, HttpMethod httpMethod = null);
    
    // 绑定
    Task InvokeBindingAsync(string bindingName, string operation, object data, Dictionary<string, string> metadata = null);
    
    // 可观测性
    Task<IEnumerable<DaprHealthCheckResponse>> CheckHealthAsync();
}

// Dapr服务实现
public class DaprService : IDaprService
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<DaprService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    
    public DaprService(DaprClient daprClient, ILogger<DaprService> logger, IOptions<DaprOptions> options)
    {
        _daprClient = daprClient;
        _logger = logger;
        
        // 弹性策略
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(options.Value.RetryCount, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
                
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(options.Value.CircuitBreakerThreshold, 
                TimeSpan.FromMinutes(1));
    }
    
    // 状态管理方法实现...
    
    // 服务调用方法实现
    public async Task<TResponse> InvokeMethodAsync<TRequest, TResponse>(string appId, string methodName, TRequest request, HttpMethod httpMethod = null)
    {
        return await _retryPolicy.ExecuteAsync(async () => 
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                var httpRequest = _daprClient.CreateInvokeMethodRequest(httpMethod ?? HttpMethod.Post, appId, methodName, request);
                return await _daprClient.InvokeMethodAsync<TResponse>(httpRequest);
            });
        });
    }
    
    // 绑定方法实现
    public async Task InvokeBindingAsync(string bindingName, string operation, object data, Dictionary<string, string> metadata = null)
    {
        await _daprClient.InvokeBindingAsync(bindingName, operation, data, metadata);
    }
    
    // 健康检查方法实现
    public async Task<IEnumerable<DaprHealthCheckResponse>> CheckHealthAsync()
    {
        return await _daprClient.CheckHealthAsync();
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDaprServices(this IServiceCollection services, Action<DaprOptions> configureOptions = null)
    {
        services.AddOptions<DaprOptions>().Configure(configureOptions ?? (opts => {}));
        
        // 添加DaprClient配置
        services.AddDaprClient(builder => builder
            .UseJsonSerializationOptions(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            })
            .UseGrpcChannelOptions(new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30)
                }
            }));
            
        // 添加Actor支持
        services.AddActors(options =>
        {
            options.ActorIdleTimeout = configureOptions?.Invoke(new DaprOptions())?.ActorIdleTimeout ?? TimeSpan.FromMinutes(30);
            options.ActorScanInterval = configureOptions?.Invoke(new DaprOptions())?.ActorScanInterval ?? TimeSpan.FromMinutes(1);
            options.ReentrancyConfig = new Dapr.Actors.Runtime.ActorReentrancyConfig
            {
                Enabled = true,
                MaxStackDepth = 32
            };
        });
            
        services.AddSingleton<IDaprService, DaprService>();
        
        // 添加OpenTelemetry集成
        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddDaprInstrumentation()
                .AddOtlpExporter())
            .WithMetrics(builder => builder
                .AddAspNetCoreInstrumentation()
                .AddDaprInstrumentation()
                .AddOtlpExporter());
                
        return services;
    }
}

// 示例控制器
[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IDaprService _daprService;

    public OrderController(IDaprService daprService)
    {
        _daprService = daprService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var order = await _daprService.GetStateAsync<Order>($"order_{id}");
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(Order order)
    {
        await _daprService.SaveStateAsync($"order_{order.Id}", order);
        await _daprService.PublishAsync("orders", order);
        return Ok();
    }
}

public class Order
{
    public string Id { get; set; }
    public string CustomerName { get; set; }
    public decimal Amount { get; set; }
}