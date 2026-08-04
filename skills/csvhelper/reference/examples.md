# CsvHelper AOT - 使用示例

## 快速开始

### 1. 基本用法示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CsvHelper.AOT;

// 定义测试模型
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置CsvHelper选项
        builder.Services.Configure<CsvHelperOptions>(options => {
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.DefaultDelimiter = ',';
            options.HasHeaderRecord = true;
        });
        
        // 注册服务
        builder.Services.AddSingleton<ICsvHelperService, CsvHelperService>();
        builder.Services.AddSingleton<CsvHelperAotEngine>();
        
        var host = builder.Build();
        var serviceProvider = host.Services;
        
        Console.WriteLine("CsvHelper AOT 基本用法示例");
        Console.WriteLine("=" * 60);
        
        // 使用CsvHelper AOT引擎
        var engine = serviceProvider.GetRequiredService<CsvHelperAotEngine>();
        
        // 创建测试数据
        var records = new List<Product>
        {
            new Product { Id = 1, Name = "产品1", Price = 100.00, Category = "分类1" },
            new Product { Id = 2, Name = "产品2", Price = 200.00, Category = "分类2" },
            new Product { Id = 3, Name = "产品3", Price = 300.00, Category = "分类1" },
            new Product { Id = 4, Name = "产品4", Price = 400.00, Category = "分类3" },
            new Product { Id = 5, Name = "产品5", Price = 500.00, Category = "分类2" }
        };
        
        Console.WriteLine($"准备导出 {records.Count} 条记录...");
        
        // 导出为CSV字符串
        var csvString = await engine.ExportToCsvStringAsync(records);
        Console.WriteLine("\n导出CSV字符串成功");
        Console.WriteLine("CSV内容:");
        Console.WriteLine(csvString);
        
        // 导出为CSV文件
        var result = await engine.ExportToCsvFileAsync(records, "products.csv");
        Console.WriteLine($"\n导出CSV文件{result.Success ? "成功" : "失败"}，路径: products.csv，记录数: {result.RecordCount}");
        
        // 获取服务状态
        var status = await engine.GetStatusAsync();
        Console.WriteLine($"\n服务状态: 运行中={status.IsRunning}, 已处理文件数={status.ProcessedFiles}, 已处理记录数={status.ProcessedRecords}");
    }
}
```

### 2. 自定义CSV配置示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CsvHelper.AOT;

// 定义测试模型
public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 注册服务
        builder.Services.AddSingleton<ICsvHelperService, CsvHelperService>();
        builder.Services.AddSingleton<CsvHelperAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CsvHelperAotEngine>();
        
        Console.WriteLine("CsvHelper AOT 自定义CSV配置示例");
        Console.WriteLine("=" * 60);
        
        // 创建测试数据
        var orders = new List<Order>
        {
            new Order { OrderId = 1001, CustomerName = "客户A", OrderDate = DateTime.Now.AddDays(-10), TotalAmount = 1500.50, Status = "已发货" },
            new Order { OrderId = 1002, CustomerName = "客户B", OrderDate = DateTime.Now.AddDays(-5), TotalAmount = 2300.75, Status = "已付款" },
            new Order { OrderId = 1003, CustomerName = "客户C", OrderDate = DateTime.Now.AddDays(-2), TotalAmount = 890.20, Status = "待发货" }
        };
        
        Console.WriteLine("示例1: 使用分号分隔符");
        // 创建自定义CSV配置 - 使用分号分隔符
        var semicolonConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            IgnoreBlankLines = true
        };
        
        var semicolonCsv = await engine.ExportToCsvStringAsync(orders, semicolonConfig);
        Console.WriteLine(semicolonCsv);
        
        Console.WriteLine("\n示例2: 自定义日期格式");
        // 创建自定义CSV配置 - 自定义日期格式
        var customDateConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            DateFormat = "yyyy-MM-dd"
        };
        
        var dateFormatCsv = await engine.ExportToCsvStringAsync(orders, customDateConfig);
        Console.WriteLine(dateFormatCsv);
        
        Console.WriteLine("\n示例3: 不包含标题行");
        // 创建自定义CSV配置 - 不包含标题行
        var noHeaderConfig = new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = false,
            IgnoreBlankLines = true
        };
        
        var noHeaderCsv = await engine.ExportToCsvStringAsync(orders, noHeaderConfig);
        Console.WriteLine(noHeaderCsv);
    }
}
```

### 3. 批量导出示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CsvHelper.AOT;

// 定义测试模型
public class Employee
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 配置CsvHelper选项
        builder.Services.Configure<CsvHelperOptions>(options => {
            options.EnableCache = true;
            options.WorkerCount = Environment.ProcessorCount;
        });
        
        // 注册服务
        builder.Services.AddSingleton<ICsvHelperService, CsvHelperService>();
        builder.Services.AddSingleton<CsvHelperAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CsvHelperAotEngine>();
        
        Console.WriteLine("CsvHelper AOT 批量导出示例");
        Console.WriteLine("=" * 60);
        
        // 准备测试数据
        var employees1 = new List<Employee>
        {
            new Employee { EmployeeId = 1, FullName = "张三", Department = "技术部", Salary = 12000, HireDate = DateTime.Now.AddYears(-3) },
            new Employee { EmployeeId = 2, FullName = "李四", Department = "技术部", Salary = 15000, HireDate = DateTime.Now.AddYears(-2) },
            new Employee { EmployeeId = 3, FullName = "王五", Department = "市场部", Salary = 10000, HireDate = DateTime.Now.AddYears(-1) }
        };
        
        var employees2 = new List<Employee>
        {
            new Employee { EmployeeId = 4, FullName = "赵六", Department = "财务部", Salary = 11000, HireDate = DateTime.Now.AddYears(-2) },
            new Employee { EmployeeId = 5, FullName = "孙七", Department = "人力资源部", Salary = 9500, HireDate = DateTime.Now.AddYears(-1) }
        };
        
        var employees3 = new List<Employee>
        {
            new Employee { EmployeeId = 6, FullName = "周八", Department = "技术部", Salary = 13000, HireDate = DateTime.Now.AddYears(-1) },
            new Employee { EmployeeId = 7, FullName = "吴九", Department = "市场部", Salary = 10500, HireDate = DateTime.Now.AddMonths(-6) }
        };
        
        // 准备批量导出请求
        var exportRequests = new List<CsvExportRequest<Employee>>();
        
exportRequests.Add(new CsvExportRequest<Employee>
{
    RequestId = "Export-1",
    Records = employees1,
    FilePath = "employees_tech.csv",
    Configuration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
    {
        Delimiter = ",",
        HasHeaderRecord = true
    }
});

exportRequests.Add(new CsvExportRequest<Employee>
{
    RequestId = "Export-2",
    Records = employees2,
    FilePath = "employees_admin.csv",
    Configuration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
    {
        Delimiter = ",",
        HasHeaderRecord = true
    }
});

exportRequests.Add(new CsvExportRequest<Employee>
{
    RequestId = "Export-3",
    Records = employees3,
    FilePath = "employees_other.csv",
    Configuration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
    {
        Delimiter = ",",
        HasHeaderRecord = true
    }
});
        
        Console.WriteLine($"准备批量导出 {exportRequests.Count} 个CSV文件...");
        
        // 执行批量导出
        var results = await engine.ExportBatchAsync(exportRequests);
        
        // 处理结果
        Console.WriteLine("\n批量导出结果:");
        Console.WriteLine("-" * 40);
        
        int successCount = 0;
        int totalRecords = 0;
        
        foreach (var result in results)
        {
            if (result.Success)
            {
                successCount++;
                totalRecords += result.RecordCount;
                Console.WriteLine($"✓ 成功: 记录数={result.RecordCount}, 时间={result.ExecutionTimeMs}ms");
            }
            else
            {
                Console.WriteLine($"✗ 失败: 错误={result.ErrorMessage}");
            }
        }
        
        Console.WriteLine("-" * 40);
        Console.WriteLine($"统计: 总请求数={exportRequests.Count}, 成功数={successCount}, 总记录数={totalRecords}");
    }
}
```

### 4. 从CSV导入数据示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CsvHelper.AOT;

// 定义测试模型
public class Student
{
    public int StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Class { get; set; } = string.Empty;
    public decimal Score { get; set; }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 注册服务
        builder.Services.AddSingleton<ICsvHelperService, CsvHelperService>();
        builder.Services.AddSingleton<CsvHelperAotEngine>();
        
        var host = builder.Build();
        var engine = host.Services.GetRequiredService<CsvHelperAotEngine>();
        
        Console.WriteLine("CsvHelper AOT 从CSV导入数据示例");
        Console.WriteLine("=" * 60);
        
        // 示例1: 从CSV字符串导入
        Console.WriteLine("示例1: 从CSV字符串导入");
        
        var csvContent = @"StudentId,Name,Age,Class,Score
1,小明,18,高三1班,95.5
2,小红,17,高二3班,88.0
3,小刚,18,高三2班,92.5
4,小丽,17,高二1班,90.0
5,小强,18,高三1班,85.5";
        
        try
        {
            var students = await engine.ImportFromCsvStringAsync<Student>(csvContent);
            Console.WriteLine($"成功导入 {students.Count} 条学生记录:");
            
            foreach (var student in students)
            {
                Console.WriteLine($"  ID: {student.StudentId}, 姓名: {student.Name}, 年龄: {student.Age}, 班级: {student.Class}, 分数: {student.Score}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"导入失败: {ex.Message}");
        }
        
        // 示例2: 从CSV文件导入
        Console.WriteLine("\n示例2: 从CSV文件导入");
        
        // 首先创建一个测试CSV文件
        var testStudents = new List<Student>
        {
            new Student { StudentId = 101, Name = "小王", Age = 19, Class = "大一1班", Score = 87.5 },
            new Student { StudentId = 102, Name = "小张", Age = 18, Class = "大一2班", Score = 91.0 },
            new Student { StudentId = 103, Name = "小刘", Age = 19, Class = "大一1班", Score = 89.5 }
        };
        
        // 导出到文件
        await engine.ExportToCsvFileAsync(testStudents, "students.csv");
        
        try
        {
            var result = await engine.ImportFromCsvFileAsync<Student>("students.csv");
            if (result.Success && result.ResultData != null)
            {
                Console.WriteLine($"成功从文件导入 {result.ResultData.Count} 条学生记录:");
                
                foreach (var student in result.ResultData)
                {
                    Console.WriteLine($"  ID: {student.StudentId}, 姓名: {student.Name}, 年龄: {student.Age}, 班级: {student.Class}, 分数: {student.Score}");
                }
            }
            else
            {
                Console.WriteLine($"导入失败: {result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"导入失败: {ex.Message}");
        }
    }
}
```

### 5. 自定义记录映射示例

```csharp
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// 定义测试模型
public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public decimal TotalSpent { get; set; }
}

// 自定义记录映射
public class CustomerMap : ClassMap<Customer>
{
    public CustomerMap()
    {
        // 映射ID列
        Map(m => m.Id).Name("客户ID");
        
        // 映射姓名，将FirstName和LastName合并为一个列
        Map(m => m.FirstName).Name("姓名").Convert(args => {
            // 这里演示自定义转换，实际使用时可以根据需求修改
            return args.Row.GetField<string>("姓名")?.Split(' ')[0] ?? string.Empty;
        });
        
        // 映射姓
        Map(m => m.LastName).Name("姓名").Convert(args => {
            return args.Row.GetField<string>("姓名")?.Split(' ')[1] ?? string.Empty;
        });
        
        // 映射邮箱
        Map(m => m.Email).Name("电子邮箱");
        
        // 映射注册日期，使用自定义日期格式
        Map(m => m.RegistrationDate).Name("注册日期").Format("yyyy年MM月dd日");
        
        // 映射消费总额，使用货币格式
        Map(m => m.TotalSpent).Name("消费总额").Format("C2", CultureInfo.GetCultureInfo("zh-CN"));
    }
}

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var builder = Host.CreateApplicationBuilder();
        
        // 注册服务
        builder.Services.AddSingleton<MyCsvHelperService>();
        
        var host = builder.Build();
        var csvService = host.Services.GetRequiredService<MyCsvHelperService>();
        
        Console.WriteLine("CsvHelper AOT 自定义记录映射示例");
        Console.WriteLine("=" * 60);
        
        // 创建测试数据
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "张", LastName = "三", Email = "zhangsan@example.com", RegistrationDate = DateTime.Now.AddYears(-2), TotalSpent = 1500.50 },
            new Customer { Id = 2, FirstName = "李", LastName = "四", Email = "lisi@example.com", RegistrationDate = DateTime.Now.AddYears(-1), TotalSpent = 2300.75 },
            new Customer { Id = 3, FirstName = "王", LastName = "五", Email = "wangwu@example.com", RegistrationDate = DateTime.Now.AddMonths(-6), TotalSpent = 890.20 }
        };
        
        // 使用自定义映射导出
        var csvString = await csvService.ExportWithCustomMapAsync(customers);
        Console.WriteLine("使用自定义映射导出的CSV:");
        Console.WriteLine(csvString);
    }
}

// 自定义CsvHelper服务，演示自定义映射的使用
public class MyCsvHelperService
{
    public async Task<string> ExportWithCustomMapAsync<T>(IEnumerable<T> records) where T : class
    {
        using (var writer = new StringWriter())
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        }))
        {
            // 注册自定义映射
            if (typeof(T) == typeof(Customer))
            {
                csv.Context.RegisterClassMap<CustomerMap>();
            }
            else
            {
                csv.Context.AutoMap();
            }
            
            // 写入记录
            await csv.WriteRecordsAsync(records);
            await writer.FlushAsync();
            
            return writer.ToString();
        }
    }
}
```

## 总结

通过以上示例，您可以学习到CsvHelper AOT的主要功能和使用方法：

1. **基本用法**：如何初始化服务、创建测试数据、导出为CSV字符串和文件
2. **自定义CSV配置**：如何配置不同的分隔符、日期格式、标题行等
3. **批量导出**：如何同时导出多个CSV文件，提高效率
4. **从CSV导入**：如何从CSV字符串和文件导入数据到对象列表
5. **自定义记录映射**：如何创建和使用自定义映射，实现复杂的数据转换

CsvHelper AOT的设计遵循.NET 10最佳实践，具有良好的可扩展性和可维护性，适合各种规模和复杂度的项目。

## 常见使用场景

1. **数据导出**：将数据库数据导出为CSV文件，用于报表生成或数据备份
2. **数据迁移**：在不同系统之间迁移数据，使用CSV作为中间格式
3. **API响应**：提供CSV格式的API响应，方便客户端直接下载和使用
4. **批量处理**：批量生成多个CSV文件，用于不同部门或客户
5. **数据分析**：将数据导出为CSV格式，用于后续的数据分析和处理
6. **日志导出**：将系统日志导出为CSV格式，方便日志分析和监控

通过灵活运用CsvHelper AOT，您可以在.NET应用中实现高效、可靠的CSV处理功能，同时享受AOT编译带来的性能优势。