#:sdk Microsoft.NET.Sdk
#:package TensorFlowSharp@2.4.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@7.0.0
#:package Microsoft.Extensions.Options@7.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TensorFlow;

namespace Trae.Vsa.AI
{
    public class TensorFlowOptions
    {
        public Dictionary<string, string> ModelPaths { get; set; } = new();
        public string RedisConnectionString { get; set; } = string.Empty;
        public int BatchSize { get; set; } = 32;
        public int MaxConcurrentBatches { get; set; } = 4;
        public bool EnableGpu { get; set; } = true;
        public int GpuMemoryFraction { get; set; } = 90;
        public bool EnableModelVersioning { get; set; } = true;
        public string DefaultModelName { get; set; } = "default";
    }

    public interface ITensorFlowService
    {
        Task<float[]> PredictAsync(float[] input, CancellationToken ct = default);
        Task<float[][]> BatchPredictAsync(float[][] inputs, CancellationToken ct = default);
    }

    public class TensorFlowService : ITensorFlowService, IDisposable
    {
        private readonly ConcurrentDictionary<string, (TFGraph Graph, TFSession Session)> _models = new();
        private readonly ReaderWriterLockSlim _modelLock = new();
        private readonly Channel<float[][]> _batchChannel;
        private readonly ILogger<TensorFlowService> _logger;
        private readonly IDistributedCache _cache;
        private readonly TensorFlowOptions _options;
        private readonly CancellationTokenSource _cts = new();

        public TensorFlowService(
            IOptions<TensorFlowOptions> options,
            ILogger<TensorFlowService> logger,
            IDistributedCache cache)
        {
            _options = options.Value;
            _logger = logger;
            _cache = cache;

            // Load all models
            foreach (var kvp in _options.ModelPaths)
            {
                LoadModel(kvp.Key, kvp.Value);
            }

            // Configure GPU if enabled
            if (_options.EnableGpu)
            {
                var config = new TFConfig
                {
                    GpuOptions = new TFGPUOptions
                    {
                        PerProcessGpuMemoryFraction = _options.GpuMemoryFraction / 100.0
                    }
                };
                TFSession.SetConfig(config);
            }

            // Setup batch processing
            _batchChannel = Channel.CreateBounded<float[][]>(
                new BoundedChannelOptions(_options.MaxConcurrentBatches)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });

            // Start batch processors
            for (int i = 0; i < _options.MaxConcurrentBatches; i++)
            {
                Task.Run(() => ProcessBatchesAsync(_cts.Token));
            }
        }

        public async Task<float[]> PredictAsync(float[] input, string modelName = null, CancellationToken ct = default)
        {
            modelName ??= _options.DefaultModelName;
            
            if (!_models.TryGetValue(modelName, out var model))
                throw new ArgumentException($"Model '{modelName}' not found");

            var cacheKey = $"tf_{modelName}_{string.Join("_", input)}";
            var cached = await _cache.GetAsync(cacheKey, ct);
            if (cached != null)
                return cached.ToArray().Select(x => BitConverter.ToSingle(x, 0)).ToArray();

            var runner = model.Session.GetRunner();
            runner.AddInput(model.Graph["input"][0], new TFTensor(input));
            runner.Fetch(model.Graph["output"][0]);

            var output = runner.Run();
            var result = (float[])output[0].GetValue();

            await _cache.SetAsync(cacheKey, result.Select(x => BitConverter.GetBytes(x)).ToArray(), 
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) }, ct);

            return result;
        }

        public async Task<float[][]> BatchPredictAsync(float[][] inputs, CancellationToken ct = default)
        {
            var batch = inputs.Take(_options.BatchSize).ToArray();
            var completion = new TaskCompletionSource<float[][]>();
            await _batchChannel.Writer.WriteAsync(batch, ct);
            return await completion.Task;
        }

        private async Task ProcessBatchesAsync(CancellationToken ct)
        {
            await foreach (var batch in _batchChannel.Reader.ReadAllAsync(ct))
            {
                try
                {
                    var results = new List<float[]>();
                    foreach (var input in batch)
                    {
                        results.Add(await PredictAsync(input, ct));
                    }
                    // TODO: Complete batch processing
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing batch");
                }
            }
        }

        public void LoadModel(string name, string path)
        {
            _modelLock.EnterWriteLock();
            try
            {
                var graph = new TFGraph();
                var model = File.ReadAllBytes(path);
                graph.Import(model);
                var session = new TFSession(graph);
                _models[name] = (graph, session);
            }
            finally
            {
                _modelLock.ExitWriteLock();
            }
        }

        public void UnloadModel(string name)
        {
            _modelLock.EnterWriteLock();
            try
            {
                if (_models.TryRemove(name, out var model))
                {
                    model.Session.Dispose();
                    model.Graph.Dispose();
                }
            }
            finally
            {
                _modelLock.ExitWriteLock();
            }
        }

        public IEnumerable<string> GetLoadedModels() => _models.Keys;

        public void Dispose()
        {
            _cts.Cancel();
            _modelLock.EnterWriteLock();
            try
            {
                foreach (var model in _models.Values)
                {
                    model.Session.Dispose();
                    model.Graph.Dispose();
                }
                _models.Clear();
            }
            finally
            {
                _modelLock.ExitWriteLock();
            }
            GC.SuppressFinalize(this);
        }
    }

    public static class TensorFlowExtensions
    {
        public static IServiceCollection AddTensorFlowService(this IServiceCollection services, Action<TensorFlowOptions> configure)
        {
            services.Configure(configure);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = services.BuildServiceProvider()
                    .GetRequiredService<IOptions<TensorFlowOptions>>().Value.RedisConnectionString;
            });
            
            services.AddSingleton<ITensorFlowService, TensorFlowService>();
            
            // Add background service for model versioning
            if (services.BuildServiceProvider().GetRequiredService<IOptions<TensorFlowOptions>>().Value.EnableModelVersioning)
            {
                services.AddHostedService<TensorFlowModelVersioningService>();
            }
            
            return services;
        }
    }

    public class TensorFlowModelVersioningService : BackgroundService
    {
        private readonly ITensorFlowService _tfService;
        private readonly IOptions<TensorFlowOptions> _options;
        private readonly ILogger<TensorFlowModelVersioningService> _logger;

        public TensorFlowModelVersioningService(
            ITensorFlowService tfService,
            IOptions<TensorFlowOptions> options,
            ILogger<TensorFlowModelVersioningService> logger)
        {
            _tfService = tfService;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // TODO: Implement model version checking and hot reload
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Ignore
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in model versioning service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }
}