#:sdk Microsoft.NET.Sdk.Web
#:package PaddleOCRSharp@2.2.0
#:package PaddleSharp@2.4.0
#:package OpenCvSharp4@4.8.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Buffers;
using System.Threading.Channels;
using PaddleOCRSharp;
using OpenCvSharp;
using Microsoft.Extensions.ObjectPool;

// 1. 扩展OCR处理器(支持颜色/人脸/行为分析)
[SkipLocalsInit]
public sealed class ExtendedOcrProcessor : IAsyncDisposable
{
    private readonly Channel<AnalysisFrame> _analysisChannel;
    private readonly ObjectPool<AnalysisResult> _resultPool;
    private readonly OCRParameter _ocrParameter;
    private readonly OCRModelConfig _modelConfig;
    private readonly CancellationTokenSource _cts = new();

    public ExtendedOcrProcessor()
    {
        // 初始化分析通道(Disruptor模式)
        _analysisChannel = Channel.CreateBounded<AnalysisFrame>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 结果对象池
        _resultPool = new DefaultObjectPool<AnalysisResult>(
            new AnalysisResultPooledPolicy(), 100);

        // OCR配置
        _ocrParameter = new OCRParameter { use_angle_cls = true };
        _modelConfig = new OCRModelConfig();
    }

    // 2. 颜色识别功能
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private Color DetectColor(Mat image)
    {
        using var hsv = new Mat();
        Cv2.CvtColor(image, hsv, ColorConversionCodes.BGR2HSV);
        
        // 使用直方图分析主色调
        using var hist = new Mat();
        int[] hRanges = {0, 180};
        int[] sRanges = {0, 256};
        int[] vRanges = {0, 256};
        Cv2.CalcHist(new[]{hsv}, new[]{0,1,2}, null, hist, 3, 
            new[]{180,256,256}, new[]{hRanges, sRanges, vRanges});
        
        // 返回主要HSV颜色
        return new Color(hist.At<float>(0), hist.At<float>(1), hist.At<float>(2));
    }

    // 3. 人脸检测功能
    private Rect[] DetectFaces(Mat image)
    {
        using var classifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
        using var gray = new Mat();
        Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
        
        return classifier.DetectMultiScale(gray, 1.1, 3);
    }

    // 4. 尾随行为分析
    private bool DetectFollowingBehavior(Mat currentFrame, Mat previousFrame)
    {
        if (previousFrame.Empty()) return false;
        
        // 光流分析
        using var prevGray = new Mat();
        using var currGray = new Mat();
        Cv2.CvtColor(previousFrame, prevGray, ColorConversionCodes.BGR2GRAY);
        Cv2.CvtColor(currentFrame, currGray, ColorConversionCodes.BGR2GRAY);
        
        // 使用LK光流法检测运动
        var features = Cv2.GoodFeaturesToTrack(prevGray, 100, 0.01, 10);
        if (features == null || features.Length == 0) return false;
        
        var status = new byte[features.Length];
        var err = new float[features.Length];
        var nextPts = Cv2.CalcOpticalFlowPyrLK(prevGray, currGray, 
            features, null, out status, out err);
        
        // 分析运动向量
        int movingCount = status.Count(x => x == 1);
        return movingCount > features.Length * 0.7;
    }

    // 5. 综合处理流程
    public async Task<AnalysisResult> ProcessFrameAsync(byte[] imageData)
    {
        using var mat = Mat.FromImageData(imageData);
        var result = _resultPool.Get();
        
        try {
            // OCR识别
            using var ocr = new PaddleOCREngine(_modelConfig, _ocrParameter);
            result.Text = ocr.DetectText(imageData).Text;
            
            // 颜色识别
            result.DominantColor = DetectColor(mat);
            
            // 人脸检测
            result.Faces = DetectFaces(mat);
            
            return result;
        }
        finally {
            _resultPool.Return(result);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _analysisChannel.Writer.Complete();
    }
}

// 6. 主程序集成
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<ExtendedOcrProcessor>();

var app = builder.Build();

// 分析端点
app.MapPost("/analyze", async (byte[] imageData, ExtendedOcrProcessor processor) =>
{
    var result = await processor.ProcessFrameAsync(imageData);
    return Results.Ok(result);
});

app.Run();

// 7. 辅助类
public record AnalysisFrame(byte[] ImageData, byte[]? PreviousFrame = null);
public class AnalysisResult
{
    public string Text { get; set; }
    public Color DominantColor { get; set; }
    public Rect[] Faces { get; set; }
    public bool IsFollowing { get; set; }
}

public class AnalysisResultPooledPolicy : IPooledObjectPolicy<AnalysisResult>
{
    public AnalysisResult Create() => new AnalysisResult();
    public bool Return(AnalysisResult obj) => true;
}

public struct Color
{
    public float H { get; }
    public float S { get; }
    public float V { get; }
    
    public Color(float h, float s, float v) => (H,S,V) = (h,s,v);
}