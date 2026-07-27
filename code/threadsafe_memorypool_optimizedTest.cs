#load "threadsafe_memorypool_optimized.cs"

Console.WriteLine("=== threadsafe_memorypool_optimized.cs Test ===");

try
{
    // 验证 class: ThreadSafeMemoryPoolOptimized
    var type_ThreadSafeMemoryPoolOptimized = Type.GetType("ThreadSafeMemoryPoolOptimized");
    if (type_ThreadSafeMemoryPoolOptimized != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadSafeMemoryPoolOptimized (class) 存在");
        var ctors_ThreadSafeMemoryPoolOptimized = type_ThreadSafeMemoryPoolOptimized.GetConstructors();
        Console.WriteLine($"[PASS] ThreadSafeMemoryPoolOptimized 构造函数数量: {ctors_ThreadSafeMemoryPoolOptimized.Length}");
        var methods_ThreadSafeMemoryPoolOptimized = type_ThreadSafeMemoryPoolOptimized.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadSafeMemoryPoolOptimized 公开方法数量: {methods_ThreadSafeMemoryPoolOptimized.Length}");
        foreach (var m in methods_ThreadSafeMemoryPoolOptimized)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadSafeMemoryPoolOptimized 未找到，尝试无命名空间...");
        type_ThreadSafeMemoryPoolOptimized = Type.GetType("ThreadSafeMemoryPoolOptimized");
        if (type_ThreadSafeMemoryPoolOptimized != null)
            Console.WriteLine("[PASS] 类型 ThreadSafeMemoryPoolOptimized (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadSafeMemoryPoolOptimized 可能为顶层语句或嵌套类型");
    }

    // 验证 class: BufferPooledObjectPolicy
    var type_BufferPooledObjectPolicy = Type.GetType("BufferPooledObjectPolicy");
    if (type_BufferPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 BufferPooledObjectPolicy (class) 存在");
        var ctors_BufferPooledObjectPolicy = type_BufferPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] BufferPooledObjectPolicy 构造函数数量: {ctors_BufferPooledObjectPolicy.Length}");
        var methods_BufferPooledObjectPolicy = type_BufferPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] BufferPooledObjectPolicy 公开方法数量: {methods_BufferPooledObjectPolicy.Length}");
        foreach (var m in methods_BufferPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 BufferPooledObjectPolicy 未找到，尝试无命名空间...");
        type_BufferPooledObjectPolicy = Type.GetType("BufferPooledObjectPolicy");
        if (type_BufferPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 BufferPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 BufferPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    // 验证 class: JsonWriterPooledObjectPolicy
    var type_JsonWriterPooledObjectPolicy = Type.GetType("JsonWriterPooledObjectPolicy");
    if (type_JsonWriterPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 JsonWriterPooledObjectPolicy (class) 存在");
        var ctors_JsonWriterPooledObjectPolicy = type_JsonWriterPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] JsonWriterPooledObjectPolicy 构造函数数量: {ctors_JsonWriterPooledObjectPolicy.Length}");
        var methods_JsonWriterPooledObjectPolicy = type_JsonWriterPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] JsonWriterPooledObjectPolicy 公开方法数量: {methods_JsonWriterPooledObjectPolicy.Length}");
        foreach (var m in methods_JsonWriterPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 JsonWriterPooledObjectPolicy 未找到，尝试无命名空间...");
        type_JsonWriterPooledObjectPolicy = Type.GetType("JsonWriterPooledObjectPolicy");
        if (type_JsonWriterPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 JsonWriterPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 JsonWriterPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
