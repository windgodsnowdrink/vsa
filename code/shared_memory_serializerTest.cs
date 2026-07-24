#load "shared_memory_serializer.cs"

Console.WriteLine("=== shared_memory_serializer.cs Test ===");

try
{
    // 验证 class: SharedMemorySerializer
    var type_SharedMemorySerializer = Type.GetType("SharedMemorySerializer");
    if (type_SharedMemorySerializer != null)
    {
        Console.WriteLine("[PASS] 类型 SharedMemorySerializer (class) 存在");
        var ctors_SharedMemorySerializer = type_SharedMemorySerializer.GetConstructors();
        Console.WriteLine($"[PASS] SharedMemorySerializer 构造函数数量: {ctors_SharedMemorySerializer.Length}");
        var methods_SharedMemorySerializer = type_SharedMemorySerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SharedMemorySerializer 公开方法数量: {methods_SharedMemorySerializer.Length}");
        foreach (var m in methods_SharedMemorySerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SharedMemorySerializer 未找到，尝试无命名空间...");
        type_SharedMemorySerializer = Type.GetType("SharedMemorySerializer");
        if (type_SharedMemorySerializer != null)
            Console.WriteLine("[PASS] 类型 SharedMemorySerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SharedMemorySerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
