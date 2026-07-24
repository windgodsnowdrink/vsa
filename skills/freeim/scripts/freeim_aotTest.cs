#load "freeim_aot.cs"

Console.WriteLine("=== freeim_aot Test ===");

try
{
    var t0 = typeof(FreeIM.AOT.FreeIMOptions);
    Console.WriteLine($"[PASS] FreeIMOptions 存在");
    var t1 = typeof(FreeIM.AOT.Message);
    Console.WriteLine($"[PASS] Message 存在");
    var t2 = typeof(FreeIM.AOT.User);
    Console.WriteLine($"[PASS] User 存在");
    var t3 = typeof(FreeIM.AOT.FreeIMCommandResult);
    Console.WriteLine($"[PASS] FreeIMCommandResult 存在");
    var t4 = typeof(FreeIM.AOT.FreeIMService);
    Console.WriteLine($"[PASS] FreeIMService 存在");
    var t5 = typeof(FreeIM.AOT.FreeIMAotEngine);
    Console.WriteLine($"[PASS] FreeIMAotEngine 存在");
    var t6 = typeof(FreeIM.AOT.IFreeIMService);
    Console.WriteLine($"[PASS] IFreeIMService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(FreeIM.AOT.FreeIMCommandType);
    Console.WriteLine($"[PASS] FreeIMCommandType enum 存在 (IsEnum: {t7.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}