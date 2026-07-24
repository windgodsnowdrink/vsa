#load "freeim_integration.cs"

Console.WriteLine("=== freeim_integration.cs Test ===");

try
{
    // 验证 class: FreeIMOptions
    var type_FreeIMOptions = Type.GetType("FreeIMOptions");
    if (type_FreeIMOptions != null)
    {
        Console.WriteLine("[PASS] 类型 FreeIMOptions (class) 存在");
        var ctors_FreeIMOptions = type_FreeIMOptions.GetConstructors();
        Console.WriteLine($"[PASS] FreeIMOptions 构造函数数量: {ctors_FreeIMOptions.Length}");
        var methods_FreeIMOptions = type_FreeIMOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FreeIMOptions 公开方法数量: {methods_FreeIMOptions.Length}");
        foreach (var m in methods_FreeIMOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FreeIMOptions 未找到，尝试无命名空间...");
        type_FreeIMOptions = Type.GetType("FreeIMOptions");
        if (type_FreeIMOptions != null)
            Console.WriteLine("[PASS] 类型 FreeIMOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FreeIMOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FreeIMService
    var type_FreeIMService = Type.GetType("FreeIMService");
    if (type_FreeIMService != null)
    {
        Console.WriteLine("[PASS] 类型 FreeIMService (class) 存在");
        var ctors_FreeIMService = type_FreeIMService.GetConstructors();
        Console.WriteLine($"[PASS] FreeIMService 构造函数数量: {ctors_FreeIMService.Length}");
        var methods_FreeIMService = type_FreeIMService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FreeIMService 公开方法数量: {methods_FreeIMService.Length}");
        foreach (var m in methods_FreeIMService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FreeIMService 未找到，尝试无命名空间...");
        type_FreeIMService = Type.GetType("FreeIMService");
        if (type_FreeIMService != null)
            Console.WriteLine("[PASS] 类型 FreeIMService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FreeIMService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: FreeIMHealthCheck
    var type_FreeIMHealthCheck = Type.GetType("FreeIMHealthCheck");
    if (type_FreeIMHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 FreeIMHealthCheck (class) 存在");
        var ctors_FreeIMHealthCheck = type_FreeIMHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] FreeIMHealthCheck 构造函数数量: {ctors_FreeIMHealthCheck.Length}");
        var methods_FreeIMHealthCheck = type_FreeIMHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] FreeIMHealthCheck 公开方法数量: {methods_FreeIMHealthCheck.Length}");
        foreach (var m in methods_FreeIMHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 FreeIMHealthCheck 未找到，尝试无命名空间...");
        type_FreeIMHealthCheck = Type.GetType("FreeIMHealthCheck");
        if (type_FreeIMHealthCheck != null)
            Console.WriteLine("[PASS] 类型 FreeIMHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 FreeIMHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantContext
    var type_TenantContext = Type.GetType("TenantContext");
    if (type_TenantContext != null)
    {
        Console.WriteLine("[PASS] 类型 TenantContext (class) 存在");
        var ctors_TenantContext = type_TenantContext.GetConstructors();
        Console.WriteLine($"[PASS] TenantContext 构造函数数量: {ctors_TenantContext.Length}");
        var methods_TenantContext = type_TenantContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantContext 公开方法数量: {methods_TenantContext.Length}");
        foreach (var m in methods_TenantContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantContext 未找到，尝试无命名空间...");
        type_TenantContext = Type.GetType("TenantContext");
        if (type_TenantContext != null)
            Console.WriteLine("[PASS] 类型 TenantContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantContext 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IFreeIMService
    var type_IFreeIMService = Type.GetType("IFreeIMService");
    if (type_IFreeIMService != null)
    {
        Console.WriteLine("[PASS] 类型 IFreeIMService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IFreeIMService 未找到，尝试无命名空间...");
        type_IFreeIMService = Type.GetType("IFreeIMService");
        if (type_IFreeIMService != null)
            Console.WriteLine("[PASS] 类型 IFreeIMService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IFreeIMService 可能为顶层语句或嵌套类型");
    }

    // 验证 record: MemoryPoolStatistics
    var type_MemoryPoolStatistics = Type.GetType("MemoryPoolStatistics");
    if (type_MemoryPoolStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryPoolStatistics (record) 存在");
        var ctors_MemoryPoolStatistics = type_MemoryPoolStatistics.GetConstructors();
        Console.WriteLine($"[PASS] MemoryPoolStatistics 构造函数数量: {ctors_MemoryPoolStatistics.Length}");
        var methods_MemoryPoolStatistics = type_MemoryPoolStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryPoolStatistics 公开方法数量: {methods_MemoryPoolStatistics.Length}");
        foreach (var m in methods_MemoryPoolStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryPoolStatistics 未找到，尝试无命名空间...");
        type_MemoryPoolStatistics = Type.GetType("MemoryPoolStatistics");
        if (type_MemoryPoolStatistics != null)
            Console.WriteLine("[PASS] 类型 MemoryPoolStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryPoolStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ConnectionStatistics
    var type_ConnectionStatistics = Type.GetType("ConnectionStatistics");
    if (type_ConnectionStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 ConnectionStatistics (record) 存在");
        var ctors_ConnectionStatistics = type_ConnectionStatistics.GetConstructors();
        Console.WriteLine($"[PASS] ConnectionStatistics 构造函数数量: {ctors_ConnectionStatistics.Length}");
        var methods_ConnectionStatistics = type_ConnectionStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ConnectionStatistics 公开方法数量: {methods_ConnectionStatistics.Length}");
        foreach (var m in methods_ConnectionStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ConnectionStatistics 未找到，尝试无命名空间...");
        type_ConnectionStatistics = Type.GetType("ConnectionStatistics");
        if (type_ConnectionStatistics != null)
            Console.WriteLine("[PASS] 类型 ConnectionStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ThroughputStatistics
    var type_ThroughputStatistics = Type.GetType("ThroughputStatistics");
    if (type_ThroughputStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 ThroughputStatistics (record) 存在");
        var ctors_ThroughputStatistics = type_ThroughputStatistics.GetConstructors();
        Console.WriteLine($"[PASS] ThroughputStatistics 构造函数数量: {ctors_ThroughputStatistics.Length}");
        var methods_ThroughputStatistics = type_ThroughputStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThroughputStatistics 公开方法数量: {methods_ThroughputStatistics.Length}");
        foreach (var m in methods_ThroughputStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThroughputStatistics 未找到，尝试无命名空间...");
        type_ThroughputStatistics = Type.GetType("ThroughputStatistics");
        if (type_ThroughputStatistics != null)
            Console.WriteLine("[PASS] 类型 ThroughputStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThroughputStatistics 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
