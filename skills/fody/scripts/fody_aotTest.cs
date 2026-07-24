#load "fody_aot.cs"

Console.WriteLine("=== fody_aot Test ===");

try
{
    var t0 = typeof(Fody.AOT.FodyOptions);
    Console.WriteLine($"[PASS] FodyOptions 存在");
    var t1 = typeof(Fody.AOT.FodyCommandResult);
    Console.WriteLine($"[PASS] FodyCommandResult 存在");
    var t2 = typeof(Fody.AOT.FodyService);
    Console.WriteLine($"[PASS] FodyService 存在");
    var t3 = typeof(Fody.AOT.FodyAotEngine);
    Console.WriteLine($"[PASS] FodyAotEngine 存在");
    var t4 = typeof(Fody.AOT.IFodyService);
    Console.WriteLine($"[PASS] IFodyService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(Fody.AOT.FodyCommandType);
    Console.WriteLine($"[PASS] FodyCommandType enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}