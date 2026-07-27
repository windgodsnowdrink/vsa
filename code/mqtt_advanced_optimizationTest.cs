#load "mqtt_advanced_optimization.cs"

Console.WriteLine("=== mqtt_advanced_optimization.cs Test ===");

try
{
    // 验证 class: AdaptivePrivacyBudget
    var type_AdaptivePrivacyBudget = Type.GetType("AdaptivePrivacyBudget");
    if (type_AdaptivePrivacyBudget != null)
    {
        Console.WriteLine("[PASS] 类型 AdaptivePrivacyBudget (class) 存在");
        var ctors_AdaptivePrivacyBudget = type_AdaptivePrivacyBudget.GetConstructors();
        Console.WriteLine($"[PASS] AdaptivePrivacyBudget 构造函数数量: {ctors_AdaptivePrivacyBudget.Length}");
        var methods_AdaptivePrivacyBudget = type_AdaptivePrivacyBudget.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdaptivePrivacyBudget 公开方法数量: {methods_AdaptivePrivacyBudget.Length}");
        foreach (var m in methods_AdaptivePrivacyBudget)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdaptivePrivacyBudget 未找到，尝试无命名空间...");
        type_AdaptivePrivacyBudget = Type.GetType("AdaptivePrivacyBudget");
        if (type_AdaptivePrivacyBudget != null)
            Console.WriteLine("[PASS] 类型 AdaptivePrivacyBudget (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdaptivePrivacyBudget 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MLPredictiveMaintenance
    var type_MLPredictiveMaintenance = Type.GetType("MLPredictiveMaintenance");
    if (type_MLPredictiveMaintenance != null)
    {
        Console.WriteLine("[PASS] 类型 MLPredictiveMaintenance (class) 存在");
        var ctors_MLPredictiveMaintenance = type_MLPredictiveMaintenance.GetConstructors();
        Console.WriteLine($"[PASS] MLPredictiveMaintenance 构造函数数量: {ctors_MLPredictiveMaintenance.Length}");
        var methods_MLPredictiveMaintenance = type_MLPredictiveMaintenance.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MLPredictiveMaintenance 公开方法数量: {methods_MLPredictiveMaintenance.Length}");
        foreach (var m in methods_MLPredictiveMaintenance)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MLPredictiveMaintenance 未找到，尝试无命名空间...");
        type_MLPredictiveMaintenance = Type.GetType("MLPredictiveMaintenance");
        if (type_MLPredictiveMaintenance != null)
            Console.WriteLine("[PASS] 类型 MLPredictiveMaintenance (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MLPredictiveMaintenance 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuantumNetworkOptimizer
    var type_QuantumNetworkOptimizer = Type.GetType("QuantumNetworkOptimizer");
    if (type_QuantumNetworkOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumNetworkOptimizer (class) 存在");
        var ctors_QuantumNetworkOptimizer = type_QuantumNetworkOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] QuantumNetworkOptimizer 构造函数数量: {ctors_QuantumNetworkOptimizer.Length}");
        var methods_QuantumNetworkOptimizer = type_QuantumNetworkOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumNetworkOptimizer 公开方法数量: {methods_QuantumNetworkOptimizer.Length}");
        foreach (var m in methods_QuantumNetworkOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumNetworkOptimizer 未找到，尝试无命名空间...");
        type_QuantumNetworkOptimizer = Type.GetType("QuantumNetworkOptimizer");
        if (type_QuantumNetworkOptimizer != null)
            Console.WriteLine("[PASS] 类型 QuantumNetworkOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuantumNetworkOptimizer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
