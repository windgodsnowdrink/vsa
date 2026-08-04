# llcom - 使用示例

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
        var llComService = serviceProvider.GetRequiredService<ILLComService>();
        
        Console.WriteLine("llcom 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 llcom 功能
        var device = await llComService.ConnectDeviceAsync();
        Console.WriteLine($"设备连接成功: {device.DeviceId}");
        
        // 发送数据
        await llComService.SendDataAsync(device.DeviceId, new byte[] { 0x01, 0x02, 0x03 });
        Console.WriteLine("数据发送成功");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
        builder.AddSingleton<ILLComService, LLComService>();
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
        Console.WriteLine("llcom 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 llcom 设置
        builder.Configure<LLComSettings>(options => {
            options.EnableDataProcessing = true;
            options.MaxBufferSize = 1024 * 1024;
            options.ReadTimeout = TimeSpan.FromSeconds(30);
            options.WriteTimeout = TimeSpan.FromSeconds(10);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
        builder.AddSingleton<ILLComService, LLComService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<LLComSettings>>().Value;
        Console.WriteLine($"配置: 数据处理={settings.EnableDataProcessing}, 最大缓冲区={settings.MaxBufferSize}");
        Console.WriteLine($"读取超时={settings.ReadTimeout}, 写入超时={settings.WriteTimeout}");
        
        // 使用服务
        var llComService = serviceProvider.GetRequiredService<ILLComService>();
        var device = await llComService.ConnectDeviceAsync();
        Console.WriteLine($"设备连接成功: {device.DeviceId}");
    }
}
`

### 3. AOT 架构执行示例

#### 3.1 AOT 编译示例

`bash
# AOT 编译命令
dotnet publish scripts/llcom_integration.cs -c Release -r win-x64 --aot

# 运行编译后的程序
./llcom_integration.exe
`

#### 3.2 AOT 运行示例

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("llcom AOT 运行示例");
        Console.WriteLine("=" * 50);
        
        // 启动 llcom 服务
        Process.Start("./llcom_integration.exe", "start");
        Console.WriteLine("llcom 服务启动成功");
        
        // 连接设备
        var connectResult = Process.Start("./llcom_integration.exe", "connect-device");
        connectResult.WaitForExit();
        Console.WriteLine("设备连接命令执行完成");
        
        // 发送数据
        Process.Start("./llcom_integration.exe", "send-data --device-id=123 --data=010203");
        Console.WriteLine("数据发送命令执行完成");
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
        Console.WriteLine("llcom Channel 事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建 Channel
        var channel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        
        // 启动处理器
        var processorTask = ProcessDataAsync(channel.Reader);
        
        // 发送数据
        for (int i = 0; i < 10; i++)
        {
            var data = new byte[] { (byte)i, (byte)(i + 1), (byte)(i + 2) };
            await channel.Writer.WriteAsync(data);
            Console.WriteLine($"发送数据: {BitConverter.ToString(data)}");
        }
        
        // 标记完成
        channel.Writer.Complete();
        
        // 等待处理完成
        await processorTask;
        Console.WriteLine("所有数据处理完成");
    }
    
    private static async Task ProcessDataAsync(ChannelReader<byte[]> reader)
    {
        await foreach (var data in reader.ReadAllAsync())
        {
            Console.WriteLine($"处理数据: {BitConverter.ToString(data)}");
            // 模拟处理时间
            await Task.Delay(10);
        }
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
        Console.WriteLine("llcom 性能测试示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var llComService = serviceProvider.GetRequiredService<ILLComService>();
        
        // 连接设备
        var device = await llComService.ConnectDeviceAsync();
        Console.WriteLine($"设备连接成功: {device.DeviceId}");
        
        // 性能测试
        const int iterations = 1000;
        var testData = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            await llComService.SendDataAsync(device.DeviceId, testData);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"执行 {iterations} 次发送操作的时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次发送: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
        builder.AddSingleton<ILLComService, LLComService>();
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
        Console.WriteLine("llcom 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var llComService = serviceProvider.GetRequiredService<ILLComService>();
        
        try
        {
            // 测试无效设备
            await llComService.SendDataAsync("invalid-device", new byte[] { 0x01, 0x02, 0x03 });
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
        builder.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
        builder.AddSingleton<ILLComService, LLComService>();
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
                    // 注册 llcom 服务
                    services.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
                    services.AddSingleton<ILLComService, LLComService>();
                });
                
                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/llcom/connect", async context =>
                        {
                            var llComService = context.RequestServices.GetRequiredService<ILLComService>();
                            var device = await llComService.ConnectDeviceAsync();
                            await context.Response.WriteAsync($"Device connected: {device.DeviceId}");
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
        var llComService = serviceProvider.GetRequiredService<ILLComService>();
        
        while (true)
        {
            Console.WriteLine("\nllcom 控制台应用");
            Console.WriteLine("1. 连接设备");
            Console.WriteLine("2. 发送数据");
            Console.WriteLine("3. 断开设备");
            Console.WriteLine("4. 退出");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    var device = await llComService.ConnectDeviceAsync();
                    Console.WriteLine($"设备连接成功: {device.DeviceId}");
                    break;
                case "2":
                    Console.Write("请输入设备ID: ");
                    var deviceId = Console.ReadLine();
                    Console.Write("请输入要发送的数据 (十六进制): ");
                    var dataHex = Console.ReadLine();
                    var data = ConvertHexStringToByteArray(dataHex);
                    await llComService.SendDataAsync(deviceId, data);
                    Console.WriteLine("数据发送成功");
                    break;
                case "3":
                    Console.Write("请输入设备ID: ");
                    var disconnectDeviceId = Console.ReadLine();
                    await llComService.DisconnectDeviceAsync(disconnectDeviceId);
                    Console.WriteLine("设备断开成功");
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("无效选择");
                    break;
            }
        }
    }
    
    private static byte[] ConvertHexStringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length / 2)
            .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
            .ToArray();
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILLStreamProcessor, LLStreamProcessor>();
        builder.AddSingleton<ILLComService, LLComService>();
        return builder.BuildServiceProvider();
    }
}
`

## 总结

以上示例展示了 llcom 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 使用 AOT 编译提高性能
4. 利用 Channel 实现高效的事件处理
5. 优化性能
6. 处理错误情况
7. 与不同类型的应用集成

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。
