#:sdk Microsoft.NET.Sdk.Web
#:package OpenCvSharp4@4.8.0
#:package OpenCvSharp4.runtime.win@4.8.0
#:package OpenCvSharp4.Extensions@4.8.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using OpenCvSharp;
using System.Buffers;
using System.Threading.Channels;

public class ImageProcessor
{
    private readonly Channel<Mat> _processingChannel;
    private readonly IMemoryOwner<byte> _memoryOwner;
    
    public ImageProcessor()
    {
        // 使用Channel实现生产者-消费者模式
        _processingChannel = Channel.CreateBounded<Mat>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
        
        // 使用内存池优化
        _memoryOwner = MemoryPool<byte>.Shared.Rent(1024 * 1024 * 10); // 10MB
    }
    
    public async Task ProcessImageAsync(string imagePath)
    {
        // 使用Span<T>优化内存分配
        using var src = new Mat(imagePath, ImreadModes.Color);
        
        // 预处理 - 高斯模糊
        using var blurred = new Mat();
        Cv2.GaussianBlur(src, blurred, new Size(5, 5), 1.5);
        
        // 边缘检测
        using var edges = new Mat();
        Cv2.Canny(blurred, edges, 100, 200);
        
        // 使用零拷贝技术处理图像
        ProcessWithSpan(edges);
        
        // 将结果写入通道
        await _processingChannel.Writer.WriteAsync(edges);
    }
    
    private unsafe void ProcessWithSpan(Mat mat)
    {
        // 使用Span<T>进行高性能处理
        var span = new Span<byte>(mat.Data.ToPointer(), (int)mat.Total() * mat.Channels());
        
        // 示例处理：反转颜色
        for (int i = 0; i < span.Length; i++)
        {
            span[i] = (byte)(255 - span[i]);
        }
    }
    
    public async Task StartProcessingAsync(CancellationToken cancellationToken)
    {
        // 启动后台处理任务
        while (await _processingChannel.Reader.WaitToReadAsync(cancellationToken))
        {
            if (_processingChannel.Reader.TryRead(out var mat))
            {
                try
                {
                    // 实际处理逻辑
                    using var dst = new Mat();
                    Cv2.Threshold(mat, dst, 0, 255, ThresholdTypes.Otsu);
                    
                    // 保存或进一步处理
                    Cv2.ImWrite("processed_" + Guid.NewGuid() + ".png", dst);
                }
                finally
                {
                    mat.Dispose();
                }
            }
        }
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddImageProcessing(this IServiceCollection services)
    {
        services.AddSingleton<ImageProcessor>();
        return services;
    }
}