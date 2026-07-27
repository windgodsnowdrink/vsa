#:sdk Microsoft.NET.Sdk.Web
#:package FaceDetectAndLandmarkForTorchSharp@1.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Threading.Tasks;
using FaceDetectAndLandmarkForTorchSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

public interface IFaceDetectionService
{
    Task<FaceDetectionResult> DetectFacesAsync(byte[] imageData);
}

public class FaceDetectionService : IFaceDetectionService
{
    private readonly ObjectPool<FaceDetector> _detectorPool;
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer;

    public FaceDetectionService(ObjectPool<FaceDetector> detectorPool)
    {
        _detectorPool = detectorPool;
        _threadLocalBuffer = new ThreadLocal<Span<byte>>(() => 
            new byte[1024 * 1024].AsSpan()); // 1MB thread-local buffer
    }

    public async Task<FaceDetectionResult> DetectFacesAsync(byte[] imageData)
    {
        var detector = _detectorPool.Get();
        try
        {
            using var span = _threadLocalBuffer.Value;
            // 使用Span<T>零拷贝技术处理图像数据
            imageData.AsSpan().CopyTo(span);
            
            // 执行人脸检测
            var result = await Task.Run(() => detector.Detect(span));
            return result;
        }
        finally
        {
            _detectorPool.Return(detector);
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaceDetectionServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<FaceDetector>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<FaceDetector>(() => 
                new FaceDetector("models/face_detection.pt"));
            return new DefaultObjectPool<FaceDetector>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
        return services;
    }
}

// 示例用法
var services = new ServiceCollection();
services.AddFaceDetectionServices();

var provider = services.BuildServiceProvider();
var faceService = provider.GetRequiredService<IFaceDetectionService>();

// 加载图像数据
var imageData = await File.ReadAllBytesAsync("test.jpg");
var result = await faceService.DetectFacesAsync(imageData);