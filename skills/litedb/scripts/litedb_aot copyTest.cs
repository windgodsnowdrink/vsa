#load "litedb_aot copy.cs"

Console.WriteLine("=== litedb_aot copy Test ===");

try
{
    var t0 = typeof(LiteDBAot.LiteDBService);
    Console.WriteLine($"[PASS] LiteDBService 存在");
    var t1 = typeof(LiteDBAot.ILiteDBService);
    Console.WriteLine($"[PASS] ILiteDBService 接口存在 (IsInterface: {t1.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}