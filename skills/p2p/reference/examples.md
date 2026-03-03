# P2P 技能 - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 基本使用示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 注册 P2P 服务
        services.AddP2PServices(options =>
        {
            options.Enabled = true;
            options.MaxConnections = 100;
            options.Port = 8080;
            options.EnableEncryption = true;
            options.EnableDiscovery = true;
        });

        using var serviceProvider = services.BuildServiceProvider();

        // 获取 P2P 服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            logger.LogInformation("初始化 P2P 服务...");
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 发现节点
            logger.LogInformation("发现节点...");
            var nodes = await p2pService.DiscoverNodesAsync();
            logger.LogInformation($"发现 {nodes.Count()} 个节点");

            // 连接到节点
            foreach (var node in nodes)
            {
                logger.LogInformation($"连接到节点: {node.Id} ({node.Address}:{node.Port})");
                var connected = await p2pService.ConnectAsync(node.Id);
                logger.LogInformation($"连接结果: {connected}");
            }

            // 获取网络状态
            var status = await p2pService.GetNetworkStatusAsync();
            logger.LogInformation($"网络状态: {status.Status}");
            logger.LogInformation($"连接数: {status.ConnectionCount}");
            logger.LogInformation($"网络健康度: {status.NetworkHealth}%");

            // 模拟运行
            logger.LogInformation("P2P 服务运行中...");
            await Task.Delay(5000);

        }catch (Exception ex)
        {
            logger.LogError(ex, "P2P 服务操作失败");
        }finally
        {
            // 关闭服务
            logger.LogInformation("关闭 P2P 服务...");
            await p2pService.ShutdownAsync();
            logger.LogInformation("P2P 服务已关闭");
        }

        Console.WriteLine("\n示例运行完成，按任意键退出...");
        Console.ReadKey();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 高级配置示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        
        // 注册 P2P 服务（高级配置）
        services.AddP2PServices(options =>
        {
            // 基本配置
            options.Enabled = true;
            options.MaxConnections = 200;
            options.Port = 8081;
            options.BufferSize = 16384;
            options.Timeout = TimeSpan.FromMinutes(2);
            
            // 安全配置
            options.EnableEncryption = true;
            
            // 发现配置
            options.EnableDiscovery = true;
            options.DiscoveryInterval = TimeSpan.FromSeconds(15);
            
            // 性能配置
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;
            options.EnableBatching = true;
            options.BatchSize = 200;
            
            // 缓存配置
            options.EnableCaching = true;
            options.CacheSize = 5000;
            options.CacheDuration = TimeSpan.FromMinutes(10);
        });

        using var serviceProvider = services.BuildServiceProvider();

        // 获取配置
        var p2pOptions = serviceProvider.GetRequiredService<IOptions<P2POptions>>().Value;
        Console.WriteLine($"P2P 配置:");
        Console.WriteLine($"  启用状态: {p2pOptions.Enabled}");
        Console.WriteLine($"  最大连接数: {p2pOptions.MaxConnections}");
        Console.WriteLine($"  端口: {p2pOptions.Port}");
        Console.WriteLine($"  启用加密: {p2pOptions.EnableEncryption}");
        Console.WriteLine($"  启用发现: {p2pOptions.EnableDiscovery}");
        Console.WriteLine($"  最大并行度: {p2pOptions.MaxDegreeOfParallelism}");

        // 获取服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 模拟操作
            await Task.Delay(3000);

        }catch (Exception ex)
        {
            logger.LogError(ex, "操作失败");
        }finally
        {
            // 关闭服务
            await p2pService.ShutdownAsync();
        }

        Console.WriteLine("\n示例运行完成，按任意键退出...");
        Console.ReadKey();
    }
}
```

### 3. 文件共享示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 文件共享示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddP2PServices();

        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var fileSharingService = serviceProvider.GetRequiredService<IP2PFileSharingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 创建测试文件
            var testFile = System.IO.Path.Combine(Environment.CurrentDirectory, "test_file.txt");
            System.IO.File.WriteAllText(testFile, "这是一个测试文件，用于 P2P 文件共享示例。\n包含多行内容以测试文件传输功能。");
            logger.LogInformation($"创建测试文件: {testFile}");

            // 分享文件
            logger.LogInformation("分享文件...");
            await fileSharingService.ShareFileAsync(testFile);
            logger.LogInformation("文件分享成功");

            // 获取已分享的文件
            logger.LogInformation("获取已分享的文件...");
            var sharedFiles = await fileSharingService.GetSharedFilesAsync();
            
            foreach (var file in sharedFiles)
            {
                logger.LogInformation($"文件: {file.Name}");
                logger.LogInformation($"  ID: {file.Id}");
                logger.LogInformation($"  大小: {file.Size} 字节");
                logger.LogInformation($"  哈希: {file.Hash}");
                logger.LogInformation($"  路径: {file.Path}");
            }

            // 模拟下载文件
            if (sharedFiles.Any())
            {
                var fileToDownload = sharedFiles.First();
                var downloadPath = System.IO.Path.Combine(Environment.CurrentDirectory, "downloaded");
                System.IO.Directory.CreateDirectory(downloadPath);
                
                logger.LogInformation($"下载文件: {fileToDownload.Name}");
                var transferId = await fileSharingService.DownloadFileAsync(
                    fileToDownload.Id, 
                    System.IO.Path.Combine(downloadPath, fileToDownload.Name)
                );
                
                // 获取传输状态
                var transferStatus = await fileSharingService.GetTransferStatusAsync(transferId);
                logger.LogInformation($"传输状态: {transferStatus.Status}");
                logger.LogInformation($"传输进度: {transferStatus.Progress}%");
                logger.LogInformation($"传输大小: {transferStatus.TransferredSize}/{transferStatus.TotalSize} 字节");
            }

        }catch (Exception ex)
        {
            logger.LogError(ex, "文件共享操作失败");
        }finally
        {
            // 关闭服务
            await p2pService.ShutdownAsync();
            logger.LogInformation("P2P 服务已关闭");
        }

        Console.WriteLine("\n示例运行完成，按任意键退出...");
        Console.ReadKey();
    }
}
```

### 4. 消息传递示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 消息传递示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddP2PServices();

        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var messagingService = serviceProvider.GetRequiredService<IP2PMessagingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 订阅消息
            logger.LogInformation("订阅消息...");
            await messagingService.SubscribeToMessagesAsync(async message =>
            {
                logger.LogInformation($"接收到消息:");
                logger.LogInformation($"  发送者: {message.SenderId}");
                logger.LogInformation($"  内容: {message.Content}");
                logger.LogInformation($"  状态: {message.Status}");
            });

            // 模拟发送消息到本地节点
            logger.LogInformation("发送测试消息...");
            var messageId = await messagingService.SendMessageAsync(
                "local", 
                "Hello P2P! 这是一条测试消息。"
            );
            logger.LogInformation($"消息发送成功，ID: {messageId}");

            // 获取消息
            logger.LogInformation("获取消息...");
            var message = await messagingService.GetMessageAsync(messageId);
            logger.LogInformation($"获取到消息: {message.Content}");
            logger.LogInformation($"消息状态: {message.Status}");

            // 发送二进制消息
            logger.LogInformation("发送二进制消息...");
            var binaryData = System.Text.Encoding.UTF8.GetBytes("这是二进制消息内容");
            var binaryMessageId = await messagingService.SendMessageAsync("local", binaryData);
            logger.LogInformation($"二进制消息发送成功，ID: {binaryMessageId}");

            // 模拟等待消息处理
            logger.LogInformation("等待消息处理...");
            await Task.Delay(2000);

        }catch (Exception ex)
        {
            logger.LogError(ex, "消息传递操作失败");
        }finally
        {
            // 关闭服务
            await p2pService.ShutdownAsync();
            logger.LogInformation("P2P 服务已关闭");
        }

        Console.WriteLine("\n示例运行完成，按任意键退出...");
        Console.ReadKey();
    }
}
```

### 5. 网络拓扑示例

```csharp
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 网络拓扑示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddP2PServices();

        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var networkService = serviceProvider.GetRequiredService<IP2PNetworkService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 获取本地节点
            var localNode = await networkService.GetLocalNodeAsync();
            logger.LogInformation($"本地节点: {localNode.Id} ({localNode.Address}:{localNode.Port})");

            // 添加模拟节点
            logger.LogInformation("添加模拟节点...");
            for (int i = 1; i <= 3; i++)
            {
                var node = new P2PNode
                {
                    Id = $"node-{i}",
                    Name = $"Node {i}",
                    Address = IPAddress.Parse($"192.168.1.{100 + i}"),
                    Port = 8080 + i
                };
                
                var added = await networkService.AddNodeAsync(node);
                logger.LogInformation($"添加节点 {node.Id}: {added}");
            }

            // 获取网络拓扑
            logger.LogInformation("获取网络拓扑...");
            var topology = await networkService.GetNetworkTopologyAsync();
            logger.LogInformation($"网络节点数: {topology.NodeCount}");
            logger.LogInformation($"网络连接数: {topology.ConnectionCount}");

            // 优化网络拓扑
            logger.LogInformation("优化网络拓扑...");
            await networkService.OptimizeNetworkTopologyAsync();
            logger.LogInformation("网络拓扑优化完成");

            // 获取网络大小
            var networkSize = await networkService.GetNetworkSizeAsync();
            logger.LogInformation($"网络大小: {networkSize} 个节点");

        }catch (Exception ex)
        {
            logger.LogError(ex, "网络拓扑操作失败");
        }finally
        {
            // 关闭服务
            await p2pService.ShutdownAsync();
            logger.LogInformation("P2P 服务已关闭");
        }

        Console.WriteLine("\n示例运行完成，按任意键退出...");
        Console.ReadKey();
    }
}
```

### 6. 安全服务示例

```csharp
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 安全服务示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        services.AddP2PServices();

        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var securityService = serviceProvider.GetRequiredService<IP2PSecurityService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 生成密钥对
            logger.LogInformation("生成密钥对...");
            var keyPair = await securityService.GenerateKeyPairAsync();
            logger.LogInformation($"密钥对生成成功: {keyPair.Substring(0, 20)}...");

            // 准备测试数据
            var testData = System.Text.Encoding.UTF8.GetBytes("这是一条需要加密的数据，用于测试 P2P 安全服务。");
            logger.LogInformation($"测试数据长度: {testData.Length} 字节");

            // 加密数据
            logger.LogInformation("加密数据...");
            var encryptedData = await securityService.EncryptAsync(testData, "test-node");
            logger.LogInformation($"数据加密成功，加密后长度: {encryptedData.Length} 字节");

            // 解密数据
            logger.LogInformation("解密数据...");
            var decryptedData = await securityService.DecryptAsync(encryptedData, "test-node");
            var decryptedText = System.Text.Encoding.UTF8.GetString(decryptedData);
            logger.LogInformation($"数据解密成功: {decryptedText}");

            // 签名数据
            logger.LogInformation("签名数据...");
            var signature = await securityService.SignDataAsync(testData);
            logger.LogInformation($"数据签名成功，签名长度: {signature.Length} 字节");

            // 验证签名
            logger.LogInformation("验证签名...");
            var signatureValid = await securityService.VerifySignatureAsync(testData, signature, "test-node");
            logger.LogInformation($"签名验证结果: {signatureValid}");

            // 认证节点
            logger.LogInformation("认证节点...");
            var node = new P2PNode { Id = "test-node", Address = IPAddress.Loopback, Port = 8080 };
            var authenticated = await securityService.AuthenticateNodeAsync(node);
            logger.LogInformation($"节点认证结果: {authenticated}");

        }catch (Exception ex)
        {
            logger.LogError(ex, "安全服务操作失败");
        }finally
        {
            // 关闭服务
            await p2pService.ShutdownAsync();
            logger.LogInformation("P2P 服务已关闭");
        }

        Console.WriteLine("\n示例运行完成，按任意键退出...");
        Console.ReadKey();
    }
}
```

### 7. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P2P;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("P2P 性能优化示例");
        Console.WriteLine("=" * 50);

        // 构建服务容器
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
        
        // 配置高性能选项
        services.AddP2PServices(options =>
        {
            options.Enabled = true;
            options.EnableCache = true;
            options.CacheSize = 10000;
            options.EnableParallelProcessing = true;
            options.MaxDegreeOfParallelism = Environment.ProcessorCount * 2;
            options.EnableBatching = true;
            options.BatchSize = 500;
        });

        using var serviceProvider = services.BuildServiceProvider();

        // 获取服务
        var p2pService = serviceProvider.GetRequiredService<IP2PService>();
        var messagingService = serviceProvider.GetRequiredService<IP2PMessagingService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // 初始化服务
            await p2pService.InitializeAsync();
            logger.LogInformation("P2P 服务初始化成功");

            // 性能测试: 消息发送
            const int messageCount = 1000;
            var stopwatch = Stopwatch.StartNew();
            
            logger.LogInformation($"开始性能测试: 发送 {messageCount} 条消息...");
            
            for (int i = 0; i < messageCount; i++)
            {
                await messagingService.SendMessageAsync(
                    "local", 
                    $"性能测试消息 {i + 1}/{messageCount}