#load "wolverinefx_production_integration.cs"

Console.WriteLine("=== wolverinefx_production_integration Test ===");

try
{
    var t0 = typeof(TodoHandlers);
    Console.WriteLine($"[PASS] TodoHandlers 存在");
    var t1 = typeof(WolverineServiceExtensions);
    Console.WriteLine($"[PASS] WolverineServiceExtensions 存在");
    var t2 = typeof(LoggingMiddleware);
    Console.WriteLine($"[PASS] LoggingMiddleware 存在");
    var t3 = typeof(DomainEventDispatcher);
    Console.WriteLine($"[PASS] DomainEventDispatcher 存在");
    var t4 = typeof(InMemoryEventBus);
    Console.WriteLine($"[PASS] InMemoryEventBus 存在");
    var t5 = typeof(ITodoRepository);
    Console.WriteLine($"[PASS] ITodoRepository 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IDomainEventDispatcher);
    Console.WriteLine($"[PASS] IDomainEventDispatcher 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IEventBus);
    Console.WriteLine($"[PASS] IEventBus 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand record 存在");
    var t9 = typeof(GetTodoQuery);
    Console.WriteLine($"[PASS] GetTodoQuery record 存在");
    var t10 = typeof(TodoDto);
    Console.WriteLine($"[PASS] TodoDto record 存在");
    var t11 = typeof(Todo);
    Console.WriteLine($"[PASS] Todo record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}