#load "wolverine_cqrs.cs"

Console.WriteLine("=== wolverine_cqrs Test ===");

try
{
    var t0 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");
    var t2 = typeof(CreateTodoHandler);
    Console.WriteLine($"[PASS] CreateTodoHandler 存在");
    var t3 = typeof(GetTodoHandler);
    Console.WriteLine($"[PASS] GetTodoHandler 存在");
    var t4 = typeof(CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand record 存在");
    var t5 = typeof(CreateTodoResponse);
    Console.WriteLine($"[PASS] CreateTodoResponse record 存在");
    var t6 = typeof(GetTodoQuery);
    Console.WriteLine($"[PASS] GetTodoQuery record 存在");
    var t7 = typeof(GetTodoResponse);
    Console.WriteLine($"[PASS] GetTodoResponse record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}