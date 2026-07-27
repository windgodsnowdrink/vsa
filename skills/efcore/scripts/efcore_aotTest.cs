#load "efcore_aot.cs"

Console.WriteLine("=== efcore_aot Test ===");

try
{
    var t0 = typeof(EFCore.AOT.EFCoreOptions);
    Console.WriteLine($"[PASS] EFCoreOptions 存在");
    var t1 = typeof(EFCore.AOT.EFCoreCommandResult);
    Console.WriteLine($"[PASS] EFCoreCommandResult 存在");
    var t2 = typeof(EFCore.AOT.User);
    Console.WriteLine($"[PASS] User 存在");
    var t3 = typeof(EFCore.AOT.Order);
    Console.WriteLine($"[PASS] Order 存在");
    var t4 = typeof(EFCore.AOT.EFCoreAotContext);
    Console.WriteLine($"[PASS] EFCoreAotContext 存在");
    var t5 = typeof(EFCore.AOT.EFCoreService);
    Console.WriteLine($"[PASS] EFCoreService 存在");
    var t6 = typeof(EFCore.AOT.IEFCoreService);
    Console.WriteLine($"[PASS] IEFCoreService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(EFCore.AOT.EFCoreCommandType);
    Console.WriteLine($"[PASS] EFCoreCommandType enum 存在 (IsEnum: {t7.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}