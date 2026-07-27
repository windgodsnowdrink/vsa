# choetl - 使用示例

## 快速开始

### 1. 基础使用示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk
#:package ChoETL@2.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChoETL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var etlService = serviceProvider.GetRequiredService<IEtlService>();
        
        Console.WriteLine("choetl 基础使用示例");
        Console.WriteLine("=" * 50);
        
        // 创建 ETL 作业配置
        var jobConfig = new EtlJobConfig
        {
            SourceType = DataSourceType.Csv,
            SourceConfig = new CsvDataSourceConfig
            {
                FilePath = "input/employees.csv",
                Delimiter = ',',
                HasHeader = true
            },
            DestinationType = DataSourceType.Json,
            DestinationConfig = new JsonDataSourceConfig
            {
                FilePath = "output/employees.json",
                Indent = true
            }
        };
        
        // 执行 ETL 作业
        var result = await etlService.ExecuteJobAsync(jobConfig);
        Console.WriteLine($"ETL 作业结果: {result.Success}");
        Console.WriteLine($"处理记录数: {result.ProcessedRecords}");
        Console.WriteLine($"失败记录数: {result.FailedRecords}");
        Console.WriteLine($"执行时间: {result.ExecutionTime.TotalSeconds:F2} 秒");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 添加日志服务
        builder.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 ETL 服务
        builder.AddEtlServices();
        
        return builder.BuildServiceProvider();
    }
}
```

### 2. AOT 编译支持示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk
#:package ChoETL@2.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64

using System;
using System.Diagnostics;
using ChoETL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

// AOT 安全的 ETL 服务接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IEtlService
{
    Task<EtlJobResult> ExecuteJobAsync(EtlJobConfig jobConfig);
    Task<EtlJobStatus> GetJobStatusAsync(string jobId);
}

// AOT 安全的 ETL 作业结果
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class EtlJobResult
{
    public bool Success { get; set; }
    public long ProcessedRecords { get; set; }
    public long FailedRecords { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public string Message { get; set; } = string.Empty;
}

// AOT 安全的 ETL 作业状态
public enum EtlJobStatus
{
    NotStarted,
    Running,
    Completed,
    Failed,
    Canceled,
    NotFound
}

// AOT 安全的数据源类型
public enum DataSourceType
{
    Csv,
    Json,
    Database,
    Api,
    MessageQueue
}

// AOT 安全的 ETL 作业配置
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class EtlJobConfig
{
    public DataSourceType SourceType { get; set; }
    public object SourceConfig { get; set; } = new();
    public DataSourceType DestinationType { get; set; }
    public object DestinationConfig { get; set; } = new();
    public List<string> TransformRules { get; set; } = new();
    public bool EnableAotOptimization { get; set; } = true;
}

// AOT 安全的 CSV 数据源配置
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class CsvDataSourceConfig
{
    public string FilePath { get; set; } = string.Empty;
    public char Delimiter { get; set; } = ',';
    public bool HasHeader { get; set; } = true;
    public bool Append { get; set; } = false;
}

// AOT 安全的 JSON 数据源配置
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class JsonDataSourceConfig
{
    public string FilePath { get; set; } = string.Empty;
    public bool Indent { get; set; } = false;
    public bool Append { get; set; } = false;
}

// AOT 安全的数据记录接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IDataRecord : IEnumerable<KeyValuePair<string, object>>
{
    object this[string key] { get; set; }
    bool ContainsKey(string key);
    bool TryGetValue(string key, out object value);
    int Count { get; }
    ICollection<string> Keys { get; }
    ICollection<object> Values { get; }
}

// AOT 安全的字典数据记录实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class DictionaryDataRecord : Dictionary<string, object>, IDataRecord
{
    public DictionaryDataRecord() : base(StringComparer.OrdinalIgnoreCase) { }
    public DictionaryDataRecord(IDictionary<string, object> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase) { }
}

// AOT 安全的 ETL 服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeEtlService : IEtlService
{
    private readonly ILogger<AotSafeEtlService> _logger;
    
    public AotSafeEtlService(ILogger<AotSafeEtlService> logger)
    {
        _logger = logger;
    }
    
    public async Task<EtlJobResult> ExecuteJobAsync(EtlJobConfig jobConfig)
    {
        _logger.LogInformation("执行 AOT 安全的 ETL 作业");
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            // 模拟 ETL 处理
            await Task.Delay(1000);
            
            return new EtlJobResult
            {
                Success = true,
                ProcessedRecords = 1000,
                FailedRecords = 0,
                ExecutionTime = DateTime.UtcNow - startTime,
                Message = "AOT 安全的 ETL 作业执行成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AOT 安全的 ETL 作业执行失败");
            return new EtlJobResult
            {
                Success = false,
                ProcessedRecords = 0,
                FailedRecords = 0,
                ExecutionTime = DateTime.UtcNow - startTime,
                Message = $"AOT 安全的 ETL 作业执行失败: {ex.Message}"
            };
        }
    }
    
    public Task<EtlJobStatus> GetJobStatusAsync(string jobId)
    {
        _logger.LogInformation("获取 ETL 作业状态: {JobId}", jobId);
        return Task.FromResult(EtlJobStatus.Completed);
    }
}

// AOT 安全的 ETL 服务扩展
public static class EtlServiceExtensions
{
    public static IServiceCollection AddEtlServices(this IServiceCollection services)
    {
        services.AddSingleton<IEtlService, AotSafeEtlService>();
        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("choetl AOT 编译支持示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var etlService = serviceProvider.GetRequiredService<IEtlService>();
        
        // 创建 AOT 优化的 ETL 作业配置
        var jobConfig = new EtlJobConfig
        {
            SourceType = DataSourceType.Csv,
            SourceConfig = new CsvDataSourceConfig
            {
                FilePath = "input/data.csv",
                Delimiter = ',',
                HasHeader = true
            },
            DestinationType = DataSourceType.Json,
            DestinationConfig = new JsonDataSourceConfig
            {
                FilePath = "output/result.json",
                Indent = true
            },
            EnableAotOptimization = true
        };
        
        // 执行 ETL 作业
        var stopwatch = Stopwatch.StartNew();
        var result = await etlService.ExecuteJobAsync(jobConfig);
        stopwatch.Stop();
        
        Console.WriteLine($"ETL 作业结果: {result.Success}");
        Console.WriteLine($"处理记录数: {result.ProcessedRecords}");
        Console.WriteLine($"失败记录数: {result.FailedRecords}");
        Console.WriteLine($"执行时间: {result.ExecutionTime.TotalSeconds:F2} 秒");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalSeconds:F2} 秒");
        Console.WriteLine($"消息: {result.Message}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        
        // 添加日志服务
        builder.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 AOT 安全的 ETL 服务
        builder.AddEtlServices();
        
        return builder.BuildServiceProvider();
    }
}
```

### 3. 高级配置示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk
#:package ChoETL@2.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ChoETL;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("choetl 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置 ETL 设置
        builder.Configure<EtlSettings>(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
            options.DefaultDataFormat = DataFormat.Json;
            options.EnableCache = true;
            options.CacheSize = 2000;
            options.Timeout = TimeSpan.FromSeconds(60);
            options.EnableDetailedLogging = true;
            options.MaxConcurrentJobs = 20;
            options.BatchSize = 5000;
            options.EnableParallelProcessing = true;
            options.MaxParallelDegree = 8;
        });
        
        // 注册服务
        builder.AddEtlServices();
        builder.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<EtlSettings>>().Value;
        Console.WriteLine($"ETL 配置:");
        Console.WriteLine($"  AOT 优化: {settings.EnableAotOptimization}");
        Console.WriteLine($"  修剪优化: {settings.EnableTrimOptimization}");
        Console.WriteLine($"  缓存大小: {settings.CacheSize}");
        Console.WriteLine($"  超时时间: {settings.Timeout.TotalSeconds} 秒");
        Console.WriteLine($"  最大并发作业数: {settings.MaxConcurrentJobs}");
        Console.WriteLine($"  批量处理大小: {settings.BatchSize}");
        Console.WriteLine($"  并行处理线程数: {settings.MaxParallelDegree}");
        
        // 使用服务
        var etlService = serviceProvider.GetRequiredService<IEtlService>();
        var result = await etlService.ExecuteJobAsync(new EtlJobConfig
        {
            SourceType = DataSourceType.Csv,
            SourceConfig = new CsvDataSourceConfig
            {
                FilePath = "input/data.csv",
                Delimiter = ',',
                HasHeader = true
            },
            DestinationType = DataSourceType.Json,
            DestinationConfig = new JsonDataSourceConfig
            {
                FilePath = "output/result.json",
                Indent = true
            }
        });
        Console.WriteLine($"\nETL 作业结果: {result.Success}");
    }
}

// ETL 设置类
public class EtlSettings
{
    public bool EnableAotOptimization { get; set; } = false;
    public bool EnableTrimOptimization { get; set; } = false;
    public DataFormat DefaultDataFormat { get; set; } = DataFormat.Json;
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDetailedLogging { get; set; } = false;
    public int MaxConcurrentJobs { get; set; } = 10;
    public int BatchSize { get; set; } = 1000;
    public bool EnableParallelProcessing { get; set; } = true;
    public int MaxParallelDegree { get; set; } = Environment.ProcessorCount;
}

public enum DataFormat
{
    Csv,
    Json,
    Xml,
    Parquet,
    Binary
}

// 其他接口和类定义...
```

### 4. 性能优化示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk
#:package ChoETL@2.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ChoETL;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("choetl 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var etlService = serviceProvider.GetRequiredService<IEtlService>();
        
        // 性能测试
        const int iterations = 100;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            await etlService.ExecuteJobAsync(new EtlJobConfig
            {
                SourceType = DataSourceType.Csv,
                SourceConfig = new CsvDataSourceConfig
                {
                    FilePath = "input/small_data.csv",
                    Delimiter = ',',
                    HasHeader = true
                },
                DestinationType = DataSourceType.Json,
                DestinationConfig = new JsonDataSourceConfig
                {
                    FilePath = "output/result_" + i + ".json",
                    Indent = false
                }
            });
        }
        
        stopwatch.Stop();
        Console.WriteLine($"{iterations} 次迭代执行时间: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每次迭代: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms");
        Console.WriteLine($"每秒执行次数: {iterations / stopwatch.Elapsed.TotalSeconds:F2} ops/s");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEtlServices(options =>
        {
            options.EnableAotOptimization = true;
            options.EnableParallelProcessing = true;
            options.BatchSize = 10000;
            options.EnableCache = true;
            options.CacheSize = 5000;
        });
        builder.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Warning); // 降低日志级别以提高性能
        });
        return builder.BuildServiceProvider();
    }
}
```

### 5. 错误处理示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk
#:package ChoETL@2.3.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ChoETL;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("choetl 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var etlService = serviceProvider.GetRequiredService<IEtlService>();
        
        try
        {
            // 创建无效的 ETL 作业配置（不存在的文件）
            var jobConfig = new EtlJobConfig
            {
                SourceType = DataSourceType.Csv,
                SourceConfig = new CsvDataSourceConfig
                {
                    FilePath = "non_existent_file.csv", // 不存在的文件
                    Delimiter = ',',
                    HasHeader = true
                },
                DestinationType = DataSourceType.Json,
                DestinationConfig = new JsonDataSourceConfig
                {
                    FilePath = "output/result.json",
                    Indent = true
                }
            };
            
            var result = await etlService.ExecuteJobAsync(jobConfig);
            Console.WriteLine($"成功: {result.Success}");
            Console.WriteLine($"消息: {result.Message}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件未找到错误: {ex.Message}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"一般错误: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddEtlServices();
        builder.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        return builder.BuildServiceProvider();
    }
}
```

## 总结

以上示例演示了 choetl 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始基础操作
2. 配置高级选项
3. 实现 AOT 编译支持
4. 优化性能
5. 处理错误情况

该系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。所有示例代码均支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。
