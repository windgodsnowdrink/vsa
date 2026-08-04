# MinIO 智能体技能

## 技能概述

基于 .NET 10 的高性能 MinIO 技能，采用 AOT 编译优化，为 .NET 开发者提供强大的对象存储功能，支持高并发、低延迟的对象存储操作。

## 快速开始指南

### 安装依赖

在主应用程序的 runfile 中添加以下依赖：

```yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Minio@6.0.0
#:package AWSSDK.S3@3.7.0
```

### 注册服务

在主应用程序中注册 MinIO 服务：

```csharp
// 注册 MinIO 服务
builder.Services.AddMinioServices(options => {
    options.Endpoint = "localhost:9000";
    options.AccessKey = "minioadmin";
    options.SecretKey = "minioadmin";
    options.WithSSL = false;
});
```

### 使用示例

```csharp
// 获取 MinIO 服务
var minioClient = serviceProvider.GetRequiredService<MinioClient>();

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
```

## AOT 架构执行

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

### 执行流程

1. **编译阶段**：使用 .NET 10 的 AOT 编译功能将代码编译为本地机器码，提高运行性能
2. **打包阶段**：将编译后的代码打包为单文件可执行文件，包含所有依赖项
3. **部署阶段**：将打包后的可执行文件部署到目标环境，无需安装 .NET 运行时
4. **运行阶段**：执行单文件可执行文件，处理对象存储操作，享受 AOT 编译带来的性能优势

### AOT 架构优势

1. **启动速度快**：AOT 编译消除了 JIT 编译开销，启动时间显著缩短
2. **内存占用低**：减少了运行时编译所需的内存，降低了内存使用
3. **执行效率高**：本地机器码执行效率更高，特别是对于计算密集型操作
4. **部署简单**：单文件可执行文件，无需依赖外部运行时
5. **安全性强**：减少了运行时攻击面，提高了应用程序安全性

## 导航地图

```
minio/
├── index.yaml                   # 元数据索引描述
├── SKILL.md                    # 技能入口点（当前文件）
├── reference/                  # 参考文件
│   ├── README.md              # 完整功能描述
│   └── examples.md            # 使用示例
└── scripts/                    # 脚本和工具
    ├── *.cs                    # MinIO 核心实现
    ├── *.run.json              # 运行配置
    └── *.setting.json          # 设置文件
```

## 主要功能

1. **高性能对象存储操作**：基于 MinIO 客户端库的高性能实现，支持 AOT 编译优化
2. **文件上传下载**：支持大文件上传下载，断点续传，多线程并行处理
3. **批量操作**：支持批量上传下载、批量删除等操作，提高处理效率
4. **预签名 URL**：支持生成临时访问 URL，实现安全的对象共享
5. **生命周期管理**：支持对象生命周期配置，自动过期删除
6. **多租户支持**：支持多租户隔离，实现资源的安全共享
7. **高可用性**：支持 MinIO 集群，实现高可用存储
8. **安全认证**：支持多种认证方式，包括基本认证、IAM 角色等
9. **监控集成**：支持 Prometheus 监控，实时了解存储状态
10. **事件通知**：支持对象事件通知，实现存储操作的实时响应

## 扩展说明

本技能提供了完整的 MinIO 解决方案，您可以根据需要进行扩展：

1. **自定义客户端**：实现自定义 MinIO 客户端，添加特定业务逻辑
2. **扩展功能**：添加新的对象存储功能，如加密、压缩等
3. **与其他系统集成**：与数据库、消息队列等系统集成，实现数据流转
4. **性能优化**：针对特定场景优化性能，如大文件处理、高频访问等

## 最佳实践

1. **依赖注入**：使用依赖注入管理服务，提高代码可测试性和可维护性
2. **异步编程**：优先使用异步 API 避免阻塞，提高应用程序响应速度
3. **错误处理**：正确处理异常情况，实现重试机制和故障转移
4. **日志记录**：添加适当的日志记录，便于问题排查和性能分析
5. **性能监控**：监控关键性能指标，如上传下载速度、请求延迟等
6. **连接池**：使用连接池管理 MinIO 连接，减少连接建立开销
7. **重试机制**：实现合理的重试机制，提高操作成功率
8. **超时设置**：设置适当的超时时间，避免长时间阻塞

## 性能优化建议

1. **内存分配优化**：减少不必要的内存分配，使用 Span<T> 和 Memory<T> 进行零拷贝操作
2. **GC 压力优化**：减少 GC 触发次数，使用对象池复用频繁创建的对象
3. **并发优化**：使用多线程并行处理，提高处理效率
4. **批处理优化**：批量处理请求，减少网络往返次数
5. **缓存使用**：合理使用缓存，减少重复操作
6. **网络传输优化**：优化网络传输中的数据处理，使用压缩等技术
7. **分块上传**：对于大文件使用分块上传，提高可靠性和速度
8. **并行上传**：使用并行上传，充分利用网络带宽

## AOT 编译最佳实践

1. **避免反射**：使用静态分析可检测的代码，减少反射使用
2. **避免动态类型**：使用强类型，提高编译时类型检查
3. **避免运行时代码生成**：使用预编译代码，减少运行时开销
4. **优化内存使用**：使用 Span<T> 和 Memory<T>，减少内存拷贝
5. **减少依赖**：最小化依赖项，减少编译时间和可执行文件大小
6. **使用值类型**：减少 GC 压力，提高内存访问效率
7. **避免大对象分配**：避免分配大于 85KB 的对象，减少大对象堆使用
8. **使用对象池**：对于频繁创建和销毁的对象，使用对象池提高性能
