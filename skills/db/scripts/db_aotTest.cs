#load "db_aot.cs"

Console.WriteLine("=== db_aot Test ===");

try
{
    var t0 = typeof(Db.AOT.DatabaseOptions);
    Console.WriteLine($"[PASS] DatabaseOptions 存在");
    var t1 = typeof(Db.AOT.DatabaseResult);
    Console.WriteLine($"[PASS] DatabaseResult 存在");
    var t2 = typeof(Db.AOT.DatabaseStatus);
    Console.WriteLine($"[PASS] DatabaseStatus 存在");
    var t3 = typeof(Db.AOT.DatabaseService);
    Console.WriteLine($"[PASS] DatabaseService 存在");
    var t4 = typeof(Db.AOT.DatabaseAotEngine);
    Console.WriteLine($"[PASS] DatabaseAotEngine 存在");
    var t5 = typeof(Db.AOT.DatabaseExtensions);
    Console.WriteLine($"[PASS] DatabaseExtensions 存在");
    var t6 = typeof(Db.AOT.IDatabaseService);
    Console.WriteLine($"[PASS] IDatabaseService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(Db.AOT.in);
    Console.WriteLine($"[PASS] in record 存在");
    var t8 = typeof(Db.AOT.DatabaseType);
    Console.WriteLine($"[PASS] DatabaseType enum 存在 (IsEnum: {t8.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}