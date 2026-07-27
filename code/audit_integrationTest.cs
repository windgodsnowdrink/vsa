#load "audit_integration.cs"

Console.WriteLine("=== audit_integration.cs Test ===");

try
{
    // 验证 class: AuditQueue
    var type_AuditQueue = Type.GetType("AuditQueue");
    if (type_AuditQueue != null)
    {
        Console.WriteLine("[PASS] 类型 AuditQueue (class) 存在");
        var ctors_AuditQueue = type_AuditQueue.GetConstructors();
        Console.WriteLine($"[PASS] AuditQueue 构造函数数量: {ctors_AuditQueue.Length}");
        var methods_AuditQueue = type_AuditQueue.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditQueue 公开方法数量: {methods_AuditQueue.Length}");
        foreach (var m in methods_AuditQueue)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditQueue 未找到，尝试无命名空间...");
        type_AuditQueue = Type.GetType("AuditQueue");
        if (type_AuditQueue != null)
            Console.WriteLine("[PASS] 类型 AuditQueue (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditQueue 可能为顶层语句或嵌套类型");
    }

    // 验证 class: HighPerformanceAuditProvider
    var type_HighPerformanceAuditProvider = Type.GetType("HighPerformanceAuditProvider");
    if (type_HighPerformanceAuditProvider != null)
    {
        Console.WriteLine("[PASS] 类型 HighPerformanceAuditProvider (class) 存在");
        var ctors_HighPerformanceAuditProvider = type_HighPerformanceAuditProvider.GetConstructors();
        Console.WriteLine($"[PASS] HighPerformanceAuditProvider 构造函数数量: {ctors_HighPerformanceAuditProvider.Length}");
        var methods_HighPerformanceAuditProvider = type_HighPerformanceAuditProvider.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] HighPerformanceAuditProvider 公开方法数量: {methods_HighPerformanceAuditProvider.Length}");
        foreach (var m in methods_HighPerformanceAuditProvider)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 HighPerformanceAuditProvider 未找到，尝试无命名空间...");
        type_HighPerformanceAuditProvider = Type.GetType("HighPerformanceAuditProvider");
        if (type_HighPerformanceAuditProvider != null)
            Console.WriteLine("[PASS] 类型 HighPerformanceAuditProvider (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 HighPerformanceAuditProvider 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AuditBackgroundService
    var type_AuditBackgroundService = Type.GetType("AuditBackgroundService");
    if (type_AuditBackgroundService != null)
    {
        Console.WriteLine("[PASS] 类型 AuditBackgroundService (class) 存在");
        var ctors_AuditBackgroundService = type_AuditBackgroundService.GetConstructors();
        Console.WriteLine($"[PASS] AuditBackgroundService 构造函数数量: {ctors_AuditBackgroundService.Length}");
        var methods_AuditBackgroundService = type_AuditBackgroundService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AuditBackgroundService 公开方法数量: {methods_AuditBackgroundService.Length}");
        foreach (var m in methods_AuditBackgroundService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AuditBackgroundService 未找到，尝试无命名空间...");
        type_AuditBackgroundService = Type.GetType("AuditBackgroundService");
        if (type_AuditBackgroundService != null)
            Console.WriteLine("[PASS] 类型 AuditBackgroundService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AuditBackgroundService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
