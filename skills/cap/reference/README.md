# CAP - 参考文档

## 概述

CAP是一个基于.NET 10的高性能分布式事务一致性系统，专为.NET开发者设计，用于实现分布式事务、事件总线和消息队列功能，支持AOT（提前编译）编译。

## 核心组件

### 1. CAP服务
- **位置**: scripts/cap_redis_integration.cs
- **功能**: CAP核心业务逻辑处理
- **特性**: 
  - 分布式事务一致性保证
  - 事件发布与订阅
  - 支持多种消息队列和数据库
  - 高性能设计
  - 错误处理和重试机制
  - 详细日志记录
  - AOT编译支持

### 2. 分布式事务服务
- **位置**: scripts/dotnetcore_extension_integration.cs
- **功能**: 分布式事务管理
- **特性**: 
  - 事务上下文管理
  - 事务提交与回滚
  - 事件发布协调
  - 事务补偿机制
  - 幂等性保障

## 使用示例

### 基本使用

```csharp
// 获取CAP服务
var capService = serviceProvider.GetRequiredService<ICapService>();

// 发布事件
await capService.PublishAsync("order.created", new OrderCreatedEvent {
    OrderId = 1,
    ProductId = 1001,
    Quantity = 2,
    TotalAmount = 99.99
});
```

### 高级配置

```csharp
// 配置CAP服务
builder.Services.AddCap(options => {
    // 使用Redis作为消息队列
    options.UseRedis(redisOptions => {
        redisOptions.Configuration = "localhost:6379";
        redisOptions.InstanceName = "cap:";
    });
    
    // 使用MySQL作为存储
    options.UseMySql(mySqlOptions => {
        mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
        mySqlOptions.TableNamePrefix = "cap_";
    });
    
    // 配置高级选项
    options.GroupName = "cap-example";
    options.DefaultGroupName = "cap-example";
    options.FailedRetryCount = 5;
    options.FailedRetryInterval = 60;
    options.EnableConsumerPrefetch = true;
    options.PrefetchCount = 100;
    options.EnableAotOptimization = true;
    options.EnableDetailedLogging = false;
});
```

## 配置选项

### CAP配置

```json
{
  "Cap": {
    "GroupName": "cap-example",
    "DefaultGroupName": "cap-example",
    "FailedRetryCount": 5,
    "FailedRetryInterval": 60,
    "EnableConsumerPrefetch": true,
    "PrefetchCount": 100,
    "EnableAotOptimization": true,
    "EnableDetailedLogging": false,
    "EnableScheduledPublish": true,
    "ScheduledJobPollingInterval": 15
  }
}
```

## AOT编译支持

### AOT编译配置

在项目文件中添加以下配置以支持AOT编译：

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

### AOT编译命令

```bash
# 编译为Windows x64原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为Linux x64原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained
```

### AOT兼容性注意事项

1. 使用支持AOT的CAP版本（8.0.0+）
2. 避免在事件处理中使用反射
3. 使用System.Text.Json等AOT兼容的序列化库
4. 确保所有依赖都支持AOT编译
5. 编译后进行充分测试

## 性能优化

1. **启用AOT编译**: 提高运行时性能和启动速度
2. **使用异步API**: 避免阻塞主线程
3. **批量处理**: 批量发布和处理事件
4. **优化数据库性能**: 确保数据库配置优化，如索引、连接池等
5. **优化消息队列性能**: 根据实际情况调整消息队列配置
6. **实现幂等处理**: 确保事件处理逻辑是幂等的
7. **启用消费者预取**: 提高消费吞吐量

## 故障排除

### 常见问题

1. **连接失败**
   - 检查消息队列连接配置
   - 验证数据库连接字符串
   - 检查网络连接
   - 查看CAP日志

2. **事件发布失败**
   - 检查事件内容是否可序列化
   - 验证消息队列是否可用
   - 检查数据库状态
   - 查看详细日志

3. **分布式事务不一致**
   - 检查事务是否正确提交
   - 验证事件是否成功发布
   - 确保事件处理逻辑幂等
   - 考虑实现事务补偿机制

4. **AOT编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持AOT
   - 检查是否使用了反射等不兼容特性

## 扩展开发

### 添加自定义功能

```csharp
// 自定义CAP服务
public class CustomCapService : ICapService
{
    private readonly ICapService _innerCapService;
    private readonly ILogger<CustomCapService> _logger;
    
    public CustomCapService(ICapService innerCapService, ILogger<CustomCapService> logger)
    {
        _innerCapService = innerCapService;
        _logger = logger;
    }
    
    public async Task PublishAsync<T>(string name, T content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"准备发布事件: {name}");
        await _innerCapService.PublishAsync(name, content, cancellationToken);
        _logger.LogInformation($"事件发布成功: {name}");
    }
    
    // 实现其他接口方法...
}
```

### 自定义事件处理

```csharp
// 自定义事件处理器
public class CustomEventHandler
{
    [CapSubscribe("order.created", Group = "custom-group")]
    public async Task HandleOrderCreatedAsync(OrderCreatedEvent @event)
    {
        // 实现自定义事件处理逻辑
        await ProcessOrderAsync(@event);
    }
    
    [CapSubscribe("order.paid", Group = "custom-group")]
    public async Task HandleOrderPaidAsync(OrderPaidEvent @event)
    {
        // 实现自定义事件处理逻辑
        await ProcessPaymentAsync(@event);
    }
}
```
