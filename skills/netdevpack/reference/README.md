# NetDevPack - 参考文档

## 功能说明

NetDevPack 是一个基于 .NET 10 的高性能开发工具包，专为 .NET 开发者设计。它提供了一系列强大的功能，包括依赖注入增强、缓存管理、事件总线、对象映射和验证工具等，帮助开发者快速构建高质量的应用程序。

## 核心组件

### 1. INetDevPackService (NetDevPack 服务)
- **位置**: scripts/netdevpack_integration.cs
- **功能**: 核心业务逻辑处理，提供基础功能
- **特性**: 
  - 核心功能实现
  - 性能优化
  - 错误处理
  - 日志记录

### 2. INetDevPackEnhancedService (NetDevPack 增强服务)
- **位置**: scripts/netdevpack_enhanced_integration.cs
- **功能**: 提供增强的功能和高级特性
- **特性**: 
  - 高级业务逻辑处理
  - 复杂操作支持
  - 性能优化
  - 扩展功能

### 3. ICacheManager (缓存管理器)
- **位置**: scripts/netdevpack_integration.cs
- **功能**: 高性能缓存管理
- **特性**: 
  - 内存缓存支持
  - 分布式缓存集成
  - 缓存过期策略
  - 缓存统计

### 4. IEventBus (事件总线)
- **位置**: scripts/netdevpack_integration.cs
- **功能**: 轻量级事件总线
- **特性**: 
  - 发布/订阅模式
  - 事件处理
  - 异步事件
  - 事件过滤

### 5. IObjectMapper (对象映射器)
- **位置**: scripts/netdevpack_enhanced_integration.cs
- **功能**: 高性能对象映射
- **特性**: 
  - 复杂对象转换
  - 映射配置
  - 性能优化
  - 类型安全

### 6. IValidator (验证器)
- **位置**: scripts/netdevpack_enhanced_integration.cs
- **功能**: 数据验证和业务规则验证
- **特性**: 
  - 数据验证
  - 业务规则验证
  - 验证结果处理
  - 自定义验证规则

## 使用示例

### 基本用法

`csharp
// 获取 NetDevPack 服务
var netDevPackService = serviceProvider.GetRequiredService<INetDevPackService>();
var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
var eventBus = serviceProvider.GetRequiredService<IEventBus>();

// 使用基础功能
var result = await netDevPackService.DoSomethingAsync();
Console.WriteLine($"结果: {result}");

// 使用缓存
await cacheManager.SetAsync("key", "value", TimeSpan.FromMinutes(5));
var cachedValue = await cacheManager.GetAsync<string>("key");
Console.WriteLine($"缓存值: {cachedValue}");

// 使用事件总线
await eventBus.PublishAsync(new SampleEvent { Message = "Hello NetDevPack!" });
`

### 高级配置

`csharp
// 配置 NetDevPack 选项
builder.Services.Configure<NetDevPackOptions>(options => {
    options.EnableCache = true;
    options.CacheSize = 2000;
    options.Timeout = TimeSpan.FromSeconds(60);
    options.EnableDetailedLogging = true;
});

// 注册服务
builder.Services.AddSingleton<INetDevPackService, NetDevPackService>();
builder.Services.AddSingleton<INetDevPackEnhancedService, NetDevPackEnhancedService>();
builder.Services.AddSingleton<ICacheManager, CacheManager>();
builder.Services.AddSingleton<IEventBus, EventBus>();
builder.Services.AddSingleton<IObjectMapper, ObjectMapper>();
builder.Services.AddSingleton<IValidator, Validator>();

// 获取配置
var settings = serviceProvider.GetRequiredService<IOptions<NetDevPackOptions>>().Value;
Console.WriteLine($"配置: 缓存={settings.EnableCache}, 大小={settings.CacheSize}");
`

## 配置选项

### NetDevPackOptions 配置

`json
{
  "NetDevPackOptions": {
    "EnableCache": true,          // 是否启用缓存
    "CacheSize": 1000,            // 缓存大小
    "Timeout": "00:00:30",        // 超时时间
    "EnableDetailedLogging": false // 是否启用详细日志
  }
}
`

## 性能优化

1. **使用缓存**: 启用缓存以提高性能，减少重复计算和IO操作
2. **异步编程**: 使用异步 API 避免阻塞，提高系统吞吐量
3. **批处理**: 对于大量操作，使用批处理提高效率
4. **内存池**: 使用内存池减少内存分配和 GC 压力
5. **Span 优化**: 使用 Span<T> 减少内存拷贝，提高性能
6. **对象池**: 使用对象池减少对象创建和销毁的开销
7. **并行处理**: 对于 CPU 密集型操作，使用并行处理提高性能

## 故障排除

### 常见问题

1. **依赖注入失败**
   - 检查服务注册是否正确
   - 验证依赖项是否已安装
   - 查看日志信息以获取详细错误

2. **缓存问题**
   - 检查缓存配置是否正确
   - 验证缓存键是否唯一
   - 检查缓存过期策略

3. **事件总线问题**
   - 检查事件处理程序是否正确注册
   - 验证事件类型是否匹配
   - 查看事件处理日志

4. **性能问题**
   - 启用详细日志以识别瓶颈
   - 检查缓存使用情况
   - 优化对象映射和验证逻辑

5. **内存泄漏**
   - 检查缓存使用是否合理
   - 验证事件处理程序是否正确注销
   - 使用内存分析工具识别泄漏源

## 扩展开发

### 添加自定义功能

`csharp
public class CustomNetDevPackService : INetDevPackService
{
    private readonly ICacheManager _cacheManager;
    private readonly IEventBus _eventBus;

    public CustomNetDevPackService(ICacheManager cacheManager, IEventBus eventBus)
    {
        _cacheManager = cacheManager;
        _eventBus = eventBus;
    }

    public async Task<string> DoSomethingAsync()
    {
        // 检查缓存
        var cachedResult = await _cacheManager.GetAsync<string>("custom-operation-result");
        if (cachedResult != null)
        {
            return cachedResult;
        }

        // 执行自定义逻辑
        var result = "Custom operation result";

        // 缓存结果
        await _cacheManager.SetAsync("custom-operation-result", result, TimeSpan.FromMinutes(10));

        // 发布事件
        await _eventBus.PublishAsync(new CustomOperationCompletedEvent { Result = result });

        return result;
    }

    // 其他方法实现...
}
`

### 自定义缓存策略

`csharp
public class CustomCacheStrategy : ICacheStrategy
{
    public TimeSpan GetExpirationTime(string key, object value)
    {
        // 根据键和值类型确定过期时间
        if (key.StartsWith("critical-"))
        {
            // 关键数据过期时间较短
            return TimeSpan.FromMinutes(1);
        }
        else if (key.StartsWith("static-"))
        {
            // 静态数据过期时间较长
            return TimeSpan.FromHours(24);
        }
        else
        {
            // 默认过期时间
            return TimeSpan.FromMinutes(10);
        }
    }

    public bool ShouldCache(string key, object value)
    {
        // 决定是否缓存特定键值对
        if (value == null)
        {
            return false;
        }

        // 缓存大小限制
        var size = GetObjectSize(value);
        return size < 1024 * 1024; // 缓存小于 1MB 的对象
    }

    private long GetObjectSize(object value)
    {
        // 实现对象大小计算逻辑
        // ...
        return 0;
    }
}
`

### 自定义事件处理程序

`csharp
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent @event)
    {
        _logger.LogInformation($"处理订单创建事件: 订单ID={@event.OrderId}");

        // 执行订单创建后的逻辑
        // 例如: 发送通知、更新库存、记录日志等

        await Task.CompletedTask;
    }
}

// 注册事件处理程序
builder.Services.AddSingleton<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
`

## 部署指南

### 容器化部署

1. **创建 Dockerfile**

`dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["netdevpack_integration.csproj", "."]
RUN dotnet restore "./netdevpack_integration.csproj"
COPY . .
WORKDIR "/src/.."
RUN dotnet build "netdevpack_integration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "netdevpack_integration.csproj" -c Release -o /app/publish /p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./netdevpack_integration"]
`

2. **构建和运行容器**

`bash
docker build -t netdevpack-integration .
docker run --name netdevpack-integration -d netdevpack-integration
`

### 云平台部署

NetDevPack 技能可以部署在各种云平台上，如 Azure、AWS、GCP 等。以下是在 Azure 上部署的示例：

1. **创建 Azure App Service**
2. **配置应用设置**：设置环境变量和配置选项
3. **部署应用**：使用 Azure DevOps 或 GitHub Actions 自动部署
4. **监控和日志**：配置 Azure Monitor 进行监控和日志收集

## 安全最佳实践

1. **数据验证**：使用 IValidator 进行数据验证，确保输入数据安全
2. **错误处理**：正确处理异常，避免暴露敏感信息
3. **日志记录**：添加适当的日志记录，便于安全审计和故障排查
4. **权限控制**：实现适当的权限控制，确保只有授权用户可以访问资源
5. **加密**：对敏感数据进行加密，保护数据安全
6. **定期更新**：定期更新依赖项，修复安全漏洞

## 监控和维护

1. **健康检查**：实现健康检查端点，监控系统状态
2. **性能监控**：监控关键性能指标，如响应时间、吞吐量、内存使用等
3. **日志管理**：集中管理日志，便于分析和故障排查
4. **告警机制**：设置告警机制，及时发现和处理问题
5. **定期维护**：定期清理缓存、备份数据、优化性能

## 版本历史

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本，基于 .NET 10 |
| 1.0.1 | 2026-02-01 | 性能优化，添加对象映射功能 |
| 1.0.2 | 2026-02-15 | 修复缓存问题，增加事件总线功能 |
| 1.0.3 | 2026-03-01 | 添加验证工具，优化内存使用 |
