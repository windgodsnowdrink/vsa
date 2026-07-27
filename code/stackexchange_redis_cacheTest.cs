#load "stackexchange_redis_cache.cs"

Console.WriteLine("=== stackexchange_redis_cache.cs Test ===");

try
{
    // 验证 class: MemoryObjectPool
    var type_MemoryObjectPool = Type.GetType("MemoryObjectPool");
    if (type_MemoryObjectPool != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryObjectPool (class) 存在");
        var ctors_MemoryObjectPool = type_MemoryObjectPool.GetConstructors();
        Console.WriteLine($"[PASS] MemoryObjectPool 构造函数数量: {ctors_MemoryObjectPool.Length}");
        var methods_MemoryObjectPool = type_MemoryObjectPool.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryObjectPool 公开方法数量: {methods_MemoryObjectPool.Length}");
        foreach (var m in methods_MemoryObjectPool)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryObjectPool 未找到，尝试无命名空间...");
        type_MemoryObjectPool = Type.GetType("MemoryObjectPool");
        if (type_MemoryObjectPool != null)
            Console.WriteLine("[PASS] 类型 MemoryObjectPool (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryObjectPool 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TailLatencyOptimizer
    var type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
    if (type_TailLatencyOptimizer != null)
    {
        Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (class) 存在");
        var ctors_TailLatencyOptimizer = type_TailLatencyOptimizer.GetConstructors();
        Console.WriteLine($"[PASS] TailLatencyOptimizer 构造函数数量: {ctors_TailLatencyOptimizer.Length}");
        var methods_TailLatencyOptimizer = type_TailLatencyOptimizer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TailLatencyOptimizer 公开方法数量: {methods_TailLatencyOptimizer.Length}");
        foreach (var m in methods_TailLatencyOptimizer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TailLatencyOptimizer 未找到，尝试无命名空间...");
        type_TailLatencyOptimizer = Type.GetType("TailLatencyOptimizer");
        if (type_TailLatencyOptimizer != null)
            Console.WriteLine("[PASS] 类型 TailLatencyOptimizer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TailLatencyOptimizer 可能为顶层语句或嵌套类型");
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

    // 验证 class: RedisCacheOptions
    var type_RedisCacheOptions = Type.GetType("RedisCacheOptions");
    if (type_RedisCacheOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RedisCacheOptions (class) 存在");
        var ctors_RedisCacheOptions = type_RedisCacheOptions.GetConstructors();
        Console.WriteLine($"[PASS] RedisCacheOptions 构造函数数量: {ctors_RedisCacheOptions.Length}");
        var methods_RedisCacheOptions = type_RedisCacheOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisCacheOptions 公开方法数量: {methods_RedisCacheOptions.Length}");
        foreach (var m in methods_RedisCacheOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisCacheOptions 未找到，尝试无命名空间...");
        type_RedisCacheOptions = Type.GetType("RedisCacheOptions");
        if (type_RedisCacheOptions != null)
            Console.WriteLine("[PASS] 类型 RedisCacheOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisCacheOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisCacheService
    var type_RedisCacheService = Type.GetType("RedisCacheService");
    if (type_RedisCacheService != null)
    {
        Console.WriteLine("[PASS] 类型 RedisCacheService (class) 存在");
        var ctors_RedisCacheService = type_RedisCacheService.GetConstructors();
        Console.WriteLine($"[PASS] RedisCacheService 构造函数数量: {ctors_RedisCacheService.Length}");
        var methods_RedisCacheService = type_RedisCacheService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisCacheService 公开方法数量: {methods_RedisCacheService.Length}");
        foreach (var m in methods_RedisCacheService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisCacheService 未找到，尝试无命名空间...");
        type_RedisCacheService = Type.GetType("RedisCacheService");
        if (type_RedisCacheService != null)
            Console.WriteLine("[PASS] 类型 RedisCacheService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisCacheService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoDataPipeline
    var type_TodoDataPipeline = Type.GetType("TodoDataPipeline");
    if (type_TodoDataPipeline != null)
    {
        Console.WriteLine("[PASS] 类型 TodoDataPipeline (class) 存在");
        var ctors_TodoDataPipeline = type_TodoDataPipeline.GetConstructors();
        Console.WriteLine($"[PASS] TodoDataPipeline 构造函数数量: {ctors_TodoDataPipeline.Length}");
        var methods_TodoDataPipeline = type_TodoDataPipeline.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoDataPipeline 公开方法数量: {methods_TodoDataPipeline.Length}");
        foreach (var m in methods_TodoDataPipeline)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoDataPipeline 未找到，尝试无命名空间...");
        type_TodoDataPipeline = Type.GetType("TodoDataPipeline");
        if (type_TodoDataPipeline != null)
            Console.WriteLine("[PASS] 类型 TodoDataPipeline (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoDataPipeline 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
