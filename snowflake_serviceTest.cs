#load "snowflake_service.cs"

Console.WriteLine("=== snowflake_service Test ===");

try
{
    var t0 = typeof(SnowflakeGenerator);
    Console.WriteLine($"[PASS] SnowflakeGenerator 存在");
    var t1 = typeof(SnowflakeService);
    Console.WriteLine($"[PASS] SnowflakeService 存在");
    var t2 = typeof(IdRequest);
    Console.WriteLine($"[PASS] IdRequest 存在");
    var t3 = typeof(IdContext);
    Console.WriteLine($"[PASS] IdContext 存在");
    var t4 = typeof(IdContextPooledPolicy);
    Console.WriteLine($"[PASS] IdContextPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}