#load "accord_audio_integration.cs"

Console.WriteLine("=== accord_audio_integration Test ===");

try
{
    var t0 = typeof(AudioProcessor);
    Console.WriteLine($"[PASS] AudioProcessor 存在");
    var t1 = typeof(FFTAnalyzer);
    Console.WriteLine($"[PASS] FFTAnalyzer 存在");
    var t2 = typeof(VoiceprintExtractor);
    Console.WriteLine($"[PASS] VoiceprintExtractor 存在");
    var t3 = typeof(IAudioProcessingPipeline);
    Console.WriteLine($"[PASS] IAudioProcessingPipeline 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(FFTResult);
    Console.WriteLine($"[PASS] FFTResult record 存在");
    var t5 = typeof(Voiceprint);
    Console.WriteLine($"[PASS] Voiceprint record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}