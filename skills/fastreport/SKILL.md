# FastReport Agent Skill - FastReport 技能

## 技能概述

基于 .NET 10 构建的高性能 FastReport 技能，为 .NET 开发者提供强大的 FastReport 功能。该技能采用 AOT（提前编译）架构，具有启动速度快、内存占用低、执行效率高等特点，适用于各种报表生成和处理场景。

## 快速开始指南

### 安装依赖

在您的主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
```

### 注册服务

在您的主应用程序中注册 FastReport 服务：

```csharp
// 注册 FastReport 服务
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<FastReportOptions>(builder.Configuration.GetSection("FastReport"));
builder.Services.AddSingleton<IFastReportService, FastReportService>();
```

### 使用示例

```csharp
// 获取 FastReport 服务
var serviceProvider = builder.Build().Services;
var fastReportService = serviceProvider.GetRequiredService<IFastReportService>();

// 使用 FastReport 功能 - 导出报表
var result = await fastReportService.ExportReportAsync(
    "templates/SalesReport.frx",
    "output/SalesReport.pdf",
    "PDF"
);
Console.WriteLine($"结果: {result.Success}");
if (result.Success)
{
    Console.WriteLine($"输出路径: {result.OutputPath}");
}
```

### AOT 单文件执行

FastReport 技能提供了 .NET 10 AOT 编译的单文件执行脚本，可以直接运行：

```bash
# 运行版本命令
./fastreport_aot version

# 创建报表
./fastreport_aot create SalesReport

# 加载报表
./fastreport_aot load templates/SalesReport.frx

# 运行报表
./fastreport_aot run templates/SalesReport.frx

# 导出报表
./fastreport_aot export templates/SalesReport.frx output/SalesReport.pdf PDF

# 预览报表
./fastreport_aot preview templates/SalesReport.frx
```

## 导航地图

```
fastreport/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── fastreport_aot.cs        # FastReport AOT 核心实现
    ├── fastreport_aot.run.json  # 运行配置
    ├── fastreport_aot.setting.json  # 应用程序设置
    ├── fastreport_integration.cs     # FastReport 集成
    ├── fastreport_integration.run.json
    └── fastreport_integration.setting.json
```

## 主要特性

1. **报表创建**：支持创建新的 FastReport 报表
2. **报表加载**：支持加载现有的 FastReport 报表模板
3. **报表运行**：支持运行报表并处理数据
4. **报表导出**：支持将报表导出为多种格式（PDF、Excel、Word 等）
5. **报表预览**：支持预览报表内容
6. **高性能 AOT 架构**：采用 .NET 10 AOT 编译，启动速度快，内存占用低
7. **多环境配置支持**：支持开发、测试、生产等多环境配置
8. **详细的日志记录**：支持详细的日志记录和性能监控
9. **简单易用的 API**：提供简洁直观的 API 设计
10. **命令行工具**：提供功能完整的命令行工具

## 命令行使用

FastReport AOT 引擎提供了丰富的命令行功能：

```bash
# 显示帮助信息
fastreport_aot help

# 显示版本信息
fastreport_aot version

# 创建报表
fastreport_aot create <reportName>

# 加载报表
fastreport_aot load <reportPath>

# 运行报表
fastreport_aot run <reportPath>

# 导出报表
fastreport_aot export <reportPath> <outputPath> <format>

# 预览报表
fastreport_aot preview <reportPath>
```

## 扩展说明

FastReport 技能提供了完整的报表处理解决方案，您可以根据需要进行扩展：

1. **自定义报表导出器**：实现自定义的报表导出逻辑
2. **自定义数据源**：实现自定义的数据源处理
3. **扩展导出格式**：添加对新的导出格式的支持
4. **集成其他系统**：与其他系统进行集成
5. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：妥善处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **报表模板管理**：合理组织和管理报表模板
7. **使用 AOT 编译**：对于生产环境，推荐使用 AOT 编译以获得最佳性能
8. **配置管理**：使用配置文件管理不同环境的设置
9. **资源管理**：确保及时释放报表资源
10. **安全考虑**：验证输入参数，避免安全风险

## 性能特性

- **AOT 编译**：提前编译为本地代码，减少启动时间和内存占用
- **内存优化**：采用高效的内存管理策略，适合处理复杂报表
- **异步设计**：全异步 API 设计，提高并发处理能力
- **性能监控**：内置性能监控功能，便于分析和优化
- **缓存机制**：支持报表缓存，提高重复执行的性能
- **并行处理**：支持并行处理，提高处理速度

## 应用场景

1. **企业报表**：生成各种企业级报表，如销售报表、财务报表等
2. **数据可视化**：将数据转换为可视化的报表
3. **批量报表**：批量生成和导出报表
4. **报表自动化**：自动化生成和分发报表
5. **Web 报表**：在 Web 应用中集成报表功能
6. **移动报表**：为移动设备生成适合的报表
7. **报表集成**：与其他系统集成报表功能
8. **报表设计**：使用代码动态设计报表
9. **报表转换**：在不同格式之间转换报表
10. **报表存档**：将报表导出为存档格式