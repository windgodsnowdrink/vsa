#load "todo_repository.cs"

Console.WriteLine("=== todo_repository Test ===");

try
{
    var t0 = typeof(VSA.TodoSystem.Infrastructure.TodoRepository);
    Console.WriteLine($"[PASS] TodoRepository 存在");
    var t1 = typeof(VSA.TodoSystem.Infrastructure.EventBus);
    Console.WriteLine($"[PASS] EventBus 存在");
    var t2 = typeof(VSA.TodoSystem.Infrastructure.ITodoRepository);
    Console.WriteLine($"[PASS] ITodoRepository 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}