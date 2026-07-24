#load "litedb_eventhandlers.cs"

Console.WriteLine("=== litedb_eventhandlers Test ===");

try
{
    var t0 = typeof(EventHandlerPool);
    Console.WriteLine($"[PASS] EventHandlerPool 存在");
    var t1 = typeof(EventHandlerPoolPolicy);
    Console.WriteLine($"[PASS] EventHandlerPoolPolicy 存在");
    var t2 = typeof(DefaultEventHandler);
    Console.WriteLine($"[PASS] DefaultEventHandler 存在");
    var t3 = typeof(IEventHandler);
    Console.WriteLine($"[PASS] IEventHandler 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}