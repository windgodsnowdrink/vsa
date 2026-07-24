#load "wolverine_demo_integration.cs"

Console.WriteLine("=== wolverine_demo_integration Test ===");

try
{
    var t0 = typeof(GreetingHandler);
    Console.WriteLine($"[PASS] GreetingHandler 存在");
    var t1 = typeof(AppJsonContext);
    Console.WriteLine($"[PASS] AppJsonContext 存在");
    var t2 = typeof(GreetingCommand);
    Console.WriteLine($"[PASS] GreetingCommand record 存在");
    var t3 = typeof(GreetingResponse);
    Console.WriteLine($"[PASS] GreetingResponse record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}