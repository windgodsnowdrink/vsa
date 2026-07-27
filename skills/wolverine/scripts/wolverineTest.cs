#load "wolverine.cs"

Console.WriteLine("=== wolverine Test ===");

try
{
    // 验证 TodoItem 类
    var todoItemType = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 类型存在: {todoItemType.Name}");

    // 验证 TodoDbContext 类
    var dbContextType = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 类型存在: {dbContextType.Name}");

    // 验证 CreateTodoCommand record
    var createCommandType = typeof(CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand 类型存在: {createCommandType.Name}");

    // 验证 CreateTodoHandler 类
    var createHandlerType = typeof(CreateTodoHandler);
    Console.WriteLine($"[PASS] CreateTodoHandler 类型存在: {createHandlerType.Name}");

    // 验证 TodoEventHandler 类
    var eventHandlerType = typeof(TodoEventHandler);
    Console.WriteLine($"[PASS] TodoEventHandler 类型存在: {eventHandlerType.Name}");

    // 验证 TodoSaga 类
    var sagaType = typeof(TodoSaga);
    Console.WriteLine($"[PASS] TodoSaga 类型存在: {sagaType.Name}");

    // 验证 TodoSagaState 类
    var sagaStateType = typeof(TodoSagaState);
    Console.WriteLine($"[PASS] TodoSagaState 类型存在: {sagaStateType.Name}");

    // 验证 ResourceService 类
    var resourceServiceType = typeof(ResourceService);
    Console.WriteLine($"[PASS] ResourceService 类型存在: {resourceServiceType.Name}");

    // 验证 MemoryObjectPool 类
    var poolType = typeof(MemoryObjectPool);
    Console.WriteLine($"[PASS] MemoryObjectPool 类型存在: {poolType.Name}");

    // 验证 TailLatencyOptimizer 类
    var optimizerType = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 类型存在: {optimizerType.Name}");

    // 验证 PoolingManager 类
    var managerType = typeof(PoolingManager);
    Console.WriteLine($"[PASS] PoolingManager 类型存在: {managerType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}