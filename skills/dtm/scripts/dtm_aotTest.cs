#load "dtm_aot.cs"

Console.WriteLine("=== dtm_aot Test ===");

try
{
    var t0 = typeof(DTM.AOT.DtmOptions);
    Console.WriteLine($"[PASS] DtmOptions 存在");
    var t1 = typeof(DTM.AOT.DtmCommandResult);
    Console.WriteLine($"[PASS] DtmCommandResult 存在");
    var t2 = typeof(DTM.AOT.DtmService);
    Console.WriteLine($"[PASS] DtmService 存在");
    var t3 = typeof(DTM.AOT.DtmAotEngine);
    Console.WriteLine($"[PASS] DtmAotEngine 存在");
    var t4 = typeof(DTM.AOT.DtmServiceExtensions);
    Console.WriteLine($"[PASS] DtmServiceExtensions 存在");
    var t5 = typeof(DTM.AOT.IDtmService);
    Console.WriteLine($"[PASS] IDtmService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(DTM.AOT.DtmCommandType);
    Console.WriteLine($"[PASS] DtmCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}