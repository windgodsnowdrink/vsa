#load "libvlcsharp_integration.cs"

Console.WriteLine("=== libvlcsharp_integration Test ===");

try
{
    var t0 = typeof(VideoStreamProcessor);
    Console.WriteLine($"[PASS] VideoStreamProcessor 存在");
    var t1 = typeof(AudioStreamProcessor);
    Console.WriteLine($"[PASS] AudioStreamProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}