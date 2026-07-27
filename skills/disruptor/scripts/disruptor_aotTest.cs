#load "disruptor_aot.cs"

Console.WriteLine("=== disruptor_aot Test ===");

try
{
    var t0 = typeof(Disruptor.AOT.DisruptorEvent);
    Console.WriteLine($"[PASS] DisruptorEvent 存在");
    var t1 = typeof(Disruptor.AOT.DisruptorOptions);
    Console.WriteLine($"[PASS] DisruptorOptions 存在");
    var t2 = typeof(Disruptor.AOT.DisruptorResult);
    Console.WriteLine($"[PASS] DisruptorResult 存在");
    var t3 = typeof(Disruptor.AOT.DisruptorStatus);
    Console.WriteLine($"[PASS] DisruptorStatus 存在");
    var t4 = typeof(Disruptor.AOT.DisruptorService);
    Console.WriteLine($"[PASS] DisruptorService 存在");
    var t5 = typeof(Disruptor.AOT.DisruptorAotEngine);
    Console.WriteLine($"[PASS] DisruptorAotEngine 存在");
    var t6 = typeof(Disruptor.AOT.DisruptorExtensions);
    Console.WriteLine($"[PASS] DisruptorExtensions 存在");
    var t7 = typeof(Disruptor.AOT.IDisruptorService);
    Console.WriteLine($"[PASS] IDisruptorService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(Disruptor.AOT.DisruptorEventType);
    Console.WriteLine($"[PASS] DisruptorEventType enum 存在 (IsEnum: {t8.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}