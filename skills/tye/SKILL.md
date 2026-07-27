# Transport 技能文档

## 技能概述

Transport 技能是一个基于 .NET 10 的传输层实现，提供了多种高性能传输协议和数据处理技术。本技能旨在简化网络通信和数据传输的开发过程，同时提供高性能、可扩展的解决方案。

### 主要功能

- **多种传输协议支持**：HTTP、TCP、UDP
- **高性能数据处理**：System.IO.Pipelines、System.Threading.Channels
- **内存管理优化**：System.Buffers、Span<T>、Memory<T>
- **依赖注入增强**：Scrutor 装饰器模式实现
- **AOT 编译支持**：原生编译优化
- **命令行接口**：System.CommandLine
- **日志记录**：Microsoft.Extensions.Logging

### 应用场景

- **微服务通信**：高效的服务间通信
- **实时数据传输**：低延迟数据传输
- **高性能 API**：HTTP API 服务
- **数据流处理**：大量数据的流式处理
- **异步通信**：基于通道的异步消息传递

## 技术栈

### 核心技术

| 技术 | 版本 | 用途 |
|------|------|------|
| .NET | 10.0 | 运行时框架 |
| C# | 12.0 | 开发语言 |
| System.Net.Http | 9.0.0 | HTTP 客户端 |
| System.Net.Sockets | 9.0.0 | TCP/UDP 通信 |
| System.IO.Pipelines | 9.0.0 | 高性能数据处理 |
| System.Threading.Channels | 9.0.0 | 异步消息传递 |
| System.Buffers | 9.0.0 | 内存缓冲区管理 |
| Microsoft.Extensions.DependencyInjection | 9.0.0 | 依赖注入 |
| Scrutor | 4.2.2 | 装饰器模式实现 |
| System.CommandLine | 2.0.0-beta4.22272.1 | 命令行接口 |
| Microsoft.Extensions.Logging | 9.0.0 | 日志记录 |

### 编译配置

| 配置项 | 值 | 说明 |
|--------|-----|------|
| TargetFramework | net11.0 | 目标框架 |
| PublishAot | true | AOT 编译 |
| TrimMode | partial | 裁剪模式 |
| SelfContained | true | 自包含部署 |
| PublishSingleFile | true | 单文件发布 |
| RuntimeIdentifier | win-x64 | 运行时标识符 |
| Nullable | enable | 可空类型 |
| ImplicitUsings | enable | 隐式使用 |
| LangVersion | preview | 语言版本 |

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- Windows 10/11 x64 系统
- Visual Studio 2022 或更高版本（可选）

### 安装与配置

1. **克隆或下载** 本技能到本地目录
2. **构建项目**：
   ```bash
   dotnet build -c Release
   ```
3. **发布项目**（AOT 编译）：
   ```bash
   dotnet publish -c Release -r win-x64 --self-contained
   ```

### 基本使用

#### HTTP 传输示例

```bash
# 发送 GET 请求
transport_core --transport http --url "https://api.example.com/users"

# 发送 POST 请求
transport_core --transport http --url "https://api.example.com/users" --method POST --data "{\"name\": \"John\", \"email\": \"john@example.com\"}"
```

#### TCP 传输示例

```bash
# 发送 TCP 数据
transport_core --transport tcp --host "localhost" --port 8080 --data "Hello, TCP!"
```

#### UDP 传输示例

```bash
# 发送 UDP 数据
transport_core --transport udp --host "localhost" --port 8081 --data "Hello, UDP!"
```

#### 管道传输示例

```bash
# 使用管道处理数据
transport_core --transport pipeline --input "input.txt" --output "output.txt"
```

#### 通道传输示例

```bash
# 使用通道传输数据
transport_core --transport channel --capacity 1000
```

#### 缓冲区传输示例

```bash
# 使用缓冲区管理
transport_core --transport buffer --size 4096
```

#### Scrutor 示例

```bash
# 运行所有 Scrutor 示例
transport_generator --demo all

# 运行装饰器模式示例
transport_generator --demo decorator
```

## 核心功能

### HTTP 传输服务

HTTP 传输服务提供了基于 `System.Net.Http` 的 HTTP 客户端功能，支持 GET、POST、PUT、DELETE 等 HTTP 方法。

#### 主要特性

- **异步操作**：全异步 API，支持 `async/await`
- **自动重试**：内置重试机制
- **超时控制**：可配置的请求超时
- **请求/响应头**：完整的 HTTP 头支持
- **内容协商**：支持多种媒体类型

#### 使用示例

```csharp
var httpService = provider.GetRequiredService<IHttpTransportService>();

// 发送 GET 请求
var response = await httpService.GetAsync("https://api.example.com/users");

// 发送 POST 请求
var postData = "{\"name\": \"John\", \"email\": \"john@example.com\"}";
var postResponse = await httpService.PostAsync("https://api.example.com/users", postData);

// 发送自定义请求
var request = new HttpRequestMessage(HttpMethod.Put, "https://api.example.com/users/1");
request.Content = new StringContent("{\"name\": \"Updated Name\"}");
request.Headers.Add("Authorization", "Bearer token");
var customResponse = await httpService.SendAsync(request);
```

### TCP 传输服务

TCP 传输服务提供了基于 `System.Net.Sockets` 的 TCP 客户端功能，支持可靠的面向连接的通信。

#### 主要特性

- **异步操作**：全异步 API
- **连接池**：重用 TCP 连接
- **缓冲区管理**：优化的缓冲区大小
- **超时控制**：可配置的连接和操作超时
- **错误处理**：完善的异常处理机制

#### 使用示例

```csharp
var tcpService = provider.GetRequiredService<ITcpTransportService>();

// 发送数据
await tcpService.SendAsync("localhost", 8080, Encoding.UTF8.GetBytes("Hello, TCP!"));

// 接收数据
var response = await tcpService.ReceiveAsync("localhost", 8080);
var responseString = Encoding.UTF8.GetString(response);

// 使用流操作
using var stream = await tcpService.ConnectAsync("localhost", 8080);
var writer = new StreamWriter(stream);
await writer.WriteLineAsync("Hello from TCP stream!");
await writer.FlushAsync();

var reader = new StreamReader(stream);
var streamResponse = await reader.ReadLineAsync();
```

### UDP 传输服务

UDP 传输服务提供了基于 `System.Net.Sockets` 的 UDP 客户端功能，支持无连接的通信。

#### 主要特性

- **异步操作**：全异步 API
- **无连接**：不需要建立持久连接
- **广播支持**：支持 UDP 广播
- **多播支持**：支持 UDP 多播
- **缓冲区管理**：优化的缓冲区大小

#### 使用示例

```csharp
var udpService = provider.GetRequiredService<IUdpTransportService>();

// 发送数据
await udpService.SendAsync("localhost", 8081, Encoding.UTF8.GetBytes("Hello, UDP!"));

// 接收数据
var (responseData, endpoint) = await udpService.ReceiveAsync();
var responseString = Encoding.UTF8.GetString(responseData);
Console.WriteLine($"从 {endpoint} 接收到: {responseString}");
```

### 管道传输服务

管道传输服务提供了基于 `System.IO.Pipelines` 的高性能数据处理功能，适用于处理大量数据。

#### 主要特性

- **高性能**：零拷贝数据处理
- **背压支持**：自动背压机制
- **异步操作**：全异步 API
- **内存优化**：减少内存分配和复制
- **流处理**：支持流式数据处理

#### 使用示例

```csharp
var pipelineService = provider.GetRequiredService<IPipelineTransportService>();

// 创建管道读取器和写入器
using var stream = new MemoryStream();
var reader = pipelineService.CreateReader(stream);
var writer = pipelineService.CreateWriter(stream);

// 写入数据
var writeBuffer = writer.GetMemory(1024);
var data = Encoding.UTF8.GetBytes("Hello, Pipeline!");
data.CopyTo(writeBuffer);
writer.Advance(data.Length);
await writer.FlushAsync();

// 读取数据
stream.Position = 0;
var readResult = await reader.ReadAsync();
var readBuffer = readResult.Buffer;
if (readBuffer.Length > 0)
{
    var result = Encoding.UTF8.GetString(readBuffer.FirstSpan);
    Console.WriteLine($"管道读取: {result}");
}
reader.AdvanceTo(readBuffer.End);

// 使用处理器
await pipelineService.ProcessPipelineAsync(reader, writer, async (buffer) =>
{
    var input = Encoding.UTF8.GetString(buffer.FirstSpan);
    var processed = input.ToUpper();
    var processedData = Encoding.UTF8.GetBytes(processed);
    await writer.WriteAsync(processedData);
    return ProcessResult.Completed;
});
```

### 通道传输服务

通道传输服务提供了基于 `System.Threading.Channels` 的异步消息传递功能，适用于生产者-消费者场景。

#### 主要特性

- **异步操作**：全异步 API
- **背压支持**：自动背压机制
- **边界检查**：有界通道支持
- **取消支持**：CancellationToken 支持
- **异常处理**：完善的异常传播机制

#### 使用示例

```csharp
var channelService = provider.GetRequiredService<IChannelTransportService>();

// 创建通道
var channel = channelService.CreateChannel<string>(10); // 有界通道，容量为 10
var reader = channelService.CreateReader(channel);
var writer = channelService.CreateWriter(channel);

// 写入数据
await writer.WriteAsync("Message 1");
await writer.WriteAsync("Message 2");
await writer.WriteAsync("Message 3");
writer.Complete();

// 读取数据
await foreach (var message in reader.ReadAllAsync())
{
    Console.WriteLine($"通道接收: {message}");
}

// 使用处理器
var processorTask = channelService.ProcessChannelAsync(reader, async (message) =>
{
    Console.WriteLine($"处理消息: {message}");
    await Task.Delay(100); // 模拟处理延迟
});

// 写入更多数据
for (int i = 4; i <= 10; i++)
{
    await writer.WriteAsync($"Message {i}");
}
writer.Complete();

// 等待处理器完成
await processorTask;
```

### 缓冲区管理服务

缓冲区管理服务提供了基于 `System.Buffers` 的内存缓冲区管理功能，优化内存使用。

#### 主要特性

- **内存池**：重用缓冲区，减少 GC 压力
- **零拷贝**：使用 Span<T> 和 Memory<T> 进行零拷贝操作
- **大小优化**：根据需要分配适当大小的缓冲区
- **线程安全**：线程安全的缓冲区管理
- **生命周期管理**：自动缓冲区回收

#### 使用示例

```csharp
var bufferService = provider.GetRequiredService<IBufferTransportService>();

// 租用缓冲区
using var owner = bufferService.Rent(1024);
var buffer = owner.Memory;

// 写入数据
var data = Encoding.UTF8.GetBytes("Hello, Buffer!");
data.CopyTo(buffer.Span);

// 读取数据
var result = Encoding.UTF8.GetString(buffer.Span.Slice(0, data.Length));
Console.WriteLine($"缓冲区内容: {result}");

// 使用 Span 进行零拷贝操作
var span = bufferService.GetSpan(1024);
var written = Encoding.UTF8.GetBytes("Hello, Span!", span);
var spanResult = Encoding.UTF8.GetString(span.Slice(0, written));
Console.WriteLine($"Span 内容: {spanResult}");

// 使用 Memory 进行异步操作
var memory = bufferService.GetMemory(1024);
await Task.Run(() =>
{
    Encoding.UTF8.GetBytes("Hello, Memory!", memory.Span);
});
var memoryResult = Encoding.UTF8.GetString(memory.Span.Slice(0, 13));
Console.WriteLine($"Memory 内容: {memoryResult}");

// 清理缓冲区
bufferService.Clear(buffer);
```

### 依赖注入服务

依赖注入服务提供了基于 `Microsoft.Extensions.DependencyInjection` 的依赖注入功能，并集成了 Scrutor 用于装饰器模式实现。

#### 主要特性

- **自动注册**：自动扫描和注册服务
- **装饰器模式**：支持多层装饰器
- **生命周期管理**：支持 Singleton、Scoped、Transient 生命周期
- **服务过滤**：基于条件过滤服务
- **程序集扫描**：从程序集自动发现服务

#### 使用示例

```csharp
// 创建服务容器
var services = new ServiceCollection();

// 注册传输服务
services.AddTransportServices();

// 添加装饰器
services.Decorate<IHttpTransportService, LoggingHttpTransportDecorator>();
services.Decorate<IHttpTransportService, CachingHttpTransportDecorator>();

// 构建服务提供者
using var provider = services.BuildServiceProvider();

// 解析服务
var httpService = provider.GetRequiredService<IHttpTransportService>();

// 使用服务
var response = await httpService.GetAsync("https://api.example.com/users");
```

## API 参考

### 传输服务接口

#### IHttpTransportService

```csharp
public interface IHttpTransportService
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<string> GetAsync(string url, CancellationToken cancellationToken = default);
    Task<string> PostAsync(string url, string content, CancellationToken cancellationToken = default);
    Task<string> PutAsync(string url, string content, CancellationToken cancellationToken = default);
    Task<string> DeleteAsync(string url, CancellationToken cancellationToken = default);
}
```

#### ITcpTransportService

```csharp
public interface ITcpTransportService
{
    Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default);
    Task SendAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default);
    Task<byte[]> ReceiveAsync(string host, int port, CancellationToken cancellationToken = default);
    Task SendStreamAsync(string host, int port, Stream stream, CancellationToken cancellationToken = default);
    Task ReceiveStreamAsync(string host, int port, Stream stream, CancellationToken cancellationToken = default);
}
```

#### IUdpTransportService

```csharp
public interface IUdpTransportService
{
    Task SendAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default);
    Task<(byte[], IPEndPoint)> ReceiveAsync(CancellationToken cancellationToken = default);
    Task SendBroadcastAsync(int port, byte[] data, CancellationToken cancellationToken = default);
    Task JoinMulticastGroup(IPAddress multicastAddress, CancellationToken cancellationToken = default);
    Task LeaveMulticastGroup(IPAddress multicastAddress, CancellationToken cancellationToken = default);
}
```

#### IPipelineTransportService

```csharp
public interface IPipelineTransportService
{
    PipeReader CreateReader(Stream stream);
    PipeWriter CreateWriter(Stream stream);
    Task ProcessPipelineAsync(PipeReader reader, PipeWriter writer, Func<ReadOnlySequence<byte>, ValueTask<ProcessResult>> processor, CancellationToken cancellationToken = default);
    Task CopyPipeAsync(PipeReader reader, PipeWriter writer, CancellationToken cancellationToken = default);
}

public enum ProcessResult
{
    Completed,
    Continue,
    Error
}
```

#### IChannelTransportService

```csharp
public interface IChannelTransportService
{
    Channel<T> CreateChannel<T>(int capacity = -1);
    ChannelReader<T> CreateReader<T>(Channel<T> channel);
    ChannelWriter<T> CreateWriter<T>(Channel<T> channel);
    Task ProcessChannelAsync<T>(ChannelReader<T> reader, Func<T, ValueTask> processor, CancellationToken cancellationToken = default);
    Task<T[]> ReadAllAsync<T>(ChannelReader<T> reader, CancellationToken cancellationToken = default);
    Task WriteAllAsync<T>(ChannelWriter<T> writer, IEnumerable<T> items, CancellationToken cancellationToken = default);
}
```

#### IBufferTransportService

```csharp
public interface IBufferTransportService
{
    IMemoryOwner<byte> Rent(int size);
    void Return(IMemoryOwner<byte> owner);
    Memory<byte> GetMemory(int size);
    Span<byte> GetSpan(int size);
    void Clear(Memory<byte> memory);
    void Clear(Span<byte> span);
    int CopyTo(ReadOnlySpan<byte> source, Span<byte> destination);
    T[] ToArray<T>(ReadOnlySpan<T> span);
}
```

### 扩展方法

#### ServiceCollection 扩展

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransportServices(this IServiceCollection services);
    public static IServiceCollection AddHttpTransport(this IServiceCollection services);
    public static IServiceCollection AddTcpTransport(this IServiceCollection services);
    public static IServiceCollection AddUdpTransport(this IServiceCollection services);
    public static IServiceCollection AddPipelineTransport(this IServiceCollection services);
    public static IServiceCollection AddChannelTransport(this IServiceCollection services);
    public static IServiceCollection AddBufferTransport(this IServiceCollection services);
}
```

## 命令行接口

### transport_core 命令

#### 语法

```bash
transport_core [OPTIONS]
```

#### 选项

| 选项 | 描述 | 默认值 | 必需 |
|------|------|--------|------|
| `--transport` | 传输类型 (http, tcp, udp, pipeline, channel, buffer) | http | 否 |
| `--url` | HTTP URL | - | 否（HTTP 传输时必需） |
| `--method` | HTTP 方法 (GET, POST, PUT, DELETE) | GET | 否 |
| `--host` | TCP/UDP 主机 | localhost | 否（TCP/UDP 传输时必需） |
| `--port` | TCP/UDP 端口 | 8080 | 否（TCP/UDP 传输时必需） |
| `--data` | 传输数据 | - | 否 |
| `--input` | 输入文件路径 | - | 否（管道传输时可选） |
| `--output` | 输出文件路径 | - | 否（管道传输时可选） |
| `--capacity` | 通道容量 | -1 | 否（通道传输时可选） |
| `--size` | 缓冲区大小 | 4096 | 否（缓冲区传输时可选） |
| `--verbose` | 详细输出 | false | 否 |
| `--help` | 显示帮助信息 | - | 否 |
| `--version` | 显示版本信息 | - | 否 |

### transport_generator 命令

#### 语法

```bash
transport_generator [OPTIONS]
```

#### 选项

| 选项 | 描述 | 默认值 | 必需 |
|------|------|--------|------|
| `--demo` | 演示类型 (basic, decorator, filter, lifetime, advanced, scanning, multiple, conditional, all) | all | 否 |
| `--assembly` | 要扫描的程序集 | - | 否 |
| `--pattern` | 扫描模式 | *Service* | 否 |
| `--verbose` | 详细输出 | false | 否 |
| `--help` | 显示帮助信息 | - | 否 |
| `--version` | 显示版本信息 | - | 否 |

## 配置

### 环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| `DOTNET_ENVIRONMENT` | 运行环境 | Production |
| `DOTNET_LOGGING_LEVEL` | 日志级别 | Information |
| `DOTNET_AOT_ENABLED` | AOT 编译启用 | true |
| `TRANSPORT_HTTP_TIMEOUT` | HTTP 请求超时（秒） | 30 |
| `TRANSPORT_TCP_BUFFER_SIZE` | TCP 缓冲区大小 | 8192 |
| `TRANSPORT_UDP_BUFFER_SIZE` | UDP 缓冲区大小 | 8192 |
| `TRANSPORT_PIPELINE_MIN_SIZE` | 管道最小缓冲区大小 | 512 |
| `TRANSPORT_PIPELINE_MAX_SIZE` | 管道最大缓冲区大小 | 65536 |
| `TRANSPORT_CHANNEL_CAPACITY` | 通道容量 | -1 |
| `TRANSPORT_BUFFER_POOL_SIZE` | 缓冲区池大小 | 1024 |
| `SCRUTOR_DEMO_MODE` | Scrutor 演示模式 | false |
| `SCRUTOR_ASSEMBLY_SCAN` | 程序集扫描启用 | false |
| `SCRUTOR_DECORATOR_ENABLED` | 装饰器启用 | false |
| `SCRUTOR_FILTER_CRITERIA` | 服务过滤条件 | Service |
| `SCRUTOR_LIFETIME_TEST` | 生命周期测试 | false |
| `SCRUTOR_ADVANCED_FEATURES` | 高级功能启用 | false |
| `SCRUTOR_SCAN_PATTERN` | 扫描模式 | *Service* |
| `SCRUTOR_DECORATOR_COUNT` | 装饰器数量 | 2 |
| `SCRUTOR_CONDITIONAL_ENABLED` | 条件注册启用 | true |

### 配置文件

#### transport_core.setting.json

```json
{
  "compilation": {
    "options": {
      "TargetFramework": "net11.0",
      "PublishAot": true,
      "TrimMode": "partial",
      "SelfContained": true,
      "PublishSingleFile": true,
      "RuntimeIdentifier": "win-x64",
      "OutputType": "Exe",
      "Nullable": "enable",
      "ImplicitUsings": "enable",
      "LangVersion": "preview"
    },
    "dependencies": {
      "Microsoft.Extensions.DependencyInjection": "9.0.0",
      "Microsoft.Extensions.Logging": "9.0.0",
      "Microsoft.Extensions.Logging.Console": "9.0.0",
      "Microsoft.Extensions.Options": "9.0.0",
      "System.CommandLine": "2.0.0-beta4.22272.1",
      "System.Net.Http": "9.0.0",
      "System.Net.Sockets": "9.0.0",
      "System.IO.Pipelines": "9.0.0",
      "System.Threading.Channels": "9.0.0",
      "System.Buffers": "9.0.0"
    }
  }
}
```

#### transport_core.run.json

```json
{
  "runtime": {
    "environment_variables": {
      "DOTNET_ENVIRONMENT": "Production",
      "DOTNET_LOGGING_LEVEL": "Information",
      "DOTNET_AOT_ENABLED": "true",
      "TRANSPORT_HTTP_TIMEOUT": "30",
      "TRANSPORT_TCP_BUFFER_SIZE": "8192",
      "TRANSPORT_UDP_BUFFER_SIZE": "8192",
      "TRANSPORT_PIPELINE_MIN_SIZE": "512",
      "TRANSPORT_PIPELINE_MAX_SIZE": "65536",
      "TRANSPORT_CHANNEL_CAPACITY": "-1",
      "TRANSPORT_BUFFER_POOL_SIZE": "1024"
    },
    "profiles": {
      "HttpTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "http"],
        "environment_variables": {
          "TRANSPORT_HTTP_TIMEOUT": "30"
        }
      },
      "TcpTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "tcp"],
        "environment_variables": {
          "TRANSPORT_TCP_BUFFER_SIZE": "8192"
        }
      },
      "UdpTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "udp"],
        "environment_variables": {
          "TRANSPORT_UDP_BUFFER_SIZE": "8192"
        }
      },
      "PipelineTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "pipeline"],
        "environment_variables": {
          "TRANSPORT_PIPELINE_MIN_SIZE": "512",
          "TRANSPORT_PIPELINE_MAX_SIZE": "65536"
        }
      },
      "ChannelTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "channel"],
        "environment_variables": {
          "TRANSPORT_CHANNEL_CAPACITY": "1000"
        }
      },
      "BufferTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "buffer"],
        "environment_variables": {
          "TRANSPORT_BUFFER_POOL_SIZE": "1024"
        }
      }
    }
  }
}
```

#### transport_generator.setting.json

```json
{
  "compilation": {
    "options": {
      "TargetFramework": "net11.0",
      "PublishAot": true,
      "TrimMode": "partial",
      "SelfContained": true,
      "PublishSingleFile": true,
      "RuntimeIdentifier": "win-x64",
      "OutputType": "Exe",
      "Nullable": "enable",
      "ImplicitUsings": "enable",
      "LangVersion": "preview"
    },
    "dependencies": {
      "Microsoft.Extensions.DependencyInjection": "9.0.0",
      "Microsoft.Extensions.Logging": "9.0.0",
      "Microsoft.Extensions.Logging.Console": "9.0.0",
      "System.CommandLine": "2.0.0-beta4.22272.1",
      "Scrutor": "4.2.2",
      "System.Reflection.MetadataLoadContext": "9.0.0"
    }
  }
}
```

#### transport_generator.run.json

```json
{
  "runtime": {
    "environment_variables": {
      "DOTNET_ENVIRONMENT": "Development",
      "DOTNET_LOGGING_LEVEL": "Information",
      "SCRUTOR_DEMO_MODE": "true",
      "SCRUTOR_ASSEMBLY_SCAN": "true",
      "SCRUTOR_DECORATOR_ENABLED": "true",
      "SCRUTOR_FILTER_CRITERIA": "Service",
      "SCRUTOR_LIFETIME_TEST": "false",
      "SCRUTOR_ADVANCED_FEATURES": "false",
      "SCRUTOR_SCAN_PATTERN": "*Service*",
      "SCRUTOR_DECORATOR_COUNT": "2",
      "SCRUTOR_CONDITIONAL_ENABLED": "true"
    },
    "profiles": {
      "BasicRegistration": {
        "command": "transport_generator",
        "arguments": ["--demo", "basic"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "basic"
        }
      },
      "DecoratorPattern": {
        "command": "transport_generator",
        "arguments": ["--demo", "decorator"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "decorator",
          "SCRUTOR_DECORATOR_COUNT": "2"
        }
      },
      "ServiceFiltering": {
        "command": "transport_generator",
        "arguments": ["--demo", "filter"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "filter",
          "SCRUTOR_FILTER_CRITERIA": "Service"
        }
      },
      "LifetimeManagement": {
        "command": "transport_generator",
        "arguments": ["--demo", "lifetime"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "lifetime",
          "SCRUTOR_LIFETIME_TEST": "true"
        }
      },
      "AdvancedRegistration": {
        "command": "transport_generator",
        "arguments": ["--demo", "advanced"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "advanced",
          "SCRUTOR_ADVANCED_FEATURES": "true"
        }
      },
      "AssemblyScanning": {
        "command": "transport_generator",
        "arguments": ["--demo", "scanning"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "scanning",
          "SCRUTOR_ASSEMBLY_SCAN": "true",
          "SCRUTOR_SCAN_PATTERN": "*Service*"
        }
      },
      "MultipleDecorators": {
        "command": "transport_generator",
        "arguments": ["--demo", "multiple"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "multiple",
          "SCRUTOR_DECORATOR_COUNT": "3"
        }
      },
      "ConditionalRegistration": {
        "command": "transport_generator",
        "arguments": ["--demo", "conditional"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "conditional",
          "SCRUTOR_CONDITIONAL_ENABLED": "true"
        }
      },
      "AllDemos": {
        "command": "transport_generator",
        "arguments": ["--demo", "all"],
        "environment_variables": {
          "SCRUTOR_DEMO_MODE": "true",
          "SCRUTOR_ASSEMBLY_SCAN": "true",
          "SCRUTOR_DECORATOR_ENABLED": "true"
        }
      }
    }
  }
}
```

## 性能优化

### 内存管理

1. **使用 Span<T> 和 Memory<T>**：减少内存分配和复制
2. **利用对象池**：重用缓冲区和对象，减少 GC 压力
3. **避免字符串拼接**：使用 StringBuilder 或 Span<T> 进行字符串操作
4. **合理设置缓冲区大小**：根据实际数据大小调整缓冲区
5. **使用管道和通道**：对于大量数据，使用 System.IO.Pipelines 和 System.Threading.Channels

### 网络优化

1. **连接池**：重用 HTTP 和 TCP 连接
2. **批量操作**：减少网络往返次数
3. **压缩数据**：对于大 payload，使用压缩
4. **异步编程**：使用 async/await 进行非阻塞操作
5. **超时控制**：合理设置超时，避免长时间阻塞
6. **错误重试**：实现智能重试机制，处理网络瞬态错误

### AOT 编译优化

1. **TrimMode 设置**：使用 partial 模式减少裁剪
2. **反射使用**：避免运行时反射，使用静态分析
3. **依赖项管理**：确保所有依赖项支持 AOT
4. **资源管理**：正确处理非托管资源
5. **代码大小**：减少不必要的代码和依赖

### 并发优化

1. **异步编程**：使用 async/await 避免线程阻塞
2. **并行处理**：对于 CPU 密集型任务，使用 Parallel 或 Task.WhenAll
3. **线程池管理**：避免创建过多线程
4. **锁优化**：减少锁的范围和竞争
5. **无锁数据结构**：对于高并发场景，使用无锁数据结构

## 部署

### 部署选项

1. **独立部署**：使用 `--self-contained` 发布，包含运行时
2. **框架依赖部署**：依赖目标机器上的 .NET 运行时
3. **容器部署**：使用 Docker 容器
4. **云部署**：部署到 Azure、AWS 等云平台

### 容器化部署

#### Dockerfile 示例

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
WORKDIR /app

# 复制项目文件
COPY *.csproj .
RUN dotnet restore

# 复制源代码
COPY . .

# 发布项目
RUN dotnet publish -c Release -r win-x64 --self-contained

# 构建运行镜像
FROM mcr.microsoft.com/windows/servercore:ltsc2022
WORKDIR /app
COPY --from=build /app/bin/Release/net11.0/win-x64/publish .

# 设置环境变量
ENV DOTNET_ENVIRONMENT=Production
ENV DOTNET_LOGGING_LEVEL=Information

# 运行应用
ENTRYPOINT ["transport_core.exe"]
```

### 云部署

#### Azure 部署

1. **Azure Functions**：作为无服务器函数部署
2. **Azure App Service**：作为 Web 应用部署
3. **Azure Container Apps**：作为容器应用部署
4. **Azure VM**：在虚拟机上部署

#### AWS 部署

1. **AWS Lambda**：作为无服务器函数部署
2. **AWS ECS/EKS**：作为容器部署
3. **AWS EC2**：在 EC2 实例上部署

## 监控与日志

### 日志记录

1. **内置日志**：使用 Microsoft.Extensions.Logging
2. **结构化日志**：使用 JSON 格式记录日志
3. **日志级别**：根据环境设置适当的日志级别
4. **日志目标**：控制台、文件、ELK 堆栈等

### 指标监控

1. **性能计数器**：监控 CPU、内存、网络等指标
2. **自定义指标**：记录业务相关指标
3. **健康检查**：实现健康检查端点
4. **分布式追踪**：使用 OpenTelemetry 进行分布式追踪

### 告警机制

1. **阈值告警**：基于指标阈值触发告警
2. **异常检测**：检测异常模式
3. **告警渠道**：邮件、短信、Slack 等

## 安全性

### 网络安全

1. **HTTPS**：使用 HTTPS 保护网络通信
2. **证书管理**：正确管理 TLS 证书
3. **防火墙**：配置适当的防火墙规则
4. **网络隔离**：使用网络隔离保护敏感服务

### 数据安全

1. **数据加密**：加密敏感数据
2. **数据验证**：验证输入数据
3. **数据脱敏**：对日志中的敏感数据进行脱敏
4. **访问控制**：实现适当的访问控制

### 代码安全

1. **代码分析**：使用静态代码分析工具
2. **依赖项扫描**：扫描依赖项漏洞
3. **安全编码实践**：遵循安全编码规范
4. **定期审计**：定期进行安全审计

## 扩展性

### 扩展传输协议

1. **创建新的传输服务接口**
2. **实现接口**
3. **在 DI 容器中注册**
4. **更新命令行接口**
5. **添加配置选项**

### 扩展装饰器

1. **创建装饰器类**
2. **实现被装饰的接口**
3. **在构造函数中接收被装饰的实例**
4. **使用 `services.Decorate` 注册装饰器**

### 扩展命令行

1. **添加新的命令选项**
2. **更新命令处理逻辑**
3. **添加相应的配置选项**
4. **更新文档**

## 故障排除

### 常见问题

1. **AOT 编译失败**
   - 检查依赖项是否支持 AOT
   - 调整 TrimMode 设置
   - 避免运行时反射
   - 检查资源文件处理

2. **内存泄漏**
   - 检查对象池使用
   - 确保 Dispose 模式正确实现
   - 检查事件订阅是否正确取消
   - 使用内存分析工具

3. **性能问题**
   - 检查缓冲区大小
   - 优化网络连接
   - 使用性能分析工具
   - 检查并发处理

4. **网络连接问题**
   - 检查网络配置
   - 验证防火墙规则
   - 检查超时设置
   - 测试网络连通性

5. **依赖注入错误**
   - 检查服务注册
   - 确保装饰器顺序正确
   - 验证生命周期设置
   - 检查循环依赖

### 调试技巧

1. **启用详细日志**：设置 `DOTNET_LOGGING_LEVEL=Debug`
2. **使用诊断工具**：dotnet-dump、dotnet-trace、dotnet-counters
3. **代码分析**：使用 Roslyn 分析器
4. **单元测试**：编写针对性测试
5. **远程调试**：使用 Visual Studio 远程调试

## 示例

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

// 添加装饰器
services.Decorate<IHttpTransportService, LoggingHttpTransportDecorator>();
services.Decorate<IHttpTransportService, CachingHttpTransportDecorator>();

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
    await udpService.SendAsync("localhost", 8081, Encoding.UTF8.GetBytes("Hello UDP"));
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
    using var stream = new MemoryStream();
    var writer = pipelineService.CreateWriter(stream);
    await writer.WriteAsync(Encoding.UTF8.GetBytes("Hello Pipeline"));
    await writer.FlushAsync();
    
    stream.Position = 0;
    var reader = pipelineService.CreateReader(stream);
    var readResult = await reader.ReadAsync();
    var result = Encoding.UTF8.GetString(readResult.Buffer.FirstSpan);
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
    
    var data = Encoding.UTF8.GetBytes("Hello Buffer");
    data.CopyTo(buffer.Span);
    
    var result = Encoding.UTF8.GetString(buffer.Span.Slice(0, data.Length));
    logger.LogInformation($"缓冲区传输结果: {result}");
}
catch (Exception ex)
{
    logger.LogError(ex, "缓冲区传输测试失败");
}

logger.LogInformation("\n所有传输服务测试完成");
```

### Scrutor 装饰器模式示例

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
```

## 总结

Transport 技能是一个功能全面、高性能的传输层实现，提供了多种传输协议和数据处理技术。通过 AOT 编译优化、内存管理、网络优化等技术，实现了高效、可靠的传输服务。

本技能的主要优势包括：

1. **多种传输协议**：支持 HTTP、TCP、UDP 等多种传输协议
2. **高性能**：使用管道、通道、Span 等技术实现高性能数据处理
3. **可扩展**：通过依赖注入和装饰器模式实现灵活的扩展
4. **易于使用**：提供简洁的 API 和命令行接口
5. **安全可靠**：内置安全和错误处理机制
6. **全面的文档**：详细的中文文档和示例

Transport 技能适用于各种网络通信和数据传输场景，特别是对性能和可靠性要求较高的应用。通过本技能，开发者可以快速构建高效、可扩展的传输服务，专注于业务逻辑的实现。