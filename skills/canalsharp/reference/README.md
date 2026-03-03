# canalsharp - 参考文档

## 概述

canalsharp 是一个基于 .NET 10 的高性能 Canal 客户端系统，专为 .NET 开发者设计，用于连接 Canal 服务器，实时捕获 MySQL 数据库变更。

## 核心组件

### 1. Canal 客户端服务
- **位置**: scripts/canalsharp_integration.cs
- **功能**: 连接 Canal 服务器，接收和处理数据库变更事件
- **特性**: 
  - 支持 Canal 协议 v1.1.4+ 版本
  - 支持实时数据库变更捕获
  - 支持断点续传
  - 支持批量处理
  - 支持 AOT 编译优化
  - 完善的错误处理和重试机制
  - 支持集群模式

### 2. 数据库变更处理器
- **位置**: 应用程序中的 IDatabaseChangeHandler 实现
- **功能**: 处理接收到的数据库变更事件
- **特性**: 
  - 支持自定义事件处理逻辑
  - 支持异步处理
  - 支持多种变更类型（INSERT、UPDATE、DELETE）
  - 支持与其他系统集成

### 3. Canal 客户端工厂
- **位置**: 应用程序中的 ICanalClientFactory 实现
- **功能**: 创建和管理 Canal 客户端实例
- **特性**: 
  - 支持创建多个 Canal 客户端实例
  - 支持客户端生命周期管理
  - 支持依赖注入
  - 支持配置管理

## 使用示例

### 基本使用

```csharp
// 初始化服务容器
var serviceProvider = BuildServiceProvider();
var canalService = serviceProvider.GetRequiredService<ICanalService>();

// 启动 Canal 客户端
await canalService.StartAsync();

// 订阅数据库变更事件
canalService.OnDatabaseChanged += async (sender, args) => {
    Console.WriteLine($"数据库: {args.Database}, 表: {args.Table}");
    Console.WriteLine($"变更类型: {args.ChangeType}");
    
    foreach (var row in args.Rows)
    {
        Console.WriteLine("变更数据:");
        foreach (var (column, value) in row)
        {
            Console.WriteLine($"  {column}: {value}");
        }
    }
    
    // 处理数据库变更
    await ProcessChangeAsync(args);
};

// 等待取消
await Task.Delay(Timeout.Infinite, cancellationToken);
```

### 高级配置

```csharp
// 配置 Canal 客户端
var options = new CanalClientOptions {
    Host = "localhost",
    Port = 11111,
    Username = "canal",
    Password = "canal",
    Destination = "example",
    ClientId = "client1",
    BatchSize = 100,
    Timeout = TimeSpan.FromSeconds(30),
    EnableAotOptimization = true,
    EnableResumable = true,
    HeartbeatInterval = TimeSpan.FromSeconds(30)
};

// 注册服务
var builder = new ServiceCollection();
builder.AddSingleton<ICanalClientFactory, CanalClientFactory>();
builder.AddSingleton<ICanalService, CanalService>();
builder.AddSingleton<IDatabaseChangeHandler, DatabaseChangeHandler>();
builder.AddSingleton(options);

var serviceProvider = builder.BuildServiceProvider();
```

### 自定义事件处理器

```csharp
// 实现自定义事件处理器
public class CustomDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly ILogger<CustomDatabaseChangeHandler> _logger;
    private readonly IMessageQueueService _messageQueueService;
    private readonly ICacheService _cacheService;
    
    public CustomDatabaseChangeHandler(
        ILogger<CustomDatabaseChangeHandler> logger,
        IMessageQueueService messageQueueService,
        ICacheService cacheService)
    {
        _logger = logger;
        _messageQueueService = messageQueueService;
        _cacheService = cacheService;
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        try
        {
            _logger.LogInformation("处理数据库变更事件: {Database}.{Table}, 变更类型: {ChangeType}", 
                @event.Database, @event.Table, @event.ChangeType);
            
            // 1. 发送到消息队列
            await _messageQueueService.PublishAsync("database-changes", @event);
            
            // 2. 更新缓存
            await UpdateCacheAsync(@event);
            
            // 3. 其他业务逻辑
            await ProcessBusinessLogicAsync(@event);
            
            _logger.LogInformation("数据库变更事件处理完成: {Database}.{Table}", @event.Database, @event.Table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理数据库变更事件失败: {Database}.{Table}", @event.Database, @event.Table);
            // 实现重试逻辑
            await RetryHandleAsync(@event, ex);
        }
    }
    
    private async Task UpdateCacheAsync(DatabaseChangeEvent @event)
    {
        // 根据变更类型更新缓存
        switch (@event.ChangeType)
        {
            case ChangeType.Insert:
            case ChangeType.Update:
                // 移除相关缓存
                foreach (var row in @event.Rows)
                {
                    if (row.TryGetValue("Id", out var id))
                    {
                        var cacheKey = $"{@event.Table}:{id}";
                        await _cacheService.RemoveAsync(cacheKey);
                    }
                }
                break;
            case ChangeType.Delete:
                // 移除相关缓存
                foreach (var row in @event.Rows)
                {
                    if (row.TryGetValue("Id", out var id))
                    {
                        var cacheKey = $"{@event.Table}:{id}";
                        await _cacheService.RemoveAsync(cacheKey);
                    }
                }
                break;
        }
    }
    
    // 其他方法实现...
}
```

## 配置选项

### Canal 客户端配置

```json
{
  "Canal": {
    "Host": "localhost",           // Canal 服务器主机名
    "Port": 11111,                 // Canal 服务器端口
    "Username": "canal",          // 用户名
    "Password": "canal",          // 密码
    "Destination": "example",     // 目标名称
    "ClientId": "client1",        // 客户端 ID
    "BatchSize": 100,              // 批量大小
    "Timeout": "00:00:30",        // 超时时间
    "EnableAotOptimization": true, // 启用 AOT 优化
    "EnableResumable": true,       // 启用断点续传
    "HeartbeatInterval": "00:00:30", // 心跳间隔
    "EnableDetailedLogging": false  // 启用详细日志
  }
}
```

### 应用程序配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "CanalSharp": "Debug",
      "System": "Warning",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "*"
}
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

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
  <PackageReference Include="System.Text.Json" Version="10.0.0" />
  <PackageReference Include="CanalSharp" Version="2.0.0" />
</ItemGroup>
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

### AOT 兼容注意事项

1. **使用 AOT 兼容的库**: 确保使用的 CanalSharp 库版本支持 AOT 编译
2. **避免反射**: 避免在事件处理中使用反射，尤其是在序列化和反序列化过程中
3. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库
4. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
5. **动态代码生成**: 避免使用动态代码生成技术，如 System.Reflection.Emit
6. **泛型类型**: 限制使用复杂的泛型类型，尤其是在 Canal 客户端配置中
7. **测试验证**: 在 AOT 编译后进行充分测试，确保所有功能正常工作
8. **避免使用 HttpContext**: 在 AOT 编译中，HttpContext 相关的功能可能受限

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **调整批量大小**: 根据实际情况调整批量处理大小，平衡延迟和吞吐量
3. **实现异步事件处理**: 事件处理逻辑应尽量异步，避免阻塞 Canal 客户端主线程
4. **使用高效的序列化**: 使用高效的序列化库，如 MessagePack、Protobuf 等
5. **批量处理事件**: 实现事件批处理，减少外部系统调用次数
6. **实现背压机制**: 当事件处理速度跟不上产生速度时，实现背压机制
7. **使用连接池**: 如果需要连接外部系统，使用连接池管理连接
8. **优化网络配置**: 调整网络参数，如 TCP 缓冲区大小、超时时间等
9. **监控性能指标**: 定期监控性能指标，如延迟、吞吐量、内存占用等
10. **使用多线程处理**: 对于高并发场景，考虑使用多线程处理事件

## 故障排除

### 常见问题

1. **连接失败**
   - 检查 Canal 服务器是否正在运行
   - 检查主机名、端口、用户名、密码是否正确
   - 检查网络连接是否正常
   - 检查 Canal 服务器配置是否允许该客户端连接
   - 检查防火墙设置是否允许连接

2. **没有收到变更事件**
   - 检查 Canal 服务器是否正确配置了 MySQL 实例
   - 检查 MySQL 的 binlog 是否开启
   - 检查 MySQL 的 binlog 格式是否为 ROW 格式
   - 检查目标名称（destination）是否正确
   - 检查客户端 ID 是否唯一
   - 检查 Canal 服务器日志是否有错误信息

3. **性能问题**
   - 调整批量大小（BatchSize）
   - 实现异步事件处理
   - 启用 AOT 编译优化
   - 考虑使用多线程处理事件
   - 优化事件处理逻辑
   - 检查网络延迟

4. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容的特性
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT 编译
   - 检查是否有未处理的警告

5. **数据丢失**
   - 确保启用了断点续传（EnableResumable = true）
   - 实现可靠的事件处理逻辑
   - 考虑使用持久化存储记录处理位置
   - 实现事件确认机制

6. **客户端频繁断开连接**
   - 检查网络稳定性
   - 调整心跳间隔（HeartbeatInterval）
   - 检查 Canal 服务器配置
   - 检查客户端超时设置

## 扩展开发

### 自定义事件处理器

```csharp
// 实现 IDatabaseChangeHandler 接口
public class MyDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly ILogger<MyDatabaseChangeHandler> _logger;
    
    public MyDatabaseChangeHandler(ILogger<MyDatabaseChangeHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        // 自定义事件处理逻辑
        _logger.LogInformation("处理数据库变更: {Database}.{Table}", @event.Database, @event.Table);
        
        // 实现业务逻辑
        await ProcessChangeAsync(@event);
    }
    
    private async Task ProcessChangeAsync(DatabaseChangeEvent @event)
    {
        // 业务逻辑实现
    }
}

// 注册自定义处理器
builder.Services.AddSingleton<IDatabaseChangeHandler, MyDatabaseChangeHandler>();
```

### 扩展 Canal 客户端

```csharp
// 扩展 Canal 客户端
public class CustomCanalClient : DefaultCanalClient
{
    private readonly ILogger<CustomCanalClient> _logger;
    
    public CustomCanalClient(CanalClientOptions options, ILogger<CustomCanalClient> logger)
        : base(options, logger)
    {
        _logger = logger;
    }
    
    // 重写方法，添加自定义逻辑
    protected override async Task OnMessageReceivedAsync(Message message)
    {
        _logger.LogDebug("收到消息: {MessageId}, 条目数: {EntriesCount}", message.Id, message.Entries.Count);
        
        // 添加自定义逻辑
        await base.OnMessageReceivedAsync(message);
    }
    
    // 添加新方法
    public async Task CustomMethodAsync()
    {
        // 自定义方法实现
    }
}

// 注册自定义客户端
builder.Services.AddSingleton<ICanalClient, CustomCanalClient>();
builder.Services.AddSingleton<ICanalClientFactory, CustomCanalClientFactory>();
```

### 集成消息队列

```csharp
// 与 RabbitMQ 集成
public class RabbitMQDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMQDatabaseChangeHandler> _logger;
    
    public RabbitMQDatabaseChangeHandler(ILogger<RabbitMQDatabaseChangeHandler> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // 声明队列
        _channel.QueueDeclare(queue: "database-changes", durable: true, exclusive: false, autoDelete: false, arguments: null);
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        try
        {
            // 序列化事件
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);
            
            // 发送消息
            _channel.BasicPublish(exchange: "", routingKey: "database-changes", basicProperties: null, body: body);
            
            _logger.LogInformation("数据库变更事件已发送到 RabbitMQ: {Database}.{Table}", @event.Database, @event.Table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送数据库变更事件到 RabbitMQ 失败");
            throw;
        }
    }
}
```

## 部署建议

1. **容器化部署**: 使用 Docker 容器化应用，便于部署和扩展
2. **使用 Kubernetes**: 对于分布式应用，使用 Kubernetes 进行编排
3. **使用 CI/CD 流程**: 实现自动化构建、测试和部署
4. **使用环境变量**: 使用环境变量配置应用，便于不同环境部署
5. **实施监控**: 实施监控和告警，及时发现问题
6. **高可用部署**: 部署多个 Canal 客户端实例，提高可用性
7. **数据备份**: 对于重要数据，实施适当的备份策略
8. **使用 AOT 编译**: 对于生产环境，考虑使用 AOT 编译提高性能
9. **优化资源配置**: 根据实际负载优化 CPU、内存等资源配置
10. **实施安全措施**: 实施适当的安全措施，如加密、访问控制等

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 CanalSharp 服务
2. **采用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **实现幂等处理**: 确保事件处理逻辑是幂等的，避免重复处理
4. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
5. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
6. **实现错误重试机制**: 对外部系统调用实现重试机制
7. **监控性能指标**: 监控 CanalSharp 客户端的性能指标，如延迟、吞吐量等
8. **合理设置批量大小**: 根据实际情况调整批量处理大小
9. **实现断点续传**: 确保客户端支持断点续传，避免数据丢失
10. **测试恢复机制**: 测试客户端的错误恢复机制，确保在网络故障等情况下能够正常恢复
11. **使用配置管理**: 使用配置管理系统管理 Canal 客户端配置
12. **实现优雅关闭**: 实现优雅关闭机制，确保在关闭时正确处理未完成的任务

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Program.cs 中配置
var builder = WebApplication.CreateBuilder(args);

// 注册 CanalSharp 服务
builder.Services.AddSingleton<ICanalClientFactory, CanalClientFactory>();
builder.Services.AddSingleton<ICanalService, CanalService>();
builder.Services.AddSingleton<IDatabaseChangeHandler, DatabaseChangeHandler>();

builder.Services.Configure<CanalClientOptions>(builder.Configuration.GetSection("Canal"));

var app = builder.Build();

// 启动 Canal 服务
var canalService = app.Services.GetRequiredService<ICanalService>();
await canalService.StartAsync();

app.MapGet("/", () => "CanalSharp 服务已启动");

await app.RunAsync();
```

### 与缓存系统集成

```csharp
// 与 Redis 缓存集成
public class RedisCacheDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly IDistributedCache _redisCache;
    private readonly ILogger<RedisCacheDatabaseChangeHandler> _logger;
    
    public RedisCacheDatabaseChangeHandler(IDistributedCache redisCache, ILogger<RedisCacheDatabaseChangeHandler> logger)
    {
        _redisCache = redisCache;
        _logger = logger;
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        try
        {
            // 根据变更类型更新缓存
            foreach (var row in @event.Rows)
            {
                if (row.TryGetValue("Id", out var id))
                {
                    var cacheKey = $"{@event.Table}:{id}";
                    await _redisCache.RemoveAsync(cacheKey);
                    _logger.LogInformation("缓存已移除: {CacheKey}", cacheKey);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新 Redis 缓存失败");
            // 不抛出异常，避免影响其他处理器
        }
    }
}
```

## 总结

CanalSharp 参考文档提供了关于 CanalSharp 系统的详细信息，包括核心组件、使用示例、配置选项、AOT 编译支持、性能优化、故障排除、扩展开发和部署建议等。通过遵循最佳实践和合理配置，可以构建出可靠、高效的数据库变更捕获系统，适用于各种实时数据同步、事件驱动架构等场景。

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持 AOT 编译优化，能够满足高性能、高可用的业务需求。
