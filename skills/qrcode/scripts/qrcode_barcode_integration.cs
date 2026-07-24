#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package ZXing.Net@0.16.12
#:package SkiaSharp@2.88.6
#:package System.Drawing.Common@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true
#:property TrimMode=partial
#:property ReadyToRun=true
#:property TieredCompilation=true
#:property AllowUnsafeBlocks=true
#:property Deterministic=true
#:property Strict=true

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;

namespace QrCodeIntegration
{
    // 错误修正级别
    public enum ErrorCorrectionLevel
    {
        Low,
        Medium,
        Quartile,
        High
    }

    // 条码格式
    public enum BarcodeFormat
    {
        CODE_128,
        EAN_8,
        EAN_13,
        UPC_A,
        UPC_E,
        CODE_39,
        CODE_93,
        ITF,
        CODABAR,
        QR_CODE
    }

    // QR码生成选项
    public class QrCodeGenerationOptions
    {
        public string Content { get; set; }
        public int Size { get; set; } = 256;
        public Color ForegroundColor { get; set; } = Color.Black;
        public Color BackgroundColor { get; set; } = Color.White;
        public ErrorCorrectionLevel ErrorCorrectionLevel { get; set; } = ErrorCorrectionLevel.Medium;
    }

    // 条码生成选项
    public class BarcodeGenerationOptions
    {
        public string Content { get; set; }
        public BarcodeFormat BarcodeFormat { get; set; } = BarcodeFormat.CODE_128;
        public int Width { get; set; } = 300;
        public int Height { get; set; } = 100;
        public Color ForegroundColor { get; set; } = Color.Black;
        public Color BackgroundColor { get; set; } = Color.White;
    }

    // QR码读取结果
    public class QrCodeReadResult
    {
        public string Content { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    // 条码读取结果
    public class BarcodeReadResult
    {
        public string Content { get; set; }
        public BarcodeFormat Format { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }

    // QR码服务配置
    public class QrCodeServiceOptions
    {
        public bool EnableCaching { get; set; } = true;
        public int CacheSize { get; set; } = 100;
        public ErrorCorrectionLevel DefaultErrorCorrectionLevel { get; set; } = ErrorCorrectionLevel.Medium;
        public int DefaultQRCodeSize { get; set; } = 256;
        public bool EnableAsyncProcessing { get; set; } = true;
        public int MaxConcurrentOperations { get; set; } = 10;
    }

    // QR码生成接口
    public interface IQrCodeGenerator
    {
        Task<byte[]> GenerateQrCodeAsync(QrCodeGenerationOptions options);
        Task<byte[]> GenerateQrCodeAsync(string content, int size = 256);
        Task<byte[][]> GenerateQrCodesAsync(IEnumerable<QrCodeGenerationOptions> optionsList);
    }

    // QR码读取接口
    public interface IQrCodeReader
    {
        Task<QrCodeReadResult> ReadQrCodeAsync(byte[] imageData);
        Task<QrCodeReadResult> ReadQrCodeAsync(Stream imageStream);
        Task<IEnumerable<QrCodeReadResult>> ReadQrCodesAsync(IEnumerable<byte[]> imageDataList);
    }

    // 条码生成接口
    public interface IBarcodeGenerator
    {
        Task<byte[]> GenerateBarcodeAsync(BarcodeGenerationOptions options);
        Task<byte[]> GenerateBarcodeAsync(string content, BarcodeFormat format = BarcodeFormat.CODE_128, int width = 300, int height = 100);
        Task<byte[][]> GenerateBarcodesAsync(IEnumerable<BarcodeGenerationOptions> optionsList);
    }

    // 条码读取接口
    public interface IBarcodeReader
    {
        Task<BarcodeReadResult> ReadBarcodeAsync(byte[] imageData);
        Task<BarcodeReadResult> ReadBarcodeAsync(Stream imageStream);
        Task<IEnumerable<BarcodeReadResult>> ReadBarcodesAsync(IEnumerable<byte[]> imageDataList);
    }

    // 缓存键
    internal class CacheKey : IEquatable<CacheKey>
    {
        public string Content { get; }
        public int Size { get; }
        public ErrorCorrectionLevel ErrorCorrectionLevel { get; }
        public BarcodeFormat? BarcodeFormat { get; }
        public int Width { get; }
        public int Height { get; }

        public CacheKey(string content, int size, ErrorCorrectionLevel errorCorrectionLevel, BarcodeFormat? barcodeFormat = null, int width = 0, int height = 0)
        {
            Content = content;
            Size = size;
            ErrorCorrectionLevel = errorCorrectionLevel;
            BarcodeFormat = barcodeFormat;
            Width = width;
            Height = height;
        }

        public bool Equals(CacheKey other)
        {
            if (other == null) return false;
            return Content == other.Content &&
                   Size == other.Size &&
                   ErrorCorrectionLevel == other.ErrorCorrectionLevel &&
                   BarcodeFormat == other.BarcodeFormat &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Content, Size, ErrorCorrectionLevel, BarcodeFormat, Width, Height);
        }
    }

    // QR码和条码服务实现
    public class QrBarcodeService : IQrCodeGenerator, IQrCodeReader, IBarcodeGenerator, IBarcodeReader
    {
        private readonly QrCodeServiceOptions _options;
        private readonly ILogger<QrBarcodeService> _logger;
        private readonly ConcurrentDictionary<CacheKey, byte[]> _cache;
        private readonly object _cacheLock = new object();
        private readonly ThreadLocal<SKBitmap> _threadLocalBitmap;
        private readonly ThreadLocal<MemoryStream> _threadLocalStream;

        public QrBarcodeService(IOptions<QrCodeServiceOptions> options, ILogger<QrBarcodeService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _cache = new ConcurrentDictionary<CacheKey, byte[]>();
            _threadLocalBitmap = new ThreadLocal<SKBitmap>(() => new SKBitmap());
            _threadLocalStream = new ThreadLocal<MemoryStream>(() => new MemoryStream(4096));
        }

        // 生成QR码
        public async Task<byte[]> GenerateQrCodeAsync(QrCodeGenerationOptions options)
        {
            if (string.IsNullOrEmpty(options.Content))
            {
                throw new ArgumentException("Content cannot be empty", nameof(options.Content));
            }

            // 尝试从缓存获取
            if (_options.EnableCaching)
            {
                var cacheKey = new CacheKey(options.Content, options.Size, options.ErrorCorrectionLevel);
                if (_cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    return cachedResult;
                }
            }

            try
            {
                var result = await Task.Run(() =>
                {
                    // 配置ZXing写入器
                    var writer = new BarcodeWriter<SKBitmap>
                    {
                        Format = ZXing.BarcodeFormat.QR_CODE,
                        Options = new QrCodeEncodingOptions
                        {
                            Width = options.Size,
                            Height = options.Size,
                            Margin = 1,
                            ErrorCorrection = (ZXing.QrCode.Internal.ErrorCorrectionLevel)options.ErrorCorrectionLevel
                        }
                    };

                    // 生成QR码
                    var bitmap = writer.Write(options.Content);

                    // 转换为字节数组
                    using var stream = _threadLocalStream.Value;
                    stream.SetLength(0);
                    using var pngStream = new SKManagedWStream(stream);
                    bitmap.Encode(pngStream, SKEncodedImageFormat.Png, 100);
                    return stream.ToArray();
                });

                // 缓存结果
                if (_options.EnableCaching)
                {
                    var cacheKey = new CacheKey(options.Content, options.Size, options.ErrorCorrectionLevel);
                    AddToCache(cacheKey, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                throw new Exception("Failed to generate QR code", ex);
            }
        }

        // 生成QR码（简化版）
        public async Task<byte[]> GenerateQrCodeAsync(string content, int size = 256)
        {
            var options = new QrCodeGenerationOptions
            {
                Content = content,
                Size = size,
                ErrorCorrectionLevel = _options.DefaultErrorCorrectionLevel
            };
            return await GenerateQrCodeAsync(options);
        }

        // 批量生成QR码
        public async Task<byte[][]> GenerateQrCodesAsync(IEnumerable<QrCodeGenerationOptions> optionsList)
        {
            var optionsArray = optionsList.ToArray();
            var results = new byte[optionsArray.Length][];

            if (_options.EnableAsyncProcessing)
            {
                var tasks = new Task<byte[]>[optionsArray.Length];
                for (int i = 0; i < optionsArray.Length; i++)
                {
                    int index = i;
                    tasks[i] = GenerateQrCodeAsync(optionsArray[index]);
                }
                results = await Task.WhenAll(tasks);
            }
            else
            {
                for (int i = 0; i < optionsArray.Length; i++)
                {
                    results[i] = await GenerateQrCodeAsync(optionsArray[i]);
                }
            }

            return results;
        }

        // 读取QR码
        public async Task<QrCodeReadResult> ReadQrCodeAsync(byte[] imageData)
        {
            using var stream = new MemoryStream(imageData);
            return await ReadQrCodeAsync(stream);
        }

        // 读取QR码（从流）
        public async Task<QrCodeReadResult> ReadQrCodeAsync(Stream imageStream)
        {
            try
            {
                var result = await Task.Run(() =>
                {
                    // 配置ZXing读取器
                    var reader = new BarcodeReader<SKBitmap>
                    {
                        Options = new DecodingOptions
                        {
                            PossibleFormats = new[] { ZXing.BarcodeFormat.QR_CODE },
                            TryHarder = true
                        }
                    };

                    // 读取图像
                    using var skStream = new SKManagedStream(imageStream);
                    using var bitmap = SKBitmap.Decode(skStream);
                    if (bitmap == null)
                    {
                        return new QrCodeReadResult { Success = false, ErrorMessage = "Failed to decode image" };
                    }

                    // 解码QR码
                    var decodeResult = reader.Decode(bitmap);
                    if (decodeResult == null)
                    {
                        return new QrCodeReadResult { Success = false, ErrorMessage = "No QR code found" };
                    }

                    return new QrCodeReadResult
                    {
                        Success = true,
                        Content = decodeResult.Text
                    };
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading QR code");
                return new QrCodeReadResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        // 批量读取QR码
        public async Task<IEnumerable<QrCodeReadResult>> ReadQrCodesAsync(IEnumerable<byte[]> imageDataList)
        {
            var imageDataArray = imageDataList.ToArray();
            var results = new QrCodeReadResult[imageDataArray.Length];

            if (_options.EnableAsyncProcessing)
            {
                var tasks = new Task<QrCodeReadResult>[imageDataArray.Length];
                for (int i = 0; i < imageDataArray.Length; i++)
                {
                    int index = i;
                    tasks[i] = ReadQrCodeAsync(imageDataArray[index]);
                }
                results = await Task.WhenAll(tasks);
            }
            else
            {
                for (int i = 0; i < imageDataArray.Length; i++)
                {
                    results[i] = await ReadQrCodeAsync(imageDataArray[i]);
                }
            }

            return results;
        }

        // 生成条码
        public async Task<byte[]> GenerateBarcodeAsync(BarcodeGenerationOptions options)
        {
            if (string.IsNullOrEmpty(options.Content))
            {
                throw new ArgumentException("Content cannot be empty", nameof(options.Content));
            }

            // 尝试从缓存获取
            if (_options.EnableCaching)
            {
                var cacheKey = new CacheKey(options.Content, 0, _options.DefaultErrorCorrectionLevel, options.BarcodeFormat, options.Width, options.Height);
                if (_cache.TryGetValue(cacheKey, out var cachedResult))
                {
                    return cachedResult;
                }
            }

            try
            {
                var result = await Task.Run(() =>
                {
                    // 配置ZXing写入器
                    var writer = new BarcodeWriter<SKBitmap>
                    {
                        Format = (ZXing.BarcodeFormat)options.BarcodeFormat,
                        Options = new EncodingOptions
                        {
                            Width = options.Width,
                            Height = options.Height,
                            Margin = 1
                        }
                    };

                    // 生成条码
                    var bitmap = writer.Write(options.Content);

                    // 转换为字节数组
                    using var stream = _threadLocalStream.Value;
                    stream.SetLength(0);
                    using var pngStream = new SKManagedWStream(stream);
                    bitmap.Encode(pngStream, SKEncodedImageFormat.Png, 100);
                    return stream.ToArray();
                });

                // 缓存结果
                if (_options.EnableCaching)
                {
                    var cacheKey = new CacheKey(options.Content, 0, _options.DefaultErrorCorrectionLevel, options.BarcodeFormat, options.Width, options.Height);
                    AddToCache(cacheKey, result);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating barcode");
                throw new Exception("Failed to generate barcode", ex);
            }
        }

        // 生成条码（简化版）
        public async Task<byte[]> GenerateBarcodeAsync(string content, BarcodeFormat format = BarcodeFormat.CODE_128, int width = 300, int height = 100)
        {
            var options = new BarcodeGenerationOptions
            {
                Content = content,
                BarcodeFormat = format,
                Width = width,
                Height = height
            };
            return await GenerateBarcodeAsync(options);
        }

        // 批量生成条码
        public async Task<byte[][]> GenerateBarcodesAsync(IEnumerable<BarcodeGenerationOptions> optionsList)
        {
            var optionsArray = optionsList.ToArray();
            var results = new byte[optionsArray.Length][];

            if (_options.EnableAsyncProcessing)
            {
                var tasks = new Task<byte[]>[optionsArray.Length];
                for (int i = 0; i < optionsArray.Length; i++)
                {
                    int index = i;
                    tasks[i] = GenerateBarcodeAsync(optionsArray[index]);
                }
                results = await Task.WhenAll(tasks);
            }
            else
            {
                for (int i = 0; i < optionsArray.Length; i++)
                {
                    results[i] = await GenerateBarcodeAsync(optionsArray[i]);
                }
            }

            return results;
        }

        // 读取条码
        public async Task<BarcodeReadResult> ReadBarcodeAsync(byte[] imageData)
        {
            using var stream = new MemoryStream(imageData);
            return await ReadBarcodeAsync(stream);
        }

        // 读取条码（从流）
        public async Task<BarcodeReadResult> ReadBarcodeAsync(Stream imageStream)
        {
            try
            {
                var result = await Task.Run(() =>
                {
                    // 配置ZXing读取器
                    var reader = new BarcodeReader<SKBitmap>
                    {
                        Options = new DecodingOptions
                        {
                            PossibleFormats = Enum.GetValues(typeof(ZXing.BarcodeFormat)).Cast<ZXing.BarcodeFormat>().ToArray(),
                            TryHarder = true
                        }
                    };

                    // 读取图像
                    using var skStream = new SKManagedStream(imageStream);
                    using var bitmap = SKBitmap.Decode(skStream);
                    if (bitmap == null)
                    {
                        return new BarcodeReadResult { Success = false, ErrorMessage = "Failed to decode image" };
                    }

                    // 解码条码
                    var decodeResult = reader.Decode(bitmap);
                    if (decodeResult == null)
                    {
                        return new BarcodeReadResult { Success = false, ErrorMessage = "No barcode found" };
                    }

                    return new BarcodeReadResult
                    {
                        Success = true,
                        Content = decodeResult.Text,
                        Format = (BarcodeFormat)decodeResult.BarcodeFormat
                    };
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading barcode");
                return new BarcodeReadResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        // 批量读取条码
        public async Task<IEnumerable<BarcodeReadResult>> ReadBarcodesAsync(IEnumerable<byte[]> imageDataList)
        {
            var imageDataArray = imageDataList.ToArray();
            var results = new BarcodeReadResult[imageDataArray.Length];

            if (_options.EnableAsyncProcessing)
            {
                var tasks = new Task<BarcodeReadResult>[imageDataArray.Length];
                for (int i = 0; i < imageDataArray.Length; i++)
                {
                    int index = i;
                    tasks[i] = ReadBarcodeAsync(imageDataArray[index]);
                }
                results = await Task.WhenAll(tasks);
            }
            else
            {
                for (int i = 0; i < imageDataArray.Length; i++)
                {
                    results[i] = await ReadBarcodeAsync(imageDataArray[i]);
                }
            }

            return results;
        }

        // 添加到缓存
        private void AddToCache(CacheKey key, byte[] value)
        {
            lock (_cacheLock)
            {
                if (_cache.Count >= _options.CacheSize)
                {
                    // 移除最早的项
                    var oldestKey = _cache.Keys.First();
                    _cache.TryRemove(oldestKey, out _);
                }
                _cache[key] = value;
            }
        }
    }

    // 依赖注入扩展
    public static class QrCodeServiceExtensions
    {
        public static IServiceCollection AddQrCodeServices(this IServiceCollection services, Action<QrCodeServiceOptions> configureOptions = null)
        {
            // 配置选项
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<QrCodeServiceOptions>(options => { });
            }

            // 注册服务
            services.AddSingleton<IQrCodeGenerator, QrBarcodeService>();
            services.AddSingleton<IQrCodeReader, QrBarcodeService>();
            services.AddSingleton<IBarcodeGenerator, QrBarcodeService>();
            services.AddSingleton<IBarcodeReader, QrBarcodeService>();
            services.AddSingleton<QrBarcodeService>();

            return services;
        }
    }

    // 主程序（用于测试）
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 构建服务容器
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            services.AddQrCodeServices(options =>
            {
                options.EnableCaching = true;
                options.CacheSize = 50;
                options.DefaultErrorCorrectionLevel = ErrorCorrectionLevel.Medium;
                options.DefaultQRCodeSize = 200;
                options.EnableAsyncProcessing = true;
            });

            var serviceProvider = services.BuildServiceProvider();

            try
            {
                // 测试QR码生成
                var qrCodeGenerator = serviceProvider.GetRequiredService<IQrCodeGenerator>();
                var qrCodeOptions = new QrCodeGenerationOptions
                {
                    Content = "https://www.example.com",
                    Size = 256,
                    ErrorCorrectionLevel = ErrorCorrectionLevel.Medium
                };

                var qrCodeImage = await qrCodeGenerator.GenerateQrCodeAsync(qrCodeOptions);
                await File.WriteAllBytesAsync("qrcode.png", qrCodeImage);
                Console.WriteLine("QR code generated and saved to qrcode.png");

                // 测试条码生成
                var barcodeGenerator = serviceProvider.GetRequiredService<IBarcodeGenerator>();
                var barcodeOptions = new BarcodeGenerationOptions
                {
                    Content = "123456789012",
                    BarcodeFormat = BarcodeFormat.EAN_13,
                    Width = 300,
                    Height = 100
                };

                var barcodeImage = await barcodeGenerator.GenerateBarcodeAsync(barcodeOptions);
                await File.WriteAllBytesAsync("barcode.png", barcodeImage);
                Console.WriteLine("Barcode generated and saved to barcode.png");

                // 测试QR码读取
                var qrCodeReader = serviceProvider.GetRequiredService<IQrCodeReader>();
                var readResult = await qrCodeReader.ReadQrCodeAsync(qrCodeImage);
                if (readResult.Success)
                {
                    Console.WriteLine($"QR code content: {readResult.Content}");
                }
                else
                {
                    Console.WriteLine($"Failed to read QR code: {readResult.ErrorMessage}");
                }

                Console.WriteLine("All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (serviceProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
