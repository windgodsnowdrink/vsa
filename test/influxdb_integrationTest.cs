#load "influxdb_integration.cs"

Console.WriteLine("=== influxdb_integration Test ===");

try
{
    var t0 = typeof(InfluxDbExtensions);
    Console.WriteLine($"[PASS] InfluxDbExtensions 存在");
    var t1 = typeof(InfluxDbBackgroundWriter);
    Console.WriteLine($"[PASS] InfluxDbBackgroundWriter 存在");
    var t2 = typeof(InfluxDbQueryService);
    Console.WriteLine($"[PASS] InfluxDbQueryService 存在");
    var t3 = typeof(InfluxDbOptions);
    Console.WriteLine($"[PASS] InfluxDbOptions record 存在");
    var t4 = typeof(in);
    Console.WriteLine($"[PASS] in record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}