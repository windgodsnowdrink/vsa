# gRPC - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var grpcServer = serviceProvider.GetRequiredService<GrpcServer>();
        var grpcClient = serviceProvider.GetRequiredService<GrpcClient>();
        
        Console.WriteLine("gRPC 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 启动服务器
        Console.WriteLine("启动 gRPC 服务器...");
        await grpcServer.StartAsync();
        
        // 使用客户端发送请求
        Console.WriteLine("发送 Hello 请求...");
        var helloResponse = await grpcClient.SayHelloAsync("World");
        Console.WriteLine($"Hello 响应: {helloResponse.Message}");
        
        Console.WriteLine("发送 Ping 请求...");
        var pingResponse = await grpcClient.PingAsync();
        Console.WriteLine($"Ping 响应: {pingResponse.Timestamp}");
        
        Console.WriteLine("发送健康检查请求...");
        var healthResponse = await grpcClient.CheckHealthAsync();
        Console.WriteLine($"健康检查响应: {healthResponse.Status}");
        
        // 停止服务器
        await grpcServer.StopAsync();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<GrpcServer>();
        builder.AddSingleton<GrpcClient>();
        builder.AddSingleton<GrpcSettings>();
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

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("gRPC 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 gRPC 设置
        builder.Configure<GrpcSettings>(options => {
            options.ServerHost = "localhost";
            options.ServerPort = 50051;
            options.MaxConcurrentCalls = 200;
            options.KeepAliveTimeSeconds = 30;
            options.MaxMessageSize = 8388608; // 8MB
            options.EnableTls = false;
        });
        
        // 注册服务
        builder.AddSingleton<GrpcServer>();
        builder.AddSingleton<GrpcClient>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<GrpcSettings>>().Value;
        Console.WriteLine($"配置: 主机={settings.ServerHost}, 端口={settings.ServerPort}");
        Console.WriteLine($"最大并发调用={settings.MaxConcurrentCalls}, 心跳时间={settings.KeepAliveTimeSeconds}秒");
        
        // 使用服务
        var server = serviceProvider.GetRequiredService<GrpcServer>();
        var client = serviceProvider.GetRequiredService<GrpcClient>();
        
        await server.StartAsync();
        
        var response = await client.SayHelloAsync("Advanced Configuration");
        Console.WriteLine($"Hello 响应: {response.Message}");
        
        await server.StopAsync();
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("gRPC 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var server = serviceProvider.GetRequiredService<GrpcServer>();
        var client = serviceProvider.GetRequiredService<GrpcClient>();
        
        // 启动服务器
        await server.StartAsync();
        
        // 性能测试
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"执行 {iterations} 次请求...");
        
        for (int i = 0; i < iterations; i++)
        {
            await client.PingAsync();
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        Console.WriteLine($"每秒请求数: {iterations / stopwatch.Elapsed.TotalSeconds:F2} QPS");
        
        // 停止服务器
        await server.StopAsync();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.Configure<GrpcSettings>(options => {
            options.MaxConcurrentCalls = 1000;
            options.KeepAliveTimeSeconds = 60;
        });
        builder.AddSingleton<GrpcServer>();
        builder.AddSingleton<GrpcClient>();
        return builder.BuildServiceProvider();
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("gRPC 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var server = serviceProvider.GetRequiredService<GrpcServer>();
        var client = serviceProvider.GetRequiredService<GrpcClient>();
        
        try
        {
            // 启动服务器
            await server.StartAsync();
            
            // 发送正常请求
            var helloResponse = await client.SayHelloAsync("Error Handling");
            Console.WriteLine($"成功: {helloResponse.Message}");
            
            // 模拟网络错误
            Console.WriteLine("模拟网络错误...");
            // 这里可以修改客户端配置来模拟错误
            
        }
        catch (Grpc.Core.RpcException ex)
        {
            Console.WriteLine($"RPC 错误: {ex.Status.Detail}");
            Console.WriteLine($"错误码: {ex.Status.StatusCode}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        finally
        {
            // 确保停止服务器
            await server.StopAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<GrpcServer>();
        builder.AddSingleton<GrpcClient>();
        return builder.BuildServiceProvider();
    }
}
```

### 5. AOT 编译示例

```csharp
// scripts/grpc_aot.cs 文件示例
#:sdk Microsoft.NET.Sdk
#:package Grpc.AspNetCore@2.62.0
#:package Grpc.Net.Client@2.62.0
#:package Google.Protobuf@3.26.1
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("gRPC AOT 编译示例");
        Console.WriteLine("=" * 50);
        
        // 解析命令行参数
        var command = args.Length > 0 ? args[0] : "help";
        
        using var serviceProvider = BuildServiceProvider();
        
        switch (command.ToLower())
        {
            case "server":
            case "s":
                await StartServer(serviceProvider);
                break;
            case "client":
            case "c":
                await StartClient(serviceProvider);
                break;
            case "hello":
            case "h":
                await SendHello(serviceProvider);
                break;
            case "ping":
            case "p":
                await SendPing(serviceProvider);
                break;
            case "health":
            case "he":
                await CheckHealth(serviceProvider);
                break;
            default:
                ShowHelp();
                break;
        }
    }
    
    // 其他方法实现...
}
```

## 总结

以上示例展示了 gRPC 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用 AOT 编译

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### AOT 编译优势

- **快速启动**: 比 JIT 编译快 3-5 倍
- **低内存占用**: 内存占用减少 20-30%
- **简单部署**: 单文件执行，无运行时依赖
- **稳定性能**: 编译时优化
- **高安全性**: 减少运行时攻击面

### 命令行使用

```bash
# 启动 gRPC 服务器
grpc_aot.exe server

# 发送 Hello 请求
grpc_aot.exe hello

# 发送 Ping 请求
grpc_aot.exe ping

# 发送健康检查请求
grpc_aot.exe health

# 显示帮助信息
grpc_aot.exe help
```
