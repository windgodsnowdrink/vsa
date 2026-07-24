#load "wolverine_event_bus.cs"

Console.WriteLine("=== wolverine_event_bus Test ===");

try
{
    // 验证 TodoCreated record
    var todoCreatedType = typeof(TodoCreated);
    Console.WriteLine($"[PASS] TodoCreated 类型存在: {todoCreatedType.Name}");

    // 验证 TodoCompleted record
    var todoCompletedType = typeof(TodoCompleted);
    Console.WriteLine($"[PASS] TodoCompleted 类型存在: {todoCompletedType.Name}");

    // 验证 TodoEventHandler 类
    var handlerType = typeof(TodoEventHandler);
    Console.WriteLine($"[PASS] TodoEventHandler 类型存在: {handlerType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}