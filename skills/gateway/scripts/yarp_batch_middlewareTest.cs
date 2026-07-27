#load "yarp_batch_middleware.cs"

Console.WriteLine("=== yarp_batch_middleware Test ===");

try
{
    var t0 = typeof(BatchMiddleware);
    Console.WriteLine($"[PASS] BatchMiddleware 存在");
    var t1 = typeof(BatchOptions);
    Console.WriteLine($"[PASS] BatchOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}