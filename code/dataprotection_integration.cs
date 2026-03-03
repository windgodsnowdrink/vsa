#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.DataProtection@8.0.0
#:package Microsoft.AspNetCore.DataProtection.Extensions@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Caching.Distributed;

[SkipLocalsInit]
public sealed class DataProtectionService : IAsyncDisposable
{
    private readonly Channel<ProtectRequest> _requestChannel;
    private readonly ObjectPool<IDataProtector> _protectorPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;
    private readonly IDistributedCache _cache;

    public DataProtectionService(IDataProtectionProvider provider, IDistributedCache cache)
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        _cache = cache;
        
        _requestChannel = Channel.CreateBounded<ProtectRequest>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _protectorPool = new DefaultObjectPool<IDataProtector>(
            new ProtectorPooledPolicy(provider), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessRequestsAsync);
    }

    // 数据保护方法
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<string> ProtectAsync(string plaintext, TimeSpan? lifetime = null)
    {
        var request = new ProtectRequest(plaintext, ProtectOperation.Protect, lifetime);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    // 数据解保护方法
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<string> UnprotectAsync(string protectedText)
    {
        var request = new ProtectRequest(protectedText, ProtectOperation.Unprotect);
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var protector = _protectorPool.Get();
            try
            {
                var result = ProcessProtection(protector, request);
                request.Completion.SetResult(result);
            }
            finally
            {
                _protectorPool.Return(protector);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private string ProcessProtection(IDataProtector protector, ProtectRequest request)
    {
        return request.Operation switch
        {
            ProtectOperation.Protect => protector.Protect(request.Data),
            ProtectOperation.Unprotect => protector.Unprotect(request.Data),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _requestChannel.Writer.Complete();
        await _requestChannel.Reader.Completion;
    }
}

internal record ProtectRequest(string Data, ProtectOperation Operation, TimeSpan? Lifetime = null)
{
    public TaskCompletionSource<string> Completion { get; } = new();
}

internal enum ProtectOperation { Protect, Unprotect }

[SkipLocalsInit]
internal sealed class ProtectorPooledPolicy : PooledObjectPolicy<IDataProtector>
{
    private readonly IDataProtectionProvider _provider;

    public ProtectorPooledPolicy(IDataProtectionProvider provider)
    {
        _provider = provider;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override IDataProtector Create() => _provider.CreateProtector("DataProtectionService");

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(IDataProtector obj) => true;
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);

// 配置数据保护
builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("localhost"))
    .SetApplicationName("MyApp")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// 配置分布式缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "DataProtection_";
});

builder.Services.AddSingleton<DataProtectionService>();

var app = builder.Build();
app.MapGet("/", () => "Data Protection Service");
app.Run();