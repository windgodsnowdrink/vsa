#load "kiota_cache.cs"

Console.WriteLine("=== kiota_cache Test ===");

try
{
    var t0 = typeof(ChannelCacheHandler);
    Console.WriteLine($"[PASS] ChannelCacheHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}