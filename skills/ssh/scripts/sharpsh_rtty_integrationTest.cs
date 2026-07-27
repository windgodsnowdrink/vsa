#load "sharpsh_rtty_integration.cs"

Console.WriteLine("=== sharpsh_rtty_integration Test ===");

try
{
    var t0 = typeof(RttyOptions);
    Console.WriteLine($"[PASS] RttyOptions 存在");
    var t1 = typeof(RttyService);
    Console.WriteLine($"[PASS] RttyService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IRttyService);
    Console.WriteLine($"[PASS] IRttyService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}