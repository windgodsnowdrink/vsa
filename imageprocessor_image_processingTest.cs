#load "imageprocessor_image_processing.cs"

Console.WriteLine("=== imageprocessor_image_processing Test ===");

try
{
    var t0 = typeof(ImageProcessorService);
    Console.WriteLine($"[PASS] ImageProcessorService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IImageProcessorService);
    Console.WriteLine($"[PASS] IImageProcessorService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}