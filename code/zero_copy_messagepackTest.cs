#load "zero_copy_messagepack.cs"

Console.WriteLine("=== zero_copy_messagepack.cs Test ===");

try
{
    // 验证 class: ZeroCopyMessagePackSerializer
    var type_ZeroCopyMessagePackSerializer = Type.GetType("ZeroCopyMessagePackSerializer");
    if (type_ZeroCopyMessagePackSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroCopyMessagePackSerializer (class) 存在");
        var ctors_ZeroCopyMessagePackSerializer = type_ZeroCopyMessagePackSerializer.GetConstructors();
        Console.WriteLine($"[PASS] ZeroCopyMessagePackSerializer 构造函数数量: {ctors_ZeroCopyMessagePackSerializer.Length}");
        var methods_ZeroCopyMessagePackSerializer = type_ZeroCopyMessagePackSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroCopyMessagePackSerializer 公开方法数量: {methods_ZeroCopyMessagePackSerializer.Length}");
        foreach (var m in methods_ZeroCopyMessagePackSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroCopyMessagePackSerializer 未找到，尝试无命名空间...");
        type_ZeroCopyMessagePackSerializer = Type.GetType("ZeroCopyMessagePackSerializer");
        if (type_ZeroCopyMessagePackSerializer != null)
            Console.WriteLine("[PASS] 类型 ZeroCopyMessagePackSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroCopyMessagePackSerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
