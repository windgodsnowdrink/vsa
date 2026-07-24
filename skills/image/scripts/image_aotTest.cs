#load "image_aot.cs"

Console.WriteLine("=== image_aot Test ===");

try
{
    var t0 = typeof(ImageSettings);
    Console.WriteLine($"[PASS] ImageSettings 存在");
    var t1 = typeof(ImageProcessingResult);
    Console.WriteLine($"[PASS] ImageProcessingResult 存在");
    var t2 = typeof(ImageOptimizationResult);
    Console.WriteLine($"[PASS] ImageOptimizationResult 存在");
    var t3 = typeof(ImageMetadataResult);
    Console.WriteLine($"[PASS] ImageMetadataResult 存在");
    var t4 = typeof(BatchProcessingResult);
    Console.WriteLine($"[PASS] BatchProcessingResult 存在");
    var t5 = typeof(BenchmarkResult);
    Console.WriteLine($"[PASS] BenchmarkResult 存在");
    var t6 = typeof(ImageService);
    Console.WriteLine($"[PASS] ImageService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}