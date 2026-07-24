#:sdk Microsoft.NET.Sdk.Web
#:package ImageProcessor@2.9.1
#:package ImageProcessor.Plugins.WebP@1.2.0
#:property LangVersion=preview
#:property TargetFramework=net11.0

using System;
using System.IO;
using System.Threading.Tasks;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Plugins.WebP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

public interface IImageProcessorService
{
    Task<byte[]> ResizeAsync(byte[] image, int width, int height);
    Task<byte[]> ConvertFormatAsync(byte[] image, string format);
    Task<byte[]> ApplyFilterAsync(byte[] image, string filterType);
}

public class ImageProcessorService : IImageProcessorService
{
    private readonly ObjectPool<ImageFactory> _imageFactoryPool;
    
    public ImageProcessorService(ObjectPool<ImageFactory> imageFactoryPool)
    {
        _imageFactoryPool = imageFactoryPool;
    }

    public async Task<byte[]> ResizeAsync(byte[] image, int width, int height)
    {
        var factory = _imageFactoryPool.Get();
        try
        {
            using (var inStream = new MemoryStream(image))
            using (var outStream = new MemoryStream())
            {
                factory.Load(inStream)
                    .Resize(new ResizeLayer(new Size(width, height)))
                    .Save(outStream);
                return outStream.ToArray();
            }
        }
        finally
        {
            _imageFactoryPool.Return(factory);
        }
    }

    public async Task<byte[]> ConvertFormatAsync(byte[] image, string format)
    {
        var factory = _imageFactoryPool.Get();
        try
        {
            using (var inStream = new MemoryStream(image))
            using (var outStream = new MemoryStream())
            {
                factory.Load(inStream);
                
                switch (format.ToLower())
                {
                    case "webp":
                        factory.Format(new WebPFormat());
                        break;
                    case "jpeg":
                    case "jpg":
                        factory.Format(new JpegFormat());
                        break;
                    case "png":
                        factory.Format(new PngFormat());
                        break;
                }
                
                factory.Save(outStream);
                return outStream.ToArray();
            }
        }
        finally
        {
            _imageFactoryPool.Return(factory);
        }
    }

    public async Task<byte[]> ApplyFilterAsync(byte[] image, string filterType)
    {
        var factory = _imageFactoryPool.Get();
        try
        {
            using (var inStream = new MemoryStream(image))
            using (var outStream = new MemoryStream())
            {
                factory.Load(inStream);
                
                switch (filterType.ToLower())
                {
                    case "gaussian":
                        factory.GaussianBlur(5);
                        break;
                    case "grayscale":
                        factory.Grayscale();
                        break;
                    case "sepia":
                        factory.Sepia();
                        break;
                }
                
                factory.Save(outStream);
                return outStream.ToArray();
            }
        }
        finally
        {
            _imageFactoryPool.Return(factory);
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageProcessorServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<ImageFactory>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<ImageFactory>();
            return new DefaultObjectPool<ImageFactory>(policy, Environment.ProcessorCount * 2);
        });
        
        services.AddSingleton<IImageProcessorService, ImageProcessorService>();
        return services;
    }
}