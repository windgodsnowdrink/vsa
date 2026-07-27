#load "state_machine_flow.cs"

Console.WriteLine("=== state_machine_flow Test ===");

try
{
    var t0 = typeof(StateMachineFlow);
    Console.WriteLine($"[PASS] StateMachineFlow 存在");
    var t1 = typeof(TaskItem);
    Console.WriteLine($"[PASS] TaskItem record 存在");
    var t2 = typeof(TaskState);
    Console.WriteLine($"[PASS] TaskState record 存在");
    var t3 = typeof(State);
    Console.WriteLine($"[PASS] State enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}