# Excel - 参考文档

## 概述

Excel 是基于 .NET 10 构建的高性能 Excel 系统，专为 .NET 开发者设计。它采用 AOT（提前编译）架构，具有启动速度快、内存占用低、执行效率高等特点，适用于各种 Excel 处理场景。

## 核心组件

### 1. Excel 服务 (IExcelService)
- **位置**: scripts/excel_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - Excel 文件读取
  - Excel 文件写入
  - 格式转换
  - 文件合并
  - 文件拆分
  - 性能优化
  - 错误处理
  - 日志记录
  - AOT 编译支持

### 2. Excel AOT 引擎 (ExcelAotEngine)
- **位置**: scripts/excel_aot.cs
- **功能**: 命令行执行引擎
- **特性**: 
  - 命令行界面
  - 多种命令支持
  - 结果格式化输出
  - AOT 编译
  - 单文件执行

### 3. 数据模型

#### ExcelCell
- **功能**: 表示 Excel 单元格数据
- **属性**: 
  - RowIndex: 行索引
  - ColumnIndex: 列索引
  - Value: 单元格值
  - DataType: 数据类型
  - Format: 格式

#### ExcelWorksheet
- **功能**: 表示 Excel 工作表数据
- **属性**: 
  - Name: 工作表名称
  - Rows: 行数据
  - RowCount: 行数
  - ColumnCount: 列数

#### ExcelDocument
- **功能**: 表示 Excel 文档数据
- **属性**: 
  - FilePath: 文件路径
  - Worksheets: 工作表列表
  - WorksheetCount: 工作表数量

## 架构设计

### 分层架构

```
┌───────────────────────────────────────────────────┐
│                   命令行界面层                    │
│    (ExcelAotEngine, 命令解析与执行)             │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   服务接口层                      │
│              (IExcelService)                    │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   服务实现层                      │
│              (ExcelService)                     │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   数据访问层                      │
│    (Excel 读取器、写入器、文件处理)             │
└───────────────────────────────────────────────────┘
                          ↓
┌───────────────────────────────────────────────────┐
│                   基础设施层                      │
│ (依赖注入、日志记录、配置管理、性能监控)         │
└───────────────────────────────────────────────────┘
```

## 使用示例

### 基本用法

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Excel.AOT;

public class Program
{
    public static async Task Main()
    {
        // 创建主机构建器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置和注册服务
        builder.Services.Configure<ExcelOptions>(options => {
            options.ReadMode = "Auto";
            options.WriteMode = "Xlsx";
            options.EnableDetailedLogging = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取 Excel 服务
        var excelService = serviceProvider.GetRequiredService<IExcelService>();
        
        // 使用 Excel 功能
        var result = await excelService.ReadExcelAsync("sample.xlsx", "Sheet1");
        Console.WriteLine($"读取结果: {result.Success}");
        
        if (result.Success && result.ExcelDocument != null)
        {
            Console.WriteLine($"文件路径: {result.ExcelDocument.FilePath}");
            Console.WriteLine($"工作表数量: {result.ExcelDocument.WorksheetCount}");
        }
    }
}
```

### AOT 单文件使用

```bash
# 运行版本命令
./excel_aot version

# 读取 Excel 文件
./excel_aot read sample.xlsx Sheet1

# 转换 Excel 格式
./excel_aot convert input.xls output.xlsx

# 合并 Excel 文件
./excel_aot merge file1.xlsx;file2.xlsx merged.xlsx
```

### 高级配置

```csharp
// 配置 Excel 选项
var excelOptions = new ExcelOptions
{
    ReadMode = "Auto",
    WriteMode = "Xlsx",
    MaxRows = 100000,
    MaxColumns = 200,
    EnableDataValidation = true,
    EnableFormatting = true,
    EnableCompression = true,
    RequestTimeoutMs = 60000,
    EnableDetailedLogging = true,
    EnablePerformanceMonitoring = true
};

// 注册配置
builder.Services.Configure<ExcelOptions>(options =>
{
    options.ReadMode = excelOptions.ReadMode;
    options.WriteMode = excelOptions.WriteMode;
    options.MaxRows = excelOptions.MaxRows;
    options.MaxColumns = excelOptions.MaxColumns;
    options.EnableDataValidation = excelOptions.EnableDataValidation;
    options.EnableFormatting = excelOptions.EnableFormatting;
    options.EnableCompression = excelOptions.EnableCompression;
    options.RequestTimeoutMs = excelOptions.RequestTimeoutMs;
    options.EnableDetailedLogging = excelOptions.EnableDetailedLogging;
    options.EnablePerformanceMonitoring = excelOptions.EnablePerformanceMonitoring;
});
```

## 配置选项

### Excel 配置

```json
{
  "Excel": {
    "ReadMode": "Auto",                // Excel 读取模式
    "WriteMode": "Xlsx",               // Excel 写入模式
    "MaxRows": 10000,                  // 最大行数
    "MaxColumns": 100,                 // 最大列数
    "EnableDataValidation": false,     // 是否启用数据验证
    "EnableFormatting": true,          // 是否启用格式化
    "EnableCompression": true,         // 是否启用压缩
    "RequestTimeoutMs": 30000,         // 请求超时时间（毫秒）
    "EnableDetailedLogging": false,    // 是否启用详细日志
    "EnablePerformanceMonitoring": true // 是否启用性能监控
  }
}
```

## 性能优化

1. **使用 AOT 编译**：对于生产环境，推荐使用 AOT 编译以获得最佳性能
2. **流式处理**：对于大文件，使用流式处理减少内存占用
3. **异步编程**：使用异步 API 避免阻塞
4. **批量处理**：对于大量文件，使用批量处理提高效率
5. **内存管理**：合理设置最大行数和列数，避免内存溢出
6. **关闭不必要功能**：对于不需要的功能，如格式化，可以关闭以提高性能
7. **启用压缩**：对于大文件，启用压缩减少存储和传输开销
8. **性能监控**：启用性能监控，便于分析和优化

## 故障排除

### 常见问题

1. **文件读取失败**
   - 检查文件是否存在
   - 验证文件格式是否受支持
   - 检查文件权限
   - 查看日志输出获取详细错误信息

2. **内存溢出**
   - 对于大文件，考虑使用流式处理
   - 减少每次处理的数据量
   - 增加内存限制
   - 优化配置，减少最大行数和列数

3. **性能问题**
   - 启用性能监控查看瓶颈
   - 考虑使用 AOT 编译
   - 优化文件大小和结构
   - 调整配置参数

4. **命令行执行失败**
   - 检查命令格式是否正确
   - 查看错误信息获取详细原因
   - 检查文件路径是否正确
   - 验证文件权限

## 扩展开发

### 添加自定义 Excel 读取器

```csharp
// 自定义 Excel 读取器接口
public interface ICustomExcelReader
{
    Task<ExcelDocument> ReadAsync(string filePath, string? sheetName = null);
    bool CanRead(string filePath);
}

// 自定义 Excel 读取器实现
public class CustomExcelReader : ICustomExcelReader
{
    // 实现自定义 Excel 读取逻辑
    public async Task<ExcelDocument> ReadAsync(string filePath, string? sheetName = null)
    {
        // 实现读取逻辑
        return new ExcelDocument();
    }
    
    public bool CanRead(string filePath)
    {
        // 检查是否支持该文件格式
        var extension = Path.GetExtension(filePath).ToLower();
        return extension == ".xls" || extension == ".xlsx";
    }
}

// 注册自定义读取器
builder.Services.AddSingleton<ICustomExcelReader, CustomExcelReader>();
```

### 扩展命令行功能

可以通过修改 `ExcelAotEngine` 类来扩展命令行功能，添加新的命令处理逻辑。

## AOT 编译说明

### 编译命令

```bash
# 编译为 AOT 单文件
dotnet publish -c Release -r win-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true

# 编译为 Linux AOT 单文件
dotnet publish -c Release -r linux-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true

# 编译为 macOS AOT 单文件
dotnet publish -c Release -r osx-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true
```

### AOT 优势

- **快速启动**：AOT 编译的应用程序启动时间比 JIT 编译的应用程序快得多
- **低内存占用**：AOT 编译的应用程序内存占用更低
- **无需 JIT 编译**：运行时不需要 JIT 编译，减少了运行时开销
- **自包含**：可以编译为单个可执行文件，便于部署和分发
- **跨平台**：支持多个平台的 AOT 编译

## 最佳实践

1. **文件处理**
   - 妥善处理大文件，避免内存溢出
   - 及时释放文件资源
   - 验证输入文件，避免安全风险

2. **错误处理**
   - 妥善处理异常情况
   - 提供清晰的错误信息
   - 记录详细的日志

3. **性能考虑**
   - 对于生产环境，使用 AOT 编译
   - 合理设置配置参数
   - 监控关键性能指标

4. **安全考虑**
   - 验证输入文件，避免恶意文件
   - 限制文件大小和内容
   - 考虑使用沙箱环境处理不可信文件

5. **配置管理**
   - 使用配置文件管理不同环境的设置
   - 加密敏感配置
   - 定期备份配置文件

6. **测试策略**
   - 编写单元测试和集成测试
   - 测试各种文件格式和大小
   - 测试边界情况
   - 进行性能测试

## 应用场景

### 1. 数据导入导出

用于将数据从数据库或其他系统导入到 Excel，或从 Excel 导出到其他系统。

### 2. 报表生成

根据数据生成各种格式的报表，如财务报表、销售报表等。

### 3. 数据分析

对 Excel 数据进行分析和处理，如数据清洗、统计分析等。

### 4. 格式转换

在不同的 Excel 格式之间转换，如从 XLS 转换为 XLSX。

### 5. 批量处理

批量处理多个 Excel 文件，如批量转换、批量合并等。

### 6. 自动化办公

自动化处理 Excel 相关的办公任务，如自动生成报告、自动发送邮件等。

### 7. 数据迁移

在不同系统之间迁移数据，使用 Excel 作为中间格式。

### 8. 模板生成

基于模板生成 Excel 文件，如合同模板、发票模板等。

### 9. 数据验证

验证 Excel 数据的完整性和准确性，如检查必填字段、数据格式等。

### 10. 批量打印

批量打印多个 Excel 文件，如批量打印报表、发票等。