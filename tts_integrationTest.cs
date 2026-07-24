#load "tts_integration.cs"

Console.WriteLine("=== tts_integration Test ===");

try
{
    var t0 = typeof(TtsIntegration.TtsOptions);
    Console.WriteLine($"[PASS] TtsOptions 存在");
    var t1 = typeof(TtsIntegration.TtsProcessor);
    Console.WriteLine($"[PASS] TtsProcessor 存在");
    var t2 = typeof(TtsIntegration.TtsContext);
    Console.WriteLine($"[PASS] TtsContext 存在");
    var t3 = typeof(TtsIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}