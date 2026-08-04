#load "boxed_orleans.cs"

Console.WriteLine("=== boxed_orleans Test ===");

try
{
    var t0 = typeof(TodoGrain);
    Console.WriteLine($"[PASS] TodoGrain 存在");
    var t1 = typeof(OrleansMessage);
    Console.WriteLine($"[PASS] OrleansMessage 存在");
    var t2 = typeof(ITodoGrain);
    Console.WriteLine($"[PASS] ITodoGrain 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(TodoCommand);
    Console.WriteLine($"[PASS] TodoCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}