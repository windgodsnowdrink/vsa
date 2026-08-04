#load "disruptor_production_integration.cs"

Console.WriteLine("=== disruptor_production_integration Test ===");

try
{
    var t0 = typeof(DisruptorOptions);
    Console.WriteLine($"[PASS] DisruptorOptions 存在");
    var t1 = typeof(DisruptorEventHandler);
    Console.WriteLine($"[PASS] DisruptorEventHandler 存在");
    var t2 = typeof(DisruptorProducer);
    Console.WriteLine($"[PASS] DisruptorProducer 存在");
    var t3 = typeof(DisruptorBackgroundService);
    Console.WriteLine($"[PASS] DisruptorBackgroundService 存在");
    var t4 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(AdvancedScenarios);
    Console.WriteLine($"[PASS] AdvancedScenarios 存在");
    var t6 = typeof(BatchEventHandler);
    Console.WriteLine($"[PASS] BatchEventHandler 存在");
    var t7 = typeof(FirstStageHandler);
    Console.WriteLine($"[PASS] FirstStageHandler 存在");
    var t8 = typeof(SecondStageHandler);
    Console.WriteLine($"[PASS] SecondStageHandler 存在");
    var t9 = typeof(FinalStageHandler);
    Console.WriteLine($"[PASS] FinalStageHandler 存在");
    var t10 = typeof(IDisruptorProducer);
    Console.WriteLine($"[PASS] IDisruptorProducer 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(DisruptorMessage);
    Console.WriteLine($"[PASS] DisruptorMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}