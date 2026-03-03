# livechat - 使用示例

## 快速开始

### 1. 基本使用示例

`csharp
using System;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var liveChatService = serviceProvider.GetRequiredService<ILiveChatService>();
        
        Console.WriteLine("livechat 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 livechat 功能
        var session = await liveChatService.CreateSessionAsync();
        Console.WriteLine($"创建会话成功: {session.SessionId}");
        
        // 发送消息
        await liveChatService.SendMessageAsync(session.SessionId, "Hello, livechat!");
        Console.WriteLine("消息发送成功");
        
        // 结束会话
        await liveChatService.EndSessionAsync(session.SessionId);
        Console.WriteLine("会话结束成功");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILiveStreamProcessor, LiveStreamProcessor>();
        builder.AddSingleton<ILiveChatService, LiveChatService>();
        return builder.BuildServiceProvider();
    }
}
`

### 2. 高级配置示例

`csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("livechat 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 livechat 设置
        builder.Configure<LiveChatSettings>(options => {
            options.EnableVideoProcessing = true;
            options.EnableAudioProcessing = true;
            options.MaxConcurrentSessions = 100;
            options.MessageTimeout = TimeSpan.FromSeconds(30);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<ILiveStreamProcessor, LiveStreamProcessor>();
        builder.AddSingleton<ILiveChatService, LiveChatService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<LiveChatSettings>>().Value;
        Console.WriteLine($"配置: 视频处理={settings.EnableVideoProcessing}, 音频处理={settings.EnableAudioProcessing}");
        Console.WriteLine($"最大会话数={settings.MaxConcurrentSessions}, 超时={settings.MessageTimeout}");
        
        // 使用服务
        var liveChatService = serviceProvider.GetRequiredService<ILiveChatService>();
        var session = await liveChatService.CreateSessionAsync();
        Console.WriteLine($"创建会话成功: {session.SessionId}");
    }
}
`

### 3. AOT 架构执行示例

#### 3.1 AOT 编译示例

`bash
# AOT 编译命令
dotnet publish scripts/livechat_integration.cs -c Release -r win-x64 --aot

# 运行编译后的程序
./livechat_integration.exe
`

#### 3.2 AOT 运行示例

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("livechat AOT 运行示例");
        Console.WriteLine("=" * 50);
        
        // 启动 livechat 服务
        Process.Start("./livechat_integration.exe", "start");
        Console.WriteLine("livechat 服务启动成功");
        
        // 创建会话
        var createResult = Process.Start("./livechat_integration.exe", "create-session");
        createResult.WaitForExit();
        Console.WriteLine("创建会话命令执行完成");
        
        // 发送消息
        Process.Start("./livechat_integration.exe", "send-message --session-id=123 --message=Hello");
        Console.WriteLine("消息发送命令执行完成");
    }
}
`

### 4. Channel 事件处理示例

`csharp
using System;
using System.Threading.Tasks;
using System.Threading.Channels;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("livechat Channel 事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建 Channel
        var channel = Channel.CreateBounded<MediaFrame>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        
        // 启动处理器
        var processorTask = ProcessFramesAsync(channel.Reader);
        
        // 发送视频帧
        for (int i = 0; i < 10; i++)
        {
            var frame = new MediaFrame { FrameId = i, Data = new byte[1024] };
            await channel.Writer.WriteAsync(frame);
            Console.WriteLine($"发送视频帧: {i}");
        }
        
        // 标记完成
        channel.Writer.Complete();
        
        // 等待处理完成
        await processorTask;
        Console.WriteLine("所有帧处理完成");
    }
    
    private static async Task ProcessFramesAsync(ChannelReader<MediaFrame> reader)
    {
        await foreach (var frame in reader.ReadAllAsync())
        {
            Console.WriteLine($"处理视频帧: {frame.FrameId}");
            // 模拟处理时间
            await Task.Delay(10);
        }
    }
    
    public class MediaFrame
    {
        public int FrameId { get; set; }
        public byte[] Data { get; set; }
    }
}
`

### 5. 性能测试示例

`csharp
using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("livechat 性能测试示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var liveChatService = serviceProvider.GetRequiredService<ILiveChatService>();
        
        // 性能测试
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            var session = await liveChatService.CreateSessionAsync();
            await liveChatService.SendMessageAsync(session.SessionId, $"Test message {i}");
            await liveChatService.EndSessionAsync(session.SessionId);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行 {iterations} 次操作的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次操作: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILiveStreamProcessor, LiveStreamProcessor>();
        builder.AddSingleton<ILiveChatService, LiveChatService>();
        return builder.BuildServiceProvider();
    }
}
`

### 6. 错误处理示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("livechat 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var liveChatService = serviceProvider.GetRequiredService<ILiveChatService>();
        
        try
        {
            // 测试无效会话
            await liveChatService.SendMessageAsync("invalid-session", "Test message");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"参数错误: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILiveStreamProcessor, LiveStreamProcessor>();
        builder.AddSingleton<ILiveChatService, LiveChatService>();
        return builder.BuildServiceProvider();
    }
}
`

### 7. 集成示例

#### 7.1 ASP.NET Core 集成

`csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
    
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    // 注册 livechat 服务
                    services.AddSingleton<ILiveStreamProcessor, LiveStreamProcessor>();
                    services.AddSingleton<ILiveChatService, LiveChatService>();
                });
                
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/livechat/session", async context =>
                        {
                            var liveChatService = context.RequestServices.GetRequiredService<ILiveChatService>();
                            var session = await liveChatService.CreateSessionAsync();
                            await context.Response.WriteAsync($"Session created: {session.SessionId}");
                        });
                    });
                });
            });
}
`

#### 7.2 控制台应用集成

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        var serviceProvider = BuildServiceProvider();
        var liveChatService = serviceProvider.GetRequiredService<ILiveChatService>();
        
        while (true)
        {
            Console.WriteLine("\nlivechat 控制台应用");
            Console.WriteLine("1. 创建会话");
            Console.WriteLine("2. 发送消息");
            Console.WriteLine("3. 结束会话");
            Console.WriteLine("4. 退出");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    var session = await liveChatService.CreateSessionAsync();
                    Console.WriteLine($"会话创建成功: {session.SessionId}");
                    break;
                case "2":
                    Console.Write("请输入会话ID: ");
                    var sessionId = Console.ReadLine();
                    Console.Write("请输入消息: ");
                    var message = Console.ReadLine();
                    await liveChatService.SendMessageAsync(sessionId, message);
                    Console.WriteLine("消息发送成功");
                    break;
                case "3":
                    Console.Write("请输入会话ID: ");
                    var endSessionId = Console.ReadLine();
                    await liveChatService.EndSessionAsync(endSessionId);
                    Console.WriteLine("会话结束成功");
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("无效选择");
                    break;
            }
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILiveStreamProcessor, LiveStreamProcessor>();
        builder.AddSingleton<ILiveChatService, LiveChatService>();
        return builder.BuildServiceProvider();
    }
}
`

## 总结

以上示例展示了 livechat 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 使用 AOT 编译提高性能
4. 利用 Channel 实现高效的事件处理
5. 优化性能
6. 处理错误情况
7. 与不同类型的应用集成

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
