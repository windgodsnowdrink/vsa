#load "libvlcsharp_live_streaming.cs"

Console.WriteLine("=== libvlcsharp_live_streaming Test ===");

try
{
    var liveStreamProcessorType = Type.GetType("LiveStreamProcessor");
    Console.WriteLine(liveStreamProcessorType != null ? "[PASS] LiveStreamProcessor 类型存在" : "[FAIL] LiveStreamProcessor 类型未找到");

    var tailLatencyOptimizerType = Type.GetType("TailLatencyOptimizer");
    Console.WriteLine(tailLatencyOptimizerType != null ? "[PASS] TailLatencyOptimizer 类型存在" : "[FAIL] TailLatencyOptimizer 类型未找到");

    var mediaPlayerPooledPolicyType = Type.GetType("MediaPlayerPooledPolicy");
    Console.WriteLine(mediaPlayerPooledPolicyType != null ? "[PASS] MediaPlayerPooledPolicy 类型存在" : "[FAIL] MediaPlayerPooledPolicy 类型未找到");

    if (liveStreamProcessorType != null)
    {
        Console.WriteLine(liveStreamProcessorType.GetMethod("ProcessLiveStream") != null ? "[PASS] LiveStreamProcessor.ProcessLiveStream 方法存在" : "[FAIL] LiveStreamProcessor.ProcessLiveStream 方法未找到");
    }

    if (tailLatencyOptimizerType != null)
    {
        Console.WriteLine(tailLatencyOptimizerType.GetMethod("Optimize") != null ? "[PASS] TailLatencyOptimizer.Optimize 方法存在" : "[FAIL] TailLatencyOptimizer.Optimize 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}