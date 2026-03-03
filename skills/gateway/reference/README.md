# Gateway - 参考文档

## 概述

Gateway 是基于 .NET 10 的高性能网关系统，专为 .NET 开发者设计。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 核心组件

### 1. GatewayService (Gateway 服务)
- **位置**: scripts/gateway_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 网关服务管理（启动、停止、重启）
  - 路由管理（添加、删除、列出）
  - 状态监控
  - 版本信息查询
  - 性能优化
  - 错误处理
  - 日志记录

### 2. GatewayAotEngine (Gateway AOT 引擎)
- **位置**: scripts/gateway_aot.cs
- **功能**: 命令行引擎，处理命令行参数和执行命令
- **特性**: 
  - 命令行参数解析
  - 别名支持
  - 结果输出格式化
  - 错误处理

## 使用示例

### 基本使用

```csharp
// 获取 Gateway 服务
var gatewayService = serviceProvider.GetRequiredService<IGatewayService>();

// 启动网关服务
var startResult = await gatewayService.StartAsync();
Console.WriteLine($"启动结果: {(startResult.Success ? "成功" : "失败")}");

// 添加路由
var addRouteResult = await gatewayService.AddRouteAsync("api_route", "/api/{**remainder}", "api_cluster", "http://localhost:5000");
Console.WriteLine($"添加路由结果: {(addRouteResult.Success ? "成功" : "失败")}");

// 列出路由
var listRoutesResult = await gatewayService.ListRoutesAsync();
Console.WriteLine($"列出路由结果: {(listRoutesResult.Success ? "成功" : "失败")}");

// 获取状态
var statusResult = await gatewayService.GetStatusAsync();
Console.WriteLine($"获取状态结果: {(statusResult.Success ? "成功" : "失败")}");

// 停止网关服务
var stopResult = await gatewayService.StopAsync();
Console.WriteLine($"停止结果: {(stopResult.Success ? "成功" : "失败")}");
```

### 高级配置

```csharp
// 配置 Gateway 选项
var gatewayOptions = new GatewayOptions
{
    ServerAddress = "localhost",
    ServerPort = 8080,
    EnableSsl = false,
    EnableCompression = true,
    EnableCircuitBreaker = true,
    CircuitBreakerFailureThreshold = 5,
    CircuitBreakerResetTimeMs = 30000,
    EnableCache = true,
    CacheSize = 1000,
    RequestTimeoutMs = 30000,
    EnableDetailedLogging = false
};

builder.Services.Configure<GatewayOptions>(options => {
    options.ServerAddress = gatewayOptions.ServerAddress;
    options.ServerPort = gatewayOptions.ServerPort;
    options.EnableSsl = gatewayOptions.EnableSsl;
    options.EnableCompression = gatewayOptions.EnableCompression;
    options.EnableCircuitBreaker = gatewayOptions.EnableCircuitBreaker;
    options.CircuitBreakerFailureThreshold = gatewayOptions.CircuitBreakerFailureThreshold;
    options.CircuitBreakerResetTimeMs = gatewayOptions.CircuitBreakerResetTimeMs;
    options.EnableCache = gatewayOptions.EnableCache;
    options.CacheSize = gatewayOptions.CacheSize;
    options.RequestTimeoutMs = gatewayOptions.RequestTimeoutMs;
    options.EnableDetailedLogging = gatewayOptions.EnableDetailedLogging;
});
```

## 配置选项

### Gateway 配置

```json
{
  "Gateway": {
    "ServerAddress": "localhost",
    "ServerPort": 8080,
    "EnableSsl": false,
    "EnableCompression": true,
    "EnableCircuitBreaker": true,
    "CircuitBreakerFailureThreshold": 5,
    "CircuitBreakerResetTimeMs": 30000,
    "WorkingDirectory": "d:\\Trae\\vsa\\skills\\gateway",
    "EnableDetailedLogging": false,
    "EnablePerformanceMonitoring": true,
    "RequestTimeoutMs": 30000
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **连接池**: 使用连接池管理资源
5. **AOT 编译**: 使用 AOT 编译提高启动速度和运行性能
6. **单文件部署**: 减少依赖，提高部署效率
7. **电路 breaker**: 合理配置电路 breaker 参数，提高系统稳定性
8. **压缩配置**: 根据实际情况配置压缩选项

## 故障排除

### 常见问题

1. **网关启动失败**
   - 检查配置文件
   - 验证端口是否被占用
   - 检查日志信息

2. **路由添加失败**
   - 检查路由 ID 是否重复
   - 验证目标地址是否可达
   - 检查权限配置

3. **性能问题**
   - 启用缓存
   - 优化路由配置
   - 增加资源限制
   - 使用 AOT 编译

4. **AOT 编译问题**
   - 确保使用 .NET 10
   - 检查依赖项是否支持 AOT
   - 查看编译错误信息

## 扩展开发

### 添加自定义功能

```csharp
public class CustomGatewayService : IGatewayService
{
    private readonly GatewayOptions _options;
    private readonly ILogger<CustomGatewayService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public CustomGatewayService(IOptions<GatewayOptions> options, ILogger<CustomGatewayService> logger, IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    // 实现接口方法...
    public async Task<GatewayCommandResult> StartAsync()
    {
        // 自定义实现
        var result = new GatewayCommandResult();
        // 实现逻辑
        return result;
    }

    // 实现其他接口方法...
}
```

### 注册自定义服务

```csharp
// 注册自定义 Gateway 服务
builder.Services.Configure<GatewayOptions>(builder.Configuration.GetSection("Gateway"));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IGatewayService, CustomGatewayService>();
builder.Services.AddSingleton<GatewayAotEngine>();
```
