#load "webapiclientcore_circuitbreaker_enhanced.cs"

Console.WriteLine("=== webapiclientcore_circuitbreaker_enhanced Test ===");

try
{
    var t0 = typeof(ChannelCircuitStateHandler);
    Console.WriteLine($"[PASS] ChannelCircuitStateHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}