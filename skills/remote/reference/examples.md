# Remote 技能 - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        
        // 获取 OneRemote 服务
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        
        try
        {
            // 启动服务
            await remoteService.StartAsync();
            Console.WriteLine("服务启动成功");
            
            // 连接到远程服务器
            var connectionInfo = await remoteService.ConnectAsync("localhost", 22, "ssh");
            Console.WriteLine($"连接成功: {connectionInfo.Host}:{connectionInfo.Port}");
            
            // 执行远程命令
            var commandResult = await remoteService.ExecuteCommandAsync(connectionInfo.Id, "echo hello world");
            Console.WriteLine($"执行结果: {commandResult.Output}");
            
            // 断开连接
            await remoteService.DisconnectAsync(connectionInfo.Id);
            Console.WriteLine("连接已断开");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remoteService.StopAsync();
            Console.WriteLine("服务已停止");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册 OneRemote 服务
        builder.AddOneRemoteService(options => {
            options.ConnectionPoolSize = 100;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.RetryCount = 3;
        });
        
        // 注册日志服务
        builder.AddLogging();
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remote.Services;
using Remote.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 OneRemote 设置
        builder.Configure<OneRemoteOptions>(options => {
            options.ConnectionPoolSize = 150;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.RetryCount = 5;
            options.CircuitBreakerFailureThreshold = 0.5;
            options.CircuitBreakerDurationOfBreak = TimeSpan.FromSeconds(30);
            options.EnableCompression = true;
            options.EnableEncryption = true;
        });
        
        // 配置 Remotely 设置
        builder.Configure<RemotelyOptions>(options => {
            options.ServerUrl = "https://localhost:5001";
            options.OrganizationId = "your-organization-id";
            options.DeviceId = "your-device-id";
            options.DeviceAlias = "Test Device";
            options.HeartbeatInterval = 30;
            options.EnableRemoteControl = true;
            options.EnableFileTransfer = true;
            options.EnableChat = true;
        });
        
        // 注册服务
        builder.AddOneRemoteService();
        builder.AddRemotelyService();
        builder.AddLogging();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var oneRemoteSettings = serviceProvider.GetRequiredService<IOptions<OneRemoteOptions>>().Value;
        var remotelySettings = serviceProvider.GetRequiredService<IOptions<RemotelyOptions>>().Value;
        
        Console.WriteLine("OneRemote 配置:");
        Console.WriteLine($"  连接池大小: {oneRemoteSettings.ConnectionPoolSize}");
        Console.WriteLine($"  超时时间: {oneRemoteSettings.Timeout.TotalSeconds}秒");
        Console.WriteLine($"  重试次数: {oneRemoteSettings.RetryCount}");
        
        Console.WriteLine("\nRemotely 配置:");
        Console.WriteLine($"  服务器 URL: {remotelySettings.ServerUrl}");
        Console.WriteLine($"  设备别名: {remotelySettings.DeviceAlias}");
        Console.WriteLine($"  心跳间隔: {remotelySettings.HeartbeatInterval}秒");
        
        // 使用服务
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        var remotelyService = serviceProvider.GetRequiredService<IRemotelyService>();
        
        try
        {
            // 启动服务
            await Task.WhenAll(
                remoteService.StartAsync(),
                remotelyService.StartAsync()
            );
            
            Console.WriteLine("\n所有服务启动成功");
            
            // 执行操作...
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await Task.WhenAll(
                remoteService.StopAsync(),
                remotelyService.StopAsync()
            );
            
            Console.WriteLine("所有服务已停止");
        }
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        
        try
        {
            // 启动服务
            await remoteService.StartAsync();
            
            // 连接到远程服务器
            var connectionInfo = await remoteService.ConnectAsync("localhost", 22, "ssh");
            Console.WriteLine($"连接成功: {connectionInfo.Host}:{connectionInfo.Port}");
            
            // 性能测试
            const int iterations = 100;
            var stopwatch = Stopwatch.StartNew();
            
            Console.WriteLine($"\n执行 {iterations} 次远程命令...");
            
            for (int i = 0; i < iterations; i++)
            {
                var commandResult = await remoteService.ExecuteCommandAsync(connectionInfo.Id, "echo test command");
                if (i % 10 == 0)
                {
                    Console.WriteLine($"已执行 {i} 次");
                }
            }
            
            stopwatch.Stop();
            Console.WriteLine($"\n执行完成!");
            Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"平均每次执行: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
            
            // 断开连接
            await remoteService.DisconnectAsync(connectionInfo.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remoteService.StopAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册 OneRemote 服务，启用所有性能优化选项
        builder.AddOneRemoteService(options => {
            options.ConnectionPoolSize = 200;
            options.Timeout = TimeSpan.FromSeconds(30);
            options.RetryCount = 3;
            options.EnableCompression = true;
            options.EnableConnectionPooling = true;
            options.EnableMemoryPooling = true;
        });
        
        builder.AddLogging();
        
        return builder.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        
        try
        {
            // 启动服务
            await remoteService.StartAsync();
            
            // 尝试连接到不存在的服务器
            Console.WriteLine("尝试连接到不存在的服务器...");
            var connectionInfo = await remoteService.ConnectAsync("non-existent-server", 22, "ssh");
            Console.WriteLine($"连接成功: {connectionInfo.Host}:{connectionInfo.Port}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"网络错误: {ex.Message}");
            Console.WriteLine($"错误代码: {ex.ErrorCode}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
            Console.WriteLine($"错误类型: {ex.GetType().Name}");
        }
        finally
        {
            // 停止服务
            await remoteService.StopAsync();
            Console.WriteLine("服务已停止");
        }
        
        Console.WriteLine("\n错误处理示例完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册 OneRemote 服务
        builder.AddOneRemoteService(options => {
            options.ConnectionPoolSize = 100;
            options.Timeout = TimeSpan.FromSeconds(10); // 缩短超时时间以便快速看到错误
            options.RetryCount = 1; // 减少重试次数
        });
        
        builder.AddLogging();
        
        return builder.BuildServiceProvider();
    }
}
```

### 5. 远程命令执行示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 远程命令执行示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        
        try
        {
            // 启动服务
            await remoteService.StartAsync();
            
            // 连接到远程服务器
            var connectionInfo = await remoteService.ConnectAsync("localhost", 22, "ssh");
            Console.WriteLine($"连接成功: {connectionInfo.Host}:{connectionInfo.Port}");
            
            // 执行多个命令
            string[] commands = {
                "ls -la",
                "echo Current directory: $(pwd)",
                "date",
                "whoami"
            };
            
            foreach (var command in commands)
            {
                Console.WriteLine($"\n执行命令: {command}");
                Console.WriteLine("-" * 30);
                
                var commandResult = await remoteService.ExecuteCommandAsync(connectionInfo.Id, command);
                
                if (commandResult.ExitCode == 0)
                {
                    Console.WriteLine($"执行成功:");
                    Console.WriteLine(commandResult.Output);
                }
                else
                {
                    Console.WriteLine($"执行失败，退出码: {commandResult.ExitCode}");
                    Console.WriteLine($"错误信息: {commandResult.Output}");
                }
                
                Console.WriteLine($"执行时间: {commandResult.ExecutionTime.TotalMilliseconds:F2} ms");
            }
            
            // 断开连接
            await remoteService.DisconnectAsync(connectionInfo.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remoteService.StopAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddOneRemoteService();
        builder.AddLogging();
        return builder.BuildServiceProvider();
    }
}
```

### 6. 网络通信示例

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 网络通信示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        
        try
        {
            // 启动服务
            await remoteService.StartAsync();
            
            // 发送 HTTP 请求
            Console.WriteLine("发送 HTTP GET 请求到百度...");
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.baidu.com");
            
            var response = await remoteService.SendHttpRequestAsync(request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"请求成功，状态码: {response.StatusCode}");
                Console.WriteLine($"响应长度: {content.Length} 字符");
                Console.WriteLine($"响应内容前 500 字符: {content.Substring(0, Math.Min(500, content.Length))}...");
            }
            else
            {
                Console.WriteLine($"请求失败，状态码: {response.StatusCode}");
            }
            
            // 发送 TCP 请求
            Console.WriteLine("\n发送 TCP 请求...");
            var tcpResult = await remoteService.SendTcpRequestAsync(
                "example.com", 
                80, 
                "GET / HTTP/1.1\r\nHost: example.com\r\nConnection: close\r\n\r\n"
            );
            
            Console.WriteLine($"TCP 请求成功，响应长度: {tcpResult.Length} 字节");
            Console.WriteLine($"响应内容: {System.Text.Encoding.UTF8.GetString(tcpResult)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remoteService.StopAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddOneRemoteService();
        builder.AddLogging();
        return builder.BuildServiceProvider();
    }
}
```

### 7. 文件传输示例

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 文件传输示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remotelyService = serviceProvider.GetRequiredService<IRemotelyService>();
        
        try
        {
            // 启动服务
            await remotelyService.StartAsync();
            Console.WriteLine("服务启动成功");
            
            // 准备测试文件
            var testFilePath = Path.Combine(Environment.CurrentDirectory, "test-file.txt");
            File.WriteAllText(testFilePath, "This is a test file for Remote file transfer example.");
            Console.WriteLine($"创建测试文件: {testFilePath}");
            
            // 上传文件
            Console.WriteLine("\n上传文件到服务器...");
            var uploadResult = await remotelyService.UploadFileAsync(testFilePath, "/uploads/test-file.txt");
            
            if (uploadResult.Success)
            {
                Console.WriteLine($"上传成功，文件大小: {uploadResult.FileSize} 字节");
                Console.WriteLine($"服务器路径: {uploadResult.RemotePath}");
                
                // 下载文件
                Console.WriteLine("\n从服务器下载文件...");
                var downloadedContent = await remotelyService.DownloadFileAsync("/uploads/test-file.txt");
                
                Console.WriteLine($"下载成功，文件大小: {downloadedContent.Length} 字节");
                Console.WriteLine($"文件内容: {System.Text.Encoding.UTF8.GetString(downloadedContent)}");
            }
            else
            {
                Console.WriteLine($"上传失败: {uploadResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remotelyService.StopAsync();
            Console.WriteLine("服务已停止");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册 Remotely 服务
        builder.AddRemotelyService(options => {
            options.ServerUrl = "https://localhost:5001";
            options.OrganizationId = "your-organization-id";
            options.DeviceId = "your-device-id";
            options.DeviceAlias = "Test Device";
            options.EnableFileTransfer = true;
        });
        
        builder.AddLogging();
        return builder.BuildServiceProvider();
    }
}
```

### 8. 远程控制示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 远程控制示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remotelyService = serviceProvider.GetRequiredService<IRemotelyService>();
        
        try
        {
            // 启动服务
            await remotelyService.StartAsync();
            Console.WriteLine("服务启动成功");
            
            // 启动远程控制会话
            Console.WriteLine("启动远程控制会话...");
            var sessionId = await remotelyService.StartRemoteControlAsync();
            
            Console.WriteLine($"远程控制会话已启动，会话 ID: {sessionId}");
            Console.WriteLine("按任意键停止远程控制会话...");
            
            // 等待用户输入
            Console.ReadKey(true);
            
            // 停止远程控制会话
            Console.WriteLine("停止远程控制会话...");
            await remotelyService.StopRemoteControlAsync(sessionId);
            Console.WriteLine("远程控制会话已停止");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remotelyService.StopAsync();
            Console.WriteLine("服务已停止");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册 Remotely 服务
        builder.AddRemotelyService(options => {
            options.ServerUrl = "https://localhost:5001";
            options.OrganizationId = "your-organization-id";
            options.DeviceId = "your-device-id";
            options.DeviceAlias = "Test Device";
            options.EnableRemoteControl = true;
        });
        
        builder.AddLogging();
        return builder.BuildServiceProvider();
    }
}
```

### 9. 分布式任务处理示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remote.Services;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Remote 分布式任务处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var remoteService = serviceProvider.GetRequiredService<IOneRemoteService>();
        
        try
        {
            // 启动服务
            await remoteService.StartAsync();
            Console.WriteLine("服务启动成功");
            
            // 连接到多个远程服务器
            var servers = new List<(string host, int port)> {
                ("server1", 22),
                ("server2", 22),
                ("server3", 22)
            };
            
            var connections = new List<Remote.Services.ConnectionInfo>();
            
            foreach (var (host, port) in servers)
            {
                try
                {
                    Console.WriteLine($"连接到服务器: {host}:{port}");
                    var connection = await remoteService.ConnectAsync(host, port, "ssh");
                    connections.Add(connection);
                    Console.WriteLine($"连接成功: {connection.Host}:{connection.Port}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"连接失败: {ex.Message}");
                }
            }
            
            // 在所有连接的服务器上执行任务
            Console.WriteLine("\n在所有服务器上执行任务...");
            var tasks = new List<Task>();
            
            foreach (var connection in connections)
            {
                tasks.Add(Task.Run(async () => {
                    try
                    {
                        Console.WriteLine($"在 {connection.Host}:{connection.Port} 上执行任务...");
                        
                        // 执行系统更新
                        var updateResult = await remoteService.ExecuteCommandAsync(connection.Id, "sudo apt update");
                        Console.WriteLine($"{connection.Host}: 系统更新结果: {updateResult.ExitCode == 0 ? "成功" : "失败"}");
                        
                        // 执行磁盘检查
                        var diskResult = await remoteService.ExecuteCommandAsync(connection.Id, "df -h");
                        Console.WriteLine($"{connection.Host}: 磁盘使用情况:");
                        Console.WriteLine(diskResult.Output);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{connection.Host}: 执行任务失败: {ex.Message}");
                    }
                }));
            }
            
            // 等待所有任务完成
            await Task.WhenAll(tasks);
            Console.WriteLine("\n所有任务执行完成");
            
            // 断开所有连接
            foreach (var connection in connections)
            {
                await remoteService.DisconnectAsync(connection.Id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        finally
        {
            // 停止服务
            await remoteService.StopAsync();
            Console.WriteLine("服务已停止");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 注册 OneRemote 服务
        builder.AddOneRemoteService(options => {
            options.ConnectionPoolSize = 100;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.RetryCount = 3;
        });
        
        builder.AddLogging();
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例展示了 Remote 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 执行远程命令
6. 进行网络通信
7. 传输文件
8. 实现远程控制
9. 处理分布式任务

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

通过合理使用 Remote 技能，您可以轻松构建高性能、可靠的分布式系统，提高应用程序的可扩展性和可用性。
