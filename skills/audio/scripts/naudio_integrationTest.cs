#load "naudio_integration.cs"

Console.WriteLine("=== naudio_integration Test ===");

try
{
    var t0 = typeof(AudioProcessingPipeline);
    Console.WriteLine($"[PASS] AudioProcessingPipeline 存在");
    var t1 = typeof(AudioProcessor);
    Console.WriteLine($"[PASS] AudioProcessor 存在");
    var t2 = typeof(WaveStreamPooledPolicy);
    Console.WriteLine($"[PASS] WaveStreamPooledPolicy 存在");
    var t3 = typeof(SpeechRecognitionService);
    Console.WriteLine($"[PASS] SpeechRecognitionService 存在");
    var t4 = typeof(IAudioAnalyzer);
    Console.WriteLine($"[PASS] IAudioAnalyzer 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(struct);
    Console.WriteLine($"[PASS] struct record 存在");
    var t6 = typeof(FFTResult);
    Console.WriteLine($"[PASS] FFTResult record 存在");
    var t7 = typeof(AudioFrame);
    Console.WriteLine($"[PASS] AudioFrame struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}