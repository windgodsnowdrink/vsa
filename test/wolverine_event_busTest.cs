#load "wolverine_event_bus.cs"

Console.WriteLine("=== wolverine_event_bus Test ===");

try
{
    var t0 = typeof(TodoEventHandler);
    Console.WriteLine($"[PASS] TodoEventHandler 存在");
    var t1 = typeof(TodoCreated);
    Console.WriteLine($"[PASS] TodoCreated record 存在");
    var t2 = typeof(TodoCompleted);
    Console.WriteLine($"[PASS] TodoCompleted record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}