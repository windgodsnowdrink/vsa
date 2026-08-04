#load "audio_enhancement.cs"

Console.WriteLine("=== audio_enhancement Test ===");

try
{
    var t0 = typeof(AudioPipeline);
    Console.WriteLine($"[PASS] AudioPipeline 存在");
    var t1 = typeof(WaveSourcePooledPolicy);
    Console.WriteLine($"[PASS] WaveSourcePooledPolicy 存在");
    var t2 = typeof(WasapiOutPooledPolicy);
    Console.WriteLine($"[PASS] WasapiOutPooledPolicy 存在");
    var t3 = typeof(AudioPipelineConfig);
    Console.WriteLine($"[PASS] AudioPipelineConfig record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}