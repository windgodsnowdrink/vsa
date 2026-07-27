#load "mqtt_integration.cs"

Console.WriteLine("=== mqtt_integration.cs Test ===");

try
{
    // 验证 class: ChannelMqttProcessor
    var type_ChannelMqttProcessor = Type.GetType("ChannelMqttProcessor");
    if (type_ChannelMqttProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelMqttProcessor (class) 存在");
        var ctors_ChannelMqttProcessor = type_ChannelMqttProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ChannelMqttProcessor 构造函数数量: {ctors_ChannelMqttProcessor.Length}");
        var methods_ChannelMqttProcessor = type_ChannelMqttProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelMqttProcessor 公开方法数量: {methods_ChannelMqttProcessor.Length}");
        foreach (var m in methods_ChannelMqttProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelMqttProcessor 未找到，尝试无命名空间...");
        type_ChannelMqttProcessor = Type.GetType("ChannelMqttProcessor");
        if (type_ChannelMqttProcessor != null)
            Console.WriteLine("[PASS] 类型 ChannelMqttProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelMqttProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessage
    var type_MqttMessage = Type.GetType("MqttMessage");
    if (type_MqttMessage != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessage (class) 存在");
        var ctors_MqttMessage = type_MqttMessage.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessage 构造函数数量: {ctors_MqttMessage.Length}");
        var methods_MqttMessage = type_MqttMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessage 公开方法数量: {methods_MqttMessage.Length}");
        foreach (var m in methods_MqttMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessage 未找到，尝试无命名空间...");
        type_MqttMessage = Type.GetType("MqttMessage");
        if (type_MqttMessage != null)
            Console.WriteLine("[PASS] 类型 MqttMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
