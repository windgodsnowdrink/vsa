# sharppcap Agent Skill - sharppcap 技能

## 技能概述

基于 .NET 10 的高性能 sharppcap 技能，为 .NET 开发者提供强大的网络数据包捕获和分析功能。该技能采用 AOT 编译优化，提供实时、高效的网络数据处理能力。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package SharpPcap@6.3.0
#:package PacketDotNet@1.4.7
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.CommandLine@2.0.0
#:package System.Text.Json@10.0.0
```

### 注册服务

在主应用程序中注册 sharppcap 服务：

```csharp
// 注册 sharppcap 服务
builder.Services.AddSingleton<IPacketCaptureService, PacketCaptureService>();
builder.Services.AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>();
builder.Services.AddSingleton<IPacketFilterService, PacketFilterService>();
builder.Services.AddSingleton<INetworkStatsService, NetworkStatsService>();
builder.Services.AddSingleton<ICodeGeneratorService, CodeGeneratorService>();
```

### 使用示例

```csharp
// 获取网络接口列表
var captureService = serviceProvider.GetRequiredService<IPacketCaptureService>();
var interfaces = await captureService.GetNetworkInterfacesAsync();

// 选择第一个网络接口进行捕获
if (interfaces.Any())
{
    var targetInterface = interfaces.First();
    Console.WriteLine($"开始在接口 {targetInterface.Name} 上捕获数据包...");
    
    // 开始捕获
    await captureService.StartCaptureAsync(targetInterface.Name, "tcp port 80", "capture.pcap", 60);
    Console.WriteLine("捕获完成！");
}
```

## 导航地图

```
sharppcap/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── sharppcap_core.cs      # sharppcap 核心实现
    ├── sharppcap_generator.cs # sharppcap 代码生成实现
    ├── sharppcap_core.setting.json  # 编译配置
    ├── sharppcap_core.run.json  # 运行配置
    ├── sharppcap_generator.setting.json  # 编译配置
    └── sharppcap_generator.run.json  # 运行配置
```

## 核心功能

1. **网络数据包捕获**：支持从指定网络接口实时捕获数据包，可设置过滤器表达式和捕获持续时间
2. **网络数据包分析**：支持分析已捕获的数据包，识别协议类型，提取关键信息
3. **网络数据包过滤**：支持使用 BPF 过滤器表达式过滤数据包，减少处理量
4. **网络统计信息**：支持收集网络接口的统计信息，如带宽使用情况、数据包数量等
5. **代码生成**：支持生成网络协议解析器和分析器的代码，简化开发过程
6. **高性能设计**：采用 AOT 编译优化，提供实时、高效的网络数据处理能力
7. **易于使用的 API**：提供简单直观的 API 设计，降低使用门槛
8. **可扩展架构**：支持自定义扩展，适应不同场景的需求

## API 参考

### IPacketCaptureService

```csharp
public interface IPacketCaptureService
{
    Task<IEnumerable<NetworkInterfaceInfo>> GetNetworkInterfacesAsync();
    Task StartCaptureAsync(string interfaceName, string filter, string outputPath, int durationSeconds);
    Task StopCaptureAsync();
}
```

### IPacketAnalyzerService

```csharp
public interface IPacketAnalyzerService
{
    Task<PacketAnalysisResult> AnalyzeFileAsync(string filePath, string protocol);
    Task<PacketAnalysisResult> AnalyzePacketAsync(byte[] packetData);
}
```

### IPacketFilterService

```csharp
public interface IPacketFilterService
{
    Task FilterFileAsync(string inputPath, string filterExpression, string outputPath);
    Task<byte[]> FilterPacketAsync(byte[] packetData, string filterExpression);
}
```

### INetworkStatsService

```csharp
public interface INetworkStatsService
{
    Task<NetworkStats> GetStatsAsync(string interfaceName);
    Task StartStatsMonitoringAsync(string interfaceName, int intervalSeconds, Action<NetworkStats> callback);
    Task StopStatsMonitoringAsync();
}
```

### ICodeGeneratorService

```csharp
public interface ICodeGeneratorService
{
    Task<string> GenerateParserAsync(string protocol);
    Task<string> GenerateAnalyzerAsync(string protocol);
    Task SaveGeneratedCodeAsync(string code, string outputPath);
}
```

## AOT 编译

该技能采用 AOT（Ahead-of-Time）编译技术，提供以下优势：

1. **启动速度快**：预编译代码，减少运行时 JIT 编译开销
2. **运行时性能高**：优化的机器代码执行效率更高
3. **内存占用小**：通过裁剪未使用的代码，减少应用程序体积
4. **部署简单**：生成单个可执行文件，无需安装 .NET 运行时

### AOT 编译配置

```json
{
  "compilationOptions": {
    "targetFramework": "net11.0",
    "langVersion": "preview",
    "nullable": true,
    "implicitUsings": true
  },
  "publishOptions": {
    "publishAot": true,
    "trimMode": "partial",
    "selfContained": true,
    "publishSingleFile": true,
    "runtimeIdentifier": "win-x64"
  }
}
```

## 使用示例

### 示例 1：基本数据包捕获

```csharp
using var serviceProvider = new ServiceCollection()
    .AddSingleton<IPacketCaptureService, PacketCaptureService>()
    .BuildServiceProvider();

var captureService = serviceProvider.GetRequiredService<IPacketCaptureService>();

// 获取网络接口
var interfaces = await captureService.GetNetworkInterfacesAsync();
foreach (var iface in interfaces)
{
    Console.WriteLine($"接口: {iface.Name}, 描述: {iface.Description}, MAC: {iface.MacAddress}");
}

// 开始捕获
if (interfaces.Any())
{
    var ifaceName = interfaces.First().Name;
    await captureService.StartCaptureAsync(ifaceName, "tcp", "capture.pcap", 10);
    Console.WriteLine("捕获完成，数据已保存到 capture.pcap");
}
```

### 示例 2：数据包分析

```csharp
using var serviceProvider = new ServiceCollection()
    .AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>()
    .BuildServiceProvider();

var analyzerService = serviceProvider.GetRequiredService<IPacketAnalyzerService>();

// 分析捕获文件
var result = await analyzerService.AnalyzeFileAsync("capture.pcap", "tcp");
Console.WriteLine($"分析结果: 共 {result.PacketCount} 个数据包");
foreach (var packet in result.Packets.Take(5))
{
    Console.WriteLine($"  时间: {packet.Timestamp}, 源: {packet.Source}, 目标: {packet.Destination}, 长度: {packet.Length}");
}
```

### 示例 3：网络统计信息

```csharp
using var serviceProvider = new ServiceCollection()
    .AddSingleton<INetworkStatsService, NetworkStatsService>()
    .BuildServiceProvider();

var statsService = serviceProvider.GetRequiredService<INetworkStatsService>();

// 获取网络接口统计信息
var interfaces = await captureService.GetNetworkInterfacesAsync();
if (interfaces.Any())
{
    var ifaceName = interfaces.First().Name;
    await statsService.StartStatsMonitoringAsync(ifaceName, 1, stats =>
    {
        Console.WriteLine($"{DateTime.Now}: 接收: {stats.BytesReceived} 字节, 发送: {stats.BytesSent} 字节");
    });
    
    // 运行 10 秒后停止
    await Task.Delay(10000);
    await statsService.StopStatsMonitoringAsync();
}
```

## 扩展说明

该技能提供了完整的网络数据包处理解决方案，您可以根据需要进行扩展：

1. **自定义数据包处理**：实现自定义的数据包处理器，处理特定类型的数据包
2. **扩展协议支持**：添加对新协议的支持，如自定义应用层协议
3. **集成其他系统**：与监控系统、安全系统等集成，提供更全面的网络分析能力
4. **性能优化**：针对特定场景优化性能，如高速网络环境下的数据包处理

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，如网络接口不可用、文件读写失败等
4. **日志记录**：添加适当的日志记录，便于排查问题
5. **性能监控**：监控关键性能指标，如数据包处理速度、内存使用情况等
6. **资源管理**：正确释放资源，如网络接口、文件句柄等
7. **过滤器使用**：合理使用过滤器表达式，减少需要处理的数据包数量
8. **数据存储**：对于大量捕获数据，考虑使用压缩或分段存储，避免内存溢出

## 故障排除

1. **网络接口不可用**：检查网络接口是否存在，是否有权限访问
2. **过滤器表达式错误**：确保使用正确的 BPF 过滤器语法
3. **文件写入失败**：检查输出路径是否存在，是否有写入权限
4. **内存占用过高**：对于大量数据包，考虑使用流式处理或增加内存限制
5. **性能问题**：调整捕获缓冲区大小，使用更精确的过滤器表达式
6. **权限不足**：在某些系统上，需要管理员权限才能访问网络接口
7. **兼容性问题**：确保使用与操作系统兼容的 SharpPcap 版本

## 常见问题

### Q: 如何获取所有可用的网络接口？
A: 使用 `IPacketCaptureService.GetNetworkInterfacesAsync()` 方法获取网络接口列表。

### Q: 如何设置数据包过滤器？
A: 使用 BPF（Berkeley Packet Filter）语法设置过滤器表达式，如 "tcp port 80" 只捕获 TCP 端口 80 的数据包。

### Q: 如何分析捕获的数据包？
A: 使用 `IPacketAnalyzerService.AnalyzeFileAsync()` 方法分析捕获文件，或使用 `AnalyzePacketAsync()` 方法分析单个数据包。

### Q: 如何生成网络协议解析器代码？
A: 使用 `ICodeGeneratorService.GenerateParserAsync()` 方法生成指定协议的解析器代码。

### Q: 如何监控网络接口的统计信息？
A: 使用 `INetworkStatsService.StartStatsMonitoringAsync()` 方法启动统计监控，设置回调函数接收统计数据。
