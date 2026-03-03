# cellreport - 使用示例

## 快速开始

### 1. 基础使用示例

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
using CellReport;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("cellreport 基础使用示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务提供器
        var serviceProvider = BuildServiceProvider();
        var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();
        
        // 创建报表配置
        var reportConfig = new ReportConfig
        {
            ReportType = ReportType.Excel,
            OutputPath = "basic-output-report.xlsx"
        };
        
        // 准备示例数据
        var sampleData = new List<Product>
        {
            new Product { Id = 1, Name = "产品1", Price = 10.99m, Category = "电子产品", Stock = 100 },
            new Product { Id = 2, Name = "产品2", Price = 19.99m, Category = "家居用品", Stock = 200 },
            new Product { Id = 3, Name = "产品3", Price = 29.99m, Category = "办公用品", Stock = 300 },
            new Product { Id = 4, Name = "产品4", Price = 39.99m, Category = "电子产品", Stock = 150 },
            new Product { Id = 5, Name = "产品5", Price = 49.99m, Category = "家居用品", Stock = 250 }
        };
        
        // 生成报表
        Console.WriteLine("开始生成报表...");
        var result = await cellReportService.GenerateReportAsync(
            reportConfig,
            new ReportData { DataSource = new ListReportDataSource(sampleData) });
        
        Console.WriteLine($"报表生成成功: {result}");
        Console.WriteLine($"报表路径: {result}");
        Console.WriteLine("\n示例完成!");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 cellreport 服务
        services.AddCellReport();
        
        return services.BuildServiceProvider();
    }
}

// 产品模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// 列表数据源实现
public class ListReportDataSource : IReportDataSource
{
    private readonly IEnumerable<object> _data;
    
    public ListReportDataSource(IEnumerable<object> data)
    {
        _data = data;
    }
    
    public Task<object> GetDataAsync() => Task.FromResult<object>(_data.ToList());
    
    public Task<object> GetDataAsync(string query, params object[] parameters) => GetDataAsync();
    
    public Task<int> GetDataCountAsync() => Task.FromResult(_data.Count());
}
```

### 2. AOT 编译优化示例

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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CellReport;

// AOT 安全的报表服务实现
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeCellReportService : ICellReportService
{
    private readonly ILogger<AotSafeCellReportService> _logger;
    
    public AotSafeCellReportService(ILogger<AotSafeCellReportService> logger)
    {
        _logger = logger;
    }
    
    public async Task<string> GenerateReportAsync(ReportConfig config, ReportData data)
    {
        _logger.LogInformation("开始生成 AOT 优化的报表...");
        
        // AOT 安全的报表生成逻辑
        // 注意：避免使用反射、动态代码生成等 AOT 不兼容的技术
        var outputPath = config.OutputPath ?? $"aot-report-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        
        // 模拟报表生成过程
        await Task.Delay(500);
        
        _logger.LogInformation("AOT 优化的报表生成完成: {OutputPath}", outputPath);
        return outputPath;
    }
}

// AOT 安全的报表配置
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class ReportConfig
{
    public ReportType ReportType { get; set; }
    public string? TemplatePath { get; set; }
    public string? OutputPath { get; set; }
    public bool EnableAotOptimization { get; set; } = true;
}

// AOT 安全的报表数据
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class ReportData
{
    public IReportDataSource DataSource { get; set; }
}

// AOT 安全的数据源接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface IReportDataSource
{
    Task<object> GetDataAsync();
    Task<object> GetDataAsync(string query, params object[] parameters);
    Task<int> GetDataCountAsync();
}

// AOT 安全的列表数据源
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class AotSafeListDataSource : IReportDataSource
{
    private readonly List<object> _data;
    
    public AotSafeListDataSource(List<object> data)
    {
        _data = data;
    }
    
    public Task<object> GetDataAsync() => Task.FromResult<object>(_data);
    
    public Task<object> GetDataAsync(string query, params object[] parameters) => GetDataAsync();
    
    public Task<int> GetDataCountAsync() => Task.FromResult(_data.Count);
}

// 报表类型枚举
public enum ReportType
{
    Excel,
    Pdf,
    Html,
    Csv
}

// 主程序
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("cellreport AOT 编译优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务提供器
        var serviceProvider = BuildServiceProvider();
        var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();
        
        // 创建 AOT 优化的报表配置
        var reportConfig = new ReportConfig
        {
            ReportType = ReportType.Excel,
            OutputPath = "aot-optimized-report.xlsx",
            EnableAotOptimization = true
        };
        
        // 准备示例数据
        var sampleData = new List<object>
        {
            new { Id = 1, Name = "AOT 产品1", Price = 10.99m, Category = "电子产品" },
            new { Id = 2, Name = "AOT 产品2", Price = 19.99m, Category = "家居用品" },
            new { Id = 3, Name = "AOT 产品3", Price = 29.99m, Category = "办公用品" }
        };
        
        // 生成 AOT 优化的报表
        Console.WriteLine("开始生成 AOT 优化的报表...");
        var stopwatch = Stopwatch.StartNew();
        
        var result = await cellReportService.GenerateReportAsync(
            reportConfig,
            new ReportData { DataSource = new AotSafeListDataSource(sampleData) });
        
        stopwatch.Stop();
        
        Console.WriteLine($"AOT 优化报表生成成功: {result}");
        Console.WriteLine($"生成时间: {stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine($"AOT 优化: 已启用");
        Console.WriteLine("\n示例完成!");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册 AOT 安全的报表服务
        services.AddSingleton<ICellReportService, AotSafeCellReportService>();
        
        return services.BuildServiceProvider();
    }
}

// 报表服务接口
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public interface ICellReportService
{
    Task<string> GenerateReportAsync(ReportConfig config, ReportData data);
}
```

### 3. 高级配置示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using CellReport;

// 报表设置类
public class CellReportSettings
{
    public bool EnableAotOptimization { get; set; } = false;
    public bool EnableTrimOptimization { get; set; } = false;
    public bool EnableCache { get; set; } = true;
    public int CacheSize { get; set; } = 1000;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableDetailedLogging { get; set; } = false;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

// 高级报表服务实现
public class AdvancedCellReportService : ICellReportService
{
    private readonly ILogger<AdvancedCellReportService> _logger;
    private readonly CellReportSettings _settings;
    
    public AdvancedCellReportService(ILogger<AdvancedCellReportService> logger, IOptions<CellReportSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }
    
    public async Task<string> GenerateReportAsync(ReportConfig config, ReportData data)
    {
        _logger.LogInformation("开始生成高级配置报表...");
        _logger.LogDebug("报表配置: {ReportType}, AOT优化: {EnableAotOptimization}", 
            config.ReportType, _settings.EnableAotOptimization);
        
        // 使用配置设置
        if (_settings.EnableCache)
        {
            _logger.LogInformation("启用缓存: 大小={CacheSize}, 过期时间={CacheExpiration}", 
                _settings.CacheSize, _settings.CacheExpiration);
        }
        
        // 模拟报表生成过程
        await Task.Delay(1000);
        
        var outputPath = config.OutputPath ?? $"advanced-report-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        _logger.LogInformation("高级配置报表生成完成: {OutputPath}", outputPath);
        return outputPath;
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("cellreport 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建配置
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> {
                { "CellReport:EnableAotOptimization", "true" },
                { "CellReport:EnableTrimOptimization", "true" },
                { "CellReport:EnableCache", "true" },
                { "CellReport:CacheSize", "2000" },
                { "CellReport:CacheExpiration", "02:00:00" },
                { "CellReport:EnableDetailedLogging", "true" },
                { "CellReport:LogLevel", "Debug" }
            })
            .Build();
        
        // 构建服务提供器
        var serviceProvider = BuildServiceProvider(configuration);
        
        // 获取配置信息
        var settings = serviceProvider.GetRequiredService<IOptions<CellReportSettings>>().Value;
        Console.WriteLine($"配置信息:");
        Console.WriteLine($"  AOT 优化: {settings.EnableAotOptimization}");
        Console.WriteLine($"  修剪优化: {settings.EnableTrimOptimization}");
        Console.WriteLine($"  启用缓存: {settings.EnableCache}");
        Console.WriteLine($"  缓存大小: {settings.CacheSize}");
        Console.WriteLine($"  缓存过期: {settings.CacheExpiration}");
        Console.WriteLine($"  详细日志: {settings.EnableDetailedLogging}");
        Console.WriteLine($"  日志级别: {settings.LogLevel}");
        Console.WriteLine();
        
        // 使用报表服务
        var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();
        var result = await cellReportService.GenerateReportAsync(
            new ReportConfig { ReportType = ReportType.Excel },
            new ReportData { DataSource = new ListReportDataSource(new List<object>()) });
        
        Console.WriteLine($"报表生成成功: {result}");
        Console.WriteLine("\n示例完成!");
    }
    
    private static ServiceProvider BuildServiceProvider(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        
        // 添加配置
        services.AddSingleton(configuration);
        
        // 配置 cellreport 设置
        services.Configure<CellReportSettings>(configuration.GetSection("CellReport"));
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Debug);
        });
        
        // 注册报表服务
        services.AddSingleton<ICellReportService, AdvancedCellReportService>();
        
        return services.BuildServiceProvider();
    }
}
```

### 4. 性能优化示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CellReport;

// 高性能报表服务实现
public class HighPerformanceCellReportService : ICellReportService
{
    private readonly ILogger<HighPerformanceCellReportService> _logger;
    private readonly ConcurrentDictionary<string, string> _reportCache;
    private readonly SemaphoreSlim _cacheSemaphore;
    
    public HighPerformanceCellReportService(ILogger<HighPerformanceCellReportService> logger)
    {
        _logger = logger;
        _reportCache = new ConcurrentDictionary<string, string>();
        _cacheSemaphore = new SemaphoreSlim(1, 1);
    }
    
    public async Task<string> GenerateReportAsync(ReportConfig config, ReportData data)
    {
        // 生成缓存键
        var cacheKey = GenerateCacheKey(config, data);
        
        // 检查缓存
        if (_reportCache.TryGetValue(cacheKey, out var cachedReport))
        {
            _logger.LogInformation("从缓存获取报表: {CacheKey}", cacheKey);
            return cachedReport;
        }
        
        // 限制并发生成
        await _cacheSemaphore.WaitAsync();
        try
        {
            // 双重检查锁定
            if (_reportCache.TryGetValue(cacheKey, out cachedReport))
            {
                return cachedReport;
            }
            
            // 高性能报表生成
            _logger.LogInformation("生成高性能报表...");
            
            // 模拟高性能报表生成过程
            await Task.Delay(200);
            
            var outputPath = $"high-performance-report-{Guid.NewGuid()}.xlsx";
            
            // 添加到缓存
            _reportCache[cacheKey] = outputPath;
            _logger.LogInformation("高性能报表生成完成并缓存: {OutputPath}", outputPath);
            
            return outputPath;
        }
        finally
        {
            _cacheSemaphore.Release();
        }
    }
    
    private string GenerateCacheKey(ReportConfig config, ReportData data)
    {
        // 生成唯一缓存键
        return $"{config.ReportType}-{config.TemplatePath}-{data.GetDataCountAsync().Result}";
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("cellreport 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务提供器
        var serviceProvider = BuildServiceProvider();
        var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();
        
        // 准备测试数据
        var testData = new List<object>
        {
            new { Id = 1, Name = "性能测试产品1", Price = 10.99m },
            new { Id = 2, Name = "性能测试产品2", Price = 19.99m },
            new { Id = 3, Name = "性能测试产品3", Price = 29.99m }
        };
        
        var reportConfig = new ReportConfig { ReportType = ReportType.Excel };
        var reportData = new ReportData { DataSource = new ListReportDataSource(testData) };
        
        // 性能测试：多次调用
        const int iterations = 10;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"开始性能测试: 执行 {iterations} 次报表生成");
        
        for (int i = 0; i < iterations; i++)
        {
            var result = await cellReportService.GenerateReportAsync(reportConfig, reportData);
            Console.WriteLine($"  执行 #{i+1}: {result}");
        }
        
        stopwatch.Stop();
        
        Console.WriteLine($"\n性能测试完成!");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
        Console.WriteLine($"平均执行时间: {stopwatch.Elapsed.TotalMilliseconds / iterations:F2} ms");
        Console.WriteLine($"每秒处理请求: {iterations / stopwatch.Elapsed.TotalSeconds:F2} RPS");
        Console.WriteLine("\n示例完成!");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册高性能报表服务
        services.AddSingleton<ICellReportService, HighPerformanceCellReportService>();
        
        return services.BuildServiceProvider();
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
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=Full
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CellReport;

// API 请求模型
public class ReportRequest
{
    public ReportType ReportType { get; set; }
    public string? TemplatePath { get; set; }
    public List<object> DataSource { get; set; } = new();
    public Dictionary<string, object> Parameters { get; set; } = new();
}

// API 响应模型
public class ReportResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ReportPath { get; set; } = string.Empty;
    public ReportType ReportType { get; set; }
    public DateTime GeneratedAt { get; set; }
}

// API 错误响应模型
public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ErrorCode { get; set; } = string.Empty;
    public string? Details { get; set; }
}

// API 控制器
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
            // 验证请求
            if (request.DataSource == null || !request.DataSource.Any())
            {
                return BadRequest(new ErrorResponse {
                    Success = false,
                    Message = "数据源不能为空",
                    ErrorCode = "INVALID_DATA_SOURCE"
                });
            }
            
            // 生成报表
            var reportPath = await _cellReportService.GenerateReportAsync(
                new ReportConfig {
                    ReportType = request.ReportType,
                    TemplatePath = request.TemplatePath,
                    EnableAotOptimization = true
                },
                new ReportData {
                    DataSource = new ListReportDataSource(request.DataSource),
                    Parameters = request.Parameters
                });

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
        
        // 注册报表服务
        builder.Services.AddSingleton<ICellReportService, AotSafeCellReportService>();
        
        // AOT 优化配置
        builder.Services.Configure<CellReportSettings>(options => {
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
        
        Console.WriteLine("cellreport Web API 集成示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("API 服务已启动!");
        Console.WriteLine("访问 http://localhost:5000/swagger 查看 API 文档");
        Console.WriteLine("\n按 Ctrl+C 停止服务");
        
        await app.RunAsync();
    }
}
```

### 6. 与 EF Core 集成示例

```csharp
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.EntityFrameworkCore@8.0.0
#:package Microsoft.EntityFrameworkCore.InMemory@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CellReport;

// 数据库上下文
public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 配置模型关系
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
        
        // 种子数据
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "电子产品" },
            new Category { Id = 2, Name = "家居用品" },
            new Category { Id = 3, Name = "办公用品" }
        );
        
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "笔记本电脑", Price = 5999.99m, CategoryId = 1, Stock = 50 },
            new Product { Id = 2, Name = "智能手机", Price = 3999.99m, CategoryId = 1, Stock = 100 },
            new Product { Id = 3, Name = "办公桌", Price = 1299.99m, CategoryId = 2, Stock = 30 },
            new Product { Id = 4, Name = "办公椅", Price = 899.99m, CategoryId = 2, Stock = 40 },
            new Product { Id = 5, Name = "打印机", Price = 1999.99m, CategoryId = 3, Stock = 20 }
        );
    }
}

// 实体模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // 导航属性
    public Category Category { get; set; } = null!;
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // 导航属性
    public List<Product> Products { get; set; } = new();
}

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
        return await _dbContext.Products.Include(p => p.Category).ToListAsync();
    }
    
    public async Task<object> GetDataAsync(string query, params object[] parameters)
    {
        // 示例: 使用 LINQ 查询
        if (query == "GetProductsByCategory")
        {
            var categoryId = (int)parameters[0];
            return await _dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
        
        return await GetDataAsync();
    }
    
    public async Task<int> GetDataCountAsync()
    {
        return await _dbContext.Products.CountAsync();
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("cellreport 与 EF Core 集成示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务提供器
        var serviceProvider = BuildServiceProvider();
        
        // 初始化数据库
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
            Console.WriteLine("数据库初始化完成");
        }
        
        // 获取服务
        var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();
        
        // 使用 EF Core 数据源生成报表
        using (var scope = serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dataSource = new EfCoreReportDataSource(dbContext);
            
            Console.WriteLine("\n从数据库获取产品数据...");
            var productCount = await dataSource.GetDataCountAsync();
            Console.WriteLine($"找到 {productCount} 个产品");
            
            // 生成报表
            Console.WriteLine("\n使用 EF Core 数据源生成报表...");
            var result = await cellReportService.GenerateReportAsync(
                new ReportConfig { ReportType = ReportType.Excel, OutputPath = "efcore-report.xlsx" },
                new ReportData { DataSource = dataSource });
            
            Console.WriteLine($"报表生成成功: {result}");
            
            // 使用查询获取特定数据
            Console.WriteLine("\n使用查询获取电子产品...");
            var electronicsDataSource = new EfCoreReportDataSource(dbContext);
            var electronicsData = await electronicsDataSource.GetDataAsync("GetProductsByCategory", 1);
            var electronicsCount = ((List<Product>)electronicsData).Count;
            Console.WriteLine($"找到 {electronicsCount} 个电子产品");
            
            // 生成电子产品报表
            var electronicsResult = await cellReportService.GenerateReportAsync(
                new ReportConfig { ReportType = ReportType.Excel, OutputPath = "electronics-report.xlsx" },
                new ReportData { DataSource = electronicsDataSource });
            
            Console.WriteLine($"电子产品报表生成成功: {electronicsResult}");
        }
        
        Console.WriteLine("\n示例完成!");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        // 添加日志服务
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });
        
        // 配置 EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("CellReportDemoDb"));
        
        // 注册报表服务
        services.AddSingleton<ICellReportService, AdvancedCellReportService>();
        
        return services.BuildServiceProvider();
    }
}
```

## 总结

以上示例演示了 cellreport 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速开始使用 cellreport 生成基础报表
2. 配置 AOT 编译优化，提高运行时性能
3. 进行高级配置，自定义报表生成行为
4. 实现性能优化，包括缓存、并发处理等
5. 将 cellreport 集成到 Web API 中
6. 与 EF Core 数据库集成，使用数据库数据源生成报表

这些示例遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适合各种规模的项目。所有示例都支持 AOT 编译，可以编译为本机代码以获得更高的性能和更快的启动速度。

通过这些示例，您可以学习到如何使用 cellreport 技能构建高性能、可靠的报表系统，满足各种复杂的报表需求。