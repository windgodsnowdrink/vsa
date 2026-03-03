# SharpPcap 技能使用示例

## 1. 基础使用

### 1.1 列出网络接口

**功能说明**：列出所有可用的网络接口，包括名称、描述、MAC 地址和 IP 地址。

**命令行使用**：

```bash
sharppcap_core interfaces
```

**输出示例**：

```
可用的网络接口:
  名称: \Device\NPF_{12345678-1234-1234-1234-1234567890AB}
  描述: Intel(R) Ethernet Connection (7) I219-V
  MAC 地址: 00:11:22:33:44:55
  IP 地址: 192.168.1.100

  名称: \Device\NPF_{87654321-4321-4321-4321-BA0987654321}
  描述: Microsoft Wi-Fi Direct Virtual Adapter
  MAC 地址: AA:BB:CC:DD:EE:FF
  IP 地址: 169.254.123.45
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketCaptureService, PacketCaptureService>()
    .BuildServiceProvider();

var captureService = serviceProvider.GetRequiredService<IPacketCaptureService>();
var interfaces = await captureService.GetNetworkInterfacesAsync();

foreach (var iface in interfaces)
{
    Console.WriteLine($"接口: {iface.Name}");
    Console.WriteLine($"描述: {iface.Description}");
    Console.WriteLine($"MAC 地址: {iface.MacAddress}");
    Console.WriteLine($"IP 地址: {iface.IpAddress}");
    Console.WriteLine();
}
```

### 1.2 捕获网络数据包

**功能说明**：在指定的网络接口上捕获数据包，并保存到文件中。

**命令行使用**：

```bash
sharppcap_core capture --interface "\\Device\\NPF_{12345678-1234-1234-1234-1234567890AB}" --filter "tcp port 80" --output capture.pcap --duration 60
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketCaptureService, PacketCaptureService>()
    .BuildServiceProvider();

var captureService = serviceProvider.GetRequiredService<IPacketCaptureService>();

// 开始捕获
await captureService.StartCaptureAsync(
    "\\Device\\NPF_{12345678-1234-1234-1234-1234567890AB}", // 网络接口名称
    "tcp port 80", // 过滤器表达式
    "capture.pcap", // 输出文件路径
    60 // 捕获持续时间（秒）
);

Console.WriteLine("捕获完成，数据已保存到 capture.pcap");
```

### 1.3 分析网络数据包

**功能说明**：分析指定的数据包文件，提取关键信息。

**命令行使用**：

```bash
sharppcap_core analyze --input capture.pcap --protocol tcp
```

**输出示例**：

```
分析结果: 共 120 个数据包
协议分布:
  TCP: 120
前 5 个数据包:
  时间: 2026/1/24 10:00:00, 源: 192.168.1.100, 目标: 104.26.11.229, 协议: TCP, 信息: 54321 -> 80 [SYN]
  时间: 2026/1/24 10:00:00, 源: 104.26.11.229, 目标: 192.168.1.100, 协议: TCP, 信息: 80 -> 54321 [SYN, ACK]
  时间: 2026/1/24 10:00:00, 源: 192.168.1.100, 目标: 104.26.11.229, 协议: TCP, 信息: 54321 -> 80 [ACK]
  时间: 2026/1/24 10:00:00, 源: 192.168.1.100, 目标: 104.26.11.229, 协议: TCP, 信息: 54321 -> 80 [PSH, ACK]
  时间: 2026/1/24 10:00:00, 源: 104.26.11.229, 目标: 192.168.1.100, 协议: TCP, 信息: 80 -> 54321 [ACK]
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>()
    .BuildServiceProvider();

var analyzerService = serviceProvider.GetRequiredService<IPacketAnalyzerService>();

// 分析捕获文件
var result = await analyzerService.AnalyzeFileAsync("capture.pcap", "tcp");

Console.WriteLine($"分析结果: 共 {result.PacketCount} 个数据包");
Console.WriteLine("协议分布:");
foreach (var kvp in result.ProtocolDistribution)
{
    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
}

Console.WriteLine("前 5 个数据包:");
foreach (var packet in result.Packets.Take(5))
{
    Console.WriteLine($"  时间: {packet.Timestamp}, 源: {packet.Source}, 目标: {packet.Destination}, 协议: {packet.Protocol}, 信息: {packet.Info}");
}
```

### 1.4 过滤网络数据包

**功能说明**：过滤指定的数据包文件，只保留符合条件的数据包。

**命令行使用**：

```bash
sharppcap_core filter --input capture.pcap --expression "tcp port 443" --output filtered.pcap
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketFilterService, PacketFilterService>()
    .BuildServiceProvider();

var filterService = serviceProvider.GetRequiredService<IPacketFilterService>();

// 过滤数据包文件
await filterService.FilterFileAsync(
    "capture.pcap", // 输入文件路径
    "tcp port 443", // 过滤表达式
    "filtered.pcap" // 输出文件路径
);

Console.WriteLine("过滤完成，结果已保存到 filtered.pcap");
```

### 1.5 查看网络统计信息

**功能说明**：查看指定网络接口的统计信息，如带宽使用情况、数据包数量等。

**命令行使用**：

```bash
sharppcap_core stats --interface "\\Device\\NPF_{12345678-1234-1234-1234-1234567890AB}" --interval 1
```

**输出示例**：

```
2026/1/24 10:00:00: 接收: 123456 字节, 发送: 78901 字节, 接收包数: 123, 发送包数: 45
2026/1/24 10:00:01: 接收: 234567 字节, 发送: 89012 字节, 接收包数: 234, 发送包数: 56
2026/1/24 10:00:02: 接收: 345678 字节, 发送: 90123 字节, 接收包数: 345, 发送包数: 67
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<INetworkStatsService, NetworkStatsService>()
    .BuildServiceProvider();

var statsService = serviceProvider.GetRequiredService<INetworkStatsService>();

// 开始监控统计信息
await statsService.StartStatsMonitoringAsync(
    "\\Device\\NPF_{12345678-1234-1234-1234-1234567890AB}", // 网络接口名称
    1, // 统计间隔（秒）
    stats => // 回调函数
    {
        Console.WriteLine($"{DateTime.Now}: 接收: {stats.BytesReceived} 字节, 发送: {stats.BytesSent} 字节, 接收包数: {stats.PacketsReceived}, 发送包数: {stats.PacketsSent}");
    }
);

// 运行 10 秒后停止
await Task.Delay(10000);
await statsService.StopStatsMonitoringAsync();

Console.WriteLine("监控停止");
```

## 2. 高级使用

### 2.1 生成网络协议解析器代码

**功能说明**：生成指定协议的解析器代码，简化协议解析的开发工作。

**命令行使用**：

```bash
sharppcap_generator generate --type parser --protocol TCP --output Parsers/TcpParser.cs
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<ICodeGeneratorService, CodeGeneratorService>()
    .BuildServiceProvider();

var codeGeneratorService = serviceProvider.GetRequiredService<ICodeGeneratorService>();

// 生成 TCP 协议解析器代码
var code = await codeGeneratorService.GenerateParserAsync("TCP");

// 保存到文件
await codeGeneratorService.SaveGeneratedCodeAsync(code, "Parsers/TcpParser.cs");

Console.WriteLine("TCP 协议解析器代码生成完成，已保存到 Parsers/TcpParser.cs");
```

### 2.2 生成网络协议分析器代码

**功能说明**：生成指定协议的分析器代码，简化协议分析的开发工作。

**命令行使用**：

```bash
sharppcap_generator generate --type analyzer --protocol UDP --output Analyzers/UdpAnalyzer.cs
```

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<ICodeGeneratorService, CodeGeneratorService>()
    .BuildServiceProvider();

var codeGeneratorService = serviceProvider.GetRequiredService<ICodeGeneratorService>();

// 生成 UDP 协议分析器代码
var code = await codeGeneratorService.GenerateAnalyzerAsync("UDP");

// 保存到文件
await codeGeneratorService.SaveGeneratedCodeAsync(code, "Analyzers/UdpAnalyzer.cs");

Console.WriteLine("UDP 协议分析器代码生成完成，已保存到 Analyzers/UdpAnalyzer.cs");
```

### 2.3 实时数据包处理

**功能说明**：实时捕获和处理网络数据包，无需保存到文件。

**代码使用**：

```csharp
using System;
using System.ServiceProcess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharpPcap;
using PacketDotNet;

// 创建服务容器
var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

// 获取网络接口
var device = CaptureDeviceList.Instance.FirstOrDefault();
if (device == null)
{
    logger.LogError("未找到网络接口");
    return;
}

logger.LogInformation($"使用网络接口: {device.Description}");

// 打开设备
device.Open();

// 设置过滤器
device.Filter = "tcp port 80 or tcp port 443";

// 注册数据包接收事件
device.OnPacketArrival += (sender, e) =>
{
    try
    {
        var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
        var tcpPacket = packet.Extract<TcpPacket>();
        var ipPacket = packet.Extract<IpPacket>();

        if (tcpPacket != null && ipPacket != null)
        {
            logger.LogInformation($"TCP 数据包: {ipPacket.SourceAddress}:{tcpPacket.SourcePort} -> {ipPacket.DestinationAddress}:{tcpPacket.DestinationPort}");
            
            // 处理数据包...
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "处理数据包时发生错误");
    }
};

// 开始捕获
device.StartCapture();

logger.LogInformation("开始实时捕获数据包，按任意键停止...");
Console.ReadKey();

// 停止捕获
device.StopCapture();
device.Close();

logger.LogInformation("捕获停止");
```

### 2.4 多协议分析

**功能说明**：同时分析多种协议的数据包，提取不同协议的关键信息。

**代码使用**：

```csharp
using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>()
    .BuildServiceProvider();

var analyzerService = serviceProvider.GetRequiredService<IPacketAnalyzerService>();

// 分析捕获文件（不指定协议，分析所有协议）
var result = await analyzerService.AnalyzeFileAsync("capture.pcap", string.Empty);

Console.WriteLine($"分析结果: 共 {result.PacketCount} 个数据包");
Console.WriteLine("协议分布:");
foreach (var kvp in result.ProtocolDistribution)
{
    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
}

// 按协议分组显示
foreach (var protocol in result.ProtocolDistribution.Keys)
{
    Console.WriteLine($"\n{protocol} 协议数据包:");
    var protocolPackets = result.Packets.Where(p => p.Protocol.Equals(protocol, StringComparison.OrdinalIgnoreCase)).Take(3);
    foreach (var packet in protocolPackets)
    {
        Console.WriteLine($"  时间: {packet.Timestamp}, 源: {packet.Source}, 目标: {packet.Destination}, 信息: {packet.Info}");
    }
}
```

## 3. 实际应用场景

### 3.1 网络监控系统

**功能说明**：构建一个简单的网络监控系统，实时监控网络流量和异常行为。

**代码示例**：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class NetworkMonitor
{
    private readonly IPacketCaptureService _captureService;
    private readonly IPacketAnalyzerService _analyzerService;
    private readonly INetworkStatsService _statsService;
    private readonly ILogger<NetworkMonitor> _logger;

    public NetworkMonitor(
        IPacketCaptureService captureService,
        IPacketAnalyzerService analyzerService,
        INetworkStatsService statsService,
        ILogger<NetworkMonitor> logger)
    {
        _captureService = captureService;
        _analyzerService = analyzerService;
        _statsService = statsService;
        _logger = logger;
    }

    public async Task StartMonitoringAsync(string interfaceName, int intervalSeconds)
    {
        _logger.LogInformation($"开始监控网络接口: {interfaceName}");

        // 启动统计信息监控
        await _statsService.StartStatsMonitoringAsync(
            interfaceName,
            intervalSeconds,
            stats =>
            {
                _logger.LogInformation($"网络统计: 接收 {stats.BytesReceived} 字节, 发送 {stats.BytesSent} 字节, 接收包数 {stats.PacketsReceived}, 发送包数 {stats.PacketsSent}");
                
                // 检测异常流量
                if (stats.BytesReceived > 10 * 1024 * 1024) // 10MB
                {
                    _logger.LogWarning("警告: 接收流量异常高！");
                }
            }
        );
    }

    public async Task StopMonitoringAsync()
    {
        await _statsService.StopStatsMonitoringAsync();
        _logger.LogInformation("网络监控停止");
    }
}

// 使用示例
var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketCaptureService, PacketCaptureService>()
    .AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>()
    .AddSingleton<INetworkStatsService, NetworkStatsService>()
    .AddSingleton<NetworkMonitor>()
    .BuildServiceProvider();

var monitor = serviceProvider.GetRequiredService<NetworkMonitor>();
var captureService = serviceProvider.GetRequiredService<IPacketCaptureService>();

// 获取网络接口
var interfaces = await captureService.GetNetworkInterfacesAsync();
if (interfaces.Any())
{
    var interfaceName = interfaces.First().Name;
    
    // 开始监控
    await monitor.StartMonitoringAsync(interfaceName, 1);
    
    Console.WriteLine("网络监控已启动，按任意键停止...");
    Console.ReadKey();
    
    // 停止监控
    await monitor.StopMonitoringAsync();
}
else
{
    Console.WriteLine("未找到网络接口");
}
```

### 3.2 网络安全审计

**功能说明**：构建一个简单的网络安全审计工具，检测网络中的异常行为和潜在威胁。

**代码示例**：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PacketDotNet;

class NetworkSecurityAuditor
{
    private readonly IPacketAnalyzerService _analyzerService;
    private readonly ILogger<NetworkSecurityAuditor> _logger;
    private readonly HashSet<string> _knownIpAddresses = new HashSet<string>
    {
        "192.168.1.1",
        "192.168.1.100",
        "192.168.1.101"
    };

    public NetworkSecurityAuditor(
        IPacketAnalyzerService analyzerService,
        ILogger<NetworkSecurityAuditor> logger)
    {
        _analyzerService = analyzerService;
        _logger = logger;
    }

    public async Task AuditAsync(string captureFile)
    {
        _logger.LogInformation($"开始审计网络捕获文件: {captureFile}");

        // 分析捕获文件
        var result = await _analyzerService.AnalyzeFileAsync(captureFile, string.Empty);

        _logger.LogInformation($"审计结果: 共 {result.PacketCount} 个数据包");

        // 检测未知 IP 地址
        var unknownIps = new HashSet<string>();
        foreach (var packet in result.Packets)
        {
            if (!string.IsNullOrEmpty(packet.Source) && !_knownIpAddresses.Contains(packet.Source))
            {
                unknownIps.Add(packet.Source);
            }
            if (!string.IsNullOrEmpty(packet.Destination) && !_knownIpAddresses.Contains(packet.Destination))
            {
                unknownIps.Add(packet.Destination);
            }
        }

        if (unknownIps.Any())
        {
            _logger.LogWarning("检测到未知 IP 地址:");
            foreach (var ip in unknownIps)
            {
                _logger.LogWarning($"  - {ip}");
            }
        }

        // 检测异常协议分布
        var totalPackets = result.PacketCount;
        foreach (var kvp in result.ProtocolDistribution)
        {
            var percentage = (double)kvp.Value / totalPackets * 100;
            if (percentage > 50)
            {
                _logger.LogWarning($"警告: {kvp.Key} 协议占比过高 ({percentage:F2}%)");
            }
        }

        _logger.LogInformation("网络安全审计完成");
    }
}

// 使用示例
var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<IPacketAnalyzerService, PacketAnalyzerService>()
    .AddSingleton<NetworkSecurityAuditor>()
    .BuildServiceProvider();

var auditor = serviceProvider.GetRequiredService<NetworkSecurityAuditor>();

// 审计捕获文件
await auditor.AuditAsync("capture.pcap");
```

### 3.3 网络性能测试

**功能说明**：构建一个简单的网络性能测试工具，测试网络的吞吐量和延迟。

**代码示例**：

```csharp
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class NetworkPerformanceTester
{
    private readonly ILogger<NetworkPerformanceTester> _logger;

    public NetworkPerformanceTester(ILogger<NetworkPerformanceTester> logger)
    {
        _logger = logger;
    }

    public async Task TestThroughputAsync(string targetIp, int port, int packetSize, int packetCount)
    {
        _logger.LogInformation($"开始吞吐量测试: {targetIp}:{port}, 数据包大小: {packetSize} 字节, 数据包数量: {packetCount}");

        var stopwatch = Stopwatch.StartNew();
        int sentCount = 0;
        int receivedCount = 0;

        using var client = new TcpClient();
        await client.ConnectAsync(targetIp, port);
        using var stream = client.GetStream();

        // 发送数据包
        var data = new byte[packetSize];
        new Random().NextBytes(data);

        for (int i = 0; i < packetCount; i++)
        {
            await stream.WriteAsync(data);
            sentCount++;
        }

        // 接收响应
        var buffer = new byte[packetSize];
        while (receivedCount < packetCount)
        {
            var bytesRead = await stream.ReadAsync(buffer);
            if (bytesRead == 0)
            {
                break;
            }
            receivedCount++;
        }

        stopwatch.Stop();

        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        var totalBytes = packetSize * sentCount;
        var throughput = totalBytes / elapsedSeconds / 1024 / 1024; // MB/s

        _logger.LogInformation($"吞吐量测试完成:");
        _logger.LogInformation($"  发送数据包: {sentCount}");
        _logger.LogInformation($"  接收数据包: {receivedCount}");
        _logger.LogInformation($"  耗时: {elapsedSeconds:F2} 秒");
        _logger.LogInformation($"  吞吐量: {throughput:F2} MB/s");
    }

    public async Task TestLatencyAsync(string targetIp, int port, int testCount)
    {
        _logger.LogInformation($"开始延迟测试: {targetIp}:{port}, 测试次数: {testCount}");

        var totalLatency = 0L;
        int successfulTests = 0;

        for (int i = 0; i < testCount; i++)
        {
            try
            {
                using var client = new TcpClient();
                var connectStart = DateTime.Now;
                await client.ConnectAsync(targetIp, port);
                var connectEnd = DateTime.Now;
                
                var latency = (connectEnd - connectStart).TotalMilliseconds;
                totalLatency += (long)latency;
                successfulTests++;

                _logger.LogInformation($"  测试 {i + 1}: {latency:F2} ms");

                client.Close();
                await Task.Delay(100); // 避免过快的连接
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"测试 {i + 1} 失败");
            }
        }

        if (successfulTests > 0)
        {
            var averageLatency = (double)totalLatency / successfulTests;
            _logger.LogInformation($"延迟测试完成:");
            _logger.LogInformation($"  成功测试: {successfulTests}");
            _logger.LogInformation($"  平均延迟: {averageLatency:F2} ms");
        }
        else
        {
            _logger.LogError("所有延迟测试都失败了");
        }
    }
}

// 使用示例
var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<NetworkPerformanceTester>()
    .BuildServiceProvider();

var tester = serviceProvider.GetRequiredService<NetworkPerformanceTester>();

// 测试吞吐量
await tester.TestThroughputAsync("127.0.0.1", 8080, 1024, 1000);

// 测试延迟
await tester.TestLatencyAsync("127.0.0.1", 8080, 10);
```

### 3.4 网络流量分析仪表板

**功能说明**：构建一个简单的网络流量分析仪表板，展示网络流量的实时统计信息和趋势。

**代码示例**：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class NetworkDashboard
{
    private readonly INetworkStatsService _statsService;
    private readonly ILogger<NetworkDashboard> _logger;
    private readonly List<NetworkStats> _statsHistory = new List<NetworkStats>();
    private readonly object _lock = new object();

    public NetworkDashboard(
        INetworkStatsService statsService,
        ILogger<NetworkDashboard> logger)
    {
        _statsService = statsService;
        _logger = logger;
    }

    public async Task StartAsync(string interfaceName, int intervalSeconds)
    {
        _logger.LogInformation($"启动网络仪表板，监控接口: {interfaceName}");

        // 启动统计信息监控
        await _statsService.StartStatsMonitoringAsync(
            interfaceName,
            intervalSeconds,
            stats =>
            {
                lock (_lock)
                {
                    _statsHistory.Add(stats);
                    // 只保留最近 60 个数据点
                    if (_statsHistory.Count > 60)
                    {
                        _statsHistory.RemoveAt(0);
                    }
                }

                // 显示实时统计信息
                DisplayStats(stats);
                
                // 显示趋势
                DisplayTrend();
            }
        );
    }

    public async Task StopAsync()
    {
        await _statsService.StopStatsMonitoringAsync();
        _logger.LogInformation("网络仪表板停止");
    }

    private void DisplayStats(NetworkStats stats)
    {
        Console.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine($"网络仪表板 - {DateTime.Now}");
        Console.WriteLine("========================================");
        Console.WriteLine($"接收字节: {stats.BytesReceived:n0}");
        Console.WriteLine($"发送字节: {stats.BytesSent:n0}");
        Console.WriteLine($"接收包数: {stats.PacketsReceived:n0}");
        Console.WriteLine($"发送包数: {stats.PacketsSent:n0}");
        Console.WriteLine($"接收错误: {stats.ErrorsReceived:n0}");
        Console.WriteLine($"发送错误: {stats.ErrorsSent:n0}");
        Console.WriteLine("========================================");
    }

    private void DisplayTrend()
    {
        Console.WriteLine("流量趋势 (最近 60 秒):");
        Console.WriteLine("========================================");

        lock (_lock)
        {
            if (_statsHistory.Count > 1)
            {
                // 计算每秒的流量变化
                for (int i = 1; i < _statsHistory.Count; i++)
                {
                    var current = _statsHistory[i];
                    var previous = _statsHistory[i - 1];
                    
                    var bytesReceivedPerSec = current.BytesReceived - previous.BytesReceived;
                    var bytesSentPerSec = current.BytesSent - previous.BytesSent;
                    
                    Console.WriteLine($"秒 {i}: 接收 {bytesReceivedPerSec:n0} B/s, 发送 {bytesSentPerSec:n0} B/s");
                }
            }
        }

        Console.WriteLine("========================================");
        Console.WriteLine("按任意键停止...");
    }
}

// 使用示例
var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddConsole())
    .AddSingleton<INetworkStatsService, NetworkStatsService>()
    .AddSingleton<NetworkDashboard>()
    .AddSingleton<IPacketCaptureService, PacketCaptureService>()
    .BuildServiceProvider();

var dashboard = serviceProvider.GetRequiredService<NetworkDashboard>();
var captureService = serviceProvider.GetRequiredService<IPacketCaptureService>();

// 获取网络接口
var interfaces = await captureService.GetNetworkInterfacesAsync();
if (interfaces.Any())
{
    var interfaceName = interfaces.First().Name;
    
    // 启动仪表板
    await dashboard.StartAsync(interfaceName, 1);
    
    Console.ReadKey();
    
    // 停止仪表板
    await dashboard.StopAsync();
}
else
{
    Console.WriteLine("未找到网络接口");
}
```

## 4. 常见问题解决方案

### 4.1 网络接口不可用

**问题描述**：运行 `interfaces` 命令时，未列出任何网络接口或列出的接口不可用。

**解决方案**：

1. **检查权限**：以管理员权限运行命令行工具
2. **检查驱动程序**：确保网络适配器驱动程序已正确安装
3. **检查 SharpPcap 版本**：确保使用与操作系统兼容的 SharpPcap 版本
4. **检查网络连接**：确保网络适配器已启用并连接到网络

### 4.2 捕获数据包时出现错误

**问题描述**：运行 `capture` 命令时，出现 "无法打开网络接口" 或 "权限不足" 等错误。

**解决方案**：

1. **以管理员权限运行**：网络数据包捕获需要管理员权限
2. **检查接口名称**：确保使用正确的网络接口名称
3. **检查接口状态**：确保网络接口已启用且未被其他程序占用
4. **检查过滤器语法**：确保使用正确的 BPF 过滤器语法

### 4.3 分析大型捕获文件时内存不足

**问题描述**：分析大型捕获文件时，出现 "内存不足" 或 "OutOfMemoryException" 错误。

**解决方案**：

1. **使用过滤器**：在捕获时使用过滤器，减少需要分析的数据包数量
2. **分段分析**：将大型文件分成多个小文件进行分析
3. **增加内存限制**：在 64 位系统上运行，增加应用程序的内存限制
4. **使用流式处理**：修改代码，使用流式处理减少内存使用

### 4.4 生成的代码无法编译

**问题描述**：使用 `generate` 命令生成的代码无法编译，出现语法错误或缺少依赖项。

**解决方案**：

1. **检查协议名称**：确保使用正确的协议名称（如 TCP、UDP 等）
2. **添加依赖项**：确保项目中已添加必要的依赖项，如 SharpPcap 和 PacketDotNet
3. **检查命名空间**：确保生成的代码的命名空间与项目结构一致
4. **手动修复**：根据编译错误信息，手动修复生成的代码

### 4.5 网络统计信息不准确

**问题描述**：运行 `stats` 命令时，显示的网络统计信息不准确或为零。

**解决方案**：

1. **检查接口名称**：确保使用正确的网络接口名称
2. **检查接口状态**：确保网络接口已启用并连接到网络
3. **检查权限**：以管理员权限运行命令行工具
4. **等待一段时间**：统计信息需要一定时间才能累积，等待一段时间后再查看

## 5. 性能优化技巧

### 5.1 捕获性能优化

1. **使用精确的过滤器**：使用更精确的 BPF 过滤器表达式，减少需要处理的数据包数量
   ```bash
   # 只捕获 HTTP 和 HTTPS 流量
   sharppcap_core capture --interface "\\Device\\NPF_{12345678-1234-1234-1234-1234567890AB}" --filter "tcp port 80 or tcp port 443" --output capture.pcap --duration 60
   ```

2. **调整捕获缓冲区大小**：根据网络流量调整捕获缓冲区大小，避免丢包
   ```csharp
   // 在代码中调整缓冲区大小
device.BufferSize = 10 * 1024 * 1024; // 10MB
   ```

3. **使用文件缓冲**：对于大量数据，使用文件缓冲减少内存使用
   ```csharp
   // 使用 FileStream 进行缓冲
   using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, FileOptions.SequentialScan);
   ```

### 5.2 分析性能优化

1. **增量分析**：对于大型捕获文件，使用增量分析减少内存使用
   ```csharp
   // 增量分析示例
   public async Task IncrementalAnalyzeAsync(string filePath, int batchSize)
   {
       // 打开捕获文件
       using var device = new CaptureFileReaderDevice(filePath);
       device.Open();

       var batchCount = 0;
       var packetCount = 0;

       // 批量处理数据包
       device.OnPacketArrival += (sender, e) =>
       {
           // 处理数据包...
           packetCount++;

           if (packetCount % batchSize == 0)
           {
               batchCount++;
               Console.WriteLine($"处理批次 {batchCount}, 已处理 {packetCount} 个数据包");
           }
       };

       // 开始分析
       device.Capture();

       Console.WriteLine($"分析完成，共处理 {packetCount} 个数据包");
   }
   ```

2. **并行分析**：对于多个文件，使用并行分析提高速度
   ```csharp
   // 并行分析多个文件
   public async Task ParallelAnalyzeAsync(IEnumerable<string> filePaths)
   {
       var tasks = filePaths.Select(filePath => AnalyzeFileAsync(filePath, string.Empty));
       var results = await Task.WhenAll(tasks);

       // 汇总结果...
   }
   ```

### 5.3 内存优化

1. **使用 Span<T>**：对于数据包处理，使用 Span<T> 减少内存分配
   ```csharp
   // 使用 Span<T> 处理数据包
   public void ProcessPacket(Span<byte> packetData)
   {
       // 处理数据包...
   }
   ```

2. **对象池**：使用对象池重用对象，减少 GC 压力
   ```csharp
   // 使用对象池
   private readonly ObjectPool<PacketInfo> _packetInfoPool = ObjectPool.Create<PacketInfo>();

   public void ProcessPacket(Packet packet)
   {
       var packetInfo = _packetInfoPool.Get();
       
       // 处理数据包...
       
       _packetInfoPool.Return(packetInfo);
   }
   ```

3. **流式处理**：对于大型文件，使用流式处理减少内存使用
   ```csharp
   // 流式处理示例
   public async Task StreamProcessAsync(string filePath)
   {
       using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
       using var reader = new BinaryReader(stream);
       
       // 流式读取和处理...
   }
   ```

### 5.4 代码生成优化

1. **模板缓存**：缓存代码生成模板，减少重复解析
   ```csharp
   // 模板缓存
   private readonly Dictionary<string, string> _templateCache = new Dictionary<string, string>();

   public string GetTemplate(string templateName)
   {
       if (!_templateCache.TryGetValue(templateName, out var template))
       {
           template = LoadTemplate(templateName);
           _templateCache[templateName] = template;
       }
       return template;
   }
   ```

2. **并行生成**：对于多个协议，使用并行生成提高速度
   ```csharp
   // 并行生成多个协议的代码
   public async Task ParallelGenerateAsync(IEnumerable<string> protocols)
   {
       var tasks = protocols.Select(async protocol =>
       {
           var parserCode = await GenerateParserAsync(protocol);
           var analyzerCode = await GenerateAnalyzerAsync(protocol);
           return (protocol, parserCode, analyzerCode);
       });

       var results = await Task.WhenAll(tasks);

       // 保存结果...
   }
   ```

## 6. 总结

SharpPcap 技能是一个功能强大的网络数据包捕获和分析工具，基于 .NET 10 和 AOT 编译技术，提供高效的网络数据处理能力。通过本示例文档，您可以了解如何使用 SharpPcap 技能的各种功能，包括：

1. **基础功能**：列出网络接口、捕获数据包、分析数据包、过滤数据包、查看网络统计信息
2. **高级功能**：生成网络协议解析器和分析器代码、实时数据包处理、多协议分析
3. **实际应用**：网络监控系统、网络安全审计、网络性能测试、网络流量分析仪表板
4. **故障排除**：解决常见问题，如网络接口不可用、捕获错误、内存不足等
5. **性能优化**：优化捕获性能、分析性能、内存使用和代码生成

通过合理使用 SharpPcap 技能，您可以构建各种网络相关的应用程序，如网络监控工具、安全审计工具、性能测试工具等。同时，SharpPcap 技能的模块化设计和可扩展架构也使得您可以根据具体需求进行定制和扩展。

希望本示例文档对您使用 SharpPcap 技能有所帮助！
