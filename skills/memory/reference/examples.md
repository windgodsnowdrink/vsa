# Memory - 使用示例

## 快速开始

### 1. 内存池基本使用示例

```csharp
using System;
using System.Buffers;
using System.Text;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("内存池基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 获取内存池
        var memoryPool = MemoryPool<byte>.Shared;
        
        // 从内存池租用内存
        using var owner = memoryPool.Rent(1024);
        var memory = owner.Memory;
        
        // 使用内存
        var span = memory.Span;
        var message = "Hello, Memory Pool!";
        var bytes = Encoding.UTF8.GetBytes(message);
        
        // 将数据复制到内存
        bytes.CopyTo(span);
        
        // 处理内存数据
        var readMessage = Encoding.UTF8.GetString(span.Slice(0, bytes.Length));
        Console.WriteLine($"读取的消息: {readMessage}");
        
        // 内存会在using块结束时自动归还到内存池
        Console.WriteLine("内存已归还到内存池");
    }
}
```

### 2. 可回收内存流示例

```csharp
using System;
using System.IO;
using System.Text;
using Microsoft.IO;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("可回收内存流示例");
        Console.WriteLine("=" * 50);
        
        // 创建内存流管理器
        var manager = new RecyclableMemoryStreamManager();
        
        // 创建可回收内存流
        using var stream = manager.GetStream();
        
        // 写入数据
        var writer = new StreamWriter(stream);
        writer.WriteLine("Hello, Recyclable Memory Stream!");
        writer.WriteLine("This is a test.");
        writer.Flush();
        
        // 重置流位置
        stream.Position = 0;
        
        // 读取数据
        var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        Console.WriteLine("读取的内容:");
        Console.WriteLine(content);
        
        // 流会在using块结束时自动归还到内存流管理器
        Console.WriteLine("内存流已归还到管理器");
    }
}
```

### 3. Span<T> 和 Memory<T> 优化示例

```csharp
using System;
using System.Buffers;
using System.Text;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Span<T> 和 Memory<T> 优化示例");
        Console.WriteLine("=" * 50);
        
        // 使用Span<T>处理字符串
        var text = "Hello, Span and Memory!";
        var textSpan = text.AsSpan();
        
        // 查找子字符串
        var helloIndex = textSpan.IndexOf("Hello");
        var spanIndex = textSpan.IndexOf("Span");
        var memoryIndex = textSpan.IndexOf("Memory");
        
        Console.WriteLine($"'Hello' 位置: {helloIndex}");
        Console.WriteLine($"'Span' 位置: {spanIndex}");
        Console.WriteLine($"'Memory' 位置: {memoryIndex}");
        
        // 使用Memory<T>处理数组
        var array = new int[10];
        var memory = array.AsMemory();
        
        // 填充数据
        for (int i = 0; i < memory.Length; i++)
        {
            memory.Span[i] = i * 2;
        }
        
        // 处理数据
        Console.WriteLine("数组内容:");
        foreach (var item in memory.Span)
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
    }
}
```

### 4. 内存泄漏检测示例

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public class Program
{
    private static List<byte[]> _memoryLeaks = new List<byte[]>();
    
    public static void Main()
    {
        Console.WriteLine("内存泄漏检测示例");
        Console.WriteLine("=" * 50);
        
        // 监控内存使用
        var process = Process.GetCurrentProcess();
        var initialMemory = process.WorkingSet64;
        Console.WriteLine($"初始内存使用: {initialMemory / 1024 / 1024} MB");
        
        // 模拟内存泄漏
        Console.WriteLine("模拟内存泄漏...");
        for (int i = 0; i < 100; i++)
        {
            // 分配内存但不释放
            var buffer = new byte[1024 * 1024]; // 1MB
            _memoryLeaks.Add(buffer);
            
            // 每10次迭代显示内存使用
            if (i % 10 == 0)
            {
                process.Refresh();
                var currentMemory = process.WorkingSet64;
                Console.WriteLine($"迭代 {i}: 内存使用: {currentMemory / 1024 / 1024} MB");
            }
            
            Thread.Sleep(50);
        }
        
        // 显示最终内存使用
        process.Refresh();
        var finalMemory = process.WorkingSet64;
        Console.WriteLine($"最终内存使用: {finalMemory / 1024 / 1024} MB");
        Console.WriteLine($"内存增长: {(finalMemory - initialMemory) / 1024 / 1024} MB");
        
        // 清理内存
        Console.WriteLine("清理内存...");
        _memoryLeaks.Clear();
        
        // 强制GC
        GC.Collect();
        GC.WaitForPendingFinalizers();
        
        // 显示清理后的内存使用
        process.Refresh();
        var cleanedMemory = process.WorkingSet64;
        Console.WriteLine($"清理后内存使用: {cleanedMemory / 1024 / 1024} MB");
        Console.WriteLine($"内存释放: {(finalMemory - cleanedMemory) / 1024 / 1024} MB");
    }
}
```

### 5. 分层内存管理示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("分层内存管理示例");
        Console.WriteLine("=" * 50);
        
        // 创建分层内存管理器
        var memoryManager = new TieredMemoryManager();
        
        // 添加热数据
        Console.WriteLine("添加热数据...");
        for (int i = 0; i < 5; i++)
        {
            var data = new DataItem { Id = i, Value = $"Hot Data {i}", AccessTime = DateTime.UtcNow };
            memoryManager.AddData(data);
            Console.WriteLine($"添加热数据: {data.Value}");
        }
        
        // 添加温数据
        Console.WriteLine("\n添加温数据...");
        for (int i = 5; i < 10; i++)
        {
            var data = new DataItem { Id = i, Value = $"Warm Data {i}", AccessTime = DateTime.UtcNow.AddMinutes(-10) };
            memoryManager.AddData(data);
            Console.WriteLine($"添加温数据: {data.Value}");
        }
        
        // 添加冷数据
        Console.WriteLine("\n添加冷数据...");
        for (int i = 10; i < 15; i++)
        {
            var data = new DataItem { Id = i, Value = $"Cold Data {i}", AccessTime = DateTime.UtcNow.AddHours(-1) };
            memoryManager.AddData(data);
            Console.WriteLine($"添加冷数据: {data.Value}");
        }
        
        // 访问数据，将冷数据变为热数据
        Console.WriteLine("\n访问冷数据...");
        var coldData = memoryManager.GetData(10);
        if (coldData != null)
        {
            Console.WriteLine($"访问冷数据: {coldData.Value}");
        }
        
        // 显示内存分层状态
        Console.WriteLine("\n内存分层状态:");
        memoryManager.DisplayStatus();
    }
}

// 数据项
public class DataItem
{
    public int Id { get; set; }
    public string Value { get; set; }
    public DateTime AccessTime { get; set; }
}

// 分层内存管理器
public class TieredMemoryManager
{
    private List<DataItem> _hotData = new List<DataItem>();
    private List<DataItem> _warmData = new List<DataItem>();
    private List<DataItem> _coldData = new List<DataItem>();
    
    public void AddData(DataItem data)
    {
        if (data.AccessTime > DateTime.UtcNow.AddMinutes(-5))
        {
            _hotData.Add(data);
        }
        else if (data.AccessTime > DateTime.UtcNow.AddMinutes(-30))
        {
            _warmData.Add(data);
        }
        else
        {
            _coldData.Add(data);
        }
    }
    
    public DataItem GetData(int id)
    {
        // 先在热数据中查找
        var data = _hotData.Find(d => d.Id == id);
        if (data != null)
        {
            data.AccessTime = DateTime.UtcNow;
            return data;
        }
        
        // 在温数据中查找
        data = _warmData.Find(d => d.Id == id);
        if (data != null)
        {
            // 移到热数据
            _warmData.Remove(data);
            data.AccessTime = DateTime.UtcNow;
            _hotData.Add(data);
            return data;
        }
        
        // 在冷数据中查找
        data = _coldData.Find(d => d.Id == id);
        if (data != null)
        {
            // 移到热数据
            _coldData.Remove(data);
            data.AccessTime = DateTime.UtcNow;
            _hotData.Add(data);
            return data;
        }
        
        return null;
    }
    
    public void DisplayStatus()
    {
        Console.WriteLine($"热数据数量: {_hotData.Count}");
        Console.WriteLine($"温数据数量: {_warmData.Count}");
        Console.WriteLine($"冷数据数量: {_coldData.Count}");
        
        Console.WriteLine("\n热数据:");
        foreach (var item in _hotData)
        {
            Console.WriteLine($"  ID: {item.Id}, Value: {item.Value}");
        }
        
        Console.WriteLine("\n温数据:");
        foreach (var item in _warmData)
        {
            Console.WriteLine($"  ID: {item.Id}, Value: {item.Value}");
        }
        
        Console.WriteLine("\n冷数据:");
        foreach (var item in _coldData)
        {
            Console.WriteLine($"  ID: {item.Id}, Value: {item.Value}");
        }
    }
}
```

### 6. 共享内存示例

```csharp
using System;
using System.IO.MemoryMappedFiles;
using System.Text;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("共享内存示例");
        Console.WriteLine("=" * 50);
        
        // 创建共享内存
        using var mmf = MemoryMappedFile.CreateNew("SharedMemoryDemo", 1024);
        
        // 写入数据到共享内存
        using var accessor = mmf.CreateViewAccessor();
        var message = "Hello, Shared Memory!";
        var bytes = Encoding.UTF8.GetBytes(message);
        
        accessor.WriteArray(0, bytes, 0, bytes.Length);
        Console.WriteLine($"写入到共享内存: {message}");
        
        // 从共享内存读取数据
        var readBytes = new byte[1024];
        accessor.ReadArray(0, readBytes, 0, bytes.Length);
        var readMessage = Encoding.UTF8.GetString(readBytes, 0, bytes.Length);
        Console.WriteLine($"从共享内存读取: {readMessage}");
        
        // 共享内存会在using块结束时自动释放
        Console.WriteLine("共享内存已释放");
    }
}
```

### 7. 内存池性能优化示例

```csharp
using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("内存池性能优化示例");
        Console.WriteLine("=" * 50);
        
        const int iterations = 1000000;
        var stopwatch = new Stopwatch();
        
        // 测试直接分配内存
        Console.WriteLine("测试直接分配内存...");
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            var buffer = new byte[1024];
            // 使用缓冲区
            buffer[0] = 0x48;
        }
        stopwatch.Stop();
        Console.WriteLine($"直接分配内存: {stopwatch.ElapsedMilliseconds} ms");
        
        // 测试使用内存池
        Console.WriteLine("\n测试使用内存池...");
        var memoryPool = MemoryPool<byte>.Shared;
        stopwatch.Reset();
        stopwatch.Start();
        for (int i = 0; i < iterations; i++)
        {
            using var owner = memoryPool.Rent(1024);
            var buffer = owner.Memory.Span;
            // 使用缓冲区
            buffer[0] = 0x48;
        }
        stopwatch.Stop();
        Console.WriteLine($"使用内存池: {stopwatch.ElapsedMilliseconds} ms");
        
        Console.WriteLine("\n测试完成!");
    }
}
```

### 8. AOT 编译示例

```csharp
//:sdk Microsoft.NET.Sdk
//:package Microsoft.Extensions.DependencyInjection@10.0.0
//:package Microsoft.Extensions.Logging@10.0.0
//:package Microsoft.IO.RecyclableMemoryStream@3.0.0
//:property LangVersion=preview
//:property TargetFramework=net11.0
//:property Nullable=enable
//:property ImplicitUsings=enable
//:property PublishAot=true
//:property IncludeNativeLibrariesForSelfExtract=true
//:property EnableCppCodeGen=true
//:property PublishSingleFile=true
//:property SelfContained=true
//:property RuntimeIdentifier=win-x64

using System;
using System.Buffers;
using System.Text;
using Microsoft.IO;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Memory AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 测试内存池
        TestMemoryPool();
        
        // 测试可回收内存流
        TestRecyclableMemoryStream();
        
        // 测试 Span<T> 和 Memory<T>
        TestSpanAndMemory();
        
        Console.WriteLine("\nAOT 编译示例执行完成");
    }
    
    private static void TestMemoryPool()
    {
        Console.WriteLine("\n测试内存池:");
        var memoryPool = MemoryPool<byte>.Shared;
        
        using var owner = memoryPool.Rent(1024);
        var memory = owner.Memory;
        var span = memory.Span;
        
        var message = "Hello, Memory Pool!";
        var bytes = Encoding.UTF8.GetBytes(message);
        bytes.CopyTo(span);
        
        var readMessage = Encoding.UTF8.GetString(span.Slice(0, bytes.Length));
        Console.WriteLine($"内存池测试结果: {readMessage}");
    }
    
    private static void TestRecyclableMemoryStream()
    {
        Console.WriteLine("\n测试可回收内存流:");
        var manager = new RecyclableMemoryStreamManager();
        
        using var stream = manager.GetStream();
        using var writer = new System.IO.StreamWriter(stream);
        writer.WriteLine("Hello, Recyclable Memory Stream!");
        writer.Flush();
        
        stream.Position = 0;
        using var reader = new System.IO.StreamReader(stream);
        var content = reader.ReadToEnd();
        Console.WriteLine($"可回收内存流测试结果: {content.Trim()}");
    }
    
    private static void TestSpanAndMemory()
    {
        Console.WriteLine("\n测试 Span<T> 和 Memory<T>:");
        var text = "Hello, Span and Memory!";
        var textSpan = text.AsSpan();
        
        var helloIndex = textSpan.IndexOf("Hello");
        var spanIndex = textSpan.IndexOf("Span");
        var memoryIndex = textSpan.IndexOf("Memory");
        
        Console.WriteLine($"'Hello' 位置: {helloIndex}");
        Console.WriteLine($"'Span' 位置: {spanIndex}");
        Console.WriteLine($"'Memory' 位置: {memoryIndex}");
    }
}
```

## 总结

以上示例展示了 Memory 技能的主要功能和使用方法。通过这些示例，您可以：

1. **快速开始**：快速上手内存池、可回收内存流等基本操作
2. **高级配置**：配置内存管理服务的高级选项
3. **性能优化**：使用内存池、Span<T> 和 Memory<T> 优化内存使用
4. **内存泄漏检测**：检测和预防内存泄漏
5. **分层内存管理**：根据数据特性使用不同级别的内存存储
6. **共享内存**：实现进程间共享内存
7. **AOT 编译**：使用 AOT 编译提高性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。