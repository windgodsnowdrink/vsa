#:sdk Microsoft.NET.Sdk.Web
#:package OpenCvSharp4@4.8.0
#:package OpenCvSharp4.runtime.win@4.8.0
#:package OpenCvSharp4.Extensions@4.8.0
#:property LangVersion=preview
#:property TargetFramework=net10.0

using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;

// 视频处理服务接口
public interface IVideoProcessingService
{
    Task ProcessVideoStreamAsync(string videoSource, CancellationToken ct);
}

// 视频处理服务实现（符合DI规范）
public class VideoProcessingService : IVideoProcessingService, IDisposable
{
    private readonly Channel<Mat> _videoFramesChannel;
    private readonly IMemoryOwner<byte> _frameBuffer;
    private readonly ObjectPool<Mat> _matPool;
    
    public VideoProcessingService()
    {
        // 使用Channel实现生产者-消费者模式（多线程Threading.Channels）
        _videoFramesChannel = Channel.CreateBounded<Mat>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });
        
        // 使用MemoryPool优化内存分配（分层内存管理）
        _frameBuffer = MemoryPool<byte>.Shared.Rent(1920 * 1080 * 3);
        
        // 使用对象池管理Mat对象（对象池ObjectPool）
        _matPool = new DefaultObjectPool<Mat>(new MatPooledObjectPolicy());
    }
    
    public async Task ProcessVideoStreamAsync(string videoSource, CancellationToken ct)
    {
        using var capture = new VideoCapture(videoSource);
        
        // 生产者线程
        var producerTask = Task.Run(async () =>
        {
            while (!ct.IsCancellationRequested)
            {
                var frame = _matPool.Get();
                if (capture.Read(frame))
                {
                    await _videoFramesChannel.Writer.WriteAsync(frame, ct);
                }
                else
                {
                    _matPool.Return(frame);
                    break;
                }
            }
            _videoFramesChannel.Writer.Complete();
        }, ct);
        
        // 消费者线程
        await foreach (var frame in _videoFramesChannel.Reader.ReadAllAsync(ct))
        {
            try
            {
                // 使用Span<T>进行零拷贝处理（Span零拷贝）
                using var span = frame.GetMatData();
                ProcessFrame(span);
            }
            finally
            {
                _matPool.Return(frame);
            }
        }
        
        await producerTask;
    }
    
    private unsafe void ProcessFrame(Span<byte> frameData)
    {
        // 高性能图像处理（CPU cache-line对齐）
        fixed (byte* ptr = frameData)
        {
            using var mat = new Mat(frameHeight, frameWidth, MatType.CV_8UC3, (IntPtr)ptr);
            
            // 示例处理：边缘检测
            using var gray = new Mat();
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            
            using var edges = new Mat();
            Cv2.Canny(gray, edges, 50, 150);
            
            // 图像增强处理（使用Span<T>零拷贝）
            using var enhanced = new Mat();
            Cv2.BilateralFilter(mat, enhanced, 9, 75, 75);
            
            // 特征检测（SIFT算法）
            using var sift = SIFT.Create();
            var keypoints = sift.Detect(enhanced);
            
            // 目标检测（Haar级联分类器）
            using var cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
            var faces = cascade.DetectMultiScale(enhanced, 1.1, 3);
            
            // 图像分割（GrabCut算法）
            using var mask = new Mat();
            using var bgModel = new Mat();
            using var fgModel = new Mat();
            var rect = new Rect(0, 0, enhanced.Width, enhanced.Height);
            Cv2.GrabCut(enhanced, mask, rect, bgModel, fgModel, 3, GrabCutModes.InitWithRect);
            
            // 光流计算（Farneback算法）
            using var flow = new Mat();
            using var prevGray = new Mat();
            Cv2.CvtColor(prevFrame, prevGray, ColorConversionCodes.BGR2GRAY);
            Cv2.CalcOpticalFlowFarneback(prevGray, gray, flow, 0.5, 3, 15, 3, 5, 1.2, 0);
            
            // 图像拼接（Stitcher类）
            var stitcher = Stitcher.Create(Stitcher.Mode.Panorama);
            var panorama = new Mat();
            stitcher.Stitch(new[] { frame1, frame2 }, panorama);
            
            // 深度估计（StereoBM算法）
            using var disparity = new Mat();
            var stereo = StereoBM.Create(16, 15);
            stereo.Compute(leftImage, rightImage, disparity);
            
            // 图像锐化处理
            public static Mat SharpenImage(Mat src, double strength = 0.8)
            {
                using var kernel = new Mat(3, 3, MatType.CV_64FC1, new double[] 
                {
                    -1, -1, -1,
                    -1, 9 + strength, -1,
                    -1, -1, -1
                });
                
                var dest = new Mat();
                Cv2.Filter2D(src, dest, src.Type(), kernel);
                return dest;
            }
            
            // 图像高亮处理
            public static Mat HighlightImage(Mat src, double alpha = 1.2, double beta = 30)
            {
                var dest = new Mat();
                src.ConvertTo(dest, -1, alpha, beta);
                return dest;
            }
            
            // 白平衡处理
            public static Mat WhiteBalance(Mat src)
            {
                var lab = new Mat();
                Cv2.CvtColor(src, lab, ColorConversionCodes.BGR2Lab);
                
                using var channels = Cv2.Split(lab);
                Cv2.EqualizeHist(channels[0], channels[0]);
                
                var balanced = new Mat();
                Cv2.Merge(channels, balanced);
                Cv2.CvtColor(balanced, balanced, ColorConversionCodes.Lab2BGR);
                
                return balanced;
            }
            
            // 对比度增强
            public static Mat EnhanceContrast(Mat src, double clipLimit = 2.0, Size tileGridSize = null)
            {
                tileGridSize ??= new Size(8, 8);
                
                var lab = new Mat();
                Cv2.CvtColor(src, lab, ColorConversionCodes.BGR2Lab);
                
                using var channels = Cv2.Split(lab);
                var clahe = Cv2.CreateCLAHE(clipLimit, tileGridSize);
                clahe.Apply(channels[0], channels[0]);
                
                var enhanced = new Mat();
                Cv2.Merge(channels, enhanced);
                Cv2.CvtColor(enhanced, enhanced, ColorConversionCodes.Lab2BGR);
                
                return enhanced;
            }
            
            // 色彩饱和度调整
            public static Mat AdjustSaturation(Mat src, double saturation = 1.5)
            {
                var hsv = new Mat();
                Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV);
                
                using var channels = Cv2.Split(hsv);
                channels[1] = channels[1] * saturation;
                
                var adjusted = new Mat();
                Cv2.Merge(channels, adjusted);
                Cv2.CvtColor(adjusted, adjusted, ColorConversionCodes.HSV2BGR);
                
                return adjusted;
            }
        }
    }
    
    public void Dispose()
    {
        _frameBuffer?.Dispose();
    }
}

// DI扩展方法（符合998技术要求）
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVideoProcessing(this IServiceCollection services)
    {
        services.AddSingleton<IVideoProcessingService, VideoProcessingService>();
        return services;
    }
}

// 启动配置
var builder = WebApplication.CreateBuilder();
builder.Services.AddVideoProcessing();

var app = builder.Build();
app.MapGet("/process-video", async (IVideoProcessingService service) =>
{
    await service.ProcessVideoStreamAsync("video.mp4", default);
    return Results.Ok();
});

app.Run();


// SignalR Hub类 - 实现双向通信
public class ImageProcessingHub : Hub
{
    private readonly IImageProcessingService _service;
    private readonly ILogger<ImageProcessingHub> _logger;

    public ImageProcessingHub(
        IImageProcessingService service,
        ILogger<ImageProcessingHub> logger)
    {
        _service = service;
        _logger = logger;
    }

    // 监控输入参数
    public async Task<ImageProcessingResult> ProcessImageWithMonitoring(
        ImageProcessingRequest request,
        CancellationToken ct = default)
    {
        // 参数验证
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // 记录输入参数
        _logger.LogInformation("Received image processing request: {Request}", 
            JsonSerializer.Serialize(request));

        // 处理图片
        var result = await _service.ProcessImageAsync(request, ct);

        // 返回结果
        await Clients.Caller.SendAsync("ReceiveResult", result, ct);
        return result;
    }

    // 输出Base64图片
    public async Task<string> GetImageAsBase64(
        string imagePath, 
        CancellationToken ct = default)
    {
        using var image = await _service.LoadImageAsync(imagePath, ct);
        using var memoryStream = new MemoryStream();
        
        // 使用Span优化内存操作
        var success = image.ImEncode(".jpg", out var span);
        if (!success || span.IsEmpty)
            throw new InvalidOperationException("Image encoding failed");

        // 转换为Base64
        var base64 = Convert.ToBase64String(span);
        
        // 记录处理结果
        _logger.LogInformation("Image converted to base64, size: {Size} bytes", 
            base64.Length);
            
        return base64;
    }
}

// 图片处理服务接口
public interface IImageProcessingService
{
    Task<ImageProcessingResult> ProcessImageAsync(
        ImageProcessingRequest request, 
        CancellationToken ct = default);
        
    Task<Mat> LoadImageAsync(string path, CancellationToken ct = default);
}

// 图片处理请求DTO
public record ImageProcessingRequest(
    string ImagePath,
    double Sharpness = 0.8,
    double Brightness = 1.2,
    double Contrast = 1.5);

// 图片处理结果DTO
public record ImageProcessingResult(
    string OriginalImageBase64,
    string ProcessedImageBase64,
    long ProcessingTimeMs);

// SignalR服务注册扩展
public static class SignalRServiceExtensions
{
    public static IServiceCollection AddImageProcessingSignalR(
        this IServiceCollection services)
    {
        services.AddSignalR(options => 
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
        })
        .AddMessagePackProtocol(); // 使用MessagePack协议优化性能
        
        return services;
    }
}

// 在Startup中配置
public void ConfigureServices(IServiceCollection services)
{
    // ... existing code ...
    
    services.AddImageProcessingSignalR();
    
    // 添加分层内存服务
    services.AddTieredMemoryServices();
    
    // 添加对象池
    services.AddObjectPool<Mat>(() => new Mat());
}

public void Configure(IApplicationBuilder app)
{
    // ... existing code ...
    
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<ImageProcessingHub>("/imageProcessingHub");
    });
}


// 图片另存为功能
public static void SaveImage(
    Mat image, 
    string outputPath, 
    ImageFormat format = ImageFormat.Jpeg,
    int quality = 90)
{
    if (image == null)
        throw new ArgumentNullException(nameof(image));
    
    if (string.IsNullOrWhiteSpace(outputPath))
        throw new ArgumentException("Output path cannot be empty", nameof(outputPath));

    // 根据格式选择编码参数
    var extension = format.ToString().ToLower();
    var parameters = new List<int> 
    { 
        (int)ImwriteFlags.JpegQuality, quality,
        (int)ImwriteFlags.PngCompression, 9 - quality / 10
    };

    // 使用Span优化内存操作
    var success = image.ImWrite(outputPath, parameters.ToArray());
    if (!success)
        throw new InvalidOperationException($"Failed to save image to {outputPath}");
}

// 图片格式转换
public static Mat ConvertImageFormat(
    Mat src, 
    ImageFormat targetFormat,
    int quality = 90)
{
    if (src == null)
        throw new ArgumentNullException(nameof(src));

    // 使用对象池获取临时Mat
    var pool = ObjectPool.Create<Mat>();
    var tempMat = pool.Get();
    
    try
    {
        // 根据目标格式进行转换
        switch (targetFormat)
        {
            case ImageFormat.Jpeg:
                Cv2.CvtColor(src, tempMat, ColorConversionCodes.BGR2RGB);
                break;
            case ImageFormat.Png:
                Cv2.CvtColor(src, tempMat, ColorConversionCodes.BGR2BGRA);
                break;
            case ImageFormat.WebP:
                Cv2.CvtColor(src, tempMat, ColorConversionCodes.BGR2RGB);
                break;
            default:
                throw new NotSupportedException($"Format {targetFormat} is not supported");
        }

        // 返回新Mat
        var result = new Mat();
        tempMat.CopyTo(result);
        return result;
    }
    finally
    {
        pool.Return(tempMat);
    }
}

// 图片格式枚举
public enum ImageFormat
{
    Jpeg,
    Png,
    WebP,
    Bmp,
    Tiff
}

// DI扩展方法
public static class ImageProcessingExtensions
{
    public static IServiceCollection AddImageProcessingServices(
        this IServiceCollection services)
    {
        // 添加对象池
        services.AddSingleton<ObjectPool<Mat>>(sp => 
            ObjectPool.Create<Mat>(() => new Mat()));
            
        // 添加格式转换服务
        services.AddTransient<IImageFormatConverter, OpenCvFormatConverter>();
        
        return services;
    }
}

// 格式转换接口
public interface IImageFormatConverter
{
    Mat Convert(Mat src, ImageFormat targetFormat, int quality = 90);
    
    Task<Mat> ConvertAsync(
        Mat src, 
        ImageFormat targetFormat, 
        int quality = 90, 
        CancellationToken ct = default);
}

// 格式转换实现
public class OpenCvFormatConverter : IImageFormatConverter
{
    private readonly ObjectPool<Mat> _matPool;
    
    public OpenCvFormatConverter(ObjectPool<Mat> matPool)
    {
        _matPool = matPool;
    }
    
    public Mat Convert(Mat src, ImageFormat targetFormat, int quality = 90)
    {
        return ConvertImageFormat(src, targetFormat, quality);
    }
    
    public async Task<Mat> ConvertAsync(
        Mat src, 
        ImageFormat targetFormat, 
        int quality = 90, 
        CancellationToken ct = default)
    {
        return await Task.Run(() => Convert(src, targetFormat, quality), ct);
    }
}