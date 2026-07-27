#load "compression_aot.cs"

Console.WriteLine("=== compression_aot Test ===");

try
{
    var t0 = typeof(Compression.AOT.CompressionService);
    Console.WriteLine($"[PASS] CompressionService 存在");
    var t1 = typeof(Compression.AOT.CompressionAotEngine);
    Console.WriteLine($"[PASS] CompressionAotEngine 存在");
    var t2 = typeof(Compression.AOT.ICompressionService);
    Console.WriteLine($"[PASS] ICompressionService 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(Compression.AOT.CompressionAlgorithm);
    Console.WriteLine($"[PASS] CompressionAlgorithm enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}