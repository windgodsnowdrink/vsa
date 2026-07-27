#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.Channels@8.0.0
#:package System.Runtime.CompilerServices.Unsafe@6.0.0
#:package System.Buffers@4.5.1
#:package System.Memory@4.5.5
#:package System.Collections.Immutable@8.0.0
#:package MessagePack@2.5.129
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property RuntimeIdentifier=win-x64
#:property EnableCompilationRelaxations=true
#:property EnableAggressiveOptimization=true

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("高频处理 AOT 引擎");
        Console.WriteLine("=" * 60);
        
        var serviceProvider = BuildServiceProvider();
        var highFrequencyService = serviceProvider.GetRequiredService<HighFrequencyService>();
        var settings = serviceProvider.GetRequiredService<IOptions<HighFrequencySettings>>().Value;
        
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        
        try
        {
            switch (command)
            {
                case "run":
                case "r":
                    await RunHighFrequencyTest(highFrequencyService, settings);
                    break;
                case "benchmark":
                case "b":
                    await RunBenchmark(highFrequencyService, settings);
                    break;
                case "zero-copy":
                case "zc":
                    await RunZeroCopyTest(highFrequencyService, settings);
                    break;
                case "pipeline":
                case "pl":
                    await RunPipelineTest(highFrequencyService, settings);
                    break;
                case "channel":
                case "ch":
                    await RunChannelTest(highFrequencyService, settings);
                    break;
                case "config":
                case "c":
                    ShowConfig(settings);
                    break;
                case "help":
                case "h":
                case "?":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine($"未知命令: {command}");
                    ShowHelp();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            await highFrequencyService.DisposeAsync();
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<HighFrequencySettings>(options => {
            options.MessageCount = 1_000_000;
            options.ConcurrencyLevel = Environment.ProcessorCount;
            options.BufferSize = 65536;
            options.EnableZeroCopy = true;
            options.EnableBatchProcessing = true;
            options.BatchSize = 1000;
            options.EnableCompression = false;
            options.EnableMetrics = true;
        });
        
        services.AddSingleton<HighFrequencyService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
    
    private static async Task RunHighFrequencyTest(HighFrequencyService service, HighFrequencySettings settings)
    {
        Console.WriteLine("运行高频处理测试...");
        Console.WriteLine($"消息数量: {settings.MessageCount:N0}");
        Console.WriteLine($"并发级别: {settings.ConcurrencyLevel}");
        Console.WriteLine($"缓冲区大小: {settings.BufferSize}");
        Console.WriteLine($"启用零拷贝: {settings.EnableZeroCopy}");
        Console.WriteLine($"启用批处理: {settings.EnableBatchProcessing}");
        Console.WriteLine($"批处理大小: {settings.BatchSize}");
        Console.WriteLine();
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ProcessMessagesAsync(settings.MessageCount);
        stopwatch.Stop();
        
        Console.WriteLine("测试完成!");
        Console.WriteLine($"处理时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理消息: {result.ProcessedMessages:N0}");
        Console.WriteLine($"吞吐量: {result.Throughput:F2} msg/s");
        Console.WriteLine($"平均延迟: {result.AverageLatency:F3} μs");
        Console.WriteLine($"最大延迟: {result.MaxLatency:F3} μs");
        Console.WriteLine($"最小延迟: {result.MinLatency:F3} μs");
    }
    
    private static async Task RunBenchmark(HighFrequencyService service, HighFrequencySettings settings)
    {
        Console.WriteLine("运行性能基准测试...");
        Console.WriteLine();
        
        var messageCounts = new[] { 100_000, 500_000, 1_000_000 };
        
        foreach (var count in messageCounts)
        {
            Console.WriteLine($"测试消息数量: {count:N0}");
            var stopwatch = Stopwatch.StartNew();
            var result = await service.ProcessMessagesAsync(count);
            stopwatch.Stop();
            
            Console.WriteLine($"  处理时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
            Console.WriteLine($"  吞吐量: {result.Throughput:F2} msg/s");
            Console.WriteLine($"  平均延迟: {result.AverageLatency:F3} μs");
            Console.WriteLine();
        }
    }
    
    private static async Task RunZeroCopyTest(HighFrequencyService service, HighFrequencySettings settings)
    {
        Console.WriteLine("运行零拷贝测试...");
        Console.WriteLine();
        
        // 测试启用零拷贝
        settings.EnableZeroCopy = true;
        var stopwatch1 = Stopwatch.StartNew();
        var result1 = await service.ProcessMessagesAsync(1_000_000);
        stopwatch1.Stop();
        
        Console.WriteLine("启用零拷贝:");
        Console.WriteLine($"  处理时间: {stopwatch1.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  吞吐量: {result1.Throughput:F2} msg/s");
        Console.WriteLine();
        
        // 测试禁用零拷贝
        settings.EnableZeroCopy = false;
        var stopwatch2 = Stopwatch.StartNew();
        var result2 = await service.ProcessMessagesAsync(1_000_000);
        stopwatch2.Stop();
        
        Console.WriteLine("禁用零拷贝:");
        Console.WriteLine($"  处理时间: {stopwatch2.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"  吞吐量: {result2.Throughput:F2} msg/s");
        Console.WriteLine();
        
        var improvement = ((result1.Throughput - result2.Throughput) / result2.Throughput) * 100;
        Console.WriteLine($"性能提升: {improvement:F2}%");
    }
    
    private static async Task RunPipelineTest(HighFrequencyService service, HighFrequencySettings settings)
    {
        Console.WriteLine("运行管道测试...");
        Console.WriteLine();
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ProcessMessagesWithPipelineAsync(settings.MessageCount);
        stopwatch.Stop();
        
        Console.WriteLine("管道测试完成!");
        Console.WriteLine($"处理时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理消息: {result.ProcessedMessages:N0}");
        Console.WriteLine($"吞吐量: {result.Throughput:F2} msg/s");
    }
    
    private static async Task RunChannelTest(HighFrequencyService service, HighFrequencySettings settings)
    {
        Console.WriteLine("运行通道测试...");
        Console.WriteLine();
        
        var stopwatch = Stopwatch.StartNew();
        var result = await service.ProcessMessagesWithChannelAsync(settings.MessageCount);
        stopwatch.Stop();
        
        Console.WriteLine("通道测试完成!");
        Console.WriteLine($"处理时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"处理消息: {result.ProcessedMessages:N0}");
        Console.WriteLine($"吞吐量: {result.Throughput:F2} msg/s");
    }
    
    private static void ShowConfig(HighFrequencySettings settings)
    {
        Console.WriteLine("高频处理配置:");
        Console.WriteLine("=" * 60);
        Console.WriteLine($"消息数量: {settings.MessageCount:N0}");
        Console.WriteLine($"并发级别: {settings.ConcurrencyLevel}");
        Console.WriteLine($"缓冲区大小: {settings.BufferSize}");
        Console.WriteLine($"启用零拷贝: {settings.EnableZeroCopy}");
        Console.WriteLine($"启用批处理: {settings.EnableBatchProcessing}");
        Console.WriteLine($"批处理大小: {settings.BatchSize}");
        Console.WriteLine($"启用压缩: {settings.EnableCompression}");
        Console.WriteLine($"启用指标: {settings.EnableMetrics}");
    }
    
    private static void ShowHelp()
    {
        Console.WriteLine("高频处理 AOT 引擎 命令帮助:");
        Console.WriteLine("=" * 60);
        Console.WriteLine("run (r)        - 运行高频处理测试");
        Console.WriteLine("benchmark (b)  - 运行性能基准测试");
        Console.WriteLine("zero-copy (zc) - 运行零拷贝测试");
        Console.WriteLine("pipeline (pl)  - 运行管道测试");
        Console.WriteLine("channel (ch)   - 运行通道测试");
        Console.WriteLine("config (c)     - 显示配置信息");
        Console.WriteLine("help (h, ?)    - 显示帮助信息");
    }
}

public class HighFrequencySettings
{
    public int MessageCount { get; set; } = 1_000_000;
    public int ConcurrencyLevel { get; set; } = Environment.ProcessorCount;
    public int BufferSize { get; set; } = 65536;
    public bool EnableZeroCopy { get; set; } = true;
    public bool EnableBatchProcessing { get; set; } = true;
    public int BatchSize { get; set; } = 1000;
    public bool EnableCompression { get; set; } = false;
    public bool EnableMetrics { get; set; } = true;
}

public class ProcessingResult
{
    public long ProcessedMessages { get; set; }
    public double Throughput { get; set; }
    public double AverageLatency { get; set; }
    public double MaxLatency { get; set; }
    public double MinLatency { get; set; }
}

public class HighFrequencyService : IAsyncDisposable
{
    private readonly ILogger<HighFrequencyService> _logger;
    private readonly Channel<byte[]> _channel;
    private readonly List<Task> _workerTasks = new();
    private readonly CancellationTokenSource _cts = new();
    
    public HighFrequencyService(ILogger<HighFrequencyService> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
    }
    
    public async Task<ProcessingResult> ProcessMessagesAsync(long messageCount)
    {
        var processed = 0L;
        var latencies = new List<double>();
        var stopwatch = Stopwatch.StartNew();
        
        for (long i = 0; i < messageCount; i++)
        {
            var startTime = Stopwatch.GetTimestamp();
            
            // 模拟消息处理
            var message = CreateMessage(i);
            ProcessMessage(message);
            
            var endTime = Stopwatch.GetTimestamp();
            var latency = (endTime - startTime) * 1000000.0 / Stopwatch.Frequency;
            latencies.Add(latency);
            processed++;
        }
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        
        return new ProcessingResult
        {
            ProcessedMessages = processed,
            Throughput = processed / elapsedSeconds,
            AverageLatency = latencies.Average(),
            MaxLatency = latencies.Max(),
            MinLatency = latencies.Min()
        };
    }
    
    public async Task<ProcessingResult> ProcessMessagesWithPipelineAsync(long messageCount)
    {
        var processed = 0L;
        var stopwatch = Stopwatch.StartNew();
        
        var pipe = new Pipe();
        var writeTask = WriteToPipeAsync(pipe.Writer, messageCount, _cts.Token);
        var readTask = ReadFromPipeAsync(pipe.Reader, _cts.Token, count => processed = count);
        
        await Task.WhenAll(writeTask, readTask);
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        
        return new ProcessingResult
        {
            ProcessedMessages = processed,
            Throughput = processed / elapsedSeconds,
            AverageLatency = 0,
            MaxLatency = 0,
            MinLatency = 0
        };
    }
    
    public async Task<ProcessingResult> ProcessMessagesWithChannelAsync(long messageCount)
    {
        var processed = 0L;
        var stopwatch = Stopwatch.StartNew();
        
        // 启动工作线程
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            _workerTasks.Add(WorkerAsync(_cts.Token, count => processed += count));
        }
        
        // 发送消息
        for (long i = 0; i < messageCount; i++)
        {
            var message = CreateMessage(i);
            await _channel.Writer.WriteAsync(message, _cts.Token);
        }
        
        // 完成写入
        _channel.Writer.Complete();
        
        // 等待所有工作线程完成
        await Task.WhenAll(_workerTasks);
        
        stopwatch.Stop();
        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        
        return new ProcessingResult
        {
            ProcessedMessages = processed,
            Throughput = processed / elapsedSeconds,
            AverageLatency = 0,
            MaxLatency = 0,
            MinLatency = 0
        };
    }
    
    private async Task WriteToPipeAsync(PipeWriter writer, long messageCount, CancellationToken cancellationToken)
    {
        try
        {
            for (long i = 0; i < messageCount; i++)
            {
                var message = CreateMessage(i);
                
                var memory = writer.GetMemory(message.Length);
                message.CopyTo(memory.Span);
                writer.Advance(message.Length);
                
                var flushResult = await writer.FlushAsync(cancellationToken);
                if (flushResult.IsCompleted)
                {
                    break;
                }
            }
        }
        finally
        {
            await writer.CompleteAsync();
        }
    }
    
    private async Task ReadFromPipeAsync(PipeReader reader, CancellationToken cancellationToken, Action<long> onProcessed)
    {
        long processed = 0;
        try
        {
            while (true)
            {
                var result = await reader.ReadAsync(cancellationToken);
                var buffer = result.Buffer;
                
                // 处理所有消息
                while (TryReadMessage(ref buffer, out var message))
                {
                    ProcessMessage(message.ToArray());
                    processed++;
                }
                
                reader.AdvanceTo(buffer.Start, buffer.End);
                
                if (result.IsCompleted)
                {
                    break;
                }
            }
        }
        finally
        {
            await reader.CompleteAsync();
            onProcessed(processed);
        }
    }
    
    private async Task WorkerAsync(CancellationToken cancellationToken, Action<long> onProcessed)
    {
        long processed = 0;
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                ProcessMessage(message);
                processed++;
            }
        }
        finally
        {
            onProcessed(processed);
        }
    }
    
    private byte[] CreateMessage(long id)
    {
        // 创建一个简单的消息
        var message = new byte[64];
        BitConverter.GetBytes(id).CopyTo(message, 0);
        return message;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessMessage(byte[] message)
    {
        // 模拟消息处理
        var id = BitConverter.ToInt64(message, 0);
        // 这里可以添加更复杂的处理逻辑
    }
    
    private bool TryReadMessage(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> message)
    {
        // 简单的消息读取逻辑
        if (buffer.Length >= 64)
        {
            message = buffer.Slice(0, 64);
            buffer = buffer.Slice(64);
            return true;
        }
        
        message = default;
        return false;
    }
    
    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _cts.Dispose();
        
        // 等待所有工作线程完成
        if (_workerTasks.Count > 0)
        {
            await Task.WhenAll(_workerTasks);
        }
        
        _channel.Writer.TryComplete();
        GC.SuppressFinalize(this);
    }
}
