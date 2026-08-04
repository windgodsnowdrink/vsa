#load "signalr_optimized.cs"

Console.WriteLine("=== signalr_optimized Test ===");

try
{
    var t0 = typeof(AdaptiveCompressionStrategy);
    Console.WriteLine($"[PASS] AdaptiveCompressionStrategy 存在");
    var t1 = typeof(TimeBasedExpiration);
    Console.WriteLine($"[PASS] TimeBasedExpiration 存在");
    var t2 = typeof(DynamicQoSAdjuster);
    Console.WriteLine($"[PASS] DynamicQoSAdjuster 存在");
    var t3 = typeof(CompressionOptions);
    Console.WriteLine($"[PASS] CompressionOptions 存在");
    var t4 = typeof(CompressionType);
    Console.WriteLine($"[PASS] CompressionType enum 存在 (IsEnum: {t4.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}