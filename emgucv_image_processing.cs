#:sdk Microsoft.NET.Sdk.Web
#:package Emgu.CV@4.8.0
#:package Emgu.CV.runtime.windows@4.8.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;
using System.Runtime.InteropServices;

public interface IEmguCvImageProcessingService
{
    Task<Mat> ResizeImageAsync(Mat image, int width, int height);
    Task<Mat> ApplyFilterAsync(Mat image, Action<IInputOutputArray> filter);
    Task<byte[]> ConvertToFormatAsync(Mat image, string format);
}

public class EmguCvImageProcessingService : IEmguCvImageProcessingService
{
    private readonly ObjectPool<Mat> _matPool;
    private readonly ArrayPool<byte> _arrayPool = ArrayPool<byte>.Shared;

    public EmguCvImageProcessingService(ObjectPool<Mat> matPool)
    {
        _matPool = matPool;
    }

    public async Task<Mat> ResizeImageAsync(Mat image, int width, int height)
    {
        
    }

    public Mat ResizeImage(Mat source, Size newSize, Inter interpolation = Inter.Linear)
    {
        using var pool = _matPool.Get();
        var result = pool.Item;
        CvInvoke.Resize(source, result, newSize, 0, 0, interpolation);
        return result.Clone();
    }

    public async Task<Mat> ApplyFilterAsync(Mat image, Action<IInputOutputArray> filter)
    {
        
    }

    public Mat ApplyFilter(Mat source, FilterType filterType, double param = 0)
    {
        using var pool = _matPool.Get();
        var result = pool.Item;
        
        switch (filterType)
        {
            case FilterType.GaussianBlur:
                CvInvoke.GaussianBlur(source, result, new Size(5, 5), param);
                break;
            case FilterType.MedianBlur:
                CvInvoke.MedianBlur(source, result, (int)param);
                break;
            case FilterType.BilateralFilter:
                CvInvoke.BilateralFilter(source, result, (int)param, param * 2, param / 2);
                break;
        }
        
        return result.Clone();
    }

    public async Task<byte[]> ConvertToFormatAsync(Mat image, string format)
    {
        
    }

    public byte[] ConvertFormat(Mat source, ImageFormat format, int quality = 95)
    {
        using var ms = new MemoryStream();
        var parameters = new List<KeyValuePair<ImwriteFlags, int>>
        {
            new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, quality)
        };
        
        CvInvoke.Imencode($".{format.ToString().ToLower()}", source, ms, parameters.ToArray());
        return ms.ToArray();
    }

    public enum FilterType { GaussianBlur, MedianBlur, BilateralFilter }
    public enum ImageFormat { Jpg, Png, Bmp }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmguCvImageProcessingServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<Mat>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<Mat>();
            return new DefaultObjectPool<Mat>(policy, 10);
        });

        services.AddSingleton<IEmguCvImageProcessingService, EmguCvImageProcessingService>();
        return services;
    }
}