#load "libvlcsharp_blazor_integration.cs"

Console.WriteLine("=== libvlcsharp_blazor_integration Test ===");

try
{
    var blazorVideoStreamProcessorType = Type.GetType("BlazorVideoStreamProcessor");
    Console.WriteLine(blazorVideoStreamProcessorType != null ? "[PASS] BlazorVideoStreamProcessor 类型存在" : "[FAIL] BlazorVideoStreamProcessor 类型未找到");

    var videoPlayerType = Type.GetType("VideoPlayer");
    Console.WriteLine(videoPlayerType != null ? "[PASS] VideoPlayer 类型存在" : "[FAIL] VideoPlayer 类型未找到");

    if (blazorVideoStreamProcessorType != null)
    {
        Console.WriteLine(blazorVideoStreamProcessorType.GetMethod("ProcessStream") != null ? "[PASS] BlazorVideoStreamProcessor.ProcessStream 方法存在" : "[FAIL] BlazorVideoStreamProcessor.ProcessStream 方法未找到");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}