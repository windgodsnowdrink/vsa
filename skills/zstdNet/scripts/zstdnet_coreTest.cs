#load "zstdnet_core.cs"

Console.WriteLine("=== zstdnet_core Test ===");

try
{
    var t0 = typeof(ZstdNet.ZstdCompressionService);
    Console.WriteLine($"[PASS] ZstdCompressionService 存在");
    var t1 = typeof(ZstdNet.ParallelZstdCompressionService);
    Console.WriteLine($"[PASS] ParallelZstdCompressionService 存在");
    var t2 = typeof(ZstdNet.MemoryOptimizedZstdCompressionService);
    Console.WriteLine($"[PASS] MemoryOptimizedZstdCompressionService 存在");
    var t3 = typeof(ZstdNet.ZstdCompressionExtensions);
    Console.WriteLine($"[PASS] ZstdCompressionExtensions 存在");
    var t4 = typeof(ZstdNet.ZstdCompressionOptions);
    Console.WriteLine($"[PASS] ZstdCompressionOptions 存在");
    var t5 = typeof(ZstdNet.ZstdPerformanceTester);
    Console.WriteLine($"[PASS] ZstdPerformanceTester 存在");
    var t6 = typeof(ZstdNet.PerformanceTestResult);
    Console.WriteLine($"[PASS] PerformanceTestResult 存在");
    var t7 = typeof(ZstdNet.IZstdCompressionService);
    Console.WriteLine($"[PASS] IZstdCompressionService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}