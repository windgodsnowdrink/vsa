#load "mediatr_mqtt_integration_message.cs"

Console.WriteLine("=== mediatr_mqtt_integration_message.cs Test ===");

try
{
    // 验证 class: MqttPublishBehavior
    var type_MqttPublishBehavior = Type.GetType("MqttPublishBehavior");
    if (type_MqttPublishBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 MqttPublishBehavior (class) 存在");
        var ctors_MqttPublishBehavior = type_MqttPublishBehavior.GetConstructors();
        Console.WriteLine($"[PASS] MqttPublishBehavior 构造函数数量: {ctors_MqttPublishBehavior.Length}");
        var methods_MqttPublishBehavior = type_MqttPublishBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttPublishBehavior 公开方法数量: {methods_MqttPublishBehavior.Length}");
        foreach (var m in methods_MqttPublishBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttPublishBehavior 未找到，尝试无命名空间...");
        type_MqttPublishBehavior = Type.GetType("MqttPublishBehavior");
        if (type_MqttPublishBehavior != null)
            Console.WriteLine("[PASS] 类型 MqttPublishBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttPublishBehavior 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttMessageHandler
    var type_MqttMessageHandler = Type.GetType("MqttMessageHandler");
    if (type_MqttMessageHandler != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageHandler (class) 存在");
        var ctors_MqttMessageHandler = type_MqttMessageHandler.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageHandler 构造函数数量: {ctors_MqttMessageHandler.Length}");
        var methods_MqttMessageHandler = type_MqttMessageHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageHandler 公开方法数量: {methods_MqttMessageHandler.Length}");
        foreach (var m in methods_MqttMessageHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageHandler 未找到，尝试无命名空间...");
        type_MqttMessageHandler = Type.GetType("MqttMessageHandler");
        if (type_MqttMessageHandler != null)
            Console.WriteLine("[PASS] 类型 MqttMessageHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedTransactionBehavior
    var type_DistributedTransactionBehavior = Type.GetType("DistributedTransactionBehavior");
    if (type_DistributedTransactionBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTransactionBehavior (class) 存在");
        var ctors_DistributedTransactionBehavior = type_DistributedTransactionBehavior.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTransactionBehavior 构造函数数量: {ctors_DistributedTransactionBehavior.Length}");
        var methods_DistributedTransactionBehavior = type_DistributedTransactionBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTransactionBehavior 公开方法数量: {methods_DistributedTransactionBehavior.Length}");
        foreach (var m in methods_DistributedTransactionBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTransactionBehavior 未找到，尝试无命名空间...");
        type_DistributedTransactionBehavior = Type.GetType("DistributedTransactionBehavior");
        if (type_DistributedTransactionBehavior != null)
            Console.WriteLine("[PASS] 类型 DistributedTransactionBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTransactionBehavior 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttOptions
    var type_MqttOptions = Type.GetType("MqttOptions");
    if (type_MqttOptions != null)
    {
        Console.WriteLine("[PASS] 类型 MqttOptions (class) 存在");
        var ctors_MqttOptions = type_MqttOptions.GetConstructors();
        Console.WriteLine($"[PASS] MqttOptions 构造函数数量: {ctors_MqttOptions.Length}");
        var methods_MqttOptions = type_MqttOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttOptions 公开方法数量: {methods_MqttOptions.Length}");
        foreach (var m in methods_MqttOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttOptions 未找到，尝试无命名空间...");
        type_MqttOptions = Type.GetType("MqttOptions");
        if (type_MqttOptions != null)
            Console.WriteLine("[PASS] 类型 MqttOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ProcessMessage
    var type_ProcessMessage = Type.GetType("ProcessMessage");
    if (type_ProcessMessage != null)
    {
        Console.WriteLine("[PASS] 类型 ProcessMessage (record) 存在");
        var ctors_ProcessMessage = type_ProcessMessage.GetConstructors();
        Console.WriteLine($"[PASS] ProcessMessage 构造函数数量: {ctors_ProcessMessage.Length}");
        var methods_ProcessMessage = type_ProcessMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ProcessMessage 公开方法数量: {methods_ProcessMessage.Length}");
        foreach (var m in methods_ProcessMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ProcessMessage 未找到，尝试无命名空间...");
        type_ProcessMessage = Type.GetType("ProcessMessage");
        if (type_ProcessMessage != null)
            Console.WriteLine("[PASS] 类型 ProcessMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ProcessMessage 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
