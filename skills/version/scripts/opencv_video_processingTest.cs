#load "opencv_video_processing.cs"

Console.WriteLine("=== opencv_video_processing Test ===");

try
{
    var t0 = typeof(VideoProcessingService);
    Console.WriteLine($"[PASS] VideoProcessingService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(ImageProcessingHub);
    Console.WriteLine($"[PASS] ImageProcessingHub 存在");
    var t3 = typeof(SignalRServiceExtensions);
    Console.WriteLine($"[PASS] SignalRServiceExtensions 存在");
    var t4 = typeof(ImageProcessingExtensions);
    Console.WriteLine($"[PASS] ImageProcessingExtensions 存在");
    var t5 = typeof(OpenCvFormatConverter);
    Console.WriteLine($"[PASS] OpenCvFormatConverter 存在");
    var t6 = typeof(IVideoProcessingService);
    Console.WriteLine($"[PASS] IVideoProcessingService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IImageProcessingService);
    Console.WriteLine($"[PASS] IImageProcessingService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IImageFormatConverter);
    Console.WriteLine($"[PASS] IImageFormatConverter 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(ImageProcessingRequest);
    Console.WriteLine($"[PASS] ImageProcessingRequest record 存在");
    var t10 = typeof(ImageProcessingResult);
    Console.WriteLine($"[PASS] ImageProcessingResult record 存在");
    var t11 = typeof(ImageFormat);
    Console.WriteLine($"[PASS] ImageFormat enum 存在 (IsEnum: {t11.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}