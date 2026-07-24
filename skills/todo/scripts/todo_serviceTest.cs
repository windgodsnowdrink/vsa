#load "todo_service.cs"

Console.WriteLine("=== todo_service Test ===");

try
{
    var t0 = typeof(VSA.TodoSystem.Core.TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(VSA.TodoSystem.Core.CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand 存在");
    var t2 = typeof(VSA.TodoSystem.Core.UpdateTodoCommand);
    Console.WriteLine($"[PASS] UpdateTodoCommand 存在");
    var t3 = typeof(VSA.TodoSystem.Core.TodoQuery);
    Console.WriteLine($"[PASS] TodoQuery 存在");
    var t4 = typeof(VSA.TodoSystem.Core.PagedResult);
    Console.WriteLine($"[PASS] PagedResult 存在");
    var t5 = typeof(VSA.TodoSystem.Core.TodoStatistics);
    Console.WriteLine($"[PASS] TodoStatistics 存在");
    var t6 = typeof(VSA.TodoSystem.Core.TodoService);
    Console.WriteLine($"[PASS] TodoService 存在");
    var t7 = typeof(VSA.TodoSystem.Core.TodoCreatedEvent);
    Console.WriteLine($"[PASS] TodoCreatedEvent 存在");
    var t8 = typeof(VSA.TodoSystem.Core.TodoUpdatedEvent);
    Console.WriteLine($"[PASS] TodoUpdatedEvent 存在");
    var t9 = typeof(VSA.TodoSystem.Core.TodoDeletedEvent);
    Console.WriteLine($"[PASS] TodoDeletedEvent 存在");
    var t10 = typeof(VSA.TodoSystem.Core.TodoStatusChangedEvent);
    Console.WriteLine($"[PASS] TodoStatusChangedEvent 存在");
    var t11 = typeof(VSA.TodoSystem.Core.TodoPriorityChangedEvent);
    Console.WriteLine($"[PASS] TodoPriorityChangedEvent 存在");
    var t12 = typeof(VSA.TodoSystem.Core.TodoDependencyAddedEvent);
    Console.WriteLine($"[PASS] TodoDependencyAddedEvent 存在");
    var t13 = typeof(VSA.TodoSystem.Core.TodoDependencyRemovedEvent);
    Console.WriteLine($"[PASS] TodoDependencyRemovedEvent 存在");
    var t14 = typeof(VSA.TodoSystem.Core.ITodoService);
    Console.WriteLine($"[PASS] ITodoService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(VSA.TodoSystem.Core.IPriorityCalculator);
    Console.WriteLine($"[PASS] IPriorityCalculator 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(VSA.TodoSystem.Core.IEventBus);
    Console.WriteLine($"[PASS] IEventBus 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(VSA.TodoSystem.Core.Priority);
    Console.WriteLine($"[PASS] Priority enum 存在 (IsEnum: {t17.IsEnum})");
    var t18 = typeof(VSA.TodoSystem.Core.TaskStatus);
    Console.WriteLine($"[PASS] TaskStatus enum 存在 (IsEnum: {t18.IsEnum})");
    var t19 = typeof(VSA.TodoSystem.Core.TaskCategory);
    Console.WriteLine($"[PASS] TaskCategory enum 存在 (IsEnum: {t19.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}