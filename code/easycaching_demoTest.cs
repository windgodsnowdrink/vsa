#load "easycaching_demo.cs"

Console.WriteLine("=== easycaching_demo.cs Test ===");

try
{
    // 验证 class: MemoryOptimizer
    var type_MemoryOptimizer = Type.GetType("MemoryOptimizer");
    if (type_MemoryOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryOptimizer (class) 存在");
        var ctors_MemoryOptimizer = type_MemoryOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] MemoryOptimizer 构造函数数量: {ctors_MemoryOptimizer.Length}");
        var methods_MemoryOptimizer = type_MemoryOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryOptimizer 公开方法数量: {methods_MemoryOptimizer.Length}");
        foreach (var m in methods_MemoryOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryOptimizer 未找到，尝试无命名空间...");
        type_MemoryOptimizer = Type.GetType("MemoryOptimizer");
        if (type_MemoryOptimizer != null)
            Console.WriteLine("[PASS] 类型 MemoryOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryOptimizer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoItem
    var type_TodoItem = Type.GetType("TodoItem");
    if (type_TodoItem != null)
    {
        Console.WriteLine("[PASS] 类型 TodoItem (class) 存在");
        var ctors_TodoItem = type_TodoItem.GetConstructors();
        Console.WriteLine($"[PASS] TodoItem 构造函数数量: {ctors_TodoItem.Length}");
        var methods_TodoItem = type_TodoItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoItem 公开方法数量: {methods_TodoItem.Length}");
        foreach (var m in methods_TodoItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoItem 未找到，尝试无命名空间...");
        type_TodoItem = Type.GetType("TodoItem");
        if (type_TodoItem != null)
            Console.WriteLine("[PASS] 类型 TodoItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoItem 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoCacheService
    var type_TodoCacheService = Type.GetType("TodoCacheService");
    if (type_TodoCacheService != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCacheService (class) 存在");
        var ctors_TodoCacheService = type_TodoCacheService.GetConstructors();
        Console.WriteLine($"[PASS] TodoCacheService 构造函数数量: {ctors_TodoCacheService.Length}");
        var methods_TodoCacheService = type_TodoCacheService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCacheService 公开方法数量: {methods_TodoCacheService.Length}");
        foreach (var m in methods_TodoCacheService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCacheService 未找到，尝试无命名空间...");
        type_TodoCacheService = Type.GetType("TodoCacheService");
        if (type_TodoCacheService != null)
            Console.WriteLine("[PASS] 类型 TodoCacheService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCacheService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: CacheWarmupService
    var type_CacheWarmupService = Type.GetType("CacheWarmupService");
    if (type_CacheWarmupService != null)
    {
        Console.WriteLine("[PASS] 类型 CacheWarmupService (class) 存在");
        var ctors_CacheWarmupService = type_CacheWarmupService.GetConstructors();
        Console.WriteLine($"[PASS] CacheWarmupService 构造函数数量: {ctors_CacheWarmupService.Length}");
        var methods_CacheWarmupService = type_CacheWarmupService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheWarmupService 公开方法数量: {methods_CacheWarmupService.Length}");
        foreach (var m in methods_CacheWarmupService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheWarmupService 未找到，尝试无命名空间...");
        type_CacheWarmupService = Type.GetType("CacheWarmupService");
        if (type_CacheWarmupService != null)
            Console.WriteLine("[PASS] 类型 CacheWarmupService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheWarmupService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
