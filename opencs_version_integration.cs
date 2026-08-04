
#:sdk Microsoft.NET.Sdk.Web
#:package OpenCvSharp4@4.8.0
#:package OpenCvSharp4.runtime.win@4.8.0
#:package OpenCvSharp4.Extensions@4.8.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System;
using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using OpenCvSharp;

// 计算机视觉服务接口
public interface IComputerVisionService
{
    ValueTask<Mat> DetectFacesAsync(Mat input);
    ValueTask<Mat> TrackObjectsAsync(Mat input);
    ValueTask<Mat> ExtractFeaturesAsync(Mat input);
}

// 计算机视觉服务实现
public sealed class ComputerVisionService : IComputerVisionService, IDisposable
{
    private readonly CascadeClassifier _faceClassifier;
    private readonly ObjectPool<Mat> _matPool;
    private readonly ChannelWriter<Mat> _resultChannel;
    private readonly ThreadLocal<Span<byte>> _threadLocalBuffer;

    public ComputerVisionService(
        ObjectPool<Mat> matPool,
        Channel<Mat> resultChannel)
    {
        _matPool = matPool;
        _resultChannel = resultChannel.Writer;
        _faceClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        _threadLocalBuffer = new ThreadLocal<Span<byte>>(() => 
            new byte[4096].AsSpan());
    }

    public async ValueTask<Mat> DetectFacesAsync(Mat input)
    {
        using var gray = _matPool.Get();
        Cv2.CvtColor(input, gray, ColorConversionCodes.BGR2GRAY);
        
        var faces = _faceClassifier.DetectMultiScale(gray);
        var result = _matPool.Get();
        input.CopyTo(result);
        
        foreach (var rect in faces)
        {
            Cv2.Rectangle(result, rect, Scalar.Red, 2);
        }
        
        await _resultChannel.WriteAsync(result);
        return result;
    }

    // ... 其他方法实现 ...

    public void Dispose()
    {
        _faceClassifier.Dispose();
        _threadLocalBuffer.Dispose();
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddComputerVisionServices(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPool<Mat>>(sp => 
            new DefaultObjectPool<Mat>(new MatPooledObjectPolicy(), 10));
            
        services.AddSingleton<Channel<Mat>>(_ => 
            Channel.CreateBounded<Mat>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            }));
            
        services.AddSingleton<IComputerVisionService, ComputerVisionService>();
        return services;
    }
}

// Mat对象池策略
public class MatPooledObjectPolicy : PooledObjectPolicy<Mat>
{
    public override Mat Create() => new Mat();
    
    public override bool Return(Mat obj)
    {
        obj.Release();
        return true;
    }
}

// 示例使用
public static class Program
{
    public static async Task Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddComputerVisionServices()
            .BuildServiceProvider();
            
        var visionService = services.GetRequiredService<IComputerVisionService>();
        
        using var image = Cv2.ImRead("test.jpg");
        var result = await visionService.DetectFacesAsync(image);
        
        Cv2.ImShow("Result", result);
        Cv2.WaitKey(0);
    }
}