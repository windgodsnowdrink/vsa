#load "wolverine_cqrs.cs"

Console.WriteLine("=== wolverine_cqrs Test ===");

try
{
    // 验证 TodoItem 类
    var todoItemType = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 类型存在: {todoItemType.Name}");

    // 验证 CreateTodoCommand record
    var createCommandType = typeof(CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand 类型存在: {createCommandType.Name}");

    // 验证 CreateTodoHandler 类
    var createHandlerType = typeof(CreateTodoHandler);
    Console.WriteLine($"[PASS] CreateTodoHandler 类型存在: {createHandlerType.Name}");

    // 验证 GetTodoHandler 类
    var getHandlerType = typeof(GetTodoHandler);
    Console.WriteLine($"[PASS] GetTodoHandler 类型存在: {getHandlerType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}