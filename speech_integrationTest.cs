#load "speech_integration.cs"

Console.WriteLine("=== speech_integration Test ===");

try
{
    var t0 = typeof(SpeechSynthesisEventHandler);
    Console.WriteLine($"[PASS] SpeechSynthesisEventHandler 存在");
    var t1 = typeof(AudioNoiseEchoProcessor);
    Console.WriteLine($"[PASS] AudioNoiseEchoProcessor 存在");
    var t2 = typeof(ChannelStrategyConfig);
    Console.WriteLine($"[PASS] ChannelStrategyConfig 存在");
    var t3 = typeof(OfflineAudioProcessor);
    Console.WriteLine($"[PASS] OfflineAudioProcessor 存在");
    var t4 = typeof(MultiScenarioRecognizer);
    Console.WriteLine($"[PASS] MultiScenarioRecognizer 存在");
    var t5 = typeof(MemoryPoolPolicy);
    Console.WriteLine($"[PASS] MemoryPoolPolicy 存在");
    var t6 = typeof(RecognizerPooledPolicy);
    Console.WriteLine($"[PASS] RecognizerPooledPolicy 存在");
    var t7 = typeof(SynthesizerPooledPolicy);
    Console.WriteLine($"[PASS] SynthesizerPooledPolicy 存在");
    var t8 = typeof(FftNoiseSuppressor);
    Console.WriteLine($"[PASS] FftNoiseSuppressor 存在");
    var t9 = typeof(SpeechRecognitionService);
    Console.WriteLine($"[PASS] SpeechRecognitionService 存在");
    var t10 = typeof(ICustomAudioSource);
    Console.WriteLine($"[PASS] ICustomAudioSource 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(IAudioProcessingPipeline);
    Console.WriteLine($"[PASS] IAudioProcessingPipeline 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(struct);
    Console.WriteLine($"[PASS] struct record 存在");
    var t13 = typeof(AudioFrame);
    Console.WriteLine($"[PASS] AudioFrame struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}