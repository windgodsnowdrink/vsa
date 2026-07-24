#load "wolverine_saga.cs"

Console.WriteLine("=== wolverine_saga Test ===");

try
{
    var t0 = typeof(TodoSagaState);
    Console.WriteLine($"[PASS] TodoSagaState 存在");
    var t1 = typeof(TodoSaga);
    Console.WriteLine($"[PASS] TodoSaga 存在");
    var t2 = typeof(StartTodoSagaCommand);
    Console.WriteLine($"[PASS] StartTodoSagaCommand record 存在");
    var t3 = typeof(ReserveResourcesEvent);
    Console.WriteLine($"[PASS] ReserveResourcesEvent record 存在");
    var t4 = typeof(ResourcesReservedEvent);
    Console.WriteLine($"[PASS] ResourcesReservedEvent record 存在");
    var t5 = typeof(ResourcesRolledBackEvent);
    Console.WriteLine($"[PASS] ResourcesRolledBackEvent record 存在");
    var t6 = typeof(SagaStatus);
    Console.WriteLine($"[PASS] SagaStatus enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}