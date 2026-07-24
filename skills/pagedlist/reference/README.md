# PagedList - 参考文档

## 概述

PagedList 是一个基于 .NET 10 的高性能分页解决方案，专为 .NET 开发者设计。它提供了强大的分页功能，支持异步操作、内存优化、缓存机制等特性，适用于各种分页场景，如数据查询、API 响应格式化、数据展示等。

## 核心组件

### 1. PagedList 服务
- **位置**: scripts/x_pagedlist_integration.cs
- **功能**: 核心分页业务逻辑处理
- **特性**:
  - 高性能分页查询实现
  - 异步分页操作
  - 内存优化（零拷贝、内存池等）
  - 缓存支持（全局缓存和线程本地缓存）
  - 错误处理与重试机制
  - 性能监控与指标
  - 支持多种数据源（IQueryable、IEnumerable）

### 2. 配置管理
- **位置**: scripts/x_pagedlist_integration.setting.json
- **功能**: 管理 PagedList 的配置选项
- **特性**:
  - 可配置的分页大小
  - 缓存配置
  - 性能优化选项
  - 日志配置
  - AOT 编译配置

### 3. 运行环境
- **位置**: scripts/x_pagedlist_integration.run.json
- **功能**: 管理 PagedList 的运行时环境
- **特性**:
  - 运行时选项配置
  - 编译选项配置
  - 依赖项管理
  - 构建选项配置
  - 发布选项配置
  - 运行时标识符配置

## 快速开始

### 1. 安装依赖

在你的主应用程序的运行文件中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package X.PagedList@8.4.0
#:package X.PagedList.Mvc.Core@8.4.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property TrimMode=partial
#:property Optimize=true
```

### 2. 注册服务

在你的主应用程序中注册 PagedList 服务：

```csharp
// 注册 PagedList 服务
builder.Services.AddPagedListServices(options =>
{
    options.DefaultPageSize = 20;
    options.MaxPageSize = 100;
    options.EnableCache = true;
    options.CacheDuration = TimeSpan.FromSeconds(300);
    options.EnablePerformanceMetrics = true;
    options.EnableZeroCopy = true;
    options.ThreadLocalCacheSize = 1024;
    options.EnableParallelProcessing = true;
    options.MaxDegreeOfParallelism = Environment.ProcessorCount;
});
```

### 3. 基本使用

```csharp
// 获取 PagedList 服务
var pagedListService = serviceProvider.GetRequiredService<IPagedListService>();

// 准备数据源
var source = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" }).AsQueryable();

// 获取分页数据
var pageNumber = 1;
var pageSize = 20;
var pagedList = await pagedListService.GetPagedListAsync(source, pageNumber, pageSize);

// 使用分页结果
Console.WriteLine($"总记录数: {pagedList.TotalItemCount}");
Console.WriteLine($"总页数: {pagedList.PageCount}");
Console.WriteLine($"当前页: {pagedList.PageNumber}");
Console.WriteLine($"每页大小: {pagedList.PageSize}");

// 遍历当前页数据
foreach (var item in pagedList)
{
    Console.WriteLine($"{item.Id}: {item.Name}");
}
```

## 高级使用

### 1. 从 IEnumerable 获取分页数据

```csharp
// 准备 IEnumerable 数据源
var enumerableSource = Enumerable.Range(1, 1000).Select(i => new { Id = i, Name = $"Item {i}" });

// 获取分页数据
var pagedList = await pagedListService.GetPagedListAsync(enumerableSource, pageNumber, pageSize);
```

### 2. 获取分页结果模型

```csharp
// 获取分页结果模型
var pagedResult = await pagedListService.GetPagedResultAsync(source, pageNumber, pageSize);

// 使用分页结果和配置选项
Console.WriteLine($"总记录数: {pagedResult.Items.TotalItemCount}");
Console.WriteLine($"默认页大小: {pagedResult.Options.DefaultPageSize}");
Console.WriteLine($"最大页大小: {pagedResult.Options.MaxPageSize}");
```

### 3. 自定义配置

```csharp
// 自定义 PagedList 配置
var pagedListOptions = new PagedListOptions
{
    DefaultPageSize = 50,
    MaxPageSize = 200,
    EnableCache = true,
    CacheDuration = TimeSpan.FromMinutes(10),
    EnablePerformanceMetrics = true,
    EnableZeroCopy = true,
    ThreadLocalCacheSize = 2048,
    EnableParallelProcessing = true,
    MaxDegreeOfParallelism = Environment.ProcessorCount * 2
};

// 注册服务时使用自定义配置
builder.Services.AddPagedListServices(options =>
{
    options.DefaultPageSize = pagedListOptions.DefaultPageSize;
    options.MaxPageSize = pagedListOptions.MaxPageSize;
    options.EnableCache = pagedListOptions.EnableCache;
    options.CacheDuration = pagedListOptions.CacheDuration;
    options.EnablePerformanceMetrics = pagedListOptions.EnablePerformanceMetrics;
    options.EnableZeroCopy = pagedListOptions.EnableZeroCopy;
    options.ThreadLocalCacheSize = pagedListOptions.ThreadLocalCacheSize;
    options.EnableParallelProcessing = pagedListOptions.EnableParallelProcessing;
    options.MaxDegreeOfParallelism = pagedListOptions.MaxDegreeOfParallelism;
});
```

## 配置选项

### 1. 基本配置

```json
{
  "PagedListOptions": {
    "DefaultPageSize": 20,            // 默认分页大小
    "MaxPageSize": 100,              // 最大分页大小
    "CacheDuration": "00:05:00",     // 缓存持续时间
    "EnableCache": true,             // 是否启用缓存
    "EnablePerformanceMetrics": true, // 是否启用性能指标监控
    "EnableZeroCopy": true,           // 是否启用零拷贝优化
    "ThreadLocalCacheSize": 1024,     // 线程本地缓存大小
    "CacheLineSize": 64,              // CPU 缓存行对齐大小
    "EnableParallelProcessing": true,  // 是否启用并行处理
    "MaxDegreeOfParallelism": 4,       // 最大并行度
    "EnableBatching": true,            // 是否启用批处理
    "BatchSize": 100                   // 批处理大小
  }
}
```

### 2. 日志配置

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "PagedList": "Debug"  // 设置 PagedList 相关日志为 Debug 级别
    }
  }
}
```

### 3. AOT 编译配置

```json
{
  "AOT": {
    "PublishAot": true,      // 启用 AOT 编译
    "ReadyToRun": true,      // 启用 ReadyToRun 编译
    "TieredCompilation": true,  // 启用分层编译
    "TrimMode": "partial",     // 剪裁模式
    "Optimize": true         // 启用优化
  }
}
```

## 性能优化

### 1. 缓存使用

- **启用全局缓存**: 通过 `EnableCache` 配置启用
- **调整缓存大小**: 通过 `ThreadLocalCacheSize` 配置调整线程本地缓存大小
- **设置合理的缓存持续时间**: 通过 `CacheDuration` 配置设置

### 2. 异步编程

- **使用异步 API**: 优先使用 `GetPagedListAsync` 等异步方法
- **避免阻塞调用**: 不要在异步方法中使用 `Wait()` 或 `Result` 属性

### 3. 内存优化

- **启用零拷贝优化**: 通过 `EnableZeroCopy` 配置启用
- **调整 CPU 缓存行对齐**: 通过 `CacheLineSize` 配置调整

### 4. 并行处理

- **启用并行处理**: 通过 `EnableParallelProcessing` 配置启用
- **调整并行度**: 通过 `MaxDegreeOfParallelism` 配置调整

### 5. 批处理

- **启用批处理**: 通过 `EnableBatching` 配置启用
- **调整批处理大小**: 通过 `BatchSize` 配置调整

## 故障排除

### 1. 常见问题

#### 分页查询性能问题
- **检查数据源**: 确保数据源支持高效的分页操作
- **启用缓存**: 通过 `EnableCache` 配置启用缓存
- **优化查询条件**: 减少查询复杂度，添加适当的索引
- **合理设置分页大小**: 避免设置过大的分页大小

#### 内存使用过高
- **减少分页大小**: 通过 `MaxPageSize` 配置限制最大分页大小
- **启用内存优化**: 确保 `EnableZeroCopy` 配置为 true
- **调整缓存大小**: 通过 `ThreadLocalCacheSize` 配置调整缓存大小
- **检查内存泄漏**: 确保正确释放资源

#### 缓存失效
- **检查缓存配置**: 确保 `EnableCache` 配置为 true
- **验证缓存键生成**: 检查缓存键是否正确生成
- **监控缓存命中率**: 查看日志中的缓存命中情况

#### 参数验证错误
- **验证分页参数**: 确保页码和每页大小在合理范围内
- **检查数据源**: 确保数据源不为空
- **使用默认值**: 当参数无效时，使用默认值

### 2. 日志记录

PagedList 提供了详细的日志记录，可以帮助诊断问题：

```json
{
  "logging": {
    "logLevel": {
      "Default": "Information",
      "PagedList": "Debug"  // 设置 PagedList 相关日志为 Debug 级别
    }
  }
}
```

### 3. 性能监控

启用性能指标监控：

```csharp
// 启用性能指标监控
builder.Services.AddPagedListServices(options =>
{
    options.EnablePerformanceMetrics = true;
});
```

## 扩展开发

### 1. 自定义 PagedList 服务

```csharp
public class CustomPagedListService : IPagedListService
{
    private readonly ILogger<CustomPagedListService> _logger;
    private readonly PagedListOptions _options;

    public CustomPagedListService(ILogger<CustomPagedListService> logger, IOptions<PagedListOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<IPagedList<T>> GetPagedListAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Custom paged list service: page {pageNumber}, size {pageSize}");
        // 自定义分页逻辑
        return await Task.FromResult(source.ToPagedList(pageNumber, pageSize));
    }

    public async Task<IPagedList<T>> GetPagedListAsync<T>(IEnumerable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await GetPagedListAsync(source.AsQueryable(), pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedResultAsync<T>(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var pagedList = await GetPagedListAsync(source, pageNumber, pageSize, cancellationToken);
        return new PagedResult<T>(pagedList, _options);
    }
}
```

### 2. 注册自定义服务

```csharp
// 注册自定义 PagedList 服务
builder.Services.AddSingleton<IPagedListService, CustomPagedListService>();
builder.Services.Configure<PagedListOptions>(options =>
{
    // 配置选项
});
```

## 部署指南

### 1. 自包含部署

```bash
# 构建自包含部署包
dotnet publish -c Release -r win-x64 --self-contained true

# 运行应用
./bin/Release/net11.0/win-x64/publish/YourApp.exe
```

### 2. AOT 编译部署

```bash
# 使用 AOT 编译构建
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true

# 运行应用
./bin/Release/net11.0/win-x64/publish/YourApp.exe
```

### 3. 容器化部署

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0-windowsservercore-ltsc2022 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["YourApp.csproj", "."]
RUN dotnet restore "YourApp.csproj"
COPY . .
WORKDIR "/src/YourApp"
RUN dotnet build "YourApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YourApp.csproj" -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["YourApp.exe"]
```

## 结论

PagedList 是一个功能强大的分页解决方案，可以帮助开发者构建高性能的分页功能，适用于各种场景，如数据查询、API 响应格式化、数据展示等。该技能支持 .NET 10 AOT 编译，具有高性能、可扩展性和易用性等特点，适用于各种规模的项目。

通过本文档，你应该已经了解了 PagedList 的核心组件、使用方法、配置选项、性能优化和故障排除等内容。如果你有任何问题或建议，请参考 SKILL.md 文件或联系 VSA Architecture Team。
