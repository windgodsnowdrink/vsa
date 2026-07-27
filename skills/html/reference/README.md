# HTML - 参考文档

## 概述

HTML 是基于 .NET 10 的高性能 HTML 处理系统，专为 .NET 开发者设计，支持 AOT 编译，提供单文件执行脚本，具有极高的性能和低内存占用。

## 核心组件

### 1. HTML AOT 引擎
- **位置**: scripts/html_aot.cs
- **功能**: 核心 HTML 处理业务逻辑，包括解析、清理、内容提取、网站抓取、验证和压缩
- **特性**: 
  - HTML 解析：使用 HtmlAgilityPack 和 AngleSharp 解析 HTML
  - HTML 清理：使用 HtmlSanitizer 清理不安全的 HTML
  - 内容提取：使用 XPath 和 CSS 选择器提取内容
  - 网站抓取：抓取网站内容，包括标题、链接、图片等
  - HTML 验证：验证 HTML 文档的有效性
  - HTML 压缩：压缩 HTML 文件，减少文件大小
  - 缓存优化：智能缓存机制，提高性能
  - AOT 编译优化：使用 .NET 10 AOT 编译，提高性能和降低内存占用
  - 命令行工具：完整的命令行界面，支持多种操作和别名
  - 配置管理：支持环境变量和配置文件配置

## 使用示例

### 基本用法

```csharp
// 初始化服务
var serviceProvider = BuildServiceProvider();
var htmlService = serviceProvider.GetRequiredService<HtmlService>();

// 解析 HTML
var parseResult = await htmlService.ParseHtmlAsync("index.html");
Console.WriteLine($"标题: {parseResult.Title}");
Console.WriteLine($"链接数: {parseResult.Links.Count}");
Console.WriteLine($"图片数: {parseResult.Images.Count}");

// 清理 HTML
var sanitizedHtml = await htmlService.SanitizeHtmlAsync("<p>安全的 <script>alert('危险')</script> HTML</p>");
Console.WriteLine($"清理后的 HTML: {sanitizedHtml}");

// 提取内容
var content = await htmlService.ExtractContentAsync("https://example.com", "//div[@class='content']");
Console.WriteLine($"提取的内容: {content}");

// 抓取网站
var scrapeResult = await htmlService.ScrapeWebsiteAsync("https://example.com");
Console.WriteLine($"网站标题: {scrapeResult.Title}");
Console.WriteLine($"元描述: {scrapeResult.MetaDescription}");

// 验证 HTML
var validateResult = await htmlService.ValidateHtmlAsync("index.html");
Console.WriteLine($"HTML 是否有效: {validateResult.IsValid}");

// 压缩 HTML
var minifiedHtml = await htmlService.MinifyHtmlAsync("index.html");
Console.WriteLine($"压缩后的 HTML: {minifiedHtml}");

// 释放资源
await htmlService.DisposeAsync();
```

### 高级配置

```csharp
var htmlSettings = new HtmlSettings {
    EnableCache = true,
    CacheSize = 100,
    Timeout = TimeSpan.FromSeconds(30),
    EnableDetailedLogging = false,
    MaxDocumentSize = 10485760, // 10MB
    EnableJavaScript = false,
    EnableCss = false,
    EnableImages = false
};

builder.Services.Configure<HtmlSettings>(options => {
    options.EnableCache = htmlSettings.EnableCache;
    options.CacheSize = htmlSettings.CacheSize;
    options.Timeout = htmlSettings.Timeout;
    options.EnableDetailedLogging = htmlSettings.EnableDetailedLogging;
    options.MaxDocumentSize = htmlSettings.MaxDocumentSize;
    options.EnableJavaScript = htmlSettings.EnableJavaScript;
    options.EnableCss = htmlSettings.EnableCss;
    options.EnableImages = htmlSettings.EnableImages;
});
```

## 配置选项

### HTML 配置

```json
{
  "HtmlSettings": {
    "EnableCache": true,              // 启用缓存
    "CacheSize": 100,                // 缓存大小
    "Timeout": "00:00:30",          // 超时
    "EnableDetailedLogging": false,   // 启用详细日志
    "MaxDocumentSize": 10485760,      // 最大文档大小（字节）
    "EnableJavaScript": false,        // 启用 JavaScript
    "EnableCss": false,              // 启用 CSS
    "EnableImages": false            // 启用图片
  }
}
```

## 性能优化

1. **AOT 编译**: 使用 .NET 10 AOT 编译，提高性能和降低内存占用
2. **缓存使用**: 启用缓存以提高性能
3. **异步编程**: 使用异步 API 避免阻塞
4. **批处理**: 批量处理提高效率
5. **连接池**: 使用连接池管理网络资源
6. **选择器优化**: 优化 XPath 和 CSS 选择器
7. **文档大小限制**: 限制文档大小以提高性能

## 故障排除

### 常见问题

1. **解析错误**
   - 检查 HTML 文件格式是否正确
   - 验证 URL 是否可访问
   - 检查网络连接
   - 查看日志信息

2. **性能问题**
   - 启用缓存
   - 优化选择器
   - 减少文档大小
   - 增加资源限制

3. **编译错误**
   - 确保使用 .NET 10 SDK
   - 检查依赖项版本
   - 验证 AOT 编译配置

4. **网络问题**
   - 检查网络连接
   - 验证代理设置
   - 调整超时设置

## 扩展开发

### 添加自定义解析器

```csharp
public interface IHtmlParser
{
    HtmlParseResult Parse(string html);
}

public class CustomHtmlParser : IHtmlParser
{
    public HtmlParseResult Parse(string html)
    {
        var result = new HtmlParseResult { Html = html };
        // 自定义解析逻辑
        return result;
    }
}

// 注册服务
builder.Services.AddSingleton<IHtmlParser, CustomHtmlParser>();
```

### 添加自定义清理规则

```csharp
public class CustomHtmlSanitizer
{
    public string Sanitize(string html)
    {
        var sanitizer = new HtmlSanitizer();
        // 添加自定义规则
        sanitizer.AllowedTags.Add("custom-tag");
        sanitizer.AllowedAttributes.Add("data-custom");
        return sanitizer.Sanitize(html);
    }
}

// 注册服务
builder.Services.AddSingleton<CustomHtmlSanitizer>();
```

## AOT 编译指南

### 编译命令

```bash
# 编译 AOT 版本
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial

# 编译 Linux 版本
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial

# 编译 macOS 版本
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishAot=true /p:TrimMode=partial
```

### 编译选项

- **Target Framework**: net11.0
- **Runtime Identifier**: win-x64, linux-x64, osx-x64
- **Trim Mode**: partial
- **Publish Aot**: true
- **Optimize**: true
- **Debug Type**: none

### 部署指南

1. **准备环境**: 无需安装 .NET 运行时
2. **复制文件**: 将编译后的单文件可执行文件复制到目标机器
3. **配置环境**: 设置必要的环境变量
4. **启动服务**: 运行可执行文件

### 监控和维护

1. **性能监控**: 定期检查解析速度和内存使用
2. **日志分析**: 分析日志文件，识别潜在问题
3. **缓存管理**: 监控缓存使用情况，避免内存泄漏
4. **定期更新**: 及时更新到最新版本，获取性能改进

