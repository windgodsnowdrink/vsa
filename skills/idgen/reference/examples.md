# IDGen - 使用示例

## IDGen AOT 引擎示例

IDGen AOT 引擎是一个基于 .NET 10 AOT 编译的高性能 ID 生成解决方案，以下是详细的使用示例。

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IDGen AOT 引擎 - 基本使用示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var idgenService = serviceProvider.GetRequiredService<IdgenService>();
        
        // 生成 Snowflake ID
        Console.WriteLine("1. 生成 Snowflake ID");
        var snowflakeId = await idgenService.GenerateIdAsync("snowflake");
        Console.WriteLine($"Snowflake ID: {snowflakeId}");
        
        // 生成 ULID
        Console.WriteLine("\n2. 生成 ULID");
        var ulid = await idgenService.GenerateIdAsync("ulid");
        Console.WriteLine($"ULID: {ulid}");
        
        // 生成 UUID
        Console.WriteLine("\n3. 生成 UUID");
        var uuid = await idgenService.GenerateIdAsync("uuid");
        Console.WriteLine($"UUID: {uuid}");
        
        // 生成 Snowflake Drift ID
        Console.WriteLine("\n4. 生成 Snowflake Drift ID");
        var snowflakeDriftId = await idgenService.GenerateIdAsync("snowflake_drift");
        Console.WriteLine($"Snowflake Drift ID: {snowflakeDriftId}");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        
        services.Configure<IdgenSettings>(options => {
            options.DefaultAlgorithm = "snowflake";
            options.SnowflakeWorkerId = 1;
            options.SnowflakeDatacenterId = 1;
            options.SnowflakeEpoch = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            options.SnowflakeDriftThreshold = 1000;
            options.UlidEncoding = "base32";
            options.EnableCache = true;
            options.CacheSize = 1000;
            options.CacheExpiry = TimeSpan.FromMinutes(5);
        });
        
        services.AddSingleton<IdgenService>();
        services.AddLogging(builder => {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        return services.BuildServiceProvider();
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IDGen AOT 引擎 - 高级配置示例");
        Console.WriteLine("=" * 60);
        
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
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取配置
        var settings = serviceProvider.GetRequiredService<IOptions<IdgenSettings>>().Value;
        Console.WriteLine("配置信息:");
        Console.WriteLine($"默认算法: {settings.DefaultAlgorithm}");
        Console.WriteLine($"Snowflake Worker ID: {settings.SnowflakeWorkerId}");
        Console.WriteLine($"Snowflake Datacenter ID: {settings.SnowflakeDatacenterId}");
        Console.WriteLine($"Snowflake Epoch: {settings.SnowflakeEpoch}");
        Console.WriteLine($"Snowflake Drift Threshold: {settings.SnowflakeDriftThreshold} ms");
        Console.WriteLine($"ULID 编码: {settings.UlidEncoding}");
        Console.WriteLine($"启用缓存: {settings.EnableCache}");
        Console.WriteLine($"缓存大小: {settings.CacheSize}");
        Console.WriteLine($"缓存过期: {settings.CacheExpiry}");
        
        // 使用服务
        var idgenService = serviceProvider.GetRequiredService<IdgenService>();
        
        // 生成带配置的 ID
        Console.WriteLine("\n生成 ID");
        var id = await idgenService.GenerateIdAsync(settings.DefaultAlgorithm);
        Console.WriteLine($"生成的 ID: {id}");
    }
}
```

### 3. ID 验证和解码示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("IDGen AOT 引擎 - ID 验证和解码示例");
        Console.WriteLine("=" * 60);
        
        // 构建服务容器
        var serviceProvider = BuildServiceProvider();
        var idgenService = serviceProvider.GetRequiredService<IdgenService>();
        
        // 生成 ID
        var snowflakeId = await idgenService.GenerateIdAsync("snowflake");
        var ulid = await idgenService.GenerateIdAsync("ulid");
        
        // 验证 Snowflake ID
        Console.WriteLine("1. 验证 Snowflake ID");
        var snowflakeValidationResult = await idgenService.ValidateIdAsync("snowflake", snowflakeId);
        Console.WriteLine($"Snowflake ID: {snowflakeId}");
        Console.WriteLine($"验证状态: {(snowflakeValidationResult.Valid ? "有效" : "无效"}");
        
        // 验证 ULID
        Console.WriteLine("\n2. 验证 ULID");
        var ulidValidationResult = await idgenService.ValidateIdAsync("ulid", ulid);
        Console.WriteLine($"ULID: {ulid}");
        Console.WriteLine($"验证状态: {(ulidValidationResult.Valid ? "有效" : "无效"}");
        
        // 解码 Snowflake ID
        Console.WriteLine("\n3. 解码 Snowflake ID");
        var snowflakeDecodeResult = await idgenService.DecodeIdAsync("snowflake", snowflakeId);
        if (snowflakeDecodeResult.Success)
        {
            Console.WriteLine("解码结果:");
            foreach (var kvp in snowflakeDecodeResult.DecodedData)
            {
                Console.WriteLine($"  {kvp.Key}: {kvp.Value}
