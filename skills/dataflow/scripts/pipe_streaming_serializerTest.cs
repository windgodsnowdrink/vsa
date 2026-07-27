#load "pipe_streaming_serializer.cs"

Console.WriteLine("=== pipe_streaming_serializer Test ===");

try
{
    var t0 = typeof(PipeStreamingSerializer);
    Console.WriteLine($"[PASS] PipeStreamingSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}