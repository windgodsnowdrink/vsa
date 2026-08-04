# http Agent Skill - HTTP 客户端和服务器技能

## 技能概述

基于 .NET 10 的高性能 HTTP 客户端和服务器技能，为 .NET 开发者提供强大的 HTTP 功能，包括 HTTP 客户端、HTTP 服务器、HTTP/3、QUIC、HTTP 弹性、HTTP 报告、安全头、Webhook、AntiXSS、数据保护等功能。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Net.Http.Json@10.0.0
#:package Microsoft.AspNetCore.Http.Abstractions@10.0.0
```

### 注册服务

在您的主应用程序中注册 HTTP 服务：

```csharp
// 注册 HTTP 服务
var builder = WebApplication.CreateBuilder();

// 配置 HTTP 客户端
builder.Services.AddHttpClient("Default", client => {
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// 配置 HTTP 弹性
builder.Services.AddHttpClient("Resilient")
    .AddStandardResilienceHandler(options => {
        options.Retry.MaxRetryAttempts = 3;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(60);
    });

var app = builder.Build();

app.Run();
```

### 使用示例

```csharp
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

## 导航地图

```
http/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
├── scripts/                    # 脚本和工具
│   ├── http_aot.cs               # HTTP AOT 核心引擎
│   ├── http_aot.run.json         # HTTP AOT 运行配置
│   ├── http_aot.setting.json     # HTTP AOT 设置文件
│   ├── http3_quic.cs               # HTTP/3 QUIC 实现
│   ├── http3_quic.run.json         # HTTP/3 QUIC 运行配置
│   ├── http3_quic.setting.json     # HTTP/3 QUIC 设置文件
│   ├── http_resilience_integration.cs  # HTTP 弹性集成
│   ├── http_resilience_integration.run.json  # HTTP 弹性运行配置
│   ├── http_resilience_integration.setting.json  # HTTP 弹性设置文件
│   ├── httpreports_integration.cs  # HTTP 报告集成
│   ├── httpreports_integration.run.json  # HTTP 报告运行配置
│   ├── httpreports_integration.setting.json  # HTTP 报告设置文件
│   ├── securityheaders_integration.cs  # 安全头集成
│   ├── securityheaders_integration.run.json  # 安全头运行配置
│   ├── securityheaders_integration.setting.json  # 安全头设置文件
│   ├── antixss_integration.cs      # AntiXSS 集成
│   ├── antixss_integration.run.json  # AntiXSS 运行配置
│   ├── antixss_integration.setting.json  # AntiXSS 设置文件
│   ├── webapiclientcore_integration.cs  # WebApiClientCore 集成
│   ├── webapiclientcore_integration.run.json  # WebApiClientCore 运行配置
│   ├── webapiclientcore_integration.setting.json  # WebApiClientCore 设置文件
│   └── ...                        # 更多 HTTP 相关脚本
└── securityheaders_config.json    # 安全头配置文件
```

## 主要功能

1. **HTTP 客户端**: 基于 .NET 10 的高性能 HTTP 客户端，支持 HTTP/1.1、HTTP/2 和 HTTP/3
2. **HTTP 服务器**: 基于 ASP.NET Core 10 的高性能 HTTP 服务器
3. **HTTP/3 和 QUIC**: 支持 HTTP/3 和 QUIC 协议，提供更快的连接建立和数据传输
4. **HTTP 弹性**: 内置弹性策略，包括重试、断路器、超时和舱壁隔离
5. **HTTP 报告**: 提供 HTTP 请求和响应的报告功能，支持 Prometheus、OpenTelemetry 和告警
6. **安全头**: 提供安全头配置和管理功能，增强应用程序的安全性
7. **AntiXSS**: 提供跨站点脚本攻击防护
8. **数据保护**: 提供数据加密和保护功能
9. **WebApiClientCore**: 基于 AOT 编译的高性能 HTTP 客户端库
10. **HATEOAS**: 支持 HATEOAS（基于超媒体的状态引擎）
11. **Webhook**: 提供 Webhook 集成功能
12. **HTTP REPL**: 提供 HTTP REPL 工具，用于交互式测试 HTTP API
13. **源生成器**: 使用 .NET 10 源生成器，减少运行时反射，提高性能

## 扩展说明

此技能提供完整的 HTTP 解决方案，您可以根据需要进行扩展：

1. **自定义 HTTP 客户端**: 实现自定义的 HTTP 客户端，支持特定的协议或功能
2. **自定义 HTTP 服务器中间件**: 实现自定义的 HTTP 服务器中间件，添加自定义的请求处理逻辑
3. **扩展 HTTP 弹性策略**: 实现自定义的 HTTP 弹性策略，适应特定的业务需求
4. **集成其他 HTTP 库**: 集成其他 HTTP 库，如 Refit、RestSharp 等
5. **扩展 HTTP 报告**: 扩展 HTTP 报告功能，支持更多的报告格式和数据源
6. **自定义安全头**: 实现自定义的安全头，增强应用程序的安全性

## 最佳实践

1. **使用 HttpClientFactory**: 使用 IHttpClientFactory 管理 HTTP 客户端，避免连接泄漏
2. **异步编程**: 优先使用异步 API 进行 HTTP 操作，避免阻塞主线程
3. **配置适当的超时**: 为 HTTP 请求配置适当的超时时间，避免长时间阻塞
4. **使用 HTTP 弹性**: 为 HTTP 请求添加弹性策略，提高系统的可靠性
5. **添加适当的日志记录**: 为 HTTP 请求和响应添加适当的日志记录，便于调试和监控
6. **使用安全头**: 配置适当的安全头，增强应用程序的安全性
7. **使用 HTTPS**: 优先使用 HTTPS 进行通信，保护数据传输的安全性
8. **优化 HTTP 请求**: 优化 HTTP 请求，减少请求大小和数量
9. **使用缓存**: 适当使用缓存，减少重复请求
10. **监控 HTTP 性能**: 监控 HTTP 请求的性能指标，如响应时间、成功率等

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
        "MaxDelay": "00:00:10"
      },
      "CircuitBreaker": {
        "FailureRatio": 0.5,
        "SamplingDuration": "00:00:30",
        "BreakDuration": "00:01:00"
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
    "X XssProtection": "1; mode=block",
    "ReferrerPolicy": "strict-origin-when-cross-origin",
    "PermissionsPolicy": "geolocation=(self)",
    "ContentSecurityPolicy": "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self';"
  }
}
```

## 性能优化建议

1. **使用源生成器**: 启用 WebApiClientCore 等库的源生成器，减少运行时反射开销
2. **配置适当的连接池**: 配置适当的 HTTP 连接池大小，避免连接频繁创建和销毁
3. **使用 HTTP/3 和 QUIC**: 对于支持 HTTP/3 的环境，使用 HTTP/3 和 QUIC 协议
4. **优化序列化**: 使用高效的序列化器，如 MessagePack 或 Protobuf
5. **批量处理**: 对于大量小请求，考虑使用批量处理减少开销
6. **使用缓存**: 适当使用缓存，减少重复请求
7. **配置适当的超时**: 为 HTTP 请求配置适当的超时时间，避免长时间阻塞
8. **使用 AOT 编译**: 将 HTTP 应用编译为本机代码，提高启动速度和运行性能

## 故障排除

### 常见问题

1. **HTTP 请求失败**: 检查请求 URL、HTTP 方法、请求头和请求体是否正确
2. **连接超时**: 检查网络连接、服务器状态和超时设置
3. **认证失败**: 检查认证信息是否正确，如 API 密钥、令牌等
4. **序列化/反序列化错误**: 检查请求和响应的数据格式是否正确
5. **安全头错误**: 检查安全头配置是否正确，避免与应用程序冲突
6. **HTTP 弹性策略触发**: 检查弹性策略配置，调整重试次数、断路器设置等

### 调试建议

1. **启用详细日志**: 配置 HTTP 客户端和服务器的详细日志记录
2. **使用 Fiddler 或 Wireshark**: 使用网络调试工具，分析 HTTP 请求和响应
3. **检查 HTTP 状态码**: 检查 HTTP 响应的状态码，了解请求失败的原因
4. **检查服务器日志**: 检查服务器日志，了解服务器端的错误信息
5. **使用 HTTP REPL**: 使用 HTTP REPL 工具，交互式测试 HTTP API
6. **监控 HTTP 性能**: 使用 Prometheus、Grafana 等工具监控 HTTP 性能指标

## HTTP AOT 架构

此 HTTP 技能提供了完整的 AOT（Ahead-of-Time）编译架构，通过 `http_aot.cs` 脚本实现了高性能的 HTTP 客户端功能，支持多种 HTTP 方法、弹性策略、基准测试等功能。

### HTTP AOT 核心功能

1. **多 HTTP 方法支持**: GET、POST、PUT、DELETE、HEAD、OPTIONS
2. **弹性策略**: 内置重试、断路器、超时等弹性机制
3. **基准测试**: 支持 HTTP 请求的性能基准测试
4. **代理支持**: 内置 HTTP 代理测试功能
5. **缓存机制**: 支持 HTTP 响应缓存，提高性能
6. **压缩支持**: 自动处理 HTTP 压缩
7. **详细日志**: 支持详细的 HTTP 请求和响应日志
8. **命令行界面**: 提供友好的命令行界面，支持命令别名

### HTTP AOT 配置文件

HTTP AOT 架构包含以下配置文件：

- **http_aot.cs**: 核心 HTTP AOT 引擎，实现了完整的 HTTP 客户端功能
- **http_aot.setting.json**: AOT 编译配置，包含依赖项和运行时选项
- **http_aot.run.json**: 运行环境配置，包含不同命令的启动设置

### HTTP AOT 使用示例

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

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
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
```

### AOT 编译注意事项

1. **使用 AOT 兼容的 HTTP 客户端库**: 如 WebApiClientCore，它专门为 AOT 编译优化
2. **避免使用反射**: 避免在运行时使用反射，或使用 Source Generator 替代
3. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit
4. **资源加载**: 确保所有资源都能在 AOT 编译时被正确处理
5. **第三方库兼容性**: 确保使用的第三方库支持 AOT 编译
6. **测试验证**: 在 AOT 编译后进行充分的测试，确保应用正常运行

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

// 使用安全头中间件
app.UseSecurityHeaders();

// 使用 HTTP 报告中间件
app.UseHttpReports();

app.Run();
```

### 与 WebApiClientCore 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 注册 WebApiClientCore
builder.Services.AddHttpApi<IUserApi>(o => {
    o.HttpHost = new Uri("https://api.example.com");
});

builder.Services.AddHttpApiClient<IUserApi>();

var app = builder.Build();

// 在控制器中使用 WebApiClientCore
app.MapGet("/users", async ([FromServices] IUserApi userApi) => {
    var users = await userApi.GetUsersAsync();
    return Results.Ok(users);
});

app.Run();
```

### 与 Refit 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 Refit
builder.Services.AddRefitClient<IUserApi>(new RefitSettings {
    ContentSerializer = new SystemTextJsonContentSerializer()
}).ConfigureHttpClient(c => {
    c.BaseAddress = new Uri("https://api.example.com");
}).AddStandardResilienceHandler();

var app = builder.Build();

app.Run();
```

### 与 RestSharp 集成

```csharp
var builder = WebApplication.CreateBuilder(args);

// 配置 RestSharp
builder.Services.AddSingleton<IRestClient>(sp => {
    var options = new RestClientOptions("https://api.example.com") {
        Timeout = TimeSpan.FromSeconds(30)
    };
    return new RestClient(options);
});

var app = builder.Build();

app.Run();
```
