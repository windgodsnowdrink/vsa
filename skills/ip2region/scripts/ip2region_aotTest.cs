#load "ip2region_aot.cs"

Console.WriteLine("=== ip2region_aot Test ===");

try
{
    var t0 = typeof(Ip2RegionSettings);
    Console.WriteLine($"[PASS] Ip2RegionSettings 存在");
    var t1 = typeof(Ip2RegionService);
    Console.WriteLine($"[PASS] Ip2RegionService 存在");
    var t2 = typeof(RegionInfo);
    Console.WriteLine($"[PASS] RegionInfo 存在");
    var t3 = typeof(BenchmarkResult);
    Console.WriteLine($"[PASS] BenchmarkResult 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}