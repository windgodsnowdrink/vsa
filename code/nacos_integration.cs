#:sdk Microsoft.NET.Sdk
#:package nacos-sdk-csharp@2.1.0
#:package Microsoft.Extensions.Options@7.0.0
#:package Polly@7.2.3
#:package Microsoft.Extensions.ObjectPool@7.0.0
#:package OpenTelemetry.Extensions.Hosting@1.0.0-rc9

using Nacos.V2;
using Nacos.V2.Config;
using Microsoft.Extensions.Options;
using Polly;
using Microsoft.Extensions.ObjectPool;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace NacosIntegration
{
    public class NacosOptions
    {
        public string ServerAddresses { get; set; } = "http://localhost:8848";
        public string Namespace { get; set; } = "public";
        public string GroupName { get; set; } = "DEFAULT_GROUP";
        public string ServiceName { get; set; } = "";
        public string ClusterName { get; set; } = "DEFAULT";
        public bool MultiTenantEnabled { get; set; } = false;
        public string TenantHeaderName { get; set; } = "X-Tenant-ID";
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(200);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
        public bool ZeroCopyEnabled { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024 * 10; // 10MB
        public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan TracingSamplingInterval { get; set; } = TimeSpan.FromSeconds(10);
    }

    public interface INacosService
{
    /* 配置管理 */
    /// <summary>
    /// 获取配置
    /// </summary>
    /// <param name="dataId">配置ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置内容</returns>
    Task<string> GetConfigAsync(string dataId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 发布配置
    /// </summary>
    /// <param name="dataId">配置ID</param>
    /// <param name="content">配置内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task PublishConfigAsync(string dataId, string content, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除配置
    /// </summary>
    /// <param name="dataId">配置ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task RemoveConfigAsync(string dataId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 监听配置变更
    /// </summary>
    /// <param name="dataId">配置ID</param>
    /// <param name="callback">变更回调</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ListenConfigAsync(string dataId, Action<string> callback, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 取消监听配置变更
    /// </summary>
    /// <param name="dataId">配置ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task UnlistenConfigAsync(string dataId, CancellationToken cancellationToken = default);
    
    /* 服务管理 */
    /// <summary>
    /// 注册服务实例
    /// </summary>
    /// <param name="serviceName">服务名</param>
    /// <param name="ip">实例IP</param>
    /// <param name="port">实例端口</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task RegisterInstanceAsync(string serviceName, string ip, int port, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 注销服务实例
    /// </summary>
    /// <param name="serviceName">服务名</param>
    /// <param name="ip">实例IP</param>
    /// <param name="port">实例端口</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeregisterInstanceAsync(string serviceName, string ip, int port, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取所有服务实例
    /// </summary>
    /// <param name="serviceName">服务名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实例列表</returns>
    Task<List<Host>> GetAllInstancesAsync(string serviceName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 监听服务实例变更
    /// </summary>
    /// <param name="serviceName">服务名</param>
    /// <param name="callback">变更回调</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ListenInstanceAsync(string serviceName, Action<List<Host>> callback, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 取消监听服务实例变更
    /// </summary>
    /// <param name="serviceName">服务名</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task UnlistenInstanceAsync(string serviceName, CancellationToken cancellationToken = default);
    
    /* 命名空间管理 */
    /// <summary>
    /// 创建命名空间
    /// </summary>
    /// <param name="namespaceName">命名空间名称</param>
    /// <param name="namespaceDesc">命名空间描述</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task CreateNamespaceAsync(string namespaceName, string namespaceDesc, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除命名空间
    /// </summary>
    /// <param name="namespaceId">命名空间ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteNamespaceAsync(string namespaceId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 修改命名空间
    /// </summary>
    /// <param name="namespaceId">命名空间ID</param>
    /// <param name="namespaceName">命名空间名称</param>
    /// <param name="namespaceDesc">命名空间描述</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyNamespaceAsync(string namespaceId, string namespaceName, string namespaceDesc, CancellationToken cancellationToken = default);
    
    /* 集群管理 */
    /// <summary>
    /// 创建集群
    /// </summary>
    /// <param name="clusterName">集群名称</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="healthChecker">健康检查配置</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task CreateClusterAsync(string clusterName, string serviceName, HealthChecker healthChecker, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除集群
    /// </summary>
    /// <param name="clusterName">集群名称</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteClusterAsync(string clusterName, string serviceName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 修改集群
    /// </summary>
    /// <param name="clusterName">集群名称</param>
    /// <param name="serviceName">服务名称</param>
    /// <param name="healthChecker">健康检查配置</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyClusterAsync(string clusterName, string serviceName, HealthChecker healthChecker, CancellationToken cancellationToken = default);
        Task<string> GetConfigAsync(string dataId, CancellationToken cancellationToken = default);
        Task PublishConfigAsync(string dataId, string content, CancellationToken cancellationToken = default);
        Task RemoveConfigAsync(string dataId, CancellationToken cancellationToken = default);
        Task RegisterInstanceAsync(string serviceName, string ip, int port, CancellationToken cancellationToken = default);
        Task DeregisterInstanceAsync(string serviceName, string ip, int port, CancellationToken cancellationToken = default);
        Task<List<Host>> GetAllInstancesAsync(string serviceName, CancellationToken cancellationToken = default);

        // 多租户支持
        Task SetTenantContextAsync(string tenantId);

        // 弹性策略
        Task ResetCircuitBreakerAsync();

        // 性能优化
        ValueTask<MemoryPoolStatistics> GetMemoryPoolAsync();

        // 高级监控
        Task<ConnectionStatistics> GetConnectionStatsAsync();
        Task<ThroughputStatistics> GetThroughputStatsAsync();
    }

    public class NacosService : INacosService, IDisposable
{
    // 配置变更监听器字典
    private readonly ConcurrentDictionary<string, IConfigListener> _configListeners = new();
    
    // 服务实例变更监听器字典
    private readonly ConcurrentDictionary<string, IEventListener> _instanceListeners = new();
        private readonly INacosConfigService _configService;
        private readonly INacosNamingService _namingService;
        private readonly IOptions<NacosOptions> _options;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly ICircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly ObjectPool<Memory<byte>> _memoryPool;
        private string _currentTenantId;

        public NacosService(
            INacosConfigService configService,
            INacosNamingService namingService,
            IOptions<NacosOptions> options,
            IAsyncPolicy retryPolicy,
            ICircuitBreakerPolicy circuitBreakerPolicy,
            ObjectPool<Memory<byte>> memoryPool)
        {
            _configService = configService;
            _namingService = namingService;
            _options = options;
            _retryPolicy = retryPolicy;
            _circuitBreakerPolicy = circuitBreakerPolicy;
            _memoryPool = memoryPool;
        }

        public async Task<string> GetConfigAsync(string dataId, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    return await _configService.GetConfig(dataId, _options.Value.GroupName, 3000);
                });
            });
        }

        public async Task PublishConfigAsync(string dataId, string content, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    await _configService.PublishConfig(dataId, _options.Value.GroupName, content);
                    return "OK";
                });
            });
        }

        public async Task RemoveConfigAsync(string dataId, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    await _configService.RemoveConfig(dataId, _options.Value.GroupName);
                    return "OK";
                });
            });
        }

        public async Task RegisterInstanceAsync(string serviceName, string ip, int port, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    await _namingService.RegisterInstance(serviceName, ip, port, _options.Value.ClusterName);
                    return "OK";
                });
            });
        }

        public async Task DeregisterInstanceAsync(string serviceName, string ip, int port, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    await _namingService.DeregisterInstance(serviceName, ip, port, _options.Value.ClusterName);
                    return "OK";
                });
            });
        }

        public async Task<List<Host>> GetAllInstancesAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    return await _namingService.GetAllInstances(serviceName);
                });
            });
        }

        // 多租户支持
        public async Task SetTenantContextAsync(string tenantId)
        {
            _currentTenantId = tenantId;
            await Task.CompletedTask;
        }

        // 弹性策略
        public async Task ResetCircuitBreakerAsync()
        {
            if (_circuitBreakerPolicy is ICircuitBreakerPolicy policy)
            {
                policy.Reset();
            }
            await Task.CompletedTask;
        }

        // 性能优化
        public ValueTask<MemoryPoolStatistics> GetMemoryPoolAsync()
        {
            var stats = new MemoryPoolStatistics
            {
                TotalMemory = _memoryPool.GetType().GetProperty("TotalMemory")?.GetValue(_memoryPool) as long? ?? 0,
                AvailableMemory = _memoryPool.GetType().GetProperty("AvailableMemory")?.GetValue(_memoryPool) as long? ?? 0
            };
            return ValueTask.FromResult(stats);
        }

        // 高级监控
        public Task<ConnectionStatistics> GetConnectionStatsAsync()
        {
            return Task.FromResult(new ConnectionStatistics
            {
                ActiveConnections = 0,
                TotalConnections = 0,
                FailedConnections = 0
            });
        }

        public Task<ThroughputStatistics> GetThroughputStatsAsync()
        {
            return Task.FromResult(new ThroughputStatistics
            {
                RequestsPerSecond = 0,
                BytesSentPerSecond = 0,
                BytesReceivedPerSecond = 0
            });
        }

        public async Task ListenConfigAsync(string dataId, Action<string> callback, CancellationToken cancellationToken = default)
{
    var listener = new ConfigListener(callback);
    _configListeners[dataId] = listener;
    await _configService.AddListener(dataId, _options.Value.GroupName, listener);
}

public async Task UnlistenConfigAsync(string dataId, CancellationToken cancellationToken = default)
{
    if (_configListeners.TryRemove(dataId, out var listener))
    {
        await _configService.RemoveListener(dataId, _options.Value.GroupName, listener);
    }
}

public async Task ListenInstanceAsync(string serviceName, Action<List<Host>> callback, CancellationToken cancellationToken = default)
{
    var listener = new InstanceListener(callback);
    _instanceListeners[serviceName] = listener;
    await _namingService.Subscribe(serviceName, _options.Value.ClusterName, listener);
}

public async Task UnlistenInstanceAsync(string serviceName, CancellationToken cancellationToken = default)
{
    if (_instanceListeners.TryRemove(serviceName, out var listener))
    {
        await _namingService.Unsubscribe(serviceName, _options.Value.ClusterName, listener);
    }
}

public async Task CreateNamespaceAsync(string namespaceName, string namespaceDesc, CancellationToken cancellationToken = default)
{
    await _namingService.CreateNamespace(namespaceName, namespaceDesc);
}

public async Task DeleteNamespaceAsync(string namespaceId, CancellationToken cancellationToken = default)
{
    await _namingService.DeleteNamespace(namespaceId);
}

public async Task ModifyNamespaceAsync(string namespaceId, string namespaceName, string namespaceDesc, CancellationToken cancellationToken = default)
{
    await _namingService.ModifyNamespace(namespaceId, namespaceName, namespaceDesc);
}

public async Task CreateClusterAsync(string clusterName, string serviceName, HealthChecker healthChecker, CancellationToken cancellationToken = default)
{
    await _namingService.CreateCluster(clusterName, serviceName, healthChecker);
}

public async Task DeleteClusterAsync(string clusterName, string serviceName, CancellationToken cancellationToken = default)
{
    await _namingService.DeleteCluster(clusterName, serviceName);
}

public async Task ModifyClusterAsync(string clusterName, string serviceName, HealthChecker healthChecker, CancellationToken cancellationToken = default)
{
    await _namingService.ModifyCluster(clusterName, serviceName, healthChecker);
}

public void Dispose()
{       {
            (_configService as IDisposable)?.Dispose();
            (_namingService as IDisposable)?.Dispose();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNacosService(this IServiceCollection services, Action<NacosOptions> configureOptions)
        {
            services.Configure(configureOptions);

            // 注册内存池
            services.AddSingleton<ObjectPool<Memory<byte>>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NacosOptions>>().Value;
                return new DefaultObjectPool<Memory<byte>>(
                    new MemoryPooledObjectPolicy(),
                    maximumRetained: options.MemoryPoolSize / 4096);
            });

            // 注册弹性策略
            services.AddSingleton<IAsyncPolicy>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NacosOptions>>().Value;
                return Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        options.RetryCount,
                        _ => options.RetryDelay);
            });

            // 注册熔断器策略
            services.AddSingleton<ICircuitBreakerPolicy>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NacosOptions>>().Value;
                return Policy
                    .Handle<Exception>()
                    .CircuitBreakerAsync(
                        options.CircuitBreakerThreshold,
                        options.CircuitBreakerDuration);
            });

            // 注册Nacos客户端
            services.AddNacosV2Config(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NacosOptions>>().Value;
                return new NacosSdkOptions
                {
                    ServerAddresses = new List<string> { options.ServerAddresses },
                    Namespace = options.Namespace,
                    DefaultTimeOut = 8000,
                    ListenInterval = 30000
                };
            });

            services.AddNacosV2Naming(sp =>
            {
                var options = sp.GetRequiredService<IOptions<NacosOptions>>().Value;
                return new NacosSdkOptions
                {
                    ServerAddresses = new List<string> { options.ServerAddresses },
                    Namespace = options.Namespace,
                    DefaultTimeOut = 8000
                };
            });

            // 注册OpenTelemetry监控
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddMeter("NacosService")
                    .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(
                        sp.GetRequiredService<IOptions<NacosOptions>>().Value.TracingSamplingInterval.TotalSeconds))))
                .WithTracing(tracing => tracing
                    .AddSource("NacosService"));

            services.AddSingleton<INacosService, NacosService>();
            return services;
        }
    }
}