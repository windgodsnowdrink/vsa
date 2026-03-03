# MinIO - 使用示例

## 快速开始

### 1. 基本使用示例

```csharp
using System;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;
using Minio.Requests;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MinIO 基本使用示例");
        Console.WriteLine("=" * 50);
        
        // 创建 MinIO 客户端
        var minioClient = new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("minioadmin", "minioadmin")
            .WithSSL(false)
            .Build();
        
        try
        {
            // 创建存储桶
            var bucketName = "mybucket";
            var found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                Console.WriteLine($"存储桶 {bucketName} 创建成功");
            }
            else
            {
                Console.WriteLine($"存储桶 {bucketName} 已存在");
            }
            
            // 上传文件
            var objectName = "myobject.txt";
            var filePath = "path/to/file.txt";
            var contentType = "text/plain";
            
            await minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(contentType)
            );
            Console.WriteLine($"文件 {objectName} 上传成功");
            
            // 下载文件
            var downloadPath = "path/to/download.txt";
            await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFile(downloadPath)
            );
            Console.WriteLine($"文件 {objectName} 下载成功");
            
            // 列出存储桶中的对象
            var results = await minioClient.ListObjectsAsync(
                new ListObjectsArgs()
                    .WithBucket(bucketName)
                    .WithRecursive(true)
            );
            
            Console.WriteLine("存储桶中的对象:");
            await foreach (var item in results)
            {
                Console.WriteLine($"- {item.Key}, 大小: {item.Size}, 最后修改: {item.LastModified}");
            }
            
            // 删除对象
            await minioClient.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
            );
            Console.WriteLine($"对象 {objectName} 删除成功");
            
        }
        catch (MinioException ex)
        {
            Console.WriteLine($"MinIO 错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 2. 高级配置示例

```csharp
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Minio;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MinIO 高级配置示例");
        Console.WriteLine("=" * 50);
        
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置 MinIO 服务
        services.AddMinioClient(options => {
            options.Endpoint = "localhost:9000";
            options.AccessKey = "minioadmin";
            options.SecretKey = "minioadmin";
            options.WithSSL = false;
            options.ConnectTimeout = TimeSpan.FromSeconds(30);
            options.WriteTimeout = TimeSpan.FromMinutes(5);
            options.ReadTimeout = TimeSpan.FromMinutes(5);
            options.TCPKeepAlive = true;
            options.RetryCount = 3;
            options.RetryWaitTime = TimeSpan.FromSeconds(2);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取 MinIO 客户端
        var minioClient = serviceProvider.GetRequiredService<MinioClient>();
        
        try
        {
            // 创建存储桶
            var bucketName = "advanced-bucket";
            var found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                Console.WriteLine($"存储桶 {bucketName} 创建成功");
            }
            
            // 设置存储桶策略
            var policyJson = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Effect"": ""Allow"",
                        ""Principal"": {""AWS"": [""*""]},
                        ""Action"": [""s3:GetObject""],
                        ""Resource"": [""arn:aws:s3:::advanced-bucket/*""]
                    }
                ]
            }";
            
            await minioClient.SetBucketPolicyAsync(
                new SetBucketPolicyArgs()
                    .WithBucket(bucketName)
                    .WithPolicy(policyJson)
            );
            Console.WriteLine("存储桶策略设置成功");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}

// 扩展方法: 添加 MinIO 客户端到依赖注入
public static class MinioExtensions
{
    public static IServiceCollection AddMinioClient(this IServiceCollection services, Action<MinioOptions> configureOptions)
    {
        services.AddOptions<MinioOptions>()
            .Configure(configureOptions)
            .ValidateDataAnnotations();
        
        services.AddSingleton<MinioClient>(sp => {
            var options = sp.GetRequiredService<IOptions<MinioOptions>>().Value;
            return new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.WithSSL)
                .Build();
        });
        
        return services;
    }
}

// MinIO 选项类
public class MinioOptions
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public bool WithSSL { get; set; }
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan WriteTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan ReadTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public bool TCPKeepAlive { get; set; } = true;
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryWaitTime { get; set; } = TimeSpan.FromSeconds(2);
}
```

### 3. 批量操作示例

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minio;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MinIO 批量操作示例");
        Console.WriteLine("=" * 50);
        
        // 创建 MinIO 客户端
        var minioClient = new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("minioadmin", "minioadmin")
            .WithSSL(false)
            .Build();
        
        var bucketName = "batch-bucket";
        
        try
        {
            // 确保存储桶存在
            var found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
            
            // 批量上传文件
            var filesToUpload = new List<(string ObjectName, string FilePath)>
            {
                ("file1.txt", "path/to/file1.txt"),
                ("file2.txt", "path/to/file2.txt"),
                ("file3.txt", "path/to/file3.txt")
            };
            
            await BatchUploadAsync(minioClient, bucketName, filesToUpload);
            
            // 批量删除文件
            var filesToDelete = new List<string>
            {
                "file1.txt",
                "file2.txt",
                "file3.txt"
            };
            
            await BatchDeleteAsync(minioClient, bucketName, filesToDelete);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static async Task BatchUploadAsync(MinioClient client, string bucketName, IEnumerable<(string ObjectName, string FilePath)> files)
    {
        var tasks = new List<Task>();
        
        foreach (var (objectName, filePath) in files)
        {
            tasks.Add(client.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType("text/plain")
            ));
        }
        
        await Task.WhenAll(tasks);
        Console.WriteLine($"批量上传成功，共上传 {tasks.Count} 个文件");
    }
    
    private static async Task BatchDeleteAsync(MinioClient client, string bucketName, IEnumerable<string> objectNames)
    {
        var tasks = new List<Task>();
        
        foreach (var objectName in objectNames)
        {
            tasks.Add(client.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
            ));
        }
        
        await Task.WhenAll(tasks);
        Console.WriteLine($"批量删除成功，共删除 {tasks.Count} 个文件");
    }
}
```

### 4. 预签名 URL 示例

```csharp
using System;
using System.Threading.Tasks;
using Minio;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MinIO 预签名 URL 示例");
        Console.WriteLine("=" * 50);
        
        // 创建 MinIO 客户端
        var minioClient = new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("minioadmin", "minioadmin")
            .WithSSL(false)
            .Build();
        
        var bucketName = "presigned-bucket";
        var objectName = "myobject.txt";
        
        try
        {
            // 确保存储桶存在
            var found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
            
            // 上传一个文件用于测试
            await minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName("path/to/file.txt")
                    .WithContentType("text/plain")
            );
            
            // 生成预签名下载 URL
            var presignedUrl = await minioClient.PresignedGetObjectAsync(
                new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(60 * 60) // 1小时过期
            );
            Console.WriteLine($"预签名下载 URL: {presignedUrl}");
            
            // 生成预签名上传 URL
            var presignedPutUrl = await minioClient.PresignedPutObjectAsync(
                new PresignedPutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject("newobject.txt")
                    .WithExpiry(60 * 30) // 30分钟过期
                    .WithContentType("text/plain")
            );
            Console.WriteLine($"预签名上传 URL: {presignedPutUrl}");
            
            // 生成预签名删除 URL
            var presignedDeleteUrl = await minioClient.PresignedRemoveObjectAsync(
                new PresignedRemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithExpiry(60 * 15) // 15分钟过期
            );
            Console.WriteLine($"预签名删除 URL: {presignedDeleteUrl}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
}
```

### 5. 性能优化示例

```csharp
using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Minio;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("MinIO 性能优化示例");
        Console.WriteLine("=" * 50);
        
        // 创建 MinIO 客户端
        var minioClient = new MinioClient()
            .WithEndpoint("localhost:9000")
            .WithCredentials("minioadmin", "minioadmin")
            .WithSSL(false)
            .Build();
        
        var bucketName = "performance-bucket";
        
        try
        {
            // 确保存储桶存在
            var found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            if (!found)
            {
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
            }
            
            // 使用内存池进行大文件上传
            await UploadWithMemoryPoolAsync(minioClient, bucketName, "largefile.bin", 1024 * 1024 * 10); // 10MB
            
            // 使用并行上传多个小文件
            await ParallelUploadAsync(minioClient, bucketName);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"错误: {ex.Message}");
        }
    }
    
    private static async Task UploadWithMemoryPoolAsync(MinioClient client, string bucketName, string objectName, long sizeInBytes)
    {
        Console.WriteLine($"开始上传大文件: {objectName} ({sizeInBytes / (1024 * 1024)}MB)");
        
        // 使用内存池分配缓冲区
        using var memory = MemoryPool<byte>.Shared.Rent(1024 * 1024); // 1MB 缓冲区
        var buffer = memory.Memory;
        
        // 创建临时文件
        var tempFile = Path.GetTempFileName();
        
        try
        {
            // 生成测试数据
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                long written = 0;
                while (written < sizeInBytes)
                {
                    int writeSize = (int)Math.Min(buffer.Length, sizeInBytes - written);
                    // 填充缓冲区
                    for (int i = 0; i < writeSize; i++)
                    {
                        buffer.Span[i] = (byte)(i % 255);
                    }
                    await fs.WriteAsync(buffer[..writeSize]);
                    written += writeSize;
                }
            }
            
            // 上传文件
            await client.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(tempFile)
                    .WithContentType("application/octet-stream")
            );
            
            Console.WriteLine($"大文件上传成功: {objectName}");
        }
        finally
        {
            // 清理临时文件
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
    
    private static async Task ParallelUploadAsync(MinioClient client, string bucketName)
    {
        Console.WriteLine("开始并行上传多个小文件");
        
        var tasks = new List<Task>();
        
        for (int i = 0; i < 10; i++)
        {
            var objectName = $"smallfile{i}.txt";
            var tempFile = Path.GetTempFileName();
            
            // 创建临时文件
            File.WriteAllText(tempFile, $"This is small file {i}");
            
            // 并行上传
            tasks.Add(Task.Run(async () => {
                try
                {
                    await client.PutObjectAsync(
                        new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(objectName)
                            .WithFileName(tempFile)
                            .WithContentType("text/plain")
                    );
                    Console.WriteLine($"文件 {objectName} 上传成功");
                }
                finally
                {
                    // 清理临时文件
                    if (File.Exists(tempFile))
                    {
                        File.Delete(tempFile);
                    }
                }
            }));
        }
        
        // 等待所有上传完成
        await Task.WhenAll(tasks);
        Console.WriteLine($"并行上传完成，共上传 {tasks.Count} 个文件");
    }
}
