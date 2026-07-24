# charts - 参考文档

## 概述

charts 是基于 .NET 10 构建的高性能图表生成系统，为 .NET 开发者提供强大的图表生成功能，支持 AOT（提前编译）编译，适用于各种数据可视化场景。

## 核心组件

### 1. 图表服务 (IChartsService)

- **位置**: scripts/echarts_integration.cs 和 scripts/highcharts_integration.cs
- **功能**: 图表生成的核心业务逻辑处理
- **特性**: 
  - 多种图表类型的生成支持
  - 图表配置和样式管理
  - 图表导出功能
  - 高性能设计，支持高并发场景
  - 完善的错误处理和日志记录
  - AOT 编译优化支持
  - 异步编程模型
- **使用示例**: 
  ```csharp
  public class ChartsService : IChartsService
  {
      private readonly ILogger<ChartsService> _logger;
      private readonly ChartsSettings _settings;
      private readonly Dictionary<string, ChartResult> _chartCache = new();
      private readonly List<IChartGenerator> _chartGenerators = new();
      
      public ChartsService(ILogger<ChartsService> logger, IOptions<ChartsSettings> settings, IEnumerable<IChartGenerator> chartGenerators)
      {
          _logger = logger;
          _settings = settings.Value;
          _chartGenerators.AddRange(chartGenerators);
      }
      
      public async Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig)
      {
          _logger.LogInformation("生成图表: {ChartType}, 标题: {Title}", 
              chartConfig.ChartType, chartConfig.Title);
          
          // 检查缓存
          if (_settings.EnableCache)
          {
              var cacheKey = GenerateCacheKey(chartConfig);
              if (_chartCache.TryGetValue(cacheKey, out var cachedResult))
              {
                  return cachedResult;
              }
          }
          
          // 查找合适的图表生成器
          var chartGenerator = _chartGenerators.FirstOrDefault(g => g.CanHandle(chartConfig.ChartType));
          if (chartGenerator == null)
          {
              return new ChartResult {
                  Success = false,
                  Message = $"不支持的图表类型: {chartConfig.ChartType}"
              };
          }
          
          // 生成图表
          var result = await chartGenerator.GenerateChartAsync(chartConfig);
          
          // 缓存结果
          if (_settings.EnableCache && result.Success)
          {
              var cacheKey = GenerateCacheKey(chartConfig);
              _chartCache[cacheKey] = result;
              // 限制缓存大小
              if (_chartCache.Count > _settings.CacheSize)
              {
                  _chartCache.Remove(_chartCache.First().Key);
              }
          }
          
          return result;
      }
      
      public async Task<ExportResult> ExportChartAsync(string chartId, ExportFormat format)
      {
          _logger.LogInformation("导出图表: {ChartId}, 格式: {Format}", chartId, format);
          
          // 实现图表导出逻辑
          await Task.Delay(100);
          
          return new ExportResult {
              Success = true,
              ExportUrl = $"/export/{chartId}.{format.ToString().ToLower()}",
              Message = "图表导出成功"
          };
      }
      
      private string GenerateCacheKey(ChartConfig chartConfig)
      {
          // 生成缓存键
          return $"{chartConfig.ChartType}_{chartConfig.Title}_{string.Join('_', chartConfig.XAxis.Data)}";
      }
  }
  ```

### 2. 图表配置 (ChartConfig)

- **功能**: 定义图表生成的配置参数
- **特性**: 
  - 支持多种图表类型
  - 灵活的样式配置
  - 数据源配置
  - AOT 优化开关
  - 响应式设计支持
- **使用示例**: 
  ```csharp
  public class ChartConfig
  {
      public ChartType ChartType { get; set; }
      public string Title { get; set; } = string.Empty;
      public Axis XAxis { get; set; } = new();
      public Axis YAxis { get; set; } = new();
      public List<Series> Series { get; set; } = new();
      public Theme Theme { get; set; } = Theme.Default;
      public ResponsiveConfig Responsive { get; set; } = new();
      public bool EnableAotOptimization { get; set; } = false;
      public string Description { get; set; } = string.Empty;
      public Dictionary<string, object> AdditionalOptions { get; set; } = new();
  }
  
  public class Axis
  {
      public string Name { get; set; } = string.Empty;
      public string[] Data { get; set; } = Array.Empty<string>();
      public AxisType Type { get; set; } = AxisType.Category;
      public TextStyle TextStyle { get; set; } = new();
  }
  
  public class Series
  {
      public string Name { get; set; } = string.Empty;
      public object[] Data { get; set; } = Array.Empty<object>();
      public SeriesType Type { get; set; } = SeriesType.Default;
      public bool EnableAotOptimization { get; set; } = false;
      public Dictionary<string, object> AdditionalOptions { get; set; } = new();
  }
  ```

### 3. 图表生成器 (IChartGenerator)

- **功能**: 具体图表类型的生成实现
- **特性**: 
  - 支持多种图表类型
  - 插件式设计，便于扩展
  - AOT 编译优化支持
  - 异步编程模型
- **使用示例**: 
  ```csharp
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public interface IChartGenerator
  {
      Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig);
      bool CanHandle(ChartType chartType);
  }
  
  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
  public class EChartsGenerator : IChartGenerator
  {
      private readonly ILogger<EChartsGenerator> _logger;
      
      public EChartsGenerator(ILogger<EChartsGenerator> logger)
      {
          _logger = logger;
      }
      
      public async Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig)
      {
          _logger.LogInformation("使用 ECharts 生成图表: {ChartType}", chartConfig.ChartType);
          
          // 实现 ECharts 图表生成逻辑
          await Task.Delay(100);
          
          return new ChartResult {
              Success = true,
              ChartId = Guid.NewGuid().ToString(),
              ChartData = "{ /* ECharts 图表配置 */ }",
              Message = "ECharts 图表生成成功"
          };
      }
      
      public bool CanHandle(ChartType chartType)
      {
          // 支持所有图表类型
          return true;
      }
  }
  ```

### 4. 图表设置 (ChartsSettings)

- **功能**: 全局图表配置
- **特性**: 
  - AOT 优化配置
  - 缓存设置
  - 超时设置
  - 日志配置
  - 默认图表库配置
- **使用示例**: 
  ```csharp
  public class ChartsSettings
  {
      public bool EnableAotOptimization { get; set; } = false;
      public bool EnableTrimOptimization { get; set; } = false;
      public ChartLibrary DefaultChartLibrary { get; set; } = ChartLibrary.ECharts;
      public bool EnableCache { get; set; } = true;
      public int CacheSize { get; set; } = 1000;
      public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
      public LogLevel LogLevel { get; set; } = LogLevel.Information;
      public bool EnableDetailedLogging { get; set; } = false;
      public int MaxConcurrentRequests { get; set; } = 100;
  }
  ```

## 使用示例

### 基础使用

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Charts;

// 配置服务
var services = new ServiceCollection();

// 添加日志服务
services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

// 注册图表服务
services.AddCharts();

// 构建服务提供器
var serviceProvider = services.BuildServiceProvider();

// 获取图表服务
var chartsService = serviceProvider.GetRequiredService<IChartsService>();

// 创建图表配置
var chartConfig = new ChartConfig
{
    ChartType = ChartType.Line,
    Title = "销售趋势图",
    XAxis = new Axis { Data = new[] { "1月", "2月", "3月", "4月", "5月", "6月" } },
    Series = new List<Series>
    {
        new Series {
            Name = "销售额",
            Data = new[] { 120, 200, 150, 80, 70, 110 }
        }
    }
};

// 生成图表
Console.WriteLine("开始生成图表...");
var result = await chartsService.GenerateChartAsync(chartConfig);
Console.WriteLine($"图表生成结果: {result.Success}");
Console.WriteLine($"图表数据: {result.ChartData}");
Console.WriteLine($"图表 ID: {result.ChartId}");

// 导出图表
var exportResult = await chartsService.ExportChartAsync(result.ChartId, ExportFormat.PNG);
Console.WriteLine($"图表导出结果: {exportResult.Success}");
Console.WriteLine($"导出 URL: {exportResult.ExportUrl}");
```

### AOT 编译优化示例

```csharp
#:sdk Microsoft.NET.Sdk
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Charts;

// 图表类型枚举
public enum ChartType
{
    Line,
    Bar,
    Pie,
    Scatter,
    Map,
    Radar,
    Heatmap,
    Tree
}

// 图表库枚举
public enum ChartLibrary
{
    ECharts,
    Highcharts
}

// 导出格式枚举
public enum ExportFormat
{
    PNG,
    JPG,
    PDF,
    SVG
}

// 坐标轴类型枚举
public enum AxisType
{
    Category,
    Value,
    Time,
    Log
}

// 系列类型枚举
public enum SeriesType
{
    Default,
    Smooth,
    Stack
}

// 主题枚举
public enum Theme
{
    Default,
    Light,
    Dark,
    Vintage,
    Macarons,
    Infographic
}

// 图表结果类
public class ChartResult
{
    public bool Success { get; set; }
    public string ChartId { get; set; } = string.Empty;
    public string ChartData { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

// 导出结果类
public class ExportResult
{
    public bool Success { get; set; }
    public string ExportUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
}

// 坐标轴配置类
public class Axis
{
    public string Name { get; set; } = string.Empty;
    public string[] Data { get; set; } = Array.Empty<string>();
    public AxisType Type { get; set; } = AxisType.Category;
    public TextStyle TextStyle { get; set; } = new();
}

// 文本样式类
public class TextStyle
{
    public string Color { get; set; } = "#333";
    public int FontSize { get; set; } = 12;
    public string FontFamily { get; set; } = "Arial";
}

// 系列配置类
public class Series
{
    public string Name { get; set; } = string.Empty;
    public object[] Data { get; set; } = Array.Empty<object>();
    public SeriesType Type { get; set; } = SeriesType.Default;
    public bool EnableAotOptimization { get; set; } = false;
}

// 响应式配置类
public class ResponsiveConfig
{
    public bool EnableResponsive { get; set; } = true;
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 400;
    public bool MaintainAspectRatio { get; set; } = true;
}

// 图表配置类
public class ChartConfig
{
    public ChartType ChartType { get; set; }
    public string Title { get; set; } = string.Empty;
    public Axis XAxis { get; set; } = new();
    public Axis YAxis { get; set; } = new();
    public List<Series> Series { get; set; } = new();
    public Theme Theme { get; set; } = Theme.Default;
    public ResponsiveConfig Responsive { get; set; } = new();
    public bool EnableAotOptimization { get; set; } = false;
}

// 图表设置类
public class ChartsSettings
{
    public bool EnableAotOptimization { get; set; }
    public bool EnableTrimOptimization { get; set; }
    public ChartLibrary DefaultChartLibrary { get; set; }
    public bool EnableCache { get; set; }
    public int CacheSize { get; set; }
    public TimeSpan CacheExpiration { get; set; }
    public TimeSpan Timeout { get; set; }
    public LogLevel LogLevel { get; set; }
}

// AOT 安全的图表服务接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IChartsService
{
    Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig);
    Task<ExportResult> ExportChartAsync(string chartId, ExportFormat format);
}

// AOT 安全的图表生成器接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IChartGenerator
{
    Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig);
    bool CanHandle(ChartType chartType);
}

// AOT 安全的 ECharts 生成器
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeEChartsGenerator : IChartGenerator
{
    private readonly ILogger<AotSafeEChartsGenerator> _logger;
    
    public AotSafeEChartsGenerator(ILogger<AotSafeEChartsGenerator> logger)
    {
        _logger = logger;
    }
    
    public async Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig)
    {
        _logger.LogInformation("使用 AOT 优化的 ECharts 生成图表: {ChartType}", chartConfig.ChartType);
        
        // AOT 安全的图表生成逻辑
        await Task.Delay(100);
        
        return new ChartResult {
            Success = true,
            ChartId = Guid.NewGuid().ToString(),
            ChartData = "{ /* AOT 优化的 ECharts 配置 */ }",
            Message = "AOT 优化的 ECharts 图表生成成功"
        };
    }
    
    public bool CanHandle(ChartType chartType)
    {
        // 支持所有图表类型
        return true;
    }
}

// AOT 安全的图表服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeChartsService : IChartsService
{
    private readonly ILogger<AotSafeChartsService> _logger;
    private readonly ChartsSettings _settings;
    private readonly List<IChartGenerator> _chartGenerators = new();
    
    public AotSafeChartsService(ILogger<AotSafeChartsService> logger, IOptions<ChartsSettings> settings, IEnumerable<IChartGenerator> chartGenerators)
    {
        _logger = logger;
        _settings = settings.Value;
        _chartGenerators.AddRange(chartGenerators);
    }
    
    public async Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig)
    {
        _logger.LogInformation("使用 AOT 优化的图表服务生成图表");
        
        // 查找合适的图表生成器
        var chartGenerator = _chartGenerators.FirstOrDefault(g => g.CanHandle(chartConfig.ChartType));
        if (chartGenerator == null)
        {
            return new ChartResult {
                Success = false,
                Message = $"不支持的图表类型: {chartConfig.ChartType}"
            };
        }
        
        // 生成图表
        return await chartGenerator.GenerateChartAsync(chartConfig);
    }
    
    public async Task<ExportResult> ExportChartAsync(string chartId, ExportFormat format)
    {
        _logger.LogInformation("使用 AOT 优化的图表服务导出图表");
        
        // AOT 安全的图表导出逻辑
        await Task.Delay(100);
        
        return new ExportResult {
            Success = true,
            ExportUrl = $"/export/{chartId}.{format.ToString().ToLower()}",
            Message = "AOT 优化的图表导出成功"
        };
    }
}

// 图表服务扩展类
public static class ChartsServiceExtensions
{
    public static IServiceCollection AddCharts(this IServiceCollection services)
    {
        services.AddSingleton<IChartGenerator, AotSafeEChartsGenerator>();
        services.AddSingleton<IChartsService, AotSafeChartsService>();
        services.AddOptions<ChartsSettings>();
        return services;
    }
    
    public static IServiceCollection AddCharts(this IServiceCollection services, Action<ChartsSettings> configureOptions)
    {
        services.AddCharts();
        services.Configure(configureOptions);
        return services;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("图表生成 AOT 编译优化示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 AOT 优化的图表服务
        services.AddCharts(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
            options.DefaultChartLibrary = ChartLibrary.ECharts;
            options.EnableCache = true;
            options.CacheSize = 1000;
        });
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        var chartsService = serviceProvider.GetRequiredService<IChartsService>();
        
        Console.WriteLine("\n1. 执行 AOT 优化的折线图生成:");
        Console.WriteLine("-" * 40);
        
        // 创建 AOT 优化的图表配置
        var chartConfig = new ChartConfig
        {
            ChartType = ChartType.Line,
            Title = "AOT 优化的销售趋势图",
            XAxis = new Axis { Data = new[] { "1月", "2月", "3月", "4月", "5月", "6月" } },
            Series = new List<Series>
            {
                new Series {
                    Name = "销售额",
                    Data = new[] { 120, 200, 150, 80, 70, 110 },
                    EnableAotOptimization = true
                }
            },
            EnableAotOptimization = true
        };
        
        // 执行 AOT 优化的图表生成
        var result = await chartsService.GenerateChartAsync(chartConfig);
        Console.WriteLine($"结果: {result.Success}");
        Console.WriteLine($"消息: {result.Message}");
        Console.WriteLine($"图表 ID: {result.ChartId}");
        
        Console.WriteLine("\n2. 执行 AOT 优化的图表导出:");
        Console.WriteLine("-" * 40);
        
        // 执行 AOT 优化的图表导出
        var exportResult = await chartsService.ExportChartAsync(result.ChartId, ExportFormat.PNG);
        Console.WriteLine($"结果: {exportResult.Success}");
        Console.WriteLine($"消息: {exportResult.Message}");
        Console.WriteLine($"导出 URL: {exportResult.ExportUrl}");
    }
}
```

### 高级配置

```csharp
// 配置应用程序
var builder = WebApplication.CreateBuilder(args);

// 配置日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 注册服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 高级图表服务配置
builder.Services.AddCharts(options => {
    // AOT 优化配置
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
    
    // 缓存配置
    options.EnableCache = true;
    options.CacheSize = 2000;
    options.CacheExpiration = TimeSpan.FromHours(2);
    
    // 其他配置
    options.DefaultChartLibrary = ChartLibrary.Highcharts;
    options.Timeout = TimeSpan.FromSeconds(60);
    options.LogLevel = LogLevel.Information;
    options.EnableDetailedLogging = true;
    options.MaxConcurrentRequests = 200;
});

var app = builder.Build();

// 配置中间件
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

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

1. **使用 AOT 兼容的库**: 确保使用的图表库版本和依赖库支持 AOT 编译
2. **避免反射**: 避免在图表生成逻辑中使用反射
3. **资源处理**: 确保所有资源在 AOT 编译时能被正确处理
4. **动态代码生成**: 避免使用动态代码生成技术
5. **使用值类型**: 优先使用值类型而非引用类型，减少内存分配
6. **测试验证**: 在 AOT 编译后进行充分测试
7. **使用 AOT 兼容的序列化**: 优先使用 System.Text.Json 等 AOT 兼容的序列化库
8. **使用 DynamicallyAccessedMembers 特性**: 为 AOT 编译提供必要的类型信息

## 配置选项

### 图表服务配置

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| EnableAotOptimization | bool | false | 启用 AOT 优化 |
| EnableTrimOptimization | bool | false | 启用修剪优化 |
| DefaultChartLibrary | ChartLibrary | ECharts | 默认图表库 |
| EnableCache | bool | true | 启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| CacheExpiration | TimeSpan | 01:00:00 | 缓存过期时间 |
| Timeout | TimeSpan | 00:00:30 | 操作超时时间 |
| LogLevel | LogLevel | Information | 日志级别 |
| EnableDetailedLogging | bool | false | 启用详细日志记录 |
| MaxConcurrentRequests | int | 100 | 最大并发请求数 |

### 应用配置示例

```json
{
  "Charts": {
    "EnableAotOptimization": true,
    "EnableTrimOptimization": true,
    "DefaultChartLibrary": "ECharts",
    "EnableCache": true,
    "CacheSize": 2000,
    "CacheExpiration": "02:00:00",
    "Timeout": "00:01:00",
    "LogLevel": "Information",
    "EnableDetailedLogging": false,
    "MaxConcurrentRequests": 200
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Charts": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

## 性能优化

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **启用缓存**: 对频繁生成的图表进行缓存，提高响应速度
4. **优化数据处理**: 在生成图表前优化数据处理逻辑
5. **选择合适的图表库**: 根据需求选择合适的图表库
6. **优化图表数据量**: 减少不必要的数据点，提高渲染性能
7. **使用高效的序列化**: 优先使用 System.Text.Json 等高效的序列化库
8. **调整缓存设置**: 根据实际使用情况调整缓存大小和过期时间
9. **限制并发请求数**: 根据系统资源调整最大并发请求数
10. **监控性能**: 使用 OpenTelemetry 等工具监控图表生成性能

## 故障排除

### 常见问题

1. **图表生成失败**
   - 检查图表配置是否正确
   - 验证服务注册是否完整
   - 查看详细日志
   - 检查图表库是否支持该图表类型

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode
   - 使用 AOT 分析工具检查问题
   - 添加必要的 DynamicallyAccessedMembers 特性

3. **图表渲染性能问题**
   - 优化图表数据量
   - 启用 AOT 编译
   - 使用缓存
   - 选择更轻量级的图表类型
   - 优化图表配置，减少不必要的样式和动画

4. **图表导出失败**
   - 检查导出格式是否支持
   - 验证图表 ID 是否有效
   - 查看详细日志
   - 检查导出服务是否正常运行

5. **服务连接问题**
   - 检查网络连接
   - 验证服务地址和端口
   - 检查防火墙设置
   - 查看详细日志

## 扩展开发

### 创建自定义图表类型

```csharp
// 自定义图表类型枚举扩展
enum CustomChartType
{
    Funnel,
    Sankey,
    WordCloud
}

// 自定义图表生成器
public class CustomChartGenerator : IChartGenerator
{
    private readonly ILogger<CustomChartGenerator> _logger;
    
    public CustomChartGenerator(ILogger<CustomChartGenerator> logger)
    {
        _logger = logger;
    }
    
    public async Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig)
    {
        _logger.LogInformation("生成自定义图表: {ChartType}", chartConfig.ChartType);
        
        // 实现自定义图表生成逻辑
        await Task.Delay(100);
        
        return new ChartResult {
            Success = true,
            ChartId = Guid.NewGuid().ToString(),
            ChartData = "{ /* 自定义图表数据 */ }",
            Message = "自定义图表生成成功"
        };
    }
    
    public bool CanHandle(ChartType chartType)
    {
        // 检查是否可以处理该图表类型
        return chartType == (ChartType)CustomChartType.WordCloud;
    }
}

// 注册自定义图表生成器
builder.Services.AddSingleton<IChartGenerator, CustomChartGenerator>();
builder.Services.AddCharts();
```

### 创建自定义图表库集成

```csharp
// 自定义图表库生成器
public class CustomChartLibraryGenerator : IChartGenerator
{
    private readonly ILogger<CustomChartLibraryGenerator> _logger;
    
    public CustomChartLibraryGenerator(ILogger<CustomChartLibraryGenerator> logger)
    {
        _logger = logger;
    }
    
    public async Task<ChartResult> GenerateChartAsync(ChartConfig chartConfig)
    {
        _logger.LogInformation("使用自定义图表库生成图表: {ChartType}", chartConfig.ChartType);
        
        // 实现自定义图表库集成逻辑
        await Task.Delay(100);
        
        return new ChartResult {
            Success = true,
            ChartId = Guid.NewGuid().ToString(),
            ChartData = "{ /* 自定义图表库配置 */ }",
            Message = "自定义图表库图表生成成功"
        };
    }
    
    public bool CanHandle(ChartType chartType)
    {
        // 支持所有图表类型
        return true;
    }
}

// 注册自定义图表库生成器
builder.Services.AddSingleton<IChartGenerator, CustomChartLibraryGenerator>();
builder.Services.AddCharts(options => {
    options.DefaultChartLibrary = (ChartLibrary)999; // 自定义图表库枚举值
});
```

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
[ApiController]
[Route("api/[controller]")]
public class ChartsController : ControllerBase
{
    private readonly IChartsService _chartsService;
    private readonly ILogger<ChartsController> _logger;

    public ChartsController(IChartsService chartsService, ILogger<ChartsController> logger)
    {
        _chartsService = chartsService;
        _logger = logger;
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(ChartResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateChart([FromBody] ChartConfig request)
    {
        _logger.LogInformation("API 图表生成请求: {ChartType}, 标题: {Title}", 
            request.ChartType, request.Title);
        
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(request.Title))
            {
                return BadRequest(new ErrorResponse {
                    Success = false,
                    Message = "图表标题不能为空",
                    ErrorCode = "INVALID_TITLE",
                    Timestamp = DateTime.UtcNow
                });
            }
            
            // 执行图表生成
            var result = await _chartsService.GenerateChartAsync(request);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 图表生成失败: {Message}", ex.Message);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse {
                Success = false,
                Message = "图表生成失败",
                ErrorCode = "CHART_GENERATION_FAILED",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpPost("export/{chartId}")]
    [ProducesResponseType(typeof(ExportResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportChart(string chartId, [FromQuery] ExportFormat format)
    {
        _logger.LogInformation("API 图表导出请求: {ChartId}, 格式: {Format}", chartId, format);
        
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(chartId))
            {
                return BadRequest(new ErrorResponse {
                    Success = false,
                    Message = "图表 ID 不能为空",
                    ErrorCode = "INVALID_CHART_ID",
                    Timestamp = DateTime.UtcNow
                });
            }
            
            // 执行图表导出
            var result = await _chartsService.ExportChartAsync(chartId, format);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 图表导出失败: {Message}", ex.Message);
            
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse {
                Success = false,
                Message = "图表导出失败",
                ErrorCode = "CHART_EXPORT_FAILED",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new HealthResponse {
            Success = true,
            Status = "Healthy",
            Service = "Charts API",
            Timestamp = DateTime.UtcNow
        });
    }
}

// 错误响应类
public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

// 健康响应类
public class HealthResponse
{
    public bool Success { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
```

### 与消息队列集成

```csharp
// 消息队列消费者
public class ChartMessageConsumer : BackgroundService
{
    private readonly IChartsService _chartsService;
    private readonly ILogger<ChartMessageConsumer> _logger;
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public ChartMessageConsumer(IChartsService chartsService, ILogger<ChartMessageConsumer> logger, IMessageQueueClient messageQueueClient)
    {
        _chartsService = chartsService;
        _logger = logger;
        _messageQueueClient = messageQueueClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("图表消息消费者已启动");
        
        await _messageQueueClient.SubscribeAsync("chart-commands", async (message) => {
            try
            {
                _logger.LogInformation("收到图表命令: {MessageType}, 消息 ID: {MessageId}", 
                    message.MessageType, message.MessageId);
                
                switch (message.MessageType)
                {
                    case "GenerateChart":
                        await HandleGenerateChartCommandAsync(message, stoppingToken);
                        break;
                    
                    case "ExportChart":
                        await HandleExportChartCommandAsync(message, stoppingToken);
                        break;
                    
                    default:
                        _logger.LogWarning("未知的图表命令类型: {MessageType}", message.MessageType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理图表命令失败: {Message}", ex.Message);
            }
        }, stoppingToken);
        
        _logger.LogInformation("图表消息消费者已完成初始化");
    }
    
    private async Task HandleGenerateChartCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析图表配置
        var chartConfig = JsonSerializer.Deserialize<ChartConfig>(message.Body, _jsonOptions);
        if (chartConfig == null)
        {
            _logger.LogError("无法解析图表配置: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行图表生成命令: {ChartType}, 标题: {Title}", 
            chartConfig.ChartType, chartConfig.Title);
        
        // 执行图表生成
        var result = await _chartsService.GenerateChartAsync(chartConfig);
        
        _logger.LogInformation("图表生成命令执行完成: {Title}, 结果: {Success}", 
            chartConfig.Title, result.Success);
        
        // 发布生成结果
        var resultMessage = new Message {
            MessageType = "ChartGenerated",
            Body = JsonSerializer.Serialize(result, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CorrelationId", message.MessageId }
            }
        };
        
        await _messageQueueClient.PublishAsync("chart-results", resultMessage, cancellationToken);
    }
    
    private async Task HandleExportChartCommandAsync(Message message, CancellationToken cancellationToken)
    {
        // 解析导出请求
        var exportRequest = JsonSerializer.Deserialize<ExportRequest>(message.Body, _jsonOptions);
        if (exportRequest == null)
        {
            _logger.LogError("无法解析图表导出请求: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行图表导出命令: {ChartId}, 格式: {Format}", 
            exportRequest.ChartId, exportRequest.Format);
        
        // 执行图表导出
        var result = await _chartsService.ExportChartAsync(exportRequest.ChartId, exportRequest.Format);
        
        _logger.LogInformation("图表导出命令执行完成: {ChartId}, 结果: {Success}", 
            exportRequest.ChartId, result.Success);
        
        // 发布导出结果
        var resultMessage = new Message {
            MessageType = "ChartExported",
            Body = JsonSerializer.Serialize(result, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CorrelationId", message.MessageId }
            }
        };
        
        await _messageQueueClient.PublishAsync("chart-results", resultMessage, cancellationToken);
    }
}

// 导出请求类
public class ExportRequest
{
    public string ChartId { get; set; } = string.Empty;
    public ExportFormat Format { get; set; }
}

// 注册消息消费者
builder.Services.AddHostedService<ChartMessageConsumer>();
```

## 最佳实践

1. **从简单开始**: 从简单的图表类型和配置开始，逐步增加复杂性
2. **使用依赖注入**: 利用依赖注入管理图表服务
3. **优化数据处理**: 在生成图表前优化数据处理逻辑
4. **使用异步编程**: 优先使用异步 API，提高并发处理能力
5. **缓存频繁生成的图表**: 对频繁生成的图表进行缓存，提高响应速度
6. **选择合适的图表类型**: 根据数据特点和展示需求选择合适的图表类型
7. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译
8. **测试不同场景**: 在不同场景下测试图表生成性能
9. **监控和日志**: 添加适当的监控和日志，便于故障排查
10. **持续优化**: 根据实际使用情况持续优化图表生成逻辑
11. **使用合适的图表库**: 根据项目需求选择合适的图表库
12. **优化图表配置**: 减少不必要的样式和动画，提高渲染性能
13. **考虑响应式设计**: 确保图表在不同屏幕尺寸下都能正常显示
14. **安全性考虑**: 确保图表生成和导出过程中的安全性
15. **文档化**: 为图表配置和使用方式提供详细的文档

## 总结

charts 是一个基于 .NET 10 的现代化图表生成框架，具有高性能、模块化、可扩展等特点。通过 charts，开发者可以快速构建各种类型的图表，支持多种图表库集成，适用于各种数据可视化场景。

本参考文档提供了 charts 的核心组件、使用示例、AOT 编译支持、配置选项、性能优化建议、故障排除指南和扩展开发方法，帮助开发者充分利用 charts 框架的优势，构建高质量的图表生成系统。

charts 支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。通过遵循本文档中的最佳实践和 AOT 兼容性建议，开发者可以构建出高性能、可靠的图表生成系统，满足各种数据可视化需求。

数据可视化是现代应用程序的重要组成部分，通过 charts 框架，开发者可以轻松实现各种复杂的图表展示，提升应用程序的用户体验和数据洞察力。
