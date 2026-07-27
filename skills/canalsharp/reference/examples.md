# canalsharp - 使用示例

## 快速入门

### 1. 基本使用示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using CanalSharp.Client.Simple;
using CanalSharp.Client;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var canalService = serviceProvider.GetRequiredService<ICanalService>();
        
        Console.WriteLine("CanalSharp 基本使用示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 启动 Canal 客户端
            await canalService.StartAsync();
            
            Console.WriteLine("Canal 客户端已启动，正在监听数据库变更...");
            Console.WriteLine("按 Ctrl+C 停止...");
            
            // 订阅数据库变更事件
            canalService.OnDatabaseChanged += async (sender, args) => {
                Console.WriteLine($"\n数据库变更事件");
                Console.WriteLine($"- 数据库: {args.Database}");
                Console.WriteLine($"- 表: {args.Table}");
                Console.WriteLine($"- 变更类型: {args.ChangeType}");
                Console.WriteLine($"- 变更时间: {args.ChangeTime}");
                
                Console.WriteLine("- 变更数据:");
                foreach (var row in args.Rows)
                {
                    Console.WriteLine("  行数据:");
                    foreach (var (column, value) in row)
                    {
                        Console.WriteLine($"    {column}: {value}");
                    }
                }
                
                // 处理数据库变更
                await ProcessDatabaseChangeAsync(args);
            };
            
            // 等待取消信号
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("\n正在停止 Canal 客户端...");
            };
            
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("\n任务已取消");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n发生错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            // 停止 Canal 客户端
            await canalService.StopAsync();
            Console.WriteLine("Canal 客户端已停止");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 添加日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 配置 Canal 客户端选项
        builder.Configure<CanalClientOptions>(options => {
            options.Host = "localhost";
            options.Port = 11111;
            options.Username = "canal";
            options.Password = "canal";
            options.Destination = "example";
            options.ClientId = "client1";
            options.BatchSize = 100;
            options.EnableAotOptimization = true;
        });
        
        // 注册 Canal 服务
        builder.AddSingleton<ICanalClientFactory, CanalClientFactory>();
        builder.AddSingleton<ICanalService, CanalService>();
        builder.AddSingleton<IDatabaseChangeHandler, DatabaseChangeHandler>();
        
        return builder.BuildServiceProvider();
    }
    
    private static async Task ProcessDatabaseChangeAsync(DatabaseChangeEvent @event)
    {
        // 在这里实现你的数据库变更处理逻辑
        // 例如：更新缓存、发送消息、同步数据等
        await Task.CompletedTask;
    }
}

// 数据库变更事件类
public class DatabaseChangeEvent
{
    public string Database { get; set; }
    public string Table { get; set; }
    public ChangeType ChangeType { get; set; }
    public List<Dictionary<string, object>> Rows { get; set; }
    public DateTime ChangeTime { get; set; }
    public long EventId { get; set; }
}

public enum ChangeType
{
    Insert,
    Update,
    Delete
}

// Canal 服务接口
public interface ICanalService
{
    Task StartAsync();
    Task StopAsync();
    event EventHandler<DatabaseChangeEvent> OnDatabaseChanged;
    CanalClientStatus GetStatus();
    Task CommitAsync();
}

public enum CanalClientStatus
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Error
}

// Canal 客户端工厂接口
public interface ICanalClientFactory
{
    ICanalClient CreateClient(CanalClientOptions options);
}

// 数据库变更处理器接口
public interface IDatabaseChangeHandler
{
    Task HandleAsync(DatabaseChangeEvent @event);
}

// 简单的数据库变更处理器实现
public class DatabaseChangeHandler : IDatabaseChangeHandler
{
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        // 实现数据库变更处理逻辑
        await Task.CompletedTask;
    }
}

// 简单的 Canal 服务实现
public class CanalService : ICanalService
{
    private readonly ICanalClientFactory _clientFactory;
    private readonly CanalClientOptions _options;
    private ICanalClient _client;
    private bool _isRunning;
    private Task _processingTask;
    private CancellationTokenSource _cts;
    
    public event EventHandler<DatabaseChangeEvent> OnDatabaseChanged;
    
    public CanalService(ICanalClientFactory clientFactory, IOptions<CanalClientOptions> options)
    {
        _clientFactory = clientFactory;
        _options = options.Value;
    }
    
    public async Task StartAsync()
    {
        if (_isRunning)
        {
            return;
        }
        
        _cts = new CancellationTokenSource();
        _client = _clientFactory.CreateClient(_options);
        
        // 连接 Canal 服务器
        await _client.ConnectAsync();
        
        // 订阅所有数据库和表
        await _client.SubscribeAsync("*.*");
        
        _isRunning = true;
        _processingTask = Task.Run(() => ProcessMessagesAsync(_cts.Token));
    }
    
    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                // 获取消息
                var message = await _client.GetAsync(_options.BatchSize);
                
                if (message == null || message.Entries.Count == 0)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }
                
                // 处理消息
                foreach (var entry in message.Entries)
                {
                    // 过滤事务开始和结束的条目
                    if (entry.EntryType == EntryType.Transactionbegin || entry.EntryType == EntryType.Transactionend)
                    {
                        continue;
                    }
                    
                    // 解析行变更
                    var rowChange = RowChange.Parser.ParseFrom(entry.StoreValue);
                    
                    // 创建数据库变更事件
                    var changeEvent = new DatabaseChangeEvent
                    {
                        Database = entry.Header.SchemaName,
                        Table = entry.Header.TableName,
                        ChangeType = (ChangeType)rowChange.EventType,
                        ChangeTime = DateTime.UtcNow,
                        EventId = entry.Header.LogfileName.GetHashCode() + entry.Header.LogfileOffset,
                        Rows = new List<Dictionary<string, object>>()
                    };
                    
                    // 处理行数据
                    foreach (var rowData in rowChange.RowDatas)
                    {
                        var row = new Dictionary<string, object>();
                        
                        // 根据变更类型获取行数据
                        var columns = rowChange.EventType == EventType.Insert ? rowData.AfterColumnsList : 
                                      rowChange.EventType == EventType.Delete ? rowData.BeforeColumnsList : 
                                      rowData.AfterColumnsList;
                        
                        foreach (var column in columns)
                        {
                            row[column.Name] = column.Value;
                        }
                        
                        changeEvent.Rows.Add(row);
                    }
                    
                    // 触发事件
                    OnDatabaseChanged?.Invoke(this, changeEvent);
                }
                
                // 提交位置
                await _client.AckAsync(message.Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"处理消息时发生错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    public async Task StopAsync()
    {
        if (!_isRunning)
        {
            return;
        }
        
        _isRunning = false;
        _cts?.Cancel();
        
        if (_processingTask != null)
        {
            await _processingTask;
        }
        
        if (_client != null)
        {
            await _client.DisconnectAsync();
        }
        
        _cts?.Dispose();
    }
    
    public CanalClientStatus GetStatus()
    {
        return _isRunning ? CanalClientStatus.Running : CanalClientStatus.Stopped;
    }
    
    public async Task CommitAsync()
    {
        if (_client != null)
        {
            await _client.AckAsync(0);
        }
    }
}

// 简单的 Canal 客户端工厂实现
public class CanalClientFactory : ICanalClientFactory
{
    public ICanalClient CreateClient(CanalClientOptions options)
    {
        return new SimpleCanalClient(
            options.Host,
            options.Port,
            options.Username,
            options.Password,
            options.Destination,
            options.ClientId
        );
    }
}

// Canal 客户端选项类
public class CanalClientOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 11111;
    public string Username { get; set; } = "canal";
    public string Password { get; set; } = "canal";
    public string Destination { get; set; } = "example";
    public string ClientId { get; set; } = "client1";
    public int BatchSize { get; set; } = 100;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableAotOptimization { get; set; } = true;
    public bool EnableResumable { get; set; } = true;
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(30);
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("CanalSharp 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建配置
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 添加日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.AddConfiguration(configuration.GetSection("Logging"));
        });
        
        // 配置 Canal 客户端
        builder.Configure<CanalClientOptions>(configuration.GetSection("Canal"));
        
        // 注册服务
        builder.AddSingleton<ICanalClientFactory, AdvancedCanalClientFactory>();
        builder.AddSingleton<ICanalService, AdvancedCanalService>();
        builder.AddSingleton<IDatabaseChangeHandler, AdvancedDatabaseChangeHandler>();
        builder.AddSingleton<IMessageQueueService, RabbitMQMessageQueueService>();
        builder.AddSingleton<ICacheService, RedisCacheService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var canalOptions = serviceProvider.GetRequiredService<IOptions<CanalClientOptions>>().Value;
        Console.WriteLine("Canal 客户端配置:");
        Console.WriteLine($"- 主机: {canalOptions.Host}");
        Console.WriteLine($"- 端口: {canalOptions.Port}");
        Console.WriteLine($"- 目标: {canalOptions.Destination}");
        Console.WriteLine($"- 客户端 ID: {canalOptions.ClientId}");
        Console.WriteLine($"- 批量大小: {canalOptions.BatchSize}");
        Console.WriteLine($"- 启用 AOT 优化: {canalOptions.EnableAotOptimization}");
        
        // 获取并使用服务
        var canalService = serviceProvider.GetRequiredService<ICanalService>();
        
        // 启动 Canal 客户端
        await canalService.StartAsync();
        
        Console.WriteLine("\n高级 Canal 客户端已启动");
        Console.WriteLine("按 Ctrl+C 停止...");
        
        // 等待取消
        await Task.Delay(Timeout.Infinite, new CancellationToken());
    }
}

// 高级 Canal 客户端工厂
public class AdvancedCanalClientFactory : ICanalClientFactory
{
    private readonly ILogger<AdvancedCanalClientFactory> _logger;
    
    public AdvancedCanalClientFactory(ILogger<AdvancedCanalClientFactory> logger)
    {
        _logger = logger;
    }
    
    public ICanalClient CreateClient(CanalClientOptions options)
    {
        _logger.LogInformation("创建高级 Canal 客户端");
        
        // 可以在这里实现更复杂的客户端创建逻辑
        // 例如：根据配置选择不同的客户端实现
        
        return new SimpleCanalClient(
            options.Host,
            options.Port,
            options.Username,
            options.Password,
            options.Destination,
            options.ClientId
        );
    }
}

// 高级 Canal 服务
public class AdvancedCanalService : CanalService
{
    private readonly ILogger<AdvancedCanalService> _logger;
    private readonly IDatabaseChangeHandler _changeHandler;
    
    public AdvancedCanalService(
        ICanalClientFactory clientFactory,
        IOptions<CanalClientOptions> options,
        ILogger<AdvancedCanalService> logger,
        IDatabaseChangeHandler changeHandler)
        : base(clientFactory, options)
    {
        _logger = logger;
        _changeHandler = changeHandler;
        
        // 订阅变更事件并使用处理器处理
        this.OnDatabaseChanged += async (sender, args) => {
            try
            {
                await _changeHandler.HandleAsync(args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理数据库变更事件失败");
            }
        };
    }
}

// 高级数据库变更处理器
public class AdvancedDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly ILogger<AdvancedDatabaseChangeHandler> _logger;
    private readonly IMessageQueueService _messageQueueService;
    private readonly ICacheService _cacheService;
    
    public AdvancedDatabaseChangeHandler(
        ILogger<AdvancedDatabaseChangeHandler> logger,
        IMessageQueueService messageQueueService,
        ICacheService cacheService)
    {
        _logger = logger;
        _messageQueueService = messageQueueService;
        _cacheService = cacheService;
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        _logger.LogInformation("处理数据库变更事件: {Database}.{Table}", @event.Database, @event.Table);
        
        try
        {
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
            throw;
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
    
    private async Task ProcessBusinessLogicAsync(DatabaseChangeEvent @event)
    {
        // 实现自定义业务逻辑
        await Task.CompletedTask;
    }
}

// 消息队列服务接口
public interface IMessageQueueService
{
    Task PublishAsync<T>(string topic, T message);
}

// RabbitMQ 消息队列服务实现
public class RabbitMQMessageQueueService : IMessageQueueService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    
    public RabbitMQMessageQueueService()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // 声明交换机
        _channel.ExchangeDeclare(exchange: "database-changes-exchange", type: ExchangeType.Topic);
        
        // 声明队列
        _channel.QueueDeclare(queue: "database-changes", durable: true, exclusive: false, autoDelete: false, arguments: null);
        
        // 绑定队列到交换机
        _channel.QueueBind(queue: "database-changes", exchange: "database-changes-exchange", routingKey: "database.*");
    }
    
    public async Task PublishAsync<T>(string topic, T message)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(message);
        var body = System.Text.Encoding.UTF8.GetBytes(json);
        
        _channel.BasicPublish(
            exchange: "database-changes-exchange",
            routingKey: topic,
            basicProperties: null,
            body: body);
        
        await Task.CompletedTask;
    }
}

// 缓存服务接口
public interface ICacheService
{
    Task SetAsync<T>(string key, T value, TimeSpan expirationTime);
    Task<T> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}

// Redis 缓存服务实现
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    
    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, json, new DistributedCacheEntryOptions {
            AbsoluteExpirationRelativeToNow = expirationTime
        });
    }
    
    public async Task<T> GetAsync<T>(string key)
    {
        var json = await _distributedCache.GetStringAsync(key);
        return json != null ? System.Text.Json.JsonSerializer.Deserialize<T>(json) : default;
    }
    
    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}
```

### 3. AOT 编译示例

```csharp
// 这是一个支持 AOT 编译的 CanalSharp 示例
// 项目文件需要包含 AOT 配置

#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package CanalSharp@2.0.0
#:package System.Text.Json@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("CanalSharp AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var canalService = serviceProvider.GetRequiredService<ICanalService>();
        
        try
        {
            // 启动 Canal 客户端
            await canalService.StartAsync();
            
            Console.WriteLine("AOT 编译的 Canal 客户端已启动");
            Console.WriteLine("按 Ctrl+C 停止...");
            
            // 订阅数据库变更事件
            canalService.OnDatabaseChanged += async (sender, args) => {
                Console.WriteLine($"\n数据库变更: {args.Database}.{args.Table}");
                Console.WriteLine($"变更类型: {args.ChangeType}");
                
                // 处理变更（使用 AOT 兼容的方式）
                await ProcessChangeAotCompatibleAsync(args);
            };
            
            // 等待取消
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
            };
            
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("\n任务已取消");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n错误: {ex.Message}");
        }
        finally
        {
            await canalService.StopAsync();
            Console.WriteLine("Canal 客户端已停止");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 添加日志服务（AOT 兼容）
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 配置 Canal 客户端（AOT 兼容）
        builder.Configure<CanalClientOptions>(options => {
            options.Host = "localhost";
            options.Port = 11111;
            options.Username = "canal";
            options.Password = "canal";
            options.Destination = "example";
            options.ClientId = "aot-client";
            options.BatchSize = 100;
            options.EnableAotOptimization = true;
        });
        
        // 注册服务（AOT 兼容，避免使用反射）
        builder.AddSingleton<ICanalClientFactory, AotCompatibleCanalClientFactory>();
        builder.AddSingleton<ICanalService, AotCompatibleCanalService>();
        
        return builder.BuildServiceProvider();
    }
    
    // AOT 兼容的变更处理方法
    private static async Task ProcessChangeAotCompatibleAsync(DatabaseChangeEvent @event)
    {
        // 使用 AOT 兼容的方式处理变更
        // 避免使用反射、动态类型等不兼容特性
        
        // 示例：写入到文件（AOT 兼容）
        var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {event.Database}.{event.Table} | {event.ChangeType}\n";
        
        // 使用 File.AppendAllTextAsync 是 AOT 兼容的
        await File.AppendAllTextAsync("database-changes.log", logEntry, CancellationToken.None);
        
        await Task.CompletedTask;
    }
}

// AOT 兼容的 Canal 客户端工厂
public class AotCompatibleCanalClientFactory : ICanalClientFactory
{
    public ICanalClient CreateClient(CanalClientOptions options)
    {
        // 创建 AOT 兼容的 Canal 客户端
        return new AotCompatibleCanalClient(options);
    }
}

// AOT 兼容的 Canal 客户端
public class AotCompatibleCanalClient : ICanalClient
{
    private readonly CanalClientOptions _options;
    // 实现 AOT 兼容的 Canal 客户端
    
    public AotCompatibleCanalClient(CanalClientOptions options)
    {
        _options = options;
    }
    
    // 实现 ICanalClient 接口的 AOT 兼容方法
    // ...
}

// AOT 兼容的 Canal 服务
public class AotCompatibleCanalService : ICanalService
{
    // 实现 AOT 兼容的 Canal 服务
    // ...
}
```

### 4. 错误处理示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("CanalSharp 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 添加日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
        });
        
        // 配置 Canal 客户端
        builder.Configure<CanalClientOptions>(options => {
            options.Host = "localhost";
            options.Port = 11111;
            options.Username = "canal";
            options.Password = "canal";
            options.Destination = "example";
            options.ClientId = "error-handling-client";
        });
        
        // 注册服务
        builder.AddSingleton<ICanalClientFactory, CanalClientFactory>();
        builder.AddSingleton<ICanalService, ErrorHandlingCanalService>();
        builder.AddSingleton<IDatabaseChangeHandler, ErrorHandlingDatabaseChangeHandler>();
        
        var serviceProvider = builder.BuildServiceProvider();
        var canalService = serviceProvider.GetRequiredService<ICanalService>();
        
        try
        {
            await canalService.StartAsync();
            Console.WriteLine("Canal 客户端已启动，带错误处理");
            Console.WriteLine("按 Ctrl+C 停止...");
            
            // 等待取消
            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) => cts.Cancel();
            
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"严重错误: {ex.Message}");
        }
        finally
        {
            await canalService.StopAsync();
        }
    }
}

// 带错误处理的 Canal 服务
public class ErrorHandlingCanalService : CanalService
{
    private readonly ILogger<ErrorHandlingCanalService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    
    public ErrorHandlingCanalService(
        ICanalClientFactory clientFactory,
        IOptions<CanalClientOptions> options,
        ILogger<ErrorHandlingCanalService> logger)
        : base(clientFactory, options)
    {
        _logger = logger;
        
        // 配置重试策略
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 5,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) => {
                    _logger.LogWarning(exception, "连接失败，正在重试... (第 {RetryCount} 次)", retryCount);
                });
    }
    
    public override async Task StartAsync()
    {
        await _retryPolicy.ExecuteAsync(async () => {
            try
            {
                await base.StartAsync();
                _logger.LogInformation("Canal 客户端启动成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Canal 客户端启动失败");
                throw;
            }
        });
    }
}

// 带错误处理的数据库变更处理器
public class ErrorHandlingDatabaseChangeHandler : IDatabaseChangeHandler
{
    private readonly ILogger<ErrorHandlingDatabaseChangeHandler> _logger;
    private readonly AsyncRetryPolicy _processingPolicy;
    
    public ErrorHandlingDatabaseChangeHandler(ILogger<ErrorHandlingDatabaseChangeHandler> logger)
    {
        _logger = logger;
        
        // 配置处理重试策略
        _processingPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(100 * retryAttempt),
                onRetry: (exception, timeSpan, retryCount, context) => {
                    _logger.LogWarning(exception, "处理变更失败，正在重试... (第 {RetryCount} 次)", retryCount);
                });
    }
    
    public async Task HandleAsync(DatabaseChangeEvent @event)
    {
        await _processingPolicy.ExecuteAsync(async () => {
            try
            {
                _logger.LogInformation("处理数据库变更: {Database}.{Table}", @event.Database, @event.Table);
                
                // 实现可能失败的处理逻辑
                await ProcessChangeWithRiskAsync(@event);
                
                _logger.LogInformation("处理成功: {Database}.{Table}", @event.Database, @event.Table);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理失败: {Database}.{Table}", @event.Database, @event.Table);
                throw;
            }
        });
    }
    
    private async Task ProcessChangeWithRiskAsync(DatabaseChangeEvent @event)
    {
        // 实现可能失败的处理逻辑
        await Task.Delay(100);
        
        // 模拟随机失败
        if (Random.Shared.Next(10) == 0)
        {
            throw new Exception("模拟处理失败");
        }
    }
}
```

## 总结

以上示例展示了 CanalSharp 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速上手基本操作**: 学习如何初始化、启动和使用 Canal 客户端
2. **配置高级选项**: 学习如何配置 Canal 客户端，包括连接参数、批量大小等
3. **实现 AOT 编译**: 学习如何编写 AOT 兼容的 Canal 客户端代码
4. **处理错误情况**: 学习如何实现错误处理和重试机制
5. **与其他系统集成**: 学习如何与消息队列、缓存系统等集成

所有示例均遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

### AOT 编译建议

1. **使用 AOT 兼容的库**: 确保所有依赖都支持 AOT 编译
2. **避免反射**: 避免在 Canal 客户端和事件处理中使用反射
3. **使用 System.Text.Json**: 优先使用 System.Text.Json 进行序列化，它是 AOT 兼容的
4. **避免动态代码生成**: 避免使用 System.Reflection.Emit 等动态代码生成技术
5. **测试 AOT 编译**: 在开发过程中定期测试 AOT 编译，确保所有功能正常
6. **使用 AOT 兼容的日志库**: 确保使用的日志库支持 AOT 编译

### 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **调整批量大小**: 根据实际情况调整批量处理大小，平衡延迟和吞吐量
3. **实现异步事件处理**: 事件处理逻辑应尽量异步，避免阻塞主线程
4. **使用高效的序列化**: 使用高效的序列化库，如 MessagePack、Protobuf 等
5. **批量处理事件**: 实现事件批处理，减少外部系统调用次数
6. **监控性能指标**: 定期监控性能指标，如延迟、吞吐量、内存占用等

通过遵循这些最佳实践和示例代码，您可以构建出高效、可靠的 CanalSharp 应用，用于实时数据库变更捕获和处理。
