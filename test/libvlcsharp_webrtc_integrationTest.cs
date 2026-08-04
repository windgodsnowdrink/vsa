#load "libvlcsharp_webrtc_integration.cs"

Console.WriteLine("=== libvlcsharp_webrtc_integration Test ===");

try
{
    var t0 = typeof(VideoConferenceProcessor);
    Console.WriteLine($"[PASS] VideoConferenceProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}