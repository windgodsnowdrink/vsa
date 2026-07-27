# charts Agent Skill - 图表生成技能

## 技能概述

基于 .NET 10 的高性能图表生成技能，为 .NET 开发者提供强大的图表生成功能，支持 AOT（提前编译）编译，适用于各种数据可视化场景。

## 快速入门指南

### 安装依赖

在您的主应用程序运行文件中添加以下依赖项：

```yaml
#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Aot@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
```

### 注册服务

在您的主应用程序中注册图表服务：

```csharp
// 配置应用程序
var builder = WebApplication.CreateBuilder(args);

// 注册图表服务
builder.Services.AddCharts();

// AOT 优化配置
builder.Services.Configure<ChartsSettings>(options => {
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
    options.DefaultChartLibrary = ChartLibrary.ECharts;
});

var app = builder.Build();
```

### 使用示例

```csharp
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
            Data = new[] { 120, 200, 150, 80, 70, 110 },
            EnableAotOptimization = true
        }
    },
    EnableAotOptimization = true
};

// 生成图表
var chartResult = await chartsService.GenerateChartAsync(chartConfig);
Console.WriteLine($"图表生成结果: {chartResult.Success}");
Console.WriteLine($"图表数据: {chartResult.ChartData}");

// 导出图表
var exportResult = await chartsService.ExportChartAsync(chartResult.ChartId, ExportFormat.PNG);
Console.WriteLine($"图表导出结果: {exportResult.Success}");
```

## 导航地图

```
charts/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── echarts_integration.cs         # ECharts 集成示例
    ├── echarts_integration.run.json   # 运行配置
    ├── echarts_integration.setting.json # 设置文件
    ├── highcharts_integration.cs      # Highcharts 集成示例
    ├── highcharts_integration.run.json # 运行配置
    └── highcharts_integration.setting.json # 设置文件
```

## 主要功能

1. **多种图表类型支持**: 支持折线图、柱状图、饼图、散点图、地图、雷达图、热力图等多种图表类型
2. **高性能图表生成**: 基于 .NET 10 构建，支持高并发场景
3. **自定义图表样式**: 支持自定义主题、颜色、字体、布局等样式设置
4. **多种数据源支持**: 支持从数据库、文件、API 等多种数据源获取数据
5. **图表导出功能**: 支持导出为 PNG、JPG、PDF、SVG 等多种格式
6. **响应式设计**: 适配不同屏幕尺寸，支持移动端和桌面端
7. **多种图表库集成**: 支持 ECharts、Highcharts 等主流前端图表库
8. **AOT 编译支持**: 支持将应用编译为本机代码，提高运行时性能和启动速度
9. **模块化设计**: 便于扩展和维护，支持自定义图表类型和功能
10. **异步编程模型**: 基于异步/等待模式，提高并发处理能力

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

## 与其他系统集成

### 与 Web API 集成

```csharp
// 定义 API 控制器
[ApiController]
[Route("api/[controller]")]
public class ChartsController : ControllerBase
{
    private readonly IChartsService _chartsService;

    public ChartsController(IChartsService chartsService)
    {
        _chartsService = chartsService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateChart([FromBody] ChartConfig request)
    {
        var result = await _chartsService.GenerateChartAsync(request);
        return Ok(new { Result = result });
    }

    [HttpPost("export/{chartId}")]
    public async Task<IActionResult> ExportChart(string chartId, [FromQuery] ExportFormat format)
    {
        var result = await _chartsService.ExportChartAsync(chartId, format);
        return Ok(new { Result = result });
    }
}
```

### 与 gRPC 集成

```csharp
// gRPC 服务实现
public class ChartsServiceImpl : ChartsService.ChartsServiceBase
{
    private readonly IChartsService _chartsService;

    public ChartsServiceImpl(IChartsService chartsService)
    {
        _chartsService = chartsService;
    }

    public override async Task<GenerateChartResponse> GenerateChart(GenerateChartRequest request, ServerCallContext context)
    {
        var chartConfig = MapToChartConfig(request);
        var result = await _chartsService.GenerateChartAsync(chartConfig);
        return new GenerateChartResponse { Result = result.ToString() };
    }
}
```

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化图表数据**: 减少不必要的数据点，使用合适的数据结构
4. **使用缓存**: 对频繁生成的图表进行缓存
5. **选择合适的图表库**: 根据需求选择合适的图表库
6. **优化渲染逻辑**: 简化复杂图表的渲染逻辑
7. **监控性能**: 使用 OpenTelemetry 等工具监控图表生成性能

## 故障排除

### 常见问题

1. **图表生成失败**
   - 检查图表配置是否正确
   - 验证服务注册是否完整
   - 查看详细日志

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode

3. **图表渲染性能问题**
   - 优化图表数据量
   - 启用 AOT 编译
   - 使用缓存
   - 选择更轻量级的图表类型

4. **图表导出失败**
   - 检查导出格式是否支持
   - 验证图表 ID 是否有效
   - 查看详细日志

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

## 总结

charts 技能提供了一套完整的现代化图表生成解决方案，基于 .NET 10 构建，具有高性能、模块化、可扩展等特点。通过 charts，开发者可以快速构建各种类型的图表，支持多种图表库集成，适用于各种数据可视化场景。

该技能遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持与多种系统集成，如 Web API、gRPC 等。同时，提供了详细的性能优化建议和故障排除指南，帮助开发者构建高质量的图表生成系统。

charts 支持 AOT 编译，可以编译为本机代码，提高运行时性能和启动速度。通过遵循本文档中的最佳实践和 AOT 兼容性建议，开发者可以构建出高性能、可靠的图表生成系统，满足各种数据可视化需求。
