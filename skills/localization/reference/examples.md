# localization - 使用示例

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
        var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
        
        Console.WriteLine("localization 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 localization 功能
        var localizedText = await localizationService.GetLocalizedTextAsync("Hello", "zh-CN");
        Console.WriteLine($"本地化文本: {localizedText}");
        
        // 测试其他语言
        var englishText = await localizationService.GetLocalizedTextAsync("Hello", "en-US");
        Console.WriteLine($"英文文本: {englishText}");
        
        var japaneseText = await localizationService.GetLocalizedTextAsync("Hello", "ja-JP");
        Console.WriteLine($"日文文本: {japaneseText}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILocalizationProcessor, LocalizationProcessor>();
        builder.AddSingleton<ILocalizationService, LocalizationService>();
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
        Console.WriteLine("localization 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 localization 设置
        builder.Configure<LocalizationSetting>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.DefaultCulture = "en-US";
            options.SupportedCultures = new[] { "en-US", "zh-CN", "ja-JP" };
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<ILocalizationProcessor, LocalizationProcessor>();
        builder.AddSingleton<ILocalizationService, LocalizationService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<LocalizationSetting>>().Value;
        Console.WriteLine($"配置: 缓存={settings.EnableCache}, 缓存大小={settings.CacheSize}");
        Console.WriteLine($"默认文化={settings.DefaultCulture}");
        Console.WriteLine($"支持的文化: {string.Join(", ", settings.SupportedCultures)}");
        
        // 使用服务
        var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
        var localizedText = await localizationService.GetLocalizedTextAsync("Hello", "zh-CN");
        Console.WriteLine($"本地化文本: {localizedText}");
    }
}
`

### 3. AOT 架构执行示例

#### 3.1 AOT 编译示例

`bash
# AOT 编译命令
dotnet publish scripts/localization_integration.cs -c Release -r win-x64 --aot

# 运行编译后的程序
./localization_integration.exe
`

#### 3.2 AOT 运行示例

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("localization AOT 运行示例");
        Console.WriteLine("=" * 50);
        
        // 启动 localization 服务
        Process.Start("./localization_integration.exe", "start");
        Console.WriteLine("localization 服务启动成功");
        
        // 获取本地化文本
        var localizeResult = Process.Start("./localization_integration.exe", "localize --key=Hello --culture=zh-CN");
        localizeResult.WaitForExit();
        Console.WriteLine("本地化命令执行完成");
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
        Console.WriteLine("localization Channel 事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建 Channel
        var channel = Channel.CreateBounded<LocalizationRequest>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        
        // 启动处理器
        var processorTask = ProcessRequestsAsync(channel.Reader);
        
        // 发送请求
        for (int i = 0; i < 5; i++)
        {
            var request = new LocalizationRequest {
                Key = $"Hello{i}",
                Culture = i % 2 == 0 ? "zh-CN" : "en-US"
            };
            await channel.Writer.WriteAsync(request);
            Console.WriteLine($"发送请求: Key={request.Key}, Culture={request.Culture}
