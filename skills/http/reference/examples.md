# http - 使用示例

## 快速入门

### 1. 基本 HTTP 客户端使用示例

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("基本 HTTP 客户端使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置 HTTP 客户端
        services.AddHttpClient("Default", client => {
            client.BaseAddress = new Uri("https://api.example.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取 HTTP 客户端
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("Default");
        
        try
        {
            // 发送 GET 请求
            Console.WriteLine("发送 GET 请求...");
            var response = await httpClient.GetAsync("/api/resource");
            response.EnsureSuccessStatusCode();
            
            var data = await response.Content.ReadFromJsonAsync<Resource>();
            Console.WriteLine($"获取资源成功: {data.Name} (ID: {data.Id})");
            
            // 发送 POST 请求
            Console.WriteLine("\n发送 POST 请求...");
            var newResource = new Resource { Name = "新资源", Description = "这是一个新资源" };
            var postResponse = await httpClient.PostAsJsonAsync("/api/resource", newResource);
            postResponse.EnsureSuccessStatusCode();
            
            Console.WriteLine("创建资源成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"请求失败: {ex.Message}");
        }
    }
    
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
```

### 2. HTTP 弹性策略示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("HTTP 弹性策略示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置带弹性策略的 HTTP 客户端
        services.AddHttpClient("Resilient")
            .AddStandardResilienceHandler(options => {
                // 重试策略
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromMilliseconds(500);
                options.Retry.MaxDelay = TimeSpan.FromSeconds(10);
                options.Retry.UseJitter = true;
                
                // 断路器策略
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(60);
                options.CircuitBreaker.MinimumThroughput = 10;
                
                // 超时策略
                options.Timeout.Timeout = TimeSpan.FromSeconds(30);
                
                // 舱壁隔离
                options.Bulkhead.MaxConcurrency = 100;
                options.Bulkhead.MaxQueuedItems = 1000;
            });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取带弹性策略的 HTTP 客户端
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var resilientHttpClient = httpClientFactory.CreateClient("Resilient");
        
        try
        {
            Console.WriteLine("使用弹性 HTTP 客户端发送请求...");
            var response = await resilientHttpClient.GetAsync("https://api.example.com/fragile-resource");
            response.EnsureSuccessStatusCode();
            
            Console.WriteLine("请求成功 (弹性策略生效)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"请求失败: {ex.Message}");
            Console.WriteLine("弹性策略已尽力恢复，但请求仍失败");
        }
    }
}
```

### 3. WebApiClientCore AOT 支持示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package WebApiClientCore@3.1.0
#:package WebApiClientCore.Extensions.DependencyInjection@3.1.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiClientCore.Attributes;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("WebApiClientCore AOT 支持示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        
        // 注册 WebApiClientCore (AOT 兼容)
        services.AddHttpApi<IUserApi>(o => {
            o.HttpHost = new Uri("https://api.example.com");
        });
        services.AddHttpApiClient<IUserApi>();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 使用 WebApiClientCore
        var userApi = serviceProvider.GetRequiredService<IUserApi>();
        
        try
        {
            Console.WriteLine("获取用户列表...");
            var users = await userApi.GetUsersAsync();
            Console.WriteLine($"获取到 {users.Count} 个用户");
            
            foreach (var user in users)
            {
                Console.WriteLine($"- {user.Name} ({user.Email})");
            }
            
            // 获取单个用户
            Console.WriteLine("\n获取单个用户...");
            var user1 = await userApi.GetUserAsync(1);
            Console.WriteLine($"用户详情: {user1.Name} - {user1.Department}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"操作失败: {ex.Message}");
        }
    }
}

// 定义 HTTP API 接口 (WebApiClientCore 使用源生成器，AOT 兼容)
[HttpApi("https://api.example.com")]
public interface IUserApi
{
    [Get("/api/users")]
    Task<List<User>> GetUsersAsync();
    
    [Get("/api/users/{id}")]
    Task<User> GetUserAsync([Path] int id);
    
    [Post("/api/users")]
    Task CreateUserAsync([JsonContent] User user);
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
}
```

### 4. 安全头配置示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.AspNetCore.Http.Abstractions@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("安全头配置示例");
        Console.WriteLine("=" * 50);
        
        var builder = WebApplication.CreateBuilder();
        
        // 注册服务
        builder.Services.AddSingleton<SecurityHeadersMiddleware>();
        
        var app = builder.Build();
        
        // 使用安全头中间件
        app.UseMiddleware<SecurityHeadersMiddleware>();
        
        // 定义 API 端点
        app.MapGet("/", () => {
            return Results.Ok(new { Message = "Hello, World!", Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") });
        });
        
        app.MapGet("/api/secure", () => {
            return Results.Ok(new { Message = "This endpoint is protected with security headers" });
        });
        
        Console.WriteLine("启动 HTTP 服务器...");
        Console.WriteLine("访问 http://localhost:5000 查看安全头");
        app.Run("http://localhost:5000");
    }
}

// 自定义安全头中间件
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    
    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        // 添加安全头
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Permissions-Policy", "geolocation=(self); camera=(); microphone=()");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline';");
        
        await _next(context);
    }
}
```

### 5. HTTP/3 和 QUIC 支持示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("HTTP/3 和 QUIC 支持示例");
        Console.WriteLine("=" * 50);
        
        var builder = WebApplication.CreateBuilder();
        
        // 配置 Kestrel 支持 HTTP/3
        builder.WebHost.ConfigureKestrel(options => {
            options.ListenAnyIP(5000); // HTTP/1.1, HTTP/2
            options.ListenAnyIP(5001, listenOptions => {
                listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
                listenOptions.UseHttps(); // HTTP/3 需要 HTTPS
            });
        });
        
        var app = builder.Build();
        
        // 定义 API 端点
        app.MapGet("/", () => {
            return Results.Ok(new {
                Message = "Hello from HTTP/3 and QUIC!",
                Protocols = "HTTP/1.1, HTTP/2, HTTP/3",
                QUIC = "Supported"
            });
        });
        
        app.MapGet("/api/quic", () => {
            return Results.Ok(new {
                Message = "This endpoint is accessible via QUIC protocol",
                HTTPVersion = "HTTP/3"
            });
        });
        
        Console.WriteLine("启动 HTTP/3 服务器...");
        Console.WriteLine("HTTP/1.1, HTTP/2 端口: 5000");
        Console.WriteLine("HTTP/1.1, HTTP/2, HTTP/3 端口: 5001 (HTTPS)");
        app.Run();
    }
}
```

### 6. HTTP AOT 核心引擎示例

#### HTTP AOT 命令行使用示例

```bash
# 发送 GET 请求
http_aot get https://example.com

# 发送 POST 请求
http_aot post https://example.com {"name": "test", "value": 123}

# 发送 PUT 请求
http_aot put https://example.com {"name": "test", "value": 456}

# 发送 DELETE 请求
http_aot delete https://example.com

# 发送 HEAD 请求
http_aot head https://example.com

# 发送 OPTIONS 请求
http_aot options https://example.com

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

#### HTTP AOT 核心引擎代码示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Net.Http@4.3.4
#:package System.Net.Http.Json@8.0.0
#:package Polly@8.3.1
#:package Microsoft.Extensions.Http.Polly@10.0.0
#:package System.Text.Json@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("HTTP AOT 核心引擎示例");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var httpService = serviceProvider.GetRequiredService<HttpService>();
        
        try
        {
            // 测试 GET 请求
            Console.WriteLine("测试 GET 请求...");
            var getResult = await httpService.SendGetRequestAsync("https://example.com");
            Console.WriteLine($"GET 请求结果: {getResult.StatusCode}");
            Console.WriteLine($"响应长度: {getResult.ContentLength} 字节");
            
            // 测试 POST 请求
            Console.WriteLine("\n测试 POST 请求...");
            var postResult = await httpService.SendPostRequestAsync("https://example.com", "{\"name\": \"test\", \"value\": 123}");
            Console.WriteLine($"POST 请求结果: {postResult.StatusCode}");
            
            // 测试 PUT 请求
            Console.WriteLine("\n测试 PUT 请求...");
            var putResult = await httpService.SendPutRequestAsync("https://example.com", "{\"name\": \"test\", \"value\": 456}");
            Console.WriteLine($"PUT 请求结果: {putResult.StatusCode}");
            
            // 测试 DELETE 请求
            Console.WriteLine("\n测试 DELETE 请求...");
            var deleteResult = await httpService.SendDeleteRequestAsync("https://example.com");
            Console.WriteLine($"DELETE 请求结果: {deleteResult.StatusCode}");
            
            // 测试 HEAD 请求
            Console.WriteLine("\n测试 HEAD 请求...");
            var headResult = await httpService.SendHeadRequestAsync("https://example.com");
            Console.WriteLine($"HEAD 请求结果: {headResult.StatusCode}");
            Console.WriteLine($"响应头数量: {headResult.Headers.Count}");
            
            // 测试 OPTIONS 请求
            Console.WriteLine("\n测试 OPTIONS 请求...");
            var optionsResult = await httpService.SendOptionsRequestAsync("https://example.com");
            Console.WriteLine($"OPTIONS 请求结果: {optionsResult.StatusCode}");
            Console.WriteLine($"允许的方法: {optionsResult.Allow}");
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
        
        services.Configure<HttpSettings>(options => {
            options.BaseUrl = "https://example.com";
            options.Timeout = TimeSpan.FromSeconds(30);
            options.RetryCount = 3;
            options.RetryDelay = TimeSpan.FromSeconds(1);
            options.EnableCircuitBreaker = true;
            options.CircuitBreakDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreakFailureThreshold = 0.5;
            options.CircuitBreakSamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreakMinimumThroughput = 10;
            options.EnableProxy = false;
            options.ProxyAddress = "";
            options.EnableCompression = true;
            options.EnableCaching = true;
            options.CacheSize = 100;
            options.CacheDuration = TimeSpan.FromMinutes(5);
            options.EnableDetailedLogging = false;
        });
        
        services.AddSingleton<HttpService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}

public class HttpSettings
{
    public string BaseUrl { get; set; } = "https://example.com";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public bool EnableCircuitBreaker { get; set; } = true;
    public TimeSpan CircuitBreakDuration { get; set; } = TimeSpan.FromSeconds(30);
    public double CircuitBreakFailureThreshold { get; set; } = 0.5;
    public TimeSpan CircuitBreakSamplingDuration { get; set; } = TimeSpan.FromSeconds(30);
    public int CircuitBreakMinimumThroughput { get; set; } = 10;
    public bool EnableProxy { get; set; } = false;
    public string ProxyAddress { get; set; } = "";
    public bool EnableCompression { get; set; } = true;
    public bool EnableCaching { get; set; } = true;
    public int CacheSize { get; set; } = 100;
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableDetailedLogging { get; set; } = false;
}

public class HttpResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; }
    public long ContentLength { get; set; }
    public string ContentType { get; set; }
    public Dictionary<string, IEnumerable<string>> Headers { get; set; } = new();
    public string Allow { get; set; }
}

public class HttpService : IAsyncDisposable
{
    private readonly ILogger<HttpService> _logger;
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
    private readonly AsyncTimeoutPolicy<HttpResponseMessage> _timeoutPolicy;
    private readonly Dictionary<string, (string Content, DateTime Expires)> _cache;
    private readonly object _cacheLock = new();
    
    public HttpService(ILogger<HttpService> logger)
    {
        _logger = logger;
        
        // 创建 HttpClient
        var handler = new HttpClientHandler {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HTTP-AOT-Engine/1.0");
        
        // 配置重试策略
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) * 0.5));
        
        // 配置断路器策略
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(30),
                minimumThroughput: 10,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );
        
        // 配置超时策略
        _timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
        
        // 初始化缓存
        _cache = new Dictionary<string, (string, DateTime)>();
    }
    
    public async Task<HttpResponse> SendGetRequestAsync(string url)
    {
        // 检查缓存
        if (_cache.TryGetValue(url, out var cachedItem) && cachedItem.Expires > DateTime.UtcNow)
        {
            return new HttpResponse {
                StatusCode = HttpStatusCode.OK,
                Content = cachedItem.Content,
                ContentLength = cachedItem.Content.Length,
                ContentType = "text/plain"
            };
        }
        
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.GetAsync(url);
                });
            });
        });
        
        var result = await ProcessResponseAsync(response);
        
        // 添加到缓存
        if (result.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(result.Content))
        {
            lock (_cacheLock)
            {
                if (_cache.Count >= 100)
                {
                    var oldestKey = _cache.OrderBy(kv => kv.Value.Expires).First().Key;
                    _cache.Remove(oldestKey);
                }
                _cache[url] = (result.Content, DateTime.UtcNow.AddMinutes(5));
            }
        }
        
        return result;
    }
    
    public async Task<HttpResponse> SendPostRequestAsync(string url, string content)
    {
        var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
        
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.PostAsync(url, httpContent);
                });
            });
        });
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendPutRequestAsync(string url, string content)
    {
        var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
        
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.PutAsync(url, httpContent);
                });
            });
        });
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendDeleteRequestAsync(string url)
    {
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.DeleteAsync(url);
                });
            });
        });
        
        return await ProcessResponseAsync(response);
    }
    
    public async Task<HttpResponse> SendHeadRequestAsync(string url)
    {
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                });
            });
        });
        
        var result = new HttpResponse {
            StatusCode = response.StatusCode
        };
        
        foreach (var header in response.Headers)
        {
            result.Headers[header.Key] = header.Value;
        }
        
        return result;
    }
    
    public async Task<HttpResponse> SendOptionsRequestAsync(string url)
    {
        var response = await _timeoutPolicy.ExecuteAsync(async () => {
            return await _circuitBreakerPolicy.ExecuteAsync(async () => {
                return await _retryPolicy.ExecuteAsync(async () => {
                    return await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, url));
                });
            });
        });
        
        var result = new HttpResponse {
            StatusCode = response.StatusCode
        };
        
        if (response.Headers.TryGetValues("Allow", out var allowValues))
        {
            result.Allow = string.Join(", ", allowValues);
        }
        
        return result;
    }
    
    private async Task<HttpResponse> ProcessResponseAsync(HttpResponseMessage response)
    {
        var result = new HttpResponse {
            StatusCode = response.StatusCode,
            ContentLength = response.Content.Headers.ContentLength ?? 0,
            ContentType = response.Content.Headers.ContentType?.ToString()
        };
        
        // 复制响应头
        foreach (var header in response.Headers)
        {
            result.Headers[header.Key] = header.Value;
        }
        
        // 读取内容
        if (response.Content != null)
        {
            result.Content = await response.Content.ReadAsStringAsync();
            if (result.ContentLength == 0 && !string.IsNullOrEmpty(result.Content))
            {
                result.ContentLength = result.Content.Length;
            }
        }
        
        return result;
    }
    
    public async ValueTask DisposeAsync()
    {
        _httpClient.Dispose();
        _cache.Clear();
        await Task.CompletedTask;
    }
}
```

## AOT 编译示例

### AOT 编译配置文件 (csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    
    <!-- AOT 编译配置 -->
    <PublishAot>true</PublishAot>
    <TrimMode>Full</TrimMode>
    <PublishReadyToRun>true</PublishReadyToRun>
    <PublishSingleFile>true</PublishSingleFile>
    <SelfContained>true</SelfContained>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="System.Net.Http.Json" Version="10.0.0" />
    <PackageReference Include="WebApiClientCore" Version="3.1.0" />
    <PackageReference Include="WebApiClientCore.Extensions.DependencyInjection" Version="3.1.0" />
  </ItemGroup>
</Project>
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

### 运行 AOT 编译后的应用

```bash
# Windows
./bin/Release/net10.0/win-x64/publish/HttpApp.exe

# Linux
./bin/Release/net10.0/linux-x64/publish/HttpApp

# macOS
./bin/Release/net10.0/osx-x64/publish/HttpApp
```

## 总结

上述示例展示了 HTTP 技能的主要功能和使用方法，包括：

1. **基本 HTTP 客户端**：使用 IHttpClientFactory 创建和管理 HTTP 客户端
2. **HTTP 弹性策略**：实现重试、断路器、超时和舱壁隔离
3. **WebApiClientCore AOT 支持**：使用源生成器实现高性能 HTTP 客户端
4. **安全头配置**：保护应用程序免受常见 Web 攻击
5. **HTTP/3 和 QUIC 支持**：利用新一代网络协议提高性能
6. **HTTP AOT 核心引擎**：基于 AOT 编译的高性能 HTTP 客户端核心引擎，支持多 HTTP 方法、弹性策略、基准测试等功能
7. **AOT 编译**：将应用编译为本机代码，提高启动速度和运行性能

所有示例均遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

## HTTP AOT 架构总结

HTTP AOT 架构是一个完整的高性能 HTTP 客户端解决方案，包含以下核心组件：

- **http_aot.cs**：核心 HTTP AOT 引擎，实现了完整的 HTTP 客户端功能，支持多种 HTTP 方法、弹性策略、基准测试等
- **http_aot.setting.json**：AOT 编译配置，包含依赖项和运行时选项
- **http_aot.run.json**：运行环境配置，包含不同命令的启动设置

HTTP AOT 架构的主要优势：

1. **高性能**：基于 AOT 编译，启动速度快，运行性能高
2. **功能丰富**：支持多种 HTTP 方法、弹性策略、基准测试等功能
3. **易于使用**：提供友好的命令行界面和命令别名
4. **可靠性高**：内置弹性策略，提高系统的可靠性和稳定性
5. **可扩展性强**：模块化设计，便于扩展和维护

通过 HTTP AOT 架构，开发者可以快速构建高性能、可靠的 HTTP 客户端应用，满足各种 HTTP 相关的开发需求。
