# Cscore AOT - 使用示例

## 快速开始

### 1. 基本音频处理示例

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cscore.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Cscore AOT 基本音频处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 配置Cscore选项
        builder.Services.Configure<CscoreOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.SampleRate = 44100;
        });
        
        // 配置日志
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        // 注册服务
        builder.Services.AddSingleton<ICscoreService, CscoreService>();
        builder.Services.AddSingleton<CscoreAotEngine>();
        
        // 构建主机并获取服务
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CscoreAotEngine>();
        
        try
        {
            // 准备测试音频数据（这里使用空数据模拟）
            byte[] audioData = new byte[44100 * 2 * 2]; // 1秒44.1kHz立体声16位音频
            
            // 创建处理选项
            var options = new AudioProcessingOptions
            {
                ProcessingType = "volume",
                VolumeGain = 3.0f
            };
            
            Console.WriteLine($"处理选项: 类型={options.ProcessingType}, 音量增益={options.VolumeGain}dB");
            Console.WriteLine($"音频数据大小: {audioData.Length} 字节");
            
            // 执行音频处理
            var result = await engine.ProcessAudioAsync(audioData, options);
            
            // 显示结果
            Console.WriteLine($"\n处理结果: 成功={result.Success}");
            Console.WriteLine($"执行时间: {result.ExecutionTimeMs} 毫秒");
            
            if (result.SizeInfo != null)
            {
                Console.WriteLine($"原始大小: {result.SizeInfo.OriginalSize} 字节");
                Console.WriteLine($"处理后大小: {result.SizeInfo.ProcessedSize} 字节");
                Console.WriteLine($"压缩率: {result.SizeInfo.CompressionRatio:F2}%");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        Console.WriteLine("\n示例完成！");
    }
}
```

### 2. 批量音频处理示例

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cscore.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Cscore AOT 批量音频处理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 配置服务
        builder.Services.Configure<CscoreOptions>(options => {
            options.EnableCache = true;
            options.WorkerCount = Environment.ProcessorCount;
        });
        
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        builder.Services.AddSingleton<ICscoreService, CscoreService>();
        builder.Services.AddSingleton<CscoreAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CscoreAotEngine>();
        
        try
        {
            // 准备批量处理的音频数据
            var audioDataList = new List<byte[]>();
            int fileCount = 5;
            
            for (int i = 0; i < fileCount; i++)
            {
                // 创建不同大小的测试音频数据
                int size = 44100 * 2 * 2 * (i + 1); // 1-5秒音频
                audioDataList.Add(new byte[size]);
            }
            
            Console.WriteLine($"批量处理 {fileCount} 个音频文件");
            Console.WriteLine($"单个文件大小范围: 1-5秒");
            
            // 批量处理选项
            var options = new AudioProcessingOptions
            {
                ProcessingType = "noise_reduction",
                EnableNoiseReduction = true,
                NoiseReductionLevel = 0.5f
            };
            
            // 执行批量处理
            var results = await engine.ProcessAudioBatchAsync(audioDataList, options);
            
            // 统计结果
            int successCount = results.Count(r => r.Success);
            long totalExecutionTime = results.Sum(r => r.ExecutionTimeMs);
            
            Console.WriteLine($"\n批量处理结果:");
            Console.WriteLine($"总文件数: {results.Count}");
            Console.WriteLine($"成功: {successCount}");
            Console.WriteLine($"失败: {results.Count - successCount}");
            Console.WriteLine($"总执行时间: {totalExecutionTime} 毫秒");
            Console.WriteLine($"平均执行时间: {totalExecutionTime / results.Count} 毫秒");
            
            // 显示每个文件的结果
            Console.WriteLine("\n每个文件的处理结果:");
            for (int i = 0; i < results.Count; i++)
            {
                var result = results.ElementAt(i);
                Console.WriteLine($"文件 {i+1}: 成功={result.Success}, 时间={result.ExecutionTimeMs}ms");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        Console.WriteLine("\n示例完成！");
    }
}
```

### 3. 音频设备管理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cscore.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Cscore AOT 音频设备管理示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 配置服务
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        builder.Services.AddSingleton<ICscoreService, CscoreService>();
        builder.Services.AddSingleton<CscoreAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CscoreAotEngine>();
        
        try
        {
            // 获取音频设备列表
            var devices = await engine.GetAudioDevicesAsync();
            
            Console.WriteLine($"找到 {devices.Count()} 个音频设备");
            Console.WriteLine();
            
            // 显示每个设备的信息
            int deviceIndex = 1;
            foreach (var device in devices)
            {
                Console.WriteLine($"设备 {deviceIndex}:");
                Console.WriteLine($"  ID: {device.DeviceId}");
                Console.WriteLine($"  名称: {device.DeviceName}");
                Console.WriteLine($"  类型: {device.DeviceType}");
                Console.WriteLine($"  支持的采样率: {string.Join(", ", device.SupportedSampleRates)}");
                Console.WriteLine($"  支持的通道数: {string.Join(", ", device.SupportedChannelCounts)}");
                Console.WriteLine();
                deviceIndex++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        Console.WriteLine("示例完成！");
    }
}
```

### 4. 状态监控和重置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cscore.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Cscore AOT 状态监控和重置示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 配置服务
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        builder.Services.AddSingleton<ICscoreService, CscoreService>();
        builder.Services.AddSingleton<CscoreAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CscoreAotEngine>();
        
        try
        {
            // 1. 获取初始状态
            Console.WriteLine("1. 获取初始状态:");
            var initialStatus = await engine.GetStatusAsync();
            DisplayStatus(initialStatus);
            
            // 2. 执行一些操作，生成状态数据
            Console.WriteLine("\n2. 执行一些操作...");
            
            // 模拟处理一些音频数据
            for (int i = 0; i < 3; i++)
            {
                byte[] audioData = new byte[44100 * 2 * 2];
                var options = new AudioProcessingOptions { ProcessingType = "default" };
                await engine.ProcessAudioAsync(audioData, options);
            }
            
            // 3. 获取处理后的状态
            Console.WriteLine("\n3. 获取处理后的状态:");
            var processedStatus = await engine.GetStatusAsync();
            DisplayStatus(processedStatus);
            
            // 4. 重置状态
            Console.WriteLine("\n4. 重置状态...");
            var resetResult = await engine.ResetStatusAsync();
            Console.WriteLine($"重置结果: 成功={resetResult}");
            
            // 5. 获取重置后的状态
            Console.WriteLine("\n5. 获取重置后的状态:");
            var resetStatus = await engine.GetStatusAsync();
            DisplayStatus(resetStatus);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
        
        Console.WriteLine("\n示例完成！");
    }
    
    private static void DisplayStatus(CscoreStatus status)
    {
        Console.WriteLine($"  运行状态: {status.IsRunning}");
        Console.WriteLine($"  已处理文件数: {status.ProcessedFiles}");
        Console.WriteLine($"  成功文件数: {status.SuccessfulFiles}");
        Console.WriteLine($"  失败文件数: {status.FailedFiles}");
        Console.WriteLine($"  缓存命中率: {status.CacheHitRate}%");
        Console.WriteLine($"  平均处理时间: {status.AverageProcessingTimeMs} ms");
        Console.WriteLine($"  启动时间: {status.StartTime.ToLocalTime()}");
    }
}
```

## 总结

以上示例演示了Cscore AOT的主要功能和使用方法，包括：

1. 基本音频处理 - 如何使用Cscore AOT引擎处理单个音频文件
2. 批量音频处理 - 如何高效处理多个音频文件
3. 音频设备管理 - 如何获取和管理系统音频设备
4. 状态监控和重置 - 如何监控Cscore AOT的运行状态和重置状态

这些示例遵循了.NET 10最佳实践，采用了依赖注入、选项模式、异步编程等现代.NET技术，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目使用。

通过这些示例，您可以快速上手使用Cscore AOT，并根据自己的需求进行扩展和定制。
