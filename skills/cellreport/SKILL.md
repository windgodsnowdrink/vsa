# cellreport Agent Skill - 单元格报表技能

## 技能概述

基于 .NET 10 的高性能单元格报表技能，为 .NET 开发者提供强大的单元格报表生成和处理功能，支持 AOT（提前编译）编译，适用于构建高性能、可扩展的报表系统。

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

在您的主应用程序中注册 cellreport 服务：

```csharp
// 配置应用程序
var builder = WebApplication.CreateBuilder(args);

// 注册 cellreport 服务
builder.Services.AddCellReport();

// AOT 优化配置
builder.Services.Configure<CellReportSettings>(options => {
    options.EnableAotOptimization = true;
    options.EnableTrimOptimization = true;
});

var app = builder.Build();
```

### 使用示例

```csharp
// 获取 cellreport 服务
var cellReportService = serviceProvider.GetRequiredService<ICellReportService>();

// 创建报表配置
var reportConfig = new ReportConfig
{
    ReportType = ReportType.Excel,
    TemplatePath = "report-template.xlsx",
    OutputPath = "output-report.xlsx",
    EnableAotOptimization = true
};

// 准备报表数据
var reportData = new ReportData
{
    DataSource = GetReportDataSource(),
    Parameters = new Dictionary<string, object> {
        { "ReportTitle", "销售报表" },
        { "ReportDate", DateTime.Now }
    }
};

// 生成报表
var result = await cellReportService.GenerateReportAsync(reportConfig, reportData);
Console.WriteLine($"报表生成成功: {result}");
```

## 导航地图

```
cellreport/
├── index.yaml                           # 元数据索引描述
├── SKILL.md                            # 技能入口点（当前文件）
├── reference/                          # 参考文件
│   ├── README.md                      # 完整功能描述
│   └── examples.md                    # 使用示例
├── scripts/                            # 脚本和工具
    ├── cellreport_integration.cs       # cellreport 集成示例
    ├── cellreport_integration.run.json      # 运行配置
    └── cellreport_integration.setting.json  # 设置文件
```

## 主要功能

1. **现代化报表框架**: 基于 .NET 10 构建的轻量级、高性能单元格报表框架
2. **AOT 编译支持**: 支持将应用编译为本机代码，提高运行时性能和启动速度
3. **多种报表格式**: 支持 Excel、PDF、HTML 等多种报表格式生成
4. **高性能设计**: 优化的单元格计算和数据处理算法
5. **复杂报表支持**: 支持嵌套表格、图表、条件格式等复杂报表设计
6. **数据源绑定**: 支持多种数据源类型，包括数据库、对象集合、JSON 等
7. **模板驱动**: 支持基于模板的报表生成，提高开发效率
8. **异步编程模型**: 基于异步/等待模式，提高并发处理能力
9. **依赖注入**: 原生支持 .NET 依赖注入容器
10. **模块化设计**: 便于扩展和定制功能

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

### AOT 编译命令

```bash
# 编译为 Windows x64 原生可执行文件
dotnet publish -c Release -r win-x64 --self-contained

# 编译为 Linux x64 原生可执行文件
dotnet publish -c Release -r linux-x64 --self-contained

# 编译为 macOS x64 原生可执行文件
dotnet publish -c Release -r osx-x64 --self-contained
```

### AOT 兼容性注意事项

1. **使用 AOT 兼容的库**: 确保使用的 cellreport 版本和依赖库支持 AOT 编译
2. **避免反射**: 避免在报表处理中使用反射
3. **资源处理**: 确保所有模板资源在 AOT 编译时能被正确处理
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
public class ReportController : ControllerBase
{
    private readonly ICellReportService _cellReportService;

    public ReportController(ICellReportService cellReportService)
    {
        _cellReportService = cellReportService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport([FromBody] ReportRequest request)
    {
        // 生成报表
        var reportPath = await _cellReportService.GenerateReportAsync(
            new ReportConfig { ReportType = request.ReportType },
            new ReportData { DataSource = request.DataSource });

        // 返回报表文件
        return PhysicalFile(reportPath, "application/octet-stream", Path.GetFileName(reportPath));
    }
}
```

### 与 EF Core 集成

```csharp
// 配置 EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 注册报表服务
builder.Services.AddScoped<ICellReportService, CellReportService>();
builder.Services.AddScoped<IReportDataSource, EfCoreReportDataSource>();

// 使用 EF Core 数据源生成报表
var dataSource = new EfCoreReportDataSource(dbContext);
var reportData = new ReportData { DataSource = dataSource };
var result = await cellReportService.GenerateReportAsync(reportConfig, reportData);
```

## 性能优化建议

1. **启用 AOT 编译**: 对于性能敏感场景，启用 AOT 编译可以显著提高性能
2. **使用异步 API**: 优先使用异步 API，避免阻塞主线程
3. **优化数据源**: 减少不必要的数据查询和处理
4. **使用缓存**: 对频繁使用的报表模板和数据进行缓存
5. **优化报表设计**: 避免过于复杂的报表结构
6. **使用内存池**: 对于频繁分配的内存，使用 ArrayPool<T> 减少 GC 压力
7. **并行处理**: 对于大型报表，考虑使用并行处理提高生成速度
8. **监控性能**: 使用 OpenTelemetry 等工具监控报表生成性能

## 故障排除

### 常见问题

1. **报表生成失败**
   - 检查模板文件路径是否正确
   - 验证数据源格式是否符合要求
   - 查看详细日志

2. **AOT 编译失败**
   - 检查是否使用了不兼容的库
   - 查看详细的编译日志
   - 确保所有依赖都支持 AOT
   - 检查是否使用了反射等不兼容特性
   - 考虑调整 TrimMode

3. **性能问题**
   - 使用性能分析工具定位瓶颈
   - 优化数据源查询
   - 启用缓存
   - 考虑使用 AOT 编译

4. **报表格式问题**
   - 检查模板设计是否正确
   - 验证数据类型匹配
   - 查看报表生成配置

## 最佳实践

1. **使用模板驱动设计**: 采用模板驱动的报表生成方式，提高开发效率和可维护性
2. **分离业务逻辑和报表逻辑**: 将业务逻辑与报表生成逻辑分离，便于测试和维护
3. **实现适当的错误处理**: 提供清晰的错误信息和日志记录
4. **使用依赖注入**: 避免硬编码依赖，提高代码的可测试性和可扩展性
5. **编写单元测试**: 测试报表生成逻辑和业务规则
6. **考虑并发处理**: 对于高并发场景，设计线程安全的报表生成逻辑
7. **监控和日志**: 实现全面的监控和日志记录，便于问题定位和性能优化
8. **考虑安全性**: 确保报表生成过程中的数据安全和访问控制

## 总结

cellreport 技能提供了一套完整的现代化单元格报表解决方案，基于 .NET 10 构建，支持 AOT 编译，具有高性能、模块化、可扩展等特点。通过 cellreport，开发者可以快速构建高性能、可维护的报表系统，适用于各种规模的应用场景。

该技能遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，支持与多种系统集成，如 Web API、EF Core 等。同时，提供了详细的性能优化建议和故障排除指南，帮助开发者构建高性能、可靠的报表服务。