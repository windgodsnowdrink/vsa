#load "netcore_encrypt_cache.cs"

Console.WriteLine("=== netcore_encrypt_cache Test ===");

try
{
    var t0 = typeof(EncryptCacheService);
    Console.WriteLine($"[PASS] EncryptCacheService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}