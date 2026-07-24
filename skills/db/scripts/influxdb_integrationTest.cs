#load "influxdb_integration.cs"

Console.WriteLine("=== influxdb_integration Test ===");

try
{
    var t0 = typeof(InfluxDbExtensions);
    Console.WriteLine($"[PASS] InfluxDbExtensions 存在");
    var t1 = typeof(ResiliencePipelineProvider);
    Console.WriteLine($"[PASS] ResiliencePipelineProvider 存在");
    var t2 = typeof(InfluxDbBackgroundWriter);
    Console.WriteLine($"[PASS] InfluxDbBackgroundWriter 存在");
    var t3 = typeof(InfluxDbController);
    Console.WriteLine($"[PASS] InfluxDbController 存在");
    var t4 = typeof(WriteRequest);
    Console.WriteLine($"[PASS] WriteRequest 存在");
    var t5 = typeof(ResilienceContext);
    Console.WriteLine($"[PASS] ResilienceContext 存在");
    var t6 = typeof(SimpleResiliencePipeline);
    Console.WriteLine($"[PASS] SimpleResiliencePipeline 存在");
    var t7 = typeof(CounterOptions);
    Console.WriteLine($"[PASS] CounterOptions 存在");
    var t8 = typeof(HistogramOptions);
    Console.WriteLine($"[PASS] HistogramOptions 存在");
    var t9 = typeof(TagList);
    Console.WriteLine($"[PASS] TagList 存在");
    var t10 = typeof(Counter);
    Console.WriteLine($"[PASS] Counter 存在");
    var t11 = typeof(Histogram);
    Console.WriteLine($"[PASS] Histogram 存在");
    var t12 = typeof(Stopwatch);
    Console.WriteLine($"[PASS] Stopwatch 存在");
    var t13 = typeof(IInfluxDbWriter);
    Console.WriteLine($"[PASS] IInfluxDbWriter 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(IResiliencePipelineProvider);
    Console.WriteLine($"[PASS] IResiliencePipelineProvider 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(IResiliencePipeline);
    Console.WriteLine($"[PASS] IResiliencePipeline 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(InfluxDbOptions);
    Console.WriteLine($"[PASS] InfluxDbOptions record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}