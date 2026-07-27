#load "wolverine_integration.cs"

Console.WriteLine("=== wolverine_integration Test ===");

try
{
    // 验证 TodoHandlers 类
    var handlersType = typeof(TodoHandlers);
    Console.WriteLine($"[PASS] TodoHandlers 类型存在: {handlersType.Name}");

    // 验证 TodoEndpoints 类
    var endpointsType = typeof(TodoEndpoints);
    Console.WriteLine($"[PASS] TodoEndpoints 类型存在: {endpointsType.Name}");

    // 验证 CreateTodo record
    var createTodoType = typeof(CreateTodo);
    Console.WriteLine($"[PASS] CreateTodo 类型存在: {createTodoType.Name}");

    // 验证 TodoCreated record
    var todoCreatedType = typeof(TodoCreated);
    Console.WriteLine($"[PASS] TodoCreated 类型存在: {todoCreatedType.Name}");

    // 验证 Todo 类
    var todoType = typeof(Todo);
    Console.WriteLine($"[PASS] Todo 类型存在: {todoType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}