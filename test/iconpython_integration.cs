#:sdk Microsoft.NET.Sdk.Web
#:package Python.Runtime@3.10.0
#:package Microsoft.Extensions.Options@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Polly@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Python.Runtime;

namespace IconPythonIntegration
{
    public class IconPythonOptions
{
    public string PythonPath { get; set; } = "/usr/bin/python3";
    public bool MultiTenantEnabled { get; set; } = false;
    public string TenantHeaderName { get; set; } = "X-Tenant-ID";
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(1);
    public double FailureThreshold { get; set; } = 0.5;
    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromMinutes(1);
    public int BufferSize { get; set; } = 8192;
    public bool ZeroCopyEnabled { get; set; } = true;
    public int MemoryPoolSize { get; set; } = 1024 * 1024 * 100;
    public bool MetricsEnabled { get; set; } = true;
    public bool TracingEnabled { get; set; } = true;
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);
        public bool MultiTenantEnabled { get; set; } = false;
        public string TenantHeaderName { get; set; } = "X-Tenant-ID";
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(1);
        public double FailureThreshold { get; set; } = 0.5;
        public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan MinimumThroughput { get; set; } = TimeSpan.FromSeconds(10);
        public int BufferSize { get; set; } = 8192;
        public bool ZeroCopyEnabled { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024;
        public bool MetricsEnabled { get; set; } = true;
        public bool TracingEnabled { get; set; } = true;
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
    }

    public interface IIconPythonService
    {
        Task<dynamic> ExecutePythonAsync(string script, CancellationToken cancellationToken = default);
        Task<T> ExecutePythonAsync<T>(string script, CancellationToken cancellationToken = default);
        Task SetTenantContextAsync(string tenantId, CancellationToken cancellationToken = default);
        Task ResetCircuitBreakerAsync(CancellationToken cancellationToken = default);
        ValueTask<IMemoryOwner<byte>> GetMemoryPoolAsync(int size, CancellationToken cancellationToken = default);
        Task<ConnectionStatistics> GetConnectionStatisticsAsync(CancellationToken cancellationToken = default);
        Task<ThroughputStatistics> GetThroughputStatisticsAsync(CancellationToken cancellationToken = default);
        Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    }

    public class IconPythonService : IIconPythonService, IDisposable
    {
        private readonly IconPythonOptions _options;
        private readonly IObjectPool<byte[]> _memoryPool;
        private readonly IAsyncPolicy<dynamic> _resiliencePolicy;
        private readonly PyModule _scope;
        private bool _disposed;

        public IconPythonService(IOptions<IconPythonOptions> options, IObjectPool<byte[]> memoryPool)
        {
            _options = options.Value;
            _memoryPool = memoryPool;
            
            PythonEngine.Initialize();
            PythonEngine.BeginAllowThreads();
            _scope = Py.CreateScope();
            
            _resiliencePolicy = Policy<dynamic>
                .Handle<PythonException>()
                .Or<Exception>()
                .WaitAndRetryAsync(_options.RetryCount, 
                    attempt => _options.RetryInterval,
                    (exception, delay, retryCount, context) => 
                    {
                        // Log retry attempts
                    });
        }

        public async Task<dynamic> ExecutePythonAsync(string script, CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async (ct) =>
            {
                using (Py.GIL())
                {
                    return await Task.Run(() => _scope.Eval(script), ct);
                }
            }, cancellationToken);
        }

        public async Task<T> ExecutePythonAsync<T>(string script, CancellationToken cancellationToken = default)
        {
            dynamic result = await ExecutePythonAsync(script, cancellationToken);
            return (T)result;
        }

        public async Task SetTenantContextAsync(string tenantId, CancellationToken cancellationToken = default)
        {
            if (_options.MultiTenantEnabled)
            {
                await ExecutePythonAsync($"tenant_id = '{tenantId}'", cancellationToken);
            }
        }

        public Task ResetCircuitBreakerAsync(CancellationToken cancellationToken = default)
        {
            // Implementation for resetting circuit breaker
            return Task.CompletedTask;
        }

        public ValueTask<IMemoryOwner<byte>> GetMemoryPoolAsync(int size, CancellationToken cancellationToken = default)
        {
            var buffer = _memoryPool.Get();
            return new ValueTask<IMemoryOwner<byte>>(new MemoryOwner(buffer, _memoryPool));
        }

        public Task<ConnectionStatistics> GetConnectionStatisticsAsync(CancellationToken cancellationToken = default)
        {
            // Implementation for connection statistics
            return Task.FromResult(new ConnectionStatistics());
        }

        public Task<ThroughputStatistics> GetThroughputStatisticsAsync(CancellationToken cancellationToken = default)
        {
            // Implementation for throughput statistics
            return Task.FromResult(new ThroughputStatistics());
        }

        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
        {
            // Implementation for health check
            return Task.FromResult(HealthCheckResult.Healthy());
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _scope?.Dispose();
                PythonEngine.Shutdown();
                _disposed = true;
            }
        }

        private class MemoryOwner : IMemoryOwner<byte>
        {
            private readonly byte[] _array;
            private readonly IObjectPool<byte[]> _pool;

            public MemoryOwner(byte[] array, IObjectPool<byte[]> pool)
            {
                _array = array;
                _pool = pool;
                Memory = new Memory<byte>(_array);
            }

            public Memory<byte> Memory { get; }

            public void Dispose()
            {
                _pool.Return(_array);
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIconPythonService(this IServiceCollection services, Action<IconPythonOptions> configureOptions)
        {
            services.Configure(configureOptions);
            
            services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddMeter("IconPython");
                    metrics.AddPrometheusExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource("IconPython");
                    tracing.AddOtlpExporter();
                });
            
            services.AddHealthChecks().AddCheck<IconPythonHealthCheck>("iconpython");
            
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<IOptions<IconPythonOptions>>().Value;
                var poolProvider = provider.GetRequiredService<ObjectPoolProvider>();
                return poolProvider.Create(new DefaultPooledObjectPolicy<byte[]>(), options.MemoryPoolSize);
            });
            
            services.AddPolicyRegistry();
            services.AddResiliencePipeline<string, dynamic>("iconpython-retry-pipeline", builder =>
            {
                builder.AddRetry(new()
                {
                    MaxRetryAttempts = 3,
                    ShouldHandle = new PredicateBuilder<dynamic>()
                        .Handle<PythonException>()
                        .Handle<Exception>(),
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential
                });
                builder.AddCircuitBreaker(new()
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromMinutes(1),
                    MinimumThroughput = 10,
                    BreakDuration = TimeSpan.FromSeconds(30),
                    ShouldHandle = new PredicateBuilder<dynamic>()
                        .Handle<PythonException>()
                        .Handle<Exception>()
                });
            });
            
            services.AddScoped<TenantContext>();
            services.AddScoped<IIconPythonService, IconPythonService>();
            
            return services;
        }
    }

    public class IconPythonHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Implementation for health check
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }

    public class ConnectionStatistics { }
    public class ThroughputStatistics { }
    public class TenantContext { }
}