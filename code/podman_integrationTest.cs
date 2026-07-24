#load "podman_integration.cs"

Console.WriteLine("=== podman_integration.cs Test ===");

try
{
    // 验证 class: PodmanIntegration.PodmanOptions
    var type_PodmanOptions = Type.GetType("PodmanIntegration.PodmanOptions");
    if (type_PodmanOptions != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.PodmanOptions (class) 存在");
        var ctors_PodmanOptions = type_PodmanOptions.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanOptions 构造函数数量: {ctors_PodmanOptions.Length}");
        var methods_PodmanOptions = type_PodmanOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanOptions 公开方法数量: {methods_PodmanOptions.Length}");
        foreach (var m in methods_PodmanOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.PodmanOptions 未找到，尝试无命名空间...");
        type_PodmanOptions = Type.GetType("PodmanOptions");
        if (type_PodmanOptions != null)
            Console.WriteLine("[PASS] 类型 PodmanOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PodmanOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PodmanIntegration.PodmanContainerManager
    var type_PodmanContainerManager = Type.GetType("PodmanIntegration.PodmanContainerManager");
    if (type_PodmanContainerManager != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.PodmanContainerManager (class) 存在");
        var ctors_PodmanContainerManager = type_PodmanContainerManager.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanContainerManager 构造函数数量: {ctors_PodmanContainerManager.Length}");
        var methods_PodmanContainerManager = type_PodmanContainerManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanContainerManager 公开方法数量: {methods_PodmanContainerManager.Length}");
        foreach (var m in methods_PodmanContainerManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.PodmanContainerManager 未找到，尝试无命名空间...");
        type_PodmanContainerManager = Type.GetType("PodmanContainerManager");
        if (type_PodmanContainerManager != null)
            Console.WriteLine("[PASS] 类型 PodmanContainerManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PodmanContainerManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PodmanIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("PodmanIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PodmanIntegration.PodmanImageBuilder
    var type_PodmanImageBuilder = Type.GetType("PodmanIntegration.PodmanImageBuilder");
    if (type_PodmanImageBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.PodmanImageBuilder (class) 存在");
        var ctors_PodmanImageBuilder = type_PodmanImageBuilder.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanImageBuilder 构造函数数量: {ctors_PodmanImageBuilder.Length}");
        var methods_PodmanImageBuilder = type_PodmanImageBuilder.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanImageBuilder 公开方法数量: {methods_PodmanImageBuilder.Length}");
        foreach (var m in methods_PodmanImageBuilder)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.PodmanImageBuilder 未找到，尝试无命名空间...");
        type_PodmanImageBuilder = Type.GetType("PodmanImageBuilder");
        if (type_PodmanImageBuilder != null)
            Console.WriteLine("[PASS] 类型 PodmanImageBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PodmanImageBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PodmanIntegration.PodmanNetworkManager
    var type_PodmanNetworkManager = Type.GetType("PodmanIntegration.PodmanNetworkManager");
    if (type_PodmanNetworkManager != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.PodmanNetworkManager (class) 存在");
        var ctors_PodmanNetworkManager = type_PodmanNetworkManager.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanNetworkManager 构造函数数量: {ctors_PodmanNetworkManager.Length}");
        var methods_PodmanNetworkManager = type_PodmanNetworkManager.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanNetworkManager 公开方法数量: {methods_PodmanNetworkManager.Length}");
        foreach (var m in methods_PodmanNetworkManager)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.PodmanNetworkManager 未找到，尝试无命名空间...");
        type_PodmanNetworkManager = Type.GetType("PodmanNetworkManager");
        if (type_PodmanNetworkManager != null)
            Console.WriteLine("[PASS] 类型 PodmanNetworkManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PodmanNetworkManager 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PodmanIntegration.PodmanMonitorService
    var type_PodmanMonitorService = Type.GetType("PodmanIntegration.PodmanMonitorService");
    if (type_PodmanMonitorService != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.PodmanMonitorService (class) 存在");
        var ctors_PodmanMonitorService = type_PodmanMonitorService.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanMonitorService 构造函数数量: {ctors_PodmanMonitorService.Length}");
        var methods_PodmanMonitorService = type_PodmanMonitorService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanMonitorService 公开方法数量: {methods_PodmanMonitorService.Length}");
        foreach (var m in methods_PodmanMonitorService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.PodmanMonitorService 未找到，尝试无命名空间...");
        type_PodmanMonitorService = Type.GetType("PodmanMonitorService");
        if (type_PodmanMonitorService != null)
            Console.WriteLine("[PASS] 类型 PodmanMonitorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PodmanMonitorService 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: PodmanIntegration.IPodmanContainerManager
    var type_IPodmanContainerManager = Type.GetType("PodmanIntegration.IPodmanContainerManager");
    if (type_IPodmanContainerManager != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.IPodmanContainerManager (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.IPodmanContainerManager 未找到，尝试无命名空间...");
        type_IPodmanContainerManager = Type.GetType("IPodmanContainerManager");
        if (type_IPodmanContainerManager != null)
            Console.WriteLine("[PASS] 类型 IPodmanContainerManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPodmanContainerManager 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: PodmanIntegration.IPodmanImageBuilder
    var type_IPodmanImageBuilder = Type.GetType("PodmanIntegration.IPodmanImageBuilder");
    if (type_IPodmanImageBuilder != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.IPodmanImageBuilder (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.IPodmanImageBuilder 未找到，尝试无命名空间...");
        type_IPodmanImageBuilder = Type.GetType("IPodmanImageBuilder");
        if (type_IPodmanImageBuilder != null)
            Console.WriteLine("[PASS] 类型 IPodmanImageBuilder (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPodmanImageBuilder 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: PodmanIntegration.IPodmanNetworkManager
    var type_IPodmanNetworkManager = Type.GetType("PodmanIntegration.IPodmanNetworkManager");
    if (type_IPodmanNetworkManager != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.IPodmanNetworkManager (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.IPodmanNetworkManager 未找到，尝试无命名空间...");
        type_IPodmanNetworkManager = Type.GetType("IPodmanNetworkManager");
        if (type_IPodmanNetworkManager != null)
            Console.WriteLine("[PASS] 类型 IPodmanNetworkManager (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPodmanNetworkManager 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: PodmanIntegration.IPodmanMonitorService
    var type_IPodmanMonitorService = Type.GetType("PodmanIntegration.IPodmanMonitorService");
    if (type_IPodmanMonitorService != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.IPodmanMonitorService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.IPodmanMonitorService 未找到，尝试无命名空间...");
        type_IPodmanMonitorService = Type.GetType("IPodmanMonitorService");
        if (type_IPodmanMonitorService != null)
            Console.WriteLine("[PASS] 类型 IPodmanMonitorService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IPodmanMonitorService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: PodmanIntegration.PodmanMetrics
    var type_PodmanMetrics = Type.GetType("PodmanIntegration.PodmanMetrics");
    if (type_PodmanMetrics != null)
    {
        Console.WriteLine("[PASS] 类型 PodmanIntegration.PodmanMetrics (record) 存在");
        var ctors_PodmanMetrics = type_PodmanMetrics.GetConstructors();
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanMetrics 构造函数数量: {ctors_PodmanMetrics.Length}");
        var methods_PodmanMetrics = type_PodmanMetrics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PodmanIntegration.PodmanMetrics 公开方法数量: {methods_PodmanMetrics.Length}");
        foreach (var m in methods_PodmanMetrics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PodmanIntegration.PodmanMetrics 未找到，尝试无命名空间...");
        type_PodmanMetrics = Type.GetType("PodmanMetrics");
        if (type_PodmanMetrics != null)
            Console.WriteLine("[PASS] 类型 PodmanMetrics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PodmanMetrics 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
