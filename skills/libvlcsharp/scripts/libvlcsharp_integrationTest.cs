#load "libvlcsharp_integration.cs"

Console.WriteLine("=== libvlcsharp_integration Test ===");

try
{
    var videoStreamProcessorType = Type.GetType("VideoStreamProcessor");
    Console.WriteLine(videoStreamProcessorType != null ? "[PASS] VideoStreamProcessor 类型存在" : "[FAIL] VideoStreamProcessor 类型未找到");

    var audioStreamProcessorType = Type.GetType("AudioStreamProcessor");
    Console.WriteLine(audioStreamProcessorType != null ? "[PASS] AudioStreamProcessor 类型存在" : "[FAIL] AudioStreamProcessor 类型未找到");

    if (videoStreamProcessorType != null)
    {
        Console.WriteLine(videoStreamProcessorType.GetMethod("ProcessStream") != null ? "[PASS] VideoStreamProcessor.ProcessStream 方法存在" : "[FAIL] VideoStreamProcessor.ProcessStream 方法未找到");
    }

    if (audioStreamProcessorType != null)
    {
        Console.WriteLine(audioStreamProcessorType.GetMethod("ProcessAudio") != null ? "[PASS] AudioStreamProcessor.ProcessAudio 方法存在" : "[FAIL] AudioStreamProcessor.ProcessAudio 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}