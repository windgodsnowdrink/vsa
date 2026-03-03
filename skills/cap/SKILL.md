# cap Agent Skill - CAP 技能

## 技能概述

基于 .NET 10 的高性能 CAP（分布式事务一致性）技能，为 .NET 开发者提供强大的分布式事务、事件总线和消息队列功能，支持 AOT（提前编译）编译，适用于构建高性能、可靠的分布式系统。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package DotNetCore.CAP@8.0.0
#:package DotNetCore.CAP.Redis@8.0.0
#:package DotNetCore.CAP.MySql@8.0.0
```

### 注册服务

在您的主应用程序中注册 CAP 服务：

```csharp
// 配置 CAP 服务
builder.Services.AddCap(options => {
    // 使用 Redis 作为消息队列
    options.UseRedis(redisOptions => {
        redisOptions.Configuration = "localhost:6379";
    });
    
    // 使用 MySQL 作为存储
    options.UseMySql(mySqlOptions => {
        mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
    });
    
    // 配置 CAP
    options.GroupName = "cap-example";
    options.DefaultGroupName = "cap-example";
    options.FailedRetryCount = 5;
    options.FailedRetryInterval = 60;
    options.EnableConsumerPrefetch = true;
    options.PrefetchCount = 100;
    options.EnableAotOptimization = true;
});

// 注册自定义 CAP 服务
builder.Services.AddSingleton<ICapService, CapService>();
builder.Services.AddSingleton<IDistributedTransactionService, DistributedTransactionService>();
```

### 使用示例

```csharp
// 获取 CAP 服务
var capService = serviceProvider.GetRequiredService<ICapService>();

// 发布事件
await capService.PublishAsync("order.created", new OrderCreatedEvent {
    OrderId = 1,
    ProductId = 1001,
    Quantity = 2,
    TotalAmount = 99.99,
    CreatedAt = DateTime.UtcNow
});

Console.WriteLine("事件已发布");

// 订阅事件（在控制器或服务中）
[CapSubscribe("order.created")]
public async Task HandleOrderCreatedAsync(OrderCreatedEvent @event)
{
    Console.WriteLine($"收到订单创建事件: 订单ID={@event.OrderId}, 产品ID={@event.ProductId}");
    
    // 处理订单创建事件，如库存扣减、通知用户等
    await ProcessOrderCreatedAsync(@event);
}

// 使用分布式事务
var transactionService = serviceProvider.GetRequiredService<IDistributedTransactionService>();

await transactionService.ExecuteAsync(async (transactionContext) => {
    // 1. 保存订单到数据库
    var order = new Order {
        Id = 1,
        ProductId = 1001,
        Quantity = 2,
        TotalAmount = 99.99,
        CreatedAt = DateTime.UtcNow
    };
    await _dbContext.Orders.AddAsync(order);
    await _dbContext.SaveChangesAsync();
    
    // 2. 发布订单创建事件（在事务内）
    await transactionContext.PublishAsync("order.created", new OrderCreatedEvent {
        OrderId = order.Id,
        ProductId = order.ProductId,
        Quantity = order.Quantity,
        TotalAmount = order.TotalAmount,
        CreatedAt = order.CreatedAt
    });
    
    // 3. 调用其他服务（可选）
    await _inventoryService.DeductAsync(order.ProductId, order.Quantity);
});
```

## 导航地图

```
cap/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── cap_redis_integration.cs       # Redis 集成示例
    ├── cap_redis_integration.run.json  # 运行配置
    ├── cap_redis_integration.setting.json  # 设置文件
    ├── dotnetcore_extension_integration.cs # .NET Core 扩展集成
    ├── dotnetcore_extension_integration.run.json # 运行配置
    └── dotnetcore_extension_integration.setting.json # 设置文件
```

## 主要功能

1. **分布式事务一致性保证**: 基于本地消息表 + 消息队列的分布式事务解决方案
2. **事件总线实现**: 提供可靠的事件发布和订阅机制
3. **支持多种消息队列**: Redis、RabbitMQ、Kafka 等
4. **支持多种数据库**: MySQL、PostgreSQL、SQL Server 等
5. **支持 AOT 编译优化**: 支持将应用编译为本机代码，提高运行时性能
6. **支持批量处理**: 支持批量发布和处理事件
7. **异步编程模型**: 基于异步编程模型，避免阻塞主线程
8. **事务补偿机制**: 支持失败重试和手动补偿
9. **监控和统计**: 提供事件处理状态、性能指标等监控功能
10. **支持集群模式**: 支持多实例部署，提高可用性
11. **灵活的配置**: 支持丰富的配置选项，适应不同场景

## 扩展说明

此技能提供完整的 CAP 解决方案，您可以根据需要进行扩展：

1. **自定义消息队列**: 实现 ICapPublisher 和 ICapSubscriber 接口，支持新的消息队列
2. **自定义存储**: 实现 IStorage 接口，支持新的数据库
3. **扩展事件处理**: 实现自定义事件过滤器、拦截器等
4. **添加监控指标**: 实现监控和统计功能，跟踪 CAP 性能
5. **实现自定义序列化**: 实现自定义序列化器，支持新的序列化格式
6. **优化性能**: 根据特定场景优化性能，如调整批量大小、并发数等

## 最佳实践

1. **使用依赖注入**: 始终使用依赖注入管理 CAP 服务
2. **采用异步 API**: 优先使用异步 API 避免阻塞主线程
3. **实现幂等处理**: 确保事件处理逻辑是幂等的，避免重复处理
4. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
5. **添加适当的日志**: 添加详细的日志记录，便于调试和监控
6. **设置合理的重试策略**: 根据业务需求设置适当的失败重试次数和间隔
7. **监控性能指标**: 监控 CAP 的性能指标，如事件延迟、成功率等
8. **使用合理的分组**: 根据业务模块设置合理的事件分组
9. **实现事务补偿**: 对于关键业务，实现手动补偿机制
10. **测试分布式事务**: 充分测试分布式事务场景，确保一致性

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
  <PackageReference Include="DotNetCore.CAP" Version="8.0.0" />
  <PackageReference Include="DotNetCore.CAP.Redis" Version="8.0.0" />
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

### AOT 编译注意事项

1. **使用 AOT 兼容的库**: 确保使用的 CAP 库版本支持 AOT 编译
2. **避免反射**: 避免在事件处理中使用反射
3. **使用 AOT 兼容的序列化库**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库
4. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
5. **动态代码生成**: 避免使用动态代码生成技术
6. **测试验证**: 在 AOT 编译后进行充分测试
7. **避免使用动态代理**: 避免使用 Castle DynamicProxy 等动态代理库
8. **使用 AOT 兼容的依赖注入**: 确保依赖注入容器支持 AOT 编译

## 与其他系统集成

### 与 ASP.NET Core 集成

```csharp
// 在 Program.cs 中配置
var builder = WebApplication.CreateBuilder(args);

// 配置 CAP
builder.Services.AddCap(options => {
    options.UseRedis(redisOptions => {
        redisOptions.Configuration = builder.Configuration.GetConnectionString("Redis");
    });
    options.UseMySql(mySqlOptions => {
        mySqlOptions.ConnectionString = builder.Configuration.GetConnectionString("MySQL");
    });
    options.GroupName = builder.Configuration["Cap:GroupName"];
    options.FailedRetryCount = 5;
    options.EnableAotOptimization = true;
});

var app = builder.Build();

// 启用 CAP 中间件
app.UseCap();

app.MapGet("/", () => "CAP 服务已启动");

await app.RunAsync();
```

### 与 EF Core 集成

```csharp
// 在 DbContext 中集成 CAP
public class AppDbContext : DbContext, ICapDbContext
{
    private readonly ICapTransaction _capTransaction;
    
    public AppDbContext(DbContextOptions<AppDbContext> options, ICapTransaction capTransaction)
        : base(options)
    {
        _capTransaction = capTransaction;
    }
    
    public ICapTransaction GetCapTransaction()
    {
        return _capTransaction;
    }
    
    // DbSet 属性...
}

// 在控制器中使用
[HttpPost("create")]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
{
    using var transaction = await _dbContext.Database.BeginTransactionAsync(_capTransaction);
    
    try
    {
        // 1. 保存订单
        var order = new Order {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        
        // 2. 发布事件（在事务内）
        await _capTransaction.PublishAsync("order.created", new OrderCreatedEvent {
            OrderId = order.Id,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalAmount = order.TotalAmount
        });
        
        // 3. 提交事务
        await transaction.CommitAsync();
        
        return Ok(order);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return StatusCode(500, ex.Message);
    }
}
```

### 与消息队列集成

```csharp
// 与 RabbitMQ 集成
builder.Services.AddCap(options => {
    options.UseRabbitMQ(rabbitOptions => {
        rabbitOptions.HostName = "localhost";
        rabbitOptions.UserName = "guest";
        rabbitOptions.Password = "guest";
        rabbitOptions.Port = 5672;
        rabbitOptions.VirtualHost = "/";
    });
    options.UseMySql(mySqlOptions => {
        mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
    });
});

// 与 Kafka 集成
builder.Services.AddCap(options => {
    options.UseKafka(kafkaOptions => {
        kafkaOptions.Servers = "localhost:9092";
        kafkaOptions.ConsumerGroup = "cap-consumer-group";
    });
    options.UseMySql(mySqlOptions => {
        mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
    });
});
```

## 接口定义

### ICapService 接口

```csharp
public interface ICapService
{
    // 发布事件
    Task PublishAsync<T>(string name, T content, CancellationToken cancellationToken = default);
    Task PublishAsync(string name, object content, string callbackName = null, CancellationToken cancellationToken = default);
    
    // 发布延迟事件
    Task PublishDelayAsync<T>(string name, T content, TimeSpan delay, CancellationToken cancellationToken = default);
    
    // 获取事件状态
    Task<EventStatus> GetEventStatusAsync(string eventId);
    
    // 重试失败事件
    Task RetryFailedEventAsync(string eventId);
    
    // 获取事件列表
    Task<IEnumerable<CapEvent>> GetEventsAsync(EventQuery query);
}
```

### IDistributedTransactionService 接口

```csharp
public interface IDistributedTransactionService
{
    // 执行分布式事务
    Task ExecuteAsync(Func<IDistributedTransactionContext, Task> action, CancellationToken cancellationToken = default);
    
    // 执行分布式事务（带返回值）
    Task<TResult> ExecuteAsync<TResult>(Func<IDistributedTransactionContext, Task<TResult>> action, CancellationToken cancellationToken = default);
}
```

### IDistributedTransactionContext 接口

```csharp
public interface IDistributedTransactionContext
{
    // 发布事件（在事务内）
    Task PublishAsync<T>(string name, T content, CancellationToken cancellationToken = default);
    
    // 获取事务 ID
    string TransactionId { get; }
    
    // 获取事务状态
    TransactionStatus Status { get; }
    
    // 添加事务日志
    void AddLog(string message);
    
    // 获取事务日志
    IEnumerable<string> GetLogs();
}
```

## 故障排除

### 常见问题

1. **事件发布失败**
   - 检查消息队列是否可用
   - 检查数据库连接是否正常
   - 检查事件内容是否可序列化
   - 检查 CAP 配置是否正确

2. **事件订阅失败**
   - 检查事件名称是否匹配
   - 检查订阅方法是否标记了 [CapSubscribe] 特性
   - 检查订阅服务是否被正确注册
   - 检查事件处理逻辑是否有异常

3. **分布式事务不一致**
   - 检查事务是否正确提交
   - 检查事件是否成功发布
   - 检查事件处理是否幂等
   - 考虑实现事务补偿机制

4. **性能问题**
   - 调整批量大小
   - 实现异步事件处理
   - 启用 AOT 编译优化
   - 考虑使用多线程处理事件
   - 检查数据库和消息队列性能

5. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容的特性
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT 编译

6. **CAP 启动失败**
   - 检查消息队列连接
   - 检查数据库连接
   - 检查配置文件
   - 查看 CAP 日志

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **调整批量大小**: 根据实际情况调整批量处理大小，平衡延迟和吞吐量
3. **实现异步事件处理**: 事件处理逻辑应尽量异步，避免阻塞主线程
4. **使用高效的序列化**: 使用高效的序列化库，如 MessagePack、Protobuf 等
5. **批量发布事件**: 对于大量事件，使用批量发布减少网络开销
6. **优化数据库性能**: 确保数据库配置优化，如索引、连接池等
7. **优化消息队列性能**: 确保消息队列配置优化，如分区、副本等
8. **使用合适的消息队列**: 根据业务需求选择合适的消息队列
9. **监控性能指标**: 定期监控性能指标，如事件延迟、成功率、吞吐量等
10. **使用缓存**: 对于频繁访问的数据，使用缓存减少数据库查询

## 总结

CAP 技能提供了一套完整的 .NET 10 分布式事务和事件总线解决方案，支持 AOT 编译优化，适用于构建高性能、可靠的分布式系统。通过遵循最佳实践和合理配置，可以构建出可靠、高效的分布式事务和事件驱动架构。

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持多种消息队列和数据库，能够满足不同场景的需求。
