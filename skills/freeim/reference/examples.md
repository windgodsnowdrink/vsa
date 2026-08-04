# FreeIM - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FreeIM.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FreeIM 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<FreeIMOptions>(builder.Configuration.GetSection("FreeIM"));
        builder.Services.AddSingleton<IFreeIMService, FreeIMService>();
        builder.Services.AddSingleton<FreeIMAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 FreeIM 服务
        var freeIMService = host.Services.GetRequiredService<IFreeIMService>();
        
        // 连接到 IM 服务器
        Console.WriteLine("1. 连接到 IM 服务器...");
        var connectResult = await freeIMService.ConnectAsync("user1");
        Console.WriteLine($"   连接结果: {(connectResult.Success ? "成功" : "失败")}");
        if (!connectResult.Success)
        {
            Console.WriteLine($"   错误信息: {connectResult.ErrorMessage}");
            return;
        }
        
        // 发送消息
        Console.WriteLine("2. 发送消息...");
        var sendResult = await freeIMService.SendMessageAsync("user1", "user2", "Hello World", "text");
        Console.WriteLine($"   发送消息结果: {(sendResult.Success ? "成功" : "失败")}");
        if (!sendResult.Success)
        {
            Console.WriteLine($"   错误信息: {sendResult.ErrorMessage}");
            return;
        }
        
        // 接收消息
        Console.WriteLine("3. 接收消息...");
        var receiveResult = await freeIMService.ReceiveMessageAsync("user2");
        Console.WriteLine($"   接收消息结果: {(receiveResult.Success ? "成功" : "失败")}");
        if (receiveResult.Success && receiveResult.Messages != null)
        {
            foreach (var message in receiveResult.Messages)
            {
                Console.WriteLine($"   消息: {message.Content}");
            }
        }
        
        // 列出用户
        Console.WriteLine("4. 列出用户...");
        var listResult = await freeIMService.ListUsersAsync();
        Console.WriteLine($"   列出用户结果: {(listResult.Success ? "成功" : "失败")}");
        if (listResult.Success && listResult.Users != null)
        {
            foreach (var user in listResult.Users)
            {
                Console.WriteLine($"   用户: {user.DisplayName} ({user.Status})");
            }
        }
        
        // 获取版本信息
        Console.WriteLine("5. 获取版本信息...");
        var versionResult = await freeIMService.GetVersionInfoAsync();
        Console.WriteLine($"   获取版本信息结果: {(versionResult.Success ? "成功" : "失败")}");
        if (versionResult.Success)
        {
            foreach (var item in versionResult.Results)
            {
                Console.WriteLine($"   {item}");
            }
        }
        
        // 断开连接
        Console.WriteLine("6. 断开连接...");
        var disconnectResult = await freeIMService.DisconnectAsync("user1");
        Console.WriteLine($"   断开连接结果: {(disconnectResult.Success ? "成功" : "失败")}");
        
        Console.WriteLine("\n示例完成！");
    }
}
```

### 2. 命令行使用示例

FreeIM AOT 引擎支持通过命令行执行各种操作，以下是命令行使用示例：

#### 连接到 IM 服务器
```bash
# 使用命令行连接到 IM 服务器
freeim_aot connect user1

# 输出示例
命令执行结果: 成功
执行时间: 205 ms
- 成功连接到 IM 服务器
- 用户: 用户1
- 服务器地址: localhost:8080
- 连接时间: 2024-01-19 12:00:00
```

#### 发送消息
```bash
# 使用命令行发送消息
freeim_aot send user1 user2 "Hello from command line"

# 输出示例
命令执行结果: 成功
执行时间: 102 ms
- 成功发送消息: 12345678-1234-1234-1234-1234567890ab
- 发送者: 用户1
- 接收者: 用户2
- 消息类型: text
- 发送时间: 2024-01-19 12:00:01
```

#### 接收消息
```bash
# 使用命令行接收消息
freeim_aot receive user2

# 输出示例
命令执行结果: 成功
执行时间: 55 ms
- 成功接收消息: 1 条
- 接收者: 用户2

消息列表:
  [2024-01-19 12:00:01] user1 -> user2: Hello from command line
```

#### 列出用户
```bash
# 使用命令行列出用户
freeim_aot list

# 输出示例
命令执行结果: 成功
执行时间: 52 ms
- 成功获取用户列表
- 总用户数: 3
- 在线用户数: 1

用户列表:
  用户1 (user1) - online
  用户2 (user2) - offline
  用户3 (user3) - offline
```

#### 断开连接
```bash
# 使用命令行断开连接
freeim_aot disconnect user1

# 输出示例
命令执行结果: 成功
执行时间: 105 ms
- 成功断开连接
- 用户: 用户1
- 断开时间: 2024-01-19 12:00:02
```

#### 显示版本信息
```bash
# 使用命令行显示版本信息
freeim_aot version

# 输出示例
命令执行结果: 成功
执行时间: 50 ms
- FreeIM AOT Engine
- 版本: 1.0.0
- .NET 版本: 10.0.0
- 操作系统: Microsoft Windows 10.0.19045
- 架构: X64
- AOT 编译: True
- 服务器地址: localhost:8080
- 启用 SSL: False
- 工作目录: d:\Trae\vsa\skills\freeim
```

### 3. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using FreeIM.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FreeIM 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 创建配置
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "FreeIM:ServerAddress", "localhost" },
                { "FreeIM:ServerPort", "8080" },
                { "FreeIM:EnableSsl", "false" },
                { "FreeIM:EnableCache", "true" },
                { "FreeIM:CacheSize", "1000" },
                { "FreeIM:RequestTimeoutMs", "30000" },
                { "FreeIM:EnableDetailedLogging", "true" },
                { "FreeIM:EnablePerformanceMonitoring", "true" }
            })
            .Build();
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        builder.Configuration.AddConfiguration(configuration);
        
        // 配置服务
        builder.Services.Configure<FreeIMOptions>(builder.Configuration.GetSection("FreeIM"));
        builder.Services.AddSingleton<IFreeIMService, FreeIMService>();
        builder.Services.AddSingleton<FreeIMAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取配置
        var options = host.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<FreeIMOptions>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"- 服务器地址: {options.ServerAddress}:{options.ServerPort}");
        Console.WriteLine($"- 启用 SSL: {options.EnableSsl}");
        Console.WriteLine($"- 启用缓存: {options.EnableCache}");
        Console.WriteLine($"- 缓存大小: {options.CacheSize}");
        Console.WriteLine($"- 请求超时: {options.RequestTimeoutMs} ms");
        Console.WriteLine($"- 启用详细日志: {options.EnableDetailedLogging}");
        Console.WriteLine($"- 启用性能监控: {options.EnablePerformanceMonitoring}");
        
        // 获取 FreeIM 服务
        var freeIMService = host.Services.GetRequiredService<IFreeIMService>();
        
        // 使用服务
        Console.WriteLine("\n使用 FreeIM 服务...");
        var versionResult = await freeIMService.GetVersionInfoAsync();
        if (versionResult.Success)
        {
            Console.WriteLine("版本信息获取成功:");
            foreach (var item in versionResult.Results)
            {
                Console.WriteLine($"- {item}");
            }
        }
    }
}
```

### 4. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FreeIM.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FreeIM 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<FreeIMOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.EnablePerformanceMonitoring = true;
        });
        builder.Services.AddSingleton<IFreeIMService, FreeIMService>();
        builder.Services.AddSingleton<FreeIMAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 FreeIM 服务
        var freeIMService = host.Services.GetRequiredService<IFreeIMService>();
        
        // 连接到 IM 服务器
        await freeIMService.ConnectAsync("user1");
        
        // 性能测试
        const int iterations = 100;
        var totalTime = 0L;
        
        Console.WriteLine($"执行 {iterations} 次消息发送测试...");
        
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var result = await freeIMService.SendMessageAsync("user1", "user2", $"Test message {i}", "text");
            if (!result.Success)
            {
                Console.WriteLine($"测试失败: {result.ErrorMessage}");
                break;
            }
            totalTime += result.ExecutionTimeMs;
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"测试完成！");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次执行时间: {totalTime / (double)iterations:F3} ms");
        Console.WriteLine($"AOT 编译提升性能: 启动速度快，内存占用低");
        
        // 断开连接
        await freeIMService.DisconnectAsync("user1");
    }
}
```

### 5. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FreeIM.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("FreeIM 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机
        var builder = Host.CreateApplicationBuilder();
        
        // 配置服务
        builder.Services.Configure<FreeIMOptions>(builder.Configuration.GetSection("FreeIM"));
        builder.Services.AddSingleton<IFreeIMService, FreeIMService>();
        builder.Services.AddSingleton<FreeIMAotEngine>();
        
        // 构建主机
        using var host = builder.Build();
        
        // 获取 FreeIM 服务
        var freeIMService = host.Services.GetRequiredService<IFreeIMService>();
        
        // 测试错误处理
        Console.WriteLine("1. 测试发送消息给不存在的用户...");
        var sendResult = await freeIMService.SendMessageAsync("user1", "nonexistent", "Hello");
        Console.WriteLine($"   结果: {(sendResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {sendResult.ErrorMessage}");
        
        Console.WriteLine("2. 测试未连接时发送消息...");
        sendResult = await freeIMService.SendMessageAsync("user1", "user2", "Hello");
        Console.WriteLine($"   结果: {(sendResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {sendResult.ErrorMessage}");
        
        Console.WriteLine("3. 测试重复连接...");
        var connectResult1 = await freeIMService.ConnectAsync("user1");
        Console.WriteLine($"   第一次连接结果: {(connectResult1.Success ? "成功" : "失败")}");
        
        var connectResult2 = await freeIMService.ConnectAsync("user1");
        Console.WriteLine($"   第二次连接结果: {(connectResult2.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {connectResult2.ErrorMessage}");
        
        Console.WriteLine("4. 测试未连接时断开连接...");
        // 先断开连接
        await freeIMService.DisconnectAsync("user1");
        
        var disconnectResult = await freeIMService.DisconnectAsync("user1");
        Console.WriteLine($"   断开连接结果: {(disconnectResult.Success ? "成功" : "失败")}");
        Console.WriteLine($"   错误信息: {disconnectResult.ErrorMessage}");
        
        Console.WriteLine("\n错误处理测试完成！");
    }
}
```

## 总结

以上示例展示了 FreeIM AOT 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用命令行工具执行操作

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。通过 AOT 编译，FreeIM 引擎获得了更快的启动速度和更低的内存占用，同时保持了完整的功能和良好的用户体验。
