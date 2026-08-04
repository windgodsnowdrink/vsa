#load "SourceGenerator_mqtt_disruptor_impl.cs"

Console.WriteLine("=== SourceGenerator_mqtt_disruptor_impl Test ===");

try
{
    var t0 = typeof(MqttMessageEvent);
    Console.WriteLine($"[PASS] MqttMessageEvent 存在");
    var t1 = typeof(MqttMessageEventHandler);
    Console.WriteLine($"[PASS] MqttMessageEventHandler 存在");
    var t2 = typeof(HighPerformanceMqttService);
    Console.WriteLine($"[PASS] HighPerformanceMqttService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}