#load "wolverine_integration_full.cs"

Console.WriteLine("=== wolverine_integration_full Test ===");

try
{
    var t0 = typeof(TodoHandlers);
    Console.WriteLine($"[PASS] TodoHandlers 存在");
    var t1 = typeof(TodoEndpoints);
    Console.WriteLine($"[PASS] TodoEndpoints 存在");
    var t2 = typeof(Todo);
    Console.WriteLine($"[PASS] Todo 存在");
    var t3 = typeof(CreateTodoValidator);
    Console.WriteLine($"[PASS] CreateTodoValidator 存在");
    var t4 = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");
    var t5 = typeof(OrderValidator);
    Console.WriteLine($"[PASS] OrderValidator 存在");
    var t6 = typeof(CreateTodo);
    Console.WriteLine($"[PASS] CreateTodo record 存在");
    var t7 = typeof(TodoCreated);
    Console.WriteLine($"[PASS] TodoCreated record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}