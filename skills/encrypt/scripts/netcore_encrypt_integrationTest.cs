#load "netcore_encrypt_integration.cs"

Console.WriteLine("=== netcore_encrypt_integration Test ===");

try
{
    var t0 = typeof(EncryptService);
    Console.WriteLine($"[PASS] EncryptService 存在");
    var t1 = typeof(ArrayPoolPolicy);
    Console.WriteLine($"[PASS] ArrayPoolPolicy 存在");
    var t2 = typeof(ShaType);
    Console.WriteLine($"[PASS] ShaType enum 存在 (IsEnum: {t2.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}