#load "idgenerator_service.cs"

Console.WriteLine("=== idgenerator_service Test ===");

try
{
    var t0 = typeof(IdGeneratorService);
    Console.WriteLine($"[PASS] IdGeneratorService 存在");
    var t1 = typeof(IdRequest);
    Console.WriteLine($"[PASS] IdRequest 存在");
    var t2 = typeof(IdContext);
    Console.WriteLine($"[PASS] IdContext 存在");
    var t3 = typeof(IdContextPooledPolicy);
    Console.WriteLine($"[PASS] IdContextPooledPolicy 存在");
    var t4 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}