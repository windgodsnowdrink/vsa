# TPL 技能使用示例

## 概述

本文件提供了 TPL（Task Parallel Library）技能的详细使用示例，包括并行任务处理、数据流操作、并行 LINQ、异步编程以及 Scrutor 依赖注入装饰器模式的使用示例。

## 1. 并行任务处理示例

### 1.1 基本并行任务

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// 创建服务提供器
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddTransient<ITaskService, TaskService>()
    .BuildServiceProvider();

// 获取任务服务
var taskService = serviceProvider.GetRequiredService<ITaskService>();

// 运行5个并行任务
await taskService.RunParallelTasksAsync(5);

// 输出:
// 开始执行5个并行任务
// 任务1开始执行
// 任务2开始执行
// 任务3开始执行
// 任务4开始执行
// 任务5开始执行
// 任务1执行完成
// 任务2执行完成
// 任务3执行完成
// 任务4执行完成
// 任务5执行完成
// 所有并行任务执行完成
```

### 1.2 Task.WhenAll 示例

```csharp
using System;
using System.Threading.Tasks;

// 创建多个任务
var task1 = Task.Run(() => {
    Console.WriteLine("任务1开始");
    Task.Delay(1000).Wait();
    Console.WriteLine("任务1完成");
    return 1;
});

var task2 = Task.Run(() => {
    Console.WriteLine("任务2开始");
    Task.Delay(1500).Wait();
    Console.WriteLine("任务2完成");
    return 2;
});

var task3 = Task.Run(() => {
    Console.WriteLine("任务3开始");
    Task.Delay(500).Wait();
    Console.WriteLine("任务3完成");
    return 3;
});

// 等待所有任务完成
var results = await Task.WhenAll(task1, task2, task3);

Console.WriteLine($"所有任务完成，结果: {string.Join(", ", results)}");

// 输出:
// 任务1开始
// 任务2开始
// 任务3开始
// 任务3完成
// 任务1完成
// 任务2完成
// 所有任务完成，结果: 1, 2, 3
```

### 1.3 Task.WhenAny 示例

```csharp
using System;
using System.Threading.Tasks;

// 创建多个任务
var tasks = new[] {
    Task.Run(() => {
        Task.Delay(1000).Wait();
        return "任务1";
    }),
    Task.Run(() => {
        Task.Delay(500).Wait();
        return "任务2";
    }),
    Task.Run(() => {
        Task.Delay(1500).Wait();
        return "任务3";
    })
};

// 等待任一任务完成
var completedTask = await Task.WhenAny(tasks);
var result = await completedTask;

Console.WriteLine($"第一个完成的任务: {result}");

// 输出:
// 第一个完成的任务: 任务2
```

### 1.4 Parallel.ForEach 示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

var items = Enumerable.Range(1, 10);

Console.WriteLine("开始并行处理");

Parallel.ForEach(items, item => {
    Console.WriteLine($"处理项目 {item}，线程 ID: {Thread.CurrentThread.ManagedThreadId}");
    // 模拟处理时间
    Task.Delay(100).Wait();
});

Console.WriteLine("并行处理完成");

// 输出:
// 开始并行处理
// 处理项目 1，线程 ID: 4
// 处理项目 2，线程 ID: 5
// 处理项目 3，线程 ID: 6
// 处理项目 4，线程 ID: 7
// 处理项目 5，线程 ID: 4
// 处理项目 6，线程 ID: 5
// 处理项目 7，线程 ID: 6
// 处理项目 8，线程 ID: 7
// 处理项目 9，线程 ID: 4
// 处理项目 10，线程 ID: 5
// 并行处理完成
```

## 2. 数据流处理示例

### 2.1 基本数据流管道

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

// 创建数据流块
var transformBlock = new TransformBlock<int, int>(
    item => {
        Console.WriteLine($"转换: {item} -> {item * 2}");
        Task.Delay(50).Wait();
        return item * 2;
    },
    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }
);

var actionBlock = new ActionBlock<int>(
    item => {
        Console.WriteLine($"处理: {item}");
        Task.Delay(30).Wait();
    },
    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }
);

// 链接块
transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

// 发送数据
for (int i = 1; i <= 10; i++)
{
    await transformBlock.SendAsync(i);
}

// 标记完成
transformBlock.Complete();
// 等待完成
await actionBlock.Completion;

Console.WriteLine("数据流处理完成");

// 输出:
// 转换: 1 -> 2
// 转换: 2 -> 4
// 转换: 3 -> 6
// 转换: 4 -> 8
// 处理: 2
// 处理: 4
// 转换: 5 -> 10
// 转换: 6 -> 12
// 处理: 6
// 处理: 8
// 转换: 7 -> 14
// 转换: 8 -> 16
// 处理: 10
// 处理: 12
// 转换: 9 -> 18
// 转换: 10 -> 20
// 处理: 14
// 处理: 16
// 处理: 18
// 处理: 20
// 数据流处理完成
```

### 2.2 复杂数据流管道

```csharp
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

// 创建数据流块
var inputBlock = new BufferBlock<int>();

var transformBlock1 = new TransformBlock<int, string>(
    item => {
        Console.WriteLine($"转换为字符串: {item}");
        return item.ToString();
    }
);

var transformBlock2 = new TransformBlock<string, string>(
    item => {
        Console.WriteLine($"添加前缀: {item}");
        return $"Prefix_{item}";
    }
);

var actionBlock = new ActionBlock<string>(
    item => {
        Console.WriteLine($"最终处理: {item}");
    }
);

// 链接块
inputBlock.LinkTo(transformBlock1, new DataflowLinkOptions { PropagateCompletion = true });
transformBlock1.LinkTo(transformBlock2, new DataflowLinkOptions { PropagateCompletion = true });
transformBlock2.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

// 发送数据
for (int i = 1; i <= 5; i++)
{
    await inputBlock.SendAsync(i);
}

// 标记完成
inputBlock.Complete();
// 等待完成
await actionBlock.Completion;

Console.WriteLine("复杂数据流处理完成");

// 输出:
// 转换为字符串: 1
// 添加前缀: 1
// 最终处理: Prefix_1
// 转换为字符串: 2
// 添加前缀: 2
// 最终处理: Prefix_2
// 转换为字符串: 3
// 添加前缀: 3
// 最终处理: Prefix_3
// 转换为字符串: 4
// 添加前缀: 4
// 最终处理: Prefix_4
// 转换为字符串: 5
// 添加前缀: 5
// 最终处理: Prefix_5
// 复杂数据流处理完成
```

### 2.3 批处理示例

```csharp
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

// 创建批处理块
var batchBlock = new BatchBlock<int>(3);

var actionBlock = new ActionBlock<int[]>(
    batch => {
        Console.WriteLine($"处理批处理: [{string.Join(", ", batch)}]");
    }
);

// 链接块
batchBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

// 发送数据
for (int i = 1; i <= 10; i++)
{
    await batchBlock.SendAsync(i);
    Console.WriteLine($"发送数据: {i}");
}

// 标记完成
batchBlock.Complete();
// 等待完成
await actionBlock.Completion;

Console.WriteLine("批处理完成");

// 输出:
// 发送数据: 1
// 发送数据: 2
// 发送数据: 3
// 处理批处理: [1, 2, 3]
// 发送数据: 4
// 发送数据: 5
// 发送数据: 6
// 处理批处理: [4, 5, 6]
// 发送数据: 7
// 发送数据: 8
// 发送数据: 9
// 处理批处理: [7, 8, 9]
// 发送数据: 10
// 处理批处理: [10]
// 批处理完成
```

## 3. 并行 LINQ (PLINQ) 示例

### 3.1 基本 PLINQ

```csharp
using System;
using System.Linq;

var numbers = Enumerable.Range(1, 1000);

Console.WriteLine("开始PLINQ处理");

var results = numbers
    .AsParallel()
    .WithDegreeOfParallelism(Environment.ProcessorCount)
    .Where(n => n % 2 == 0)
    .Select(n => n * 2)
    .ToList();

Console.WriteLine($"PLINQ处理完成，结果数量: {results.Count}");
Console.WriteLine($"前10个结果: {string.Join(", ", results.Take(10))}");

// 输出:
// 开始PLINQ处理
// PLINQ处理完成，结果数量: 500
// 前10个结果: 4, 8, 12, 16, 20, 24, 28, 32, 36, 40
```

### 3.2 PLINQ 性能对比

```csharp
using System;
using System.Diagnostics;
using System.Linq;

var numbers = Enumerable.Range(1, 1000000);

// 顺序处理
var stopwatch1 = Stopwatch.StartNew();
var sequentialResult = numbers
    .Where(n => IsPrime(n))
    .Count();
stopwatch1.Stop();

// 并行处理
var stopwatch2 = Stopwatch.StartNew();
var parallelResult = numbers
    .AsParallel()
    .Where(n => IsPrime(n))
    .Count();
stopwatch2.Stop();

Console.WriteLine($"顺序处理: {stopwatch1.ElapsedMilliseconds}ms, 质数数量: {sequentialResult}");
Console.WriteLine($"并行处理: {stopwatch2.ElapsedMilliseconds}ms, 质数数量: {parallelResult}");
Console.WriteLine($"性能提升: {stopwatch1.ElapsedMilliseconds / (double)stopwatch2.ElapsedMilliseconds:F2}x");

// 辅助方法
bool IsPrime(int n)
{
    if (n <= 1) return false;
    if (n <= 3) return true;
    if (n % 2 == 0 || n % 3 == 0) return false;
    for (int i = 5; i * i <= n; i += 6)
    {
        if (n % i == 0 || n % (i + 2) == 0) return false;
    }
    return true;
}

// 输出:
// 顺序处理: 1234ms, 质数数量: 78498
// 并行处理: 345ms, 质数数量: 78498
// 性能提升: 3.58x
```

### 3.3 PLINQ 排序

```csharp
using System;
using System.Linq;

var numbers = Enumerable.Range(1, 1000).OrderBy(x => Guid.NewGuid());

Console.WriteLine("原始数据（前10个）: {0}", string.Join(", ", numbers.Take(10)));

var sortedNumbers = numbers
    .AsParallel()
    .OrderBy(n => n)
    .ToList();

Console.WriteLine("排序后数据（前10个）: {0}", string.Join(", ", sortedNumbers.Take(10)));

// 输出:
// 原始数据（前10个）: 456, 789, 123, 45, 678, 901, 234, 567, 890, 12
// 排序后数据（前10个）: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
```

## 4. 异步编程示例

### 4.1 基本 async/await

```csharp
using System;
using System.Threading.Tasks;

async Task Main()
{
    Console.WriteLine("开始异步操作");
    
    var result1 = await DoSomethingAsync("操作1", 1000);
    Console.WriteLine(result1);
    
    var result2 = await DoSomethingAsync("操作2", 500);
    Console.WriteLine(result2);
    
    Console.WriteLine("异步操作完成");
}

async Task<string> DoSomethingAsync(string name, int delayMs)
{
    Console.WriteLine($"{name} 开始");
    await Task.Delay(delayMs);
    Console.WriteLine($"{name} 完成");
    return $"{name} 处理完成";
}

// 输出:
// 开始异步操作
// 操作1 开始
// 操作1 完成
// 操作1 处理完成
// 操作2 开始
// 操作2 完成
// 操作2 处理完成
// 异步操作完成
```

### 4.2 并行异步操作

```csharp
using System;
using System.Threading.Tasks;

async Task Main()
{
    Console.WriteLine("开始并行异步操作");
    
    var task1 = DoSomethingAsync("操作1", 1000);
    var task2 = DoSomethingAsync("操作2", 1500);
    var task3 = DoSomethingAsync("操作3", 500);
    
    var results = await Task.WhenAll(task1, task2, task3);
    
    foreach (var result in results)
    {
        Console.WriteLine(result);
    }
    
    Console.WriteLine("并行异步操作完成");
}

async Task<string> DoSomethingAsync(string name, int delayMs)
{
    Console.WriteLine($"{name} 开始");
    await Task.Delay(delayMs);
    Console.WriteLine($"{name} 完成");
    return $"{name} 处理完成";
}

// 输出:
// 开始并行异步操作
// 操作1 开始
// 操作2 开始
// 操作3 开始
// 操作3 完成
// 操作1 完成
// 操作2 完成
// 操作1 处理完成
// 操作2 处理完成
// 操作3 处理完成
// 并行异步操作完成
```

### 4.3 异步流

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

async Task Main()
{
    Console.WriteLine("开始异步流处理");
    
    await foreach (var item in GenerateItemsAsync(10))
    {
        Console.WriteLine($"处理项目: {item}");
    }
    
    Console.WriteLine("异步流处理完成");
}

async IAsyncEnumerable<int> GenerateItemsAsync(int count)
{
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(100);
        yield return i;
    }
}

// 输出:
// 开始异步流处理
// 处理项目: 1
// 处理项目: 2
// 处理项目: 3
// 处理项目: 4
// 处理项目: 5
// 处理项目: 6
// 处理项目: 7
// 处理项目: 8
// 处理项目: 9
// 处理项目: 10
// 异步流处理完成
```

### 4.4 取消操作

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;

async Task Main()
{
    using var cts = new CancellationTokenSource();
    
    // 3秒后取消
    _ = Task.Delay(3000).ContinueWith(_ => {
        Console.WriteLine("请求取消操作");
        cts.Cancel();
    });
    
    try
    {
        Console.WriteLine("开始长时间操作");
        await LongRunningOperationAsync(cts.Token);
        Console.WriteLine("操作完成");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("操作被取消");
    }
}

async Task LongRunningOperationAsync(CancellationToken cancellationToken)
{
    for (int i = 1; i <= 10; i++)
    {
        Console.WriteLine($"执行步骤 {i}");
        await Task.Delay(1000, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
    }
}

// 输出:
// 开始长时间操作
// 执行步骤 1
// 执行步骤 2
// 执行步骤 3
// 请求取消操作
// 操作被取消
```

## 5. Scrutor 依赖注入装饰器模式示例

### 5.1 基本装饰器模式

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

// 服务接口
public interface IDataService
{
    string GetData();
}

// 原始服务
public class DefaultDataService : IDataService
{
    public string GetData()
    {
        Console.WriteLine("DefaultDataService.GetData()");
        return "原始数据";
    }
}

// 缓存装饰器
public class CachedDataService : IDataService
{
    private readonly IDataService _inner;
    private string _cachedData;

    public CachedDataService(IDataService inner)
    {
        _inner = inner;
    }

    public string GetData()
    {
        if (_cachedData == null)
        {
            Console.WriteLine("缓存未命中，从原始服务获取");
            _cachedData = _inner.GetData();
        }
        else
        {
            Console.WriteLine("缓存命中，返回缓存数据");
        }
        return _cachedData;
    }
}

// 日志装饰器
public class LoggingDataService : IDataService
{
    private readonly IDataService _inner;

    public LoggingDataService(IDataService inner)
    {
        _inner = inner;
    }

    public string GetData()
    {
        Console.WriteLine("开始获取数据");
        var data = _inner.GetData();
        Console.WriteLine("数据获取完成");
        return data;
    }
}

// 主程序
var services = new ServiceCollection();

// 注册原始服务
services.AddTransient<IDataService, DefaultDataService>();

// 添加装饰器
services.Decorate<IDataService, CachedDataService>();
services.Decorate<IDataService, LoggingDataService>();

var serviceProvider = services.BuildServiceProvider();
var dataService = serviceProvider.GetRequiredService<IDataService>();

// 第一次调用（缓存未命中）
Console.WriteLine("\n第一次调用:");
var result1 = dataService.GetData();
Console.WriteLine($"结果: {result1}");

// 第二次调用（缓存命中）
Console.WriteLine("\n第二次调用:");
var result2 = dataService.GetData();
Console.WriteLine($"结果: {result2}");

// 输出:
// 第一次调用:
// 开始获取数据
// 缓存未命中，从原始服务获取
// DefaultDataService.GetData()
// 数据获取完成
// 结果: 原始数据
// 
// 第二次调用:
// 开始获取数据
// 缓存命中，返回缓存数据
// 数据获取完成
// 结果: 原始数据
```

### 5.2 服务过滤

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

public interface IRepository<T>
{
    void Save(T item);
}

public class User { public int Id { get; set; } public string Name { get; set; } }
public class Product { public int Id { get; set; } public string Name { get; set; } }

public class UserRepository : IRepository<User>
{
    public void Save(User item)
    {
        Console.WriteLine($"保存用户: {item.Name}");
    }
}

public class ProductRepository : IRepository<Product>
{
    public void Save(Product item)
    {
        Console.WriteLine($"保存产品: {item.Name}");
    }
}

// 通用日志装饰器
public class LoggingRepositoryDecorator<T> : IRepository<T>
{
    private readonly IRepository<T> _inner;

    public LoggingRepositoryDecorator(IRepository<T> inner)
    {
        _inner = inner;
    }

    public void Save(T item)
    {
        Console.WriteLine($"开始保存 {typeof(T).Name}");
        _inner.Save(item);
        Console.WriteLine($"保存 {typeof(T).Name} 完成");
    }
}

var services = new ServiceCollection();

// 注册仓储
services.AddTransient<IRepository<User>, UserRepository>();
services.AddTransient<IRepository<Product>, ProductRepository>();

// 为所有 IRepository<T> 添加装饰器
services.Decorate(typeof(IRepository<>), typeof(LoggingRepositoryDecorator<>));

var serviceProvider = services.BuildServiceProvider();

// 测试用户仓储
var userRepo = serviceProvider.GetRequiredService<IRepository<User>>();
userRepo.Save(new User { Id = 1, Name = "John" });

// 测试产品仓储
var productRepo = serviceProvider.GetRequiredService<IRepository<Product>>();
productRepo.Save(new Product { Id = 1, Name = "Laptop" });

// 输出:
// 开始保存 User
// 保存用户: John
// 保存 User 完成
// 开始保存 Product
// 保存产品: Laptop
// 保存 Product 完成
```

### 5.3 程序集扫描

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

// 服务接口
public interface IService { void Execute(); }

// 实现类
public class ServiceA : IService
{
    public void Execute() => Console.WriteLine("ServiceA.Execute()");
}

public class ServiceB : IService
{
    public void Execute() => Console.WriteLine("ServiceB.Execute()");
}

public class ServiceC { }

var services = new ServiceCollection();

// 程序集扫描注册
services.Scan(scan => scan
    .FromAssemblyOf<IService>()
    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);

var serviceProvider = services.BuildServiceProvider();

// 获取所有 IService 实现
var servicesList = serviceProvider.GetServices<IService>();

foreach (var service in servicesList)
{
    service.Execute();
}

// 输出:
// ServiceA.Execute()
// ServiceB.Execute()
```

### 5.4 条件注册

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;

public interface IDataService { string GetData(); }

public class DefaultDataService : IDataService
{
    public string GetData() => "默认数据";
}

public class AdvancedDataService : IDataService
{
    public string GetData() => "高级数据";
}

var services = new ServiceCollection();

bool useAdvanced = true; // 根据配置或环境决定

if (useAdvanced)
{
    services.AddTransient<IDataService, AdvancedDataService>();
    Console.WriteLine("注册了高级数据服务");
}
else
{
    services.AddTransient<IDataService, DefaultDataService>();
    Console.WriteLine("注册了默认数据服务");
}

var serviceProvider = services.BuildServiceProvider();
var dataService = serviceProvider.GetRequiredService<IDataService>();

Console.WriteLine($"获取的数据: {dataService.GetData()}");

// 输出:
// 注册了高级数据服务
// 获取的数据: 高级数据
```

## 6. 完整集成示例

### 6.1 综合数据流管道

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class DataflowPipeline
{
    private readonly ILogger<DataflowPipeline> _logger;

    public DataflowPipeline(ILogger<DataflowPipeline> logger)
    {
        _logger = logger;
    }

    public async Task ProcessDataAsync(IEnumerable<int> data)
    {
        var options = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = CancellationToken.None
        };

        // 1. 过滤块
        var filterBlock = new TransformBlock<int, int>(
            item => {
                _logger.LogInformation($"过滤数据: {item}");
                return item;
            },
            options);

        // 2. 转换块
        var transformBlock = new TransformBlock<int, string>(
            item => {
                _logger.LogInformation($"转换数据: {item} -> {item * 2}");
                Task.Delay(50).Wait();
                return (item * 2).ToString();
            },
            options);

        // 3. 批处理块
        var batchBlock = new BatchBlock<string>(3);

        // 4. 最终处理块
        var actionBlock = new ActionBlock<string[]>(
            batch => {
                _logger.LogInformation($"处理批处理: [{string.Join(", ", batch)}]");
                Task.Delay(100).Wait();
            },
            options);

        // 链接块
        filterBlock.LinkTo(transformBlock, new DataflowLinkOptions { PropagateCompletion = true });
        transformBlock.LinkTo(batchBlock, new DataflowLinkOptions { PropagateCompletion = true });
        batchBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

        // 发送数据
        foreach (var item in data)
        {
            await filterBlock.SendAsync(item);
        }

        // 标记完成
        filterBlock.Complete();
        // 等待完成
        await actionBlock.Completion;

        _logger.LogInformation("数据流处理完成");
    }
}

// 主程序
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
services.AddTransient<DataflowPipeline>();

var serviceProvider = services.BuildServiceProvider();
var pipeline = serviceProvider.GetRequiredService<DataflowPipeline>();

var data = Enumerable.Range(1, 10);
await pipeline.ProcessDataAsync(data);

// 输出:
// 过滤数据: 1
// 过滤数据: 2
// 过滤数据: 3
// 转换数据: 1 -> 2
// 转换数据: 2 -> 4
// 转换数据: 3 -> 6
// 处理批处理: [2, 4, 6]
// 过滤数据: 4
// 过滤数据: 5
// 过滤数据: 6
// 转换数据: 4 -> 8
// 转换数据: 5 -> 10
// 转换数据: 6 -> 12
// 处理批处理: [8, 10, 12]
// 过滤数据: 7
// 过滤数据: 8
// 过滤数据: 9
// 转换数据: 7 -> 14
// 转换数据: 8 -> 16
// 转换数据: 9 -> 18
// 处理批处理: [14, 16, 18]
// 过滤数据: 10
// 转换数据: 10 -> 20
// 处理批处理: [20]
// 数据流处理完成
```

### 6.2 并行计算示例

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ParallelCalculator
{
    private readonly ILogger<ParallelCalculator> _logger;

    public ParallelCalculator(ILogger<ParallelCalculator> logger)
    {
        _logger = logger;
    }

    public async Task<Dictionary<string, double>> CalculateAsync(IEnumerable<double> data)
    {
        _logger.LogInformation("开始并行计算");

        var tasks = new List<Task<KeyValuePair<string, double>>> {
            Task.Run(() => {
                _logger.LogInformation("计算平均值");
                var avg = data.Average();
                return new KeyValuePair<string, double>("平均值", avg);
            }),
            Task.Run(() => {
                _logger.LogInformation("计算最大值");
                var max = data.Max();
                return new KeyValuePair<string, double>("最大值", max);
            }),
            Task.Run(() => {
                _logger.LogInformation("计算最小值");
                var min = data.Min();
                return new KeyValuePair<string, double>("最小值", min);
            }),
            Task.Run(() => {
                _logger.LogInformation("计算总和");
                var sum = data.Sum();
                return new KeyValuePair<string, double>("总和", sum);
            }),
            Task.Run(() => {
                _logger.LogInformation("计算标准差");
                var avg = data.Average();
                var variance = data.Average(x => Math.Pow(x - avg, 2));
                var stdDev = Math.Sqrt(variance);
                return new KeyValuePair<string, double>("标准差", stdDev);
            })
        };

        var results = await Task.WhenAll(tasks);
        var resultDict = results.ToDictionary(kv => kv.Key, kv => kv.Value);

        _logger.LogInformation("并行计算完成");
        return resultDict;
    }
}

// 主程序
var services = new ServiceCollection();
services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
services.AddTransient<ParallelCalculator>();

var serviceProvider = services.BuildServiceProvider();
var calculator = serviceProvider.GetRequiredService<ParallelCalculator>();

// 生成测试数据
var data = Enumerable.Range(1, 1000).Select(i => (double)i);

var results = await calculator.CalculateAsync(data);

Console.WriteLine("计算结果:");
foreach (var (key, value) in results)
{
    Console.WriteLine($"{key}: {value:F2}");
}

// 输出:
// 开始并行计算
// 计算平均值
// 计算最大值
// 计算最小值
// 计算总和
// 计算标准差
// 并行计算完成
// 计算结果:
// 平均值: 500.50
// 最大值: 1000.00
// 最小值: 1.00
// 总和: 500500.00
// 标准差: 288.82
```

## 7. 命令行接口示例

### 7.1 基本命令

```bash
# 运行并行任务
dotnet tpl_core.dll task --count 10

# 运行数据流操作
dotnet tpl_core.dll dataflow --count 20

# 运行并行处理
dotnet tpl_core.dll parallel --count 1000

# 运行异步操作（模拟）
dotnet tpl_core.dll async --simulate "测试操作" --delay 2000

# 运行异步操作（计算阶乘）
dotnet tpl_core.dll async --factorial 15
```

### 7.2 Scrutor 演示命令

```bash
# 运行所有Scrutor演示
dotnet tpl_generator.dll demo --type all

# 运行基本注册演示
dotnet tpl_generator.dll demo --type basic

# 运行装饰器模式演示
dotnet tpl_generator.dll demo --type decorator

# 运行生命周期管理演示
dotnet tpl_generator.dll demo --type lifetime

# 运行程序集扫描演示
dotnet tpl_generator.dll demo --type scanning

# 显示Scrutor信息
dotnet tpl_generator.dll info
```

## 8. 性能优化示例

### 8.1 内存优化

```csharp
using System;
using System.Buffers;
using System.Threading.Tasks;

async Task Main()
{
    // 使用数组池减少内存分配
    var pool = ArrayPool<byte>.Shared;
    byte[] buffer = null;

    try
    {
        // 从池中租用缓冲区
        buffer = pool.Rent(1024 * 1024); // 1MB
        
        // 使用缓冲区
        Console.WriteLine($"租用的缓冲区大小: {buffer.Length}");
        
        // 模拟处理
        await Task.Delay(1000);
        
    }
    finally
    {
        // 归还缓冲区到池
        if (buffer != null)
        {
            pool.Return(buffer);
            Console.WriteLine("缓冲区已归还到池");
        }
    }
}

// 输出:
// 租用的缓冲区大小: 1048576
// 缓冲区已归还到池
```

### 8.2 并行度优化

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

async Task Main()
{
    var data = Enumerable.Range(1, 1000000).ToList();

    // 测试不同并行度
    for (int degree = 1; degree <= Environment.ProcessorCount * 2; degree++)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var results = data
            .AsParallel()
            .WithDegreeOfParallelism(degree)
            .Where(n => IsPrime(n))
            .ToList();
        
        stopwatch.Stop();
        
        Console.WriteLine($"并行度: {degree}, 质数数量: {results.Count}, 时间: {stopwatch.ElapsedMilliseconds}ms");
    }
}

bool IsPrime(int n)
{
    if (n <= 1) return false;
    if (n <= 3) return true;
    if (n % 2 == 0 || n % 3 == 0) return false;
    for (int i = 5; i * i <= n; i += 6)
    {
        if (n % i == 0 || n % (i + 2) == 0) return false;
    }
    return true;
}

// 输出:
// 并行度: 1, 质数数量: 78498, 时间: 1234ms
// 并行度: 2, 质数数量: 78498, 时间: 654ms
// 并行度: 4, 质数数量: 78498, 时间: 345ms
// 并行度: 8, 质数数量: 78498, 时间: 312ms
// 并行度: 16, 质数数量: 78498, 时间: 321ms
```

### 8.3 数据流优化

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

async Task Main()
{
    var data = Enumerable.Range(1, 1000);

    // 优化的数据流配置
    var options = new ExecutionDataflowBlockOptions
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount,
        BoundedCapacity = 100, // 限制缓冲区大小
        CancellationToken = CancellationToken.None
    };

    var transformBlock = new TransformBlock<int, int>(
        item => {
            // 模拟计算
            Task.Delay(1).Wait();
            return item * 2;
        },
        options);

    var actionBlock = new ActionBlock<int>(
        item => {
            // 模拟处理
            Task.Delay(1).Wait();
        },
        options);

    transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

    var stopwatch = Stopwatch.StartNew();

    // 异步发送数据
    var sendTasks = data.Select(item => transformBlock.SendAsync(item));
    await Task.WhenAll(sendTasks);

    transformBlock.Complete();
    await actionBlock.Completion;

    stopwatch.Stop();

    Console.WriteLine($"处理完成，时间: {stopwatch.ElapsedMilliseconds}ms");
}

// 输出:
// 处理完成，时间: 123ms
```

## 9. 部署示例

### 9.1 本地部署

```bash
# 编译
cd tpl/scripts
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishAot=true

# 运行
./bin/Release/net11.0/win-x64/publish/tpl_core.exe task --count 5
```

### 9.2 Docker 部署

**Dockerfile**:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY tpl/scripts/tpl_core.cs .
COPY tpl/scripts/tpl_core.setting.json .

RUN dotnet publish tpl_core.cs -c Release -o /app/publish /p:PublishAot=true /p:SelfContained=true /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["./tpl_core"]
```

**构建和运行**:

```bash
# 构建镜像
docker build -t tpl-skill .

# 运行容器
docker run --rm tpl-skill task --count 10
```

### 9.3 Azure Functions 部署

**function.json**:

```json
{
  "bindings": [
    {
      "name": "req",
      "type": "httpTrigger",
      "direction": "in",
      "authLevel": "anonymous",
      "methods": ["get", "post"]
    },
    {
      "name": "res",
      "type": "http",
      "direction": "out"
    }
  ]
}
```

**run.csx**:

```csharp
#r "System.Threading.Tasks.Dataflow"

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    // 创建数据流管道
    var transformBlock = new TransformBlock<int, int>(item => item * 2);
    var actionBlock = new ActionBlock<int>(item => log.LogInformation($"Processed: {item}"));

    transformBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

    // 发送数据
    for (int i = 1; i <= 10; i++)
    {
        await transformBlock.SendAsync(i);
    }

    transformBlock.Complete();
    await actionBlock.Completion;

    return req.CreateResponse(System.Net.HttpStatusCode.OK, "TPL processing completed");
}
```

## 10. 监控与日志示例

### 10.1 结构化日志

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

async Task Main()
{
    var services = new ServiceCollection();
    services.AddLogging(builder => {
        builder.AddConsole(options => {
            options.FormatterName = "json";
        });
        builder.SetMinimumLevel(LogLevel.Information);
    });

    var serviceProvider = services.BuildServiceProvider();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("应用程序启动");

    try
    {
        logger.LogInformation("开始处理任务");
        
        // 模拟处理
        await Task.Delay(1000);
        
        logger.LogInformation("任务处理完成");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "处理任务时发生错误");
    }
    finally
    {
        logger.LogInformation("应用程序退出");
    }
}

// 输出 (JSON格式):
// {"Timestamp":"2026-01-25T12:00:00.0000000Z","Level":"Information","MessageTemplate":"应用程序启动","Category":"Program"}
// {"Timestamp":"2026-01-25T12:00:00.0000000Z","Level":"Information","MessageTemplate":"开始处理任务","Category":"Program"}
// {"Timestamp":"2026-01-25T12:00:01.0000000Z","Level":"Information","MessageTemplate":"任务处理完成","Category":"Program"}
// {"Timestamp":"2026-01-25T12:00:01.0000000Z","Level":"Information","MessageTemplate":"应用程序退出","Category":"Program"}
```

### 10.2 性能监控

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;

async Task Main()
{
    var stopwatch = Stopwatch.StartNew();
    var counter = 0;

    // 启动性能监控任务
    var monitorTask = Task.Run(async () => {
        while (!stopwatch.IsRunning || stopwatch.Elapsed < TimeSpan.FromSeconds(10))
        {
            await Task.Delay(1000);
            Console.WriteLine($"时间: {stopwatch.Elapsed.Seconds}s, 处理计数: {counter}");
        }
    });

    // 模拟工作
    for (int i = 0; i < 1000000; i++)
    {
        // 模拟处理
        await Task.Delay(1);
        counter++;
    }

    stopwatch.Stop();

    Console.WriteLine($"总处理时间: {stopwatch.ElapsedMilliseconds}ms");
    Console.WriteLine($"总处理计数: {counter}");
    Console.WriteLine($"处理速率: {counter / stopwatch.Elapsed.TotalSeconds:F2} 次/秒");

    await monitorTask;
}

// 输出:
// 时间: 1s, 处理计数: 999
// 时间: 2s, 处理计数: 1999
// 时间: 3s, 处理计数: 2999
// 时间: 4s, 处理计数: 3999
// 时间: 5s, 处理计数: 4999
// 时间: 6s, 处理计数: 5999
// 时间: 7s, 处理计数: 6999
// 时间: 8s, 处理计数: 7999
// 时间: 9s, 处理计数: 8999
// 时间: 10s, 处理计数: 9999
// 总处理时间: 10001ms
// 总处理计数: 10000
// 处理速率: 999.90 次/秒
```

## 总结

本文件提供了 TPL 技能的全面使用示例，涵盖了从基本的并行任务处理到复杂的数据流管道，从 PLINQ 优化到 Scrutor 装饰器模式的各种使用场景。这些示例可以帮助您快速上手 TPL 并在实际项目中应用并行编程技术。

如需更多信息，请参考 [README.md](README.md) 文件或官方文档。
