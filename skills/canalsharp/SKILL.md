# canalsharp Agent Skill - CanalSharp 技能

## 技能概述

基于 .NET 10 的高性能 CanalSharp 客户端技能，为 .NET 开发者提供强大的数据库变更捕获功能，支持 AOT（提前编译）编译，用于连接 Canal 服务器，实时捕获 MySQL 数据库变更，适用于构建实时数据同步、事件驱动架构等场景。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package CanalSharp@2.0.0
```

### 注册服务

在您的主应用程序中注册 CanalSharp 服务：

```csharp
// 注册 CanalSharp 服务
builder.Services.AddSingleton<ICanalClientFactory, CanalClientFactory>();
builder.Services.AddSingleton<ICanalService, CanalService>();
builder.Services.AddSingleton<IDatabaseChangeHandler, DatabaseChangeHandler>();

// 配置 Canal 客户端
builder.Services.Configure<CanalClientOptions>(options => {
    options.Host = "localhost";
    options.Port = 11111;
    options.Username = "canal";
    options.Password = "canal";
    options.Destination = "example";
    options.ClientId = "client1";
    options.BatchSize = 100;
    options.EnableAotOptimization = true;
});
```

### 使用示例

```csharp
// 获取 Canal 服务
var canalService = serviceProvider.GetRequiredService<ICanalService>();

// 启动 Canal 客户端
await canalService.StartAsync();

// 订阅数据库变更事件
canalService.OnDatabaseChanged += async (sender, args) => {
    Console.WriteLine($"数据库变更事件: {args.Database}.{args.Table}");
    Console.WriteLine($"变更类型: {args.ChangeType}");
    
    foreach (var row in args.Rows)
    {
        Console.WriteLine("变更数据:");
        foreach (var (column, value) in row)
        {
            Console.WriteLine($"  {column}: {value}");
        }
    }
    
    // 处理数据库变更，如更新缓存、发送消息等
    await ProcessDatabaseChangeAsync(args);
};

Console.WriteLine("CanalSharp 客户端已启动，正在监听数据库变更...");
Console.WriteLine("按 Ctrl+C 停止...");

// 等待取消
await Task.Delay(Timeout.Infinite, cancellationToken);
```

## 导航地图

```
canalsharp/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── canalsharp_integration.cs      # CanalSharp 集成示例
    ├── canalsharp_integration.run.json  # 运行配置
    └── canalsharp_integration.setting.json  # 设置文件
```

## 主要功能

1. **MySQL 数据库变更实时捕获**: 实时捕获 MySQL 数据库的 INSERT、UPDATE、DELETE 操作
2. **支持 Canal 协议**: 兼容 Canal 协议 v1.1.4+ 版本
3. **多种消息格式**: 支持 JSON、Protobuf 等多种消息格式
4. **断点续传**: 支持从上次断开的位置继续消费，避免数据丢失
5. **集群模式**: 支持 Canal 服务器集群模式，提高可用性
6. **AOT 编译优化**: 支持将应用编译为本机代码，提高运行时性能
7. **批量处理**: 支持批量处理变更事件，提高处理效率
8. **异步编程模型**: 基于异步编程模型，避免阻塞主线程
9. **灵活的事件处理**: 支持自定义事件处理逻辑，方便扩展
10. **完善的错误处理**: 包含重试机制和错误恢复功能

## 扩展说明

此技能提供完整的 CanalSharp 解决方案，您可以根据需要进行扩展：

1. **自定义事件处理器**: 实现 `IDatabaseChangeHandler` 接口，处理特定业务逻辑
2. **扩展消息格式**: 添加对新消息格式的支持
3. **集成消息队列**: 将变更事件发送到 Kafka、RabbitMQ 等消息队列
4. **添加监控指标**: 实现监控和统计功能，跟踪 CanalSharp 客户端性能
5. **实现自定义序列化**: 添加对自定义序列化方式的支持
6. **优化性能**: 根据特定场景优化性能，如调整批量大小、并发数等

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

1. **使用 AOT 兼容的库**: 确保使用的 CanalSharp 库支持 AOT 编译
2. **避免反射**: 避免在事件处理中使用反射
3. **使用 AOT 兼容的序列化库**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库
4. **资源加载**: 确保所有资源在 AOT 编译时能被正确处理
5. **动态代码生成**: 避免使用动态代码生成技术
6. **测试验证**: 在 AOT 编译后进行充分测试

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

### 与消息队列集成

```csharp
// 与 Kafka 集成，将数据库变更事件发送到 Kafka
public class KafkaDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly IProducer<string, string> _kafkaProducer;
    private readonly ILogger<KafkaDatabaseChangeHandler> _logger;
    
    public KafkaDatabaseChangeHandler(IProducer<string, string> kafkaProducer, ILogger<KafkaDatabaseChangeHandler> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        try
        {
            // 将变更事件序列化为 JSON
            var json = JsonSerializer.Serialize(@event);
            
            // 发送到 Kafka 主题
            await _kafkaProducer.ProduceAsync("database-changes", 
                new Message<string, string> {
                    Key = $"{@event.Database}.{@event.Table}",
                    Value = json
                });
            
            _logger.LogInformation("数据库变更事件已发送到 Kafka: {Database}.{Table}", @event.Database, @event.Table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送数据库变更事件到 Kafka 失败");
            throw;
        }
    }
}
```

### 与缓存集成

```csharp
// 与缓存集成，自动更新缓存
public class CacheDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheDatabaseChangeHandler> _logger;
    
    public CacheDatabaseChangeHandler(ICacheService cacheService, ILogger<CacheDatabaseChangeHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        try
        {
            // 根据变更类型更新缓存
            switch (@event.ChangeType)
            {
                case ChangeType.Insert:
                case ChangeType.Update:
                    // 插入或更新缓存
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
                    // 删除缓存
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
            
            _logger.LogInformation("缓存已根据数据库变更更新: {Database}.{Table}", @event.Database, @event.Table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新缓存失败");
            // 不抛出异常，避免影响其他处理器
        }
    }
}
```

## 接口定义

### ICanalService 接口

```csharp
public interface ICanalService
{
    // 启动 Canal 客户端
    Task StartAsync();
    
    // 停止 Canal 客户端
    Task StopAsync();
    
    // 数据库变更事件
    event EventHandler<DatabaseChangeEvent> OnDatabaseChanged;
    
    // 获取客户端状态
    CanalClientStatus GetStatus();
    
    // 手动提交位置
    Task CommitAsync();
}
```

### IDatabaseChangeHandler 接口

```csharp
public interface IDatabaseChangeHandler
{
    // 处理数据库变更事件
    Task HandleAsync(DatabaseChangeEvent @event);
}
```

### DatabaseChangeEvent 类

```csharp
public class DatabaseChangeEvent
{
    // 数据库名称
    public string Database { get; set; }
    
    // 表名称
    public string Table { get; set; }
    
    // 变更类型
    public ChangeType ChangeType { get; set; }
    
    // 变更行数据
    public List<Dictionary<string, object>> Rows { get; set; }
    
    // 变更时间
    public DateTime ChangeTime { get; set; }
    
    // 事件 ID
    public long EventId { get; set; }
}

public enum ChangeType
{
    Insert,
    Update,
    Delete
}
```

## 配置选项

### CanalClientOptions 类

```csharp
public class CanalClientOptions
{
    // Canal 服务器主机名
    public string Host { get; set; } = "localhost";
    
    // Canal 服务器端口
    public int Port { get; set; } = 11111;
    
    // 用户名
    public string Username { get; set; } = "canal";
    
    // 密码
    public string Password { get; set; } = "canal";
    
    // 目标名称
    public string Destination { get; set; } = "example";
    
    // 客户端 ID
    public string ClientId { get; set; } = "client1";
    
    // 批量大小
    public int BatchSize { get; set; } = 100;
    
    // 超时时间
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    
    // 是否启用 AOT 优化
    public bool EnableAotOptimization { get; set; } = true;
    
    // 是否启用断点续传
    public bool EnableResumable { get; set; } = true;
    
    // 心跳间隔
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(30);
}
```

## 故障排除

### 常见问题

1. **连接失败**
   - 检查 Canal 服务器是否正在运行
   - 检查主机名、端口、用户名、密码是否正确
   - 检查网络连接是否正常
   - 检查 Canal 服务器配置是否允许该客户端连接

2. **没有收到变更事件**
   - 检查 Canal 服务器是否正确配置了 MySQL 实例
   - 检查 MySQL 的 binlog 是否开启
   - 检查目标名称（destination）是否正确
   - 检查客户端 ID 是否唯一

3. **性能问题**
   - 调整批量大小（BatchSize）
   - 实现异步事件处理
   - 启用 AOT 编译优化
   - 考虑使用多线程处理事件

4. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 检查是否使用了反射等不兼容的特性
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT 编译

5. **数据丢失**
   - 确保启用了断点续传（EnableResumable = true）
   - 实现可靠的事件处理逻辑
   - 考虑使用持久化存储记录处理位置

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **调整批量大小**: 根据实际情况调整批量处理大小，平衡延迟和吞吐量
3. **实现异步事件处理**: 事件处理逻辑应尽量异步，避免阻塞 Canal 客户端主线程
4. **使用高效的序列化**: 使用高效的序列化库，如 MessagePack、Protobuf 等
5. **避免在事件处理中执行耗时操作**: 对于耗时操作，考虑将其放入队列异步处理
6. **实现背压机制**: 当事件处理速度跟不上产生速度时，实现背压机制
7. **使用连接池**: 如果需要连接外部系统，使用连接池管理连接
8. **监控性能指标**: 定期监控性能指标，如延迟、吞吐量、内存占用等

## 总结

CanalSharp 技能提供了一套完整的 .NET 10 数据库变更捕获解决方案，支持 AOT 编译优化，适用于构建实时数据同步、事件驱动架构等场景。通过遵循最佳实践和合理配置，可以构建出可靠、高效的数据库变更捕获系统。
