#load "ffmpeg_integration.cs"

Console.WriteLine("=== ffmpeg_integration Test ===");

try
{
    var t0 = typeof(DistributedStreamProcessor);
    Console.WriteLine($"[PASS] DistributedStreamProcessor 存在");
    var t1 = typeof(StreamMetrics);
    Console.WriteLine($"[PASS] StreamMetrics 存在");
    var t2 = typeof(AdaptiveBitrateController);
    Console.WriteLine($"[PASS] AdaptiveBitrateController 存在");
    var t3 = typeof(FailoverHandler);
    Console.WriteLine($"[PASS] FailoverHandler 存在");
    var t4 = typeof(BitrateAdjuster);
    Console.WriteLine($"[PASS] BitrateAdjuster 存在");
    var t5 = typeof(MainProcessor);
    Console.WriteLine($"[PASS] MainProcessor 存在");
    var t6 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t7 = typeof(MediaFrame);
    Console.WriteLine($"[PASS] MediaFrame record 存在");
    var t8 = typeof(ProcessedFrame);
    Console.WriteLine($"[PASS] ProcessedFrame record 存在");
    var t9 = typeof(BitrateAdjustment);
    Console.WriteLine($"[PASS] BitrateAdjustment record 存在");
    var t10 = typeof(NetworkCondition);
    Console.WriteLine($"[PASS] NetworkCondition record 存在");
    var t11 = typeof(FailoverEvent);
    Console.WriteLine($"[PASS] FailoverEvent record 存在");
    var t12 = typeof(struct);
    Console.WriteLine($"[PASS] struct record 存在");
    var t13 = typeof(NetworkMetrics);
    Console.WriteLine($"[PASS] NetworkMetrics struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}