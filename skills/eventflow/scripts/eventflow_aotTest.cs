#load "eventflow_aot.cs"

Console.WriteLine("=== eventflow_aot Test ===");

try
{
    var t0 = typeof(EventFlow.AOT.EventFlowOptions);
    Console.WriteLine($"[PASS] EventFlowOptions 存在");
    var t1 = typeof(EventFlow.AOT.EventData);
    Console.WriteLine($"[PASS] EventData 存在");
    var t2 = typeof(EventFlow.AOT.EventFlowCommandResult);
    Console.WriteLine($"[PASS] EventFlowCommandResult 存在");
    var t3 = typeof(EventFlow.AOT.EventFlowService);
    Console.WriteLine($"[PASS] EventFlowService 存在");
    var t4 = typeof(EventFlow.AOT.EventFlowAotEngine);
    Console.WriteLine($"[PASS] EventFlowAotEngine 存在");
    var t5 = typeof(EventFlow.AOT.IEventFlowService);
    Console.WriteLine($"[PASS] IEventFlowService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(EventFlow.AOT.EventFlowCommandType);
    Console.WriteLine($"[PASS] EventFlowCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}