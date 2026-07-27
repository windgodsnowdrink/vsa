#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.SemanticKernel@1.0.0
#:package Microsoft.SemanticKernel.Memory@1.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace SemanticMemoryIntegration
{
    public class SemanticMemoryOptions
    {
        public string ApiKey { get; set; }
        public string ModelId { get; set; }
        public string Endpoint { get; set; }
        public string MemoryConnectionString { get; set; }
        public int BatchSize { get; set; } = 100;
        public bool EnableDistributedCaching { get; set; }
        public string RedisConnectionString { get; set; }
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours(1);
    }

    public interface ISemanticMemoryService
    {
        // 基础记忆操作
        Task<string> RetrieveMemoryAsync(string query, CancellationToken cancellationToken = default);
        Task StoreMemoryAsync(string key, string text, CancellationToken cancellationToken = default);
        
        // 批量操作
        Task BatchIndexAsync(IEnumerable<string> documents, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> BatchRetrieveAsync(IEnumerable<string> queries, CancellationToken cancellationToken = default);
        
        // 多模态记忆
        Task<string> RetrieveImageMemoryAsync(byte[] image, CancellationToken cancellationToken = default);
        Task StoreImageMemoryAsync(string key, byte[] image, CancellationToken cancellationToken = default);
        Task<string> RetrieveAudioMemoryAsync(byte[] audio, CancellationToken cancellationToken = default);
        Task StoreAudioMemoryAsync(string key, byte[] audio, CancellationToken cancellationToken = default);
        
        // 高级RAG操作
        Task<IEnumerable<string>> RetrieveWithRAGAsync(string query, int topK = 5, float minScore = 0.7f, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> RetrieveHybridAsync(string query, int topK = 5, CancellationToken cancellationToken = default);
        
        // 记忆压缩
        Task CompressMemoriesAsync(int threshold = 1000, CancellationToken cancellationToken = default);
        
        // 分层存储管理
        Task MoveToColdStorageAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);
        Task PromoteFromColdStorageAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default);
        
        // 缓存管理
        Task ClearCacheAsync();
    }

    public class SemanticMemoryService : ISemanticMemoryService
    {
        private readonly ISemanticTextMemory _memory;
        private readonly IDistributedCache _cache;
        private readonly ILogger<SemanticMemoryService> _logger;
        private readonly Channel<string> _batchChannel;
        private readonly IModelRepository _modelRepository;
        private readonly ITieredMemoryService _tieredMemory;
        private readonly SemanticMemoryOptions _options;

        public SemanticMemoryService(
            IOptions<SemanticMemoryOptions> options,
            IDistributedCache cache,
            ILogger<SemanticMemoryService> logger,
            IModelRepository modelRepository,
            ITieredMemoryService tieredMemory)
        {
            _options = options.Value;
            _cache = cache;
            _logger = logger;
            _batchChannel = Channel.CreateBounded<string>(_options.BatchSize * 2);
            _modelRepository = modelRepository;
            _tieredMemory = tieredMemory;

            var memoryBuilder = new MemoryBuilder();
            memoryBuilder.WithAzureOpenAITextEmbeddingGeneration(
                _options.ModelId,
                _options.Endpoint,
                _options.ApiKey);

            if (!string.IsNullOrEmpty(_options.MemoryConnectionString))
            {
                memoryBuilder.WithMemoryStore(new AzureCognitiveSearchMemoryStore(
                    _options.MemoryConnectionString));
            }
            else
            {
                memoryBuilder.WithMemoryStore(new VolatileMemoryStore());
            }

            _memory = memoryBuilder.Build();
            InitializeBatchProcessing();
        }

        public async Task<string> RetrieveMemoryAsync(string query, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"memory:{query}";
            if (_options.EnableDistributedCaching)
            {
                var cachedResult = await _cache.GetStringAsync(cacheKey, cancellationToken);
                if (cachedResult != null)
                {
                    return cachedResult;
                }
            }

            // 优先从热存储查询
            var result = await _tieredMemory.GetFromHotStorageAsync(query, cancellationToken);
            if (result == null)
            {
                // 热存储未命中，从语义记忆查询
                var memoryRecord = await _memory.GetAsync("default", query, cancellationToken: cancellationToken);
                result = memoryRecord?.Metadata.Text ?? string.Empty;
                
                // 存入缓存和热存储
                if (_options.EnableDistributedCaching && !string.IsNullOrEmpty(result))
                {
                    await _cache.SetStringAsync(
                        cacheKey,
                        result,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _options.CacheExpiration },
                        cancellationToken);
                    await _tieredMemory.StoreToHotStorageAsync(query, result, cancellationToken);
                }
            }

            return result;
        }

        public async Task StoreMemoryAsync(string key, string text, CancellationToken cancellationToken = default)
        {
            await _memory.SaveInformationAsync("default", text, key, cancellationToken: cancellationToken);
        }

        public async Task BatchIndexAsync(IEnumerable<string> documents, CancellationToken cancellationToken = default)
        {
            var batchTasks = documents.Select(doc => 
                _batchChannel.Writer.WriteAsync(doc, cancellationToken).AsTask());
            
            await Task.WhenAll(batchTasks);
        }

        public async Task<IReadOnlyList<string>> BatchRetrieveAsync(IEnumerable<string> queries, CancellationToken cancellationToken = default)
        {
            var results = new ConcurrentBag<string>();
            await Parallel.ForEachAsync(queries, cancellationToken, async (query, ct) =>
            {
                results.Add(await RetrieveMemoryAsync(query, ct));
            });
            
            return results.ToList();
        }

        public async Task ClearCacheAsync()
        {
            if (_cache is MemoryCache memCache)
            {
                memCache.Compact(1.0);
            }
        }

        private void InitializeBatchProcessing()
        {
            Task.Run(async () =>
            {
                var batch = new List<string>();
                while (await _batchChannel.Reader.WaitToReadAsync())
                {
                    while (_batchChannel.Reader.TryRead(out var document))
                    {
                        batch.Add(document);
                        if (batch.Count >= _options.BatchSize)
                        {
                            await ProcessBatchAsync(batch);
                            batch.Clear();
                        }
                    }

                    if (batch.Count > 0)
                    {
                        await ProcessBatchAsync(batch);
                        batch.Clear();
                    }
                }
            });
        }

        private async Task ProcessBatchAsync(IEnumerable<string> documents)
        {
            await Parallel.ForEachAsync(documents, async (doc, ct) =>
            {
                await _memory.SaveInformationAsync("documents", doc, Guid.NewGuid().ToString(), ct);
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSemanticMemoryService(
            this IServiceCollection services,
            Action<SemanticMemoryOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<ISemanticMemoryService, SemanticMemoryService>();
            
            services.AddStackExchangeRedisCache(options =>
            {
                var settings = services.BuildServiceProvider()
                    .GetRequiredService<IOptions<SemanticMemoryOptions>>().Value;
                
                if (!string.IsNullOrEmpty(settings.RedisConnectionString))
                {
                    options.Configuration = settings.RedisConnectionString;
                }
            });
            
            return services;
        }
    }
}