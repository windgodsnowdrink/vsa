# ClosedXML AOT 使用示例

## 1. 基本Excel处理示例

### 1.1 读取Excel文件数据

```csharp
using (var workbook = new XLWorkbook("input.xlsx"))
{
    var worksheet = workbook.Worksheet(1);
    
    // 读取所有数据
    foreach (var row in worksheet.RowsUsed())
    {
        foreach (var cell in row.CellsUsed())
        {
            Console.WriteLine($"Row {row.RowNumber()}, Column {cell.ColumnNumber()}: {cell.Value}");
        }
    }
}
```

### 1.2 写入Excel文件

```csharp
using (var workbook = new XLWorkbook())
{
    var worksheet = workbook.Worksheets.Add("Sheet1");
    
    // 写入标题
    worksheet.Cell(1, 1).Value = "姓名";
    worksheet.Cell(1, 2).Value = "年龄";
    worksheet.Cell(1, 3).Value = "城市";
    
    // 写入数据
    worksheet.Cell(2, 1).Value = "张三";
    worksheet.Cell(2, 2).Value = 25;
    worksheet.Cell(2, 3).Value = "北京";
    
    worksheet.Cell(3, 1).Value = "李四";
    worksheet.Cell(3, 2).Value = 30;
    worksheet.Cell(3, 3).Value = "上海";
    
    // 保存文件
    workbook.SaveAs("output.xlsx");
}
```

## 2. 高级Excel处理示例

### 2.1 格式化Excel文件

```csharp
using (var workbook = new XLWorkbook())
{
    var worksheet = workbook.Worksheets.Add("格式化示例");
    
    // 写入数据
    worksheet.Cell(1, 1).Value = "产品名称";
    worksheet.Cell(1, 2).Value = "数量";
    worksheet.Cell(1, 3).Value = "单价";
    worksheet.Cell(1, 4).Value = "金额";
    
    // 设置标题样式
    var headerRow = worksheet.Row(1);
    headerRow.Style.Font.Bold = true;
    headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
    headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    
    // 设置边框
    var range = worksheet.Range("A1:D10");
    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
    
    // 设置数字格式
    worksheet.Column(2).Style.NumberFormat.Format = "#,##0";
    worksheet.Column(3).Style.NumberFormat.Format = "¥#,##0.00";
    worksheet.Column(4).Style.NumberFormat.Format = "¥#,##0.00";
    
    workbook.SaveAs("formatted.xlsx");
}
```

### 2.2 使用公式

```csharp
using (var workbook = new XLWorkbook())
{
    var worksheet = workbook.Worksheets.Add("公式示例");
    
    // 写入数据
    worksheet.Cell(1, 1).Value = "数值1";
    worksheet.Cell(1, 2).Value = "数值2";
    worksheet.Cell(1, 3).Value = "和";
    worksheet.Cell(1, 4).Value = "平均值";
    
    // 填充数据
    for (int i = 2; i <= 10; i++)
    {
        worksheet.Cell(i, 1).Value = i * 10;
        worksheet.Cell(i, 2).Value = i * 20;
        worksheet.Cell(i, 3).FormulaA1 = $"A{i} + B{i}";
        worksheet.Cell(i, 4).FormulaA1 = $"AVERAGE(A{i}:B{i})";
    }
    
    // 计算总和
    worksheet.Cell(11, 1).Value = "总和:";
    worksheet.Cell(11, 3).FormulaA1 = "SUM(C2:C10)";
    
    workbook.SaveAs("formula.xlsx");
}
```

## 3. ClosedXML AOT引擎示例

### 3.1 基本引擎使用

```csharp
// 创建主机
var builder = Host.CreateApplicationBuilder(args);

// 配置日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// 注册服务
builder.Services.AddSingleton<IExcelService, ExcelService>();
builder.Services.AddSingleton<ClosedXmlAotEngine>();

// 构建主机
var host = builder.Build();
var engine = host.Services.GetRequiredService<ClosedXmlAotEngine>();

// 执行Excel处理
var result = await engine.ExecuteAsync("input.xlsx", "output.xlsx");

if (result)
{
    Console.WriteLine("Excel处理成功！");
}
else
{
    Console.WriteLine("Excel处理失败！");
}
```

### 3.2 自定义Excel服务

```csharp
// 自定义Excel服务实现
public class CustomExcelService : IExcelService
{
    private readonly ILogger<CustomExcelService> _logger;
    
    public CustomExcelService(ILogger<CustomExcelService> logger)
    {
        _logger = logger;
    }
    
    public async Task<bool> ProcessExcelAsync(string inputFile, string outputFile)
    {
        try
        {
            // 自定义处理逻辑
            using var workbook = new XLWorkbook(inputFile);
            var worksheet = workbook.Worksheet(1);
            
            // 自定义处理：统计数据
            var rowCount = worksheet.RowsUsed().Count();
            _logger.LogInformation($"文件包含 {rowCount} 行数据");
            
            // 添加统计信息
            var summaryRow = worksheet.Row(rowCount + 2);
            summaryRow.Cell(1).Value = "统计信息";
            summaryRow.Cell(2).Value = $"总记录数: {rowCount}";
            summaryRow.Cell(3).Value = $"处理时间: {DateTime.Now}";
            
            workbook.SaveAs(outputFile);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理Excel文件时发生错误");
            return false;
        }
    }
}

// 注册自定义服务
builder.Services.AddSingleton<IExcelService, CustomExcelService>();
```

## 4. 批量处理示例

### 4.1 批量转换Excel文件

```csharp
// 批量处理多个Excel文件
public async Task BatchProcessExcelFiles(string[] inputFiles, string outputDirectory)
{
    // 创建主机
    var builder = Host.CreateApplicationBuilder();
    builder.Services.AddSingleton<IExcelService, ExcelService>();
    builder.Services.AddSingleton<ClosedXmlAotEngine>();
    
    var host = builder.Build();
    var engine = host.Services.GetRequiredService<ClosedXmlAotEngine>();
    
    // 确保输出目录存在
    Directory.CreateDirectory(outputDirectory);
    
    // 并行处理多个文件
    var tasks = new List<Task>();
    foreach (var inputFile in inputFiles)
    {
        var fileName = Path.GetFileName(inputFile);
        var outputFile = Path.Combine(outputDirectory, fileName);
        
        tasks.Add(Task.Run(async () =>
        {
            var result = await engine.ExecuteAsync(inputFile, outputFile);
            Console.WriteLine($"处理 {inputFile} -> {outputFile}: {(result ? "成功" : "失败")}");
        }));
    }
    
    // 等待所有任务完成
    await Task.WhenAll(tasks);
}
```

### 4.2 大规模数据处理

```csharp
// 处理大规模Excel数据
public async Task ProcessLargeExcel(string inputFile, string outputFile)
{
    try
    {
        using var workbook = new XLWorkbook(inputFile);
        var worksheet = workbook.Worksheet(1);
        
        // 获取数据范围
        var usedRange = worksheet.RangeUsed();
        var rowCount = usedRange.RowCount();
        var colCount = usedRange.ColumnCount();
        
        Console.WriteLine($"开始处理 {rowCount} 行，{colCount} 列数据");
        
        // 启用并行处理
        Parallel.For(1, rowCount + 1, (rowIndex) =>
        {
            var row = worksheet.Row(rowIndex);
            
            // 示例：处理每行数据
            for (int colIndex = 1; colIndex <= colCount; colIndex++)
            {
                var cell = row.Cell(colIndex);
                // 执行数据处理逻辑
            }
        });
        
        workbook.SaveAs(outputFile);
        Console.WriteLine("大规模数据处理完成");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"处理大规模数据时发生错误: {ex.Message}");
    }
}
```

## 5. 命令行工具示例

### 5.1 基本命令行使用

```bash
# 基本Excel处理
closedxml_aot.exe input.xlsx output.xlsx

# 显示帮助信息
closedxml_aot.exe --help

# 显示版本信息
closedxml_aot.exe --version
```

### 5.2 使用配置文件

```bash
# 使用默认配置文件
closedxml_aot.exe input.xlsx output.xlsx

# 使用自定义配置文件
closedxml_aot.exe --setting my_config.json input.xlsx output.xlsx
```

### 5.3 批量处理命令

```bash
# 批量处理当前目录下所有Excel文件
for file in *.xlsx; do
    closedxml_aot.exe "$file" "processed_$file"
done
```

## 6. 性能优化示例

### 6.1 内存优化配置

```json
{
  "ExcelSettings": {
    "MaxRowCount": 500000,
    "MaxColumnCount": 100,
    "EnableMemoryOptimization": true,
    "BufferSize": 131072,
    "TempFileLocation": "./temp"
  },
  "Performance": {
    "EnableCaching": true,
    "CacheSize": 500,
    "CacheDuration": 600
  }
}
```

### 6.2 并行处理配置

```json
{
  "ExcelSettings": {
    "EnableParallelProcessing": true,
    "MaxDegreeOfParallelism": 4
  }
}
```

## 7. 集成示例

### 7.1 与ASP.NET Core集成

```csharp
// ASP.NET Core控制器
[ApiController]
[Route("api/[controller]")]
public class ExcelController : ControllerBase
{
    private readonly IExcelService _excelService;
    
    public ExcelController(IExcelService excelService)
    {
        _excelService = excelService;
    }
    
    [HttpPost("process")]
    public async Task<IActionResult> ProcessExcel(IFormFile file)
    {
        // 保存上传的文件
        var tempInput = Path.GetTempFileName() + ".xlsx";
        var tempOutput = Path.GetTempFileName() + ".xlsx";
        
        using (var stream = new FileStream(tempInput, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        // 处理Excel文件
        var result = await _excelService.ProcessExcelAsync(tempInput, tempOutput);
        
        if (result)
        {
            // 返回处理后的文件
            var fileBytes = await System.IO.File.ReadAllBytesAsync(tempOutput);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "processed.xlsx");
        }
        else
        {
            return BadRequest("Excel处理失败");
        }
    }
}
```

### 7.2 与Worker Service集成

```csharp
// Worker Service
public class ExcelWorker : BackgroundService
{
    private readonly ILogger<ExcelWorker> _logger;
    private readonly IExcelService _excelService;
    
    public ExcelWorker(ILogger<ExcelWorker> logger, IExcelService excelService)
    {
        _logger = logger;
        _excelService = excelService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            
            // 检查是否有待处理的Excel文件
            var inputFiles = Directory.GetFiles("./input", "*.xlsx");
            
            foreach (var inputFile in inputFiles)
            {
                var fileName = Path.GetFileName(inputFile);
                var outputFile = Path.Combine("./output", fileName);
                
                _logger.LogInformation("Processing file: {file}", fileName);
                var result = await _excelService.ProcessExcelAsync(inputFile, outputFile);
                
                if (result)
                {
                    _logger.LogInformation("Processed file: {file}", fileName);
                    // 处理成功后删除原文件
                    System.IO.File.Delete(inputFile);
                }
                else
                {
                    _logger.LogError("Failed to process file: {file}", fileName);
                }
            }
            
            // 每5分钟检查一次
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

## 8. 常见场景示例

### 8.1 数据导入导出

```csharp
// 从数据库导入到Excel
public async Task ExportDataToExcel(string connectionString, string outputFile)
{
    using var workbook = new XLWorkbook();
    var worksheet = workbook.Worksheets.Add("数据导出");
    
    // 从数据库读取数据
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    
    var command = new SqlCommand("SELECT * FROM Users", connection);
    using var reader = await command.ExecuteReaderAsync();
    
    // 写入列名
    for (int i = 0; i < reader.FieldCount; i++)
    {
        worksheet.Cell(1, i + 1).Value = reader.GetName(i);
    }
    
    // 写入数据
    int row = 2;
    while (await reader.ReadAsync())
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            worksheet.Cell(row, i + 1).Value = reader[i];
        }
        row++;
    }
    
    // 保存文件
    workbook.SaveAs(outputFile);
}
```

### 8.2 Excel报表生成

```csharp
// 生成销售报表
public async Task GenerateSalesReport(string inputFile, string outputFile)
{
    using var workbook = new XLWorkbook(inputFile);
    var salesWorksheet = workbook.Worksheet("销售数据");
    var reportWorksheet = workbook.Worksheets.Add("销售报表");
    
    // 设置报表标题
    reportWorksheet.Cell(1, 1).Value = "销售报表";
    reportWorksheet.Cell(1, 1).Style.Font.Size = 16;
    reportWorksheet.Cell(1, 1).Style.Font.Bold = true;
    
    // 计算销售统计
    var totalSales = salesWorksheet.Evaluate<decimal>("SUM(D2:D1000)");
    var avgSales = salesWorksheet.Evaluate<decimal>("AVERAGE(D2:D1000)");
    var maxSales = salesWorksheet.Evaluate<decimal>("MAX(D2:D1000)");
    
    // 写入统计数据
    reportWorksheet.Cell(3, 1).Value = "统计项目";
    reportWorksheet.Cell(3, 2).Value = "数值";
    
    reportWorksheet.Cell(4, 1).Value = "总销售额";
    reportWorksheet.Cell(4, 2).Value = totalSales;
    
    reportWorksheet.Cell(5, 1).Value = "平均销售额";
    reportWorksheet.Cell(5, 2).Value = avgSales;
    
    reportWorksheet.Cell(6, 1).Value = "最高销售额";
    reportWorksheet.Cell(6, 2).Value = maxSales;
    
    // 设置数字格式
    reportWorksheet.Range("B4:B6").Style.NumberFormat.Format = "¥#,##0.00";
    
    workbook.SaveAs(outputFile);
}
```