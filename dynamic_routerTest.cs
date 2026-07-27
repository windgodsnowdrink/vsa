#load "dynamic_router.cs"

Console.WriteLine("=== dynamic_router Test ===");

try
{
    var t0 = typeof(DynamicMessageRouter);
    Console.WriteLine($"[PASS] DynamicMessageRouter 存在");
    var t1 = typeof(DynamicRouter);
    Console.WriteLine($"[PASS] DynamicRouter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}