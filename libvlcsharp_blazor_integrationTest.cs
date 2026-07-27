#load "libvlcsharp_blazor_integration.cs"

Console.WriteLine("=== libvlcsharp_blazor_integration Test ===");

try
{
    var t0 = typeof(BlazorVideoStreamProcessor);
    Console.WriteLine($"[PASS] BlazorVideoStreamProcessor 存在");
    var t1 = typeof(VideoPlayer);
    Console.WriteLine($"[PASS] VideoPlayer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}