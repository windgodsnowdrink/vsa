# Excel - 使用示例

## 快速入门

### 1. 基本用法示例

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
        Console.WriteLine("Excel 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 创建主机构建器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置 Excel 选项
        builder.Services.Configure<ExcelOptions>(options => {
            options.ReadMode = "Auto";
            options.WriteMode = "Xlsx";
            options.EnableDetailedLogging = true;
        });
        
        // 注册服务
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        // 构建主机
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取 Excel 服务
        var excelService = serviceProvider.GetRequiredService<IExcelService>();
        
        // 使用 Excel 功能 - 读取 Excel 文件
        var result = await excelService.ReadExcelAsync("sample.xlsx", "Sheet1");
        Console.WriteLine($"创建事件结果: {result.Success}");
        
        if (result.Success && result.ExcelDocument != null)
        {
            Console.WriteLine($"文件路径: {result.ExcelDocument.FilePath}");
            Console.WriteLine($"工作表数量: {result.ExcelDocument.WorksheetCount}");
            
            foreach (var worksheet in result.ExcelDocument.Worksheets)
            {
                Console.WriteLine($"\n  工作表: {worksheet.Name}");
                Console.WriteLine($"    行数: {worksheet.RowCount}");
                Console.WriteLine($"    列数: {worksheet.ColumnCount}");
            }
        }
    }
}
```

### 2. AOT 单文件执行示例

Excel 提供了 .NET 10 AOT 编译的单文件执行脚本，可以直接运行，无需安装 .NET 运行时。

#### 编译命令

```bash
# 编译为 Windows AOT 单文件
dotnet publish -c Release -r win-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true

# 编译为 Linux AOT 单文件
dotnet publish -c Release -r linux-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true

# 编译为 macOS AOT 单文件
dotnet publish -c Release -r osx-x64 -p:PublishAot=true -p:PublishSingleFile=true --self-contained true
```

#### 运行示例

```bash
# 显示版本信息
./excel_aot version

# 读取 Excel 文件
./excel_aot read sample.xlsx Sheet1

# 列出所有工作表的数据
./excel_aot read sample.xlsx

# 转换 Excel 格式
./excel_aot convert input.xls output.xlsx

# 合并多个 Excel 文件
./excel_aot merge file1.xlsx;file2.xlsx merged.xlsx

# 拆分 Excel 文件
./excel_aot split data.xlsx output/

# 查看帮助信息
./excel_aot help
```

### 3. Excel 文件读取示例

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
        Console.WriteLine("Excel 文件读取示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.ReadMode = "Auto";
            options.EnableDetailedLogging = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 读取 Excel 文件
        string filePath = "sample.xlsx";
        string? sheetName = "Sheet1"; // 可以为 null，表示读取所有工作表
        
        var result = await excelService.ReadExcelAsync(filePath, sheetName);
        
        if (result.Success && result.ExcelDocument != null)
        {
            Console.WriteLine($"成功读取文件: {result.ExcelDocument.FilePath}");
            Console.WriteLine($"工作表数量: {result.ExcelDocument.WorksheetCount}");
            
            foreach (var worksheet in result.ExcelDocument.Worksheets)
            {
                Console.WriteLine($"\n工作表: {worksheet.Name}");
                Console.WriteLine($"  行数: {worksheet.RowCount}");
                Console.WriteLine($"  列数: {worksheet.ColumnCount}");
                
                // 显示前 5 行数据
                int displayRows = Math.Min(5, worksheet.RowCount);
                for (int i = 0; i < displayRows; i++)
                {
                    var row = worksheet.Rows[i];
                    var rowValues = string.Join(" | ", row.Select(cell => cell.Value));
                    Console.WriteLine($"  行 {i + 1}: {rowValues}");
                }
                
                if (worksheet.RowCount > displayRows)
                {
                    Console.WriteLine($"  ... 还有 {worksheet.RowCount - displayRows} 行数据");
                }
            }
        }
        else
        {
            Console.WriteLine($"读取失败: {result.ErrorMessage}");
        }
    }
}
```

### 4. Excel 文件写入示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Excel.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Excel 文件写入示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.WriteMode = "Xlsx";
            options.EnableFormatting = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 准备测试数据
        var data = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object>
            {
                { "ID", 1 },
                { "姓名", "张三" },
                { "年龄", 25 },
                { "邮箱", "zhangsan@example.com" },
                { "工资", 5000.50 },
                { "入职日期", DateTime.Now.AddYears(-1) }
            },
            new Dictionary<string, object>
            {
                { "ID", 2 },
                { "姓名", "李四" },
                { "年龄", 30 },
                { "邮箱", "lisi@example.com" },
                { "工资", 6500.75 },
                { "入职日期", DateTime.Now.AddYears(-2) }
            },
            new Dictionary<string, object>
            {
                { "ID", 3 },
                { "姓名", "王五" },
                { "年龄", 28 },
                { "邮箱", "wangwu@example.com" },
                { "工资", 5800.25 },
                { "入职日期", DateTime.Now.AddYears(-1.5) }
            }
        };
        
        // 写入 Excel 文件
        string outputFilePath = "output.xlsx";
        var result = await excelService.WriteExcelAsync(outputFilePath, data);
        
        if (result.Success)
        {
            Console.WriteLine($"成功写入文件: {outputFilePath}");
            Console.WriteLine($"写入数据行数: {data.Count}");
        }
        else
        {
            Console.WriteLine($"写入失败: {result.ErrorMessage}");
        }
    }
}
```

### 5. 格式转换示例

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
        Console.WriteLine("Excel 格式转换示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.EnableDetailedLogging = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 转换 Excel 文件格式
        string inputFilePath = "input.xls";  // 旧格式
        string outputFilePath = "output.xlsx"; // 新格式
        
        Console.WriteLine($"开始转换: {inputFilePath} -> {outputFilePath}");
        
        var result = await excelService.ConvertExcelAsync(inputFilePath, outputFilePath);
        
        if (result.Success)
        {
            Console.WriteLine("转换成功!");
            Console.WriteLine($"输入文件: {inputFilePath}");
            Console.WriteLine($"输出文件: {outputFilePath}");
            Console.WriteLine($"转换类型: {Path.GetExtension(inputFilePath).TrimStart('.').ToUpper()} -> {Path.GetExtension(outputFilePath).TrimStart('.').ToUpper()}");
        }
        else
        {
            Console.WriteLine($"转换失败: {result.ErrorMessage}");
        }
    }
}
```

### 6. 文件合并示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Excel.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Excel 文件合并示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.EnableDetailedLogging = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 准备要合并的文件列表
        var inputFilePaths = new List<string>
        {
            "file1.xlsx",
            "file2.xlsx",
            "file3.xlsx"
        };
        
        string outputFilePath = "merged.xlsx";
        
        Console.WriteLine($"开始合并 {inputFilePaths.Count} 个文件到: {outputFilePath}");
        foreach (var filePath in inputFilePaths)
        {
            Console.WriteLine($"  - {filePath}");
        }
        
        var result = await excelService.MergeExcelAsync(inputFilePaths, outputFilePath);
        
        if (result.Success)
        {
            Console.WriteLine("\n合并成功!");
            Console.WriteLine($"输出文件: {outputFilePath}");
        }
        else
        {
            Console.WriteLine($"\n合并失败: {result.ErrorMessage}");
        }
    }
}
```

### 7. 文件拆分示例

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
        Console.WriteLine("Excel 文件拆分示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.EnableDetailedLogging = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 准备要拆分的文件和输出目录
        string inputFilePath = "data.xlsx";
        string outputDirectory = "output";
        
        Console.WriteLine($"开始拆分文件: {inputFilePath}");
        Console.WriteLine($"输出目录: {outputDirectory}");
        
        var result = await excelService.SplitExcelAsync(inputFilePath, outputDirectory);
        
        if (result.Success)
        {
            Console.WriteLine("\n拆分成功!");
            foreach (var item in result.Results)
            {
                Console.WriteLine($"  - {item}");
            }
        }
        else
        {
            Console.WriteLine($"\n拆分失败: {result.ErrorMessage}");
        }
    }
}
```

### 8. 性能优化示例

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Excel.AOT;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Excel 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务 - 优化性能设置
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.ReadMode = "Auto";
            options.WriteMode = "Xlsx";
            options.EnableDetailedLogging = false; // 关闭详细日志以提高性能
            options.EnableFormatting = false; // 关闭格式化以提高性能
            options.EnableCompression = true; // 启用压缩以减少文件大小
            options.MaxRows = 100000; // 设置合适的最大行数
            options.MaxColumns = 200; // 设置合适的最大列数
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 性能测试 - 读取 Excel 文件
        string filePath = "large_sample.xlsx";
        
        Console.WriteLine($"开始性能测试: 读取文件 {filePath}");
        
        var stopwatch = Stopwatch.StartNew();
        var result = await excelService.ReadExcelAsync(filePath);
        stopwatch.Stop();
        
        if (result.Success && result.ExcelDocument != null)
        {
            Console.WriteLine("\n性能测试结果:");
            Console.WriteLine($"  读取成功: {result.Success}");
            Console.WriteLine($"  执行时间: {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"  文件路径: {result.ExcelDocument.FilePath}");
            Console.WriteLine($"  工作表数量: {result.ExcelDocument.WorksheetCount}");
            Console.WriteLine($"  总数据行数: {result.ExcelDocument.Worksheets.Sum(w => w.RowCount)}");
            Console.WriteLine($"  平均每行读取时间: {stopwatch.ElapsedMilliseconds / (double)result.ExcelDocument.Worksheets.Sum(w => w.RowCount):F3} ms/行");
        }
        else
        {
            Console.WriteLine($"\n测试失败: {result.ErrorMessage}");
            Console.WriteLine($"  执行时间: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
```

### 9. 错误处理示例

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
        Console.WriteLine("Excel 错误处理示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.EnableDetailedLogging = true;
        });
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        var host = builder.Build();
        var excelService = host.Services.GetRequiredService<IExcelService>();
        
        // 测试1: 读取不存在的文件
        Console.WriteLine("\n测试1: 读取不存在的文件");
        try
        {
            var result = await excelService.ReadExcelAsync("nonexistent_file.xlsx");
            Console.WriteLine($"结果: {result.Success}");
            if (!result.Success)
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"捕获异常: {ex.Message}");
        }
        
        // 测试2: 转换不支持的格式
        Console.WriteLine("\n测试2: 转换不支持的格式");
        try
        {
            var result = await excelService.ConvertExcelAsync("sample.txt", "output.xlsx");
            Console.WriteLine($"结果: {result.Success}");
            if (!result.Success)
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"捕获异常: {ex.Message}");
        }
        
        // 测试3: 写入到无权限的目录
        Console.WriteLine("\n测试3: 写入到无权限的目录");
        try
        {
            var data = new List<Dictionary<string, object>>();
            var result = await excelService.WriteExcelAsync("C:\\Windows\\output.xlsx", data);
            Console.WriteLine($"结果: {result.Success}");
            if (!result.Success)
            {
                Console.WriteLine($"错误信息: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"捕获异常: {ex.Message}");
        }
        
        Console.WriteLine("\n所有测试完成");
    }
}
```

### 10. 扩展开发示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Excel.AOT;

// 自定义 Excel 处理器接口
public interface ICustomExcelProcessor
{
    Task<ExcelDocument> ProcessAsync(ExcelDocument document);
    string GetProcessorName();
}

// 自定义 Excel 处理器实现 - 数据清洗
public class DataCleaningProcessor : ICustomExcelProcessor
{
    public string GetProcessorName() => "数据清洗处理器";
    
    public async Task<ExcelDocument> ProcessAsync(ExcelDocument document)
    {
        Console.WriteLine($"[{GetProcessorName()}] 开始处理文档: {document.FilePath}");
        
        // 模拟数据清洗操作
        await Task.Delay(500);
        
        // 这里可以实现实际的数据清洗逻辑
        // 例如：移除空行、转换数据类型、验证数据等
        
        Console.WriteLine($"[{GetProcessorName()}] 处理完成");
        return document;
    }
}

// 自定义 Excel 处理器实现 - 数据统计
public class DataStatisticsProcessor : ICustomExcelProcessor
{
    public string GetProcessorName() => "数据统计处理器";
    
    public async Task<ExcelDocument> ProcessAsync(ExcelDocument document)
    {
        Console.WriteLine($"[{GetProcessorName()}] 开始处理文档: {document.FilePath}");
        
        // 模拟数据统计操作
        await Task.Delay(300);
        
        // 这里可以实现实际的数据统计逻辑
        // 例如：计算总和、平均值、最大值、最小值等
        
        Console.WriteLine($"[{GetProcessorName()}] 处理完成");
        return document;
    }
}

// 主程序
public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Excel 扩展开发示例");
        Console.WriteLine("=" * 50);
        
        // 创建并配置服务
        var builder = Host.CreateApplicationBuilder();
        builder.Services.Configure<ExcelOptions>(options => {
            options.EnableDetailedLogging = true;
        });
        
        // 注册核心服务
        builder.Services.AddSingleton<IExcelService, ExcelService>();
        
        // 注册自定义处理器
        builder.Services.AddSingleton<ICustomExcelProcessor, DataCleaningProcessor>();
        builder.Services.AddSingleton<ICustomExcelProcessor, DataStatisticsProcessor>();
        
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        // 获取服务
        var excelService = serviceProvider.GetRequiredService<IExcelService>();
        var processors = serviceProvider.GetServices<ICustomExcelProcessor>();
        
        // 读取 Excel 文件
        var readResult = await excelService.ReadExcelAsync("sample.xlsx");
        
        if (readResult.Success && readResult.ExcelDocument != null)
        {
            Console.WriteLine($"\n成功读取文件: {readResult.ExcelDocument.FilePath}");
            
            // 使用自定义处理器处理文档
            var document = readResult.ExcelDocument;
            
            foreach (var processor in processors)
            {
                document = await processor.ProcessAsync(document);
            }
            
            Console.WriteLine("\n所有处理器执行完成");
            
            // 可以将处理后的文档写回文件
            // var writeResult = await excelService.WriteExcelAsync("processed.xlsx", data);
        }
        else
        {
            Console.WriteLine($"读取文件失败: {readResult.ErrorMessage}");
        }
    }
}
```

## 总结

以上示例展示了 Excel 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速入门 Excel 基本操作
2. 了解 AOT 单文件执行的使用方法
3. 掌握 Excel 文件读取和写入
4. 学习格式转换、文件合并和拆分
5. 优化性能，提高处理效率
6. 实现有效的错误处理
7. 扩展开发，添加自定义功能

Excel 技能设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。AOT 编译特性使其在生产环境中具有出色的性能表现，包括快速启动、低内存占用和高效执行。

通过结合 Excel 处理功能和 AOT 编译技术，该技能为 .NET 开发者提供了一个强大、高效的 Excel 处理解决方案，可广泛应用于数据导入导出、报表生成、数据分析、自动化办公等场景。