#load "csharpflink_aot.cs"

Console.WriteLine("=== csharpflink_aot Test ===");

try
{
    var t0 = typeof(CsharpFlink.AOT.CsharpFlinkOptions);
    Console.WriteLine($"[PASS] CsharpFlinkOptions 存在");
    var t1 = typeof(CsharpFlink.AOT.FlinkJobConfig);
    Console.WriteLine($"[PASS] FlinkJobConfig 存在");
    var t2 = typeof(CsharpFlink.AOT.CsharpFlinkResult);
    Console.WriteLine($"[PASS] CsharpFlinkResult 存在");
    var t3 = typeof(CsharpFlink.AOT.CsharpFlinkStatus);
    Console.WriteLine($"[PASS] CsharpFlinkStatus 存在");
    var t4 = typeof(CsharpFlink.AOT.CsharpFlinkService);
    Console.WriteLine($"[PASS] CsharpFlinkService 存在");
    var t5 = typeof(CsharpFlink.AOT.CsharpFlinkAotEngine);
    Console.WriteLine($"[PASS] CsharpFlinkAotEngine 存在");
    var t6 = typeof(CsharpFlink.AOT.ICsharpFlinkService);
    Console.WriteLine($"[PASS] ICsharpFlinkService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(CsharpFlink.AOT.JobStatus);
    Console.WriteLine($"[PASS] JobStatus enum 存在 (IsEnum: {t7.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}