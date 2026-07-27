#load "shared_memory_messagepack.cs"

Console.WriteLine("=== shared_memory_messagepack.cs Test ===");

try
{
    // 验证 class: SharedMemoryMessagePackSerializer
    var type_SharedMemoryMessagePackSerializer = Type.GetType("SharedMemoryMessagePackSerializer");
    if (type_SharedMemoryMessagePackSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 SharedMemoryMessagePackSerializer (class) 存在");
        var ctors_SharedMemoryMessagePackSerializer = type_SharedMemoryMessagePackSerializer.GetConstructors();
        Console.WriteLine($"[PASS] SharedMemoryMessagePackSerializer 构造函数数量: {ctors_SharedMemoryMessagePackSerializer.Length}");
        var methods_SharedMemoryMessagePackSerializer = type_SharedMemoryMessagePackSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SharedMemoryMessagePackSerializer 公开方法数量: {methods_SharedMemoryMessagePackSerializer.Length}");
        foreach (var m in methods_SharedMemoryMessagePackSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SharedMemoryMessagePackSerializer 未找到，尝试无命名空间...");
        type_SharedMemoryMessagePackSerializer = Type.GetType("SharedMemoryMessagePackSerializer");
        if (type_SharedMemoryMessagePackSerializer != null)
            Console.WriteLine("[PASS] 类型 SharedMemoryMessagePackSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SharedMemoryMessagePackSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
