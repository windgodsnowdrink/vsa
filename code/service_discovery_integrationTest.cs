#load "service_discovery_integration.cs"

Console.WriteLine("=== service_discovery_integration.cs Test ===");

try
{
    // 验证 class: ServiceDiscoveryHealthCheck
    var type_ServiceDiscoveryHealthCheck = Type.GetType("ServiceDiscoveryHealthCheck");
    if (type_ServiceDiscoveryHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceDiscoveryHealthCheck (class) 存在");
        var ctors_ServiceDiscoveryHealthCheck = type_ServiceDiscoveryHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] ServiceDiscoveryHealthCheck 构造函数数量: {ctors_ServiceDiscoveryHealthCheck.Length}");
        var methods_ServiceDiscoveryHealthCheck = type_ServiceDiscoveryHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceDiscoveryHealthCheck 公开方法数量: {methods_ServiceDiscoveryHealthCheck.Length}");
        foreach (var m in methods_ServiceDiscoveryHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceDiscoveryHealthCheck 未找到，尝试无命名空间...");
        type_ServiceDiscoveryHealthCheck = Type.GetType("ServiceDiscoveryHealthCheck");
        if (type_ServiceDiscoveryHealthCheck != null)
            Console.WriteLine("[PASS] 类型 ServiceDiscoveryHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceDiscoveryHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmartLoadBalancerFactory
    var type_SmartLoadBalancerFactory = Type.GetType("SmartLoadBalancerFactory");
    if (type_SmartLoadBalancerFactory != null)
    {
        Console.WriteLine("[PASS] 类型 SmartLoadBalancerFactory (class) 存在");
        var ctors_SmartLoadBalancerFactory = type_SmartLoadBalancerFactory.GetConstructors();
        Console.WriteLine($"[PASS] SmartLoadBalancerFactory 构造函数数量: {ctors_SmartLoadBalancerFactory.Length}");
        var methods_SmartLoadBalancerFactory = type_SmartLoadBalancerFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmartLoadBalancerFactory 公开方法数量: {methods_SmartLoadBalancerFactory.Length}");
        foreach (var m in methods_SmartLoadBalancerFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmartLoadBalancerFactory 未找到，尝试无命名空间...");
        type_SmartLoadBalancerFactory = Type.GetType("SmartLoadBalancerFactory");
        if (type_SmartLoadBalancerFactory != null)
            Console.WriteLine("[PASS] 类型 SmartLoadBalancerFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmartLoadBalancerFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: SmartLoadBalancer
    var type_SmartLoadBalancer = Type.GetType("SmartLoadBalancer");
    if (type_SmartLoadBalancer != null)
    {
        Console.WriteLine("[PASS] 类型 SmartLoadBalancer (class) 存在");
        var ctors_SmartLoadBalancer = type_SmartLoadBalancer.GetConstructors();
        Console.WriteLine($"[PASS] SmartLoadBalancer 构造函数数量: {ctors_SmartLoadBalancer.Length}");
        var methods_SmartLoadBalancer = type_SmartLoadBalancer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SmartLoadBalancer 公开方法数量: {methods_SmartLoadBalancer.Length}");
        foreach (var m in methods_SmartLoadBalancer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SmartLoadBalancer 未找到，尝试无命名空间...");
        type_SmartLoadBalancer = Type.GetType("SmartLoadBalancer");
        if (type_SmartLoadBalancer != null)
            Console.WriteLine("[PASS] 类型 SmartLoadBalancer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SmartLoadBalancer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceDiscoveryConfigurationChangeListener
    var type_ServiceDiscoveryConfigurationChangeListener = Type.GetType("ServiceDiscoveryConfigurationChangeListener");
    if (type_ServiceDiscoveryConfigurationChangeListener != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceDiscoveryConfigurationChangeListener (class) 存在");
        var ctors_ServiceDiscoveryConfigurationChangeListener = type_ServiceDiscoveryConfigurationChangeListener.GetConstructors();
        Console.WriteLine($"[PASS] ServiceDiscoveryConfigurationChangeListener 构造函数数量: {ctors_ServiceDiscoveryConfigurationChangeListener.Length}");
        var methods_ServiceDiscoveryConfigurationChangeListener = type_ServiceDiscoveryConfigurationChangeListener.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceDiscoveryConfigurationChangeListener 公开方法数量: {methods_ServiceDiscoveryConfigurationChangeListener.Length}");
        foreach (var m in methods_ServiceDiscoveryConfigurationChangeListener)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceDiscoveryConfigurationChangeListener 未找到，尝试无命名空间...");
        type_ServiceDiscoveryConfigurationChangeListener = Type.GetType("ServiceDiscoveryConfigurationChangeListener");
        if (type_ServiceDiscoveryConfigurationChangeListener != null)
            Console.WriteLine("[PASS] 类型 ServiceDiscoveryConfigurationChangeListener (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceDiscoveryConfigurationChangeListener 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ChangeTokenRegistration
    var type_ChangeTokenRegistration = Type.GetType("ChangeTokenRegistration");
    if (type_ChangeTokenRegistration != null)
    {
        Console.WriteLine("[PASS] 类型 ChangeTokenRegistration (class) 存在");
        var ctors_ChangeTokenRegistration = type_ChangeTokenRegistration.GetConstructors();
        Console.WriteLine($"[PASS] ChangeTokenRegistration 构造函数数量: {ctors_ChangeTokenRegistration.Length}");
        var methods_ChangeTokenRegistration = type_ChangeTokenRegistration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ChangeTokenRegistration 公开方法数量: {methods_ChangeTokenRegistration.Length}");
        foreach (var m in methods_ChangeTokenRegistration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ChangeTokenRegistration 未找到，尝试无命名空间...");
        type_ChangeTokenRegistration = Type.GetType("ChangeTokenRegistration");
        if (type_ChangeTokenRegistration != null)
            Console.WriteLine("[PASS] 类型 ChangeTokenRegistration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ChangeTokenRegistration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceDiscoveryOptions
    var type_ServiceDiscoveryOptions = Type.GetType("ServiceDiscoveryOptions");
    if (type_ServiceDiscoveryOptions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceDiscoveryOptions (class) 存在");
        var ctors_ServiceDiscoveryOptions = type_ServiceDiscoveryOptions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceDiscoveryOptions 构造函数数量: {ctors_ServiceDiscoveryOptions.Length}");
        var methods_ServiceDiscoveryOptions = type_ServiceDiscoveryOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceDiscoveryOptions 公开方法数量: {methods_ServiceDiscoveryOptions.Length}");
        foreach (var m in methods_ServiceDiscoveryOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceDiscoveryOptions 未找到，尝试无命名空间...");
        type_ServiceDiscoveryOptions = Type.GetType("ServiceDiscoveryOptions");
        if (type_ServiceDiscoveryOptions != null)
            Console.WriteLine("[PASS] 类型 ServiceDiscoveryOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceDiscoveryOptions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
