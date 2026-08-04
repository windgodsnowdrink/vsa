# NodaTime - 使用示例

## 快速入门

### 1. 基本用法示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Extensions;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        var nodaTimeCalculator = serviceProvider.GetRequiredService<INodaTimeCalculator>();
        
        Console.WriteLine("NodaTime 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 使用核心功能
        var now = nodaTimeService.GetCurrentDateTime();
        Console.WriteLine($"当前时间: {now}");
        
        // 计算两个时间之间的差异
        var start = new LocalDateTime(2024, 1, 1, 0, 0, 0);
        var end = new LocalDateTime(2024, 1, 2, 12, 0, 0);
        var duration = nodaTimeCalculator.CalculateDuration(start, end);
        Console.WriteLine($"时间差异: {duration}");
        
        // 时区转换
        var utcTime = Instant.FromUtc(2024, 1, 1, 0, 0);
        var localTime = nodaTimeService.ConvertToLocalTime(utcTime, "Asia/Shanghai");
        Console.WriteLine($"UTC 时间: {utcTime}");
        Console.WriteLine($"上海时间: {localTime}");
        
        // 格式化时间
        var formattedTime = nodaTimeService.FormatDateTime(now, "yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"格式化时间: {formattedTime}");
        
        // 解析时间
        var parsedTime = nodaTimeService.ParseDateTime("2024-01-01 12:00:00", "yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"解析时间: {parsedTime}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 选项
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "UTC";
            options.EnableCache = true;
            options.CacheSize = 1000;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 2. 高级配置示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NodaTime;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 设置
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "Asia/Shanghai";
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<NodaTimeOptions>>().Value;
        Console.WriteLine($"配置: 默认时区={settings.DefaultTimeZone}, 缓存={settings.EnableCache}");
        Console.WriteLine($"缓存大小={settings.CacheSize}, 超时={settings.Timeout}");
        
        // 使用服务
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        var now = nodaTimeService.GetCurrentDateTime();
        Console.WriteLine($"当前时间: {now}");
        
        // 测试时区转换
        var utcTime = Instant.FromUtc(2024, 1, 1, 0, 0);
        var localTime = nodaTimeService.ConvertToLocalTime(utcTime, settings.DefaultTimeZone);
        Console.WriteLine($"UTC 时间: {utcTime}");
        Console.WriteLine($"本地时间 ({settings.DefaultTimeZone}): {localTime}");
    }
}
`

### 3. 性能优化示例

`csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        var nodaTimeCalculator = serviceProvider.GetRequiredService<INodaTimeCalculator>();
        
        // 性能测试
        const int iterations = 10000;
        var stopwatch = Stopwatch.StartNew();
        
        // 预热
        await nodaTimeService.GetCurrentDateTimeAsync();
        
        // 测试获取当前时间
        stopwatch.Restart();
        for (int i = 0; i < iterations; i++)
        {
            await nodaTimeService.GetCurrentDateTimeAsync();
        }
        stopwatch.Stop();
        Console.WriteLine($"获取当前时间 ({iterations} 次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 测试时区转换
        var utcTime = Instant.FromUtc(2024, 1, 1, 0, 0);
        stopwatch.Restart();
        for (int i = 0; i < iterations; i++)
        {
            await nodaTimeService.ConvertToLocalTimeAsync(utcTime, "Asia/Shanghai");
        }
        stopwatch.Stop();
        Console.WriteLine($"时区转换 ({iterations} 次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 测试时间计算
        var start = new LocalDateTime(2024, 1, 1, 0, 0, 0);
        var end = new LocalDateTime(2024, 1, 2, 12, 0, 0);
        stopwatch.Restart();
        for (int i = 0; i < iterations; i++)
        {
            nodaTimeCalculator.CalculateDuration(start, end);
        }
        stopwatch.Stop();
        Console.WriteLine($"时间计算 ({iterations} 次): {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        
        // 内存使用情况
        var memoryInfo = GC.GetTotalMemory(true);
        Console.WriteLine($"内存使用: {memoryInfo / 1024 / 1024:F2} MB");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 选项（启用缓存）
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "UTC";
            options.EnableCache = true;
            options.CacheSize = 5000;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Error));
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 4. 错误处理示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        
        try
        {
            // 尝试解析无效的日期时间
            var invalidDate = "2024-13-01 12:00:00"; // 无效的月份
            var parsedTime = nodaTimeService.ParseDateTime(invalidDate, "yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"解析时间: {parsedTime}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"格式错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 尝试使用无效的时区
            var utcTime = Instant.FromUtc(2024, 1, 1, 0, 0);
            var localTime = nodaTimeService.ConvertToLocalTime(utcTime, "Invalid/TimeZone");
            Console.WriteLine($"本地时间: {localTime}");
        }
        catch (DateTimeZoneNotFoundException ex)
        {
            Console.WriteLine($"时区未找到: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"通用错误: {ex.Message}");
        }
        
        try
        {
            // 尝试计算无效的时间差异
            var start = new LocalDateTime(2024, 1, 2, 0, 0, 0);
            var end = new LocalDateTime(2024, 1, 1, 0, 0, 0); // 结束时间早于开始时间
            var duration = serviceProvider.GetRequiredService<INodaTimeCalculator>().CalculateDuration(start, end);
            Console.WriteLine($"时间差异: {duration}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"计算错误: {ex.Message}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 选项
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "UTC";
            options.EnableCache = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 5. 自定义时间计算器示例

`csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;

// 自定义时间计算器
public class CustomNodaTimeCalculator : INodaTimeCalculator
{
    private readonly ILogger<CustomNodaTimeCalculator> _logger;
    private readonly NodaTimeOptions _options;

    public CustomNodaTimeCalculator(ILogger<CustomNodaTimeCalculator> logger, IOptions<NodaTimeOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Duration CalculateDuration(LocalDateTime start, LocalDateTime end)
    {
        _logger.LogInformation($"计算时间差异: {start} 到 {end}");
        return end - start;
    }

    public LocalDateTime AddDuration(LocalDateTime dateTime, Duration duration)
    {
        _logger.LogInformation($"添加时间: {dateTime} + {duration}");
        return dateTime + duration;
    }

    public LocalDateTime SubtractDuration(LocalDateTime dateTime, Duration duration)
    {
        _logger.LogInformation($"减去时间: {dateTime} - {duration}");
        return dateTime - duration;
    }

    public bool IsInRange(LocalDateTime dateTime, LocalDateTime start, LocalDateTime end)
    {
        return dateTime >= start && dateTime <= end;
    }

    // 自定义方法：计算两个日期之间的工作日数
    public int CalculateWorkingDays(LocalDate start, LocalDate end)
    {
        _logger.LogInformation($"计算工作日数: {start} 到 {end}");
        int workingDays = 0;
        LocalDate current = start;
        
        while (current <= end)
        {
            // 排除周末
            if (current.DayOfWeek != IsoDayOfWeek.Saturday && current.DayOfWeek != IsoDayOfWeek.Sunday)
            {
                workingDays++;
            }
            current = current.PlusDays(1);
        }
        
        return workingDays;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 自定义时间计算器示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var customCalculator = serviceProvider.GetRequiredService<INodaTimeCalculator>();
        
        // 测试自定义方法
        var startDate = new LocalDate(2024, 1, 1);
        var endDate = new LocalDate(2024, 1, 31);
        var workingDays = ((CustomNodaTimeCalculator)customCalculator).CalculateWorkingDays(startDate, endDate);
        Console.WriteLine($"2024年1月工作日数: {workingDays}");
        
        // 测试基本方法
        var startTime = new LocalDateTime(2024, 1, 1, 9, 0, 0);
        var endTime = new LocalDateTime(2024, 1, 1, 17, 30, 0);
        var duration = customCalculator.CalculateDuration(startTime, endTime);
        Console.WriteLine($"工作时间长度: {duration}");
        
        // 测试时间加减
        var futureTime = customCalculator.AddDuration(startTime, Duration.FromHours(2));
        Console.WriteLine($"2小时后: {futureTime}");
        
        var pastTime = customCalculator.SubtractDuration(startTime, Duration.FromMinutes(30));
        Console.WriteLine($"30分钟前: {pastTime}");
        
        // 测试时间范围
        var testTime = new LocalDateTime(2024, 1, 1, 12, 0, 0);
        var isInRange = customCalculator.IsInRange(testTime, startTime, endTime);
        Console.WriteLine($"12:00 是否在工作时间内: {isInRange}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 选项
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "UTC";
            options.EnableCache = true;
        });
        
        // 注册服务
        builder.AddLogging(builder => builder.AddConsole());
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, CustomNodaTimeCalculator>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 6. 序列化示例

`csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

public class Event
{
    public string Name { get; set; }
    public Instant Timestamp { get; set; }
    public LocalDate Date { get; set; }
    public LocalTime Time { get; set; }
    public ZonedDateTime ZonedDateTime { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 序列化示例");
        Console.WriteLine("=" * 50);
        
        // 配置 JSON 序列化
        var options = new JsonSerializerOptions();
        options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
        options.WriteIndented = true;
        
        // 创建测试对象
        var now = SystemClock.Instance.GetCurrentInstant();
        var eventObj = new Event
        {
            Name = "测试事件",
            Timestamp = now,
            Date = now.InZone(DateTimeZoneProviders.Tzdb["Asia/Shanghai"]).Date,
            Time = now.InZone(DateTimeZoneProviders.Tzdb["Asia/Shanghai"]).TimeOfDay,
            ZonedDateTime = now.InZone(DateTimeZoneProviders.Tzdb["Asia/Shanghai"])
        };
        
        // 序列化
        var json = JsonSerializer.Serialize(eventObj, options);
        Console.WriteLine("序列化结果:");
        Console.WriteLine(json);
        
        // 反序列化
        var deserializedEvent = JsonSerializer.Deserialize<Event>(json, options);
        Console.WriteLine("\n反序列化结果:");
        Console.WriteLine($"名称: {deserializedEvent.Name}");
        Console.WriteLine($"时间戳: {deserializedEvent.Timestamp}");
        Console.WriteLine($"日期: {deserializedEvent.Date}");
        Console.WriteLine($"时间: {deserializedEvent.Time}");
        Console.WriteLine($"带时区的日期时间: {deserializedEvent.ZonedDateTime}");
    }
}
`

## 性能优化最佳实践

### 1. 内存优化示例

`csharp
using System;
using System.Buffers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 内存优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        
        // 使用内存池
        var pool = ArrayPool<byte>.Shared;
        var buffer = pool.Rent(1024); // 1KB 缓冲区
        
        try
        {
            // 模拟处理大量日期时间数据
            const int iterations = 10000;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < iterations; i++)
            {
                // 获取当前时间
                var now = nodaTimeService.GetCurrentDateTime();
                
                // 格式化时间到缓冲区
                var formatted = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                var bytesWritten = System.Text.Encoding.UTF8.GetBytes(formatted, 0, formatted.Length, buffer, 0);
                
                // 模拟处理缓冲区数据
                // ...
            }
            
            stopwatch.Stop();
            Console.WriteLine($"处理 {iterations} 次时间格式化: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        }
        finally
        {
            // 归还缓冲区
            pool.Return(buffer);
        }
        
        // 检查内存使用
        var memoryInfo = GC.GetTotalMemory(true);
        Console.WriteLine($"内存使用: {memoryInfo / 1024 / 1024:F2} MB");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 选项
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "UTC";
            options.EnableCache = true;
        });
        
        // 注册服务
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();
        
        return builder.BuildServiceProvider();
    }
}
`

### 2. 并发优化示例

`csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("NodaTime 并发优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
        
        // 创建通道
        var channel = Channel.CreateUnbounded<DateTimeTask>();
        
        // 生产者
        var producerTask = Task.Run(async () => {
            for (int i = 0; i < 1000; i++)
            {
                await channel.Writer.WriteAsync(new DateTimeTask { Id = i, Timestamp = Instant.FromUtc(2024, 1, 1, 0, i) });
                await Task.Delay(1);
            }
            channel.Writer.Complete();
        });
        
        // 消费者
        var consumerTask = Task.Run(async () => {
            var tasks = new List<Task>();
            var results = new List<DateTimeResult>();
            
            await foreach (var task in channel.Reader.ReadAllAsync())
            {
                // 并行处理
                tasks.Add(Task.Run(async () => {
                    // 时区转换
                    var localTime = await nodaTimeService.ConvertToLocalTimeAsync(task.Timestamp, "Asia/Shanghai");
                    // 格式化时间
                    var formattedTime = nodaTimeService.FormatDateTime(localTime, "yyyy-MM-dd HH:mm:ss");
                    
                    lock (results)
                    {
                        results.Add(new DateTimeResult { Id = task.Id, FormattedTime = formattedTime });
                    }
                }));
                
                // 限制并发数
                if (tasks.Count >= 50)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }
            }
            
            // 处理剩余任务
            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }
            
            Console.WriteLine($"处理完成 {results.Count} 个时间任务");
            // 打印前10个结果
            foreach (var result in results.Take(10))
            {
                Console.WriteLine($"任务 {result.Id}: {result.FormattedTime}");
            }
        });
        
        // 等待完成
        await Task.WhenAll(producerTask, consumerTask);
        Console.WriteLine("所有任务处理完成");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 配置 NodaTime 选项
        builder.Configure<NodaTimeOptions>(options => {
            options.DefaultTimeZone = "UTC";
            options.EnableCache = true;
        });
        
        // 注册服务
        builder.AddSingleton<INodaTimeService, NodaTimeService>();
        builder.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();
        
        return builder.BuildServiceProvider();
    }
    
    private class DateTimeTask
    {
        public int Id { get; set; }
        public Instant Timestamp { get; set; }
    }
    
    private class DateTimeResult
    {
        public int Id { get; set; }
        public string FormattedTime { get; set; }
    }
}
`

## 总结

以上示例展示了 NodaTime 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作，如获取当前时间、计算时间差异、时区转换等
2. 配置高级选项，如默认时区、缓存大小、超时设置等
3. 优化性能，如使用缓存、内存池、并发处理等
4. 处理错误情况，如日期时间解析错误、时区转换错误等
5. 创建自定义时间计算器，实现特定的时间计算逻辑
6. 序列化和反序列化 NodaTime 类型

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

## 实际应用场景

### 企业级应用

NodaTime 技能可以用于构建企业级应用，如人力资源系统、项目管理系统等，需要处理大量日期时间数据的场景。

### 金融系统

在金融系统中，NodaTime 技能可以用于处理交易时间、结算时间、报表时间等，确保时间计算的准确性和一致性。

### 预约系统

预约系统需要处理各种时间相关的操作，如预约时间、取消时间、提醒时间等，NodaTime 技能可以提供可靠的时间处理功能。

### 国际化应用

国际化应用需要支持不同时区的时间显示和转换，NodaTime 技能内置了丰富的时区数据，支持全球时区转换。

### 日志系统

日志系统需要记录事件发生的时间，NodaTime 技能可以提供高精度的时间戳和格式化功能，便于日志分析和查询。
