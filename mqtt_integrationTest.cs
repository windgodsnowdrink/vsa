#load "mqtt_integration.cs"

Console.WriteLine("=== mqtt_integration Test ===");

try
{
    var t0 = typeof(ChannelMqttProcessor);
    Console.WriteLine($"[PASS] ChannelMqttProcessor 存在");
    var t1 = typeof(MqttMessage);
    Console.WriteLine($"[PASS] MqttMessage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}