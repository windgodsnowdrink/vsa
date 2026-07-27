#load "voip_integration.cs"

Console.WriteLine("=== voip_integration Test ===");

try
{
    var t0 = typeof(VoipOptions);
    Console.WriteLine($"[PASS] VoipOptions 存在");
    var t1 = typeof(VoipService);
    Console.WriteLine($"[PASS] VoipService 存在");
    var t2 = typeof(VoipExtensions);
    Console.WriteLine($"[PASS] VoipExtensions 存在");
    var t3 = typeof(IVoipService);
    Console.WriteLine($"[PASS] IVoipService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}