#:sdk Microsoft.NET.Sdk.Web
#:package Gradio.NET@1.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using Gradio.NET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace GradioNetIntegration
{
    public class GradioNetOptions
{
    public string ServerUrl { get; set; } = "http://localhost:7860";
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

    public interface IGradioNetService
{
    Task<string> CreateInterfaceAsync(Func<Inputs, Outputs> function, string title = "Tech Demo");
    Task SetTenantContextAsync(string tenantId);
    Task ResetCircuitBreakerAsync();
    ValueTask<IMemoryOwner<byte>> GetMemoryPoolAsync(int size);
    Task<ConnectionStatistics> GetConnectionStatisticsAsync();
    Task<ThroughputStatistics> GetThroughputStatisticsAsync();
    Task<HealthCheckResult> CheckHealthAsync();
    }

    public class GradioNetService : IGradioNetService
    {
        private readonly GradioNetOptions _options;
        private readonly ILogger<GradioNetService> _logger;
        private readonly TenantContext _tenantContext;
        private readonly IAsyncPolicy _resiliencePolicy;

        public GradioNetService(
            IOptions<GradioNetOptions> options,
            ILogger<GradioNetService> logger,
            TenantContext tenantContext)
        {
            _options = options.Value;
            _logger = logger;
            _tenantContext = tenantContext;

            _resiliencePolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(_options.RetryCount, 
                    attempt => _options.RetryInterval,
                    (ex, _) => _logger.LogWarning(ex, "Retrying Gradio.NET operation"));
        }

        public async Task<string> CreateInterfaceAsync(Func<Inputs, Outputs> function, string title = "Tech Demo")
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
            {
                using var activity = DiagnosticsConfig.ActivitySource.StartActivity("GradioNet.CreateInterface");
                
                var iface = await Gradio.Interface(
                    function,
                    inputs: new [] { "text" },
                    outputs: new [] { "text" },
                    title: title);
                
                return iface.Launch(_options.ServerUrl);
            });
        }

        public Task SetTenantContextAsync(string tenantId)
        {
            _tenantContext.TenantId = tenantId;
            return Task.CompletedTask;
        }

        public Task ResetCircuitBreakerAsync()
        {
            // Implementation for circuit breaker reset
            return Task.CompletedTask;
        }

        public Task<ConnectionStatistics> GetConnectionStatisticsAsync()
        {
            // Implementation for connection statistics
            return Task.FromResult(new ConnectionStatistics());
        }

        public Task<ThroughputStatistics> GetThroughputStatisticsAsync()
        {
            // Implementation for throughput statistics
            return Task.FromResult(new ThroughputStatistics());
        }

        public Task<HealthCheckResult> CheckHealthAsync()
        {
            // Implementation for health check
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }

    public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGradioNetService(this IServiceCollection services, Action<GradioNetOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IGradioNetService, GradioNetService>();
        services.AddSingleton<TenantContext>();
        
        services.AddPolicyRegistry((sp, registry) =>
        {
            var options = sp.GetRequiredService<IOptions<GradioNetOptions>>().Value;
            
            registry.Add("RetryPolicy", Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(options.RetryCount, _ => options.RetryInterval));
                
            registry.Add("CircuitBreakerPolicy", Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(options.CircuitBreakerThreshold, options.CircuitBreakerDuration));
        });
        
        services.AddSingleton<ObjectPool<Memory<byte>>>(sp => 
            new DefaultObjectPool<Memory<byte>>(
                new MemoryPooledPolicy(), 
                sp.GetRequiredService<IOptions<GradioNetOptions>>().Value.MemoryPoolSize));
                
        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter("GradioNet.Metrics"))
            .WithTracing(tracing => tracing
                .AddSource("GradioNet.Tracing"));
                
        services.AddHealthChecks()
            .AddCheck<GradioNetHealthCheck>("gradionet", 
                failureStatus: HealthStatus.Degraded, 
                tags: new[] { "gradio" });
                
        return services;
    }
    }

    // Supporting classes
    public class TenantContext
    {
        public string TenantId { get; set; }
    }

    public class ConnectionStatistics { }
    public class ThroughputStatistics { }
    
    public class GradioNetHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
    
    public static class DiagnosticsConfig
    {
        public static readonly ActivitySource ActivitySource = new ActivitySource("GradioNet");
    }
}

// Example usage
var builder = WebApplication.CreateBuilder();
builder.Services.AddGradioNetService(options =>
{
    options.ServerUrl = "http://localhost:7860";
    options.MultiTenantEnabled = true;
    options.MetricsEnabled = true;
    options.TracingEnabled = true;
});

var app = builder.Build();

// Example endpoint to create a Gradio interface
app.MapPost("/create-interface", async (IGradioNetService service) =>
{
    return await service.CreateInterfaceAsync(inputs => 
    {
        // Your function logic here
        return new Outputs { Text = inputs.Text.ToUpper() };
    }, "Text Uppercaser");
});

app.Run();