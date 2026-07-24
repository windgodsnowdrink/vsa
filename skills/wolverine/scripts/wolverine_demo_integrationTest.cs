#load "wolverine_demo_integration.cs"

Console.WriteLine("=== wolverine_demo_integration Test ===");

try
{
    // 验证 GreetingCommand record
    var greetingCmdType = typeof(GreetingCommand);
    Console.WriteLine($"[PASS] GreetingCommand 类型存在: {greetingCmdType.Name}");

    // 验证 GreetingResponse record
    var greetingRespType = typeof(GreetingResponse);
    Console.WriteLine($"[PASS] GreetingResponse 类型存在: {greetingRespType.Name}");

    // 验证 GreetingHandler 类
    var handlerType = typeof(GreetingHandler);
    Console.WriteLine($"[PASS] GreetingHandler 类型存在: {handlerType.Name}");

    // 验证 AppJsonContext 类
    var jsonContextType = typeof(AppJsonContext);
    Console.WriteLine($"[PASS] AppJsonContext 类型存在: {jsonContextType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}