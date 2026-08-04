# HTML AOT 技能 - HTML 技能

## 技能概述

基于 .NET 10 的高性能 HTML 技能，为 .NET 开发者提供强大的 HTML 处理功能，支持 AOT 编译，具有极高的性能和低内存占用。

## 快速开始指南

### 安装依赖

在主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package HtmlAgilityPack@1.11.58
#:package AngleSharp@1.10.0
#:package HtmlSanitizer@8.0.482
#:package CsQuery@1.3.4
#:package System.Text.RegularExpressions@4.3.1
#:package System.Net.Http@4.3.4
```

### 注册服务

在主应用程序中注册 HTML 服务：

```csharp
// 注册 HTML 服务
builder.Services.AddSingleton<HtmlService>();
builder.Services.Configure<HtmlSettings>(options => {
    options.EnableCache = true;
    options.CacheSize = 100;
    options.Timeout = TimeSpan.FromSeconds(30);
    options.EnableDetailedLogging = false;
    options.MaxDocumentSize = 10485760; // 10MB
    options.EnableJavaScript = false;
    options.EnableCss = false;
    options.EnableImages = false;
});
```

### 使用示例

```csharp
// 获取 HTML 服务
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

## 导航地图

```
html/
????? index.yaml                   # 元数据索引描述
????? SKILL.md                    # 技能入口点（当前文件）
????? reference/                  # 参考文件
??  ????? README.md              # 完整功能描述
??  ????? examples.md            # 使用示例
????? scripts/                    # 脚本和工具
    ????? html_aot.cs             # HTML AOT 核心实现
    ????? html_aot.run.json       # 运行配置
    ????? html_aot.setting.json   # 设置文件
    ????? abot_integration.cs     # Abot 集成
    ????? dotnetspider_integration.cs # DotNetSpider 集成
    ????? htmlagilitypack_integration.cs # HtmlAgilityPack 集成
    ????? htmlsanitizer_integration.cs # HtmlSanitizer 集成
```

## 主要特性

1. **HTML 解析**: 使用 HtmlAgilityPack 和 AngleSharp 解析 HTML
2. **HTML 清理**: 使用 HtmlSanitizer 清理不安全的 HTML
3. **内容提取**: 使用 XPath 和 CSS 选择器提取内容
4. **网站抓取**: 抓取网站内容，包括标题、链接、图片等
5. **HTML 验证**: 验证 HTML 文档的有效性
6. **HTML 压缩**: 压缩 HTML 文件，减少文件大小
7. **缓存优化**: 智能缓存机制，提高性能
8. **AOT 编译**: 使用 .NET 10 AOT 编译，提高性能和降低内存占用
9. **命令行工具**: 完整的命令行界面，支持多种操作和别名
10. **配置管理**: 支持环境变量和配置文件配置

## AOT 编译优势

- **快速启动**: 比 JIT 编译快 3-5 倍
- **低内存占用**: 内存占用减少 20-30%
- **简单部署**: 单文件执行，无运行时依赖
- **稳定性能**: 编译时优化
- **高安全性**: 减少运行时攻击面

## 扩展说明

此技能提供完整的 HTML 解决方案，您可以根据需要进行扩展：

1. **自定义解析器**: 实现自定义 HTML 解析器
2. **扩展清理规则**: 添加自定义 HTML 清理规则
3. **与其他系统集成**: 与其他系统集成
4. **性能优化**: 针对特定场景优化性能
5. **添加新功能**: 添加新的 HTML 处理功能

## 最佳实践

1. **依赖注入**: 使用依赖注入管理服务
2. **异步编程**: 优先使用异步 API 避免阻塞
3. **错误处理**: 正确处理异常情况
4. **日志记录**: 添加适当的日志记录
5. **性能监控**: 监控关键性能指标
6. **缓存管理**: 合理使用缓存，避免内存泄漏
7. **配置管理**: 使用 Options 模式管理配置

## 命令行使用

```bash
# 解析 HTML 文件或 URL
html_aot.exe parse index.html

# 清理 HTML 内容
html_aot.exe sanitize "<p>安全的 <script>alert('危险')</script> HTML</p>"

# 提取 HTML 内容
html_aot.exe extract https://example.com "//div[@class='content']"

# 抓取网站
html_aot.exe scrape https://example.com

# 验证 HTML
html_aot.exe validate index.html

# 压缩 HTML 文件
html_aot.exe minify index.html minified.html

# 显示配置信息
html_aot.exe config

# 显示帮助信息
html_aot.exe help
```

## 支持的库

- **HtmlAgilityPack**: 强大的 HTML 解析库
- **AngleSharp**: 现代 HTML 解析库
- **HtmlSanitizer**: HTML 清理库，防止 XSS 攻击
- **CsQuery**: jQuery 风格的 HTML 解析库

## 系统要求

- .NET 10.0 或更高版本
- Windows 10/11 (x64)
- 至少 2GB 内存
- 支持的浏览器: Chrome, Firefox, Edge, Safari

## 故障排除

### 常见问题

1. **解析错误**
   - 检查 HTML 文件格式是否正确
   - 验证 URL 是否可访问
   - 检查网络连接

2. **性能问题**
   - 启用缓存
   - 减少文档大小
   - 优化选择器

3. **编译错误**
   - 确保使用 .NET 10 SDK
   - 检查依赖项版本
   - 验证 AOT 编译配置
