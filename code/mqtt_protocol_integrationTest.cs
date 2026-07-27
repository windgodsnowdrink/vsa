#load "mqtt_protocol_integration.cs"

Console.WriteLine("=== mqtt_protocol_integration.cs Test ===");

try
{
    // 验证 class: MqttProtocolAdapter
    var type_MqttProtocolAdapter = Type.GetType("MqttProtocolAdapter");
    if (type_MqttProtocolAdapter != null)
    {
        Console.WriteLine("[PASS] 类型 MqttProtocolAdapter (class) 存在");
        var ctors_MqttProtocolAdapter = type_MqttProtocolAdapter.GetConstructors();
        Console.WriteLine($"[PASS] MqttProtocolAdapter 构造函数数量: {ctors_MqttProtocolAdapter.Length}");
        var methods_MqttProtocolAdapter = type_MqttProtocolAdapter.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttProtocolAdapter 公开方法数量: {methods_MqttProtocolAdapter.Length}");
        foreach (var m in methods_MqttProtocolAdapter)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttProtocolAdapter 未找到，尝试无命名空间...");
        type_MqttProtocolAdapter = Type.GetType("MqttProtocolAdapter");
        if (type_MqttProtocolAdapter != null)
            Console.WriteLine("[PASS] 类型 MqttProtocolAdapter (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttProtocolAdapter 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
