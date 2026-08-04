#load "httprepl_integration.cs"

Console.WriteLine("=== httprepl_integration Test ===");

try
{
    var t0 = typeof(ChannelHttpReplClientFactory);
    Console.WriteLine($"[PASS] ChannelHttpReplClientFactory 存在");
    var t1 = typeof(HttpReplRetryStrategy);
    Console.WriteLine($"[PASS] HttpReplRetryStrategy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}