#load "liquidstate_aot.cs"

Console.WriteLine("=== liquidstate_aot Test ===");

try
{
    var t0 = typeof(LiquidStateAot.TransitionAction);
    Console.WriteLine($"[PASS] TransitionAction 存在");
    var t1 = typeof(LiquidStateAot.StateMachineConfig);
    Console.WriteLine($"[PASS] StateMachineConfig 存在");
    var t2 = typeof(LiquidStateAot.StateMachine);
    Console.WriteLine($"[PASS] StateMachine 存在");
    var t3 = typeof(LiquidStateAot.StateMachineBuilder);
    Console.WriteLine($"[PASS] StateMachineBuilder 存在");
    var t4 = typeof(LiquidStateAot.StateMachineService);
    Console.WriteLine($"[PASS] StateMachineService 存在");
    var t5 = typeof(LiquidStateAot.IStateMachine);
    Console.WriteLine($"[PASS] IStateMachine 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}