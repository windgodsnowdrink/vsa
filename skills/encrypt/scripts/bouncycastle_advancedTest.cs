#load "bouncycastle_advanced.cs"

Console.WriteLine("=== bouncycastle_advanced Test ===");

try
{
    var t0 = typeof(BouncyCastleAdvancedService);
    Console.WriteLine($"[PASS] BouncyCastleAdvancedService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}