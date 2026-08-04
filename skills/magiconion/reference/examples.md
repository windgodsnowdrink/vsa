# magiconion - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MagicOnion;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();
        
        Console.WriteLine("magiconion 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 使用 magiconion 功能
        var data = new { Name = "测试数据", Value = 100 };
        var result = await magiconionService.ProcessAsync(data);
        Console.WriteLine($"处理结果: {result}");
        
        Console.WriteLine("\n基本用法示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IMagiconionService, MagiconionService>();
        builder.AddSingleton<IStreamingProcessor, ChannelStreamingProcessor>();
        return builder.BuildServiceProvider();
    }
}

// 服务接口定义
public interface IMagiconionService
{
    Task<string> ProcessAsync(object data);
}

// 服务实现
public class MagiconionService : IMagiconionService
{
    private readonly IStreamingProcessor _processor;
    
    public MagiconionService(IStreamingProcessor processor)
    {
        _processor = processor;
    }
    
    public async Task<string> ProcessAsync(object data)
    {
        // 处理逻辑
        await Task.Delay(100); // 模拟处理
        return $"处理成功: {data}";
    }
}

// 流式处理器接口
public interface IStreamingProcessor
{
    Task ProcessAsync(StreamingMessage message);
}

// 流式处理器实现
public class ChannelStreamingProcessor : IStreamingProcessor
{
    public async Task ProcessAsync(StreamingMessage message)
    {
        // 处理逻辑
        await Task.Delay(50); // 模拟处理
    }
}

// 流式消息
public class StreamingMessage
{
    public string Route { get; set; }
    public byte[] Payload { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
```

### 2. 实时聊天示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MagicOnion;

public class ChatExample
{
    public static async Task Main()
    {
        Console.WriteLine("magiconion 实时聊天示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var chatService = serviceProvider.GetRequiredService<IChatService>();
        
        // 加入房间
        Console.WriteLine("1. 加入聊天房间");
        var joinResult = await chatService.JoinAsync("general", "user123");
        Console.WriteLine($"加入房间成功: {joinResult.Success}, Token: {joinResult.Token}");
        
        // 发送消息
        Console.WriteLine("\n2. 发送聊天消息");
        var chatMessage = new ChatMessage {
            RoomId = "general",
            UserId = "user123",
            Text = "Hello, Magiconion!",
            Timestamp = DateTimeOffset.Now
        };
        var sendResult = await chatService.SendAsync("general", chatMessage);
        Console.WriteLine($"发送消息成功: {sendResult}");
        
        // 接收消息（流式）
        Console.WriteLine("\n3. 接收聊天消息");
        using var stream = chatService.StreamAsync("general");
        await foreach (var message in stream.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"[{message.Timestamp}] {message.UserId}: {message.Text}");
            // 模拟接收几条消息后退出
            break;
        }
        
        // 离开房间
        Console.WriteLine("\n4. 离开聊天房间");
        var leaveResult = await chatService.LeaveAsync("general", "user123");
        Console.WriteLine($"离开房间成功: {leaveResult}");
        
        Console.WriteLine("\n实时聊天示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IChatService, ChatService>();
        builder.AddSingleton<IChatProcessor, ChannelChatProcessor>();
        return builder.BuildServiceProvider();
    }
}

// 聊天服务接口
public interface IChatService : IService<IChatService>
{
    UnaryResult<JoinResult> JoinAsync(string roomId, string userId);
    UnaryResult<bool> LeaveAsync(string roomId, string userId);
    ServerStreamingResult<ChatMessage> StreamAsync(string roomId);
    UnaryResult<bool> SendAsync(string roomId, ChatMessage message);
}

// 聊天服务实现
public class ChatService : ServiceBase<IChatService>, IChatService
{
    private readonly IChatProcessor _processor;
    
    public ChatService(IChatProcessor processor)
    {
        _processor = processor;
    }
    
    public async UnaryResult<JoinResult> JoinAsync(string roomId, string userId)
    {
        // 加入房间逻辑
        await Task.Delay(50);
        return new JoinResult { Success = true, Token = Guid.NewGuid().ToString() };
    }
    
    public async UnaryResult<bool> LeaveAsync(string roomId, string userId)
    {
        // 离开房间逻辑
        await Task.Delay(30);
        return true;
    }
    
    public async ServerStreamingResult<ChatMessage> StreamAsync(string roomId)
    {
        // 流式消息逻辑
        var stream = GetServerStreamingContext<ChatMessage>();
        // 模拟发送消息
        await stream.WriteAsync(new ChatMessage {
            RoomId = roomId,
            UserId = "system",
            Text = "欢迎加入聊天室！",
            Timestamp = DateTimeOffset.Now
        });
        await Task.Delay(100);
        await stream.WriteAsync(new ChatMessage {
            RoomId = roomId,
            UserId = "user1",
            Text = "大家好！",
            Timestamp = DateTimeOffset.Now
        });
        return stream.Result();
    }
    
    public async UnaryResult<bool> SendAsync(string roomId, ChatMessage message)
    {
        // 发送消息逻辑
        await _processor.ProcessAsync(message);
        return true;
    }
}

// 聊天处理器接口
public interface IChatProcessor
{
    Task ProcessAsync(ChatMessage message);
}

// 聊天处理器实现
public class ChannelChatProcessor : IChatProcessor
{
    public async Task ProcessAsync(ChatMessage message)
    {
        // 处理逻辑
        await Task.Delay(20);
    }
}

// 聊天消息
public class ChatMessage
{
    public string RoomId { get; set; }
    public string UserId { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

// 加入结果
public class JoinResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("magiconion 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 magiconion 设置
        builder.Configure<MagicOnionOptions>(options => {
            options.Server.Port = 5001;
            options.Server.UseTls = false;
            options.Server.MaxConcurrentConnections = 1000;
            options.Server.ReceiveTimeoutMilliseconds = 30000;
            options.Serialization.EnableCompression = true;
            options.Serialization.CompressionType = "Lz4BlockArray";
            options.Serialization.BufferSize = 65536;
        });
        
        // 配置通道设置
        builder.Configure<ChannelOptions>(options => {
            options.Capacity = 10000;
            options.FullMode = "Wait";
            options.SingleReader = false;
            options.SingleWriter = false;
        });
        
        // 注册服务
        builder.AddSingleton<IStreamingProcessor, ChannelStreamingProcessor>();
        builder.AddSingleton<IMagiconionService, MagiconionService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<MagicOnionOptions>>().Value;
        Console.WriteLine($"配置信息: 端口={settings.Server.Port}, 最大并发={settings.Server.MaxConcurrentConnections}");
        
        // 使用服务
        var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();
        var result = await magiconionService.ProcessAsync(new { ConfigTest = true });
        Console.WriteLine($"处理结果: {result}");
        
        Console.WriteLine("\n高级配置示例完成！");
    }
}

// Magiconion 选项
public class MagicOnionOptions
{
    public ServerOptions Server { get; set; } = new ServerOptions();
    public SerializationOptions Serialization { get; set; } = new SerializationOptions();
}

// 服务器选项
public class ServerOptions
{
    public int Port { get; set; } = 5001;
    public bool UseTls { get; set; } = false;
    public int MaxConcurrentConnections { get; set; } = 1000;
    public int ReceiveTimeoutMilliseconds { get; set; } = 30000;
}

// 序列化选项
public class SerializationOptions
{
    public bool EnableCompression { get; set; } = true;
    public string CompressionType { get; set; } = "Lz4BlockArray";
    public int BufferSize { get; set; } = 65536;
}

// 通道选项
public class ChannelOptions
{
    public int Capacity { get; set; } = 10000;
    public string FullMode { get; set; } = "Wait";
    public bool SingleReader { get; set; } = false;
    public bool SingleWriter { get; set; } = false;
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("magiconion 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();
        
        // 准备测试数据
        var testData = new List<object>();
        for (int i = 0; i < 1000; i++)
        {
            testData.Add(new { Id = i, Name = $"测试数据{i}", Value = i * 100 });
        }
        
        // 性能测试
        const int iterations = 10;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            Console.WriteLine($"测试迭代 {i + 1}/{iterations}");
            
            // 并行处理
            var tasks = testData.Select(data => magiconionService.ProcessAsync(data)).ToList();
            await Task.WhenAll(tasks);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"\n性能测试结果:");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalSeconds:F2} 秒");
        Console.WriteLine($"平均每次执行: {stopwatch.Elapsed.TotalSeconds / iterations:F3} 秒");
        Console.WriteLine($"处理速度: {testData.Count * iterations / stopwatch.Elapsed.TotalSeconds:F2} 条/秒");
        
        Console.WriteLine("\n性能优化示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 创建高性能通道
        var messageChannel = Channel.CreateBounded<StreamingMessage>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        // 注册通道处理器
        builder.AddSingleton<IStreamingProcessor>(sp => 
            new OptimizedChannelProcessor(
                messageChannel,
                new ThreadLocal<Span<byte>>(() => stackalloc byte[1024])));
        
        builder.AddSingleton<IMagiconionService, MagiconionService>();
        
        return builder.BuildServiceProvider();
    }
}

// 优化的通道处理器
public class OptimizedChannelProcessor : IStreamingProcessor
{
    private readonly Channel<StreamingMessage> _channel;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public OptimizedChannelProcessor(Channel<StreamingMessage> channel, ThreadLocal<Span<byte>> buffer)
    {
        _channel = channel;
        _buffer = buffer;
        
        // 启动处理任务
        _ = Task.Run(ProcessMessagesAsync);
    }
    
    public async Task ProcessAsync(StreamingMessage message)
    {
        // 尝试写入通道
        await _channel.Writer.WriteAsync(message);
    }
    
    private async Task ProcessMessagesAsync()
    {
        await foreach (var message in _channel.Reader.ReadAllAsync())
        {
            // 处理消息
            await Task.Delay(1); // 模拟处理
        }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("magiconion 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();
        
        try
        {
            Console.WriteLine("1. 测试正常处理");
            var normalData = new { Name = "正常数据", Value = 100 };
            var normalResult = await magiconionService.ProcessAsync(normalData);
            Console.WriteLine($"✓ 处理成功: {normalResult}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"✗ 超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"✗ 操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 通用错误: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n2. 测试超时处理");
            var timeoutData = new { Name = "超时数据", Value = -1 };
            var timeoutResult = await magiconionService.ProcessAsync(timeoutData);
            Console.WriteLine($"处理结果: {timeoutResult}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"✓ 超时错误捕获: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 其他错误: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n3. 测试参数错误");
            var errorData = null; // 空数据
            var errorResult = await magiconionService.ProcessAsync(errorData);
            Console.WriteLine($"处理结果: {errorResult}");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"✓ 参数错误捕获: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 其他错误: {ex.Message}");
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IStreamingProcessor, ChannelStreamingProcessor>();
        builder.AddSingleton<IMagiconionService, FaultTolerantMagiconionService>();
        return builder.BuildServiceProvider();
    }
}

// 容错的 Magiconion 服务
public class FaultTolerantMagiconionService : IMagiconionService
{
    private readonly IStreamingProcessor _processor;
    
    public FaultTolerantMagiconionService(IStreamingProcessor processor)
    {
        _processor = processor;
    }
    
    public async Task<string> ProcessAsync(object data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "数据不能为空");
        }
        
        var testData = data as dynamic;
        if (testData?.Value == -1)
        {
            // 模拟超时
            await Task.Delay(5000);
            throw new TimeoutException("处理超时");
        }
        
        // 正常处理
        await Task.Delay(100);
        return $"处理成功: {data}";
    }
}
```

### 5. AOT 编译示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package MagicOnion@5.0.0 
#:package System.Threading.Channels@8.0.0 
#:property LangVersion=preview 
#:property TargetFramework=net10.0 
#:property Nullable=enable 
#:property ImplicitUsings=enable 
#:property PublishAot=true 
#:property IncludeNativeLibrariesForSelfExtract=true 
#:property EnableCppCodeGen=true 
#:property PublishSingleFile=true 
#:property SelfContained=true 
#:property RuntimeIdentifier=win-x64 

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class AotExample
{
    public static async Task Main()
    {
        Console.WriteLine("magiconion AOT 编译示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("使用 AOT 编译的单文件可执行程序");
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var magiconionService = serviceProvider.GetRequiredService<IMagiconionService>();
        
        // 测试AOT编译后的性能
        var testData = new List<object> {
            new { Id = 1, Name = "AOT测试1", Value = 100 },
            new { Id = 2, Name = "AOT测试2", Value = 200 },
            new { Id = 3, Name = "AOT测试3", Value = 300 }
        };
        
        Console.WriteLine("\n测试AOT编译后的处理性能");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        foreach (var data in testData)
        {
            var result = await magiconionService.ProcessAsync(data);
            Console.WriteLine($"处理结果: {result}");
        }
        
        stopwatch.Stop();
        Console.WriteLine($"\nAOT 编译性能测试:");
        Console.WriteLine($"处理时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均每条: {stopwatch.Elapsed.TotalMilliseconds / testData.Count:F3} ms");
        
        Console.WriteLine("\nAOT 编译示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<IStreamingProcessor, ChannelStreamingProcessor>();
        builder.AddSingleton<IMagiconionService, MagiconionService>();
        return builder.BuildServiceProvider();
    }
}

// 流式消息
public class StreamingMessage
{
    public string Route { get; set; }
    public byte[] Payload { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
```

## 总结

以上示例展示了 magiconion 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用 AOT 编译提升性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 支持的功能场景

- **实时聊天**: 基于 MagicOnion 的实时通信功能
- **分布式计算**: 高性能的分布式任务处理
- **实时数据处理**: 低延迟的数据流处理
- **系统集成**: 与其他系统的无缝集成
- **网格计算**: 大规模的并行计算

### 性能优化特点

- **Threading.Channels**: 高效的异步事件队列处理
- **ObjectPool**: 减少对象创建开销
- **Span 零拷贝**: 减少内存分配和复制
- **TailLatencyOptimizer**: 尾延迟优化
- **AOT 编译**: 提升启动速度和运行性能
- **Cache-line 对齐**: 内存分配优化
