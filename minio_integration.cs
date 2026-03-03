#:sdk Microsoft.NET.Sdk.Web
#:package Minio@7.1.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package System.IO.Pipelines@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Buffers;
using System.IO.Pipelines;
using Minio;
using Microsoft.Extensions.Caching.Memory;

// 高性能Minio文件服务
[SkipLocalsInit]
public sealed class MinioFileService : IAsyncDisposable
{
    private readonly IMinioClient _minioClient;
    private readonly Pipe _uploadPipe;
    private readonly Pipe _downloadPipe;
    private readonly IMemoryCache _cache;
    private readonly ObjectPool<Memory<byte>> _bufferPool;

    public MinioFileService(string endpoint, string accessKey, string secretKey)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
        
        _uploadPipe = new Pipe(new PipeOptions(
            pool: new PipePool(),
            readerScheduler: PipeScheduler.Inline,
            writerScheduler: PipeScheduler.Inline,
            pauseWriterThreshold: 0));
        
        _downloadPipe = new Pipe(new PipeOptions(
            pool: new PipePool(),
            readerScheduler: PipeScheduler.Inline,
            writerScheduler: PipeScheduler.Inline,
            pauseWriterThreshold: 0));
        
        _bufferPool = new DefaultObjectPool<Memory<byte>>(
            new MemoryPooledPolicy(), 
            Environment.ProcessorCount * 2);
        
        _cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 1024 * 1024 * 100 // 100MB缓存
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask UploadFileAsync(string bucket, string objectName, Stream stream)
    {
        var buffer = _bufferPool.Get();
        try
        {
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length));
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<Memory<byte>> DownloadFileAsync(string bucket, string objectName)
    {
        if (_cache.TryGetValue<Memory<byte>>($"{bucket}/{objectName}", out var cached))
            return cached;
        
        var buffer = _bufferPool.Get();
        try
        {
            var args = new GetObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithCallbackStream(stream => 
                {
                    stream.CopyTo(buffer.Span);
                    return buffer.Slice(0, (int)stream.Length);
                });
            
            var result = await _minioClient.GetObjectAsync(args);
            _cache.Set($"{bucket}/{objectName}", result, TimeSpan.FromMinutes(5));
            return result;
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _uploadPipe.Writer.CompleteAsync();
        await _downloadPipe.Writer.CompleteAsync();
        _minioClient.Dispose();
    }
}

// 服务配置
public static class MinioExtensions
{
    public static IServiceCollection AddMinioService(this IServiceCollection services, 
        string endpoint, string accessKey, string secretKey)
    {
        services.AddSingleton<IMinioClient>(_ => 
            new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build());
        
        services.AddSingleton<MinioFileService>();
        services.AddMemoryCache();

        // 添加FastEndpoints支持
        services.AddFastEndpoints();
        services.AddSingleton<AdvancedMinioService>();
        services.AddSingleton<ObjectPool<Memory<byte>>>(sp => 
            new DefaultObjectPool<Memory<byte>>(new MemoryPooledPolicy(), Environment.ProcessorCount * 4));

        // 添加Carter支持
        services.AddCarter();
        services.AddSingleton<MinioModule>();

        // 添加VSA支持
        services.AddVerticalSliceArchitecture();
        services.AddSingleton<MinioEndpoints>();

        return services;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMinioService(
    "play.min.io", 
    "Q3AM3UQ867SPQQA43P2F", 
    "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");

var app = builder.Build();
app.MapGet("/", () => "MinIO File Service Running");
app.Run();