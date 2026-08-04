#:sdk Microsoft.NET.Sdk
#:package dotnet-etcd@7.0.0
#:package Microsoft.Extensions.Options.ConfigurationExtensions@8.0.0
#:package Polly@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package OpenTelemetry.Extensions.Hosting@1.8.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Collections.Generic;
using dotnet_etcd;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Microsoft.Extensions.ObjectPool;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace EtcdIntegration
{
    public class EtcdOptions
    {
        public string[] Endpoints { get; set; } = new[] { "http://localhost:2379" };
        public string Prefix { get; set; } = "etcd/";
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        public bool MultiTenantEnabled { get; set; }
        public string TenantHeaderName { get; set; } = "X-Tenant-ID";
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(200);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
        public bool ZeroCopyEnabled { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024 * 10; // 10MB
        public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan TracingSamplingInterval { get; set; } = TimeSpan.FromSeconds(10);
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
        public bool ZeroCopyEnabled { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024; // 1MB
    }

    public interface IEtcdService
    {
        Task PutAsync(string key, string value, CancellationToken cancellationToken = default);
        Task<string> GetAsync(string key, CancellationToken cancellationToken = default);
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);
        Task<IEnumerable<KeyValuePair<string, string>>> GetRangeAsync(string prefix, CancellationToken cancellationToken = default);
        Task WatchAsync(string key, Action<string, string> onChange, CancellationToken cancellationToken = default);
        
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

    public class EtcdService : IEtcdService, IDisposable
    {
        private readonly EtcdClient _client;
        private readonly EtcdOptions _options;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly ICircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly ObjectPool<Memory<byte>> _memoryPool;
        private readonly IMeter _meter;
        private readonly ITracer _tracer;

        public EtcdService(
            IOptions<EtcdOptions> options,
            ObjectPool<Memory<byte>> memoryPool,
            IAsyncPolicy retryPolicy,
            ICircuitBreakerPolicy circuitBreakerPolicy,
            IMeter meter,
            ITracer tracer)
        {
            _options = options.Value;
            _memoryPool = memoryPool;
            _retryPolicy = retryPolicy;
            _circuitBreakerPolicy = circuitBreakerPolicy;
            _meter = meter;
            _tracer = tracer;
            _client = new EtcdClient(_options.Endpoints);
        }

        public async Task PutAsync(string key, string value, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var activity = _tracer.StartActivity("Etcd.Put");
                await _client.PutAsync(_options.Prefix + key, value);
            });
        }

        public async Task<string> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using var activity = _tracer.StartActivity("Etcd.Get");
                var response = await _client.GetAsync(_options.Prefix + key);
                return response;
            });
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var activity = _tracer.StartActivity("Etcd.Delete");
                await _client.DeleteAsync(_options.Prefix + key);
            });
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetRangeAsync(string prefix, CancellationToken cancellationToken = default)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                using var activity = _tracer.StartActivity("Etcd.GetRange");
                var response = await _client.GetRangeAsync(_options.Prefix + prefix);
                return response;
            });
        }

        public async Task WatchAsync(string key, Action<string, string> onChange, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                _client.Watch(_options.Prefix + key, (watchResponse) =>
                {
                    foreach (var eventItem in watchResponse.Events)
                    {
                        onChange(eventItem.Key, eventItem.Value);
                    }
                }, cancellationToken);
            }, cancellationToken);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEtcdService(this IServiceCollection services, Action<EtcdOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            // 注册内存池
            services.AddSingleton<ObjectPool<Memory<byte>>>(sp => 
            {
                var options = sp.GetRequiredService<IOptions<EtcdOptions>>().Value;
                return new DefaultObjectPool<Memory<byte>>(
                    new MemoryPooledObjectPolicy(), 
                    maximumRetained: options.MemoryPoolSize / 4096);
            });
            
            // 注册弹性策略
            services.AddSingleton<IAsyncPolicy>(sp => 
            {
                var options = sp.GetRequiredService<IOptions<EtcdOptions>>().Value;
                return Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        options.RetryCount, 
                        _ => options.RetryDelay);
            });
            
            // 注册熔断器策略
            services.AddSingleton<ICircuitBreakerPolicy>(sp => 
            {
                var options = sp.GetRequiredService<IOptions<EtcdOptions>>().Value;
                return Policy
                    .Handle<Exception>()
                    .CircuitBreakerAsync(
                        options.CircuitBreakerThreshold, 
                        options.CircuitBreakerDuration);
            });
            
            // 注册OpenTelemetry监控
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddMeter("EtcdService")
                    .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(
                        sp.GetRequiredService<IOptions<EtcdOptions>>().Value.TracingSamplingInterval.TotalSeconds))))
                .WithTracing(tracing => tracing
                    .AddSource("EtcdService"));
            
            services.AddSingleton<IEtcdService, EtcdService>();
            return services;
        }
    }
}