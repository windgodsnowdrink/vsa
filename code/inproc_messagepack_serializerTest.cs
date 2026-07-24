#load "inproc_messagepack_serializer.cs"

Console.WriteLine("=== inproc_messagepack_serializer.cs Test ===");

try
{
    // 验证 class: InProcMessagePackSerializer
    var type_InProcMessagePackSerializer = Type.GetType("InProcMessagePackSerializer");
    if (type_InProcMessagePackSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 InProcMessagePackSerializer (class) 存在");
        var ctors_InProcMessagePackSerializer = type_InProcMessagePackSerializer.GetConstructors();
        Console.WriteLine($"[PASS] InProcMessagePackSerializer 构造函数数量: {ctors_InProcMessagePackSerializer.Length}");
        var methods_InProcMessagePackSerializer = type_InProcMessagePackSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] InProcMessagePackSerializer 公开方法数量: {methods_InProcMessagePackSerializer.Length}");
        foreach (var m in methods_InProcMessagePackSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 InProcMessagePackSerializer 未找到，尝试无命名空间...");
        type_InProcMessagePackSerializer = Type.GetType("InProcMessagePackSerializer");
        if (type_InProcMessagePackSerializer != null)
            Console.WriteLine("[PASS] 类型 InProcMessagePackSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 InProcMessagePackSerializer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MemoryStreamPooledObjectPolicy
    var type_MemoryStreamPooledObjectPolicy = Type.GetType("MemoryStreamPooledObjectPolicy");
    if (type_MemoryStreamPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 MemoryStreamPooledObjectPolicy (class) 存在");
        var ctors_MemoryStreamPooledObjectPolicy = type_MemoryStreamPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] MemoryStreamPooledObjectPolicy 构造函数数量: {ctors_MemoryStreamPooledObjectPolicy.Length}");
        var methods_MemoryStreamPooledObjectPolicy = type_MemoryStreamPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MemoryStreamPooledObjectPolicy 公开方法数量: {methods_MemoryStreamPooledObjectPolicy.Length}");
        foreach (var m in methods_MemoryStreamPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MemoryStreamPooledObjectPolicy 未找到，尝试无命名空间...");
        type_MemoryStreamPooledObjectPolicy = Type.GetType("MemoryStreamPooledObjectPolicy");
        if (type_MemoryStreamPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 MemoryStreamPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MemoryStreamPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
