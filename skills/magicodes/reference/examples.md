# magicodes - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Magicodes.IE.Core;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务
        var serviceProvider = BuildServiceProvider();
        var documentService = serviceProvider.GetRequiredService<DocumentService>();
        
        Console.WriteLine("magicodes 基本用法示例");
        Console.WriteLine("=" * 50);
        
        // 准备测试数据
        var exportData = new List<ExportModel> {
            new ExportModel { Id = 1, Name = "测试数据1", Value = 100 },
            new ExportModel { Id = 2, Name = "测试数据2", Value = 200 },
            new ExportModel { Id = 3, Name = "测试数据3", Value = 300 }
        };
        
        // Excel导出示例
        Console.WriteLine("1. Excel导出示例");
        await documentService.ExportExcelAsync(exportData, "output.xlsx");
        Console.WriteLine("Excel导出完成: output.xlsx");
        
        // Excel导入示例
        Console.WriteLine("\n2. Excel导入示例");
        var importedData = await documentService.ImportExcelAsync<ImportModel>("output.xlsx");
        Console.WriteLine($"导入数据行数: {importedData.Count}");
        
        Console.WriteLine("\n基本用法示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<DocumentService>();
        builder.AddSingleton<IImporter, MagicodesImporter>();
        builder.AddSingleton<IExporter, MagicodesExporter>();
        return builder.BuildServiceProvider();
    }
    
    public class ExportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
    
    public class ImportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("magicodes 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var builder = new ServiceCollection();
        
        // 配置文档处理选项
        builder.Configure<DocumentProcessingOptions>(options => {
            options.MaxConcurrentRequests = 100;
            options.ChannelCapacity = 10000;
            options.StreamPoolSize = 16;
            options.TimeoutSeconds = 300;
        });
        
        // 注册服务
        builder.AddSingleton<IImporter, MagicodesImporter>();
        builder.AddSingleton<IExporter, MagicodesExporter>();
        builder.AddSingleton<DocumentService>();
        
        var serviceProvider = builder.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<DocumentProcessingOptions>>().Value;
        Console.WriteLine($"配置信息: 最大并发={settings.MaxConcurrentRequests}, 通道容量={settings.ChannelCapacity}");
        
        // 使用服务
        var documentService = serviceProvider.GetRequiredService<DocumentService>();
        
        // 测试Word导出
        Console.WriteLine("\n测试Word导出");
        var wordData = new List<WordExportModel> {
            new WordExportModel { Title = "标题1", Content = "内容1" },
            new WordExportModel { Title = "标题2", Content = "内容2" }
        };
        await documentService.ExportWordAsync(wordData, "output.docx");
        Console.WriteLine("Word导出完成: output.docx");
        
        Console.WriteLine("\n高级配置示例完成！");
    }
    
    public class DocumentProcessingOptions
    {
        public int MaxConcurrentRequests { get; set; }
        public int ChannelCapacity { get; set; }
        public int StreamPoolSize { get; set; }
        public int TimeoutSeconds { get; set; }
    }
    
    public class WordExportModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
```

### 3. 性能优化示例

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("magicodes 性能优化示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var documentService = serviceProvider.GetRequiredService<DocumentService>();
        
        // 准备测试数据
        var testData = new List<PerformanceTestModel>();
        for (int i = 0; i < 1000; i++)
        {
            testData.Add(new PerformanceTestModel {
                Id = i + 1,
                Name = $"测试数据{i + 1}",
                Value = i * 100,
                Description = $"这是第{i + 1}条测试数据的详细描述"
            });
        }
        
        // 性能测试
        const int iterations = 10;
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            Console.WriteLine($"测试迭代 {i + 1}/{iterations}");
            await documentService.ExportExcelAsync(testData, $"performance_test_{i + 1}.xlsx");
        }
        
        stopwatch.Stop();
        Console.WriteLine($"\n性能测试结果:");
        Console.WriteLine($"总执行时间: {stopwatch.Elapsed.TotalSeconds:F2} 秒");
        Console.WriteLine($"平均每次执行: {stopwatch.Elapsed.TotalSeconds / iterations:F3} 秒");
        Console.WriteLine($"处理速度: {testData.Count * iterations / stopwatch.Elapsed.TotalSeconds:F2} 条/秒");
        
        Console.WriteLine("\n性能优化示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<DocumentService>();
        builder.AddSingleton<IImporter, MagicodesImporter>();
        builder.AddSingleton<IExporter, MagicodesExporter>();
        return builder.BuildServiceProvider();
    }
    
    public class PerformanceTestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}
```

### 4. 错误处理示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("magicodes 错误处理示例");
        Console.WriteLine("=" * 50);
        
        var serviceProvider = BuildServiceProvider();
        var documentService = serviceProvider.GetRequiredService<DocumentService>();
        
        try
        {
            Console.WriteLine("1. 测试正常Excel导出");
            var normalData = new List<NormalModel> {
                new NormalModel { Id = 1, Name = "正常数据" }
            };
            await documentService.ExportExcelAsync(normalData, "normal_output.xlsx");
            Console.WriteLine("✓ Excel导出成功");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"✗ 超时错误: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"✗ 操作错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 通用错误: {ex.Message}");
        }
        
        try
        {
            Console.WriteLine("\n2. 测试不存在的文件导入");
            var nonExistentData = await documentService.ImportExcelAsync<ImportModel>("non_existent_file.xlsx");
            Console.WriteLine($"导入数据行数: {nonExistentData.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ 文件不存在错误: {ex.Message}");
        }
        
        Console.WriteLine("\n错误处理示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<DocumentService>();
        builder.AddSingleton<IImporter, MagicodesImporter>();
        builder.AddSingleton<IExporter, MagicodesExporter>();
        return builder.BuildServiceProvider();
    }
    
    public class NormalModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    
    public class ImportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

### 5. AOT 编译示例

```csharp
#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web 
#:package Magicodes.IE.Core@2.6.0 
#:package Magicodes.IE.Excel@2.6.0 
#:package Magicodes.IE.Pdf@2.6.0 
#:package Magicodes.IE.Word@2.6.0 
#:package Magicodes.IE.Html@2.6.0 
#:package System.Threading.Channels@8.0.0 
#:property LangVersion=preview 
#:property TargetFramework=net10.0 
#:property Nullable=enable 
#:property ImplicitUsings=enable 
#:property PublishAot=true 
#:property IncludeNativeLibrariesForSelfExtract=true 
#:property EnableCppCodeGen=true 
#:property PublishSingleFile=true 
#:property SelfContained=true 
#:property RuntimeIdentifier=win-x64 

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class AotExample
{
    public static async Task Main()
    {
        Console.WriteLine("magicodes AOT 编译示例");
        Console.WriteLine("=" * 50);
        Console.WriteLine("使用 AOT 编译的单文件可执行程序");
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var documentService = serviceProvider.GetRequiredService<DocumentService>();
        
        // 测试AOT编译后的性能
        var testData = new List<AotTestModel> {
            new AotTestModel { Id = 1, Name = "AOT测试1", Value = 100 },
            new AotTestModel { Id = 2, Name = "AOT测试2", Value = 200 }
        };
        
        Console.WriteLine("\n测试AOT编译后的Excel导出");
        await documentService.ExportExcelAsync(testData, "aot_test_output.xlsx");
        Console.WriteLine("✓ AOT Excel导出成功");
        
        Console.WriteLine("\n测试AOT编译后的PDF导出");
        await documentService.ExportPdfAsync(testData, "aot_test_output.pdf");
        Console.WriteLine("✓ AOT PDF导出成功");
        
        Console.WriteLine("\nAOT 编译示例完成！");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddSingleton<DocumentService>();
        builder.AddSingleton<IImporter, MagicodesImporter>();
        builder.AddSingleton<IExporter, MagicodesExporter>();
        return builder.BuildServiceProvider();
    }
    
    public class AotTestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
```

## 总结

以上示例展示了 magicodes 技能的主要功能和使用方法。通过这些示例，您可以：

1. 快速上手基本操作
2. 配置高级选项
3. 优化性能
4. 处理错误情况
5. 使用 AOT 编译提升性能

系统设计遵循 .NET 10 最佳实践，具有良好的可扩展性和可维护性，适用于各种规模和复杂度的项目。

### 支持的文档格式

- **Excel**: .xlsx, .xls 格式的导入导出
- **Word**: .docx 格式的导出
- **PDF**: .pdf 格式的导出
- **HTML**: .html 格式的导入导出

### 性能优化特点

- **Threading.Channels**: 高效的异步事件队列处理
- **ObjectPool**: 减少对象创建开销
- **Span 零拷贝**: 减少内存分配和复制
- **TailLatencyOptimizer**: 尾延迟优化
- **AOT 编译**: 提升启动速度和运行性能
