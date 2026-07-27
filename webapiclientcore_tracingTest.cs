#load "webapiclientcore_tracing.cs"

Console.WriteLine("=== webapiclientcore_tracing Test ===");

try
{
    var t0 = typeof(ChannelTracePropagator);
    Console.WriteLine($"[PASS] ChannelTracePropagator 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}