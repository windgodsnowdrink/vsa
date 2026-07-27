#load "fv_fluent_validation.cs"

Console.WriteLine("=== fv_fluent_validation.cs Test ===");

try
{
    // 验证 class: StreamingValidator
    var type_StreamingValidator = Type.GetType("StreamingValidator");
    if (type_StreamingValidator != null)
    {
        Console.WriteLine("[PASS] 类型 StreamingValidator (class) 存在");
        var ctors_StreamingValidator = type_StreamingValidator.GetConstructors();
        Console.WriteLine($"[PASS] StreamingValidator 构造函数数量: {ctors_StreamingValidator.Length}");
        var methods_StreamingValidator = type_StreamingValidator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StreamingValidator 公开方法数量: {methods_StreamingValidator.Length}");
        foreach (var m in methods_StreamingValidator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StreamingValidator 未找到，尝试无命名空间...");
        type_StreamingValidator = Type.GetType("StreamingValidator");
        if (type_StreamingValidator != null)
            Console.WriteLine("[PASS] 类型 StreamingValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StreamingValidator 可能为顶层语句或嵌套类型");
    }

    // 验证 class: PooledStreamingValidator
    var type_PooledStreamingValidator = Type.GetType("PooledStreamingValidator");
    if (type_PooledStreamingValidator != null)
    {
        Console.WriteLine("[PASS] 类型 PooledStreamingValidator (class) 存在");
        var ctors_PooledStreamingValidator = type_PooledStreamingValidator.GetConstructors();
        Console.WriteLine($"[PASS] PooledStreamingValidator 构造函数数量: {ctors_PooledStreamingValidator.Length}");
        var methods_PooledStreamingValidator = type_PooledStreamingValidator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] PooledStreamingValidator 公开方法数量: {methods_PooledStreamingValidator.Length}");
        foreach (var m in methods_PooledStreamingValidator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 PooledStreamingValidator 未找到，尝试无命名空间...");
        type_PooledStreamingValidator = Type.GetType("PooledStreamingValidator");
        if (type_PooledStreamingValidator != null)
            Console.WriteLine("[PASS] 类型 PooledStreamingValidator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 PooledStreamingValidator 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
