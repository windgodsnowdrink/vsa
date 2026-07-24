# Transport 技能参考文档

## 概述

Transport 技能是一个基于 .NET 10 的传输层实现，支持多种传输协议和高性能数据处理技术。本参考文档提供了详细的 API 参考、使用指南和最佳实践。

## 核心功能

- **多种传输协议支持**：HTTP、TCP、UDP
- **高性能数据处理**：System.IO.Pipelines、System.Threading.Channels
- **内存管理优化**：System.Buffers、Span<T>、Memory<T>
- **依赖注入增强**：Scrutor 装饰器模式实现
- **AOT 编译支持**：原生编译优化

## API 参考

### 传输服务接口

#### IHttpTransportService

```csharp
public interface IHttpTransportService
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<string> GetAsync(string url, CancellationToken cancellationToken = default);
    Task<string> PostAsync(string url, string content, CancellationToken cancellationToken = default);
}
```

#### ITcpTransportService

```csharp
public interface ITcpTransportService
{
    Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken = default);
    Task SendAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default);
    Task<byte[]> ReceiveAsync(string host, int port, CancellationToken cancellationToken = default);
}
```

#### IUdpTransportService

```csharp
public interface IUdpTransportService
{
    Task SendAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default);
    Task<(byte[], IPEndPoint)> ReceiveAsync(CancellationToken cancellationToken = default);
}
```

#### IPipelineTransportService

```csharp
public interface IPipelineTransportService
{
    PipeReader CreateReader(Stream stream);
    PipeWriter CreateWriter(Stream stream);
    Task ProcessPipelineAsync(PipeReader reader, PipeWriter writer, Func<ReadOnlySequence<byte>, ValueTask<ProcessResult>> processor, CancellationToken cancellationToken = default);
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
}
```

### Scrutor 扩展方法

#### 基本注册

```csharp
// 注册单个服务
services.AddSingleton<ISomeService, SomeService>();

// 使用 Scrutor 注册
services.Scan(scan => scan
    .FromAssemblyOf<ISomeService>()
    .AddClasses(classes => classes.AssignableTo<ISomeService>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());
```

#### 装饰器模式

```csharp
// 添加装饰器
services.Decorate<ISomeService, SomeServiceDecorator>();

// 多个装饰器
services.Decorate<ISomeService, FirstDecorator>();
services.Decorate<ISomeService, SecondDecorator>();
```

#### 服务过滤

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<ISomeService>()
    .AddClasses(classes => classes
        .AssignableTo<ISomeService>()
        .Where(type => type.Name.EndsWith("Service")))
    .AsImplementedInterfaces()
    .WithTransientLifetime());
```

#### 生命周期管理

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<ISomeService>()
    .AddClasses(classes => classes.AssignableTo<ISomeService>())
    .AsImplementedInterfaces()
    .WithLifetime(ServiceLifetime.Scoped));
```

#### 高级注册

```csharp
services.Scan(scan => scan
    .FromAssembliesOf(typeof(ISomeService), typeof(IOtherService))
    .AddClasses(false)
    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
    .AsImplementedInterfaces()
    .WithTransientLifetime());
```

#### 程序集扫描

```csharp
services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(classes => classes.Where(type => type.Name.Contains("Service")))
    .AsImplementedInterfaces()
    .WithSingletonLifetime());
```

#### 条件注册

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<ISomeService>()
    .AddClasses(classes => classes.AssignableTo<ISomeService>())
    .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name))
    .WithTransientLifetime());
```

## 配置参考

### 编译配置

#### transport_core.setting.json

```json
{
  "compilation": {
    "options": {
      "TargetFramework": "net11.0",
      "PublishAot": true,
      "TrimMode": "partial",
      "SelfContained": true,
      "PublishSingleFile": true
    },
    "dependencies": {
      "Microsoft.Extensions.DependencyInjection": "9.0.0",
      "System.Net.Http": "9.0.0",
      "System.Net.Sockets": "9.0.0",
      "System.IO.Pipelines": "9.0.0",
      "System.Threading.Channels": "9.0.0",
      "System.Buffers": "9.0.0"
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
      "TrimMode": "partial"
    },
    "dependencies": {
      "Microsoft.Extensions.DependencyInjection": "9.0.0",
      "Scrutor": "4.2.2"
    }
  }
}
```

### 运行时配置

#### transport_core.run.json

```json
{
  "runtime": {
    "environment_variables": {
      "DOTNET_ENVIRONMENT": "Production",
      "DOTNET_LOGGING_LEVEL": "Information"
    },
    "profiles": {
      "HttpTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "http"]
      },
      "TcpTransport": {
        "command": "transport_core",
        "arguments": ["--transport", "tcp"]
      }
    }
  }
}
```

#### transport_generator.run.json

```json
{
  "runtime": {
    "environment_variables": {
      "SCRUTOR_DEMO_MODE": "true"
    },
    "profiles": {
      "BasicRegistration": {
        "command": "transport_generator",
        "arguments": ["--demo", "basic"]
      },
      "DecoratorPattern": {
        "command": "transport_generator",
        "arguments": ["--demo", "decorator"]
      }
    }
  }
}
```

## 命令行接口

### transport_core 命令

```bash
# 显示帮助信息
transport_core --help

# 使用 HTTP 传输
transport_core --transport http --url "https://api.example.com" --method GET

# 使用 TCP 传输
transport_core --transport tcp --host "localhost" --port 8080 --data "Hello, TCP!"

# 使用 UDP 传输
transport_core --transport udp --host "localhost" --port 8081 --data "Hello, UDP!"

# 使用管道传输
transport_core --transport pipeline --input "input.txt" --output "output.txt"

# 使用通道传输
transport_core --transport channel --capacity 1000

# 使用缓冲区传输
transport_core --transport buffer --size 4096
```

### transport_generator 命令

```bash
# 显示帮助信息
transport_generator --help

# 运行所有演示
transport_generator --demo all

# 运行基本注册演示
transport_generator --demo basic

# 运行装饰器模式演示
transport_generator --demo decorator

# 运行服务过滤演示
transport_generator --demo filter

# 运行生命周期管理演示
transport_generator --demo lifetime

# 运行高级注册演示
transport_generator --demo advanced

# 运行程序集扫描演示
transport_generator --demo scanning

# 运行多个装饰器演示
transport_generator --demo multiple

# 运行条件注册演示
transport_generator --demo conditional
```

## 环境变量

### 通用环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| DOTNET_ENVIRONMENT | 运行环境 | Production |
| DOTNET_LOGGING_LEVEL | 日志级别 | Information |
| DOTNET_AOT_ENABLED | AOT 编译启用 | true |

### 传输相关环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| TRANSPORT_HTTP_TIMEOUT | HTTP 请求超时（秒） | 30 |
| TRANSPORT_TCP_BUFFER_SIZE | TCP 缓冲区大小 | 8192 |
| TRANSPORT_UDP_BUFFER_SIZE | UDP 缓冲区大小 | 8192 |
| TRANSPORT_PIPELINE_MIN_SIZE | 管道最小缓冲区大小 | 512 |
| TRANSPORT_PIPELINE_MAX_SIZE | 管道最大缓冲区大小 | 65536 |
| TRANSPORT_CHANNEL_CAPACITY | 通道容量 | -1 (无限制) |
| TRANSPORT_BUFFER_POOL_SIZE | 缓冲区池大小 | 1024 |

### Scrutor 相关环境变量

| 环境变量 | 描述 | 默认值 |
|---------|------|--------|
| SCRUTOR_DEMO_MODE | Scrutor 演示模式 | true |
| SCRUTOR_ASSEMBLY_SCAN | 程序集扫描启用 | true |
| SCRUTOR_DECORATOR_ENABLED | 装饰器启用 | true |
| SCRUTOR_FILTER_CRITERIA | 服务过滤条件 | Service |
| SCRUTOR_LIFETIME_TEST | 生命周期测试 | false |
| SCRUTOR_ADVANCED_FEATURES | 高级功能启用 | false |
| SCRUTOR_SCAN_PATTERN | 扫描模式 | *Service* |
| SCRUTOR_DECORATOR_COUNT | 装饰器数量 | 2 |
| SCRUTOR_CONDITIONAL_ENABLED | 条件注册启用 | true |

## 性能优化

### 内存管理

1. **使用 Span<T> 和 Memory<T>**：减少内存分配和复制
2. **利用对象池**：重用缓冲区和对象，减少 GC 压力
3. **管道和通道**：使用 System.IO.Pipelines 和 System.Threading.Channels 进行高效数据处理

### 网络优化

1. **连接池**：重用 HTTP 和 TCP 连接
2. **缓冲区大小调整**：根据实际数据大小调整缓冲区
3. **异步编程**：使用 async/await 进行非阻塞操作
4. **CancellationToken**：支持取消操作，避免资源泄漏

### AOT 编译优化

1. **TrimMode 设置**：使用 partial 模式减少裁剪
2. **反射使用**：避免运行时反射，使用静态分析
3. **依赖项管理**：确保所有依赖项支持 AOT

## 最佳实践

1. **依赖注入**：使用构造函数注入，避免服务定位器模式
2. **接口分离**：遵循 ISP 原则，拆分大接口
3. **错误处理**：使用异常和状态码进行错误处理
4. **日志记录**：在关键操作处添加日志
5. **配置管理**：使用 Options 模式管理配置
6. **测试**：编写单元测试和集成测试
7. **文档**：保持代码和文档同步

## 故障排除

### 常见问题

1. **AOT 编译失败**
   - 检查依赖项是否支持 AOT
   - 调整 TrimMode 设置
   - 避免运行时反射

2. **内存泄漏**
   - 检查对象池使用
   - 确保 Dispose 模式正确实现
   - 使用内存分析工具

3. **性能问题**
   - 检查缓冲区大小
   - 优化网络连接
   - 使用性能分析工具

4. **依赖注入错误**
   - 检查服务注册
   - 确保装饰器顺序正确
   - 验证生命周期设置

### 调试技巧

1. **启用详细日志**：设置 DOTNET_LOGGING_LEVEL=Debug
2. **使用诊断工具**：dotnet-dump、dotnet-trace
3. **代码分析**：使用 Roslyn 分析器
4. **单元测试**：编写针对性测试

## 扩展指南

### 添加新传输协议

1. 创建新的传输服务接口
2. 实现接口
3. 在 DI 容器中注册
4. 更新命令行接口
5. 添加配置选项

### 扩展 Scrutor 用法

1. 创建新的服务和装饰器
2. 实现自定义注册逻辑
3. 添加演示代码
4. 更新文档

### 集成第三方库

1. 确保库支持 AOT
2. 添加依赖项
3. 实现适配器
4. 注册服务

## 总结

Transport 技能提供了一个全面的传输层实现，支持多种传输协议和高性能数据处理技术。通过 AOT 编译优化和 Scrutor 增强的依赖注入，它提供了一个高效、可扩展的传输解决方案。

本参考文档涵盖了所有核心功能、API 和使用指南，帮助开发者快速上手和扩展 Transport 技能。