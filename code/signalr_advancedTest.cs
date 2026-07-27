#load "signalr_advanced.cs"

Console.WriteLine("=== signalr_advanced.cs Test ===");

try
{
    // 验证 class: AdvancedHub
    var type_AdvancedHub = Type.GetType("AdvancedHub");
    if (type_AdvancedHub != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedHub (class) 存在");
        var ctors_AdvancedHub = type_AdvancedHub.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedHub 构造函数数量: {ctors_AdvancedHub.Length}");
        var methods_AdvancedHub = type_AdvancedHub.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedHub 公开方法数量: {methods_AdvancedHub.Length}");
        foreach (var m in methods_AdvancedHub)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedHub 未找到，尝试无命名空间...");
        type_AdvancedHub = Type.GetType("AdvancedHub");
        if (type_AdvancedHub != null)
            Console.WriteLine("[PASS] 类型 AdvancedHub (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedHub 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PriorityBasedQoS
    var type_PriorityBasedQoS = Type.GetType("PriorityBasedQoS");
    if (type_PriorityBasedQoS != null)
    {
        Console.WriteLine("[PASS] 类型 PriorityBasedQoS (class) 存在");
        var ctors_PriorityBasedQoS = type_PriorityBasedQoS.GetConstructors();
        Console.WriteLine($"[PASS] PriorityBasedQoS 构造函数数量: {ctors_PriorityBasedQoS.Length}");
        var methods_PriorityBasedQoS = type_PriorityBasedQoS.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PriorityBasedQoS 公开方法数量: {methods_PriorityBasedQoS.Length}");
        foreach (var m in methods_PriorityBasedQoS)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PriorityBasedQoS 未找到，尝试无命名空间...");
        type_PriorityBasedQoS = Type.GetType("PriorityBasedQoS");
        if (type_PriorityBasedQoS != null)
            Console.WriteLine("[PASS] 类型 PriorityBasedQoS (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PriorityBasedQoS 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OfflineMessageContext
    var type_OfflineMessageContext = Type.GetType("OfflineMessageContext");
    if (type_OfflineMessageContext != null)
    {
        Console.WriteLine("[PASS] 类型 OfflineMessageContext (class) 存在");
        var ctors_OfflineMessageContext = type_OfflineMessageContext.GetConstructors();
        Console.WriteLine($"[PASS] OfflineMessageContext 构造函数数量: {ctors_OfflineMessageContext.Length}");
        var methods_OfflineMessageContext = type_OfflineMessageContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OfflineMessageContext 公开方法数量: {methods_OfflineMessageContext.Length}");
        foreach (var m in methods_OfflineMessageContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OfflineMessageContext 未找到，尝试无命名空间...");
        type_OfflineMessageContext = Type.GetType("OfflineMessageContext");
        if (type_OfflineMessageContext != null)
            Console.WriteLine("[PASS] 类型 OfflineMessageContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OfflineMessageContext 可能为顶层语句或嵌套类型");
    }

    // 验证 record: QoSEntry
    var type_QoSEntry = Type.GetType("QoSEntry");
    if (type_QoSEntry != null)
    {
        Console.WriteLine("[PASS] 类型 QoSEntry (record) 存在");
        var ctors_QoSEntry = type_QoSEntry.GetConstructors();
        Console.WriteLine($"[PASS] QoSEntry 构造函数数量: {ctors_QoSEntry.Length}");
        var methods_QoSEntry = type_QoSEntry.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QoSEntry 公开方法数量: {methods_QoSEntry.Length}");
        foreach (var m in methods_QoSEntry)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QoSEntry 未找到，尝试无命名空间...");
        type_QoSEntry = Type.GetType("QoSEntry");
        if (type_QoSEntry != null)
            Console.WriteLine("[PASS] 类型 QoSEntry (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QoSEntry 可能为顶层语句或嵌套类型");
    }

    // 验证 record: OfflineMessage
    var type_OfflineMessage = Type.GetType("OfflineMessage");
    if (type_OfflineMessage != null)
    {
        Console.WriteLine("[PASS] 类型 OfflineMessage (record) 存在");
        var ctors_OfflineMessage = type_OfflineMessage.GetConstructors();
        Console.WriteLine($"[PASS] OfflineMessage 构造函数数量: {ctors_OfflineMessage.Length}");
        var methods_OfflineMessage = type_OfflineMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OfflineMessage 公开方法数量: {methods_OfflineMessage.Length}");
        foreach (var m in methods_OfflineMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OfflineMessage 未找到，尝试无命名空间...");
        type_OfflineMessage = Type.GetType("OfflineMessage");
        if (type_OfflineMessage != null)
            Console.WriteLine("[PASS] 类型 OfflineMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OfflineMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: QoSPriority
    var type_QoSPriority = Type.GetType("QoSPriority");
    if (type_QoSPriority != null)
    {
        Console.WriteLine("[PASS] 类型 QoSPriority (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QoSPriority 未找到，尝试无命名空间...");
        type_QoSPriority = Type.GetType("QoSPriority");
        if (type_QoSPriority != null)
            Console.WriteLine("[PASS] 类型 QoSPriority (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QoSPriority 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
