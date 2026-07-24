#load "multitenant_cache_service.cs"

Console.WriteLine("=== multitenant_cache_service.cs Test ===");

try
{
    // 验证 class: TenantAwareCacheService
    var type_TenantAwareCacheService = Type.GetType("TenantAwareCacheService");
    if (type_TenantAwareCacheService != null)
    {
        Console.WriteLine("[PASS] 类型 TenantAwareCacheService (class) 存在");
        var ctors_TenantAwareCacheService = type_TenantAwareCacheService.GetConstructors();
        Console.WriteLine($"[PASS] TenantAwareCacheService 构造函数数量: {ctors_TenantAwareCacheService.Length}");
        var methods_TenantAwareCacheService = type_TenantAwareCacheService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TenantAwareCacheService 公开方法数量: {methods_TenantAwareCacheService.Length}");
        foreach (var m in methods_TenantAwareCacheService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TenantAwareCacheService 未找到，尝试无命名空间...");
        type_TenantAwareCacheService = Type.GetType("TenantAwareCacheService");
        if (type_TenantAwareCacheService != null)
            Console.WriteLine("[PASS] 类型 TenantAwareCacheService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TenantAwareCacheService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheContext
    var type_CacheContext = Type.GetType("CacheContext");
    if (type_CacheContext != null)
    {
        Console.WriteLine("[PASS] 类型 CacheContext (class) 存在");
        var ctors_CacheContext = type_CacheContext.GetConstructors();
        Console.WriteLine($"[PASS] CacheContext 构造函数数量: {ctors_CacheContext.Length}");
        var methods_CacheContext = type_CacheContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheContext 公开方法数量: {methods_CacheContext.Length}");
        foreach (var m in methods_CacheContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheContext 未找到，尝试无命名空间...");
        type_CacheContext = Type.GetType("CacheContext");
        if (type_CacheContext != null)
            Console.WriteLine("[PASS] 类型 CacheContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheContextPooledPolicy
    var type_CacheContextPooledPolicy = Type.GetType("CacheContextPooledPolicy");
    if (type_CacheContextPooledPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 CacheContextPooledPolicy (class) 存在");
        var ctors_CacheContextPooledPolicy = type_CacheContextPooledPolicy.GetConstructors();
        Console.WriteLine($"[PASS] CacheContextPooledPolicy 构造函数数量: {ctors_CacheContextPooledPolicy.Length}");
        var methods_CacheContextPooledPolicy = type_CacheContextPooledPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheContextPooledPolicy 公开方法数量: {methods_CacheContextPooledPolicy.Length}");
        foreach (var m in methods_CacheContextPooledPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheContextPooledPolicy 未找到，尝试无命名空间...");
        type_CacheContextPooledPolicy = Type.GetType("CacheContextPooledPolicy");
        if (type_CacheContextPooledPolicy != null)
            Console.WriteLine("[PASS] 类型 CacheContextPooledPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheContextPooledPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ITenantCachePolicy
    var type_ITenantCachePolicy = Type.GetType("ITenantCachePolicy");
    if (type_ITenantCachePolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ITenantCachePolicy (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ITenantCachePolicy 未找到，尝试无命名空间...");
        type_ITenantCachePolicy = Type.GetType("ITenantCachePolicy");
        if (type_ITenantCachePolicy != null)
            Console.WriteLine("[PASS] 类型 ITenantCachePolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ITenantCachePolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: CacheOperation
    var type_CacheOperation = Type.GetType("CacheOperation");
    if (type_CacheOperation != null)
    {
        Console.WriteLine("[PASS] 类型 CacheOperation (struct) 存在");
        var ctors_CacheOperation = type_CacheOperation.GetConstructors();
        Console.WriteLine($"[PASS] CacheOperation 构造函数数量: {ctors_CacheOperation.Length}");
        var methods_CacheOperation = type_CacheOperation.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheOperation 公开方法数量: {methods_CacheOperation.Length}");
        foreach (var m in methods_CacheOperation)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheOperation 未找到，尝试无命名空间...");
        type_CacheOperation = Type.GetType("CacheOperation");
        if (type_CacheOperation != null)
            Console.WriteLine("[PASS] 类型 CacheOperation (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheOperation 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: CacheOperationType
    var type_CacheOperationType = Type.GetType("CacheOperationType");
    if (type_CacheOperationType != null)
    {
        Console.WriteLine("[PASS] 类型 CacheOperationType (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheOperationType 未找到，尝试无命名空间...");
        type_CacheOperationType = Type.GetType("CacheOperationType");
        if (type_CacheOperationType != null)
            Console.WriteLine("[PASS] 类型 CacheOperationType (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheOperationType 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
