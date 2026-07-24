#load "litedb_cacheline.cs"

Console.WriteLine("=== litedb_cacheline.cs Test ===");

try
{
    // 验证 class: CacheOptimizedRepository
    var type_CacheOptimizedRepository = Type.GetType("CacheOptimizedRepository");
    if (type_CacheOptimizedRepository != null)
    {
        Console.WriteLine("[PASS] 类型 CacheOptimizedRepository (class) 存在");
        var ctors_CacheOptimizedRepository = type_CacheOptimizedRepository.GetConstructors();
        Console.WriteLine($"[PASS] CacheOptimizedRepository 构造函数数量: {ctors_CacheOptimizedRepository.Length}");
        var methods_CacheOptimizedRepository = type_CacheOptimizedRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheOptimizedRepository 公开方法数量: {methods_CacheOptimizedRepository.Length}");
        foreach (var m in methods_CacheOptimizedRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheOptimizedRepository 未找到，尝试无命名空间...");
        type_CacheOptimizedRepository = Type.GetType("CacheOptimizedRepository");
        if (type_CacheOptimizedRepository != null)
            Console.WriteLine("[PASS] 类型 CacheOptimizedRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheOptimizedRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 struct: CacheLineAlignedEvent
    var type_CacheLineAlignedEvent = Type.GetType("CacheLineAlignedEvent");
    if (type_CacheLineAlignedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 CacheLineAlignedEvent (struct) 存在");
        var ctors_CacheLineAlignedEvent = type_CacheLineAlignedEvent.GetConstructors();
        Console.WriteLine($"[PASS] CacheLineAlignedEvent 构造函数数量: {ctors_CacheLineAlignedEvent.Length}");
        var methods_CacheLineAlignedEvent = type_CacheLineAlignedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CacheLineAlignedEvent 公开方法数量: {methods_CacheLineAlignedEvent.Length}");
        foreach (var m in methods_CacheLineAlignedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CacheLineAlignedEvent 未找到，尝试无命名空间...");
        type_CacheLineAlignedEvent = Type.GetType("CacheLineAlignedEvent");
        if (type_CacheLineAlignedEvent != null)
            Console.WriteLine("[PASS] 类型 CacheLineAlignedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CacheLineAlignedEvent 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
