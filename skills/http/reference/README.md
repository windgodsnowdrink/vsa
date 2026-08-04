# http - 参考文档

## 概述

HTTP 技能是一个基于 .NET 10 的高性能 HTTP 客户端和服务器解决方案，专为 .NET 开发者设计，提供了全面的 HTTP 功能，包括 HTTP 客户端、HTTP 服务器、HTTP/3、QUIC、HTTP 弹性、HTTP 报告、安全头、Webhook、AntiXSS、数据保护等。

## 核心组件

### 1. HTTP 客户端 (IHttpClientFactory)
- **位置**: .NET 10 核心库
- **功能**: 提供高性能、可配置的 HTTP 客户端，支持 HTTP/1.1、HTTP/2 和 HTTP/3
- **特性**: 
  - 支持连接池管理
  - 支持 HTTP 弹性策略
  - 支持 HTTP 日志记录
  - 支持 HTTP 诊断
  - 支持 HTTP 拦截器

### 2. HTTP 弹性 (IHttpClientBuilder)
- **位置**: scripts/http_resilience_integration.cs, scripts/http_resilience_advanced_integration.cs
- **功能**: 为 HTTP 客户端提供弹性策略，包括重试、断路器、超时和舱壁隔离
- **特性**: 
  - 支持可配置的重试策略
  - 支持断路器模式
  - 支持超时配置
  - 支持舱壁隔离
  - 支持自定义弹性策略

### 3. HTTP/3 和 QUIC
- **位置**: scripts/http3_quic.cs
- **功能**: 支持 HTTP/3 和 QUIC 协议，提供更快的连接建立和数据传输
- **特性**: 
  - 支持 QUIC 协议
  - 支持 HTTP/3 协议
  - 支持多路复用
  - 支持 0-RTT 连接建立
  - 支持连接迁移

### 4. HTTP 报告
- **位置**: scripts/httpreports_integration.cs, scripts/httpreports_prometheus.cs, scripts/httpreports_opentelemetry.cs, scripts/httpreports_alerts.cs
- **功能**: 提供 HTTP 请求和响应的报告功能，支持 Prometheus、OpenTelemetry 和告警
- **特性**: 
  - 支持请求和响应的指标收集
  - 支持 Prometheus 指标导出
  - 支持 OpenTelemetry 集成
  - 支持告警配置
  - 支持报告可视化

### 5. 安全头
- **位置**: scripts/securityheaders_integration.cs
- **功能**: 提供安全头配置和管理功能，增强应用程序的安全性
- **特性**: 
  - 支持多种安全头配置
  - 支持自定义安全头
  - 支持安全头策略
  - 支持安全头报告

### 6. AntiXSS
- **位置**: scripts/antixss_integration.cs
- **功能**: 提供跨站点脚本攻击防护
- **特性**: 
  - 支持输入验证
  - 支持输出编码
  - 支持 Content-Security-Policy
  - 支持 XSS 过滤

### 7. WebApiClientCore
- **位置**: scripts/webapiclientcore_integration.cs, scripts/webapiclientcore_aot.cs, scripts/webapiclientcore_dynamicproxy.cs 等
- **功能**: 基于 AOT 编译的高性能 HTTP 客户端库
- **特性**: 
  - 支持 AOT 编译
  - 支持源生成器
  - 支持动态代理
  - 支持弹性策略
  - 支持 circuit breaker
  - 支持 tracing

### 8. Webhook
- **位置**: scripts/webhook_integration.cs
- **功能**: 提供 Webhook 集成功能
- **特性**: 
  - 支持 Webhook 接收
  - 支持 Webhook 发送
  - 支持 Webhook 验证
  - 支持 Webhook 重试

### 9. HTTP REPL
- **位置**: scripts/httprepl_integration.cs, scripts/httprepl_mesh.cs, scripts/httprepl_openapi.cs, scripts/httprepl_tracing.cs
- **功能**: 提供 HTTP REPL 工具，用于交互式测试 HTTP API
- **特性**: 
  - 支持交互式 HTTP 请求
  - 支持 API 发现
  - 支持 OpenAPI 集成
  - 支持 tracing
  - 支持 mesh 集成

### 10. 数据保护
- **位置**: scripts/dataprotection_integration.cs
- **功能**: 提供数据加密和保护功能
- **特性**: 
  - 支持数据加密
  - 支持数据解密
  - 支持数据签名
  - 支持数据验证

### 11. HTTP AOT 核心引擎
- **位置**: scripts/http_aot.cs, scripts/http_aot.setting.json, scripts/http_aot.run.json
- **功能**: 基于 AOT 编译的高性能 HTTP 客户端核心引擎，支持多 HTTP 方法、弹性策略、基准测试等功能
- **特性**: 
  - 支持多种 HTTP 方法（GET、POST、PUT、DELETE、HEAD、OPTIONS）
  - 内置弹性策略（重试、断路器、超时）
  - 支持基准测试功能
  - 支持代理测试
  - 支持缓存机制
  - 支持压缩处理
  - 提供命令行界面和命令别名
  - 详细的错误处理和日志记录

## 使用示例

### 基本 HTTP 客户端使用示例

```csharp
// 创建依赖注入容器
var services = new ServiceCollection();

// 配置 HTTP 客户端
services.AddHttpClient("Default", client => {
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var serviceProvider = services.BuildServiceProvider();

// 获取 HTTP 客户端
var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("Default");

// 发送 GET 请求
var response = await httpClient.GetAsync("/api/resource");
if (response.IsSuccessStatusCode)
{
    var data = await response.Content.ReadFromJsonAsync<Resource>();
    Console.WriteLine($"获取资源成功: {data.Name}");
}

// 发送 POST 请求
var newResource = new Resource { Name = "New Resource" };
var postResponse = await httpClient.PostAsJsonAsync("/api/resource", newResource);
if (postResponse.IsSuccessStatusCode)
{
    Console.WriteLine("创建资源成功");
}
```

### HTTP 弹性使用示例

```csharp
// 配置带弹性策略的 HTTP 客户端
services.AddHttpClient("Resilient")
    .AddStandardResilienceHandler(options => {
        // 配置重试
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromMilliseconds(100);
        options.Retry.MaxDelay = TimeSpan.FromSeconds(10);
        options.Retry.UseJitter = true;
        
        // 配置断路器
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(60);
        options.CircuitBreaker.MinimumThroughput = 10;
        
        // 配置超时
        options.Timeout.Timeout = TimeSpan.FromSeconds(30);
        
        // 配置舱壁隔离
        options.Bulkhead.MaxConcurrency = 100;
        options.Bulkhead.MaxQueuedItems = 1000;
    });

// 使用带弹性策略的 HTTP 客户端
var resilientHttpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("Resilient");
var response = await resilientHttpClient.GetAsync("/api/resource");
```

### WebApiClientCore 使用示例

```csharp
// 定义 HTTP API 接口
[HttpApi("https://api.example.com")]
public interface IUserApi
{
    [Get("/api/users")]
    Task<List<User>> GetUsersAsync();
    
    [Get("/api/users/{id}")]
    Task<User> GetUserAsync([Path] int id);
    
    [Post("/api/users")]
    Task CreateUserAsync([JsonContent] User user);
    
    [Put("/api/users/{id}")]
    Task UpdateUserAsync([Path] int id, [JsonContent] User user);
    
    [Delete("/api/users/{id}")]
    Task DeleteUserAsync([Path] int id);
}

// 注册 WebApiClientCore
services.AddHttpApi<IUserApi>();
services.AddHttpApiClient<IUserApi>();

// 使用 WebApiClientCore
var userApi = serviceProvider.GetRequiredService<IUserApi>();
var users = await userApi.GetUsersAsync();
```

### 安全头使用示例

```csharp
// 配置安全头
var builder = WebApplication.CreateBuilder(args);

// 注册安全头服务
builder.Services.AddSecurityHeaders(options => {
    options.StrictTransportSecurity = "max-age=31536000; includeSubDomains";
    options.XContentTypeOptions = "nosniff";
    options.XFrameOptions = "DENY";
    options.XssProtection = "1; mode=block";
    options.ReferrerPolicy = "strict-origin-when-cross-origin";
    options.PermissionsPolicy = "geolocation=(self)";
    options.ContentSecurityPolicy = "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';";
});

var app = builder.Build();

// 使用安全头中间件
app.UseSecurityHeaders();

app.Run();
```

### HTTP AOT 核心引擎使用示例

```bash
# 发送 GET 请求
http_aot get https://example.com

# 发送 POST 请求
http_aot post https://example.com {"name": "test", "value": 123}

# 运行基准测试
http_aot benchmark https://example.com 100

# 测试代理
http_aot proxy https://example.com http://localhost:8080

# 测试超时
http_aot timeout https://example.com 1

# 测试断路器
http_aot circuit https://example.com

# 显示配置
http_aot config

# 显示帮助
http_aot help
```

### HTTP AOT 核心引擎代码示例

```csharp
// HTTP AOT 核心引擎使用示例
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        // 创建依赖注入容器
        var serviceProvider = BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<HttpService>();
        
        try
        {
            // 发送 GET 请求
            var getResult = await httpService.SendGetRequestAsync("https://example.com");
            Console.WriteLine($"GET 请求结果: {getResult.StatusCode}");
            
            // 发送 POST 请求
            var postResult = await httpService.SendPostRequestAsync("https://example.com", "{\"name\": \"test\", \"value\": 123}");
            Console.WriteLine($"POST 请求结果: {postResult.StatusCode}");
            
            // 发送 PUT 请求
            var putResult = await httpService.SendPutRequestAsync("https://example.com", "{\"name\": \"test\", \"value\": 456}");
            Console.WriteLine($"PUT 请求结果: {putResult.StatusCode}");
            
            // 发送 DELETE 请求
            var deleteResult = await httpService.SendDeleteRequestAsync("https://example.com");
            Console.WriteLine($"DELETE 请求结果: {deleteResult.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            await httpService.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        services.AddSingleton<HttpService>();
        return services.BuildServiceProvider();
    }
}
```

## 配置选项

### HttpClientSettings 配置

```json
{
  "HttpClientSettings": {
    "BaseAddress": "https://api.example.com",
    "Timeout": "00:00:30",
    "DefaultRequestHeaders": {
      "User-Agent": "YourApp/1.0",
      "Accept": "application/json"
    },
    "Resilience": {
      "Retry": {
        "MaxRetryAttempts": 3,
        "Delay": "00:00:01",
        "MaxDelay": "00:00:10",
        "UseJitter": true
      },
      "CircuitBreaker": {
        "SamplingDuration": "00:00:30",
        "FailureRatio": 0.5,
        "BreakDuration": "00:01:00",
        "MinimumThroughput": 10
      },
      "Timeout": {
        "Timeout": "00:00:30"
      },
      "Bulkhead": {
        "MaxConcurrency": 100,
        "MaxQueuedItems": 1000
      }
    }
  }
}
```

### SecurityHeadersSettings 配置

```json
{
  "SecurityHeadersSettings": {
    "StrictTransportSecurity": "max-age=31536000; includeSubDomains",
    "XContentTypeOptions": "nosniff",
    "XFrameOptions": "DENY",
    "XssProtection": "1; mode=block",
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "PermissionsPolicy": "geolocation=(self); camera=(); microphone=()",
    "ContentSecurityPolicy": "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self'; object-src 'none'; frame-ancestors 'none'; base-uri 'self'; form-action 'self';"
  }
}
```

### HttpReportsSettings 配置

```json
{
  "HttpReportsSettings": {
    "EnableMetrics": true,
    "EnableTracing": true,
    "EnableAlerts": true,
    "Metrics": {
      "EnableRequestMetrics": true,
      "EnableResponseMetrics": true,
      "EnableErrorMetrics": true
    },
    "Tracing": {
      "SamplingRate": 1.0,
      "EnableActivitySource": true
    },
    "Alerts": {
      "ErrorRateThreshold": 0.05,
      "ResponseTimeThresholdMs": 1000,
      "RequestVolumeThreshold": 1000
    }
  }
}
```

## 性能优化

### HTTP 客户端性能优化

1. **使用 HttpClientFactory**: 使用 IHttpClientFactory 管理 HTTP 客户端，避免连接泄漏和资源耗尽
2. **配置适当的连接池大小**: 根据应用程序的负载情况，配置适当的连接池大小
3. **使用 HTTP/3 和 QUIC**: 对于支持 HTTP/3 的环境，使用 HTTP/3 和 QUIC 协议
4. **优化 HTTP 请求**: 减少请求大小，使用适当的 HTTP 方法，使用压缩
5. **使用缓存**: 适当使用缓存，减少重复请求
6. **使用批量处理**: 对于大量小请求，考虑使用批量处理减少开销
7. **配置适当的超时**: 为 HTTP 请求配置适当的超时时间，避免长时间阻塞
8. **使用异步编程**: 优先使用异步 API 进行 HTTP 操作，避免阻塞主线程
9. **使用源生成器**: 启用 WebApiClientCore 等库的源生成器，减少运行时反射开销
10. **使用 AOT 编译**: 将 HTTP 应用编译为本机代码，提高启动速度和运行性能

### HTTP 服务器性能优化

1. **使用 ASP.NET Core 10**: 使用最新的 ASP.NET Core 10，享受性能改进
2. **启用 HTTP/3 和 QUIC**: 为 HTTP 服务器启用 HTTP/3 和 QUIC 支持
3. **优化中间件**: 只使用必要的中间件，避免不必要的请求处理
4. **使用响应压缩**: 启用响应压缩，减少网络传输量
5. **使用缓存**: 启用输出缓存，减少重复计算
6. **优化序列化**: 使用高效的序列化器，如 MessagePack 或 Protobuf
7. **配置适当的服务器设置**: 配置适当的服务器设置，如最大连接数、请求队列大小等
8. **使用 AOT 编译**: 将 HTTP 服务器应用编译为本机代码，提高启动速度和运行性能

## 故障排除

### 常见问题及解决方案

1. **HTTP 请求失败**
   - 检查请求 URL 是否正确
   - 检查 HTTP 方法是否正确
   - 检查请求头是否正确
   - 检查请求体是否正确
   - 检查 HTTP 状态码，了解请求失败的原因

2. **连接超时**
   - 检查网络连接是否正常
   - 检查服务器是否可达
   - 检查服务器是否正常运行
   - 调整超时设置

3. **认证失败**
   - 检查认证信息是否正确
   - 检查认证方式是否正确
   - 检查认证令牌是否过期

4. **序列化/反序列化错误**
   - 检查请求和响应的数据格式是否正确
   - 检查序列化器配置是否正确
   - 检查数据类型是否匹配

5. **安全头错误**
   - 检查安全头配置是否正确
   - 检查安全头是否与应用程序冲突
   - 检查浏览器控制台的错误信息

6. **HTTP 弹性策略触发**
   - 检查弹性策略配置是否合理
   - 检查服务器是否稳定
   - 检查网络连接是否稳定
   - 调整弹性策略设置

### 调试建议

1. **启用详细日志**: 配置 HTTP 客户端和服务器的详细日志记录，便于调试
   ```csharp
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Debug);
   ```

2. **使用网络调试工具**: 使用 Fiddler、Wireshark 等工具，分析 HTTP 请求和响应

3. **使用 HTTP REPL**: 使用 HTTP REPL 工具，交互式测试 HTTP API
   ```bash
   dotnet httprepl https://api.example.com
   ```

4. **检查 HTTP 状态码**: 检查 HTTP 响应的状态码，了解请求失败的原因

5. **检查服务器日志**: 检查服务器日志，了解服务器端的错误信息

6. **使用 OpenTelemetry**: 使用 OpenTelemetry 跟踪 HTTP 请求，了解请求的完整生命周期
   ```csharp
   builder.Services.AddOpenTelemetry()
       .WithTracing(tracerProviderBuilder => {
           tracerProviderBuilder.AddSource("System.Net.Http")
               .AddAspNetCoreInstrumentation()
               .AddHttpClientInstrumentation()
               .AddConsoleExporter();
       });
   ```

7. **使用 Prometheus 和 Grafana**: 使用 Prometheus 收集 HTTP 指标，使用 Grafana 可视化监控数据

## 扩展开发

### 实现自定义 HTTP 客户端拦截器

```csharp
public class CustomHttpClientHandler : DelegatingHandler
{
    private readonly ILogger<CustomHttpClientHandler> _logger;
    
    public CustomHttpClientHandler(ILogger<CustomHttpClientHandler> logger)
    {
        _logger = logger;
        // 设置内部处理程序
        InnerHandler = new SocketsHttpHandler();
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 发送请求前的处理
        _logger.LogInformation("发送 HTTP 请求: {Method} {Uri}", request.Method, request.RequestUri);
        
        // 添加自定义请求头
        request.Headers.Add("X-Custom-Header", "CustomValue");
        
        // 记录请求时间
        var startTime = DateTime.UtcNow;
        
        try
        {
            // 发送请求
            var response = await base.SendAsync(request, cancellationToken);
            
            // 发送请求后的处理
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("收到 HTTP 响应: {StatusCode} {Uri}, 耗时: {Duration}ms", 
                response.StatusCode, request.RequestUri, duration.TotalMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            // 处理请求异常
            var duration = DateTime.UtcNow - startTime;
            _logger.LogError(ex, "HTTP 请求失败: {Method} {Uri}, 耗时: {Duration}ms", 
                request.Method, request.RequestUri, duration.TotalMilliseconds);
            
            throw;
        }
    }
}

// 注册自定义 HTTP 客户端处理程序
services.AddTransient<CustomHttpClientHandler>();
services.AddHttpClient("Custom")
    .AddHttpMessageHandler<CustomHttpClientHandler>();
```

### 实现自定义 HTTP 弹性策略

```csharp
// 定义自定义弹性策略
services.AddHttpClient("CustomResilient")
    .AddResilienceHandler("CustomPolicy", options => {
        // 配置自定义重试策略
        options.Retry = new RetryStrategyOptions
        {
            MaxRetryAttempts = 5,
            Delay = TimeSpan.FromMilliseconds(500),
            MaxDelay = TimeSpan.FromSeconds(10),
            UseJitter = true,
            ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>().Handle<TimeoutException>()
        };
        
        // 配置自定义断路器策略
        options.CircuitBreaker = new CircuitBreakerStrategyOptions
        {
            SamplingDuration = TimeSpan.FromSeconds(60),
            FailureRatio = 0.3,
            BreakDuration = TimeSpan.FromSeconds(300),
            MinimumThroughput = 20,
            ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>().Handle<TimeoutException>()
        };
        
        // 配置自定义超时策略
        options.Timeout = new TimeoutStrategyOptions
        {
            Timeout = TimeSpan.FromSeconds(45)
        };
    });
```

### 实现自定义安全头

```csharp
// 实现自定义安全头中间件
public class CustomSecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomSecurityHeadersMiddleware> _logger;
    
    public CustomSecurityHeadersMiddleware(RequestDelegate next, ILogger<CustomSecurityHeadersMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        // 添加自定义安全头
        context.Response.Headers.Append("X-Custom-Security-Header", "CustomValue");
        
        // 添加 Content-Security-Policy
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline';");
        
        // 添加 Strict-Transport-Security
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        
        // 调用下一个中间件
        await _next(context);
    }
}

// 注册自定义安全头中间件
builder.Services.AddTransient<CustomSecurityHeadersMiddleware>();
app.UseMiddleware<CustomSecurityHeadersMiddleware>();
```

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>
```

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 编译注意事项

1. **使用 AOT 兼容的 HTTP 客户端库**: 优先使用 AOT 兼容的 HTTP 客户端库，如 WebApiClientCore
2. **避免使用反射**: 避免在运行时使用反射，或使用 Source Generator 替代
3. **避免动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
4. **资源加载**: 确保所有资源都能在 AOT 编译时被正确处理
5. **第三方库兼容性**: 确保使用的第三方库支持 AOT 编译
6. **测试验证**: 在 AOT 编译后进行充分的测试，确保应用正常运行
7. **序列化**: 使用 AOT 兼容的序列化库，如 MessagePack 或 Protobuf
8. **HTTP 客户端配置**: 避免在运行时动态配置 HTTP 客户端，使用编译时配置

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 HTTP 客户端
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("Resilient").AddStandardResilienceHandler();

// 配置 HTTP 报告
builder.Services.AddHttpReports();

// 配置安全头
builder.Services.AddSecurityHeaders();

var app = builder.Build();

// 使用 HTTP 报告中间件
app.UseHttpReports();

// 使用安全头中间件
app.UseSecurityHeaders();

// 定义 HTTP 端点
app.MapGet("/api/resource", async ([FromServices] IHttpClientFactory httpClientFactory) => {
    var httpClient = httpClientFactory.CreateClient("Resilient");
    var response = await httpClient.GetAsync("https://api.example.com/external-resource");
    var data = await response.Content.ReadFromJsonAsync<ExternalResource>();
    return Results.Ok(data);
});

app.Run();
```

### 与 WebApiClientCore 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 注册 WebApiClientCore
builder.Services.AddHttpApi<IUserApi>(o => {
    o.HttpHost = new Uri("https://api.example.com");
    o.UseTokenProvider<MyTokenProvider>();
});

builder.Services.AddHttpApiClient<IUserApi>(o => {
    o.UseResiliencePolicy();
    o.UseTracing();
});

var app = builder.Build();

// 在控制器中使用 WebApiClientCore
app.MapGet("/api/users", async ([FromServices] IUserApi userApi) => {
    var users = await userApi.GetUsersAsync();
    return Results.Ok(users);
});

app.Run();
```

### 与 Refit 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 Refit
builder.Services.AddRefitClient<IProductApi>(new RefitSettings {
    ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    })
}).ConfigureHttpClient(c => {
    c.BaseAddress = new Uri("https://api.example.com");
    c.Timeout = TimeSpan.FromSeconds(30);
}).AddStandardResilienceHandler(options => {
    options.Retry.MaxRetryAttempts = 3;
    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(60);
});

var app = builder.Build();

// 在控制器中使用 Refit
app.MapGet("/api/products", async ([FromServices] IProductApi productApi) => {
    var products = await productApi.GetProductsAsync();
    return Results.Ok(products);
});

app.Run();
```

### 与 RestSharp 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 RestSharp
builder.Services.AddSingleton<IRestClient>(sp => {
    var options = new RestClientOptions("https://api.example.com") {
        Timeout = TimeSpan.FromSeconds(30),
        ConfigureMessageHandler = _ => new HttpClientHandler {
            UseDefaultCredentials = true
        }
    };
    return new RestClient(options);
});

var app = builder.Build();

// 在控制器中使用 RestSharp
app.MapGet("/api/orders", async ([FromServices] IRestClient restClient) => {
    var request = new RestRequest("/orders", Method.Get);
    var response = await restClient.ExecuteAsync<List<Order>>(request);
    return Results.Ok(response.Data);
});

app.Run();
```

### 与 OpenTelemetry 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => {
        metrics.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter();
    })
    .WithTracing(tracing => {
        tracing.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    });

// 配置 HTTP 客户端
builder.Services.AddHttpClient();

var app = builder.Build();

// 配置 Prometheus 端点
app.MapPrometheusScrapingEndpoint();

// 定义 HTTP 端点
app.MapGet("/api/resource", async ([FromServices] IHttpClientFactory httpClientFactory) => {
    var httpClient = httpClientFactory.CreateClient();
    var response = await httpClient.GetAsync("https://api.example.com/external-resource");
    var data = await response.Content.ReadFromJsonAsync<ExternalResource>();
    return Results.Ok(data);
});

app.Run();
```

