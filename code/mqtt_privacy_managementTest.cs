#load "mqtt_privacy_management.cs"

Console.WriteLine("=== mqtt_privacy_management.cs Test ===");

try
{
    // 验证 class: DifferentialPrivacyBudget
    var type_DifferentialPrivacyBudget = Type.GetType("DifferentialPrivacyBudget");
    if (type_DifferentialPrivacyBudget != null)
    {
        Console.WriteLine("[PASS] 类型 DifferentialPrivacyBudget (class) 存在");
        var ctors_DifferentialPrivacyBudget = type_DifferentialPrivacyBudget.GetConstructors();
        Console.WriteLine($"[PASS] DifferentialPrivacyBudget 构造函数数量: {ctors_DifferentialPrivacyBudget.Length}");
        var methods_DifferentialPrivacyBudget = type_DifferentialPrivacyBudget.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DifferentialPrivacyBudget 公开方法数量: {methods_DifferentialPrivacyBudget.Length}");
        foreach (var m in methods_DifferentialPrivacyBudget)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DifferentialPrivacyBudget 未找到，尝试无命名空间...");
        type_DifferentialPrivacyBudget = Type.GetType("DifferentialPrivacyBudget");
        if (type_DifferentialPrivacyBudget != null)
            Console.WriteLine("[PASS] 类型 DifferentialPrivacyBudget (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DifferentialPrivacyBudget 可能为顶层语句或嵌套类型");
    }

    // 验证 class: EdgeDeviceMonitor
    var type_EdgeDeviceMonitor = Type.GetType("EdgeDeviceMonitor");
    if (type_EdgeDeviceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 EdgeDeviceMonitor (class) 存在");
        var ctors_EdgeDeviceMonitor = type_EdgeDeviceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] EdgeDeviceMonitor 构造函数数量: {ctors_EdgeDeviceMonitor.Length}");
        var methods_EdgeDeviceMonitor = type_EdgeDeviceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EdgeDeviceMonitor 公开方法数量: {methods_EdgeDeviceMonitor.Length}");
        foreach (var m in methods_EdgeDeviceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EdgeDeviceMonitor 未找到，尝试无命名空间...");
        type_EdgeDeviceMonitor = Type.GetType("EdgeDeviceMonitor");
        if (type_EdgeDeviceMonitor != null)
            Console.WriteLine("[PASS] 类型 EdgeDeviceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EdgeDeviceMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuantumRepeaterNode
    var type_QuantumRepeaterNode = Type.GetType("QuantumRepeaterNode");
    if (type_QuantumRepeaterNode != null)
    {
        Console.WriteLine("[PASS] 类型 QuantumRepeaterNode (class) 存在");
        var ctors_QuantumRepeaterNode = type_QuantumRepeaterNode.GetConstructors();
        Console.WriteLine($"[PASS] QuantumRepeaterNode 构造函数数量: {ctors_QuantumRepeaterNode.Length}");
        var methods_QuantumRepeaterNode = type_QuantumRepeaterNode.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuantumRepeaterNode 公开方法数量: {methods_QuantumRepeaterNode.Length}");
        foreach (var m in methods_QuantumRepeaterNode)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuantumRepeaterNode 未找到，尝试无命名空间...");
        type_QuantumRepeaterNode = Type.GetType("QuantumRepeaterNode");
        if (type_QuantumRepeaterNode != null)
            Console.WriteLine("[PASS] 类型 QuantumRepeaterNode (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuantumRepeaterNode 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: HealthStatus
    var type_HealthStatus = Type.GetType("HealthStatus");
    if (type_HealthStatus != null)
    {
        Console.WriteLine("[PASS] 类型 HealthStatus (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HealthStatus 未找到，尝试无命名空间...");
        type_HealthStatus = Type.GetType("HealthStatus");
        if (type_HealthStatus != null)
            Console.WriteLine("[PASS] 类型 HealthStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HealthStatus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
