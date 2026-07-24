# PagedList Agent Skill - PagedList 技能

## 技能概述

PagedList 技能是一个基于 .NET 10 的高性能分页解决方案，为 .NET 开发者提供强大的分页功能。该技能支持 AOT 编译，具有高性能、可扩展性和易用性等特点，适用于各种分页场景，如数据查询、API 响应格式化、数据展示等。

## 快速开始指南

### 环境要求

- .NET 10 SDK 或更高版本
- 支持的操作系统：Windows、Linux、macOS

### 安装依赖

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

### 注册服务

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
});
```

### 使用示例

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

## 导航地图

```
pagedlist/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── x_pagedlist_integration.cs     # PagedList 集成功能实现
    ├── x_pagedlist_integration.run.json  # 运行配置
    └── x_pagedlist_integration.setting.json  # 设置文件
```

## 核心功能

1. **高性能分页查询**：基于 LINQ 的高性能分页实现，支持多种数据源
2. **异步分页操作**：使用 async/await 模式，提高并发性能
3. **内存优化**：采用内存池、零拷贝等技术，减少内存使用
4. **缓存支持**：内置缓存机制，提高重复查询性能
5. **自定义分页选项**：支持灵活的分页配置，如默认页大小、最大页大小等
6. **错误处理与重试机制**：提供完善的错误处理和重试逻辑
7. **性能监控与指标**：内置性能监控，提供关键指标
8. **支持多种数据源**：支持 IEnumerable、IQueryable 等多种数据源

## 技术特性

1. **模块化设计**：采用模块化设计，便于扩展和维护
2. **依赖注入**：支持 .NET 依赖注入，便于服务管理
3. **异步编程**：使用 async/await 模式，提高并发性能
4. **高性能算法**：实现高效的分页算法
5. **LINQ 优化**：优化 LINQ 查询，提高查询性能
6. **错误处理与重试机制**：提供完善的错误处理和重试逻辑
7. **配置管理**：支持灵活的配置管理
8. **状态管理**：使用状态机管理分页状态
9. **管道处理模式**：使用管道模式处理分页数据

## 性能特性

1. **高性能设计**：优化的性能实现，支持高并发
2. **内存优化**：减少内存使用，提高内存效率
3. **并发支持**：支持并行处理，提高处理速度
4. **异步编程**：使用异步 API，避免阻塞
5. **批处理**：支持批处理，提高效率
6. **缓存机制**：使用缓存，减少重复计算
7. **零拷贝技术**：使用零拷贝技术，提高数据传输速度
8. **线程本地缓存**：使用线程本地缓存，减少线程竞争
9. **内存池管理**：使用内存池，减少内存分配和回收开销

## AOT 编译支持

PagedList 技能支持 .NET 10 AOT 编译，可以显著提升应用的启动速度和运行性能。

### 编译选项

```yaml
# AOT 编译选项
PublishAot: true      # 启用 AOT 编译
ReadyToRun: true      # 启用 ReadyToRun 编译
TieredCompilation: true  # 启用分层编译
TrimMode: partial     # 剪裁模式
Optimize: true        # 启用优化
EnableCompilationRelaxations: true  # 启用编译松弛
EnableEnhancedNgen: true  # 启用增强的 Ngen
```

### 支持的运行时

- win-x64
- linux-x64
- osx-x64
- win-arm64
- linux-arm64
- osx-arm64

### 编译命令

```bash
# 使用 AOT 编译 PagedList 技能
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true
```

## 扩展说明

PagedList 技能提供了完整的分页解决方案，你可以根据需要进行扩展：

1. **自定义分页服务实现**：实现 IPagedListService 接口，提供自定义的分页实现
2. **扩展功能**：添加新的分页功能，如自定义排序、过滤等
3. **与其他系统集成**：将 PagedList 技能与其他系统集成，如 ORM 框架、缓存系统等
4. **性能优化**：针对特定场景优化性能，如大规模数据分页、复杂查询分页等

### 示例：自定义分页服务实现

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
}
```

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，便于测试和扩展
2. **异步编程**：优先使用异步 API，避免阻塞主线程
3. **错误处理**：正确处理异常情况，提供适当的错误消息
4. **日志记录**：添加适当的日志记录，便于故障排除
5. **性能监控**：监控关键性能指标，如查询时间、内存使用等
6. **资源管理**：正确管理资源，避免资源泄漏
7. **配置管理**：使用配置文件或环境变量管理配置，便于部署和维护
8. **安全实践**：遵循安全最佳实践，如验证输入参数
9. **分页大小**：合理设置分页大小，平衡性能和用户体验
10. **缓存策略**：根据数据特性选择合适的缓存策略

## 配置选项

PagedList 技能提供了丰富的配置选项，可以根据需要进行调整：

### 基本配置

```json
{
  "PagedListOptions": {
    "DefaultPageSize": 20,            // 默认分页大小
    "MaxPageSize": 100,              // 最大分页大小
    "EnableCache": true,             // 是否启用缓存
    "CacheDuration": "00:05:00",     // 缓存持续时间
    "EnablePerformanceMetrics": true, // 是否启用性能指标
    "EnableZeroCopy": true,           // 是否启用零拷贝优化
    "ThreadLocalCacheSize": 1024      // 线程本地缓存大小
  }
}
```

### 性能配置

```json
{
  "PagedListPerformance": {
    "EnableParallelProcessing": true,  // 是否启用并行处理
    "MaxDegreeOfParallelism": 4,       // 最大并行度
    "EnableBatching": true,            // 是否启用批处理
    "BatchSize": 100,                  // 批处理大小
    "EnableCaching": true,             // 是否启用缓存
    "CacheSize": 1000,                 // 缓存大小
    "CacheDuration": "00:05:00"        // 缓存持续时间
  }
}
```

## 部署指南

### 自包含部署

```bash
# 构建自包含部署包
dotnet publish -c Release -r win-x64 --self-contained true

# 运行应用
./bin/Release/net11.0/win-x64/publish/YourApp.exe
```

### AOT 编译部署

```bash
# 使用 AOT 编译构建
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishAot=true /p:ReadyToRun=true /p:TieredCompilation=true /p:TrimMode=partial /p:Optimize=true

# 运行应用
./bin/Release/net11.0/win-x64/publish/YourApp.exe
```

### 容器化部署

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

## 故障排除

### 常见问题

1. **分页查询性能问题**
   - 检查数据源是否支持高效的分页
   - 启用缓存
   - 优化查询条件
   - 合理设置分页大小

2. **内存使用过高**
   - 减少分页大小
   - 启用内存池
   - 优化数据结构
   - 检查是否有内存泄漏

3. **缓存失效**
   - 检查缓存配置
   - 验证缓存键生成逻辑
   - 监控缓存命中率

4. **参数验证错误**
   - 验证分页参数
   - 检查数据源是否为空
   - 确保分页大小在合理范围内

### 日志记录

PagedList 技能提供了详细的日志记录，可以帮助诊断问题：

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

## 结论

PagedList 技能是一个功能强大的分页解决方案，可以帮助开发者构建高性能的分页功能，适用于各种场景，如数据查询、API 响应格式化、数据展示等。该技能支持 .NET 10 AOT 编译，具有高性能、可扩展性和易用性等特点，适用于各种规模的项目。

通过本文档，你应该已经了解了 PagedList 技能的核心功能、技术特性、使用方法和最佳实践。如果你有任何问题或建议，请参考参考文档或联系 VSA Architecture Team。
