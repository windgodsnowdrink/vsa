#:sdk Microsoft.NET.Sdk.Web
#:package PaddleSharp@2.4.0
#:package PaddleOCRSharp@2.2.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Buffers;
using System.Threading.Channels;
using PaddleOCRSharp;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Caching.Memory;

// 1. OCR处理器(高性能实现)
[SkipLocalsInit]
public sealed class OcrProcessor : IAsyncDisposable
{
    private readonly Channel<OcrFrame> _ocrChannel;
    private readonly ThreadLocal<Span<byte>> _imageBuffer;
    private readonly ObjectPool<OcrResult> _resultPool;
    private readonly IMemoryCache _resultCache;
    private readonly OCRParameter _ocrParameter;
    private readonly OCRModelConfig _modelConfig;
    private readonly CancellationTokenSource _cts = new();
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public OcrProcessor()
    {
        // 初始化OCR参数(启用MKLDNN加速)
        _ocrParameter = new OCRParameter
        {
            cpu_math_library_num_threads = Environment.ProcessorCount,
            use_angle_cls = true,
            enable_mkldnn = true,
            use_tensorrt = true
        };

        // 初始化模型配置
        _modelConfig = new OCRModelConfig
        {
            det_infer = "ch_PP-OCRv3_det_infer",
            rec_infer = "ch_PP-OCRv3_rec_infer",
            cls_infer = "ch_ppocr_mobile_v2.0_cls_infer",
            lang = "ch"
        };

        // Disruptor模式通道配置
        _ocrChannel = Channel.CreateBounded<OcrFrame>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 线程本地图像缓冲区
        // _imageBuffer = new(() => stackalloc byte[4096 * 4096 * 4]); // 4K图像缓冲区
        _imageBuffer = new(() => 
        {
            var memory = GC.AllocateUninitializedArray<byte>(4096 * 4096 * 4, pinned: true);
            return new Memory<byte>(memory);
        });
        
        // OCR结果对象池
        _resultPool = new DefaultObjectPool<OcrResult>(
            new OcrResultPooledPolicy(), 1000);
        
        _resultCache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 1024 * 1024 * 100 // 100MB缓存
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task ProcessImageAsync(byte[] imageData, string cacheKey = null)
    {
        // 缓存检查
        if (cacheKey != null && _resultCache.TryGetValue(cacheKey, out OcrResult cachedResult))
            return cachedResult;

        var frame = new OcrFrame(imageData);
        await _ocrChannel.Writer.WriteAsync(frame);

        // 等待处理完成
        var tcs = new TaskCompletionSource<OcrResult>();
        EventHandler<OcrResult> handler = null;
        handler = (s, e) => 
        {
            if (e.CacheKey == cacheKey)
            {
                tcs.TrySetResult(e);
                OnOcrCompleted -= handler;
            }
        };
        OnOcrCompleted += handler;
        
        return await tcs.Task;
    }

    private async Task ProcessOcrAsync()
    {
        await foreach (var frame in _ocrChannel.Reader.ReadAllAsync(_cts.Token))
        {
            var result = _resultPool.Get();
            try
            {
                // 零拷贝处理
                // Span<byte> buffer = _imageBuffer.Value;
                // frame.ImageData.AsSpan().CopyTo(buffer);
                var memory = _imageMemory.Value;
                frame.ImageData.AsSpan().CopyTo(memory.Span);
                
                // 使用PaddleOCRSharp进行识别
                using var ocr = new PaddleOCREngine(_modelConfig, _ocrParameter);
                var ocrResult = ocr.DetectText(buffer.ToArray());
                
                // 处理识别结果
                result.Text = ocrResult.Text;
                result.Score = ocrResult.Score;
                result.CacheKey = frame.CacheKey;
                
                // 缓存结果
                if (frame.CacheKey != null)
                {
                    _resultCache.Set(frame.CacheKey, result, 
                        new MemoryCacheEntryOptions
                        {
                            Size = ocrResult.Text.Length * 2,
                            SlidingExpiration = TimeSpan.FromMinutes(30)
                        });
                }

                // 触发结果事件
                OnOcrCompleted?.Invoke(this, result);
            }
            finally
            {
                _resultPool.Return(result);
            }
        }
    }

    public event EventHandler<OcrResult> OnOcrCompleted;

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _ocrChannel.Writer.Complete();
    }
}

// 2. 主程序集成
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<OcrProcessor>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// OCR处理端点
app.MapPost("/ocr", async (byte[] imageData, OcrProcessor processor) =>
{
    await processor.ProcessImageAsync(imageData);
    return Results.Ok("OCR处理已开始");
});

// OCR处理端点(支持缓存)
app.MapPost("/ocr", async (HttpRequest request, EnhancedOcrProcessor processor) =>
{
    using var ms = new MemoryStream();
    await request.Body.CopyToAsync(ms);
    var cacheKey = request.Headers["X-Cache-Key"];
    var result = await processor.ProcessImageAsync(ms.ToArray(), cacheKey);
    return Results.Ok(result);
});

app.Run();

// 3. 辅助类
public record OcrFrame(byte[] ImageData);
public class OcrResult
{
    public string Text { get; set; }
    public float Score { get; set; }
}

public class OcrResultPooledPolicy : IPooledObjectPolicy<OcrResult>
{
    public OcrResult Create() => new OcrResult();
    public bool Return(OcrResult obj) => true;
}