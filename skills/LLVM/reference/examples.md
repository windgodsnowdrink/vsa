# LLVM - 使用示例

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
        var llvmService = serviceProvider.GetRequiredService<ILLVMService>();
        
        Console.WriteLine("LLVM 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 使用 LLVM 功能
        var optimizedIR = await llvmService.OptimizeIRAsync("function test() { return 42; }");
        Console.WriteLine($"优化后的 IR: {optimizedIR}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<ILLVMProcessor, LLVMProcessor>();
        builder.AddSingleton<ILLVMService, LLVMService>();
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
        Console.WriteLine("LLVM 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 LLVM 设置
        builder.Configure<LLVMSetting>(options => {
            options.EnableOptimization = true;
            options.OptimizationLevel = 3;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddSingleton<ILLVMProcessor, LLVMProcessor>();
        builder.AddSingleton<ILLVMService, LLVMService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<LLVMSetting>>().Value;
        Console.WriteLine($"配置: 优化={settings.EnableOptimization}, 优化级别={settings.OptimizationLevel}");
        Console.WriteLine($"超时={settings.Timeout}");
        
        // 使用服务
        var llvmService = serviceProvider.GetRequiredService<ILLVMService>();
        var optimizedIR = await llvmService.OptimizeIRAsync("function test() { return 42; }");
        Console.WriteLine($"优化后的 IR: {optimizedIR}");
    }
}
`

### 3. AOT 架构执行示例

#### 3.1 AOT 编译示例

`bash
# AOT 编译命令
dotnet publish scripts/llvm_ir_optimizer.cs -c Release -r win-x64 --aot

# 运行编译后的程序
./llvm_ir_optimizer.exe
`

#### 3.2 AOT 运行示例

`csharp
using System;
using System.Diagnostics;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("LLVM AOT 运行示例");
        Console.WriteLine("=" * 50);
        
        // 启动 LLVM 服务
        Process.Start("./llvm_ir_optimizer.exe", "start");
        Console.WriteLine("LLVM 服务启动成功");
        
        // 优化 IR
        var optimizeResult = Process.Start("./llvm_ir_optimizer.exe", "optimize --ir=function test() { return 42; }");
        optimizeResult.WaitForExit();
        Console.WriteLine("优化 IR 命令执行完成");
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
        Console.WriteLine("LLVM Channel 事件处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建 Channel
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true
        });
        
        // 启动处理器
        var processorTask = ProcessIRAsync(channel.Reader);
        
        // 发送 IR 代码
        for (int i = 0; i < 5; i++)
        {
            var irCode = $"function test{i}() {{ return {i}; }}";
            await channel.Writer.WriteAsync(irCode);
            Console.WriteLine($"发送 IR 代码: {irCode}
