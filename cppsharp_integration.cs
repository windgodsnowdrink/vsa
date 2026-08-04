#:sdk Microsoft.NET.Sdk.Web
#:package CppSharp@0.9.1
#:package Microsoft.Extensions.Options@8.0.0
#:package Polly@8.3.1
#:package System.Memory@4.5.5
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System;
using System.Buffers;
using System.Threading.Tasks;
using CppSharp;
using CppSharp.AST;
using CppSharp.Passes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;

namespace CppSharpIntegration
{
    public class CppSharpOptions
    {
        public string LibraryPath { get; set; } = string.Empty;
        public string[] IncludeDirs { get; set; } = Array.Empty<string>();
        public string[] LibraryDirs { get; set; } = Array.Empty<string>();
        public bool MultiTenantEnabled { get; set; } = false;
        public string TenantHeaderName { get; set; } = "X-Tenant-Id";
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(1);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromMinutes(1);
        public int BufferSize { get; set; } = 8192;
        public bool ZeroCopyEnabled { get; set; } = true;
        public int MemoryPoolSize { get; set; } = 1024 * 1024;
        public bool MetricsEnabled { get; set; } = true;
        public bool TracingEnabled { get; set; } = true;
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);
    }

    public interface ICppSharpService
{
    Task<string> ParseAndGenerateBindingsAsync(string code);
    Task SetTenantContextAsync(string tenantId);
    Task ResetCircuitBreakerAsync();
    Task<MemoryPoolStatistics> GetMemoryPoolAsync();
    Task<ConnectionStatistics> GetConnectionStatisticsAsync();
    Task<ThroughputStatistics> GetThroughputStatisticsAsync();
    Task<HealthCheckResult> CheckHealthAsync();
        Task SetTenantContextAsync(string tenantId);
        Task ResetCircuitBreakerAsync();
        ValueTask<IMemoryOwner<byte>> GetMemoryPoolAsync(int size);
        Task<ConnectionStatistics> GetConnectionStatisticsAsync();
        Task<ThroughputStatistics> GetThroughputStatisticsAsync();
        Task<HealthCheckResult> CheckHealthAsync();
    }

    public class CppSharpService : ICppSharpService
    {
        private readonly CppSharpOptions _options;
        private readonly TenantContext _tenantContext;
        private readonly IAsyncPolicy _resiliencyPolicy;
        private readonly ObjectPool<Memory<byte>> _memoryPool;

        public CppSharpService(
            IOptions<CppSharpOptions> options,
            TenantContext tenantContext,
            IReadOnlyPolicyRegistry<string> policyRegistry,
            ObjectPool<Memory<byte>> memoryPool)
        {
            _options = options.Value;
            _tenantContext = tenantContext;
            _resiliencyPolicy = policyRegistry.Get<IAsyncPolicy>("CppSharpPolicy");
            _memoryPool = memoryPool;
        }

        public async Task<string> ParseAndGenerateBindingsAsync(string headerFile)
        {
            return await _resiliencyPolicy.ExecuteAsync(async () =>
            {
                using var driver = new Driver(new DriverOptions
                {
                    OutputDir = "Bindings",
                    IncludeDirs = _options.IncludeDirs,
                    LibraryDirs = _options.LibraryDirs
                });

                var module = driver.Options.AddModule("Bindings");
                module.Headers.Add(headerFile);
                module.LibraryName = _options.LibraryPath;

                driver.RunGenerator();
                return "Bindings generated successfully";
            });
        }

        public Task SetTenantContextAsync(string tenantId)
        {
            _tenantContext.CurrentTenant = tenantId;
            return Task.CompletedTask;
        }

        public Task ResetCircuitBreakerAsync()
        {
            if (_resiliencyPolicy is AsyncCircuitBreakerPolicy cbPolicy)
            {
                cbPolicy.Reset();
            }
            return Task.CompletedTask;
        }

        public ValueTask<IMemoryOwner<byte>> GetMemoryPoolAsync(int size)
        {
            var memory = _memoryPool.Get();
            return new ValueTask<IMemoryOwner<byte>>(memory);
        }

        public Task<ConnectionStatistics> GetConnectionStatisticsAsync() => Task.FromResult(new ConnectionStatistics());
        public Task<ThroughputStatistics> GetThroughputStatisticsAsync() => Task.FromResult(new ThroughputStatistics());
        public Task<HealthCheckResult> CheckHealthAsync() => Task.FromResult(HealthCheckResult.Healthy());
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCppSharpService(this IServiceCollection services, Action<CppSharpOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddSingleton<ICppSharpService, CppSharpService>();
            services.AddSingleton<TenantContext>();
            
            services.AddPolicyRegistry((sp, registry) =>
            {
                var options = sp.GetRequiredService<IOptions<CppSharpOptions>>().Value;
                
                registry.Add("CppSharpPolicy", Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(options.RetryCount, _ => options.RetryInterval)
                    .WrapAsync(Policy
                        .Handle<Exception>()
                        .CircuitBreakerAsync(options.CircuitBreakerThreshold, options.CircuitBreakerDuration)));
            });
            
            services.AddSingleton<ObjectPool<Memory<byte>>>(sp => 
                new DefaultObjectPool<Memory<byte>>(
                    new MemoryPooledPolicy(), 
                    sp.GetRequiredService<IOptions<CppSharpOptions>>().Value.MemoryPoolSize));
                
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddMeter("CppSharp.Metrics"))
                .WithTracing(tracing => tracing
                    .AddSource("CppSharp.Tracing"));
                
            services.AddHealthChecks()
                .AddCheck<CppSharpHealthCheck>("cppsharp", 
                    failureStatus: HealthStatus.Degraded, 
                    tags: new[] { "cpp" });
                
            return services;
        }
    }

    // Helper classes
    public class TenantContext
    {
        public string CurrentTenant { get; set; } = string.Empty;
    }

    public class ConnectionStatistics { }
    public class ThroughputStatistics { }
    
    public class CppSharpHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }

    public class MemoryPooledPolicy : PooledObjectPolicy<Memory<byte>>
    {
        public override Memory<byte> Create() => new byte[1024];
        public override bool Return(Memory<byte> obj) => true;
    }
}