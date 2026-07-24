#load "rtc_audio_processor.cs"

Console.WriteLine("=== rtc_audio_processor Test ===");

try
{
    var t0 = typeof(AudioStreamCache);
    Console.WriteLine($"[PASS] AudioStreamCache 存在");
    var t1 = typeof(WasmAudioProcessor);
    Console.WriteLine($"[PASS] WasmAudioProcessor 存在");
    var t2 = typeof(EnhancedAudioProcessor);
    Console.WriteLine($"[PASS] EnhancedAudioProcessor 存在");
    var t3 = typeof(AudioHub);
    Console.WriteLine($"[PASS] AudioHub 存在");
    var t4 = typeof(struct);
    Console.WriteLine($"[PASS] struct record 存在");
    var t5 = typeof(AudioFrame);
    Console.WriteLine($"[PASS] AudioFrame struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}