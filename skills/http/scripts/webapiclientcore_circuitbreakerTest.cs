#load "webapiclientcore_circuitbreaker.cs"

Console.WriteLine("=== webapiclientcore_circuitbreaker Test ===");

try
{
    var t0 = typeof(ChannelCircuitEventHandler);
    Console.WriteLine($"[PASS] ChannelCircuitEventHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}