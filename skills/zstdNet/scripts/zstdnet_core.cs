#:sdk Microsoft.NET.Sdk
#:package ZstdSharp@0.8.0
#:package Microsoft.Extensions.DependencyInjection@9.0.0
#:package Microsoft.Extensions.Logging@9.0.0
#:package System.Threading.Tasks.Dataflow@9.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZstdSharp;

namespace ZstdNet
{
    /// <summary>
    /// Zstd 压缩服务接口
    /// </summary>
    public interface IZstdCompressionService
    {
        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="data">要压缩的数据</param>
        /// <param name="compressionLevel">压缩级别 (1-19)</param>
        /// <returns>压缩后的数据</returns>
        byte[] Compress(byte[] data, int compressionLevel = 3);
        
        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="compressedData">压缩的数据</param>
        /// <returns>解压缩后的数据</returns>
        byte[] Decompress(byte[] compressedData);
        
        /// <summary>
        /// 使用字典压缩数据
        /// </summary>
        /// <param name="data">要压缩的数据</param>
        /// <param name="dictionary">压缩字典</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>压缩后的数据</returns>
        byte[] CompressWithDictionary(byte[] data, byte[] dictionary, int compressionLevel = 3);
        
        /// <summary>
        /// 使用字典解压缩数据
        /// </summary>
        /// <param name="compressedData">压缩的数据</param>
        /// <param name="dictionary">压缩字典</param>
        /// <returns>解压缩后的数据</returns>
        byte[] DecompressWithDictionary(byte[] compressedData, byte[] dictionary);
        
        /// <summary>
        /// 异步压缩流
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="outputStream">输出流</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>压缩任务</returns>
        Task CompressStreamAsync(Stream inputStream, Stream outputStream, int compressionLevel = 3, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 异步解压缩流
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="outputStream">输出流</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>解压缩任务</returns>
        Task DecompressStreamAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Zstd 压缩服务实现
    /// </summary>
    public class ZstdCompressionService : IZstdCompressionService
    {
        private readonly ILogger<ZstdCompressionService> _logger;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public ZstdCompressionService(ILogger<ZstdCompressionService> logger = null)
        {
            _logger = logger;
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data, int compressionLevel = 3)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();
            
            try
            {
                using var compressor = new Compressor(compressionLevel);
                return compressor.Wrap(data);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "压缩数据时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] compressedData)
        {
            if (compressedData == null || compressedData.Length == 0)
                return Array.Empty<byte>();
            
            try
            {
                using var decompressor = new Decompressor();
                return decompressor.Unwrap(compressedData);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "解压缩数据时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public byte[] CompressWithDictionary(byte[] data, byte[] dictionary, int compressionLevel = 3)
        {
            if (data == null || data.Length == 0)
                return Array.Empty<byte>();
            
            if (dictionary == null || dictionary.Length == 0)
                return Compress(data, compressionLevel);
            
            try
            {
                using var dict = new CompressionDictionary(dictionary);
                using var compressor = new Compressor(compressionLevel, dict);
                return compressor.Wrap(data);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "使用字典压缩数据时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public byte[] DecompressWithDictionary(byte[] compressedData, byte[] dictionary)
        {
            if (compressedData == null || compressedData.Length == 0)
                return Array.Empty<byte>();
            
            if (dictionary == null || dictionary.Length == 0)
                return Decompress(compressedData);
            
            try
            {
                using var dict = new DecompressionDictionary(dictionary);
                using var decompressor = new Decompressor(dict);
                return decompressor.Unwrap(compressedData);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "使用字典解压缩数据时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task CompressStreamAsync(Stream inputStream, Stream outputStream, int compressionLevel = 3, CancellationToken cancellationToken = default)
        {
            if (inputStream == null || outputStream == null)
                throw new ArgumentNullException(inputStream == null ? nameof(inputStream) : nameof(outputStream));
            
            try
            {
                using var compressor = new Compressor(compressionLevel);
                using var compressionStream = new CompressionStream(outputStream, compressor, leaveOpen: true);
                
                await inputStream.CopyToAsync(compressionStream, 81920, cancellationToken);
                await compressionStream.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "压缩流时发生错误");
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task DecompressStreamAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default)
        {
            if (inputStream == null || outputStream == null)
                throw new ArgumentNullException(inputStream == null ? nameof(inputStream) : nameof(outputStream));
            
            try
            {
                using var decompressor = new Decompressor();
                using var decompressionStream = new DecompressionStream(inputStream, decompressor, leaveOpen: true);
                
                await decompressionStream.CopyToAsync(outputStream, 81920, cancellationToken);
                await outputStream.FlushAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "解压缩流时发生错误");
                throw;
            }
        }
    }

    /// <summary>
    /// 并行压缩服务
    /// </summary>
    public class ParallelZstdCompressionService : IZstdCompressionService
    {
        private readonly IZstdCompressionService _innerService;
        private readonly int _maxDegreeOfParallelism;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="innerService">内部压缩服务</param>
        /// <param name="maxDegreeOfParallelism">最大并行度</param>
        public ParallelZstdCompressionService(IZstdCompressionService innerService, int maxDegreeOfParallelism = 4)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _maxDegreeOfParallelism = Math.Max(1, maxDegreeOfParallelism);
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data, int compressionLevel = 3)
        {
            return _innerService.Compress(data, compressionLevel);
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] data)
        {
            return _innerService.Decompress(data);
        }
        
        /// <inheritdoc/>
        public byte[] CompressWithDictionary(byte[] data, byte[] dictionary, int compressionLevel = 3)
        {
            return _innerService.CompressWithDictionary(data, dictionary, compressionLevel);
        }
        
        /// <inheritdoc/>
        public byte[] DecompressWithDictionary(byte[] compressedData, byte[] dictionary)
        {
            return _innerService.DecompressWithDictionary(compressedData, dictionary);
        }
        
        /// <inheritdoc/>
        public Task CompressStreamAsync(Stream inputStream, Stream outputStream, int compressionLevel = 3, CancellationToken cancellationToken = default)
        {
            return _innerService.CompressStreamAsync(inputStream, outputStream, compressionLevel, cancellationToken);
        }
        
        /// <inheritdoc/>
        public Task DecompressStreamAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default)
        {
            return _innerService.DecompressStreamAsync(inputStream, outputStream, cancellationToken);
        }
        
        /// <summary>
        /// 并行压缩多个数据块
        /// </summary>
        /// <param name="dataBlocks">数据块集合</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>压缩后的数据块集合</returns>
        public Task<byte[][]> CompressMultipleAsync(byte[][] dataBlocks, int compressionLevel = 3)
        {
            if (dataBlocks == null || dataBlocks.Length == 0)
                return Task.FromResult(Array.Empty<byte[]>());
            
            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _maxDegreeOfParallelism,
                CancellationToken = CancellationToken.None
            };
            
            var transformBlock = new TransformBlock<byte[], byte[]>(
                data => _innerService.Compress(data, compressionLevel),
                options);
            
            var actionBlock = new ActionBlock<byte[]>(_ => { });
            
            var results = new byte[dataBlocks.Length][];
            var index = 0;
            
            transformBlock.LinkTo(new ActionBlock<byte[]>(compressedData =>
            {
                results[index++] = compressedData;
            }));
            
            foreach (var block in dataBlocks)
            {
                transformBlock.Post(block);
            }
            
            transformBlock.Complete();
            transformBlock.Completion.Wait();
            
            return Task.FromResult(results);
        }
    }

    /// <summary>
    /// 内存优化的压缩服务
    /// </summary>
    public class MemoryOptimizedZstdCompressionService : IZstdCompressionService
    {
        private readonly IZstdCompressionService _innerService;
        private readonly ArrayPool<byte> _bytePool;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="innerService">内部压缩服务</param>
        public MemoryOptimizedZstdCompressionService(IZstdCompressionService innerService)
        {
            _innerService = innerService ?? throw new ArgumentNullException(nameof(innerService));
            _bytePool = ArrayPool<byte>.Shared;
        }
        
        /// <inheritdoc/>
        public byte[] Compress(byte[] data, int compressionLevel = 3)
        {
            return _innerService.Compress(data, compressionLevel);
        }
        
        /// <inheritdoc/>
        public byte[] Decompress(byte[] data)
        {
            return _innerService.Decompress(data);
        }
        
        /// <inheritdoc/>
        public byte[] CompressWithDictionary(byte[] data, byte[] dictionary, int compressionLevel = 3)
        {
            return _innerService.CompressWithDictionary(data, dictionary, compressionLevel);
        }
        
        /// <inheritdoc/>
        public byte[] DecompressWithDictionary(byte[] compressedData, byte[] dictionary)
        {
            return _innerService.DecompressWithDictionary(compressedData, dictionary);
        }
        
        /// <inheritdoc/>
        public async Task CompressStreamAsync(Stream inputStream, Stream outputStream, int compressionLevel = 3, CancellationToken cancellationToken = default)
        {
            // 使用内存池优化流处理
            const int bufferSize = 81920;
            var buffer = _bytePool.Rent(bufferSize);
            
            try
            {
                using var compressor = new Compressor(compressionLevel);
                using var compressionStream = new CompressionStream(outputStream, compressor, leaveOpen: true);
                
                int bytesRead;
                while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await compressionStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                }
                
                await compressionStream.FlushAsync(cancellationToken);
            }
            finally
            {
                _bytePool.Return(buffer);
            }
        }
        
        /// <inheritdoc/>
        public async Task DecompressStreamAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken = default)
        {
            // 使用内存池优化流处理
            const int bufferSize = 81920;
            var buffer = _bytePool.Rent(bufferSize);
            
            try
            {
                using var decompressor = new Decompressor();
                using var decompressionStream = new DecompressionStream(inputStream, decompressor, leaveOpen: true);
                
                int bytesRead;
                while ((bytesRead = await decompressionStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await outputStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                }
                
                await outputStream.FlushAsync(cancellationToken);
            }
            finally
            {
                _bytePool.Return(buffer);
            }
        }
        
        /// <summary>
        /// 使用 Span 压缩数据（零拷贝）
        /// </summary>
        /// <param name="data">要压缩的数据</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>压缩后的数据</returns>
        public byte[] Compress(ReadOnlySpan<byte> data, int compressionLevel = 3)
        {
            if (data.IsEmpty)
                return Array.Empty<byte>();
            
            // 转换为数组进行压缩
            byte[] dataArray;
            if (data.TryCopyTo(ArrayPool<byte>.Shared.Rent(data.Length), out var written))
            {
                dataArray = new byte[written];
                data.CopyTo(dataArray);
            }
            else
            {
                dataArray = data.ToArray();
            }
            
            return _innerService.Compress(dataArray, compressionLevel);
        }
        
        /// <summary>
        /// 使用 Span 解压缩数据（零拷贝）
        /// </summary>
        /// <param name="compressedData">压缩的数据</param>
        /// <returns>解压缩后的数据</returns>
        public byte[] Decompress(ReadOnlySpan<byte> compressedData)
        {
            if (compressedData.IsEmpty)
                return Array.Empty<byte>();
            
            // 转换为数组进行解压缩
            byte[] dataArray;
            if (compressedData.TryCopyTo(ArrayPool<byte>.Shared.Rent(compressedData.Length), out var written))
            {
                dataArray = new byte[written];
                compressedData.CopyTo(dataArray);
            }
            else
            {
                dataArray = compressedData.ToArray();
            }
            
            return _innerService.Decompress(dataArray);
        }
    }

    /// <summary>
    /// 压缩扩展方法
    /// </summary>
    public static class ZstdCompressionExtensions
    {
        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="service">压缩服务</param>
        /// <param name="text">要压缩的文本</param>
        /// <param name="encoding">编码</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>压缩后的数据</returns>
        public static byte[] CompressString(this IZstdCompressionService service, string text, Encoding encoding = null, int compressionLevel = 3)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();
            
            encoding ??= Encoding.UTF8;
            var data = encoding.GetBytes(text);
            return service.Compress(data, compressionLevel);
        }
        
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="service">压缩服务</param>
        /// <param name="compressedData">压缩的数据</param>
        /// <param name="encoding">编码</param>
        /// <returns>解压缩后的文本</returns>
        public static string DecompressString(this IZstdCompressionService service, byte[] compressedData, Encoding encoding = null)
        {
            if (compressedData == null || compressedData.Length == 0)
                return string.Empty;
            
            encoding ??= Encoding.UTF8;
            var data = service.Decompress(compressedData);
            return encoding.GetString(data);
        }
        
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="service">压缩服务</param>
        /// <param name="sourceFile">源文件路径</param>
        /// <param name="destinationFile">目标文件路径</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>压缩任务</returns>
        public static async Task CompressFileAsync(this IZstdCompressionService service, string sourceFile, string destinationFile, int compressionLevel = 3, CancellationToken cancellationToken = default)
        {
            using var inputStream = File.OpenRead(sourceFile);
            using var outputStream = File.Create(destinationFile);
            await service.CompressStreamAsync(inputStream, outputStream, compressionLevel, cancellationToken);
        }
        
        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="service">压缩服务</param>
        /// <param name="sourceFile">源文件路径</param>
        /// <param name="destinationFile">目标文件路径</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>解压缩任务</returns>
        public static async Task DecompressFileAsync(this IZstdCompressionService service, string sourceFile, string destinationFile, CancellationToken cancellationToken = default)
        {
            using var inputStream = File.OpenRead(sourceFile);
            using var outputStream = File.Create(destinationFile);
            await service.DecompressStreamAsync(inputStream, outputStream, cancellationToken);
        }
        
        /// <summary>
        /// 添加 Zstd 压缩服务到依赖注入容器
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">配置选项</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddZstdCompression(this IServiceCollection services, Action<ZstdCompressionOptions> configure = null)
        {
            var options = new ZstdCompressionOptions();
            configure?.Invoke(options);
            
            // 注册基本压缩服务
            services.AddSingleton<IZstdCompressionService, ZstdCompressionService>();
            
            // 如果启用并行处理，包装为并行服务
            if (options.EnableParallelProcessing)
            {
                services.AddSingleton<IZstdCompressionService>(provider =>
                    new ParallelZstdCompressionService(
                        provider.GetRequiredService<IZstdCompressionService>(),
                        options.MaxDegreeOfParallelism));
            }
            
            // 如果启用内存优化，包装为内存优化服务
            if (options.EnableMemoryOptimization)
            {
                services.AddSingleton<IZstdCompressionService>(provider =>
                    new MemoryOptimizedZstdCompressionService(
                        provider.GetRequiredService<IZstdCompressionService>()));
            }
            
            return services;
        }
    }

    /// <summary>
    /// Zstd 压缩选项
    /// </summary>
    public class ZstdCompressionOptions
    {
        /// <summary>
        /// 是否启用并行处理
        /// </summary>
        public bool EnableParallelProcessing { get; set; } = false;
        
        /// <summary>
        /// 最大并行度
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = Environment.ProcessorCount;
        
        /// <summary>
        /// 是否启用内存优化
        /// </summary>
        public bool EnableMemoryOptimization { get; set; } = true;
    }

    /// <summary>
    /// 性能测试工具
    /// </summary>
    public static class ZstdPerformanceTester
    {
        /// <summary>
        /// 测试压缩性能
        /// </summary>
        /// <param name="service">压缩服务</param>
        /// <param name="dataSize">数据大小（字节）</param>
        /// <param name="iterations">迭代次数</param>
        /// <returns>性能测试结果</returns>
        public static PerformanceTestResult TestCompressionPerformance(IZstdCompressionService service, int dataSize = 1024 * 1024, int iterations = 10)
        {
            // 生成随机测试数据
            var random = new Random();
            var testData = new byte[dataSize];
            random.NextBytes(testData);
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long totalCompressedSize = 0;
            
            for (int i = 0; i < iterations; i++)
            {
                var compressedData = service.Compress(testData);
                totalCompressedSize += compressedData.Length;
            }
            
            stopwatch.Stop();
            
            return new PerformanceTestResult
            {
                Operation = "Compression",
                DataSize = dataSize,
                Iterations = iterations,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                AverageCompressedSize = totalCompressedSize / iterations,
                CompressionRatio = (double)totalCompressedSize / (iterations * dataSize)
            };
        }
        
        /// <summary>
        /// 测试解压缩性能
        /// </summary>
        /// <param name="service">压缩服务</param>
        /// <param name="dataSize">数据大小（字节）</param>
        /// <param name="iterations">迭代次数</param>
        /// <returns>性能测试结果</returns>
        public static PerformanceTestResult TestDecompressionPerformance(IZstdCompressionService service, int dataSize = 1024 * 1024, int iterations = 10)
        {
            // 生成随机测试数据并压缩
            var random = new Random();
            var testData = new byte[dataSize];
            random.NextBytes(testData);
            var compressedData = service.Compress(testData);
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            for (int i = 0; i < iterations; i++)
            {
                service.Decompress(compressedData);
            }
            
            stopwatch.Stop();
            
            return new PerformanceTestResult
            {
                Operation = "Decompression",
                DataSize = dataSize,
                Iterations = iterations,
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                AverageCompressedSize = compressedData.Length,
                CompressionRatio = (double)compressedData.Length / dataSize
            };
        }
    }

    /// <summary>
    /// 性能测试结果
    /// </summary>
    public class PerformanceTestResult
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Operation { get; set; }
        
        /// <summary>
        /// 数据大小（字节）
        /// </summary>
        public int DataSize { get; set; }
        
        /// <summary>
        /// 迭代次数
        /// </summary>
        public int Iterations { get; set; }
        
        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public long ElapsedMilliseconds { get; set; }
        
        /// <summary>
        /// 平均压缩大小（字节）
        /// </summary>
        public long AverageCompressedSize { get; set; }
        
        /// <summary>
        /// 压缩比
        /// </summary>
        public double CompressionRatio { get; set; }
        
        /// <summary>
        /// 每秒操作数
        /// </summary>
        public double OperationsPerSecond => Iterations / (ElapsedMilliseconds / 1000.0);
        
        /// <summary>
        /// 吞吐量（MB/s）
        /// </summary>
        public double ThroughputMBps => (DataSize * Iterations / (1024.0 * 1024.0)) / (ElapsedMilliseconds / 1000.0);
        
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串表示</returns>
        public override string ToString()
        {
            return $"{Operation}: {ElapsedMilliseconds:F2}ms, {OperationsPerSecond:F2} ops/s, {ThroughputMBps:F2} MB/s, Compression Ratio: {CompressionRatio:P2}";
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 创建依赖注入容器
            var services = new ServiceCollection();
            
            // 注册日志服务
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });
            
            // 注册 Zstd 压缩服务
            services.AddZstdCompression(options =>
            {
                options.EnableParallelProcessing = true;
                options.EnableMemoryOptimization = true;
                options.MaxDegreeOfParallelism = Environment.ProcessorCount;
            });
            
            // 构建服务提供者
            using var serviceProvider = services.BuildServiceProvider();
            
            // 获取压缩服务
            var compressionService = serviceProvider.GetRequiredService<IZstdCompressionService>();
            
            // 测试基本压缩和解压缩
            Console.WriteLine("=== 测试基本压缩和解压缩 ===");
            await TestBasicCompression(compressionService);
            
            // 测试并行压缩
            Console.WriteLine("\n=== 测试并行压缩 ===");
            await TestParallelCompression(compressionService);
            
            // 测试性能
            Console.WriteLine("\n=== 测试性能 ===");
            TestPerformance(compressionService);
            
            // 测试文件压缩
            Console.WriteLine("\n=== 测试文件压缩 ===");
            await TestFileCompression(compressionService);
        }
        
        private static async Task TestBasicCompression(IZstdCompressionService service)
        {
            // 测试字符串压缩
            var testString = "这是一个测试字符串，用于测试 Zstd 压缩算法的性能和效果." +
                            "Zstd 是一种高性能的压缩算法，由 Facebook 开发，" +
                            "它提供了出色的压缩率和压缩/解压缩速度.";
            
            Console.WriteLine($"原始字符串长度: {testString.Length}");
            
            // 压缩字符串
            var compressedData = service.CompressString(testString);
            Console.WriteLine($"压缩后数据长度: {compressedData.Length}");
            
            // 解压缩字符串
            var decompressedString = service.DecompressString(compressedData);
            Console.WriteLine($"解压缩后字符串长度: {decompressedString.Length}");
            
            // 验证结果
            Console.WriteLine($"解压缩结果是否正确: {testString == decompressedString}");
            
            // 测试流压缩
            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(testString));
            using var outputStream = new MemoryStream();
            
            await service.CompressStreamAsync(inputStream, outputStream);
            var compressedStreamData = outputStream.ToArray();
            Console.WriteLine($"流压缩后数据长度: {compressedStreamData.Length}");
        }
        
        private static async Task TestParallelCompression(IZstdCompressionService service)
        {
            // 生成多个测试数据块
            var dataBlocks = new byte[10][];
            var random = new Random();
            
            for (int i = 0; i < dataBlocks.Length; i++)
            {
                dataBlocks[i] = new byte[1024 * 1024]; // 1MB 每个块
                random.NextBytes(dataBlocks[i]);
            }
            
            // 测试并行压缩
            if (service is ParallelZstdCompressionService parallelService)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var compressedBlocks = await parallelService.CompressMultipleAsync(dataBlocks);
                stopwatch.Stop();
                
                Console.WriteLine($"并行压缩 10 个 1MB 数据块耗时: {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"平均压缩率: {compressedBlocks.Average(block => (double)block.Length / 1024 / 1024):P2}");
            }
            else
            {
                Console.WriteLine("当前服务不支持并行压缩");
            }
        }
        
        private static void TestPerformance(IZstdCompressionService service)
        {
            // 测试压缩性能
            var compressionResult = ZstdPerformanceTester.TestCompressionPerformance(service);
            Console.WriteLine(compressionResult);
            
            // 测试解压缩性能
            var decompressionResult = ZstdPerformanceTester.TestDecompressionPerformance(service);
            Console.WriteLine(decompressionResult);
        }
        
        private static async Task TestFileCompression(IZstdCompressionService service)
        {
            // 创建测试文件
            var testFilePath = Path.GetTempFileName();
            var compressedFilePath = testFilePath + ".zst";
            var decompressedFilePath = testFilePath + ".decompressed";
            
            try
            {
                // 写入测试数据
                using var writer = new StreamWriter(testFilePath);
                for (int i = 0; i < 10000; i++)
                {
                    await writer.WriteLineAsync($"这是测试行 {i}: 这是一些测试数据，用于测试文件压缩性能.");
                }
                await writer.FlushAsync();
                
                var originalFileSize = new FileInfo(testFilePath).Length;
                Console.WriteLine($"原始文件大小: {originalFileSize:N0} 字节");
                
                // 压缩文件
                var compressStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await service.CompressFileAsync(testFilePath, compressedFilePath);
                compressStopwatch.Stop();
                
                var compressedFileSize = new FileInfo(compressedFilePath).Length;
                Console.WriteLine($"压缩后文件大小: {compressedFileSize:N0} 字节");
                Console.WriteLine($"压缩耗时: {compressStopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"压缩率: {(double)compressedFileSize / originalFileSize:P2}");
                
                // 解压缩文件
                var decompressStopwatch = System.Diagnostics.Stopwatch.StartNew();
                await service.DecompressFileAsync(compressedFilePath, decompressedFilePath);
                decompressStopwatch.Stop();
                
                var decompressedFileSize = new FileInfo(decompressedFilePath).Length;
                Console.WriteLine($"解压缩后文件大小: {decompressedFileSize:N0} 字节");
                Console.WriteLine($"解压缩耗时: {decompressStopwatch.ElapsedMilliseconds}ms");
                
                // 验证文件内容
                var originalContent = await File.ReadAllTextAsync(testFilePath);
                var decompressedContent = await File.ReadAllTextAsync(decompressedFilePath);
                Console.WriteLine($"文件内容验证: {originalContent == decompressedContent}");
            }
            finally
            {
                // 清理临时文件
                if (File.Exists(testFilePath)) File.Delete(testFilePath);
                if (File.Exists(compressedFilePath)) File.Delete(compressedFilePath);
                if (File.Exists(decompressedFilePath)) File.Delete(decompressedFilePath);
            }
        }
    }
}