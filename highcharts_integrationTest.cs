#load "highcharts_integration.cs"

Console.WriteLine("=== highcharts_integration Test ===");

try
{
    var t0 = typeof(HighchartsIntegration.HighchartsOptions);
    Console.WriteLine($"[PASS] HighchartsOptions 存在");
    var t1 = typeof(HighchartsIntegration.HighchartsInstancePooledObjectPolicy);
    Console.WriteLine($"[PASS] HighchartsInstancePooledObjectPolicy 存在");
    var t2 = typeof(HighchartsIntegration.HighchartsRenderService);
    Console.WriteLine($"[PASS] HighchartsRenderService 存在");
    var t3 = typeof(HighchartsIntegration.HighchartsServiceExtensions);
    Console.WriteLine($"[PASS] HighchartsServiceExtensions 存在");
    var t4 = typeof(HighchartsIntegration.HighchartsExamples);
    Console.WriteLine($"[PASS] HighchartsExamples 存在");
    var t5 = typeof(HighchartsIntegration.HighchartsRenderJob);
    Console.WriteLine($"[PASS] HighchartsRenderJob record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}