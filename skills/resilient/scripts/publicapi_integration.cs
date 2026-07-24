#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Http@10.0.0
#:package Microsoft.Extensions.Http.Polly@10.0.0
#:package Polly@7.2.4
#:package System.Net.Http@8.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property PublishAot=true
#:property TrimMode=partial
#:property EnableCompressionInSingleFile=true
#:property SelfContained=true

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// API集成选项
public class ApiIntegrationOptions
{
    // 基础URL
    public string BaseUrl { get; set; } = "https://api.example.com";
    
    // 超时设置
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    
    // 重试策略
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryBackoff { get; set; } = TimeSpan.FromSeconds(1);
    
    // 熔断策略
    public int CircuitBreakThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakDuration { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan CircuitBreakSamplingDuration { get; set; } = TimeSpan.FromMinutes(1);
    
    // 限速策略
    public int RateLimitPermits { get; set; } = 100;
    public TimeSpan RateLimitWindow { get; set; } = TimeSpan.FromSeconds(1);
    
    // 健康检查
    public bool EnableHealthCheck { get; set; } = true;
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
    
    // 遥测
    public bool EnableTelemetry { get; set; } = true;
    
    // 日志
    public bool EnableDetailedLogging { get; set; } = false;
}

// API响应模型
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; }
    public string? RequestId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// API客户端接口
public interface IApiClient
{
    // GET请求
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    
    // POST请求
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    // PUT请求
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    // DELETE请求
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    
    // 发送自定义请求
    Task<ApiResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default);
    
    // 获取健康状态
    Task<ApiResponse<HealthCheckResult>> HealthCheckAsync(CancellationToken cancellationToken = default);
    
    // 获取客户端指标
    ApiClientMetrics GetMetrics();
}

// API客户端指标
public class ApiClientMetrics
{
    public int TotalRequests { get; set; }
    public int SuccessfulRequests { get; set; }
    public int FailedRequests { get; set; }
    public int RetriedRequests { get; set; }
    public int CircuitBrokenRequests { get; set; }
    public int RateLimitedRequests { get; set; }
    public TimeSpan TotalRequestTime { get; set; }
    public double AverageRequestTime => TotalRequests > 0 ? TotalRequestTime.TotalMilliseconds / TotalRequests : 0;
    public double SuccessRate => TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests : 0;
}

// 健康检查结果
public class HealthCheckResult
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, object> Metrics { get; set; } = new();
}

// API客户端实现
public class ApiClient : IApiClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ApiIntegrationOptions _options;
    private readonly ILogger<ApiClient>? _logger;
    private readonly ApiClientMetrics _metrics = new();
    private Timer? _healthCheckTimer;
    private bool _disposed = false;

    public ApiClient(HttpClient httpClient, IOptions<ApiIntegrationOptions> options, ILogger<ApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        // 配置HttpClient
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = _options.Timeout;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ApiClient/1.0");

        // 启动健康检查
        if (_options.EnableHealthCheck)
        {
            StartHealthCheck();
        }
    }

    // 启动健康检查
    private void StartHealthCheck()
    {
        _healthCheckTimer = new Timer(async _ =>
        {
            try
            {
                var result = await HealthCheckAsync();
                _logger?.LogInformation("[ApiClient] Health check status: {Status}, Message: {Message}",
                    result.Success ? "Healthy" : "Unhealthy", result.Error ?? result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "[ApiClient] Failed to perform health check");
            }
        }, null, _options.HealthCheckInterval, _options.HealthCheckInterval);
    }

    // GET请求
    public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return await SendAsync<T>(request, cancellationToken);
    }

    // POST请求
    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(data)
        };
        return await SendAsync<T>(request, cancellationToken);
    }

    // PUT请求
    public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = JsonContent.Create(data)
        };
        return await SendAsync<T>(request, cancellationToken);
    }

    // DELETE请求
    public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
        return await SendAsync<T>(request, cancellationToken);
    }

    // 发送自定义请求
    public async Task<ApiResponse<T>> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        _metrics.TotalRequests++;

        try
        {
            if (_options.EnableDetailedLogging)
            {
                _logger?.LogInformation("[ApiClient] Sending {Method} request to {Url}",
                    request.Method, request.RequestUri);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var endTime = DateTime.UtcNow;
            _metrics.TotalRequestTime += endTime - startTime;

            if (response.IsSuccessStatusCode)
            {
                _metrics.SuccessfulRequests++;
                var data = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
                
                return new ApiResponse<T>
                {
                    Success = true,
                    Data = data,
                    StatusCode = (int)response.StatusCode
                };
            }
            else
            {
                _metrics.FailedRequests++;
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                
                _logger?.LogWarning("[ApiClient] Request failed with status code {StatusCode}: {Error}",
                    (int)response.StatusCode, errorContent);

                return new ApiResponse<T>
                {
                    Success = false,
                    Error = errorContent,
                    StatusCode = (int)response.StatusCode
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _metrics.FailedRequests++;
            var endTime = DateTime.UtcNow;
            _metrics.TotalRequestTime += endTime - startTime;

            _logger?.LogError(ex, "[ApiClient] HTTP request failed");
            
            return new ApiResponse<T>
            {
                Success = false,
                Error = ex.Message,
                StatusCode = 0
            };
        }
        catch (TaskCanceledException ex)
        {
            _metrics.FailedRequests++;
            var endTime = DateTime.UtcNow;
            _metrics.TotalRequestTime += endTime - startTime;

            _logger?.LogError(ex, "[ApiClient] Request canceled or timed out");
            
            return new ApiResponse<T>
            {
                Success = false,
                Error = "Request canceled or timed out",
                StatusCode = 408 // Request Timeout
            };
        }
        catch (Exception ex)
        {
            _metrics.FailedRequests++;
            var endTime = DateTime.UtcNow;
            _metrics.TotalRequestTime += endTime - startTime;

            _logger?.LogError(ex, "[ApiClient] Unexpected error");
            
            return new ApiResponse<T>
            {
                Success = false,
                Error = ex.Message,
                StatusCode = 500 // Internal Server Error
            };
        }
    }

    // 获取健康状态
    public async Task<ApiResponse<HealthCheckResult>> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await GetAsync<object>("/health", cancellationToken);
            
            var healthResult = new HealthCheckResult
            {
                IsHealthy = response.Success,
                Status = response.Success ? "Healthy" : "Unhealthy",
                Message = response.Success ? "API service is healthy" : "API service is unhealthy"
            };

            // 添加指标
            healthResult.Metrics.Add("TotalRequests", _metrics.TotalRequests);
            healthResult.Metrics.Add("SuccessfulRequests", _metrics.SuccessfulRequests);
            healthResult.Metrics.Add("FailedRequests", _metrics.FailedRequests);
            healthResult.Metrics.Add("SuccessRate", _metrics.SuccessRate);
            healthResult.Metrics.Add("AverageRequestTimeMs", _metrics.AverageRequestTime);

            return new ApiResponse<HealthCheckResult>
            {
                Success = response.Success,
                Data = healthResult,
                StatusCode = response.StatusCode
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "[ApiClient] Health check failed");
            
            return new ApiResponse<HealthCheckResult>
            {
                Success = false,
                Error = ex.Message,
                StatusCode = 500
            };
        }
    }

    // 获取客户端指标
    public ApiClientMetrics GetMetrics()
    {
        return _metrics;
    }

    // 释放资源
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // 释放资源
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _healthCheckTimer?.Dispose();
            }

            _disposed = true;
        }
    }
}

// 依赖注入扩展
public static class ApiClientExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        return services.AddApiClient(options => { });
    }

    public static IServiceCollection AddApiClient(this IServiceCollection services, Action<ApiIntegrationOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        services.AddHttpClient<IApiClient, ApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ApiIntegrationOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.Timeout;
        })
        .AddPolicyHandler((serviceProvider, request) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ApiIntegrationOptions>>().Value;
            
            // 重试策略
            var retryPolicy = Polly.Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(
                    retryCount: options.RetryCount,
                    sleepDurationProvider: retryAttempt => options.RetryBackoff * Math.Pow(2, retryAttempt - 1),
                    onRetry: (exception, timespan, retryAttempt, context) =>
                    {
                        var logger = serviceProvider.GetService<ILogger<ApiClient>>();
                        logger?.LogWarning("[ApiClient] Retry {RetryAttempt} after {Delay}ms due to {Exception}",
                            retryAttempt, timespan.TotalMilliseconds, exception.Result?.StatusCode ?? exception.Exception?.Message);
                    });
            
            // 熔断策略
            var circuitBreakerPolicy = Polly.Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: options.CircuitBreakThreshold,
                    durationOfBreak: options.CircuitBreakDuration,
                    onBreak: (exception, timespan) =>
                    {
                        var logger = serviceProvider.GetService<ILogger<ApiClient>>();
                        logger?.LogWarning("[ApiClient] Circuit breaker opened for {Duration}ms due to {Exception}",
                            timespan.TotalMilliseconds, exception.Result?.StatusCode ?? exception.Exception?.Message);
                    },
                    onReset: () =>
                    {
                        var logger = serviceProvider.GetService<ILogger<ApiClient>>();
                        logger?.LogInformation("[ApiClient] Circuit breaker reset");
                    });
            
            // 组合策略
            return Polly.Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        });

        return services;
    }
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册API客户端
        services.AddApiClient(options =>
        {
            options.BaseUrl = "https://jsonplaceholder.typicode.com";
            options.Timeout = TimeSpan.FromSeconds(10);
            options.RetryCount = 3;
            options.EnableHealthCheck = true;
            options.EnableDetailedLogging = true;
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取API客户端
        var apiClient = serviceProvider.GetRequiredService<IApiClient>();
        
        Console.WriteLine("API Client Integration Demo");
        Console.WriteLine("=" + new string('=', 50));
        
        try
        {
            // 测试GET请求
            Console.WriteLine("\nTesting GET request...");
            var getUserResponse = await apiClient.GetAsync<dynamic>("/users/1");
            if (getUserResponse.Success)
            {
                Console.WriteLine($"✓ GET request successful: {getUserResponse.Data.name}");
            }
            else
            {
                Console.WriteLine($"✗ GET request failed: {getUserResponse.Error}");
            }
            
            // 测试POST请求
            Console.WriteLine("\nTesting POST request...");
            var postData = new { title = "Test Post", body = "This is a test post", userId = 1 };
            var postResponse = await apiClient.PostAsync<dynamic>("/posts", postData);
            if (postResponse.Success)
            {
                Console.WriteLine($"✓ POST request successful: Post ID = {postResponse.Data.id}");
            }
            else
            {
                Console.WriteLine($"✗ POST request failed: {postResponse.Error}");
            }
            
            // 测试健康检查
            Console.WriteLine("\nTesting health check...");
            var healthResponse = await apiClient.HealthCheckAsync();
            if (healthResponse.Success)
            {
                Console.WriteLine($"✓ Health check successful: {healthResponse.Data.Status}");
                Console.WriteLine($"  Total Requests: {healthResponse.Data.Metrics["TotalRequests"]}");
                Console.WriteLine($"  Successful Requests: {healthResponse.Data.Metrics["SuccessfulRequests"]}");
                Console.WriteLine($"  Failed Requests: {healthResponse.Data.Metrics["FailedRequests"]}");
                Console.WriteLine($"  Success Rate: {healthResponse.Data.Metrics["SuccessRate"]:P2}");
                Console.WriteLine($"  Average Request Time: {healthResponse.Data.Metrics["AverageRequestTimeMs"]:F2}ms");
            }
            else
            {
                Console.WriteLine($"✗ Health check failed: {healthResponse.Error}");
            }
            
            // 测试错误处理
            Console.WriteLine("\nTesting error handling...");
            var errorResponse = await apiClient.GetAsync<dynamic>("/nonexistent-endpoint");
            if (!errorResponse.Success)
            {
                Console.WriteLine($"✓ Error handling working: {errorResponse.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // 释放资源
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}