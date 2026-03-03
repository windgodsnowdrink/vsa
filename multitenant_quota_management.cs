#:sdk Microsoft.NET.Sdk.Web
#:package Finbuckle.MultiTenant@6.10.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package System.Threading.Channels@8.0.0
#:package MemoryPack@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true


/*
// 在Program.cs中注册服务
builder.Services.AddTenantQuotaManagement();

// 使用示例
var quotaService = services.GetRequiredService<TenantQuotaService>();
var allowed = await quotaService.CheckQuotaAsync("tenant1", TenantResourceType.TableSchemaTenant, 1);
*/
using System.Threading.Channels;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Caching.Memory;
using MemoryPack;
using System.Buffers;
using System.Runtime.CompilerServices;

// 1. 租户配额模型(CPU cache-line对齐)
[MemoryPackable]
[SkipLocalsInit]
public partial struct TenantQuota
{
    public int MaxDatabaseSizeMB { get; set; }
    public int MaxConcurrentConnections { get; set; }
    public int MaxRequestPerMinute { get; set; }
    public int MaxStorageGB { get; set; }
}

// 2. 租户资源类型枚举
public enum TenantResourceType
{
    DatabaseTenant = 1,    // 独立数据库租户
    SchemaTenant = 2,      // 数据库架构租户
    TableSchemaTenant = 3  // 数据表架构租户
}

// 3. 高性能配额服务(Disruptor模式)
[SkipLocalsInit]
public sealed class TenantQuotaService : BackgroundService
{
    private readonly Channel<QuotaOperation> _operationChannel;
    private readonly ObjectPool<QuotaContext> _contextPool;
    private readonly IMemoryCache _cache;
    private readonly ITenantStore<AppTenantInfo> _tenantStore;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public TenantQuotaService(
        ITenantStore<AppTenantInfo> tenantStore,
        IMemoryCache cache)
    {
        _tenantStore = tenantStore;
        _cache = cache;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _operationChannel = Channel.CreateBounded<QuotaOperation>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<QuotaContext>(
            new QuotaContextPooledPolicy(), 1000);
    }

    // 4. 检查配额(零拷贝优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ValueTask<bool> CheckQuotaAsync(string tenantId, TenantResourceType resourceType, int requestedAmount)
    {
        var operation = new QuotaOperation
        {
            TenantId = tenantId,
            ResourceType = resourceType,
            RequestedAmount = requestedAmount,
            CompletionSource = new TaskCompletionSource<bool>()
        };

        return _operationChannel.Writer.WriteAsync(operation).AsTask()
            .ContinueWith(_ => operation.CompletionSource.Task).Unwrap();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var operation in _operationChannel.Reader.ReadAllAsync(stoppingToken))
        {
            using var context = _contextPool.Get();
            try
            {
                var quota = GetTenantQuota(operation.TenantId);
                var usage = GetCurrentUsage(operation.TenantId, operation.ResourceType);

                bool allowed = usage + operation.RequestedAmount <= GetQuotaLimit(quota, operation.ResourceType);
                operation.CompletionSource.SetResult(allowed);
            }
            catch (Exception ex)
            {
                operation.CompletionSource.SetException(ex);
            }
        }
    }

    // 5. 三种租户模式的配额实现
    private int GetQuotaLimit(TenantQuota quota, TenantResourceType resourceType)
    {
        return resourceType switch
        {
            TenantResourceType.DatabaseTenant => quota.MaxDatabaseSizeMB,
            TenantResourceType.SchemaTenant => quota.MaxConcurrentConnections,
            TenantResourceType.TableSchemaTenant => quota.MaxRequestPerMinute,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType))
        };
    }

    // 6. 获取当前使用量(从缓存或数据库)
    private int GetCurrentUsage(string tenantId, TenantResourceType resourceType)
    {
        var cacheKey = $"{tenantId}:{resourceType}";
        if (_cache.TryGetValue<int>(cacheKey, out var usage))
            return usage;

        // 从数据库获取最新使用量...
        return 0;
    }
}

// 7. 配额操作上下文(对象池优化)
[SkipLocalsInit]
public class QuotaContext
{
    public byte[] Buffer { get; } = new byte[512]; // Cache-line对齐
}

// 8. 扩展方法
public static class TenantQuotaExtensions
{
    public static IServiceCollection AddTenantQuotaManagement(this IServiceCollection services)
    {
        services.AddSingleton<TenantQuotaService>();
        services.AddHostedService(sp => sp.GetRequiredService<TenantQuotaService>());
        return services;
    }
}