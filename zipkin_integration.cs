#:sdk Microsoft.NET.Sdk.Web
#:package OpenTelemetry.Exporter.Zipkin@1.7.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public class ZipkinOptions
{
    public string Endpoint { get; set; } = "http://localhost:9411/api/v2/spans";
    public string ServiceName { get; set; } = "MyService";
    public bool MultiTenantEnabled { get; set; }
    public string TenantHeaderName { get; set; } = "X-Tenant-Id";
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMs { get; set; } = 100;
    public int CircuitBreakerThreshold { get; set; } = 5;
    public int CircuitBreakerDurationMs { get; set; } = 30000;
    public bool ZeroCopyEnabled { get; set; } = true;
    public int MemoryPoolSize { get; set; } = 1024 * 1024;
    public bool MetricsEnabled { get; set; } = true;
    public bool TracingEnabled { get; set; } = true;
    public int HealthCheckIntervalMs { get; set; } = 5000;
}

public interface IZipkinService
{
    ActivitySource GetActivitySource();
    Task ExportActivityAsync(Activity activity);
    Task<HealthCheckResult> CheckHealthAsync();
    Task SetTenantContextAsync(string tenantId);
    Task ResetCircuitBreakerAsync();
    Task<ObjectPool<Memory<byte>>> GetMemoryPoolAsync();
    Task<ZipkinConnectionStats> GetConnectionStatsAsync();
    Task<ZipkinThroughputStats> GetThroughputStatsAsync();
}

public class ZipkinService : IZipkinService
{
    private readonly ZipkinOptions _options;
    private readonly TracerProvider _tracerProvider;
    private readonly ActivitySource _activitySource;

    public ZipkinService(
    IOptions<ZipkinOptions> options,
    ObjectPool<Memory<byte>> memoryPool,
    IAsyncPolicy resiliencyPolicy)
{
    _options = options.Value;
    _memoryPool = memoryPool;
    _resiliencyPolicy = resiliencyPolicy;
    _activitySource = new ActivitySource(_options.ServiceName);
        
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(_options.ServiceName)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_options.ServiceName))
            .AddZipkinExporter(zipkinOptions =>
            {
                zipkinOptions.Endpoint = new Uri(_options.Endpoint);
                zipkinOptions.MaxPayloadSizeInBytes = 4096;
                zipkinOptions.UseShortTraceIds = true;
            })
            .SetSampler(new AlwaysOnSampler())
            .Build();
    }

    public ActivitySource GetActivitySource() => _activitySource;

    public async Task ExportActivityAsync(Activity activity)
{
    if (_options.MultiTenantEnabled && !string.IsNullOrEmpty(_tenantContext.CurrentTenant))
    {
        activity.SetTag("tenant.id", _tenantContext.CurrentTenant);
    }

    await _policy.ExecuteAsync(async () =>
    {
        using var memory = _memoryPool.Get();
        var span = memory.Memory.Span;
        
        // 使用零拷贝技术序列化Activity
        if (_options.ZeroCopyEnabled)
        {
            ZeroCopySerialize(activity, ref span);
        }
        else
        {
            // 传统序列化方式
            Serialize(activity, ref span);
        }

        await _httpClient.PostAsync(_options.Endpoint, new ReadOnlyMemoryContent(memory.Memory));
        _memoryPool.Return(memory);
    });
}

    public async Task<HealthCheckResult> CheckHealthAsync()
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(_options.Endpoint.Replace("/api/v2/spans", ""));
            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy() 
                : HealthCheckResult.Unhealthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddZipkinService(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<ZipkinOptions>(configuration.GetSection("Zipkin"));
    
    // Core services
    services.AddSingleton<IZipkinService, ZipkinService>();
    
    // Memory pool
    services.AddSingleton<ObjectPool<Memory<byte>>>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<ZipkinOptions>>();
        var policy = new DefaultPooledObjectPolicy<Memory<byte>>();
        return new DefaultObjectPool<Memory<byte>>(policy, options.Value.MemoryPoolSize);
    });
    
    // Resiliency policy
    services.AddSingleton<IAsyncPolicy>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<ZipkinOptions>>();
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                options.Value.RetryCount, 
                attempt => options.Value.RetryDelay);
    });
        
        services.AddHealthChecks()
            .AddCheck<ZipkinHealthCheck>("zipkin");
            
        return services;
    }
}

public class ZipkinHealthCheck : IHealthCheck
{
    private readonly IZipkinService _zipkinService;
    
    public ZipkinHealthCheck(IZipkinService zipkinService)
    {
        _zipkinService = zipkinService;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await _zipkinService.CheckHealthAsync();
    }
}