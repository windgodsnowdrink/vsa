#load "masstransit_litedb_integration.cs"

Console.WriteLine("=== masstransit_litedb_integration Test ===");

try
{
    var t0 = typeof(OrderStateMachine);
    Console.WriteLine($"[PASS] OrderStateMachine 存在");
    var t1 = typeof(OrderState);
    Console.WriteLine($"[PASS] OrderState 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}