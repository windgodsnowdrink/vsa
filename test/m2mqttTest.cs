#load "m2mqtt.cs"

Console.WriteLine("=== m2mqtt Test ===");

try
{
    var t0 = typeof(MqttMessageProcessor);
    Console.WriteLine($"[PASS] MqttMessageProcessor 存在");
    var t1 = typeof(MqttClientService);
    Console.WriteLine($"[PASS] MqttClientService 存在");
    var t2 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}