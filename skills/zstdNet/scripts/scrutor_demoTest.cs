#load "scrutor_demo.cs"

Console.WriteLine("=== scrutor_demo Test ===");

try
{
    var t0 = typeof(ZstdNet.ScrutorDemo.ZstdCompressionService);
    Console.WriteLine($"[PASS] ZstdCompressionService 存在");
    var t1 = typeof(ZstdNet.ScrutorDemo.PerformanceMonitoringCompressionDecorator);
    Console.WriteLine($"[PASS] PerformanceMonitoringCompressionDecorator 存在");
    var t2 = typeof(ZstdNet.ScrutorDemo.CachingCompressionDecorator);
    Console.WriteLine($"[PASS] CachingCompressionDecorator 存在");
    var t3 = typeof(ZstdNet.ScrutorDemo.RetryCompressionDecorator);
    Console.WriteLine($"[PASS] RetryCompressionDecorator 存在");
    var t4 = typeof(ZstdNet.ScrutorDemo.AdvancedZstdCompressionService);
    Console.WriteLine($"[PASS] AdvancedZstdCompressionService 存在");
    var t5 = typeof(ZstdNet.ScrutorDemo.SampleAutoRegisterService);
    Console.WriteLine($"[PASS] SampleAutoRegisterService 存在");
    var t6 = typeof(ZstdNet.ScrutorDemo.AnotherAutoRegisterService);
    Console.WriteLine($"[PASS] AnotherAutoRegisterService 存在");
    var t7 = typeof(ZstdNet.ScrutorDemo.ScrutorExtensions);
    Console.WriteLine($"[PASS] ScrutorExtensions 存在");
    var t8 = typeof(ZstdNet.ScrutorDemo.ICompressionService);
    Console.WriteLine($"[PASS] ICompressionService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(ZstdNet.ScrutorDemo.IAdvancedCompressionService);
    Console.WriteLine($"[PASS] IAdvancedCompressionService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(ZstdNet.ScrutorDemo.IAutoRegister);
    Console.WriteLine($"[PASS] IAutoRegister 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}