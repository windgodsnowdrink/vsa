# log - 使用示例

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
        var logService = serviceProvider.GetRequiredService<ILogService>();
        
        Console.WriteLine("log 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 log 功能
        await logService.LogInformationAsync("应用程序启动");
        Console.WriteLine("信息日志记录成功");
        
        // 记录错误
        await logService.LogErrorAsync("发生错误", new Exception("测试错误"));
        Console.WriteLine("错误日志记录成功");
        
        // 记录警告
        await logService.LogWarningAsync("警告信息");
        Console.WriteLine("警告日志记录成功");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILogProcessor, LogProcessor>();
        builder.AddSingleton<ILogService, LogService>();
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
        Console.WriteLine("log 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 log 设置
        builder.Configure<LogSetting>(options => {
            options.EnableAsyncProcessing = true;
            options.BatchSize = 100;
            options.FlushInterval = TimeSpan.FromSeconds(5);
            options.MinimumLogLevel = LogLevel.Information;
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<ILogProcessor, LogProcessor>();
        builder.AddSingleton<ILogService, LogService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<LogSetting>>().Value;
        Console.WriteLine($"配置: 异步处理={settings.EnableAsyncProcessing}, 批处理大小={settings.BatchSize}");
        Console.WriteLine($"刷新间隔={settings.FlushInterval}, 最小日志级别={settings.MinimumLogLevel}");
        
        // 使用服务
        var logService = serviceProvider.GetRequiredService<ILogService>();
        await logService.LogInformationAsync("使用高级配置记录日志");
        Console.WriteLine("日志记录成功");
    }
}
`

### 3. AOT 架构执行示例

#### 3.1 AOT 编译示例

`bash
# AOT 编译命令
dotnet publish scripts/serilog_integration.cs -c Release -r win-x64 --aot

# 运行编译后的程序
./serilog_integration.exe
`

#### 3.2 AOT 运行示例

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("log AOT 运行示例");
        Console.WriteLine("=" * 50);
        
        // 启动 log 服务
        Process.Start("./serilog_integration.exe", "start");
        Console.WriteLine("log 服务启动成功");
        
        // 记录信息日志
        var infoResult = Process.Start("./serilog_integration.exe", "log --level=info --message=Application started");
        infoResult.WaitForExit();
        Console.WriteLine("信息日志命令执行完成");
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
        Console.WriteLine("log Channel 事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建 Channel
        var channel = Channel.CreateBounded<LogEntry>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        
        // 启动处理器
        var processorTask = ProcessLogsAsync(channel.Reader);
        
        // 发送日志
        for (int i = 0; i < 5; i++)
        {
            var logEntry = new LogEntry {
                Message = $"Test log {i}
