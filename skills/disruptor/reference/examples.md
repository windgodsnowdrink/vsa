# disruptor - 使用示例

## AOT 架构示例

### 1. 基本事件处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Disruptor.AOT;

public class BasicEventProcessingExample
{
    public static async Task Run()
    {
        Console.WriteLine("基本事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机和服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置 Disruptor 选项
        builder.Configuration.AddJsonFile("disruptor_aot.setting.json", optional: true);
        builder.Services.Configure<DisruptorOptions>(builder.Configuration.GetSection("Disruptor"));
        
        // 注册 Disruptor 服务
        builder.Services.AddDisruptor();
        
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取 Disruptor 引擎
        var engine = serviceProvider.GetRequiredService<DisruptorAotEngine>();
        
        // 启动 Disruptor 服务
        Console.WriteLine("1. 启动 Disruptor 服务...");
        var startResult = await engine.StartAsync();
        Console.WriteLine($"   结果: {(startResult ? "成功" : "失败")}");
        
        // 创建并发布多个事件
        Console.WriteLine("\n2. 发布多个测试事件...");
        for (int i = 1; i <= 5; i++)
        {
            var evnt = new DisruptorEvent
            {
                Id = Guid.NewGuid(),
                Type = DisruptorEventType.Normal,
                Data = $"测试事件 {i}",
                CreatedAt = DateTime.UtcNow,
                Source = "BasicExample",
                Tags = new List<string> { "demo", "basic", $"event-{i}" }
            };
            
            var result = await engine.PublishEventAsync(evnt);
            Console.WriteLine($"   事件 {i}: {(result.Success ? "成功" : "失败")}");
            
            // 短暂延迟，以便观察效果
            await Task.Delay(100);
        }
        
        // 获取 Disruptor 状态
        Console.WriteLine("\n3. 获取 Disruptor 状态...");
        var status = await engine.GetStatusAsync();
        Console.WriteLine($"   已处理事件: {status.ProcessedEvents}");
        Console.WriteLine($"   成功事件: {status.SuccessfulEvents}");
        Console.WriteLine($"   失败事件: {status.FailedEvents}");
        Console.WriteLine($"   平均处理时间: {status.AverageProcessingTimeMs} ms");
        
        // 停止 Disruptor 服务
        Console.WriteLine("\n4. 停止 Disruptor 服务...");
        var stopResult = await engine.StopAsync();
        Console.WriteLine($"   结果: {(stopResult ? "成功" : "失败")}");
        
        Console.WriteLine("\n基本事件处理示例完成！");
    }
}
```

### 2. 批量事件处理示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Disruptor.AOT;

public class BatchEventProcessingExample
{
    public static async Task Run()
    {
        Console.WriteLine("批量事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器，配置批量处理选项
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DisruptorOptions>(options =>
        {
            options.RingBufferSize = 4096;
            options.ConsumerCount = 2;
            options.EnableBatching = true; // 启用批量处理
            options.BatchSize = 10; // 批量大小设为 10
        });
        builder.Services.AddDisruptor();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DisruptorAotEngine>();
        
        // 启动服务
        await engine.StartAsync();
        
        // 生成大量事件
        Console.WriteLine("1. 生成测试事件...");
        var events = new List<DisruptorEvent>();
        for (int i = 1; i <= 100; i++)
        {
            events.Add(new DisruptorEvent
            {
                Id = Guid.NewGuid(),
                Type = DisruptorEventType.Normal,
                Data = $"批量事件 {i}",
                CreatedAt = DateTime.UtcNow,
                Source = "BatchExample",
                Tags = new List<string> { "demo", "batch" }
            });
        }
        Console.WriteLine($"   生成了 {events.Count} 个事件");
        
        // 批量发布事件
        Console.WriteLine("\n2. 批量发布事件...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = await engine.PublishEventsAsync(events);
        stopwatch.Stop();
        
        // 统计结果
        var successful = results.Count(r => r.Success);
        var failed = results.Count(r => !r.Success);
        
        Console.WriteLine($"   批量发布完成！");
        Console.WriteLine($"   总事件数: {events.Count}");
        Console.WriteLine($"   成功: {successful}");
        Console.WriteLine($"   失败: {failed}");
        Console.WriteLine($"   总耗时: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"   平均事件耗时: {stopwatch.ElapsedMilliseconds / (double)events.Count:F2} ms");
        
        // 查看状态
        Console.WriteLine("\n3. 查看 Disruptor 状态...");
        var status = await engine.GetStatusAsync();
        Console.WriteLine($"   已处理事件: {status.ProcessedEvents}");
        Console.WriteLine($"   环形缓冲区使用率: {status.RingBufferUsage:F2}%");
        
        // 停止服务
        await engine.StopAsync();
        
        Console.WriteLine("\n批量事件处理示例完成！");
    }
}
```

### 3. 优先级事件处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Disruptor.AOT;

public class PriorityEventExample
{
    public static async Task Run()
    {
        Console.WriteLine("优先级事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DisruptorOptions>(options =>
        {
            options.EnablePriorityQueue = true; // 启用优先级队列
        });
        builder.Services.AddDisruptor();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DisruptorAotEngine>();
        
        // 启动服务
        await engine.StartAsync();
        
        Console.WriteLine("1. 发布不同优先级的事件...");
        
        // 发布普通事件（低优先级）
        for (int i = 1; i <= 3; i++)
        {
            var evnt = new DisruptorEvent
            {
                Id = Guid.NewGuid(),
                Type = DisruptorEventType.Normal,
                Data = $"普通事件 {i}",
                CreatedAt = DateTime.UtcNow,
                Source = "PriorityExample"
            };
            await engine.PublishEventAsync(evnt);
            Console.WriteLine($"   发布了普通事件 {i}（优先级: 0）");
        }
        
        // 发布优先级事件
        var priorityEvent = new DisruptorEvent
        {
            Id = Guid.NewGuid(),
            Type = DisruptorEventType.Priority,
            Data = "高优先级事件",
            CreatedAt = DateTime.UtcNow,
            Source = "PriorityExample"
        };
        await engine.PublishPriorityEventAsync(priorityEvent, 100);
        Console.WriteLine("   发布了高优先级事件（优先级: 100）");
        
        // 发布更多普通事件
        for (int i = 4; i <= 6; i++)
        {
            var evnt = new DisruptorEvent
            {
                Id = Guid.NewGuid(),
                Type = DisruptorEventType.Normal,
                Data = $"普通事件 {i}",
                CreatedAt = DateTime.UtcNow,
                Source = "PriorityExample"
            };
            await engine.PublishEventAsync(evnt);
            Console.WriteLine($"   发布了普通事件 {i}（优先级: 0）");
        }
        
        // 发布紧急事件
        var emergencyEvent = new DisruptorEvent
        {
            Id = Guid.NewGuid(),
            Type = DisruptorEventType.Emergency,
            Data = "紧急事件",
            CreatedAt = DateTime.UtcNow,
            Source = "PriorityExample"
        };
        await engine.PublishPriorityEventAsync(emergencyEvent, 90);
        Console.WriteLine("   发布了紧急事件（优先级: 90）");
        
        // 等待事件处理完成
        await Task.Delay(1000);
        
        // 查看状态
        Console.WriteLine("\n2. 事件处理完成，查看状态:");
        var status = await engine.GetStatusAsync();
        Console.WriteLine($"   已处理事件: {status.ProcessedEvents}");
        Console.WriteLine($"   成功事件: {status.SuccessfulEvents}");
        
        // 停止服务
        await engine.StopAsync();
        
        Console.WriteLine("\n优先级事件处理示例完成！");
        Console.WriteLine("   注意：高优先级事件会被优先处理，即使它们是后发布的");
    }
}
```

### 4. 多消费者配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Disruptor.AOT;

public class MultiConsumerExample
{
    public static async Task Run()
    {
        Console.WriteLine("多消费者配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器，配置多个消费者
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DisruptorOptions>(options =>
        {
            options.RingBufferSize = 4096;
            options.ConsumerCount = 4; // 配置 4 个消费者
            options.EnableBatching = true;
            options.BatchSize = 32;
        });
        builder.Services.AddDisruptor();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DisruptorAotEngine>();
        
        // 启动服务
        await engine.StartAsync();
        
        Console.WriteLine("1. 启动了包含 4 个消费者的 Disruptor 服务");
        
        // 发布大量事件
        Console.WriteLine("\n2. 发布 1000 个事件...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 1; i <= 1000; i++)
        {
            var evnt = new DisruptorEvent
            {
                Id = Guid.NewGuid(),
                Type = DisruptorEventType.Normal,
                Data = $"多消费者事件 {i}",
                CreatedAt = DateTime.UtcNow,
                Source = "MultiConsumerExample"
            };
            await engine.PublishEventAsync(evnt);
            
            // 每 100 个事件显示进度
            if (i % 100 == 0)
            {
                Console.WriteLine($"   已发布 {i}/1000 个事件");
            }
        }
        
        stopwatch.Stop();
        
        // 等待所有事件处理完成
        await Task.Delay(2000);
        
        // 查看状态
        Console.WriteLine("\n3. 事件处理完成，查看状态:");
        var status = await engine.GetStatusAsync();
        
        Console.WriteLine($"   总发布事件: 1000");
        Console.WriteLine($"   已处理事件: {status.ProcessedEvents}");
        Console.WriteLine($"   成功事件: {status.SuccessfulEvents}");
        Console.WriteLine($"   发布耗时: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"   活跃消费者: {status.ActiveConsumers}");
        Console.WriteLine($"   平均处理时间: {status.AverageProcessingTimeMs} ms");
        
        // 停止服务
        await engine.StopAsync();
        
        Console.WriteLine("\n多消费者配置示例完成！");
        Console.WriteLine("   注意：多个消费者可以并行处理事件，提高系统吞吐量");
    }
}
```

### 5. 状态监控示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Disruptor.AOT;

public class StatusMonitoringExample
{
    public static async Task Run()
    {
        Console.WriteLine("状态监控示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<DisruptorOptions>(options =>
        {
            options.EnablePerformanceMonitoring = true; // 启用性能监控
        });
        builder.Services.AddDisruptor();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<DisruptorAotEngine>();
        
        // 启动服务
        await engine.StartAsync();
        
        // 初始状态
        Console.WriteLine("1. 初始状态:");
        await PrintStatusAsync(engine);
        
        // 发布一些事件
        Console.WriteLine("\n2. 发布 50 个事件...");
        for (int i = 1; i <= 50; i++)
        {
            var evnt = new DisruptorEvent
            {
                Id = Guid.NewGuid(),
                Type = DisruptorEventType.Normal,
                Data = $"监控事件 {i}",
                CreatedAt = DateTime.UtcNow,
                Source = "MonitoringExample"
            };
            await engine.PublishEventAsync(evnt);
        }
        
        // 等待事件处理
        await Task.Delay(1000);
        
        // 查看处理后的状态
        Console.WriteLine("\n3. 处理后的状态:");
        await PrintStatusAsync(engine);
        
        // 重置状态
        Console.WriteLine("\n4. 重置 Disruptor 状态...");
        var resetResult = await engine.ResetStatusAsync();
        Console.WriteLine($"   结果: {(resetResult ? "成功" : "失败")}");
        
        // 查看重置后的状态
        Console.WriteLine("\n5. 重置后的状态:");
        await PrintStatusAsync(engine);
        
        // 停止服务
        await engine.StopAsync();
        
        Console.WriteLine("\n状态监控示例完成！");
    }
    
    private static async Task PrintStatusAsync(DisruptorAotEngine engine)
    {
        var status = await engine.GetStatusAsync();
        
        Console.WriteLine($"   运行状态: {(status.IsRunning ? "正常" : "异常")}");
        Console.WriteLine($"   已处理事件: {status.ProcessedEvents}");
        Console.WriteLine($"   成功事件: {status.SuccessfulEvents}");
        Console.WriteLine($"   失败事件: {status.FailedEvents}");
        Console.WriteLine($"   平均处理时间: {status.AverageProcessingTimeMs} ms");
        Console.WriteLine($"   环形缓冲区使用率: {status.RingBufferUsage:F2}%");
        Console.WriteLine($"   活跃消费者: {status.ActiveConsumers}");
        Console.WriteLine($"   环形缓冲区大小: {status.RingBufferSize}");
        Console.WriteLine($"   服务启动时间: {status.StartTime.ToLocalTime()}");
    }
}
```

## 总结

以上示例展示了基于 .NET 10 AOT 架构的 Disruptor 事件处理技能的主要功能和使用方法：

1. **基本事件处理**：创建并发布简单事件，演示了 Disruptor 服务的基本使用流程
2. **批量事件处理**：展示了批量发布大量事件的高效处理方式
3. **优先级事件处理**：演示了如何处理不同优先级的事件，高优先级事件会被优先处理
4. **多消费者配置**：展示了如何配置多个消费者并行处理事件，提高系统吞吐量
5. **状态监控**：演示了如何监控 Disruptor 服务的运行状态和性能指标

该系统设计遵循了 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的事件处理场景。通过 AOT 编译技术，提供了极致的性能和启动速度，非常适合高并发、低延迟的事件处理需求。

所有示例代码都可以直接在支持 .NET 10 的环境中运行，只需要配置相应的 Disruptor 选项即可。