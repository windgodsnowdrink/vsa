#load "yarp_service_discovery.cs"

Console.WriteLine("=== yarp_service_discovery.cs Test ===");

try
{
    // 验证 class: ServiceRegistryMiddleware
    var type_ServiceRegistryMiddleware = Type.GetType("ServiceRegistryMiddleware");
    if (type_ServiceRegistryMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceRegistryMiddleware (class) 存在");
        var ctors_ServiceRegistryMiddleware = type_ServiceRegistryMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] ServiceRegistryMiddleware 构造函数数量: {ctors_ServiceRegistryMiddleware.Length}");
        var methods_ServiceRegistryMiddleware = type_ServiceRegistryMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceRegistryMiddleware 公开方法数量: {methods_ServiceRegistryMiddleware.Length}");
        foreach (var m in methods_ServiceRegistryMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceRegistryMiddleware 未找到，尝试无命名空间...");
        type_ServiceRegistryMiddleware = Type.GetType("ServiceRegistryMiddleware");
        if (type_ServiceRegistryMiddleware != null)
            Console.WriteLine("[PASS] 类型 ServiceRegistryMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceRegistryMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DiscoveryClientPooledPolicy
    var type_DiscoveryClientPooledPolicy = Type.GetType("DiscoveryClientPooledPolicy");
    if (type_DiscoveryClientPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DiscoveryClientPooledPolicy (class) 存在");
        var ctors_DiscoveryClientPooledPolicy = type_DiscoveryClientPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DiscoveryClientPooledPolicy 构造函数数量: {ctors_DiscoveryClientPooledPolicy.Length}");
        var methods_DiscoveryClientPooledPolicy = type_DiscoveryClientPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DiscoveryClientPooledPolicy 公开方法数量: {methods_DiscoveryClientPooledPolicy.Length}");
        foreach (var m in methods_DiscoveryClientPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DiscoveryClientPooledPolicy 未找到，尝试无命名空间...");
        type_DiscoveryClientPooledPolicy = Type.GetType("DiscoveryClientPooledPolicy");
        if (type_DiscoveryClientPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 DiscoveryClientPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DiscoveryClientPooledPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
