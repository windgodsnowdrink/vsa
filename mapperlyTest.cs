#load "mapperly.cs"

Console.WriteLine("=== mapperly Test ===");

try
{
    var t0 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(TodoItemDto);
    Console.WriteLine($"[PASS] TodoItemDto 存在");
    var t2 = typeof(TodoMapper);
    Console.WriteLine($"[PASS] TodoMapper 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}