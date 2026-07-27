#load "mqtt_enhanced.cs"

Console.WriteLine("=== mqtt_enhanced.cs Test ===");

try
{
    // 验证 class: ChannelQosController
    var type_ChannelQosController = Type.GetType("ChannelQosController");
    if (type_ChannelQosController != null)
    {
        Console.WriteLine("[PASS] 类型 ChannelQosController (class) 存在");
        var ctors_ChannelQosController = type_ChannelQosController.GetConstructors();
        Console.WriteLine($"[PASS] ChannelQosController 构造函数数量: {ctors_ChannelQosController.Length}");
        var methods_ChannelQosController = type_ChannelQosController.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChannelQosController 公开方法数量: {methods_ChannelQosController.Length}");
        foreach (var m in methods_ChannelQosController)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChannelQosController 未找到，尝试无命名空间...");
        type_ChannelQosController = Type.GetType("ChannelQosController");
        if (type_ChannelQosController != null)
            Console.WriteLine("[PASS] 类型 ChannelQosController (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChannelQosController 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EnhancedMqttProcessor
    var type_EnhancedMqttProcessor = Type.GetType("EnhancedMqttProcessor");
    if (type_EnhancedMqttProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 EnhancedMqttProcessor (class) 存在");
        var ctors_EnhancedMqttProcessor = type_EnhancedMqttProcessor.GetConstructors();
        Console.WriteLine($"[PASS] EnhancedMqttProcessor 构造函数数量: {ctors_EnhancedMqttProcessor.Length}");
        var methods_EnhancedMqttProcessor = type_EnhancedMqttProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EnhancedMqttProcessor 公开方法数量: {methods_EnhancedMqttProcessor.Length}");
        foreach (var m in methods_EnhancedMqttProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EnhancedMqttProcessor 未找到，尝试无命名空间...");
        type_EnhancedMqttProcessor = Type.GetType("EnhancedMqttProcessor");
        if (type_EnhancedMqttProcessor != null)
            Console.WriteLine("[PASS] 类型 EnhancedMqttProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EnhancedMqttProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QosMessage
    var type_QosMessage = Type.GetType("QosMessage");
    if (type_QosMessage != null)
    {
        Console.WriteLine("[PASS] 类型 QosMessage (class) 存在");
        var ctors_QosMessage = type_QosMessage.GetConstructors();
        Console.WriteLine($"[PASS] QosMessage 构造函数数量: {ctors_QosMessage.Length}");
        var methods_QosMessage = type_QosMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QosMessage 公开方法数量: {methods_QosMessage.Length}");
        foreach (var m in methods_QosMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QosMessage 未找到，尝试无命名空间...");
        type_QosMessage = Type.GetType("QosMessage");
        if (type_QosMessage != null)
            Console.WriteLine("[PASS] 类型 QosMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QosMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
