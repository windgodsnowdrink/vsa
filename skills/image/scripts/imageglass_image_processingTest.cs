#load "imageglass_image_processing.cs"

Console.WriteLine("=== imageglass_image_processing Test ===");

try
{
    var t0 = typeof(ImageGlassProcessingService);
    Console.WriteLine($"[PASS] ImageGlassProcessingService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IImageGlassProcessingService);
    Console.WriteLine($"[PASS] IImageGlassProcessingService 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(FilterType);
    Console.WriteLine($"[PASS] FilterType enum 存在 (IsEnum: {t3.IsEnum})");
    var t4 = typeof(ImageFormat);
    Console.WriteLine($"[PASS] ImageFormat enum 存在 (IsEnum: {t4.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}