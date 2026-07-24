#load "yarp_compression_middleware.cs"

Console.WriteLine("=== yarp_compression_middleware Test ===");

try
{
    var t0 = typeof(CompressionMiddleware);
    Console.WriteLine($"[PASS] CompressionMiddleware 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}