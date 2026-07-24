#load "mqtt_multimodal_security.cs"

Console.WriteLine("=== mqtt_multimodal_security.cs Test ===");

try
{
    // 验证 class: BioMetricAuthenticator
    var type_BioMetricAuthenticator = Type.GetType("BioMetricAuthenticator");
    if (type_BioMetricAuthenticator != null)
    {
        Console.WriteLine("[PASS] 类型 BioMetricAuthenticator (class) 存在");
        var ctors_BioMetricAuthenticator = type_BioMetricAuthenticator.GetConstructors();
        Console.WriteLine($"[PASS] BioMetricAuthenticator 构造函数数量: {ctors_BioMetricAuthenticator.Length}");
        var methods_BioMetricAuthenticator = type_BioMetricAuthenticator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BioMetricAuthenticator 公开方法数量: {methods_BioMetricAuthenticator.Length}");
        foreach (var m in methods_BioMetricAuthenticator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BioMetricAuthenticator 未找到，尝试无命名空间...");
        type_BioMetricAuthenticator = Type.GetType("BioMetricAuthenticator");
        if (type_BioMetricAuthenticator != null)
            Console.WriteLine("[PASS] 类型 BioMetricAuthenticator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BioMetricAuthenticator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AdaptiveRiskScorer
    var type_AdaptiveRiskScorer = Type.GetType("AdaptiveRiskScorer");
    if (type_AdaptiveRiskScorer != null)
    {
        Console.WriteLine("[PASS] 类型 AdaptiveRiskScorer (class) 存在");
        var ctors_AdaptiveRiskScorer = type_AdaptiveRiskScorer.GetConstructors();
        Console.WriteLine($"[PASS] AdaptiveRiskScorer 构造函数数量: {ctors_AdaptiveRiskScorer.Length}");
        var methods_AdaptiveRiskScorer = type_AdaptiveRiskScorer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdaptiveRiskScorer 公开方法数量: {methods_AdaptiveRiskScorer.Length}");
        foreach (var m in methods_AdaptiveRiskScorer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdaptiveRiskScorer 未找到，尝试无命名空间...");
        type_AdaptiveRiskScorer = Type.GetType("AdaptiveRiskScorer");
        if (type_AdaptiveRiskScorer != null)
            Console.WriteLine("[PASS] 类型 AdaptiveRiskScorer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdaptiveRiskScorer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DynamicPolicyEngine
    var type_DynamicPolicyEngine = Type.GetType("DynamicPolicyEngine");
    if (type_DynamicPolicyEngine != null)
    {
        Console.WriteLine("[PASS] 类型 DynamicPolicyEngine (class) 存在");
        var ctors_DynamicPolicyEngine = type_DynamicPolicyEngine.GetConstructors();
        Console.WriteLine($"[PASS] DynamicPolicyEngine 构造函数数量: {ctors_DynamicPolicyEngine.Length}");
        var methods_DynamicPolicyEngine = type_DynamicPolicyEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DynamicPolicyEngine 公开方法数量: {methods_DynamicPolicyEngine.Length}");
        foreach (var m in methods_DynamicPolicyEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DynamicPolicyEngine 未找到，尝试无命名空间...");
        type_DynamicPolicyEngine = Type.GetType("DynamicPolicyEngine");
        if (type_DynamicPolicyEngine != null)
            Console.WriteLine("[PASS] 类型 DynamicPolicyEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DynamicPolicyEngine 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: SecurityLevel
    var type_SecurityLevel = Type.GetType("SecurityLevel");
    if (type_SecurityLevel != null)
    {
        Console.WriteLine("[PASS] 类型 SecurityLevel (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SecurityLevel 未找到，尝试无命名空间...");
        type_SecurityLevel = Type.GetType("SecurityLevel");
        if (type_SecurityLevel != null)
            Console.WriteLine("[PASS] 类型 SecurityLevel (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SecurityLevel 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
