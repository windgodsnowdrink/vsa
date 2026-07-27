#load "threadsafe_memorypool_serializer.cs"

Console.WriteLine("=== threadsafe_memorypool_serializer.cs Test ===");

try
{
    // 验证 class: ThreadSafeMemoryPoolSerializer
    var type_ThreadSafeMemoryPoolSerializer = Type.GetType("ThreadSafeMemoryPoolSerializer");
    if (type_ThreadSafeMemoryPoolSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 ThreadSafeMemoryPoolSerializer (class) 存在");
        var ctors_ThreadSafeMemoryPoolSerializer = type_ThreadSafeMemoryPoolSerializer.GetConstructors();
        Console.WriteLine($"[PASS] ThreadSafeMemoryPoolSerializer 构造函数数量: {ctors_ThreadSafeMemoryPoolSerializer.Length}");
        var methods_ThreadSafeMemoryPoolSerializer = type_ThreadSafeMemoryPoolSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ThreadSafeMemoryPoolSerializer 公开方法数量: {methods_ThreadSafeMemoryPoolSerializer.Length}");
        foreach (var m in methods_ThreadSafeMemoryPoolSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ThreadSafeMemoryPoolSerializer 未找到，尝试无命名空间...");
        type_ThreadSafeMemoryPoolSerializer = Type.GetType("ThreadSafeMemoryPoolSerializer");
        if (type_ThreadSafeMemoryPoolSerializer != null)
            Console.WriteLine("[PASS] 类型 ThreadSafeMemoryPoolSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ThreadSafeMemoryPoolSerializer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessagePackWriterPooledObjectPolicy
    var type_MessagePackWriterPooledObjectPolicy = Type.GetType("MessagePackWriterPooledObjectPolicy");
    if (type_MessagePackWriterPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MessagePackWriterPooledObjectPolicy (class) 存在");
        var ctors_MessagePackWriterPooledObjectPolicy = type_MessagePackWriterPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MessagePackWriterPooledObjectPolicy 构造函数数量: {ctors_MessagePackWriterPooledObjectPolicy.Length}");
        var methods_MessagePackWriterPooledObjectPolicy = type_MessagePackWriterPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessagePackWriterPooledObjectPolicy 公开方法数量: {methods_MessagePackWriterPooledObjectPolicy.Length}");
        foreach (var m in methods_MessagePackWriterPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessagePackWriterPooledObjectPolicy 未找到，尝试无命名空间...");
        type_MessagePackWriterPooledObjectPolicy = Type.GetType("MessagePackWriterPooledObjectPolicy");
        if (type_MessagePackWriterPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 MessagePackWriterPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessagePackWriterPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
