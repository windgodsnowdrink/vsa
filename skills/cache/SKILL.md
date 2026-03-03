# cache Agent Skill - 缓存技能

## 技能概述

基于 .NET 10 的高性能缓存技能，为 .NET 开发者提供强大的缓存管理和优化功能，支持 AOT（提前编译）编译，适用于构建高性能、可扩展的数据缓存系统。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package StackExchange.Redis@2.8.0
```

### 注册服务

在您的主应用程序中注册缓存服务：

```csharp
// 注册内存缓存服务
builder.Services.AddMemoryCache(options => {
    options.SizeLimit = 1024 * 1024 * 1024; // 1GB 缓存大小
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
});

// 注册 Redis 缓存服务
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
    options.InstanceName = "CacheSkill:";
});

// 注册自定义缓存服务
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<IDistributedCacheService, DistributedCacheService>();
builder.Services.AddSingleton<IMultiLevelCacheService, MultiLevelCacheService>();
```

### 使用示例

```csharp
// 获取缓存服务
var cacheService = serviceProvider.GetRequiredService<ICacheService>();

// 使用内存缓存
await cacheService.SetAsync("key1", "Hello World", TimeSpan.FromMinutes(10));
var value1 = await cacheService.GetAsync<string>("key1");
Console.WriteLine($"内存缓存结果: {value1}");

// 使用分布式缓存
var distributedCache = serviceProvider.GetRequiredService<IDistributedCacheService>();
await distributedCache.SetAsync("distributedKey", new User { Id = 1, Name = "张三" }, TimeSpan.FromHours(1));
var user = await distributedCache.GetAsync<User>("distributedKey");
Console.WriteLine($"分布式缓存结果: {user?.Name}");

// 使用多级缓存
var multiLevelCache = serviceProvider.GetRequiredService<IMultiLevelCacheService>();
await multiLevelCache.SetAsync("multiKey", "多级缓存数据", TimeSpan.FromMinutes(30));
var multiValue = await multiLevelCache.GetAsync<string>("multiKey");
Console.WriteLine($"多级缓存结果: {multiValue}");
```

## 导航地图

```
cache/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── cache_heartbeat_integration.cs # 缓存心跳集成
    ├── cache_hybrid_integration.cs    # 混合缓存集成
    ├── easycaching_demo.cs            # EasyCaching 演示
    ├── fusioncache_demo.cs            # FusionCache 演示
    ├── garnet_redis_cache.cs          # Garnet Redis 缓存
    ├── lazycache_integration.cs       # LazyCache 集成
    ├── microsoft_cache_demo.cs        # Microsoft 缓存演示
    ├── multitenant_cache_service.cs   # 多租户缓存服务
    ├── redis_eventbus_integration.cs  # Redis 事件总线集成
    ├── redis_stream_integration.cs    # Redis 流集成
    └── *.run.json/*.setting.json      # 运行配置和设置文件
```

## 主要功能

1. **内存缓存实现**: 基于 .NET 10 的高性能内存缓存，支持多种过期策略
2. **分布式缓存集成**: 支持 Redis、Garnet 等分布式缓存系统
3. **多级缓存支持**: 内存缓存 + 分布式缓存的多级缓存架构
4. **缓存过期策略**: 支持绝对过期、滑动过期、相对过期等多种过期策略
5. **缓存预热和刷新**: 支持缓存预热和自动刷新功能
6. **缓存穿透防护**: 实现布隆过滤器等缓存穿透防护机制
7. **缓存击穿处理**: 实现热点数据保护，防止缓存击穿
8. **缓存雪崩预防**: 实现缓存过期时间随机化，防止缓存雪崩
9. **支持 AOT 编译优化**: 支持将缓存应用编译为本机代码，提高运行时性能
10. **缓存监控和统计**: 提供缓存命中率、性能指标等监控功能
11. **多租户支持**: 支持多租户缓存隔离
12. **事件驱动缓存**: 支持基于事件的缓存更新机制

## 扩展说明

此技能提供完整的缓存解决方案，您可以根据需要进行扩展：

1. **自定义缓存提供程序**: 实现自定义缓存提供程序，支持更多缓存系统
2. **自定义过期策略**: 实现自定义缓存过期策略
3. **扩展缓存监控**: 扩展缓存监控和统计功能
4. **添加新的缓存功能**: 添加新的缓存功能，如缓存压缩、加密等
5. **优化性能**: 根据特定场景优化缓存性能
6. **集成其他系统**: 与其他系统集成，如消息队列、数据库等

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理缓存服务
2. **采用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **合理设置缓存大小**: 根据实际情况设置缓存大小限制
4. **使用合适的过期策略**: 根据数据特性选择合适的过期策略
5. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
6. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
7. **监控缓存性能**: 定期监控缓存命中率、内存占用等性能指标
8. **实现缓存穿透防护**: 对高频查询的不存在数据实现防护机制
9. **优化序列化**: 使用高效的序列化方式，如 MessagePack、Protobuf 等
10. **测试性能**: 定期测试缓存性能，确保满足需求

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

1. **使用 AOT 兼容的库**: 确保使用的缓存库支持 AOT 编译
2. **避免反射**: 避免在缓存操作中使用反射
3. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **序列化选择**: 使用 AOT 兼容的序列化库，如 MessagePack、Protobuf 等
6. **测试验证**: 在 AOT 编译后进行充分测试

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Program.cs 中配置
var builder = WebApplication.CreateBuilder(args);

// 注册缓存服务
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "WebApp:";
});

builder.Services.AddSingleton<ICacheService, CacheService>();

var app = builder.Build();

// 在控制器中使用
[ApiController]
[Route("[controller]")]
public class CacheController : ControllerBase
{
    private readonly ICacheService _cacheService;
    
    public CacheController(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var value = await _cacheService.GetAsync<string>(key);
        if (value == null)
        {
            return NotFound();
        }
        return Ok(value);
    }
}
```

### 与消息队列集成

```csharp
// 与 RabbitMQ 集成实现缓存更新
builder.Services.AddSingleton<ICacheInvalidationService, RabbitMQCacheInvalidationService>();

public class RabbitMQCacheInvalidationService : ICacheInvalidationService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ICacheService _cacheService;
    
    public RabbitMQCacheInvalidationService(ICacheService cacheService)
    {
        _cacheService = cacheService;
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "cache-invalidation", durable: false, exclusive: false, autoDelete: false, arguments: null);
        
        // 订阅缓存失效消息
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) => {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var cacheKey = message;
            await _cacheService.RemoveAsync(cacheKey);
        };
        _channel.BasicConsume(queue: "cache-invalidation", autoAck: true, consumer: consumer);
    }
    
    // 发布缓存失效消息
    public async Task InvalidateCacheAsync(string cacheKey)
    {
        var body = Encoding.UTF8.GetBytes(cacheKey);
        _channel.BasicPublish(exchange: "", routingKey: "cache-invalidation", basicProperties: null, body: body);
    }
}
```
