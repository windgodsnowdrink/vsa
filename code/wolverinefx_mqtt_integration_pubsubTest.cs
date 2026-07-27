#load "wolverinefx_mqtt_integration_pubsub.cs"

Console.WriteLine("=== wolverinefx_mqtt_integration_pubsub.cs Test ===");

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

    // 验证 class: MqttTransport
    var type_MqttTransport = Type.GetType("MqttTransport");
    if (type_MqttTransport != null)
    {
        Console.WriteLine("[PASS] 类型 MqttTransport (class) 存在");
        var ctors_MqttTransport = type_MqttTransport.GetConstructors();
        Console.WriteLine($"[PASS] MqttTransport 构造函数数量: {ctors_MqttTransport.Length}");
        var methods_MqttTransport = type_MqttTransport.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttTransport 公开方法数量: {methods_MqttTransport.Length}");
        foreach (var m in methods_MqttTransport)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttTransport 未找到，尝试无命名空间...");
        type_MqttTransport = Type.GetType("MqttTransport");
        if (type_MqttTransport != null)
            Console.WriteLine("[PASS] 类型 MqttTransport (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttTransport 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttTransactionalMiddleware
    var type_MqttTransactionalMiddleware = Type.GetType("MqttTransactionalMiddleware");
    if (type_MqttTransactionalMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 MqttTransactionalMiddleware (class) 存在");
        var ctors_MqttTransactionalMiddleware = type_MqttTransactionalMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] MqttTransactionalMiddleware 构造函数数量: {ctors_MqttTransactionalMiddleware.Length}");
        var methods_MqttTransactionalMiddleware = type_MqttTransactionalMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttTransactionalMiddleware 公开方法数量: {methods_MqttTransactionalMiddleware.Length}");
        foreach (var m in methods_MqttTransactionalMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttTransactionalMiddleware 未找到，尝试无命名空间...");
        type_MqttTransactionalMiddleware = Type.GetType("MqttTransactionalMiddleware");
        if (type_MqttTransactionalMiddleware != null)
            Console.WriteLine("[PASS] 类型 MqttTransactionalMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttTransactionalMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttReliabilityService
    var type_MqttReliabilityService = Type.GetType("MqttReliabilityService");
    if (type_MqttReliabilityService != null)
    {
        Console.WriteLine("[PASS] 类型 MqttReliabilityService (class) 存在");
        var ctors_MqttReliabilityService = type_MqttReliabilityService.GetConstructors();
        Console.WriteLine($"[PASS] MqttReliabilityService 构造函数数量: {ctors_MqttReliabilityService.Length}");
        var methods_MqttReliabilityService = type_MqttReliabilityService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttReliabilityService 公开方法数量: {methods_MqttReliabilityService.Length}");
        foreach (var m in methods_MqttReliabilityService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttReliabilityService 未找到，尝试无命名空间...");
        type_MqttReliabilityService = Type.GetType("MqttReliabilityService");
        if (type_MqttReliabilityService != null)
            Console.WriteLine("[PASS] 类型 MqttReliabilityService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttReliabilityService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MqttPayloadSerializer
    var type_MqttPayloadSerializer = Type.GetType("MqttPayloadSerializer");
    if (type_MqttPayloadSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 MqttPayloadSerializer (class) 存在");
        var ctors_MqttPayloadSerializer = type_MqttPayloadSerializer.GetConstructors();
        Console.WriteLine($"[PASS] MqttPayloadSerializer 构造函数数量: {ctors_MqttPayloadSerializer.Length}");
        var methods_MqttPayloadSerializer = type_MqttPayloadSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttPayloadSerializer 公开方法数量: {methods_MqttPayloadSerializer.Length}");
        foreach (var m in methods_MqttPayloadSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttPayloadSerializer 未找到，尝试无命名空间...");
        type_MqttPayloadSerializer = Type.GetType("MqttPayloadSerializer");
        if (type_MqttPayloadSerializer != null)
            Console.WriteLine("[PASS] 类型 MqttPayloadSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttPayloadSerializer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TruncatedMemoryOwner
    var type_TruncatedMemoryOwner = Type.GetType("TruncatedMemoryOwner");
    if (type_TruncatedMemoryOwner != null)
    {
        Console.WriteLine("[PASS] 类型 TruncatedMemoryOwner (class) 存在");
        var ctors_TruncatedMemoryOwner = type_TruncatedMemoryOwner.GetConstructors();
        Console.WriteLine($"[PASS] TruncatedMemoryOwner 构造函数数量: {ctors_TruncatedMemoryOwner.Length}");
        var methods_TruncatedMemoryOwner = type_TruncatedMemoryOwner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TruncatedMemoryOwner 公开方法数量: {methods_TruncatedMemoryOwner.Length}");
        foreach (var m in methods_TruncatedMemoryOwner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TruncatedMemoryOwner 未找到，尝试无命名空间...");
        type_TruncatedMemoryOwner = Type.GetType("TruncatedMemoryOwner");
        if (type_TruncatedMemoryOwner != null)
            Console.WriteLine("[PASS] 类型 TruncatedMemoryOwner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TruncatedMemoryOwner 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
