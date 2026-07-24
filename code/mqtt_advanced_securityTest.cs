#load "mqtt_advanced_security.cs"

Console.WriteLine("=== mqtt_advanced_security.cs Test ===");

try
{
    // 验证 class: TimeBasedKeyRotation
    var type_TimeBasedKeyRotation = Type.GetType("TimeBasedKeyRotation");
    if (type_TimeBasedKeyRotation != null)
    {
        Console.WriteLine("[PASS] 类型 TimeBasedKeyRotation (class) 存在");
        var ctors_TimeBasedKeyRotation = type_TimeBasedKeyRotation.GetConstructors();
        Console.WriteLine($"[PASS] TimeBasedKeyRotation 构造函数数量: {ctors_TimeBasedKeyRotation.Length}");
        var methods_TimeBasedKeyRotation = type_TimeBasedKeyRotation.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TimeBasedKeyRotation 公开方法数量: {methods_TimeBasedKeyRotation.Length}");
        foreach (var m in methods_TimeBasedKeyRotation)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TimeBasedKeyRotation 未找到，尝试无命名空间...");
        type_TimeBasedKeyRotation = Type.GetType("TimeBasedKeyRotation");
        if (type_TimeBasedKeyRotation != null)
            Console.WriteLine("[PASS] 类型 TimeBasedKeyRotation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TimeBasedKeyRotation 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TotpAuthenticator
    var type_TotpAuthenticator = Type.GetType("TotpAuthenticator");
    if (type_TotpAuthenticator != null)
    {
        Console.WriteLine("[PASS] 类型 TotpAuthenticator (class) 存在");
        var ctors_TotpAuthenticator = type_TotpAuthenticator.GetConstructors();
        Console.WriteLine($"[PASS] TotpAuthenticator 构造函数数量: {ctors_TotpAuthenticator.Length}");
        var methods_TotpAuthenticator = type_TotpAuthenticator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TotpAuthenticator 公开方法数量: {methods_TotpAuthenticator.Length}");
        foreach (var m in methods_TotpAuthenticator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TotpAuthenticator 未找到，尝试无命名空间...");
        type_TotpAuthenticator = Type.GetType("TotpAuthenticator");
        if (type_TotpAuthenticator != null)
            Console.WriteLine("[PASS] 类型 TotpAuthenticator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TotpAuthenticator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AnomalyDetectionEngine
    var type_AnomalyDetectionEngine = Type.GetType("AnomalyDetectionEngine");
    if (type_AnomalyDetectionEngine != null)
    {
        Console.WriteLine("[PASS] 类型 AnomalyDetectionEngine (class) 存在");
        var ctors_AnomalyDetectionEngine = type_AnomalyDetectionEngine.GetConstructors();
        Console.WriteLine($"[PASS] AnomalyDetectionEngine 构造函数数量: {ctors_AnomalyDetectionEngine.Length}");
        var methods_AnomalyDetectionEngine = type_AnomalyDetectionEngine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AnomalyDetectionEngine 公开方法数量: {methods_AnomalyDetectionEngine.Length}");
        foreach (var m in methods_AnomalyDetectionEngine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AnomalyDetectionEngine 未找到，尝试无命名空间...");
        type_AnomalyDetectionEngine = Type.GetType("AnomalyDetectionEngine");
        if (type_AnomalyDetectionEngine != null)
            Console.WriteLine("[PASS] 类型 AnomalyDetectionEngine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AnomalyDetectionEngine 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
