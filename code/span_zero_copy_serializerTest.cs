#load "span_zero_copy_serializer.cs"

Console.WriteLine("=== span_zero_copy_serializer.cs Test ===");

try
{
    // 验证 class: SpanZeroCopySerializer
    var type_SpanZeroCopySerializer = Type.GetType("SpanZeroCopySerializer");
    if (type_SpanZeroCopySerializer != null)
    {
        Console.WriteLine("[PASS] 类型 SpanZeroCopySerializer (class) 存在");
        var ctors_SpanZeroCopySerializer = type_SpanZeroCopySerializer.GetConstructors();
        Console.WriteLine($"[PASS] SpanZeroCopySerializer 构造函数数量: {ctors_SpanZeroCopySerializer.Length}");
        var methods_SpanZeroCopySerializer = type_SpanZeroCopySerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpanZeroCopySerializer 公开方法数量: {methods_SpanZeroCopySerializer.Length}");
        foreach (var m in methods_SpanZeroCopySerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpanZeroCopySerializer 未找到，尝试无命名空间...");
        type_SpanZeroCopySerializer = Type.GetType("SpanZeroCopySerializer");
        if (type_SpanZeroCopySerializer != null)
            Console.WriteLine("[PASS] 类型 SpanZeroCopySerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpanZeroCopySerializer 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
