#load "sip_turn_integration.cs"

Console.WriteLine("=== sip_turn_integration.cs Test ===");

try
{
    // 验证 class: TURNClient
    var type_TURNClient = Type.GetType("TURNClient");
    if (type_TURNClient != null)
    {
        Console.WriteLine("[PASS] 类型 TURNClient (class) 存在");
        var ctors_TURNClient = type_TURNClient.GetConstructors();
        Console.WriteLine($"[PASS] TURNClient 构造函数数量: {ctors_TURNClient.Length}");
        var methods_TURNClient = type_TURNClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TURNClient 公开方法数量: {methods_TURNClient.Length}");
        foreach (var m in methods_TURNClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TURNClient 未找到，尝试无命名空间...");
        type_TURNClient = Type.GetType("TURNClient");
        if (type_TURNClient != null)
            Console.WriteLine("[PASS] 类型 TURNClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TURNClient 可能为顶层语句或嵌套类型");
    }

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

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
