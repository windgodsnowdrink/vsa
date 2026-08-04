# cache - 使用示例

## 快速入门

### 1. 基本内存缓存示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

public class Program
{
    public static async Task Main()
    {
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        
        Console.WriteLine("内存缓存基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 设置缓存项
        memoryCache.Set("user:1", "张三", TimeSpan.FromMinutes(10));
        memoryCache.Set("user:2", "李四", TimeSpan.FromMinutes(10));
        memoryCache.Set("config:app", new AppConfig { Version = "1.0.0", Env = "Development" }, TimeSpan.FromHours(1));
        
        Console.WriteLine("缓存项设置完成");
        
        // 获取缓存项
        var user1 = memoryCache.Get<string>("user:1");
        var user2 = memoryCache.Get<string>("user:2");
        var appConfig = memoryCache.Get<AppConfig>("config:app");
        
        Console.WriteLine($"用户1: {user1}");
        Console.WriteLine($"用户2: {user2}");
        Console.WriteLine($"应用配置: 版本={appConfig?.Version}, 环境={appConfig?.Env}");
        
        // 使用滑动过期
        memoryCache.Set("temp:data", "临时数据", new MemoryCacheEntryOptions {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        });
        
        Console.WriteLine("\n滑动过期缓存设置完成");
        
        // 检查缓存是否存在
        if (!memoryCache.TryGetValue("non:existent", out string nonExistent))
        {
            Console.WriteLine("不存在的缓存项: 返回默认值");
        }
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMemoryCache(options => {
            options.SizeLimit = 1024 * 1024 * 1024; // 1GB
        });
        return builder.BuildServiceProvider();
    }
}

public class AppConfig
{
    public string Version { get; set; }
    public string Env { get; set; }
}
```

### 2. 分布式缓存示例

```csharp
using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("分布式缓存示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
        
        // 设置分布式缓存
        await distributedCache.SetAsync("distributed:user:1", 
            Encoding.UTF8.GetBytes("分布式缓存用户1"),
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
        
        Console.WriteLine("分布式缓存项设置完成");
        
        // 获取分布式缓存
        var bytes = await distributedCache.GetAsync("distributed:user:1");
        var distributedValue = bytes != null ? Encoding.UTF8.GetString(bytes) : "缓存不存在";
        Console.WriteLine($"分布式缓存结果: {distributedValue}");
        
        // 使用自定义分布式缓存服务
        var customDistributedCache = serviceProvider.GetRequiredService<ICustomDistributedCache>();
        await customDistributedCache.SetAsync("custom:distributed:key", "自定义分布式缓存值", TimeSpan.FromHours(2));
        var customValue = await customDistributedCache.GetAsync<string>("custom:distributed:key");
        
        Console.WriteLine($"自定义分布式缓存结果: {customValue}");
        
        // 移除分布式缓存
        await distributedCache.RemoveAsync("distributed:user:1");
        Console.WriteLine("分布式缓存项已移除");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddStackExchangeRedisCache(options => {
            options.Configuration = "localhost:6379";
            options.InstanceName = "CacheSkill:";
        });
        builder.AddSingleton<ICustomDistributedCache, CustomDistributedCache>();
        return builder.BuildServiceProvider();
    }
}

public interface ICustomDistributedCache
{
    Task SetAsync<T>(string key, T value, TimeSpan expirationTime);
    Task<T> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}

public class CustomDistributedCache : ICustomDistributedCache
{
    private readonly IDistributedCache _distributedCache;
    
    public CustomDistributedCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(json), 
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = expirationTime
            });
    }
    
    public async Task<T> GetAsync<T>(string key)
    {
        var bytes = await _distributedCache.GetAsync(key);
        if (bytes == null)
        {
            return default;
        }
        var json = Encoding.UTF8.GetString(bytes);
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }
    
    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}
```

### 3. 多级缓存示例

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("多级缓存示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var multiLevelCache = serviceProvider.GetRequiredService<IMultiLevelCache>();
        
        // 设置多级缓存
        await multiLevelCache.SetAsync("multi:user:1", "多级缓存用户1", TimeSpan.FromMinutes(30));
        await multiLevelCache.SetAsync("multi:config:app", 
            new AppConfig { Version = "2.0.0", Env = "Production" }, 
            TimeSpan.FromHours(2));
        
        Console.WriteLine("多级缓存项设置完成");
        
        // 第一次获取（内存缓存中没有，从分布式缓存获取并同步到内存）
        var user1 = await multiLevelCache.GetAsync<string>("multi:user:1");
        Console.WriteLine($"第一次获取用户1: {user1}");
        
        // 第二次获取（直接从内存缓存获取）
        user1 = await multiLevelCache.GetAsync<string>("multi:user:1");
        Console.WriteLine($"第二次获取用户1: {user1}");
        
        // 获取应用配置
        var appConfig = await multiLevelCache.GetAsync<AppConfig>("multi:config:app");
        Console.WriteLine($"应用配置: 版本={appConfig?.Version}, 环境={appConfig?.Env}");
        
        // 移除多级缓存
        await multiLevelCache.RemoveAsync("multi:user:1");
        Console.WriteLine("多级缓存项已移除");
    }
    
    private static ServiceProvider BuildServiceProvider()
    {
        var builder = new ServiceCollection();
        builder.AddMemoryCache();
        builder.AddStackExchangeRedisCache(options => {
            options.Configuration = "localhost:6379";
            options.InstanceName = "CacheSkill:";
        });
        builder.AddSingleton<IMultiLevelCache, MultiLevelCache>();
        return builder.BuildServiceProvider();
    }
}

public interface IMultiLevelCache
{
    Task SetAsync<T>(string key, T value, TimeSpan expirationTime);
    Task<T> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}

public class MultiLevelCache : IMultiLevelCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    
    public MultiLevelCache(IMemoryCache memoryCache, IDistributedCache distributedCache)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        // 设置到内存缓存
        _memoryCache.Set(key, value, expirationTime);
        
        // 设置到分布式缓存
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(json), 
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = expirationTime
            });
    }
    
    public async Task<T> GetAsync<T>(string key)
    {
        // 先从内存缓存获取
        if (_memoryCache.TryGetValue(key, out T memoryValue))
        {
            return memoryValue;
        }
        
        // 从分布式缓存获取
        var bytes = await _distributedCache.GetAsync(key);
        if (bytes == null)
        {
            return default;
        }
        
        var json = Encoding.UTF8.GetString(bytes);
        var distributedValue = System.Text.Json.JsonSerializer.Deserialize<T>(json);
        
        // 同步到内存缓存
        _memoryCache.Set(key, distributedValue, TimeSpan.FromMinutes(10));
        
        return distributedValue;
    }
    
    public async Task RemoveAsync(string key)
    {
        // 从内存缓存移除
        _memoryCache.Remove(key);
        
        // 从分布式缓存移除
        await _distributedCache.RemoveAsync(key);
    }
}

public class AppConfig
{
    public string Version { get; set; }
    public string Env { get; set; }
}
```

### 4. AOT 编译缓存示例

```csharp
// 这是一个支持 AOT 编译的缓存示例
// 项目文件需要包含 AOT 配置

#:sdk Microsoft.NET.Sdk
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Caching.Memory@10.0.0
#:package MessagePack@2.5.149
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property PublishAot=true
#:property TrimMode=Full
#:property PublishReadyToRun=true
#:property PublishSingleFile=true
#:property SelfContained=true

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using MessagePack;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("AOT 编译缓存示例");
        Console.WriteLine("=" * 50);
        
        // 初始化服务容器
        var serviceProvider = BuildServiceProvider();
        var cacheService = serviceProvider.GetRequiredService<IAotCacheService>();
        
        // 性能测试
        const int iterations = 10000;
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine($"\n执行 {iterations} 次缓存写入操作...");
        
        // 批量写入缓存
        for (int i = 0; i < iterations; i++)
        {
            await cacheService.SetAsync($"aot:key:{i}", new CacheData { Id = i, Value = $"数据 {i}", CreatedAt = DateTime.UtcNow }, TimeSpan.FromMinutes(30));
        }
        
        stopwatch.Stop();
        Console.WriteLine($"写入完成，耗时: {stopwatch.Elapsed.TotalMilliseconds:F3} ms");
        Console.WriteLine($"平均每写入: {stopwatch.Elapsed.TotalMilliseconds / iterations:F3} ms
