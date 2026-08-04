# GoodWords - 参考文档

## 概述

GoodWords 是基于 .NET 10 的高性能好词好句系统，专为 .NET 开发者设计。支持 AOT 编译，提供单文件执行脚本，具有高性能、低内存占用的特点。

## 核心组件

### 1. GoodWordsService (GoodWords 服务)
- **位置**: scripts/goodWords_aot.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 好词好句生成
  - 好词好句分类
  - 好词好句搜索
  - 类别管理
  - 性能优化
  - 错误处理
  - 日志记录

### 2. GoodWordsAotEngine (GoodWords AOT 引擎)
- **位置**: scripts/goodWords_aot.cs
- **功能**: 命令行引擎，处理命令行参数和执行命令
- **特性**: 
  - 命令行参数解析
  - 别名支持
  - 结果输出格式化
  - 错误处理

## 使用示例

### 基本使用

```csharp
// 获取 GoodWords 服务
var goodWordsService = serviceProvider.GetRequiredService<IGoodWordsService>();

// 生成好词好句
var generateResult = await goodWordsService.GenerateAsync("love", 5, "zh");
Console.WriteLine($"生成结果: {(generateResult.Success ? "成功" : "失败")}");

// 分类好词好句
var classifyResult = await goodWordsService.ClassifyAsync("生活是美好的");
Console.WriteLine($"分类结果: {(classifyResult.Success ? "成功" : "失败")}");

// 搜索好词好句
var searchResult = await goodWordsService.SearchAsync("爱情");
Console.WriteLine($"搜索结果: {(searchResult.Success ? "成功" : "失败")}");

// 列出类别
var listCategoriesResult = await goodWordsService.ListCategoriesAsync();
Console.WriteLine($"列出类别结果: {(listCategoriesResult.Success ? "成功" : "失败")}");
```

### 高级配置

```csharp
// 配置 GoodWords 选项
var goodWordsOptions = new GoodWordsOptions
{
    DefaultCategory = "general",
    DefaultLanguage = "zh",
    EnableCache = true,
    CacheSize = 1000,
    RequestTimeoutMs = 30000,
    EnableDetailedLogging = false,
    EnablePerformanceMonitoring = true
};

builder.Services.Configure<GoodWordsOptions>(options => {
    options.DefaultCategory = goodWordsOptions.DefaultCategory;
    options.DefaultLanguage = goodWordsOptions.DefaultLanguage;
    options.EnableCache = goodWordsOptions.EnableCache;
    options.CacheSize = goodWordsOptions.CacheSize;
    options.RequestTimeoutMs = goodWordsOptions.RequestTimeoutMs;
    options.EnableDetailedLogging = goodWordsOptions.EnableDetailedLogging;
    options.EnablePerformanceMonitoring = goodWordsOptions.EnablePerformanceMonitoring;
});
```

## 配置选项

### GoodWords 配置

```json
{
  "GoodWords": {
    "DefaultCategory": "general",
    "DefaultLanguage": "zh",
    "EnableCache": true,
    "CacheSize": 1000,
    "RequestTimeoutMs": 30000,
    "EnableDetailedLogging": false,
    "EnablePerformanceMonitoring": true,
    "WorkingDirectory": "d:\\Trae\\vsa\\skills\\goodWords"
  }
}
```

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **AOT 编译**: 使用 AOT 编译提高启动速度和运行性能
5. **单文件部署**: 减少依赖，提高部署效率

## 故障排除

### 常见问题

1. **生成失败**
   - 检查类别是否存在
   - 验证参数是否正确
   - 检查日志信息

2. **分类失败**
   - 检查输入内容是否有效
   - 验证参数是否正确
   - 检查日志信息

3. **搜索失败**
   - 检查关键词是否正确
   - 验证参数是否正确
   - 检查日志信息

4. **AOT 编译问题**
   - 确保使用 .NET 10
   - 检查依赖项是否支持 AOT
   - 查看编译错误信息

## 扩展开发

### 添加自定义功能

```csharp
public class CustomGoodWordsService : IGoodWordsService
{
    private readonly GoodWordsOptions _options;
    private readonly ILogger<CustomGoodWordsService> _logger;

    public CustomGoodWordsService(IOptions<GoodWordsOptions> options, ILogger<CustomGoodWordsService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    // 实现接口方法...
    public async Task<GoodWordsCommandResult> GenerateAsync(string category = "general", int count = 5, string language = "zh")
    {
        // 自定义实现
        var result = new GoodWordsCommandResult();
        // 实现逻辑
        return result;
    }

    // 实现其他接口方法...
}
```

### 注册自定义服务

```csharp
// 注册自定义 GoodWords 服务
builder.Services.Configure<GoodWordsOptions>(builder.Configuration.GetSection("GoodWords"));
builder.Services.AddSingleton<IGoodWordsService, CustomGoodWordsService>();
builder.Services.AddSingleton<GoodWordsAotEngine>();
```
