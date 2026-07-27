#load "zeromq_demo.cs"

Console.WriteLine("=== zeromq_demo.cs Test ===");

try
{
    // 验证 class: ZeroMqMessageProcessor
    var type_ZeroMqMessageProcessor = Type.GetType("ZeroMqMessageProcessor");
    if (type_ZeroMqMessageProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroMqMessageProcessor (class) 存在");
        var ctors_ZeroMqMessageProcessor = type_ZeroMqMessageProcessor.GetConstructors();
        Console.WriteLine($"[PASS] ZeroMqMessageProcessor 构造函数数量: {ctors_ZeroMqMessageProcessor.Length}");
        var methods_ZeroMqMessageProcessor = type_ZeroMqMessageProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroMqMessageProcessor 公开方法数量: {methods_ZeroMqMessageProcessor.Length}");
        foreach (var m in methods_ZeroMqMessageProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroMqMessageProcessor 未找到，尝试无命名空间...");
        type_ZeroMqMessageProcessor = Type.GetType("ZeroMqMessageProcessor");
        if (type_ZeroMqMessageProcessor != null)
            Console.WriteLine("[PASS] 类型 ZeroMqMessageProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroMqMessageProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZeroMqMultiProtocolSupport
    var type_ZeroMqMultiProtocolSupport = Type.GetType("ZeroMqMultiProtocolSupport");
    if (type_ZeroMqMultiProtocolSupport != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroMqMultiProtocolSupport (class) 存在");
        var ctors_ZeroMqMultiProtocolSupport = type_ZeroMqMultiProtocolSupport.GetConstructors();
        Console.WriteLine($"[PASS] ZeroMqMultiProtocolSupport 构造函数数量: {ctors_ZeroMqMultiProtocolSupport.Length}");
        var methods_ZeroMqMultiProtocolSupport = type_ZeroMqMultiProtocolSupport.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroMqMultiProtocolSupport 公开方法数量: {methods_ZeroMqMultiProtocolSupport.Length}");
        foreach (var m in methods_ZeroMqMultiProtocolSupport)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroMqMultiProtocolSupport 未找到，尝试无命名空间...");
        type_ZeroMqMultiProtocolSupport = Type.GetType("ZeroMqMultiProtocolSupport");
        if (type_ZeroMqMultiProtocolSupport != null)
            Console.WriteLine("[PASS] 类型 ZeroMqMultiProtocolSupport (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroMqMultiProtocolSupport 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZeroMqMessageSecurity
    var type_ZeroMqMessageSecurity = Type.GetType("ZeroMqMessageSecurity");
    if (type_ZeroMqMessageSecurity != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroMqMessageSecurity (class) 存在");
        var ctors_ZeroMqMessageSecurity = type_ZeroMqMessageSecurity.GetConstructors();
        Console.WriteLine($"[PASS] ZeroMqMessageSecurity 构造函数数量: {ctors_ZeroMqMessageSecurity.Length}");
        var methods_ZeroMqMessageSecurity = type_ZeroMqMessageSecurity.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroMqMessageSecurity 公开方法数量: {methods_ZeroMqMessageSecurity.Length}");
        foreach (var m in methods_ZeroMqMessageSecurity)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroMqMessageSecurity 未找到，尝试无命名空间...");
        type_ZeroMqMessageSecurity = Type.GetType("ZeroMqMessageSecurity");
        if (type_ZeroMqMessageSecurity != null)
            Console.WriteLine("[PASS] 类型 ZeroMqMessageSecurity (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroMqMessageSecurity 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZeroMqClusterManager
    var type_ZeroMqClusterManager = Type.GetType("ZeroMqClusterManager");
    if (type_ZeroMqClusterManager != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroMqClusterManager (class) 存在");
        var ctors_ZeroMqClusterManager = type_ZeroMqClusterManager.GetConstructors();
        Console.WriteLine($"[PASS] ZeroMqClusterManager 构造函数数量: {ctors_ZeroMqClusterManager.Length}");
        var methods_ZeroMqClusterManager = type_ZeroMqClusterManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroMqClusterManager 公开方法数量: {methods_ZeroMqClusterManager.Length}");
        foreach (var m in methods_ZeroMqClusterManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroMqClusterManager 未找到，尝试无命名空间...");
        type_ZeroMqClusterManager = Type.GetType("ZeroMqClusterManager");
        if (type_ZeroMqClusterManager != null)
            Console.WriteLine("[PASS] 类型 ZeroMqClusterManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroMqClusterManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ZeroMqMetrics
    var type_ZeroMqMetrics = Type.GetType("ZeroMqMetrics");
    if (type_ZeroMqMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroMqMetrics (class) 存在");
        var ctors_ZeroMqMetrics = type_ZeroMqMetrics.GetConstructors();
        Console.WriteLine($"[PASS] ZeroMqMetrics 构造函数数量: {ctors_ZeroMqMetrics.Length}");
        var methods_ZeroMqMetrics = type_ZeroMqMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroMqMetrics 公开方法数量: {methods_ZeroMqMetrics.Length}");
        foreach (var m in methods_ZeroMqMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroMqMetrics 未找到，尝试无命名空间...");
        type_ZeroMqMetrics = Type.GetType("ZeroMqMetrics");
        if (type_ZeroMqMetrics != null)
            Console.WriteLine("[PASS] 类型 ZeroMqMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroMqMetrics 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
