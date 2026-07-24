#load "event_sourcing_processor.cs"

Console.WriteLine("=== event_sourcing_processor Test ===");

try
{
    var t0 = typeof(TieredEventProcessor);
    Console.WriteLine($"[PASS] TieredEventProcessor 存在");
    var t1 = typeof(EventWrapper);
    Console.WriteLine($"[PASS] EventWrapper record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}