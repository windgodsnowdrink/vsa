#load "cscore_audio_integration.cs"

Console.WriteLine("=== cscore_audio_integration Test ===");

try
{
    var t0 = typeof(AudioProcessor);
    Console.WriteLine($"[PASS] AudioProcessor 存在");
    var t1 = typeof(AudioPipelineConfig);
    Console.WriteLine($"[PASS] AudioPipelineConfig record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}