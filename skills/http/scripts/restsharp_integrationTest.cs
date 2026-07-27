#load "restsharp_integration.cs"

Console.WriteLine("=== restsharp_integration Test ===");

try
{
    var t0 = typeof(ChannelRestClientFactory);
    Console.WriteLine($"[PASS] ChannelRestClientFactory 存在");
    var t1 = typeof(PollyRetryStrategy);
    Console.WriteLine($"[PASS] PollyRetryStrategy 存在");
    var t2 = typeof(MemoryCacheStrategy);
    Console.WriteLine($"[PASS] MemoryCacheStrategy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}