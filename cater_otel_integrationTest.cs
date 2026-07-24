#load "cater_otel_integration.cs"

Console.WriteLine("=== cater_otel_integration Test ===");

try
{
    var t0 = typeof(CarterModuleBase);
    Console.WriteLine($"[PASS] CarterModuleBase 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}