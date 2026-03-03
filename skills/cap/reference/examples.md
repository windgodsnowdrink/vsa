# CAP - 使用示例

## 快速开始

### 1. 基础使用示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package DotNetCore.CAP@8.0.0
#:package DotNetCore.CAP.Redis@8.0.0
#:package DotNetCore.CAP.MySql@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using DotNetCore.CAP;

// 事件模型
public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var capService = serviceProvider.GetRequiredService<ICapService>();
        
        Console.WriteLine("CAP 基础使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 CAP 发布事件
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = 1,
            ProductId = 1001,
            Quantity = 2,
            TotalAmount = 99.99,
            CreatedAt = DateTime.UtcNow
        };
        
        await capService.PublishAsync("order.created", orderEvent);
        Console.WriteLine("事件发布成功");
        Console.WriteLine($"事件内容: OrderId={orderEvent.OrderId}, ProductId={orderEvent.ProductId}");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 配置 CAP 服务
        builder.AddCap(options => {
            // 使用 Redis 作为消息队列
            options.UseRedis(redisOptions => {
                redisOptions.Configuration = "localhost:6379";
            });
            
            // 使用 MySQL 作为存储
            options.UseMySql(mySqlOptions => {
                mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
            });
            
            // 配置 CAP 基本选项
            options.GroupName = "cap-basic-example";
            options.DefaultGroupName = "cap-basic-example";
            options.EnableAotOptimization = true;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Configuration.Json@10.0.0
#:package DotNetCore.CAP@8.0.0
#:package DotNetCore.CAP.Redis@8.0.0
#:package DotNetCore.CAP.MySql@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using DotNetCore.CAP;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("CAP 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        
        // 获取 CAP 服务
        var capService = serviceProvider.GetRequiredService<ICapService>();
        
        // 发布延迟事件
        await capService.PublishDelayAsync("order.delayed", new {
            OrderId = 2,
            Message = "延迟10秒发送的订单事件"
        }, TimeSpan.FromSeconds(10));
        
        Console.WriteLine("延迟事件已安排，10秒后将被发送");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        // 配置构建器
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        
        var builder = new ServiceCollection();
        
        // 注册配置
        builder.AddSingleton<IConfiguration>(configuration);
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
        
        // 配置 CAP 服务
        builder.AddCap(options => {
            // 使用 Redis 作为消息队列
            options.UseRedis(redisOptions => {
                redisOptions.Configuration = configuration.GetConnectionString("Redis");
            });
            
            // 使用 MySQL 作为存储
            options.UseMySql(mySqlOptions => {
                mySqlOptions.ConnectionString = configuration.GetConnectionString("MySQL");
                mySqlOptions.TableNamePrefix = "cap_";
            });
            
            // 从配置文件读取 CAP 选项
            options.GroupName = configuration["Cap:GroupName"];
            options.DefaultGroupName = configuration["Cap:DefaultGroupName"];
            options.FailedRetryCount = int.Parse(configuration["Cap:FailedRetryCount"]);
            options.FailedRetryInterval = int.Parse(configuration["Cap:FailedRetryInterval"]);
            options.EnableConsumerPrefetch = bool.Parse(configuration["Cap:EnableConsumerPrefetch"]);
            options.PrefetchCount = int.Parse(configuration["Cap:PrefetchCount"]);
            options.EnableAotOptimization = bool.Parse(configuration["Cap:EnableAotOptimization"]);
            options.EnableDetailedLogging = bool.Parse(configuration["Cap:EnableDetailedLogging"]);
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 3. AOT 编译示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:package DotNetCore.CAP@8.0.0
#:package DotNetCore.CAP.Redis@8.0.0
#:package DotNetCore.CAP.MySql@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
using System;
using Microsoft.Extensions.DependencyInjection;
using DotNetCore.CAP;

// AOT 安全的事件模型
public record ProductUpdatedEvent(int ProductId, string ProductName, decimal Price, DateTime UpdatedAt);

// AOT 安全的事件处理器
public class ProductEventHandler
{
    public ProductEventHandler() { }
    
    // 注意：AOT 环境下，事件处理器必须是公共的且可实例化的
    [CapSubscribe("product.updated", Group = "aot-product-group")]
    public async Task HandleProductUpdatedAsync(ProductUpdatedEvent @event)
    {
        Console.WriteLine($"[AOT 模式] 收到产品更新事件: ProductId={@event.ProductId}, ProductName={@event.ProductName}");
        Console.WriteLine($"更新时间: {@event.UpdatedAt}");
        
        // 模拟异步处理
        await Task.Delay(100);
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var capService = serviceProvider.GetRequiredService<ICapService>();
        
        Console.WriteLine("CAP AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 发布 AOT 安全的事件
        var productEvent = new ProductUpdatedEvent
        {
            ProductId = 3001,
            ProductName = "高性能 AOT 产品",
            Price = 199.99m,
            UpdatedAt = DateTime.UtcNow
        };
        
        await capService.PublishAsync("product.updated", productEvent);
        Console.WriteLine("AOT 安全事件发布成功");
        
        // 保持程序运行以接收事件
        Console.WriteLine("\n程序正在运行，按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册事件处理器（AOT 环境下必须显式注册）
        builder.AddSingleton<ProductEventHandler>();
        
        // 配置 CAP 服务
        builder.AddCap(options => {
            // 使用 Redis 作为消息队列
            options.UseRedis(redisOptions => {
                redisOptions.Configuration = "localhost:6379";
            });
            
            // 使用 MySQL 作为存储
            options.UseMySql(mySqlOptions => {
                mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
            });
            
            // 关键：启用 AOT 优化
            options.EnableAotOptimization = true;
            options.GroupName = "cap-aot-example";
            options.DefaultGroupName = "cap-aot-example";
            options.EnableDetailedLogging = false; // AOT 环境下减少日志以提高性能
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 4. 性能优化示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package DotNetCore.CAP@8.0.0
#:package DotNetCore.CAP.Redis@8.0.0
#:package DotNetCore.CAP.MySql@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DotNetCore.CAP;

// 轻量级事件模型，适合高性能场景
public struct PerformanceEvent
{
    public int EventId { get; set; }
    public long Timestamp { get; set; }
    public int Value { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var capService = serviceProvider.GetRequiredService<ICapService>();
        
        Console.WriteLine("CAP 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 性能测试：批量发布事件
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"开始发布 {iterations} 个事件...");
        
        // 批量发布事件（异步并行）
        var tasks = new List<Task>();
        for (int i = 0; i < iterations; i++)
        {
            var performanceEvent = new PerformanceEvent
            {
                EventId = i + 1,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Value = i * 10
            };
            
            // 并行发布事件
            tasks.Add(capService.PublishAsync("performance.test", performanceEvent));
        }
        
        // 等待所有发布完成
        await Task.WhenAll(tasks);
        
        stopwatch.Stop();
        
        Console.WriteLine($"发布 {iterations} 个事件完成");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每个事件: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        Console.WriteLine($"每秒处理: {iterations / stopwatch.Elapsed.TotalSeconds:F0} 个事件");
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Warning); // 减少日志输出以提高性能
        });
        
        // 配置高性能 CAP 服务
        builder.AddCap(options => {
            // 使用 Redis 作为消息队列
            options.UseRedis(redisOptions => {
                redisOptions.Configuration = "localhost:6379";
                redisOptions.ConnectionMultiplexerFactory = () => {
                    // 自定义 Redis 连接配置，优化性能
                    return Task.FromResult(ConnectionMultiplexer.Connect(new ConfigurationOptions {
                        EndPoints = { "localhost:6379" },
                        SyncTimeout = 5000,
                        AsyncTimeout = 5000,
                        ConnectTimeout = 5000,
                        ConnectRetry = 3,
                        AllowAdmin = false,
                        AbortOnConnectFail = false,
                        KeepAlive = 60,
                        DefaultDatabase = 0
                    }));
                };
            });
            
            // 使用 MySQL 作为存储，优化连接池
            options.UseMySql(mySqlOptions => {
                mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;Connection Timeout=30;Pooling=True;Min Pool Size=5;Max Pool Size=100;";
            });
            
            // 高性能配置选项
            options.GroupName = "cap-performance-example";
            options.DefaultGroupName = "cap-performance-example";
            options.EnableConsumerPrefetch = true;
            options.PrefetchCount = 500; // 增加预取数量以提高吞吐量
            options.FailedRetryCount = 3; // 减少重试次数以降低系统负载
            options.EnableAotOptimization = true; // 启用 AOT 优化
            options.EnableDetailedLogging = false; // 关闭详细日志
        });
        
        return builder.BuildServiceProvider();
    }
}
```

### 5. 错误处理示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package DotNetCore.CAP@8.0.0
#:package DotNetCore.CAP.Redis@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using DotNetCore.CAP;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var capService = serviceProvider.GetRequiredService<ICapService>();
        
        Console.WriteLine("CAP 错误处理示例");
        Console.WriteLine("=" * 50);
        
        try
        {
            // 尝试发布事件
            await capService.PublishAsync("test.error", new {
                Message = "测试错误处理的事件"
            });
            
            Console.WriteLine("事件发布成功");
        }
        catch (CapPublishException ex)
        {
            Console.WriteLine($"CAP 发布异常: {ex.Message}");
            Console.WriteLine($"异常类型: {ex.GetType().Name}");
        }
        catch (ConnectionFailedException ex)
        {
            Console.WriteLine($"连接失败异常: {ex.Message}");
            Console.WriteLine("请检查消息队列和数据库连接");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作异常: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用异常: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
        
        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册日志服务
        builder.AddLogging(logging => {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
        
        // 配置 CAP 服务
        builder.AddCap(options => {
            // 使用 Redis 作为消息队列（故意配置错误的地址）
            options.UseRedis(redisOptions => {
                redisOptions.Configuration = "invalid-host:6379"; // 错误的 Redis 地址
            });
            
            // 使用 MySQL 作为存储
            options.UseMySql(mySqlOptions => {
                mySqlOptions.ConnectionString = "Server=localhost;Database=capdb;User=root;Password=password;";
            });
            
            options.GroupName = "cap-error-handling-example";
            options.EnableAotOptimization = true;
        });
        
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例演示了 CAP 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用 CAP 进行事件发布和订阅
2. 配置高级选项以满足复杂场景需求
3. 在 AOT 环境下安全使用 CAP
4. 优化 CAP 性能以处理高并发场景
5. 实现完善的错误处理机制

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。所有示例都支持 AOT 编译，可以编译为本机代码以获得更高的性能和更快的启动速度。
