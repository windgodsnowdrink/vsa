#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.0.0
#:package System.IO.Compression@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.IO.Compression;
using Microsoft.Extensions.ObjectPool;

[SkipLocalsInit]
public sealed class CompressionMiddleware
{
    private readonly Channel<ProxyRequest> _requestChannel;
    private readonly ObjectPool<MemoryStream> _streamPool;
    private readonly CancellationTokenSource _cts;

    public CompressionMiddleware()
    {
        _cts = new CancellationTokenSource();
        
        _requestChannel = Channel.CreateBounded<ProxyRequest>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
            
        _streamPool = new DefaultObjectPool<MemoryStream>(
            new MemoryStreamPooledPolicy(), 
            Environment.ProcessorCount * 2);
            
        _ = Task.Run(ProcessRequestsAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<ProxyResponse> ExecuteAsync(ProxyRequest request)
    {
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(_cts.Token))
        {
            var response = await ProcessRequestWithCompressionAsync(request);
            request.Completion.SetResult(response);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task<ProxyResponse> ProcessRequestWithCompressionAsync(ProxyRequest request)
    {
        using var buffer = MemoryPool<byte>.Shared.Rent(4096);
        var memory = buffer.Memory;
        
        // 实现基于Brotli/Gzip的压缩解压逻辑
        var response = new ProxyResponse();
        // ... 实际压缩解压逻辑实现
        
        return response;
    }
}

// 添加到现有YARP配置中
builder.Services.AddSingleton<CompressionMiddleware>();