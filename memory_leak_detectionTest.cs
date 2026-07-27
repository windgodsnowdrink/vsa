#load "memory_leak_detection.cs"

Console.WriteLine("=== memory_leak_detection Test ===");

try
{
    var t0 = typeof(ChannelMemoryLeakDetector);
    Console.WriteLine($"[PASS] ChannelMemoryLeakDetector 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}