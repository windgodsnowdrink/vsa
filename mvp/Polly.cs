#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package Microsoft.Extensions.Hosting@10.0.0-rc.1.25451.107
#:package System.Threading.Channels@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool@10.0.0-rc.1.25451.107
#:package Microsoft.Extensions.ObjectPool.DependencyInjection@9.9.0
#:package Swashbuckle.AspNetCore@9.0.4
#:package Swashbuckle.AspNetCore.Swagger@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerGen@9.0.4
#:package Swashbuckle.AspNetCore.SwaggerUI@9.0.4
#:package Scalar.AspNetCore@2.8.0
#:package Ardalis.ListStartupServices@1.1.4
#:package Serilog@4.3.0
#:package Serilog.AspNetCore@9.0.0
#:package Serilog.Enrichers.Environment@3.0.1
#:package Serilog.Enrichers.Process@3.0.0
#:package Serilog.Enrichers.Span@3.1.0
#:package Serilog.Enrichers.Thread@4.0.0
#:package Serilog.Exceptions@8.4.0
#:package Serilog.Extensions.Hosting@9.0.0
#:package Serilog.Extensions.Logging@9.0.2
#:package Serilog.Formatting.Compact@3.0.0
#:package Serilog.Settings.Configuration@9.0.0
#:package Serilog.Sinks.Async@2.1.0
#:package Serilog.Sinks.Console@6.0.0
#:package Serilog.Sinks.EventLog@4.0.0
#:package Serilog.Sinks.Http@9.2.0
#:package Serilog.Sinks.RollingFileAlternate@2.0.9
#:package Serilog.Sinks.Seq@9.0.0
#:package Serilog.Sinks.SpectreConsole@0.3.3
#:package Serilog.Sinks.Trace@4.0.0
#:package ModelContextProtocol@0.3.0-preview.4
#:package ModelContextProtocol.Core@0.3.0-preview.4
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property RollForward=Major
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=True
#:property Platform=Any CPU
#:property PackAsTool=True
#:property PackageType=McpServer
#:property PackageReadmeFile=README.md
#:property PackageId=AOT.SampleMcpServer
#:property PackageVersion=0.0.1-beta
#:property PackageTags=AI; MCP; server; stdio
#:property Description=An MCP server using the MCP C# SDK.

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Ardalis.ListStartupServices;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Polly;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Polly.CircuitBreaker;
using System.Net;
using Polly.Retry;
using Microsoft.Extensions.Logging;
using Polly.Wrap;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

builder.Logging.ClearProviders();               // 只保留下面的 ConsoleProvider，避免干扰
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);                   // 使用颜色化的控制台日志
builder.Logging.SetMinimumLevel(LogLevel.Trace); // 设为 Trace，最细粒度
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.ServiceDiscovery", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Extensions.Resilience", LogLevel.Trace);
builder.Logging.AddFilter("RestEase.HttpClientFactory", LogLevel.Trace);
builder.Logging.AddFilter("App.ServiceDiscoveryHandler", LogLevel.Trace);
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "error", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Fatal)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "fatal", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Information)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "info", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Logger(lc => lc
    .Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Warning)
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "warning", $"{DateTime.Now:yyyyMMdd}.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30))
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog(Log.Logger, true).ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration);
    logging.AddSerilog(Log.Logger, true);
});
builder.Services.AddOptions();
builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination", "X-Platform-Type", "X-CSRF-TOKEN-HEADERNAME", "X-CSRF-TOKEN", "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Forwarded-Prefix", "X-Body-Hash")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
// });

// 测试MCP
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<App.RandomNumberTools>();

builder.Services.Configure<Ardalis.ListStartupServices.ServiceConfig>(config =>
{
    config.Services = [.. builder.Services];
    config.Path = "/services";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapScalarApiReference(); // scalar
}
else
{
    app.UseExceptionHandler("/Error");
}
app.UseSerilogRequestLogging(opt =>
{
    opt.IncludeQueryInRequestPath = true;
});
app.UseRouting();
app.UseCors();
// app.UseAuthorization();
// app.UseSwagger();
// app.UseSwaggerUI(options =>
// {
//     options.SwaggerEndpoint("/swagger/v1/swagger.json", "Robot API V1.1.0.0");
// });
app.UseShowAllServicesMiddleware();
app.MapGet("/", () => "Mcp Agent Integration!");
app.UseEndpoints(options =>
{
    options.MapControllers();
});

await app.RunAsync();

namespace App
{
    public partial class Program;

    // 1. HTTP客户端接口定义
    public interface IUserServiceClient
    {
        Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> UpdateUserAsync(int userId, User user, CancellationToken cancellationToken = default);
    }

    // 2. 用户实体类
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RetryCount { get; set; } = 0;
    }

    // 3. HTTP客户端实现类 - 集成Polly策略
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;

        public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// 获取用户信息 - 包含重试和断路器策略
        /// </summary>
        public async Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Fetching user {UserId} from service", userId);

            try
            {
                var response = await _httpClient.GetAsync($"api/users/{userId}", cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return System.Text.Json.JsonSerializer.Deserialize<User>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// 创建用户 - 包含超时和重试策略
        /// </summary>
        public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating user {UserName}", user.Name);

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/users", content, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", user.Name);
                throw;
            }
        }

        /// <summary>
        /// 更新用户 - 包含复杂的策略组合
        /// </summary>
        public async Task<bool> UpdateUserAsync(int userId, User user, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating user {UserId}: {UserName}", userId, user.Name);

            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/users/{userId}", content, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}: {UserName}", userId, user.Name);
                throw;
            }
        }
    }

    // 4. Polly策略配置类 - 生产级别策略定义
    public static class PollyPolicyConfiguration
    {
        /// <summary>
        /// 配置重试策略
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns>重试策略</returns>
        public static IAsyncPolicy<HttpResponseMessage> ConfigureRetryPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // 处理瞬态HTTP错误（5xx状态码、408请求超时、网络异常）
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound) // 或处理404
                .WaitAndRetryAsync(
                    retryCount: 3, // 最多重试3次
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 指数退避：2秒、4秒、8秒
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        logger.LogWarning(
                            "Retry {RetryCount} after {Delay} seconds due to {Error}",
                            retryCount, timespan.TotalSeconds, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    });
        }

        /// <summary>
        /// 配置断路器策略
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns>断路器策略</returns>
        public static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(ILogger logger)
        {
            return Policy
                .Handle<HttpRequestException>() // 处理HTTP请求异常
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests) // 处理429
                .OrResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.ServiceUnavailable) // 处理503
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5, // 允许5个异常
                    durationOfBreak: TimeSpan.FromMinutes(1), // 断路1分钟
                    onBreak: (outcome, breakDelay) =>
                    {
                        logger.LogError(
                            "Circuit breaker OPENING for {BreakDelay} seconds due to {Error}",
                            breakDelay.TotalSeconds, outcome.Exception?.Message ?? "policy failure");
                    },
                    onReset: () =>
                    {
                        logger.LogInformation("Circuit breaker CLOSED - accepting requests again");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit breaker HALF-OPEN - testing service availability");
                    });
        }

        /// <summary>
        /// 配置超时策略
        /// </summary>
        /// <param name="timeoutSeconds">超时秒数</param>
        /// <param name="logger">日志记录器</param>
        /// <returns>超时策略</returns>
        public static IAsyncPolicy<HttpResponseMessage> ConfigureTimeoutPolicy(
            int timeoutSeconds,
            ILogger logger)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(timeoutSeconds),
                TimeoutStrategy.Optimistic, // 乐观超时策略
                onTimeoutAsync: async (context, timeout, task, exception) =>
                {
                    logger.LogWarning(
                        "Request timed out after {TimeoutSeconds} seconds. Path: {Path}",
                        timeoutSeconds, context.OperationKey);

                    // 可以在这里执行超时后的清理操作
                    await Task.CompletedTask; // 满足async要求
                });
        }

        /// <summary>
        /// 配置回退（Fallback）策略
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <returns>回退策略</returns>
        public static IAsyncPolicy<HttpResponseMessage> ConfigureFallbackPolicy(ILogger logger)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>() // Polly超时异常
                .Or<BrokenCircuitException>() // 断路器打开异常
                .OrResult(r => r.StatusCode == HttpStatusCode.ServiceUnavailable)
                .FallbackAsync(
                    fallbackAction: async (context, cancellationToken) =>
                    {
                        logger.LogWarning("Service unavailable, using fallback response for {OperationKey}",
                            context.OperationKey);

                        // 返回缓存或默认响应
                        var fallbackResponse = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StringContent(
                                System.Text.Json.JsonSerializer.Serialize(new User
                                {
                                    Id = 0,
                                    Name = "Fallback User",
                                    Email = "fallback@example.com"
                                }))
                        };

                        return fallbackResponse;
                    },
                    onFallbackAsync: async (outcome, context) =>
                    {
                        logger.LogWarning("Fallback triggered due to: {Error}",
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());

                        await Task.CompletedTask;
                    });
        }

        /// <summary>
        /// 配置复杂的策略包装（Policy Wrap）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>策略包装</returns>
        public static IAsyncPolicy<HttpResponseMessage> ConfigureComplexPolicy(IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();

            // 策略包装顺序很重要：从内到外的执行顺序
            // 先重试 -> 超时 -> 断路器 -> 回退
            var retryPolicy = ConfigureRetryPolicy(logger);
            var timeoutPolicy = ConfigureTimeoutPolicy(30, logger);
            var circuitBreakerPolicy = ConfigureCircuitBreakerPolicy(logger);
            var fallbackPolicy = ConfigureFallbackPolicy(logger);

            // 使用策略包装组合多个策略
            return Policy.WrapAsync(fallbackPolicy, circuitBreakerPolicy, timeoutPolicy, retryPolicy);
        }
    }

    // 5. 服务依赖注入配置类
    public static class HttpClientServiceConfiguration
    {
        /// <summary>
        /// 配置使用Polly策略的HTTP客户端服务
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void ConfigureHttpClients(IServiceCollection services)
        {
            #region 基础用户服务客户端配置

            services.AddHttpClient<IUserServiceClient, UserServiceClient>("UserService")
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://api.userservice.com/");
                    client.Timeout = TimeSpan.FromMinutes(5); // HttpClient基础超时设置

                    // 添加默认请求头
                    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                })

                // 添加重试策略
                .AddPolicyHandler((services, request) =>
                    PollyPolicyConfiguration.ConfigureRetryPolicy(
                        services.GetRequiredService<ILogger<UserServiceClient>>()))

                // 添加断路器策略
                .AddPolicyHandler((services, request) =>
                    PollyPolicyConfiguration.ConfigureCircuitBreakerPolicy(
                        services.GetRequiredService<ILogger<UserServiceClient>>()))

                // 添加超时策略
                .AddPolicyHandler((services, request) =>
                    PollyPolicyConfiguration.ConfigureTimeoutPolicy(30,
                        services.GetRequiredService<ILogger<UserServiceClient>>()));

            #endregion

            #region 高级策略配置示例

            // 订单服务客户端 - 使用复杂的策略包装
            services.AddHttpClient("OrderService")
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://api.orderservice.com/");
                })
                .AddPolicyHandler(PollyPolicyConfiguration.ConfigureComplexPolicy);

            #endregion

            #region 针对特定HTTP方法的策略配置

            // 为不同HTTP方法配置不同策略
            services.AddHttpClient("PaymentService")
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://api.paymentservice.com/");
                })
                .AddPolicyHandler((services, request) =>
                {
                    // GET请求：允许重试但快速失败
                    if (request.Method == HttpMethod.Get)
                    {
                        return Policy<HttpResponseMessage>
                            .Handle<HttpRequestException>()
                            .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout)
                            .OrResult(r => r.StatusCode == HttpStatusCode.InternalServerError)
                            .WaitAndRetryAsync(
                                2, // 最多重试2次
                                retryAttempt => TimeSpan.FromMilliseconds(500 * retryAttempt),
                                onRetry: (outcome, duration, retryCount, context) =>
                                {
                                    services.GetRequiredService<ILogger<Program>>()
                                        .LogWarning("GET retry #{RetryCount} for {Path}",
                                            retryCount, request.RequestUri?.PathAndQuery);
                                });
                    }

                    // POST/PUT请求：更保守的策略，避免重复操作
                    if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                    {
                        return Policy<HttpResponseMessage>
                            .Handle<HttpRequestException>()
                            .OrResult(r => r.StatusCode == HttpStatusCode.ServiceUnavailable)
                            .WaitAndRetryAsync(
                                1, // 只重试1次
                                retryAttempt => TimeSpan.FromSeconds(2),
                                onRetry: (outcome, duration, retryCount, context) =>
                                {
                                    services.GetRequiredService<ILogger<Program>>()
                                        .LogWarning("POST/PUT retry #{RetryCount} for {Path} - " +
                                            "CAUTION: This might cause duplicate operations",
                                            retryCount, request.RequestUri?.PathAndQuery);
                                });
                    }

                    // DELETE请求：不重试
                    return Policy.NoOpAsync<HttpResponseMessage>();
                });

            #endregion
        }
    }

    // 6. 基于配置的策略提供者类
    public class ConfigurationBasedRetryPolicyProvider
    {
        private readonly ILogger _logger;

        public ConfigurationBasedRetryPolicyProvider(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 根据配置创建动态重试策略
        /// </summary>
        /// <param name="maxRetries">最大重试次数</param>
        /// <param name="delayBase">延迟基数</param>
        /// <param name="useExponentialBackoff">是否使用指数退避</param>
        /// <returns>重试策略</returns>
        public IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(
            int maxRetries,
            double delayBase,
            bool useExponentialBackoff = true)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: maxRetries,
                    sleepDurationProvider: retryAttempt =>
                    {
                        if (useExponentialBackoff)
                        {
                            // 指数退避
                            return TimeSpan.FromSeconds(Math.Pow(delayBase, retryAttempt));
                        }
                        else
                        {
                            // 固定延迟
                            return TimeSpan.FromSeconds(delayBase);
                        }
                    },
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            "Retry {RetryCount}/{MaxRetries} for {Operation} after {Delay} seconds due to {Error}",
                            retryCount, maxRetries, context.OperationKey, timespan.TotalSeconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    });
        }
    }

    // 7. 自定义策略评估逻辑类
    public class CustomPolicyEvaluator
    {
        private readonly ILogger _logger;

        public CustomPolicyEvaluator(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 动态评估是否应该应用重试策略
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="response">HTTP响应</param>
        /// <returns>是否应该重试</returns>
        public bool ShouldRetry(HttpRequestMessage request, HttpResponseMessage response)
        {
            // 根据请求路径和响应状态决定是否重试
            if (request.RequestUri?.AbsolutePath.Contains("/critical") == true)
            {
                // 关键路径不重试
                return false;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // 429状态码需要重试
                return true;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                // 400错误通常是客户端错误，不需要重试
                return false;
            }

            return true; // 默认可以重试
        }
    }

    // 8. Polly持久化状态管理类
    public class PolicyStateManager
    {
        private readonly ILogger _logger;

        public PolicyStateManager(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 监控和记录策略状态变化
        /// </summary>
        /// <param name="circuitBreakerPolicy">断路器策略</param>
        /// <param name="serviceName">服务名称</param>
        public void ConfigureCircuitBreakerMonitoring(ICircuitBreakerPolicy circuitBreakerPolicy, string serviceName)
        {
            var breaker = circuitBreakerPolicy as CircuitBreakerPolicy<HttpResponseMessage>;

            if (breaker != null)
            {
                breaker.OnCircuitOpened += (sender, args) =>
                {
                    _logger.LogError(
                        "Circuit breaker for {ServiceName} OPENED after {FailureCount} failures",
                        serviceName, args.FailureCount);
                };

                breaker.OnCircuitClosed += (sender, args) =>
                {
                    _logger.LogInformation(
                        "Circuit breaker for {ServiceName} CLOSED", serviceName);
                };
            }
        }

        /// <summary>
        /// 获取服务健康状态
        /// </summary>
        /// <param name="circuitBreakerPolicy">断路器策略</param>
        /// <returns>服务状态信息</returns>
        public string GetServiceHealthStatus(ICircuitBreakerPolicy circuitBreakerPolicy)
        {
            var breaker = circuitBreakerPolicy as CircuitBreakerPolicy<HttpResponseMessage>;

            return breaker?.CircuitState switch
            {
                CircuitState.Closed => "Healthy",
                CircuitState.Open => "Unavailable - Circuit Breaker Open",
                CircuitState.HalfOpen => "Recovering - Circuit Breaker Half Open",
                _ => "Unknown"
            };
        }
    }

    // 9. HTTP客户端服务调用示例类
    public class HttpServiceInvoker
    {
        private readonly IUserServiceClient _userServiceClient;
        private readonly HttpClient _orderServiceClient;
        private readonly ILogger<HttpServiceInvoker> _logger;

        public HttpServiceInvoker(
            IUserServiceClient userServiceClient,
            IHttpClientFactory httpClientFactory,
            ILogger<HttpServiceInvoker> logger)
        {
            _userServiceClient = userServiceClient;
            _orderServiceClient = httpClientFactory.CreateClient("OrderService");
            _logger = logger;
        }

        /// <summary>
        /// 安全获取用户信息
        /// </summary>
        public async Task<User> SafeGetUserAsync(int userId)
        {
            try
            {
                return await _userServiceClient.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user {UserId}", userId);
                // 重试策略会自动处理瞬态错误
                throw;
            }
        }

        /// <summary>
        /// 安全创建订单
        /// </summary>
        public async Task<bool> SafeCreateOrderAsync(Order order)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(order);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _orderServiceClient.PostAsync("api/orders", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order for user {UserId}", order.UserId);
                throw;
            }
        }

        /// <summary>
        /// 批量安全操作演示
        /// </summary>
        public async Task BatchOperationsDemoAsync()
        {
            // 模拟多个并发操作以测试Circuit Breaker
            var tasks = new List<Task>();

            for (int i = 1; i <= 10; i++)
            {
                tasks.Add(SafeGetUserAsync(i));
            }

            try
            {
                await Task.WhenAll(tasks);
                _logger.LogInformation("All batch operations completed successfully");
            }
            catch (Exception)
            {
                _logger.LogWarning("Some batch operations failed, but Polly handled retries and circuit breaking");
            }
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }

    // 10. 监控和遥测服务类
    public class PollyMonitoringService
    {
        private readonly ILogger<PollyMonitoringService> _logger;

        public PollyMonitoringService(ILogger<PollyMonitoringService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 记录API调用性能指标
        /// </summary>
        public async Task<T> WithPerformanceMonitoring<T>(Func<Task<T>> operation, string operationName)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var result = await operation();

                stopwatch.Stop();
                _logger.LogInformation("{OperationName} completed in {Duration}ms",
                    operationName, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "{OperationName} failed after {Duration}ms",
                    operationName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }

    // 11. 生产环境策略配置类
    public class ProductionPollyConfiguration
    {
        /// <summary>
        /// 生产环境策略配置 - 包含合理的默认值和监控
        /// </summary>
        public static void ConfigureProductionPolicies(IServiceCollection services)
        {
            services.Configure<HttpClientFactoryOptions>("UserService", options =>
            {
                // 配置生产环境策略
                options.HttpMessageHandlerBuilderActions.Add(builder =>
                {
                    // 添加默认策略
                    var retryPolicy = Policy<HttpResponseMessage>
                        .Handle<HttpRequestException>()
                        .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout)
                        .OrResult(r => r.StatusCode >= HttpStatusCode.InternalServerError)
                        .WaitAndRetryAsync(
                            retryCount: 3,
                            sleepDurationProvider: retryAttempt =>
                                TimeSpan.FromMilliseconds(1000 * Math.Pow(2, retryAttempt - 1)),
                            onRetry: (outcome, timespan, retryCount, context) =>
                            {
                                Console.WriteLine($"Retry #{retryCount} failed: {outcome.Exception?.Message}");
                            });

                    options.AdditionalHttpMessageHandlers.Add(
                        new Polly.Extensions.Http.PollyHttpMessageHandler(retryPolicy));
                });
            });
        }

        /// <summary>
        /// 配置健康检查相关的策略监控
        /// </summary>
        public static void ConfigureHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<PollyHealthCheck>("polly-policies", tags: new[] { "polly" });
        }
    }

    // 12. Polly健康检查类
    public class PollyHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
    {
        public Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
            Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // 实现策略健康检查逻辑
            try
            {
                // 检查各种策略的状态
                return Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(
                    "Polly policies health check failed", ex));
            }
        }
    }

    // 13. 程序启动配置类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("Microsoft.Extensions.Http.Polly Production Demo");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            #region 配置服务

            // 配置日志
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // 配置HTTP客户端和Polly策略
            HttpClientServiceConfiguration.ConfigureHttpClients(builder.Services);

            // 注册应用服务
            builder.Services.AddTransient<HttpServiceInvoker>();
            builder.Services.AddTransient<ConfigurationBasedRetryPolicyProvider>();
            builder.Services.AddTransient<CustomPolicyEvaluator>();
            builder.Services.AddTransient<PollyMonitoringService>();

            #endregion

            var host = builder.Build();

            #region 演示各种Polly策略

            var serviceInvoker = host.Services.GetRequiredService<HttpServiceInvoker>();
            var monitoringService = host.Services.GetRequiredService<PollyMonitoringService>();
            var policyProvider = host.Services.GetRequiredService<ConfigurationBasedRetryPolicyProvider>();

            Console.WriteLine("1. Demonstration:");
            Console.WriteLine("   - Transient HTTP Error Retry Policy (GET requests)");
            Console.WriteLine("   - Circuit Breaker Policy");
            Console.WriteLine("   - Timeout Policy");
            Console.WriteLine("   - Fallback Policy");
            Console.WriteLine();

            // 安全的用户获取演示
            Console.WriteLine("2. Safe User Retrieval Demo:");
            try
            {
                var user = await monitoringService.WithPerformanceMonitoring(
                    () => serviceInvoker.SafeGetUserAsync(1), "GetUser_1");
                Console.WriteLine($"   Retrieved user: {user.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   User retrieval failed: {ex.Message}");
            }

            // 批量操作演示
            Console.WriteLine("\n3. Batch Operations Demo:");
            await serviceInvoker.BatchOperationsDemoAsync();

            // 自定义策略演示
            Console.WriteLine("\n4. Custom Policy Demo:");
            var customPolicy = policyProvider.CreateRetryPolicy(2, 1.5, true);
            Console.WriteLine("   Created custom exponential backoff retry policy");

            #endregion

            Console.WriteLine("\n=== Demo Complete ===");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        // 策略配置最佳实践
        public void Test()
        {
            // 正确的策略包装顺序（从内到外）：
            // Fallback -> CircuitBreaker -> Timeout -> Retry

            // 处理瞬态HTTP错误
            HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        // 记录重试信息
                    });

            // 安全设计原则
            // POST/PUT操作谨慎重试，避免重复创建
            if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
            {
                return Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(2)); // 限制重试次数
            }

            // 上下文感知策略
            // 根据请求类型动态选择策略
            HttpPolicyExtensions.AddPolicyHandler((services, request) =>
             {
                 // 对关键路径使用更保守的策略
                 if (request.RequestUri?.AbsolutePath.Contains("/critical"))
                 {
                     return Policy.NoOpAsync<HttpResponseMessage>();
                 }

                 return ConfigureDefaultRetryPolicy(services);
             });
        }
    }

    // 核心Polly策略类型
    // Retry Policy - 重试策略
    // Circuit Breaker Policy - 断路器策略
    // Timeout Policy - 超时策略
    // Fallback Policy - 回退策略
    // Policy Wrap - 策略包装组合

    // 生产级配置建议
    // 重试策略配置：
    // 次数限制：通常3-5次重试
    // 延迟策略：使用指数退避避免雪崩效应
    // 条件控制：正确区分瞬态错误和永久错误
    // 断路器策略配置：
    // 触发条件：合理设置异常数量阈值
    // 断开时间：给服务足够恢复时间（通常1-5分钟）
    // 状态监控：记录断路器开关状态用于诊断
    // 超时策略配置：
    // 合理超时：根据服务响应特性设置
    // 避免嵌套：防止HttpClient.Timeout和Polly.Timeout重复
    // 回退策略配置：
    // 缓存响应：提供合理的默认值
    // 降级功能：确保部分功能在故障时仍可使用

    // 监控和诊断功能
    // 日志记录：详细记录每个策略的触发和执行
    // 性能监控：测量操作执行时间
    // 状态检查：定期检查断路器和服务状态
    // 指标收集：收集重试次数、失败率等关键指标
    
    // 常见使用场景
    // 外部API调用：处理不稳定的服务依赖
    // 微服务通信：提升分布式系统容错能力
    // 数据库连接：处理网络波动和超时
    // 文件操作：处理I/O相关瞬态错误
}