# CsharpFlink AOT - 使用示例

## 快速开始

### 1. 基本作业执行示例

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CsharpFlink.AOT;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("CsharpFlink AOT 基本作业执行示例");
        Console.WriteLine("=" * 50);
        
        // 构建主机
        var builder = Host.CreateApplicationBuilder(args);
        
        // 配置CsharpFlink选项
        builder.Services.Configure<CsharpFlinkOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.Parallelism = Environment.ProcessorCount;
        });
        
        // 配置日志
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        // 注册服务
        builder.Services.AddSingleton<ICsharpFlinkService, CsharpFlinkService>();
        builder.Services.AddSingleton<CsharpFlinkAotEngine>();
        
        // 构建主机并获取服务
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CsharpFlinkAotEngine>();
        
        try
        {
            // 创建作业配置
            var jobConfig = new FlinkJobConfig
            {
                JobName = "BasicStreamingJob",
                JobType = "streaming",
                InputSource = "input.txt",
                OutputSink = "output.txt"
            };
            
            Console.WriteLine($"作业配置: 名称={jobConfig.JobName}, 类型={jobConfig.JobType}");
            Console.WriteLine($"输入源: {jobConfig.InputSource}, 输出目的地: {jobConfig.OutputSink}");
            
            // 执行作业
            var result = await engine.ExecuteJobAsync(jobConfig);
            
            // 显示结果
            Console.WriteLine($"\n作业执行结果: 成功={result.Success}");
            Console.WriteLine($"JobId: {result.JobId}");
            Console.WriteLine($"执行时间: {result.ExecutionTimeMs} 毫秒");
            Console.WriteLine($"作业状态: {result.JobStatus}