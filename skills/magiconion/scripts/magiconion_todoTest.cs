#load "magiconion_todo.cs"

Console.WriteLine("=== magiconion_todo Test ===");

try
{
    var t0 = typeof(ChannelTodoProcessor);
    Console.WriteLine($"[PASS] ChannelTodoProcessor 存在");
    var t1 = typeof(MetricsSnapshot);
    Console.WriteLine($"[PASS] MetricsSnapshot 存在");
    var t2 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t3 = typeof(ITodoService);
    Console.WriteLine($"[PASS] ITodoService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}