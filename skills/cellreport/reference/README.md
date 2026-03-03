# cellreport - 参考文档

## 概述

cellreport 是基于 .NET 10 构建的高性能单元格报表系统，为 .NET 开发者提供强大的报表生成和处理功能，支持 AOT（提前编译）编译，适用于各种规模的报表应用场景。

## 核心组件

### 1. 单元格报表服务 (ICellReportService)

- **位置**: scripts/cellreport_integration.cs
- **功能**: 报表生成和处理的核心业务逻辑
- **特性**: 
  - 支持多种报表格式生成
  - 高性能单元格计算
  - 模板驱动的报表设计
  - AOT 编译优化支持
  - 异步编程模型
  - 完善的错误处理和日志记录
- **使用示例**: 
  ```csharp
  public class CellReportService : ICellReportService
  {
      private readonly ILogger<CellReportService> _logger;
      private readonly CellReportSettings _settings;

      public CellReportService(ILogger<CellReportService> logger, IOptions<CellReportSettings> settings)
      {
          _logger = logger;
          _settings = settings.Value;
      }

      public async Task<string> GenerateReportAsync(ReportConfig config, ReportData data)
      {
          // 实现报表生成逻辑
          _logger.LogInformation("开始生成报表...");
          // 报表生成代码
          return await Task.FromResult("report-output.xlsx");
      }
  }
  ```

### 2. 报表配置 (ReportConfig)

- **功能**: 定义报表生成的配置参数
- **特性**: 
  - 支持多种报表类型
  - 模板路径配置
  - 输出路径配置
  - AOT 优化开关
  - 报表参数配置
- **使用示例**: 
  ```csharp
  public class ReportConfig
  {
      public ReportType ReportType { get; set; }
      public string TemplatePath { get; set; } = string.Empty;
      public string OutputPath { get; set; } = string.Empty;
      public bool EnableAotOptimization { get; set; } = true;
      public Dictionary<string, object> Parameters { get; set; } = new();
  }
  ```

### 3. 报表数据 (ReportData)

- **功能**: 提供报表生成所需的数据
- **特性**: 
  - 支持多种数据源类型
  - 动态参数传递
  - 数据转换和处理
  - 内存优化设计
- **使用示例**: 
  ```csharp
  public class ReportData
  {
      public IReportDataSource DataSource { get; set; }
      public Dictionary<string, object> Parameters { get; set; } = new();
      public List<DataTable> AdditionalData { get; set; } = new();
  }
  ```

### 4. 报表数据源 (IReportDataSource)

- **功能**: 定义报表数据源接口
- **特性**: 
  - 支持多种数据源实现
  - 异步数据获取
  - 数据分页支持
  - AOT 兼容设计
- **使用示例**: 
  ```csharp
  public interface IReportDataSource
  {
      Task<object> GetDataAsync();
      Task<object> GetDataAsync(string query, params object[] parameters);
      Task<int> GetDataCountAsync();
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
using CellReport;

// 配置服务
var services = new ServiceCollection();
services.AddCellReport();

// 构建服务提供器
var serviceProvider = services.BuildServiceProvider();

// 获取报表服务
var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();

// 创建报表配置
var reportConfig = new ReportConfig
{
    ReportType = ReportType.Excel,
    OutputPath = "output-report.xlsx"
};

// 准备报表数据
var reportData = new ReportData
{
    DataSource = new ListReportDataSource(new List<object> {
        new { Id = 1, Name = "产品1", Price = 10.99m },
        new { Id = 2, Name = "产品2", Price = 19.99m }
    })
};

// 生成报表
var result = await cellReportService.GenerateReportAsync(reportConfig, reportData);
Console.WriteLine($"报表生成成功: {result}");
```

### AOT 编译示例

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
using System;
using Microsoft.Extensions.DependencyInjection;
using CellReport;

// AOT 安全的报表服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeCellReportService : ICellReportService
{
    public async Task<string> GenerateReportAsync(ReportConfig config, ReportData data)
    {
        // AOT 安全的报表生成逻辑
        return await Task.FromResult("aot-output-report.xlsx");
    }
}

// 主程序
var builder = WebApplication.CreateBuilder(args);

// 注册 AOT 安全的报表服务
builder.Services.AddSingleton<ICellReportService, AotSafeCellReportService>();

var app = builder.Build();
app.MapGet("/generate-report", async (ICellReportService reportService) =>
{
    // 生成 AOT 优化的报表
    var result = await reportService.GenerateReportAsync(
        new ReportConfig { ReportType = ReportType.Excel, EnableAotOptimization = true },
        new ReportData { DataSource = new ListReportDataSource(new List<object>()) });
    return Results.Ok(new { Message = "报表生成成功", Path = result });
});

await app.RunAsync();
```

### 高级配置

```csharp
// 配置 cellreport 服务
builder.Services.AddCellReport(options =>
{
    // 配置 AOT 优化
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
    
    // 配置缓存
    options.CacheOptions.EnableCache = true;
    options.CacheOptions.CacheSize = 1000;
    options.CacheOptions.CacheExpiration = TimeSpan.FromHours(1);
    
    // 配置日志
    options.LoggingOptions.EnableDetailedLogging = true;
    options.LoggingOptions.LogLevel = LogLevel.Information;
});

// 配置应用
var app = builder.Build();

// 使用 cellreport 中间件
app.UseCellReport();

await app.RunAsync();
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
</ItemGroup>
```

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的 cellreport 版本和依赖库支持 AOT 编译
2. **避免反射**: 避免在报表处理中使用反射
3. **资源处理**: 确保所有模板资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用值类型**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试
7. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库

## 配置选项

### cellreport 配置

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| EnableAotOptimization | bool | false | 启用 AOT 优化 |
| EnableTrimOptimization | bool | false | 启用修剪优化 |
| EnableCache | bool | true | 启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| CacheExpiration | TimeSpan | 01:00:00 | 缓存过期时间 |
| EnableDetailedLogging | bool | false | 启用详细日志记录 |
| LogLevel | LogLevel | Information | 日志级别 |
| Timeout | TimeSpan | 00:05:00 | 操作超时时间 |

### 应用配置示例

```json
{
  "CellReport": {
    "EnableAotOptimization": true,
    "EnableTrimOptimization": true,
    "EnableCache": true,
    "CacheSize": 1000,
    "CacheExpiration": "01:00:00",
    "EnableDetailedLogging": false,
    "LogLevel": "Information",
    "Timeout": "00:05:00"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "CellReport": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化数据源**: 减少不必要的数据查询和处理
4. **使用缓存**: 对频繁使用的报表模板和数据进行缓存
5. **优化报表设计**: 避免过于复杂的报表结构
6. **使用内存池**: 对于频繁分配的内存，使用 ArrayPool<T> 减少 GC 压力
7. **并行处理**: 对于大型报表，考虑使用并行处理提高生成速度
8. **数据分页**: 对于大数据集，使用数据分页减少内存占用
9. **连接池管理**: 对于数据库数据源，优化连接池配置
10. **监控性能**: 使用 OpenTelemetry 等工具监控报表生成性能

## 故障排除

### 常见问题

1. **报表生成失败**
   - 检查模板文件路径是否正确
   - 验证数据源格式是否符合要求
   - 查看详细日志
   - 检查权限设置

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode
   - 使用 AOT 分析工具检查问题

3. **性能问题**
   - 使用性能分析工具定位瓶颈
   - 优化数据源查询
   - 启用缓存
   - 考虑使用 AOT 编译
   - 优化报表设计
   - 增加资源限制

4. **报表格式问题**
   - 检查模板设计是否正确
   - 验证数据类型匹配
   - 查看报表生成配置
   - 检查输出格式设置

5. **缓存问题**
   - 检查缓存配置是否正确
   - 验证缓存键生成逻辑
   - 检查缓存过期时间设置
   - 考虑清除缓存

## 扩展开发

### 创建自定义数据源

```csharp
// 自定义 JSON 数据源
public class JsonReportDataSource : IReportDataSource
{
    private readonly string _jsonData;
    
    public JsonReportDataSource(string jsonData)
    {
        _jsonData = jsonData;
    }
    
    public async Task<object> GetDataAsync()
    {
        // 解析 JSON 数据
        return await Task.FromResult(JsonSerializer.Deserialize<dynamic>(_jsonData));
    }
    
    public async Task<object> GetDataAsync(string query, params object[] parameters)
    {
        // 支持简单的 JSON 查询
        var data = await GetDataAsync();
        // 实现查询逻辑
        return data;
    }
    
    public async Task<int> GetDataCountAsync()
    {
        var data = await GetDataAsync() as JsonElement;
        return data.GetArrayLength();
    }
}
```

### 创建自定义报表处理器

```csharp
// 自定义 PDF 报表处理器
public class PdfReportProcessor : IReportProcessor
{
    public Task<string> ProcessReportAsync(ReportConfig config, ReportData data)
    {
        // 实现 PDF 报表生成逻辑
        _logger.LogInformation("生成 PDF 报表...");
        // PDF 生成代码
        return Task.FromResult("report-output.pdf");
    }
    
    public bool SupportsReportType(ReportType reportType)
    {
        return reportType == ReportType.Pdf;
    }
}

// 注册自定义处理器
builder.Services.AddSingleton<IReportProcessor, PdfReportProcessor>();
```

### 创建自定义中间件

```csharp
// 自定义报表监控中间件
public class ReportMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ReportMonitoringMiddleware> _logger;
    private readonly IReportMetricsService _metricsService;
    
    public ReportMonitoringMiddleware(RequestDelegate next, ILogger<ReportMonitoringMiddleware> logger, IReportMetricsService metricsService)
    {
        _next = next;
        _logger = logger;
        _metricsService = metricsService;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // 记录请求开始时间
        var startTime = DateTime.UtcNow;
        var reportId = Guid.NewGuid().ToString();
        
        // 设置请求上下文
        context.Items["ReportId"] = reportId;
        
        try
        {
            // 执行下一个中间件
            await _next(context);
            
            // 记录成功指标
            var duration = DateTime.UtcNow - startTime;
            _metricsService.RecordReportGenerationSuccess(reportId, duration);
        }
        catch (Exception ex)
        {
            // 记录失败指标
            var duration = DateTime.UtcNow - startTime;
            _metricsService.RecordReportGenerationFailure(reportId, duration, ex.Message);
            throw;
        }
    }
}

// 扩展方法
public static class ReportMonitoringMiddlewareExtensions
{
    public static IApplicationBuilder UseReportMonitoring(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ReportMonitoringMiddleware>();
    }
}

// 使用中间件
app.UseReportMonitoring();
```

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly ICellReportService _cellReportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(ICellReportService cellReportService, ILogger<ReportController> logger)
    {
        _cellReportService = cellReportService;
        _logger = logger;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(ReportResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
    {
        _logger.LogInformation("生成报表请求: {ReportType}", request.ReportType);
        
        try
        {
            // 生成报表
            var reportPath = await _cellReportService.GenerateReportAsync(
                new ReportConfig { ReportType = request.ReportType, TemplatePath = request.TemplatePath },
                new ReportData { DataSource = new ListReportDataSource(request.DataSource) });

            // 返回报表结果
            return Ok(new ReportResult {
                Success = true,
                Message = "报表生成成功",
                ReportPath = reportPath,
                ReportType = request.ReportType,
                GeneratedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "报表生成失败: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse {
                Success = false,
                Message = "报表生成失败",
                ErrorCode = "REPORT_GENERATION_FAILED",
                Details = ex.Message
            });
        }
    }
}
```

### 与 EF Core 集成

```csharp
// 配置 EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// EF Core 数据源实现
public class EfCoreReportDataSource : IReportDataSource
{
    private readonly AppDbContext _dbContext;
    
    public EfCoreReportDataSource(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<object> GetDataAsync()
    {
        // 从数据库获取数据
        return await _dbContext.Products.ToListAsync();
    }
    
    public async Task<object> GetDataAsync(string query, params object[] parameters)
    {
        // 使用 LINQ 查询数据
        return await _dbContext.Products.Where(p => p.Price > (decimal)parameters[0]).ToListAsync();
    }
    
    public async Task<int> GetDataCountAsync()
    {
        return await _dbContext.Products.CountAsync();
    }
}

// 注册服务
builder.Services.AddScoped<IReportDataSource, EfCoreReportDataSource>();
builder.Services.AddScoped<ICellReportService, CellReportService>();

// 使用 EF Core 数据源生成报表
var dataSource = new EfCoreReportDataSource(dbContext);
var reportData = new ReportData { DataSource = dataSource };
var result = await cellReportService.GenerateReportAsync(reportConfig, reportData);
```

## 最佳实践

1. **使用模板驱动设计**: 采用模板驱动的报表生成方式，提高开发效率和可维护性
2. **分离业务逻辑和报表逻辑**: 将业务逻辑与报表生成逻辑分离，便于测试和维护
3. **实现适当的错误处理**: 提供清晰的错误信息和日志记录
4. **使用依赖注入**: 避免硬编码依赖，提高代码的可测试性和可扩展性
5. **编写单元测试**: 测试报表生成逻辑和业务规则
6. **考虑并发处理**: 对于高并发场景，设计线程安全的报表生成逻辑
7. **监控和日志**: 实现全面的监控和日志记录，便于问题定位和性能优化
8. **考虑安全性**: 确保报表生成过程中的数据安全和访问控制
9. **使用 AOT 编译**: 对于性能敏感场景，考虑使用 AOT 编译提高性能
10. **优化资源使用**: 合理配置缓存、连接池等资源，提高系统可靠性和性能
11. **设计可扩展的架构**: 采用模块化设计，便于未来扩展和维护
12. **文档化设计**: 提供清晰的文档，便于其他开发者使用和维护

## 总结

cellreport 是一个基于 .NET 10 的现代化单元格报表框架，具有高性能、模块化、可扩展等特点。通过 cellreport，开发者可以快速构建高性能、可维护的报表系统，适用于各种规模的应用场景。

本参考文档提供了 cellreport 的核心组件、使用示例、配置选项、性能优化建议、故障排除指南和扩展开发方法，帮助开发者充分利用 cellreport 框架的优势，构建高质量的报表服务。

cellreport 支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。通过遵循本文档中的最佳实践和 AOT 兼容性建议，开发者可以构建出高性能、可靠的报表系统。