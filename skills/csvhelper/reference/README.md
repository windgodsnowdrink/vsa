# CsvHelper AOT - 参考文档

## 概述

CsvHelper AOT是基于.NET 10 AOT架构的高性能CSV处理工具，专为.NET开发者设计，提供高效、可靠的CSV导出和导入功能。

## 核心组件

### 1. ICsvHelperService 接口
- **位置**: scripts/csvhelper_aot.cs
- **功能**: 定义CSV处理的核心功能接口
- **方法**:
  - `ExportToCsvStringAsync`: 将对象列表导出为CSV字符串
  - `ExportToCsvFileAsync`: 将对象列表导出为CSV文件
  - `ImportFromCsvStringAsync`: 从CSV字符串导入对象列表
  - `ImportFromCsvFileAsync`: 从CSV文件导入对象列表
  - `ExportBatchAsync`: 批量导出多个CSV文件
  - `GetStatusAsync`: 获取服务状态
  - `ResetStatusAsync`: 重置服务状态

### 2. CsvHelperService 实现
- **位置**: scripts/csvhelper_aot.cs
- **功能**: 实现ICsvHelperService接口，提供CSV处理的核心功能
- **特性**:
  - 支持AOT编译，提供原生性能
  - 内置缓存机制，提高重复执行速度
  - 支持异步编程，提高并发性能
  - 详细的日志记录，便于调试和监控
  - 支持批量处理，提高效率

### 3. CsvHelperAotEngine 执行引擎
- **位置**: scripts/csvhelper_aot.cs
- **功能**: 管理CSV处理的执行引擎
- **特性**:
  - 封装ICsvHelperService的调用
  - 提供更高级别的CSV处理接口
  - 支持命令行交互

### 4. CsvHelperOptions 配置选项
- **位置**: scripts/csvhelper_aot.cs
- **功能**: 配置选项类，用于控制CSV处理的行为
- **配置项**:
  - `EnableCache`: 是否启用缓存
  - `CacheSize`: 缓存大小
  - `Timeout`: 操作超时时间
  - `EnableDetailedLogging`: 是否启用详细日志
  - `WorkerCount`: 工作线程数
  - `RetryCount`: 重试次数
  - `RetryInterval`: 重试间隔
  - `DefaultEncoding`: 默认字符编码
  - `DefaultDelimiter`: 默认分隔符
  - `HasHeaderRecord`: 是否包含标题行
  - `IgnoreBlankLines`: 是否忽略空行
  - `SkipFirstRecord`: 是否跳过首行

### 5. 结果类
- **位置**: scripts/csvhelper_aot.cs
- **类**:
  - `CsvHelperResult`: CSV处理结果类
  - `CsvHelperResult<T>`: 带泛型结果的CSV处理结果类
  - `CsvHelperStatus`: 服务状态信息类

## 使用示例

### 基本用法

```csharp
// 获取CsvHelper AOT引擎
var engine = serviceProvider.GetRequiredService<CsvHelper.AOT.CsvHelperAotEngine>();

// 创建测试数据
var records = new List<Product>
{
    new Product { Id = 1, Name = "产品1", Price = 100.00, Category = "分类1" },
    new Product { Id = 2, Name = "产品2", Price = 200.00, Category = "分类2" },
    new Product { Id = 3, Name = "产品3", Price = 300.00, Category = "分类1" }
};

// 导出为CSV字符串
var csvString = await engine.ExportToCsvStringAsync(records);
Console.WriteLine("导出CSV字符串成功");
Console.WriteLine(csvString);

// 导出为CSV文件
var result = await engine.ExportToCsvFileAsync(records, "products.csv");
Console.WriteLine($"导出CSV文件{result.Success ? "成功" : "失败"}，路径: products.csv，记录数: {result.RecordCount}");
```

### 高级配置

```csharp
// 从配置文件加载配置
builder.Configuration.AddJsonFile("csvhelper_aot.setting.json");
builder.Services.Configure<CsvHelper.AOT.CsvHelperOptions>(builder.Configuration.GetSection("CsvHelper"));

// 或者直接在代码中配置
builder.Services.Configure<CsvHelper.AOT.CsvHelperOptions>(options => {
    options.EnableCache = true;
    options.CacheSize = 2000;
    options.DefaultDelimiter = ';';
    options.HasHeaderRecord = true;
    options.IgnoreBlankLines = true;
    options.WorkerCount = Environment.ProcessorCount;
});
```

### 批量导出

```csharp
// 准备批量导出请求
var exportRequests = new List<CsvHelper.AOT.CsvExportRequest<Product>>();

exportRequests.Add(new CsvHelper.AOT.CsvExportRequest<Product>
{
    RequestId = "Export-1",
    Records = productList1,
    FilePath = "products1.csv"
});

exportRequests.Add(new CsvHelper.AOT.CsvExportRequest<Product>
{
    RequestId = "Export-2",
    Records = productList2,
    FilePath = "products2.csv"
});

// 批量导出
var results = await engine.ExportBatchAsync(exportRequests);

// 处理结果
foreach (var result in results)
{
    Console.WriteLine($"请求处理{result.Success ? "成功" : "失败"}，记录数: {result.RecordCount}");
}
```

### 自定义CSV配置

```csharp
// 创建自定义CSV配置
var configuration = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
{
    Delimiter = ";",
    HasHeaderRecord = true,
    IgnoreBlankLines = true,
    SkipEmptyRecords = true,
    TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
    MissingFieldFound = null,
    HeaderValidated = null
};

// 使用自定义配置导出
var csvString = await engine.ExportToCsvStringAsync(records, configuration);
```

## 配置选项

### 配置文件格式

```json
{
  "CsvHelper": {
    "EnableCache": true,
    "CacheSize": 1000,
    "Timeout": "00:00:30",
    "EnableDetailedLogging": false,
    "WorkerCount": 4,
    "RetryCount": 3,
    "RetryInterval": "00:00:00.5",
    "DefaultEncoding": "utf-8",
    "DefaultDelimiter": ",",
    "HasHeaderRecord": true,
    "IgnoreBlankLines": true,
    "SkipFirstRecord": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "CsvHelper.AOT": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

### 环境变量配置

CsvHelper AOT支持通过环境变量进行配置，环境变量名称为配置项的大写形式，前缀为`CSVHELPER_`：

- `CSVHELPER_ENABLE_CACHE`: 是否启用缓存
- `CSVHELPER_CACHE_SIZE`: 缓存大小
- `CSVHELPER_DEFAULT_DELIMITER`: 默认分隔符
- `CSVHELPER_HAS_HEADER_RECORD`: 是否包含标题行
- `CSVHELPER_IGNORE_BLANK_LINES`: 是否忽略空行

## 性能优化

1. **启用AOT编译**: 生产环境建议使用AOT编译，获得最佳性能
2. **合理配置缓存**: 根据实际需求配置缓存大小和启用/禁用缓存
3. **使用异步API**: 优先使用异步API，提高并发性能
4. **合理配置工作线程数**: 根据CPU核心数配置合适的工作线程数
5. **使用批量处理**: 对于多个CSV导出，使用批量处理提高效率
6. **优化记录映射**: 合理配置记录映射，避免不必要的映射
7. **禁用不必要的验证**: 在生产环境中，禁用不必要的验证可以提高性能
8. **使用合适的分隔符**: 根据数据特点选择合适的分隔符

## 故障排除

### 常见问题

1. **导出失败**
   - 检查文件路径是否正确
   - 检查文件权限是否足够
   - 查看日志信息，了解具体错误原因
   - 检查系统资源是否充足

2. **导出结果不符合预期**
   - 检查记录映射是否正确
   - 检查CSV配置是否合适
   - 检查数据类型是否匹配
   - 尝试使用自定义映射

3. **性能问题**
   - 启用缓存
   - 调整工作线程数
   - 减少重试次数
   - 优化记录数量，分批次导出
   - 启用AOT编译

4. **缓存命中率低**
   - 增加缓存大小
   - 确保相同的数据多次导出
   - 确保EnableCache设置为true
   - 检查缓存键生成逻辑是否合理

5. **内存占用高**
   - 减少缓存大小
   - 禁用缓存
   - 减少工作线程数
   - 分批次处理大量数据
   - 优化记录映射

## 扩展开发

### 自定义记录映射

```csharp
// 自定义记录映射
public class CustomProductMap : CsvHelper.Configuration.ClassMap<Product>
{
    public CustomProductMap()
    {
        Map(m => m.Id).Name("产品ID");
        Map(m => m.Name).Name("产品名称");
        Map(m => m.Price).Name("价格").Format("C2");
        Map(m => m.Category).Name("分类");
        Map(m => m.CreatedAt).Name("创建时间").Format("yyyy-MM-dd HH:mm:ss");
    }
}
```

### 自定义CsvHelper服务

```csharp
// 自定义CsvHelper服务实现
public class CustomCsvHelperService : CsvHelper.AOT.CsvHelperService
{
    public CustomCsvHelperService(ILogger<CsvHelperService> logger, IOptions<CsvHelper.AOT.CsvHelperOptions> options)
        : base(logger, options)
    {
    }
    
    // 重写ExportToCsvStringAsync方法，添加自定义处理逻辑
    public override async Task<string> ExportToCsvStringAsync<T>(IEnumerable<T> records, CsvHelper.Configuration.CsvConfiguration? configuration = null) where T : class
    {
        // 自定义预处理逻辑
        _logger.LogInformation("自定义CSV导出处理开始");
        
        // 调用基类方法执行导出
        var result = await base.ExportToCsvStringAsync(records, configuration);
        
        // 自定义后处理逻辑
        _logger.LogInformation("自定义CSV导出处理完成");
        
        return result;
    }
}

// 注册自定义服务
builder.Services.AddSingleton<CsvHelper.AOT.ICsvHelperService, CustomCsvHelperService>();
```

## 部署建议

1. **使用AOT编译**: 生产环境建议使用AOT编译，获得最佳性能
2. **配置环境变量**: 使用环境变量进行配置，便于在不同环境中部署
3. **监控服务状态**: 定期获取服务状态，监控系统运行情况
4. **配置日志轮换**: 配置日志轮换，防止日志文件过大
5. **使用容器化部署**: 考虑使用Docker等容器技术进行部署，便于管理和扩展
6. **配置资源限制**: 为服务配置适当的资源限制，防止资源耗尽
7. **启用监控**: 启用应用性能监控，便于发现和解决性能问题

## 版本兼容性

- **.NET版本**: .NET 10.0
- **操作系统**: Windows, Linux, macOS
- **架构**: x86, x64, Arm64

## 依赖项

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Microsoft.Extensions.DependencyInjection | 10.0.0 | 依赖注入容器 |
| Microsoft.Extensions.Logging | 10.0.0 | 日志记录 |
| Microsoft.Extensions.Hosting | 10.0.0 | 主机管理 |
| Microsoft.Extensions.Options | 10.0.0 | 配置选项 |
| Newtonsoft.Json | 13.0.3 | JSON序列化和反序列化 |
| CsvHelper | 32.0.1 | CSV处理库 |
| CsvHelper.Schema | 32.0.1 | CsvHelper模式支持 |