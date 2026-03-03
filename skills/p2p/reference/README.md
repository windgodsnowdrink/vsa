# P2P 技能 - 参考文档

## 概述

P2P 技能是一个基于 .NET 10 构建的高性能点对点网络通信系统，专为 .NET 开发者设计。它提供了完整的 P2P 网络功能，包括节点发现、连接管理、消息传递、文件共享和网络拓扑优化等核心特性。该技能支持 AOT 编译，具有高性能、可扩展性和安全性等特点，适用于各种 P2P 网络场景，如分布式系统、文件共享、实时通信、区块链、分布式存储等。

## 核心组件

### 1. IP2PService
- **位置**: scripts/p2p_integration.cs
- **功能**: 核心 P2P 网络服务，负责节点发现、连接管理和网络状态监控
- **主要方法**:
  - `InitializeAsync`: 初始化 P2P 服务
  - `ShutdownAsync`: 关闭 P2P 服务
  - `DiscoverNodesAsync`: 发现网络中的节点
  - `GetNodeAsync`: 获取指定节点
  - `ConnectAsync`: 连接到指定节点
  - `DisconnectAsync`: 断开与指定节点的连接
  - `IsConnectedAsync`: 检查是否已连接到指定节点
  - `GetConnectionCountAsync`: 获取当前连接数
  - `GetNetworkStatusAsync`: 获取网络状态

### 2. IP2PFileSharingService
- **位置**: scripts/p2p_integration.cs
- **功能**: 文件共享服务，负责文件的分享、下载和传输状态管理
- **主要方法**:
  - `ShareFileAsync`: 分享文件
  - `DownloadFileAsync`: 下载文件
  - `GetSharedFilesAsync`: 获取已分享的文件
  - `UnshareFileAsync`: 取消分享文件
  - `GetTransferStatusAsync`: 获取文件传输状态

### 3. IP2PMessagingService
- **位置**: scripts/p2p_integration.cs
- **功能**: 消息传递服务，负责节点间的消息发送和接收
- **主要方法**:
  - `SendMessageAsync`: 发送文本消息到指定节点
  - `SendMessageAsync`: 发送二进制消息到指定节点
  - `GetMessagesAsync`: 获取所有消息
  - `GetMessageAsync`: 获取指定消息
  - `DeleteMessageAsync`: 删除消息
  - `SubscribeToMessagesAsync`: 订阅消息

### 4. IP2PNetworkService
- **位置**: scripts/p2p_integration.cs
- **功能**: 网络拓扑服务，负责网络拓扑的管理和优化
- **主要方法**:
  - `GetNetworkTopologyAsync`: 获取网络拓扑
  - `OptimizeNetworkTopologyAsync`: 优化网络拓扑
  - `AddNodeAsync`: 添加节点
  - `RemoveNodeAsync`: 移除节点
  - `GetNeighborsAsync`: 获取指定节点的邻居
  - `GetNetworkSizeAsync`: 获取网络大小
  - `GetLocalNodeAsync`: 获取本地节点

### 5. IP2PSecurityService
- **位置**: scripts/p2p_integration.cs
- **功能**: 安全服务，负责数据加密、解密和节点认证
- **主要方法**:
  - `EncryptAsync`: 加密数据
  - `DecryptAsync`: 解密数据
  - `AuthenticateNodeAsync`: 认证节点
  - `GenerateKeyPairAsync`: 生成密钥对
  - `SignDataAsync`: 签名数据
  - `VerifySignatureAsync`: 验证签名

## 快速开始

### 环境要求

- .NET 10 SDK 或更高版本
- 支持 AOT 编译的操作系统（Windows、Linux、macOS）

### 安装依赖

```bash
dotnet add package Microsoft.Extensions.DependencyInjection@10.0.0
dotnet add package Microsoft.Extensions.Logging@10.0.0
dotnet add package Microsoft.Extensions.Logging.Console@10.0.0
dotnet add package Microsoft.Extensions.Options@10.0.0
dotnet add package System.Net.Sockets@10.0.0
dotnet add package System.Net.Http@10.0.0
dotnet add package System.Threading.Channels@10.0.0
dotnet add package System.Text.Json@10.0.0
dotnet add package System.Security.Cryptography@10.0.0
dotnet add package System.Collections.Immutable@10.0.0
```

### 服务注册

```csharp
using P2P;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();
// ...
app.Run();
```

## 使用示例

### 基本使用

```csharp
using P2P;

// 获取 P2P 服务
var p2pService = serviceProvider.GetRequiredService<IP2PService>();

// 初始化服务
await p2pService.InitializeAsync();
Console.WriteLine("P2P 服务初始化成功");

// 发现节点
var nodes = await p2pService.DiscoverNodesAsync();
Console.WriteLine($"发现 {nodes.Count()} 个节点");

// 连接到节点
foreach (var node in nodes)
{
    var connected = await p2pService.ConnectAsync(node.Id);
    Console.WriteLine($"连接到节点 {node.Id}: {connected}");
}

// 检查连接状态
var connectionCount = await p2pService.GetConnectionCountAsync();
Console.WriteLine($"当前连接数: {connectionCount}");

// 获取网络状态
var status = await p2pService.GetNetworkStatusAsync();
Console.WriteLine($"网络状态: {status.Status}");
Console.WriteLine($"连接数: {status.ConnectionCount}");
Console.WriteLine($"节点数: {status.NodeCount}");
Console.WriteLine($"网络健康度: {status.NetworkHealth:F2}%");

// 断开连接
foreach (var node in nodes)
{
    var disconnected = await p2pService.DisconnectAsync(node.Id);
    Console.WriteLine($"断开与节点 {node.Id} 的连接: {disconnected}");
}

// 关闭服务
await p2pService.ShutdownAsync();
Console.WriteLine("P2P 服务已关闭");
```

### 文件共享示例

```csharp
using P2P;

// 获取文件共享服务
var fileSharingService = serviceProvider.GetRequiredService<IP2PFileSharingService>();

// 分享文件
var testFile = System.IO.Path.Combine(Environment.CurrentDirectory, "test.txt");
System.IO.File.WriteAllText(testFile, "This is a test file for P2P sharing");
await fileSharingService.ShareFileAsync(testFile);
Console.WriteLine("文件分享成功");

// 获取已分享的文件
var sharedFiles = await fileSharingService.GetSharedFilesAsync();
Console.WriteLine($"已分享 {sharedFiles.Count()} 个文件:");
foreach (var file in sharedFiles)
{
    Console.WriteLine($"- {file.Name} (大小: {file.Size} 字节, ID: {file.Id})");
}

// 下载文件
if (sharedFiles.Any())
{
    var fileId = sharedFiles.First().Id;
    var destinationPath = System.IO.Path.Combine(Environment.CurrentDirectory, "downloaded_test.txt");
    var transferId = await fileSharingService.DownloadFileAsync(fileId, destinationPath);
    Console.WriteLine($"开始下载文件，传输 ID: {transferId}");

    // 获取传输状态
    var transferStatus = await fileSharingService.GetTransferStatusAsync(transferId);
    Console.WriteLine($"传输状态: {transferStatus.Status}");
    Console.WriteLine($"进度: {transferStatus.Progress}%");
    Console.WriteLine($"已传输: {transferStatus.TransferredSize} / {transferStatus.TotalSize} 字节");

    // 取消分享文件
    var unshared = await fileSharingService.UnshareFileAsync(fileId);
    Console.WriteLine($"取消分享文件: {unshared}");
}
```

### 消息传递示例

```csharp
using P2P;

// 获取消息服务
var messagingService = serviceProvider.GetRequiredService<IP2PMessagingService>();

// 订阅消息
await messagingService.SubscribeToMessagesAsync(async message =>
{
    Console.WriteLine($"收到消息从 {message.SenderId}:");
    Console.WriteLine($"内容: {message.Content}");
    Console.WriteLine($"状态: {message.Status}");
});
Console.WriteLine("已订阅消息");

// 发送文本消息
var nodeId = "target-node-id";
var messageId = await messagingService.SendMessageAsync(nodeId, "Hello from P2P network!");
Console.WriteLine($"发送文本消息成功，消息 ID: {messageId}");

// 发送二进制消息
var binaryData = System.Text.Encoding.UTF8.GetBytes("Binary message content");
var binaryMessageId = await messagingService.SendMessageAsync(nodeId, binaryData);
Console.WriteLine($"发送二进制消息成功，消息 ID: {binaryMessageId}");

// 获取消息
var message = await messagingService.GetMessageAsync(messageId);
Console.WriteLine($"获取消息: {message.Content}, 状态: {message.Status}");

// 删除消息
var deleted = await messagingService.DeleteMessageAsync(messageId);
Console.WriteLine($"删除消息: {deleted}");
```

### 网络拓扑示例

```csharp
using P2P;

// 获取网络服务
var networkService = serviceProvider.GetRequiredService<IP2PNetworkService>();

// 获取本地节点
var localNode = await networkService.GetLocalNodeAsync();
Console.WriteLine($"本地节点: {localNode.Name} (ID: {localNode.Id})");

// 添加模拟节点
for (int i = 1; i <= 3; i++)
{
    var node = new P2PNode
    {
        Id = $"node-{i}",
        Name = $"Node {i}",
        Address = System.Net.IPAddress.Parse($"192.168.1.{100 + i}"),
        Port = 8080 + i
    };
    var added = await networkService.AddNodeAsync(node);
    Console.WriteLine($"添加节点 {node.Id}: {added}");
}

// 获取网络大小
var networkSize = await networkService.GetNetworkSizeAsync();
Console.WriteLine($"网络大小: {networkSize} 个节点");

// 获取网络拓扑
var topology = await networkService.GetNetworkTopologyAsync();
Console.WriteLine($"网络拓扑: {topology.NodeCount} 个节点, {topology.ConnectionCount} 个连接");

// 优化网络拓扑
await networkService.OptimizeNetworkTopologyAsync();
Console.WriteLine("网络拓扑优化完成");
```

### 安全服务示例

```csharp
using P2P;

// 获取安全服务
var securityService = serviceProvider.GetRequiredService<IP2PSecurityService>();

// 生成密钥对
var keyPair = await securityService.GenerateKeyPairAsync();
Console.WriteLine($"生成密钥对: {keyPair}");

// 加密数据
var originalData = System.Text.Encoding.UTF8.GetBytes("Sensitive data");
var encryptedData = await securityService.EncryptAsync(originalData, "target-node-id");
Console.WriteLine($"加密数据长度: {encryptedData.Length}");

// 解密数据
var decryptedData = await securityService.DecryptAsync(encryptedData, "target-node-id");
var decryptedText = System.Text.Encoding.UTF8.GetString(decryptedData);
Console.WriteLine($"解密数据: {decryptedText}");

// 签名数据
var signature = await securityService.SignDataAsync(originalData);
Console.WriteLine($"签名长度: {signature.Length}");

// 验证签名
var verified = await securityService.VerifySignatureAsync(originalData, signature, "source-node-id");
Console.WriteLine($"签名验证: {verified}");

// 认证节点
var nodeToAuthenticate = new P2PNode
{
    Id = "auth-node-id",
    Name = "Node to authenticate",
    Address = System.Net.IPAddress.Parse("192.168.1.100"),
    Port = 8080
};
var authenticated = await securityService.AuthenticateNodeAsync(nodeToAuthenticate);
Console.WriteLine($"节点认证: {authenticated}");
```

## 配置选项

### P2POptions 配置

```json
{
  "P2POptions": {
    "Enabled": true,                    // 启用 P2P 服务
    "MaxConnections": 100,              // 最大连接数
    "Port": 8080,                       // 监听端口
    "BufferSize": 8192,                 // 缓冲区大小
    "Timeout": "00:01:00",             // 超时时间
    "EnableEncryption": true,           // 启用加密
    "EnableDiscovery": true,            // 启用节点发现
    "DiscoveryInterval": "00:00:30",   // 发现间隔
    "EnableParallelProcessing": true,   // 启用并行处理
    "MaxDegreeOfParallelism": 4,        // 最大并行度
    "EnableBatching": true,             // 启用批处理
    "BatchSize": 100,                   // 批处理大小
    "EnableCaching": true,              // 启用缓存
    "CacheSize": 1000,                  // 缓存大小
    "CacheDuration": "00:05:00"         // 缓存持续时间
  }
}
```

### 日志配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "P2P": "Debug"  // 设置 P2P 相关日志为 Debug 级别
    }
  }
}
```

## 性能优化

1. **启用缓存**: 启用缓存可以提高节点发现和消息处理的性能
2. **异步编程**: 使用异步 API 避免阻塞主线程
3. **批处理**: 对于大量消息或文件传输，使用批处理提高效率
4. **连接池管理**: 合理设置最大连接数以避免资源耗尽
5. **网络拓扑优化**: 定期优化网络拓扑以提高路由效率
6. **并行处理**: 启用并行处理以充分利用多核 CPU
7. **内存优化**: 使用内存池和零拷贝技术减少内存开销
8. **网络拥塞控制**: 实现网络拥塞控制，提高网络稳定性

## 故障排除

### 常见问题

1. **连接失败**
   - 检查网络连接是否正常
   - 验证目标节点是否在线
   - 检查防火墙设置是否阻止了端口
   - 查看日志文件获取详细错误信息

2. **节点发现失败**
   - 确保启用了节点发现功能
   - 检查网络广播是否被允许
   - 验证发现间隔设置是否合理

3. **文件传输失败**
   - 检查文件路径是否正确
   - 验证文件权限是否足够
   - 确保网络连接稳定
   - 检查磁盘空间是否充足

4. **消息传递失败**
   - 检查网络连接是否正常
   - 验证目标节点是否在线
   - 检查消息大小是否超过限制

5. **性能问题**
   - 启用缓存
   - 增加最大并行度
   - 优化网络拓扑
   - 检查系统资源使用情况

### 日志记录

P2P 技能提供了详细的日志记录，可以帮助诊断问题。在配置文件中设置 P2P 相关日志为 Debug 级别：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "P2P": "Debug"
    }
  }
}
```

## 扩展开发

### 添加自定义功能

```csharp
using P2P;

// 自定义消息处理器
public class CustomMessageHandler
{
    private readonly ILogger<CustomMessageHandler> _logger;

    public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleMessageAsync(P2PMessage message)
    {
        // 自定义消息处理逻辑
        _logger.LogInformation($"Custom handler received message from {message.SenderId}");
        
        // 根据消息内容执行业务逻辑
        if (message.Content.Contains("ping"))
        {
            // 处理 ping 消息
            _logger.LogInformation("Received ping message, sending pong response");
            // 发送响应消息
        }
        else if (message.Content.Contains("query"))
        {
            // 处理查询消息
            _logger.LogInformation("Received query message, processing...");
            // 处理查询逻辑
        }
    }
}

// 注册和使用
var messagingService = serviceProvider.GetRequiredService<IP2PMessagingService>();
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<CustomMessageHandler>();
var handler = new CustomMessageHandler(logger);

await messagingService.SubscribeToMessagesAsync(handler.HandleMessageAsync);
```

### 自定义网络拓扑优化

```csharp
using P2P;

// 自定义网络拓扑优化策略
public class CustomNetworkOptimizer
{
    private readonly IP2PNetworkService _networkService;
    private readonly ILogger<CustomNetworkOptimizer> _logger;

    public CustomNetworkOptimizer(IP2PNetworkService networkService, ILogger<CustomNetworkOptimizer> logger)
    {
        _networkService = networkService;
        _logger = logger;
    }

    public async Task OptimizeAsync()
    {
        // 获取当前拓扑
        var topology = await _networkService.GetNetworkTopologyAsync();
        _logger.LogInformation($"Current network topology: {topology.NodeCount} nodes, {topology.ConnectionCount} connections");
        
        // 自定义优化逻辑
        // 1. 检查网络密度
        var networkDensity = CalculateNetworkDensity(topology);
        _logger.LogInformation($"Network density: {networkDensity:F2}");
        
        // 2. 识别孤立节点
        var isolatedNodes = IdentifyIsolatedNodes(topology);
        if (isolatedNodes.Any())
        {
            _logger.LogWarning($"Found {isolatedNodes.Count} isolated nodes");
            // 处理孤立节点
        }
        
        // 3. 优化连接
        _logger.LogInformation("Optimizing network connections...");
        
        // 应用优化
        await _networkService.OptimizeNetworkTopologyAsync();
        _logger.LogInformation("Network topology optimization completed");
    }

    private double CalculateNetworkDensity(P2PNetworkTopology topology)
    {
        // 计算网络密度
        if (topology.NodeCount < 2)
            return 0;
        
        var possibleConnections = topology.NodeCount * (topology.NodeCount - 1) / 2;
        return (double)topology.ConnectionCount / possibleConnections;
    }

    private List<P2PNode> IdentifyIsolatedNodes(P2PNetworkTopology topology)
    {
        // 识别孤立节点
        var connectedNodeIds = new HashSet<string>(
            topology.Connections.Select(c => c.Node1Id)
            .Concat(topology.Connections.Select(c => c.Node2Id))
        );
        
        return topology.Nodes.Where(n => !connectedNodeIds.Contains(n.Id)).ToList();
    }
}
```

## AOT 编译支持

P2P 技能完全支持 .NET 10 的 AOT 编译，以提高启动速度和运行时性能。

### 编译配置

```csproj
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <PublishAot>true</PublishAot>
    <ReadyToRun>true</ReadyToRun>
    <TieredCompilation>true</TieredCompilation>
    <TrimMode>partial</TrimMode>
    <Optimize>true</Optimize>
    <EnableCompilationRelaxations>true</EnableCompilationRelaxations>
    <EnableEnhancedNgen>true</EnableEnhancedNgen>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <!-- 依赖项 -->
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
    <PackageReference Include="System.Net.Sockets" Version="10.0.0" />
    <PackageReference Include="System.Net.Http" Version="10.0.0" />
    <PackageReference Include="System.Threading.Channels" Version="10.0.0" />
    <PackageReference Include="System.Text.Json" Version="10.0.0" />
    <PackageReference Include="System.Security.Cryptography" Version="10.0.0" />
    <PackageReference Include="System.Collections.Immutable" Version="10.0.0" />
    <PackageReference Include="System.Runtime.CompilerServices.Unsafe" Version="6.0.0" />
    <PackageReference Include="System.Buffers" Version="4.5.1" />
  </ItemGroup>

</Project>
```

### 编译命令

```bash
# 发布为 AOT 编译的可执行文件（Windows）
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true

# 发布为 AOT 编译的可执行文件（Linux）
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true

# 发布为 AOT 编译的可执行文件（macOS）
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true
```

## 部署指南

### 本地部署

1. **编译应用**: 使用上述 AOT 编译命令编译应用
2. **配置文件**: 根据需要修改配置文件
3. **启动应用**: 运行编译生成的可执行文件

### 容器部署

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["P2PApp.csproj", "."]
RUN dotnet restore "P2PApp.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "P2PApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "P2PApp.csproj" -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./P2PApp"]
```

### 部署注意事项

1. **网络配置**: 确保防火墙已开放 P2P 服务使用的端口
2. **资源限制**: 根据服务器资源设置合理的最大连接数和并行度
3. **监控**: 部署监控系统，实时监控 P2P 网络状态
4. **备份**: 定期备份重要数据，尤其是共享文件
5. **安全**: 启用加密和认证，确保网络安全

## 总结

P2P 技能提供了一套完整的点对点网络通信解决方案，基于 .NET 10 构建，支持 AOT 编译，具有高性能、可扩展、安全可靠的特点。通过使用 P2P 技能，开发者可以快速构建分布式应用、文件共享系统、实时通信应用、区块链和分布式存储等需要点对点网络功能的场景。

P2P 技能的核心优势包括：

- **完整的 P2P 功能**：提供节点发现、连接管理、消息传递、文件共享等核心功能
- **高性能设计**：支持 AOT 编译、内存优化、并行处理等性能优化技术
- **可扩展性**：模块化设计，易于扩展和定制
- **安全性**：提供数据加密、身份认证等安全功能
- **可靠性**：实现故障检测和自动恢复机制

如需更多信息，请参考 [SKILL.md](../SKILL.md) 文件和 [examples.md](./examples.md) 文件。