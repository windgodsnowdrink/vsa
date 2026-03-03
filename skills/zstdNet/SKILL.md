# zstdNet 技能

## 技能介绍

zstdNet 是一个基于 .NET 10 框架的高性能压缩/解压技能，提供了对 Zstandard (zstd) 压缩算法的完整封装和扩展。Zstandard 是 Facebook 开发的一种快速无损压缩算法，具有极高的压缩率和压缩/解压速度。

## 核心特性

- **高性能压缩/解压**：基于 Zstandard 算法，提供极快的压缩和解压速度
- **多级别压缩**：支持从 1 到 19 的压缩级别，满足不同场景需求
- **字典压缩**：支持使用字典进行压缩，进一步提高压缩率
- **流式处理**：支持大文件的流式压缩和解压
- **内存优化**：使用 Span<T> 和 Memory<T> 进行内存优化
- **多线程处理**：支持多线程并行压缩和解压
- **错误处理**：完善的异常处理和错误恢复机制
- **Scrutor 集成**：支持使用 Scrutor 进行自动服务注册和装饰器模式

## 安装方法

### 方法一：使用 .NET CLI

```bash
dotnet add package zstdNet
```

### 方法二：使用 NuGet 包管理器

在 Visual Studio 中打开 NuGet 包管理器，搜索 "zstdNet" 并安装。

### 方法三：手动下载

从 GitHub 仓库下载源代码，手动编译并引用。

## 注册和配置

### 在 .NET 应用中注册

```csharp
// 在 Startup.cs 或 Program.cs 中注册
var builder = WebApplication.CreateBuilder(args);

// 注册 zstdNet 服务
builder.Services.AddZstdNet();

// 配置压缩级别
builder.Services.Configure<ZstdOptions>(options =>
{
    options.CompressionLevel = 3; // 默认压缩级别
    options.BufferSize = 8192; // 缓冲区大小
});

var app = builder.Build();
// ...
```

### 配置选项

| 配置项 | 类型 | 默认值 | 描述 |
|--------|------|--------|------|
| CompressionLevel | int | 3 | 压缩级别，范围 1-19 |
| BufferSize | int | 8192 | 缓冲区大小（字节） |
| UseDictionary | bool | false | 是否使用字典压缩 |
| DictionaryPath | string | null | 字典文件路径 |
| EnableParallelCompression | bool | true | 是否启用并行压缩 |
| MaxDegreeOfParallelism | int | Environment.ProcessorCount | 最大并行度 |

## 使用示例

### 基本压缩/解压

```csharp
using zstdNet;

// 创建压缩器实例
var compressor = new ZstdCompressor();

// 压缩数据
byte[] originalData = Encoding.UTF8.GetBytes("这是一段需要压缩的数据");
byte[] compressedData = await compressor.CompressAsync(originalData);
Console.WriteLine($"原始大小: {originalData.Length} 字节");
Console.WriteLine($"压缩大小: {compressedData.Length} 字节");
Console.WriteLine($"压缩率: {(float)compressedData.Length / originalData.Length:P2}");

// 解压数据
var decompressor = new ZstdDecompressor();
byte[] decompressedData = await decompressor.DecompressAsync(compressedData);
string result = Encoding.UTF8.GetString(decompressedData);
Console.WriteLine($"解压结果: {result}");
```

### 文件压缩/解压

```csharp
using zstdNet;

// 文件压缩
var fileCompressor = new ZstdFileCompressor();
await fileCompressor.CompressFileAsync("input.txt", "output.txt.zst");

// 文件解压
var fileDecompressor = new ZstdFileDecompressor();
await fileDecompressor.DecompressFileAsync("output.txt.zst", "output.txt");
```

### 流式处理

```csharp
using zstdNet;

// 流式压缩
using (var inputStream = File.OpenRead("largefile.bin"))
using (var outputStream = File.Create("largefile.bin.zst"))
using (var compressionStream = new ZstdCompressionStream(outputStream))
{
    await inputStream.CopyToAsync(compressionStream);
}

// 流式解压
using (var inputStream = File.OpenRead("largefile.bin.zst"))
using (var outputStream = File.Create("largefile.bin"))
using (var decompressionStream = new ZstdDecompressionStream(inputStream))
{
    await decompressionStream.CopyToAsync(outputStream);
}
```

### 字典压缩

```csharp
using zstdNet;

// 创建字典
var dictionaryBuilder = new ZstdDictionaryBuilder();
byte[] dictionary = await dictionaryBuilder.BuildFromFilesAsync(new[] { "sample1.txt", "sample2.txt" });

// 使用字典压缩
var compressor = new ZstdCompressor(new ZstdOptions { UseDictionary = true, DictionaryData = dictionary });
byte[] compressedData = await compressor.CompressAsync(data);

// 使用字典解压
var decompressor = new ZstdDecompressor(new ZstdOptions { UseDictionary = true, DictionaryData = dictionary });
byte[] decompressedData = await decompressor.DecompressAsync(compressedData);
```

## 性能特性

### 压缩速度对比

| 压缩级别 | 压缩速度 (MB/s) | 解压速度 (MB/s) | 压缩率 (%) |
|----------|----------------|----------------|-----------|
| 1        | 500+           | 1500+          | 50-60%    |
| 3        | 300+           | 1500+          | 40-50%    |
| 5        | 200+           | 1400+          | 35-40%    |
| 10       | 50+            | 1200+          | 25-30%    |
| 15       | 10+            | 1000+          | 20-25%    |
| 19       | 1+             | 800+           | 15-20%    |

### 内存使用

- **小型数据**：约 1-2 MB
- **中型数据**：约 5-10 MB
- **大型数据**：根据数据大小动态调整，最大约 100 MB

### 线程使用

- **单线程模式**：仅使用一个线程
- **并行模式**：默认使用所有可用 CPU 核心
- **自定义线程数**：可通过 MaxDegreeOfParallelism 配置

## 应用场景

### 适合的场景

- **数据传输**：网络传输前压缩数据，减少带宽使用
- **数据存储**：压缩存储数据，减少存储空间
- **日志处理**：压缩日志文件，节省磁盘空间
- **备份系统**：压缩备份数据，提高备份速度和存储效率
- **实时系统**：需要快速压缩/解压的实时应用

### 不适合的场景

- **已经高度压缩的数据**：如图片、视频、音频等
- **非常小的数据**：小于 100 字节的数据，压缩可能会增加大小
- **对压缩率要求极高的场景**：如需要极致压缩率的归档存储

## 错误处理

zstdNet 提供了完善的错误处理机制，包括：

- **ZstdCompressionException**：压缩过程中的错误
- **ZstdDecompressionException**：解压过程中的错误
- **ZstdDictionaryException**：字典相关的错误
- **ZstdFileException**：文件操作相关的错误

### 错误处理示例

```csharp
using zstdNet;

try
{
    var compressor = new ZstdCompressor();
    var compressedData = await compressor.CompressAsync(data);
    // 处理压缩数据
}
catch (ZstdCompressionException ex)
{
    Console.WriteLine($"压缩失败: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"发生未知错误: {ex.Message}");
}
```

## 高级功能

### 压缩级别调优

根据不同的使用场景，选择合适的压缩级别：

- **级别 1-3**：速度优先，适合实时应用
- **级别 4-9**：平衡速度和压缩率，适合大多数场景
- **级别 10-19**：压缩率优先，适合存储场景

### 字典优化

- **静态字典**：适用于特定类型的数据，如 JSON、XML 等
- **动态字典**：根据实际数据生成字典，适合多样化的数据
- **预训练字典**：使用大量样本数据预训练字典，提高压缩率

### 并行处理

- **数据分块**：将大文件分成多个块并行处理
- **任务调度**：智能任务调度，充分利用多核 CPU
- **内存管理**：优化内存使用，避免内存溢出

## 与其他压缩库对比

| 压缩算法 | 速度 | 压缩率 | 内存使用 | 特点 |
|----------|------|--------|----------|------|
| zstd     | 极快 | 优秀 | 低 | 平衡速度和压缩率的最佳选择 |
| gzip     | 中等 | 良好 | 低 | 广泛使用，兼容性好 |
| lz4      | 极快 | 一般 | 极低 | 速度最快，适合实时场景 |
| brotli   | 慢 | 优秀 | 高 | 压缩率最高，适合静态内容 |
| deflate  | 中等 | 良好 | 低 | 标准算法，广泛支持 |

## 版本历史

### v1.0.0 (2026-01-25)

- 初始版本
- 支持基本的压缩/解压功能
- 支持文件压缩/解压
- 支持流式处理
- 支持字典压缩
- 支持多线程并行处理

### v1.1.0 (计划)

- 增加压缩率统计和分析功能
- 支持更多压缩格式的转换
- 增加压缩配置的可视化工具
- 支持更多平台和架构

## 常见问题

### Q: 压缩率不理想怎么办？

**A:** 可以尝试以下方法：
1. 提高压缩级别（如设置为 10 或更高）
2. 使用字典压缩
3. 检查数据是否已经高度压缩（如图片、视频等）

### Q: 压缩/解压速度慢怎么办？

**A:** 可以尝试以下方法：
1. 降低压缩级别（如设置为 1-3）
2. 启用并行压缩
3. 增加缓冲区大小
4. 确保使用的是最新版本

### Q: 压缩后文件变大了怎么办？

**A:** 这通常发生在以下情况：
1. 数据太小（小于 100 字节）
2. 数据已经高度压缩
3. 压缩级别设置不当

### Q: 解压失败怎么办？

**A:** 可能的原因：
1. 压缩数据损坏
2. 使用了错误的字典
3. 压缩和解压使用的版本不匹配

### Q: 内存使用过高怎么办？

**A:** 可以尝试以下方法：
1. 减小缓冲区大小
2. 禁用并行压缩
3. 使用流式处理处理大文件

## 联系和支持

### 官方网站

[https://github.com/zstdNet/zstdNet](https://github.com/zstdNet/zstdNet)

### 文档

[https://github.com/zstdNet/zstdNet/wiki](https://github.com/zstdNet/zstdNet/wiki)

### 问题反馈

[https://github.com/zstdNet/zstdNet/issues](https://github.com/zstdNet/zstdNet/issues)

### 社区支持

- Gitter: [https://gitter.im/zstdNet/community](https://gitter.im/zstdNet/community)
- Discord: [https://discord.gg/zstdNet](https://discord.gg/zstdNet)

## 许可证

zstdNet 使用 MIT 许可证，详情请查看 LICENSE 文件。

## 贡献

欢迎社区贡献！请查看 CONTRIBUTING.md 文件了解如何参与项目开发。

## 致谢

- 感谢 Facebook 开发的 Zstandard 算法
- 感谢所有为项目做出贡献的开发者
- 感谢使用和支持 zstdNet 的用户

---

**zstdNet** - 高性能 .NET 压缩/解压解决方案
