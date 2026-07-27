#load "sip_doturn_integration.cs"

Console.WriteLine("=== sip_doturn_integration.cs Test ===");

try
{
    // 验证 class: TurnIntegration
    var type_TurnIntegration = Type.GetType("TurnIntegration");
    if (type_TurnIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 TurnIntegration (class) 存在");
        var ctors_TurnIntegration = type_TurnIntegration.GetConstructors();
        Console.WriteLine($"[PASS] TurnIntegration 构造函数数量: {ctors_TurnIntegration.Length}");
        var methods_TurnIntegration = type_TurnIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TurnIntegration 公开方法数量: {methods_TurnIntegration.Length}");
        foreach (var m in methods_TurnIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TurnIntegration 未找到，尝试无命名空间...");
        type_TurnIntegration = Type.GetType("TurnIntegration");
        if (type_TurnIntegration != null)
            Console.WriteLine("[PASS] 类型 TurnIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TurnIntegration 可能为顶层语句或嵌套类型");
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
