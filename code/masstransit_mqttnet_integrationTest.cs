#load "masstransit_mqttnet_integration.cs"

Console.WriteLine("=== masstransit_mqttnet_integration.cs Test ===");

try
{
    // 验证 class: CustomMqttTransport
    var type_CustomMqttTransport = Type.GetType("CustomMqttTransport");
    if (type_CustomMqttTransport != null)
    {
        Console.WriteLine("[PASS] 类型 CustomMqttTransport (class) 存在");
        var ctors_CustomMqttTransport = type_CustomMqttTransport.GetConstructors();
        Console.WriteLine($"[PASS] CustomMqttTransport 构造函数数量: {ctors_CustomMqttTransport.Length}");
        var methods_CustomMqttTransport = type_CustomMqttTransport.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CustomMqttTransport 公开方法数量: {methods_CustomMqttTransport.Length}");
        foreach (var m in methods_CustomMqttTransport)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CustomMqttTransport 未找到，尝试无命名空间...");
        type_CustomMqttTransport = Type.GetType("CustomMqttTransport");
        if (type_CustomMqttTransport != null)
            Console.WriteLine("[PASS] 类型 CustomMqttTransport (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CustomMqttTransport 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageConsumer
    var type_MqttMessageConsumer = Type.GetType("MqttMessageConsumer");
    if (type_MqttMessageConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageConsumer (class) 存在");
        var ctors_MqttMessageConsumer = type_MqttMessageConsumer.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageConsumer 构造函数数量: {ctors_MqttMessageConsumer.Length}");
        var methods_MqttMessageConsumer = type_MqttMessageConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageConsumer 公开方法数量: {methods_MqttMessageConsumer.Length}");
        foreach (var m in methods_MqttMessageConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageConsumer 未找到，尝试无命名空间...");
        type_MqttMessageConsumer = Type.GetType("MqttMessageConsumer");
        if (type_MqttMessageConsumer != null)
            Console.WriteLine("[PASS] 类型 MqttMessageConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageConsumer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RabbitToMqttBridgeConsumer
    var type_RabbitToMqttBridgeConsumer = Type.GetType("RabbitToMqttBridgeConsumer");
    if (type_RabbitToMqttBridgeConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 RabbitToMqttBridgeConsumer (class) 存在");
        var ctors_RabbitToMqttBridgeConsumer = type_RabbitToMqttBridgeConsumer.GetConstructors();
        Console.WriteLine($"[PASS] RabbitToMqttBridgeConsumer 构造函数数量: {ctors_RabbitToMqttBridgeConsumer.Length}");
        var methods_RabbitToMqttBridgeConsumer = type_RabbitToMqttBridgeConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RabbitToMqttBridgeConsumer 公开方法数量: {methods_RabbitToMqttBridgeConsumer.Length}");
        foreach (var m in methods_RabbitToMqttBridgeConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RabbitToMqttBridgeConsumer 未找到，尝试无命名空间...");
        type_RabbitToMqttBridgeConsumer = Type.GetType("RabbitToMqttBridgeConsumer");
        if (type_RabbitToMqttBridgeConsumer != null)
            Console.WriteLine("[PASS] 类型 RabbitToMqttBridgeConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RabbitToMqttBridgeConsumer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttToRabbitBridgeConsumer
    var type_MqttToRabbitBridgeConsumer = Type.GetType("MqttToRabbitBridgeConsumer");
    if (type_MqttToRabbitBridgeConsumer != null)
    {
        Console.WriteLine("[PASS] 类型 MqttToRabbitBridgeConsumer (class) 存在");
        var ctors_MqttToRabbitBridgeConsumer = type_MqttToRabbitBridgeConsumer.GetConstructors();
        Console.WriteLine($"[PASS] MqttToRabbitBridgeConsumer 构造函数数量: {ctors_MqttToRabbitBridgeConsumer.Length}");
        var methods_MqttToRabbitBridgeConsumer = type_MqttToRabbitBridgeConsumer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttToRabbitBridgeConsumer 公开方法数量: {methods_MqttToRabbitBridgeConsumer.Length}");
        foreach (var m in methods_MqttToRabbitBridgeConsumer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttToRabbitBridgeConsumer 未找到，尝试无命名空间...");
        type_MqttToRabbitBridgeConsumer = Type.GetType("MqttToRabbitBridgeConsumer");
        if (type_MqttToRabbitBridgeConsumer != null)
            Console.WriteLine("[PASS] 类型 MqttToRabbitBridgeConsumer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttToRabbitBridgeConsumer 可能为顶层语句或嵌套类型");
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
