#load "mqtt_enhanced.cs"

Console.WriteLine("=== mqtt_enhanced Test ===");

try
{
    var t0 = typeof(ChannelQosController);
    Console.WriteLine($"[PASS] ChannelQosController 存在");
    var t1 = typeof(EnhancedMqttProcessor);
    Console.WriteLine($"[PASS] EnhancedMqttProcessor 存在");
    var t2 = typeof(QosMessage);
    Console.WriteLine($"[PASS] QosMessage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}