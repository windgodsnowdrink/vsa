# NetDevPack - 使用示例

## 快速入门

### 1. 基本用法示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetDevPack 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var netDevPackService = serviceProvider.GetRequiredService<INetDevPackService>();
        var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        
        // 订阅事件
        eventBus.Subscribe<SampleEvent>(@event => {
            Console.WriteLine($"收到事件: {@event.Message}");
        });
        
        // 使用基础功能
        var result = await netDevPackService.DoSomethingAsync();
        Console.WriteLine($"结果: {result}");
        
        // 使用缓存
        await cacheManager.SetAsync("greeting", "Hello NetDevPack!", TimeSpan.FromMinutes(5));
        var cachedGreeting = await cacheManager.GetAsync<string>("greeting");
        Console.WriteLine($"缓存的问候语: {cachedGreeting}");
        
        // 发布事件
        await eventBus.PublishAsync(new SampleEvent { Message = "示例事件" });
        
        // 等待一段时间，确保事件被处理
        await Task.Delay(1000);
        
        Console.WriteLine("示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetDevPack 选项
        builder.Configure<NetDevPackOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = false;
        });
        
        // 注册服务
        builder.AddSingleton<INetDevPackService, NetDevPackService>();
        builder.AddSingleton<ICacheManager, CacheManager>();
        builder.AddSingleton<IEventBus, EventBus>();
        
        return builder.BuildServiceProvider();
    }
}

// 示例事件类
public class SampleEvent
{
    public string Message { get; set; }
}
`

### 2. 高级配置示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetDevPack 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 NetDevPack 设置
        builder.Configure<NetDevPackOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<INetDevPackService, NetDevPackService>();
        builder.AddSingleton<INetDevPackEnhancedService, NetDevPackEnhancedService>();
        builder.AddSingleton<ICacheManager, CacheManager>();
        builder.AddSingleton<IEventBus, EventBus>();
        builder.AddSingleton<IObjectMapper, ObjectMapper>();
        builder.AddSingleton<IValidator, Validator>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<NetDevPackOptions>>().Value;
        Console.WriteLine($"配置: 缓存={settings.EnableCache}, 大小={settings.CacheSize}");
        Console.WriteLine($"超时时间: {settings.Timeout}, 详细日志: {settings.EnableDetailedLogging}");
        
        // 使用服务
        var enhancedService = serviceProvider.GetRequiredService<INetDevPackEnhancedService>();
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var validator = serviceProvider.GetRequiredService<IValidator>();
        
        // 使用增强功能
        var enhancedResult = await enhancedService.DoEnhancedOperationAsync();
        Console.WriteLine($"增强操作结果: {enhancedResult}");
        
        // 使用对象映射
        var source = new SourceObject { Id = 1, Name = "测试对象" };
        var target = objectMapper.Map<SourceObject, TargetObject>(source);
        Console.WriteLine($"对象映射结果: Id={target.Id}, Name={target.Name}");
        
        // 使用验证器
        var validationResult = validator.Validate(target);
        Console.WriteLine($"验证结果: {(validationResult.IsValid ? "有效" : "无效")}");
        
        Console.WriteLine("高级配置示例完成");
    }
}

// 源对象类
public class SourceObject
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// 目标对象类
public class TargetObject
{
    public int Id { get; set; }
    public string Name { get; set; }
}
`

### 3. 性能优化示例

`csharp
using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetDevPack 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        var netDevPackService = serviceProvider.GetRequiredService<INetDevPackService>();
        
        // 性能测试：缓存操作
        Console.WriteLine("测试缓存操作性能...");
        const int cacheIterations = 10000;
        var cacheStopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < cacheIterations; i++)
        {
            var key = $"test-key-{i}";
            var value = $"test-value-{i}";
            
            // 设置缓存
            await cacheManager.SetAsync(key, value, TimeSpan.FromMinutes(5));
            
            // 获取缓存
            var cachedValue = await cacheManager.GetAsync<string>(key);
        }
        
        cacheStopwatch.Stop();
        Console.WriteLine($"执行 {cacheIterations} 次缓存操作的时间: {cacheStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次缓存操作: {cacheStopwatch.Elapsed.TotalMilliseconds / cacheIterations:F3} ms");
        
        // 性能测试：服务操作
        Console.WriteLine("\n测试服务操作性能...");
        const int serviceIterations = 1000;
        var serviceStopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < serviceIterations; i++)
        {
            await netDevPackService.DoSomethingAsync();
        }
        
        serviceStopwatch.Stop();
        Console.WriteLine($"执行 {serviceIterations} 次服务操作的时间: {serviceStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次服务操作: {serviceStopwatch.Elapsed.TotalMilliseconds / serviceIterations:F3} ms");
        
        // 使用内存池优化
        Console.WriteLine("\n测试内存池优化...");
        var pool = ArrayPool<byte>.Shared;
        const int memoryIterations = 10000;
        var memoryStopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < memoryIterations; i++)
        {
            // 从内存池获取缓冲区
            var buffer = pool.Rent(1024);
            try
            {
                // 使用缓冲区
                for (int j = 0; j < 1024; j++)
                {
                    buffer[j] = (byte)(i % 256);
                }
            }
            finally
            {
                // 归还缓冲区到内存池
                pool.Return(buffer);
            }
        }
        
        memoryStopwatch.Stop();
        Console.WriteLine($"执行 {memoryIterations} 次内存池操作的时间: {memoryStopwatch.Elapsed.TotalMilliseconds:F3} ms");
        
        Console.WriteLine("性能优化示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetDevPack 选项
        builder.Configure<NetDevPackOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 10000;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = false;
        });
        
        // 注册服务
        builder.AddSingleton<INetDevPackService, NetDevPackService>();
        builder.AddSingleton<INetDevPackEnhancedService, NetDevPackEnhancedService>();
        builder.AddSingleton<ICacheManager, CacheManager>();
        builder.AddSingleton<IEventBus, EventBus>();
        builder.AddSingleton<IObjectMapper, ObjectMapper>();
        builder.AddSingleton<IValidator, Validator>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 4. 错误处理示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetDevPack 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var netDevPackService = serviceProvider.GetRequiredService<INetDevPackService>();
        var cacheManager = serviceProvider.GetRequiredService<ICacheManager>();
        var validator = serviceProvider.GetRequiredService<IValidator>();
        
        try
        {
            // 测试服务操作
            var result = await netDevPackService.DoSomethingAsync();
            Console.WriteLine($"服务操作成功: {result}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
            // 处理超时错误
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
            // 处理操作错误
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
            // 处理通用错误
        }
        
        try
        {
            // 测试缓存操作
            await cacheManager.SetAsync("test-key", "test-value", TimeSpan.FromMinutes(5));
            Console.WriteLine("缓存操作成功");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"缓存错误: {ex.Message}");
            // 处理缓存错误
        }
        
        try
        {
            // 测试验证操作
            var invalidObject = new InvalidObject { Name = "", Age = -1 };
            var validationResult = validator.Validate(invalidObject);
            
            if (!validationResult.IsValid)
            {
                Console.WriteLine("验证失败:");
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine($"- {error.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"验证错误: {ex.Message}");
            // 处理验证错误
        }
        
        Console.WriteLine("错误处理示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetDevPack 选项
        builder.Configure<NetDevPackOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<INetDevPackService, NetDevPackService>();
        builder.AddSingleton<ICacheManager, CacheManager>();
        builder.AddSingleton<IValidator, Validator>();
        
        return builder.BuildServiceProvider();
    }
}

// 无效对象类
public class InvalidObject
{
    public string Name { get; set; }
    public int Age { get; set; }
}
`

### 5. 事件总线示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetDevPack 事件总线示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        
        // 订阅事件
        eventBus.Subscribe<OrderCreatedEvent>(HandleOrderCreated);
        eventBus.Subscribe<PaymentProcessedEvent>(HandlePaymentProcessed);
        eventBus.Subscribe<ShippingNotifiedEvent>(HandleShippingNotified);
        
        Console.WriteLine("已订阅事件");
        
        // 发布事件
        Console.WriteLine("\n发布订单创建事件...");
        await eventBus.PublishAsync(new OrderCreatedEvent {
            OrderId = 123,
            CustomerName = "张三",
            TotalAmount = 99.99
        });
        
        Console.WriteLine("\n发布支付处理事件...");
        await eventBus.PublishAsync(new PaymentProcessedEvent {
            PaymentId = 456,
            OrderId = 123,
            Amount = 99.99,
            PaymentStatus = "成功"
        });
        
        Console.WriteLine("\n发布物流通知事件...");
        await eventBus.PublishAsync(new ShippingNotifiedEvent {
            ShippingId = 789,
            OrderId = 123,
            TrackingNumber = "SF1234567890",
            Status = "已发货"
        });
        
        // 等待事件处理
        await Task.Delay(2000);
        
        Console.WriteLine("\n事件总线示例完成");
    }
    
    private static void HandleOrderCreated(OrderCreatedEvent @event)
    {
        Console.WriteLine($"处理订单创建事件: 订单ID={@event.OrderId}, 客户={@event.CustomerName}, 金额={@event.TotalAmount}");
        // 执行订单创建后的逻辑
    }
    
    private static void HandlePaymentProcessed(PaymentProcessedEvent @event)
    {
        Console.WriteLine($"处理支付事件: 支付ID={@event.PaymentId}, 订单ID={@event.OrderId}, 状态={@event.PaymentStatus}");
        // 执行支付处理后的逻辑
    }
    
    private static void HandleShippingNotified(ShippingNotifiedEvent @event)
    {
        Console.WriteLine($"处理物流事件: 物流ID={@event.ShippingId}, 订单ID={@event.OrderId}, 单号={@event.TrackingNumber}");
        // 执行物流通知后的逻辑
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetDevPack 选项
        builder.Configure<NetDevPackOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.Timeout = TimeSpan.FromSeconds(30);
        });
        
        // 注册服务
        builder.AddSingleton<IEventBus, EventBus>();
        
        return builder.BuildServiceProvider();
    }
}

// 事件类
public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public double TotalAmount { get; set; }
}

public class PaymentProcessedEvent
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public double Amount { get; set; }
    public string PaymentStatus { get; set; }
}

public class ShippingNotifiedEvent
{
    public int ShippingId { get; set; }
    public int OrderId { get; set; }
    public string TrackingNumber { get; set; }
    public string Status { get; set; }
}
`

### 6. 对象映射和验证示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NetDevPack 对象映射和验证示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();
        var validator = serviceProvider.GetRequiredService<IValidator>();
        
        // 测试对象映射
        Console.WriteLine("测试对象映射...");
        var customerDto = new CustomerDto {
            Id = 1,
            FirstName = "张",
            LastName = "三",
            Email = "zhangsan@example.com",
            Age = 30
        };
        
        // 映射到领域对象
        var customer = objectMapper.Map<CustomerDto, Customer>(customerDto);
        Console.WriteLine($"映射结果: ID={customer.Id}, 姓名={customer.FullName}, 邮箱={customer.Email}, 年龄={customer.Age}");
        
        // 测试验证
        Console.WriteLine("\n测试验证...");
        
        // 验证有效对象
        var validCustomer = new Customer {
            Id = 2,
            FullName = "李四",
            Email = "lisi@example.com",
            Age = 25
        };
        
        var validResult = validator.Validate(validCustomer);
        Console.WriteLine($"有效对象验证结果: {(validResult.IsValid ? "通过" : "失败")}");
        
        // 验证无效对象
        var invalidCustomer = new Customer {
            Id = 3,
            FullName = "", // 空姓名
            Email = "invalid-email", // 无效邮箱
            Age = 150 // 无效年龄
        };
        
        var invalidResult = validator.Validate(invalidCustomer);
        Console.WriteLine($"无效对象验证结果: {(invalidResult.IsValid ? "通过" : "失败")}");
        
        if (!invalidResult.IsValid)
        {
            Console.WriteLine("验证错误:");
            foreach (var error in invalidResult.Errors)
            {
                Console.WriteLine($"- {error.Message}");
            }
        }
        
        Console.WriteLine("\n对象映射和验证示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NetDevPack 选项
        builder.Configure<NetDevPackOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.Timeout = TimeSpan.FromSeconds(30);
        });
        
        // 注册服务
        builder.AddSingleton<IObjectMapper, ObjectMapper>();
        builder.AddSingleton<IValidator, Validator>();
        
        return builder.BuildServiceProvider();
    }
}

// 数据传输对象
public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// 领域对象
public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
`

## 总结

以上示例展示了 NetDevPack 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用事件总线
6. 实现对象映射和验证

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 性能优化技巧

1. **使用缓存**：合理使用缓存，减少重复计算和IO操作
2. **异步编程**：使用异步 API 避免阻塞，提高系统吞吐量
3. **内存池**：使用 ArrayPool<T> 减少内存分配和 GC 压力
4. **Span 优化**：使用 Span<T> 减少内存拷贝，提高性能
5. **对象池**：使用对象池减少对象创建和销毁的开销
6. **并行处理**：对于 CPU 密集型操作，使用并行处理提高性能

### 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期，提高代码可测试性
2. **错误处理**：正确处理异常情况，确保系统稳定性
3. **日志记录**：添加适当的日志记录，便于故障排查
4. **事件设计**：设计合理的事件结构，避免事件风暴
5. **缓存策略**：合理使用缓存，避免缓存穿透和缓存雪崩
6. **验证逻辑**：在适当的层级实现验证逻辑，确保数据安全

通过这些示例和最佳实践，您可以充分利用 NetDevPack 技能的强大功能，构建高性能、可靠的应用程序。
