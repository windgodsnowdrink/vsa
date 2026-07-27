#:sdk Microsoft.NET.Sdk
#:package SkaiaSharp@1.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using SkaiaSharp;

public interface ISkaiaImageProcessingService
{
    ValueTask<byte[]> ResizeImageAsync(byte[] imageData, int width, int height);
    ValueTask<byte[]> ApplyFilterAsync(byte[] imageData, FilterType filterType);
    ValueTask<byte[]> ConvertFormatAsync(byte[] imageData, ImageFormat format);
}

public class SkaiaImageProcessingService : ISkaiaImageProcessingService
{
    private readonly ObjectPool<SkaiaImage> _imagePool;
    private readonly Channel<ImageProcessingContext> _processingChannel;
    
    public SkaiaImageProcessingService(ObjectPool<SkaiaImage> imagePool)
    {
        _imagePool = imagePool;
        _processingChannel = Channel.CreateBounded<ImageProcessingContext>(1000);
        StartProcessingWorkers(4);
    }
    
    private void StartProcessingWorkers(int workerCount)
    {
        for (int i = 0; i < workerCount; i++)
        {
            _ = Task.Run(async () =>
            {
                await foreach (var context in _processingChannel.Reader.ReadAllAsync())
                {
                    using var image = _imagePool.Get();
                    // 处理逻辑
                }
            });
        }
    }
    
    public async ValueTask<byte[]> ResizeImageAsync(byte[] imageData, int width, int height)
    {
        using var image = _imagePool.Get();
        using var span = image.Item.Load(imageData.AsSpan());
        image.Item.Resize(width, height);
        return image.Item.SaveToBuffer();
    }
    
    public async ValueTask<byte[]> ApplyFilterAsync(byte[] imageData, FilterType filterType)
    {
        using var image = _imagePool.Get();
        using var span = image.Item.Load(imageData.AsSpan());
        
        switch (filterType)
        {
            case FilterType.GaussianBlur:
                image.Item.ApplyGaussianBlur(5);
                break;
            case FilterType.Sharpen:
                image.Item.ApplySharpen();
                break;
        }
        
        return image.Item.SaveToBuffer();
    }
    
    public async ValueTask<byte[]> ConvertFormatAsync(byte[] imageData, ImageFormat format)
    {
        using var image = _imagePool.Get();
        using var span = image.Item.Load(imageData.AsSpan());
        return image.Item.SaveToBuffer(format.ToSkaiaFormat());
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSkaiaImageProcessing(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<SkaiaImage>>(sp =>
        {
            var policy = new SkaiaImagePooledObjectPolicy();
            return new DefaultObjectPool<SkaiaImage>(policy, 10);
        });
        
        services.AddSingleton<ISkaiaImageProcessingService, SkaiaImageProcessingService>();
        return services;
    }
}

public enum FilterType { GaussianBlur, Sharpen }
public enum ImageFormat { Jpg, Png, Bmp }