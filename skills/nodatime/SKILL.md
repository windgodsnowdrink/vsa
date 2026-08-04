# NodaTime Agent Skill - NodaTime 技能

## 技能概览

基于 .NET 10 的高性能 NodaTime 技能，为 .NET 开发者提供强大的日期和时间处理功能。

## 快速入门指南

### 安装依赖

在主应用的运行文件中添加以下依赖：

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package NodaTime@3.1.9
#:package NodaTime.Serialization.JsonNet@3.1.0
#:package NodaTime.Serialization.SystemTextJson@1.0.0
`

### 注册服务

在主应用中注册 NodaTime 服务：

`csharp
// 注册 NodaTime 服务
builder.Services.AddSingleton<INodaTimeService, NodaTimeService>();
builder.Services.AddSingleton<INodaTimeCalculator, NodaTimeCalculator>();

// 配置 JSON 序列化以支持 NodaTime 类型
builder.Services.Configure<JsonOptions>(options => {
    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
});
`

### 使用示例

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

## 导航地图

`
nodatime/
├── index.yaml                   # 元数据索引说明
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能说明
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── nodatime_extensions.cs     # NodaTime 核心实现
    ├── nodatime_extensions.run.json  # 运行配置
    └── nodatime_extensions.setting.json  # 设置文件
`

## 主要功能

1. **日期和时间处理**：提供强大的日期和时间类型，如 LocalDate、LocalTime、LocalDateTime、ZonedDateTime 等
2. **时区支持**：内置丰富的时区数据，支持全球时区转换
3. **时间计算**：提供时间差计算、时间加减、周期计算等功能
4. **格式化和解析**：支持多种日期时间格式的格式化和解析
5. **序列化支持**：集成 JSON 序列化，支持 NodaTime 类型的序列化和反序列化
6. **高性能设计**：使用不可变类型和缓存，提高性能
7. **易于使用的 API**：简单直观的 API 设计，减少错误
8. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供了完整的 NodaTime 解决方案，您可以根据需要进行扩展：

1. **自定义时间计算**：实现 INodaTimeCalculator 接口，创建自定义时间计算逻辑
2. **自定义序列化**：实现自定义的 NodaTime 序列化器
3. **集成其他系统**：将 NodaTime 与其他系统集成，如数据库、缓存等
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务生命周期，提高代码可测试性
2. **不可变类型**：使用 NodaTime 的不可变类型，避免并发问题
3. **时区处理**：始终使用显式时区，避免时区混淆
4. **错误处理**：正确处理日期时间解析和计算错误
5. **日志记录**：添加适当的日志记录，便于故障排查
6. **性能监控**：监控关键性能指标，及时发现和解决性能问题

## 配置选项

### NodaTimeOptions 配置

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| DefaultTimeZone | string | "UTC" | 默认时区 |
| EnableCache | bool | true | 是否启用缓存 |
| CacheSize | int | 1000 | 缓存大小 |
| Timeout | TimeSpan | 30秒 | 操作超时时间 |
| EnableDetailedLogging | bool | false | 是否启用详细日志 |

## 性能特性

- **高吞吐量**：使用缓存和不可变类型，支持高并发处理
- **低延迟**：优化的时间计算算法，减少计算时间
- **内存优化**：使用缓存和对象池，减少内存分配
- **可扩展性**：支持水平扩展，适用于大规模应用

## 版本兼容性

| .NET 版本 | 兼容性 |
|-----------|--------|------|
| .NET 10.0 | ✅ 完全支持 |
| .NET 9.0  | ✅ 支持 |
| .NET 8.0  | ✅ 支持 |
