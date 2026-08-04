# localization - 参考文档

## 概述

localization 是一个基于 .NET 10 的高性能本地化系统，专为 .NET 开发者设计。

## 核心组件

### 1. LocalizationProcessor（本地化处理器）
- **位置**: scripts/localization_integration.cs
- **功能**: 处理本地化资源和翻译
- **特性**: 
  - 高性能资源处理
  - Threading.Channels 事件处理
  - Span 零拷贝优化
  - 多语言支持

### 2. LocalizationService（本地化服务）
- **位置**: scripts/localization_integration.cs
- **功能**: 核心业务逻辑处理
- **特性**: 
  - 资源管理
  - 动态翻译
  - 错误处理
  - 日志记录

## 使用示例

### 基本使用

`csharp
var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
var localizedText = await localizationService.GetLocalizedTextAsync("Hello", "zh-CN");
Console.WriteLine($"本地化文本: {localizedText}");
`

### 高级配置

`csharp
var settings = new LocalizationSetting {
    EnableCache = true,
    CacheSize = 1000,
    DefaultCulture = "en-US",
    SupportedCultures = new[] { "en-US", "zh-CN", "ja-JP" }
};

builder.Services.Configure<LocalizationSetting>(options => {
    options.EnableCache = settings.EnableCache;
    options.CacheSize = settings.CacheSize;
    options.DefaultCulture = settings.DefaultCulture;
    options.SupportedCultures = settings.SupportedCultures;
});
`

## 配置选项

### Localization 配置

`json
{
  "LocalizationSetting": {
    "EnableCache": true,          // 启用缓存
    "CacheSize": 1000,            // 缓存大小
    "DefaultCulture": "en-US",        // 默认文化
    "SupportedCultures": ["en-US", "zh-CN", "ja-JP"],        // 支持的文化
    "EnableDetailedLogging": false          // 启用详细日志
  }
}
`

## 性能优化

1. **缓存使用**: 启用缓存以提高性能
2. **异步编程**: 使用异步 API 避免阻塞
3. **批处理**: 批量处理以提高效率
4. **连接池**: 使用连接池管理资源
5. **Channel 事件处理**: 使用 Threading.Channels 实现高效的事件队列
6. **Span 零拷贝**: 使用 Span 减少内存分配和复制
7. **对象池**: 使用 ObjectPool 减少对象创建开销

## AOT 编译配置

### 构建配置

```yaml
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Localization@10.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
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
```

### 发布命令

```bash
# AOT 发布命令
dotnet publish scripts/localization_integration.cs -c Release -r win-x64 --aot

# Linux 发布命令
dotnet publish scripts/localization_integration.cs -c Release -r linux-x64 --aot

# macOS 发布命令
dotnet publish scripts/localization_integration.cs -c Release -r osx-x64 --aot
```

## 故障排除

### 常见问题

1. **资源加载失败**
   - 检查配置文件
   - 验证资源文件存在
   - 检查日志信息

2. **性能问题**
   - 启用缓存
   - 优化资源管理
   - 增加资源限制
   - 检查 Channel 配置

3. **内存问题**
   - 调整缓存大小
   - 优化内存使用
   - 检查 Span 使用

## 扩展开发

### 添加自定义功能

`csharp
public class CustomLocalizationService : ILocalizationService
{
    public async Task<string> GetLocalizedTextAsync(string key, string culture)
    {
        // 实现自定义逻辑
        return $"Custom localized text for {key} in {culture}";
    }
    
    public async Task<string> GetLocalizedTextAsync(string key, string culture, params object[] args)
    {
        // 实现自定义逻辑
        return $"Custom localized text for {key} in {culture} with args";
    }
}
`

### 扩展处理器

`csharp
public class CustomLocalizationProcessor : ILocalizationProcessor
{
    public async Task<string> ProcessAsync(string key, string culture, LocalizationSetting settings)
    {
        // 实现自定义处理逻辑
        return $"Processed localization for {key} in {culture}";
    }
}
`
