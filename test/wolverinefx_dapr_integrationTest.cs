#load "wolverinefx_dapr_integration.cs"

Console.WriteLine("=== wolverinefx_dapr_integration Test ===");

try
{
    var t0 = typeof(DaprEventBusOptions);
    Console.WriteLine($"[PASS] DaprEventBusOptions 存在");
    var t1 = typeof(DaprEventBusExtensions);
    Console.WriteLine($"[PASS] DaprEventBusExtensions 存在");
    var t2 = typeof(DaprHealthCheck);
    Console.WriteLine($"[PASS] DaprHealthCheck 存在");
    var t3 = typeof(OrderHandlers);
    Console.WriteLine($"[PASS] OrderHandlers 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}