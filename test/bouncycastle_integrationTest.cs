#load "bouncycastle_integration.cs"

Console.WriteLine("=== bouncycastle_integration Test ===");

try
{
    var t0 = typeof(BouncyCastleCryptoService);
    Console.WriteLine($"[PASS] BouncyCastleCryptoService 存在");
    var t1 = typeof(ArrayPoolPolicy);
    Console.WriteLine($"[PASS] ArrayPoolPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}