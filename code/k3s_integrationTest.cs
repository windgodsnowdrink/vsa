#load "k3s_integration.cs"

Console.WriteLine("=== k3s_integration.cs Test ===");

try
{
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

    // 验证 class: K3SOptions
    var type_K3SOptions = Type.GetType("K3SOptions");
    if (type_K3SOptions != null)
    {
        Console.WriteLine("[PASS] 类型 K3SOptions (class) 存在");
        var ctors_K3SOptions = type_K3SOptions.GetConstructors();
        Console.WriteLine($"[PASS] K3SOptions 构造函数数量: {ctors_K3SOptions.Length}");
        var methods_K3SOptions = type_K3SOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] K3SOptions 公开方法数量: {methods_K3SOptions.Length}");
        foreach (var m in methods_K3SOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 K3SOptions 未找到，尝试无命名空间...");
        type_K3SOptions = Type.GetType("K3SOptions");
        if (type_K3SOptions != null)
            Console.WriteLine("[PASS] 类型 K3SOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 K3SOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: K3SExtensions
    var type_K3SExtensions = Type.GetType("K3SExtensions");
    if (type_K3SExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 K3SExtensions (class) 存在");
        var ctors_K3SExtensions = type_K3SExtensions.GetConstructors();
        Console.WriteLine($"[PASS] K3SExtensions 构造函数数量: {ctors_K3SExtensions.Length}");
        var methods_K3SExtensions = type_K3SExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] K3SExtensions 公开方法数量: {methods_K3SExtensions.Length}");
        foreach (var m in methods_K3SExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 K3SExtensions 未找到，尝试无命名空间...");
        type_K3SExtensions = Type.GetType("K3SExtensions");
        if (type_K3SExtensions != null)
            Console.WriteLine("[PASS] 类型 K3SExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 K3SExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: K3SDeploymentManager
    var type_K3SDeploymentManager = Type.GetType("K3SDeploymentManager");
    if (type_K3SDeploymentManager != null)
    {
        Console.WriteLine("[PASS] 类型 K3SDeploymentManager (class) 存在");
        var ctors_K3SDeploymentManager = type_K3SDeploymentManager.GetConstructors();
        Console.WriteLine($"[PASS] K3SDeploymentManager 构造函数数量: {ctors_K3SDeploymentManager.Length}");
        var methods_K3SDeploymentManager = type_K3SDeploymentManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] K3SDeploymentManager 公开方法数量: {methods_K3SDeploymentManager.Length}");
        foreach (var m in methods_K3SDeploymentManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 K3SDeploymentManager 未找到，尝试无命名空间...");
        type_K3SDeploymentManager = Type.GetType("K3SDeploymentManager");
        if (type_K3SDeploymentManager != null)
            Console.WriteLine("[PASS] 类型 K3SDeploymentManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 K3SDeploymentManager 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
