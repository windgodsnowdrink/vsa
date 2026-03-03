# LLM - 使用示例

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
        var llmService = serviceProvider.GetRequiredService<ILLMService>();
        
        Console.WriteLine("LLM 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 LLM 功能
        var response = await llmService.GenerateResponseAsync("Hello, LLM!");
        Console.WriteLine($"LLM 响应: {response}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILLMProcessor, LLMProcessor>();
        builder.AddSingleton<ILLMService, LLMService>();
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
        Console.WriteLine("LLM 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 LLM 设置
        builder.Configure<LLMSetting>(options => {
            options.EnableMemory = true;
            options.MaxMemorySize = 1000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.ModelName = "gpt-4";
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<ILLMProcessor, LLMProcessor>();
        builder.AddSingleton<ILLMService, LLMService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<LLMSetting>>().Value;
        Console.WriteLine($"配置: 记忆={settings.EnableMemory}, 模型={settings.ModelName}");
        Console.WriteLine($"最大记忆大小={settings.MaxMemorySize}, 超时={settings.Timeout}");
        
        // 使用服务
        var llmService = serviceProvider.GetRequiredService<ILLMService>();
        var response = await llmService.GenerateResponseAsync("Hello, LLM!");
        Console.WriteLine($"LLM 响应: {response}");
    }
}
`

### 3. AOT 架构执行示例

#### 3.1 AOT 编译示例

`bash
# AOT 编译命令
dotnet publish scripts/llm_integration.cs -c Release -r win-x64 --aot

# 运行编译后的程序
./llm_integration.exe
`

#### 3.2 AOT 运行示例

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("LLM AOT 运行示例");
        Console.WriteLine("=" * 50);
        
        // 启动 LLM 服务
        Process.Start("./llm_integration.exe", "start");
        Console.WriteLine("LLM 服务启动成功");
        
        // 生成响应
        var generateResult = Process.Start("./llm_integration.exe", "generate --prompt=Hello, LLM!");
        generateResult.WaitForExit();
        Console.WriteLine("生成响应命令执行完成");
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
        Console.WriteLine("LLM Channel 事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建 Channel
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        
        // 启动处理器
        var processorTask = ProcessPromptsAsync(channel.Reader);
        
        // 发送提示
        for (int i = 0; i < 5; i++)
        {
            var prompt = $"Prompt {i}: Hello, LLM!";
            await channel.Writer.WriteAsync(prompt);
            Console.WriteLine($"发送提示: {prompt}
