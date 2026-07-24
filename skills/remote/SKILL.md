# Remote 技能

## 技能概述

基于 .NET 10 的高性能远程操作技能，为 .NET 开发者提供强大的远程执行、网络通信和分布式系统功能，支持 AOT 编译优化。

- **高性能设计**：优化的性能实现，支持 AOT 编译
- **内存管理**：实现对象池和内存池，减少垃圾回收，提高性能
- **依赖注入集成**：与 .NET 标准依赖注入框架无缝集成
- **配置管理**：提供灵活的配置选项，支持运行时调整
- **网络通信**：支持 HTTP、TCP/IP 等多种网络协议
- **安全认证**：内置安全认证机制
- **连接池**：实现连接池，提高网络操作性能
- **弹性模式**：实现重试和断路器模式，提高系统可靠性
- **错误处理**：完善的错误处理和恢复机制

## 快速开始

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Http@10.0.0
#:package Microsoft.Extensions.Diagnostics.HealthChecks@10.0.0
#:package System.Net.Http@8.0.0
#:package System.Text.Json@8.0.0
#:package System.Threading.Tasks.Dataflow@8.0.0
#:package System.Runtime.CompilerServices.Unsafe@6.0.0
#:package System.Net.Sockets@8.0.0
#:package System.Threading.Channels@8.0.0
#:package Polly@7.2.4
#:package System.Buffers@4.5.1
```

### 注册服务

在主应用程序中注册 Remote 服务：

```csharp
// 注册 OneRemote 服务
builder.Services.AddOneRemoteService(options => {
    options.ConnectionPoolSize = 100;
    options.Timeout = TimeSpan.FromSeconds(30);
    options.RetryCount = 3;
    options.CircuitBreakerFailureThreshold = 0.5;
    options.CircuitBreakerDurationOfBreak = TimeSpan.FromSeconds(30);
    options.EnableCompression = true;
    options.EnableEncryption = true;
});
```

### 使用示例

```csharp
// 获取 OneRemote 服务
var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();

// 启动服务
await remoteService.StartAsync();

// 连接到远程服务器
var connectionInfo = await remoteService.ConnectAsync("localhost", 22, "ssh");
Console.WriteLine($"连接成功: {connectionInfo.Host}:{connectionInfo.Port}");

// 创建会话
var sessionInfo = await remoteService.CreateSessionAsync("localhost", 22, "ssh");
Console.WriteLine($"会话创建成功: {sessionInfo.SessionId}");

// 执行远程命令
var commandResult = await remoteService.ExecuteCommandAsync(sessionInfo.SessionId, "echo hello world");
Console.WriteLine($"执行结果: {commandResult.Output}");

// 发送 HTTP 请求
var httpResult = await remoteService.SendHttpRequestAsync(new HttpRequestMessage(HttpMethod.Get, "https://api.example.com/data"));
Console.WriteLine($"HTTP 请求结果: {await httpResult.Content.ReadAsStringAsync()}");

// 断开连接
await remoteService.DisconnectAsync(connectionInfo.ConnectionId);

// 停止服务
await remoteService.StopAsync();
```

## 目录结构

```
remote/
├── index.yaml                   # 技能元数据
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── oneremote_integration.cs     # Remote 核心实现
    ├── oneremote_integration.run.json  # 运行配置
    ├── oneremote_integration.setting.json  # 设置文件
    ├── remotely_integration.cs     # Remote 扩展实现
    ├── remotely_integration.run.json  # 运行配置
    └── remotely_integration.setting.json  # 设置文件
```

## 主要功能

1. **远程命令执行**：支持在远程服务器上执行命令，返回执行结果
2. **网络通信**：支持 HTTP、TCP/IP 等多种网络协议，提供简单的 API 接口
3. **分布式任务处理**：支持分发任务到多个远程节点，并行处理
4. **远程资源管理**：支持管理远程服务器上的资源，如文件、进程等
5. **异步操作支持**：所有操作都支持异步执行，避免阻塞主线程
6. **安全认证**：内置安全认证机制，支持多种认证方式
7. **连接池**：实现网络连接池，减少连接建立和销毁的开销
8. **错误处理和恢复**：完善的错误处理和自动重试机制
9. **弹性模式**：实现重试和断路器模式，提高系统可靠性
10. **内存管理**：实现对象池和内存池，减少垃圾回收，提高性能
11. **高性能设计**：优化的性能实现，支持高并发场景
12. **易于使用的 API**：简单直观的 API 设计，易于集成和使用
13. **可扩展架构**：支持自定义扩展，满足特定需求

## AOT 编译配置

### 项目配置

在 `.csproj` 文件中添加以下配置：

```xml
<PropertyGroup>
  <TargetFramework>net11.0</TargetFramework>
  <PublishAot>true</PublishAot>
  <TrimMode>partial</TrimMode>
  <ReadyToRun>true</ReadyToRun>
  <TieredCompilation>true</TieredCompilation>
  <Optimize>true</Optimize>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  <SelfContained>true</SelfContained>
  <LangVersion>preview</LangVersion>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

### 运行时配置

在 `appsettings.json` 文件中添加以下配置：

```json
{
  "OneRemote": {
    "ConnectionPoolSize": 100,
    "Timeout": "00:00:30",
    "RetryCount": 3,
    "CircuitBreakerFailureThreshold": 0.5,
    "CircuitBreakerDurationOfBreak": "00:00:30",
    "EnableCompression": true,
    "EnableEncryption": true,
    "MaxConcurrentConnections": 10,
    "BatchSize": 100,
    "EnableDebugLogging": false
  }
}
```

## 核心 API

### IOneRemoteService 接口

```csharp
public interface IOneRemoteService
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
    Task<ConnectionInfo> ConnectAsync(string host, int port, string protocol = "ssh", CancellationToken cancellationToken = default);
    Task DisconnectAsync(Guid connectionId, CancellationToken cancellationToken = default);
    Task<SessionInfo> CreateSessionAsync(string host, int port, string protocol = "ssh", CancellationToken cancellationToken = default);
    Task CloseSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<CommandResult> ExecuteCommandAsync(Guid sessionId, string command, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<T> SendHttpRequestAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken = default);
    Task<byte[]> SendTcpRequestAsync(string host, int port, byte[] data, CancellationToken cancellationToken = default);
    Task<IEnumerable<ConnectionInfo>> GetActiveConnectionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetConnectionPoolUsageAsync(CancellationToken cancellationToken = default);
    Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
}
```

## 弹性模式

### 重试策略

Remote 技能使用 Polly 库实现了重试策略，用于处理网络操作中的临时失败：

- **重试次数**：可配置，默认为 3 次
- **重试间隔**：指数退避，每次重试间隔增加
- **重试条件**：仅对特定类型的异常进行重试（如网络异常）

### 断路器模式

Remote 技能使用 Polly 库实现了断路器模式，用于防止系统在服务不可用时继续发送请求：

- **失败阈值**：可配置，默认为 50%
- **断路持续时间**：可配置，默认为 30 秒
- **半开状态**：在断路持续时间结束后，尝试发送少量请求以检测服务是否恢复

## 内存管理

### 对象池

Remote 技能使用 `ObjectPool<byte[]>` 实现了对象池，用于重用 byte[] 数组，减少垃圾回收：

- **池大小**：可配置，默认为 100
- **缓冲区大小**：可配置，默认为 4096 字节

### 内存池

Remote 技能使用 `MemoryPool<byte>` 实现了内存池，用于更高效的内存管理：

- **内存分配**：使用 `Memory<byte>` 和 `Span<byte>` 进行内存操作
- **零拷贝**：尽可能使用零拷贝技术，减少内存复制

## 故障排除

### 常见问题

1. **连接失败**
   - 检查网络连接是否正常
   - 验证远程服务器地址和端口是否正确
   - 检查防火墙设置是否允许连接
   - 查看日志信息，了解具体错误原因

2. **执行超时**
   - 增加超时时间配置
   - 检查远程服务器响应速度
   - 优化命令执行逻辑，减少执行时间
   - 考虑使用异步执行方式

3. **认证失败**
   - 验证认证信息是否正确
   - 检查认证方式是否支持
   - 查看远程服务器的认证日志

4. **性能问题**
   - 启用连接池
   - 优化并发连接数
   - 使用批处理方式处理多个请求
   - 启用压缩，减少网络传输数据量
   - 调整对象池和内存池大小

5. **AOT 编译错误**
   - 确保所有依赖项支持 AOT 编译
   - 调整 `TrimMode` 为 `partial` 或 `full`
   - 检查是否使用了反射或动态类型
   - 查看编译日志，了解具体错误原因

6. **断路器触发**
   - 检查远程服务是否可用
   - 查看系统日志，了解失败原因
   - 调整断路器配置参数

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：妥善处理异常情况，提供友好的错误提示
4. **日志记录**：添加适当的日志记录，便于排查问题
5. **性能监控**：监控关键性能指标，及时发现和解决性能问题
6. **连接池管理**：合理配置连接池大小，避免资源浪费
7. **超时设置**：根据实际场景设置合理的超时时间
8. **重试机制**：对网络操作使用适当的重试机制，提高可靠性
9. **安全认证**：使用安全的认证方式，保护敏感信息
10. **批处理**：对多个相似请求使用批处理方式，减少网络开销
11. **压缩**：对大量数据传输启用压缩，提高传输速度
12. **资源释放**：及时释放网络连接等资源，避免资源泄漏
13. **内存管理**：合理使用对象池和内存池，减少垃圾回收
14. **断路器配置**：根据实际场景配置合理的断路器参数

## 扩展开发

### 添加自定义功能

1. **实现 IOneRemoteService 接口**：创建自定义的远程服务实现

```csharp
public class CustomRemoteService : IOneRemoteService
{
    private readonly OneRemoteOptions _options;
    private readonly ILogger<CustomRemoteService> _logger;
    
    public CustomRemoteService(IOptions<OneRemoteOptions> options, ILogger<CustomRemoteService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // 自定义启动逻辑
        _logger.LogInformation("Starting custom remote service");
        // 实现具体逻辑
    }
    
    // 实现其他接口方法...
}
```

2. **注册自定义服务**：

```csharp
builder.Services.AddOneRemoteService(options => {
    // 配置选项
});

builder.Services.AddSingleton<IOneRemoteService, CustomRemoteService>();
```

3. **扩展配置选项**：

```csharp
public class CustomRemoteOptions : OneRemoteOptions
{
    public string CustomSetting { get; set; } = "default";
}

// 注册配置
builder.Services.Configure<CustomRemoteOptions>(Configuration.GetSection("CustomRemote"));
```

## 总结

Remote 技能提供了一个完整的远程操作解决方案，基于 .NET 10 和 AOT 编译技术，具有高性能、可靠、易于使用的特点。通过本技能，您可以轻松实现远程命令执行、网络通信、分布式任务处理等功能，为您的应用程序添加强大的远程操作能力。

本技能支持多种扩展方式，您可以根据具体需求自定义实现，满足特定场景的要求。同时，我们提供了详细的文档和示例，帮助您快速上手和使用。

通过合理配置和使用 Remote 技能，您可以构建出高性能、可靠的分布式系统，提高应用程序的可扩展性和可用性。
