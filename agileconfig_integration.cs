#:sdk Microsoft.NET.Sdk.Web.Worker
#:package AgileConfig.Client@1.6.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@7.0.0
#:property TargetFramework net8.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using AgileConfig.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

public interface IAgileConfigService
{
    Task<T> GetConfigAsync<T>(string key);
    Task WatchAsync<T>(string key, Action<T> onChange);
}

public class AgileConfigService : IAgileConfigService, IDisposable
{
    private readonly ConfigClient _client;
    private readonly IDistributedCache _cache;
    private readonly Channel<ConfigWatchItem> _watchChannel;
    private readonly ThreadLocal<Memory<byte>> _threadLocalBuffer;
    private readonly ObjectPool<ConfigClient> _clientPool;
    
    public AgileConfigService(
        IOptions<ConfigClientOptions> options,
        ObjectPool<ConfigClient> clientPool,
        RedisCacheOptions redisOptions)
    {
        _clientPool = clientPool;
        _client = _clientPool.Get();
        _client.Options = options.Value;
        
        // 使用Redis作为二级缓存
        _cache = new RedisCache(redisOptions);
        
        // 使用Channel处理配置变更通知
        _watchChannel = Channel.CreateBounded<ConfigWatchItem>(1000);
        
        // 线程专用内存缓冲区
        _threadLocalBuffer = new ThreadLocal<Memory<byte>>(
            () => new Memory<byte>(new byte[4096]));
            
        // 启动配置监听任务
        Task.Run(ProcessConfigChangesAsync);
    }

    public async Task<T> GetConfigAsync<T>(string key)
    {
        // 一级缓存检查
        var cachedValue = await _cache.GetAsync(key);
        if (cachedValue != null)
        {
            // 使用Span<T>零拷贝处理
            return JsonSerializer.Deserialize<T>(_threadLocalBuffer.Value.Span);
        }
        
        // 从AgileConfig服务器获取
        var config = _client.GetConfig(key);
        if (config != null)
        {
            // 写入缓存
            await _cache.SetAsync(key, 
                Encoding.UTF8.GetBytes(config.Value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                });
                
            return JsonSerializer.Deserialize<T>(config.Value);
        }
        
        return default;
    }

    public async Task WatchAsync<T>(string key, Action<T> onChange)
    {
        await _watchChannel.Writer.WriteAsync(new ConfigWatchItem
        {
            Key = key,
            Action = (val) => onChange((T)val)
        });
    }

    private async Task ProcessConfigChangesAsync()
    {
        // 监听配置变更事件
        _client.ConfigChanged += async (e) =>
        {
            foreach (var item in e.NewConfigs)
            {
                // 更新缓存
                await _cache.SetAsync(item.Key, 
                    Encoding.UTF8.GetBytes(item.Value),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    });
                
                // 通知监听者
                await _watchChannel.Writer.WriteAsync(new ConfigWatchItem
                {
                    Key = item.Key,
                    Value = item.Value
                });
            }
        };
        
        // 处理变更通知
        await foreach (var item in _watchChannel.Reader.ReadAllAsync())
        {
            // 触发回调
            item.Action?.Invoke(item.Value);
        }
    }

    public void Dispose()
    {
        _clientPool.Return(_client);
        _threadLocalBuffer.Dispose();
    }
}

public static class AgileConfigExtensions
{
    public static IServiceCollection AddAgileConfigServices(
        this IServiceCollection services,
        Action<ConfigClientOptions> configureOptions)
    {
        // 配置选项
        services.Configure(configureOptions);
        
        // Redis缓存配置
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
            options.InstanceName = "AgileConfig_";
        });
        
        // 对象池配置
        services.AddSingleton<ObjectPool<ConfigClient>>(sp => 
            new DefaultObjectPool<ConfigClient>(new ConfigClientPooledObjectPolicy(), 100));
            
        // 注册服务
        services.AddSingleton<IAgileConfigService, AgileConfigService>();
        
        return services;
    }
}

// 启动配置（在Program.cs中使用）
// builder.Services.AddAgileConfigServices(options =>
// {
//     options.AppId = "your_app_id";
//     options.Secret = "your_app_secret";
//     options.Nodes = new[] { "http://agileconfig-server:5000" };
//     options.Name = "your_app_name";
//     options.Env = "DEV";
// });