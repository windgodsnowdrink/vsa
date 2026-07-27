#load "channels_demo.cs"

Console.WriteLine("=== channels_demo Test ===");

try
{
    var t0 = typeof(ChannelProcessor);
    Console.WriteLine($"[PASS] ChannelProcessor 存在");
    var t1 = typeof(BatchProcessor);
    Console.WriteLine($"[PASS] BatchProcessor 存在");
    var t2 = typeof(TodoProcessor);
    Console.WriteLine($"[PASS] TodoProcessor 存在");
    var t3 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t4 = typeof(BackpressureChannel);
    Console.WriteLine($"[PASS] BackpressureChannel 存在");
    var t5 = typeof(ChannelMetrics);
    Console.WriteLine($"[PASS] ChannelMetrics 存在");
    var t6 = typeof(ResilientChannelProcessor);
    Console.WriteLine($"[PASS] ResilientChannelProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}