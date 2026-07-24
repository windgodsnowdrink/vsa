#:sdk Microsoft.NET.Sdk.Web
#:package FaceRecognitionDotNet@1.3.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Threading.Tasks;
using FaceRecognitionDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

public interface IFaceRecognitionService
{
    Task<FaceLocation[]> DetectFacesAsync(byte[] imageData);
    Task<double> CompareFacesAsync(byte[] imageData1, byte[] imageData2);
}

public class FaceRecognitionService : IFaceRecognitionService
{
    private readonly ObjectPool<FaceRecognition> _recognitionPool;
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer;

    public FaceRecognitionService(ObjectPool<FaceRecognition> recognitionPool)
    {
        _recognitionPool = recognitionPool;
        _threadLocalBuffer = new ThreadLocal<Span<byte>>(() => 
            new byte[1024 * 1024 * 4].AsSpan()); // 4MB thread-local buffer
    }

    public async Task<FaceLocation[]> DetectFacesAsync(byte[] imageData)
    {
        var recognition = _recognitionPool.Get();
        try
        {
            using var span = _threadLocalBuffer.Value;
            // 使用Span<T>零拷贝技术处理图像数据
            imageData.AsSpan().CopyTo(span);
            
            // 执行人脸检测
            using var image = FaceRecognition.LoadImage(span.ToArray());
            return await Task.Run(() => recognition.FaceLocations(image));
        }
        finally
        {
            _recognitionPool.Return(recognition);
        }
    }

    public async Task<double> CompareFacesAsync(byte[] imageData1, byte[] imageData2)
    {
        var recognition = _recognitionPool.Get();
        try
        {
            using var span1 = _threadLocalBuffer.Value;
            using var span2 = _threadLocalBuffer.Value;
            
            // 使用Span<T>零拷贝技术处理图像数据
            imageData1.AsSpan().CopyTo(span1);
            imageData2.AsSpan().CopyTo(span2);
            
            // 执行人脸比对
            using var image1 = FaceRecognition.LoadImage(span1.ToArray());
            using var image2 = FaceRecognition.LoadImage(span2.ToArray());
            
            var faceLocations1 = await Task.Run(() => recognition.FaceLocations(image1));
            var faceLocations2 = await Task.Run(() => recognition.FaceLocations(image2));
            
            if (faceLocations1.Length == 0 || faceLocations2.Length == 0)
                return 0;
                
            using var encoding1 = recognition.FaceEncodings(image1, faceLocations1)[0];
            using var encoding2 = recognition.FaceEncodings(image2, faceLocations2)[0];
            
            return await Task.Run(() => FaceRecognition.FaceDistance(encoding1, encoding2));
        }
        finally
        {
            _recognitionPool.Return(recognition);
        }
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFaceRecognitionServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<FaceRecognition>>(sp =>
        {
            var policy = new DefaultPooledObjectPolicy<FaceRecognition>(() => 
                FaceRecognition.Create("models"));
            return new DefaultObjectPool<FaceRecognition>(policy, Environment.ProcessorCount * 2);
        });

        services.AddSingleton<IFaceRecognitionService, FaceRecognitionService>();
        return services;
    }
}

// 示例用法
var services = new ServiceCollection();
services.AddFaceRecognitionServices();

var provider = services.BuildServiceProvider();
var faceService = provider.GetRequiredService<IFaceRecognitionService>();

// 加载图像数据
var imageData1 = await File.ReadAllBytesAsync("test1.jpg");
var imageData2 = await File.ReadAllBytesAsync("test2.jpg");

// 人脸检测
var faces = await faceService.DetectFacesAsync(imageData1);

// 人脸比对
var similarity = await faceService.CompareFacesAsync(imageData1, imageData2);