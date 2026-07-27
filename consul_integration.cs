#:sdk Microsoft.NET.Sdk.Web
#:package Consul@1.6.10.6
#:package Microsoft.Extensions.Http.Polly@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package OpenTelemetry.Extensions.Hosting@1.6.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.ObjectPool;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Polly;

namespace ConsulIntegration
{
    public class ConsulOptions
{
    public string Address { get; set; } = "http://localhost:8500";
    public string ServiceName { get; set; } = "MyService";
    public string ServiceId { get; set; } = Guid.NewGuid().ToString();
    public string[] Tags { get; set; } = Array.Empty<string>();
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromSeconds(10);
    public string HealthCheckEndpoint { get; set; } = "/health";
    public bool MultiTenantEnabled { get; set; }
    public string TenantHeaderName { get; set; } = "X-Tenant-ID";
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);
    public int CircuitBreakerThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMilliseconds(30000);
    public bool ZeroCopyEnabled { get; set; } = true;
    public int MemoryPoolSize { get; set; } = 1024 * 1024;
    public TimeSpan MetricsInterval { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan TracingSamplingInterval { get; set; } = TimeSpan.FromSeconds(10);
    }

    public interface IConsulService
{
    Task RegisterServiceAsync(CancellationToken cancellationToken = default);
    Task DeregisterServiceAsync(CancellationToken cancellationToken = default);
    Task<CatalogService[]> GetServiceInstancesAsync(string serviceName, CancellationToken cancellationToken = default);
    Task<HealthCheck[]> GetHealthChecksAsync(CancellationToken cancellationToken = default);
    Task SetTenantContextAsync(string tenantId);
    Task ResetCircuitBreakerAsync();
    Task<MemoryPoolStats> GetMemoryPoolAsync();
    Task<ConnectionStats> GetConnectionStatsAsync();
    Task<ThroughputStats> GetThroughputStatsAsync();
    }

    public class ConsulService : IConsulService, IDisposable
{
    private readonly IConsulClient _consulClient;
    private readonly ConsulOptions _options;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly ICircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly ITenantContext _tenantContext;
    private readonly IMeter _meter;
    private readonly ITracer _tracer;
    private string _currentTenantId = string.Empty;

        public ConsulService(
            IConsulClient consulClient,
            IOptions<ConsulOptions> options,
            IAsyncPolicy policy,
            ObjectPool<Memory<byte>> memoryPool,
            ITenantContext tenantContext,
            IMeterProvider meterProvider,
            TracerProvider tracerProvider)
        {
            _consulClient = consulClient;
            _options = options.Value;
            _policy = policy;
            _memoryPool = memoryPool;
            _tenantContext = tenantContext;
            _meter = meterProvider.GetMeter("ConsulIntegration");
            _tracer = tracerProvider.GetTracer("ConsulIntegration");
        }

        public async Task RegisterServiceAsync(CancellationToken cancellationToken = default)
        {
            using var activity = _tracer.StartActivity("ConsulService.RegisterService");
            
            var registration = new AgentServiceRegistration
            {
                ID = _options.ServiceId,
                Name = _options.ServiceName,
                Tags = _options.Tags,
                Port = 80,
                Check = new AgentServiceCheck
                {
                    HTTP = _options.HealthCheckEndpoint,
                    Interval = TimeSpan.FromSeconds(_options.HealthCheckIntervalSeconds)
                }
            };

            await _policy.ExecuteAsync(async () => 
            {
                var result = await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new ConsulException("Failed to register service with Consul");
                }
            });
        }

        public async Task DeregisterServiceAsync(CancellationToken cancellationToken = default)
        {
            using var activity = _tracer.StartActivity("ConsulService.DeregisterService");
            
            await _policy.ExecuteAsync(async () => 
            {
                var result = await _consulClient.Agent.ServiceDeregister(_options.ServiceId, cancellationToken);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new ConsulException("Failed to deregister service with Consul");
                }
            });
        }

        public async Task<CatalogService[]> GetServiceInstancesAsync(string serviceName, CancellationToken cancellationToken = default)
        {
            using var activity = _tracer.StartActivity("ConsulService.GetServiceInstances");
            
            return await _policy.ExecuteAsync(async () => 
            {
                var result = await _consulClient.Catalog.Service(serviceName, cancellationToken);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new ConsulException("Failed to get service instances from Consul");
                }
                return result.Response;
            });
        }

        public async Task<HealthCheck[]> GetHealthChecksAsync(CancellationToken cancellationToken = default)
        {
            using var activity = _tracer.StartActivity("ConsulService.GetHealthChecks");
            
            return await _policy.ExecuteAsync(async () => 
            {
                var result = await _consulClient.Health.Checks(_options.ServiceName, cancellationToken);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new ConsulException("Failed to get health checks from Consul");
                }
                return result.Response;
            });
        }

        public Task SetTenantContextAsync(string tenantId)
        {
            _tenantContext.CurrentTenant = tenantId;
            return Task.CompletedTask;
        }

        public Task ResetCircuitBreakerAsync()
        {
            if (_policy is AsyncCircuitBreakerPolicy circuitBreaker)
            {
                circuitBreaker.Reset();
            }
            return Task.CompletedTask;
        }

        public Task<ObjectPool<Memory<byte>>> GetMemoryPoolAsync() => Task.FromResult(_memoryPool);

        public Task<ConsulConnectionStats> GetConnectionStatsAsync()
        {
            // 实现连接统计逻辑
            return Task.FromResult(new ConsulConnectionStats());
        }

        public Task<ConsulThroughputStats> GetThroughputStatsAsync()
        {
            // 实现吞吐量统计逻辑
            return Task.FromResult(new ConsulThroughputStats());
        }

        public void Dispose()
        {
            _consulClient?.Dispose();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsulService(this IServiceCollection services, Action<ConsulOptions> configureOptions)
{
    services.Configure(configureOptions);
    
    services.AddSingleton<IConsulClient>(sp => new ConsulClient(config =>
    {
        var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;
        config.Address = new Uri(options.Address);
    }));
    
    services.AddSingleton<ObjectPool<Memory<byte>>>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;
        var policy = new DefaultPooledObjectPolicy<Memory<byte>>()
        {
            Create = () => new Memory<byte>(new byte[options.MemoryPoolSize])
        };
        return new DefaultObjectPool<Memory<byte>>(policy);
    });
    
    services.AddSingleton<IAsyncPolicy>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(options.RetryCount, _ => options.RetryDelay);
    });
    
    services.AddSingleton<ICircuitBreakerPolicy>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<ConsulOptions>>().Value;
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(options.CircuitBreakerThreshold, options.CircuitBreakerDuration);
    });
    
    services.AddSingleton<IConsulService, ConsulService>();
    
    services.AddOpenTelemetry()
        .WithMetrics(metrics => metrics
            .AddMeter("ConsulIntegration")
            .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1))));
    
    return services;
        }
    }

    public class ConsulException : Exception
    {
        public ConsulException(string message) : base(message) { }
    }

    public class ConsulConnectionStats { }
    public class ConsulThroughputStats { }
    public interface ITenantContext { string CurrentTenant { get; set; } }
}