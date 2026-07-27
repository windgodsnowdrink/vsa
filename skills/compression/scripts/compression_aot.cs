#:sdk Microsoft.NET.Sdk.Web
#:package System.IO.Compression@10.0.0
#:package System.IO.Compression.ZipFile@10.0.0
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Hosting@10.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property InvariantGlobalization=true
#:property EnableCompilationRelaxations=true
#:property PublishReadyToRun=true

using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Compression.AOT
{
    /// <summary>
    /// 压缩算法类型枚举
    /// </summary>
    public enum CompressionAlgorithm
    {
        /// <summary>
        /// Gzip压缩算法
        /// </summary>
        Gzip,
        /// <summary>
        /// Deflate压缩算法
        /// </summary>
        Deflate,
        /// <summary>
        /// Brotli压缩算法
        /// </summary>
        Brotli,
        /// <summary>
        /// Zip压缩格式
        /// </summary>
        Zip
    }

    /// <summary>
    /// 压缩服务接口
    /// 定义了压缩和解压缩的核心功能
    /// </summary>
    public interface ICompressionService
    {
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="inputFile">输入文件路径</param>
        /// <param name="outputFile">输出文件路径</param>
        /// <param name="algorithm">压缩算法</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>压缩结果</returns>
        Task<bool> CompressFileAsync(string inputFile, string outputFile, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip, CompressionLevel compressionLevel = CompressionLevel.Optimal);

        /// <summary>
        /// 解压缩文件
        /// </summary>
        /// <param name="inputFile">输入文件路径</param>
        /// <param name="outputFile">输出文件路径</param>
        /// <param name="algorithm">压缩算法</param>
        /// <returns>解压缩结果</returns>
        Task<bool> DecompressFileAsync(string inputFile, string outputFile, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip);

        /// <summary>
        /// 压缩数据流
        /// </summary>
        /// <param name="inputStream">输入数据流</param>
        /// <param name="outputStream">输出数据流</param>
        /// <param name="algorithm">压缩算法</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>压缩结果</returns>
        Task<bool> CompressStreamAsync(Stream inputStream, Stream outputStream, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip, CompressionLevel compressionLevel = CompressionLevel.Optimal);

        /// <summary>
        /// 解压缩数据流
        /// </summary>
        /// <param name="inputStream">输入数据流</param>
        /// <param name="outputStream">输出数据流</param>
        /// <param name="algorithm">压缩算法</param>
        /// <returns>解压缩结果</returns>
        Task<bool> DecompressStreamAsync(Stream inputStream, Stream outputStream, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip);
    }

    /// <summary>
    /// 压缩服务实现
    /// 基于.NET 10 AOT架构，提供高性能压缩功能
    /// </summary>
    public class CompressionService : ICompressionService
    {
        private readonly ILogger<CompressionService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        public CompressionService(ILogger<CompressionService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        public async Task<bool> CompressFileAsync(string inputFile, string outputFile, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            try
            {
                if (!File.Exists(inputFile))
                {
                    _logger.LogError($"输入文件不存在: {inputFile}");
                    return false;
                }

                var fileInfo = new FileInfo(inputFile);
                _logger.LogInformation($"开始压缩文件: {inputFile}, 大小: {fileInfo.Length:N0} 字节");

                using var inputStream = File.OpenRead(inputFile);
                using var outputStream = File.Create(outputFile);

                var result = await CompressStreamAsync(inputStream, outputStream, algorithm, compressionLevel);

                if (result)
                {
                    var outputFileInfo = new FileInfo(outputFile);
                    var compressionRatio = (double)outputFileInfo.Length / fileInfo.Length;
                    _logger.LogInformation($"压缩完成: {outputFile}, 大小: {outputFileInfo.Length:N0} 字节, 压缩率: {compressionRatio:P2}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "压缩文件时发生错误");
                return false;
            }
        }

        /// <summary>
        /// 解压缩文件
        /// </summary>
        public async Task<bool> DecompressFileAsync(string inputFile, string outputFile, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip)
        {
            try
            {
                if (!File.Exists(inputFile))
                {
                    _logger.LogError($"输入文件不存在: {inputFile}");
                    return false;
                }

                var fileInfo = new FileInfo(inputFile);
                _logger.LogInformation($"开始解压缩文件: {inputFile}, 大小: {fileInfo.Length:N0} 字节");

                using var inputStream = File.OpenRead(inputFile);
                using var outputStream = File.Create(outputFile);

                var result = await DecompressStreamAsync(inputStream, outputStream, algorithm);

                if (result)
                {
                    var outputFileInfo = new FileInfo(outputFile);
                    _logger.LogInformation($"解压缩完成: {outputFile}, 大小: {outputFileInfo.Length:N0} 字节");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解压缩文件时发生错误");
                return false;
            }
        }

        /// <summary>
        /// 压缩数据流
        /// </summary>
        public async Task<bool> CompressStreamAsync(Stream inputStream, Stream outputStream, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            try
            {
                switch (algorithm)
                {
                    case CompressionAlgorithm.Gzip:
                        using (var gzipStream = new GZipStream(outputStream, compressionLevel, leaveOpen: true))
                        {
                            await inputStream.CopyToAsync(gzipStream);
                        }
                        break;
                    case CompressionAlgorithm.Deflate:
                        using (var deflateStream = new DeflateStream(outputStream, compressionLevel, leaveOpen: true))
                        {
                            await inputStream.CopyToAsync(deflateStream);
                        }
                        break;
                    case CompressionAlgorithm.Brotli:
                        using (var brotliStream = new BrotliStream(outputStream, compressionLevel, leaveOpen: true))
                        {
                            await inputStream.CopyToAsync(brotliStream);
                        }
                        break;
                    case CompressionAlgorithm.Zip:
                        // Zip压缩需要特殊处理
                        using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
                        {
                            var entry = archive.CreateEntry("compressed.bin", compressionLevel);
                            using var entryStream = entry.Open();
                            await inputStream.CopyToAsync(entryStream);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"不支持的压缩算法: {algorithm}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "压缩数据流时发生错误");
                return false;
            }
        }

        /// <summary>
        /// 解压缩数据流
        /// </summary>
        public async Task<bool> DecompressStreamAsync(Stream inputStream, Stream outputStream, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip)
        {
            try
            {
                switch (algorithm)
                {
                    case CompressionAlgorithm.Gzip:
                        using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress, leaveOpen: true))
                        {
                            await gzipStream.CopyToAsync(outputStream);
                        }
                        break;
                    case CompressionAlgorithm.Deflate:
                        using (var deflateStream = new DeflateStream(inputStream, CompressionMode.Decompress, leaveOpen: true))
                        {
                            await deflateStream.CopyToAsync(outputStream);
                        }
                        break;
                    case CompressionAlgorithm.Brotli:
                        using (var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress, leaveOpen: true))
                        {
                            await brotliStream.CopyToAsync(outputStream);
                        }
                        break;
                    case CompressionAlgorithm.Zip:
                        // Zip解压缩需要特殊处理
                        using (var archive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true))
                        {
                            var entry = archive.Entries[0];
                            using var entryStream = entry.Open();
                            await entryStream.CopyToAsync(outputStream);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"不支持的压缩算法: {algorithm}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解压缩数据流时发生错误");
                return false;
            }
        }
    }

    /// <summary>
    /// 压缩AOT执行引擎
    /// 管理压缩任务的执行
    /// </summary>
    public class CompressionAotEngine
    {
        private readonly ILogger<CompressionAotEngine> _logger;
        private readonly ICompressionService _compressionService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="compressionService">压缩服务</param>
        public CompressionAotEngine(ILogger<CompressionAotEngine> logger, ICompressionService compressionService)
        {
            _logger = logger;
            _compressionService = compressionService;
        }

        /// <summary>
        /// 执行压缩任务
        /// </summary>
        /// <param name="inputFile">输入文件</param>
        /// <param name="outputFile">输出文件</param>
        /// <param name="algorithm">压缩算法</param>
        /// <param name="compressionLevel">压缩级别</param>
        /// <returns>执行结果</returns>
        public async Task<bool> ExecuteCompressAsync(string inputFile, string outputFile, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            return await _compressionService.CompressFileAsync(inputFile, outputFile, algorithm, compressionLevel);
        }

        /// <summary>
        /// 执行解压缩任务
        /// </summary>
        /// <param name="inputFile">输入文件</param>
        /// <param name="outputFile">输出文件</param>
        /// <param name="algorithm">压缩算法</param>
        /// <returns>执行结果</returns>
        public async Task<bool> ExecuteDecompressAsync(string inputFile, string outputFile, CompressionAlgorithm algorithm = CompressionAlgorithm.Gzip)
        {
            return await _compressionService.DecompressFileAsync(inputFile, outputFile, algorithm);
        }
    }

    /// <summary>
    /// 主程序
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 主入口点
        /// </summary>
        /// <param name="args">命令行参数</param>
        /// <returns>退出代码</returns>
        public static async Task<int> Main(string[] args)
        {
            // 构建主机
            var builder = Host.CreateApplicationBuilder(args);

            // 配置日志
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // 注册服务
            builder.Services.AddSingleton<ICompressionService, CompressionService>();
            builder.Services.AddSingleton<CompressionAotEngine>();

            // 构建主机
            var host = builder.Build();
            var serviceProvider = host.Services;

            // 获取引擎实例
            var engine = serviceProvider.GetRequiredService<CompressionAotEngine>();

            // 解析命令行参数
            if (args.Length < 3)
            {
                Console.WriteLine("用法:");
                Console.WriteLine("  压缩: compression_aot.exe compress <inputfile> <outputfile> [algorithm] [compressionlevel]");
                Console.WriteLine("  解压缩: compression_aot.exe decompress <inputfile> <outputfile> [algorithm]");
                Console.WriteLine("\n算法选项:");
                Console.WriteLine("  gzip, deflate, brotli, zip");
                Console.WriteLine("\n压缩级别选项:");
                Console.WriteLine("  fastest, optimal, noCompression");
                return 1;
            }

            var command = args[0].ToLower();
            var inputFile = args[1];
            var outputFile = args[2];

            try
            {
                bool result;
                if (command == "compress")
                {
                    var algorithm = args.Length > 3 ? ParseCompressionAlgorithm(args[3]) : CompressionAlgorithm.Gzip;
                    var compressionLevel = args.Length > 4 ? ParseCompressionLevel(args[4]) : CompressionLevel.Optimal;
                    
                    result = await engine.ExecuteCompressAsync(inputFile, outputFile, algorithm, compressionLevel);
                }
                else if (command == "decompress")
                {
                    var algorithm = args.Length > 3 ? ParseCompressionAlgorithm(args[3]) : CompressionAlgorithm.Gzip;
                    
                    result = await engine.ExecuteDecompressAsync(inputFile, outputFile, algorithm);
                }
                else
                {
                    Console.WriteLine($"未知命令: {command}");
                    return 1;
                }

                return result ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"执行错误: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// 解析压缩算法
        /// </summary>
        /// <param name="algorithmStr">算法字符串</param>
        /// <returns>压缩算法枚举</returns>
        private static CompressionAlgorithm ParseCompressionAlgorithm(string algorithmStr)
        {
            return algorithmStr.ToLower() switch
            {
                "gzip" => CompressionAlgorithm.Gzip,
                "deflate" => CompressionAlgorithm.Deflate,
                "brotli" => CompressionAlgorithm.Brotli,
                "zip" => CompressionAlgorithm.Zip,
                _ => throw new ArgumentException($"不支持的压缩算法: {algorithmStr}")
            };
        }

        /// <summary>
        /// 解析压缩级别
        /// </summary>
        /// <param name="compressionLevelStr">压缩级别字符串</param>
        /// <returns>压缩级别枚举</returns>
        private static CompressionLevel ParseCompressionLevel(string compressionLevelStr)
        {
            return compressionLevelStr.ToLower() switch
            {
                "fastest" => CompressionLevel.Fastest,
                "optimal" => CompressionLevel.Optimal,
                "nocompression" => CompressionLevel.NoCompression,
                _ => throw new ArgumentException($"不支持的压缩级别: {compressionLevelStr}")
            };
        }
    }
}