# NodaTime - 参考文档

## 功能说明

NodaTime 是一个基于 .NET 10 的高性能日期和时间处理库，专为 .NET 开发者设计。它提供了强大的日期和时间类型、时区支持、时间计算、格式化和解析等功能，帮助开发者构建可靠的时间相关应用。

## 核心组件

### 1. INodaTimeService (NodaTime 服务)
- **位置**: scripts/nodatime_extensions.cs
- **功能**: 核心日期和时间处理功能，提供基础操作
- **特性**: 
  - 获取当前日期和时间
  - 时区转换
  - 日期时间格式化和解析
  - 序列化支持

### 2. INodaTimeCalculator (NodaTime 计算器)
- **位置**: scripts/nodatime_extensions.cs
- **功能**: 提供时间计算功能
- **特性**: 
  - 计算时间差异
  - 时间加减操作
  - 周期计算
  - 日期范围计算

### 3. NodaTimeOptions (NodaTime 选项)
- **位置**: scripts/nodatime_extensions.cs
- **功能**: 配置 NodaTime 服务的选项
- **特性**: 
  - 默认时区设置
  - 缓存配置
  - 超时设置
  - 日志配置

## 使用示例

### 基本用法

`csharp
// 获取 NodaTime 服务
var nodaTimeService = serviceProvider.GetRequiredService<INodaTimeService>();
var nodaTimeCalculator = serviceProvider.GetRequiredService<INodaTimeCalculator>();

// 使用核心功能
var now = nodaTimeService.GetCurrentDateTime();
Console.WriteLine($"当前时间: {now}");

// 计算两个时间之间的差异
var start = new LocalDateTime(2024, 1, 1, 0, 0, 0);
var end = new LocalDateTime(2024, 1, 2, 12, 0, 0);
var duration = nodaTimeCalculator.CalculateDuration(start, end);
Console.WriteLine($"时间差异: {duration}");

// 时区转换
var utcTime = Instant.FromUtc(2024, 1, 1, 0, 0);
var localTime = nodaTimeService.ConvertToLocalTime(utcTime, "Asia/Shanghai");
Console.WriteLine($"UTC 时间: {utcTime}");
Console.WriteLine($"上海时间: {localTime}");

// 格式化时间
var formattedTime = nodaTimeService.FormatDateTime(now, "yyyy-MM-dd HH:mm:ss");
Console.WriteLine($"格式化时间: {formattedTime}");
`

### 高级配置

`csharp
// 配置 NodaTime 选项
builder.Services.Configure<NodaTimeOptions>(options => {
    options.DefaultTimeZone = "Asia/Shanghai";
    options.EnableCache = true;
    options.CacheSize = 2000;
    options.Timeout = TimeSpan.FromSeconds(60);
    options.EnableDetailedLogging = true;
});

// 注册服务
builder.Services.AddSingleton<INodaTimeService, NodaTimeService>();
builder.Services.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();

// 获取配置
var settings = serviceProvider.GetRequiredService<IOptions<NodaTimeOptions>>().Value;
Console.WriteLine($"配置: 默认时区={settings.DefaultTimeZone}, 缓存={settings.EnableCache}");
Console.WriteLine($"缓存大小={settings.CacheSize}, 超时={settings.Timeout}");
`

## 配置选项

### NodaTimeOptions 配置

`json
{
  "NodaTimeOptions": {
    "DefaultTimeZone": "UTC",      // 默认时区
    "EnableCache": true,          // 是否启用缓存
    "CacheSize": 1000,            // 缓存大小
    "Timeout": "00:00:30",        // 超时时间
    "EnableDetailedLogging": false  // 是否启用详细日志
  }
}
`

## 性能优化

1. **使用缓存**: 启用缓存以提高性能，减少重复计算和时区数据加载
2. **不可变类型**: 使用 NodaTime 的不可变类型，避免并发问题和内存分配
3. **批量处理**: 对于大量日期时间操作，使用批处理提高效率
4. **内存池**: 使用内存池减少内存分配和 GC 压力
5. **异步编程**: 使用异步 API 避免阻塞，提高系统吞吐量
6. **预加载时区数据**: 预加载常用时区数据，减少运行时加载时间
7. **使用适当的类型**: 根据需要选择合适的 NodaTime 类型，如 LocalDate、LocalTime、LocalDateTime 等
8. **避免频繁格式化**: 减少日期时间格式化操作，使用缓存存储格式化结果

## 故障排除

### 常见问题

1. **时区转换错误**
   - 检查时区标识符是否正确
   - 验证系统时区设置
   - 检查 NodaTime 时区数据是否最新

2. **日期时间解析错误**
   - 检查日期时间格式是否正确
   - 验证输入数据是否符合预期格式
   - 使用 try-catch 块捕获解析异常

3. **性能问题**
   - 启用缓存
   - 优化日期时间计算逻辑
   - 减少频繁的时区转换
   - 增加缓存大小

4. **序列化错误**
   - 确保已配置 NodaTime 序列化器
   - 检查 JSON 格式是否正确
   - 验证 NodaTime 类型是否支持序列化

5. **内存使用过高**
   - 减少不必要的日期时间对象创建
   - 使用不可变类型
   - 适当调整缓存大小

## 扩展开发

### 创建自定义时间计算器

`csharp
public class CustomNodaTimeCalculator : INodaTimeCalculator
{
    private readonly ILogger<CustomNodaTimeCalculator> _logger;
    private readonly NodaTimeOptions _options;

    public CustomNodaTimeCalculator(ILogger<CustomNodaTimeCalculator> logger, IOptions<NodaTimeOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Duration CalculateDuration(LocalDateTime start, LocalDateTime end)
    {
        _logger.LogInformation($"计算时间差异: {start} 到 {end}");
        return end - start;
    }

    public LocalDateTime AddDuration(LocalDateTime dateTime, Duration duration)
    {
        _logger.LogInformation($"添加时间: {dateTime} + {duration}");
        return dateTime + duration;
    }

    public LocalDateTime SubtractDuration(LocalDateTime dateTime, Duration duration)
    {
        _logger.LogInformation($"减去时间: {dateTime} - {duration}");
        return dateTime - duration;
    }

    public bool IsInRange(LocalDateTime dateTime, LocalDateTime start, LocalDateTime end)
    {
        return dateTime >= start && dateTime <= end;
    }
}
`

### 注册自定义计算器

`csharp
// 注册自定义计算器
builder.Services.AddSingleton<INodaTimeCalculator, CustomNodaTimeCalculator>();
`

### 自定义序列化

`csharp
// 配置自定义 NodaTime 序列化
builder.Services.Configure<JsonOptions>(options => {
    var settings = options.JsonSerializerOptions;
    settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    // 添加自定义序列化设置
    settings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    settings.WriteIndented = true;
});
`

## 部署指南

### 容器化部署

1. **创建 Dockerfile**

`dockerfile
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["nodatime_extensions.csproj", "."]
RUN dotnet restore "./nodatime_extensions.csproj"
COPY . .
WORKDIR "/src/.."
RUN dotnet build "nodatime_extensions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nodatime_extensions.csproj" -c Release -o /app/publish /p:PublishAot=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./nodatime_extensions"]
`

2. **构建和运行容器**

`bash
docker build -t nodatime-extensions .
docker run --name nodatime-extensions -d nodatime-extensions
`

### 云平台部署

NodaTime 技能可以部署在各种云平台上，如 Azure、AWS、GCP 等。以下是在 Azure 上部署的示例：

1. **创建 Azure App Service**
2. **配置应用设置**：设置环境变量和配置选项
3. **部署应用**：使用 Azure DevOps 或 GitHub Actions 自动部署
4. **监控和日志**：配置 Azure Monitor 进行监控和日志收集

## 安全最佳实践

1. **不可变类型**: 使用 NodaTime 的不可变类型，避免并发问题和数据篡改
2. **输入验证**: 对日期时间输入进行验证，防止恶意输入
3. **错误处理**: 正确处理日期时间解析和计算错误，避免信息泄露
4. **日志记录**: 添加适当的日志记录，便于安全审计和故障排查
5. **时区处理**: 始终使用显式时区，避免时区混淆导致的安全问题
6. **依赖管理**: 定期更新 NodaTime 依赖，修复安全漏洞

## 监控和维护

1. **健康检查**: 实现健康检查端点，监控系统状态
2. **性能监控**: 监控关键性能指标，如响应时间、吞吐量、内存使用等
3. **日志管理**: 集中管理日志，便于分析和故障排查
4. **告警机制**: 设置告警机制，及时发现和处理问题
5. **定期维护**: 定期更新 NodaTime 时区数据，确保时区信息准确
6. **缓存管理**: 监控缓存使用情况，及时调整缓存大小

## 版本历史

| 版本 | 日期 | 变更内容 |
|------|------|----------|
| 1.0.0 | 2026-01-24 | 初始版本，基于 .NET 10 |
| 1.0.1 | 2026-02-01 | 性能优化，添加缓存功能 |
| 1.0.2 | 2026-02-15 | 修复时区转换问题，增加序列化支持 |
| 1.0.3 | 2026-03-01 | 添加自定义计算器支持，优化内存使用 |
