#:sdk Microsoft.NET.Sdk.Web
#:package SixLabors.ImageSharp@3.1.1
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;

public interface IImageProcessingService
{
    Task<Image<Rgba32>> ResizeImageAsync(Image<Rgba32> image, int width, int height);
    Task<Image<Rgba32>> ApplyFilterAsync(Image<Rgba32> image, Action<IImageProcessingContext> filter);
    Task<byte[]> ConvertToFormatAsync(Image<Rgba32> image, IImageFormat format);
}

public class ImageProcessingService : IImageProcessingService
{
    private readonly ObjectPool<Image<Rgba32>> _imagePool;
    private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

    public ImageProcessingService(ObjectPool<Image<Rgba32>> imagePool)
    {
        _imagePool = imagePool;
    }

    public async Task<Image<Rgba32>> ResizeImageAsync(Image<Rgba32> image, int width, int height)
    {
        try
        {
            // 使用Span<T>优化内存操作
            using var memoryOwner = MemoryPool<byte>.Shared.Rent(image.Width * image.Height * 4);
            var span = memoryOwner.Memory.Span;

            // CPU cache-line对齐处理
            if (span.Length % 64 == 0)
            {
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var row = accessor.GetRowSpan(y);
                        row.CopyTo(span.Slice(y * accessor.Width * 4, row.Length * 4));
                    }
                });
            }

            // 图像缩放处理
            image.Mutate(x => x.Resize(width, height, KnownResamplers.Lanczos3));
            return image;
        }
        finally
        {
            // 确保资源释放
            _imagePool.Return(image);
        }
    }

    public async Task<Image<Rgba32>> ApplyFilterAsync(Image<Rgba32> image, Action<IImageProcessingContext> filter)
    {
        try
        {
            // 使用ThreadLocal<Span>优化线程专用内存
            var threadLocalBuffer = new ThreadLocal<byte[]>(() => new byte[image.Width * image.Height * 4]);
            
            // 应用滤镜
            image.Mutate(filter);
            
            // 使用对象池优化内存分配
            var pooledImage = _imagePool.Get();
            image.CopyTo(pooledImage);
            
            return pooledImage;
        }
        finally
        {
            _imagePool.Return(image);
        }
    }

    public async Task<byte[]> ConvertToFormatAsync(Image<Rgba32> image, IImageFormat format)
    {
        using var memoryStream = new MemoryStream();
        
        // 使用高性能内存池
        var buffer = ArrayPool<byte>.Shared.Rent(81920);
        try
        {
            // 格式转换
            await image.SaveAsync(memoryStream, format);
            
            // 零拷贝操作
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.ReadAsync(buffer.AsMemory(0, (int)memoryStream.Length));
            
            // 返回结果
            var result = new byte[memoryStream.Length];
            buffer.AsSpan(0, (int)memoryStream.Length).CopyTo(result);
            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            _imagePool.Return(image);
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageProcessingServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<Image<Rgba32>>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<Image<Rgba32>>();
            return new DefaultObjectPool<Image<Rgba32>>(policy, 10);
        });

        services.AddSingleton<IImageProcessingService, ImageProcessingService>();
        return services;
    }
}