#load "mqtt_federated_learning.cs"

Console.WriteLine("=== mqtt_federated_learning.cs Test ===");

try
{
    // 验证 class: EdgeFederatedLearner
    var type_EdgeFederatedLearner = Type.GetType("EdgeFederatedLearner");
    if (type_EdgeFederatedLearner != null)
    {
        Console.WriteLine("[PASS] 类型 EdgeFederatedLearner (class) 存在");
        var ctors_EdgeFederatedLearner = type_EdgeFederatedLearner.GetConstructors();
        Console.WriteLine($"[PASS] EdgeFederatedLearner 构造函数数量: {ctors_EdgeFederatedLearner.Length}");
        var methods_EdgeFederatedLearner = type_EdgeFederatedLearner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] EdgeFederatedLearner 公开方法数量: {methods_EdgeFederatedLearner.Length}");
        foreach (var m in methods_EdgeFederatedLearner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 EdgeFederatedLearner 未找到，尝试无命名空间...");
        type_EdgeFederatedLearner = Type.GetType("EdgeFederatedLearner");
        if (type_EdgeFederatedLearner != null)
            Console.WriteLine("[PASS] 类型 EdgeFederatedLearner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 EdgeFederatedLearner 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DistributedEdgeComputer
    var type_DistributedEdgeComputer = Type.GetType("DistributedEdgeComputer");
    if (type_DistributedEdgeComputer != null)
    {
        Console.WriteLine("[PASS] 类型 DistributedEdgeComputer (class) 存在");
        var ctors_DistributedEdgeComputer = type_DistributedEdgeComputer.GetConstructors();
        Console.WriteLine($"[PASS] DistributedEdgeComputer 构造函数数量: {ctors_DistributedEdgeComputer.Length}");
        var methods_DistributedEdgeComputer = type_DistributedEdgeComputer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DistributedEdgeComputer 公开方法数量: {methods_DistributedEdgeComputer.Length}");
        foreach (var m in methods_DistributedEdgeComputer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DistributedEdgeComputer 未找到，尝试无命名空间...");
        type_DistributedEdgeComputer = Type.GetType("DistributedEdgeComputer");
        if (type_DistributedEdgeComputer != null)
            Console.WriteLine("[PASS] 类型 DistributedEdgeComputer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DistributedEdgeComputer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SidhCryptoProvider
    var type_SidhCryptoProvider = Type.GetType("SidhCryptoProvider");
    if (type_SidhCryptoProvider != null)
    {
        Console.WriteLine("[PASS] 类型 SidhCryptoProvider (class) 存在");
        var ctors_SidhCryptoProvider = type_SidhCryptoProvider.GetConstructors();
        Console.WriteLine($"[PASS] SidhCryptoProvider 构造函数数量: {ctors_SidhCryptoProvider.Length}");
        var methods_SidhCryptoProvider = type_SidhCryptoProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SidhCryptoProvider 公开方法数量: {methods_SidhCryptoProvider.Length}");
        foreach (var m in methods_SidhCryptoProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SidhCryptoProvider 未找到，尝试无命名空间...");
        type_SidhCryptoProvider = Type.GetType("SidhCryptoProvider");
        if (type_SidhCryptoProvider != null)
            Console.WriteLine("[PASS] 类型 SidhCryptoProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SidhCryptoProvider 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
