#load "magiconion_persistence_full.cs"

Console.WriteLine("=== magiconion_persistence_full Test ===");

try
{
    var t0 = typeof(HybridCacheStrategy);
    Console.WriteLine($"[PASS] HybridCacheStrategy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}