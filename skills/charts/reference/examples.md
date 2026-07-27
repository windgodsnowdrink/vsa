# charts - 使用示例

## 快速开始

### 1. 基础使用示例

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

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("图表生成基础使用示例");
        Console.WriteLine("=" * 50);
        
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
        
        Console.WriteLine("\n1. 执行折线图生成:");
        Console.WriteLine("-" * 30);
        
        // 创建折线图配置
        var lineChartConfig = new ChartConfig
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
        
        // 生成折线图
        var lineResult = await chartsService.GenerateChartAsync(lineChartConfig);
        Console.WriteLine($"结果: {lineResult.Success}");
        Console.WriteLine($"消息: {lineResult.Message}");
        Console.WriteLine($"图表 ID: {lineResult.ChartId}");
        
        Console.WriteLine("\n2. 执行柱状图生成:");
        Console.WriteLine("-" * 30);
        
        // 创建柱状图配置
        var barChartConfig = new ChartConfig
        {
            ChartType = ChartType.Bar,
            Title = "产品销量对比",
            XAxis = new Axis { Data = new[] { "产品A", "产品B", "产品C", "产品D", "产品E" } },
            Series = new List<Series>
            {
                new Series {
                    Name = "销量",
                    Data = new[] { 350, 280, 420, 190, 310 }
                }
            }
        };
        
        // 生成柱状图
        var barResult = await chartsService.GenerateChartAsync(barChartConfig);
        Console.WriteLine($"结果: {barResult.Success}");
        Console.WriteLine($"消息: {barResult.Message}");
        Console.WriteLine($"图表 ID: {barResult.ChartId}");
        
        Console.WriteLine("\n3. 执行饼图生成:");
        Console.WriteLine("-" * 30);
        
        // 创建饼图配置
        var pieChartConfig = new ChartConfig
        {
            ChartType = ChartType.Pie,
            Title = "市场份额分布",
            Series = new List<Series>
            {
                new Series {
                    Name = "市场份额",
                    Data = new[] {
                        new { Name = "公司A", Value = 35.5 },
                        new { Name = "公司B", Value = 28.3 },
                        new { Name = "公司C", Value = 19.2 },
                        new { Name = "其他", Value = 17.0 }
                    }
                }
            }
        };
        
        // 生成饼图
        var pieResult = await chartsService.GenerateChartAsync(pieChartConfig);
        Console.WriteLine($"结果: {pieResult.Success}");
        Console.WriteLine($"消息: {pieResult.Message}");
        Console.WriteLine($"图表 ID: {pieResult.ChartId}");
        
        Console.WriteLine("\n示例完成!");
    }
}
```

### 2. AOT 编译优化示例

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
using System.Diagnostics;
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
    public string Type { get; set; } = "category";
}

// 系列配置类
public class Series
{
    public string Name { get; set; } = string.Empty;
    public object[] Data { get; set; } = Array.Empty<object>();
    public bool EnableAotOptimization { get; set; } = false;
}

// 图表配置类
public class ChartConfig
{
    public ChartType ChartType { get; set; }
    public string Title { get; set; } = string.Empty;
    public Axis XAxis { get; set; } = new();
    public Axis YAxis { get; set; } = new();
    public List<Series> Series { get; set; } = new();
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
        var stopwatch = Stopwatch.StartNew();
        var result = await chartsService.GenerateChartAsync(chartConfig);
        stopwatch.Stop();
        
        Console.WriteLine($"结果: {result.Success}");
        Console.WriteLine($"消息: {result.Message}");
        Console.WriteLine($"图表 ID: {result.ChartId}");
        Console.WriteLine($"执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        
        Console.WriteLine("\n2. 执行 AOT 优化的图表导出:");
        Console.WriteLine("-" * 40);
        
        // 执行 AOT 优化的图表导出
        stopwatch.Restart();
        var exportResult = await chartsService.ExportChartAsync(result.ChartId, ExportFormat.PNG);
        stopwatch.Stop();
        
        Console.WriteLine($"结果: {exportResult.Success}");
        Console.WriteLine($"消息: {exportResult.Message}");
        Console.WriteLine($"导出 URL: {exportResult.ExportUrl}");
        Console.WriteLine($"执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        
        Console.WriteLine("\n3. 执行性能测试:");
        Console.WriteLine("-" * 40);
        
        // 执行性能测试
        const int iterations = 100;
        stopwatch.Restart();
        
        for (int i = 0; i < iterations; i++)
        {
            await chartsService.GenerateChartAsync(chartConfig);
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"执行 {iterations} 次图表生成");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均执行时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2} ms");
        Console.WriteLine($"每秒处理请求: {iterations / stopwatch.Elapsed.TotalSeconds:F2} RPS");
    }
}
```

### 3. 高级配置示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Charts;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("图表生成高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建配置
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "Charts:EnableAotOptimization", "true" },
                { "Charts:EnableTrimOptimization", "true" },
                { "Charts:DefaultChartLibrary", "ECharts" },
                { "Charts:EnableCache", "true" },
                { "Charts:CacheSize", "2000" },
                { "Charts:CacheExpiration", "02:00:00" },
                { "Charts:Timeout", "00:01:00" },
                { "Charts:LogLevel", "Information" },
                { "Charts:EnableDetailedLogging", "true" },
                { "Charts:MaxConcurrentRequests", "200" }
            })
            .Build();
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
            config.AddConfiguration(configuration.GetSection("Logging"));
        });
        
        // 添加配置
        services.AddSingleton<IConfiguration>(configuration);
        
        // 配置图表设置
        services.Configure<ChartsSettings>(configuration.GetSection("Charts"));
        
        // 注册图表服务
        services.AddCharts();
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置信息
        var chartsSettings = serviceProvider.GetRequiredService<IOptions<ChartsSettings>>().Value;
        Console.WriteLine("\n图表服务配置信息:");
        Console.WriteLine("-" * 30);
        Console.WriteLine($"AOT 优化: {chartsSettings.EnableAotOptimization}");
        Console.WriteLine($"修剪优化: {chartsSettings.EnableTrimOptimization}");
        Console.WriteLine($"默认图表库: {chartsSettings.DefaultChartLibrary}");
        Console.WriteLine($"启用缓存: {chartsSettings.EnableCache}");
        Console.WriteLine($"缓存大小: {chartsSettings.CacheSize}");
        
        // 获取图表服务
        var chartsService = serviceProvider.GetRequiredService<IChartsService>();
        
        // 创建高级图表配置
        var chartConfig = new ChartConfig
        {
            ChartType = ChartType.Radar,
            Title = "多维度性能分析",
            Series = new List<Series>
            {
                new Series {
                    Name = "系统性能",
                    Data = new[] {
                        new { Name = "响应时间", Value = 85 },
                        new { Name = "吞吐量", Value = 92 },
                        new { Name = "可用性", Value = 98 },
                        new { Name = "可靠性", Value = 95 },
                        new { Name = "可扩展性", Value = 88 },
                        new { Name = "安全性", Value = 94 }
                    }
                }
            },
            EnableAotOptimization = true
        };
        
        // 生成图表
        Console.WriteLine("\n执行高级配置的图表生成:");
        Console.WriteLine("-" * 30);
        
        var result = await chartsService.GenerateChartAsync(chartConfig);
        Console.WriteLine($"结果: {result.Success}");
        Console.WriteLine($"消息: {result.Message}");
        Console.WriteLine($"图表 ID: {result.ChartId}");
    }
}
```

### 4. 性能优化示例

```csharp
#:sdk Microsoft.NET.Sdk
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
using Microsoft.Extensions.Logging;
using Charts;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("图表生成性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Warning); // 降低日志级别以提高性能
        });
        
        // 注册图表服务，启用性能优化
        services.AddCharts(options => {
            options.EnableAotOptimization = true;
            options.DefaultChartLibrary = ChartLibrary.ECharts;
            options.EnableCache = true; // 启用缓存
            options.CacheSize = 5000; // 增加缓存大小
        });
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        var chartsService = serviceProvider.GetRequiredService<IChartsService>();
        
        // 创建高性能图表配置
        var chartConfig = new ChartConfig
        {
            ChartType = ChartType.Line,
            Title = "高性能销售趋势图",
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
        
        Console.WriteLine("\n执行性能测试:");
        Console.WriteLine("-" * 30);
        
        // 执行多次图表生成以测试性能
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();
        
        // 并行执行图表生成以测试并发性能
        var tasks = new List<Task<ChartResult>>();
        for (int i = 0; i < iterations; i++)
        {
            tasks.Add(chartsService.GenerateChartAsync(chartConfig));
        }
        
        // 等待所有任务完成
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // 计算成功次数
        var successCount = results.Count(r => r.Success);
        
        Console.WriteLine($"执行次数: {iterations}");
        Console.WriteLine($"成功次数: {successCount}");
        Console.WriteLine($"成功率: {successCount / (double)iterations:P2}");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均执行时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2} ms");
        Console.WriteLine($"每秒处理请求: {iterations / stopwatch.Elapsed.TotalSeconds:F2} RPS");
        
        // 测试缓存效果
        Console.WriteLine("\n测试缓存效果:");
        Console.WriteLine("-" * 30);
        
        stopwatch.Restart();
        // 再次执行相同的图表生成，应该命中缓存
        for (int i = 0; i < iterations; i++)
        {
            await chartsService.GenerateChartAsync(chartConfig);
        }
        stopwatch.Stop();
        
        Console.WriteLine($"使用缓存执行 {iterations} 次图表生成");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均执行时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2} ms");
        Console.WriteLine($"每秒处理请求: {iterations / stopwatch.Elapsed.TotalSeconds:F2} RPS");
        Console.WriteLine($"缓存效果提升: {iterations / stopwatch.Elapsed.TotalSeconds / (iterations / (stopwatch.Elapsed.TotalMilliseconds / 1000)):F2}x");
    }
}
```

### 5. Web API 集成示例

```csharp
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.Mvc.Core@10.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Charts;

// API 请求模型
public class GenerateChartRequest
{
    public ChartType ChartType { get; set; }
    public string Title { get; set; } = string.Empty;
    public Axis XAxis { get; set; } = new();
    public Axis YAxis { get; set; } = new();
    public List<Series> Series { get; set; } = new();
    public bool EnableAotOptimization { get; set; } = false;
}

// API 响应模型
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// API 控制器
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
    [ProducesResponseType(typeof(ApiResponse<ChartResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateChart([FromBody] GenerateChartRequest request)
    {
        _logger.LogInformation("API 图表生成请求: {ChartType}, 标题: {Title}", 
            request.ChartType, request.Title);
        
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(request.Title))
            {
                return BadRequest(new ApiResponse<object> {
                    Success = false,
                    Message = "图表标题不能为空",
                    ErrorCode = "INVALID_TITLE"
                });
            }
            
            if (request.Series == null || request.Series.Count == 0)
            {
                return BadRequest(new ApiResponse<object> {
                    Success = false,
                    Message = "图表数据不能为空",
                    ErrorCode = "INVALID_SERIES"
                });
            }
            
            // 转换为内部图表配置
            var chartConfig = new ChartConfig
            {
                ChartType = request.ChartType,
                Title = request.Title,
                XAxis = request.XAxis,
                YAxis = request.YAxis,
                Series = request.Series,
                EnableAotOptimization = request.EnableAotOptimization
            };
            
            // 生成图表
            var result = await _chartsService.GenerateChartAsync(chartConfig);
            
            // 返回成功响应
            return Ok(new ApiResponse<ChartResult> {
                Success = true,
                Data = result,
                Message = "图表生成成功"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 图表生成失败: {Message}", ex.Message);
            
            // 返回错误响应
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> {
                Success = false,
                Message = "图表生成失败",
                ErrorCode = "CHART_GENERATION_FAILED"
            });
        }
    }

    [HttpPost("export/{chartId}")]
    [ProducesResponseType(typeof(ApiResponse<ExportResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportChart(string chartId, [FromQuery] ExportFormat format)
    {
        _logger.LogInformation("API 图表导出请求: {ChartId}, 格式: {Format}", chartId, format);
        
        try
        {
            // 验证请求
            if (string.IsNullOrEmpty(chartId))
            {
                return BadRequest(new ApiResponse<object> {
                    Success = false,
                    Message = "图表 ID 不能为空",
                    ErrorCode = "INVALID_CHART_ID"
                });
            }
            
            // 执行图表导出
            var result = await _chartsService.ExportChartAsync(chartId, format);
            
            // 返回成功响应
            return Ok(new ApiResponse<ExportResult> {
                Success = true,
                Data = result,
                Message = "图表导出成功"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 图表导出失败: {Message}", ex.Message);
            
            // 返回错误响应
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object> {
                Success = false,
                Message = "图表导出失败",
                ErrorCode = "CHART_EXPORT_FAILED"
            });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new ApiResponse<object> {
            Success = true,
            Data = new { Status = "Healthy", Service = "Charts API" },
            Message = "图表服务运行正常"
        });
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // 配置日志
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        
        // 注册服务
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        // 注册 AOT 优化的图表服务
        builder.Services.AddCharts(options => {
            options.EnableAotOptimization = true;
            options.EnableTrimOptimization = true;
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
        
        Console.WriteLine("图表生成 Web API 集成示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("API 服务已启动!");
        Console.WriteLine("访问 http://localhost:5000/swagger 查看 API 文档");
        Console.WriteLine("\nAPI 端点:");
        Console.WriteLine("  - POST /api/charts/generate - 生成图表");
        Console.WriteLine("  - POST /api/charts/export/{chartId} - 导出图表");
        Console.WriteLine("  - GET /api/charts/health - 健康检查");
        Console.WriteLine("\n按 Ctrl+C 停止服务");
        
        await app.RunAsync();
    }
}
```

### 6. 消息队列集成示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Charts;

// 消息队列客户端接口
public interface IMessageQueueClient
{
    Task SubscribeAsync(string topic, Func<Message, CancellationToken, Task> handler, CancellationToken cancellationToken);
    Task PublishAsync(string topic, Message message, CancellationToken cancellationToken);
}

// 消息类
public class Message
{
    public string MessageType { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Dictionary<string, string> Headers { get; set; } = new();
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 模拟消息队列客户端实现
public class MockMessageQueueClient : IMessageQueueClient
{
    private readonly ILogger<MockMessageQueueClient> _logger;
    private readonly Dictionary<string, List<Func<Message, CancellationToken, Task>>> _subscribers = new();
    
    public MockMessageQueueClient(ILogger<MockMessageQueueClient> logger)
    {
        _logger = logger;
    }
    
    public async Task SubscribeAsync(string topic, Func<Message, CancellationToken, Task> handler, CancellationToken cancellationToken)
    {
        if (!_subscribers.ContainsKey(topic))
        {
            _subscribers[topic] = new List<Func<Message, CancellationToken, Task>>();
        }
        
        _subscribers[topic].Add(handler);
        _logger.LogInformation("已订阅主题: {Topic}", topic);
        
        // 模拟发布一些测试消息
        await Task.Delay(1000, cancellationToken);
        await PublishTestMessagesAsync(topic, cancellationToken);
    }
    
    public async Task PublishAsync(string topic, Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("发布消息到主题: {Topic}, 消息类型: {MessageType}", topic, message.MessageType);
        
        if (_subscribers.TryGetValue(topic, out var handlers))
        {
            foreach (var handler in handlers)
            {
                await handler(message, cancellationToken);
            }
        }
    }
    
    private async Task PublishTestMessagesAsync(string topic, CancellationToken cancellationToken)
    {
        // 发布测试消息
        var testMessages = new List<Message>
        {
            new Message {
                MessageType = "GenerateChart",
                Body = JsonSerializer.Serialize(new GenerateChartRequest {
                    ChartType = ChartType.Line,
                    Title = "测试图表",
                    XAxis = new Axis { Data = new[] { "1月", "2月", "3月" } },
                    Series = new List<Series> {
                        new Series {
                            Name = "测试数据",
                            Data = new[] { 100, 200, 150 }
                        }
                    }
                })
            }
        };
        
        foreach (var message in testMessages)
        {
            await PublishAsync(topic, message, cancellationToken);
            await Task.Delay(2000, cancellationToken);
        }
    }
}

// 图表生成请求类
public class GenerateChartRequest
{
    public ChartType ChartType { get; set; }
    public string Title { get; set; } = string.Empty;
    public Axis XAxis { get; set; } = new();
    public Axis YAxis { get; set; } = new();
    public List<Series> Series { get; set; } = new();
    public bool EnableAotOptimization { get; set; } = false;
}

// 图表导出请求类
public class ExportChartRequest
{
    public string ChartId { get; set; } = string.Empty;
    public ExportFormat Format { get; set; } = ExportFormat.PNG;
}

// 消息队列消费者服务
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
        
        await _messageQueueClient.SubscribeAsync("chart-commands", async (message, cancellationToken) => {
            try
            {
                _logger.LogInformation("收到图表命令: {MessageType}, 消息 ID: {MessageId}", 
                    message.MessageType, message.MessageId);
                
                switch (message.MessageType)
                {
                    case "GenerateChart":
                        await HandleGenerateChartCommandAsync(message, cancellationToken);
                        break;
                    
                    case "ExportChart":
                        await HandleExportChartCommandAsync(message, cancellationToken);
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
        // 解析图表生成请求
        var request = JsonSerializer.Deserialize<GenerateChartRequest>(message.Body, _jsonOptions);
        if (request == null)
        {
            _logger.LogError("无法解析图表生成请求: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行图表生成命令: {ChartType}, 标题: {Title}", 
            request.ChartType, request.Title);
        
        // 创建图表配置
        var chartConfig = new ChartConfig
        {
            ChartType = request.ChartType,
            Title = request.Title,
            XAxis = request.XAxis,
            YAxis = request.YAxis,
            Series = request.Series,
            EnableAotOptimization = request.EnableAotOptimization
        };
        
        // 执行图表生成
        var result = await _chartsService.GenerateChartAsync(chartConfig);
        
        _logger.LogInformation("图表生成命令执行完成: {Title}, 结果: {Success}", 
            request.Title, result.Success);
        
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
        // 解析图表导出请求
        var request = JsonSerializer.Deserialize<ExportChartRequest>(message.Body, _jsonOptions);
        if (request == null)
        {
            _logger.LogError("无法解析图表导出请求: {Body}", message.Body);
            return;
        }
        
        _logger.LogInformation("执行图表导出命令: {ChartId}, 格式: {Format}", 
            request.ChartId, request.Format);
        
        // 执行图表导出
        var result = await _chartsService.ExportChartAsync(request.ChartId, request.Format);
        
        _logger.LogInformation("图表导出命令执行完成: {ChartId}, 结果: {Success}", 
            request.ChartId, result.Success);
        
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

// 消息队列发布者服务
public class ChartMessagePublisher
{
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public ChartMessagePublisher(IMessageQueueClient messageQueueClient)
    {
        _messageQueueClient = messageQueueClient;
    }
    
    public async Task PublishGenerateChartCommandAsync(GenerateChartRequest request, CancellationToken cancellationToken = default)
    {
        var message = new Message {
            MessageType = "GenerateChart",
            Body = JsonSerializer.Serialize(request, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CommandSource", "ChartMessagePublisher" },
                { "Timestamp", DateTime.UtcNow.ToString("o") }
            }
        };
        
        await _messageQueueClient.PublishAsync("chart-commands", message, cancellationToken);
    }
    
    public async Task PublishExportChartCommandAsync(ExportChartRequest request, CancellationToken cancellationToken = default)
    {
        var message = new Message {
            MessageType = "ExportChart",
            Body = JsonSerializer.Serialize(request, _jsonOptions),
            Headers = new Dictionary<string, string> {
                { "CommandSource", "ChartMessagePublisher" },
                { "Timestamp", DateTime.UtcNow.ToString("o") }
            }
        };
        
        await _messageQueueClient.PublishAsync("chart-commands", message, cancellationToken);
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("图表生成消息队列集成示例");
        Console.WriteLine("=" * 50);
        
        // 配置服务
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册消息队列客户端
        services.AddSingleton<IMessageQueueClient, MockMessageQueueClient>();
        
        // 注册 AOT 优化的图表服务
        services.AddCharts(options => {
            options.EnableAotOptimization = true;
        });
        
        // 注册消息队列消费者
        services.AddHostedService<ChartMessageConsumer>();
        
        // 注册消息队列发布者
        services.AddSingleton<ChartMessagePublisher>();
        
        // 构建服务提供器
        var serviceProvider = services.BuildServiceProvider();
        
        Console.WriteLine("服务已启动!");
        Console.WriteLine("-" * 30);
        Console.WriteLine("消息队列消费者正在运行...");
        Console.WriteLine("等待接收图表命令...");
        Console.WriteLine("\n按 Ctrl+C 停止服务");
        
        // 启动消息队列发布者，发布一些测试命令
        var publisher = serviceProvider.GetRequiredService<ChartMessagePublisher>();
        await Task.Delay(3000); // 等待消费者启动完成
        
        Console.WriteLine("\n发布额外的测试命令...");
        
        // 发布图表生成命令
        await publisher.PublishGenerateChartCommandAsync(new GenerateChartRequest {
            ChartType = ChartType.Bar,
            Title = "消息队列触发的柱状图",
            XAxis = new Axis { Data = new[] { "产品A", "产品B", "产品C" } },
            Series = new List<Series> {
                new Series {
                    Name = "销量",
                    Data = new[] { 350, 280, 420 }
                }
            }
        });
        
        await Task.Delay(5000);
        
        Console.WriteLine("\n发布图表导出命令...");
        
        // 发布图表导出命令
        await publisher.PublishExportChartCommandAsync(new ExportChartRequest {
            ChartId = "test-chart-id",
            Format = ExportFormat.PNG
        });
        
        // 保持应用运行
        await Task.Delay(Timeout.Infinite);
    }
}
```

## 总结

以上示例演示了 charts 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用基本操作
2. 配置 AOT 编译优化，提高运行时性能
3. 进行高级配置，满足复杂需求
4. 优化性能，支持高并发场景
5. 将图表服务集成到 Web API 中
6. 与消息队列集成，实现异步图表生成

所有示例都遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

通过这些示例，您可以学习到如何使用 charts 技能构建高性能、可靠的图表生成系统，支持多种图表类型和图表库，适用于各种数据可视化场景。
