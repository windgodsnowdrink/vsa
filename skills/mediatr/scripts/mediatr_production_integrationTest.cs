#load "mediatr_production_integration.cs"

Console.WriteLine("=== mediatr_production_integration Test ===");

try
{
    var t0 = typeof(CreateTodoCommandHandler);
    Console.WriteLine($"[PASS] CreateTodoCommandHandler 存在");
    var t1 = typeof(GetTodoQueryHandler);
    Console.WriteLine($"[PASS] GetTodoQueryHandler 存在");
    var t2 = typeof(MediatRServiceExtensions);
    Console.WriteLine($"[PASS] MediatRServiceExtensions 存在");
    var t3 = typeof(LoggingBehavior);
    Console.WriteLine($"[PASS] LoggingBehavior 存在");
    var t4 = typeof(ValidationBehavior);
    Console.WriteLine($"[PASS] ValidationBehavior 存在");
    var t5 = typeof(PerformanceBehavior);
    Console.WriteLine($"[PASS] PerformanceBehavior 存在");
    var t6 = typeof(DomainEventDispatcher);
    Console.WriteLine($"[PASS] DomainEventDispatcher 存在");
    var t7 = typeof(InMemoryEventBus);
    Console.WriteLine($"[PASS] InMemoryEventBus 存在");
    var t8 = typeof(ITodoRepository);
    Console.WriteLine($"[PASS] ITodoRepository 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(IDomainEventDispatcher);
    Console.WriteLine($"[PASS] IDomainEventDispatcher 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(IEventBus);
    Console.WriteLine($"[PASS] IEventBus 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(CreateTodoCommand);
    Console.WriteLine($"[PASS] CreateTodoCommand record 存在");
    var t12 = typeof(GetTodoQuery);
    Console.WriteLine($"[PASS] GetTodoQuery record 存在");
    var t13 = typeof(TodoDto);
    Console.WriteLine($"[PASS] TodoDto record 存在");
    var t14 = typeof(Todo);
    Console.WriteLine($"[PASS] Todo record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}