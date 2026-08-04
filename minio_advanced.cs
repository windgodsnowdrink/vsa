#:sdk Microsoft.NET.Sdk.Web
#:package Minio@7.1.0
#:package System.IO.Pipelines@7.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Buffers;
using System.IO.Pipelines;
using Minio;
using Minio.DataModel.Args;

// 增强版Minio服务
[SkipLocalsInit]
public sealed class AdvancedMinioService : IAsyncDisposable
{
    private readonly IMinioClient _minioClient;
    private readonly ObjectPool<Memory<byte>> _bufferPool;
    private readonly ThreadLocal<Span<byte>> _threadBuffer;

    // 在AdvancedMinioService类中添加以下成员
    private readonly Channel<UploadPart> _uploadChannel;
    private readonly ObjectPool<Memory<byte>> _uploadBufferPool;
    private readonly IEventStoreService _eventStore;

    // 修改构造函数
    public AdvancedMinioService(string endpoint, string accessKey, string secretKey, IEventStoreService eventStore = null)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL()
            .Build();

        _bufferPool = new DefaultObjectPool<Memory<byte>>(
            new MemoryPooledPolicy(),
            Environment.ProcessorCount * 4);

        _threadBuffer = new(() => stackalloc byte[8192]);
    }

    // 桶管理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<bool> BucketExistsAsync(string bucketName)
    {
        var args = new BucketExistsArgs().WithBucket(bucketName);
        return await _minioClient.BucketExistsAsync(args);
    }

    // 流式上传
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task StreamUploadAsync(string bucket, string objectName, PipeReader reader)
    {
        var buffer = _bufferPool.Get();
        try
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var bufferSpan = buffer.Span;

                if (result.Buffer.Length > 0)
                {
                    result.Buffer.CopyTo(bufferSpan);
                    await _minioClient.PutObjectAsync(new PutObjectArgs()
                        .WithBucket(bucket)
                        .WithObject(objectName)
                        .WithStreamData(new MemoryStream(buffer[..(int)result.Buffer.Length]))
                        .WithObjectSize(result.Buffer.Length));
                }

                reader.AdvanceTo(result.Buffer.End);
                if (result.IsCompleted) break;
            }
        }
        finally
        {
            _bufferPool.Return(buffer);
        }
    }

    // 分片上传
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task MultipartUploadAsync(string bucket, string objectName, Stream stream,
        int partSize = 5 * 1024 * 1024)
    {
        var uploadId = await _minioClient.StartMultipartUploadAsync(
            new StartMultipartUploadArgs(bucket, objectName));

        var partNumber = 1;
        var tasks = new List<Task>();
        var buffer = _threadBuffer.Value;

        while (stream.Position < stream.Length)
        {
            var bytesRead = await stream.ReadAsync(buffer);
            var partStream = new MemoryStream(buffer[..bytesRead]);

            tasks.Add(_minioClient.PutObjectPartAsync(new PutObjectPartArgs()
                .WithBucket(bucket)
                .WithObject(objectName)
                .WithUploadId(uploadId)
                .WithPartNumber(partNumber++)
                .WithStreamData(partStream)
                .WithPartSize(bytesRead)));
        }

        await Task.WhenAll(tasks);
        await _minioClient.CompleteMultipartUploadAsync(
            new CompleteMultipartUploadArgs(bucket, objectName, uploadId));
    }

    // 文件列表管理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async IAsyncEnumerable<MinioObject> ListObjectsAsync(string bucketName, string prefix = "", bool recursive = true)
    {
        var args = new ListObjectsArgs()
            .WithBucket(bucketName)
            .WithPrefix(prefix)
            .WithRecursive(recursive);

        var observable = _minioClient.ListObjectsAsync(args);
        await foreach (var item in observable.ToAsyncEnumerable())
        {
            yield return new MinioObject(
                item.Key,
                item.Size,
                item.LastModifiedDateTime,
                item.ETag);
        }
    }

    public record MinioObject(
        string Key,
        ulong Size,
        DateTimeOffset LastModified,
        string ETag);

    public async ValueTask DisposeAsync()
    {
        _minioClient.Dispose();
    }
}

// 服务配置扩展
public static class MinioServiceExtensions
{
    public static IServiceCollection AddAdvancedMinioService(this IServiceCollection services,
        string endpoint, string accessKey, string secretKey)
    {
        services.AddSingleton<AdvancedMinioService>(_ =>
            new AdvancedMinioService(endpoint, accessKey, secretKey));

        // 添加HTTP客户端用于预签名URL验证
        services.AddHttpClient("MinioPresigned");

        return services;
    }
}

// 启动配置
/*
// 列出所有文件
await foreach (var obj in minioService.ListObjectsAsync("my-bucket"))
{
    Console.WriteLine($"{obj.Key} - {obj.Size} bytes");
}

// 生成下载链接
var url = await minioService.GeneratePresignedUrlAsync(
    "my-bucket", 
    "file.txt", 
    3600);
*/
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAdvancedMinioService(
    "play.min.io",
    "Q3AM3UQ867SPQQA43P2F",
    "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");

var app = builder.Build();
app.MapGet("/", () => "Advanced MinIO Service Running");
app.Run();

// 预签名URL生成
[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task<string> GeneratePresignedUrlAsync(
    string bucketName,
    string objectName,
    int expiresInSeconds = 3600,
    Dictionary<string, string> reqParams = null)
{
    var args = new PresignedGetObjectArgs()
        .WithBucket(bucketName)
        .WithObject(objectName)
        .WithExpiry(expiresInSeconds);

    if (reqParams != null)
    {
        args.WithHeaders(reqParams);
    }

    return await _minioClient.PresignedGetObjectAsync(args);
}

// 带策略的预签名URL
[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task<string> GeneratePresignedPostPolicyAsync(
    string bucketName,
    string objectName,
    DateTime expiration,
    long contentLengthRangeMin = 0,
    long contentLengthRangeMax = 1073741824) // 默认1GB
{
    var policy = new PostPolicy()
        .SetBucket(bucketName)
        .SetKey(objectName)
        .SetExpires(expiration);

    policy.SetContentLengthRange(contentLengthRangeMin, contentLengthRangeMax);

    return await _minioClient.PresignedPostPolicyAsync(policy);
}

// 添加多线程分块上传方法
[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task MultiThreadUploadAsync(string bucket, string objectName, Stream stream)
{
    var uploadId = await _minioClient.InitiateMultipartUploadAsync(bucket, objectName);
    var partNumber = 1;
    var tasks = new List<Task>();
    
    // 使用Span零拷贝读取
    var buffer = _uploadBufferPool.Get();
    try
    {
        while (stream.Position < stream.Length)
        {
            var bytesRead = await stream.ReadAsync(buffer);
            var partData = buffer.Slice(0, bytesRead);
            
            await _uploadChannel.Writer.WriteAsync(new UploadPart(
                bucket, objectName, uploadId, partNumber++, partData));
        }

        // 启动后台处理任务
        var processors = Enumerable.Range(0, Environment.ProcessorCount)
            .Select(_ => Task.Run(ProcessUploadsAsync))
            .ToArray();

        await Task.WhenAll(processors);
        await _minioClient.CompleteMultipartUploadAsync(bucket, objectName, uploadId);

        // 记录事件
        if (_eventStore != null)
        {
            await _eventStore.AppendEventAsync(new MinioUploadEvent(
                bucket, objectName, stream.Length));
        }
    }
    finally
    {
        _uploadBufferPool.Return(buffer);
    }
}

private async Task ProcessUploadsAsync()
{
    await foreach (var part in _uploadChannel.Reader.ReadAllAsync())
    {
        using var memoryStream = new MemoryStream(part.Data.ToArray());
        await _minioClient.PutObjectPartAsync(new PutObjectPartArgs()
            .WithBucket(part.Bucket)
            .WithObject(part.ObjectName)
            .WithUploadId(part.UploadId)
            .WithPartNumber(part.PartNumber)
            .WithStreamData(memoryStream));
    }
}

// 添加记录类
public record UploadPart(
    string Bucket,
    string ObjectName,
    string UploadId,
    int PartNumber,
    Memory<byte> Data);

public record MinioUploadEvent(
    string Bucket,
    string ObjectName,
    long FileSize) : IEvent;


// 修改构造函数
public AdvancedMinioService(string endpoint, string accessKey, string secretKey, IEventStoreService eventStore = null)
{
    _minioClient = new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL()
        .Build();

    _bufferPool = new DefaultObjectPool<Memory<byte>>(
        new MemoryPooledPolicy(),
        Environment.ProcessorCount * 4);

    _threadBuffer = new(() => stackalloc byte[8192]);

    // 添加智能缓存组件
    _lruCache = new LruKCache<string, Memory<byte>>(capacity: 1000, k: 2);
    _tieredMemory = new TieredMemoryManager();
    _alignedPool = new AlignedMemoryPool(64); // 64字节对齐
}

// 在AdvancedMinioService类中添加以下成员
private readonly AlignedMemoryPool _alignedPool;

// 修改构造函数
public AdvancedMinioService(string endpoint, string accessKey, string secretKey, IEventStoreService eventStore = null)
{
    _minioClient = new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL()
        .Build();

    _bufferPool = new DefaultObjectPool<Memory<byte>>(
        new MemoryPooledPolicy(),
        Environment.ProcessorCount * 4);

    _threadBuffer = new(() => stackalloc byte[8192]);

    // 添加智能缓存组件
    _lruCache = new LruKCache<string, Memory<byte>>(capacity: 1000, k: 2);
    _tieredMemory = new TieredMemoryManager();
    _alignedPool = new AlignedMemoryPool(64); // 64字节对齐
}

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public unsafe Memory<byte> GetAlignedBuffer(int size)
{
    var memory = _alignedPool.Rent(size);
    if (((nint)memory.Pin().Pointer % 64) != 0)
        throw new InvalidOperationException("Memory not aligned");
    return memory;
}

// 添加辅助类
public sealed class AlignedMemoryPool : MemoryPool<byte>
{
    private readonly int _alignment;
    
    public AlignedMemoryPool(int alignment) => _alignment = alignment;
    
    protected override IMemoryOwner<byte> Allocate(int size)
    {
        var ptr = NativeMemory.AlignedAlloc((nuint)size, (nuint)_alignment);
        return new AlignedMemoryOwner(ptr, size);
    }
    
    private sealed class AlignedMemoryOwner : IMemoryOwner<byte>
    {
        private readonly void* _ptr;
        private readonly int _length;
        
        public AlignedMemoryOwner(void* ptr, int length) => (_ptr, _length) = (ptr, length);
        
        public Memory<byte> Memory => new UnsafeMemoryManager<byte>(_ptr, _length).Memory;
        
        public void Dispose() => NativeMemory.AlignedFree(_ptr);
    }
}

// 分层存储自动迁移
[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task AutoTierMigrationAsync(string sourceBucket, string destBucket, TimeSpan threshold)
{
    var cutoff = DateTime.UtcNow - threshold;
    var objects = _minioClient.ListObjectsAsync(sourceBucket)
        .Where(o => o.LastModifiedDateTime < cutoff)
        .ToAsyncEnumerable();
    
    await foreach (var obj in objects)
    {
        using var stream = new MemoryStream();
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(sourceBucket)
            .WithObject(obj.Key)
            .WithCallbackStream(s => s.CopyTo(stream)));
        
        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(destBucket)
            .WithObject(obj.Key)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length));
        
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(sourceBucket)
            .WithObject(obj.Key));
    }
}

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task<Memory<byte>> DownloadFileAsync(string bucket, string objectName, bool useCache = true)
{
    // 检查缓存
    if (useCache && _cache.TryGetValue<Memory<byte>>($"{bucket}/{objectName}", out var cached))
        return cached;

    // 使用对象池获取缓冲区
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
        
        // 缓存结果
        if (useCache)
            _cache.Set($"{bucket}/{objectName}", result, TimeSpan.FromMinutes(5));
            
        return result;
    }
    finally
    {
        _bufferPool.Return(buffer);
    }
}

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task ClearCacheAsync(string bucket, IEnumerable<string> objectNames)
{
    foreach (var name in objectNames)
        _cache.Remove($"{bucket}/{name}");
}

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
public async Task PreloadCacheAsync(string bucket, string objectName)
{
    await DownloadFileAsync(bucket, objectName);
}
