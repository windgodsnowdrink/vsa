#:sdk Microsoft.NET.Sdk.Web
#:package DotNetCore.CAP@8.3.5
#:package Microsoft.Data.Sqlite@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Channels;

namespace DotNetCoreExtensions
{
    // 配置选项
    public class DotNetCoreExtensionOptions
    {
        public int MaxParallelism { get; set; } = 4;
        public int CacheExpirationSeconds { get; set; } = 300;
        public int EventChannelCapacity { get; set; } = 1000;
    }

    // 服务接口
    public interface IDotNetCoreExtensionService
    {
        Task ProcessDataAsync<T>(T data);
        Task<bool> TryPublishEventAsync<T>(T @event);
        Task<T> GetOrAddCacheAsync<T>(string key, Func<Task<T>> valueFactory);
    }

    // 服务实现
    public class DotNetCoreExtensionService : IDotNetCoreExtensionService
    {
        private readonly IMemoryCache _cache;
        private readonly DotNetCoreExtensionOptions _options;
        private readonly Channel<object> _eventChannel;

        public DotNetCoreExtensionService(
            IMemoryCache cache,
            IOptions<DotNetCoreExtensionOptions> options)
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
            // 使用通道进行并行处理
            var processingChannel = Channel.CreateBounded<T>(
                new BoundedChannelOptions(_options.MaxParallelism));

            // 启动处理任务
            var processingTasks = Enumerable.Range(0, _options.MaxParallelism)
                .Select(_ => Task.Run(async () =>
                {
                    await foreach (var item in processingChannel.Reader.ReadAllAsync())
                    {
                        // 处理数据
                        await ProcessItemAsync(item);
                    }
                }));

            // 添加数据到通道
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
                    TimeSpan.FromSeconds(_options.CacheExpirationSeconds);
                return valueFactory();
            });
        }

        private async Task ProcessItemAsync<T>(T item)
        {
            // 具体处理逻辑
            await Task.Delay(100); // 模拟处理
        }
    }

    // DI扩展方法
    public static class DotNetCoreExtensionServiceCollectionExtensions
    {
        public static IServiceCollection AddDotNetCoreExtensionServices(
            this IServiceCollection services,
            Action<DotNetCoreExtensionOptions> configureOptions = null)
        {
            services.AddOptions<DotNetCoreExtensionOptions>()
                .Configure(configureOptions ?? (opt => { }));

            services.AddMemoryCache();
            services.AddSingleton<IDotNetCoreExtensionService, DotNetCoreExtensionService>();

            return services;
        }
    }

    // 示例用法
    public static class ExampleUsage
    {
        public static async Task Demo()
        {
            var services = new ServiceCollection();
            services.AddDotNetCoreExtensionServices(options =>
            {
                options.MaxParallelism = 8;
                options.CacheExpirationSeconds = 600;
                options.EventChannelCapacity = 5000;
            });

            var provider = services.BuildServiceProvider();
            var service = provider.GetRequiredService<IDotNetCoreExtensionService>();

            // 使用缓存
            var cachedData = await service.GetOrAddCacheAsync("sample_key", 
                () => Task.FromResult("sample_value"));

            // 处理数据
            await service.ProcessDataAsync(new { Id = 1, Name = "Test" });

            // 发布事件
            await service.TryPublishEventAsync(new { EventId = Guid.NewGuid() });
        }
    }
}