#load "asr_integration.cs"

Console.WriteLine("=== asr_integration Test ===");

try
{
    var t0 = typeof(AsrIntegration.AsrProcessor);
    Console.WriteLine($"[PASS] AsrProcessor 存在");
    var t1 = typeof(AsrIntegration.AsrDiagnostics);
    Console.WriteLine($"[PASS] AsrDiagnostics 存在");
    var t2 = typeof(AsrIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(AsrIntegration.SpeechRecognizerPooledPolicy);
    Console.WriteLine($"[PASS] SpeechRecognizerPooledPolicy 存在");
    var t4 = typeof(AsrIntegration.MemoryStreamPooledPolicy);
    Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 存在");
    var t5 = typeof(AsrIntegration.IAudioProcessingPipeline);
    Console.WriteLine($"[PASS] IAudioProcessingPipeline 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(AsrIntegration.AsrOptions);
    Console.WriteLine($"[PASS] AsrOptions record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}