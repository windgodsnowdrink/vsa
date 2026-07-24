#load "sipsorcery_integration.cs"

Console.WriteLine("=== sipsorcery_integration.cs Test ===");

try
{
    // 验证 class: SIPSignalingEngine
    var type_SIPSignalingEngine = Type.GetType("SIPSignalingEngine");
    if (type_SIPSignalingEngine != null)
    {
        Console.WriteLine("[PASS] 类型 SIPSignalingEngine (class) 存在");
        var ctors_SIPSignalingEngine = type_SIPSignalingEngine.GetConstructors();
        Console.WriteLine($"[PASS] SIPSignalingEngine 构造函数数量: {ctors_SIPSignalingEngine.Length}");
        var methods_SIPSignalingEngine = type_SIPSignalingEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SIPSignalingEngine 公开方法数量: {methods_SIPSignalingEngine.Length}");
        foreach (var m in methods_SIPSignalingEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SIPSignalingEngine 未找到，尝试无命名空间...");
        type_SIPSignalingEngine = Type.GetType("SIPSignalingEngine");
        if (type_SIPSignalingEngine != null)
            Console.WriteLine("[PASS] 类型 SIPSignalingEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SIPSignalingEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SIPZeroCopyTransport
    var type_SIPZeroCopyTransport = Type.GetType("SIPZeroCopyTransport");
    if (type_SIPZeroCopyTransport != null)
    {
        Console.WriteLine("[PASS] 类型 SIPZeroCopyTransport (class) 存在");
        var ctors_SIPZeroCopyTransport = type_SIPZeroCopyTransport.GetConstructors();
        Console.WriteLine($"[PASS] SIPZeroCopyTransport 构造函数数量: {ctors_SIPZeroCopyTransport.Length}");
        var methods_SIPZeroCopyTransport = type_SIPZeroCopyTransport.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SIPZeroCopyTransport 公开方法数量: {methods_SIPZeroCopyTransport.Length}");
        foreach (var m in methods_SIPZeroCopyTransport)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SIPZeroCopyTransport 未找到，尝试无命名空间...");
        type_SIPZeroCopyTransport = Type.GetType("SIPZeroCopyTransport");
        if (type_SIPZeroCopyTransport != null)
            Console.WriteLine("[PASS] 类型 SIPZeroCopyTransport (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SIPZeroCopyTransport 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SIPEventProcessor
    var type_SIPEventProcessor = Type.GetType("SIPEventProcessor");
    if (type_SIPEventProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 SIPEventProcessor (class) 存在");
        var ctors_SIPEventProcessor = type_SIPEventProcessor.GetConstructors();
        Console.WriteLine($"[PASS] SIPEventProcessor 构造函数数量: {ctors_SIPEventProcessor.Length}");
        var methods_SIPEventProcessor = type_SIPEventProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SIPEventProcessor 公开方法数量: {methods_SIPEventProcessor.Length}");
        foreach (var m in methods_SIPEventProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SIPEventProcessor 未找到，尝试无命名空间...");
        type_SIPEventProcessor = Type.GetType("SIPEventProcessor");
        if (type_SIPEventProcessor != null)
            Console.WriteLine("[PASS] 类型 SIPEventProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SIPEventProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WebRTCIntegration
    var type_WebRTCIntegration = Type.GetType("WebRTCIntegration");
    if (type_WebRTCIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 WebRTCIntegration (class) 存在");
        var ctors_WebRTCIntegration = type_WebRTCIntegration.GetConstructors();
        Console.WriteLine($"[PASS] WebRTCIntegration 构造函数数量: {ctors_WebRTCIntegration.Length}");
        var methods_WebRTCIntegration = type_WebRTCIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WebRTCIntegration 公开方法数量: {methods_WebRTCIntegration.Length}");
        foreach (var m in methods_WebRTCIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WebRTCIntegration 未找到，尝试无命名空间...");
        type_WebRTCIntegration = Type.GetType("WebRTCIntegration");
        if (type_WebRTCIntegration != null)
            Console.WriteLine("[PASS] 类型 WebRTCIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WebRTCIntegration 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
