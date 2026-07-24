#load "lazycache_integration.cs"

Console.WriteLine("=== lazycache_integration Test ===");

try
{
    var t0 = typeof(HybridCacheStrategy);
    Console.WriteLine($"[PASS] HybridCacheStrategy 存在");
    var t1 = typeof(MultiLayerCacheExpirationStrategy);
    Console.WriteLine($"[PASS] MultiLayerCacheExpirationStrategy 存在");
    var t2 = typeof(ChannelCacheProcessor);
    Console.WriteLine($"[PASS] ChannelCacheProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}