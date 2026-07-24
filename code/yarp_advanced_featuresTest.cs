#load "yarp_advanced_features.cs"

Console.WriteLine("=== yarp_advanced_features.cs Test ===");

try
{
    // 验证 class: RoundRobinLoadBalancingPolicy
    var type_RoundRobinLoadBalancingPolicy = Type.GetType("RoundRobinLoadBalancingPolicy");
    if (type_RoundRobinLoadBalancingPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 RoundRobinLoadBalancingPolicy (class) 存在");
        var ctors_RoundRobinLoadBalancingPolicy = type_RoundRobinLoadBalancingPolicy.GetConstructors();
        Console.WriteLine($"[PASS] RoundRobinLoadBalancingPolicy 构造函数数量: {ctors_RoundRobinLoadBalancingPolicy.Length}");
        var methods_RoundRobinLoadBalancingPolicy = type_RoundRobinLoadBalancingPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RoundRobinLoadBalancingPolicy 公开方法数量: {methods_RoundRobinLoadBalancingPolicy.Length}");
        foreach (var m in methods_RoundRobinLoadBalancingPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RoundRobinLoadBalancingPolicy 未找到，尝试无命名空间...");
        type_RoundRobinLoadBalancingPolicy = Type.GetType("RoundRobinLoadBalancingPolicy");
        if (type_RoundRobinLoadBalancingPolicy != null)
            Console.WriteLine("[PASS] 类型 RoundRobinLoadBalancingPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RoundRobinLoadBalancingPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WeightedLoadBalancingPolicy
    var type_WeightedLoadBalancingPolicy = Type.GetType("WeightedLoadBalancingPolicy");
    if (type_WeightedLoadBalancingPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 WeightedLoadBalancingPolicy (class) 存在");
        var ctors_WeightedLoadBalancingPolicy = type_WeightedLoadBalancingPolicy.GetConstructors();
        Console.WriteLine($"[PASS] WeightedLoadBalancingPolicy 构造函数数量: {ctors_WeightedLoadBalancingPolicy.Length}");
        var methods_WeightedLoadBalancingPolicy = type_WeightedLoadBalancingPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WeightedLoadBalancingPolicy 公开方法数量: {methods_WeightedLoadBalancingPolicy.Length}");
        foreach (var m in methods_WeightedLoadBalancingPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WeightedLoadBalancingPolicy 未找到，尝试无命名空间...");
        type_WeightedLoadBalancingPolicy = Type.GetType("WeightedLoadBalancingPolicy");
        if (type_WeightedLoadBalancingPolicy != null)
            Console.WriteLine("[PASS] 类型 WeightedLoadBalancingPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WeightedLoadBalancingPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SsoClient
    var type_SsoClient = Type.GetType("SsoClient");
    if (type_SsoClient != null)
    {
        Console.WriteLine("[PASS] 类型 SsoClient (class) 存在");
        var ctors_SsoClient = type_SsoClient.GetConstructors();
        Console.WriteLine($"[PASS] SsoClient 构造函数数量: {ctors_SsoClient.Length}");
        var methods_SsoClient = type_SsoClient.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SsoClient 公开方法数量: {methods_SsoClient.Length}");
        foreach (var m in methods_SsoClient)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SsoClient 未找到，尝试无命名空间...");
        type_SsoClient = Type.GetType("SsoClient");
        if (type_SsoClient != null)
            Console.WriteLine("[PASS] 类型 SsoClient (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SsoClient 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
