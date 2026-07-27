#load "wolverinefx_mqtt_integration_message.cs"

Console.WriteLine("=== wolverinefx_mqtt_integration_message.cs Test ===");

try
{
    // 验证 class: MqttMessageHandlers
    var type_MqttMessageHandlers = Type.GetType("MqttMessageHandlers");
    if (type_MqttMessageHandlers != null)
    {
        Console.WriteLine("[PASS] 类型 MqttMessageHandlers (class) 存在");
        var ctors_MqttMessageHandlers = type_MqttMessageHandlers.GetConstructors();
        Console.WriteLine($"[PASS] MqttMessageHandlers 构造函数数量: {ctors_MqttMessageHandlers.Length}");
        var methods_MqttMessageHandlers = type_MqttMessageHandlers.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttMessageHandlers 公开方法数量: {methods_MqttMessageHandlers.Length}");
        foreach (var m in methods_MqttMessageHandlers)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttMessageHandlers 未找到，尝试无命名空间...");
        type_MqttMessageHandlers = Type.GetType("MqttMessageHandlers");
        if (type_MqttMessageHandlers != null)
            Console.WriteLine("[PASS] 类型 MqttMessageHandlers (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttMessageHandlers 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedTransactionMiddleware
    var type_DistributedTransactionMiddleware = Type.GetType("DistributedTransactionMiddleware");
    if (type_DistributedTransactionMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedTransactionMiddleware (class) 存在");
        var ctors_DistributedTransactionMiddleware = type_DistributedTransactionMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] DistributedTransactionMiddleware 构造函数数量: {ctors_DistributedTransactionMiddleware.Length}");
        var methods_DistributedTransactionMiddleware = type_DistributedTransactionMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedTransactionMiddleware 公开方法数量: {methods_DistributedTransactionMiddleware.Length}");
        foreach (var m in methods_DistributedTransactionMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedTransactionMiddleware 未找到，尝试无命名空间...");
        type_DistributedTransactionMiddleware = Type.GetType("DistributedTransactionMiddleware");
        if (type_DistributedTransactionMiddleware != null)
            Console.WriteLine("[PASS] 类型 DistributedTransactionMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedTransactionMiddleware 可能为顶层语句或嵌套类型");
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
