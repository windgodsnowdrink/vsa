#load "antixss_integration.cs"

Console.WriteLine("=== antixss_integration Test ===");

try
{
    var t0 = typeof(AntiXssService);
    Console.WriteLine($"[PASS] AntiXssService 存在");
    var t1 = typeof(EncoderPooledPolicy);
    Console.WriteLine($"[PASS] EncoderPooledPolicy 存在");
    var t2 = typeof(SanitizeRequest);
    Console.WriteLine($"[PASS] SanitizeRequest record 存在");
    var t3 = typeof(SanitizeType);
    Console.WriteLine($"[PASS] SanitizeType enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}