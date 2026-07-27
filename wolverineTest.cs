#load "wolverine.cs"

Console.WriteLine("=== wolverine Test ===");

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
    var t4 = typeof(TodoEventHandler);
    Console.WriteLine($"[PASS] TodoEventHandler 存在");
    var t5 = typeof(MemoryObjectPool);
    Console.WriteLine($"[PASS] MemoryObjectPool 存在");
    var t6 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t7 = typeof(PoolingManager);
    Console.WriteLine($"[PASS] PoolingManager 存在");
    var t8 = typeof(TodoSagaState);
    Console.WriteLine($"[PASS] TodoSagaState 存在");
    var t9 = typeof(TodoSaga);
    Console.WriteLine($"[PASS] TodoSaga 存在");
    var t10 = typeof(ResourceService);
    Console.WriteLine($"[PASS] ResourceService 存在");
    var t11 = typeof(TodoService);
    Console.WriteLine($"[PASS] TodoService 存在");
    var t12 = typeof(NotificationService);
    Console.WriteLine($"[PASS] NotificationService 存在");
    var t13 = typeof(CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand record 存在");
    var t14 = typeof(CreateTodoResponse);
    Console.WriteLine($"[PASS] CreateTodoResponse record 存在");
    var t15 = typeof(GetTodoQuery);
    Console.WriteLine($"[PASS] GetTodoQuery record 存在");
    var t16 = typeof(GetTodoResponse);
    Console.WriteLine($"[PASS] GetTodoResponse record 存在");
    var t17 = typeof(TodoCreated);
    Console.WriteLine($"[PASS] TodoCreated record 存在");
    var t18 = typeof(TodoCompleted);
    Console.WriteLine($"[PASS] TodoCompleted record 存在");
    var t19 = typeof(StartTodoSagaCommand);
    Console.WriteLine($"[PASS] StartTodoSagaCommand record 存在");
    var t20 = typeof(ReserveResourcesEvent);
    Console.WriteLine($"[PASS] ReserveResourcesEvent record 存在");
    var t21 = typeof(ResourcesReservedEvent);
    Console.WriteLine($"[PASS] ResourcesReservedEvent record 存在");
    var t22 = typeof(CreateTodoItemEvent);
    Console.WriteLine($"[PASS] CreateTodoItemEvent record 存在");
    var t23 = typeof(TodoItemCreatedEvent);
    Console.WriteLine($"[PASS] TodoItemCreatedEvent record 存在");
    var t24 = typeof(NotifyUserEvent);
    Console.WriteLine($"[PASS] NotifyUserEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}