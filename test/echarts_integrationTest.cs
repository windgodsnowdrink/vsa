#load "echarts_integration.cs"

Console.WriteLine("=== echarts_integration Test ===");

try
{
    var t0 = typeof(EChartsExtensions);
    Console.WriteLine($"[PASS] EChartsExtensions 存在");
    var t1 = typeof(EChartsOptions);
    Console.WriteLine($"[PASS] EChartsOptions 存在");
    var t2 = typeof(EChartsInstancePooledObjectPolicy);
    Console.WriteLine($"[PASS] EChartsInstancePooledObjectPolicy 存在");
    var t3 = typeof(EChartsExamples);
    Console.WriteLine($"[PASS] EChartsExamples 存在");
    var t4 = typeof(EChartsRenderService);
    Console.WriteLine($"[PASS] EChartsRenderService 存在");
    var t5 = typeof(DynamicDataService);
    Console.WriteLine($"[PASS] DynamicDataService 存在");
    var t6 = typeof(EChartsRenderJob);
    Console.WriteLine($"[PASS] EChartsRenderJob record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}