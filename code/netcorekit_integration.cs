using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Channels;
using System.Threading.Tasks;
using System;

namespace NetCoreKitExtensions
{
    // 配置选项
    public class NetCoreKitOptions
    {
        public int MaxConcurrentOperations { get; set; } = 10;
        public int CacheDurationInMinutes { get; set; } = 30;
        public int EventChannelCapacity { get; set; } = 1000;
        public bool EnableZeroCopy { get; set; } = true;
    }

    // 服务接口
    public interface INetCoreKitService
    {
        Task ProcessDataAsync<T>(T data);
        Task<bool> TryPublishEventAsync<T>(T @event);
        Task<T> GetOrAddCacheAsync<T>(string key, Func<Task<T>> valueFactory);
        Task<T> ProcessWithZeroCopyAsync<T>(ReadOnlyMemory<byte> data);
    }

    // 服务实现
    public class NetCoreKitService : INetCoreKitService
    {
        private readonly IMemoryCache _cache;
        private readonly NetCoreKitOptions _options;
        private readonly Channel<object> _eventChannel;

        public NetCoreKitService(
            IMemoryCache cache,
            IOptions<NetCoreKitOptions> options)
        {
            _cache = cache;
            _options = options.Value;
            _eventChannel = Channel.CreateBounded<object>(
                new BoundedChannelOptions(_options.EventChannelCapacity)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });
        }

        public async Task ProcessDataAsync<T>(T data)
        {
            var processingChannel = Channel.CreateBounded<T>(
                new BoundedChannelOptions(_options.MaxConcurrentOperations));

            var processingTasks = Enumerable.Range(0, _options.MaxConcurrentOperations)
                .Select(_ => Task.Run(async () =>
                {
                    await foreach (var item in processingChannel.Reader.ReadAllAsync())
                    {
                        await ProcessItemAsync(item);
                    }
                }));

            await processingChannel.Writer.WriteAsync(data);
            processingChannel.Writer.Complete();

            await Task.WhenAll(processingTasks);
        }

        public async Task<bool> TryPublishEventAsync<T>(T @event)
        {
            return await _eventChannel.Writer.WaitToWriteAsync() &&
                   await _eventChannel.Writer.WriteAsync(@event);
        }

        public async Task<T> GetOrAddCacheAsync<T>(string key, Func<Task<T>> valueFactory)
        {
            return await _cache.GetOrCreateAsync(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = 
                    TimeSpan.FromMinutes(_options.CacheDurationInMinutes);
                return valueFactory();
            });
        }

        public async Task<T> ProcessWithZeroCopyAsync<T>(ReadOnlyMemory<byte> data)
        {
            if (!_options.EnableZeroCopy)
                throw new InvalidOperationException("Zero copy is disabled");

            // 零拷贝处理逻辑
            await Task.Delay(100); // 模拟处理
            return default;
        }

        private async Task ProcessItemAsync<T>(T item)
        {
            await Task.Delay(100); // 模拟处理
        }
    }

    // DI扩展方法
    public static class NetCoreKitServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCoreKitServices(
            this IServiceCollection services,
            Action<NetCoreKitOptions> configureOptions = null)
        {
            services.AddOptions<NetCoreKitOptions>()
                .Configure(configureOptions ?? (opt => { }));

            services.AddMemoryCache();
            services.AddSingleton<INetCoreKitService, NetCoreKitService>();

            return services;
        }
    }

    // 示例用法
    public static class ExampleUsage
    {
        public static async Task Demo()
        {
            var services = new ServiceCollection();
            services.AddNetCoreKitServices(options =>
            {
                options.MaxConcurrentOperations = 16;
                options.CacheDurationInMinutes = 60;
                options.EventChannelCapacity = 5000;
                options.EnableZeroCopy = true;
            });

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<INetCoreKitService>();

            // 使用缓存
            var cachedData = await service.GetOrAddCacheAsync("sample_key", 
                () => Task.FromResult("sample_value"));

            // 处理数据
            await service.ProcessDataAsync(new { Id = 1, Name = "Test" });

            // 发布事件
            await service.TryPublishEventAsync(new { EventId = Guid.NewGuid() });

            // 零拷贝处理
            if (service is INetCoreKitService netCoreKitService)
            {
                var data = new byte[1024];
                await netCoreKitService.ProcessWithZeroCopyAsync<string>(data);
            }
        }
    }
}