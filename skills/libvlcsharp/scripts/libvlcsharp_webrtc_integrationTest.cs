#load "libvlcsharp_webrtc_integration.cs"

Console.WriteLine("=== libvlcsharp_webrtc_integration Test ===");

try
{
    var videoConferenceProcessorType = Type.GetType("VideoConferenceProcessor");
    Console.WriteLine(videoConferenceProcessorType != null ? "[PASS] VideoConferenceProcessor 类型存在" : "[FAIL] VideoConferenceProcessor 类型未找到");

    if (videoConferenceProcessorType != null)
    {
        Console.WriteLine(videoConferenceProcessorType.GetMethod("StartConference") != null ? "[PASS] VideoConferenceProcessor.StartConference 方法存在" : "[FAIL] VideoConferenceProcessor.StartConference 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}