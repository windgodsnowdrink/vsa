# Message - 使用示例

## 快速开始

### 1. 消息队列基本使用示例

```csharp
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

// 消息队列接口
public interface IMessageQueue<T>
{
    Task SendAsync(T message, CancellationToken cancellationToken = default);
    Task<T> ReceiveAsync(CancellationToken cancellationToken = default);
    int Count { get; }
}

// 消息队列实现
public class MessageQueue<T> : IMessageQueue<T>
{
    private readonly Channel<T> _channel;
    
    public MessageQueue(int capacity = 10000)
    {
        _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.Wait
        });
    }
    
    public async Task SendAsync(T message, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(message, cancellationToken);
    }
    
    public async Task<T> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
    
    public int Count => _channel.Reader.Count;
}

// 扩展方法
public static class MessageQueueExtensions
{
    public static IServiceCollection AddMessageQueue<T>(this IServiceCollection services, int capacity = 10000)
    {
        services.AddSingleton<IMessageQueue<T>>(_ => new MessageQueue<T>(capacity));
        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("消息队列基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var services = new ServiceCollection();
        services.AddMessageQueue<string>();
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取消息队列
        var messageQueue = serviceProvider.GetRequiredService<IMessageQueue<string>>();
        
        // 发送消息
        Console.WriteLine("发送消息...");
        await messageQueue.SendAsync("Hello, Message Queue!");
        await messageQueue.SendAsync("这是第二条消息");
        await messageQueue.SendAsync("这是第三条消息");
        
        Console.WriteLine($"队列中的消息数量: {messageQueue.Count}");
        
        // 接收消息
        Console.WriteLine("\n接收消息...");
        for (int i = 0; i < 3; i++)
        {
            var message = await messageQueue.ReceiveAsync();
            Console.WriteLine($"接收到消息: {message}");
        }
        
        Console.WriteLine($"队列中的消息数量: {messageQueue.Count}");
        Console.WriteLine("\n示例执行完成");
    }
}
```

### 2. 消息压缩示例

```csharp
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

public class MessageCompressor
{
    private const int CompressionThreshold = 1024; // 1KB
    
    public byte[] Compress(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        
        if (bytes.Length < CompressionThreshold)
        {
            return bytes;
        }
        
        using var memoryStream = new MemoryStream();
        using var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal);
        gzipStream.Write(bytes, 0, bytes.Length);
        gzipStream.Flush();
        
        return memoryStream.ToArray();
    }
    
    public string Decompress(byte[] compressedData)
    {
        if (compressedData.Length < 4) // GZip header is at least 4 bytes
        {
            return Encoding.UTF8.GetString(compressedData);
        }
        
        using var memoryStream = new MemoryStream(compressedData);
        using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream, Encoding.UTF8);
        
        return reader.ReadToEnd();
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("消息压缩示例");
        Console.WriteLine("=" * 50);
        
        var compressor = new MessageCompressor();
        
        // 测试短消息
        var shortMessage = "Hello, World!";
        var compressedShort = compressor.Compress(shortMessage);
        var decompressedShort = compressor.Decompress(compressedShort);
        
        Console.WriteLine("短消息测试:");
        Console.WriteLine($"原始长度: {Encoding.UTF8.GetBytes(shortMessage).Length} 字节");
        Console.WriteLine($"压缩后长度: {compressedShort.Length} 字节");
        Console.WriteLine($"解压缩后: {decompressedShort}");
        
        // 测试长消息
        var longMessage = new string('A', 10000); // 10KB 消息
        var compressedLong = compressor.Compress(longMessage);
        var decompressedLong = compressor.Decompress(compressedLong);
        
        Console.WriteLine("\n长消息测试:");
        Console.WriteLine($"原始长度: {Encoding.UTF8.GetBytes(longMessage).Length} 字节");
        Console.WriteLine($"压缩后长度: {compressedLong.Length} 字节");
        Console.WriteLine($"压缩率: {(1 - (double)compressedLong.Length / Encoding.UTF8.GetBytes(longMessage).Length) * 100:F2}%");
        Console.WriteLine($"解压缩后长度: {decompressedLong.Length} 字符");
        
        Console.WriteLine("\n示例执行完成");
    }
}
```

### 3. 消息持久化示例

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

public class MessagePersistence
{
    private readonly string _storagePath;
    
    public MessagePersistence(string storagePath = "./messages")
    {
        _storagePath = storagePath;
        Directory.CreateDirectory(_storagePath);
    }
    
    public async Task PersistAsync<T>(string id, T message)
    {
        var filePath = Path.Combine(_storagePath, $"{id}.msg");
        var data = MessagePackSerializer.Serialize(message);
        await File.WriteAllBytesAsync(filePath, data);
    }
    
    public async Task<T> LoadAsync<T>(string id)
    {
        var filePath = Path.Combine(_storagePath, $"{id}.msg");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"消息不存在: {id}");
        }
        
        var data = await File.ReadAllBytesAsync(filePath);
        return MessagePackSerializer.Deserialize<T>(data);
    }
    
    public IEnumerable<string> GetAllMessageIds()
    {
        return Directory.GetFiles(_storagePath, "*.msg")
            .Select(Path.GetFileNameWithoutExtension);
    }
    
    public void Delete(string id)
    {
        var filePath = Path.Combine(_storagePath, $"{id}.msg");
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("消息持久化示例");
        Console.WriteLine("=" * 50);
        
        var persistence = new MessagePersistence();
        
        // 保存消息
        Console.WriteLine("保存消息...");
        var message1 = new { Id = 1, Content = "Hello, Persistence!", Timestamp = DateTime.Now };
        var message2 = new { Id = 2, Content = "这是第二条消息", Timestamp = DateTime.Now };
        
        await persistence.PersistAsync("message1", message1);
        await persistence.PersistAsync("message2", message2);
        
        // 加载消息
        Console.WriteLine("\n加载消息...");
        var loadedMessage1 = await persistence.LoadAsync<dynamic>("message1");
        var loadedMessage2 = await persistence.LoadAsync<dynamic>("message2");
        
        Console.WriteLine($"消息1: {loadedMessage1.Content}, 时间: {loadedMessage1.Timestamp}");
        Console.WriteLine($"消息2: {loadedMessage2.Content}, 时间: {loadedMessage2.Timestamp}");
        
        // 获取所有消息ID
        Console.WriteLine("\n所有消息ID:");
        foreach (var id in persistence.GetAllMessageIds())
        {
            Console.WriteLine($"- {id}");
        }
        
        // 删除消息
        Console.WriteLine("\n删除消息...");
        persistence.Delete("message1");
        
        // 再次获取所有消息ID
        Console.WriteLine("\n删除后的消息ID:");
        foreach (var id in persistence.GetAllMessageIds())
        {
            Console.WriteLine($"- {id}");
        }
        
        Console.WriteLine("\n示例执行完成");
    }
}
```

### 4. 消息序列化示例

```csharp
using System;
using System.Text;
using MessagePack;

[MessagePackObject]
public class Message
{
    [Key(0)]
    public int Id { get; set; }
    
    [Key(1)]
    public string Content { get; set; }
    
    [Key(2)]
    public DateTime Timestamp { get; set; }
    
    [Key(3)]
    public string Sender { get; set; }
}

public class MessageSerializer
{
    public byte[] Serialize<T>(T message)
    {
        return MessagePackSerializer.Serialize(message);
    }
    
    public T Deserialize<T>(byte[] data)
    {
        return MessagePackSerializer.Deserialize<T>(data);
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("消息序列化示例");
        Console.WriteLine("=" * 50);
        
        var serializer = new MessageSerializer();
        
        // 创建消息
        var message = new Message
        {
            Id = 1,
            Content = "Hello, Serialization!",
            Timestamp = DateTime.Now,
            Sender = "User1"
        };
        
        // 序列化
        Console.WriteLine("序列化消息...");
        var serializedData = serializer.Serialize(message);
        Console.WriteLine($"序列化后大小: {serializedData.Length} 字节");
        Console.WriteLine($"序列化后数据: {BitConverter.ToString(serializedData)}");
        
        // 反序列化
        Console.WriteLine("\n反序列化消息...");
        var deserializedMessage = serializer.Deserialize<Message>(serializedData);
        Console.WriteLine($"ID: {deserializedMessage.Id}");
        Console.WriteLine($"内容: {deserializedMessage.Content}");
        Console.WriteLine($"时间: {deserializedMessage.Timestamp}");
        Console.WriteLine($"发送者: {deserializedMessage.Sender}");
        
        // 与JSON比较
        Console.WriteLine("\n与JSON比较:");
        var json = System.Text.Json.JsonSerializer.Serialize(message);
        Console.WriteLine($"JSON大小: {Encoding.UTF8.GetByteCount(json)} 字节");
        Console.WriteLine($"JSON数据: {json}");
        
        Console.WriteLine("\n示例执行完成");
    }
}
```

### 5. 高性能消息处理示例

```csharp
using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;

public class HighPerformanceMessageProcessor
{
    private readonly Channel<string> _channel;
    private readonly int _workerCount;
    private readonly CancellationTokenSource _cts;
    private readonly List<Task> _workers;
    
    public HighPerformanceMessageProcessor(int capacity = 10000, int workerCount = 4)
    {
        _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity)
        {
            SingleReader = false,
            SingleWriter = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.Wait
        });
        
        _workerCount = workerCount;
        _cts = new CancellationTokenSource();
        _workers = new List<Task>();
    }
    
    public void Start()
    {
        for (int i = 0; i < _workerCount; i++)
        {
            var workerId = i;
            _workers.Add(Task.Run(() => ProcessMessagesAsync(workerId, _cts.Token)));
        }
        
        Console.WriteLine($"启动了 {_workerCount} 个工作线程");
    }
    
    public async Task SendAsync(string message)
    {
        await _channel.Writer.WriteAsync(message, _cts.Token);
    }
    
    public async Task StopAsync()
    {
        _cts.Cancel();
        await Task.WhenAll(_workers);
        Console.WriteLine("所有工作线程已停止");
    }
    
    private async Task ProcessMessagesAsync(int workerId, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                // 处理消息
                Console.WriteLine($"Worker {workerId} 处理消息: {message}");
                
                // 模拟处理时间
                await Task.Delay(100, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"Worker {workerId} 被取消");
        }
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("高性能消息处理示例");
        Console.WriteLine("=" * 50);
        
        var processor = new HighPerformanceMessageProcessor(workerCount: 4);
        processor.Start();
        
        // 发送消息
        Console.WriteLine("发送消息...");
        for (int i = 0; i < 20; i++)
        {
            await processor.SendAsync($"消息 {i}");
        }
        
        // 等待处理完成
        await Task.Delay(3000);
        
        // 停止处理器
        await processor.StopAsync();
        
        Console.WriteLine("\n示例执行完成");
    }
}
```

### 6. 消息历史记录示例

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

[MessagePackObject]
public class MessageHistory
{
    [Key(0)]
    public string Id { get; set; }
    
    [Key(1)]
    public string Content { get; set; }
    
    [Key(2)]
    public DateTime Timestamp { get; set; }
    
    [Key(3)]
    public string Status { get; set; }
}

public class HistoryService
{
    private readonly string _historyPath;
    private readonly List<MessageHistory> _history;
    
    public HistoryService(string historyPath = "./history")
    {
        _historyPath = historyPath;
        _history = new List<MessageHistory>();
        Directory.CreateDirectory(_historyPath);
        LoadHistory();
    }
    
    public void Add(string content, string status = "Sent")
    {
        var history = new MessageHistory
        {
            Id = Guid.NewGuid().ToString(),
            Content = content,
            Timestamp = DateTime.Now,
            Status = status
        };
        
        _history.Add(history);
        SaveHistory();
    }
    
    public List<MessageHistory> GetAll()
    {
        return _history;
    }
    
    public List<MessageHistory> GetByStatus(string status)
    {
        return _history.Where(h => h.Status == status).ToList();
    }
    
    public MessageHistory GetById(string id)
    {
        return _history.FirstOrDefault(h => h.Id == id);
    }
    
    public void UpdateStatus(string id, string status)
    {
        var history = GetById(id);
        if (history != null)
        {
            history.Status = status;
            SaveHistory();
        }
    }
    
    private void SaveHistory()
    {
        var filePath = Path.Combine(_historyPath, "history.msgpack");
        var data = MessagePackSerializer.Serialize(_history);
        File.WriteAllBytes(filePath, data);
    }
    
    private void LoadHistory()
    {
        var filePath = Path.Combine(_historyPath, "history.msgpack");
        if (File.Exists(filePath))
        {
            var data = File.ReadAllBytes(filePath);
            _history.AddRange(MessagePackSerializer.Deserialize<List<MessageHistory>>(data));
        }
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("消息历史记录示例");
        Console.WriteLine("=" * 50);
        
        var historyService = new HistoryService();
        
        // 添加消息历史
        Console.WriteLine("添加消息历史...");
        historyService.Add("Hello, History!");
        historyService.Add("这是第二条消息");
        historyService.Add("这是第三条消息");
        
        // 更新状态
        Console.WriteLine("\n更新消息状态...");
        var allHistory = historyService.GetAll();
        if (allHistory.Count > 0)
        {
            historyService.UpdateStatus(allHistory[0].Id, "Processed");
            Console.WriteLine($"更新了消息 {allHistory[0].Id} 的状态为 Processed");
        }
        
        // 获取所有历史
        Console.WriteLine("\n所有消息历史:");
        allHistory = historyService.GetAll();
        foreach (var history in allHistory)
        {
            Console.WriteLine($"ID: {history.Id}");
            Console.WriteLine($"内容: {history.Content}");
            Console.WriteLine($"时间: {history.Timestamp}");
            Console.WriteLine($"状态: {history.Status}");
            Console.WriteLine("-");
        }
        
        // 按状态获取
        Console.WriteLine("\n按状态获取消息:");
        var processedMessages = historyService.GetByStatus("Processed");
        Console.WriteLine($"已处理的消息数量: {processedMessages.Count}");
        
        var sentMessages = historyService.GetByStatus("Sent");
        Console.WriteLine($"已发送的消息数量: {sentMessages.Count}");
        
        Console.WriteLine("\n示例执行完成");
    }
}
```

### 7. AOT 编译示例

```csharp
//:sdk Microsoft.NET.Sdk
//:package Microsoft.Extensions.DependencyInjection@10.0.0
//:package Microsoft.Extensions.Logging@10.0.0
//:package MessagePack@2.0.0
//:package System.Threading.Channels@7.0.0
//:property LangVersion=preview
//:property TargetFramework=net10.0
//:property Nullable=enable
//:property ImplicitUsings=enable
//:property PublishAot=true
//:property IncludeNativeLibrariesForSelfExtract=true
//:property EnableCppCodeGen=true
//:property PublishSingleFile=true
//:property SelfContained=true
//:property RuntimeIdentifier=win-x64

using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MessagePack;

[MessagePackObject]
public class AotMessage
{
    [Key(0)]
    public string Content { get; set; }
    
    [Key(1)]
    public DateTime Timestamp { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Message AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 测试消息队列
        await TestMessageQueue();
        
        // 测试消息序列化
        TestMessageSerialization();
        
        Console.WriteLine("\nAOT 编译示例执行完成");
    }
    
    private static async Task TestMessageQueue()
    {
        Console.WriteLine("\n测试消息队列:");
        
        var channel = Channel.CreateBounded<string>(100);
        
        // 启动消费者
        var consumerTask = Task.Run(async () => {
            await foreach (var message in channel.Reader.ReadAllAsync())
            {
                Console.WriteLine($"接收到消息: {message}");
            }
        });
        
        // 发送消息
        for (int i = 0; i < 5; i++)
        {
            await channel.Writer.WriteAsync($"AOT 消息 {i}");
            await Task.Delay(100);
        }
        
        channel.Writer.Complete();
        await consumerTask;
    }
    
    private static void TestMessageSerialization()
    {
        Console.WriteLine("\n测试消息序列化:");
        
        var message = new AotMessage
        {
            Content = "Hello, AOT Serialization!",
            Timestamp = DateTime.Now
        };
        
        // 序列化
        var serialized = MessagePackSerializer.Serialize(message);
        Console.WriteLine($"序列化后大小: {serialized.Length} 字节");
        
        // 反序列化
        var deserialized = MessagePackSerializer.Deserialize<AotMessage>(serialized);
        Console.WriteLine($"内容: {deserialized.Content}");
        Console.WriteLine($"时间: {deserialized.Timestamp}");
    }
}
```

## 总结

以上示例展示了 Message 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速开始**：快速上手消息队列、消息压缩、消息持久化等基本操作
2. **高级配置**：配置消息处理服务的高级选项
3. **性能优化**：使用高性能消息处理方案
4. **错误处理**：正确处理消息处理中的异常情况
5. **消息序列化**：使用高效的消息序列化方案
6. **消息持久化**：确保消息不丢失
7. **消息历史记录**：跟踪消息处理状态
8. **AOT 编译**：使用 AOT 编译提高性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
