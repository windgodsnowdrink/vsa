#load "gateway_aot.cs"

Console.WriteLine("=== gateway_aot Test ===");

try
{
    var t0 = typeof(Gateway.AOT.GatewayOptions);
    Console.WriteLine($"[PASS] GatewayOptions 存在");
    var t1 = typeof(Gateway.AOT.RouteConfig);
    Console.WriteLine($"[PASS] RouteConfig 存在");
    var t2 = typeof(Gateway.AOT.GatewayCommandResult);
    Console.WriteLine($"[PASS] GatewayCommandResult 存在");
    var t3 = typeof(Gateway.AOT.GatewayService);
    Console.WriteLine($"[PASS] GatewayService 存在");
    var t4 = typeof(Gateway.AOT.GatewayAotEngine);
    Console.WriteLine($"[PASS] GatewayAotEngine 存在");
    var t5 = typeof(Gateway.AOT.IGatewayService);
    Console.WriteLine($"[PASS] IGatewayService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(Gateway.AOT.GatewayCommandType);
    Console.WriteLine($"[PASS] GatewayCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}