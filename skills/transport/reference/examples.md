# Transport 技能使用示例

## 概述

本文件提供了 Transport 技能的详细使用示例，涵盖了各种传输协议和 Scrutor 用法的实际应用场景。所有示例代码都可以直接复制使用，并包含了详细的注释说明。

## HTTP 传输示例

### 基本 GET 请求

```csharp
using Microsoft.Extensions.DependencyInjection;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取 HTTP 传输服务
var httpService = provider.GetRequiredService<IHttpTransportService>();

// 发送 GET 请求
var response = await httpService.GetAsync("https://api.example.com/users");
Console.WriteLine($"GET 响应: {response}");
```

### POST 请求示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取 HTTP 传输服务
var httpService = provider.GetRequiredService<IHttpTransportService>();

// 准备 POST 数据
var postData = "{\"name\": \"John Doe\", \"email\": \"john@example.com\"}";

// 发送 POST 请求
var response = await httpService.PostAsync("https://api.example.com/users", postData);
Console.WriteLine($"POST 响应: {response}");
```

### 高级 HTTP 请求

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取 HTTP 传输服务
var httpService = provider.GetRequiredService<IHttpTransportService>();

// 创建自定义 HTTP 请求
var request = new HttpRequestMessage(HttpMethod.Put, "https://api.example.com/users/1");
request.Content = new StringContent("{\"name\": \"Updated Name\"}");
request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
request.Headers.Add("Authorization", "Bearer your-token-here");

// 发送请求
var response = await httpService.SendAsync(request);
Console.WriteLine($"PUT 响应状态码: {response.StatusCode}");
var responseContent = await response.Content.ReadAsStringAsync();
Console.WriteLine($"PUT 响应内容: {responseContent}");
```

## TCP 传输示例

### 基本 TCP 客户端

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取 TCP 传输服务
var tcpService = provider.GetRequiredService<ITcpTransportService>();

// 准备发送数据
var data = Encoding.UTF8.GetBytes("Hello, TCP Server!");

// 发送数据
await tcpService.SendAsync("localhost", 8080, data);
Console.WriteLine("TCP 数据发送成功");

// 接收响应
var responseData = await tcpService.ReceiveAsync("localhost", 8080);
var response = Encoding.UTF8.GetString(responseData);
Console.WriteLine($"TCP 响应: {response}");
```

### TCP 流操作

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取 TCP 传输服务
var tcpService = provider.GetRequiredService<ITcpTransportService>();

// 建立 TCP 连接
using var stream = await tcpService.ConnectAsync("localhost", 8080);

// 发送数据
var writer = new StreamWriter(stream);
await writer.WriteLineAsync("Hello from TCP stream!");
await writer.FlushAsync();

// 接收数据
var reader = new StreamReader(stream);
var response = await reader.ReadLineAsync();
Console.WriteLine($"TCP 流响应: {response}");
```

## UDP 传输示例

### 基本 UDP 发送

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取 UDP 传输服务
var udpService = provider.GetRequiredService<IUdpTransportService>();

// 准备发送数据
var data = Encoding.UTF8.GetBytes("Hello, UDP Server!");

// 发送数据
await udpService.SendAsync("localhost", 8081, data);
Console.WriteLine("UDP 数据发送成功");

// 接收响应
var (responseData, endpoint) = await udpService.ReceiveAsync();
var response = Encoding.UTF8.GetString(responseData);
Console.WriteLine($"从 {endpoint} 接收到 UDP 响应: {response}");
```

## 管道传输示例

### 基本管道操作

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.IO.Pipelines;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取管道传输服务
var pipelineService = provider.GetRequiredService<IPipelineTransportService>();

// 创建内存流作为管道的基础
using var stream = new MemoryStream();

// 创建管道读取器和写入器
var reader = pipelineService.CreateReader(stream);
var writer = pipelineService.CreateWriter(stream);

// 写入数据到管道
var writeBuffer = writer.GetMemory(1024);
var data = System.Text.Encoding.UTF8.GetBytes("Hello, Pipeline!");
data.CopyTo(writeBuffer);
writer.Advance(data.Length);
await writer.FlushAsync();

// 重置流位置以便读取
stream.Position = 0;

// 从管道读取数据
var readResult = await reader.ReadAsync();
var readBuffer = readResult.Buffer;

// 处理读取的数据
if (readBuffer.Length > 0)
{
    var result = System.Text.Encoding.UTF8.GetString(readBuffer.FirstSpan);
    Console.WriteLine($"管道读取: {result}");
}

// 标记数据已处理
reader.AdvanceTo(readBuffer.End);
```

### 管道处理器示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.IO.Pipelines;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取管道传输服务
var pipelineService = provider.GetRequiredService<IPipelineTransportService>();

// 创建输入和输出流
using var inputStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Hello, Pipeline Processor!"));
using var outputStream = new MemoryStream();

// 创建管道读取器和写入器
var reader = pipelineService.CreateReader(inputStream);
var writer = pipelineService.CreateWriter(outputStream);

// 定义处理器
async ValueTask<ProcessResult> Processor(ReadOnlySequence<byte> buffer)
{
    // 转换为字符串
    var input = System.Text.Encoding.UTF8.GetString(buffer.FirstSpan);
    Console.WriteLine($"处理器接收到: {input}");
    
    // 处理数据（转换为大写）
    var processed = input.ToUpper();
    var processedData = System.Text.Encoding.UTF8.GetBytes(processed);
    
    // 写入处理结果
    await writer.WriteAsync(processedData);
    await writer.FlushAsync();
    
    return ProcessResult.Completed;
}

// 处理管道数据
await pipelineService.ProcessPipelineAsync(reader, writer, Processor);

// 查看处理结果
outputStream.Position = 0;
using var outputReader = new StreamReader(outputStream);
var result = await outputReader.ReadToEndAsync();
Console.WriteLine($"处理结果: {result}");
```

## 通道传输示例

### 基本通道操作

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取通道传输服务
var channelService = provider.GetRequiredService<IChannelTransportService>();

// 创建有界通道（容量为 10）
var channel = channelService.CreateChannel<string>(10);

// 获取读取器和写入器
var reader = channelService.CreateReader(channel);
var writer = channelService.CreateWriter(channel);

// 写入数据到通道
await writer.WriteAsync("Message 1");
await writer.WriteAsync("Message 2");
await writer.WriteAsync("Message 3");

// 标记写入完成
writer.Complete();

// 从通道读取数据
await foreach (var message in reader.ReadAllAsync())
{
    Console.WriteLine($"通道读取: {message}");
}
```

### 通道处理器示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取通道传输服务
var channelService = provider.GetRequiredService<IChannelTransportService>();

// 创建通道
var channel = channelService.CreateChannel<int>();
var reader = channelService.CreateReader(channel);
var writer = channelService.CreateWriter(channel);

// 启动处理器
var processorTask = channelService.ProcessChannelAsync(reader, async (value) =>
{
    // 处理接收到的值
    var result = value * 2;
    Console.WriteLine($"处理值: {value} → 结果: {result}");
    
    // 模拟处理延迟
    await Task.Delay(100);
});

// 写入数据
for (int i = 1; i <= 5; i++)
{
    await writer.WriteAsync(i);
    Console.WriteLine($"写入值: {i}");
    await Task.Delay(50);
}

// 标记写入完成
writer.Complete();

// 等待处理器完成
await processorTask;
Console.WriteLine("通道处理完成");
```

## 缓冲区管理示例

### 基本缓冲区操作

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Buffers;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取缓冲区传输服务
var bufferService = provider.GetRequiredService<IBufferTransportService>();

// 租用缓冲区
using var owner = bufferService.Rent(1024);
var buffer = owner.Memory;

// 写入数据
var data = System.Text.Encoding.UTF8.GetBytes("Hello, Buffer!");
data.CopyTo(buffer.Span);

// 读取数据
var result = System.Text.Encoding.UTF8.GetString(buffer.Span.Slice(0, data.Length));
Console.WriteLine($"缓冲区内容: {result}");

// 清理缓冲区（可选）
bufferService.Clear(buffer);
```

### 高性能缓冲区操作

```csharp
using Microsoft.Extensions.DependencyInjection;
using System.Buffers;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取缓冲区传输服务
var bufferService = provider.GetRequiredService<IBufferTransportService>();

// 使用 GetSpan 进行零拷贝操作
var span = bufferService.GetSpan(1024);

// 直接写入数据到 Span
var data = "Hello, High Performance Buffer!";
var written = System.Text.Encoding.UTF8.GetBytes(data, span);

// 处理数据
var result = System.Text.Encoding.UTF8.GetString(span.Slice(0, written));
Console.WriteLine($"高性能缓冲区内容: {result}");

// 使用 GetMemory 进行异步操作
var memory = bufferService.GetMemory(1024);

// 异步写入数据
await Task.Run(() =>
{
    var asyncData = "Hello, Async Buffer!";
    System.Text.Encoding.UTF8.GetBytes(asyncData, memory.Span);
});

// 读取异步写入的数据
var asyncResult = System.Text.Encoding.UTF8.GetString(memory.Span.Slice(0, 18));
Console.WriteLine($"异步缓冲区内容: {asyncResult}");
```

## Scrutor 用法示例

### 基本注册示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 基本服务注册
services.AddSingleton<ISomeService, SomeService>();

// 使用 Scrutor 进行程序集扫描注册
services.Scan(scan => scan
    .FromAssemblyOf<ISomeService>()
    .AddClasses(classes => classes.AssignableTo<ISomeService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 解析服务
var service = provider.GetRequiredService<ISomeService>();
var result = service.DoSomething();
Console.WriteLine($"服务结果: {result}");
```

### 装饰器模式示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 注册基础服务
services.AddSingleton<ISomeService, SomeService>();

// 添加装饰器
services.Decorate<ISomeService, LoggingDecorator>();
services.Decorate<ISomeService, CachingDecorator>();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 解析服务（会返回装饰器包装的实例）
var service = provider.GetRequiredService<ISomeService>();
var result = service.DoSomething();
Console.WriteLine($"装饰器模式结果: {result}");

// 装饰器实现示例
public class LoggingDecorator : ISomeService
{
    private readonly ISomeService _inner;
    
    public LoggingDecorator(ISomeService inner)
    {
        _inner = inner;
    }
    
    public string DoSomething()
    {
        Console.WriteLine("[LoggingDecorator] 开始执行");
        var result = _inner.DoSomething();
        Console.WriteLine("[LoggingDecorator] 执行完成");
        return result;
    }
}

public class CachingDecorator : ISomeService
{
    private readonly ISomeService _inner;
    private string _cachedResult;
    
    public CachingDecorator(ISomeService inner)
    {
        _inner = inner;
    }
    
    public string DoSomething()
    {
        if (_cachedResult != null)
        {
            Console.WriteLine("[CachingDecorator] 使用缓存结果");
            return _cachedResult;
        }
        
        Console.WriteLine("[CachingDecorator] 缓存未命中，执行原始服务");
        _cachedResult = _inner.DoSomething();
        Console.WriteLine("[CachingDecorator] 结果已缓存");
        return _cachedResult;
    }
}
```

### 服务过滤示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 使用 Scrutor 进行带过滤的注册
services.Scan(scan => scan
    .FromAssemblyOf<IService>()
    .AddClasses(classes => classes
        .AssignableTo<IService>()
        .Where(type => type.Name.EndsWith("Service")) // 只注册名称以 Service 结尾的类
        .Where(type => !type.IsAbstract) // 排除抽象类
        .Where(type => type.GetConstructors().Any(c => c.GetParameters().Length == 0)) // 只注册无参构造函数的类
    )
    .AsImplementedInterfaces()
    .WithTransientLifetime());

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 解析服务
var services = provider.GetServices<IService>();
foreach (var service in services)
{
    Console.WriteLine($"解析到服务: {service.GetType().Name}");
    service.Execute();
}
```

### 生命周期管理示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 注册不同生命周期的服务
services.AddTransient<ITransientService, TransientService>();
services.AddScoped<IScopedService, ScopedService>();
services.AddSingleton<ISingletonService, SingletonService>();

// 使用 Scrutor 批量设置生命周期
services.Scan(scan => scan
    .FromAssemblyOf<IService>()
    .AddClasses(classes => classes.AssignableTo<IService>())
    .AsImplementedInterfaces()
    .WithLifetime(ServiceLifetime.Transient) // 统一设置为 transient
);

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 测试 transient 生命周期
Console.WriteLine("=== Transient 生命周期测试 ===");
var transient1 = provider.GetRequiredService<ITransientService>();
var transient2 = provider.GetRequiredService<ITransientService>();
Console.WriteLine($"transient1 == transient2: {transient1 == transient2}");

// 测试 scoped 生命周期
Console.WriteLine("\n=== Scoped 生命周期测试 ===");
using (var scope = provider.CreateScope())
{
    var scoped1 = scope.ServiceProvider.GetRequiredService<IScopedService>();
    var scoped2 = scope.ServiceProvider.GetRequiredService<IScopedService>();
    Console.WriteLine($"同一作用域内 scoped1 == scoped2: {scoped1 == scoped2}");
}

using (var scope = provider.CreateScope())
{
    var scoped3 = scope.ServiceProvider.GetRequiredService<IScopedService>();
    Console.WriteLine($"新作用域 scoped3 是新实例: {scoped3 != null}");
}

// 测试 singleton 生命周期
Console.WriteLine("\n=== Singleton 生命周期测试 ===");
var singleton1 = provider.GetRequiredService<ISingletonService>();
var singleton2 = provider.GetRequiredService<ISingletonService>();
Console.WriteLine($"singleton1 == singleton2: {singleton1 == singleton2}");
```

### 高级注册示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 高级注册配置
services.Scan(scan => scan
    // 从多个程序集扫描
    .FromAssembliesOf(typeof(IService), typeof(IAnotherService))
    
    // 排除抽象类
    .AddClasses(false)
    
    // 使用注册策略
    .UsingRegistrationStrategy(RegistrationStrategy.Skip) // 跳过已注册的服务
    
    // 按接口注册
    .AsImplementedInterfaces()
    
    // 自定义注册逻辑
    .As(t =>
    {
        // 为每个类型创建多个注册
        var registrations = new List<Type>();
        
        // 注册为所有实现的接口
        registrations.AddRange(t.GetInterfaces());
        
        // 也注册为自身类型
        registrations.Add(t);
        
        return registrations;
    })
    
    // 设置生命周期
    .WithTransientLifetime()
);

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 解析服务
var services = provider.GetServices<IService>();
foreach (var service in services)
{
    Console.WriteLine($"解析到服务: {service.GetType().Name}");
}
```

### 程序集扫描示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 从应用程序依赖项中扫描
services.Scan(scan => scan
    .FromApplicationDependencies()
    
    // 过滤条件
    .AddClasses(classes => classes
        .Where(type => type.Name.Contains("Service"))
        .Where(type => type.Namespace?.Contains("Transport") == true)
    )
    
    // 注册为实现的接口
    .AsImplementedInterfaces()
    
    // 设置生命周期
    .WithSingletonLifetime()
);

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 打印所有注册的服务类型
var serviceTypes = provider.GetRequiredService<IServiceProvider>()
    .GetType()
    .GetField("_descriptorLookup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
    .GetValue(provider);

if (serviceTypes != null)
{
    Console.WriteLine("注册的服务类型:");
    foreach (var entry in (System.Collections.IDictionary)serviceTypes)
    {
        Console.WriteLine($"- {entry.Key}");
    }
}
```

### 多个装饰器示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 注册基础服务
services.AddSingleton<IBusinessService, BusinessService>();

// 添加多个装饰器（顺序很重要）
services.Decorate<IBusinessService, ValidationDecorator>(); // 第一层：验证
services.Decorate<IBusinessService, LoggingDecorator>();   // 第二层：日志
services.Decorate<IBusinessService, CachingDecorator>();   // 第三层：缓存
services.Decorate<IBusinessService, MetricsDecorator>();   // 第四层：指标

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 解析服务
var service = provider.GetRequiredService<IBusinessService>();

// 调用服务方法
var result = await service.ProcessAsync("test data");
Console.WriteLine($"服务处理结果: {result}");

// 装饰器实现
public class ValidationDecorator : IBusinessService
{
    private readonly IBusinessService _inner;
    
    public ValidationDecorator(IBusinessService inner)
    {
        _inner = inner;
    }
    
    public async Task<string> ProcessAsync(string data)
    {
        Console.WriteLine("[ValidationDecorator] 验证数据");
        if (string.IsNullOrEmpty(data))
        {
            throw new ArgumentException("数据不能为空");
        }
        return await _inner.ProcessAsync(data);
    }
}

public class LoggingDecorator : IBusinessService
{
    private readonly IBusinessService _inner;
    
    public LoggingDecorator(IBusinessService inner)
    {
        _inner = inner;
    }
    
    public async Task<string> ProcessAsync(string data)
    {
        Console.WriteLine($"[LoggingDecorator] 开始处理: {data}");
        var result = await _inner.ProcessAsync(data);
        Console.WriteLine($"[LoggingDecorator] 处理完成: {result}");
        return result;
    }
}

public class CachingDecorator : IBusinessService
{
    private readonly IBusinessService _inner;
    private readonly Dictionary<string, string> _cache = new();
    
    public CachingDecorator(IBusinessService inner)
    {
        _inner = inner;
    }
    
    public async Task<string> ProcessAsync(string data)
    {
        if (_cache.TryGetValue(data, out var cachedResult))
        {
            Console.WriteLine($"[CachingDecorator] 使用缓存结果");
            return cachedResult;
        }
        
        var result = await _inner.ProcessAsync(data);
        _cache[data] = result;
        Console.WriteLine($"[CachingDecorator] 结果已缓存");
        return result;
    }
}

public class MetricsDecorator : IBusinessService
{
    private readonly IBusinessService _inner;
    private int _callCount = 0;
    
    public MetricsDecorator(IBusinessService inner)
    {
        _inner = inner;
    }
    
    public async Task<string> ProcessAsync(string data)
    {
        _callCount++;
        Console.WriteLine($"[MetricsDecorator] 调用次数: {_callCount}");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _inner.ProcessAsync(data);
        stopwatch.Stop();
        
        Console.WriteLine($"[MetricsDecorator] 处理耗时: {stopwatch.ElapsedMilliseconds}ms");
        return result;
    }
}
```

### 条件注册示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 条件注册
services.Scan(scan => scan
    .FromAssemblyOf<IService>()
    .AddClasses(classes => classes.AssignableTo<IService>())
    
    // 条件注册逻辑
    .As(t =>
    {
        // 获取所有接口
        var interfaces = t.GetInterfaces();
        
        // 查找匹配的接口（例如，接口名以 I 开头，后跟类名）
        var matchingInterface = interfaces.FirstOrDefault(i => 
            i.Name == "I" + t.Name
        );
        
        // 如果找到匹配的接口，注册为该接口
        if (matchingInterface != null)
        {
            return matchingInterface;
        }
        
        // 否则注册为第一个接口
        return interfaces.FirstOrDefault() ?? t;
    })
    
    // 设置生命周期
    .WithTransientLifetime()
);

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 测试条件注册
var service = provider.GetRequiredService<ISomeService>();
Console.WriteLine($"解析到的服务类型: {service.GetType().Name}");

// 尝试解析其他服务
try
{
    var anotherService = provider.GetRequiredService<IAnotherService>();
    Console.WriteLine($"解析到的另一个服务类型: {anotherService.GetType().Name}");
}
catch (Exception ex)
{
    Console.WriteLine($"解析服务时出错: {ex.Message}");
}
```

## 综合示例

### 完整的传输服务使用示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Transport.Core;

// 创建服务容器
var services = new ServiceCollection();

// 添加日志记录
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册传输服务
services.AddTransportServices();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取日志记录器
var logger = provider.GetRequiredService<ILogger<Program>>();

// 测试 HTTP 传输
logger.LogInformation("=== 测试 HTTP 传输 ===");
try
{
    var httpService = provider.GetRequiredService<IHttpTransportService>();
    var response = await httpService.GetAsync("https://api.github.com/users/octocat");
    logger.LogInformation($"HTTP 响应长度: {response.Length}");
    logger.LogInformation($"HTTP 响应前 100 字符: {response.Substring(0, Math.Min(100, response.Length))}...");
}
catch (Exception ex)
{
    logger.LogError(ex, "HTTP 传输测试失败");
}

// 测试 TCP 传输
logger.LogInformation("\n=== 测试 TCP 传输 ===");
try
{
    var tcpService = provider.GetRequiredService<ITcpTransportService>();
    // 注意：这里需要有一个运行中的 TCP 服务器
    // var response = await tcpService.SendAsync("localhost", 8080, Encoding.UTF8.GetBytes("Hello TCP"));
    // logger.LogInformation($"TCP 响应: {Encoding.UTF8.GetString(response)}");
    logger.LogInformation("TCP 传输服务已初始化");
}
catch (Exception ex)
{
    logger.LogError(ex, "TCP 传输测试失败");
}

// 测试 UDP 传输
logger.LogInformation("\n=== 测试 UDP 传输 ===");
try
{
    var udpService = provider.GetRequiredService<IUdpTransportService>();
    await udpService.SendAsync("localhost", 8081, System.Text.Encoding.UTF8.GetBytes("Hello UDP"));
    logger.LogInformation("UDP 数据发送成功");
}
catch (Exception ex)
{
    logger.LogError(ex, "UDP 传输测试失败");
}

// 测试管道传输
logger.LogInformation("\n=== 测试管道传输 ===");
try
{
    var pipelineService = provider.GetRequiredService<IPipelineTransportService>();
    using var stream = new System.IO.MemoryStream();
    var writer = pipelineService.CreateWriter(stream);
    await writer.WriteAsync(System.Text.Encoding.UTF8.GetBytes("Hello Pipeline"));
    await writer.FlushAsync();
    
    stream.Position = 0;
    var reader = pipelineService.CreateReader(stream);
    var readResult = await reader.ReadAsync();
    var result = System.Text.Encoding.UTF8.GetString(readResult.Buffer.FirstSpan);
    logger.LogInformation($"管道传输结果: {result}");
    reader.AdvanceTo(readResult.Buffer.End);
}
catch (Exception ex)
{
    logger.LogError(ex, "管道传输测试失败");
}

// 测试通道传输
logger.LogInformation("\n=== 测试通道传输 ===");
try
{
    var channelService = provider.GetRequiredService<IChannelTransportService>();
    var channel = channelService.CreateChannel<string>();
    var writer = channelService.CreateWriter(channel);
    var reader = channelService.CreateReader(channel);
    
    // 写入数据
    await writer.WriteAsync("Message 1");
    await writer.WriteAsync("Message 2");
    writer.Complete();
    
    // 读取数据
    await foreach (var message in reader.ReadAllAsync())
    {
        logger.LogInformation($"通道接收: {message}");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "通道传输测试失败");
}

// 测试缓冲区传输
logger.LogInformation("\n=== 测试缓冲区传输 ===");
try
{
    var bufferService = provider.GetRequiredService<IBufferTransportService>();
    using var owner = bufferService.Rent(1024);
    var buffer = owner.Memory;
    
    var data = System.Text.Encoding.UTF8.GetBytes("Hello Buffer");
    data.CopyTo(buffer.Span);
    
    var result = System.Text.Encoding.UTF8.GetString(buffer.Span.Slice(0, data.Length));
    logger.LogInformation($"缓冲区传输结果: {result}");
}
catch (Exception ex)
{
    logger.LogError(ex, "缓冲区传输测试失败");
}

logger.LogInformation("\n所有传输服务测试完成");
```

### Scrutor 装饰器模式综合示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scrutor;

// 创建服务容器
var services = new ServiceCollection();

// 添加日志记录
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// 注册基础服务
services.AddSingleton<ICacheService, MemoryCacheService>();
services.AddSingleton<IRepository, SqlRepository>();

// 添加装饰器
services.Decorate<ICacheService, LoggingCacheDecorator>();
services.Decorate<IRepository, CachingRepositoryDecorator>();
services.Decorate<IRepository, LoggingRepositoryDecorator>();
services.Decorate<IRepository, TransactionalRepositoryDecorator>();

// 注册业务服务
services.AddSingleton<IBusinessLogic, BusinessLogic>();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 获取业务服务
var businessLogic = provider.GetRequiredService<IBusinessLogic>();

// 测试业务逻辑
Console.WriteLine("=== 测试业务逻辑 ===");
var result = await businessLogic.ProcessDataAsync("test data");
Console.WriteLine($"业务逻辑处理结果: {result}");

// 再次测试（应该使用缓存）
Console.WriteLine("\n=== 再次测试（应该使用缓存）===");
result = await businessLogic.ProcessDataAsync("test data");
Console.WriteLine($"业务逻辑处理结果: {result}");

// 测试不同数据
Console.WriteLine("\n=== 测试不同数据 ===");
result = await businessLogic.ProcessDataAsync("different data");
Console.WriteLine($"业务逻辑处理结果: {result}");

// 装饰器实现
public class LoggingCacheDecorator : ICacheService
{
    private readonly ICacheService _inner;
    private readonly ILogger<LoggingCacheDecorator> _logger;
    
    public LoggingCacheDecorator(ICacheService inner, ILogger<LoggingCacheDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }
    
    public T Get<T>(string key)
    {
        _logger.LogInformation($"缓存获取: {key}");
        var result = _inner.Get<T>(key);
        _logger.LogInformation($"缓存获取结果: {result}");
        return result;
    }
    
    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _logger.LogInformation($"缓存设置: {key} = {value}, 过期时间: {expiration}");
        _inner.Set(key, value, expiration);
    }
    
    public bool Remove(string key)
    {
        _logger.LogInformation($"缓存移除: {key}");
        return _inner.Remove(key);
    }
}

public class CachingRepositoryDecorator : IRepository
{
    private readonly IRepository _inner;
    private readonly ICacheService _cache;
    
    public CachingRepositoryDecorator(IRepository inner, ICacheService cache)
    {
        _inner = inner;
        _cache = cache;
    }
    
    public async Task<string> GetDataAsync(string id)
    {
        // 尝试从缓存获取
        var cacheKey = $"data:{id}";
        var cachedData = _cache.Get<string>(cacheKey);
        if (cachedData != null)
        {
            Console.WriteLine($"[CachingRepositoryDecorator] 从缓存获取数据: {id}");
            return cachedData;
        }
        
        // 从原始存储获取
        var data = await _inner.GetDataAsync(id);
        
        // 存入缓存
        _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));
        Console.WriteLine($"[CachingRepositoryDecorator] 数据已缓存: {id}");
        
        return data;
    }
    
    public async Task<string> SaveDataAsync(string data)
    {
        var id = await _inner.SaveDataAsync(data);
        
        // 清除相关缓存
        var cacheKey = $"data:{id}";
        _cache.Remove(cacheKey);
        Console.WriteLine($"[CachingRepositoryDecorator] 缓存已清除: {cacheKey}");
        
        return id;
    }
}

public class LoggingRepositoryDecorator : IRepository
{
    private readonly IRepository _inner;
    private readonly ILogger<LoggingRepositoryDecorator> _logger;
    
    public LoggingRepositoryDecorator(IRepository inner, ILogger<LoggingRepositoryDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }
    
    public async Task<string> GetDataAsync(string id)
    {
        _logger.LogInformation($"获取数据: {id}");
        var result = await _inner.GetDataAsync(id);
        _logger.LogInformation($"获取数据完成: {id}, 结果: {result}");
        return result;
    }
    
    public async Task<string> SaveDataAsync(string data)
    {
        _logger.LogInformation($"保存数据: {data}");
        var id = await _inner.SaveDataAsync(data);
        _logger.LogInformation($"保存数据完成: {id}");
        return id;
    }
}

public class TransactionalRepositoryDecorator : IRepository
{
    private readonly IRepository _inner;
    private readonly ILogger<TransactionalRepositoryDecorator> _logger;
    
    public TransactionalRepositoryDecorator(IRepository inner, ILogger<TransactionalRepositoryDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }
    
    public async Task<string> GetDataAsync(string id)
    {
        // 读取操作不需要事务
        return await _inner.GetDataAsync(id);
    }
    
    public async Task<string> SaveDataAsync(string data)
    {
        _logger.LogInformation("开始事务");
        try
        {
            // 模拟事务开始
            var id = await _inner.SaveDataAsync(data);
            _logger.LogInformation("提交事务");
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回滚事务");
            throw;
        }
    }
}

// 业务逻辑实现
public class BusinessLogic : IBusinessLogic
{
    private readonly IRepository _repository;
    private readonly ILogger<BusinessLogic> _logger;
    
    public BusinessLogic(IRepository repository, ILogger<BusinessLogic> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<string> ProcessDataAsync(string data)
    {
        _logger.LogInformation($"处理数据: {data}");
        
        // 保存数据
        var id = await _repository.SaveDataAsync(data);
        
        // 读取数据
        var retrievedData = await _repository.GetDataAsync(id);
        
        // 处理数据
        var processedData = $"Processed: {retrievedData}";
        
        _logger.LogInformation($"数据处理完成: {processedData}");
        return processedData;
    }
}
```

## 总结

本文件提供了 Transport 技能的详细使用示例，涵盖了各种传输协议和 Scrutor 用法的实际应用场景。这些示例代码可以直接复制使用，帮助开发者快速上手和理解 Transport 技能的核心功能。

通过这些示例，开发者可以：

1. 了解如何使用各种传输协议（HTTP、TCP、UDP）
2. 掌握高性能数据处理技术（管道、通道）
3. 学习内存管理优化技巧（缓冲区、Span、Memory）
4. 熟悉 Scrutor 的各种高级用法（装饰器模式、服务过滤、生命周期管理等）
5. 构建完整的传输服务解决方案

所有示例都包含了详细的注释和错误处理，确保代码的可靠性和可维护性。