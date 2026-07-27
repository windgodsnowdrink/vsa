#load "libvlcsharp_live_streaming.cs"

Console.WriteLine("=== libvlcsharp_live_streaming Test ===");

try
{
    var t0 = typeof(LiveStreamProcessor);
    Console.WriteLine($"[PASS] LiveStreamProcessor 存在");
    var t1 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t2 = typeof(MediaPlayerPooledPolicy);
    Console.WriteLine($"[PASS] MediaPlayerPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}