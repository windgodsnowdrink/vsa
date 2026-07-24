#:sdk Microsoft.NET.Sdk
#:package QRCoder@1.4.3
#:package ZXing.Net@0.16.8
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using QRCoder;
using ZXing;
using ZXing.Common;

namespace QrBarcodeIntegration
{
    public interface IQrBarcodeService
    {
        byte[] GenerateQrCode(string content, int pixelsPerModule = 20);
        string DecodeQrCode(byte[] imageData);
        byte[] GenerateBarcode(string content, BarcodeFormat format = BarcodeFormat.CODE_128, int width = 300, int height = 100);
        string DecodeBarcode(byte[] imageData);
    }

    public class QrBarcodeService : IQrBarcodeService
    {
        private readonly ObjectPool<QRCodeGenerator> _qrGeneratorPool;
        private readonly ObjectPool<BarcodeWriter<Bitmap>> _barcodeWriterPool;
        private readonly ObjectPool<BarcodeReader> _barcodeReaderPool;
        private readonly ThreadLocal<MemoryStream> _threadLocalStream;

        public QrBarcodeService(
            ObjectPool<QRCodeGenerator> qrGeneratorPool,
            ObjectPool<BarcodeWriter<Bitmap>> barcodeWriterPool,
            ObjectPool<BarcodeReader> barcodeReaderPool)
        {
            _qrGeneratorPool = qrGeneratorPool;
            _barcodeWriterPool = barcodeWriterPool;
            _barcodeReaderPool = barcodeReaderPool;
            _threadLocalStream = new ThreadLocal<MemoryStream>(() => new MemoryStream(4096));
        }

        public byte[] GenerateQrCode(string content, int pixelsPerModule = 20)
        {
            var qrGenerator = _qrGeneratorPool.Get();
            try
            {
                using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new BitmapByteQRCode(qrCodeData);
                return qrCode.GetGraphic(pixelsPerModule);
            }
            finally
            {
                _qrGeneratorPool.Return(qrGenerator);
            }
        }

        public string DecodeQrCode(byte[] imageData)
        {
            var reader = _barcodeReaderPool.Get();
            try
            {
                using var bitmap = new Bitmap(new MemoryStream(imageData));
                var result = reader.Decode(bitmap);
                return result?.Text ?? string.Empty;
            }
            finally
            {
                _barcodeReaderPool.Return(reader);
            }
        }

        public byte[] GenerateBarcode(string content, BarcodeFormat format = BarcodeFormat.CODE_128, int width = 300, int height = 100)
        {
            var writer = _barcodeWriterPool.Get();
            try
            {
                writer.Format = format;
                writer.Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 1,
                    PureBarcode = true
                };

                var bitmap = writer.Write(content);
                var stream = _threadLocalStream.Value;
                stream.SetLength(0);
                bitmap.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
            finally
            {
                _barcodeWriterPool.Return(writer);
            }
        }

        public string DecodeBarcode(byte[] imageData)
        {
            var reader = _barcodeReaderPool.Get();
            try
            {
                using var bitmap = new Bitmap(new MemoryStream(imageData));
                var result = reader.Decode(bitmap);
                return result?.Text ?? string.Empty;
            }
            finally
            {
                _barcodeReaderPool.Return(reader);
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQrBarcodeServices(this IServiceCollection services)
        {
            services.AddSingleton<ObjectPool<QRCodeGenerator>>(sp =>
            {
                var policy = new DefaultPooledObjectPolicy<QRCodeGenerator>();
                return new DefaultObjectPool<QRCodeGenerator>(policy, Environment.ProcessorCount * 2);
            });

            services.AddSingleton<ObjectPool<BarcodeWriter<Bitmap>>>(sp =>
            {
                var policy = new DefaultPooledObjectPolicy<BarcodeWriter<Bitmap>>();
                return new DefaultObjectPool<BarcodeWriter<Bitmap>>(policy, Environment.ProcessorCount * 2);
            });

            services.AddSingleton<ObjectPool<BarcodeReader>>(sp =>
            {
                var policy = new DefaultPooledObjectPolicy<BarcodeReader>();
                return new DefaultObjectPool<BarcodeReader>(policy, Environment.ProcessorCount * 2);
            });

            services.AddSingleton<IQrBarcodeService, QrBarcodeService>();
            return services;
        }
    }
}

// 在Program.cs中注册服务
builder.Services.AddQrBarcodeServices();

// 在控制器或服务中使用
var qrBarcodeService = serviceProvider.GetRequiredService<IQrBarcodeService>();

// 生成二维码
var qrCode = qrBarcodeService.GenerateQrCode("https://example.com");

// 生成条形码
var barcode = qrBarcodeService.GenerateBarcode("123456789");

// 解码二维码
var decodedText = qrBarcodeService.DecodeQrCode(qrCode);