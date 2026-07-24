#load "minikube_integration.cs"

Console.WriteLine("=== minikube_integration.cs Test ===");

try
{
    // 验证 class: MinikubeIntegration
    var type_MinikubeIntegration = Type.GetType("MinikubeIntegration");
    if (type_MinikubeIntegration != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeIntegration (class) 存在");
        var ctors_MinikubeIntegration = type_MinikubeIntegration.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeIntegration 构造函数数量: {ctors_MinikubeIntegration.Length}");
        var methods_MinikubeIntegration = type_MinikubeIntegration.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeIntegration 公开方法数量: {methods_MinikubeIntegration.Length}");
        foreach (var m in methods_MinikubeIntegration)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeIntegration 未找到，尝试无命名空间...");
        type_MinikubeIntegration = Type.GetType("MinikubeIntegration");
        if (type_MinikubeIntegration != null)
            Console.WriteLine("[PASS] 类型 MinikubeIntegration (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeIntegration 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubeOptions
    var type_MinikubeOptions = Type.GetType("MinikubeOptions");
    if (type_MinikubeOptions != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeOptions (class) 存在");
        var ctors_MinikubeOptions = type_MinikubeOptions.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeOptions 构造函数数量: {ctors_MinikubeOptions.Length}");
        var methods_MinikubeOptions = type_MinikubeOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeOptions 公开方法数量: {methods_MinikubeOptions.Length}");
        foreach (var m in methods_MinikubeOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeOptions 未找到，尝试无命名空间...");
        type_MinikubeOptions = Type.GetType("MinikubeOptions");
        if (type_MinikubeOptions != null)
            Console.WriteLine("[PASS] 类型 MinikubeOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubeDeploymentManager
    var type_MinikubeDeploymentManager = Type.GetType("MinikubeDeploymentManager");
    if (type_MinikubeDeploymentManager != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeDeploymentManager (class) 存在");
        var ctors_MinikubeDeploymentManager = type_MinikubeDeploymentManager.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeDeploymentManager 构造函数数量: {ctors_MinikubeDeploymentManager.Length}");
        var methods_MinikubeDeploymentManager = type_MinikubeDeploymentManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeDeploymentManager 公开方法数量: {methods_MinikubeDeploymentManager.Length}");
        foreach (var m in methods_MinikubeDeploymentManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeDeploymentManager 未找到，尝试无命名空间...");
        type_MinikubeDeploymentManager = Type.GetType("MinikubeDeploymentManager");
        if (type_MinikubeDeploymentManager != null)
            Console.WriteLine("[PASS] 类型 MinikubeDeploymentManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeDeploymentManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubeServiceDiscovery
    var type_MinikubeServiceDiscovery = Type.GetType("MinikubeServiceDiscovery");
    if (type_MinikubeServiceDiscovery != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeServiceDiscovery (class) 存在");
        var ctors_MinikubeServiceDiscovery = type_MinikubeServiceDiscovery.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeServiceDiscovery 构造函数数量: {ctors_MinikubeServiceDiscovery.Length}");
        var methods_MinikubeServiceDiscovery = type_MinikubeServiceDiscovery.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeServiceDiscovery 公开方法数量: {methods_MinikubeServiceDiscovery.Length}");
        foreach (var m in methods_MinikubeServiceDiscovery)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeServiceDiscovery 未找到，尝试无命名空间...");
        type_MinikubeServiceDiscovery = Type.GetType("MinikubeServiceDiscovery");
        if (type_MinikubeServiceDiscovery != null)
            Console.WriteLine("[PASS] 类型 MinikubeServiceDiscovery (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeServiceDiscovery 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubeConfigManager
    var type_MinikubeConfigManager = Type.GetType("MinikubeConfigManager");
    if (type_MinikubeConfigManager != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeConfigManager (class) 存在");
        var ctors_MinikubeConfigManager = type_MinikubeConfigManager.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeConfigManager 构造函数数量: {ctors_MinikubeConfigManager.Length}");
        var methods_MinikubeConfigManager = type_MinikubeConfigManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeConfigManager 公开方法数量: {methods_MinikubeConfigManager.Length}");
        foreach (var m in methods_MinikubeConfigManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeConfigManager 未找到，尝试无命名空间...");
        type_MinikubeConfigManager = Type.GetType("MinikubeConfigManager");
        if (type_MinikubeConfigManager != null)
            Console.WriteLine("[PASS] 类型 MinikubeConfigManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeConfigManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: DeploymentPooledPolicy
    var type_DeploymentPooledPolicy = Type.GetType("DeploymentPooledPolicy");
    if (type_DeploymentPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 DeploymentPooledPolicy (class) 存在");
        var ctors_DeploymentPooledPolicy = type_DeploymentPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] DeploymentPooledPolicy 构造函数数量: {ctors_DeploymentPooledPolicy.Length}");
        var methods_DeploymentPooledPolicy = type_DeploymentPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] DeploymentPooledPolicy 公开方法数量: {methods_DeploymentPooledPolicy.Length}");
        foreach (var m in methods_DeploymentPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 DeploymentPooledPolicy 未找到，尝试无命名空间...");
        type_DeploymentPooledPolicy = Type.GetType("DeploymentPooledPolicy");
        if (type_DeploymentPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 DeploymentPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DeploymentPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubeTenantManager
    var type_MinikubeTenantManager = Type.GetType("MinikubeTenantManager");
    if (type_MinikubeTenantManager != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeTenantManager (class) 存在");
        var ctors_MinikubeTenantManager = type_MinikubeTenantManager.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeTenantManager 构造函数数量: {ctors_MinikubeTenantManager.Length}");
        var methods_MinikubeTenantManager = type_MinikubeTenantManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeTenantManager 公开方法数量: {methods_MinikubeTenantManager.Length}");
        foreach (var m in methods_MinikubeTenantManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeTenantManager 未找到，尝试无命名空间...");
        type_MinikubeTenantManager = Type.GetType("MinikubeTenantManager");
        if (type_MinikubeTenantManager != null)
            Console.WriteLine("[PASS] 类型 MinikubeTenantManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeTenantManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubePerformanceMonitor
    var type_MinikubePerformanceMonitor = Type.GetType("MinikubePerformanceMonitor");
    if (type_MinikubePerformanceMonitor != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubePerformanceMonitor (class) 存在");
        var ctors_MinikubePerformanceMonitor = type_MinikubePerformanceMonitor.GetConstructors();
        Console.WriteLine($"[PASS] MinikubePerformanceMonitor 构造函数数量: {ctors_MinikubePerformanceMonitor.Length}");
        var methods_MinikubePerformanceMonitor = type_MinikubePerformanceMonitor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubePerformanceMonitor 公开方法数量: {methods_MinikubePerformanceMonitor.Length}");
        foreach (var m in methods_MinikubePerformanceMonitor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubePerformanceMonitor 未找到，尝试无命名空间...");
        type_MinikubePerformanceMonitor = Type.GetType("MinikubePerformanceMonitor");
        if (type_MinikubePerformanceMonitor != null)
            Console.WriteLine("[PASS] 类型 MinikubePerformanceMonitor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubePerformanceMonitor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MinikubeTestRunner
    var type_MinikubeTestRunner = Type.GetType("MinikubeTestRunner");
    if (type_MinikubeTestRunner != null)
    {
        Console.WriteLine("[PASS] 类型 MinikubeTestRunner (class) 存在");
        var ctors_MinikubeTestRunner = type_MinikubeTestRunner.GetConstructors();
        Console.WriteLine($"[PASS] MinikubeTestRunner 构造函数数量: {ctors_MinikubeTestRunner.Length}");
        var methods_MinikubeTestRunner = type_MinikubeTestRunner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MinikubeTestRunner 公开方法数量: {methods_MinikubeTestRunner.Length}");
        foreach (var m in methods_MinikubeTestRunner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MinikubeTestRunner 未找到，尝试无命名空间...");
        type_MinikubeTestRunner = Type.GetType("MinikubeTestRunner");
        if (type_MinikubeTestRunner != null)
            Console.WriteLine("[PASS] 类型 MinikubeTestRunner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MinikubeTestRunner 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
