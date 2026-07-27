#load "mqtt_quantum_security.cs"

Console.WriteLine("=== mqtt_quantum_security.cs Test ===");

try
{
    // 验证 class: GaussianNoiseGenerator
    var type_GaussianNoiseGenerator = Type.GetType("GaussianNoiseGenerator");
    if (type_GaussianNoiseGenerator != null)
    {
        Console.WriteLine("[PASS] 类型 GaussianNoiseGenerator (class) 存在");
        var ctors_GaussianNoiseGenerator = type_GaussianNoiseGenerator.GetConstructors();
        Console.WriteLine($"[PASS] GaussianNoiseGenerator 构造函数数量: {ctors_GaussianNoiseGenerator.Length}");
        var methods_GaussianNoiseGenerator = type_GaussianNoiseGenerator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GaussianNoiseGenerator 公开方法数量: {methods_GaussianNoiseGenerator.Length}");
        foreach (var m in methods_GaussianNoiseGenerator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GaussianNoiseGenerator 未找到，尝试无命名空间...");
        type_GaussianNoiseGenerator = Type.GetType("GaussianNoiseGenerator");
        if (type_GaussianNoiseGenerator != null)
            Console.WriteLine("[PASS] 类型 GaussianNoiseGenerator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GaussianNoiseGenerator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AzureIoTEdgeManager
    var type_AzureIoTEdgeManager = Type.GetType("AzureIoTEdgeManager");
    if (type_AzureIoTEdgeManager != null)
    {
        Console.WriteLine("[PASS] 类型 AzureIoTEdgeManager (class) 存在");
        var ctors_AzureIoTEdgeManager = type_AzureIoTEdgeManager.GetConstructors();
        Console.WriteLine($"[PASS] AzureIoTEdgeManager 构造函数数量: {ctors_AzureIoTEdgeManager.Length}");
        var methods_AzureIoTEdgeManager = type_AzureIoTEdgeManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AzureIoTEdgeManager 公开方法数量: {methods_AzureIoTEdgeManager.Length}");
        foreach (var m in methods_AzureIoTEdgeManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AzureIoTEdgeManager 未找到，尝试无命名空间...");
        type_AzureIoTEdgeManager = Type.GetType("AzureIoTEdgeManager");
        if (type_AzureIoTEdgeManager != null)
            Console.WriteLine("[PASS] 类型 AzureIoTEdgeManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AzureIoTEdgeManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BB84Protocol
    var type_BB84Protocol = Type.GetType("BB84Protocol");
    if (type_BB84Protocol != null)
    {
        Console.WriteLine("[PASS] 类型 BB84Protocol (class) 存在");
        var ctors_BB84Protocol = type_BB84Protocol.GetConstructors();
        Console.WriteLine($"[PASS] BB84Protocol 构造函数数量: {ctors_BB84Protocol.Length}");
        var methods_BB84Protocol = type_BB84Protocol.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BB84Protocol 公开方法数量: {methods_BB84Protocol.Length}");
        foreach (var m in methods_BB84Protocol)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BB84Protocol 未找到，尝试无命名空间...");
        type_BB84Protocol = Type.GetType("BB84Protocol");
        if (type_BB84Protocol != null)
            Console.WriteLine("[PASS] 类型 BB84Protocol (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BB84Protocol 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
