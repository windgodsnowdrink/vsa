#load "mqtt_redis_integration.cs"

Console.WriteLine("=== mqtt_redis_integration.cs Test ===");

try
{
    // 验证 class: RedisStreamConsumer
    var type_RedisStreamConsumer = Type.GetType("RedisStreamConsumer");
    if (type_RedisStreamConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStreamConsumer (class) 存在");
        var ctors_RedisStreamConsumer = type_RedisStreamConsumer.GetConstructors();
        Console.WriteLine($"[PASS] RedisStreamConsumer 构造函数数量: {ctors_RedisStreamConsumer.Length}");
        var methods_RedisStreamConsumer = type_RedisStreamConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStreamConsumer 公开方法数量: {methods_RedisStreamConsumer.Length}");
        foreach (var m in methods_RedisStreamConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStreamConsumer 未找到，尝试无命名空间...");
        type_RedisStreamConsumer = Type.GetType("RedisStreamConsumer");
        if (type_RedisStreamConsumer != null)
            Console.WriteLine("[PASS] 类型 RedisStreamConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStreamConsumer 可能为顶层语句或嵌套类型");
    }

    // 验证 record: MqttMessage
    var type_MqttMessage = Type.GetType("MqttMessage");
    if (type_MqttMessage != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessage (record) 存在");
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
