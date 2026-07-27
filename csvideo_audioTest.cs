#load "csvideo_audio.cs"

Console.WriteLine("=== csvideo_audio Test ===");

try
{
    var t0 = typeof(AudioProcessor);
    Console.WriteLine($"[PASS] AudioProcessor 存在");
    var t1 = typeof(FFTAnalyzer);
    Console.WriteLine($"[PASS] FFTAnalyzer 存在");
    var t2 = typeof(WaveSourcePooledPolicy);
    Console.WriteLine($"[PASS] WaveSourcePooledPolicy 存在");
    var t3 = typeof(struct);
    Console.WriteLine($"[PASS] struct record 存在");
    var t4 = typeof(AudioFrame);
    Console.WriteLine($"[PASS] AudioFrame struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}