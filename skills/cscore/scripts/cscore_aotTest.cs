#load "cscore_aot.cs"

Console.WriteLine("=== cscore_aot Test ===");

try
{
    var t0 = typeof(Cscore.AOT.CscoreOptions);
    Console.WriteLine($"[PASS] CscoreOptions 存在");
    var t1 = typeof(Cscore.AOT.AudioProcessingOptions);
    Console.WriteLine($"[PASS] AudioProcessingOptions 存在");
    var t2 = typeof(Cscore.AOT.CscoreResult);
    Console.WriteLine($"[PASS] CscoreResult 存在");
    var t3 = typeof(Cscore.AOT.AudioSizeInfo);
    Console.WriteLine($"[PASS] AudioSizeInfo 存在");
    var t4 = typeof(Cscore.AOT.AudioDevice);
    Console.WriteLine($"[PASS] AudioDevice 存在");
    var t5 = typeof(Cscore.AOT.CscoreStatus);
    Console.WriteLine($"[PASS] CscoreStatus 存在");
    var t6 = typeof(Cscore.AOT.CscoreService);
    Console.WriteLine($"[PASS] CscoreService 存在");
    var t7 = typeof(Cscore.AOT.CscoreAotEngine);
    Console.WriteLine($"[PASS] CscoreAotEngine 存在");
    var t8 = typeof(Cscore.AOT.ICscoreService);
    Console.WriteLine($"[PASS] ICscoreService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(Cscore.AOT.DeviceType);
    Console.WriteLine($"[PASS] DeviceType enum 存在 (IsEnum: {t9.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}