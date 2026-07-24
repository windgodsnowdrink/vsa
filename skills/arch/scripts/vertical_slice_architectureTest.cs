#load "vertical_slice_architecture.cs"

Console.WriteLine("=== vertical_slice_architecture Test ===");

try
{
    var t0 = typeof(VerticalSliceArchitecture.TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(VerticalSliceArchitecture.TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");
    var t2 = typeof(VerticalSliceArchitecture.TodoItemCreatedEvent);
    Console.WriteLine($"[PASS] TodoItemCreatedEvent 存在");
    var t3 = typeof(VerticalSliceArchitecture.TodoItemUpdatedEvent);
    Console.WriteLine($"[PASS] TodoItemUpdatedEvent 存在");
    var t4 = typeof(VerticalSliceArchitecture.TodoItemDeletedEvent);
    Console.WriteLine($"[PASS] TodoItemDeletedEvent 存在");
    var t5 = typeof(VerticalSliceArchitecture.TodoEndpoints);
    Console.WriteLine($"[PASS] TodoEndpoints 存在");
    var t6 = typeof(VerticalSliceArchitecture.MessageBus);
    Console.WriteLine($"[PASS] MessageBus 存在");
    var t7 = typeof(VerticalSliceArchitecture.IMessageBus);
    Console.WriteLine($"[PASS] IMessageBus 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}