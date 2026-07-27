#load "mqtt_protocol_integration.cs"

Console.WriteLine("=== mqtt_protocol_integration Test ===");

try
{
    var t0 = typeof(MqttProtocolAdapter);
    Console.WriteLine($"[PASS] MqttProtocolAdapter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}