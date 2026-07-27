#load "litedb_eventbus.cs"

Console.WriteLine("=== litedb_eventbus Test ===");

try
{
    var t0 = typeof(EventBusService);
    Console.WriteLine($"[PASS] EventBusService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}