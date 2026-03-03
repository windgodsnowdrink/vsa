# MinIO - 参考文档

## 概述

MinIO 是基于 .NET 10 的高性能对象存储系统，专为 .NET 开发者设计。它提供了一系列对象存储功能，包括文件上传下载、批量操作、预签名 URL、生命周期管理等，帮助开发者快速构建高性能的对象存储应用。

## 核心组件

### 1. MinIO 客户端
- **位置**: scripts/minio_integration.cs
- **功能**: 核心 MinIO 客户端实现
- **特性**: 
  - 基于 Minio 客户端库
  - 支持所有 MinIO API
  - 高性能实现
  - 异步编程模型

### 2. 依赖注入服务
- **位置**: scripts/minio_integration.cs
- **功能**: 提供依赖注入集成
- **特性**: 
  - 集成 Microsoft.Extensions.DependencyInjection
  - 支持配置绑定
  - 支持服务生命周期管理

### 3. 存储桶管理
- **位置**: scripts/minio_integration.cs
- **功能**: 管理 MinIO 存储桶
- **特性**: 
  - 创建存储桶
  - 检查存储桶是否存在
  - 列出存储桶
  - 删除存储桶

### 4. 对象操作
- **位置**: scripts/minio_integration.cs
- **功能**: 管理 MinIO 对象
- **特性**: 
  - 上传对象
  - 下载对象
  - 删除对象
  - 列出对象
  - 复制对象

### 5. 预签名 URL
- **位置**: scripts/minio_integration.cs
- **功能**: 生成预签名 URL
- **特性**: 
  - 生成上传 URL
  - 生成下载 URL
  - 生成删除 URL
  - 设置过期时间

## 使用示例

### 基本用法

```csharp
// 创建 MinIO 客户端
var minioClient = new MinioClient()
    .WithEndpoint("localhost:9000")
    .WithCredentials("minioadmin", "minioadmin")
    .WithSSL(false)
    .Build();

// 创建存储桶
var bucketName = "mybucket";
var found = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
if (!found)
{
    await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    Console.WriteLine($"存储桶 {bucketName} 创建成功");
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

// 删除对象
await minioClient.RemoveObjectAsync(
    new RemoveObjectArgs()
        .WithBucket(bucketName)
        .WithObject(objectName)
);
Console.WriteLine($"对象 {objectName} 删除成功");
```

### 高级配置

```csharp
// 配置 MinIO 服务
builder.Services.AddMinioServices(options => {
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

// 获取配置的 MinIO 客户端
var minioClient = serviceProvider.GetRequiredService<MinioClient>();

// 使用 MinIO 客户端
var bucketName = "mybucket";

// 列出存储桶中的对象
var results = await minioClient.ListObjectsAsync(
    new ListObjectsArgs()
        .WithBucket(bucketName)
        .WithPrefix("documents/")
        .WithRecursive(true)
);

await foreach (var item in results)
{
    Console.WriteLine($"对象: {item.Key}, 大小: {item.Size}, 最后修改: {item.LastModified}");
}
```

### 预签名 URL 示例

```csharp
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
```

## 配置选项

### MinIO 客户端配置

```json
{
  "MinioOptions": {
    "Endpoint": "localhost:9000",           // MinIO 服务端点
    "AccessKey": "minioadmin",           // 访问密钥
    "SecretKey": "minioadmin",           // 秘密密钥
    "WithSSL": false,                    // 是否启用 SSL
    "ConnectTimeout": "00:00:30",        // 连接超时时间
    "WriteTimeout": "00:05:00",          // 写入超时时间
    "ReadTimeout": "00:05:00",           // 读取超时时间
    "TCPKeepAlive": true,                // 是否启用 TCP 保活
    "RetryCount": 3,                     // 重试次数
    "RetryWaitTime": "00:00:02"         // 重试等待时间
  }
}
```

### 依赖注入配置

```csharp
// 基本配置
builder.Services.AddMinioClient(options => {
    options.Endpoint = Configuration["MinioOptions:Endpoint"];
    options.AccessKey = Configuration["MinioOptions:AccessKey"];
    options.SecretKey = Configuration["MinioOptions:SecretKey"];
    options.WithSSL = bool.Parse(Configuration["MinioOptions:WithSSL"]);
});

// 高级配置
builder.Services.AddMinioClient((sp, options) => {
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.Endpoint = configuration["MinioOptions:Endpoint"];
    options.AccessKey = configuration["MinioOptions:AccessKey"];
    options.SecretKey = configuration["MinioOptions:SecretKey"];
    options.WithSSL = bool.Parse(configuration["MinioOptions:WithSSL"]);
    options.ConnectTimeout = TimeSpan.Parse(configuration["MinioOptions:ConnectTimeout"]);
    options.WriteTimeout = TimeSpan.Parse(configuration["MinioOptions:WriteTimeout"]);
    options.ReadTimeout = TimeSpan.Parse(configuration["MinioOptions:ReadTimeout"]);
    options.TCPKeepAlive = bool.Parse(configuration["MinioOptions:TCPKeepAlive"]);
    options.RetryCount = int.Parse(configuration["MinioOptions:RetryCount"]);
    options.RetryWaitTime = TimeSpan.Parse(configuration["MinioOptions:RetryWaitTime"]);
});
```

## 性能优化

1. **内存分配优化**：减少不必要的内存分配
2. **GC 压力优化**：减少 GC 触发次数
3. **并发优化**：使用线程安全的代码
4. **批处理优化**：批量处理请求提高效率
5. **缓存使用**：合理使用缓存提高性能
6. **网络传输优化**：优化网络传输中的数据处理
7. **分块上传**：对于大文件使用分块上传
8. **并行上传**：使用并行上传提高速度
9. **连接池**：使用连接池管理 MinIO 连接
10. **超时设置**：设置适当的超时时间

## 故障排除

### 常见问题

1. **连接失败**
   - 检查配置文件中的 Endpoint、AccessKey 和 SecretKey
   - 验证网络连接是否正常
   - 检查 MinIO 服务是否运行
   - 查看日志信息

2. **上传失败**
   - 检查文件路径是否正确
   - 验证存储桶是否存在
   - 检查权限设置
   - 查看网络连接

3. **下载失败**
   - 检查对象名称是否正确
   - 验证对象是否存在
   - 检查权限设置
   - 查看网络连接

4. **性能问题**
   - 启用连接池
   - 使用分块上传
   - 优化并发设置
   - 增加服务器资源

5. **预签名 URL 问题**
   - 检查过期时间设置
   - 验证对象是否存在
   - 检查权限设置
   - 查看网络连接

## 扩展开发

### 自定义 MinIO 客户端

```csharp
public class CustomMinioClient
{
    private readonly MinioClient _minioClient;
    private readonly ILogger<CustomMinioClient> _logger;

    public CustomMinioClient(IOptions<MinioOptions> options, ILogger<CustomMinioClient> logger)
    {
        var opt = options.Value;
        _minioClient = new MinioClient()
            .WithEndpoint(opt.Endpoint)
            .WithCredentials(opt.AccessKey, opt.SecretKey)
            .WithSSL(opt.WithSSL)
            .Build();
        _logger = logger;
    }

    public async Task UploadFileAsync(string bucketName, string objectName, string filePath)
    {
        try
        {
            _logger.LogInformation("开始上传文件: {ObjectName}", objectName);
            
            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(GetContentType(filePath))
            );
            
            _logger.LogInformation("文件上传成功: {ObjectName}", objectName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文件上传失败: {ObjectName}", objectName);
            throw;
        }
    }

    private string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        return extension switch
        {
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".jpg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}

// 注册自定义客户端
builder.Services.AddOptions<MinioOptions>()
    .Bind(builder.Configuration.GetSection("MinioOptions"))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<CustomMinioClient>();
```

### 批量操作扩展

```csharp
public class MinioBatchOperations
{
    private readonly MinioClient _minioClient;

    public MinioBatchOperations(MinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task BatchUploadAsync(string bucketName, IEnumerable<(string ObjectName, string FilePath)> files)
    {
        var tasks = files.Select(async file =>
        {
            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(file.ObjectName)
                    .WithFileName(file.FilePath)
                    .WithContentType(GetContentType(file.FilePath))
            );
        });

        await Task.WhenAll(tasks);
    }

    public async Task BatchDeleteAsync(string bucketName, IEnumerable<string> objectNames)
    {
        var tasks = objectNames.Select(async objectName =>
        {
            await _minioClient.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
            );
        });

        await Task.WhenAll(tasks);
    }

    private string GetContentType(string filePath)
    {
        // 实现内容类型检测
        return "application/octet-stream";
    }
}
```

## AOT 编译优化

### AOT 编译配置

```yaml
#:property PublishAot=true
#:property IncludeNativeLibrariesForSelfExtract=true
#:property EnableCppCodeGen=true
#:property PublishSingleFile=true
#:property SelfContained=true
#:property RuntimeIdentifier=win-x64
#:property RuntimeIdentifier=linux-x64
#:property RuntimeIdentifier=osx-x64
```

### AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码
2. **避免动态类型**：使用强类型
3. **避免运行时代码生成**：使用预编译代码
4. **优化内存使用**：使用 Span<T> 和 Memory<T>
5. **减少依赖**：最小化依赖项
6. **使用值类型**：减少 GC 压力
7. **避免大对象分配**：避免分配大于 85KB 的对象
8. **使用对象池**：对于频繁创建和销毁的对象，使用对象池

## 部署说明

### 部署步骤

1. **编译**：使用 .NET 10 SDK 编译代码
2. **打包**：打包为单文件可执行文件
3. **部署**：部署到目标环境
4. **配置**：配置环境变量和配置文件
5. **启动**：启动服务

### 环境要求

- .NET 10 运行时或更高版本
- 足够的内存和磁盘空间
- 支持 AOT 编译的操作系统
- 网络连接（用于访问 MinIO 服务）

### 配置文件

```json
{
  "MinioOptions": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "WithSSL": false,
    "ConnectTimeout": "00:00:30",
    "WriteTimeout": "00:05:00",
    "ReadTimeout": "00:05:00",
    "TCPKeepAlive": true,
    "RetryCount": 3,
    "RetryWaitTime": "00:00:02"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 监控和维护

### 监控指标

1. **上传成功率**：上传成功的文件比例
2. **下载成功率**：下载成功的文件比例
3. **平均上传时间**：文件上传的平均时间
4. **平均下载时间**：文件下载的平均时间
5. **错误率**：操作错误率
6. **内存使用**：内存使用情况
7. **CPU 使用**：CPU 使用情况
8. **网络流量**：网络流量情况

### 维护建议

1. **定期检查**：定期检查 MinIO 服务状态
2. **优化配置**：根据实际使用情况优化配置
3. **更新依赖**：定期更新依赖项
4. **性能测试**：定期进行性能测试
5. **安全审计**：定期进行安全审计
6. **备份**：定期备份重要数据
7. **监控**：建立完善的监控系统
8. **告警**：设置合理的告警阈值

## 总结

MinIO 智能体技能提供了一套完整的对象存储解决方案，包括文件上传下载、批量操作、预签名 URL、生命周期管理等功能。它基于 .NET 10 构建，支持 AOT 编译，可以帮助 .NET 开发者更高效地构建高性能的对象存储应用，提高应用程序性能，确保数据的安全性和可靠性，实现更好的用户体验。
