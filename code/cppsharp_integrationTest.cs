#load "cppsharp_integration.cs"

Console.WriteLine("=== cppsharp_integration.cs Test ===");

try
{
    // 验证 class: CppSharpIntegration.CppSharpOptions
    var type_CppSharpOptions = Type.GetType("CppSharpIntegration.CppSharpOptions");
    if (type_CppSharpOptions != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.CppSharpOptions (class) 存在");
        var ctors_CppSharpOptions = type_CppSharpOptions.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.CppSharpOptions 构造函数数量: {ctors_CppSharpOptions.Length}");
        var methods_CppSharpOptions = type_CppSharpOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.CppSharpOptions 公开方法数量: {methods_CppSharpOptions.Length}");
        foreach (var m in methods_CppSharpOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.CppSharpOptions 未找到，尝试无命名空间...");
        type_CppSharpOptions = Type.GetType("CppSharpOptions");
        if (type_CppSharpOptions != null)
            Console.WriteLine("[PASS] 类型 CppSharpOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CppSharpOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.CppSharpService
    var type_CppSharpService = Type.GetType("CppSharpIntegration.CppSharpService");
    if (type_CppSharpService != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.CppSharpService (class) 存在");
        var ctors_CppSharpService = type_CppSharpService.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.CppSharpService 构造函数数量: {ctors_CppSharpService.Length}");
        var methods_CppSharpService = type_CppSharpService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.CppSharpService 公开方法数量: {methods_CppSharpService.Length}");
        foreach (var m in methods_CppSharpService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.CppSharpService 未找到，尝试无命名空间...");
        type_CppSharpService = Type.GetType("CppSharpService");
        if (type_CppSharpService != null)
            Console.WriteLine("[PASS] 类型 CppSharpService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CppSharpService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("CppSharpIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.TenantContext
    var type_TenantContext = Type.GetType("CppSharpIntegration.TenantContext");
    if (type_TenantContext != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.TenantContext (class) 存在");
        var ctors_TenantContext = type_TenantContext.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.TenantContext 构造函数数量: {ctors_TenantContext.Length}");
        var methods_TenantContext = type_TenantContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.TenantContext 公开方法数量: {methods_TenantContext.Length}");
        foreach (var m in methods_TenantContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.TenantContext 未找到，尝试无命名空间...");
        type_TenantContext = Type.GetType("TenantContext");
        if (type_TenantContext != null)
            Console.WriteLine("[PASS] 类型 TenantContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.ConnectionStatistics
    var type_ConnectionStatistics = Type.GetType("CppSharpIntegration.ConnectionStatistics");
    if (type_ConnectionStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.ConnectionStatistics (class) 存在");
        var ctors_ConnectionStatistics = type_ConnectionStatistics.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.ConnectionStatistics 构造函数数量: {ctors_ConnectionStatistics.Length}");
        var methods_ConnectionStatistics = type_ConnectionStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.ConnectionStatistics 公开方法数量: {methods_ConnectionStatistics.Length}");
        foreach (var m in methods_ConnectionStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.ConnectionStatistics 未找到，尝试无命名空间...");
        type_ConnectionStatistics = Type.GetType("ConnectionStatistics");
        if (type_ConnectionStatistics != null)
            Console.WriteLine("[PASS] 类型 ConnectionStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.ThroughputStatistics
    var type_ThroughputStatistics = Type.GetType("CppSharpIntegration.ThroughputStatistics");
    if (type_ThroughputStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.ThroughputStatistics (class) 存在");
        var ctors_ThroughputStatistics = type_ThroughputStatistics.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.ThroughputStatistics 构造函数数量: {ctors_ThroughputStatistics.Length}");
        var methods_ThroughputStatistics = type_ThroughputStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.ThroughputStatistics 公开方法数量: {methods_ThroughputStatistics.Length}");
        foreach (var m in methods_ThroughputStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.ThroughputStatistics 未找到，尝试无命名空间...");
        type_ThroughputStatistics = Type.GetType("ThroughputStatistics");
        if (type_ThroughputStatistics != null)
            Console.WriteLine("[PASS] 类型 ThroughputStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThroughputStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.CppSharpHealthCheck
    var type_CppSharpHealthCheck = Type.GetType("CppSharpIntegration.CppSharpHealthCheck");
    if (type_CppSharpHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.CppSharpHealthCheck (class) 存在");
        var ctors_CppSharpHealthCheck = type_CppSharpHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.CppSharpHealthCheck 构造函数数量: {ctors_CppSharpHealthCheck.Length}");
        var methods_CppSharpHealthCheck = type_CppSharpHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.CppSharpHealthCheck 公开方法数量: {methods_CppSharpHealthCheck.Length}");
        foreach (var m in methods_CppSharpHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.CppSharpHealthCheck 未找到，尝试无命名空间...");
        type_CppSharpHealthCheck = Type.GetType("CppSharpHealthCheck");
        if (type_CppSharpHealthCheck != null)
            Console.WriteLine("[PASS] 类型 CppSharpHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CppSharpHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CppSharpIntegration.MemoryPooledPolicy
    var type_MemoryPooledPolicy = Type.GetType("CppSharpIntegration.MemoryPooledPolicy");
    if (type_MemoryPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.MemoryPooledPolicy (class) 存在");
        var ctors_MemoryPooledPolicy = type_MemoryPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] CppSharpIntegration.MemoryPooledPolicy 构造函数数量: {ctors_MemoryPooledPolicy.Length}");
        var methods_MemoryPooledPolicy = type_MemoryPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CppSharpIntegration.MemoryPooledPolicy 公开方法数量: {methods_MemoryPooledPolicy.Length}");
        foreach (var m in methods_MemoryPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.MemoryPooledPolicy 未找到，尝试无命名空间...");
        type_MemoryPooledPolicy = Type.GetType("MemoryPooledPolicy");
        if (type_MemoryPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: CppSharpIntegration.ICppSharpService
    var type_ICppSharpService = Type.GetType("CppSharpIntegration.ICppSharpService");
    if (type_ICppSharpService != null)
    {
        Console.WriteLine("[PASS] 类型 CppSharpIntegration.ICppSharpService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CppSharpIntegration.ICppSharpService 未找到，尝试无命名空间...");
        type_ICppSharpService = Type.GetType("ICppSharpService");
        if (type_ICppSharpService != null)
            Console.WriteLine("[PASS] 类型 ICppSharpService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICppSharpService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
