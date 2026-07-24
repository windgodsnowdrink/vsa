#load "read_write_split.cs"

Console.WriteLine("=== read_write_split Test ===");

try
{
    var t0 = typeof(ReadOnly);
    Console.WriteLine($"[PASS] ReadOnly 存在");
    var t1 = typeof(ReadWriteDbContext);
    Console.WriteLine($"[PASS] ReadWriteDbContext 存在");
    var t2 = typeof(ReadOnlyDbContext);
    Console.WriteLine($"[PASS] ReadOnlyDbContext 存在");
    var t3 = typeof(DapperReadOnlyRepository);
    Console.WriteLine($"[PASS] DapperReadOnlyRepository 存在");
    var t4 = typeof(IReadOnly);
    Console.WriteLine($"[PASS] IReadOnly 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}