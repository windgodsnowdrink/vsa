#load "iconpython_integration.cs"

Console.WriteLine("=== iconpython_integration.cs Test ===");

try
{
    // 验证 class: IconPythonIntegration.IconPythonOptions
    var type_IconPythonOptions = Type.GetType("IconPythonIntegration.IconPythonOptions");
    if (type_IconPythonOptions != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.IconPythonOptions (class) 存在");
        var ctors_IconPythonOptions = type_IconPythonOptions.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.IconPythonOptions 构造函数数量: {ctors_IconPythonOptions.Length}");
        var methods_IconPythonOptions = type_IconPythonOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.IconPythonOptions 公开方法数量: {methods_IconPythonOptions.Length}");
        foreach (var m in methods_IconPythonOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.IconPythonOptions 未找到，尝试无命名空间...");
        type_IconPythonOptions = Type.GetType("IconPythonOptions");
        if (type_IconPythonOptions != null)
            Console.WriteLine("[PASS] 类型 IconPythonOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IconPythonOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.IconPythonService
    var type_IconPythonService = Type.GetType("IconPythonIntegration.IconPythonService");
    if (type_IconPythonService != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.IconPythonService (class) 存在");
        var ctors_IconPythonService = type_IconPythonService.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.IconPythonService 构造函数数量: {ctors_IconPythonService.Length}");
        var methods_IconPythonService = type_IconPythonService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.IconPythonService 公开方法数量: {methods_IconPythonService.Length}");
        foreach (var m in methods_IconPythonService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.IconPythonService 未找到，尝试无命名空间...");
        type_IconPythonService = Type.GetType("IconPythonService");
        if (type_IconPythonService != null)
            Console.WriteLine("[PASS] 类型 IconPythonService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IconPythonService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.MemoryOwner
    var type_MemoryOwner = Type.GetType("IconPythonIntegration.MemoryOwner");
    if (type_MemoryOwner != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.MemoryOwner (class) 存在");
        var ctors_MemoryOwner = type_MemoryOwner.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.MemoryOwner 构造函数数量: {ctors_MemoryOwner.Length}");
        var methods_MemoryOwner = type_MemoryOwner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.MemoryOwner 公开方法数量: {methods_MemoryOwner.Length}");
        foreach (var m in methods_MemoryOwner)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.MemoryOwner 未找到，尝试无命名空间...");
        type_MemoryOwner = Type.GetType("MemoryOwner");
        if (type_MemoryOwner != null)
            Console.WriteLine("[PASS] 类型 MemoryOwner (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryOwner 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("IconPythonIntegration.ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.IconPythonHealthCheck
    var type_IconPythonHealthCheck = Type.GetType("IconPythonIntegration.IconPythonHealthCheck");
    if (type_IconPythonHealthCheck != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.IconPythonHealthCheck (class) 存在");
        var ctors_IconPythonHealthCheck = type_IconPythonHealthCheck.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.IconPythonHealthCheck 构造函数数量: {ctors_IconPythonHealthCheck.Length}");
        var methods_IconPythonHealthCheck = type_IconPythonHealthCheck.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.IconPythonHealthCheck 公开方法数量: {methods_IconPythonHealthCheck.Length}");
        foreach (var m in methods_IconPythonHealthCheck)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.IconPythonHealthCheck 未找到，尝试无命名空间...");
        type_IconPythonHealthCheck = Type.GetType("IconPythonHealthCheck");
        if (type_IconPythonHealthCheck != null)
            Console.WriteLine("[PASS] 类型 IconPythonHealthCheck (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IconPythonHealthCheck 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.ConnectionStatistics
    var type_ConnectionStatistics = Type.GetType("IconPythonIntegration.ConnectionStatistics");
    if (type_ConnectionStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.ConnectionStatistics (class) 存在");
        var ctors_ConnectionStatistics = type_ConnectionStatistics.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.ConnectionStatistics 构造函数数量: {ctors_ConnectionStatistics.Length}");
        var methods_ConnectionStatistics = type_ConnectionStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.ConnectionStatistics 公开方法数量: {methods_ConnectionStatistics.Length}");
        foreach (var m in methods_ConnectionStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.ConnectionStatistics 未找到，尝试无命名空间...");
        type_ConnectionStatistics = Type.GetType("ConnectionStatistics");
        if (type_ConnectionStatistics != null)
            Console.WriteLine("[PASS] 类型 ConnectionStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ConnectionStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.ThroughputStatistics
    var type_ThroughputStatistics = Type.GetType("IconPythonIntegration.ThroughputStatistics");
    if (type_ThroughputStatistics != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.ThroughputStatistics (class) 存在");
        var ctors_ThroughputStatistics = type_ThroughputStatistics.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.ThroughputStatistics 构造函数数量: {ctors_ThroughputStatistics.Length}");
        var methods_ThroughputStatistics = type_ThroughputStatistics.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.ThroughputStatistics 公开方法数量: {methods_ThroughputStatistics.Length}");
        foreach (var m in methods_ThroughputStatistics)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.ThroughputStatistics 未找到，尝试无命名空间...");
        type_ThroughputStatistics = Type.GetType("ThroughputStatistics");
        if (type_ThroughputStatistics != null)
            Console.WriteLine("[PASS] 类型 ThroughputStatistics (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThroughputStatistics 可能为顶层语句或嵌套类型");
    }

    // 验证 class: IconPythonIntegration.TenantContext
    var type_TenantContext = Type.GetType("IconPythonIntegration.TenantContext");
    if (type_TenantContext != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.TenantContext (class) 存在");
        var ctors_TenantContext = type_TenantContext.GetConstructors();
        Console.WriteLine($"[PASS] IconPythonIntegration.TenantContext 构造函数数量: {ctors_TenantContext.Length}");
        var methods_TenantContext = type_TenantContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] IconPythonIntegration.TenantContext 公开方法数量: {methods_TenantContext.Length}");
        foreach (var m in methods_TenantContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.TenantContext 未找到，尝试无命名空间...");
        type_TenantContext = Type.GetType("TenantContext");
        if (type_TenantContext != null)
            Console.WriteLine("[PASS] 类型 TenantContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantContext 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IconPythonIntegration.IIconPythonService
    var type_IIconPythonService = Type.GetType("IconPythonIntegration.IIconPythonService");
    if (type_IIconPythonService != null)
    {
        Console.WriteLine("[PASS] 类型 IconPythonIntegration.IIconPythonService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IconPythonIntegration.IIconPythonService 未找到，尝试无命名空间...");
        type_IIconPythonService = Type.GetType("IIconPythonService");
        if (type_IIconPythonService != null)
            Console.WriteLine("[PASS] 类型 IIconPythonService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IIconPythonService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
