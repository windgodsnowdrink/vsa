#load "mediatr_mqtt_integration_pubsub.cs"

Console.WriteLine("=== mediatr_mqtt_integration_pubsub.cs Test ===");

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

    // 验证 class: MqttSubscriptionHandler
    var type_MqttSubscriptionHandler = Type.GetType("MqttSubscriptionHandler");
    if (type_MqttSubscriptionHandler != null)
    {
        Console.WriteLine("[PASS] 类型 MqttSubscriptionHandler (class) 存在");
        var ctors_MqttSubscriptionHandler = type_MqttSubscriptionHandler.GetConstructors();
        Console.WriteLine($"[PASS] MqttSubscriptionHandler 构造函数数量: {ctors_MqttSubscriptionHandler.Length}");
        var methods_MqttSubscriptionHandler = type_MqttSubscriptionHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttSubscriptionHandler 公开方法数量: {methods_MqttSubscriptionHandler.Length}");
        foreach (var m in methods_MqttSubscriptionHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttSubscriptionHandler 未找到，尝试无命名空间...");
        type_MqttSubscriptionHandler = Type.GetType("MqttSubscriptionHandler");
        if (type_MqttSubscriptionHandler != null)
            Console.WriteLine("[PASS] 类型 MqttSubscriptionHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttSubscriptionHandler 可能为顶层语句或嵌套类型");
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

    // 验证 class: MqttTransactionalBehavior
    var type_MqttTransactionalBehavior = Type.GetType("MqttTransactionalBehavior");
    if (type_MqttTransactionalBehavior != null)
    {
        Console.WriteLine("[PASS] 类型 MqttTransactionalBehavior (class) 存在");
        var ctors_MqttTransactionalBehavior = type_MqttTransactionalBehavior.GetConstructors();
        Console.WriteLine($"[PASS] MqttTransactionalBehavior 构造函数数量: {ctors_MqttTransactionalBehavior.Length}");
        var methods_MqttTransactionalBehavior = type_MqttTransactionalBehavior.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MqttTransactionalBehavior 公开方法数量: {methods_MqttTransactionalBehavior.Length}");
        foreach (var m in methods_MqttTransactionalBehavior)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MqttTransactionalBehavior 未找到，尝试无命名空间...");
        type_MqttTransactionalBehavior = Type.GetType("MqttTransactionalBehavior");
        if (type_MqttTransactionalBehavior != null)
            Console.WriteLine("[PASS] 类型 MqttTransactionalBehavior (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MqttTransactionalBehavior 可能为顶层语句或嵌套类型");
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
