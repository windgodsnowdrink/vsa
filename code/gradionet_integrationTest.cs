#load "gradionet_integration.cs"

Console.WriteLine("=== gradionet_integration.cs Test ===");

try
{
    // 验证 class: GradioNetIntegration.GradioNetOptions
    var type_GradioNetOptions = Type.GetType("GradioNetIntegration.GradioNetOptions");
    if (type_GradioNetOptions != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.GradioNetOptions (class) 存在");
        var ctors_GradioNetOptions = type_GradioNetOptions.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.GradioNetOptions 构造函数数量: {ctors_GradioNetOptions.Length}");
        var methods_GradioNetOptions = type_GradioNetOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.GradioNetOptions 公开方法数量: {methods_GradioNetOptions.Length}");
        foreach (var m in methods_GradioNetOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.GradioNetOptions 未找到，尝试无命名空间...");
        type_GradioNetOptions = Type.GetType("GradioNetOptions");
        if (type_GradioNetOptions != null)
            Console.WriteLine("[PASS] 类型 GradioNetOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GradioNetOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.GradioNetService
    var type_GradioNetService = Type.GetType("GradioNetIntegration.GradioNetService");
    if (type_GradioNetService != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.GradioNetService (class) 存在");
        var ctors_GradioNetService = type_GradioNetService.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.GradioNetService 构造函数数量: {ctors_GradioNetService.Length}");
        var methods_GradioNetService = type_GradioNetService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.GradioNetService 公开方法数量: {methods_GradioNetService.Length}");
        foreach (var m in methods_GradioNetService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.GradioNetService 未找到，尝试无命名空间...");
        type_GradioNetService = Type.GetType("GradioNetService");
        if (type_GradioNetService != null)
            Console.WriteLine("[PASS] 类型 GradioNetService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GradioNetService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("GradioNetIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.TenantContext
    var type_TenantContext = Type.GetType("GradioNetIntegration.TenantContext");
    if (type_TenantContext != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.TenantContext (class) 存在");
        var ctors_TenantContext = type_TenantContext.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.TenantContext 构造函数数量: {ctors_TenantContext.Length}");
        var methods_TenantContext = type_TenantContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.TenantContext 公开方法数量: {methods_TenantContext.Length}");
        foreach (var m in methods_TenantContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.TenantContext 未找到，尝试无命名空间...");
        type_TenantContext = Type.GetType("TenantContext");
        if (type_TenantContext != null)
            Console.WriteLine("[PASS] 类型 TenantContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.ConnectionStatistics
    var type_ConnectionStatistics = Type.GetType("GradioNetIntegration.ConnectionStatistics");
    if (type_ConnectionStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.ConnectionStatistics (class) 存在");
        var ctors_ConnectionStatistics = type_ConnectionStatistics.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.ConnectionStatistics 构造函数数量: {ctors_ConnectionStatistics.Length}");
        var methods_ConnectionStatistics = type_ConnectionStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.ConnectionStatistics 公开方法数量: {methods_ConnectionStatistics.Length}");
        foreach (var m in methods_ConnectionStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.ConnectionStatistics 未找到，尝试无命名空间...");
        type_ConnectionStatistics = Type.GetType("ConnectionStatistics");
        if (type_ConnectionStatistics != null)
            Console.WriteLine("[PASS] 类型 ConnectionStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.ThroughputStatistics
    var type_ThroughputStatistics = Type.GetType("GradioNetIntegration.ThroughputStatistics");
    if (type_ThroughputStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.ThroughputStatistics (class) 存在");
        var ctors_ThroughputStatistics = type_ThroughputStatistics.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.ThroughputStatistics 构造函数数量: {ctors_ThroughputStatistics.Length}");
        var methods_ThroughputStatistics = type_ThroughputStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.ThroughputStatistics 公开方法数量: {methods_ThroughputStatistics.Length}");
        foreach (var m in methods_ThroughputStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.ThroughputStatistics 未找到，尝试无命名空间...");
        type_ThroughputStatistics = Type.GetType("ThroughputStatistics");
        if (type_ThroughputStatistics != null)
            Console.WriteLine("[PASS] 类型 ThroughputStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThroughputStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.GradioNetHealthCheck
    var type_GradioNetHealthCheck = Type.GetType("GradioNetIntegration.GradioNetHealthCheck");
    if (type_GradioNetHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.GradioNetHealthCheck (class) 存在");
        var ctors_GradioNetHealthCheck = type_GradioNetHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.GradioNetHealthCheck 构造函数数量: {ctors_GradioNetHealthCheck.Length}");
        var methods_GradioNetHealthCheck = type_GradioNetHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.GradioNetHealthCheck 公开方法数量: {methods_GradioNetHealthCheck.Length}");
        foreach (var m in methods_GradioNetHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.GradioNetHealthCheck 未找到，尝试无命名空间...");
        type_GradioNetHealthCheck = Type.GetType("GradioNetHealthCheck");
        if (type_GradioNetHealthCheck != null)
            Console.WriteLine("[PASS] 类型 GradioNetHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 GradioNetHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: GradioNetIntegration.DiagnosticsConfig
    var type_DiagnosticsConfig = Type.GetType("GradioNetIntegration.DiagnosticsConfig");
    if (type_DiagnosticsConfig != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.DiagnosticsConfig (class) 存在");
        var ctors_DiagnosticsConfig = type_DiagnosticsConfig.GetConstructors();
        Console.WriteLine($"[PASS] GradioNetIntegration.DiagnosticsConfig 构造函数数量: {ctors_DiagnosticsConfig.Length}");
        var methods_DiagnosticsConfig = type_DiagnosticsConfig.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] GradioNetIntegration.DiagnosticsConfig 公开方法数量: {methods_DiagnosticsConfig.Length}");
        foreach (var m in methods_DiagnosticsConfig)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.DiagnosticsConfig 未找到，尝试无命名空间...");
        type_DiagnosticsConfig = Type.GetType("DiagnosticsConfig");
        if (type_DiagnosticsConfig != null)
            Console.WriteLine("[PASS] 类型 DiagnosticsConfig (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 DiagnosticsConfig 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: GradioNetIntegration.IGradioNetService
    var type_IGradioNetService = Type.GetType("GradioNetIntegration.IGradioNetService");
    if (type_IGradioNetService != null)
    {
        Console.WriteLine("[PASS] 类型 GradioNetIntegration.IGradioNetService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 GradioNetIntegration.IGradioNetService 未找到，尝试无命名空间...");
        type_IGradioNetService = Type.GetType("IGradioNetService");
        if (type_IGradioNetService != null)
            Console.WriteLine("[PASS] 类型 IGradioNetService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IGradioNetService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
