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
#:property TargetFramework=net11.0
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
using Microsoft.Extensions.ServiceDiscovery;
using Microsoft.Extensions.ServiceDiscovery.Dns;
using Microsoft.Extensions.ServiceDiscovery.Yarp;
using Microsoft.Extensions.Logging;
using Yarp.ReverseProxy.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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

    // 1. 用户服务接口定义
    public interface IUserService
    {
        Task<string> GetUserByIdAsync(int userId);
    }

    // 2. 用户服务实现类 - 使用服务发现的HTTP客户端
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;

        // 注入配置了服务发现的HttpClient
        public UserService(HttpClient httpClient, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("Attempting to call user-service for user {UserId}", userId);

            try
            {
                // 这里使用服务发现解析后的地址发起请求
                var response = await _httpClient.GetAsync($"api/users/{userId}");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call user-service");
                throw;
            }
        }
    }

    // 3. API网关配置提供者 - 动态配置YARP代理
    public class DynamicReverseProxyConfigProvider : IProxyConfigProvider
    {
        private readonly InMemoryConfigProvider _configProvider;

        public DynamicReverseProxyConfigProvider()
        {
            _configProvider = new InMemoryConfigProvider();
        }

        public ProxyConfig GetConfig()
        {
            return _configProvider.GetConfig();
        }

        // 动态更新集群配置
        public void UpdateRoutesAndClusters(
            IEnumerable<RouteConfig> routes,
            IEnumerable<ClusterConfig> clusters)
        {
            _configProvider.Update(routes, clusters);
        }
    }

    // 4. 启动配置类 - 服务发现配置
    public class ServiceDiscoveryConfiguration
    {
        /// <summary>
        /// 配置服务发现功能，包括DNS、YARP集成等
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void ConfigureServiceDiscovery(IServiceCollection services)
        {
            #region 基础服务发现配置

            // 添加服务发现核心服务
            services.AddServiceDiscovery();

            // 添加DNS服务发现提供者
            // 用于通过DNS SRV记录或A记录发现服务
            services.AddDnsServiceDiscovery();

            #endregion

            #region HTTP客户端服务发现配置

            // 配置用户服务的HttpClient，使用服务发现
            // service.user-service 是在DNS中定义的服务名称
            services.AddHttpClient<IUserService, UserService>(static client =>
            {
                // 这里使用服务名称而不是具体的IP地址
                // 服务发现会自动解析为实际的端点地址
                client.BaseAddress = new Uri("https://user-service");
            })
            // 添加服务发现的HTTP消息处理程序
            .AddServiceDiscoveryResolver();

            #endregion

            #region YARP服务发现集成配置

            // 配置YARP反向代理，集成服务发现
            services.AddReverseProxy()
                // 添加服务发现目的地解析器
                .AddServiceDiscoveryDestinationResolver()
                .ConfigureFromMemory(
                    // 配置路由
                    new[]
                    {
                    // 路由到用户服务
                    new RouteConfig()
                    {
                        RouteId = "user-service-route",
                        ClusterId = "user-service-cluster",
                        Match = new RouteMatch
                        {
                            // 匹配 /user-api/ 开头的请求
                            Path = "/user-api/{**catch-all}"
                        },
                        Transforms = new[]
                        {
                            new Dictionary<string, string>
                            {
                                // 路径前缀去除转换
                                // 将 /user-api/users/1 转换为 /users/1
                                ["PathRemovePrefix"] = "/user-api"
                            }
                        }
                    },
                    // 路由到订单服务
                    new RouteConfig()
                    {
                        RouteId = "order-service-route",
                        ClusterId = "order-service-cluster",
                        Match = new RouteMatch
                        {
                            Path = "/order-api/{**catch-all}"
                        },
                        Transforms = new[]
                        {
                            new Dictionary<string, string>
                            {
                                ["PathRemovePrefix"] = "/order-api"
                            }
                        }
                    }
                    },
                    // 配置集群
                    new[]
                    {
                    // 用户服务集群配置
                    new ClusterConfig()
                    {
                        ClusterId = "user-service-cluster",
                        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            // 使用服务发现，destinationId会通过DNS解析实际地址
                            ["user-service-destination"] = new DestinationConfig()
                            {
                                // 服务发现会通过DNS解析 user-service.local 这个服务名称
                                Address = "https://user-service.local",
                                Health = "https://user-service.local/health"
                            }
                        }
                    },
                    // 订单服务集群配置
                    new ClusterConfig()
                    {
                        ClusterId = "order-service-cluster",
                        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["order-service-destination"] = new DestinationConfig()
                            {
                                Address = "https://order-service.local",
                                Health = "https://order-service.local/health"
                            }
                        }
                    }
                    });

            #endregion

            #region 自定义服务发现提供者配置

            // 可以添加自定义服务发现提供者
            services.AddSingleton<IServiceEndpointProvider>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<CustomServiceEndpointProvider>>();
                return new CustomServiceEndpointProvider(logger);
            });

            #endregion
        }
    }

    // 5. 自定义服务发现端点提供者示例
    public class CustomServiceEndpointProvider : IServiceEndpointProvider
    {
        private readonly ILogger<CustomServiceEndpointProvider> _logger;
        private readonly Dictionary<string, string[]> _serviceEndpoints;

        public CustomServiceEndpointProvider(ILogger<CustomServiceEndpointProvider> logger)
        {
            _logger = logger;
            // 模拟服务端点配置（可以从配置文件、注册中心等获取）
            _serviceEndpoints = new Dictionary<string, string[]>
            {
                ["user-service"] = new[] { "https://user1.local:8080", "https://user2.local:8080" },
                ["order-service"] = new[] { "https://order1.local:8081", "https://order2.local:8081" }
            };
        }

        /// <summary>
        /// 解析服务名称为实际的端点地址
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>服务端点列表</returns>
        public async ValueTask<ICollection<Uri>> GetEndpointsAsync(
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Resolving service endpoints for {ServiceName}", serviceName);

            if (_serviceEndpoints.TryGetValue(serviceName, out var endpoints))
            {
                var uriList = new List<Uri>();
                foreach (var endpoint in endpoints)
                {
                    uriList.Add(new Uri(endpoint));
                }
                return uriList;
            }

            // 如果没有找到配置，返回空列表
            _logger.LogWarning("No endpoints found for service {ServiceName}", serviceName);
            return new List<Uri>();
        }
    }

    // 6. 服务发现控制器 - 演示如何在API中使用服务发现功能
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceDiscoveryController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<ServiceDiscoveryController> _logger;
        private readonly ServiceDiscoveryService _serviceDiscovery;

        public ServiceDiscoveryController(
            IUserService userService,
            ILogger<ServiceDiscoveryController> logger,
            ServiceDiscoveryService serviceDiscovery)
        {
            _userService = userService;
            _logger = logger;
            _serviceDiscovery = serviceDiscovery;
        }

        /// <summary>
        /// 获取用户信息 - 演示服务发现的HTTP客户端使用
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户信息</returns>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                // 通过服务发现解析的HttpClient调用用户服务
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", userId);
                return StatusCode(500, "Service discovery failure");
            }
        }

        /// <summary>
        /// 解析服务端点 - 演示直接使用服务发现功能
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>服务端点列表</returns>
        [HttpGet("endpoints/{serviceName}")]
        public async Task<IActionResult> ResolveEndpoints(string serviceName)
        {
            try
            {
                // 直接调用服务发现解析服务端点
                var endpoints = await _serviceDiscovery.GetEndpointsAsync(serviceName);
                return Ok(new
                {
                    ServiceName = serviceName,
                    Endpoints = endpoints.Select(e => e.ToString()).ToArray(),
                    Count = endpoints.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving endpoints for {ServiceName}", serviceName);
                return StatusCode(500, "Endpoint resolution failed");
            }
        }
    }

    // 7. 主程序启动类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region 配置服务发现相关服务

            // 添加日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 配置服务发现
            ServiceDiscoveryConfiguration.ConfigureServiceDiscovery(builder.Services);

            // 添加控制器服务
            builder.Services.AddControllers();

            #endregion

            var app = builder.Build();

            #region 配置HTTP请求管道

            // 使用控制器路由
            app.MapControllers();

            // 配置YARP反向代理中间件
            // 这会处理所有通过反向代理的请求
            app.MapReverseProxy();

            #endregion

            Console.WriteLine("=== Microsoft.Extensions.ServiceDiscovery Production Demo ===");
            Console.WriteLine("Starting service discovery enabled application...");
            Console.WriteLine("API Gateway routes:");
            Console.WriteLine("  - GET /user-api/users/{id} -> resolves to user-service");
            Console.WriteLine("  - GET /order-api/orders/{id} -> resolves to order-service");
            Console.WriteLine("Direct service calls:");
            Console.WriteLine("  - GET /api/ServiceDiscovery/users/{id}");
            Console.WriteLine("  - GET /api/ServiceDiscovery/endpoints/{serviceName}");
            Console.WriteLine();

            await app.RunAsync();
        }
    }

    // 8. 高级服务发现配置示例
    public static class AdvancedServiceDiscoveryConfiguration
    {
        /// <summary>
        /// 高级服务发现配置，包括多种发现机制
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void ConfigureAdvancedServiceDiscovery(IServiceCollection services)
        {
            #region 多种服务发现机制配置

            // 添加服务发现核心功能
            services.AddServiceDiscovery(options =>
            {
                // 设置解析超时时间
                options.DefaultTimeout = TimeSpan.FromSeconds(5);

                // 设置重试次数
                options.DefaultRetryAttempts = 3;

                // 设置缓存过期时间
                options.CacheExpiration = TimeSpan.FromMinutes(1);
            });

            // 添加DNS发现机制
            services.AddDnsServiceDiscovery(dnsOptions =>
            {
                // 设置DNS查询超时
                dnsOptions.QueryTimeout = TimeSpan.FromSeconds(3);

                // 启用SRV记录查询
                dnsOptions.UseSrvRecords = true;

                // 设置DNS后缀（用于服务名称自动补全）
                dnsOptions.DefaultDomain = "local";
            });

            #endregion

            #region 自定义解析策略

            // 添加自定义解析策略
            services.AddServiceDiscoveryCore();

            // 可以注册多个端点提供者，服务发现会轮询调用
            services.AddSingleton<IServiceEndpointProvider>(provider =>
                new CustomServiceEndpointProvider(
                    provider.GetRequiredService<ILogger<CustomServiceEndpointProvider>>()));

            #endregion

            #region 健康检查集成

            // 配置健康检查
            services.AddHealthChecks();

            #endregion
        }
    }

    // 9. 服务发现客户端使用示例类
    public class ServiceDiscoveryClientDemo
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceDiscoveryService _serviceDiscovery;
        private readonly ILogger<ServiceDiscoveryClientDemo> _logger;

        public ServiceDiscoveryClientDemo(
            IHttpClientFactory httpClientFactory,
            ServiceDiscoveryService serviceDiscovery,
            ILogger<ServiceDiscoveryClientDemo> logger)
        {
            _httpClientFactory = httpClientFactory;
            _serviceDiscovery = serviceDiscovery;
            _logger = logger;
        }

        /// <summary>
        /// 演示动态负载均衡调用
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="endpointPath">端点路径</param>
        /// <returns>响应结果</returns>
        public async Task<string> CallServiceWithLoadBalancing(string serviceName, string endpointPath)
        {
            _logger.LogInformation("Calling service {ServiceName} with path {EndpointPath}",
                serviceName, endpointPath);

            try
            {
                // 重复调用显示负载均衡效果
                for (int i = 0; i < 5; i++)
                {
                    // 创建HTTP客户端（服务发现会自动解析地址）
                    var client = _httpClientFactory.CreateClient(serviceName);

                    // 发起请求
                    var response = await client.GetAsync(endpointPath);
                    var result = await response.Content.ReadAsStringAsync();

                    _logger.LogInformation("Call #{Attempt}: Status={Status}, Response={Response}",
                        i + 1, response.StatusCode, result.Substring(0, Math.Min(100, result.Length)));

                    await Task.Delay(100); // 等待一下再下一次调用
                }

                return "Service calls completed successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during service discovery client demo");
                throw;
            }
        }
    }

    // 10. 监控和遥测服务发现活动
    public class ServiceDiscoveryMonitoringService
    {
        private readonly ServiceDiscoveryService _serviceDiscovery;
        private readonly ILogger<ServiceDiscoveryMonitoringService> _logger;

        public ServiceDiscoveryMonitoringService(
            ServiceDiscoveryService serviceDiscovery,
            ILogger<ServiceDiscoveryMonitoringService> logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _logger = logger;
        }

        /// <summary>
        /// 监控服务发现解析统计信息
        /// </summary>
        public async Task MonitorServiceDiscovery()
        {
            var servicesToMonitor = new[] { "user-service", "order-service", "payment-service" };

            foreach (var service in servicesToMonitor)
            {
                try
                {
                    _logger.LogInformation("Monitoring service discovery for {ServiceName}", service);

                    // 获取服务端点信息
                    var endpoints = await _serviceDiscovery.GetEndpointsAsync(service);

                    _logger.LogInformation("Service {ServiceName} resolved to {EndpointCount} endpoints:",
                        service, endpoints.Count);

                    foreach (var endpoint in endpoints)
                    {
                        _logger.LogInformation("  - {Endpoint}", endpoint);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to resolve service {ServiceName}", service);
                }
            }
        }
    }

    //核心组件说明
    // Microsoft.Extensions.ServiceDiscovery - 核心服务发现功能
    // Microsoft.Extensions.ServiceDiscovery.Dns - DNS发现机制
    // Microsoft.Extensions.ServiceDiscovery.Yarp - YARP反向代理集成

    // 服务发现配置关键点
    // 服务名称使用约定：通常使用简化的名称，不包含协议和端口
    // 如：user-service 而不是 https://user-service:8080

    // 服务发现会在后台解析这些服务名称为实际的端点地址
    // services.AddHttpClient<IUserService, UserService>(static client => { client.BaseAddress = new Uri("https://user-service"); }); // 服务名称

    // YARP与服务发现集成
    // YARP配置中使用服务发现解析的地址
    services.AddReverseProxy().AddServiceDiscoveryDestinationResolver() .ConfigureFromMemory(routes, clusters); // // 启用服务发现问题解析

    // 集群配置使用服务名称
    new ClusterConfig()
    {
        ClusterId = "user-service-cluster",
        Destinations = new Dictionary<string, DestinationConfig>
        {
            ["user-service"] = new DestinationConfig()
            {
                Address = "https://user-service.local",
                Health = "https://user-service.local/health"
            }
        }
    }

    // 生产级配置建议
    // 超时设置：配置合理的解析超时时间
    // 缓存策略：避免频繁的DNS查询
    // 重试机制：解析失败时的重试策略
    // 健康检查：定期检查服务端点健康状态
    // 日志记录：详细记录服务发现问题和活动

    // 使用优势
    // 服务解耦：客户端代码不需要知道具体的服务地址
    // 负载均衡：自动在多个实例间进行负载均衡
    // 故障恢复：支持服务实例的动态添加和移除
    // 零配置部署：支持通过环境变量或DNS配置服务发现
}