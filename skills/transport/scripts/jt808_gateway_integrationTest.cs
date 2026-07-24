#load "jt808_gateway_integration.cs"

Console.WriteLine("=== jt808_gateway_integration Test ===");

try
{
    var t0 = typeof(JT808GatewayService);
    Console.WriteLine($"[PASS] JT808GatewayService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IJT808GatewayService);
    Console.WriteLine($"[PASS] IJT808GatewayService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}