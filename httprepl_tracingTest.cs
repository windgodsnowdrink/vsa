#load "httprepl_tracing.cs"

Console.WriteLine("=== httprepl_tracing Test ===");

try
{
    var t0 = typeof(ChannelTraceProcessor);
    Console.WriteLine($"[PASS] ChannelTraceProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}