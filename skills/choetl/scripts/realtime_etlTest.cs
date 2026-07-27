#load "realtime_etl.cs"

Console.WriteLine("=== realtime_etl Test ===");

try
{
    var t0 = typeof(RealtimeEtlProcessor);
    Console.WriteLine($"[PASS] RealtimeEtlProcessor 存在");
    var t1 = typeof(ParsedData);
    Console.WriteLine($"[PASS] ParsedData record 存在");
    var t2 = typeof(TransformedData);
    Console.WriteLine($"[PASS] TransformedData record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}