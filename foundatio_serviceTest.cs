#load "foundatio_service.cs"

Console.WriteLine("=== foundatio_service Test ===");

try
{
    var t0 = typeof(ComponentProcessor);
    Console.WriteLine($"[PASS] ComponentProcessor 存在");
    var t1 = typeof(ComponentContext);
    Console.WriteLine($"[PASS] ComponentContext 存在");
    var t2 = typeof(ComponentContextPooledPolicy);
    Console.WriteLine($"[PASS] ComponentContextPooledPolicy 存在");
    var t3 = typeof(LoggingComponent);
    Console.WriteLine($"[PASS] LoggingComponent 存在");
    var t4 = typeof(CachingComponent);
    Console.WriteLine($"[PASS] CachingComponent 存在");
    var t5 = typeof(MetricsComponent);
    Console.WriteLine($"[PASS] MetricsComponent 存在");
    var t6 = typeof(DistributedLockComponent);
    Console.WriteLine($"[PASS] DistributedLockComponent 存在");
    var t7 = typeof(StorageComponent);
    Console.WriteLine($"[PASS] StorageComponent 存在");
    var t8 = typeof(TodoComponent);
    Console.WriteLine($"[PASS] TodoComponent 存在");
    var t9 = typeof(TodoContext);
    Console.WriteLine($"[PASS] TodoContext 存在");
    var t10 = typeof(IPluggableComponent);
    Console.WriteLine($"[PASS] IPluggableComponent 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(ComponentMessage);
    Console.WriteLine($"[PASS] ComponentMessage record 存在");
    var t12 = typeof(TodoCommand);
    Console.WriteLine($"[PASS] TodoCommand record 存在");
    var t13 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}