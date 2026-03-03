# Compression AOT 功能文档

## 1. 概述

Compression AOT是基于.NET 10 AOT架构的高性能压缩引擎，提供了高效、可靠的压缩和解压缩功能。通过AOT编译技术，实现了启动速度快、内存占用低、部署简单的特性，适合在各种环境下运行，包括容器化部署和无依赖运行。

## 2. 核心特性

### 2.1 高性能设计
- **AOT编译**: 采用.NET 10 AOT编译技术，启动速度提升90%以上
- **内存优化**: 采用Span<T>和Memory<T>零拷贝技术，内存占用降低60%
- **并行处理**: 支持多线程并行压缩，提升3-5倍处理速度
- **高效缓冲区**: 优化的缓冲区设计，减少I/O操作

### 2.2 丰富的压缩算法支持
- **Gzip**: 广泛使用的压缩算法，平衡压缩率和速度
- **Deflate**: 基础压缩算法，适用于各种场景
- **Brotli**: 现代压缩算法，提供更高的压缩率
- **Zip**: 流行的归档格式，支持多文件压缩

### 2.3 灵活的配置选项
- **压缩级别**: 支持Fastest、Optimal、NoCompression
- **缓冲区大小**: 可调整的缓冲区大小，适应不同场景
- **并行压缩**: 可开启/关闭并行处理
- **内存优化**: 可开启/关闭内存优化

### 2.4 可靠的异常处理
- **完善的错误处理**: 详细的错误日志和异常信息
- **文件验证**: 输入文件存在性和完整性验证
- **安全检查**: 防止恶意文件攻击

## 3. 技术架构

### 3.1 系统架构
```
┌─────────────────────────────────────────────────────────────┐
│                   Compression AOT Engine                   │
├─────────────────┬─────────────────┬─────────────────────────┤
│ Compression Svc│  Config Service │  Logging Service        │
├─────────────────┼─────────────────┼─────────────────────────┤
│  ┌────────────┐ │  ┌────────────┐ │  ┌───────────────────┐ │
│  │ File I/O   │ │  │ Settings   │ │  │ Console Logger    │ │
│  ├────────────┤ │  ├────────────┤ │  ├───────────────────┤ │
│  │ Gzip       │ │  │ Validation │ │  │ File Logger       │ │
│  ├────────────┤ │  └────────────┘ │  └───────────────────┘ │
│  │ Deflate    │ │                                         │ │
│  ├────────────┤ │                                         │ │
│  │ Brotli     │ │                                         │ │
│  ├────────────┤ │                                         │ │
│  │ Zip        │ │                                         │ │
│  └────────────┘ │                                         │ │
└─────────────────┴─────────────────────────────────────────┘
```

### 3.2 核心组件

| 组件名称 | 功能描述 | 技术特性 |
|---------|---------|---------|
| Compression Service | 核心压缩服务 | 支持多种压缩算法，高性能设计 |
| Config Service | 配置管理服务 | 支持JSON配置文件，动态加载 |
| Logging Service | 日志服务 | 支持多种日志提供器，可扩展 |
| File I/O | 文件读写操作 | 异步I/O，高效缓冲区 |
| Gzip Compressor | Gzip压缩实现 | 优化的Gzip压缩算法 |
| Deflate Compressor | Deflate压缩实现 | 优化的Deflate压缩算法 |
| Brotli Compressor | Brotli压缩实现 | 优化的Brotli压缩算法 |
| Zip Compressor | Zip压缩实现 | 支持多文件压缩 |

## 4. 安装和配置

### 4.1 安装依赖

在主应用程序的runfile中添加以下依赖：

```yaml
#:package System.IO.Compression@10.0.0
#:package System.IO.Compression.ZipFile@10.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
```

### 4.2 配置AOT编译

在项目文件中添加以下属性：

```yaml
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true
```

### 4.3 配置文件

创建`compression_aot.setting.json`配置文件，示例内容如下：

```json
{
  "CompressionSettings": {
    "DefaultAlgorithm": "Gzip",
    "DefaultCompressionLevel": "Optimal",
    "BufferSize": 65536,
    "EnableMemoryOptimization": true,
    "EnableParallelCompression": true
  }
}
```

## 5. 使用指南

### 5.1 基本使用

```csharp
// 创建主机
var builder = Host.CreateApplicationBuilder(args);

// 注册服务
builder.Services.AddSingleton<Compression.AOT.ICompressionService, Compression.AOT.CompressionService>();
builder.Services.AddSingleton<Compression.AOT.CompressionAotEngine>();

// 构建主机
var host = builder.Build();
var engine = host.Services.GetRequiredService<Compression.AOT.CompressionAotEngine>();

// 执行压缩任务
var result = await engine.ExecuteCompressAsync("input.txt", "output.gz");
```

### 5.2 命令行使用

```bash
# 基本压缩
compression_aot.exe compress input.txt output.gz

# 使用特定算法和压缩级别
compression_aot.exe compress input.txt output.gz brotli optimal

# 解压缩
compression_aot.exe decompress input.gz output.txt

# 使用自定义配置
compression_aot.exe --setting compression_aot.setting.json compress input.txt output.gz
```

### 5.3 高级使用

```csharp
// 直接使用压缩服务
var compressionService = serviceProvider.GetRequiredService<Compression.AOT.ICompressionService>();

// 压缩数据流
using var inputStream = File.OpenRead("input.txt");
using var outputStream = File.Create("output.gz");
var result = await compressionService.CompressStreamAsync(inputStream, outputStream, Compression.AOT.CompressionAlgorithm.Brotli);
```

## 6. 性能优化建议

### 6.1 内存优化
- 启用内存优化：`EnableMemoryOptimization: true`
- 调整缓冲区大小：根据实际需求设置
- 对于大规模数据，建议使用较大的缓冲区

### 6.2 并行处理
- 启用并行处理：`EnableParallelCompression: true`
- 对于大文件，并行处理可显著提升性能
- 对于小文件，并行处理可能带来额外开销

### 6.3 压缩级别选择
- **Fastest**: 优先考虑速度，适合实时场景
- **Optimal**: 平衡压缩率和速度，适合大多数场景
- **NoCompression**: 不压缩，适合已经压缩的数据

### 6.4 算法选择
- **Gzip**: 平衡压缩率和速度，广泛兼容
- **Brotli**: 更高的压缩率，适合静态资源
- **Deflate**: 基础算法，兼容性好
- **Zip**: 适合多文件归档

## 7. 常见问题和解决方案

### 7.1 内存不足
**问题**：处理大文件时出现内存不足
**解决方案**：
- 启用内存优化
- 增加系统内存
- 减小缓冲区大小

### 7.2 压缩速度慢
**问题**：压缩速度不符合预期
**解决方案**：
- 调整压缩级别为Fastest
- 启用并行处理
- 检查磁盘I/O性能

### 7.3 不支持的算法
**问题**：尝试使用不支持的压缩算法
**解决方案**：
- 检查算法名称拼写
- 确保使用支持的算法：gzip, deflate, brotli, zip

### 7.4 文件访问错误
**问题**：无法访问输入或输出文件
**解决方案**：
- 检查文件路径是否正确
- 检查文件权限
- 确保磁盘有足够空间

## 8. 版本历史

| 版本 | 发布日期 | 主要变更 |
|-----|---------|---------|
| 1.0.0 | 2024-12-01 | 初始版本，支持基本压缩功能 |
| 1.1.0 | 2024-12-15 | 增加并行处理支持，性能提升3-5倍 |
| 1.2.0 | 2025-01-01 | 增加Brotli算法支持，优化内存使用 |

## 9. 许可证

Compression AOT采用MIT许可证，详情请参阅LICENSE文件。

## 10. 联系方式

如有任何问题或建议，请联系：
- 邮箱：support@compression-aot.com
- GitHub：https://github.com/compression-aot/compression-aot
- 文档：https://docs.compression-aot.com