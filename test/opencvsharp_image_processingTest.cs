#load "opencvsharp_image_processing.cs"

Console.WriteLine("=== opencvsharp_image_processing Test ===");

try
{
    var t0 = typeof(ImageProcessor);
    Console.WriteLine($"[PASS] ImageProcessor 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}