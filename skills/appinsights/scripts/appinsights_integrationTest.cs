#load "appinsights_integration.cs"

Console.WriteLine("=== appinsights_integration Test ===");

try
{
    var t0 = typeof(ChannelTelemetryProcessor);
    Console.WriteLine($"[PASS] ChannelTelemetryProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}