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

using App;
using Ardalis.ListStartupServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ModelContextProtocol.Server;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks;

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

    // 1. 数据模型定义
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        // 计算属性
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}";

        [JsonIgnore]
        public string DisplayName => string.IsNullOrEmpty(FullName) ? UserName : FullName;
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int ErrorCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // 分页信息 (如果是分页结果)
        public long TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class UserQueryRequest
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
        public List<string> Filters { get; set; } = new List<string>();
    }

    // 2. 异常定义
    public class HttpJsonClientException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ResponseBody { get; }

        public HttpJsonClientException(string message, HttpStatusCode statusCode, string responseBody)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public HttpJsonClientException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class ApiValidationException : Exception
    {
        public Dictionary<string, string[]> ValidationErrors { get; }

        public ApiValidationException(string message, Dictionary<string, string[]> errors)
            : base(message)
        {
            ValidationErrors = errors;
        }
    }

    // 3. 生产级HttpClient工厂服务
    public interface IHttpJsonClientFactory
    {
        Task<HttpClient> CreateClientAsync(string clientName);
        Task<HttpClient> CreateSecureClientAsync(string clientName, string accessToken);
        Task<bool> ValidateEndpointAsync(string baseUrl, CancellationToken cancellationToken = default);
    }

    public class HttpJsonClientFactory : IHttpJsonClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpJsonClientFactory> _logger;
        private readonly Dictionary<string, HttpClientConfiguration> _clientConfigurations;

        public HttpJsonClientFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<HttpJsonClientFactory> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _clientConfigurations = new Dictionary<string, HttpClientConfiguration>();
            InitializeDefaultConfigurations();
        }

        /// <summary>
        /// 创建优化配置的HttpClient实例
        /// </summary>
        public async Task<HttpClient> CreateClientAsync(string clientName)
        {
            _logger.LogInformation("Creating HTTP client instance for {ClientName}", clientName);

            try
            {
                var client = _httpClientFactory.CreateClient(clientName);

                if (_clientConfigurations.TryGetValue(clientName, out var config))
                {
                    // 配置默认头部
                    if (config.DefaultHeaders != null)
                    {
                        foreach (var header in config.DefaultHeaders)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }

                    // 配置JSON序列化选项
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                }

                _logger.LogDebug("HTTP client created successfully for {ClientName}", clientName);
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating HTTP client for {ClientName}", clientName);
                throw new HttpJsonClientException($"Failed to create HTTP client for {clientName}", ex);
            }
        }

        /// <summary>
        /// 创建安全认证的HttpClient实例
        /// </summary>
        public async Task<HttpClient> CreateSecureClientAsync(string clientName, string accessToken)
        {
            _logger.LogInformation("Creating secure HTTP client for {ClientName}", clientName);

            try
            {
                var client = await CreateClientAsync(clientName);

                // 添加认证头部
                if (!string.IsNullOrEmpty(accessToken))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                }

                _logger.LogDebug("Secure HTTP client created with token for {ClientName}", clientName);
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating secure HTTP client for {ClientName}", clientName);
                throw new HttpJsonClientException($"Failed to create secure HTTP client for {clientName}", ex);
            }
        }

        /// <summary>
        /// 验证API端点可访问性
        /// </summary>
        public async Task<bool> ValidateEndpointAsync(string baseUrl, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Validating endpoint accessibility for {BaseUrl}", baseUrl);

            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.GetAsync("/health", cancellationToken);
                var isAccessible = response.IsSuccessStatusCode;

                _logger.LogInformation("Endpoint validation result for {BaseUrl}: {IsAccessible}",
                    baseUrl, isAccessible);

                return isAccessible;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Endpoint validation failed for {BaseUrl}", baseUrl);
                return false;
            }
        }

        private void InitializeDefaultConfigurations()
        {
            _clientConfigurations["Default"] = new HttpClientConfiguration
            {
                BaseUrl = "https://api.example.com",
                DefaultHeaders = new Dictionary<string, string>
            {
                { "User-Agent", "HttpJsonClient/1.0" },
                { "Accept", "application/json" },
                { "Content-Type", "application/json" }
            },
                TimeoutSeconds = 30,
                RetryAttempts = 3,
                JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }
            };
        }
    }

    public class HttpClientConfiguration
    {
        public string BaseUrl { get; set; }
        public Dictionary<string, string> DefaultHeaders { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryAttempts { get; set; } = 3;
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions();
    }

    // 4. 生产级HttpJson客户端服务
    public class HttpJsonClientService
    {
        private readonly ILogger<HttpJsonClientService> _logger;
        private readonly IHttpJsonClientFactory _clientFactory;
        private readonly SemaphoreSlim _rateLimitSemaphore;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public HttpJsonClientService(
            ILogger<HttpJsonClientService> logger,
            IHttpJsonClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _rateLimitSemaphore = new SemaphoreSlim(10, 10); // 限制并发请求

            // 配置标准的JSON序列化选项
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        /// GET请求获取单个对象
        /// </summary>
        public async Task<T> GetAsync<T>(
            string clientName,
            string endpoint,
            CancellationToken cancellationToken = default,
            int timeoutSeconds = 30)
        {
            _logger.LogInformation("GET request to {ClientName}/{Endpoint}", clientName, endpoint);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);

            try
            {
                using var client = await _clientFactory.CreateClientAsync(clientName);
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                // 使用System.Net.Http.Json扩展方法
                var response = await client.GetAsync(endpoint, cancellationToken);

                await HandleHttpResponseAsync(response, "GET", endpoint);

                // 从JSON响应反序列化对象
                var result = await response.Content.ReadFromJsonAsync<T>(
                    _jsonSerializerOptions, cancellationToken);

                _logger.LogDebug("GET request completed - {Endpoint}: {ResultType}",
                    endpoint, typeof(T).Name);

                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("GET request cancelled - {Endpoint}", endpoint);
                throw new HttpJsonClientException("Request cancelled by user", HttpStatusCode.RequestTimeout, null);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "GET request timeout - {Endpoint}", endpoint);
                throw new HttpJsonClientException("Request timeout", HttpStatusCode.RequestTimeout, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET request failed - {Endpoint}", endpoint);
                throw new HttpJsonClientException($"GET request failed: {ex.Message}", ex);
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        /// <summary>
        /// GET请求获取对象列表
        /// </summary>
        public async Task<List<T>> GetListAsync<T>(
            string clientName,
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET list request to {ClientName}/{Endpoint}", clientName, endpoint);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);

            try
            {
                using var client = await _clientFactory.CreateClientAsync(clientName);
                var response = await client.GetAsync(endpoint, cancellationToken);

                await HandleHttpResponseAsync(response, "GET", endpoint);

                // 从JSON数组响应反序列化列表
                var result = await response.Content.ReadFromJsonAsync<List<T>>(
                    _jsonSerializerOptions, cancellationToken);

                _logger.LogInformation("List GET request completed - Count: {ItemCount}", result?.Count ?? 0);

                return result ?? new List<T>();
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        /// <summary>
        /// POST请求创建对象
        /// </summary>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string clientName,
            string endpoint,
            TRequest content,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST request to {ClientName}/{Endpoint} with {ContentType}",
                clientName, endpoint, typeof(TRequest).Name);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);

            try
            {
                using var client = await _clientFactory.CreateClientAsync(clientName);

                // 使用System.Net.Http.Json扩展发送JSON内容
                var response = await client.PostAsJsonAsync(endpoint, content,
                    _jsonSerializerOptions, cancellationToken);

                await HandleHttpResponseAsync(response, "POST", endpoint);

                // 从JSON响应读取创建结果
                var result = await response.Content.ReadFromJsonAsync<TResponse>(
                    _jsonSerializerOptions, cancellationToken);

                _logger.LogInformation("POST request completed successfully");

                return result;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        /// <summary>
        /// PUT请求更新对象
        /// </summary>
        public async Task<TResponse> PutAsync<TRequest, TResponse>(
            string clientName,
            string endpoint,
            TRequest content,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT request to {ClientName}/{Endpoint} with {ContentType}",
                clientName, endpoint, typeof(TRequest).Name);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);

            try
            {
                using var client = await _clientFactory.CreateClientAsync(clientName);

                // 使用System.Net.Http.Json扩展发送JSON内容
                var response = await client.PutAsJsonAsync(endpoint, content,
                    _jsonSerializerOptions, cancellationToken);

                await HandleHttpResponseAsync(response, "PUT", endpoint);

                var result = await response.Content.ReadFromJsonAsync<TResponse>(
                    _jsonSerializerOptions, cancellationToken);

                _logger.LogInformation("PUT request completed successfully");

                return result;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        /// <summary>
        /// DELETE请求删除对象
        /// </summary>
        public async Task<bool> DeleteAsync(
            string clientName,
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE request to {ClientName}/{Endpoint}", clientName, endpoint);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);

            try
            {
                using var client = await _clientFactory.CreateClientAsync(clientName);
                var response = await client.DeleteAsync(endpoint, cancellationToken);

                await HandleHttpResponseAsync(response, "DELETE", endpoint);

                _logger.LogInformation("DELETE request completed successfully");

                return response.IsSuccessStatusCode;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        /// <summary>
        /// 发送带有JSON内容的自定义HTTP请求
        /// </summary>
        public async Task<HttpResponseMessage> SendJsonRequestAsync<T>(
            string clientName,
            HttpMethod method,
            string endpoint,
            T content = default,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Custom JSON request to {ClientName}/{Endpoint} with {Method}",
                clientName, endpoint, method.Method);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);

            try
            {
                using var client = await _clientFactory.CreateClientAsync(clientName);
                using var request = new HttpRequestMessage(method, endpoint);

                if (content != null)
                {
                    // 创建JSON内容
                    request.Content = JsonContent.Create(content, options: _jsonSerializerOptions);
                }

                var response = await client.SendAsync(request, cancellationToken);

                _logger.LogDebug("Custom JSON request sent successfully");

                return response;
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        /// <summary>
        /// 处理HTTP响应和错误
        /// </summary>
        private async Task HandleHttpResponseAsync(HttpResponseMessage response, string method, string endpoint)
        {
            _logger.LogDebug("HTTP Response - Status: {StatusCode}, Method: {Method}, Endpoint: {Endpoint}",
                response.StatusCode, method, endpoint);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                // 特殊处理API验证错误
                if ((int)response.StatusCode == 422) // Unprocessable Entity
                {
                    var validationErrors = JsonSerializer.Deserialize<Dictionary<string, string[]>>(
                        responseBody, _jsonSerializerOptions);
                    throw new ApiValidationException("Validation failed", validationErrors ?? new Dictionary<string, string[]>());
                }

                // 处理其他HTTP错误
                string errorMessage = $"HTTP {response.StatusCode} - {response.ReasonPhrase}";
                if (!string.IsNullOrEmpty(responseBody))
                {
                    errorMessage += $": {responseBody}";
                }

                throw new HttpJsonClientException(errorMessage, response.StatusCode, responseBody);
            }
        }
    }

    // 5. 生产级API查询服务
    public class ApiServiceQueryHandler
    {
        private readonly HttpJsonClientService _clientService;
        private readonly ILogger<ApiServiceQueryHandler> _logger;

        public ApiServiceQueryHandler(
            HttpJsonClientService clientService,
            ILogger<ApiServiceQueryHandler> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// 分页查询用户数据
        /// </summary>
        public async Task<ApiResponse<List<User>>> QueryUsersAsync(
            string clientName,
            UserQueryRequest queryRequest,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Querying users with {SearchTerm} - Page {PageNumber}",
                queryRequest.SearchTerm, queryRequest.PageNumber);

            try
            {
                var endpoint = $"/api/users/search?page={queryRequest.PageNumber}&size={queryRequest.PageSize}";

                // 发送查询请求
                var result = await _clientService.PostAsync<UserQueryRequest, ApiResponse<List<User>>>(
                    clientName, endpoint, queryRequest, cancellationToken);

                _logger.LogInformation("User query completed - Returned {ItemCount} items, Total {TotalCount}",
                    result.Data?.Count ?? 0, result.TotalRecords);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying users");

                return new ApiResponse<List<User>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new List<User>(),
                    ErrorCode = ex is ApiValidationException ? 422 : 500
                };
            }
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        public async Task<ApiResponse<User>> GetUserAsync(
            string clientName,
            int userId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting user details for ID {UserId}", userId);

            try
            {
                var endpoint = $"/api/users/{userId}";
                var result = await _clientService.GetAsync<ApiResponse<User>>(
                    clientName, endpoint, cancellationToken);

                _logger.LogDebug("User details retrieved for {UserId}", userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);

                return new ApiResponse<User>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                    ErrorCode = ex is HttpJsonClientException clientEx ? (int)clientEx.StatusCode : 500
                };
            }
        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        public async Task<ApiResponse<User>> CreateUserAsync(
            string clientName,
            User newUser,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating new user {UserName}", newUser.UserName);

            try
            {
                var endpoint = "/api/users";
                var result = await _clientService.PostAsync<User, ApiResponse<User>>(
                    clientName, endpoint, newUser, cancellationToken);

                _logger.LogInformation("User created successfully - ID: {UserId}", result.Data?.Id);

                return result;
            }
            catch (ApiValidationException validationEx)
            {
                _logger.LogWarning("User creation validation failed: {ValidationErrors}",
                    string.Join(", ", validationEx.ValidationErrors.Values));

                return new ApiResponse<User>
                {
                    Success = false,
                    Message = validationEx.Message,
                    Data = null,
                    ErrorCode = 422
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", newUser.UserName);

                return new ApiResponse<User>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                    ErrorCode = ex is HttpJsonClientException clientEx ? (int)clientEx.StatusCode : 500
                };
            }
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        public async Task<ApiResponse<User>> UpdateUserAsync(
            string clientName,
            int userId,
            User updatedUser,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating user {UserId} - {UserName}", userId, updatedUser.UserName);

            try
            {
                var endpoint = $"/api/users/{userId}";
                var result = await _clientService.PutAsync<User, ApiResponse<User>>(
                    clientName, endpoint, updatedUser, cancellationToken);

                _logger.LogInformation("User updated successfully - ID: {UserId}", userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);

                return new ApiResponse<User>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                    ErrorCode = ex is HttpJsonClientException clientEx ? (int)clientEx.StatusCode : 500
                };
            }
        }
    }

    // 6. 流式JSON处理服务 - 处理大容量JSON数据
    public class StreamingJsonHandler
    {
        private readonly ILogger<StreamingJsonHandler> _logger;
        private readonly JsonSerializerOptions _options;

        public StreamingJsonHandler(ILogger<StreamingJsonHandler> logger)
        {
            _logger = logger;
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }

        /// <summary>
        /// 流式处理批量用户数据
        /// </summary>
        public async Task<List<User>> StreamUsersFromAsync<T>(
            HttpContent content,
            Func<User, Task<T>> itemProcessor,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting streaming JSON deserialization");

            var processedResults = new List<T>();
            var users = new List<User>();

            try
            {
                // 流式反序列化大型JSON数组
                await using var stream = await content.ReadAsStreamAsync(cancellationToken);
                using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    try
                    {
                        var user = JsonSerializer.Deserialize<User>(element, _options);
                        if (user != null)
                        {
                            users.Add(user);
                            var result = await itemProcessor(user);
                            processedResults.Add(result);
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogWarning(jsonEx, "Failed to deserialize user from JSON element");
                    }
                }

                _logger.LogInformation("Streaming JSON deserialization completed - Processed {ItemCount} items",
                    users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during JSON streaming processing");
                throw new HttpJsonClientException("JSON streaming processing failed", ex);
            }
        }
    }

    // 7. API客户端门面服务 - 简化常见API操作
    public class ApiClientService
    {
        private readonly ApiServiceQueryHandler _queryHandler;
        private readonly HttpJsonClientService _httpService;
        private readonly ILogger<ApiClientService> _logger;
        private readonly string _clientName;

        public ApiClientService(
            ApiServiceQueryHandler queryHandler,
            HttpJsonClientService httpService,
            ILogger<ApiClientService> logger,
            string clientName = "Default")
        {
            _queryHandler = queryHandler;
            _httpService = httpService;
            _logger = logger;
            _clientName = clientName;
        }

        /// <summary>
        /// 获取所有用户列表
        /// </summary>
        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all users");

            try
            {
                var users = await _httpService.GetListAsync<User>(
                    _clientName, "/api/users", cancellationToken);

                _logger.LogInformation("Retrieved {UserCount} users successfully", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users list");
                throw new HttpJsonClientException("Failed to retrieve users", ex);
            }
        }

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        public async Task<User> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving user by ID: {UserId}", userId);

            try
            {
                var result = await _queryHandler.GetUserAsync(_clientName, userId, cancellationToken);

                if (!result.Success)
                {
                    throw new HttpJsonClientException($"Failed to get user: {result.Message}",
                        HttpStatusCode.BadRequest, result.Message);
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Creating user: {UserName}", user.UserName);

            try
            {
                var result = await _queryHandler.CreateUserAsync(_clientName, user, cancellationToken);

                if (!result.Success)
                {
                    throw new HttpJsonClientException($"Failed to create user: {result.Message}",
                        HttpStatusCode.BadRequest, result.Message);
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserName}", user.UserName);
                throw;
            }
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        public async Task<User> UpdateUserAsync(int userId, User user, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating user ID: {UserId} - {UserName}", userId, user.UserName);

            try
            {
                var result = await _queryHandler.UpdateUserAsync(_clientName, userId, user, cancellationToken);

                if (!result.Success)
                {
                    throw new HttpJsonClientException($"Failed to update user: {result.Message}",
                        HttpStatusCode.BadRequest, result.Message);
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deleting user: {UserId}", userId);

            try
            {
                var result = await _httpService.DeleteAsync(
                    _clientName, $"/api/users/{userId}", cancellationToken);

                _logger.LogInformation("User deletion result: {Success}", result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                throw new HttpJsonClientException("Failed to delete user", ex);
            }
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        public async Task<ApiResponse<List<User>>> QueryUsersAsync(
            UserQueryRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Querying users with search term: {SearchTerm}", request.SearchTerm);

            return await _queryHandler.QueryUsersAsync(_clientName, request, cancellationToken);
        }
    }

    // 8. HTTP JSON性能监控服务
    public class HttpJsonTelemetryService
    {
        private readonly ILogger<HttpJsonTelemetryService> _logger;
        private readonly Dictionary<string, ApiEndpointMetrics> _endpointMetrics =
            new Dictionary<string, ApiEndpointMetrics>();

        public async Task TrackApiCallAsync(string endpoint, TimeSpan duration, bool success)
        {
            var metrics = GetOrAddMetrics(endpoint);

            metrics.TotalCalls++;
            metrics.TotalDuration += duration.TotalMilliseconds;

            if (success)
            {
                metrics.SuccessfulCalls++;
            }
            else
            {
                metrics.FailedCalls++;
            }

            metrics.LastCallTime = DateTime.UtcNow;
            metrics.AverageDuration = metrics.TotalDuration / metrics.TotalCalls;

            _logger.LogDebug("TRACKING - Endpoint {Endpoint}: Duration {Duration}ms, Success {Success}",
                endpoint, duration.TotalMilliseconds, success);
        }

        public ApiPerformanceMetrics GetPerformanceMetrics()
        {
            return new ApiPerformanceMetrics
            {
                EndpointMetrics = new Dictionary<string, ApiEndpointMetrics>(_endpointMetrics),
                TotalApiCalls = _endpointMetrics.Values.Sum(m => m.TotalCalls),
                SuccessfulApiCalls = _endpointMetrics.Values.Sum(m => m.SuccessfulCalls),
                FailedApiCalls = _endpointMetrics.Values.Sum(m => m.FailedCalls),
                RetrievedAt = DateTime.UtcNow
            };
        }

        public void ResetMetrics(string endpoint = null)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                _endpointMetrics.Clear();
                _logger.LogInformation("All HTTP JSON telemetry metrics reset");
            }
            else
            {
                _endpointMetrics.Remove(endpoint);
                _logger.LogInformation("Endpoint metrics reset for {Endpoint}", endpoint);
            }
        }

        private ApiEndpointMetrics GetOrAddMetrics(string endpoint)
        {
            if (!_endpointMetrics.TryGetValue(endpoint, out var metrics))
            {
                metrics = new ApiEndpointMetrics { Endpoint = endpoint };
                _endpointMetrics[endpoint] = metrics;
            }

            return metrics;
        }
    }

    public class ApiPerformanceMetrics
    {
        public Dictionary<string, ApiEndpointMetrics> EndpointMetrics { get; set; } = new Dictionary<string, ApiEndpointMetrics>();
        public long TotalApiCalls { get; set; }
        public long SuccessfulApiCalls { get; set; }
        public long FailedApiCalls { get; set; }
        public DateTime RetrievedAt { get; set; }

        public double SuccessRate => TotalApiCalls > 0 ?
            (double)SuccessfulApiCalls / TotalApiCalls * 100 : 0;

        public Dictionary<string, double> AverageDurations =>
            EndpointMetrics.ToDictionary(k => k.Key, v => v.Value.AverageDuration);
    }

    public class ApiEndpointMetrics
    {
        public string Endpoint { get; set; }
        public long TotalCalls { get; set; }
        public long SuccessfulCalls { get; set; }
        public long FailedCalls { get; set; }
        public double TotalDuration { get; set; }
        public double AverageDuration { get; set; }
        public DateTime LastCallTime { get; set; }
        public List<string> RecentErrors { get; set; } = new List<string>();

        public double CallRate => TotalCalls > 0 ?
            TotalCalls / (DateTime.UtcNow - LastCallTime).TotalSeconds : 0;
    }

    // 9. 生产级HTTP JSON中间件包装器
    public class HttpJsonMiddleware
    {
        private readonly HttpJsonClientService _clientService;
        private readonly HttpJsonTelemetryService _telemetryService;
        private readonly ILogger<HttpJsonMiddleware> _logger;
        private readonly SemaphoreSlim _retrySemaphore = new SemaphoreSlim(3, 3);

        public HttpJsonMiddleware(
            HttpJsonClientService clientService,
            HttpJsonTelemetryService telemetryService,
            ILogger<HttpJsonMiddleware> logger)
        {
            _clientService = clientService;
            _telemetryService = telemetryService;
            _logger = logger;
        }

        /// <summary>
        /// 带有重试机制的HTTP JSON调用
        /// </summary>
        public async Task<T> CallWithRetryAsync<T>(
            string clientName,
            string endpoint,
            Func<Task<T>> operation,
            int maxRetries = 3,
            CancellationToken cancellationToken = default)
        {
            await _retrySemaphore.WaitAsync(cancellationToken);

            try
            {
                for (int attempt = 0; attempt <= maxRetries; attempt++)
                {
                    var startTime = DateTime.UtcNow;

                    try
                    {
                        var result = await operation();
                        var duration = DateTime.UtcNow - startTime;

                        await _telemetryService.TrackApiCallAsync(endpoint, duration, true);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        var duration = DateTime.UtcNow - startTime;
                        await _telemetryService.TrackApiCallAsync(endpoint, duration, false);

                        if (attempt == maxRetries)
                        {
                            _logger.LogError(ex, "HTTP JSON operation failed after {MaxRetries} retries", maxRetries);
                            throw;
                        }

                        // 检查是否应该重试（根据HTTP状态码或其他条件）
                        if (!ShouldRetry(ex))
                        {
                            throw;
                        }

                        var delay = CalculateRetryDelay(attempt);
                        _logger.LogWarning(ex, "HTTP JSON operation failed (attempt {Attempt}), retrying in {Delay}ms",
                            attempt + 1, delay.TotalMilliseconds);

                        await Task.Delay(delay, cancellationToken);
                    }
                }

                throw new HttpJsonClientException("Operation failed - maximum retries exceeded",
                    HttpStatusCode.InternalServerError, null);
            }
            finally
            {
                _retrySemaphore.Release();
            }
        }

        /// <summary>
        /// 带时限的HTTP JSON调用
        /// </summary>
        public async Task<T> CallWithTimeoutAsync<T>(
            string clientName,
            string endpoint,
            Func<CancellationToken, Task<T>> operation,
            int timeoutSeconds = 30)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            try
            {
                var startTime = DateTime.UtcNow;
                var result = await operation(cts.Token);
                var duration = DateTime.UtcNow - startTime;

                await _telemetryService.TrackApiCallAsync(endpoint, duration, true);
                return result;
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                await _telemetryService.TrackApiCallAsync(endpoint, TimeSpan.FromSeconds(timeoutSeconds), false);
                throw new HttpJsonClientException($"Operation timeout after {timeoutSeconds} seconds",
                    HttpStatusCode.RequestTimeout, null);
            }
            catch (Exception ex)
            {
                await _telemetryService.TrackApiCallAsync(endpoint, TimeSpan.Zero, false);
                throw;
            }
        }

        /// <summary>
        /// 带缓存的HTTP JSON调用
        /// </summary>
        public async Task<T> CallWithCacheAsync<T>(
            string clientName,
            string endpoint,
            Func<CancellationToken, Task<T>> operation,
            TimeSpan cacheDuration,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{clientName}:{endpoint}";
            var cache = new Dictionary<string, CacheEntry<T>>();

            if (cache.TryGetValue(cacheKey, out var cachedEntry) &&
                DateTime.UtcNow - cachedEntry.Timestamp < cacheDuration)
            {
                _logger.LogDebug("Using cached response for {Endpoint}", endpoint);
                return cachedEntry.Value;
            }

            var result = await operation(cancellationToken);

            cache[cacheKey] = new CacheEntry<T>
            {
                Value = result,
                Timestamp = DateTime.UtcNow
            };

            return result;
        }

        private bool ShouldRetry(Exception ex)
        {
            return ex is HttpJsonClientException clientEx &&
                   (clientEx.StatusCode == HttpStatusCode.RequestTimeout ||
                    clientEx.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    clientEx.StatusCode == HttpStatusCode.GatewayTimeout ||
                    (int)clientEx.StatusCode >= 500);
        }

        private TimeSpan CalculateRetryDelay(int attempt)
        {
            // 指数退避策略
            return TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 1000);
        }
    }

    public class CacheEntry<T>
    {
        public T Value { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // 10. 主程序演示类
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            Console.WriteLine("System.Net.Http.Json Production Demo");
            Console.WriteLine("====================================");
            Console.WriteLine();

            #region 服务配置

            // 配置日志服务
            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Information);
            });

            // 配置HttpClient工厂
            builder.Services.AddHttpClient("Default", client =>
            {
                client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
                client.DefaultRequestHeaders.Add("User-Agent", "HttpJsonDemo/1.0");
            });

            builder.Services.AddHttpClient("SecureApi", client =>
            {
                client.BaseAddress = new Uri("https://secure.api.example.com/");
                client.DefaultRequestHeaders.Add("User-Agent", "SecureHttpJsonDemo/1.0");
            });

            // 注册核心服务
            builder.Services.AddSingleton<IHttpJsonClientFactory, HttpJsonClientFactory>();
            builder.Services.AddSingleton<HttpJsonClientService>();
            builder.Services.AddSingleton<ApiServiceQueryHandler>();
            builder.Services.AddSingleton<StreamingJsonHandler>();
            builder.Services.AddSingleton<ApiClientService>();
            builder.Services.AddSingleton<HttpJsonTelemetryService>();
            builder.Services.AddSingleton<HttpJsonMiddleware>();

            #endregion

            var host = builder.Build();

            await RunDemoAsync(host.Services);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunDemoAsync(IServiceProvider services)
        {
            var apiClient = services.GetRequiredService<ApiClientService>();
            var httpService = services.GetRequiredService<HttpJsonClientService>();
            var telemetryService = services.GetRequiredService<HttpJsonTelemetryService>();
            var streamingHandler = services.GetRequiredService<StreamingJsonHandler>();
            var middleware = services.GetRequiredService<HttpJsonMiddleware>();

            Console.WriteLine("1. Basic Http.Json GET Demo:");
            try
            {
                // 使用System.Net.Http.Json获取用户数据
                var users = await httpService.GetListAsync<User>("Default", "/users");
                Console.WriteLine($"   Retrieved {users.Count} users");

                foreach (var user in users.Take(3))
                {
                    Console.WriteLine($"   - {user.DisplayName} ({user.Email})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n2. Http.Json POST Demo:");
            try
            {
                var newUser = CreateSampleUser();

                // 构造响应包装器模型用于演示
                var postResult = await httpService.PostAsync<User, object>(
                    "Default", "/users", newUser);

                Console.WriteLine($"   POST completed successfully with response: {postResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n3. Http.Json PUT Demo:");
            try
            {
                var updatedUser = CreateSampleUser();
                updatedUser.Id = 1; // 更新ID=1的用户
                updatedUser.FirstName = "Updated Name";

                var putResult = await httpService.PutAsync<User, object>(
                    "Default", "/users/1", updatedUser);

                Console.WriteLine($"   PUT completed successfully with response: {putResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n4. Http.Json DELETE Demo:");
            try
            {
                var deleteResult = await httpService.DeleteAsync("Default", "/users/1");
                Console.WriteLine($"   DELETE result: {deleteResult}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n5. Client Service API Operations Demo:");
            try
            {
                var allUsers = await apiClient.GetAllUsersAsync();
                Console.WriteLine($"   Get all users count: {allUsers.Count}");

                var user = await apiClient.GetUserByIdAsync(1);
                Console.WriteLine($"   Get user by ID: {user?.DisplayName}");

                var createdUser = await apiClient.CreateUserAsync(CreateSampleUser());
                Console.WriteLine($"   Created user ID: {createdUser?.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n6. Streaming JSON Processing Demo:");
            try
            {
                var client = services.GetRequiredService<IHttpClientFactory>().CreateClient("Default");
                var response = await client.GetAsync("/users");

                if (response.IsSuccessStatusCode)
                {
                    var processedUserCount = 0;

                    var users = await streamingHandler.StreamUsersFromAsync(
                        response.Content,
                        async user =>
                        {
                            await Task.Delay(1); // 模拟处理延迟
                            processedUserCount++;
                            return user.Id;
                        });

                    Console.WriteLine($"   Streamed and processed {users.Count} users");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n7. Retry Middleware Demo:");
            try
            {
                var retryResult = await middleware.CallWithRetryAsync(
                    "Default",
                    "/posts",
                    () => httpService.GetAsync<List<object>>("Default", "/posts"));

                Console.WriteLine($"   Retry operation completed successfully - Result count: {retryResult?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n8. Timeout Middleware Demo:");
            try
            {
                var timeoutResult = await middleware.CallWithTimeoutAsync(
                    "Default",
                    "/posts",
                    ct => httpService.GetAsync<List<object>>("Default", "/posts", ct, 10));

                Console.WriteLine($"   Timeout operation completed with timeout applied");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message} (This is expected in timeout demo)");
            }

            Console.WriteLine("\n9. Performance Telemetry Demo:");
            try
            {
                var metrics = telemetryService.GetPerformanceMetrics();
                Console.WriteLine($"   Total API calls tracked: {metrics.TotalApiCalls}");
                Console.WriteLine($"   Successful API calls: {metrics.SuccessfulApiCalls}");
                Console.WriteLine($"   Success rate: {metrics.SuccessRate:F2}%");

                foreach (var endpointMetric in metrics.EndpointMetrics.Take(3))
                {
                    Console.WriteLine($"   - {endpointMetric.Key}: {endpointMetric.Value.TotalCalls} calls, " +
                                    $"avg {endpointMetric.Value.AverageDuration:F2}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Error: {ex.Message}");
            }

            Console.WriteLine("\n10. Secure HTTP Client Demo:");
            try
            {
                var clientFactory = services.GetRequiredService<IHttpJsonClientFactory>();
                var secureClient = await clientFactory.CreateSecureClientAsync("SecureApi", "sample-token");

                Console.WriteLine($"   Secure client created with base address: {secureClient.BaseAddress}");
                Console.WriteLine($"   Authorization header set: {secureClient.DefaultRequestHeaders.Authorization != null}");

                // 实际请求由于是模拟URL所以会失败，这里演示配置
                Console.WriteLine("   Secure client configuration demonstration completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Expected error for secure client demo: {ex.Message}");
            }
        }

        private static User CreateSampleUser()
        {
            return new User
            {
                UserName = $"user_{Guid.NewGuid():N}",
                Email = "sample@example.com",
                FirstName = "Sample",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Roles = new List<string> { "User", "Tester" },
                Metadata = new Dictionary<string, object>
            {
                { "source", "demo" },
                { "version", "1.0" }
            }
            };
        }
    }


    public class Test
    {
        public class TestJson() 
        {
            // HttpContentJsonExtensions 核心扩展
            // 异步读取JSON内容
            // 从HttpContent异步反序列化对象
            var users = await httpResponse.Content.ReadFromJsonAsync<List<User>>(
                options: jsonSerializerOptions,
                cancellationToken: cancellationToken);

            // 从HttpContent读取异步流式处理 (仅在.NET 6+可用)
            var stream = await httpResponse.Content.ReadFromJsonAsync<UserStream>();

            // HttpClientJsonExtensions 核心扩展
            // 发送JSON内容的便捷方法
            // POST JSON请求
            var postResponse = await httpClient.PostAsJsonAsync(
                "/api/users",  // 端点
                user,          // 请求对象
                options: jsonOptions,  // 序列化选项
                cancellationToken: ct); // 取消令牌

            // PUT JSON请求
            var putResponse = await httpClient.PutAsJsonAsync(
                "/api/users/123",
                updatedUser,
                options: jsonOptions,
                cancellationToken: ct);

            // 配置化客户端创建
            // 使用IHttpClientFactory的命名客户端
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);

            // 高级序列化选项配置
            // 生产级序列化选项
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,     // 驼峰命名转换
                WriteIndented = false,                                // 不格式化减小体积
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // 忽略null值
                PropertyNameCaseInsensitive = true,                   // 属性名大小写不敏感
                Converters = { new JsonStringEnumConverter() }       // 枚举字符串转换
            };

            // 错误处理和恢复
            // 生产级HTTP JSON异常处理
            try
            {
                var response = await httpClient.GetAsync("/api/users");
                await HandleHttpResponseAsync(response, "GET", "/api/users");

                var users = await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (OperationCanceledException) when(cancellationToken.IsCancellationRequested)
            {
                // 处理用户取消
            }
            catch (TaskCanceledException)
            {
                // 处理HTTP超时
            }
            catch (JsonException)
            {
                // 处理JSON解析错误
            }
            catch (HttpRequestException)
            {
                // 处理网络错误
            }

            // 性能优化和监控
            // 并发控制和速率限制
            private readonly SemaphoreSlim _rateLimitSemaphore =
            new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);

            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                // 执行HTTP请求
                await httpClient.GetAsync(endpoint, cancellationToken);
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }

            // 生产环境推荐实践
            // 内存效率处理
            // 避免将大型响应加载到内存
            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/large-data");
            var httpResponse = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

            // 逐步处理响应流
            await using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            using var jsonDocument = await JsonDocument.ParseAsync(responseStream);

            // 取消令牌正确使用
            // 外部控制的取消令牌
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            var users = await httpService.GetAsync<List<User>>(
                clientName, endpoint, cts.Token, timeoutSeconds: 30);

            // 在长时间操作中定期检查取消状态
            if (cancellationToken.IsCancellationRequested) { throw new OperationCanceledException(cancellationToken); }

            // 重试和错误恢复策略
            // 指数退避重试
            private TimeSpan CalculateRetryDelay(int attempt)
            {
                return TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * baseDelayMs);
            }

            // 条件重试判断
            private bool ShouldRetry(HttpResponseMessage response)
            {
                return response.StatusCode == HttpStatusCode.RequestTimeout ||
                       response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                       response.StatusCode >= HttpStatusCode.InternalServerError;
            }

            // 安全性考虑
            // 验证SSL证书
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    // 自定义证书验证逻辑
                    return errors == SslPolicyErrors.None;
                }
            };

            // 限制重定向
            handler.AllowAutoRedirect = false;

            // 设置超时避免拒绝服务
            client.Timeout = TimeSpan.FromSeconds(30);

            // 分页结果处理
            var response = await httpService.PostAsync<QueryRequest, ApiResponse<List<User>>>(
            clientName, "/api/users/search", queryRequest);

            Console.WriteLine($"Total records: {response.TotalRecords}");
            Console.WriteLine($"Current page: {response.PageNumber}");
        }

        // 流式JSON处理 (适用于大型响应)
        // 大型JSON数组流式处理
        public async Task<List<T>> StreamJsonArrayAsync<T>(
        HttpContent content,
        Func<JsonElement, T> elementConverter,
        CancellationToken cancellationToken = default)
        {
            await using var stream = await content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var results = new List<T>();

            foreach (var element in document.RootElement.EnumerateArray())
            {
                var item = elementConverter(element);
                results.Add(item);
            }

            return results;
        }
    }

    // 生产级HTTP客户端设计模式
    // 扩展工厂模式包
    public interface IHttpJsonClientFactory
    {
        Task<HttpClient> CreateClientAsync(string clientName);
        Task<HttpClient> CreateSecureClientAsync(string clientName, string accessToken);
    }

    // 性能跟踪中间
    public class HttpJsonMiddleware
    {
        public async Task<T> CallWithRetryAsync<T>(...);
        public async Task<T> CallWithTimeoutAsync<T>(...);
        public async Task<T> CallWithCacheAsync<T>(...);
    }

    // API一致性处理
    // 标准API包装器
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int ErrorCode { get; set; }
    }
}