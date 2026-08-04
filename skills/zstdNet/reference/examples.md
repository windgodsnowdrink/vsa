# zstdNet 技能使用示例

## 1. 基本用法示例

### 1.1 基本压缩/解压

**功能说明**：对内存中的数据进行压缩和解压。

**示例代码**：

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("基本压缩/解压示例");
        Console.WriteLine("==================");

        // 创建压缩器实例
        var compressor = new ZstdCompressor();

        // 准备数据
        string originalText = "这是一段需要压缩的数据。" + 
            "Zstandard 是一种高性能的压缩算法，" +
            "由 Facebook 开发，" +
            "具有极高的压缩率和压缩/解压速度。" +
            "zstdNet 是基于 Zstandard 算法的 .NET 实现，" +
            "提供了完整的压缩/解压功能。";
        byte[] originalData = Encoding.UTF8.GetBytes(originalText);

        Console.WriteLine($"原始数据大小: {originalData.Length} 字节");
        Console.WriteLine($"原始数据: {originalText}");
        Console.WriteLine();

        // 压缩数据
        Console.WriteLine("正在压缩数据...");
        byte[] compressedData = await compressor.CompressAsync(originalData);
        Console.WriteLine($"压缩后大小: {compressedData.Length} 字节");
        Console.WriteLine($"压缩率: {(float)compressedData.Length / originalData.Length:P2}");
        Console.WriteLine();

        // 创建解压实例
        var decompressor = new ZstdDecompressor();

        // 解压数据
        Console.WriteLine("正在解压数据...");
        byte[] decompressedData = await decompressor.DecompressAsync(compressedData);
        string decompressedText = Encoding.UTF8.GetString(decompressedData);
        Console.WriteLine($"解压后大小: {decompressedData.Length} 字节");
        Console.WriteLine($"解压后数据: {decompressedText}");
        Console.WriteLine();

        // 验证数据完整性
        bool isDataIntact = originalData.SequenceEqual(decompressedData);
        Console.WriteLine($"数据完整性验证: {isDataIntact ? "成功" : "失败"}");

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }
}
```

### 1.2 文件压缩/解压

**功能说明**：直接压缩和解压文件。

**示例代码**：

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("文件压缩/解压示例");
        Console.WriteLine("==================");

        // 准备测试文件
        string inputFilePath = "test_input.txt";
        string compressedFilePath = "test_compressed.zst";
        string decompressedFilePath = "test_decompressed.txt";

        // 创建测试文件
        Console.WriteLine("正在创建测试文件...");
        await CreateTestFile(inputFilePath);

        // 获取文件大小
        long originalSize = new FileInfo(inputFilePath).Length;
        Console.WriteLine($"原始文件大小: {originalSize} 字节");
        Console.WriteLine();

        // 创建文件压缩器
        var fileCompressor = new ZstdFileCompressor();

        // 压缩文件
        Console.WriteLine("正在压缩文件...");
        await fileCompressor.CompressFileAsync(inputFilePath, compressedFilePath);
        long compressedSize = new FileInfo(compressedFilePath).Length;
        Console.WriteLine($"压缩后文件大小: {compressedSize} 字节");
        Console.WriteLine($"压缩率: {(float)compressedSize / originalSize:P2}");
        Console.WriteLine();

        // 创建文件解压
        var fileDecompressor = new ZstdFileDecompressor();

        // 解压文件
        Console.WriteLine("正在解压文件...");
        await fileDecompressor.DecompressFileAsync(compressedFilePath, decompressedFilePath);
        long decompressedSize = new FileInfo(decompressedFilePath).Length;
        Console.WriteLine($"解压后文件大小: {decompressedSize} 字节");
        Console.WriteLine();

        // 验证文件内容
        Console.WriteLine("正在验证文件内容...");
        bool isContentEqual = await CompareFiles(inputFilePath, decompressedFilePath);
        Console.WriteLine($"文件内容验证: {isContentEqual ? "成功" : "失败"}");
        Console.WriteLine();

        // 清理临时文件
        Console.WriteLine("正在清理临时文件...");
        if (File.Exists(inputFilePath)) File.Delete(inputFilePath);
        if (File.Exists(compressedFilePath)) File.Delete(compressedFilePath);
        if (File.Exists(decompressedFilePath)) File.Delete(decompressedFilePath);

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }

    static async Task CreateTestFile(string filePath)
    {
        string content = "这是一个测试文件，用于演示 zstdNet 的文件压缩/解压功能。\n" +
            "Zstandard 是一种高性能的压缩算法，由 Facebook 开发。\n" +
            "它具有极高的压缩率和压缩/解压速度。\n" +
            "zstdNet 是基于 Zstandard 算法的 .NET 实现。\n" +
            "这个文件包含了多行文本，用于测试压缩效果。\n" +
            "重复的内容可以更好地展示压缩算法的效果。\n" +
            "重复的内容可以更好地展示压缩算法的效果。\n" +
            "重复的内容可以更好地展示压缩算法的效果。\n" +
            "重复的内容可以更好地展示压缩算法的效果。\n" +
            "重复的内容可以更好地展示压缩算法的效果。\n";

        await File.WriteAllTextAsync(filePath, content);
    }

    static async Task<bool> CompareFiles(string filePath1, string filePath2)
    {
        byte[] content1 = await File.ReadAllBytesAsync(filePath1);
        byte[] content2 = await File.ReadAllBytesAsync(filePath2);
        return content1.SequenceEqual(content2);
    }
}
```

### 1.3 流式压缩/解压

**功能说明**：支持大文件的流式处理。

**示例代码**：

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("流式压缩/解压示例");
        Console.WriteLine("==================");

        // 准备测试文件
        string inputFilePath = "large_input.bin";
        string compressedFilePath = "large_compressed.zst";
        string decompressedFilePath = "large_decompressed.bin";

        // 创建大测试文件（10MB）
        Console.WriteLine("正在创建大测试文件...");
        await CreateLargeTestFile(inputFilePath, 10 * 1024 * 1024); // 10MB

        // 获取文件大小
        long originalSize = new FileInfo(inputFilePath).Length;
        Console.WriteLine($"原始文件大小: {originalSize / 1024 / 1024} MB");
        Console.WriteLine();

        // 流式压缩
        Console.WriteLine("正在流式压缩文件...");
        await StreamCompressFile(inputFilePath, compressedFilePath);
        long compressedSize = new FileInfo(compressedFilePath).Length;
        Console.WriteLine($"压缩后文件大小: {compressedSize / 1024 / 1024} MB");
        Console.WriteLine($"压缩率: {(float)compressedSize / originalSize:P2}");
        Console.WriteLine();

        // 流式解压
        Console.WriteLine("正在流式解压文件...");
        await StreamDecompressFile(compressedFilePath, decompressedFilePath);
        long decompressedSize = new FileInfo(decompressedFilePath).Length;
        Console.WriteLine($"解压后文件大小: {decompressedSize / 1024 / 1024} MB");
        Console.WriteLine();

        // 验证文件内容
        Console.WriteLine("正在验证文件内容...");
        bool isContentEqual = await CompareFiles(inputFilePath, decompressedFilePath);
        Console.WriteLine($"文件内容验证: {isContentEqual ? "成功" : "失败"}");
        Console.WriteLine();

        // 清理临时文件
        Console.WriteLine("正在清理临时文件...");
        if (File.Exists(inputFilePath)) File.Delete(inputFilePath);
        if (File.Exists(compressedFilePath)) File.Delete(compressedFilePath);
        if (File.Exists(decompressedFilePath)) File.Delete(decompressedFilePath);

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }

    static async Task CreateLargeTestFile(string filePath, long sizeInBytes)
    {
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[8192];
            new Random().NextBytes(buffer);

            long written = 0;
            while (written < sizeInBytes)
            {
                int writeSize = (int)Math.Min(buffer.Length, sizeInBytes - written);
                await stream.WriteAsync(buffer, 0, writeSize);
                written += writeSize;
            }
        }
    }

    static async Task StreamCompressFile(string inputPath, string outputPath)
    {
        using (var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
        using (var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
        using (var compressionStream = new ZstdCompressionStream(outputStream))
        {
            await inputStream.CopyToAsync(compressionStream);
        }
    }

    static async Task StreamDecompressFile(string inputPath, string outputPath)
    {
        using (var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
        using (var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
        using (var decompressionStream = new ZstdDecompressionStream(inputStream))
        {
            await decompressionStream.CopyToAsync(outputStream);
        }
    }

    static async Task<bool> CompareFiles(string filePath1, string filePath2)
    {
        using (var stream1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read))
        using (var stream2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read))
        {
            if (stream1.Length != stream2.Length)
                return false;

            byte[] buffer1 = new byte[8192];
            byte[] buffer2 = new byte[8192];

            while (true)
            {
                int read1 = await stream1.ReadAsync(buffer1, 0, buffer1.Length);
                int read2 = await stream2.ReadAsync(buffer2, 0, buffer2.Length);

                if (read1 != read2)
                    return false;

                if (read1 == 0)
                    return true;

                for (int i = 0; i < read1; i++)
                {
                    if (buffer1[i] != buffer2[i])
                        return false;
                }
            }
        }
    }
}
```

## 2. 高级用法示例

### 2.1 多级别压缩

**功能说明**：演示不同压缩级别的效果。

**示例代码**：

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("多级别压缩示例");
        Console.WriteLine("================");

        // 准备数据
        string originalText = "这是一段需要压缩的数据。" + 
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。" +
            "重复的内容可以更好地展示不同压缩级别的效果。";
        byte[] originalData = Encoding.UTF8.GetBytes(originalText);

        Console.WriteLine($"原始数据大小: {originalData.Length} 字节");
        Console.WriteLine();

        // 测试不同压缩级别
        int[] compressionLevels = { 1, 3, 5, 10, 15 };

        foreach (int level in compressionLevels)
        {
            Console.WriteLine($"测试压缩级别: {level}");
            
            // 创建指定级别的压缩器
            var compressor = new ZstdCompressor(new ZstdOptions { CompressionLevel = level });

            // 测量压缩时间
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            byte[] compressedData = await compressor.CompressAsync(originalData);
            stopwatch.Stop();

            // 计算压缩率和速度
            double compressionRatio = (double)compressedData.Length / originalData.Length;
            double compressionSpeed = (double)originalData.Length / 1024 / 1024 / stopwatch.Elapsed.TotalSeconds;

            Console.WriteLine($"压缩后大小: {compressedData.Length} 字节");
            Console.WriteLine($"压缩率: {compressionRatio:P2}");
            Console.WriteLine($"压缩时间: {stopwatch.Elapsed.TotalMilliseconds:F2} 毫秒");
            Console.WriteLine($"压缩速度: {compressionSpeed:F2} MB/s");
            Console.WriteLine();
        }

        Console.WriteLine("示例完成。按任意键退出...");
        Console.ReadKey();
    }
}
```

### 2.2 字典压缩

**功能说明**：使用字典提高压缩率。

**示例代码**：

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("字典压缩示例");
        Console.WriteLine("============");

        // 准备字典数据
        Console.WriteLine("正在准备字典数据...");
        var dictionaryBuilder = new ZstdDictionaryBuilder();

        // 从示例文本构建字典
        var sampleTexts = new List<string>
        {
            "{\"name\": \"John\", \"age\": 30, \"city\": \"New York\"}",
            "{\"name\": \"Jane\", \"age\": 25, \"city\": \"Los Angeles\"}",
            "{\"name\": \"Bob\", \"age\": 35, \"city\": \"Chicago\"}",
            "{\"name\": \"Alice\", \"age\": 28, \"city\": \"Houston\"}"
        };

        var sampleData = sampleTexts.Select(text => Encoding.UTF8.GetBytes(text)).ToList();
        byte[] dictionary = await dictionaryBuilder.BuildFromDataAsync(sampleData);
        Console.WriteLine($"字典大小: {dictionary.Length} 字节");
        Console.WriteLine();

        // 准备测试数据
        string testJson = "{\"name\": \"Mike\", \"age\": 40, \"city\": \"Miami\"}";
        byte[] testData = Encoding.UTF8.GetBytes(testJson);
        Console.WriteLine($"测试数据: {testJson}");
        Console.WriteLine($"测试数据大小: {testData.Length} 字节");
        Console.WriteLine();

        // 不使用字典压缩
        Console.WriteLine("不使用字典压缩:");
        var regularCompressor = new ZstdCompressor();
        byte[] regularCompressed = await regularCompressor.CompressAsync(testData);
        Console.WriteLine($"压缩后大小: {regularCompressed.Length} 字节");
        Console.WriteLine($"压缩率: {(float)regularCompressed.Length / testData.Length:P2}");
        Console.WriteLine();

        // 使用字典压缩
        Console.WriteLine("使用字典压缩:");
        var dictionaryCompressor = new ZstdCompressor(new ZstdOptions 
        { 
            UseDictionary = true, 
            DictionaryData = dictionary 
        });
        byte[] dictionaryCompressed = await dictionaryCompressor.CompressAsync(testData);
        Console.WriteLine($"压缩后大小: {dictionaryCompressed.Length} 字节");
        Console.WriteLine($"压缩率: {(float)dictionaryCompressed.Length / testData.Length:P2}");
        Console.WriteLine($"字典压缩优势: {(float)dictionaryCompressed.Length / regularCompressed.Length:P2}");
        Console.WriteLine();

        // 解压验证
        Console.WriteLine("验证解压:");
        var dictionaryDecompressor = new ZstdDecompressor(new ZstdOptions 
        { 
            UseDictionary = true, 
            DictionaryData = dictionary 
        });
        byte[] decompressedData = await dictionaryDecompressor.DecompressAsync(dictionaryCompressed);
        string decompressedJson = Encoding.UTF8.GetString(decompressedData);
        Console.WriteLine($"解压后数据: {decompressedJson}");
        Console.WriteLine($"数据完整性: {testJson == decompressedJson}");
        Console.WriteLine();

        Console.WriteLine("示例完成。按任意键退出...");
        Console.ReadKey();
    }
}
```

### 2.3 多线程并行压缩

**功能说明**：使用多线程并行压缩提高性能。

**示例代码**：

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("多线程并行压缩示例");
        Console.WriteLine("==================");

        // 准备测试文件（20MB）
        string inputFilePath = "parallel_input.bin";
        string singleThreadOutputPath = "single_thread_compressed.zst";
        string multiThreadOutputPath = "multi_thread_compressed.zst";

        // 创建测试文件
        Console.WriteLine("正在创建测试文件...");
        await CreateTestFile(inputFilePath, 20 * 1024 * 1024); // 20MB

        long originalSize = new FileInfo(inputFilePath).Length;
        Console.WriteLine($"测试文件大小: {originalSize / 1024 / 1024} MB");
        Console.WriteLine();

        // 单线程压缩
        Console.WriteLine("单线程压缩:");
        var singleThreadCompressor = new ZstdFileCompressor(new ZstdOptions
        {
            CompressionLevel = 5,
            EnableParallelCompression = false
        });

        var stopwatch = Stopwatch.StartNew();
        await singleThreadCompressor.CompressFileAsync(inputFilePath, singleThreadOutputPath);
        stopwatch.Stop();

        long singleThreadSize = new FileInfo(singleThreadOutputPath).Length;
        Console.WriteLine($"压缩后大小: {singleThreadSize / 1024 / 1024} MB");
        Console.WriteLine($"压缩时间: {stopwatch.Elapsed.TotalSeconds:F2} 秒");
        Console.WriteLine($"压缩速度: {originalSize / 1024 / 1024 / stopwatch.Elapsed.TotalSeconds:F2} MB/s");
        Console.WriteLine();

        // 多线程压缩
        Console.WriteLine("多线程压缩:");
        var multiThreadCompressor = new ZstdFileCompressor(new ZstdOptions
        {
            CompressionLevel = 5,
            EnableParallelCompression = true,
            MaxDegreeOfParallelism = Environment.ProcessorCount
        });

        stopwatch.Restart();
        await multiThreadCompressor.CompressFileAsync(inputFilePath, multiThreadOutputPath);
        stopwatch.Stop();

        long multiThreadSize = new FileInfo(multiThreadOutputPath).Length;
        Console.WriteLine($"压缩后大小: {multiThreadSize / 1024 / 1024} MB");
        Console.WriteLine($"压缩时间: {stopwatch.Elapsed.TotalSeconds:F2} 秒");
        Console.WriteLine($"压缩速度: {originalSize / 1024 / 1024 / stopwatch.Elapsed.TotalSeconds:F2} MB/s");
        Console.WriteLine();

        // 比较结果
        Console.WriteLine("比较结果:");
        double speedup = (double)singleThreadSize / multiThreadSize;
        double timeReduction = 1.0 - (double)stopwatch.Elapsed.TotalSeconds / (singleThreadCompressor as IDisposable).GetType().GetMethod("GetElapsedTime").Invoke(singleThreadCompressor, null);
        Console.WriteLine($"多线程速度提升: {speedup:F2}x");
        Console.WriteLine($"时间减少: {timeReduction:P2}");
        Console.WriteLine();

        // 清理临时文件
        Console.WriteLine("清理临时文件...");
        if (File.Exists(inputFilePath)) File.Delete(inputFilePath);
        if (File.Exists(singleThreadOutputPath)) File.Delete(singleThreadOutputPath);
        if (File.Exists(multiThreadOutputPath)) File.Delete(multiThreadOutputPath);

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }

    static async Task CreateTestFile(string filePath, long sizeInBytes)
    {
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            byte[] buffer = new byte[8192];
            new Random().NextBytes(buffer);

            long written = 0;
            while (written < sizeInBytes)
            {
                int writeSize = (int)Math.Min(buffer.Length, sizeInBytes - written);
                await stream.WriteAsync(buffer, 0, writeSize);
                written += writeSize;
            }
        }
    }
}
```

## 3. Scrutor 集成示例

### 3.1 自动服务注册

**功能说明**：使用 Scrutor 自动注册 zstdNet 服务。

**示例代码**：

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Scrutor 自动服务注册示例");
        Console.WriteLine("========================");

        // 构建服务容器
        var services = new ServiceCollection();

        // 使用 Scrutor 自动注册服务
        services.Scan(scan => scan
            .FromAssemblyOf<ZstdCompressor>()
            .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Compressor") || c.Name.EndsWith("Decompressor")))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        // 配置压缩选项
        services.Configure<ZstdOptions>(options =>
        {
            options.CompressionLevel = 5;
            options.EnableParallelCompression = true;
        });

        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();

        // 获取压缩器和解压实例
        var compressor = serviceProvider.GetRequiredService<IZstdCompressor>();
        var decompressor = serviceProvider.GetRequiredService<IZstdDecompressor>();

        Console.WriteLine("服务注册成功!");
        Console.WriteLine();

        // 测试压缩/解压
        string testText = "这是一段测试数据，用于验证 Scrutor 自动服务注册功能。";
        byte[] testData = Encoding.UTF8.GetBytes(testText);

        Console.WriteLine($"测试数据: {testText}");
        Console.WriteLine($"原始大小: {testData.Length} 字节");
        Console.WriteLine();

        // 压缩
        byte[] compressedData = await compressor.CompressAsync(testData);
        Console.WriteLine($"压缩后大小: {compressedData.Length} 字节");
        Console.WriteLine($"压缩率: {(float)compressedData.Length / testData.Length:P2}");
        Console.WriteLine();

        // 解压
        byte[] decompressedData = await decompressor.DecompressAsync(compressedData);
        string decompressedText = Encoding.UTF8.GetString(decompressedData);
        Console.WriteLine($"解压后数据: {decompressedText}");
        Console.WriteLine($"解压后大小: {decompressedData.Length} 字节");
        Console.WriteLine();

        // 验证
        bool isSuccess = testText == decompressedText;
        Console.WriteLine($"测试结果: {isSuccess ? "成功" : "失败"}");

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }
}
```

### 3.2 装饰器模式

**功能说明**：使用 Scrutor 装饰器模式扩展 zstdNet 功能。

**示例代码**：

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Scrutor 装饰器模式示例");
        Console.WriteLine("====================");

        // 构建服务容器
        var services = new ServiceCollection();

        // 注册原始服务
        services.AddTransient<IZstdCompressor, ZstdCompressor>();
        services.AddTransient<IZstdDecompressor, ZstdDecompressor>();

        // 使用 Scrutor 添加装饰器
        services.Decorate<IZstdCompressor, LoggingCompressorDecorator>();
        services.Decorate<IZstdDecompressor, MetricsDecompressorDecorator>();

        // 构建服务提供程序
        using var serviceProvider = services.BuildServiceProvider();

        // 获取装饰后的服务
        var compressor = serviceProvider.GetRequiredService<IZstdCompressor>();
        var decompressor = serviceProvider.GetRequiredService<IZstdDecompressor>();

        Console.WriteLine("装饰器注册成功!");
        Console.WriteLine();

        // 测试压缩/解压
        string testText = "这是一段测试数据，用于验证 Scrutor 装饰器模式功能。";
        byte[] testData = Encoding.UTF8.GetBytes(testText);

        Console.WriteLine($"测试数据: {testText}");
        Console.WriteLine($"原始大小: {testData.Length} 字节");
        Console.WriteLine();

        // 压缩（会触发日志装饰器）
        byte[] compressedData = await compressor.CompressAsync(testData);
        Console.WriteLine();

        // 解压（会触发指标装饰器）
        byte[] decompressedData = await decompressor.DecompressAsync(compressedData);
        string decompressedText = Encoding.UTF8.GetString(decompressedData);
        Console.WriteLine();

        // 验证
        bool isSuccess = testText == decompressedText;
        Console.WriteLine($"测试结果: {isSuccess ? "成功" : "失败"}");

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }
}

// 日志装饰器
public class LoggingCompressorDecorator : IZstdCompressor
{
    private readonly IZstdCompressor _decorated;

    public LoggingCompressorDecorator(IZstdCompressor decorated)
    {
        _decorated = decorated;
    }

    public async Task<byte[]> CompressAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[日志] 开始压缩数据，大小: {data.Length} 字节");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var result = await _decorated.CompressAsync(data, cancellationToken);

        stopwatch.Stop();
        Console.WriteLine($"[日志] 压缩完成，大小: {result.Length} 字节");
        Console.WriteLine($"[日志] 压缩率: {(float)result.Length / data.Length:P2}");
        Console.WriteLine($"[日志] 压缩时间: {stopwatch.Elapsed.TotalMilliseconds:F2} 毫秒");

        return result;
    }

    public async Task<byte[]> CompressAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return await CompressAsync(data.ToArray(), cancellationToken);
    }

    public async Task<byte[]> CompressAsync(ReadOnlySpan<byte> data, CancellationToken cancellationToken = default)
    {
        return await CompressAsync(data.ToArray(), cancellationToken);
    }
}

// 指标装饰器
public class MetricsDecompressorDecorator : IZstdDecompressor
{
    private readonly IZstdDecompressor _decorated;
    private int _decompressionCount = 0;
    private long _totalOriginalSize = 0;
    private long _totalDecompressedSize = 0;
    private TimeSpan _totalDecompressionTime = TimeSpan.Zero;

    public MetricsDecompressorDecorator(IZstdDecompressor decorated)
    {
        _decorated = decorated;
    }

    public async Task<byte[]> DecompressAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        _decompressionCount++;
        _totalOriginalSize += data.Length;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _decorated.DecompressAsync(data, cancellationToken);
        stopwatch.Stop();

        _totalDecompressedSize += result.Length;
        _totalDecompressionTime += stopwatch.Elapsed;

        Console.WriteLine($"[指标] 解压完成，编号: {_decompressionCount}");
        Console.WriteLine($"[指标] 原始大小: {data.Length} 字节");
        Console.WriteLine($"[指标] 解压后大小: {result.Length} 字节");
        Console.WriteLine($"[指标] 解压时间: {stopwatch.Elapsed.TotalMilliseconds:F2} 毫秒");
        Console.WriteLine($"[指标] 累计解压次数: {_decompressionCount}");
        Console.WriteLine($"[指标] 累计原始大小: {_totalOriginalSize} 字节");
        Console.WriteLine($"[指标] 累计解压后大小: {_totalDecompressedSize} 字节");
        Console.WriteLine($"[指标] 累计解压时间: {_totalDecompressionTime.TotalMilliseconds:F2} 毫秒");

        return result;
    }

    public async Task<byte[]> DecompressAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return await DecompressAsync(data.ToArray(), cancellationToken);
    }

    public async Task<byte[]> DecompressAsync(ReadOnlySpan<byte> data, CancellationToken cancellationToken = default)
    {
        return await DecompressAsync(data.ToArray(), cancellationToken);
    }
}
```

## 4. 性能测试示例

### 4.1 压缩速度测试

**功能说明**：测试不同数据类型的压缩速度。

**示例代码**：

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("压缩速度测试示例");
        Console.WriteLine("==================");

        // 准备不同类型的测试数据
        var testData = new Dictionary<string, byte[]>
        {
            { "文本数据", Encoding.UTF8.GetBytes(CreateTextData(10 * 1024 * 1024)) }, // 10MB
            { "JSON数据", Encoding.UTF8.GetBytes(CreateJsonData(5 * 1024 * 1024)) },   // 5MB
            { "随机数据", CreateRandomData(8 * 1024 * 1024) },                    // 8MB
            { "重复数据", CreateRepeatedData(12 * 1024 * 1024) }                   // 12MB
        };

        // 测试不同压缩级别
        int[] compressionLevels = { 1, 3, 5, 10 };

        foreach (var (dataType, data) in testData)
        {
            Console.WriteLine($"\n测试 {dataType} (大小: {data.Length / 1024 / 1024} MB)");
            Console.WriteLine(new string('-', 60));

            foreach (int level in compressionLevels)
            {
                // 创建压缩器
                var compressor = new ZstdCompressor(new ZstdOptions { CompressionLevel = level });

                // 测量压缩时间
                var stopwatch = Stopwatch.StartNew();
                byte[] compressedData = await compressor.CompressAsync(data);
                stopwatch.Stop();

                // 计算结果
                double compressionRatio = (double)compressedData.Length / data.Length;
                double compressionSpeed = (double)data.Length / 1024 / 1024 / stopwatch.Elapsed.TotalSeconds;

                Console.WriteLine($"级别 {level}: 压缩率 = {compressionRatio:P2}, 速度 = {compressionSpeed:F2} MB/s, 时间 = {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
            }
        }

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }

    static string CreateTextData(int sizeInBytes)
    {
        StringBuilder sb = new StringBuilder(sizeInBytes);
        string sampleText = "这是一段示例文本，用于测试压缩算法的性能。重复的内容可以更好地展示压缩效果。";
        
        while (sb.Length < sizeInBytes)
        {
            sb.Append(sampleText);
        }
        
        return sb.ToString(0, sizeInBytes);
    }

    static string CreateJsonData(int sizeInBytes)
    {
        StringBuilder sb = new StringBuilder(sizeInBytes);
        string sampleJson = '{"id":{0},"name":"User{0}","email":"user{0}@example.com","age":{1},"city":"City{2}","country":"Country{3}","active":true},';
        
        int counter = 0;
        while (sb.Length < sizeInBytes)
        {
            sb.AppendFormat(sampleJson, counter, counter % 100, counter % 10, counter % 5);
            counter++;
        }
        
        // 确保 JSON 格式正确
        string json = "[{" + sb.ToString(0, sizeInBytes - 2) + "]";
        return json.Substring(0, Math.Min(sizeInBytes, json.Length));
    }

    static byte[] CreateRandomData(int sizeInBytes)
    {
        byte[] data = new byte[sizeInBytes];
        new Random().NextBytes(data);
        return data;
    }

    static byte[] CreateRepeatedData(int sizeInBytes)
    {
        byte[] pattern = Encoding.UTF8.GetBytes("重复的模式重复的模式重复的模式");
        byte[] data = new byte[sizeInBytes];
        
        for (int i = 0; i < sizeInBytes; i++)
        {
            data[i] = pattern[i % pattern.Length];
        }
        
        return data;
    }
}
```

### 4.2 内存使用测试

**功能说明**：测试不同操作的内存使用情况。

**示例代码**：

```csharp
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using zstdNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("内存使用测试示例");
        Console.WriteLine("==================");

        // 获取初始内存使用
        var initialMemory = Process.GetCurrentProcess().PrivateMemorySize64;
        Console.WriteLine($"初始内存使用: {initialMemory / 1024 / 1024:F2} MB");
        Console.WriteLine();

        // 测试不同大小数据的内存使用
        int[] dataSizes = { 1 * 1024 * 1024, 5 * 1024 * 1024, 10 * 1024 * 1024, 50 * 1024 * 1024 };

        foreach (int size in dataSizes)
        {
            Console.WriteLine($"测试 {size / 1024 / 1024} MB 数据");
            Console.WriteLine(new string('-', 60));

            // 准备数据
            byte[] testData = new byte[size];
            new Random().NextBytes(testData);

            // 测量压缩内存使用
            var compressor = new ZstdCompressor(new ZstdOptions { CompressionLevel = 5 });
            var compressMemoryBefore = Process.GetCurrentProcess().PrivateMemorySize64;

            var stopwatch = Stopwatch.StartNew();
            byte[] compressedData = await compressor.CompressAsync(testData);
            stopwatch.Stop();

            var compressMemoryAfter = Process.GetCurrentProcess().PrivateMemorySize64;
            var compressMemoryUsed = (compressMemoryAfter - compressMemoryBefore) / 1024 / 1024;

            Console.WriteLine($"压缩: 内存使用增加 = {compressMemoryUsed:F2} MB, 时间 = {stopwatch.Elapsed.TotalMilliseconds:F2} ms");

            // 测量解压内存使用
            var decompressor = new ZstdDecompressor();
            var decompressMemoryBefore = Process.GetCurrentProcess().PrivateMemorySize64;

            stopwatch.Restart();
            byte[] decompressedData = await decompressor.DecompressAsync(compressedData);
            stopwatch.Stop();

            var decompressMemoryAfter = Process.GetCurrentProcess().PrivateMemorySize64;
            var decompressMemoryUsed = (decompressMemoryAfter - decompressMemoryBefore) / 1024 / 1024;

            Console.WriteLine($"解压: 内存使用增加 = {decompressMemoryUsed:F2} MB, 时间 = {stopwatch.Elapsed.TotalMilliseconds:F2} ms");
            Console.WriteLine();

            // 清理
            Array.Clear(testData, 0, testData.Length);
            Array.Clear(compressedData, 0, compressedData.Length);
            Array.Clear(decompressedData, 0, decompressedData.Length);

            // 强制垃圾回收
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        // 获取最终内存使用
        var finalMemory = Process.GetCurrentProcess().PrivateMemorySize64;
        Console.WriteLine($"最终内存使用: {finalMemory / 1024 / 1024:F2} MB");
        Console.WriteLine($"内存变化: {(finalMemory - initialMemory) / 1024 / 1024:F2} MB");

        Console.WriteLine("\n示例完成。按任意键退出...");
        Console.ReadKey();
    }
}
```

## 5. 实际应用示例

### 5.1 Web API 压缩中间件

**功能说明**：在 ASP.NET Core Web API 中使用 zstdNet 进行响应压缩。

**示例代码**：

```csharp
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using zstdNet;

public class ZstdCompressionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ZstdCompressor _compressor;

    public ZstdCompressionMiddleware(RequestDelegate next)
    {
        _next = next;
        _compressor = new ZstdCompressor(new ZstdOptions { CompressionLevel = 3 });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 检查客户端是否支持 zstd 压缩
        var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();
        if (!acceptEncoding.Contains("zstd"))
        {
            await _next(context);
            return;
        }

        // 替换响应流
        var originalBodyStream = context.Response.Body;
        using var compressedBodyStream = new MemoryStream();
        context.Response.Body = compressedBodyStream;

        try
        {
            // 处理请求
            await _next(context);

            // 压缩响应
            compressedBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBytes = await new BinaryReader(compressedBodyStream).ReadBytesAsync((int)compressedBodyStream.Length);
            var compressedBytes = await _compressor.CompressAsync(responseBytes);

            // 设置响应头
            context.Response.Headers["Content-Encoding"] = "zstd";
            context.Response.Headers["Content-Length"] = compressedBytes.Length.ToString();
            context.Response.Headers["Vary"] = "Accept-Encoding";

            // 写入压缩响应
            context.Response.Body = originalBodyStream;
            await context.Response.Body.WriteAsync(compressedBytes);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

public static class ZstdCompressionMiddlewareExtensions
{
    public static IApplicationBuilder UseZstdCompression(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ZstdCompressionMiddleware>();
    }
}

// 在 Startup.cs 中使用
// app.UseZstdCompression();
```

### 5.2 日志压缩存储

**功能说明**：使用 zstdNet 压缩日志文件，节省存储空间。

**示例代码**：

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using zstdNet;

public class LogCompressor
{
    private readonly ZstdFileCompressor _compressor;

    public LogCompressor()
    {
        _compressor = new ZstdFileCompressor(new ZstdOptions { CompressionLevel = 7 });
    }

    public async Task CompressLogFileAsync(string logFilePath)
    {
        if (!File.Exists(logFilePath))
        {
            throw new FileNotFoundException("日志文件不存在", logFilePath);
        }

        string compressedFilePath = logFilePath + ".zst";

        Console.WriteLine($"正在压缩日志文件: {logFilePath}");
        Console.WriteLine($"原始大小: {new FileInfo(logFilePath).Length / 1024 / 1024:F2} MB");

        await _compressor.CompressFileAsync(logFilePath, compressedFilePath);

        var originalSize = new FileInfo(logFilePath).Length;
        var compressedSize = new FileInfo(compressedFilePath).Length;
        var compressionRatio = (double)compressedSize / originalSize;

        Console.WriteLine($"压缩完成: {compressedFilePath}");
        Console.WriteLine($"压缩后大小: {compressedSize / 1024 / 1024:F2} MB");
        Console.WriteLine($"压缩率: {compressionRatio:P2}");
        Console.WriteLine($"节省空间: {(1 - compressionRatio) * 100:P2}");

        // 可选：删除原始日志文件
        // File.Delete(logFilePath);
    }

    public async Task CompressLogDirectoryAsync(string directoryPath, string searchPattern = "*.log")
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException("日志目录不存在", directoryPath);
        }

        var logFiles = Directory.GetFiles(directoryPath, searchPattern);
        Console.WriteLine($"找到 {logFiles.Length} 个日志文件");

        foreach (var logFile in logFiles)
        {
            // 跳过已压缩的文件
            if (logFile.EndsWith(".zst"))
                continue;

            await CompressLogFileAsync(logFile);
            Console.WriteLine();
        }
    }
}

// 使用示例
// var logCompressor = new LogCompressor();
// await logCompressor.CompressLogDirectoryAsync(@"C:\Logs");
```

### 5.3 网络传输压缩

**功能说明**：在网络传输前压缩数据，减少带宽使用。

**示例代码**：

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using zstdNet;

public class CompressedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ZstdCompressor _compressor;
    private readonly ZstdDecompressor _decompressor;

    public CompressedHttpClient()
    {
        _httpClient = new HttpClient();
        _compressor = new ZstdCompressor(new ZstdOptions { CompressionLevel = 3 });
        _decompressor = new ZstdDecompressor();
    }

    public async Task<string> PostCompressedAsync(string url, string content)
    {
        // 压缩内容
        byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        byte[] compressedBytes = await _compressor.CompressAsync(contentBytes);

        Console.WriteLine($"原始内容大小: {contentBytes.Length} 字节");
        Console.WriteLine($"压缩后大小: {compressedBytes.Length} 字节");
        Console.WriteLine($"压缩率: {(float)compressedBytes.Length / contentBytes.Length:P2}");

        // 创建压缩请求
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new ByteArrayContent(compressedBytes);
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        request.Content.Headers.Add("Content-Encoding", "zstd");
        request.Content.Headers.ContentLength = compressedBytes.Length;

        // 发送请求
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        // 处理响应
        var responseContent = await response.Content.ReadAsByteArrayAsync();
        string responseText;

        // 检查响应是否压缩
        if (response.Content.Headers.Contains("Content-Encoding") && 
            response.Content.Headers.GetValues("Content-Encoding").Contains("zstd"))
        {
            // 解压响应
            byte[] decompressedResponse = await _decompressor.DecompressAsync(responseContent);
            responseText = Encoding.UTF8.GetString(decompressedResponse);
            Console.WriteLine($"响应压缩大小: {responseContent.Length} 字节");
            Console.WriteLine($"响应解压大小: {decompressedResponse.Length} 字节");
        }
        else
        {
            // 直接处理响应
            responseText = Encoding.UTF8.GetString(responseContent);
        }

        return responseText;
    }
}

// 使用示例
// var client = new CompressedHttpClient();
// string content = "{\"data\": \"大量数据需要传输\"}";
// string response = await client.PostCompressedAsync("https://api.example.com/data", content);
// Console.WriteLine("响应: " + response);
```

## 6. 总结

本示例文档提供了 zstdNet 技能的全面使用示例，包括：

- **基本用法**：基本压缩/解压、文件压缩/解压、流式压缩/解压
- **高级用法**：多级别压缩、字典压缩、多线程并行压缩
- **Scrutor 集成**：自动服务注册、装饰器模式
- **性能测试**：压缩速度测试、内存使用测试
- **实际应用**：Web API 压缩中间件、日志压缩存储、网络传输压缩

这些示例展示了 zstdNet 的强大功能和灵活性，可根据实际需求选择合适的使用方式。

---

**zstdNet** - 高性能 .NET 压缩/解压解决方案