#load "sqlite_connection_pool.cs"

Console.WriteLine("=== sqlite_connection_pool Test ===");

try
{
    var t0 = typeof(SqliteConnectionPool);
    Console.WriteLine($"[PASS] SqliteConnectionPool 存在");
    var t1 = typeof(SqliteConnectionPoolPolicy);
    Console.WriteLine($"[PASS] SqliteConnectionPoolPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}