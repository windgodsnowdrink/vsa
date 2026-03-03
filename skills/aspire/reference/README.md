# aspire - 参考文档

## 概述

aspire 是一个基于 .NET 10 的高性能云原生应用开发和部署系统，专为 .NET 开发者设计，提供了分布式应用开发、服务发现、配置管理、健康检查等核心功能，支持微服务架构和容器化部署。

## 核心组件

### 1. DistributedApplication (分布式应用构建器)
- **位置**: Aspire.Hosting 包
- **功能**: 提供统一的方式来定义、配置和部署分布式应用
- **特性**: 
  - 支持多种服务类型（项目、容器、云服务等）
  - 内置服务发现和注册机制
  - 简化服务间依赖管理
  - 支持健康检查和监控
  - 提供声明式 API 设计

### 2. Service Discovery (服务发现)
- **位置**: Aspire.Hosting 包
- **功能**: 自动管理服务的注册和发现
- **特性**: 
  - 支持多种服务发现机制
  - 自动生成服务连接字符串
  - 支持服务健康状态监控
  - 支持服务拓扑可视化

### 3. Configuration Management (配置管理)
- **位置**: Aspire.Hosting 包
- **功能**: 集中管理应用配置
- **特性**: 
  - 支持动态配置更新
  - 支持环境隔离
  - 支持多种配置源
  - 支持配置加密

### 4. Health Checks (健康检查)
- **位置**: Aspire.Hosting 包
- **功能**: 内置健康检查和监控功能
- **特性**: 
  - 支持自定义健康检查逻辑
  - 实时监控服务状态
  - 支持健康状态可视化
  - 支持告警通知

### 5. Dashboard (仪表板)
- **位置**: Aspire.Dashboard 包
- **功能**: 提供应用监控和管理界面
- **特性**: 
  - 服务拓扑可视化
  - 实时指标监控
  - 日志查看和分析
  - 健康状态监控
  - 配置管理

### 6. AOT Compilation (AOT 编译)
- **位置**: 内置支持
- **功能**: 将应用编译为本机代码，提高启动速度和运行性能
- **特性**: 
  - 支持静态代码分析
  - 减少内存占用
  - 提高启动速度
  - 优化运行时性能

## 使用示例

### 基本使用

```csharp
// 创建分布式应用构建器
var builder = DistributedApplication.CreateBuilder(args);

// 添加 PostgreSQL 数据库服务
var db = builder.AddPostgres("postgres")
    .WithPassword("postgres")
    .WithDatabase("mydb");

// 添加 ASP.NET Core 项目
var api = builder.AddProject<Projects.MyApi>("api")
    .WithReference(db);

// 构建并运行应用
var app = builder.Build();
await app.RunAsync();
```

### 高级配置

```csharp
// 创建分布式应用构建器
var builder = DistributedApplication.CreateBuilder(args);

// 配置应用全局设置
builder.Configuration.AddJsonFile("appsettings.custom.json");

// 添加 Redis 缓存服务
var redis = builder.AddRedis("redis")
    .WithRedisConfiguration("Cache", "0")
    .WithPersistence()
    .WithMemoryLimit("512MB")
    .WithCpuLimit(0.5);

// 添加自定义服务配置
var myService = builder.AddProject<Projects.MyService>("myservice")
    .WithReference(redis)
    .WithReplicas(3)
    .WithHealthChecks("/health")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production")
    .WithEnvironment("REDIS_CONFIG", redis.GetEndpoint("redis"))
    .WithBindMount("./config", "/app/config")
    .WithVolumeMount("myvolume", "/app/data");

// 添加外部服务
var externalApi = builder.AddExternalService("external-api")
    .WithHttpEndpoint("https://api.example.com")
    .WithHealthChecks("/health");

myService.WithReference(externalApi);

// 构建并运行应用
var app = builder.Build();
await app.RunAsync();
```

## 配置选项

### 分布式应用配置

```json
{
  "DistributedApplication": {
    "EnableDashboard": true,          // 启用仪表板
    "DashboardPort": 18888,           // 仪表板端口
    "EnableTracing": true,            // 启用追踪
    "TracingSamplingRate": 1.0,       // 追踪采样率
    "EnableMetrics": true,            // 启用指标收集
    "MetricsPort": 9100,              // 指标端口
    "EnableHealthChecks": true,       // 启用健康检查
    "HealthCheckInterval": "00:00:10" // 健康检查间隔
  }
}
```

### 服务配置示例

```json
{
  "Services": {
    "postgres": {
      "Password": "postgres",
      "Database": "mydb",
      "Persistence": true,
      "MemoryLimit": "1GB",
      "CpuLimit": 1.0
    },
    "redis": {
      "Configuration": "Cache,0",
      "Persistence": true
    },
    "api": {
      "Replicas": 2,
      "Environment": {
        "ASPNETCORE_ENVIRONMENT": "Production",
        "LOG_LEVEL": "Information"
      },
      "HealthChecks": "/health"
    }
  }
}
```

## 性能优化

### 应用开发优化

1. **使用 AOT 编译**: 对于性能敏感的服务，使用 AOT 编译优化
   ```csharp
   // 在项目文件中启用 AOT 编译
   <PublishAot>true</PublishAot>
   <TrimMode>Full</TrimMode>
   ```

2. **异步编程**: 优先使用异步 API 进行操作，避免阻塞主线程

3. **合理的服务粒度**: 合理划分服务边界，避免服务过大或过小

4. **缓存策略**: 适当使用缓存，减少重复计算和数据库查询

### 部署优化

1. **资源限制**: 为每个服务配置合理的资源限制（CPU、内存）

2. **服务副本**: 根据负载情况配置适当的服务副本数量

3. **容器化**: 使用容器化部署，提高资源利用率和部署效率

4. **网络优化**: 优化服务间通信，减少网络延迟

5. **存储优化**: 选择合适的存储方案，优化存储访问性能

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

1. **使用 AOT 兼容的库**: 确保所有依赖库都支持 AOT 编译

2. **避免反射**: 避免在运行时使用反射，或使用 Source Generator 替代

3. **资源加载**: 确保所有资源都能在 AOT 编译时被正确处理

4. **动态代码生成**: 避免使用动态代码生成，如 System.Reflection.Emit

5. **序列化优化**: 使用 AOT 兼容的序列化库，如 MessagePack 或 Protobuf

6. **测试验证**: 在 AOT 编译后进行充分的测试，确保应用正常运行

## 故障排除

### 常见问题

1. **服务启动失败**
   - 检查服务配置文件
   - 查看服务日志
   - 检查依赖服务是否正常运行
   - 检查资源限制是否合理

2. **服务间通信失败**
   - 检查服务发现配置
   - 验证服务连接字符串
   - 检查网络连接
   - 查看防火墙设置

3. **性能问题**
   - 启用监控和追踪，定位瓶颈
   - 优化服务资源配置
   - 检查数据库查询性能
   - 优化服务间通信

4. **仪表板访问问题**
   - 检查仪表板端口配置
   - 验证网络连接
   - 查看仪表板日志
   - 检查防火墙设置

### 调试建议

1. **启用详细日志**: 配置应用日志为详细级别，便于调试
   ```csharp
   builder.Logging.AddConsole();
   builder.Logging.SetMinimumLevel(LogLevel.Debug);
   ```

2. **使用仪表板**: 使用 aspire 仪表板查看服务状态和日志

3. **健康检查**: 检查服务健康状态

4. **网络调试**: 使用网络调试工具，如 Wireshark，分析服务间通信

5. **性能分析**: 使用性能分析工具，如 dotnet-trace，分析性能问题

## 扩展开发

### 添加自定义服务组件

```csharp
// 定义自定义服务组件
public class MyCustomService : ContainerResource
{
    public MyCustomService(string name)
        : base(name, "mycompany/myservice:latest")
    {
        // 配置默认设置
        WithPortBinding(8080, name: "http");
        WithHealthCheck("http://localhost:8080/health");
    }
    
    // 添加自定义配置方法
    public MyCustomService WithCustomSetting(string key, string value)
    {
        WithEnvironment(key, value);
        return this;
    }
    
    public MyCustomService WithCustomVolume(string volumeName)
    {
        WithVolumeMount(volumeName, "/app/data");
        return this;
    }
}

// 扩展 DistributedApplicationBuilder
public static class MyCustomServiceExtensions
{
    public static MyCustomService AddMyCustomService(this IDistributedApplicationBuilder builder, string name)
    {
        var service = new MyCustomService(name);
        builder.AddResource(service);
        return service;
    }
}

// 使用自定义服务组件
var builder = DistributedApplication.CreateBuilder(args);
var myService = builder.AddMyCustomService("myservice")
    .WithCustomSetting("CUSTOM_KEY", "custom-value")
    .WithCustomVolume("myvolume");
```

### 添加自定义健康检查

```csharp
// 定义自定义健康检查
public class MyHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    
    public MyHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // 自定义健康检查逻辑
            var response = await _httpClient.GetAsync("https://external-service.com/health", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External service is healthy");
            }
            
            return HealthCheckResult.Degraded("External service is degraded");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("External service is unhealthy", ex);
        }
    }
}

// 注册自定义健康检查
builder.Services.AddHealthChecks()
    .AddCheck<MyHealthCheck>("my-custom-check")
    .AddNpgSql(Configuration.GetConnectionString("Postgres"))
    .AddRedis(Configuration.GetConnectionString("Redis"));

// 在应用中使用健康检查
app.MapHealthChecks("/health");
```

### 扩展配置源

```csharp
// 定义自定义配置源
public class MyCustomConfigSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new MyCustomConfigProvider();
    }
}

public class MyCustomConfigProvider : ConfigurationProvider
{
    public override void Load()
    {
        // 加载自定义配置
        Data = new Dictionary<string, string>
        {
            { "Custom:Key1", "Value1" },
            { "Custom:Key2", "Value2" }
        };
    }
}

// 扩展 IConfigurationBuilder
public static class MyCustomConfigExtensions
{
    public static IConfigurationBuilder AddMyCustomConfig(this IConfigurationBuilder builder)
    {
        return builder.Add(new MyCustomConfigSource());
    }
}

// 使用自定义配置源
var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.AddMyCustomConfig();
```

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 ASP.NET Core 应用中使用 aspire
var builder = WebApplication.CreateBuilder(args);

// 添加 aspire 服务
builder.AddServiceDefaults();

// 配置数据库连接
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));

// 配置 Redis 缓存
builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("redis"));

// 构建应用
var app = builder.Build();

// 使用 aspire 健康检查中间件
app.MapDefaultEndpoints();

// 定义 API 端点
app.MapGet("/", () => "Hello from ASP.NET Core with aspire!");

app.Run();
```

### 与 Kubernetes 集成

```csharp
// 配置 Kubernetes 部署
var builder = DistributedApplication.CreateBuilder(args);

// 添加服务
var api = builder.AddProject<Projects.MyApi>("api")
    .WithReplicas(3)
    .WithIngress(
        name: "myapi",
        host: "api.example.com",
        path: "/",
        pathType: IngressPathType.Prefix);

// 配置 Kubernetes 资源
builder.AddKubernetesDeployment(
    name: "myapp",
    namespace: "default",
    labels: new Dictionary<string, string> { { "app", "myapp" } });

var app = builder.Build();
await app.RunAsync();
```

### 与 OpenTelemetry 集成

```csharp
// 配置 OpenTelemetry
var builder = DistributedApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => {
        metrics.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddPrometheusExporter();
    })
    .WithTracing(tracing => {
        tracing.AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddNpgsqlInstrumentation()
            .AddRedisInstrumentation()
            .AddConsoleExporter();
    });

// 添加 Prometheus 端点
var app = builder.Build();
app.MapPrometheusScrapingEndpoint();

await app.RunAsync();
```
