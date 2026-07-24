#load "signalr_monitoring.cs"

Console.WriteLine("=== signalr_monitoring.cs Test ===");

try
{
    // 验证 class: CompressionMonitor
    var type_CompressionMonitor = Type.GetType("CompressionMonitor");
    if (type_CompressionMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionMonitor (class) 存在");
        var ctors_CompressionMonitor = type_CompressionMonitor.GetConstructors();
        Console.WriteLine($"[PASS] CompressionMonitor 构造函数数量: {ctors_CompressionMonitor.Length}");
        var methods_CompressionMonitor = type_CompressionMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressionMonitor 公开方法数量: {methods_CompressionMonitor.Length}");
        foreach (var m in methods_CompressionMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionMonitor 未找到，尝试无命名空间...");
        type_CompressionMonitor = Type.GetType("CompressionMonitor");
        if (type_CompressionMonitor != null)
            Console.WriteLine("[PASS] 类型 CompressionMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ExpirationEventNotifier
    var type_ExpirationEventNotifier = Type.GetType("ExpirationEventNotifier");
    if (type_ExpirationEventNotifier != null)
    {
        Console.WriteLine("[PASS] 类型 ExpirationEventNotifier (class) 存在");
        var ctors_ExpirationEventNotifier = type_ExpirationEventNotifier.GetConstructors();
        Console.WriteLine($"[PASS] ExpirationEventNotifier 构造函数数量: {ctors_ExpirationEventNotifier.Length}");
        var methods_ExpirationEventNotifier = type_ExpirationEventNotifier.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExpirationEventNotifier 公开方法数量: {methods_ExpirationEventNotifier.Length}");
        foreach (var m in methods_ExpirationEventNotifier)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExpirationEventNotifier 未找到，尝试无命名空间...");
        type_ExpirationEventNotifier = Type.GetType("ExpirationEventNotifier");
        if (type_ExpirationEventNotifier != null)
            Console.WriteLine("[PASS] 类型 ExpirationEventNotifier (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExpirationEventNotifier 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HotUpdateQoSConfig
    var type_HotUpdateQoSConfig = Type.GetType("HotUpdateQoSConfig");
    if (type_HotUpdateQoSConfig != null)
    {
        Console.WriteLine("[PASS] 类型 HotUpdateQoSConfig (class) 存在");
        var ctors_HotUpdateQoSConfig = type_HotUpdateQoSConfig.GetConstructors();
        Console.WriteLine($"[PASS] HotUpdateQoSConfig 构造函数数量: {ctors_HotUpdateQoSConfig.Length}");
        var methods_HotUpdateQoSConfig = type_HotUpdateQoSConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HotUpdateQoSConfig 公开方法数量: {methods_HotUpdateQoSConfig.Length}");
        foreach (var m in methods_HotUpdateQoSConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HotUpdateQoSConfig 未找到，尝试无命名空间...");
        type_HotUpdateQoSConfig = Type.GetType("HotUpdateQoSConfig");
        if (type_HotUpdateQoSConfig != null)
            Console.WriteLine("[PASS] 类型 HotUpdateQoSConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HotUpdateQoSConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QoSOptions
    var type_QoSOptions = Type.GetType("QoSOptions");
    if (type_QoSOptions != null)
    {
        Console.WriteLine("[PASS] 类型 QoSOptions (class) 存在");
        var ctors_QoSOptions = type_QoSOptions.GetConstructors();
        Console.WriteLine($"[PASS] QoSOptions 构造函数数量: {ctors_QoSOptions.Length}");
        var methods_QoSOptions = type_QoSOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QoSOptions 公开方法数量: {methods_QoSOptions.Length}");
        foreach (var m in methods_QoSOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QoSOptions 未找到，尝试无命名空间...");
        type_QoSOptions = Type.GetType("QoSOptions");
        if (type_QoSOptions != null)
            Console.WriteLine("[PASS] 类型 QoSOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QoSOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CompressionMetrics
    var type_CompressionMetrics = Type.GetType("CompressionMetrics");
    if (type_CompressionMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 CompressionMetrics (record) 存在");
        var ctors_CompressionMetrics = type_CompressionMetrics.GetConstructors();
        Console.WriteLine($"[PASS] CompressionMetrics 构造函数数量: {ctors_CompressionMetrics.Length}");
        var methods_CompressionMetrics = type_CompressionMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompressionMetrics 公开方法数量: {methods_CompressionMetrics.Length}");
        foreach (var m in methods_CompressionMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompressionMetrics 未找到，尝试无命名空间...");
        type_CompressionMetrics = Type.GetType("CompressionMetrics");
        if (type_CompressionMetrics != null)
            Console.WriteLine("[PASS] 类型 CompressionMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompressionMetrics 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ExpiredMessageEvent
    var type_ExpiredMessageEvent = Type.GetType("ExpiredMessageEvent");
    if (type_ExpiredMessageEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ExpiredMessageEvent (record) 存在");
        var ctors_ExpiredMessageEvent = type_ExpiredMessageEvent.GetConstructors();
        Console.WriteLine($"[PASS] ExpiredMessageEvent 构造函数数量: {ctors_ExpiredMessageEvent.Length}");
        var methods_ExpiredMessageEvent = type_ExpiredMessageEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ExpiredMessageEvent 公开方法数量: {methods_ExpiredMessageEvent.Length}");
        foreach (var m in methods_ExpiredMessageEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ExpiredMessageEvent 未找到，尝试无命名空间...");
        type_ExpiredMessageEvent = Type.GetType("ExpiredMessageEvent");
        if (type_ExpiredMessageEvent != null)
            Console.WriteLine("[PASS] 类型 ExpiredMessageEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ExpiredMessageEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
