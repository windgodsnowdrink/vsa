#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Hosting@8.0.0
#:package Microsoft.Extensions.Options.ConfigurationExtensions@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace AspireIntegration
{
    public class AspireOptions
    {
        public string ServiceName { get; set; } = "default-service";
        public int MaxConcurrentRequests { get; set; } = 100;
        public bool EnableHealthChecks { get; set; } = true;
        public bool EnableMetrics { get; set; } = true;
        public bool EnableTracing { get; set; } = true;
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public string[] ServiceDiscoveryUrls { get; set; } = Array.Empty<string>();
        public string ConfigurationStoreName { get; set; } = "config-store";
        public string SecretStoreName { get; set; } = "secret-store";
        public int HealthCheckTimeoutSeconds { get; set; } = 10;
        public int MetricsCollectionIntervalSeconds { get; set; } = 30;
        public string TracingExporter { get; set; } = "otlp";
    }

    public interface IAspireService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
        Task<HealthReport> CheckHealthAsync();
        Task<MetricsSnapshot> GetMetricsAsync();
        Task<T> GetConfigurationAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<T> GetSecretAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<IEnumerable<ServiceEndpoint>> DiscoverServicesAsync(string serviceName, CancellationToken cancellationToken = default);
        Task ReportMetricAsync(string name, double value, IDictionary<string, object>? tags = null);
        Task<Activity?> StartActivityAsync(string name, ActivityKind kind = ActivityKind.Internal);
    }

    public class AspireService : IAspireService, IHostedService
    {
        private readonly AspireOptions _options;
        private readonly Channel<Func<CancellationToken, Task>> _workChannel;
        private readonly CancellationTokenSource _cts = new();

        public AspireService(IOptions<AspireOptions> options)
        {
            _options = options.Value;
            _workChannel = Channel.CreateBounded<Func<CancellationToken, Task>>(
                new BoundedChannelOptions(_options.MaxConcurrentRequests)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false
                });
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < _options.MaxConcurrentRequests; i++)
            {
                _ = Task.Run(async () =>
                {
                    while (await _workChannel.Reader.WaitToReadAsync(cancellationToken))
                    {
                        if (_workChannel.Reader.TryRead(out var workItem))
                        {
                            await workItem(cancellationToken);
                        }
                    }
                }, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }

        public Task<HealthReport> CheckHealthAsync()
        {
            return Task.FromResult(new HealthReport { Status = "Healthy" });
        }

        public Task<MetricsSnapshot> GetMetricsAsync()
        {
            return Task.FromResult(new MetricsSnapshot());
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAspireService(this IServiceCollection services, Action<AspireOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<IAspireService, AspireService>();
            services.AddHostedService<AspireService>();
            
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation())
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation());
                    
            services.AddHealthChecks();
            services.AddServiceDiscovery();
            services.AddConfigurationClient();
            services.AddSecretClient();
            
            return services;
        }
    }

    public class HealthReport { public string Status { get; set; } }
    public class MetricsSnapshot { }
}