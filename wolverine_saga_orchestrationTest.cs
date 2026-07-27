#load "wolverine_saga_orchestration.cs"

Console.WriteLine("=== wolverine_saga_orchestration Test ===");

try
{
    var t0 = typeof(TodoSagaOrchestrator);
    Console.WriteLine($"[PASS] TodoSagaOrchestrator 存在");
    var t1 = typeof(CreateTodoItemEvent);
    Console.WriteLine($"[PASS] CreateTodoItemEvent record 存在");
    var t2 = typeof(NotifyUserEvent);
    Console.WriteLine($"[PASS] NotifyUserEvent record 存在");
    var t3 = typeof(UserNotifiedEvent);
    Console.WriteLine($"[PASS] UserNotifiedEvent record 存在");
    var t4 = typeof(CompleteTodoSagaCommand);
    Console.WriteLine($"[PASS] CompleteTodoSagaCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}