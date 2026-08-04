# Excel Agent Skill - Excel 技能

## 技能概述

基于 .NET 10 构建的高性能 Excel 技能，为 .NET 开发者提供强大的 Excel 功能。该技能采用 AOT（提前编译）架构，具有启动速度快、内存占用低、执行效率高等特点，适用于各种 Excel 处理场景。

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

在您的主应用程序中注册 Excel 服务：

```csharp
// 注册 Excel 服务
var builder = Host.CreateApplicationBuilder();
builder.Services.Configure<ExcelOptions>(builder.Configuration.GetSection("Excel"));
builder.Services.AddSingleton<IExcelService, ExcelService>();
```

### 使用示例

```csharp
// 获取 Excel 服务
var serviceProvider = builder.Build().Services;
var excelService = serviceProvider.GetRequiredService<IExcelService>();

// 使用 Excel 功能 - 读取 Excel 文件
var result = await excelService.ReadExcelAsync("sample.xlsx", "Sheet1");
Console.WriteLine($"结果: {result.Success}");
if (result.Success && result.ExcelDocument != null)
{
    Console.WriteLine($"工作表数量: {result.ExcelDocument.WorksheetCount}");
}
```

### AOT 单文件执行

Excel 技能提供了 .NET 10 AOT 编译的单文件执行脚本，可以直接运行：

```bash
# 运行版本命令
./excel_aot version

# 读取 Excel 文件
./excel_aot read sample.xlsx Sheet1

# 转换 Excel 格式
./excel_aot convert input.xls output.xlsx

# 合并 Excel 文件
./excel_aot merge file1.xlsx;file2.xlsx merged.xlsx

# 拆分 Excel 文件
./excel_aot split data.xlsx output/
```

## 导航地图

```
excel/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── excel_aot.cs            # Excel AOT 核心实现
    ├── excel_aot.run.json      # 运行配置
    ├── excel_aot.setting.json  # 应用程序设置
    ├── exceldatareader_integration.cs     # ExcelDataReader 集成
    ├── exceldatareader_integration.run.json
    ├── exceldatareader_integration.setting.json
    ├── miniexcel_advanced.cs              # MiniExcel 高级用法
    ├── miniexcel_advanced.run.json
    ├── miniexcel_advanced.setting.json
    ├── miniexcel_full_features.cs         # MiniExcel 完整功能
    ├── miniexcel_full_features.run.json
    ├── miniexcel_full_features.setting.json
    ├── miniexcel_integration.cs           # MiniExcel 集成
    ├── miniexcel_integration.run.json
    ├── miniexcel_integration.setting.json
    ├── miniexcel_production.cs            # MiniExcel 生产环境
    ├── miniexcel_production.run.json
    ├── miniexcel_production.setting.json
    ├── minikube_integration.cs           # Minikube 集成
    ├── minikube_integration.run.json
    └── minikube_integration.setting.json
```

## 主要特性

1. **Excel 文件读取**：支持读取各种格式的 Excel 文件（XLS、XLSX、CSV 等）
2. **Excel 文件写入**：支持写入 Excel 文件，包括格式化和样式设置
3. **格式转换**：支持在不同 Excel 格式之间转换
4. **文件合并**：支持合并多个 Excel 文件
5. **文件拆分**：支持将一个 Excel 文件拆分为多个文件
6. **高性能 AOT 架构**：采用 .NET 10 AOT 编译，启动速度快，内存占用低
7. **多环境配置支持**：支持开发、测试、生产等多环境配置
8. **详细的日志记录**：支持详细的日志记录和性能监控
9. **简单易用的 API**：提供简洁直观的 API 设计
10. **命令行工具**：提供功能完整的命令行工具

## 命令行使用

Excel AOT 引擎提供了丰富的命令行功能：

```bash
# 显示帮助信息
excel_aot help

# 显示版本信息
excel_aot version

# 读取 Excel 文件
excel_aot read <filePath> [sheetName]

# 写入 Excel 文件
excel_aot write <filePath> <jsonData>

# 转换 Excel 格式
excel_aot convert <inputFilePath> <outputFilePath>

# 合并 Excel 文件
excel_aot merge <inputFile1>;<inputFile2>;... <outputFilePath>

# 拆分 Excel 文件
excel_aot split <inputFilePath> <outputDirectory>
```

## 扩展说明

Excel 技能提供了完整的 Excel 处理解决方案，您可以根据需要进行扩展：

1. **自定义 Excel 读取器**：实现自定义的 Excel 读取逻辑
2. **自定义 Excel 写入器**：实现自定义的 Excel 写入逻辑
3. **扩展文件格式支持**：添加对新的文件格式的支持
4. **添加高级功能**：添加数据透视表、图表生成等高级功能
5. **性能优化**：针对特定场景进行性能优化

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：优先使用异步 API 避免阻塞
3. **错误处理**：妥善处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **文件处理**：妥善处理大文件，避免内存溢出
7. **使用 AOT 编译**：对于生产环境，推荐使用 AOT 编译以获得最佳性能
8. **配置管理**：使用配置文件管理不同环境的设置
9. **资源释放**：确保及时释放文件资源
10. **安全考虑**：验证输入文件，避免安全风险

## 性能特性

- **AOT 编译**：提前编译为本地代码，减少启动时间和内存占用
- **内存优化**：采用高效的内存管理策略，适合处理大文件
- **异步设计**：全异步 API 设计，提高并发处理能力
- **性能监控**：内置性能监控功能，便于分析和优化
- **文件流处理**：支持流式处理，减少内存占用
- **并行处理**：支持并行处理，提高处理速度

## 应用场景

1. **数据导入导出**：将数据导入到 Excel 或从 Excel 导出数据
2. **报表生成**：生成各种格式的报表
3. **数据分析**：对 Excel 数据进行分析和处理
4. **格式转换**：在不同 Excel 格式之间转换
5. **批量处理**：批量处理多个 Excel 文件
6. **自动化办公**：自动化处理 Excel 相关的办公任务
7. **数据迁移**：在不同系统之间迁移数据
8. **模板生成**：基于模板生成 Excel 文件
9. **数据验证**：验证 Excel 数据的完整性和准确性
10. **批量打印**：批量打印 Excel 文件