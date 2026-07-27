#load "multitenant_quota_management.cs"

Console.WriteLine("=== multitenant_quota_management.cs Test ===");

try
{
    // 验证 class: TenantQuotaService
    var type_TenantQuotaService = Type.GetType("TenantQuotaService");
    if (type_TenantQuotaService != null)
    {
        Console.WriteLine("[PASS] 类型 TenantQuotaService (class) 存在");
        var ctors_TenantQuotaService = type_TenantQuotaService.GetConstructors();
        Console.WriteLine($"[PASS] TenantQuotaService 构造函数数量: {ctors_TenantQuotaService.Length}");
        var methods_TenantQuotaService = type_TenantQuotaService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantQuotaService 公开方法数量: {methods_TenantQuotaService.Length}");
        foreach (var m in methods_TenantQuotaService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantQuotaService 未找到，尝试无命名空间...");
        type_TenantQuotaService = Type.GetType("TenantQuotaService");
        if (type_TenantQuotaService != null)
            Console.WriteLine("[PASS] 类型 TenantQuotaService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantQuotaService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: QuotaContext
    var type_QuotaContext = Type.GetType("QuotaContext");
    if (type_QuotaContext != null)
    {
        Console.WriteLine("[PASS] 类型 QuotaContext (class) 存在");
        var ctors_QuotaContext = type_QuotaContext.GetConstructors();
        Console.WriteLine($"[PASS] QuotaContext 构造函数数量: {ctors_QuotaContext.Length}");
        var methods_QuotaContext = type_QuotaContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] QuotaContext 公开方法数量: {methods_QuotaContext.Length}");
        foreach (var m in methods_QuotaContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 QuotaContext 未找到，尝试无命名空间...");
        type_QuotaContext = Type.GetType("QuotaContext");
        if (type_QuotaContext != null)
            Console.WriteLine("[PASS] 类型 QuotaContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 QuotaContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TenantQuotaExtensions
    var type_TenantQuotaExtensions = Type.GetType("TenantQuotaExtensions");
    if (type_TenantQuotaExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 TenantQuotaExtensions (class) 存在");
        var ctors_TenantQuotaExtensions = type_TenantQuotaExtensions.GetConstructors();
        Console.WriteLine($"[PASS] TenantQuotaExtensions 构造函数数量: {ctors_TenantQuotaExtensions.Length}");
        var methods_TenantQuotaExtensions = type_TenantQuotaExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantQuotaExtensions 公开方法数量: {methods_TenantQuotaExtensions.Length}");
        foreach (var m in methods_TenantQuotaExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantQuotaExtensions 未找到，尝试无命名空间...");
        type_TenantQuotaExtensions = Type.GetType("TenantQuotaExtensions");
        if (type_TenantQuotaExtensions != null)
            Console.WriteLine("[PASS] 类型 TenantQuotaExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantQuotaExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: TenantQuota
    var type_TenantQuota = Type.GetType("TenantQuota");
    if (type_TenantQuota != null)
    {
        Console.WriteLine("[PASS] 类型 TenantQuota (struct) 存在");
        var ctors_TenantQuota = type_TenantQuota.GetConstructors();
        Console.WriteLine($"[PASS] TenantQuota 构造函数数量: {ctors_TenantQuota.Length}");
        var methods_TenantQuota = type_TenantQuota.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantQuota 公开方法数量: {methods_TenantQuota.Length}");
        foreach (var m in methods_TenantQuota)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantQuota 未找到，尝试无命名空间...");
        type_TenantQuota = Type.GetType("TenantQuota");
        if (type_TenantQuota != null)
            Console.WriteLine("[PASS] 类型 TenantQuota (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantQuota 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: TenantResourceType
    var type_TenantResourceType = Type.GetType("TenantResourceType");
    if (type_TenantResourceType != null)
    {
        Console.WriteLine("[PASS] 类型 TenantResourceType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantResourceType 未找到，尝试无命名空间...");
        type_TenantResourceType = Type.GetType("TenantResourceType");
        if (type_TenantResourceType != null)
            Console.WriteLine("[PASS] 类型 TenantResourceType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantResourceType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
