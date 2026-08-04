#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Http.Resilience@8.0.0
#:package Microsoft.Extensions.ServiceDiscovery@1.0.0-preview.1.24072.1
#:package Polly@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.ServiceDiscovery;
using Polly;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// 1. 添加HTTP客户端弹性策略
builder.Services.AddHttpClient("resilient-client")
    .AddStandardResilienceHandler(options =>
    {
        // 重试策略
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.Retry.BaseDelay = TimeSpan.FromSeconds(1);
        
        // 熔断策略
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
        
        // 超时策略
        options.Timeout.Timeout = TimeSpan.FromSeconds(5);
        
        // 服务发现集成
        options.ServiceDiscovery.DiscoveryMode = ServiceDiscoveryMode.PerRequest;
    })
    .AddPolicyHandlerFromRegistry("fallback-policy");

// 2. 注册Polly策略
builder.Services.AddPolicyRegistry(registry =>
{
    // 降级策略
    registry.Add("fallback-policy", Policy<HttpResponseMessage>
        .HandleResult(r => !r.IsSuccessStatusCode)
        .FallbackAsync(async (outcome, context, token) =>
        {
            // 实现降级逻辑
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("Fallback response")
            };
        }));
});

// 3. 添加健康检查
builder.Services.AddHealthChecks()
    .AddCheck<HttpClientHealthCheck>("http-client");

var app = builder.Build();

app.MapGet("/", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient("resilient-client");
    var response = await client.GetAsync("http://example.com/api");
    return response.IsSuccessStatusCode 
        ? Results.Ok(await response.Content.ReadAsStringAsync()) 
        : Results.Problem("Service unavailable");
});

app.MapHealthChecks("/health");

app.Run();

// 自定义健康检查
public class HttpClientHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _clientFactory;

    public HttpClientHealthCheck(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient("resilient-client");
            var response = await client.GetAsync("http://example.com/health");
            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy() 
                : HealthCheckResult.Unhealthy("HTTP client unhealthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}