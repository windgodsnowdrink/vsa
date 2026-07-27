# choetl Agent Skill - 高效ETL处理技能

## 技能概述

基于 .NET 10 的高性能 ETL 处理技能，为 .NET 开发者提供强大的 ETL 功能，支持 AOT（提前编译）编译，用于实现高效的数据提取、转换和加载。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:package ChoETL@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
```

### 注册服务

在您的主应用程序中注册 ETL 服务：

```csharp
// 配置应用程序
var builder = WebApplication.CreateBuilder(args);

// 注册 ETL 服务
builder.Services.AddEtlServices();

// AOT 优化配置
builder.Services.Configure<EtlSettings>(options => {
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
    options.DefaultDataFormat = DataFormat.Json;
});

var app = builder.Build();
```

### 使用示例

```csharp
// 获取 ETL 服务
var etlService = serviceProvider.GetRequiredService<IEtlService>();

// 创建 ETL 作业配置
var etlJobConfig = new EtlJobConfig
{
    SourceType = DataSourceType.Csv,
    SourcePath = "input/data.csv",
    DestinationType = DataSourceType.Database,
    DestinationConnectionString = "Server=localhost;Database=test;User=sa;Password=password;",
    DestinationTable = "Employees",
    EnableAotOptimization = true
};

// 执行 ETL 作业
var result = await etlService.ExecuteJobAsync(etlJobConfig);
Console.WriteLine($"ETL 作业结果: {result.Success}");
Console.WriteLine($"处理记录数: {result.ProcessedRecords}");
Console.WriteLine($"失败记录数: {result.FailedRecords}");
Console.WriteLine($"执行时间: {result.ExecutionTime.TotalSeconds:F2} 秒");
```

## 导航地图

```
choetl/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── choetl_integration.cs         # ETL 集成示例
    ├── choetl_integration.run.json   # 运行配置
    ├── choetl_integration.setting.json # 设置文件
    ├── realtime_etl.cs               # 实时 ETL 实现
    ├── realtime_etl.run.json         # 实时 ETL 运行配置
    ├── realtime_etl.setting.json      # 实时 ETL 设置文件
    ├── DataValidationOptions.cs      # 数据验证配置
    ├── DataValidationOptions.run.json # 数据验证运行配置
    ├── DataValidationOptions.setting.json # 数据验证设置文件
    ├── DistributedETLOptions.cs      # 分布式 ETL 配置
    ├── DistributedETLOptions.run.json # 分布式 ETL 运行配置
    └── DistributedETLOptions.setting.json # 分布式 ETL 设置文件
```

## 主要功能

1. **多种数据源支持**: 支持数据库、文件（CSV、JSON、XML、Parquet 等）、API、消息队列等多种数据源
2. **高效的数据转换引擎**: 高性能数据转换引擎，支持复杂的数据转换规则
3. **实时和批量数据处理**: 支持实时数据流处理和批量数据处理模式
4. **数据验证和清洗**: 内置数据验证和清洗功能，确保数据质量
5. **分布式 ETL 支持**: 支持分布式 ETL 处理，提高处理效率和可靠性
6. **数据监控和报告**: 实时监控 ETL 作业状态，生成详细的处理报告
7. **模块化设计**: 模块化架构，便于扩展和维护
8. **AOT 编译支持**: 支持 AOT 编译，提高运行时性能和启动速度
9. **异步编程模型**: 基于异步/等待模式，提高并发处理能力
10. **依赖注入支持**: 支持依赖注入，提高代码的可测试性和可维护性

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

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
[ApiController]
[Route("api/[controller]")]
public class EtlController : ControllerBase
{
    private readonly IEtlService _etlService;

    public EtlController(IEtlService etlService)
    {
        _etlService = etlService;
    }

    [HttpPost("execute-job")]
    public async Task<IActionResult> ExecuteEtlJob([FromBody] EtlJobConfig request)
    {
        var result = await _etlService.ExecuteJobAsync(request);
        return Ok(new { Result = result });
    }

    [HttpGet("job-status/{jobId}")]
    public async Task<IActionResult> GetJobStatus(string jobId)
    {
        var status = await _etlService.GetJobStatusAsync(jobId);
        return Ok(new { Status = status });
    }
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
                _logger.LogInformation("收到 ETL 作业消息: {MessageType}", message.MessageType);
                
                // 解析 ETL 作业配置
                var etlJobConfig = JsonSerializer.Deserialize<EtlJobConfig>(message.Body);
                if (etlJobConfig != null)
                {
                    // 执行 ETL 作业
                    await _etlService.ExecuteJobAsync(etlJobConfig);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理 ETL 作业消息失败: {Message}", ex.Message);
            }
        }, stoppingToken);
    }
}
```

## 性能优化建议

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

## 故障排除

### 常见问题

1. **ETL 作业执行失败**
   - 检查数据源连接是否正确
   - 验证数据格式和结构
   - 查看详细日志
   - 检查目标数据库权限

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode
   - 使用 AOT 分析工具检查问题
   - 添加必要的 DynamicallyAccessedMembers 特性

3. **性能问题**
   - 优化数据源访问
   - 增加批量处理大小
   - 启用并行处理
   - 检查内存使用情况
   - 优化转换逻辑
   - 启用 AOT 编译

4. **数据质量问题**
   - 检查数据验证规则
   - 优化数据清洗逻辑
   - 增加数据质量监控
   - 调整错误处理策略

## 扩展开发

### 创建自定义数据源

```csharp
// 自定义数据源接口实现
public class CustomDataSource : IDataSource
{
    private readonly string _connectionString;
    
    public CustomDataSource(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<IEnumerable<IDataRecord>> ReadAsync(CancellationToken cancellationToken = default)
    {
        // 实现自定义数据源读取逻辑
        await Task.Delay(100);
        return new List<IDataRecord>();
    }
    
    public async Task WriteAsync(IEnumerable<IDataRecord> records, CancellationToken cancellationToken = default)
    {
        // 实现自定义数据源写入逻辑
        await Task.Delay(100);
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
public class CustomTransformRule : ITransformRule
{
    public string Name => "CustomTransform";
    
    public IDataRecord Apply(IDataRecord record)
    {
        // 实现自定义转换逻辑
        var transformedRecord = new Dictionary<string, object>(record);
        transformedRecord["CustomField"] = "CustomValue";
        return transformedRecord;
    }
}

// 注册自定义转换规则
builder.Services.AddSingleton<ITransformRule, CustomTransformRule>();
builder.Services.AddEtlServices();
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

## 总结

choetl 技能提供了一套完整的现代化 ETL 解决方案，基于 .NET 10 构建，具有高性能、模块化、可扩展等特点。通过 choetl，开发者可以快速构建各种 ETL 作业，支持多种数据源和数据格式，适用于实时和批量处理场景。

该技能遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持与多种系统集成，如 Web API、消息队列等。同时，提供了详细的性能优化建议和故障排除指南，帮助开发者构建高质量的 ETL 系统。

choetl 支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。通过遵循本文档中的最佳实践和 AOT 兼容性建议，开发者可以构建出高性能、可靠的 ETL 系统，满足各种数据处理需求。

ETL 是现代数据系统中的重要组成部分，通过 choetl 技能，开发者可以轻松实现各种复杂的 ETL 作业，提高数据处理效率和质量，为数据驱动决策提供有力支持。
