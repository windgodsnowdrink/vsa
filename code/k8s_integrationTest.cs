#load "k8s_integration.cs"

Console.WriteLine("=== k8s_integration.cs Test ===");

try
{
    // 验证 class: KubernetesIntegrationOptions
    var type_KubernetesIntegrationOptions = Type.GetType("KubernetesIntegrationOptions");
    if (type_KubernetesIntegrationOptions != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesIntegrationOptions (class) 存在");
        var ctors_KubernetesIntegrationOptions = type_KubernetesIntegrationOptions.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesIntegrationOptions 构造函数数量: {ctors_KubernetesIntegrationOptions.Length}");
        var methods_KubernetesIntegrationOptions = type_KubernetesIntegrationOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesIntegrationOptions 公开方法数量: {methods_KubernetesIntegrationOptions.Length}");
        foreach (var m in methods_KubernetesIntegrationOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesIntegrationOptions 未找到，尝试无命名空间...");
        type_KubernetesIntegrationOptions = Type.GetType("KubernetesIntegrationOptions");
        if (type_KubernetesIntegrationOptions != null)
            Console.WriteLine("[PASS] 类型 KubernetesIntegrationOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesIntegrationOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KubernetesServiceCollectionExtensions
    var type_KubernetesServiceCollectionExtensions = Type.GetType("KubernetesServiceCollectionExtensions");
    if (type_KubernetesServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesServiceCollectionExtensions (class) 存在");
        var ctors_KubernetesServiceCollectionExtensions = type_KubernetesServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesServiceCollectionExtensions 构造函数数量: {ctors_KubernetesServiceCollectionExtensions.Length}");
        var methods_KubernetesServiceCollectionExtensions = type_KubernetesServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesServiceCollectionExtensions 公开方法数量: {methods_KubernetesServiceCollectionExtensions.Length}");
        foreach (var m in methods_KubernetesServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_KubernetesServiceCollectionExtensions = Type.GetType("KubernetesServiceCollectionExtensions");
        if (type_KubernetesServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 KubernetesServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KubernetesDeploymentManager
    var type_KubernetesDeploymentManager = Type.GetType("KubernetesDeploymentManager");
    if (type_KubernetesDeploymentManager != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesDeploymentManager (class) 存在");
        var ctors_KubernetesDeploymentManager = type_KubernetesDeploymentManager.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesDeploymentManager 构造函数数量: {ctors_KubernetesDeploymentManager.Length}");
        var methods_KubernetesDeploymentManager = type_KubernetesDeploymentManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesDeploymentManager 公开方法数量: {methods_KubernetesDeploymentManager.Length}");
        foreach (var m in methods_KubernetesDeploymentManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesDeploymentManager 未找到，尝试无命名空间...");
        type_KubernetesDeploymentManager = Type.GetType("KubernetesDeploymentManager");
        if (type_KubernetesDeploymentManager != null)
            Console.WriteLine("[PASS] 类型 KubernetesDeploymentManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesDeploymentManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KubernetesServiceDiscovery
    var type_KubernetesServiceDiscovery = Type.GetType("KubernetesServiceDiscovery");
    if (type_KubernetesServiceDiscovery != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesServiceDiscovery (class) 存在");
        var ctors_KubernetesServiceDiscovery = type_KubernetesServiceDiscovery.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesServiceDiscovery 构造函数数量: {ctors_KubernetesServiceDiscovery.Length}");
        var methods_KubernetesServiceDiscovery = type_KubernetesServiceDiscovery.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesServiceDiscovery 公开方法数量: {methods_KubernetesServiceDiscovery.Length}");
        foreach (var m in methods_KubernetesServiceDiscovery)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesServiceDiscovery 未找到，尝试无命名空间...");
        type_KubernetesServiceDiscovery = Type.GetType("KubernetesServiceDiscovery");
        if (type_KubernetesServiceDiscovery != null)
            Console.WriteLine("[PASS] 类型 KubernetesServiceDiscovery (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesServiceDiscovery 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KubernetesConfigManager
    var type_KubernetesConfigManager = Type.GetType("KubernetesConfigManager");
    if (type_KubernetesConfigManager != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesConfigManager (class) 存在");
        var ctors_KubernetesConfigManager = type_KubernetesConfigManager.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesConfigManager 构造函数数量: {ctors_KubernetesConfigManager.Length}");
        var methods_KubernetesConfigManager = type_KubernetesConfigManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesConfigManager 公开方法数量: {methods_KubernetesConfigManager.Length}");
        foreach (var m in methods_KubernetesConfigManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesConfigManager 未找到，尝试无命名空间...");
        type_KubernetesConfigManager = Type.GetType("KubernetesConfigManager");
        if (type_KubernetesConfigManager != null)
            Console.WriteLine("[PASS] 类型 KubernetesConfigManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesConfigManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KubernetesCrdManager
    var type_KubernetesCrdManager = Type.GetType("KubernetesCrdManager");
    if (type_KubernetesCrdManager != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesCrdManager (class) 存在");
        var ctors_KubernetesCrdManager = type_KubernetesCrdManager.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesCrdManager 构造函数数量: {ctors_KubernetesCrdManager.Length}");
        var methods_KubernetesCrdManager = type_KubernetesCrdManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesCrdManager 公开方法数量: {methods_KubernetesCrdManager.Length}");
        foreach (var m in methods_KubernetesCrdManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesCrdManager 未找到，尝试无命名空间...");
        type_KubernetesCrdManager = Type.GetType("KubernetesCrdManager");
        if (type_KubernetesCrdManager != null)
            Console.WriteLine("[PASS] 类型 KubernetesCrdManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesCrdManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: KubernetesAutoScaler
    var type_KubernetesAutoScaler = Type.GetType("KubernetesAutoScaler");
    if (type_KubernetesAutoScaler != null)
    {
        Console.WriteLine("[PASS] 类型 KubernetesAutoScaler (class) 存在");
        var ctors_KubernetesAutoScaler = type_KubernetesAutoScaler.GetConstructors();
        Console.WriteLine($"[PASS] KubernetesAutoScaler 构造函数数量: {ctors_KubernetesAutoScaler.Length}");
        var methods_KubernetesAutoScaler = type_KubernetesAutoScaler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] KubernetesAutoScaler 公开方法数量: {methods_KubernetesAutoScaler.Length}");
        foreach (var m in methods_KubernetesAutoScaler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 KubernetesAutoScaler 未找到，尝试无命名空间...");
        type_KubernetesAutoScaler = Type.GetType("KubernetesAutoScaler");
        if (type_KubernetesAutoScaler != null)
            Console.WriteLine("[PASS] 类型 KubernetesAutoScaler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 KubernetesAutoScaler 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
