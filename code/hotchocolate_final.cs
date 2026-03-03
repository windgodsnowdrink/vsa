#:sdk Microsoft.NET.Sdk.Web
#:package HotChocolate.AspNetCore@13.0.0
#:package Microsoft.Extensions.Configuration@8.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:package OpenTelemetry.Exporter.Prometheus@1.7.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder();

// 1. 缓存分区动态调整
builder.Services.AddSingleton<IDynamicPartitionStrategy>(sp => 
    new AdaptivePartitionStrategy(
        sp.GetRequiredService<IOptionsMonitor<PartitionOptions>>(),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

// 2. 权限策略热更新
builder.Services.AddSingleton<IPermissionPolicyUpdater>(sp => 
    new HotReloadPolicyUpdater(
        sp.GetRequiredService<IOptionsMonitor<PermissionOptions>>(),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[256])));

// 3. 监控数据可视化
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("GraphQL.Metrics")
        .AddPrometheusExporter());

// ... existing code ...

// 自适应分区策略
[SkipLocalsInit]
public class AdaptivePartitionStrategy : IDynamicPartitionStrategy
{
    private readonly IOptionsMonitor<PartitionOptions> _options;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public AdaptivePartitionStrategy(
        IOptionsMonitor<PartitionOptions> options,
        ThreadLocal<Span<byte>> buffer)
    {
        _options = options;
        _buffer = buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe string GetPartitionKey(string baseKey)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // 根据当前负载动态调整分区数
                return $"{baseKey}:{baseKey.GetHashCode() % _options.CurrentValue.PartitionCount}";
            }
        }
        return baseKey;
    }
}

// 权限策略热更新器
[SkipLocalsInit]
public class HotReloadPolicyUpdater : IPermissionPolicyUpdater
{
    private readonly IOptionsMonitor<PermissionOptions> _options;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public HotReloadPolicyUpdater(
        IOptionsMonitor<PermissionOptions> options,
        ThreadLocal<Span<byte>> buffer)
    {
        _options = options;
        _buffer = buffer;
        _options.OnChange(ReloadPolicies);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private unsafe void ReloadPolicies(PermissionOptions options)
    {
        Span<byte> buffer = _buffer.Value;
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0)
            {
                // 热更新权限策略
            }
        }
    }
}

// Prometheus监控端点
app.MapPrometheusScrapingEndpoint();
app.MapGet("/metrics", async context =>
{
    await context.Response.WriteAsync(await MetricsExporter.CollectAsync());
});

public class PartitionOptions
{
    public int PartitionCount { get; set; } = 16;
    public int RebalanceThreshold { get; set; } = 1000;
}

public class PermissionOptions
{
    public Dictionary<string, string[]> Policies { get; set; }
}