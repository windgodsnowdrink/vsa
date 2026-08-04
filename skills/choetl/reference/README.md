# choetl - 参考文档

## 概述

choetl 是基于 .NET 10 构建的高性能 ETL（提取、转换、加载）系统，为 .NET 开发者提供强大的 ETL 功能，支持 AOT（提前编译）编译，用于实现高效的数据处理和转换。

## 核心组件

### 1. ETL 服务 (IEtlService)

- **位置**: scripts/choetl_integration.cs
- **功能**: ETL 处理的核心业务逻辑处理
- **特性**: 
  - 多种数据源支持（数据库、文件、API、消息队列等）
  - 高效的数据转换引擎
  - 实时和批量数据处理
  - 数据验证和清洗
  - 分布式 ETL 支持
  - 数据监控和报告
  - 高性能设计，支持高并发场景
  - 完善的错误处理和日志记录
  - AOT 编译优化支持
  - 异步编程模型
- **使用示例**: 
  ```csharp
  public class EtlService : IEtlService
  {
      private readonly ILogger<EtlService> _logger;
      private readonly EtlSettings _settings;
      private readonly Dictionary<string, EtlJob> _activeJobs = new();
      private readonly List<IDataSourceFactory> _dataSourceFactories = new();
      private readonly List<ITransformRule> _transformRules = new();
      
      public EtlService(ILogger<EtlService> logger, IOptions<EtlSettings> settings, 
          IEnumerable<IDataSourceFactory> dataSourceFactories, IEnumerable<ITransformRule> transformRules)
      {
          _logger = logger;
          _settings = settings.Value;
          _dataSourceFactories.AddRange(dataSourceFactories);
          _transformRules.AddRange(transformRules);
      }
      
      public async Task<EtlJobResult> ExecuteJobAsync(EtlJobConfig jobConfig)
      {
          _logger.LogInformation("执行 ETL 作业: 源类型: {SourceType}, 目标类型: {DestinationType}", 
              jobConfig.SourceType, jobConfig.DestinationType);
          
          var startTime = DateTime.UtcNow;
          long processedRecords = 0;
          long failedRecords = 0;
          
          try
          {
              // 创建数据源
              var sourceFactory = _dataSourceFactories.FirstOrDefault(f => f.CanHandle(jobConfig.SourceType));
              var destinationFactory = _dataSourceFactories.FirstOrDefault(f => f.CanHandle(jobConfig.DestinationType));
              
              if (sourceFactory == null || destinationFactory == null)
              {
                  return new EtlJobResult {
                      Success = false,
                      Message = "不支持的数据源类型"
                  };
              }
              
              var source = sourceFactory.CreateSource(jobConfig.SourceConfig);
              var destination = destinationFactory.CreateDestination(jobConfig.DestinationConfig);
              
              // 执行 ETL 处理
              await foreach (var record in source.ReadAsync())
              {
                  try
                  {
                      // 应用转换规则
                      var transformedRecord = ApplyTransformRules(record, jobConfig.TransformRules);
                      
                      // 写入目标
                      await destination.WriteAsync(new[] { transformedRecord });
                      processedRecords++;
                  }
                  catch (Exception ex)
                  {
                      _logger.LogError(ex, "处理记录失败: {Message}", ex.Message);
                      failedRecords++;
                  }
              }
              
              return new EtlJobResult {
                  Success = true,
                  ProcessedRecords = processedRecords,
                  FailedRecords = failedRecords,
                  ExecutionTime = DateTime.UtcNow - startTime,
                  Message = "ETL 作业执行成功"
              };
          }
          catch (Exception ex)
          {
              _logger.LogError(ex, "ETL 作业执行失败: {Message}", ex.Message);
              return new EtlJobResult {
                  Success = false,
                  ProcessedRecords = processedRecords,
                  FailedRecords = failedRecords,
                  ExecutionTime = DateTime.UtcNow - startTime,
                  Message = $"ETL 作业执行失败: {ex.Message}"
              };
          }
      }
      
      public async Task<EtlJobStatus> GetJobStatusAsync(string jobId)
      {
          _logger.LogInformation("获取 ETL 作业状态: {JobId}", jobId);
          
          if (_activeJobs.TryGetValue(jobId, out var job))
          {
              return job.Status;
          }
          
          return EtlJobStatus.NotFound;
      }
      
      private IDataRecord ApplyTransformRules(IDataRecord record, List<string> ruleNames)
      {
          var transformedRecord = record;
          
          foreach (var ruleName in ruleNames)
          {
              var transformRule = _transformRules.FirstOrDefault(r => r.Name == ruleName);
              if (transformRule != null)
              {
                  transformedRecord = transformRule.Apply(transformedRecord);
              }
          }
          
          return transformedRecord;
      }
  }
  ```

### 2. 数据源工厂 (IDataSourceFactory)

- **功能**: 数据源的工厂类，用于创建不同类型的数据源实例
- **特性**: 
  - 支持多种数据源类型
  - 插件式设计，便于扩展
  - AOT 编译优化支持
  - 异步编程模型
- **使用示例**: 
  ```csharp
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public interface IDataSourceFactory
  {
      IDataSource CreateSource(object config);
      IDataSource CreateDestination(object config);
      bool CanHandle(DataSourceType type);
  }
  
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public class CsvDataSourceFactory : IDataSourceFactory
  {
      private readonly ILogger<CsvDataSourceFactory> _logger;
      
      public CsvDataSourceFactory(ILogger<CsvDataSourceFactory> logger)
      {
          _logger = logger;
      }
      
      public IDataSource CreateSource(object config)
      {
          _logger.LogInformation("创建 CSV 数据源");
          return new CsvDataSource(config as CsvDataSourceConfig);
      }
      
      public IDataSource CreateDestination(object config)
      {
          _logger = logger;
          _logger.LogInformation("创建 CSV 目标数据源");
          return new CsvDataSource(config as CsvDataSourceConfig);
      }
      
      public bool CanHandle(DataSourceType type)
      {
          return type == DataSourceType.Csv;
      }
  }
  ```

### 3. 数据源 (IDataSource)

- **功能**: 具体数据源的实现，用于读取和写入数据
- **特性**: 
  - 支持多种数据格式
  - AOT 编译优化支持
  - 异步编程模型
  - 高性能设计
- **使用示例**: 
  ```csharp
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public interface IDataSource
  {
      IAsyncEnumerable<IDataRecord> ReadAsync(CancellationToken cancellationToken = default);
      Task WriteAsync(IEnumerable<IDataRecord> records, CancellationToken cancellationToken = default);
      DataSourceType Type { get; }
  }
  
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public class CsvDataSource : IDataSource
  {
      private readonly CsvDataSourceConfig _config;
      
      public CsvDataSource(CsvDataSourceConfig config)
      {
          _config = config;
      }
      
      public async IAsyncEnumerable<IDataRecord> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
      {
          // 实现 CSV 读取逻辑
          using var reader = new StreamReader(_config.FilePath);
          
          // 读取标题行
          var headerLine = await reader.ReadLineAsync(cancellationToken);
          if (string.IsNullOrEmpty(headerLine))
          {
              yield break;
          }
          
          var headers = headerLine.Split(_config.Delimiter);
          
          // 读取数据行
          string? line;
          while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
          {
              var values = line.Split(_config.Delimiter);
              var record = new Dictionary<string, object>();
              
              for (int i = 0; i < headers.Length && i < values.Length; i++)
              {
                  record[headers[i]] = values[i];
              }
              
              yield return record;
          }
      }
      
      public async Task WriteAsync(IEnumerable<IDataRecord> records, CancellationToken cancellationToken = default)
      {
          // 实现 CSV 写入逻辑
          using var writer = new StreamWriter(_config.FilePath, _config.Append);
          
          foreach (var record in records)
          {
              var values = record.Values.Select(v => v?.ToString() ?? "").ToArray();
              var line = string.Join(_config.Delimiter, values);
              await writer.WriteLineAsync(line);
          }
      }
      
      public DataSourceType Type => DataSourceType.Csv;
  }
  
  public class CsvDataSourceConfig
  {
      public string FilePath { get; set; } = string.Empty;
      public char Delimiter { get; set; } = ',';
      public bool HasHeader { get; set; } = true;
      public bool Append { get; set; } = false;
  }
  ```

### 4. 转换规则 (ITransformRule)

- **功能**: 数据转换的规则实现
- **特性**: 
  - 支持多种转换规则
  - 插件式设计，便于扩展
  - AOT 编译优化支持
- **使用示例**: 
  ```csharp
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public interface ITransformRule
  {
      string Name { get; }
      IDataRecord Apply(IDataRecord record);
  }
  
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public class DateFormatTransformRule : ITransformRule
  {
      public string Name => "DateFormat";
      
      private readonly string _sourceFormat;
      private readonly string _targetFormat;
      private readonly string _fieldName;
      
      public DateFormatTransformRule(string fieldName, string sourceFormat, string targetFormat)
      {
          _fieldName = fieldName;
          _sourceFormat = sourceFormat;
          _targetFormat = targetFormat;
      }
      
      public IDataRecord Apply(IDataRecord record)
      {
          var transformedRecord = new Dictionary<string, object>(record);
          
          if (record.TryGetValue(_fieldName, out var value) && value != null)
          {
              if (DateTime.TryParseExact(value.ToString(), _sourceFormat, null, DateTimeStyles.None, out var date))
              {
                  transformedRecord[_fieldName] = date.ToString(_targetFormat);
              }
          }
          
          return transformedRecord;
      }
  }
  ```

### 5. ETL 设置 (EtlSettings)

- **功能**: 全局 ETL 配置
- **特性**: 
  - AOT 优化配置
  - 缓存设置
  - 超时设置
  - 日志配置
  - 默认数据格式配置
- **使用示例**: 
  ```csharp
  public class EtlSettings
  {
      public bool EnableAotOptimization { get; set; } = false;
      public bool EnableTrimOptimization { get; set; } = false;
      public DataFormat DefaultDataFormat { get; set; } = DataFormat.Json;
      public bool EnableCache { get; set; } = true;
      public int CacheSize { get; set; } = 1000;
      public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
      public LogLevel LogLevel { get; set; } = LogLevel.Information;
      public bool EnableDetailedLogging { get; set; } = false;
      public int MaxConcurrentJobs { get; set; } = 10;
      public int BatchSize { get; set; } = 1000;
      public bool EnableParallelProcessing { get; set; } = true;
      public int MaxParallelDegree { get; set; } = Environment.ProcessorCount;
  }
  ```

## 使用示例

### 基础使用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChoETL;

// 配置服务
var services = new ServiceCollection();

// 添加日志服务
services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

// 注册 ETL 服务
services.AddEtlServices();

// 构建服务提供器
var serviceProvider = services.BuildServiceProvider();

// 获取 ETL 服务
var etlService = serviceProvider.GetRequiredService<IEtlService>();

// 创建 ETL 作业配置
var etlJobConfig = new EtlJobConfig
{
    SourceType = DataSourceType.Csv,
    SourceConfig = new CsvDataSourceConfig {
        FilePath = "input/employees.csv",
        Delimiter = ',',
        HasHeader = true
    },
    DestinationType = DataSourceType.Json,
    DestinationConfig = new JsonDataSourceConfig {
        FilePath = "output/employees.json",
        Indent = true
    },
    TransformRules = new List<string> { "DateFormat" }
};

// 执行 ETL 作业
Console.WriteLine("开始执行 ETL 作业...");
var result = await etlService.ExecuteJobAsync(etlJobConfig);
Console.WriteLine($"ETL 作业结果: {result.Success}");
Console.WriteLine($"处理记录数: {result.ProcessedRecords}");
Console.WriteLine($"失败记录数: {result.FailedRecords}");
Console.WriteLine($"执行时间: {result.ExecutionTime.TotalSeconds:F2} 秒");
Console.WriteLine($"消息: {result.Message}");
```

### AOT 编译优化示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChoETL;

// 数据源类型枚举
public enum DataSourceType
{
    Csv,
    Json,
    Database,
    Api,
    MessageQueue,
    Xml,
    Parquet,
    Custom
}

// 数据格式枚举
public enum DataFormat
{
    Csv,
    Json,
    Xml,
    Parquet,
    Binary
}

// ETL 作业结果类
public class EtlJobResult
{
    public bool Success { get; set; }
    public long ProcessedRecords { get; set; }
    public long FailedRecords { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public string Message { get; set; } = string.Empty;
}

// ETL 作业状态枚举
public enum EtlJobStatus
{
    NotStarted,
    Running,
    Completed,
    Failed,
    Canceled,
    NotFound
}

// ETL 作业配置类
public class EtlJobConfig
{
    public DataSourceType SourceType { get; set; }
    public object SourceConfig { get; set; } = new();
    public DataSourceType DestinationType { get; set; }
    public object DestinationConfig { get; set; } = new();
    public List<string> TransformRules { get; set; } = new();
    public bool EnableAotOptimization { get; set; } = false;
}

// CSV 数据源配置类
public class CsvDataSourceConfig
{
    public string FilePath { get; set; } = string.Empty;
    public char Delimiter { get; set; } = ',';
    public bool HasHeader { get; set; } = true;
    public bool Append { get; set; } = false;
}

// JSON 数据源配置类
public class JsonDataSourceConfig
{
    public string FilePath { get; set; } = string.Empty;
    public bool Indent { get; set; } = false;
    public bool Append { get; set; } = false;
}

// 数据记录接口
public interface IDataRecord : IEnumerable<KeyValuePair<string, object>>
{
    object this[string key] { get; set; }
    bool ContainsKey(string key);
    bool TryGetValue(string key, out object value);
    int Count { get; }
    ICollection<string> Keys { get; }
    ICollection<object> Values { get; }
}

// 字典数据记录实现
public class DictionaryDataRecord : Dictionary<string, object>, IDataRecord
{
    public DictionaryDataRecord() : base(StringComparer.OrdinalIgnoreCase) { }
    public DictionaryDataRecord(IDictionary<string, object> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase) { }
}

// ETL 服务接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IEtlService
{
    Task<EtlJobResult> ExecuteJobAsync(EtlJobConfig jobConfig);
    Task<EtlJobStatus> GetJobStatusAsync(string jobId);
    Task CancelJobAsync(string jobId);
}

// AOT 安全的 ETL 服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeEtlService : IEtlService
{
    private readonly ILogger<AotSafeEtlService> _logger;
    private readonly EtlSettings _settings;
    
    public AotSafeEtlService(ILogger<AotSafeEtlService> logger, IOptions<EtlSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }
    
    public async Task<EtlJobResult> ExecuteJobAsync(EtlJobConfig jobConfig)
    {
        _logger.LogInformation("执行 AOT 优化的 ETL 作业");
        
        var startTime = DateTime.UtcNow;
        
        try
        {
            // 模拟 ETL 作业执行
            await Task.Delay(100);
            
            return new EtlJobResult {
                Success = true,
                ProcessedRecords = 1000,
                FailedRecords = 0,
                ExecutionTime = DateTime.UtcNow - startTime,
                Message = "AOT 优化的 ETL 作业执行成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AOT 优化的 ETL 作业执行失败");
            return new EtlJobResult {
                Success = false,
                ProcessedRecords = 0,
                FailedRecords = 0,
                ExecutionTime = DateTime.UtcNow - startTime,
                Message = $"AOT 优化的 ETL 作业执行失败: {ex.Message}"
            };
        }
    }
    
    public Task<EtlJobStatus> GetJobStatusAsync(string jobId)
    {
        _logger.LogInformation("获取 AOT 优化的 ETL 作业状态: {JobId}", jobId);
        return Task.FromResult(EtlJobStatus.Completed);
    }
    
    public Task CancelJobAsync(string jobId)
    {
        _logger.LogInformation("取消 AOT 优化的 ETL 作业: {JobId}", jobId);
        return Task.CompletedTask;
    }
}

// ETL 服务扩展类
public static class EtlServiceExtensions
{
    public static IServiceCollection AddEtlServices(this IServiceCollection services)
    {
        services.AddSingleton<IEtlService, AotSafeEtlService>();
        services.AddOptions<EtlSettings>();
        return services;
    }
    
    public static IServiceCollection AddEtlServices(this IServiceCollection services, Action<EtlSettings> configureOptions)
    {
        services.AddEtlServices();
        services.Configure(configureOptions);
        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("ETL 处理 AOT 编译优化示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 AOT 优化的 ETL 服务
        services.AddEtlServices(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
            options.DefaultDataFormat = DataFormat.Json;
            options.EnableCache = true;
            options.CacheSize = 1000;
        });
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        var etlService = serviceProvider.GetRequiredService<IEtlService>();
        
        Console.WriteLine("\n1. 执行 AOT 优化的 ETL 作业:");
        Console.WriteLine("-" * 40);
        
        // 创建 AOT 优化的 ETL 作业配置
        var etlJobConfig = new EtlJobConfig
        {
            SourceType = DataSourceType.Csv,
            SourceConfig = new CsvDataSourceConfig {
                FilePath = "input/data.csv",
                Delimiter = ',',
                HasHeader = true
            },
            DestinationType = DataSourceType.Json,
            DestinationConfig = new JsonDataSourceConfig {
                FilePath = "output/data.json",
                Indent = true
            },
            EnableAotOptimization = true
        };
        
        // 执行 AOT 优化的 ETL 作业
        var stopwatch = Stopwatch.StartNew();
        var result = await etlService.ExecuteJobAsync(etlJobConfig);
        stopwatch.Stop();
        
        Console.WriteLine($"结果: {result.Success}");
        Console.WriteLine($"处理记录数: {result.ProcessedRecords}");
        Console.WriteLine($"失败记录数: {result.FailedRecords}");
        Console.WriteLine($"执行时间: {result.ExecutionTime.TotalSeconds:F2} 秒");
        Console.WriteLine($"消息: {result.Message}");
    }
}
```

## AOT 编译支持

### AOT 编译配置

在项目文件中添加以下配置以支持 AOT 编译：

```xml
<PropertyGroup>
  <PublishAot>true</PublishAot>
  <TrimMode>Full</TrimMode>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
</PropertyGroup>

<!-- 添加 AOT 兼容的依赖 -->
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Aot" Version="10.0.0" />
  <PackageReference Include="ChoETL" Version="1.0.0" />
</ItemGroup>
```

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的 ChoETL 版本和依赖库支持 AOT 编译
2. **避免反射**: 避免在 ETL 处理逻辑中使用反射
3. **资源处理**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用值类型**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试
7. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库
8. **使用 AOT 安全的数据源**: 确保使用的数据源实现支持 AOT 编译
9. **使用 AOT 安全的转换规则**: 确保转换规则实现支持 AOT 编译
10. **添加必要的特性**: 为 AOT 编译添加必要的 DynamicallyAccessedMembers 特性

## 配置选项

### ETL 配置

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| EnableAotOptimization | bool | false | 启用 AOT 优化 |
| EnableTrimOptimization | bool | false | 启用修剪优化 |
| DefaultDataFormat | enum | Json | 默认数据格式 |
| EnableCache | bool | true | 启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| CacheExpiration | TimeSpan | 01:00:00 | 缓存过期时间 |
| Timeout | TimeSpan | 00:00:30 | 操作超时时间 |
| LogLevel | LogLevel | Information | 日志级别 |
| EnableDetailedLogging | bool | false | 启用详细日志记录 |
| MaxConcurrentJobs | int | 10 | 最大并发作业数 |
| BatchSize | int | 1000 | 批量处理大小 |
| EnableParallelProcessing | bool | true | 启用并行处理 |
| MaxParallelDegree | int | ProcessorCount | 最大并行度 |

### 应用配置示例

```json
{
  "Etl": {
    "EnableAotOptimization": true,
    "EnableTrimOptimization": true,
    "DefaultDataFormat": "Json",
    "EnableCache": true,
    "CacheSize": 2000,
    "CacheExpiration": "02:00:00",
    "Timeout": "00:01:00",
    "LogLevel": "Information",
    "EnableDetailedLogging": false,
    "MaxConcurrentJobs": 20,
    "BatchSize": 5000,
    "EnableParallelProcessing": true,
    "MaxParallelDegree": 8
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "ChoETL": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化数据源访问**: 优化数据源的连接和访问方式
4. **批量处理**: 使用批量处理减少 I/O 操作
5. **内存优化**: 减少内存分配和拷贝，使用内存池和零拷贝技术
6. **并行处理**: 对于大数据量，使用并行处理提高效率
7. **使用合适的数据格式**: 选择适合场景的数据格式，如 Parquet 对于分析场景
8. **监控性能**: 使用 OpenTelemetry 等工具监控 ETL 性能
9. **缓存频繁使用的数据**: 对频繁使用的数据进行缓存
10. **优化转换逻辑**: 简化复杂的转换逻辑，提高执行效率
11. **使用对象池**: 对频繁创建的对象使用对象池，减少垃圾回收压力
12. **优化数据模型**: 设计合理的数据模型，减少转换复杂度
13. **使用高效的序列化**: 优先使用高效的序列化库
14. **调整批量大小**: 根据系统资源和数据特点调整批量处理大小
15. **优化网络传输**: 对于分布式 ETL，优化网络传输效率

## 故障排除

### 常见问题

1. **ETL 作业执行失败**
   - 检查数据源连接是否正确
   - 验证数据格式和结构
   - 查看详细日志
   - 检查目标数据库权限
   - 验证文件路径和权限
   - 检查网络连接（对于远程数据源）

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode
   - 使用 AOT 分析工具检查问题
   - 添加必要的 DynamicallyAccessedMembers 特性
   - 确保所有类型都能在编译时被解析

3. **性能问题**
   - 优化数据源访问
   - 增加批量处理大小
   - 启用并行处理
   - 检查内存使用情况
   - 优化转换逻辑
   - 启用 AOT 编译
   - 增加缓存大小
   - 优化数据模型

4. **数据质量问题**
   - 检查数据验证规则
   - 优化数据清洗逻辑
   - 增加数据质量监控
   - 调整错误处理策略
   - 增加数据转换的错误处理

5. **内存溢出**
   - 减少批量处理大小
   - 优化内存使用
   - 增加系统内存
   - 使用更高效的数据结构
   - 启用内存监控

6. **并发问题**
   - 调整最大并发作业数
   - 优化并行处理逻辑
   - 增加系统资源
   - 检查线程安全问题

## 扩展开发

### 创建自定义数据源

```csharp
// 自定义数据源工厂
public class CustomDataSourceFactory : IDataSourceFactory
{
    private readonly ILogger<CustomDataSourceFactory> _logger;
    
    public CustomDataSourceFactory(ILogger<CustomDataSourceFactory> logger)
    {
        _logger = logger;
    }
    
    public IDataSource CreateSource(object config)
    {
        _logger.LogInformation("创建自定义数据源");
        return new CustomDataSource(config as CustomDataSourceConfig);
    }
    
    public IDataSource CreateDestination(object config)
    {
        _logger = logger;
        _logger.LogInformation("创建自定义目标数据源");
        return new CustomDataSource(config as CustomDataSourceConfig);
    }
    
    public bool CanHandle(DataSourceType type)
    {
        return type == DataSourceType.Custom;
    }
}

// 自定义数据源配置
public class CustomDataSourceConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 1000;
}

// 自定义数据源实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class CustomDataSource : IDataSource
{
    private readonly CustomDataSourceConfig _config;
    
    public CustomDataSource(CustomDataSourceConfig config)
    {
        _config = config;
    }
    
    public async IAsyncEnumerable<IDataRecord> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 实现自定义数据源读取逻辑
        // 例如：从自定义 API 或服务读取数据
        _logger.LogInformation("从自定义数据源读取数据");
        
        // 模拟数据读取
        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(100, cancellationToken);
            yield return new DictionaryDataRecord {
                { "Id", i },
                { "Name", $"Item {i}" },
                { "Value", i * 100 }
            };
        }
    }
    
    public async Task WriteAsync(IEnumerable<IDataRecord> records, CancellationToken cancellationToken = default)
    {
        // 实现自定义数据源写入逻辑
        // 例如：写入自定义 API 或服务
        _logger.LogInformation("写入自定义数据源");
        
        foreach (var record in records)
        {
            // 模拟写入操作
            await Task.Delay(50, cancellationToken);
            _logger.LogInformation("写入记录: {Id}", record["Id"]);
        }
    }
    
    public DataSourceType Type => DataSourceType.Custom;
}

// 注册自定义数据源
builder.Services.AddSingleton<IDataSourceFactory, CustomDataSourceFactory>();
builder.Services.AddEtlServices();
```

### 创建自定义转换规则

```csharp
// 自定义转换规则
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class CustomTransformRule : ITransformRule
{
    public string Name => "CustomTransform";
    
    public IDataRecord Apply(IDataRecord record)
    {
        var transformedRecord = new DictionaryDataRecord(record);
        
        // 实现自定义转换逻辑
        if (record.TryGetValue("Amount", out var amountValue) && amountValue != null)
        {
            if (decimal.TryParse(amountValue.ToString(), out var amount))
            {
                // 添加税费计算
                var tax = amount * 0.1m; // 10% 税费
                transformedRecord["Tax"] = tax;
                transformedRecord["TotalAmount"] = amount + tax;
            }
        }
        
        return transformedRecord;
    }
}

// 注册自定义转换规则
builder.Services.AddSingleton<ITransformRule, CustomTransformRule>();
builder.Services.AddEtlServices();
```

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
[ApiController]
[Route("api/[controller]")]
public class EtlController : ControllerBase
{
    private readonly IEtlService _etlService;
    private readonly ILogger<EtlController> _logger;

    public EtlController(IEtlService etlService, ILogger<EtlController> logger)
    {
        _etlService = etlService;
        _logger = logger;
    }

    [HttpPost("execute-job")]
    [ProducesResponseType(typeof(EtlJobResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExecuteEtlJob([FromBody] EtlJobConfig request)
    {
        _logger.LogInformation("API ETL 作业执行请求: 源类型: {SourceType}, 目标类型: {DestinationType}", 
            request.SourceType, request.DestinationType);
        
        try
        {
            // 验证请求
            if (request == null)
            {
                return BadRequest(new ErrorResponse {
                    Success = false,
                    Message = "无效的请求参数",
                    ErrorCode = "INVALID_REQUEST"
                });
            }
            
            // 执行 ETL 作业
            var result = await _etlService.ExecuteJobAsync(request);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API ETL 作业执行失败: {Message}", ex.Message);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse {
                Success = false,
                Message = "ETL 作业执行失败",
                ErrorCode = "ETL_JOB_FAILED"
            });
        }
    }

    [HttpGet("job-status/{jobId}")]
    [ProducesResponseType(typeof(EtlJobStatus), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetJobStatus(string jobId)
    {
        _logger.LogInformation("API 获取 ETL 作业状态请求: {JobId}", jobId);
        
        try
        {
            var status = await _etlService.GetJobStatusAsync(jobId);
            
            if (status == EtlJobStatus.NotFound)
            {
                return NotFound(new ErrorResponse {
                    Success = false,
                    Message = "作业未找到",
                    ErrorCode = "JOB_NOT_FOUND"
                });
            }
            
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 获取 ETL 作业状态失败: {Message}", ex.Message);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse {
                Success = false,
                Message = "获取作业状态失败",
                ErrorCode = "GET_STATUS_FAILED"
            });
        }
    }

    [HttpPost("cancel-job/{jobId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelJob(string jobId)
    {
        _logger.LogInformation("API 取消 ETL 作业请求: {JobId}", jobId);
        
        try
        {
            await _etlService.CancelJobAsync(jobId);
            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 取消 ETL 作业失败: {Message}", ex.Message);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse {
                Success = false,
                Message = "取消作业失败",
                ErrorCode = "CANCEL_JOB_FAILED"
            });
        }
    }
}

// 错误响应类
public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

### 与消息队列集成

```csharp
// 消息队列消费者
public class EtlMessageConsumer : BackgroundService
{
    private readonly IEtlService _etlService;
    private readonly ILogger<EtlMessageConsumer> _logger;
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public EtlMessageConsumer(IEtlService etlService, ILogger<EtlMessageConsumer> logger, IMessageQueueClient messageQueueClient)
    {
        _etlService = etlService;
        _logger = logger;
        _messageQueueClient = messageQueueClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ETL 消息消费者已启动");
        
        await _messageQueueClient.SubscribeAsync("etl-jobs", async (message) => {
            try
            {
                _logger.LogInformation("收到 ETL 作业消息: {MessageType}, 消息 ID: {MessageId}", 
                    message.MessageType, message.MessageId);
                
                switch (message.MessageType)
                {
                    case "ExecuteJob":
                        await HandleExecuteJobCommandAsync(message, stoppingToken);
                        break;
                    
                    case "GetStatus":
                        await HandleGetStatusCommandAsync(message, stoppingToken);
                        break;
                    
                    case "CancelJob":
                        await HandleCancelJobCommandAsync(message, stoppingToken);
                        break;
                    
                    default:
                        _logger.LogWarning("未知的 ETL 命令类型: {MessageType}", message.MessageType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理 ETL 消息失败: {Message}", ex.Message);
            }
        }, stoppingToken);
        
        _logger.LogInformation("ETL 消息消费者已完成初始化");
    }
    
    private async Task HandleExecuteJobCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析 ETL 作业配置
        var jobConfig = JsonSerializer.Deserialize<EtlJobConfig>(message.Body, _jsonOptions);
        if (jobConfig == null)
        {
            _logger.LogError("无法解析 ETL 作业配置: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行消息驱动的 ETL 作业: 源类型: {SourceType}, 目标类型: {DestinationType}", 
            jobConfig.SourceType, jobConfig.DestinationType);
        
        // 执行 ETL 作业
        var result = await _etlService.ExecuteJobAsync(jobConfig);
        
        _logger.LogInformation("消息驱动的 ETL 作业执行完成: 结果: {Success}", result.Success);
        
        // 发布执行结果
        var resultMessage = new Message {
            MessageType = "JobCompleted",
            Body = JsonSerializer.Serialize(result, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CorrelationId", message.MessageId },
                { "CommandSource", "EtlMessageConsumer" }
            }
        };
        
        await _messageQueueClient.PublishAsync("etl-results", resultMessage, cancellationToken);
    }
    
    private async Task HandleGetStatusCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析作业 ID
        var request = JsonSerializer.Deserialize<GetStatusRequest>(message.Body, _jsonOptions);
        if (request == null || string.IsNullOrEmpty(request.JobId))
        {
            _logger.LogError("无法解析获取状态请求: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("获取消息驱动的 ETL 作业状态: {JobId}", request.JobId);
        
        // 获取作业状态
        var status = await _etlService.GetJobStatusAsync(request.JobId);
        
        // 发布状态结果
        var resultMessage = new Message {
            MessageType = "JobStatus",
            Body = JsonSerializer.Serialize(status, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CorrelationId", message.MessageId },
                { "CommandSource", "EtlMessageConsumer" }
            }
        };
        
        await _messageQueueClient.PublishAsync("etl-results", resultMessage, cancellationToken);
    }
    
    private async Task HandleCancelJobCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析作业 ID
        var request = JsonSerializer.Deserialize<CancelJobRequest>(message.Body, _jsonOptions);
        if (request == null || string.IsNullOrEmpty(request.JobId))
        {
            _logger.LogError("无法解析取消作业请求: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("取消消息驱动的 ETL 作业: {JobId}", request.JobId);
        
        // 取消作业
        await _etlService.CancelJobAsync(request.JobId);
        
        // 发布取消结果
        var resultMessage = new Message {
            MessageType = "JobCanceled",
            Body = JsonSerializer.Serialize(new { JobId = request.JobId, Canceled = true }, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CorrelationId", message.MessageId },
                { "CommandSource", "EtlMessageConsumer" }
            }
        };
        
        await _messageQueueClient.PublishAsync("etl-results", resultMessage, cancellationToken);
    }
}

// 获取状态请求类
public class GetStatusRequest
{
    public string JobId { get; set; } = string.Empty;
}

// 取消作业请求类
public class CancelJobRequest
{
    public string JobId { get; set; } = string.Empty;
}

// 注册消息队列消费者
builder.Services.AddHostedService<EtlMessageConsumer>();
```

## 最佳实践

1. **从简单开始**: 从简单的 ETL 作业开始，逐步增加复杂性
2. **使用依赖注入**: 利用依赖注入管理 ETL 服务
3. **优化数据模型**: 设计合理的数据模型，减少转换复杂度
4. **使用异步编程**: 优先使用异步 API，提高并发处理能力
5. **监控和日志**: 添加适当的监控和日志，便于故障排查
6. **测试 ETL 作业**: 对 ETL 作业进行充分测试，确保数据正确性
7. **考虑失败处理**: 实现适当的失败处理和重试机制
8. **使用事务**: 对于关键操作，使用事务确保数据一致性
9. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
10. **文档化**: 为 ETL 作业提供详细的文档
11. **考虑可扩展性**: 设计可扩展的 ETL 架构，支持未来需求
12. **使用版本控制**: 对 ETL 配置和代码进行版本控制
13. **考虑安全性**: 确保 ETL 作业的安全性，特别是涉及敏感数据时
14. **定期优化**: 定期优化 ETL 作业，提高性能和可靠性
15. **使用合适的工具**: 根据需求选择合适的 ETL 工具和库
16. **实现数据质量监控**: 添加数据质量监控，确保数据准确性
17. **使用批量处理**: 对大数据量使用批量处理，减少 I/O 操作
18. **实现并行处理**: 对于大数据量，使用并行处理提高效率
19. **考虑数据分区**: 对于超大数据集，考虑使用数据分区
20. **实现增量更新**: 对于频繁更新的数据，实现增量更新机制

## 总结

choetl 是一个基于 .NET 10 的现代化 ETL 框架，具有高性能、模块化、可扩展等特点。通过 choetl，开发者可以快速构建各种 ETL 作业，支持多种数据源和数据格式，适用于实时和批量处理场景。

本参考文档提供了 choetl 的核心组件、使用示例、AOT 编译支持、配置选项、性能优化建议、故障排除指南和扩展开发方法，帮助开发者充分利用 choetl 框架的优势，构建高质量的 ETL 系统。

choetl 支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。通过遵循本文档中的最佳实践和 AOT 兼容性建议，开发者可以构建出高性能、可靠的 ETL 系统，满足各种数据处理需求。

ETL 是现代数据系统中的重要组成部分，通过 choetl 框架，开发者可以轻松实现各种复杂的 ETL 作业，提高数据处理效率和质量，为数据驱动决策提供有力支持。无论是批量处理还是实时数据流，choetl 都能提供高性能、可靠的解决方案。
