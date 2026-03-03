# IDGen Agent Skill - IDGen 技能

## 技能概述

基于 .NET 10 的高性能 ID 生成技能，为 .NET 开发者提供强大的 ID 生成功能，包括 Snowflake、Snowflake Drift、ULID 和 UUID 等多种算法。

## IDGen AOT 引擎

IDGen AOT 引擎是一个基于 .NET 10 AOT 编译的高性能 ID 生成解决方案，提供以下特性：

- **AOT 编译优化**：使用 .NET 10 的 AOT 编译技术，减少启动时间和内存占用
- **多种 ID 生成算法**：支持 Snowflake、Snowflake Drift、ULID 和 UUID
- **高性能设计**：优化的内存管理和并发处理
- **完善的错误处理**：详细的错误信息和日志记录
- **灵活的配置选项**：支持自定义配置和环境变量
- **ID 验证和解码**：提供 ID 验证和解析功能
- **基准测试**：内置性能基准测试功能

## 快速开始指南

### 安装依赖

在主应用的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Text.Json@8.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package System.Security.Cryptography@4.3.0
```

### 注册服务

在主应用中注册 ID 生成服务：

```csharp
// 配置 IDGen 设置
builder.Services.Configure<IdgenSettings>(options => {
    options.DefaultAlgorithm = "snowflake";
    options.SnowflakeWorkerId = 1;
    options.SnowflakeDatacenterId = 1;
    options.SnowflakeEpoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    options.SnowflakeDriftThreshold = 1000;
    options.UlidEncoding = "base32";
    options.EnableCache = true;
    options.CacheSize = 1000;
});

// 注册 IDGen 服务
builder.Services.AddSingleton<IdgenService>();
```

### 使用示例

```csharp
// 获取 IDGen 服务
var idgenService = serviceProvider.GetRequiredService<IdgenService>();

// 生成 ID
var snowflakeId = await idgenService.GenerateIdAsync("snowflake");
Console.WriteLine($"Snowflake ID: {snowflakeId}");

var ulid = await idgenService.GenerateIdAsync("ulid");
Console.WriteLine($"ULID: {ulid}");

// 验证 ID
var validationResult = await idgenService.ValidateIdAsync("snowflake", snowflakeId);
Console.WriteLine($"验证结果: {validationResult.Valid}");

// 解码 ID
var decodeResult = await idgenService.DecodeIdAsync("snowflake", snowflakeId);
if (decodeResult.Success)
{
    Console.WriteLine("解码结果:");
    foreach (var kvp in decodeResult.DecodedData)
    {
        Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
    }
}
```

## 导航地图

```
idgen/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── idgen_aot.cs            # IDGen AOT 核心实现
    ├── idgen_aot.run.json      # AOT 运行配置
    ├── idgen_aot.setting.json  # AOT 设置文件
    ├── idgenerator_service.cs  # ID 生成器服务
    ├── snowflake_drift_service.cs # Snowflake Drift 服务
    ├── snowflake_service.cs    # Snowflake 服务
    ├── ulid_service.cs         # ULID 服务
    └── 其他服务文件              # 其他 ID 生成服务
```

## 主要功能

1. **多种 ID 生成算法**：支持 Snowflake、Snowflake Drift、ULID 和 UUID
2. **ID 验证**：验证生成的 ID 是否有效
3. **ID 解码**：解析 ID 包含的信息（如时间戳、工作节点等）
4. **批量生成**：支持批量生成多个 ID
5. **高性能设计**：AOT 编译优化和内存管理
6. **缓存机制**：内置内存缓存，提高性能
7. **基准测试**：内置性能基准测试功能
8. **易使用的 API**：简洁直观的 API 设计
9. **可扩展架构**：支持自定义扩展和集成

## 扩展说明

本技能提供完整的 ID 生成解决方案，您可以根据需要进行扩展：

1. **自定义实现**：实现自定义的 ID 生成算法
2. **扩展功能**：添加新的 ID 生成特性
3. **与其他系统集成**：与现有的系统集成
4. **性能优化**：针对特定场景优化性能

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务
2. **异步编程**：使用异步 API 避免阻塞
3. **错误处理**：正确处理异常情况
4. **日志记录**：添加适当的日志记录
5. **性能监控**：监控关键性能指标
6. **合理配置**：根据实际需求调整配置参数
7. **选择合适的算法**：根据业务场景选择合适的 ID 生成算法

## 命令行使用

### 生成 ID

```bash
# 生成 Snowflake ID
idgen_aot.exe generate snowflake

# 生成 ULID
idgen_aot.exe generate ulid

# 生成 UUID
idgen_aot.exe generate uuid

# 生成 Snowflake Drift ID
idgen_aot.exe generate snowflake_drift
```

### 批量生成 ID

```bash
# 批量生成 100 个 Snowflake ID
idgen_aot.exe batch snowflake 100
```

### 验证 ID

```bash
# 验证 Snowflake ID
idgen_aot.exe validate snowflake <id>

# 验证 ULID
idgen_aot.exe validate ulid <id>
```

### 解码 ID

```bash
# 解码 Snowflake ID
idgen_aot.exe decode snowflake <id>

# 解码 ULID
idgen_aot.exe decode ulid <id>
```

### 运行基准测试

```bash
# 运行 Snowflake 算法基准测试（10000 次迭代）
idgen_aot.exe benchmark snowflake 10000

# 运行 ULID 算法基准测试
idgen_aot.exe benchmark ulid 10000
```

### 显示配置

```bash
# 显示当前配置
idgen_aot.exe config
```

### 显示帮助

```bash
# 显示帮助信息
idgen_aot.exe help
```
