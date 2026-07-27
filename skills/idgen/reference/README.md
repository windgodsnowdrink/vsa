# IDGen - 参考文档

## 概述

IDGen 是一个基于 .NET 10 的高性能 ID 生成系统，专为 .NET 开发者设计，提供多种 ID 生成算法和完整的 ID 管理功能。

## IDGen AOT 引擎

IDGen AOT 引擎是一个基于 .NET 10 AOT 编译的高性能 ID 生成解决方案，提供以下特性：

- **AOT 编译优化**：使用 .NET 10 的 AOT 编译技术，减少启动时间和内存占用
- **多种 ID 生成算法**：支持 Snowflake、Snowflake Drift、ULID 和 UUID
- **高性能设计**：优化的内存管理和并发处理
- **完善的错误处理**：详细的错误信息和日志记录
- **灵活的配置选项**：支持自定义配置和环境变量
- **ID 验证和解码**：提供 ID 验证和解析功能
- **基准测试**：内置性能基准测试功能

## 核心组件

### 1. IdgenService
- **位置**：scripts/idgen_aot.cs
- **功能**：核心 ID 生成业务逻辑处理
- **特性**：
  - 多种 ID 生成算法实现
  - ID 验证和解码
  - 内存缓存管理
  - 性能基准测试
  - 错误处理和日志记录

### 2. IdgenSettings
- **位置**：scripts/idgen_aot.cs
- **功能**：ID 生成服务配置选项
- **特性**：
  - 默认算法配置
  - Snowflake 相关配置
  - ULID 编码配置
  - 缓存配置
  - 速率限制配置

### 3. ULID 实现
- **位置**：scripts/idgen_aot.cs
- **功能**：ULID 算法的完整实现
- **特性**：
  - Base32 编码
  - Hex 编码
  - 时间戳支持
  - 随机数生成

## 使用示例

### 基本使用

```csharp
// 构建服务容器
var serviceProvider = BuildServiceProvider();
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

### 高级配置

```csharp
// 构建服务容器
var services = new ServiceCollection();

// 配置 IDGen 设置
services.Configure<IdgenSettings>(options => {
    options.DefaultAlgorithm = "snowflake";
    options.SnowflakeWorkerId = 1;
    options.SnowflakeDatacenterId = 1;
    options.SnowflakeEpoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    options.SnowflakeDriftThreshold = 1000;
    options.UlidEncoding = "base32";
    options.EnableCache = true;
    options.CacheSize = 2000;
    options.CacheExpiry = TimeSpan.FromMinutes(10);
    options.EnableRateLimiting = false;
    options.MaxRequestsPerSecond = 5000;
});

// 注册服务
services.AddSingleton<IdgenService>();
services.AddLogging(builder => {
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var serviceProvider = services.BuildServiceProvider();

// 使用服务
var idgenService = serviceProvider.GetRequiredService<IdgenService>();
var id = await idgenService.GenerateIdAsync("snowflake");
Console.WriteLine($"生成的 ID: {id}");
```

### 命令行使用

```bash
# 生成 Snowflake ID
idgen_aot.exe generate snowflake

# 批量生成 100 个 ULID
idgen_aot.exe batch ulid 100

# 验证 ID
idgen_aot.exe validate snowflake 1234567890

# 解码 ID
idgen_aot.exe decode snowflake 1234567890

# 运行基准测试
idgen_aot.exe benchmark snowflake 10000

# 显示配置
idgen_aot.exe config

# 显示帮助
idgen_aot.exe help
```

## 配置选项

### IdgenSettings 配置

```json
{
  "IdgenSettings": {
    "DefaultAlgorithm": "snowflake",
    "SnowflakeWorkerId": 1,
    "SnowflakeDatacenterId": 1,
    "SnowflakeEpoch": "2020-01-01T00:00:00Z",
    "SnowflakeDriftThreshold": 1000,
    "UlidEncoding": "base32",
    "EnableCache": true,
    "CacheSize": 1000,
    "CacheExpiry": "00:05:00",
    "EnableRateLimiting": false,
    "MaxRequestsPerSecond": 1000
  }
}
```

### AOT 编译配置

**idgen_aot.setting.json**：

```json
{
  "compilationOptions": {
    "targetFramework": "net11.0",
    "publishAot": true,
    "trimMode": "partial",
    "runtimeIdentifier": "win-x64"
  },
  "dependencies": {
    "Microsoft.Extensions.DependencyInjection": "10.0.0",
    "Microsoft.Extensions.Logging": "10.0.0",
    "System.Text.Json": "8.0.0",
    "Microsoft.Extensions.Caching.Memory": "10.0.0",
    "System.Security.Cryptography": "4.3.0"
  }
}
```

### 运行配置

**idgen_aot.run.json**：

```json
{
  "profiles": {
    "Generate ID": {
      "commandLineArgs": "generate snowflake",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Development"
      }
    },
    "Run Benchmark": {
      "commandLineArgs": "benchmark snowflake 10000",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Production"
      }
    }
  }
}
```

## 性能优化

1. **启用缓存**：启用内存缓存以提高性能
2. **异步编程**：使用异步 API 避免阻塞
3. **批量处理**：批量生成 ID 以提高效率
4. **选择合适的算法**：根据业务场景选择合适的 ID 生成算法
5. **合理配置 Snowflake 参数**：根据集群规模调整 Worker ID 和 Datacenter ID
6. **使用 AOT 编译**：利用 .NET 10 的 AOT 编译技术提高性能

## 故障排除

### 常见问题

1. **Snowflake 时钟回拨**
   - 检查系统时间是否正确
   - 调整 SnowflakeDriftThreshold 参数
   - 考虑使用 Snowflake Drift 算法

2. **性能问题**
   - 启用缓存
   - 调整缓存大小
   - 优化配置参数
   - 考虑使用更高性能的算法

3. **ID 冲突**
   - 确保 Snowflake Worker ID 在集群中唯一
   - 检查系统时间同步
   - 考虑使用 ULID 或 UUID 算法

4. **编译错误**
   - 检查依赖版本是否正确
   - 验证 .NET 10 安装是否完整
   - 检查 AOT 编译配置

## 扩展开发

### 添加自定义 ID 生成算法

```csharp
public class CustomIdAlgorithm
{
    public string GenerateId()
    {
        // 实现自定义 ID 生成算法
        return Guid.NewGuid().ToString("N");
    }
}

// 集成到 IdgenService
public class ExtendedIdgenService : IdgenService
{
    private readonly CustomIdAlgorithm _customAlgorithm;

    public ExtendedIdgenService(ILogger<IdgenService> logger, IOptions<IdgenSettings> options)
        : base(logger, options)
    {
        _customAlgorithm = new CustomIdAlgorithm();
    }

    public async Task<string> GenerateCustomIdAsync()
    {
        var id = _customAlgorithm.GenerateId();
        
        // 缓存生成的 ID
        if (_settings.EnableCache)
        {
            var cacheKey = $"id:custom:{id}";
            _cache.Set(cacheKey, new { Algorithm = "custom", GeneratedAt = DateTime.UtcNow }, new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = _settings.CacheExpiry,
                Size = 1
            });
        }
        
        return id;
    }
}

// 注册自定义服务
builder.Services.AddSingleton<ExtendedIdgenService>();
```

### 集成现有系统

```csharp
// 集成现有 ID 生成系统
public class IntegratedIdgenService : IdgenService
{
    private readonly IExistingIdService _existingIdService;

    public IntegratedIdgenService(ILogger<IdgenService> logger, IOptions<IdgenSettings> options, IExistingIdService existingIdService)
        : base(logger, options)
    {
        _existingIdService = existingIdService;
    }

    public async Task<string> GenerateExistingIdAsync()
    {
        // 使用现有系统生成 ID
        var id = await _existingIdService.GenerateIdAsync();
        
        // 缓存生成的 ID
        if (_settings.EnableCache)
        {
            var cacheKey = $"id:existing:{id}";
            _cache.Set(cacheKey, new { Algorithm = "existing", GeneratedAt = DateTime.UtcNow }, new MemoryCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = _settings.CacheExpiry,
                Size = 1
            });
        }
        
        return id;
    }
}
```

## 算法比较

| 算法 | 长度 | 顺序性 | 时间戳 | 唯一性 | 性能 | 适用场景 |
|------|------|--------|--------|--------|------|----------|
| Snowflake | 18-20 位 | 是 | 是 | 高 | 极高 | 分布式系统、需要顺序 ID |
| Snowflake Drift | 18-20 位 | 是 | 是 | 高 | 极高 | 分布式系统、时钟不稳定环境 |
| ULID | 26 字符 | 是 | 是 | 极高 | 高 | 需要排序的唯一标识符 |
| UUID | 36 字符 | 否 | 否 | 极高 | 中 | 不需要排序的唯一标识符 |

## 最佳实践

1. **根据业务场景选择算法**：
   - 分布式系统：Snowflake 或 Snowflake Drift
   - 需要排序：ULID
   - 简单唯一标识：UUID

2. **合理配置 Snowflake**：
   - Worker ID：集群中唯一
   - Datacenter ID：数据中心唯一
   - Epoch：选择合适的起始时间

3. **性能优化**：
   - 启用缓存
   - 使用批量生成
   - 选择合适的算法

4. **可靠性考虑**：
   - 处理时钟回拨
   - 确保 ID 唯一性
   - 监控性能指标

5. **安全考虑**：
   - 避免在 ID 中包含敏感信息
   - 考虑 ID 的可预测性
   - 适当使用随机化算法
