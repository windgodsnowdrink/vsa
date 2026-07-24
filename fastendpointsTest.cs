#load "fastendpoints.cs"

Console.WriteLine("=== fastendpoints Test ===");

try
{
    var t0 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");
    var t2 = typeof(CreateTodoRequest);
    Console.WriteLine($"[PASS] CreateTodoRequest 存在");
    var t3 = typeof(CreateTodoResponse);
    Console.WriteLine($"[PASS] CreateTodoResponse 存在");
    var t4 = typeof(CreateTodoEndpoint);
    Console.WriteLine($"[PASS] CreateTodoEndpoint 存在");
    var t5 = typeof(GetTodoEndpoint);
    Console.WriteLine($"[PASS] GetTodoEndpoint 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}