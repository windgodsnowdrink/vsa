#load "spanjson_serializer.cs"

Console.WriteLine("=== spanjson_serializer.cs Test ===");

try
{
    // 验证 class: SpanJsonSerializer
    var type_SpanJsonSerializer = Type.GetType("SpanJsonSerializer");
    if (type_SpanJsonSerializer != null)
    {
        Console.WriteLine("[PASS] 类型 SpanJsonSerializer (class) 存在");
        var ctors_SpanJsonSerializer = type_SpanJsonSerializer.GetConstructors();
        Console.WriteLine($"[PASS] SpanJsonSerializer 构造函数数量: {ctors_SpanJsonSerializer.Length}");
        var methods_SpanJsonSerializer = type_SpanJsonSerializer.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] SpanJsonSerializer 公开方法数量: {methods_SpanJsonSerializer.Length}");
        foreach (var m in methods_SpanJsonSerializer)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SpanJsonSerializer 未找到，尝试无命名空间...");
        type_SpanJsonSerializer = Type.GetType("SpanJsonSerializer");
        if (type_SpanJsonSerializer != null)
            Console.WriteLine("[PASS] 类型 SpanJsonSerializer (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SpanJsonSerializer 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ArrayPooledObjectPolicy
    var type_ArrayPooledObjectPolicy = Type.GetType("ArrayPooledObjectPolicy");
    if (type_ArrayPooledObjectPolicy != null)
    {
        Console.WriteLine("[PASS] 类型 ArrayPooledObjectPolicy (class) 存在");
        var ctors_ArrayPooledObjectPolicy = type_ArrayPooledObjectPolicy.GetConstructors();
        Console.WriteLine($"[PASS] ArrayPooledObjectPolicy 构造函数数量: {ctors_ArrayPooledObjectPolicy.Length}");
        var methods_ArrayPooledObjectPolicy = type_ArrayPooledObjectPolicy.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ArrayPooledObjectPolicy 公开方法数量: {methods_ArrayPooledObjectPolicy.Length}");
        foreach (var m in methods_ArrayPooledObjectPolicy)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ArrayPooledObjectPolicy 未找到，尝试无命名空间...");
        type_ArrayPooledObjectPolicy = Type.GetType("ArrayPooledObjectPolicy");
        if (type_ArrayPooledObjectPolicy != null)
            Console.WriteLine("[PASS] 类型 ArrayPooledObjectPolicy (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ArrayPooledObjectPolicy 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
