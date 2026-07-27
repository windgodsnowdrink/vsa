//#:sdk Microsoft.NET.Sdk.Web
//#:package Microsoft.Tye.Extensions.Configuration@0.11.0-alpha.22111.1
//#:property LangVersion=preview
//#:property TargetFramework=net11.0
//#:property Nullable=enable
//#:property ImplicitUsings=enable

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Tye;
using System.Threading.Channels;

namespace TyeIntegration
{
    public class TyeOptions
    {
        public string ServiceName { get; set; } = "tye-service";
        public int MaxConcurrentRequests { get; set; } = 1000;
        public bool EnableHealthChecks { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
        public bool EnableTracing { get; set; } = true;
        public int RequestTimeoutMs { get; set; } = 30000;
        public string ConfigurationStoreName { get; set; } = "tye-config";
        public string SecretStoreName { get; set; } = "tye-secrets";
        public int HealthCheckTimeoutMs { get; set; } = 5000;
        public int MetricsCollectionIntervalMs { get; set; } = 10000;
        public string TracingExporter { get; set; } = "jaeger";
    }

    public interface ITyeService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<bool> CheckHealthAsync();
        Task<Dictionary<string, object>> GetMetricsAsync();
        Task<Dictionary<string, string>> GetConfigurationAsync(string key);
        Task<Dictionary<string, string>> GetSecretAsync(string key);
        Task<List<string>> DiscoverServicesAsync(string serviceName);
        Task ReportMetricAsync(string name, double value, Dictionary<string, string> tags = null);
        Task StartActivityAsync(string name, Dictionary<string, object> tags = null);
    }

    public class TyeService : ITyeService, IHostedService
    {
        private readonly TyeOptions _options;
        private readonly Channel<Func<CancellationToken, Task>> _workChannel;
        private readonly IConfiguration _configuration;

        public TyeService(TyeOptions options, IConfiguration configuration)
        {
            _options = options;
            _configuration = configuration;
            _workChannel = Channel.CreateBounded<Func<CancellationToken, Task>>(
                new BoundedChannelOptions(_options.MaxConcurrentRequests)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = true,
                    SingleWriter = false
                });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Start processing work items
            _ = Task.Run(async () =>
            {
                await foreach (var workItem in _workChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    try
                    {
                        await workItem(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // Handle exception
                    }
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _workChannel.Writer.Complete();
            return Task.CompletedTask;
        }

        public Task<bool> CheckHealthAsync()
        {
            return Task.FromResult(true);
        }

        public Task<Dictionary<string, object>> GetMetricsAsync()
        {
            var metrics = new Dictionary<string, object>
            {
                ["current_requests"] = _workChannel.Reader.Count,
                ["max_concurrent_requests"] = _options.MaxConcurrentRequests
            };
            return Task.FromResult(metrics);
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTyeService(this IServiceCollection services, Action<TyeOptions> configure)
        {
            var options = new TyeOptions();
            configure?.Invoke(options);
            
            services.AddSingleton(options);
            services.AddSingleton<ITyeService, TyeService>();
            services.AddHostedService<TyeService>();
            
            // Add OpenTelemetry for metrics and tracing
            services.AddOpenTelemetry()
                .WithMetrics(builder => builder
                    .AddMeter("TyeService")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(options.ServiceName))
                    .AddOtlpExporter())
                .WithTracing(builder => builder
                    .AddSource("TyeService")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(options.ServiceName))
                    .AddOtlpExporter());
            
            // Add health checks
            services.AddHealthChecks()
                .AddCheck<TyeService>("tye-service-health");
            
            // Add configuration and secret clients
            services.AddTyeConfigurationClient();
            services.AddTyeSecretClient();
            
            return services;
        }
    }
}