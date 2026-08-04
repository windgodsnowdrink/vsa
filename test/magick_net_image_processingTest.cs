#load "magick_net_image_processing.cs"

Console.WriteLine("=== magick_net_image_processing Test ===");

try
{
    var t0 = typeof(ImageProcessingService);
    Console.WriteLine($"[PASS] ImageProcessingService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(MagickImagePooledObjectPolicy);
    Console.WriteLine($"[PASS] MagickImagePooledObjectPolicy 存在");
    var t3 = typeof(IImageProcessingService);
    Console.WriteLine($"[PASS] IImageProcessingService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}