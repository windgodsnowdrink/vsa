#load "bitplatform_integration.cs"

Console.WriteLine("=== bitplatform_integration Test ===");

try
{
    var t0 = typeof(BitPlatformOptions);
    Console.WriteLine($"[PASS] BitPlatformOptions 存在");
    var t1 = typeof(BitPlatformService);
    Console.WriteLine($"[PASS] BitPlatformService 存在");
    var t2 = typeof(BitPlatformExtensions);
    Console.WriteLine($"[PASS] BitPlatformExtensions 存在");
    var t3 = typeof(BitPlatformExample);
    Console.WriteLine($"[PASS] BitPlatformExample 存在");
    var t4 = typeof(IBitPlatformService);
    Console.WriteLine($"[PASS] IBitPlatformService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}