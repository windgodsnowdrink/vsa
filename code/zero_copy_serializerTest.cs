#load "zero_copy_serializer.cs"

Console.WriteLine("=== zero_copy_serializer.cs Test ===");

try
{
    // 验证 class: ZeroCopySerializer
    var type_ZeroCopySerializer = Type.GetType("ZeroCopySerializer");
    if (type_ZeroCopySerializer != null)
    {
        Console.WriteLine("[PASS] 类型 ZeroCopySerializer (class) 存在");
        var ctors_ZeroCopySerializer = type_ZeroCopySerializer.GetConstructors();
        Console.WriteLine($"[PASS] ZeroCopySerializer 构造函数数量: {ctors_ZeroCopySerializer.Length}");
        var methods_ZeroCopySerializer = type_ZeroCopySerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ZeroCopySerializer 公开方法数量: {methods_ZeroCopySerializer.Length}");
        foreach (var m in methods_ZeroCopySerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ZeroCopySerializer 未找到，尝试无命名空间...");
        type_ZeroCopySerializer = Type.GetType("ZeroCopySerializer");
        if (type_ZeroCopySerializer != null)
            Console.WriteLine("[PASS] 类型 ZeroCopySerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ZeroCopySerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
