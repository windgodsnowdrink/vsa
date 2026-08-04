#load "litedb_logging.cs"

Console.WriteLine("=== litedb_logging Test ===");

try
{
    var t0 = typeof(LogEntry);
    Console.WriteLine($"[PASS] LogEntry 存在");
    var t1 = typeof(LiteDbLogService);
    Console.WriteLine($"[PASS] LiteDbLogService 存在");
    var t2 = typeof(LiteDbPoolPolicy);
    Console.WriteLine($"[PASS] LiteDbPoolPolicy 存在");
    var t3 = typeof(LogQueryService);
    Console.WriteLine($"[PASS] LogQueryService 存在");
    var t4 = typeof(LiteDbLogDemo);
    Console.WriteLine($"[PASS] LiteDbLogDemo 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}