# P2P Agent Skill - P2P 技能

## 技能概述

P2P 技能是一个基于 .NET 10 的高性能点对点网络解决方案，为 .NET 开发者提供强大的 P2P 功能。该技能支持 AOT 编译，具有高性能、可扩展性和安全性等特点，适用于各种 P2P 网络场景，如分布式系统、文件共享、实时通信、区块链、分布式存储等。

## 快速开始指南

### 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS
- 网络连接（P2P 通信需要）

### 安装依赖

在你的主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Net.Sockets@10.0.0
#:package System.Net.Http@10.0.0
#:package System.Threading.Channels@10.0.0
#:package System.Text.Json@10.0.0
#:package System.Security.Cryptography@10.0.0
#:package System.Collections.Immutable@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true
```

### 注册服务

在你的主应用程序中注册 P2P 服务：

```csharp
// 注册 P2P 服务
builder.Services.AddP2PServices(options =>
{
    options.Enabled = true;
    options.MaxConnections = 100;
    options.Port = 8080;
    options.BufferSize = 8192;
    options.Timeout = TimeSpan.FromMinutes(1);
    options.EnableEncryption = true;
    options.EnableDiscovery = true;
    options.DiscoveryInterval = TimeSpan.FromSeconds(30);
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = Environment.ProcessorCount;
    options.EnableBatching = true;
    options.BatchSize = 100;
    options.EnableCaching = true;
    options.CacheSize = 1000;
    options.CacheDuration = TimeSpan.FromMinutes(5);
});
```

### 使用示例

```csharp
// 获取 P2P 服务
var p2pService = serviceProvider.GetRequiredService<IP2PService>();
var fileSharingService = serviceProvider.GetRequiredService<IP2PFileSharingService>();
var messagingService = serviceProvider.GetRequiredService<IP2PMessagingService>();
var networkService = serviceProvider.GetRequiredService<IP2PNetworkService>();
var securityService = serviceProvider.GetRequiredService<IP2PSecurityService>();

// 初始化 P2P 服务
await p2pService.InitializeAsync();
Console.WriteLine("P2P 服务初始化成功");

// 发现节点
var nodes = await p2pService.DiscoverNodesAsync();
Console.WriteLine($"发现 {nodes.Count} 个节点");

// 连接到节点
if (nodes.Any())
{
    var node = nodes[0];
    var connected = await p2pService.ConnectAsync(node);
    Console.WriteLine($"连接到节点 {node.Id}: {connected}");

    // 发送消息
    await messagingService.SendMessageAsync(node.Id, "Hello from P2P network!");
    Console.WriteLine("消息发送成功");

    // 共享文件
    await fileSharingService.ShareFileAsync(@"C:\example.txt");
    Console.WriteLine("文件共享成功");

    // 断开连接
    await p2pService.DisconnectAsync(node.Id);
    Console.WriteLine($"与节点 {node.Id} 断开连接");
}

// 关闭 P2P 服务
await p2pService.ShutdownAsync();
Console.WriteLine("P2P 服务已关闭");
```

## 导航地图

```
p2p/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── p2p_integration.cs     # P2P 集成功能实现
    ├── p2p_integration.run.json  # 运行配置
    └── p2p_integration.setting.json  # 设置文件
```

## 核心功能

1. **P2P 节点发现与连接管理**：自动发现网络中的其他节点，建立和维护连接
2. **分布式消息传递**：在 P2P 网络中可靠地传递消息
3. **文件共享与传输**：在 P2P 网络中共享和传输文件，支持断点续传
4. **实时通信**：支持实时的 P2P 通信
5. **网络拓扑管理**：管理 P2P 网络的拓扑结构，优化数据传输路径
6. **故障检测与恢复**：检测节点故障并自动恢复网络连接
7. **安全性与加密**：提供数据加密和身份认证功能
8. **性能优化**：优化 P2P 网络的性能，提高数据传输速度

## 技术特性

1. **模块化设计**：采用模块化设计，便于扩展和维护
2. **依赖注入**：支持 .NET 依赖注入，便于服务管理
3. **异步编程**：使用 async/await 模式，提高并发性能
4. **高性能算法**：实现高效的 P2P 网络算法
5. **网络协议实现**：支持多种 P2P 网络协议
6. **消息序列化优化**：优化消息序列化和反序列化性能
7. **错误处理与重试机制**：提供完善的错误处理和重试机制
8. **配置管理**：支持灵活的配置管理
9. **状态机设计**：使用状态机管理连接和会话状态
10. **管道处理模式**：使用管道模式处理网络数据

## 性能特性

1. **高性能设计**：优化的性能实现，支持高并发
2. **内存优化**：减少内存使用，提高内存效率
3. **并发支持**：支持并行处理，提高处理速度
4. **异步编程**：使用异步 API，避免阻塞
5. **批量处理**：支持批量处理，提高效率
6. **连接池管理**：管理网络连接池，减少连接建立开销
7. **网络拥塞控制**：实现网络拥塞控制，提高网络稳定性
8. **缓存机制**：使用缓存，减少重复计算和网络传输
9. **零拷贝技术**：使用零拷贝技术，提高数据传输速度
10. **内存池管理**：使用内存池，减少内存分配和回收开销

## AOT 编译支持

P2P 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 编译选项

```yaml
# AOT 编译选项
PublishAot: true      # 启用 AOT 编译
ReadyToRun: true      # 启用 ReadyToRun 编译
TieredCompilation: true  # 启用分层编译
TrimMode: partial     # 剪裁模式
Optimize: true        # 启用优化
EnableCompilationRelaxations: true  # 启用编译松弛
EnableEnhancedNgen: true  # 启用增强的 Ngen
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

### 编译命令

```bash
# 使用 AOT 编译 P2P 技能
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true
```

## 扩展说明

P2P 技能提供了完整的 P2P 解决方案，你可以根据需要进行扩展：

1. **自定义服务实现**：实现 IP2PService 接口，提供自定义的 P2P 服务实现
2. **扩展功能**：添加新的 P2P 功能，如自定义协议、高级路由算法等
3. **与其他系统集成**：将 P2P 技能与其他系统集成，如消息队列、存储系统等
4. **性能优化**：针对特定场景优化性能，如大规模网络、高带宽传输等
5. **安全增强**：添加额外的安全措施，如高级加密算法、身份验证机制等

### 示例：自定义 P2P 服务实现

```csharp
public class CustomP2PService : IP2PService
{
    private readonly ILogger<CustomP2PService> _logger;
    private readonly P2POptions _options;

    public CustomP2PService(ILogger<CustomP2PService> logger, IOptions<P2POptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // 自定义初始化逻辑
        _logger.LogInformation("Custom P2P service initialized");
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<P2PNode>> DiscoverNodesAsync(CancellationToken cancellationToken = default)
    {
        // 自定义节点发现逻辑
        return new List<P2PNode>();
    }

    // 实现其他接口方法...
}
```

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，便于测试和扩展
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，提供适当的错误消息
4. **日志记录**：添加适当的日志记录，便于故障排除
5. **性能监控**：监控关键性能指标，如连接数、消息延迟、文件传输速度等
6. **资源管理**：正确管理网络连接、文件句柄等资源，避免资源泄漏
7. **配置管理**：使用配置文件或环境变量管理配置，便于部署和维护
8. **安全实践**：遵循安全最佳实践，如使用加密、验证输入等
9. **网络优化**：根据网络环境调整 P2P 配置，如缓冲区大小、超时时间等
10. **扩展性设计**：设计时考虑系统的扩展性，便于后续功能添加和性能优化

## 配置选项

P2P 技能提供了丰富的配置选项，可以根据需要进行调整：

### 基本配置

```json
{
  "p2p.options": {
    "enabled": true,            // 是否启用 P2P 服务
    "maxConnections": 100,      // 最大连接数
    "port": 8080,               // 监听端口
    "bufferSize": 8192,         // 缓冲区大小
    "timeout": "00:01:00",      // 超时时间
    "enableEncryption": true,   // 是否启用加密
    "enableDiscovery": true,    // 是否启用节点发现
    "discoveryInterval": "00:00:30"  // 节点发现间隔
  }
}
```

### 性能配置

```json
{
  "p2p.performance": {
    "enableParallelProcessing": true,  // 是否启用并行处理
    "maxDegreeOfParallelism": 4,       // 最大并行度
    "enableBatching": true,            // 是否启用批量处理
    "batchSize": 100,                  // 批处理大小
    "enableCaching": true,             // 是否启用缓存
    "cacheSize": 1000,                 // 缓存大小
    "cacheDuration": "00:05:00"        // 缓存持续时间
  }
}
```

### 安全配置

```json
{
  "p2p.security": {
    "enableEncryption": true,          // 是否启用加密
    "encryptionAlgorithm": "AES-256",  // 加密算法
    "enableAuthentication": true,       // 是否启用身份认证
    "authenticationMethod": "RSA",     // 认证方法
    "certificatePath": "cert.pfx",     // 证书路径
    "certificatePassword": "password"  // 证书密码
  }
}
```

## 部署指南

### 自包含部署

```bash
# 构建自包含部署包
dotnet publish -c Release -r win-x64 --self-contained true

# 运行应用
./bin/Release/net10.0/win-x64/publish/YourApp.exe
```

### AOT 编译部署

```bash
# 使用 AOT 编译构建
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true

# 运行应用
./bin/Release/net10.0/win-x64/publish/YourApp.exe
```

### 容器化部署

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0-windowsservercore-ltsc2022 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["YourApp.csproj", "."]
RUN dotnet restore "YourApp.csproj"
COPY . .
WORKDIR "/src/YourApp"
RUN dotnet build "YourApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YourApp.csproj" -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["YourApp.exe"]
```

## 故障排除

### 常见问题

1. **节点发现失败**
   - 检查网络连接是否正常
   - 检查防火墙设置，确保端口已开放
   - 检查节点发现配置是否正确

2. **连接建立失败**
   - 检查网络连接是否正常
   - 检查目标节点是否在线
   - 检查端口是否已开放
   - 检查加密配置是否正确

3. **消息传递失败**
   - 检查网络连接是否正常
   - 检查目标节点是否在线
   - 检查消息大小是否超过限制
   - 检查消息格式是否正确

4. **文件传输失败**
   - 检查网络连接是否正常
   - 检查文件是否存在
   - 检查文件大小是否超过限制
   - 检查磁盘空间是否充足

5. **性能问题**
   - 检查系统资源使用情况（CPU、内存、网络）
   - 调整并行处理设置
   - 启用缓存
   - 优化网络配置

### 日志记录

P2P 技能提供了详细的日志记录，可以帮助诊断问题：

```json
{
  "logging": {
    "logLevel": {
      "Default": "Information",
      "P2P": "Debug"  // 设置 P2P 相关日志为 Debug 级别
    }
  }
}
```

## 结论

P2P 技能是一个功能强大的点对点网络解决方案，可以帮助开发者构建各种 P2P 应用，如分布式系统、文件共享、实时通信、区块链、分布式存储等。该技能支持 .NET 10 AOT 编译，具有高性能、可扩展性和安全性等特点，适用于各种规模的项目。

通过本文档，你应该已经了解了 P2P 技能的核心功能、技术特性、使用方法和最佳实践。如果你有任何问题或建议，请参考参考文档或联系 VSA Architecture Team。
