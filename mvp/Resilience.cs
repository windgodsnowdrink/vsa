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
using Microsoft.Extensions.Resilience;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;

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

    // 1. 业务服务接口和实现
    public interface IUserService
    {
        Task<User> GetUserAsync(int userId);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<bool> UpdateUserAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;
        private readonly ResiliencePipeline _resiliencePipeline;
        private readonly ResiliencePipeline<HttpResponseMessage> _httpResiliencePipeline;

        public UserService(
            HttpClient httpClient,
            ILogger<UserService> logger,
            ResiliencePipeline resiliencePipeline,
            ResiliencePipeline<HttpResponseMessage> httpResiliencePipeline)
        {
            _httpClient = httpClient;
            _logger = logger;
            _resiliencePipeline = resiliencePipeline;
            _httpResiliencePipeline = httpResiliencePipeline;
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var url = $"/api/users/{userId}";

            // 使用HTTP专用韧性管道
            var result = await _httpResiliencePipeline.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("Attempting to get user {UserId}", userId);
                var response = await _httpClient.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                return System.Text.Json.JsonSerializer.Deserialize<User>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            });

            return result;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var url = "/api/users";

            return await _httpResiliencePipeline.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("Attempting to get all users");
                var response = await _httpClient.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<User>>(json,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            });
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var url = $"/api/users/{user.Id}";

            return await _httpResiliencePipeline.ExecuteAsync(async ct =>
            {
                _logger.LogInformation("Attempting to update user {UserId}", user.Id);

                var json = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content, ct);
                return response.IsSuccessStatusCode;
            });
        }
    }

    // 2. 用户实体类
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LastLogin { get; set; }
    }

    // 3. 数据库访问服务（演示通用韧性管道）
    public interface IDatabaseService
    {
        Task<T> ExecuteQueryAsync<T>(Func<Task<T>> operation);
        Task ExecuteCommandAsync(Func<Task> operation);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly ResiliencePipeline _resiliencePipeline;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(ResiliencePipeline resiliencePipeline, ILogger<DatabaseService> logger)
        {
            _resiliencePipeline = resiliencePipeline;
            _logger = logger;
        }

        public async Task<T> ExecuteQueryAsync<T>(Func<Task<T>> operation)
        {
            return await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                try
                {
                    _logger.LogInformation("Executing database query");
                    return await operation();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database query failed");
                    throw;
                }
            });
        }

        public async Task ExecuteCommandAsync(Func<Task> operation)
        {
            await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                try
                {
                    _logger.LogInformation("Executing database command");
                    await operation();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database command failed");
                    throw;
                }
            });
        }
    }

    // 4. 启动配置类
    public class ResilienceConfiguration
    {
        public static void ConfigureResilience(IServiceCollection services)
        {
            // 配置默认韧性管道
            services.AddResiliencePipeline("default-pipeline", builder =>
            {
                // 添加重试策略
                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    MaxDelay = TimeSpan.FromSeconds(10),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>()
                        .Handle<InvalidOperationException>(),
                    OnRetry = args =>
                    {
                        Console.WriteLine($"Retry attempt {args.AttemptNumber} after delay {args.RetryDelay}");
                        return ValueTask.CompletedTask;
                    }
                });

                // 添加超时策略
                builder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(30),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"Operation timed out after {args.Timeout}");
                        return ValueTask.CompletedTask;
                    }
                });

                // 添加断路器策略
                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromMinutes(1),
                    MinimumThroughput = 10,
                    BreakDuration = TimeSpan.FromMinutes(1),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutException>(),
                    OnClosed = args =>
                    {
                        Console.WriteLine("Circuit breaker closed");
                        return ValueTask.CompletedTask;
                    },
                    OnOpened = args =>
                    {
                        Console.WriteLine("Circuit breaker opened");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine("Circuit breaker half-opened");
                        return ValueTask.CompletedTask;
                    }
                });

                // 添加速率限制策略
                builder.AddRateLimiter(new RateLimiterStrategyOptions
                {
                    DefaultRateLimiterOptions = new Microsoft.Extensions.Resilience.RateLimiting.FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 10,
                        QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst
                    }
                });

                // 添加熔断策略
                builder.AddFallback(new FallbackStrategyOptions<HttpResponseMessage>
                {
                    FallbackAction = args =>
                    {
                        var fallbackResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                        {
                            Content = new StringContent("{\"message\": \"Service temporarily unavailable, using fallback data\"}")
                        };
                        return new ValueTask<Outcome<HttpResponseMessage>>(Outcome.FromResult(fallbackResponse));
                    },
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>()
                        .HandleResult(response => !response.IsSuccessStatusCode)
                });
            });

            // 配置特定于HTTP的韧性管道
            services.AddResiliencePipeline<HttpResponseMessage>("http-pipeline", builder =>
            {
                // 添加HTTP专用重试策略
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    MaxDelay = TimeSpan.FromSeconds(5),
                    UseJitter = true
                });

                // 添加HTTP超时
                builder.AddTimeout(TimeSpan.FromSeconds(15));

                // 添加HTTP断路器
                builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.3,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromSeconds(30)
                });
            });

            // 配置数据库专用韧性管道
            services.AddResiliencePipeline("database-pipeline", builder =>
            {
                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 2,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Linear,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<SqlException>()  // 假设的数据库异常
                        .Handle<TimeoutException>()
                });

                builder.AddTimeout(TimeSpan.FromMinutes(2));

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.7,
                    SamplingDuration = TimeSpan.FromMinutes(2),
                    MinimumThroughput = 5,
                    BreakDuration = TimeSpan.FromMinutes(2)
                });
            });

            // 配置HttpClient
            services.AddHttpClient<IUserService, UserService>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri("https://api.example.com");
                client.Timeout = TimeSpan.FromMinutes(1);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                MaxConnectionsPerServer = 50
            })
            .AddResilienceHandler("http-pipeline", (builder, context) =>
            {
                // 可以在这里添加更多策略或基于上下文定制策略
            });
        }
    }

    // 5. 模拟的数据库异常类
    public class SqlException : Exception
    {
        public SqlException(string message) : base(message) { }
        public SqlException(string message, Exception innerException) : base(message, innerException) { }
    }

    // 6. 业务逻辑服务类
    public class BusinessLogicService
    {
        private readonly IUserService _userService;
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<BusinessLogicService> _logger;

        public BusinessLogicService(
            IUserService userService,
            IDatabaseService databaseService,
            ILogger<BusinessLogicService> logger)
        {
            _userService = userService;
            _databaseService = databaseService;
            _logger = logger;
        }

        public async Task<User> GetOrCreateUserAsync(int userId)
        {
            try
            {
                // 尝试从API获取用户
                var user = await _userService.GetUserAsync(userId);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get user from API, falling back to database");

                // 降级到数据库查询
                return await _databaseService.ExecuteQueryAsync(async () =>
                {
                    // 模拟数据库查询
                    await Task.Delay(100);
                    return new User { Id = userId, Name = "Fallback User", Email = "fallback@example.com" };
                });
            }
        }

        public async Task<bool> BatchUpdateUsersAsync(IEnumerable<User> users)
        {
            var successCount = 0;
            var failureCount = 0;

            // 并行处理用户更新
            var tasks = new List<Task>();
            foreach (var user in users)
            {
                tasks.Add(ProcessUserUpdate(user));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation("Batch update completed: {SuccessCount} succeeded, {FailureCount} failed",
                successCount, failureCount);

            return failureCount == 0;

            async Task ProcessUserUpdate(User user)
            {
                try
                {
                    var success = await _userService.UpdateUserAsync(user);
                    if (success)
                        Interlocked.Increment(ref successCount);
                    else
                        Interlocked.Increment(ref failureCount);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref failureCount);
                    _logger.LogError(ex, "Failed to update user {UserId}", user.Id);
                }
            }
        }
    }

    // 7. 系统健康检查服务
    public class HealthCheckService
    {
        private readonly ResiliencePipeline _resiliencePipeline;
        private readonly ILogger<HealthCheckService> _logger;

        public HealthCheckService(ResiliencePipeline resiliencePipeline, ILogger<HealthCheckService> logger)
        {
            _resiliencePipeline = resiliencePipeline;
            _logger = logger;
        }

        public async Task<bool> CheckServiceHealthAsync()
        {
            return await _resiliencePipeline.ExecuteAsync(async ct =>
            {
                try
                {
                    // 模拟健康检查
                    await Task.Delay(1000, ct);

                    // 随机模拟健康检查结果
                    var random = new Random();
                    if (random.NextDouble() < 0.1) // 10%的失败率
                    {
                        throw new HttpRequestException("Health check failed");
                    }

                    _logger.LogInformation("Health check passed");
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Health check failed");
                    return false;
                }
            });
        }
    }

    // 8. 程序入口点
    class Program
    {
        static async Task Main(string[] args)
        {
            // 配置依赖注入容器
            var services = new ServiceCollection();

            // 添加日志
            services.AddLogging(builder => builder.AddConsole());

            // 配置韧性策略
            ResilienceConfiguration.ConfigureResilience(services);

            // 注册服务
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDatabaseService, DatabaseService>();
            services.AddTransient<BusinessLogicService>();
            services.AddTransient<HealthCheckService>();

            var serviceProvider = services.BuildServiceProvider();

            // 获取服务并演示使用
            var businessService = serviceProvider.GetRequiredService<BusinessLogicService>();
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();

            Console.WriteLine("=== Microsoft.Extensions.Resilience Production Demo ===");

            // 演示健康检查
            Console.WriteLine("1. Health Check Demo:");
            var isHealthy = await healthCheckService.CheckServiceHealthAsync();
            Console.WriteLine($"Service Health Status: {(isHealthy ? "Healthy" : "Unhealthy")}");

            // 演示用户获取（可能失败，展示重试和降级）
            Console.WriteLine("\n2. User Retrieval Demo:");
            try
            {
                var user = await businessService.GetOrCreateUserAsync(123);
                Console.WriteLine($"Retrieved User: {user.Name} ({user.Email})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"User retrieval failed after all retries: {ex.Message}");
            }

            // 演示批量更新（展示并发控制和错误处理）
            Console.WriteLine("\n3. Batch Update Demo:");
            var usersToUpdate = new[]
            {
            new User { Id = 1, Name = "User 1", Email = "user1@example.com" },
            new User { Id = 2, Name = "User 2", Email = "user2@example.com" },
            new User { Id = 3, Name = "User 3", Email = "user3@example.com" }
        };

            try
            {
                var batchSuccess = await businessService.BatchUpdateUsersAsync(usersToUpdate);
                Console.WriteLine($"Batch update completed: {(batchSuccess ? "SUCCESS" : "PARTIAL SUCCESS")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Batch update failed: {ex.Message}");
            }

            // 演示直接使用韧性管道
            Console.WriteLine("\n4. Direct Pipeline Usage Demo:");
            var pipeline = serviceProvider.GetRequiredService<ResiliencePipeline>();

            await pipeline.ExecuteAsync(async ct =>
            {
                Console.WriteLine("Executing operation with resilience pipeline...");
                await Task.Delay(500, ct);
                Console.WriteLine("Operation completed successfully!");
            });

            Console.WriteLine("\n=== Demo Complete ===");

            // 释放资源
            await serviceProvider.DisposeAsync();
        }
    }

    // 9. 高级配置示例
    public static class AdvancedResilienceConfiguration
    {
        public static void ConfigureAdvancedPipelines(IServiceCollection services)
        {
            // 配置基于租户的自定义策略
            services.AddResiliencePipeline("tenant-pipeline", (builder, context) =>
            {
                var tenantId = context.ServiceProviderKey?.ToString() ?? "default";

                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = tenantId == "premium" ? 5 : 2,
                    Delay = TimeSpan.FromMilliseconds(100),
                    BackoffType = DelayBackoffType.Exponential
                });
            });

            // 配置遥测和监控
            services.AddResiliencePipeline("telemetry-pipeline", builder =>
            {
                builder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    OnRetry = args =>
                    {
                        // 记录重试事件
                        Console.WriteLine($"Retry #{args.AttemptNumber} for operation {args.Context.OperationKey}");
                        return ValueTask.CompletedTask;
                    },
                    OnRetryOpened = args =>
                    {
                        // 记录失败事件
                        Console.WriteLine($"Circuit opened after failed operation {args.Context.OperationKey}");
                        return ValueTask.CompletedTask;
                    }
                });
            });
        }
    }

    // 多策略组合
    // 重试策略（Retry）：指数退避、抖动
    // 超时策略（Timeout）：防止长时间阻塞
    // 断路器策略（Circuit Breaker）：防止级联失败
    // 速率限制（Rate Limiting）：保护后端服务
    // 熔断降级（Fallback）：提供降级处理

    // 类型安全的韧性管道
    // 可以创建针对特定类型的管道
    // services.AddResiliencePipeline<HttpResponseMessage>("http-pipeline", builder => {});

    // 上下文感知配置
    // 基于服务键的上下文配置
    // var tenantPipeline = resiliencePipelineProvider.GetPipeline("tenant-pipeline", "premium");

    // 集成HttpClient
    // services.AddHttpClient<IUserService, UserService>().AddResilienceHandler("http-pipeline", (builder, context) => { });

    // 遥测和监控集成
    // 与Logging集成
    // 与Metrics集成
    // 操作上下文跟踪
}